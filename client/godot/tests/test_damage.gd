extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func finish() -> void:
	print("%d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
func run() -> void:
	check(DamageCalculator.base_damage(10,8,12,1) == 42,"base GDD")
	check(DamageCalculator.mitigate(42,20) == 35,"physical mitigation")
	check(DamageCalculator.mitigate(63,20) == 52,"critical then defense floors")
	check(DamageCalculator.final_damage(35,0,0.4) == 21,"guard final floor")
	check(DamageCalculator.mitigate(42,5) == 40 and DamageCalculator.mitigate(50,5) == 47 and DamageCalculator.mitigate(62,5) == 59,"wolf damage")
	check(DamageCalculator.final_damage(0,75,0.6) == 1,"minimum damage")
	check(DamageCalculator.final_damage(10,76,0) == -1 and DamageCalculator.final_damage(10,0,NAN) == -1,"invalid reductions")
	check(DamageCalculator.mitigate(42,-1) == -1 and DamageCalculator.offensive(42,INF) == -1,"invalid offense")
	var rng := RandomNumberGenerator.new()
	rng.seed = 1
	check(DamageCalculator.critical(42,rng,10000,1.5).value == 63,"guaranteed crit")
	var stats := CharacterStats.new()
	check(stats.hp_max() == 172 and stats.mp_max() == 52 and stats.defense(2) == 9 and stats.magic_defense(0) == 2,"Bastion level 1")
	stats.level = 2
	check(stats.hp_max() == 178 and stats.mp_max() == 55,"level 2 growth")
	check(stats.crit_chance_bp() == 650,"critical basis points")
	var a := ResourceRegen.new()
	var b := ResourceRegen.new()
	a.notify_combat_event()
	b.notify_combat_event()
	a.notify_mp_spent()
	b.notify_mp_spent()
	var mp_a := 0.0
	var mp_b := 0.0
	for i in range(625): mp_a = a.tick(0.016,mp_a,52,false)
	for i in range(100): mp_b = b.tick(0.1,mp_b,52,false)
	check(is_equal_approx(mp_a,5.0) and is_equal_approx(mp_b,5.0),"regeneration frame independence")
	check(a.tick(9,mp_a,52,true) == mp_a,"hostile engaged blocks whole step")
	var before := a.combat_left
	a.tick(NAN,mp_a,52,false)
	check(a.combat_left == before,"invalid delta no mutation")
	finish()
