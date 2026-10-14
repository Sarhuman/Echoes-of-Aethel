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
	var guard := GuardController.new()
	check(guard.request(1,0,52,true).mp == 44,"guard costs 8")
	check(guard.request(1,0,44,true).status == AttackController.Status.REPLAYED,"guard replay")
	check(guard.reduce_damage(35,true) == 21 and guard.reduce_damage(35,false) == 35,"front only")
	guard.tick(2)
	check(guard.reduce_damage(35,true) == 35,"expires at 2 seconds")
	check(guard.request(2,2,44,true).status == AttackController.Status.REJECTED,"cooldown independent")
	guard.tick(4)
	check(guard.request(3,6,7,true).status == AttackController.Status.REJECTED,"insufficient MP")
	var player := preload("res://entities/player/player.tscn").instantiate() as Player
	root.add_child(player)
	player.input_enabled = false
	player.set_physics_process(false)
	check(player.request_skill(3) == AttackController.Status.ACCEPTED and player.mp == 32,"rift spends 20")
	check(player.attacks.definition.windup == 0.6,"rift telegraph")
	check(player.request_skill(4) == AttackController.Status.REJECTED and player.mp == 32,"reserved free")
	player.advance_attack(1.2)
	player.advance_timers(1.2)
	check(player.request_skill(3) == AttackController.Status.REJECTED,"rift cooldown after animation")
	player.queue_free()
	await process_frame
	finish()
