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
var durable: Dictionary
func save_candidate(candidate: Dictionary, _reason: String) -> Dictionary:
	var problem := WorldSaveSchema.validate(candidate)
	if not problem.is_empty(): return {"ok": false, "code": problem}
	durable = candidate.duplicate(true)
	return {"ok": true}
func fail_save(_candidate: Dictionary, _reason: String) -> Dictionary:
	return {"ok": false, "code": "DISK_FAILED"}
func run() -> void:
	var state := WorldSaveSchema.initial()
	durable = state.duplicate(true)
	var boss: EnemyDefinition = load("res://content/enemies/moss_claws.tres")
	check(not LootService.grant("forest_01.moss_claws.01:1",boss,state,fail_save).ok and durable == state,"failed reward leaves memory unchanged")
	check(LootService.grant("forest_01.moss_claws.01:1",boss,state,save_candidate).ok,"boss reward commit")
	check(durable.world_flags.get("forest_01.moss_claws.defeated",false) and durable.granted_rewards.has(LootService.BOSS_REWARD),"boss flag and reward together")
	check(durable.character.level == 2 and durable.character.hp == 172,"level grows without healing")
	var before := durable.duplicate(true)
	check(LootService.grant("forest_01.moss_claws.01:2",boss,durable,save_candidate).get("replayed",false) and durable == before,"boss unique across expeditions")
	var wolf: EnemyDefinition = load("res://content/enemies/ethereal_wolf.tres")
	check(LootService.grant("forest_01.ethereal_wolf.01:1",wolf,durable,save_candidate).ok,"common reward")
	before = durable.duplicate(true)
	check(LootService.grant("forest_01.ethereal_wolf.01:1",wolf,durable,save_candidate).get("replayed",false) and durable == before,"replay common")
	check(LootService.grant("forest_01.ethereal_wolf.01:2",wolf,durable,save_candidate).ok and durable.character.xp == before.character.xp + 10,"new expedition rewards")
	var full := WorldSaveSchema.initial()
	full.inventory = InventoryModel.new().to_dict()
	full.inventory.items = {"wolf_fang":240}
	check(LootService.grant("forest_01.moss_claws.01:1",boss,full,save_candidate).ok and not durable.pending_loot.is_empty(),"full bag stores overflow")
	check(durable.inventory.items.has("root_essence"),"quest drop bypasses full bag")
	var quest := QuestService.new()
	state = WorldSaveSchema.initial()
	check(not quest.choose("act1.spy_hunt","shelter_villagers",state,save_candidate).ok,"investigation required")
	for clue in quest.definition.clues: state.world_flags[clue] = true
	check(not quest.choose("act1.spy_hunt","invalid",state,save_candidate).ok,"invalid choice")
	check(not quest.choose("act1.spy_hunt","shelter_villagers",state,fail_save).ok and state.quest_choices.is_empty(),"failed choice atomic")
	check(quest.choose("act1.spy_hunt","shelter_villagers",state,save_candidate).ok,"mercy confirmed")
	check(durable.progress.ritual_charge == 1 and durable.pending_effects.has("first_return_to_citadel"),"choice and delayed effect persisted")
	check(not DialogueResolver.resolve("npc.blacksmith",durable.world_flags).text.contains("aprendiz"),"no future consequence spoiler")
	HubStateService.apply_return(durable)
	check(durable.world_flags.get("hub.contamination_observed",false) and durable.progress.hub_stage == 0,"local consequence does not advance act stage")
	before = durable.duplicate(true)
	HubStateService.apply_return(durable)
	check(durable == before,"return idempotent")
	check(quest.choose("act1.spy_hunt","shelter_villagers",durable,save_candidate).get("replayed",false),"choice replay")
	check(not quest.choose("act1.spy_hunt","judge_suspects",durable,save_candidate).ok,"opposite choice rejected")
	check(DialogueResolver.resolve("npc.blacksmith",durable.world_flags).text.contains("aprendiz"),"observed consequence dialogue")
	state = WorldSaveSchema.initial()
	for clue in quest.definition.clues: state.world_flags[clue] = true
	check(quest.choose("act1.spy_hunt","judge_suspects",state,save_candidate).ok,"judgment confirmed")
	HubStateService.apply_return(durable)
	check(durable.world_flags.get("hub.memorial_observed",false) and not durable.world_flags.get("hub.contamination_observed",false),"routes exclusive")
	check(not DialogueResolver.available(["unknown"],{}),"requirements fail closed")
	var table: LootTable = load("res://content/loot/common.tres")
	var rng := RandomNumberGenerator.new()
	rng.seed = 419
	var equipment_count := 0
	var magic_count := 0
	for i in range(10000):
		var drops := LootService.roll(table,rng,&"ethereal_wolf")
		for id in drops.items:
			if id in ["iron_axe","padded_vest","runed_axe","ironweave_cuirass"]:
				equipment_count += 1
			if id in ["runed_axe","ironweave_cuirass"]: magic_count += 1
	check(absf(equipment_count/10000.0-0.12) < 0.02,"equipment rate fixed-seed tolerance 2pp")
	check(absf(float(magic_count)/equipment_count-0.25) < 0.04,"conditional magic rate tolerance 4pp")
	finish()
