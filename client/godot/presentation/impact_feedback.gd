class_name AethelImpactFeedback
extends Node

var camera: Camera2D
var enabled := true
var time_left := 0.0
var duration := 0.0
var amplitude := 0.0

func set_camera(new_camera: Camera2D) -> void:
	if is_instance_valid(camera): camera.offset = Vector2.ZERO
	camera = new_camera
	reset()

func impulse(strength: float) -> void:
	if not enabled or not is_instance_valid(camera) or strength <= 0.0: return
	amplitude = minf(6.0, maxf(amplitude, strength))
	duration = 0.17
	time_left = duration

func reset() -> void:
	time_left = 0.0
	amplitude = 0.0
	if is_instance_valid(camera): camera.offset = Vector2.ZERO

func _process(delta: float) -> void:
	if not enabled or not is_instance_valid(camera):
		reset()
		return
	if time_left <= 0.0: return
	time_left = maxf(0.0, time_left - delta)
	var fade := time_left / duration
	var phase := (duration - time_left) * 75.0
	camera.offset = Vector2(sin(phase), cos(phase * 0.73)) * amplitude * fade
	if time_left <= 0.0: camera.offset = Vector2.ZERO
