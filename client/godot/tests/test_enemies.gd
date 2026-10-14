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
	var wolf: EnemyDefinition = load("res://content/enemies/ethereal_wolf.tres")
	var invalid := wolf.duplicate() as EnemyDefinition
	invalid.attack_range = NAN
	check(not invalid.valid(),"invalid parameters refused")
	var world := Node2D.new()
	root.add_child(world)
	var player := preload("res://entities/player/player.tscn").instantiate() as Player
	world.add_child(player)
	player.input_enabled = false
	player.set_physics_process(false)
	player.zone_id = &"forest_01"
	player.position = Vector2(200,200)
	var boss := preload("res://entities/enemies/moss_claws.tscn").instantiate() as EnemyActor
	boss.actor_id = &"forest_01.moss_claws.01:1"
	boss.zone_id = &"forest_01"
	world.add_child(boss)
	boss.set_physics_process(false)
	boss.position = Vector2(150,200)
	check(boss.brain.windup_for(0) == 0.9 and boss.brain.windup_for(1) == 1.1 and boss.brain.windup_for(2) == 1.2,"all boss telegraphs")
	boss.brain.locked_position = Vector2(200,200)
	check(not boss.brain.roots_hit(Vector2(200,200)) and boss.brain.roots_hit(Vector2(250,200)),"roots preserve corridor")
	boss.brain.interrupt(0.5)
	check(boss.brain.interrupted == 0,"boss stun immune")
	boss.current_health = 399
	boss.brain.step(0.01,player)
	check(boss.brain.phase_two and boss.brain.phase == "recovery","phase change offers recovery")
	var common := preload("res://entities/enemies/ethereal_wolf.tscn").instantiate() as EnemyActor
	common.actor_id = &"forest_01.ethereal_wolf.01:1"
	common.zone_id = &"forest_01"
	world.add_child(common)
	common.set_physics_process(false)
	common.brain.phase = "windup"
	common.interrupt(0.5)
	check(common.brain.phase == "recovery" and common.brain.interrupted == 0.5,"impact cancels common windup")
	common.zone_id = &"citadel"
	check(not common.take_damage(10,player),"safe zone protected")
	var wall := StaticBody2D.new()
	wall.collision_layer = 2
	wall.position = Vector2(250,200)
	var shape := CollisionShape2D.new()
	var rect := RectangleShape2D.new()
	rect.size = Vector2(20,160)
	shape.shape = rect
	wall.add_child(shape)
	world.add_child(wall)
	common.zone_id = &"forest_01"
	common.position = Vector2(210,200)
	common.brain.phase = "charge"
	common.brain.remaining = 1
	common.brain.interrupted = 0
	common.brain.locked_direction = Vector2.RIGHT
	player.position = Vector2(320,200)
	common.set_physics_process(true)
	for i in range(45): await physics_frame
	check(common.position.x < 240,"charge blocked by physical wall")
	common.set_physics_process(false)
	check((common.get_node("Shape") as CollisionShape2D).shape.radius >= 20, "enemy body keeps visible separation")
	check(EnemyActor.WOLF_WALK is Texture2D and EnemyActor.WOLF_WALK.get_width() > 1000, "wolf locomotion frames import")
	for frame in range(4):
		var cell := EnemyActor.atlas_cell(EnemyActor.ATLAS, frame)
		check(cell.size == Vector2(627, 627) and cell.end.x <= EnemyActor.ATLAS.get_width() and cell.end.y <= EnemyActor.ATLAS.get_height(), "enemy atlas frame %d has exact bounds" % frame)
	var cultist: EnemyDefinition = load("res://content/enemies/blind_cultist.tres")
	var shot := EnemyProjectile.new()
	shot.source = common
	shot.spec = cultist
	shot.direction = Vector2.RIGHT
	world.add_child(shot)
	shot.position = Vector2(225,200)
	var hp := player.current_health
	for i in range(35): await physics_frame
	check(not is_instance_valid(shot) and player.current_health == hp,"projectile blocked by wall")
	var corpse := preload("res://entities/enemies/ethereal_wolf.tscn").instantiate() as EnemyActor
	corpse.actor_id = &"forest_01.ethereal_wolf.test:1"
	corpse.zone_id = &"forest_01"
	world.add_child(corpse)
	corpse.set_physics_process(false)
	corpse.current_health = 1
	check(corpse.take_damage(1, player) and corpse.is_dead, "wolf death confirmed")
	await create_timer(0.4).timeout
	check(not is_instance_valid(corpse), "dead wolf leaves no painted marker or collision")
	var forest := preload("res://world/maps/forest_01.tscn").instantiate() as SliceMap
	world.add_child(forest)
	player.position = Vector2(1010, 500)
	common.position = Vector2(925, 500)
	common.brain.phase = "idle"
	common.brain.interrupted = 0
	common.brain.attack_timer = 0
	await physics_frame
	common.brain.step(0.016, player)
	check(common.brain.current_state == EnemyFSM.State.CHASE and absf(common.velocity.y) > 0, "wolf steers around well instead of freezing")
	forest.queue_free()
	await process_frame
	player.position = Vector2(200, 300)
	common.position = Vector2(235, 300)
	await physics_frame
	for i in range(35):
		player.velocity = Vector2(150, 0)
		player.move_and_slide()
		await physics_frame
	check(player.position.x > 255, "player can move through wolf without becoming pinned")
	var mage := preload("res://entities/enemies/blind_cultist.tscn").instantiate() as EnemyActor
	world.add_child(mage)
	mage.set_physics_process(false)
	check(player.collision_mask == 2 and common.collision_mask == 2 and boss.collision_mask == 2 and mage.collision_mask == 2, "all actors collide with scenery, not each other")
	check(EnemyActor.CULTIST_WALK.get_width() / 4.0 == 543.0 and EnemyActor.MOSS_WALK.get_width() / 4.0 == 543.0, "cultist and boss have four walk cells")
	for frame in range(4):
		var cultist_foot: float = 10.0 - EnemyActor.CULTIST_FEET[frame] / EnemyActor.CULTIST_WALK.get_height() * 86.0 + EnemyActor.CULTIST_FEET[frame] / EnemyActor.CULTIST_WALK.get_height() * 86.0
		var moss_foot: float = 10.0 - EnemyActor.MOSS_FEET[frame] / EnemyActor.MOSS_WALK.get_height() * 152.0 + EnemyActor.MOSS_FEET[frame] / EnemyActor.MOSS_WALK.get_height() * 152.0
		check(absf(cultist_foot - moss_foot) < 0.1, "grounded enemy frame %d shares foot baseline" % frame)
	boss.position = Vector2(435, 400)
	player.position = Vector2(380, 400)
	await physics_frame
	for i in range(35):
		player.velocity = Vector2(150, 0)
		player.move_and_slide()
		await physics_frame
	check(player.position.x > 450, "player can pass through moss claws")
	for i in range(35):
		player.velocity = Vector2(-150, 0)
		player.move_and_slide()
		await physics_frame
	check(player.position.x < 400, "player can return across moss claws")
	world.queue_free()
	await process_frame
	finish()
