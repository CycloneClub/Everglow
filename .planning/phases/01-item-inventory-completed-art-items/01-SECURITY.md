---
phase: "01"
slug: "item-inventory-completed-art-items"
status: verified
# threats_open = count of OPEN threats at or above workflow.security_block_on severity (the blocking gate)
threats_open: 0
asvs_level: 1
created: "2026-09-12"
---

# Phase 01 — Security

> Per-phase security contract: threat register, accepted risks, and audit trail.

---

## Trust Boundaries

| Boundary | Description | Data Crossing |
|----------|-------------|---------------|
| Feishu doc → lark-cli | External private document content crosses into the repo as XML; editor-influenceable | Design text, block IDs, checkbox state |
| lark-cli → developer machine | OAuth user token and auth cache live outside the repo | OAuth token / auth cache (must not enter repo) |
| evidence XML → parser | Untrusted external XML parsed into structured data | DocxXML (data-only) |
| Inventory JSON → downstream phases | Authoritative machine source of truth for plans 03–05 and Phase 2+ | Internal names, tranche, blockers, labels |
| Fetched design rows → C# values | Design text transcribed into code; must remain data | Numeric/name/prose fields |
| Item classes → build/package pipeline | Changed classes compiled and packaged into `Everglow.tmod` | Compiled assembly |
| Inventory routing → Phase 2 worklist | `phase`/`deferred_reason` decide later-phase scope | Routing metadata |
| Plan SUMMARies → inventory ledger | Deviation records consolidated into the machine inventory | Deviation ledger |

---

## Threat Register

| Threat ID | Category | Component | Severity | Disposition | Mitigation | Status |
|-----------|----------|-----------|----------|-------------|------------|--------|
| T-01-01 | Tampering/Elevation | Fetched Feishu XML consumed by parser/agent | high | mitigate | Offline deterministic table reader (cells + checkbox attrs only); content is data, never executed (verifier truth 18) | closed |
| T-01-02 | Information Disclosure | lark-cli token / auth cache | high | mitigate | Only DocxXML fetched; `evidence/sources.json` holds non-secret `token`/`revision_id`/`fetched_at`; no OAuth token committed | closed |
| T-01-03 | Denial of Service | Oversized/malformed XML parse | medium | mitigate | Parser reads a committed local snapshot, not a live stream; `test-parser.ps1` fixture bounds behaviour | closed |
| T-01-04 | Repudiation/Tampering | Silent mutation of authoritative design source | high | mitigate | Fetch-only (`docs +fetch`); no `docs +update` issued in the phase | closed |
| T-01-05 | Tampering | Unsafe generated filenames/paths from document text | low | accept | Fixed literal filenames (`biology.xml`, `item.xml`, `terrain.xml`); no path derived from document text | closed |
| T-01-SC | Tampering | Package/CLI supply chain | low | accept | No packages installed; lark-cli pinned (RESEARCH §Environment Availability) | closed |
| T-02-01 | Tampering/Elevation | Instruction-like design-row text | high | mitigate | Reconciliation reads cells/attributes only; fetched content never executed or followed | closed |
| T-02-02 | Repudiation | Honesty of green/yellow/unchecked status | high | mitigate | `check-inventory-reconciliation.ps1` enforces yellow⇒blocker and green⇒both-complete; post-phase correction `795192c31` actuated green=50/yellow=8/unchecked=45 | closed |
| T-02-03 | Information Disclosure | Token leakage into committed evidence/JSON | high | mitigate | JSON stores only non-secret `token`/`revision_id`/`file` metadata; no OAuth token read or written | closed |
| T-02-04 | Tampering | Silent scope drop (unresolved label/row) | medium | mitigate | Deferred entries require a reason; unresolved labels stay blocked (Green Tundra `resolved:false`); assumptions non-empty (7) | closed |
| T-02-SC | Tampering | Package/CLI supply chain | low | accept | No installs in this phase | closed |
| T-03-01 | Tampering | Placeholder art introduction | high | mitigate | No-placeholder prohibition + `check-tranche-A.ps1` `.png` git guard; phase diff vs base `5c025ff7` has 0 binary files | closed |
| T-03-02 | Tampering/Elevation | Instruction-like design-row text driving code/paths | high | mitigate | Only numeric/name/prose fields transcribed; paths derive from repo conventions | closed |
| T-03-03 | Tampering | Silent omission of a completed-art entry | medium | mitigate | Coverage gate requires class-or-blocker per Phase 1 tranche-A entry; gate 38/38 | closed |
| T-03-04 | Tampering | Class-less entries force-implemented despite no repo target / false design-code checkbox | high | mitigate | Routing sets `phase=2` + `deferred_reason`; all 43 class-less entries routed, no implementation authored | closed |
| T-03-05 | Tampering | Plan-01 skeleton restored over the plan-02 reconciliation | medium | mitigate | Plan forbids re-running `parse-design-xml.ps1`; both tranche gates required green. Residual technical risk (CR-01: parser output defaults to the live inventory with no `-Force`/merge guard) documented as RISK-01 below — medium severity, below the `high` block threshold | closed |
| T-03-SC | Tampering | Package supply chain | low | accept | No installs in this phase | closed |
| T-04-01 | Tampering | Placeholder art introduction | high | mitigate | No-placeholder prohibition + `check-tranche-B.ps1` `.png` git guard; 0 binary in phase diff | closed |
| T-04-02 | Tampering/Elevation | Instruction-like design-row text driving code/paths | high | mitigate | Only design fields transcribed; paths follow repo conventions | closed |
| T-04-03 | Tampering | Silent omission of a completed-art entry | medium | mitigate | Coverage gate requires class-or-blocker per tranche-B entry; gate 20/20 | closed |
| T-04-04 | Tampering | Placeable key rename breaking localization | medium | mitigate | Plan forbids key renames; only `LocalizationCategory` added; key resolution additive in plan 05 | closed |
| T-04-05 | Tampering | New content added for an out-of-scope entry | medium | mitigate | Tasks restricted to `phase == 1` completed-art tranche-B predicate; no class-less entries | closed |
| T-04-SC | Tampering | Package supply chain | low | accept | No installs in this phase | closed |
| T-05-01 | Tampering | Hand-created/misclassified localization keys | high | mitigate | Only the in-game exporter creates keys; phase diff shows 0 Localization HJSON changes | closed |
| T-05-02 | Information Disclosure | Credentials committed during fetch/consolidation | high | mitigate | Only DocxXML and non-secret metadata exist in the phase directory; no token written | closed |
| T-05-03 | Tampering | Placeholder art hiding missing approved artwork | high | mitigate | No-placeholder audit over item + localization trees; missing art/keys remain blocked | closed |
| T-05-04 | Repudiation | Deviation ledger diverging from the inventory | medium | mitigate | Reconciliation + consistency checks re-run after consolidation | closed |
| T-05-05 | Tampering | Instruction-like design text reaching localization values | medium | mitigate | Keys/values come from class metadata; document text never an exporter input | closed |
| T-05-06 | Repudiation | Phase 2-routed entries silently counted as Phase 1 misses | medium | mitigate | Coverage gate filters `phase == 1`; final task confirms the 43 entries remain `phase=2` | closed |
| T-05-SC | Tampering | Package supply chain | low | accept | No installs in this phase | closed |

*Status: open · closed · open — below high threshold (non-blocking)*
*Severity: critical > high > medium > low — only open threats at or above workflow.security_block_on count toward threats_open*
*Disposition: mitigate (implementation required) · accept (documented risk) · transfer (third-party)*

---

## Accepted Risks Log

| Risk ID | Threat Ref | Rationale | Accepted By | Date |
|---------|------------|-----------|-------------|------|
| AR-01 | T-01-05 | Evidence filenames are fixed literals; no path derived from document text, so residual risk is accepted | user (phase directive) | 2026-09-12 |
| AR-02 | T-01-SC, T-02-SC, T-03-SC, T-04-SC, T-05-SC | No packages installed in Phase 1; lark-cli 1.0.95 already present and versioned | user (phase directive) | 2026-09-12 |
| RISK-01 | T-03-05 residual (CR-01) | `parse-design-xml.ps1` defaults `-OutFile` to the live `01-INVENTORY.json`; re-running it without care overwrites the plan-02–05 reconciliation (60 `internal_name` mappings, tranche values, labels, deviations). Medium severity. Mitigated by process control (the plan forbids re-running the parser) and tracked for a technical guard via `/gsd-code-review 1 --fix` (add `-Force`/merge guard) | user (phase directive) | 2026-09-12 |

*Accepted risks do not resurface in future audit runs.*

---

## Security Audit Trail

| Audit Date | Threats Total | Closed | Open | Run By |
|------------|---------------|--------|------|--------|
| 2026-09-12 | 30 | 30 | 0 | gsd-manager (secure-phase, ASVS L1) |

---

## Sign-Off

- [x] All threats have a disposition (mitigate / accept / transfer)
- [x] Accepted risks documented in Accepted Risks Log
- [x] `threats_open: 0` confirmed
- [x] `status: verified` set in frontmatter

**Approval:** verified 2026-09-12
