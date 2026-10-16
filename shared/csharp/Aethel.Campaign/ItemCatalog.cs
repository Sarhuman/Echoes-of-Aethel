// Generated from first-slice .tres; experimental footprints: axe 1x3, chest 2x3, others 1x1.
namespace Aethel.Campaign
{
    public static class ItemCatalog
    {
        private static readonly ItemTemplate[] Items = new[]
        {
            new ItemTemplate { Id = "antidote_flask", Kind = "consumable", Slot = "", Width = 1, Height = 1, MaxStack = 20, Power = 0, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "bastion_axe", Kind = "equipment", Slot = "main_hand", Width = 1, Height = 3, MaxStack = 1, Power = 14, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "bastion_cuirass", Kind = "equipment", Slot = "chest", Width = 2, Height = 3, MaxStack = 1, Power = 0, Armor = 8, Restore = 0 },
            new ItemTemplate { Id = "cultist_ash", Kind = "material", Slot = "", Width = 1, Height = 1, MaxStack = 20, Power = 0, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "flask_aether_small", Kind = "consumable", Slot = "", Width = 1, Height = 1, MaxStack = 20, Power = 0, Armor = 0, Restore = 30 },
            new ItemTemplate { Id = "iron_axe", Kind = "equipment", Slot = "main_hand", Width = 1, Height = 3, MaxStack = 1, Power = 9, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "ironweave_cuirass", Kind = "equipment", Slot = "chest", Width = 2, Height = 3, MaxStack = 1, Power = 0, Armor = 5, Restore = 0 },
            new ItemTemplate { Id = "padded_vest", Kind = "equipment", Slot = "chest", Width = 2, Height = 3, MaxStack = 1, Power = 0, Armor = 3, Restore = 0 },
            new ItemTemplate { Id = "potion_health_small", Kind = "consumable", Slot = "", Width = 1, Height = 1, MaxStack = 20, Power = 0, Armor = 0, Restore = 50 },
            new ItemTemplate { Id = "root_essence", Kind = "quest", Slot = "", Width = 1, Height = 1, MaxStack = 20, Power = 0, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "runed_axe", Kind = "equipment", Slot = "main_hand", Width = 1, Height = 3, MaxStack = 1, Power = 11, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "training_axe", Kind = "equipment", Slot = "main_hand", Width = 1, Height = 3, MaxStack = 1, Power = 8, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "twilight_axe", Kind = "equipment", Slot = "main_hand", Width = 1, Height = 3, MaxStack = 1, Power = 18, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "twilight_cuirass", Kind = "equipment", Slot = "chest", Width = 2, Height = 3, MaxStack = 1, Power = 0, Armor = 12, Restore = 0 },
            new ItemTemplate { Id = "wolf_fang", Kind = "material", Slot = "", Width = 1, Height = 1, MaxStack = 20, Power = 0, Armor = 0, Restore = 0 },
            new ItemTemplate { Id = "worn_tunic", Kind = "equipment", Slot = "chest", Width = 2, Height = 3, MaxStack = 1, Power = 0, Armor = 2, Restore = 0 },
        };
        public static ItemTemplate Find(string id)
        {
            foreach (var item in Items) if (item.Id == id) return item;
            return null;
        }
    }
}
