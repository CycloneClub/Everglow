# Exercise the repository's actual naming configuration with the CI SDK.
# The temporary project has no package dependencies and does not reference Terraria.
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path -LiteralPath "$PSScriptRoot/../../..").Path
[xml]$ciProps = Get-Content -LiteralPath (Join-Path $repo '.github/workflows/Directory.Build.props') -Raw
if ($ciProps.Project.PropertyGroup.EnforceCodeStyleInBuild -ne 'true') { throw 'CI must enforce code style during builds.' }
[xml]$localProps = Get-Content -LiteralPath (Join-Path $repo 'Sources/Directory.Build.props') -Raw
# Local builds use IDE diagnostics; CI promotes naming warnings to build errors.
if ($localProps.Project.PropertyGroup.ContinuousIntegrationBuild -ne 'true' -and
	$localProps.Project.PropertyGroup.EnforceCodeStyleInBuild -eq 'true') {
	throw 'Local builds must leave code style checking to the IDE.'
}
$sdk = @(dotnet --list-sdks | ForEach-Object {
	if ($_ -match '^(8\.0\.\d+) ') { [version]$Matches[1] }
}) | Sort-Object -Descending | Select-Object -First 1
if (-not $sdk) { throw '.NET SDK 8 is required for the naming rule regression check.' }
$probe = Join-Path ([IO.Path]::GetTempPath()) ('everglow-naming-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $probe | Out-Null
Copy-Item -LiteralPath (Join-Path $repo '.editorconfig') -Destination (Join-Path $probe '.editorconfig')
@{ sdk = @{ version = $sdk.ToString(); rollForward = 'disable' } } | ConvertTo-Json |
	Set-Content -LiteralPath (Join-Path $probe 'global.json') -Encoding utf8NoBOM
@'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <NuGetAudit>false</NuGetAudit>
  </PropertyGroup>
</Project>
'@ | Set-Content -LiteralPath (Join-Path $probe 'NamingProbe.csproj') -Encoding utf8NoBOM
@'
public class PlainPascalCase { }
public class Foo_Proj_Glow { }
public class Foo_2 { }
public class Foo_NewStyleTree { }
public interface IFoo_Proj { }
public struct Struct_Proj { }
public enum Enum_Proj { Value }
public delegate void Delegate_Proj();
public class Members { public event System.Action Changed; }
'@ | Set-Content -LiteralPath (Join-Path $probe 'Cases.cs') -Encoding utf8NoBOM
Push-Location $probe
try {
	$validLog = & dotnet build --nologo /p:WarningsAsErrors=IDE1006 2>&1 | Out-String
	if ($LASTEXITCODE -ne 0) { throw "Valid names failed in ${probe}:`n$validLog" }
	Write-Host 'PASS: PascalCase, underscore segments, numbers and all type kinds compile.'
	@'
public class BadType_proj { }
public class BadMiddle_small_Proj { }
public interface IBadInterface_proj { }
public struct BadStruct_proj { }
public enum BadEnum_proj { Value }
public delegate void BadDelegate_proj();
public class BadMembers { public event System.Action changed; }
'@ | Set-Content -LiteralPath (Join-Path $probe 'Cases.cs') -Encoding utf8NoBOM
	$localLog = & dotnet build --no-restore --nologo /p:EnforceCodeStyleInBuild=false 2>&1 | Out-String
	if ($LASTEXITCODE -ne 0) { throw "Local build failed in ${probe}:`n$localLog" }
	if ($localLog -match 'IDE1006') { throw "Local build unexpectedly ran naming checks:`n$localLog" }
	Write-Host 'PASS: Local builds succeed without IDE1006 build diagnostics.'
	$ciErrors = @($ciProps.Project.PropertyGroup.WarningsAsErrors -split ';' | ForEach-Object { $_.Trim() })
	if ('IDE1006' -notin $ciErrors) { throw 'CI does not promote IDE1006 to an error.' }
	$invalidLog = & dotnet build --no-restore --no-incremental --nologo /p:WarningsAsErrors=IDE1006 2>&1 | Out-String
	if ($LASTEXITCODE -eq 0) { throw "Invalid type names compiled successfully in $probe" }
	foreach ($line in 1..7) {
		if ($invalidLog -notmatch "Cases\.cs\($line,\d+\): error IDE1006:") {
			throw "Missing IDE1006 for invalid declaration on line ${line}:`n$invalidLog"
		}
	}
	Write-Host 'PASS: CI promotes every invalid type declaration to an IDE1006 error.'
}
finally { Pop-Location }
# The CI build is expected to fail; do not leak its exit code to the CI runner.
$global:LASTEXITCODE = 0
