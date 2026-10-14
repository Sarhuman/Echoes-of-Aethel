extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func run() -> void:
	var text := AethelCombatText.new()
	root.add_child(text)
	text.show_damage(Vector2(100, 200), 42)
	check(text.entries.size() == 1 and text.entries[0].text == "42" and text.entries[0].color.r > text.entries[0].color.b, "actual damage appears as red number")
	text.show_miss(Vector2(130, 200))
	check(text.entries.size() == 2 and text.entries[1].text == "MISS" and text.entries[1].color.b > text.entries[1].color.r, "failed attack appears blue")
	text._process(1.0)
	check(text.entries.is_empty(), "combat text expires")
	text.queue_free()
	await process_frame
	print("Combat text: %d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
