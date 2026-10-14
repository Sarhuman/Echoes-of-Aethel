param([string]$Godot = "godot", [string]$Only = "")
$ErrorActionPreference = "Stop"
$repoPath = Split-Path $PSScriptRoot -Parent
$projectPath = Join-Path $repoPath "client/godot"
$engineCommand = Get-Command $Godot -ErrorAction Stop
$logPath = Join-Path $repoPath "test-results"
New-Item -ItemType Directory -Force -Path $logPath | Out-Null
function Invoke-GodotCheck([string]$Name, [string[]]$EngineArgs) {
    $info = [Diagnostics.ProcessStartInfo]::new()
    $info.FileName = $engineCommand.Source
    $info.UseShellExecute = $false
    $info.CreateNoWindow = $true
    $info.RedirectStandardOutput = $true
    $info.RedirectStandardError = $true
    foreach ($argument in $EngineArgs) { $info.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($info)
    $outTask = $process.StandardOutput.ReadToEndAsync()
    $errTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit(60000)) { $process.Kill($true); throw "Timeout: $Name" }
    $log = $outTask.Result + $errTask.Result
    [IO.File]::WriteAllText((Join-Path $logPath "$Name.log"), $log)
    $log -split "`n" | Where-Object { $_ -match 'checks|failures|SCRIPT ERROR|Parse Error|ERROR:' } | Write-Output
    if ($process.ExitCode -ne 0 -or $log -match 'SCRIPT ERROR|Parse Error') { throw "Falha em $Name (exit $($process.ExitCode)). Veja test-results." }
}
Invoke-GodotCheck "import" @("--headless", "--editor", "--path", $projectPath, "--quit")
$suites = @(Get-ChildItem (Join-Path $projectPath "tests") -Filter "test_*.gd" -Recurse | Sort-Object FullName)
if ($Only) { $suites = @($suites | Where-Object { $_.Name -match $Only }) }
if ($suites.Count -eq 0) { throw "Nenhuma suíte encontrada." }
foreach ($suite in $suites) {
    $relative = [IO.Path]::GetRelativePath($projectPath, $suite.FullName).Replace("\", "/")
    Write-Output "== $relative"
    Invoke-GodotCheck $suite.BaseName @("--headless", "--path", $projectPath, "--script", "res://$relative", "--", "--test-harness")
}
Write-Output "PASS: $($suites.Count) suites."
