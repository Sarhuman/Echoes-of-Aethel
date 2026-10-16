using System;
using System.Linq;
using Aethel.WorldMemory;

namespace Aethel.Campaign
{
    public sealed class EncounterDefinition
    {
        public string Id, Name, Zone, Behavior, Element, Recipe;
        public EncounterCategory Category;
        public int MinLevel, MaxLevel, Xp, Hp, Defense, Ward, Power, TelegraphMs;
        public bool Undead, Terminal, Optional;
    }
    public static partial class EncounterCatalog
    {
        public static EncounterDefinition[] All { get { return Entries.Select(Clone).ToArray(); } }
        private static EncounterDefinition Clone(EncounterDefinition e)
        {return new EncounterDefinition {Id=e.Id,Name=e.Name,Zone=e.Zone,Behavior=e.Behavior,Element=e.Element,Recipe=e.Recipe,Category=e.Category,MinLevel=e.MinLevel,MaxLevel=e.MaxLevel,Xp=e.Xp,Hp=e.Hp,Defense=e.Defense,Ward=e.Ward,Power=e.Power,TelegraphMs=e.TelegraphMs,Undead=e.Undead,Terminal=e.Terminal,Optional=e.Optional};}
        public static EncounterDefinition Find(string id) { var e=Entries.FirstOrDefault(x=>x.Id==id);return e==null?null:Clone(e); }
        public static bool CanEnter(CampaignState s,string zone)
        {
            string parent=zone=="exp.dungeon.ash_crown"?"dunes_04":zone=="exp.dungeon.lost_names"?"crypt_06":zone;
            var q=CanonicalCampaign.Quests.FirstOrDefault(x=>x.Zone==parent);
            return q!=null && s.Memory.Unlocked(q.Order) && s.Vitals.Hp>0 && s.Defeats.Count<8192 && s.Inventory.Items.Count(x=>x.Location=="reserve")<=GameplayBalance.ReserveInstances-4;
        }
    }
    public interface IEncounterRewardAccess
    {
        bool TryConfirmVictory(string character,string spawn,out string definitionId,out int level);
        // Persisted/authoritative encounter seed. Retry before commit repeats the same roll.
        ILootRandom CreateVictoryRandom(string character,string spawn);
    }
    public sealed class EncounterRewardService
    {
        private readonly ProgressionService commands;
        private readonly IEncounterRewardAccess access;
        public EncounterRewardService(ICampaignStore store,ITrainingAccess training,IEncounterRewardAccess access)
        {commands=new ProgressionService(store,training);this.access=access??throw new ArgumentNullException("access");}
        public ProgressionResult Claim(string token,string spawn,long expected)
        {
            return commands.Run(token,"encounter-reward|"+spawn,expected,s=> {
                if(!CampaignState.ValidId(spawn))return ProgressionStatus.Invalid;
                if(s.Defeats.ContainsKey(spawn))return ProgressionStatus.Replayed;
                string id;int level;
                if(!access.TryConfirmVictory(s.CharacterId,spawn,out id,out level))return ProgressionStatus.Locked;
                var e=EncounterCatalog.Find(id);
                if(e==null || level<e.MinLevel || level>e.MaxLevel || !EncounterCatalog.CanEnter(s,e.Zone))return ProgressionStatus.Locked;
                if(e.Terminal && (e.Optional?s.Defeats.Values.Contains(e.Id):s.Memory.Encounters.Contains(e.Zone)))return ProgressionStatus.Locked; // No implicit boss farm/endgame.
                var b=LootRules.Roll(e.Category,level,s.ClassId,spawn,access.CreateVictoryRandom(s.CharacterId,spawn),e.Optional && e.Terminal);
                if(s.Gold>int.MaxValue-b.Gold)return ProgressionStatus.Locked;
                if(b.Equipment!=null && !s.Inventory.GrantEquipment(s.CharacterId,s.Memory.CampaignId,b.TemplateId,b.Equipment))return ProgressionStatus.Locked;
                if(b.Consumable!=null && !s.Inventory.Grant(s.CharacterId,s.Memory.CampaignId,b.Consumable,1,true))return ProgressionStatus.Locked;
                if(b.Material && !s.Inventory.Grant(s.CharacterId,s.Memory.CampaignId,"exp.material",1,true))return ProgressionStatus.Locked;
                if(b.Essence && !s.Inventory.Grant(s.CharacterId,s.Memory.CampaignId,"exp.essence",1,true))return ProgressionStatus.Locked;
                s.Gold+=b.Gold;s.TotalXp=Math.Min(ExperienceRules.MinimumForLevel(30),s.TotalXp+e.Xp);s.Level=ExperienceRules.LevelForTotal(s.TotalXp);s.Defeats.Add(spawn,e.Id);
                if(e.Terminal && !e.Optional)s.Memory.Encounters.Add(e.Zone);
                if(e.Recipe!="" && !s.Loot.Recipes.Contains(e.Recipe)){s.Loot.Recipes.Add(e.Recipe);s.Loot.Fragments.Add(e.Id);}
                return ProgressionStatus.Accepted;
            });
        }
    }
}
