# Run with PowerShell 7; no game installation or test framework is required.
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/ModLoadResult.ps1"
$complete = "[12:00:00] [Main Thread/INFO] [tML]: Finalizing Content: Everglow (Everglow) v0.2`n[12:00:00] [Main Thread/INFO] [tML]: Finalizing Content: SubworldLibrary (Subworld Library) v2.2`n[12:00:00] [Main Thread/INFO] [tML]: Finalizing Content: ModLiquidLib (ModLiquid Library) v1.0`n[12:00:01] [.NET TP Worker/INFO] [tML]: Mod Load Completed in 1000ms"
$cases = @(
	@{ Name = 'Complete load'; Log = $complete; Code = 0; Timeout = $false; Pass = $true },
	@{ Name = 'Nonzero exit'; Log = $complete; Code = 1; Timeout = $false; Pass = $false },
	@{ Name = 'Timeout'; Log = $complete; Code = 0; Timeout = $true; Pass = $false },
	@{ Name = 'No log'; Log = ''; Code = 0; Timeout = $false; Pass = $false },
	@{ Name = 'Only started loading'; Log = ($complete -replace '(?m)^.*Mod Load Completed.*$', ''); Code = 0; Timeout = $false; Pass = $false },
	@{ Name = 'Everglow missing'; Log = ($complete -replace '(?m)^.*Finalizing Content: Everglow .*$', ''); Code = 0; Timeout = $false; Pass = $false },
	@{ Name = 'Dependency missing'; Log = ($complete -replace '(?m)^.*Finalizing Content: ModLiquidLib .*$', ''); Code = 0; Timeout = $false; Pass = $false },
	@{ Name = 'Similar mod name'; Log = ($complete -replace 'Finalizing Content: Everglow ', 'Finalizing Content: EverglowOther '); Code = 0; Timeout = $false; Pass = $false },
	@{ Name = 'Error after completion'; Log = ($complete + "`n[12:00:02] [Main Thread/ERROR] [tML]: Failed load"); Code = 0; Timeout = $false; Pass = $false }
)
foreach ($case in $cases) {
	$passed = $true
	try { Assert-ModLoadResult -LogText $case.Log -ExitCode $case.Code -TimedOut $case.Timeout }
	catch { $passed = $false }
	if ($passed -ne $case.Pass) { throw "Unexpected result: $($case.Name)" }
	Write-Host "PASS: $($case.Name)"
}
