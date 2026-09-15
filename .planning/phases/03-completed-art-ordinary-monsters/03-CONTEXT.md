# Phase 3: Completed-Art Ordinary Monsters - Context

**Gathered:** 2026-09-15
**Status:** Ready for planning

<domain>
## Phase Boundary

Implement ordinary (non-boss) Kelp Curtain creature **behavior** for design entries whose textures are complete — spawn contexts, water/land behavior, hostility, status effects, capture rules, combat states, variants, and neutral behavior — and produce a completed-art biology matrix with per-entry source comparison.

This phase owns ordinary-creature code (full implementations and identity shells) and their recorded blockers, plus the `03-BIOLOGY.json`/`.md` matrix and `scripts/check-biology.ps1` gate. It does **not** implement item scope (drops are supplied by Phases 1–2), localization (excluded from all phases), unfinished-art creatures (Phase 4), terrain/generation (Phases 5–6), bosses/special encounters (Phase 7), or final design-status synchronization (Phase 8).

</domain>

<decisions>
## Implementation Decisions

### Source Comparison & Biology Matrix
- **D-24:** A new `03-BIOLOGY.json` is the machine source of truth for Phase 3–4 creature status (one row per creature: id, region, texture-complete, code-complete, `status`, `blockers`, deferred), mirrored by a human-readable `03-BIOLOGY.md`, following the Phase 1 `01-INVENTORY.json`/`.md` convention. — **Reversibility:** costly — Phase 4 and Phase 8 consume this artifact; changing its id/schema later forces re-reconciliation of both phases.
- **D-25:** The matrix is derived only from the **already-committed** `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` snapshot. Do **not** re-fetch the Feishu document and do **not** mutate the design source.
- **D-26:** ~~"Texture-complete" (i.e. belongs to Phase 3, not Phase 4) is decided **solely** by the texture cell / checkbox of the biology design row, matching the Phase 1 `artwork_complete` rule. Repository `.png` presence is not the criterion.~~ **SUPERSEDED by D-41** (2026-09-15): research proved the biology snapshot has no per-creature texture checkbox or cell, so this rule was unappliable.
- **D-27:** A new 100% ASCII PowerShell 5.1 `scripts/check-biology.ps1` gate validates coverage count, per-row classification, per-row blocker presence, and a no-placeholder-art guard, mirroring the Phase 1/2 `check-*.ps1` conventions (read JSON via `[IO.File]::ReadAllText`; no `git ls-files` dependency for newly created files). It must run green before and after each plan wave.

### Behavior Implementation Depth
- **D-28:** Ordinary creatures whose design row is complete (texture checkbox on, behavior described) get a **full implementation**: spawn, AI/movement, hostility relationships, status effects, drop wiring, hit/attack behavior. — **Reversibility:** costly — creature internal names and AI parameters become compatibility-sensitive once committed.
- **D-29:** Creatures that are texture-complete but have **no defined behavior** get an **identity-only shell class** (loadable `ModNPC` with documented stats/`LocalizationCategory` and the D-13-style fallback texture policy) plus a precise blocker — never invent behavior (extends the D-18 spirit).
- **D-30:** For creatures depending on **unimplemented systems** (capture rules, status-effect systems, variants, morale) implement only the independently-completable part and record a precise blocker naming the missing system and the affected files (the Phase 1/2 "outside this plan's file scope" precedent). Do **not** implement those systems in this phase.
- **D-31:** AI is implemented with vanilla `aiStyle` where it fits, plus locally-written `AI()` only when necessary, reusing existing KelpCurtain/Yggdrasil NPC precedents (`NPCs/RiverSlug.cs`, `NPCs/VampireMat/*`). Do **not** introduce a new generic creature base class/interface.

### Spawn & Scene Integration
- **D-32:** Every ordinary creature spawns **only inside the Yggdrasil Subworld** in its designed Kelp Curtain region, isolated so it never leaks into the main world (BIO-06 hard constraint).
- **D-33:** Spawning reuses the repository's existing Yggdrasil Subworld / spawn-system precedent rather than introducing a parallel spawn system; the exact hook is confirmed during research.
- **D-34:** Spawn conditions (region/depth/water/time) follow the design row; where the design supplies no weight/condition, use conservative defaults and record them as assumptions in `03-DEVIATIONS.md`.
- **D-35:** Subworld spawn/behavior is server-authoritative; client-only graphics/VFX work is guarded by `!Main.dedServ`, consistent with Phases 1–2.

### Drop Wiring
- **D-36:** Drops use direct drop tables (`NPCLoot()` / `ItemDropRule`) with explicit chance and quantity, consistent with existing repository NPC precedents.
- **D-37:** Drops reference **only items already implemented in Phases 1–2** (`ModContent.ItemType<...>`). This phase introduces **no new item scope**; any missing drop is a recorded blocker.
- **D-38:** Drop probability/quantity follows the design row; where absent, use conservative defaults recorded in `03-DEVIATIONS.md`.
- **D-39:** When a designed drop is not yet implemented, leave that entry out of the drop table (partial/empty is allowed) and record a precise blocker with the affected item — the mod must still build and load.

### Research-Resolved Checkpoints (2026-09-15)
- **D-41:** "Texture-complete" — the Phase 3/4 split criterion — is **the repository already containing the creature's approved art** (a corresponding `.png` in the KelpCurtain creature asset tree), **not** a design-row checkbox or inline design `<img>`. Research proved `evidence/biology.xml` carries no per-creature texture checkbox (29 tables, 2 with name headers, both Giant Winged Dragon item tables; 22 `<checkbox>` total, 0 on creature rows) and that the only per-creature artwork marker is an inline `<img>` present for just 7 Death Jade Lake creatures. **Reversibility:** costly — this rule defines both Phase 3's and Phase 4's membership and the `03-BIOLOGY.json` classification, so changing it later forces re-reconciliation of both phases.
- **D-42:** Creatures that have repository art but **no** design artwork marker (e.g. 荆棘苔龟, 格普螺, 叶飞棍) are **in Phase 3**. The repository-art-vs-design-art discrepancy is recorded in `03-DEVIATIONS.md` for later designer confirmation, not treated as out-of-scope.
- **D-43:** Research found Phase 3's artwork-bearing creatures drop only `毒腺` / `牛黄` / `软体甲壳碎片` / `亡碧膏` (plus one `待定`), **none of which exist as items**, while every implemented biology drop belongs to a non-artwork creature. Therefore Phase 3 loot tables are **empty or partial + precise blockers** (per D-37/D-39); success criterion 2 is satisfied **vacuously** and this is recorded explicitly in `03-DEVIATIONS.md`. **No item scope is promoted into this phase** (rejecting the D-37-contradicting alternative).

### Carried Forward (confirmed)
- **D-40:** Phase 2 decisions remain in force: **D-20** localization is out of scope for every phase (record in the deferred ledger only); **D-21** in-client runtime verification is required in addition to offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0`; **D-22** marking rules (code-complete + art-incomplete → `code_complete=true` / `artwork_complete=false`, row unchecked/no colour); **D-23** when implemented code conflicts with the design, follow the code unless the Feishu row is yellow with a corresponding explanation.

### the agent's Discretion
- NPC folder layout and class naming under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/`, and which adjacent existing NPC to mirror.
- Exact shell-class contents (minimal `SetDefaults` values, stats) and blocker wording.
- Which vanilla `aiStyle` fits a given creature and where a small custom `AI()` is warranted.
- Whether a creature's projectile/buff/VFX class is added now or recorded as a precise effect blocker for a later phase.
- Exact conservative default spawn weights / drop chances where the design is silent.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Authoritative Design Source (external, via lark-cli)
- `.planning/PROJECT.md` § "Feishu Design Source" — biology design `Jp5ndsvNBoCpljxq1eGc9S7vnfe`; fetch with `--doc-format xml --detail full`. (Read-only: Phase 3 uses the committed snapshot, D-25.)
- `.planning/PROJECT.md` § "Design Status Synchronization" — exact green/yellow/unchecked colour rules, row-as-status-unit.

### Planning Artifacts
- `.planning/ROADMAP.md` § "Phase 3: Completed-Art Ordinary Monsters" — goal, `**Mode:** mvp`, dependency, success criteria, verification needs.
- `.planning/REQUIREMENTS.md` — BIO-01/BIO-02/BIO-03 (completed-art tranche is this phase's primary output), BIO-06 (no main-world leakage), QUAL-03/QUAL-04; the biology-drop classification rule.
- `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` — the committed biology source snapshot (Phase 3 data source, D-25).
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` / `01-INVENTORY.md` — item/drop source of truth (drops referenced by Phase 3 creatures).
- `.planning/phases/01-item-inventory-completed-art-items/01-CONTEXT.md` — Phase 1 decisions D-01…D-12 carried forward.
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-CONTEXT.md` — Phase 2 decisions D-13…D-23 (D-20/D-21/D-22/D-23 carry into Phase 3).
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md` — deviation/blocker ledger conventions to mirror in `03-DEVIATIONS.md`.

### Repository Rules and Conventions
- `AGENTS.md` — module placement, resource rules, no-placeholder/no-binary modification, build/verification requirements, localization rules.
- `Sources/Modules/Yggdrasil/AGENTS.md` — module-specific conventions.
- `Sources/Directory.Build.props` and `Sources/Modules/Directory.Build.props` — active modules, `ModuleName`/`PathPrefix`, global usings.
- `.planning/codebase/STRUCTURE.md`, `CONVENTIONS.md`, `INTEGRATIONS.md`, `ARCHITECTURE.md`, `CONCERNS.md` — placement, naming/format, integration, and known-risk reference.

### Existing Implementation
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/` — `RiverSlug.cs` and `VampireMat/*` NPC precedents (AI, spawn, drop shape to mirror).
- `Sources/Modules/Yggdrasil/WorldGeneration/` and the Yggdrasil Subworld system — existing spawn/generation precedent (D-33).
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs` — per-player state precedent.
- `Sources/Modules/Yggdrasil/Netcode/` — `ModIns.PacketResolver` packet precedent.
- `Sources/Everglow.Function/Templates` — base patterns for new content types.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.cs` and `NPCs/VampireMat/*` — nearest in-repo ordinary-creature analogs (stats, AI, hit/loot wiring, summon variants).
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/*` — existing enemy projectile precedents for attack behavior.
- `Commons.ModAsset.White_Mod` — shared fallback texture for any art-missing shell creature (D-13 policy, carried by D-29).
- `LocalizationUtils.Categories.*` — required `LocalizationCategory` overrides.
- Source-generated `ModAsset` members — asset access without handwritten paths.

### Established Patterns
- Namespace/placement `Everglow.Yggdrasil.KelpCurtain.NPCs.<Area>`; assets beside the `.cs`.
- No manual `AddContent`; automatic content discovery.
- Tabs, LF, UTF-8 no BOM, Allman braces, file-scoped namespaces.
- Structure/status gates as ASCII PowerShell scripts (`check-*.ps1`) reading JSON via `[IO.File]::ReadAllText`.

### Integration Points
- `evidence/biology.xml` → `03-BIOLOGY.json` → `scripts/check-biology.ps1` coverage/classification/no-art gate (D-24/D-25/D-26/D-27).
- Creature `NPCLoot()` tables → Phase 1–2 implemented items via `ModContent.ItemType<...>` (D-36/D-37).
- Subworld spawn hooks → dedicated-server authority + `!Main.dedServ` client-graphics guards (D-32/D-33/D-35).
- Offline gates + `dotnet build /p:Configuration=Release /p:WarningLevel=0`; runtime verification via the tModLoader client (D-21).

</code_context>

<specifics>
## Specific Ideas

- The biology source snapshot already exists from Phase 1 (`evidence/biology.xml`), so Phase 3 starts from a committed matrix rather than a fresh fetch (D-25).
- Ordinary-creature drops are item scope and were classified/implemented in Phases 1–2; Phase 3 consumes them, it does not re-open item work.
- The completed-art creatures of BIO-01/BIO-02/BIO-03 are this phase's primary dependency for Phase 4.

</specifics>

<deferred>
## Deferred Ideas

- **Localization (all phases)** — excluded by user directive; ITEM-07 deferred. Record in the deferred ledger only (D-20).
- **Unfinished-art creatures** — belong to Phase 4 (Remaining Ordinary Monsters).
- **Boss / special-encounter creatures** (Klein Snake, Giant Winged Dragon) — Phase 7.
- **Unimplemented systems** implied by creatures (capture rules, status-effect systems, variants, morale) — recorded as precise blockers, not implemented here (D-30).
- **Runtime verification** — requires a live tModLoader client; scheduled when convenient (D-21).

</deferred>

---

*Phase: 3-Completed-Art Ordinary Monsters*
*Context gathered: 2026-09-15*
