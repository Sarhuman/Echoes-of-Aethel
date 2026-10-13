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
	var scene := load("res://scenes/test_scene.tscn").instantiate() as Node2D
	root.add_child(scene)
	var player := scene.get_node("Player") as Player
	var enemy := scene.get_node("Enemy") as ActorBase
	var mode := scene.get_node("Presentation") as CombatPresentation
	player.input_enabled = false
	player.set_physics_process(false)
	enemy.set_physics_process(false)
	scene.get_node("Enemy/Brain").set_physics_process(false)
	mode.set_process(false)
	var visual := player.get_node("Visual") as Polygon2D
	var original := visual.color
	mode.set_enhanced(false)
	var snapshot: Dictionary = root.get_node("GameState").snapshot()
	check(player.take_damage(15, enemy) and player.current_health == 85, "base damage")
	check(visual.color == original and mode._impacts.is_empty(), "base effects absent")
	mode.set_enhanced(true)
	check(player.take_damage(15, enemy) and player.current_health == 70, "enhanced same damage")
	check(visual.color == Color.WHITE and mode._impacts.size() == 1, "confirmed impact feedback")
	mode.set_enhanced(false)
	check(visual.color == original and mode._impacts.is_empty() and mode._flashes.is_empty(), "immediate restore")
	mode.set_enhanced(true)
	player.cooldown_left = 0
	player.stamina = 100
	player.attempt_dash()
	check(not player.take_damage(15, enemy) and mode._impacts.is_empty(), "missed damage no false feedback")
	player.advance_timers(0.12)
	mode.set_reduced_effects(true)
	check(player.take_damage(15, enemy) and player.current_health == 55 and visual.color == original, "reduced visuals preserve damage")
	mode.set_reduced_effects(false)
	for i in range(40): mode._on_actor_damaged("player.test", 1, "enemy.test")
	check(mode._impacts.size() <= mode.max_impacts, "effects bounded")
	mode._process(1.0)
	check(visual.color == original and mode._impacts.is_empty(), "expiry restores colors")
	mode._on_actor_damaged("player.test", 1, "enemy.test")
	mode.set_reduced_effects(true)
	check(visual.color == original and mode._impacts.is_empty(), "accessibility toggle cancels active effects")
	mode.set_reduced_effects(false)
	var nodes := mode.get_child_count()
	for i in range(50):
		mode.set_enhanced(false)
		mode.set_enhanced(true)
	check(mode.get_child_count() == nodes, "toggle does not allocate nodes")
	check(root.get_node("GameState").snapshot() == snapshot, "presentation never modifies world save")
	check(player.dash_cost == 25 and player.iframe_duration == 0.12 and player.dash_cooldown == 1.2, "gameplay parameters unchanged")
	scene.queue_free()
	await process_frame
	print("Presentation checks: %d; failures: %d" % [checks, failures])
	quit(1 if failures else 0)
