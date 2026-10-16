// Generated from regional design + Godot quest definition. Experimental region levels.
namespace Aethel.Campaign
{
    public sealed class RegionDefinition
    {
        public string ZoneId { get; private set; }
        public int CommonMin { get; private set; }
        public int CommonMax { get; private set; }
        public int MainEncounterLevel { get; private set; }
        public bool HasCommonEnemies { get { return CommonMin > 0; } }
        internal RegionDefinition(string zone, int low, int high, int boss)
        { ZoneId = zone; CommonMin = low; CommonMax = high; MainEncounterLevel = boss; }
    }
    public static class CampaignContent
    {
        public const int FirstQuestGold = 25;
        public const int FirstQuestHealthPotions = 3;
        private static readonly RegionDefinition[] Regions = new[]
        {
            new RegionDefinition("forest_01", 1, 4, 4),
            new RegionDefinition("swamp_02", 4, 6, 6),
            new RegionDefinition("temple_03", 6, 8, 8),
            new RegionDefinition("dunes_04", 8, 10, 10),
            new RegionDefinition("valley_05", 10, 12, 12),
            new RegionDefinition("crypt_06", 12, 15, 15),
            new RegionDefinition("sewers_07", 15, 17, 17),
            new RegionDefinition("foundry_08", 17, 20, 20),
            new RegionDefinition("abyss_09", 20, 22, 22),
            new RegionDefinition("gardens_10", 22, 25, 25),
            new RegionDefinition("bridge_11", 25, 28, 28),
            new RegionDefinition("heart_12", 0, 0, 30),
        };
        public static RegionDefinition FindRegion(string zoneId)
        {
            foreach (var region in Regions) if (region.ZoneId == zoneId) return region;
            return null; // Hubs have no hostile level. Unknown IDs are not invented here.
        }
    }
}
