using System;

namespace Aethel.Campaign
{
    public enum EncounterCategory { Common, Elite, SubBoss, Boss }
    public enum ThreatBand { Below, Equal, Above, FarAbove }
    public sealed class ThreatView
    {
        public ThreatBand Band { get; internal set; }
        public string ColorRole { get; internal set; }
        public string AccessibleLabel { get; internal set; }
        public EncounterCategory Category { get; internal set; }
        public int EnemyLevel { get; internal set; }
        public bool ShowThreatSymbol { get { return Band == ThreatBand.FarAbove; } }
        public bool ShowBossBar { get { return Category == EncounterCategory.Boss || Category == EncounterCategory.SubBoss; } }
    }
    public static class ThreatPresentation
    {
        // Presentation projection only. Does not change enemy level/stats, loot or detection.
        public static ThreatView Compare(int playerLevel, int enemyLevel, EncounterCategory category)
        {
            if (playerLevel < 1 || playerLevel > 30 || enemyLevel < 1 || enemyLevel > 30 ||
                !Enum.IsDefined(typeof(EncounterCategory), category)) throw new ArgumentOutOfRangeException();
            int delta = enemyLevel - playerLevel;
            var band = delta >= 4 ? ThreatBand.FarAbove : delta > 0 ? ThreatBand.Above : delta == 0 ? ThreatBand.Equal : ThreatBand.Below;
            return new ThreatView { Band = band, EnemyLevel = enemyLevel, Category = category,
                ColorRole = band == ThreatBand.FarAbove ? "dark_red" : band == ThreatBand.Above ? "light_red" : band == ThreatBand.Equal ? "white" : "blue",
                AccessibleLabel = band == ThreatBand.FarAbove ? "Muito acima do seu nível" : band == ThreatBand.Above ? "Acima do seu nível" :
                    band == ThreatBand.Equal ? "Mesmo nível" : "Abaixo do seu nível" };
        }
    }
}
