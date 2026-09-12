---
phase: 01-item-inventory-completed-art-items
plan: 05
subsystem: content-items
tags: [feishu, inventory, kelp-curtain, items, localization, deferred, user-directive, deviation-ledger, qual-05, tools-classification, phase-gate]

# Dependency graph
requires:
  - plan: 01-03
    provides: "Tranche-A parity, the design 价格 -> Item.value convention, the 43-entry class-less Phase 2 routing, and check-tranche-A.ps1 as the gate template"
  - plan: 01-04
    provides: "Tranche-B parity (4 DevilHeart recipes, Ruin rarities, placeable categories, values), recorded set-bonus/effect blockers, check-tranche-B.ps1, and the deferral of 01-INVENTORY.md regeneration to plan 05"
provides:
  - "Consolidated 01-DEVIATIONS.md deviation ledger: 25 fixed design deviations + 13 deferred localization deviations"
  - "01-INVENTORY.json deviations[] (38 total) and phase1_actions[] (11 total), including P1A-11 recording the user-directed localization deferral"
  - "scripts/check-localization-coverage.ps1 both-culture coverage gate with -AllowMissing advisory mode"
  - "LocalizationCategory Tools on the four Tools/Developer classes (GenerateMazeRoom, ResetIsleOfBloom, ResetKelpCurtain, UnderWaterDungeon)"
  - "Final Phase 1 code gates green: reconciliation, validate-inventory, tranche-A 38/38, tranche-B 20/20, Release build 0 errors, byte-level BOM check, no-placeholder audit"
affects: [02, 03, 04, 08]

# Actuals (#2632) - pairs with the plan's estimate (tokens 40000, tasks 3, confidence low).
actuals:
  tokens: 14028       # chars/4 over the realized diff (56110 chars; 673 insertions / 1 deletion across 8 files)
  tasks: 3
  commits: 3          # MEASURED: git rev-list --count ${plan_head_before}..HEAD
  plan_head_before: 46b63bd3a8f4d7cb8cef723af88e25d730e1e327

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "User-directed deferral is recorded as status=deferred in deviations[] (NOT localization.blocked) so the strict coverage gate stays honestly red and -AllowMissing is the Phase 1 evidence; blocked is reserved for a post-exporter failure with a named missing culture"
    - "Localization keys are exporter-only: when the exporter is not run, no key is fabricated and no HJSON file is hand-edited (AGENTS.md rule + threat-register T-05-01 mitigation)"
    - "Phase gates are measured against the phase base (gsd-plan-head-before-01-01), not origin/master, because this branch is a long-lived feature branch; origin/master comparison would misattribute hundreds of pre-existing binary assets"

key-files:
  created:
    - .planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md
    - .planning/phases/01-item-inventory-completed-art-items/scripts/check-localization-coverage.ps1
  modified:
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/GenerateMazeRoom.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/ResetIsleOfBloom.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/ResetKelpCurtain.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/UnderWaterDungeon.cs

key-decisions:
  - "Localization is deferred for Phase 1 by explicit user directive (2026-09-12, '记录：不考虑本地化，把代码部分完成即可'). The in-game OutputLocalizationHjsonItem exporter was NOT run, no key was fabricated, and no HJSON file was hand-edited."
  - "The 13 Phase 1 completed-art entries missing a class-name key in both cultures are recorded as status=deferred deviations (reason 'localization deferred by user directive (2026-09-12)'), not localization.blocked, so the strict coverage gate remains honestly red and the advisory -AllowMissing baseline (45/58 covered) is the Phase 1 evidence."
  - "Task 2 (checkpoint:human-action) is recorded as an intentional non-execution per the directive; its acceptance criteria are explicitly unmet and tracked, never silently passed."
  - "All Phase 1 CODE gates pass (reconciliation, validate-inventory, tranche-A 38/38, tranche-B 20/20, Release build, BOM, no-placeholder); only the localization coverage gate is advisory."
  - "No binary/art asset changed in the phase (0 of 70 changed files); every phase change is .cs, .ps1, .json, or .md."

requirements-completed: [QUAL-05]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "01-DEVIATIONS.md consolidates the plan 03/04 deviation ledgers plus the four Tools/Developer classification fixes; 01-INVENTORY.json carries deviations[] (38) and phase1_actions[] (11), retaining every entry's advances ITEM IDs."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "check-inventory-reconciliation.ps1 -> OK(0): 103 entries; matched=60; green=58 yellow=20 unchecked=25; labels=5 deferred=21 assumptions=7"
        status: pass
    human_judgment: false
  - id: D2
    description: "The four Tools/Developer classes declare LocalizationCategory Tools and the Release build compiles them with 0 errors."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "grep 'LocalizationUtils.Categories.Tools' on GenerateMazeRoom/ResetIsleOfBloom/ResetKelpCurtain/UnderWaterDungeon; dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 warnings, 0 errors, Everglow.tmod packaged"
        status: pass
    human_judgment: false
  - id: D3
    description: "check-localization-coverage.ps1 exists with -AllowMissing advisory mode; it selects phase==1 + artwork_complete + non-empty internal_name + localization.blocked != true and reports 45 covered / 13 missing of 58 selected."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "powershell -File scripts/check-localization-coverage.ps1 -AllowMissing -> MISSING(13) of 58 selected (45 covered), exit 0"
        status: pass
    human_judgment: false
  - id: D4
    description: "Judgment call: Phase 1 both-culture localization is deferred by user directive, so plan success criterion 3's localization behaviour is intentionally unmet and the strict coverage gate stays red."
    requirement: QUAL-05
    verification: []
    human_judgment: true
    rationale: "The user explicitly directed that localization not be considered for Phase 1; whether to accept the deferred disposition (versus running the exporter or blocking the 13 entries) is a user decision recorded here, not an automation result. The exact 13-entry list is in 01-DEVIATIONS.md and 01-INVENTORY.json deviations[]."
  - id: D5
    description: "Final Phase 1 code gates pass: reconciliation, validate-inventory, tranche-A 38/38, tranche-B 20/20, Release build 0 errors, byte-level UTF-8 BOM check, and no-placeholder (0 binary/art changes in the phase)."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "check-tranche-A.ps1 -> 38/38; check-tranche-B.ps1 -> 20/20; validate-inventory.ps1 -> 103 entries; dotnet build -> 0 errors; AGENTS.md BOM check passed (1295 files); phase binary diff = 0 of 70 changed files"
        status: pass
    human_judgment: false

# Metrics
duration: ~14min
completed: 2026-09-12
status: complete
---

# Phase 1 Plan 05: Localization Parity, Deviation Ledger, and Final Phase Gate Summary

**Consolidated the Phase 1 deviation ledger, classified the four Tools/Developer items, gated the Phase 1 code surface green (reconciliation, tranche-A 38/38, tranche-B 20/20, Release build 0 errors, BOM, no-placeholder) — and recorded both-culture localization as deferred by explicit user directive, with the 13 missing-key entries tracked as `deferred` instead of fabricating keys.**

## Performance

- **Duration:** ~14 min
- **Started:** 2026-09-12T18:31:00+08:00 (approx; plan 01-05 began before this continuation session)
- **Completed:** 2026-09-12T18:54:00+08:00
- **Tasks:** 3 (Task 1 committed by the prior run; Task 2 recorded as a user-directed deferral; Task 3 final gates)
- **Files modified:** 8 (2 created, 6 modified) — all `.cs`/`.ps1`/`.json`/`.md`; 0 binary/art
- **Commits this plan:** 3

## Accomplishments

- **Deviation ledger consolidated (QUAL-05).** `01-DEVIATIONS.md` merges the plan 03/04 deviation ledgers and adds the four `Tools/Developer` category fixes moved from plan 04; `01-INVENTORY.json` now carries `deviations[]` (38 records) and `phase1_actions[]` (11 records). Every entry's `advances` ITEM IDs (`ITEM-01`…`ITEM-04`, `ITEM-07`) are retained for Phase 2, and the 43 class-less entries remain `phase = 2`.
- **Four developer tools classified.** `GenerateMazeRoom`, `ResetIsleOfBloom`, `ResetKelpCurtain`, and `UnderWaterDungeon` declare `LocalizationCategory => LocalizationUtils.Categories.Tools` before any exporter run, so none defaults to `Items`.
- **Both-culture coverage gate authored.** `scripts/check-localization-coverage.ps1` selects the 58 Phase 1 completed-art entries (`phase == 1`, `artwork_complete == true`, non-empty `internal_name`, `localization.blocked != true`) and checks for a class-short-name key in both `en-US` and `zh-Hans` item category files. `-AllowMissing` prints the missing set and exits 0; the strict mode exits non-zero. It never writes an HJSON file.
- **Localization deferral recorded, not hidden.** Per the user directive, the in-game `OutputLocalizationHjsonItem` exporter was not run. The gate reports **45 covered / 13 missing**; the 13 missing entries are recorded as `status: deferred` deviations in `01-DEVIATIONS.md` and `01-INVENTORY.json` (mirrored by `phase1_actions[]` P1A-11), with **no key fabricated and no HJSON file edited**. The strict gate stays honestly red by design; `-AllowMissing` is the Phase 1 evidence.
- **Final code gates green.** `check-inventory-reconciliation.ps1` (103 entries, 103 matrix rows), `validate-inventory.ps1` (103 entries), `check-tranche-A.ps1` (38/38), `check-tranche-B.ps1` (20/20), `dotnet build /p:Configuration=Release /p:WarningLevel=0` (0 warnings, 0 errors, `Everglow.tmod` packaged), and the AGENTS.md byte-level BOM check (1295 files) all pass. The phase changed 70 files and **0** binary/art files.

## Task Commits

Each task was committed atomically:

1. **Task 1: Consolidate the deviation ledger, classify developer tools, add coverage gate** — `310c0f705` (feat) — *committed by the prior run before this continuation*
2. **Task 2: Record the deferred localization deviation (user directive)** — `d54cdbee3` (docs)
3. **Task 3: Mark deferred Phase 1 localization in the inventory matrix** — `aa5b346ba` (docs)

**Plan metadata:** committed separately (docs: complete plan 01-05).

> Note: plan Task 2 was a `checkpoint:human-action` (run the in-game exporter). It was **not executed**; the commit above records the deferral rather than a task completion.

## Files Created/Modified

- `.planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md` (new) — consolidated Phase 1 deviation ledger: 25 fixed design deviations plus the Deferred Localization (User Directive) section listing all 13 missing-key entries.
- `.planning/phases/01-item-inventory-completed-art-items/scripts/check-localization-coverage.ps1` (new) — both-culture key coverage gate; `-AllowMissing` advisory mode; excludes `phase != 1` and `localization.blocked` entries.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` — added 13 `deviations[]` records (`status: deferred`) and `phase1_actions[]` P1A-11; existing entries untouched.
- `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md` — Phase 1 Deviations & Actions section now surfaces the deferral (45/58 covered, 13 deferred, advisory); still exactly 103 matrix rows.
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Tools/Developer/{GenerateMazeRoom,ResetIsleOfBloom,ResetKelpCurtain,UnderWaterDungeon}.cs` — added `LocalizationCategory` `Tools`.

## Decisions Made

- **Deferral over fabrication:** the exporter is the only sanctioned key generator; since the user directed localization out of Phase 1, the correct action is to record the gap as deferred and leave the files untouched rather than hand-create keys (which would violate AGENTS.md and the T-05-01 mitigation).
- **Deferred, not blocked:** `localization.blocked` is reserved by the plan for a post-exporter failure with a named missing culture. Using it here would make the strict gate pass and could be read as claiming completeness; `status: deferred` keeps the gate honestly red and points to the directive.
- **Phase-scoped binary assertion:** the no-placeholder audit is measured against the phase base (`gsd-plan-head-before-01-01`), giving 0 binary changes across 70 files; against `origin/master` the long-lived branch shows hundreds of pre-existing assets that are unrelated to this phase.
- **`01-INVENTORY.md` update:** only the Deviations & Actions prose changed; the 103-row matrix is untouched so the reconciliation row-count gate still holds.

## Deviations from Plan

### User-Directed Deviation

**1. [Rule 4 - Architectural / user-directed] Plan Task 2 (`checkpoint:human-action`) was not executed — localization deferred**
- **Found during:** Task 2 (in-game localization exporter)
- **Issue:** Task 2 required firing the in-game `OutputLocalizationHjsonItem` exporter to additively generate the missing both-culture keys, and Task 3's automated gate (`check-localization-coverage.ps1`, no switch) required zero missing keys. The user directed: "记录：不考虑本地化，把代码部分完成即可" — record it; do not consider localization; just complete the code portion.
- **Resolution:** Did **not** run the exporter; did **not** fabricate keys; did **not** hand-edit any `Localization/**/*.hjson` file. Recorded the 13 missing-key entries as `status: deferred` deviations in `01-DEVIATIONS.md` and `01-INVENTORY.json` `deviations[]`, plus `phase1_actions[]` P1A-11. Task 3 ran `check-localization-coverage.ps1 -AllowMissing` (advisory) and all Phase 1 code gates strictly.
- **Acceptance impact:** Task 2's acceptance (exporter ran; zero missing keys) is **intentionally unmet** and tracked as deferred. Plan success criterion 3's localization behaviour is deferred by user directive; the code portion of criterion 3 is satisfied and gated.
- **Files modified:** `01-DEVIATIONS.md`, `01-INVENTORY.json`, `01-INVENTORY.md`
- **Verification:** `check-localization-coverage.ps1 -AllowMissing` -> 45 covered / 13 missing, exit 0; strict mode -> exit 1 (expected by design)
- **Committed in:** `d54cdbee3`, `aa5b346ba`

---

**Total deviations:** 1 user-directed (localization deferral). No Rule 1-3 auto-fixes were required in this continuation.
**Impact on plan:** The Phase 1 code surface is fully gated and green; the localization deliverable is explicitly deferred and visible, never silently passed.

## Known Stubs

| Stub | File | Reason |
|------|------|--------|
| 13 completed-art items lack a display key in both `en-US` and `zh-Hans` item category HJSON files: `EvilHalbertBarnacle`, `ArcI`, `RedAlgaeMagicStaff`, `RedAlgaeMagicSpellBook`, `RedAlgaeMagicWhip`, `CrimsonMoonSap`, `EmptyWaterStaff`, `JadeLakeRedAlgae_Item`, `Photophore`, `GreenSungloStaff`, `ActivatedDogStaff`, `RedAlgaeMinionGyroscope`, `RedAlgaeMinionStaff` | `Sources/Everglow/Localization/{en-US,zh-Hans}/Mods.Everglow.Items.*.hjson` (keys absent) | Localization deferred by user directive (2026-09-12). Recorded as `deferred` deviations in `01-DEVIATIONS.md` and `01-INVENTORY.json`; the in-game exporter is the only sanctioned generator and was not run. Resolve in Phase 2 (or when the user elects to run the exporter). |

## Threat Flags

| Flag | File | Description |
|------|------|-------------|
| threat_flag: none | - | No new network endpoint, auth path, file-access pattern, or trust-boundary schema change was introduced. The only new executable is a read-only PowerShell gate; no key was fabricated, no Feishu document was mutated, and no binary/art asset was added or modified. |

## Issues Encountered

- The Windows console decodes UTF-8 output as the ANSI code page, so Chinese identifiers appear mojibake in terminal output (e.g. the GreenSunglo id `item-weapons.misc-青须手杖`). Reads/edits via the UTF-8-aware tools and `[IO.File]::ReadAllText` handled the content correctly; the JSON parses and the reconciliation gate confirms 103 rows.
- The first binary/no-placeholder scan compared against `origin/master` and reported 210 existing assets because this is a long-lived feature branch. Re-scoped to the phase base (`gsd-plan-head-before-01-01`): 0 binary changes across 70 phase files. Documented as a decision to avoid a false positive.

## User Setup Required

None — no external service, package, or secret. No Feishu write was made.

## Next Phase Readiness

- **Ready:** the Phase 1 code surface is gated green (reconciliation, validate-inventory, tranche-A 38/38, tranche-B 20/20, Release build, BOM, no-placeholder); the deviation ledger and inventory are consistent and committed.
- **Phase 2 hand-off:** the 43 class-less entries routed in plan 03 remain `phase = 2`; the two repo-backed art-incomplete entries `item-weapons.misc-radial-carapace` (RadialCarapace) and `terrain-weapons.magic-vinerepairwand` (VineRepairWand) remain `phase = 1` tracked blockers per D-11 with their design deviations queued to Phase 2 per D-12 — they were **not** routed. Every entry's `advances` ITEM IDs remain visible for Phase 2.
- **Deferred/carried blockers:** 13 deferred localization entries (this plan); the tranche-A effect blockers (MossySpell, CyatheaArrow, GreenSungloStaff, EvilHalbertBarnacle); the tranche-B set-bonus/effect blockers (Photophore, WitherbarkHelmet, ShellMolluscsBreastPlate, RuinMask); and the RadialCarapace/VineRepairWand artwork blockers.
- **Not verified locally:** actual in-client display of the localized names/tooltips (the plan's backstop truth) — the exporter was not run and no client session was performed, by user directive.

---

*Phase: 01-item-inventory-completed-art-items*
*Completed: 2026-09-12*

## Self-Check: PASSED

- All declared files exist: `01-DEVIATIONS.md`, `scripts/check-localization-coverage.ps1`, `01-INVENTORY.json` (38 deviations / 11 phase1_actions), `01-INVENTORY.md` (103 matrix rows), and the four `Tools/Developer` classes.
- Task commits `310c0f705`, `d54cdbee3`, `aa5b346ba` exist in git history.
- `check-inventory-reconciliation.ps1`, `validate-inventory.ps1`, `check-tranche-A.ps1`, `check-tranche-B.ps1`, and `check-localization-coverage.ps1 -AllowMissing` exit 0; Release build 0 errors; BOM check passed; 0 phase binary changes.
