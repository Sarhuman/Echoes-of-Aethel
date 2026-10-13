class_name AttackController
extends RefCounted
## One owner per actor lifetime. Sequence numbers never reset during that lifetime.
enum Status { ACCEPTED, REPLAYED, REJECTED }
const HISTORY_LIMIT := 128
var phase := CombatService.Phase.IDLE
var elapsed := 0.0
var definition: AttackDefinition
var direction := Vector2.RIGHT
var instance_id := 0
var _high_water := 0
var _history: Dictionary = {}
var _targets: Dictionary = {}
var _window_open := false
var _resolving := false

func request(sequence: int, spec: AttackDefinition, forward: Vector2, mp: float, stamina: float, can_act: bool) -> Dictionary:
	var rejected := {"status": Status.REJECTED, "mp": mp, "stamina": stamina}
	if sequence <= 0 or spec == null or not spec.valid() or not forward.is_finite() or forward.is_zero_approx():
		return rejected
	var fingerprint := spec.signature(forward.normalized())
	if _history.has(sequence):
		var record: Dictionary = _history[sequence]
		if record["signature"] == fingerprint and record["accepted"]:
			return {"status": Status.REPLAYED, "mp": mp, "stamina": stamina}
		return rejected
	if sequence <= _high_water or _resolving:
		return rejected
	var candidate := CombatService.spend(mp, stamina, spec.mp_cost, spec.stamina_cost)
	var accepted: bool = can_act and phase == CombatService.Phase.IDLE and candidate["ok"]
	_high_water = sequence
	_history[sequence] = {"signature": fingerprint, "accepted": accepted}
	if _history.size() > HISTORY_LIMIT:
		_history.erase(_history.keys()[0]) # High-water mark rejects evicted commands.
	if not accepted:
		return rejected
	definition = spec.duplicate() as AttackDefinition
	direction = forward.normalized()
	instance_id = sequence
	elapsed = 0.0
	phase = CombatService.Phase.WINDUP
	_targets.clear()
	_window_open = false
	# Caller commits candidate balances synchronously, before signals/callbacks.
	return {"status": Status.ACCEPTED, "mp": candidate["mp"], "stamina": candidate["stamina"]}

func advance(delta: float) -> Dictionary:
	if _resolving:
		return {"ok": false, "active": false}
	_window_open = false
	if not is_finite(delta) or delta < 0.0:
		return {"ok": false, "active": false}
	if phase == CombatService.Phase.IDLE:
		return {"ok": true, "active": false}
	var result := CombatService.advance(elapsed, delta, definition)
	if result["ok"]:
		elapsed = result["time"]
		phase = result["phase"]
		_window_open = result["active"]
	return result

func resolve_hit(target_id: int, apply_damage: Callable) -> bool:
	if not _window_open or _resolving or target_id <= 0 or _targets.has(target_id):
		return false
	_targets[target_id] = true # Reserve before callbacks can re-enter.
	_resolving = true
	var current_instance := instance_id
	var applied: bool = apply_damage.call()
	_resolving = false
	if not applied and current_instance == instance_id:
		_targets.erase(target_id)
	return applied

func close_window() -> void:
	_window_open = false

func cancel() -> void:
	phase = CombatService.Phase.IDLE
	elapsed = 0.0
	_window_open = false
	_targets.clear()
	instance_id = 0

func can_dash() -> bool:
	return phase == CombatService.Phase.IDLE or (phase == CombatService.Phase.RECOVERY and not definition.heavy)

func movement_scale() -> float:
	match phase:
		CombatService.Phase.WINDUP: return 0.0 if definition.heavy else 0.2
		CombatService.Phase.ACTIVE: return 0.0 if definition.heavy else 0.5
		CombatService.Phase.RECOVERY: return 0.5 if definition.heavy else 0.8
	return 1.0
