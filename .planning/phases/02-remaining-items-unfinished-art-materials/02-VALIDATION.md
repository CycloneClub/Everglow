---
phase: "2"
slug: "remaining-items-unfinished-art-materials"
# status lifecycle: draft (seeded by plan-phase) → validated (set by validate-phase §6)
# audit-milestone §5.5 distinguishes NOT-VALIDATED (draft) from PARTIAL (validated + nyquist_compliant: false) (#2117)
status: draft
nyquist_compliant: false
wave_0_complete: true
created: "2026-09-14"
---

# Phase 2 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

**Promotion note:** `status` stays `draft` and `nyquist_compliant` stays `false` here; promoting them is `/gsd-validate-phase`'s step. This file records the actual verification state, not a pre-claim of validation.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | none — offline PowerShell `check-*.ps1` gates + `dotnet build` (repo has no unit-test path for `ModItem` content; MSTest cannot construct `Main` or load mod content) |
| **Config file** | none |
| **Quick run command** | `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| **Full suite command** | Phase 2 gates (`check-phase2.ps1 -RequireAll`) + every Phase 1 regression gate + `dotnet build /p:Configuration=Release /p:WarningLevel=0` |
| **Estimated runtime** | ~60 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet build /p:Configuration=Release /p:WarningLevel=0` (or the structural gate where the task changes only planning artifacts)
- **After every plan wave:** Run `check-phase2.ps1` + `dotnet build`
- **Before `/gsd-verify-work`:** Full suite must be green
- **Max feedback latency:** 120 seconds

---

## Per-Task Verification Map

Populated by plan 02-05 Task 2 from the plans' own `<automated>` blocks. Each `Automated Command` cell is transcribed verbatim from the owning plan.

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|------------|-----------------|-----------|-------------------|-------------|--------|
| 02-01-T1 | 02-01 | 1 | ITEM-02 | T-02-01, T-02-02, T-02-04 | Art-missing armor `Load()` returns on `Main.dedServ` and registers the equip slot against the shared fallback, so a missing `_Head` asset cannot abort mod loading; no `.png` is created. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-01-T2 | 02-01 | 1 | ITEM-02 | T-02-01, T-02-02 | All four armor classes register explicitly (no autoload-equip), set `code_complete:true`/`artwork_complete:false`/`status:unchecked`, and create no art. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-01-T3 | 02-01 | 1 | ITEM-02 | T-02-03, T-02-07 | The four rows keep the D-11/D-22 marking and the JSON/Markdown mirror stays reconciled. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1" } }` | ✅ | ✅ green |
| 02-02-T1 | 02-02 | 2 | ITEM-01, ITEM-03 | T-02-01, T-02-03, T-02-04 | The catapult projectile names no absent type, sets no `PreDraw`/`projFrames`, and guards every client-only dust/sound call. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-02-T2 | 02-02 | 2 | ITEM-01, ITEM-03 | T-02-01, T-02-05, T-02-08 | The greatbow/restriction device/bait write no recipe naming an absent Phase 7 type, spawn no encounter, and follow the committed evidence values. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-02-T3 | 02-02 | 2 | ITEM-01, ITEM-03 | T-02-07 | The four weapon rows carry precise blockers and the JSON/Markdown mirror reconciles at 103 rows. | structural gate | `powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1" } }` | ✅ | ✅ green |
| 02-03-T1 | 02-03 | 3 | ITEM-03, ITEM-04 | T-02-01, T-02-04 | `JadeSnakeEgg` consumes without spawning and references no NPC type; the talisman is an identity-only accessory shell. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-03-T2 | 02-03 | 3 | ITEM-03, ITEM-04 | T-02-01, T-02-03 | The four shells declare no recipe, projectile, `UpdateAccessory` or equip registration; the vanity declares no equip slot. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-03-T3 | 02-03 | 3 | ITEM-03, ITEM-04 | T-02-04, T-02-05, T-02-07 | The seven rows carry precise blockers, no NPC spawn is introduced, and the mirror reconciles. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1" } } }` | ✅ | ✅ green |
| 02-04-T1 | 02-04 | 4 | ITEM-02, ITEM-03 | T-02-01 | The three shells implement no part of the disciple system and reserve their names only. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-04-T2 | 02-04 | 4 | ITEM-02, ITEM-03 | T-02-01, T-02-03, T-02-04 | The vanity declares no equip slot; the crafting station is a plain `ModItem` with no `ModTile`/`createTile`/`DefaultToPlaceableTile`; no system is built. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" }` | ✅ | ✅ green |
| 02-04-T3 | 02-04 | 4 | ITEM-02, ITEM-03 | T-02-05, T-02-07 | The final six rows carry precise blockers; all 21 manifest rows are `implemented:true`; the mirror reconciles. | build + structural gate | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" -RequireAll; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1" } } }` | ✅ | ✅ green |
| 02-05-T1 | 02-05 | 5 | ITEM-01, ITEM-02, ITEM-03, ITEM-04 | T-02-01, T-02-02 | The consolidated ledger, the inventory `deviations[]` and the 21 rows' blockers agree, with every row still `artwork_complete:false`/`status:unchecked`. | structural gate | `powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" -RequireAll; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/validate-inventory.ps1"; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/01-item-inventory-completed-art-items/scripts/check-inventory-reconciliation.ps1" } }` | ✅ | ✅ green |
| 02-05-T2 | 02-05 | 5 | ITEM-01, ITEM-02, ITEM-03, ITEM-04 | T-02-07 | The validation map names every Phase 2 task and the roadmap close-out record is present without disturbing another phase entry. | structural gate | `powershell -NoProfile -Command "$v = [IO.File]::ReadAllText('.planning/phases/02-remaining-items-unfinished-art-materials/02-VALIDATION.md'); $rows = ([regex]::Matches($v, '02-0[1-5]')).Count; $r = [IO.File]::ReadAllText('.planning/ROADMAP.md'); $hasClose = $r -match 'Phase 2 close-out'; $plans = $r -match '02-05-PLAN\.md'; if ($rows -ge 20 -and $hasClose -and $plans) { Write-Output \"OK(0): validation rows=$rows, roadmap close-out present\" } else { Write-Output \"FAIL(3): validation rows=$rows, close-out=$hasClose, plan-list=$plans\"; exit 1 }"` | ✅ | ✅ green |
| 02-05-T3 | 02-05 | 5 | ITEM-01, ITEM-02, ITEM-03, ITEM-04 | T-02-03, T-02-04, T-02-05, T-02-06 | The full gate chain, the baseline-anchored byte-level BOM check, the no-art/no-binary guard and the advisory localization gate all pass; no failing gate is reported as passing. | structural gate chain | `dotnet build /p:Configuration=Release /p:WarningLevel=0; if ($?) { powershell -NoProfile -ExecutionPolicy Bypass -File ".planning/phases/02-remaining-items-unfinished-art-materials/scripts/check-phase2.ps1" -RequireAll; if ($?) { $d = '.planning/phases/01-item-inventory-completed-art-items/scripts'; foreach ($g in @('validate-inventory.ps1','check-inventory-reconciliation.ps1','check-carryover.ps1','check-tranche-A.ps1','check-tranche-B.ps1')) { powershell -NoProfile -ExecutionPolicy Bypass -File "$d/$g"; if (-not $?) { throw "gate failed: $g" } }; powershell -NoProfile -ExecutionPolicy Bypass -File "$d/check-localization-coverage.ps1" -AllowMissing; if ($?) { $baseline = git rev-parse '926543d99'; if (-not $?) { throw 'git rev-parse failed' }; $base = git merge-base HEAD $baseline; if (-not $?) { throw 'git merge-base failed' }; if ($base -ne $baseline) { throw "phase baseline $baseline is not an ancestor of HEAD ($base); re-anchor to the pre-Phase-2 boundary commit" }; $diffNames = git -c core.quotepath=false diff --name-only --diff-filter=ACMRTUXB $base --; if (-not $?) { throw 'git diff failed' }; $otherNames = git -c core.quotepath=false ls-files --others --exclude-standard; if (-not $?) { throw 'git ls-files failed' }; $files = @($diffNames) + @($otherNames); $bom = New-Object System.Collections.Generic.List[string]; $bin = New-Object System.Collections.Generic.List[string]; $exts = @('.png','.obj','.ttf','.atlas','.xnb','.ogg','.mp3','.bmp','.mapio','.fx'); foreach ($f in $files) { if (-not (Test-Path -LiteralPath $f -PathType Leaf)) { continue }; $b = [IO.File]::ReadAllBytes($f); if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) { $bom.Add($f) }; foreach ($e in $exts) { if ($f.EndsWith($e)) { $bin.Add($f) } } }; if ($bom.Count -gt 0) { throw "UTF-8 BOM: $($bom -join ', ')" }; if ($bin.Count -gt 0) { throw "art/binary changed: $($bin -join ', ')" }; Write-Output 'verify chain OK(0)' } } }` | ✅ | ✅ green |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

Existing infrastructure covers all phase requirements — the Phase 1 gate convention (ASCII PowerShell `check-*.ps1` reading `01-INVENTORY.json`) and the repository build are reused. `scripts/check-phase2.ps1` and `02-CLASSIFICATION.json` were created by plan 02-01 Wave 1 (Wave 0 item) and are green.

---

## Manual-Only Verifications

One row per `<human-check>` block collected from plans 02-01 to 02-04, plus the consolidated plan 02-05 bundle. All are pending a live tModLoader client session (D-21) and are the end-of-phase UAT batch.

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| 红月水藻头饰 loads and equips into the head slot (02-01 T1) | ITEM-02 | Whether an equip slot resolves and the mod loads is a runtime loader property; no offline script observes it. | In a tModLoader client with the mod enabled, launch a world, open the inventory and equip 红月水藻头饰 into the head slot. Expected: the mod loads with no missing-resource/disabled-mod entry, the item shows a white-box icon and picks up defense 8, and the head slot accepts it. |
| 红月水藻 set crafts, equips and applies its effects (02-01 T2) | ITEM-02 | Set detection, the hurt hook and the wet-speed multiplier are runtime gameplay properties. | Craft all four pieces at a Work Bench, equip head + body + legs, and step into water. Expected: both head pieces are accepted; the full set swims/runs faster than the legs piece alone; a single hit of >= 10 damage restores roughly 15% of that hit. |
| 巨石弹射装置 fires and bursts into shards (02-02 T1) | ITEM-01 | Projectile arcs, burst counts and ammo behaviour are runtime gameplay properties. | Obtain `BoulderCatapult` and fire it at a flat wall from a moderate distance. Expected: the boulder arcs under gravity, bursts on the wall, 3-6 shards fly outward, no ammunition is consumed, tooltip damage reads 44. |
| TendonGreatbow boss bonus and the blocked summon effects (02-02 T2) | ITEM-01 | The +10% boss-damage clause and the intentional absence of the blocked effects are runtime gameplay properties. | Equip `TendonGreatbow` with any arrow and fire at a non-boss enemy then a boss; then use 限制机 and 腥臭的诱饵. Expected: arrows are consumed and the mod arrow fires; boss damage reads ~10% higher than an equivalent non-boss target; 限制机 consumes 15 mana but produces no drone; 腥臭的诱饵 swings, consumes one, and summons nothing. |
| 灵蛇玉卵 consumes without summoning; 竹节步符 equips (02-03 T1) | ITEM-04 | Consumable behaviour and accessory-slot acceptance are runtime properties. | Spawn `灵蛇玉卵` and `竹节步符` and use/equip them. Expected: `灵蛇玉卵` is consumable with a 10-gold value and Blue rarity and consumes one without summoning anything; `竹节步符` equips into an accessory slot and grants no stats. |
| The six shells plus 灵蛇玉卵 load with no crash (02-03 T3) | ITEM-03 | Whether every art-missing class actually loads and resolves is a runtime loader property. | Spawn all six shells plus `灵蛇玉卵` and confirm they appear in the inventory with a white-box icon and no crash. Expected: each item is placeable, equips or uses per its declaration, and the tML log shows no missing-resource error. |
| The six 02-04 shells load cleanly and grant nothing (02-04 T2) | ITEM-02, ITEM-03 | Whether every art-missing class loads is a runtime loader property. | Spawn all six shells of this plan and view them in the inventory; also check the tML log for load errors. Expected: each shell appears with a white-box icon, the mod loads with no missing-resource error, and none of the six grants a stat or effect. |
| Consolidated runtime-verification bundle (02-05 T3, end-of-phase UAT) | ITEM-01..ITEM-04 | These are runtime gameplay and loader properties that no offline script observes (D-21). | In one client session, work the whole bundle above: load the mod, confirm no missing-resource error in the tML log, craft and equip the 红月水藻 set, fire 巨石弹射装置 at a wall, compare `TendonGreatbow` damage against a boss and a non-boss, then spawn the remaining shells and confirm each loads and grants nothing. Expected: the mod loads cleanly; the armor equips and its effects apply; the boulder bursts into 3-6 shards; the greatbow's boss damage is about 10% higher; every shell appears with a white-box icon and no effect; no `.png` was ever required. |

---

## Validation Sign-Off

- [x] All tasks have `<automated>` verify or Wave 0 dependencies
- [x] Sampling continuity: no 3 consecutive tasks without automated verify
- [x] Wave 0 covers all MISSING references
- [x] No watch-mode flags
- [x] Feedback latency < 120s
- [ ] `nyquist_compliant: true` set in frontmatter — promotion belongs to `/gsd-validate-phase`, which is not run by this phase; the front matter intentionally stays `draft` / `false`.

**Approval:** pending — Phase 2 code-complete/art-incomplete; the automated gate chain is green and the runtime bundle is collected for the human UAT batch.
