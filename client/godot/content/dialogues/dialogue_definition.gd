class_name DialogueDefinition
extends Resource
@export var dialogue_id := "hub.first_slice"
@export var lines: Array[Dictionary] = []
func valid() -> bool:
	var ids: Dictionary = {}
	for line in lines:
		for key in ["line_id","npc_id","speaker","text","requires"]:
			if not line.has(key): return false
		if not WorldSaveSchema.valid_id(line.line_id) or ids.has(line.line_id) or not WorldSaveSchema.valid_id(line.npc_id): return false
		if not line.requires is Array or not line.text is String or not line.speaker is String: return false
		for flag in line.requires:
			if not WorldSaveSchema.valid_id(flag): return false
		ids[line.line_id] = true
	return not lines.is_empty()
