extends SceneTree

var failures := 0
var checks := 0
var hit_count := 0
var death_count := 0

func _initialize() -> void:
	call_deferred("run")

func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)

func make_actor(id: StringName, position_value: Vector2, team: StringName) -> ActorBase:
	var actor := ActorBase.new()
	actor.actor_id = id
	actor.global_position = position_value
	actor.team_id = team
	actor.collision_layer = 1
	actor.collision_mask = 0
	var collision := CollisionShape2D.new()
	var shape := CircleShape2D.new()
	shape.radius = 2.0
	collision.shape = shape
	actor.add_child(collision)
	root.add_child(actor)
	actor.set_physics_process(false)
	return actor

func run() -> void:
	for suffix in ["", ".bak", ".tmp", ".corrupt"]:
		DirAccess.remove_absolute("user://savegame.json" + suffix)
	var state := root.get_node("GameState")
	var saves := root.get_node("SaveSystem")
	var bus := root.get_node("EventBus")
	state.replace_validated(WorldSaveSchema.initial(), false)
	bus.actor_damaged.connect(func(_id, _damage, _attacker): hit_count += 1)
	bus.actor_died.connect(func(_id): death_count += 1)
	var enemy := make_actor(&"enemy", Vector2.ZERO, &"monsters")
	var player := make_actor(&"player", Vector2(20, 0), &"player")
	var brain := EnemyFSM.new()
	brain.actor = enemy
	enemy.add_child(brain)
	brain.set_physics_process(false)
	await physics_frame
	await physics_frame
	await physics_frame
	check(brain._candidates.has(player), "Area2D acquires candidate")
	brain._physics_process(0.2)
	check(brain.current_state == EnemyFSM.State.ATTACK, "detected target reaches attack")
	brain._physics_process(0.1)
	check(player.current_health == 100 and brain._winding, "windup precedes damage")
	brain._physics_process(0.5)
	check(player.current_health == 85 and hit_count == 1, "one attack one damage event")
	brain._physics_process(0.2)
	check(player.current_health == 85, "cooldown blocks second hit")
	brain.attack_timer = 0.0
	brain._physics_process(0.1)
	player.global_position = Vector2(90, 0)
	brain._physics_process(0.5)
	check(player.current_health == 85, "target exits range during windup")
	brain.change_state(EnemyFSM.State.CHASE)
	player.global_position = Vector2(500, 0)
	brain._physics_process(0.1)
	check(brain.current_state == EnemyFSM.State.IDLE and enemy.velocity == Vector2.ZERO, "lost target clears velocity")
	brain.waypoints = PackedVector2Array([Vector2(80, 0)])
	brain._physics_process(0.1)
	brain._physics_process(0.1)
	check(brain.current_state == EnemyFSM.State.PATROL and enemy.velocity.x > 0, "patrol implemented")
	player.global_position = Vector2(20, 0)
	var wall := StaticBody2D.new()
	wall.collision_layer = 2
	var wall_collision := CollisionShape2D.new()
	var rectangle := RectangleShape2D.new()
	rectangle.size = Vector2(4, 30)
	wall_collision.shape = rectangle
	wall.add_child(wall_collision)
	wall.position = Vector2(10, 0)
	root.add_child(wall)
	await physics_frame
	await physics_frame
	check(not brain._line_of_sight(player), "wall blocks line of sight")
	brain.target_actor = player
	brain.change_state(EnemyFSM.State.ATTACK)
	brain.attack_timer = 0.0
	brain._physics_process(0.1)
	brain._physics_process(1.0)
	check(player.current_health == 85, "no attack through wall")
	wall.queue_free()
	await physics_frame
	await physics_frame
	enemy.combat_enabled = false
	brain.target_actor = player
	brain._physics_process(1.0)
	check(brain.current_state == EnemyFSM.State.IDLE and player.current_health == 85, "hub blocks AI")
	check(not enemy.take_damage(20, player), "hub blocks received damage")
	enemy.combat_enabled = true
	check(not player.take_damage(-10, enemy), "negative damage rejected")
	player.current_health = 5
	check(player.take_damage(15, enemy), "lethal damage")
	check(player.is_dead and death_count == 1, "one death event")
	check(not player.take_damage(15, enemy) and death_count == 1, "death is idempotent")
	brain.target_actor = player
	brain.change_state(EnemyFSM.State.CHASE)
	brain._physics_process(0.1)
	check(brain.current_state == EnemyFSM.State.IDLE, "dead target released")
	var attacker := make_actor(&"aggressor", Vector2(40, 0), &"player")
	enemy.narrative_npc = true
	brain.aggressive = false
	brain._on_body_entered(attacker)
	var crime := {"crime_id": "crime_1", "victim_id": "citizen", "attacker_id": "aggressor", "zone_id": "other_zone", "position": Vector2(30, 0)}
	brain._on_crime_committed(crime)
	check(not brain.aggressive, "remote crime ignored")
	crime["zone_id"] = "field"
	brain._on_crime_committed(crime)
	check(brain.aggressive and brain.target_actor == attacker, "witness identifies attacker")
	check(state.get_npc_state(&"enemy") == &"hostile" and saves.read_snapshot()["ok"], "hostility persisted before chase")
	brain._on_crime_committed(crime)
	check(brain._seen_crimes.size() == 1, "duplicate crime ignored")
	enemy.take_damage(999, attacker)
	brain._physics_process(0.1)
	check(brain.current_state == EnemyFSM.State.DEAD and enemy.velocity == Vector2.ZERO, "dead brain stops")
	check(saves.read_snapshot()["payload"]["npc_states"]["enemy"] == "dead", "narrative NPC death persisted")
	brain.change_state(EnemyFSM.State.CHASE)
	check(brain.current_state == EnemyFSM.State.DEAD, "dead state terminal")
	var corpse := make_actor(&"persisted_npc", Vector2(700, 0), &"citizens")
	corpse.narrative_npc = true
	state.npc_states["persisted_npc"] = "dead"
	var dead_brain := EnemyFSM.new()
	dead_brain.actor = corpse
	var previous_deaths := death_count
	corpse.add_child(dead_brain)
	check(corpse.is_dead and dead_brain.current_state == EnemyFSM.State.DEAD and death_count == previous_deaths, "restore corpse without death reward")
	for actor in [enemy, player, attacker, corpse]: actor.queue_free()
	await process_frame
	for suffix in ["", ".bak", ".tmp", ".corrupt"]:
		DirAccess.remove_absolute("user://savegame.json" + suffix)
	print("FSM checks: %d; failures: %d" % [checks, failures])
	quit(1 if failures else 0)
