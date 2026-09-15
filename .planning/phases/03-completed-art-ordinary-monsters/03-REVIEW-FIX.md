---
phase: 03
fixed_at: 2026-09-15T09:40:01Z
review_path: .planning/phases/03-completed-art-ordinary-monsters/03-REVIEW.md
iteration: 1
findings_in_scope: 5
fixed: 5
skipped: 0
status: all_fixed
---

# Phase 03: Code Review Fix Report

**Fixed at:** 2026-09-15
**Source review:** `.planning/phases/03-completed-art-ordinary-monsters/03-REVIEW.md`
**Iteration:** 1

**Summary:**
- Findings in scope: 5 (WR-01, WR-02, WR-03, WR-04, WR-05 — Critical 0, Warning 5)
- Fixed: 5
- Skipped: 0
- Info findings intentionally left unresolved: IN-01, IN-02, IN-03, IN-04, IN-05, IN-06

Each Warning was re-verified against the committed design snapshot
`.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` before editing
(the 荆棘苔龟 stats table `Cqvwdoc54o8vsWx3mIpcoKpinVg` and the 巨树人 heading
`Pm9jdOm1Zo9Tc4xy4lncGRtbnJh` / behaviour block `Zm3ZdguPboviV1xCmcmcSIEznZd`).

## Fixed Issues

### WR-01: Tortoise defence 20 was never applied while the shell was retracted (缩壳)

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs`
**Commit:** `5183f8bcb`
**Applied fix:** `PostAI` now has a third branch for `TortoiseState.Retracting` that keeps the normal
contact damage (`defDamage` = 50) but applies the design's doubled defence (`defDefense * 2` = 20),
so the 防御 10/20 pair is reachable in both designed states instead of only during the spin. The spin
branch still applies 75/20. The class doc comment was rewritten to state the three-state mapping and
why 缩壳 does not raise damage (the design's 伤害 cell brackets only 旋转). The later
`ModifyIncomingHit` override is untouched (IN-04 is out of scope).

### WR-02: GiantDandelion re-armed the mid-range cooldown from the post-smash throw

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs`
**Commit:** `b2f74f5b2`
**Applied fix:** `ThrowBoulder` now takes a `bool armMidRangeCooldown` parameter (with an XML
`<param>`). The `BoulderThrow` state calls it with `armMidRangeCooldown: true`; `ExitSmashRecovery`
calls it with `armMidRangeCooldown: false`. The 240-tick 至少240帧的间隔 is therefore armed only by
the mid-range state's own attack, so a close-range smash no longer suppresses the next mid-range
boulder. Doc comment updated to scope the gap to the mid-range state.

### WR-03: The no-aggro Idle state was unreachable while any player was alive

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs`
**Commit:** `0d4cc4ea8`
**Applied fix:** Added `AggroRangeTiles = 30f` (conservative default; design row silent, recorded in
`03-DEVIATIONS.md` §4) and a gate in `RunStateMachine` that returns to (or stays in) the no-aggro
Idle wander when the current target is farther than the radius. The gate covers only the three
locomotion states (`Idle`, `Approach`, `Chase`), so a committed smash or mid-range attack always
completes. This makes the designed state (0) reachable and stops the creature chasing across the
whole subworld, while keeping the 5-tile smash, 5-15 tile mid-range and beyond-15-tile chase all
reachable (the radius is wider than the 15-tile chase threshold).

### WR-04: The floor-anchored shockwave sampled only one column for a 200 px hit box

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs`
**Commit:** `fe20e019d`
**Applied fix:** Replaced the single centre-column probe with `FindFloorTileY()`, which samples the
leading edge, the centre and the trailing edge (leading/trailing chosen by travel direction) and
returns the deepest floor tile found; `ProbeFloorTileY(int, int)` is a small `-1`/value helper. The
wave anchors to that tile's top edge, so an isolated hole under one column no longer drops the whole
bar and a higher terrace caught by the centre no longer lifts it off the ground its leading half is
over. The `200x32` size remains the D-34 default.

### WR-05: `IsKelpCurtainLayer` did not reproduce the biome's vertical band

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs`
**Commit:** `49db5a865`
**Applied fix:** Reconciled the band semantics by correcting the helper's documentation to state the
actual server-safe intent: the predicate is anchored to the synced player centre and is deliberately
about half a screen below the camera-driven `IsBiomeActive` band, because `Main.screenPosition` /
`Main.screenHeight` are client-only and zero on a dedicated server. The predicate still uses
`player.Center.Y` and never `Main.screenPosition`, so it stays server-safe; no band bound changed and
`IsBiomeActive` was left untouched (it drives the background/lighting scene transitions). The same
clarification is recorded in `03-DEVIATIONS.md` §4 and §6.

## Intentionally Unresolved

The six Info findings (IN-01..IN-06) are advisory and out of the requested fix scope (Critical +
Warning):

- **IN-01** `GiantDandelion.EnterState` sets `netUpdate` even when the state does not change. Partially
  mitigated by the WR-03 fix: the no-aggro Idle path now returns without calling `EnterState` each
  tick, so the repeated-path churn is gone; the remaining `EnterState` calls stay per-transition.
- **IN-02** Duplicated death-dust block across the four NPC classes (extraction refactor).
- **IN-03** Inline defence literals 10/20 in `GuppyConch`.
- **IN-04** Empty `MossyThornTurtle.ModifyIncomingHit` override.
- **IN-05** `VerdantRods.TargetPos` public mutable field.
- **IN-06** `GiantDandelion`'s `214x263` collision box spawn-area risk (already tracked in
  `03-UAT.md` check 3(a)).

Each is documented in `03-REVIEW.md` and remains for a future pass.

## Verification

- **Per-fix:** each edited file section was re-read after the edit (Tier 1); text-format constraints
  (tabs, LF only, UTF-8 without BOM, trailing newline) were verified byte-level for all four changed
  source files.
- **Build:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` -> exit 0, `0 个警告` / `0 个错误`
  (0 warnings, 0 errors); `Everglow.tmod` produced.
- **Gate:** `check-biology.ps1 -RequireAll` -> exit 0,
  `OK(0): phase3 tranche = 5 / 5 (rows=31)`, `OK: implemented classes = 5 / 5`.
- **Where the gates ran:** the **main checkout**, not an isolated worktree. As recorded in the Phase 2
  fix report (`02-REVIEW-FIX.md`), a worktree nested under `.claude/worktrees/` cannot locate
  `tModLoader.targets` because the ModBuilder searches a fixed parent depth, so the documented
  fallback (verify from the main checkout) applies. The fix commits are ordinary commits on the
  current branch tip; nothing was fast-forwarded or left pending. `dotnet build-server shutdown` was
  run after the final build batch.

---

_Fixed: 2026-09-15_
_Fixer: the agent (gsd-code-fixer)_
_Iteration: 1_
