class_name EnemyFSM
extends Node

signal state_changed(previous: State, current: State)
signal attack_telegraphed(target_id: StringName, position: Vector2)

enum State { IDLE, PATROL, CHASE, ATTACK, DEAD }

@export var actor: ActorBase
@export var aggressive := true
@export var detection_range := 200.0
@export var attack_range := 40.0
@export var attack_cooldown := 1.5
@export var windup_duration := 0.4
@export var attack_damage := 15
@export var obstacle_mask: int = 2
@export var vertical_projection_scale := 1.0
@export var waypoints := PackedVector2Array()

var current_state := State.IDLE
var target_actor: ActorBase
var attack_timer := 0.0
var _windup_left := 0.0
var _winding := false
var _locked_target: ActorBase
var _candidates: Array[ActorBase] = []
var _patrol_index := 0
var _sense_timer := 0.0
var _seen_crimes: Array[String] = []
var _detector: Area2D
@onready var _events: Node = get_node("/root/EventBus")
@onready var _world: Node = get_node("/root/GameState")
@onready var _saves: Node = get_node("/root/SaveSystem")

func _ready() -> void:
	if actor == null:
		actor = get_parent() as ActorBase
	if actor == null or actor.actor_id == &"":
		push_error("EnemyFSM requires an ActorBase with a stable ID")
		set_physics_process(false)
		return
	detection_range = maxf(detection_range, 1.0)
	attack_range = clampf(attack_range, 1.0, detection_range)
	attack_cooldown = maxf(attack_cooldown, 0.05)
	windup_duration = maxf(windup_duration, 0.05)
	vertical_projection_scale = clampf(vertical_projection_scale, 0.5, 1.0)
	var shape := CircleShape2D.new()
	shape.radius = detection_range
	var collision := CollisionShape2D.new()
	collision.shape = shape
	_detector = Area2D.new()
	_detector.collision_layer = 0
	_detector.collision_mask = 1
	_detector.add_child(collision)
	actor.add_child.call_deferred(_detector)
	_detector.body_entered.connect(_on_body_entered)
	_detector.body_exited.connect(_on_body_exited)
	_events.crime_committed.connect(_on_crime_committed)
	_events.world_restored.connect(_on_world_restored)
	_on_world_restored()

func _exit_tree() -> void:
	if _events.crime_committed.is_connected(_on_crime_committed):
		_events.crime_committed.disconnect(_on_crime_committed)
	if _events.world_restored.is_connected(_on_world_restored):
		_events.world_restored.disconnect(_on_world_restored)
	if is_instance_valid(_detector):
		_detector.queue_free()

func _target_valid(target: ActorBase) -> bool:
	return is_instance_valid(target) and target != actor and not target.is_dead \
		and target.combat_enabled and actor.combat_enabled and target.zone_id == actor.zone_id

func _line_of_sight(target: ActorBase) -> bool:
	var query := PhysicsRayQueryParameters2D.create(actor.global_position, target.global_position, obstacle_mask)
	query.exclude = [actor.get_rid(), target.get_rid()]
	return actor.get_world_2d().direct_space_state.intersect_ray(query).is_empty()

func _on_body_entered(body: Node2D) -> void:
	if body is ActorBase and body != actor and not _candidates.has(body):
		_candidates.append(body)

func _on_body_exited(body: Node2D) -> void:
	_candidates.erase(body)

func _acquire_target() -> void:
	if not aggressive:
		return
	var nearest := detection_range + 1.0
	for candidate in _candidates:
		if not _target_valid(candidate) or candidate.team_id == actor.team_id:
			continue
		var distance := actor.global_position.distance_to(candidate.global_position)
		if distance < nearest and _line_of_sight(candidate):
			target_actor = candidate
			nearest = distance
	if _target_valid(target_actor):
		change_state(State.CHASE)

func change_state(next_state: State) -> void:
	if current_state == State.DEAD or current_state == next_state:
		return
	var previous := current_state
	current_state = next_state
	actor.velocity = Vector2.ZERO
	_winding = false
	_locked_target = null
	if next_state == State.DEAD:
		target_actor = null
		set_physics_process(false)
	state_changed.emit(previous, next_state)

func _physics_process(delta: float) -> void:
	if not is_instance_valid(actor):
		set_physics_process(false)
		return
	if actor.is_dead:
		change_state(State.DEAD)
		return
	if not actor.combat_enabled:
		actor.velocity = Vector2.ZERO
		target_actor = null
		change_state(State.IDLE)
		return
	attack_timer = maxf(0.0, attack_timer - delta)
	_sense_timer -= delta
	if current_state in [State.IDLE, State.PATROL] and _sense_timer <= 0.0:
		_sense_timer = 0.2
		_acquire_target()
	match current_state:
		State.IDLE:
			if not waypoints.is_empty():
				change_state(State.PATROL)
		State.PATROL:
			_process_patrol()
		State.CHASE:
			_process_chase()
		State.ATTACK:
			_process_attack(delta)

func _process_patrol() -> void:
	if waypoints.is_empty():
		change_state(State.IDLE)
		return
	var destination := waypoints[_patrol_index] # World-space waypoints.
	if actor.global_position.distance_to(destination) <= 4.0:
		_patrol_index = (_patrol_index + 1) % waypoints.size()
		actor.velocity = Vector2.ZERO
	else:
		_move_towards(destination)

func _move_towards(destination: Vector2) -> void:
	var direction := (destination - actor.global_position).normalized()
	actor.velocity = Vector2(direction.x, direction.y * vertical_projection_scale) * actor.movement_speed

func _process_chase() -> void:
	if not _target_valid(target_actor):
		target_actor = null
		change_state(State.IDLE)
		return
	var distance := actor.global_position.distance_to(target_actor.global_position)
	if distance > detection_range * 1.5 or not _line_of_sight(target_actor):
		target_actor = null
		change_state(State.IDLE)
	elif distance <= attack_range:
		change_state(State.ATTACK)
	else:
		_move_towards(target_actor.global_position)

func _process_attack(delta: float) -> void:
	actor.velocity = Vector2.ZERO
	if not _target_valid(target_actor):
		change_state(State.IDLE)
		return
	if _winding:
		_windup_left -= delta
		if _windup_left <= 0.0:
			_winding = false
			attack_timer = attack_cooldown
			if _target_valid(_locked_target) and actor.global_position.distance_to(_locked_target.global_position) <= attack_range and _line_of_sight(_locked_target):
				_locked_target.take_damage(attack_damage, actor)
		return
	if actor.global_position.distance_to(target_actor.global_position) > attack_range:
		change_state(State.CHASE)
		return
	if attack_timer == 0.0 and _line_of_sight(target_actor):
		_winding = true
		_windup_left = windup_duration
		_locked_target = target_actor
		attack_telegraphed.emit(target_actor.actor_id, target_actor.global_position)

func _on_crime_committed(event: Dictionary) -> void:
	# Only confirmed local CrimeService events may reach this adapter.
	for key in ["crime_id", "victim_id", "attacker_id", "zone_id", "position"]:
		if not event.has(key):
			return
	if not event["crime_id"] is String or not event["position"] is Vector2:
		return
	if not is_instance_valid(actor) or actor.is_dead or not actor.combat_enabled or not actor.narrative_npc:
		return
	if event["zone_id"] != String(actor.zone_id) or event["attacker_id"] == String(actor.actor_id):
		return
	if actor.global_position.distance_to(event["position"]) > detection_range or _seen_crimes.has(event["crime_id"]):
		return
	for candidate in _candidates:
		if _target_valid(candidate) and String(candidate.actor_id) == event["attacker_id"] and _line_of_sight(candidate):
			var snapshot: Dictionary = _world.snapshot()
			if snapshot["npc_states"].get(String(actor.actor_id)) == "dead":
				return
			snapshot["npc_states"][String(actor.actor_id)] = "hostile"
			if not _saves.commit_snapshot(snapshot, "witness_hostility")["ok"]:
				return
			_seen_crimes.append(event["crime_id"])
			if _seen_crimes.size() > 128:
				_seen_crimes.pop_front()
			aggressive = true
			target_actor = candidate
			change_state(State.CHASE)
			return

func _on_world_restored() -> void:
	if not is_instance_valid(actor) or not actor.narrative_npc:
		return
	var status: StringName = _world.get_npc_state(actor.actor_id)
	if status == &"dead":
		actor.restore_dead()
		change_state(State.DEAD)
	elif status == &"hostile":
		aggressive = true
