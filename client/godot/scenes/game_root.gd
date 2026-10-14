extends Node2D
var session: SliceSession
var ui: SliceHUD
var world: Node2D
var map: SliceMap
var player: Player
var begun := false
var handling_death := false
var nearest: Dictionary = {}
var audio_director: AethelAudioDirector
var combat_text: AethelCombatText
var impact_feedback: AethelImpactFeedback
var prologue_index := 0
var council_index := 0
const PROLOGUE_PAGES := [
	["O ÚLTIMO SINO", "Narrador", "Aethel aprendeu a medir o tempo pela luz do Éter. Quando o Coração se partiu, os relógios continuaram. Foi o mundo que perdeu o ritmo."],
	["O ÚLTIMO SINO", "Narrador", "A Bruma ergueu os mortos, mudou as feras e ensinou os vivos a temer a água. Atrás destes portões, um fragmento ainda respira."],
	["A PORTA DA CIDADELA", "Maera", "Guarde essa mão. Aqui dentro, qualquer brilho vira promessa."],
	["A PORTA DA CIDADELA", "Bastião", "Conheço estes portões. Foi o lado de fora que mudou."],
	["A PORTA DA CIDADELA", "Maera", "Então entre. O conselho está junto ao cristal. Se oferecerem respostas fáceis, olhe para as mãos de quem fala."]
]
const COUNCIL_PAGES := [
	["O CONSELHO DOS QUE FICARAM", "Irian", "Cinco Santuários mantinham o Éter no seu leito. Vael tentou abrir o Coração durante o eclipse. Agora temos um fragmento e quatro ausências."],
	["O CONSELHO DOS QUE FICARAM", "Lyra", "O cristal afasta a Bruma dos portões. Não a tira do que já trouxeram para dentro. Preciso de água limpa, não de outra promessa."],
	["O CONSELHO DOS QUE FICARAM", "Maera", "Há gente desaparecendo no Bosque. Se mandarmos toda a guarda, deixamos estes portões sem ninguém. Se não mandarmos ninguém, a cidade acaba por dentro."],
	["O CONSELHO DOS QUE FICARAM", "Irian", "Silvano guarda uma parte do Coração. Não lhe pedimos que salve tudo hoje. Comece pela estrada."]
]
func _ready() -> void:
	var left_click := InputEventMouseButton.new()
	left_click.button_index = MOUSE_BUTTON_LEFT
	InputMap.action_add_event("attack_basic", left_click)
	var right_click := InputEventMouseButton.new()
	right_click.button_index = MOUSE_BUTTON_RIGHT
	InputMap.action_add_event("skill_1", right_click)
	session = SliceSession.new()
	add_child(session)
	world = Node2D.new()
	world.name = "World"
	add_child(world)
	combat_text = AethelCombatText.new()
	add_child(combat_text)
	impact_feedback = AethelImpactFeedback.new()
	add_child(impact_feedback)
	ui = SliceHUD.new()
	add_child(ui)
	audio_director = AethelAudioDirector.new()
	add_child(audio_director)
	audio_director.enabled = ui.sound_enabled
	ui.command.connect(command)
	session.notice.connect(ui.show_notice)
	get_node("/root/EventBus").actor_died.connect(actor_died)
	get_node("/root/EventBus").actor_damaged.connect(show_damage)
	var preview := preload("res://world/maps/citadel.tscn").instantiate()
	world.add_child(preview)
	var camera := Camera2D.new()
	camera.position = Vector2(790,435)
	world.add_child(camera)
	show_title()
func show_title() -> void:
	get_tree().paused = true
	var buttons: Array = []
	if session.saves.has_campaign(): buttons.append(["Continuar jornada","continue"])
	buttons.append_array([["Nova jornada","new"],["Controles","controls"],["Opções","options"],["Sair","quit"]])
	ui.show_menu("ECHOES OF AETHEL","O caminho para o inferno está cheio de boas intenções.\n\nBastião • Cidadela • Bosque das Névoas\nUma primeira jornada jogável.",buttons)
func start_game(continue_game: bool) -> void:
	var result := session.start(continue_game)
	if not result.ok:
		ui.show_notice("Não foi possível abrir a jornada: " + result.code)
		return
	begun = true
	var travel := session.travel("citadel")
	if not travel.ok:
		ui.show_notice("Falha no retorno: " + travel.code)
		return
	load_zone()
	resume()
	if not continue_game and not session.state.world_flags.get("prologue.last_bell", false):
		prologue_index = 0
		show_prologue_page()
func show_prologue_page() -> void:
	get_tree().paused = true
	var page: Array = PROLOGUE_PAGES[prologue_index]
	ui.show_cinematic(page[0], page[1], page[2], prologue_index + 1, PROLOGUE_PAGES.size())
func finish_prologue() -> void:
	var candidate := session.snapshot()
	candidate.world_flags["prologue.last_bell"] = true
	var saved := session.commit(candidate, "prologue")
	if saved.ok: resume()
func show_council_page() -> void:
	get_tree().paused = true
	var page: Array = COUNCIL_PAGES[council_index]
	ui.show_cinematic(page[0], page[1], page[2], council_index + 1, COUNCIL_PAGES.size(), "council_next", "skip_council")
func finish_council() -> void:
	var candidate := session.snapshot()
	candidate.world_flags["prologue.council"] = true
	var saved := session.commit(candidate, "prologue_council")
	if saved.ok: resume()
func load_zone() -> void:
	for node in world.get_children():
		world.remove_child(node)
		node.queue_free()
	map = load("res://world/maps/" + session.zone + ".tscn").instantiate()
	map.flags = session.state.world_flags.duplicate(true)
	world.add_child(map)
	player = preload("res://entities/player/player.tscn").instantiate()
	player.zone_id = StringName(session.zone)
	world.add_child(player)
	player.position = map.spawn
	session.player = player
	session.apply_player()
	var camera := player.get_node("Camera2D") as Camera2D
	impact_feedback.set_camera(camera)
	impact_feedback.enabled = not ui.reduced_effects
	camera.limit_left = 0
	camera.limit_top = 0
	camera.limit_right = int(map.map_size.x)
	camera.limit_bottom = int(map.map_size.y)
	camera.reset_smoothing()
	player.attack_started.connect(audio_director.play_effect)
	player.attack_missed.connect(combat_text.show_miss)
	player.dash_started.connect(func(): audio_director.play_effect(&"dash"))
	audio_director.set_context(session.zone, false)
	player.action_blocked.connect(ui.show_notice)
	player.skill_unavailable.connect(func(_slot): ui.show_notice("Este slot ainda não possui habilidade."))
	if session.zone == "forest_01": spawn_enemies()
	handling_death = false
func show_damage(id: String, amount: int, _source: String) -> void:
	audio_director.play_effect(&"hit")
	impact_feedback.impulse(5.0 if id == String(player.actor_id) else 3.0)
	for actor in get_tree().get_nodes_in_group("enemies") + get_tree().get_nodes_in_group("player"):
		if is_instance_valid(actor) and String(actor.actor_id) == id:
			combat_text.show_damage(actor.global_position, amount)
			return
func spawn_enemies() -> void:
	var spawns: Array = [
		["ethereal_wolf",Vector2(520,790)],["ethereal_wolf",Vector2(700,690)],
		["blind_cultist",Vector2(820,580)],["ethereal_wolf",Vector2(1250,470)],
		["blind_cultist",Vector2(1410,370)],["moss_claws",Vector2(1600,220)]
	]
	for i in range(spawns.size()):
		var id: String = spawns[i][0]
		if id == "moss_claws" and session.state.world_flags.get("forest_01.moss_claws.defeated",false): continue
		var enemy := load("res://entities/enemies/" + id + ".tscn").instantiate() as EnemyActor
		enemy.actor_id = StringName("forest_01.%s.%02d:%d" % [id,i,session.state.encounters.expedition])
		enemy.zone_id = &"forest_01"
		world.add_child(enemy)
		enemy.position = spawns[i][1]
func _process(_delta: float) -> void:
	if not begun or not is_instance_valid(player): return
	nearest = map.nearest(player.global_position)
	var engaged := false
	for node in get_tree().get_nodes_in_group("enemies"):
		var enemy := node as EnemyActor
		if not enemy.is_dead and enemy.brain.current_state in [EnemyFSM.State.ATTACK,EnemyFSM.State.CHASE]: engaged = true
	player.engaged = engaged
	audio_director.set_context(session.zone, engaged)
	ui.update_state(player,session,nearest)
func actor_died(id: String) -> void:
	if not begun: return
	if id == String(player.actor_id) and not handling_death:
		handling_death = true
		call_deferred("death_menu")
		return
	for node in get_tree().get_nodes_in_group("enemies"):
		if String(node.actor_id) == id:
			session.grant_kill(id,node.definition)
			break
func death_menu() -> void:
	get_tree().paused = true
	ui.show_menu("A luz ainda respira","Você caiu no Bosque. Suas decisões e recompensas confirmadas permanecem. A Cidadela oferece abrigo.",[["Voltar à Cidadela","return"]])
func resume() -> void:
	if not begun:
		show_title()
		return
	if player.is_dead:
		death_menu()
		return
	get_tree().paused = false
	ui.close_menu()
func travel(destination: String) -> void:
	var result := session.travel(destination)
	if result.ok:
		load_zone()
		resume()
	else:
		ui.show_notice("Transição não concluída: " + result.code)
func command(action: String) -> void:
	if action == "new":
		if session.saves.has_campaign():
			ui.show_menu("Uma nova jornada","Iniciar substitui a campanha atual; o save anterior será mantido no backup.",[["Iniciar nova campanha","confirm_new"],["Voltar","title"]])
		else: start_game(false)
	elif action == "confirm_new": start_game(false)
	elif action == "prologue_next":
		prologue_index += 1
		if prologue_index >= PROLOGUE_PAGES.size(): finish_prologue()
		else: show_prologue_page()
	elif action == "skip_prologue": finish_prologue()
	elif action == "council_next":
		council_index += 1
		if council_index >= COUNCIL_PAGES.size(): finish_council()
		else: show_council_page()
	elif action == "skip_council": finish_council()
	elif action == "continue": start_game(true)
	elif action == "title": show_title()
	elif action == "quit": get_tree().quit()
	elif action == "resume": resume()
	elif action == "return": travel("citadel")
	elif action == "pause":
		get_tree().paused = true
		ui.show_menu("Um instante de silêncio","A simulação está pausada.",[["Continuar","resume"],["Salvar jornada","save"],["Inventário","inventory"],["Diário","journal"],["Controles","controls"],["Opções","options"],["Sair do jogo","save_quit"]])
	elif action == "save" or action == "save_quit":
		var result := session.commit(session.snapshot(),"manual")
		if result.ok:
			ui.show_notice("Jornada salva. Ao continuar, você retorna à Cidadela.")
			if action == "save_quit": get_tree().quit()
	elif action == "controls":
		var lines: Array[String] = []
		for pair in [["move_up","Mover acima"],["move_down","Mover abaixo"],["move_left","Mover à esquerda"],["move_right","Mover à direita"],["attack_basic","Machado"],["skill_1","Impacto"],["skill_2","Guarda"],["skill_3","Fenda"],["dash","Esquiva"],["interact","Interagir"],["inventory","Bolsa"],["use_health","Poção de Vida"],["use_mana","Frasco de Éter"]]:
			var keys: Array[String] = []
			for event in InputMap.action_get_events(pair[0]):
				if event is InputEventKey: keys.append(OS.get_keycode_string(event.physical_keycode))
			lines.append(pair[1]+": "+" / ".join(keys))
		get_tree().paused = true
		ui.show_menu("Controles","\n".join(lines),[["Voltar","resume"]])
	elif action == "options":
		get_tree().paused = true
		ui.show_menu("Opções","Preferências salvas separadamente da campanha.",[["Efeitos reduzidos: "+("sim" if ui.reduced_effects else "não"),"toggle_effects"],["Som: "+("ligado" if ui.sound_enabled else "desligado"),"toggle_sound"],["Voltar","resume"]])
	elif action == "toggle_effects":
		ui.reduced_effects = not ui.reduced_effects
		ui.save_options()
		impact_feedback.enabled = not ui.reduced_effects
		if ui.reduced_effects: impact_feedback.reset()
		if is_instance_valid(player): player.get_node("Camera2D").position_smoothing_enabled = not ui.reduced_effects
		if ui.menu_open: command("options")
	elif action == "toggle_sound":
		ui.sound_enabled = not ui.sound_enabled
		ui.save_options()
		audio_director.enabled = ui.sound_enabled
		command("options")
	elif not begun: return
	elif action == "interact": interact()
	elif action == "inventory": show_inventory()
	elif action == "shop": show_shop()
	elif action.begins_with("buy:"):
		var result := session.buy(action.trim_prefix("buy:"))
		var feedback: String = "Compra concluída. O item está na bolsa." if result.ok else "Compra recusada: " + ("ouro insuficiente" if result.code == "INSUFFICIENT_GOLD" else "bolsa cheia" if result.code == "BAG_FULL" else result.code)
		show_shop(feedback)
	elif action.begins_with("sell:"):
		var result := session.sell(action.trim_prefix("sell:"))
		show_shop("Venda concluída; ouro recebido." if result.ok else "Venda recusada: " + result.code)
	elif action == "journal":
		get_tree().paused = true
		var body := "A Caça ao Espião\n\nUm mensageiro acusa moradores de abastecer cultistas. A aldeia está sem escolta; parte dos moradores adoeceu. Investigue as três pistas, decida e volte à Cidadela.\n\n"
		var clues := {"forest_01.clue.tracks":"Pegadas cultistas passam pela aldeia.","forest_01.clue.testimony":"Os relatos se contradizem; pode ter havido coerção.","forest_01.clue.well":"Há um brilho violeta no poço. A origem é incerta."}
		for id in clues:
			body += ("• "+clues[id] if session.state.world_flags.get(id,false) else "• Pista ainda não observada.")+"\n"
		if session.state.quest_choices.has("act1.spy_hunt"): body += "\nDecisão: "+("Escoltar os moradores." if session.state.quest_choices["act1.spy_hunt"] == "shelter_villagers" else "Executar os suspeitos.")
		ui.show_menu("Diário da jornada",body,[["Voltar","resume"]])
	elif action == "use_health": session.use_item(&"potion_health_small")
	elif action == "use_mana": session.use_item(&"flask_aether_small")
	elif action.begins_with("equip:"):
		session.equip(action.trim_prefix("equip:"))
		show_inventory()
	elif action.begins_with("discard:"):
		session.discard(action.trim_prefix("discard:"))
		show_inventory()
	elif action.begins_with("use:"):
		session.use_item(StringName(action.trim_prefix("use:")))
		show_inventory()
	elif action.begins_with("choice:"):
		var option := action.trim_prefix("choice:")
		ui.show_menu("Confirmar decisão","Esta decisão é permanente nesta campanha. As consequências serão registradas no save.\n\n"+("Executar os suspeitos e proteger a estrada." if option == "judge_suspects" else "Escoltar os moradores até a Cidadela."),[["Confirmar","confirm:"+option],["Investigar mais","resume"]])
	elif action.begins_with("confirm:"):
		var result := session.choose(action.trim_prefix("confirm:"))
		if result.ok:
			resume()
			ui.show_notice("Decisão registrada. Retorne à Cidadela.")
		else: ui.show_notice("Escolha recusada: "+result.code)
func interact() -> void:
	if nearest.is_empty(): return
	var id: String = nearest.id
	if id == "gate": travel("forest_01" if session.zone == "citadel" else "citadel")
	elif id == "crystal":
		var result := session.travel("citadel")
		if result.ok:
			session.apply_player()
			ui.show_notice("Éter restaurado. HP e MP recuperados; jornada salva.")
	elif id == "stash":
		session.reclaim()
		ui.show_notice("Recompensas retiradas conforme o espaço disponível na bolsa.")
	elif id in ["npc.merchant", "npc.alchemist"]:
		show_shop()
	elif id.begins_with("npc."):
		if id == "npc.elder" and session.state.world_flags.get("prologue.last_bell", false) and not session.state.world_flags.get("prologue.council", false) and not session.state.quest_choices.has("act1.spy_hunt"):
			council_index = 0
			show_council_page()
			return
		if id == "npc.elder" and session.state.applied_effects.has("first_return_to_citadel") and not session.state.world_flags.get("hub.council_briefed", false):
			var candidate := session.snapshot()
			candidate.world_flags["hub.council_briefed"] = true
			var briefing := session.commit(candidate, "council_briefing")
			if not briefing.ok: return
		var line := DialogueResolver.resolve(id,session.state.world_flags)
		get_tree().paused = true
		ui.show_menu(line.speaker,line.text,[["Encerrar conversa","resume"]])
	elif id in session.quest.definition.clues:
		var result := session.investigate(id)
		if result.ok: command("journal")
	elif id == "choice":
		if session.state.quest_choices.has("act1.spy_hunt"):
			ui.show_notice("Sua decisão já foi registrada. Retorne à Cidadela.")
		elif not session.quest.available(session.state.snapshot()):
			ui.show_notice("Investigue pegadas, depoimentos e o poço antes de decidir.")
		else:
			get_tree().paused = true
			ui.show_menu("A Caça ao Espião","“Demos pão a quem veio armado. Meu menino está febril há dias. Olhe para ele antes de decidir o que somos.”\n\nNenhuma pista, sozinha, explica o que aconteceu.",[["Executar os suspeitos e proteger a estrada","choice:judge_suspects"],["Escoltar os moradores à Cidadela","choice:shelter_villagers"],["Ainda preciso pensar","resume"]])
func show_inventory() -> void:
	get_tree().paused = true
	ui.show_inventory_grid(session.inventory)
func show_shop(feedback: String = "") -> void:
	if session.zone != "citadel": return
	get_tree().paused = true
	var stock := "Ouro disponível: %d\n\nLyra oferece provisões. Sera vende equipamento e compra materiais e peças não equipadas." % session.inventory.gold
	if not feedback.is_empty(): stock += "\n\n" + feedback
	var options: Array = []
	for id in MerchantService.PRICES:
		var spec := session.inventory.item(StringName(id))
		options.append(["Comprar %s — %d ouro" % [spec.display_name, MerchantService.PRICES[id]], "buy:" + id])
	for id in MerchantService.SELL_PRICES:
		if session.inventory.counts.get(id, 0) > 0 and not id in session.inventory.equipped.values():
			var spec := session.inventory.item(StringName(id))
			options.append(["Vender %s — %d ouro" % [spec.display_name, MerchantService.SELL_PRICES[id]], "sell:" + id])
	options.append(["Voltar ao jogo", "resume"])
	ui.show_menu("MERCADO DA CIDADELA", stock, options)
