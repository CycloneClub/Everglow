---
phase: 03-completed-art-ordinary-monsters
plan: 01
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, vanillla-ai, subworld, yggdrasil, kelp-curtain, loot-table, gate-script, biology-matrix]

# Dependency graph
requires:
  - phase: 01-item-inventory-completed-art-items
    provides: committed evidence/biology.xml snapshot (the only creature data source, D-25) and the ThornTurtleShell item type
  - phase: 02-remaining-items-unfinished-art-materials
    provides: the check-*.ps1 gate conventions (ASCII-only, [IO.File]::ReadAllText, Get-ChildItem class resolution) and D-20/D-21/D-22/D-23
provides:
  - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json (31-row Phase 3-4 machine matrix, D-24)
  - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md (generated human mirror)
  - .planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1 (100% ASCII phase gate, 13 invariants)
  - .planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md (eight-section deviation/blocker ledger)
  - Everglow.Yggdrasil.KelpCurtain.NPCs.MossyThornTurtle (荆棘苔龟, full implementation)
  - Everglow.Yggdrasil.KelpCurtain.KelpCurtainBiome.IsKelpCurtainLayer(Player) (server-safe layer predicate)
affects: [03-02, 03-03, 03-04, phase-4-remaining-ordinary-monsters, phase-7-bosses, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (64000 tokens, 2 tasks) to calibrate future estimates.
# chars/4 over the files actually changed, never a harness token count.
actuals:
  tokens: 26746
  tasks: 2
  commits: 3   # measured: git rev-list --count a16ded945..HEAD (2 task commits + 1 plan-metadata commit)
plan_head_before: a16ded945adb7ebbd40192d886036784f59ef1d8

tech-stack:
  added: []
  patterns:
    - "Repository-art tranche rule (D-41/D-42) recorded in a machine matrix instead of a design-source checkbox"
    - "ASCII PowerShell 5.1 phase gate with a class-resolution biconditional (code_complete <=> on-disk class)"
    - "Server-safe subworld layer predicate beside the camera-driven biome predicate"

key-files:
  created:
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md
    - .planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md
    - .planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs
  modified:
    - Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs

key-decisions:
  - "The Phase 3/4 tranche is repository art (D-41/D-42), superseding 03-RESEARCH Finding 1 / Open Question 1 (inline design <img>)"
  - "design_art is derived mechanically from the nine enumerated inline <img> sections and is independent of texture_complete"
  - "internal_name is populated only once its class file exists on disk, so the gate's class-resolution biconditional holds at every wave boundary"
  - "Spawn gating uses the new server-safe KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player); IsBiomeActive is left byte-for-byte unchanged"
  - "The melee reflect is applied once on the client doing the damage (tML never runs OnHitByItem on the server) under a player.whoAmI != Main.myPlayer guard"
  - "BIO-01/BIO-02/BIO-03/BIO-06 are advanced by this plan but completed by none of them; REQUIREMENTS.md is not touched here"

patterns-established:
  - "A phase gate that resolves classes from the working tree (Get-ChildItem) so a class created in the same task is visible before its commit"
  - "A phase-scoped byte-level UTF-8 BOM guard over the phase's own git status change set, in addition to the later AGENTS.md origin/master run"
  - "Conservative defaults recorded in a ledger section instead of being silently chosen"

requirements-completed: []
requirements-advanced: [BIO-01, BIO-02, BIO-03, BIO-06]

coverage:
  - id: D1
    description: "31-row Phase 3-4 biology matrix (03-BIOLOGY.json) plus its generated mirror, with the repository-art tranche, frozen counts, the six-id texture_complete set and the nine-id design_art set"
    requirement: BIO-01
    verification:
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1 (invariants 2-6, 11)"
        status: pass
    human_judgment: true
    rationale: "The gate proves internal consistency, but the asset-to-creature mappings and the tranche membership are assumptions recorded for designer confirmation (03-DEVIATIONS.md sections 1-3)."
  - id: D2
    description: "MossyThornTurtle end-to-end: subworld-isolated spawn, vanilla tortoise AI with the design spin-state defence/damage switch, melee reflect and the 5% ThornTurtleShell drop"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exits 0, packages Everglow.tmod)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariants 5/7/8 (class resolution, SpawnChance tokens, Main.dedServ, item-type resolution)"
        status: pass
    human_judgment: true
    rationale: "Spawn isolation, spin-state feel, the reflect and the drop are runtime properties; the D-21 client bundle in 03-UAT.md records the run (03-DEVIATIONS.md section 8)."
  - id: D3
    description: "KelpCurtainBiome.IsKelpCurtainLayer(Player), the server-safe layer predicate shared by plans 03-02 and 03-03, with IsBiomeActive unchanged"
    requirement: BIO-06
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exits 0)"
        status: pass
    human_judgment: true
    rationale: "Its dedicated-server equivalence to IsBiomeActive is a runtime property; a dedicated-server launch is recorded as outstanding in 03-DEVIATIONS.md section 6."
  - id: D4
    description: "100% ASCII PowerShell 5.1 phase gate scripts/check-biology.ps1 with 13 invariants, red before and green after the class existed"
    verification:
      - kind: other
        ref: "check-biology.ps1 run before the class existed (exit 1, missing MossyThornTurtle.cs) and after (exit 0, 'OK(0): phase3 tranche = 5 / 5 (rows=31)')"
        status: pass
      - kind: other
        ref: "byte check of the script (0 bytes > 0x7F) and the phase-scoped UTF-8 BOM invariant 13"
        status: pass
    human_judgment: false
  - id: D5
    description: "03-DEVIATIONS.md: the eight-section ledger recording the tranche rule, the repository-art/design-art discrepancy, the mapping assumptions, the conservative defaults, the drop-availability correction, the spawn-region gap, the D-20 deferral and the D-21 runtime list"
    verification: []
    human_judgment: true
    rationale: "It records design-facing assumptions and blockers that only a designer can confirm; no offline check can validate the intent."

duration: 184min
completed: 2026-09-15
status: complete
---

# Phase 3 Plan 01: Biology Matrix Tracer Summary

**Repository-art tranche frozen in a 31-row gateable matrix, with 荆棘苔龟 (`MossyThornTurtle`) implemented end-to-end inside Yggdrasil only**

## Performance

- **Duration:** ~184 min (wall clock, including the snapshot-derivation analysis)
- **Started:** 2026-09-15T02:12:23Z
- **Completed:** 2026-09-15T05:16:43Z
- **Tasks:** 2
- **Files modified:** 6 (4 planning artifacts, 1 new class, 1 modified biome class)

## Accomplishments

- `03-BIOLOGY.json` freezes the Phase 3–4 creature matrix: **31 rows** derived only from the committed `evidence/biology.xml` (the Feishu document was neither re-fetched nor mutated), with the repository-art tranche rule (D-41/D-42) replacing the superseded inline-`<img>` proxy from `03-RESEARCH.md` Finding 1 / Open Question 1. Frozen counts: rows 31, phase3 5, phase4 23, phase7 3, deferred 2, `texture_complete_true` 6, `design_art_true` 9.
- `scripts/check-biology.ps1` implements the whole phase gate on its full contract: 13 invariants covering coverage/counts, the tranche and design-art sets, per-row fields, the class-resolution biconditional, deferred rules, the Phase 3 structural guards, item-type resolution, the no-placeholder-art guard, `-RequireAll`, Markdown parity and a phase-scoped byte-level UTF-8 BOM guard. It is 100% ASCII (0 bytes above 0x7F) and ran **red before** the class existed and **green after**.
- `MossyThornTurtle.cs` implements 荆棘苔龟 end-to-end beside its tracked `MossyThornTurtle.png` (so tML's default texture resolution finds the approved art): subworld-only spawn, vanilla `AI_039_Tortoise` AI with the design's spin-state switch (防御 10→20, 伤害 50→75), the design's 1–10 melee reflect, and the 5% 荆棘龟壳 drop from an already-implemented Phase 1 item.
- `KelpCurtainBiome.IsKelpCurtainLayer(Player)` was added as the shared, server-safe layer predicate (`IsBiomeActive` left byte-for-byte unchanged), because `ModNPC.SpawnChance` runs on the server where the client camera is zero — the camera-driven predicate would have blocked every tranche spawn there, and the same helper is what plans 03-02/03-03 will reuse.
- `03-DEVIATIONS.md` opened with its eight sections so nothing is silently absorbed: the tranche rule with D-41/D-42 verbatim, the repository-art vs design-art discrepancy (six assets, six design-art-only creatures), the mapping assumptions, the conservative defaults, the drop-availability correction to Finding 2, the spawn-region gap, the D-20 localization deferral and the D-21 runtime list.

## Task Commits

Each task was committed atomically:

1. **Task 1: End-to-end "repository-art creature spawns isolated, fights and drops" — 荆棘苔龟 only, plus the matrix and the gate on their full contract** — `421f2a09a` (feat)
2. **Task 2: Human-readable mirror, phase deviation ledger and the tranche-rule record** — `f7dfa1ac0` (docs)

**Plan metadata:** one further commit (`docs(03-01): complete biology matrix tracer plan`) carrying this SUMMARY, `STATE.md`, `ROADMAP.md`, `state.json` and `WINDOWS.md`; it is the third commit in the `plan_head_before..HEAD` range and its hash is recorded in the completion report.

## Files Created/Modified

- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — the 31-row machine matrix (D-24); machine source of truth for Phases 3–4
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — generated 31-row human mirror with matching ids, statuses and blocker cells
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — the eight-section phase deviation/blocker ledger
- `.planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1` — the 100% ASCII phase gate
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs` — `Everglow.Yggdrasil.KelpCurtain.NPCs.MossyThornTurtle`
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs` — added the `IsKelpCurtainLayer(Player)` static predicate only

## Decisions Made

- **The tranche is repository art.** D-41/D-42 define "texture-complete" as the repository already containing the creature's approved art, not a design-row checkbox or inline design `<img>`; the five tranche creatures are 荆棘苔龟, 格普螺, 叶飞棍, 巨树人 and the already-implemented 水蛞蝓.
- **`design_art` is separate from `texture_complete`.** It is derived mechanically from the ten enumerated `<img>` elements in nine design sections, so 水蛞蝓's easily-missed 36x27 image is included and 吸血魔毯 (a Phase 7 row with no inline image) is not.
- **`internal_name` is an honest field.** It is populated only once its class file exists, so the three tranche rows still `code_complete: false` carry an empty `internal_name` and plans 03-02/03-03 fill each in the same task that creates its class — which is what keeps the gate's class-resolution biconditional jointly satisfiable at every wave boundary.
- **Spawn gating is server-safe.** `SpawnChance` requires `SubworldSystem.IsActive<YggdrasilWorld>()` **and** `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`; `EditSpawnPool` returns early outside the subworld, so this per-creature gate is the real BIO-06 isolation.
- **The reflect locality is the narrowest correct choice.** tML never invokes `OnHitByItem` on the server, so the design's 1–10 reflect is applied once on the client doing the damage under a `player.whoAmI != Main.myPlayer` guard and `Player.Hurt` performs the hurt sync; the not-taken `ModIns.PacketResolver` alternative is recorded.
- **Requirements are advanced, not completed.** This plan advances BIO-01, BIO-02, BIO-03 and BIO-06, but only one of the phase's five tranche creatures exists; `REQUIREMENTS.md` is therefore left untouched and the traceability rows stay Pending until plan 03-04 closes the phase.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] `spawnInfo.player` does not exist — corrected to `spawnInfo.Player`**
- **Found during:** Task 1 (Step D, authoring `MossyThornTurtle.cs`)
- **Issue:** The plan (and its acceptance criterion) wrote `IsKelpCurtainLayer(spawnInfo.player)`. `NPCSpawnInfo` exposes `Player` as a public **field**; there is no lowercase `player` member, so the literal plan token would not compile.
- **Fix:** Used `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` — the form the plan itself permitted as "the property form" and the form the in-repo precedent `RiverSlug.cs` uses.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs`
- **Verification:** `Terraria.ModLoader.NPCSpawnInfo` member list from the tModLoader XML documentation contains only `F:Terraria.ModLoader.NPCSpawnInfo.Player`; the Release build compiles.
- **Committed in:** `421f2a09a` (Task 1 commit)

**2. [Rule 1 - Bug] `NPC.rare` does not exist — corrected to `NPC.rarity`**
- **Found during:** Task 1 (first `dotnet build` of the Release configuration)
- **Issue:** The plan wrote `NPC.rare = ItemRarityID.White;`, and the build failed with `error CS1061: "NPC" does not contain a definition for "rare"`. `Terraria.NPC` has no `rare` member; the engine's only NPC rarity field is `rarity` (the Lifeform Analyzer rarity, documented as defaulting to 0).
- **Fix:** `NPC.rarity = ItemRarityID.White;` — a no-op that records the design's empty 稀有度 cell, which is exactly the plan's stated intent for a conservative default.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 and packages `Everglow.tmod`.
- **Committed in:** `421f2a09a` (Task 1 commit)

Both corrections are recorded in `03-DEVIATIONS.md` §4 under "Plan API-name corrections (Rule 1 — non-existent members)".

---

**Total deviations:** 2 auto-fixed (2 Rule 1 bugs, both non-existent API members named by the plan)
**Impact on plan:** Both were required for the build to pass; neither changes the plan's intent, scope or behaviour. No scope creep.

## Issues Encountered

- The first Release build failed on the `NPC.rare` member (deviation 2). After the fix the build exited 0, produced `Everglow.tmod`, and the gate stayed green.
- `gsd-tools state advance-plan` acted on the **stale** STATE.md position (`current_phase: 02`, `current_plan: 5`) and rewrote the frontmatter, `completed_phases` and `status` for Phase 2. That write was reverted (`git checkout -- .planning/STATE.md .planning/state.json`) and the Phase 3 plan-1 updates were applied directly instead. The orchestrator should know the tool's positional verbs currently resolve against the stale phase pointer.
- By design, `behavior_block_id`/`drop_block_id` are derived with the plan's literal prefix rule ("first paragraph after the stats table" / "first paragraph starting with the drop or death prefix"), so a few Phase 4 rows whose drop sentence is phrased differently (装甲虾, 爆弹水母, 枯木活化士兵, 大型荆棘苔龟) intentionally carry an empty `drop_block_id` rather than a guessed id. Recorded in the JSON `assumptions[]`.

## User Setup Required

None - no external service configuration required.

## Known Stubs

None. The only deliberate omissions are design-sourced: `ModifyIncomingHit` carries no `FinalDamage` scaling because the design's 减伤 cell is empty (commented with the cell's stats-table block id), and no `SpecificDebuffImmunity` is declared because the design's 免疫 cell is empty. Both are recorded in the matrix row and `03-DEVIATIONS.md`.

## Threat Flags

None. No new network endpoint, auth path, file-access pattern or trust-boundary schema was introduced. `03-BIOLOGY.json` contains only non-sensitive creature metadata and block ids.

## Verification

- `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 (0 errors) and packages `Everglow.tmod`.
- `scripts/check-biology.ps1` exits 0 and prints `OK(0): phase3 tranche = 5 / 5 (rows=31)` followed by `OK: implemented classes = 2 / 5`; the Markdown parity invariant is green and the BOM invariant reports 4 phase files.
- The same gate exited **1** before the class existed, failing with `bio-valley-of-lush-and-moist-mossy-thorn-turtle : internal_name '...MossyThornTurtle' does not resolve to 'MossyThornTurtle.cs' on disk` and `code_complete true requires 'MossyThornTurtle.cs' on disk` — the red/green transition the plan asked for.
- No `.png` or other binary asset was added, moved, renamed or modified (`git status` under the NPCs tree shows only `.cs`); no HJSON was edited and the Feishu document was not touched.
- The D-21 client checks (spawn isolation, spin-state feel, reflect, drop) are recorded as **not yet executed** in `03-DEVIATIONS.md` §8 and are referred forward to plan 03-04's UAT bundle.

## Next Phase Readiness

- Wave 2 (03-02) precondition is satisfied: the Wave 1 gate is green and `MossyThornTurtle.cs` provides the class shape to mirror.
- Plans 03-02 and 03-03 must reuse `KelpCurtainBiome.IsKelpCurtainLayer`, not `IsBiomeActive`, and must keep the class-name-equals-asset-basename rule so the gate's class-resolution invariant stays green.
- The matrix's `assumptions[]`/`deviations[]` arrays carry one entry per `03-DEVIATIONS.md` item 1–6 with the frozen counts, tranche ids and `texture_complete` set ready for the later waves to extend.

## Self-Check: PASSED

- `03-BIOLOGY.json` exists and parses with 31 rows.
- `03-BIOLOGY.md` exists with 31 table rows.
- `03-DEVIATIONS.md` exists with its eight sections.
- `scripts/check-biology.ps1` exists, is 100% ASCII, and exits 0.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs` exists.
- `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs` contains `IsKelpCurtainLayer`.
- Commits `421f2a09a` and `f7dfa1ac0` exist on `Yggdrasil/newContent0-ai`.

---
*Phase: 03-completed-art-ordinary-monsters*
*Completed: 2026-09-15*
