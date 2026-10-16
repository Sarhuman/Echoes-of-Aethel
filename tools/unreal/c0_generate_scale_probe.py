"""Generate a disposable editor-only scale/camera probe; no gameplay actors."""

import unreal


LEVEL = "/Game/Aethel/Dev/Measurement/L_C0_ScaleProbe"
level_editor = unreal.get_editor_subsystem(unreal.LevelEditorSubsystem)
actors = unreal.get_editor_subsystem(unreal.EditorActorSubsystem)
cube = unreal.load_asset("/Engine/BasicShapes/Cube.Cube")
if cube is None:
    raise RuntimeError("Engine cube mesh unavailable")
if unreal.EditorAssetLibrary.does_asset_exist(LEVEL):
    if not level_editor.load_level(LEVEL):
        raise RuntimeError(f"Could not load existing probe: {LEVEL}")
    for existing in actors.get_all_level_actors():
        if existing.get_actor_label().startswith("C0_"):
            actors.destroy_actor(existing)
elif not level_editor.new_level(LEVEL):
    raise RuntimeError(f"Could not create level: {LEVEL}")


def add_box(label, location_cm, size_cm):
    actor = actors.spawn_actor_from_class(
        unreal.StaticMeshActor, unreal.Vector(*location_cm), unreal.Rotator()
    )
    if actor is None:
        raise RuntimeError(f"Could not spawn {label}")
    actor.set_actor_label(label)
    actor.static_mesh_component.set_static_mesh(cube)
    actor.set_actor_scale3d(unreal.Vector(*(value / 100.0 for value in size_cm)))
    return actor


add_box("C0_Floor_20x20m", (0, 0, -10), (2000, 2000, 20))

# Three independent passages. Widths are experimental geometry, not approved map metrics.
for index, (lane_y, width_cm) in enumerate(((-600, 200), (0, 300), (600, 400)), 1):
    half_width = width_cm / 2.0
    for side, sign in (("Left", -1), ("Right", 1)):
        add_box(
            f"C0_Lane{index}_{width_cm}cm_{side}",
            (0, lane_y + sign * (half_width + 10), 100),
            (800, 20, 200),
        )
    add_box(f"C0_Lane{index}_Probe", (-250, lane_y, 90), (60, 60, 180))

# Five class-labelled silhouette proxies. Sizes remain placeholders pending rig/capsule review.
for index, (label, width_cm, height_cm) in enumerate(
    (
        ("Bastion", 80, 190),
        ("RuneWeaver", 65, 180),
        ("ShadowSentinel", 60, 175),
        ("ThornSummoner", 70, 185),
        ("SacredFlameInquisitor", 75, 185),
    )
):
    add_box(
        f"C0_Proxy_{label}_EXPERIMENTAL",
        (-700 + index * 350, 900, height_cm / 2),
        (width_cm, width_cm, height_cm),
    )

add_box("C0_OcclusionColumn", (150, 0, 150), (100, 100, 300))

camera_location = unreal.Vector(-1100, -1100, 1500)
camera_rotation = unreal.Rotator(-42, 45, 0)
for label, projection in (
    ("C0_Camera_Perspective_FOV35_EXPERIMENTAL", "perspective"),
    ("C0_Camera_Orthographic_Comparison", "orthographic"),
):
    camera = actors.spawn_actor_from_class(
        unreal.CameraActor, camera_location, camera_rotation
    )
    if camera is None:
        raise RuntimeError(f"Could not spawn {label}")
    camera.set_actor_label(label)
    component = camera.get_editor_property("camera_component")
    if projection == "perspective":
        component.set_editor_property("field_of_view", 35.0)
    else:
        component.set_editor_property(
            "projection_mode", unreal.CameraProjectionMode.ORTHOGRAPHIC
        )
        component.set_editor_property("ortho_width", 1800.0)

if not level_editor.save_current_level():
    raise RuntimeError("Probe created but map save failed")
print(f"C0_SCALE_PROBE_CREATED {LEVEL}")
