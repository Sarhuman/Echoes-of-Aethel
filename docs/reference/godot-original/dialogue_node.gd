# systems/dialogue/dialogue_node.gd
# Estrutura de dados modular para compor ramificações de texto e verificadores de escolhas.

class_name DialogueNode

var node_id: String = ""
var text_lines: Array[String] = []
var choices: Array[DialogueChoice] = []

class DialogueChoice:
	var choice_text: String = ""
	var target_node_id: String = "" # Próximo nó se escolhido
	
	# Requisitos estruturais para a opção aparecer (Ex: {"flag_needed": "sabe_da_verdade"})
	var requirements: Dictionary = {}
	
	# Modificações que esta escolha aplica ao mundo quando clicada
	# Ex: {"set_flag": {"rejeitou_ajuda": true}, "rep_change": {"guards": -10}}
	var consequences: Dictionary = {}

	func is_available(world_flags: Dictionary, faction_rep: Dictionary) -> bool:
		# Lógica interna para checar se os requisitos batem com as variáveis globais
		if requirements.is_empty():
			return true
		return true # Substituir pelas verificações reais de chaves/valores
