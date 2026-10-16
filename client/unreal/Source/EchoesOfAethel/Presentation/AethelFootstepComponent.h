#pragma once
#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "AethelFootstepComponent.generated.h"

class USoundBase;

UCLASS(ClassGroup=(Aethel), meta=(BlueprintSpawnableComponent))
class ECHOESOFAETHEL_API UAethelFootstepComponent : public UActorComponent
{
    GENERATED_BODY()
public:
    UAethelFootstepComponent();
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Footsteps") TObjectPtr<USoundBase> DefaultStoneSound;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Footsteps") TObjectPtr<USoundBase> MarbleSound;
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Aethel|Footsteps") TObjectPtr<USoundBase> MudSound;
    UFUNCTION(BlueprintCallable, Category="Aethel|Footsteps") void PlayFootstepSound();
};
