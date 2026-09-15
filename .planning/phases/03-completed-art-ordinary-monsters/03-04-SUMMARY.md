---
phase: 03-completed-art-ordinary-monsters
plan: 04
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, subworld, yggdrasil, kelp-curtain, biology-matrix, gate-chain, release-build, uat-bundle, phase-closeout]

# Dependency graph
requires:
  - phase: 03-completed-art-ordinary-monsters
    plan: 01
    provides: the 31-row biology matrix (D-24), the ASCII phase gate with its 13 invariants, the server-safe KelpCurtainBiome.IsKelpCurtainLayer predicate and the 荆棘苔龟 implementation
  - phase: 03-completed-art-ordinary-monsters
    plan: 02
    provides: 格普螺 (`GuppyConch`) and 叶飞棍 (`VerdantRods`) with their drop dispositions
  - phase: 03-completed-art-ordinary-monsters
    plan: 03
    provides: 巨树人 (`GiantDandelion`) and its two hostile projectiles, closing the tranche at 5 / 5
  - phase: 01-item-inventory-completed-art-items
    provides: the Phase 1 gates re-run unchanged in the close-out chain and the five wired item types
  - phase: 02-remaining-items-unfinished-art-materials
    provides: the consolidated-ledger shape (02-DEVIATIONS.md), the check-phase2 gate and the D-20/D-21/D-22/D-23 policy
provides:
  - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json (finalized: five phase:3 rows code_complete with final blocker text, machine copy of the ledger restored)
  - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md (regenerated mirror, cell-for-cell parity over all 31 rows)
  - .planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md (consolidated ledger with the mechanical 14-row blocker register, close-out counts, decision disposition, deferred registry and the recorded gate-chain evidence)
  - .planning/phases/03-completed-art-ordinary-monsters/03-UAT.md (the D-21 client bundle, 8 checks recorded and not executed)
  - the phase's recorded green chain: Release build + check-biology -RequireAll + five Phase 1 gates + check-phase2 + the Yggdrasil MSTest link + the AGENTS.md byte-level BOM check
affects: [phase-4-remaining-ordinary-monsters, phase-7-bosses, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (48000 tokens, 2 tasks) to calibrate future estimates.
# chars/4 over the files actually changed, never a harness token count.
actuals:
  tokens: 12505
  tasks: 2
  commits: 2   # measured at SUMMARY write: git rev-list --count 33bf1dc322db0043b8eb65c45f3dffa249601ae3..HEAD (2 task commits; the plan-metadata commit is the +1 verify-work allows)
plan_head_before: 33bf1dc322db0043b8eb65c45f3dffa249601ae3

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "A close-out ledger section generated mechanically from the machine source of truth, so the blocker register and the matrix cannot drift (the plan-02-05 precedent)"
    - "A single-command offline gate chain whose links are each re-asserted green and whose AGENTS.md BOM block checks $LASTEXITCODE after every git call instead of trusting $?"
    - "A recorded-but-unexecuted UAT bundle that marks every runtime check not-executed, so an unverified tranche is never presented as verified"

key-files:
  created:
    - .planning/phases/03-completed-art-ordinary-monsters/03-UAT.md
  modified:
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md
    - .planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md
    - .planning/WINDOWS.md

key-decisions:
  - "The consolidated blocker register is appended as section 10 rather than inserted as section 2 (the 02-DEVIATIONS.md position): renumbering would invalidate every section cross-reference that STATE.md, the three prior summaries, the plan texts and the JSON carry, so sections 1-9 keep their numbers and a navigation line names the new sections"
  - "The corrupted assumptions[]/deviations[] arrays were repaired (Rule 1): they shipped from the plan-03-01 tracer commit as six 'e' placeholders, invisible to every gate, while 03-DEVIATIONS.md claims them as the machine copy of the ledger - leaving them would have made this plan's own coverage claim false"
  - "荆棘苔龟's row blocker was promoted from 03-01's ledger prose into the row itself (drop disposition, provisional effect parameters) so all five rows carry a comparable disposition; the spawn-region gap stays recorded tranche-wide in section 6 with 巨树人 carrying the per-row D-30 element, matching the plan's decision-disposition text"
  - "REQUIREMENTS.md keeps BIO-01/BIO-02/BIO-03/BIO-06 and QUAL-03/QUAL-04 Pending: the traceability table maps BIO-01/02/03 to Phase 4 and BIO-06/QUAL-03/QUAL-04 to Phase 8, and Phase 3 implemented 5 of the 25 designed ordinary creatures (2 of 14 Death Jade Lake, 1 of 4 Spiny Moss Court, 2 of 7 Valley rows), so marking them complete would be false"
  - "The AGENTS.md BOM block records a whole-branch advisory result (1375 files, 0 BOM) and names check-biology.ps1 invariant 13 (5 phase files) as the binding phase-scoped equivalent, so the non-isolating origin/master anchor is stated rather than relied on"
  - "The task-2 commit message was taken verbatim from the plan's <commit> element even though 'phase leader' reads as a typo for 'phase ledger'; plan fidelity was preferred over correcting a permanent message"

patterns-established:
  - "A close-out plan adds no gameplay scope: it reconciles the machine matrix, generates the blocker register from it, runs the whole chain in one command and records what it could not verify"
  - "A runtime check that cannot be run is recorded as not-executed with its expected observation, never omitted and never implied as passing (T-03-23)"
  - "A machine-readable field that no gate reads can still be silently corrupt; the close-out is the last place it is cheap to detect"

requirements-completed: []  # none: BIO-01/02/03 complete in Phase 4 and BIO-06/QUAL-03/QUAL-04 in Phase 8 per the REQUIREMENTS.md traceability table
requirements-advanced: [BIO-01, BIO-02, BIO-03, BIO-06, QUAL-03, QUAL-04]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "The reconciled tranche matrix: all five phase:3 rows code_complete with a resolving internal_name and final blocker text, the frozen counts/phase3_tranche/texture_complete/design_art sets unchanged, and 03-BIOLOGY.md mirroring the JSON cell-for-cell across all 31 rows"
    requirement: BIO-01
    verification:
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1 -RequireAll -> 'OK(0): phase3 tranche = 5 / 5 (rows=31)' + 'OK: implemented classes = 5 / 5'"
        status: pass
      - kind: other
        ref: "cell-by-cell JSON/MD parity script: 'PARITY OK: 31 rows, all 13 cells match'"
        status: pass
    human_judgment: false
  - id: D2
    description: "The consolidated phase ledger: strengthened coverage claim naming all five rows, the mechanically generated 14-row blocker register whose text is byte-identical to the JSON, the Phase 3 close-out counts, the D-28/D-29/D-30/D-31 decision disposition, the deferred registry and the recorded gate-chain evidence in section 8"
    requirement: QUAL-04
    verification:
      - kind: other
        ref: "check-biology.ps1 invariant 11 (row-id set and per-row status parity) over the ledger's source matrix"
        status: pass
    human_judgment: true
    rationale: "The register's wording, the tranche-rule record and the deferred dispositions are design-facing: only a designer can confirm the asset-to-creature mappings, the six design-art-only creatures D-41 sends to Phase 4, and the conservative defaults the design left unsupplied."
  - id: D3
    description: "The complete offline chain green in one command: Release build (0 warnings, 0 errors, Everglow.tmod packaged and enabled), check-biology -RequireAll, the five Phase 1 gates, check-phase2, the Yggdrasil MSTest link (3 passed, 0 failed) and the AGENTS.md byte-level UTF-8 BOM check (1375 files, 0 BOM)"
    requirement: QUAL-01
    verification:
      - kind: integration
        ref: "the single-command chain (03-DEVIATIONS.md section 8.1 table) -> CHAIN_EXIT=0, re-run as the task verify with the same result"
        status: pass
    human_judgment: false
  - id: D4
    description: "The D-21 client bundle in 03-UAT.md: per-row spawn, main-world isolation, behaviour, combat, drop, dedicated-server/multiplayer and localization-fallback checks plus the consolidated end-of-phase run, every entry marked not-executed with a header stating the bundle is recorded but not run"
    requirement: BIO-06
    verification: []
    human_judgment: true
    rationale: "Spawn isolation, AI feel, drop acquisition, dedicated-server behaviour and localization fallback are runtime properties; no offline script observes them, and no live tModLoader client session is part of this phase (D-21)."

# Metrics
duration: 30min
completed: 2026-09-15
status: complete
---

# Phase 3 Plan 04: Tranche Close-Out Summary

**The completed-art tranche reconciled and frozen at five implemented creatures with one machine-consistent ledger, a single-command green gate chain plus Release build, and the outstanding client checks captured as an explicitly unexecuted D-21 UAT bundle**

## Performance

- **Duration:** ~30 min (wall clock; task 1 committed 2026-09-15T16:53:51+08:00, task 2 at 17:07:14+08:00)
- **Started:** 2026-09-15T16:38:00+08:00 (approximate — the first planning reads)
- **Completed:** 2026-09-15T17:08:00+08:00
- **Tasks:** 2
- **Files modified:** 5 (3 phase planning artifacts, 1 new UAT bundle, WINDOWS.md)

## Accomplishments

- **The matrix is finalized.** All five `phase:3` rows read `code_complete: true` with a non-empty `internal_name` resolving to an on-disk class (`RiverSlug`, `MossyThornTurtle`, `GuppyConch`, `VerdantRods`, `GiantDandelion`), and every row now ends with the canonical `localization deferred (D-20); runtime verification outstanding (D-21)` element. The frozen `counts` (rows 31 / phase3 5 / phase4 23 / phase7 3 / deferred 2), `phase3_tranche`, the six-id `texture_complete` set and the nine-id `design_art` set are unchanged. `check-biology.ps1 -RequireAll` exits 0 with `OK(0): phase3 tranche = 5 / 5 (rows=31)` and `OK: implemented classes = 5 / 5`.
- **The mirror is exact.** `03-BIOLOGY.md` was regenerated from the JSON by a generator script (not hand-edited) and a cell-by-cell parity check over all 31 rows × 13 cells reports `PARITY OK`.
- **The ledger is consolidated and machine-consistent.** `03-DEVIATIONS.md` gains §10 (a **14-row blocker register generated mechanically** from the JSON `blockers` arrays, with each **Exact blocker text** cell byte-identical to the matrix, so the two cannot drift — T-03-25), §11 (the Phase 3 close-out: frozen counts, the five implemented rows with their wired and absent drops, the six design-art-only creatures staying in Phase 4, the one boss row with repository art, and the explicit statement that **no item scope was promoted** — five Phase 1–2 item types wired, **four** absent materials blocked), §12 (the D-28/D-29/D-30/D-31 decision disposition) and §13 (the deferred registry plus the phase close-out statement).
- **The whole offline chain is green in one run, and recorded.** `dotnet build /p:Configuration=Release /p:WarningLevel=0` (0 warnings, 0 errors, `Everglow.tmod` packaged and enabled) → `check-biology.ps1 -RequireAll` → the five Phase 1 gates → `check-phase2.ps1` → `dotnet test --filter "FullyQualifiedName~Yggdrasil"` (3 passed, 0 failed) → the AGENTS.md byte-level UTF-8 BOM check (`UTF-8 BOM check passed (1375 files).`) — `CHAIN_EXIT=0`, every link printing its success line, and the whole table recorded verbatim in §8.1 with the triage outcome (no earlier-phase failure; nothing re-anchored, bypassed or silenced — T-03-24).
- **The BOM evidence is honest about its anchor.** §8.2 records that the `git merge-base HEAD origin/master` run is whole-branch and therefore **advisory and non-isolating**, names `check-biology.ps1` invariant 13 (5 phase files) as the binding phase-scoped equivalent, and states that each of the three git stages checks `$LASTEXITCODE` explicitly with no shell pipeline after a git call.
- **The unverified part is explicit.** `03-UAT.md` records the D-21 client bundle as 8 checks (per-row spawn, main-world isolation, behaviour, combat, drops, dedicated-server/multiplayer, localization fallback, and one consolidated end-of-phase run), every one `result: not-executed`, with a header stating the bundle is recorded but not run — so the phase does not present an unverified tranche as verified (T-03-23).
- **Two open defects were recorded rather than hidden.** `WINDOWS.md` gains entry 22 (the unrun D-21 bundle) and entry 23 (the Rule 1 repair of the matrix's machine copy).

## Task Commits

Each task was committed atomically:

1. **Task 1: Reconcile all five tranche rows and consolidate the deviation ledger** — `55df01d07` (docs)
2. **Task 2: Full offline gate chain, Release build and the D-21 client UAT bundle** — `3f942ca0f` (docs)

**Plan metadata:** one further commit (`docs(03-04): complete ...`) carrying this SUMMARY, `STATE.md`, `ROADMAP.md`, `REQUIREMENTS.md`, `state.json` and `WINDOWS.md`; it is the third commit in the `plan_head_before..HEAD` range and its hash is recorded in the completion report.

## Files Created/Modified

- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — the five tranche rows' final blocker disposition (river-slug restated to the plan's exact text plus the D-20/D-21 marker; 荆棘苔龟's drop/effect disposition promoted from the ledger prose), and the corrupted `assumptions[]`/`deviations[]` machine copy restored (6 + 6 entries)
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — regenerated mirror; only the two edited rows and the trailing newline differ
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — header/navigation/coverage claim, §4 machine-copy repair note, §8.1 chain evidence, §8.2 BOM evidence, §10 blocker register, §11 close-out, §12 decision disposition, §13 deferred registry + close-out statement
- `.planning/phases/03-completed-art-ordinary-monsters/03-UAT.md` — **new**: the D-21 client bundle (8 not-executed checks)
- `.planning/WINDOWS.md` — entries 22 (unrun D-21 bundle) and 23 (machine-copy repair)

## Decisions Made

- **The blocker register is §10, not §2.** The plan anticipated the 02-DEVIATIONS.md shape (scope §1, blockers §2). Renumbering §2–§9 would invalidate the section cross-references carried by `STATE.md`, the three prior summaries, the plan texts and the JSON, so §1–§9 keep their numbers, the register is appended as §10, and a navigation line immediately after the coverage claim names all thirteen sections. Documented as a plan-text deviation below.
- **The matrix's machine copy was repaired (Rule 1).** `assumptions[]` and `deviations[]` shipped from the plan-03-01 tracer commit `421f2a09a` as the six-element placeholder `["e", …]`, invisible to every gate, while the ledger's header and §4 name them as the machine copy of the ledger. Leaving them would have made this plan's own coverage claim false, so they now hold one entry per §1–§6 plus the phase's six recorded deviations, and the repair is recorded in §4 and in `WINDOWS.md` entry 23.
- **荆棘苔龟's row blocker was completed.** The plan asked for the other four rows' wave-specific blockers "as written" plus the canonical marker; 03-01 had recorded the row's conservative defaults and its share of the spawn-region gap in the ledger's §4/§6 prose but not in the row. The row now states its drop disposition (荆棘龟壳 wired at 5%), its provisional effect parameters and the marker, so all five rows carry a comparable disposition. The per-row `spawn context partial (D-30)` element stays on 巨树人 only, exactly as §12 states.
- **`REQUIREMENTS.md` is not marked complete.** The traceability table maps BIO-01/02/03 to Phase 4 and BIO-06/QUAL-03/QUAL-04 to Phase 8. Phase 3 implemented the repository-art tranche only — 5 of the 25 designed ordinary creatures (2 of 14 Death Jade Lake rows, 1 of 4 Spiny Moss Court rows, 2 of 7 Valley rows) — so checking those boxes would be false; the file instead gains an explicit Phase 3 advance note and its rows stay Pending.
- **The BOM anchor is reported as advisory.** Rather than presenting the 1375-file whole-branch run as the phase's BOM gate, §8.2 states that invariant 13 over the phase's own change set is the binding check and records the non-isolating anchor explicitly, following the Phase 1 `01-07` and Phase 2 `02-05` precedent.
- **The plan's commit message was used verbatim.** Task 2's `<commit>` text reads "consolidate the phase leader", almost certainly a typo for "ledger"; plan fidelity was preferred over editing a permanent message.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Repaired the corrupted `assumptions[]`/`deviations[]` machine copy of the matrix**
- **Found during:** Task 1 (finalizing `03-BIOLOGY.json`)
- **Issue:** Both arrays held the six-element placeholder `["e", "e", "e", "e", "e", "e"]`. `git show` proved the corruption was present from the plan-03-01 tracer commit `421f2a09a` onward, i.e. it is a pre-existing automation defect — but the file is this task's declared output, no gate invariant reads the arrays, and `03-DEVIATIONS.md`'s header and §4 name them as the machine copy of the ledger. Leaving them would have made this plan's coverage claim and §4 false.
- **Fix:** `assumptions[]` now holds one entry per ledger §1–§6 (tranche rule, discrepancy, mappings, conservative defaults, drop availability, spawn-region gap) and `deviations[]` holds the six deviations the phase actually recorded (the two Rule 1 API-name corrections, the plan-03-02 `HitEffect` placement, the D-23 `RiverSlug` acceptance, the `White_Mod` projectile art deferral, the absent-drop dispositions and the plan-03-03 seam staging). No other field of the matrix changed.
- **Files modified:** `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` (recorded in the ledger's §4 and in `WINDOWS.md` entry 23)
- **Verification:** `node -e "JSON.parse(...)"` reports 31 rows, 6 assumptions, 6 deviations; `check-biology.ps1 -RequireAll` still exits 0; the JSON/MD parity script still reports all 13 cells matching on all 31 rows.
- **Committed in:** `55df01d07` (Task 1 commit)

### Plan-text observations that change no behaviour

**2. The consolidated blocker register is §10, not §2.** See "Decisions Made" above: the register is generated mechanically (as the plan requires) but appended after §9 to preserve the §1–§9 cross-references the rest of the repository cites.

**3. 荆棘苔龟's blocker text is richer than 03-01's row.** The plan's action said the other four rows keep "their wave-specific drop/effect/spawn blockers as written in plans 03-01…03-03"; 03-01 wrote the row's defaults and spawn-gap share in the ledger rather than the row. Promoting them into the row satisfies the plan's `must_haves` ("which designed drops are wired to Phase 1 items … which effect parameters are conservative defaults") and keeps §12's "D-30 applied only to 巨树人" statement true. No new claim is introduced: every clause restates §4, §5 or §6.

**4. The Task 2 commit message reads "phase leader".** Taken verbatim from the plan's `<commit>` element; the intent is "phase ledger".

**5. `REQUIREMENTS.md` rows stay Pending.** The plan's frontmatter lists `requirements: [BIO-01, BIO-02, BIO-03, BIO-06, QUAL-03, QUAL-04]`, which the standard flow would mark complete. The traceability table maps those IDs to Phase 4 (BIO-01/02/03) and Phase 8 (BIO-06/QUAL-03/QUAL-04), and the tranche is a strict subset of each, so the file records the Phase 3 advance instead of a completion it cannot support.

---

**Total deviations:** 1 auto-fixed (Rule 1, corrupted machine-readable field) plus 4 plan-text observations that change no behaviour or scope.
**Impact on plan:** None on scope, intent or observable behaviour. No gameplay, item, tile, buff or localization artifact was added; the build and every gate are green.

## Issues Encountered

- **Console rendering of UTF-8 was misleading three times.** `git diff` output and the chain log displayed CJK and dashes as mojibake in the Windows console even though the files themselves are correct UTF-8. Each apparent corruption was re-verified by reading the file back (the `read` tool) or by re-parsing it as JSON. One *real* corruption was caught this way: a generator script's own literal `§6` and `水蛞蝓` were mangled because PowerShell 5.1 reads a BOM-less script as ANSI, so the generator was rewritten to take all CJK from the JSON and the one affected line was repaired with the edit tool.
- **`git rev-list --count <base>..HEAD` returns `0` inside a compound PowerShell command** and the correct value standalone — the same quirk plan 03-02 recorded. The standalone value (`2`) is used for `actuals.commits`.
- **The AGENTS.md BOM anchor is non-isolating on this branch** (1375 files against a distant pre-phase ancestor). It reported 0 BOM, so no stale-anchor finding was needed; the fact is recorded in §8.2 rather than silently relied upon.
- No fix-attempt limit was reached and no issue was left deferred.

## User Setup Required

None - no external service configuration required.

## Known Stubs

None. This plan adds no code and no placeholder value. The tranche's deliberate omissions are design-sourced and already tracked: the four absent drop materials (软体甲壳碎片, 飞棍毛发, 毒腺, 枯木碎块) produce no type reference at all (D-37/D-39), the two 巨树人 attack projectiles use the shared `Commons.ModAsset.White_Mod` fallback because creating placeholder art is forbidden (`WINDOWS.md` entry 21), and localization is deferred by user directive (D-20). Nothing was marked complete that is not.

## Threat Flags

None. This plan created no network endpoint, auth path, file-access pattern or schema at a trust boundary; it wrote planning artifacts only. The threat register's own mitigations were exercised: T-03-23 (the UAT bundle marks every runtime check not-executed), T-03-24 (no Phase 1/2 gate or artifact was edited; the chain's triage outcome is recorded), T-03-25 (the blocker register is generated mechanically from the JSON and invariant 11 re-asserts parity), T-03-26 (the close-out block states no item scope was promoted) and T-03-27 (the chain's links and exit status are quoted in §8.1).

## Verification

- `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 with 0 warnings / 0 errors, packages `Everglow.tmod` and enables the mod.
- `check-biology.ps1 -RequireAll` exits 0 and prints `OK(0): phase3 tranche = 5 / 5 (rows=31)`, `OK: implemented classes = 5 / 5` and `OK: UTF-8 BOM check passed (5 files).` — the count is the phase change set at the recorded run; re-run after this SUMMARY was written it reports `OK: UTF-8 BOM check passed (6 files).` because the SUMMARY itself joins that set, and the invariant is green in both runs.
- The five Phase 1 gates and the Phase 2 gate each exit 0 with their unchanged success lines; `dotnet test --filter "FullyQualifiedName~Yggdrasil"` exits 0 with 3 passed / 0 failed; the AGENTS.md byte-level UTF-8 BOM check reports `UTF-8 BOM check passed (1375 files).`; the whole chain's `CHAIN_EXIT` is `0` and was re-run identically as the task verify with the same result.
- All five `phase:3` rows read `code_complete: true` with a resolving `internal_name`; the JSON still holds 31 rows and the frozen tranche ids, `texture_complete` set and `design_art` set; the JSON/MD parity script reports `PARITY OK: 31 rows, all 13 cells match`.
- `03-DEVIATIONS.md` holds the coverage claim naming all five rows, the mechanically generated 14-row blocker register, the close-out counts, the decision disposition, the deferred registry and the recorded chain evidence; `03-UAT.md` exists with 8 not-executed entries and a header stating the bundle is recorded but not run.
- No `.png` or other binary asset was added, moved, renamed or modified; no HJSON was created or edited; the Feishu document was neither re-fetched nor mutated; no Phase 1 or Phase 2 artifact was edited; the game was not launched.
- `dotnet build-server shutdown` was run after the final build batch (no MSBuild worker nodes left resident).

## Next Phase Readiness

- **Phase 4's precondition is satisfied.** `03-BIOLOGY.json` is the frozen Phase 3–4 machine source of truth (D-24) with all 23 `phase:4` rows present and labelled (`D-29 shell` on the four empty-design-section rows), and its per-row `blockers` are the exact form Phase 4 will consume: artwork blockers, absent materials, provisional effect parameters and the D-30 spawn-context gap.
- **What Phase 4 must not re-open:** the tranche rule (D-41/D-42), the 31-row id set, the five implemented internal names, and the Phase 1–2 item scope. `check-biology.ps1` re-asserts all of it, and invariant 13 is the phase-scoped BOM guard every later phase should reuse.
- **Outstanding, recorded and unverified:** the D-21 client bundle (`03-UAT.md`, `WINDOWS.md` entry 22), approved art for the two `GiantDandelion` projectiles (entry 21), the 森雨幽谷 / 刺苔庭园 regional spawn predicates (Phase 5–6), localization (D-20, Phase 8) and the four absent drop materials (D-37/D-43).
- **Phase 8 dependency:** every matrix row keeps `status: "unchecked"` and no Feishu colour until both artwork and code are verified; this phase changed no Feishu status (D-25).

## Self-Check: PASSED

- `.planning/phases/03-completed-art-ordinary-monsters/03-UAT.md` exists with 8 not-executed entries.
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` parses with 31 rows, 6 assumptions and 6 deviations; all five phase:3 rows are `code_complete: true`.
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` has 31 rows matching the JSON on all 13 cells.
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` has sections 1–13 with the mechanical blocker register.
- `scripts/check-biology.ps1 -RequireAll` exits 0 with `OK(0): phase3 tranche = 5 / 5 (rows=31)` and `OK: implemented classes = 5 / 5`.
- Commits `55df01d07` and `3f942ca0f` exist on `Yggdrasil/newContent0-ai`.

---
*Phase: 03-completed-art-ordinary-monsters*
*Completed: 2026-09-15*
