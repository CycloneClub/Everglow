<#
.SYNOPSIS
	Fixture regression test for parse-design-xml.ps1.

.DESCRIPTION
	Builds a small synthetic DocxXML fixture in a temp file, runs the parser over
	it, and asserts the column-anchoring, merged-cell and status-colour rules.

	The fixture exercises:
	  - a <thead> whose texture (TEX) column is deliberately NOT at index 1;
	  - a data row with a rowspan cell that spans the next row;
	  - one row with texture done=true / code done=false (mixed);
	  - one row with both checkboxes false;
	  - one row with both true and a green fill;
	  - one row with a yellow fill but both checkboxes false (a fill must never be
	    read as completion status);
	  - non-status fills rgb(255,255,255) and header grey rgb(239,240,241);
	  - a second <table> with a biology-drop name cell but no texture/code columns.

	Exit 0 on success; non-zero naming the failing assertion otherwise. The temp
	fixture and outputs are always deleted.

	This script is deliberately 100% ASCII. PowerShell 5.1 reads a BOM-less script
	as the system ANSI code page; Chinese header tokens are decoded from \uXXXX
	escapes at runtime so the fixture itself still contains real Chinese.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

function Decode-Unicode {
	param([string]$Text)
	return [regex]::Replace($Text, '\\u([0-9a-fA-F]{4})', { param($m) [string][char][int]('0x' + $m.Groups[1].Value) })
}

$NAME = Decode-Unicode '\u7269\u54c1\u540d'   # item name column
$TYPE = Decode-Unicode '\u7c7b\u578b'         # type column
$TEX = Decode-Unicode '\u8d34\u56fe'          # texture checkbox column
$CODE = Decode-Unicode '\u4ee3\u7801'         # code checkbox column
$DMG = Decode-Unicode '\u4f24\u5bb3'          # damage column
$DESC = Decode-Unicode '\u63cf\u8ff0'         # description column
$PRICE = Decode-Unicode '\u4ef7\u683c'        # price column
$NOTE = Decode-Unicode '\u5907\u6ce8'         # note column

$BLOCKER = 'checkbox columns not found (unparsed design row)'

$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$parserPath = Join-Path $scriptDir 'parse-design-xml.ps1'

$tmpDir = Join-Path ([IO.Path]::GetTempPath()) ('gsd-parser-fixture-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $tmpDir | Out-Null
$fixturePath = Join-Path $tmpDir 'fixture.xml'
$outJson = Join-Path $tmpDir 'fixture.json'
$outMd = Join-Path $tmpDir 'fixture.md'
$noSources = Join-Path $tmpDir 'sources-absent.json'

$fixture = @"
<fragment mode="full">
<table id="fixture-table-1">
<thead><tr>
<th><p id="h-name">$NAME</p></th>
<th><p id="h-type">$TYPE</p></th>
<th><p id="h-tex">$TEX</p></th>
<th><p id="h-code">$CODE</p></th>
<th><p id="h-dmg">$DMG</p></th>
<th><p id="h-desc">$DESC</p></th>
</tr></thead>
<tbody>
<tr>
<td background-color="rgb(255,255,255)"><p id="r1-name">Mixed Weapon</p></td>
<td background-color="rgb(255,255,255)"><p id="r1-type">A</p></td>
<td background-color="rgb(255,255,255)"><checkbox id="tex-r1" done="true"></checkbox></td>
<td background-color="rgb(255,255,255)"><checkbox id="code-r1" done="false"></checkbox></td>
<td background-color="rgb(255,255,255)"><p id="r1-dmg">10</p></td>
<td rowspan="2" background-color="rgb(255,255,255)"><p id="r1-desc">shared</p></td>
</tr>
<tr>
<td background-color="rgb(239,240,241)"><p id="r2-name">Both False</p></td>
<td background-color="rgb(239,240,241)"><p id="r2-type">B</p></td>
<td background-color="rgb(239,240,241)"><checkbox id="tex-r2" done="false"></checkbox></td>
<td background-color="rgb(239,240,241)"><checkbox id="code-r2" done="false"></checkbox></td>
<td background-color="rgb(239,240,241)"><p id="r2-dmg">20</p></td>
</tr>
<tr>
<td background-color="rgb(217,245,214)"><p id="r3-name">All Done</p></td>
<td background-color="rgb(217,245,214)"><p id="r3-type">C</p></td>
<td background-color="rgb(217,245,214)"><checkbox id="tex-r3" done="true"></checkbox></td>
<td background-color="rgb(217,245,214)"><checkbox id="code-r3" done="true"></checkbox></td>
<td background-color="rgb(217,245,214)"><p id="r3-dmg">30</p></td>
<td background-color="rgb(217,245,214)"><p id="r3-desc">d</p></td>
</tr>
<tr>
<td background-color="rgb(255,255,204)"><p id="r4-name">Yellow Fill</p></td>
<td background-color="rgb(255,255,204)"><p id="r4-type">D</p></td>
<td background-color="rgb(255,255,204)"><checkbox id="tex-r4" done="false"></checkbox></td>
<td background-color="rgb(255,255,204)"><checkbox id="code-r4" done="false"></checkbox></td>
<td background-color="rgb(255,255,204)"><p id="r4-dmg">40</p></td>
<td background-color="rgb(255,255,204)"><p id="r4-desc">e</p></td>
</tr>
</tbody>
</table>
<table id="fixture-table-2">
<thead><tr>
<th><p id="h2-name">$NAME</p></th>
<th><p id="h2-price">$PRICE</p></th>
<th><p id="h2-note">$NOTE</p></th>
</tr></thead>
<tbody>
<tr>
<td><p id="b1-name">Checkboxless Drop</p></td>
<td><p id="b1-price">100</p></td>
<td><p id="b1-note">n</p></td>
</tr>
</tbody>
</table>
</fragment>
"@

$script:failures = New-Object System.Collections.Generic.List[string]
function Assert-True {
	param([bool]$Condition, [string]$Message)
	if (-not $Condition) { $script:failures.Add($Message); Write-Output ("ASSERT FAIL: " + $Message) }
}

try {
	$enc = New-Object System.Text.UTF8Encoding($false)
	[IO.File]::WriteAllText($fixturePath, $fixture, $enc)

	& $parserPath -InputXml $fixturePath -SourceKind 'biology_drop' -OutJson $outJson -OutMd $outMd -SourcesJson $noSources | Out-Null

	$inv = [IO.File]::ReadAllText($outJson) | ConvertFrom-Json
	$entries = @($inv.entries)

	$e1 = @($entries | Where-Object { $_.name_en -eq 'Mixed Weapon' })[0]
	$e2 = @($entries | Where-Object { $_.name_en -eq 'Both False' })[0]
	$e3 = @($entries | Where-Object { $_.name_en -eq 'All Done' })[0]
	$e4 = @($entries | Where-Object { $_.name_en -eq 'Yellow Fill' })[0]
	$e5 = @($entries | Where-Object { $_.name_en -eq 'Checkboxless Drop' })[0]

	# Fixture sanity: all five rows must have been emitted.
	Assert-True (@($entries).Count -eq 5) "expected 5 entries, got $($entries.Count)"

	# Header-anchored lookup (TEX is column index 2, not 1): art must come from
	# the texture checkbox, code from the code checkbox.
	Assert-True ($null -ne $e1) 'Mixed Weapon entry missing'
	if ($e1) {
		Assert-True ($e1.artwork_complete -eq $true) 'Mixed Weapon artwork_complete should be true (texture checkbox read)'
		Assert-True ($e1.code_complete -eq $false) 'Mixed Weapon code_complete should be false (code checkbox read)'
		# Colour requires BOTH checkboxes complete: exactly one true must stay unchecked.
		Assert-True ($e1.status -eq 'unchecked') "Mixed Weapon status should be unchecked (no colour when a checkbox is missing), got '$($e1.status)'"
		Assert-True ($e1.feishu.texture_checkbox_id -eq 'tex-r1') "Mixed Weapon texture id mismatch: '$($e1.feishu.texture_checkbox_id)'"
		Assert-True ($e1.feishu.code_checkbox_id -eq 'code-r1') "Mixed Weapon code id mismatch: '$($e1.feishu.code_checkbox_id)'"
		Assert-True ($e1.feishu.row_color -eq 'neutral') "Mixed Weapon row_color should be neutral, got '$($e1.feishu.row_color)'"
	}

	# rowspan alignment: Both False follows a rowspan row and must still map its
	# own checkboxes.
	Assert-True ($null -ne $e2) 'Both False entry missing'
	if ($e2) {
		Assert-True ($e2.artwork_complete -eq $false) 'Both False artwork_complete should be false'
		Assert-True ($e2.code_complete -eq $false) 'Both False code_complete should be false'
		Assert-True ($e2.status -eq 'unchecked') "Both False status should be unchecked, got '$($e2.status)'"
		Assert-True ($e2.feishu.texture_checkbox_id -eq 'tex-r2') "Both False texture id mismatch after rowspan: '$($e2.feishu.texture_checkbox_id)'"
		Assert-True ($e2.feishu.row_color -eq 'neutral') "Both False row_color should be neutral, got '$($e2.feishu.row_color)'"
	}

	# Both checkboxes true with a green fill -> green.
	Assert-True ($null -ne $e3) 'All Done entry missing'
	if ($e3) {
		Assert-True ($e3.artwork_complete -eq $true) 'All Done artwork_complete should be true'
		Assert-True ($e3.code_complete -eq $true) 'All Done code_complete should be true'
		Assert-True ($e3.status -eq 'green') "All Done status should be green, got '$($e3.status)'"
		Assert-True ($e3.feishu.row_color -eq 'green') "All Done row_color should be green, got '$($e3.feishu.row_color)'"
	}

	# A yellow fill with both checkboxes false must NOT be read as completion:
	# the composite status stays unchecked even though the row is yellow-filled.
	Assert-True ($null -ne $e4) 'Yellow Fill entry missing'
	if ($e4) {
		Assert-True ($e4.artwork_complete -eq $false) 'Yellow Fill artwork_complete must stay false'
		Assert-True ($e4.code_complete -eq $false) 'Yellow Fill code_complete must stay false'
		Assert-True ($e4.status -eq 'unchecked') "Yellow Fill status must stay unchecked despite yellow fill, got '$($e4.status)'"
		Assert-True ($e4.feishu.row_color -eq 'yellow') "Yellow Fill row_color should be yellow, got '$($e4.feishu.row_color)'"
	}

	# Checkbox-less biology-drop row must be emitted with a blocker, never skipped.
	Assert-True ($null -ne $e5) 'Checkboxless Drop entry missing (row was skipped)'
	if ($e5) {
		Assert-True ($e5.source_kind -eq 'biology_drop') "Checkboxless Drop source_kind should be biology_drop, got '$($e5.source_kind)'"
		$blk = @($e5.blockers) -join '; '
		Assert-True ($blk -like "*$BLOCKER*") "Checkboxless Drop missing blocker '$BLOCKER' (got '$blk')"
		Assert-True ($e5.artwork_complete -eq $false) 'Checkboxless Drop artwork_complete should be false'
		Assert-True ($e5.code_complete -eq $false) 'Checkboxless Drop code_complete should be false'
		Assert-True ($e5.status -eq 'unchecked') "Checkboxless Drop status should be unchecked, got '$($e5.status)'"
	}

	# No non-status fill may be reported as a completion status colour.
	$nonStatusRows = @($entries | Where-Object { $_.name_en -in @('Mixed Weapon', 'Both False') })
	foreach ($e in $nonStatusRows) {
		Assert-True ($e.feishu.row_color -eq 'neutral') "non-status fill read as '$($e.feishu.row_color)' for '$($e.name_en)'"
	}
}
catch {
	$script:failures.Add("exception: $($_.Exception.Message)")
	Write-Output ("ASSERT FAIL: exception: " + $_.Exception.Message)
}
finally {
	if (Test-Path -LiteralPath $tmpDir) { Remove-Item -LiteralPath $tmpDir -Recurse -Force -ErrorAction SilentlyContinue }
}

if ($script:failures.Count -gt 0) {
	Write-Output "test-parser: FAILED ($($script:failures.Count) assertion(s))"
	exit 1
}

Write-Output 'test-parser: PASSED (column anchoring, rowspan, status colours, blocker emission)'
exit 0
