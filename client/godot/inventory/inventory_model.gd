class_name InventoryModel
extends RefCounted
var catalog: ContentCatalog
var counts: Dictionary = {}
var gold := 0
var equipped := {"main_hand": "", "chest": ""}
var config: ClassDefinition = preload("res://content/classes/twilight_bastion.tres")
func _init(source: ContentCatalog = null) -> void:
	catalog = source if source != null else ContentCatalog.new()
	if source == null: catalog.load_directory(&"items", "res://content/items", &"item_id")
func item(id: StringName) -> ItemDefinition: return catalog.get_by_id(&"items", id) as ItemDefinition
func positions() -> int:
	var total := 0
	for id in counts:
		var spec := item(id)
		if spec.kind != &"quest": total += ceili(float(counts[id]) / spec.max_stack)
	return total
func total_weight() -> float:
	var total := 0.0
	for id in counts:
		var spec := item(id)
		if spec.kind != &"quest": total += counts[id] * spec.weight
	return total
func can_add(id: StringName, amount: int) -> bool:
	var spec := item(id)
	if spec == null or amount <= 0 or amount > 1000000: return false
	if spec.kind == &"quest": return true
	var old: int = counts.get(String(id), 0)
	var slots := positions() - ceili(float(old) / spec.max_stack) + ceili(float(old + amount) / spec.max_stack)
	return slots <= config.inventory_capacity and total_weight() + spec.weight * amount <= config.inventory_weight + 0.00001
func add(id: StringName, amount: int) -> Dictionary:
	if not can_add(id, amount): return {"ok": false, "added": 0, "overflow": amount}
	counts[String(id)] = counts.get(String(id), 0) + amount
	return {"ok": true, "added": amount, "overflow": 0}
func remove(id: StringName, amount: int) -> bool:
	if amount <= 0 or counts.get(String(id), 0) < amount: return false
	counts[String(id)] -= amount
	if counts[String(id)] == 0:
		counts.erase(String(id))
		for slot in equipped:
			if equipped[slot] == String(id): equipped[slot] = ""
	return true
func equip(slot: String, id: String, level: int) -> bool:
	if not equipped.has(slot): return false
	if id.is_empty():
		equipped[slot] = ""
		return true
	var spec := item(id)
	if spec == null or spec.slot != StringName(slot) or spec.kind != &"equipment" or spec.requirement_level > level or counts.get(id, 0) < 1: return false
	equipped[slot] = id
	return true
func weapon() -> int:
	var spec := item(equipped.main_hand)
	return spec.power if spec != null else 0
func armor() -> int:
	var spec := item(equipped.chest)
	return spec.armor if spec != null else 0
func to_dict() -> Dictionary:
	return {"items": counts.duplicate(true), "gold": gold, "equipped": equipped.duplicate(true)}
static func from_dict(data: Dictionary, source: ContentCatalog = null) -> InventoryModel:
	var result := InventoryModel.new(source)
	if data.size() != 3 or not data.get("items") is Dictionary or not data.get("equipped") is Dictionary or not WorldSaveSchema.integer_between(data.get("gold"), 0, 100000000): return null
	if data.equipped.size() != 2 or not data.equipped.has("main_hand") or not data.equipped.has("chest"): return null
	result.gold = int(data.gold)
	for id in data.items:
		if not WorldSaveSchema.valid_id(id) or not WorldSaveSchema.integer_between(data.items[id], 1, 1000000): return null
		if not result.add(id, int(data.items[id])).ok: return null
	for slot in data.equipped:
		if not data.equipped[slot] is String or not result.equip(slot, data.equipped[slot], 30): return null
	return result
