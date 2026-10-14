# T02 — Lobo Etéreo e Cultista Cego-Guiado

**Depende de:** T00, T01A · **Decisões:** D7 · **Valores:** `experimental-values.md` §3

## Objetivo

Criar os dois inimigos comuns do Bosque como cenas reutilizáveis baseadas em `ActorBase` + `EnemyFSM`, parametrizadas por Resource.

## Arquivos a criar/alterar

- `client/godot/content/enemies/enemy_definition.gd` (`class_name EnemyDefinition`, `Resource`)
- `client/godot/content/enemies/ethereal_wolf.tres`, `blind_cultist.tres`
- `client/godot/entities/enemies/enemy_actor.gd` (`class_name EnemyActor extends ActorBase`, aplica o `EnemyDefinition` no `_ready`)
- `client/godot/entities/enemies/ethereal_wolf.tscn`, `blind_cultist.tscn`
- `client/godot/tests/test_enemies.gd`
- Estender `entities/ai/enemy_fsm.gd` **somente** se necessário (ex.: investida), mantendo `test_enemy_fsm.gd` verde.

## Contrato

`EnemyDefinition` exporta: `enemy_id: StringName`, `category: StringName` (`common|elite|sub_boss|boss`), `max_health`, `defense`, `attack_power`, `damage_type: StringName` (`physical|arcane`), `base_xp`, `movement_speed`, `detection_range`, `attack_range`, `attack_cooldown`, `windup_duration`, `recovery_duration`, `tags` e `valid() -> bool`.

Telegraphs do GDD (`docs/design/prologue-and-act-one.md` Ato I):

| Inimigo | Antecipação | Recuperação | Restrição |
|---|---|---|---|
| Lobo Etéreo | uivo e faixa de investida, 0,7 s | 0,9 s após choque | não atravessa paredes |
| Cultista Cego-Guiado | selo circular de lentidão 1 s; projétil visível | 1,2 s | não lança através de obstáculos |

- IDs: `ethereal_wolf`, `blind_cultist` (D7). Spawn: `forest_01.<enemy_id>.<nn>`.
- HP, DEF, P, velocidade, detecção e alcance: use a tabela de `experimental-values.md` §3 (EXPERIMENTAL). Dano ao jogador via `DamageCalculator.mitigate` com DEF 9 (físico) / MDEF 2 (arcano).
- Categoria de loot/XP: **Comum**.
- Hub e faixa inicial de saída **nunca** geram elites.
- Registre `enemy_id` novos em `project-structure.md` §4.

## Critérios de aceite

- Cada cena instancia com `actor_id` único, `CollisionShape2D` e `EnemyFSM` filho.
- Investida do Lobo respeita parede (máscara 2).
- Projétil do Cultista é bloqueado por obstáculo.
- Inimigo na zona `citadel` não causa nem recebe dano.

## Testes obrigatórios

Parâmetros inválidos recusados (`valid()`), antecipação cumpre duração, parede bloqueia investida/projétil, morte idempotente, zona segura.

## Fora de escopo

Elites, Espectros de Folhas, Bruma/corrupção, loot (T07), navegação/pathfinding.

## Leia antes

`docs/GDD/09_enemy_fsm.md`; `docs/design/prologue-and-act-one.md`; `production-decisions.md` §3.
