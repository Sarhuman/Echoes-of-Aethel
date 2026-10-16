namespace Aethel.Campaign
{
    // Experimental baseline. Origins: experimental-values.md + class-specializations.md.
    // New grid footprints and higher-tier modifiers are documented in runtime-core-v2.md.
    public static class GameplayBalance
    {
        public const int XpBase = RuntimeRewards.XpBase, XpStep = RuntimeRewards.XpStep;
        public const int GuardMp = 8, GuardImprovedMp = 6, GuardDurationMs = 2000, GuardCooldownMs = 6000;
        public const int ImpactMp = 12, ImpactImprovedMp = 9, ImpactPower = 18, ImpactActiveMs = 80, ImpactCooldownMs = 7000;
        public const int CounterBonusBp = 1500, CounterImprovedBonusBp = 3000, CounterDurationMs = 3000, CounterCooldownMs = 6000;
        public const int RuptureBp = 1500, RuptureImprovedBp = 2000, RuptureDurationMs = 5000, RuptureImprovedDurationMs = 7000;
        public const int GuardReductionBp = 4000, GuardImprovedReductionBp = 500, PostureReductionBp = 1000;
        public const int PotionCooldownMs = 5000, HealthPotionPrice = 10, AetherFlaskPrice = 12;
        public const int BagWidth = 10, BagHeight = 6, StashWidth = 10, StashHeight = 10, ReserveInstances = 100;
    }
}
