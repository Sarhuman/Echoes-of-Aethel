extends SceneTree
var failures := 0
var checks := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func blocked(world: Node2D, point: Vector2) -> bool:
	var query := PhysicsPointQueryParameters2D.new()
	query.position = point
	query.collision_mask = 2
	return not world.get_world_2d().direct_space_state.intersect_point(query).is_empty()
func run() -> void:
	var map := preload("res://world/maps/forest_01.tscn").instantiate() as SliceMap
	root.add_child(map)
	await physics_frame
	check(blocked(map, Vector2(980, 1000)), "southern ravine is impassable")
	check(blocked(map, Vector2(1390, 760)), "eastern ravine is impassable")
	check(not blocked(map, map.spawn), "forest entrance remains walkable")
	check(not blocked(map, Vector2(1090, 610)), "quest decision remains reachable")
	check(not blocked(map, Vector2(1600, 220)), "boss arena remains reachable")
	map.queue_free()
	await process_frame
	print("Map boundaries: %d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
