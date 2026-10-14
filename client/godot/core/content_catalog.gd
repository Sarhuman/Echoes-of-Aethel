class_name ContentCatalog
extends RefCounted
var entries: Dictionary = {}

func register(kind: StringName, id: StringName, resource: Resource) -> String:
	if resource == null or not WorldSaveSchema.valid_id(String(id)):
		return "INVALID_ID"
	if not entries.has(kind):
		entries[kind] = {}
	if entries[kind].has(id):
		return "DUPLICATE_ID"
	entries[kind][id] = resource
	return ""

func load_directory(kind: StringName, dir_path: String, id_property: StringName) -> Array[String]:
	var errors: Array[String] = []
	var clean := dir_path.simplify_path()
	if not clean.begins_with("res://content/") or clean.contains(".."):
		return ["INVALID_PATH"]
	var dir := DirAccess.open(clean)
	if dir == null:
		return ["MISSING_DIRECTORY"]
	var names := dir.get_files()
	names.sort()
	for name in names:
		if name.ends_with(".tres") or name.ends_with(".tres.remap"):
			var filename := name.trim_suffix(".remap")
			var resource := load(clean.path_join(filename)) as Resource
			if resource == null:
				errors.append("INVALID_RESOURCE:" + filename)
				continue
			var problem := register(kind, StringName(str(resource.get(id_property))), resource)
			if not problem.is_empty():
				errors.append(problem + ":" + filename)
	return errors

func get_by_id(kind: StringName, id: StringName) -> Resource:
	return entries.get(kind, {}).get(id)

func ids(kind: StringName) -> Array[StringName]:
	var result: Array[StringName] = []
	for id in entries.get(kind, {}):
		result.append(id)
	result.sort()
	return result

func validate_references(kind: StringName, ref_property: StringName, target_kind: StringName) -> Array[String]:
	var errors: Array[String] = []
	for id in ids(kind):
		var value: Variant = get_by_id(kind, id).get(ref_property)
		var refs: Array = value if value is Array else [value]
		for reference in refs:
			if get_by_id(target_kind, StringName(str(reference))) == null:
				errors.append("%s:%s:%s:%s" % [kind, id, ref_property, reference])
	return errors
