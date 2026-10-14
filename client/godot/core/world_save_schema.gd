class_name WorldSaveSchema
extends RefCounted

const CONTENT_VERSION := "world-save-2"
const MAX_ENTRIES := 2048
const PROGRESS_KEYS := ["act", "map", "hub_stage", "checkpoint", "ritual_charge"]
const PAYLOAD_KEYS := ["world_flags", "npc_states", "faction_reputation", "quest_choices", "pending_effects", "applied_effects", "progress", "character", "inventory", "granted_rewards", "pending_loot", "encounters"]
const ZONES := ["citadel", "dunes_camp", "underground_refuge", "liminal_post"]

static func initial() -> Dictionary:
	var stats := CharacterStats.new()
	return {
		"world_flags": {}, "npc_states": {}, "faction_reputation": {},
		"quest_choices": {}, "pending_effects": {}, "applied_effects": {},
		"progress": {"act": 1, "map": 1, "hub_stage": 0, "checkpoint": "citadel", "ritual_charge": 0},
		"character": {"hp": stats.hp_max(), "mp": float(stats.mp_max()), "xp": 0, "level": 1, "unspent_points": 0},
		"inventory": {"items": stats.definition.starter_items.duplicate(true), "gold": 0, "equipped": {"main_hand": "training_axe", "chest": "worn_tunic"}},
		"granted_rewards": {}, "pending_loot": {}, "encounters": {"expedition": 0, "kills": {}}
	}

static func valid_id(value: Variant) -> bool:
	if not value is String or value.is_empty() or value.length() > 128:
		return false
	var regex := RegEx.new()
	regex.compile("^[a-zA-Z0-9_.:-]+$")
	return regex.search(value) != null

static func integer_between(value: Variant, low: int, high: int) -> bool:
	if not (value is int or value is float):
		return false
	return is_finite(float(value)) and float(value) == floor(float(value)) and value >= low and value <= high

static func validate(data: Variant) -> String:
	if not data is Dictionary or data.size() != PAYLOAD_KEYS.size():
		return "INVALID_PAYLOAD"
	for key in PAYLOAD_KEYS:
		if not data.has(key) or not data[key] is Dictionary:
			return "INVALID_FIELD"
		if data[key].size() > MAX_ENTRIES:
			return "TOO_MANY_ENTRIES"
	for key in data["world_flags"]:
		if not valid_id(key) or not data["world_flags"][key] is bool:
			return "INVALID_FLAG"
	for key in data["npc_states"]:
		if not valid_id(key) or data["npc_states"][key] not in ["alive", "dead", "hostile"]:
			return "INVALID_NPC"
	for key in data["faction_reputation"]:
		if not valid_id(key) or not integer_between(data["faction_reputation"][key], -100, 100):
			return "INVALID_REPUTATION"
	for key in data["quest_choices"]:
		if not valid_id(key) or not valid_id(data["quest_choices"][key]):
			return "INVALID_CHOICE"
	for field in ["pending_effects", "applied_effects"]:
		for key in data[field]:
			if not valid_id(key) or data[field][key] != true or not data[field][key] is bool:
				return "INVALID_EFFECT"
	for key in data["pending_effects"]:
		if data["applied_effects"].has(key):
			return "EFFECT_ALREADY_APPLIED"
	var progress: Dictionary = data["progress"]
	if progress.size() != PROGRESS_KEYS.size():
		return "INVALID_PROGRESS"
	for key in PROGRESS_KEYS:
		if not progress.has(key):
			return "INVALID_PROGRESS"
	if not integer_between(progress["map"], 1, 12) or not integer_between(progress["act"], 1, 4):
		return "INVALID_MAP"
	if int(progress["act"]) != int((int(progress["map"]) - 1) / 3) + 1:
		return "ACT_MAP_MISMATCH"
	if not integer_between(progress["hub_stage"], 0, 4) or not integer_between(progress["ritual_charge"], 0, 12):
		return "INVALID_PROGRESS"
	if progress["checkpoint"] not in ZONES:
		return "INVALID_CHECKPOINT"
	if int(progress["ritual_charge"]) != data["quest_choices"].size():
		return "CHOICE_CHARGE_MISMATCH"
	var character: Dictionary = data.character
	if character.size() != 5: return "INVALID_CHARACTER"
	for key in ["hp", "mp", "xp", "level", "unspent_points"]:
		if not character.has(key): return "INVALID_CHARACTER"
	if not integer_between(character.level, 1, 30) or not integer_between(character.xp, 0, 100000000) or not integer_between(character.unspent_points, 0, 87): return "INVALID_CHARACTER"
	var stats := CharacterStats.new()
	stats.level = int(character.level)
	if not integer_between(character.hp, 0, stats.hp_max()) or not (character.mp is float or character.mp is int) or not is_finite(character.mp) or character.mp < 0 or character.mp > stats.mp_max(): return "INVALID_CHARACTER"
	if InventoryModel.from_dict(data.inventory) == null: return "INVALID_INVENTORY"
	var inventory := InventoryModel.new()
	for id in data.pending_loot:
		if not valid_id(id) or inventory.item(id) == null or not integer_between(data.pending_loot[id], 1, 1000000): return "INVALID_PENDING_LOOT"
	for id in data.granted_rewards:
		if not valid_id(id) or not data.granted_rewards[id] is bool or data.granted_rewards[id] != true: return "INVALID_REWARD"
	if data.encounters.size() != 2 or not integer_between(data.encounters.get("expedition"), 0, 100000000) or not data.encounters.get("kills") is Dictionary or data.encounters.kills.size() > MAX_ENTRIES: return "INVALID_ENCOUNTERS"
	for id in data.encounters.kills:
		if not valid_id(id) or not data.encounters.kills[id] is bool or data.encounters.kills[id] != true: return "INVALID_ENCOUNTER"
	return ""

static func normalize(data: Dictionary) -> Dictionary:
	var result := data.duplicate(true)
	for key in ["act", "map", "hub_stage", "ritual_charge"]:
		result["progress"][key] = int(result["progress"][key])
	for key in result["faction_reputation"]:
		result["faction_reputation"][key] = int(result["faction_reputation"][key])
	for key in ["hp", "xp", "level", "unspent_points"]:
		result.character[key] = int(result.character[key])
	result.character.mp = float(result.character.mp)
	result.inventory.gold = int(result.inventory.gold)
	for key in result.inventory.items: result.inventory.items[key] = int(result.inventory.items[key])
	for key in result.pending_loot: result.pending_loot[key] = int(result.pending_loot[key])
	result.encounters.expedition = int(result.encounters.expedition)
	return result
