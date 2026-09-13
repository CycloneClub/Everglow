---
phase: 01-item-inventory-completed-art-items
plan: "07"
subsystem: gameplay-networking
tags: [tmodloader, melee-weapon, charge-state, netcode, multiplayer-authority, modplayer]

# Dependency graph
requires:
  - phase: 01-item-inventory-completed-art-items
    provides: "01-06 carry-over implementation of ArmOfGiantTree (charge smash + shockwave) and the Phase 1 gate set"
provides:
  - "KelpCurtainPlayer.ArmOfGiantTreeCharge (per-player) + ArmOfGiantTreeChargedSlot (per-stack slot discriminator) with CopyClientState/SendClientChanges sync"
  - "ArmOfGiantTreeChargePacket (IPacket + [HandlePacket]) carrying Charge + ReleaseSmash"
  - "Server-authoritative ArmOfGiantTree.ApplyShockwave(Player) reading player.HeldItem, setting npc.netUpdate"
  - "scripts/check-armofgianttree-charge.ps1 structural gate"
  - "01-DEVIATIONS.md Gap Closure (01-07) ledger section"
affects: [phase-verify-work, phase-02-localization, phase-08-source-status-sync]

# Actuals (#2632) — same estimateTokens scale (chars/4 over the realized diff).
actuals:
  tokens: 910
  tasks: 3
  commits: 3

tech-stack:
  added: []
  patterns:
    - "Per-player transient gameplay state on ModPlayer with a slot-keyed discriminator for per-stack isolation"
    - "Client-change-detected packet sync via ModIns.PacketResolver (CopyClientState/SendClientChanges)"
    - "Server-authoritative area damage with spoof validation (sender must hold the item) and input clamping"

key-files:
  created:
    - Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs
    - .planning/phases/01-item-inventory-completed-art-items/scripts/check-armofgianttree-charge.ps1
  modified:
    - Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs
    - .planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md

key-decisions:
  - "Charge state lives on KelpCurtainPlayer (per-player), never on the shared per-type ModItem; the -1 sentinel slot forces adoption of the held stack on first touch."
  - "Per-stack isolation keys on player.selectedItem (slot) rather than Item.type, because two same-type stacks share Type but occupy different slots."
  - "The client only signals ReleaseSmash; ApplyShockwave runs on the authoritative side (server handler / singleplayer) and sets npc.netUpdate; a dedicated-server UseItem is an explicit no-op."
  - "The 0.75x..2x charge scaling is gated on player.altFunctionUse != 2 so the right-click ordinary swing keeps its base damage."
  - "The plan's verify command hard-codes git merge-base HEAD origin/master; origin/master is a stale distant ancestor that predates the whole phase, so the phase baseline 8ed6f5862 (the base used by 01-VERIFICATION/01-REVIEW) was used instead to isolate plan-07 changes."

patterns-established:
  - "Per-stack isolation: a per-player slot discriminator compared against player.selectedItem resets charge on any stack switch"
  - "Authoritative packet handler validates sender-held item and clamps untrusted ranges before applying effects"

requirements-completed: [QUAL-05]

coverage:
  - id: D1
    description: "ArmOfGiantTree charge is per-player and per-stack: charged slot discriminator reset on player.selectedItem change; shared per-type ModItem field removed."
    requirement: "QUAL-05"
    verification:
      - kind: other
        ref: "powershell -File .planning/phases/01-item-inventory-completed-art-items/scripts/check-armofgianttree-charge.ps1"
        status: pass
    human_judgment: false
  - id: D2
    description: "Charge syncs client to server through ModIns.PacketResolver (CopyClientState/SendClientChanges + ArmOfGiantTreeChargePacket)."
    requirement: "QUAL-05"
    verification:
      - kind: other
        ref: "check-armofgianttree-charge.ps1 (packet IPacket + HandlePacket + CopyClientState/SendClientChanges checks)"
        status: pass
    human_judgment: true
    rationale: "Structural presence is gated, but a real client-to-server charge observation in multiplayer requires a running tModLoader client/server and is not offline-verifiable."
  - id: D3
    description: "Full-charge shockwave is server-authoritative (client signals ReleaseSmash; handler clamps Charge, rejects a sender not holding the item, calls ApplyShockwave + npc.netUpdate; singleplayer direct; dedicated server no double-apply)."
    requirement: "QUAL-05"
    verification:
      - kind: other
        ref: "check-armofgianttree-charge.ps1 (server-authority + WR-01 invariant checks)"
        status: pass
    human_judgment: true
    rationale: "Requires a live multiplayer session to observe server-authoritative NPC damage propagation; offline checks prove structure only."
  - id: D4
    description: "The 0.75x charge floor applies only on the charged left-click; the right-click ordinary swing keeps base damage."
    requirement: "QUAL-05"
    verification:
      - kind: other
        ref: "check-armofgianttree-charge.ps1 (>=2 altFunctionUse != 2 gates)"
        status: pass
    human_judgment: false

duration: ~7min
completed: 2026-09-13
status: complete
---

# Phase 1 Plan 07: ArmOfGiantTree Per-Player, Per-Stack Charge & Server-Authoritative Shockwave Summary

**ArmOfGiantTree charge moved to per-player, slot-keyed per-stack ModPlayer state with ModIns.PacketResolver sync, and its full-charge shockwave made server-authoritative with a ReleaseSmash packet.**

## Performance

- **Duration:** ~7 min
- **Started:** 2026-09-13T15:16Z (approx.)
- **Completed:** 2026-09-13T15:24Z
- **Tasks:** 3
- **Files modified:** 5 (2 created, 3 modified)

## Accomplishments

- **CR-01 closed.** `ArmOfGiantTree.ChargeTimer` (mutable state on the shared per-type `ModItem` singleton) was removed. Charge now lives on `KelpCurtainPlayer.ArmOfGiantTreeCharge`, keyed to the held stack by `ArmOfGiantTreeChargedSlot`; `HoldItem` resets the charge and adopts `player.selectedItem` on any slot change, so two same-type stacks and two players no longer share charge.
- **WR-01 closed.** The full-charge shockwave is server-authoritative: the owning client only sends an `ArmOfGiantTreeChargePacket` with `ReleaseSmash`, singleplayer calls `ApplyShockwave` directly, and the dedicated server applies it exactly once from the packet handler (explicit no-op in `UseItem`). `ApplyShockwave` is static-safe (reads `player.HeldItem` damage/knockback) and sets `npc.netUpdate`.
- **WR-02 closed.** `ModifyWeaponDamage` applies `MathHelper.Lerp(0.75f, 2f, charge)` only under `player.altFunctionUse != 2`; the ordinary right-click swing keeps its unscaled base damage.
- **Sync + spoof hardening.** `KelpCurtainPlayer.CopyClientState`/`SendClientChanges` sync the charge; the packet handler clamps `Charge` to `[0, MaxChargeFrames]` and rejects a release from a sender whose `HeldItem.type` is not `ArmOfGiantTree` (T-07-06).
- **Gate + ledger.** New `check-armofgianttree-charge.ps1` (100% ASCII, offline) enforces the full invariant set; the closure is recorded in `01-DEVIATIONS.md` under `Gap Closure (01-07)`.

## Task Commits

Each task was committed atomically:

1. **Task 1: Move the charge to per-player, per-stack synced state and fix the right-click penalty** - `5ca103693` (fix)
2. **Task 2: Make the full-charge shockwave server-authoritative** - `53a5eddaa` (fix)
3. **Task 3: Record the gap closure and run the Phase 1 regression gates** - `06d8cf0f4` (docs)

**Plan metadata:** `.planning/phases/01-item-inventory-completed-art-items/01-07-SUMMARY.md` (docs, final commit)

_Note: no TDD tasks in this plan._

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs` - Added `ArmOfGiantTreeCharge`, `ArmOfGiantTreeChargedSlot` (init `-1`), `CopyClientState`, `SendClientChanges`; transient (not in `SaveData`/`LoadData`).
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs` - Removed the shared `ChargeTimer` field; per-player read/write; slot-change reset; `altFunctionUse != 2` damage gate; static-safe `ApplyShockwave`; `IsHeldBy`; client/singleplayer/server `UseItem` branches.
- `Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs` - New `IPacket` (`Charge`, `ReleaseSmash`) + nested `[HandlePacket]` handler with clamping and sender validation.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-armofgianttree-charge.ps1` - New structural gate for the CR-01/WR-02/WR-01 invariants.
- `.planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md` - `Gap Closure (01-07)` section.

## Decisions Made

- Charge and its discriminator live on `KelpCurtainPlayer`; the shared `ModItem` singleton holds no gameplay state.
- Per-stack isolation keys on `player.selectedItem` (slot), not `Item.type` (same for both stacks).
- The authoritative side (server handler / singleplayer) owns the shockwave; the client only signals.
- `ReleaseSmash` is validated against the sender's held item and the charge is clamped (untrusted input).
- Verification base: the plan's `git merge-base HEAD origin/master` anchor is stale (origin/master predates the entire phase), so `8ed6f5862` — the base `01-VERIFICATION.md`/`01-REVIEW.md` use — was used to isolate plan-07 changes (see Deviations).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] `PacketResolver.Send` named arguments (`toWho`/`fromWho`) do not exist**
- **Found during:** Task 1 (Release build / gate)
- **Issue:** The plan specified `ModIns.PacketResolver.Send(..., toWho, fromWho)` mirroring `YggdrasilPlayer`, but the public overload's parameters are `toClient`/`ignoreClient`; the first build failed with `error CS1739: "Send" 的最佳重载没有名为 "toWho" 的参数`.
- **Fix:** Used the actual parameter names `toClient: -1, ignoreClient: Main.myPlayer` (matching the `SendClientChanges` change-detection pattern in `YggdrasilPlayer`).
- **Files modified:** `KelpCurtainPlayer.cs`, `ArmOfGiantTree.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p=WarningLevel=0` exits 0; gate exits 0.
- **Committed in:** `5ca103693` (Task 1) and `53a5eddaa` (Task 2)

**2. [Rule 3 - Blocking] Plan verify chain anchors on a stale `origin/master`**
- **Found during:** Task 3 (regression gates)
- **Issue:** `git merge-base HEAD origin/master` resolves to `f222f9316`, a distant ancestor that predates the whole phase branch. Diffing against it reports 1312 changed files (including every pre-existing KelpCurtain `.png` and the Phase-1 `parse-design-xml.ps1`), so the plan's BOM/no-art/parser checks fail for reasons unrelated to plan 07 and cannot isolate its changes.
- **Fix:** Used the phase baseline `8ed6f5862` (the base `01-VERIFICATION.md` and `01-REVIEW.md` already use) for the BOM, no-art, and parser assertions. Verified plan-07's change set is exactly its four intended files, with no binary/`.png`, no `Localization/**`, and `parse-design-xml.ps1` unchanged.
- **Files modified:** none (measurement command only)
- **Verification:** Against `8ed6f5862`: `BOM_OK`, `BIN_OK`, `PARSER_UNCHANGED`; plan-07 delta = `check-armofgianttree-charge.ps1`, `ArmOfGiantTree.cs`, `KelpCurtainPlayer.cs`, `ArmOfGiantTreeChargePacket.cs`.
- **Committed in:** documented in `06d8cf0f4` (Task 3)

---

**Total deviations:** 2 auto-fixed (1 bug, 1 blocking)
**Impact on plan:** Both fixes were necessary — the send signature correction was required to compile; the base correction was required to obtain a meaningful BOM/no-art result. No scope creep; no source behavior beyond the plan.

## Issues Encountered

- The plan's Task 1 and Task 2 gates are cumulative (Task 2 extends the Task 1 gate). The WR-01 server-authority behavior had to be authored in the Task 1 pass so the integrated Release build compiled; Task 2 then made the dedicated-server branch explicit and documented `ReleaseSmash`, extending the gate checks.

## User Setup Required

None - no external service configuration required.

## Verification Evidence

| Check | Result |
| --- | --- |
| `dotnet build /p:Configuration=Release /p=WarningLevel=0` | exit 0, `.tmod` packaged |
| `check-armofgianttree-charge.ps1` | `OK(0): ArmOfGiantTree charge is per-player + per-stack slot-keyed, synced, and server-authoritative` |
| `check-carryover.ps1` | `OK(0): carry-over covered entries = 5 (of 5 selected)` |
| `check-tranche-A.ps1` | `OK(0): tranche-A covered entries = 43 (of 43 selected)` |
| `check-tranche-B.ps1` | `OK(0): tranche-B covered entries = 20 (of 20 selected)` |
| `check-inventory-reconciliation.ps1` | `OK(0): 103 entries; matched=65; ... labels=5 deferred=3 assumptions=7` |
| `validate-inventory.ps1` | `OK(0): 103 entries (84 weapons) - green=50 yellow=8 unchecked=45` |
| UTF-8 BOM check (base `8ed6f5862`) | `BOM_OK` |
| No `.png`/binary under KelpCurtain (base `8ed6f5862`) | `BIN_OK` |
| `parse-design-xml.ps1` unchanged | `PARSER_UNCHANGED` (not re-run) |

**Runtime behavior not verified locally:** the client-to-server charge sync, the multiplayer server-authoritative shockwave and its NPC propagation, and the two-client charge isolation require a live tModLoader client/server session (see coverage D2/D3).

## Known Stubs

None - the rework wires the charge, sync, and authoritative shockwave end-to-end; no placeholder values or unwired data sources were introduced.

## Threat Flags

None - no new network endpoint, file access pattern, or schema change beyond the plan's `<threat_model>`.

## Next Phase Readiness

- CR-01, WR-01 and WR-02 are closed by source changes; roadmap SC3 is satisfied for `ArmOfGiantTree`.
- All previously green Phase 1 gates remain green; no art, localization, or Feishu design file changed (QUAL-05).
- Outstanding for later phases: the 18 deferred localization entries (Phase 2/8), the recorded artwork/effect blockers (Phase 6/7/8), and Green Tundra label resolution (Phase 6).

## Self-Check: PASSED

- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs`
- FOUND: `Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs`
- FOUND: `.planning/phases/01-item-inventory-completed-art-items/scripts/check-armofgianttree-charge.ps1`
- FOUND: `.planning/phases/01-item-inventory-completed-art-items/01-07-SUMMARY.md`
- FOUND commit: `5ca103693`
- FOUND commit: `53a5eddaa`
- FOUND commit: `06d8cf0f4`

---
*Phase: 01-item-inventory-completed-art-items*
*Completed: 2026-09-13*
