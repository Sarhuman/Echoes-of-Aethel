extends SceneTree
func _initialize() -> void:
	var file := FileAccess.open(ProjectSettings.globalize_path("res://../../dist/GODOT-LICENSES.txt"),FileAccess.WRITE)
	file.store_string("GODOT ENGINE 4.6\n\n"+Engine.get_license_text()+"\n\nTHIRD-PARTY COMPONENTS\n"+JSON.stringify(Engine.get_copyright_info(),"\t")+"\n\nLICENSE TEXTS\n"+JSON.stringify(Engine.get_license_info(),"\t"))
	file.close()
	quit()
