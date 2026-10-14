extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func run() -> void:
	var director := AethelAudioDirector.new()
	root.add_child(director)
	for id in ["prologue", "citadel", "forest_01", "combat"]:
		check(AethelAudioDirector.TRACKS[id] is AudioStream, "music stream imports: " + id)
	for id in ["basic", "bulwark_impact", "twilight_guard", "steel_rift", "dash", "hit"]:
		check(AethelAudioDirector.EFFECTS[id] is AudioStream, "effect stream imports: " + id)
	director.set_context("forest_01", true)
	check(director.zone == "forest_01" and director.fighting, "combat context selected")
	director.set_cinematic(true)
	check(director.cinematic, "opening selects original cinematic score")
	director.set_cinematic(false)
	director.enabled = false
	director.play_effect(&"basic")
	check(not director._effects[0].playing, "muted option suppresses effects")
	director.queue_free()
	await process_frame
	print("Audio: %d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
