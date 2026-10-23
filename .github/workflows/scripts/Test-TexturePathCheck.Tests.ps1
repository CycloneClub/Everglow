# Exercises package lookup without loading Terraria or creating graphics resources.
$ErrorActionPreference = 'Stop'
Add-Type -Path "$PSScriptRoot/../asset-audit/TexturePathCheck.cs"
$inventories = [Collections.Generic.Dictionary[string, Collections.Generic.HashSet[string]]]::new([StringComparer]::Ordinal)
$inventories.Add('Everglow', [Collections.Generic.HashSet[string]]::new([string[]]@(
	'Armor/Helmet.png', 'Armor/Helmet_Head.rawimg', 'Armor/Boots.png', 'Effects/Glow.xnb', 'Sounds/Only.ogg'
), [StringComparer]::Ordinal))
$inventories.Add('Dependency', [Collections.Generic.HashSet[string]]::new([string[]]@('Shared.png'), [StringComparer]::Ordinal))
$cases = @(
	@{ Path = 'Everglow/Armor/Helmet'; Result = 'Present' },
	@{ Path = 'Everglow/Armor/Helmet_Head'; Result = 'Present' },
	@{ Path = 'Everglow/Effects/Glow'; Result = 'Present' },
	@{ Path = 'Dependency/Shared'; Result = 'Present' },
	@{ Path = 'Everglow/Armor/Boots_Legs'; Result = 'Missing' },
	@{ Path = 'Everglow/armor/Helmet'; Result = 'Missing' },
	@{ Path = 'MissingMod/Shared'; Result = 'Missing' },
	@{ Path = 'Everglow/Sounds/Only'; Result = 'Missing' },
	@{ Path = 'Terraria/Images/Item_1'; Result = 'Vanilla' },
	@{ Path = 'Everglow:Armor/Helmet'; Result = 'Present' },
	@{ Path = 'Everglow/Armor\Helmet'; Result = 'Present' },
	@{ Path = 'Armor'; Result = 'Invalid' },
	@{ Path = 'Everglow/'; Result = 'Invalid' },
	@{ Path = ''; Result = 'Invalid' },
	@{ Path = $null; Result = 'Invalid' }
)
foreach ($case in $cases) {
	$actual = [CiAssetAudit.TexturePathCheck]::Check($case.Path, $inventories).ToString()
	if ($actual -ne $case.Result) { throw "Texture '$($case.Path)': expected $($case.Result), got $actual" }
	Write-Host "PASS: '$($case.Path)' => $actual"
}
