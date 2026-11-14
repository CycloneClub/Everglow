# Checks the shipped MapIO index contract without loading Terraria or a world.
$ErrorActionPreference = 'Stop'
Add-Type -Path "$PSScriptRoot/../asset-audit/ContentNameCheck.cs", "$PSScriptRoot/../asset-audit/MapTemplateCheck.cs"
$resolved = [Collections.Generic.List[string]]::new()
$exists = [Func[string, char, bool]]{
	param($name, $kind)
	$resolved.Add("${kind}:$name")
	return $name -ne 'Everglow/Missing'
}
$cases = @(
	@{ Name = 'Tile wall and entity'; Entries = @('Everglow/Tree_Smallt', 'Everglow/Wall_2w', 'Everglow/Chest_Entitye'); Valid = $true },
	@{ Name = 'Other mod naming is preserved'; Entries = @('Other_mod/tree_smallt'); Valid = $true },
	@{ Name = 'Old name rejected'; Entries = @('Everglow/Tree_smallt'); Valid = $false },
	@{ Name = 'Middle lowercase segment rejected'; Entries = @('Everglow/Tree_small_Brancht'); Valid = $false },
	@{ Name = 'Missing content rejected'; Entries = @('Everglow/Missingt'); Valid = $false },
	@{ Name = 'Unknown kind rejected'; Entries = @('Everglow/Treex'); Valid = $false },
	@{ Name = 'Missing mod rejected'; Entries = @('Treet'); Valid = $false },
	@{ Name = 'Empty name rejected'; Entries = @(''); Valid = $false },
	@{ Name = 'Vanilla-only template'; Entries = @(); Valid = $true },
	@{ Name = 'Negative count rejected'; Entries = @(); Count = -1; Valid = $false },
	@{ Name = 'Truncated index rejected'; Entries = @(); Count = 1; Valid = $false }
)
foreach ($case in $cases) {
	$stream = [IO.MemoryStream]::new()
	$zip = [IO.Compression.GZipStream]::new($stream, [IO.Compression.CompressionLevel]::Optimal, $true)
	$writer = [IO.BinaryWriter]::new($zip)
	$count = if ($case.ContainsKey('Count')) { $case.Count } else { $case.Entries.Count }
	$writer.Write([int]$count)
	$id = 0
	foreach ($entry in $case.Entries) {
		$writer.Write([string]$entry)
		$writer.Write([ushort]$id++)
	}
	# A valid index is followed by opaque map data; the audit must not interpret it.
	if (-not $case.ContainsKey('Count')) { $writer.Write([byte[]]@(0, 255, 18, 0)) }
	$writer.Dispose()
	$stream.Position = 0
	$before = $stream.ToArray()
	$resolved.Clear()
	$failures = [CiAssetAudit.MapTemplateCheck]::Check($stream, $exists)
	if (($failures.Count -eq 0) -ne $case.Valid) { throw "$($case.Name): unexpected result: $failures" }
	if (-not $stream.CanRead) { throw 'Audit closed the caller-owned stream.' }
	if ([Convert]::ToBase64String($before) -ne [Convert]::ToBase64String($stream.ToArray())) { throw 'Audit changed the template.' }
	if ($case.Name -eq 'Tile wall and entity' -and ($resolved -join ',') -ne 't:Everglow/Tree_Small,w:Everglow/Wall_2,e:Everglow/Chest_Entity') {
		throw 'Wrong content name or kind passed to the resolver.'
	}
	$stream.Dispose()
	Write-Host "PASS: $($case.Name)"
}
