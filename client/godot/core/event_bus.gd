extends Node

signal world_restored
signal save_completed(reason: String)
signal save_failed(code: String)
signal quest_choice_applied(quest_id: String, option_id: String)
signal checkpoint_reached(zone_id: String, map_index: int)
