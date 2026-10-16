using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Aethel.WorldMemory
{
    // Dedicated new-client file, never a Godot save. Single writer lock covers CAS + replace.
    public sealed class JsonMemoryStore : IMemoryStore
    {
        private const int MaxBytes = 32768;
        private readonly string path, campaign;
        public JsonMemoryStore(string path, string campaignId)
        {
            this.path = Path.GetFullPath(path); campaign = campaignId;
            new MemorySave { CampaignId = campaign }.Validate();
            Directory.CreateDirectory(Path.GetDirectoryName(this.path));
        }
        private MemorySave ReadUnlocked()
        {
            if (!File.Exists(path)) return new MemorySave { CampaignId = campaign };
            using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (input.Length > MaxBytes) throw new SerializationException("Memory save too large");
                var s = (MemorySave)new DataContractJsonSerializer(typeof(MemorySave)).ReadObject(input);
                if (s == null) throw new SerializationException("Empty save");
                s.Validate();
                if (s.CampaignId != campaign) throw new SerializationException("Wrong campaign");
                return s;
            }
        }
        public MemorySave Load()
        {
            using (var guard = new FileStream(path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                return ReadUnlocked();
        }
        public bool TryCommit(long expectedVersion, MemorySave candidate)
        {
            candidate = candidate.Copy(); candidate.Validate();
            if (candidate.CampaignId != campaign || candidate.Version != expectedVersion + 1)
                throw new SerializationException("Invalid candidate");
            using (var guard = new FileStream(path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                if (ReadUnlocked().Version != expectedVersion) return false;
                string temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                try
                {
                    using (var output = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        new DataContractJsonSerializer(typeof(MemorySave)).WriteObject(output, candidate);
                        if (output.Length > MaxBytes) throw new SerializationException("Memory save too large");
                        output.Flush(true);
                    }
                    if (File.Exists(path)) File.Replace(temp, path, path + ".bak");
                    else File.Move(temp, path);
                    return true;
                }
                finally { if (File.Exists(temp)) File.Delete(temp); }
            }
        }
    }
}
