extends Node2D

@onready var player: Player = $Player
@onready var status: Label = $HUD/Panel/Status
@onready var hp_bar: ProgressBar = $HUD/Panel/HP
@onready var stamina_bar: ProgressBar = $HUD/Panel/Stamina

func _ready() -> void:
	player.hp_changed.connect(func(current, maximum): hp_bar.max_value = maximum; hp_bar.value = current)
	player.stamina_changed.connect(func(current, maximum): stamina_bar.max_value = maximum; stamina_bar.value = current)
	hp_bar.max_value = player.max_health
	hp_bar.value = player.current_health
	stamina_bar.max_value = player.stamina_max
	stamina_bar.value = player.stamina

func _process(_delta: float) -> void:
	status.text = "HP %d/%d | Stamina %.1f | i-frames %.0f ms | CD %.2f s\nDash %s | Fase %s | Morto %s\nWASD/setas: mover | Espaco: dash | J: recovery normal | K: windup pesado\nVermelho: inimigo FSM (15 dano). Parede bloqueia movimento e ataque.\nCena tecnica; sem quest, MP ou ataque do jogador implementados." % [player.current_health, player.max_health, player.stamina, player.iframe_left * 1000.0, player.cooldown_left, str(player.is_dashing), Player.ActionState.keys()[player.current_action_state], str(player.is_dead)]
