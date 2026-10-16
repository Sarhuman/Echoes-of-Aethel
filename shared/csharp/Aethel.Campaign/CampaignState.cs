using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Aethel.WorldMemory;

namespace Aethel.Campaign
{
    // New prototype format; never overwrite a Godot save or isolated memory JSON.
    [DataContract]
    public sealed class CampaignState
    {
        [DataMember(IsRequired = true)] public int Schema = 1;
        [DataMember(IsRequired = true)] public string CharacterId;
        [DataMember(IsRequired = true)] public string ClassId;
        [DataMember(IsRequired = true)] public int Level = 1;
        [DataMember(IsRequired = true)] public int Gold;
        [DataMember(IsRequired = true)] public int RespecCount;
        [DataMember(IsRequired = true)] public bool NoticeDismissed;
        [DataMember(IsRequired = true)] public string PrimaryPath = "";
        [DataMember(IsRequired = true)] public List<string> Nodes = new List<string>();
        [DataMember(IsRequired = true)] public bool ForestRewardClaimed;
        [DataMember(IsRequired = true)] public int ReservedHealthPotions;
        [DataMember(IsRequired = true)] public Dictionary<string, string> Commands = new Dictionary<string, string>();
        [DataMember(IsRequired = true)] public MemorySave Memory;
        public long Version { get { return Memory.Version; } }
        public int RitualCharge { get { return Memory.Choices.Count; } }
        public int EarnedSpecializationPoints { get { return SpecializationCatalog.EarnedPoints(Level); } }
        public int UnspentSpecializationPoints { get { return EarnedSpecializationPoints - Nodes.Count; } }
        public static CampaignState Create(string campaignId, string characterId, string classId)
        {
            var s = new CampaignState { CharacterId = characterId, ClassId = classId,
                Memory = new MemorySave { CampaignId = campaignId } };
            s.Validate(); return s;
        }
        internal CampaignState Copy()
        {
            return new CampaignState { Schema = Schema, CharacterId = CharacterId, ClassId = ClassId,
                Level = Level, Gold = Gold, RespecCount = RespecCount, NoticeDismissed = NoticeDismissed,
                PrimaryPath = PrimaryPath, Nodes = new List<string>(Nodes), ForestRewardClaimed = ForestRewardClaimed,
                ReservedHealthPotions = ReservedHealthPotions, Commands = new Dictionary<string, string>(Commands), Memory = Memory.Copy() };
        }
        internal void AssertExtends(CampaignState previous)
        {
            if (CharacterId != previous.CharacterId || ClassId != previous.ClassId ||
                Memory.CampaignId != previous.Memory.CampaignId || Level < previous.Level ||
                (previous.NoticeDismissed && !NoticeDismissed) || RespecCount < previous.RespecCount ||
                (previous.ForestRewardClaimed && !ForestRewardClaimed) ||
                previous.Memory.Choices.Any(p => !Memory.Choices.ContainsKey(p.Key) || Memory.Choices[p.Key] != p.Value) ||
                previous.Memory.Clues.Except(Memory.Clues).Any() || previous.Memory.Encounters.Except(Memory.Encounters).Any() ||
                previous.Memory.Entered.Except(Memory.Entered).Any() || previous.Memory.Revealed.Except(Memory.Revealed).Any() ||
                previous.Commands.Any(p => !Commands.ContainsKey(p.Key) || Commands[p.Key] != p.Value))
                throw new SerializationException("Campaign transaction would erase confirmed history");
        }
        private static bool ValidId(string id)
        {
            return !string.IsNullOrEmpty(id) && id.Length <= 128 && id.All(c =>
                (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || "_.:-".Contains(c));
        }
        public void Validate()
        {
            if (Schema != 1 || !ValidId(CharacterId) || !SpecializationCatalog.Classes.Contains(ClassId) ||
                Level < 1 || Level > 30 || Gold < 0 || RespecCount < 0 || RespecCount == int.MaxValue ||
                Memory == null || Nodes == null || Commands == null || Commands.Count > 512 || Nodes.Count > 6 || PrimaryPath == null ||
                ReservedHealthPotions < 0 || ReservedHealthPotions > CampaignContent.FirstQuestHealthPotions)
                throw new SerializationException("Invalid campaign state");
            Memory.Validate();
            if ((NoticeDismissed && Level < 5) || (RespecCount > 0 && PrimaryPath == ""))
                throw new SerializationException("Invalid progression flags");
            if (Commands.Any(p => !ValidId(p.Key) || string.IsNullOrEmpty(p.Value) || p.Value.Length > 2048))
                throw new SerializationException("Invalid command receipt");
            bool forestChosen = Memory.Choices.ContainsKey("act1.spy_hunt");
            if (ForestRewardClaimed != forestChosen || (!forestChosen && ReservedHealthPotions != 0) ||
                (forestChosen && Memory.Choices["act1.spy_hunt"] == "judge_suspects" && ReservedHealthPotions != 0))
                throw new SerializationException("Choice/reward mismatch");
            if (Nodes.Distinct().Count() != Nodes.Count || Nodes.Count > EarnedSpecializationPoints ||
                (PrimaryPath == "" && Nodes.Count != 0) || (PrimaryPath != "" &&
                 (!SpecializationCatalog.Paths(ClassId).Contains(PrimaryPath) || Level < 5 || !Nodes.Contains(PrimaryPath + ".t1"))))
                throw new SerializationException("Invalid specialization selection");
            foreach (string id in Nodes)
            {
                var node = SpecializationCatalog.Find(id);
                if (node == null || node.ClassId != ClassId || node.RequiredLevel > Level ||
                    (node.Tier > 1 && !Nodes.Contains(node.PathId + ".t" + (node.Tier - 1))) ||
                    (node.Tier == 1 && node.PathId != PrimaryPath && Level < 10))
                    throw new SerializationException("Invalid specialization node");
            }
        }
    }
    public sealed class SpecializationNode
    {
        public string Id { get; internal set; }
        public string PathId { get; internal set; }
        public string ClassId { get; internal set; }
        public int Tier { get; internal set; }
        public int RequiredLevel { get; internal set; }
        // Allocation prototype only; higher-tier effects are NOT implemented.
    }
    public static class SpecializationCatalog
    {
        public const int RespecGoldPerLevel = 5;
        private static readonly int[] PointLevels = { 5, 10, 15, 20, 25, 30 };
        private static readonly int[] NodeLevels = { 5, 10, 20, 30 };
        public static int EarnedPoints(int level) { return PointLevels.Count(x => x <= level); }
        internal static readonly string[] Classes = { "twilight_bastion", "rune_weaver", "shadow_sentinel", "thorn_summoner", "sacred_flame_inquisitor" };
        private static readonly string[][] Branches = {
            new[] { "defense", "rupture" }, new[] { "control", "burst" }, new[] { "precision", "ambush" },
            new[] { "command", "sacrifice" }, new[] { "protection", "judgment" } };
        public static string[] Paths(string classId)
        {
            int i = Array.IndexOf(Classes, classId);
            return i < 0 ? new string[0] : Branches[i].Select(x => classId + "." + x).ToArray();
        }
        public static SpecializationNode Find(string id)
        {
            foreach (string cls in Classes)
                foreach (string path in Paths(cls))
                    for (int tier = 1; tier <= 4; tier++)
                        if (id == path + ".t" + tier)
                            return new SpecializationNode { Id = id, PathId = path, ClassId = cls, Tier = tier,
                                RequiredLevel = NodeLevels[tier - 1] };
            return null;
        }
    }
    public interface ICampaignStore
    {
        CampaignState Load();
        bool TryCommit(long expectedVersion, CampaignState candidate);
    }
    // One adapter, one root CAS. MemoryService remains the authority for choice validation.
    public sealed class CampaignMemoryAdapter : IMemoryStore
    {
        private readonly ICampaignStore store;
        public CampaignMemoryAdapter(ICampaignStore store) { this.store = store ?? throw new ArgumentNullException("store"); }
        public MemorySave Load() { var s = store.Load(); s.Validate(); return s.Memory.Copy(); }
        public bool TryCommit(long expectedVersion, MemorySave memory)
        {
            var s = store.Load(); s.Validate();
            if (s.Version != expectedVersion) return false;
            if (memory.CampaignId != s.Memory.CampaignId || memory.Version != expectedVersion + 1)
                throw new SerializationException("Invalid memory transaction");
            s = s.Copy(); s.Memory = memory.Copy();
            if (!s.ForestRewardClaimed && memory.Choices.ContainsKey("act1.spy_hunt"))
            {
                if (memory.Choices["act1.spy_hunt"] == "judge_suspects")
                {
                    if (s.Gold > int.MaxValue - CampaignContent.FirstQuestGold) throw new System.IO.IOException("Gold capacity reached");
                    s.Gold += CampaignContent.FirstQuestGold;
                }
                else s.ReservedHealthPotions = CampaignContent.FirstQuestHealthPotions;
                s.ForestRewardClaimed = true;
            }
            s.Validate(); return store.TryCommit(expectedVersion, s);
        }
    }
}
