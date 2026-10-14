class_name EnemyActor
extends ActorBase
const ATLAS := preload("res://assets/painted/actors_atlas.png")
@export var definition: EnemyDefinition
var brain: SliceEnemyFSM
var flash := 0.0
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
		shape.radius = 17 if definition.category == &"sub_boss" else 9
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
	flash = maxf(0, flash - delta)
	queue_redraw()
func interrupt(seconds: float) -> void:
	if brain != null: brain.interrupt(seconds)
func take_damage(amount: int, attacker: ActorBase) -> bool:
	var applied := super.take_damage(amount, attacker)
	if applied:
		flash = 0.12
		queue_redraw()
	return applied
func _draw() -> void:
	if definition == null: return
	if is_dead:
		draw_circle(Vector2.ZERO, 9, Color("#474f41"))
		return
	if brain != null and brain.phase == "windup":
		var forward := Vector2(brain.locked_direction.x, brain.locked_direction.y * 0.5)
		var col := Color(0.95, 0.48, 0.29, 0.45)
		if definition.category == &"sub_boss" and brain.pattern == 2:
			var p := brain.locked_position - global_position
			var width := definition.roots_half_width - definition.corridor_half_width
			var left := -definition.roots_half_width if brain.roots_side < 0 else definition.corridor_half_width
			draw_rect(Rect2(p + Vector2(left, -definition.roots_depth), Vector2(width, definition.roots_depth * 2)), col)
		else:
			draw_set_transform(Vector2.ZERO,0,Vector2(1,0.5))
			if definition.category == &"sub_boss" and brain.pattern == 0:
				var polygon := PackedVector2Array([Vector2.ZERO])
				var angle := brain.locked_direction.angle()
				for i in range(25):
					polygon.append(Vector2.from_angle(angle-PI/3+i*TAU/3/24)*(definition.attack_range+15))
				draw_colored_polygon(polygon,col)
			else:
				var direction := brain.locked_direction
				var side := direction.orthogonal()*30
				var reach := definition.charge_speed*definition.charge_duration+30
				if definition.enemy_id == &"blind_cultist": reach = definition.attack_range
				draw_colored_polygon(PackedVector2Array([-side,side,direction*reach+side,direction*reach-side]),col)
			draw_set_transform(Vector2.ZERO)
			draw_arc(Vector2.ZERO, 27, 0, TAU * clampf(1.0 - brain.remaining / brain.windup_for(brain.pattern), 0, 1), 30, Color("#efb875"), 2)
	draw_set_transform(Vector2.ZERO, 0, Vector2(1, 0.5))
	draw_circle(Vector2(0, 12), 18 if definition.category == &"sub_boss" else 12, Color(0, 0, 0, 0.4))
	draw_set_transform(Vector2.ZERO)
	var tint := Color("#fff5dc") if flash > 0 else Color.WHITE
	var source := Rect2(640, 0, 640, 640)
	var visual := Rect2(-42, -67, 84, 84)
	if definition.enemy_id == &"blind_cultist":
		source = Rect2(0, 640, 640, 640)
		visual = Rect2(-43, -72, 86, 86)
	elif definition.enemy_id == &"moss_claws":
		source = Rect2(640, 640, 640, 640)
		visual = Rect2(-90, -136, 180, 152)
	draw_texture_rect_region(ATLAS, visual, source, tint)
	var bar_y := -81.0 if definition.category == &"common" else -143.0
	draw_rect(Rect2(-22, bar_y, 44, 4), Color("#24252a"))
	draw_rect(Rect2(-22, bar_y, 44.0 * current_health / max_health, 4), Color("#c5b47d"))
