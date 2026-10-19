# Run after dotnet build. Uses the installed tML FNA backend; does not load Terraria.
# -Legacy is a negative control: the previous shader should fail the shape tests.
param(
	[Parameter(Mandatory = $true)]
	[string]$TmlDirectory,
	[string]$Configuration = 'Debug',
	[switch]$Legacy
)

$ErrorActionPreference = 'Stop'
$sourceRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$fna = @(Get-ChildItem -LiteralPath (Join-Path $TmlDirectory 'Libraries') -Filter 'FNA.dll' -Recurse)
if ($fna.Count -ne 1) { throw 'Expected one FNA.dll in the tModLoader Libraries directory.' }
$probeDirectory = Join-Path ([IO.Path]::GetTempPath()) ('EverglowJellyReflection-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $probeDirectory | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'KingJellyBallReflectionProbe.cs') -Destination (Join-Path $probeDirectory 'Program.cs')
$fnaPath = [Security.SecurityElement]::Escape($fna[0].FullName)
$project = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup><Reference Include="FNA"><HintPath>$fnaPath</HintPath></Reference></ItemGroup>
</Project>
"@
[IO.File]::WriteAllText((Join-Path $probeDirectory 'Probe.csproj'), $project, [Text.UTF8Encoding]::new($false))
$previousPath = $env:PATH
try {
	$env:PATH = (Join-Path $TmlDirectory 'Libraries/Native/Windows') + [IO.Path]::PathSeparator + $previousPath
	$probeArguments = @($sourceRoot, $Configuration)
	if ($Legacy) { $probeArguments += '--legacy' }
	dotnet run --project (Join-Path $probeDirectory 'Probe.csproj') -- @probeArguments
	if ($LASTEXITCODE -ne 0) { throw "GPU reflection regression failed (exit $LASTEXITCODE)." }
}
finally {
	$env:PATH = $previousPath
}
