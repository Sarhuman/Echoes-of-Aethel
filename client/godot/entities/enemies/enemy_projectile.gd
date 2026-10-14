class_name EnemyProjectile
extends Node2D
var source: ActorBase
var direction := Vector2.RIGHT
var spec: EnemyDefinition
var lifetime := 0.0
func _ready() -> void: lifetime = spec.projectile_lifetime
func _physics_process(delta: float) -> void:
	if not is_instance_valid(source) or source.is_dead:
		queue_free()
		return
	lifetime -= delta
	var next := global_position + Vector2(direction.x, direction.y * 0.5) * spec.projectile_speed * delta
	var query := PhysicsRayQueryParameters2D.create(global_position, next, 3)
	query.exclude = [source.get_rid()]
	var hit := get_world_2d().direct_space_state.intersect_ray(query)
	if not hit.is_empty():
		var target := hit.collider as Player
		if target != null:
			var applied := target.take_damage(DamageCalculator.mitigate(spec.attack_power, target.magic_defense), source)
			if applied: target.slow_left = 1.2
		queue_free()
	global_position = next
	if lifetime <= 0: queue_free()
func _draw() -> void:
	draw_circle(Vector2.ZERO, 6, Color("#a17ac4"))
	draw_circle(Vector2.ZERO, 2, Color("#eee0ff"))
	draw_line(-direction * 15, Vector2.ZERO, Color("#674a90"), 3)
