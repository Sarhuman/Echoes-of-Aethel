extends Node2D
## Read-only telegraph; never controls damage.
@onready var player: Player = get_parent().get_node("Player")

func _process(_delta: float) -> void:
	queue_redraw()

func _draw() -> void:
	if not is_instance_valid(player) or player.is_dead:
		return
	var controller := player.attacks
	if controller.phase == CombatService.Phase.IDLE:
		return
	var color := Color(0.85, 0.75, 0.35, 0.7)
	if controller.phase == CombatService.Phase.ACTIVE:
		color = Color(1.0, 0.4, 0.2, 0.9)
	elif controller.phase == CombatService.Phase.RECOVERY:
		color = Color(0.5, 0.6, 0.7, 0.4)
	var origin := to_local(player.global_position)
	var points := PackedVector2Array([origin])
	for i in range(17):
		var angle := controller.direction.angle() - PI / 3.0 + (2.0 * PI / 3.0) * i / 16.0
		var offset := Vector2.from_angle(angle) * controller.definition.reach
		points.append(origin + Vector2(offset.x, offset.y * 0.5))
	points.append(origin)
	draw_polyline(points, color, 2.0)
