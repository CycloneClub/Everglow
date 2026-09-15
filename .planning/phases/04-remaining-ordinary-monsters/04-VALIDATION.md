---
phase: "04"
slug: "remaining-ordinary-monsters"
# status lifecycle: draft (seeded by plan-phase) → validated (set by validate-phase §6)
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-09-15"
---

# Phase 04 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution. Seeded from `04-RESEARCH.md` § Validation Architecture.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | **none for `ModNPC` content** — MSTest 3.10.2 (`Sources/Everglow.UnitTests`) cannot construct `Main`, load mod content, or run the game loop; verification is offline PowerShell gates + `dotnet build` + a client UAT batch (mirrors `02/03-VALIDATION.md`). |
| **Config file** | none — tests are discovered by the SDK |
| **Quick run command** | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| **Full suite command** | `dotnet build /p:Configuration=Release /p:WarningLevel=0` → the Phase 4 gate (reads `03-BIOLOGY.json`) → `03-.../scripts/check-biology.ps1 -RequireAll` → Phase 1 gates (`check-inventory-reconciliation`, `check-carryover`, `check-tranche-A`, `check-tranche-B`, `check-armofgianttree-charge`) → `check-phase2.ps1` → `dotnet test --filter "FullyQualifiedName~Yggdrasil"` → the AGENTS.md byte-level BOM block |
| **Estimated runtime** | ~60–150 s (build dominates) |

---

## Phase Requirements → Test Map

| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|--------------|
| BIO-01/02/03 | In-scope set is exactly the frozen 21 rows (`[phase]==4 && !deferred`) | structural (JSON) | Phase 4 gate set-compare | ❌ Wave 0 |
| BIO-01/02/03 | Every in-scope row has a loadable `ModNPC` class on disk | structural | gate: `internal_name` → `<Class>.cs` via `Get-ChildItem -Recurse` | ❌ Wave 0 |
| BIO-01/02/03 | No Phase 4 class references a non-existent texture | static + build | gate: `White_Mod` override present or beside-`.png`; Release build proves resolution | ❌ Wave 0 |
| BIO-01/02/03 | Spawn is subworld-isolated | static + client | gate asserts `SubworldSystem.IsActive<YggdrasilWorld>` **and** `KelpCurtainBiome.IsKelpCurtainLayer` in every `SpawnChance` | ❌ Wave 0 |
| BIO-01/02/03 | Loot tables reference no unimplemented item type | static + build | gate resolves every `ModContent.ItemType<X>()` under the whole `Sources/Modules/Yggdrasil` tree; `dotnet build` proves resolution | ❌ Wave 0 |
| BIO-06 (advanced) | No main-world behavior introduced | static + dedicated-server smoke | gate regex + normal-world idle run + dedicated-server launch | ❌ Wave 0 |
| QUAL-01 (advanced) | Build clean | build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | ✅ |
| QUAL-03 (advanced) | Multiplayer/dedicated-server safety | static + manual | gate asserts `spawnInfo.Player` (not `Main.LocalPlayer`) and `!Main.dedServ`; manual dedicated-server launch | ❌ Wave 0 |
| QUAL-04 (advanced) | Per-creature comparison record | structural | Phase 4 gate invariants 1–4, 10 | ❌ Wave 0 |

---

## Per-Task Verification Map

Populated from the finalized `04-*-PLAN.md` files after planning (one row per task; every task carries an `<automated>` verify bound to a `<fails_when>`). The requirement→test map above is the stable contract; this table tracks task-level coverage during execution.

---

## Sampling Rate

- **Per task commit:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` for any C# change; the Phase 4 gate for matrix/gate changes
- **Per wave merge:** the Phase 4 gate + `03-.../scripts/check-biology.ps1` + `dotnet build` + the Phase 1/2 regression gates
- **Before `/gsd-verify-work`:** full suite green, plus the D-21 client/`!Main.dedServ` UAT bundle (`04-UAT.md`)
- **Max feedback latency:** ~150 s

---

## Wave 0 Requirements

- [ ] `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1` — the Phase 4 gate (reads the shared `03-BIOLOGY.json`), authored before the classes so it runs red→green; leaves the Phase 3 script byte-identical
- [ ] `.planning/phases/04-remaining-ordinary-monsters/04-DEVIATIONS.md` — D-47 per-creature dispositions, conservative spawn weights (D-54), drop denominators (D-59), blocker register
- [ ] `.planning/phases/04-remaining-ordinary-monsters/04-UAT.md` — the D-21 client bundle (recorded, not executed)
- [ ] 21 `ModNPC` class files (region subfolders recommended) + any self-contained attack projectiles
- [ ] The reconciled 21 rows in `03-BIOLOGY.json` + `03-BIOLOGY.md`
- [ ] Framework install: **none** — existing infrastructure (MSTest + PowerShell + build) covers the phase

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Mod loads with no missing-resource error; each new creature appears | BIO-01/02/03 | Runtime loader property; no offline script observes it (D-21) | Launch a client, enter the Kelp Curtain context, confirm clean load and creature presence per region |
| Subworld isolation — no creature spawns in the main world | BIO-06 | Runtime spawn behavior | Normal-world idle run + dedicated-server launch; confirm zero main-world spawns |
| Per-creature water/land behavior, hostility, status effects, drops | BIO-01/02/03 | Runtime gameplay property | Walk each documented context; verify 碧灵鮟鱇's two drops, 放射虫's shell, 枯木活化士兵 犬's staff |
| Dedicated-server safety (no graphics crash) | QUAL-03 | Runtime | Start a dedicated server; confirm no crash and no client-only work |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 150s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
