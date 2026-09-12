# Phase 1: Item Inventory & Completed-Art Items - Context

**Gathered:** 2026-09-11
**Status:** Ready for planning

<domain>
## Phase Boundary

Reconcile the three authoritative Feishu second-layer design documents into an auditable item/drop inventory, classify each entry's artwork/texture completeness, classify the five terrain source labels, and implement (or bring to design parity) the completed-art item tranche without introducing placeholder assets.

This phase owns the source/inventory reconciliation record and the completed-art item/drop tranche. It does NOT implement unfinished-art items (Phase 2), creatures (Phases 3–4), terrain (Phases 5–6), bosses (Phase 7), or final design-status synchronization (Phase 8).

</domain>

<decisions>
## Implementation Decisions

### Inventory Artifact
- **D-01:** The inventory is delivered as a human-readable Markdown matrix plus a machine-readable JSON sidecar (`01-INVENTORY.md` + `01-INVENTORY.json`); the JSON is the source of truth for status fields and block-ID mapping.
- **D-02:** Matrix rows map one-to-one to Feishu design rows (the checkbox status unit), grouped by category (weapons, armor, materials, biology drops, etc.), then ordered by region.
- **D-03:** Both files live in the phase directory: `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.{md,json}`.
- **D-04:** The five terrain labels get a dedicated "Source Label Reconciliation" table in `01-INVENTORY.md` plus a `labels` array in the JSON, each classified as region / nested area / structure / transition / alias with rationale and affected entries.

### Artwork-Completeness Determination
- **D-05:** Feishu artwork/texture checkbox state is authoritative for whether artwork is complete; repository asset presence is corroborating evidence only.
- **D-06:** On conflict, Feishu wins: Feishu-complete but no repo asset → blocked (not Phase 1 scope); repo asset present but either Feishu checkbox unchecked → unchecked (no status colour) with the conflict recorded as a blocker requiring human resolution before implementation. A status colour is applied only when BOTH Feishu checkboxes are complete (D-07; PROJECT.md "Design Status Synchronization").
- **D-07:** Each JSON row carries separate `artwork_complete` and `code_complete` booleans plus a composite `status` (green / yellow / unchecked), mirroring the two Feishu checkboxes. The Markdown matrix shows both sub-columns and the composite state.

### Feishu Source Fetch
- **D-08:** Fetch all three documents once at phase start with `lark-cli docs +fetch --doc-format xml --detail full` and treat the snapshot as the reconciliation basis (reproducible).
- **D-09:** Raw XML snapshots are committed under `.planning/phases/01-item-inventory-completed-art-items/evidence/`, and the JSON sidecar references the relevant `block_id`s for later Phase 8 status writes.

### Existing-Code Baseline
- **D-10:** Reconcile each existing `Sources/Modules/Yggdrasil/KelpCurtain/Items/` implementation against the design; trust existing code that already matches, add only missing entries, and avoid wholesale rewrites. — **Reversibility:** costly — undoing a wholesale-rewrite decision would touch many item classes, localization keys, and compatibility-sensitive internal names.
- **D-11:** An entry with completed code but missing artwork STILL counts toward phase completion. It is recorded as `code_complete=true` / `artwork_complete=false`, the Feishu row stays uncolored, and artwork remains a tracked blocker. Do not wait for artwork before counting code completion. — **Reversibility:** costly — this changes phase-acceptance semantics and the Phase 1/Phase 2 boundary; undoing it requires re-auditing which entries count as complete and revising the roadmap's "completed-art" framing.
- **D-12:** For entries whose artwork is complete, reconcile and fix design deviations (values, recipes, effects, set bonuses) within Phase 1; deviations on art-incomplete entries are queued to Phase 2. — **Reversibility:** costly — deciding later to defer or pull these fixes forward would require re-opening accepted Phase 1 items.

### the agent's Discretion
- Exact JSON sidecar schema field names/nesting beyond the required `artwork_complete`, `code_complete`, `status`, and block-ID fields.
- Evidence file naming/layout under `evidence/`.
- Which existing item base classes/templates to reuse for new entries (follow adjacent KelpCurtain patterns and `Everglow.Function/Templates`).

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Authoritative Design Source (external, via lark-cli)
- `Sources/Everglow` project doc `PROJECT.md` § "Feishu Design Source" — biology `Jp5ndsvNBoCpljxq1eGc9S7vnfe`, item `FSlSdNlE1owUaAxXyFBcRSaznOe`, terrain `OCK2di9Zvoa8Blx3bfyczI0xn0b`; use `--doc-format xml --detail full`.
- `.planning/PROJECT.md` § "Design Status Synchronization" — exact green/yellow/unchecked color rules and row-as-status-unit rule.

### Planning Artifacts
- `.planning/ROADMAP.md` § "Phase 1" and § "Design-source acceptance path" — phase goal, success criteria, and label-reconciliation prerequisite.
- `.planning/REQUIREMENTS.md` — QUAL-05 (Phase 1) plus the item requirements (ITEM-01…04) this phase feeds; biology-drop classification rule.
- `.planning/STATE.md` § "Blockers/Concerns" — five terrain labels and no-placeholder constraints.

### Repository Rules and Conventions
- `AGENTS.md` — module placement, resource/localization rules, no-placeholder/no-binary-modification, build/verification requirements.
- `Sources/Directory.Build.props` and `Sources/Modules/Directory.Build.props` — active modules, `ModuleName`/`PathPrefix`, global usings.
- `.planning/codebase/STRUCTURE.md`, `.planning/codebase/CONVENTIONS.md`, `.planning/codebase/INTEGRATIONS.md` — placement, naming/format, and integration reference.
- `Documents/源代码编译流程.md` — `dotnet build` / resource-packing workflow.

### Existing Implementation
- `Sources/Modules/Yggdrasil/KelpCurtain/` — existing Items, NPCs, Tiles, Projectiles, localization-adjacent content to reconcile against.
- `Sources/Everglow/Localization/en-US/` and `Sources/Everglow/Localization/zh-Hans/` — localization targets (additive keys only).

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/` subfolders (Weapons, Armors/*, Materials, Accessories, Ammos, Placeables, Tools, etc.) — existing item classes to reconcile and extend.
- `Everglow.Function/Templates` — base patterns for new content types.
- Source-generated `ModAsset` members + `Commons.ModAsset` — asset path access without handwritten paths.
- In-game localization exporter (`OutputLocalizationHjsonItem`) — generate missing HJSON keys rather than hand-writing classification keys.

### Established Patterns
- Content namespace/placement `Everglow.Yggdrasil.KelpCurtain.*`; assets beside their `.cs` files.
- No manual `AddContent` for ordinary content — automatic discovery.
- File-scoped namespaces, tab indent, LF/UTF-8-no-BOM, Allman braces; no unrelated reformatting.
- Never modify binary/art assets; no placeholder art.

### Integration Points
- Inventory artifacts feed Phase 2 (remaining items), Phases 3–4 (drops), and Phase 8 (final Feishu status sync).
- `block_id`s captured from XML fetches are consumed by Phase 8's lark-doc update workflow.
- Item implementation connects to localization directories and the module build/resource pipeline.

</code_context>

<specifics>
## Specific Ideas

- The five labels to classify: Death Jade Lake, Spiny Moss Court, Valley of Lush and Moist (Biology), plus Green Tundra and Town of Decaying Wood (Terrain only).
- One-time snapshot at phase start; no per-item re-fetching.

</specifics>

<deferred>
## Deferred Ideas

None — discussion stayed within phase scope.

</deferred>

---

*Phase: 1-Item Inventory & Completed-Art Items*
*Context gathered: 2026-09-11*
