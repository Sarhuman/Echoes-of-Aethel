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
	var path := "user://tests/settings.cfg"
	DirAccess.remove_absolute(path)
	var hud := SliceHUD.new()
	root.add_child(hud)
	await process_frame
	check(hud.hp.position.y + hud.hp.size.y <= hud.mp.position.y and hud.mp.position.y + hud.mp.size.y <= hud.stamina.position.y,"bars do not overlap")
	var inventory := InventoryModel.new()
	check(inventory.add(&"potion_health_small", 2).ok, "inventory fixture")
	hud.show_inventory_grid(inventory)
	var grids := hud.overlay.find_children("*", "GridContainer", true, false)
	check(grids.size() == 1 and grids[0].get_child_count() == 60, "inventory renders sixty slots")
	check(hud.menu_open, "inventory opens overlay")
	hud.show_menu("Teste","Texto", [["Voltar","resume"]])
	check(hud.menu_open and hud.overlay.visible,"menu appears")
	hud.close_menu()
	check(not hud.menu_open and not hud.overlay.visible,"menu closes")
	hud.reduced_effects = true
	hud.sound_enabled = false
	check(hud.save_options() == OK,"options save")
	hud.queue_free()
	await process_frame
	var restored := SliceHUD.new()
	root.add_child(restored)
	check(restored.reduced_effects and not restored.sound_enabled,"options roundtrip")
	restored.queue_free()
	await process_frame
	var config := ConfigFile.new()
	config.set_value("display","reduced_effects","invalid")
	config.set_value("audio","enabled",17)
	config.save(path)
	var invalid := SliceHUD.new()
	root.add_child(invalid)
	check(not invalid.reduced_effects and invalid.sound_enabled,"invalid options rejected")
	invalid.queue_free()
	await process_frame
	DirAccess.remove_absolute(path)
	var dialogue: DialogueDefinition = load("res://content/dialogues/hub_first_slice.tres")
	check(dialogue.valid(),"typed dialogue catalog valid")
	for id in ["npc.blacksmith","npc.alchemist","npc.elder","npc.gatekeeper","npc.merchant"]:
		check(not DialogueResolver.resolve(id,{}).is_empty(),"essential NPC has fallback "+id)
	finish()
