#Requires -Version 7.0
[CmdletBinding()]
param(
	[Parameter(Mandatory)][string]$TmlDirectory,
	[Parameter(Mandatory)][string]$ModFile,
	[Parameter(Mandatory)][string]$WorkshopDirectory,
	[Parameter(Mandatory)][string]$RunDirectory,
	[ValidateRange(1, 1800)][int]$TimeoutSeconds = 300,
	[switch]$AuditAssets
)
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/ModLoadResult.ps1"

$TmlDirectory = (Resolve-Path -LiteralPath $TmlDirectory).Path
$ModFile = (Resolve-Path -LiteralPath $ModFile).Path
$WorkshopDirectory = (Resolve-Path -LiteralPath $WorkshopDirectory).Path
$RunDirectory = [IO.Path]::GetFullPath($RunDirectory)
# Refuse reuse: a previous run's log must never make this run pass.
if (Test-Path -LiteralPath $RunDirectory) {
	throw "RunDirectory must be new: $RunDirectory"
}
if (-not (Test-Path -LiteralPath "$TmlDirectory/tModLoader.dll" -PathType Leaf)) {
	throw 'tModLoader.dll is missing.'
}
$dependencies = @{
	'2785100219' = 'SubworldLibrary'
	'3539368598' = 'ModLiquidLib'
}
foreach ($id in $dependencies.Keys) {
	$path = "$WorkshopDirectory/content/1281930/$id"
	if (-not (Test-Path -LiteralPath "$path/workshop.json") -or
		-not (Get-ChildItem -LiteralPath $path -Recurse -Filter "$($dependencies[$id]).tmod" -ErrorAction SilentlyContinue)) {
		throw "Workshop download missing or incomplete: $($dependencies[$id]) ($id)."
	}
}

$runtime = New-Item -ItemType Directory -Path "$RunDirectory/game"
# A private runtime also isolates logs, config and console creation from the installed game.
# Avoid a directory named 'runtime': tML's compiler excludes references under '/runtime*'.
Get-ChildItem -LiteralPath $TmlDirectory | Where-Object {
	$_.Name -in @('Content', 'Libraries') -or $_.Name -like 'tModLoader.*'
} | Copy-Item -Destination $runtime.FullName -Recurse
$mods = New-Item -ItemType Directory -Path "$RunDirectory/save/Mods"
Copy-Item -LiteralPath $ModFile -Destination "$($mods.FullName)/Everglow.tmod"
$enabledMods = @('Everglow', 'SubworldLibrary', 'ModLiquidLib')
if ($AuditAssets) {
	# Compile the CI-only helper with this run's tML, outside the repository and release artifact.
	$source = New-Item -ItemType Directory -Path "$RunDirectory/save/ModSources/CiAssetAudit"
	Copy-Item -Path "$PSScriptRoot/../asset-audit/*" -Destination $source.FullName
	Push-Location $runtime.FullName
	try {
		& dotnet tModLoader.dll -server -nosteam -tmlsavedirectory "$RunDirectory/save" -build $source.FullName 2>&1 |
			Tee-Object -FilePath "$RunDirectory/asset-audit-build.log" | Write-Host
		if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath "$($mods.FullName)/CiAssetAudit.tmod")) {
			throw 'Failed to build the CI asset audit mod.'
		}
	}
	finally { Pop-Location }
	# Build logs must not serve as load evidence.
	if (Test-Path -LiteralPath "$($runtime.FullName)/tModLoader-Logs/server.log") {
		Move-Item -LiteralPath "$($runtime.FullName)/tModLoader-Logs/server.log" -Destination "$RunDirectory/asset-audit-build-server.log"
	}
	$enabledMods += 'CiAssetAudit'
}
$enabledMods | ConvertTo-Json |
	Set-Content -LiteralPath "$($mods.FullName)/enabled.json" -Encoding utf8NoBOM
$workshop = New-Item -ItemType Directory -Path "$RunDirectory/workshop/content/1281930"
foreach ($id in $dependencies.Keys) {
	Copy-Item -LiteralPath "$WorkshopDirectory/content/1281930/$id" -Destination $workshop.FullName -Recurse
}
Get-FileHash -LiteralPath "$($mods.FullName)/Everglow.tmod" |
	Format-List | Out-String | Set-Content "$RunDirectory/mod-sha256.txt"

$start = [Diagnostics.ProcessStartInfo]::new()
# Linux may expose dotnet through both /usr/bin and /bin; use one executable.
$start.FileName = (Get-Command dotnet -CommandType Application | Select-Object -First 1).Source
$start.WorkingDirectory = $runtime.FullName
$start.UseShellExecute = $false
$start.CreateNoWindow = $true
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($argument in @(
	'tModLoader.dll', '-server', '-nosteam',
	'-tmlsavedirectory', "$RunDirectory/save",
	'-modpath', $mods.FullName,
	'-steamworkshopfolder', "$RunDirectory/workshop",
	'-testservermodloading', 'Everglow'
)) {
	$start.ArgumentList.Add($argument)
}
$process = [Diagnostics.Process]::new()
$process.StartInfo = $start
$timedOut = $false
$started = $false
try {
	$started = $process.Start()
	$stdout = $process.StandardOutput.ReadToEndAsync()
	$stderr = $process.StandardError.ReadToEndAsync()
	if (-not $process.WaitForExit($TimeoutSeconds * 1000)) {
		$timedOut = $true
		$process.Kill($true)
		$process.WaitForExit()
	}
	$exitCode = $process.ExitCode
	[IO.File]::WriteAllText("$RunDirectory/stdout.log", $stdout.GetAwaiter().GetResult())
	[IO.File]::WriteAllText("$RunDirectory/stderr.log", $stderr.GetAwaiter().GetResult())
}
finally {
	if ($started -and -not $process.HasExited) { $process.Kill($true) }
	$process.Dispose()
}
$log = ''
$serverLog = "$($runtime.FullName)/tModLoader-Logs/server.log"
if (Test-Path -LiteralPath $serverLog) {
	Copy-Item -LiteralPath $serverLog -Destination "$RunDirectory/server.log"
	$log = Get-Content -LiteralPath $serverLog -Raw
	Get-Content -LiteralPath $serverLog -Tail 60 | Write-Host
}
Assert-ModLoadResult -LogText $log -ExitCode $exitCode -TimedOut $timedOut -RequireAssetAudit:$AuditAssets
Write-Host 'PASS: Everglow and its dependencies completed actual tModLoader server loading.'
