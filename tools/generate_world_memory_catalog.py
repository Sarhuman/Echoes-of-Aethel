import json,re
from pathlib import Path
root=Path(__file__).resolve().parents[1]
manifest=json.loads((root/'docs/engineering/campaign-route-manifest.json').read_text(encoding='utf-8'))
gdd=(root/'docs/GDD/04_mapas_e_quests.md').read_text(encoding='utf-8')
blocks=re.split(r'### Mapa \d+ — ',gdd)[1:]
def lit(s): return json.dumps(s,ensure_ascii=False)
if len(blocks) != 12 or len(manifest['maps']) != 12:
    raise SystemExit('Expected exactly twelve canonical maps')
rows=[]
for expected,(m,b) in enumerate(zip(manifest['maps'],blocks),1):
    if m['order'] != expected or ('ID `'+m['quest_id']+'`') not in b:
        raise SystemExit('Canonical map order/quest mismatch')
    a,c,t=re.search(r'canalizam, respectivamente, `([^`]+)` / `([^`]+)`\. Disparo tardio `([^`]+)`',b).groups()
    options=[]
    for choice,aff in zip(m['choice_ids'],[a,c]):
        line=next(x for x in b.splitlines() if '| A / `'+choice+'`' in x or '| B / `'+choice+'`' in x)
        cells=[x.strip() for x in line.split('|')[1:-1]]
        options.append('new ChoiceDefinition(%s, %s, %s, %s, %s)'%(lit(choice),lit(aff),lit(cells[1]),lit(cells[2]),lit(cells[4])))
    rows.append('            new QuestDefinition(%d, %s, %s, %s, new[] { %s }),'%(m['order'],lit(m['zone_id']),lit(m['quest_id']),lit(t),', '.join(options)))
generated=('''// Generated from GDD 04 and campaign-route-manifest.json. Do not expose pending text to UI.
namespace Aethel.WorldMemory
{
    internal static class CanonicalCampaign
    {
        internal static readonly QuestDefinition[] Quests = new[]
        {
'''+ '\n'.join(rows)+'''
        };
    }
}
''')

import argparse
p=argparse.ArgumentParser(description='Generate canonical C# world memory text from GDD 04')
p.add_argument('--write', action='store_true')
args=p.parse_args()
output=root/'shared/csharp/Aethel.WorldMemory/CanonicalCampaign.cs'
if args.write:
    output.write_text(generated, encoding='utf-8')
elif output.read_text(encoding='utf-8') != generated:
    raise SystemExit('CanonicalCampaign.cs differs from canonical sources; run with --write and review')
print('Canonical C# catalog: 12 quests / 24 choices agree with GDD 04')
