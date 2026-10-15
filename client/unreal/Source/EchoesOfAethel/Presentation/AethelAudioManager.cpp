#include "Presentation/AethelAudioManager.h"
#include "Components/AudioComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Sound/SoundBase.h"

void UAethelAudioManager::PlayBackgroundMusic(USoundBase* NewMusic, float FadeInTime, float FadeOutTime)
{
    if (!IsValid(NewMusic) || !GetWorld() || (NewMusic == CurrentMusic && IsValid(MusicComponent) && MusicComponent->IsPlaying())) return;
    if (IsValid(PreviousMusicComponent)) { PreviousMusicComponent->Stop(); PreviousMusicComponent->DestroyComponent(); PreviousMusicComponent = nullptr; }
    PreviousMusicComponent = MusicComponent;
    if (IsValid(PreviousMusicComponent)) PreviousMusicComponent->FadeOut(FMath::Max(0.0f, FadeOutTime), 0.0f);
    MusicComponent = UGameplayStatics::CreateSound2D(GetWorld(), NewMusic, 1.0f, 1.0f, 0.0f, nullptr, false, false);
    if (IsValid(MusicComponent))
    {
        MusicComponent->FadeIn(FMath::Max(0.0f, FadeInTime), IsValid(NarrationComponent) && NarrationComponent->IsPlaying() ? 0.6f : 1.0f);
        CurrentMusic = NewMusic;
    }
}

void UAethelAudioManager::PlayNarration(USoundBase* VoiceLine)
{
    if (!IsValid(VoiceLine) || !GetWorld()) return;
    StopNarration();
    NarrationComponent = UGameplayStatics::CreateSound2D(GetWorld(), VoiceLine, 1.0f, 1.0f, 0.0f, nullptr, false, false);
    if (IsValid(NarrationComponent))
    {
        NarrationComponent->OnAudioFinished.AddDynamic(this, &UAethelAudioManager::OnNarrationFinished);
        if (IsValid(MusicComponent)) MusicComponent->SetVolumeMultiplier(0.6f);
        NarrationComponent->Play();
    }
}

void UAethelAudioManager::StopNarration()
{
    if (IsValid(NarrationComponent))
    {
        NarrationComponent->OnAudioFinished.RemoveDynamic(this, &UAethelAudioManager::OnNarrationFinished);
        NarrationComponent->Stop();
        NarrationComponent->DestroyComponent();
        NarrationComponent = nullptr;
    }
    if (IsValid(MusicComponent)) MusicComponent->SetVolumeMultiplier(1.0f);
}

void UAethelAudioManager::OnNarrationFinished() { StopNarration(); }

void UAethelAudioManager::Deinitialize()
{
    StopNarration();
    if (IsValid(MusicComponent)) { MusicComponent->Stop(); MusicComponent->DestroyComponent(); }
    if (IsValid(PreviousMusicComponent)) { PreviousMusicComponent->Stop(); PreviousMusicComponent->DestroyComponent(); }
    MusicComponent = nullptr; PreviousMusicComponent = nullptr; CurrentMusic = nullptr;
    Super::Deinitialize();
}
