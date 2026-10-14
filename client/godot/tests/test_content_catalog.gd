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
	var catalog := ContentCatalog.new()
	check(catalog.load_directory(&"items","res://content/items",&"item_id").is_empty(),"load trusted catalog")
	check(catalog.ids(&"items").size() == 16,"all item templates")
	check(not catalog.load_directory(&"items","res://content/../core",&"item_id").is_empty(),"path traversal rejected")
	check(not catalog.load_directory(&"items","user://",&"item_id").is_empty(),"save resources forbidden")
	var item := ItemDefinition.new()
	item.item_id = &"sample"
	check(catalog.register(&"test",&"sample",item).is_empty(),"register memory resource")
	check(catalog.register(&"test",&"sample",item) == "DUPLICATE_ID","no overwrite")
	check(catalog.register(&"test",&"bad id",item) == "INVALID_ID","bad ID")
	item.slot = &"missing"
	check(catalog.validate_references(&"test",&"slot",&"items").size() == 1,"missing reference")
	check(catalog.load_directory(&"empty","res://content/quests/empty_test",&"item_id").is_empty(),"empty folder")
	finish()
