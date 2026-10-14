class_name SliceHUD
extends CanvasLayer
const ITEM_ART := preload("res://assets/painted/items_atlas.png")
const CITADEL_ART := preload("res://assets/painted/citadel_courtyard.png")
signal command(action: String)
var hp: ProgressBar
var mp: ProgressBar
var stamina: ProgressBar
var resources: Label
var hp_value: Label
var mp_value: Label
var stamina_value: Label
var objective: Label
var location: Label
var message: Label
var prompt: Label
var skills: Label
var boss_panel: Panel
var boss_name: Label
var boss_hp: ProgressBar
var overlay: Control
var menu_box: VBoxContainer
var menu_open := false
var cinematic_skip_action := ""
var cinematic_backdrop: TextureRect
var cinematic_time := 0.0
var reduced_effects := false
var sound_enabled := true
var notice_left := 0.0
var audio: AudioStreamPlayer
var config_path := "user://tests/settings.cfg" if "--test-harness" in OS.get_cmdline_user_args() else "user://settings.cfg"
func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	var cfg := ConfigFile.new()
	if cfg.load(config_path) == OK:
		var reduced: Variant = cfg.get_value("display", "reduced_effects", false)
		var sound: Variant = cfg.get_value("audio", "enabled", true)
		if reduced is bool: reduced_effects = reduced
		if sound is bool: sound_enabled = sound
	var theme := Theme.new()
	theme.default_font_size = 18
	var screen := Control.new()
	screen.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	screen.mouse_filter = Control.MOUSE_FILTER_IGNORE
	screen.theme = theme
	add_child(screen)
	var top := panel(screen, Rect2(24,22,320,65))
	location = label(top, "CIDADELA DOS ÚLTIMOS", 18, Color("#e0c994"))
	location.position = Vector2(18,10)
	var subtitle := label(top,"ECHOES OF AETHEL  /  PRIMEIRA FATIA",11,Color("#94afa9"))
	subtitle.position = Vector2(18,39)
	boss_panel = panel(screen,Rect2(390,22,450,62))
	boss_name = label(boss_panel,"GARRAS DE MUSGO",14,Color("#d6c797"))
	boss_name.position = Vector2(16,8)
	boss_hp = bar(boss_panel,Vector2(16,36),Vector2(418,8),Color("#b6a577"))
	boss_panel.hide()
	var quest_panel := panel(screen,Rect2(890,22,366,99))
	objective = label(quest_panel,"",15,Color("#dce0d6"))
	objective.position = Vector2(17,12)
	objective.size = Vector2(334,80)
	objective.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	var bottom := panel(screen,Rect2(24,548,410,148))
	var heading := label(bottom,"BASTIÃO DO CREPÚSCULO",12,Color("#c5ad7e"))
	heading.position = Vector2(17,7)
	hp = resource_bar(bottom,"VIDA",30,Color("#e74c3c"))
	mp = resource_bar(bottom,"ÉTER",59,Color("#3498db"))
	stamina = resource_bar(bottom,"FÔLEGO",88,Color("#f1c40f"))
	hp_value = resource_value(bottom,30)
	mp_value = resource_value(bottom,59)
	stamina_value = resource_value(bottom,88)
	resources = label(bottom,"",13,Color("#e7d6a9"))
	resources.position = Vector2(18,119)
	resources.size = Vector2(375,22)
	var skill_panel := panel(screen,Rect2(454,621,802,75))
	skills = label(skill_panel,"",15,Color("#ded9bd"))
	skills.position = Vector2(18,9)
	skills.size = Vector2(775,58)
	prompt = label(screen,"",17,Color("#ead4a0"))
	prompt.position = Vector2(400,552)
	prompt.size = Vector2(700,34)
	prompt.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	message = label(screen,"",16,Color("#b5e1d7"))
	message.position = Vector2(345,123)
	message.size = Vector2(700,55)
	message.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	message.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	overlay = Control.new()
	overlay.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	screen.add_child(overlay)
	overlay.hide()
	audio = AudioStreamPlayer.new()
	audio.volume_db = -16
	add_child(audio)
func panel(parent: Node, rect: Rect2) -> Panel:
	var p := Panel.new()
	p.position = rect.position
	p.size = rect.size
	p.mouse_filter = Control.MOUSE_FILTER_IGNORE
	var box := StyleBoxFlat.new()
	box.bg_color = Color(0.045,0.079,0.105,0.94)
	box.border_color = Color("#536666")
	box.set_border_width_all(1)
	box.set_corner_radius_all(4)
	p.add_theme_stylebox_override("panel",box)
	parent.add_child(p)
	return p
func label(parent: Node, text: String, font_size: int, color: Color) -> Label:
	var l := Label.new()
	l.text = text
	l.add_theme_font_size_override("font_size",font_size)
	l.add_theme_color_override("font_color",color)
	l.mouse_filter = Control.MOUSE_FILTER_IGNORE
	parent.add_child(l)
	return l
func bar(parent: Node, p: Vector2, s: Vector2, color: Color) -> ProgressBar:
	var b := ProgressBar.new()
	b.position = p
	b.size = s
	b.show_percentage = false
	b.add_theme_font_size_override("font_size",1)
	var bg := StyleBoxFlat.new()
	bg.bg_color = Color("#1a1a1a")
	var fg := StyleBoxFlat.new()
	fg.bg_color = color
	b.add_theme_stylebox_override("background",bg)
	b.add_theme_stylebox_override("fill",fg)
	parent.add_child(b)
	return b
func resource_bar(parent: Node, title: String, y: float, color: Color) -> ProgressBar:
	var caption := label(parent,title,12,Color("#ddd0ae"))
	caption.position = Vector2(18,y + 2)
	caption.size = Vector2(76,20)
	var meter := bar(parent,Vector2(98,y),Vector2(291,21),color)
	var background := StyleBoxFlat.new()
	background.bg_color = Color("#080e16")
	background.border_color = Color("#8c7a57")
	background.set_border_width_all(2)
	background.set_corner_radius_all(3)
	var fill := StyleBoxFlat.new()
	fill.bg_color = color.darkened(0.22)
	fill.border_color = color.lightened(0.28)
	fill.set_border_width_all(1)
	fill.set_corner_radius_all(2)
	meter.add_theme_stylebox_override("background",background)
	meter.add_theme_stylebox_override("fill",fill)
	return meter
func resource_value(parent: Node, y: float) -> Label:
	var value := label(parent,"",12,Color("#fff5df"))
	value.position = Vector2(100,y + 1)
	value.size = Vector2(285,19)
	value.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	value.add_theme_color_override("font_shadow_color",Color(0,0,0,0.95))
	value.add_theme_constant_override("shadow_offset_x",1)
	value.add_theme_constant_override("shadow_offset_y",1)
	return value
func show_menu(title: String, body: String, buttons: Array) -> void:
	cinematic_skip_action = ""
	cinematic_backdrop = null
	for child in overlay.get_children():
		overlay.remove_child(child)
		child.queue_free()
	overlay.show()
	menu_open = true
	var shade := ColorRect.new()
	shade.color = Color(0.01,0.02,0.035,0.78)
	shade.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	overlay.add_child(shade)
	var frame := panel(overlay,Rect2(315,100,650,510))
	frame.mouse_filter = Control.MOUSE_FILTER_STOP
	var scroll := ScrollContainer.new()
	scroll.position = Vector2(30,27)
	scroll.size = Vector2(590,455)
	frame.add_child(scroll)
	menu_box = VBoxContainer.new()
	menu_box.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	menu_box.add_theme_constant_override("separation",12)
	scroll.add_child(menu_box)
	var heading := label(menu_box,title,29,Color("#e1cca1"))
	heading.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	var description := label(menu_box,body,17,Color("#baccc6"))
	description.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	description.custom_minimum_size.x = 550
	var first: Button
	for item in buttons:
		var button := Button.new()
		button.text = item[0]
		button.custom_minimum_size = Vector2(550,39)
		button.alignment = HORIZONTAL_ALIGNMENT_LEFT
		button.add_theme_font_size_override("font_size",17)
		var action: String = item[1]
		button.pressed.connect(func(): tone(520); command.emit(action))
		menu_box.add_child(button)
		if first == null: first = button
	if first != null: focus_menu_button.call_deferred(first)
func show_cinematic(title: String, speaker: String, body: String, page: int, total: int, next_action: String = "prologue_next", skip_action: String = "skip_prologue") -> void:
	cinematic_skip_action = skip_action
	cinematic_time = 0.0
	for child in overlay.get_children():
		overlay.remove_child(child)
		child.queue_free()
	overlay.show()
	menu_open = true
	var backdrop := TextureRect.new()
	backdrop.texture = CITADEL_ART
	backdrop.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	backdrop.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_COVERED
	backdrop.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	overlay.add_child(backdrop)
	cinematic_backdrop = backdrop
	backdrop.offset_left = -18.0
	backdrop.offset_top = -10.0
	backdrop.offset_right = 18.0
	backdrop.offset_bottom = 10.0
	var veil := ColorRect.new()
	veil.color = Color(0.015, 0.022, 0.039, 0.56)
	veil.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	overlay.add_child(veil)
	for rect in [Rect2(0, 0, 1280, 72), Rect2(0, 464, 1280, 256)]:
		var letterbox := ColorRect.new()
		letterbox.color = Color(0.012, 0.019, 0.031, 0.96)
		letterbox.position = rect.position
		letterbox.size = rect.size
		overlay.add_child(letterbox)
	var chapter := label(overlay, title, 22, Color("#e1cca1"))
	chapter.position = Vector2(58, 18)
	var progress := label(overlay, "%d / %d" % [page, total], 15, Color("#91a7aa"))
	progress.position = Vector2(1150, 24)
	var voice := label(overlay, speaker, 22, Color("#e1cca1"))
	voice.position = Vector2(72, 490)
	var line := RichTextLabel.new()
	line.text = body
	line.scroll_active = false
	line.mouse_filter = Control.MOUSE_FILTER_IGNORE
	line.add_theme_font_size_override("normal_font_size", 23)
	line.add_theme_color_override("default_color", Color("#f0e9d8"))
	line.position = Vector2(72, 532)
	line.size = Vector2(1136, 94)
	overlay.add_child(line)
	var next := Button.new()
	next.text = ("Encerrar conselho" if next_action == "council_next" else "Entrar na Cidadela") if page == total else "Continuar"
	next.position = Vector2(72, 647)
	next.size = Vector2(225, 43)
	next.pressed.connect(func(): command.emit(next_action))
	overlay.add_child(next)
	var skip := Button.new()
	skip.text = "Pular conselho" if skip_action == "skip_council" else "Pular abertura"
	skip.position = Vector2(318, 647)
	skip.size = Vector2(190, 43)
	skip.pressed.connect(func(): command.emit(skip_action))
	overlay.add_child(skip)
	focus_menu_button.call_deferred(next)
func focus_menu_button(button: Button) -> void:
	if is_instance_valid(button) and button.is_inside_tree(): button.grab_focus()
func close_menu() -> void:
	menu_open = false
	cinematic_skip_action = ""
	cinematic_backdrop = null
	overlay.hide()
func item_icon(id: String) -> AtlasTexture:
	var cell := Vector2i(0, 0)
	if id.contains("cuirass") or id.contains("vest") or id.contains("tunic"): cell = Vector2i(1, 0)
	elif id == "potion_health_small": cell = Vector2i(2, 0)
	elif id == "flask_aether_small": cell = Vector2i(0, 1)
	elif id == "wolf_fang": cell = Vector2i(1, 1)
	elif id == "cultist_ash": cell = Vector2i(2, 1)
	elif id == "root_essence": cell = Vector2i(0, 2)
	elif id == "antidote_flask": cell = Vector2i(1, 2)
	elif id.contains("runed") or id.contains("twilight"): cell = Vector2i(2, 2)
	var icon := AtlasTexture.new()
	icon.atlas = ITEM_ART
	icon.region = Rect2(cell.x * 426 + 30, cell.y * 426 + 30, 366, 366)
	return icon
func item_rarity(id: String) -> Dictionary:
	if id in ["twilight_axe", "twilight_cuirass"]:
		return {"label": "Épico", "color": Color("#bd8bea")}
	if id in ["bastion_axe", "bastion_cuirass"]:
		return {"label": "Raro", "color": Color("#69a9f3")}
	if id in ["runed_axe", "ironweave_cuirass"]:
		return {"label": "Mágico", "color": Color("#79c96b")}
	return {"label": "Comum", "color": Color("#b0b0b0")}
func inventory_button(parent: Node, id: String, count: int, size: Vector2) -> Button:
	var button := Button.new()
	button.custom_minimum_size = size
	button.tooltip_text = id
	var slot_style := StyleBoxFlat.new()
	slot_style.bg_color = Color("#17222a")
	slot_style.border_color = item_rarity(id).color
	slot_style.set_border_width_all(1)
	button.add_theme_stylebox_override("normal", slot_style)
	parent.add_child(button)
	var art := TextureRect.new()
	art.position = Vector2(3, 2)
	art.size = size - Vector2(6, 6)
	art.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	art.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
	art.texture = item_icon(id)
	art.mouse_filter = Control.MOUSE_FILTER_IGNORE
	button.add_child(art)
	if count > 1:
		var number := label(button, str(count), 12, Color("#f4dba2"))
		number.position = Vector2(size.x - 19, size.y - 18)
	return button
func show_inventory_grid(inv: InventoryModel) -> void:
	for child in overlay.get_children():
		overlay.remove_child(child)
		child.queue_free()
	overlay.show()
	menu_open = true
	var shade := ColorRect.new()
	shade.color = Color(0.01, 0.02, 0.035, 0.88)
	shade.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	overlay.add_child(shade)
	var frame := panel(overlay, Rect2(235, 70, 810, 580))
	frame.mouse_filter = Control.MOUSE_FILTER_STOP
	var title := label(frame, "BOLSA DO BASTIÃO", 27, Color("#e5c997"))
	title.position = Vector2(22, 15)
	var summary := label(frame, "%d / %d posições   •   %.1f / %.1f peso   •   %d ouro" % [inv.positions(), inv.config.inventory_capacity, inv.total_weight(), inv.config.inventory_weight, inv.gold], 14, Color("#b7c8c0"))
	summary.position = Vector2(22, 58)
	var equipped_title := label(frame, "EQUIPAMENTO", 17, Color("#d9c39a"))
	equipped_title.position = Vector2(22, 110)
	for index in range(2):
		var slot: String = ["main_hand", "chest"][index]
		var id: String = inv.equipped[slot]
		if id.is_empty(): continue
		var icon := inventory_button(frame, id, 1, Vector2(58, 58))
		icon.position = Vector2(24, 145 + index * 100)
		icon.tooltip_text = inv.item(id).display_name
		var equipped_label := label(frame, inv.item(id).display_name, 14, Color("#d9ded2"))
		equipped_label.position = Vector2(91, 162 + index * 100)
	var grid := GridContainer.new()
	grid.columns = 10
	grid.position = Vector2(235, 105)
	grid.add_theme_constant_override("h_separation", 3)
	grid.add_theme_constant_override("v_separation", 3)
	frame.add_child(grid)
	var detail := label(frame, "Selecione um item para ver seus detalhes.", 16, Color("#e2d2a9"))
	detail.position = Vector2(22, 435)
	detail.size = Vector2(760, 45)
	detail.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	var actions := HBoxContainer.new()
	actions.position = Vector2(22, 490)
	actions.add_theme_constant_override("separation", 12)
	frame.add_child(actions)
	var items: Array[String] = []
	for id in inv.counts: items.append(String(id))
	items.sort()
	var occupied := 0
	for id in items:
		var spec := inv.item(id)
		var remaining: int = inv.counts[id]
		while remaining > 0 and occupied < 60:
			var stack: int = mini(remaining, spec.max_stack)
			var button := inventory_button(grid, id, stack, Vector2(40, 40))
			button.tooltip_text = item_rarity(id).label + " • " + spec.display_name + " ×" + str(stack)
			button.pressed.connect(func(): tone(420); select_inventory_item(inv, id, detail, actions))
			remaining -= stack
			occupied += 1
	for index in range(occupied, 60):
		var empty := Button.new()
		empty.custom_minimum_size = Vector2(40, 40)
		empty.disabled = true
		var empty_style := StyleBoxFlat.new()
		empty_style.bg_color = Color("#111b23")
		empty_style.border_color = Color("#4c4b43")
		empty_style.set_border_width_all(1)
		empty.add_theme_stylebox_override("disabled", empty_style)
		grid.add_child(empty)
	var back := Button.new()
	back.text = "Voltar ao jogo"
	back.position = Vector2(628, 535)
	back.size = Vector2(160, 32)
	back.pressed.connect(func(): command.emit("resume"))
	frame.add_child(back)
func select_inventory_item(inv: InventoryModel, id: String, detail: Label, actions: HBoxContainer) -> void:
	for child in actions.get_children():
		actions.remove_child(child)
		child.queue_free()
	var spec := inv.item(id)
	if spec == null: return
	var rarity := item_rarity(id)
	var text: String = rarity.label + " • " + spec.display_name + "  ×" + str(inv.counts[id])
	detail.add_theme_color_override("font_color", rarity.color)
	if spec.kind == &"equipment":
		text += "  •  " + ("Poder %d" % spec.power if spec.slot == &"main_hand" else "Defesa +%d" % spec.armor)
		text += "  •  equipado" if id in inv.equipped.values() else ""
	elif spec.kind == &"quest": text += "  •  item de missão"
	detail.text = text
	if spec.kind == &"equipment":
		inventory_action(actions, "Equipar", "equip:" + id)
	elif spec.kind == &"consumable":
		inventory_action(actions, "Usar", "use:" + id)
	if spec.kind != &"quest" and id not in inv.equipped.values():
		inventory_action(actions, "Descartar 1", "discard:" + id)
func inventory_action(parent: HBoxContainer, title: String, action: String) -> void:
	var button := Button.new()
	button.text = title
	button.custom_minimum_size = Vector2(148, 38)
	button.pressed.connect(func(): command.emit(action))
	parent.add_child(button)
func show_notice(text: String) -> void:
	message.text = text
	notice_left = 5.0
func update_state(player: Player, session: SliceSession, nearest: Dictionary) -> void:
	if player == null: return
	boss_panel.hide()
	for node in get_tree().get_nodes_in_group("enemies"):
		if node is EnemyActor and node.definition.category == &"sub_boss" and not node.is_dead and node.global_position.distance_to(player.global_position) < 360:
			boss_panel.show()
			boss_hp.max_value = node.max_health
			boss_hp.value = node.current_health
			boss_name.text = "GARRAS DE MUSGO   •   %d / %d" % [node.current_health,node.max_health]
	hp.max_value = player.max_health
	hp.value = player.current_health
	mp.max_value = player.mp_max
	mp.value = player.mp
	stamina.max_value = player.stamina_max
	stamina.value = player.stamina
	hp_value.text = "%d / %d" % [player.current_health,player.max_health]
	mp_value.text = "%d / %d" % [int(player.mp),int(player.mp_max)]
	stamina_value.text = "%d / %d" % [int(player.stamina),int(player.stamina_max)]
	resources.text = "NÍVEL %d     ◆  %d OURO" % [player.stats.level,session.inventory.gold]
	location.text = "CIDADELA DOS ÚLTIMOS" if session.zone == "citadel" else "BOSQUE DAS NÉVOAS"
	prompt.text = "" if nearest.is_empty() else "[C]  " + nearest.name
	var labels: Array[String] = []
	for entry in [[1,"2/Q Impacto",12],[2,"3/E Guarda",8],[3,"4/R Fenda",20]]:
		var cd := player.cooldown_for(entry[0])
		labels.append("%s  %s" % [entry[1],("%.1fs" % cd) if cd > 0 else ("%d MP" % entry[2])])
	skills.text = "1 Machado   •   " + "   |   ".join(labels) + "\n5/F Reservada     I Bolsa     J Diário     H Vida     T Éter     Esc Pausa"
	var state: Dictionary = session.state.snapshot()
	var clues := 0
	for id in session.quest.definition.clues:
		if state.world_flags.get(id,false): clues += 1
	objective.text = objective_for(state, session.zone, clues)
static func objective_for(state: Dictionary, zone: String, clues: int) -> String:
	if state.world_flags.get("prologue.last_bell", false) and not state.world_flags.get("prologue.council", false) and not state.quest_choices.has("act1.spy_hunt") and clues == 0:
		return "O CONSELHO DOS QUE FICARAM\nFale com Irian junto ao cristal da Cidadela." if zone == "citadel" else "O CONSELHO DOS QUE FICARAM\nVolte à Cidadela e fale com Irian junto ao cristal."
	if not state.world_flags.get("forest_01.moss_claws.defeated", false):
		return "A CAÇA AO ESPIÃO\nInvestigue a aldeia (%d/3) e enfrente Garras de Musgo ao nordeste." % clues
	if not state.quest_choices.has("act1.spy_hunt"):
		return "A CAÇA AO ESPIÃO\nGarras derrotado. Investigue (%d/3) e confirme sua decisão na aldeia." % clues
	if not state.applied_effects.has("first_return_to_citadel"):
		return "O PESO DA ESCOLHA\nRetorne à Cidadela e encontre quem ficou."
	if not state.world_flags.get("hub.council_briefed", false):
		return "O PESO DA ESCOLHA\n%s e fale com Irian, líder do conselho, junto ao cristal." % ("Volte à Cidadela" if zone != "citadel" else "Vá até o centro da Cidadela")
	return "PRIMEIRA FATIA CONCLUÍDA\nSua decisão foi registrada por Irian. O Bosque continua aberto para exploração."
func _process(delta: float) -> void:
	notice_left = maxf(0,notice_left-delta)
	if notice_left <= 0: message.text = ""
	if is_instance_valid(cinematic_backdrop) and menu_open and not reduced_effects:
		cinematic_time = minf(8.0, cinematic_time + delta)
		var shift := cinematic_time * 2.0
		cinematic_backdrop.offset_left = -18.0 + shift
		cinematic_backdrop.offset_right = 18.0 + shift
func _unhandled_key_input(event: InputEvent) -> void:
	if not event is InputEventKey or not event.pressed or event.echo: return
	if event.is_action_pressed("pause"):
		command.emit(cinematic_skip_action if not cinematic_skip_action.is_empty() else ("resume" if menu_open else "pause"))
		get_viewport().set_input_as_handled()
	elif not menu_open:
		for action in ["inventory","journal","interact","use_health","use_mana"]:
			if event.is_action_pressed(action):
				command.emit(action)
				get_viewport().set_input_as_handled()
				return
		if event.physical_keycode == KEY_F9: command.emit("toggle_effects")
func save_options() -> Error:
	var cfg := ConfigFile.new()
	cfg.set_value("display","reduced_effects",reduced_effects)
	cfg.set_value("audio","enabled",sound_enabled)
	return cfg.save(config_path)
func tone(frequency: float) -> void:
	if not sound_enabled: return
	var wav := AudioStreamWAV.new()
	wav.format = AudioStreamWAV.FORMAT_16_BITS
	wav.mix_rate = 22050
	var data := PackedByteArray()
	data.resize(4410)
	for i in range(2205):
		var value := int(sin(i * TAU * frequency / 22050.0) * (1.0 - i/2205.0) * 6500)
		data.encode_s16(i*2,value)
	wav.data = data
	audio.stream = wav
	audio.play()
