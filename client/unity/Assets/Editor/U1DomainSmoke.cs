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
        Vector3 attackDirection;
        if (!U1Player.TryGetAttackDirection(Vector3.zero, new Vector3(1.7f, 0.7f, 0f), 2.1f, out attackDirection) ||
            attackDirection.x < 0.99f ||
            U1Player.TryGetAttackDirection(Vector3.zero, new Vector3(2.2f, 0f, 0f), 2.1f, out attackDirection))
            throw new Exception("U1 melee targeting range or auto-facing regressed");

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

            string rewardSave = Path.Combine(root, "u1-reward.json");
            var session = new U1RewardSession(rewardSave);
            var first = session.ClaimDefeatedWolf();
            if (first.Status != ProgressionStatus.Accepted ||
                !first.State.Defeats.ContainsKey(session.CurrentWolfSpawnId) ||
                first.State.TotalXp != 10 || !File.Exists(rewardSave))
                throw new Exception("First U1 wolf reward was not committed");
            long committedVersion = first.State.Version;
            var replay = session.ClaimDefeatedWolf();
            if (replay.Status != ProgressionStatus.Replayed ||
                replay.State.Version != committedVersion || replay.State.Gold != first.State.Gold ||
                replay.State.Inventory.Items.Count != first.State.Inventory.Items.Count)
                throw new Exception("U1 wolf reward duplicated within one attempt");
            session.BeginAttempt();
            if (session.CurrentWolfSpawnId == U1RewardSession.WolfSpawnPrefix + "0001")
                throw new Exception("Second attempt reused the first encounter ID");
            var second = session.ClaimDefeatedWolf();
            if (second.Status != ProgressionStatus.Accepted || second.State.TotalXp != 20 ||
                second.State.Defeats.Count != 2)
                throw new Exception("Second U1 attempt did not earn a fresh reward");
            var reloadedSession = new U1RewardSession(rewardSave);
            if (reloadedSession.CurrentWolfSpawnId == session.CurrentWolfSpawnId ||
                reloadedSession.Read().Version != second.State.Version)
                throw new Exception("U1 next attempt did not survive reload");

            Debug.Log("AETHEL_UNITY_DOMAIN_SMOKE_OK hp=172 mp=52 json=ok replace=ok reward=per-attempt");
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
