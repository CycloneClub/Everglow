# Phase 1: Item Inventory & Completed-Art Items - Pattern Map

**Mapped:** 2026-09-11
**Files analyzed:** 9 file groups (3 new planning-artifact kinds + 6 source-content kinds)
**Analogs found:** 6 source-content kinds exact / 3 planning-artifact kinds have no direct code analog

> **Scope split.** This phase has two workstreams: (A) a **planning-artifact reconciliation** (`01-INVENTORY.md` + `01-INVENTORY.json` + committed `evidence/*.xml` + optional parser/validator tooling) and (B) a **completed-art item implementation tranche** under `Sources/Modules/Yggdrasil/KelpCurtain/Items/`. Workstream B has exact in-repo analogs; Workstream A is new to the repo and is mapped against doc/test conventions only.

## File Classification

| New/Modified File | Role | Data Flow | Closest Analog | Match Quality |
|-------------------|------|-----------|----------------|---------------|
| `.planning/phases/01-.../evidence/{biology,item,terrain}.xml` | data (external snapshot) | batch | none committed in repo | none |
| `.planning/phases/01-.../01-INVENTORY.json` | config/data (source of truth) | batch / transform | none (no prior schema) | none |
| `.planning/phases/01-.../01-INVENTORY.md` | documentation | transform | `01-RESEARCH.md` / `01-CONTEXT.md` (doc structure only) | partial (doc-format) |
| `.planning/phases/01-.../scripts/*.ps1` (parse / validate) | utility | file-I/O → transform | `Tools/FurnitureGenerator.mjs` (tooling shape only) | weak |
| `Sources/Everglow.UnitTests/Modules/Yggdrasil/**/*Inventory*Test.cs` | test | file-I/O | `Sources/Everglow.UnitTests/Core/MathUtils/MathUtils.VectorsTest.cs` + `.../UI/UIQuestOperationTipTest.cs` | role-match |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/**/` (new completed-art weapons) | model (content class) | event-driven (tML lifecycle hooks) | `Weapons/RedAlgaeMagicStaff.cs`, `Weapons/RedAlgaeMinionStaff.cs`, `Weapons/RedAlgaeMagicWhip.cs`, `Weapons/MossyCyatheaBow.cs` | exact |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/<Set>/*.cs` (new sets) | model (content class) | event-driven | `Armors/DevilHeart/DevilHeartHelmet.cs` | exact |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/*.cs` (new accessories) | model (content class) | event-driven | `Accessories/RadialCarapace.cs`, `Accessories/TheGreenSolar.cs` | exact |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/*.cs` (new materials) | model (content class) | event-driven | `Materials/CrimsonMoonSap.cs`, `Materials/DevilHeartIronBar_Item.cs` | exact |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Ammos/*.cs` (new ammo) | model (content class) | event-driven | `Ammos/CyatheaArrow.cs` | exact |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/**/*_Item.cs` (new placeables/walls) | model (content class) | event-driven | `Placeables/OldMoss_Item.cs`, `Placeables/Walls/GreenCourtWall_Item.cs` | exact |
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/BossSummon/*.cs` (new summoners) | model (content class) | event-driven | `BossSummon/MossyRuby.cs` | exact |
| `Sources/Everglow/Localization/{en-US,zh-Hans}/Mods.Everglow.Items.*.hjson` | config (localization) | transform (static data) | `en-US/Mods.Everglow.Items.Weapons.Ranged.hjson`, `...Items.Armor.hjson`, `...Items.Accessories.hjson` | exact |

---

## Pattern Assignments

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/*.cs` (model, event-driven)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicStaff.cs`

**Imports / base-class pattern** (lines 1-11) — namespaces are file-scoped; module-global usings (`Terraria`, `Terraria.ModLoader`, `Terraria.ID`, `Microsoft.Xna.Framework`, `Everglow.Commons`) come from `Sources/Modules/Directory.Build.props`, so only cross-namespace `using` lines appear:

```csharp
using Everglow.SpellAndSkull.Items;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RedAlgaeMagicStaff : SpellTomeItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;
```

**Core SetDefaults + Shoot pattern** (lines 13-39):

```csharp
	public override void SetDefaults()
	{
		Item.damage = 54;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 12;
		Item.width = 46;
		Item.height = 46;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 2.7f;
		Item.value = 35000;
		Item.rare = ItemRarityID.Orange;
		Item.UseSound = SoundID.Item42;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<RedAlgaeMagicStaff_Proj>();
		Item.shootSpeed = 12f;
		Item.staff[Type] = true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, -MathHelper.PiOver2);
		Projectile.NewProjectileDirect(source, position, velocity * 0.8f, type, damage, knockback, player.whoAmI, MathHelper.PiOver2);
		return false;
	}
```

**Recipe pattern** (lines 41-48):

```csharp
	public override void AddRecipes()
	{
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 12)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
```

**Ranged weapon analog** — `Weapons/MossyCyatheaBow.cs:6-27` (`Item.DamageType = DamageClass.Ranged`, `Item.useAmmo = AmmoID.Arrow`, `Item.shootSpeed = 12f`; note its recipe is commented out at lines 72-79 — a code-complete/recipe-pending example). Use its `Shoot` override (lines 29-65) for custom projectile substitution.

**Summon weapon analog** — `Weapons/RedAlgaeMinionStaff.cs:11,18-64`: `DamageClass.Summon`, `Item.mana`, `item.staff[Type] = true`, buff application via `player.AddBuff(ModContent.BuffType<...>(), 360000000)`, slot guard in `CanUseItem` (lines 48-55), same `CreateRecipe()` chain.

**Whip analog** — `Weapons/RedAlgaeMagicWhip.cs:8-23`: inherit `WhipItem` from `Everglow.Commons.Templates.Weapons.Whips` and override `SetDef()` (not `SetDefaults`); category is still `SummonWeapons`.

---

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/<Set>/*.cs` (model, event-driven)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/DevilHeart/DevilHeartHelmet.cs`

**Equip attribute + category + sacrifice count** (lines 5-13) — note `using Terraria.GameContent.Creative;`:

```csharp
[AutoloadEquip(EquipType.Head)]
public class DevilHeartHelmet : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}
```

**Set-defaults / equip / set-bonus pattern** (lines 15-43):

```csharp
	public override void SetDefaults()
	{
		Item.height = 20;
		Item.width = 20;

		Item.value = Item.buyPrice(0, 0, 60, 0);
		Item.rare = ItemRarityID.Green;

		Item.defense = 1;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.04f; // Increases summon damage by 4%
		player.slotsMinions += 1; // Increases the number of minions the player can summon by 1
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<DevilHeartLightBreastPlate>() && legs.type == ModContent.ItemType<DevilHeartLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.08f;
		player.slotsMinions += 1;
		player.GetAttackSpeed<SummonDamageClass>() += 0.15f;
		player.setBonus = this.GetLocalizedValue(LocalizationUtils.LocalizationKeys.SetBonus);
	}
}
```

Set-bonus text lives in HJSON under the `SetBonus:` key — see Shared Patterns → Localization.

---

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/*.cs` (model, event-driven)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/RadialCarapace.cs`

**Accessory + ModPlayer-state pattern** (lines 3-27) — this is also the placeholder-texture anti-pattern to flag in the inventory (no `RadialCarapace.png`; `Texture => Commons.ModAsset.White_Mod`, Pitfall 4):

```csharp
public class RadialCarapace : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.defense = 4;
		Item.accessory = true;
		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Green;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statDefense += 4;
		player.moveSpeed -= 0.05f;
		player.GetModPlayer<KelpCurtainPlayer>().RadialCarapace = true;
	}
}
```

For accessories with client-side rendering/draw code, `Accessories/TheGreenSolar.cs` is the heavier analog (uses `ModAsset.SunstoneTest.Value`, `Commons.ModAsset.Point.Value`, `Commons.ModAsset.Trail_6.Value` inside a `!Main.dedServ`-guarded draw path).

---

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/*.cs` (model, event-driven)

**Simple material analog:** `Materials/CrimsonMoonSap.cs:3-14`

```csharp
public class CrimsonMoonSap : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 28;
		Item.value = 5000;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Orange;
	}
}
```

**Material that is also placeable + has a recipe:** `Materials/DevilHeartIronBar_Item.cs:6-25`

```csharp
public class DevilHeartIronBar_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<DevilHeartIronBar>());
		Item.width = 30;
		Item.height = 24;
		Item.value = 1800;
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<DevilHeartIronOre_Item>(), 3)
			.AddIngredient(ModContent.ItemType<JadeizedBone_Item>(), 1)
			.AddTile(TileID.Furnaces)
			.Register();
	}
}
```

---

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/Ammos/*.cs` (model, event-driven)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Ammos/CyatheaArrow.cs:5-27` — `Item.ammo = AmmoID.Arrow`, `Item.consumable = true`, `Item.maxStack = Item.CommonMaxStack`, custom `Item.shoot` projectile, category is the **weapon** category (`RangedWeapons`, not `Ammo`).

```csharp
public class CyatheaArrow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 32;
		Item.damage = 10;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2f;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.ammo = AmmoID.Arrow;
		Item.rare = ItemRarityID.Blue;
		Item.value = 20;
		Item.shoot = ModContent.ProjectileType<CyatheaArrow_proj>();
		Item.shootSpeed = 7f;
	}
}
```

---

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/**/*_Item.cs` (model, event-driven)

**Tile placeable analog:** `Placeables/OldMoss_Item.cs:3-12`

```csharp
public class OldMoss_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OldMoss>());
		Item.width = 16;
		Item.height = 16;
	}
}
```

**Wall placeable + recipe analog:** `Placeables/Walls/GreenCourtWall_Item.cs:5-22` (`Item.DefaultToPlaceableWall(ModContent.WallType<...>())`, `CreateRecipe(4)`).

**Furniture template analog:** `Placeables/DecayingWoodCourt/WitherWoodBookcase.cs:5-11` inherits `BookcaseItem` (template at `Sources/Everglow.Function/Templates/Furniture/BookcaseItem.cs:8-24`, which already sets `LocalizationCategory => Categories.Placeables` and `SetStaticDefaults` sacrifice count). Prefer the template base over a bespoke `ModItem` for furniture per `Everglow.Function/Templates`.

---

### `Sources/Modules/Yggdrasil/KelpCurtain/Items/BossSummon/*.cs` (model, event-driven)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/BossSummon/MossyRuby.cs:5-52`

```csharp
public class MossyRuby : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonItems;

	public override void SetDefaults()
	{
		Item.consumable = true;
		Item.width = 22;
		Item.height = 34;
		Item.value = 13000;
		Item.useTime = 12;
		Item.useAnimation = 12;
		Item.noMelee = true;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTurn = true;
		Item.UseSound = SoundID.Roar;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool ConsumeItem(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		int type = ModContent.NPCType<VampireMat>();
		if (NPC.CountNPCS(type) <= 0)
		{
			var npc = NPC.NewNPCDirect(Item.GetNPCSource_FromThis(), player.Center + new Vector2(player.direction * 1800, 0), type);
			npc.velocity.X = -player.direction * 12;
			Item.stack--;
			if (Item.stack <= 0)
			{
				Item.active = false;
			}
			return true;
		}
		return false;
	}
}
```

---

### `Sources/Everglow/Localization/{en-US,zh-Hans}/Mods.Everglow.Items.*.hjson` (config, transform)

**Analog:** `Sources/Everglow/Localization/en-US/Mods.Everglow.Items.Weapons.Ranged.hjson`

**Flat entry pattern (DisplayName + Tooltip, lines 260-268):**

```hjson
GreenThornBallLauncher: {
	DisplayName: Green Thorn Ball Launcher
	Tooltip: ""
}

MossyCyatheaBow: {
	DisplayName: Mossy Cyathea Bow
	Tooltip: ""
}
```

**Rich Tooltip + SetBonus block (Armor, lines 15-28)** — note triple-single-quote multi-line value and bracket `[Buff]` syntax:

```hjson
DevilHeartHelmet: {
	DisplayName: Devil Heart Helmet
	Tooltip: 4% increased summon damage and +1 max minions
	SetBonus:
		'''
		8% increased summon damage
		15% increased whip attack speed
		+1 max minions
		Press the set bonus hotkey to gain [Devil Heart Overload] buff for 10 seconds
		During this time, you cannot naturally regenerate mana and your defense is reduced by 15%
		But you gain +6% magic damage and critical strike chance, +1 max minions
		Has a 35 second cooldown
		'''
}
```

**Placeables key-name caveat** (`en-US/Mods.Everglow.Items.Placeables.hjson:1331-1349`): some placeable items are keyed by the **tile** name (`OldMoss:`) while the item class is `OldMoss_Item`; others are keyed with the `_Item` suffix (`GreenCourtBrick_Item:`). Determine key name per item — do not bulk-rename (Open Question 3 in RESEARCH).

**Rule:** keys are **additive only**. Never delete/rename. Prefer the in-game exporter (`OutputLocalizationHjsonItem`, middle-mouse in `Shoot()`) over hand-writing classification keys. Maintain **both** `en-US` and `zh-Hans`.

---

## Shared Patterns

### LocalizationCategory override (applies to every new/modified `ModItem`)
**Source:** `Sources/Everglow.Function/Utilities/LocalizationUtils.cs:5-65`
**Apply to:** All KelpCurtain item classes — every content class must override `LocalizationCategory` with a `LocalizationUtils.Categories.*` constant (`Armor`, `RangedWeapons`, `MagicWeapons`, `SummonWeapons`, `Accessories`, `Materials`, `Ammo`, `Placeables`, `Tools`, `Critters`, `SummonItems`, `Pets`, `PermanentBoosters`, …). Unclassified items default to `"Items"` and are logged by the exporter as `Item 未分类：…`.

```csharp
public const string RangedWeapons = "Items.Weapons.Ranged";
public const string MagicWeapons = "Items.Weapons.Magic";
public const string SummonWeapons = "Items.Weapons.Summon";
public const string Armor = "Items.Armor";
public const string Accessories = "Items.Accessories";
public const string Materials = "Items.Materials";
public const string Placeables = "Items.Placeables";
```

Two equivalent spellings exist in-repo; either is acceptable, but match the nearest local file:
- Fully qualified: `public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;`
- Short: `public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;` (the `Everglow.Commons` global using makes this resolve).

### Asset path resolution (applies to every new item)
**Source:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/RadialCarapace.cs:7` and `.../TheGreenSolar.cs:142-175`
**Apply to:** All new items. Keep the `.png` beside the `.cs`; access it through source-generated `ModAsset` members (module) or `Commons.ModAsset` (Function shared). Never hand-write `"Everglow/Yggdrasil/..."` strings.

```csharp
public override string Texture => Commons.ModAsset.White_Mod;   // shared white texture
// or, for a local texture:
var stoneTex = ModAsset.SunstoneTest.Value;
```

**Anti-pattern to flag in inventory:** an item resolving to `Commons.ModAsset.White_Mod` / `Point_Mod` with no local `.png` is artwork-incomplete even if the class exists (RESEARCH Pitfall 4 / D-05). Record as `artwork_complete=false` + blocker; do **not** create placeholder art.

### Formatting / BOM compliance (applies to all text output)
**Source:** `AGENTS.md:84` (`.editorconfig`), `AGENTS.md:107-117` (BOM check)
**Apply to:** Every `.cs` and `.hjson` edit plus the new `.md`/`.json` planning artifacts.
- Tab indentation (width 4), LF line endings, UTF-8 **without BOM**, Allman braces, file-scoped namespaces, trailing newline.
- After edits, run the byte-level BOM check from `AGENTS.md` against `origin/master`. (PowerShell `Set-Content`/`Out-File` add BOM — use the Write/Edit tools; if a script must write, use `[IO.File]::WriteAllText` with `UTF8Encoding($false)`.)

### Build / verification (applies to all source edits)
**Source:** `AGENTS.md:44-55`
**Apply to:** Every code change — `dotnet build` is mandatory; `dotnet test --verbosity normal /p:WarningLevel=0` once `tML*` env vars permit. Item code behavior is tML-runtime-verified, not unit-testable.

### Localization exporter (applies to all new content keys)
**Source:** `Sources/Everglow.Function/Localization/OutputLocalizationHjsonItem.cs:45-75` (`GetItems()` at 77-119)
**Apply to:** Generating missing keys. The exporter iterates `ItemID.Search.Names` starting with `Everglow`, reads each item's `LocalizationCategory`, warns on `== "Items"`, and writes additive category HJSON for zh-Hans + en-US. Never hand-classify.

---

## Modifications: existing deviations to fix (D-10/D-12)

Per RESEARCH Pitfall 5, these existing `ModItem` classes do **not** override `LocalizationCategory` and should be reconciled during the Phase 1 pass on completed-art entries (add the override only — no wholesale rewrite):

| File | Role | Fix |
|------|------|-----|
| `.../Items/Weapons/GreenSungloStaff.cs` | model | add `LocalizationCategory => Categories.MagicWeapons` (currently misclassified as generic `Items`) |
| `.../Items/Weapons/Special/VineRepairWand.cs` | model | add `LocalizationCategory => Categories.MagicWeapons` |
| `.../Items/Placeables/GreenCourtBrick_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Placeables/AgedGreenCourtBrick_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Placeables/WaterErodedGreenBrick_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Placeables/ForestRainVineTile_Thick_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Placeables/ForestRainVineTile_Thin_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Placeables/Walls/AgedGreenCourtWall_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Placeables/DecayingWoodCourt/CrackedForestThrone_Item.cs` | model | add `Categories.Placeables` |
| `.../Items/Tools/Developer/{GenerateMazeRoom,ResetIsleOfBloom,ResetKelpCurtain,UnderWaterDungeon}.cs` | model | add `Categories.Tools` (developer tools) |

> Caveat (RESEARCH A3): some "missing key" classes are placeables whose display key is intended under the tile name (`OldMoss` exists, `OldMoss_Item` does not). Resolve per item; do not bulk-rename keys.

---

## No Analog Found

Files with no close match in the codebase (planner should use RESEARCH §"Architecture Patterns" and the schema in `01-RESEARCH.md` Pattern 4 instead):

| File | Role | Data Flow | Reason |
|------|------|-----------|--------|
| `.planning/phases/01-.../evidence/{biology,item,terrain}.xml` | data | batch | No prior committed evidence snapshots; produced by `lark-cli docs +fetch --doc-format xml --detail full` (read-only, one-time). |
| `.planning/phases/01-.../01-INVENTORY.json` | config/data | batch / transform | No inventory schema exists; shape is agent-discretion (RESEARCH Pattern 4) with required fields `artwork_complete`, `code_complete`, `status`, `feishu.*block_id`. |
| `.planning/phases/01-.../01-INVENTORY.md` | documentation | transform | No prior matrix artifact; format mirrors `01-CONTEXT.md`/`01-RESEARCH.md` doc conventions. |
| `.planning/phases/01-.../scripts/*.ps1` | utility | file-I/O → transform | Repo contains no PowerShell scripts; nearest tooling shape is `Tools/FurnitureGenerator.mjs` (Node). Parser must be defensive: expand `rowspan`/`colspan`, anchor columns to `<thead>`, treat only green `rgb(217,245,214)` / yellow `rgb(255,255,204)` as status. |
| `.planning/phases/01-.../scripts/validate-inventory.*` (or MSTest) | test/utility | file-I/O | Validator is Wave-0 new; use offline JSON parse + field assertions. |

**Optional MSTest validator analog:** `Sources/Everglow.UnitTests/Core/MathUtils/MathUtils.VectorsTest.cs` for the pure-logic `[TestClass]`/`[TestMethod]` shape (global `using Microsoft.VisualStudio.TestTools.UnitTesting;` comes from `Sources/Everglow.UnitTests/Usings.cs`), and `Sources/Everglow.UnitTests/Function/QuestSystem/UI/UIQuestOperationTipTest.cs:36-46` for the `Path.Combine(AppContext.BaseDirectory, …)` / `File.ReadAllBytes` file-loading pattern. Any test touching `Terraria.Main` must set `Program.SavePath = string.Empty;` in `[TestInitialize]` and be `[DoNotParallelize]`.

---

## Metadata

**Analog search scope:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/`, `Sources/Everglow.Function/{Templates,Localization,Utilities}/`, `Sources/Everglow/Localization/{en-US,zh-Hans}/`, `Sources/Everglow.UnitTests/`, `Tools/`, `.planning/`
**Files scanned:** ~40 (13 category folders enumerated; representative analogs read in full)
**Tracked-source gate:** all cited analog paths verified via `git ls-files` (none are gitignored mirrors)
**Pattern extraction date:** 2026-09-11
