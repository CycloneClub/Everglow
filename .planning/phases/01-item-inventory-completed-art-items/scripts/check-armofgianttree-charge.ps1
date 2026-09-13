<#
.SYNOPSIS
	Phase 1 gap-closure gate for the ArmOfGiantTree charge rework.

.DESCRIPTION
	Structural gate for the CR-01 / WR-02 / WR-01 closure (plan 01-07). It reads the
	three changed source files with [IO.File]::ReadAllText (UTF-8) and fails (exit 1)
	when any required property of the rework is missing.

	Task 1 checks (CR-01 per-player + per-stack, WR-02 charged-left-click gate):
	  - ArmOfGiantTree.cs no longer declares the shared per-type charge field
	    (public int <OldFieldName>);
	  - ArmOfGiantTree.cs resolves the charge through GetModPlayer<KelpCurtainPlayer>;
	  - KelpCurtainPlayer.cs exposes ArmOfGiantTreeCharge, ArmOfGiantTreeChargedSlot,
	    CopyClientState and SendClientChanges;
	  - ArmOfGiantTree.cs pairs player.selectedItem with ArmOfGiantTreeChargedSlot
	    (the per-stack slot discriminator);
	  - Netcode/ArmOfGiantTreeChargePacket.cs exists and implements IPacket with a
	    [HandlePacket] handler;
	  - ArmOfGiantTree.cs contains at least two 'altFunctionUse != 2' occurrences.

	Task 2 checks (WR-01 server-authoritative shockwave):
	  - ArmOfGiantTree.cs has a NetmodeID.MultiplayerClient release branch;
	  - ArmOfGiantTree.cs and the packet file contain ReleaseSmash, SimpleStrikeNPC
	    and netUpdate;
	  - ArmOfGiantTree.cs no longer contains the old owning-client damage gate
	    (the Main.myPlayer equality comparison WR-01 flagged);
	  - ArmOfGiantTree.cs contains no bare unqualified item damage/knockBack access
	    that would not compile inside the static ApplyShockwave helper.

	This script is deliberately 100% ASCII (PowerShell 5.1 reads a BOM-less script as
	the system ANSI code page); the sources are read through [IO.File]::ReadAllText.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$repoRoot = (Resolve-Path (Join-Path $phaseDir '..\..\..')).Path

$armRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs'
$playerRel = 'Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs'
$packetRel = 'Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs'

$armPath = Join-Path $repoRoot ($armRel -replace '/', '\')
$playerPath = Join-Path $repoRoot ($playerRel -replace '/', '\')
$packetPath = Join-Path $repoRoot ($packetRel -replace '/', '\')

$failures = New-Object System.Collections.Generic.List[string]
function Add-Failure([string]$message) { $script:failures.Add($message) }

foreach ($pair in @(@($armRel, $armPath), @($playerRel, $playerPath))) {
	if (-not (Test-Path -LiteralPath $pair[1] -PathType Leaf)) {
		Write-Output "FAIL: required source not found: $($pair[0])"
		exit 2
	}
}

$armText = [IO.File]::ReadAllText($armPath)
$playerText = [IO.File]::ReadAllText($playerPath)

# --- Task 1: CR-01 per-player / per-stack charge -------------------------------
if ($armText -match 'public\s+int\s+ChargeTimer') {
	Add-Failure "ArmOfGiantTree.cs : shared per-type charge field '(public int ChargeTimer)' still declared"
}
if (-not ($armText -match 'GetModPlayer<KelpCurtainPlayer>')) {
	Add-Failure "ArmOfGiantTree.cs : does not resolve the charge through GetModPlayer<KelpCurtainPlayer>"
}
if (-not ($playerText -match '\bArmOfGiantTreeCharge\b')) {
	Add-Failure "KelpCurtainPlayer.cs : missing 'ArmOfGiantTreeCharge' member"
}
if (-not ($playerText -match '\bArmOfGiantTreeChargedSlot\b')) {
	Add-Failure "KelpCurtainPlayer.cs : missing 'ArmOfGiantTreeChargedSlot' member"
}
if (-not ($playerText -match 'CopyClientState')) {
	Add-Failure "KelpCurtainPlayer.cs : missing 'CopyClientState' override"
}
if (-not ($playerText -match 'SendClientChanges')) {
	Add-Failure "KelpCurtainPlayer.cs : missing 'SendClientChanges' override"
}
if (-not (($armText -match 'player\.selectedItem') -and ($armText -match 'ArmOfGiantTreeChargedSlot'))) {
	Add-Failure "ArmOfGiantTree.cs : missing per-stack isolation (player.selectedItem + ArmOfGiantTreeChargedSlot)"
}
if (-not (Test-Path -LiteralPath $packetPath -PathType Leaf)) {
	Add-Failure "Netcode/ArmOfGiantTreeChargePacket.cs : file missing"
	$packetText = ''
}
else {
	$packetText = [IO.File]::ReadAllText($packetPath)
	if (-not ($packetText -match 'IPacket')) {
		Add-Failure "ArmOfGiantTreeChargePacket.cs : does not implement IPacket"
	}
	if (-not ($packetText -match 'HandlePacket')) {
		Add-Failure "ArmOfGiantTreeChargePacket.cs : missing [HandlePacket] handler"
	}
}
$altGates = ([regex]::Matches($armText, 'altFunctionUse != 2')).Count
if ($altGates -lt 2) {
	Add-Failure "ArmOfGiantTree.cs : found $altGates 'altFunctionUse != 2' gate(s); expected at least 2 (WR-02)"
}

# --- Task 2: WR-01 server-authoritative shockwave ------------------------------
if (-not ($armText -match 'NetmodeID\.MultiplayerClient')) {
	Add-Failure "ArmOfGiantTree.cs : missing NetmodeID.MultiplayerClient release branch (WR-01)"
}
$combined = $armText + "`n" + $packetText
foreach ($token in @('ReleaseSmash', 'SimpleStrikeNPC', 'netUpdate')) {
	if (-not ($combined -match $token)) {
		Add-Failure "WR-01 closure : missing '$token' in ArmOfGiantTree.cs / ArmOfGiantTreeChargePacket.cs"
	}
}
if ($armText -match 'Main\.myPlayer\s*==\s*player\.whoAmI') {
	Add-Failure "ArmOfGiantTree.cs : old owning-client damage gate 'Main.myPlayer == player.whoAmI' still present (WR-01)"
}
$bareDamage = @([regex]::Matches($armText, '(?<!\.)\bdamage\b') | Where-Object { $_.Index -gt 0 -and $armText[$_.Index - 1] -eq '.' })
$bareKnockback = @([regex]::Matches($armText, '(?<!\.)\bknockBack\b') | Where-Object { $_.Index -gt 0 -and $armText[$_.Index - 1] -eq '.' })
if ($bareDamage.Count -gt 0 -or $bareKnockback.Count -gt 0) {
	Add-Failure "ArmOfGiantTree.cs : unqualified instance-item damage/knockBack access (would not compile in the static ApplyShockwave)"
}

if ($failures.Count -gt 0) {
	Write-Output "FAIL($($failures.Count)):"
	$failures | ForEach-Object { Write-Output "  - $_" }
	exit 1
}

Write-Output 'OK(0): ArmOfGiantTree charge is per-player + per-stack slot-keyed, synced, and server-authoritative'
exit 0
