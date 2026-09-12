---
phase: 01-item-inventory-completed-art-items
plan: 04
subsystem: content-items
tags: [feishu, inventory, kelp-curtain, items, design-parity, d-10, d-11, d-12, tranche-b, armor, placeables, coverage-gate]

# Dependency graph
requires:
  - plan: 01-02
    provides: "Reconciled 01-INVENTORY.json (internal_name, repo_asset, localization, tranche, advances, blockers), the five-label taxonomy, and check-inventory-reconciliation.ps1"
  - plan: 01-03
    provides: "Tranche-A parity + the value convention (design 价格 -> Item.value), the class-less Phase 2 routing, and check-tranche-A.ps1 as the gate template"
provides:
  - "Tranche-B design parity for the 20 completed-art entries: 4 missing DevilHeart armor recipes, the Ruin set rarity correction, the two material rarities, the AlgaeExtractor/JadeFruit values, and 7 placeable LocalizationCategory fixes"
  - "scripts/check-tranche-B.ps1 - tranche-B coverage and no-placeholder gate (20 covered entries)"
  - "Recorded 套装效果/效果 blockers for WitherbarkHelmet, ShellMolluscsBreastPlate, RuinMask, and Photophore"
affects: [01-05, 02]

# Actuals (#2632) - pairs with the plan's estimate (tokens 55000, tasks 3, confidence low).
actuals:
  tokens: 5451        # chars/4 over the realized diff (21804 chars; 170 insertions / 11 deletions across 21 files)
  tasks: 3
  commits: 3          # MEASURED: git rev-list --count ${plan_head_before}..HEAD
  plan_head_before: 92b0397fc48e387130696548425d7df036147796

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Armor recipes live in the design 效果 cell after a '=' separator (the same convention that produced the DevilHeart Iron Bar furnace recipe in plan 02); missing recipes are a D-12 deviation, not new content"
    - "Set-bonus parity is judged on the mechanical effects (UpdateArmorSet + the runtime buff) first; a missing player.setBonus localized value is recorded as a blocker because plan 05 forbids hand-creating localization keys and the exporter does not generate SetBonus"
    - "Tranche coverage gates select by phase==1 + tranche + artwork_complete so phase-2 routing stays invisible while any un-implemented, un-blocked phase-1 entry fails"

key-files:
  created:
    - .planning/phases/01-item-inventory-completed-art-items/scripts/check-tranche-B.ps1
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartHairpin.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartHelmet.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartLightBreastPlate.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartLeggings.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinMask.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinMagicRobe.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinLeggings.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Molluscs/ShellMolluscsBreastPlate.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/DevilHeartIronOre_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/JadeizedBone_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/AlgaeExtractor_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/PermanentBoosters/JadeFruit.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/GreenCourtBrick_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/AgedGreenCourtBrick_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/WaterErodedGreenBrick_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/ForestRainVineTile_Thick_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/ForestRainVineTile_Thin_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/Walls/AgedGreenCourtWall_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/DecayingWoodCourt/CrackedForestThrone_Item.cs

key-decisions:
  - "Design cell columns were resolved by the armor-table <thead> headers (护具/贴图/代码/数值/价格/稀有度/效果/套装效果), never positionally; the recipe embedded after '=' in the 效果 cell was read as the recipe, matching the plan-02 DevilHeartIronBar convention."
  - "The 4 missing DevilHeart armor recipes were added rather than blocked: D-12 lists recipes as a completed-art deviation, the ingredient (DevilHeartIronBar_Item) and the Anvil tile exist, and the counts (18/20/32/24) are explicit in the design."
  - "The Ruin set rarity (Gray) was corrected to the designed Green on all three pieces; the item values already matched the design 1 gold."
  - "Set-bonus mechanics were claimed as matching only after inspecting UpdateArmorSet and the runtime buffs; the absent player.setBonus display text and the Witherbark minion projectile mismatch were recorded as blockers instead of force-fixed outside the modify set."
  - "Photophore is a copy of JadeFruit (permanent +5 life, 10 uses, PermanentBoosters category) rather than the designed light pet; implementing the pet would add new buff/projectile content and art, so it is recorded as an 效果 blocker."
  - "Plan 05 owns 01-INVENTORY.md regeneration; only 01-INVENTORY.json was edited here, and check-inventory-reconciliation.ps1 still sees 103 matrix rows."

requirements-completed: [QUAL-05]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "The 14 tranche-B armor classes either match their design row in defense, piece effect, set bonus, recipe, and value/rarity or carry a blocker naming the unreconciled field: 4 missing DevilHeart recipes added, 3 Ruin rarities corrected, 1 category fix, 1 comment fix, 3 set-bonus blockers."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "check-inventory-reconciliation.ps1 -> OK(0): 103 entries; dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 errors; AGENTS.md BOM check passed"
        status: pass
    human_judgment: false
  - id: D2
    description: "The 6 tranche-B placeable/pet/booster classes match their design rows or carry a blocker: DevilHeartIronOre/JadeizedBone Green rarity, AlgaeExtractor 2 gold, JadeFruit 25 silver, and a Photophore 效果 blocker; JadeLakeRedAlgae already matched."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "check-inventory-reconciliation.ps1 -> OK(0): 103 entries; dotnet build -> 0 errors"
        status: pass
    human_judgment: false
  - id: D3
    description: "All seven listed placeable _Item classes declare Placeables, and no ModItem under the tranche-B folders defaults to Items."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "filesystem scan of Armors/Placeables/Pets/PermanentBoosters -> every ModItem overrides LocalizationCategory; none uses base.LocalizationCategory; dotnet build -> 0 errors"
        status: pass
    human_judgment: false
  - id: D4
    description: "scripts/check-tranche-B.ps1 selects phase==1 + tranche=='B' + artwork_complete==true, requires a tracked .cs class or a non-no-repo blocker per entry, and guards against added/modified .png under the items tree; it reports 20 covered of 20 selected."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "powershell -File scripts/check-tranche-B.ps1 -> OK(0): tranche-B covered entries = 20 (of 20 selected); ASCII/no-BOM/LF verified"
        status: pass
    human_judgment: false
  - id: D5
    description: "Judgment call: the Witherbark minion attack pattern, the absent set-bonus display text on Witherbark/Molluscs/Ruin, and the Photophore misimplementation were recorded as blockers rather than fixed by expanding the plan's file scope into projectiles/buffs/localization."
    requirement: QUAL-05
    verification: []
    human_judgment: true
    rationale: "Whether to accept the blocker disposition (versus expanding this plan into projectile/localization work) is a scoping judgment for the verifier/user."

# Metrics
duration: ~19min
completed: 2026-09-12
status: complete
---

# Phase 1 Plan 04: Completed-Art Tranche B (Armors, Placeables, Pets, Boosters) Parity Summary

**Brought the 20 Phase 1 completed-art tranche-B entries to design parity — 4 missing DevilHeart armor recipes added, the Ruin set rarity corrected to Green, seven placeables classified, and the JadeFruit/AlgaeExtractor values fixed — and gated the tranche with `check-tranche-B.ps1` (20 covered of 20 selected).**

## Performance

- **Duration:** ~19 min
- **Tasks:** 3
- **Files modified:** 21 (1 created, 20 modified)
- **Commits:** 3

## Accomplishments

- **Design rows resolved header-anchored.** The tranche-B design rows were read from the committed `evidence/item.xml` snapshots by their `<thead>` headers (`护具/贴图/代码/数值/价格/稀有度/效果/套装效果` for the three armor tables; the 12-column item tables for the pets/boosters/placeables), never by cell position. The recipe embedded after `=` in the 效果 cell was interpreted as the recipe, consistent with the plan-02 `DevilHeartIronBar` furnace recipe.
- **DevilHeart armor parity.** The four pieces already matched defense (1/1/4/3), value (60 silver), rarity (Green), the piece effects (+5% magic/+5% magic crit, +4% summon/+1 minion, +60 mana/+6% magic & summon, +10% move), and the general/mage/summon set bonuses (the `DevilHeartSetBuff` gives +6% magic damage & crit, +1 minion, no mana regen, -15% defense for 10 s on a 35 s cooldown; the Hairpin and Helmet supply the mage/summon variants and the localized `SetBonus`). The only deviation was the **four missing recipes** (Devil Heart Iron Bar x18/20/32/24 at an Anvil), which were added. `DevilHeartHairpin` was also misclassified via `base.LocalizationCategory` (default `Items`) and now declares `Armor`.
- **Ruin set parity.** Defense (1/3/1), the distributed effects (+1 minion +40 mana on the Mask; +6% summon, -10% mana on the Robe; +4% summon, +2% damage reduction on the Leggings), the 3-piece `+1 minion` bonus, the hotkey set bonus (`RuinSetBuff`: +10% summon damage, +25% move speed, 30 s / 120 s), and the 1 gold value all matched. The **rarity was Gray on all three pieces** and was corrected to the designed **Green**.
- **Witherbark and Molluscs sets.** Defense, values, rarity, piece effects, `IsArmorSet` membership, and the mechanical set bonuses all matched (Witherbark: +3 minions, -30% summon damage, guard summon; Molluscs: `MolluscsSetBuff` + head warrior/ranger variants). The stale `ShellMolluscsBreastPlate` constants' comments (25 s / 40 s mislabelled 10 s / 35 s) were corrected. Two genuine gaps were **recorded as blockers** rather than force-fixed: the Witherbark guard's attack pattern (design: 14 damage spikes every 30 frames; wired: a blink/dash plus an 8-leaf burst in `Projectiles/Summon`, outside this task's file set) and the absent `player.setBonus` display text on Witherbark/Molluscs/Ruin (no localization key exists and plan 05 forbids hand-creating keys).
- **Placeable/pet/booster parity.** `DevilHeartIronOre_Item` and `JadeizedBone_Item` were set to the designed Green rarity; `AlgaeExtractor_Item` received the designed 2-gold value; `JadeFruit` was corrected from 15000 to the designed 25-silver (2500); `JadeLakeRedAlgae_Item` already matched (value 50, White). **`Photophore` is a copy of `JadeFruit`** (permanent +5 life, 10 uses, `PermanentBoosters`) instead of the designed light pet, so it carries an `效果` blocker — implementing the pet would add new buff/projectile content and require art that does not exist.
- **Placeable categories.** The seven unclassified placeable `_Item` classes (`GreenCourtBrick`, `AgedGreenCourtBrick`, `WaterErodedGreenBrick`, `ForestRainVineTile_Thick`, `ForestRainVineTile_Thin`, `Walls/AgedGreenCourtWall`, `DecayingWoodCourt/CrackedForestThrone`) now declare `LocalizationCategory` `Placeables`. A filesystem scan confirms every `ModItem` under the tranche-B folders overrides the property.
- **Tranche-B gate.** New `scripts/check-tranche-B.ps1` (no parameters, 100% ASCII, `[IO.File]::ReadAllText`) selects `phase == 1` + `tranche == "B"` + `artwork_complete == true`, requires a tracked `.cs` class or a non-no-repo blocker per entry, and fails on any added/modified `.png` under the items tree. It reports **20 covered of 20 selected**.
- **Verification:** `check-inventory-reconciliation.ps1` exits 0 (103 entries; matched=60; green=58 yellow=20 unchecked=25; labels=5; deferred=21; assumptions=7); `dotnet build /p:Configuration=Release /p:WarningLevel=0` -> 0 warnings, 0 errors; the AGENTS.md byte-level UTF-8 BOM check passed (1291 files); no `.png`/binary change under the Kelp Curtain items tree.

## Task Commits

Each task was committed atomically:

1. **Task 1: Complete tranche-B armor completed-art design parity (D-12)** - `22bee25ed` (feat)
2. **Task 2: Complete tranche-B placeable/pet/booster parity and fix placeable categories** - `c7da91926` (feat)
3. **Task 3: Gate tranche B with a coverage and no-placeholder check** - `bf6303649` (feat)

**Plan metadata:** (docs: complete plan - committed separately)

## Files Created/Modified

- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` - 4 blocker strings added: `WitherbarkHelmet` (2), `ShellMolluscsBreastPlate` (1), `RuinMask` (1), `Photophore` (1). All other fields preserved; still 103 entries.
- `Sources/.../Armors/DevilHeart/DevilHeartHairpin.cs` - `LocalizationCategory` `Items` -> `Armor`; added the 18-bar Anvil recipe.
- `Sources/.../Armors/DevilHeart/DevilHeartHelmet.cs` - added the 20-bar Anvil recipe.
- `Sources/.../Armors/DevilHeart/DevilHeartLightBreastPlate.cs` - added the 32-bar Anvil recipe.
- `Sources/.../Armors/DevilHeart/DevilHeartLeggings.cs` - added the 24-bar Anvil recipe.
- `Sources/.../Armors/Ruin/RuinMask.cs`, `RuinMagicRobe.cs`, `RuinLeggings.cs` - `Item.rare` `Gray` -> `Green`.
- `Sources/.../Armors/Molluscs/ShellMolluscsBreastPlate.cs` - buff/cooldown comments corrected to 25 s / 40 s.
- `Sources/.../Placeables/DevilHeartIronOre_Item.cs`, `JadeizedBone_Item.cs` - added `Item.rare = ItemRarityID.Green`.
- `Sources/.../Placeables/AlgaeExtractor_Item.cs` - added `Item.value = 20000`.
- `Sources/.../PermanentBoosters/JadeFruit.cs` - `Item.value` 15000 -> 2500.
- Seven `..._Item.cs` files under `Placeables/` and `Placeables/Walls/`/`DecayingWoodCourt/` - added `LocalizationCategory` `Placeables`.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-tranche-B.ps1` (new) - tranche-B coverage and no-placeholder gate.

## Decisions Made

- **Blocker over force-fix:** the Witherbark minion attack pattern, the absent set-bonus display text, and the Photophore misimplementation are recorded as blockers because the fixes live in `Projectiles/`, `Buffs/`, and localization HJSON outside the plan's declared modify set, and plan 05 forbids hand-creating localization keys.
- **Recipe interpretation:** the `=魔心铁锭*N 铁砧 附近` suffix of a 效果 cell is the recipe (confirmed by the plan-02 `DevilHeartIronBar` precedent), so the four missing armor recipes were added.
- **MD scope:** only `01-INVENTORY.json` was edited; `01-INVENTORY.md` remains plan 05's responsibility, so the reconciliation gate's 103-row check is unaffected.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2 - Missing Critical] Four DevilHeart armor recipes were absent (D-12)**
- **Found during:** Task 1
- **Issue:** the design rows R0-R3 of the 魔心铁 armor table specify crafting recipes (魔心铁锭 x18/20/32/24 at an Anvil) embedded after `=` in the 效果 cell; none of the four classes had `AddRecipes`.
- **Fix:** added an `AddRecipes` override to each piece using `DevilHeartIronBar_Item` at `TileID.Anvils`.
- **Files modified:** `DevilHeartHairpin.cs`, `DevilHeartHelmet.cs`, `DevilHeartLightBreastPlate.cs`, `DevilHeartLeggings.cs`
- **Verification:** Release build 0 errors.
- **Committed in:** `22bee25ed`

**2. [Rule 2 - Missing Critical] `DevilHeartHairpin` defaulted to the `Items` localization category**
- **Found during:** Task 1
- **Issue:** `LocalizationCategory => base.LocalizationCategory` resolves to the default `Items`, so the exporter would log it as unclassified (violating the tranche-B "no ModItem defaults to Items" criterion).
- **Fix:** changed to `LocalizationUtils.Categories.Armor`.
- **Files modified:** `DevilHeartHairpin.cs`
- **Verification:** filesystem scan finds no default-Items ModItem under the tranche-B folders; Release build 0 errors.
- **Committed in:** `22bee25ed`

**3. [Rule 1 - Bug] Ruin set rarity and material/booster values deviated from the design**
- **Found during:** Tasks 1-2
- **Issue:** the three Ruin pieces used `ItemRarityID.Gray` instead of the designed 绿色; `DevilHeartIronOre`/`JadeizedBone` defaulted to White instead of Green; `AlgaeExtractor` had no value (design 2 gold); `JadeFruit` was valued 15000 instead of the designed 25 silver.
- **Fix:** corrected each to the design value.
- **Files modified:** `RuinMask.cs`, `RuinMagicRobe.cs`, `RuinLeggings.cs`, `DevilHeartIronOre_Item.cs`, `JadeizedBone_Item.cs`, `AlgaeExtractor_Item.cs`, `JadeFruit.cs`
- **Verification:** Release build 0 errors.
- **Committed in:** `22bee25ed`, `c7da91926`

**4. [Rule 1 - Bug] Seven placeables defaulted to the `Items` category**
- **Found during:** Task 2
- **Issue:** RESEARCH Pitfall 5 confirmed these seven lacked `LocalizationCategory`, so the exporter would misplace their keys.
- **Fix:** added `LocalizationCategory` `Placeables`, matching the nearest local spelling.
- **Files modified:** the seven placeable `_Item` classes.
- **Verification:** filesystem scan confirms all override the property; Release build 0 errors.
- **Committed in:** `c7da91926`

**5. [Rule 1 - Bug] Stale `ShellMolluscsBreastPlate` constant comments**
- **Found during:** Task 1
- **Issue:** the 25 s / 40 s design values were commented "10 seconds" / "35 seconds" (copied from the DevilHeart plate).
- **Fix:** corrected the comments to 25 s / 40 s (values were already correct).
- **Files modified:** `ShellMolluscsBreastPlate.cs`
- **Verification:** Release build 0 errors.
- **Committed in:** `22bee25ed`

**6. [Rule 2 - Missing Critical] Unreconciled set-bonus/effect behavior outside the modify set**
- **Found during:** Tasks 1-2
- **Issue:** the Witherbark guard attack pattern, the Witherbark/Molluscs/Ruin set-bonus display text, and the Photophore light-pet behavior cannot be fixed inside the declared class files (they need projectile/buff/localization work or new art).
- **Fix:** recorded one blocker per affected entry naming the unreconciled field (6 blocker strings across 4 entries), as the plan's action permits for prose/unresolvable deviations.
- **Files modified:** `01-INVENTORY.json`
- **Verification:** `check-inventory-reconciliation.ps1` exits 0; `check-tranche-B.ps1` counts all 20 as covered.
- **Committed in:** `22bee25ed`, `c7da91926`

---

**Total deviations:** 6 auto-fixed (5 Rule 1/2 within scope, 1 blocker-disposition).
**Impact on plan:** No scope creep outside the declared armor/placeable class files, the inventory, and the new gate; the blocker disposition keeps the tranche honest instead of claiming false parity.

## Known Stubs

| Stub | File | Reason |
|------|------|--------|
| Photophore implements the Jade Fruit permanent-booster behavior instead of the designed light pet | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Pets/Photophore.cs` | Recorded as an `效果` blocker in `01-INVENTORY.json`; implementing the pet is new buff/projectile content and the required pet art does not exist (no placeholder art is permitted). |
| Witherbark set-bonus summoned guard attack pattern differs from the design | `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/WitherbarkMinion.cs` | Recorded as a `套装效果` blocker; the fix lives in projectile files outside plan 04's modify set. |
| Set-bonus display text (`player.setBonus`) absent on the Witherbark/Molluscs/Ruin sets | the three armor sets | Recorded as `套装效果` blockers; plan 05 forbids hand-creating localization keys and the exporter does not generate `SetBonus`. |

## Threat Flags

| Flag | File | Description |
|------|------|-------------|
| threat_flag: none | - | No new network endpoint, auth path, file-access pattern, or trust-boundary schema change was introduced; the only new file is a read-only PowerShell gate. |

## Issues Encountered

- The initial `evidence/item.xml` `Read`/console output was mojibake because PowerShell decoded the UTF-8 snapshot as the ANSI code page; re-reading with `[IO.File]::ReadAllText(..., UTF8)` resolved it, and the header-anchored extraction confirmed the three armor tables and the five item tables.
- `Photophore.cs` and `JadeFruit.cs` are byte-identical apart from the class name/namespace; the `Photophore` summary comment (绿琉璃果) confirmed it is a copy-paste defect. It is blocked rather than rewritten because the design target is a light pet.

## User Setup Required

None - no external service configuration, no packages or secrets; no Feishu write was made.

## Next Phase Readiness

- Tranche B is gated: `check-tranche-B.ps1` (20 covered) plus the inventory reconciliation checker and the Release build.
- Plan 05 receives: the five new blocker strings (Photophore 效果; Witherbark/Molluscs/Ruin 套装效果) and the plan-04 deviation ledger for `01-DEVIATIONS.md`; it owns `01-INVENTORY.md` regeneration and the both-culture localization coverage.
- Blockers carried forward: the Photophore misimplementation, the Witherbark minion pattern, the missing set-bonus display text, the four plan-03 tranche-A effect blockers, and the RadialCarapace/VineRepairWand artwork blockers (Phase 1 tracked).

---

*Phase: 01-item-inventory-completed-art-items*
*Completed: 2026-09-12*

## Self-Check: PASSED

- All declared files exist (20 class files plus `check-tranche-B.ps1` and `01-INVENTORY.json`).
- Task commits `22bee25ed`, `c7da91926`, `bf6303649` exist in git history.
- `check-inventory-reconciliation.ps1` and `check-tranche-B.ps1` exit 0; Release build 0 errors; BOM check passed.
