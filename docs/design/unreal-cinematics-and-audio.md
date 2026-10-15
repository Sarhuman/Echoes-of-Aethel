# Cinemáticas e áudio — primeira prova Unreal

**Estado: infraestrutura C++ sem cena, assets ou build verificada (14/10/2026).** Os textos recebidos sobre diretor de boss, música, passos e fase de 50% são referências de direção; não comprovam que esses eventos já acontecem no jogo. A prévia narrativa enviada pelo usuário descreve uma meta de experiência, não a execução atual.

## Encontro e cinemática

`AAethelBossTrigger` aceita apenas um Pawn controlado por jogador, requer boss e diretor configurados e dispara uma vez por tentativa. `AAethelMovieDirector` exige duas câmeras, guarda o alvo anterior, bloqueia movimento/olhar, permite pular, limpa timers e devolve câmera e input. `OnIntroductionFinished` é um gancho visual de Blueprint; a autoridade de iniciar o combate, exibir HP, mudar música ou avançar quest pertence aos sistemas de encontro quando existirem. Pular e assistir devem chegar ao mesmo estado confirmado. O gatilho não cria boss, NPC, câmera ou arena automaticamente.

Primeiro teste no editor: duas câmeras na arena pequena do Bosque, um actor de boss de teste, passagem repetida pelo volume, pulo nos dois planos, destruição do boss durante o take, troca de nível e retorno do controle. Verificar a barra de boss e o áudio apenas depois de existirem widgets e sons originais. O subchefe inicial é Garras de Musgo; Rei Lich e Rei Orc são propostas de dungeons opcionais, não substituem Silvano como boss do ato.

## Música, voz e passos

`UAethelAudioManager` é um `UWorldSubsystem`: mantém dois componentes de música para transição com crossfade e um de narração com ducking simples. Um novo nível reinicia o subsistema e precisa escolher seu estado musical. `UAethelFootstepComponent::PlayFootstepSound` deve ser chamado por `AnimNotify` no contato real do pé; o raycast usa material físico, não o nome do asset. `SurfaceType1 = AethelMarble` e `SurfaceType2 = AethelMud` estão no `DefaultEngine.ini`; pedra é fallback. Os materiais e sons ainda precisam ser criados e atribuídos no editor. A execução dos passos depende de malha, animação, notify e colisão de piso existentes.

Direção original: Cidadela com timbres esparsos e abrigo melancólico; Bosque com vento, madeira e silêncio; boss com contraste rítmico e peso. Referências a Diablo, Castlevania e Star Wars informam atmosfera, sem copiar melodias, timbres identificáveis ou gravações. Evitar TTS pausado: prólogo pode começar com texto e música original; voz entra só com interpretação e gravação aprovadas. Todos os futuros arquivos de áudio exigem origem/licença em `docs/media/runtime-audio.md`, mixagem de volume acessível e opção de reduzir efeitos.

## Mecânicas ainda não implementadas

Invulnerabilidade a 50%, quatro lacaios, flanqueamento com Shift, click-to-move, HUD de boss e carregamento instantâneo da dungeon são hipóteses de design. A transição de fase precisa de telegraph, pontos de spawn navegáveis, duração limitada, regra para lacaios e estado persistente do encontro. Nunca criar lacaios com `AlwaysSpawn` em posições cardeais cegas; podem nascer dentro de parede ou bloquear o jogador. Números e penalidades de combate exigem tabela de balanceamento e playtest antes da implementação.
