#include "AethelBossTrigger.h"
#include "AethelMovieDirector.h"
#include "Components/BoxComponent.h"
#include "GameFramework/Pawn.h"
#include "GameFramework/PlayerController.h"

AAethelBossTrigger::AAethelBossTrigger()
{
    PrimaryActorTick.bCanEverTick = false;
    TriggerBox = CreateDefaultSubobject<UBoxComponent>(TEXT("TriggerBox"));
    SetRootComponent(TriggerBox);
    TriggerBox->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
    TriggerBox->SetCollisionResponseToAllChannels(ECR_Ignore);
    TriggerBox->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
    TriggerBox->SetGenerateOverlapEvents(true);
}

void AAethelBossTrigger::BeginPlay()
{
    Super::BeginPlay();
    TriggerBox->OnComponentBeginOverlap.AddDynamic(this, &AAethelBossTrigger::OnPlayerEnterArena);
}

void AAethelBossTrigger::OnPlayerEnterArena(UPrimitiveComponent*, AActor* OtherActor, UPrimitiveComponent*, int32, bool, const FHitResult&)
{
    if (bHasTriggered || !IsValid(TargetBossActor) || !IsValid(MovieDirector)) return;
    APawn* PlayerPawn = Cast<APawn>(OtherActor);
    if (!IsValid(PlayerPawn) || !Cast<APlayerController>(PlayerPawn->GetController())) return;
    bHasTriggered = MovieDirector->PlayBossIntroduction(TargetBossActor, PlayerPawn);
}

void AAethelBossTrigger::ResetForEncounter() { bHasTriggered = false; }
