extends SceneTree

var failures := 0
var checks := 0

func _initialize() -> void:
	call_deferred("run")

func check(condition: bool, label: String) -> void:
	checks += 1
	if not condition:
		failures += 1
		push_error(label)

func write_text(path: String, text: String) -> void:
	var file := FileAccess.open(path, FileAccess.WRITE)
	file.store_string(text)
	file.close()

func clean() -> void:
	for suffix in ["", ".bak", ".tmp", ".corrupt"]:
		DirAccess.remove_absolute("user://tests/savegame.json" + suffix)

func run() -> void:
	clean()
	var state := root.get_node("GameState")
	var saves := root.get_node("SaveSystem")
	var bus := root.get_node("EventBus")
	state.replace_validated(WorldSaveSchema.initial(), false)
	check(saves.load_game()["code"] == "NOT_FOUND", "missing save must be explicit")
	check(saves.save_game()["ok"], "initial save")
	var effects: Array[String] = ["water_event"]
	check(saves.commit_quest_choice("act1.spy_hunt", "shelter_villagers", {"welcomed": true}, effects)["ok"], "choice commits")
	check(state.progress["ritual_charge"] == 1, "one ritual increment")
	check(saves.commit_quest_choice("act1.spy_hunt", "shelter_villagers", {})["replayed"], "same choice replay")
	check(saves.commit_quest_choice("act1.spy_hunt", "judge_suspects", {})["code"] == "QUEST_ALREADY_DECIDED", "opposite choice conflict")
	check(state.progress["ritual_charge"] == 1, "replay does not increment")
	state.npc_states["npc.external"] = "dead"
	state.faction_reputation["refugees"] = -10
	check(saves.save_game()["ok"], "world dictionaries save")
	state.replace_validated(WorldSaveSchema.initial(), false)
	check(saves.load_game()["ok"], "round trip")
	check(state.npc_states["npc.external"] == "dead" and state.faction_reputation["refugees"] == -10, "restores NPC and reputation")
	check(state.pending_effects.has("water_event") and state.world_flags["welcomed"], "pending effects and flags restored")
	var before: Dictionary = state.snapshot()
	var invalid := before.duplicate(true)
	invalid["npc_states"]["bad"] = "immortal"
	check(not saves.commit_snapshot(invalid)["ok"] and state.snapshot() == before, "invalid candidate leaves state unchanged")
	invalid = before.duplicate(true)
	invalid["progress"]["ritual_charge"] = 2
	check(not saves.commit_snapshot(invalid)["ok"], "charge mismatch rejected")
	var original := FileAccess.get_file_as_string("user://tests/savegame.json")
	write_text("user://tests/savegame.json", "{broken")
	var recovered: Dictionary = saves.load_game()
	check(recovered["ok"] and recovered.get("recovered", false), "corrupt primary loads backup")
	check(saves.save_game()["code"] == "PRIMARY_REQUIRES_REPAIR", "corrupt primary not silently overwritten")
	check(saves.repair_primary_from_backup()["ok"], "explicit repair")
	check(FileAccess.get_file_as_string("user://tests/savegame.json.corrupt") == "{broken", "preserves damaged bytes")
	write_text("user://tests/savegame.json", original)
	var envelope: Dictionary = JSON.parse_string(original)
	envelope["payload_json"] += " "
	write_text("user://tests/savegame.json", JSON.stringify(envelope))
	check(saves.read_snapshot().get("primary_error") == "CHECKSUM_MISMATCH", "checksum detects changed payload")
	envelope = JSON.parse_string(original)
	envelope["schema_version"] = 2
	write_text("user://tests/savegame.json", JSON.stringify(envelope))
	before = state.snapshot()
	check(saves.load_game()["code"] == "UNSUPPORTED_SCHEMA" and state.snapshot() == before, "future schema no rollback to backup")
	check(not saves.repair_primary_from_backup()["ok"], "future schema cannot be repaired away")
	write_text("user://tests/savegame.json", original)
	bus.checkpoint_reached.emit("dunes_camp", 4)
	check(state.progress["map"] == 4 and state.progress["act"] == 2 and state.progress["checkpoint"] == "dunes_camp", "checkpoint auto save")
	state.replace_validated(WorldSaveSchema.initial(), false)
	check(saves.load_game()["ok"] and state.progress["map"] == 4, "checkpoint persisted")
	var child_output: Array = []
	var child_status := OS.execute(OS.get_executable_path(), ["--headless", "--path", ProjectSettings.globalize_path("res://"), "--script", "res://tests/reload_probe.gd", "--", "--test-harness"], child_output, true)
	check(child_status == 0, "fresh Godot process restores persisted world")
	var durable := FileAccess.get_file_as_string("user://tests/savegame.json")
	var candidate: Dictionary = state.snapshot()
	candidate["world_flags"]["new_flag"] = true
	DirAccess.make_dir_absolute("user://tests/savegame.json.tmp")
	before = state.snapshot()
	check(not saves.commit_snapshot(candidate)["ok"], "temp path write failure")
	check(state.snapshot() == before and FileAccess.get_file_as_string("user://tests/savegame.json") == durable, "write failure preserves world and disk")
	DirAccess.remove_absolute("user://tests/savegame.json.tmp")
	var bad_envelope: Dictionary = JSON.parse_string(durable)
	var bad_payload: Dictionary = JSON.parse_string(bad_envelope["payload_json"])
	bad_payload["npc_states"]["npc.fake"] = "invincible"
	bad_envelope["payload_json"] = JSON.stringify(bad_payload)
	bad_envelope["sha256"] = bad_envelope["payload_json"].sha256_text()
	write_text("user://tests/savegame.json", JSON.stringify(bad_envelope))
	check(saves.read_snapshot().get("primary_error") == "INVALID_NPC", "recomputed checksum still requires schema validation")
	write_text("user://tests/savegame.json", durable)
	write_text("user://tests/savegame.json", "x".repeat(1024 * 1024 + 1))
	check(saves.read_snapshot().get("primary_error") == "FILE_TOO_LARGE", "oversize file rejected with backup recovery")
	clean()
	print("Persistence checks: %d; failures: %d" % [checks, failures])
	quit(1 if failures else 0)
