class_name CombatService
extends RefCounted
## Pure calculations: no nodes, signals, saves or mutable caller dictionaries.
enum Phase { IDLE, WINDUP, ACTIVE, RECOVERY }

static func spend(mp: float, stamina: float, mp_cost: float, stamina_cost: float) -> Dictionary:
	var result := {"ok": false, "mp": mp, "stamina": stamina}
	for value in [mp, stamina, mp_cost, stamina_cost]:
		if not is_finite(value) or value < 0.0:
			return result
	if mp >= mp_cost and stamina >= stamina_cost:
		result = {"ok": true, "mp": mp - mp_cost, "stamina": stamina - stamina_cost}
	return result

static func phase_at(time: float, definition: AttackDefinition) -> Phase:
	if time < definition.windup:
		return Phase.WINDUP
	if time < definition.windup + definition.active:
		return Phase.ACTIVE
	if time < definition.windup + definition.active + definition.recovery:
		return Phase.RECOVERY
	return Phase.IDLE

static func advance(time: float, delta: float, definition: AttackDefinition) -> Dictionary:
	if definition == null or not definition.valid() or not is_finite(time) or time < 0.0 or not is_finite(delta) or delta < 0.0 or not is_finite(time + delta):
		return {"ok": false}
	var end := time + delta
	var start_active := maxf(time, definition.windup)
	var end_active := minf(end, definition.windup + definition.active)
	return {"ok": true, "time": end, "phase": phase_at(end, definition),
		"active": end_active > start_active,
		"active_start": start_active - time, "active_end": end_active - time}

static func in_cone(forward: Vector2, screen_offset: Vector2, reach: float) -> bool:
	if not forward.is_finite() or not screen_offset.is_finite() or forward.is_zero_approx() or not is_finite(reach) or reach <= 0.0:
		return false
	# Inverse presentation projection: combat distances live in logical pixels.
	var offset := Vector2(screen_offset.x, screen_offset.y * 2.0)
	if not offset.is_finite() or offset.length() > reach:
		return false
	return offset.is_zero_approx() or forward.normalized().dot(offset.normalized()) >= 0.5 - 0.000001
