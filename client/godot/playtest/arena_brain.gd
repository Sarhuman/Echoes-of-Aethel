class_name ArenaBrain
extends EnemyFSM
## Remembers only the last visible position, never tracks through walls.
var navigation: ArenaNavigation
var last_seen := Vector2.ZERO
var memory_left := 0.0
var route_left := 0.0
var route_points := PackedVector2Array()
var route_target := Vector2.INF

func _physics_process(delta: float) -> void:
	memory_left = maxf(0.0, memory_left - delta)
	route_left = maxf(0.0, route_left - delta)
	super._physics_process(delta)

func _process_chase() -> void:
	if not _target_valid(target_actor):
		target_actor = null
		change_state(State.IDLE)
		return
	var distance := actor.global_position.distance_to(target_actor.global_position)
	if distance > detection_range * 1.5:
		target_actor = null
		change_state(State.IDLE)
		return
	if _line_of_sight(target_actor):
		last_seen = target_actor.global_position
		memory_left = 3.0
		if distance <= attack_range:
			change_state(State.ATTACK)
			return
	elif memory_left <= 0.0 or actor.global_position.distance_to(last_seen) < 8.0:
		target_actor = null
		change_state(State.IDLE)
		return
	_move_towards(last_seen)

func _process_attack(delta: float) -> void:
	if _target_valid(target_actor) and not _line_of_sight(target_actor):
		change_state(State.CHASE)
		return
	super._process_attack(delta)

func _move_towards(destination: Vector2) -> void:
	if navigation == null:
		super._move_towards(destination)
		return
	if route_left <= 0.0 or destination.distance_to(route_target) > 24.0:
		route_points = navigation.route(actor.global_position, destination)
		if route_points.size() > 1:
			route_points.remove_at(0)
		route_target = destination
		route_left = 0.25
	while not route_points.is_empty() and actor.global_position.distance_to(route_points[0]) < 6.0:
		route_points.remove_at(0)
	if route_points.is_empty():
		actor.velocity = Vector2.ZERO
		return
	super._move_towards(route_points[0])
