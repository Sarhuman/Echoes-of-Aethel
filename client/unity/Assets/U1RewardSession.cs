using System;
using System.Globalization;
using System.Linq;
using Aethel.Campaign;

// Small adapter for the U1 encounter. The shared domain remains the authority for
// the receipt, loot roll, XP and gold transaction.
public sealed class U1RewardSession
{
    public const string WolfSpawnPrefix = "forest_01.ethereal_wolf.u1.";
    private const string LegacyWolfSpawnId = "forest_01.ethereal_wolf.u1";

    private readonly JsonCampaignStore store;
    private readonly EncounterRewardService rewards;
    private readonly WolfVictoryAccess victory = new WolfVictoryAccess();
    public string CurrentWolfSpawnId { get; private set; }

    public U1RewardSession(string path)
    {
        store = new JsonCampaignStore(path,
            CampaignState.Create("unity_u1", "bastion", "twilight_bastion"));
        rewards = new EncounterRewardService(store, new NoTraining(), victory);
        BeginAttempt();
    }

    public CampaignState Read() { return store.Load(); }

    public void BeginAttempt()
    {
        int completed = store.Load().Defeats.Keys.Count(id =>
            id == LegacyWolfSpawnId || id.StartsWith(WolfSpawnPrefix, StringComparison.Ordinal));
        CurrentWolfSpawnId = WolfSpawnPrefix + (completed + 1).ToString("D4", CultureInfo.InvariantCulture);
        victory.SpawnId = CurrentWolfSpawnId;
    }

    // Called only after U1Enemy confirms HP zero. The access is enabled for this
    // transaction and reset even when the domain rejects or persistence fails.
    public ProgressionResult ClaimDefeatedWolf()
    {
        victory.Confirmed = true;
        try
        {
            CampaignState before = store.Load();
            return rewards.Claim(Guid.NewGuid().ToString("D"), CurrentWolfSpawnId, before.Version);
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
        public string SpawnId;

        public bool TryConfirmVictory(string character, string spawn, out string definitionId, out int level)
        {
            definitionId = "ethereal_wolf";
            level = 1;
            return Confirmed && character == "bastion" && spawn == SpawnId;
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
