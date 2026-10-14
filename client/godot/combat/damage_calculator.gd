class_name DamageCalculator
extends RefCounted
static func base_damage(power: int, weapon: int, attribute: int, level: int) -> int:
	if mini(power, mini(weapon, attribute)) < 0 or level < 1: return -1
	return power + weapon + 2 * attribute + 2 * (level - 1)
static func offensive(base: int, bonus: float) -> int:
	if base < 0 or not is_finite(bonus) or bonus < 0 or bonus > 1: return -1
	return floori(base * (1.0 + bonus))
static func critical(value: int, rng: RandomNumberGenerator, chance_bp: int, multiplier: float) -> Dictionary:
	if value < 0 or rng == null or chance_bp < 0 or chance_bp > 10000 or not is_finite(multiplier) or multiplier < 1:
		return {"value": -1, "crit": false}
	var crit := rng.randi_range(1, 10000) <= chance_bp
	return {"value": floori(value * multiplier) if crit else value, "crit": crit}
static func mitigate(value: int, defense: int) -> int:
	if value < 0 or defense < 0: return -1
	return floori(value * 100.0 / (100.0 + defense))
static func final_damage(value: int, resistance_pct: int, temp_reduction: float) -> int:
	if value < 0 or resistance_pct < -50 or resistance_pct > 75 or not is_finite(temp_reduction) or temp_reduction < 0 or temp_reduction > 0.6: return -1
	return maxi(1, floori(floori(value * (100.0 - resistance_pct) / 100.0) * (1.0 - temp_reduction)))
static func compute(spec: Dictionary, rng: RandomNumberGenerator) -> Dictionary:
	var base := base_damage(spec.get("power", 0), spec.get("weapon", 0), spec.get("attribute", 0), spec.get("level", 1))
	var off := offensive(base, spec.get("bonus", 0.0))
	# Validate all later inputs before consuming RNG.
	if final_damage(mitigate(off, spec.get("defense", 0)), spec.get("resistance", 0), spec.get("reduction", 0.0)) < 0:
		return {"final": -1, "crit": false, "nominal": base}
	var hit := critical(off, rng, spec.get("chance_bp", 0), spec.get("multiplier", 1.5))
	return {"final": final_damage(mitigate(hit.value, spec.get("defense", 0)), spec.get("resistance", 0), spec.get("reduction", 0.0)), "crit": hit.crit, "nominal": base}
