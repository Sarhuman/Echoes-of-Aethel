"""Keep the existing experimental XP curve and first-route XP rewards in sync."""
import argparse
import re
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
doc=(ROOT/'docs/engineering/experimental-values.md').read_text(encoding='utf-8')
curve=next(line for line in doc.splitlines() if line.startswith('| XP para o próximo nível |'))
base,step=map(int,re.search(r'`(\d+) \+ (\d+)×\(L−1\)`',curve).groups())
reward=next(line for line in doc.splitlines() if line.startswith('| `base_xp` |'))
wolf,cultist,moss,total=map(int,re.search(r'lobo (\d+) · cultista (\d+) · Garras de Musgo (\d+) \(→ (\d+)\)',reward).groups())
if moss*8!=total:raise SystemExit('Sub-boss XP multiplier/source disagree')
content='''// Generated from experimental-values.md section 5; not a validated campaign XP budget.
namespace Aethel.Campaign
{
    internal static class RuntimeRewards
    {
        internal const int XpBase = %d, XpStep = %d;
        internal const int WolfXp = %d, CultistXp = %d, MossClawsXp = %d;
    }
}
'''%(base,step,wolf,cultist,total)
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--write',action='store_true');args=p.parse_args()
output=ROOT/'shared/csharp/Aethel.Campaign/RuntimeRewards.cs'
if args.write:output.write_text(content,encoding='utf-8')
elif output.read_text(encoding='utf-8')!=content:raise SystemExit('Runtime XP rewards differ from sources')
print('Runtime XP agrees: existing curve and three first-route enemy rewards')
