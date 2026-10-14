class_name SliceMap
extends Node2D

@export var zone_id := "citadel"
const CITADEL_ART := preload("res://assets/painted/citadel_courtyard.png")
const FOREST_ART := preload("res://assets/painted/forest_village.png")
const NPC_ART := preload("res://assets/painted/citadel_npcs.png")
const NPC_ORDER := ["npc.blacksmith", "npc.elder", "npc.alchemist", "npc.gatekeeper", "npc.merchant"]
var interactions: Array[Dictionary] = []
var spawn := Vector2(790, 555)
var map_size := Vector2(1672, 941)
var flags: Dictionary = {}
var pillars: Array[Vector2] = []

func _ready() -> void:
	if zone_id == "citadel":
		map_size = Vector2(1672, 941)
		spawn = Vector2(790, 555)
		interactions = [
			{"id": "crystal", "name": "Fragmento de Éter", "position": Vector2(790, 430)},
			{"id": "npc.blacksmith", "name": "Bram", "position": Vector2(410, 315)},
			{"id": "npc.alchemist", "name": "Lyra", "position": Vector2(1070, 295)},
			{"id": "npc.elder", "name": "Irian", "position": Vector2(780, 245)},
			{"id": "npc.gatekeeper", "name": "Maera", "position": Vector2(1410, 535)},
			{"id": "npc.merchant", "name": "Sera", "position": Vector2(390, 710)},
			{"id": "gate", "name": "Bosque das Névoas", "position": Vector2(1510, 545)},
			{"id": "stash", "name": "Recompensas guardadas", "position": Vector2(430, 690)}
		]
		for obstacle in [Rect2(95, 125, 300, 165), Rect2(690, 5, 195, 170), Rect2(1090, 125, 205, 120), Rect2(115, 585, 225, 150), Rect2(1535, 355, 135, 235)]:
			wall(obstacle.get_center(), obstacle.size)
		for entry in interactions:
			if NPC_ORDER.has(entry.id): wall(entry.position, Vector2(18, 16))
	else:
		map_size = Vector2(1920, 1080)
		spawn = Vector2(265, 850)
		interactions = [
			{"id": "gate", "name": "Voltar à Cidadela", "position": Vector2(160, 820)},
			{"id": "forest_01.clue.tracks", "name": "Pegadas na lama", "position": Vector2(485, 790)},
			{"id": "forest_01.clue.testimony", "name": "Mensageiro e moradora", "position": Vector2(745, 585)},
			{"id": "forest_01.clue.well", "name": "Poço da aldeia", "position": Vector2(965, 530)},
			{"id": "choice", "name": "Decidir o destino da aldeia", "position": Vector2(1090, 610)}
		]
		pillars = [Vector2(1450, 130), Vector2(1780, 310), Vector2(1390, 340)]
		for point in pillars:
			wall(point, Vector2(34, 46))
		wall(Vector2(665, 335), Vector2(280, 155))
		wall(Vector2(965, 500), Vector2(35, 35))
		# The painted village has a broken fence and a drop that are not rectangular.
		# Keep the walking route above the ravine while following its visible edge.
		wall_polygon(PackedVector2Array([
			Vector2(755, 1080), Vector2(785, 980), Vector2(860, 910),
			Vector2(945, 855), Vector2(1050, 790), Vector2(1150, 750),
			Vector2(1240, 690), Vector2(1335, 635), Vector2(1430, 575),
			Vector2(1570, 500), Vector2(1740, 430), Vector2(1920, 420),
			Vector2(1920, 1080)
		]))
		for abyss in [Rect2(1210, 610, 710, 470), Rect2(1380, 440, 540, 170), Rect2(1510, 370, 410, 70)]:
			wall(abyss.get_center(), abyss.size)
	for obstacle in [Rect2(0, 0, map_size.x, 20), Rect2(0, map_size.y - 20, map_size.x, 20), Rect2(0, 0, 20, map_size.y), Rect2(map_size.x - 20, 0, 20, map_size.y)]:
		wall(obstacle.get_center(), obstacle.size)
	queue_redraw()

func wall(center: Vector2, size: Vector2) -> void:
	var body := StaticBody2D.new()
	body.position = center
	body.collision_layer = 2
	body.collision_mask = 1
	var collision := CollisionShape2D.new()
	var shape := RectangleShape2D.new()
	shape.size = size
	collision.shape = shape
	body.add_child(collision)
	add_child(body)

func wall_polygon(points: PackedVector2Array) -> void:
	var body := StaticBody2D.new()
	body.collision_layer = 2
	body.collision_mask = 1
	var collision := CollisionPolygon2D.new()
	collision.polygon = points
	body.add_child(collision)
	add_child(body)

func nearest(point: Vector2) -> Dictionary:
	var found: Dictionary = {}
	var distance := 65.0
	for entry in interactions:
		var d := point.distance_to(entry.position)
		if d < distance:
			found = entry
			distance = d
	return found

func _draw() -> void:
	var texture: Texture2D = CITADEL_ART if zone_id == "citadel" else FOREST_ART
	draw_texture_rect(texture, Rect2(Vector2.ZERO, map_size), false)
	if zone_id == "citadel":
		if flags.get("hub.contamination_observed", false):
			draw_circle(Vector2(920, 505), 80, Color(0.30, 0.18, 0.48, 0.25))
	for entry in interactions:
		var point: Vector2 = entry.position
		draw_arc(point, 12, 0, TAU, 24, Color("#d1af70"), 2)
		draw_circle(point, 3, Color("#f5d9a2"))
		var npc_index := NPC_ORDER.find(entry.id)
		if zone_id == "citadel" and npc_index >= 0:
			var cell_width := NPC_ART.get_width() / 5.0
			var source := Rect2(npc_index * cell_width, 0, cell_width, NPC_ART.get_height())
			draw_texture_rect_region(NPC_ART, Rect2(point.x - 29, point.y - 107, 58, 107), source)
			var title := entry.name as String
			draw_string(ThemeDB.fallback_font, point + Vector2(-32, -112), title, HORIZONTAL_ALIGNMENT_CENTER, 64, 15, Color("#ffe0aa"))
