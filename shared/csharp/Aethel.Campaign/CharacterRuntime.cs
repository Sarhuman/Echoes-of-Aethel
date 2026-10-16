using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Aethel.Campaign
{
    public static class ExperienceRules
    {
        public static int ToNextLevel(int level) { if (level < 1 || level > 30) throw new ArgumentOutOfRangeException(); return level == 30 ? 0 : GameplayBalance.XpBase + GameplayBalance.XpStep * (level - 1); }
        public static int MinimumForLevel(int level)
        { if (level < 1 || level > 30) throw new ArgumentOutOfRangeException(); return GameplayBalance.XpBase * (level - 1) + GameplayBalance.XpStep * (level - 1) * (level - 2) / 2; }
        public static int LevelForTotal(long xp)
        {
            if (xp < 0 || xp > MinimumForLevel(30)) throw new ArgumentOutOfRangeException();
            int level = 1; while (level < 30 && xp >= MinimumForLevel(level + 1)) level++; return level;
        }
        public static int EnemyXp(string id)
        { return id == "ethereal_wolf" ? RuntimeRewards.WolfXp : id == "blind_cultist" ? RuntimeRewards.CultistXp : id == "moss_claws" ? RuntimeRewards.MossClawsXp : EncounterCatalog.Find(id)==null?0:EncounterCatalog.Find(id).Xp; }
    }
    public static class CharacterRules
    {
        private static readonly int[][] Bases = { new[] {12,6,3,5,14}, new[] {4,6,14,8,8}, new[] {8,14,4,4,10}, new[] {4,6,12,10,8}, new[] {10,4,4,14,8} };
        public static int Attribute(CampaignState s, int index)
        { return Math.Min(99,Bases[Array.IndexOf(SpecializationCatalog.Classes, s.ClassId)][index] + s.Attributes[index] + (s.Inventory==null?0:s.Inventory.AffixStat(new[]{"str","dex","int","fth","vit"}[index]))); }
        public static int MaxHp(CampaignState s) { return 60 + 8 * Attribute(s, 4) + 6 * (s.Level - 1); }
        public static int MaxMp(CampaignState s) { return 30 + 4 * Attribute(s, 2) + 2 * Attribute(s, 3) + 3 * (s.Level - 1); }
        public static int PhysicalDefense(CampaignState s) { return s.Inventory.Armor + Attribute(s, 4) / 2 + 2 * (s.Level - 1); }
        public static int MagicalDefense(CampaignState s) { return s.Inventory.AffixStat("ward")+s.Inventory.UniqueWard+(Attribute(s,2)+Attribute(s,3))/3+s.Level-1; }
        public static int CriticalChanceBp(CampaignState s) {return Math.Min(2500,500+25*Attribute(s,1)+s.Inventory.AffixStat("crit_bp"));}
        public static int CriticalMultiplierBp(CampaignState s) {return Math.Min(20000,15000+s.Inventory.AffixStat("crit_multiplier_bp"));}
        public static int ResistanceBp(CampaignState s) { return Math.Min(7500,s.Inventory.AffixStat("resist_bp")); }
        public static int UnspentAttributes(CampaignState s) { return 3 * (s.Level - 1) - s.Attributes.Sum(); }
    }
    public static class BasicAbilityRules
    {
        private static readonly string[] Suffixes = { "a2", "p1", "a3", "p2", "a4" };
        private static readonly int[] Levels = { 2, 3, 4, 5, 6 };
        public static bool CanLearn(CampaignState s, string id)
        {
            int i = Array.IndexOf(Suffixes, id == null ? "" : id.Replace(s.ClassId + ".", ""));
            if (i < 0 || id != s.ClassId + "." + Suffixes[i] || s.Level < Levels[i]) return false;
            if (Suffixes[i] == "p1" && !s.BasicNodes.Contains(s.ClassId + ".a2")) return false;
            if ((Suffixes[i] == "p2" || Suffixes[i] == "a4") && !s.BasicNodes.Contains(s.ClassId + ".a3")) return false;
            return true;
        }
        internal static bool Validate(CampaignState s) { return s.BasicNodes.All(id => CanLearn(s,id)); }
    }
    [DataContract]
    public sealed class VitalsState
    {
        [DataMember(IsRequired = true)] public int Hp, Mp;
        [DataMember(IsRequired = true)] public bool RemovablePoison;
        [DataMember(IsRequired = true)] public long LastDamageSequence;
        [DataMember(IsRequired = true)] public long LastHostileAt = -5000, LastMpSpentAt = -3000;
        [DataMember(IsRequired = true)] public int MpRegenRemainder;
        [DataMember(IsRequired = true)] public long SharedPotionReadyAt, AntidoteReadyAt;
        internal VitalsState Copy() { return (VitalsState)MemberwiseClone(); }
        internal void Validate(CampaignState s)
        {
            if (LastHostileAt < -5000 || LastHostileAt > s.SimulationTime || LastMpSpentAt < -3000 || LastMpSpentAt > s.SimulationTime || MpRegenRemainder < 0 || MpRegenRemainder >= 1000 || LastDamageSequence < 0 || LastDamageSequence == long.MaxValue || Hp < 0 || Hp > CharacterRules.MaxHp(s) || Mp < 0 || Mp > CharacterRules.MaxMp(s) ||
                SharedPotionReadyAt < 0 || AntidoteReadyAt < 0 || SharedPotionReadyAt > s.SimulationTime + 5000 || AntidoteReadyAt > s.SimulationTime + 5000) throw new SerializationException("Invalid vitals");
        }
    }
    [DataContract]
    public sealed class BastionEffectState
    {
        [DataMember(IsRequired = true)] public long CounterReadyAt, CounterExpiresAt;
        [DataMember(IsRequired = true)] public long GuardReadyAt, GuardExpiresAt, ImpactReadyAt, PostureExpiresAt;
        [DataMember(IsRequired = true)] public bool PostureUsed;
        [DataMember(IsRequired = true)] public Dictionary<string, int> ImpactRawDamage = new Dictionary<string, int>();
        [DataMember(IsRequired = true)] public Dictionary<string, long> ImpactStartedAt = new Dictionary<string, long>();
        [DataMember(IsRequired = true)] public Dictionary<string, long> EventTimes = new Dictionary<string, long>();
        [DataMember(IsRequired = true)] public Dictionary<string, int> ImpactBonus = new Dictionary<string, int>();
        [DataMember(IsRequired = true)] public Dictionary<string, int> ImpactRuptureBp = new Dictionary<string, int>();
        [DataMember(IsRequired = true)] public Dictionary<string, int> ImpactRuptureDuration = new Dictionary<string, int>();
        [DataMember(IsRequired = true)] public Dictionary<string, string> AppliedHits = new Dictionary<string, string>();
        [DataMember(IsRequired = true)] public Dictionary<string, RuptureState> Ruptures = new Dictionary<string, RuptureState>();
        internal BastionEffectState Copy()
        { return new BastionEffectState { CounterReadyAt = CounterReadyAt, CounterExpiresAt = CounterExpiresAt, GuardReadyAt = GuardReadyAt, GuardExpiresAt = GuardExpiresAt, ImpactReadyAt = ImpactReadyAt, PostureExpiresAt = PostureExpiresAt, PostureUsed = PostureUsed,
            ImpactBonus = new Dictionary<string, int>(ImpactBonus), ImpactRawDamage = new Dictionary<string,int>(ImpactRawDamage), ImpactStartedAt = new Dictionary<string,long>(ImpactStartedAt), EventTimes = new Dictionary<string,long>(EventTimes), ImpactRuptureBp = new Dictionary<string,int>(ImpactRuptureBp), ImpactRuptureDuration = new Dictionary<string,int>(ImpactRuptureDuration), AppliedHits = new Dictionary<string, string>(AppliedHits),
            Ruptures = Ruptures.ToDictionary(x => x.Key, x => x.Value.Copy()) }; }
        internal void ClearPersonalActions()
        {
            CounterExpiresAt = 0; GuardExpiresAt = 0; PostureExpiresAt = 0;
            ImpactStartedAt.Clear(); ImpactBonus.Clear(); ImpactRawDamage.Clear(); ImpactRuptureBp.Clear(); ImpactRuptureDuration.Clear();
        }
        internal void Compact(long now)
        {
            foreach (string key in EventTimes.Where(x => now - x.Value > 7000).Select(x => x.Key).ToArray())
            { EventTimes.Remove(key); AppliedHits.Remove(key); }
            foreach (string key in Ruptures.Where(x => now >= x.Value.ExpiresAt).Select(x => x.Key).ToArray()) Ruptures.Remove(key);
            while (ImpactStartedAt.Count >= 32)
            {
                string id = ImpactStartedAt.OrderBy(x => x.Value).ThenBy(x => x.Key,StringComparer.Ordinal).First().Key;
                ImpactStartedAt.Remove(id); ImpactBonus.Remove(id); ImpactRawDamage.Remove(id); ImpactRuptureBp.Remove(id); ImpactRuptureDuration.Remove(id);
            }
        }
        internal void Validate(long now)
        {
            if (CounterReadyAt < 0 || CounterExpiresAt < 0 || GuardReadyAt < 0 || GuardExpiresAt < 0 || ImpactReadyAt < 0 || PostureExpiresAt < 0 || ImpactBonus == null || AppliedHits == null || Ruptures == null ||
                ImpactRawDamage == null || ImpactRawDamage.Count != ImpactBonus.Count || ImpactRawDamage.Any(x => !ImpactBonus.ContainsKey(x.Key) || x.Value < 1 || x.Value > 1000000) ||
                ImpactRuptureBp == null || ImpactRuptureDuration == null || ImpactStartedAt == null || EventTimes == null ||
                ImpactStartedAt.Count != ImpactBonus.Count || EventTimes.Count != AppliedHits.Count ||
                ImpactStartedAt.Any(x => !ImpactBonus.ContainsKey(x.Key) || x.Value < 0 || x.Value > now) ||
                EventTimes.Any(x => !AppliedHits.ContainsKey(x.Key) || x.Value < 0 || x.Value > now) ||
                ImpactRuptureBp.Count != ImpactBonus.Count || ImpactRuptureDuration.Count != ImpactBonus.Count ||
                ImpactBonus.Keys.Any(k => !ImpactRuptureBp.ContainsKey(k) || !ImpactRuptureDuration.ContainsKey(k)) ||
                ImpactRuptureBp.Any(x => x.Value != 0 && x.Value != 1500 && x.Value != 2000) ||
                ImpactRuptureDuration.Any(x => x.Value != 5000 && x.Value != 7000) ||
                CounterReadyAt > now + 6000 || CounterExpiresAt > now + 3000 || GuardReadyAt > now + 6000 ||
                GuardExpiresAt > now + 2000 || ImpactReadyAt > now + 7000 || PostureExpiresAt > now + 2000 ||
                ImpactBonus.Count > 32 || AppliedHits.Count > 2048 || Ruptures.Count > 256 ||
                ImpactBonus.Any(x => !CampaignState.ValidId(x.Key) || (x.Value != 0 && x.Value != 1500 && x.Value != 3000)) ||
                AppliedHits.Any(x => !CampaignState.ValidId(x.Key) || !CampaignState.ValidId(x.Value)) ||
                Ruptures.Any(x => !CampaignState.ValidId(x.Key) || x.Value == null || (x.Value.BasisPoints != 750 && x.Value.BasisPoints != 1000 && x.Value.BasisPoints != 1500 && x.Value.BasisPoints != 2000) || x.Value.ExpiresAt < 0 || x.Value.ExpiresAt > now + 7000))
                throw new SerializationException("Invalid specialization runtime");
        }
    }
    [DataContract]
    public sealed class RuptureState
    {
        [DataMember(IsRequired = true)] public int BasisPoints;
        [DataMember(IsRequired = true)] public long ExpiresAt;
        internal RuptureState Copy() { return (RuptureState)MemberwiseClone(); }
    }
    public static class BastionEffects
    {
        public static int DefenseAt(BastionEffectState state, string targetId, int baseDefense, long now)
        {
            if (baseDefense < 0 || now < 0) throw new ArgumentOutOfRangeException();
            RuptureState effect;
            return state.Ruptures.TryGetValue(targetId, out effect) && now < effect.ExpiresAt ? (int)((long)baseDefense * (10000 - effect.BasisPoints) / 10000) : baseDefense;
        }
        public static int ImpactDamage(BastionEffectState state, string actionId, string targetId, int defense,
            long now, bool critical = false, int multiplierBp = 15000, int resistanceBp = 0, int reductionBp = 0)
        {
            if (!state.ImpactRawDamage.ContainsKey(actionId)) throw new ArgumentException("Unknown or retired action");
            return CombatMath.DirectDamage(state.ImpactRawDamage[actionId],state.ImpactBonus[actionId],critical,multiplierBp,
                DefenseAt(state,targetId,defense,now),resistanceBp,reductionBp);
        }
        public static int ApplyDamageBonus(int rawDamage, int otherBonusBp, int counterBonusBp)
        {
            if (rawDamage < 0 || otherBonusBp < 0 || otherBonusBp > 10000 || (counterBonusBp != 0 && counterBonusBp != 1500 && counterBonusBp != 3000)) throw new ArgumentOutOfRangeException();
            long value = (long)rawDamage * (10000 + Math.Min(10000, otherBonusBp + counterBonusBp)) / 10000;
            if (value > int.MaxValue) throw new ArgumentOutOfRangeException();
            return (int)value;
        }
    }
}
