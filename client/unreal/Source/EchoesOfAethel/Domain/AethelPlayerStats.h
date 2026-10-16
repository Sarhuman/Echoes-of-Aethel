#pragma once

#include "CoreMinimal.h"
#include "AethelPlayerStats.generated.h"

// Generic attribute block. Class presets supply the five values; no class is assumed here.
USTRUCT(BlueprintType)
struct ECHOESOFAETHEL_API FAethelPlayerStats
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Stats") int32 Level = 1;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Stats") int32 STR = 1;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Stats") int32 DEX = 1;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Stats") int32 Intelligence = 1;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Stats") int32 FTH = 1;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Stats") int32 VIT = 1;
    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Aethel|Stats") int32 MaxHP = 68;
    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Aethel|Stats") int32 MaxMP = 36;

    static constexpr int32 CalculateMaxHP(int32 InLevel, int32 InVIT)
    {
        return 60 + 8 * InVIT + 6 * (InLevel - 1);
    }

    static constexpr int32 CalculateMaxMP(int32 InLevel, int32 InIntelligence, int32 InFTH)
    {
        return 30 + 4 * InIntelligence + 2 * InFTH + 3 * (InLevel - 1);
    }

    bool HasValidBaseAttributes() const;
    bool RefreshDerivedStats();
};
