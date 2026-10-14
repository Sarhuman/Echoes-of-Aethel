extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func run() -> void:
	var initial := WorldSaveSchema.initial()
	initial.inventory.gold = 22
	var store := {"value": initial}
	var save := func(candidate: Dictionary, _reason: String) -> Dictionary:
		store.value = candidate
		return {"ok": true}
	check(not MerchantService.purchase("potion_health_small","shop.test1","forest_01",store.value,save).ok,"no purchase outside hub")
	check(store.value.inventory.gold == 22,"outside hub spends no gold")
	check(not MerchantService.purchase("unknown","shop.test1","citadel",store.value,save).ok,"unknown item refused")
	check(MerchantService.purchase("potion_health_small","shop.test1","citadel",store.value,save).ok,"potion purchased")
	check(store.value.inventory.gold == 12 and store.value.inventory.items.get("potion_health_small",0) == 4,"gold and item committed together")
	check(MerchantService.purchase("potion_health_small","shop.test1","citadel",store.value,save).get("replayed",false),"same purchase id is idempotent")
	check(store.value.inventory.gold == 12 and store.value.inventory.items.get("potion_health_small",0) == 4,"replay gives no second item")
	check(MerchantService.purchase("flask_aether_small","shop.test2","citadel",store.value,save).ok,"ether purchased")
	check(store.value.inventory.gold == 0,"both prices deducted")
	check(not MerchantService.purchase("potion_health_small","shop.test3","citadel",store.value,save).ok,"insufficient gold refused")
	check(not store.value.applied_effects.has("shop.test3"),"refused purchase leaves no receipt")
	var full := WorldSaveSchema.initial()
	full.inventory.gold = 20
	full.inventory.items = {"training_axe": 1, "worn_tunic": 9, "potion_health_small": 4}
	var inv := InventoryModel.from_dict(full.inventory)
	check(inv != null,"full fixture valid")
	if inv != null:
		check(MerchantService.purchase("potion_health_small","shop.full","citadel",full,save).code == "BAG_FULL","full bag refused")
	check(full.inventory.gold == 20,"refused purchase preserves gold")
	print("%d checks; %d failures" % [checks,failures])
	quit(1 if failures else 0)
