#include "Presentation/AethelFootstepComponent.h"
#include "Engine/EngineTypes.h"
#include "Engine/World.h"
#include "Kismet/GameplayStatics.h"
#include "PhysicalMaterials/PhysicalMaterial.h"
#include "Sound/SoundBase.h"

UAethelFootstepComponent::UAethelFootstepComponent() { PrimaryComponentTick.bCanEverTick = false; }

void UAethelFootstepComponent::PlayFootstepSound()
{
    AActor* Owner = GetOwner();
    if (!IsValid(Owner) || !GetWorld()) return;
    const FVector Start = Owner->GetActorLocation();
    const FVector End = Start - FVector(0.0f, 0.0f, 180.0f);
    FCollisionQueryParams Params(SCENE_QUERY_STAT(AethelFootstep), false, Owner);
    Params.bReturnPhysicalMaterial = true;
    FHitResult Hit;
    if (!GetWorld()->LineTraceSingleByChannel(Hit, Start, End, ECC_Visibility, Params)) return;
    USoundBase* Sound = DefaultStoneSound;
    if (const UPhysicalMaterial* Material = Hit.PhysMaterial.Get())
    {
        if (Material->SurfaceType == SurfaceType1) Sound = MarbleSound ? MarbleSound.Get() : DefaultStoneSound.Get();
        if (Material->SurfaceType == SurfaceType2) Sound = MudSound ? MudSound.Get() : DefaultStoneSound.Get();
    }
    if (IsValid(Sound)) UGameplayStatics::PlaySoundAtLocation(this, Sound, Hit.ImpactPoint);
}
