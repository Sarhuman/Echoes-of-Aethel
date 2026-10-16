"""Generate trusted C# recommendations/reward constants; default checks for drift."""
import argparse
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DOC = ROOT / 'docs/design/regional-threat-and-training.md'
OUTPUT = ROOT / 'shared/csharp/Aethel.Campaign/CampaignContent.cs'


def generate():
    text = DOC.read_text(encoding='utf-8')
    maps = json.loads((ROOT / 'docs/engineering/campaign-route-manifest.json').read_text(encoding='utf-8'))['maps']
    rows = re.findall(r'^\| [^|]+ / `([^`]+)` \| ([^|]+) \| ([^|]+) \|$', text, re.M)
    if len(rows) != 12 or [r[0] for r in rows] != [m['zone_id'] for m in maps]:
        raise ValueError('Regional table must match twelve canonical zones in order')
    definitions = []
    for (zone, common, boss), record in zip(rows, maps):
        boss_name, boss_level = boss.rsplit(': ', 1)
        if boss_name != record['final_encounter']['name']:
            raise ValueError('Encounter name mismatch: ' + zone)
        low, high = (0, 0) if common.strip() == 'Sem comuns' else map(int, common.strip().split('–'))
        boss_level = int(boss_level)
        if not (0 <= low <= high <= boss_level <= 30) or (low == 0 and zone != 'heart_12'):
            raise ValueError('Invalid level range: ' + zone)
        definitions.append('            new RegionDefinition(%s, %d, %d, %d),' % (json.dumps(zone), low, high, boss_level))
    reward = (ROOT / 'client/godot/content/quests/quest_definition.gd').read_text(encoding='utf-8')
    gold = int(re.search(r'judgment_gold\s*:=\s*(\d+)', reward)[1])
    potions = int(re.search(r'mercy_potions\s*:=\s*(\d+)', reward)[1])
    # Capacity/validation of this prototype is explicitly limited to the existing first-slice reward.
    if gold != 25 or potions != 3:
        raise ValueError('Canonical reward changed: review aggregate validation/capacity and tests before regeneration')
    return '''// Generated from regional design + Godot quest definition. Experimental region levels.
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
        public const int FirstQuestGold = %d;
        public const int FirstQuestHealthPotions = %d;
        private static readonly RegionDefinition[] Regions = new[]
        {
%s
        };
        public static RegionDefinition FindRegion(string zoneId)
        {
            foreach (var region in Regions) if (region.ZoneId == zoneId) return region;
            return null; // Hubs have no hostile level. Unknown IDs are not invented here.
        }
    }
}
''' % (gold, potions, '\n'.join(definitions))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--write', action='store_true')
    args = parser.parse_args()
    content = generate()
    if args.write:
        OUTPUT.write_text(content, encoding='utf-8')
    elif OUTPUT.read_text(encoding='utf-8') != content:
        raise SystemExit('CampaignContent.cs differs from sources; regenerate and review')
    print('Campaign content agrees: 12 region ranges and canonical first-quest rewards')
