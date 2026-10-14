# T07 — Loot e XP atômicos e idempotentes

**Depende de:** T05, T06, T02 · **Decisões:** D7, D8 · **Valores:** `experimental-values.md` §5–6

## Objetivo

Concluir o ciclo recompensa: morte de inimigo → sorteio com RNG injetável → inventário/XP → save, sem duplicar nem perder, inclusive com bolsa cheia.

## Arquivos a criar/alterar

- `client/godot/content/loot/loot_table.gd` (`class_name LootTable`, `Resource`) + `.tres` do Bosque
- `client/godot/services/loot_service.gd` (`class_name LootService`, `RefCounted`)
- `client/godot/tests/test_loot.gd`

## Contrato

```gdscript
func roll(table: LootTable, rng: RandomNumberGenerator, source_id: StringName) -> Array[Dictionary]  # [{item_id, amount}]
func grant(source_id: StringName, drops: Array, inventory: InventoryModel, state: Dictionary) -> Dictionary
# grant: cria candidato completo (inventário + XP + granted_rewards), valida, grava via SaveSystem.commit_snapshot; só então confirma.
```
- `source_id` = ID de instância de encontro/spawn (não `enemy_id`): repetir o mesmo `source_id` não concede de novo.
- Recompensa única de campanha (boss/quest) usa `granted_rewards`.
- Bolsa cheia: itens comuns viram `overflow` (caem no chão ou descartam, regra registrada); itens de quest sempre entram.
- Tabelas: `experimental-values.md` §6 (de GDD 03 §3): categorias **Comum** e **Sub-boss** (D7). Raridade por pesos inteiros somando 10.000, só se a chance de equipamento passou. Equipamento = template fixo da raridade sorteada (D8, sem afixos).
- XP: `floor(base_xp × mult)`; curva `100 + 50×(L−1)`; ao subir de nível acumula 3 em `unspent_points` e recalcula HP/MP máx **sem curar** (`experimental-values.md` §5).
- Sub-boss `moss_claws`: recompensa única `reward.forest_01.moss_claws.first_kill` (flag `forest_01.moss_claws.defeated` na mesma gravação). Sem receita nem fragmento neste mapa.

## Critérios de aceite

- Mesmo `seed` produz mesmo resultado.
- Soma dos pesos de raridade = 10.000 em ambas as categorias; amostra de 10.000 rolagens com seed fixa respeita as proporções dentro de tolerância declarada no teste.
- Replay de `source_id` é no-op.
- Falha de gravação não altera inventário nem XP em memória.

## Testes obrigatórios

Determinismo por seed, idempotência, bolsa cheia, quest item, falha de save, XP sem overflow, recompensa única do boss não repete.

## Fora de escopo

UI de loot, comércio, crafting, drop no chão visual.

## Leia antes

`docs/GDD/03_sistema_de_loot.md`; `docs/design/loot-baseline.md`; `architecture.md` invariantes 3–4.
