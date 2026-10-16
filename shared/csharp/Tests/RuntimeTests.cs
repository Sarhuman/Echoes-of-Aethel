using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Aethel.Campaign;
using Aethel.WorldMemory;

internal sealed class RuntimeAccess : IGameplayAccess
{
    internal bool Stash = true, Shop = true, Abilities = true, Defeat = true, Front = true;
    internal string Enemy = "ethereal_wolf";
    internal long BlockTime;
    internal bool HostileDamage = true, Recover = true, OutOfCombat;
    internal long DamageSequence = 1;
    internal int Damage = 200;
    internal HitEvidence Hit = new HitEvidence { TargetId = "enemy.1", Category = EncounterCategory.Common, HpDamage = 10 };
    public bool CanUseStash(string id) { return Stash; }
    public bool CanShop(string id) { return Shop; }
    public bool CanRecoverAtHub(string id) { return Recover; }
    public bool IsOutOfCombat(string id) { return OutOfCombat; }
    public bool CanUseAbilities(string id) { return Abilities; }
    public bool CanReceiveHostileDamage(string id) { return HostileDamage; }
    public bool TryGetConfirmedDamage(string id,string ev,out long sequence,out int damage) { sequence=DamageSequence;damage=Damage;return true; }
    public bool TryGetConfirmedDefeat(string id, string spawn, out string enemy) { enemy = Enemy; return Defeat; }
    public bool TryGetConfirmedBlock(string id, string ev, out bool front, out int absorbed, out long observedAt) { front = Front; absorbed = 5; observedAt = BlockTime; return true; }
    public bool TryGetConfirmedImpactHit(string id, string ev, string action, out HitEvidence hit) { hit = Hit; return true; }
}
internal static class RuntimeTests
{
    static int checks;
    static string T() { return Guid.NewGuid().ToString("D"); }
    static void C(bool ok, string message) { checks++; if (!ok) throw new Exception(message); }
    static void S(ProgressionResult r, ProgressionStatus status) { C(r.Status == status,"Expected " + status + " got " + r.Status); }
    static GameplayService Service(CampaignTestStore store, RuntimeAccess access) { return new GameplayService(store,new TrainingTestAccess(),access); }
    static void FillBag(CampaignState s)
    {
        s.Inventory = new InventoryState();
        C(s.Inventory.Grant(s.CharacterId,s.Memory.CampaignId,"wolf_fang",1000,false),"Fill first stacks");
        C(s.Inventory.Grant(s.CharacterId,s.Memory.CampaignId,"wolf_fang",200,false),"Fill remaining stacks");
        s.Validate();
    }
    static void Inventory()
    {
        var store = new CampaignTestStore(); var access = new RuntimeAccess(); var service = Service(store,access);
        C(store.State.Inventory.Items.Count == 2 && store.State.Inventory.WeaponPower == 8 && store.State.Inventory.Armor == 2,"Starter references");
        var armor = store.State.Inventory.Items.Single(i => i.TemplateId == "worn_tunic");
        S(service.MoveItem(T(),armor.Id,"stash",0,0,service.Read().Version),ProgressionStatus.Locked);
        S(service.Equip(T(),"chest","",service.Read().Version),ProgressionStatus.Accepted);
        access.Stash = false; S(service.MoveItem(T(),armor.Id,"stash",0,0,service.Read().Version),ProgressionStatus.Locked); access.Stash = true;
        string move = T(); long old = service.Read().Version;
        store.Fail = true; S(service.MoveItem(move,armor.Id,"stash",0,0,old),ProgressionStatus.PersistenceFailed);
        C(store.State.Inventory.Items.Single(i => i.Id == armor.Id).Location == "bag","Failed move lost ownership"); store.Fail = false;
        S(service.MoveItem(move,armor.Id,"stash",0,0,old),ProgressionStatus.Accepted);
        S(service.MoveItem(move,armor.Id,"stash",0,0,old),ProgressionStatus.Replayed);
        S(service.Equip(T(),"chest",armor.Id,service.Read().Version),ProgressionStatus.Invalid);
        S(service.MoveItem(T(),armor.Id,"bag",9,5,service.Read().Version),ProgressionStatus.Locked);
        S(service.MoveItem(T(),armor.Id,"bag",0,0,service.Read().Version),ProgressionStatus.Locked); // overlaps axe
        S(service.MoveItem(T(),armor.Id,"bag",1,0,service.Read().Version),ProgressionStatus.Accepted);
        S(service.Equip(T(),"chest",armor.Id,service.Read().Version),ProgressionStatus.Accepted);
        store.State.Inventory.Grant(store.State.CharacterId,store.State.Memory.CampaignId,"potion_health_small",20,false);
        var stack = store.State.Inventory.Items.Single(i => i.TemplateId == "potion_health_small");
        string split = T(); old = service.Read().Version;
        S(service.SplitStack(split,stack.Id,7,"bag",5,0,old),ProgressionStatus.Accepted);
        S(service.SplitStack(split,stack.Id,7,"bag",5,0,old),ProgressionStatus.Replayed);
        C(store.State.Inventory.Total("potion_health_small") == 20,"Split duplicated quantities");
        var other = store.State.Inventory.Items.Single(i => i.TemplateId == "potion_health_small" && i.Id != stack.Id);
        S(service.MergeStacks(T(),other.Id,stack.Id,service.Read().Version),ProgressionStatus.Accepted);
        C(store.State.Inventory.Items.Count(i => i.TemplateId == "potion_health_small") == 1,"Merge retained empty stack");
        var invalid = store.State.Copy(); invalid.Inventory.Items[1].Id = invalid.Inventory.Items[0].Id;
        bool rejected = false; try { invalid.Validate(); } catch (SerializationException) { rejected = true; } C(rejected,"Duplicate instance accepted");
        FillBag(store.State);
        C(store.State.Inventory.Grant(store.State.CharacterId,store.State.Memory.CampaignId,"potion_health_small",3,true),"Reserve reward failed");
        var reserved = store.State.Inventory.Items.Single(i => i.Location == "reserve");
        S(service.CollectReserve(T(),reserved.Id,service.Read().Version),ProgressionStatus.Locked);
        var bagItem = store.State.Inventory.Items.First(i => i.Location == "bag");
        service.MoveItem(T(),bagItem.Id,"stash",0,0,service.Read().Version);
        string collect = T(); old = service.Read().Version;
        S(service.CollectReserve(collect,reserved.Id,old),ProgressionStatus.Accepted);
        S(service.CollectReserve(collect,reserved.Id,old),ProgressionStatus.Replayed);
        C(store.State.Inventory.Total("potion_health_small") == 3 && store.State.Inventory.Items.Single(i=>i.Id==reserved.Id).Location=="bag","Reserve duplicated/lost item");
        FillBag(store.State);
        C(store.State.Inventory.Grant(store.State.CharacterId,store.State.Memory.CampaignId,"wolf_fang",1000,true),"Reserve fill");
        C(store.State.Inventory.Grant(store.State.CharacterId,store.State.Memory.CampaignId,"wolf_fang",1000,true),"Reserve fill 2");
        var partial = store.State.Inventory.Items.First(i=>i.Location=="bag");partial.TemplateId="potion_health_small";partial.Quantity=19;
        var memory = new MemoryService(new CampaignMemoryAdapter(store));
        foreach(var clue in MemoryService.ForestClues) memory.ObserveClue(clue,memory.Read().Version);
        var result = memory.Decide("act1.spy_hunt","shelter_villagers",memory.Read().Version);
        C(result.Status==CommandStatus.PersistenceFailed && !store.State.ForestRewardClaimed && store.State.RitualCharge==0,"Full reserve accepted partial quest");
        C(store.State.Inventory.Total("potion_health_small")==19,"Failed reward partially merged stack");
        C(store.State.Inventory.Grant(store.State.CharacterId,store.State.Memory.CampaignId,"root_essence",1,true),"Quest item blocked by full bag/reserve");
    }
    static void XpAndConsumables()
    {
        for(int l=1;l<=30;l++)
        {
            C(ExperienceRules.LevelForTotal(ExperienceRules.MinimumForLevel(l))==l,"XP threshold");
            if(l>1) C(ExperienceRules.LevelForTotal(ExperienceRules.MinimumForLevel(l)-1)==l-1,"XP threshold minus one");
        }
        var store = new CampaignTestStore(); var access = new RuntimeAccess(); var service = Service(store,access);
        string token = T(); long version = service.Read().Version;
        store.Fail=true; S(service.AwardDefeat(token,"forest_01.ethereal_wolf.01",version),ProgressionStatus.PersistenceFailed);
        C(store.State.TotalXp==0 && store.State.Defeats.Count==0,"XP committed on failure"); store.Fail=false;
        S(service.AwardDefeat(token,"forest_01.ethereal_wolf.01",version),ProgressionStatus.Accepted);
        S(service.AwardDefeat(token,"forest_01.ethereal_wolf.01",version),ProgressionStatus.Replayed);
        S(service.AwardDefeat(T(),"forest_01.ethereal_wolf.01",service.Read().Version),ProgressionStatus.Replayed);
        C(store.State.TotalXp==10,"Death replay doubled XP");
        access.Enemy="moss_claws";
        for(int i=0;i<4;i++) service.AwardDefeat(T(),"test.moss."+i,service.Read().Version);
        C(store.State.Level==5 && store.State.EarnedSpecializationPoints==1 && CharacterRules.UnspentAttributes(store.State)==12,"Multi-level progression");
        C(store.State.Vitals.Hp==172 && store.State.Vitals.Mp==52,"Level-up healed resources");
        S(service.AllocateAttribute(T(),4,3,service.Read().Version),ProgressionStatus.Accepted);
        C(store.State.Vitals.Hp==172 && CharacterRules.MaxHp(store.State)==220,"Attribute allocation healed or wrong formula");
        store.State.Gold=100;
        access.Shop=false; S(service.BuyPotion(T(),"potion_health_small",service.Read().Version),ProgressionStatus.Locked); access.Shop=true;
        string buy=T();version=service.Read().Version;
        S(service.BuyPotion(buy,"potion_health_small",version),ProgressionStatus.Accepted);
        S(service.BuyPotion(buy,"potion_health_small",version),ProgressionStatus.Replayed);
        C(store.State.Gold==90 && store.State.Inventory.Total("potion_health_small")==1,"Purchase replay");
        var potion=store.State.Inventory.Items.Single(i=>i.TemplateId=="potion_health_small");
        store.State.Vitals.Hp=CharacterRules.MaxHp(store.State);
        S(service.UseConsumable(T(),potion.Id,service.Read().Version),ProgressionStatus.Locked);
        store.State.Vitals.Hp=1; string use=T();version=service.Read().Version;
        store.Fail=true; S(service.UseConsumable(use,potion.Id,version),ProgressionStatus.PersistenceFailed);
        C(store.State.Vitals.Hp==1 && store.State.Inventory.Total("potion_health_small")==1,"Failed potion lost item/healed");store.Fail=false;
        S(service.UseConsumable(use,potion.Id,version),ProgressionStatus.Accepted);
        S(service.UseConsumable(use,potion.Id,version),ProgressionStatus.Replayed);
        C(store.State.Vitals.Hp==51 && store.State.Inventory.Total("potion_health_small")==0,"Potion effect/replay");
        service.BuyPotion(T(),"flask_aether_small",service.Read().Version);
        var flask=store.State.Inventory.Items.Single(i=>i.TemplateId=="flask_aether_small"); store.State.Vitals.Mp=1;
        S(service.UseConsumable(T(),flask.Id,service.Read().Version),ProgressionStatus.Locked);
        S(service.AdvanceSimulationTo(4999,service.Read().Version),ProgressionStatus.Accepted);
        S(service.UseConsumable(T(),flask.Id,service.Read().Version),ProgressionStatus.Locked);
        S(service.AdvanceSimulationTo(5000,service.Read().Version),ProgressionStatus.Accepted);
        S(service.UseConsumable(T(),flask.Id,service.Read().Version),ProgressionStatus.Accepted);
        C(store.State.Vitals.Mp==31,"MP consumable");
        store.State.Inventory.Grant(store.State.CharacterId,store.State.Memory.CampaignId,"antidote_flask",1,false);
        var antidote=store.State.Inventory.Items.Single(i=>i.TemplateId=="antidote_flask");
        S(service.UseConsumable(T(),antidote.Id,service.Read().Version),ProgressionStatus.Locked);
        store.State.Vitals.RemovablePoison=true; S(service.UseConsumable(T(),antidote.Id,service.Read().Version),ProgressionStatus.Accepted);
        C(!store.State.Vitals.RemovablePoison,"Antidote failed");
        var clock=store.State.SimulationTime;
        S(service.AdvanceSimulationTo(0,service.Read().Version),ProgressionStatus.Replayed);
        C(store.State.SimulationTime==clock,"Clock went backwards");
        S(service.AdvanceSimulationTo(clock+60001,service.Read().Version),ProgressionStatus.Invalid);
    }
    static CampaignTestStore ReadyBastion(string path, out GameplayService service, out RuntimeAccess access)
    {
        var store=new CampaignTestStore();access=new RuntimeAccess();service=Service(store,access);
        var progress=new ProgressionService(store,new TrainingTestAccess());
        progress.SetConfirmedLevel(T(),5,progress.Read().Version);
        service.LearnBasic(T(),"twilight_bastion.a2",service.Read().Version);
        service.LearnBasic(T(),"twilight_bastion.a3",service.Read().Version);
        service.LearnBasic(T(),"twilight_bastion.p1",service.Read().Version);
        progress.SelectPath(T(),path,progress.Read().Version);return store;
    }
    static void Effects()
    {
        GameplayService service; RuntimeAccess access;
        var store=ReadyBastion("twilight_bastion.defense",out service,out access);
        S(service.UseGuard(T(),service.Read().Version),ProgressionStatus.Accepted);
        access.Front=false;S(service.ConfirmBlock(T(),"event.rear",service.Read().Version),ProgressionStatus.Locked);access.Front=true;
        string block=T(); long version=service.Read().Version;
        store.Fail=true;S(service.ConfirmBlock(block,"event.front",version),ProgressionStatus.PersistenceFailed);
        C(store.State.Effects.CounterExpiresAt==0,"Failed block prepared charge");store.Fail=false;
        S(service.ConfirmBlock(block,"event.front",version),ProgressionStatus.Accepted);
        C(store.State.Effects.CounterExpiresAt==3000 && GameplayService.GuardReductionBp(store.State)==5000,"Counter/posture");
        service.AdvanceSimulationTo(1000,service.Read().Version);
        S(service.ConfirmBlock(T(),"event.second",service.Read().Version),ProgressionStatus.Accepted);
        C(store.State.Effects.CounterExpiresAt==3000,"Charge refreshed inside internal cooldown");
        store.State.Vitals.Mp=0;S(service.UseImpact(T(),service.Read().Version),ProgressionStatus.Locked);
        C(store.State.Effects.CounterExpiresAt==3000,"Refused attack consumed charge");store.State.Vitals.Mp=30;
        string impact=T();version=service.Read().Version;
        store.Fail=true;S(service.UseImpact(impact,version),ProgressionStatus.PersistenceFailed);
        C(store.State.Vitals.Mp==30 && store.State.Effects.CounterExpiresAt==3000,"Failed impact changed resource/charge");store.Fail=false;
        S(service.UseImpact(impact,version),ProgressionStatus.Accepted);
        S(service.UseImpact(impact,version),ProgressionStatus.Replayed);
        C(store.State.Vitals.Mp==18 && store.State.Effects.CounterExpiresAt==0 && store.State.Effects.ImpactBonus[impact]==1500,"Impact resource/snapshot");
        C(BastionEffects.ApplyDamageBonus(100,9500,1500)==200,"Bonus cap");
        var progress=new ProgressionService(store,new TrainingTestAccess());
        long cooldown=store.State.Effects.ImpactReadyAt;int hp=store.State.Vitals.Hp,mp=store.State.Vitals.Mp;
        progress.Respec(T(),"twilight_bastion.rupture",new[]{"twilight_bastion.rupture.t1"},progress.Read().Version);
        C(store.State.Effects.ImpactReadyAt==cooldown && store.State.Vitals.Hp==hp && store.State.Vitals.Mp==mp,"Respec reset cooldown or healed");
        store=ReadyBastion("twilight_bastion.rupture",out service,out access);
        impact=T();service.UseImpact(impact,service.Read().Version);
        C(BastionEffects.DefenseAt(store.State.Effects,"enemy.1",100,0)==100,"Rupture before first hit");
        S(service.ConfirmImpactHit(T(),impact,"hit.1",service.Read().Version),ProgressionStatus.Accepted);
        C(BastionEffects.DefenseAt(store.State.Effects,"enemy.1",100,0)==85,"Ordinary rupture strength");
        service.AdvanceSimulationTo(1000,service.Read().Version);
        S(service.ConfirmImpactHit(T(),impact,"hit.retry",service.Read().Version),ProgressionStatus.Replayed);
        C(store.State.Effects.Ruptures["enemy.1"].ExpiresAt==5000,"Duplicate hit refreshed effect");
        access.Hit=new HitEvidence{TargetId="boss.1",Category=EncounterCategory.Boss,BarrierDamage=3};
        S(service.ConfirmImpactHit(T(),impact,"hit.boss",service.Read().Version),ProgressionStatus.Accepted);
        C(BastionEffects.DefenseAt(store.State.Effects,"boss.1",100,1000)==92,"Boss rupture/rounding");
        service.AdvanceSimulationTo(5000,service.Read().Version);
        C(BastionEffects.DefenseAt(store.State.Effects,"enemy.1",100,5000)==100,"Rupture expiration");
    }
    static void HighTiersAndLongSession()
    {
        GameplayService service;RuntimeAccess access;
        var store=ReadyBastion("twilight_bastion.defense",out service,out access);
        var progress=new ProgressionService(store,new TrainingTestAccess());
        progress.SetConfirmedLevel(T(),30,progress.Read().Version);
        foreach(var tier in new[]{2,3,4})progress.BuyNode(T(),"twilight_bastion.defense.t"+tier,progress.Read().Version);
        int mp=store.State.Vitals.Mp;
        service.UseGuard(T(),service.Read().Version);service.ConfirmBlock(T(),"high.block",service.Read().Version);
        C(GameplayService.GuardReductionBp(store.State)==5500 && store.State.Vitals.Mp==mp-6,"High-tier guard");
        string cast=T();service.UseImpact(cast,service.Read().Version);
        C(store.State.Effects.ImpactBonus[cast]==3000,"High-tier counter");
        // Respec cannot retroactively add rupture to an already-issued defensive action.
        progress.Respec(T(),"twilight_bastion.rupture",new[]{"twilight_bastion.rupture.t1","twilight_bastion.rupture.t2","twilight_bastion.rupture.t3","twilight_bastion.rupture.t4"},progress.Read().Version);
        service.ConfirmImpactHit(T(),cast,"late.pre-respec",service.Read().Version);
        C(!store.State.Effects.Ruptures.ContainsKey("enemy.1"),"Respec rewrote old action snapshot");
        service.AdvanceSimulationTo(7000,service.Read().Version);
        store.State.Vitals.Mp=50;access.Hit=new HitEvidence{TargetId="high.target",Category=EncounterCategory.Common,HpDamage=1,SimulationTime=7000};
        cast=T();service.UseImpact(cast,service.Read().Version);service.ConfirmImpactHit(T(),cast,"high.hit",service.Read().Version);
        C(store.State.Vitals.Mp==41 && BastionEffects.DefenseAt(store.State.Effects,"high.target",100,7000)==80 && store.State.Effects.Ruptures["high.target"].ExpiresAt==14000,"High-tier rupture");
        for(int i=1;i<=550;i++)
        {
            service.AdvanceSimulationTo(7000L*(i+1),service.Read().Version);store.State.Vitals.Mp=50;
            S(service.UseImpact(T(),service.Read().Version),ProgressionStatus.Accepted);
        }
        C(store.State.Commands.Count==512 && store.State.ReceiptFloor>0 && store.State.Effects.ImpactBonus.Count<=32 && store.State.Effects.Ruptures.Count==0,"Long session unbounded or stalled");
        C(store.State.Inventory.Total("training_axe")==1,"Long session duplicated starter");
        service.UseGuard(T(),service.Read().Version);access.BlockTime=0;
        S(service.ConfirmBlock(T(),"old.block.reused",service.Read().Version),ProgressionStatus.Locked);
        access.BlockTime=store.State.SimulationTime;
        S(service.ConfirmBlock(T(),"fresh.block",service.Read().Version),ProgressionStatus.Accepted);
    }
    static void CombatVectors()
    {
        C(CombatMath.DirectDamage(42,0,false,15000,20,0,0)==35,"Physical vector");
        C(CombatMath.DirectDamage(42,0,true,15000,20,0,0)==52,"Critical floor");
        C(CombatMath.DirectDamage(42,0,true,20000,20,0,0)==70,"Critical cap vector");
        C(CombatMath.DirectDamage(42,0,false,15000,20,0,4000)==21,"Guard vector");
        C(CombatMath.DirectDamage(52,0,false,15000,20,2500,0)==32,"Elemental floor");
        C(CombatMath.DirectDamage(0,0,false,15000,1000,7500,6000)==1,"Minimum damage");
        C(CombatMath.DirectDamage(42,0,false,15000,20,0,0,false)==0,"Miss");
        C(CombatMath.DirectDamage(42,0,false,15000,20,0,0,true,true)==0,"Immunity");
        bool invalid=false;try{CombatMath.DirectDamage(42,0,false,15000,20,0,6001);}catch(ArgumentOutOfRangeException){invalid=true;}C(invalid,"Invalid reduction accepted");
        GameplayService service;RuntimeAccess access;var store=ReadyBastion("twilight_bastion.defense",out service,out access);
        service.UseGuard(T(),service.Read().Version);service.ConfirmBlock(T(),"snapshot.block",service.Read().Version);
        string action=T();service.UseImpact(action,service.Read().Version);
        int before=BastionEffects.ImpactDamage(store.State.Effects,action,"target",0,0);
        store.State.Attributes[0]=3;
        C(BastionEffects.ImpactDamage(store.State.Effects,action,"target",0,0)==before,"Attribute change rewrote old attack");
    }
    static void DeathAndSafeHub()
    {
        GameplayService service;RuntimeAccess access;var store=ReadyBastion("twilight_bastion.defense",out service,out access);
        service.UseGuard(T(),service.Read().Version);service.ConfirmBlock(T(),"death.block",service.Read().Version);
        access.HostileDamage=false;S(service.ApplyConfirmedDamage(T(),"unsafe.in.hub",service.Read().Version),ProgressionStatus.Locked);
        C(store.State.Vitals.Hp==172,"Hostile damage inside safe hub");access.HostileDamage=true;
        string death=T();long v=service.Read().Version;store.Fail=true;
        S(service.ApplyConfirmedDamage(death,"death.hit",v),ProgressionStatus.PersistenceFailed);
        C(store.State.Vitals.Hp==172 && store.State.Effects.CounterExpiresAt==3000,"Failed death partially applied");store.Fail=false;
        S(service.ApplyConfirmedDamage(death,"death.hit",v),ProgressionStatus.Accepted);
        C(store.State.Vitals.Hp==0 && store.State.Effects.CounterExpiresAt==0 && store.State.Effects.GuardExpiresAt==0,"Death did not interrupt effects");
        store.State.Vitals.Hp=100;S(service.ApplyConfirmedDamage(T(),"old.damage.replayed",service.Read().Version),ProgressionStatus.Replayed);
        C(store.State.Vitals.Hp==100,"Old damage reapplied after recovery");
    }
    static void RecoveryAndRegeneration()
    {
        GameplayService service;RuntimeAccess access;var store=ReadyBastion("twilight_bastion.defense",out service,out access);
        service.UseGuard(T(),service.Read().Version);service.ConfirmBlock(T(),"recover.block",service.Read().Version);
        long ready=store.State.Effects.GuardReadyAt;int hp=store.State.Vitals.Hp;
        access.OutOfCombat=true;
        service.AdvanceSimulationTo(5000,service.Read().Version);
        C(store.State.Vitals.Mp==44,"MP regen started before 5-second combat delay");
        for(int i=1;i<=10;i++)service.AdvanceSimulationTo(5000+i*100,service.Read().Version);
        C(store.State.Vitals.Mp==45 && store.State.Vitals.Hp==hp,"Simulation-time MP regen or natural HP heal");
        access.Recover=false;S(service.RecoverAtHub(T(),service.Read().Version),ProgressionStatus.Locked);access.Recover=true;
        S(service.RecoverAtHub(T(),service.Read().Version),ProgressionStatus.Accepted);
        C(store.State.Vitals.Mp==CharacterRules.MaxMp(store.State) && store.State.Effects.GuardReadyAt==ready && store.State.Effects.CounterExpiresAt==0,"Hub recovery reset cooldown or kept charge");
    }
    static void Migration()
    {
        string dir=Path.Combine(Path.GetTempPath(),"aethel-v2-"+T());Directory.CreateDirectory(dir);string path=Path.Combine(dir,"campaign.json");
        try
        {
            var legacy=CampaignState.Create("legacy","character","twilight_bastion");legacy.Schema=1;legacy.Level=5;
            foreach(var clue in MemoryService.ForestClues)legacy.Memory.Clues.Add(clue);
            legacy.Memory.Choices["act1.spy_hunt"]="shelter_villagers";legacy.ForestRewardClaimed=true;legacy.ReservedHealthPotions=3;
            string oldNotice=T();legacy.Commands[oldNotice]="dismiss-notice";legacy.Memory.Version=1;legacy.NoticeDismissed=true;
            legacy.Inventory=null;legacy.Vitals=null;legacy.Effects=null;legacy.Attributes=null;legacy.Defeats=null;legacy.BasicNodes=null;legacy.Validate();
            using(var f=File.Create(path))new DataContractJsonSerializer(typeof(CampaignState)).WriteObject(f,legacy);
            string before=File.ReadAllText(path);
            var disk=new JsonCampaignStore(path,CampaignState.Create("legacy","character","twilight_bastion"));
            var state=disk.Load();
            S(new ProgressionService(disk,new TrainingTestAccess()).DismissNotice(oldNotice,0),ProgressionStatus.Replayed);
            C(state.Schema==2 && state.Level==5 && state.TotalXp==700 && state.Inventory.Total("potion_health_small")==3,"Migration lost XP/reserve");
            C(File.ReadAllText(path)==before,"Read-only migration overwrote original");
            var service=new GameplayService(disk,new TrainingTestAccess(),new RuntimeAccess());
            S(service.LearnBasic(T(),"twilight_bastion.a2",service.Read().Version),ProgressionStatus.Accepted);
            C(File.ReadAllText(path+".bak")==before,"Migration backup not original");
            state=disk.Load();C(state.Schema==2 && state.Inventory.Total("potion_health_small")==3,"Migration repeated reward");
            var progress=new ProgressionService(disk,new TrainingTestAccess());progress.SelectPath(T(),"twilight_bastion.defense",progress.Read().Version);
            service.LearnBasic(T(),"twilight_bastion.a3",service.Read().Version);service.UseGuard(T(),service.Read().Version);
            service.ConfirmBlock(T(),"disk.block",service.Read().Version);string action=T();service.UseImpact(action,service.Read().Version);
            var loaded=disk.Load();C(loaded.Effects.ImpactBonus[action]==1500 && loaded.Vitals.Mp==44,"Effect/resource disk reload");
        }
        finally{Directory.Delete(dir,true);}
    }
    internal static void Run()
    { Inventory();XpAndConsumables();Effects();HighTiersAndLongSession();CombatVectors();DeathAndSafeHub();RecoveryAndRegeneration();Migration();Console.WriteLine(checks+" runtime checks passed: grid, XP, consumables, effects, v1 migration"); }
}
