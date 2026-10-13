class_name ActorBase
extends CharacterBody2D

@export var actor_id: StringName
@export var zone_id: StringName = &"field"
@export var team_id: StringName = &"monsters"
@export var narrative_npc := false
@export var combat_enabled := true
@export var movement_speed := 100.0
@export var max_health := 100
var current_health := 100
var is_dead := false
@onready var _events: Node = get_node("/root/EventBus")
@onready var _world: Node = get_node("/root/GameState")
@onready var _saves: Node = get_node("/root/SaveSystem")

func _ready() -> void:
	current_health = maxi(1, max_health)

func _physics_process(_delta: float) -> void:
	if not is_dead:
		move_and_slide()

func take_damage(amount: int, attacker: ActorBase) -> bool:
	# Actor boundary validates every damage source, including the existing enemy FSM.
	if is_dead or not combat_enabled or amount <= 0 or not is_instance_valid(attacker):
		return false
	if zone_id == &"citadel" or attacker.zone_id == &"citadel":
		return false
	if attacker.is_dead or not attacker.combat_enabled or attacker.zone_id != zone_id:
		return false
	var actual := mini(amount, current_health)
	if narrative_npc and actual == current_health:
		var snapshot: Dictionary = _world.snapshot()
		snapshot["npc_states"][String(actor_id)] = "dead"
		if not _saves.commit_snapshot(snapshot, "npc_death")["ok"]:
			return false
	current_health -= actual
	if current_health == 0:
		is_dead = true
		velocity = Vector2.ZERO
		set_physics_process(false)
		for child in get_children():
			if child is CollisionShape2D:
				child.set_deferred("disabled", true)
	_events.actor_damaged.emit(String(actor_id), actual, String(attacker.actor_id))
	if is_dead:
		_events.actor_died.emit(String(actor_id))
	return true

func restore_dead() -> void:
	# Loading a persisted corpse must not emit death/loot again.
	is_dead = true
	current_health = 0
	velocity = Vector2.ZERO
	set_physics_process(false)
	for child in get_children():
		if child is CollisionShape2D:
			child.set_deferred("disabled", true)
