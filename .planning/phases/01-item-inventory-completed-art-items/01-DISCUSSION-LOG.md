# Phase 1: Item Inventory & Completed-Art Items - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-09-11
**Phase:** 1-Item Inventory & Completed-Art Items
**Areas discussed:** 清单矩阵的格式 (Inventory artifact), 美术完成的判定标准 (Artwork-completeness gate), 飞书抓取策略 (Feishu fetch strategy), 既有代码基线 (Existing-code baseline)

---

## 清单矩阵的格式 (Inventory Artifact)

| Option | Description | Selected |
|--------|-------------|----------|
| Markdown matrix + JSON sidecar | Human-readable `.md` + machine-readable JSON with internal names, category, artwork status, deps, Feishu block IDs | ✓ |
| Single Markdown matrix | One `.md`; lightest but manual machine checks | |
| Pure JSON/CSV | Machine-first; less consistent with GSD planning artifacts | |
| Split Markdown by category | Separate item/drop/label files | |

| Option | Description | Selected |
|--------|-------------|----------|
| Row = Feishu design row, grouped by category | One row per checkbox unit, category then region order | ✓ |
| Row = implemented item, grouped by region | Implementation-centric view | |
| Dual view | Design rows + mapped internal names | |

| Option | Description | Selected |
|--------|-------------|----------|
| Phase dir `01-INVENTORY.{md,json}` | Follows phase lifecycle | ✓ |
| Project-level `.planning/INVENTORY/` | Cross-phase access | |
| Multiple category files | Clearer responsibilities, more files | |

**User's choice:** Markdown matrix + JSON sidecar; row = Feishu design row grouped by category; stored in the phase directory.
**Notes:** JSON is the source of truth for status fields and block-ID mapping.

---

## 美术完成的判定标准 (Artwork-Completeness Gate)

| Option | Description | Selected |
|--------|-------------|----------|
| Feishu checkbox authoritative, repo asset corroborating | Aligns with PROJECT.md "Feishu is source of truth" | ✓ |
| Repo loadable texture authoritative | Usability-centric | |
| Both must agree | Most conservative | |

| Option | Description | Selected |
|--------|-------------|----------|
| Feishu wins; conflicts yellow + human resolution | Feishu-complete/no asset → blocked; asset present/unchecked → yellow | ✓ |
| Missing repo art always blocked; Feishu-unchecked still implement | Faster, may drift from design state | |
| Either incomplete → Phase 2 | Most conservative | |

| Option | Description | Selected |
|--------|-------------|----------|
| `artwork_complete` + `code_complete` + composite status | Mirrors the two Feishu checkboxes | ✓ |
| Single composite status | Simplest, loses auditability | |
| JSON split, Markdown composite only | Readable, needs sync | |

**User's choice:** Feishu authoritative with repo corroboration; conflicts yellow + human; split status fields plus composite.
**Notes:** Non-placeholder constraint remains.

---

## 飞书抓取策略 (Feishu Fetch Strategy)

| Option | Description | Selected |
|--------|-------------|----------|
| One-time fetch all three at phase start, cached | Reproducible reconciliation basis | ✓ |
| Per-item on-demand fetch | Freshest but slow/inconsistent | |
| Initial snapshot + pre-acceptance re-check | Most rigorous, extra step | |

| Option | Description | Selected |
|--------|-------------|----------|
| Store raw XML in phase `evidence/`, reference block IDs in JSON | Auditable snapshot for Phase 8 writes | ✓ |
| No raw XML; block IDs + summaries only | Lean repo, loses raw context | |
| XML in gitignored temp; commit distilled evidence only | No large files, not reproducible | |

| Option | Description | Selected |
|--------|-------------|----------|
| Dedicated "Source Label Reconciliation" table in inventory + JSON labels array | Co-located with items, auditable | ✓ |
| Separate `01-SOURCE-LABELS.md` | Clear scope, cross-file | |
| Only in blockers/notes | Not auditable enough | |

**User's choice:** One-time cached fetch; raw XML under phase `evidence/` with block-ID references; label reconciliation in the inventory file.
**Notes:** Labels: Death Jade Lake, Spiny Moss Court, Valley of Lush and Moist, Green Tundra, Town of Decaying Wood.

---

## 既有代码基线 (Existing-Code Baseline)

| Option | Description | Selected |
|--------|-------------|----------|
| Reconcile each, trust existing, fill gaps | Avoids rework; respects existing implementations | ✓ |
| Wholesale re-implementation | Max consistency, high cost/compat risk | |
| Inventory-first, decide later | Defers the strategy | |

| Option | Description | Selected |
|--------|-------------|----------|
| Count code completion toward phase completion; `code=true/art=false`, row uncolored, wait for art | Matches user's stated preference | ✓ |
| Do not count; keep blocked | Strict roadmap "completed-art" reading | |
| Hybrid: count code, separate success criteria | Splits code/art acceptance | |

| Option | Description | Selected |
|--------|-------------|----------|
| Fix deviations in Phase 1 for completed-art entries; others → Phase 2 | Keeps "only fill gaps" scope | ✓ |
| All deviations → Phase 2 | Minimal Phase 1 change | |
| Mark only, fix in Phase 8 | Defers all alignment | |

**User's choice:** Trust existing code + fill gaps; code completion counts toward phase completion (art stays a tracked blocker); completed-art deviations fixed in Phase 1, others deferred.
**Notes:** User explicitly required advancing entries to Phase 1 completion without waiting for artwork.

---

## the agent's Discretion

- Exact JSON sidecar schema fields/nesting beyond required status/block-ID fields.
- Evidence file naming and layout under `evidence/`.
- Which existing base classes/templates to reuse for new item entries.

## Deferred Ideas

None — discussion stayed within phase scope.
