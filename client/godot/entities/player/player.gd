class_name Player
extends ActorBase

signal stamina_changed(current: float, maximum: float)
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
var stamina := 100.0
var dash_left := 0.0
var cooldown_left := 0.0
var iframe_left := 0.0
var facing := Vector2.RIGHT
var dash_direction := Vector2.RIGHT
var current_action_state := ActionState.IDLE
var heavy_action := false
var action_left := 0.0

var is_dashing: bool:
	get: return dash_left > 0.00000001
var is_invulnerable: bool:
	get: return not is_dead and iframe_left > 0.00000001

func _ready() -> void:
	super._ready()
	stamina = stamina_max
	add_to_group("player")
	process_physics_priority = -10 # Player timers precede the test enemy's attack.

func project_direction(direction: Vector2) -> Vector2:
	var limited := direction.limit_length(1.0)
	return Vector2(limited.x, limited.y * 0.5)

func attempt_dash(direction: Vector2 = Vector2.ZERO) -> bool:
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
	current_action_state = ActionState.IDLE # Cancel normal recovery, not the attack's damage/cooldown.
	action_left = 0.0
	stamina_changed.emit(stamina, stamina_max)
	return true

func begin_test_action(heavy: bool) -> void:
	# Test phases only; this does not deal combat damage.
	if is_dead or is_dashing or current_action_state != ActionState.IDLE:
		return
	heavy_action = heavy
	current_action_state = ActionState.WINDUP if heavy else ActionState.RECOVERY
	action_left = 0.5 if heavy else 0.35

func advance_timers(delta: float) -> void:
	cooldown_left = maxf(0.0, cooldown_left - delta)
	iframe_left = maxf(0.0, iframe_left - delta)
	var dash_elapsed := minf(dash_left, delta)
	dash_left = maxf(0.0, dash_left - delta)
	if current_action_state != ActionState.IDLE:
		action_left = maxf(0.0, action_left - delta)
		if action_left == 0.0:
			current_action_state = ActionState.IDLE
			heavy_action = false
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
		if Input.is_action_just_pressed("normal_recovery"):
			begin_test_action(false)
		if Input.is_action_just_pressed("heavy_windup"):
			begin_test_action(true)
		if input_vector != Vector2.ZERO:
			facing = input_vector.normalized()
		if Input.is_action_just_pressed("dash"):
			attempt_dash(input_vector)
	var dash_fraction := minf(dash_left, delta) / delta if delta > 0.0 else 0.0
	if is_dashing:
		velocity = project_direction(dash_direction) * dash_speed * dash_fraction
	elif current_action_state == ActionState.WINDUP and heavy_action:
		velocity = Vector2.ZERO
	else:
		velocity = project_direction(input_vector) * speed
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
			current_action_state = ActionState.IDLE
			velocity = Vector2.ZERO
		hp_changed.emit(current_health, max_health)
	return applied
