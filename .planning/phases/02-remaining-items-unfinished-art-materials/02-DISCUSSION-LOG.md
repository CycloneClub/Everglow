# Phase 2: Remaining Items & Unfinished-Art Materials - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-09-14
**Phase:** 2-remaining-items-unfinished-art-materials
**Areas discussed:** Missing-art handling, Tranche/order/drop allocation, Localization timing, Verification bar & marking, System-dependent items (follow-up)

---

## Missing-art handling

| Option | Description | Selected |
|--------|-------------|----------|
| Code-first + shared fallback texture | Implement `ModItem` code; missing art uses `Commons.ModAsset.White_Mod` + recorded blocker; Feishu row unchecked | ✓ |
| Prefer neighbouring textures | Reuse semantically similar sibling textures where sensible, else fallback | |
| Wait for approved art | Implement only after approved art exists | |

**User's choice:** Code-first + shared fallback texture.
**Notes:** Clarifying question first — "what are these 25 entries?" Answered with the full `phase==2` list. Then: a further clarifying question about the Klein Snake series (see below). Decision: code-first with the shared fallback; art stays a tracked blocker.

---

## Tranche, order & drop allocation

| Option | Description | Selected |
|--------|-------------|----------|
| Keep 3 `阵法修复材料` deferred | Placeholders (design names A/B/C) stay `deferred`, not implemented | ✓ |
| Implement in Phase 2 | Requires a real use first | |
| Remove from inventory | Needs confirmation they are invalid | |

**User's choice:** Keep the 3 placeholders deferred.
**Notes:** Clarifying question first — "which item is this?" Answered: they are the design team's own placeholder rows (`name_en` A/B/C, `name_zh` 阵法修复材料, no checkbox), `deferred_reason` = undefined future design.

### `巨翼龙面具` allocation

| Option | Description | Selected |
|--------|-------------|----------|
| Reallocate to Phase 7 (ITEM-06) | It is a Giant Winged Dragon reward; set `phase=7`, add ITEM-06 | ✓ |
| Keep in Phase 2 | If it is an ordinary-creature drop | |
| Flag only | Defer the decision | |

**User's choice:** Reallocate to Phase 7 (ITEM-06).

### Implementation order

| Option | Description | Selected |
|--------|-------------|----------|
| Dependency order | Drops/materials first, then finished equipment | ✓ |
| By family set | 红月水藻 → 竹/弟子/桃 → misc → biology drops | |
| Agent's discretion | researcher/planner decides | |

**User's choice:** Dependency order.

**Notes (clarifying Q&A):** The user observed that the online Feishu doc shows several items with artwork but no code, not completed in Phase 1 (e.g. 碧绿玉髓扇, Klein Snake series). Explained: those are the Kleine Snake series reclassified to Phase 7 (ITEM-05) by the 2026-09-12 two-step allocation correction; Phase 1 intentionally implemented only the non-boss item-table completed-art tranche.

---

## Localization timing

| Option | Description | Selected |
|--------|-------------|----------|
| Continue deferring | Record-only; no exporter, no HJSON edits | |
| Run exporter at Phase 2 end | User runs the client exporter, then fill en-US/zh-Hans | |
| Leave to Phase 8 | Handle all localization at design-status sync | |
| **Out of scope for all phases** | Goal is code design only; no localization tasks | ✓ |

**User's choice (free text):** "所有阶段都不管本地化，你的目标是完成代码设计，不进行本地化的任务" — localization is out of scope for every phase; the goal is code design.

---

## Verification bar & marking

| Option | Description | Selected |
|--------|-------------|----------|
| Offline gates + Release build | Per-item structural gates + `dotnet build` + recorded blockers | |
| Require runtime verification | Also verify representative items in the tModLoader client | ✓ |
| Build + matrix only | No extra structural gates | |

**User's choice:** Require runtime verification.
**Notes:** Marking rule carried forward from Phase 1 D-11 (code-complete + art-incomplete → `code_complete=true` / `artwork_complete=false`, no colour).

---

## System-dependent items (follow-up area)

| Option | Description | Selected |
|--------|-------------|----------|
| Identity + blocker, no system | Implement what is defined; record the missing system as a blocker | ✓ (via free text) |
| Implement the minimal system | Expands scope; likely a separate phase | |
| Defer the whole item | Wait for the system phase | |

**User's choice (free text):** "文档中有不少内容暂时没有详细物品描述，也没有 checkbox，这些内容先不做代码编写，仅写一个空壳类即可" — entries with no detailed description and no checkbox get an identity-only shell class (e.g. the 弟子 set, 竹节武器, etc.).

### Shell-vs-full classification (confirmed)

| Option | Description | Selected |
|--------|-------------|----------|
| Confirm classification | 9 full implementations; 12 shell classes (+3 deferred placeholders) | ✓ |
| Needs adjustment | User would specify | |

**User's choice:** Confirm.

---

## the agent's Discretion

- Exact shell-class contents and folder placement.
- Blocker wording for missing-art and missing-system cases.
- Whether a full-implementation item needs a new projectile/buff/VFX class now or records a precise effect blocker.

## Deferred Ideas

- Localization for all phases — out of scope (user directive).
- 3 × `阵法修复材料` placeholder rows.
- Runtime verification — pending client availability.
- System-dependent items (弟子/技能/区域制作) — the system belongs to a later phase.
