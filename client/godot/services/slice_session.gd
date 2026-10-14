class_name SliceSession
extends Node
signal changed
signal notice(text: String)
var player: Player
var inventory := InventoryModel.new()
var quest := QuestService.new()
var consumables := ConsumableService.new()
var simulation_time := 0.0
var zone := "citadel"
var pending_kills: Dictionary = {}
var retry_left := 0.0
var ready_to_play := false
@onready var state: Node = get_node("/root/GameState")
@onready var saves: Node = get_node("/root/SaveSystem")
func _physics_process(delta: float) -> void:
	simulation_time += delta
	retry_left -= delta
	if retry_left <= 0 and not pending_kills.is_empty():
		retry_left = 2
		for id in pending_kills.keys(): grant_kill(id, pending_kills[id])
func start(continue_game: bool) -> Dictionary:
	var result: Dictionary
	if continue_game:
		result = saves.load_game()
		if result.get("recovered", false): result = saves.repair_primary_from_backup()
	else:
		result = saves.commit_snapshot(WorldSaveSchema.initial(), "new_game")
	if result.ok:
		ready_to_play = true
		inventory = InventoryModel.from_dict(state.inventory)
	return result
func snapshot() -> Dictionary:
	var candidate: Dictionary = state.snapshot()
	if is_instance_valid(player):
		candidate.character.hp = player.current_health
		candidate.character.mp = player.mp
	candidate.inventory = inventory.to_dict()
	return candidate
func commit(candidate: Dictionary, reason: String) -> Dictionary:
	var result: Dictionary = saves.commit_snapshot(candidate, reason)
	if result.ok:
		inventory = InventoryModel.from_dict(state.inventory)
		if is_instance_valid(player): apply_player(false)
		changed.emit()
	else: notice.emit("Não foi possível salvar: " + result.code + ". Tente novamente.")
	return result
func apply_player(restore_resources: bool = true) -> void:
	player.stats.level = int(state.character.level)
	player.weapon_power = inventory.weapon()
	player.armor_power = inventory.armor()
	player.apply_stats()
	if restore_resources:
		player.current_health = int(state.character.hp)
		player.mp = float(state.character.mp)
	player.hp_changed.emit(player.current_health, player.max_health)
	player.mp_changed.emit(player.mp, player.mp_max)
func travel(destination: String) -> Dictionary:
	if destination not in ["citadel", "forest_01"]: return {"ok": false, "code": "INVALID_ZONE"}
	for id in pending_kills.keys(): grant_kill(id,pending_kills[id])
	if not pending_kills.is_empty(): return {"ok": false, "code": "REWARD_SAVE_PENDING"}
	var candidate := snapshot()
	if destination == "citadel":
		HubStateService.apply_return(candidate)
		var stats := CharacterStats.new()
		stats.level = candidate.character.level
		candidate.character.hp = stats.hp_max()
		candidate.character.mp = float(stats.mp_max())
		candidate.progress.checkpoint = "citadel"
	elif zone == "citadel":
		candidate.encounters.expedition += 1
		candidate.encounters.kills.clear()
	var result := commit(candidate, "travel")
	if result.ok: zone = destination
	return result
func investigate(id: String) -> Dictionary:
	if id not in quest.definition.clues: return {"ok": false, "code": "INVALID_CLUE"}
	var candidate := snapshot()
	candidate.world_flags[id] = true
	return commit(candidate, "investigate")
func choose(option: String) -> Dictionary:
	var result := quest.choose("act1.spy_hunt", option, snapshot(), commit)
	if result.ok and not result.get("replayed", false):
		get_node("/root/EventBus").quest_choice_applied.emit("act1.spy_hunt", option)
	return result
func grant_kill(id: String, spec: EnemyDefinition) -> void:
	pending_kills[id] = spec
	var result := LootService.grant(id, spec, snapshot(), commit)
	if result.ok:
		pending_kills.erase(id)
		if not result.get("replayed", false):
			notice.emit("Recompensa guardada • +%d XP" % (spec.base_xp * (8 if spec.category == &"sub_boss" else 1)))
func use_item(id: StringName) -> Dictionary:
	var candidate := snapshot()
	var inv := InventoryModel.from_dict(candidate.inventory)
	var previous := consumables.next_use
	var previous_anti := consumables.next_antidote
	var result := consumables.use_item(inv, id, player.current_health, player.max_health, player.mp, player.mp_max, simulation_time, player.damage_over_time)
	if not result.ok:
		notice.emit("Item indisponível, recurso cheio ou recarga em andamento.")
		return result
	candidate.inventory = inv.to_dict()
	candidate.character.hp = result.hp
	candidate.character.mp = result.mp
	var saved := commit(candidate, "consume")
	if saved.ok:
		apply_player()
		player.damage_over_time = result.poisoned
	else:
		consumables.next_use = previous
		consumables.next_antidote = previous_anti
	return saved
func equip(id: String) -> Dictionary:
	var inv := InventoryModel.from_dict(inventory.to_dict())
	var spec := inv.item(id)
	if spec == null or not inv.equip(String(spec.slot), id, player.stats.level): return {"ok": false}
	var candidate := snapshot()
	candidate.inventory = inv.to_dict()
	return commit(candidate, "equip")
func reclaim() -> Dictionary:
	if zone != "citadel": return {"ok":false, "code":"HUB_REQUIRED"}
	var candidate := snapshot()
	var inv := InventoryModel.from_dict(candidate.inventory)
	for id in candidate.pending_loot.keys():
		if inv.add(id, candidate.pending_loot[id]).ok: candidate.pending_loot.erase(id)
	candidate.inventory = inv.to_dict()
	return commit(candidate, "reclaim")
func discard(id: String) -> Dictionary:
	var inv := InventoryModel.from_dict(inventory.to_dict())
	var spec := inv.item(id)
	if spec == null or spec.kind == &"quest" or id in inv.equipped.values() or not inv.remove(id, 1): return {"ok": false}
	var candidate := snapshot()
	candidate.inventory = inv.to_dict()
	return commit(candidate, "discard")
func buy(item_id: String) -> Dictionary:
	var request_id := "shop." + Crypto.new().generate_random_bytes(12).hex_encode()
	return MerchantService.purchase(item_id, request_id, zone, snapshot(), commit)
func sell(item_id: String) -> Dictionary:
	var request_id := "shop." + Crypto.new().generate_random_bytes(12).hex_encode()
	return MerchantService.sell(item_id, request_id, zone, snapshot(), commit)
