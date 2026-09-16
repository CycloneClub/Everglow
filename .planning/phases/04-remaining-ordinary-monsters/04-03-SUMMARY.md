---
phase: 04-remaining-ordinary-monsters
plan: 03
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, subworld, yggdrasil, kelp-curtain, group-spawn, stat-variant, neutral-ai, death-blast, spawn-predicate, loot-table]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    provides: the Phase 4 gate `scripts/check-biology.ps1` and its monotone `OK: guarded classes =` counter, `KelpCurtainSpawnConditions` (the three server-safe water predicates plus the D-54 weight bands) and the `NPCs/DeathJadeLake/` region folder with `JadeSpiritAnglerfish`
  - phase: 04-remaining-ordinary-monsters
    provides: plan 04-02's `ToxicToad` / `ToxicToad_PoisonCloud` (the `HitEffect`-spawns-a-projectile-on-death pattern and the `!Main.dedServ` dust shape), `GlowSalamander` and the `RiverSlug` foe-rule precedent
  - phase: 03-completed-art-ordinary-monsters
    provides: `RiverSlug.cs` (the critter/capture precedent that must NOT be copied where no catch item exists) and `KelpCurtainBiome.IsKelpCurtainLayer(Player)`
provides:
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs (装甲虾; the phase's first bounded group spawner)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/BombJellyfish.cs + LargeBombJellyfish.cs (爆弹水母 小/大; the phase's first stat-variant row)
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.cs (the shared 30/50 death blast, OQ4 item 9)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs (帆鳍鳢; the phase's first neutral row)
  - the guarded-class counter advanced 27 -> 32 (exactly the five files this plan adds)
affects: [04-04, 04-09, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (60000 tokens, 3 tasks, confidence low) to calibrate future estimates.
# chars/4 over the realized diff of the five created files, never a harness token count.
actuals:
  tokens: 17146
  tasks: 3
  commits: 5   # measured: git rev-list --count c2904bf5f..HEAD (3 feat + 1 refactor + 1 plan-metadata commit)
plan_head_before: c2904bf5f16ed0df1ff7c5cf837a06cf53b628c7

tech-stack:
  added: []
  patterns:
    - "A group spawn expressed with an npc-level follower marker passed through NPC.NewNPC's ai1 parameter, so the member reads it during its own OnSpawn and cannot start a second group"
    - "A stat variant as two sibling classes that both own their lifeMax in SetDefaults, with the shared behaviour (hover, blast) factored into one enemy projectile"
    - "A neutral creature whose aggro is a bounded window written from HitEffect, so the design's 获得仇恨 reading cannot become a permanent aggressor"
    - "A shallow/deep water split approximated by a local bounded liquid-column scan mirrored from the shared water-bottom probe"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/BombJellyfish.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeBombJellyfish.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.cs
  modified:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs (guard form normalised in the refactor commit)

key-decisions:
  - "装甲虾's 2-5 group is created in OnSpawn on the authoritative side with the follower marker passed through NPC.NewNPC's ai1 parameter, so the member already reads it during its own OnSpawn (T-04-17); a private static groupCreationDepth counter is the belt-and-braces guard in case the engine applied the ai[] values in a different order"
  - "爆弹水母's 小/大 split is two classes (BombJellyfish 生命 10 / LargeBombJellyfish 生命 25, OQ2/Pitfall 5); nothing mutates NPC.lifeMax outside SetDefaults and the blast's 30/50 lives in the two creature classes, never in the projectile"
  - "The death blast is built rather than blocked (OQ4 item 9): one shared BombJellyfish_Explosion whose radius scales from the named ai[0] value the dying creature passes, spawned exactly once through a HasDetonated localAI flag"
  - "帆鳍鳢's 获得仇恨 is read as aggro-on-damage refreshed by further hits and bounded by the public const AggroWindowFrames = 300, written from HitEffect (never ModifyIncomingHit) under the netmode guard with NPC.netUpdate - the D-54 conservative reading, listed for 04-DEVIATIONS.md section 10 by plan 04-09"
  - "Both jellyfish variants keep the row's empty 伤害 cell at 0 (the blast is the only threat) and honour 免疫击退 with knockBackResist = 0f; the empty 防御 and 击退抗性 cells take documented D-54 defaults (2 and 0.8f)"
  - "The 浅水区/深水区 split is approximated with a local bounded 24-tile downward liquid-column scan mirrored from KelpCurtainSpawnConditions.IsWaterBottom rather than by extending that 04-01 helper, whose file is outside this plan's files_modified; the threshold is 6 tiles"
  - "No catch item is invented anywhere (D-58): all four creatures write NPC.catchItem = 0 with no Main.npcCatchable and no NPCID.Sets.CountsAsCritter, and the absent 软体甲壳碎片 / 亡碧膏 / capture items stay commented blockers with no type reference"
  - "03-BIOLOGY.json / 03-BIOLOGY.md and 04-DEVIATIONS.md are byte-identical to their end-of-04-01 state: this plan creates classes only, and plan 04-09 owns the matrix flips and the section 10/14 ledger writes"

patterns-established:
  - "Bounded group spawn: leader rolls the design's size, NPC.NewNPC passes the follower marker via ai1 so the member's own OnSpawn is already classified, and only a leader can roll"
  - "Stat variant pair: two SetDefaults-owned stat rows plus one shared projectile that reads its damage/radius from a named ai[] value rather than hard-coding either variant"
  - "Bounded neutral aggro: the window is written from the on-hit hook, decremented authoritatively, and leaked into no Player field"
  - "A second consumer shape for the on-death projectile: ToxicToad's cloud (04-02) and the jellyfish blast share the same GetSource_FromAI + netmode-guard + !Main.dedServ skeleton"

requirements-completed: []
requirements-advanced: [BIO-01]

coverage:
  - id: D1
    description: "装甲虾 (ArmoredShrimp): a passive aquatic group spawner confined to the Yggdrasil Kelp Curtain's water, appearing as a bounded 2-5 shoal that never flees, never targets and deals no damage, with the absent capture item and 软体甲壳碎片 drop recorded as blockers"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors, Everglow.tmod packaged)"
        status: pass
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 (exit 0; OK: guarded classes = 28 after Task 1, 27 before)"
        status: pass
    human_judgment: true
    rationale: "Whether the shoal reads as 2-5 creatures drifting together, whether the cascade really terminates in a live world and whether it never leaves the lake can only be observed in a tModLoader client; the plan's own D-21 client batch in 04-UAT.md (plan 04-09) carries those checks."
  - id: D2
    description: "爆弹水母 (BombJellyfish 小 + LargeBombJellyfish 大): two sibling classes that hover motionless and knockback-immune in the layer's water at a shallow/deep liquid column, are un-capturable, and detonate once on death for the design's 30 / 50 through the shared BombJellyfish_Explosion"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 (exit 0; OK: guarded classes = 31 after Task 2, +3 = the two creatures and the projectile)"
        status: pass
    human_judgment: true
    rationale: "Spawn isolation in the shallow vs deep band, the hover feel, the knockback immunity and whether the blast lands exactly once with the designed damage are runtime properties the offline gate cannot observe; they are the D-21 bundle in 04-UAT.md."
  - id: D3
    description: "帆鳍鳢 (SailfinSnakehead): a neutral cruiser that swims at a constant speed while unprovoked, becomes hostile only inside the bounded aggro window after taking damage, charges at its target, retreats from 幽光蝾螈 and rams 水蛞蝓, with an empty commented loot table (亡碧膏 absent)"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 (exit 0; OK: guarded classes = 32 after Task 3, +1)"
        status: pass
    human_judgment: true
    rationale: "The aggro trigger is a conservative reading of an under-specified design cell and the cruise/charge/retreat feel is an in-world property; both need the client run (04-UAT.md) and the designer's confirmation of the reading (04-DEVIATIONS.md section 10)."
  - id: D4
    description: "The plan's automated verify chain: the Release build plus the Phase 4 gate, which together also re-run the byte-identical Phase 3 gate under -RequireAll and the AGENTS.md byte-level UTF-8 BOM check over the whole change set"
    verification:
      - kind: other
        ref: "check-biology.ps1 exit 0 with 'OK(0): phase4 in-scope set = 21 rows (rows=31)', 'OK: reconciled rows = 1 / 21', 'OK: guarded classes = 32'"
        status: pass
      - kind: other
        ref: "phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1 -RequireAll (exit 0: 'phase3 tranche = 5 / 5 (rows=31)', 'implemented classes = 5 / 5')"
        status: pass
      - kind: other
        ref: "AGENTS.md byte-level UTF-8 BOM check against merge-base HEAD origin/master (1433 files, 0 BOM); gate invariant 13 reports its own phase-scoped BOM check passing"
        status: pass
    human_judgment: false
  - id: D5
    description: "The ledger records for what plan 04-09 must file in 04-DEVIATIONS.md: the conservative readings and defaults this plan opens (the shallow/deep column threshold and the local scan, the group-spawn marker and its static guard, the jellyfish empty-cell defaults, the 帆鳍鳢 aggro window/leash/charge cooldown, the explosion radius-per-damage) plus the D-21 runtime bundle"
    verification: []
    human_judgment: true
    rationale: "It records design-facing assumptions and blockers whose wording only the phase close-out (plan 04-09) writes into the ledger; no offline check can validate their intent."

duration: 54min
completed: 2026-09-16
status: complete
---

# Phase 4 Plan 03: Death Jade Lake Passive & Neutral Water Family Summary

**装甲虾's bounded 2–5 shoal, 爆弹水母's 小/大 sibling pair sharing one 30/50 death blast, and 帆鳍鳢's bounded aggro window — the phase's first group-spawn, stat-variant and neutral rows, with the gate advanced 27 → 32**

## Performance

- **Duration:** ~54 min (17:01 → 17:55 local; the plan's estimate was 60000 tokens with `confidence: low`, and the realized diff is 17146 tokens on the estimate's own scale)
- **Started:** 2026-09-16T09:01:58Z
- **Completed:** 2026-09-16T09:55:44Z
- **Tasks:** 3 of 3
- **Files modified:** 5 (5 created; 1 of them touched again by the refactor commit)

## Accomplishments

- **装甲虾 (`ArmoredShrimp`)** — the phase's first **group spawn**. `OnSpawn` runs only on the authoritative side and delegates to `CreateShoal()`, which rolls the design's 2–5 size and creates the extra members with `NPC.NewNPC(NPC.GetSource_FromAI(), …, Type, 0, 0f, FollowerMarker)`; the follower marker travels in **`ai1`**, so the member reads it during its *own* `OnSpawn` and never rolls a second group (T-04-17). A private `static int groupCreationDepth` counter is the belt-and-braces guard for the (unlikely) case that the engine assigned `ai0..ai3` in a different order. Everything else is the design's passivity: `damage`/`defDamage` 0, no `TargetClosest` call anywhere, a two-state `ArmoredShrimpState { Shoaling, Drifting }` shoal drift with follower cohesion and a wall turn-around, `NPC.rarity` for the empty 稀有度 cell, `catchItem = 0` with no `Main.npcCatchable` and no `CountsAsCritter`, and an empty commented `ModifyNPCLoot`.
- **爆弹水母 (`BombJellyfish` + `LargeBombJellyfish` + `BombJellyfish_Explosion`)** — the phase's first **stat-variant** row. Two sibling classes own their own `SetDefaults` (`lifeMax` 10 / 25, boxes 26×26 / 40×40, `DeepWaterWeight` 0.5f below the small variant's `WaterWeight` 0.75f), so `NPC.lifeMax` is never mutated outside `SetDefaults` (OQ2, Pitfall 5, T-04-18). Both hover in place — the horizontal velocity is damped to zero and a two-leg `Rising`/`Sinking` bob rides `NPC.ai[0]` with `NPC.localAI[0]` as the leg timer — and both are knockback-immune (`knockBackResist = 0f`, 免疫击退). `HitEffect` spawns **one** `BombJellyfish_Explosion` per death from `NPC.GetSource_FromAI()` under `Main.netMode != NetmodeID.MultiplayerClient`, guarded by a `HasDetonated` flag over `NPC.localAI[1]`, with `NPC.netUpdate` and `!Main.dedServ` dust. The projectile carries the design's damage as both its damage parameter **and** its `ai0`, so the radius (`RadiusPerDamage = 1.6f`, i.e. 3 tiles for 30 and 5 tiles for 50) is derived rather than hard-coded per class.
- **帆鳍鳢 (`SailfinSnakehead`)** — the phase's first **neutral** row. It cruises at a constant `CruiseSpeed` with no target at all; 获得仇恨 is read as **aggro-on-damage over a bounded window** (`public const int AggroWindowFrames = 300`, refreshed by every further hit and decremented authoritatively in `UpdateTimers`), written from `HitEffect` and never from `ModifyIncomingHit` (T-04-22). While the window is open it rams at `ChargeSpeed` with a `ChargeCooldownTicks` recovery and a `LeashRange`; a nearby `GlowSalamander` outranks everything and it retreats, while a nearby `RiverSlug` is its own prey and is rammed — both resolved through `ModContent.NPCType<…>` so a rename is a compile error. Design stats are exact (生命 80 — the 80/140/200 progression's first value, 伤害 30, 防御 12, 击退抗性 20 → `knockBackResist = 0.8f`, 钱币 5银 = 500) and `ModifyNPCLoot` is empty with 亡碧膏 named as an absent D-58 blocker.
- **All four creatures** carry the art-missing shape (`Texture => Commons.ModAsset.White_Mod`), `NPCSpawnManager.RegisterNPC`, `LocalizationCategory`, the two server-safe spawn tokens, the `spawnInfo.Water` requirement, `Main.dedServ`-guarded dust, `NPC.netUpdate` on every authoritative write, and an explicit `ModifyIncomingHit` no-op naming the design's empty 减伤 cell. **None** reads `Main.LocalPlayer`, **none** uses `KelpCurtainBiome.IsBiomeActive`, **none** contains a `ModContent.ItemType<…>` token and **none** mutates `NPC.lifeMax` outside `SetDefaults`.
- **The gate advanced by exactly five, 27 → 28 → 31 → 32**, one step per task and three at once for the jellyfish + projectile task — the five files this plan adds and no others.
- **Both frozen gates and the byte level checks stay green**: the Phase 4 gate prints `OK(0): phase4 in-scope set = 21 rows (rows=31)` / `OK: reconciled rows = 1 / 21` / `OK: guarded classes = 32` / `OK: UTF-8 BOM check passed`, the Phase 3 gate still exits 0 under `-RequireAll` (`phase3 tranche = 5 / 5`, `implemented classes = 5 / 5`), `03-BIOLOGY.json` / `03-BIOLOGY.md` are byte-identical to their end-of-04-01 state, and the AGENTS.md BOM check over the 1433-file change set reports 0 BOM.

## Task Commits

Each task was committed atomically:

1. **Task 1: 装甲虾 `ArmoredShrimp` — passive group spawner with a bounded group and a blocked capture** — `414ed8f48` (feat)
2. **Task 2: 爆弹水母 `BombJellyfish` + `LargeBombJellyfish` + `BombJellyfish_Explosion`** — `dcb907b95` (feat)
3. *(process)* **装甲虾 guard form normalised to the plan's `!=` spelling** — `2c8c320af` (refactor)
4. **Task 3: 帆鳍鳢 `SailfinSnakehead` — neutral cruiser that only charges once provoked** — `6e88b4baf` (feat)

**Plan metadata:** the SUMMARY + `STATE.md` + `ROADMAP.md` commit that follows it (the fifth commit in the `plan_head_before..HEAD` range).

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs` — 装甲虾: the bounded 2–5 shoal, the `ai1` follower marker, passive drift, no capture, no loot (399 lines)
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/BombJellyfish.cs` — 爆弹水母 (小): hover, 免疫击退, shallow-column spawn gate, 30-damage death blast
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeBombJellyfish.cs` — 爆弹水母 (大): the same shape with 生命 25, a deep-column gate, the lower weight and the 50-damage blast
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs` — 帆鳍鳢: cruise/charge/retreat, the bounded aggro window, the 蝾螈 avoid and 蛞蝓 attack rules, the empty loot table
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.cs` — the shared death blast; the design's 30/50 arrive through the damage parameter and the named `ai[0]`, and the radius is derived from them

## Decisions Made

- **The follower marker must arrive *during* `OnSpawn`, not after `NPC.NewNPC` returns.** The engine runs the new NPC's `OnSpawn` inside `NPC.NewNPC`, so a marker written after the call would be too late and the member would already have rolled its own group. `ai1` is documented as initialising `NPC.ai[]`, so the marker is passed there; the static `groupCreationDepth` counter makes the property true even if that ordering ever changed.
- **The blast's 30 and 50 live in the creature classes, never in the projectile.** Both variants call the same `BombJellyfish_Explosion` with their own damage as the damage parameter *and* as `ai0`; the projectile derives its radius from `ai[0]`, so no per-class constant and no second explosion class is needed (the OQ4 "shared class, one spawn per variant" shape that `AlgaeOctopus_InkCloud` also uses).
- **The neutral row's aggro is bounded in both time and space.** `AggroWindowFrames` bounds it in time (the design names no trigger at all), and `LeashRange` (50 tiles) keeps a provoked creature from being dragged out of its lake; both are D-54 defaults named in the class comment because this plan may not write `04-DEVIATIONS.md`.
- **The 浅水区/深水区 split is a local scan, not an extension of the 04-01 helper.** `KelpCurtainSpawnConditions` is not in this plan's `files_modified`, so each jellyfish class carries its own bounded 24-tile downward scan of the same shape as that helper's `IsWaterBottom` probe, with a 6-tile threshold that the two siblings read from opposite sides (so their conditions partition the column rather than overlapping).
- **The 森雨幽谷 half of 爆弹水母's small-only condition is a comment, not an approximation.** It needs a region predicate that does not exist (D-52), and `04-DEVIATIONS.md` section 5/13 already carries the generic region-predicate gap; approximating it with terrain Phases 5–6 have not built would have been the D-53 mistake.
- **No catch item is invented, for any of the four creatures** (D-58). Only `RiverSlug`'s capture is real in this repository, so the phase's capture column stays blocked and the classes are deliberately *not* capturable rather than falsely capturable with a dangling `catchItem`.
- **The matrix and the ledger are untouched.** `03-BIOLOGY.json` / `03-BIOLOGY.md` are byte-identical to their end-of-04-01 state and `04-DEVIATIONS.md` is unedited (this plan's prohibition), so the two conservative readings and the D-54 defaults this plan opens are recorded in the class comments, in `WINDOWS.md` entries 52–59 and in this SUMMARY for plan 04-09's section 10/14 pass.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Missing `using` in `BombJellyfish_Explosion.cs`**
- **Found during:** Task 2 (the first Release build of the new projectile)
- **Issue:** `error CS0246: KelpWaterDrop could not be found` — the new projectile referenced the shared Kelp Curtain dust without importing `Everglow.Yggdrasil.KelpCurtain.Dusts`.
- **Fix:** Added the import; the Release build then reported 0 warnings / 0 errors and the gate advanced 28 → 31.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 with 0 warnings and 0 errors.
- **Committed in:** `dcb907b95` (Task 2)

**2. [Rule 1 - API/token form] 装甲虾's netmode guard normalised to the plan's `!=` spelling**
- **Found during:** Task 3's acceptance audit (after Task 1 was already committed)
- **Issue:** Task 1's acceptance criteria require the literal token `Main.netMode != NetmodeID.MultiplayerClient`, but the committed class used the equivalent early-return form `Main.netMode == NetmodeID.MultiplayerClient`. An equivalent guard that fails a stated acceptance criterion is a defect in the deliverable, not a wording preference.
- **Fix:** `OnSpawn` now delegates to a new `CreateShoal()` under `if (Main.netMode != NetmodeID.MultiplayerClient)`, and `UpdateWander` wraps its body in the same form. No behavioural change. Committed separately rather than by amending, following the `refactor(04-07)` precedent for exactly the same normalisation.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs`
- **Verification:** Release build 0/0; gate still `guarded classes = 32`; the audit now finds two `!=` occurrences and zero `==` occurrences.
- **Committed in:** `2c8c320af` (dedicated refactor commit, not an amend)

**3. [Rule 2 - Missing critical functionality] A static creation-depth guard behind the `ai1` marker**
- **Found during:** Task 1 (`CreateShoal`)
- **Issue:** The plan pins the `ai1` follower marker as the mechanism that stops the group from cascading (T-04-17), but the marker is only correct if the engine initialises `NPC.ai[]` from `NPC.NewNPC`'s parameters *before* the member's `OnSpawn` runs. If that ordering ever differed, one 2–5 roll could recurse without bound — the exact failure the threat register names.
- **Fix:** Added a private `static int groupCreationDepth` incremented around the creation loop, so a member created inside the window is classified as a follower regardless of the ordering. The `ai1` marker remains the primary, synced mechanism and is what the acceptance criteria assert; the counter is a second, ordering-independent guard.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs`
- **Verification:** Release build 0/0; the marker is still written through `ai1` and still lands in the member's synced state; `WINDOWS.md` entry 57 records the design.
- **Committed in:** `414ed8f48` (Task 1) and the `!=` normalisation in `2c8c320af`

**4. [Rule 2 - Missing critical functionality] A `HasDetonated` guard for "exactly once on death"**
- **Found during:** Task 2 (`HitEffect`)
- **Issue:** Task 2's behaviour text requires the blast to spawn "exactly once on death", but `HitEffect` is called for each hit and the engine can report the same death more than once, while the authoritative guard alone does not de-duplicate a repeat call on the server.
- **Fix:** Added a named `HasDetonated` property over `NPC.localAI[1]`, set on the authoritative side immediately before the single `Projectile.NewProjectile` call in each variant. No combat effect; the flag only makes the "exactly once" clause true rather than probable.
- **Files modified:** `BombJellyfish.cs`, `LargeBombJellyfish.cs`
- **Verification:** Release build 0/0; the flag is written and read only inside the netmode-guarded block.
- **Committed in:** `dcb907b95` (Task 2)

### Process deviations

**5. The `tdd="true"` task attribute is realised as the gate's monotone counter plus the Release build**
- **Found during:** All three tasks
- **Issue:** Every task in this plan carries `tdd="true"`, but the repository has no unit-test infrastructure for `ModNPC` behaviour (spawn isolation, shoal cohesion, hover, aggro windows, netmode discipline) and the plan's own `<fails_when>` is written in terms of `error CS`, the gate's exit code and the `OK: guarded classes =` delta.
- **Fix:** Each task ran its full automated `<verify>` (Release build → Phase 4 gate) and was committed only when green, with the counter as the monotone progress instrument (27 → 28 → 31 → 32). No test was fabricated and no RED/GREEN pair is claimed. This mirrors plan 04-02, 04-06, 04-07 and 04-08, whose `tdd="true"` tasks were handled the same way.
- **Files modified:** none (recorded in `WINDOWS.md` entry 52)
- **Verification:** the full automated chain (Release build → Phase 4 gate → Phase 3 gate `-RequireAll`) exits 0 as quoted above.
- **Committed in:** n/a (process record)

**6. `04-DEVIATIONS.md` is read but not written, so this plan's register items are ledger-only for now**
- **Found during:** Task 1–3 close-out
- **Issue:** The plan asks for two things to be "recorded" — the 森雨幽谷 region-predicate gap (D-52) and the 帆鳍鳢 aggro-trigger reading (D-54) — while one of its `<prohibitions>` forbids creating or modifying `04-DEVIATIONS.md` in wave 3. The two instructions can only both be honoured by recording outside the ledger.
- **Fix:** Both are recorded in the class XML/`<summary>` comments (naming `04-DEVIATIONS.md` section 5 and section 10 respectively), in this SUMMARY's Decisions section, and in `WINDOWS.md` entries 54–59 so plan 04-09's close-out pass picks them up; section 5/13 already carries the generic region-predicate gap for the phase. Nothing was approximated to hide the gap.
- **Files modified:** none (the ledger is deliberately unedited)
- **Verification:** `git diff 7513c752f..HEAD -- .planning/phases/04-remaining-ordinary-monsters/04-DEVIATIONS.md` is empty.
- **Committed in:** n/a (process record)

**7. The `GlowSalamander` / `RiverSlug` lookups are reads of already-committed classes, not of a same-wave sibling**
- **Found during:** Task 3
- **Issue:** The execution brief warns against consuming types created by another plan, while this plan's own `key_links` ("`GlowSalamander` / `RiverSlug` type lookups → each creature's avoid / attack rule"), `<behavior>` ("moves away from 幽光蝾螈 … actively attacks 水蛞蝓") and `<fails_when>` ("an unresolvable `GlowSalamander` or `RiverSlug` type lookup") all require exactly those two lookups. The two readings only conflict if the referenced class is not yet on disk.
- **Fix:** Implemented the lookups as the plan specifies. Both classes were created by **plan 04-02** (committed `5493aed98`) and Phase 3 (`RiverSlug`), both are present and build-verified on this branch, and the reference is compile-time only — so no same-wave artifact is consumed and no unbounded/ordering hazard is introduced. The brief's "pattern-reference only" rule is honoured for the genuine same-wave sibling reads in this plan (the two jellyfish classes mirror `ArmoredShrimp`'s shape without naming it, and the explosion is the only cross-file type reference among the five new files).
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs`
- **Verification:** Release build 0/0 with both lookups resolved; `WINDOWS.md` entry 58 records the reasoning.
- **Committed in:** `6e88b4baf` (Task 3)

---

**Total deviations:** 3 auto-fixed (2 Rule 1 + 2 Rule 2 items counted as the correctness fixes above) + 3 documented process decisions
**Impact on plan:** No scope creep. Items 1–4 were required for the build to pass and for the plan's own "exactly once" / "bounded group" clauses to be true rather than probable; items 5–7 are records of how the plan's two under-specified instructions were reconciled with its prohibitions.

## Issues Encountered

- **`NPC.NewNPC`'s `ai[]` initialisation ordering is not stated in the task's own text.** Resolved by reading the installed `tModLoader.xml` (`ai0, ai1, ai2, ai3 will initialize the NPC.ai[] array with the supplied values`) and by adding the ordering-independent `groupCreationDepth` guard rather than assuming. The same documentation confirmed `Projectile.NewProjectile`'s `ai0..ai2` contract that the explosion's radius reads.
- **The plan's `NPC.rare` correction recurred for the eighth time.** All four creature classes write `NPC.rarity = ItemRarityID.White;`, matching `04-DEVIATIONS.md` section 10 and the seven precedents; no ledger edit was needed (`WINDOWS.md` entry 54).
- **Acceptance criteria that name a token which cannot exist at that site.** Task 2 asks `BombJellyfish_Explosion.cs` to contain `NPC.GetSource_FromAI`, but a `ModProjectile` exposes no `NPC` member. The token is realised at the two real spawn sites and named in the projectile's XML doc — the same recording the 04-02/04-06/04-07/04-08 siblings made (`WINDOWS.md` entry 56).
- **Console encoding noise.** PowerShell 5.1 renders the CJK class comments and documentary text as mojibake in the console. It is a display artefact only: the bytes on disk are UTF-8 without BOM, no `CR` bytes, tabs for indentation and a trailing `LF`, which the gate's byte checks and the AGENTS.md BOM block both confirm.

## Known Stubs

None — no hardcoded empty value that flows to a consumer, no placeholder string and no unwired data source was introduced. The art-missing classes use the shared `Commons.ModAsset.White_Mod` fallback by design (D-48) and carry a documented blocker per real repository fact, not a stub; each of the four explicit `ModifyIncomingHit` no-op bodies is the design row's own empty 减伤 cell, named in the comment and already in `04-DEVIATIONS.md` section 10.

## Defect Ledger

Eight entries were appended to `.planning/WINDOWS.md` for cross-phase visibility (they block `/gsd-ship` while open, by design; `open_count` 48 → 56):

- **#52 `unrun-verify`** — the `tdd="true"` realisation (gate counter + Release build).
- **#53 `unrun-verify`** — this plan's D-21 client runtime checks, for `04-UAT.md`.
- **#54 `deviation`** — the `NPC.rare` → `NPC.rarity` correction (eighth occurrence).
- **#55 `deviation`** — the register items plan 04-09 must file in section 10 (the aggro reading and the shallow/deep column approximation).
- **#56 `deviation`** — the `NPC.GetSource_FromAI` acceptance token realised at the spawn sites rather than inside the `ModProjectile`.
- **#57 `deviation`** — the `!=` guard-form normalisation and the `groupCreationDepth` / empty-cell defaults.
- **#58 `deviation`** — the `GlowSalamander` / `RiverSlug` lookups target already-committed classes, and the inter-creature damage gap stays per section 7.
- **#59 `deviation`** — the Task 2 `CS0246` build break and its Rule 1 fix inside the task.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- **The phase's three cross-cutting hazards now have a proven shape each:** a bounded group spawn (marker through `ai1`), a stat-variant pair (two `SetDefaults`-owned stat rows plus one shared projectile reading its numbers from `ai[]`), and a bounded neutral aggro (window written from `HitEffect`). Plan 04-04 and the close-out can inherit these rather than re-deriving them.
- **Plan 04-09's obligations this plan created, all recorded and none hidden:**
  - flip the three rows (`bio-death-jade-lake-armored-shrimp`, `bio-death-jade-lake-bomb-jellyfish`, `bio-death-jade-lake-sailfin-snakehead`) to `code_complete: true` with their `internal_name`s — note that the row names **one** class (`BombJellyfish`) while 大 ships as its sibling `LargeBombJellyfish`, exactly as section 2 already records;
  - file the two conservative readings and the D-54 defaults in section 10 (`WINDOWS.md` entries 54–55);
  - carry the D-21 runtime bundle into `04-UAT.md` (`WINDOWS.md` entries 53);
  - keep `REQUIREMENTS.md` Pending: BIO-01 is advanced but completed by no single plan.
- **Outstanding, all recorded, none hidden:** the D-21 client/dedicated-server bundle; **approved art for the 5 new sprites** (four `ModNPC` + one projectile) — blocker only, no placeholder art created (D-48/D-51); the 森雨幽谷 / 亡碧湖 region-level spawn predicates (Phases 5–6, D-52); the absent 软体甲壳碎片 / 亡碧膏 drops and the absent capture items for 装甲虾 and 爆弹水母 (D-58); localization (D-20).

---

*Phase: 04-remaining-ordinary-monsters*
*Completed: 2026-09-16*

## Self-Check: PASSED

- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/BombJellyfish.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeBombJellyfish.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.cs`
- FOUND commit: `414ed8f48` (Task 1)
- FOUND commit: `dcb907b95` (Task 2)
- FOUND commit: `2c8c320af` (refactor)
- FOUND commit: `6e88b4baf` (Task 3)
- FOUND commit: `c2904bf5f` (`plan_head_before`)
