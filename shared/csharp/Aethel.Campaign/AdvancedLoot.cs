using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Aethel.Campaign
{
    public interface ILootRandom { int Next(int exclusiveMax); }
    public sealed class SeededLootRandom : ILootRandom
    {
        private readonly Random random;
        public SeededLootRandom(int seed) { random=new Random(seed); }
        public int Next(int max) { return random.Next(max); }
    }
    public sealed class AffixDefinition
    {
        public string Id {get;internal set;}
        public string Group {get;internal set;}
        public string Stat {get;internal set;}
        public string Kind {get;internal set;}
        public int MinimumLevel {get;internal set;}
        public int Minimum {get;internal set;}
        public int Maximum {get;internal set;}
        public int Weight {get;internal set;}
        public int MinimumRarity {get;internal set;}
    }
    [DataContract]
    public sealed class AffixRoll
    {
        [DataMember(IsRequired=true)] public string Id;
        [DataMember(IsRequired=true)] public int Value;
        [DataMember(IsRequired=true)] public int Tier=1;
        internal AffixRoll Copy() { return (AffixRoll)MemberwiseClone(); }
    }
    [DataContract]
    public sealed class EquipmentRoll
    {
        [DataMember(IsRequired=true)] public string Rarity, SourceSpawn, UniqueEffect;
        [DataMember(IsRequired=true)] public int Level, RequirementLevel;
        [DataMember(IsRequired=true)] public List<AffixRoll> Affixes=new List<AffixRoll>();
        internal EquipmentRoll Copy() { return new EquipmentRoll { Rarity=Rarity,SourceSpawn=SourceSpawn,UniqueEffect=UniqueEffect,Level=Level,RequirementLevel=RequirementLevel,Affixes=Affixes.Select(x=>x.Copy()).ToList() }; }
        internal void Validate(string template)
        {
            var d=AdvancedItemCatalog.Find(template);int rarity=Array.IndexOf(LootRules.Rarities,Rarity);
            if(d==null || rarity<0 || Level<1 || Level>30 || RequirementLevel!=Level || !CampaignState.ValidId(SourceSpawn) || Affixes==null || Affixes.Any(x=>x==null) ||
                UniqueEffect!=(rarity==4?AdvancedItemCatalog.UniqueFor(template):"") || Affixes.Count!=LootRules.PrefixCount[rarity]+LootRules.SuffixCount[rarity] || Affixes.Select(x=>x.Id).Distinct().Count()!=Affixes.Count)
                throw new SerializationException("Invalid equipment roll");
            var defs=Affixes.Select(x=>AdvancedItemCatalog.Affixes.FirstOrDefault(a=>a.Id==x.Id)).ToArray();
            if(defs.Any(x=>x==null) || defs.Select(x=>x.Group).Distinct().Count()!=defs.Length || defs.Count(x=>x.Kind=="prefix")!=LootRules.PrefixCount[rarity] || defs.Count(x=>x.Kind=="suffix")!=LootRules.SuffixCount[rarity] ||
                Affixes.Any(x=> { var a=AdvancedItemCatalog.Affixes.Single(y=>y.Id==x.Id);return a.MinimumLevel>Level || a.MinimumRarity>rarity || x.Tier!=1+(Level-1)/10 || x.Value<a.Minimum*x.Tier || x.Value>a.Maximum*x.Tier; })) throw new SerializationException("Invalid affix roll");
        }
        public int Stat(string name) { return Affixes.Where(x=>AdvancedItemCatalog.Affixes.Single(a=>a.Id==x.Id).Stat==name).Sum(x=>x.Value); }
    }
    public static class AdvancedItemCatalog
    {
        // NEW/EXPERIMENTAL content v1. No visual/asset paths in saves.
        private static readonly string[] SlotEntries={"main_hand","off_hand","head","chest","hands","feet","ring_1","ring_2","neck"};
        public static string[] Slots {get{return (string[])SlotEntries.Clone();}}
        private static readonly string[] Families={"axe","sword","dagger","staff","mace","shield","head","chest","hands","feet","ring","neck"};
        private static readonly ItemTemplate[] Templates=Families.Select((f,i)=>new ItemTemplate { Id="exp.gear."+f,Kind="equipment",Slot=i<5?"main_hand":i==5?"off_hand":f=="ring"?"ring_1":f,Width=i<5?1:i==7?2:1,Height=i<5?3:i==7?3:1,MaxStack=1,Power=i<5?8:0,Armor=i>=5 && i<=9?2:0 }).Concat(new[]{new ItemTemplate { Id="exp.material",Kind="material",Slot="",Width=1,Height=1,MaxStack=20 },new ItemTemplate { Id="exp.essence",Kind="material",Slot="",Width=1,Height=1,MaxStack=20 }}).ToArray();
        public static ItemTemplate Find(string id) { return Templates.FirstOrDefault(x=>x.Id==id); }
        public static ItemTemplate[] Equipment { get { return Templates.Where(x=>x.Kind=="equipment").ToArray(); } }
        public static string Family(string id) { return id!=null && id.StartsWith("exp.gear.",StringComparison.Ordinal)?id.Substring(9):id!=null && id.EndsWith("_axe",StringComparison.Ordinal)?"axe":""; }
        public static string UniqueFor(string id) {var d=Find(id);return d.Slot=="main_hand"?"unique.tempered_edge":d.Slot=="ring_1" || d.Slot=="neck"?"unique.merciful_ember":"unique.steadfast_ward";}
        public static bool TwoHands(string id) { return Family(id)=="staff"; }
        public static bool Compatible(string cls,string id)
        {
            switch(Family(id)) {
                case "axe": case "shield":return cls=="twilight_bastion";
                case "sword":return cls=="twilight_bastion" || cls=="sacred_flame_inquisitor";
                case "dagger":return cls=="shadow_sentinel";
                case "staff":return cls=="rune_weaver" || cls=="thorn_summoner";
                case "mace":return cls=="sacred_flame_inquisitor";
                default:return true;
            }
        }
        public static bool SlotMatches(string template,string slot)
        { var d=ItemCatalog.Find(template);return d!=null && (d.Slot==slot || (d.Slot=="ring_1" && slot=="ring_2")); }
        private static readonly AffixDefinition[] AffixEntries={
            A("prefix.might","strength","str","prefix",1,1,3,100),A("prefix.insight","intelligence","int","prefix",1,1,3,100),
            A("prefix.faith","faith","fth","prefix",1,1,3,100),A("prefix.precision","dexterity","dex","prefix",1,1,3,100),
            A("prefix.force","damage","damage_bp","prefix",1,100,500,50),A("prefix.ward","ward","ward","prefix",1,1,5,100),A("prefix.critical","critical","crit_bp","prefix",1,100,250,50),
            A("suffix.endurance","vitality","vit","suffix",1,1,3,100),A("suffix.armor","armor","armor","suffix",1,1,5,100),
            A("suffix.healing","healing","healing_bp","suffix",1,100,500,50),A("suffix.resistance","resistance","resist_bp","suffix",1,100,500,100),A("suffix.ferocity","ferocity","crit_multiplier_bp","suffix",1,100,500,25,2)
        };
        public static AffixDefinition[] Affixes {get{return (AffixDefinition[])AffixEntries.Clone();}}
        private static AffixDefinition A(string id,string group,string stat,string kind,int level,int min,int max,int weight,int minimumRarity=0)
        { return new AffixDefinition { Id=id,Group=group,Stat=stat,Kind=kind,MinimumLevel=level,Minimum=min,Maximum=max,Weight=weight,MinimumRarity=minimumRarity }; }
    }
    public sealed class LootBundle
    {
        public string TemplateId;
        public EquipmentRoll Equipment;
        public int Gold;
        public string Consumable;
        public bool Material, Essence;
    }
    public static class LootRules
    {
        static LootRules() {ValidateCatalog();}
        private static readonly string[] RarityEntries={"common","magic","rare","epic","legendary"};
        public static string[] Rarities {get{return (string[])RarityEntries.Clone();}}
        internal static readonly int[] PrefixCount={0,1,1,2,2},SuffixCount={0,0,1,1,2};
        private static readonly int[,] Chances={{1200,6000,1500,500,0,1,5},{6500,10000,3500,7000,500,8,20},{10000,10000,6000,10000,10000,25,50},{10000,10000,10000,10000,10000,60,120}};
        private static readonly int[][] RarityWeights={new[]{7500,2500,0,0,0},new[]{0,6000,3800,200,0},new[]{0,0,7500,2500,0},new[]{0,0,0,9000,1000}};
        private static int Next(ILootRandom r,int max) { int n=r.Next(max);if(n<0 || n>=max)throw new ArgumentException("RNG contract violated");return n; }
        public static int WeightedIndex(int[] weights,int roll)
        { if(weights==null || weights.Length==0 || weights.Any(x=>x<0) || weights.Sum()<=0 || roll<0 || roll>=weights.Sum())throw new ArgumentOutOfRangeException();for(int i=0;i<weights.Length;i++){if(roll<weights[i])return i;roll-=weights[i];}throw new InvalidOperationException(); }
        public static LootBundle Roll(EncounterCategory category,int level,string cls,string spawn,ILootRandom rng,bool guaranteedCompatibleRare=false)
        {
            if(!Enum.IsDefined(typeof(EncounterCategory),category) || level<1 || level>30 || !SpecializationCatalog.Classes.Contains(cls) || !CampaignState.ValidId(spawn) || rng==null)throw new ArgumentException("Invalid loot request");
            int c=(int)category;var b=new LootBundle();
            if(Next(rng,10000)<Chances[c,0])
            {
                int rarity=WeightedIndex(RarityWeights[c],Next(rng,10000));if(guaranteedCompatibleRare)rarity=2;
                bool favored=Next(rng,10000)<8000 || guaranteedCompatibleRare;var pool=AdvancedItemCatalog.Equipment.Where(x=>AdvancedItemCatalog.Compatible(cls,x.Id)==favored).ToArray();
                if(pool.Length==0)pool=AdvancedItemCatalog.Equipment;
                b.TemplateId=pool[Next(rng,pool.Length)].Id;
                b.Equipment=new EquipmentRoll { Rarity=Rarities[rarity],Level=level,RequirementLevel=level,SourceSpawn=spawn,UniqueEffect=rarity==4?AdvancedItemCatalog.UniqueFor(b.TemplateId):"" };
                var candidates=AdvancedItemCatalog.Affixes.Where(x=>x.MinimumLevel<=level && x.MinimumRarity<=rarity).ToList();
                for(int kind=0;kind<2;kind++) for(int n=0;n<(kind==0?PrefixCount[rarity]:SuffixCount[rarity]);n++)
                {
                    var poolAffix=candidates.Where(x=>x.Kind==(kind==0?"prefix":"suffix")).ToArray();var weights=poolAffix.Select(x=>x.Weight).ToArray();
                    var a=poolAffix[WeightedIndex(weights,Next(rng,weights.Sum()))];
                    b.Equipment.Affixes.Add(new AffixRoll { Id=a.Id,Tier=1+(level-1)/10,Value=a.Minimum*(1+(level-1)/10)+Next(rng,(a.Maximum-a.Minimum)*(1+(level-1)/10)+1) });candidates.RemoveAll(x=>x.Group==a.Group);
                }
                b.Equipment.Validate(b.TemplateId);
            }
            if(Next(rng,10000)<Chances[c,1])b.Gold=Chances[c,5]+Next(rng,Chances[c,6]-Chances[c,5]+1);
            if(Next(rng,10000)<Chances[c,2])b.Consumable=new[]{"potion_health_small","flask_aether_small","antidote_flask"}[Next(rng,3)];
            b.Material=Next(rng,10000)<Chances[c,3];b.Essence=Next(rng,10000)<Chances[c,4];return b;
        }
        public static void ValidateCatalog()
        {
            if(RarityWeights.Any(x=>x.Sum()!=10000))throw new InvalidOperationException("Rarity weights");
            for(int level=1;level<=30;level++)
            { var a=AdvancedItemCatalog.Affixes.Where(x=>x.MinimumLevel<=level).ToArray();if(a.Where(x=>x.Kind=="prefix").Select(x=>x.Group).Distinct().Count()<2 || a.Where(x=>x.Kind=="suffix").Select(x=>x.Group).Distinct().Count()<2)throw new InvalidOperationException("Insufficient affix pool"); }
        }
    }
    [DataContract]
    public sealed class LootState
    {
        [DataMember(IsRequired=true)] public List<string> Recipes=new List<string>();
        [DataMember(IsRequired=true)] public List<string> Fragments=new List<string>();
        internal LootState Copy() {return new LootState {Recipes=new List<string>(Recipes),Fragments=new List<string>(Fragments)};}
        internal void Validate() {if(Recipes==null || Fragments==null || Recipes.Count>4 || Fragments.Count>4 || Recipes.Distinct().Count()!=Recipes.Count || Fragments.Distinct().Count()!=Fragments.Count || Recipes.Any(x=>!EncounterCatalog.All.Any(e=>e.Recipe==x && x!="")) || Fragments.Any(x=>!EncounterCatalog.All.Any(e=>e.Id==x && e.Recipe!="")))throw new SerializationException("Invalid loot unlocks");if(EncounterCatalog.All.Where(e=>e.Recipe!="").Any(e=>Recipes.Contains(e.Recipe)!=Fragments.Contains(e.Id)))throw new SerializationException("Recipe/fragment mismatch");}
    }
}
