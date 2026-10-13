extends SceneTree

func _initialize() -> void:
	call_deferred("run")

func run() -> void:
	var saves := root.get_node("SaveSystem")
	var state := root.get_node("GameState")
	var result: Dictionary = saves.load_game()
	var valid: bool = result["ok"] and state.progress["map"] == 4 and state.world_flags.get("welcomed", false)
	quit(0 if valid else 1)
