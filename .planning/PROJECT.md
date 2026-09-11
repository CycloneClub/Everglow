# Everglow

## What This Is

Everglow is a Terraria content mod built on tModLoader. It adds new worlds, regions, enemies, items, systems, quests, visual effects, and other gameplay content for Terraria players.

The current milestone completes the second Yggdrasil layer, Kelp Curtain (苍苔帘幕), using the existing Feishu design documents as the source of truth while preserving room for later, not-yet-defined content plans.

## Core Value

Deliver a complete, publishable Kelp Curtain layer whose designed regions, gameplay loop, creatures, terrain, items, bosses, encounters, and rewards work together as a coherent Terraria experience.

## Requirements

### Validated

- ✓ Everglow has an active tModLoader module architecture with Core, Function, and content modules — existing codebase
- ✓ Yggdrasil is implemented as a Subworld with dedicated generation, rendering, networking, and content boundaries — existing codebase
- ✓ The Yggdrasil module already contains the Kelp Curtain area and related gameplay content — existing codebase
- ✓ A seven-document evidence-backed codebase map exists under `.planning/codebase/` — onboarding

### Active

- [ ] Complete the Kelp Curtain second-layer scope from the Feishu biology, item, and terrain design documents.
- [ ] Include the three major regions and their intended exploration, traversal, hazards, structures, and progression loops.
- [ ] Implement and integrate the planned creatures, enemies, bosses, special encounters, drops, equipment, materials, and rewards, including the final boss and special encounter chains.
- [ ] Meet publishable-quality verification: `dotnet build` passes, client gameplay is verified, and multiplayer behavior is verified where networking is involved.
- [ ] Validate each design item against the implementation and update the source design item individually after acceptance.

### Out of Scope

- New content not yet defined by the current design documents; it remains eligible for later plans rather than being assumed into this milestone.
- Design entries explicitly marked for later hardmode use remain deferred unless a later decision moves them into this milestone.
- Unrelated modules, broad architectural rewrites, or asset replacement outside the Kelp Curtain delivery needs.

## Context

The existing Yggdrasil level order identifies Kelp Curtain as the second layer. Its current implementation is centered in `Sources/Modules/Yggdrasil/`, with world generation under `Sources/Modules/Yggdrasil/WorldGeneration/`, area-specific content under `Sources/Modules/Yggdrasil/KelpCurtain/`, and shared Yggdrasil-town and player systems in their neighboring directories.

The repository map identifies important constraints: Yggdrasil is a large Subworld, world generation is a substantial single-threaded workload, client rendering and IL hooks require runtime verification, and most active Yggdrasil generation/rendering behavior is not covered by unit tests. New content must follow the Yggdrasil module boundaries and existing resource, networking, and multiplayer conventions.

### Feishu Design Source

The authoritative design source is the private `Everglow Wiki` knowledge space, under the `策划案目录` node:

- Space: `Everglow Wiki` (`space_id: 7346136587010719772`)
- Directory: `策划案目录` (`node_token: Fwp2wZhuBi5I2TkEO7BcWAyLnud`)
- Biology design: `生物策划案——第二层` (`doc_token: Jp5ndsvNBoCpljxq1eGc9S7vnfe`)
- Item design: `物品策划案——第二层` (`doc_token: FSlSdNlE1owUaAxXyFBcRSaznOe`)
- Terrain design: `地形策划案——第二层` (`doc_token: OCK2di9Zvoa8Blx3bfyczI0xn0b`)

Use the user identity for access. Stable discovery and read commands:

```text
lark-cli wiki +space-list --as user --page-all --format json
lark-cli wiki +node-list --space-id 7346136587010719772 --parent-node-token Fwp2wZhuBi5I2TkEO7BcWAyLnud --as user --page-all --format json
lark-cli docs +fetch --doc <doc_token> --doc-format xml --detail full
```

Use XML with `--detail full` when checking checkbox state, block IDs, or formatting. Markdown is suitable for reading prose but does not expose the full style metadata needed for reliable status synchronization.

### Design Status Synchronization

After each implementation item is individually verified:

1. Treat the complete design row as one status unit. Do not use different status colors for the artwork/texture cell and code cell.
2. Mark all relevant checkboxes in a fully implemented row and apply the same green background to every cell in that row: `rgb(217,245,214)`.
3. Apply the same yellow background to every cell in a partially implemented, conflicting, or known-exception row: `rgb(255,255,204)`. Keep each checkbox truthful to the specific sub-deliverable, and explain the yellow reason in the audit report and user-facing summary.
4. Keep unimplemented or blocked rows unchecked and without a completion-status background. The legacy light-orange texture-column and light-green code-column fills are not completion status and must not be interpreted or reported as such.

The repository planning artifacts are the audit trail for the comparison; the Feishu documents remain the design-status source of truth. Use the `lark-doc` update workflow with block IDs obtained from XML full fetches for status writes.

## Constraints

- **Runtime:** Target Terraria tModLoader on Windows with .NET 8 and FNA; preserve the existing build and deployment workflow.
- **Architecture:** Keep Core Terraria-independent, place tML-facing behavior in Function or modules, and keep Kelp Curtain-specific code under the Yggdrasil module boundaries.
- **Compatibility:** Treat content names, resource paths, world behavior, network state, and persisted player/world data as compatibility-sensitive.
- **Verification:** Run `dotnet build` for code changes; client, dedicated-server, and multiplayer behavior require the runtime checks appropriate to the changed feature.
- **Resources:** Do not create placeholder art or modify existing binary/art assets without an explicit requirement and approval.
- **Scope:** Include all current-milestone second-layer design content, while retaining explicit freedom to add new plans later as design decisions become available.
- **Tracking:** Planning documents and design-source status are tracked; each accepted item must have an auditable implementation and verification result.

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Use the three Feishu second-layer design documents as the source of truth | They contain the planned biology, items, terrain, completed markers, and deferred notes | — Pending |
| Treat Kelp Curtain as the current second-layer milestone | The repository level order identifies Kelp Curtain as layer two | — Pending |
| Include bosses, special encounters, and their full reward chains | The milestone is intended to complete the designed layer rather than only its common content | — Pending |
| Verify at publishable quality | A completed content layer must build and function in the client, with multiplayer checks where relevant | — Pending |
| Synchronize design status item by item | Per-item status keeps partial implementation and design conflicts visible | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd-transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd-complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-09-11 after project initialization questioning*
