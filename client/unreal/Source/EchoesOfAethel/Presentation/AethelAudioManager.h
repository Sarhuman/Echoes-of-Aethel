#pragma once
#include "CoreMinimal.h"
#include "Subsystems/WorldSubsystem.h"
#include "AethelAudioManager.generated.h"

class UAudioComponent;
class USoundBase;

UCLASS()
class ECHOESOFAETHEL_API UAethelAudioManager : public UWorldSubsystem
{
    GENERATED_BODY()
public:
    UFUNCTION(BlueprintCallable, Category="Aethel|Audio") void PlayBackgroundMusic(USoundBase* NewMusic, float FadeInTime = 1.5f, float FadeOutTime = 1.5f);
    UFUNCTION(BlueprintCallable, Category="Aethel|Audio") void PlayNarration(USoundBase* VoiceLine);
    UFUNCTION(BlueprintCallable, Category="Aethel|Audio") void StopNarration();
    virtual void Deinitialize() override;
private:
    UPROPERTY() TObjectPtr<UAudioComponent> MusicComponent;
    UPROPERTY() TObjectPtr<UAudioComponent> PreviousMusicComponent;
    UPROPERTY() TObjectPtr<UAudioComponent> NarrationComponent;
    UPROPERTY() TObjectPtr<USoundBase> CurrentMusic;
    UFUNCTION() void OnNarrationFinished();
};
