<#
.SYNOPSIS
	Phase 1 carry-over coverage gate (class-or-recorded-artwork-blocker).

.DESCRIPTION
	Selects every entry in 01-INVENTORY.json with phase == 1 and carry_over == true
	(the 2026-09-12 completed-art class-less carry-over set), then fails (exit 1)
	when:
	  - the selection is empty;
	  - a selected entry has no non-empty internal_name whose class short name
	    resolves to a git-tracked .cs file under
	    Sources/Modules/Yggdrasil/KelpCurtain/Items, and carries no recorded
	    blocker matching 'texture' or 'artwork'. An entry covered only by the
	    generic 'design code not complete' / 'no repo implementation found'
	    markers fails;
	  - git reports an added or modified *.png under that items tree.

	The predicate is phase-scoped, so entries routed to phase 7 (the boss and
	special-encounter correction) do not make the gate fail, while a phase 1
	carry-over entry that is neither implemented nor artwork-blocked always does.
	On success it prints the covered-entry count and exits 0.

	This script is deliberately 100% ASCII (PowerShell 5.1 reads a BOM-less script
	as the system ANSI code page); the inventory is read through
	[IO.File]::ReadAllText so UTF-8 content is honoured.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$repoRoot = (Resolve-Path (Join-Path $phaseDir '..\..\..')).Path
$jsonPath = Join-Path $phaseDir '01-INVENTORY.json'
$itemsRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Items'

if (-not (Test-Path -LiteralPath $jsonPath)) {
	Write-Output "FAIL: inventory JSON not found: $jsonPath"
	exit 2
}

$inv = [IO.File]::ReadAllText($jsonPath) | ConvertFrom-Json
$entries = @($inv.entries)

$selected = @($entries | Where-Object {
	([int]$_.phase -eq 1) -and ([bool]$_.carry_over)
})

$failures = New-Object System.Collections.Generic.List[string]

if ($selected.Count -eq 0) {
	Write-Output 'FAIL: phase-1 carry-over selection is empty'
	exit 1
}

# Tracked class files (filename -> true). Untracked files never count.
$trackedNames = @{}
$tracked = & git -C $repoRoot ls-files -- "$itemsRel/*.cs"
foreach ($p in @($tracked)) {
	if ([string]::IsNullOrWhiteSpace($p)) { continue }
	$trackedNames[[IO.Path]::GetFileName($p)] = $true
}

$covered = 0
foreach ($e in $selected) {
	$id = [string]$e.id
	$shortName = ''
	$internal = [string]$e.internal_name
	if (-not [string]::IsNullOrWhiteSpace($internal)) {
		$parts = $internal.Split('.')
		$shortName = $parts[$parts.Length - 1]
	}
	$hasClass = $false
	if (-not [string]::IsNullOrWhiteSpace($shortName)) {
		$hasClass = $trackedNames.ContainsKey($shortName + '.cs')
	}
	$artBlockers = @($e.blockers | Where-Object {
		(-not [string]::IsNullOrWhiteSpace([string]$_)) -and ([string]$_ -match 'texture|artwork')
	})
	if ($hasClass -or $artBlockers.Count -gt 0) {
		$covered++
	}
	else {
		$failures.Add("$id : no tracked class file and no recorded texture/artwork blocker")
	}
}

# No-placeholder guard: added or modified .png under the Kelp Curtain items tree.
$statusLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -- $itemsRel
foreach ($line in @($statusLines)) {
	if ([string]::IsNullOrWhiteSpace($line) -or $line.Length -lt 4) { continue }
	$status = $line.Substring(0, 2)
	$path = $line.Substring(3)
	if (($path -like '*.png') -and ($status -match '[AM?RC]')) {
		$failures.Add("placeholder guard : added/modified png '$path' (status '$status')")
	}
}

if ($failures.Count -gt 0) {
	Write-Output "FAIL($($failures.Count)):"
	$failures | ForEach-Object { Write-Output "  - $_" }
	exit 1
}

Write-Output "OK(0): carry-over covered entries = $covered (of $($selected.Count) selected)"
exit 0
