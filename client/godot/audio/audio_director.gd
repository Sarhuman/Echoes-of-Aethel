class_name AethelAudioDirector
extends Node

const TRACKS := {
	"prologue": preload("res://audio/music/prologue.wav"),
	"citadel": preload("res://audio/music/citadel.wav"),
	"forest_01": preload("res://audio/music/forest.wav"),
	"combat": preload("res://audio/music/combat.wav")
}
const EFFECTS := {
	"basic": preload("res://audio/sfx/axe.wav"),
	"bulwark_impact": preload("res://audio/sfx/impact.wav"),
	"twilight_guard": preload("res://audio/sfx/guard.wav"),
	"steel_rift": preload("res://audio/sfx/rift.wav"),
	"dash": preload("res://audio/sfx/dash.wav"),
	"hit": preload("res://audio/sfx/hit.wav")
}
var enabled := true
var zone := "citadel"
var fighting := false
var cinematic := false
var _music: Dictionary = {}
var _effects: Array[AudioStreamPlayer] = []
var _effect_cursor := 0

func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	for id in TRACKS:
		var player := AudioStreamPlayer.new()
		player.name = "Music_" + id
		player.stream = TRACKS[id]
		player.volume_db = -80
		add_child(player)
		_music[id] = player
	for i in range(5):
		var player := AudioStreamPlayer.new()
		player.name = "Effect_%d" % i
		player.volume_db = -8
		add_child(player)
		_effects.append(player)

func _process(delta: float) -> void:
	var active := "prologue" if cinematic else ("combat" if fighting and zone == "forest_01" else zone)
	for id in _music:
		var player: AudioStreamPlayer = _music[id]
		var target := (-19.0 if id == active and enabled else -80.0)
		player.volume_db = move_toward(player.volume_db, target, delta * 48.0)
		if enabled and id == active and not player.playing:
			player.play()
		if player.volume_db <= -79.9 and player.playing:
			player.stop()

func set_context(new_zone: String, in_combat: bool) -> void:
	zone = new_zone if TRACKS.has(new_zone) else "citadel"
	fighting = in_combat

func set_cinematic(active: bool) -> void:
	cinematic = active

func play_effect(id: StringName) -> void:
	if not enabled or not EFFECTS.has(String(id)): return
	var player := _effects[_effect_cursor]
	_effect_cursor = (_effect_cursor + 1) % _effects.size()
	player.stop()
	player.stream = EFFECTS[String(id)]
	player.play()
