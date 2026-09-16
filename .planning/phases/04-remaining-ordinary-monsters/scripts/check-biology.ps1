<#
.SYNOPSIS
	Phase 4 biology matrix reconciliation / artwork-blocker / class-guard gate.

.DESCRIPTION
	Phase 4 gate. Reads the SHARED Phase 3 matrix
	(.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json) and never
	regenerates it. It is a NEW file rather than an in-place extension of the Phase 3
	script, so the Phase 3 script stays byte-identical and the Phase 3 close-out chain
	still reproduces (OQ3/D-50).

	It exits non-zero when:
	  - the shared matrix is missing (exit 2);
	  - the frozen `counts` block disagrees with the actual row counts or with the
	    frozen values (rows 31 / phase3 5 / phase4 23 / phase7 3 / deferred 2 /
	    texture_complete_true 6 / design_art_true 9), or `phase3_tranche` does not
	    hold exactly 5 ids;
	  - the `phase == 4 and not deferred` id set is not exactly the frozen 21-id set
	    (D-44), or the `phase == 4 and deferred` id set is not exactly the two hardmode
	    rows with a non-empty `deferred_reason` (V2-HARD-01);
	  - an in-scope row lacks a non-empty id / region / name_zh / name_en, carries an
	    invalid `status`, or carries no blocker containing `artwork` (D-50);
	  - any row has a non-empty `internal_name` that does not resolve to a
	    `<short name>.cs` file on disk under the NPCs tree, or an in-scope row breaks
	    the `code_complete` / `internal_name` biconditional;
	  - a guarded class (every `*.cs` under the three owned region folders, plus every
	    `*.cs` under the enemy-projectile tree whose basename is one of the phase's
	    eleven projectiles) neither overrides `Commons.ModAsset.White_Mod` nor has a
	    `<basename>.png` beside it, uses `Main.LocalPlayer`, declares `SpawnChance`
	    without both `SubworldSystem.IsActive<YggdrasilWorld>` and the server-safe
	    `KelpCurtainBiome.IsKelpCurtainLayer`, or emits dust/gore/VFX without
	    `Main.dedServ`;
	  - a guarded class references a `ModContent.ItemType<X>()` whose `<X>.cs` does not
	    exist under the whole Sources/Modules/Yggdrasil tree;
	  - git reports an added or modified `*.png` / `*.obj` / `*.xnb` under the NPCs or
	    enemy-projectile trees (D-51);
	  - -RequireAll is set and an in-scope row is not `code_complete`;
	  - 03-BIOLOGY.md exists and its row-id set or per-row `status` disagrees with the
	    JSON;
	  - any file in the phase's own change set starts with the UTF-8 BOM bytes
	    `EF BB BF` (the AGENTS.md byte-level check, phase-scoped).

	Class files are resolved from the working tree (Get-ChildItem), never from
	`git ls-files`: this gate runs before its task's commit, so a class created in the
	same task is still untracked and a tracked-only lookup would fail on a file that
	exists on disk (Pitfall 7).

	The shared matrix and its mirror are read through [IO.File]::ReadAllText so their
	UTF-8 content is honoured; this script itself is deliberately 100% ASCII because
	PowerShell 5.1 reads a BOM-less script as the system ANSI code page.
#>
[CmdletBinding()]
param([switch]$RequireAll)

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$repoRoot = (Resolve-Path (Join-Path $phaseDir '..\..\..')).Path
$jsonPath = Join-Path $phaseDir '..\03-completed-art-ordinary-monsters\03-BIOLOGY.json'
$mdPath = Join-Path $phaseDir '..\03-completed-art-ordinary-monsters\03-BIOLOGY.md'
$phaseRel4 = '.planning/phases/04-remaining-ordinary-monsters'
$phaseRel3 = '.planning/phases/03-completed-art-ordinary-monsters'
$npcRel = 'Sources/Modules/Yggdrasil/KelpCurtain/NPCs'
$enemiesRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies'
$yggdrasilRel = 'Sources/Modules/Yggdrasil'

# --- frozen phase-4 sets -------------------------------------------------------
$frozenCounts = [ordered]@{
	rows = 31
	phase3 = 5
	phase4 = 23
	phase7 = 3
	deferred = 2
	texture_complete_true = 6
	design_art_true = 9
}
$frozenInScope = @(
	'bio-death-jade-lake-fluorescent-hydra',
	'bio-death-jade-lake-giant-tiger-shrimp',
	'bio-death-jade-lake-water-strider',
	'bio-death-jade-lake-toxic-toad',
	'bio-death-jade-lake-glow-salamander',
	'bio-death-jade-lake-armored-shrimp',
	'bio-death-jade-lake-bomb-jellyfish',
	'bio-death-jade-lake-sailfin-snakehead',
	'bio-death-jade-lake-radiolarian',
	'bio-death-jade-lake-algae-octopus',
	'bio-death-jade-lake-large-algae-octopus',
	'bio-death-jade-lake-jade-anglerfish',
	'bio-death-jade-lake-cannon-barnacle',
	'bio-spiny-moss-court-withered-soldier',
	'bio-spiny-moss-court-court-commander',
	'bio-spiny-moss-court-brodie-flydragon',
	'bio-valley-of-lush-and-moist-red-needle-caterpillar',
	'bio-valley-of-lush-and-moist-assassin-raspberry',
	'bio-valley-of-lush-and-moist-serpent-moss',
	'bio-valley-of-lush-and-moist-small-guppy-conch',
	'bio-valley-of-lush-and-moist-large-mossy-thorn-turtle'
)
$frozenDeferred = @(
	'bio-out-of-phase-withered-seed',
	'bio-out-of-phase-withered-tree-guardian'
)

# The eleven projectiles this phase builds (frozen; 26 region-folder classes + these
# 11 = the 37 guarded classes of invariant 7 once every wave has landed).
$phaseProjectiles = @(
	'ToxicToad_PoisonBubble',
	'ToxicToad_PoisonCloud',
	'Radiolarian_WaterBolt',
	'RedNeedleCaterpillar_Spike',
	'AssassinRaspberry_Spike',
	'AnimatedWitherbarkSoldier_Boulder',
	'AnimatedWitherbarkSoldier_SpellBeam',
	'AlgaeOctopus_InkCloud',
	'BombJellyfish_Explosion',
	'LargeMossyThornTurtle_Shockwave',
	'LargeMossyThornTurtle_Boulder'
)

if (-not (Test-Path -LiteralPath $jsonPath -PathType Leaf)) {
	Write-Output "FAIL: biology matrix not found: $jsonPath"
	exit 2
}

$json = [IO.File]::ReadAllText($jsonPath) | ConvertFrom-Json
$rows = @($json.rows)
$failures = New-Object System.Collections.Generic.List[string]

# --- 2. frozen counts ----------------------------------------------------------
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
	$frozen = [int]$frozenCounts[$key]
	if ([int]$actualCounts[$key] -ne $frozen) {
		$failures.Add("counts : actual $key=$($actualCounts[$key]) but the frozen value is $frozen")
	}
	$declared = $json.counts.PSObject.Properties[$key]
	if ($null -eq $declared) {
		$failures.Add("counts : declared counts block has no '$key'")
		continue
	}
	if ([int]$declared.Value -ne $frozen) {
		$failures.Add("counts : declared $key=$($declared.Value) but the frozen value is $frozen")
	}
}
if (@($json.phase3_tranche).Count -ne 5) {
	$failures.Add("counts : phase3_tranche must hold exactly 5 ids, found $(@($json.phase3_tranche).Count)")
}

# --- 3. the frozen 21-row in-scope set (D-44) ---------------------------------
$inScope = @($rows | Where-Object { [int]$_.phase -eq 4 -and -not [bool]$_.deferred })
$inScopeIds = @($inScope | ForEach-Object { [string]$_.id })
foreach ($d in @(Compare-Object -ReferenceObject $frozenInScope -DifferenceObject $inScopeIds)) {
	$failures.Add("scope : phase4 in-scope set differs from the frozen 21 ids $($d.SideIndicator) $($d.InputObject)")
}

# --- 4. the two hardmode rows stay deferred (V2-HARD-01) ----------------------
$deferredRows = @($rows | Where-Object { [int]$_.phase -eq 4 -and [bool]$_.deferred })
$deferredIds = @($deferredRows | ForEach-Object { [string]$_.id })
foreach ($d in @(Compare-Object -ReferenceObject $frozenDeferred -DifferenceObject $deferredIds)) {
	$failures.Add("deferred : phase4 deferred set differs from the frozen two ids $($d.SideIndicator) $($d.InputObject)")
}
foreach ($r in $deferredRows) {
	if ([string]::IsNullOrWhiteSpace([string]$r.deferred_reason)) {
		$failures.Add("$($r.id) : deferred row needs a non-empty deferred_reason")
	}
}

# --- 5. in-scope row shape and the artwork blocker (D-50) ---------------------
$validStatus = @('unchecked', 'green', 'yellow')
foreach ($r in $inScope) {
	$id = [string]$r.id
	if ([string]::IsNullOrWhiteSpace($id)) { $failures.Add('scope : an in-scope row has an empty id'); continue }
	foreach ($field in @('region', 'name_zh', 'name_en')) {
		if ([string]::IsNullOrWhiteSpace([string]$r.$field)) { $failures.Add("$id : empty $field") }
	}
	if ($validStatus -notcontains [string]$r.status) {
		$failures.Add("$id : status '$($r.status)' is not unchecked|green|yellow")
	}
	$blockers = @($r.blockers | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) })
	if ($blockers.Count -eq 0) {
		$failures.Add("$id : in-scope row needs at least one non-empty blocker")
	}
	elseif (@($blockers | Where-Object { ([string]$_).Contains('artwork') }).Count -eq 0) {
		$failures.Add("$id : in-scope row needs a texture/artwork blocker (D-50)")
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

# --- 6. class resolution and the in-scope biconditional -----------------------
foreach ($r in $rows) {
	$id = [string]$r.id
	$internal = [string]$r.internal_name
	if (-not [string]::IsNullOrWhiteSpace($internal)) {
		$short = $internal.Split('.')[-1]
		if (-not $npcFiles.ContainsKey($short + '.cs')) {
			$failures.Add("$id : internal_name '$internal' does not resolve to '$short.cs' under $npcRel")
		}
	}
}
foreach ($r in $inScope) {
	$id = [string]$r.id
	$internal = [string]$r.internal_name
	if ([bool]$r.code_complete) {
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
	elseif (-not [string]::IsNullOrWhiteSpace($internal)) {
		$failures.Add("$id : code_complete false requires an empty internal_name")
	}
}

# --- 7/8. the phase's guarded classes ----------------------------------------
$guardedRoots = @(
	(Join-Path $npcRel 'DeathJadeLake'),
	(Join-Path $npcRel 'SpinyMossCourt'),
	(Join-Path $npcRel 'ValleyOfLushAndMoist')
)
$guardedFiles = New-Object System.Collections.Generic.List[string]
foreach ($rel in $guardedRoots) {
	$full = Join-Path $repoRoot $rel
	# A region folder that has not been created yet is zero files, never an error.
	if (-not (Test-Path -LiteralPath $full)) { continue }
	foreach ($f in @(Get-ChildItem -LiteralPath $full -Recurse -Filter '*.cs' -File)) { [void]$guardedFiles.Add($f.FullName) }
}
$enemiesFull = Join-Path $repoRoot $enemiesRel
if (Test-Path -LiteralPath $enemiesFull) {
	# Name-scoped so the pre-existing Phase 3 enemy projectiles are never counted.
	foreach ($f in @(Get-ChildItem -LiteralPath $enemiesFull -Recurse -Filter '*.cs' -File)) {
		if ($phaseProjectiles -contains $f.BaseName) { [void]$guardedFiles.Add($f.FullName) }
	}
}

$yggdrasilRoot = Join-Path $repoRoot $yggdrasilRel
$yggdrasilFiles = @{}
if (-not (Test-Path -LiteralPath $yggdrasilRoot)) {
	$failures.Add("yggdrasil tree not found: $yggdrasilRoot")
}
else {
	foreach ($f in @(Get-ChildItem -LiteralPath $yggdrasilRoot -Recurse -Filter '*.cs' -File)) { $yggdrasilFiles[$f.Name] = $true }
}

$guardedText = @{}
foreach ($file in $guardedFiles) {
	$relPath = $file.Substring($repoRoot.Length + 1).Replace('\', '/')
	$text = [IO.File]::ReadAllText($file)
	$guardedText[$file] = $text
	$base = [IO.Path]::GetFileNameWithoutExtension($file)
	$pngBeside = Test-Path -LiteralPath (Join-Path (Split-Path -Parent $file) ($base + '.png')) -PathType Leaf
	if (-not $pngBeside -and $text -notmatch 'Commons\.ModAsset\.White_Mod') {
		$failures.Add("$relPath : no beside-.png and no Commons.ModAsset.White_Mod override (D-48, Pitfall 3)")
	}
	if ($text -match 'Dust\.NewDust|Main\.dust|VFXManager|Gore\.NewGore') {
		if ($text -notmatch 'Main\.dedServ') {
			$failures.Add("$relPath : emits dust/gore/VFX without a Main.dedServ guard (D-35)")
		}
	}
	if ($text -match 'Main\.LocalPlayer') {
		$failures.Add("$relPath : must not use Main.LocalPlayer (D-55)")
	}
	if ($text -match 'SpawnChance') {
		if ($text -notmatch 'SubworldSystem\.IsActive<YggdrasilWorld>') {
			$failures.Add("$relPath : SpawnChance must reference SubworldSystem.IsActive<YggdrasilWorld> (BIO-06 isolation)")
		}
		if ($text -notmatch 'KelpCurtainBiome\.IsKelpCurtainLayer') {
			$failures.Add("$relPath : SpawnChance must use the server-safe KelpCurtainBiome.IsKelpCurtainLayer (not the client-camera predicate)")
		}
	}
	foreach ($m in [regex]::Matches($text, 'ModContent\.ItemType<([A-Za-z0-9_]+)>\(\)')) {
		$t = $m.Groups[1].Value
		if (-not $yggdrasilFiles.ContainsKey($t + '.cs')) {
			$failures.Add("$relPath : ModContent.ItemType<$t>() has no '$t.cs' under $yggdrasilRel")
		}
	}
}

# --- 9. no-placeholder-art guard (D-51) ---------------------------------------
$statusLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -uall -- $npcRel $enemiesRel
foreach ($line in @($statusLines)) {
	if ([string]::IsNullOrWhiteSpace($line) -or $line.Length -lt 4) { continue }
	$status = $line.Substring(0, 2)
	$path = $line.Substring(3)
	if (($path -like '*.png' -or $path -like '*.obj' -or $path -like '*.xnb') -and ($status -match '[AM?RC]')) {
		$failures.Add("placeholder guard : added/modified binary asset '$path' (status '$status')")
	}
}

# --- 10. optional require-all -------------------------------------------------
if ($RequireAll) {
	foreach ($r in $inScope) {
		if (-not [bool]$r.code_complete) { $failures.Add("require-all : phase 4 row '$($r.id)' is not code_complete") }
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
	foreach ($d in @(Compare-Object -ReferenceObject @($jsonStatus.Keys) -DifferenceObject $mdIds)) {
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
$changeLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -uall -- $phaseRel4 $phaseRel3 $npcRel $enemiesRel
foreach ($line in @($changeLines)) {
	if ([string]::IsNullOrWhiteSpace($line) -or $line.Length -lt 4) { continue }
	$rest = $line.Substring(3)
	if ($rest -match ' -> ') { $rest = @($rest -split ' -> ')[-1] }
	$rest = $rest.Trim().Trim('"')
	if ($rest.Length -gt 0) { [void]$bomPaths.Add($rest) }
}
foreach ($known in @('03-BIOLOGY.json', '03-BIOLOGY.md', '04-DEVIATIONS.md', '04-UAT.md', 'scripts/check-biology.ps1')) {
	$rel = "$phaseRel4/$known"
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

$reconciled = @($inScope | Where-Object { [bool]$_.code_complete }).Count
Write-Output "OK(0): phase4 in-scope set = $($inScope.Count) rows (rows=$($rows.Count))"
Write-Output "OK: reconciled rows = $reconciled / 21"
Write-Output "OK: guarded classes = $($guardedFiles.Count)"
if ($mdPending) { Write-Output "PENDING: markdown mirror parity not checked (03-BIOLOGY.md does not exist yet)" }
Write-Output "OK: UTF-8 BOM check passed ($checkedBom files)."
exit 0
