class_name LootService
extends RefCounted
const BOSS_REWARD := "reward.forest_01.moss_claws.first_kill"
static func roll(table: LootTable, rng: RandomNumberGenerator, enemy: StringName) -> Dictionary:
	var drops: Dictionary = {}
	if rng.randi_range(1, 100) <= table.equipment_chance:
		var n := rng.randi_range(1, 10000)
		var rarity := 0
		for weight in table.rarity_weights:
			if n <= weight: break
			n -= weight
			rarity += 1
		var weapons := ["iron_axe", "runed_axe", "bastion_axe", "twilight_axe"]
		var armors := ["padded_vest", "ironweave_cuirass", "bastion_cuirass", "twilight_cuirass"]
		drops[(weapons if rng.randi_range(0, 1) == 0 else armors)[mini(rarity, 3)]] = 1
	if rng.randi_range(1, 100) <= table.consumable_chance:
		var n := rng.randi_range(1, 100)
		drops["potion_health_small" if n <= 50 else ("flask_aether_small" if n <= 80 else "antidote_flask")] = 1
	if rng.randi_range(1, 100) <= table.material_chance:
		drops["cultist_ash" if enemy == &"blind_cultist" else "wolf_fang"] = 1
	if rng.randi_range(1, 100) <= table.essence_chance: drops["root_essence"] = 1
	var gold := rng.randi_range(table.gold_min, table.gold_max) if rng.randi_range(1, 100) <= table.gold_chance else 0
	return {"items": drops, "gold": gold}
static func grant(source_id: String, spec: EnemyDefinition, state: Dictionary, commit: Callable) -> Dictionary:
	if not WorldSaveSchema.valid_id(source_id) or spec == null or not spec.valid(): return {"ok": false, "code": "INVALID_REWARD"}
	var boss := spec.category == &"sub_boss"
	if state.encounters.kills.has(source_id) or (boss and state.granted_rewards.has(BOSS_REWARD)):
		return {"ok": true, "replayed": true}
	var candidate := state.duplicate(true)
	var rng := RandomNumberGenerator.new()
	rng.seed = source_id.hash()
	var table: LootTable = load("res://content/loot/sub_boss.tres" if boss else "res://content/loot/common.tres")
	var reward := roll(table, rng, spec.enemy_id)
	var inv := InventoryModel.from_dict(candidate.inventory)
	if inv == null: return {"ok": false, "code": "INVALID_INVENTORY"}
	for id in reward.items:
		if not inv.add(id, reward.items[id]).ok:
			candidate.pending_loot[id] = candidate.pending_loot.get(id, 0) + reward.items[id]
	inv.gold += reward.gold
	candidate.inventory = inv.to_dict()
	candidate.character.xp += spec.base_xp * table.xp_multiplier
	var config := inv.config
	while candidate.character.level < config.max_level:
		var needed: int = config.xp_initial + config.xp_step * (candidate.character.level - 1)
		if candidate.character.xp < needed: break
		candidate.character.xp -= needed
		candidate.character.level += 1
		candidate.character.unspent_points += config.points_per_level
	candidate.encounters.kills[source_id] = true
	if boss:
		candidate.granted_rewards[BOSS_REWARD] = true
		candidate.world_flags["forest_01.moss_claws.defeated"] = true
	var result: Dictionary = commit.call(candidate, "loot")
	if result.ok: result["reward"] = reward
	return result
