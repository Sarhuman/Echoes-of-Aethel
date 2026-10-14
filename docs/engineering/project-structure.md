# Estrutura do projeto Godot

Revisão: 14/10/2026 · Godot 4.6 stable · GDScript · primeira fatia `0.2.0-dev`.

Este documento é o mapa operacional para quem implementa (incluindo o GPT). Ele **não substitui** [escopo](project-scope.md), [decisões de produção](production-decisions.md) nem o [GDD](../GDD/README.md). Coluna **Estado**: `existe` = no repositório e testado; `planejado` = pasta/arquivo a criar por um cartão de [tasks/](tasks/README.md); `vazio` = pasta reservada com `.gitkeep`.

## 1. Árvore alvo

`res://` = `client/godot/`.

```text
client/godot/
├── project.godot                 existe  (autoloads: EventBus, GameState, SaveSystem)
├── core/                         Infraestrutura global e estado de sessão
│   ├── event_bus.gd              existe  (autoload)  sinais de fatos confirmados
│   ├── game_state.gd             existe  (autoload)  projeção do mundo em memória
│   ├── save_system.gd            existe  (autoload)  JSON + SHA-256 + backup
│   ├── world_save_schema.gd      existe  validação/normalização do payload
│   └── content_catalog.gd        planejado (T00)     índice e validador de conteúdo
├── combat/                       Domínio de combate (sem nós de cena)
│   ├── attack_definition.gd      existe  Resource tipado
│   ├── combat_service.gd         existe  matemática pura
│   ├── attack_controller.gd      existe  comandos, fases, alvos
│   ├── basic.tres, heavy.tres    existe  (mantidos aqui; ver T12 para mover)
│   ├── damage_calculator.gd      planejado (T01A)    fórmula GDD 01 §1.3
│   ├── character_stats.gd        planejado (T01A)    atributos e derivados
│   ├── resource_regen.gd         planejado (T01A)    regeneração de MP
│   └── guard_controller.gd       planejado (T01)
├── content/                      Conteúdo declarativo confiável (Resources .tres)
│   ├── attacks/                  vazio    novos ataques/habilidades (T01)
│   ├── classes/                  vazio    ClassDefinition + twilight_bastion (T01A)
│   ├── enemies/                  vazio    EnemyDefinition (T02, T04)
│   ├── items/                    vazio    ItemDefinition (T05)
│   ├── loot/                     vazio    LootTable (T07)
│   ├── quests/                   vazio    QuestDefinition act1.spy_hunt (T08)
│   └── dialogues/                vazio    DialogueDefinition (T09)
├── entities/                     Adaptadores Godot: corpos, IA, cenas de personagens
│   ├── actor_base.gd             existe  CharacterBody2D + HP/morte
│   ├── player/player.gd          existe  movimento 2:1, dash, ataques 1 e 2/Q
│   ├── ai/enemy_fsm.gd           existe  IDLE/PATROL/CHASE/ATTACK/DEAD
│   ├── enemies/                  vazio    cenas .tscn dos inimigos (T02, T04)
│   └── npcs/                     vazio    NPCs da Cidadela e da aldeia (T09)
├── inventory/                    vazio    inventário, peso, equipamento, consumíveis (T05)
├── quest/                        vazio    domínio de quest/consequências (T08)
├── dialogue/                     vazio    resolver e adaptador de diálogo (T09)
├── services/                     vazio    serviços de aplicação (zona, loot, crime) (T03, T07)
├── world/
│   ├── maps/                     vazio    cenas dos mapas (citadel, forest_01) (T03)
│   ├── zones/                    vazio    ZoneDefinition/transições (T03)
│   └── interactables/            vazio    portões, baús, pontos de checkpoint (T03)
├── presentation/                 Apresentação opcional (recebe fatos, não decide)
│   ├── attack_indicator.gd       existe
│   └── enhanced/combat_presentation.gd  existe
├── ui/
│   ├── hud/                      vazio    HUD final (T10)
│   ├── menus/                    vazio    título, pausa, opções, controles (T10)
│   └── theme/                    vazio    Theme .tres (T10)
├── audio/{sfx,music}/            vazio    áudio mínimo (T10)
├── assets/{sprites,tilesets,fonts,vfx}/  vazio  somente com origem/licença registradas
├── scenes/test_scene.{tscn,gd}   existe  cena técnica atual (main scene)
└── tests/                        Suítes headless
    ├── test_*.gd                 existe  save, FSM, player, apresentação, combate
    ├── reload_probe.gd           existe  sonda auxiliar (não é suíte)
    └── integration/              vazio    testes com cenas completas
```

Fora de `client/godot/`: `docs/` (design e engenharia), `tools/run_tests.sh` (executa tudo), `AGENTS.md` (regras do GPT). Decisões e números: [decisions-first-slice.md](decisions-first-slice.md) e [experimental-values.md](experimental-values.md).

## 2. Camadas e regras de dependência

Seta = "pode depender de". Dependência contrária é proibida.

```text
presentation / ui / audio ──▶ services ──▶ domínio (combat, inventory, quest, dialogue-resolver)
        │                        │                     │
        └──────────▶ core (EventBus, GameState, SaveSystem, WorldSaveSchema) ◀──┘
entities (adaptadores) ──▶ domínio + core        content/*.tres ◀── lidos por domínio via catálogo
```

| Camada | Pode | Não pode |
|---|---|---|
| Domínio | Cálculo puro, validação, `RefCounted`/`Resource`, funções `static` | Referenciar nós de cena, `UI`, áudio, `get_tree()`, disco |
| Serviços | Orquestrar domínio, chamar `SaveSystem`, emitir fatos no `EventBus` **após** confirmar | Desenhar, tocar áudio, conter regra de cálculo duplicada |
| Entidades | `move_and_slide`, física, input, aplicar resultado validado | Decidir loot, XP, progresso ou escrever save direto (exceto o já existente em `ActorBase` para NPC narrativo) |
| Apresentação/UI | Reagir a sinais/fatos, exibir estado | Conceder dano, recompensa, progresso; alterar dicionários do `GameState` |
| Conteúdo | Dados tipados, IDs estáveis | Código executável, caminhos arbitrários vindos de save |

## 3. Contratos existentes que não podem quebrar

| Contrato | Onde | Observação |
|---|---|---|
| Autoloads `EventBus`, `GameState`, `SaveSystem` | `project.godot` | Nomes fixos; scripts usam `get_node("/root/...")` ou o nome global |
| `SaveSystem.SCHEMA_VERSION = 1`, `CONTENT_VERSION = "world-save-1"` | `save_system.gd`, `world_save_schema.gd` | Só mudar com migração e testes de recuperação (T06) |
| `WorldSaveSchema.validate/normalize/valid_id` | `world_save_schema.gd` | Payload exato: `world_flags, npc_states, faction_reputation, quest_choices, pending_effects, applied_effects, progress` |
| `progress.ritual_charge == quest_choices.size()` | `validate()` | Invariante: cada decisão confirmada soma 1 |
| `progress.act == (map-1)/3 + 1`; `map` 1–12; `act` 1–4 | `validate()` | |
| `WorldSaveSchema.ZONES = [citadel, dunes_camp, underground_refuge, liminal_post]` | `world_save_schema.gd` | **Não alterar** no slice: único checkpoint = `citadel` (D1) |
| `npc_states` ∈ `alive, dead, hostile`; `dead` é terminal | schema/`ActorBase` | |
| `ActorBase.take_damage(amount, attacker)` recusa dano na zona `citadel` | `actor_base.gd` | Hub nunca causa dano |
| `AttackController.request(seq, def, dir, mp, stamina, can_act)` | `attack_controller.gd` | Sequência monotônica; replays não reiniciam golpe |
| Projeção 2:1: lógico `(x,y)` → tela `(x, y×0.5)`; alcance de combate em métrica lógica | `player.gd`, `combat_service.gd` | |
| InputMap: `move_*`, `dash`, `attack_basic`, `skill_1..4`, `debug_restart` | `project.godot` | |
| Camadas de colisão: máscara 1 = atores; máscara 2 = paredes/oclusão | cena e testes | |

## 4. IDs de domínio já definidos nos docs (reutilize, não invente)

| Tipo | IDs |
|---|---|
| Classe | `twilight_bastion` |
| Mapa/zona | `forest_01` (Bosque das Névoas), `citadel` |
| Quest | `act1.spy_hunt`; opções `judge_suspects` (A → `judgment`) e `shelter_villagers` (B → `mercy`) |
| Efeito tardio | `first_return_to_citadel` |
| NPCs da Cidadela | `npc.blacksmith` (Bram), `npc.alchemist` (Lyra), `npc.elder` (Irian), `npc.gatekeeper` (Maera), `npc.merchant` (Sera) |
| Inimigos do slice | `ethereal_wolf`, `blind_cultist`, `moss_claws` (sub-boss); spawn `<zone>.<enemy>.<nn>` (D7) |
| Flags / recompensas | `forest_01.moss_claws.defeated`; `reward.forest_01.moss_claws.first_kill` (D1/T07) |
| Itens iniciais | `training_axe`, `worn_tunic`, `potion_health_small`, `flask_aether_small`, `antidote_flask`, `wolf_fang`, `cultist_ash`, `root_essence` (D8, `experimental-values.md`) |
| Prólogo | `prologue.last_bell`, `prologue.council` |

Ao criar um ID novo, registre-o nesta tabela no mesmo PR.

## 5. Convenção de nomes e locais

| Item | Padrão | Exemplo |
|---|---|---|
| Script | `snake_case.gd`, `class_name PascalCase` | `loot_service.gd` → `LootService` |
| Cena | `snake_case.tscn` ao lado do script de mesmo nome | `wolf.tscn` + `wolf.gd` (se houver script próprio) |
| Resource de conteúdo | `<id_sem_ponto>.tres` em `content/<tipo>/` | `content/items/small_potion.tres` |
| Teste | `tests/test_<modulo>.gd` ou `tests/integration/test_<fluxo>.gd` | `tests/test_inventory.gd` |
| Sinal | verbo no passado, payload com IDs | `item_added(item_id, amount)` |
| Grupo de nós | `snake_case` | `player`, `enemies` |

## 6. Estado de implementação (atualize ao concluir cada cartão)

Atualizado para `0.2.0-dev`. As suítes de teste estão distribuídas entre os módulos; o total validado é 16 suítes e 308 verificações, incluindo a arena técnica preservada da `main`.

| Módulo | Estado | Verificações | Cartão |
|---|---|---|---|
| Save/schema/backup | implementado | 27 + 9 migração | T06 |
| ActorBase + EnemyFSM | implementado | 25 | — |
| Player (movimento, dash, i-frames) | implementado | 19 | — |
| Ataques básico/pesado | implementado | 41 | T01A e T01 |
| Fórmula de dano, atributos, regeneração | implementado | 15 | T01A |
| Apresentação e sprites | implementado | 13 | T10 |
| Catálogo de conteúdo | implementado | 9 | T00 |
| Guarda e habilidades | implementado | 10 | T01 |
| Inimigos Lobo/Cultista | implementado | 9 | T02 |
| Mapas, zonas, retorno e morte | implementado | 37 integração | T03 |
| Garras de Musgo | implementado | 37 integração | T04 |
| Inventário e peso | implementado | 20 | T05 |
| Comércio no hub | implementado; Lyra/Sera vendem consumíveis | 14 + 37 integração | — |
| Save v2 e migração | implementado | 9 | T06 |
| Loot e XP atômicos | implementado | 26 | T07 |
| Quest `act1.spy_hunt` | implementado | 26 + 37 integração | T08 |
| Diálogos e NPCs | implementado | 15 UI | T09 |
| HUD, menus e áudio mínimo | implementado | 15 UI | T10 |
| Consequência local no hub | implementado | 37 integração | T11 |
| CI e export Windows | implementado; playtest humano pendente | 16 suítes + smoke | T12 |

## 7. Divergências entre documentos e como foram resolvidas

Resolvidas em [decisions-first-slice.md](decisions-first-slice.md). Resumo:

| Tema | Divergência | Resolução |
|---|---|---|
| Recurso das ativas do Bastião | GDD 01 fala em "vigor"; decisões de produção dizem MP | **MP** (D5); sem barra de vigor no slice |
| Pesado técnico (12 MP, dano 24, 800 ms) | Diverge do Impacto do Baluarte (P=18) | Vira Impacto: P=18, 12 MP, tempos mantidos (D3) |
| Dano fixo do protótipo (12/24) e HP 100 / MP 40 | Diverge do GDD 01 (HP 172 / MP 52, fórmula) | Fórmula e atributos do GDD adotados (D3, T01A) |
| Checkpoint válido | Schema só aceita 4 zonas, nenhuma é `forest_01` | Checkpoint único `citadel` (D1) |
| Slots de habilidade | 4 slots vs. 3 ativas do Bastião | `skill_4` reservada (D2) |
| Bruma/promoção | Módulo 05 previa +50% HP | Vale +25% HP/dano por ato, uma vez; fora do slice |
| Categoria de Garras de Musgo | GDD 04 diz "Sub-boss / boss" | **Sub-boss** (D7) |
