extends Node

signal world_restored
signal save_completed(reason: String)
signal save_failed(code: String)
signal quest_choice_applied(quest_id: String, option_id: String)
signal checkpoint_reached(zone_id: String, map_index: int)

signal actor_damaged(actor_id: String, hp_damage: int, attacker_id: String)
signal actor_died(actor_id: String)
signal crime_committed(event: Dictionary)
