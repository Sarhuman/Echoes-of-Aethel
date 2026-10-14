extends SceneTree
func _initialize() -> void: call_deferred("run")
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
	paused = false
	game.queue_free()
	await process_frame
	quit()
