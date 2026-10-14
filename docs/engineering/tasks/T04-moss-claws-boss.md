# T04 — Boss Garras de Musgo

**Depende de:** T02, T03 · **Decisões:** D1, D7 · **Valores:** `experimental-values.md` §4

## Objetivo

Implementar o encontro de `forest_01` com antecipações visuais evitáveis e repetição possível após derrota.

## Arquivos a criar/alterar

- `client/godot/content/enemies/moss_claws.tres` (`EnemyDefinition` ou extensão `BossDefinition`)
- `client/godot/entities/enemies/moss_claws.gd` + `.tscn`
- `client/godot/world/maps/forest_arena.tscn` (pilares como obstáculos)
- `client/godot/tests/test_boss.gd`

## Contrato

Ataques (GDD): varredura em arco, antecipação 0,9 s; investida reta, 1,1 s, recuperação 1 s, **pilares bloqueiam a investida**; abaixo de 50% HP, raízes em duas áreas alternadas, 1,2 s, **mantendo corredor livre**, sem reduzir telegraphs.
- `enemy_id = moss_claws`; categoria **sub-boss** (D7). HP 800, DEF 15; ataques e danos em `experimental-values.md` §4 (EXPERIMENTAL).
- Boss não sofre stun (A3 do Bastião).
- Ao morrer: `world_flags["forest_01.moss_claws.defeated"] = true` na mesma gravação da recompensa (T07).
- Derrotar o boss novamente **não** concede recompensa única de novo (invariante 4 em `architecture.md`).
- Onda de choque: **não implementar** (não definida no GDD).

## Critérios de aceite

- Cada ataque tem antecipação visível com a duração do GDD e é esquivável pelo dash (stamina 25).
- Pilar bloqueia investida; corredor livre sempre existe nas raízes.
- Morrer e repetir o encontro funciona.

## Testes obrigatórios

Duração de antecipação por ataque, bloqueio por pilar, fase <50%, corredor livre, morte do boss idempotente, retry.

## Fora de escopo

Loot do boss (T07), música, cutscene.

## Leia antes

`docs/design/prologue-and-act-one.md`; `docs/engineering/offline-combat.md` (limites da amostragem discreta).
