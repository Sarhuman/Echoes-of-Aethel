class_name AttackDefinition
extends Resource
## Content-owned parameters. Controllers copy the resource on acceptance.
@export var attack_id: StringName = &"basic"
@export var heavy := false
@export var windup := 0.15
@export var active := 0.08
@export var recovery := 0.25
@export var mp_cost := 0.0
@export var stamina_cost := 0.0
@export var power := 10
@export var scale_attribute: StringName = &"STR"
@export var interrupt_seconds := 0.0
@export var cooldown := 0.9
@export var effect_duration := 0.0
@export var reduction := 0.0
@export var line_width := 0.0
@export var reach := 56.0

func valid() -> bool:
	for value in [windup, active, recovery, reach]:
		if not is_finite(value) or value <= 0.0:
			return false
	for value in [mp_cost, stamina_cost, interrupt_seconds, cooldown, effect_duration, reduction, line_width]:
		if not is_finite(value) or value < 0.0:
			return false
	return WorldSaveSchema.valid_id(String(attack_id)) and power > 0 and reduction <= 0.6 and is_finite(windup + active + recovery)

func signature(direction: Vector2) -> Array:
	return [attack_id, heavy, windup, active, recovery, mp_cost, stamina_cost, power, reach, cooldown, interrupt_seconds, line_width, direction]
