#!/usr/bin/env python3
"""Check/generate trusted C# from the reviewed, experimental encounter catalog."""
import json
import pathlib
import re
import sys
root = pathlib.Path(__file__).resolve().parents[1]
data = json.loads((root / 'docs/engineering/encounter-catalog.json').read_text())
rows = data['encounters']
common_names = [line.split('|')[2].strip().split('; ') for line in (root / 'docs/design/campaign-cast-and-bestiary-3d.md').read_text().splitlines() if re.match(r'\| \d\d \|', line)]
assert len(rows) == 51 and len({r['id'] for r in rows}) == 51
manifest = json.loads((root / 'docs/engineering/campaign-route-manifest.json').read_text())
for region in manifest['maps']:
    local = [r for r in rows if r['zone'] == region['zone_id']]
    assert len([r for r in local if r['terminal']]) == 1
    assert len([r for r in local if r['category'] == 'Common']) == (0 if region['order'] == 12 else 3)
    final = next(r for r in local if r['terminal'])
    assert final['name'] == region['final_encounter']['name']
    if region['order'] < 12:
        assert sorted(r['name'] for r in local if r['category'] == 'Common') == sorted(common_names[region['order'] - 1])
    assert final['category'] == ('Boss' if region['order'] % 3 == 0 else 'SubBoss')
for r in rows:
    assert re.fullmatch(r'[A-Za-z0-9_.:-]{1,128}', r['id'])
    assert 1 <= r['min_level'] <= r['max_level'] <= 30
    assert r['xp'] > 0 and r['hp'] > 0 and r['telegraph_ms'] >= 400
    assert r['category'] in ['Common', 'Elite', 'SubBoss', 'Boss']
    assert not r['optional'] or not r['recipe']
lines = ['// Generated from encounter-catalog.json; NEW/EXPERIMENTAL stats/IDs. No AI or scene assets.', 'namespace Aethel.Campaign', '{', '    public static partial class EncounterCatalog', '    {', '        private static readonly EncounterDefinition[] Entries = {']
for r in rows:
    fields = ['Id','Name','Zone','Category','MinLevel','MaxLevel','Xp','Hp','Defense','Ward','Power','TelegraphMs','Behavior','Element','Undead','Terminal','Optional','Recipe']
    keys = ['id','name','zone','category','min_level','max_level','xp','hp','defense','ward','power','telegraph_ms','behavior','element','undead','terminal','optional','recipe']
    values = []
    for f,k in zip(fields,keys):
        v = r[k]
        val = 'EncounterCategory.' + v if k == 'category' else str(v).lower() if isinstance(v,bool) else str(v) if isinstance(v,int) else json.dumps(v, ensure_ascii=False)
        values.append(f + '=' + val)
    lines.append('            new EncounterDefinition { '+', '.join(values)+' },')
lines += ['        };', '    }', '}', '']
text = '\n'.join(lines)
out = root / 'shared/csharp/Aethel.Campaign/EncounterCatalog.Generated.cs'
if '--write' in sys.argv:
    out.write_text(text)
else:
    assert out.read_text() == text, 'Run tools/generate_encounter_catalog.py --write'
print('51 encounters checked; 12 campaign regions + 2 optional dungeons')
