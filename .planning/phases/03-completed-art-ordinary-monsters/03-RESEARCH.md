# Phase 3: Completed-Art Ordinary Monsters - Research

**Researched:** 2026-09-15
**Domain:** Terraria tModLoader `ModNPC` content (Kelp Curtain, Yggdrasil Subworld) + offline design-source matrix/gate
**Confidence:** HIGH on in-repo mechanics; MEDIUM on the tranche rule; LOW on drop availability (see Summary)

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

#### Source Comparison & Biology Matrix
- **D-24:** A new `03-BIOLOGY.json` is the machine source of truth for Phase 3–4 creature status (one row per creature: id, region, texture-complete, code-complete, `status`, `blockers`, deferred), mirrored by a human-readable `03-BIOLOGY.md`, following the Phase 1 `01-INVENTORY.json`/`.md` convention. — **Reversibility:** costly — Phase 4 and Phase 8 consume this artifact; changing its id/schema later forces re-reconciliation of both phases.
- **D-25:** The matrix is derived only from the **already-committed** `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` snapshot. Do **not** re-fetch the Feishu document and do **not** mutate the design source.
- **D-26:** "Texture-complete" (i.e. belongs to Phase 3, not Phase 4) is decided **solely** by the texture cell / checkbox of the biology design row, matching the Phase 1 `artwork_complete` rule. Repository `.png` presence is not the criterion.
- **D-27:** A new 100% ASCII PowerShell 5.1 `scripts/check-biology.ps1` gate validates coverage count, per-row classification, per-row blocker presence, and a no-placeholder-art guard, mirroring the Phase 1/2 `check-*.ps1` conventions (read JSON via `[IO.File]::ReadAllText`; no `git ls-files` dependency for newly created files). It must run green before and after each plan wave.

#### Behavior Implementation Depth
- **D-28:** Ordinary creatures whose design row is complete (texture checkbox on, behavior described) get a **full implementation**: spawn, AI/movement, hostility relationships, status effects, drop wiring, hit/attack behavior. — **Reversibility:** costly — creature internal names and AI parameters become compatibility-sensitive once committed.
- **D-29:** Creatures that are texture-complete but have **no defined behavior** get an **identity-only shell class** (loadable `ModNPC` with documented stats/`LocalizationCategory` and the D-13-style fallback texture policy) plus a precise blocker — never invent behavior (extends the D-18 spirit).
- **D-30:** For creatures depending on **unimplemented systems** (capture rules, status-effect systems, variants, morale) implement only the independently-completable part and record a precise blocker naming the missing system and the affected files (the Phase 1/2 "outside this plan's file scope" precedent). Do **not** implement those systems in this phase.
- **D-31:** AI is implemented with vanilla `aiStyle` where it fits, plus locally-written `AI()` only when necessary, reusing existing KelpCurtain/Yggdrasil NPC precedents (`NPCs/RiverSlug.cs`, `NPCs/VampireMat/*`). Do **not** introduce a new generic creature base class/interface.

#### Spawn & Scene Integration
- **D-32:** Every ordinary creature spawns **only inside the Yggdrasil Subworld** in its designed Kelp Curtain region, isolated so it never leaks into the main world (BIO-06 hard constraint).
- **D-33:** Spawning reuses the repository's existing Yggdrasil Subworld / spawn-system precedent rather than introducing a parallel spawn system; the exact hook is confirmed during research.
- **D-34:** Spawn conditions (region/depth/water/time) follow the design row; where the design supplies no weight/condition, use conservative defaults and record them as assumptions in `03-DEVIATIONS.md`.
- **D-35:** Subworld spawn/behavior is server-authoritative; client-only graphics/VFX work is guarded by `!Main.dedServ`, consistent with Phases 1–2.

#### Drop Wiring
- **D-36:** Drops use direct drop tables (`NPCLoot()` / `ItemDropRule`) with explicit chance and quantity, consistent with existing repository NPC precedents.
- **D-37:** Drops reference **only items already implemented in Phases 1–2** (`ModContent.ItemType<...>`). This phase introduces **no new item scope**; any missing drop is a recorded blocker.
- **D-38:** Drop probability/quantity follows the design row; where absent, use conservative defaults recorded in `03-DEVIATIONS.md`.
- **D-39:** When a designed drop is not yet implemented, leave that entry out of the drop table (partial/empty is allowed) and record a precise blocker with the affected item — the mod must still build and load.

#### Carried Forward (confirmed)
- **D-40:** Phase 2 decisions remain in force: **D-20** localization is out of scope for every phase (record in the deferred ledger only); **D-21** in-client runtime verification is required in addition to offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0`; **D-22** marking rules (code-complete + art-incomplete → `code_complete=true` / `artwork_complete=false`, row unchecked/no colour); **D-23** when implemented code conflicts with the design, follow the code unless the Feishu row is yellow with a corresponding explanation.

### the agent's Discretion
- NPC folder layout and class naming under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/`, and which adjacent existing NPC to mirror.
- Exact shell-class contents (minimal `SetDefaults` values, stats) and blocker wording.
- Which vanilla `aiStyle` fits a given creature and where a small custom `AI()` is warranted.
- Whether a creature's projectile/buff/VFX class is added now or recorded as a precise effect blocker for a later phase.
- Exact conservative default spawn weights / drop chances where the design is silent.

### Deferred Ideas (OUT OF SCOPE)
- **Localization (all phases)** — excluded by user directive; ITEM-07 deferred. Record in the deferred ledger only (D-20).
- **Unfinished-art creatures** — belong to Phase 4 (Remaining Ordinary Monsters).
- **Boss / special-encounter creatures** (Klein Snake, Giant Winged Dragon) — Phase 7.
- **Unimplemented systems** implied by creatures (capture rules, status-effect systems, variants, morale) — recorded as precise blockers, not implemented here (D-30).
- **Runtime verification** — requires a live tModLoader client; scheduled when convenient (D-21).

</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| BIO-01 (completed-art tranche) | Death Jade Lake implements the designed aquatic, amphibious, surface-water, passive, and predator creatures with water/land behavior, spawning rules, hostility relationships, status effects, capture rules, and drops. | All 7 artwork-bearing creatures in the snapshot belong to the Death Jade Lake h1 (`JITOdS5VOofnlCxo5tecFn7n4g`); stats, behavior text, immunities and drop lines are extracted verbatim in "Design Data Extracted" below; spawn hook confirmed (`NPCSpawnManager` + `KelpCurtainBiome`/`DeathJadeLakeBiome`). |
| BIO-02 (completed-art tranche) | Spiny Moss Court implements the designed Witherbark soldiers, command unit, giant tree, Brody dragonfly, and associated combat states, morale behavior, variants, and drops. | **No Spiny Moss Court creature carries design artwork in the snapshot** → the whole h1 (枯木活化士兵, 王庭号令者, 巨树人, 布罗迪蝇蜓) falls to Phase 4 under the artwork rule. Phase 3 only records them in the matrix as `texture_complete=false` / deferred-to-Phase-4 rows. |
| BIO-03 (completed-art tranche) | Valley of Lush and Moist implements the designed common creatures, disguised hazards, neutral creatures, mini-boss encounter, and related drops and behavior. | **No Valley creature carries design artwork** → the whole h1 falls to Phase 4. Two Phase-3 creatures additionally *spawn* in the Valley per the design (叶飞棍, 覆藻章鱼). |
| BIO-06 | Creature behavior, environmental interaction, status effects, spawn weights, and combat difficulty are consistent with the three source design documents and do not introduce unintended main-world behavior. | Enforced by gating `SpawnChance` on a subworld-dependent biome predicate (`KelpCurtainBiome.IsBiomeActive` requires `SubworldSystem.IsActive<YggdrasilWorld>()`) — see Pattern 1 and Pitfall 3. |
| QUAL-03 | Multiplayer behavior is verified for every changed networked, persistent, subworld, NPC, projectile, quest, and reward system; dedicated-server safety is checked where applicable. | Server-authoritative AI (`Main.netMode != NetmodeID.MultiplayerClient` gates on random/state writes) + `NPC.netUpdate` + `spawnInfo.player` (not `Main.LocalPlayer`) in `SpawnChance`; `!Main.dedServ` for dust/VFX. |
| QUAL-04 | Every design item is individually compared with the implementation after verification; exact matches green-checked, conflicts yellow, incomplete checkboxed rows unchecked. | Phase 3 produces the per-creature comparison record (`03-BIOLOGY.json`). Human Feishu writes stay out of this phase (Phase 8). |

</phase_requirements>

## Summary

Phase 3 is a **`ModNPC` content phase**, not a design-parsing phase: the committed `evidence/biology.xml` snapshot is complete and already exists, and the repository has a working subworld spawn system, two NPC precedents and a `!Main.dedServ` convention. Three findings dominate planning:

**Finding 1 — the snapshot has no per-creature texture checkbox.** `evidence/biology.xml` contains exactly **29 tables**, of which only **2** have a name header (`物品名`/`贴图`/`代码`) — and both are the *Giant Winged Dragon item tables* (`Q0ujdFxxrodsRmxhREzciEC9n8b`, `Q528dhm8XopDyExdUJUcLGIMn6e`). The file contains **22 `<checkbox>` elements** (44 lines), all inside those two tables. Creature stat tables use the header `类型 | 生命 | 伤害 | 防御 | 击退抗性 % | 减伤 % | 免疫 | 钱币（铜） | 稀有度` and carry **no texture/code column and no checkbox**. The Phase 1 parser explicitly skips them: `parse-design-xml.ps1:341-345` — *"Not an item/armour/drop table (for example a creature stat table). Iterated for audit only; never emitted as an entry."* Consequently **D-26 cannot be applied literally**: there is no "texture cell / checkbox of the biology design row" for any creature. The only per-creature artwork marker the design source has is the **inline design image** (`<img ... name="image.png" width=… height=…>` attached to a creature's `h2`). **7 ordinary creatures carry one** (all in 亡碧湖 / Death Jade Lake). This research therefore proposes the inline design image as the D-26 proxy and flags it as a **checkpoint decision** (Open Question 1) — it is the single most consequential unknown in the phase.

**Finding 2 — none of the completed-art creatures' designed drops exist as items.** The tranche's 7 creatures drop only 毒腺, 牛黄, 软体甲壳碎片 and 亡碧膏 (plus "待定" weapon/accessory drops). A full-text search of all three committed evidence documents and the whole working tree shows **`软体甲壳碎片`, `亡碧膏`, `毒腺`, `牛黄`, `飞棍毛发`, `干涸心脏` appear in the biology design but in NO item/terrain design row and in NO repository file**; `枯木碎块` appears in `item.xml` only as NPC-shop trade currency (`使用10枯木碎块交易`), never as a row. Conversely, **every implemented biology drop** (放射状甲壳, 格普螺外壳, 荆棘龟壳, 肉食性提灯, 灵灯, 活化之犬召唤杖, 巨树之臂, 硬化枯木心脏, 巨石弹射装置) belongs to a creature that has **no** design artwork (放射虫, 格普螺, 荆棘苔龟, 碧灵鮟鱇, 枯木活化士兵, 巨树人). The tranche split and the drop inventory are **disjoint**. Under D-37/D-39 this means Phase 3's drop tables are legitimately **empty for every new creature** and each entry carries a material blocker; success criterion 2 is satisfied only vacuously for this tranche (see Open Question 2).

**Finding 3 — one of the seven is already implemented.** `NPCs/RiverSlug.cs` *is* 水蛞蝓 (design image 36×27; repo `RiverSlug.png` is 36 px wide; `RiverSlugItem.bait = 30` implements the design's "提供渔力"). It is registered as a critter (`Main.npcCatchable`, `CountsAsCritter`, `catchItem`, `SpawnModBiomes`), so `code_complete=true` for that row. The other six need new `ModNPC` classes and have **no repository texture** → D-13-style `Commons.ModAsset.White_Mod` fallback + artwork blocker (Phase 2 precedent), never new placeholder art.

**Primary recommendation:** run a one-question checkpoint to confirm the tranche rule (inline design image = texture-complete, D-26 proxy), then plan: Wave 1 = `03-BIOLOGY.json`/`.md` + `scripts/check-biology.ps1`; Waves 2–N = six `ModNPC` classes (spawn gating, AI, immunities, capture wiring, empty-but-documented loot tables) + `03-DEVIATIONS.md`; final wave = matrix/gate reconcilation, build, and the D-21 client verification bundle.

## Architectural Responsibility Map

| Capability | Primary Tier | Secondary Tier | Rationale |
|------------|--------------|----------------|-----------|
| Creature spawn placement (region/depth/water/time) | tML `ModNPC.SpawnChance` + `NPCSpawnManager.EditSpawnPool` (SP/server only) | `KelpCurtainBiome` / `DeathJadeLakeBiome` predicates | Spawn weights and pooling are a tML server-side concern; biome objects own the world-context test (`SubworldSystem.IsActive<YggdrasilWorld>()`). |
| Subworld isolation (BIO-06) | Biome predicate inside `SpawnChance` | `NPCSpawnManager.EditSpawnPool` filter | `EditSpawnPool` only *filters* while inside Yggdrasil; it returns early outside, so isolation must be enforced by the per-creature `SpawnChance` returning 0. |
| Creature behavior / AI | `ModNPC.AI()` (module: `Everglow.Yggdrasil.KelpCurtain.NPCs`) | Vanilla `aiStyle` when it fits | AI runs on server/SP; authority is the server. |
| Hostility / attack / contact damage | `ModNPC.SetDefaults` + `CanHitPlayer`/`OnHitPlayer` | Enemy projectile classes under `KelpCurtain/Projectiles/Enemies` | Damage and debuff application are gameplay state. |
| Status-effect application (中毒/潮湿/困惑/减速/黑暗/窒息/束缚) | `OnHitPlayer` / `ModifyHitPlayer` / `AddBuff` on the NPC | Existing buff infrastructure | Debuffs are gameplay; `BuffID.Poisoned` etc. are used directly elsewhere in the module. |
| Capture (捕捉) | `ModNPC.SetStaticDefaults` (`Main.npcCatchable`, `NPCID.Sets.CountsAsCritter`) + `NPC.catchItem` | Critter `ModItem` | tML's built-in capture path; do not hand-roll. |
| Drop wiring | `ModNPC.ModifyNPCLoot` / `NPCLoot` with `ItemDropRule` | Phase 1–2 `ModItem` types | Structured loot tables are engine-owned; the phase only *references* implemented items. |
| Creature identity/status record | `03-BIOLOGY.json` (machine) + `03-BIOLOGY.md` (mirror) | `scripts/check-biology.ps1` | Planning artifact, mirroring Phase 1 `01-INVENTORY.json`/`.md`. |
| Client-only visuals (dust, glow, ink VFX) | `HitEffect`/`AI` dust blocks guarded by `if (!Main.dedServ)` | `KelpCurtain/VFXs`, `KelpCurtain/Dusts` | Dedicated servers have no graphics services. |
| Localization keys | **Out of scope** (D-20) | — | Localization deferred for every phase; do not hand-edit HJSON. |

## Standard Stack

**This phase installs no external packages.** It is pure in-repo C# content plus offline PowerShell gates, so the "Standard Stack" is the existing repository toolchain. All versions below were read from the committed project files in this session.

### Core

| Library / Tool | Version | Purpose | Why Standard |
|----------------|---------|---------|--------------|
| tModLoader API (`Terraria.ModLoader`) | supplied by the local tML install; project targets `net8.0` | `ModNPC`, `NPCSpawnInfo`, `NPCLoot`, `ItemDropRule`, `ModBiome`, `Subworld` | The only supported modding surface [VERIFIED: `Sources/Directory.Build.props:3` `<TargetFramework>net8.0</TargetFramework>`]. |
| SubworldLibrary (`Libraries/SubworldLibrary.dll`) | repository-pinned DLL | `SubworldSystem.IsActive<YggdrasilWorld>()` context checks | Already a module reference; must not be replaced [VERIFIED: `Sources/Modules/Directory.Build.props:14-16` `<Reference Include="$(MSBuildThisFileDirectory)\..\..\Libraries\*.dll" />`]. |
| Solaestas.tModLoader.ModBuilder | `1.5.11` | Source-generated `ModAsset` path members | Existing asset-access convention; do not hand-write paths [VERIFIED: `Sources/Directory.Build.props:23`]. |
| MSTest (`Everglow.UnitTests`) | `3.10.2` (per AGENTS.md) | Pure-logic unit tests only | Exists, but cannot construct `Main` or load mod content — no unit-test path for `ModNPC` content (see Validation Architecture). |

### Supporting

| Tool | Version | Purpose | When to Use |
|------|---------|---------|-------------|
| .NET SDK | `9.0.306` present (builds `net8.0`) | `dotnet build` / `dotnet test` | Every code change [VERIFIED: `dotnet --version` = 9.0.306 this session]. |
| Windows PowerShell | `5.1.26100.9168` | `scripts/check-biology.ps1` ASCII gate | Before and after each plan wave (D-27) [VERIFIED: `$PSVersionTable.PSVersion` this session]. |
| Git | `2.37.3.windows.1` | BOM/no-art guards, commits | Repo-wide BOM check after text edits (AGENTS.md) [VERIFIED: `git --version` this session]. |
| StyleCop.Analyzers.Unstable | `1.2.0.556` | Style enforcement during build | Automatic; keep tabs/LF/Allman/file-scoped namespaces [VERIFIED: `Sources/Directory.Build.props:24-27`]. |

### Alternatives Considered

| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| `NPCSpawnManager.RegisterNPC` + `SpawnChance` | A new Kelp-Curtain-specific spawn `ModSystem` | Rejected by D-33; the existing system is used by 17 NPCs and already implements subworld filtering. |
| Vanilla `aiStyle` reuse | Fully custom AI for every creature | D-31 says prefer vanilla `aiStyle`; the repo precedent is `aiStyle = 3` for simple walkers (`CyanOreBeetle`) and `aiStyle = -1` + custom `AI()` for everything else (`RiverSlug`, `VampireMat`). |
| `Commons.ModAsset.White_Mod` fallback | New placeholder `.png` | Forbidden by AGENTS.md ("Never modify existing binary or art assets… Do not create placeholder art"). |
| `Pandora`-style loot helpers | `ItemDropRule.Common(...)` in `ModifyNPCLoot` | D-36 locks direct drop tables; `VampireMat`/`BarkSpicyCaterpillar` already use `ItemDropRule`. |

**Installation:** none — no `npm install`, `pip`, or new `PackageReference`. Existing build command:

```powershell
dotnet build /p:Configuration=Release /p:WarningLevel=0
```

**Version verification:** no new package is recommended, so no registry verification is required. The only versioned inputs are the existing `csproj`/`Directory.Build.props` entries quoted above (all read from the working tree this session).

## Package Legitimacy Audit

**Not applicable — this phase installs no external packages.** The phase adds C# source files under an existing module and one offline PowerShell script; it references no new NuGet package, DLL, or registry artifact. No `gsd-tools query package-legitimacy check` run is required.

| Package | Registry | Age | Downloads | Source Repo | Verdict | Disposition |
|---------|----------|-----|-----------|-------------|---------|-------------|
| — (none added) | — | — | — | — | — | — |

**Packages removed due to [SLOP] verdict:** none
**Packages flagged as suspicious [SUS]:** none

## Architecture Patterns

### System Architecture Diagram

```
Feishu 生物策划案 (external, READ-ONLY)
        │  --doc-format xml --detail full  (Phase 1, already committed)
        ▼
.planning/phases/01-.../evidence/biology.xml        <-- D-25 sole data source
   h1 region  →  h2 creature  →  {<img> design art | stats <table> | behavior <p> | drop <p>}
        │  (offline extraction; no re-fetch)
        ▼
03-BIOLOGY.json  (rows: creature id, region, texture_complete, code_complete, status, blockers, deferred)
        │                                    ▲
        │  mirrored by                       │ read by
        ▼                                    │
03-BIOLOGY.md  ──────────────────────►  scripts/check-biology.ps1   <-- D-27 gate
                                             (coverage / classification / blockers / no-placeholder-art)

RUNTIME (in-game, Yggdrasil Subworld only)

  player enters Yggdrasil ─► tML builds spawn pool
        │
        ▼
  NPCSpawnManager.EditSpawnPool()  ── not in Yggdrasil? ──► return (pool untouched)
        │ in Yggdrasil
        ▼
  keep only Registered NPC types (+ vanilla slimes)
        │
        ▼
  ModNPC.SpawnChance(spawnInfo)   ── outside KelpCurtain biome? ──► 0f   (BIO-06 isolation)
        │ in region / correct water depth / correct time
        ▼
  NPC spawns ─► ModNPC.AI()  ──► attack / status effects / capture / variants
        │
        ▼
  NPC death ─► ModifyNPCLoot(npcloot) ─► ItemDropRule.Common(ModContent.ItemType<Phase1/2 item>, …)
        │                                        └─ missing item → NO rule + blocker (D-39)
        ▼
  item reaches player
```

### Recommended Project Structure

```text
Sources/Modules/Yggdrasil/KelpCurtain/
├── NPCs/                               # new ordinary creatures live here
│   ├── DeathJadeLake/                  # region-scoped folder (mirrors Tiles/DeathJadeLake)
│   │   ├── WaterStrider.cs             # 水黾          + WaterStrider.png (when art arrives)
│   │   ├── Axolotl.cs                  # 幽光蝾螈
│   │   ├── ArmoredShrimp.cs            # 装甲虾
│   │   ├── SailfinSnakehead.cs         # 帆鳍鳢
│   │   ├── AlgaeOctopus.cs             # 覆藻章鱼
│   │   └── LargeAlgaeOctopus.cs        # 大型覆藻章鱼
│   ├── RiverSlug.cs                    # 水蛞蝓 — ALREADY IMPLEMENTED (do not duplicate)
│   └── VampireMat/                     # boss precedent (Phase 7, do not touch)
├── Items/Critters/                      # critter capture items (RiverSlugItem precedent)
├── Projectiles/Enemies/                 # enemy attack projectiles (VampireMat precedent)
└── KelpCurtainBiome.cs                  # biome-active predicate reused for spawn gating

.planning/phases/03-completed-art-ordinary-monsters/
├── 03-BIOLOGY.json                      # D-24 machine matrix
├── 03-BIOLOGY.md                        # D-24 human mirror
├── 03-DEVIATIONS.md                     # D-34/D-38 conservative defaults + blockers
└── scripts/
    └── check-biology.ps1                # D-27 100% ASCII gate
```

### Pattern 1: Subworld-only spawn registration + biome-gated `SpawnChance`

**What:** Register the NPC type so the Yggdrasil pool keeps it, and make `SpawnChance` return 0 unless a subworld-dependent biome predicate is active. This is the repository's established Yggdrasil spawn precedent (D-33).

**When to use:** Every Phase 3 creature that should spawn naturally.

**Example (verified precedent, `Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/CyanOreBeetle.cs:10-16,30-…`):**

```csharp
public override void SetStaticDefaults()
{
	Main.npcFrameCount[NPC.type] = 5;
	NPCSpawnManager.RegisterNPC(Type);                       // line 13
	NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
}

public override float SpawnChance(NPCSpawnInfo spawnInfo)
{
	YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
	if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
	{
		return 0f;
	}

	return 8f;
}
```

`KelpCurtainBiome.IsBiomeActive` already hard-requires the subworld, so gating on it gives BIO-06 isolation for free:

```csharp
// Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs:35-48
public override bool IsBiomeActive(Player player)
{
	if (SubworldSystem.IsActive<YggdrasilWorld>())
	{
		if (Main.screenPosition.Y > Main.maxTilesY * 0.72f * 16 && Main.screenPosition.Y < Main.maxTilesY * 0.9f * 16)
		{
			if (player.Center.X >= FindClosestStratumBoundPointX(player) * 16)
			{
				return true;
			}
		}
	}
	return false;
}
```

The pool filter that makes `RegisterNPC` effective:

```csharp
// Sources/Modules/Yggdrasil/Common/NPCSpawnManager.cs:9-16
public static void RegisterNPC(int type) => yggdrasilNPC.Add(type);

public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
{
	if (!SubworldSystem.IsActive<YggdrasilWorld>())
	{
		return;
	}
```

**Critical caveat (drives Pitfall 3):** `EditSpawnPool` **returns early outside Yggdrasil**, leaving the vanilla-built pool untouched. Isolation therefore depends on `SpawnChance` returning `0f` outside the subworld. Never rely on the pool filter alone.

### Pattern 2: Direct drop table referencing only implemented items

**What:** `ModifyNPCLoot` + `ItemDropRule.Common(ModContent.ItemType<T>(), chanceDenominator, min, max)` with explicit chance and quantity (D-36). Missing designed drops are simply **not added** and are recorded as blockers (D-39).

**Precedent (verified):** `VampireMat.cs:951-956` (`ItemDropRule.Common(ModContent.ItemType<FleshOfVampireMat>(), 1, 1, 1)`), `BarkSpicyCaterpillar.cs:286-289` (`ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 1, 2)`).

**Phase 3 consequence:** for all six new creatures the table body is empty; the method should still exist so the intent is explicit and the gate can assert "no rule references an unimplemented item".

### Pattern 3: Identity-only shell (texture-complete but behavior undefined) — D-29

**What:** a loadable `ModNPC` with real design stats, the shared fallback texture, a `LocalizationCategory`, and no invented behavior. Mirrors Phase 2's item shells (D-18) and the `RadialCarapace` fallback precedent.

**Shape (verified fallback pattern, `KelpCurtain/Items/Accessories/RadialCarapace.cs:5-7`):**

```csharp
public override string Texture => Commons.ModAsset.White_Mod;
```

`ModNPC` supports `LocalizationCategory` through `ILocalizedModType` (`P:Terraria.ModLoader.ILocalizedModType.LocalizationCategory`: *"The category used by this modded content for use in localization keys. Localization keys follow the pattern of `Mods.{ModName}.{Category}.{ContentName}.{DataName}`"*). The category constant for creatures is `Categories.NPCs = "NPCs"` [VERIFIED: `Sources/Everglow.Function/Utilities/LocalizationUtils.cs:15` `public const string NPCs = "NPCs";`]. Existing enemy NPCs mostly rely on the default category (only `TownNPC_LiveInYggdrasil` overrides it), and `RiverSlug` has a key in `Mods.Everglow.NPCs.hjson` without an override — so an explicit override is optional but harmless and is what D-29 asks for.

### Pattern 4: Server-authoritative AI with `ai[]` wrappers and `netUpdate`

**What:** a private state `enum` plus named wrapper properties over `NPC.ai[]`/`localAI[]`, random/state writes gated by `Main.netMode != NetmodeID.MultiplayerClient`, and `NPC.netUpdate = true` when authoritative state changes (D-35). This is exactly the `RiverSlug` precedent:

```csharp
// Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs:30-45
private ActionState NPCActionState
{
	get => (ActionState)(int)NPC.ai[0];
	set => NPC.ai[0] = (int)value;
}
...
public int StuckDetectionTimer
{
	get => (int)NPC.localAI[2];
	set => NPC.localAI[2] = value;
}

// RiverSlug.cs:125-132 — authoritative random write
if (Main.netMode != NetmodeID.MultiplayerClient)
{
	if (FallFromBlockCounter == 0f && Main.rand.NextBool(FallChance))
	{
		FallFromBlockCounter = FallFromBlockMax;
		NPC.netUpdate = true;
	}
}
```

### Anti-Patterns to Avoid

- **New generic creature base class/interface:** forbidden by D-31; extend `ModNPC` directly or mirror an existing concrete class.
- **Parallel spawn system for Kelp Curtain:** forbidden by D-33; reuse `NPCSpawnManager`.
- **Relying on `EditSpawnPool` alone for isolation:** it does not run outside Yggdrasil — `SpawnChance` must return 0.
- **Referencing an unimplemented drop type:** `ModContent.ItemType<T>()` for a non-existent `T` is a compile error — the exact failure Phase 2 hit (T-02-01).
- **Creating a `.png` for a missing creature:** AGENTS.md forbids new placeholder art; use `White_Mod` + a named artwork blocker.
- **Using `Main.LocalPlayer` in `SpawnChance`:** the tML docs say *"Remember to always use spawnInfo.player and not Main.LocalPlayer when checking Player or ModPlayer fields, otherwise your mod won't work in Multiplayer."* Use `spawnInfo.player`.
- **Un-guarded dust/VFX on a dedicated server:** every `Dust.NewDust`/`Main.dust`/`VFXManager` call in AI/HitEffect needs a `!Main.dedServ` guard.
- **Hand-editing localization HJSON:** out of scope (D-20) and forbidden by AGENTS.md.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Subworld-only spawning | A new Kelp-Curtain spawn `ModSystem` | `NPCSpawnManager.RegisterNPC` + `ModNPC.SpawnChance` (17 existing users) | Already implements the subworld filter and per-NPC weights; a parallel system duplicates state and risks leaking into the main world. |
| Critter capture | Custom "net + right-click" pickup | `Main.npcCatchable[Type] = true; NPCID.Sets.CountsAsCritter[Type] = true; NPC.catchItem = ModContent.ItemType<...>()` | Engine-owned capture path; `RiverSlug.cs:64-66,84` is the verified precedent. |
| Loot generation | Custom `OnKill` + `Item.NewItem` | `ModifyNPCLoot` + `ItemDropRule.Common/…` | `ItemDropRule` handles chance, quantity, expert/master scaling and drop-luck. |
| Water-surface / water-bottom detection | Scan a large tile region every frame | `TileUtils.SafeGetTile` + `tile.LiquidAmount` (biome precedent), or `Collision.WetCollision`/`NPC.wet` | `DeathJadeLakeBiome.GetLiquidSurfaceY()` and `KelpCurtainBiome` already use `TileUtils.SafeGetTile` with `LiquidAmount` clamping; reuse it. |
| Status effects | Custom damage-over-time tracking | `player.AddBuff(BuffID.…, ticks)` / `target.AddBuff(...)` | `BuffID.Poisoned/AcidVenom/Confused/Slow/Darkness` are the design's own debuffs; used throughout `KelpCurtain/Projectiles/*`. |
| Subworld/context detection | Map size, coordinates, scene effects | `SubworldSystem.IsActive<YggdrasilWorld>()` / `YggdrasilWorld.InYggdrasil` | Yggdrasil module AGENTS.md explicitly forbids substituting map size/coordinates. |
| Asset paths | Handwritten `"Everglow/Yggdrasil/KelpCurtain/NPCs/…"` strings | Source-generated `ModAsset.*` (or `Commons.ModAsset.*`) | AGENTS.md: use generated members; do not duplicate hardcoded paths. |
| Localization keys | Hand-written HJSON entries | In-game `OutputLocalizationHjsonItem` exporter | Out of scope this phase; never hand-edit HJSON. |

**Key insight:** in this domain the expensive failures are *wiring* failures (a spawn that leaks, a loot rule that names a missing type, a matrix that disagrees with itself), not algorithm failures. Prefer the engine's own extension points and let the design source, not intuition, decide values.

## Common Pitfalls

### Pitfall 1: D-26's "texture checkbox" does not exist for creatures (HIGH severity)
**What goes wrong:** the matrix freezes `texture_complete` on a rule the snapshot cannot answer, so Phase 3/4 allocate the wrong creatures and Phase 8 re-reconciles twice.
**Why it happens:** the biology design encodes creature completion only as an inline `<img>`; only the two Giant Winged Dragon *item* tables carry `贴图`/`代码` checkboxes (22 `<checkbox>` elements total, 0 on creature rows).
**How to avoid:** confirm the tranche rule with the user **before** writing `03-BIOLOGY.json` (Open Question 1); record the chosen rule verbatim in the matrix header and in `03-DEVIATIONS.md`; in the gate, assert that the rule marker field (e.g. `design_image_block_id`) is non-empty exactly for the rows marked `texture_complete=true`.
**Warning signs:** a matrix with `texture_complete=true` on rows whose source section has no `<img>`; or a "texture checkbox id" field that is empty on every creature row.

### Pitfall 2: Referencing an unimplemented drop item breaks the build (HIGH severity)
**What goes wrong:** `ModContent.ItemType<MolluscShellFragment>()` for a class that does not exist fails compilation, which fails the whole mod build.
**Why it happens:** the design names 软体甲壳碎片/亡碧膏/毒腺/牛黄/飞棍毛发/干涸心脏 in creature drop lines, but none of them exist as items (Finding 2).
**How to avoid:** in every `ModifyNPCLoot`, add a rule **only** for a `ModContent.ItemType<T>()` whose `T` is a verified Phase 1–2 class; comment the omitted rules with the blocker id. `BoulderCatapult` (Phase 2) is the precedent for omitting a rule when its design dependency is absent.
**Warning signs:** a plan task that adds `ItemDropRule.Common(ModContent.ItemType<…>)` for a material not found in `01-INVENTORY.json`.

### Pitfall 3: Main-world leakage (HIGH severity, BIO-06)
**What goes wrong:** a Kelp Curtain creature spawns in the normal world.
**Why it happens:** `NPCSpawnManager.EditSpawnPool` returns early when `!SubworldSystem.IsActive<YggdrasilWorld>()`, so the pool is *not* filtered outside the subworld; the pool still contains any mod NPC whose `SpawnChance` is > 0.
**How to avoid:** every `SpawnChance` must begin with a subworld-dependent predicate (`KelpCurtainBiome.IsBiomeActive(spawnInfo.player)` or an explicit `SubworldSystem.IsActive<YggdrasilWorld>()` check) and return `0f` otherwise.
**Warning signs:** a `SpawnChance` that only checks `spawnInfo.SpawnTileType`/water/time; a creature sighted in a normal world in the D-21 client run.

### Pitfall 4: Placeholder art regression (HIGH severity)
**What goes wrong:** a new `.png` is created (or an existing asset modified) to make a texture-less creature look complete.
**Why it happens:** six of the seven tranche creatures have design art but no repository texture.
**How to avoid:** use `Commons.ModAsset.White_Mod` + a named artwork blocker; the gate's no-placeholder guard checks that no `.png` was added/modified under the creature tree. Report the missing art to the user; never generate it.
**Warning signs:** `git status` showing a new `.png` under `KelpCurtain/NPCs/`.

### Pitfall 5: Gate script is not 100% ASCII (MEDIUM severity)
**What goes wrong:** `check-biology.ps1` mis-parses its own literals on Windows PowerShell 5.1, which reads BOM-less scripts as the system ANSI code page.
**Why it happens:** the repo forbids UTF-8 BOMs, so literal CJK bytes cannot be embedded in the script.
**How to avoid:** build CJK id/name fragments from `[char]0x….` codepoints, exactly as `check-phase2.ps1:146-151,172` does; read JSON with `[IO.File]::ReadAllText`; resolve class files from the working tree with `Get-ChildItem`, never `git ls-files` (the file may be untracked in the same task).
**Warning signs:** a gate script whose bytes contain anything above 0x7F.

### Pitfall 6: `Main.LocalPlayer` in `SpawnChance` (MEDIUM severity, multiplayer)
**What goes wrong:** the creature spawns only for the host, or not at all on a dedicated server.
**Why it happens:** the tML contract is explicit that `SpawnChance` runs SP/server-only and must use `spawnInfo.player`.
**How to avoid:** use `spawnInfo.player` for every player/biome/ModPlayer query. (`CyanOreBeetle`/`BarkSpicyCaterpillar` use `Main.LocalPlayer`; mirror the *structure* but not that particular call.)
**Warning signs:** `spawnInfo` ignored; `Main.LocalPlayer` in the predicate.

### Pitfall 7: Water semantics drift (MEDIUM severity)
**What goes wrong:** 水黾 (surface-only), 装甲虾 (group water), 帆鳍鳢 (open water) and 大型覆藻章鱼 (water bottom only) spawn in the wrong layer, or a designed amphibious suffocation/moisture timer never fires.
**Why it happens:** "在水面上刷新 / 水底刷新 / 沉底会窒息" are three distinct conditions, and the design's 潮湿 value is a per-NPC timer, not a `Player` state.
**How to avoid:** define one small helper per condition (`IsAtWaterSurface`, `IsDeepWater`, `IsWaterBottom`) using `Main.tile[...].LiquidAmount` (and `DeathJadeLakeBiome.LiquidSurfaceY` for the lake surface), and keep amphibious timers in `NPC.localAI[]` with `netUpdate` on transitions.
**Warning signs:** a creature that spawns in air or on the lake bed when the design says surface; a moisture timer stored on the player.

### Pitfall 8: Internal-name / compatibility drift (MEDIUM severity)
**What goes wrong:** a creature's internal type name is later renamed, breaking saves, Bestiary ids and localization keys.
**Why it happens:** D-28 marks creature internal names as compatibility-sensitive once committed, and this phase invents the English names.
**How to avoid:** choose the class names once, list them in `03-BIOLOGY.json` (`internal_name`) and in the plan, and never rename afterwards. Record the naming mapping as an assumption.
**Warning signs:** two plans inventing different names for the same design row.

### Pitfall 9: JSON/Markdown mirror divergence (MEDIUM severity)
**What goes wrong:** `03-BIOLOGY.md` disagrees with `03-BIOLOGY.json`, which Phase 4 and Phase 8 consume.
**Why it happens:** both are hand-edited instead of one being generated from the other.
**How to avoid:** generate the Markdown from the JSON (as `parse-design-xml.ps1` does) or have the gate assert row-by-row agreement; keep blockers byte-identical between the two.
**Warning signs:** a row present in the JSON but missing from the MD.

## Code Examples

### The existing subworld spawn filter (read this before touching spawns)
```csharp
// Source: Sources/Modules/Yggdrasil/Common/NPCSpawnManager.cs:11-33
public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
{
	if (!SubworldSystem.IsActive<YggdrasilWorld>())
	{
		return;
	}

	var dict = new Dictionary<int, float>(yggdrasilNPC.Count);
	foreach (var pair in pool)
	{
		if (yggdrasilNPC.Contains(pair.Key))
		{
			dict.Add(pair.Key, pair.Value);
		}
	}
	pool.Clear();
	foreach (var pair in dict)
	{
		pool.Add(pair.Key, pair.Value);
	}
	pool.Add(NPCID.BlueSlime, 0.1f);
	// …vanilla slimes…
}
```

### The critter/capture precedent (already implements 水蛞蝓)
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs:62-86
public override void SetStaticDefaults()
{
	Main.npcFrameCount[NPC.type] = 4;
	Main.npcCatchable[NPC.type] = true;
	NPCID.Sets.CountsAsCritter[NPC.type] = true;
}

public override void SetDefaults()
{
	NPC.CloneDefaults(NPCID.GlowingSnail);
	NPC.width = 20; // 36;
	NPC.height = 20; // 26;
	NPC.life = 5;
	NPC.damage = 10;
	NPC.defense = 0;
	NPC.knockBackResist = 0f;
	NPC.npcSlots = 0.5f;
	NPC.aiStyle = -1;
	NPC.catchItem = ModContent.ItemType<RiverSlugItem>();
	SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
}
```
The design's "提供渔力" is implemented on the capture item, not the NPC:
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/Items/Critters/RiverSlugItem.cs:23-25
Item.consumable = true;
Item.bait = 30;
Item.makeNPC = ModContent.NPCType<RiverSlug>();
```

### Debuff-on-hit shape (existing KelpCurtain precedent)
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Melee/AcroporaSpear_proj.cs:401
target.AddBuff(BuffID.Poisoned, 600);
```

### Debuff immunity declaration (per-type, used for the design's 免疫 column)
```csharp
// Source: Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/CyanOreBeetle.cs:15
NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
```

### Dedicated-server guard for client-only work (D-35)
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs:367-382
public override void HitEffect(NPC.HitInfo hit)
{
	if (NPC.life <= 0)
	{
		for (int i = 0; i < 6; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LichenSlime>(), 2 * hit.HitDirection, -2f, newColor: Color.Blue);
			// …
		}
	}
}
```
Note: this specific precedent is **not** guarded. For Phase 3, wrap every dust/VFX block in `if (!Main.dedServ)` (the Phase 2 convention, e.g. `BoulderCatapult_SubProj.cs:29,40`).

### API contract for the spawn hook (authoritative)
```text
// Source: tModLoader.xml, M:Terraria.ModLoader.ModNPC.SpawnChance
"Whether or not this NPC can spawn with the given spawning conditions. Return the weight for the
 chance of this NPC to spawn compared to vanilla mobs. All vanilla mobs combined have a total
 weight of 1. Returns 0 by default, which disables natural spawning. Remember to always use
 spawnInfo.player and not Main.LocalPlayer … Called in single player or on the server only."

// Source: tModLoader.xml, M:Terraria.ModLoader.GlobalNPC.EditSpawnPool
"Allows you to control which NPCs can spawn and how likely each one is to spawn. The pool parameter
 maps NPC types to their spawning weights … A type of 0 in the pool represents the default vanilla
 NPC spawning. … Called in single player or on the server only."
```

## Design Data Extracted (completed-art tranche = 7 rows)

Source: `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` (read-only, D-25). Region h1 = 亡碧湖 / Death Jade Lake (`JITOdS5VOofnlCxo5tecFn7n4g`). "Design art" = inline `<img>` present. All values below are transcribed from the snapshot in this session.

| # | Creature (zh) | Proposed class (ASSUMED) | h2 block id | Design art (img id / attachment / px) | Stats table id | Life / Damage / Defense / KB-resist / Damage-reduction / Immunity / Value / Rarity |
|---|---------------|--------------------------|-------------|----------------------------------------|----------------|--------------------------------------------------------------------------------------------|
| 1 | 水黾 | `WaterStrider` | `Gca8ddqq9on2lhxraxRcBNkmn6d` | `Hx6vdtlOIoEw7lx1SSYcFwPjn3f` / `XfwQbL4Ato6a0OxtIzacNnDFnqc` / 42×14 | `SqdydxBlPoASZtx5drwc9mZcnfe` | 45 / 20 / 10 / 40 / — / 中毒、潮湿 / 80 / — |
| 2 | 水蛞蝓 **(already implemented: `RiverSlug`)** | `RiverSlug` | `doxcnnaHMkf1Q1grLuoVOb1vOrh` | `PhXZds7oNobxdFx0ZN3cWeSHngg` / `MGIVbHkr2oFIhMx2EvfcnovCnxd` / 36×27 | `Ut3ydby3Co4aCNxZXdBcitvGnPc` | 5 / 10 / 0 / 0 / 0 / — / — / — |
| 3 | 幽光蝾螈（美西螈） | `Axolotl` | `AnOud27JsouukSxerW4cQ3zWnwb` | `RTqrdfAfLord5Nxt1Zvck3ZjnVg` / `YE7JbVK1koJGSXxc2NacTmrMnxg` / 234×38 (3 variants) | `HQIodexmAoHMu2x6as0ciTTjnjh` | 120 / 30 / 4 / 20 / — / 中毒 / 2银 / — |
| 4 | 装甲虾 | `ArmoredShrimp` | `Z94rdc53uoe4Wyx7TRPck6g5nQb` | `BIm6dfbxAo2HEmx7IFgcjfPknSw` / `T6Rfbfh0GobBYpxTdGEcYeztn5x` / 52×27 | `T9xpdYaL7oDHsTxNkkUcNRUYnWh` | 10 / 4 / — / — / — / — / — / — |
| 5 | 帆鳍鳢 | `SailfinSnakehead` | `BJVjdHX2FoW4MLxOk6ocVw82nUd` | `J2rxdqCDxo4EX5xY45PccPUNngb` / `WlRsbxhQyoz5J0xdEpUcsrlsn9e` / 71×40 | `BiiNdL85hoJC6axx5lbcy5Zsnnf` | 80/140/200 / 30 / 12 / 20 / — / — / 5银 / — |
| 6 | 覆藻章鱼 | `AlgaeOctopus` | `K8PmdSvk3okdbbxo7vYc4flKnKd` | `TqdRdg0p2oy914xfPZEcDM53nFh` / `W9JkbO8sZo9VeNxTukwcpZISnwJ` / 64×49 | `Ef2vderqeo3CgJxyazwcDKbAngd` | 70 / 15（近战）25（墨水） / 12 / 10 / — / 困惑 / 5银 / — |
| 7 | 大型覆藻章鱼 | `LargeAlgaeOctopus` | `AKGxdQG2foyNqBxGXjOcomLVnBb` | `GQcadbcPoofAMTx4gtwcCeyIngb` / `LOU3bhl2gocGx7xKouHcJhlJnmh` / 124×72 | `Bp4GdMk1ropekoxCXP2cNYwqnLb` | 360 / 50（近战）60（墨水） / 12 / 20 / 15 / 中毒、困惑 / 45银 / 稀有 |

Behavior + drop block ids (for `03-BIOLOGY.json` provenance):

| # | Creature | Behavior `<p>` id | Drop `<p>` id | Design drops (verbatim intent) |
|---|----------|-------------------|---------------|-------------------------------|
| 1 | 水黾 | `Frl7dwvbGoXf8sxSJozc3QfCnF2` | — | none |
| 2 | 水蛞蝓 | `Z3oNdznu9oCJYmxJeM9c4wa2nBg` | — | none (capturable; "提供渔力") |
| 3 | 幽光蝾螈 | `Dfkidik2VoSGuwxq8YKcjUVpnRg` | `XExidTYSHop1qHxY3r6c1Co3nEd` | 毒腺 33%×1; 牛黄 1%/2%×1 — **both unimplemented** |
| 4 | 装甲虾 | `ELQgdcR30oJFwAxNBgjcmXmQnXd` | `ELQgdcR30oJFwAxNBgjcmXmQnXd` | 软体甲壳碎片 ×1 — **unimplemented**; capturable |
| 5 | 帆鳍鳢 | `UUgpdyHQjopoSHx7wCzczqxwnPc` | `QLkTdqtHoowbKexZttGc71byn5e` | 亡碧膏 33%×1 — **unimplemented** |
| 6 | 覆藻章鱼 | `WwyUddu8SooiTDxy6tPcrgGEn3b` | `GON0deHa0ogLjcxYOI9cE0SxnGh` | 软体甲壳碎片 ×1 + 亡碧膏 ×1 — **both unimplemented** |
| 7 | 大型覆藻章鱼 | `PAord6A0NomliPxRzQlcw0wEnMc` | `Hco0dB4t1oqGjbx1nXRczxm4nsg` | 软体甲壳碎片 ×2–4 + 亡碧膏 ×3–6; **"武器与饰品掉落待定"** — all unimplemented |

Behavior summaries (transcribed from the snapshot, for the matrix/plan; do not re-invent):
1. **水黾** — land creature, the only one that spawns *only on a water surface*, slime-sized; moves by short aimless surface dashes every 60–200 frames; dashes at the player every 45–150 frames when nearby; hops back to the nearest water if it ends up on land (random direction otherwise); swims back up if it ends up underwater.
2. **水蛞蝓** — amphibious passive; spawns on the surface and crawls like a snail; sinks and crawls along the bottom when in water; never attacks (contact damage only); flees other aquatic creatures; damages other passive creatures it touches; capturable; provides fishing power.
3. **幽光蝾螈** — amphibious, **three colour variants (灰蓝色 / 粉色 / 褐色)**, spawns underwater, dashes and melees, attacks all underwater creatures and slugs; deliberately avoids 剧毒蟾蜍; lower player aggro than those; hunts slugs onto the surface for a while; has a **moisture value lasting at most 60 s after leaving water**, at 10 s remaining it tries to crawl back to the nearest water, at 0 it gains 1 s of suffocation every 30 frames until it returns; contact damage only, but inflicts 中毒 10 s.
4. **装甲虾** — aquatic passive; spawns in **groups of 2–5** in water and moves as a group; capturable; too unintelligent to flee predators; drops 1 软体甲壳碎片 ("also obtainable 1:1 by crafting").
5. **帆鳍鳢** — aquatic neutral; swims at constant speed while idle; when aggroed it rams with fast swimming; avoids 幽光蝾螈; actively attacks 水蛞蝓; life scales 80/140/200.
6. **覆藻章鱼** — aquatic hostile; **also spawns in 森雨幽谷 (Valley of Lush and Moist)**; cannot be observed from a distance (stealth); on approach attacks the player and other aquatic creatures **except 碧灵鮟鱇**; applies 缓慢 (Slow) to players; dashes quickly leaving ink clouds and bursts an ink cloud on death; ink clouds inflict 黑暗 (Darkness) for 2 s.
7. **大型覆藻章鱼** — aquatic, **spawns only at the water bottom** and has the lowest weight of all aquatic creatures; stealth like the smaller octopus; grabs prey applying 束缚 + suffocation (every 60 frames for 2 s); every 180 frames releases a wave of 4 ink clouds (main direction toward the prey, other 3 random); very fast dash leaving lingering ink clouds; at low HP it flees the player while spraying lingering ink; ink clouds inflict 黑暗 2 s; weapon/accessory drops TBD.

**Non-tranche creature rows the matrix must still record (Phase 4 / deferred — `texture_complete=false`):**
亡碧湖: 荧光水螅 (empty section), 巨型虎虾 (empty), 叶飞棍, 剧毒蟾蜍, 爆弹水母, 放射虫, 碧灵鮟鱇, 吸血魔毯 (empty; actually the already-implemented `VampireMat` boss), 炮弹藤壶 (empty).
刺苔庭园: 枯木活化士兵, 王庭号令者, 巨树人, 布罗迪蝇蜓.
森雨幽谷: 荆棘苔龟, 红针洋辣子, 阿萨辛覆盘子, 蛇行苔, 格普螺, 小格普螺, 大型荆棘苔龟.
Out-of-phase (do not include as ordinary creatures): h1 苍带帘蛇/克莱因蛇 (Phase 7), h1 暂时不用/困难模式 枯萎之种 + 枯木人卫士 (hardmode-deferred), h1 特殊/巨翼龙 (Phase 7).

### Cross-check: which designed drops actually exist today

| Design drop | In biology.xml | In item.xml | In repo (code/localization) | Implemented? |
|-------------|----------------|-------------|------------------------------|--------------|
| 软体甲壳碎片 | 6 references | 0 | 0 | **No** |
| 亡碧膏 | 4 | 0 | 0 | **No** (design itself says the name is provisional: "掉落物名字随便取的，需要改") |
| 毒腺 | 3 | 0 | 0 | **No** |
| 牛黄 | 2 | 0 | 0 | **No** |
| 飞棍毛发 | 1 | 0 | 0 | **No** |
| 干涸心脏 | 2 | 0 | 0 | **No** |
| 枯木碎块 | 4 | 5 (trade currency text only: "使用10枯木碎块交易") | 0 | **No** |
| 放射状甲壳 | 1 | 1 | `Items/Accessories/RadialCarapace.cs` | Yes (art = `White_Mod`) — Phase 4 creature |
| 格普螺外壳 | 1 | 1 | `Items/Accessories/GuppyShell.cs` | Yes — Phase 4 creature |
| 荆棘龟壳 | 1 | 1 | `Items/Accessories/ThornTurtleShell.cs` | Yes — Phase 4 creature |
| 肉食性提灯 | 1 | 1 | `Items/Weapons/MeatLantern.cs` | Yes — Phase 4 creature |
| 灵灯 | 1 | 1 | `Items/Pets/Photophore.cs` | Yes (yellow) — Phase 4 creature |
| 活化之犬召唤杖 | 1 | 1 | `Items/Weapons/ActivatedDogStaff.cs` | Yes — Phase 4 creature |
| 巨树之臂 | 1 | 1 | `Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs` | Yes — Phase 4 creature |
| 硬化枯木心脏 | 1 | 1 | `Items/Accessories/HardenedWitherbarkHeart.cs` | Yes — Phase 4 creature |
| 巨石弹射装置 | 1 | 1 | `Items/Weapons/BoulderCatapult.cs` | Yes (art incomplete) — Phase 4 creature |

## Design → Matrix Schema (recommended, D-24)

One row per creature, mirroring the `01-INVENTORY.json` field shape:

```jsonc
{
  "schema_version": 1,
  "generated_at": "…",
  "source": { "biology": { "token": "Jp5ndsvNBoCpljxq1eGc9S7vnfe", "revision_id": 7333,
                            "file": "evidence/biology.xml", "fetched_at": "2026-09-12T05:24:36Z" } },
  "tranche_rule": "…verbatim description of the confirmed rule (Open Question 1)…",
  "rows": [
    {
      "id": "biology_npc-death-jade-lake-<slug>",   // stable; content ids are compatibility-sensitive
      "region": "Death Jade Lake",                   // h1 label (Phase 1 label taxonomy)
      "name_zh": "水黾",
      "name_en": "Water Strider",                    // [ASSUMED] naming mapping, recorded for audit
      "internal_name": "Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.WaterStrider",
      "feishu": { "heading_block_id": "Gca8ddqq9on2lhxraxRcBNkmn6d",
                  "stats_table_block_id": "SqdydxBlPoASZtx5drwc9mZcnfe",
                  "behavior_block_id": "Frl7dwvbGoXf8sxSJozc3QfCnF2",
                  "drop_block_id": "",
                  "design_image_block_id": "Hx6vdtlOIoEw7lx1SSYcFwPjn3f",
                  "design_image_attachment": "XfwQbL4Ato6a0OxtIzacNnDFnqc",
                  "row_color": "neutral" },
      "texture_complete": true,
      "code_complete": false,
      "status": "unchecked",
      "repo_asset": "",                              // "" while White_Mod fallback is used
      "blockers": ["artwork: design art present but no repository texture; White_Mod fallback"],
      "deferred": false,
      "deferred_reason": ""
    }
  ],
  "assumptions": [],
  "deviations": []
}
```

Gate invariants `check-biology.ps1` should assert (D-27):
1. `rows.Count` equals the frozen expected total for Phase 3 **plus** the Phase-4 rows the matrix declares (or, if the matrix is Phase 3-only, Phase 3 count is exact).
2. `Compare-Object` between the matrix's `texture_complete=true` id set and the frozen Phase 3 selection — zero diffs.
3. Every row has a non-empty `status`, and every row with `texture_complete=true` has at least one non-empty blocker (either art or behavior).
4. Every `internal_name` resolves to a `.cs` file **on disk** under `KelpCurtain/NPCs` via `Get-ChildItem` (not `git ls-files`).
5. No added/modified `*.png` under the creature tree (no-placeholder guard), using `git status --porcelain -- <path>`.
6. The Markdown mirror's row count and per-row status match the JSON.
7. Exit codes: `0` OK, `1` invariant failure, `2` missing input.

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| Hand-transcribing design rows into a doc | Header-anchored XML parser producing a JSON matrix + Markdown mirror | Phase 1 (`parse-design-xml.ps1`) | Status is auditable and gateable; Phase 3 must follow the same shape. |
| `NPCLoot()` override | `ModifyNPCLoot(NPCLoot)` with `ItemDropRule` | tML 1.4 (already in repo) | Chance/quantity/expert-master handled by the engine. |
| Manual `AddContent` registration | Automatic content discovery of `Everglow.*` assemblies | existing repo architecture | Just add a `ModNPC` subclass; no registration call beyond `NPCSpawnManager.RegisterNPC`. |
| Per-mod spawn systems | Shared `NPCSpawnManager.EditSpawnPool` + per-NPC `SpawnChance` | existing repo | Subworld isolation is centralized; new creatures opt in with one line. |

**Deprecated/outdated in this repo:**
- `Modules/IIID`, `Modules/TwilightForest`, `Modules/ZY`, `Sources/Everglow.Scripts` — dead; never add content there.
- `Sources/Everglow/Localization/en-US_Mods.Everglow.hjson` — legacy root file; never add keys.

## Assumptions Log

> Every claim below is `[ASSUMED]`. None may be turned into a locked implementation decision without user confirmation.

| # | Claim | Section | Risk if Wrong |
|---|-------|---------|---------------|
| A1 | "Texture-complete" for a creature means its h2 section carries an inline design `<img>` (the design's only per-creature artwork marker). | Summary / Design Data Extracted / Open Q1 | The Phase 3/4 tranche is wrong; Phase 4 and Phase 8 re-reconcile. |
| A2 | The 7 image-bearing creatures are exactly the Phase 3 tranche, so Phase 3 contains no 刺苔庭园 or 森雨幽谷 creature. | Design Data Extracted | The phase delivers no BIO-02/BIO-03 content at all — a scope expectation mismatch for the milestone. |
| A3 | 水蛞蝓 == the existing `RiverSlug` (`RiverSlugItem.bait = 30` is the design's "提供渔力"). | Summary / Design Data Extracted | An unnecessary duplicate creature class, or a missing acceptance row. |
| A4 | English class names (`WaterStrider`, `Axolotl`, `ArmoredShrimp`, `SailfinSnakehead`, `AlgaeOctopus`, `LargeAlgaeOctopus`) are acceptable; internal names are compatibility-sensitive once committed. | Design Data Extracted / Pitfall 8 | A rename later breaks saves/Bestiary/localization. |
| A5 | Default creature weights where the design is silent: use a conservative 1f–3f range and the existing `NPCSpawnManager` slime weights (0.1f) as the scale reference. | Pattern 1 / D-34 | Spawn density too high/low; tuning churn later. |
| A6 | The six texture-less creatures load with `Commons.ModAsset.White_Mod` and an artwork blocker instead of new art. | Summary / Pitfall 4 | A blocked or ugly phase, or an unauthorized placeholder asset. |
| A7 | Vanilla `aiStyle` numbers are not asserted here; `aiStyle = 3` (walker) is verified in-repo, and swimmers/surface-dashers should use `aiStyle = -1` + custom `AI()` unless an analogous vanilla style is confirmed in-client. | Standard Stack / D-31 | Wrong/vanilla-incompatible motion; requires rework. |
| A8 | The matrix ids use ASCII region/name slugs plus a Chinese `name_zh`, keeping `check-biology.ps1` free of CJK literals. | Design → Matrix Schema | Gate becomes non-ASCII and mis-parses on PS 5.1. |
| A9 | `03-BIOLOGY.json` will also carry the Phase-4 (non-artwork) rows so Phase 4 consumes one artifact; the gate then checks the Phase 3 subset exactly. | Design → Matrix Schema | If the matrix is Phase 3-only, Phase 4 must extend the schema (D-24 warns this is costly). |

## Open Questions (RESOLVED — 2026-09-15)

> **All five resolved before planning.** Q1 → D-41 (tranche rule = repository art, NOT the inline design `<img>` recommended below). Q2 → D-43 (empty/partial loot tables + precise blockers; no item scope added). Q3 → D-41/D-42 (repo-art creatures are Phase 3). Q4 → D-29 (empty sections → undefined-behavior shells/blockers; 吸血魔毯 = the implemented `VampireMat`). Q5 → D-30/D-39 (capture items are item scope → blockers, no new `ModItem`s). The recommendations below are retained for the audit trail only; where they conflict with the resolving decision, the decision wins.

1. **Which rule defines "texture-complete" now that no creature checkbox exists? (BLOCKING — checkpoint before freezing the matrix.)**
   - What we know: the snapshot has 0 creature texture/code checkboxes; 7 creatures have inline design art; D-26 forbids using repository `.png` presence.
   - What's unclear: whether the user's mental model of a "texture checkbox" was the item-doc convention (which does not extend to creatures) or the inline design image.
   - Recommendation: present the finding, recommend **"inline design image = texture-complete"**, and record the confirmed rule verbatim in `03-BIOLOGY.json` (`tranche_rule`) and `03-DEVIATIONS.md`. Alternatives with consequences: (a) repository-`.png`-present → tranche collapses to ~1 mappable creature (荆棘苔龟/格普螺/叶飞棍 have repo art but no design art) and contradicts D-26's written text; (b) "behavior described" → tranche grows to ~23 creatures and is no longer an *art* split.

2. **What happens to success criterion 2 for this tranche? (BLOCKING for phase acceptance.)**
   - What we know: none of the tranche creatures' designed drops exist as items; every implemented biology drop belongs to a Phase-4 creature.
   - What's unclear: whether the user accepts "no demonstrable drop in Phase 3, blockers only" or wants the missing materials promoted into item scope (which would re-open item phases and contradict "no new item scope" in D-37).
   - Recommendation: keep D-37/D-39, make every loot table empty-by-design with a precise blocker, and explicitly state in `03-VERIFICATION.md` that SC2 is vacuously satisfied for this tranche. Surface this at the checkpoint.

3. **Are the three no-design-art-but-repo-art creatures (荆棘苔龟, 格普螺, 叶飞棍) misplaced in Phase 4?**
   - What we know: `NPCs/MossyThornTurtle.png`, `NPCs/GuppyConch.png`, `NPCs/VerdantRods.png` exist and are referenced by no `.cs` file; their design rows have no inline art.
   - What's unclear: whether that pre-existing art is current design art (→ they belong in Phase 3) or legacy/unused.
   - Recommendation: record them in the matrix with `repo_asset` set and `texture_complete` per the confirmed rule; note the discrepancy in `03-DEVIATIONS.md` for the user.

4. **Where do the four empty creature sections (荧光水螅, 巨型虎虾, 吸血魔毯, 炮弹藤壶) land?**
   - What we know: they have an `h2` and no stats, behavior, art or drops.
   - What's unclear: whether 吸血魔毯 is simply the existing `VampireMat` boss (Phase 7) under its design name.
   - Recommendation: 吸血魔毯 → record as the implemented boss (`VampireMat`) and exclude; the other three → `texture_complete=false`, Phase 4 rows with "undefined behavior (D-29 shell / blocker)" notes.

5. **Do the six new NPCs get critter/capture wiring now?**
   - What we know: 水蛞蝓 (done), 装甲虾 and 爆弹水母 are `可被捕捉`; only `RiverSlugItem` exists as a critter item.
   - What's unclear: whether a capture item may be introduced given "no new item scope".
   - Recommendation: treat capture items as **item scope → blocker** (D-30/D-39); do not create new `ModItem`s. Record the exact missing item names in `03-DEVIATIONS.md`.

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|------------|-----------|---------|----------|
| .NET SDK | `dotnet build` / `dotnet test` | ✓ | 9.0.306 (builds `net8.0`) | — |
| tModLoader `tModLoader.targets` | every build | ✓ | ancestor at `ModSources/tModLoader.targets` | — |
| tModLoader client (runtime verification, D-21) | SC1/SC3 client checks | ✓ (install present; not launched this session) | — | Deferred to the end-of-phase UAT batch (`human_verify_mode: "end-of-phase"`) |
| Windows PowerShell 5.1 | `check-biology.ps1` | ✓ | 5.1.26100.9168 | — |
| Git | BOM/no-art guards, commits | ✓ | 2.37.3.windows.1 | — |
| XNA runtime components (shader compile) | `CompileEffect=true` builds | assumed ✓ (Phase 1/2 built in this repo) | — | — |
| Feishu / `lark-cli` | **not required** (D-25: committed snapshot only) | n/a | — | — |

**Missing dependencies with no fallback:** none identified.
**Missing dependencies with fallback:** none identified.

## Validation Architecture

> `workflow.nyquist_validation` is `true` in `.planning/config.json:24`, so this section is required.

### Test Framework

| Property | Value |
|----------|-------|
| Framework | **none for `ModNPC` content** — MSTest 3.10.2 (`Sources/Everglow.UnitTests`) cannot construct `Main`, load mod content, or run the game loop; the phase's verification is offline PowerShell gates + `dotnet build` + a client UAT batch. (Mirrors `02-VALIDATION.md:24`.) |
| Config file | none (`Sources/Everglow.UnitTests` has no test config file; tests are discovered by the SDK) |
| Quick run command | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| Full suite command | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1` → every earlier phase gate → `dotnet test --filter "FullyQualifiedName~Yggdrasil"` |
| Estimated runtime | ~60–120 s (build dominates) |

### Phase Requirements → Test Map

| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|--------------|
| BIO-01/02/03 (tranche) | Tranche membership is exactly the confirmed artwork set | structural (JSON assertion) | `powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1"` | ❌ Wave 0 |
| BIO-01 (tranche) | Every tranche creature has a loadable `ModNPC` class on disk | structural | same gate (invariant 4) | ❌ Wave 0 (classes) |
| BIO-01 (tranche) | Spawn is subworld-isolated | static source check (gate regex) + client | gate asserts each new class contains a `SubworldSystem.IsActive<YggdrasilWorld>()` **or** `KelpCurtainBiome.IsBiomeActive` reference inside `SpawnChance`; client confirms no main-world spawn | ❌ Wave 0 |
| BIO-01 (tranche) | Loot tables reference no unimplemented item type | static source check (gate) + build | gate asserts no `ItemDropRule.*ModContent.ItemType<` in the new NPCs; `dotnet build` proves type resolution | ❌ Wave 0 |
| BIO-06 | No main-world behavior introduced | static + dedicated-server smoke | gate regex above; client run in a normal world and a dedicated-server launch | ❌ Wave 0 |
| QUAL-01 | Build clean | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ |
| QUAL-03 | Multiplayer/dedicated-server safety | static + manual | gate asserts `spawnInfo.player` (not `Main.LocalPlayer`) and `!Main.dedServ` around dust/VFX; manual dedicated-server launch | ❌ Wave 0 |
| QUAL-04 | Per-creature comparison record | structural | gate invariants 1–3, 6 | ❌ Wave 0 |

### Sampling Rate

- **Per task commit:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` for any C# change; `scripts/check-biology.ps1` for matrix/gate changes.
- **Per wave merge:** `scripts/check-biology.ps1` + `dotnet build /p:Configuration=Release /p:WarningLevel=0` + the Phase 1/2 regression gates.
- **Phase gate:** full suite green, plus the D-21 client/`!Main.dedServ` UAT bundle, before `/gsd-verify-work`.
- **Max feedback latency:** ~120 s.

### Wave 0 Gaps

- [ ] `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — the matrix itself (D-24), produced from the frozen tranche rule.
- [ ] `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — the mirror.
- [ ] `.planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1` — the gate (D-27), authored before the creature classes so it can run red→green.
- [ ] `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — conservative spawn weights (D-34), conservative drop chances (D-38), all blockers (D-39).
- [ ] Six `ModNPC` class files under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/`.
- [ ] Framework install: **none** — existing infrastructure (MSTest + PowerShell + build) covers the phase.

## Security Domain

> `security_enforcement: true` (`security_asvs_level: 1`, `security_block_on: high` in `.planning/config.json:47-49`).

### Applicable ASVS Categories

| ASVS Category | Applies | Standard Control |
|---------------|---------|------------------|
| V2 Authentication | no | No auth surface; the phase adds gameplay NPCs. |
| V3 Session Management | no | No session state is introduced. |
| V4 Access Control | no | No privilege boundary; subworld isolation is gameplay scoping, not access control. |
| V5 Input Validation | **yes** | All design-derived values are parsed from a committed XML snapshot; the matrix/gate validate shape, counts and classification. No untrusted external input at runtime (no network, no file input). |
| V6 Cryptography | no | None. |
| V7 Error handling / logging | **yes (light)** | Gate scripts must exit non-zero on failure (0/1/2) and never report a failing gate as passing; no silent `catch`. |
| V11 Business logic | **yes** | Server-authoritative AI: no client-side authority over spawn/state; `netUpdate` on authoritative changes; no client-only code mutating gameplay state. |

### Known Threat Patterns for this stack

| Pattern | STRIDE | Standard Mitigation |
|---------|--------|---------------------|
| Main-world leakage of a subworld-only creature (BIO-06) | Elevation of Privilege / Tampering (gameplay boundary) | `SpawnChance` returns 0 outside `SubworldSystem.IsActive<YggdrasilWorld>()`; `NPCSpawnManager` filter is a second layer. |
| Client-authoritative NPC state (desync/duplication) | Tampering | Random/state transitions only under `Main.netMode != NetmodeID.MultiplayerClient`; `NPC.netUpdate = true`. |
| Dedicated-server crash from graphics access | Denial of Service | `!Main.dedServ` guards around every dust/VFX/texture call. |
| Null-reference from missing art on load | Denial of Service (mod fails to load) | `White_Mod` fallback; never reference a non-existent texture path; never create placeholder art. |
| Compile-time break from a loot rule naming an absent item | Denial of Service (build fails) | Add a rule only for a verified implemented type; omit + blocker otherwise. |
| Non-deterministic/unauditable status record | Repudiation | Matrix + gate: counts, classification, blockers and no-art guard asserted on every run. |

## Sources

### Primary (HIGH confidence)
- `Sources/Modules/Yggdrasil/Common/NPCSpawnManager.cs` (read: lines 1–89) — `RegisterNPC`, `EditSpawnPool`, `EditSpawnRate`.
- `Sources/Modules/Yggdrasil/YggdrasilWorld.cs` (read: lines 1–82) — `Subworld`, `InYggdrasil`, dimensions/ShouldSave.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs` (read: lines 1–407) — critter, capture, custom AI, spawn precedent.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VampireMat/VampireMat.cs` (read: lines 1–1124) — registration + `ModifyNPCLoot` + coroutine AI.
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs` (read: lines 1–124) — subworld-gated `IsBiomeActive`.
- `Sources/Modules/Yggdrasil/KelpCurtain/Biomes/DeathJadeLakeBiome.cs` (read: lines 1–162) — region gate + `LiquidSurfaceY`.
- `Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/CyanOreBeetle.cs`, `BarkSpicyCaterpillar.cs` — ordinary Yggdrasil spawn/loot precedents.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Critters/RiverSlugItem.cs` — critter item (`bait = 30`).
- `Sources/Everglow.Function/Utilities/LocalizationUtils.cs` (read: lines 1–71) — `Categories.NPCs`.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/parse-design-xml.ps1` (read: lines 91–111, 341–345) — declared creature-table skip + ASCII/regex conventions.
- `.planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1` (read: lines 1–209) — gate conventions to mirror.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` (read: 103 entries; `source_kind` = `biology_drop` 13 / `item` 89 / `terrain` 1) — item/drop source of truth.
- `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` (read in full via pretty-print/digest this session) — 29 tables, 2 name-header tables, 22 `<checkbox>` elements, per-creature stats/behavior/drop blocks.
- `.planning/config.json` (read) — `nyquist_validation: true`, `security_enforcement: true`, `human_verify_mode: end-of-phase`.
- `tModLoader.xml` (publicized, read) — `ModNPC.SpawnChance`, `ModNPC.SpawnModBiomes`, `GlobalNPC.EditSpawnPool`, `ILocalizedModType.LocalizationCategory` contracts.
- `Sources/Directory.Build.props` / `Sources/Modules/Directory.Build.props` / `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj` (read) — target framework, module list, references.

### Secondary (MEDIUM confidence)
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-VALIDATION.md` — validation-architecture format and the "no unit-test path for content" conclusion.
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-RESEARCH.md` (section headings) — structure and pitfall conventions to mirror.
- `.planning/ROADMAP.md` Phase 3/4 sections — success criteria and dependency text.
- `.planning/PROJECT.md` § "Design Status Synchronization" — colour/checkbox rules (relevant to the eventual Phase 8 writes).
- `AGENTS.md`, `Sources/Modules/Yggdrasil/AGENTS.md` — placement, resources, build/verification, subworld rules.

### Tertiary (LOW confidence)
- English creature names and `aiStyle` recommendations (`WaterStrider`, `Axolotl`, `ArmoredShrimp`, `SailfinSnakehead`, `AlgaeOctopus`, `LargeAlgaeOctopus`; walker `aiStyle = 3`, otherwise `-1`). Not verified against the design source or a client run — marked `[ASSUMED]`, A4/A7.

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — no new dependencies; every version read from committed project files.
- Architecture: HIGH — the subworld spawn system, NPC precedents, loot and capture patterns were read directly from source this session.
- Tranche rule (D-26 application): MEDIUM–LOW — the checkbox D-26 names does not exist; the inline-image proxy is inferred and needs confirmation.
- Drop availability: HIGH (negative finding verified three ways: `biology.xml`, `item.xml`, whole-tree search) — but the *consequence* for SC2 is a scope question, not a technical one.
- Pitfalls: MEDIUM–HIGH — engine contracts verified from `tModLoader.xml`; leakage and compile-break pitfalls are structural.

**Research date:** 2026-09-15
**Valid until:** 2026-10-15 (30 days; the design snapshot and tML API are stable, but the tranche rule must be confirmed before planning)
