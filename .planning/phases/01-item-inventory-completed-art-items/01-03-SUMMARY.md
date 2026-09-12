---
phase: 01-item-inventory-completed-art-items
plan: 03
subsystem: content-items
tags: [feishu, inventory, kelp-curtain, items, design-parity, d-11, d-12, phase-2-routing, coverage-gate, replan]

# Dependency graph
requires:
  - plan: 01-02
    provides: "Reconciled 01-INVENTORY.json (internal_name, repo_asset, localization, tranche, advances, blockers), the five-label taxonomy, and check-inventory-reconciliation.ps1"
  - plan: "01-03 (prior halted run)"
    provides: "Numeric design parity for 18 tranche-A classes and the two LocalizationCategory overrides, committed at 261996ecf"
provides:
  - "Effect/recipe/set-bonus reconciliation for the 38 completed-art tranche-A entries: 7 recipes verified exact, 34 effects matched against the wired projectile/buff, 4 effect blockers recorded"
  - "ThornTurtleShell run-speed reduction corrected to the design -10% (was -5%)"
  - "Phase 2 routing on all 43 class-less entries (phase=2); the 18 completed-art entries additionally carry deferred=true with a reason"
  - "scripts/check-tranche-A.ps1 - tranche-A coverage and no-placeholder gate (38 covered entries)"
affects: [01-04, 01-05, 02]

# Actuals (#2632) - pairs with the plan's estimate (tokens 70000).
actuals:
  tokens: 8980        # chars/4 over the realized diff (219 insertions / 84 deletions across 4 files)
  tasks: 3
  commits: 3          # MEASURED: git rev-list --count ${plan_head_before}..HEAD
  plan_head_before: 266ff85fcfc3a7ed3864d72d7ae55d86aa9047db

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Prose 效果 cells are reconciled by reading the wired projectile/buff: a match is only claimed when the implementing code was inspected; otherwise the entry records an 效果 blocker instead of a false parity claim"
    - "Phase-scoped coverage gate: selecting by phase==1 plus tranche=='A' plus artwork_complete==true makes Phase 2 routing invisible to the tranche gate while still catching any un-implemented, un-blocked Phase 1 entry"

key-files:
  created:
    - .planning/phases/01-item-inventory-completed-art-items/scripts/check-tranche-A.ps1
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/ThornTurtleShell.cs

key-decisions:
  - "The replan resumed from the committed numeric pass (261996ecf) and completed the D-12 effect/recipe/set-bonus remainder; the numeric columns were not re-touched (D-10)."
  - "Four tranche-A weapons carry 效果 blockers (MossySpell, CyatheaArrow, GreenSungloStaff, EvilHalbertBarnacle): their design behavior is implemented in projectiles outside this task's modify set and either differs from the design or cannot be confirmed mechanically."
  - "All seven tranche-A recipes already match the design rows exactly (RedAlgaeMagicStaff/Gyroscope/SpellBook/Whip/SummonStaff, DevilHeartIronBar, MossyCyatheaBow); no recipe edits were required."
  - "All 43 class-less entries are routed to Phase 2; the 18 completed-art ones also carry deferred=true with a reason naming the missing repository target and the false Feishu design-code checkbox."
  - "RadialCarapace and VineRepairWand remain phase 1 per D-11 (code-complete but artwork-missing still counts toward Phase 1) with their deviations queued to Phase 2 per D-12."
  - "The plan ledger base was reset to the spawn-time HEAD (266ff85fc); the previous halted run's stale ledger (35ccd096c) would have double-counted 261996ecf and the intermediate docs commits."

requirements-completed: [QUAL-05]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "The 38 completed-art tranche-A entries have effect/recipe/set-bonus parity: 7 recipes verified exact against their design rows, 34 effects matched against the wired projectile/buff, and 4 entries carry a recorded 效果 blocker naming the unreconciled field."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "powershell -File scripts/check-inventory-reconciliation.ps1 -> OK(0): 103 entries; validate-inventory.ps1 -> OK(0); dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 errors"
        status: pass
    human_judgment: false
  - id: D2
    description: "All 43 class-less entries are routed to Phase 2 (phase=2); the 18 completed-art class-less entries additionally carry deferred=true and a non-empty reason; artwork/code/status/tranche/advances/blockers are unchanged."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "check-inventory-reconciliation.ps1 -> OK(0): 103 entries; deferred=21; phase counts {1:60, 2:43}"
        status: pass
    human_judgment: false
  - id: D3
    description: "scripts/check-tranche-A.ps1 selects phase==1 + tranche=='A' + artwork_complete==true and passes with 38 covered entries; it also guards against added/modified .png under the Kelp Curtain items tree."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "powershell -File scripts/check-tranche-A.ps1 -> OK(0): tranche-A covered entries = 38 (of 38 selected)"
        status: pass
    human_judgment: false
  - id: D4
    description: "ThornTurtleShell's run-speed reduction matches the design -10% (was -5%); no other material/accessory class deviated."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "git diff ThornTurtleShell.cs; dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 errors; AGENTS.md UTF-8 BOM check passed (1273 files)"
        status: pass
    human_judgment: false
  - id: D5
    description: "Judgment call: four prose 效果 cells were recorded as blockers rather than force-implemented in projectile files outside the task's modify set."
    requirement: QUAL-05
    verification: []
    human_judgment: true
    rationale: "Whether to accept the blocker disposition (versus expanding this plan's file scope into projectiles) is a scoping judgment for the verifier/user."

# Metrics
duration: ~18min
completed: 2026-09-12
status: complete
---

# Phase 1 Plan 03: Completed-Art Tranche A Parity and Class-less Routing Summary

**Resumed the committed numeric pass and completed the D-12 remainder: 7 recipes verified exact, 34 effects matched to their projectiles/buffs, 4 effect blockers recorded, ThornTurtleShell brought to the design -10%, all 43 class-less entries routed to Phase 2 (18 deferred), and a 38-entry tranche-A coverage gate added.**

## Performance

- **Duration:** ~18 min (from the spawn-time ledger base 266ff85fc to the last task commit)
- **Tasks:** 3
- **Files modified:** 4 (1 created, 3 modified)
- **Commits:** 3

## Accomplishments

- **Effect reconciliation (D-12):** for each of the 38 completed-art tranche-A entries the design `效果` cell was read from the committed header-anchored rows and compared against the item class and the projectile/buff it wires. 34 matched (e.g. `GreenVineWhip` -> `GreenVineWhip_proj` applies MossCover 300 frames + 25% poison; `DevilHeartStaff` -> `proj_II` 250% on the 8th shot and `proj_Kill` 110-radius AOE + 300% necrosis + 3s Confused; `EmptyWaterStaff` -> bubble traps 180 frames / 360 when wet; `LegumeGyroscope` -> 1-3 seeds on whip hit; `ArcI` -> 5-shot power bar and 3 stored arcs).
- **Recipe reconciliation:** all seven `AddRecipes` classes match their design rows exactly (`RedAlgaeMagicStaff` 12 + 1, `RedAlgaeMinionGyroscope` 9 + 1, `RedAlgaeMagicSpellBook` 18 + 1, `RedAlgaeMagicWhip` 15 + 1, `RedAlgaeMinionStaff` 15 + 1 at Work Benches; `DevilHeartIronBar_Item` ore x3 + Jadeized Bone x1 at a Furnace; `MossyCyatheaBow` has a commented-out recipe and a blank design row, so no change). Set bonuses: none of the 26 weapon/ammo classes participates in an armor set, so no `套装效果` work applied.
- **Four effect blockers recorded** (Rule 2/3 scope honesty, not silent acceptance): `MossySpell` (design 25% poison for 5s absent from `MossySpell_proj`), `CyatheaArrow` (design spike damage 1 vs `Projectile.damage/2`), `GreenSungloStaff` (design thorn lifetime 20s not confirmed against `GreenSungloThorns`), `EvilHalbertBarnacle` (design right-click 60% panel damage / 16 defense-pierce not confirmed in the shuttle). Each names the unreconciled field (`效果`).
- **One code parity fix:** `ThornTurtleShell` `maxRunSpeed` reduction corrected from 5% to the design 10% (`-7%` move speed and `+5%` damage reduction already matched).
- **Phase 2 routing:** all 43 class-less entries (empty `internal_name` + `no repo implementation found`) now carry `phase = 2`; the 18 completed-art ones also carry `deferred = true` with the reason "no repository implementation target and Feishu design-code checkbox false; routed to Phase 2 (remaining non-boss item scope)". No `artwork_complete`, `code_complete`, `status`, `tranche`, `advances`, or `blockers` value was changed.
- **Phase 1 tracked blockers preserved:** `RadialCarapace` and `VineRepairWand` stay `phase = 1` (code-complete, artwork-missing per D-11), with their deviations queued to Phase 2 per D-12; `01-INVENTORY.md` records both dispositions.
- **Tranche-A coverage gate:** new `scripts/check-tranche-A.ps1` (ASCII-only, `[IO.File]::ReadAllText`) selects `phase == 1` + `tranche == "A"` + `artwork_complete == true`, requires a tracked `.cs` class or a non-no-repo blocker per entry, and fails on any added/modified `.png` under the items tree. It reports 38 covered entries and exits 0.
- **Verification:** `check-inventory-reconciliation.ps1` and `validate-inventory.ps1` exit 0 (103 entries; matched=60; green=58 yellow=20 unchecked=25; labels=5; deferred=21; assumptions=7); `dotnet build /p:Configuration=Release /p:WarningLevel=0` -> 0 warnings, 0 errors; AGENTS.md byte-level UTF-8 BOM check passed (1273 files).

## Task Commits

Each task was committed atomically:

1. **Task 1: Complete tranche-A weapon/ammo completed-art design parity (D-12)** - `3a339daf0` (docs)
2. **Task 2: Complete tranche-A material/accessory parity and route class-less entries to Phase 2** - `8439cdb31` (feat)
3. **Task 3: Gate tranche A with a coverage and no-placeholder check** - `d2bd94c11` (feat)

**Plan metadata:** (docs: complete plan - committed separately)

## Files Created/Modified

- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` - four `效果` blockers added; `phase = 2` on the 43 class-less entries; `deferred`/`deferred_reason` on the 18 completed-art class-less entries; all other fields preserved.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` - Deferred & Flagged section now lists the 18 routed entries, states that all class-less entries are Phase 2 scope, and records the two Phase 1 tracked blockers; 103 matrix rows and the label table are unchanged.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/ThornTurtleShell.cs` - run-speed reduction 5% -> 10% to match the design row.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-tranche-A.ps1` (new) - tranche-A coverage and no-placeholder gate.

## Decisions Made

- **Effect blocker disposition:** a design prose effect is claimed as matched only when the implementing projectile/buff was inspected. Four entries could not be confirmed (or differ) and therefore carry an `效果` blocker rather than a false parity claim; fixing them would require editing projectile files outside this task's declared set.
- **Routing semantics:** `phase = 2` is the routing field; `deferred = true` + reason is reserved for the 18 artwork-complete entries (undefined-future/blocked implementation), while the 25 artwork-incomplete entries keep their artwork/no-repo blockers and an unset `deferred` flag.
- **D-11 discipline:** the two code-complete, artwork-missing repo entries are explicitly kept in Phase 1 rather than routed; their deviations are queued to Phase 2.
- **Ledger base:** reset `gsd-plan-head-before-01-03` to the spawn-time HEAD so this run's actuals measure only its own three commits.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Stale plan ledger from the halted run**
- **Found during:** Task 1 (pre-commit ledger setup)
- **Issue:** `gsd-plan-head-before-01-03` already existed with the halted run's base (`35ccd096c`), which would have counted `261996ecf` and the intermediate docs commits as this run's work.
- **Fix:** Reset the ledger to the spawn-time HEAD (`266ff85fc`) before the first task commit so `actuals.commits` measures this execution (3).
- **Files modified:** `.git/gsd-plan-head-before-01-03` (planning state, not committed)
- **Verification:** `git rev-list --count 266ff85fc..HEAD` = 3.
- **Committed in:** n/a (planning state)

**2. [Rule 1 - Bug] ThornTurtleShell run-speed deviation (D-12)**
- **Found during:** Task 2 (accessory parity)
- **Issue:** the design row states `-10% 冲刺速度`; the code reduced `maxRunSpeed` by 5%.
- **Fix:** changed the factor to 0.1 and updated the comment. Move speed (`-7%`) and damage reduction (`+5%`) already matched.
- **Files modified:** `Sources/.../Accessories/ThornTurtleShell.cs`
- **Verification:** Release build 0 errors; BOM check passed.
- **Committed in:** `8439cdb31`

**3. [Rule 2 - Missing Critical] Effect parity could not be forced inside the item class**
- **Found during:** Task 1
- **Issue:** four design `效果` cells describe behavior implemented in projectile files, which this task's acceptance criteria place outside its modify set (`No file outside the 26 listed classes ... is modified`).
- **Fix:** recorded a single-line `效果` blocker per affected entry naming the unreconciled field, as the plan's Task 1 action directs for prose-ambiguous cells.
- **Files modified:** `01-INVENTORY.json`
- **Verification:** both inventory gates exit 0; the records flip those entries from silently-accepted to explicitly-blocked.
- **Committed in:** `3a339daf0`

---

**Total deviations:** 3 auto-fixed (1 blocking, 1 bug, 1 missing-critical)
**Impact on plan:** No scope creep. The only code change is the design-mandated accessory parity fix; everything else is inventory/planning record and a new gate.

## Issues Encountered

- The old `01-03-SUMMARY.md` (from the halted run) was stale; it is overwritten here per the replan instruction. Its numeric-parity evidence is preserved in the `requires` graph (commit `261996ecf`).
- Windows PowerShell console mojibake while printing Chinese literals is cosmetic; the committed JSON/Markdown/script were verified byte-level (UTF-8, no BOM, LF) and the JSON parses.

## User Setup Required

None - no external service configuration, no packages or secrets; no Feishu write was made.

## Next Phase Readiness

- Tranche A is gated: `check-tranche-A.ps1` (38 covered) plus the two reconciliation gates and the Release build.
- Phase 2 receives 43 class-less entries with `phase = 2`; the 18 completed-art ones are marked `deferred` with reasons. Phase 2 can also consume the 4 `效果` blockers as deviation work.
- Blockers carried forward: the 4 tranche-A effect blockers, the RadialCarapace/VineRepairWand artwork blockers (Phase 1 tracked), the unresolved Green Tundra label, and the 3 undefined-future placeholder rows.
- Plan 01-04 (tranche B) and 01-05 (localization/roll-up) are the remaining Phase 1 plans.

---

*Phase: 01-item-inventory-completed-art-items*
*Completed: 2026-09-12*

## Self-Check: PASSED

- All 4 declared files exist (inventory JSON/MD, ThornTurtleShell.cs, check-tranche-A.ps1).
- Task commits `3a339daf0`, `8439cdb31`, `d2bd94c11` exist in git history.
- `check-inventory-reconciliation.ps1`, `validate-inventory.ps1`, and `check-tranche-A.ps1` all exit 0; Release build 0 errors; BOM check passed.
