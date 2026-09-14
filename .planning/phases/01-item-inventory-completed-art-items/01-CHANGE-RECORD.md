# Phase 1 Change Record — Item Inventory & Completed-Art Items

> **Purpose:** a review record of every source change made in Phase 1, for confirmation
> with the other developers and the design team (策划). Derived from
> `git diff 5c025ff7e..HEAD` (Phase 1 start → current).
>
> **Status:** Phase 1 is marked complete, but the changes below have **not yet been
> confirmed against the authoritative Feishu design rows by the design team**. Please
> review the "Confirmation checklist" at the end before treating any value as final.

## Scope

- **45 modified source files** (`472 insertions, 44 deletions`) + **6 new source files**.
- All paths are under `Sources/Modules/Yggdrasil/KelpCurtain/` unless noted.
- Change classes: (1) stat parity, (2) value/rarity fixes, (3) recipes, (4) localization
  classification, (5) comment fix, (6) ArmOfGiantTree networking fix.

## New files (5 item classes + 1 network packet)

| File | Item (zh) | Notes |
|------|-----------|-------|
| `Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs` | 巨树之臂 | Charged AoE smash; per-player/per-stack charge |
| `Items/Materials/ElftigernPowder.cs` | 厄佛提根的净化粉末 | Consumable; effect deferred to Phase 6 (GAME-03) |
| `Items/Misc/ForestBreath.cs` | 森林之息 | Quest item; artwork missing (blocker) |
| `Items/Misc/WitheredMask.cs` | 枯萎面具 | Vanity; artwork missing (blocker) |
| `Items/Weapons/QuetzalsWish.cs` | 魁札尔的愿望 | Melee giant-blade; artwork missing (blocker) |
| `Sources/Modules/Yggdrasil/Netcode/ArmOfGiantTreeChargePacket.cs` | — | Charge sync + server-authoritative release |

## Modified files (45)

### 1. Stat parity — implemented stats aligned to the design rows

| File | Change |
|------|--------|
| `Weapons/MossySpell.cs` | damage 13→29, mana 4→12, useTime/useAnimation 16→50, rare Green→Orange |
| `Weapons/ActivatedDogStaff.cs` | damage 8→16, knockBack 1.1→4, useTime 20→29, rare Green→Orange |
| `Weapons/UnderwaterTreasury/ArcI.cs` | damage 39→21, knockBack 6→1, crit 8→14 |
| `Weapons/DevilHeart/DevilHeartBayonet.cs` | damage 22→13 |
| `Weapons/DevilHeart/DevilHeartGyroscope.cs` | damage 17→24 |
| `Weapons/LegumeGyroscope.cs` | knockBack 0.2→0.8 |
| `Weapons/RedAlgaeMinionGyroscope.cs` | knockBack 0.2→1.5, useTime/useAnimation 16→42 |
| `Weapons/RedAlgaeMagicWhip.cs` | +knockBack 1.5, useAnimation/useTime 30→22 |
| `Weapons/RedAlgaeMagicSpellBook.cs` | useTime/useAnimation 4→6 |
| `Weapons/RazorbeakBow.cs` | useTime 12→11 |
| `Ammos/HuskburstBullet.cs` | damage 10→6, +knockBack 1.15, shootSpeed 16→18 |
| `Accessories/ThornTurtleShell.cs` | maxRunSpeed penalty 5%→10% |

### 2. Value / rarity fixes

| File | Change |
|------|--------|
| `Accessories/AntiCorrosiveSole.cs` | value `sellPrice(gold:1)` → `buyPrice(gold:1)` |
| `Accessories/CorrodedPearl.cs` | value `sellPrice(gold:1)` → `buyPrice(gold:1)` |
| `Accessories/ForestMino.cs` | value `sellPrice(gold:2)` → `buyPrice(gold:2)` |
| `Weapons/MeatLantern.cs` | value `sellPrice(gold:1)` → `buyPrice(gold:1)` |
| `Weapons/BladeOfGreenMoss.cs` | value `sellPrice(gold:1)` → `buyPrice(silver:80)` |
| `Weapons/GreenSungloStaff.cs` | value `sellPrice(0,2,0,0)` → `buyPrice(silver:80)` |
| `Weapons/GreenVineWhip.cs` | value `sellPrice(0,1,0,0)` → `10000`; rare Blue→Orange |
| `Weapons/LegumeGyroscope.cs` | value 11700→20000; rare Green→Blue |
| `PermanentBoosters/JadeFruit.cs` | value 15000→2500 |
| `Placeables/AlgaeExtractor_Item.cs` | +value 20000 |
| `Armors/Ruin/RuinMask.cs` | rare Gray→Green |
| `Armors/Ruin/RuinMagicRobe.cs` | rare Gray→Green |
| `Armors/Ruin/RuinLeggings.cs` | rare Gray→Green |
| `Materials/DevilHeartIronBar_Item.cs` | +rare Green |
| `Placeables/DevilHeartIronOre_Item.cs` | +rare Green |
| `Placeables/JadeizedBone_Item.cs` | +rare Green |

### 3. Recipes added — DevilHeart armor set

| File | New recipe |
|------|------------|
| `Armors/DevilHeart/DevilHeartHairpin.cs` | 18 × `DevilHeartIronBar_Item` @ Anvil |
| `Armors/DevilHeart/DevilHeartHelmet.cs` | 20 × `DevilHeartIronBar_Item` @ Anvil |
| `Armors/DevilHeart/DevilHeartLeggings.cs` | 24 × `DevilHeartIronBar_Item` @ Anvil |
| `Armors/DevilHeart/DevilHeartLightBreastPlate.cs` | 32 × `DevilHeartIronBar_Item` @ Anvil |

### 4. Localization classification (`LocalizationCategory`) added/fixed

| File | Category |
|------|----------|
| `Placeables/AgedGreenCourtBrick_Item.cs` | `Placeables` |
| `Placeables/GreenCourtBrick_Item.cs` | `Placeables` |
| `Placeables/WaterErodedGreenBrick_Item.cs` | `Placeables` |
| `Placeables/Walls/AgedGreenCourtWall_Item.cs` | `Placeables` |
| `Placeables/DecayingWoodCourt/CrackedForestThrone_Item.cs` | `Placeables` |
| `Placeables/ForestRainVineTile_Thick_Item.cs` | `Placeables` |
| `Placeables/ForestRainVineTile_Thin_Item.cs` | `Placeables` |
| `Tools/Developer/GenerateMazeRoom.cs` | `Tools` |
| `Tools/Developer/ResetIsleOfBloom.cs` | `Tools` |
| `Tools/Developer/ResetKelpCurtain.cs` | `Tools` |
| `Tools/Developer/UnderWaterDungeon.cs` | `Tools` |
| `Weapons/GreenSungloStaff.cs` | `MagicWeapons` |
| `Weapons/Special/VineRepairWand.cs` | `MagicWeapons` |
| `Armors/DevilHeart/DevilHeartHairpin.cs` | `base.LocalizationCategory` (unclassified) → `Armor` |

### 5. Comment correction

| File | Change |
|------|--------|
| `Armors/Molluscs/ShellMolluscsBreastPlate.cs` | values unchanged; comments 10s/35s corrected to 25s/40s |

### 6. ArmOfGiantTree networking fix (gap closure 01-07)

| File | Change |
|------|--------|
| `KelpCurtainPlayer.cs` | new `ArmOfGiantTreeCharge`, `ArmOfGiantTreeChargedSlot` (-1), `CopyClientState` / `SendClientChanges` + usings — charge moved off the shared `ModItem` to per-player/per-stack synced state |

## Deferred / still open (not part of this change set)

- **Live multiplayer verification** of `ArmOfGiantTree` charge isolation + server-authoritative
  shockwave — recorded in `01-UAT.md` as a deferred follow-up.
- **Localization** for the new/carry-over entries — deferred by user directive; 18 keys missing.
- **3 missing artwork blockers**: `ForestBreath`, `WitheredMask`, `QuetzalsWish` (no repo texture).
- **13 boss/encounter reward entries** routed to Phase 7 (ITEM-05 / ITEM-06).
- **25 remaining `phase:2` entries** (artwork-incomplete) belong to Phase 2.

## Confirmation checklist (for developers & design team)

Please confirm or correct each item before these values are treated as final:

1. **Stats** — do the aligned damage / knockback / crit / useTime / mana values match the
   Feishu design rows for: MossySpell, ActivatedDogStaff, ArcI, DevilHeartBayonet,
   DevilHeartGyroscope, LegumeGyroscope, RedAlgaeMinionGyroscope, RedAlgaeMagicWhip,
   RedAlgaeMagicSpellBook, RazorbeakBow, HuskburstBullet, ThornTurtleShell?
2. **Value / rarity** — confirm the corrected `buyPrice` values and rarity tiers, in
   particular: Ruin set Gray→Green; JadeFruit 15000→2500; GreenVineWhip →10000 (Orange);
   LegumeGyroscope 11700→20000 (Blue); AlgaeExtractor 20000.
3. **DevilHeart recipes** — confirm the ingredient counts (18/20/24/32) and the Anvil station.
4. **Localization categories** — confirm `Placeables` / `Tools` / `MagicWeapons` / `Armor`
   are the intended buckets (affects the in-game exporter output).
5. **`QuetzalsWish`** — the design row is Melee (`mel37`) while the parser set the inventory
   category to `weapons.ranged`; implemented as Melee, discrepancy recorded. Confirm.
6. **`ArmOfGiantTree`** — confirm the intended charge behaviour (2.5 s cap, 200 % charged hit +
   100 % shockwave) and that client-authoritative charge + server-applied shockwave is acceptable.

## Reversion (2026-09-14) — Phase 1 design-value edits reverted

**Developer decision: follow-the-code.** The governing rule is: when implemented code conflicts with the design, follow the code unless the Feishu row is marked yellow with a corresponding explanation. During live testing the Feishu design numbers were found unreasonable, so the original code values were correct.

The Phase 1 numeric parity edits below were therefore reverted to the pre-Phase-1 code baseline at commit **`5c025ff7e`** (26 files restored byte-identical to that commit). The kept non-numeric Phase 1 changes — the four DevilHeart armor recipes, every added `LocalizationCategory` override, and the `ShellMolluscsBreastPlate` comment correction — remain in the tree.

Sections 1 (stat parity) and 2 (value/rarity) of "Modified files (45)" above are **superseded by this reversion** and describe the now-reverted Phase 1 numeric edits; they are retained as historical execution evidence only.

### Reverted files (27)

Stat-parity reverts (12): `Weapons/MossySpell.cs`, `Weapons/ActivatedDogStaff.cs`, `Weapons/UnderwaterTreasury/ArcI.cs`, `Weapons/DevilHeart/DevilHeartBayonet.cs`, `Weapons/DevilHeart/DevilHeartGyroscope.cs`, `Weapons/LegumeGyroscope.cs`, `Weapons/RedAlgaeMinionGyroscope.cs`, `Weapons/RedAlgaeMagicWhip.cs`, `Weapons/RedAlgaeMagicSpellBook.cs`, `Weapons/RazorbeakBow.cs`, `Ammos/HuskburstBullet.cs`, `Accessories/ThornTurtleShell.cs`.

Value/rarity reverts (16, `Weapons/LegumeGyroscope.cs` overlapping): `Accessories/AntiCorrosiveSole.cs`, `Accessories/CorrodedPearl.cs`, `Accessories/ForestMino.cs`, `Weapons/MeatLantern.cs`, `Weapons/BladeOfGreenMoss.cs`, `Weapons/GreenSungloStaff.cs` (special case), `Weapons/GreenVineWhip.cs`, `Weapons/LegumeGyroscope.cs`, `PermanentBoosters/JadeFruit.cs`, `Placeables/AlgaeExtractor_Item.cs`, `Armors/Ruin/RuinMask.cs`, `Armors/Ruin/RuinMagicRobe.cs`, `Armors/Ruin/RuinLeggings.cs`, `Materials/DevilHeartIronBar_Item.cs`, `Placeables/DevilHeartIronOre_Item.cs`, `Placeables/JadeizedBone_Item.cs`.

### Special case — `Weapons/GreenSungloStaff.cs`

`Item.value` was reverted to `Item.sellPrice(0, 2, 0, 0);`, but the file was **not** full-checked-out from the base: the `public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;` override (row 4 above) is intentionally **kept**.

The revert commit is `revert(260914-kl8): restore pre-Phase-1 code values for design-parity edits`; the rule is also recorded in `01-DEVIATIONS.md`, `.planning/PROJECT.md`, and `.planning/phases/02-remaining-items-unfinished-art-materials/02-CONTEXT.md` (D-23).
