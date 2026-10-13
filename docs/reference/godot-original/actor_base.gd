# entities/actor_base.gd
# Classe base para qualquer entidade física móvel e interativa no mundo 2D Isométrico.

class_name ActorBase

# Propriedades físicas básicas
var movement_speed: float = 100.0
var velocity: Vector2 = Vector2.ZERO

# Atributos de Sobrevivência básicos
var max_health: int = 100
var current_health: int = 100
var is_dead: bool = false

# Referências a nós comuns (Física 2D)
var character_body: Object # Representação conceitual do corpo físico 2D (ex: CharacterBody2D)

func _init():
	current_health = max_health

func take_damage(amount: int, attacker: Node) -> void:
	if is_dead:
		return
		
	current_health = max(0, current_health - amount)
	# Alerta o ecossistema que esta entidade sofreu dano
	# EventBus.actor_damaged.emit(self, amount, attacker)
	
	if current_health <= 0:
		die()

func die() -> void:
	is_dead = true
	# EventBus.actor_died.emit(self)
	# Executar lógica de desativação de colisão e disparo de animações de morte
