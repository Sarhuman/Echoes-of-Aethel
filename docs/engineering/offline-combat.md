# Combate offline — ataques reais na cena técnica

Base: PR #16, commit 9809c40cb3399fa3bbf4ef13232b25c46b2b6f7b. Godot 4.6.

## Executar

Abrir `client/godot/project.godot` e F5. WASD/setas movem; J ataca; K executa pesado; Espaço esquiva; R recarrega a cena técnica. F8/F9 preservam apresentação opcional/redução de efeitos. HUD mostra HP, MP, stamina, fase e HP do inimigo. O cone representa a direção/alcance lógico projetado; amarelo prepara, vermelho ativo, cinza recupera.

| Ataque | Preparação / ativo / recuperação | Custo | Dano | Alcance lógico |
|---|---|---|---|---|
| Básico J | 150 / 80 / 250 ms | Gratuito | 12 | 56 px |
| Pesado K | 350 / 80 / 370 ms | 12 MP | 24 | 64 px |

Os 800 ms do pesado seguem a revisão da conversa. Demais valores são configuração EXPERIMENTAL desta cena, não fórmula de arma nem balanceamento final. MP máximo 40, sem regeneração/poção nesta entrega; R permite repetir o teste. Não interpretar os 24 como solução da ambiguidade do +22 do machado. Stamina continua no dash (25); guarda, interrupção por dano comum, recompensa, migração v2 e checkpoint permanecem pendentes.

## Responsabilidades

- `AttackDefinition`: Resource de conteúdo confiável do projeto, copiado ao aceitar ataque; não é formato de save externo.
- `CombatService`: matemática pura de custos, linha do tempo e cone com projeção inversa `(x, 2*y)`.
- `AttackController`: estado por ator, direção fixa, histórico limitado, intervalo ativo e registro de alvos. Sem acesso a árvore, input, save ou sinais.
- `Player`: confirma resultado e saldos sem await/callback intermediário, depois emite sinais; consulta física e chama ActorBase.
- `ActorBase`: mantém contrato inteiro de HP, morte/persistência de NPC e recusa dano na Cidadela em qualquer direção.

Sequência de comando é inteiro positivo monotônico, local ao tempo de vida do controlador. Histórico guarda 128 resultados (incluindo recusas); comandos anteriores removidos são recusados por high-water mark. Repetições aceitas retornam REPLAYED e nunca reiniciam golpe/recurso. Mudança em direção, ID, custos, tempos ou parâmetros gera conflito. Reset só com novo ator/cena, sem comandos antigos em trânsito. Isto não é protocolo multiplayer ou ledger persistente.

Reserva de alvo acontece antes de aplicar dano; reentrada durante callback é recusada. Aplicação recusada libera reserva. Morte/dash permitido cancela janela; pagamento não é devolvido. Recuperação normal admite dash, pesado não. Dano comum não interrompe automaticamente nesta entrega.

## Semântica temporal e física

A linha do tempo mantém tempo absoluto do golpe e calcula interseção de duração positiva com ACTIVE; delta zero não cria dano. Um passo de 500 ms do pesado cruza ACTIVE e termina 70 ms dentro de RECOVERY. IDLE não produz janela.

A cena usa amostragem discreta do estado físico no início de cada passo, antes do movimento do Player. Consulta espacial circular (máscara 1, máximo 128 resultados), filtro de centros por cone lógico de 120 graus e raio de oclusão na máscara 2. Registro é por instância de alvo, não por frame. Não há rewind, interpolação histórica nem sweep de alvos móveis: o intervalo atravessado evita perder toda a fase num passo longo, mas não reconstrói trajetórias. Para alta velocidade/densidade ou multiplayer, substituir o adaptador físico e validar limites; não extrapolar testes estáticos para colisão contínua.

Animações e indicador são apresentação. A onda do boss não foi implementada. A conta 103,3–250 ms permanece um modelo frontal com jogador inicialmente parado, não garantia de gameplay.

## Referências externas avaliadas

- [godot-tactical-rpg: pawn](https://github.com/ramaureirac/godot-tactical-rpg/blob/development/data/modules/tactics/level/pawn/pawn.gd), blob b0d5d98a4f4930dd5669f5e2acd3b74f955acd7b.
- [Serviço de combate](https://github.com/ramaureirac/godot-tactical-rpg/blob/development/data/models/world/combat/participant/pawn/service/combat.gd), blob 410c69128d731a3c56a9db5da16d1051d7445034.

Aproveitado o padrão de composição ator/modelo/serviço. Implementação original para tempo real 2D, sem transplantar turnos, CharacterBody3D ou código. Nenhum asset de terceiros foi adicionado.

[Maaack GameSaving](https://github.com/Maaack/Godot-Game-Template/blob/main/addons/maaacks_game_template/docs/GameSaving.md), blob 7c3173793f60395ed79e100095c0ed74d633302f: o próprio autor alerta que saves Resource podem executar scripts injetados. Mantido o envelope JSON existente. Menus/scene loader ficam como candidatos após o loop de combate.

[Dialogic](https://github.com/dialogic-godot/dialogic): candidato à apresentação de diálogos através de adaptador; mínimo Godot 4.5 segundo README consultado. Não instalado neste marco. QuestService/GameState deverão continuar donos das consequências e do save. Fixar release e conferir licenças dos arquivos quando houver incorporação.

## Validação

Godot 4.6 Linux headless: 41 combate + 19 Player + 25 FSM + 27 save + 13 apresentação = 125 verificações, sem falhas. Testes de combate incluem integração real de consultas físicas com parede, recursos, morte e zona segura. Importação do editor e smoke da cena também verificados. Sem playtest visual/manual, export Windows ou benchmark de densidade.

```bash
godot --headless --editor --path client/godot --quit
godot --headless --path client/godot --script res://tests/test_combat.gd
```
