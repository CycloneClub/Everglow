---
phase: 04-remaining-ordinary-monsters
plan: 04
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, subworld, yggdrasil, kelp-curtain, death-jade-lake, spawn-predicate, stealth-ai, grab-debuff, ink-projectile, hostile-projectile, loot-table]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    plan: 01
    provides: the Phase 4 gate (`scripts/check-biology.ps1`), `KelpCurtainSpawnConditions` (IsDryLand / IsWaterSurface / IsWaterBottom + the D-54 weight bands), the `DeathJadeLake` region folder + `White_Mod` art-missing shape and the 碧灵鮟鱇 tracer
  - phase: 04-remaining-ordinary-monsters
    plan: 02
    provides: the ranged-creature + hostile-projectile pattern (`ToxicToad` / `ToxicToad_PoisonBubble`), the unguarded `OnHitPlayer` debuff shape and the authoritative-side `AddBuff` suffocation path
  - phase: 01-item-inventory-completed-art-items
    provides: the read-only `evidence/biology.xml` snapshot (D-25) and the `RadialCarapace` item type
provides:
  - Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.Radiolarian (放射虫, ranged-fire-dash predator)
  - Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.AlgaeOctopus (覆藻章鱼, stealth ink ambusher)
  - Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.LargeAlgaeOctopus (大型覆藻章鱼, bottom grabber)
  - Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.Radiolarian_WaterBolt
  - Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.AlgaeOctopus_InkCloud (shared by both octopuses)
affects: [04-09, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (60000 tokens, 3 tasks) to calibrate future estimates.
# chars/4 over the added lines of the plan's own commits (840bb4876..HEAD), never a harness token count.
actuals:
  tokens: 18965
  tasks: 3
  commits: 4   # measured: git rev-list --count 840bb4876..HEAD after the plan-metadata commit (3 task commits + 1 plan-metadata commit)
plan_head_before: 840bb4876aa87dddbffa5314fba8c39351975be1

tech-stack:
  added: []
  patterns:
    - "A squared-distance dash trigger (`DashTriggerTiles * 16f`, squared at compile time) so a client and a server agree about a 4-tile range with no square root and no camera value (D-55, T-04-26)"
    - "A distance-driven stealth `NPC.alpha` derived from the synced NPC centre and the synced target centre, linearly fading in below an observation range and clamped 0-255"
    - "One hostile projectile class parameterised through a synced `Projectile.ai[0]` damage value, so 覆藻章鱼's 25 and 大型覆藻章鱼's 60 share one class instead of two"
    - "A grab implemented as timed debuffs only (120 ticks, re-applied every 60 frames) on the authoritative side, so no player can be left permanently held if the creature dies mid-grab (T-04-27)"
    - "An ink wave as four spawns of one shared projectile (the first aimed, three random) rather than a dedicated wave class"
    - "A bounded upward liquid-column probe for the 浅水区 spawn band, mirrored from `KelpCurtainSpawnConditions`' downward water-bottom probe"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/AlgaeOctopus.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeAlgaeOctopus.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/Radiolarian_WaterBolt.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AlgaeOctopus_InkCloud.cs
  modified: []

key-decisions:
  - "Only the three classes this plan owns are referenced: no `ModContent.NPCType<...>()` points at a class created by another plan, so the design's 蝾螈 prey preference and the 鮟鱇 exclusion are left as the inter-creature hostility already recorded as unmodelled in 04-DEVIATIONS.md sections 7 and 13"
  - "The 放射虫 dash disengage reuses the one `ManeuverTimer` (`NPC.localAI[2]`) for both the charge and the retreat instead of adding a fourth localAI slot"
  - "The 大型覆藻章鱼 grab applies 束缚/窒息 on the authoritative side (`Main.netMode != MultiplayerClient`), the same sync path as the 04-02 幽光蝾螈 suffocation implementation, rather than through the client-only `OnHitPlayer` hook, because the design repeats the debuff on a 60-frame cadence rather than on contact"
  - "The ink cloud keeps its damage on the synced `Projectile.ai[0]` and re-asserts `Projectile.damage` every tick, because damage is not part of the projectile net message"
  - "The 4-cloud wave is four spawns of `AlgaeOctopus_InkCloud` (a recorded multiplicity simplification of a pure-VFX effect, never of the damage, 04-DEVIATIONS.md section 7 item 8)"
  - "The ink cloud's lingering rendering is an effect blocker (OQ4) named in the class comments; its damage and its 2 s of 黑暗 are implemented and no client VFX class was added"
  - "The two `NPC.GetSource_FromAI` acceptance tokens are realised at the real spawn sites in the NPC classes and named in each projectile's XML doc, the plan 04-02/04-06/04-07/04-08 precedent (a `ModProjectile` exposes no `NPC` member)"

patterns-established:
  - "Range logic that must agree on both sides lives in squared pixel space computed from `NPC.Center` and the synced target, never in a screen-space or camera-space value"
  - "A stealth presentation shared by two creature classes is duplicated as two small private statics (`ComputeStealthAlpha` / `StepAlpha`) rather than promoted into a shared helper this plan does not own"
  - "Conservative defaults the design leaves unnumbered are named constants with a D-54 pointer in the doc comment (FireInterval 60, dash speeds/durations, ObservationRangeTiles 24/32, LowLifeFraction 0.4, FleeInkInterval 24)"

requirements-completed: []
requirements-advanced: [BIO-01]

coverage:
  - id: D1
    description: "放射虫 (`Radiolarian`) + `Radiolarian_WaterBolt`: shallow-water-only spawn at the phase's lowest aquatic weight, sustained water-bolt fire while submerged, a squared 4-tile dash trigger on the design's 300-frame cooldown, the design's stats (生命 180, 伤害 45 melee / 35 ranged, 防御 15, 击退抗性 80, 减伤 10, 免疫 困惑, 钱币 1000) and the single `RadialCarapace` drop at 6.7%"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors, Everglow.tmod packaged)"
        status: pass
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 (exit 0; 'OK: guarded classes = 34')"
        status: pass
    human_judgment: true
    rationale: "Spawn-band isolation, the fire cadence's feel and the dash-through behaviour are runtime properties; the plan's own human check is the D-21 client batch recorded in plan 04-09 (04-DEVIATIONS.md section 11)."
  - id: D2
    description: "覆藻章鱼 (`AlgaeOctopus`) + `AlgaeOctopus_InkCloud`: layer-water spawn with the 森雨幽谷 gap recorded, a distance-driven `NPC.alpha` stealth, `BuffID.Slow` on contact, a fast dash leaving Darkness-causing ink puffs, a death ink burst, the design's stats (生命 70, 伤害 15 melee, 防御 12, 击退抗性 10, 免疫 困惑, 钱币 500) and an empty commented loot table"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 (exit 0; 'OK: guarded classes = 36') plus the Task 2 acceptance-token audit over AlgaeOctopus.cs / AlgaeOctopus_InkCloud.cs"
        status: pass
    human_judgment: true
    rationale: "Whether the alpha fade reads as 无法在远处被观察, whether the dash trail lands where the design intends and whether Darkness lasts 2 s in play are runtime properties; recorded for the D-21 bundle in plan 04-09."
  - id: D3
    description: "大型覆藻章鱼 (`LargeAlgaeOctopus`): bottom-only spawn via `IsWaterBottom` at the lowest aquatic weight, the design's stats (生命 360, 伤害 50 melee / 60 ink, 防御 12, 击退抗性 20, 减伤 15, 免疫 中毒/困惑, 钱币 4500, 类型 稀有), a 60-frame grab applying 束缚/窒息 for 120 ticks, a 180-frame four-cloud ink wave, a fast dash, a low-life flee that inks behind it, and an empty commented loot table"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 (exit 0; 'OK: guarded classes = 37' - the phase's frozen 37) plus the Task 3 acceptance-token audit over LargeAlgaeOctopus.cs"
        status: pass
    human_judgment: true
    rationale: "The grab's movement-agency effect, the wave geometry and the flee threshold can only be judged in a client session; the release case (the debuff expiring after the creature dies) is the check the D-21 bundle in plan 04-09 records (T-04-27)."
  - id: D4
    description: "The phase's offline evidence unchanged: `03-BIOLOGY.json` / `03-BIOLOGY.md` byte-identical, no `.png`/`.obj`/`.xnb` or `.hjson` touched, and the phase gate still green at the frozen 37 guarded classes"
    verification:
      - kind: other
        ref: "git diff --stat 840bb4876..HEAD -- .planning/phases/03-completed-art-ordinary-monsters/ (empty); git diff --name-only 840bb4876..HEAD (exactly the five declared files); the gate's byte-level UTF-8 BOM guard over the phase change set"
        status: pass
    human_judgment: false

duration: 9min
completed: 2026-09-16
status: complete
---

# Phase 04 Plan 04: Death Jade Lake Stealth, Ranged & Bottom Predators Summary

**放射虫's sustained water-bolt fire with a 4-tile dash counter, 覆藻章鱼's distance-driven invisibility with an ink trail, and 大型覆藻章鱼's water-bottom grab-and-wave — three predators plus the two projectiles they share, all gated on the Yggdrasil subworld and the Kelp Curtain layer**

## Performance

- **Duration:** ~9 min (the first task's build to the plan-metadata commit)
- **Started:** 2026-09-16T10:05:33Z
- **Completed:** 2026-09-16T10:14:33Z
- **Tasks:** 3
- **Files modified:** 5 (5 created, 0 modified)

## Accomplishments

- **放射虫 (`Radiolarian` + `Radiolarian_WaterBolt`)** — the phase's first real *distance* logic. `SpawnChance` returns `0f` without `SubworldSystem.IsActive<YggdrasilWorld>()`, `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`, `spawnInfo.Water` **and** a bounded upward liquid-column probe (the 浅水区 band), and otherwise returns `RareWaterBottomWeight` (0.35f, the phase's lowest aquatic weight). While submerged it plants itself and fires `Radiolarian_WaterBolt` on a fixed 60-frame cadence carrying the design's 35 ranged damage; when its target reaches four tiles it commits a `DashSpeed` 10 charge for 24 frames on the design's own **300-frame** cooldown and then retreats for 45 frames before resuming fire. The trigger is a **compile-time squared pixel constant** (`DashTriggerTiles * DashTriggerTiles * 16f * 16f`), so no square root is taken and no client value is read. Design stats: 生命 180, 伤害 45 melee / 35 ranged, 防御 15, 击退抗性 80 (`0.2f`), 减伤 10 (`FinalDamage *= 0.9f`), 免疫 困惑, 钱币 1000, `NPC.rarity = White`.
- **覆藻章鱼 (`AlgaeOctopus` + `AlgaeOctopus_InkCloud`)** — the stealth ambusher. `NPC.alpha` is a **linear function of the distance between the synced `NPC.Center` and the synced target centre**, running from 255 (invisible) at and beyond `ObservationRangeTiles = 24` down to 0 up close, stepped gradually per tick. `OnHitPlayer` applies `BuffID.Slow` **unguarded** (the hook is client-only, so a netmode guard would make the design's debuff dead code in multiplayer — the 剧毒蟾蜍 / 荆棘苔龟 precedent). The fast dash leaves `AlgaeOctopus_InkCloud` puffs every 10 frames and death bursts one more, all spawned from `NPC.GetSource_FromAI()` under `Main.netMode != NetmodeID.MultiplayerClient`. Design stats: 生命 70, 伤害 15 melee / 25 ink, 防御 12, 击退抗性 10 (`0.9f`), empty 减伤 cell honoured by an explicit no-op `ModifyIncomingHit`, 免疫 困惑, 钱币 500.
- **大型覆藻章鱼 (`LargeAlgaeOctopus`)** — the lake-floor mini-ambusher. It spawns **only** when `KelpCurtainSpawnConditions.IsWaterBottom(spawnInfo)` also holds (the 碧灵鮟鱇 precedent) and holds itself just above the first solid tile under its centre while hidden. Its grab applies `BuffID.Webbed` **and** `BuffID.Suffocation` for 120 ticks, re-applied every 60 frames, **timed only** so a player can never be left permanently held (T-04-27). Every 180 frames it emits the design's four-cloud wave — the first aimed at the prey, three random — as four instances of the one shared ink cloud, each carrying 60 damage and 2 s of `BuffID.Darkness`. Under `LowLifeFraction` 0.4 against a player it flees while spraying ink behind it. Design stats: 生命 360, 伤害 50 melee / 60 ink, 防御 12, 击退抗性 20 (`0.8f`), 减伤 15 (`FinalDamage *= 0.85f`), 免疫 中毒/困惑, 钱币 4500, 类型 稀有 → `ItemRarityID.LightPurple`.
- **`AlgaeOctopus_InkCloud` is one class, not two.** Its damage rides the synced `Projectile.ai[0]` (`AssignedDamage`, defaulting to 25) and `Projectile.damage` is re-asserted from it every tick because damage is not part of the projectile net message — so 覆藻章鱼's 25（墨水）and 大型覆藻章鱼's 60（墨水）share one projectile, and 大型覆藻章鱼's wave adds no twelfth projectile to the phase's frozen eleven.
- **The phase gate closed at its frozen total.** `check-biology.ps1` rose **32 → 34 → 36 → 37** across the three tasks — exactly the five files this plan adds — and ends at the phase's own frozen `26 region classes + 11 projectiles = 37`. Every file carries the `White_Mod` override, both spawn tokens, `Main.dedServ` beside its dust and `NPC.netUpdate` on authoritative transitions; none references `Main.LocalPlayer`, `Main.screenPosition` or `KelpCurtainBiome.IsBiomeActive`, and only `Radiolarian` references an item type (`RadialCarapace`).

## Task Commits

Each task was committed atomically:

1. **Task 1: 放射虫 `Radiolarian` + `Radiolarian_WaterBolt`** — `69a5872ea` (feat)
2. **Task 2: 覆藻章鱼 `AlgaeOctopus` + `AlgaeOctopus_InkCloud`** — `50d721376` (feat)
3. **Task 3: 大型覆藻章鱼 `LargeAlgaeOctopus`** — `649eae128` (feat)

**Plan metadata:** this SUMMARY, `STATE.md` and `ROADMAP.md` are carried by the plan-metadata commit that follows it (the fourth commit in the `plan_head_before..HEAD` range).

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.cs` — 放射虫: four-state ranged/dash machine, squared 4-tile trigger, 300-frame cooldown, `RadialCarapace` at 6.7%
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/Radiolarian_WaterBolt.cs` — the water bolt: no tile collision, `ignoreWater`, water drag, no debuff, bounded lifetime
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/AlgaeOctopus.cs` — 覆藻章鱼: `NPC.alpha` stealth, Slow on contact, dash with an ink trail, death ink burst, empty commented loot table
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AlgaeOctopus_InkCloud.cs` — the shared ink cloud: `ai[0]`-parameterised damage, 120 ticks of `BuffID.Darkness`, lingering penetration
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeAlgaeOctopus.cs` — 大型覆藻章鱼: bottom-only spawn, 60-frame grab, 180-frame four-cloud wave, fast dash, low-life flee

## Decisions Made

- **No cross-plan creature type reference.** The plan's per-task behaviour asks 放射虫 to prefer a nearby 幽光蝾螈 and both octopuses to exclude 碧灵鮟鱇, but the execution constraint for this plan forbids `ModContent.NPCType<...>()` references to classes created by other plans. All three creatures therefore take their target from the engine (`NPC.TargetClosest(false)`) and the design's inter-creature hostility stays the system `04-DEVIATIONS.md` sections 7 and 13 already record as unmodelled — the phase's existing disposition, not a new gap. Recorded in `WINDOWS.md` entry 60 and in the class comments.
- **The grab goes through the authoritative `AddBuff` path, not `OnHitPlayer`.** The design repeats 束缚与窒息 every 60 frames rather than only on contact damage, so the client-only `OnHitPlayer` hook cannot express it; the class applies both timed debuffs on the authoritative side, which is the same sync path the 04-02 幽光蝾螈 suffocation implementation uses (a server-side `AddBuff` carrying its own net sync). The choice is written into `TryGrab`'s doc comment as T-04-27 requires.
- **The dash disengage reuses one timer.** `Radiolarian` names `FireTimer` (`localAI[0]`) and `DashCooldown` (`localAI[1]`) exactly as the plan requires, and folds the charge and the retreat into a single `ManeuverTimer` (`localAI[2]`) rather than adding a fourth slot.
- **The ink cloud's damage is re-asserted from `ai[0]` every tick.** `Projectile.damage` is not part of the projectile net message and `ai[]` is synced, so the shared class reads its owner's value back on every side instead of trusting the field.
- **The stealth helpers are duplicated, not promoted.** `ComputeStealthAlpha` / `StepAlpha` are small private statics in both octopus classes; publishing them would put a new shared helper in a file this plan does not own (`files_modified` is exactly the five created files).
- **The two `NPC.GetSource_FromAI` acceptance tokens are realised at the real spawn sites.** A `ModProjectile` exposes no `NPC` member, so the token is named in each projectile's XML doc that describes its spawn site, exactly as `ToxicToad_PoisonBubble` did in 04-02 and as the 04-06/04-07/04-08 siblings recorded (WINDOWS entry 62).
- **REQUIREMENTS.md is not touched.** BIO-01 is advanced but completed by none of this plan (3 of the 21 in-scope rows), so the traceability row stays Pending until plan 04-09 closes the phase — the 04-01/04-02/04-03/04-05/04-06/04-07/04-08 precedent.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Missing `using Everglow.Yggdrasil.Common;` and a non-existent `Projectile.noGravity`**
- **Found during:** Task 1 (the first Release build of the two new files)
- **Issue:** `Radiolarian.cs` called `NPCSpawnManager.RegisterNPC(Type)` without `using Everglow.Yggdrasil.Common;` (error CS0103), and `Radiolarian_WaterBolt.cs` set `Projectile.noGravity`, which does not exist on `Projectile` in this tML build (error CS1061). The second error is instructive rather than cosmetic: an aquatic projectile simply never adds gravity when `aiStyle = -1`, so there is no field to set.
- **Fix:** Added the import; removed the `noGravity` line and documented why the bolt needs no gravity suppression.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/Radiolarian_WaterBolt.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 with 0 warnings and 0 errors; the Task 1 gate run prints `OK: guarded classes = 34`.
- **Committed in:** `69a5872ea` (Task 1)
- **Ledger:** `WINDOWS.md` entry 63

**2. [Rule 1 - Bug] The 放射虫 ranged state entered itself instead of the firing state**
- **Found during:** Task 1 (the re-read of `Radiolarian.cs` before staging, in the same task)
- **Issue:** `UpdateRanging` called `EnterState(RadiolarianState.Ranging)` after the firing-band test rather than `EnterState(RadiolarianState.Firing)`, so the creature would never have opened fire. Caught before the first commit and fixed in the committed file.
- **Fix:** Corrected the transition target; the state machine now runs Ranging → Firing → (Dashing → Disengaging) → Firing.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.cs`
- **Verification:** the Release build stays green and the committed file's Task 1 acceptance-token audit passes.
- **Committed in:** `69a5872ea` (Task 1)

### Instruction-driven deviations

**3. The design's inter-creature hostility is not modelled in these three classes**
- **Found during:** Task 1 (the 放射虫 target rule) and Tasks 2/3 (the 鮟鱇 exclusion)
- **Issue:** `04-04-PLAN.md`'s behaviour blocks ask 放射虫 to actively attack a nearby 幽光蝾螈 and both octopuses to attack other aquatic creatures except 碧灵鮟鱇. Both would require `ModContent.NPCType<GlowSalamander>()` / `<JadeSpiritAnglerfish>()` — references to classes created by plans 04-02 and 04-01, which the execution constraint for this plan forbids.
- **Fix:** Only the player target is taken from the engine; the prey preference and the 鮟鱇 exclusion are left as the unmodelled cross-creature hostility that `04-DEVIATIONS.md` sections 7 and 13 already record for every predator row with an "attacks other aquatic creatures" clause. The reasoning is written into `AlgaeOctopus.ModifyNPCLoot`'s doc comment and recorded in `WINDOWS.md` entry 60.
- **Files modified:** none beyond the three class comments (`Radiolarian.cs`, `AlgaeOctopus.cs`, `LargeAlgaeOctopus.cs`)
- **Verification:** the Release build is green and the gate is green, so nothing was left half-wired.
- **Committed in:** `69a5872ea` / `50d721376` / `649eae128`

### Recorded-token deviations

**4. `NPC.GetSource_FromAI` cannot be a call inside a `ModProjectile`**
- **Found during:** Tasks 1 and 2
- **Issue:** Both tasks' acceptance criteria list `NPC.GetSource_FromAI` as a token inside `Radiolarian_WaterBolt.cs` / `AlgaeOctopus_InkCloud.cs`, but a `ModProjectile` exposes no `NPC` member.
- **Fix:** The real call lives at the spawn sites (`Radiolarian.TryFireWaterBolt`, `AlgaeOctopus.SpawnInkPuff`, `LargeAlgaeOctopus.SpawnInkCloud`), where it is wrapped in `Main.netMode != NetmodeID.MultiplayerClient`, and each projectile's XML doc names that spawn site — the plan 04-02/04-06/04-07/04-08 precedent. `WINDOWS.md` entry 62.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/Radiolarian_WaterBolt.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AlgaeOctopus_InkCloud.cs`, and the three NPC classes that own the spawn sites
- **Verification:** the gate's `White_Mod` + `Main.dedServ` assertions over both guarded projectile files pass, and the Release build is green.
- **Committed in:** `69a5872ea` (water bolt), `50d721376` (ink cloud)

**5. `NPC.rare` does not exist — corrected to `NPC.rarity` (9th occurrence)**
- **Found during:** Tasks 1–3 (the `SetDefaults` of all three classes)
- **Issue:** The plan's Design Values prose and per-task action text repeat `NPC.rare = ItemRarityID.White;`, which is a compile break in this tML build.
- **Fix:** All three classes write `NPC.rarity` (the engine's only NPC rarity field), with 大型覆藻章鱼's 类型 稀有 mapped to `ItemRarityID.LightPurple` per the 04-01 §10 register. No new ledger edit was needed — the correction is already in `04-DEVIATIONS.md` §10 (recorded as `WINDOWS.md` entry 65).
- **Files modified:** `Radiolarian.cs`, `AlgaeOctopus.cs`, `LargeAlgaeOctopus.cs`
- **Verification:** the Release build is green.
- **Committed in:** `69a5872ea` / `50d721376` / `649eae128`

**6. `tdd="true"` realised as the gate's monotone counter plus the Release build**
- **Found during:** all three tasks
- **Issue:** The repository has no unit-test infrastructure for `ModNPC` spawn isolation or AI feel, so no meaningful RED/GREEN pair can be authored for these tasks.
- **Fix:** The TDD attribute is realised as the Phase 4 gate's monotone guarded-class counter (`32 → 34 → 36 → 37`) plus the Release build, with each task committed only when its full automated verify is green. No test was fabricated and no RED/GREEN pair is claimed (the 04-02/04-03/04-06/04-07/04-08 precedent, `WINDOWS.md` entry 64).
- **Files modified:** none
- **Verification:** the four gate runs quoted below and the four Release builds, all exit 0.
- **Committed in:** all three task commits

---

**Total deviations:** 2 auto-fixed (Rule 1) + 4 documented (3 recording-only, 1 process/realisation)
**Impact on plan:** No scope creep. The two Rule 1 fixes were required for the build to pass and for the creature to actually fire; the four recorded items are the plan's own constraints colliding with the execution constraints, resolved in the direction the phase ledger already documents rather than by inventing behaviour.

## Issues Encountered

- **The plan's Task 1 acceptance criterion contains a name that does not compile as written.** Beyond the two Rule 1 fixes above, the task text asks for a "`DashTriggerTiles`" constant and a squared comparison; the implementation keeps `DashTriggerTiles = 4f` plus a compile-time `DashTriggerRangeSquared`, so both the design's literal `4` and the no-square-root requirement are visible in the file.
- **Console encoding noise (unchanged from 04-01).** `dotnet build` and `git` render CJK as mojibake in the PowerShell 5.1 console session. It is a display artefact only: the byte-level checks confirm all five new files are UTF-8 **without** BOM and LF-only (0 CRLF bytes), and the gate's own UTF-8 BOM guard passes.
- **The gate's guarded-class count is the only monotone progress signal available.** With no unit tests in scope, `OK: guarded classes =` is what proves a task's files actually landed (32 → 34 → 36 → 37), which is why each task's verify re-runs the gate rather than only building.

## Known Stubs

None — no hardcoded empty value, placeholder string or unwired data source was introduced. Concretely:

- The art-missing classes use the shared `Commons.ModAsset.White_Mod` fallback by design (D-48) and carry a documented blocker per real repository fact, not a stub.
- `AlgaeOctopus.ModifyIncomingHit` is an explicit no-op because the design row's 减伤 cell is empty (the 04-01 §10 register), and `LargeAlgaeOctopus.ModifyIncomingHit` scales by 0.85f because its cell reads 15.
- `AlgaeOctopus.ModifyNPCLoot` and `LargeAlgaeOctopus.ModifyNPCLoot` are empty by design: neither 软体甲壳碎片 nor 亡碧膏 (nor 大型覆藻章鱼's TBD 武器与饰品) has a `ModItem` in the repository, so no type is referenced (D-58) and the blockers live in `04-DEVIATIONS.md` §6.2 and the biology matrix.
- `Radiolarian`'s loot table wires the one implemented item, `RadialCarapace`, at the design's 6.7% (denominator 15).

## Defect Ledger

Five entries were appended to `.planning/WINDOWS.md` for cross-phase visibility (entries 60–65; they block `/gsd-ship` while open, by design):

- **#60 `deviation`** — the cross-plan `NPCType` constraint vs the plan's inter-creature target rules (Deviation 3 above).
- **#61 `unrun-verify`** — the Task 1–3 runtime human checks (spawn bands, the stealth fade, the grab, the ink wave, the dedicated-server run) are not executed; plan 04-09 records them in `04-UAT.md`.
- **#62 `deviation`** — the `NPC.GetSource_FromAI` token realised at the real spawn sites rather than inside the two projectiles.
- **#63 `deviation`** — the Task 1 first-build Rule 1 corrections (missing import, non-existent `Projectile.noGravity`).
- **#64 `deviation`** — the `tdd="true"` realisation (gate counter + Release build).
- **#65 `deviation`** — the 9th `NPC.rare` → `NPC.rarity` correction.

## Verification

- `dotnet build /p:Configuration=Release /p:WarningLevel=0` — **exit 0, 0 warnings, 0 errors**, `Everglow.tmod` packaged, after each of the three tasks; `dotnet build-server shutdown` run after the final batch.
- `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1` — **exit 0** after every task, printing `OK(0): phase4 in-scope set = 21 rows (rows=31)`, `OK: reconciled rows = 1 / 21` and `OK: guarded classes =` **34 / 36 / 37** (a rise of exactly five, the phase's frozen total).
- **Both spawn tokens** (`SubworldSystem.IsActive<YggdrasilWorld>`, `KelpCurtainBiome.IsKelpCurtainLayer`) present in all three creature classes; **no** `Main.LocalPlayer`, `Main.screenPosition` or `KelpCurtainBiome.IsBiomeActive` anywhere in the five files.
- **Only `Radiolarian` references an item type**, and only `RadialCarapace`; the two octopus loot tables reference none.
- **`Radiolarian_WaterBolt` and `AlgaeOctopus_InkCloud`** exist under the enemy-projectile tree with `White_Mod`; the ink cloud carries its damage through the named `ai[0]` value and `BuffID.Darkness`, the water bolt applies no unmodelled debuff.
- **`03-BIOLOGY.json` and `03-BIOLOGY.md` byte-identical** to their plan-04-01 state (`git diff --stat 840bb4876..HEAD` over that directory is empty); the whole plan's change set is exactly the five declared files, with **no** `.png`/`.obj`/`.xnb` and **no** `.hjson`.
- **`04-DEVIATIONS.md` was not modified** by this plan (read-only per its prohibition); the register items this plan opens live in the class comments, this SUMMARY and `WINDOWS.md` for plan 04-09 to consolidate.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- **All three Death Jade Lake predator rows now exist as classes**, so plan 04-09's reconciliation of the remaining in-scope rows can name `Radiolarian`, `AlgaeOctopus` and `LargeAlgaeOctopus` with real files on disk. The gate's `OK: guarded classes = 37` is the phase's frozen total, so every later plan's run is now a no-op counter.
- **Outstanding, all recorded, none hidden:**
  - the D-21 runtime bundle (`04-UAT.md`, plan 04-09): the 浅水区 / layer-water / water-bottom spawn bands, the three creatures' AI feel, the grab's release case, the ink wave, and the dedicated-server run;
  - **approved art for the three creature sprites and the two projectile sprites** — blocker only, no placeholder art created (D-48/D-51);
  - 软体甲壳碎片 and 亡碧膏 as absent drop materials (`04-DEVIATIONS.md` §6.2), and 大型覆藻章鱼's TBD 武器与饰品 list;
  - the 亡碧湖 / 森雨幽谷 region-level spawn predicates (Phases 5–6, D-52) — each named in the affected class's `SpawnChance` doc comment;
  - the ink cloud's lingering *rendering* as the §7 effect blocker (its damage and Darkness are implemented), and the cross-creature hostility left unmodelled per §7/§13;
  - localization (D-20) and runtime verification (D-21).
- **Plan 04-09 owns §14** of the ledger and the flip of the remaining twenty in-scope rows to `code_complete: true`. `REQUIREMENTS.md` stays Pending until then; `progress.completed_phases` stays at 3.

---

*Phase: 04-remaining-ordinary-monsters*
*Completed: 2026-09-16*

## Self-Check: PASSED

- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/AlgaeOctopus.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeAlgaeOctopus.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/Radiolarian_WaterBolt.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AlgaeOctopus_InkCloud.cs`
- FOUND commit: `69a5872ea` (Task 1)
- FOUND commit: `50d721376` (Task 2)
- FOUND commit: `649eae128` (Task 3)
- FOUND commit: `840bb4876` (`plan_head_before`)
