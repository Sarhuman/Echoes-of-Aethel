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
@export var damage := 12
@export var reach := 56.0

func valid() -> bool:
	for value in [windup, active, recovery, reach]:
		if not is_finite(value) or value <= 0.0:
			return false
	for value in [mp_cost, stamina_cost]:
		if not is_finite(value) or value < 0.0:
			return false
	return attack_id != &"" and damage > 0 and is_finite(windup + active + recovery)

func signature(direction: Vector2) -> Array:
	return [attack_id, heavy, windup, active, recovery, mp_cost, stamina_cost, damage, reach, direction]
