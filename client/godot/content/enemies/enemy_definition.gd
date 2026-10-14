class_name EnemyDefinition
extends Resource
@export var enemy_id: StringName
@export var display_name := ""
@export var category: StringName = &"common"
@export var max_health := 60
@export var defense := 5
@export var attack_power := 18
@export var damage_type: StringName = &"physical"
@export var base_xp := 10
@export var movement_speed := 120.0
@export var detection_range := 200.0
@export var attack_range := 40.0
@export var attack_cooldown := 1.5
@export var windup_duration := 0.7
@export var recovery_duration := 0.9
@export var charge_speed := 300.0
@export var charge_duration := 0.45
@export var projectile_speed := 180.0
@export var projectile_lifetime := 3.0
@export var sweep_power := 32
@export var charge_power := 40
@export var roots_power := 24
@export var sweep_windup := 0.9
@export var charge_windup := 1.1
@export var roots_windup := 1.2
@export var roots_half_width := 130.0
@export var roots_depth := 80.0
@export var corridor_half_width := 26.0
@export var tags: Array[StringName] = []
func valid() -> bool:
	if not WorldSaveSchema.valid_id(String(enemy_id)) or category not in [&"common", &"sub_boss", &"boss", &"elite"] or damage_type not in [&"physical", &"arcane"]: return false
	for number in [max_health, attack_power, movement_speed, detection_range, attack_range, attack_cooldown, windup_duration, recovery_duration]:
		if not is_finite(float(number)) or number <= 0: return false
	return defense >= 0 and base_xp >= 0
