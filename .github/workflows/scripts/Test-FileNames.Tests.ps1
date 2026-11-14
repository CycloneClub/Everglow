$ErrorActionPreference = 'Stop'
$checker = Join-Path $PSScriptRoot 'Test-FileNames.ps1'
$cases = @(
	@{ Name = 'Plain PascalCase'; Path = 'Sources/Modules/Food/Items/FooBar.cs'; Valid = $true },
	@{ Name = 'Multiple uppercase segments'; Path = 'Sources/Modules/Food/Items/Foo_Proj_Glow.png'; Valid = $true },
	@{ Name = 'Numeric segment'; Path = 'Sources/Modules/Food/Items/Foo_2.png'; Valid = $true },
	@{ Name = 'Repeated underscores'; Path = 'Sources/Modules/Food/Items/Foo__Bar.cs'; Valid = $true },
	@{ Name = 'CamelCase within a capitalized segment'; Path = 'Sources/Modules/Food/Items/Foo_NewStyleTree.png'; Valid = $true },
	@{ Name = 'Lowercase final segment'; Path = 'Sources/Modules/Food/Items/Foo_glow.png'; Valid = $false },
	@{ Name = 'Lowercase middle segment'; Path = 'Sources/Modules/Food/Items/Foo_small_Proj.cs'; Valid = $false },
	@{ Name = 'Lowercase numbered segment'; Path = 'Sources/Modules/Food/Items/Foo_gore2.png'; Valid = $false },
	@{ Name = 'Resource tools images'; Path = 'Resources/Tools/ArmorHelper/Armor_template.png'; Valid = $false },
	@{ Name = 'Windows separators'; Path = 'Sources\Modules\Food\Items\Foo_glow.png'; Valid = $false },
	@{ Name = 'Current directory prefix'; Path = './Sources/Modules/Food/Foo_glow.fx'; Valid = $false },
	@{ Name = 'Directory names are not filename segments'; Path = 'Sources/Modules/Food/area_small/Foo.png'; Valid = $true },
	@{ Name = 'Audio resource'; Path = 'Sources/Modules/Food/Sounds/Foo_attack.ogg'; Valid = $false },
	@{ Name = 'Bitmap resource'; Path = 'Sources/Modules/Food/Tiles/Foo_shape.bmp'; Valid = $false },
	@{ Name = 'Skeleton exports'; Path = 'Sources/Modules/Example/Skeleton/Animations/images/L_eye-iris.png'; Valid = $true },
	@{ Name = 'Skeletons exports'; Path = 'Sources/Modules/Yggdrasil/NPCs/Skeletons/images/Limb_b_1.png'; Valid = $true },
	@{ Name = 'Skeleton exclusion uses a whole segment'; Path = 'Sources/Modules/Food/SkeletonWarrior/Foo_glow.png'; Valid = $false },
	@{ Name = 'Imported Spine source'; Path = 'Sources/Everglow.Function/Skeleton2D/Spine/External_type.cs'; Valid = $true },
	@{ Name = 'Project-owned skeleton adapter'; Path = 'Sources/Everglow.Function/Skeleton2D/Reader/Adapter_small.cs'; Valid = $false },
	@{ Name = 'Generated output'; Path = 'Sources/Modules/Food/obj/Release/Generated_code.cs'; Valid = $true },
	@{ Name = 'Build output'; Path = 'Sources/Modules/Food/bin/Release/Foo_glow.png'; Valid = $true },
	@{ Name = 'Dead module'; Path = 'Sources/Modules/TwilightForest/Items/Foo_small.cs'; Valid = $true },
	@{ Name = 'Live module area with old module name'; Path = 'Sources/Modules/Yggdrasil/TwilightForest/Foo_small.cs'; Valid = $false },
	@{ Name = 'Dead script project'; Path = 'Sources/Everglow.Scripts/Foo_small.cs'; Valid = $true },
	@{ Name = 'Map template'; Path = 'Sources/Modules/Food/Maps/Foo_withRoom.mapio'; Valid = $true },
	@{ Name = 'Art source'; Path = 'Sources/Modules/Food/Foo_paint.pdn'; Valid = $true },
	@{ Name = 'External atlas'; Path = 'Sources/Modules/Food/Foo_data.atlas'; Valid = $true },
	@{ Name = 'Publishing small icon'; Path = 'Sources/Everglow/icon_small.png'; Valid = $true },
	@{ Name = 'Publishing workshop icon'; Path = 'Sources/Everglow/workshop_icon_512x.png'; Valid = $true },
	@{ Name = 'Publishing exception is not global'; Path = 'Sources/Modules/Food/icon_small.png'; Valid = $false },
	@{ Name = 'Other repository tools'; Path = 'Tools/Imported/Foo_small.cs'; Valid = $true }
)
foreach ($case in $cases) {
	$passed = $true
	try { & $checker -Paths @($case.Path) *> $null }
	catch {
		$passed = $false
		if (-not $_.Exception.Message.Contains($case.Path.Replace('\', '/').Replace('./', ''))) {
			throw "Diagnostic did not identify '$($case.Path)': $_"
		}
	}
	if ($passed -ne $case.Valid) { throw "Unexpected filename result: $($case.Name)" }
	Write-Host "PASS: $($case.Name)"
}

$failed = $false
try { & $checker -Paths @('Sources/Modules/Food/Foo_Glow.png', 'Sources/Modules/Food/Foo_glow.png') *> $null }
catch {
	$failed = $true
	if (-not $_.Exception.Message.Contains('Sources/Modules/Food/Foo_glow.png')) { throw 'Missing case-sensitive path diagnostic.' }
}
if (-not $failed) { throw 'Case-insensitive de-duplication hid an invalid filename.' }
Write-Host 'PASS: Case-sensitive duplicate paths remain checked'
