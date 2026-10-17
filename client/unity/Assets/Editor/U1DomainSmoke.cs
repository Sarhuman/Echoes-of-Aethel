using System;
using System.IO;
using System.Runtime.Serialization.Json;
using Aethel.Campaign;
using UnityEngine;

public static class U1DomainSmoke
{
    public static void Verify()
    {
        var state = CampaignState.Create("unity_smoke", "bastion", "twilight_bastion");
        if (CharacterRules.MaxHp(state) != 172 || CharacterRules.MaxMp(state) != 52)
        {
            throw new Exception("Bastion starting vitals changed");
        }

        string root = Path.Combine(Path.GetTempPath(), "aethel-unity-smoke-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            string save = Path.Combine(root, "campaign.json");
            using (var output = new FileStream(save, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                new DataContractJsonSerializer(typeof(CampaignState)).WriteObject(output, state);
                output.Flush(true);
            }
            using (var input = File.OpenRead(save))
            {
                var reloaded = (CampaignState)new DataContractJsonSerializer(typeof(CampaignState)).ReadObject(input);
                reloaded.Validate();
                if (reloaded.CharacterId != state.CharacterId || reloaded.Schema != 3)
                {
                    throw new Exception("Campaign JSON roundtrip changed identity or schema");
                }
            }

            string replacement = Path.Combine(root, "replacement.tmp");
            string backup = Path.Combine(root, "campaign.bak");
            File.WriteAllText(replacement, "replacement");
            File.Replace(replacement, save, backup);
            if (File.ReadAllText(save) != "replacement" || !File.Exists(backup))
            {
                throw new Exception("Atomic file replacement failed");
            }

            Debug.Log("AETHEL_UNITY_DOMAIN_SMOKE_OK hp=172 mp=52 json=ok replace=ok");
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
