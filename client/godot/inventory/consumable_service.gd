class_name ConsumableService
extends RefCounted
var next_use := 0.0
var next_antidote := 0.0
func use_item(inventory: InventoryModel, id: StringName, hp: int, hp_max: int, mp: float, mp_max: float, now: float, poisoned: bool = false) -> Dictionary:
	var spec := inventory.item(id)
	var refused := {"ok": false, "hp": hp, "mp": mp, "poisoned": poisoned}
	if spec == null or spec.kind != &"consumable" or not is_finite(now) or now < 0 or inventory.counts.get(String(id), 0) <= 0: return refused
	var anti := id == &"antidote_flask"
	if now < (next_antidote if anti else next_use): return refused
	if id == &"potion_health_small":
		if hp >= hp_max: return refused
		hp = mini(hp_max, hp + spec.restore)
	elif id == &"flask_aether_small":
		if mp >= mp_max: return refused
		mp = minf(mp_max, mp + spec.restore)
	elif anti:
		if not poisoned: return refused
		poisoned = false
	else: return refused
	if not inventory.remove(id, 1): return refused
	if anti: next_antidote = now + spec.cooldown
	else: next_use = now + spec.cooldown
	return {"ok": true, "hp": hp, "mp": mp, "poisoned": poisoned}
