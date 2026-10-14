class_name ResourceRegen
extends RefCounted
var intelligence := 3
var combat_left := 0.0
var spent_left := 0.0
func notify_combat_event() -> void: combat_left = 5.0
func notify_mp_spent() -> void: spent_left = 3.0
func tick(delta: float, mp: float, mp_max: float, in_combat: bool, spent_mp_recently: bool = false) -> float:
	if not is_finite(delta) or delta < 0 or not is_finite(mp) or not is_finite(mp_max) or mp < 0 or mp > mp_max: return mp
	if in_combat: notify_combat_event()
	if spent_mp_recently: notify_mp_spent()
	var blocked := maxf(combat_left, spent_left)
	combat_left = maxf(0, combat_left - delta)
	spent_left = maxf(0, spent_left - delta)
	var eligible := 0.0 if in_combat or spent_mp_recently else maxf(0, delta - blocked)
	return minf(mp_max, mp + eligible * (1 + floori(intelligence / 10.0)))
