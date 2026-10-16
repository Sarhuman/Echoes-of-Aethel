param(
    [Parameter(Mandatory = $true)]
    [string]$EngineRoot,
    [int]$TimeoutSeconds = 900
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectRoot = Join-Path $repoRoot 'client\unreal'
$project = Join-Path $projectRoot 'EchoesOfAethel.uproject'
$build = Join-Path $EngineRoot 'Engine\Build\BatchFiles\Build.bat'
$editor = Join-Path $EngineRoot 'Engine\Binaries\Win64\UnrealEditor-Cmd.exe'
$logDir = Join-Path $projectRoot 'Saved\Logs'
$buildLog = Join-Path $logDir 'AethelAutomationBuild.log'
$stdoutLog = Join-Path $logDir 'AethelAutomationStdout.log'
$stderrLog = Join-Path $logDir 'AethelAutomationStderr.log'
$editorLog = Join-Path $logDir 'EchoesOfAethel.log'

foreach ($path in @($project, $build, $editor)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Arquivo necessário não encontrado: $path"
    }
}
New-Item -ItemType Directory -Path $logDir -Force | Out-Null

& $build EchoesOfAethelEditor Win64 Development "-Project=$project" -WaitMutex -MaxParallelActions=2 -NoUBA *> $buildLog
if ($LASTEXITCODE -ne 0 -or -not (Select-String -LiteralPath $buildLog -Pattern 'Result: Succeeded' -Quiet)) {
    throw "Build Unreal falhou. Veja $buildLog"
}

$args = @(
    ('"' + $project + '"'),
    '-unattended', '-nullrhi', '-nosplash', '-NoSound',
    '-ExecCmds="Automation RunTest Aethel.Domain.PlayerStats"',
    '-TestExit="Automation Test Queue Empty"'
)
$run = Start-Process -FilePath $editor -ArgumentList $args -PassThru -WindowStyle Hidden `
    -RedirectStandardOutput $stdoutLog -RedirectStandardError $stderrLog
if (-not $run.WaitForExit($TimeoutSeconds * 1000)) {
    Stop-Process -Id $run.Id -Force -ErrorAction SilentlyContinue
    throw "Automation excedeu $TimeoutSeconds segundos. Veja $editorLog"
}
$run.Refresh()
if ($run.ExitCode -ne 0) {
    throw "Editor retornou código $($run.ExitCode). Veja $editorLog"
}
if (-not (Test-Path -LiteralPath $editorLog -PathType Leaf)) {
    throw "Log de Automation não encontrado: $editorLog"
}

$result = Get-Content -LiteralPath $editorLog -Raw
$found = $result -match "Found 1 automation tests based on 'Aethel\.Domain\.PlayerStats'"
$passed = $result -match 'Test Completed\. Result=\{Success\} Name=\{PlayerStats\} Path=\{Aethel\.Domain\.PlayerStats\}'
$queueDone = $result -match 'Automation Test Queue Empty 1 tests performed\.'
if (-not ($found -and $passed -and $queueDone)) {
    throw "Teste ausente ou não aprovado. Veja $editorLog"
}

Write-Output 'Aethel.Domain.PlayerStats: 1 teste executado, Success.'
Write-Output "Build: $buildLog"
Write-Output "Automation: $editorLog"
