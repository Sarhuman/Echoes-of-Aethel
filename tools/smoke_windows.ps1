param([string]$Executable = "")
$ErrorActionPreference = "Stop"
$repoPath = Split-Path $PSScriptRoot -Parent
if (-not $Executable) { $Executable = Join-Path $repoPath "dist/EchoesOfAethel.exe" }
$info = [Diagnostics.ProcessStartInfo]::new()
$info.FileName = $Executable
$info.WorkingDirectory = Split-Path $Executable -Parent
$info.UseShellExecute = $false
$info.CreateNoWindow = $true
$info.RedirectStandardOutput = $true
$info.RedirectStandardError = $true
foreach ($argument in @("--headless", "--quit-after", "120")) { $info.ArgumentList.Add($argument) }
$process = [Diagnostics.Process]::Start($info)
$outTask = $process.StandardOutput.ReadToEndAsync()
$errTask = $process.StandardError.ReadToEndAsync()
if (-not $process.WaitForExit(60000)) { $process.Kill($true); throw "Timeout no executável exportado." }
$log = $outTask.Result + $errTask.Result
New-Item -ItemType Directory -Force -Path (Join-Path $repoPath "test-results") | Out-Null
[IO.File]::WriteAllText((Join-Path $repoPath "test-results/export-smoke.log"), $log)
Write-Output $log
Write-Output "Exit code: $($process.ExitCode)"
if ($process.ExitCode -ne 0 -or $log -match 'SCRIPT ERROR|Parse Error|Failed to load script|Failed to load resource') { throw "Falha na inicialização da build Windows." }
Write-Output "PASS: executável Windows inicializou por 120 frames."
