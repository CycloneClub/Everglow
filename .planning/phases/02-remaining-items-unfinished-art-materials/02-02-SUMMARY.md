---
phase: 02-remaining-items-unfinished-art-materials
plan: 02
subsystem: gameplay-content
tags: [terraria, tmodloader, weapons, projectiles, fallback-texture, inventory-audit, powershell-gate]

# Dependency graph
requires:
  - phase: 02-remaining-items-unfinished-art-materials
    provides: 02-CLASSIFICATION.json (21 rows, 9 full / 12 shell) and the green scripts/check-phase2.ps1 gate; the D-13 fallback-texture and D-11/D-22 marking precedents (plan 02-01)
  - phase: 01-item-inventory-completed-art-items
    provides: 01-INVENTORY.json (103 rows, per-entry phase/blockers/advances) and the committed evidence XML design tables
provides:
  - "BoulderCatapult + BoulderCatapult_Proj + BoulderCatapult_SubProj: a self-contained ranged launcher with a 150% direct hit and a 3-6 shard burst, no ammunition"
  - "TendonGreatbow + TendonGreatbow_Arrow: a bow with a +10% boss-target final-damage clause"
  - "RestrictionDeviceRE01 and ReekingBait: summon identity classes carrying precise effect/recipe/encounter blockers"
  - "Four biology-design weapon rows marked code_complete with precise texture/recipe/effect blockers; gate advanced to 8/21"
affects: [02-03, 02-04, 02-05, 07-bosses-special-encounters, 08-design-status-sync]

# Actuals (#2632)
actuals:
  tokens: 6058    # chars/4 over the changed-line diff of the three plan commits
  tasks: 3
  commits: 3
plan_head_before: 13b97a97b87d32e0e170988fdce9ac2af1b00bbc

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Two-hit 150% direct-hit: base Projectile.damage on impact plus one owner-guarded ApplyDamageToNPC for the remaining 50%, gated by a per-projectile AppliedDirectBonus flag"
    - "Boss-target damage bonus via ModifyHitNPC: if (target.boss) modifiers.FinalDamage *= 1.1f"
    - "Recipe/effect blockers recorded as precise in-code comments plus inventory blockers because the named ingredients/encounters are absent Phase 7 types that would not compile"
    - "Surgical single-row JSON transform (UTF-8, minimal diff) instead of a whole-file ConvertTo-Json reformat"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BoulderCatapult.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_Proj.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_SubProj.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/TendonGreatbow_Arrow.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RestrictionDeviceRE01.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ReekingBait.cs
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md

key-decisions:
  - "Implement the two effects the design fully specifies (150% direct hit + 3-6 shards; +10% to bosses) and record the charge curve, the 限制无人机 beam and the 巨翼龙 encounter as precise blockers rather than inventing behaviour"
  - "Write no AddRecipes body for 肌腱巨弓 / 限制机 / 腥臭的诱饵: their design 合成方式 cells name only Phase 7 Giant Winged Dragon items absent from the repository, and an item-type reference to an absent type is a compile error"
  - "Map the qualitative 击退 强 -> 8f and 弱 -> 2f on the accepted family scale (DD-05) and follow the committed evidence row for 肌腱巨弓 damage 58, recording both under D-23 (DD-05/DD-06)"
  - "Guard every client-only dust/sound call in the two new projectiles with if (!Main.dedServ), unlike the GreenThornLauncher analog whose OnKill leaves them unguarded"

patterns-established:
  - "Absent-dependency weapon: full identity + stats + comment-named recipe/effect blockers matching the inventory blockers verbatim"
  - "150% direct hit without a second projectile: owner-guarded ApplyDamageToNPC for the remaining 50% under a one-shot flag"
  - "Per-entry inventory edit as a single-row string transform anchored on the unique id"

requirements-completed: [ITEM-01, ITEM-03]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "BoulderCatapult fires its own boulder projectile (no ammo) that arcs under gravity, deals a 150% direct hit and bursts into 3-6 shards at 15% damage, with every client-only call server-guarded"
    requirement: "ITEM-01"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0, 0 errors)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 (OK)"
        status: pass
    human_judgment: true
    rationale: "Projectile arcs, burst counts, the 150% total and the no-ammo behaviour are runtime gameplay properties (D-21); no offline script observes them."
  - id: D2
    description: "TendonGreatbow consumes arrows and fires TendonGreatbow_Arrow which multiplies final damage by 1.1 against boss targets; RestrictionDeviceRE01 and ReekingBait exist with their documented stats and no absent-type references"
    requirement: "ITEM-01"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0, 0 errors)"
        status: pass
    human_judgment: true
    rationale: "The +10% boss-damage clause and the intentional absence of the blocked summon/encounter effects are runtime gameplay properties (D-21)."
  - id: D3
    description: "The four weapon rows carry code_complete/artwork_complete/status plus precise texture/recipe/effect blockers; 02-CLASSIFICATION.json has eight implemented rows; the phase gate and both Phase 1 inventory gates are green at 8/21 and 103 entries"
    requirement: "ITEM-03"
    verification:
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 (OK(0) 8/21)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1 (OK 103 entries)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1 (OK 103 entries)"
        status: pass
    human_judgment: false

# Metrics
duration: 10min
completed: 2026-09-14
status: complete
---

# Phase 2 Plan 02: Biology-Design Weapon Drops Summary

**BoulderCatapult with a 150%-direct-hit explode-into-shards projectile chain, TendonGreatbow with a +10% boss-damage arrow, and the RestrictionDeviceRE01 / ReekingBait identities carrying precise blockers for their absent Phase 7 dependencies — gate advanced to 8/21.**

## Performance

- **Duration:** 10 min
- **Started:** 2026-09-14T10:15:01Z
- **Completed:** 2026-09-14T10:24:50Z
- **Tasks:** 3 / 3
- **Files modified:** 11 (7 created, 4 modified)

## Accomplishments

- Implemented the branch's one fully self-contained ranged weapon: `BoulderCatapult` fires its own boulder (no `Item.useAmmo`), the boulder arcs under gravity, the direct hit totals the documented 150% (base hit + one owner-guarded 50% `ApplyDamageToNPC`), and on death it bursts into `Main.rand.Next(3, 7)` shards at 15% damage — 3 to 6 inclusive.
- Implemented `TendonGreatbow` (damage 58, crit 12, use time 28, Pink, 4g) with `TendonGreatbow_Arrow`, whose `ModifyHitNPC` multiplies final damage by 1.1 when `target.boss`, realising 对Boss单位额外造成10%伤害.
- Added `RestrictionDeviceRE01` and `ReekingBait` as summon identities that compile without referencing any absent Phase 7 type, each carrying in-code comments that mirror the inventory recipe/effect/encounter blockers.
- Marked the four rows per D-11/D-22 (`code_complete:true`, `artwork_complete:false`, `status:unchecked`), advanced `02-CLASSIFICATION.json` to eight implemented rows, and kept the phase gate and both Phase 1 inventory gates green (8/21, 103 entries).

## Task Commits

Each task was committed atomically:

1. **Task 1: Implement the boulder catapult and its explode-into-shards projectile chain** - `e7692b528` (feat)
2. **Task 2: Implement the tendon greatbow with its boss-damage arrow, the restriction device, and the reeking bait** - `cdc5f50c6` (feat)
3. **Task 3: Mark the four weapon rows, record their blockers and deviations, and run the gate chain** - `bdef8269f` (docs)

**Plan metadata:** `(final metadata commit made by this execution)` (docs: complete plan)

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BoulderCatapult.cs` - ranged launcher; damage 44, knockback 15, use time 77, Orange, 2g, no ammo; `Item.shoot` = `BoulderCatapult_Proj`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_Proj.cs` - gravity arc, one-shot 150% direct hit, 3-6 shard burst, `!Main.dedServ`-guarded dust/sound
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_SubProj.cs` - 14x22 shard, penetrate 2, 300-frame life, rotation, guarded dust/sound
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs` - bow; damage 58, crit 12, use time 28, Pink, 4g, arrow ammo; `Shoot` reassigns to `TendonGreatbow_Arrow`; no recipe/charge (comments name the blockers)
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/TendonGreatbow_Arrow.cs` - arrow flight mirroring `CyatheaArrow_proj`; `ModifyHitNPC` +10% final damage to bosses
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RestrictionDeviceRE01.cs` - summon staff identity; damage 18, mana 15, use time 21, Pink, 4g; no `Item.shoot`, no recipe
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ReekingBait.cs` - consumable summon identity; Blue, 20S; no NPC spawn, no recipe
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` - the four rows marked with `internal_name`, `code_complete:true` and the texture/recipe/effect blockers (minimal 22/16-line diff)
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` - the four mirror rows refreshed (103-row matrix preserved)
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json` - the four rows flipped to `implemented: true` (8/21)
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md` - §5 added: design values, recipe blockers, effect blockers, DD-05 击退 mapping, DD-06 damage correction, D-20 localization deferral

## Decisions Made

- **Implement only what the design fully specifies.** The two fully-specified effects ship (150% direct hit with 3-6 shards; +10% to bosses); the charge curve, the 限制无人机 beam system and the 巨翼龙 encounter are recorded as precise blockers instead of invented behaviour.
- **No recipe bodies for absent ingredients.** `肌腱巨弓` / `限制机` / `腥臭的诱饵` reference only Phase 7 Giant Winged Dragon items; an item-type reference to an absent type is a compile error (T-02-01), so each records a `recipe` blocker.
- **Numeric knockback mapping (DD-05).** Design 强 -> `Item.knockBack = 8f`, 弱 -> `2f`, calibrated against the accepted family (`GreenThornBallLauncher` 5.5, `QuetzalsWish` 6, `RedAlgaeMinionStaff` 2); pending designer confirmation.
- **Evidence over research (DD-06/T-02-08).** `肌腱巨弓` damage is 58 per the committed `evidence/biology.xml` row; the earlier research draft's 28 was the use-time cell, and the correction is recorded.
- **Server-guard every client-only call.** Unlike the analog `GreenThornLauncher_Proj`, the new projectiles wrap all dust/sound in `if (!Main.dedServ)` (T-02-04).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] TendonGreatbow comment contained the literal planned-grep token**
- **Found during:** Task 2 (verify)
- **Issue:** The do-not-write-a-recipe comment originally contained the literal text `ModContent.ItemType<T>()`. The plan's verification requires `grep -l 'ModContent.ItemType'` over the four new classes to return nothing; the comment alone made it match.
- **Fix:** Reworded the comment to "an item-type reference to an absent type does not compile" (no literal token).
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs`
- **Verification:** grep returns nothing; Release build and `check-phase2.ps1` re-run green
- **Committed in:** `cdc5f50c6` (Task 2)

---

**Total deviations:** 1 auto-fixed (1 Rule 1 bug in a new class comment)
**Impact on plan:** No scope creep; the fix was required to satisfy the plan's own verification bullet and touched only a comment.

## Issues Encountered

- The first run of the surgical JSON transform joined the extra blocker strings without wrapping them in quotes, so the file it wrote was momentarily invalid. It was restored with `git checkout -- 01-INVENTORY.json` (no Task 3 commit had been made yet), the script was corrected to quote each element, and the re-run produced valid JSON — confirmed by `JSON.parse` and both inventory gates. No data was lost and the final diff is minimal (22 insertions / 16 deletions).
- PowerShell 5.1 console encoding renders the CJK content of the planning files as mojibake, so verification was performed through the gates (which read the files via `[IO.File]::ReadAllText`, honouring UTF-8) and through a Node parse rather than by eyeballing console output.

## Known Stubs

None. The `RestrictionDeviceRE01` (no `Item.shoot`) and `ReekingBait` (no spawn) behaviour is intentionally absent by design and is recorded as precise, named blockers in both `01-INVENTORY.json` and `02-DEVIATIONS.md` — this is the plan's stated discipline (implement everything the design documents that this phase can build; record the rest), not an untracked stub.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- The phase gate stays green at 8/21; plans 02-03/02-04/02-05 can add their rows by flipping `implemented` and filling `internal_name` + blockers on the same contract.
- Outstanding recorded blockers for Phase 7: the `肌腱巨弓` charge curve, the `限制机` 限制无人机/聚能射线/浊燃 system, and the `腥臭的诱饵` 巨翼龙 encounter.
- Runtime verification (D-21) of both weapon chains — arcs/burst counts/no-ammo for the catapult and the boss-damage bonus for the greatbow — remains outstanding and cannot be covered offline; the two `<human-check>` blocks in the plan hold the exact reproduction steps.

---

*Phase: 02-remaining-items-unfinished-art-materials*
*Completed: 2026-09-14*

## Self-Check: PASSED

- All 7 created `.cs` files exist on disk; both projectile class files resolve under `Items/` for the gate.
- All 3 task commits exist: `e7692b528`, `cdc5f50c6`, `bdef8269f` (measured `3` commits since `plan_head_before` = `13b97a97b87d32e0e170988fdce9ac2af1b00bbc`).
