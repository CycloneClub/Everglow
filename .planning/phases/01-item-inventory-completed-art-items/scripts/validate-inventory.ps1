<#
.SYNOPSIS
	Offline gate for the Phase 1 item inventory (01-INVENTORY.json).

.DESCRIPTION
	Exit codes:
	  0  valid
	  2  no weapons rows were parsed into entries[]
	  3  an entry is missing a required field
	  4  completeness/audit failure

	Required per entry: non-empty `id`, boolean `artwork_complete`, boolean
	`code_complete`, non-empty `status` in {green, yellow, unchecked}. A non-empty
	`feishu.texture_checkbox_id` is required unless the entry carries at least one
	non-empty blocker. Real design rows exist whose table has resolvable texture and
	code columns but whose cells have never been filled in; those rows must be kept
	(not dropped) and are represented by a blocker instead of a checkbox id.

	Completeness gate (exit 4) fails when:
	  - the parse_audit block is missing,
	  - a source document with a name-header table emitted zero entries,
	  - parse_audit.biology.tables_with_name_header is 0, or
	  - parse_audit.biology.rows_recorded_without_checkboxes exceeds the number of
	    biology_drop entries that carry the "checkbox columns not found" blocker.

	This script is deliberately 100% ASCII: PowerShell 5.1 reads a BOM-less script
	as the system ANSI code page (see parse-design-xml.ps1), and the inventory JSON
	is read through [IO.File]::ReadAllText, which honours UTF-8.

.PARAMETER JsonPath
	Inventory JSON path. Defaults to ../01-INVENTORY.json.
#>
[CmdletBinding()]
param(
	[string]$JsonPath
)

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
if (-not $JsonPath) { $JsonPath = Join-Path $phaseDir '01-INVENTORY.json' }

if (-not (Test-Path -LiteralPath $JsonPath)) {
	Write-Error "inventory JSON not found: $JsonPath"
	exit 3
}

$inv = [IO.File]::ReadAllText($JsonPath) | ConvertFrom-Json
$entries = @($inv.entries)
$bad = New-Object System.Collections.Generic.List[string]

$weapons = @($entries | Where-Object { $_.category -like 'weapons*' })
if ($weapons.Count -eq 0) {
	Write-Output 'FAIL(2): no entries with category starting with weapons'
	exit 2
}

foreach ($e in $entries) {
	$id = [string]$e.id
	if ([string]::IsNullOrWhiteSpace($id)) { $bad.Add('<missing id>'); continue }
	if ($e.artwork_complete -isnot [bool]) { $bad.Add("$id : artwork_complete not boolean") }
	if ($e.code_complete -isnot [bool]) { $bad.Add("$id : code_complete not boolean") }
	if ($e.status -notin @('green', 'yellow', 'unchecked')) { $bad.Add("$id : status '$($e.status)' invalid") }
	$texId = [string]$e.feishu.texture_checkbox_id
	$blockers = @($e.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
	if ([string]::IsNullOrWhiteSpace($texId) -and $blockers.Count -eq 0) {
		$bad.Add("$id : feishu.texture_checkbox_id empty and no blocker recorded")
	}
}

if ($bad.Count -gt 0) {
	Write-Output "FAIL(3): $($bad.Count) invalid entry field(s):"
	$bad | Select-Object -First 20 | ForEach-Object { Write-Output "  - $_" }
	exit 3
}

# --- completeness gate (exit 4) -------------------------------------------
$auditIssues = New-Object System.Collections.Generic.List[string]
if ($null -eq $inv.parse_audit) {
	$auditIssues.Add('parse_audit block missing')
}
else {
	if ($null -ne $inv.source) {
		foreach ($p in $inv.source.PSObject.Properties) {
			$doc = $p.Name
			$ad = $inv.parse_audit.$doc
			if ($null -eq $ad) { $auditIssues.Add("parse_audit.$doc missing"); continue }
			if ([int]$ad.tables_with_name_header -gt 0 -and [int]$ad.rows_emitted -eq 0) {
				$auditIssues.Add("parse_audit.$doc : tables_with_name_header=$($ad.tables_with_name_header) but rows_emitted=0")
			}
		}
	}

	$bio = $inv.parse_audit.biology
	if ($null -eq $bio) {
		$auditIssues.Add('parse_audit.biology missing')
	}
	else {
		if ([int]$bio.tables_with_name_header -eq 0) {
			$auditIssues.Add('parse_audit.biology.tables_with_name_header == 0')
		}
		$blockedBio = @($entries | Where-Object {
			$_.source_kind -eq 'biology_drop' -and
			(@($_.blockers) | Where-Object { $_ -like '*checkbox columns not found*' }).Count -gt 0
		}).Count
		if ([int]$bio.rows_recorded_without_checkboxes -gt $blockedBio) {
			$auditIssues.Add("parse_audit.biology.rows_recorded_without_checkboxes=$($bio.rows_recorded_without_checkboxes) exceeds blocked biology_drop entries=$blockedBio")
		}
	}
}

if ($auditIssues.Count -gt 0) {
	Write-Output "FAIL(4): $($auditIssues.Count) completeness issue(s):"
	$auditIssues | ForEach-Object { Write-Output "  - $_" }
	exit 4
}

$green = @($entries | Where-Object { $_.status -eq 'green' }).Count
$yellow = @($entries | Where-Object { $_.status -eq 'yellow' }).Count
$unchecked = @($entries | Where-Object { $_.status -eq 'unchecked' }).Count
Write-Output "OK(0): $($entries.Count) entries ($($weapons.Count) weapons) - green=$green yellow=$yellow unchecked=$unchecked"
exit 0
