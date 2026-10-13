extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void:
	call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func run() -> void:
	var arena = load("res://playtest/arena.tscn").instantiate()
	root.add_child(arena)
	arena.player.input_enabled = false
	check(arena.stage == arena.Stage.READY and arena.enemies.is_empty(), "safe ready state")
	var nav: ArenaNavigation = arena.navigation
	var route := nav.route(Vector2(350, 280), Vector2(470, 280))
	check(route.size() > 6, "route detours around wall")
	var clear := true
	for point in route:
		if arena.OBSTACLES[0].grow(19).has_point(point): clear = false
	check(clear, "route preserves body clearance")
	check(nav.route(Vector2(-100, -100), Vector2.ZERO).is_empty(), "invalid route rejected")
	var walker := ActorBase.new()
	walker.actor_id = &"navigation.probe"
	walker.position = Vector2(350, 280)
	walker.movement_speed = 100
	arena._body(walker)
	arena.add_child(walker)
	var walker_brain := ArenaBrain.new()
	walker_brain.actor = walker
	walker_brain.navigation = nav
	walker.add_child(walker_brain)
	walker_brain.set_physics_process(false)
	var crossed_wall := false
	for i in range(300):
		walker_brain.route_left = maxf(0.0, walker_brain.route_left - 1.0 / 60.0)
		walker_brain._move_towards(Vector2(470, 280))
		await physics_frame
		if arena.OBSTACLES[0].has_point(walker.position): crossed_wall = true
	check(walker.position.distance_to(Vector2(470, 280)) < 25, "physical agent completes obstacle detour")
	check(not crossed_wall, "physical agent never crosses wall")
	walker.queue_free()
	await physics_frame
	arena.advance_session()
	check(arena.wave == 1 and arena.enemies.size() == 1, "first encounter starts")
	arena.advance_session()
	check(arena.wave == 1, "start during fight cannot duplicate enemies")
	var enemy: ActorBase = arena.enemies[0]
	var brain: ArenaBrain = enemy.get_child(1)
	var before := enemy.position
	for i in range(180): await physics_frame
	check(enemy.position.distance_to(before) > 40, "enemy approaches player physically")
	# Explicit occlusion scenario: preserve only the previously visible point.
	brain.set_physics_process(false)
	enemy.position = Vector2(350, 280)
	arena.player.position = Vector2(480, 280)
	brain.target_actor = arena.player
	brain.last_seen = Vector2(350, 190)
	brain.memory_left = 1.0
	brain.change_state(EnemyFSM.State.CHASE)
	await physics_frame
	brain._process_chase()
	check(brain.last_seen == Vector2(350, 190), "occlusion does not reveal current position")
	brain.memory_left = 0
	brain._process_chase()
	check(brain.target_actor == null and enemy.velocity == Vector2.ZERO, "lost target stops chase")
	brain.set_physics_process(true)
	var enemy_position := enemy.position
	paused = true
	for i in range(5): await process_frame
	check(enemy.position == enemy_position, "pause freezes actors")
	paused = false
	for expected_wave in range(1, 4):
		for foe in arena.enemies:
			foe.take_damage(1000, arena.player)
		arena._process(0.0)
		check(arena.stage == (arena.Stage.WON if expected_wave == 3 else arena.Stage.REST), "wave clear transition %d" % expected_wave)
		if expected_wave < 3:
			arena.player.mp = 0
			arena.advance_session()
			check(arena.wave == expected_wave + 1 and arena.player.mp == arena.player.mp_max, "rest restores MP and advances")
	check(arena.enemies.size() == 3 and arena.stage == arena.Stage.WON, "three waves reach victory")
	arena.queue_free()
	await process_frame
	var defeat = load("res://playtest/arena.tscn").instantiate()
	root.add_child(defeat)
	defeat.player.input_enabled = false
	defeat.advance_session()
	defeat.player.take_damage(1000, defeat.enemies[0])
	defeat._process(0.0)
	check(defeat.stage == defeat.Stage.LOST, "player death reaches defeat")
	defeat.queue_free()
	await process_frame
	print("ARENA: %d checks, %d failures" % [checks, failures])
	quit(1 if failures else 0)
