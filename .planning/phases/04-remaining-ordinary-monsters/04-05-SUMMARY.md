---
phase: 04-remaining-ordinary-monsters
plan: 05
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, identity-shell, d-45, missing-art, subworld, yggdrasil, kelp-curtain, spawn-predicate, death-jade-lake]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    plan: 01
    provides: the green 13-invariant Phase 4 gate (`scripts/check-biology.ps1` with its `OK: guarded classes =` progress counter), `KelpCurtainSpawnConditions` (`WaterWeight` and the server-safe predicates), the `NPCs/DeathJadeLake/` region folder that makes the D-49 art migration mechanical, and the `JadeSpiritAnglerfish` art-missing class shape this plan mirrors
  - phase: 02-remaining-items-unfinished-art-materials
    provides: the D-18 identity-shell convention (conservative documented defaults, an explicit blocker, no invented behaviour)
provides:
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs (荧光水螅 D-45 identity shell)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GiantTigerShrimp.cs (巨型虎虾 D-45 identity shell)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/CannonBarnacle.cs (炮弹藤壶 D-45 identity shell, explicitly no cannon behaviour)
  - the exact behaviour-blocker text the three rows need at close-out (`behavior undefined in the design row (D-45 shell)`), recorded here because this plan's prohibitions forbid editing the matrix or `04-DEVIATIONS.md`
affects: [04-09, phase-8-source-acceptance, phase-5-6-region-terrain]

# Actuals (#2632) - pairs with the plan's estimate (30000 tokens, 3 tasks) to calibrate future estimates.
# chars/4 over the realized diff (13982 chars of added source), never a harness token count.
actuals:
  tokens: 3496
  tasks: 3
  commits: 4   # measured: git rev-list --count 63f1c0f07..HEAD after the plan-metadata commit (3 task commits + 1 plan-metadata commit)
plan_head_before: 63f1c0f077a002bed382ede308b8ca7394c73f6f

tech-stack:
  added: []
  patterns:
    - "D-45 identity shell: a loadable ModNPC whose obligations stop at loading, registering, layer-gating, documenting and being blocked — no AI() body, no loot override, no item token"
    - "The vanilla aiStyle 0 ('No AI' — the NPC does not move, it only faces the player) as the documented engine-owned behaviour for a shell, so no AI() body is needed at all"
    - "A shell's blocker text lives in the shared matrix (written by plan 04-09), not in the class: the class comment points at 03-BIOLOGY.json instead of duplicating a mutable string"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GiantTigerShrimp.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/CannonBarnacle.cs
  modified: []

key-decisions:
  - "The three heading-only rows become loadable D-45 identity shells instead of being skipped: the row's code obligation is fulfilled (a class exists and loads) while its behaviour obligation stays explicitly unfinished and visible in the matrix (D-45/D-53)"
  - "No shell declares an AI() body: they sit on the vanilla aiStyle 0, which the Official Terraria Wiki defines as 'No AI - does not move' (it only faces the player), so a hydra/sessile creature needs no custom code and the D-49 migration stays an art drop plus two deletions"
  - "The shells deliberately do NOT override ModifyNPCLoot, OnHitPlayer, ModifyIncomingHit or any attack hook — this plan's acceptance criteria grep for the absence of those tokens, so the empty drop table is the engine's own (a ModNPC with no loot override drops nothing), not an empty override body"
  - "SpawnChance uses only the two server-safe tokens plus KelpCurtainSpawnConditions.WaterWeight: the design says nothing about land or water for these three rows, so no IsWaterSurface / IsWaterBottom / IsDryLand condition is invented (D-52/D-55)"
  - "Every shell sets NPC.value = 0 (the design supplies no 钱币 cell), NPC.rarity = ItemRarityID.White (the design supplies no 稀有度 cell) and NPC.catchItem = 0 with no critter flag (none of the three rows claims 可被捕捉, D-58)"
  - "The plan prose's NPC.rare = ItemRarityID.White; was corrected to NPC.rarity for the fourth time in this project (Phase 3, 04-01, 04-02, now): NPC exposes no rare member, so the literal line is a compile break (Rule 1)"
  - "The exact behaviour blocker the three rows must carry at close-out — behavior undefined in the design row (D-45 shell) — is recorded in this SUMMARY rather than written into 03-BIOLOGY.json or 04-DEVIATIONS.md §10, because this plan's prohibitions forbid editing either (§1's label variant is D-29/D-45 shell; plan 04-09 picks one wording for all three)"
  - "FluorescentHydraWoodDust is named in the hydra's class comment as an unrelated wood-tile dust, so the shell is never read as already owning a dust (the plan's explicit assumption A3)"

patterns-established:
  - "A shell class can be proven conformant by token audit + gate + build alone: no AI()/loot/attack override, the White_Mod override, both spawn tokens, LocalizationCategory, NPCSpawnManager.RegisterNPC, value 0 / rarity White / catchItem 0"
  - "A wave-2 plan that may not touch the phase ledger or the shared matrix still leaves a complete audit trail: the class comments carry the D-45/D-49 statements and the SUMMARY carries the exact close-out blocker strings"
  - "The gate's guarded-class counter is the wave's monotone progress signal: 6 -> 7 -> 8 -> 9 across this plan's three tasks (+3 exactly)"

requirements-completed: []
requirements-advanced: [BIO-01]

coverage:
  - id: D1
    description: "The three heading-only Death Jade Lake rows exist as loadable, registered, layer-gated, art-blocked and behaviour-blocked D-45 identity shells with no invented behaviour: FluorescentHydra, GiantTigerShrimp and CannonBarnacle under NPCs/DeathJadeLake/, each with Texture => Commons.ModAsset.White_Mod, LocalizationCategory, NPCSpawnManager.RegisterNPC, the two spawn tokens, NPC.value = 0, NPC.rarity = ItemRarityID.White, NPC.catchItem = 0 and its design row's heading block id"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (3 runs, one per task: 0 warnings, 0 errors, Everglow.tmod packaged)"
        status: pass
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 after each task (exit 0; 'OK: guarded classes =' 6 -> 7 -> 8 -> 9, exactly +3; 'OK(0): phase4 in-scope set = 21 rows (rows=31)'; 'OK: UTF-8 BOM check passed (3 files).')"
        status: pass
      - kind: other
        ref: "per-file token audit: required tokens present (White_Mod override, LocalizationCategory, RegisterNPC, SubworldSystem.IsActive<YggdrasilWorld>, KelpCurtainBiome.IsKelpCurtainLayer, NPC.value = 0;, NPC.rarity = ItemRarityID.White, NPC.catchItem = 0, the row's block id, SpawnModBiomes) and forbidden tokens absent (override void AI(, ModifyNPCLoot, OnHitPlayer, ModifyIncomingHit, ModContent.ItemType<, Main.npcCatchable, NPCID.Sets.CountsAsCritter, Main.LocalPlayer, KelpCurtainBiome.IsBiomeActive, Dust.NewDust, Projectile.NewProjectile)"
        status: pass
      - kind: other
        ref: "AGENTS.md byte-level checks on all three files: 0 UTF-8 BOM, 0 CR bytes (LF), tab indentation, trailing newline; plus the repo-wide AGENTS.md BOM block 'UTF-8 BOM check passed (1406 files).'"
        status: pass
    human_judgment: false
  - id: D2
    description: "The D-48/D-49 migration property: each shell is loadable with no approved art today and becomes an asset drop plus two deletions later (add <Class>.png beside the .cs, delete the Texture override, delete the blocker) with no behaviour rework — and each shell actually spawns inside the Kelp Curtain layer of the Yggdrasil subworld and never in an ordinary world"
    requirement: BIO-01
    verification:
      - kind: other
        ref: "gate invariant 7 over the guarded classes (White_Mod override or a resolving beside-.png; both spawn tokens on any class declaring SpawnChance; no Main.LocalPlayer) and the region-folder decision recorded in 04-DEVIATIONS.md section 3"
        status: pass
    human_judgment: true
    rationale: "Whether the loader resolves the White_Mod fallback on a live client, and whether these three weights actually place the shells in the lake (and never in the main world), are runtime properties: the offline gate cannot observe a load-time texture resolution or a spawn leak. The check is part of the D-21 client bundle that plan 04-09 records in 04-UAT.md."
  - id: D3
    description: "The row-level blocker text each of the three rows needs before it can be marked code_complete at close-out — behavior undefined in the design row (D-45 shell) — recorded with the exact wording because this plan's prohibitions forbid editing 03-BIOLOGY.json and 04-DEVIATIONS.md"
    verification: []
    human_judgment: true
    rationale: "The blocker is a design-facing statement about an unfinished design row, and it enters the matrix through plan 04-09's consolidation, not through this plan. Only the close-out (or the designer) can confirm the final wording is the one the phase records."

duration: 16min
completed: 2026-09-16
status: complete
---

# Phase 4 Plan 05: Death Jade Lake Identity Shells (荧光水螅, 巨型虎虾, 炮弹藤壶) Summary

**Three heading-only design rows given loadable, layer-gated, art-blocked and behaviour-blocked `ModNPC` shells with no invented behaviour — the D-49 migration stays an asset drop plus two deletions**

## Performance

- **Duration:** ~16 min (from the plan read to the plan-metadata commit)
- **Started:** 2026-09-16T07:28:00Z
- **Completed:** 2026-09-16T07:44:00Z
- **Tasks:** 3
- **Files modified:** 3 (3 created, 0 modified)

## Accomplishments

- **`FluorescentHydra` (荧光水螅, `CvAGdz87Son7TQxKZY8c9Dt5nke`)** — a loadable identity shell: `Texture => Commons.ModAsset.White_Mod` (D-48), `LocalizationCategory` (keys generatable later, D-20), `Main.npcFrameCount[NPC.type] = 1` + `NPCSpawnManager.RegisterNPC(Type)`, and a `SpawnChance` that returns `0f` unless `SubworldSystem.IsActive<YggdrasilWorld>()` **and** `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` both hold, then returns `KelpCurtainSpawnConditions.WaterWeight`. No `AI()` body, no projectile, no buff, no loot rule, no item token (D-45/D-58). Its comment names the row, the heading block id, the heading-only snapshot evidence, the D-45 shell status, the D-49 migration and `03-BIOLOGY.json`, and states that the pre-existing `FluorescentHydraWoodDust` is an unrelated wood-tile dust and is **not** this creature's dust.
- **`GiantTigerShrimp` (巨型虎虾, `UJw8dvF1qozy1AxjEBMcne0Ln5f`)** — the same shell, mirrored line for line, with a wide, low conservative box (`56x28`) for a large shrimp-like aquatic creature instead of the hydra's mid-sized polyp box (`32x40`), and the same heading-only/D-45/D-49 statements.
- **`CannonBarnacle` (炮弹藤壶, `K4q2dw6hUoGkRrxIn3OcIuVenEb`)** — the same shell with a small sessile box (`28x24`) and an explicit statement that **no cannon behaviour is created**: no artillery, no projectile and no firing state machine, because the design defines none and the row's name alone is not a specification. Its comment also records the contrast with the `RiverSlug` critter shape (this row has no 可被捕捉 text, so it is not a critter).
- **The no-invention contract is mechanically checkable.** All three files were audited token by token: no `override void AI(`, no `ModifyNPCLoot`, no `OnHitPlayer`, no `ModifyIncomingHit`, no `ModContent.ItemType<`, no `Main.npcCatchable`, no `NPCID.Sets.CountsAsCritter`, no `Main.LocalPlayer`, no `KelpCurtainBiome.IsBiomeActive`, no `Dust.NewDust` and no `Projectile.NewProjectile`; and each carries the `White_Mod` override, both spawn tokens, `NPC.value = 0;`, `NPC.rarity = ItemRarityID.White`, `NPC.catchItem = 0`, `SpawnModBiomes` and its row's heading block id.
- **The phase gate advanced by exactly three.** `scripts/check-biology.ps1` printed `OK: guarded classes = 6` before the plan, then `7`, `8`, `9` after tasks 1, 2 and 3 — the monotone progress counter plan 04-01 established, with `OK(0): phase4 in-scope set = 21 rows (rows=31)` and the phase-scoped BOM invariant green on every run.

## Task Commits

Each task was committed atomically:

1. **Task 1: 荧光水螅 `FluorescentHydra` identity shell** — `3e40c19c9` (feat)
2. **Task 2: 巨型虎虾 `GiantTigerShrimp` identity shell** — `a19c0863b` (feat)
3. **Task 3: 炮弹藤壶 `CannonBarnacle` identity shell** — `e17230dd3` (feat)

**Plan metadata:** this SUMMARY, `STATE.md` and `ROADMAP.md` are carried by the plan-metadata commit that follows it (the fourth commit in the `plan_head_before..HEAD` range).

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs` — 荧光水螅 D-45 identity shell (96 lines)
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GiantTigerShrimp.cs` — 巨型虎虾 D-45 identity shell (97 lines)
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/CannonBarnacle.cs` — 炮弹藤壶 D-45 identity shell, explicitly no cannon behaviour (98 lines)

No other file changed: `git diff --stat 63f1c0f07..HEAD` is exactly these three files (291 insertions), and the plan-range name filter finds **no** `.png` / `.obj` / `.xnb` / `.hjson` / `.tmod`. `03-BIOLOGY.json` and `03-BIOLOGY.md` are byte-identical to their end-of-04-01 state (`git diff --stat 63f1c0f07..HEAD -- …/03-BIOLOGY.json …/03-BIOLOGY.md` is empty), as the plan's verification required.

## Decisions Made

- **The shells satisfy D-45 by loading, registering, being gated, being documented and being blocked — nothing else.** The row's code obligation (a content class exists and loads) is met; its behaviour obligation stays explicitly unfinished and is visible in the matrix, which is what makes the row's eventual `code_complete` marking honest at close-out (D-45/D-53).
- **`NPC.aiStyle = 0` is the shell's engine-owned behaviour.** Verified against the Official Terraria Wiki AI table (`0 | No AI | Does not move`) and the tModLoader `NPC.aiStyle` documentation ("an aiStyle of 0 will face the player automatically"), so a hydra/shrimp/barnacle holds position and faces the player with **no** `AI()` body in the class. This is the plan's "engine style that requires no custom code", and it is the reason the D-49 migration is art-only.
- **No `ModifyNPCLoot` override at all** (not even an empty body). This plan's acceptance criteria forbid the token, so the shells rely on the engine's own behaviour: a `ModNPC` with no loot override drops nothing. The design supplies no drop for these three rows, so the effective table is empty — D-58's intent — without a token the criteria forbid. This is a deliberate contrast with the 04-01/04-02 siblings, which ship explicitly empty, commented bodies for rows whose design *names* an absent drop.
- **`SpawnChance` invents no positional condition.** These three rows say nothing about land or water (unlike 水黾's 水面上 or 碧灵鮟鱇's 水底), so the shells take the neutral `KelpCurtainSpawnConditions.WaterWeight` band and state in the comment that the design supplies no position. The two isolation tokens are mandatory and unchanged (D-52/D-55); the shells never use `IsBiomeActive`.
- **Conservative defaults are recorded in the class comments, not in `04-DEVIATIONS.md` §10.** This plan's prohibitions forbid creating or modifying `04-DEVIATIONS.md` in wave 2 (a wave-2 edit would make the close-out ledger diverge from the finalized `03-BIOLOGY.json`), so each class carries the "every value is this phase's conservative default pending the design (D-45/D-54)" statement and the values are registered below for plan 04-09. The defaults: hydra `32x40` / life 60 / damage 20 / defense 4; shrimp `56x28` / life 90 / damage 25 / defense 6; barnacle `28x24` / life 50 / damage 15 / defense 10; all three `value = 0`, `rarity = ItemRarityID.White`, `catchItem = 0`, engine default `HitSound`/`DeathSound`.
- **The close-out blocker wording is recorded here, not in the matrix.** Plan 04-09 must add `behavior undefined in the design row (D-45 shell)` to each of the three rows' `blockers` arrays alongside the existing D-48 artwork element (`04-DEVIATIONS.md` §1 writes the same label as `D-29/D-45 shell`; one wording should be chosen for all three).
- **REQUIREMENTS.md is not touched.** BIO-01 is advanced by three of the 21 rows and completed by none of this plan, so its traceability row stays Pending until plan 04-09 closes the phase (the Phase 3 / 04-01 precedent).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] `NPC.rare` does not exist — written as `NPC.rarity`**
- **Found during:** Task 1 (the first shell's `SetDefaults`)
- **Issue:** The plan's Task 1/2/3 actions and acceptance criteria repeat `NPC.rare = ItemRarityID.White;` (and the criteria grep for that literal token). `NPC` exposes no `rare` member in this tML build — the engine's only NPC rarity field is `NPC.rarity` — so the literal line is a compile break.
- **Fix:** Wrote `NPC.rarity = ItemRarityID.White;` with a comment naming the empty 稀有度 cell, and carried the same correction into all three shells. The correction is already recorded in `04-DEVIATIONS.md` §10 by plans 04-01/04-02 (fourth occurrence in this project), so no ledger edit was required — and this plan may not edit that file anyway.
- **Files modified:** the three shell classes
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 with 0 warnings and 0 errors after every task.
- **Committed in:** `3e40c19c9` (Task 1), mirrored in `a19c0863b` (Task 2) and `e17230dd3` (Task 3)

### Process notes (documented, no code impact)

**2. The exact blocker strings could not be written where §1 says they live.**
- **Found during:** Task 1 (pre-commit audit)
- **Issue:** `04-DEVIATIONS.md` §1 states the shells "carry a precise `behavior undefined in the design row (D-29/D-45 shell)` blocker", and the plan's `must_haves` say the same with the `(D-45 shell)` label. Blockers live in `03-BIOLOGY.json`, which this plan's prohibitions forbid editing, and `04-DEVIATIONS.md` is explicitly forbidden too (opened by 04-01, consolidated by 04-09).
- **Resolution:** The class comments state the D-45 shell status, the heading-only evidence and the D-49 migration and point at `03-BIOLOGY.json`; the exact close-out blocker text is recorded in the Decisions section above for plan 04-09. No prohibition was relaxed and no artifact was touched.
- **Verification:** `git diff --stat 63f1c0f07..HEAD -- …/03-BIOLOGY.json …/03-BIOLOGY.md` is empty; `04-DEVIATIONS.md` has no entry in this plan's diff.

**3. Tooling note: the plan commit ledger was rewritten from UTF-16LE to ASCII.**
- **Found during:** the SUMMARY measurement
- **Issue:** PowerShell 5.1's `>` redirect wrote `.git/gsd-plan-head-before-04-05` as UTF-16LE with a BOM, so `git rev-list --count $(cat …)..HEAD` returned `0` instead of `3` and would have under-reported the plan's commit count to `/gsd-verify-work`.
- **Resolution:** The ledger (a file inside `.git/`, not a tracked artifact) was rewritten as plain UTF-8 without BOM with the correct base `63f1c0f077a002bed382ede308b8ca7394c73f6f`; the count then read `3` (3 task commits), and this SUMMARY records the measured `commits: 4` including the plan-metadata commit.
- **Files modified:** `.git/gsd-plan-head-before-04-05` only (no repository file).

---

**Total deviations:** 1 auto-fixed (Rule 1, repeated in all three files) + 2 documented process notes with no code impact
**Impact on plan:** No scope creep and no prohibition relaxed. The Rule 1 fix was required for the build to pass; the other two are recorded so a reader sees the reasoning rather than an unexplained divergence.

## Issues Encountered

- **Console encoding noise (unchanged from prior plans).** `dotnet build` and `Get-Content` render CJK as mojibake in the PowerShell 5.1 console. It is a display artefact only: all three files are valid UTF-8 without BOM (0 BOM, 0 CR bytes, tab indentation, trailing newline), which the byte-level checks and the phase gate's BOM invariant confirm.

## Known Stubs

None. No hardcoded empty value, placeholder string or unwired data source was introduced. The three classes use the shared `Commons.ModAsset.White_Mod` fallback by design (D-48/D-49) and are identity shells by design (D-45) — that is the plan's deliberate scope, not a stub — and the behaviour gap is recorded as a blocker for plan 04-09 rather than hidden.

## Defect Ledger

Two entries were appended to `.planning/WINDOWS.md` for cross-phase visibility:

- **`unrun-verify`** — the D-21 client bundle for these three shells is not executed offline: a client must show each shell spawning inside the Kelp Curtain layer within Yggdrasil and never in an ordinary world, and loading cleanly with the `White_Mod` fallback. Plan 04-09 records it in `04-UAT.md`.
- **`deviation`** — the Rule 1 `NPC.rare` → `NPC.rarity` correction in all three shells (the fourth occurrence of the same plan-prose error in this project).

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- **The three heading-only rows are no longer invisible.** Every row of the 21-row Phase 4 scope now has either a full implementation or an explicit, documented shell, so plan 04-09's close-out cannot silently drop 荧光水螅, 巨型虎虾 or 炮弹藤壶.
- **Wave 3 inherits a green gate at 9 guarded classes** (`OK: guarded classes = 9` of the 37 that close the phase) and an unchanged shared seam; the next tasks add classes to an already-green gate, as intended.
- **Outstanding for these three rows, all recorded, none hidden:**
  - plan 04-09 must write `behavior undefined in the design row (D-45 shell)` into each row's `blockers` array and set `code_complete: true` with `internal_name` (`Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.FluorescentHydra` / `…GiantTigerShrimp` / `…CannonBarnacle`);
  - **approved art for three `ModNPC` sprites** — blocker only, no placeholder art created (D-48/D-51);
  - the D-21 client checks (spawn isolation, clean load) in `04-UAT.md`;
  - if the designer later specifies these creatures, the shell is the starting point: adding behaviour means adding an `AI()` body and changing `aiStyle`, which is a design-driven change and not part of the D-49 migration.
- **`REQUIREMENTS.md` stays Pending** until plan 04-09 closes the phase.

---

*Phase: 04-remaining-ordinary-monsters*
*Completed: 2026-09-16*

## Self-Check: PASSED

- FOUND: `.planning/phases/04-remaining-ordinary-monsters/04-05-SUMMARY.md`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GiantTigerShrimp.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/CannonBarnacle.cs`
- FOUND commit: `3e40c19c9` (Task 1)
- FOUND commit: `a19c0863b` (Task 2)
- FOUND commit: `e17230dd3` (Task 3)
- FOUND commit: `63f1c0f07` (`plan_head_before`)
