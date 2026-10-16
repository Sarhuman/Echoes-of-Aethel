#include "AethelMovieDirector.h"
#include "Camera/CameraActor.h"
#include "Camera/CameraShakeBase.h"
#include "GameFramework/Character.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/Pawn.h"
#include "GameFramework/PlayerController.h"
#include "Kismet/GameplayStatics.h"
#include "Sound/SoundBase.h"
#include "TimerManager.h"

AAethelMovieDirector::AAethelMovieDirector() { PrimaryActorTick.bCanEverTick = false; }

bool AAethelMovieDirector::PlayBossIntroduction(AActor* TargetBoss, APawn* PlayerPawn)
{
    if (bPlaying || !IsValid(TargetBoss) || !IsValid(PlayerPawn) || !IsValid(IntroCamera) || !IsValid(BossCamera)) return false;
    APlayerController* Controller = Cast<APlayerController>(PlayerPawn->GetController());
    if (!IsValid(Controller)) return false;
    CachedBoss = TargetBoss;
    CachedPlayer = PlayerPawn;
    CachedController = Controller;
    PreviousViewTarget = Controller->GetViewTarget();
    bPlaying = true;
    Controller->SetIgnoreMoveInput(true);
    Controller->SetIgnoreLookInput(true);
    Controller->StopMovement();
    if (ACharacter* Character = Cast<ACharacter>(PlayerPawn))
    {
        if (UCharacterMovementComponent* Movement = Character->GetCharacterMovement())
        {
            Movement->StopMovementImmediately();
        }
    }
    Controller->SetViewTargetWithBlend(IntroCamera, 0.0f);
    GetWorldTimerManager().SetTimer(ShotTimerHandle, this, &AAethelMovieDirector::TransitionToBossShot, 2.0f, false);
    return true;
}

void AAethelMovieDirector::TransitionToBossShot()
{
    if (!bPlaying || !CachedController.IsValid() || !CachedBoss.IsValid() || !IsValid(BossCamera)) { FinishIntroduction(true); return; }
    CachedController->SetViewTargetWithBlend(BossCamera, 1.5f, VTBlend_EaseIn);
    if (IsValid(BossRoarSound)) UGameplayStatics::PlaySoundAtLocation(this, BossRoarSound, CachedBoss->GetActorLocation());
    if (CameraShakeClass) CachedController->ClientStartCameraShake(CameraShakeClass);
    GetWorldTimerManager().SetTimer(EndTimerHandle, [this]() { FinishIntroduction(false); }, 3.0f, false);
}

void AAethelMovieDirector::SkipIntroduction() { FinishIntroduction(true); }

void AAethelMovieDirector::FinishIntroduction(bool bSkipped)
{
    if (!bPlaying) return;
    GetWorldTimerManager().ClearTimer(ShotTimerHandle);
    GetWorldTimerManager().ClearTimer(EndTimerHandle);
    bPlaying = false;
    if (CachedController.IsValid())
    {
        AActor* ReturnTarget = PreviousViewTarget.IsValid() ? PreviousViewTarget.Get() : CachedPlayer.Get();
        if (IsValid(ReturnTarget)) CachedController->SetViewTargetWithBlend(ReturnTarget, bSkipped ? 0.0f : 1.0f, VTBlend_EaseOut);
        CachedController->SetIgnoreMoveInput(false);
        CachedController->SetIgnoreLookInput(false);
    }
    AActor* FinishedBoss = CachedBoss.Get();
    CachedBoss.Reset(); CachedPlayer.Reset(); CachedController.Reset(); PreviousViewTarget.Reset();
    OnIntroductionFinished(FinishedBoss, bSkipped);
}

void AAethelMovieDirector::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
    FinishIntroduction(true);
    Super::EndPlay(EndPlayReason);
}
