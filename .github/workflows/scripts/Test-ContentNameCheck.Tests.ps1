# Checks registered content identities, independently of CLR type spelling.
$ErrorActionPreference = 'Stop'
Add-Type -Path "$PSScriptRoot/../asset-audit/ContentNameCheck.cs"
$cases = @(
	@{ Name = 'FooBar'; Valid = $true },
	@{ Name = 'Foo_Proj_Glow'; Valid = $true },
	@{ Name = 'Foo_2'; Valid = $true },
	@{ Name = 'Foo_2_Glow'; Valid = $true },
	@{ Name = 'Foo__Bar'; Valid = $true },
	@{ Name = 'Foo_NewStyleTree'; Valid = $true },
	@{ Name = 'lowerInitial'; Valid = $true },
	@{ Name = 'Foo_proj'; Valid = $false },
	@{ Name = 'Foo_small_Proj'; Valid = $false },
	@{ Name = 'Foo_newStyleTree'; Valid = $false },
	@{ Name = 'Foo_2_glow'; Valid = $false },
	@{ Name = '_private'; Valid = $false },
	@{ Name = ''; Valid = $false },
	@{ Name = $null; Valid = $false }
)
foreach ($case in $cases) {
	$actual = [CiAssetAudit.ContentNameCheck]::IsCanonical($case.Name)
	if ($actual -ne $case.Valid) { throw "Content name '$($case.Name)': expected $($case.Valid), got $actual" }
	Write-Host "PASS: '$($case.Name)' => $actual"
}
