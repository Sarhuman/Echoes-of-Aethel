param([string]$Godot = "godot")
$ErrorActionPreference = "Stop"
$repoPath = Split-Path $PSScriptRoot -Parent
New-Item -ItemType Directory -Force -Path (Join-Path $repoPath "dist") | Out-Null
& $Godot --headless --path (Join-Path $repoPath "client/godot") --export-release "Windows Desktop"
if ($LASTEXITCODE -ne 0) { throw "Export Windows falhou." }
Write-Output (Join-Path $repoPath "dist/EchoesOfAethel.exe")
