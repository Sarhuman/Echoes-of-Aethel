extends Node

# Minimal offline world state; economy/character/encounter saves are not implemented.
var world_flags: Dictionary = {}
var npc_states: Dictionary = {}
var faction_reputation: Dictionary = {}
var quest_choices: Dictionary = {}
var pending_effects: Dictionary = {}
var applied_effects: Dictionary = {}
var progress: Dictionary = WorldSaveSchema.initial()["progress"]
var character: Dictionary = WorldSaveSchema.initial()["character"]
var inventory: Dictionary = WorldSaveSchema.initial()["inventory"]
var granted_rewards: Dictionary = {}
var pending_loot: Dictionary = {}
var encounters: Dictionary = WorldSaveSchema.initial()["encounters"]

func snapshot() -> Dictionary:
	return {
		"world_flags": world_flags.duplicate(true), "npc_states": npc_states.duplicate(true),
		"faction_reputation": faction_reputation.duplicate(true), "quest_choices": quest_choices.duplicate(true),
		"pending_effects": pending_effects.duplicate(true), "applied_effects": applied_effects.duplicate(true),
		"progress": progress.duplicate(true), "character": character.duplicate(true),
		"inventory": inventory.duplicate(true), "granted_rewards": granted_rewards.duplicate(true),
		"pending_loot": pending_loot.duplicate(true), "encounters": encounters.duplicate(true)
	}

func replace_validated(data: Dictionary, notify: bool = true) -> bool:
	if not WorldSaveSchema.validate(data).is_empty():
		return false
	var normalized := WorldSaveSchema.normalize(data)
	world_flags = normalized["world_flags"]
	npc_states = normalized["npc_states"]
	faction_reputation = normalized["faction_reputation"]
	quest_choices = normalized["quest_choices"]
	pending_effects = normalized["pending_effects"]
	applied_effects = normalized["applied_effects"]
	progress = normalized["progress"]
	character = normalized.character
	inventory = normalized.inventory
	granted_rewards = normalized.granted_rewards
	pending_loot = normalized.pending_loot
	encounters = normalized.encounters
	if notify:
		EventBus.world_restored.emit()
	return true

func get_npc_state(npc_id: StringName) -> StringName:
	return StringName(npc_states.get(String(npc_id), "alive"))
