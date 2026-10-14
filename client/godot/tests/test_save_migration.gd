extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func finish() -> void:
	print("%d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
func run() -> void:
	var old := WorldSaveSchema.initial()
	for field in ["character","inventory","pending_loot","granted_rewards","encounters"]: old.erase(field)
	old.world_flags["old_fact"] = true
	var migrated := SaveMigrations.migrate(old,"world-save-1")
	check(WorldSaveSchema.validate(migrated).is_empty(),"v1 migrates to valid v2")
	check(migrated.world_flags.old_fact and migrated.character.hp == 172,"preserves narrative and creates character")
	check(SaveMigrations.migrate(old,"world-save-99").is_empty(),"unknown future refused")
	var invalid := migrated.duplicate(true)
	invalid.character.mp = INF
	check(WorldSaveSchema.validate(invalid) == "INVALID_CHARACTER","nonfinite MP rejected")
	invalid = migrated.duplicate(true)
	invalid.character.hp = 173
	check(WorldSaveSchema.validate(invalid) == "INVALID_CHARACTER","HP maximum checked")
	var saves := root.get_node("SaveSystem")
	for suffix in ["",".bak",".tmp",".corrupt"]: DirAccess.remove_absolute(saves.SAVE_PATH + suffix)
	var payload := JSON.stringify(old)
	var file := FileAccess.open(saves.SAVE_PATH,FileAccess.WRITE)
	file.store_string(JSON.stringify({"schema_version":1,"content_version":"world-save-1","payload_json":payload,"sha256":payload.sha256_text()}))
	file.close()
	check(saves.load_game().ok,"actual legacy envelope loads")
	check(saves.save_game().ok,"writes v2 after migration")
	var backup: Dictionary = JSON.parse_string(FileAccess.get_file_as_string(saves.SAVE_PATH+".bak"))
	check(backup.content_version == "world-save-1","original retained in backup")
	var current: Dictionary = JSON.parse_string(FileAccess.get_file_as_string(saves.SAVE_PATH))
	check(current.content_version == "world-save-2","new envelope version")
	for suffix in ["",".bak",".tmp",".corrupt"]: DirAccess.remove_absolute(saves.SAVE_PATH + suffix)
	finish()
