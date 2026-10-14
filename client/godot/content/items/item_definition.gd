class_name ItemDefinition
extends Resource
@export var item_id: StringName
@export var display_name := ""
@export var kind: StringName = &"material"
@export var weight := 0.25
@export var max_stack := 20
@export var slot: StringName
@export var power := 0
@export var armor := 0
@export var requirement_level := 1
@export var restore := 0
@export var cooldown := 5.0
func valid() -> bool:
	return WorldSaveSchema.valid_id(String(item_id)) and kind in [&"material", &"consumable", &"equipment", &"quest"] and is_finite(weight) and weight >= 0 and max_stack >= 1
