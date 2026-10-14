extends Node2D
const IDLE_ART := preload("res://assets/painted/actors_atlas.png")
const WALK_ART := preload("res://assets/painted/bastion_walk.png")
const ATTACK_ART := preload("res://assets/painted/bastion_attack.png")
const WALK_FEET := [602.0, 597.0, 560.0, 560.0]
const ATTACK_FEET := [598.0, 602.0, 540.0, 555.0]
var player: Player
var walk_clock := 0.0
var flash_left := 0.0
var last_action: StringName

func _ready() -> void:
	player = get_parent() as Player
	player.attack_started.connect(_action_started)

func _action_started(id: StringName) -> void:
	last_action = id
	flash_left = 0.65

func _process(delta: float) -> void:
	if player.velocity.length() > 2 and not player.is_dashing:
		walk_clock += delta
	else:
		walk_clock = 0.0
	flash_left = maxf(0.0, flash_left - delta)
	queue_redraw()

func _draw() -> void:
	if player == null: return
	draw_set_transform(Vector2(0, 8), 0, Vector2(1, 0.5))
	draw_circle(Vector2.ZERO, 23, Color(0, 0, 0, 0.48))
	draw_set_transform(Vector2.ZERO)
	var tint := Color("#d8ffff") if player.is_invulnerable else Color.WHITE
	var attacking := player.attacks.phase != CombatService.Phase.IDLE and player.attacks.definition != null
	if player.facing.x < -0.1:
		draw_set_transform(Vector2.ZERO, 0, Vector2(-1, 1))
	if attacking:
		var spec := player.attacks.definition
		var duration := spec.windup + spec.active + spec.recovery
		var frame := clampi(int(player.attacks.elapsed / duration * 4.0), 0, 3)
		draw_frame(ATTACK_ART, frame, Vector2(108, 108), tint)
	elif player.velocity.length() > 2 and not player.is_dashing:
		var frame := int(walk_clock * 9.0) % 4
		draw_frame(WALK_ART, frame, Vector2(94, 94), tint)
	else:
		draw_texture_rect_region(IDLE_ART, Rect2(-47, -75, 94, 94), atlas_cell(IDLE_ART, 0), tint)
	draw_set_transform(Vector2.ZERO)
	if player.is_dashing:
		var trail := -player.dash_direction * 28
		draw_line(Vector2.ZERO, Vector2(trail.x, trail.y * 0.5), Color(0.51, 0.87, 0.97, 0.56), 12)
		draw_arc(Vector2.ZERO, 24, 0, TAU, 28, Color(0.63, 0.94, 1, 0.68), 3)
	if player.guard.active_left > 0:
		var center := player.project_direction(player.facing) * 33 + Vector2(0, -21)
		var shield := PackedVector2Array([center + Vector2(-12, -14), center + Vector2(12, -14), center + Vector2(11, 8), center + Vector2(0, 19), center + Vector2(-11, 8)])
		draw_colored_polygon(shield, Color(0.12, 0.42, 0.55, 0.29))
		for i in range(shield.size()):
			draw_line(shield[i], shield[(i + 1) % shield.size()], Color(0.50, 0.88, 0.97, 0.72), 2)
	if flash_left > 0 and last_action == &"twilight_guard":
		draw_circle(Vector2.ZERO, 20 + (0.65 - flash_left) * 28, Color(0.26, 0.88, 0.97, flash_left * 0.06))
	if attacking:
		draw_attack_effect()

func draw_frame(texture: Texture2D, frame: int, dimensions: Vector2, tint: Color) -> void:
	var source := atlas_cell(texture, frame)
	var feet: Array = WALK_FEET if texture == WALK_ART else ATTACK_FEET
	var alignment := (600.0 - float(feet[frame])) / source.size.y * dimensions.y
	draw_texture_rect_region(texture, Rect2(-dimensions.x / 2, -dimensions.y + 15 + alignment, dimensions.x, dimensions.y), source, tint)

static func atlas_cell(texture: Texture2D, frame: int) -> Rect2:
	var cell := Vector2(texture.get_width(), texture.get_height()) / 2.0
	return Rect2(Vector2(frame % 2, frame / 2) * cell, cell)

func draw_attack_effect() -> void:
	var spec := player.attacks.definition
	var phase := player.attacks.phase
	var direction := player.attacks.direction
	var angle := direction.angle()
	var alpha := 0.34 if phase == CombatService.Phase.WINDUP else (0.82 if phase == CombatService.Phase.ACTIVE else 0.28)
	draw_set_transform(Vector2.ZERO, 0, Vector2(1, 0.5))
	if spec.line_width > 0:
		var side := direction.orthogonal() * spec.line_width
		var reach := direction * spec.reach
		draw_colored_polygon(PackedVector2Array([-side, side, reach + side, reach - side]), Color(0.35, 0.73, 1.0, alpha * 0.36))
		draw_line(Vector2.ZERO, reach, Color(0.73, 0.89, 1.0, alpha), 7)
	else:
		var spread := 0.9 if spec.heavy else 0.65
		var color := Color(1.0, 0.72, 0.34, alpha) if spec.heavy else Color(0.85, 0.94, 1.0, alpha)
		var fan := PackedVector2Array([Vector2.ZERO])
		for index in range(17):
			fan.append(Vector2.from_angle(angle - spread + index * 2.0 * spread / 16.0) * spec.reach)
		draw_colored_polygon(fan, Color(color.r, color.g, color.b, alpha * 0.19))
		draw_arc(Vector2.ZERO, spec.reach, angle - spread, angle + spread, 24, color, 8)
		draw_arc(Vector2.ZERO, spec.reach * 0.73, angle - spread, angle + spread, 24, Color(color.r, color.g, color.b, alpha * 0.54), 4)
	draw_set_transform(Vector2.ZERO)
