class_name CombatPresentation
extends Node2D

signal mode_changed(active: bool)
@export var enhanced := false
@export var flashes_enabled := true
@export var particles_enabled := true
@export var reduced_effects := false
@export var flash_duration := 0.033
@export var impact_duration := 0.25
@export var max_impacts := 24
var _actors: Dictionary = {}
var _original_colors: Dictionary = {}
var _flashes: Dictionary = {}
var _impacts: Array[Dictionary] = []
var _bus: Node
var _label: Label

func _ready() -> void:
	_bus = get_node("/root/EventBus")
	for path in ["Player", "Enemy"]:
		var actor := get_parent().get_node_or_null(path) as ActorBase
		if actor != null:
			_actors[String(actor.actor_id)] = actor
			var visual := actor.get_node_or_null("Visual") as Polygon2D
			if visual != null:
				_original_colors[String(actor.actor_id)] = visual.color
	_bus.actor_damaged.connect(_on_actor_damaged)
	var layer := CanvasLayer.new()
	_label = Label.new()
	_label.position = Vector2(16, 175)
	layer.add_child(_label)
	add_child(layer)
	var args := OS.get_cmdline_user_args()
	flashes_enabled = flashes_enabled and not args.has("--no-flash")
	particles_enabled = particles_enabled and not args.has("--no-particles")
	reduced_effects = reduced_effects or args.has("--reduced-effects")
	set_enhanced(enhanced or args.has("--enhanced"))

func _exit_tree() -> void:
	_restore_visuals()
	if is_instance_valid(_bus) and _bus.actor_damaged.is_connected(_on_actor_damaged):
		_bus.actor_damaged.disconnect(_on_actor_damaged)

func _unhandled_key_input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.echo and event.physical_keycode == KEY_F9:
		set_reduced_effects(not reduced_effects)
		get_viewport().set_input_as_handled()
	if event is InputEventKey and event.pressed and not event.echo and event.physical_keycode == KEY_F8:
		set_enhanced(not enhanced)
		get_viewport().set_input_as_handled()

func set_reduced_effects(active: bool) -> void:
	reduced_effects = active
	if active:
		_restore_visuals()
		_flashes.clear()
		_impacts.clear()
	set_enhanced(enhanced)

func set_enhanced(active: bool) -> void:
	if not active:
		_restore_visuals()
		_flashes.clear()
		_impacts.clear()
	enhanced = active
	if is_instance_valid(_label):
		_label.text = "Apresentacao: %s | F8 alterna | Efeitos reduzidos: %s" % ["Melhorada" if enhanced else "Base", str(reduced_effects)]
	queue_redraw()
	mode_changed.emit(active)

func _restore_visuals() -> void:
	for id in _original_colors:
		var actor = _actors.get(id)
		if is_instance_valid(actor):
			var visual := actor.get_node_or_null("Visual") as Polygon2D
			if visual != null:
				visual.color = _original_colors[id]

func _on_actor_damaged(id: String, hp_damage: int, _attacker_id: String) -> void:
	if not enhanced or reduced_effects or hp_damage <= 0:
		return
	var actor = _actors.get(id)
	if not is_instance_valid(actor):
		return
	if flashes_enabled:
		var visual := actor.get_node_or_null("Visual") as Polygon2D
		if visual != null:
			visual.color = Color.WHITE
			_flashes[id] = maxf(flash_duration, 0.0)
	if particles_enabled:
		if _impacts.size() >= maxi(1, max_impacts):
			_impacts.pop_front()
		_impacts.append({"position": to_local(actor.global_position), "left": maxf(impact_duration, 0.001)})
	queue_redraw()

func _process(delta: float) -> void:
	for id in _flashes.keys():
		_flashes[id] -= delta
		if _flashes[id] <= 0.0:
			var actor = _actors.get(id)
			if is_instance_valid(actor):
				var visual := actor.get_node_or_null("Visual") as Polygon2D
				if visual != null:
					visual.color = _original_colors[id]
			_flashes.erase(id)
	var had_impacts := not _impacts.is_empty()
	for i in range(_impacts.size() - 1, -1, -1):
		_impacts[i]["left"] -= delta
		if _impacts[i]["left"] <= 0.0:
			_impacts.remove_at(i)
	if had_impacts or not _impacts.is_empty():
		queue_redraw()

func _draw() -> void:
	if not enhanced or reduced_effects:
		return
	for impact in _impacts:
		var ratio := clampf(float(impact["left"]) / maxf(impact_duration, 0.001), 0.0, 1.0)
		var radius := 8.0 + (1.0 - ratio) * 12.0
		var color := Color(0.9, 0.8, 0.5, ratio)
		draw_arc(impact["position"], radius, 0, TAU, 16, color, 1.0, false)
		for index in range(4):
			var direction := Vector2.RIGHT.rotated(index * PI / 2.0)
			draw_line(impact["position"] + direction * radius, impact["position"] + direction * (radius + 4), color, 1.0, false)
