class_name ArenaNavigation
extends RefCounted
## Static arena geometry shared by all agents; routes remain actor-local.
const CELL := 20.0
var grid := AStarGrid2D.new()

func configure(bounds: Rect2, obstacles: Array[Rect2]) -> void:
	grid.region = Rect2i(0, 0, int(bounds.size.x / CELL), int(bounds.size.y / CELL))
	grid.cell_size = Vector2.ONE * CELL
	grid.offset = bounds.position + Vector2.ONE * CELL * 0.5
	grid.diagonal_mode = AStarGrid2D.DIAGONAL_MODE_ONLY_IF_NO_OBSTACLES
	grid.update()
	for x in range(grid.region.size.x):
		for y in range(grid.region.size.y):
			var id := Vector2i(x, y)
			var point := grid.get_point_position(id)
			for obstacle in obstacles:
				if obstacle.grow(20.0).has_point(point):
					grid.set_point_solid(id)
					break

func cell(point: Vector2) -> Vector2i:
	return Vector2i(((point - grid.offset) / CELL).round())

func route(origin: Vector2, destination: Vector2) -> PackedVector2Array:
	var start := cell(origin)
	var finish := cell(destination)
	if not grid.is_in_boundsv(start) or not grid.is_in_boundsv(finish):
		return PackedVector2Array()
	if grid.is_point_solid(start) or grid.is_point_solid(finish):
		return PackedVector2Array()
	return grid.get_point_path(start, finish)
