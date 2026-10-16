#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
task_build_dir=$(mktemp -d)
trap 'rm -rf "$task_build_dir"' EXIT
mcs -langversion:7.2 -warnaserror -r:System.Runtime.Serialization -out:"$task_build_dir/CampaignTests.exe" shared/csharp/Aethel.WorldMemory/*.cs shared/csharp/Aethel.Campaign/*.cs shared/csharp/Tests/CampaignTests.cs shared/csharp/Tests/RuntimeTests.cs
mono "$task_build_dir/CampaignTests.exe"
