class_name MerchantService
extends RefCounted

const PRICES := {"potion_health_small": 10, "flask_aether_small": 12, "antidote_flask": 8, "iron_axe": 35, "padded_vest": 35}
const SELL_PRICES := {"wolf_fang": 3, "cultist_ash": 4, "root_essence": 12, "iron_axe": 10, "padded_vest": 10, "runed_axe": 16, "ironweave_cuirass": 16, "bastion_axe": 30, "bastion_cuirass": 30, "twilight_axe": 55, "twilight_cuirass": 55}

static func purchase(item_id: String, request_id: String, zone: String, state: Dictionary, commit: Callable) -> Dictionary:
	if zone != "citadel": return {"ok": false, "code": "HUB_REQUIRED"}
	if not WorldSaveSchema.valid_id(request_id) or not request_id.begins_with("shop."):
		return {"ok": false, "code": "INVALID_REQUEST"}
	if not PRICES.has(item_id): return {"ok": false, "code": "INVALID_ITEM"}
	if state.get("applied_effects", {}).has(request_id): return {"ok": true, "replayed": true}
	var candidate := state.duplicate(true)
	var inv := InventoryModel.from_dict(candidate.inventory)
	if inv == null: return {"ok": false, "code": "INVALID_INVENTORY"}
	var price: int = PRICES[item_id]
	if inv.gold < price: return {"ok": false, "code": "INSUFFICIENT_GOLD"}
	if not inv.add(StringName(item_id), 1).ok: return {"ok": false, "code": "BAG_FULL"}
	inv.gold -= price
	candidate.inventory = inv.to_dict()
	candidate.applied_effects[request_id] = true
	return commit.call(candidate, "purchase")

static func sell(item_id: String, request_id: String, zone: String, state: Dictionary, commit: Callable) -> Dictionary:
	if zone != "citadel": return {"ok": false, "code": "HUB_REQUIRED"}
	if not WorldSaveSchema.valid_id(request_id) or not request_id.begins_with("shop."):
		return {"ok": false, "code": "INVALID_REQUEST"}
	if not SELL_PRICES.has(item_id): return {"ok": false, "code": "INVALID_ITEM"}
	if state.get("applied_effects", {}).has(request_id): return {"ok": true, "replayed": true}
	var candidate := state.duplicate(true)
	var inv := InventoryModel.from_dict(candidate.inventory)
	if inv == null: return {"ok": false, "code": "INVALID_INVENTORY"}
	if item_id in inv.equipped.values() or inv.counts.get(item_id, 0) < 1:
		return {"ok": false, "code": "ITEM_UNAVAILABLE"}
	if inv.gold > 100000000 - int(SELL_PRICES[item_id]): return {"ok": false, "code": "GOLD_LIMIT"}
	if not inv.remove(StringName(item_id), 1): return {"ok": false, "code": "ITEM_UNAVAILABLE"}
	inv.gold += int(SELL_PRICES[item_id])
	candidate.inventory = inv.to_dict()
	candidate.applied_effects[request_id] = true
	return commit.call(candidate, "sale")
