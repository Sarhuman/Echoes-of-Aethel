"""Build a non-playable C0 forest route with canonical clue/encounter markers.

Geometry and dimensions are provisional editor-only blockout. No PlayerStart,
AI, quest triggers, loot, travel, damage, or checkpoint logic is added.
"""

import json
from pathlib import Path

import unreal


ROOT = Path(__file__).resolve().parents[2]
CONTENT = json.loads(
    (ROOT / "docs/engineering/first-route-content.json").read_text(encoding="utf-8")
)
LEVEL = "/Game/Aethel/Dev/Blockouts/L_C0_ForestRoute"
PREFIX = "C0_Forest_"
level_editor = unreal.get_editor_subsystem(unreal.LevelEditorSubsystem)
actors = unreal.get_editor_subsystem(unreal.EditorActorSubsystem)
cube = unreal.load_asset("/Engine/BasicShapes/Cube.Cube")
if cube is None:
    raise RuntimeError("Engine cube mesh unavailable")

if unreal.EditorAssetLibrary.does_asset_exist(LEVEL):
    if not level_editor.load_level(LEVEL):
        raise RuntimeError(f"Could not load {LEVEL}")
    for old in actors.get_all_level_actors():
        if old.get_actor_label().startswith(PREFIX):
            actors.destroy_actor(old)
elif not level_editor.new_level(LEVEL):
    raise RuntimeError(f"Could not create {LEVEL}")


def box(name, xyz, size):
    actor = actors.spawn_actor_from_class(
        unreal.StaticMeshActor, unreal.Vector(*xyz), unreal.Rotator()
    )
    if actor is None:
        raise RuntimeError(f"Could not spawn {name}")
    actor.set_actor_label(PREFIX + name)
    actor.static_mesh_component.set_static_mesh(cube)
    actor.set_actor_scale3d(unreal.Vector(*(axis / 100 for axis in size)))
    return actor


# West-to-east route: Citadel gate, first encounter, clues, village, boss arena.
# Pads overlap so the initial 3D read has a continuous, unobstructed spine.
for name, xyz, size in (
    ("EntryPad_EXPERIMENTAL", (-4000, 0, -10), (1000, 1100, 20)),
    ("FirstWolfPad_EXPERIMENTAL", (-3000, 0, -10), (1100, 700, 20)),
    ("ClueForkPad_EXPERIMENTAL", (-1850, 0, -10), (1300, 1400, 20)),
    ("VillageApproach_EXPERIMENTAL", (-650, 0, -10), (1200, 700, 20)),
    ("VillagePad_EXPERIMENTAL", (550, 0, -10), (1500, 1600, 20)),
    ("ArenaApproach_EXPERIMENTAL", (1850, 0, -10), (1300, 700, 20)),
    ("MossClawsArena_EXPERIMENTAL", (3200, 0, -10), (1800, 1800, 20)),
    ("HouseApproach_EXPERIMENTAL", (550, -950, -10), (500, 450, 20)),
    ("AbandonedHouseFloor_EXPERIMENTAL", (550, -1450, -10), (800, 650, 20)),
    ("RootCellarFloor_EXPERIMENTAL", (1500, -1500, -230), (700, 600, 20)),
):
    box(name, xyz, size)

# Gate framing and house shell leave visible openings. They are not tested for
# actual capsule clearance or camera occlusion yet.
for name, xyz, size in (
    ("ReturnGateNorth", (-4450, 320, 180), (140, 140, 360)),
    ("ReturnGateSouth", (-4450, -320, 180), (140, 140, 360)),
    ("HouseWallWest", (170, -1450, 150), (35, 620, 300)),
    ("HouseWallEast", (930, -1450, 150), (35, 620, 300)),
    ("HouseWallSouth", (550, -1760, 150), (800, 35, 300)),
    ("CellarAccessMarker_EXPERIMENTAL", (780, -1520, 50), (100, 100, 100)),
    ("WellProxy_EXPERIMENTAL", (750, 500, 60), (140, 140, 120)),
):
    box(name, xyz, size)

clue_positions = ((-2100, 380, 60), (-1550, -410, 60), (350, 600, 60))
clues = CONTENT["quest"]["required_clues"]
if len(clues) != 3 or CONTENT["quest"]["location"] != "forest_01":
    raise RuntimeError("First-route clue contract changed; review layout")
for clue_id, position in zip(clues, clue_positions):
    box(f"Clue_{clue_id}", position, (70, 70, 120))

encounter_positions = {
    "ethereal_wolf": (-2900, 100, 90),
    "blind_cultist": (1400, 220, 90),
    "moss_claws": (3200, 0, 145),
}
for encounter in CONTENT["encounters"]:
    enemy_id = encounter["enemy_id"]
    if enemy_id not in encounter_positions:
        raise RuntimeError(f"Encounter {enemy_id} has no layout marker")
    box(
        f"EncounterMarker_{enemy_id}",
        encounter_positions[enemy_id],
        (100, 100, 180) if enemy_id != "moss_claws" else (260, 260, 290),
    )

for name, xyz in (
    ("ArenaPylonNW", (2400, 800, 150)),
    ("ArenaPylonNE", (4000, 800, 150)),
    ("ArenaPylonSW", (2400, -800, 150)),
    ("ArenaPylonSE", (4000, -800, 150)),
    ("TrailTreeNorth", (-2950, 470, 180)),
    ("TrailTreeSouth", (-2200, -600, 180)),
):
    box(name, xyz, (110, 110, 300))

for name, xyz in (
    ("RouteWest", (-2650, -900, 1100)),
    ("RouteEast", (2900, 700, 1100)),
):
    light = actors.spawn_actor_from_class(
        unreal.PointLight, unreal.Vector(*xyz), unreal.Rotator()
    )
    if light is None:
        raise RuntimeError(f"Could not spawn {name} light")
    light.set_actor_label(PREFIX + "ReviewLight_" + name)
    component = light.get_components_by_class(unreal.PointLightComponent)[0]
    component.set_mobility(unreal.ComponentMobility.MOVABLE)
    component.set_intensity(50000.0)
    component.set_attenuation_radius(4500.0)

sun = actors.spawn_actor_from_class(
    unreal.DirectionalLight, unreal.Vector(0, 0, 1200), unreal.Rotator(-60, -35, 0)
)
if sun is None:
    raise RuntimeError("Could not spawn review directional light")
sun.set_actor_label(PREFIX + "ReviewDirectionalLight_EXPERIMENTAL")
sun_component = sun.get_components_by_class(unreal.DirectionalLightComponent)[0]
sun_component.set_mobility(unreal.ComponentMobility.MOVABLE)
sun_component.set_intensity(4.0)

camera = actors.spawn_actor_from_class(
    unreal.CameraActor, unreal.Vector(-1000, -6500, 6200), unreal.Rotator(-43, 70, 0)
)
if camera is None:
    raise RuntimeError("Could not spawn review camera")
camera.set_actor_label(PREFIX + "ReviewCamera_FOV35_EXPERIMENTAL")
camera.get_editor_property("camera_component").set_editor_property("field_of_view", 35.0)

if not level_editor.save_current_level():
    raise RuntimeError("Forest blockout save failed")
print(f"C0_FOREST_BLOCKOUT_CREATED {LEVEL}")

