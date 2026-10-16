using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Aethel.WorldMemory
{
    internal sealed class ChoiceDefinition
    {
        internal readonly string Id, Affinity, Label, Immediate, Delayed;
        internal ChoiceDefinition(string id, string affinity, string label, string immediate, string delayed)
        { Id = id; Affinity = affinity; Label = label; Immediate = immediate; Delayed = delayed; }
    }
    internal sealed class QuestDefinition
    {
        internal readonly int Order;
        internal readonly string Zone, Id, Trigger;
        internal readonly ChoiceDefinition[] Choices;
        internal QuestDefinition(int order, string zone, string id, string trigger, ChoiceDefinition[] choices)
        { Order = order; Zone = zone; Id = id; Trigger = trigger; Choices = choices; }
    }

    // Save contains only bounded domain IDs. No scripts, assets, text or pending effect payloads.
    [DataContract]
    public sealed class MemorySave
    {
        [DataMember(IsRequired = true)] public int Schema = 1;
        [DataMember(IsRequired = true)] public string CampaignId;
        [DataMember(IsRequired = true)] public long Version;
        [DataMember(IsRequired = true)] public Dictionary<string, string> Choices = new Dictionary<string, string>();
        [DataMember(IsRequired = true)] public List<string> Encounters = new List<string>();
        [DataMember(IsRequired = true)] public List<string> Clues = new List<string>();
        [DataMember(IsRequired = true)] public List<string> Entered = new List<string>();
        [DataMember(IsRequired = true)] public List<string> Revealed = new List<string>();
        internal MemorySave Copy()
        {
            return new MemorySave { CampaignId = CampaignId, Version = Version,
                Choices = new Dictionary<string, string>(Choices), Encounters = new List<string>(Encounters),
                Clues = new List<string>(Clues), Entered = new List<string>(Entered), Revealed = new List<string>(Revealed) };
        }
        internal bool Unlocked(int order)
        {
            return CanonicalCampaign.Quests.Take(order - 1).All(q => Choices.ContainsKey(q.Id) && Encounters.Contains(q.Zone));
        }
        internal bool Completed(int order)
        {
            var q = CanonicalCampaign.Quests[order - 1];
            return Choices.ContainsKey(q.Id) && Encounters.Contains(q.Zone);
        }
        public void Validate()
        {
            if (Schema != 1 || string.IsNullOrWhiteSpace(CampaignId) || CampaignId.Length > 128 ||
                !CampaignId.All(c => char.IsLetterOrDigit(c) && c <= 127 || "_.:-".Contains(c)) ||
                Version < 0 || Version == long.MaxValue || Choices == null || Encounters == null || Clues == null || Entered == null || Revealed == null)
                throw new SerializationException("Invalid memory header");
            if (Choices.Count > 12 || Encounters.Count > 12 || Clues.Count > 3 || Revealed.Count > 12 || Entered.Count > 12 || Entered.Distinct().Count() != Entered.Count ||
                Encounters.Distinct().Count() != Encounters.Count || Clues.Distinct().Count() != Clues.Count ||
                Revealed.Distinct().Count() != Revealed.Count || Clues.Any(c => !MemoryService.ForestClues.Contains(c)))
                throw new SerializationException("Invalid memory collection");
            foreach (var zone in Encounters.Concat(Entered))
            {
                var q = CanonicalCampaign.Quests.FirstOrDefault(x => x.Zone == zone);
                if (q == null || !Unlocked(q.Order)) throw new SerializationException("Invalid encounter");
            }
            foreach (var pair in Choices)
            {
                var q = CanonicalCampaign.Quests.FirstOrDefault(x => x.Id == pair.Key);
                if (q == null || !q.Choices.Any(c => c.Id == pair.Value) || !Unlocked(q.Order) ||
                    (q.Order % 3 == 0 && !Encounters.Contains(q.Zone)) || (q.Order == 1 && Clues.Count != 3))
                    throw new SerializationException("Invalid choice");
            }
            foreach (var id in Revealed)
            {
                var q = CanonicalCampaign.Quests.FirstOrDefault(x => x.Id == id);
                if (q == null || !Choices.ContainsKey(id) || !TriggerReached(q.Trigger))
                    throw new SerializationException("Invalid revealed effect");
            }
        }
        internal bool TriggerReached(string trigger)
        {
            // Return is a transaction supplied by the trusted scene adapter, not a save flag.
            if (trigger == "first_return_to_citadel") return true;
            if (trigger == "enter_crypt_06") return Entered.Contains("crypt_06");
            if (trigger == "enter_heart_12") return Entered.Contains("heart_12");
            if (trigger == "act4_pre_final_checkpoint") return Completed(11);
            if (trigger == "campaign_epilogue") return Completed(12);
            for (int act = 1; act <= 3; act++)
                if (trigger == "act" + act + "_completion") return Completed(act * 3);
            return false;
        }
    }

    // Implementations must atomically compare persisted version and write candidate.
    // False/exception MUST mean the previous authoritative state remains intact.
    public interface IMemoryStore
    {
        MemorySave Load();
        bool TryCommit(long expectedVersion, MemorySave candidate);
    }
    public enum CommandStatus { Accepted, Replayed, Invalid, Locked, Conflict, PersistenceFailed }
    public sealed class MemoryFact
    {
        public string QuestId { get; private set; }
        public string ChoiceId { get; private set; }
        public string Text { get; private set; }
        public bool IsDelayed { get; private set; }
        internal MemoryFact(string quest, string choice, string text, bool delayed)
        { QuestId = quest; ChoiceId = choice; Text = text; IsDelayed = delayed; }
    }
    public sealed class MemoryView
    {
        public long Version { get; internal set; }
        public int Stage { get; internal set; }
        public bool IsEpilogue { get; internal set; }
        public int Mercy { get; internal set; }
        public int Judgment { get; internal set; }
        public string Predominance { get { return Mercy == Judgment ? "mixed" : Mercy > Judgment ? "mercy" : "judgment"; } }
        public MemoryFact[] Facts { get; internal set; }
        // Presentation only. This service never disables or mutates hub services.
    }
    public sealed class CommandResult
    {
        public CommandStatus Status { get; internal set; }
        public MemoryView View { get; internal set; }
    }
    public sealed class MemoryService
    {
        internal static readonly string[] ForestClues = { "forest_01.clue.tracks", "forest_01.clue.testimony", "forest_01.clue.well" };
        private readonly IMemoryStore store;
        public MemoryService(IMemoryStore store) { this.store = store ?? throw new ArgumentNullException("store"); }
        public MemoryView Read() { var s = store.Load(); s.Validate(); return Project(s); }
        private static MemoryView Project(MemorySave s)
        {
            var facts = new List<MemoryFact>(); int mercy = 0, judgment = 0;
            foreach (var q in CanonicalCampaign.Quests)
            {
                if (!s.Choices.ContainsKey(q.Id)) continue;
                var c = q.Choices.Single(x => x.Id == s.Choices[q.Id]);
                if (c.Affinity == "mercy") mercy++; else judgment++;
                facts.Add(new MemoryFact(q.Id, c.Id, c.Immediate, false));
                if (s.Revealed.Contains(q.Id)) facts.Add(new MemoryFact(q.Id, c.Id, c.Delayed, true));
            }
            int stage = s.Completed(11) ? 4 : s.Completed(9) ? 3 : s.Completed(6) ? 2 : s.Completed(3) ? 1 : 0;
            return new MemoryView { Version = s.Version, Stage = stage, IsEpilogue = s.Revealed.Contains("act4.heart_fate"), Mercy = mercy, Judgment = judgment, Facts = facts.ToArray() };
        }
        private CommandResult Execute(long expected, Func<MemorySave, CommandStatus> change)
        {
            var original = store.Load(); original.Validate(); var candidate = original.Copy();
            var status = change(candidate);
            if (status == CommandStatus.Accepted && original.Version != expected) status = CommandStatus.Conflict;
            if (status == CommandStatus.Accepted)
            {
                candidate.Version++; candidate.Validate();
                try { if (!store.TryCommit(expected, candidate.Copy())) status = CommandStatus.Conflict; }
                catch (System.IO.IOException) { status = CommandStatus.PersistenceFailed; }
                catch (UnauthorizedAccessException) { status = CommandStatus.PersistenceFailed; }
            }
            // Publish only data actually confirmed by the repository; no event emitted before persistence.
            return new CommandResult { Status = status, View = Read() };
        }
        public CommandResult ObserveClue(string clueId, long expectedVersion)
        {
            return Execute(expectedVersion, s => {
                if (!ForestClues.Contains(clueId)) return CommandStatus.Invalid;
                if (s.Clues.Contains(clueId)) return CommandStatus.Replayed;
                s.Clues.Add(clueId); return CommandStatus.Accepted;
            });
        }
        public CommandResult ConfirmEncounter(string zoneId, long expectedVersion)
        {
            return Execute(expectedVersion, s => {
                var q = CanonicalCampaign.Quests.FirstOrDefault(x => x.Zone == zoneId);
                if (q == null) return CommandStatus.Invalid;
                if (s.Encounters.Contains(zoneId)) return CommandStatus.Replayed;
                if (!s.Unlocked(q.Order)) return CommandStatus.Locked;
                s.Encounters.Add(zoneId); return CommandStatus.Accepted;
            });
        }
        public CommandResult Decide(string questId, string choiceId, long expectedVersion)
        {
            return Execute(expectedVersion, s => {
                var q = CanonicalCampaign.Quests.FirstOrDefault(x => x.Id == questId);
                if (q == null || !q.Choices.Any(c => c.Id == choiceId)) return CommandStatus.Invalid;
                if (s.Choices.ContainsKey(questId)) return s.Choices[questId] == choiceId ? CommandStatus.Replayed : CommandStatus.Conflict;
                if (!s.Unlocked(q.Order) || (q.Order == 1 && s.Clues.Count != 3) ||
                    (q.Order % 3 == 0 && !s.Encounters.Contains(q.Zone))) return CommandStatus.Locked;
                s.Choices.Add(questId, choiceId); return CommandStatus.Accepted;
            });
        }
        public CommandResult EnterZone(string zoneId, long expectedVersion)
        {
            return Execute(expectedVersion, s => {
                var q = CanonicalCampaign.Quests.FirstOrDefault(x => x.Zone == zoneId);
                if (q == null) return CommandStatus.Invalid;
                if (s.Entered.Contains(zoneId)) return CommandStatus.Replayed;
                if (!s.Unlocked(q.Order)) return CommandStatus.Locked;
                s.Entered.Add(zoneId); return CommandStatus.Accepted;
            });
        }
        private static CommandStatus RevealReached(MemorySave s)
        {
            bool changed = false;
            foreach (var q in CanonicalCampaign.Quests)
                if (s.Choices.ContainsKey(q.Id) && !s.Revealed.Contains(q.Id) && s.TriggerReached(q.Trigger))
                { s.Revealed.Add(q.Id); changed = true; }
            return changed ? CommandStatus.Accepted : CommandStatus.Replayed;
        }
        public CommandResult VisitCitadel(long expectedVersion)
        {
            return Execute(expectedVersion, RevealReached);
        }
        public CommandResult RevealEpilogue(long expectedVersion)
        {
            return Execute(expectedVersion, s => s.Completed(12) ? RevealReached(s) : CommandStatus.Locked);
        }
    }
}
