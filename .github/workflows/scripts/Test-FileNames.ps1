[CmdletBinding()]
param([AllowEmptyCollection()][string[]]$Paths)

$ErrorActionPreference = 'Stop'
if (-not $PSBoundParameters.ContainsKey('Paths')) {
	$repository = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
	$listed = git -C $repository -c core.quotepath=false ls-files --cached --others --exclude-standard -z
	if ($LASTEXITCODE -ne 0) { throw 'Cannot enumerate repository filenames.' }
	$Paths = @(($listed -join "`n") -split "`0" | Where-Object {
		$_ -and (Test-Path -LiteralPath (Join-Path $repository $_) -PathType Leaf)
	})
}

$failures = [Collections.Generic.List[string]]::new()
$checked = 0
foreach ($candidate in ($Paths | Sort-Object -Unique -CaseSensitive)) {
	$path = $candidate.Replace('\', '/') -replace '^\./', ''
	if ($path -notmatch '^(Sources|Resources)/' -or $path -notmatch '\.(cs|png|bmp|fx|ogg|wav)$') { continue }
	if ($path -match '/(bin|obj|Skeleton|Skeletons)/' -or
		$path -match '^Sources/Modules/(IIID|TwilightForest|ZY)/' -or
		$path -match '^Sources/Everglow\.Scripts/' -or
		$path -match '^Sources/Everglow\.Function/Skeleton2D/Spine/') { continue }
	# Publishing tools require these names; similarly named gameplay assets are still checked.
	if ($path -cin @('Sources/Everglow/icon_small.png', 'Sources/Everglow/workshop_icon_512x.png')) { continue }

	$checked++
	$name = [IO.Path]::GetFileNameWithoutExtension($path)
	if ($name -cmatch '_[a-z]') { $failures.Add($path) }
}
if ($failures.Count) {
	throw "Filename segments after underscores must start with uppercase letters:`n$($failures -join "`n")"
}
Write-Host "File naming check passed ($checked project code/resource files)."
