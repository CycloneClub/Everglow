---
phase: 04-remaining-ordinary-monsters
plan: 09
subsystem: planning-artifacts
tags: [tmodloader, terraria, planning, close-out, reconciliation, gate-chain, uat, d21, biology-matrix, yggdrasil, kelp-curtain]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    provides: the 13-invariant Phase 4 gate (`scripts/check-biology.ps1`), the frozen 21-row in-scope set (D-44), the shared `KelpCurtainSpawnConditions` seam and the phase ledger `04-DEVIATIONS.md` sections 1-13 with section 14 reserved for this close-out (plan 04-01)
  - phase: 04-remaining-ordinary-monsters
    provides: the 26 region-folder `ModNPC` classes and eleven enemy projectiles created by plans 04-02 to 04-08, plus their register items (conservative readings, D-54 defaults, literal-token and locality records) recorded in each SUMMARY and in `.planning/WINDOWS.md`
provides:
  - the reconciled shared matrix `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` (21 / 21 in-scope rows `code_complete: true` with resolving `internal_name` and final blocker arrays — 83 blocker elements) and its regenerated mirror `03-BIOLOGY.md`
  - `.planning/phases/04-remaining-ordinary-monsters/04-DEVIATIONS.md` section 14 - the mechanical Blockers table (14.1), the close-out counts (14.2), the decision disposition (14.3), the offline chain results (14.4), the consolidated deferred registry (14.5) and the client-verification pointer (14.6) - plus the section 5 / 6.2 / 7 extensions and the new section 10.1 register
  - `.planning/phases/04-remaining-ordinary-monsters/04-UAT.md` - the D-21 client bundle (24 checks, all not executed)
  - the completed `.planning/phases/04-remaining-ordinary-monsters/04-VALIDATION.md` (23-row per-task verification map, Wave 0 and sign-off completed, `nyquist_compliant: true`)
affects: [phase-8-source-acceptance, phase-5-6-region-terrain]

# Actuals (#2632) - pairs with the plan's estimate (52000 tokens, 2 tasks) to calibrate future estimates.
# chars/4 over the added lines of the plan's two task commits (132529 chars / 4), the 04-08 basis, never a harness token count.
actuals:
  tokens: 33132
  tasks: 2
  commits: 3   # measured: git rev-list --count d532b76b2..HEAD after the plan-metadata commit (2 task commits + 1 plan-metadata commit)
plan_head_before: d532b76b2a306fda6201bdab8ae39e73abe535a3

tech-stack:
  added: []
  patterns:
    - "A formatting-preserving textual edit of a machine-readable matrix (never a JSON round-trip), followed by a field-by-field diff against the plan-head revision, is what makes 'only the intended fields changed' a checked fact rather than a claim"
    - "A ledger table generated from the machine source and then diffed back against it in the same task turns 'the ledger cannot drift from the matrix' into a measurable 83-of-83 byte comparison (T-04-57)"
    - "A close-out that must not renumber a widely cited document puts its new sections where that document reserved them and carries the plan's expected headings as subsections, so no existing citation is invalidated"
    - "A per-task verification map generated mechanically from the plan files (count the `<task>` elements, quote each `<automated>` and `<fails_when>` verbatim) keeps the validation contract and the plans in sync by construction"

key-files:
  created:
    - .planning/phases/04-remaining-ordinary-monsters/04-UAT.md
  modified:
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md
    - .planning/phases/04-remaining-ordinary-monsters/04-DEVIATIONS.md
    - .planning/phases/04-remaining-ordinary-monsters/04-VALIDATION.md

key-decisions:
  - "All 21 in-scope rows were reconciled by a formatting-preserving textual edit (never a JSON round-trip) plus a regenerated Markdown mirror, so a field-by-field diff against the plan-head JSON shows zero changes to phase / region / name_zh / name_en / repo_asset / design_art / texture_complete / status / frame_count / mapping_confidence / deferred / deferred_reason / feishu, and exactly 20 internal_name changes, 20 code_complete changes and 20 rewritten blocker arrays (the 21st, 碧灵鮟鱇, re-emits byte-identically)"
  - "The frozen counts (31/5/23/3/2/6/9), the 5-id phase3_tranche, the 6-id texture_complete set and the 9-id design_art set are unchanged, the two hardmode rows stay deferred with a non-empty reason, and every row's status stays unchecked - no Feishu colour is written here (D-22/D-25)"
  - "The plan's task enumerates the elements each row's blockers array must carry; the seeded arrays already held several of them with slightly different wording, so the arrays were rebuilt from the enumeration. Where the plan gives a literal (the D-45 marker, the OQ1 element, the region element, the ink-cloud and water-surface effect elements, the summoned-soldier element), the literal is used verbatim; where a seeded element carried extra facts (the morale bonus values, the summon-and-buff detail, the egg-breaking small variant), the element was rewritten so the plan's literal is its prefix and the facts survive"
  - "Two rows' combined drop elements were split into one element per blocked drop (毒腺 and 牛黄 separately on 剧毒蟾蜍 and 幽光蝾螈, 软体甲壳碎片 and 亡碧膏 separately on the octopuses and the radiolarian), and the three variant rows gained an explicit sibling-class element so the record of LargeBombJellyfish, SmallBrodieFlydragon, AnimatedWitherbarkSoldierRanged, AnimatedWitherbarkSoldierSpell and AnimatedWitherbarkHound is not lost behind the matrix's one-internal_name-per-row schema"
  - "The region gap's matrix wording was finalised to ONE literal on three rows - 'region: 森雨幽谷 / 刺苔庭园 spawn predicate not implemented (Phases 5-6; D-52)' on 覆藻章鱼, 大型覆藻章鱼 and 布罗迪蝇蜓 - replacing the planning-time draft that named a single sub-region; section 5 was extended to quote the final text and to record that 红针洋辣子's Valley split stays a section note rather than a row element"
  - "大型覆藻章鱼's 武器与饰品掉落待定 list got its own drop element instead of riding on the two material elements, and section 6.2's cell for it was updated to point at that element"
  - "SECTION NUMBERING: the plan's Task 1 asks for new sections 11-15 (Blockers, close-out, decision disposition, deferred registry, client verification), but plan 04-01's ledger already uses 11 (runtime verification), 12 (localization) and 13 (deferred registry), and dozens of wave-plan, SUMMARY, WINDOWS.md and STATE.md records cite sections 5, 6, 7, 8, 10, 11 and 13 by number. The close-out therefore lives in section 14 (the section 04-01 reserved for it) as subsections 14.1 Blockers, 14.2 close-out counts, 14.3 decision disposition, 14.4 chain results, 14.5 deferred registry and 14.6 client verification, so every heading the plan names exists and no existing citation is invalidated. Recorded as a documented structure deviation"
  - "The section 14.1 Blockers table is generated mechanically from 03-BIOLOGY.json (83 rows, one per blocker element, with a kind derived from the element prefix) and was then diffed back against the JSON: 83 of 83 'exact blocker text' cells are byte-identical to the element they mirror, which is the T-04-57 mitigation made checkable"
  - "04-VALIDATION.md's per-task map was generated mechanically from the nine plans: the script counts each plan's `<task>` elements (23, matching the plan's stated 04-01 x2, 04-02 x2, 04-03 x3, 04-04 x3, 04-05 x3, 04-06 x3, 04-07 x3, 04-08 x2, 04-09 x2) and quotes each task's automated command and fails-when verbatim (only newlines/tabs/space runs collapsed, and a literal pipe escaped); nyquist_compliant flips to true and the frontmatter status stays draft, because the /gsd-validate-phase section 6 step owns the promotion to validated"
  - "04-UAT.md records the D-21 bundle as recorded but NOT executed: 24 checks (one per in-scope row plus isolation, dedicated-server/multiplayer and localization), every one marked not-executed, with the mini boss's two edge cases named explicitly (a target lost mid-flight, and a melee reflect while the shell is closed) so the tranche is never presented as client-verified (T-04-55/T-04-58)"
  - "The AGENTS.md byte-level BOM check is advisory and non-isolating on this branch (its 'git merge-base HEAD origin/master' anchor predates the whole phase, so it measures 1440 files); the binding phase-scoped equivalent is the Phase 4 gate's invariant 13, which ran first over the phase's own change set. Its three git stages each check $LASTEXITCODE explicitly and no git call is followed by a pipeline, so a git failure aborts rather than passing as an empty change set"
  - "REQUIREMENTS.md: BIO-01, BIO-02 and BIO-03 stay Pending and a Phase 4 close-out advance paragraph is added instead, because the plan states that their requirement rows and Phase 8's source synchronization remain for the final phase; no Feishu status is written (D-25)"
  - "No earlier-phase gate or artifact was edited and no item scope was promoted into the phase (D-57/D-58); the Phase 3 gate is still byte-identical and still reproduces 'OK(0): phase3 tranche = 5 / 5 (rows=31)'"

patterns-established:
  - "A close-out whose whole job is reconciliation should prove the reconciliation: the matrix edit is diffed field-by-field against the plan head, and the generated ledger table is diffed back against the JSON, so 'nothing else changed' and 'the ledger cannot drift' are both machine-checked in the same task"
  - "When a plan's expected section numbers collide with a document's existing, widely cited numbers, keep the citations valid and place the new content in the reserved section under the expected headings"

requirements-advanced: [BIO-01, BIO-02, BIO-03]

coverage:
  - id: D1
    description: "All 21 in-scope phase:4 rows reconciled: code_complete true, a namespace-qualified internal_name resolving to an on-disk class under Sources/Modules/Yggdrasil/KelpCurtain/NPCs, and a final blockers array (83 elements total) carrying the D-48 artwork element, the row's disposition elements and the canonical localization/runtime tail; 03-BIOLOGY.md regenerated so its 31 rows, ids, statuses and blocker cells mirror the JSON"
    requirement: BIO-01
    verification:
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 -RequireAll (exit 0; 'OK(0): phase4 in-scope set = 21 rows (rows=31)', 'OK: reconciled rows = 21 / 21', 'OK: guarded classes = 37', 'OK: UTF-8 BOM check passed (4 files).')"
        status: pass
      - kind: other
        ref: "field-by-field diff of the final JSON against the plan-head revision: 0 unexpected field diffs; 20 internal_name + 20 code_complete + 20 rewritten blockers arrays; frozen counts, phase3_tranche, texture_complete and design_art identical; every status still unchecked"
        status: pass
      - kind: other
        ref: "markdown mirror audit: 31 rows, every row 13 cells, ids and per-row status identical to the JSON (the gate's invariant 11 re-asserts the same on every run); both files BOM-free and LF"
        status: pass
    human_judgment: false
  - id: D2
    description: "04-DEVIATIONS.md consolidated: coverage claim and sections 1-13 preserved and extended (the region wording in 5, the TBD drop cell in 6.2, the filed effect blockers in 7, and the new 10.1 wave-2/3 register), then 14.1 Blockers (83 rows), 14.2 close-out counts with the explicit no-item-scope-promoted statement, 14.3 decision disposition (D-44 to D-60), 14.4 chain results, 14.5 deferred registry and 14.6 client-verification pointer"
    requirement: BIO-02
    verification:
      - kind: other
        ref: "14.1 table diffed back against the JSON: 83 of 83 exact-blocker-text cells byte-identical (Compare-Object reports no difference)"
        status: pass
      - kind: other
        ref: "ledger is BOM-free and LF; every CJK run in the file also occurs in the committed JSON / plan / SUMMARY corpus (no invented design names)"
        status: pass
    human_judgment: false
  - id: D3
    description: "The complete offline chain ran in one pass with every link green: Release build, Phase 4 gate -RequireAll, Phase 3 gate -RequireAll, the five Phase 1 gates, the Phase 2 gate, the Yggdrasil MSTest filter and the AGENTS.md byte-level BOM block, then dotnet build-server shutdown"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exit 0; build succeeded, 0 warnings / 0 errors, Everglow.tmod packaged and the mod enabled)"
        status: pass
      - kind: other
        ref: "the 04-09 Task 2 automated verify, run verbatim as a single command (exit 0): Phase 4 gate OK, Phase 3 gate OK, Phase 1 gates 4-8 OK, Phase 2 gate OK, 'dotnet test --filter FullyQualifiedName~Yggdrasil' 3 passed / 0 failed / 0 skipped, 'OK: 04-UAT.md present, token-complete, BOM-free', 'OK: 04-VALIDATION.md present, 23 rows match 23 plan tasks, BOM-free'"
        status: pass
      - kind: other
        ref: "AGENTS.md BOM block: anchor f222f9316508c71658a331a1c309776fd862fa2d, 'UTF-8 BOM check passed (1440 files).', no BOM-prefixed file; its three git stages each assert $LASTEXITCODE"
        status: pass
      - kind: other
        ref: "no earlier gate or artifact edited: the Phase 1/2 gate scripts and artifacts and the Phase 3 gate script are byte-identical, and git status in the plan range shows only the five phase-4/phase-3 planning files"
        status: pass
    human_judgment: false
  - id: D4
    description: "The outstanding client-side verification of the 21-row tranche is captured as an explicit unexecuted bundle (04-UAT.md, 24 checks) and the validation strategy is closed (04-VALIDATION.md, 23-row per-task map, nyquist_compliant true)"
    requirement: BIO-03
    verification:
      - kind: other
        ref: "04-UAT.md: 24 '### N.' entries, 24 'result: not-executed' lines, token-complete for all 24 required class/scope tokens, BOM-free"
        status: pass
      - kind: other
        ref: "04-VALIDATION.md: the gate-verify contract passes (tokens present, nyquist_compliant: true in the frontmatter, 23 table rows equal to the 23 counted <task> elements, BOM-free)"
        status: pass
    human_judgment: true
    rationale: "Every check inside 04-UAT.md is a runtime property no offline artifact observes (D-21); the bundle is recorded so the tranche is never presented as client-verified, and Phase 8's source synchronization remains outstanding (QUAL-04)."

duration: ~28min
completed: 2026-09-16
status: complete
---

# Phase 4 Plan 09: Phase Close-Out — Matrix Reconciliation, Consolidated Ledger, Full Chain and the D-21 Client Bundle Summary

**The phase's machine record caught up with its code in one audited pass: 21 / 21 in-scope rows reconciled across 26 classes, one mechanically generated ledger that diffs byte-for-byte against the matrix, and an eleven-link offline chain green in a single run while the client and dedicated-server checks are recorded, not claimed**

## Performance

- **Duration:** ~28 min (from the plan-start HEAD `d532b76b2` to the plan-metadata commit)
- **Started:** 2026-09-16T18:21:42Z
- **Completed:** 2026-09-16
- **Tasks:** 3 of 3 (all `<task type="auto">`; no checkpoint was reached)
- **Files modified/created:** 5

## Task Commits

| Task | Name | Commit | Files |
| --- | --- | --- | --- |
| 1 | Reconcile all 21 in-scope rows and consolidate the deviation and blocker ledger | `dae72b4fa` | `03-BIOLOGY.json`, `03-BIOLOGY.md`, `04-DEVIATIONS.md` |
| 2 | Full offline gate chain, Release build and the D-21 client UAT bundle | `79a63a4f3` | `04-UAT.md`, `04-VALIDATION.md`, `04-DEVIATIONS.md` |
| 3 | Plan metadata (this SUMMARY plus STATE/ROADMAP/REQUIREMENTS) | pending at write time | `04-09-SUMMARY.md`, `STATE.md`, `ROADMAP.md`, `REQUIREMENTS.md` |

## What the Close-Out Actually Did

### 1. The matrix caught up with the code (D-24)

`03-BIOLOGY.json` now reports `code_complete: true` for all 21 non-deferred `phase: 4` rows, each with a namespace-qualified `internal_name` that resolves to a real file under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs` (26 classes: 21 row owners plus the five stat-variant siblings). The edit was a **textual** replacement of exactly three fields per row (`internal_name`, `code_complete`, `blockers`) followed by a regenerated Markdown mirror, and it was then diffed against the plan-head revision: only the intended fields moved. The frozen `counts` block, the 5-id `phase3_tranche`, the 6-id `texture_complete` set and the 9-id `design_art` set are untouched, both hardmode rows stay `deferred`, and all 31 rows stay `status: "unchecked"` — no Feishu colour is written here (Phase 8 owns synchronization).

The 21 arrays were rebuilt from the plan's enumeration. Seven rows changed shape; the rest changed only wording. Every array starts with the D-48 artwork literal and ends with `localization deferred (D-20); runtime verification outstanding (D-21)`. Between them: the D-45 behaviour marker on the three identity shells; one `drop:` element per blocked drop; the `capture:` elements on the three un-capturable rows; the morale/command, egg-system and disguised-hazard `system:` elements; the single finalised `region:` literal on the three affected rows; the ink-cloud and derived-surface `effect:` elements; the OQ1 `CaterpillarJuice` element; and explicit sibling-class elements so the variant record is not lost behind the schema's one-`internal_name`-per-row shape. 83 blocker elements in total.

### 2. One ledger, generated from the matrix and diffed back against it (T-04-57)

`04-DEVIATIONS.md` keeps the coverage claim and sections 1-13 (extended where the waves changed the facts: §5's region wording, §6.2's TBD cell, §7's filed effect blockers) and adds section 14 with 14.1 Blockers (83 rows: a kind, the exact text, and a mechanical *why*), 14.2 the close-out counts including the explicit **no item scope was promoted** statement, 14.3 the D-44…D-60 disposition table, 14.4 the chain results, 14.5 the consolidated deferred registry and 14.6 the `04-UAT.md` pointer. Every `exact blocker text` cell was diffed back against the JSON it mirrors: **83 / 83 byte-identical**.

The wave plans' register items — the 装甲虾 knockback default, the 爆弹水母 shallow/deep liquid-column approximation, the 帆鳍鳢 aggro window, the 放射虫 squared-distance trigger, the 大型覆藻章鱼 grab sync path, the three shells' defaults, the CourtCommander summon contract, the flydragon `NPC.value` snapshot correction, the mini boss's reflect locality and every geometry constant, the `NPC.GetSource_FromAI` token realisation, the netmode-guard normalisations and the `tdd="true"` realisation — are filed in the new §10.1 so the phase's conservative record is single-sourced (QUAL-05).

### 3. One green chain, and the client bundle that is deliberately not claimed (D-21)

The complete offline chain ran in one pass and **every link exited 0**: the Release build (0 warnings / 0 errors, `Everglow.tmod` packaged), the Phase 4 gate `-RequireAll` (`21 rows`, `reconciled rows = 21 / 21`, `guarded classes = 37`), the Phase 3 gate `-RequireAll` (`5 / 5`, `implemented classes = 5 / 5` — still reproducing on the byte-identical script), the five Phase 1 gates, the Phase 2 gate, `dotnet test --filter "FullyQualifiedName~Yggdrasil"` (3 passed / 0 failed) and the AGENTS.md byte-level BOM block (1440 files, no BOM). `dotnet build-server shutdown` was run afterwards. No triage was needed and no earlier-phase artifact was touched.

`04-UAT.md` records the 24 checks the offline chain cannot reach — per-row spawn band, isolation, behaviour, combat, drops, plus dedicated-server/multiplayer and localization — every one marked not executed, with the mini boss's target-lost-mid-flight and retracted-reflect cases named explicitly. `04-VALIDATION.md` is completed with the 23-row per-task map generated from the plans, its Wave 0 list and sign-off ticked and `nyquist_compliant: true`; its `status` stays `draft` for the `/gsd-validate-phase` step.

## Deviations from Plan

### Documented structure deviation

**1. [Rule 4 - structure] The ledger's close-out content lives in section 14, not sections 11-15**

- **Found during:** Task 1 (the plan's `read_first` assumed plan 04-01 "reserved section 11 for blockers")
- **Issue:** The plan's Task 1 asks for new sections 11 (Blockers), 12 (close-out), 13 (decision disposition), 14 (deferred registry) and 15 (client verification). Plan 04-01's ledger already uses 11 (runtime verification, D-21), 12 (localization, D-20) and 13 (deferred registry), and its navigation line and its own section-14 text say section 14 is the reserved close-out. More importantly, dozens of existing records cite the ledger by number — `04-DEVIATIONS.md` section 5, 6, 7, 8, 10, 11 and 13 appear across the nine plans, eight SUMMARYs, `WINDOWS.md` and `STATE.md` — so renumbering would have silently invalidated all of them.
- **Fix:** Section 1-13 keep their numbers and content; the close-out content is placed in section 14 (the reserved section) as subsections **14.1 Blockers**, **14.2 Phase 4 close-out counts**, **14.3 Decision disposition**, **14.4 Offline chain results**, **14.5 Consolidated deferred registry** and **14.6 Client verification**, so every heading the plan names exists. The navigation line and a "section numbering note" at the top of section 14 record the choice.
- **Files modified:** `.planning/phases/04-remaining-ordinary-monsters/04-DEVIATIONS.md`
- **Commit:** `dae72b4fa`
- **Impact:** none on the acceptance criteria; a reader following "section 11" still lands on runtime verification, which is where that citation means to go.

### Plan-superset changes (same instruction, wider application)

**2. [Rule 2 - completeness] The region element was extended to all three affected rows and its wording finalised**

- **Found during:** Task 1
- **Issue:** The plan names the region element on 覆藻章鱼 and 布罗迪蝇蜓; the ledger's section 5 (written by 04-01) records it on 覆藻章鱼 and 大型覆藻章鱼 as well. The two wordings also differed (the planning-time draft named one sub-region; the plan's literal names both).
- **Fix:** One finalised literal — `region: 森雨幽谷 / 刺苔庭园 spawn predicate not implemented (Phases 5-6; D-52)` — is carried by all three rows, and section 5 was extended to quote the final text and to record that 红针洋辣子's Valley split stays a section note.
- **Files modified:** `03-BIOLOGY.json`, `03-BIOLOGY.md`, `04-DEVIATIONS.md` §5
- **Commit:** `dae72b4fa`

**3. [Rule 2 - completeness] The 大型覆藻章鱼 TBD weapon/accessory list got its own drop element**

- **Found during:** Task 1
- **Issue:** Section 6.2 claimed the 武器与饰品掉落待定 list was "covered by its own 软体甲壳碎片 / 亡碧膏 D-58 element", while the plan's table lists it as a blocked drop that needs "one `drop:` element per blocked drop".
- **Fix:** The row now carries `drop: 武器与饰品掉落待定 (weapon/accessory drop list TBD) not implemented (item scope; D-58)`, and section 6.2's cell was updated to point at it.
- **Files modified:** `03-BIOLOGY.json`, `03-BIOLOGY.md`, `04-DEVIATIONS.md` §6.2
- **Commit:** `dae72b4fa`

### Execution notes (not deviations)

- **The plan's `est. 52000 tokens` vs the measured 33132** — the close-out's cost is dominated by one-off artifact text (the 83-row ledger table, the 23-row verification map), which is why the estimate's `confidence: low` was right.
- **Two generator-script defects were caught and fixed inside the same task, before either was committed**, both by re-deriving from the pristine plan-head JSON rather than from the already-edited file: a PowerShell `return , @(a, b)` unwrapping bug that concatenated split drop elements, and a `(GetParts ...)[0]` indexing bug that reduced two `system:` elements to the single character `s`. Both were detected by a post-write assertion (an array-length diff and a "no blocker shorter than 10 characters" scan) rather than by the gate, which only requires an `artwork` element — recorded here because the gate alone would not have caught them.
- **A UTF-8/ANSI script-encoding defect was caught the same way**: a BOM-less generator script containing CJK literals is decoded by PowerShell 5.1 as the system ANSI code page, which mangled the chain results in section 14.4; that block was rewritten ASCII-only. This is the same reason the phase's gate script is deliberately 100% ASCII.

## Authentication Gates

None. No auth gate, network fetch or credential was needed; no package was installed (04-RESEARCH §Package Legitimacy Audit: not applicable), and the Feishu design source was neither re-fetched nor mutated (D-25).

## Known Stubs

None introduced. This plan creates no code: it edits two planning matrices, one ledger and the validation contract, and creates the UAT bundle. The deliberate gaps the phase carries (26 + 11 missing sprites, the absent drop materials and capture items, the three unimplemented systems, the region predicates, the deferred localization and the unrun client verification) are recorded as blockers in `03-BIOLOGY.json`, as entries in `04-DEVIATIONS.md` §14.1/§14.5, and as 24 unexecuted checks in `04-UAT.md` — they are the phase's documented residue, not stubs hidden in this plan.

## Threat Flags

None. No new endpoint, auth path, file-access pattern or trust-boundary schema change is introduced: the plan's entire change set is five `.planning/` artifacts, and it verified (by diff) that no binary asset, HJSON file, earlier-phase artifact or earlier-phase gate script was touched.

## Verification Evidence

| Check | Command | Result |
| --- | --- | --- |
| Task 1 automated verify | `check-biology.ps1 -RequireAll` (Phase 4) then the Phase 3 gate | exit 0 - `21 rows` / `reconciled rows = 21 / 21` / `guarded classes = 37`; `phase3 tranche = 5 / 5 (rows=31)` / `implemented classes = 5 / 5` |
| Task 2 automated verify (verbatim, one command) | the plan's `<automated>` block | exit 0 - `VERIFY-OK: every link in the 04-09 Task 2 automated verify is green` |
| Release build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | exit 0 - 0 warnings / 0 errors, `Everglow.tmod` packaged |
| Yggdrasil tests | `dotnet test --filter "FullyQualifiedName~Yggdrasil"` | exit 0 - 3 passed, 0 failed, 0 skipped |
| Phase-scoped BOM guard | Phase 4 gate invariant 13 | `OK: UTF-8 BOM check passed (4 files).` |
| AGENTS.md BOM block | `git merge-base HEAD origin/master` anchor | `UTF-8 BOM check passed (1440 files).` (advisory, non-isolating) |
| Matrix integrity | field diff vs the plan-head JSON | 0 unexpected field diffs |
| Ledger/matrix parity | 14.1 table vs the JSON `blockers` arrays | 83 / 83 byte-identical |
| Validation contract | the Task 2 verify's 04-VALIDATION.md block | `23 rows match 23 plan tasks, BOM-free` |
| UAT bundle | the Task 2 verify's 04-UAT.md block | `present, token-complete, BOM-free` |
| Build servers | `dotnet build-server shutdown` | exit 0 - MSBuild and VB/C# compiler servers shut down |

## Handoff to the Phase Gate

- **Requirement rows:** BIO-01, BIO-02 and BIO-03 are complete on the code side for the whole ordinary-creature tranche. Their `REQUIREMENTS.md` traceability rows stay **Pending** and a Phase 4 close-out advance paragraph is added, because the plan states that their requirement rows and Phase 8's source synchronization remain for the final phase; QUAL-03/QUAL-04 keep the runtime and Feishu halves (Phase 8).
- **`progress.completed_phases` stays at 3** — Phase 4's code is complete but its verification is outstanding, so the phase is not closed out as a completed phase here.
- **The next step is the phase gate:** `/gsd-verify-work` for Phase 4 (the `/gsd-validate-phase` step then promotes `04-VALIDATION.md` from `draft` to `validated`), followed by Phase 5.
- **Nothing here writes a Feishu status (D-25).** Every row remains `unchecked` until Phase 8 synchronizes the design source.

## Self-Check: PASSED

**Files verified present:** `03-BIOLOGY.json`, `03-BIOLOGY.md`, `04-DEVIATIONS.md`, `04-VALIDATION.md`, `04-UAT.md`, `04-09-SUMMARY.md` — all FOUND.

**Commits verified present:** `dae72b4fa` (Task 1), `79a63a4f3` (Task 2) — both FOUND in `git log --all`.

**Chain re-run after the metadata edit:** the plan's Task 2 automated verify was executed verbatim as a single command and printed `VERIFY-OK: every link in the 04-09 Task 2 automated verify is green` with exit 0; the Task 1 verify (both `check-biology.ps1 -RequireAll` runs) also exits 0.
