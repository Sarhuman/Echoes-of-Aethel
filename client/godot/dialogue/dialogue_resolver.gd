class_name DialogueResolver
extends RefCounted
static func resolve(id: String, flags: Dictionary) -> Dictionary:
	var definition: DialogueDefinition = load("res://content/dialogues/hub_first_slice.tres")
	var selected: Dictionary = {}
	var priority := -1
	for line in definition.lines:
		if line.npc_id == id and available(line.requires,flags) and line.requires.size() > priority:
			selected = line.duplicate(true)
			priority = line.requires.size()
	return selected
static func available(requires: Array, flags: Dictionary) -> bool:
	for flag in requires:
		if not flag is String or not flags.get(flag,false): return false
	return true