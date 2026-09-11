---
phase: "1"
slug: "item-inventory-completed-art-items"
status: draft
nyquist_compliant: true
wave_0_complete: true
created: "2026-09-11"
updated: "2026-09-11"
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
| 1-05-03 | 05 | 5 | QUAL-05 / ITEM-07 | T-05-04 | Deviations merged; blocked localization recorded and honored | static + build | `powershell -File scripts/check-localization-coverage.ps1` + `check-inventory-reconciliation.ps1` + `dotnet build` | ✅ existing | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

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
| No placeholder art introduced | QUAL-05 | Visual/asset judgment; repo rule | Confirm no new `.png` files and no `White_Mod`/placeholder texture fallbacks on accepted items |

---

## Validation Sign-Off

- [x] All tasks have `<automated>` verify or Wave 0 dependencies
- [x] Sampling continuity: no 3 consecutive tasks without automated verify
- [x] Wave 0 covers all MISSING references
- [x] No watch-mode flags
- [x] Feedback latency < 180s
- [x] `nyquist_compliant: true` set in frontmatter

**Approval:** pending (set by `/gsd-validate-phase`)
