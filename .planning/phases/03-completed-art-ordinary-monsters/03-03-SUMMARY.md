---
phase: 03-completed-art-ordinary-monsters
plan: 03
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, local-ai, state-machine, subworld, yggdrasil, kelp-curtain, enemy-projectile, loot-table, vulnerability-window, biology-matrix]

# Dependency graph
requires:
  - phase: 03-completed-art-ordinary-monsters
    plan: 01
    provides: the 31-row biology matrix + ASCII gate (D-24), the class shape to mirror (MossyThornTurtle), the server-safe KelpCurtainBiome.IsKelpCurtainLayer(Player) predicate and the spawned `Projectiles/Enemies/` tree
  - phase: 03-completed-art-ordinary-monsters
    plan: 02
    provides: the second/third local-AI class shapes (GuppyConch, VerdantRods) and the "empty/partial loot table" precedent
  - phase: 01-item-inventory-completed-art-items
    provides: ArmOfGiantTree, HardenedWitherbarkHeart and the shared Commons.ModAsset.White_Mod fallback
  - phase: 02-remaining-items-unfinished-art-materials
    provides: BoulderCatapult (the third guaranteed drop) and the art-deferral fallback convention
provides:
  - Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion (巨树人, full implementation, D-28)
  - Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.GiantDandelion_Shockwave
  - Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.GiantDandelion_Boulder
  - the hostile-enemy-projectile pattern (NPC.GetSource_FromAI + server-authority guard + White_Mod art deferral) for the Kelp Curtain tree
  - the first real multi-item drop table in the tranche (all three wired drops exist as Phase 1-2 items)
affects: [03-04, phase-4-remaining-ordinary-monsters, phase-7-bosses, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (68000 tokens, 3 tasks) to calibrate future estimates.
# chars/4 over the files actually changed, never a harness token count.
actuals:
  tokens: 30288
  tasks: 3
  commits: 3   # measured at SUMMARY write: git rev-list --count 6d076409f421696e4d675f2c9ea39bf6c5c42120..HEAD (3 task commits; the plan-metadata commit is the +1 verify-work allows)
plan_head_before: 6d076409f421696e4d675f2c9ea39bf6c5c42120

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Multi-range NPC state machine (close smash / mid-range thrown projectile / long-range chase) as a private enum over a single wrapped NPC.ai[0], with timers in wrapped NPC.localAI[] and a cooldown that enforces the design's minimum gap between mid-range attacks"
    - "A timed NPC state that owns a defense swing (30 -> 0 -> 30) inside its own EnterState helper, so every exit path restores the normal value and the transition stays server-authoritative"
    - "Hostile enemy projectiles written beside the existing D-13 fallback-texture convention when no approved art exists, with the art gap raised as a named blocker instead of created as placeholder art"
    - "Backwards arm raise -> fast forward swing -> recovery expressed as three short states rather than one blended timer, so the design's 120-tick wind-up and 120-tick recovery stay independently observable"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Boulder.cs
  modified:
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md
    - .planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md

key-decisions:
  - "Task 1 deliberately shipped with the smash and boulder spawns as clearly-marked compile-safe seams and Task 2 filled them: the plan's staging exists so the state machine is proven against a compiled class before the projectile types exist, and the seams are not stubs (none remain after Task 2)"
  - "The defense swing lives in EnterState, not in two hand-written branches: comparing the previous state there means every exit from SmashRecovery — including one caused by the player dying mid-recovery — restores defense 30, so the vulnerability window can never leak"
  - "The 240-tick mid-range gap is enforced by re-arming the cooldown when a boulder is thrown rather than when the wind-up starts, so the guarantee is measured between attacks (the design's 至少240帧的间隔) instead of between wind-ups"
  - "Both projectiles reuse Commons.ModAsset.White_Mod and name the missing art in 03-DEVIATIONS.md section 9; creating art is forbidden by AGENTS.md and the plan"
  - "BoulderCatapult_Proj was not reused: it is a friendly, player-owned ranged projectile whose ownership model and 150% direct-hit bonus invert under a hostile spawn"
  - "Spawn gating reuses the plan-03-01 server-safe KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player) plus SubworldSystem.IsActive<YggdrasilWorld>() and land conditions; IsBiomeActive is never called from a spawn hook because its camera term is zero on a dedicated server"
  - "The two plan-prose API names that do not exist in this tML version were corrected again (spawnInfo.player -> NPCSpawnInfo.Player, NPC.rare -> NPC.rarity), the same Rule 1 corrections plan 03-01 recorded"
  - "NPC.value is 25000 copper for 2金50银 with no conversion, and the 100x reading 250000 is explicitly rejected in 03-DEVIATIONS.md section 4"

patterns-established:
  - "A design row whose timings are design-exact (120/300/240) records only the unsupplied values as conservative defaults, so the ledger separates design truth from executor choice"
  - "A sprite-derived hit box is flagged as a runtime spawn-area question when it is unusually large (214x263), rather than silently shrinking the design's creature"
  - "Every authoritative projectile spawn is wrapped in Main.netMode != NetmodeID.MultiplayerClient so clients sync it instead of duplicating it"

requirements-completed: []  # advanced, not completed: phase 03 closes in plan 03-04
requirements-advanced: [BIO-02, BIO-06, QUAL-03]

# Coverage metadata (#1602)
coverage:
  - id: D1
    description: "巨树人 (GiantDandelion) core end-to-end: subworld-isolated rare spawn, the 120-tick smash wind-up, the 300-tick recovery with defense 0 and 150% incoming damage, the 5-15 tile boulder branch with its 240-tick gap, the beyond-15-tile faster chase, and three guaranteed Phase 1 drops"
    requirement: BIO-02
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exits 0, packages Everglow.tmod)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariants 5/7/8 (class resolution, SpawnChance tokens, Main.dedServ, item-type resolution)"
        status: pass
    human_judgment: true
    rationale: "Range switching, animation timing, the smash feel, the vulnerability window and drop acquisition are runtime gameplay properties; the D-21 client bundle in plan 03-04 records the run."
  - id: D2
    description: "The two hostile enemy projectiles (GiantDandelion_Shockwave, GiantDandelion_Boulder) and their authoritative wiring into the state machine at the design's 50/60 damage rows, with no new art, dust, gore, buff or item scope"
    requirement: BIO-02
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exits 0)"
        status: pass
      - kind: other
        ref: "token inspection: Projectile.NewProjectile(NPC.GetSource_FromAI() present for both types, each inside a Main.netMode != NetmodeID.MultiplayerClient guard; both files carry Commons.ModAsset.White_Mod and Main.dedServ and no ModContent.ItemType<"
        status: pass
    human_judgment: true
    rationale: "Projectile motion, hit areas and visual readability are runtime properties; the D-21 client bundle in plan 03-04 records the run."
  - id: D3
    description: "The row/mirror/ledger reconciliation: bio-spiny-moss-court-giant-tree-man flipped to code_complete with its internal_name in the same edit, the four precise blockers, and the 03-BIOLOGY.md row regenerated with a byte-identical blocker cell"
    requirement: BIO-06
    verification:
      - kind: other
        ref: "check-biology.ps1 prints 'OK(0): phase3 tranche = 5 / 5 (rows=31)' and 'OK: implemented classes = 5 / 5'"
        status: pass
      - kind: other
        ref: "cell-by-cell JSON/MD parity check (all 31 rows: status match = 0 diffs; the row's blocker cell equals the JSON blockers joined by '; ')"
        status: pass
    human_judgment: false
  - id: D4
    description: "03-DEVIATIONS.md extended (sections 3/4/5/6/7/8 as-built additions plus a new section 9 recording the two enemy projectiles, the White_Mod art deferral and why BoulderCatapult_Proj was not reused)"
    verification: []
    human_judgment: true
    rationale: "It records design-facing assumptions, a raised artwork blocker and runtime questions that only a designer or a client run can settle; no offline check can validate the intent."

# Metrics
duration: 11min
completed: 2026-09-15
status: complete
---

# Phase 3 Plan 03: 巨树人 Summary

**Rare subworld-only 巨树人 (`GiantDandelion`) with its design-exact 120/300/240-tick three-range state machine, a defense-0 post-smash vulnerability window, two hostile `White_Mod` enemy projectiles and three guaranteed Phase 1 drops**

## Performance

- **Duration:** ~11 min (wall clock)
- **Started:** 2026-09-15T08:15:56Z (inherited base `6d076409f`)
- **Completed:** 2026-09-15T08:26:28Z
- **Tasks:** 3
- **Files modified:** 6 (3 new classes, 3 planning artifacts)

## Accomplishments

- `GiantDandelion.cs` implements 巨树人 end-to-end beside its tracked `GiantDandelion.png` (so tML's default texture resolution finds the approved art with no `Texture` override and no asset move): registration with `NPCSpawnManager`, a subworld-only `SpawnChance` gated on `SubworldSystem.IsActive<YggdrasilWorld>()` **and** the server-safe `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` plus land conditions (rejecting `spawnInfo.Water` and liquid spawn tiles) at a deliberately rare `0.25f`, and a local `AI()` whose `GiantDandelionState` enum is wrapped over `NPC.ai[0]` with named timer wrappers over `NPC.localAI[0]`/`NPC.localAI[1]` — no bare numeric `NPC.ai[]` index anywhere outside those wrappers.
- The full documented behaviour exists: within 5 tiles the creature commits to a 120-tick arm raise that ends in the ground smash, then a fixed 300-tick immobile recovery in which `NPC.defense` is 0 and `ModifyIncomingHit` scales `FinalDamage` by `1.5f`; the recovery ends with a thrown boulder and repeats only if the player is still inside 5 tiles. Between 5 and 15 tiles it approaches and, on its own cooldown, pauses for a 120-tick backwards arm raise, swings forward (`BoulderThrow`), recovers the arms over 120 ticks and resumes, with the 240-tick minimum between mid-range attacks enforced by re-arming the cooldown at each throw. Beyond 15 tiles it chases at `3.2f` against the approach's `1.5f`.
- `GiantDandelion_Shockwave.cs` and `GiantDandelion_Boulder.cs` are hostile projectiles under the existing `Projectiles/Enemies/` tree: the wave is a `200x32` (~2 tiles high) floor-anchored 40-tick ground wave with `tileCollide = false` and `penetrate = -1`; the boulder is a gravity-arc, velocity-rotating, single-pierce projectile that despawns on the first tile it strikes. Both request only the existing shared `Commons.ModAsset.White_Mod`, contain no `ModContent.ItemType<`, and keep every dust call inside `if (!Main.dedServ)`. Each is spawned with `Projectile.NewProjectile(NPC.GetSource_FromAI(), ...)` inside a `Main.netMode != NetmodeID.MultiplayerClient` guard, carrying the design's own damage row (50 shockwave / 60 boulder).
- `ModifyNPCLoot` wires exactly three guaranteed rules, all to implemented items — `ArmOfGiantTree`, `HardenedWitherbarkHeart` and `BoulderCatapult` — making 巨树人 the first tranche row with a real multi-item drop table. The design's absent 4~6 枯木碎块 produces no type reference at all and lives as a row blocker and a ledger entry (D-37/D-39).
- `03-BIOLOGY.json` / `03-BIOLOGY.md` record the row as `code_complete: true` with `internal_name` `Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion`, `frame_count` 1, `mapping_confidence: "assumed"` and the four precise blockers, while the frozen counts (31 rows, phase3 5, phase4 23, phase7 3, deferred 2, `texture_complete_true` 6, `design_art_true` 9), the five `phase3_tranche` ids and the two frozen sets are unchanged. The mirror row was regenerated with a blocker cell byte-identical to the JSON join.
- The wave gate closed at `OK: implemented classes = 5 / 5`: every Phase 3 tranche row is now `code_complete: true` with its class on disk, which is exactly the state plan 03-04's `-RequireAll` run freezes. No `.png` or other binary asset was created, moved, renamed or modified; no HJSON was edited; the Feishu design source was not touched.

## Task Commits

Each task was committed atomically:

1. **Task 1: GiantDandelion core — subworld-gated rare spawn, three-range state machine, smash vulnerability and drop table** — `16a41354c` (feat)
2. **Task 2: The smash shockwave and the thrown boulder — two enemy projectiles wired into the state machine** — `87777df22` (feat)
3. **Task 3: Reconcile the giant tree man row, mirror and ledger; close the wave gate** — `5ad16f4ce` (docs)

**Plan metadata:** one further commit (`docs(03-03): complete ...`) carrying this SUMMARY, `STATE.md`, `ROADMAP.md`, `state.json` and `WINDOWS.md`; it is the fourth commit in the `plan_head_before..HEAD` range and its hash is recorded in the completion report.

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs` — `Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs` — `Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.GiantDandelion_Shockwave`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Boulder.cs` — `Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.GiantDandelion_Boulder`
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — the row flipped to `code_complete: true` with its `internal_name` and four blockers
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — the same row regenerated in the mirror
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — sections 3/4/5/6/7/8 extended and a new section 9 added

## Decisions Made

- **Task 1's seams were the plan's staged design, not stubs.** Task 1 shipped the smash and boulder seams as clearly-marked comments naming the two exact projectile types the calls had to become, so the state machine compiled and could be proven before the projectile classes existed; Task 2 replaced both seams with real server-guarded spawns and no placeholder comment remains (`SEAM` count is 0). This is the plan's own ordering, recorded so a later reader does not read the Task 1 intermediate state as unfinished work.
- **The defense swing is owned by `EnterState`.** Comparing the previous state there means *every* exit from `SmashRecovery` — including the edge case where the player dies mid-recovery and the creature falls back to `Idle` — restores `NPC.defense = NormalDefense`, with `NPC.netUpdate = true`. A hand-written "restore on exit" branch would have had three exit paths and one leak.
- **The 240-tick gap is armed at the throw.** Re-arming the cooldown inside `ThrowBoulder` (which both the mid-range swing and the post-recovery yank call) measures the design's 至少240帧的间隔 between *attacks*. Arming it when the wind-up started would have measured it between wind-ups and allowed the attacks closer together than the design states.
- **`TargetClosest` is suppressed only during `SmashRecovery`.** The design locks the creature to the floor for those 300 ticks, so re-acquiring a target then would let it pivot away from a committed smash. Every other state refreshes the target normally, and `IsTargetValid()` bounds-checks the engine's "no target" sentinel before reading `Main.player[]`.
- **The two projectiles reuse the shared fallback texture.** No approved art exists for either attack and AGENTS.md forbids creating placeholder art, so both use `Commons.ModAsset.White_Mod` — the same D-13 policy `BoulderCatapult` and `RadialCarapace` already follow — and the missing ground-wave and boulder sprites are raised as a named artwork blocker in `03-DEVIATIONS.md` §9.
- **`BoulderCatapult_Proj` was not reused as the creature's attack.** It is a friendly, player-owned ranged projectile (`owner`-relative bonus, `Main.myPlayer == Projectile.owner` sub-projectile spawning); a hostile spawn inverts its ownership model, and the plan's prohibitions forbid it. `GiantDandelion_Boulder` imitates only its arc-and-rotate motion.
- **Rarity is a reading, not a design statement.** This row's stats table is headerless with six cells and its leading cell reads `稀有`; the sibling tables' 稀有度 column is empty and absent here, so `NPC.rarity = ItemRarityID.LightPurple` is recorded in §4 as a conservative reading. `NPC.rare` (the name the plan prose used) does not exist in this tML version.
- **`NPC.value` is 25000.** The design's 钱币（铜） column is copper and `NPC.value` is documented in copper coins, so 2金50银 transposes 1:1; the 100x reading 250000 is explicitly rejected in §4 because it would exceed this repository's bosses (32000–81000).
- **Requirements are advanced, not completed.** This plan advances BIO-02, BIO-06 and QUAL-03, but the phase closes only when plan 03-04 runs the `-RequireAll` gate and the D-21 UAT bundle, so `REQUIREMENTS.md` is again left untouched and the traceability rows stay Pending (the plan-03-01/03-02 precedent).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Two non-existent API members named by the plan prose were corrected**
- **Found during:** Task 1 (authoring `GiantDandelion.cs`)
- **Issue:** The plan (and its acceptance criterion) wrote `IsKelpCurtainLayer(spawnInfo.player)` and `NPC.rare = ItemRarityID.LightPurple;`. `NPCSpawnInfo` exposes `Player` as a public **field** (there is no lowercase `player` member) and `Terraria.NPC` has no `rare` member — its only rarity field is `rarity`. Both literals would fail to compile.
- **Fix:** Used `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` (the form plan 03-01 already corrected and the form the in-repo precedent `RiverSlug.cs` uses) and `NPC.rarity = ItemRarityID.LightPurple`. Both corrections were already recorded as a table in `03-DEVIATIONS.md` §4; plan 03-03 appended a note that it applied the same two corrections.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0; the acceptance tokens (`IsKelpCurtainLayer(spawnInfo.Player)`, `ItemRarityID.LightPurple`) are present.
- **Committed in:** `16a41354c` (Task 1 commit)

### Plan-text observations that change no behaviour

**2. The plan's Task 1 wording "leave a temporary compile-safe placeholder seam that Task 2 wires" is exactly what shipped, so the seams are not defects.** Task 1 contained no `ModContent.ProjectileType<...>()` reference at all — only comments naming the two future types — and Task 2 removed those comments. The `SEAM` marker count in `GiantDandelion.cs` is 0 after Task 2, which is the Task 2 acceptance criterion.

**3. The plan's Task 1 `NPC.rare` and signature text for `SpawnChance` is the plan's own prose, corrected as deviation 1.** No behavioural difference from the plan's stated intent.

---

**Total deviations:** 1 auto-fixed (Rule 1, two non-existent API members) plus 2 plan-text observations that change no behaviour.
**Impact on plan:** None on scope, intent or observable behaviour. The spawn gate is the plan's, the rarity is the plan's conservative reading, and the build is clean.

## Issues Encountered

- **The sprite-derived extents are large.** `GiantDandelion.png` is 214x263, so the class uses `NPC.width = 214` / `NPC.height = 263` (about 13x16 tiles) under the tranche's sprite-derived-extents rule. That is bigger than this repository's bosses' hit boxes and could make a valid spawn area hard to find in the Kelp Curtain swamp, independent of the deliberately low `0.25f` weight. It is **not** silently shrunk here; it is recorded as an explicit runtime question in `03-DEVIATIONS.md` §8 (if the creature never spawns, the extents are the first thing to revisit, not the weight).
- **The Task 3 gate BOM count changed from 7 to 4 files.** The gate's BOM invariant scans the phase's own git status change set; once the three new `.cs` files were committed they dropped out of that set, so the count is smaller after Task 3 than after Task 2. The invariant is still green and nothing regressed.
- No other problems. No fix-attempt limit was reached and no issue was left deferred.

## User Setup Required

None - no external service configuration required.

## Known Stubs

None. All three classes are full implementations. The only deliberate omissions are design-sourced: the absent 4~6 枯木碎块 drop is a comment plus a matrix blocker (never a type reference), and the two attack projectiles have no approved art, so they use the existing shared fallback texture and raise the gap as a named blocker rather than generating placeholder art. Neither is an unfinished stub under the D-37/D-39 contract.

## Threat Flags

None. No new network endpoint, auth path, file-access pattern or trust-boundary schema was introduced. The only new surface is the pair of hostile projectiles, which sit entirely inside the existing `NPC.GetSource_FromAI()` + `Main.netMode != NetmodeID.MultiplayerClient` authority boundary and request only the already-shipped `Commons.ModAsset.White_Mod` asset path.

## Verification

- `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 (0 errors, 0 warnings) and packages `Everglow.tmod`, after each of the three tasks.
- `scripts/check-biology.ps1` exits 0 after every task and prints `OK(0): phase3 tranche = 5 / 5 (rows=31)`; after Task 3 it prints `OK: implemented classes = 5 / 5` — all five tranche rows are now `code_complete` with their classes on disk, which is the state plan 03-04's `-RequireAll` run freezes.
- `GiantDandelion.cs` contains `NPCSpawnManager.RegisterNPC`, `SubworldSystem.IsActive<YggdrasilWorld>`, `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`, `NPC.aiStyle = -1`, `NPC.lifeMax = 500`, `NPC.defense = 30`, `NPC.knockBackResist = 0f`, `NPC.value = 25000;` (with its terminating semicolon, so a stray `250000` cannot satisfy it by substring), `ItemRarityID.LightPurple`, `ModifyIncomingHit` with a `1.5f` scale, the 120/300/240 constants, three `ModifyNPCLoot` rules and two `Projectile.NewProjectile(NPC.GetSource_FromAI()` spawns. It contains no `Main.LocalPlayer`, no `Texture` override, no handwritten asset path, no base class or interface, no 枯木碎块 type reference and no `SEAM` marker.
- Both projectile files carry `Commons.ModAsset.White_Mod`, `Projectile.hostile = true`, `Main.dedServ`, and no `ModContent.ItemType<`; both are under `Projectiles/Enemies/`.
- The only `NPC.ai[]` / `NPC.localAI[]` occurrences in `GiantDandelion.cs` are the three wrapper properties (and the doc comments describing them); no bare numeric index appears in any state-machine branch.
- `03-BIOLOGY.json` parses with 31 rows, phase3 = 5, `texture_complete` = 6, `design_art` = 9; the edited row reads `code_complete: true`, `internal_name` `Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion`, `frame_count` 1, `mapping_confidence` `assumed`, `status` `unchecked` and four blockers. A cell-by-cell JSON/MD parity script reported 31 rows, 0 status diffs, 13 cells per row and a blocker cell byte-identical to the JSON row's blockers joined by `; `. `03-BIOLOGY.json`, `03-BIOLOGY.md` and `03-DEVIATIONS.md` all remain UTF-8 without BOM and LF-only (the gate's invariant 13 is green).
- `03-DEVIATIONS.md` retains sections 1–8 with the plan 03-03 additions and gains section 9; sections 3/4/5/6/7/8 were extended, never restructured.
- No `.png` or other binary asset was added, moved, renamed or modified (`git diff --name-only` over the plan range matches no `.png`/`.obj`/`.atlas`/`.xnb`/`.ttf`/`.hjson` path), no file was deleted, no HJSON was edited and the Feishu document was not touched.
- The D-21 client checks for the creature and its two projectiles are recorded as **not yet executed** in `03-DEVIATIONS.md` §8 and in `WINDOWS.md`, and are referred forward to plan 03-04's UAT bundle.

## Next Phase Readiness

- Wave 4 (plan 03-04) precondition is satisfied: the matrix now has all five `code_complete: true` tranche rows, the gate reports `implemented classes = 5 / 5`, and `-RequireAll` is now satisfiable.
- Plan 03-04 must run the `-RequireAll` gate, bundle the D-21 client checks (including the two new open runtime questions this plan recorded: the `214x263` spawn-area question and the floor-anchored wave's hit box on uneven terrain), and confirm the dedicated-server launch shows no dust from either projectile and no duplicated authoritative spawns.
- The row's blocker shape is the final form Phase 4 and Phase 8 will consume: three wired Phase 1 items, the absent 枯木碎块, provisional effect parameters, and the D-30 partial spawn context.

## Self-Check: PASSED

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs` exists beside the tracked `GiantDandelion.png`.
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs` and `GiantDandelion_Boulder.cs` exist.
- `03-BIOLOGY.json` parses with 31 rows and the row is `code_complete: true` with a resolving `internal_name`.
- `03-BIOLOGY.md` has 31 rows whose statuses and blocker cell match the JSON.
- `03-DEVIATIONS.md` has sections 1–9.
- `scripts/check-biology.ps1` exits 0 with `OK(0): phase3 tranche = 5 / 5 (rows=31)` and `OK: implemented classes = 5 / 5`.
- Commits `16a41354c`, `87777df22` and `5ad16f4ce` exist on `Yggdrasil/newContent0-ai`.

---
*Phase: 03-completed-art-ordinary-monsters*
*Completed: 2026-09-15*
