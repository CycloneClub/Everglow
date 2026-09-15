---
phase: "03"
slug: "completed-art-ordinary-monsters"
# status lifecycle: draft (seeded by plan-phase) → validated (set by validate-phase §6)
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-09-15"
---

# Phase 03 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | **none for `ModNPC` content** — MSTest 3.10.2 (`Sources/Everglow.UnitTests`) cannot construct `Main`, load mod content, or run the game loop; verification is offline PowerShell gates + `dotnet build` + a client UAT batch (mirrors `02-VALIDATION.md`). |
| **Config file** | none — `Sources/Everglow.UnitTests` has no test config file; tests are discovered by the SDK |
| **Quick run command** | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| **Full suite command** | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1 -RequireAll` → Phase 1 gates (`validate-inventory`, `check-inventory-reconciliation`, `check-carryover`, `check-tranche-A`, `check-tranche-B`) → Phase 2 `check-phase2.ps1` → `dotnet test --filter "FullyQualifiedName~Yggdrasil"` |
| **Estimated runtime** | ~60–120 s (build dominates) |

---

## Sampling Rate

- **After every task commit:** Run `dotnet build /p:Configuration=Release /p:WarningLevel=0` for any C# change; `scripts/check-biology.ps1` for matrix/gate changes
- **After every plan wave:** Run `scripts/check-biology.ps1` + `dotnet build /p:Configuration=Release /p:WarningLevel=0` + the Phase 1/2 regression gates
- **Before `/gsd-verify-work`:** Full suite must be green, plus the D-21 client/`!Main.dedServ` UAT bundle
- **Max feedback latency:** ~120 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|------------|-----------------|-----------|-------------------|-------------|--------|
| 03-01-01 | 03-01 | 1 | BIO-01/BIO-02/BIO-03 (tranche) | T-03-01 / T-03-02 / T-03-03 | Subworld-isolated spawn; no placeholder art; no absent drop type | structural + build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-01-02 | 03-01 | 1 | BIO-01 (tranche) | T-03-20 | Mirror/ledger match matrix; no hand-edited design source | structural | `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-02-01 | 03-02 | 2 | BIO-01 (tranche) | T-03-09 | Neutral shell-up creature; Phase 1 accessory drop only | structural + build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-02-02 | 03-02 | 2 | BIO-01 (tranche) | T-03-16 | Water suffocation server-authoritative; 50% poison applied once on the hit client (local-client `OnHitPlayer` hook) | structural + build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-02-03 | 03-02 | 2 | QUAL-04 | T-03-20 | Per-creature comparison record current | structural | `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-03-01 | 03-03 | 3 | BIO-02 (tranche) | T-03-01 | Subworld-gated rare spawn; state machine server-authoritative | structural + build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-03-02 | 03-03 | 3 | BIO-02 (tranche) | T-03-11 / T-03-17 | Enemy projectiles never spawn/reference absent types | structural + build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-03-03 | 03-03 | 3 | QUAL-04 | T-03-20 | Matrix/mirror/ledger reconciled | structural | `scripts/check-biology.ps1` | ❌ W0 | ⬜ pending |
| 03-04-01 | 03-04 | 4 | QUAL-04 | T-03-20 | All five tranche rows + consolidated ledger | structural | `scripts/check-biology.ps1 -RequireAll` | ❌ W0 | ⬜ pending |
| 03-04-02 | 03-04 | 4 | QUAL-01 / QUAL-03 | T-03-01 / T-03-16 | Build clean; no main-world leakage; dedicated-server safe | full chain + manual | full suite command above | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — the matrix (D-24), produced from the frozen tranche rule (D-41)
- [ ] `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — the human-readable mirror
- [ ] `.planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1` — the gate (D-27), authored before the creature classes so it runs red→green
- [ ] `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — conservative spawn weights (D-34), drop chances (D-38), all blockers (D-39/D-43)
- [ ] Creature `ModNPC` class files under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/` (class names mirror asset basenames)
- [ ] Framework install: **none** — existing infrastructure (MSTest + PowerShell + build) covers the phase

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Mod loads with no missing-resource error; each new creature appears | BIO-01/BIO-02 | Runtime loader property; no offline script observes it (D-21) | Launch a tModLoader client, enter the Kelp Curtain/Death Jade Lake context, confirm clean load and creature presence |
| Subworld isolation — no creature spawns in the main world | BIO-06 | Runtime spawn behavior | Run a normal world session and a dedicated-server launch; confirm zero main-world spawns |
| Per-creature water/land behavior, hostility, status effects, drops | BIO-01/BIO-02/BIO-03 | Runtime gameplay property | Walk the documented contexts and observe each behavior/drop |
| Dedicated-server safety (no graphics crash) | QUAL-03 | Runtime | Start a dedicated server; confirm no crash and no client-only work |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 120s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
