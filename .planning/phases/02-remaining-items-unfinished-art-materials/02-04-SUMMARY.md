---
phase: 02-remaining-items-unfinished-art-materials
plan: 04
subsystem: gameplay-content
tags: [terraria, tmodloader, identity-shell, system-blocker, fallback-texture, inventory-audit, powershell-gate]

# Dependency graph
requires:
  - phase: 02-remaining-items-unfinished-art-materials
    provides: 02-CLASSIFICATION.json (21 rows, 9 full / 12 shell) and the green scripts/check-phase2.ps1 gate; the D-13 fallback-texture, D-18 shell and D-11/D-22 marking precedents (plans 02-01 / 02-02 / 02-03)
  - phase: 01-item-inventory-completed-art-items
    provides: 01-INVENTORY.json / 01-INVENTORY.md (103 rows, per-entry phase/blockers/advances) and the committed evidence XML design tables
provides:
  - "Six identity-only shell classes: AlcoholicDrinks, FluorescentHydraStaff, DiscipleSword, DiscipleVanity, SkillBambooSlip, RegionalCraftingStation"
  - "Four D-19 system-blocker shells that reserve the disciple, skill and regional-crafting names without building any of those systems; RegionalCraftingStation stays a plain ModItem (no ModTile)"
  - "Six inventory rows marked code_complete with the D-13 texture blocker plus an entry-specific system/artwork blocker; gate closed at 21/21"
  - "02-DEVIATIONS.md §7: the D-19 system blockers, shell placement (DD-11..DD-13) and the extended D-20 localization deferral"
affects: [02-05, 07-bosses-special-encounters, 08-design-status-sync]

# Actuals (#2632)
actuals:
  tokens: 5773   # chars/4 over the changed-line diff of the three plan commits
  tasks: 3
  commits: 3
plan_head_before: 0d03d305230df8f640c95a9bee21d5937d2188a3

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "System-dependent identity shell (D-19): a minimal ModItem whose class comment and inventory blocker name the exact missing system, with no system class, no recipe and no tile introduced"
    - "Plain-ModItem placeable reservation: an Items/Placeables/ class that declares no ModTile, no Item.createTile and no DefaultToPlaceableTile while the placement system is absent, so no half-built tile exists"
    - "Surgical six-row JSON transform anchored on each unique id, preserving the PowerShell ConvertTo-Json layout and re-validated with JSON.parse"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/AlcoholicDrinks.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/FluorescentHydraStaff.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/DiscipleSword.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/DiscipleVanity.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/SkillBambooSlip.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/RegionalCraftingStation.cs
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md

key-decisions:
  - "Implement all six entries as identity-only shells with ForestBreath's conservative defaults (20x20, 50 silver, Blue) because the design rows carry no 伤害 / 价格 / 稀有度 / 效果 (D-18); no behaviour is invented"
  - "Reserve the disciple, skill and regional-crafting names as shells whose comments and inventory blockers name the missing system exactly; none of those systems is implemented (D-19)"
  - "Keep RegionalCraftingStation a plain ModItem: no ModTile, no createTile, no DefaultToPlaceableTile, because a tile without its regional-crafting system would be a half-built feature (T-02-04)"
  - "Place 技能竹简 and 弟子时装 in Items/Misc/ rather than the Items/Weapons/ and Items/Vanity/ folders 02-PATTERNS.md proposed, correcting both 02-CLASSIFICATION.json class_file paths (DD-11/DD-12)"
  - "Record 荧光水螅召唤杖 under artwork/absent-summon-projectile blockers, not the system blockers, because it needs no system"

patterns-established:
  - "System-blocker shell: family LocalizationCategory + D-13 texture + a class comment that names the exact missing system"
  - "Audit-truth correction: a manifest class_file path that disagrees with the repository's actual placement is corrected in the same task that flips implemented"

requirements-completed: [ITEM-02, ITEM-03]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "The six remaining shell entries (AlcoholicDrinks, FluorescentHydraStaff, DiscipleSword, DiscipleVanity, SkillBambooSlip, RegionalCraftingStation) exist as loadable identity-only classes with their family LocalizationCategory, the D-13 texture and no invented behaviour (no recipe, Item.shoot, UpdateAccessory, equip slot, ModTile or createTile)"
    requirement: "ITEM-03"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0, 0 errors)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 (OK 15/21, class resolution green)"
        status: pass
    human_judgment: true
    rationale: "Whether every art-missing class loads and resolves (no missing-resource error) and that none grants a stat or effect are runtime loader properties (D-21); no offline script observes them."
  - id: D2
    description: "Skills 弟子剑/弟子时装 name the missing 弟子 progression system, 技能竹简 names the missing skill system and 区域放置物品制作台 names the missing regional-crafting system plus its placement tile; none of those systems appears in the diff and the crafting station stays a plain ModItem"
    requirement: "ITEM-02"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0, 0 errors)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 -RequireAll (OK(0) 21/21)"
        status: pass
    human_judgment: true
    rationale: "The absence of the systems and the reserved-name shells are structural facts checked offline; the runtime load behaviour of the shells is the plan's backstop (D-21)."
  - id: D3
    description: "The six inventory rows carry internal_name + code_complete:true + artwork_complete:false + status:unchecked + a matching system/artwork blocker; 02-CLASSIFICATION.json has all 21 rows implemented:true; 02-DEVIATIONS.md §7 records the D-19 system blockers, the shell placement and the extended D-20 deferral; the phase gate and both Phase 1 inventory gates are green at 21/21 and 103 entries"
    requirement: "ITEM-02"
    verification:
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 -RequireAll (OK(0) 21/21)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1 (OK(0) 103 entries)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1 (OK(0) 103 entries; 103-row matrix preserved)"
        status: pass
    human_judgment: false

# Metrics
duration: 8min
completed: 2026-09-14
status: complete
---

# Phase 2 Plan 04: The Four System-Dependent Shells and the Remaining Two Art-Pending Shells Summary

**Six final identity-only shells — AlcoholicDrinks, FluorescentHydraStaff, DiscipleSword, DiscipleVanity, SkillBambooSlip and RegionalCraftingStation — each reserving a name with a precise system/artwork blocker, closing the phase's item coverage at 21 of 21.**

## Performance

- **Duration:** 8 min
- **Started:** 2026-09-14T10:47:19Z
- **Completed:** 2026-09-14T10:55:06Z
- **Tasks:** 3 / 3
- **Files modified:** 10 (6 created, 4 modified)

## Accomplishments

- Created the last six identity-only shells (D-18) with family-correct `LocalizationCategory` values and the D-13 `Commons.ModAsset.White_Mod` texture: 若干酒类 / 技能竹简 (`Miscs`), 荧光水螅召唤杖 (`SummonWeapons`), 弟子剑 (`MeleeWeapons`), 弟子时装 (`Vanity`) and 区域放置物品制作台 (`Placeables`). No recipe, `Item.shoot`, `UpdateAccessory`, equip slot, `ModTile` or `createTile` was invented.
- Honoured the D-19 boundary: the four system-dependent shells name the missing 弟子 (disciple) progression, skill and regional-crafting systems in both their class comments and their inventory blockers; no system was implemented. `RegionalCraftingStation` stays a plain `ModItem` so no half-built tile exists while both its system and its placement tile are absent.
- Marked the six inventory rows per D-11/D-22 (`internal_name` filled, `code_complete:true`, `artwork_complete:false`, `status:"unchecked"`) with the D-13 texture blocker plus an entry-specific blocker, refreshed the Markdown mirror while preserving the 103-row matrix, and flipped the last six `02-CLASSIFICATION.json` rows to `implemented:true`.
- Opened `02-DEVIATIONS.md` §7 with the D-19 system-blocker table, the 荧光水螅召唤杖 absent-summon-projectile note, the shell-placement rows (DD-11..DD-13) and the extended D-20 localization deferral; the phase gate closes green at 21/21.

## Task Commits

Each task was committed atomically:

1. **Task 1: Implement the 若干酒类, 荧光水螅召唤杖 and 弟子剑 shells** - `1343dcc14` (feat)
2. **Task 2: Implement the 弟子时装, 技能竹简 and 区域放置物品制作台 system-dependent shells** - `24f49bda5` (feat)
3. **Task 3: Mark the final six rows, record the system blockers, and gate the phase at 21 of 21** - `15ce55664` (docs)

**Plan metadata:** `(final metadata commit made by this execution)` (docs: complete plan)

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/AlcoholicDrinks.cs` - identity-only `Miscs` shell; comment records the 描述 只喝了一两口就丢进去了 and that no consumable buff is declared
- `.../Items/Weapons/FluorescentHydraStaff.cs` - identity-only `SummonWeapons` shell; no `Item.shoot`/`Item.staff` because the 荧光水螅 summon projectile is absent
- `.../Items/Weapons/DiscipleSword.cs` - identity-only `MeleeWeapons` shell, `Item.maxStack = 1`; comment names the missing 弟子 progression system
- `.../Items/Misc/DiscipleVanity.cs` - identity-only `Vanity` shell with `Item.vanity = true`, no equip slot; comment names the 弟子 system; placed beside `WitheredMask`
- `.../Items/Misc/SkillBambooSlip.cs` - identity-only `Miscs` shell, `Item.CommonMaxStack`; comment names the skill system and quotes 提交给NPC后学习
- `.../Items/Placeables/RegionalCraftingStation.cs` - plain `ModItem` (`Placeables`); comment names the regional-crafting system and the missing placement tile, and records 宝箱的副掉落，非核心物品
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` - six rows marked with `internal_name`, `code_complete:true`, the D-13 texture blocker, an entry-specific blocker and a `notes` sentence (minimal 30/24-line diff)
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` - the six mirror Blockers cells refreshed (103-row matrix preserved)
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json` - the last six rows flipped to `implemented: true` (21/21); the `SkillBambooSlip` and `DiscipleVanity` `class_file` paths corrected
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md` - §7 added: the D-19 system-blocker table, the 荧光水螅召唤杖 note, DD-11..DD-13 shell placement, the effect/artwork blocker table and the D-20 deferral

## Decisions Made

- **Identity without invention (D-18).** Every shell reproduces `ForestBreath`'s conservative shape (20x20, 50 silver, Blue) because the design rows carry no 伤害 / 价格 / 稀有度 / 效果; the two rows with a 描述 still declare no behaviour.
- **System boundary held (D-19).** The four system-dependent shells reserve their names and record the exact missing system; no disciple, skill or regional-crafting code exists in the diff.
- **No half-built tile (T-02-04).** `RegionalCraftingStation` is a plain `ModItem`; the missing tile is part of its recorded blocker rather than a `ModTile` without a system.
- **Placement over the planner's folder guess (DD-11/DD-12).** 技能竹简 and 弟子时装 live in `Items/Misc/` (the quest-item analog `ForestBreath` and the tracked vanity `WitheredMask`), and the pre-existing manifest paths were corrected.
- **Surgical, validated JSON/Markdown transform.** The six-row update was anchored on each unique id, preserved the PowerShell `ConvertTo-Json` layout, and was re-validated with `JSON.parse` plus the 103-row Markdown invariant.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] `02-CLASSIFICATION.json` pointed 技能竹简 and 弟子时装 at non-existent folders**
- **Found during:** Task 3 (shell-placement decision)
- **Issue:** The pre-existing classification rows carried `Items/Weapons/SkillBambooSlip.cs` and `Items/Vanity/DiscipleVanity.cs`, but the plan places both shells in `Items/Misc/` (no `Items/Vanity/` directory exists, and 技能竹简's 描述 提交给NPC后学习 makes `Items/Misc/ForestBreath.cs` the closer analog).
- **Fix:** Corrected the two `class_file` values to `Items/Misc/SkillBambooSlip.cs` and `Items/Misc/DiscipleVanity.cs`, and recorded the phase-wide placement rule as DD-11/DD-12 in `02-DEVIATIONS.md`.
- **Files modified:** `.planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json`, `02-DEVIATIONS.md`
- **Verification:** `check-phase2.ps1 -RequireAll` resolves the classes from the working tree and passes at 21/21
- **Committed in:** `15ce55664` (Task 3)

---

**Total deviations:** 1 auto-fixed (1 Rule 1 manifest path correction, the mechanical consequence of the plan's own placement decisions)
**Impact on plan:** No scope creep. The system boundary was respected; the path correction keeps the manifest honest.

## Issues Encountered

- PowerShell 5.1 reads BOM-less scripts as the system ANSI code page, so the phase gate stays 100% ASCII and the CJK inventory content renders as mojibake in the console; verification was performed through the gates (which read UTF-8 via `[IO.File]::ReadAllText`) and a Node `JSON.parse`, not by eyeballing console output.
- `||` is not a valid statement separator in PowerShell 5.1; the pre-commit branch assertion was rewritten with `;`-chained statements.

## Known Stubs

None. Every shell's absent behaviour is intentional by D-18/D-19 and is recorded as a precise, named blocker in both `01-INVENTORY.json` and `02-DEVIATIONS.md`; the missing disciple/skill/regional-crafting systems are the plan's stated boundary, not untracked stubs.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- The phase gate is green at 21/21; plan 02-05 (the consolidated phase validation/deviations closure) can run against a fully-implemented manifest.
- Outstanding recorded blockers: all 21 entries' approved artwork (D-13), and the four system-dependent shells (弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台) until the disciple/skill/regional-crafting systems and the crafting station's tile are designed.
- Runtime verification (D-21) of the six shells — white-box icon, no missing-resource load error, no granted stat or effect — remains outstanding and cannot be covered offline; it is recorded in `.planning/WINDOWS.md` entry 12 and the plan's `<human-check>` holds the exact reproduction steps.

---

*Phase: 02-remaining-items-unfinished-art-materials*
*Completed: 2026-09-14*

## Self-Check: PASSED

- All six created `.cs` files exist on disk and resolve under `Items/` for the gate.
- All three task commits exist: `1343dcc14`, `24f49bda5`, `15ce55664` (measured `3` commits since `plan_head_before` = `0d03d305230df8f640c95a9bee21d5937d2188a3`).
