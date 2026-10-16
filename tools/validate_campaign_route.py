"""Validate the design-only campaign route against its canonical documents.

Run from any directory: python tools/validate_campaign_route.py
Use --write only after reviewing a deliberate GDD/index change.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
GDD = ROOT / "docs/GDD/04_mapas_e_quests.md"
INDEX = ROOT / "docs/engineering/campaign-content-index.md"
FIRST_ROUTE = ROOT / "docs/engineering/first-route-content.json"
MANIFEST = ROOT / "docs/engineering/campaign-route-manifest.json"


def build_manifest() -> dict:
    gdd = GDD.read_text(encoding="utf-8")
    index = INDEX.read_text(encoding="utf-8")
    first_route = json.loads(FIRST_ROUTE.read_text(encoding="utf-8"))
    sections = re.findall(
        r"^### Mapa (\d{2}) — (.+?)\s*$([\s\S]*?)(?=^### Mapa |^## 4\.)",
        gdd,
        re.MULTILINE,
    )
    if len(sections) != 12:
        raise ValueError(f"GDD: expected 12 map sections, found {len(sections)}")

    index_rows = {}
    for line in index.splitlines():
        match = re.match(
            r"^\| (I{1,3}|IV)/(\d{2}) (.+?) \| `([^`]+)` \| `([^`]+)` \| (.+?), (subchefe|boss de ato|boss final) \|",
            line,
        )
        if match:
            index_rows[int(match.group(2))] = match
    if len(index_rows) != 12:
        raise ValueError(f"index: expected 12 map rows, found {len(index_rows)}")

    maps = []
    for ordinal, (number, title, body) in enumerate(sections, 1):
        if int(number) != ordinal:
            raise ValueError(f"GDD map order: expected {ordinal}, found {number}")
        zone = re.search(r"^\| Ato / ID \| (\d) / `([^`]+)` \|$", body, re.MULTILINE)
        quest = re.search(r"^\*\*Quest:\*\* (.+?) · ID `([^`]+)`", body, re.MULTILINE)
        encounter = re.search(r"^\| Sub-boss / boss \| (.+?) \|$", body, re.MULTILINE)
        options = re.findall(r"^\| ([AB]) / `([^`]+)` \|", body, re.MULTILINE)
        if not zone or not quest or not encounter or len(options) != 2:
            raise ValueError(f"GDD map {number}: missing zone, quest, encounter or two options")
        if [letter for letter, _ in options] != ["A", "B"]:
            raise ValueError(f"GDD map {number}: choices must be A then B")
        act, zone_id = int(zone.group(1)), zone.group(2)
        row = index_rows[ordinal]
        if (
            zone_id != row.group(4)
            or quest.group(2) != row.group(5)
            or row.group(6) not in encounter.group(1)
        ):
            raise ValueError(f"map {number}: GDD and campaign index disagree")
        if act != (ordinal - 1) // 3 + 1:
            raise ValueError(f"map {number}: unexpected act {act}")
        category = "act_boss" if ordinal in (3, 6, 9) else "final_boss" if ordinal == 12 else "sub_boss"
        expected_label = "subchefe" if category == "sub_boss" else "boss final" if category == "final_boss" else "boss de ato"
        if row.group(7) != expected_label:
            raise ValueError(f"map {number}: unexpected encounter category")
        maps.append({
            "order": ordinal,
            "act": act,
            "zone_id": zone_id,
            "name": title,
            "quest_id": quest.group(2),
            "choice_ids": [choice_id for _, choice_id in options],
            "final_encounter": {
                "name": row.group(6),
                "technical_id": "moss_claws" if ordinal == 1 else None,
                "category": category,
            },
            "requires_previous_zone": maps[-1]["zone_id"] if maps else None,
            "unlocks_zone": None,
        })
        if ordinal > 1:
            maps[-2]["unlocks_zone"] = zone_id

    first_quest = first_route["quest"]
    first_boss = next(
        item for item in first_route["encounters"] if item["category"] == "sub_boss"
    )
    if (
        first_quest["id"] != maps[0]["quest_id"]
        or first_quest["location"] != maps[0]["zone_id"]
        or first_quest["options"] != maps[0]["choice_ids"]
        or first_boss["enemy_id"] != maps[0]["final_encounter"]["technical_id"]
    ):
        raise ValueError("forest_01 IDs disagree with first-route-content.json")
    all_ids = [choice for item in maps for choice in item["choice_ids"]]
    if len(set(all_ids)) != 24 or len({item["zone_id"] for item in maps}) != 12:
        raise ValueError("duplicate choice or zone ID")

    return {
        "schema_version": 1,
        "status": "design_only_not_runtime",
        "source_of_truth": [
            "docs/GDD/04_mapas_e_quests.md",
            "docs/engineering/campaign-content-index.md",
            "docs/engineering/first-route-content.json",
        ],
        "progression_rule": "next_zone_requires_previous_encounter_and_recorded_choice",
        "final_choice_rule": "heart_12_choice_after_vael_victory",
        "maps": maps,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--write", action="store_true", help="regenerate the reviewed manifest")
    args = parser.parse_args()
    try:
        expected = build_manifest()
        rendered = json.dumps(expected, ensure_ascii=False, indent=2) + "\n"
        if args.write:
            MANIFEST.write_text(rendered, encoding="utf-8")
        elif not MANIFEST.exists() or MANIFEST.read_text(encoding="utf-8") != rendered:
            raise ValueError("manifest differs from sources; review changes, then run --write")
    except (ValueError, KeyError, StopIteration, OSError) as error:
        print(f"Campaign route validation failed: {error}", file=sys.stderr)
        return 1
    print("Campaign route: 12 maps, 24 choices, 8 sub-bosses, 4 bosses; sources agree.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

