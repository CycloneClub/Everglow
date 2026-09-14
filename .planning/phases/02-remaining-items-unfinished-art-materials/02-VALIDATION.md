---
phase: "2"
slug: "remaining-items-unfinished-art-materials"
# status lifecycle: draft (seeded by plan-phase) → validated (set by validate-phase §6)
# audit-milestone §5.5 distinguishes NOT-VALIDATED (draft) from PARTIAL (validated + nyquist_compliant: false) (#2117)
status: draft
nyquist_compliant: false
wave_0_complete: true
created: "2026-09-14"
---

# Phase 2 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | none — offline PowerShell `check-*.ps1` gates + `dotnet build` (repo has no unit test framework for item content) |
| **Config file** | none |
| **Quick run command** | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| **Full suite command** | Phase 2 gates (`check-phase2.ps1`) + `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| **Estimated runtime** | ~60 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet build /p:Configuration=Release /p:WarningLevel=0`
- **After every plan wave:** Run the Phase 2 gate + `dotnet build`
- **Before `/gsd-verify-work`:** Full suite must be green
- **Max feedback latency:** 120 seconds

---

## Per-Task Verification Map

*Populated after the Phase 2 plans are created (task IDs + `<automated>` commands). The Research report's `## Validation Architecture` section (02-RESEARCH.md) defines the source of validation requirements.*

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|------------|-----------------|-----------|-------------------|-------------|--------|
| (to be filled) | — | — | ITEM-01…04 | — | — | offline gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

Existing infrastructure covers all phase requirements — the Phase 1 gate convention (ASCII PowerShell `check-*.ps1` reading `01-INVENTORY.json`) and the repository build are reused.

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| In-client usability of the implemented items (obtain/craft/use with documented effect) | ITEM-01…04 | Runtime gameplay property; no offline script can prove it. Art is incomplete, so items use the shared fallback texture. | In a tModLoader client, obtain/craft/use representative Phase 2 items and compare to `02-RESEARCH.md` design values; confirm no crash/load failure. |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 120s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
