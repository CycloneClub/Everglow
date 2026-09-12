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

## Deferred Localization (User Directive)

Status: **deferred**. Reason: `localization deferred by user directive (2026-09-12)`.

Plan 05 Task 2 was a `checkpoint:human-action` whose only source of missing keys is the in-game `OutputLocalizationHjsonItem` exporter (a consumable `ModItem` fired from a running tModLoader client). The user directed that localization not be considered for Phase 1 — "记录：不考虑本地化，把代码部分完成即可" (record it; do not consider localization; just complete the code portion). The exporter was therefore **not run**, no key was fabricated, and **no `Localization/**/*.hjson` file was hand-edited** (AGENTS.md forbids hand-created classification keys and the plan's T-05-01 mitigation forbids hand-classification).

`scripts/check-localization-coverage.ps1 -AllowMissing` selects **58** Phase 1 completed-art entries (`phase == 1`, `artwork_complete == true`, non-empty `internal_name`, `localization.blocked != true`) and reports **45 covered / 13 missing**. The 13 entries below are recorded with `status: deferred` and the reason above; the same 13 records are mirrored in `01-INVENTORY.json` `deviations[]`, and the deferral is recorded as `phase1_actions[]` P1A-11. The strict coverage gate (no switch) consequently fails by design; Phase 1 treats both-culture key coverage as advisory per the directive.

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

Deferred localization deviations: 13. These are **not** marked `localization.blocked` — blocking is the plan's mechanism for a post-exporter failure with a named missing culture; this is an explicit user-directed deferral, recorded as deferred so the strict gate stays honestly red and the advisory `-AllowMissing` baseline is the Phase 1 evidence.

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
