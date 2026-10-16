using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aethel.Campaign
{
    public interface IGameplayAccess
    {
        bool CanUseStash(string characterId);
        bool CanShop(string characterId);
        bool CanRecoverAtHub(string characterId);
        bool IsOutOfCombat(string characterId);
        bool CanUseAbilities(string characterId);
        bool CanReceiveHostileDamage(string characterId);
        bool TryGetConfirmedDamage(string characterId, string eventId, out long sequence, out int damage);
        bool TryGetConfirmedDefeat(string characterId, string spawnId, out string enemyId);
        bool TryGetConfirmedBlock(string characterId, string eventId, out bool frontal, out int absorbedDamage, out long observedAt);
        bool TryGetConfirmedImpactHit(string characterId, string eventId, string actionId, out HitEvidence hit);
    }
    // Evidence comes from the trusted simulation adapter, never directly from menu/input data.
    public sealed class HitEvidence
    {
        public string TargetId;
        public EncounterCategory Category;
        public int HpDamage, BarrierDamage;
        public long SimulationTime;
    }
    public sealed class GameplayService
    {
        private readonly ICampaignStore store;
        private readonly ProgressionService commands;
        private readonly IGameplayAccess access;
        public GameplayService(ICampaignStore store, ITrainingAccess training, IGameplayAccess access)
        { this.store = store; this.access = access ?? throw new ArgumentNullException("access"); commands = new ProgressionService(store, training); }
        public CampaignState Read() { return commands.Read(); }
        public ProgressionResult AwardDefeat(string token, string spawnId, long expected)
        {
            return commands.Run(token, "defeat|" + spawnId, expected, s => {
                if (!CampaignState.ValidId(spawnId)) return ProgressionStatus.Invalid;
                if (s.Defeats.ContainsKey(spawnId)) return ProgressionStatus.Replayed;
                string enemy;
                if (!access.TryGetConfirmedDefeat(s.CharacterId, spawnId, out enemy)) return ProgressionStatus.Locked;
                int xp = ExperienceRules.EnemyXp(enemy);
                if (xp == 0) return ProgressionStatus.Invalid;
                if (s.Defeats.Count >= 8192) return ProgressionStatus.HistoryFull;
                s.TotalXp = Math.Min(ExperienceRules.MinimumForLevel(30), s.TotalXp + xp);
                s.Level = ExperienceRules.LevelForTotal(s.TotalXp); s.Defeats.Add(spawnId, enemy);
                return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult AllocateAttribute(string token, int attribute, int points, long expected)
        {
            return commands.Run(token, "attribute|" + attribute + "|" + points, expected, s => {
                if (attribute < 0 || attribute > 4 || points < 1 || points > 87) return ProgressionStatus.Invalid;
                if (CharacterRules.UnspentAttributes(s) < points || CharacterRules.Attribute(s,attribute) > 99 - points) return ProgressionStatus.Locked;
                s.Attributes[attribute] += points; return ProgressionStatus.Accepted; // Never heals current HP/MP.
            });
        }
        public ProgressionResult LearnBasic(string token, string nodeId, long expected)
        {
            return commands.Run(token, "learn|" + nodeId, expected, s => {
                if (s.BasicNodes.Contains(nodeId)) return ProgressionStatus.Replayed;
                if (!BasicAbilityRules.CanLearn(s,nodeId) || s.BasicNodes.Count >= s.Level - 1) return ProgressionStatus.Locked;
                s.BasicNodes.Add(nodeId); return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult CollectReserve(string token, string instanceId, long expected)
        {
            return commands.Run(token, "collect|" + instanceId, expected, s => s.Inventory.Collect(instanceId) ? ProgressionStatus.Accepted : ProgressionStatus.Locked);
        }
        public ProgressionResult MoveItem(string token, string id, string location, int x, int y, long expected)
        {
            return commands.Run(token, "move|" + id + "|" + location + "|" + x + "|" + y, expected, s => {
                var item = s.Inventory.Items.FirstOrDefault(i => i.Id == id);
                if (item == null) return ProgressionStatus.Invalid;
                if ((location == "stash" || item.Location == "stash") && !access.CanUseStash(s.CharacterId)) return ProgressionStatus.Locked;
                return s.Inventory.Move(id,location,x,y) ? ProgressionStatus.Accepted : ProgressionStatus.Locked;
            });
        }
        public ProgressionResult SplitStack(string token, string id, int amount, string location, int x, int y, long expected)
        {
            return commands.Run(token, "split|" + id + "|" + amount + "|" + location + "|" + x + "|" + y, expected, s => {
                var item = s.Inventory.Items.FirstOrDefault(i => i.Id == id);
                if (item == null) return ProgressionStatus.Invalid;
                if ((location == "stash" || item.Location == "stash") && !access.CanUseStash(s.CharacterId)) return ProgressionStatus.Locked;
                return s.Inventory.Split(s.CharacterId,s.Memory.CampaignId,id,amount,location,x,y) ? ProgressionStatus.Accepted : ProgressionStatus.Locked;
            });
        }
        public ProgressionResult MergeStacks(string token, string source, string target, long expected)
        {
            return commands.Run(token, "merge|" + source + "|" + target, expected, s => {
                var a = s.Inventory.Items.FirstOrDefault(i => i.Id == source); var b = s.Inventory.Items.FirstOrDefault(i => i.Id == target);
                if (a == null || b == null) return ProgressionStatus.Invalid;
                if ((a.Location == "stash" || b.Location == "stash") && !access.CanUseStash(s.CharacterId)) return ProgressionStatus.Locked;
                return s.Inventory.Merge(source,target) ? ProgressionStatus.Accepted : ProgressionStatus.Locked;
            });
        }
        public ProgressionResult Equip(string token, string slot, string instance, long expected)
        {
            return commands.Run(token, "equip|" + slot + "|" + instance, expected, s => s.Inventory.Equip(s.ClassId,slot,instance) ? ProgressionStatus.Accepted : ProgressionStatus.Invalid);
        }
        public ProgressionResult BuyPotion(string token, string template, long expected)
        {
            return commands.Run(token, "buy|" + template, expected, s => {
                int cost = template == "potion_health_small" ? GameplayBalance.HealthPotionPrice : template == "flask_aether_small" ? GameplayBalance.AetherFlaskPrice : 0;
                if (cost == 0) return ProgressionStatus.Invalid;
                if (!access.CanShop(s.CharacterId)) return ProgressionStatus.Locked;
                if (s.Gold < cost) return ProgressionStatus.InsufficientGold;
                if (!s.Inventory.Grant(s.CharacterId,s.Memory.CampaignId,template,1,false)) return ProgressionStatus.Locked;
                s.Gold -= cost; return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult UseConsumable(string token, string instanceId, long expected)
        {
            return commands.Run(token, "consume|" + instanceId, expected, s => {
                var item = s.Inventory.Items.FirstOrDefault(i => i.Id == instanceId && i.Location == "bag");
                if (item == null || s.Vitals.Hp <= 0) return ProgressionStatus.Invalid;
                var def = ItemCatalog.Find(item.TemplateId);
                if (def.Id == "potion_health_small" || def.Id == "flask_aether_small")
                {
                    if (s.SimulationTime < s.Vitals.SharedPotionReadyAt) return ProgressionStatus.Locked;
                    if (def.Id == "potion_health_small")
                    { if (s.Vitals.Hp >= CharacterRules.MaxHp(s)) return ProgressionStatus.Locked; s.Vitals.Hp = Math.Min(CharacterRules.MaxHp(s),s.Vitals.Hp + def.Restore); }
                    else { if (s.Vitals.Mp >= CharacterRules.MaxMp(s)) return ProgressionStatus.Locked; s.Vitals.Mp = Math.Min(CharacterRules.MaxMp(s),s.Vitals.Mp + def.Restore); }
                    s.Vitals.SharedPotionReadyAt = s.SimulationTime + GameplayBalance.PotionCooldownMs;
                }
                else if (def.Id == "antidote_flask")
                {
                    if (!s.Vitals.RemovablePoison || s.SimulationTime < s.Vitals.AntidoteReadyAt) return ProgressionStatus.Locked;
                    s.Vitals.RemovablePoison = false; s.Vitals.AntidoteReadyAt = s.SimulationTime + GameplayBalance.PotionCooldownMs;
                }
                else return ProgressionStatus.Invalid;
                s.Inventory.RemoveOne(instanceId); return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult AdvanceSimulationTo(long time, long expected)
        {
            var s = Read();
            if (time < 0 || time > long.MaxValue - 60000 || time - s.SimulationTime > 60000)
                return new ProgressionResult { Status = ProgressionStatus.Invalid, State = s };
            if (time <= s.SimulationTime) return new ProgressionResult { Status = ProgressionStatus.Replayed, State = s };
            if (s.Version != expected) return new ProgressionResult { Status = ProgressionStatus.Conflict, State = s };
            if (s.Vitals.Hp > 0 && access.IsOutOfCombat(s.CharacterId) && !s.Vitals.RemovablePoison)
            {
                long start = Math.Max(s.SimulationTime,Math.Max(s.Vitals.LastHostileAt + 5000,s.Vitals.LastMpSpentAt + 3000));
                long duration = Math.Max(0,time-start);
                long numerator = s.Vitals.MpRegenRemainder + duration * (1 + CharacterRules.Attribute(s,2)/10);
                s.Vitals.Mp = Math.Min(CharacterRules.MaxMp(s),s.Vitals.Mp + (int)(numerator/1000));
                s.Vitals.MpRegenRemainder = s.Vitals.Mp == CharacterRules.MaxMp(s) ? 0 : (int)(numerator%1000);
            }
            else s.Vitals.LastHostileAt = time;
            s.SimulationTime = time; s.Memory.Version++; s.Validate();
            var status = ProgressionStatus.Accepted;
            try { if (!store.TryCommit(expected,s)) status = ProgressionStatus.Conflict; }
            catch (System.IO.IOException) { status = ProgressionStatus.PersistenceFailed; }
            catch (UnauthorizedAccessException) { status = ProgressionStatus.PersistenceFailed; }
            return new ProgressionResult { Status = status, State = Read() };
        }
        public ProgressionResult RecoverAtHub(string token, long expected)
        {
            return commands.Run(token,"recover-at-hub",expected,s => {
                if (!access.CanRecoverAtHub(s.CharacterId)) return ProgressionStatus.Locked;
                if (s.Vitals.Hp == CharacterRules.MaxHp(s) && s.Vitals.Mp == CharacterRules.MaxMp(s) &&
                    !s.Vitals.RemovablePoison && s.Effects.CounterExpiresAt == 0 && s.Effects.GuardExpiresAt == 0 && s.Effects.ImpactBonus.Count == 0)
                    return ProgressionStatus.Replayed;
                s.Vitals.Hp = CharacterRules.MaxHp(s); s.Vitals.Mp = CharacterRules.MaxMp(s); s.Vitals.RemovablePoison = false;
                s.Vitals.MpRegenRemainder = 0; s.Effects.ClearPersonalActions(); s.Effects.Ruptures.Clear();
                return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult ApplyConfirmedDamage(string token, string eventId, long expected)
        {
            return commands.Run(token,"damage|" + eventId,expected,s => {
                long sequence; int damage;
                if (!CampaignState.ValidId(eventId)) return ProgressionStatus.Invalid;
                if (!access.CanReceiveHostileDamage(s.CharacterId) || !access.TryGetConfirmedDamage(s.CharacterId,eventId,out sequence,out damage)) return ProgressionStatus.Locked;
                if (sequence < 1 || sequence == long.MaxValue) return ProgressionStatus.Invalid;
                if (sequence <= s.Vitals.LastDamageSequence) return ProgressionStatus.Replayed;
                if (sequence != s.Vitals.LastDamageSequence + 1 || damage <= 0 || s.Vitals.Hp <= 0) return ProgressionStatus.Invalid;
                s.Vitals.LastDamageSequence = sequence; s.Vitals.LastHostileAt = s.SimulationTime; s.Vitals.Hp = Math.Max(0,s.Vitals.Hp - damage);
                if (s.Vitals.Hp == 0)
                {
                    s.Effects.ClearPersonalActions();
                }
                return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult UseGuard(string token, long expected)
        {
            return commands.Run(token, "guard", expected, s => {
                int cost = s.Nodes.Contains("twilight_bastion.defense.t3") ? GameplayBalance.GuardImprovedMp : GameplayBalance.GuardMp;
                if (s.ClassId != "twilight_bastion" || !s.BasicNodes.Contains(s.ClassId + ".a2") || s.Vitals.Hp <= 0 ||
                    !access.CanUseAbilities(s.CharacterId) || s.SimulationTime < s.Effects.GuardReadyAt || s.Vitals.Mp < cost) return ProgressionStatus.Locked;
                s.Vitals.Mp -= cost; s.Vitals.LastMpSpentAt = s.SimulationTime; s.Effects.GuardExpiresAt = s.SimulationTime + GameplayBalance.GuardDurationMs; s.Effects.GuardReadyAt = s.SimulationTime + GameplayBalance.GuardCooldownMs;
                s.Effects.PostureUsed = false; return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult ConfirmBlock(string token, string eventId, long expected)
        {
            return commands.Run(token, "block|" + eventId, expected, s => {
                bool front; int absorbed; long observedAt;
                s.Effects.Compact(s.SimulationTime);
                if (!CampaignState.ValidId(eventId)) return ProgressionStatus.Invalid;
                if (s.Effects.AppliedHits.ContainsKey(HashId("block|" + eventId))) return ProgressionStatus.Replayed;
                if (s.ClassId != "twilight_bastion" || s.Vitals.Hp <= 0 || s.SimulationTime >= s.Effects.GuardExpiresAt ||
                    !access.TryGetConfirmedBlock(s.CharacterId,eventId,out front,out absorbed,out observedAt) || !front || absorbed <= 0 || observedAt < s.Effects.GuardExpiresAt - 2000 || observedAt > s.SimulationTime) return ProgressionStatus.Locked;
                if (s.Effects.AppliedHits.Count >= 2048) return ProgressionStatus.HistoryFull;
                s.Vitals.LastHostileAt = Math.Max(s.Vitals.LastHostileAt,observedAt);
                s.Effects.AppliedHits.Add(HashId("block|" + eventId),eventId); s.Effects.EventTimes.Add(HashId("block|" + eventId),observedAt);
                if (s.BasicNodes.Contains(s.ClassId + ".p1") && !s.Effects.PostureUsed)
                { s.Effects.PostureExpiresAt = observedAt + 2000; s.Effects.PostureUsed = true; }
                if (s.Nodes.Contains("twilight_bastion.defense.t1") && s.SimulationTime >= s.Effects.CounterReadyAt)
                { s.Effects.CounterExpiresAt = observedAt + GameplayBalance.CounterDurationMs; s.Effects.CounterReadyAt = observedAt + GameplayBalance.CounterCooldownMs; }
                return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult UseImpact(string token, long expected)
        {
            Guid action;
            if (!Guid.TryParseExact(token,"D",out action)) return new ProgressionResult { Status = ProgressionStatus.Invalid, State = Read() };
            string actionId = action.ToString("D");
            return commands.Run(token, "impact", expected, s => {
                s.Effects.Compact(s.SimulationTime);
                int cost = s.Nodes.Contains("twilight_bastion.rupture.t2") ? GameplayBalance.ImpactImprovedMp : GameplayBalance.ImpactMp;
                if (s.ClassId != "twilight_bastion" || !s.BasicNodes.Contains(s.ClassId + ".a3") || s.Vitals.Hp <= 0 ||
                    !access.CanUseAbilities(s.CharacterId) || s.SimulationTime < s.Effects.ImpactReadyAt || s.Vitals.Mp < cost) return ProgressionStatus.Locked;
                if (s.Effects.ImpactBonus.Count >= 512) return ProgressionStatus.HistoryFull;
                int bonus = s.Nodes.Contains("twilight_bastion.defense.t1") && s.SimulationTime < s.Effects.CounterExpiresAt ? (s.Nodes.Contains("twilight_bastion.defense.t4") ? GameplayBalance.CounterImprovedBonusBp : GameplayBalance.CounterBonusBp) : 0;
                s.Effects.ImpactStartedAt.Add(actionId,s.SimulationTime); s.Effects.ImpactBonus.Add(actionId,bonus);
                s.Effects.ImpactRawDamage.Add(actionId,GameplayBalance.ImpactPower + s.Inventory.WeaponPower + 2 * CharacterRules.Attribute(s,0) + 2 * (s.Level - 1));
                s.Effects.ImpactRuptureBp.Add(actionId,s.Nodes.Contains("twilight_bastion.rupture.t1") ? (s.Nodes.Contains("twilight_bastion.rupture.t4") ? GameplayBalance.RuptureImprovedBp : GameplayBalance.RuptureBp) : 0);
                s.Effects.ImpactRuptureDuration.Add(actionId,s.Nodes.Contains("twilight_bastion.rupture.t3") ? GameplayBalance.RuptureImprovedDurationMs : GameplayBalance.RuptureDurationMs); s.Effects.CounterExpiresAt = 0;
                s.Vitals.Mp -= cost; s.Vitals.LastMpSpentAt = s.SimulationTime; s.Vitals.LastHostileAt = s.SimulationTime; s.Effects.ImpactReadyAt = s.SimulationTime + GameplayBalance.ImpactCooldownMs; return ProgressionStatus.Accepted;
            });
        }
        public ProgressionResult ConfirmImpactHit(string token, string actionId, string eventId, long expected)
        {
            return commands.Run(token, "impact-hit|" + actionId + "|" + eventId, expected, s => {
                HitEvidence hit; s.Effects.Compact(s.SimulationTime);
                if (s.Vitals.Hp <= 0) return ProgressionStatus.Locked;
                if (!s.Effects.ImpactBonus.ContainsKey(actionId) || !CampaignState.ValidId(eventId)) return ProgressionStatus.Invalid;
                if (!access.TryGetConfirmedImpactHit(s.CharacterId,eventId,actionId,out hit) || hit == null ||
                    !CampaignState.ValidId(hit.TargetId) || !Enum.IsDefined(typeof(EncounterCategory),hit.Category) || hit.HpDamage < 0 || hit.BarrierDamage < 0 ||
                    (hit.HpDamage == 0 && hit.BarrierDamage == 0) || hit.SimulationTime < s.Effects.ImpactStartedAt[actionId] ||
                    hit.SimulationTime >= s.Effects.ImpactStartedAt[actionId] + GameplayBalance.ImpactActiveMs || hit.SimulationTime > s.SimulationTime) return ProgressionStatus.Locked;
                string key;
                using (var sha = SHA256.Create()) key = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(actionId + "|" + hit.TargetId))).Replace("-","");
                if (s.Effects.AppliedHits.ContainsKey(key)) return ProgressionStatus.Replayed;
                if (s.Effects.AppliedHits.Count >= 2048 || (!s.Effects.Ruptures.ContainsKey(hit.TargetId) && s.Effects.Ruptures.Count >= 256)) return ProgressionStatus.HistoryFull;
                s.Effects.AppliedHits.Add(key,hit.TargetId); s.Effects.EventTimes.Add(key,hit.SimulationTime);
                if (s.Effects.ImpactRuptureBp[actionId] > 0 && s.SimulationTime < hit.SimulationTime + s.Effects.ImpactRuptureDuration[actionId])
                {
                    int bp = s.Effects.ImpactRuptureBp[actionId];
                    if (hit.Category == EncounterCategory.Boss || hit.Category == EncounterCategory.SubBoss) bp /= 2;
                    RuptureState previous;
                    if (s.Effects.Ruptures.TryGetValue(hit.TargetId,out previous) && s.SimulationTime < previous.ExpiresAt) bp = Math.Max(bp,previous.BasisPoints);
                    s.Effects.Ruptures[hit.TargetId] = new RuptureState { BasisPoints = bp, ExpiresAt = hit.SimulationTime + s.Effects.ImpactRuptureDuration[actionId] };
                }
                return ProgressionStatus.Accepted;
            });
        }
        private static string HashId(string text)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-","");
        }
        public static int GuardReductionBp(CampaignState s)
        { return s.SimulationTime < s.Effects.GuardExpiresAt ? Math.Min(6000,GameplayBalance.GuardReductionBp + (s.Nodes.Contains("twilight_bastion.defense.t2") ? GameplayBalance.GuardImprovedReductionBp : 0) + (s.SimulationTime < s.Effects.PostureExpiresAt ? GameplayBalance.PostureReductionBp : 0)) : 0; }
    }
}
