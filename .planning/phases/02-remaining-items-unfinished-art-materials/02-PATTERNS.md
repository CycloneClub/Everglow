# Phase 2: Remaining Items & Unfinished-Art Materials - Pattern Map

**Mapped:** 2026-09-14
**Files analyzed:** 24 (21 item classes + 1 projectile + 1 modified ModPlayer + 2 planning artifacts; 4 rows are decision-only and create no file)
**Analogs found:** 24 / 24 (every new file has a tracked in-repo analog)

## File Classification

| New/Modified File | Role | Data Flow | Closest Analog | Match Quality |
|-------------------|------|-----------|----------------|---------------|
| `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs` (肌腱巨弓, full) | item | request-response (ranged) | `Items/Weapons/MossyCyatheaBow.cs` | exact |
| `.../Items/Weapons/RestrictionDeviceRE01.cs` (限制机, full) | item | event-driven (summon) | `Items/Weapons/RedAlgaeMinionStaff.cs` | exact |
| `.../Items/Weapons/BoulderCatapult.cs` (巨石弹射装置, full) | item | request-response (ranged) | `Items/Weapons/GreenThornBallLauncher.cs` | exact |
| `.../Projectiles/Ranged/BoulderCatapult_Proj.cs` (new projectile) | projectile | event-driven (explode/shrapnel) | `Projectiles/Ranged/GreenThornLauncher_Proj.cs` | exact |
| `.../Items/Weapons/ReekingBait.cs` (腥臭的诱饵, full) | item | event-driven (summon-use) | `Items/BossSummon/MossyRuby.cs` | role-match |
| `.../Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeHeaddress.cs` (full) | item (armor head) | event-driven (equip) | `Items/Armors/Molluscs/MossyMolluscsHelmet.cs` + `ShellMolluscsBreastPlate.cs` | exact |
| `.../Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeMask.cs` (full) | item (armor head) | event-driven (equip) | `MossyMolluscsHelmet.cs` | exact |
| `.../Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeBreastPlate.cs` (full) | item (armor body) | event-driven (equip) | `ShellMolluscsBreastPlate.cs` | exact |
| `.../Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeGreaves.cs` (full) | item (armor legs) | event-driven (equip) | `ShellMolluscsBreastPlate.cs` | role-match |
| `.../Items/Misc/JadeSnakeEgg.cs` (灵蛇玉卵, full) | item | event-driven (use item) | `Items/BossSummon/MossyRuby.cs` | exact |
| `.../Items/Accessories/BambooStepTalisman.cs` (shell) | item (accessory) | request-response | `Items/Accessories/RadialCarapace.cs` | exact |
| `.../Items/Accessories/PeachBranchAmulet.cs` (shell) | item (accessory) | request-response | `Items/Accessories/RadialCarapace.cs` | exact |
| `.../Items/Weapons/BambooWeapon.cs` (shell) | item (melee) | request-response | `Items/Weapons/QuetzalsWish.cs` | exact |
| `.../Items/Weapons/DiscipleSword.cs` (shell) | item (melee) | request-response | `Items/Weapons/QuetzalsWish.cs` | exact |
| `.../Items/Weapons/FluorescentHydraStaff.cs` (shell) | item (summon) | event-driven | `Items/Weapons/MossyCyatheaBow.cs` (shape) / `QuetzalsWish.cs` (fallback) | role-match |
| `.../Items/Weapons/SkillBambooSlip.cs` (shell) | item (misc) | request-response | `Items/Misc/ForestBreath.cs` | role-match |
| `.../Items/Vanity/BambooHairpin.cs` (shell) | item (vanity) | event-driven (equip) | `Items/Misc/WitheredMask.cs` | exact |
| `.../Items/Vanity/DiscipleVanity.cs` (shell) | item (vanity) | event-driven (equip) | `Items/Misc/WitheredMask.cs` | exact |
| `.../Items/Misc/PeachBlossomKite.cs` (shell) | item (misc) | request-response | `Items/Misc/ForestBreath.cs` | exact |
| `.../Items/Misc/AlcoholicDrinks.cs` (shell) | item (misc) | request-response | `Items/Misc/ForestBreath.cs` | exact |
| `.../Items/Pets/PandaPet.cs` (shell) | item (pet) | event-driven | `Items/Misc/ForestBreath.cs` | role-match |
| `.../Items/Placeables/RegionalCraftingStation.cs` (shell, no tile) | item (plain) | request-response | `Items/Misc/ForestBreath.cs` | role-match |
| `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs` (modify, only if set needs state) | player-state | event-driven | itself (existing `RadialCarapace` flag pattern) | exact |
| `.planning/phases/02-.../scripts/check-phase2.ps1` | gate script | file-I/O | `.planning/phases/01-item-inventory-completed-art-items/scripts/check-carryover.ps1` | exact |
| `.planning/phases/02-.../02-CLASSIFICATION.json` | manifest | transform | `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` | role-match |

**Decision-only rows (no file created):** 3 × `阵法修复材料` stay `deferred`; `巨翼龙面具` reallocated to Phase 7 (edit `01-INVENTORY.json` fields only).

---

## Pattern Assignments

### `Items/Weapons/TendonGreatbow.cs` (肌腱巨弓, full, ranged bow) and `.../BoulderCatapult.cs`

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/MossyCyatheaBow.cs` (bow) and `Items/Weapons/GreenThornBallLauncher.cs` (launcher)

**Imports / namespace** (`MossyCyatheaBow.cs:1-6`):
```csharp
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
```

**SetDefaults + LocalizationCategory** (`GreenThornBallLauncher.cs:5-31`):
```csharp
public class GreenThornBallLauncher : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 60;
		Item.height = 36;
		Item.DamageType = DamageClass.Ranged;
		Item.damage = 25;
		Item.knockBack = 5.5f;
		Item.crit = 10;
		Item.useTime = Item.useAnimation = 35;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.UseSound = SoundID.Item108;
		Item.autoReuse = true;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 80);
		Item.useAmmo = AmmoID.Bullet;
		Item.shoot = ProjectileID.Bullet;
		Item.shootSpeed = 12f;
	}
```

**Item→projectile redirect** (`GreenThornBallLauncher.cs:33-37`):
```csharp
public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
{
	type = ModContent.ProjectileType<GreenThornLauncher_Proj>();
	position += velocity * 4;
}
```

**Design values to encode** (from `02-RESEARCH.md` §Design Data; do NOT copy blindly — use the mapped rarity):
- 肌腱巨弓: damage 28 (`固定`→ not fixed; design 28), crit 12%, `useTime` 28, value `Item.buyPrice(gold: 4)`, `ItemRarityID.Pink` (粉), boss-only +10% and charge behaviour → **effect blocker** (no projectile art), recipe ingredients `血云母`/`血肉聚合物` → **recipe blocker** (Pitfall 2).
- 巨石弹射装置: damage `44 R`/`Item.damage = 44`, `Item.value = Item.buyPrice(gold: 2)`, `ItemRarityID.Orange` (橙色), self-contained projectile (A4) → real `BoulderCatapult_Proj`; 150% direct / 3–6 shrapnel at 15%.

**Per D-13 fallback texture** — add the same override every art-missing class uses (see Shared Patterns).

---

### `Items/Weapons/RestrictionDeviceRE01.cs` (限制机, full, summon staff)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMinionStaff.cs`

**Imports** (`RedAlgaeMinionStaff.cs:1-7`) — note `Buffs`, `Items.Materials`, `Items.Placeables`, `Projectiles.Summon`, `Terraria.DataStructures`.

**Summon-staff shape + recipe** (`RedAlgaeMinionStaff.cs:13-64`):
```csharp
public override void SetStaticDefaults()
{
	Item.staff[Type] = true;
}

public override void SetDefaults()
{
	Item.DamageType = DamageClass.Summon;
	Item.damage = 42;
	Item.knockBack = 2;
	Item.mana = 27;
	Item.useStyle = ItemUseStyleID.Swing;
	Item.useTime = Item.useAnimation = 27;
	Item.noMelee = true;
	Item.rare = ItemRarityID.Orange;
	Item.value = 35000;
	Item.shoot = ModContent.ProjectileType<CrimsonMoonAlgaeSummonStaff_minion>();
	Item.shootSpeed = 12f;
}

public override void AddRecipes()
{
	CreateRecipe()
	.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
	.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
	.AddTile(TileID.WorkBenches)
	.Register();
}
```

**Phase 2 constraints for 限制机:** design recipe names `血云母 + 血肉聚合物 + 熔炉钢 + 隐生之眼` (all Phase-7, absent) → **do NOT write `AddRecipes`**; record a recipe blocker. Summon drone projectiles are not in repo → **effect blocker**; implement item identity + stats (`damage 18`, mana cost 15, `ItemRarityID.Pink`, `Item.value = Item.buyPrice(gold: 4)`).

---

### `Projectiles/Ranged/BoulderCatapult_Proj.cs` (new projectile)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/GreenThornLauncher_Proj.cs` (+ `GreenThornLauncher_SubProj.cs` for the 3–6 stone shrapnel)

**Core projectile pattern** (`GreenThornLauncher_Proj.cs:5-20, 38-80`):
```csharp
public class GreenThornLauncher_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
	}
```

**Collision → spawn sub-projectiles** (`GreenThornLauncher_Proj.cs:38-75, 133-141`):
```csharp
public override bool OnTileCollide(Vector2 oldVelocity)
{
	// ... dust/gore ...
	if (Projectile.velocity.X != oldVelocity.X) { Projectile.velocity.X = -oldVelocity.X; }
	if (Projectile.velocity.Y != oldVelocity.Y) { Projectile.velocity.Y = -oldVelocity.Y; }
	Projectile.velocity *= 0.69f;
	return false;
}

public override void OnKill(int timeLeft)
{
	// ... explosion dust ...
	if (Main.myPlayer == Projectile.owner)
	{
		var projNum = Main.rand.Next(2, 5);
		for (int i = 0; i < projNum; i++)
		{
			var projVelo = new Vector2(0, 10f).RotatedByRandom(MathHelper.TwoPi);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, projVelo, ModContent.ProjectileType<GreenThornLauncher_SubProj>(), (int)(Projectile.damage * 0.4f), knockback, Projectile.owner);
		}
	}
}
```

**Design mapping:** direct hit 150% damage; explode into 3–6 small stones at 15%. Art is missing → **optional**: fall back to `Commons.ModAsset.White_Mod` via a `Texture` override on the projectile too (the projectile has no approved art). Guard any `PreDraw`/texture work with `!Main.dedServ`.

---

### `Items/Weapons/ReekingBait.cs` (腥臭的诱饵) and `Items/Misc/JadeSnakeEgg.cs` (灵蛇玉卵, full, use item)

**Analog:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/BossSummon/MossyRuby.cs`

**Imports / namespace** (`MossyRuby.cs:1-7`):
```csharp
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Items.BossSummon;
```

**Use-item shape** (`MossyRuby.cs:9-52`):
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

	public override bool? UseItem(Player player) => base.UseItem(player);
}
```

**Phase 2 constraints:** the spawn target NPC (`苍翠灵蛇` / `巨翼龙`) is Phase 7 → **do NOT call `NPC.NewNPCDirect` / `ModContent.NPCType<T>()`** (compile error); implement identity + `SummonItems` category + use-style and record an **effect/encounter blocker** (QuetzalsWish precedent). `灵蛇玉卵` value `Item.buyPrice(gold: 10)`, `ItemRarityID.Blue` (蓝色); use condition "在森雨幽谷顶部使用" → blocker. `腥臭的诱饵` design row is column-sparse; use `LocalizationUtils.Categories.SummonItems` and record recipe + encounter blockers.

---

### `Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgae*.cs` (红月水藻 4-piece set)

**Analogs:** `MossyMolluscsHelmet.cs` (stats/set), `ShellMolluscsBreastPlate.cs` (equip registration)

**Armor stats/set pattern** (`MossyMolluscsHelmet.cs:7-43`):
```csharp
using Everglow.Commons.Mechanics;
using Everglow.Yggdrasil.Common;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.Molluscs;

[AutoloadEquip(EquipType.Head)]
public class MossyMolluscsHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.value = Item.buyPrice(silver: 60);
		Item.rare = ItemRarityID.Green;
		Item.defense = 2;
	}

	override public void UpdateEquip(Player player)
	{
		player.GetDamage<RangedDamageClass>() += 0.04f;
		player.GetCritChance<RangedDamageClass>() += 6;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
		=> body.type == ModContent.ItemType<ShellMolluscsBreastPlate>() && legs.type == ModContent.ItemType<MolluscsLeggings>();

	public override void UpdateArmorSet(Player player)
	{
		player.armorPenetration += 3;
		player.GetModPlayer<EverglowPlayer>().ammoCost *= 1 - SaveAmmoChance;
	}
}
```

**Equip-slot registration when art is missing** (`ShellMolluscsBreastPlate.cs:18-27`) — this is THE pattern to reuse instead of `[AutoloadEquip]` (D-13 / Pitfall 1):
```csharp
public override void Load()
{
	if (Main.dedServ)
	{
		return;
	}

	// Add a special equip texture by providing a custom name reference instead of an item reference
	EquipLoader.AddEquipTexture(Mod, $"{Texture}_{AltTextureName}_{EquipType.Body}", EquipType.Body, this, AltTextureKey);
}
```

**Phase 2 constraint (Pitfall 1):** `[AutoloadEquip]` + `Texture => Commons.ModAsset.White_Mod` throws `MissingResourceException`. The planner must pick ONE strategy and state it: (a) explicit `EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Head, this, nameof(CrimsonMoonAlgaeHeaddress))`, or (b) omit the equip slot and record a blocker. Recommendation = (a) so ITEM-02 is usable.

**Design values:** headdress `defense 8`, value `54000`, `ItemRarityID.Orange` (浅橙, A2), magic +18% / mana cost -12%; mask `defense 11`, `54000`, summon +18% / +3 max minions; breastplate `defense 16`, `72000`, heal 15% on damage ≥10; greaves `defense 4`, `48000`, speed +12% (<-24% underwater) / immune red-algae slow. Set has **no design recipe** → mirror family recipe (A3): `CrimsonMoonSap` + `JadeLakeRedAlgae_Item` @ Work Benches (see Shared Recipes). New set-bonus flags go on `KelpCurtainPlayer` (below).

---

### Shell classes (12) — identity-only

**Primary analog:** `Items/Misc/ForestBreath.cs` (misc/shell), `Items/Misc/WitheredMask.cs` (vanity), `Items/Accessories/RadialCarapace.cs` (accessory), `Items/Weapons/QuetzalsWish.cs` (melee).

**Minimal identity shell** (`ForestBreath.cs:7-22`):
```csharp
public class ForestBreath : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Miscs;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
	}
}
```

**Accessory shell** (`RadialCarapace.cs:3-20`) — add `Item.accessory = true;`:
```csharp
public class RadialCarapace : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Green;
	}
}
```

**Vanity shell** (`WitheredMask.cs:8-23`) — `Item.vanity = true;`, category `Vanity`, **do not** autoload the equip slot (art absent).

**Melee shell** (`QuetzalsWish.cs:9-32`) — full `SetDefaults` + `LocalizationUtils.Categories.MeleeWeapons` + `Texture` override.

**Per shell mapping** (folder / category / blocker):
| Class | Folder | `LocalizationCategory` | Blocker |
|-------|--------|------------------------|---------|
| `BambooStepTalisman` | `Accessories/` | `Accessories` | art pending |
| `PeachBranchAmulet` | `Accessories/` | `Accessories` | art pending |
| `BambooWeapon` | `Weapons/` | `MeleeWeapons` | art pending |
| `FluorescentHydraStaff` | `Weapons/` | `SummonWeapons` | art pending |
| `DiscipleSword` | `Weapons/` | `MeleeWeapons` | missing disciple system |
| `SkillBambooSlip` | `Weapons/` | `Miscs` | missing skill system |
| `BambooHairpin` | `Vanity/` | `Vanity` | art pending |
| `DiscipleVanity` | `Vanity/` | `Vanity` | missing disciple system |
| `PeachBlossomKite` | `Misc/` | `Miscs` | art pending |
| `AlcoholicDrinks` | `Misc/` | `Miscs` | art pending |
| `PandaPet` | `Pets/` | `Pets` | art pending |
| `RegionalCraftingStation` | `Placeables/` | `Placeables` | missing regional-crafting system (plain `ModItem`, no `ModTile`) |

All shells use `Item.width = Item.height = 20;` + a conservative `Item.value` + `ItemRarityID.Blue` unless the researched row gives a value. No `AddRecipes`, no `Shoot`, no buff wiring.

---

### `KelpCurtainPlayer.cs` (modify — only if the set needs per-player state)

**Analog:** itself; the Phase 1 `RadialCarapace` flag precedent.

**Flag pattern** (`KelpCurtainPlayer.cs:20-51, 62-75`):
```csharp
/// <summary>
/// <see cref="Items.Accessories.RadialCarapace"/>
/// </summary>
public bool RadialCarapace { get; set; }

public override void ResetEffects()
{
	MolluscsLeggings = false;
	MolluscsSetBuff = false;
	RadialCarapace = false;
	CorrodedPearl = false;
}

public override void UpdateEquips()
{
	if (Player.wet)
	{
		float multiplier = 1f
			+ (MolluscsSetBuff ? 0.3f : 0f)
			+ (MolluscsLeggings ? 0.35f : 0f)
			+ (RadialCarapace ? 0.35f : 0f)
			+ (CorrodedPearl ? 0.2f : 0f);
		Player.runAcceleration *= multiplier;
		Player.maxRunSpeed *= multiplier;
	}
}
```
If the 红月水藻 set needs a damage-triggered heal or red-algae immunity flag, add it here (reset in `ResetEffects`, consume in `UpdateEquips`). Do NOT persist/network it unless authoritative state changes; if so, follow `ArmOfGiantTreeCharge`/`ArmOfGiantTreeChargePacket` (`KelpCurtainPlayer.cs:77-97`).

---

### `.planning/phases/02-.../scripts/check-phase2.ps1` (gate script)

**Analog:** `.planning/phases/01-item-inventory-completed-art-items/scripts/check-carryover.ps1`

**Header + repo-relative resolution** (`check-carryover.ps1:27-44`):
```powershell
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
$phaseDir = Split-Path -Parent $scriptDir
$repoRoot = (Resolve-Path (Join-Path $phaseDir '..\..\..')).Path
$jsonPath = Join-Path $phaseDir '01-INVENTORY.json'
$itemsRel = 'Sources/Modules/Yggdrasil/KelpCurtain/Items'

$inv = [IO.File]::ReadAllText($jsonPath) | ConvertFrom-Json
$entries = @($inv.entries)
```

**Tracked-class coverage + no-placeholder guard** (`check-carryover.ps1:57-98`):
```powershell
$trackedNames = @{}
$tracked = & git -C $repoRoot ls-files -- "$itemsRel/*.cs"
foreach ($p in @($tracked)) {
	if ([string]::IsNullOrWhiteSpace($p)) { continue }
	$trackedNames[[IO.Path]::GetFileName($p)] = $true
}
# ... per-entry short-name check against $trackedNames ...
$statusLines = & git -C $repoRoot -c core.quotepath=false status --porcelain -- $itemsRel
foreach ($line in @($statusLines)) {
	$path = $line.Substring(3)
	if (($path -like '*.png') -and ($status -match '[AM?RC]')) {
		$failures.Add("placeholder guard : added/modified png '$path' (status '$status')")
	}
}
```

**Phase 2 additions** (per `02-RESEARCH.md` §Verification Gate Design): select `phase == 2`; require `code_complete == true` / `artwork_complete == false` / `status == "unchecked"` / blocker matching `texture|artwork|system|effect|recipe`; preserve the 3 deferred rows; assert `巨翼龙面具` `phase == 7`; cross-check `02-CLASSIFICATION.json` = exactly 9 full / 12 shell.

### `.planning/phases/02-.../02-CLASSIFICATION.json` (manifest)

**Analog:** `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` entry shape (fields `id`, `phase`, `code_complete`, `artwork_complete`, `status`, `blockers`, `feishu.block_id`, `internal_name`, `deferred`, `deferred_reason`). Phase 2 manifest rows: `{id, mode: "full"|"shell", class_file, blocker_kind}` × 21.

---

## Shared Patterns

### Fallback texture (ALL 21 item classes + projectile)
**Source:** `Items/Misc/ForestBreath.cs:11-13` (Phase 1 D-13 precedent)
```csharp
// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
// reuse the existing shared fallback texture rather than create placeholder art.
public override string Texture => Commons.ModAsset.White_Mod;
```
Apply to every class. Do NOT create or modify any `.png` (AGENTS / D-13).

### `LocalizationCategory` (ALL content classes)
**Source:** `Sources/Everglow.Function/Utilities/LocalizationUtils.cs:19-50` [verified]
Files in namespace `...KelpCurtain.Items.*` may use the short form `LocalizationUtils.Categories.X`; others use `Everglow.Commons.Utilities.LocalizationUtils.Categories.X` (both appear in-repo). Do NOT write HJSON (D-20).

### Equip registration without art (4 armor pieces + any equip-vanity shell)
**Source:** `Items/Armors/Molluscs/ShellMolluscsBreastPlate.cs:18-27`
```csharp
public override void Load()
{
	if (Main.dedServ) { return; }
	EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Head, this, nameof(CrimsonMoonAlgaeHeaddress));
}
```
Never combine `[AutoloadEquip]` with the fallback `Texture` while `_Head/_Body/_Legs.png` is absent (Pitfall 1). Guard client-only asset work with `!Main.dedServ`.

### Recipe (family-consistent — only when ingredients exist)
**Source:** `Items/Weapons/RedAlgaeMinionStaff.cs:57-64`
```csharp
CreateRecipe()
.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
.AddTile(TileID.WorkBenches)
.Register();
```
Only the 红月水藻 set has resolvable ingredients. `肌腱巨弓`/`限制机`/`腥臭的诱饵` must NOT encode absent Phase-7 types (Pitfall 2) — record recipe blockers.

### Rarity / value mapping (ALL full items)
**Source:** `Items/Materials/CrimsonMoonSap.cs:10-13`, `RedAlgaeMinionStaff.cs:29-30`, `02-RESEARCH.md` Pitfall 6
- design `浅橙` → `ItemRarityID.Orange` (A2, confirm against family), `粉` → `Pink`, `蓝` → `Blue`, `橙色` → `Orange`.
- bare number (`54000`) → `Item.value = 54000`; `Ng` → `Item.buyPrice(gold: N)`.

### Blocker recording (ALL entries)
Each implemented entry must carry ≥1 precise blocker matching `texture|artwork|system|effect|recipe` (Phase 1 wording): e.g. `"approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending"`. Update `01-INVENTORY.json` (`internal_name`, `code_complete: true`, `artwork_complete: false`, `status: "unchecked"`, `blockers`) and leave the Feishu row unchecked (D-22). `巨翼龙面具` → `phase: 7` + `advances` append `ITEM-06` (D-16).

### No-placeholder guard (planning)
**Source:** `check-carryover.ps1:89-98`. Repeat in `check-phase2.ps1`; `git status --porcelain -- Sources/Modules/Yggdrasil/KelpCurtain/Items` must not show added/modified `.png`.

---

## No Analog Found

| File | Role | Data Flow | Reason |
|------|------|-----------|--------|
| (none) | — | — | Every file has a tracked KelpCurtain analog. The genuinely novel cases (explicit `EquipLoader` fallback for a *headless* armor set, armor-set heal/immunity state) are covered by composing the `ShellMolluscsBreastPlate` + `MossyMolluscsHelmet` + `KelpCurtainPlayer` patterns; `02-RESEARCH.md` §Common Pitfalls is the fallback reference. |

---

## Metadata

**Analog search scope:** `Sources/Modules/Yggdrasil/KelpCurtain/{Items,Projectiles,Buffs}`, `KelpCurtainPlayer.cs`, `Sources/Everglow.Function/{Utilities,ItemGlowManager}`, `.planning/phases/01-.../scripts`, `.planning/phases/01-.../01-INVENTORY.json`
**Files scanned:** 12 analog files read in full; ~130 tracked item/projectile files enumerated
**All analog paths verified git-tracked** (`git ls-files`) — no gitignored mirror paths.
**Pattern extraction date:** 2026-09-14
