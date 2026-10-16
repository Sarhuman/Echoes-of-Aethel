"""Check that the non-playable Citadel blockout reloads with its landmarks."""

import unreal


level = "/Game/Aethel/Dev/Blockouts/L_C0_CitadelLayout"
editor = unreal.get_editor_subsystem(unreal.LevelEditorSubsystem)
if not editor.load_level(level):
    raise RuntimeError(f"Could not reload {level}")
all_actors = unreal.get_editor_subsystem(unreal.EditorActorSubsystem).get_all_level_actors()
labels = {
    actor.get_actor_label()
    for actor in all_actors
    if actor.get_actor_label().startswith("C0_Citadel_")
}
required = {
    "C0_Citadel_Floor_Plaza_28x22m_EXPERIMENTAL",
    "C0_Citadel_Crystal_Proxy",
    "C0_Citadel_Council_Archive",
    "C0_Citadel_Bram_Workshop",
    "C0_Citadel_Lyra_Care",
    "C0_Citadel_Sera_Commerce",
    "C0_Citadel_Maera_GatePost",
    "C0_Citadel_EastGate_Pillar_North",
    "C0_Citadel_EastGate_Pillar_South",
    "C0_Citadel_ReviewCamera_FOV35_EXPERIMENTAL",
}
required.update(
    f"C0_Citadel_NPCMarker_{name}_EXPERIMENTAL"
    for name in ("Irian", "Bram", "Lyra", "Sera", "Maera")
)
missing = required - labels
if missing:
    raise RuntimeError(f"Missing blockout landmarks: {sorted(missing)}")
if len(labels) != 21:
    raise RuntimeError(f"Expected 21 C0 actors, found {len(labels)}")
print(f"C0_CITADEL_BLOCKOUT_CHECK_OK actors={len(labels)} landmarks={len(required)}")

