---
phase: 01-item-inventory-completed-art-items
reviewed: 2026-09-13T15:35:00Z
depth: standard
base_ref: 6007fd9525fdf419a1b213ca8811e7dc94d0c39e
files_reviewed: 3
files_reviewed_list:
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs
  - Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs
findings:
  critical: 0
  warning: 2
  info: 4
  total: 6
status: issues_found
---

# Phase 01: Code Review Report

**Reviewed:** 2026-09-13T15:35:00Z
**Depth:** standard
**Base ref:** `6007fd9525` (previous 01-REVIEW.md commit; plan-07 delta)
**Files Reviewed:** 3
**Status:** issues_found

## Summary

Wave-scoped review of the three source files touched by gap-closure plan 01-07
(`ArmOfGiantTree`, `KelpCurtainPlayer`, `ArmOfGiantTreeChargePacket`). The plan's
three focal fixes land as described: the shared `ChargeTimer` is gone (prior **CR-01**
and **IN-01** closed), the right-click swing is exempted from the charge scaling via the
`altFunctionUse != 2` gate (prior **WR-02** closed), and the shockwave now runs from the
authoritative packet handler rather than the owner's client (prior **WR-01** substantially
closed). The new fields are transient (no `SaveData`/`LoadData`), the packet reader/writer
order matches, `LocalizationCategory` is present, and no binary/art asset was touched.

Two robustness/authority gaps remain in the new networking code (WR-01/WR-02 below); the
prior report's remaining Info items on unrelated carry-over items were out of this wave's
scope. The earlier 5-file review of plan 01-06 is preserved in git history.

## Warnings

### WR-01: The authoritative shockwave trusts a client-controlled release flag

**File:** `Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs:51-56`
**Issue:** `Handle` applies the full-charge AoE whenever `packetData.ReleaseSmash && IsHeldBy(player)`.
`ReleaseSmash` is a plain client-supplied bool, and the handler never checks that
`packetData.Charge` actually reached `ArmOfGiantTree.MaxChargeFrames` (it clamps the
value but does not gate on it). A modified client can send `ReleaseSmash = true,
Charge = 0` and get the 200-px server-authoritative shockwave (plus the locally scaled
primary hit) without ever charging, defeating the "server-authoritative" intent of WR-01.
`IsHeldBy` only proves item possession, not that the charge was earned.
**Fix:** Gate the release on the replicated charge, e.g.:
```csharp
if (packetData.ReleaseSmash
    && ArmOfGiantTree.IsHeldBy(player)
    && packetData.Charge >= ArmOfGiantTree.MaxChargeFrames)
{
    ArmOfGiantTree.ApplyShockwave(player);
}
```

### WR-02: The per-stack slot discriminator is never synchronized

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs:77-97`
**Issue:** `CopyClientState`/`SendClientChanges` and `ArmOfGiantTreeChargePacket` carry only
`ArmOfGiantTreeCharge`; `ArmOfGiantTreeChargedSlot` stays at its `-1` sentinel on every
non-owner instance. `ArmOfGiantTree.HoldItem` (`ArmOfGiantTree.cs:47-51`) treats
`player.selectedItem != ArmOfGiantTreeChargedSlot` as a slot switch and zeroes the charge,
so wherever the replicated state is evaluated on the sender's behalf (e.g. the server, if
`HoldItem` runs there) the received charge is cleared on the next tick. The observable
consequence is that the replicated `Charge` has no durable consumer, and the WR-01 fix
above cannot be implemented because the authoritative side never has a trustworthy
full-charge signal. The client-side per-stack isolation itself works.
**Fix:** Include `ArmOfGiantTreeChargedSlot` in `CopyClientState`/`SendClientChanges` and in
the packet payload (read/write order kept in lockstep), or drop the slot-coupled reset on
the authoritative side and validate from the packet's `Charge` value.

## Info

### IN-01: `SendClientChanges` reaches only the server, not other clients

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs:88-95`
**Issue:** The comment says the change is sent "to the world (server and other clients)",
but `ModIns.PacketResolver.Send(..., RouteDestination.WorldOnly)` from a client is executed
by the server and is not forwarded (`PacketResolver.Resolve` returns `Forward = false` for
`WorldOnly` from a client slot), so other clients never receive it. Harmless today because
no other client consumes the charge.
**Fix:** Correct the comment, or route via `AllDownstream` if remote-client replication is
actually intended.

### IN-02: `npc.netUpdate = true` is redundant after `SimpleStrikeNPC`

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs:142-143`
**Issue:** `SimpleStrikeNPC` already handles multiplayer synchronization for the damage;
setting `netUpdate` again is a no-op for correctness.
**Fix:** Drop the extra assignment (or keep with a comment noting intent).

### IN-03: Unreachable null guard on `Main.player[whoAmI]`

**File:** `Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs:40-44`
**Issue:** `Main.player[whoAmI]` is a pre-allocated array element and is never null; the
`player is null` branch is dead. `whoAmI` is always a valid transport index.
**Fix:** Keep `!player.active` if desired; the null check can be removed.

### IN-04: Packet payload uses mutable public fields

**File:** `Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs:13,20`
**Issue:** `Charge`/`ReleaseSmash` are public mutable fields. Matches the existing
`PermanentBoostPacket` convention, so this is a consistency note rather than a defect.
**Fix:** Prefer auto-properties if the local pattern is later tightened.

---

_Reviewed: 2026-09-13T15:35:00Z_
_Reviewer: gsd-code-reviewer (orchestrator-inline; subagent runtime unavailable)_
_Depth: standard_
