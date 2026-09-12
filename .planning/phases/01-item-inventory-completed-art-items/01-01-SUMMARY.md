---
phase: 01-item-inventory-completed-art-items
plan: 01
subsystem: planning-artifacts
tags: [feishu, lark-cli, docxxml, inventory, powershell, parser, validator, tracer]

# Dependency graph
requires: []
provides:
  - "Committed well-formed XML evidence snapshots of the three authoritative Feishu design docs (block IDs + checkbox state)"
  - "scripts/parse-design-xml.ps1 — deterministic, header-anchored, rowspan-aware DocxXML -> inventory parser"
  - "scripts/validate-inventory.ps1 — reusable offline inventory gate (exit 2/3/4)"
  - "scripts/test-parser.ps1 — fixture regression test locking column/status rules"
  - "01-INVENTORY.json — machine source of truth skeleton (103 entries: item 89, biology_drop 13, terrain 1)"
  - "01-INVENTORY.md — human-readable mirror, one row per JSON entry"
affects: [01-02, 01-03, 01-04, 01-05, 02, 03, 04, 05, 08]

# Actuals (#2632) — pairs with the plan's estimate to calibrate future estimates.
actuals:
  tokens: 133058
  tasks: 3
  commits: 3
  plan_head_before: 5c025ff7ee0e8984c39f803c42aa709750706c4a

tech-stack:
  added: []
  patterns:
    - "Committed design-source evidence: fetch once, commit raw XML, parse offline and reproducibly"
    - "Header-anchored table parsing (thead or detected first tbody row) with rowspan/colspan logical-grid expansion"
    - "ASCII-only PowerShell scripts with .NET regex \\uXXXX escapes to survive PowerShell 5.1 BOM-less ANSI decoding"

key-files:
  created:
    - .planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml
    - .planning/phases/01-item-inventory-completed-art-items/evidence/item.xml
    - .planning/phases/01-item-inventory-completed-art-items/evidence/terrain.xml
    - .planning/phases/01-item-inventory-completed-art-items/evidence/sources.json
    - .planning/phases/01-item-inventory-completed-art-items/scripts/parse-design-xml.ps1
    - .planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1
    - .planning/phases/01-item-inventory-completed-art-items/scripts/test-parser.ps1
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json
    - .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.md
  modified: []

key-decisions:
  - "Evidence snapshots are committed wrapped in a single <fragment> root so they are well-formed XML (raw lark-cli full-detail content is a multi-root fragment)."
  - "Completion status is derived only from the 贴图/代码 checkbox `done` attributes; rgb(217,245,214)/rgb(255,255,204) fills are classified but never used to compute status."
  - "Only item/armour/drop name tables (物品名/护具/掉落物/名字 headers) emit entries; creature-stat and terrain-tile tables are iterated for parse_audit only."
  - "A non-empty texture_checkbox_id is required unless the entry records a blocker, because real design rows exist with resolvable columns but unfilled checkbox cells."
  - "source_kind is doc-scoped (item / biology_drop / terrain); terrain uses 'terrain' for its tile tables and the one 物品名 table still yields a weapons entry."

requirements-completed: [QUAL-05]

coverage:
  - id: D1
    description: "Three authoritative Feishu design documents committed as well-formed XML evidence snapshots exposing block IDs and checkbox done state"
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "[xml] load of evidence/{biology,item,terrain}.xml + checkbox counts (22/152/166)"
        status: pass
    human_judgment: false
  - id: D2
    description: "Deterministic parser turns committed XML into the inventory, resolving 贴图/代码 by header name and expanding rowspan/colspan"
    requirement: QUAL-05
    verification:
      - kind: unit
        ref: "scripts/test-parser.ps1#column anchoring, rowspan, status colours, blocker emission"
        status: pass
    human_judgment: false
  - id: D3
    description: "Offline validator gates the inventory (exit 2 no weapons rows, exit 3 malformed entry, exit 4 incomplete audit)"
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "powershell -File scripts/validate-inventory.ps1 -> OK(0): 103 entries (84 weapons)"
        status: pass
    human_judgment: false
  - id: D4
    description: "01-INVENTORY.json is a valid machine source of truth with source + parse_audit blocks and 103 entries"
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "ConvertFrom-Json + per-entry required-field checks + parse_audit completeness gate"
        status: pass
    human_judgment: false
  - id: D5
    description: "01-INVENTORY.md mirrors the JSON row set one-to-one"
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "markdown data rows (103) == JSON entries (103)"
        status: pass
    human_judgment: false

duration: ~21min
completed: 2026-09-12
status: complete
---

# Phase 1 Plan 01: Inventory Pipeline Tracer Summary

**Committed Feishu XML evidence plus a deterministic PowerShell parser/validator that emits a 103-entry JSON inventory (item 89, biology_drop 13, terrain 1) and a matching Markdown matrix.**

## Performance

- **Duration:** ~21 min
- **Started:** 2026-09-12T05:17:45Z
- **Completed:** 2026-09-12T05:39:09Z
- **Tasks:** 3
- **Files modified:** 9 created

## Accomplishments

- Fetched the three authoritative design docs once (read-only, user identity) and committed them as well-formed XML evidence: `evidence/biology.xml` (rev 7333, 22 checkboxes), `evidence/item.xml` (rev 5144, 152 checkboxes), `evidence/terrain.xml` (rev 5281, 166 checkboxes).
- Built `scripts/parse-design-xml.ps1`: header-anchored (never positional), rowspan/colspan-logical-grid aware, emits `01-INVENTORY.json` + `01-INVENTORY.md`, with per-document `parse_audit`.
- Built `scripts/validate-inventory.ps1` (offline gate) and `scripts/test-parser.ps1` (synthetic fixture locking texture/code anchoring, merged cells, and the fill-colour rule).
- Full skeleton inventory: 103 entries across categories armor=19 and weapons.{misc=47,summon=15,melee=10,ranged=7,magic=5}; sources item=89, biology_drop=13, terrain=1. Status split: green=59, yellow=19, unchecked=25.

## Task Commits

Each task was committed atomically:

1. **Task 1: End-to-end evidence snapshot and one-category inventory slice** - `8bfa16ca4` (feat)
2. **Task 2: Expand the parser to every design table and emit the full inventory skeleton** - `cf23a886c` (feat)
3. **Task 3: Add a fixture regression test for the parser's column and status rules** - `4d8570e7f` (test)

**Plan metadata:** (docs: complete plan — committed separately)

## Files Created/Modified

- `evidence/biology.xml`, `evidence/item.xml`, `evidence/terrain.xml` - committed DocxXML snapshots (wrapped in a `<fragment>` root)
- `evidence/sources.json` - token/revision_id/file/fetched_at sidecar for the JSON `source` block
- `scripts/parse-design-xml.ps1` - deterministic header-anchored parser → JSON + Markdown
- `scripts/validate-inventory.ps1` - offline gate (exit 0/2/3/4)
- `scripts/test-parser.ps1` - synthetic-fixture regression test
- `01-INVENTORY.json` - machine source of truth (103 entries)
- `01-INVENTORY.md` - human matrix mirror (Weapons, Armor sections)

## Decisions Made

- **Fragment wrapper for evidence:** raw `--detail full` content is a multi-root fragment, which cannot load with `[xml]`. Evidence is stored as `<fragment mode="full">…</fragment>`, and the parser descends through the wrapper (and tolerates a bare fragment).
- **Status from checkboxes only:** `artwork_complete`/`code_complete` come from the `贴图`/`代码` checkbox `done` attributes; `status` is green(both)/yellow(one)/unchecked(none). Fill colours are recorded as `feishu.row_color` but never feed status.
- **Name-header scope:** only tables with an item/armour/drop name header emit entries; the remaining tables are counted in `parse_audit` but skipped as non-item design rows.
- **ASCII-only scripts:** see Deviations.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] PowerShell 5.1 mis-decoded UTF-8 BOM-less scripts**
- **Found during:** Task 1
- **Issue:** Windows PowerShell 5.1 reads a BOM-less `.ps1` using the system ANSI code page; literal Chinese in the script was turned into mojibake and corrupted parsing (e.g. the texture regex silently matched the wrong column, producing empty texture IDs). The repo forbids UTF-8 BOMs.
- **Fix:** Made all three scripts 100% ASCII; Chinese header tokens are written as .NET regex `\uXXXX` escapes (and the test decodes `\uXXXX` to build its fixture). Added `ConvertTo-Json | -replace CRLF` normalization to LF.
- **Files modified:** `scripts/parse-design-xml.ps1`, `scripts/validate-inventory.ps1`, `scripts/test-parser.ps1`
- **Verification:** parser resolves texture/code checkbox IDs; `test-parser.ps1` passes; BOM check passes.
- **Committed in:** `8bfa16ca4` (parser/validator), `4d8570e7f` (test)

**2. [Rule 3 - Blocking] Evidence snapshots were not well-formed XML**
- **Found during:** Task 1
- **Issue:** `lark-cli docs +fetch --detail full` returns a multi-root fragment; the plan's acceptance requires the files to load with `[xml]`.
- **Fix:** Wrapped each snapshot in a single `<fragment mode="full">` root and taught the parser to descend through it.
- **Files modified:** `evidence/*.xml`, `scripts/parse-design-xml.ps1`
- **Verification:** `[xml]` load succeeds and each file exposes its checkbox IDs (22/152/166).
- **Committed in:** `8bfa16ca4`

**3. [Rule 2 - Missing Critical] Added `evidence/sources.json` metadata sidecar**
- **Found during:** Task 1
- **Issue:** The required JSON `source` block needs `revision_id`/`fetched_at`, captured during the one-time fetch; without a committed sidecar those values would be lost or require a forbidden re-fetch.
- **Fix:** Capture token/revision_id/file/fetched_at per document into `evidence/sources.json`; the parser reads it into `source`.
- **Files modified:** `evidence/sources.json`, `scripts/parse-design-xml.ps1`
- **Verification:** `source` carries non-empty `file`, `token`, `revision_id` for all three docs.
- **Committed in:** `8bfa16ca4`

**4. [Rule 2 - Missing Critical] Validator exemption for unfilled checkbox cells**
- **Found during:** Task 2
- **Issue:** The plan's exit-3 rule requires a non-empty `feishu.texture_checkbox_id` on every entry, but real design tables (`竹林供台`, `旧址废墟`, `桃林酒罐`, one `水下迷宫` row) have resolvable `贴图`/`代码` columns whose cells were never filled in — making the strict rule unsatisfiable and the plan self-contradictory.
- **Fix:** Exit 3 only when an entry has neither a texture checkbox id nor any recorded blocker. Such rows are kept (not dropped) with `artwork_complete=false`, `code_complete=false`, `status=unchecked`, and a blocker.
- **Files modified:** `scripts/validate-inventory.ps1`
- **Verification:** `validate-inventory.ps1` exits 0; 15 real unfilled-cell rows retained with blockers.
- **Committed in:** `cf23a886c`

**5. [Rule 3 - Blocking] No-op parser change staged as generic-by-default**
- **Found during:** Task 2
- **Issue:** The plan staged Task 1 as a weapons-only parser and Task 2 as its generalisation. The parser was authored generic in Task 1 with a `-CategoryFilter` switch, so Task 2 needed only the validator extension plus a full default run.
- **Fix:** Task 2 regenerated the full inventory and extended the validator; the parser required no code change (it already iterated every table, header-anchored).
- **Files modified:** `01-INVENTORY.json`, `01-INVENTORY.md`, `scripts/validate-inventory.ps1`
- **Verification:** 103 entries with complete `parse_audit`; `validate-inventory.ps1` exits 0.
- **Committed in:** `cf23a886c`

---

**Total deviations:** 5 auto-fixed (3 blocking, 2 missing-critical)
**Impact on plan:** All fixes were required for the pipeline to run correctly or for the plan's own acceptance criteria to be satisfiable. No scope creep beyond the phase's inventory-record objective.

## Issues Encountered

- `lark-cli auth status` does not accept `--as user` (the plan's precondition wording); `lark-cli auth status` confirmed the user identity as `needs_refresh`, which auto-refreshed on the first user API call. No human action was required.
- Windows console mojibake during debugging was cosmetic; all files were verified byte-level (no BOM, LF, parseable JSON/XML).

## Known Stubs

The following fields are intentionally populated as skeleton defaults by this plan and completed by later plans (they do not block this plan's goal — a structural inventory skeleton):

- `internal_name`, `repo_asset`, `dependencies`, `tranche`, `advances`, `deferred`/`deferred_reason` on every entry — reconciliation in plan 02 and tranche plans 03/04.
- `localization.{en_us,zh_hans}` default `false` for every entry — plan 05 resolves localization parity.

## User Setup Required

None - `lark-cli` 1.0.95 was already installed with an authenticated user identity; no new packages or secrets were added.

## Next Phase Readiness

- Plan 02 can consume `01-INVENTORY.json` (schema_version 1) and reuse `scripts/validate-inventory.ps1` unchanged.
- `feishu.block_id`s (table, row-name, texture/code checkbox) are captured for Phase 8 status writes.
- No Feishu writes were made; no credentials, tokens, or placeholder art were committed.

---
*Phase: 01-item-inventory-completed-art-items*
*Completed: 2026-09-12*

## Self-Check: PASSED

- All 9 created artifacts exist (3 evidence XML, sources.json, 3 scripts, JSON, MD).
- Task commits `8bfa16ca4`, `cf23a886c`, `4d8570e7f` exist in git history.
- `validate-inventory.ps1` exits 0 (103 entries); `test-parser.ps1` exits 0.
