---
phase: 01-item-inventory-completed-art-items
plan: 03
subsystem: content-items
tags: [feishu, inventory, kelp-curtain, items, design-parity, localization, d-10, d-12, phase-split, blocked]

# Dependency graph
requires:
  - plan: 01-02
    provides: "Reconciled 01-INVENTORY.json (internal_name, repo_asset, localization, tranche, advances, blockers), the five-label taxonomy, and check-inventory-reconciliation.ps1"
provides:
  - "LocalizationCategory overrides on GreenSungloStaff and VineRepairWand (RESEARCH Pitfall 5 gap closed for the tranche-A folders)"
  - "Completed-art tranche-A numeric design parity for 18 item classes: damage, knockback, use time, mana, value, rarity corrected to the Feishu item/terrain design rows (D-12)"
  - "Data-driven PHASE SPLIT RECOMMENDED with a per-category breakdown of the 18 class-less completed-art tranche-A entries (Task 2 work bound)"
affects: [01-04, 01-05, 02]

# Actuals (#2632) — pairs with the plan's estimate (tokens 55000).
actuals:
  tokens: 3584        # chars/4 over the realized code diff (14,334 chars) — plan halted before the bulk tranche work
  tasks: 1            # Task 1 complete; Task 2 halted by its >8 guard; Task 3 not executed
  commits: 1          # MEASURED: git rev-list --count ${plan_head_before}..HEAD
  plan_head_before: 35ccd096c97a2350b7baf0afa69d730ef3244c15

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Header-anchored design-row transcription: the DevilHeart entries live in an armor table (护具/数值/价格/稀有度) while the rest use the weapons table, so cell indices must be resolved per <thead> header, not assumed"
    - "Design value convention: the design 价格 cell maps to Item.value; buyPrice(...)/raw Item.value match it, while sellPrice(...) calls that yield 5x are deviations"

key-files:
  created: []
  modified:
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/GreenSungloStaff.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/Special/VineRepairWand.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/MossySpell.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/GreenVineWhip.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Ammos/HuskburstBullet.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/LegumeGyroscope.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/DevilHeart/DevilHeartGyroscope.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/DevilHeart/DevilHeartBayonet.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/DevilHeartIronBar_Item.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/MeatLantern.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArcI.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ActivatedDogStaff.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/AntiCorrosiveSole.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BladeOfGreenMoss.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/CorrodedPearl.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMinionGyroscope.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicSpellBook.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicWhip.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/ForestMino.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RazorbeakBow.cs

key-decisions:
  - "Task 1 deviation reconciliation expanded from the plan's declared two files to 20 files: an audit of all 38 class-bearing completed-art tranche-A entries found numeric deviations on 18 of them, and D-12 requires completed-art parity. Recorded as a Rule 2/3 scope expansion."
  - "Effects, set bonuses, and recipes were NOT exhaustively re-verified: the design encodes them as prose, so only the numeric/name columns (the threat model's sanctioned transcription surface) were reconciled. Flagged for the verifier."
  - "Task 2 halted by its own guard: 18 class-less completed-art tranche-A entries exceeds the >8 bound, so no classes were implemented and scripts/check-tranche-A.ps1 (Task 3) was not authored. A phase split is recommended instead."

requirements-completed: []

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "GreenSungloStaff and VineRepairWand declare a non-default LocalizationCategory; no ModItem in the tranche-A folders still defaults to Items (LichenTentacle inherits SummonWeapons from the WhipItem template)."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 errors; grep for LocalizationCategory across Items/{Weapons,Ammos,Materials,Accessories,Misc,BossSummon}"
        status: pass
    human_judgment: false
  - id: D2
    description: "18 completed-art tranche-A classes were brought to numeric design parity (damage, knockback, use time, mana, value, rarity); see the deviation ledger."
    requirement: QUAL-05
    verification:
      - kind: automated
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 -> 0 errors; AGENTS.md UTF-8 BOM check passed (1270 files)"
        status: pass
    human_judgment: false
  - id: D3
    description: "Class-less completed-art coverage is surfaced data-driven: 18 entries (all also Feishu code-incomplete) exceed the >8 bound and are returned as PHASE SPLIT RECOMMENDED rather than silently implemented or dropped."
    requirement: QUAL-05
    verification: []
    human_judgment: true
    rationale: "Choosing how to split the tranche is a scoping decision for the user/orchestrator, not an automated check."

# Metrics
duration: ~1h (incl. XML audit; commit-to-commit interval much shorter)
completed: 2026-09-12
status: blocked
---

# Phase 1 Plan 03: Completed-Art Item Tranche A (Weapons, Ammos, Materials, Accessories, Misc, Boss Summon) Summary

**Task 1 complete: added the two missing `LocalizationCategory` overrides and reconciled 18 completed-art tranche-A classes to their Feishu design values; Task 2 halted — 18 class-less completed-art entries exceed the >8 bound, so PHASE SPLIT RECOMMENDED.**

## Performance

- **Duration:** ~1h of analysis + one code commit (the plan halted before the bulk tranche work)
- **Tasks:** 1 of 3 complete (Task 1 committed; Task 2 halted by its guard; Task 3 not executed)
- **Files modified:** 20 (all under `Sources/Modules/Yggdrasil/KelpCurtain/Items/`)
- **Commits:** 1

## Accomplishments

- **Localization (RESEARCH Pitfall 5):** added `LocalizationCategory => LocalizationUtils.Categories.MagicWeapons` to `GreenSungloStaff` and `VineRepairWand`. A recursive scan of the six tranche-A folders confirms no other `ModItem` lacks a category (`LichenTentacle` is a `WhipItem`, which already supplies `SummonWeapons`).
- **Design parity (D-12):** audited all 38 class-bearing completed-art tranche-A entries against their committed Feishu rows. Design cells were resolved per-table from each row's `<thead>` header — important because the six `DevilHeart*` rows live in an armor table (`护具/贴图/代码/数值/价格/稀有度/效果/套装效果`), not the weapons table. 18 classes had numeric deviations; all were corrected (see ledger). Remaining 20 entries already matched (e.g. `EvilHalbertBarnacle`, `EmptyWaterStaff`, `RedAlgaeMagicStaff`, `GreenThornBallLauncher`).
- **Value convention locked:** across the audited set, the design `价格` maps to `Item.value` (`buyPrice(...)`/raw `value`). Oversized `sellPrice(...)` values that yielded 5x the design price were corrected (`GreenSungloStaff`, `MeatLantern`, `AntiCorrosiveSole`, `BladeOfGreenMoss`, `CorrodedPearl`, `ForestMino`, `GreenVineWhip`).
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` → 0 errors; AGENTS.md byte-level UTF-8 BOM check → passed (1270 files).

## Task Commits

1. **Task 1: Fix localization categories and completed-art design deviations (tranche A)** — `261996ecf` (feat)

**Task 2** was not committed: its precondition bound tripped (18 > 8), so it returns the split recommendation below and stops.
**Task 3** (`scripts/check-tranche-A.ps1`) was not executed: the plan's Task 2 instruction is to stop.

## Files Created/Modified

All 20 files are existing `ModItem` classes; no file was created, no binary/art asset was touched, and no localization key was added, renamed, or deleted.

## Deviation Ledger (file, field, before → after)

| File | Field | Before | After (Feishu design) |
|------|-------|--------|------------------------|
| `Weapons/GreenSungloStaff.cs` | `LocalizationCategory` | (default `Items`) | `MagicWeapons` |
| `Weapons/GreenSungloStaff.cs` | `value` | `sellPrice(0,2,0,0)` (10g) | `buyPrice(silver: 80)` (80s) |
| `Weapons/Special/VineRepairWand.cs` | `LocalizationCategory` | (default `Items`) | `MagicWeapons` |
| `Weapons/MossySpell.cs` | `damage` | 13 | 29 |
| `Weapons/MossySpell.cs` | `mana` | 4 | 12 |
| `Weapons/MossySpell.cs` | `useTime`/`useAnimation` | 16 | 50 |
| `Weapons/MossySpell.cs` | `rare` | `Green` | `Orange` |
| `Weapons/GreenVineWhip.cs` | `value` | `sellPrice(0,1,0,0)` (5g) | `10000` |
| `Weapons/GreenVineWhip.cs` | `rare` | `Blue` | `Orange` |
| `Ammos/HuskburstBullet.cs` | `damage` | 10 | 6 |
| `Ammos/HuskburstBullet.cs` | `knockBack` | (unset) | 1.15 |
| `Ammos/HuskburstBullet.cs` | `shootSpeed` | 16 | 18 |
| `Ammos/HuskburstBullet.cs` | `value` | 50 | 20 |
| `Ammos/HuskburstBullet.cs` | `rare` | (unset) | `Blue` |
| `Weapons/LegumeGyroscope.cs` | `knockBack` | 0.2 | 0.8 |
| `Weapons/LegumeGyroscope.cs` | `value` | 11700 | 20000 |
| `Weapons/LegumeGyroscope.cs` | `rare` | `Green` | `Blue` |
| `Weapons/DevilHeart/DevilHeartGyroscope.cs` | `damage` | 17 | 24 |
| `Weapons/DevilHeart/DevilHeartBayonet.cs` | `damage` | 22 | 13 |
| `Materials/DevilHeartIronBar_Item.cs` | `rare` | (unset) | `Green` |
| `Weapons/MeatLantern.cs` | `value` | `sellPrice(gold:1)` (5g) | `buyPrice(gold:1)` |
| `Weapons/UnderwaterTreasury/ArcI.cs` | `damage` | 39 | 21 |
| `Weapons/UnderwaterTreasury/ArcI.cs` | `knockBack` | 6 | 1 |
| `Weapons/UnderwaterTreasury/ArcI.cs` | `crit` | 8 | 14 |
| `Weapons/ActivatedDogStaff.cs` | `damage` | 8 | 16 |
| `Weapons/ActivatedDogStaff.cs` | `knockBack` | 1.1 | 4 |
| `Weapons/ActivatedDogStaff.cs` | `useTime`/`useAnimation` | 20 | 29 |
| `Weapons/ActivatedDogStaff.cs` | `value` | `buyPrice(gold:1)` | `buyPrice(silver:80)` |
| `Weapons/ActivatedDogStaff.cs` | `rare` | `Green` | `Orange` |
| `Accessories/AntiCorrosiveSole.cs` | `value` | `sellPrice(gold:1)` (5g) | `buyPrice(gold:1)` |
| `Weapons/BladeOfGreenMoss.cs` | `value` | `sellPrice(gold:1)` (5g) | `buyPrice(silver:80)` |
| `Accessories/CorrodedPearl.cs` | `value` | `sellPrice(gold:1)` (5g) | `buyPrice(gold:1)` |
| `Weapons/RedAlgaeMinionGyroscope.cs` | `knockBack` | 0.2 | 1.5 |
| `Weapons/RedAlgaeMinionGyroscope.cs` | `useTime`/`useAnimation` | 16 | 42 |
| `Weapons/RedAlgaeMagicSpellBook.cs` | `useTime`/`useAnimation` | 4 | 6 |
| `Weapons/RedAlgaeMagicWhip.cs` | `knockBack` | (base 2) | 1.5 |
| `Weapons/RedAlgaeMagicWhip.cs` | `useTime`/`useAnimation` | 30 | 22 |
| `Accessories/ForestMino.cs` | `value` | `sellPrice(gold:2)` (10g) | `buyPrice(gold:2)` |
| `Weapons/RazorbeakBow.cs` | `useTime` | 12 | 11 |

**Not reconciled (out of the numeric transcription surface):** item `效果` (prose effects), armor `套装效果` (set bonuses), and full recipes. These were not exhaustively compared because the design encodes them as free prose; they remain part of the D-12 obligation for the (re)scoped tranche. Where a recipe was spot-checked, it already matched (e.g. `DevilHeartIronBar_Item` = ore×3 + bone×1 at a Furnace).

## PHASE SPLIT RECOMMENDED

Task 2's bound tripped: **18** entries have `tranche == "A"` and `artwork_complete == true` with an empty `internal_name` and the `no repo implementation found` blocker — well above the bound of 8. Per the plan, none were implemented in this context.

Because the plan's Task 2 is meant to be split data-driven once the count is known, here is the breakdown of those 18.

**By `category`:**

| category | count |
|----------|-------|
| weapons.melee | 7 |
| weapons.misc | 7 |
| weapons.ranged | 2 |
| weapons.magic | 1 |
| weapons.summon | 1 |
| **total** | **18** |

**By source / intent:**

| grouping | count | entries |
|----------|-------|---------|
| biology drops → melee weapons | 5 | 龙骸巨块大剑, 狂战士角盔, 狂战士板甲, 狂战士胫甲, 骇翼链剑 |
| biology drops → misc items | 3 | 血肉聚合物, 巨翼龙圣物-大师, 巨翼龙纪念章 |
| biology drops → magic weapons | 1 | 崩解阈限 |
| item rows → misc | 4 | arm-of-giant-tree, 森林之息, 厄佛提根的净化粉末, 枯萎面具 |
| item rows → ranged | 2 | 魁札尔的愿望, 龙骨猎枪 |
| item rows → melee | 2 | 碧绿玉髓扇, 碧玉弯刀 |
| item rows → summon | 1 | 魂蛇手杖 |

All 18 additionally carry the `design code not complete (Feishu code checkbox false)` blocker, so the Task 3 coverage gate would already treat them as blocked; implementing them is nonetheless what Task 2 asks for, which is why the split (not silent omission) is recommended.

**Suggested split axes for the replan:**
1. **Biology drops (9)** — they are biology-design drops and may belong with the creature phases (3–4) per the roadmap's drop rule; confirm before implementing.
2. **Item rows (9)** — split by type (misc/ranged/melee/summon) into bounded implementation plans, each with its own `check-tranche-A-<slice>.ps1`-style gate.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2/3 - Missing Critical / Scope] Task 1 deviation reconciliation expanded from 2 declared files to 20**
- **Found during:** Task 1
- **Issue:** The plan's `files_modified` named only `GreenSungloStaff.cs` and `VineRepairWand.cs`, but Task 1's action requires correcting deviations on *every* completed-art tranche-A entry (D-12). A full audit found 18 of the 38 class-bearing entries deviating on damage/knockback/use time/mana/value/rarity.
- **Fix:** Corrected all 18 classes to the design values, resolved per-table headers. Recorded the expanded surface here and in the ledger.
- **Files modified:** the 20 listed above.
- **Verification:** Release build 0 errors; BOM check passed.
- **Committed in:** `261996ecf`

**2. [Rule 3 - Blocking] Header mapping differs for the DevilHeart rows**
- **Found during:** Task 1
- **Issue:** A naive cell index mis-mapped the six `DevilHeart*` rows (armor table header `护具/贴图/代码/数值/价格/稀有度/效果/套装效果`) as if they were weapon rows.
- **Fix:** Resolved column names from each row's `<thead>` before comparing, then applied only the verified fields (`damage`, `rare`).
- **Files modified:** `DevilHeartGyroscope.cs`, `DevilHeartBayonet.cs`, `DevilHeartIronBar_Item.cs`.
- **Verification:** Build 0 errors.
- **Committed in:** `261996ecf`

**3. [Rule 3 - Blocking] Task 2 precondition bound exceeded → plan halted**
- **Found during:** Task 2 (precondition)
- **Issue:** 18 class-less completed-art tranche-A entries > 8.
- **Fix:** Followed the plan's instruction verbatim: returned `PHASE SPLIT RECOMMENDED` with the breakdown and stopped; Task 3 was not authored.
- **Files modified:** none.
- **Committed in:** n/a (documented here)

## Issues Encountered

- The plan's `files_modified` and Task 1 action disagreed on scope (2 files vs "every completed-art entry"); resolved in favour of the action + D-12 and documented as deviation 1.
- Effects / set bonuses / recipes could not be mechanically compared (prose cells) and are carried forward.

## User Setup Required

None — no external service configuration, no packages or secrets; no Feishu write was made.

## Next Phase Readiness

- Plan 01-04 (tranche B) has the identical Task 2 bound; it should be split under the same data-driven approach (its count is independent).
- Task 3's `scripts/check-tranche-A.ps1` still needs to be authored by whichever plan owns the split tranche, since it was not created here.
- Blockers carried forward: the 18 class-less completed-art tranche-A entries, the un-reconciled prose effect/set-bonus/recipe fields, the 43 total class-less entries from plan 02, and the unresolved Green Tundra label.

---

*Phase: 01-item-inventory-completed-art-items*
*Status: blocked (PHASE SPLIT RECOMMENDED at Task 2)*

## Self-Check: PASSED

- All 20 declared modified files exist.
- Task commit `261996ecf` exists in git history.
- `01-03-SUMMARY.md` written; Release build 0 errors; AGENTS.md BOM check passed.
