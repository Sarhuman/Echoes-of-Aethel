extends SceneTree
var checks := 0
var failures := 0
var calls := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func damage_callback() -> bool:
	calls += 1
	return true
func run() -> void:
	var basic := load("res://combat/basic.tres") as AttackDefinition
	var heavy := load("res://combat/heavy.tres") as AttackDefinition
	var controller := AttackController.new()
	check(not controller.advance(0.5)["active"], "idle never creates active window")
	check(not CombatService.spend(10, 20, -1, 0)["ok"], "negative cost rejected")
	check(not CombatService.spend(NAN, 20, 0, 0)["ok"], "invalid balance rejected even with free action")
	check(CombatService.spend(10, 20, 10, 20) == {"ok": true, "mp": 0.0, "stamina": 0.0}, "exact combined payment")
	check(CombatService.spend(10, 20, 11, 1) == {"ok": false, "mp": 10.0, "stamina": 20.0}, "insufficient payment unchanged")
	var r := controller.request(1, heavy, Vector2.RIGHT, 40, 100, true)
	check(r["status"] == AttackController.Status.ACCEPTED and r["mp"] == 28, "heavy accepted and costs candidate 12")
	check(controller.advance(0.0)["ok"] and controller.phase == CombatService.Phase.WINDUP, "zero delta preserves windup")
	check(not controller.advance(INF)["ok"] and controller.elapsed == 0, "invalid delta preserves state")
	r = controller.advance(0.5)
	check(r["active"] and controller.phase == CombatService.Phase.RECOVERY and is_equal_approx(controller.elapsed - 0.43, 0.07), "500ms spans active into recovery")
	check(is_equal_approx(r["active_start"], 0.35) and is_equal_approx(r["active_end"], 0.43), "active interval offsets in step")
	check(controller.resolve_hit(1, damage_callback), "first hit after crossed active")
	check(not controller.resolve_hit(1, damage_callback) and calls == 1, "one hit per attack")
	controller.close_window()
	check(not controller.resolve_hit(2, damage_callback), "closed window cannot hit")
	controller.advance(0.31)
	check(controller.phase == CombatService.Phase.IDLE and controller.can_dash(), "completion restores idle permissions")
	controller.request(2, basic, Vector2.RIGHT, 28, 100, true)
	r = controller.request(1, heavy, Vector2.RIGHT, 28, 100, true)
	check(r["status"] == AttackController.Status.REPLAYED and r["mp"] == 28 and controller.instance_id == 2, "A B A replay never restarts A")
	check(controller.request(2, basic, Vector2.LEFT, 28, 100, true)["status"] == AttackController.Status.REJECTED, "changed direction conflicts")
	controller.advance(0.16)
	check(controller.resolve_hit(9, func(): return false) == false, "failed application not committed")
	check(controller.resolve_hit(9, damage_callback), "failed application can retry")
	check(controller.resolve_hit(10, func():
		check(not controller.resolve_hit(10, damage_callback), "reentrant hit refused")
		return true), "outer hit succeeds")
	controller.cancel()
	check(not controller.resolve_hit(11, damage_callback), "cancel invalidates open window")
	for seq in range(3, 140):
		controller.request(seq, basic, Vector2.RIGHT, 28, 100, true)
		controller.cancel()
	check(controller._history.size() == 128 and controller.request(1, heavy, Vector2.RIGHT, 28, 100, true)["status"] == AttackController.Status.REJECTED, "bounded history rejects evicted IDs")
	check(not CombatService.in_cone(Vector2.RIGHT, Vector2(-20, 0), 56), "rear target excluded")
	check(CombatService.in_cone(Vector2.DOWN, Vector2(0, 28), 56) and not CombatService.in_cone(Vector2.DOWN, Vector2(0, 29), 56), "reach uses inverse isometric projection")
	var invalid := basic.duplicate() as AttackDefinition
	invalid.active = NAN
	check(not invalid.valid(), "nonfinite duration rejected")
	var boundary := AttackController.new()
	boundary.request(1, heavy, Vector2.RIGHT, 40, 100, true)
	check(not boundary.advance(0.35)["active"], "touching active start yields no duration")
	check(boundary.advance(0.08)["active"], "full active interval processed")
	check(not boundary.advance(0.01)["active"], "recovery boundary cannot repeat active")

	var scene := load("res://scenes/test_scene.tscn").instantiate() as Node2D
	root.add_child(scene)
	var player := scene.get_node("Player") as Player
	var enemy := scene.get_node("Enemy") as ActorBase
	player.input_enabled = false
	player.set_physics_process(false)
	enemy.set_physics_process(false)
	scene.get_node("Enemy/Brain").set_physics_process(false)
	player.global_position = Vector2(300, 360)
	enemy.global_position = Vector2(335, 360)
	await physics_frame
	await physics_frame
	check(player.request_attack(false, 1) == AttackController.Status.ACCEPTED, "scene basic accepted")
	player.advance_attack(0.16)
	check(enemy.current_health == 88, "real physics query applies basic damage")
	player.advance_attack(0.02)
	check(enemy.current_health == 88, "overlapping body not hit twice")
	player.advance_attack(0.06)
	check(player.attempt_dash(), "basic recovery cancels with paid dash")
	player.advance_timers(1.2)
	check(player.request_attack(true, 2) == AttackController.Status.ACCEPTED and player.mp == 28, "scene heavy commits MP")
	check(not player.attempt_dash(), "heavy windup blocks dash")
	player.advance_attack(0.5)
	check(enemy.current_health == 64, "crossed active applies one heavy hit")
	player.advance_attack(0.31)
	player.global_position = Vector2(415, 360)
	enemy.global_position = Vector2(465, 360)
	await physics_frame
	await physics_frame
	player.request_attack(true, 3)
	player.advance_attack(0.5)
	check(enemy.current_health == 64, "wall occludes attack")
	player.attacks.cancel()
	player._sync_attack_state()
	player.zone_id = &"citadel"
	enemy.zone_id = &"citadel"
	check(not enemy.take_damage(10, player) and not player.take_damage(10, enemy), "citadel blocks damage both directions")
	check(player.request_attack(false, 4) == AttackController.Status.REJECTED, "citadel refuses attack")
	player.zone_id = &"field"
	enemy.zone_id = &"field"
	player.request_attack(false, 5)
	player.current_health = 1
	check(player.take_damage(10, enemy) and player.is_dead, "death during attack")
	player.advance_attack(0.5)
	check(player.attacks.phase == CombatService.Phase.IDLE and enemy.current_health == 64, "death clears attack and future hits")
	check(player.request_attack(false, 6) == AttackController.Status.REJECTED, "dead cannot start attack")
	scene.queue_free()
	await process_frame
	print("Combat checks: %d; failures: %d" % [checks, failures])
	quit(1 if failures else 0)
