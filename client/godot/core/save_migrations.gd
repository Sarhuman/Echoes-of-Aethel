class_name SaveMigrations
extends RefCounted
static func migrate(payload: Dictionary, version: String) -> Dictionary:
	if version == "world-save-2": return payload.duplicate(true)
	if version != "world-save-1" or payload.size() != 7: return {}
	var result := payload.duplicate(true)
	var defaults := WorldSaveSchema.initial()
	for field in ["character", "inventory", "granted_rewards", "pending_loot", "encounters"]:
		result[field] = defaults[field]
	return result
