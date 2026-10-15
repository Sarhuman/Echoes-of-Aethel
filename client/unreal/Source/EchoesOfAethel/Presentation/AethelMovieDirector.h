#pragma once
#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "AethelMovieDirector.generated.h"

class ACameraActor;
class APlayerController;
class APawn;
class USoundBase;
class UCameraShakeBase;

UCLASS()
class ECHOESOFAETHEL_API AAethelMovieDirector : public AActor
{
    GENERATED_BODY()
public:
    AAethelMovieDirector();
    UPROPERTY(EditInstanceOnly, BlueprintReadWrite, Category="Aethel|Cinematic") TObjectPtr<ACameraActor> IntroCamera;
    UPROPERTY(EditInstanceOnly, BlueprintReadWrite, Category="Aethel|Cinematic") TObjectPtr<ACameraActor> BossCamera;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Cinematic") TObjectPtr<USoundBase> BossRoarSound;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Cinematic") TSubclassOf<UCameraShakeBase> CameraShakeClass;
    UFUNCTION(BlueprintCallable, Category="Aethel|Cinematic") bool PlayBossIntroduction(AActor* TargetBoss, APawn* PlayerPawn);
    UFUNCTION(BlueprintCallable, Category="Aethel|Cinematic") void SkipIntroduction();
    UFUNCTION(BlueprintPure, Category="Aethel|Cinematic") bool IsPlayingIntroduction() const { return bPlaying; }
    UFUNCTION(BlueprintImplementableEvent, Category="Aethel|Cinematic") void OnIntroductionFinished(AActor* TargetBoss, bool bSkipped);
protected:
    virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;
private:
    FTimerHandle ShotTimerHandle;
    FTimerHandle EndTimerHandle;
    TWeakObjectPtr<APawn> CachedPlayer;
    TWeakObjectPtr<APlayerController> CachedController;
    TWeakObjectPtr<AActor> CachedBoss;
    TWeakObjectPtr<AActor> PreviousViewTarget;
    bool bPlaying = false;
    void TransitionToBossShot();
    void FinishIntroduction(bool bSkipped);
};
