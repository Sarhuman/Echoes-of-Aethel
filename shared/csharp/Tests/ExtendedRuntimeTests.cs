using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Aethel.Campaign;
using Aethel.WorldMemory;

internal sealed class KitAccess : IClassCombatAccess
{
    internal CombatTarget Target=new CombatTarget { Id="target.1", Category=EncounterCategory.Common };
    internal long At;
    internal string SummonAction;
    internal int Crit=9999;
    internal bool Allow=true, Ack;
    public bool TryConfirmHit(string c,string a,string e,out CombatTarget t,out long at) {t=Target;at=At;return Allow;}
    public bool TryGetLiveTarget(string c,string id,out CombatTarget t) {t=Target;return Allow && Target.Id==id;}
    public int CriticalRoll(string c,string a,string target) {return Crit;}
    public bool HasAppliedEmission(string c,string id) {return Ack;}
    public bool TryConfirmSummonStrike(string c,string kind,string id,out CombatTarget t) {return TryGetLiveTarget(c,id,out t);}
    public bool TryConfirmSummonDamage(string c,string e,out string kind,out string actionId,out long seq,out int damage) {kind="golem";actionId=SummonAction;seq=1;damage=10000;return Allow;}
}
internal sealed class RewardAccess : IEncounterRewardAccess
{
    internal string Enemy="moss_claws";
    internal int Level=4, Seed=9, Rolls;
    internal bool Allow=true;
    public bool TryConfirmVictory(string c,string spawn,out string enemy,out int level) {enemy=Enemy;level=Level;return Allow;}
    public ILootRandom CreateVictoryRandom(string c,string spawn) {Rolls++;return new SeededLootRandom(Seed);}
}
internal sealed class ConstantRandom : ILootRandom
{
    private readonly int value;
    internal ConstantRandom(int n) {value=n;}
    public int Next(int max) {return Math.Min(value,max-1);}
}
internal static class ExtendedRuntimeTests
{
    static int checks;
    static string T() {return Guid.NewGuid().ToString("D");}
    static void C(bool b,string m) {checks++;if(!b)throw new Exception(m);}
    static void S(ProgressionResult r,ProgressionStatus s) {C(r.Status==s,"Expected "+s+" got "+r.Status);}
    static void Invalid(Action a,string message) {bool failed=false;try{a();}catch(SerializationException){failed=true;}C(failed,message);}
    static CampaignTestStore Store(string cls)
    {
        var store=new CampaignTestStore {State=CampaignState.Create("campaign","character",cls)};
        store.State.Level=30;store.State.TotalXp=ExperienceRules.MinimumForLevel(30);
        foreach(string suffix in new[]{"a2","p1","a3","p2","a4"})store.State.BasicNodes.Add(cls+"."+suffix);
        store.State.Vitals.Hp=CharacterRules.MaxHp(store.State);store.State.Vitals.Mp=CharacterRules.MaxMp(store.State);store.State.Validate();return store;
    }
    static void Time(CampaignTestStore store,long t)
    { var g=new GameplayService(store,new TrainingTestAccess(),new RuntimeAccess());S(g.AdvanceSimulationTo(t,g.Read().Version),ProgressionStatus.Accepted); }
    static ClassCombatService Service(CampaignTestStore store,RuntimeAccess access,KitAccess kit)
    {return new ClassCombatService(store,new TrainingTestAccess(),access,kit);}
    static void Kits()
    {
        foreach(string cls in new[]{"rune_weaver","shadow_sentinel","thorn_summoner","sacred_flame_inquisitor"})
        {
            var store=Store(cls);var access=new RuntimeAccess();var evidence=new KitAccess();var svc=Service(store,access,evidence);
            string a=T();long v=store.State.Version;int mp=store.State.Vitals.Mp;
            store.Fail=true;S(svc.Cast(a,1,v),ProgressionStatus.PersistenceFailed);C(store.State.Kit.Actions.Count==0,"Failed cast leaked action");store.Fail=false;
            S(svc.Cast(a,1,v),ProgressionStatus.Accepted);S(svc.Cast(a,1,v),ProgressionStatus.Replayed);C(store.State.Vitals.Mp==mp,"A1 costs MP");
            store.Fail=true;var failedHit=svc.ConfirmHit(T(),a,"hit.failed",store.State.Version);S(failedHit.Transaction,ProgressionStatus.PersistenceFailed);C(failedHit.Emissions.Count==0 && store.State.Kit.Pending.Count==0 && store.State.Kit.Actions.Single().Targets.Count==0,"Failed hit emitted or reserved target");store.Fail=false;
            var hit=svc.ConfirmHit(T(),a,"hit.1",store.State.Version);S(hit.Transaction,ProgressionStatus.Accepted);C(hit.Emissions.Count==1 && store.State.Kit.Pending.Count==1,"Missing persisted combat outbox");
            S(svc.ConfirmHit(T(),a,"hit.2",store.State.Version).Transaction,ProgressionStatus.Replayed);C(store.State.Kit.Pending.Count==1,"Same target damaged twice");
            string emission=hit.Emissions[0].Id;S(svc.AcknowledgeEmission(T(),emission,store.State.Version),ProgressionStatus.Locked);evidence.Ack=true;
            S(svc.AcknowledgeEmission(T(),emission,store.State.Version),ProgressionStatus.Accepted);C(store.State.Kit.Pending.Count==0,"Outbox ack");
            access.Abilities=false;Time(store,1000);S(svc.Cast(T(),2,store.State.Version),ProgressionStatus.Locked);access.Abilities=true;
            store.State.Vitals.Mp=0;S(svc.Cast(T(),2,store.State.Version),ProgressionStatus.Locked);C(store.State.Kit.Actions.Count==1,"Rejected input changed history");
            store.State.Vitals.Mp=CharacterRules.MaxMp(store.State);
            var bad=store.State.Copy();bad.Kit.Actions[0].Targets.Add("target.1");Invalid(bad.Validate,"Duplicate action target accepted");
        }
    }
    static void Weaver()
    {
        var store=Store("rune_weaver");var access=new RuntimeAccess();var evidence=new KitAccess();var svc=Service(store,access,evidence);string frost=T();
        S(svc.Cast(frost,2,0),ProgressionStatus.Accepted);S(svc.ConfirmHit(T(),frost,"frost",store.State.Version).Transaction,ProgressionStatus.Accepted);
        C(store.State.Kit.Targets.Single().SlowBp==2500,"Common slow");Time(store,500);evidence.At=500;
        string basic=T();S(svc.Cast(basic,1,store.State.Version),ProgressionStatus.Accepted);var h=svc.ConfirmHit(T(),basic,"basic",store.State.Version);C(h.Emissions[0].Damage>store.State.Kit.Actions.Single(x=>x.Id==basic).Raw,"Fracture did not proc");
        Time(store,1000);string barrier=T();S(svc.Cast(barrier,4,store.State.Version),ProgressionStatus.Accepted);int barrierHp=store.State.Kit.Barrier;
        access.Damage=5;var gameplay=new GameplayService(store,new TrainingTestAccess(),access);int hp=store.State.Vitals.Hp;
        S(gameplay.ApplyConfirmedDamage(T(),"damage.1",store.State.Version),ProgressionStatus.Accepted);C(store.State.Vitals.Hp==hp && store.State.Kit.Barrier==barrierHp-5,"Barrier not absorbing");
        Time(store,6000);evidence.At=6000;evidence.Target.Category=EncounterCategory.Boss;string frost2=T();S(svc.Cast(frost2,2,store.State.Version),ProgressionStatus.Accepted);svc.ConfirmHit(T(),frost2,"boss.frost",store.State.Version);C(store.State.Kit.Targets.Single().SlowBp==1000,"Boss slow cap");
        Time(store,6500);evidence.At=6500;string fire=T();svc.Cast(fire,3,store.State.Version);svc.ConfirmHit(T(),fire,"fire",store.State.Version);evidence.Target.Id="target.2";svc.ConfirmHit(T(),fire,"fire.2",store.State.Version);
        C(store.State.Kit.Targets.Count(x=>x.Kind=="burn")==2,"Area target registry");Time(store,10000);var ticks=svc.ResolveDueEffects(T(),store.State.Version);C(ticks.Emissions.Count==3 && ticks.Emissions.All(x=>x.Periodic),"Long-frame DoT drain");
        S(svc.ResolveDueEffects(T(),store.State.Version).Transaction,ProgressionStatus.Replayed);C(store.State.Kit.Pending.Count==8,"Ticks replayed or missing");
    }
    static void Sentinel()
    {
        var store=Store("shadow_sentinel");var access=new RuntimeAccess();var evidence=new KitAccess();var svc=Service(store,access,evidence);
        svc.Cast(T(),2,0);C(store.State.Kit.ConcealedUntil==2000,"Conceal duration");Time(store,500);evidence.At=500;string a=T();svc.Cast(a,1,store.State.Version);C(store.State.Kit.ConcealedUntil==0 && store.State.Kit.Actions.Single().BonusBp==2000,"Conceal did not break/consume shadow");
        svc.ConfirmHit(T(),a,"shadow.hit",store.State.Version);Time(store,1000);evidence.At=1000;string trap=T();svc.Cast(trap,3,store.State.Version);svc.ConfirmHit(T(),trap,"trap.hit",store.State.Version);evidence.Target.Id="other";S(svc.ConfirmHit(T(),trap,"trap.second",store.State.Version).Transaction,ProgressionStatus.Locked);
        evidence.Target.Id="target.1";Time(store,1500);evidence.At=1500;string b=T();svc.Cast(b,1,store.State.Version);var h=svc.ConfirmHit(T(),b,"bleed.basic",store.State.Version);C(h.Emissions[0].Damage>store.State.Kit.Actions.Single(x=>x.Id==b).Raw,"Own bleed bonus");
        Time(store,10000);svc.Cast(T(),2,store.State.Version);var g=new GameplayService(store,new TrainingTestAccess(),access);access.Damage=1;g.ApplyConfirmedDamage(T(),"reveal",store.State.Version);C(store.State.Kit.ConcealedUntil==0,"Damage failed to reveal");
    }
    static void Summoner()
    {
        var store=Store("thorn_summoner");var access=new RuntimeAccess();var evidence=new KitAccess();var svc=Service(store,access,evidence);
        svc.Cast(T(),2,0);C(store.State.Kit.Summons["golem"].Defense==30,"Mineral root defense");Time(store,500);svc.Cast(T(),4,store.State.Version);C(store.State.Kit.Summons.Count==2,"Summon count");
        Time(store,2000);evidence.Allow=false;S(svc.SummonStrike(T(),"golem","target.1",store.State.Version).Transaction,ProgressionStatus.Locked);evidence.Allow=true;
        var h=svc.SummonStrike(T(),"golem","target.1",store.State.Version);S(h.Transaction,ProgressionStatus.Accepted);C(h.Emissions.Count==1,"Summon strike missing");S(svc.SummonStrike(T(),"golem","target.1",store.State.Version).Transaction,ProgressionStatus.Locked);
        evidence.SummonAction=store.State.Kit.Summons["golem"].ActionId;S(svc.DamageSummon(T(),"summon.damage",store.State.Version),ProgressionStatus.Accepted);C(store.State.Kit.Summons["golem"].Hp==0,"Summon immortal");S(svc.DamageSummon(T(),"summon.damage",store.State.Version),ProgressionStatus.Replayed);
        Time(store,12000);S(svc.SummonStrike(T(),"beast","target.1",store.State.Version).Transaction,ProgressionStatus.Locked);
        Time(store,15000);svc.Cast(T(),2,store.State.Version);Time(store,17000);svc.Cast(T(),4,store.State.Version);C(store.State.Kit.Summons.Count==2,"Replacement duplicated summons");
        var g=new GameplayService(store,new TrainingTestAccess(),access);access.Damage=10000;g.ApplyConfirmedDamage(T(),"death",store.State.Version);C(store.State.Vitals.Hp==0 && store.State.Kit.Summons.Count==0,"Death leaked summons");
    }
    static void Inquisitor()
    {
        var store=Store("sacred_flame_inquisitor");var access=new RuntimeAccess();var evidence=new KitAccess();var svc=Service(store,access,evidence);
        S(svc.Cast(T(),3,0),ProgressionStatus.Locked);C(store.State.Vitals.Mp==CharacterRules.MaxMp(store.State),"Full HP consumed heal");store.State.Vitals.Hp-=10;svc.Cast(T(),3,0);C(store.State.Vitals.Hp==CharacterRules.MaxHp(store.State),"Heal overflow");
        Time(store,500);evidence.At=500;evidence.Target.Undead=true;string hammer=T();svc.Cast(hammer,2,store.State.Version);var h=svc.ConfirmHit(T(),hammer,"hammer",store.State.Version);C(store.State.Kit.Targets.Single().RemainingTicks==4,"Persistent ember missing");C(h.Emissions[0].Damage>store.State.Kit.Actions.Single().Raw,"Undead bonus");
        Time(store,1000);string cone=T();svc.Cast(cone,4,store.State.Version);evidence.At=1000;S(svc.ConfirmHit(T(),cone,"early",store.State.Version).Transaction,ProgressionStatus.Locked);Time(store,1700);evidence.At=1700;S(svc.ConfirmHit(T(),cone,"cone",store.State.Version).Transaction,ProgressionStatus.Accepted);
    }
    static void Loot()
    {
        LootRules.ValidateCatalog();
        C(LootRules.WeightedIndex(new[]{7500,2500},7499)==0 && LootRules.WeightedIndex(new[]{7500,2500},7500)==1,"Weight boundary");
        foreach(EncounterCategory category in Enum.GetValues(typeof(EncounterCategory)))for(int seed=0;seed<300;seed++)
        {
            var b=LootRules.Roll(category,30,"rune_weaver","spawn",new SeededLootRandom(seed));
            if(b.Equipment!=null) {b.Equipment.Validate(b.TemplateId);C(b.Equipment.Affixes.Select(x=>x.Id).Distinct().Count()==b.Equipment.Affixes.Count,"Repeated affix");}
            if(category==EncounterCategory.Boss)C(b.Equipment!=null && b.Gold>=60 && b.Gold<=120 && b.Material && b.Essence && b.Consumable!=null,"Guaranteed boss columns");
        }
        var legendary=LootRules.Roll(EncounterCategory.Boss,30,"twilight_bastion","boss",new ConstantRandom(9999));C(legendary.Equipment.Rarity=="legendary" && legendary.Equipment.Affixes.Count==4,"Legendary generation");
        legendary.Equipment.Affixes[0].Value=100000;Invalid(()=>legendary.Equipment.Validate(legendary.TemplateId),"Tampered affix accepted");
        // Extremes and all families/slots, including conflicting hands and distinct ring instances.
        var s=Store("rune_weaver").State;var i=s.Inventory;
        Func<EquipmentRoll> roll=()=>new EquipmentRoll {Rarity="common",Level=1,RequirementLevel=1,SourceSpawn="test",UniqueEffect=""};
        C(i.GrantEquipment(s.CharacterId,s.Memory.CampaignId,"exp.gear.staff",roll()),"Grant staff");C(i.Equip(s.ClassId,"main_hand",i.Items.Last().Id,30),"Equip staff");
        C(i.GrantEquipment(s.CharacterId,s.Memory.CampaignId,"exp.gear.shield",roll()),"Grant shield");C(!i.Equip(s.ClassId,"off_hand",i.Items.Last().Id,30),"Invalid class/2H shield");
        var bastion=Store("twilight_bastion").State;bastion.Inventory.GrantEquipment(bastion.CharacterId,bastion.Memory.CampaignId,"exp.gear.staff",roll());C(!bastion.Inventory.Equip(bastion.ClassId,"main_hand",bastion.Inventory.Items.Last().Id),"Wrong weapon class");
        foreach(string slot in new[]{"ring_1","ring_2"}) {i.GrantEquipment(s.CharacterId,s.Memory.CampaignId,"exp.gear.ring",roll());C(i.Equip(s.ClassId,slot,i.Items.Last().Id,30),"Equip ring");}
        C(i.Equipped["ring_1"]!=i.Equipped["ring_2"],"Rings shared instance");C(!i.Equip(s.ClassId,"ring_2",i.Equipped["ring_1"],30),"One ring in two slots");s.Validate();
        var bad=s.Copy();bad.Inventory.Items.First(x=>x.Roll!=null).Roll.Level=31;Invalid(bad.Validate,"Invalid item level");
    }
    static void RewardsAndRoute()
    {
        var store=new CampaignTestStore();var a=new RewardAccess();var svc=new EncounterRewardService(store,new TrainingTestAccess(),a);string token=T();
        store.Fail=true;S(svc.Claim(token,"boss.1",0),ProgressionStatus.PersistenceFailed);C(store.State.Gold==0 && store.State.Defeats.Count==0 && store.State.Memory.Encounters.Count==0,"Partial victory");store.Fail=false;
        S(svc.Claim(token,"boss.1",0),ProgressionStatus.Accepted);int rolls=a.Rolls;long v=store.State.Version;int gold=store.State.Gold;S(svc.Claim(token,"boss.1",0),ProgressionStatus.Replayed);S(svc.Claim(T(),"boss.1",v),ProgressionStatus.Replayed);C(a.Rolls==rolls && store.State.Gold==gold,"Retry rerolled rewards");
        S(svc.Claim(T(),"boss.duplicate",v),ProgressionStatus.Locked);
        var memory=new MemoryService(new CampaignMemoryAdapter(store));foreach(var clue in MemoryService.ForestClues)memory.ObserveClue(clue,memory.Read().Version);
        foreach(var q in CanonicalCampaign.Quests)
        {
            if(q.Order>1) {var e=EncounterCatalog.All.Single(x=>x.Zone==q.Zone && x.Terminal);a.Enemy=e.Id;a.Level=e.MinLevel;S(svc.Claim(T(),"boss."+q.Order,store.State.Version),ProgressionStatus.Accepted);}
            C(store.State.Memory.Encounters.Contains(q.Zone),"Terminal encounter missing");
            if(q.Order==12)C(store.State.Loot.Recipes.Count==4 && store.State.Loot.Fragments.Count==4 && !store.State.Memory.Choices.ContainsKey(q.Id),"Vael reward not before ending");
            var choice=CanonicalCampaign.Quests.First(x=>x.Id==q.Id).Choices[0];
            var r=memory.Decide(q.Id,choice.Id,memory.Read().Version);C(r.Status==CommandStatus.Accepted,"Campaign choice failed "+q.Id);
        }
        C(store.State.Memory.Choices.Count==12,"Full route failed");
        C(EncounterCatalog.CanEnter(store.State,"exp.dungeon.ash_crown") && EncounterCatalog.CanEnter(store.State,"exp.dungeon.lost_names"),"Optional access");
        foreach(var e in EncounterCatalog.All.Where(x=>x.Optional && x.Terminal)) {a.Enemy=e.Id;a.Level=e.MinLevel;int before=store.State.Loot.Fragments.Count;S(svc.Claim(T(),e.Id+".spawn",store.State.Version),ProgressionStatus.Accepted);C(store.State.Loot.Fragments.Count==before,"Dungeon granted fragment");var item=store.State.Inventory.Items.Last(x=>x.Roll!=null);C(item.Roll.Rarity=="rare" && AdvancedItemCatalog.Compatible(store.State.ClassId,item.TemplateId),"Dungeon guaranteed compatible rare");}
        C(EncounterCatalog.All.Length==51 && EncounterCatalog.All.Count(x=>x.Category==EncounterCategory.Common)==33 && !EncounterCatalog.All.Any(x=>x.Zone=="heart_12" && !x.Terminal),"Incomplete encounter catalog");
    }
    static void Persistence()
    {
        string dir=Path.Combine(Path.GetTempPath(),"aethel-extended-"+T());Directory.CreateDirectory(dir);
        try
        {
            string path=Path.Combine(dir,"campaign.json");var initial=CampaignState.Create("campaign","character","rune_weaver");var legacy=initial.Copy();legacy.Schema=2;legacy.Kit=null;legacy.Loot=null;foreach(var k in legacy.Inventory.Equipped.Keys.Where(x=>x!="main_hand" && x!="chest").ToArray())legacy.Inventory.Equipped.Remove(k);legacy.Vitals.Mp-=10;
            using(var f=File.Create(path))new DataContractJsonSerializer(typeof(CampaignState)).WriteObject(f,legacy);
            string original=File.ReadAllText(path);var disk=new JsonCampaignStore(path,initial);C(disk.Load().Schema==3 && disk.Load().Vitals.Mp==legacy.Vitals.Mp,"V2 migration reset resources");C(File.ReadAllText(path)==original,"Read mutated legacy");
            var ev=new KitAccess();var service=new ClassCombatService(disk,new TrainingTestAccess(),new RuntimeAccess(),ev);string a=T();S(service.Cast(a,1,0),ProgressionStatus.Accepted);C(File.ReadAllText(path+".bak")==original,"V2 backup missing");var hit=service.ConfirmHit(T(),a,"hit",1);S(hit.Transaction,ProgressionStatus.Accepted);
            var loaded=disk.Load();C(loaded.Kit.Pending.Count==1 && loaded.Kit.Actions[0].Targets.Count==1,"Lost hit/outbox on reload");S(service.ConfirmHit(T(),a,"hit.reloaded",loaded.Version).Transaction,ProgressionStatus.Replayed);
            var bad=loaded.Copy();bad.Kit.Pending[0].Damage=-1;Invalid(bad.Validate,"Malformed pending damage");
        }
        finally {Directory.Delete(dir,true);}
    }
    static void Specializations()
    {
        foreach(string cls in new[]{"rune_weaver","shadow_sentinel","thorn_summoner","sacred_flame_inquisitor"})foreach(string path in SpecializationCatalog.Paths(cls))
        {
            var store=Store(cls);store.State.PrimaryPath=path;for(int tier=1;tier<=4;tier++)store.State.Nodes.Add(path+".t"+tier);store.State.Validate();
            var svc=Service(store,new RuntimeAccess(),new KitAccess());
            for(int slot=1;slot<=4;slot++)
            {
                if(slot==3 && cls=="sacred_flame_inquisitor")store.State.Vitals.Hp-=100;
                S(svc.Cast(T(),slot,store.State.Version),ProgressionStatus.Accepted);store.State.Validate();Time(store,store.State.SimulationTime+2000);
            }
            C(store.State.Kit.Actions.Count<=32,"Specialization bounded actions");
        }
    }

    static void SetPath(CampaignTestStore store,string path)
    {store.State.PrimaryPath=store.State.ClassId+"."+path;store.State.Nodes.Clear();for(int t=1;t<=4;t++)store.State.Nodes.Add(store.State.PrimaryPath+".t"+t);store.State.Validate();}
    static void ModifiersAndCriticals()
    {
        var w=Store("rune_weaver");SetPath(w,"control");var ev=new KitAccess();var svc=Service(w,new RuntimeAccess(),ev);int mp=w.State.Vitals.Mp;string a=T();svc.Cast(a,2,0);
        C(mp-w.State.Vitals.Mp==9,"Control geada cost");var snap=w.State.Kit.Actions.Single();C(snap.SlowBp==3000 && snap.DurationMs==3000,"Control slow parameters");
        // Actual respec preserves issued action parameters and cooldown.
        var progress=new ProgressionService(w,new TrainingTestAccess());S(progress.Respec(T(),"rune_weaver.burst",new[]{"rune_weaver.burst.t1"},w.State.Version),ProgressionStatus.Accepted);
        C(w.State.Kit.Actions.Single().SlowBp==3000 && w.State.Kit.Ready[2]==5000 && w.State.Vitals.Mp==mp-9,"Respec rewrote old snapshot/resources");
        ev.Crit=0;var hit=svc.ConfirmHit(T(),a,"critical",w.State.Version);C(hit.Emissions.Single().Critical && hit.Emissions.Single().Damage==snap.Raw*15000/10000,"Critical stage or snapshot");
        Time(w,1000);ev.At=1000;string fire=T();svc.Cast(fire,3,w.State.Version);C(w.State.Kit.Actions.Single(x=>x.Id==fire).BonusBp==1000,"Burst path not applied");
        var sentinel=Store("shadow_sentinel");SetPath(sentinel,"precision");var se=Service(sentinel,new RuntimeAccess(),new KitAccess());string basic=T();se.Cast(basic,1,0);C(sentinel.State.Kit.Actions.Single().BonusBp==1000 && sentinel.State.Kit.Actions.Single().BleedBonusBp==2000,"Precision effects");Time(sentinel,1000);mp=sentinel.State.Vitals.Mp;se.Cast(T(),4,sentinel.State.Version);C(mp-sentinel.State.Vitals.Mp==15 && sentinel.State.Kit.Actions.Last().BonusBp==1000,"Precision rupture");
        var ambush=Store("shadow_sentinel");SetPath(ambush,"ambush");var sa=Service(ambush,new RuntimeAccess(),new KitAccess());sa.Cast(T(),2,0);C(ambush.State.Kit.ConcealedUntil==3000,"Ambush conceal");Time(ambush,500);sa.Cast(T(),1,ambush.State.Version);C(ambush.State.Kit.Actions.Single().BonusBp==3500,"Ambush opening");Time(ambush,1000);mp=ambush.State.Vitals.Mp;sa.Cast(T(),3,ambush.State.Version);C(mp-ambush.State.Vitals.Mp==11 && ambush.State.Kit.Actions.Last().TickCount==4,"Ambush trap");
        var summoner=Store("thorn_summoner");SetPath(summoner,"command");var ss=Service(summoner,new RuntimeAccess(),new KitAccess());mp=summoner.State.Vitals.Mp;ss.Cast(T(),2,0);var golem=summoner.State.Kit.Summons["golem"];C(mp-summoner.State.Vitals.Mp==18 && golem.Hp==(40+4*12)*120/100 && golem.ExpiresAt==14000 && golem.Raw>(2+2*12+58),"Command summon snapshot");
        var sacrifice=Store("thorn_summoner");SetPath(sacrifice,"sacrifice");var sx=Service(sacrifice,new RuntimeAccess(),new KitAccess());sx.Cast(T(),2,0);Time(sacrifice,1000);sx.Cast(T(),4,sacrifice.State.Version);Time(sacrifice,2000);sacrifice.State.Vitals.Hp-=100;int hp=sacrifice.State.Vitals.Hp;mp=sacrifice.State.Vitals.Mp;sx.Cast(T(),3,sacrifice.State.Version);C(sacrifice.State.Kit.Summons.Count==0 && sacrifice.State.Vitals.Hp==hp+CharacterRules.MaxHp(sacrifice.State)/10 && mp-sacrifice.State.Vitals.Mp==15,"Deliberate sacrifice/cost/healing");C(sacrifice.State.Kit.Actions.Last().TickCount==4 && sacrifice.State.Kit.Actions.Last().TickRaw==(2+24+58)*125/100,"Sacrifice poison snapshot");
        var protection=Store("sacred_flame_inquisitor");SetPath(protection,"protection");var sp=Service(protection,new RuntimeAccess(),new KitAccess());protection.State.Vitals.Hp-=100;mp=protection.State.Vitals.Mp;sp.Cast(T(),3,0);C(mp-protection.State.Vitals.Mp==16 && protection.State.Kit.Barrier==50*40/100 && protection.State.Kit.BarrierUntil==3000,"Protection effective heal/barrier");
        var judgment=Store("sacred_flame_inquisitor");SetPath(judgment,"judgment");var sj=Service(judgment,new RuntimeAccess(),new KitAccess());mp=judgment.State.Vitals.Mp;sj.Cast(T(),4,0);C(mp-judgment.State.Vitals.Mp==20 && judgment.State.Kit.Actions.Single().BonusBp==1000 && judgment.State.Kit.Actions.Single().UndeadBonusBp==2500,"Judgment modifiers");
    }
    static void CapacityAndDistribution()
    {
        var store=new CampaignTestStore();store.State.Inventory=new InventoryState();store.State.Inventory.Grant("character","campaign","wolf_fang",1000,false);store.State.Inventory.Grant("character","campaign","wolf_fang",200,false);
        for(int n=0;n<100;n++)store.State.Inventory.Grant("character","campaign","training_axe",1,true);
        var access=new RewardAccess();var svc=new EncounterRewardService(store,new TrainingTestAccess(),access);C(!EncounterCatalog.CanEnter(store.State,"forest_01"),"Encounter allowed with full reserve");S(svc.Claim(T(),"full.reserve",0),ProgressionStatus.Locked);C(store.State.TotalXp==0 && store.State.Defeats.Count==0 && access.Rolls==0,"Full reserve rolled or partially rewarded");
        var probabilities=new int[5];const int count=5000;
        for(int n=0;n<count;n++) {var b=LootRules.Roll(EncounterCategory.Common,1,"rune_weaver","sample",new SeededLootRandom(n));if(b.Equipment!=null){probabilities[0]++;if(b.Equipment.Rarity=="magic")probabilities[4]++;}if(b.Gold>0)probabilities[1]++;if(b.Consumable!=null)probabilities[2]++;if(b.Material)probabilities[3]++;}
        // Smoke test, broad 3 percentage-point absolute tolerances. Not economic balancing.
        double[] expected={.12,.60,.15,.05,.03};for(int i=0;i<5;i++)C(Math.Abs((double)probabilities[i]/count-expected[i])<.03,"Drop distribution drift "+i);
        var e=LootRules.Roll(EncounterCategory.Boss,30,"rune_weaver","reloaded",new ConstantRandom(9999));var state=Store("rune_weaver").State;C(state.Inventory.GrantEquipment("character","campaign",e.TemplateId,e.Equipment),"Advanced grant");
        using(var stream=new MemoryStream()) {new DataContractJsonSerializer(typeof(CampaignState)).WriteObject(stream,state);stream.Position=0;var loaded=(CampaignState)new DataContractJsonSerializer(typeof(CampaignState)).ReadObject(stream);loaded.Validate();C(loaded.Inventory.Items.Last().Roll.Affixes.Count==4 && loaded.Inventory.Items.Last().Roll.Affixes.All(x=>x.Tier==3),"Advanced loot serialization");}
    }

    internal static void Run() { ModifiersAndCriticals();CapacityAndDistribution();Kits();Weaver();Sentinel();Summoner();Inquisitor();Loot();RewardsAndRoute();Persistence();Specializations();Console.WriteLine(checks+" extended runtime checks passed: four kits, loot, encounters, migrations"); }
}
