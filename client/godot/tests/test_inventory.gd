extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func finish() -> void:
	print("%d checks; %d failures" % [checks, failures])
	quit(1 if failures else 0)
func run() -> void:
	var inv := InventoryModel.new()
	check(inv.add(&"wolf_fang",240).ok and inv.total_weight() == 60,"weight exact boundary")
	var before := inv.to_dict()
	check(not inv.add(&"wolf_fang",1).ok and inv.to_dict() == before,"atomic overflow")
	check(inv.add(&"root_essence",1).ok and inv.total_weight() == 60,"quest bypasses capacity")
	check(not inv.remove(&"wolf_fang",241),"overspend rejected")
	check(inv.positions() == 12,"stacks of 20")
	check(InventoryModel.from_dict(inv.to_dict()).to_dict() == inv.to_dict(),"round trip")
	var bad := inv.to_dict()
	bad.items.wolf_fang = 1.5
	check(InventoryModel.from_dict(bad) == null,"fractional save quantity rejected")
	bad = inv.to_dict()
	bad.items["untrusted"] = 1
	check(InventoryModel.from_dict(bad) == null,"unknown content rejected")
	var equipment := InventoryModel.from_dict(WorldSaveSchema.initial().inventory)
	check(equipment.weapon() == 8 and equipment.armor() == 2,"starter equipment")
	check(equipment.add(&"iron_axe",1).ok and equipment.equip("main_hand","iron_axe",1) and equipment.weapon() == 9,"equip changes weapon")
	check(equipment.equip("main_hand","",1) and equipment.weapon() == 0,"unequip")
	check(not equipment.equip("chest","iron_axe",1),"wrong slot rejected")
	var consumable := ConsumableService.new()
	var count: int = equipment.counts.potion_health_small
	check(not consumable.use_item(equipment,&"potion_health_small",172,172,52,52,0).ok and equipment.counts.potion_health_small == count,"full health no consumption")
	check(consumable.use_item(equipment,&"potion_health_small",100,172,20,52,0).hp == 150,"heal 50")
	check(not consumable.use_item(equipment,&"flask_aether_small",150,172,20,52,4.99).ok,"shared cooldown")
	check(consumable.use_item(equipment,&"flask_aether_small",150,172,20,52,5).mp == 50,"5 second boundary")
	var empty := InventoryModel.new()
	var sample := ItemDefinition.new()
	sample.item_id = &"weightless_fixture"
	sample.weight = 0
	sample.max_stack = 1
	empty.catalog.register(&"items",sample.item_id,sample)
	check(empty.add(sample.item_id,60).ok and empty.positions() == 60,"60 positions exact")
	check(not empty.add(sample.item_id,1).ok,"61st position refused")
	check(empty.add(&"root_essence",2).ok,"quest bypasses position limit")
	check(not empty.add(sample.item_id,0).ok and not empty.add(sample.item_id,-1).ok,"nonpositive quantity refused")
	finish()
