using System;
using Aethel.Campaign;

// Small adapter for the U1 encounter. The shared domain remains the authority for
// the receipt, loot roll, XP and gold transaction.
public sealed class U1RewardSession
{
    public const string WolfSpawnId = "forest_01.ethereal_wolf.u1";

    private readonly JsonCampaignStore store;
    private readonly EncounterRewardService rewards;
    private readonly WolfVictoryAccess victory = new WolfVictoryAccess();

    public U1RewardSession(string path)
    {
        store = new JsonCampaignStore(path,
            CampaignState.Create("unity_u1", "bastion", "twilight_bastion"));
        rewards = new EncounterRewardService(store, new NoTraining(), victory);
    }

    public CampaignState Read() { return store.Load(); }

    // Called only after U1Enemy confirms HP zero. The access is enabled for this
    // transaction and reset even when the domain rejects or persistence fails.
    public ProgressionResult ClaimDefeatedWolf()
    {
        victory.Confirmed = true;
        try
        {
            CampaignState before = store.Load();
            return rewards.Claim(Guid.NewGuid().ToString("D"), WolfSpawnId, before.Version);
        }
        finally { victory.Confirmed = false; }
    }

    private sealed class NoTraining : ITrainingAccess
    {
        public bool CanTrain(string characterId, string classId) { return false; }
    }

    private sealed class WolfVictoryAccess : IEncounterRewardAccess
    {
        public bool Confirmed;

        public bool TryConfirmVictory(string character, string spawn, out string definitionId, out int level)
        {
            definitionId = "ethereal_wolf";
            level = 1;
            return Confirmed && character == "bastion" && spawn == WolfSpawnId;
        }

        public ILootRandom CreateVictoryRandom(string character, string spawn)
        {
            // Stable U1 seed: retries before commit cannot change the roll.
            unchecked
            {
                int seed = 17;
                foreach (char c in spawn) seed = seed * 31 + c;
                return new SeededLootRandom(seed);
            }
        }
    }
}
