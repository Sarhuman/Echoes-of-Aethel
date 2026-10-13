extends Node2D

var unavailable_notice := ""

@onready var player: Player = $Player
@onready var status: Label = $HUD/Panel/Status
@onready var hp_bar: ProgressBar = $HUD/Panel/HP
@onready var mp_bar: ProgressBar = $HUD/Panel/MP
@onready var stamina_bar: ProgressBar = $HUD/Panel/Stamina

func _ready() -> void:
	player.skill_unavailable.connect(func(slot): unavailable_notice = "Habilidade %d ainda nao implementada." % slot)
	player.attack_started.connect(func(_attack): unavailable_notice = "")
	player.mp_changed.connect(func(current, maximum): mp_bar.max_value = maximum; mp_bar.value = current)
	mp_bar.max_value = player.mp_max
	mp_bar.value = player.mp
	player.hp_changed.connect(func(current, maximum): hp_bar.max_value = maximum; hp_bar.value = current)
	player.stamina_changed.connect(func(current, maximum): stamina_bar.max_value = maximum; stamina_bar.value = current)
	hp_bar.max_value = player.max_health
	hp_bar.value = player.current_health
	stamina_bar.max_value = player.stamina_max
	stamina_bar.value = player.stamina

func _process(_delta: float) -> void:
	status.text = "HP %d/%d | MP %.0f/%.0f | Stamina %.1f | CD dash %.2f s\nFase %s | Inimigo HP %d | Morto %s\nWASD/setas: mover | Espaco: dash | 1: basico | 2/Q: pesado (12 MP)\nParede bloqueia movimento e ataque. F6: reiniciar. 3/E, 4/R, 5/F: habilidades futuras.\n%s" % [player.current_health, player.max_health, player.mp, player.mp_max, player.stamina, player.cooldown_left, Player.ActionState.keys()[player.current_action_state], $Enemy.current_health, str(player.is_dead), unavailable_notice]

func _unhandled_key_input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.echo and event.is_action_pressed("debug_restart"):
		get_tree().reload_current_scene()

