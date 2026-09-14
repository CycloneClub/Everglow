---
phase: 02-remaining-items-unfinished-art-materials
plan: 05
subsystem: planning-verification
tags: [terraria, tmodloader, phase-closeout, deviation-ledger, validation-map, powershell-gate, bom-check, inventory-audit]

# Dependency graph
requires:
  - phase: 02-remaining-items-unfinished-art-materials
    provides: the 21 implemented entries (9 full / 12 shell), 02-CLASSIFICATION.json and the green scripts/check-phase2.ps1 gate from plans 02-01..02-04; the D-11/D-22 marking, D-13 fallback, D-18/D-19 shell policy and D-20 deferral precedents
  - phase: 01-item-inventory-completed-art-items
    provides: 01-INVENTORY.json / .md (103 rows), the Phase 1 regression gates (validate-inventory, check-inventory-reconciliation, check-carryover, check-tranche-A/B, check-localization-coverage) and the 01-07 stale-anchor re-baselining precedent
provides:
  - "One consolidated .planning/phases/02-.../02-DEVIATIONS.md six-section ledger covering all 21 implemented entries (scope/allocation, 44-row blocker table, D-23 deviations DD-01..DD-13, D-19 system blockers, D-20 localization deferral, verification evidence)"
  - "21 Phase 2 localization-deferral records in 01-INVENTORY.json deviations[] and a phase2_closeout counts block"
  - "Populated 02-VALIDATION.md Per-Task Verification Map (16 tasks across 02-01..02-05) and Manual-Only Verifications table (every <human-check>)"
  - "Scoped Phase 2 close-out record in ROADMAP.md"
  - "Captured full gate-chain evidence: Release build 0/0, check-phase2 -RequireAll 21/21, Phase 1 regression gates green, baseline-anchored BOM + no-art guard OK"
affects: [07-bosses-special-encounters, 08-design-status-sync, gsd-verify-work]

# Actuals (#2632)
actuals:
  tokens: 28320    # chars/4 over the realized diff of the three plan commits
  tasks: 3
  commits: 3
plan_head_before: 15daae02fe4c8a0949a5fd6c2fa99aacb8f9b75b

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Consolidated six-section phase ledger: scope/allocation -> blockers -> design deviations -> system blockers -> deferred localization -> verification evidence, with a coverage claim and a byte-identical cross-artifact blocker register"
    - "Baseline-anchored byte-level BOM + no-art guard that re-anchors to the merge commit immediately preceding a phase's execution commits when a branch merge makes the plan-named baseline non-isolating"
    - "Plan close-out counts recorded on both the JSON machine source (phase2_closeout) and the Markdown mirror"

key-files:
  created: []
  modified:
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-VALIDATION.md
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - .planning/ROADMAP.md

key-decisions:
  - "Make the ledger's blocker text byte-identical to 01-INVENTORY.json by generating the 44-row table from the JSON rather than retyping it, so the three audit artifacts cannot drift"
  - "Re-anchor the BOM/no-art guard from the plan-named baseline 926543d99 to the merge commit 43478f8ef: the branch merge pulled a parallel developer art commit (a1975d1bf, 10 .pngs) into 926543d99..HEAD, so the named anchor could not isolate the Phase 2 change set (Rule 3 blocking fix; the Phase 1 01-07 precedent)"
  - "Record the plan close-out counts on both 01-INVENTORY.json (phase2_closeout) and 01-INVENTORY.md instead of editing the 103-row matrix, so the reconciliation gate's row count is untouched"
  - "Keep 02-VALIDATION.md front matter at status:draft / nyquist_compliant:false; promotion belongs to /gsd-validate-phase"
  - "Report the strict localization gate as expected-red evidence under D-20 (the gate selects Phase 1 entries only; the 18-missing baseline is unchanged)"

patterns-established:
  - "Generated-from-JSON ledger table: a byte-identical blocker register produced mechanically from the machine source of truth"
  - "Merge-topology baseline correction: when a phase's plan-named anchor is not the merge boundary, re-anchor to the merge commit rather than accept a false-positive art/BOM result"

requirements-completed: [ITEM-01, ITEM-02, ITEM-03, ITEM-04]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "02-DEVIATIONS.md is one consolidated six-section ledger covering all 21 implemented entries; its Blockers table carries a byte-identical copy of every entry's JSON blocker and the System-blocker section names the four D-19 system entries; 01-INVENTORY.json deviations[] gains a Phase 2 localization record for each of the 21 entries"
    requirement: "ITEM-01"
    verification:
      - kind: integration
        ref: "pwsh -File .planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1 -RequireAll (OK(0) 21/21)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1 (OK(0) 103 entries)"
        status: pass
      - kind: integration
        ref: "pwsh -File .planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1 (OK(0) 103 entries; 103-row matrix)"
        status: pass
    human_judgment: false
  - id: D2
    description: "02-VALIDATION.md has a row for every task across plans 02-01..02-05 with its verbatim <automated> command and a row for every <human-check> from 02-01..02-04; its front matter stays draft/false; ROADMAP.md gains a scoped Phase 2 close-out record naming 21 implemented / 3 deferred / 9 full / 12 shell and the code-complete/art-incomplete marking"
    requirement: "ITEM-02"
    verification:
      - kind: integration
        ref: "powershell regex gate: validation rows=72, roadmap close-out present"
        status: pass
    human_judgment: false
  - id: D3
    description: "The complete Phase 2 gate chain exits 0 — Release build 0 errors/0 warnings, check-phase2 -RequireAll 21/21, validate-inventory / check-inventory-reconciliation / check-carryover / check-tranche-A / check-tranche-B green, advisory localization baseline recorded — and the baseline-anchored byte-level BOM check plus no-art/no-binary guard report zero BOM and zero art/binary paths over the true Phase 2 change set"
    requirement: "ITEM-03"
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0; 0 Warning(s), 0 Error(s))"
        status: pass
      - kind: integration
        ref: "verify chain OK(0): check-phase2 21/21, 5 Phase 1 gates green, BOM_OK (41 files, 0 BOM), BIN_OK (0 art/binary, KelpCurtain=26)"
        status: pass
    human_judgment: false
  - id: D4
    description: "The runtime-verification bundle transcribed from every <human-check> in plans 02-01..02-04 is recorded for the end-of-phase UAT batch (D-21); no Feishu status colour was written and every implemented row stays artwork_complete:false / status:unchecked"
    requirement: "ITEM-04"
    verification: []
    human_judgment: true
    rationale: "The bundle is a set of live tModLoader client checks (equip/load, projectile arcs, boss-damage delta, shell load behaviour) that no offline script observes; the human must run them before the phase's code-complete/art-incomplete marking is accepted."

# Metrics
duration: 10min
completed: 2026-09-14
status: complete
---

# Phase 2 Plan 05: Phase Close-Out — Consolidated Ledger, Validation Sign-Off and Full Gate Chain Summary

**One consolidated six-section Phase 2 audit ledger covering all 21 implemented entries with a byte-identical 44-row blocker register, a populated validation map, a scoped roadmap close-out, and the full gate chain — Release build 0/0, `check-phase2` 21/21, every Phase 1 regression gate green, and a baseline-anchored BOM/no-art guard reporting zero — captured as evidence.**

## Performance

- **Duration:** 10 min
- **Started:** 2026-09-14T19:08:56+08:00
- **Completed:** 2026-09-14T19:18:25+08:00
- **Tasks:** 3 / 3
- **Files modified:** 5 (483 insertions, 165 deletions)

## Accomplishments

- Consolidated `02-DEVIATIONS.md` into the phase's single audit trail: §1 scope/allocation (25 allocated, 3 D-15 deferred, 1 D-16 reallocated, 9 full / 12 shell), §2 a 44-row blocker table with text generated byte-identically from `01-INVENTORY.json` (all 21 entries carry an `artwork` row; 4 `system`, 8 `effect`, 3 `recipe` rows), §3 the DD-01..DD-13 D-23 design deviations, §4 the four D-19 system blockers with an explicit "no system was implemented" statement, §5 the 21-row D-20 localization deferral, and §6 the captured verification evidence. A coverage claim closes the loop: "a Phase 2 entry absent from it is a defect."
- Mirrored the deferral into the machine source: `01-INVENTORY.json` `deviations[]` gained 21 Phase 2 records (`field: localization`, `status: deferred`), and a `phase2_closeout` counts block was added; `01-INVENTORY.md` gained a Phase 2 close-out note while its matrix stayed at exactly 103 rows.
- Populated `02-VALIDATION.md`: a 16-row Per-Task Verification Map for plans 02-01..02-05 (verbatim `<automated>` commands, requirements, threat refs, test type, green status) and a Manual-Only Verifications table covering every `<human-check>` plus the 02-05 bundle; front matter deliberately left `draft` / `nyquist_compliant: false`.
- Appended the scoped `.planning/ROADMAP.md` Phase 2 close-out record (21 implemented of 24 in-scope rows, 3 deferred placeholders, 9 full / 12 shell, code-complete/art-incomplete with a blocker and an unchecked Feishu row, localization deferred, evidence locations).
- Ran and captured the complete Phase 2 gate chain and the AGENTS.md byte-level checks: `dotnet build` 0 warnings / 0 errors; `check-phase2.ps1` and `check-phase2.ps1 -RequireAll` both `OK(0) 21/21`; `validate-inventory` 103 entries; `check-inventory-reconciliation` 103 entries / deferred=3; `check-carryover` 5/5; `check-tranche-A` 43/43; `check-tranche-B` 20/20; advisory localization 45/63 with the strict run recorded as expected-red; BOM 0 and no-art/binary 0 over the Phase 2 change set.

## Task Commits

Each task was committed atomically:

1. **Task 1: Consolidate the deviation, blocker and localization ledger against the inventory** - `eab47477d` (docs)
2. **Task 2: Populate the validation map and record the Phase 2 close-out in the roadmap** - `377d98d9b` (docs)
3. **Task 3: Run the complete Phase 2 gate chain and record the runtime-verification bundle** - `d5592c262` (docs)

**Plan metadata:** `(final metadata commit made by this execution)` (docs: complete plan)

## Files Created/Modified

- `.planning/phases/02-remaining-items-unfinished-art-materials/02-DEVIATIONS.md` - restructured into the six-section consolidated ledger; §2 blocker table generated from the JSON; §6 verification evidence with the gate chain, BOM/no-art results, runtime bundle and final counts
- `.planning/phases/02-remaining-items-unfinished-art-materials/02-VALIDATION.md` - Per-Task Verification Map (16 rows) and Manual-Only Verifications (8 rows); front matter stays draft/false
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` - 21 Phase 2 `deviations[]` localization records (44 -> 65 total) and a `phase2_closeout` counts block
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` - Phase 2 close-out note; 103-row matrix preserved
- `.planning/ROADMAP.md` - scoped Phase 2 close-out record under the Phase 2 `**Plans:**` line

## Decisions Made

- **Generate the ledger table from the JSON.** The 44 blocker rows were produced mechanically from `01-INVENTORY.json` so the exact blocker text is byte-identical across the ledger, the JSON and the Markdown mirror; the drift the plan warns about (T-02-01) is designed out, not checked for after the fact.
- **Re-anchor the BOM/no-art guard to the merge commit `43478f8ef`.** The plan named `926543d99` (parent of the first Phase 2 planning commit `2fc6aa346`) as the phase boundary, but the branch contains a merge commit (`43478f8ef`) that joined a parallel developer line; `926543d99..HEAD` therefore includes `a1975d1bf` ("Fix bugs and improve visual effect of ForestRainVine."), which added 10 `.png`s. Re-anchoring to `43478f8ef` yields a 41-file change set (26 under `Sources/Modules/Yggdrasil/KelpCurtain/`) with 0 BOM and 0 art/binary — the plan's own instruction ("if they differ, stop and re-anchor to the pre-Phase 2 boundary commit") and the Phase 1 01-07 `8ed6f5862` precedent.
- **Record counts on both JSON and Markdown, without touching the matrix.** `01-INVENTORY.json` gains `phase2_closeout` and `01-INVENTORY.md` a one-section note; the 103-row matrix and all 21 Phase 2 Blockers cells were already consistent with the JSON and were left unchanged.
- **Report the strict localization gate honestly.** Under D-20 no exporter was run and no key fabricated; the gate (which selects Phase 1 entries) reports the unchanged 45/63 baseline, so the strict run stays red by design and the advisory `-AllowMissing` run is the recorded gate.
- **Keep validation at draft.** `nyquist_compliant` stays `false` with an explicit line that promotion is `/gsd-validate-phase`'s step.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Plan-named Phase 2 baseline `926543d99` cannot isolate the phase change set**
- **Found during:** Task 3 (the baseline-anchored BOM/no-art guard)
- **Issue:** `git rev-parse 926543d99` resolves and `git merge-base HEAD 926543d99` equals it, so the plan's own assertion passes; but because the branch contains the merge commit `43478f8ef`, the range `926543d99..HEAD` also includes the parallel developer commit `a1975d1bf`, which added 10 `.png` files (`VineRepairWand*.png`, `VineEnergyBeam/Orb.png`, `ForestRainVineTile_*.png`). The no-art guard therefore reported `art/binary changed:` on non-Phase-2 art, exactly the false-positive class the plan's anchor was meant to remove.
- **Fix:** Re-anchored the guard to `43478f8ef` (the merge commit immediately preceding every Phase 2 execution commit), which excludes the parallel line while retaining every Phase 2 source/planning change. Result: 41 changed files, `BOM=0`, `BIN=0`, `KelpCurtain=26`.
- **Files modified:** none (the guard is run, not stored; the evidence and the re-anchor are recorded in `02-DEVIATIONS.md` §6.3)
- **Verification:** `verify chain OK(0)`; the plan-named baseline's result is also recorded (`BOM=0`, 10 art paths, all from `a1975d1bf`) so the deviation is fully auditable.
- **Committed in:** `d5592c262` (Task 3)

---

**Total deviations:** 1 auto-fixed (1 Rule 3 blocking)
**Impact on plan:** No scope creep. The re-anchor is strictly necessary to obtain a meaningful no-art result and follows the plan's own re-anchor instruction and the Phase 1 precedent; the guard is unchanged in strength and still covers every Phase 2 file.

## Issues Encountered

- The plan's Task 3 `<verify>` command hard-codes `git rev-parse '926543d99'` and expects the guard to pass; it cannot in this merge topology. The corrected chain (same build + gates, anchor `43478f8ef`) was run and prints `verify chain OK(0)`; both outcomes are recorded in `02-DEVIATIONS.md` §6.3.
- PowerShell 5.1 renders the CJK planning content as mojibake in the console, so verification was performed through the gates (which read UTF-8 via `[IO.File]::ReadAllText`) and Node `JSON.parse`, not by eyeballing console output.
- The nested `$`-interpolating verify command cannot be pasted directly through the outer PowerShell shell; it was run from a script file preserving the command body exactly.

## Known Stubs

None. The ledger's recorded artwork/system/effect/recipe blockers are intentional, named deferrals (D-13/D-19/D-20/D-21), not untracked stubs.

## Threat Flags

None. This plan changes only planning artifacts; it adds no network, auth, file-access or schema surface.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Phase 2 is closed with one consistent audit trail (ledger ↔ `01-INVENTORY.json` ↔ `01-INVENTORY.md` ↔ `02-CLASSIFICATION.json`) and a fully green runnable gate chain; `/gsd-verify-work` can drive the end-of-phase UAT from the recorded runtime bundle.
- Outstanding recorded blockers for later phases: all 21 entries' approved artwork (D-13/D-14); the four D-19 system shells (弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台) until the disciple/skill/regional-crafting systems and the crafting station's tile are designed; the 红月水藻 set clauses E-1/E-2; and the Phase 7 encounters/recipes named in §2.3/§2.4.
- No Feishu status colour was written (D-22) and localization remains deferred by directive (D-20); the strict localization gate stays honestly red while the advisory baseline is recorded.
- `02-VALIDATION.md` stays `draft` / `nyquist_compliant: false` for `/gsd-validate-phase` to promote.

---

*Phase: 02-remaining-items-unfinished-art-materials*
*Completed: 2026-09-14*

## Self-Check: PASSED

- All 5 modified plan artifacts exist on disk; `01-INVENTORY.md` retains exactly 103 matrix rows.
- All 3 task commits exist: `eab47477d`, `377d98d9b`, `d5592c262` (measured `3` commits since `plan_head_before` = `15daae02fe4c8a0949a5fd6c2fa99aacb8f9b75b`).
- `verify chain OK(0)` (build 0/0, `check-phase2 -RequireAll` 21/21, 5 Phase 1 gates green, BOM 0, art/binary 0).
