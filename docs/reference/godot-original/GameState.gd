# core/GameState.gd
# Gerenciador de Estado Global (Singleton / Autoload)
# Responsável por reter a persistência de escolhas, estados de facções e flags do mundo.

class_name GameStateNode

# Dicionário de flags genéricas de decisões (Ex: "poupou_guarda_ponte": true)
var world_flags: Dictionary = {}

# Dicionário que mapeia o ID único do NPC ao seu estado atual ("vivo", "morto", "hostil")
var npc_states: Dictionary = {}

# Dicionário de reputação com grupos/facções do jogo (Valores de -100 a 100)
var faction_reputation: Dictionary = {
	"default_faction": 0
}

# Emite sinais quando mudanças drásticas no mundo ocorrem
signal flag_changed(flag_name: String, new_value)
signal npc_state_changed(npc_id: String, new_state: String)

func set_world_flag(flag_name: String, value) -> void:
	world_flags[flag_name] = value
	flag_changed.emit(flag_name, value)

func get_world_flag(flag_name: String, default_value = false):
	if world_flags.has(flag_name):
		return world_flags[flag_name]
	return default_value

func set_npc_state(npc_id: String, state: String) -> void:
	npc_states[npc_id] = state
	npc_state_changed.emit(npc_id, state)
	
	# Se o estado mudou para hostil, pode disparar lógica de alerta local via EventBus
	if state == "hostil":
		if %EventBus: # Substituir pela referência real do seu EventBus
			%EventBus.emit_signal("crime_committed", npc_id)

func get_npc_state(npc_id: String) -> String:
	if npc_states.has(npc_id):
		return npc_states[npc_id]
	return "normal" # Estado padrão se nada aconteceu

func modify_reputation(faction_id: String, amount: int) -> void:
	if not faction_reputation.has(faction_id):
		faction_reputation[faction_id] = 0
	faction_reputation[faction_id] = clampi(faction_reputation[faction_id] + amount, -100, 100)
