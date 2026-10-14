class_name CharacterStats
extends RefCounted
var definition: ClassDefinition = preload("res://content/classes/twilight_bastion.tres")
var level := 1
func hp_max() -> int: return 60 + 8 * definition.vitality + 6 * (level - 1)
func mp_max() -> int: return 30 + 4 * definition.intelligence + 2 * definition.faith + 3 * (level - 1)
func defense(armor: int) -> int: return armor + floori(definition.vitality / 2.0) + 2 * (level - 1)
func magic_defense(ward: int) -> int: return ward + floori((definition.intelligence + definition.faith) / 3.0) + level - 1
func crit_chance_bp() -> int: return mini(2500, 500 + 25 * definition.dexterity)
