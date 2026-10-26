function Assert-ModLoadResult {
	[CmdletBinding()]
	param(
		[AllowEmptyString()][string]$LogText,
		[int]$ExitCode,
		[bool]$TimedOut,
		[switch]$RequireAssetAudit
	)
	if ($TimedOut) { throw 'tModLoader load timed out.' }
	if ($ExitCode -ne 0) { throw "tModLoader exited with code $ExitCode." }
	if ($LogText -match '\[(?:[^\]\r\n]*/)?(?:ERROR|FATAL)\]') {
		throw 'tModLoader reported an error. See server.log.'
	}
	foreach ($name in @('Everglow', 'SubworldLibrary', 'ModLiquidLib')) {
		if ($LogText -notmatch "(?m)\[tML\]: Finalizing Content: $name \(") {
			throw "Missing load evidence for $name."
		}
	}
	if ($LogText -notmatch '\[tML\]: Mod Load Completed in \d+ms') {
		throw 'tModLoader did not complete mod loading.'
	}
	if ($RequireAssetAudit -and $LogText -notmatch '\[CiAssetAudit\]: Asset audit passed: Everglow; content=[1-9]\d*; equipment=[1-9]\d*\.') {
		throw 'Missing evidence that the content and equipment texture audit completed.'
	}
}
