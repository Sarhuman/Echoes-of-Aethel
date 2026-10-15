#if WITH_DEV_AUTOMATION_TESTS

#include "../../Domain/AethelPlayerStats.h"
#include "Misc/AutomationTest.h"

IMPLEMENT_SIMPLE_AUTOMATION_TEST(
    FAethelPlayerStatsTest,
    "Aethel.Domain.PlayerStats",
    EAutomationTestFlags::EditorContext | EAutomationTestFlags::EngineFilter)

bool FAethelPlayerStatsTest::RunTest(const FString& Parameters)
{
    FAethelPlayerStats Stats;
    Stats.VIT = 14;
    Stats.Intelligence = 3;
    Stats.FTH = 5;

    TestTrue(TEXT("Level 1 attributes are valid"), Stats.RefreshDerivedStats());
    TestEqual(TEXT("Level 1 Bastion HP"), Stats.MaxHP, 172);
    TestEqual(TEXT("Level 1 Bastion MP"), Stats.MaxMP, 52);

    Stats.Level = 2;
    TestTrue(TEXT("Level 2 attributes are valid"), Stats.RefreshDerivedStats());
    TestEqual(TEXT("Level 2 Bastion HP"), Stats.MaxHP, 178);
    TestEqual(TEXT("Level 2 Bastion MP"), Stats.MaxMP, 55);

    Stats.Level = 0;
    TestFalse(TEXT("Level zero is rejected"), Stats.RefreshDerivedStats());
    TestEqual(TEXT("Rejected update preserves HP"), Stats.MaxHP, 178);
    TestEqual(TEXT("Rejected update preserves MP"), Stats.MaxMP, 55);

    Stats.Level = 1;
    Stats.VIT = 0;
    TestFalse(TEXT("Invalid vitality is rejected"), Stats.RefreshDerivedStats());
    TestEqual(TEXT("Rejected vitality preserves HP"), Stats.MaxHP, 178);

    return true;
}

#endif
