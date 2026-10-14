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
	var camera := Camera2D.new()
	root.add_child(camera)
	var feedback := AethelImpactFeedback.new()
	root.add_child(feedback)
	feedback.set_camera(camera)
	feedback.impulse(4.0)
	feedback._process(0.03)
	check(camera.offset.length() > 0.0, "confirmed hit moves camera briefly")
	feedback._process(0.3)
	check(camera.offset == Vector2.ZERO, "camera returns to exact origin")
	feedback.enabled = false
	feedback.impulse(6.0)
	feedback._process(0.03)
	check(camera.offset == Vector2.ZERO and feedback.time_left == 0.0, "reduced effects suppress shake")
	feedback.enabled = true
	feedback.impulse(4.0)
	feedback.set_camera(null)
	check(camera.offset == Vector2.ZERO and feedback.time_left == 0.0, "zone transition resets previous camera")
	feedback.queue_free()
	camera.queue_free()
	await process_frame
	print("Impact feedback: %d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
