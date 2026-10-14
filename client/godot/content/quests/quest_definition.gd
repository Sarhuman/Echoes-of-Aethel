class_name QuestDefinition
extends Resource
@export var quest_id := "act1.spy_hunt"
@export var clues: Array[String] = ["forest_01.clue.tracks", "forest_01.clue.testimony", "forest_01.clue.well"]
@export var options: Array[String] = ["judge_suspects", "shelter_villagers"]
@export var mercy_potions := 3
@export var judgment_gold := 25
