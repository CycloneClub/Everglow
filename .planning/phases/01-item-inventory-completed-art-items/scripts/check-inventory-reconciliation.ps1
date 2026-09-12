<#
.SYNOPSIS
	Phase 1 inventory reconciliation gate (01-INVENTORY.json).

.DESCRIPTION
	Entry-level gate for the plan-02 reconciliation record. Exits non-zero when:
	  - any entry is missing internal_name, status, artwork_complete,
	    code_complete, localization, tranche, advances, or blockers;
	  - any tranche is outside {A, B, ""};
	  - any advances array is empty;
	  - any localization object lacks the blocked boolean;
	  - any status yellow entry has an empty blockers array;
	  - any entry has code_complete true and artwork_complete true yet status
	    is not green;
	  - any source_kind item entry with a non-empty internal_name has an empty
	    repo_asset while artwork_complete is true.

	This script is deliberately 100% ASCII (PowerShell 5.1 reads a BOM-less
	script as the system ANSI code page); the inventory is read through
	[IO.File]::ReadAllText so UTF-8 content is honoured.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$jsonPath = Join-Path $phaseDir '01-INVENTORY.json'

if (-not (Test-Path -LiteralPath $jsonPath)) {
	Write-Output "FAIL: inventory JSON not found: $jsonPath"
	exit 2
}

$inv = [IO.File]::ReadAllText($jsonPath) | ConvertFrom-Json
$entries = @($inv.entries)
$failures = New-Object System.Collections.Generic.List[string]

$requiredFields = @('id','internal_name','status','artwork_complete','code_complete','localization','tranche','advances','blockers','source_kind','repo_asset')
foreach ($e in $entries) {
	$id = [string]$e.id
	if ([string]::IsNullOrWhiteSpace($id)) { $id = '<missing id>' }
	$names = @($e.PSObject.Properties.Name)

	foreach ($f in $requiredFields) {
		if ($names -notcontains $f) { $failures.Add("$id : missing field '$f'") }
	}
	if ($names -notcontains 'internal_name') { continue }

	if ($e.tranche -notin @('A','B','')) { $failures.Add("$id : tranche '$($e.tranche)' not in {A,B,empty}") }

	$adv = @($e.advances)
	if ($adv.Count -eq 0) { $failures.Add("$id : advances array is empty") }

	if ($null -eq $e.localization) {
		$failures.Add("$id : localization block missing")
	}
	elseif (@($e.localization.PSObject.Properties.Name) -notcontains 'blocked') {
		$failures.Add("$id : localization.blocked boolean missing")
	}

	$blockers = @($e.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
	if ($e.status -eq 'yellow' -and $blockers.Count -eq 0) {
		$failures.Add("$id : status yellow with no blocker")
	}
	if ([bool]$e.code_complete -and [bool]$e.artwork_complete -and $e.status -ne 'green') {
		$failures.Add("$id : both complete but status '$($e.status)' is not green")
	}
	if ($e.source_kind -eq 'item' -and -not [string]::IsNullOrWhiteSpace([string]$e.internal_name)) {
		if ([string]::IsNullOrWhiteSpace([string]$e.repo_asset) -and [bool]$e.artwork_complete) {
			$failures.Add("$id : artwork-complete item with a repo class but no repo_asset")
		}
	}
}

# --- source label taxonomy gate -------------------------------------------
$labels = @($inv.labels)
$validClassifications = @('region','nested_area','structure','transition','alias')
if ($labels.Count -ne 5) {
	$failures.Add("labels : expected exactly 5 objects, found $($labels.Count)")
}
$entryById = @{}
foreach ($e in $entries) { $entryById[[string]$e.id] = $e }
foreach ($l in $labels) {
	$name = [string]$l.label_en
	if ([string]::IsNullOrWhiteSpace($name)) { $name = '<missing label_en>' }
	if ([string]::IsNullOrWhiteSpace([string]$l.label_zh)) { $failures.Add("label $name : missing label_zh") }
	if ($validClassifications -notcontains [string]$l.classification) {
		$failures.Add("label $name : classification '$($l.classification)' not in {region,nested_area,structure,transition,alias}")
	}
	if ([string]::IsNullOrWhiteSpace([string]$l.rationale)) {
		$failures.Add("label $name : rationale is empty")
	}
	if ($l.PSObject.Properties.Name -notcontains 'resolved') {
		$failures.Add("label $name : resolved boolean missing")
	}
	elseif (-not [bool]$l.resolved) {
		$hasBlocker = -not [string]::IsNullOrWhiteSpace([string]$l.blocker)
		if (-not $hasBlocker) {
			$blockedEntry = $false
			foreach ($rid in @($l.affected_entry_ids)) {
				$re = $entryById[[string]$rid]
				if ($null -ne $re -and $re.status -ne 'green' -and @($re.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }).Count -gt 0) { $blockedEntry = $true }
			}
			if (-not $blockedEntry) { $failures.Add("label $name : unresolved label has no blocking entry and no blocker") }
		}
	}
}

# --- deferred / assumptions / markdown consistency gate -------------------
foreach ($e in $entries) {
	if ([bool]$e.deferred -and [string]::IsNullOrWhiteSpace([string]$e.deferred_reason)) {
		$failures.Add("$($e.id) : deferred entry has no deferred_reason")
	}
}

$assumptions = @($inv.assumptions)
if ($assumptions.Count -eq 0) {
	$failures.Add('assumptions : array is empty')
}

$mdPath = Join-Path $phaseDir '01-INVENTORY.md'
if (-not (Test-Path -LiteralPath $mdPath)) {
	$failures.Add("markdown mirror not found: $mdPath")
}
else {
	$mdLines = [IO.File]::ReadAllLines($mdPath)
	$mdRows = @($mdLines | Where-Object { $_ -match '^\|.*\|\s*(done|no)\s*\|\s*(done|no)\s*\|\s*(green|yellow|unchecked)\s*\|' })
	if ($mdRows.Count -ne $entries.Count) {
		$failures.Add("markdown row count ($($mdRows.Count)) != JSON entries ($($entries.Count))")
	}
}

if ($failures.Count -gt 0) {
	Write-Output "FAIL($($failures.Count)):"
	$failures | Select-Object -First 40 | ForEach-Object { Write-Output "  - $_" }
	exit 1
}

$green = @($entries | Where-Object { $_.status -eq 'green' }).Count
$yellow = @($entries | Where-Object { $_.status -eq 'yellow' }).Count
$unchecked = @($entries | Where-Object { $_.status -eq 'unchecked' }).Count
$matched = @($entries | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_.internal_name) }).Count
$deferred = @($entries | Where-Object { [bool]$_.deferred }).Count
Write-Output "OK(0): $($entries.Count) entries; matched=$matched; green=$green yellow=$yellow unchecked=$unchecked; labels=$($labels.Count) deferred=$deferred assumptions=$($assumptions.Count)"
exit 0
