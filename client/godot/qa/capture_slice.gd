extends SceneTree
func _initialize() -> void:
	if "--test-harness" not in OS.get_cmdline_user_args():
		push_error("Captura bloqueada: execute com -- --test-harness para isolar o save.")
		quit(2)
		return
	call_deferred("run")
func capture(name: String) -> void:
	for i in range(8): await process_frame
	await RenderingServer.frame_post_draw
	var image := root.get_texture().get_image()
	image.save_png(ProjectSettings.globalize_path("res://../../test-results/" + name + ".png"))
func run() -> void:
	root.size = Vector2i(1280,720)
	DirAccess.make_dir_recursive_absolute(ProjectSettings.globalize_path("res://../../test-results"))
	var game := preload("res://scenes/game_root.tscn").instantiate()
	root.add_child(game)
	await capture("title")
	game.start_game(false)
	await capture("prologue")
	game.command("skip_prologue")
	game.nearest = {"id": "npc.elder", "name": "Irian"}
	game.interact()
	await capture("council")
	game.command("skip_council")
	await capture("citadel")
	game.travel("forest_01")
	game.player.position = Vector2(970,700)
	game.player.get_node("Camera2D").reset_smoothing()
	await capture("forest")
	game.player.set_physics_process(false)
	game.player.facing = Vector2.RIGHT
	game.player.request_attack(false)
	game.player.advance_attack(0.18)
	await capture("attack")
	game.player.attacks.cancel()
	game.player.set_physics_process(true)
	game.player.position = Vector2(1510,260)
	game.player.get_node("Camera2D").reset_smoothing()
	await capture("arena")
	game.command("inventory")
	await capture("inventory")
	game.command("resume")
	game.travel("citadel")
	await process_frame
	var candidate: Dictionary = game.session.snapshot()
	candidate.inventory.gold = 25
	game.session.commit(candidate,"capture_fixture")
	game.ui.update_state(game.player,game.session,{})
	game.command("shop")
	await capture("merchant")
	paused = false
	game.queue_free()
	await process_frame
	quit()
