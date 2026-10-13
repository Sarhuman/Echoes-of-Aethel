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
	player.input_enabled = false
	player.set_physics_process(false)
	enemy.set_physics_process(false)
	scene.get_node("Enemy/Brain").set_physics_process(false)
	check(player.current_health == player.max_health, "parent health initialization")
	check(player.project_direction(Vector2(0.5, 0)) == Vector2(0.5, 0), "analog magnitude retained")
	check(player.project_direction(Vector2.DOWN) == Vector2(0, 0.5), "2:1 projection")
	check(player.project_direction(Vector2(1, 1)).x < 0.71, "diagonal capped")
	check(player.attempt_dash(Vector2.RIGHT) and player.stamina == 75, "approved cost 25")
	check(not player.attempt_dash(), "repeat dash refused")
	check(not player.take_damage(15, enemy) and player.current_health == 100, "initial invulnerability")
	player.advance_timers(0.119)
	check(player.is_invulnerable, "protected before 120ms")
	player.advance_timers(0.001)
	check(not player.is_invulnerable and player.is_dashing, "iframes end independently from dash")
	check(player.take_damage(15, enemy) and player.current_health == 85, "damage after window while dashing")
	player.advance_timers(0.08)
	check(not player.is_dashing and not player.attempt_dash(), "dash duration and cooldown separate")
	player.advance_timers(1.0)
	player.begin_test_action(false)
	check(player.attempt_dash(), "normal recovery cancellable")
	player.advance_timers(1.2)
	player.begin_test_action(true)
	check(not player.attempt_dash(), "heavy windup blocks dash")
	player.advance_timers(0.5)
	player.stamina = 24
	check(not player.attempt_dash(), "insufficient stamina")
	player.stamina = 100
	player.global_position = Vector2(414, 360)
	check(player.attempt_dash(Vector2.RIGHT), "wall test dash starts")
	player.set_physics_process(true)
	for i in range(18): await physics_frame
	player.set_physics_process(false)
	check(player.global_position.x < 422, "solid wall blocks dash")
	player.cooldown_left = 0
	player.stamina = 100
	player.attempt_dash()
	player.advance_timers(0.12)
	player.current_health = 5
	check(player.take_damage(15, enemy) and player.is_dead, "death during remaining dash")
	check(not player.is_dashing and not player.is_invulnerable and player.velocity == Vector2.ZERO, "death clears dash and timers")
	check(not player.attempt_dash(), "dead player cannot dash")
	scene.queue_free()
	await process_frame
	print("Player checks: %d; failures: %d" % [checks, failures])
	quit(1 if failures else 0)
