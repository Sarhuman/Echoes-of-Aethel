using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Aethel.Campaign
{
    public sealed class ItemTemplate
    {
        public string Id { get; internal set; }
        public string Kind { get; internal set; }
        public string Slot { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int MaxStack { get; internal set; }
        public int Power { get; internal set; }
        public int Armor { get; internal set; }
        public int Restore { get; internal set; }
    }
    [DataContract]
    public sealed class ItemInstance
    {
        [DataMember(IsRequired = true)] public string Id;
        [DataMember(IsRequired = true)] public string TemplateId;
        [DataMember(IsRequired = true)] public string Owner;
        [DataMember(IsRequired = true)] public int ContentVersion = 1;
        [DataMember(IsRequired = true)] public int Quantity;
        [DataMember(IsRequired = true)] public string Location;
        [DataMember(IsRequired = true)] public int X, Y;
        [DataMember] public EquipmentRoll Roll;
        internal ItemInstance Copy() { var x=(ItemInstance)MemberwiseClone(); x.Roll=Roll==null?null:Roll.Copy(); return x; }
    }
    [DataContract]
    public sealed class InventoryState
    {
        [DataMember(IsRequired = true)] public List<ItemInstance> Items = new List<ItemInstance>();
        [DataMember(IsRequired = true)] public Dictionary<string, int> QuestItems = new Dictionary<string, int>();
        [DataMember(IsRequired = true)] public Dictionary<string, string> Equipped = AdvancedItemCatalog.Slots.ToDictionary(x=>x,x=>"");
        [DataMember(IsRequired = true)] public long NextSerial;
        internal InventoryState Copy()
        {
            return new InventoryState { Items = Items.Select(x => x.Copy()).ToList(), QuestItems = new Dictionary<string, int>(QuestItems),
                Equipped = new Dictionary<string, string>(Equipped), NextSerial = NextSerial };
        }
        public int Total(string templateId) { if (templateId == null) return 0; return Items.Where(x => x.TemplateId == templateId).Sum(x => x.Quantity) + (QuestItems.ContainsKey(templateId) ? QuestItems[templateId] : 0); }
        public int WeaponPower { get { return EquippedStat("main_hand", false); } }
        public int Armor { get { return Equipped.Values.Where(x=>x!="").Select(id=>Items.Single(x=>x.Id==id)).Sum(x=>ItemCatalog.Find(x.TemplateId).Armor + (x.Roll==null?0:x.Roll.Stat("armor"))); } }
        public int AffixStat(string stat) { return Equipped.Values.Where(x=>x!="").Distinct().Select(id=>Items.Single(x=>x.Id==id)).Sum(x=>x.Roll==null?0:x.Roll.Stat(stat)); }
        public int UniqueBonus(string id) { return Equipped.Values.Any(e=>Items.Any(x=>x.Id==e && x.Roll!=null && x.Roll.UniqueEffect==id))?500:0; }
        public int UniqueWard { get { return Equipped.Values.Any(id=>Items.Any(x=>x.Id==id && x.Roll!=null && x.Roll.UniqueEffect=="unique.steadfast_ward"))?5:0; } }
        private int EquippedStat(string slot, bool armor)
        {
            var item = Items.FirstOrDefault(x => x.Id == Equipped[slot]);
            var def = item == null ? null : ItemCatalog.Find(item.TemplateId);
            return def == null ? 0 : armor ? def.Armor : def.Power + (item.Roll==null?0:(item.Roll.Level-1)/2);
        }
        public void Validate(string owner)
        {
            if (Items == null || QuestItems == null || Equipped == null || NextSerial < 0 || NextSerial == long.MaxValue ||
                Items.Count > GameplayBalance.BagWidth * GameplayBalance.BagHeight + GameplayBalance.StashWidth * GameplayBalance.StashHeight + GameplayBalance.ReserveInstances || Items.Any(x => x == null) || Items.Select(x => x.Id).Distinct().Count() != Items.Count ||
                QuestItems.Count > 32 || (Equipped.Count != 2 && Equipped.Count != 9) || Equipped.Keys.Any(k=>!AdvancedItemCatalog.Slots.Contains(k)) || !Equipped.ContainsKey("main_hand") || !Equipped.ContainsKey("chest") ||
                Items.Count(x => x.Location == "reserve") > GameplayBalance.ReserveInstances)
                throw new SerializationException("Invalid inventory collections");
            var occupied = new HashSet<string>();
            foreach (var item in Items)
            {
                var def = ItemCatalog.Find(item.TemplateId); Guid instance;
                if (def == null || def.Kind == "quest" || !Guid.TryParseExact(item.Id, "D", out instance) || item.Id != instance.ToString("D") || item.Owner != owner ||
                    (item.ContentVersion != 1 && item.ContentVersion != 2) || (item.ContentVersion==2 && item.Roll==null) || (item.ContentVersion==1 && item.Roll!=null) || item.Quantity < 1 || item.Quantity > def.MaxStack ||
                    (item.Location != "bag" && item.Location != "stash" && item.Location != "reserve"))
                    throw new SerializationException("Invalid item instance");
                if (item.Roll!=null) item.Roll.Validate(item.TemplateId);
                if (AdvancedItemCatalog.Find(item.TemplateId)!=null && def.Kind=="equipment" && item.Roll==null) throw new SerializationException("Advanced equipment requires roll");
                if (item.Location == "reserve")
                { if (item.X != -1 || item.Y != -1) throw new SerializationException("Reserved placement"); continue; }
                int height = item.Location == "bag" ? GameplayBalance.BagHeight : GameplayBalance.StashHeight;
                int width = item.Location == "bag" ? GameplayBalance.BagWidth : GameplayBalance.StashWidth;
                if (item.X < 0 || item.Y < 0 || item.X + def.Width > width || item.Y + def.Height > height)
                    throw new SerializationException("Item outside grid");
                for (int y = item.Y; y < item.Y + def.Height; y++)
                    for (int x = item.X; x < item.X + def.Width; x++)
                        if (!occupied.Add(item.Location + ":" + x + ":" + y)) throw new SerializationException("Overlapping items");
            }
            foreach (var q in QuestItems)
                if (ItemCatalog.Find(q.Key) == null || ItemCatalog.Find(q.Key).Kind != "quest" || q.Value < 1 || q.Value > 1000000)
                    throw new SerializationException("Invalid quest item");
            foreach (var e in Equipped)
                if (e.Value != "" && !Items.Any(x => x.Id == e.Value && x.Location == "bag" && AdvancedItemCatalog.SlotMatches(x.TemplateId,e.Key)))
                    throw new SerializationException("Equipped reference missing");
        }
        internal bool GrantEquipment(string owner,string campaign,string template,EquipmentRoll roll)
        {
            roll.Validate(template);if(NextSerial==long.MaxValue)return false;
            var item=new ItemInstance { Id=Mint(owner,campaign),Owner=owner,TemplateId=template,Roll=roll.Copy(),ContentVersion=2,Quantity=1,Location="reserve",X=-1,Y=-1 };
            if(!PlaceFirst(item,"bag") && Items.Count(x=>x.Location=="reserve")>=GameplayBalance.ReserveInstances)return false;Items.Add(item);return true;
        }
        private string Mint(string owner, string campaign)
        {
            byte[] bytes;
            using (var sha = SHA256.Create()) bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(campaign + "|" + owner + "|" + NextSerial++));
            return new Guid(bytes.Take(16).ToArray()).ToString("D");
        }
        internal bool CanPlace(ItemInstance item, string location, int x, int y)
        {
            var def = ItemCatalog.Find(item.TemplateId);
            int height = location == "bag" ? GameplayBalance.BagHeight : location == "stash" ? GameplayBalance.StashHeight : 0;
            int width = location == "bag" ? GameplayBalance.BagWidth : GameplayBalance.StashWidth;
            if (def == null || x < 0 || y < 0 || x + def.Width > width || y + def.Height > height) return false;
            return !Items.Any(other => other.Id != item.Id && other.Location == location &&
                x < other.X + ItemCatalog.Find(other.TemplateId).Width && x + def.Width > other.X &&
                y < other.Y + ItemCatalog.Find(other.TemplateId).Height && y + def.Height > other.Y);
        }
        internal bool PlaceFirst(ItemInstance item, string location)
        {
            for (int y = 0; y < (location == "bag" ? GameplayBalance.BagHeight : GameplayBalance.StashHeight); y++)
                for (int x = 0; x < (location == "bag" ? GameplayBalance.BagWidth : GameplayBalance.StashWidth); x++) if (CanPlace(item, location, x, y))
                { item.Location = location; item.X = x; item.Y = y; return true; }
            return false;
        }
        // Caller mutates a transaction candidate; false means discard the entire candidate.
        internal bool Grant(string owner, string campaign, string templateId, int amount, bool allowReserve)
        {
            var def = ItemCatalog.Find(templateId);
            if ((AdvancedItemCatalog.Find(templateId)!=null && def!=null && def.Kind=="equipment") || NextSerial > long.MaxValue - 1000 || def == null || amount < 1 || amount > 1000) return false;
            if (def.Kind == "quest")
            {
                int old = QuestItems.ContainsKey(templateId) ? QuestItems[templateId] : 0;
                if (old > 1000000 - amount) return false;
                QuestItems[templateId] = old + amount; return true;
            }
            foreach (var item in Items.Where(x => x.Location == "bag" && x.TemplateId == templateId).OrderBy(x => x.Y).ThenBy(x => x.X))
            { int moved = Math.Min(amount, def.MaxStack - item.Quantity); item.Quantity += moved; amount -= moved; }
            while (amount > 0)
            {
                var item = new ItemInstance { Id = Mint(owner, campaign), Owner = owner, TemplateId = templateId,
                    Quantity = Math.Min(amount, def.MaxStack), Location = "reserve", X = -1, Y = -1 };
                if (!PlaceFirst(item, "bag") && (!allowReserve || Items.Count(x => x.Location == "reserve") >= GameplayBalance.ReserveInstances)) return false;
                Items.Add(item); amount -= item.Quantity;
            }
            return true;
        }
        internal bool Collect(string id)
        {
            var source = Items.FirstOrDefault(x => x.Id == id && x.Location == "reserve");
            if (source == null) return false;
            int original = source.Quantity;
            var def = ItemCatalog.Find(source.TemplateId);
            foreach (var item in Items.Where(x => x.Location == "bag" && x.TemplateId == source.TemplateId).OrderBy(x => x.Y).ThenBy(x => x.X))
            { int n = Math.Min(source.Quantity, def.MaxStack - item.Quantity); item.Quantity += n; source.Quantity -= n; }
            if (source.Quantity == 0) { Items.Remove(source); return true; }
            if (PlaceFirst(source, "bag")) return true;
            return source.Quantity != original;
        }
        internal bool Move(string id, string location, int x, int y)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item == null || (location != "bag" && location != "stash") ||
                (location != "bag" && Equipped.Values.Contains(id)) || !CanPlace(item, location, x, y)) return false;
            item.Location = location; item.X = x; item.Y = y; return true;
        }
        internal bool Split(string owner, string campaign, string id, int amount, string location, int x, int y)
        {
            var source = Items.FirstOrDefault(i => i.Id == id);
            if (source == null || amount < 1 || amount >= source.Quantity || source.Location == "reserve") return false;
            var split = source.Copy(); split.Id = Mint(owner, campaign); split.Quantity = amount;
            if (!CanPlace(split, location, x, y)) return false;
            source.Quantity -= amount; split.Location = location; split.X = x; split.Y = y; Items.Add(split); return true;
        }
        internal bool Merge(string sourceId, string targetId)
        {
            var source = Items.FirstOrDefault(i => i.Id == sourceId); var target = Items.FirstOrDefault(i => i.Id == targetId);
            if (source == null || target == null || source.Id == target.Id || source.TemplateId != target.TemplateId || target.Location == "reserve") return false;
            int n = Math.Min(source.Quantity, ItemCatalog.Find(target.TemplateId).MaxStack - target.Quantity);
            if (n <= 0) return false;
            source.Quantity -= n; target.Quantity += n; if (source.Quantity == 0) Items.Remove(source); return true;
        }
        internal bool Equip(string classId, string slot, string instanceId, int level=30)
        {
            if (slot == null || instanceId == null || !Equipped.ContainsKey(slot)) return false;
            if (instanceId == "") { Equipped[slot] = ""; return true; }
            var item = Items.FirstOrDefault(x => x.Id == instanceId && x.Location == "bag");
            var def = item == null ? null : ItemCatalog.Find(item.TemplateId);
            if (def == null || def.Kind != "equipment" || !AdvancedItemCatalog.SlotMatches(item.TemplateId,slot) || !AdvancedItemCatalog.Compatible(classId,item.TemplateId) || (item.Roll!=null && level<item.Roll.RequirementLevel)) return false;
            if (Equipped.Any(e=>e.Key!=slot && e.Value==instanceId)) return false;
            if (slot=="main_hand" && AdvancedItemCatalog.TwoHands(item.TemplateId) && Equipped.ContainsKey("off_hand") && Equipped["off_hand"]!="")return false;
            if (slot=="off_hand" && Items.Any(i=>i.Id==Equipped["main_hand"] && AdvancedItemCatalog.TwoHands(i.TemplateId)))return false;
            Equipped[slot] = instanceId; return true;
        }
        internal void RemoveOne(string id)
        {
            var item = Items.Single(x => x.Id == id); item.Quantity--;
            if (item.Quantity == 0) Items.Remove(item);
        }
    }
}
