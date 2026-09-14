<#
.SYNOPSIS
	Phase 2 coverage / classification / status gate (01-INVENTORY.json + 02-CLASSIFICATION.json).

.DESCRIPTION
	Phase 2 gate. Exits non-zero when:
	  - either input file is missing (exit 2);
	  - the non-deferred phase==2 selection is not exactly 21 entries;
	  - 02-CLASSIFICATION.json does not have exactly 21 rows, or does not split
	    exactly 9 full / 12 shell, or its id set differs from the selection;
	  - an implemented manifest row has no non-empty internal_name whose class
	    file exists on disk, or does not carry code_complete=true,
	    artwork_complete=false, status=unchecked and a blocker matching
	    texture|artwork|system|effect|recipe;
	  - any .cs under Items/Armors/CrimsonMoonAlgae carries the autoload-equip
	    attribute, or does not register with EquipLoader.AddEquipTexture;
	  - the three formation-repair placeholder ids are not deferred with a reason;
	  - the reallocated Giant Winged Dragon mask is not phase 7 with ITEM-06;
	  - git reports an added or modified *.png under the items tree;
	  - -RequireAll is set and any manifest row is not implemented.

	Class files are resolved from the working tree (Get-ChildItem), never from
	git ls-files: this gate runs before its task's commit, so a class created in
	the same task is still untracked and a tracked-only lookup would fail on a
	file that exists on disk.

	This script is deliberately 100% ASCII (PowerShell 5.1 reads a BOM-less
	script as the system ANSI code page); the JSON inputs are read through
	[IO.File]::ReadAllText so their UTF-8 content is honoured.
#>
[CmdletBinding()]
param([switch]$RequireAll)

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$repoRoot = (Resolve-Path (Join-Path $phaseDir '..\..\..')).Path
$invPath = Join-Path $phaseDir '..\01-item-inventory-completed-art-items\01-INVENTORY.json'
$manifestPath = Join-Path $phaseDir '02-CLASSIFICATION.json'
$itemsRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Items'

if (-not (Test-Path -LiteralPath $invPath)) {
	Write-Output "FAIL: inventory JSON not found: $invPath"
	exit 2
}
if (-not (Test-Path -LiteralPath $manifestPath)) {
	Write-Output "FAIL: classification manifest not found: $manifestPath"
	exit 2
}

$inv = [IO.File]::ReadAllText($invPath) | ConvertFrom-Json
$manifest = [IO.File]::ReadAllText($manifestPath) | ConvertFrom-Json

$entries = @($inv.entries)
$rows = @($manifest.rows)
$failures = New-Object System.Collections.Generic.List[string]

$entryById = @{}
foreach ($e in $entries) { $entryById[[string]$e.id] = $e }

# --- 1. selection -----------------------------------------------------------
$selected = @($entries | Where-Object { ([int]$_.phase -eq 2) -and (-not [bool]$_.deferred) })
if ($selected.Count -ne 21) {
	$failures.Add("selection : expected exactly 21 non-deferred phase==2 entries, found $($selected.Count)")
}

# --- 2. classification manifest ---------------------------------------------
if ($rows.Count -ne 21) {
	$failures.Add("manifest : expected exactly 21 rows, found $($rows.Count)")
}
$fullCount = @($rows | Where-Object { [string]$_.mode -eq 'full' }).Count
$shellCount = @($rows | Where-Object { [string]$_.mode -eq 'shell' }).Count
if ($fullCount -ne 9) { $failures.Add("manifest : expected 9 full rows, found $fullCount") }
if ($shellCount -ne 12) { $failures.Add("manifest : expected 12 shell rows, found $shellCount") }

$selIds = @($selected | ForEach-Object { [string]$_.id })
$rowIds = @($rows | ForEach-Object { [string]$_.id })
$diff = @(Compare-Object -ReferenceObject $selIds -DifferenceObject $rowIds)
foreach ($d in $diff) {
	$failures.Add("manifest/selection mismatch : $($d.SideIndicator) $($d.InputObject)")
}

# --- working-tree class index ------------------------------------------------
$classNames = @{}
$itemRoot = Join-Path $repoRoot $itemsRel
if (-not (Test-Path -LiteralPath $itemRoot)) {
	$failures.Add("items tree not found: $itemRoot")
}
else {
	foreach ($f in @(Get-ChildItem -LiteralPath $itemRoot -Recurse -Filter '*.cs' -File)) {
		$classNames[$f.Name] = $true
	}
}

# --- 3. implemented-row invariants -------------------------------------------
$implemented = @($rows | Where-Object { [bool]$_.implemented })
foreach ($r in $implemented) {
	$id = [string]$r.id
	$e = $entryById[$id]
	if ($null -eq $e) {
		$failures.Add("$id : implemented manifest row has no matching inventory entry")
		continue
	}
	$internal = [string]$e.internal_name
	if ([string]::IsNullOrWhiteSpace($internal)) {
		$failures.Add("$id : implemented row has an empty internal_name")
	}
	else {
		$parts = $internal.Split('.')
		$shortName = $parts[$parts.Length - 1]
		if (-not $classNames.ContainsKey($shortName + '.cs')) {
			$failures.Add("$id : class '$shortName.cs' not found on disk under $itemsRel")
		}
	}
	if (-not [bool]$e.code_complete) { $failures.Add("$id : implemented row must have code_complete true") }
	if ([bool]$e.artwork_complete) { $failures.Add("$id : implemented row must have artwork_complete false") }
	if ([string]$e.status -ne 'unchecked') { $failures.Add("$id : implemented row must have status unchecked but is '$($e.status)'") }
	$blockers = @($e.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) })
	$precise = @($blockers | Where-Object { [string]$_ -match 'texture|artwork|system|effect|recipe' })
	if ($precise.Count -eq 0) {
		$failures.Add("$id : implemented row needs a blocker matching texture|artwork|system|effect|recipe")
	}
}

# --- 4. equip-registration structural guard (Pitfall 1) ----------------------
$algaeDir = Join-Path $itemRoot 'Armors\CrimsonMoonAlgae'
if (-not (Test-Path -LiteralPath $algaeDir)) {
	$failures.Add("equip guard : directory not found: $algaeDir")
}
else {
	$csFiles = @(Get-ChildItem -LiteralPath $algaeDir -Recurse -Filter '*.cs' -File)
	if ($csFiles.Count -eq 0) { $failures.Add("equip guard : no .cs files under $algaeDir") }
	foreach ($f in $csFiles) {
		$text = [IO.File]::ReadAllText($f.FullName)
		if ($text -match 'AutoloadEquip') {
			$failures.Add("equip guard : $($f.Name) must not carry the autoload-equip attribute (Pitfall 1)")
		}
		if ($text -notmatch 'EquipLoader\.AddEquipTexture') {
			$failures.Add("equip guard : $($f.Name) must register its slot with EquipLoader.AddEquipTexture")
		}
	}
}

# --- 5. deferred placeholders ------------------------------------------------
$placeholderIds = @(
	'item-weapons.misc-a',
	'item-weapons.misc-b',
	('item-weapons.misc-c-' + [char]0x540D + [char]0x5B57 + [char]0x8981 + [char]0x666E + [char]0x901A)
)
foreach ($placeholderId in $placeholderIds) {
	$e = $entryById[$placeholderId]
	if ($null -eq $e) {
		$failures.Add("$placeholderId : deferred placeholder entry missing")
		continue
	}
	if (-not [bool]$e.deferred) { $failures.Add("$placeholderId : must stay deferred") }
	if ([string]::IsNullOrWhiteSpace([string]$e.deferred_reason)) {
		$failures.Add("$placeholderId : deferred entry needs a non-empty deferred_reason")
	}
	$internal = [string]$e.internal_name
	if (-not [string]::IsNullOrWhiteSpace($internal)) {
		$parts = $internal.Split('.')
		$shortName = $parts[$parts.Length - 1]
		if ($classNames.ContainsKey($shortName + '.cs')) {
			$failures.Add("$placeholderId : deferred placeholder must not have a class file on disk")
		}
	}
}

# --- 6. Giant Winged Dragon mask reallocation (D-16) -------------------------
$maskId = 'biology_drop-weapons.misc-' + [char]0x5DE8 + [char]0x7FFC + [char]0x9F99 + [char]0x9762 + [char]0x5177
$mask = $entryById[$maskId]
if ($null -eq $mask) {
	$failures.Add("$maskId : reallocation entry missing")
}
else {
	if ([int]$mask.phase -ne 7) { $failures.Add("$maskId : expected phase 7 (D-16) but is $($mask.phase)") }
	if (@($mask.advances) -notcontains 'ITEM-06') { $failures.Add("$maskId : advances must contain ITEM-06 (D-16)") }
}

# --- 7. no-placeholder guard -------------------------------------------------
$statusLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -- $itemsRel
foreach ($line in @($statusLines)) {
	if ([string]::IsNullOrWhiteSpace($line) -or $line.Length -lt 4) { continue }
	$status = $line.Substring(0, 2)
	$path = $line.Substring(3)
	if (($path -like '*.png') -and ($status -match '[AM?RC]')) {
		$failures.Add("placeholder guard : added/modified png '$path' (status '$status')")
	}
}

# --- 8. optional require-all -------------------------------------------------
if ($RequireAll) {
	foreach ($r in $rows) {
		if (-not [bool]$r.implemented) { $failures.Add("require-all : row '$($r.id)' is not implemented") }
	}
}

# --- result ------------------------------------------------------------------
if ($failures.Count -gt 0) {
	Write-Output "FAIL($($failures.Count)):"
	$failures | Select-Object -First 60 | ForEach-Object { Write-Output "  - $_" }
	exit 1
}

$implCount = @($rows | Where-Object { [bool]$_.implemented }).Count
Write-Output "OK(0): phase2 implemented = $implCount / 21 (full=9, shell=12)"
exit 0
