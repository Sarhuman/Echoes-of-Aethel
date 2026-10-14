class_name QuestService
extends RefCounted
var definition: QuestDefinition = preload("res://content/quests/act1_spy_hunt.tres")
func available(state: Dictionary) -> bool:
	for clue in definition.clues:
		if not state.world_flags.get(clue, false): return false
	return true
func choose(quest_id: String, option_id: String, state: Dictionary, commit: Callable) -> Dictionary:
	if quest_id != definition.quest_id or option_id not in definition.options: return {"ok": false, "code": "INVALID_CHOICE"}
	if state.quest_choices.has(quest_id):
		return {"ok": true, "replayed": true} if state.quest_choices[quest_id] == option_id else {"ok": false, "code": "QUEST_ALREADY_DECIDED"}
	if not available(state): return {"ok": false, "code": "INVESTIGATE_FIRST"}
	var candidate := state.duplicate(true)
	candidate.quest_choices[quest_id] = option_id
	candidate.progress.ritual_charge += 1
	candidate.pending_effects["first_return_to_citadel"] = true
	candidate.world_flags[option_id] = true
	var inv := InventoryModel.from_dict(candidate.inventory)
	if option_id == "judge_suspects":
		inv.gold += definition.judgment_gold
	else:
		if not inv.add(&"potion_health_small", definition.mercy_potions).ok:
			candidate.pending_loot["potion_health_small"] = candidate.pending_loot.get("potion_health_small", 0) + definition.mercy_potions
	candidate.inventory = inv.to_dict()
	return commit.call(candidate, "quest_choice")
