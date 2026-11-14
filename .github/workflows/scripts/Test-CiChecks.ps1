$ErrorActionPreference = 'Stop'
# Isolate Add-Type assemblies and expected native failures between test scripts.
foreach ($test in (Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.Tests.ps1' | Sort-Object Name)) {
	& pwsh -NoProfile -File $test.FullName
	if ($LASTEXITCODE -ne 0) { throw "CI check failed: $($test.Name)" }
}
