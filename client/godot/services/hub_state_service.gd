class_name HubStateService
extends RefCounted
static func stage(flags: Dictionary) -> int:
	if flags.get("map11.completed", false): return 4
	if flags.get("map09.completed", false): return 3
	if flags.get("map06.completed", false): return 2
	if flags.get("map03.completed", false): return 1
	return 0
static func apply_return(candidate: Dictionary) -> void:
	if candidate.pending_effects.has("first_return_to_citadel"):
		var choice: String = candidate.quest_choices.get("act1.spy_hunt", "")
		if choice == "shelter_villagers":
			candidate.world_flags["hub.shelter_observed"] = true
			candidate.world_flags["hub.contamination_observed"] = true
		elif choice == "judge_suspects":
			candidate.world_flags["hub.memorial_observed"] = true
		candidate.pending_effects.erase("first_return_to_citadel")
		candidate.applied_effects["first_return_to_citadel"] = true
	candidate.progress.hub_stage = stage(candidate.world_flags)
