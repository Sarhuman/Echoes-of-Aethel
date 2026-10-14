class_name Player
extends ActorBase

signal stamina_changed(current: float, maximum: float)
signal mp_changed(current: float, maximum: float)
signal skill_unavailable(slot: int)
signal attack_started(attack_id: StringName)
signal dash_started
signal hp_changed(current: int, maximum: int)
signal action_blocked(reason: String)

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
var _basic: AttackDefinition = preload("res://content/attacks/basic.tres")
var _heavy: AttackDefinition = preload("res://content/attacks/heavy.tres")
var stamina := 100.0
var dash_left := 0.0
var cooldown_left := 0.0
var iframe_left := 0.0
var facing := Vector2.RIGHT
var dash_direction := Vector2.RIGHT
var current_action_state := ActionState.IDLE
var heavy_action := false
var stats := CharacterStats.new()
var regeneration := ResourceRegen.new()
var guard := GuardController.new()
var rng := RandomNumberGenerator.new()
var crit_chance_override := -1
var weapon_power := 8
var armor_power := 2
var engaged := false
var damage_over_time := false
var skill_cooldowns: Dictionary = {}
var _rift: AttackDefinition = preload("res://content/attacks/steel_rift.tres")
var slow_left := 0.0
var _normal_collision_mask := 3

var is_dashing: bool:
	get: return dash_left > 0.00000001
var is_invulnerable: bool:
	get: return not is_dead and iframe_left > 0.00000001

func _ready() -> void:
	speed = stats.definition.move_speed
	dash_speed = stats.definition.dash_speed
	stamina_max = stats.definition.stamina_max
	stamina_regen = stats.definition.stamina_regen
	dash_cost = stats.definition.dash_cost
	dash_duration = stats.definition.dash_duration
	iframe_duration = stats.definition.iframe_duration
	dash_cooldown = stats.definition.dash_cooldown
	apply_stats()
	super._ready()
	rng.randomize()
	stamina = stamina_max
	mp = mp_max
	_normal_collision_mask = collision_mask
	add_to_group("player")
	process_physics_priority = -10 # Player timers precede the test enemy's attack.

func apply_stats() -> void:
	max_health = stats.hp_max()
	mp_max = stats.mp_max()
	defense = stats.defense(armor_power)
	magic_defense = stats.magic_defense(0)
	regeneration.intelligence = stats.definition.intelligence
	current_health = mini(current_health, max_health)
	mp = minf(mp, mp_max)

func cooldown_for(slot: int) -> float:
	if slot == 2: return guard.cooldown_left
	var spec := _basic if slot == 0 else (_heavy if slot == 1 else _rift)
	return skill_cooldowns.get(spec.attack_id, 0.0)

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
	# Pass through enemies during the escape, while scenery (layer 2) stays solid.
	collision_mask = _normal_collision_mask & ~1
	iframe_left = minf(iframe_duration, dash_duration)
	cooldown_left = dash_cooldown
	attacks.cancel()
	current_action_state = ActionState.IDLE # Costs already paid are not refunded.
	stamina_changed.emit(stamina, stamina_max)
	dash_started.emit()
	return true

func request_attack(heavy: bool, sequence: int = 0) -> AttackController.Status:
	return request_definition(_heavy if heavy else _basic, sequence)

func request_definition(spec: AttackDefinition, sequence: int = 0) -> AttackController.Status:
	if sequence == 0:
		_command_sequence += 1
		sequence = _command_sequence
	else:
		_command_sequence = maxi(_command_sequence, sequence)
	var result := attacks.request(sequence, spec, facing, mp, stamina,
		not is_dead and not is_dashing and combat_enabled and zone_id != &"citadel" and skill_cooldowns.get(spec.attack_id, 0.0) <= 0.0)
	if result["status"] == AttackController.Status.ACCEPTED:
		# Single synchronous commit; publish only after resources and action agree.
		mp = result["mp"]
		stamina = result["stamina"]
		skill_cooldowns[spec.attack_id] = spec.cooldown
		regeneration.notify_combat_event()
		if spec.mp_cost > 0: regeneration.notify_mp_spent()
		_sync_attack_state()
		mp_changed.emit(mp, mp_max)
		stamina_changed.emit(stamina, stamina_max)
		attack_started.emit(spec.attack_id)
	return result["status"]

func request_skill(slot: int) -> AttackController.Status:
	if slot == 1:
		return request_attack(true)
	if slot == 3:
		return request_definition(_rift)
	if slot == 2:
		_command_sequence += 1
		var result := guard.request(_command_sequence, 0, mp, not is_dead and not is_dashing and current_action_state == ActionState.IDLE and zone_id != &"citadel")
		if result.status == AttackController.Status.ACCEPTED:
			mp = result.mp
			regeneration.notify_mp_spent()
			mp_changed.emit(mp, mp_max)
			attack_started.emit(&"twilight_guard")
		return result.status
	if slot == 4 and not is_dead:
		skill_unavailable.emit(slot)
	return AttackController.Status.REJECTED

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
		var offset := target.global_position - global_position
		var inside := CombatService.in_cone(attacks.direction, offset, attacks.definition.reach)
		if attacks.definition.line_width > 0:
			var logical := Vector2(offset.x, offset.y * 2)
			inside = logical.dot(attacks.direction) >= 0 and logical.dot(attacks.direction) <= attacks.definition.reach and absf(logical.cross(attacks.direction)) <= attacks.definition.line_width
		if not inside:
			continue
		var ray := PhysicsRayQueryParameters2D.create(global_position, target.global_position, 2)
		ray.exclude = [get_rid(), target.get_rid()]
		if not get_world_2d().direct_space_state.intersect_ray(ray).is_empty():
			continue
		attacks.resolve_hit(target.get_instance_id(), func():
			var hit := DamageCalculator.compute({"power": attacks.definition.power, "weapon": weapon_power, "attribute": stats.definition.strength, "level": stats.level, "defense": target.defense, "chance_bp": stats.crit_chance_bp() if crit_chance_override < 0 else crit_chance_override}, rng)
			var applied := target.take_damage(hit.final, self)
			if applied and attacks.definition.interrupt_seconds > 0 and target.has_method("interrupt"):
				target.interrupt(attacks.definition.interrupt_seconds)
			return applied)

func advance_timers(delta: float) -> void:
	if not is_finite(delta) or delta < 0.0:
		return
	cooldown_left = maxf(0.0, cooldown_left - delta)
	guard.tick(delta)
	slow_left = maxf(0, slow_left - delta)
	for key in skill_cooldowns:
		skill_cooldowns[key] = maxf(0, skill_cooldowns[key] - delta)
	var previous_mp := mp
	mp = regeneration.tick(delta, mp, mp_max, engaged or damage_over_time)
	if previous_mp != mp: mp_changed.emit(mp, mp_max)
	iframe_left = maxf(0.0, iframe_left - delta)
	var dash_elapsed := minf(dash_left, delta)
	dash_left = maxf(0.0, dash_left - delta)
	if not is_dashing and collision_mask != _normal_collision_mask:
		dash_left = 0.0
		collision_mask = _normal_collision_mask
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
		if Input.is_action_just_pressed("attack_basic"):
			var status := request_attack(false)
			if status == AttackController.Status.REJECTED: explain_rejection(false)
		elif Input.is_action_just_pressed("skill_1"):
			var status := request_skill(1)
			if status == AttackController.Status.REJECTED: explain_rejection(true)
		else:
			for slot in range(2, 5):
				if Input.is_action_just_pressed("skill_%d" % slot):
					var status := request_skill(slot)
					if status == AttackController.Status.REJECTED and slot != 4: explain_rejection(true)
					break
		if Input.is_action_just_pressed("dash"):
			attempt_dash(input_vector)
	advance_attack(delta)
	var dash_fraction := minf(dash_left, delta) / delta if delta > 0.0 else 0.0
	if is_dashing:
		velocity = project_direction(dash_direction) * dash_speed * dash_fraction
	elif current_action_state == ActionState.WINDUP and heavy_action:
		velocity = Vector2.ZERO
	else:
		velocity = project_direction(input_vector) * speed * attacks.movement_scale() * (0.7 if slow_left > 0 else 1.0)
	move_and_slide() # Once per physics tick; scenery collision remains enabled.
	advance_timers(delta)

func explain_rejection(skill: bool) -> void:
	if zone_id == &"citadel":
		action_blocked.emit("A Cidadela é área segura. Ataques e habilidades de combate funcionam no Bosque.")
	elif skill:
		action_blocked.emit("Habilidade indisponível: aguarde a recarga ou recupere MP.")

func take_damage(amount: int, attacker: ActorBase) -> bool:
	if is_invulnerable:
		return false
	var front := false
	if is_instance_valid(attacker):
		var offset := attacker.global_position - global_position
		front = facing.dot(Vector2(offset.x, offset.y * 2).normalized()) >= 0
	var applied := super.take_damage(guard.reduce_damage(amount, front), attacker)
	if applied:
		regeneration.notify_combat_event()
		if is_dead:
			dash_left = 0.0
			collision_mask = _normal_collision_mask
			iframe_left = 0.0
			attacks.cancel()
			current_action_state = ActionState.IDLE
			velocity = Vector2.ZERO
		hp_changed.emit(current_health, max_health)
	return applied
