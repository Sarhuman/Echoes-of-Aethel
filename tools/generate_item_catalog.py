"""Port fixed first-slice templates. Grid footprints are an explicit experimental baseline."""
import argparse
import json
import re
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'shared/csharp/Aethel.Campaign/ItemCatalog.cs'
rows=[]
for path in sorted((ROOT/'client/godot/content/items').glob('*.tres')):
    data=path.read_text(encoding='utf-8')
    def string(key):
        m=re.search(r'^'+key+r' = &?"([^"]*)"$',data,re.M)
        if not m:raise ValueError('Missing '+key+' in '+str(path))
        return m[1]
    def number(key):
        return int(re.search(r'^'+key+r' = (\d+)$',data,re.M)[1])
    item,kind,slot=string('item_id'),string('kind'),string('slot')
    width,height=(1,3) if slot=='main_hand' else (2,3) if slot=='chest' else (1,1)
    rows.append('            new ItemTemplate { Id = %s, Kind = %s, Slot = %s, Width = %d, Height = %d, MaxStack = %d, Power = %d, Armor = %d, Restore = %d },'%(json.dumps(item),json.dumps(kind),json.dumps(slot),width,height,number('max_stack'),number('power'),number('armor'),number('restore')))
if len(rows)!=16:raise SystemExit('Review changed catalog coverage before regeneration')
text='''// Generated from first-slice .tres; experimental footprints: axe 1x3, chest 2x3, others 1x1.
namespace Aethel.Campaign
{
    public static class ItemCatalog
    {
        private static readonly ItemTemplate[] Items = new[]
        {
'''+ '\n'.join(rows)+'''
        };
        public static ItemTemplate Find(string id)
        {
            foreach (var item in Items) if (item.Id == id) return item;
            return AdvancedItemCatalog.Find(id);
        }
    }
}
'''
p=argparse.ArgumentParser(description=__doc__);p.add_argument('--write',action='store_true');args=p.parse_args()
if args.write:OUT.write_text(text,encoding='utf-8')
elif OUT.read_text(encoding='utf-8')!=text:raise SystemExit('Item catalog differs; regenerate and review')
print('Fixed item catalog agrees: 16 templates; explicit experimental footprints')
