class_name AethelCombatText
extends Node2D

const LIFETIME := 0.8
var entries: Array[Dictionary] = []

func show_damage(world_position: Vector2, amount: int) -> void:
	if amount <= 0: return
	entries.append({"position": world_position + Vector2(0, -63), "text": str(amount), "color": Color("#ff5e57"), "left": LIFETIME})
	queue_redraw()

func show_miss(world_position: Vector2) -> void:
	entries.append({"position": world_position + Vector2(0, -44), "text": "MISS", "color": Color("#74caff"), "left": LIFETIME})
	queue_redraw()

func _process(delta: float) -> void:
	for index in range(entries.size() - 1, -1, -1):
		entries[index].left -= delta
		if entries[index].left <= 0:
			entries.remove_at(index)
	if not entries.is_empty(): queue_redraw()

func _draw() -> void:
	for entry in entries:
		var progress: float = 1.0 - float(entry.left) / LIFETIME
		var at: Vector2 = entry.position + Vector2(0, -28 * progress)
		var tint: Color = entry.color
		tint.a = minf(1.0, entry.left * 2.5)
		draw_string(ThemeDB.fallback_font, at + Vector2(-32, 1), entry.text, HORIZONTAL_ALIGNMENT_CENTER, 64, 20, Color(0, 0, 0, tint.a))
		draw_string(ThemeDB.fallback_font, at + Vector2(-32, 0), entry.text, HORIZONTAL_ALIGNMENT_CENTER, 64, 20, tint)
