# Phase 2: Remaining Items & Unfinished-Art Materials - Research

**Researched:** 2026-09-14
**Domain:** tModLoader (Terraria) content implementation — `ModItem` classes for the Yggdrasil / Kelp Curtain module, art-missing fallback, structural verification
**Confidence:** HIGH (repo patterns, inventory data, tML equip-loader behaviour), MEDIUM (design→class mappings and rarity interpretation)

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

- **D-13:** For entries with no approved texture, implement the code first and use the shared fallback texture `Commons.ModAsset.White_Mod` (the Phase 1 precedent — `ForestBreath`/`WitheredMask`/`QuetzalsWish`), record a precise "approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending" blocker, and leave the Feishu row unchecked (no colour). Never create placeholder art and never modify a binary/art asset. — **Reversibility:** costly — the content class + internal name become compatibility-sensitive once committed, and replacing the fallback with real art later touches each class and its asset references.
- **D-14:** When approved art arrives later, the migration is a texture addition beside the `.cs` (source-generated `ModAsset`) plus removing the artwork blocker — not a class rework. The item identity must not depend on the fallback.
- **D-15:** The three `阵法修复材料` placeholder rows (`item-weapons.misc-a`, `-b`, `-c-名字要普通`; design names A/B/C, no checkbox) stay `deferred` and are NOT implemented in Phase 2.
- **D-16:** `巨翼龙面具` is reallocated to **Phase 7 (ITEM-06)** — it is a Giant Winged Dragon reward; apply `phase=7` and append `ITEM-06` during planning/execution. The Klein Snake series (`碧绿玉髓扇`, `龙骨猎枪`, `碧玉弯刀`, `魂蛇手杖`) is already Phase 7 (ITEM-05) and unchanged.
- **D-17:** Implementation order is **dependency order** — drops/materials first (e.g. `CrimsonMoonSap`, `JadeLakeRedAlgae_Item` for the 红月水藻 set), then the finished equipment that consumes them, so the intended progression is never bypassed (ITEM-01…04).
- **D-18:** Entries **with** a defined design row / checkbox get a **full implementation** (documented recipe/value/effect, `LocalizationCategory`, gates). Entries with **no detailed description and no checkbox** get an **identity-only shell class** (minimal `ModItem`, no gameplay behaviour, fallback texture + `LocalizationCategory`), so the type exists without inventing undefined behaviour.
  - Full implementation (9): 肌腱巨弓, 限制机, 腥臭的诱饵, 巨石弹射装置, 红月水藻头饰, 红月水藻面具, 红月水藻板甲, 红月水藻护胫, 灵蛇玉卵.
  - Shell class (12): 竹节步符, 竹制武器, 竹簪子, 桃枝护符, 桃花纸鸢（风筝）, 熊猫宠物, 若干酒类, 弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台, 荧光水螅召唤杖.
- **D-19:** Items that imply an unimplemented system (弟子剑/弟子时装/技能竹简/区域放置物品制作台 → a disciple/skill/regional-crafting system) are shell classes only; record a precise blocker naming the missing system. Do NOT implement the system in this phase.
- **D-20:** Localization is **out of scope for every phase** — the goal is code design. Do not run the in-game exporter, do not hand-edit HJSON, and treat ITEM-07 as deferred from the current milestone scope. Localization gaps are recorded in the deferred ledger only. — **Reversibility:** costly — excluding a roadmap requirement changes milestone acceptance and the Phase 8 synchronization scope.
- **D-21:** Phase 2 requires **in-client runtime verification** of representative items (obtainable/craftable/usable with the documented effect) in addition to offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0` (exit 0, no `error CS`). Runtime verification needs a running tModLoader client.
- **D-22:** Item marking follows Phase 1 D-11: code-complete + art-incomplete → `code_complete=true` / `artwork_complete=false`, Feishu row unchecked (no colour), art tracked as a blocker. A row only receives a status colour when both checkboxes are complete (D-05/D-07).

### the agent's Discretion

- Exact shell-class contents (minimal `SetDefaults` values, folder placement) and which adjacent existing item class to mirror.
- Blocker wording for missing-art and missing-system cases.
- Whether a full-implementation item needs a new projectile/buff/VFX class now or records a precise effect blocker for a later phase (Phase 1 precedent: `QuetzalsWish`).

### Deferred Ideas (OUT OF SCOPE)

- **Localization (all phases)** — excluded by user directive; ITEM-07 deferred. Record in the deferred ledger only.
- **3 × `阵法修复材料` placeholders** — wait for a real design entry/name.
- **Runtime verification** — requires a live tModLoader client; to be scheduled when convenient.
- **System-dependent items** (弟子剑/弟子时装/技能竹简/区域放置物品制作台) — the disciple/skill/regional-crafting system belongs to a later phase.
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| ITEM-01 | Planned natural-themed weapons, ammunition, materials, utility items, and biology-design creature drops/materials implemented with documented recipes, values, effects. | 9 full implementations + 12 shells; design rows extracted from `evidence/*.xml`; nearest analogs identified (`MossyCyatheaBow`, `GreenThornBallLauncher`, `RedAlgaeMinionStaff`, `Craft…` etc.). |
| ITEM-02 | Devil Heart Iron, Witherbark, Molluscs, and other planned armor/accessory sets (incl. biology-design materials) with documented class/set effects and progression placement. | 红月水藻 4-piece armor set is the Phase 2 armor tranche; nearest analog is `Armors/Molluscs/*`; the missing-art equip-slot hazard is documented below. |
| ITEM-03 | Underwater treasury, underwater maze, regional chest, fishing, collection rewards implemented with documented loot sources/access conditions/effects. | `灵蛇玉卵`, `巨石弹射装置`, 竹/弟子/桃/若干酒类 shells map to ITEM-03 (`advances` in inventory); source (NPC trade / regional chest) not in the XML → effect/access blockers. |
| ITEM-04 | Quest-exclusive, NPC-trade, settlement-restoration, purification, mission rewards, and their biology drops implemented with documented unlock conditions. | `灵蛇玉卵` (`advances: ["ITEM-04","ITEM-07"]`) is the Phase 2 ITEM-04 carrier; use condition "在森雨幽谷顶部使用" is documented, the summoned encounter is Phase 7. |
</phase_requirements>

## Summary

Phase 2 implements **21 of the 25 `phase == 2` inventory rows**: 9 full implementations and 12 identity-only shell classes. The remaining 4 rows are handled by decision, not code — the 3 `阵法修复材料` placeholders stay `deferred` (D-15) and `巨翼龙面具` is reallocated to Phase 7 (D-16). All 25 rows are artwork-incomplete, so **every** class must render the shared fallback `Commons.ModAsset.White_Mod` and carry a blocker; the Feishu row stays unchecked because the artwork checkbox is false (D-22).

The decisive technical hazard this research surfaces is **armor with missing equip art**. The four `红月水藻` pieces (and any vanity shell) are `EquipType` items; tModLoader's `[AutoloadEquip]` path registers `X_Head.png/_Body.png/_Legs.png`, and `EquipLoader.AddEquipTexture` calls `ModContent.Request<Texture2D>(texture); //ensure texture exists` — a missing equip texture throws and **the whole mod fails to load**. Since no placeholder art may be created (AGENTS / D-13), the plan must register the equip texture explicitly against an existing fallback (or omit the equip slot and record a blocker). This is the single highest-risk decision in the phase and is resolved in §Common Pitfalls 1.

The other cross-cutting hazard is **recipe ingredients that do not exist yet**. Three of the nine full implementations (`肌腱巨弓`, `限制机`, `腥臭的诱饵`) have designer recipes naming `血云母` (Blood Mica), `血肉聚合物` (Flesh Polymer), `隐生之眼`, `熔炉钢`, `干枯心脏` — none of which exist in `Sources/` (confirmed by grep; they are Phase 7 Giant-Winged-Dragon items). A recipe that references `ModContent.ItemType<…>()` on an absent type is a **compile error**. These recipes must be recorded as blockers, not coded, until Phase 7 (mirrors the Phase 1 `QuetzalsWish` effect-blocker precedent).

**Primary recommendation:** implement in dependency order — inventory reclassification first, then shells and art-fallback item identities, then the `巨石弹射装置` projectile, then the `红月水藻` armor set (materials `CrimsonMoonSap`/`JadeLakeRedAlgae_Item` already exist from Phase 1), recording precise art/recipe/effect/system blockers on every entry; gate with an ASCII PowerShell coverage+no-placeholder script plus `dotnet build /p:Configuration=Release /p:WarningLevel=0`, then human client verification of representative items.

## Architectural Responsibility Map

| Capability | Primary Tier | Secondary Tier | Rationale |
|------------|-------------|----------------|-----------|
| Item definitions (full + shell) | Module content (`Sources/Modules/Yggdrasil/KelpCurtain/Items/**`) | — | Ordinary `ModItem` types; auto-discovered, no manual `AddContent` (AGENTS). |
| Item icon / art fallback | Module content | Shared asset (`Everglow.Commons.ModAsset.White_Mod` → `Everglow.Commons/Textures/White.png`) | `White_Mod` is a Function asset consumed by module classes (Phase 1 precedent). |
| Equip-slot registration (armor/vanity) | Module content `Load()` | tML `EquipLoader` | Must be explicit when the `_Head/_Body/_Legs` art is absent; `[AutoloadEquip]` would throw. |
| Recipes | Module content `AddRecipes()` | tML `Recipe` system | Ingredient types must resolve at compile time; absent Phase-7 items → blocker. |
| Per-player armor-set state | Module `ModPlayer` (`KelpCurtainPlayer`) | Module content | Phase 1 precedent (`MolluscsLeggings`, `RadialCarapace`, `ArmOfGiantTreeCharge`). |
| Classification / blocker / status records | Planning artifacts (`.planning/phases/…`) | Inventory JSON + PowerShell gates | Phase 1 D-01/D-11; JSON is source of truth. |
| Runtime verification | tModLoader client (human) | — | D-21; cannot be unit-tested. |

## Standard Stack

### Core

There are **no new external packages** in this phase (confirmed: no package install requests and `.planning/config.json` shows no new dependency workflow). Everything uses the already-referenced tModLoader 1.4 API and the module's existing templates.

| Library / API | Version | Purpose | Why Standard |
|---------------|---------|---------|--------------|
| `Terraria.ModLoader.ModItem` | tML 1.4 (net8.0) | All item classes | Repo-wide content pattern; ~200 existing KelpCurtain items. |
| `Everglow.Commons.ModAsset.White_Mod` | source-generated (`EnablePathGenerator`) | Fallback item texture | Phase 1 precedent (D-13). VERIFIED below. |
| `LocalizationUtils.Categories.*` | `Sources/Everglow.Function/Utilities/LocalizationUtils.cs` | Required `LocalizationCategory` | AGENTS: content classes must override `LocalizationCategory`. |
| `EquipLoader.AddEquipTexture` | tML 1.4.4 | Register equip slot when `_Head/_Body/_Legs.png` absent | Only way to equip armor with no art (see Pitfall 1). |
| `ModContent.HasAsset` / `RootContentSource.HasAsset` | tML 1.4 | Detect optional art presence | In-repo precedent `ItemGlowManager.cs`. |

**Version verification (repo discrete values, read this session):**
- `Commons.ModAsset.White_Mod` is used verbatim as an item texture in four KelpCurtain/Example classes — `ForestBreath.cs:13`: `public override string Texture => Commons.ModAsset.White_Mod;`; `WitheredMask.cs:14`, `QuetzalsWish.cs:15`, `RadialCarapace.cs:7` [VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs:13].
- The backing asset exists: `Sources/Everglow.Function/Textures/White.png`; the generated member suffix comes from `EnablePathGenerator` — `<EnablePathGenerator>true</EnablePathGenerator>` at `Sources/Directory.Build.props:12` [VERIFIED: Sources/Directory.Build.props:12].
- Module prefix/namespace come from `<ModuleName>$(MSBuildProjectName.SubString(9))</ModuleName>` and `<PathPrefix>$(ModuleName)</PathPrefix>` at `Sources/Modules/Directory.Build.props:4-5` [VERIFIED: Sources/Modules/Directory.Build.props:4-5].

**`LocalizationCategory` values that this phase needs** (verbatim from `Sources/Everglow.Function/Utilities/LocalizationUtils.cs`) [VERIFIED: Sources/Everglow.Function/Utilities/LocalizationUtils.cs:19-50]:

```
public const string MeleeWeapons = "Items.Weapons.Melee";      // :19
public const string RangedWeapons = "Items.Weapons.Ranged";    // :21
public const string SummonWeapons = "Items.Weapons.Summon";    // :22
public const string Armor = "Items.Armor";                     // :26
public const string Vanity = "Items.Vanity";                   // :27
public const string Accessories = "Items.Accessories";         // :30
public const string Pets = "Items.Pets";                       // :34
public const string Materials = "Items.Materials";             // :41
public const string SummonItems = "Items.SummonItems";         // :47
public const string Miscs = "Items.Miscs";                     // :50
```

### Alternatives Considered

| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| `Commons.ModAsset.White_Mod` fallback | `[AutoloadEquip]` with missing art | **Forbidden** — fails at load (Pitfall 1). |
| Scattered `Item.*` assignments | `Item.DefaultToPlaceableTile(...)` etc. | Only valid if the tile/type exists; shell placeables have no tile → plain `ModItem`. |
| A new projectile class for every full weapon | Record precise effect blocker | Discretion D-18; `QuetzalsWish` precedent supports blockers for absent art/effect dependencies. |

**Installation:** none (no new packages).

## Package Legitimacy Audit

**Not applicable.** Phase 2 installs no external packages; all code uses already-referenced tModLoader / SubworldLibrary / existing NuGet references. The Package Legitimacy Gate is therefore skipped, and there are no `[SUS]`/`[SLOP]` dispositions to record.

## Architecture Patterns

### System Architecture Diagram

```
Feishu design rows (25 × phase==2)
        │  (already snapshotted in Phase 1)
        ▼
01-INVENTORY.json  ── source of truth (phase, artwork_complete, code_complete, blockers, feishu.block_id)
        │
        ▼  classification (Phase 2 planning)
   ┌──────────────┬───────────────┬────────────────┬───────────────┐
   │ 9 full       │ 12 shell      │ 3 deferred     │ 1 reallocated │
   │ impl         │ classes       │ (阵法修复材料)  │ 巨翼龙面具→P7 │
   └──────┬───────┴──────┬────────┴────────────────┴───────────────┘
          │              │
   dependency order: materials/drops → consuming equipment
          ▼              ▼
   ModItem class(es) — Texture => Commons.ModAsset.White_Mod; LocalizationCategory override
     │ optional: Projectile class, Buff wiring, KelpCurtainPlayer flag
     ▼
   structural gate (PowerShell, ASCII)  ──▶  dotnet build /p:Configuration=Release /p:WarningLevel=0
     ▼
   human client runtime verification (representative items, D-21)
     ▼
   inventory status update (code_complete=true, artwork_complete=false, blocker; row unchecked)
```

### Recommended Project Structure

```
Sources/Modules/Yggdrasil/KelpCurtain/Items/
├── Weapons/
│   ├── TendonGreatbow.cs                      # 肌腱巨弓 (full, Ranged)
│   ├── RestrictionDeviceRE01.cs               # 限制机 (full, Summon)
│   ├── BoulderCatapult.cs                     # 巨石弹射装置 (full, Ranged)
│   ├── ReekingBait.cs                         # 腥臭的诱饵 (full, summon item)
│   ├── DiscipleSword.cs                       # 弟子剑 (shell)
│   ├── BambooWeapon.cs                        # 竹制武器 (shell)
│   ├── SkillBambooSlip.cs                     # 技能竹简 (shell)
│   └── FluorescentHydraStaff.cs               # 荧光水螅召唤杖 (shell)
├── Armors/
│   └── CrimsonMoonAlgae/                      # 红月水藻 set (full, 4 pieces)
│       ├── CrimsonMoonAlgaeHeaddress.cs
│       ├── CrimsonMoonAlgaeMask.cs
│       ├── CrimsonMoonAlgaeBreastPlate.cs
│       └── CrimsonMoonAlgaeGreaves.cs
├── Accessories/  BambooStepTalisman.cs        # 竹节步符 (shell)
│                 PeachBranchAmulet.cs         # 桃枝护符 (shell)
├── Pets/         PandaPet.cs                  # 熊猫宠物 (shell)
├── Placeables/   RegionalCraftingStation.cs   # 区域放置物品制作台 (shell, no tile)
├── Misc/         PeachBlossomKite.cs          # 桃花纸鸢（风筝）(shell)
│                 AlcoholicDrinks.cs           # 若干酒类 (shell)
│                 DiscipleVanity.cs            # 弟子时装 (shell)
│                 JadeSnakeEgg.cs              # 灵蛇玉卵 (full, use item)
└── Vanity/       BambooHairpin.cs             # 竹簪子 (shell; mirrors WitheredMask category)
```

Existing `Armors/` contains only `DevilHeart`, `Molluscs`, `Ruin`, `Witherbark`; the new `CrimsonMoonAlgae` subfolder is consistent with that pattern. Existing `Items/Misc` holds the Phase 1 fallback precedent (`ForestBreath`, `WitheredMask`).

### Pattern 1: Art-missing item identity (Phase 1 verified precedent)

**What:** A `ModItem` whose approved art is absent overrides `Texture` with the Function shared fallback and records a blocker.
**When to use:** every one of the 9 full + 12 shell classes in this phase.
**Example (verbatim, `ForestBreath.cs:7-22`)** [VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs:7-22]:

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

### Pattern 2: Full weapon with a companion projectile (existing precedent)

**What:** Item declares stats and redirects `Item.shoot` to a module projectile.
**When to use:** `巨石弹射装置` (fully specified, no Phase-7 dependency) — mirror `GreenThornBallLauncher` [VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/GreenThornBallLauncher.cs:9-37]:

```csharp
public override void SetDefaults()
{
	Item.DamageType = DamageClass.Ranged;
	Item.damage = 25;
	Item.knockBack = 5.5f;
	Item.crit = 10;
	Item.useTime = Item.useAnimation = 35;
	Item.useStyle = ItemUseStyleID.Shoot;
	Item.noMelee = true;
	// ...
	Item.useAmmo = AmmoID.Bullet;
	Item.shoot = ProjectileID.Bullet;
	Item.shootSpeed = 12f;
}

public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
{
	type = ModContent.ProjectileType<GreenThornLauncher_Proj>();
	position += velocity * 4;
}
```

### Pattern 3: Armor set with per-player set state (existing precedent)

Use `[AutoloadEquip]` **only when the equip art exists** (see Pitfall 1). Stats/set logic mirror `Armors/Molluscs` [VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Molluscs/MossyMolluscsHelmet.cs:7-43]:

```csharp
[AutoloadEquip(EquipType.Head)]
public class MossyMolluscsHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;
	public override void SetDefaults() { Item.defense = 2; /* … */ }
	override public void UpdateEquip(Player player) { player.GetDamage<RangedDamageClass>() += 0.04f; }
	public override bool IsArmorSet(Item head, Item body, Item legs) { /* … */ }
	public override void UpdateArmorSet(Player player) { /* … */ }
}
```

### Pattern 4: Recipe (existing red-algae family — the set's ingredients already exist)

[VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMinionStaff.cs:57-64]:

```csharp
public override void AddRecipes()
{
	CreateRecipe()
	.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
	.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
	.AddTile(TileID.WorkBenches)
	.Register();
}
```

### Anti-Patterns to Avoid

- **`[AutoloadEquip]` with a fallback `Texture` override and no `_Head/_Body/_Legs.png`** — throws at load (Pitfall 1).
- **Referencing a Phase-7 item type in `AddRecipes`/`Item.shoot`** — compile error.
- **Creating or editing any `.png`** — forbidden by AGENTS and Phase 1 no-placeholder gates.
- **Overriding `Texture` while also expecting the class asset path to auto-resolve** — the override removes the default path.
- **Renaming an internal name after it is committed** — compatibility-sensitive (D-13/AGENTS).

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Equip slot when art is missing | Custom `player.head`/draw hooks | `EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, type, this, nameof(Class))` | tML owns equip registration/draw; matches `EquipLoader` contract. |
| Optional-art detection | Custom file IO / `File.Exists` | `RootContentSource.HasAsset(path)` / `ModContent.HasAsset(path)` | In-repo precedent `ItemGlowManager.cs`; works for packed mods. |
| Localization keys | Hand-written HJSON | nothing in this phase | D-20: localization is out of scope; do not touch HJSON. |
| Placeholder textures | Any generated/substituted image | `Commons.ModAsset.White_Mod` | AGENTS no-placeholder rule. |
| Item→NPC summon glue | Custom spawn packets | existing `Item.GetNPCSource_FromThis()` + `NPC.NewNPCDirect` (see `MossyRuby`) | Repo precedent; keep to tML APIs. |

**Key insight:** this phase's complexity is *policy* (missing art, progression order, blockers) not *algorithms*. The only genuinely tricky engine behaviour is equip-slot registration, and tML already provides the API — the risk is using the wrong one.

## Common Pitfalls

### Pitfall 1: `[AutoloadEquip]` + missing equip texture fails the whole mod load (HIGH severity)

**What goes wrong:** A class with `[AutoloadEquip(EquipType.Head)]` and `Texture => Commons.ModAsset.White_Mod` makes tML look for `Commons/Textures/White_Head`; that asset does not exist.
**Why it happens:** The ExampleMod docs state `[AutoloadEquip]` "will result in TML expecting a X_Head.png file to be placed next to the item's main texture" [CITED: github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Items/Armor/ExampleHood.cs]. The autoload path funnels into `EquipLoader.AddEquipTexture`, whose body begins `ModContent.Request<Texture2D>(texture); //ensure texture exists` [CITED: cdn.jsdelivr.net/gh/tModLoader/tModLoader@1.4.4/patches/tModLoader/Terraria/ModLoader/EquipLoader.cs]. A missing asset raises `MissingResourceException`, which disables/aborts mod loading [CITED: github.com/tModLoader/tModLoader/issues/4943].
**How to avoid:** For the four `红月水藻` armor pieces (and any vanity shell that needs an equip slot), **do not** use `[AutoloadEquip]` while the `_Head/_Body/_Legs` art is absent. Instead register the slot explicitly with the shared fallback, e.g. in `Load()` (client-guarded):

```csharp
// Recommended shape — verify the exact HasAsset path form at implementation time.
public override void Load()
{
	if (Main.dedServ)
	{
		return;
	}
	EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Head, this, nameof(CrimsonMoonAlgaeHeaddress));
}
```

This still requires the class to be the item's own registration for set balance, and D-18 permits recording a blocker rather than inventing art. When approved `X_Head.png` arrives, either point the explicit registration at the class asset path or migrate to `[AutoloadEquip]` (the D-14 "texture addition … not a class rework" intent). **The planner must pick one strategy and state it**; the naive `[AutoloadEquip]` + fallback combination is a guaranteed load failure.
**Warning signs:** `MissingResourceException`, "Failed to load asset", mod disabled in the tML log; `dotnet build` still succeeds (this is a runtime load error, invisible to the compiler).

### Pitfall 2: Recipes that reference non-existent Phase-7 items break the build

**What goes wrong:** `肌腱巨弓`, `限制机`, `腥臭的诱饵` design recipes name ingredients that are not in the repository (`血云母`, `血肉聚合物`, `隐生之眼`, `熔炉钢`, `干枯心脏`). `ModContent.ItemType<X>()` on an absent type is a compile error.
**Why it happens:** These are Giant-Winged-Dragon (Phase 7) materials by design; Phase 1 recorded them at `phase: 7` with `internal_name: ""` and blocker `"no repo implementation found"` [VERIFIED: 01-INVENTORY.json entry `biology_drop-weapons.misc-血肉聚合物`].
**How to avoid:** Do **not** write these recipes. Implement the item identity + stats and record a precise recipe blocker ("design recipe ingredient 血云母/血肉聚合物 is a Phase 7 Giant Winged Dragon item not present in the repository").
**Warning signs:** `CS0246`/`CS0117` on missing types; `dotnet build` failure.

### Pitfall 3: Art-missing class with no `Texture` override fails to load

**What goes wrong:** New classes in `KelpCurtain/Items/**` have no `.png`; a `ModItem` without a resolvable icon texture raises `MissingResourceException`.
**How to avoid:** Every class in this phase overrides `Texture => Commons.ModAsset.White_Mod;` (Pattern 1). Do not rely on a future `.png`.
**Warning signs:** tML log "Failed to load asset" for `<module>/Items/...`; `dotnet build` still succeeds.

### Pitfall 4: No-placeholder guard regression

**What goes wrong:** Any added/modified `.png` under the items tree fails Phase 1's gates and violates AGENTS.
**How to avoid:** The Phase 2 gate must repeat the guard `git status --porcelain -- Sources/Modules/Yggdrasil/KelpCurtain/Items` and reject added/modified `.png` — the exact pattern in `check-carryover.ps1:89-98` [VERIFIED: .planning/phases/01-item-inventory-completed-art-items/scripts/check-carryover.ps1:89-98].
**Warning signs:** `FAIL: placeholder guard : added/modified png ...`.

### Pitfall 5: Internal-name / compatibility drift

**What goes wrong:** Internal names (`Everglow.Yggdrasil.KelpCurtain.Items.…`) and the display key become compatibility-sensitive once committed; renaming later breaks saves/localization.
**How to avoid:** Fix the English internal names deliberately in the plan, record them in the inventory `internal_name`, and never rename (D-13, AGENTS).

### Pitfall 6: Rarity / value interpretation drift

**What goes wrong:** Design rarities (`浅橙`, `粉`, `蓝`) and values (`4g`, `2金`, `54000`) can be mis-mapped.
**How to avoid:** Phase 1 established the mapping: design 价格 → `Item.value` (raw for bare numbers, `Item.buyPrice(...)` for `Ng` forms) [VERIFIED: .planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md design-deviations table]. The existing red-algae family uses `ItemRarityID.Orange` and `Item.value = 35000` for the design's `浅橙`/`35000`; mirror that mapping and treat `浅橙 → ItemRarityID.Orange` as [ASSUMED] to be confirmed against the family.

### Pitfall 7: Withering scope — do not implement Phase-7-dependent behaviour

**What goes wrong:** `灵蛇玉卵` summons `苍翠灵蛇` and `腥臭的诱饵` summons `巨翼龙`; both encounters are Phase 7. Coding the summon against a non-existent NPC type is a compile error and duplicates Phase 7 scope.
**How to avoid:** Implement identity + documented value/use-style; record an effect/encounter blocker (Phase 1 `QuetzalsWish` precedent).

## Code Examples

Verified patterns from official/in-repo sources:

### Optional-art existence check (in-repo)
[VERIFIED: Sources/Everglow.Function/ItemGlowManager.cs:27-44] — note the `AsSpan(9)` strips the `"Everglow/"` prefix to obtain the module-relative asset path:

```csharp
var asset = ModIns.Mod.Assets;
var source = ModIns.Mod.RootContentSource;
// ...
var itemLocation = item.Texture.AsSpan(9);
var path = string.Concat(itemLocation, "_glow");
if (source.HasAsset(path)) { /* asset exists */ }
```

### Summon item (existing precedent for `灵蛇玉卵` / `腥臭的诱饵` identity)
[VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/Items/BossSummon/MossyRuby.cs:9-52] shows the `Item.usestyle`/`consumable`/`UseItem` shape; the actual NPC spawn is **out of scope** here.

### Player-state flag for set bonuses
[VERIFIED: Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs:45-51]:

```csharp
public override void ResetEffects()
{
	MolluscsLeggings = false;
	MolluscsSetBuff = false;
	RadialCarapace = false;
	CorrodedPearl = false;
}
```

If the 红月水藻 set needs new flags, add them to `KelpCurtainPlayer` following `RadialCarapace`/`MolluscsLeggings`.

## Design Data Extracted for the 9 Full Implementations

> Source: committed Phase 1 evidence snapshot. Tables/rows cited so the planner can re-read them.

`肌腱巨弓` (biology table `Q528dhm8XopDyExdUJUcLGIMn6e`, row `GJ9ZdHtZ1omJhpxDT8PcxBxVneb`) [VERIFIED: .planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml]:
`["肌腱巨弓","","","58Ra","强","12%","28（慢）","血云母+血肉聚合物+玉化龙骨","4g","粉","按住左键蓄力拉出大弓对Boss单位额外造成10%伤害","巨翼龙肌腱制成的巨大弓…"]`

`限制机-RE01` / `腥臭的诱饵` (same table, rows `EjlsdeZZUoyAFQx7XT5cTBDMnUh` / `VJhMdMbirobSspxpz9Ucekudn2s`) [VERIFIED: evidence/biology.xml]:
- `["限制机-RE01","","","18Su","弱","/","21（普通）","血云母+血肉聚合物+熔炉钢+隐生之眼","4g","粉","消耗15魔力召唤高速飞行的限制无人机…"]`
- `["腥臭的诱饵","","","/","血云母+干枯心脏","20S","蓝","召唤巨翼龙",""]` (design row is column-sparse — do not over-read it)

`巨石弹射装置` (item table `doxcnnJSAMpWWmUqUwHGBjnoX3d`, row `doxcnJ44UhuwvFY6HOKgBUNOrJd`) [VERIFIED: evidence/item.xml]:
`["巨石弹射装置","","","44 R","15","","77","","2金","橙色","发射器，不消耗子弹，发射巨石，巨石在撞击到物块或者敌怪后爆裂，直击造成150%伤害，碎裂爆开3~6块小石块，造成15%伤害"]`

`红月水藻` set (item table `D48ndhX87o9stux04tDc3aBonZd`) [VERIFIED: evidence/item.xml]:
- `["红月水藻头饰","","","","","","","防御8","54000","浅橙","魔法伤害+18%法力消耗-12%","红藻毒素Buff：不会造成伤害，但是会持续累积…施加的红藻毒素Buff持续时长翻倍…法力上限+50免疫和红藻相关的所有伤害。"]`
- `["红月水藻面具","","","","","","","防御11","54000","浅橙","召唤伤害+18%召唤上限+3"]`
- `["红月水藻板甲","","","","","","","防御16","72000","浅橙","受到大于等于10的伤害时治疗该伤害的15%"]`
- `["红月水藻护胫","","","","","","","防御4","48000","浅橙","速度增加12%，水下额外增加24%免疫红藻减速"]`
- Design shows **no recipe** for the set (the columns are 其他数值/价格/稀有度/效果/描述 only).

`灵蛇玉卵` (item table `doxcnF5NkT1L8TXx60UT6do5YKc`) [VERIFIED: evidence/item.xml]:
`["灵蛇玉卵","","","","","","","","10金","蓝色","在森雨幽谷顶部使用以召唤苍翠灵蛇",""]`

Existing consumables/materials the set can build on: `Items/Materials/CrimsonMoonSap.cs` (`Item.value = 5000`) and `Items/Placeables/JadeLakeRedAlgae_Item.cs` (`Item.value = 50`) [VERIFIED: those files]. Existing buffs include `Buffs/RedAlgaeDebuff.cs` and `Buffs/CrimsonMoonAlgaeSummonStaff_Buff.cs`.

## Shell Class Catalogue (12)

Each shell = minimal `ModItem` + `Texture => Commons.ModAsset.White_Mod` + `LocalizationCategory` + art/`system` blocker. Source table rows are empty for every field except the description [VERIFIED: evidence/item.xml tables `PI6XdIIVBoF8TbxlMAxcttdkn7f`, `WuYSdAC6lozlj1xJ0o0cwnYjn3b`, `MWZzdiSfNoFDmdxNeiscNELFnDh`, `doxcnlTJMIxnmaRlOltbxZDYzGc`].

| Design name | Suggested class | Folder | `LocalizationCategory` | Blocker kind |
|-------------|-----------------|--------|------------------------|--------------|
| 竹节步符 | `BambooStepTalisman` | `Accessories/` | `Accessories` | art pending |
| 竹制武器 | `BambooWeapon` | `Weapons/` | `MeleeWeapons` | art pending |
| 竹簪子 | `BambooHairpin` | `Vanity/` (or `Misc/`) | `Vanity` | art pending |
| 桃枝护符 | `PeachBranchAmulet` | `Accessories/` | `Accessories` | art pending |
| 桃花纸鸢（风筝） | `PeachBlossomKite` | `Misc/` | `Miscs` | art pending |
| 熊猫宠物 | `PandaPet` | `Pets/` | `Pets` | art pending |
| 若干酒类 | `AlcoholicDrinks` | `Misc/` | `Miscs` | art pending |
| 弟子剑 | `DiscipleSword` | `Weapons/` | `MeleeWeapons` | missing disciple system |
| 弟子时装 | `DiscipleVanity` | `Vanity/` (or `Misc/`) | `Vanity` | missing disciple system |
| 技能竹简 | `SkillBambooSlip` | `Weapons/` | `Miscs` | missing skill system |
| 区域放置物品制作台 | `RegionalCraftingStation` | `Placeables/` | `Placeables` | missing regional-crafting system (no `ModTile` → plain `ModItem`) |
| 荧光水螅召唤杖 | `FluorescentHydraStaff` | `Weapons/` | `SummonWeapons` | art pending |

## Verification Gate Design

Phase 1 gates are ASCII PowerShell 5.1 scripts under
`.planning/phases/01-item-inventory-completed-art-items/scripts/` that read the inventory with
`[IO.File]::ReadAllText` (UTF-8 safe) and resolve tracked classes with `git -C $repoRoot ls-files` [VERIFIED: scripts/check-carryover.ps1:27-98]. Phase 2 should add an analogous `scripts/check-phase2.ps1`:

1. **Selection:** entries with `phase == 2` (after planning applies D-16, this is 21 non-deferred entries; 3 remain `deferred == true`).
2. **Class coverage:** each non-deferred entry has a non-empty `internal_name` whose short name maps to a git-tracked `.cs` under `Sources/Modules/Yggdrasil/KelpCurtain/Items`.
3. **Status invariance:** each selected entry has `code_complete == true`, `artwork_complete == false`, `status == "unchecked"`, and ≥1 blocker matching `texture|artwork|system|effect|recipe` (precise blocker, not just a generic marker).
4. **Deferral preservation:** `item-weapons.misc-a`, `-b`, `-c-名字要普通` remain `deferred == true` with a non-empty `deferred_reason`; no class file exists for them.
5. **Reallocation:** `biology_drop-weapons.misc-巨翼龙面具` has `phase == 7` and `advances` contains `ITEM-06`.
6. **Classification manifest:** a `02-CLASSIFICATION.json` with 21 rows `{id, mode: "full"|"shell", class_file, blocker_kind}`; counts must be exactly 9 full / 12 shell.
7. **No-placeholder guard:** `git status --porcelain -- Sources/Modules/Yggdrasil/KelpCurtain/Items` must not contain added/modified `.png` (copy the Phase 1 substring logic).
8. **Build gate:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 with no `error CS`.

## Validation Architecture

> `workflow.nyquist_validation` is `true` in `.planning/config.json`, so this section is included.

### Test Framework

| Property | Value |
|----------|-------|
| Framework | MSTest 3.10.2 (`MSTest.TestAdapter`/`MSTest.TestFramework`), `Microsoft.NET.Test.Sdk` 17.14.1 |
| Config file | `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj` (project references `Everglow.Yggdrasil`) |
| Quick run command | `dotnet test --filter "FullyQualifiedName~Yggdrasil" /p:WarningLevel=0` |
| Full suite command | `dotnet test --verbosity normal /p:WarningLevel=0` |
| Structural gate | `pwsh -File .planning/phases/02-…/scripts/check-phase2.ps1` (ASCII PowerShell 5.1) |
| Build gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |

**Constraint:** MSTest cannot construct `Main` or load mod content; per AGENTS, tests touching `Terraria.Main` must set `Program.SavePath = string.Empty;` and not start graphics/content loading. Item classes are therefore **not** unit-testable here.

### Phase Requirements → Test Map

| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|--------------|
| ITEM-01 | 9 full + 12 shell natural weapons/utility/drops exist and load with fallback | structural + build | `check-phase2.ps1`; `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ❌ Wave 0 (script) |
| ITEM-02 | 红月水藻 4-piece armor set equippable + set bonuses | structural + runtime (human) | `check-phase2.ps1`; client equip check | ❌ Wave 0 (script); runtime = manual |
| ITEM-03 | `巨石弹射装置` / `灵蛇玉卵` usable per design; source/access conditions | runtime (human) | client craft/use check | manual |
| ITEM-04 | `灵蛇玉卵` use-at-location summon condition | runtime (human) | client check (encounter is Phase 7 → blocked) | manual |
| — | No placeholder art introduced | structural | `check-phase2.ps1` no-placeholder guard | ❌ Wave 0 (script) |
| — | Inventory marking unchanged from D-11/D-22 | structural | `check-phase2.ps1` status invariance | ❌ Wave 0 (script) |

### Sampling Rate

- **Per task commit:** `check-phase2.ps1` (fast, deterministic).
- **Per wave merge:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` (exit 0, no `error CS`).
- **Phase gate:** full `dotnet test` green + `check-phase2.ps1` green + human client runtime verification of representative items (D-21) before `/gsd-verify-work`.

### Wave 0 Gaps

- [ ] `scripts/check-phase2.ps1` — coverage, classification, deferral, no-placeholder, status invariance.
- [ ] `02-CLASSIFICATION.json` — 21-row `{id, mode, class_file, blocker_kind}` manifest (9 full / 12 shell).
- [ ] (Optional) a pure-logic MSTest only if a helper is extracted; otherwise no new test file — item content is runtime-verified.
- [ ] If approved art is later added: no test change, remove the artwork blocker and re-run `check-phase2.ps1`.

## Security Domain

`security_enforcement` is enabled (`security_asvs_level: 1`). This phase adds offline content definitions only; there is no network, auth, session, crypto, or external-input surface.

### Applicable ASVS Categories

| ASVS Category | Applies | Standard Control |
|---------------|---------|-----------------|
| V2 Authentication | no | — (no auth surface) |
| V3 Session Management | no | — |
| V4 Access Control | no | — |
| V5 Input Validation | no | No external input parsed by this phase. |
| V6 Cryptography | no | — (never hand-roll; none needed) |

### Known Threat Patterns for this stack

| Pattern | STRIDE | Standard Mitigation |
|---------|--------|---------------------|
| Item code mutating authoritative multiplayer state without sync | Tampering | If any item adds per-player state, use `KelpCurtainPlayer` + `ModIns.PacketResolver` (Phase 1 `ArmOfGiantTreeCharge` precedent) and set `netUpdate`. This phase is expected to need none. |
| Content loaded on dedicated servers touching graphics | Denial of Service | Guard any equip/asset `Load()` with `if (Main.dedServ) return;` (existing `ShellMolluscsBreastPlate.cs:18-27` precedent). |

## Assumptions Log

| # | Claim | Section | Risk if Wrong |
|---|-------|---------|---------------|
| A1 | Suggested English internal names (e.g. `TendonGreatbow`, `CrimsonMoonAlgaeHeaddress`, `RestrictionDeviceRE01`) | Project structure / Shell catalogue | Compatibility-sensitive; renaming later breaks saves/localization. Planner must fix names before coding. |
| A2 | Design rarity `浅橙` maps to `ItemRarityID.Orange` (matching the existing red-algae family) | Pitfalls 6 | Wrong rarity colour. Confirm against `RedAlgaeMagicStaff.cs` / `RedAlgaeMinionStaff.cs`. |
| A3 | `红月水藻` set recipe is not in the design; plan mirrors the family recipe (`CrimsonMoonSap` + `JadeLakeRedAlgae_Item` @ Work Benches) | Design data / Pattern 4 | If the designer intended a different recipe, progression may differ; record as a deviation/assumption. |
| A4 | `巨石弹射装置` gets a real projectile implementation (self-contained, no Phase-7 dependency) rather than an effect blocker | Summary / Pattern 2 | If the planner defers the projectile, use an effect blocker instead (D-18 discretion). |
| A5 | The exact optional-asset path form for the armor fallback helper (`HasAsset` + module-relative path) | Pitfall 1 / Code examples | Must be verified at implementation time against `ItemGlowManager.cs` and tML's `EquipLoader`; a wrong path silently falls back or throws. |
| A6 | `巨翼龙面具` reallocation (D-16) is applied to `01-INVENTORY.json` during planning/execution | Gate design | If not applied, the selection count is 22 not 21 and the gate miscounts. |

**If this table is empty:** not applicable — see rows above; A1/A2/A3/A5 need confirmation before becoming locked decisions.

## Open Questions

1. **Armor fallback strategy (highest priority).**
   - What we know: `[AutoloadEquip]` requires `_Head/_Body/_Legs.png`; `EquipLoader.AddEquipTexture` throws on a missing texture; no placeholder art may be created.
   - What's unclear: whether the team prefers (a) explicit `EquipLoader.AddEquipTexture` with `White_Mod` (equippable, white-box visuals) or (b) omit the equip slot and record a blocker (not equippable until art).
   - Recommendation: choose (a) for the 4 armor pieces so ITEM-02 is actually usable; also author the helper to detect future art so D-14 needs no class edit. Confirm with the user if a white-box body is unacceptable.

2. **`红月水藻` recipe source.**
   - What we know: the design table shows no recipe; the family's existing weapons use `CrimsonMoonSap` + `JadeLakeRedAlgae_Item` at a Work Bench.
   - What's unclear: exact quantities/tile for the armor.
   - Recommendation: mirror the family pattern, record as an assumption/deviation (A3).

3. **`腥臭的诱饵` design row is column-shifted/sparse.**
   - What we know: row text is `["腥臭的诱饵","","","/","血云母+干枯心脏","20S","蓝","召唤巨翼龙",""]`.
   - What's unclear: which value is the use-time/value and whether rarity is truly Blue.
   - Recommendation: implement identity + `SummonItems` category + record both the recipe and the missing-encounter blocker rather than guess.

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|------------|-----------|---------|----------|
| .NET SDK | Build/test | ✓ | 9.0.306 (targets net8.0) | — |
| tModLoader install (`tModLoader.targets`) | Build + client runtime | ✓ | found at `..\..\..\tModLoader.targets` (ancestor) | — |
| Git | Structural gates (`git ls-files`, `git status`) | ✓ | 2.37.3.windows | — |
| PowerShell | Gate scripts (ASCII, PS 5.1 semantics) | ✓ | 5.1.26100.9168 | — |
| Node.js | Used for this research's JSON/XML extraction only | ✓ | v22.18.0 | — |
| tModLoader **client** (runtime verification) | D-21 | not probed (interactive) | — | Schedule human client session; offline gates + build still run. |

**Missing dependencies with no fallback:** none blocking the offline/build work.
**Missing dependencies with fallback:** in-client runtime verification is human-scheduled (D-21); offline gates and the Release build do not depend on it.

## Sources

### Primary (HIGH confidence)
- `01-INVENTORY.json` (`entries[phase==2]`, `deferred`, `blockers`, `feishu.block_id`) — the 25-row selection and per-entry fields.
- `evidence/item.xml`, `evidence/biology.xml` — design rows/tables quoted above (committed Phase 1 snapshot).
- `Sources/Everglow.Function/Utilities/LocalizationUtils.cs:19-50` — category constants.
- `Sources/Directory.Build.props:12`, `Sources/Modules/Directory.Build.props:4-5` — `EnablePathGenerator`, `ModuleName`/`PathPrefix`.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/**` — `ForestBreath`, `WitheredMask`, `QuetzalsWish`, `RadialCarapace`, `Molluscs*`, `RedAlgaeMinionStaff`, `GreenThornBallLauncher`, `MossyCyatheaBow`, `MossyRuby`.
- `Sources/Everglow.Function/ItemGlowManager.cs:27-44` — optional-asset detection precedent.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-*.ps1` — gate conventions.

### Secondary (MEDIUM confidence)
- [tModLoader `EquipLoader.cs` (1.4.4)](https://cdn.jsdelivr.net/gh/tModLoader/tModLoader@1.4.4/patches/tModLoader/Terraria/ModLoader/EquipLoader.cs) — `AddEquipTexture` "ensure texture exists".
- [tModLoader ExampleMod `ExampleHood.cs`](https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Items/Armor/ExampleHood.cs) — `[AutoloadEquip]` expects `X_Head.png`.
- [tModLoader issue #4943](https://github.com/tModLoader/tModLoader/issues/4943) — `MissingResourceException` disables a mod.

### Tertiary (LOW confidence)
- Rarity mapping `浅橙 → ItemRarityID.Orange` (A2) — inferred from the existing red-algae family, not design-authored.

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — no new packages; repo APIs and fallback are read directly.
- Architecture: HIGH — existing item/armor/recipe/player-state patterns are all present in the KelpCurtain tree.
- Pitfalls: HIGH for the equip-load hazard (official tML source/docs) and recipe dependency (grep-verified absence); MEDIUM for rarity mapping and shell placement.

**Research date:** 2026-09-14
**Valid until:** 2026-10-14 (stable; tML 1.4 content APIs)
