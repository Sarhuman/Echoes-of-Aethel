extends Node2D
## Disposable playtest session. Does not read/write campaign progress.
enum Stage { READY, FIGHT, REST, WON, LOST }
const BOUNDS := Rect2(40, 140, 880, 320)
const OBSTACLES: Array[Rect2] = [Rect2(390, 220, 40, 120), Rect2(610, 340, 100, 30)]
var navigation := ArenaNavigation.new()
var player: Player
var enemies: Array[ActorBase] = []
var stage := Stage.READY
var wave := 0
var elapsed := 0.0
var banner: Label
var stats: Label
var hint: Label
var action: Button
var notice := ""

func _ready() -> void:
	navigation.configure(BOUNDS, OBSTACLES)
	for rect in OBSTACLES:
		_wall(rect)
	for rect in [Rect2(20, 120, 920, 20), Rect2(20, 460, 920, 20), Rect2(20, 140, 20, 320), Rect2(920, 140, 20, 320)]:
		_wall(rect)
	player = Player.new()
	player.name = "Player"
	player.actor_id = &"playtest.player"
	player.team_id = &"player"
	player.position = Vector2(170, 300)
	_body(player)
	add_child(player)
	player.skill_unavailable.connect(func(slot): notice = "Habilidade %d reservada. Use 1 ou 2/Q." % slot)
	player.attack_started.connect(func(_id): notice = "")
	var layer := CanvasLayer.new()
	add_child(layer)
	banner = _label(layer, Vector2(40, 20), 24)
	stats = _label(layer, Vector2(40, 65), 17)
	hint = _label(layer, Vector2(40, 486), 15)
	hint.text = "WASD / setas: mover   •   1: atacar   •   2/Q: pesado   •   Espaço: dash\nEnter: iniciar / avançar   •   F6: reiniciar   •   Esc: pausar"
	action = Button.new()
	action.position = Vector2(710, 22)
	action.size = Vector2(210, 36)
	action.focus_mode = Control.FOCUS_NONE
	action.pressed.connect(advance_session)
	layer.add_child(action)
	process_mode = Node.PROCESS_MODE_ALWAYS
	player.process_mode = Node.PROCESS_MODE_PAUSABLE
	_refresh()

func _label(parent: Node, point: Vector2, size: int) -> Label:
	var label := Label.new()
	label.position = point
	label.add_theme_font_size_override("font_size", size)
	parent.add_child(label)
	return label

func _body(actor: ActorBase) -> void:
	actor.collision_layer = 1
	actor.collision_mask = 2 # Actors can separate freely; scenery remains solid.
	var collision := CollisionShape2D.new()
	var shape := CircleShape2D.new()
	shape.radius = 9.0
	collision.shape = shape
	actor.add_child(collision)

func _wall(rect: Rect2) -> void:
	var body := StaticBody2D.new()
	body.position = rect.get_center()
	body.collision_layer = 2
	body.collision_mask = 0
	var shape := RectangleShape2D.new()
	shape.size = rect.size
	var collision := CollisionShape2D.new()
	collision.shape = shape
	body.add_child(collision)
	add_child(body)

func advance_session() -> void:
	if get_tree().paused:
		return
	if stage in [Stage.WON, Stage.LOST]:
		get_tree().reload_current_scene()
	elif stage in [Stage.READY, Stage.REST]:
		_start_wave()

func _start_wave() -> void:
	for enemy in enemies:
		enemy.queue_free()
	enemies.clear()
	wave += 1
	stage = Stage.FIGHT
	player.position = Vector2(170, 300)
	player.attacks.cancel()
	player._sync_attack_state()
	player.mp = player.mp_max
	player.stamina = player.stamina_max
	player.current_health = mini(player.max_health, player.current_health + 30)
	player.dash_left = 0.0
	player.iframe_left = 0.0
	player.cooldown_left = 0.0
	_spawn("Lobo Etéreo", Vector2(740, 240), 55, 82.0, 34.0, 0.65, 10)
	if wave >= 2:
		_spawn("Cultista", Vector2(760, 410), 75, 48.0, 85.0, 1.1, 16)
	if wave >= 3:
		_spawn("Lobo Etéreo", Vector2(530, 190), 55, 82.0, 34.0, 0.65, 10)
	_refresh()

func _spawn(title: String, point: Vector2, hp: int, speed: float, reach: float, windup: float, damage: int) -> void:
	var enemy := ActorBase.new()
	enemy.actor_id = StringName("arena.%d.%d" % [wave, enemies.size()])
	enemy.set_meta("title", title)
	enemy.position = point
	enemy.max_health = hp
	enemy.movement_speed = speed
	enemy.process_mode = Node.PROCESS_MODE_PAUSABLE
	_body(enemy)
	var brain := ArenaBrain.new()
	brain.name = "Brain"
	brain.actor = enemy
	brain.navigation = navigation
	brain.detection_range = 900.0
	brain.attack_range = reach
	brain.windup_duration = windup
	brain.attack_damage = damage
	brain.attack_cooldown = 1.5
	brain.vertical_projection_scale = 0.7
	brain.waypoints = PackedVector2Array([point, Vector2(270, 190), Vector2(270, 410)])
	enemy.add_child(brain)
	add_child(enemy)
	enemies.append(enemy)

func _process(delta: float) -> void:
	if not get_tree().paused and stage == Stage.FIGHT:
		elapsed += delta
		if player.is_dead:
			stage = Stage.LOST
		elif enemies.all(func(enemy): return enemy.is_dead):
			stage = Stage.WON if wave == 3 else Stage.REST
	_refresh()
	queue_redraw()

func _refresh() -> void:
	var titles := ["BOSQUE • Treino do Bastião", "ENCONTRO %d / 3" % wave, "ÁREA LIMPA • Recupere o fôlego", "VITÓRIA • Bosque protegido", "DERROTA • Tente outra abordagem"]
	banner.text = "PAUSADO" if get_tree().paused else titles[stage]
	stats.text = "VIDA %d/%d     ÉTER %.0f/%.0f     FOLEGO %.0f     %.0f s\n%s" % [player.current_health, player.max_health, player.mp, player.mp_max, player.stamina, elapsed, notice if notice != "" else "Evite o círculo vermelho antes do impacto. Mire pela direção do movimento."]
	action.visible = stage != Stage.FIGHT
	action.text = "Iniciar treino" if stage == Stage.READY else ("Próximo encontro" if stage == Stage.REST else "Tentar novamente")

func _unhandled_key_input(event: InputEvent) -> void:
	if not event is InputEventKey or not event.pressed or event.echo:
		return
	if event.keycode == KEY_ESCAPE:
		get_tree().paused = not get_tree().paused
	elif event.is_action_pressed("debug_restart"):
		get_tree().paused = false
		get_tree().reload_current_scene()
	elif event.keycode == KEY_ENTER:
		advance_session()

func _draw() -> void:
	draw_rect(Rect2(0, 0, 960, 540), Color("101c24"))
	draw_rect(BOUNDS, Color("203a37"))
	for x in range(60, 920, 40):
		for y in range(160, 460, 40):
			draw_line(Vector2(x - 12, y), Vector2(x + 12, y), Color("29443e"))
	for rect in OBSTACLES:
		draw_rect(rect.grow(3), Color("101c24"))
		draw_rect(rect, Color("63716b"))
		draw_line(rect.position, rect.position + Vector2(rect.size.x, 0), Color("a4aa91"), 3)
	if not is_instance_valid(player):
		return
	_draw_actor(player, Color("e6be6f"), "Bastião")
	if not player.is_dead:
		var direction := player.facing
		draw_line(player.position + direction * 15, player.position + direction * 26, Color("fff0bd"), 3)
		if player.attacks.phase != CombatService.Phase.IDLE:
			draw_arc(player.position, player.attacks.definition.reach, player.attacks.direction.angle() - PI / 3, player.attacks.direction.angle() + PI / 3, 24, Color("ffe2a0"), 3)
	for enemy in enemies:
		if enemy.is_dead:
			continue
		var brain := enemy.get_node("Brain") as ArenaBrain
		if brain._winding:
			draw_arc(enemy.position, brain.attack_range, 0, TAU, 40, Color("ff826e"), 2)
			var progress := 1.0 - brain._windup_left / brain.windup_duration
			draw_arc(enemy.position, brain.attack_range, -PI / 2, -PI / 2 + TAU * progress, 40, Color("ffe6b0"), 4)
		_draw_actor(enemy, Color("91c4bc") if enemy.get_meta("title") == "Lobo Etéreo" else Color("bd85b2"), enemy.get_meta("title"))

func _draw_actor(actor: ActorBase, color: Color, title: String) -> void:
	var point := actor.position
	if actor.is_dead:
		draw_line(point - Vector2(8, 8), point + Vector2(8, 8), Color("bb7972"), 3)
		return
	draw_circle(point + Vector2(0, 5), 12, Color(0, 0, 0, 0.3))
	draw_colored_polygon(PackedVector2Array([point + Vector2(0, -16), point + Vector2(11, 0), point + Vector2(0, 10), point + Vector2(-11, 0)]), color)
	draw_rect(Rect2(point + Vector2(-20, -26), Vector2(40, 4)), Color("172329"))
	draw_rect(Rect2(point + Vector2(-20, -26), Vector2(40.0 * actor.current_health / actor.max_health, 4)), color)
	draw_string(ThemeDB.fallback_font, point + Vector2(-28, -33), title, HORIZONTAL_ALIGNMENT_LEFT, -1, 12, color)
