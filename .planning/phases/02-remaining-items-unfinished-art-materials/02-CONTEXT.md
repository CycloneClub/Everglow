# Phase 2: Remaining Items & Unfinished-Art Materials - Context

**Gathered:** 2026-09-14
**Status:** Ready for planning

<domain>
## Phase Boundary

Implement the remaining **non-boss** item scope whose Feishu design rows exist but whose artwork/texture is not approved yet — weapons, summon weapons, ranged/melee/magic utility, materials, and biology-design drops — in dependency order, **without placeholder art** and **without localization**.

This phase owns item code (full implementations and identity shell classes) and their recorded blockers. It does NOT implement localization (excluded from all phases by user directive), creatures (Phases 3–4), terrain (Phases 5–6), bosses/special encounters (Phase 7), or final design-status synchronization (Phase 8).

</domain>

<decisions>
## Implementation Decisions

### Missing-Art Handling
- **D-13:** For entries with no approved texture, implement the code first and use the shared fallback texture `Commons.ModAsset.White_Mod` (the Phase 1 precedent — `ForestBreath`/`WitheredMask`/`QuetzalsWish`), record a precise "approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending" blocker, and leave the Feishu row unchecked (no colour). Never create placeholder art and never modify a binary/art asset. — **Reversibility:** costly — the content class + internal name become compatibility-sensitive once committed, and replacing the fallback with real art later touches each class and its asset references.
- **D-14:** When approved art arrives later, the migration is a texture addition beside the `.cs` (source-generated `ModAsset`) plus removing the artwork blocker — not a class rework. The item identity must not depend on the fallback.

### Allocations & Scope
- **D-15:** The three `阵法修复材料` placeholder rows (`item-weapons.misc-a`, `-b`, `-c-名字要普通`; design names A/B/C, no checkbox) stay `deferred` and are NOT implemented in Phase 2.
- **D-16:** `巨翼龙面具` is reallocated to **Phase 7 (ITEM-06)** — it is a Giant Winged Dragon reward; apply `phase=7` and append `ITEM-06` during planning/execution. The Klein Snake series (`碧绿玉髓扇`, `龙骨猎枪`, `碧玉弯刀`, `魂蛇手杖`) is already Phase 7 (ITEM-05) and unchanged.
- **D-17:** Implementation order is **dependency order** — drops/materials first (e.g. `CrimsonMoonSap`, `JadeLakeRedAlgae_Item` for the 红月水藻 set), then the finished equipment that consumes them, so the intended progression is never bypassed (ITEM-01…04).

### Code Depth (full implementation vs shell)
- **D-18:** Entries **with** a defined design row / checkbox get a **full implementation** (documented recipe/value/effect, `LocalizationCategory`, gates). Entries with **no detailed description and no checkbox** get an **identity-only shell class** (minimal `ModItem`, no gameplay behaviour, fallback texture + `LocalizationCategory`), so the type exists without inventing undefined behaviour.
  - Full implementation (9): 肌腱巨弓, 限制机, 腥臭的诱饵, 巨石弹射装置, 红月水藻头饰, 红月水藻面具, 红月水藻板甲, 红月水藻护胫, 灵蛇玉卵.
  - Shell class (12): 竹节步符, 竹制武器, 竹簪子, 桃枝护符, 桃花纸鸢（风筝）, 熊猫宠物, 若干酒类, 弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台, 荧光水螅召唤杖.
- **D-19:** Items that imply an unimplemented system (弟子剑/弟子时装/技能竹简/区域放置物品制作台 → a disciple/skill/regional-crafting system) are shell classes only; record a precise blocker naming the missing system. Do NOT implement the system in this phase.

### Localization
- **D-20:** Localization is **out of scope for every phase** — the goal is code design. Do not run the in-game exporter, do not hand-edit HJSON, and treat ITEM-07 as deferred from the current milestone scope. Localization gaps are recorded in the deferred ledger only. — **Reversibility:** costly — excluding a roadmap requirement changes milestone acceptance and the Phase 8 synchronization scope.

### Verification
- **D-21:** Phase 2 requires **in-client runtime verification** of representative items (obtainable/craftable/usable with the documented effect) in addition to offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0` (exit 0, no `error CS`). Runtime verification needs a running tModLoader client.
- **D-22:** Item marking follows Phase 1 D-11: code-complete + art-incomplete → `code_complete=true` / `artwork_complete=false`, Feishu row unchecked (no colour), art tracked as a blocker. A row only receives a status colour when both checkboxes are complete (D-05/D-07).
- **D-23:** Carried-forward project rule: when implemented code conflicts with the design, follow the code unless the Feishu row is marked yellow with a corresponding explanation. Under this rule Phase 1's numeric design-parity edits were reverted to the pre-Phase-1 code baseline `5c025ff7e` (2026-09-14, `revert(260914-kl8)`); Phase 2 must apply the same rule to every conflict it meets — prefer the implemented code and correct the design, unless the Feishu row is yellow with a corresponding explanation. — **Reversibility:** costly — the ruling is now the project's conflict policy and is recorded in `PROJECT.md` "Design Status Synchronization" and "Constraints".

### the agent's Discretion
- Exact shell-class contents (minimal `SetDefaults` values, folder placement) and which adjacent existing item class to mirror.
- Blocker wording for missing-art and missing-system cases.
- Whether a full-implementation item needs a new projectile/buff/VFX class now or records a precise effect blocker for a later phase (Phase 1 precedent: `QuetzalsWish`).

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Authoritative Design Source (external, via lark-cli)
- `.planning/PROJECT.md` § "Feishu Design Source" — biology `Jp5ndsvNBoCpljxq1eGc9S7vnfe`, item `FSlSdNlE1owUaAxXyFBcRSaznOe`, terrain `OCK2di9Zvoa8Blx3bfyczI0xn0b`; fetch with `--doc-format xml --detail full`.
- `.planning/PROJECT.md` § "Design Status Synchronization" — exact green/yellow/unchecked colour rules, row-as-status-unit.

### Planning Artifacts
- `.planning/ROADMAP.md` § "Phase 2" — phase goal, `**Mode:** mvp`, allocation note, requirements, success criteria.
- `.planning/REQUIREMENTS.md` — ITEM-01…ITEM-04 (this phase); ITEM-07 (localization, now excluded); biology-drop classification rule.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` — source of truth for entry `phase`, `artwork_complete`, `code_complete`, `status`, `blockers`, `feishu.block_id`s, and `deferred`.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` — human-readable mirror (103 rows).
- `.planning/phases/01-item-inventory-completed-art-items/01-CONTEXT.md` — Phase 1 decisions D-01…D-12 carried forward (D-05/06/07, D-10, D-11, D-12).
- `.planning/phases/01-item-inventory-completed-art-items/01-CHANGE-RECORD.md` — the exact Phase 1 change set under cross-team review.
- `.planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md` — deviation and deferred-localization ledger conventions.

### Repository Rules and Conventions
- `AGENTS.md` — module placement, resource rules, no-placeholder/no-binary modification, build/verification requirements, localization rules.
- `Sources/Modules/Yggdrasil/AGENTS.md` — module-specific conventions.
- `Sources/Directory.Build.props` and `Sources/Modules/Directory.Build.props` — active modules, `ModuleName`/`PathPrefix`, global usings.
- `.planning/codebase/STRUCTURE.md`, `CONVENTIONS.md`, `INTEGRATIONS.md` — placement, naming/format, integration reference.

### Existing Implementation
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/` — existing item classes and folder layout to mirror.
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs` — per-player state precedent (Phase 1 ArmOfGiantTree charge).
- `Sources/Modules/Yggdrasil/Netcode/` — `ModIns.PacketResolver` packet precedent.
- `Sources/Everglow.Function/Templates` — base patterns for new content types.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `Commons.ModAsset.White_Mod` — shared fallback texture for art-missing classes (Phase 1 precedent).
- Existing KelpCurtain item classes (`Weapons/`, `Weapons/UnderwaterTreasury/`, `Weapons/RedAlgae*`, `Materials/`, `Accessories/`) — nearest analogs for stats, recipes, and folders.
- `LocalizationUtils.Categories.*` — required `LocalizationCategory` overrides.
- Source-generated `ModAsset` members — asset path access without handwritten paths.

### Established Patterns
- Namespace/placement `Everglow.Yggdrasil.KelpCurtain.Items.<Area>`; assets beside the `.cs`.
- No manual `AddContent`; automatic content discovery.
- Tabs, LF, UTF-8 no BOM, Allman braces, file-scoped namespaces.
- Structure gates as ASCII PowerShell scripts (`check-*.ps1`) reading the inventory via `[IO.File]::ReadAllText`.

### Integration Points
- `01-INVENTORY.json` drives per-entry implementation status and blockers; a Phase 2 gate should verify shell-vs-full classification and the no-placeholder rule.
- Offline gates + `dotnet build /p:Configuration=Release /p:WarningLevel=0`; runtime verification via the client.
- Recipe/value/effect decisions must not bypass the Phase 1 progression ledger (ITEM-01…04).

</code_context>

<specifics>
## Specific Ideas

- 红月水藻套 (头饰/面具/板甲/护胫) depends on `CrimsonMoonSap` and `JadeLakeRedAlgae_Item` — implement materials first.
- The 3 `阵法修复材料` rows are the design team's own placeholders (names A/B/C) — not real content yet.
- Shell classes exist so the type/name is reserved; behaviour is intentionally absent until the design defines it.

</specifics>

<deferred>
## Deferred Ideas

- **Localization (all phases)** — excluded by user directive; ITEM-07 deferred. Record in the deferred ledger only.
- **3 × `阵法修复材料` placeholders** — wait for a real design entry/name.
- **Runtime verification** — requires a live tModLoader client; to be scheduled when convenient.
- **System-dependent items** (弟子剑/弟子时装/技能竹简/区域放置物品制作台) — the disciple/skill/regional-crafting system belongs to a later phase.

</deferred>

---

*Phase: 2-Remaining Items & Unfinished-Art Materials*
*Context gathered: 2026-09-14*
