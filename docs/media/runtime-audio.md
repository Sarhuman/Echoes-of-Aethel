# Áudio original de runtime

As três faixas (`citadel.wav`, `forest.wav`, `combat.wav`) e os seis efeitos em `client/godot/audio/` foram sintetizados especificamente para Echoes of Aethel por `tools/generate_audio.py`. O script compõe notas, acordes, drones, pulsos e transientes matematicamente, com semente fixa. Não usa gravações, samples, soundfonts nem material de terceiros. Os WAVs são PCM mono 16-bit a 22,05 kHz e podem ser regenerados com Python 3 executando `python tools/generate_audio.py` na raiz.

O diretor de áudio alterna Cidadela, Bosque e combate conforme o estado real do jogo; os efeitos acompanham ações confirmadas de machado, Impacto, Guarda, Fenda, esquiva e dano. A opção de som silencia música e efeitos. Estas faixas são uma primeira composição funcional: mixagem, transições sem lacuna e avaliação humana em fones e caixas continuam sujeitas a playtest.
