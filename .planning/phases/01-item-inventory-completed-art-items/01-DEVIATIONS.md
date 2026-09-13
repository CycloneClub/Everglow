# Phase 1 Deviation Ledger — Item Inventory & Completed-Art Items

Generated: 2026-09-12
Sources merged: `01-03-SUMMARY.md` and `01-04-SUMMARY.md` deviation ledgers, plus the plan-05 moved developer-tool category fixes.
Machine mirror: `01-INVENTORY.json` `deviations[]` (design deviations) and `phase1_actions[]` (process/routing actions).

This ledger records every deviation discovered and fixed while bringing the completed-art tranche to design parity (D-10/D-12). It is the audit trail QUAL-05 requires: no field below was changed by editing the Feishu design source, and no localization key listed here was renamed or removed.

## Design Deviations

Columns: entry id, class file, field, before, after, source (design header / row block id).

| Entry id | Class file | Field | Before | After | Source (header / row block id) |
| --- | --- | --- | --- | --- | --- |
| `item-weapons.misc-thorn-turtle-shell` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/ThornTurtleShell.cs` | `maxRunSpeed` | `-5%` run speed | `-10%` run speed (design `-10% 冲刺速度`) | item table `HkdvdyBRroWjHJxNY4YcXCTsnRb` row `RCQWdF1jLoM91rxt4F0chgaenYf` |
| `item-armor-魔心发簪` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartHairpin.cs` | `LocalizationCategory` | `Items` (base default) | `Items.Armor` | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `Z2hqdY9tJo6qifx7EzmcRZhXn4g` |
| `item-armor-魔心发簪` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartHairpin.cs` | `AddRecipes` | none | Anvil recipe: Devil Heart Iron Bar x18 | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `Z2hqdY9tJo6qifx7EzmcRZhXn4g` |
| `item-armor-魔心笼盔` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartHelmet.cs` | `AddRecipes` | none | Anvil recipe: Devil Heart Iron Bar x20 | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `LUhDdMyp5orzkYxg1atcbUbvnah` |
| `item-armor-魔心轻甲` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartLightBreastPlate.cs` | `AddRecipes` | none | Anvil recipe: Devil Heart Iron Bar x32 | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `Q5FAdxhPuoquJ3xL5HzcxiMDnwd` |
| `item-armor-魔心靴` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartLeggings.cs` | `AddRecipes` | none | Anvil recipe: Devil Heart Iron Bar x24 | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `RGyUdRbWUokxLKx0DwgcRgMgnTe` |
| `item-weapons.summon-ruin-mask` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinMask.cs` | `Item.rare` | `Gray` | `Green` (design 绿色) | item table `doxcnK1twRSRyYrOvXg4tyBBkxf` row `doxcnkJfbN3VxSujhhJE4JsAdjd` |
| `item-weapons.summon-ruin-magic-rob` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinMagicRobe.cs` | `Item.rare` | `Gray` | `Green` (design 绿色) | item table `doxcnK1twRSRyYrOvXg4tyBBkxf` row `doxcnFTjXG53qDd8GZW4qgWcryh` |
| `item-weapons.summon-ruin-leggings` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinLeggings.cs` | `Item.rare` | `Gray` | `Green` (design 绿色) | item table `doxcnK1twRSRyYrOvXg4tyBBkxf` row `doxcnyG6nOqRzArF3PJhsH7ZuUb` |
| `item-armor-devil-heart-iron-ore` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/DevilHeartIronOre_Item.cs` | `Item.rare` | White (unset) | `Green` | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `HtPMdRS6xoH9hmxahBGcYPSbnyf` |
| `item-armor-jadeized-bone` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/JadeizedBone_Item.cs` | `Item.rare` | White (unset) | `Green` | armor table `TK7ddgDgMosNN8xzvs1cS0H0nBH` row `WIJxdBV94oNHRhxq9jdcN3qpnHb` |
| `item-weapons.misc-藻类提取机` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/AlgaeExtractor_Item.cs` | `Item.value` | unset (0) | 20000 (2 gold) | item table `doxcnlTJMIxnmaRlOltbxZDYzGc` row `doxcndsOZ8HadK2TCsufoaQ9fpJ` |
| `item-weapons.misc-绿琉璃果` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/PermanentBoosters/JadeFruit.cs` | `Item.value` | 15000 | 2500 (25 silver) | item table `doxcn8LaEaSegCa81vMthGjpd1d` row `doxcnqF2ygEU5utPUl6ucSJnLcg` |
| `item-armor-软体外壳护甲` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Molluscs/ShellMolluscsBreastPlate.cs` | buff/cooldown comments | 10 s / 35 s | 25 s / 40 s (values already correct) | armor table `SgUPdGf2uoUWVlxMY2Qc8Yc5nrb` row `G8NadCO4uoVhJdxxXNgcKVJun3B` |
| `GreenCourtBrick_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/GreenCourtBrick_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `AgedGreenCourtBrick_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/AgedGreenCourtBrick_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `WaterErodedGreenBrick_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/WaterErodedGreenBrick_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `ForestRainVineTile_Thick_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/ForestRainVineTile_Thick_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `ForestRainVineTile_Thin_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/ForestRainVineTile_Thin_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `AgedGreenCourtWall_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/Walls/AgedGreenCourtWall_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `CrackedForestThrone_Item` (repo class; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/DecayingWoodCourt/CrackedForestThrone_Item.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Placeables` | repo placeable; no design row |
| `GenerateMazeRoom` (repo developer tool; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/GenerateMazeRoom.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Tools` | repo developer tool; no design row |
| `ResetIsleOfBloom` (repo developer tool; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/ResetIsleOfBloom.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Tools` | repo developer tool; no design row |
| `ResetKelpCurtain` (repo developer tool; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/ResetKelpCurtain.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Tools` | repo developer tool; no design row |
| `UnderWaterDungeon` (repo developer tool; no design row) | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/UnderWaterDungeon.cs` | `LocalizationCategory` | `Items` (unset) | `Items.Tools` | repo developer tool; no design row |

Total fixed design deviations: 25 (14 on design-row-backed entries, 11 repo-only classification fixes moved to plan 05 from plan 04).

### Parser Category Discrepancy (recorded in plan 01-06, not fixed)

The parser's category heuristic set `item-weapons.ranged-魁札尔的愿望` to `weapons.ranged`, but the design row (`doxcnD2XMx9mBf5DL0hUqX98vqb`, table `doxcnK1twRSRyYrOvXg4tyBBkxf`) is Melee (`mel37`, 巨刃类武器). `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs` is implemented as Melee; the entry `id` and inventory `category` field are left unchanged because the id is a compatibility-sensitive key. Machine mirror: `01-INVENTORY.json` `deviations[]` (`field: category`, `plan: 01-06`).

## Deferred Localization (User Directive)

Status: **deferred**. Reason: `localization deferred by user directive (2026-09-12)`.

Plan 05 Task 2 was a `checkpoint:human-action` whose only source of missing keys is the in-game `OutputLocalizationHjsonItem` exporter (a consumable `ModItem` fired from a running tModLoader client). The user directed that localization not be considered for Phase 1 — "记录：不考虑本地化，把代码部分完成即可" (record it; do not consider localization; just complete the code portion). The exporter was therefore **not run**, no key was fabricated, and **no `Localization/**/*.hjson` file was hand-edited** (AGENTS.md forbids hand-created classification keys and the plan's T-05-01 mitigation forbids hand-classification).

`scripts/check-localization-coverage.ps1 -AllowMissing` selects **63** Phase 1 completed-art entries (`phase == 1`, `artwork_complete == true`, non-empty `internal_name`, `localization.blocked != true`) and reports **45 covered / 18 missing**. The 18 entries below are recorded with `status: deferred` and the reason above; the same 18 records are mirrored in `01-INVENTORY.json` `deviations[]`, and the deferral is recorded as `phase1_actions[]` P1A-11 and P1A-13. The strict coverage gate (no switch) consequently fails by design; Phase 1 treats both-culture key coverage as advisory per the directive.

| Entry id | Class file | Missing cultures | Status | Reason |
| --- | --- | --- | --- | --- |
| `item-weapons.melee-evil-halbert-barnacle` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/EvilHalbertBarnacle.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-arc-i` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArcI.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-crimson-moon-algae-magic-staff` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicStaff.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-crimson-moon-algae-spell-book` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicSpellBook.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-crimson-moon-algae-whip` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicWhip.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-crimson-moon-sap` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/CrimsonMoonSap.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-empty-water-staff` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/EmptyWaterStaff.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-jade-lake-red-algae` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/JadeLakeRedAlgae_Item.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-photophore` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Pets/Photophore.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-青须手杖` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/GreenSungloStaff.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.summon-activated-dog-staff` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ActivatedDogStaff.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.summon-crimson-moon-algae-gyroscope` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMinionGyroscope.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.summon-crimson-moon-algae-summon-staff` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMinionStaff.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-arm-of-giant-tree` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-森林之息` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-厄佛提根的净化粉末` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/ElftigernPowder.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.misc-枯萎面具` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/WitheredMask.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |
| `item-weapons.ranged-魁札尔的愿望` | `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs` | en-US, zh-Hans | deferred | localization deferred by user directive (2026-09-12) |

Deferred localization deviations: 18 (13 from plan 01-05; 5 carried over from plan 01-06). These are **not** marked `localization.blocked` — blocking is the plan's mechanism for a post-exporter failure with a named missing culture; this is an explicit user-directed deferral, recorded as deferred so the strict gate stays honestly red and the advisory `-AllowMissing` baseline is the Phase 1 evidence.

## Tranche Advances (ITEM IDs preserved for Phase 2)

The inventory entry `advances` arrays were not modified by plans 03/04/05; they remain visible in `01-INVENTORY.json` for Phase 2 to consume:

- **Tranche A (38 completed-art entries):** `ITEM-01`, `ITEM-02` by item type, with procurement overrides to `ITEM-03`/`ITEM-04`, plus `ITEM-07` (localization) on every entry.
- **Tranche B (20 completed-art entries):** `ITEM-01`, `ITEM-02` by item type, with procurement overrides to `ITEM-03`/`ITEM-04`, plus `ITEM-07` (localization) on every entry.
- **Biology drops and class-less rows:** carry the same `ITEM-01`/`ITEM-07` (and type-appropriate) IDs but are `phase = 2`.

## Planned Blockers (recorded, not fixed here)

These are recorded as entry `blockers` rather than fixed, because the fixes live outside the item-class modify set (projectiles/buffs/localization) or would require new art:

- Tranche A effect (效果) blockers: `MossySpell`, `CyatheaArrow`, `GreenSungloStaff`, `EvilHalbertBarnacle`.
- Tranche B set-bonus/effect blockers: `Photophore` (效果, implemented as a Jade Fruit copy instead of the designed light pet); `WitherbarkHelmet`, `ShellMolluscsBreastPlate`, `RuinMask` (套装效果 display text / minion attack pattern).
- Phase 1 tracked artwork blockers (D-11): `RadialCarapace`, `VineRepairWand` (code-complete, artwork-missing).

## Phase 1 Actions

See `01-INVENTORY.json` `phase1_actions[]` for the machine-readable list. Summary:

1. Retained every entry's `advances` ITEM IDs for Phase 2.
2. Routed all 43 class-less entries to `phase = 2` (18 completed-art ones additionally `deferred = true` with a reason).
3. Kept `RadialCarapace` and `VineRepairWand` at `phase = 1` per D-11.
4. Recorded the tranche-A and tranche-B effect/recipe/set-bonus blockers.
5. Classified the four `Tools/Developer` items as `Items.Tools` before the exporter run.
6. Captured the pre-exporter localization baseline in `01-05-SUMMARY.md`.
7. Consolidated the plan 03/04 deviation ledgers into this file and the inventory.
8. Deferred Phase 1 localization by user directive (2026-09-12); recorded the 13 missing-key entries as deferred deviations — no key was fabricated and no HJSON file was edited.
9. Extended P1A-11 to the five plan 01-06 carry-over entries (P1A-13); the deferred-localization set is now 18 (45/63 covered).

## Status-Colour Rule Correction (2026-09-12)

Factual error corrected after Phase 1 review: the composite `status` had been computed as `green` when both Feishu checkboxes are complete, `yellow` when exactly one is complete, and `unchecked` when neither is complete. That contradicts the authoritative rule in `PROJECT.md` "Design Status Synchronization" (lines 67-70).

Corrected rule (now enforced by `parse-design-xml.ps1` `Get-Status`, `validate-inventory.ps1`, and `check-inventory-reconciliation.ps1`, and restated in `.planning/ROADMAP.md`, `.planning/REQUIREMENTS.md`, `01-CONTEXT.md` D-06, `01-RESEARCH.md`, and `01-INVENTORY.md`):

- A status colour applies **only** when **both** `artwork_complete` and `code_complete` are true.
- **Green:** both complete with an exact design match (no conflict/known-exception blocker).
- **Yellow:** both complete but a content/description conflict or known exception is recorded as a blocker.
- **Unchecked (no colour):** either checkbox incomplete, whatever the repository asset state.

Recomputed `01-INVENTORY.json` / `01-INVENTORY.md`: green 50, yellow 8, unchecked 45 (previously green 58, yellow 20, unchecked 25). The 8 both-complete entries carrying tranche effect/set-bonus blockers are now yellow: `MossySpell`, `CyatheaArrow`, `GreenSungloStaff`, `EvilHalbertBarnacle`, `Photophore`, `WitherbarkHelmet` (`枯木头盔`), `ShellMolluscsBreastPlate` (`软体外壳护甲`), `RuinMask`. The 8 Markdown rows whose Blockers cell had not been refreshed after the plan-03/04 blocker edits were re-synced from the JSON at the same time.

The earlier gate lines in the `01-01`/`01-02`/`01-03`/`01-04`/`01-05` SUMMARY.md files that printed `green=58 yellow=20 unchecked=25` record the superseded computation and are left as historical execution evidence.

## Allocation Correction (2026-09-12)

Corrects the routing recorded by `phase1_actions[]` P1A-03 ("Route class-less entries to Phase 2"). P1A-03 is **superseded**: class-less status never defers an entry. Allocation is now two-step: (1) design section -> phase (non-boss item tables stay in the item phases; the Giant Winged Dragon and Klein Snake sections belong to Phase 7 via ITEM-06/ITEM-05); (2) within the item phases, Feishu artwork state -> Phase 1 (complete) or Phase 2 (incomplete). Of the 18 artwork-complete class-less entries, **5 are Phase 1 carry-over** and **13 are Phase 7**. Phase counts: `{1: 65, 2: 25, 7: 13}` (was `{1: 60, 2: 43}`). The 25 remaining `phase = 2` entries are artwork-incomplete. Machine record: `01-INVENTORY.json` `phase1_actions[]` P1A-12.

### Phase 1 Carry-Over (5) — implemented in plan 01-06

| Entry id | Name (zh) | Evidence section | Advances | Class / texture |
| --- | --- | --- | --- | --- |
| `item-weapons.misc-arm-of-giant-tree` | 巨树之臂 | `item.xml` H2 水下宝库物品, table `doxcnnJSAMpWWmUqUwHGBjnoX3d` | ITEM-03 | `ArmOfGiantTree.cs` / repo texture exists |
| `item-weapons.misc-森林之息` | 森林之息 | `item.xml` H2 水下迷宫宝箱物品, table `doxcnlTJMIxnmaRlOltbxZDYzGc` | ITEM-03 | `ForestBreath.cs` / no repo texture (artwork blocker) |
| `item-weapons.misc-厄佛提根的净化粉末` | 厄佛提根的净化粉末 | `item.xml` H2 NPC交易物品, table `doxcnK1twRSRyYrOvXg4tyBBkxf` | ITEM-04 | `ElftigernPowder.cs` / repo texture exists |
| `item-weapons.misc-枯萎面具` | 枯萎面具 | `item.xml` H2 NPC交易物品, table `doxcnK1twRSRyYrOvXg4tyBBkxf` | ITEM-04 | `WitheredMask.cs` / no repo texture (artwork blocker) |
| `item-weapons.ranged-魁札尔的愿望` | 魁札尔的愿望 | `item.xml` H2 NPC交易物品, table `doxcnK1twRSRyYrOvXg4tyBBkxf` | ITEM-04 | `QuetzalsWish.cs` / no repo texture (artwork blocker) |

### Phase 7 Correction (13) — no implementation in Phase 1/2

The 9 Giant Winged Dragon rows come from the biology-design section `特殊：/巨翼龙` (tail-kill text and the 掉落物 / 可制作装备 tables) and advance **ITEM-06**; the 4 Klein Snake rows come from the item-design H1 `克莱因蛇系列` (table `N4W9drIpio4C86xx8rDc04JJn3e`) and advance **ITEM-05**. Neither group is Phase 1 or Phase 2 scope.

| Entry id | Name (zh) | Owner / mapping |
| --- | --- | --- |
| `biology_drop-weapons.melee-龙骸巨块大剑` | 龙骸巨块大剑 | Giant Winged Dragon tail-kill drop -> ITEM-06 |
| `biology_drop-weapons.misc-血肉聚合物` | 血肉聚合物 | Giant Winged Dragon tail-kill drop -> ITEM-06 |
| `biology_drop-weapons.melee-狂战士角盔` | 狂战士角盔 | Giant Winged Dragon tail-kill drop -> ITEM-06 |
| `biology_drop-weapons.melee-狂战士板甲` | 狂战士板甲 | Giant Winged Dragon tail-kill drop -> ITEM-06 |
| `biology_drop-weapons.melee-狂战士胫甲` | 狂战士胫甲 | Giant Winged Dragon tail-kill drop -> ITEM-06 |
| `biology_drop-weapons.misc-巨翼龙圣物-大师` | 巨翼龙圣物（大师） | Giant Winged Dragon relic -> ITEM-06 |
| `biology_drop-weapons.misc-巨翼龙纪念章` | 巨翼龙纪念章 | Giant Winged Dragon medal -> ITEM-06 |
| `biology_drop-weapons.magic-崩解阈限` | 崩解阈限 | Giant Winged Dragon craftable -> ITEM-06 |
| `biology_drop-weapons.melee-骇翼链剑` | 骇翼链剑 | Giant Winged Dragon craftable -> ITEM-06 |
| `item-weapons.melee-碧绿玉髓扇` | 碧绿玉髓扇 | Klein Snake series -> ITEM-05 |
| `item-weapons.ranged-龙骨猎枪` | 龙骨猎枪 | Klein Snake series -> ITEM-05 |
| `item-weapons.melee-碧玉弯刀` | 碧玉弯刀 | Klein Snake series -> ITEM-05 |
| `item-weapons.summon-魂蛇手杖` | 魂蛇手杖 | Klein Snake series -> ITEM-05 |

## Carry-Over Gate (Plan 01-06)

Plan 01-06 closes the Phase 1 carry-over:

- **Allocation:** 18 artwork-complete class-less entries split **5 Phase 1 / 13 Phase 7**; phase counts `{1: 65, 2: 25, 7: 13}`. No `scripts/parse-design-xml.ps1` re-run (CR-01); the inventory was edited in place and P1A-12 records the correction.
- **Implemented (5):** `ArmOfGiantTree` (charge smash + shockwave; repo texture reused), `ElftigernPowder` (material; purification gated to GAME-03/Phase 6), `ForestBreath` (quest item; underwater-maze chest), `WitheredMask` (vanity), `QuetzalsWish` (melee giant blade; design row Melee vs parser `weapons.ranged`).
- **Artwork blockers (3):** `ForestBreath`, `WitheredMask` and `QuetzalsWish` carry "approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending" and `repo_asset` empty. They reuse the existing shared `Commons.ModAsset.White_Mod` fallback (the `RadialCarapace` precedent) so the mod still loads; **no placeholder art was created and no `.png`/binary asset was added or modified**.
- **Effect blockers:** `ElftigernPowder` (Wilted Zone restoration, GAME-03/Phase 6) and `QuetzalsWish` (four-stage combo, charged throw/explosion, wound debuff need absent projectile/buff/VFX assets).
- **Phase 2 remainder:** the 25 remaining `phase == 2` entries are artwork-incomplete; none are Phase 1 carry-over.
- **Localization:** still deferred by the 2026-09-12 user directive. The five new entries are added to the deferred ledger (P1A-13); `check-localization-coverage.ps1 -AllowMissing` now selects 63 entries (45 covered / 18 missing). No key was fabricated and no HJSON file was hand-edited.
- **Gates:** `validate-inventory.ps1` (103), `check-inventory-reconciliation.ps1` (103, deferred=3), `check-tranche-A.ps1` (43/43), `check-tranche-B.ps1` (20/20), `check-carryover.ps1` (5/5), `check-localization-coverage.ps1 -AllowMissing` (advisory), `dotnet build /p:Configuration=Release /p:WarningLevel=0` (0 errors), byte-level BOM check, and the no-placeholder `.png` guard all pass.
