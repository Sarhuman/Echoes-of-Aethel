using System;

namespace Aethel.Campaign
{
    public static class CombatMath
    {
        // GDD 01/05: floor at each stage. Basis points; no random roll or scene access.
        public static int DirectDamage(int raw, int bonusBp, bool critical, int criticalMultiplierBp,
            int defense, int resistanceBp, int reductionBp, bool hit = true, bool immune = false)
        {
            if (raw < 0 || raw > 1000000 || bonusBp < 0 || bonusBp > 10000 || criticalMultiplierBp < 10000 || criticalMultiplierBp > 20000 ||
                defense < 0 || defense > 1000000 || resistanceBp < -5000 || resistanceBp > 7500 || reductionBp < 0 || reductionBp > 6000)
                throw new ArgumentOutOfRangeException();
            if (!hit || immune) return 0;
            long value = (long)raw * (10000 + bonusBp) / 10000;
            if (critical) value = value * criticalMultiplierBp / 10000;
            value = value * 100 / (100 + defense);
            value = value * (10000 - resistanceBp) / 10000;
            value = value * (10000 - reductionBp) / 10000;
            return (int)Math.Max(1,value);
        }
    }
}
