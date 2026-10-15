#pragma once
#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "AethelBossTrigger.generated.h"

class UBoxComponent;
class AAethelMovieDirector;

UCLASS()
class ECHOESOFAETHEL_API AAethelBossTrigger : public AActor
{
    GENERATED_BODY()
public:
    AAethelBossTrigger();
    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Aethel|Boss") TObjectPtr<UBoxComponent> TriggerBox;
    UPROPERTY(EditInstanceOnly, BlueprintReadWrite, Category="Aethel|Boss") TObjectPtr<AActor> TargetBossActor;
    UPROPERTY(EditInstanceOnly, BlueprintReadWrite, Category="Aethel|Boss") TObjectPtr<AAethelMovieDirector> MovieDirector;
    UFUNCTION(BlueprintCallable, Category="Aethel|Boss") void ResetForEncounter();
protected:
    virtual void BeginPlay() override;
private:
    bool bHasTriggered = false;
    UFUNCTION() void OnPlayerEnterArena(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComponent, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
};
