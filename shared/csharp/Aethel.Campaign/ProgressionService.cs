using System;
using System.Linq;
using System.IO;

namespace Aethel.Campaign
{
    // Must be implemented by the trusted scene/application adapter, not supplied by a menu checkbox.
    public interface ITrainingAccess { bool CanTrain(string characterId, string classId); }
    public enum ProgressionStatus { Accepted, Replayed, Invalid, Locked, Conflict, InsufficientGold, PersistenceFailed, HistoryFull, Expired }
    public sealed class ProgressionResult
    {
        public ProgressionStatus Status { get; internal set; }
        public CampaignState State { get; internal set; }
    }
    public sealed class ProgressionService
    {
        private readonly ICampaignStore store;
        private readonly ITrainingAccess training;
        public ProgressionService(ICampaignStore store, ITrainingAccess training)
        {
            this.store = store ?? throw new ArgumentNullException("store");
            this.training = training ?? throw new ArgumentNullException("training");
        }
        public CampaignState Read() { var s = store.Load(); s.Validate(); if (s.Schema != 2) throw new System.Runtime.Serialization.SerializationException("Campaign store must migrate to schema 2"); return s.Copy(); }
        internal ProgressionResult Run(string commandId, string payload, long expected, Func<CampaignState, ProgressionStatus> operation)
        {
            var original = Read();
            Guid token;
            if (!Guid.TryParseExact(commandId, "D", out token))
                return new ProgressionResult { Status = ProgressionStatus.Invalid, State = original };
            // Normalize casing so a UUID is one identity.
            commandId = token.ToString("D");
            if (original.Commands.ContainsKey(commandId))
                return new ProgressionResult { Status = original.Commands[commandId] == payload ? ProgressionStatus.Replayed : ProgressionStatus.Conflict, State = original };
            if (expected < original.ReceiptFloor)
                return new ProgressionResult { Status = ProgressionStatus.Expired, State = original };
            if (original.Version != expected)
                return new ProgressionResult { Status = ProgressionStatus.Conflict, State = original };
            var candidate = original.Copy();
            var status = operation(candidate);
            if (status == ProgressionStatus.Accepted)
            {
                candidate.Memory.Version++;
                if (candidate.Commands.Count >= 512)
                {
                    var oldest = candidate.ReceiptVersions.OrderBy(x => x.Value).ThenBy(x => x.Key,StringComparer.Ordinal).First();
                    candidate.ReceiptFloor = Math.Max(candidate.ReceiptFloor,oldest.Value + 1);
                    candidate.Commands.Remove(oldest.Key); candidate.ReceiptVersions.Remove(oldest.Key);
                }
                candidate.Commands.Add(commandId, payload); candidate.ReceiptVersions.Add(commandId,expected);
                candidate.Validate();
                try { if (!store.TryCommit(expected, candidate.Copy())) status = ProgressionStatus.Conflict; }
                catch (IOException) { status = ProgressionStatus.PersistenceFailed; }
                catch (UnauthorizedAccessException) { status = ProgressionStatus.PersistenceFailed; }
            }
            return new ProgressionResult { Status = status, State = Read() };
        }
        // Application seam for a confirmed level from the future XP service. Not an XP award API.
        internal ProgressionResult SetConfirmedLevel(string commandId, int level, long expected)
        {
            return Run(commandId, "level|" + level, expected, s => {
                if (level < 1 || level > 30) return ProgressionStatus.Invalid;
                if (level < s.Level) return ProgressionStatus.Conflict;
                if (level == s.Level) return ProgressionStatus.Replayed;
                s.TotalXp = Math.Max(s.TotalXp, ExperienceRules.MinimumForLevel(level)); s.Level = level; return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult DismissNotice(string commandId, long expected)
        {
            return Run(commandId, "dismiss-notice", expected, s => {
                if (s.Level < 5) return ProgressionStatus.Locked;
                if (s.NoticeDismissed) return ProgressionStatus.Replayed;
                s.NoticeDismissed = true; return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult SelectPath(string commandId, string pathId, long expected)
        {
            return Run(commandId, "select|" + pathId, expected, s => {
                if (!SpecializationCatalog.Paths(s.ClassId).Contains(pathId)) return ProgressionStatus.Invalid;
                if (s.PrimaryPath != "") return s.PrimaryPath == pathId ? ProgressionStatus.Replayed : ProgressionStatus.Conflict;
                if (s.Level < 5 || !training.CanTrain(s.CharacterId, s.ClassId)) return ProgressionStatus.Locked;
                s.PrimaryPath = pathId; s.Nodes.Add(pathId + ".t1"); return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult BuyNode(string commandId, string nodeId, long expected)
        {
            return Run(commandId, "node|" + nodeId, expected, s => {
                var n = SpecializationCatalog.Find(nodeId);
                if (n == null || n.ClassId != s.ClassId) return ProgressionStatus.Invalid;
                if (s.Nodes.Contains(nodeId)) return ProgressionStatus.Replayed;
                if (s.PrimaryPath == "" || n.RequiredLevel > s.Level || s.UnspentSpecializationPoints < 1 ||
                    (n.Tier == 1 && n.PathId != s.PrimaryPath && s.Level < 10) ||
                    (n.Tier > 1 && !s.Nodes.Contains(n.PathId + ".t" + (n.Tier - 1))) ||
                    !training.CanTrain(s.CharacterId, s.ClassId)) return ProgressionStatus.Locked;
                s.Nodes.Add(nodeId); return ProgressionStatus.Accepted;
            });
        }
        public int RespecPrice() { var s = Read(); return s.RespecCount == 0 ? 0 : SpecializationCatalog.RespecGoldPerLevel * s.Level; }
        public ProgressionResult Respec(string commandId, string primaryPath, string[] requestedNodes, long expected)
        {
            // Normalize set order but preserve duplicates for validation. Never mutate caller-owned arrays.
            if (requestedNodes != null && (requestedNodes.Length > 6 || requestedNodes.Any(x => x == null || x.Length > 128)))
                return new ProgressionResult { Status = ProgressionStatus.Invalid, State = Read() };
            string[] nodes = requestedNodes == null ? null : requestedNodes.ToArray();
            string payload = "respec|" + primaryPath + "|" + (nodes == null ? "<null>" : string.Join(",", nodes.OrderBy(x => x, StringComparer.Ordinal)));
            return Run(commandId, payload, expected, s => {
                if (nodes == null || nodes.Length > 6 || nodes.Any(x => SpecializationCatalog.Find(x) == null) ||
                    nodes.Distinct().Count() != nodes.Length || !SpecializationCatalog.Paths(s.ClassId).Contains(primaryPath))
                    return ProgressionStatus.Invalid;
                if (s.Level < 5 || s.PrimaryPath == "" || !training.CanTrain(s.CharacterId, s.ClassId)) return ProgressionStatus.Locked;
                if (s.PrimaryPath == primaryPath && s.Nodes.OrderBy(x => x).SequenceEqual(nodes.OrderBy(x => x)))
                    return ProgressionStatus.Replayed;
                var probe = s.Copy(); probe.PrimaryPath = primaryPath; probe.Nodes = nodes.ToList();
                try { probe.Validate(); } catch (System.Runtime.Serialization.SerializationException) { return ProgressionStatus.Invalid; }
                int price = s.RespecCount == 0 ? 0 : SpecializationCatalog.RespecGoldPerLevel * s.Level;
                if (s.Gold < price) return ProgressionStatus.InsufficientGold;
                s.Effects.CounterExpiresAt = 0;
                s.Gold -= price; s.RespecCount++; s.PrimaryPath = primaryPath; s.Nodes = nodes.ToList();
                return ProgressionStatus.Accepted;
            });
        }
    }
}
