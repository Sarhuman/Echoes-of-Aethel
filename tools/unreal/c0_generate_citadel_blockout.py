"""Create a non-playable, experimental 3D layout of the Citadel hub.

Editor-only C0 blockout. Positions are provisional centimetres; no gameplay,
navigation, quest, shop, save, NPC, or approved final art is created here.
"""

import unreal


LEVEL = "/Game/Aethel/Dev/Blockouts/L_C0_CitadelLayout"
PREFIX = "C0_Citadel_"
level_editor = unreal.get_editor_subsystem(unreal.LevelEditorSubsystem)
actors = unreal.get_editor_subsystem(unreal.EditorActorSubsystem)
cube = unreal.load_asset("/Engine/BasicShapes/Cube.Cube")
if cube is None:
    raise RuntimeError("Engine cube mesh unavailable")

if unreal.EditorAssetLibrary.does_asset_exist(LEVEL):
    if not level_editor.load_level(LEVEL):
        raise RuntimeError(f"Could not load existing blockout: {LEVEL}")
    for old in actors.get_all_level_actors():
        if old.get_actor_label().startswith(PREFIX):
            actors.destroy_actor(old)
elif not level_editor.new_level(LEVEL):
    raise RuntimeError(f"Could not create blockout: {LEVEL}")


def box(name, xyz, size):
    actor = actors.spawn_actor_from_class(
        unreal.StaticMeshActor, unreal.Vector(*xyz), unreal.Rotator()
    )
    if actor is None:
        raise RuntimeError(f"Could not spawn {name}")
    actor.set_actor_label(PREFIX + name)
    actor.static_mesh_component.set_static_mesh(cube)
    actor.set_actor_scale3d(unreal.Vector(*(value / 100 for value in size)))
    return actor


# East is +X; the unobstructed centreline connects the square and forest gate.
box("Floor_Plaza_28x22m_EXPERIMENTAL", (0, 0, -10), (2800, 2200, 20))
box("Floor_EastGateApproach", (1750, 0, -10), (700, 600, 20))
box("Floor_BramWorkshop_Threshold", (-900, -1250, -10), (650, 300, 20))

# Low plinths indicate functions, not inaccessible walls or finished buildings.
box("Crystal_Base", (-150, 0, 35), (260, 260, 70))
box("Crystal_Proxy", (-150, 0, 165), (95, 95, 190))
box("Council_Archive", (-980, 590, 85), (480, 280, 170))
box("Bram_Workshop", (-900, -840, 100), (520, 360, 200))
box("Lyra_Care", (-250, 820, 55), (370, 230, 110))
box("Sera_Commerce", (650, 820, 55), (370, 230, 110))
box("Maera_GatePost", (1100, -490, 65), (230, 260, 130))

# The gate is framed, not filled, so its passage reads clearly from the square.
box("EastGate_Pillar_North", (1410, 350, 180), (140, 140, 360))
box("EastGate_Pillar_South", (1410, -350, 180), (140, 140, 360))
box("EastGate_Lintel", (1410, 0, 395), (140, 840, 70))

# Services remain reachable in every future narrative variant. These thin
# markers identify positions for review, not runtime NPCs or their final scale.
for name, position in (
    ("Irian", (-690, 390, 90)),
    ("Bram", (-900, -570, 90)),
    ("Lyra", (-250, 580, 90)),
    ("Sera", (650, 580, 90)),
    ("Maera", (1110, -170, 90)),
):
    box(f"NPCMarker_{name}_EXPERIMENTAL", position, (55, 55, 180))

for name, position in (
    ("SouthWest", (-1160, -930, 950)),
    ("NorthEast", (1100, 930, 950)),
):
    light = actors.spawn_actor_from_class(
        unreal.PointLight, unreal.Vector(*position), unreal.Rotator()
    )
    if light is None:
        raise RuntimeError(f"Could not spawn {name} light")
    light.set_actor_label(PREFIX + "ReviewLight_" + name)
    component = light.get_components_by_class(unreal.PointLightComponent)[0]
    component.set_intensity(50000.0)
    component.set_attenuation_radius(3500.0)

camera = actors.spawn_actor_from_class(
    unreal.CameraActor, unreal.Vector(-2700, -2700, 3000), unreal.Rotator(-42, 45, 0)
)
if camera is None:
    raise RuntimeError("Could not spawn review camera")
camera.set_actor_label(PREFIX + "ReviewCamera_FOV35_EXPERIMENTAL")
camera.get_editor_property("camera_component").set_editor_property("field_of_view", 35.0)

if not level_editor.save_current_level():
    raise RuntimeError("Blockout created but map save failed")
print(f"C0_CITADEL_BLOCKOUT_CREATED {LEVEL}")

