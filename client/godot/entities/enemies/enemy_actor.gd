class_name EnemyActor
extends ActorBase
const ATLAS := preload("res://assets/painted/actors_atlas.png")
const WOLF_WALK := preload("res://assets/painted/wolf_walk.png")
const CULTIST_WALK := preload("res://assets/painted/cultist_walk.png")
const MOSS_WALK := preload("res://assets/painted/moss_claws_walk.png")
const CULTIST_FEET := [683.0, 678.0, 684.0, 672.0]
const MOSS_FEET := [637.0, 638.0, 640.0, 623.0]
@export var definition: EnemyDefinition
var brain: SliceEnemyFSM
var flash := 0.0
var walk_clock := 0.0
func _ready() -> void:
	if definition == null or not definition.valid():
		combat_enabled = false
		return
	max_health = definition.max_health
	defense = definition.defense
	movement_speed = definition.movement_speed
	super._ready()
	add_to_group("enemies")
	if not has_node("Shape"):
		var collision := CollisionShape2D.new()
		var shape := CircleShape2D.new()
		shape.radius = 34 if definition.category == &"sub_boss" else 20
		collision.shape = shape
		collision.name = "Shape"
		add_child(collision)
	brain = SliceEnemyFSM.new()
	brain.name = "Brain"
	brain.actor = self
	brain.spec = definition
	add_child(brain)
func _physics_process(delta: float) -> void:
	if brain == null: return
	var player := get_tree().get_first_node_in_group("player") as Player
	brain.step(delta, player)
	if not is_dead: move_and_slide()
	if not is_dead and get_real_velocity().length() > 8:
		walk_clock += delta * (8.0 if definition.category == &"common" else 4.0)
	else:
		walk_clock = 0.0
	flash = maxf(0, flash - delta)
	queue_redraw()
func interrupt(seconds: float) -> void:
	if brain != null: brain.interrupt(seconds)
func take_damage(amount: int, attacker: ActorBase) -> bool:
	var applied := super.take_damage(amount, attacker)
	if applied:
		flash = 0.12
		queue_redraw()
		if is_dead:
			_fade_out()
	return applied
func restore_dead() -> void:
	super.restore_dead()
	queue_free()
func _fade_out() -> void:
	var fade := create_tween()
	fade.tween_property(self, "modulate:a", 0.0, 0.3)
	fade.tween_callback(queue_free)
func _draw() -> void:
	if definition == null: return
	if is_dead: return
	if brain != null and brain.phase == "windup":
		var forward := Vector2(brain.locked_direction.x, brain.locked_direction.y * 0.5)
		var progress := clampf(1.0 - brain.remaining / brain.windup_for(brain.pattern), 0.0, 1.0)
		var col := Color("#a94750", lerpf(0.26, 0.64, progress))
		var edge := Color("#f5eee2", 0.94)
		if definition.category == &"sub_boss" and brain.pattern == 2:
			var p := brain.locked_position - global_position
			var width := definition.roots_half_width - definition.corridor_half_width
			var left := -definition.roots_half_width if brain.roots_side < 0 else definition.corridor_half_width
			var area := Rect2(p + Vector2(left, -definition.roots_depth), Vector2(width, definition.roots_depth * 2))
			draw_rect(area, col)
			draw_rect(area, edge, false, 3.0)
		else:
			draw_set_transform(Vector2.ZERO,0,Vector2(1,0.5))
			if definition.category == &"sub_boss" and brain.pattern == 0:
				var polygon := PackedVector2Array([Vector2.ZERO])
				var angle := brain.locked_direction.angle()
				for i in range(25):
					polygon.append(Vector2.from_angle(angle-PI/3+i*TAU/3/24)*(definition.attack_range+15))
				draw_colored_polygon(polygon,col)
				polygon.append(Vector2.ZERO)
				draw_polyline(polygon, edge, 3.0)
			else:
				var direction := brain.locked_direction
				var side := direction.orthogonal()*30
				var reach := definition.charge_speed*definition.charge_duration+30
				if definition.enemy_id == &"blind_cultist": reach = definition.attack_range
				var area := PackedVector2Array([-side,side,direction*reach+side,direction*reach-side])
				draw_colored_polygon(area,col)
				area.append(-side)
				draw_polyline(area, edge, 3.0)
			draw_set_transform(Vector2.ZERO)
			draw_arc(Vector2.ZERO, 27, 0, TAU * progress, 30, edge, 3)
	draw_set_transform(Vector2(0, 8), 0, Vector2(1, 0.23))
	var shadow_radius := 58.0 if definition.category == &"sub_boss" else (22.0 if definition.enemy_id == &"blind_cultist" else 17.0)
	draw_circle(Vector2.ZERO, shadow_radius, Color("#202331", 0.54))
	draw_set_transform(Vector2.ZERO)
	var tint := Color("#fff5dc") if flash > 0 else Color.WHITE
	var source := atlas_cell(ATLAS, 1)
	var visual := Rect2(-42, -67, 84, 84)
	if definition.enemy_id == &"blind_cultist":
		source = atlas_cell(ATLAS, 2)
		visual = Rect2(-43, -66, 86, 86)
	elif definition.enemy_id == &"moss_claws":
		source = atlas_cell(ATLAS, 3)
		visual = Rect2(-90, -134, 180, 152)
	if definition.enemy_id == &"ethereal_wolf" and walk_clock > 0:
		var frame := int(walk_clock) % 4
		var cell_width := WOLF_WALK.get_width() / 4.0
		draw_texture_rect_region(WOLF_WALK, visual, Rect2(frame * cell_width, 0, cell_width, WOLF_WALK.get_height()), tint)
	elif definition.enemy_id in [&"blind_cultist", &"moss_claws"] and walk_clock > 0:
		var frame := int(walk_clock) % 4
		var texture: Texture2D = CULTIST_WALK if definition.enemy_id == &"blind_cultist" else MOSS_WALK
		var feet: Array = CULTIST_FEET if definition.enemy_id == &"blind_cultist" else MOSS_FEET
		var cell_width := texture.get_width() / 4.0
		visual.position.y = 10.0 - float(feet[frame]) / texture.get_height() * visual.size.y
		draw_texture_rect_region(texture, visual, Rect2(frame * cell_width, 0, cell_width, texture.get_height()), tint)
	else:
		draw_texture_rect_region(ATLAS, visual, source, tint)
	draw_set_transform(Vector2.ZERO)
	var bar_y := -81.0 if definition.category == &"common" else -143.0
	draw_rect(Rect2(-22, bar_y, 44, 4), Color("#24252a"))
	draw_rect(Rect2(-22, bar_y, 44.0 * current_health / max_health, 4), Color("#c5b47d"))

static func atlas_cell(texture: Texture2D, frame: int) -> Rect2:
	var cell := Vector2(texture.get_width(), texture.get_height()) / 2.0
	return Rect2(Vector2(frame % 2, frame / 2) * cell, cell)
