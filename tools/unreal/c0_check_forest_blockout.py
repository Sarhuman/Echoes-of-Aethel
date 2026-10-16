"""Reload the C0 forest blockout and verify its canonical route markers."""

import json
from pathlib import Path

import unreal


root = Path(__file__).resolve().parents[2]
manifest = json.loads(
    (root / "docs/engineering/first-route-content.json").read_text(encoding="utf-8")
)
level = "/Game/Aethel/Dev/Blockouts/L_C0_ForestRoute"
editor = unreal.get_editor_subsystem(unreal.LevelEditorSubsystem)
if not editor.load_level(level):
    raise RuntimeError(f"Could not reload {level}")
all_actors = unreal.get_editor_subsystem(unreal.EditorActorSubsystem).get_all_level_actors()
actors = {
    actor.get_actor_label(): actor
    for actor in all_actors
    if actor.get_actor_label().startswith("C0_Forest_")
}
required = {
    "C0_Forest_EntryPad_EXPERIMENTAL",
    "C0_Forest_FirstWolfPad_EXPERIMENTAL",
    "C0_Forest_ClueForkPad_EXPERIMENTAL",
    "C0_Forest_VillageApproach_EXPERIMENTAL",
    "C0_Forest_VillagePad_EXPERIMENTAL",
    "C0_Forest_ArenaApproach_EXPERIMENTAL",
    "C0_Forest_MossClawsArena_EXPERIMENTAL",
    "C0_Forest_AbandonedHouseFloor_EXPERIMENTAL",
    "C0_Forest_RootCellarFloor_EXPERIMENTAL",
    "C0_Forest_ReturnGateNorth",
    "C0_Forest_ReturnGateSouth",
    "C0_Forest_ReviewCamera_FOV35_EXPERIMENTAL",
    "C0_Forest_ReviewDirectionalLight_EXPERIMENTAL",
}
required.update(
    "C0_Forest_Clue_" + clue_id
    for clue_id in manifest["quest"]["required_clues"]
)
required.update(
    "C0_Forest_EncounterMarker_" + item["enemy_id"]
    for item in manifest["encounters"]
)
missing = required - actors.keys()
if missing:
    raise RuntimeError(f"Missing forest landmarks: {sorted(missing)}")
if len(actors) != 33:
    raise RuntimeError(f"Expected 33 C0 actors, found {len(actors)}")

# Continuous preliminary floor spine; this checks geometry only, not navmesh.
pads = [
    "EntryPad", "FirstWolfPad", "ClueForkPad", "VillageApproach",
    "VillagePad", "ArenaApproach", "MossClawsArena",
]
for left_name, right_name in zip(pads, pads[1:]):
    left = actors[f"C0_Forest_{left_name}_EXPERIMENTAL"]
    right = actors[f"C0_Forest_{right_name}_EXPERIMENTAL"]
    left_end = left.get_actor_location().x + 50 * left.get_actor_scale3d().x
    right_start = right.get_actor_location().x - 50 * right.get_actor_scale3d().x
    if left_end < right_start:
        raise RuntimeError(f"Floor gap between {left_name} and {right_name}")

house = actors["C0_Forest_AbandonedHouseFloor_EXPERIMENTAL"]
for clue_id in manifest["quest"]["required_clues"]:
    clue = actors["C0_Forest_Clue_" + clue_id]
    if abs(clue.get_actor_location().y - house.get_actor_location().y) < 500:
        raise RuntimeError(f"Clue {clue_id} too close to house interior")

print(f"C0_FOREST_BLOCKOUT_CHECK_OK actors={len(actors)} landmarks={len(required)}")

