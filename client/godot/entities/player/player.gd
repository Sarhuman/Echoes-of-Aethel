class_name Player
extends ActorBase

signal stamina_changed(current: float, maximum: float)
signal mp_changed(current: float, maximum: float)
signal attack_started(attack_id: StringName)
signal hp_changed(current: int, maximum: int)

enum ActionState { IDLE, WINDUP, ACTIVE, RECOVERY }
@export var input_enabled := true
@export var speed := 150.0
@export var dash_speed := 400.0
@export var dash_duration := 0.2
@export var iframe_duration := 0.12
@export var stamina_max := 100.0
@export var stamina_regen := 12.0
@export var dash_cost := 25.0
@export var dash_cooldown := 1.2
@export var mp_max := 40.0
var mp := 40.0
var attacks := AttackController.new()
var _command_sequence := 0
var _basic: AttackDefinition = preload("res://combat/basic.tres")
var _heavy: AttackDefinition = preload("res://combat/heavy.tres")
var stamina := 100.0
var dash_left := 0.0
var cooldown_left := 0.0
var iframe_left := 0.0
var facing := Vector2.RIGHT
var dash_direction := Vector2.RIGHT
var current_action_state := ActionState.IDLE
var heavy_action := false

var is_dashing: bool:
	get: return dash_left > 0.00000001
var is_invulnerable: bool:
	get: return not is_dead and iframe_left > 0.00000001

func _ready() -> void:
	super._ready()
	stamina = stamina_max
	mp = mp_max
	add_to_group("player")
	process_physics_priority = -10 # Player timers precede the test enemy's attack.

func project_direction(direction: Vector2) -> Vector2:
	var limited := direction.limit_length(1.0)
	return Vector2(limited.x, limited.y * 0.5)

func attempt_dash(direction: Vector2 = Vector2.ZERO) -> bool:
	if not direction.is_finite():
		return false
	if is_dead or is_dashing or cooldown_left > 0.00000001 or stamina < dash_cost:
		return false
	if current_action_state in [ActionState.WINDUP, ActionState.ACTIVE] or (current_action_state == ActionState.RECOVERY and heavy_action):
		return false
	if direction != Vector2.ZERO:
		facing = direction.normalized()
	dash_direction = facing
	stamina -= dash_cost
	dash_left = dash_duration
	iframe_left = minf(iframe_duration, dash_duration)
	cooldown_left = dash_cooldown
	attacks.cancel()
	current_action_state = ActionState.IDLE # Costs already paid are not refunded.
	stamina_changed.emit(stamina, stamina_max)
	return true

func request_attack(heavy: bool, sequence: int = 0) -> AttackController.Status:
	if sequence == 0:
		_command_sequence += 1
		sequence = _command_sequence
	else:
		_command_sequence = maxi(_command_sequence, sequence)
	var spec := _heavy if heavy else _basic
	var result := attacks.request(sequence, spec, facing, mp, stamina,
		not is_dead and not is_dashing and combat_enabled and zone_id != &"citadel")
	if result["status"] == AttackController.Status.ACCEPTED:
		# Single synchronous commit; publish only after resources and action agree.
		mp = result["mp"]
		stamina = result["stamina"]
		_sync_attack_state()
		mp_changed.emit(mp, mp_max)
		stamina_changed.emit(stamina, stamina_max)
		attack_started.emit(spec.attack_id)
	return result["status"]

func _sync_attack_state() -> void:
	current_action_state = attacks.phase as ActionState
	heavy_action = attacks.definition != null and attacks.definition.heavy

func advance_attack(delta: float) -> void:
	if is_dead:
		attacks.cancel()
		_sync_attack_state()
		return
	var result := attacks.advance(delta)
	if not result["ok"]:
		return
	_sync_attack_state()
	if result["active"]:
		_resolve_attack_contacts()
	attacks.close_window()

func _resolve_attack_contacts() -> void:
	# Fixed-step snapshot sampling, not historical rewind or swept collision.
	var shape := CircleShape2D.new()
	shape.radius = attacks.definition.reach
	var query := PhysicsShapeQueryParameters2D.new()
	query.shape = shape
	query.transform = Transform2D(0.0, global_position)
	query.collision_mask = 1
	query.exclude = [get_rid()]
	var contacts := get_world_2d().direct_space_state.intersect_shape(query, 128)
	contacts.sort_custom(func(a, b): return a["collider_id"] < b["collider_id"])
	for contact in contacts:
		var target := contact["collider"] as ActorBase
		if not is_instance_valid(target) or target.is_dead or target.team_id == team_id:
			continue
		if not CombatService.in_cone(attacks.direction, target.global_position - global_position, attacks.definition.reach):
			continue
		var ray := PhysicsRayQueryParameters2D.create(global_position, target.global_position, 2)
		ray.exclude = [get_rid(), target.get_rid()]
		if not get_world_2d().direct_space_state.intersect_ray(ray).is_empty():
			continue
		attacks.resolve_hit(target.get_instance_id(), func(): return target.take_damage(attacks.definition.damage, self))

func advance_timers(delta: float) -> void:
	if not is_finite(delta) or delta < 0.0:
		return
	cooldown_left = maxf(0.0, cooldown_left - delta)
	iframe_left = maxf(0.0, iframe_left - delta)
	var dash_elapsed := minf(dash_left, delta)
	dash_left = maxf(0.0, dash_left - delta)
	var previous := stamina
	stamina = minf(stamina_max, stamina + stamina_regen * (delta - dash_elapsed))
	if stamina != previous:
		stamina_changed.emit(stamina, stamina_max)

func _physics_process(delta: float) -> void:
	if is_dead:
		return
	var input_vector := Vector2.ZERO
	if input_enabled:
		input_vector = Input.get_vector("move_left", "move_right", "move_up", "move_down")
		if input_vector != Vector2.ZERO and current_action_state == ActionState.IDLE:
			facing = input_vector.normalized()
		if Input.is_action_just_pressed("normal_recovery"):
			request_attack(false)
		if Input.is_action_just_pressed("heavy_windup"):
			request_attack(true)
		if Input.is_action_just_pressed("dash"):
			attempt_dash(input_vector)
	advance_attack(delta)
	var dash_fraction := minf(dash_left, delta) / delta if delta > 0.0 else 0.0
	if is_dashing:
		velocity = project_direction(dash_direction) * dash_speed * dash_fraction
	elif current_action_state == ActionState.WINDUP and heavy_action:
		velocity = Vector2.ZERO
	else:
		velocity = project_direction(input_vector) * speed * attacks.movement_scale()
	move_and_slide() # Once per physics tick; scenery collision remains enabled.
	advance_timers(delta)

func take_damage(amount: int, attacker: ActorBase) -> bool:
	if is_invulnerable:
		return false
	var applied := super.take_damage(amount, attacker)
	if applied:
		if is_dead:
			dash_left = 0.0
			iframe_left = 0.0
			attacks.cancel()
			current_action_state = ActionState.IDLE
			velocity = Vector2.ZERO
		hp_changed.emit(current_health, max_health)
	return applied
