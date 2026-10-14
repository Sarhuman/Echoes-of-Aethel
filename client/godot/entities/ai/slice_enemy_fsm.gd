class_name SliceEnemyFSM
extends EnemyFSM
var spec: EnemyDefinition
var phase := "idle"
var remaining := 0.0
var locked_position := Vector2.ZERO
var locked_direction := Vector2.RIGHT
var pattern := 0
var attack_number := 0
var interrupted := 0.0
var struck := false
var phase_two := false
var roots_side := 1
func _ready() -> void:
	set_physics_process(false)
func interrupt(seconds: float) -> void:
	if spec.category != &"common": return
	interrupted = seconds
	phase = "recovery"
	remaining = spec.recovery_duration
	actor.velocity = Vector2.ZERO
func logical_offset(to: Vector2) -> Vector2:
	var offset := to - actor.global_position
	return Vector2(offset.x, offset.y * 2)
func step(delta: float, player: Player) -> void:
	actor.velocity = Vector2.ZERO
	if actor.is_dead or actor.zone_id == &"citadel" or not actor.combat_enabled:
		current_state = State.DEAD if actor.is_dead else State.IDLE
		phase = "idle"
		return
	if not is_instance_valid(player) or player.is_dead or player.zone_id != actor.zone_id: return
	target_actor = player
	if interrupted > 0:
		interrupted = maxf(0, interrupted - delta)
		return
	if spec.category == &"sub_boss" and not phase_two and actor.current_health < actor.max_health * 0.5:
		phase_two = true
		phase = "recovery"
		remaining = spec.recovery_duration
	attack_timer = maxf(0, attack_timer - delta)
	if phase == "windup":
		remaining -= delta
		if remaining <= 0: execute(player)
	elif phase == "charge":
		remaining -= delta
		actor.velocity = Vector2(locked_direction.x, locked_direction.y * 0.5) * spec.charge_speed
		if not struck and logical_offset(player.global_position).length() < 30:
			hit_player(player, spec.charge_power if spec.category == &"sub_boss" else spec.attack_power)
			struck = true
		if remaining <= 0 or actor.get_slide_collision_count() > 0:
			phase = "recovery"
			remaining = spec.recovery_duration
			actor.velocity = Vector2.ZERO
	elif phase == "recovery":
		remaining -= delta
		if remaining <= 0: phase = "idle"
	else:
		var distance := logical_offset(player.global_position).length()
		if distance > spec.detection_range * 1.5 or not _line_of_sight(player):
			current_state = State.IDLE
			return
		if distance <= spec.attack_range and attack_timer <= 0:
			current_state = State.ATTACK
			phase = "windup"
			locked_position = player.global_position
			locked_direction = logical_offset(locked_position).normalized()
			pattern = attack_number % (3 if phase_two else 2) if spec.category == &"sub_boss" else 0
			if pattern == 2: roots_side = -roots_side
			remaining = windup_for(pattern)
			attack_number += 1
			attack_telegraphed.emit(player.actor_id, locked_position)
		elif distance > spec.attack_range * 0.85:
			current_state = State.CHASE
			var direction := logical_offset(player.global_position).normalized()
			actor.velocity = Vector2(direction.x, direction.y * 0.5) * spec.movement_speed
func windup_for(kind: int) -> float:
	if spec.category != &"sub_boss": return spec.windup_duration
	return [spec.sweep_windup, spec.charge_windup, spec.roots_windup][kind]
func roots_hit(point: Vector2) -> bool:
	var offset := point - locked_position
	return offset.x * roots_side > spec.corridor_half_width and absf(offset.x) < spec.roots_half_width and absf(offset.y) < spec.roots_depth
func execute(player: Player) -> void:
	attack_timer = spec.attack_cooldown
	struck = false
	if spec.enemy_id == &"blind_cultist":
		if _line_of_sight(player):
			var shot := EnemyProjectile.new()
			shot.source = actor
			shot.spec = spec
			shot.direction = locked_direction
			actor.get_parent().add_child(shot)
			shot.global_position = actor.global_position + Vector2(locked_direction.x, locked_direction.y * 0.5) * 18
		phase = "recovery"
		remaining = spec.recovery_duration
	elif spec.category == &"sub_boss" and pattern != 1:
		if pattern == 0 and CombatService.in_cone(locked_direction, player.global_position - actor.global_position, spec.attack_range + 15) and _line_of_sight(player):
			hit_player(player, spec.sweep_power)
		elif pattern == 2 and roots_hit(player.global_position):
			hit_player(player, spec.roots_power)
		phase = "recovery"
		remaining = spec.recovery_duration
	else:
		phase = "charge"
		remaining = spec.charge_duration
func hit_player(player: Player, power: int) -> void:
	var defense_value := player.magic_defense if spec.damage_type == &"arcane" else player.defense
	player.take_damage(DamageCalculator.mitigate(power, defense_value), actor)
