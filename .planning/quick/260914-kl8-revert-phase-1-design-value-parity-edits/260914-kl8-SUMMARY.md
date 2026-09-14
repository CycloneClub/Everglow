---
phase: 260914-kl8
plan: 01
subsystem: Yggdrasil / Kelp Curtain items; Phase 1 design-value parity
tags: [revert, design-parity, item-values, kelp-curtain, follow-the-code]
requires:
  - "5c025ff7e (pre-Phase-1 code baseline)"
  - "Phase 1 numeric parity edits staged on disk"
provides:
  - "Working tree and committed baseline for the 26 full-revert item classes matching 5c025ff7e"
  - "Recorded follow-the-code conflict rule in Phase 1 records, PROJECT.md, and Phase 2 D-23"
affects:
  - "Phase 2 planning (02-CONTEXT.md D-23)"
  - "Phase 8 design-status synchronization scope"
tech-stack:
  added: []
  patterns:
    - "Follow-the-code conflict rule: when implemented code conflicts with the design, follow the code unless the Feishu row is marked yellow with a corresponding explanation"
key-files:
  created:
    - .planning/quick/260914-kl8-revert-phase-1-design-value-parity-edits/260914-kl8-SUMMARY.md
  modified:
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/AntiCorrosiveSole.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/CorrodedPearl.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/ForestMino.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/ThornTurtleShell.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Ammos/HuskburstBullet.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinMask.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinMagicRobe.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Ruin/RuinLeggings.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/DevilHeartIronBar_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/PermanentBoosters/JadeFruit.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/AlgaeExtractor_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/DevilHeartIronOre_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/JadeizedBone_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ActivatedDogStaff.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BladeOfGreenMoss.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/GreenSungloStaff.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/GreenVineWhip.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/LegumeGyroscope.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/MeatLantern.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/MossySpell.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RazorbeakBow.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicSpellBook.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicWhip.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMinionGyroscope.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArcI.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/DevilHeart/DevilHeartBayonet.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/DevilHeart/DevilHeartGyroscope.cs
    - .planning/phases/01-item-inventory-completed-art-items/01-CHANGE-RECORD.md
    - .planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md
    - .planning/PROJECT.md
    - .planning/phases/02-remaining-items-unfinished-art-materials/02-CONTEXT.md
decisions:
  - "Follow-the-code: when implemented code conflicts with the design, follow the code unless the Feishu row is marked yellow with a corresponding explanation"
  - "Revert the Phase 1 numeric parity edits to 5c025ff7e; keep the non-numeric Phase 1 additions (recipes, LocalizationCategory, comment fix)"
  - "GreenSungloStaff: revert value only; retain the MagicWeapons LocalizationCategory override"
metrics:
  duration: ~20min
  completed: 2026-09-14
  tasks: 2
  commits: 1
status: complete
---

# Phase 260914-kl8 Plan 01: Revert Phase 1 design-value parity edits Summary

## One-liner

Reverted Phase 1's numeric design-parity edits across 27 Kelp Curtain item classes to the pre-Phase-1 code baseline `5c025ff7e` (keeping recipes, localization categories, and the comment fix), and recorded the follow-the-code conflict rule as D-23 in the Phase 1 records, `PROJECT.md`, and the Phase 2 context.

## Objective

During live testing the Feishu design numbers were found unreasonable, so the original code values were correct. This quick task undid Phase 1's numeric parity edits while keeping its non-numeric additions, and documented the governing rule: when implemented code conflicts with the design, follow the code unless the Feishu row is marked yellow with a corresponding explanation.

## What Was Built

### Task 1 — Complete and verify the source revert (27 item files)

- Re-ran the idempotent `git checkout 5c025ff7e -- <file>` for the 26 full-revert files; all are now byte-identical to the baseline commit.
- `GreenSungloStaff.cs` is the special case: `Item.value` reverted to `Item.sellPrice(0, 2, 0, 0);` while `public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;` is retained.
- Kept non-numeric Phase 1 changes verified still different from the base: the four DevilHeart armor recipes, the added `LocalizationCategory` overrides, and the `ShellMolluscsBreastPlate` comment correction.
- Committed the 27 source files as `e8103ff9051c4003c1a1c12523905167ec5c84ae` (27 files changed, 41 insertions, 48 deletions).

### Task 2 — Document the reversion and the rule

- `01-CHANGE-RECORD.md`: appended `## Reversion (2026-09-14)` naming the 27 reverted files (12 stat-parity / 16 value-rarity, `LegumeGyroscope` overlapping), the `5c025ff7e` baseline, the supersession of sections 1–2, and the `GreenSungloStaff` special case; rule sentence included verbatim.
- `01-DEVIATIONS.md`: appended `## Reversion Ledger (2026-09-14)` with the `File | Pre-revert (Phase 1) | Reverted to (5c025ff7e)` table for all 27 files, entry type `reversion`, and the note that `01-INVENTORY.json` `deviations[]`/`phase1_actions[]` are unchanged; rule sentence included verbatim.
- `PROJECT.md`: added item 6 to "Design Status Synchronization", a "Conflict resolution" bullet under "Constraints", a Key Decisions row with outcome `Reverted Phase 1 numeric edits (2026-09-14)`, and updated the trailing `*Last updated:*` line; rule sentence included verbatim in both policy locations.
- `02-CONTEXT.md`: added **D-23** carrying the rule forward into Phase 2; rule sentence included verbatim.

## Verification

| Check | Result |
| --- | --- |
| Parity gate: 26 full-revert files match `5c025ff7e` | PASS |
| `GreenSungloStaff` value reverted + `LocalizationCategory` retained | PASS |
| Kept changes (4 recipes, category overrides, comment fix) still differ from base | PASS |
| `dotnet build /p:Configuration=Release /p:WarningLevel=0` | PASS — exit code **0**, `0 个错误` (4 pre-existing `.xnb` shader warnings, unrelated) |
| Rule sentence verbatim in all four docs | PASS (`DOCS OK`) |
| `5c025ff7e` present in `01-CHANGE-RECORD.md`; `D-23` present in `02-CONTEXT.md` | PASS |
| Byte-level UTF-8 BOM check (repo-wide, `origin/master` base) | PASS — 1296 files, no BOM |
| LF-only line endings on the four edited docs | PASS — CR=0 |
| No `.png`/binary/art asset, no `Localization/**`, no Feishu source touched | PASS |

## Files Changed

**Source (Task 1, committed in `e8103ff90`):** 27 Kelp Curtain item classes under `Sources/Modules/Yggdrasil/KelpCurtain/Items/` — 12 stat-parity + 16 value-rarity (one overlapping) and the `GreenSungloStaff` special case.

**Docs (Task 2, left uncommitted on disk for the orchestrator's docs commit):** `.planning/phases/01-item-inventory-completed-art-items/01-CHANGE-RECORD.md`, `.planning/phases/01-item-inventory-completed-art-items/01-DEVIATIONS.md`, `.planning/PROJECT.md`, `.planning/phases/02-remaining-items-unfinished-art-materials/02-CONTEXT.md`.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 — Bug] Plan's parity gate listed `Placeables/AlgaeExtractor_Item.cs` in both the full-revert and kept sets**
- **Found during:** Task 1 verification
- **Issue:** The `<verify>` `$f` (26 full-revert) array and the `$kept` array both contained `Placeables/AlgaeExtractor_Item.cs`, so the gate threw `kept change was reverted`. The file has no Phase-1 `LocalizationCategory` addition (its `LocalizationCategory` line predates `5c025ff7e`; the only Phase-1 change was a removed `Item.value = 20000;`), so it belongs to the full-revert set. The `$kept` entry is a plan copy-paste error.
- **Fix:** Ran the parity gate with `AlgaeExtractor_Item.cs` removed from the `$kept` list only; it remains correctly full-reverted. No source change was needed; the file is at `5c025ff7e` as the Task 1 `<files>` list intends.
- **Files modified:** none (verification-only correction)
- **Commit:** n/a (documented in this SUMMARY)

## Commits

| Hash | Message |
| --- | --- |
| `e8103ff90` | revert(260914-kl8): restore pre-Phase-1 code values for design-parity edits |

## Notes

- Per the orchestrator constraint, only source changes were committed; the four planning-document edits from Task 2 are on disk and are intentionally left for the orchestrator's docs commit.
- `git status` after the source commit showed only the four doc files modified and the untracked `.planning/quick/` directory; no untracked generated artifacts.

## Self-Check: PASSED
