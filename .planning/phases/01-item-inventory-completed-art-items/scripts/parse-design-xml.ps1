<#
.SYNOPSIS
	Deterministically parse committed Feishu DocxXML design snapshots into the
	Phase 1 machine-readable inventory (01-INVENTORY.json) plus a Markdown mirror.

.DESCRIPTION
	Header-anchored, rowspan/colspan-aware parser. Column semantics come from a
	table's header row (the <thead>, or the first <tbody> row when a header row is
	present), never from cell position. Only rgb(217,245,214) green and
	rgb(255,255,204) yellow count as completion status colours; every other
	background-color is neutral and must never be read as completion status.

	This script is deliberately 100% ASCII. PowerShell 5.1 reads a BOM-less script
	as the system ANSI code page, so literal non-ASCII bytes would be mis-decoded
	(and can even corrupt parsing); the repo also forbids UTF-8 BOMs. Chinese
	header tokens are therefore written as .NET regex \uXXXX escapes, which the
	regex engine expands at match time.

	Completion (artwork_complete / code_complete) is read from the Feishu texture
	checkbox (\u8d34\u56fe) and code checkbox (\u4ee3\u7801) `done` attributes.
	A status colour is applied ONLY when BOTH checkboxes are complete: green for a
	both-complete entry with no recorded conflict, yellow for a both-complete entry
	whose reconciliation record carries a conflict/known-exception blocker. If
	either checkbox is incomplete the status is unchecked (no colour), whatever the
	repository asset state. The parser has no reconciliation record yet, so it
	emits green for every both-complete entry and unchecked otherwise; plan-02
	reconciliation downgrades a both-complete entry that carries a blocker to
	yellow (see check-inventory-reconciliation.ps1).

	Rows from a table that has an item-name header but no resolvable texture/code
	checkbox columns are still emitted as entries, each carrying the blocker
	"checkbox columns not found (unparsed design row)". They are never silently
	skipped; the per-document parse_audit block counts them so a dropped design row
	fails the validator.

.PARAMETER InputXml
	One or more committed evidence XML snapshot paths. Defaults to the three
	Phase 1 snapshots (biology, item, terrain) under ../evidence/.

.PARAMETER SourceKind
	Parallel array of source kinds (item, biology_drop, terrain) matching InputXml
	by index.

.PARAMETER CategoryFilter
	Optional category prefixes to keep (for example weapons). Empty means keep
	every parsed entry.

.PARAMETER OutJson
	Output JSON path. Defaults to ../01-INVENTORY.json.

.PARAMETER OutMd
	Optional Markdown output path. Defaults to ../01-INVENTORY.md.

.PARAMETER SourcesJson
	Optional source metadata sidecar (token/revision_id/file/fetched_at per doc).
	Defaults to ../evidence/sources.json.
#>
[CmdletBinding()]
param(
	[string[]]$InputXml = @(),
	[string[]]$SourceKind = @(),
	[string[]]$CategoryFilter = @(),
	[string]$OutJson,
	[string]$OutMd,
	[string]$SourcesJson
)

$ErrorActionPreference = 'Stop'

$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$evidenceDir = Join-Path $phaseDir 'evidence'

if (-not $OutJson) { $OutJson = Join-Path $phaseDir '01-INVENTORY.json' }
if (-not $OutMd) { $OutMd = Join-Path $phaseDir '01-INVENTORY.md' }
if (-not $SourcesJson) { $SourcesJson = Join-Path $evidenceDir 'sources.json' }

if ($InputXml.Count -eq 0) {
	$InputXml = @(
		(Join-Path $evidenceDir 'biology.xml'),
		(Join-Path $evidenceDir 'item.xml'),
		(Join-Path $evidenceDir 'terrain.xml')
	)
	$SourceKind = @('biology_drop', 'item', 'terrain')
}
if ($SourceKind.Count -ne $InputXml.Count) {
	throw 'InputXml and SourceKind must be parallel arrays of equal length.'
}

# Header-token regexes. \uXXXX escapes keep this file ASCII (see .DESCRIPTION).
# RE_NAME         : item name / armour / drop / name column headers
# RE_TEXTURE      : texture / grain / material column header
# RE_CODE         : code column header
# RE_HEADER_HINT  : tokens that make a first tbody row look like a header row
# RE_ARMOR        : armour / set-effect header tokens
# RE_WEAPON       : weapon stat header tokens (damage/knockback/crit/use time)
$RE_NAME = '\u7269\u54c1\u540d|\u62a4\u5177|\u6389\u843d\u7269|\u540d\u5b57'
$RE_TEXTURE = '\u8d34\u56fe|\u7eb9\u7406|\u6750\u8d28'
$RE_CODE = '\u4ee3\u7801'
$RE_HEADER_HINT = '\u7269\u54c1\u540d|\u8d34\u56fe|\u4ee3\u7801|\u62a4\u5177|\u540d\u79f0|\u7c7b\u578b|\u751f\u547d|\u4f24\u5bb3'
$RE_ARMOR = '\u62a4\u5177|\u5957\u88c5\u6548\u679c'
$RE_WEAPON = '\u4f24\u5bb3|\u51fb\u9000|\u66b4\u51fb|\u4f7f\u7528\u65f6\u95f4'
$RE_SUMMON = '\u53ec\u5524'
$RE_MAGIC = '\u9b54\u6cd5|\u8017\u9b54|\u9b54\u529b'
$RE_RANGED = '\u5f13|\u7bad|\u67aa|\u8fdc\u7a0b|\u6295\u63b7'
$RE_MELEE = '\u5251|\u8fd1\u6218|\u6325\u780d|\u957f\u77db|\u5200'

$STATUS_GREEN = 'rgb(217,245,214)'
$STATUS_YELLOW = 'rgb(255,255,204)'
$CHECKBOX_LESS_BLOCKER = 'checkbox columns not found (unparsed design row)'

function Get-Attr {
	param([System.Xml.XmlNode]$Node, [string]$Name)
	if ($null -eq $Node) { return '' }
	$v = $Node.GetAttribute($Name)
	if ($null -eq $v) { return '' }
	return $v
}

function Get-TrimText {
	param([System.Xml.XmlNode]$Node)
	if ($null -eq $Node) { return '' }
	$t = $Node.InnerText
	if ($null -eq $t) { return '' }
	$t = $t.Replace("`r", ' ').Replace("`n", ' ')
	return $t.Trim()
}

function Test-IsNameHeader {
	param([string]$Header)
	return ($Header -match $RE_NAME)
}

function Test-IsTextureHeader {
	param([string]$Header)
	return ($Header -match $RE_TEXTURE)
}

function Test-IsCodeHeader {
	param([string]$Header)
	return ($Header -match $RE_CODE)
}

function Test-LooksLikeHeaderRow {
	param([System.Xml.XmlNode]$Tr)
	$texts = @()
	foreach ($td in $Tr.SelectNodes('td')) { $texts += (Get-TrimText $td) }
	$joined = ($texts -join '|')
	return ($joined -match $RE_HEADER_HINT)
}

function Get-LogicalGrid {
	param([object[]]$TrNodes)
	$grid = @{}
	for ($r = 0; $r -lt $TrNodes.Count; $r++) {
		$c = 0
		foreach ($td in $TrNodes[$r].SelectNodes('td')) {
			while ($grid.ContainsKey("$r,$c")) { $c++ }
			$rs = 1
			$cs = 1
			$rsAttr = Get-Attr $td 'rowspan'
			if ($rsAttr -match '^\d+$') { $rs = [int]$rsAttr }
			$csAttr = Get-Attr $td 'colspan'
			if ($csAttr -match '^\d+$') { $cs = [int]$csAttr }
			for ($dr = 0; $dr -lt $rs; $dr++) {
				for ($dc = 0; $dc -lt $cs; $dc++) {
					$grid["$($r + $dr),$($c + $dc)"] = $td
				}
			}
			$c += $cs
		}
	}
	return $grid
}

function Find-ColIndex {
	param([string[]]$Names, [scriptblock]$Predicate)
	for ($i = 0; $i -lt $Names.Count; $i++) {
		if (& $Predicate $Names[$i]) { return $i }
	}
	return -1
}

function Split-NameText {
	param([string]$Text)
	$t = ''
	if ($null -ne $Text) { $t = $Text.Trim() }
	if ($t -eq '') { return @{ zh = ''; en = '' } }
	$m = [regex]::Match($t, '^(?<zh>[\u4e00-\u9fff\u3400-\u4dbf\u3000-\u303f\uff00-\uffef]+)\s*(?<en>.*)$')
	if ($m.Success -and $m.Groups['zh'].Value -ne '') {
		return @{ zh = $m.Groups['zh'].Value.Trim(); en = $m.Groups['en'].Value.Trim() }
	}
	if ($t -match '[\u4e00-\u9fff]') { return @{ zh = $t; en = '' } }
	return @{ zh = ''; en = $t }
}

function Get-NameParts {
	param([System.Xml.XmlNode]$Cell)
	$ps = @()
	foreach ($p in $Cell.SelectNodes('.//p')) {
		$txt = Get-TrimText $p
		if ($txt -ne '') { $ps += $txt }
	}
	if ($ps.Count -eq 0) {
		return (Split-NameText (Get-TrimText $Cell))
	}
	if ($ps.Count -eq 1) {
		return (Split-NameText $ps[0])
	}
	$zh = $ps[0]
	$en = ($ps[1..($ps.Count - 1)] -join ' ').Trim()
	$firstSplit = Split-NameText $zh
	if ($firstSplit.en -ne '') {
		$zh = $firstSplit.zh
		$en = (($firstSplit.en + ' ' + $en).Trim())
	}
	return @{ zh = $zh; en = $en }
}

function ConvertTo-Slug {
	param([string]$Text)
	if ([string]::IsNullOrWhiteSpace($Text)) { return '' }
	$s = $Text.ToLowerInvariant()
	$s = $s -replace '[^a-z0-9\u4e00-\u9fff]+', '-'
	$s = $s.Trim('-')
	return $s
}

function Get-RowColor {
	param([object[]]$Cells)
	$hasGreen = $false
	$hasYellow = $false
	foreach ($cell in $Cells) {
		if ($null -eq $cell) { continue }
		$bg = (Get-Attr $cell 'background-color').Trim()
		if ($bg -eq $STATUS_GREEN) { $hasGreen = $true }
		elseif ($bg -eq $STATUS_YELLOW) { $hasYellow = $true }
	}
	if ($hasGreen) { return 'green' }
	if ($hasYellow) { return 'yellow' }
	return 'neutral'
}

function Get-Category {
	param([string[]]$Names, [string]$RowText)
	$joined = ($Names -join '|')
	if ($joined -match $RE_ARMOR) { return 'armor' }
	if ($joined -match $RE_WEAPON) {
		$sub = 'misc'
		if ($RowText -match $RE_SUMMON) { $sub = 'summon' }
		elseif ($RowText -match $RE_MAGIC) { $sub = 'magic' }
		elseif ($RowText -match $RE_RANGED) { $sub = 'ranged' }
		elseif ($RowText -match $RE_MELEE) { $sub = 'melee' }
		return "weapons.$sub"
	}
	return 'items'
}

function Get-Status {
	param([bool]$Artwork, [bool]$Code)
	# Colour requires BOTH checkboxes complete. A both-complete entry with a
	# conflict/known-exception blocker is downgraded to yellow by reconciliation,
	# which owns the blocker record; the parser emits green here.
	if ($Artwork -and $Code) { return 'green' }
	return 'unchecked'
}

# --- load source metadata -------------------------------------------------
$source = [ordered]@{}
if (Test-Path -LiteralPath $SourcesJson) {
	$meta = Get-Content -LiteralPath $SourcesJson -Raw | ConvertFrom-Json
	foreach ($p in $meta.PSObject.Properties) {
		$source[$p.Name] = [ordered]@{
			token       = $p.Value.token
			revision_id = $p.Value.revision_id
			file        = $p.Value.file
			fetched_at  = $p.Value.fetched_at
		}
	}
}

# --- parse ----------------------------------------------------------------
$allEntries = New-Object System.Collections.Generic.List[object]
$audit = [ordered]@{}
$usedIds = @{}
$filterActive = ($CategoryFilter.Count -gt 0)

for ($i = 0; $i -lt $InputXml.Count; $i++) {
	$xmlPath = $InputXml[$i]
	$kind = $SourceKind[$i]
	$docName = [IO.Path]::GetFileNameWithoutExtension($xmlPath)

	$raw = [IO.File]::ReadAllText($xmlPath)
	$x = New-Object System.Xml.XmlDocument
	$x.LoadXml("<root>$raw</root>")
	# Evidence snapshots are committed as a single <fragment>...</fragment> root so
	# they are well-formed XML; descend through it when present. A bare fragment
	# (no wrapper) is handled too.
	$container = $x.DocumentElement
	if ($container.ChildNodes.Count -eq 1 -and $container.FirstChild.NodeType -eq 'Element' -and $container.FirstChild.Name -eq 'fragment') {
		$container = $container.FirstChild
	}

	$tablesTotal = 0
	$tablesWithNameHeader = 0
	$rowsEmitted = 0
	$rowsWithoutCheckboxes = 0

	$headings = @{ 1 = ''; 2 = ''; 3 = ''; 4 = ''; 5 = ''; 6 = '' }

	foreach ($node in $container.ChildNodes) {
		if ($node.NodeType -ne 'Element') { continue }

		if ($node.Name -match '^h([1-6])$') {
			$lvl = [int]$Matches[1]
			$headings[$lvl] = (Get-TrimText $node)
			for ($d = $lvl + 1; $d -le 6; $d++) { $headings[$d] = '' }
			continue
		}

		if ($node.Name -ne 'table') { continue }
		$tablesTotal++

		$tableId = Get-Attr $node 'id'
		$trNodes = @($node.SelectNodes('tbody/tr'))
		if ($trNodes.Count -eq 0) { continue }

		$thead = $node.SelectSingleNode('thead')
		$headerNames = @()
		$dataStart = 0
		if ($thead) {
			foreach ($th in $thead.SelectNodes('.//th')) { $headerNames += (Get-TrimText $th) }
		}
		elseif (Test-LooksLikeHeaderRow $trNodes[0]) {
			foreach ($td in $trNodes[0].SelectNodes('td')) { $headerNames += (Get-TrimText $td) }
			$dataStart = 1
		}

		$nameCol = Find-ColIndex $headerNames { param($h) Test-IsNameHeader $h }
		if ($nameCol -lt 0) {
			# Not an item/armour/drop table (for example a creature stat or terrain
			# tile table). Iterated for audit only; never emitted as an entry.
			continue
		}
		$tablesWithNameHeader++

		$textureCol = Find-ColIndex $headerNames { param($h) Test-IsTextureHeader $h }
		$codeCol = Find-ColIndex $headerNames { param($h) Test-IsCodeHeader $h }
		$hasCheckboxes = ($textureCol -ge 0) -and ($codeCol -ge 0)

		$grid = Get-LogicalGrid $trNodes

		for ($r = $dataStart; $r -lt $trNodes.Count; $r++) {
			$nameCell = $grid["$r,$nameCol"]
			if ($null -eq $nameCell) { continue }
			$rowCells = @()
			foreach ($key in $grid.Keys) {
				if ($key -match "^$r,") { $rowCells += $grid[$key] }
			}
			$rowText = (($rowCells | ForEach-Object { Get-TrimText $_ }) -join ' | ')
			$nameParts = Get-NameParts $nameCell
			if ($nameParts.zh -eq '' -and $nameParts.en -eq '') { continue }

			$category = Get-Category $headerNames $rowText
			if ($filterActive) {
				$keep = $false
				foreach ($prefix in $CategoryFilter) {
					if ($category -like "$prefix*") { $keep = $true; break }
				}
				if (-not $keep) { continue }
			}

			$artwork = $false
			$code = $false
			$textureId = ''
			$codeId = ''
			$blockers = @()
			if ($hasCheckboxes) {
				$texCell = $grid["$r,$textureCol"]
				$codeCell = $grid["$r,$codeCol"]
				$texCb = $null
				$codeCb = $null
				if ($texCell) { $texCb = $texCell.SelectSingleNode('.//checkbox') }
				if ($codeCell) { $codeCb = $codeCell.SelectSingleNode('.//checkbox') }
				if ($texCb) {
					$textureId = Get-Attr $texCb 'id'
					$artwork = ((Get-Attr $texCb 'done').ToLowerInvariant() -eq 'true')
				}
				if ($codeCb) {
					$codeId = Get-Attr $codeCb 'id'
					$code = ((Get-Attr $codeCb 'done').ToLowerInvariant() -eq 'true')
				}
				if (-not $artwork) { $blockers += 'design artwork not complete (Feishu texture checkbox false)' }
				if (-not $code) { $blockers += 'design code not complete (Feishu code checkbox false)' }
			}
			else {
				$blockers += $CHECKBOX_LESS_BLOCKER
				$rowsWithoutCheckboxes++
			}

			$nameP = $nameCell.SelectSingleNode('.//p')
			$rowNameBlockId = ''
			if ($nameP) { $rowNameBlockId = Get-Attr $nameP 'id' }

			$slugSource = $nameParts.en
			if ([string]::IsNullOrWhiteSpace($slugSource)) { $slugSource = $nameParts.zh }
			$slug = ConvertTo-Slug $slugSource
			if ($slug -eq '') { $slug = "row-$r" }

			$region = ''
			if ($kind -ne 'item') { $region = $headings[1] }

			$idBase = "$kind-$category-$slug"
			$entryId = $idBase
			$n = 2
			while ($usedIds.ContainsKey($entryId)) { $entryId = "$idBase-$n"; $n++ }
			$usedIds[$entryId] = $true

			$status = Get-Status $artwork $code
			$entry = [ordered]@{
				id                 = $entryId
				source_kind        = $kind
				category           = $category
				tranche            = ''
				region             = $region
				name_en            = $nameParts.en
				name_zh            = $nameParts.zh
				internal_name      = ''
				feishu             = [ordered]@{
					doc                 = $docName
					table_block_id      = $tableId
					row_name_block_id   = $rowNameBlockId
					texture_checkbox_id = $textureId
					code_checkbox_id    = $codeId
					row_color           = (Get-RowColor $rowCells)
				}
				artwork_complete   = $artwork
				code_complete      = $code
				status             = $status
				repo_asset         = ''
				localization       = [ordered]@{ en_us = $false; zh_hans = $false; blocked = $false }
				dependencies       = @()
				advances           = @()
				blockers           = @($blockers)
				phase              = 1
				notes              = ''
				deferred           = $false
				deferred_reason    = ''
			}
			$allEntries.Add($entry)
			$rowsEmitted++
		}
	}

	$audit[$docName] = [ordered]@{
		tables_total                     = $tablesTotal
		tables_with_name_header          = $tablesWithNameHeader
		rows_emitted                     = $rowsEmitted
		rows_recorded_without_checkboxes = $rowsWithoutCheckboxes
	}
}

# Any source doc listed in the metadata but not parsed this run still needs a
# complete parse_audit block so downstream gates can reason about it.
foreach ($srcName in $source.Keys) {
	if (-not $audit.Contains($srcName)) {
		$audit[$srcName] = [ordered]@{
			tables_total                     = 0
			tables_with_name_header          = 0
			rows_emitted                     = 0
			rows_recorded_without_checkboxes = 0
		}
	}
}

$generatedAt = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
$root = [ordered]@{
	schema_version = 1
	generated_at   = $generatedAt
	source         = $source
	parse_audit    = $audit
	labels         = @()
	entries        = $allEntries
	assumptions    = @()
}

$json = ($root | ConvertTo-Json -Depth 12) -replace "`r`n", "`n"
$enc = New-Object System.Text.UTF8Encoding($false)
[IO.File]::WriteAllText($OutJson, ($json + "`n"), $enc)
Write-Output "parse-design-xml: wrote $($allEntries.Count) entries to $OutJson"

# --- Markdown mirror ------------------------------------------------------
if ($OutMd) {
	$byTop = [ordered]@{}
	foreach ($e in $allEntries) {
		$top = ($e.category -split '\.')[0]
		if (-not $byTop.Contains($top)) { $byTop[$top] = New-Object System.Collections.Generic.List[object] }
		$byTop[$top].Add($e)
	}

	$md = New-Object System.Collections.Generic.List[string]
	$md.Add('# Phase 1 Inventory of Items and Biology-Design Drops')
	$md.Add('')
	$md.Add("Generated: $generatedAt  ")
	$md.Add("Source metadata: $([IO.Path]::GetFileName($SourcesJson))  ")
	$md.Add('')
	$md.Add('> Machine source of truth: `01-INVENTORY.json`. This file mirrors that row set.')
	$md.Add('')

	function Format-Cell([string]$Text) {
		$t = ''
		if ($null -ne $Text) { $t = $Text }
		$t = $t.Replace('|', '\|').Replace("`r", ' ').Replace("`n", ' ')
		return $t.Trim()
	}

	foreach ($top in $byTop.Keys) {
		$title = (Get-Culture).TextInfo.ToTitleCase($top)
		$md.Add("## $title")
		$md.Add('')
		$md.Add('| Name (en) | Name (zh) | Category | Region | Artwork | Code | Status | Blockers |')
		$md.Add('| --- | --- | --- | --- | --- | --- | --- | --- |')
		$rows = $byTop[$top] | Sort-Object region, name_en, name_zh
		foreach ($e in $rows) {
			$art = if ($e.artwork_complete) { 'done' } else { 'no' }
			$cod = if ($e.code_complete) { 'done' } else { 'no' }
			$blk = (($e.blockers) -join '; ')
			$md.Add("| $(Format-Cell $e.name_en) | $(Format-Cell $e.name_zh) | $(Format-Cell $e.category) | $(Format-Cell $e.region) | $art | $cod | $($e.status) | $(Format-Cell $blk) |")
		}
		$md.Add('')
	}

	[IO.File]::WriteAllText($OutMd, (($md -join "`n") + "`n"), $enc)
	Write-Output "parse-design-xml: wrote Markdown mirror to $OutMd"
}
