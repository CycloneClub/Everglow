# Phase 4: Remaining Ordinary Monsters - Context

**Gathered:** 2026-09-15
**Status:** Ready for planning

<domain>
## Phase Boundary

Implement the **remaining ordinary (non-boss) Kelp Curtain creatures** — those without repository art — across the three designed regions, integrating their behavior against the item drops already classified and implemented in Phases 1–2.

Scope source: `03-BIOLOGY.json` records **23 `phase:4` rows**. This phase implements **21** of them; the two `out of phase` hardmode rows (枯萎之种 / 枯木人卫士) stay deferred under V2-HARD-01. Breakdown: 13 Death Jade Lake, 3 Spiny Moss Court, 5 Valley of Lush and Moist.

This phase owns ordinary-creature code (full implementations and identity shells), their recorded blockers, and the reconciliation of the Phase 4 rows in `03-BIOLOGY.json`/`.md`. It does **not** implement item scope (drops come from Phases 1–2), localization (D-20), terrain/regions (Phases 5–6), bosses/special encounters (Phase 7), hardmode-deferred designs, or final design-status synchronization (Phase 8).

</domain>

<decisions>
## Implementation Decisions

### Scope & Shell Strategy
- **D-44:** Phase 4 scope is the **21** ordinary creatures = the 23 `phase:4` rows minus the two `out of phase` hardmode rows (`bio-out-of-phase-withered-seed` 枯萎之种, `bio-out-of-phase-withered-tree-guardian` 枯木人卫士), which stay deferred under V2-HARD-01. — **Reversibility:** costly — the in-scope set defines the phase's row count and every later reconciliation; changing it forces re-reconciliation of the matrix and Phase 8.
- **D-45:** In-scope creatures with **no defined behavior** (design row carries only a name/texture, e.g. 荧光水螅, 巨型虎虾, 炮弹藤壶) get an **identity-only shell class** (loadable `ModNPC`, documented stats, `LocalizationCategory`, `Commons.ModAsset.White_Mod` fallback) plus a precise blocker — never invent behavior (extends D-29/D-18).
- **D-46:** Creatures depending on **unimplemented systems** — Spiny Moss Court morale/command (枯木活化士兵, 王庭号令者) and Valley disguised-hazard mechanics (阿萨辛覆盘子, 蛇行苔) — implement only the independently-completable part and record a precise blocker naming the missing system and affected files (extends D-30). Do **not** implement those systems here.
- **D-47:** Complex creatures (BIO-02 group: 枯木活化士兵, 王庭号令者, 布罗迪蝇蜓) are judged **case by case, safety-first**: full implementation when the design row is complete enough, otherwise identity shell + blocker. Each decision is recorded in `04-DEVIATIONS.md`.

### Missing-Art Handling
- **D-48:** Phase 4 creatures have **no repository art**. Implement the code first using the shared fallback `Commons.ModAsset.White_Mod` and record a precise "approved texture (贴图) missing from repository" blocker per entry. Never create placeholder art; never modify a binary/art asset (extends D-13).
- **D-49:** When approved art arrives, the migration is adding `<Class>.png` beside the `.cs` plus removing the artwork blocker — not a class rework (extends D-14).
- **D-50:** The phase gate (`scripts/check-biology.ps1`, extended for the Phase 4 rows) must assert no new class references a non-existent texture path (a missing texture can abort mod loading), and that every in-scope row carries a texture/artwork blocker.
- **D-51:** This phase creates or modifies **no** `.png`/`.obj`/`.xnb` or other binary art asset (AGENTS.md hard constraint); needed art is listed as a blocker only.

### Spawn & Terrain Dependency
- **D-52:** Spawning uses the **layer-level** server-safe predicate `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` (consume the Phase 3 helper; do NOT use `IsBiomeActive` in `SpawnChance`) plus `SubworldSystem.IsActive<YggdrasilWorld>()`. **Region-level refinement** (森雨幽谷 / 刺苔庭园 / 亡碧湖 sub-biome or tile predicates) is recorded as a precise blocker for Phases 5–6 (extends D-30).
- **D-53:** Spawn/behavior must **not hard-depend on terrain tiles/blocks that do not yet exist** (Phase 5–6). Use existing biome/depth/liquid/time predicates; terrain-specific conditions become blockers.
- **D-54:** Spawn conditions (region/depth/water/time) follow the design row; where the design supplies no weight/condition, use conservative defaults recorded in `04-DEVIATIONS.md` (extends D-34).
- **D-55:** Subworld spawn/behavior is server-authoritative; spawn predicates must be server-safe (never `Main.screenPosition`, never `Main.LocalPlayer`); client-only graphics/VFX guarded by `!Main.dedServ` (extends D-32/D-35).

### Drop Wiring
- **D-56:** Drops use direct drop tables (`NPCLoot()` / `ItemDropRule`) with explicit chance and quantity, consistent with repository NPC precedents (extends D-36).
- **D-57:** Drops reference **only items already implemented in Phases 1–2** (`ModContent.ItemType<...>`, e.g. `ThornTurtleShell`, `GuppyShell`, `ArmOfGiantTree`, `HardenedWitherbarkHeart`, `RadialCarapace`, `Photophore`, `ActivatedDogStaff`, `MeatLantern`). No new item scope (extends D-37).
- **D-58:** When a designed drop is not implemented (毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 飞棍毛发, 枯木碎块, …), leave that rule out (empty/partial loot table allowed) and record a precise blocker naming the affected item — the mod must still build and load (extends D-39/D-43).
- **D-59:** Drop probability/quantity follows the design row (including guaranteed/percentage rules); where the design is silent, use conservative defaults recorded in `04-DEVIATIONS.md` (extends D-38).

### Carried Forward (confirmed)
- **D-60 [informational]:** **D-20** localization out of scope every phase; **D-21** in-client runtime verification required in addition to offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0`; **D-22** marking rules (`code_complete=true` / `artwork_complete=false` → row unchecked/no colour); **D-23** code-wins-unless-yellow; **D-41/D-42** the Phase 3/4 tranche rule (repository art). *(Restatement of already-cited policy — informational.)*

### the agent's Discretion
- NPC folder layout / class naming under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/`, and which existing NPC to mirror.
- Exact shell-class contents and blocker wording.
- Which vanilla `aiStyle` fits a creature and where a small custom `AI()` is warranted.
- Whether a creature's projectile/buff/VFX class is added now or recorded as an effect blocker.
- Exact conservative default spawn weights / drop chances where the design is silent.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Authoritative Design Source (external, via lark-cli)
- `.planning/PROJECT.md` § "Feishu Design Source" — biology design `Jp5ndsvNBoCpljxq1eGc9S7vnfe`; `--doc-format xml --detail full`. (Read-only: committed snapshot only, per D-25.)
- `.planning/PROJECT.md` § "Design Status Synchronization" — green/yellow/unchecked colour rules, row-as-status-unit.

### Planning Artifacts
- `.planning/ROADMAP.md` § "Phase 4: Remaining Ordinary Monsters" — goal, `**Mode:** mvp`, dependency, success criteria, verification needs.
- `.planning/REQUIREMENTS.md` — BIO-01, BIO-02, BIO-03 (this phase), BIO-06, QUAL-01/03/04; biology-drop classification rule.
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — the **23-row Phase 4 set** (minus the 2 hardmode rows), with per-row status/blockers.
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — human-readable mirror (31 rows).
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — ledger and blocker conventions to mirror in `04-DEVIATIONS.md`.
- `.planning/phases/03-completed-art-ordinary-monsters/03-RESEARCH.md`, `03-VALIDATION.md` — Phase 3 research and validation conventions.
- `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` — the committed biology snapshot.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` / `01-INVENTORY.md` — item/drop source of truth.
- `.planning/phases/01-item-inventory-completed-art-items/01-CONTEXT.md` — decisions D-01…D-12.
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-CONTEXT.md` — decisions D-13…D-23.
- `.planning/phases/03-completed-art-ordinary-monsters/03-CONTEXT.md` — decisions D-24…D-43.

### Repository Rules and Conventions
- `AGENTS.md` — module placement, resource rules, no-placeholder/no-binary modification, build/verification, localization.
- `Sources/Modules/Yggdrasil/AGENTS.md` — module-specific conventions.
- `Sources/Directory.Build.props` and `Sources/Modules/Directory.Build.props` — active modules, `ModuleName`/`PathPrefix`, global usings.
- `.planning/codebase/STRUCTURE.md`, `CONVENTIONS.md`, `INTEGRATIONS.md`, `ARCHITECTURE.md`, `CONCERNS.md`.
- `Documents/源代码编译流程.md` — build process and the lingering-MSBuild-process cleanup note.

### Existing Implementation
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/` — `MossyThornTurtle.cs`, `GuppyConch.cs`, `VerdantRods.cs`, `GiantDandelion.cs`, `RiverSlug.cs` (Phase 3 precedents) and `VampireMat/*`.
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/` — Phase 3 enemy-projectile precedents.
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs` — `IsKelpCurtainLayer(Player)` server-safe spawn predicate (D-52).
- `Sources/Modules/Yggdrasil/Common/NPCSpawnManager.cs` — `RegisterNPC` / `EditSpawnPool` precedent.
- `Sources/Modules/Yggdrasil/KelpCurtain/Biomes/DeathJadeLakeBiome.cs` — server-safe region predicate precedent (`player.Center`).
- `Sources/Everglow.Function/Templates` — base patterns for new content types.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- Phase 3 creature classes — nearest analogs for spawn gating, `aiStyle`/custom `AI()`, drop wiring, and the `White_Mod` shell shape.
- `KelpCurtainBiome.IsKelpCurtainLayer(Player)` — the server-safe layer spawn predicate (D-52).
- `Commons.ModAsset.White_Mod` — shared fallback texture for art-missing classes (D-48).
- `LocalizationUtils.Categories.*` — required `LocalizationCategory` overrides.
- Source-generated `ModAsset` members — asset access without handwritten paths.

### Established Patterns
- Namespace/placement `Everglow.Yggdrasil.KelpCurtain.NPCs.<Name>`; class names mirror asset basenames; assets beside the `.cs`.
- No manual `AddContent`; automatic content discovery.
- Tabs, LF, UTF-8 no BOM, Allman braces, file-scoped namespaces; enum-wrapped `NPC.ai[]` indexing.
- Structure/status gates as ASCII PowerShell scripts (`check-*.ps1`) reading JSON via `[IO.File]::ReadAllText`.

### Integration Points
- `03-BIOLOGY.json` (23 Phase 4 rows) → Phase 4 reconciliation → extended phase gate → Phase 8.
- Creature `NPCLoot()` tables → Phase 1–2 implemented items via `ModContent.ItemType<...>` (D-56/D-57).
- `SpawnChance` → `SubworldSystem.IsActive<YggdrasilWorld>()` + `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` (D-52/D-55).
- Offline gates + `dotnet build /p:Configuration=Release /p:WarningLevel=0`; runtime verification via the tModLoader client (D-21).

</code_context>

<specifics>
## Specific Ideas

- The Phase 4 set is enumerated in `03-BIOLOGY.json`: 13 Death Jade Lake (荧光水螅, 巨型虎虾, 水黾, 剧毒蟾蜍, 幽光蝾螈, 装甲虾, 爆弹水母, 帆鳍鳢, 放射虫, 覆藻章鱼, 大型覆藻章鱼, 碧灵鮟鱇, 炮弹藤壶), 3 Spiny Moss Court (枯木活化士兵, 王庭号令者, 布罗迪蝇蜓), 5 Valley of Lush and Moist (红针洋辣子, 阿萨辛覆盘子, 蛇行苔, 小格普螺, 大型荆棘苔龟).
- Empty creature sections in the design (荧光水螅, 巨型虎虾, 炮弹藤壶; 吸血魔毯 is the Phase 7 boss) imply identity shells under D-45.
- Ordinary-creature drops are item scope and were classified/implemented in Phases 1–2; Phase 4 consumes them.

</specifics>

<deferred>
## Deferred Ideas

- **Hardmode-deferred designs** — 枯萎之种, 枯木人卫士 (V2-HARD-01).
- **Localization (all phases)** — excluded by user directive; ITEM-07 deferred (D-20).
- **Region-level spawn predicates** (森雨幽谷 / 刺苔庭园) — Phases 5–6 terrain work (D-52).
- **Boss / special-encounter creatures** (Klein Snake, Giant Winged Dragon, 吸血魔毯/VampireMat) — Phase 7.
- **Unimplemented systems** (morale/command, disguised hazards, capture items) — precise blockers only (D-46/D-57).
- **Absent drop materials** (毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 飞棍毛发, 枯木碎块) — item scope; recorded blockers (D-58).
- **Runtime verification** — requires a live tModLoader client; batched with the deferred Phase 2/3 UAT (D-21).

</deferred>

---

*Phase: 4-Remaining Ordinary Monsters*
*Context gathered: 2026-09-15*
