using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Aethel.Campaign
{
    [DataContract]
    public sealed class KitAction
    {
        [DataMember(IsRequired=true)] public string Id, Effect, Element;
        [DataMember(IsRequired=true)] public int Slot, Raw, TickRaw, BonusBp, SlowBp, DurationMs, TickCount, CritChanceBp, CritMultiplierBp, BleedBonusBp, UndeadBonusBp;
        [DataMember(IsRequired=true)] public bool Fracture;
        [DataMember(IsRequired=true)] public long StartsAt, EndsAt;
        [DataMember(IsRequired=true)] public bool Area;
        [DataMember(IsRequired=true)] public List<string> Targets = new List<string>();
        internal KitAction Copy() { var x=(KitAction)MemberwiseClone();x.Targets=new List<string>(Targets);return x; }
    }
    [DataContract]
    public sealed class KitTargetEffect
    {
        [DataMember(IsRequired=true)] public string Target, Kind, Element;
        [DataMember(IsRequired=true)] public int Raw, RemainingTicks, SlowBp;
        [DataMember(IsRequired=true)] public long NextTickAt, ExpiresAt;
        internal KitTargetEffect Copy() { return (KitTargetEffect)MemberwiseClone(); }
    }
    [DataContract]
    public sealed class SummonState
    {
        [DataMember(IsRequired=true)] public string ActionId;
        [DataMember(IsRequired=true)] public int Hp, Defense, Ward, Raw, IntervalMs;
        [DataMember(IsRequired=true)] public long ExpiresAt, NextAttackAt, LastDamageSequence;
        internal SummonState Copy() { return (SummonState)MemberwiseClone(); }
    }
    [DataContract]
    public sealed class KitState
    {
        [DataMember(IsRequired=true)] public Dictionary<int,long> Ready = new Dictionary<int,long>();
        [DataMember(IsRequired=true)] public List<KitAction> Actions = new List<KitAction>();
        [DataMember(IsRequired=true)] public List<KitTargetEffect> Targets = new List<KitTargetEffect>();
        [DataMember(IsRequired=true)] public Dictionary<string,SummonState> Summons = new Dictionary<string,SummonState>();
        [DataMember(IsRequired=true)] public List<CombatEmission> Pending = new List<CombatEmission>();
        [DataMember(IsRequired=true)] public long BusyUntil, BarrierUntil, ConcealedUntil, ShadowUntil, ShadowReadyAt, FractureReadyAt;
        [DataMember(IsRequired=true)] public int Barrier;
        internal KitState Copy() { return new KitState { Pending=Pending.Select(x=>x.Copy()).ToList(),Ready=new Dictionary<int,long>(Ready),Actions=Actions.Select(x=>x.Copy()).ToList(),Targets=Targets.Select(x=>x.Copy()).ToList(),Summons=Summons.ToDictionary(x=>x.Key,x=>x.Value.Copy()),BusyUntil=BusyUntil,BarrierUntil=BarrierUntil,ConcealedUntil=ConcealedUntil,ShadowUntil=ShadowUntil,ShadowReadyAt=ShadowReadyAt,FractureReadyAt=FractureReadyAt,Barrier=Barrier }; }
        internal void Clear() { Actions.Clear();Targets.Clear();Summons.Clear();Barrier=0;BarrierUntil=0;ConcealedUntil=0;ShadowUntil=0; }
        internal void Compact(long now)
        {
            Actions.RemoveAll(x=>now>=x.EndsAt);Targets.RemoveAll(x=>now>=x.ExpiresAt && x.RemainingTicks==0);
            foreach(var id in Summons.Where(x=>now>=x.Value.ExpiresAt || x.Value.Hp==0).Select(x=>x.Key).ToArray()) Summons.Remove(id);
            if(now>=BarrierUntil) Barrier=0;
        }
        internal void Validate(CampaignState s)
        {
            long now=s.SimulationTime;
            if(Pending==null || Pending.Count>512 || Pending.Any(x=>x==null || !CampaignState.ValidId(x.Id) || !CampaignState.ValidId(x.TargetId) || !CampaignState.ValidId(x.SourceId) || !ValidElement(x.Element) || x.Damage<0 || x.Damage>1000000) || Pending.Select(x=>x.Id).Distinct().Count()!=Pending.Count || Ready==null || Actions==null || Targets==null || Summons==null || Ready.Count>4 || Actions.Count>CombatKitCatalog.MaxActions || Targets.Count>CombatKitCatalog.MaxTargets || Summons.Count>2 ||
                Ready.Any(x=>x.Key<1 || x.Key>4 || x.Value<0 || x.Value>now+20000) || Barrier<0 || Barrier>10000 ||
                new[]{BusyUntil,BarrierUntil,ConcealedUntil,ShadowUntil,ShadowReadyAt,FractureReadyAt}.Any(x=>x<0 || x>now+20000) ||
                Actions.Any(x=>x==null || !CampaignState.ValidId(x.Id) || CombatKitCatalog.Find(s.ClassId,x.Slot)==null || x.StartsAt<0 || x.StartsAt>now || x.EndsAt<x.StartsAt || x.EndsAt>now+20000 || x.Raw<0 || x.Raw>100000 || x.TickRaw<0 || x.TickRaw>100000 || x.CritChanceBp<0 || x.CritChanceBp>2500 || x.CritMultiplierBp<15000 || x.CritMultiplierBp>20000 || x.BonusBp<0 || x.BonusBp>10000 || x.BleedBonusBp<0 || x.BleedBonusBp>10000 || x.UndeadBonusBp<0 || x.UndeadBonusBp>10000 || x.SlowBp<0 || x.SlowBp>5000 || x.DurationMs<0 || x.DurationMs>15000 || x.TickCount<0 || x.TickCount>6 || x.Targets==null || x.Targets.Count>128 || x.Targets.Distinct().Count()!=x.Targets.Count || x.Targets.Any(t=>!CampaignState.ValidId(t)) || !ValidKind(x.Effect) || !ValidElement(x.Element)) ||
                Actions.Select(x=>x.Id).Distinct().Count()!=Actions.Count ||
                Targets.Any(x=>x==null || !CampaignState.ValidId(x.Target) || !new[]{"slow","burn","bleed","poison"}.Contains(x.Kind) || !ValidElement(x.Element) || x.Raw<0 || x.Raw>100000 || x.RemainingTicks<0 || x.RemainingTicks>6 || x.SlowBp<0 || x.SlowBp>5000 || x.NextTickAt<0 || x.NextTickAt>now+20000 || x.ExpiresAt<0 || x.ExpiresAt>now+20000) || Targets.Select(x=>x.Target+"|"+x.Kind).Distinct().Count()!=Targets.Count ||
                Summons.Any(x=>!new[]{"golem","beast"}.Contains(x.Key) || x.Value==null || !CampaignState.ValidId(x.Value.ActionId) || x.Value.LastDamageSequence<0 || x.Value.LastDamageSequence==long.MaxValue || x.Value.Hp<0 || x.Value.Hp>10000 || x.Value.Defense<0 || x.Value.Defense>1000 || x.Value.Ward<0 || x.Value.Ward>1000 || x.Value.Raw<1 || x.Value.Raw>100000 || x.Value.IntervalMs<500 || x.Value.IntervalMs>2000 || x.Value.ExpiresAt<0 || x.Value.ExpiresAt>now+20000 || x.Value.NextAttackAt<0 || x.Value.NextAttackAt>now+20000)) throw new SerializationException("Invalid class combat state");
        }
        private static bool ValidKind(string k) { return new[]{"","slow","burn","bleed","poison","heal","barrier","conceal","golem","beast"}.Contains(k); }
        private static bool ValidElement(string e) { return new[]{"physical","arcane","cold","fire","poison","sacred"}.Contains(e); }
    }
    public sealed class CombatTarget
    {
        public string Id;
        public EncounterCategory Category;
        public int Defense, Ward, ResistanceBp, ReductionBp;
        public bool Undead, Immune;
        internal bool Valid() { return CampaignState.ValidId(Id) && Enum.IsDefined(typeof(EncounterCategory),Category) && Defense>=0 && Defense<=100000 && Ward>=0 && Ward<=100000 && ResistanceBp>=-5000 && ResistanceBp<=7500 && ReductionBp>=0 && ReductionBp<=6000; }
    }
    // Trusted simulation adapter supplies collision/target evidence, never UI.
    public interface IClassCombatAccess
    {
        bool TryConfirmHit(string character,string action,string eventId,out CombatTarget target,out long observedAt);
        bool TryGetLiveTarget(string character,string targetId,out CombatTarget target);
        int CriticalRoll(string character,string action,string target);
        bool HasAppliedEmission(string character,string emissionId);
        bool TryConfirmSummonStrike(string character,string kind,string targetId,out CombatTarget target);
        bool TryConfirmSummonDamage(string character,string eventId,out string kind,out string actionId,out long sequence,out int damage);
    }
    [DataContract]
    public sealed class CombatEmission
    {
        [DataMember(IsRequired=true)] public string Id, TargetId, SourceId, Element;
        [DataMember(IsRequired=true)] public int Damage;
        [DataMember(IsRequired=true)] public bool Periodic;
        [DataMember(IsRequired=true)] public bool Critical;
        internal CombatEmission Copy() {return (CombatEmission)MemberwiseClone();}
    }
    public sealed class ClassCombatResult
    {
        public ProgressionResult Transaction;
        // Valid only on Accepted; replay must reconcile the confirmed simulation, never reapply emissions.
        public List<CombatEmission> Emissions = new List<CombatEmission>();
    }
    public sealed class ClassCombatService
    {
        private readonly ProgressionService commands;
        private readonly IGameplayAccess access;
        private readonly IClassCombatAccess evidence;
        public ClassCombatService(ICampaignStore store,ITrainingAccess training,IGameplayAccess access,IClassCombatAccess evidence)
        { commands=new ProgressionService(store,training);this.access=access??throw new ArgumentNullException("access");this.evidence=evidence??throw new ArgumentNullException("evidence"); }
        private static int B(CampaignState s,string path,int tier) {return KitSpecializationBalance.Applied(s,path,tier);}
        private static bool N(CampaignState s,string path,int tier) { return s.Nodes.Contains(s.ClassId+"."+path+".t"+tier); }
        private static bool P(CampaignState s,int p) { return s.BasicNodes.Contains(s.ClassId+".p"+p); }
        public ProgressionResult Cast(string token,int slot,long expected)
        {
            return commands.Run(token,"class-cast|"+slot,expected,s=> {
                var d=CombatKitCatalog.Find(s.ClassId,slot);s.Kit.Compact(s.SimulationTime);
                if(d==null) return ProgressionStatus.Invalid;
                int cost=d.Mp;
                if(s.ClassId=="rune_weaver" && slot==2 )cost-=B(s,"control",3);
                if(s.ClassId=="rune_weaver" && slot==3 )cost-=B(s,"burst",2);
                if(s.ClassId=="shadow_sentinel" && slot==4 )cost-=B(s,"precision",2);
                if(s.ClassId=="shadow_sentinel" && slot==3 )cost-=B(s,"ambush",2);
                if(s.ClassId=="thorn_summoner" && slot==2 )cost-=B(s,"command",2);
                if(s.ClassId=="thorn_summoner" && slot==3 )cost-=B(s,"sacrifice",2);
                if(s.ClassId=="sacred_flame_inquisitor" && slot==3 )cost-=B(s,"protection",2);
                if(s.ClassId=="sacred_flame_inquisitor" && slot==4 )cost-=B(s,"judgment",2);
                if(s.Vitals.Hp==0 || s.SimulationTime<s.Kit.BusyUntil || !access.CanUseAbilities(s.CharacterId) || (slot!=1 && !s.BasicNodes.Contains(s.ClassId+".a"+slot)) || s.Vitals.Mp<cost || (s.Kit.Ready.ContainsKey(slot) && s.SimulationTime<s.Kit.Ready[slot])) return ProgressionStatus.Locked;
                if(d.Effect=="heal" && s.Vitals.Hp==CharacterRules.MaxHp(s)) return ProgressionStatus.Locked;
                if(s.Kit.Actions.Count>=CombatKitCatalog.MaxActions) return ProgressionStatus.HistoryFull;
                int stat=CharacterRules.Attribute(s,d.Attribute), raw=d.Power+s.Inventory.WeaponPower+2*stat+2*(s.Level-1);
                int bonus=s.Inventory.AffixStat("damage_bp")+s.Inventory.UniqueBonus("unique.tempered_edge");
                if(s.ClassId=="rune_weaver")bonus+=(slot==3 ?B(s,"burst",1):0)+(slot==1 ?B(s,"burst",3):0)+(slot==3 ?B(s,"burst",4):0);
                if(s.ClassId=="shadow_sentinel")bonus+=(slot==1 ?B(s,"precision",1):0)+(slot==4 ?B(s,"precision",3):0);
                if(s.ClassId=="sacred_flame_inquisitor")bonus+=(slot==2 ?B(s,"judgment",1):0)+(slot==4 ?B(s,"judgment",3):0);
                if(s.ClassId=="shadow_sentinel")
                {
                    if(d.Effect!="conceal") BreakConceal(s);
                    if(slot==1 && P(s,1) && s.SimulationTime<s.Kit.ShadowUntil && s.SimulationTime>=s.Kit.ShadowReadyAt)
                    {bonus+=2000+B(s,"ambush",4);s.Kit.ShadowUntil=0;s.Kit.ShadowReadyAt=s.SimulationTime+10000;}
                }
                s.Vitals.Mp-=cost;if(cost>0)s.Vitals.LastMpSpentAt=s.SimulationTime;
                s.Kit.Ready[slot]=s.SimulationTime+d.CooldownMs;s.Kit.BusyUntil=s.SimulationTime+d.WindupMs+150;
                if(d.Effect=="heal") { int healing=(int)((long)(d.Power+2*stat)*(10000+Math.Min(5000,s.Inventory.AffixStat("healing_bp")+s.Inventory.UniqueBonus("unique.merciful_ember")+B(s,"protection",1)))/10000);int actual=Math.Min(CharacterRules.MaxHp(s)-s.Vitals.Hp,healing);s.Vitals.Hp+=actual;
                    if(N(s,"protection",3)) {s.Kit.Barrier=actual*(B(s,"protection",3)+B(s,"protection",4))/100;s.Kit.BarrierUntil=s.SimulationTime+3000;}return ProgressionStatus.Accepted; }
                if(d.Effect=="barrier") {s.Kit.Barrier=d.Power+2*stat;s.Kit.BarrierUntil=s.SimulationTime+d.DurationMs;return ProgressionStatus.Accepted;}
                if(d.Effect=="conceal") {s.Kit.ConcealedUntil=s.SimulationTime+d.DurationMs+B(s,"ambush",1);s.Kit.ShadowUntil=0;return ProgressionStatus.Accepted;}
                if(d.Effect=="golem" || d.Effect=="beast")
                {
                    bool golem=d.Effect=="golem";
                    s.Kit.Summons[d.Effect]=new SummonState { ActionId=token.ToLowerInvariant(),Hp=((golem?40:30)+(golem?4:3)*stat)*(100+B(s,"command",1))/100,Defense=(golem?20:10)+(golem && P(s,1)?10:0),Ward=golem?10:20,Raw=(int)((long)raw*(10000+B(s,"command",3))/10000),IntervalMs=golem?1500:1200,ExpiresAt=s.SimulationTime+d.DurationMs+B(s,"command",4),NextAttackAt=s.SimulationTime+(golem?1500:1200) };
                    return ProgressionStatus.Accepted;
                }
                if(s.ClassId=="shadow_sentinel" && slot==3) s.Kit.Actions.RemoveAll(x=>x.Slot==3);
                int ticks=3+(s.ClassId=="sacred_flame_inquisitor" && slot==2 && P(s,1)?1:0)+(s.ClassId=="rune_weaver" && slot==3 ?B(s,"control",4):0)+(s.ClassId=="shadow_sentinel" && slot==3 ?B(s,"ambush",3):0);
                int tickRaw=2+s.Inventory.WeaponPower+2*stat+2*(s.Level-1);
                if(s.ClassId=="thorn_summoner" && slot==3) {bool consumed=false;if(N(s,"sacrifice",1) && s.Kit.Summons.Remove("golem")){tickRaw=tickRaw*(100+B(s,"sacrifice",1))/100;consumed=true;}if(N(s,"sacrifice",3) && s.Kit.Summons.Remove("beast")){ticks+=B(s,"sacrifice",3);consumed=true;}if(consumed && N(s,"sacrifice",4))s.Vitals.Hp=Math.Min(CharacterRules.MaxHp(s),s.Vitals.Hp+CharacterRules.MaxHp(s)*B(s,"sacrifice",4)/100);}
                s.Kit.Actions.Add(new KitAction { Id=token.ToLowerInvariant(),Slot=slot,Effect=d.Effect,Element=d.Element,Raw=d.Effect=="poison"?0:raw,TickRaw=tickRaw,BonusBp=Math.Min(10000,bonus),CritChanceBp=CharacterRules.CriticalChanceBp(s),CritMultiplierBp=CharacterRules.CriticalMultiplierBp(s),BleedBonusBp=s.ClassId=="shadow_sentinel" && P(s,2)?1000+B(s,"precision",4):0,UndeadBonusBp=s.ClassId=="sacred_flame_inquisitor"?(P(s,2)?1500:0)+B(s,"judgment",4):0,Fracture=s.ClassId=="rune_weaver" && P(s,1),SlowBp=2500+(s.ClassId=="rune_weaver" ?B(s,"control",1):0),DurationMs=d.DurationMs+(s.ClassId=="rune_weaver" && d.Effect=="slow" ?B(s,"control",2):0),TickCount=ticks,StartsAt=s.SimulationTime,EndsAt=s.SimulationTime+(d.Effect=="bleed"?8000:d.Effect=="poison"?3000:d.WindupMs+CombatKitCatalog.HitToleranceMs),Area=d.Area });
                s.Vitals.LastHostileAt=s.SimulationTime;return ProgressionStatus.Accepted;
            });
        }
        internal static void BreakConceal(CampaignState s)
        {
            if(s.Kit.ConcealedUntil>0)
            { long ended=Math.Min(s.SimulationTime,s.Kit.ConcealedUntil);s.Kit.ConcealedUntil=0;if(P(s,1))s.Kit.ShadowUntil=ended+2000; }
        }
        public ClassCombatResult ConfirmHit(string token,string actionId,string eventId,long expected)
        {
            var result=new ClassCombatResult();
            result.Transaction=commands.Run(token,"class-hit|"+actionId+"|"+eventId,expected,s=> {
                var a=s.Kit.Actions.FirstOrDefault(x=>x.Id==actionId);CombatTarget target;long at;
                if(!CampaignState.ValidId(eventId) || a==null) return ProgressionStatus.Invalid;
                var d=CombatKitCatalog.Find(s.ClassId,a.Slot);
                if(s.Vitals.Hp==0 || !access.CanUseAbilities(s.CharacterId) || !evidence.TryConfirmHit(s.CharacterId,actionId,eventId,out target,out at) || target==null || !target.Valid() || at<a.StartsAt+d.WindupMs || at>=a.EndsAt || at>s.SimulationTime) return ProgressionStatus.Locked;
                if(a.Targets.Contains(target.Id)) return ProgressionStatus.Replayed;
                if((!a.Area && a.Targets.Count!=0) || a.Targets.Count>=128) return ProgressionStatus.Locked;
                if(s.Kit.Targets.Count>CombatKitCatalog.MaxTargets-2) return ProgressionStatus.HistoryFull;
                int bonus=a.BonusBp;
                if(a.Fracture && at>=s.Kit.FractureReadyAt && s.Kit.Targets.Any(x=>x.Target==target.Id && x.Kind=="slow" && at<x.ExpiresAt)) {bonus+=1500;s.Kit.FractureReadyAt=at+3000;}
                if(a.BleedBonusBp>0 && s.Kit.Targets.Any(x=>x.Target==target.Id && x.Kind=="bleed" && at<x.ExpiresAt))bonus+=a.BleedBonusBp;
                if(a.UndeadBonusBp>0 && target.Undead && a.Element=="sacred")bonus+=a.UndeadBonusBp;
                a.Targets.Add(target.Id);
                if(a.Raw>0) {if(s.Kit.Pending.Count>=512)return ProgressionStatus.HistoryFull;int roll=evidence.CriticalRoll(s.CharacterId,a.Id,target.Id);if(roll<0 || roll>=10000)throw new ArgumentException("Invalid critical RNG");Stage(s,result,Emit(a.Id,target,a.Element,a.Raw,bonus,false,roll<a.CritChanceBp,a.CritMultiplierBp));}
                if(!target.Immune)
                {
                    if(a.Effect=="slow")
                    { int slow=target.Category==EncounterCategory.Boss || target.Category==EncounterCategory.SubBoss?1000:a.SlowBp;
                      Upsert(s,new KitTargetEffect { Target=target.Id,Kind="slow",Element=a.Element,SlowBp=slow,ExpiresAt=at+a.DurationMs }); }
                    if(new[]{"burn","bleed","poison"}.Contains(a.Effect)) Upsert(s,new KitTargetEffect { Target=target.Id,Kind=a.Effect,Element=a.Effect=="bleed"?"physical":a.Element,Raw=a.TickRaw,RemainingTicks=a.TickCount,NextTickAt=at+1000,ExpiresAt=at+a.TickCount*1000+1 });
                }
                return ProgressionStatus.Accepted;
            });
            if(result.Transaction.Status!=ProgressionStatus.Accepted)result.Emissions.Clear();return result;
        }
        private static void Stage(CampaignState s,ClassCombatResult r,CombatEmission e)
        { using(var sha=System.Security.Cryptography.SHA256.Create())e.Id=BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(e.SourceId+"|"+e.TargetId+"|"+e.Element))).Replace("-","");s.Kit.Pending.Add(e.Copy());r.Emissions.Add(e); }
        public ProgressionResult AcknowledgeEmission(string token,string id,long expected)
        { return commands.Run(token,"combat-ack|"+id,expected,s=> {var e=s.Kit.Pending.FirstOrDefault(x=>x.Id==id);if(e==null)return ProgressionStatus.Replayed;if(!evidence.HasAppliedEmission(s.CharacterId,id))return ProgressionStatus.Locked;s.Kit.Pending.Remove(e);return ProgressionStatus.Accepted;}); }
        private static void Upsert(CampaignState s,KitTargetEffect effect)
        { s.Kit.Targets.RemoveAll(x=>x.Target==effect.Target && x.Kind==effect.Kind);s.Kit.Targets.Add(effect); }
        private static CombatEmission Emit(string source,CombatTarget target,string element,int raw,int bonus,bool periodic,bool critical=false,int multiplier=15000)
        { return new CombatEmission { SourceId=source,TargetId=target.Id,Element=element,Periodic=periodic,Critical=critical,Damage=CombatMath.DirectDamage(raw,Math.Min(10000,bonus),critical,multiplier,element=="physical"?target.Defense:target.Ward,target.ResistanceBp,target.ReductionBp,true,target.Immune) }; }
        // Drain ticks through the confirmed simulation clock, including long frames. No wall-clock time.
        public ClassCombatResult ResolveDueEffects(string token,long expected)
        {
            var result=new ClassCombatResult();
            result.Transaction=commands.Run(token,"class-periodics",expected,s=> {
                if(s.Vitals.Hp==0 || !access.CanUseAbilities(s.CharacterId)) return ProgressionStatus.Locked;
                bool changed=false;
                foreach(var e in s.Kit.Targets.OrderBy(x=>x.NextTickAt).ThenBy(x=>x.Target,StringComparer.Ordinal))
                {
                    while(e.RemainingTicks>0 && e.NextTickAt<=s.SimulationTime)
                    {
                        CombatTarget target;if(evidence.TryGetLiveTarget(s.CharacterId,e.Target,out target) && target!=null && target.Valid() && target.Id==e.Target){if(s.Kit.Pending.Count>=512)return ProgressionStatus.HistoryFull;Stage(s,result,Emit("tick."+e.Kind+"."+e.NextTickAt,target,e.Element,e.Raw,0,true));}
                        e.RemainingTicks--;e.NextTickAt+=1000;changed=true;
                    }
                }
                s.Kit.Compact(s.SimulationTime);return changed?ProgressionStatus.Accepted:ProgressionStatus.Replayed;
            });
            if(result.Transaction.Status!=ProgressionStatus.Accepted)result.Emissions.Clear();return result;
        }
        public ProgressionResult DamageSummon(string token,string eventId,long expected)
        { return commands.Run(token,"summon-damage|"+eventId,expected,s=> {string kind,actionId;long seq;int damage;SummonState summon;if(!CampaignState.ValidId(eventId) || !access.CanReceiveHostileDamage(s.CharacterId) || !evidence.TryConfirmSummonDamage(s.CharacterId,eventId,out kind,out actionId,out seq,out damage) || kind==null || !s.Kit.Summons.TryGetValue(kind,out summon) || summon.ActionId!=actionId || s.SimulationTime>=summon.ExpiresAt || seq<1 || seq==long.MaxValue || damage<1)return ProgressionStatus.Locked;if(seq<=summon.LastDamageSequence)return ProgressionStatus.Replayed;if(seq!=summon.LastDamageSequence+1)return ProgressionStatus.Invalid;summon.LastDamageSequence=seq;summon.Hp=Math.Max(0,summon.Hp-damage);return ProgressionStatus.Accepted;}); }
        // One trusted target per summon strike; missed intervals are not banked as a burst.
        public ClassCombatResult SummonStrike(string token,string kind,string targetId,long expected)
        {
            var result=new ClassCombatResult();
            result.Transaction=commands.Run(token,"summon-hit|"+kind+"|"+targetId,expected,s=> {
                SummonState summon;CombatTarget target;
                if(s.Vitals.Hp==0 || !access.CanUseAbilities(s.CharacterId) || !s.Kit.Summons.TryGetValue(kind??"",out summon) || summon.Hp==0 || s.SimulationTime>=summon.ExpiresAt || s.SimulationTime<summon.NextAttackAt || !evidence.TryConfirmSummonStrike(s.CharacterId,kind,targetId,out target) || target==null || !target.Valid() || target.Id!=targetId) return ProgressionStatus.Locked;
                if(s.Kit.Pending.Count>=512)return ProgressionStatus.HistoryFull;summon.NextAttackAt=s.SimulationTime+summon.IntervalMs;Stage(s,result,Emit(summon.ActionId+"."+s.SimulationTime,target,kind=="golem"?"physical":"arcane",summon.Raw,0,false));return ProgressionStatus.Accepted;
            });
            if(result.Transaction.Status!=ProgressionStatus.Accepted)result.Emissions.Clear();return result;
        }
    }
}
