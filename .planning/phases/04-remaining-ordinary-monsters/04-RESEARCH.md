# Phase 4: Remaining Ordinary Monsters - Research

**Researched:** 2026-09-15
**Domain:** Terraria tModLoader `ModNPC` content (21 art-missing ordinary creatures, Kelp Curtain, Yggdrasil Subworld) + in-place biology-matrix reconciliation
**Confidence:** HIGH on the creature design data, spawn/loot/api mechanics and gate extension; MEDIUM on the shell-vs-full split for the complex BIO-02 group; LOW on a handful of drop-reference and variant-strategy decisions (see Assumptions Log)

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

#### Scope & Shell Strategy
- **D-44:** Phase 4 scope is the **21** ordinary creatures = the 23 `phase:4` rows minus the two `out of phase` hardmode rows (`bio-out-of-phase-withered-seed` 枯萎之种, `bio-out-of-phase-withered-tree-guardian` 枯木人卫士), which stay deferred under V2-HARD-01. — **Reversibility:** costly — the in-scope set defines the phase's row count and every later reconciliation; changing it forces re-reconciliation of the matrix and Phase 8.
- **D-45:** In-scope creatures with **no defined behavior** (design row carries only a name/texture, e.g. 荧光水螅, 巨型虎虾, 炮弹藤壶) get an **identity-only shell class** (loadable `ModNPC`, documented stats, `LocalizationCategory`, `Commons.ModAsset.White_Mod` fallback) plus a precise blocker — never invent behavior (extends D-29/D-18).
- **D-46:** Creatures depending on **unimplemented systems** — Spiny Moss Court morale/command (枯木活化士兵, 王庭号令者) and Valley disguised-hazard mechanics (阿萨辛覆盘子, 蛇行苔) — implement only the independently-completable part and record a precise blocker naming the missing system and affected files (extends D-30). Do **not** implement those systems here.
- **D-47:** Complex creatures (BIO-02 group: 枯木活化士兵, 王庭号令者, 布罗迪蝇蜓) are judged **case by case, safety-first**: full implementation when the design row is complete enough, otherwise identity shell + blocker. Each decision is recorded in `04-DEVIATIONS.md`.

#### Missing-Art Handling
- **D-48:** Phase 4 creatures have **no repository art**. Implement the code first using the shared fallback `Commons.ModAsset.White_Mod` and record a precise "approved texture (贴图) missing from repository" blocker per entry. Never create placeholder art; never modify a binary/art asset (extends D-13).
- **D-49:** When approved art arrives, the migration is adding `<Class>.png` beside the `.cs` plus removing the artwork blocker — not a class rework (extends D-14).
- **D-50:** The phase gate (`scripts/check-biology.ps1`, extended for the Phase 4 rows) must assert no new class references a non-existent texture path (a missing texture can abort mod loading), and that every in-scope row carries a texture/artwork blocker.
- **D-51:** This phase creates or modifies **no** `.png`/`.obj`/`.xnb` or other binary art asset (AGENTS.md hard constraint); needed art is listed as a blocker only.

#### Spawn & Terrain Dependency
- **D-52:** Spawning uses the **layer-level** server-safe predicate `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` (consume the Phase 3 helper; do NOT use `IsBiomeActive` in `SpawnChance`) plus `SubworldSystem.IsActive<YggdrasilWorld>()`. **Region-level refinement** (森雨幽谷 / 刺苔庭园 / 亡碧湖 sub-biome or tile predicates) is recorded as a precise blocker for Phases 5–6 (extends D-30).
- **D-53:** Spawn/behavior must **not hard-depend on terrain tiles/blocks that do not yet exist** (Phase 5–6). Use existing biome/depth/liquid/time predicates; terrain-specific conditions become blockers.
- **D-54:** Spawn conditions (region/depth/water/time) follow the design row; where the design supplies no weight/condition, use conservative defaults recorded in `04-DEVIATIONS.md` (extends D-34).
- **D-55:** Subworld spawn/behavior is server-authoritative; spawn predicates must be server-safe (never `Main.screenPosition`, never `Main.LocalPlayer`); client-only graphics/VFX guarded by `!Main.dedServ` (extends D-32/D-35).

#### Drop Wiring
- **D-56:** Drops use direct drop tables (`NPCLoot()` / `ItemDropRule`) with explicit chance and quantity, consistent with repository NPC precedents (extends D-36).
- **D-57:** Drops reference **only items already implemented in Phases 1–2** (`ModContent.ItemType<...>`, e.g. `ThornTurtleShell`, `GuppyShell`, `ArmOfGiantTree`, `HardenedWitherbarkHeart`, `RadialCarapace`, `Photophore`, `ActivatedDogStaff`, `MeatLantern`). No new item scope (extends D-37).
- **D-58:** When a designed drop is not implemented (毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 飞棍毛发, 枯木碎块, …), leave that rule out (empty/partial loot table allowed) and record a precise blocker naming the affected item — the mod must still build and load (extends D-39/D-43).
- **D-59:** Drop probability/quantity follows the design row (including guaranteed/percentage rules); where the design is silent, use conservative defaults recorded in `04-DEVIATIONS.md` (extends D-38).

#### Carried Forward (confirmed)
- **D-60 [informational]:** **D-20** localization out of scope every phase; **D-21** in-client runtime verification required in addition to offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0`; **D-22** marking rules (`code_complete=true` / `artwork_complete=false` → row unchecked/no colour); **D-23** code-wins-unless-yellow; **D-41/D-42** the Phase 3/4 tranche rule (repository art). *(Restatement of already-cited policy — informational.)*

### the agent's Discretion
- NPC folder layout / class naming under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/`, and which existing NPC to mirror.
- Exact shell-class contents and blocker wording.
- Which vanilla `aiStyle` fits a creature and where a small custom `AI()` is warranted.
- Whether a creature's projectile/buff/VFX class is added now or recorded as an effect blocker.
- Exact conservative default spawn weights / drop chances where the design is silent.

### Deferred Ideas (OUT OF SCOPE)
- **Hardmode-deferred designs** — 枯萎之种, 枯木人卫士 (V2-HARD-01).
- **Localization (all phases)** — excluded by user directive; ITEM-07 deferred (D-20).
- **Region-level spawn predicates** (森雨幽谷 / 刺苔庭园) — Phases 5–6 terrain work (D-52).
- **Boss / special-encounter creatures** (Klein Snake, Giant Winged Dragon, 吸血魔毯/VampireMat) — Phase 7.
- **Unimplemented systems** (morale/command, disguised hazards, capture items) — precise blockers only (D-46/D-57).
- **Absent drop materials** (毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 飞棍毛发, 枯木碎块) — item scope; recorded blockers (D-58).
- **Runtime verification** — requires a live tModLoader client; batched with the deferred Phase 2/3 UAT (D-21).

</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| BIO-01 | Death Jade Lake implements the designed aquatic, amphibious, surface-water, passive, and predator creatures with their water/land behavior, spawning rules, hostility relationships, status effects, capture rules, and drops. | All 13 `phase:4` Death Jade Lake rows are transcribed verbatim from the committed snapshot in §Creature Map below — stats, behavior text, drops, and the per-creature spawn predicate (water / surface / land / water-bottom). 10 are full implementations, 3 are D-45 identity shells (荧光水螅, 巨型虎虾, 炮弹藤壶); one drop (`RadialCarapace`) is wired, one creature (`碧灵鮟鱇`) wires both of its drops. |
| BIO-02 | Spiny Moss Court implements the designed Witherbark soldiers, command unit, giant tree, Brody dragonfly, and associated combat states, morale behavior, variants, and drops. | 巨树人 was delivered in Phase 3; Phase 4 owns the remaining 3 rows — 枯木活化士兵 (4 stat variants), 王庭号令者, 布罗迪蝇蜓 — with the design's exact per-variant stat tables extracted below. Morale/command buff is an unimplemented system → D-46 blocker; the 犬 variant's `ActivatedDogStaff` drop is wireable. |
| BIO-03 | Valley of Lush and Moist implements the designed common creatures, disguised hazards, neutral creatures, mini-boss encounter, and related drops and behavior. | The 5 Valley rows — 红针洋辣子, 阿萨辛覆盘子, 蛇行苔, 小格普螺, 大型荆棘苔龟 — are transcribed below. Disguised-hazard visuals are a D-46 blocker; the station/trigger/attack mechanics and the 大型荆棘苔龟 mini-boss state machine are implementable. |
| BIO-06 (advanced, completes Phase 8) | No unintended main-world behavior. | Every new `SpawnChance` is gated on `SubworldSystem.IsActive<YggdrasilWorld>()` **and** `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`; the Phase 4 gate asserts both tokens on every class that declares `SpawnChance`. |
| QUAL-03 (advanced) | Multiplayer/dedicated-server safety. | `spawnInfo.Player` (never `Main.LocalPlayer`); `!Main.dedServ` around dust/VFX; server-authoritative state transitions under `Main.netMode != NetmodeID.MultiplayerClient` with `NPC.netUpdate`. |
| QUAL-04 (advanced) | Per-creature comparison record. | The reconciliation of the 21 `phase:4` rows in `03-BIOLOGY.json`/`.md` (status/blockers/internal_name) is this phase's comparison record; no Feishu writes (Phase 8). |

*Note: `03-RESEARCH.md`'s "design-art <img> = texture-complete" recommendation (Finding 1 / Open Q1) was superseded by D-41 (repository art). It must not be resurrected here.*

</phase_requirements>

## Summary

Phase 4 is a **`ModNPC` content phase for 21 creatures that have no repository art**, consuming a frozen, already-committed design snapshot and the Phase 1–2 item implementations. It is not a design-parsing phase and it adds no item, tile, wall, buff or localization artifact. Three findings dominate planning:

**Finding 1 — the design data for all 21 creatures is complete enough to implement, and it is already extracted in this document.** Direct XML parsing of the committed `evidence/biology.xml` this session recovered the full stats table, behavior paragraph and drop line for 18 of the 21 rows; only 3 rows (荧光水螅, 巨型虎虾, 炮弹藤壶) carry a heading and nothing else — these are exactly the D-45 identity-shell candidates the CONTEXT names. The 21 rows are definitively enumerable: `[phase]==4` in `03-BIOLOGY.json` yields 23 rows, of which the two `out of phase` hardmode rows are already `deferred: true`; 23 − 2 = **21** (D-44 confirmed against the machine source).

**Finding 2 — the reward surface is narrow but real.** Of the 21 design rows only **5 carry an implementable drop**: 碧灵鮟鱇 → `MeatLantern` (25%) + `Photophore` (12.5%); 放射虫 → `RadialCarapace` (6.7%); 枯木活化士兵 (犬 variant) → `ActivatedDogStaff` (9%); 红针洋辣子 → mirrors `BarkSpicyCaterpillar`'s existing `CaterpillarJuice` (100%, 1–2). Every other designed drop is an absent material (毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 枯木碎块, 干涸心脏) or explicitly "暂定/TBD" (大型荆棘苔龟) and becomes a D-58 blocker with **no type reference written**. The remaining 15 rows have no designed drop at all. The Phase 3 precedent (`VerdantRods` ships an empty, commented `ModifyNPCLoot`) is the exact shape to reuse.

**Finding 3 — the technical hazards are the same two that Phase 3 proved out, plus one new one.** (a) **Main-world leakage**: `NPCSpawnManager.EditSpawnPool` returns early outside Yggdrasil, so the per-creature `SpawnChance` is the only real isolation — every class must return `0f` without both the subworld token and the server-safe layer predicate. (b) **Compile-break from an absent drop type**: a `ModContent.ItemType<X>()` for a non-existent `X` fails the whole build, so absent drops get a blocker and no rule. (c) **New — the texture-free class shape**: because no Phase 4 creature has art, every class overrides `Texture => Commons.ModAsset.White_Mod`, and the D-49 art migration must place the arriving `.png` where tML's default (namespace-derived) texture resolution will find it. This makes the **folder/namespace choice a load-bearing decision**, not cosmetics (see §Architecture Patterns, Pattern 3).

**Primary recommendation:** Plan the phase as (Wave 1) `04-DEVIATIONS.md` + the extended Phase 4 gate asserting the frozen 21-row set and the White_Mod/no-missing-texture invariant, proven end-to-end on one creature (a three-region tracer: 水黾 or 碧灵鮟鱇), then (Waves 2–N) creature groups by region/predicate family, then (final wave) the in-place reconciliation of the 21 rows in `03-BIOLOGY.json`/`.md`, the full offline chain, the Release build and the `04-UAT.md` D-21 bundle.

## Architectural Responsibility Map

| Capability | Primary Tier | Secondary Tier | Rationale |
|------------|--------------|----------------|-----------|
| Creature spawn placement (region/depth/water/time) | tML `ModNPC.SpawnChance` + `NPCSpawnManager.EditSpawnPool` (SP/server only) | `KelpCurtainBiome.IsKelpCurtainLayer` predicate | Spawn weights/ pooling are a server-side tML concern; the biome object owns the world-context test. |
| Subworld isolation (BIO-06) | Biome/layer predicate inside `SpawnChance` | `NPCSpawnManager.EditSpawnPool` filter | The pool filter returns early outside Yggdrasil, so isolation must be enforced by `SpawnChance` returning 0. |
| Creature behavior / AI | `ModNPC.AI()` (module: `...KelpCurtain.NPCs.*`) | Vanilla `aiStyle`/`CloneDefaults` when it fits | AI runs SP/server; the server is authoritative. |
| Hostility / attack / contact damage / status effects | `ModNPC.SetDefaults` + `OnHitPlayer`/`ModifyHitPlayer`/`CanHitPlayer` | Enemy projectile classes under `KelpCurtain/Projectiles/Enemies` | Damage and debuff application are gameplay state; poison/darkness/confusion/slow are applied with `BuffID`. |
| Water-surface / water-bottom detection | Per-creature helper reading `spawnInfo.Water`, `Main.tile[..].LiquidAmount`, `DeathJadeLakeBiome.LiquidSurfaceY` | — | Three distinct design conditions ("水面上 / 水底 / 沉底窒息") must not collapse into one flag. |
| Capture (捕捉) | `Main.npcCatchable` + `NPCID.Sets.CountsAsCritter` + `NPC.catchItem` | Critter `ModItem` (absent → blocker) | Engine-owned capture path; the three capturable creatures have no catch item (item scope, D-58). |
| Drop wiring | `ModNPC.ModifyNPCLoot` + `ItemDropRule.Common` | Phase 1–2 `ModItem` types | Structured loot; the phase only *references* implemented items. |
| Client-only visuals (dust, glow, ink VFX) | `HitEffect`/`AI` dust blocks guarded by `if (!Main.dedServ)` | `KelpCurtain/Dusts`, `VFXs` | Dedicated servers have no graphics services. |
| Art-fallback identity | `ModNPC.Texture => Commons.ModAsset.White_Mod` | Generated `ModAsset` member | Shared fallback; the `.png` is added beside the class when approved art arrives (D-49). |
| Design-status record | `03-BIOLOGY.json` (machine) + `03-BIOLOGY.md` (mirror), 21 rows reconciled in place | Extended phase gate | Phase 4 consumes/extend the Phase 3 artifact (D-24); no schema change. |
| Localization keys | **Out of scope** (D-20) | — | No HJSON is created or hand-edited. |

## Standard Stack

**This phase installs no external packages.** It is pure in-repo C# content plus offline PowerShell gates; the "Standard Stack" is the existing repository toolchain. All versions below were read from the working tree in this session.

### Core

| Library / Tool | Version | Purpose | Why Standard |
|----------------|---------|---------|--------------|
| tModLoader API (`Terraria.ModLoader`) | local tML install at `F:\SteamLibrary\steamapps\common\tModLoader` (verified this session) | `ModNPC`, `NPCSpawnInfo`, `NPCLoot`, `ItemDropRule`, `ModBiome`, `Subworld` | The only supported modding surface [VERIFIED: `Sources/Directory.Build.props:3` `<TargetFramework>net8.0</TargetFramework>`]. |
| SubworldLibrary (`Libraries/SubworldLibrary.dll`) | repository-pinned DLL | `SubworldSystem.IsActive<YggdrasilWorld>()` context checks | Already a module reference [VERIFIED: `Sources/Modules/Directory.Build.props:13-14` `<ProjectReference …Everglow.Function.csproj>` + `:14` `<Reference Include="$(MSBuildThisFileDirectory)\..\..\Libraries\*.dll" />`]. |
| Solaestas.tModLoader.ModBuilder | `1.5.11` | Source-generated `ModAsset` members | Existing asset-access convention [VERIFIED: `Sources/Directory.Build.props:23` `<PackageReference Include="Solaestas.tModLoader.ModBuilder" Version="1.5.11" />`]. |
| MSTest (`Everglow.UnitTests`) | `3.10.2` (per AGENTS.md) | Pure-logic unit tests only | Cannot construct `Main` or load mod content — no unit-test path for `ModNPC` content. |

### Supporting

| Tool | Version | Purpose | When to Use |
|------|---------|---------|-------------|
| .NET SDK | `9.0.306` (builds `net8.0`) | `dotnet build` / `dotnet test` | Every code change [VERIFIED: `dotnet --version` = 9.0.306 this session]. |
| Windows PowerShell | `5.1.26100.9168` | Extended phase gate (ASCII-only) | Before and after each plan wave [VERIFIED: `$PSVersionTable.PSVersion` this session]. |
| Git | `2.37.3.windows.1` | BOM/no-art guards, commits | Repo-wide BOM check after text edits [VERIFIED: `git --version` this session]. |
| StyleCop.Analyzers.Unstable | `1.2.0.556` | Style enforcement during build | Tabs/LF/Allman/file-scoped namespaces [VERIFIED: `Sources/Directory.Build.props:24-27`]. |

### Alternatives Considered

| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| `NPCSpawnManager.RegisterNPC` + `SpawnChance` | A new Kelp-Curtain spawn `ModSystem` | Rejected by D-52/D-33; the existing system already implements subworld filtering and is used by 17+ NPCs. |
| `Commons.ModAsset.White_Mod` fallback | A new placeholder `.png` | Forbidden by AGENTS.md ("Never modify existing binary or art assets… Do not create placeholder art"). |
| `ItemDropRule.Common(...)` in `ModifyNPCLoot` | `NPCLoot()` or a loot bag | D-56 locks direct drop tables; `VampireMat`/`MossyThornTurtle` already use `ItemDropRule`. |
| One `ModNPC` class per design row | Separate classes per stat-variant (soldier 4 / jellyfish 2 / flydragon 2) | Open decision — see Open Question 2; the matrix schema names one `internal_name` per row. |

**Installation:** none — no `npm install`, `pip`, or new `PackageReference`. Existing build command:

```powershell
dotnet build /p:Configuration=Release /p:WarningLevel=0
```

**Version verification:** no new package is recommended, so no registry verification is required. The only versioned inputs are the existing `csproj`/`Directory.Build.props` entries quoted above (all read from the working tree this session).

## Package Legitimacy Audit

**Not applicable — this phase installs no external packages.** It adds C# source files under an existing module and extends one offline PowerShell script; it references no new NuGet package, DLL, or registry artifact. No `gsd-tools query package-legitimacy check` run is required.

| Package | Registry | Age | Downloads | Source Repo | Verdict | Disposition |
|---------|----------|-----|-----------|-------------|---------|-------------|
| — (none added) | — | — | — | — | — | — |

**Packages removed due to [SLOP] verdict:** none
**Packages flagged as suspicious [SUS]:** none

## Architecture Patterns

### System Architecture Diagram

```
Committed snapshot .planning/phases/01-.../evidence/biology.xml   (READ-ONLY, D-25)
        │  (already parsed for Phase 3; Phase 4 reads the same file)
        ▼
03-BIOLOGY.json  (machine source of truth, D-24)  ── 23 rows with phase==4
        │                                                    │
        │  Phase 4 reconciles 21 in-scope rows in place       │ 2 hardmode rows stay deferred
        ▼                                                    ▼
03-BIOLOGY.md  ◄── mirror parity ──►  extended gate  scripts/check-biology.ps1  (D-50)
        │                                       (frozen 21-row set · class resolution ·
        │                                         White_Mod / no-missing-texture · spawn tokens ·
        │                                         no-placeholder-art · BOM)
        ▼
04-DEVIATIONS.md   (D-47 dispositions · conservative defaults · blocker register)

RUNTIME (Yggdrasil Subworld only)

  player enters Yggdrasil ─► tML builds spawn pool
        │
        ▼
  NPCSpawnManager.EditSpawnPool()  ── not in Yggdrasil? ──► return (pool untouched!)
        │ in Yggdrasil
        ▼
  keep only Registered NPC types (+ vanilla slimes)
        │
        ▼
  ModNPC.SpawnChance(spawnInfo)  ── !IsActive<YggdrasilWorld> OR !IsKelpCurtainLayer ──► 0f  (BIO-06)
        │ in layer
        ├─ land creature?   reject spawnInfo.Water + liquid spawn tile
        ├─ water creature?  require spawnInfo.Water; surface/floor variants add a liquid-column test
        ▼
  NPC spawns ─► AI()  ──► dash / ranged / stealth / 4-variant soldier / mini-boss state machine
        │                          └─ dust/VFX inside if (!Main.dedServ)
        │                          └─ authoritative transitions under Main.netMode != MultiplayerClient + netUpdate
        ▼
  NPC death ─► ModifyNPCLoot(npcloot) ─► ItemDropRule.Common(ModContent.ItemType<Phase1/2 item>, denom, min, max)
        │                                        └─ absent material → NO rule + D-58 blocker
        │                                        └─ art: Texture => Commons.ModAsset.White_Mod (D-48)
        ▼
  item reaches player
```

### Recommended Project Structure

```text
Sources/Modules/Yggdrasil/KelpCurtain/
├── NPCs/
│   ├── <Phase 3 classes — MossyThornTurtle, GuppyConch, VerdantRods, GiantDandelion, RiverSlug, VampireMat/>
│   │
│   ├── DeathJadeLake/                  # 13 rows — mirrors Tiles/DeathJadeLake; VampireMat/ proves the subfolder shape
│   │   ├── FluorescentHydra.cs         # 荧光水螅   identity shell (D-45)
│   │   ├── GiantTigerShrimp.cs         # 巨型虎虾   identity shell (D-45)
│   │   ├── WaterStrider.cs             # 水黾       full (surface-water)
│   │   ├── ToxicToad.cs                # 剧毒蟾蜍   full
│   │   ├── GlowSalamander.cs           # 幽光蝾螈   full (3 colour variants)
│   │   ├── ArmoredShrimp.cs            # 装甲虾     full (group spawn; capture blocked)
│   │   ├── BombJellyfish.cs            # 爆弹水母   full (small/large)
│   │   ├── SailfinSnakehead.cs         # 帆鳍鳢     full (neutral)
│   │   ├── Radiolarian.cs              # 放射虫     full (ranged + dash)
│   │   ├── AlgaeOctopus.cs             # 覆藻章鱼   full (stealth/ink)
│   │   ├── LargeAlgaeOctopus.cs        # 大型覆藻章鱼 full (water-bottom)
│   │   ├── JadeSpiritAnglerfish.cs     # 碧灵鮟鱇   full (stealth lamp)
│   │   └── CannonBarnacle.cs           # 炮弹藤壶   identity shell (D-45)
│   │
│   ├── SpinyMossCourt/                 # 3 rows
│   │   ├── AnimatedWitherbarkSoldier*.cs   # 枯木活化士兵 — variant strategy is an open decision (OQ2)
│   │   ├── CourtCommander.cs           # 王庭号令者
│   │   └── BrodieFlydragon.cs          # 布罗迪蝇蜓
│   │
│   └── ValleyOfLushAndMoist/           # 5 rows
│       ├── RedNeedleCaterpillar.cs     # 红针洋辣子
│       ├── AssassinRaspberry.cs        # 阿萨辛覆盘子
│       ├── SerpentMoss.cs              # 蛇行苔
│       ├── SmallGuppyConch.cs          # 小格普螺
│       └── LargeMossyThornTurtle.cs    # 大型荆棘苔龟 (Mini Boss)
│
├── Projectiles/Enemies/                # per-creature hostile projectiles when they carry the core attack
└── Dusts/                              # reuse existing dusts (LichenSlime, JadeLakeSargassum_Dust, FishSkeletonDust, Husk)

.planning/phases/04-remaining-ordinary-monsters/
├── 04-CONTEXT.md / 04-DISCUSSION-LOG.md   (existing)
├── 04-DEVIATIONS.md                       # NEW — dispositions, conservative defaults, blocker register
├── 04-UAT.md                              # NEW — the D-21 client bundle (recorded, not run)
└── (extends) ../03-.../scripts/check-biology.ps1   # D-50
```

> The subfolder layout is **recommended, not locked** (CONTEXT makes folder layout the agent's discretion). See Pattern 3 for why it is the lower-migration-cost choice.

### Pattern 1: Subworld-only spawn registration + layer-gated `SpawnChance`

**What:** Register the NPC so the Yggdrasil pool keeps it, then return 0 unless the Yggdrasil subworld is active *and* the server-safe layer predicate is true.

**When to use:** every Phase 4 creature that spawns naturally (all 21; even shells register, so the class is loadable and bestiary-visible).

**Verified precedent** [VERIFIED: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs:177-185`]:

```csharp
public override float SpawnChance(NPCSpawnInfo spawnInfo)
{
	if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
	{
		return 0f;
	}

	return 1.5f;
}
```

The layer predicate's signature and band — read from the source of truth this session — are:

```csharp
// Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs:76
public static bool IsKelpCurtainLayer(Player player)
// :80
if (player.Center.Y > Main.maxTilesY * 0.72f * 16 && player.Center.Y < Main.maxTilesY * 0.9f * 16)
```

[VERIFIED: `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs:76,80`]

`NPCSpawnManager.RegisterNPC` is the registration seam [VERIFIED: `Sources/Modules/Yggdrasil/Common/NPCSpawnManager.cs:9` `public static void RegisterNPC(int type) => yggdrasilNPC.Add(type);`], and the pool filter that makes it effective returns early outside the subworld [VERIFIED: `NPCSpawnManager.cs:13-16` `if (!SubworldSystem.IsActive<YggdrasilWorld>()) { return; }`].

**Land vs water variants (D-53, design-driven).** Phase 3's land crawlers reject a water spawn explicitly [VERIFIED: `GuppyConch.cs:256-266`]:

```csharp
if (spawnInfo.Water)
{
	return 0f;
}

int tileX = spawnInfo.SpawnTileX;
int tileY = spawnInfo.SpawnTileY;
if (WorldGen.InWorld(tileX, tileY, 1) && Main.tile[tileX, tileY].LiquidAmount > 0)
{
	return 0f;
}
```

Water creatures invert this (`if (!spawnInfo.Water) return 0f;`), and the design's surface-only / water-bottom-only creatures add a liquid-column test on top. **`NPCSpawnInfo` exposes `Water`, `SpawnTileX`, `SpawnTileY`, `Player`, `PlayerFloorX/Y`, `Sky`, `Invasion`, `SafeRangeX`** [VERIFIED: `tModLoader.xml` publicized docs, F:Terraria.ModLoader.NPCSpawnInfo.*] — there is **no** dedicated "water surface" field, so a surface predicate must be derived (see Pattern 4).

### Pattern 2: Direct drop table referencing only implemented items

**What:** `ModifyNPCLoot` + `ItemDropRule.Common(ModContent.ItemType<T>(), denominator, min, max)` with explicit chance/quantity (D-56). Absent designed drops are simply not added and are recorded as blockers (D-58); the method body may legitimately be empty.

**Verified precedents:** `MossyThornTurtle.cs:191-194` (`ItemDropRule.Common(ModContent.ItemType<ThornTurtleShell>(), 20, 1, 1)`) and the deliberately empty `VerdantRods.cs:293-295` with a blocker comment. [VERIFIED: both files read this session]

**Denominator arithmetic** (D-59): `ItemDropRule.Common(item, chanceDenominator, min, max)` — a denominator of `N` is `1/N`. Design percentages map as 25% → `4`, 12.5% → `8`, 6.7% → `15` (=6.67%), 9% (=1/11) → `11`, 33% → `3`, 50% → `2`. Guaranteed → `1`.

### Pattern 3: Art-missing creature class — `White_Mod` + a namespace that the D-49 migration can satisfy

**What:** Every Phase 4 creature overrides `Texture => Commons.ModAsset.White_Mod` (the Phase 2 precedent used for `RadialCarapace`, `ForestBreath` and 18 other art-missing classes) and carries a `LocalizationCategory`.

**Verified shape** [VERIFIED: `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/RadialCarapace.cs:7` `public override string Texture => Commons.ModAsset.White_Mod;`] and [VERIFIED: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs` `public override string Texture => Commons.ModAsset.White_Mod;`]. The backing asset is `Sources/Everglow.Function/Textures/White.png` (exists on disk, verified this session); `Commons.ModAsset.White_Mod` is used by 20+ classes across the repo, so the generated member resolves and compiles.

**Why the namespace/folder choice is load-bearing.** tML resolves an `ModNPC`'s default texture from its namespace path, so the `.png` must later sit at the path the *class's namespace* implies. The repo already proves a subfolder NPC works with default resolution: `NPCs/VampireMat/VampireMat.cs` (namespace `...KelpCurtain.NPCs.VampireMat`) carries `VampireMat.png` beside it and defines **no** `Texture` override [VERIFIED: `VampireMat.cs` read lines 1-80; no `override string Texture` anywhere in the file]. The module's `PathPrefix` is the module name (`Yggdrasil`) [VERIFIED: `Sources/Modules/Directory.Build.props:4-5` `<ModuleName>$(MSBuildProjectName.SubString(9))</ModuleName>` / `<PathPrefix>$(ModuleName)</PathPrefix>`], so `...NPCs.DeathJadeLake.WaterStrider` ↔ `Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.png` ↔ the source file beside the `.cs`.

Consequence: choose the region subfolder now, override `Texture` to `White_Mod` today, and the D-49 migration is **add `<Class>.png` beside the `.cs`, delete the override, delete the blocker** — no namespace change, no asset move. A flat layout works too, but then all 21 art files land in one folder when they arrive. Either is gate-safe; the subfolder is the lower-churn option.

> Phase 3's naming rule ("never place a ModNPC in a subfolder when its approved art sits directly in `NPCs/`") does **not** apply here — its precondition is art that already exists in `NPCs/`, and no Phase 4 creature has art.

### Pattern 4: Server-authoritative AI with `ai[]` wrappers, `netUpdate`, and the three water predicates

**What:** a private state `enum` over `NPC.ai[0]`/`localAI[]`, random/state writes gated by `Main.netMode != NetmodeID.MultiplayerClient`, `NPC.netUpdate = true` on authoritative transitions. This is the `RiverSlug`/`GuppyConch`/`VerdantRods`/`GiantDandelion` precedent (D-55).

**Water-condition helpers** (D-53 — derive them from synced state, never from a screen coordinate):
- **in water**: `spawnInfo.Water` at spawn; `NPC.wet && Main.tile[center].LiquidAmount > 0` in AI.
- **water surface**: `spawnInfo.Water` plus a check that the tile *above* the spawn tile is dry (or `|spawnTileY*16 - DeathJadeLakeBiome.LiquidSurfaceY|` within a couple of tiles). `DeathJadeLakeBiome.LiquidSurfaceY` is a synced static float computed by `GetLiquidSurfaceY()` scanning `TileUtils.SafeGetTile(..., LiquidAmount)` [VERIFIED: `DeathJadeLakeBiome.cs:11,15-27`].
- **water bottom**: liquid column depth below the spawn tile (scan down for a solid floor) — `Radiolarian`/`LargeAlgaeOctopus`/`JadeSpiritAnglerfish` are bottom/floor spawners.

### Anti-Patterns to Avoid

- **Relying on `EditSpawnPool` alone for isolation:** it returns early outside Yggdrasil, leaving the pool untouched — `SpawnChance` must return 0.
- **Using `Main.LocalPlayer` or `Main.screenPosition` in `SpawnChance`:** it runs SP/server-only; use `spawnInfo.Player`.
- **`ModContent.ItemType<T>()` for an absent material:** compile error, breaks the whole mod build (Phase 2's T-02-01). Omit + blocker.
- **Creating any `.png`/`.obj`/`.xnb`** (D-51) or hand-editing HJSON (D-20).
- **Trying to make a capturable creature without a catch item:** `NPC.catchItem` cannot be set to a non-existent type; leave it 0 and record the blocker.
- **Referencing `IsBiomeActive` in `SpawnChance`:** it is camera-driven and evaluates to a different band (and fails on a dedicated server). Use `IsKelpCurtainLayer`.
- **A new generic creature base class/interface:** Phase 3's D-31 analogue — extend `ModNPC` directly.
- **Un-guarded dust/VFX** in `AI`/`HitEffect`: wrap in `if (!Main.dedServ)`.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Subworld-only spawning | A Kelp-Curtain spawn `ModSystem` | `NPCSpawnManager.RegisterNPC` + `ModNPC.SpawnChance` | Already implements the subworld filter; 17+ users. |
| Loot generation | `OnKill` + `Item.NewItem` | `ModifyNPCLoot` + `ItemDropRule.Common` | Handles chance, quantity and drop-luck. |
| Critter capture | custom net/pickup logic | `Main.npcCatchable` + `NPCID.Sets.CountsAsCritter` + `NPC.catchItem` | Engine-owned; `RiverSlug.cs:65-66,84` precedent. |
| Water-surface / bottom detection | a per-frame large tile scan from a screen coordinate | `TileUtils.SafeGetTile` + `tile.LiquidAmount`, `DeathJadeLakeBiome.LiquidSurfaceY`, `Collision.WetCollision`/`NPC.wet` | Existing helpers are sync-safe; screen coordinates are client-only. |
| Status effects | custom DoT tracking | `player.AddBuff(BuffID.Poisoned/Confused/Slow/Darkness/Suffocation/…)` | The design's own debuffs; used throughout KelpCurtain. |
| Subworld/context detection | map size / coordinates / scene effects | `SubworldSystem.IsActive<YggdrasilWorld>()` / `YggdrasilWorld.InYggdrasil` | Yggdrasil AGENTS.md forbids substituting these. |
| Asset paths | handwritten `"Everglow/Yggdrasil/..."` strings | generated `ModAsset.*` (or `Commons.ModAsset.*`) | AGENTS.md: no duplicated hardcoded paths. |
| Localization keys | hand-written HJSON | in-game `OutputLocalizationHjsonItem` exporter | Out of scope (D-20); never hand-edit HJSON. |

**Key insight:** in this domain the expensive failures are *wiring* failures (a leaky spawn, a loot rule naming a missing type, a missing texture aborting load), not algorithm failures. Prefer the engine's extension points and let the design source — not intuition — decide values.

## Common Pitfalls

### Pitfall 1: Main-world leakage (HIGH severity, BIO-06)
**What goes wrong:** a Kelp Curtain creature appears in the ordinary world.
**Why it happens:** `NPCSpawnManager.EditSpawnPool` returns early when `!SubworldSystem.IsActive<YggdrasilWorld>()`, so the pool is *not* filtered outside the subworld; any mod NPC whose `SpawnChance` > 0 is eligible.
**How to avoid:** every `SpawnChance` starts with `SubworldSystem.IsActive<YggdrasilWorld>() && KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` and returns `0f` otherwise; the gate asserts both tokens.
**Warning signs:** a `SpawnChance` that only checks water/time; a creature sighted in a normal world during the D-21 run.

### Pitfall 2: Referencing an absent drop type breaks the build (HIGH severity)
**What goes wrong:** `ModContent.ItemType<MolluscShellFragment>()` for a class that does not exist fails the whole mod compilation.
**Why it happens:** 15 of the 21 rows name an absent material or TBD drop.
**How to avoid:** add a rule only for a verified implemented `T`; omit + blocker otherwise (D-58). Use the empty `ModifyNPCLoot` shape with a blocker comment.
**Warning signs:** a task that adds `ItemDropRule.Common(ModContent.ItemType<毒腺>)` or any type not found under the items tree.

### Pitfall 3: Missing-texture load abort (HIGH severity, D-48/D-50)
**What goes wrong:** a Phase 4 class resolves a texture that does not exist (because the default namespace-derived path was assumed while overriding nothing, or the override was deleted before the `.png` was added), and mod loading aborts with a missing-resource error.
**Why it happens:** no Phase 4 creature has art; a class that neither overrides `Texture` nor has a beside-`.png` requests a non-existent path.
**How to avoid:** every Phase 4 class keeps `public override string Texture => Commons.ModAsset.White_Mod;` until the D-49 migration; the gate asserts each Phase 4 class contains the override (or a resolving beside-`.png`), and asserts the no-art guard.
**Warning signs:** a Phase 4 class with no `Texture` override and no `.png`; a `git status` showing a new `.png` under the NPC tree.

### Pitfall 4: `Main.LocalPlayer` / `Main.screenPosition` in a spawn predicate (MEDIUM severity, multiplayer/DS)
**What goes wrong:** the creature spawns only for the host, or never on a dedicated server.
**Why it happens:** `SpawnChance` runs SP/server-only where the client camera is zero.
**How to avoid:** use `spawnInfo.Player` and only world/tile/player-centre state; the gate fails on `Main.LocalPlayer`.
**Warning signs:** `Main.LocalPlayer` in the predicate; `IsBiomeActive` used instead of `IsKelpCurtainLayer`.

### Pitfall 5: The variant/stat problem (MEDIUM severity, D-47)
**What goes wrong:** a design row with several stat variants (枯木活化士兵 4 types; 爆弹水母 小/大; 布罗迪蝇蜓 小/标准) is crammed into one `ModNPC` type whose `SetDefaults` cannot know its variant, so the stats are wrong at spawn or `NPC.lifeMax` is mutated unsafely.
**Why it happens:** `SetDefaults` is per-type, not per-instance; only `OnSpawn` (or a separate type) can vary stats.
**How to avoid:** pick a variant strategy once, apply it consistently, and record it in `04-DEVIATIONS.md` (Open Question 2). The matrix schema names one `internal_name` per row, so if separate classes are used, name the base class in the row and document the extra classes in `04-DEVIATIONS.md`.
**Warning signs:** `NPC.lifeMax` assigned in `AI`; a row whose `internal_name` is one class but whose design lists four stat rows with no recorded decision.

### Pitfall 6: Water-semantics drift (MEDIUM severity)
**What goes wrong:** 水黾 (surface-only), 装甲虾 (group water), 放射虫 (shallow), 大型覆藻章鱼 (water bottom only) and 碧灵鮟鱇 (water bottom only) spawn in the wrong layer, or an amphibious moisture/suffocation timer never fires.
**Why it happens:** "水面上 / 水底 / 沉底窒息" are three distinct conditions and the design's 潮湿 value is a per-NPC timer, not a player state.
**How to avoid:** one small helper per condition (Pattern 4); keep amphibious timers in `NPC.localAI[]` with `netUpdate` on transitions; the 幽光蝾螈 moisture budget is 60 s with a 10 s "return to water" threshold and a 30-frame suffocation tick at zero.
**Warning signs:** a surface creature spawning on the lake bed; a moisture timer stored on the player.

### Pitfall 7: Gate script is not 100% ASCII (MEDIUM severity)
**What goes wrong:** the extended gate mis-parses its own literals on Windows PowerShell 5.1, which reads BOM-less scripts as the system ANSI code page.
**Why it happens:** the repo forbids UTF-8 BOMs, so CJK cannot be embedded literally.
**How to avoid:** build CJK id/name fragments from `[char]0x….` codepoints (as `check-phase2.ps1` does); read JSON via `[IO.File]::ReadAllText`; resolve classes from the working tree with `Get-ChildItem`, never `git ls-files` (a class created in the same task is still untracked).
**Warning signs:** a gate script whose bytes contain anything above 0x7F.

### Pitfall 8: Matrix/mirror drift (MEDIUM severity, D-24)
**What goes wrong:** `03-BIOLOGY.md` disagrees with `03-BIOLOGY.json`, which Phase 4 and Phase 8 consume.
**Why it happens:** both are hand-edited.
**How to avoid:** update the JSON, regenerate/update the mirror in the same task, and keep the gate's row-id + status parity check green; keep blocker text byte-identical.
**Warning signs:** a row updated in the JSON but not the MD.

### Pitfall 9: Internal-name / compatibility drift (MEDIUM severity)
**What goes wrong:** a creature internal name is later renamed, breaking saves, Bestiary ids and localization keys.
**Why it happens:** this phase invents the class names; they become compatibility-sensitive once committed.
**How to avoid:** choose the 21 names once, list them in `03-BIOLOGY.json` (`internal_name`) and in the plan, and never rename. Record the naming mapping as an assumption.
**Warning signs:** two plans inventing different names for the same design row.

## Code Examples

### The critter/capture precedent (for 装甲虾 / 爆弹水母 / 小格普螺 — capture item absent → blocker)
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs:65-66,84
Main.npcCatchable[NPC.type] = true;
NPCID.Sets.CountsAsCritter[NPC.type] = true;
// …
NPC.catchItem = ModContent.ItemType<RiverSlugItem>();
```
Because no catch item exists for 装甲虾/爆弹水母/小格普螺, **do not** set `catchItem`; record the absent-item blocker. (`CanBeCaughtBy` and `OnCaughtBy` hooks exist in this tML version [VERIFIED: `tModLoader.xml`], but neither can substitute for the absent `ModItem`.)

### `ai[]` wrapper + server-authoritative transition (the shape every full implementation should mirror)
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs:36-58,171-183
private enum GuppyConchState { Crawling = 0, Shelled = 1 }

private GuppyConchState State
{
	get => (GuppyConchState)(int)NPC.ai[0];
	set => NPC.ai[0] = (int)value;
}

private int ShellTimer
{
	get => (int)NPC.localAI[0];
	set => NPC.localAI[0] = value;
}

private void EnterShell()
{
	if (State == GuppyConchState.Shelled) { return; }
	State = GuppyConchState.Shelled;
	ShellTimer = ShellDuration;
	NPC.defense = 20;
	NPC.velocity = Vector2.Zero;
	NPC.netUpdate = true;
}
```
`HitEffect` is the on-hit hook that runs on the server, so the transition lives there under `Main.netMode != NetmodeID.MultiplayerClient` — `ModifyIncomingHit` stays modifiers-only [VERIFIED: `GuppyConch.cs:212-239` and the tML docs pattern Phase 3 recorded].

### Debuff-on-hit and per-type immunity
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs:235-241
public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
{
	if (Main.rand.NextBool(2))
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}
}

// Source: Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs:70
NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
```
`OnHitPlayer` is documented "Called on the local client only" [VERIFIED: `tModLoader.xml`, M:Terraria.ModLoader.ModNPC.OnHitPlayer] — so it must **not** be wrapped in a multiplayer-client guard (that would make the design's poison dead code in MP). `SpawnChance` is documented "Called in single player or on the server only" [VERIFIED: `tModLoader.xml`, M:Terraria.ModLoader.ModNPC.SpawnChance]. `SendExtraAI`/`ReceiveExtraAI` are available for extra variant state [VERIFIED: `tModLoader.xml`, M:Terraria.ModLoader.ModNPC.SendExtraAI / ReceiveExtraAI].

### Dedicated-server guard (mandatory on all new dust/VFX)
```csharp
// Source: Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs
if (!Main.dedServ)
{
	int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, -Projectile.velocity.X * 0.2f, -1f);
	Main.dust[dust].noGravity = true;
}
```

## Creature Map — the 21 in-scope rows (the core deliverable)

Transcription method: the committed snapshot `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` was parsed in memory by block id and each section's raw `InnerText` printed; the values below are verbatim from that extraction (source-of-truth snapshot, read-only per D-25). Confidence tag `[VERIFIED: evidence/biology.xml <block id>]` = verbatim block text extracted this session. Class names are the executor-supplied `name_en` already frozen in `03-BIOLOGY.json` with spaces removed (the naming rule of Phase 3 made English names an assumption; here they are inherited assumptions, `[ASSUMED]` for the naming mapping only).

### Death Jade Lake (13)

| # | Row id / design name | Row block id | Class (recommended) | Mode | Spawn predicate (layer gate +) | Drop wiring | Blockers |
|---|---|---|---|---|---|---|---|
| 1 | `bio-death-jade-lake-fluorescent-hydra` 荧光水螅 | `CvAGdz87Son7TQxKZY8c9Dt5nke` | `FluorescentHydra` | **identity shell (D-45)** — section carries only the heading | land/water unknown → layer + subworld only | none | artwork + "behavior undefined (D-29/D-45)" |
| 2 | `bio-death-jade-lake-giant-tiger-shrimp` 巨型虎虾 | `UJw8dvF1qozy1AxjEBMcne0Ln5f` | `GiantTigerShrimp` | **identity shell (D-45)** | layer + subworld | none | artwork + behavior-undefined |
| 3 | `bio-death-jade-lake-water-strider` 水黾 | `Gca8ddqq9on2lhxraxRcBNkmn6d` | `WaterStrider` | **full** | **water surface only**: require `spawnInfo.Water` + dry tile above (surface test) | none | artwork |
| 4 | `bio-death-jade-lake-toxic-toad` 剧毒蟾蜍 | `KicYd5bbPo4psdxHl1ScX90Bnpf` | `ToxicToad` | **full** | amphibious surface: land conditions (reject `spawnInfo.Water`/liquid tile) | empty + blocker (毒腺 33%, 牛黄 1%/2% absent) | artwork · drop |
| 5 | `bio-death-jade-lake-glow-salamander` 幽光蝾螈（美西螈） | `AnOud27JsouukSxerW4cQ3zWnwb` | `GlowSalamander` | **full** (3 colour variants) | underwater: require `spawnInfo.Water` | empty + blocker (毒腺, 牛黄 absent) | artwork · drop |
| 6 | `bio-death-jade-lake-armored-shrimp` 装甲虾 | `Z94rdc53uoe4Wyx7TRPck6g5nQb` | `ArmoredShrimp` | **full** (passive, group 2–5) | water: require `spawnInfo.Water` | empty + blocker (1 软体甲壳碎片 absent) | artwork · drop · capture item absent |
| 7 | `bio-death-jade-lake-bomb-jellyfish` 爆弹水母 | `LDXndBembo98qsxPzlLcPt0An9d` | `BombJellyfish` | **full** (small/large) | water hover: require `spawnInfo.Water`; small in shallow, large in deep | none | artwork · capture item absent · variant strategy (OQ2) |
| 8 | `bio-death-jade-lake-sailfin-snakehead` 帆鳍鳢 | `BJVjdHX2FoW4MLxOk6ocVw82nUd` | `SailfinSnakehead` | **full** (neutral) | open water: require `spawnInfo.Water` | empty + blocker (亡碧膏 33% absent) | artwork · drop |
| 9 | `bio-death-jade-lake-radiolarian` 放射虫 | `Z1hzdRHyToqCUHxN6qScwKeGnVd` | `Radiolarian` | **full** (ranged + 4-tile dash) | shallow water, lowest weight: require `spawnInfo.Water` | **partial**: `Common(RadialCarapace, 15, 1, 1)`; blockers for 软体甲壳碎片 2–5 + 亡碧膏 1–2 | artwork · drop |
| 10 | `bio-death-jade-lake-algae-octopus` 覆藻章鱼 | `K8PmdSvk3okdbbxo7vYc4flKnKd` | `AlgaeOctopus` | **full** (stealth, ink, Slow) | water: require `spawnInfo.Water`; also 森雨幽谷 (region blocker) | empty + blocker (软体甲壳碎片 + 亡碧膏 absent) | artwork · drop · region |
| 11 | `bio-death-jade-lake-large-algae-octopus` 大型覆藻章鱼 | `AKGxdQG2foyNqBxGXjOcomLVnBb` | `LargeAlgaeOctopus` | **full** (water-bottom, grab, ink wave, flee) | **water bottom only**, lowest weight: `spawnInfo.Water` + floor/depth test | empty + blocker (软体甲壳碎片 2–4, 亡碧膏 3–6, weapons/accessories TBD) | artwork · drop |
| 12 | `bio-death-jade-lake-jade-anglerfish` 碧灵鮟鱇 | `RbODdEzlYo9XtHxQ6z7cmc5Wn2q` | `JadeSpiritAnglerfish` | **full** (stealth lamp, reveal dash) | **water bottom only**: `spawnInfo.Water` + floor test | **full**: `Common(MeatLantern, 4, 1, 1)` (25%) + `Common(Photophore, 8, 1, 1)` (12.5%) | artwork |
| 13 | `bio-death-jade-lake-cannon-barnacle` 炮弹藤壶 | `K4q2dw6hUoGkRrxIn3OcIuVenEb` | `CannonBarnacle` | **identity shell (D-45)** | layer + subworld | none | artwork + behavior-undefined |

**Verbally critical design facts (verbatim, abbreviated):**
- 水黾 `[VERIFIED: evidence/biology.xml Frl7dwvbGoXf8sxSJozc3QfCnF2]`: *"陆生生物，第二层唯一一个陆生但是只会在水面上出现的生物，体型接近史莱姆。只会在水面上刷新，以一小段一小段的无目的的水上冲刺作为移动方式（60~200帧随机一次），如果附近有玩家，则会向玩家冲刺，且冲刺的频率会提高一点（45~150帧随机一次）。如果因为各种原因移动到了陆地上，会尝试小跳回最近的水面（如果没有就随机选择一个方向一直跳）。如果因为各种原因出现在水底，会正常游回水面。"* Stats `[VERIFIED: SqdydxBlPoASZtx5drwc9mZcnfe]`: 生命 45 / 伤害 20 / 防御 10 / 击退抗性 40 / 减伤 (empty) / 免疫 中毒、潮湿 / 钱币 80 / 稀有度 (empty).
- 剧毒蟾蜍 `[VERIFIED: GjwedzoyPouXssx66zpcBbo5nWd]`: *"两栖生物，在地表生成，会主动攻击蝾螈与水蛞蝓，对玩家仇恨低于这些生物。通过发射毒泡泡攻击，不会主动近战但是会造成近战伤害，不论何种方式都会有75%概率造成10秒中毒，剩下25%概率造成7秒酸性毒液。落入水中的话会游泳。死亡后爆炸，产生一小团剧毒云，持续3秒，接触后造成10伤害且造成5秒酸性毒液。"* Drop `[VERIFIED: GftbdkuVuovYXoxpZm7cAE4Mndb]`: *"死亡后33%概率掉落1 毒腺，1%/2%概率掉落 1 牛黄。"* Stats: 生命 80 / 伤害 25（All） / 防御 8 / 击退抗性 80 / 减伤 (empty) / 免疫 中毒、酸性毒液 / 钱币 2银.
- 幽光蝾螈 `[VERIFIED: Dfkidik2VoSGuwxq8YKcjUVpnRg]`: *"两栖生物…拥有三种颜色变种（灰蓝色、粉色、褐色）。在水下生成，在水中冲刺并近战攻击，会主动攻击所有发现的水下生物和蛞蝓，主动远离蟾蜍…拥有潮湿值，每次出水后最多持续60秒，剩余10秒时会尝试爬回最近的水（没有则朝着一个方向爬行），潮湿值清零后每30帧获得1秒窒息，回到水中后移除窒息且恢复潮湿值。只会造成接触伤害，但是会给予10秒中毒。"* Stats: 120 / 30 / 4 / 20 / (empty) / 免疫 中毒 / 2银.
- 装甲虾 `[VERIFIED: ELQgdcR30oJFwAxNBgjcmXmQnXd]`: *"水生生物，被动生物，在水里成群（2~5只）刷新，然后成群移动。可被捕捉，因为智商太低了不会逃离捕食者，死亡后掉落1 软体甲壳碎片，合成也可以1:1获得。"* Stats: 10 / (伤害 empty) / 4 / (KB empty) / (减伤 empty) / (免疫 empty) / (钱币 empty).
- 爆弹水母 `[VERIFIED: OwnZdVZaJogXNSxk6M7cSF2enHW]` + `[VERIFIED: WxyJdoRyXo0cFNxkaGac67tNnde]`: *"水生生物，被动生物，在水中单独刷新并且悬停，只会有些许的上下摆动但是不会移动，在浅水区和森雨幽谷区域只会刷新小号的，深水区还会刷新大一号的版本。"* / *"可被捕捉，死亡后自爆，造成30（小）/50（大）伤害。"* Stats: 10（小）/25（大） / (伤害 empty) / (防御 empty) / 免疫击退 / (减伤 empty) / (免疫 empty) / (钱币 empty).
- 帆鳍鳢 `[VERIFIED: UUgpdyHQjopoSHx7wCzczqxwnPc]`: *"水生生物，中立生物，闲置时匀速在水中游动，获得仇恨时通过快速游动来撞击，会远离蝾螈，主动攻击蛞蝓"*; drop `[VERIFIED: QLkTdqtHoowbKexZttGc71byn5e]` *"死亡后 33%概率掉落1 亡碧膏（掉落物名字随便取的，需要改，就是软体动物提炼后的膏）。"* Stats: 80/140/200 / 30 / 12 / 20 / (减伤 empty) / (免疫 empty) / 5银.
- 放射虫 `[VERIFIED: TdwNdpn2XoFla9x8OUKc8AQanrd]`: *"水生生物，在浅水区生成权重最低，结构抽象的大型水生虫类…在水中时连续发射水弹进行攻击，当仇恨目标靠近时（4格）会尝试冲刺撞击穿过目标，然后拉开距离后重新使用远程（冲刺AI的冷却为300帧），主动攻击蝾螈与玩家，其余生物中立接战。"*; drop `[VERIFIED: PAord6A0NomliPxRzQlcw0wEnMc]` *"死亡后掉落2~5 软体甲壳碎片与1~2 亡碧膏， 6.7%概率掉落 放射状甲壳。"* Stats: 180/300/500 / 35（远程）45（近战） / 15 / 80 / 减伤 10 / 免疫 困惑 / 10银.
- 覆藻章鱼 `[VERIFIED: WwyUddu8SooiTDxy6tPcrgGEn3b]`: *"水生生物，敌对生物，也会在森雨幽谷刷新。无法在远处被观察（隐身），接近后主动攻击玩家与其他水生生物（除了鮟鱇），对玩家施加缓慢，自身能快速冲刺并留下墨水云，死亡后爆出墨水云。墨水云会造成黑暗效果2秒。"*; drop *"死亡后掉落1 软体甲壳碎片与 1 亡碧膏。"* Stats: 70 / 15（近战）25（墨水） / 12 / 10 / (减伤 empty) / 免疫 困惑 / 5银.
- 大型覆藻章鱼 `[VERIFIED: ZyROdHeOZoAGLsxHnTUcIK7unSb]`: *"水生生物，只有在水底才会刷新，权重是全部水生生物最低…尝试抓住猎物施加束缚与窒息（每60帧施加2秒），且每过180帧放出一波（4个，主方向为猎物方向，剩下3个随机）四向扩散的墨水云。并且，冲刺速度非常快…若猎物为玩家且自身残血，会尝试逃离玩家并不断喷射滞留墨水云进行阻挡。墨水云会造成黑暗效果2秒。"*; drop `[VERIFIED: Hco0dB4t1oqGjbx1nXRczxm4nsg]` *"死亡后掉落2~4 软体甲壳碎片与3~6亡碧膏，武器与饰品掉落待定。"* Stats: 类型 稀有 / 360 / 50（近战）60（墨水） / 12 / 20 / 减伤 15 / 免疫 中毒、困惑 / 45银.
- 碧灵鮟鱇 `[VERIFIED: E0aLdZ0tmoEvhTxQvJNcYFeEnxe]`: *"水生生物，只会在水底刷新，隐身但是会亮出绿色的灯，无法在远处被观察，接近后主动攻击玩家与其他水生生物（除了章鱼），破隐第一次冲刺速度非常快且伤害非常高，随后正常速度追随猎物并逐渐隐身（除了灯），完全隐身后失去仇恨并尝试回到水底"*; drop `[VERIFIED: GMYGdtpeZo4oHMx05TdcHK0WnGd]` *"死亡后有25%概率掉落 1 肉食性提灯，12.5%概率掉落1 灵灯。"* Stats: 150 / 60（出动）30（正常） / 8 / 20 / (减伤 empty) / 免疫 中毒、困惑 / 5银.

### Spiny Moss Court (3)

| # | Row id / design name | Row block id | Class | Mode | Spawn predicate | Drop wiring | Blockers |
|---|---|---|---|---|---|---|---|
| 14 | `bio-spiny-moss-court-withered-soldier` 枯木活化士兵 | `S4Jadlq0colTtbx8UagceDb7nCq` | `AnimatedWitherbarkSoldier` (variant strategy OQ2) | **full, D-46 partial** — 4 stat variants; morale bonus unimplemented | land, highest weight (design: "拥有最高的刷新权重") | **partial**: 犬 variant `Common(ActivatedDogStaff, 11, 1, 1)` (9%); blockers for 枯木碎块 (1–2 / dog 1) and 干涸心脏 (morale-gated 50%) | artwork · drop · morale/command system · variant strategy |
| 15 | `bio-spiny-moss-court-court-commander` 王庭号令者 | `M0h6dc7myo8FQTxx1nQcYb2Xn4b` | `CourtCommander` | **full, D-46 partial** — wander/keep-distance/contact + summon 1–3 soldiers implementable; the buff system is not | land, low weight | empty + blocker (枯木碎块 2–4, 干涸心脏 50%) | artwork · drop · morale/command system |
| 16 | `bio-spiny-moss-court-brodie-flydragon` 布罗迪蝇蜓 | `BAQKdvFlvoy7wJxCglZcUWGCnOh` | `BrodieFlydragon` | **full, D-47 case-by-case** — flying melee + small/standard; Valley egg system unimplemented | air, layer + subworld (region refinement for 刺苔庭园/森雨幽谷 blocked) | none | artwork · egg system (Valley) · variant strategy |

**Verbatim design facts:**
- 枯木活化士兵 `[VERIFIED: BYhvdnXwrogK3Jx1z35cwgS7nhg]`: *"庭院的基础敌怪单位，在默认情况下是中立状态，也拥有最高的刷新权重。不同种类拥有不同的攻击方式。"* + `[VERIFIED: MGYedPrhioUFFdxP5XrcMpnxnMd]` *"近战敌怪会像原版的地牢骷髅一样尝试近战。远程敌怪会向玩家投掷巨石，每次需要从地面拾取巨石，经过120帧后才能投出（缓慢的捡石头动画），在此期间无法移动，每次攻击后会尝试和玩家保持8格左右的距离，持续200帧，随后才会进行下一轮攻击。法术敌怪会固定间隔使用一次法术，然后随机传送（原版法师AI），法术为向前喷洒三束受重力影响的粒子。犬类敌怪会高速来回冲撞玩家"* + `[VERIFIED: KzhDde2Q7o3Te6xUDPsclRNqnCW]` *"除了犬，均掉落1~2 枯木碎块。犬类掉落1枯木碎块，9%（1/11）概率掉落1 活化之犬召唤杖。任何枯木士兵在意志高涨期间死亡，都会有50%概率掉落1 干涸心脏。"*
  Stat variants `[VERIFIED: NNe6dSQqxoxXllxT9PvcVmApn1c]` (header 类型/生命/伤害/防御/击退抗性/免疫/速度/钱币): 近战 80/22/4/20/中毒/较快/2银 · 远程 60/35/0/40/中毒、困惑/中等/1银50铜 · 法术 55/24/0/40/中毒、困惑/瞬移/1银50铜 · 犬 75/20/2/70/中毒/快/1银20铜.
- 王庭号令者 `[VERIFIED: PQU5dPXNQo1pnyxAWWpchZ5wnkg]`: *"生成权重较低，每次生成时与初次使用权杖时，如果身边没有枯木活化士兵，生成1~3个随机品种枯木活化士兵。本身在没有仇恨的情况下只会漫无目的地游荡，当玩家进入仇恨范围后，高举权杖，使附近所有的活化士兵战斗意志高涨，随后尝试与玩家保持安全距离（不低于10格），但是能造成接触伤害。意志高涨的活化士兵防御力+4，攻击+35%，移速+15%，若是在此期间击杀枯木人号令者的话则会使这些强化士兵失去战意，重新恢复中立状态。"*; drop `[VERIFIED: OB6sdTaOPoUc70x3zeWcKrHzn2b]` *"掉落2~4 枯木碎块，50%概率掉落1 干涸心脏。"* Stats: 100 / 25（接触） / 4 / 50 KB / 2银50铜.
- 布罗迪蝇蜓 `[VERIFIED: KLvadaAVTodoIWxPu5kcN4Spnsg]`: *"会在刺苔庭园与森雨幽谷刷新，小型的飞行敌怪，在森雨幽谷大面积产卵，主动靠近玩家并且进行近战攻击。"* + `[VERIFIED: Pf95dbVTFodZTMx5qJSc34ZPnbd]` *"自然刷新只会刷新标准大小的蝇蜓，在森雨幽谷打破卵后会刷新小型的蝇蜓。"* Stats: 小 20/15/2/50/(免疫 empty)/中毒/0/(钱币 empty); 标准 40/25/5/50/中毒/20/(钱币 empty).

### Valley of Lush and Moist (5)

| # | Row id / design name | Row block id | Class | Mode | Spawn predicate | Drop wiring | Blockers |
|---|---|---|---|---|---|---|---|
| 17 | `bio-valley-of-lush-and-moist-red-needle-caterpillar` 红针洋辣子 | `XrNWdCklJoFPUyxEtANcQ9tWnNe` | `RedNeedleCaterpillar` | **full** — mirror `BarkSpicyCaterpillar` AI + 4–6 spikes every 180 frames at ≥4 tiles | land, layer + subworld | **wireable**: mirror the existing `BarkSpicyCaterpillar` drop `Common(CaterpillarJuice, 1, 1, 2)` (see OQ1) | artwork · OQ1 drop-scope decision |
| 18 | `bio-valley-of-lush-and-moist-assassin-raspberry` 阿萨辛覆盘子 | `TjICdnhRvowaSdxcAcXchNGDnco` | `AssassinRaspberry` | **full, D-46 partial** — stationary trigger + 4–6 ground spikes at 4–8 tiles implementable | land, layer + subworld | none | artwork · disguised-hazard visual system |
| 19 | `bio-valley-of-lush-and-moist-serpent-moss` 蛇行苔 | `C8fRdR6uhoYxOKxrWzDc8IHwnud` | `SerpentMoss` | **full, D-46 partial** — bind + 15 dmg/60 frames + 33% 15s poison implementable | land, layer + subworld | none | artwork · disguised-hazard visual system |
| 20 | `bio-valley-of-lush-and-moist-small-guppy-conch` 小格普螺 | `BY1nd7O1Xokz4dxb8Mlc2MrVnwd` | `SmallGuppyConch` | **full** — passive, capturable | land, layer + subworld | none | artwork · capture item absent |
| 21 | `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle` 大型荆棘苔龟 | `doxcnzBmJS1yk7hq4csTiRt20ut` | `LargeMossyThornTurtle` | **full, D-47 case-by-case** — Mini Boss 4-state machine | land, very rare | empty + blocker (死亡掉落物暂定 / TBD) | artwork · drop TBD |

**Verbatim design facts:**
- 红针洋辣子 `[VERIFIED: Ofr1dFMe4ot9tLxq6hicZfLxnzh]` + `[VERIFIED: G1ModIH6no6tFXxeuLCcZ8HYnGc]`: *"AI同树皮刺毛虫，区别是与玩家距离不低于4格时，每过180帧会像尖刺史莱姆一样在头部发射4~6尖刺，且命中后有25%概率会造成中毒20秒，37.5%概率造成中毒10秒。"* / *"掉落同树皮刺毛虫。"* Stats: 60 / 20（近战）15（远程） / 4 / KB `-12/-8/4` (a per-difficulty progression, see below) / (减伤 empty) / 免疫 中毒 / 80铜.
- 阿萨辛覆盘子 `[VERIFIED: PqdMdnpwQomTTIxDt3PcdyGDnNd]`: *"静止的敌对生物，覆盘子状怪物，玩家在距离恰好为4~8格时，会伸出并向着玩家随机散射4~6颗地刺，少于4格无法攻击但是不会缩回去，超过8格后重新缩回地下。"* Stats: 80 / 30 / 20（被动）4（攻击） / 免疫击退 / (减伤 empty) / 免疫 中毒、困惑 / 1银.
- 蛇行苔 `[VERIFIED: RmwmdtUCwoMPI1xMVQrclFS4nuh]`: *"敌对生物，出生后伪装成环境植物，在玩家接近（不低于2格）后，束缚玩家并持续造成伤害。束缚玩家后，每60帧对玩家施加60帧束缚并造成15伤害，每次造成伤害后，有33%概率对玩家造成15秒中毒。"* Stats: 100 / 15 / 10 / 免疫击退 / (减伤 empty) / 免疫 中毒、困惑 / 2银.
- 小格普螺 `[VERIFIED: MGMFdA62foF8gix4hqYcHHIpnhb]`: *"被动生物，可以被捕获。"* Stats: 20 / (伤害 empty) / 5 / (KB empty) / 减伤 5 / (免疫 empty) / (钱币 empty).
- 大型荆棘苔龟 `[VERIFIED: TyxWdwlaMoE4mhxYWfEcM9gtnUg]`: *"非常稀有的MiniBoss，背上长满石笋和苔藓的大王八。正常形态下不会穿墙，会在地面上非常非常缓慢（1格/s）地爬行，并不会主动攻击玩家，当玩家对王八造成伤害且王八生命值不高于900时，进入敌对状态，攻击模式如下：1、缩壳，此时玩家如果攻击会受到自身攻击伤害的20%反伤，取值范围为2~20，当超过100帧没有受到伤害或者缩壳时间超过320帧后，伸出头，进入状态2。2、伸出头，震击地板，产生范围的震荡波，间隔120帧后重复1次，间隔120帧后进入状态3。3、缩壳，像原版王八一样飞天下坠，但是无视物块碰撞，落地后震击一次地板，然后从上方屏幕外均匀落下3颗巨石，然后进入状态4。4、伸出头，向着远离玩家的方向以王八飞快地爬的速度（2格/s）爬行240帧，然后背对玩家回到状态1"*; drop `[VERIFIED: E3X5dKd17ohqDExLK0dcZpVCnyh]` *"死亡掉落物暂定"*. Stats: 类型 Mini Boss / 1000 / 50/100/150（接触）85/160/240（旋转，或者玩家y轴大于自身时接触）70/140/200（震荡波）55/100/150（巨石） / 10（正常）999（缩壳） / 免疫击退 / 减伤 20 / 免疫 中毒、困惑 / 1金.

### Drop availability cross-check (the D-57/D-58 contract)

| Design drop | Implementing phase | Implemented? | Wiring decision |
|---|---|---|---|
| 放射状甲壳 | Phase 1 (`item-weapons.misc-radial-carapace`, `code_complete=true`, art `White_Mod`) | **yes** | wire `RadialCarapace` (6.7% → denom 15) |
| 肉食性提灯 | Phase 1 (`item-weapons.melee-…`, green) | **yes** | wire `MeatLantern` (25% → denom 4) |
| 灵灯 | Phase 1 (`item-weapons.misc-photophore`, yellow) | **yes** | wire `Photophore` (12.5% → denom 8) |
| 活化之犬召唤杖 | Phase 1 (`item-weapons.summon-activated-dog-staff`, green) | **yes** | wire `ActivatedDogStaff` (9% → denom 11, 犬 variant only) |
| (红针洋辣子) 同树皮刺毛虫 | pre-existing `YggdrasilTown/Items/Materials/CaterpillarJuice.cs`; `BarkSpicyCaterpillar.cs:288` drops `Common(CaterpillarJuice, 1, 1, 2)` | **yes** (outside the Kelp Curtain inventory) | OQ1 — see Open Question 1 |
| 毒腺 | — | no | empty + blocker |
| 牛黄 | — | no | empty + blocker |
| 软体甲壳碎片 | — | no | empty + blocker |
| 亡碧膏 | — | no | empty + blocker |
| 枯木碎块 | — | no | empty + blocker |
| 干涸心脏 | — | no | empty + blocker (morale-gated) |
| 大型荆棘苔龟 drops | — | no (design TBD) | empty + blocker |

[VERIFIED: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs` (read), `MossyThornTurtle.cs` (read), `GuppyConch.cs` (read), `VerdantRods.cs` (read), `VampireMat/VampireMat.cs` (read), `KelpCurtainBiome.cs` (read), `Biomes/DeathJadeLakeBiome.cs` (read), `Common/NPCSpawnManager.cs` (read), `Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/BarkSpicyCaterpillar.cs:286-288` inspected, `.planning/phases/01-…/01-INVENTORY.json` entries matched by `internal_name`, `Sources/Directory.Build.props` / `Sources/Modules/Directory.Build.props` read]

## Matrix Reconciliation (Phase 4's structural deliverable, D-24/D-50)

Phase 4 **does not create a new matrix**: it reconciles the 21 `phase:4` non-deferred rows in the existing `03-BIOLOGY.json` and mirrors them in `03-BIOLOGY.md`, then extends the gate. The frozen `counts` block must not change: `rows 31, phase3 5, phase4 23, phase7 3, deferred 2, texture_complete_true 6, design_art_true 9` [VERIFIED: `03-BIOLOGY.json` line 13-21 and `check-biology.ps1:103-121`].

Per-row changes required (all 21):
- `code_complete`: `false → true`
- `internal_name`: `"" → "Everglow.Yggdrasil.KelpCurtain.NPCs.<Region>.<Class>"` (must resolve to a `<Class>.cs` on disk; the gate resolves it with `Get-ChildItem -Recurse`, so subfolders are fine [VERIFIED: `check-biology.ps1:150-152`]).
- `blockers`: replace `artwork: no approved repository texture; Phase 4 (D-41)` with the D-48 wording (e.g. `artwork: approved texture (贴图) missing from repository; uses Commons.ModAsset.White_Mod (D-48)`), keep `behavior undefined in the design row (D-29 shell)` for the 3 shells, and add the per-row drop/morale/disguised-hazard/region/variant blockers and the canonical `localization deferred (D-20); runtime verification outstanding (D-21)` element (Phase 3's uniform tail).
- `status` stays `unchecked` (D-22/D-60; no Feishu writes — Phase 8).

Phase 3's gate invariants must stay green: the 5-id `phase3_tranche` set, the 6-id `texture_complete` set, the 9 design-art headings, and the 5 already-implemented classes are frozen and must not be touched.

**Extended Phase 4 gate invariants** (D-50; a new Phase 4 script under `.planning/phases/04-…/scripts/` reading the shared `03-BIOLOGY.json` is the cleanest faithful reading of D-50, leaving the Phase 3 script's phase-3 assertions byte-identical — flag as a small decision):
1. `[phase]==4 && !deferred` is exactly the frozen 21-id set (D-44).
2. `[phase]==4 && deferred` is exactly `{bio-out-of-phase-withered-seed, bio-out-of-phase-withered-tree-guardian}` with a non-empty `deferred_reason`.
3. Every in-scope row is `code_complete` with a resolving `internal_name`.
4. Every in-scope row carries a non-empty artwork/texture blocker (D-50).
5. Every guarded Phase 4 class that has no beside-`.png` contains `Commons.ModAsset.White_Mod` (no missing texture reference can abort loading).
6. Every guarded Phase 4 class declaring `SpawnChance` references both `SubworldSystem.IsActive<YggdrasilWorld>` and `KelpCurtainBiome.IsKelpCurtainLayer`; no class contains `Main.LocalPlayer`.
7. Every guarded Phase 4 class with dust/gore/VFX tokens contains `Main.dedServ`.
8. Every `ModContent.ItemType<X>()` in a Phase 4 class resolves to an existing `<X>.cs` **under the whole `Sources/Modules/Yggdrasil` tree** (not only `KelpCurtain/Items`) — required because `CaterpillarJuice` lives under `YggdrasilTown` (OQ1). Phase 3's gate used `KelpCurtain/Items` only [VERIFIED: `check-biology.ps1:63,221-227`].
9. No added/modified `*.png` under the NPCs tree (D-51).
10. Markdown mirror row-id + per-row `status` parity for all 31 rows (Phase 3 invariant 11).
11. Byte-level UTF-8 BOM check over the phase's change set.
12. Exit codes 0 OK / 1 invariant failure / 2 missing input.

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| `NPCLoot()` override | `ModifyNPCLoot(NPCLoot)` + `ItemDropRule` | tML 1.4 (already in repo) | chance/quantity/expert-master handled by the engine. |
| Manual `AddContent` | automatic content discovery of `Everglow.*` | existing repo | just add a `ModNPC` subclass + `NPCSpawnManager.RegisterNPC`. |
| Per-mod spawn systems | shared `NPCSpawnManager.EditSpawnPool` + per-NPC `SpawnChance` | existing repo | subworld isolation centralized; one line to opt in. |
| Phase 3: default texture resolution beside existing `.png` | Phase 4: `White_Mod` override + beside-`.png` added later (D-48/D-49) | this phase | the class loads with no art and migrates by adding the file and deleting the override. |

**Deprecated/outdated in this repo:**
- `Modules/IIID`, `Modules/TwilightForest`, `Modules/ZY`, `Sources/Everglow.Scripts` — dead; never add content there.
- `Sources/Everglow/Localization/en-US_Mods.Everglow.hjson` — legacy root file; never add keys.
- The `03-RESEARCH.md` "design-art `<img>` = texture-complete" rule — superseded by D-41/D-42.

## Assumptions Log

> Every claim below is `[ASSUMED]`. None may become a locked implementation decision without user confirmation. Locked decisions from CONTEXT.md (D-44…D-59) are **not** repeated here.

| # | Claim | Section | Risk if Wrong |
|---|-------|---------|---------------|
| A1 | The 21 class names (spaces removed from the frozen `name_en`: `WaterStrider`, `AnimatedWitherbarkSoldier`, `LargeMossyThornTurtle`, …) are acceptable internal names. | Creature Map | A rename later breaks saves/Bestiary/localization (Pitfall 9). |
| A2 | Conservative spawn weights: land 0.75f–1.5f; water 0.5f–1f; rare/bottom 0.25f–0.5f; mini-boss ≤0.1f; shell 0 (or 0.5f). Calibrated against `NPCSpawnManager`'s 0.1f slime weights and Phase 3's bands. | Spawn plan | Density tuning churn; excessive spawns. |
| A3 | Drop denominators: 25%→4, 12.5%→8, 6.7%→15, 9%→11, 50%→2, 33%→3, guaranteed→1. | Drop wiring | Drop-rate drift from the design. |
| A4 | `稀有` rarity maps to `ItemRarityID.LightPurple` (Phase 3's 巨树人 precedent) and `Mini Boss` 大型荆棘苔龟 keeps the empty-rarity default (`ItemRarityID.White`) because its 稀有度 cell is empty. | Creature Map | Lifeform Analyzer display only; low impact. |
| A5 | `BuffID.AcidVenom` and `BuffID.Wet` are the correct member names for 酸性毒液 / 潮湿 (neither is used anywhere in the repo and neither appears in the publicized XML; `BuffID.Poisoned`/`Confused`/`Slow`/`Darkness`/`Suffocation` are confirmed). | Creature Map / status effects | A compile error at implementation; one-line fix, caught by the Release build. |
| A6 | 红针洋辣子's "掉落同树皮刺毛虫" may be satisfied by referencing the pre-existing `CaterpillarJuice` (outside the Kelp Curtain Phase 1–2 inventory). | Creature Map / OQ1 | A scope question, not a technical one; a D-58 empty table + blocker is the fallback. |
| A7 | The variant strategy: one `ModNPC` class per matrix row for colour-only variants (幽光蝾螈), separate classes for stat variants (soldier 4, jellyfish 2, flydragon 2), with the extra classes documented in `04-DEVIATIONS.md`. | Pattern 3 / OQ2 | Wrong stats at spawn or an unrecorded class; rework. |
| A8 | The 大型荆棘苔龟's `-12/-8/4` 击退抗性 cell is a difficulty progression whose normal-state value is the first (`-12`), per Phase 3's "slash-separated stats: first value" rule. | Creature Map | `knockBackResist` wrong; note a negative resistance value needs a sanity clamp. |
| A9 | Water-surface spawning for 水黾 is satisfiable with `spawnInfo.Water` + a dry-tile-above test (there is no tML surface flag). | Pattern 4 | Spawn may occur slightly below the surface; needs client tuning. |

## Open Questions (RESOLVED — 2026-09-15)

> **All five resolved before planning; the resolutions are adopted by the plans.** Where a resolution differs from the recommendation below, the resolution wins.
> - **OQ1 — `CaterpillarJuice`:** RESOLVED — wire it for 红针洋辣子; D-57's intent is "no absent type reference + no new item scope", which an existing implemented item satisfies. The gate's item index spans the whole `Sources/Modules/Yggdrasil` tree. Adopted in `04-07` (gate §6.1) and recorded in `04-DEVIATIONS.md`.
> - **OQ2 — variants:** RESOLVED — separate classes for stat variants (base variant named in the row's `internal_name`); one class + `NPC.localAI[]` variant index for colour-only (幽光蝾螈). Adopted in `04-01`/`04-06`.
> - **OQ3 — gate placement:** RESOLVED — new `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1` reading the shared `03-BIOLOGY.json`; the Phase 3 script stays byte-identical. Adopted in `04-01` §9.
> - **OQ4 — projectiles:** RESOLVED — build the self-contained primary-attack projectiles; pure-VFX/system behaviours become effect blockers. Adopted in `04-01` §7/§8 and the wave plans. The built set is **eleven (11)** projectiles: the recommendation in question 4 below plus `ToxicToad_PoisonCloud` (剧毒蟾蜍's death cloud) and `BombJellyfish_Explosion` (爆弹水母's 30/50 death blast), both of which carry a creature's primary threat. That 11 is the frozen list in `04-01` §7 and in the gate's `$phaseProjectiles`, and it is the number the ROADMAP §Phase 4 scope anchor states (26 `ModNPC` classes + 11 projectiles = the 37 guarded classes of gate invariant 7).
> - **OQ5 — 大型荆棘苔龟:** RESOLVED — implement the full four-state mini-boss machine (no state-3 blocker). Adopted in `04-08` (and `04-01` §8).

1. **Is `CaterpillarJuice` (a pre-existing `YggdrasilTown` item) an allowed drop reference for 红针洋辣子?**
   - What we know: 红针洋辣子's design says *"掉落同树皮刺毛虫"*; `BarkSpicyCaterpillar.cs:288` drops `CaterpillarJuice` 100% ×1–2; `CaterpillarJuice` is implemented but is **not** in `01-INVENTORY.json` (it predates the Kelp Curtain item inventory).
   - What's unclear: whether D-57's "only items already implemented in Phases 1–2" admits a pre-existing implemented item outside that inventory.
   - Recommendation: wire `CaterpillarJuice` (compile-safe, faithful to the design) and record the cross-namespace decision in `04-DEVIATIONS.md`; if the planner prefers strict D-57, ship an empty table + blocker. Either satisfies D-58's "mod must still build and load". The gate's item index must cover the whole Yggdrasil tree either way.

2. **How are the stat variants represented — separate classes or one class with per-instance scaling?**
   - What we know: 枯木活化士兵 has 4 stat rows (80/60/55/75 life) and different drops (only 犬 drops the staff); 爆弹水母 has 小/大 (10/25); 布罗迪蝇蜓 has 小/标准 (20/40); the matrix schema names a single `internal_name` per row and `SetDefaults` is per-type.
   - What's unclear: whether mutating `NPC.lifeMax` in `OnSpawn` is acceptable here, or whether the extra classes are preferable.
   - Recommendation: **separate classes for stat variants** (idiomatic tML; avoids `lifeMax` mutation), name the base variant in the row's `internal_name`, and document the sibling classes in `04-DEVIATIONS.md`. For colour-only variants (幽光蝾螈) use one class with a variant index over `NPC.localAI[]` and choose frame/colour in `FindFrame`/`AI`. Record the decision for all three creatures in `04-DEVIATIONS.md` (D-47).

3. **Does the extended gate edit the Phase 3 script in place, or add a Phase 4 script?**
   - What we know: D-50 names `scripts/check-biology.ps1`; the Phase 3 gate lives at `.planning/phases/03-…/scripts/check-biology.ps1` and its phase-3 invariants are frozen by the Phase 3 close-out record.
   - What's unclear: which path D-50 intends.
   - Recommendation: add `.planning/phases/04-…/scripts/check-biology.ps1` reading the shared `03-BIOLOGY.json`, mirroring the Phase 3 conventions, and leave the Phase 3 script byte-identical (the Phase 3 close-out chain then still reproduces). If the planner prefers the literal in-place extension, add the Phase 4 invariants as an opt-in switch so the Phase 3 output lines are unchanged.

4. **Which projectiles/VFX are built now vs recorded as effect blockers?**
   - What we know: Phase 3 built two `White_Mod` projectiles for 巨树人; the discretion allows either.
   - Recommendation: build the projectile when it carries the creature's primary attack and is self-contained (剧毒蟾蜍 poison bubble, 放射虫 water bolt, 红针洋辣子 spikes, 阿萨辛覆盘子 spikes, 枯木活化士兵 ranged boulder + spell, 大型覆藻章鱼/覆藻章鱼 ink cloud + ink wave, 大型荆棘苔龟 shockwave + boulders); record an effect blocker for pure-VFX/system behaviours (ink-cloud lingering rendering, Valley egg system, morale visual, disguise visual).

5. **How much of the 大型荆棘苔龟 Mini Boss state machine is in scope?**
   - What we know: the design gives the full 4-state machine with exact timings (100 / 320 frames retract, 120-frame repeats, 240-frame retreat, 3 boulders, 999 缩壳 defence, 20% reflect 2–20) — it is the richest single row in the phase and is `phase:4` (not Phase 7).
   - Recommendation: implement it fully if the wave budget allows (the design is complete enough per D-47); otherwise implement the normal/retract/state-2 core and record the state-3 flight-and-rock-rain as an effect blocker. Record the call in `04-DEVIATIONS.md`.

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|-------------|-----------|---------|----------|
| .NET SDK | `dotnet build` / `dotnet test` | ✓ | 9.0.306 (builds `net8.0`) | — |
| tModLoader install (`tMLMod.targets`, `tModLoader.dll`, publicized `tModLoader.xml`) | every build; API contract lookups | ✓ | `F:\SteamLibrary\steamapps\common\tModLoader` | — |
| `tModLoader.targets` ancestor | build target resolution | ✓ | `E:\Documents\My Games\Terraria\tModLoader\ModSources\tModLoader.targets` → `F:\…\tMLMod.targets` | — |
| Windows PowerShell 5.1 | extended gate | ✓ | 5.1.26100.9168 | — |
| Git | BOM/no-art guards, commits | ✓ | 2.37.3.windows.1 | — |
| Node | GSD tooling | ✓ | v22.18.0 | — |
| tModLoader client (D-21 runtime verification) | SC1/SC3 client checks | ✓ (install present; not launched this session) | — | Batched with the deferred Phase 2/3 UAT (`human_verify_mode: end-of-phase`) |
| XNA runtime components (shader compile) | `CompileEffect=true` builds | assumed ✓ (Phase 1–3 built in this repo) | — | — |
| Feishu / `lark-cli` | **not required** (D-25: committed snapshot only) | n/a | — | — |

**Missing dependencies with no fallback:** none identified.
**Missing dependencies with fallback:** none identified.

## Validation Architecture

> `workflow.nyquist_validation` is `true` in `.planning/config.json:24`, so this section is required. `security_enforcement` is `true` with `security_asvs_level: 1` (`config.json:47-49`).

### Test Framework
| Property | Value |
|----------|-------|
| Framework | **none for `ModNPC` content** — MSTest 3.10.2 (`Sources/Everglow.UnitTests`) cannot construct `Main`, load mod content, or run the game loop; the phase's verification is offline PowerShell gates + `dotnet build` + a client UAT batch. (Mirrors `03-VALIDATION.md:21`.) |
| Config file | none (`Sources/Everglow.UnitTests` has no test config file; tests are discovered by the SDK) |
| Quick run command | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| Full suite command | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → the Phase 4 gate (reads `03-BIOLOGY.json`) → `03-…/scripts/check-biology.ps1 -RequireAll` → Phase 1 gates (`check-inventory-reconciliation`, `check-carryover`, `check-tranche-A`, `check-tranche-B`, `check-armofgianttree-charge`) → `check-phase2.ps1` → `dotnet test --filter "FullyQualifiedName~Yggdrasil"` → the AGENTS.md byte-level BOM block |
| Estimated runtime | ~60–150 s (build dominates) |

### Phase Requirements → Test Map
| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|--------------|
| BIO-01/02/03 | The in-scope set is exactly the frozen 21 rows | structural (JSON assertion) | Phase 4 gate: `[phase]==4 && !deferred` set compare | ❌ Wave 0 |
| BIO-01/02/03 | Every in-scope row has a loadable `ModNPC` class on disk | structural | Phase 4 gate: `internal_name` → `<Class>.cs` via `Get-ChildItem -Recurse` | ❌ Wave 0 (classes) |
| BIO-01/02/03 | No Phase 4 class references a non-existent texture | static source check (gate) + build | Phase 4 gate: `White_Mod` override present or beside-`.png`; Release build proves resolution | ❌ Wave 0 |
| BIO-01/02/03 | Spawn is subworld-isolated | static + client | gate asserts `SubworldSystem.IsActive<YggdrasilWorld>` **and** `KelpCurtainBiome.IsKelpCurtainLayer` in every `SpawnChance`; client confirms no main-world spawn | ❌ Wave 0 |
| BIO-01/02/03 | Loot tables reference no unimplemented item type | static (gate) + build | gate resolves every `ModContent.ItemType<X>()` under the Yggdrasil tree; `dotnet build` proves type resolution | ❌ Wave 0 |
| BIO-06 (advanced) | No main-world behavior introduced | static + dedicated-server smoke | gate regex + a normal-world idle run + a dedicated-server launch | ❌ Wave 0 |
| QUAL-01 (advanced) | Build clean | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ |
| QUAL-03 (advanced) | Multiplayer/dedicated-server safety | static + manual | gate asserts `spawnInfo.Player` (not `Main.LocalPlayer`) and `!Main.dedServ`; manual dedicated-server launch | ❌ Wave 0 |
| QUAL-04 (advanced) | Per-creature comparison record | structural | Phase 4 gate invariants 1–4, 10 | ❌ Wave 0 |

### Sampling Rate
- **Per task commit:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` for any C# change; the Phase 4 gate for matrix/gate changes.
- **Per wave merge:** the Phase 4 gate + `03-…/scripts/check-biology.ps1` + `dotnet build /p:Configuration=Release /p:WarningLevel=0` + the Phase 1/2 regression gates.
- **Phase gate:** full suite green, plus the D-21 client/`!Main.dedServ` UAT bundle (`04-UAT.md`), before `/gsd-verify-work`.
- **Max feedback latency:** ~150 s.

### Wave 0 Gaps
- [ ] `.planning/phases/04-…/04-DEVIATIONS.md` — D-47 per-creature dispositions, conservative spawn weights (D-54), drop denominators (D-59), the blocker register.
- [ ] `.planning/phases/04-…/scripts/check-biology.ps1` (or the extended Phase 3 script) — authored before the classes so it runs red→green.
- [ ] `.planning/phases/04-…/04-UAT.md` — the D-21 client bundle (recorded, not executed).
- [ ] 21 `ModNPC` class files under the recommended region subfolders.
- [ ] The reconciled 21 rows in `03-BIOLOGY.json` + `03-BIOLOGY.md`.
- [ ] Framework install: **none** — existing infrastructure (MSTest + PowerShell + build) covers the phase.

### Manual-Only Verifications
| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Mod loads with no missing-resource error; each new creature appears | BIO-01/02/03 | Runtime loader property; no offline script observes it (D-21) | Launch a client, enter the Kelp Curtain context, confirm clean load and creature presence in each region |
| Subworld isolation — no creature spawns in the main world | BIO-06 | Runtime spawn behavior | Normal-world idle run + dedicated-server launch; confirm zero main-world spawns |
| Per-creature water/land behavior, hostility, status effects, drops | BIO-01/02/03 | Runtime gameplay property | Walk each documented context and observe behavior; verify `碧灵鮟鱇`'s two drops, `放射虫`'s shell, `枯木活化士兵 犬`'s staff |
| Dedicated-server safety (no graphics crash) | QUAL-03 | Runtime | Start a dedicated server; confirm no crash and no client-only work |

## Security Domain

> Required — `security_enforcement: true`, `security_asvs_level: 1`, `security_block_on: high` (`.planning/config.json:47-49`).

### Applicable ASVS Categories
| ASVS Category | Applies | Standard Control |
|---------------|---------|-----------------|
| V2 Authentication | no | No auth surface; the phase adds gameplay NPCs. |
| V3 Session Management | no | No session state introduced. |
| V4 Access Control | no | No privilege boundary; subworld isolation is gameplay scoping. |
| V5 Input Validation | **yes** | All design-derived values come from the committed XML snapshot; the gate validates shape/counts/classification. No untrusted runtime input (no network, no file input). |
| V6 Cryptography | no | None. |
| V7 Error handling / logging | **yes (light)** | Gate scripts exit non-zero on failure (0/1/2) and never report a failing gate as passing; no silent `catch`. |
| V11 Business logic | **yes** | Server-authoritative AI: no client-side authority over spawn/state; `netUpdate` on authoritative changes. |

### Known Threat Patterns for this stack
| Pattern | STRIDE | Standard Mitigation |
|---------|--------|---------------------|
| Main-world leakage of a subworld-only creature (BIO-06) | Elevation of Privilege / Tampering (gameplay boundary) | `SpawnChance` returns 0 outside `SubworldSystem.IsActive<YggdrasilWorld>()`; `NPCSpawnManager` filter is a second layer. |
| Client-authoritative NPC state (desync/duplication) | Tampering | Random/state transitions only under `Main.netMode != NetmodeID.MultiplayerClient`; `NPC.netUpdate = true`; `SendExtraAI`/`ReceiveExtraAI` for variant state. |
| Dedicated-server crash from graphics access | Denial of Service | `!Main.dedServ` around every dust/VFX/texture call. |
| Null-reference / missing-resource from absent art | Denial of Service (mod fails to load) | `White_Mod` override on every art-missing class; never reference a non-existent path; never create placeholder art. |
| Compile-time break from a loot rule naming an absent item | Denial of Service (build fails) | Add a rule only for a verified implemented type; omit + blocker otherwise. |
| Non-deterministic/unauditable status record | Repudiation | Matrix + gate: frozen 21-row set, class resolution, blockers, no-art guard asserted on every run. |

## Sources

### Primary (HIGH confidence)
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs` (read in full this session) — spawn gate, `PostAI`, loot, `White_Mod`-free default texture.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs` (read in full) — `ai[]` state enum, land spawn rejection, `ModifyIncomingHit`, `HitEffect` transition.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs` (read in full) — empty `ModifyNPCLoot` + blocker, `OnHitPlayer` poison, water suffocation.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VampireMat/VampireMat.cs` (read lines 1–80) — subfolder NPC with default texture resolution.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs` (grep: lines 65-66,84-85,385) — critter/capture precedent, `SpawnModBiomes`.
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs` (read in full) — `IsKelpCurtainLayer(Player)` signature + band.
- `Sources/Modules/Yggdrasil/KelpCurtain/Biomes/DeathJadeLakeBiome.cs` (read in full) — region predicate, `LiquidSurfaceY`.
- `Sources/Modules/Yggdrasil/Common/NPCSpawnManager.cs` (read in full) — `RegisterNPC`, `EditSpawnPool` early-return.
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs` / `_Boulder.cs` (read) — Phase 3 hostile-projectile + `White_Mod` precedent.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/RadialCarapace.cs` (grep) — `Texture => Commons.ModAsset.White_Mod`.
- `Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/BarkSpicyCaterpillar.cs` (grep: line 288) — `CaterpillarJuice` drop; `…/YggdrasilTown/Items/Materials/CaterpillarJuice.cs` exists.
- `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` (parsed by block id this session) — the 21 creature sections' stats/behavior/drop text.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` (entries matched by `internal_name`) — `RadialCarapace`/`MeatLantern`/`Photophore`/`ActivatedDogStaff` code-complete.
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` / `03-BIOLOGY.md` (read) — the 23 `phase:4` rows, frozen counts.
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` (read) — tranche rule, conservative defaults, blocker conventions, close-out.
- `.planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1` (read in full) — gate conventions and invariants to extend.
- `.planning/phases/03-completed-art-ordinary-monsters/03-VALIDATION.md` (read in full) — validation-architecture format.
- `Sources/Directory.Build.props` / `Sources/Modules/Directory.Build.props` / `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj` (read) — target framework, module/pathprefix, references.
- `F:\SteamLibrary\steamapps\common\tModLoader\tModLoader.xml` (publicized docs, grep) — `NPCSpawnInfo.*` fields; `ModNPC.SpawnChance` ("Called in single player or on the server only"), `ModNPC.OnHitPlayer` ("Called on the local client only"), the `ModNPC` hook inventory.
- `AGENTS.md`, `Sources/Modules/Yggdrasil/AGENTS.md` (read) — placement, resource, build/verification, subworld rules.

### Secondary (MEDIUM confidence)
- `.planning/ROADMAP.md` §Phase 4 — goal, mode, dependency, success criteria.
- `.planning/REQUIREMENTS.md` — BIO-01/02/03, BIO-06, QUAL-01/03/04, biology-drop classification rule.
- `.planning/STATE.md` — Phase 3 residue and the deferred UAT batch.
- `.planning/PROJECT.md` — design-status synchronization rules.

### Tertiary (LOW confidence)
- English class names, `aiStyle` choices, conservative spawn weights/drop denominators, and the variant strategy — all marked `[ASSUMED]` (A1–A9).

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — no new dependencies; every version read from committed project files this session.
- Architecture: HIGH — the subworld spawn system, layer predicate, NPC precedents, loot and art-fallback patterns were read directly from source this session.
- Creature design data: HIGH — 18 of 21 rows transcribed verbatim from the committed snapshot; the 3 empty rows are decisive (identity shells).
- Drop availability: HIGH — the implemented/absent split was verified against `01-INVENTORY.json` and the working tree.
- Shell-vs-full split for the complex BIO-02/Valley group: MEDIUM — the design is complete enough to implement, but D-46/D-47 leave the boundary to the executor.
- Pitfalls: MEDIUM–HIGH — engine contracts verified from `tModLoader.xml`; leakage, compile-break and missing-texture pitfalls are structural.
- Open decisions (CaterpillarJuice scope, variant strategy, gate placement, projectile scope, mini-boss depth): LOW — flagged for a checkpoint.

**Research date:** 2026-09-15
**Valid until:** 2026-10-15 (30 days; the design snapshot and tML API are stable, and the Phase 3 precedents are frozen)
