---
phase: 02-remaining-items-unfinished-art-materials
plan: 03
subsystem: gameplay-content
tags: [terraria, tmodloader, use-item, identity-shell, fallback-texture, inventory-audit, powershell-gate]

# Dependency graph
requires:
  - phase: 02-remaining-items-unfinished-art-materials
    provides: 02-CLASSIFICATION.json (21 rows, 9 full / 12 shell) and the green scripts/check-phase2.ps1 gate; the D-13 fallback-texture, D-18 shell and D-11/D-22 marking precedents (plans 02-01 / 02-02)
  - phase: 01-item-inventory-completed-art-items
    provides: 01-INVENTORY.json / 01-INVENTORY.md (103 rows, per-entry phase/blockers/advances) and the committed evidence XML design tables
provides:
  - "JadeSnakeEgg: the ITEM-04 consumable 灵蛇玉卵 use item (10 gold, Blue, SummonItems) carrying its Phase 7 encounter + location blockers"
  - "Six identity-only shell classes: BambooStepTalisman, BambooWeapon, BambooHairpin, PeachBranchAmulet, PeachBlossomKite, PandaPet"
  - "Seven inventory rows marked code_complete with the D-13 texture blocker plus an entry-specific effect/artwork blocker; gate advanced to 15/21"
  - "02-DEVIATIONS.md §6: the identity-only-shell policy, the Items/Misc vanity placement, the use-item category artefact and the D-20 deferral"
affects: [02-04, 02-05, 07-bosses-special-encounters, 08-design-status-sync]

# Actuals (#2632)
actuals:
  tokens: 10297   # chars/4 over the changed-line diff of the three plan commits
  tasks: 3
  commits: 3
plan_head_before: 9ffa2db58bf4798eeeb0c17090d670ef05f9ba5c

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Identity-only shell (D-18): a ModItem declaring only LocalizationCategory, the D-13 White_Mod Texture override and conservative SetDefaults values, with no recipe/shoot/UpdateAccessory/equip slot for a design row that defines none"
    - "Use-item identity without its encounter: a consumable SummonItems item with documented value/rarity that spawns nothing and references no NPC type while the encounter phase owns the spawn and the location gate"
    - "Surgical single-entry JSON transform anchored on the unique id and the notes sentinel, preserving the PowerShell ConvertTo-Json layout instead of a whole-file reformat"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/JadeSnakeEgg.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/BambooStepTalisman.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BambooWeapon.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/BambooHairpin.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/PeachBranchAmulet.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/PeachBlossomKite.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Pets/PandaPet.cs
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md

key-decisions:
  - "Implement 灵蛇玉卵 as a real consumable use item (10 gold, Blue, 12-frame swing) but spawn nothing: the 苍翠灵蛇 encounter is Phase 7 and the 在森雨幽谷顶部使用 condition is a gate that encounter phase owns, so both are recorded as one precise effect blocker rather than an absent-NPC reference"
  - "Implement the six shells as identity-only classes with ForestBreath's conservative defaults (20x20, 50 silver, Blue) because the design rows carry no 伤害 / 价格 / 稀有度 / 效果 (D-18); no behaviour is invented"
  - "Place the 竹簪子 vanity shell in Items/Misc (not a new Items/Vanity folder) and correct its 02-CLASSIFICATION.json class_file path, matching the only tracked vanity precedent WitheredMask"
  - "Give each shell its family LocalizationCategory: Accessories (竹节步符, 桃枝护符), MeleeWeapons (竹制武器), Vanity (竹簪子), Miscs (桃花纸鸢), Pets (熊猫宠物)"
  - "Record entry-specific blockers verbatim in both 01-INVENTORY.json and 01-INVENTORY.md so the JSON/Markdown mirror reconciliation stays exact"

patterns-established:
  - "Consumable use-item identity for an encounter owned by a later phase (value/rarity transcribed, spawn + location blocker)"
  - "Per-family identity shell: LocalizationCategory + D-13 texture + accessory/vanity flag only, never a stat block"
  - "Seven-row audit update driven by an anchored, JSON.parse-validated transform that preserves the existing file layout"

requirements-completed: [ITEM-03, ITEM-04]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "灵蛇玉卵 exists as a loadable consumable use item with ItemRarityID.Blue, Item.buyPrice(gold: 10), the D-13 fallback texture, and a precise blocker for the 苍翠灵蛇 encounter and the 在森雨幽谷顶部使用 location condition; it references no NPC type"
    requirement: "ITEM-04"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0, 0 errors)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 (OK 15/21)"
        status: pass
    human_judgment: true
    rationale: "Whether the item is actually held and consumed in a client, and that it spawns nothing, are runtime gameplay properties (D-21); no offline script observes them."
  - id: D2
    description: "The six shells (BambooStepTalisman, BambooWeapon, BambooHairpin, PeachBranchAmulet, PeachBlossomKite, PandaPet) exist as loadable identity-only classes with their family LocalizationCategory, the D-13 texture and no invented behaviour (no recipe, Item.shoot, UpdateAccessory or equip slot)"
    requirement: "ITEM-03"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0, 0 errors)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 (OK 15/21, class resolution green)"
        status: pass
    human_judgment: true
    rationale: "Whether every art-missing class loads and resolves (no missing-resource error) and that the accessory equips / the vanity declares no slot are runtime loader properties (D-21)."
  - id: D3
    description: "The seven inventory rows carry internal_name + code_complete:true + artwork_complete:false + status:unchecked + a matching blocker; 02-CLASSIFICATION.json has fifteen implemented rows; 02-DEVIATIONS.md §6 records the shell policy, vanity placement, use-item category artefact and D-20 deferral; the phase gate and both Phase 1 inventory gates are green at 15/21 and 103 entries"
    requirement: "ITEM-03"
    verification:
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 (OK(0) 15/21)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1 (OK(0) 103 entries)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1 (OK(0) 103 entries)"
        status: pass
    human_judgment: false

# Metrics
duration: 7min
completed: 2026-09-14
status: complete
---

# Phase 2 Plan 03: Snake Egg Use Item and the Six Art-Pending Shells (Group A) Summary

**The 灵蛇玉卵 consumable use item (10 gold, Blue) plus six identity-only shells — BambooStepTalisman, BambooWeapon, BambooHairpin, PeachBranchAmulet, PeachBlossomKite and PandaPet — marked with precise art/effect blockers, advancing the phase gate to 15/21.**

## Performance

- **Duration:** 7 min
- **Started:** 2026-09-14T18:32:06+08:00
- **Completed:** 2026-09-14T18:39:22+08:00
- **Tasks:** 3 / 3
- **Files modified:** 11 (7 created, 4 modified)

## Accomplishments

- Implemented the ITEM-04 carrier `灵蛇玉卵` as a real consumable use item (`SummonItems`, 20x20, `Item.buyPrice(gold: 10)`, `ItemRarityID.Blue`, 12-frame swing, Roar sound) that spawns nothing and references no NPC type; the 苍翠灵蛇 encounter (Phase 7) and the 在森雨幽谷顶部使用 location condition are recorded as one precise effect blocker.
- Created the first six of the twelve identity-only shells (D-18) with family-correct `LocalizationCategory` values and the D-13 `Commons.ModAsset.White_Mod` texture: 竹节步符 / 桃枝护符 (Accessories), 竹制武器 (MeleeWeapons), 竹簪子 (Vanity), 桃花纸鸢（风筝） (Miscs) and 熊猫宠物 (Pets). No recipe, `Item.shoot`, `UpdateAccessory` or equip slot was invented.
- Marked the seven inventory rows per D-11/D-22 (`internal_name` filled, `code_complete:true`, `artwork_complete:false`, `status:"unchecked"`) with the D-13 texture blocker plus an entry-specific blocker, and refreshed the Markdown mirror while preserving the 103-row matrix.
- Advanced `02-CLASSIFICATION.json` to fifteen implemented rows and opened `02-DEVIATIONS.md` §6 with the identity-only-shell policy (all twelve shells), the Items/Misc vanity placement, the use-item category artefact and the D-20 localization deferral; the full gate chain is green.

## Task Commits

Each task was committed atomically:

1. **Task 1: Implement the 灵蛇玉卵 use item and the 竹节步符 accessory shell** - `b8ac91f86` (feat)
2. **Task 2: Implement the 竹制武器, 竹簪子, 桃枝护符 and 桃花纸鸢（风筝） shells** - `3a1efac30` (feat)
3. **Task 3: Implement the 熊猫宠物 shell, then mark the seven rows and gate** - `e5d43b4d4` (docs)

**Plan metadata:** `(final metadata commit made by this execution)` (docs: complete plan)

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/JadeSnakeEgg.cs` - consumable SummonItems use item; 10 gold, Blue, D-13 texture; no spawn / no NPC type; encounter + location blockers in the doc comment
- `.../Items/Accessories/BambooStepTalisman.cs` - identity-only Accessories shell; `Item.accessory = true`, no `UpdateAccessory`
- `.../Items/Weapons/BambooWeapon.cs` - identity-only MeleeWeapons shell; `Item.maxStack = 1`, no damage/stat block
- `.../Items/Misc/BambooHairpin.cs` - identity-only Vanity shell; `Item.vanity = true`, no equip slot, placed in Items/Misc
- `.../Items/Accessories/PeachBranchAmulet.cs` - identity-only Accessories shell; `Item.accessory = true`
- `.../Items/Misc/PeachBlossomKite.cs` - identity-only Miscs shell; `Item.CommonMaxStack`; 宝箱的副掉落，非核心物品 recorded
- `.../Items/Pets/PandaPet.cs` - identity-only Pets shell; no pet projectile or pet buff
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` - seven rows marked with `internal_name`, `code_complete:true`, the D-13 texture blocker, an entry-specific blocker and three `notes` sentences
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` - the seven mirror Blockers cells refreshed (103-row matrix preserved)
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json` - seven rows flipped to `implemented: true` (15/21); the 竹簪子 `class_file` corrected to `Items/Misc/BambooHairpin.cs`
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md` - §6 added: design values, the D-18 identity-only-shell table (all twelve), blocker table, DD-07 vanity placement, DD-08 use-item category artefact, DD-09 描述-derived categories, DD-10 non-core chest drop, D-20 deferral

## Decisions Made

- **Identity without invention (D-18).** Every shell reproduces `ForestBreath`'s conservative shape (20x20, 50 silver, Blue) because the design rows carry no 伤害 / 价格 / 稀有度 / 效果; a shell is an identity, not a guessed stat block.
- **Consumable identity, no spawn (T-02-04).** `JadeSnakeEgg` consumes one on use but references no NPC type; the Phase 7 苍翠灵蛇 encounter and the location gate are recorded as a single precise effect blocker computed against the committed design row.
- **Vanity in Items/Misc (DD-07).** The 竹簪子 shell lives beside `WitheredMask` (the only tracked vanity item) because no `Items/Vanity/` directory exists; the pre-existing classification path was corrected and the phase-wide rule recorded for 02-04's 弟子时装.
- **Category from the 描述 cell (DD-09).** 竹簪子's 时装 and 桃枝护符's 饰品 are the only category evidence in otherwise empty rows, selecting Vanity and Accessories.
- **Surgical, validated JSON/Markdown transform.** The seven-row update was anchored on each unique id and the `notes` sentinel and re-validated with `JSON.parse`, preserving the PowerShell `ConvertTo-Json` layout (minimal diff) and the 103-row Markdown invariant.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] The row-edit script's notes token omitted its trailing comma**
- **Found during:** Task 3 (first run of the audit-trail transform)
- **Issue:** The block window ended immediately after `"notes":  ""`, so the replacement token `"notes":  "",` (with a comma) never matched and the script threw before writing anything.
- **Fix:** Matched the comma-less token `"notes":  ""`; the script then completed and all three files passed `JSON.parse` / row-count validation.
- **Files modified:** `C:/Users/book/AppData/Local/Temp/opencode/edit-0203.js` (out-of-repo helper; no committed file affected)
- **Verification:** The transform printed `01-INVENTORY.json updated: 7 rows`, `103 matrix rows preserved`, `implemented=15 / 21`, and all three gates passed.
- **Committed in:** n/a (helper script outside the repository; the resulting edits shipped in `e5d43b4d4`)

### Manifest path correction

**2. [Rule 1 - Bug] `02-CLASSIFICATION.json` pointed 竹簪子 at a non-existent `Items/Vanity/` path**
- **Found during:** Task 3 (vanity-placement decision)
- **Issue:** The pre-existing classification row carried `Items/Vanity/BambooHairpin.cs`, but the plan places the vanity shell in `Items/Misc/` (no `Items/Vanity/` directory exists in the repository).
- **Fix:** Corrected the row's `class_file` to `Items/Misc/BambooHairpin.cs` and recorded the phase-wide rule as DD-07 in `02-DEVIATIONS.md`.
- **Files modified:** `.planning/phases/02-remaining-items-unfinished-art-materials/02-CLASSIFICATION.json`, `02-DEVIATIONS.md`
- **Verification:** `check-phase2.ps1` resolves the class from the working tree and passes at 15/21
- **Committed in:** `e5d43b4d4` (Task 3)

---

**Total deviations:** 2 auto-fixed (1 Rule 3 blocking helper-script fix, 1 Rule 1 manifest path correction)
**Impact on plan:** No scope creep. The helper-script fix was a tooling typo with no committed effect; the path correction keeps the manifest honest and is the mechanical consequence of the plan's own vanity-placement decision.

## Issues Encountered

- PowerShell 5.1 reads BOM-less scripts as the system ANSI code page, so the phase gate stays 100% ASCII and builds its CJK ids from `[char]0x....` codepoints; verification was performed through the gates (which read UTF-8 via `[IO.File]::ReadAllText`) and a Node `JSON.parse`, not by eyeballing console output.
- A PowerShell `$before..HEAD` expression is parsed by the range operator unless quoted; the commit count was re-measured with the quoted range form (`commits = 3`).

## Known Stubs

None. Every shell's absent behaviour is intentional by D-18 and is recorded as a precise, named blocker in both `01-INVENTORY.json` and `02-DEVIATIONS.md`; `JadeSnakeEgg`'s missing spawn is the plan's stated discipline (implement everything the design documents that this phase can build; record the rest), not an untracked stub.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- The phase gate stays green at 15/21; plan 02-04 can add its six remaining shell rows (the four `system` shells plus 若干酒类 and 荧光水螅召唤杖) by flipping `implemented` and filling `internal_name` + blockers on the same contract.
- Outstanding recorded blockers for Phase 7: the protected `JadeSnakeEgg` spawn (苍翠灵蛇) and its 在森雨幽谷顶部使用 location gate, plus all twelve shells' approved artwork.
- Runtime verification (D-21) of the seven entries (consume-without-spawn, accessory equip, vanity with no slot, every art-missing class loading) remains outstanding and cannot be covered offline; the two `<human-check>` blocks in the plan hold the exact reproduction steps.

---

*Phase: 02-remaining-items-unfinished-art-materials*
*Completed: 2026-09-14*

## Self-Check: PASSED

- All 7 created `.cs` files exist on disk and resolve under `Items/` for the gate.
- All 3 task commits exist: `b8ac91f86`, `3a1efac30`, `e5d43b4d4` (measured `3` commits since `plan_head_before` = `9ffa2db58bf4798eeeb0c17090d670ef05f9ba5c`).
