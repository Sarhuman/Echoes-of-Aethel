class_name GuardController
extends RefCounted
var definition: AttackDefinition = preload("res://content/attacks/guard.tres")
var active_left := 0.0
var cooldown_left := 0.0
var _last_sequence := 0
var _accepted: Dictionary = {}
func request(seq: int, _now: float, mp: float, can_act: bool) -> Dictionary:
	if not is_finite(_now) or _now < 0: return {"status":AttackController.Status.REJECTED,"mp":mp}
	if _accepted.has(seq): return {"status": AttackController.Status.REPLAYED, "mp": mp}
	if seq <= _last_sequence or not is_finite(mp) or mp < definition.mp_cost or not can_act or cooldown_left > 0:
		return {"status": AttackController.Status.REJECTED, "mp": mp}
	_last_sequence = seq
	_accepted[seq] = true
	if _accepted.size() > 128: _accepted.erase(_accepted.keys()[0])
	active_left = definition.effect_duration
	cooldown_left = definition.cooldown
	return {"status": AttackController.Status.ACCEPTED, "mp": mp - definition.mp_cost}
func tick(delta: float) -> void:
	if not is_finite(delta) or delta < 0: return
	active_left = maxf(0, active_left - delta)
	cooldown_left = maxf(0, cooldown_left - delta)
func reduce_damage(amount: int, hit_from_front: bool) -> int:
	if amount <= 0: return amount
	return maxi(1, floori(amount * (1.0 - definition.reduction))) if active_left > 0 and hit_from_front else amount
