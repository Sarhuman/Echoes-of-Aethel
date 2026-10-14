# T05 — Inventário com capacidade e peso

**Depende de:** T00, T01A · **Decisões:** D5, D8 · **Valores:** `experimental-values.md` §7–8

## Objetivo

Domínio puro de inventário: 60 posições, peso máximo, ouro separado, equipamento (`main_hand` e `chest`, sem afixos) e consumíveis com cooldown, sem perder item de quest por bolsa cheia.

## Arquivos a criar/alterar

- `client/godot/content/items/item_definition.gd` (`class_name ItemDefinition`, `Resource`: `item_id`, `display_name`, `kind` (`consumable|equipment|material|quest`), `weight`, `stackable`, `max_stack`)
- `client/godot/inventory/inventory_model.gd` (`class_name InventoryModel`, `RefCounted`)
- `client/godot/content/items/` itens iniciais (poção pequena, dente de lobo, essência de raiz — nomes do GDD; números `EXPERIMENTAL`)
- `client/godot/inventory/equipment_state.gd` (`class_name EquipmentState`, `RefCounted`)
- `client/godot/inventory/consumable_service.gd` (`class_name ConsumableService`, `RefCounted`)
- `client/godot/content/items/` também: `training_axe`, `worn_tunic` e os 8 templates de equipamento
- `client/godot/tests/test_inventory.gd`

## Contrato

```gdscript
class_name InventoryModel
extends RefCounted
func can_add(item_id: StringName, amount: int) -> bool
func add(item_id: StringName, amount: int) -> Dictionary   # {ok, added, overflow}; atômico
func remove(item_id: StringName, amount: int) -> bool
func total_weight() -> float
func to_dict() -> Dictionary      # somente tipos primitivos (para o save v2)
static func from_dict(data: Dictionary, catalog: ContentCatalog) -> InventoryModel
```
- Item `kind == quest` **ignora** limites de peso/capacidade (nunca se perde).
- `add` é tudo-ou-nada: falha não altera estado.
- Capacidade **60 posições**, stack 20, peso máx. **60,0**, ouro em campo separado (`gold: int`, sem peso): valores de `experimental-values.md` §7, lidos de um único `Resource`.
- Equipamento ocupa 1 posição; item equipado continua na bolsa (referência por ID). `EquipmentState.equip(slot, item_id)` valida slot, requisito de nível e recalcula derivados **sem curar**.
- `ConsumableService.use(item_id, hp, hp_max, mp, mp_max, now) -> Dictionary`: Poção de Vida +50 HP, Éter +30 MP, Antídoto remove veneno removível; cooldown compartilhado vida/MP 5 s, antídoto 5 s próprio; recurso cheio/efeito inexistente **não consome** nem inicia cooldown.
- `to_dict()` inclui `gold` e `equipped`.

## Critérios de aceite

- Adicionar além do limite devolve `overflow` sem corromper.
- Item de quest sempre entra e tem peso 0.
- Equipar `iron_axe` muda `W` de 8 para 9 no cálculo de dano; desequipar recalcula.
- `to_dict/from_dict` faz round-trip idêntico.

## Testes obrigatórios

Limites exatos (60 posições, 60,0 de peso, stack 20), remoção maior que o saldo, quantidade ≤ 0 ou NaN, item de quest com bolsa cheia, round-trip, equipar/desequipar, consumo com recurso cheio, cooldown de 5 s.

## Fora de escopo

UI do inventário (T10), loot (T07), crafting, comércio, integração com save (T06).

## Leia antes

`docs/GDD/03_sistema_de_loot.md`; `docs/design/loot-baseline.md`; `project-scope.md` §3.
