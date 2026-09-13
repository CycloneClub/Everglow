---
phase: "1"
slug: "item-inventory-completed-art-items"
status: validated
nyquist_compliant: true
wave_0_complete: true
created: "2026-09-11"
updated: "2026-09-13"
---

# Phase 1 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | MSTest 3.10.2 + Microsoft.NET.Test.Sdk 17.14.1 |
| **Config file** | `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj` (no `.runsettings`) |
| **Quick run command** | `dotnet test --filter "FullyQualifiedName~Yggdrasil" /p:WarningLevel=0` |
| **Full suite command** | `dotnet test --verbosity normal /p:WarningLevel=0` |
| **Estimated runtime** | ~60–180 seconds (full suite) |

---

## Sampling Rate

- **After every task commit:** Run `dotnet build` (mandatory for every code change per AGENTS.md)
- **After every plan wave:** Run `dotnet test --verbosity normal /p:WarningLevel=0` and the JSON/Markdown inventory consistency check
- **Before `/gsd-verify-work`:** Build green + inventory artifacts internally consistent + label table complete
- **Max feedback latency:** 180 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|------------|-----------------|-----------|-------------------|-------------|--------|
| 1-01-01 | 01 | 1 | QUAL-05 | T-01-01 / T-01-02 / T-01-03 | Fetched Feishu XML treated as data; no token committed | static | `powershell -File scripts/validate-inventory.ps1` | ❌ W0 | ⬜ pending |
| 1-01-02 | 01 | 1 | QUAL-05 | T-01-03 | Bounded parse; checkbox-less biology rows recorded with a blocker; `parse_audit` completeness | static | `powershell -File scripts/validate-inventory.ps1` | ❌ W0 | ⬜ pending |
| 1-01-03 | 01 | 1 | QUAL-05 | T-01-01 | Header-anchored column/status rules locked by fixture | static | `powershell -File scripts/test-parser.ps1` | ❌ W0 | ⬜ pending |
| 1-02-01 | 02 | 2 | QUAL-05 | T-02-01 / T-02-02 | Feishu wins; `tranche`/`advances` recorded | static | `powershell -File scripts/check-inventory-reconciliation.ps1` | ❌ W0 | ⬜ pending |
| 1-02-02 | 02 | 2 | QUAL-05 | T-02-04 | Unresolved labels stay blocked | static | `powershell -File scripts/check-inventory-reconciliation.ps1` | ❌ W0 | ⬜ pending |
| 1-02-03 | 02 | 2 | QUAL-05 | T-02-04 | No silent scope drop; matrix/JSON agree | static + manual | `powershell -File scripts/check-inventory-reconciliation.ps1` + human-check | ❌ W0 | ⬜ pending |
| 1-03-01 | 03 | 3 | QUAL-05 / ITEM-01 / ITEM-02 | T-03-02 | Only numeric/name fields transcribed; paths from repo conventions | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ existing | ⬜ pending |
| 1-03-02 | 03 | 3 | QUAL-05 / ITEM-01 / ITEM-02 | T-03-01 / T-03-03 | No placeholder art; classes written back to the inventory | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ existing | ⬜ pending |
| 1-03-03 | 03 | 3 | QUAL-05 / ITEM-01 / ITEM-02 | T-03-01 / T-03-03 | `tranche`-selected coverage gate can fail for class-less entries | static | `powershell -File scripts/check-tranche-A.ps1` | ❌ W0 | ⬜ pending |
| 1-04-01 | 04 | 4 | QUAL-05 / ITEM-02 / ITEM-03 | T-04-02 / T-04-04 | Placeable bindings; no key renames | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ existing | ⬜ pending |
| 1-04-02 | 04 | 4 | QUAL-05 / ITEM-02 / ITEM-03 | T-04-01 / T-04-03 | No placeholder art; classes written back to the inventory | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ existing | ⬜ pending |
| 1-04-03 | 04 | 4 | QUAL-05 / ITEM-02 / ITEM-03 | T-04-01 / T-04-03 | `tranche`-selected coverage gate can fail for class-less entries | static | `powershell -File scripts/check-tranche-B.ps1` | ❌ W0 | ⬜ pending |
| 1-05-01 | 05 | 5 | QUAL-05 / ITEM-07 | T-05-01 / T-05-04 | Additive keys; `localization.blocked` excluded from the gate | static + build | `powershell -File scripts/check-localization-coverage.ps1 -AllowMissing` + `dotnet build` | ❌ W0 | ⬜ pending |
| 1-05-02 | 05 | 5 | ITEM-07 | T-05-01 / T-05-03 | In-game exporter only; no hand-created keys | manual (checkpoint) | `powershell -File scripts/check-localization-coverage.ps1` | ✅ existing | ⬜ pending |
| 1-05-03 | 05 | 5 | QUAL-05 / ITEM-07 | T-05-04 | Deviations merged; blocked localization recorded and honored | static + build | `powershell -File scripts/check-localization-coverage.ps1` + `check-inventory-reconciliation.ps1` + `dotnet build` | ✅ existing | ✅ green |
| 1-06-01 | 06 | 6 | QUAL-05 | T-06-01 / T-06-03 | Recorded-artwork-blocker exception keeps the no-silent-gap guard; 103 rows preserved | static | `validate-inventory.ps1` + `check-inventory-reconciliation.ps1` | ✅ existing | ✅ green |
| 1-06-02 | 06 | 6 | QUAL-05 | T-06-02 / T-06-05 | No placeholder art; orphan textures consumed; class-or-blocker gate cannot pass a generic marker | build + static | `dotnet build /p:Configuration=Release /p:WarningLevel=0` + `check-carryover.ps1` + `check-tranche-A.ps1` + `check-inventory-reconciliation.ps1` | ✅ existing | ✅ green |
| 1-06-03 | 06 | 6 | QUAL-05 | T-06-04 | Localization stays deferred; no HJSON hand-edit; parser not re-run | static + build | `check-carryover.ps1` + `check-tranche-A.ps1` + `check-inventory-reconciliation.ps1` + `check-localization-coverage.ps1 -AllowMissing` + `dotnet build` | ✅ existing | ✅ green |
| 1-07-01 | 07 | 7 | QUAL-05 | T-07-06 | Per-player/per-stack charge (shared `ModItem` field removed); right-click exempt from the 0.75x floor | static + build | `check-armofgianttree-charge.ps1` + `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ existing | ✅ green |
| 1-07-02 | 07 | 7 | QUAL-05 | T-07-06 | Server-authoritative shockwave; sender-held-item spoof rejection; charge clamped to `[0, MaxChargeFrames]` | static + manual | `check-armofgianttree-charge.ps1` + human-check | ✅ existing | ✅ green (structure); manual (runtime) |
| 1-07-03 | 07 | 7 | QUAL-05 | T-06-01 | Parser not re-run; no art/HJSON change; prior Phase 1 gates stay green | static + build | `check-carryover.ps1` + `validate-inventory.ps1` + `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ existing | ✅ green |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

Plan-01-01..01-05 rows above were audited after phase completion (all gates exit 0 as of
the 2026-09-13 re-run); their `File Exists` marker (`❌ W0`) reflects the Wave-0 decision to
use committed offline PowerShell validators rather than MSTest files — the scripts exist
under `scripts/` and run green, so the coverage status is ✅ green despite the `W0` marker.

---

## Wave 0 Requirements

Wave 0 decision made: use committed offline PowerShell validators rather than a pure-logic MSTest for the JSON model — the artifacts are documentation-driven and the content itself is tML-runtime-verified, not unit-testable.

- [x] `scripts/validate-inventory.ps1` + `scripts/test-parser.ps1` — schema, checkbox-column, biology completeness, and status-colour gates (plan 01).
- [x] `scripts/check-inventory-reconciliation.ps1` — entry/label/`tranche`/`advances` consistency gate (plan 02).
- [x] `scripts/check-tranche-{A,B}.ps1` and `scripts/check-localization-coverage.ps1` — tranche coverage and both-culture localization gates (plans 03–05).
- [ ] `tML*` env vars remain required before `dotnet test` can run; `dotnet build` is the per-task code gate in the meantime (AGENTS.md).
- [ ] No MSTest helper is anticipated; add one under `Sources/Everglow.UnitTests/Modules/Yggdrasil/` only if a pure helper (for example a label-classification enum) is actually introduced.

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Design-status reconciliation per item | QUAL-05 | Requires reading the Feishu source snapshot and human judgment on green/yellow/unchecked | Compare each inventory row to the cached XML; confirm `artwork_complete`/`code_complete` and reasons for yellow/blocked |
| No placeholder art introduced | QUAL-05 | Visual/asset judgment; repo rule | Confirm no new `.png`/binary file was added or modified and no placeholder art was fabricated. A `Commons.ModAsset.White_Mod` fallback is allowed only for an entry that carries a recorded `texture\|artwork` blocker (D-06 carry-over rule), not as a general shortcut. |
| Carry-over entries usable in game | QUAL-05 / ITEM-03 / ITEM-04 | Requires a running tML client and Phase 6 loot tables | Once Phase 6 sources exist, obtain each of the five carry-over items via its designed chest/trade source and use the behaviour (charge smash, purification, quest turn-in, vanity equip, melee combo) |
| ArmOfGiantTree charge sync + server-authoritative shockwave in multiplayer | QUAL-05 (coverage D2/D3) | The client→server charge observation, the server-side `ReleaseSmash` shockwave and its NPC propagation, and two-client charge/slot isolation require a live tModLoader client + server; `check-armofgianttree-charge.ps1` proves structure only | Run a 2-client dedicated server: charge on client A, confirm the server receives the charge and applies the AoE once on full release, and confirm client B / a second same-type stack does not inherit A's charge |

---

## Validation Sign-Off

- [x] All tasks have `<automated>` verify or Wave 0 dependencies
- [x] Sampling continuity: no 3 consecutive tasks without automated verify
- [x] Wave 0 covers all MISSING references
- [x] No watch-mode flags
- [x] Feedback latency < 180s
- [x] `nyquist_compliant: true` set in frontmatter

**Approval:** validated (2026-09-13) — `/gsd-validate-phase 01`

---

## Validation Audit 2026-09-13

| Metric | Count |
|--------|-------|
| Gaps found | 3 |
| Resolved | 3 |
| Escalated | 0 |

Audit notes:
- Plan 01-06 was absent from the per-task map; tasks 1-06-01..03 added above.
- The "No placeholder art" manual check was stale: plan 01-06 intentionally reuses the
  `Commons.ModAsset.White_Mod` fallback for entries whose approved texture is absent,
  under the D-06 recorded-artwork-blocker rule. The manual criterion was corrected.
- Re-ran the offline gates after phase completion (no source changes made by this audit):
  `validate-inventory.ps1` → `OK(0): 103 entries (84 weapons)`;
  `check-inventory-reconciliation.ps1` → `OK(0): 103 entries; matched=65; ... labels=5 deferred=3 assumptions=7`;
  `check-carryover.ps1` → `OK(0): carry-over covered entries = 5 (of 5 selected)`.
- No MSTest gap: the phase's verification is documentation/asset-driven and covered by
  committed offline validators plus the mandatory `dotnet build` gate, per the Wave-0 decision.

## Validation Audit 2026-09-13 (plan 01-07 gap closure)

| Metric | Count |
|--------|-------|
| Gaps found | 1 |
| Resolved | 1 |
| Escalated | 1 (manual-only) |

Audit notes:
- Plan 01-07 (ArmOfGiantTree per-player/per-stack charge + server-authoritative shockwave)
  was absent from the per-task map; tasks 1-07-01..03 added above.
- Re-ran the new structural gate (no source changes made by this audit):
  `check-armofgianttree-charge.ps1` → `OK(0): ... per-stack slot-keyed, synced, and
  server-authoritative`; `check-carryover.ps1` → `OK(0): 5 (of 5 selected)`.
- Escalated to Manual-Only: the client↔server charge sync, the `ReleaseSmash` shockwave
  propagation and the two-client isolation cannot be observed offline (coverage D2/D3);
  the structural gate proves presence, and a live 2-client session is required for behavior.
- No new MSTest gap, consistent with the Wave-0 offline-validator decision.
