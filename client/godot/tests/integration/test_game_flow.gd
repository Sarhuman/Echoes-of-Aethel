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
func clean() -> void:
	var saves := root.get_node("SaveSystem")
	for suffix in ["",".bak",".tmp",".corrupt"]: DirAccess.remove_absolute(saves.SAVE_PATH+suffix)
func run() -> void:
	clean()
	var game := preload("res://scenes/game_root.tscn").instantiate()
	root.add_child(game)
	await process_frame
	check(paused and game.ui.menu_open,"title pauses simulation")
	game.start_game(false)
	await physics_frame
	check(game.begun and game.session.zone == "citadel" and paused and game.ui.menu_open,"new campaign shows opening")
	game.command("prologue_next")
	check(game.prologue_index == 1 and paused,"opening advances without moving world")
	game.command("skip_prologue")
	check(not paused and game.session.state.world_flags.get("prologue.last_bell",false),"skipping opening saves objective without reward")
	check(game.begun and game.session.zone == "citadel" and not paused,"new campaign playable")
	await process_frame
	game.ui.update_state(game.player,game.session,{})
	check(game.ui.objective.text.contains("Fale com Irian"),"new journey directs player to first council")
	game.nearest = {"id": "npc.elder", "name": "Irian"}
	game.interact()
	check(paused and game.ui.menu_open and game.council_index == 0,"Irian opens council scene")
	game.command("skip_council")
	check(not paused and game.session.state.world_flags.get("prologue.council",false),"council skip persists once")
	check(game.player.current_health == 172 and game.player.mp == 52,"starts healed")
	check(game.ui.hp.size.y >= 20 and game.ui.hp_value.text.contains("172"),"framed resource meter shows numeric health")
	var shop_state: Dictionary = game.session.snapshot()
	shop_state.inventory.gold = 20
	check(game.session.commit(shop_state,"shop_fixture").ok,"shop fixture persists")
	game.command("shop")
	check(paused and game.ui.menu_open,"merchant menu opens in hub")
	game.command("buy:potion_health_small")
	check(game.session.inventory.gold == 10 and game.session.inventory.counts.get("potion_health_small",0) == 4,"merchant debits gold and delivers item")
	game.command("resume")
	check(game.player.request_attack(false) == AttackController.Status.REJECTED,"hub blocks offense")
	Input.action_press("attack_basic")
	await physics_frame
	await physics_frame
	Input.action_release("attack_basic")
	check(game.ui.message.text.contains("área segura"),"hub explains why combat is blocked")
	check(InputMap.action_get_events("attack_basic").any(func(event): return event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT),"mouse basic attack binding")
	check(not game.session.travel("invalid").ok,"invalid destination refused")
	var original: Vector2 = game.player.position
	Input.action_press("move_right")
	for i in range(8): await physics_frame
	Input.action_release("move_right")
	check(game.player.position.x > original.x,"real input moves player")
	check(game.player.get_node("Visual").walk_clock > 0.0,"movement advances walk animation")
	game.command("pause")
	var position: Vector2 = game.player.position
	Input.action_press("move_right")
	for i in range(3): await physics_frame
	Input.action_release("move_right")
	check(game.player.position == position,"pause freezes physics")
	game.command("resume")
	game.travel("forest_01")
	await physics_frame
	check(game.session.zone == "forest_01" and get_nodes_in_group("enemies").size() == 6,"forest scene and six encounters")
	for enemy in get_nodes_in_group("enemies"): enemy.set_physics_process(false)
	Input.action_press("attack_basic")
	await physics_frame
	await physics_frame
	Input.action_release("attack_basic")
	check(game.player.attacks.phase != CombatService.Phase.IDLE,"basic input starts visible attack")
	game.player.advance_attack(1.0)
	game.player.advance_timers(1.0)
	Input.action_press("skill_1")
	await physics_frame
	await physics_frame
	Input.action_release("skill_1")
	check(game.player.attacks.definition != null and game.player.attacks.definition.attack_id == &"bulwark_impact","skill input starts heavy attack")
	game.player.advance_attack(1.0)
	var first: EnemyActor = get_nodes_in_group("enemies")[0]
	game.player.set_physics_process(false)
	game.player.crit_chance_override = 0
	game.player.global_position = first.global_position - Vector2(30,0)
	game.player.facing = Vector2.RIGHT
	await physics_frame
	await physics_frame
	game.player.request_attack(false)
	game.player.advance_attack(0.2)
	check(first.current_health == 20,"real contact applies formula")
	game.player.advance_attack(0.5)
	game.player.advance_timers(0.9)
	game.player.request_attack(false)
	game.player.advance_attack(0.2)
	check(first.is_dead and game.session.state.encounters.kills.has(String(first.actor_id)),"combat death commits reward")
	var before_xp: int = game.session.state.character.xp
	game.session.grant_kill(String(first.actor_id),first.definition)
	check(game.session.state.character.xp == before_xp,"death replay no extra XP")
	for id in game.session.quest.definition.clues: check(game.session.investigate(id).ok,"clue persists "+id)
	check(game.session.choose("shelter_villagers").ok,"quest through runtime service")
	check(game.session.state.pending_effects.has("first_return_to_citadel"),"pending return event")
	game.travel("citadel")
	await physics_frame
	check(game.session.state.world_flags.get("hub.contamination_observed",false),"return reveals consequence")
	check(get_nodes_in_group("enemies").is_empty(),"hub has no enemies")
	var state_before: Dictionary = game.session.state.snapshot()
	check(game.session.saves.load_game().ok,"reload durable campaign")
	check(game.session.state.quest_choices == state_before.quest_choices,"choice survives reload")
	game.travel("forest_01")
	await physics_frame
	check(game.session.state.encounters.expedition == 2 and game.session.state.encounters.kills.is_empty(),"new expedition resets only encounter ledger")
	for enemy in get_nodes_in_group("enemies"): enemy.set_physics_process(false)
	var boss: EnemyActor = get_nodes_in_group("enemies")[-1]
	check(boss.definition.enemy_id == &"moss_claws","boss exists")
	boss.take_damage(boss.max_health,game.player)
	check(game.session.state.world_flags.get("forest_01.moss_claws.defeated",false),"boss reward and flag durable")
	var living: ActorBase = get_nodes_in_group("enemies")[0]
	game.player.take_damage(game.player.current_health,living)
	await process_frame
	check(paused and game.player.is_dead,"death pauses with recovery action")
	game.command("return")
	await physics_frame
	check(not game.player.is_dead and game.player.current_health == game.player.max_health and game.session.zone == "citadel","death retry heals in hub")
	check(game.session.state.quest_choices.has("act1.spy_hunt"),"death preserves choices")
	game.ui.update_state(game.player,game.session,{})
	check(game.ui.objective.text.contains("Irian"),"completed return directs player to council leader")
	game.nearest = {"id": "npc.elder", "name": "Irian"}
	game.interact()
	check(game.session.state.world_flags.get("hub.council_briefed",false) and game.ui.menu_open,"council conversation recorded")
	game.command("resume")
	game.ui.update_state(game.player,game.session,{})
	check(game.ui.objective.text.contains("PRIMEIRA FATIA CONCLUÍDA"),"briefing completes objective")
	check(game.session.saves.load_game().ok and game.session.state.world_flags.get("hub.council_briefed",false),"council briefing survives reload")
	game.travel("forest_01")
	await physics_frame
	check(get_nodes_in_group("enemies").size() == 5,"defeated sub-boss stays defeated")
	game.command("inventory")
	check(game.ui.menu_open and paused,"inventory operable without console")
	game.command("resume")
	paused = false
	game.queue_free()
	await process_frame
	clean()
	finish()
