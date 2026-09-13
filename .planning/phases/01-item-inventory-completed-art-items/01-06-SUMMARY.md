---
phase: 01-item-inventory-completed-art-items
plan: 06
subsystem: content-items
tags: [feishu, inventory, kelp-curtain, items, carry-over, allocation-correction, artwork-blocker, deferral, qual-05, phase-gate]

# Dependency graph
requires:
  - plan: 01-05
    provides: "The consolidated deviation ledger, the deferred-localization directive record (P1A-11), and the Phase 1 gate suite (reconciliation, tranche-A/B, localization coverage)"
  - plan: 01-03
    provides: "The class-less Phase 2 routing record (P1A-03, superseded here) and the check-tranche-A.ps1 gate template"
provides:
  - "Corrected 18-entry completed-art allocation: 5 Phase 1 carry-over / 13 Phase 7; phase counts {1: 65, 2: 25, 7: 13}"
  - "Five implemented Phase 1 carry-over item classes: ArmOfGiantTree, ElftigernPowder, ForestBreath, WitheredMask, QuetzalsWish"
  - "scripts/check-carryover.ps1 - class-or-recorded-artwork-blocker gate with the no-placeholder .png guard (5/5 covered)"
  - "check-inventory-reconciliation.ps1 extended for the D-06 recorded-artwork-blocker repo_asset exception"
  - "P1A-12 (allocation correction) and P1A-13 (deferred-localization extension) inventory actions"
affects: [02, 07, 08]

# Actuals (#2632) - pairs with the plan's estimate (tokens 48000, tasks 3, confidence low).
actuals:
  tokens: 26500       # chars/4 over the realized diff (106000 chars; 867 insertions / 108 deletions across 12 files)
  tasks: 3
  commits: 3          # MEASURED: git rev-list --count ${plan_head_before}..HEAD
  plan_head_before: b8fc5c5834d53289f7bd635b3bf6973ac4c745d9

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Two-step allocation: design section -> phase (boss/encounter rows belong to Phase 7 via ITEM-05/ITEM-06), then Feishu artwork state -> Phase 1/2 within the item phases; repository class presence never allocates"
    - "A class whose approved texture is absent reuses the existing shared Commons.ModAsset.White_Mod fallback (the RadialCarapace precedent) so the mod still loads, and carries a named texture/artwork blocker; no placeholder art is created and no .png is added"
    - "The reconciliation gate honors a recorded artwork blocker (blocker matching texture|artwork) instead of failing an artwork-complete class-less entry, preserving the no-silent-gap intent"
    - "User-directed localization deferral is extended to new entries as status=deferred deviations (P1A-13), never localization.blocked and never a fabricated key"

key-files:
  created:
    - .planning/phases/01-item-inventory-completed-art-items/scripts/check-carryover.ps1
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/ElftigernPowder.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/WitheredMask.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - .planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md
    - .planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1
    - .planning/ROADMAP.md

key-decisions:
  - "The 13 boss/special-encounter rows (9 Giant Winged Dragon -> ITEM-06, 4 Klein Snake -> ITEM-05) are Phase 7 item scope, not Phase 1/2; only the 5 non-boss item-table rows are Phase 1 carry-over. P1A-03 is superseded by P1A-12."
  - "The three carry-over entries with no repository texture keep code implemented + `repo_asset` empty and carry the named artwork blocker; the existing shared White_Mod fallback is reused so the mod loads, and no art is fabricated."
  - "Localization stays deferred by the 2026-09-12 user directive: the five new entries are mirrored as status=deferred deviations (P1A-13); the strict coverage gate stays honestly red and -AllowMissing is the evidence."
  - "`scripts/parse-design-xml.ps1` was not re-run (CR-01); the inventory was edited in place. The parser file is unchanged vs HEAD."

requirements-completed: [QUAL-05]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "The 18 artwork-complete class-less entries are correctly allocated 5 Phase 1 / 13 Phase 7; phase counts {1: 65, 2: 25, 7: 13}; P1A-12 records the two-step rule and P1A-03 is marked superseded."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "check-inventory-reconciliation.ps1 -> OK(0): 103 entries; matched=65; deferred=3; labels=5 assumptions=7"
        status: pass
    human_judgment: false
  - id: D2
    description: "Five Phase 1 carry-over item classes exist and compile (ArmOfGiantTree charge smash, ElftigernPowder material, ForestBreath quest, WitheredMask vanity, QuetzalsWish melee giant blade)."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 warnings, 0 errors, Everglow.tmod packaged"
        status: pass
      - kind: automated
        ref: "scripts/check-carryover.ps1 -> OK(0): carry-over covered entries = 5 (of 5 selected)"
        status: pass
    human_judgment: false
  - id: D3
    description: "check-inventory-reconciliation.ps1 accepts the D-06 recorded-artwork-blocker path (item + internal_name + empty repo_asset + artwork_complete true passes only with a texture|artwork blocker), and still reports 103 entries / 103 matrix rows."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "scripts/check-inventory-reconciliation.ps1 -> exit 0; matrix rows == JSON entries (103)"
        status: pass
    human_judgment: false
  - id: D4
    description: "Localization remains deferred by user directive: the five new entries are added to deviations[] and P1A-13; no HJSON key was created or edited and localization.blocked stays false."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "scripts/check-localization-coverage.ps1 -AllowMissing -> MISSING(18) of 63 selected (45 covered), exit 0"
        status: pass
      - kind: automated
        ref: "git status shows no Localization/**/*.hjson change in this plan"
        status: pass
    human_judgment: false
  - id: D5
    description: "Final Phase 1 gates pass after the carry-over: validate-inventory 103, reconciliation 103, tranche-A 43/43, tranche-B 20/20, carry-over 5/5, Release build 0 errors, byte-level BOM check, and no added/modified .png under the items tree."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "validate-inventory.ps1 OK(0) 103; check-tranche-A.ps1 OK(0) 43/43; check-tranche-B.ps1 OK(0) 20/20; build 0 errors; AGENTS.md BOM check passed (1307 files); 0 phase binary changes"
        status: pass
    human_judgment: false
  - id: D6
    description: "Judgment call: the three texture-less carry-over classes reuse the shared Commons.ModAsset.White_Mod fallback rather than approved art, and record an artwork/effect blocker; the plan's backstop truth (client obtains and uses the items once Phase 6 loot tables exist) is not runtime-verified here."
    requirement: QUAL-05
    verification: []
    human_judgment: true
    rationale: "Whether the code-complete / artwork-missing disposition (with the shared fallback texture) is acceptable, versus blocking these three entries entirely, is a user/verifier judgment. In-client obtain/use behavior is a backstop truth that requires a live tModLoader session not performed by this plan."

# Metrics
duration: ~30min
completed: 2026-09-13
status: complete
---

# Phase 1 Plan 06: Carry-Over Allocation Correction and Phase 1 Slice Summary

**Corrected the 18 completed-art class-less entries to 5 Phase 1 carry-over / 13 Phase 7 encounter rewards, implemented the five carry-over item classes (reusing two orphan textures and blocking three missing ones), and gated the lot with a new `check-carryover.ps1` (5/5) plus a green Release build — all with localization still deferred by user directive.**

## Performance

- **Duration:** ~30 min
- **Started:** 2026-09-13 (session start)
- **Completed:** 2026-09-13
- **Tasks:** 3
- **Files modified:** 12 (6 created, 6 modified)
- **Commits:** 3

## Accomplishments

- **Allocation corrected (QUAL-05).** The 18 artwork-complete class-less entries were reallocated by the two-step rule: design section → phase, then Feishu artwork state → Phase 1/2 within the item phases. Result: **5 Phase 1 carry-over** (Arm of Giant Tree, 森林之息, 厄佛提根的净化粉末, 枯萎面具, 魁札尔的愿望) and **13 Phase 7** encounter rewards (9 Giant Winged Dragon rows → ITEM-06, 4 Klein Snake rows → ITEM-05). Phase counts are now `{1: 65, 2: 25, 7: 13}` (was `{1: 60, 2: 43}`); total stays 103. `P1A-12` records the correction and supersedes `P1A-03`. The destructive `scripts/parse-design-xml.ps1` was **not** re-run (CR-01) and is unchanged vs HEAD.
- **Five carry-over classes implemented.** `ArmOfGiantTree` (melee smash: 55 damage, 7.25 knockback, 45/45 with a 30-frame retract lockout, 2 gold, Orange; holds a charge to 2.5 s for 200% damage + a 100% shockwave, scaling 75%→200%, right-click normal swing), `ElftigernPowder` (material, 25 silver, Blue), `ForestBreath` (quest item, 50 silver, Blue), `WitheredMask` (vanity, 15 silver, Blue, no combat stats), and `QuetzalsWish` (melee giant blade, 37 damage, 8% crit, 10 gold, Pink).
- **No placeholder art.** `ArmOfGiantTree` and `ElftigernPowder` consume the pre-existing orphan textures beside them. `ForestBreath`, `WitheredMask` and `QuetzalsWish` have no repository texture: they carry the named blocker "approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending", reuse the existing shared `Commons.ModAsset.White_Mod` fallback so the mod still loads (the `RadialCarapace` precedent), and set `repo_asset` empty. **No `.png`/binary asset was added or modified.**
- **Gates.** New `scripts/check-carryover.ps1` (class-or-recorded-artwork-blocker, 100% ASCII, no-placeholder guard) reports **5/5**; `check-tranche-A.ps1` rose 38→**43/43**; `check-inventory-reconciliation.ps1` reports **103 entries, matched=65, deferred=3**; `validate-inventory.ps1` 103; `check-tranche-B.ps1` 20/20; `dotnet build /p:Configuration=Release /p:WarningLevel=0` → **0 warnings, 0 errors**; byte-level BOM check passed; 0 binary changes.
- **Localization deferral extended.** Per the 2026-09-12 user directive, no exporter run and no HJSON edit. The five new entries are recorded as `status: deferred` deviations and `P1A-13`; `check-localization-coverage.ps1 -AllowMissing` now selects **63** entries (**45 covered / 18 missing**, exit 0). The strict gate stays red by design.

## Task Commits

Each task was committed atomically:

1. **Task 1: Reconcile the 18-entry allocation in the inventory, ledger, and reconciliation gate** — `2c8a184d9` (docs)
2. **Task 2: Implement the five Phase 1 carry-over item classes and add the carry-over coverage gate** — `a80773162` (feat)
3. **Task 3: Record the carry-over localization deferral and pass the final Phase 1 gates** — `835483e05` (docs)

**Plan metadata:** committed separately (docs: complete plan 01-06).

## Files Created/Modified

- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-carryover.ps1` (new) — selects `phase==1 && carry_over==true`, requires a tracked class or a `texture|artwork` blocker, and guards against added/modified `.png` under the items tree.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1` (modified) — D-06 exception: an item entry with a non-empty `internal_name`, empty `repo_asset` and `artwork_complete == true` now passes only when a blocker matches `texture|artwork`.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs` (new) — melee charge smash + 100% shockwave, sibling texture reused.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/ElftigernPowder.cs` (new) — material identity; purification gated off until GAME-03/Phase 6.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs` (new) — quest item identity; artwork blocker.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/WitheredMask.cs` (new) — vanity identity; artwork blocker.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs` (new) — melee giant-blade identity; artwork + effect blockers; design/parser category discrepancy recorded.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` (modified) — 18-entry reallocation, `carry_over` flag, `internal_name`/`repo_asset`/`blockers`/`notes` for the five, `P1A-12`/`P1A-13`, one category deviation and five localization-deferral deviations.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` (modified) — header allocation-correction note; five Blockers cells refreshed; still 103 matrix rows.
- `.planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md` (modified) — §Allocation Correction, the parser category discrepancy, the five deferred-localization rows, and §Carry-Over Gate.
- `.planning/ROADMAP.md` (modified) — Phase 1 cross-cutting constraint, Phase 2 allocation note and Phase 7 carry-over note record the 5/13 split.

## Decisions Made

- **Two-step allocation, P1A-12 over P1A-03:** the retired "class-less → Phase 2" rule is superseded; the 13 boss/encounter rows belong to Phase 7 (ITEM-06/ITEM-05) and only 5 rows are Phase 1 carry-over.
- **Class-or-blocker over fabrication:** the three texture-less classes reuse the existing shared fallback texture and carry a named artwork blocker rather than fabricate art or leave a class that would fail content load.
- **Deferral over blocking:** localization is recorded as `status: deferred` (P1A-13) matching P1A-11, so the strict coverage gate stays honestly red and `-AllowMissing` is the Phase 1 evidence.
- **JSON edited in place:** `parse-design-xml.ps1` was not re-run (CR-01); the correction was applied with structured in-place edits preserving the existing serialization.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Texture-less carry-over classes would fail content load**
- **Found during:** Task 2 (implement the five carry-over classes)
- **Issue:** A `ModItem` with no matching `.png` and no `Texture` override throws a missing-resource error when tModLoader loads content (every other `ModItem` in the Kelp Curtain tree has either a `.png` or a `Texture` override). Implementing `ForestBreath`, `WitheredMask` and `QuetzalsWish` as bare classes would have broken the mod load.
- **Fix:** Added `public override string Texture => Commons.ModAsset.White_Mod;` (existing shared asset; the `RadialCarapace` precedent) to the three classes while still recording the approved-art blocker and leaving `repo_asset` empty. No art was created or modified.
- **Files modified:** `ForestBreath.cs`, `WitheredMask.cs`, `QuetzalsWish.cs`
- **Verification:** Release build 0 errors; no `.png` added/modified; `check-carryover.ps1` and `check-tranche-A.ps1` pass.
- **Committed in:** `a80773162`

**2. [Rule 2 - Missing Critical] Consumable powder could be consumed with no effect**
- **Found during:** Task 2 (ElftigernPowder)
- **Issue:** The designed purification effect lives in the Wilted Zone restoration system (GAME-03/Phase 6), which does not exist yet. A `consumable` item with no `UseItem` hook would delete itself for no benefit.
- **Fix:** Added `public override bool CanUseItem(Player player) => false;` so the item exists and is visible but cannot be consumed until the purification target exists; recorded the dependency blocker.
- **Files modified:** `ElftigernPowder.cs`, `01-INVENTORY.json`
- **Verification:** Release build 0 errors; `check-inventory-reconciliation.ps1` exit 0.
- **Committed in:** `a80773162`

---

**Total deviations:** 2 auto-fixed (1 Rule 1, 1 Rule 2).
**Impact on plan:** Both are required for a loadable, non-destructive implementation; no scope creep. The artwork/effect blockers keep the three texture-less entries honest instead of claiming false parity.

## Known Stubs

| Stub | File | Reason |
|------|------|--------|
| `ForestBreath`, `WitheredMask`, `QuetzalsWish` reuse the shared `Commons.ModAsset.White_Mod` fallback texture instead of approved item art | the three `.cs` classes | Approved texture missing from the repository despite the Feishu artwork checkbox; recorded as artwork blockers in `01-INVENTORY.json`. No placeholder art was created; resolve when the approved art lands. |
| `ElftigernPowder` purification effect is not implemented | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/ElftigernPowder.cs` | Requires the Wilted Zone restoration system (GAME-03/Phase 6); `CanUseItem` is gated off so the powder is not consumed effectless. |
| `QuetzalsWish` four-stage combo, charged right-click throw/explosion and wound debuff are not implemented | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs` | Need new projectile/buff/VFX assets that are absent; recorded as an effect blocker. |
| 18 Phase 1 completed-art items lack display keys in both `en-US` and `zh-Hans` (13 from plan 01-05 + the 5 carry-over: `ArmOfGiantTree`, `ForestBreath`, `ElftigernPowder`, `WitheredMask`, `QuetzalsWish`) | `Sources/Everglow/Localization/{en-US,zh-Hans}/Mods.Everglow.Items.*.hjson` (keys absent) | Localization deferred by user directive (2026-09-12). Recorded as `deferred` deviations (P1A-11/P1A-13); the in-game exporter was not run and no key was fabricated. |

## Threat Flags

| Flag | File | Description |
|------|------|-------------|
| threat_flag: none | - | No new network endpoint, auth path, file-access pattern, or trust-boundary schema change. The only new executable is a read-only PowerShell gate; no Feishu document was mutated, no key was fabricated, and no binary/art asset was added or modified. |

## Issues Encountered

- **New classes were untracked when the carry-over gate first ran.** `check-carryover.ps1` (like `check-tranche-A.ps1`) resolves classes through `git ls-files`, so the freshly created `.cs` files were invisible until staged. Staged the five classes before running the gate; it then reported 5/5. This matches the gates' "git-tracked" contract.
- **Windows console mojibake.** CJK entry ids render as the ANSI code page in terminal output; the JSON was read with `[IO.File]::ReadAllText` (UTF-8) and parsed correctly, and the gates report the correct counts.

## User Setup Required

None — no external service, package, or secret. No Feishu write was made.

## Next Phase Readiness

- **Ready:** Phase 1 carry-over is gated — allocation corrected, the five items implemented with recorded artwork/effect blockers, localization deferral recorded, and every runnable gate plus the Release build green.
- **Phase 7 hand-off:** 13 entries are `phase = 7` with their ITEM-05/ITEM-06 advancement appended; they are not Phase 1/2 scope.
- **Phase 2 hand-off:** the 25 remaining `phase = 2` entries are artwork-incomplete; `RadialCarapace`/`VineRepairWand` stay Phase 1 tracked blockers per D-11.
- **Not verified locally:** in-client obtain/use of the five carry-over items (the plan's backstop truth) — no live tModLoader session was performed; the three texture-less items display the shared fallback texture until approved art exists.

---

*Phase: 01-item-inventory-completed-art-items*
*Completed: 2026-09-13*

## Self-Check: PASSED

- Declared files exist: `01-06-SUMMARY.md`, `scripts/check-carryover.ps1`, and all five carry-over `.cs` classes.
- Task commits exist: `2c8a184d9`, `a80773162`, `835483e05`.
- Gates green: `validate-inventory.ps1` (103), `check-inventory-reconciliation.ps1` (103, deferred=3), `check-tranche-A.ps1` (43/43), `check-tranche-B.ps1` (20/20), `check-carryover.ps1` (5/5), `check-localization-coverage.ps1 -AllowMissing` (63 selected / 45 covered / 18 missing, exit 0), Release build 0 errors, byte-level BOM check passed, 0 added/modified `.png`.
