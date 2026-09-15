<#
.SYNOPSIS
	Phase 3 biology matrix coverage / classification / design-art / class-resolution gate.

.DESCRIPTION
	Phase 3 gate. Exits non-zero when:
	  - the machine input (03-BIOLOGY.json) is missing (exit 2);
	  - the matrix does not have exactly 31 rows, or its `counts` block does not
	    match the actual row counts, or `phase3_tranche` is not exactly 5 ids;
	  - `phase3_tranche` differs from the set of `phase == 3` ids;
	  - the `texture_complete == true` id set differs from the frozen six-id set,
	    or `design_art == true` does not hold for exactly the nine frozen
	    design-image headings (and for no other row);
	  - a row lacks a non-empty id / region / name_zh / name_en or a valid `status`;
	  - a `phase == 3` row has no repository texture, an unresolvable
	    `repo_asset`, no blocker, or breaks the class-resolution biconditional
	    (`code_complete == true` <=> a non-empty `internal_name` whose
	    `<short name>.cs` exists on disk);
	  - any row has a non-empty `internal_name` that does not resolve to a
	    `<short name>.cs` file on disk under the NPCs tree;
	  - `deferred` / `deferred_reason` disagree, or a `phase == 3` row is deferred;
	  - a guarded Phase 3 class declares `SpawnChance` without both
	    `SubworldSystem.IsActive<YggdrasilWorld>` and the server-safe
	    `KelpCurtainBiome.IsKelpCurtainLayer`, contains `Main.LocalPlayer`,
	    emits dust/gore without `Main.dedServ`, or clones the vanilla tortoise AI
	    without `defDamage` / `defDefense` and an `NPC.ai[]` state wrapper;
	  - a guarded Phase 3 class references a `ModContent.ItemType<X>()` whose
	    `<X>.cs` does not exist under the KelpCurtain Items tree;
	  - git reports an added or modified `*.png` under the NPCs tree;
	  - -RequireAll is set and a `phase == 3` row is not `code_complete`;
	  - 03-BIOLOGY.md exists and its row-id set or per-row `status` disagrees
	    with the JSON;
	  - any file in the phase's own change set starts with the UTF-8 BOM bytes
	    `EF BB BF` (the AGENTS.md byte-level check, phase-scoped).

	Class files are resolved from the working tree (Get-ChildItem), never from
	`git ls-files`: this gate runs before its task's commit, so a class created
	in the same task is still untracked and a tracked-only lookup would fail on
	a file that exists on disk.

	The machine input is 03-BIOLOGY.json and its absence is exit 2. The human
	mirror 03-BIOLOGY.md is created by a later task, so a missing mirror is
	reported as pending rather than failing (mirror parity then runs green the
	moment the file exists).

	This script is deliberately 100% ASCII (PowerShell 5.1 reads a BOM-less
	script as the system ANSI code page); the JSON and Markdown inputs are read
	through [IO.File]::ReadAllText so their UTF-8 content is honoured.
#>
[CmdletBinding()]
param([switch]$RequireAll)

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$repoRoot = (Resolve-Path (Join-Path $phaseDir '..\..\..')).Path
$jsonPath = Join-Path $phaseDir '03-BIOLOGY.json'
$mdPath = Join-Path $phaseDir '03-BIOLOGY.md'
$phaseRel = '.planning/phases/03-completed-art-ordinary-monsters'
$npcRel = 'Sources/Modules/Yggdrasil/KelpCurtain/NPCs'
$enemiesRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies'
$itemsRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Items'

if (-not (Test-Path -LiteralPath $jsonPath -PathType Leaf)) {
	Write-Output "FAIL: biology matrix not found: $jsonPath"
	exit 2
}

$json = [IO.File]::ReadAllText($jsonPath) | ConvertFrom-Json
$rows = @($json.rows)
$failures = New-Object System.Collections.Generic.List[string]

$frozenTranche = @(
	'bio-death-jade-lake-river-slug',
	'bio-death-jade-lake-verdant-rods',
	'bio-spiny-moss-court-giant-tree-man',
	'bio-valley-of-lush-and-moist-mossy-thorn-turtle',
	'bio-valley-of-lush-and-moist-guppy-conch'
)
$frozenTextureComplete = @($frozenTranche + 'bio-out-of-phase-kelp-snake')
$frozenDesignArtHeadings = @(
	'Gca8ddqq9on2lhxraxRcBNkmn6d',
	'doxcnnaHMkf1Q1grLuoVOb1vOrh',
	'AnOud27JsouukSxerW4cQ3zWnwb',
	'Z94rdc53uoe4Wyx7TRPck6g5nQb',
	'BJVjdHX2FoW4MLxOk6ocVw82nUd',
	'K8PmdSvk3okdbbxo7vYc4flKnKd',
	'AKGxdQG2foyNqBxGXjOcomLVnBb',
	'UkGgdgBa9oZQ3UxAKS9czGRFnmc',
	'CYrgdMl7co9q1Cxl9ADcBBuXnPf'
)
$preExistingAllowlist = @('RiverSlug.cs')

# --- 2. shape and frozen counts ----------------------------------------------
if ($rows.Count -ne 31) {
	$failures.Add("shape : expected exactly 31 rows, found $($rows.Count)")
}
if (@($json.phase3_tranche).Count -ne 5) {
	$failures.Add("shape : phase3_tranche must hold exactly 5 ids, found $(@($json.phase3_tranche).Count)")
}

$actualCounts = [ordered]@{
	rows = $rows.Count
	phase3 = @($rows | Where-Object { [int]$_.phase -eq 3 }).Count
	phase4 = @($rows | Where-Object { [int]$_.phase -eq 4 }).Count
	phase7 = @($rows | Where-Object { [int]$_.phase -eq 7 }).Count
	deferred = @($rows | Where-Object { [bool]$_.deferred }).Count
	texture_complete_true = @($rows | Where-Object { [bool]$_.texture_complete }).Count
	design_art_true = @($rows | Where-Object { [bool]$_.design_art }).Count
}
foreach ($key in $actualCounts.Keys) {
	$declared = $json.counts.PSObject.Properties[$key]
	if ($null -eq $declared) {
		$failures.Add("counts : declared counts block has no '$key'")
		continue
	}
	if ([int]$declared.Value -ne [int]$actualCounts[$key]) {
		$failures.Add("counts : declared $key=$($declared.Value) but actual=$($actualCounts[$key])")
	}
}

# --- 3. tranche / texture / design-art sets ----------------------------------
$p3Ids = @($rows | Where-Object { [int]$_.phase -eq 3 } | ForEach-Object { [string]$_.id })
foreach ($d in @(Compare-Object -ReferenceObject $frozenTranche -DifferenceObject $p3Ids)) {
	$failures.Add("tranche : phase3_tranche/frozen mismatch $($d.SideIndicator) $($d.InputObject)")
}
foreach ($d in @(Compare-Object -ReferenceObject @($json.phase3_tranche) -DifferenceObject $p3Ids)) {
	$failures.Add("tranche : declared phase3_tranche differs from phase==3 ids $($d.SideIndicator) $($d.InputObject)")
}
$tcIds = @($rows | Where-Object { [bool]$_.texture_complete } | ForEach-Object { [string]$_.id })
foreach ($d in @(Compare-Object -ReferenceObject $frozenTextureComplete -DifferenceObject $tcIds)) {
	$failures.Add("texture_complete : frozen six-id set mismatch $($d.SideIndicator) $($d.InputObject)")
}
foreach ($r in $rows) {
	$expectArt = @($frozenDesignArtHeadings -contains [string]$r.feishu.heading_block_id)
	if ([bool]$r.design_art -ne $expectArt) {
		if ($expectArt) { $failures.Add("design_art : $($r.id) has an inline design image heading but design_art is false") }
		else { $failures.Add("design_art : $($r.id) has no inline design image heading but design_art is true") }
	}
}

# --- working-tree class index -------------------------------------------------
$npcRoot = Join-Path $repoRoot $npcRel
$npcFiles = @{}
if (-not (Test-Path -LiteralPath $npcRoot)) {
	$failures.Add("npc tree not found: $npcRoot")
}
else {
	foreach ($f in @(Get-ChildItem -LiteralPath $npcRoot -Recurse -Filter '*.cs' -File)) {
		if (-not $npcFiles.ContainsKey($f.Name)) { $npcFiles[$f.Name] = $f.FullName }
	}
}

# --- 4/5/6. per-row invariants ------------------------------------------------
$validStatus = @('unchecked', 'green', 'yellow')
foreach ($r in $rows) {
	$id = [string]$r.id
	if ([string]::IsNullOrWhiteSpace($id)) { $failures.Add("row : a row has an empty id"); continue }
	foreach ($field in @('region', 'name_zh', 'name_en')) {
		if ([string]::IsNullOrWhiteSpace([string]$r.$field)) { $failures.Add("$id : empty $field") }
	}
	if ($validStatus -notcontains [string]$r.status) {
		$failures.Add("$id : status '$($r.status)' is not unchecked|green|yellow")
	}
	if ([bool]$r.deferred) {
		if ([string]::IsNullOrWhiteSpace([string]$r.deferred_reason)) { $failures.Add("$id : deferred row needs a non-empty deferred_reason") }
	}
	else {
		if (-not [string]::IsNullOrWhiteSpace([string]$r.deferred_reason)) { $failures.Add("$id : non-deferred row must have an empty deferred_reason") }
	}
	$internal = [string]$r.internal_name
	if (-not [string]::IsNullOrWhiteSpace($internal)) {
		$short = $internal.Split('.')[-1]
		if (-not $npcFiles.ContainsKey($short + '.cs')) {
			$failures.Add("$id : internal_name '$internal' does not resolve to '$short.cs' on disk under $npcRel")
		}
	}
}

foreach ($r in $rows) {
	if ([int]$r.phase -ne 3) { continue }
	$id = [string]$r.id
	if (-not [bool]$r.texture_complete) { $failures.Add("$id : phase 3 row must be texture_complete") }
	$repoAsset = [string]$r.repo_asset
	if ([string]::IsNullOrWhiteSpace($repoAsset)) {
		$failures.Add("$id : phase 3 row needs a non-empty repo_asset")
	}
	elseif (-not $repoAsset.StartsWith($npcRel)) {
		$failures.Add("$id : repo_asset '$repoAsset' is not under $npcRel")
	}
	elseif (-not (Test-Path -LiteralPath (Join-Path $repoRoot $repoAsset) -PathType Leaf)) {
		$failures.Add("$id : repo_asset '$repoAsset' does not exist on disk")
	}
	$blockers = @($r.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) })
	if ($blockers.Count -eq 0) { $failures.Add("$id : phase 3 row needs at least one non-empty blocker") }
	if ([bool]$r.code_complete) {
		$internal = [string]$r.internal_name
		if ([string]::IsNullOrWhiteSpace($internal)) {
			$failures.Add("$id : code_complete true requires a non-empty internal_name")
		}
		else {
			$short = $internal.Split('.')[-1]
			if (-not $npcFiles.ContainsKey($short + '.cs')) {
				$failures.Add("$id : code_complete true requires '$short.cs' on disk under $npcRel")
			}
		}
	}
	else {
		if (-not [string]::IsNullOrWhiteSpace([string]$r.internal_name)) {
			$failures.Add("$id : code_complete false requires an empty internal_name")
		}
	}
	if ([bool]$r.deferred) { $failures.Add("$id : a phase 3 row must never be deferred") }
}

# --- 7/8. structural guards on the implemented phase-3 classes ----------------
$guardRows = @($rows | Where-Object { [int]$_.phase -eq 3 -and [bool]$_.code_complete } |
	Where-Object { $preExistingAllowlist -notcontains (([string]$_.internal_name).Split('.')[-1] + '.cs') })

$itemRoot = Join-Path $repoRoot $itemsRel
$itemFiles = @{}
if (-not (Test-Path -LiteralPath $itemRoot)) {
	$failures.Add("items tree not found: $itemRoot")
}
else {
	foreach ($f in @(Get-ChildItem -LiteralPath $itemRoot -Recurse -Filter '*.cs' -File)) { $itemFiles[$f.Name] = $true }
}

foreach ($r in $guardRows) {
	$id = [string]$r.id
	$short = ([string]$r.internal_name).Split('.')[-1]
	if (-not $npcFiles.ContainsKey($short + '.cs')) { continue }
	$text = [IO.File]::ReadAllText($npcFiles[$short + '.cs'])
	if ($text -match 'SpawnChance') {
		if ($text -notmatch 'SubworldSystem\.IsActive<YggdrasilWorld>') {
			$failures.Add("$id : SpawnChance must reference SubworldSystem.IsActive<YggdrasilWorld> (BIO-06 isolation)")
		}
		if ($text -notmatch 'KelpCurtainBiome\.IsKelpCurtainLayer') {
			$failures.Add("$id : SpawnChance must use the server-safe KelpCurtainBiome.IsKelpCurtainLayer (not the client-camera predicate)")
		}
	}
	if ($text -match 'Main\.LocalPlayer') {
		$failures.Add("$id : must not use Main.LocalPlayer (multiplayer; SpawnChance runs on the server)")
	}
	if ($text -match 'Dust\.NewDust|Main\.dust|VFXManager|Gore\.NewGore') {
		if ($text -notmatch 'Main\.dedServ') {
			$failures.Add("$id : emits dust/gore/VFX without a Main.dedServ guard (D-35)")
		}
	}
	if ($text -match 'CloneDefaults\(NPCID\.GiantTortoise\)') {
		if ($text -notmatch 'defDamage') { $failures.Add("$id : GiantTortoise clone must pin defDamage") }
		if ($text -notmatch 'defDefense') { $failures.Add("$id : GiantTortoise clone must pin defDefense") }
		if ($text -notmatch 'NPC\.ai\[') { $failures.Add("$id : GiantTortoise clone must wrap the spin state over NPC.ai[]") }
	}
	foreach ($m in [regex]::Matches($text, 'ModContent\.ItemType<([A-Za-z0-9_]+)>\(\)')) {
		$t = $m.Groups[1].Value
		if (-not $itemFiles.ContainsKey($t + '.cs')) {
			$failures.Add("$id : ModContent.ItemType<$t>() has no '$t.cs' under $itemsRel")
		}
	}
}

# --- 9. no-placeholder-art guard ---------------------------------------------
$statusLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -uall -- $npcRel
foreach ($line in @($statusLines)) {
	if ([string]::IsNullOrWhiteSpace($line) -or $line.Length -lt 4) { continue }
	$status = $line.Substring(0, 2)
	$path = $line.Substring(3)
	if (($path -like '*.png') -and ($status -match '[AM?RC]')) {
		$failures.Add("placeholder guard : added/modified png '$path' (status '$status')")
	}
}

# --- 10. optional require-all -------------------------------------------------
if ($RequireAll) {
	foreach ($r in @($rows | Where-Object { [int]$_.phase -eq 3 })) {
		if (-not [bool]$r.code_complete) { $failures.Add("require-all : phase 3 row '$($r.id)' is not code_complete") }
	}
}

# --- 11. markdown parity ------------------------------------------------------
$mdPending = $false
if (-not (Test-Path -LiteralPath $mdPath -PathType Leaf)) {
	$mdPending = $true
}
else {
	$md = [IO.File]::ReadAllText($mdPath)
	$jsonStatus = @{}
	foreach ($r in $rows) { $jsonStatus[[string]$r.id] = [string]$r.status }
	$mdIds = New-Object System.Collections.Generic.List[string]
	$mdStatus = @{}
	foreach ($line in ($md -split "`n")) {
		$t = $line.Trim()
		if (-not $t.StartsWith('|')) { continue }
		$cells = @($t.Trim('|') -split '\|' | ForEach-Object { $_.Trim() })
		if ($cells.Count -lt 11) { continue }
		$cellId = $cells[0]
		if ($cellId -notmatch '^bio-') { continue }
		if ($mdIds -contains $cellId) { $failures.Add("markdown : duplicate row id '$cellId'") }
		$mdIds.Add($cellId)
		$mdStatus[$cellId] = $cells[10]
	}
	foreach ($d in @(Compare-Object -ReferenceObject @($jsonStatus.Keys) -DifferenceObject @($mdIds))) {
		$failures.Add("markdown : row-id set mismatch $($d.SideIndicator) $($d.InputObject)")
	}
	foreach ($mdId in $mdIds) {
		if (-not $jsonStatus.ContainsKey($mdId)) { continue }
		if ($mdStatus[$mdId] -ne $jsonStatus[$mdId]) {
			$failures.Add("markdown : '$mdId' status '$($mdStatus[$mdId])' does not match the JSON '$($jsonStatus[$mdId])'")
		}
	}
}

# --- 13. byte-level UTF-8 BOM guard over the phase change set -----------------
$bomPaths = New-Object System.Collections.Generic.List[string]
$changeLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -uall -- $phaseRel $npcRel $enemiesRel
foreach ($line in @($changeLines)) {
	if ([string]::IsNullOrWhiteSpace($line) -or $line.Length -lt 4) { continue }
	$rest = $line.Substring(3)
	if ($rest -match ' -> ') { $rest = @($rest -split ' -> ')[-1] }
	$rest = $rest.Trim().Trim('"')
	if ($rest.Length -gt 0) { [void]$bomPaths.Add($rest) }
}
foreach ($known in @('03-BIOLOGY.json', '03-BIOLOGY.md', '03-DEVIATIONS.md', '03-UAT.md', 'scripts/check-biology.ps1')) {
	$rel = "$phaseRel/$known"
	if (Test-Path -LiteralPath (Join-Path $repoRoot $rel) -PathType Leaf) { [void]$bomPaths.Add($rel) }
}
$checkedBom = 0
foreach ($rel in @($bomPaths | Select-Object -Unique)) {
	$full = Join-Path $repoRoot $rel
	if (-not (Test-Path -LiteralPath $full -PathType Leaf)) { continue }
	$checkedBom++
	$bytes = [IO.File]::ReadAllBytes($full)
	if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
		$failures.Add("UTF-8 BOM: $rel")
	}
}

# --- result -------------------------------------------------------------------
if ($failures.Count -gt 0) {
	Write-Output "FAIL($($failures.Count)):"
	$failures | Select-Object -First 60 | ForEach-Object { Write-Output "  - $_" }
	exit 1
}

$implemented = @($rows | Where-Object { [int]$_.phase -eq 3 -and [bool]$_.code_complete }).Count
Write-Output "OK(0): phase3 tranche = 5 / 5 (rows=31)"
Write-Output "OK: implemented classes = $implemented / 5"
if ($mdPending) { Write-Output "PENDING: markdown mirror parity not checked (03-BIOLOGY.md does not exist yet)" }
Write-Output "OK: UTF-8 BOM check passed ($checkedBom files)."
exit 0
