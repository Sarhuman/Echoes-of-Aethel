#include "Domain/AethelPlayerStats.h"

static_assert(FAethelPlayerStats::CalculateMaxHP(1, 14) == 172);
static_assert(FAethelPlayerStats::CalculateMaxMP(1, 3, 5) == 52);
static_assert(FAethelPlayerStats::CalculateMaxHP(2, 14) == 178);
static_assert(FAethelPlayerStats::CalculateMaxMP(2, 3, 5) == 55);

bool FAethelPlayerStats::HasValidBaseAttributes() const
{
    return Level >= 1 && Level <= 30
        && STR >= 1 && STR <= 99 && DEX >= 1 && DEX <= 99
        && INT >= 1 && INT <= 99 && FTH >= 1 && FTH <= 99
        && VIT >= 1 && VIT <= 99;
}

bool FAethelPlayerStats::RefreshDerivedStats()
{
    if (!HasValidBaseAttributes()) return false;
    MaxHP = CalculateMaxHP(Level, VIT);
    MaxMP = CalculateMaxMP(Level, INT, FTH);
    return true;
}
