using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Aethel.Campaign
{
    public sealed class JsonCampaignStore : ICampaignStore
    {
        private const int MaxBytes = 1048576;
        private readonly string path;
        private readonly CampaignState initial;
        public JsonCampaignStore(string path, CampaignState initial)
        {
            if (initial == null) throw new ArgumentNullException("initial");
            initial.Validate(); this.initial = initial.Copy(); this.path = Path.GetFullPath(path);
            if (initial.Version != 0 || initial.Memory.Choices.Count != 0 || initial.Commands.Count != 0 || initial.Nodes.Count != 0 ||
                initial.Level != 1 || initial.Gold != 0 || initial.RespecCount != 0 || initial.NoticeDismissed ||
                initial.Memory.Clues.Count != 0 || initial.Memory.Encounters.Count != 0 || initial.Memory.Entered.Count != 0 || initial.Memory.Revealed.Count != 0)
                throw new ArgumentException("Initial state must be a new campaign");
            Directory.CreateDirectory(Path.GetDirectoryName(this.path));
        }
        private bool SameIdentity(CampaignState s)
        {
            return s.CharacterId == initial.CharacterId && s.ClassId == initial.ClassId && s.Memory.CampaignId == initial.Memory.CampaignId;
        }
        private CampaignState ReadUnlocked()
        {
            if (!File.Exists(path)) return initial.Copy();
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (file.Length > MaxBytes) throw new SerializationException("Campaign save too large");
                var s = (CampaignState)new DataContractJsonSerializer(typeof(CampaignState)).ReadObject(file);
                if (s == null) throw new SerializationException("Empty campaign");
                s.Validate();
                if (s.Schema == 1) s = CampaignState.MigrateV1(s);
                if (!SameIdentity(s)) throw new SerializationException("Campaign identity mismatch");
                return s;
            }
        }
        public CampaignState Load()
        {
            using (var guard = new FileStream(path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                return ReadUnlocked();
        }
        public bool TryCommit(long expectedVersion, CampaignState candidate)
        {
            candidate = candidate.Copy(); candidate.Validate();
            if (candidate.Schema != 2 || !SameIdentity(candidate) || candidate.Version != expectedVersion + 1)
                throw new SerializationException("Invalid campaign transaction");
            using (var guard = new FileStream(path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var previous = ReadUnlocked();
                if (previous.Version != expectedVersion) return false;
                candidate.AssertExtends(previous);
                string temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                try
                {
                    using (var file = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        new DataContractJsonSerializer(typeof(CampaignState)).WriteObject(file, candidate);
                        if (file.Length > MaxBytes) throw new IOException("Campaign save capacity exceeded");
                        file.Flush(true);
                    }
                    if (File.Exists(path)) File.Replace(temp, path, path + ".bak"); else File.Move(temp, path);
                    return true;
                }
                finally
                {
                    // Cleanup after the commit point must never report a committed write as rejected.
                    try { if (File.Exists(temp)) File.Delete(temp); } catch (IOException) { } catch (UnauthorizedAccessException) { }
                }
            }
        }
    }
}
