extends Node

const SCHEMA_VERSION := 1
const MAX_BYTES := 1024 * 1024
var SAVE_PATH := "user://tests/savegame.json" if "--test-harness" in OS.get_cmdline_user_args() else "user://savegame.json"
var _busy := false
var legacy_path := OS.get_user_data_dir().get_base_dir().path_join("Echoes of Aethel - Persistence Harness/savegame.json")

func has_campaign() -> bool:
	return FileAccess.file_exists(SAVE_PATH) or ("--test-harness" not in OS.get_cmdline_user_args() and FileAccess.file_exists(legacy_path))

func _ready() -> void:
	DirAccess.make_dir_recursive_absolute(SAVE_PATH.get_base_dir())
	EventBus.checkpoint_reached.connect(_on_checkpoint_reached)

func _error(code: String) -> Dictionary:
	return {"ok": false, "code": code}

func _read(path: String) -> Dictionary:
	if not FileAccess.file_exists(path):
		return _error("NOT_FOUND")
	var file := FileAccess.open(path, FileAccess.READ)
	if file == null:
		return _error("READ_FAILED")
	if file.get_length() > MAX_BYTES:
		file.close()
		return _error("FILE_TOO_LARGE")
	var text := file.get_as_text()
	file.close()
	var parser := JSON.new()
	if parser.parse(text) != OK or not parser.data is Dictionary:
		return _error("INVALID_JSON")
	var envelope: Dictionary = parser.data
	if not envelope.has("schema_version") or envelope["schema_version"] != SCHEMA_VERSION:
		return _error("UNSUPPORTED_SCHEMA")
	if envelope.get("content_version") not in [WorldSaveSchema.CONTENT_VERSION, "world-save-1"]:
		return _error("UNSUPPORTED_CONTENT")
	if envelope.size() != 4 or not envelope.get("payload_json") is String or not envelope.get("sha256") is String:
		return _error("INVALID_ENVELOPE")
	var payload_text: String = envelope["payload_json"]
	if payload_text.sha256_text() != envelope["sha256"]:
		return _error("CHECKSUM_MISMATCH")
	var payload_parser := JSON.new()
	if payload_parser.parse(payload_text) != OK:
		return _error("INVALID_PAYLOAD_JSON")
	if not payload_parser.data is Dictionary:
		return _error("INVALID_PAYLOAD")
	var migrated := SaveMigrations.migrate(payload_parser.data, envelope.content_version)
	var problem := WorldSaveSchema.validate(migrated)
	if not problem.is_empty():
		return _error(problem)
	return {"ok": true, "payload": WorldSaveSchema.normalize(migrated), "source": path, "migrated": envelope.content_version == "world-save-1"}

func read_snapshot() -> Dictionary:
	var primary := _read(SAVE_PATH)
	if primary["ok"]:
		return primary
	# Never silently downgrade a future/incompatible save to an older backup.
	if primary["code"] in ["UNSUPPORTED_SCHEMA", "UNSUPPORTED_CONTENT"]:
		return primary
	var backup := _read(SAVE_PATH + ".bak")
	if backup["ok"]:
		backup["recovered"] = true
		backup["primary_error"] = primary["code"]
		return backup
	if primary["code"] == "NOT_FOUND" and "--test-harness" not in OS.get_cmdline_user_args():
		var legacy := _read(legacy_path)
		if legacy["ok"]:
			legacy["legacy_location"] = true
			return legacy
	return primary

func load_game() -> Dictionary:
	if _busy:
		return _error("BUSY")
	var result := read_snapshot()
	if result["ok"]:
		GameState.replace_validated(result["payload"])
	return result

func save_game(reason: String = "manual") -> Dictionary:
	return commit_snapshot(GameState.snapshot(), reason)

func commit_snapshot(candidate: Dictionary, reason: String = "manual") -> Dictionary:
	if _busy:
		return _error("BUSY")
	_busy = true
	var result := _write_snapshot(candidate)
	_busy = false
	if result["ok"]:
		GameState.replace_validated(candidate, false)
		EventBus.save_completed.emit(reason)
	else:
		EventBus.save_failed.emit(result["code"])
	return result

func _write_snapshot(candidate: Dictionary) -> Dictionary:
	var problem := WorldSaveSchema.validate(candidate)
	if not problem.is_empty():
		return _error(problem)
	var payload_text := JSON.stringify(WorldSaveSchema.normalize(candidate))
	var envelope := {"schema_version": SCHEMA_VERSION, "content_version": WorldSaveSchema.CONTENT_VERSION,
		"payload_json": payload_text, "sha256": payload_text.sha256_text()}
	var text := JSON.stringify(envelope)
	if text.to_utf8_buffer().size() > MAX_BYTES:
		return _error("FILE_TOO_LARGE")
	# Preserve an invalid/incompatible primary and its backup for explicit recovery.
	if FileAccess.file_exists(SAVE_PATH):
		var previous := _read(SAVE_PATH)
		if not previous["ok"]:
			return _error("PRIMARY_REQUIRES_REPAIR")
	var temporary := SAVE_PATH + ".tmp"
	var file := FileAccess.open(temporary, FileAccess.WRITE)
	if file == null:
		return _error("WRITE_FAILED")
	file.store_string(text)
	file.flush()
	var write_error := file.get_error()
	file.close()
	if write_error != OK or not _read(temporary)["ok"]:
		return _error("TEMP_VERIFICATION_FAILED")
	if FileAccess.file_exists(SAVE_PATH):
		var backup := SAVE_PATH + ".bak"
		if FileAccess.file_exists(backup) and DirAccess.remove_absolute(backup) != OK:
			return _error("BACKUP_REMOVE_FAILED")
		if DirAccess.rename_absolute(SAVE_PATH, backup) != OK:
			return _error("BACKUP_ROTATION_FAILED")
	if DirAccess.rename_absolute(temporary, SAVE_PATH) != OK:
		return _error("COMMIT_RENAME_FAILED")
	return {"ok": true, "source": SAVE_PATH}

func repair_primary_from_backup() -> Dictionary:
	if _busy:
		return _error("BUSY")
	var primary := _read(SAVE_PATH)
	if primary["ok"] or primary["code"] in ["UNSUPPORTED_SCHEMA", "UNSUPPORTED_CONTENT"]:
		return _error("REPAIR_NOT_ALLOWED")
	var backup := _read(SAVE_PATH + ".bak")
	if not backup["ok"]:
		return _error("NO_VALID_BACKUP")
	# Keep the damaged original. Refuse to overwrite an existing quarantine file.
	if FileAccess.file_exists(SAVE_PATH):
		if FileAccess.file_exists(SAVE_PATH + ".corrupt"):
			return _error("QUARANTINE_EXISTS")
		if DirAccess.rename_absolute(SAVE_PATH, SAVE_PATH + ".corrupt") != OK:
			return _error("QUARANTINE_FAILED")
	return commit_snapshot(backup["payload"], "recovery")

func commit_quest_choice(quest_id: String, option_id: String, flag_updates: Dictionary,
		pending_ids: Array[String] = []) -> Dictionary:
	# Offline application boundary. Catalog/availability checks belong to QuestService.
	if not WorldSaveSchema.valid_id(quest_id) or not WorldSaveSchema.valid_id(option_id):
		return _error("INVALID_CHOICE")
	var candidate := GameState.snapshot()
	if candidate["quest_choices"].has(quest_id):
		if candidate["quest_choices"][quest_id] == option_id:
			return {"ok": true, "replayed": true}
		return _error("QUEST_ALREADY_DECIDED")
	candidate["quest_choices"][quest_id] = option_id
	candidate["progress"]["ritual_charge"] += 1
	for key in flag_updates:
		candidate["world_flags"][key] = flag_updates[key]
	for effect_id in pending_ids:
		candidate["pending_effects"][effect_id] = true
	var result := commit_snapshot(candidate, "quest_choice")
	if result["ok"]:
		EventBus.quest_choice_applied.emit(quest_id, option_id)
	return result

func _on_checkpoint_reached(zone_id: String, map_index: int) -> void:
	var candidate := GameState.snapshot()
	candidate["progress"]["checkpoint"] = zone_id
	candidate["progress"]["map"] = map_index
	candidate["progress"]["act"] = int((map_index - 1) / 3) + 1
	var result := commit_snapshot(candidate, "checkpoint")
	if result["ok"]:
		EventBus.world_restored.emit()
