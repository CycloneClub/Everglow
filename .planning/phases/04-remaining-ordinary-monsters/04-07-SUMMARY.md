---
phase: 04-remaining-ordinary-monsters
plan: 07
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, yggdrasil, kelp-curtain, valley-of-lush-and-moist, template-reuse, segmented-worm, hostile-projectile, ambusher, bind-debuff, missing-art, spawn-predicate, d-46-blocker, oq1]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    plan: 01
    provides: the green 13-invariant Phase 4 gate (`scripts/check-biology.ps1` with its `OK: guarded classes =` progress counter), `KelpCurtainSpawnConditions.IsDryLand` and the D-54 land weight band, the `NPCs/<Region>/` folder-and-namespace rule that makes the D-49 art migration mechanical, the art-missing `White_Mod` class shape, and the OQ1 `CaterpillarJuice` drop decision (`04-DEVIATIONS.md` section 6.1)
  - phase: 03-completed-art-ordinary-monsters
    provides: `KelpCurtainBiome.IsKelpCurtainLayer(Player)` (the server-safe layer predicate), the `GuppyConch` state-dependent-defence and `VerdantRods` empty-loot-table / unguarded-`OnHitPlayer` precedents, `MossyThornTurtle`'s `PostAI` re-assertion discipline and the `GiantDandelion_*` hostile-projectile shape
  - repository (pre-existing, read-only)
    provides: `Everglow.Function/Templates/Enemies/Caterpillar.cs` (the segmented-worm coroutine AI), `YggdrasilTown/NPCs/BarkSpicyCaterpillar.cs` (the concrete precedent) and `YggdrasilTown/Items/Materials/CaterpillarJuice.cs` (the drop item)
provides:
  - the ValleyOfLushAndMoist region folder (the third and last of the gate's three region roots to be created)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs (红针洋辣子; the phase's one genuine cross-namespace template reuse)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/AssassinRaspberry.cs (阿萨辛覆盘子; the 4-8 tile buried ambusher)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs (蛇行苔; the 2-tile binder)
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs (the needle volley; frozen OQ4 item 4)
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs (the ground spike; frozen OQ4 item 5)
affects: [04-08, 04-09, phase-5-6-region-terrain, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (60000 tokens, 3 tasks) to calibrate future estimates.
# chars/4 over the five new source files (48457 bytes), never a harness token count.
actuals:
  tokens: 12114
  tasks: 3
  commits: 6   # measured: git rev-list --count 16c1ef950..HEAD after the plan close-out (3 task commits + 1 guard-form refactor + 1 plan-metadata commit + 1 housekeeping commit)
plan_head_before: 16c1ef950cdf00ddb03b6b81abae15c0f8732641

tech-stack:
  added: []
  patterns:
    - "Template reuse done read-only: a new ModNPC mirrors a concrete precedent on an Everglow.Commons.Templates class and adapts to the template instead of patching it"
    - "A template's unguarded dust path is disabled (DustType = -1) and replaced by the subclass's own Main.dedServ-guarded HitEffect, so no new unguarded graphics path is opened"
    - "Distance-windowed ambushers: one private state enum over NPC.ai[0] with named localAI wrappers, every transition in a single EnterState helper that owns the state-dependent defence"
    - "A stationary hazard keeps a public Vector2 AnchorPosition captured when it comes to rest, so a retract/release path returns it to its own spawn point without a per-frame position write"
    - "Visual-only design clauses of an unimplemented system (Valley 伪装) are D-46 blockers in the class doc while the trigger/attack mechanics behind them are fully built"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/AssassinRaspberry.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs
  modified:
    - .planning/WINDOWS.md
    - .planning/STATE.md
    - .planning/ROADMAP.md

key-decisions:
  - "红针洋辣子 extends Everglow.Commons.Templates.Enemies.Caterpillar and mirrors BarkSpicyCaterpillar (the -0.12f/-0.08f/-0.04f knockback progression, [NoGameModeScale], the segmented-worm geometry), and neither the template, the precedent nor CaterpillarJuice was edited: the class adapts to the template, never the reverse (D-57, T-04-43)"
  - "Its drop is the pre-existing implemented CaterpillarJuice with exactly BarkSpicyCaterpillar's rule (ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 1, 2)), i.e. the OQ1 WIRE decision recorded in 04-DEVIATIONS.md section 6.1 - the reason the gate's item index spans the whole Sources/Modules/Yggdrasil tree"
  - "DustType = -1 rather than a dust type: the Caterpillar template's own HitEffect emits DustType with no dedicated-server guard, so this class disables that path and emits the same dust from its own guarded HitEffect (D-35/T-04-46)"
  - "No PreKill override and no gore: BarkSpicyCaterpillar's PreKill loads BarkSpicyCaterpillar_gore* assets that this class does not own, and creating art is forbidden (D-48/D-51, T-04-41)"
  - "阿萨辛覆盘子的 defence switch (防御 20 (被动) / 4 (攻击)) lives in one EnterState helper as a single ternary, so both values are reachable only there and no exit path can leave the attacking value stuck when it goes back to 缩回地下 (T-04-44)"
  - "Its AnchorPosition is a public Vector2 field captured once the creature is settled (the VerdantRods public-field precedent), used by the retract path to return it to its own spawn point"
  - "蛇行苔 applies only the section 10 mapping its row needs - 束缚 -> BuffID.Webbed - with the register's 窒息 -> BuffID.Suffocation mapping named in the class comment; the bind cadence lives in NPC.localAI[0] and the bound player index in NPC.localAI[1], never on a Player (Pitfall 6)"
  - "Both 伪装 presentations (阿萨辛覆盘子 and 蛇行苔) are D-46 blockers naming the Valley disguised-hazard visual system in the class docs, and no disguise trick is built: an alpha trick would be indistinguishable from a rendering bug"
  - "The two 4~6-spike attacks are separate classes (RedNeedleCaterpillar_Spike for the needle volley, AssassinRaspberry_Spike for the ground spike), matching the frozen eleven-projectile OQ4 list rather than sharing one projectile"
  - "The two stateful guards use the plan's pinned Main.netMode != NetmodeID.MultiplayerClient form rather than the equivalent early return; a follow-up refactor commit made both committed classes match the pinned spelling with no behavioural change"
  - "NPC.rare in the plan prose was corrected to NPC.rarity for the sixth time in this project (Phase 3, 04-01, 04-02, 04-05, 04-06, 04-07); the correction is already in 04-DEVIATIONS.md section 10, so no ledger edit was needed"

patterns-established:
  - "A subclass of a shared template that fixes the template's graphics gaps from outside: keep the template untouched, set DustType = -1, own the guarded HitEffect"
  - "A distance-windowed ambusher's three separate ranges (extend, attack-minimum, retract) expressed as named constants over one tile-to-pixel helper, with the attack window as the intersection and no retract below the attack minimum"
  - "Anchoring a stationary hazard by capturing its rest position instead of adding a floor predicate the plan forbids approximating"

requirements-completed: []
requirements-advanced: [BIO-03]

coverage:
  - id: D1
    description: "红针洋辣子 (RedNeedleCaterpillar) end-to-end on the repository's segmented-worm template: the design's 60 life / 20 contact damage / 4 defence / 80 copper, the -0.12f/-0.08f/-0.04f knockback progression, the 4~6 needle volley every 180 frames at 4+ tiles, the 25%/37.5% 中毒 split on both contact and needles, and the CaterpillarJuice drop"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors, Everglow.tmod packaged)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariant 7/8 over the guarded classes (White_Mod, no Main.LocalPlayer, both spawn tokens, Main.dedServ beside the dust, item-type resolution) - guarded classes 18 -> 20"
        status: pass
      - kind: other
        ref: "git diff --stat over the template, BarkSpicyCaterpillar.cs and CaterpillarJuice.cs is empty (uncommitted-tree check run before the task commit)"
        status: pass
    human_judgment: true
    rationale: "The worm's crawling feel, the volley's readability at range, the drop rate on real kills and the spawn isolation are runtime properties; the plan's own human check is the D-21 client batch recorded in plan 04-09 (04-DEVIATIONS.md section 11)."
  - id: D2
    description: "阿萨辛覆盘子 (AssassinRaspberry) and its ground spike: buried beyond 8 tiles, extended inside 8, silent (but not retracted) below 4, 4~6 scattered spikes only inside the exact 4~8 window, defence 20 passive / 4 attacking restored on every exit path, knockback immunity, 中毒/困惑 immunity, and the D-46 disguise blocker"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariant 7/8 (White_Mod, no Main.LocalPlayer, no Main.screenPosition, both spawn tokens, Main.dedServ) - guarded classes 20 -> 22"
        status: pass
      - kind: other
        ref: "token audit of the class: enum AssassinRaspberryState, the named ExtendTiles/AttackMinTiles/MinSpikes/MaxSpikes constants, Main.netMode != NetmodeID.MultiplayerClient, NPC.netUpdate and one defence ternary in EnterState"
        status: pass
    human_judgment: true
    rationale: "The ambush window's feel (whether a player can cross it without being hit), the spike scatter's readability and the defence switch under real combat are runtime properties; the D-21 bundle carries them."
  - id: D3
    description: "蛇行苔 (SerpentMoss): the 2-tile bind trigger, 60 ticks of the mapped 束缚 plus 15 damage every 60 frames, the 33% 15-second 中毒 roll after each damage tick, release on leaving the trigger radius, knockback immunity, 中毒/困惑 immunity, and the D-46 disguise blocker plus the section 10 mapping statement"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariant 7/8 (White_Mod, no Main.LocalPlayer, both spawn tokens, Main.dedServ) - guarded classes 22 -> 23"
        status: pass
      - kind: other
        ref: "token audit of the class: enum SerpentMossState, BindTriggerTiles/BindIntervalFrames/BindDebuffTicks/BindDamage/PoisonTicks, BuffID.Webbed, the 1-in-3 roll, NPC.netUpdate and no Player-hosted timer"
        status: pass
    human_judgment: true
    rationale: "Whether a bound player is reliably held (and how the 60-frame cadence feels against the engine's own webbed duration) can only be judged in a client; the D-21 bundle records it."
  - id: D4
    description: "The ValleyOfLushAndMoist region root and the gate's +5 progress: the phase's third and last guarded region folder, advanced 18 -> 20 -> 22 -> 23 across the three tasks"
    requirement: BIO-03
    verification:
      - kind: other
        ref: "check-biology.ps1: 'OK: guarded classes = 23' after task 3 (18 before the plan; +5 exactly, the three NPCs plus the two projectiles)"
        status: pass
      - kind: other
        ref: "check-biology.ps1: 'OK(0): phase4 in-scope set = 21 rows (rows=31)', 'OK: reconciled rows = 1 / 21' (unchanged - only plan 04-09 flips rows), 'OK: UTF-8 BOM check passed'"
        status: pass
    human_judgment: false

duration: 11min
completed: 2026-09-16
status: complete
---

# Phase 4 Plan 07: Valley Hazards — 红针洋辣子, 阿萨辛覆盘子, 蛇行苔 Summary

**The Valley's three ordinary hazards built on the repository's own segmented-worm template and a pre-existing material: the needle caterpillar reuses `Caterpillar` + `CaterpillarJuice` read-only, the buried raspberry ambushes inside a 4–8 tile window, and the moss binds at 2 tiles — with both 伪装 presentations recorded as precise D-46 blockers instead of faked**

## Performance

- **Duration:** ~11 min (from the previous plan's metadata commit to this plan's metadata commit)
- **Started:** 2026-09-16T08:21:02Z
- **Completed:** 2026-09-16T08:32:20Z
- **Tasks:** 3
- **Files modified:** 8 (5 created in `Sources/`, 3 planning files updated)

## Accomplishments

- **红针洋辣子 (`RedNeedleCaterpillar`)** — the phase's one genuine cross-namespace reuse, done read-only. The class extends `Everglow.Commons.Templates.Enemies.Caterpillar` and mirrors `BarkSpicyCaterpillar` (`[NoGameModeScale]`, `SegmentBehavioralSize`/`SegmentHitBoxSize`/`SegmentCount`/`AnimationSpeed`, 生命 60 / 伤害 20 / 防御 4 / 钱币 80, the `-0.12f` / `-0.08f` / `-0.04f` knockback progression, 免疫 中毒), adds the design's volley — 4–6 `RedNeedleCaterpillar_Spike` needles from the head segment every 180 frames once the player is at least 4 tiles away — and the 25% 20-second / 37.5% 10-second 中毒 split on both the contact hit and the needle hit. Its loot table is exactly one rule, `ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 1, 2)`, i.e. the OQ1 WIRE decision of `04-DEVIATIONS.md` §6.1. The template, the precedent and the material are unmodified (`git diff --stat` empty, checked before the task commit), and **no `PreKill` override was copied** — that precedent's `PreKill` loads gore art this class does not own, and creating art is forbidden (T-04-41).
- **阿萨辛覆盘子 (`AssassinRaspberry`)** — a stationary hostile with a private `AssassinRaspberryState` enum over `NPC.ai[0]`: 缩回地下 beyond 8 tiles, 伸出 once the player is inside 8, **silent below 4 tiles without retracting**, and its 4–6 `AssassinRaspberry_Spike` ground-spike scatter only inside the design's exact 4–8 tile window. 生命 80 / 伤害 30 / 钱币 100 / 免疫击退 / 免疫 中毒 困惑, and the design's 防御 20（被动）/ 4（攻击） switch lives in **one** `EnterState` helper as a single ternary, so no exit path can leave the attacking value stuck. Its `AnchorPosition` (a public `Vector2` captured once the creature comes to rest) returns it to its own spawn point on every retract.
- **蛇行苔 (`SerpentMoss`)** — a stationary binder: a player inside 2 tiles is bound, and every 60 frames they take 60 ticks of the mapped 束缚 (`BuffID.Webbed`) plus 15 damage, with a 33% 15-second 中毒 roll after each damage tick and a release as soon as the bound player leaves the trigger radius. 生命 100 / 伤害 15 / 防御 10 / 钱币 200 / 免疫击退 / 免疫 中毒 困惑. The cadence lives in `NPC.localAI[0]` and the bound index in `NPC.localAI[1]` — **never on a `Player`** (Pitfall 6) — and the class states the `04-DEVIATIONS.md` §10 mapping it applies (束缚 → `BuffID.Webbed`, with 窒息 → `BuffID.Suffocation` in the same register) rather than depending on a same-wave sibling.
- **Two hostile projectiles, both art-missing** — `RedNeedleCaterpillar_Spike` (gravity-affected, tile-colliding needle with the same poison split) and `AssassinRaspberry_Spike` (a ground spike that leaves at the creature's feet and arcs back to the ground), both `Commons.ModAsset.White_Mod`, both spawned from `NPC.GetSource_FromAI()` at their owners' spawn sites under `Main.netMode != NetmodeID.MultiplayerClient`, and both emitting dust only behind `!Main.dedServ`.
- **The ValleyOfLushAndMoist region root** now exists (the phase's third and last guarded region folder), so the gate's `OK: guarded classes =` counter advanced **18 → 20 → 22 → 23** — exactly the five files this plan adds. `03-BIOLOGY.json`, `03-BIOLOGY.md` and `04-DEVIATIONS.md` are byte-identical to their end-of-04-01 state, and the plan range contains no `.png` / `.obj` / `.xnb` / `.hjson` change.
- **Both 伪装 (disguise) presentations are recorded, not faked.** The raspberry's "indistinguishable from scenery while buried" and the moss's "spawns disguised as an environmental plant" belong to the Valley disguised-hazard visual system that does not exist (`04-DEVIATIONS.md` §7 lists it as an unimplemented system). Each class carries the D-46 blocker by name **and** implements the trigger, the window, the bind, the damage and the spikes it hides behind, so no behaviour is missing and no fake stands in for the missing art.

## Task Commits

Each task was committed atomically:

1. **Task 1: 红针洋辣子 `RedNeedleCaterpillar` + `RedNeedleCaterpillar_Spike`** — `f9fdf0d4a` (feat)
2. **Task 2: 阿萨辛覆盘子 `AssassinRaspberry` + `AssassinRaspberry_Spike`** — `e54850445` (feat)
3. **Guard-form refactor** — `0012b19f3` (refactor; see Deviations 3)
4. **Task 3: 蛇行苔 `SerpentMoss`** — `c6b843713` (feat)

**Plan metadata:** this SUMMARY, `STATE.md`, `ROADMAP.md` and the four `WINDOWS.md` entries are carried by the plan-metadata commit that follows Task 3 (`54193c3df`), and the generated `.planning/state.json` (synced by the state verbs) together with the closing record of this commit count are carried by the final housekeeping commit — the sixth and last commit in the `plan_head_before..HEAD` range.

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs` — 红针洋辣子: the template reuse, the design's stats and knockback progression, the head volley, the poison split and the `CaterpillarJuice` drop
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/AssassinRaspberry.cs` — 阿萨辛覆盘子: the 8-tile extend / 4–8 tile attack window, the state-owned defence switch, the scattered spike volley and the D-46 disguise blocker
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs` — 蛇行苔: the 2-tile bind, the 60-frame 束缚 + damage cadence, the 33% poison roll, the D-46 disguise blocker and the §10 mapping statement
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs` — the needle (frozen OQ4 projectile 4)
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs` — the ground spike (frozen OQ4 projectile 5)
- `.planning/WINDOWS.md` — four `deviation` entries (see Defect Ledger)

## Decisions Made

- **`DustType = -1` plus an own guarded `HitEffect`.** The `Caterpillar` template's `HitEffect` reads `DustType` with no dedicated-server guard, so setting a dust type would have opened an unguarded graphics path from a class the plan forbids editing. Disabling it and emitting the same dust from the subclass behind `if (!Main.dedServ)` keeps every dust call this plan adds inside the guard (T-04-46) without touching the template.
- **`AnchorPosition` as a public field, not a `localAI[]` pair.** The plan allows either; the field mirrors `VerdantRods.TargetPos` (the repository's own stationary-creature precedent) and is captured only once the creature is settled (`NPC.velocity.Y == 0f && NPC.collideY`), so the anchor is the rest position rather than a mid-fall spawn point. No floor predicate was invented, because approximating one is exactly what D-52/D-53 and the prohibitions forbid.
- **The two 4~6-spike attacks are two projectile classes.** OQ4's frozen list names `RedNeedleCaterpillar_Spike` and `AssassinRaspberry_Spike` separately, so the needle (arcing, catching the player mid-flight) and the ground spike (leaving at the feet) keep their own cadence, shapes and dust rather than sharing one class.
- **蛇行苔 binds through the §10 mapping, not through a sibling.** The class comment states both register entries (束缚 → `BuffID.Webbed`, 窒息 → `BuffID.Suffocation`) and applies only 束缚, because this row's design carries no 窒息 clause; that is what makes the class buildable regardless of whether the same-wave plan 04-04 has landed.
- **All three classes write state through the `!=` guard form the plan names.** Discovered while auditing the acceptance tokens: the first pass used the equivalent `if (Main.netMode == NetmodeID.MultiplayerClient) { return; }` early return. The authoritative work paths were extracted into `UpdateVolley` / `UpdateVolley` / `UpdateBind` and the two already-committed classes were brought to the pinned spelling in a separate `refactor` commit (no behavioural change; see Deviations 3).

## Deviations from Plan

### Auto-fixed / recorded Issues

**1. [Rule 1 - Documentation] `NPC.GetSource_FromAI` cannot exist inside the two new `ModProjectile` files**
- **Found during:** Task 1 (the acceptance audit) and repeated in Task 2
- **Issue:** Both tasks' acceptance criteria require `NPC.GetSource_FromAI` inside the projectile file, but a `ModProjectile` exposes no `NPC` member, so the call is a compile error there.
- **Fix:** The token is realised at the real spawn sites — `RedNeedleCaterpillar.UpdateVolley` → `FireVolley` and `AssassinRaspberry.FireSpikeScatter`, both inside `Main.netMode != NetmodeID.MultiplayerClient` — and each projectile's XML doc names that spawn site. This is the plan 04-02 and 04-06 precedent (recorded rather than faked with a comment-only token; `WINDOWS.md` entries 39, 40, 41).
- **Files modified:** the two projectile classes (documentation only)
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0; the gate's invariant 8 resolves every item type and finds none in either projectile.
- **Committed in:** `f9fdf0d4a`, `e54850445`

**2. [Rule 1 - Literal token] `Main.rand.NextBool(3)` is expressed through the named constant the same task prescribes**
- **Found during:** Task 3 (the acceptance audit)
- **Issue:** The acceptance list requires the literal token `Main.rand.NextBool(3)`, while the same task's action requires a named `PoisonChanceDenominator = 3` and a roll over it. Writing both would duplicate the value and leave one dead.
- **Fix:** The roll is written as `Main.rand.NextBool(PoisonChanceDenominator)` (the constant is 3, so the behaviour is the same 1-in-3 roll) and the adjacent comment names the literal form. The missing literal is a literal-grep false negative, not an unimplemented behaviour.
- **Files modified:** `SerpentMoss.cs` (comment only)
- **Verification:** the file contains `PoisonChanceDenominator`, `PoisonTicks = 900` and the 1-in-3 roll; the Release build and the gate exit 0.
- **Committed in:** `c6b843713`

**3. [Rule 1 - Guard form] The two committed classes used the early-return spelling of the pinned netmode guard**
- **Found during:** Task 3, while checking the Task 2 acceptance token list against all five files
- **Issue:** Tasks 1 and 2 pin "spawns … under `Main.netMode != NetmodeID.MultiplayerClient`", but the first pass wrote the semantically identical `if (Main.netMode == NetmodeID.MultiplayerClient) { return; }` form in `RedNeedleCaterpillar.AI` and `AssassinRaspberry.AI`/`EnterState`.
- **Fix:** The authoritative work paths were extracted into `UpdateVolley` / `UpdateVolley` / `UpdateBind` and gated on the `!=` form; the two already-committed files were corrected in a dedicated `refactor(04-07)` commit rather than folded into Task 3's commit, so each task's commit still contains its own task.
- **Files modified:** `RedNeedleCaterpillar.cs`, `AssassinRaspberry.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 (0 warnings, 0 errors) and the gate stays green (`guarded classes = 23`) after the change; no state, cadence or `netUpdate` site changed.
- **Committed in:** `0012b19f3`

**4. [Rule 1 - API name] `NPC.rare` corrected to `NPC.rarity` for the sixth time in this project**
- **Found during:** Tasks 1–3 (`SetDefaults`)
- **Issue:** The plan's behavior text repeats `NPC.rare = ItemRarityID.White;`, which does not exist in this tML build.
- **Fix:** `NPC.rarity = ItemRarityID.White;` with the design's empty 稀有度 cell named in the comment. The correction is already recorded in `04-DEVIATIONS.md` §10 (Phase 3, 04-01, 04-02, 04-05, 04-06, 04-07), and this plan may not edit that ledger (`WINDOWS.md` entry 43).
- **Files modified:** the three NPC classes
- **Verification:** Release build 0/0.
- **Committed in:** `f9fdf0d4a`, `e54850445`, `c6b843713`

**Total deviations:** 4 recorded (all Rule 1 class: 1 API name, 1 literal-token/documentation, 1 guard form, 1 impossible-token documentation). No architectural change, no scope addition.
**Impact on plan:** None on behaviour or scope. Two of the four are documentation-level (`NPC.GetSource_FromAI` cannot exist in a projectile; `Main.rand.NextBool(3)` is spelled through its named constant) and two are corrections that keep the code faithful to the plan's own pinned forms and to this tML build's API.

## Issues Encountered

- **Console encoding noise (again).** `git commit` / `git log` render CJK as mojibake in the PowerShell 5.1 console session, but the bytes on disk and in the commit objects are correct UTF-8: a code-point dump of the first task's commit subject returns `7ea2 9488` (红 针) as expected. It is a display artefact only.
- **A second build per task.** The guard-form correction (Deviation 3) touched two already-verified classes, so the Release build and the Phase 4 gate were re-run after it; both stayed green before Task 3 was committed.

## Known Stubs

None — no hardcoded empty value, placeholder string or unwired data source was introduced. The three art-missing classes use the shared `Commons.ModAsset.White_Mod` fallback by design (D-48) and carry a documented blocker per real repository fact. The two empty `ModifyNPCLoot` bodies are the design's own "no drop" cells expressed the `VerdantRods` way, named in their comments, and contain **no** `ModContent.ItemType<...>` token (D-57/D-58). No disguise, alpha or scale trick stands in for the unimplemented Valley disguised-hazard presentation.

## Defect Ledger

Four entries were appended to `.planning/WINDOWS.md` for cross-phase visibility (they block `/gsd-ship` while open, by design):

- **#40 `deviation`** — `RedNeedleCaterpillar_Spike.cs`: the `NPC.GetSource_FromAI` acceptance token cannot live in a `ModProjectile`; realised at the spawn site.
- **#41 `deviation`** — `AssassinRaspberry_Spike.cs`: the same, for the ground spike's spawn site.
- **#42 `deviation`** — `SerpentMoss.cs`: the `Main.rand.NextBool(3)` acceptance token is spelled as `Main.rand.NextBool(PoisonChanceDenominator)`; the behaviour is the same 1-in-3 roll.
- **#43 `deviation`** — the sixth `NPC.rare` → `NPC.rarity` correction (Phase 3, 04-01, 04-02, 04-05, 04-06, 04-07), already in `04-DEVIATIONS.md` §10.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- **What the plan hands on:** three of the five Valley rows are now code-complete classes with their two frozen projectiles, and the `ValleyOfLushAndMoist` region root exists, so plan 04-08 (小格普螺 + 大型荆棘苔龟 + shockwave & boulder) lands into a folder the gate already guards. The gate's counter reads **23** of the 37 that close the phase.
- **Outstanding, all recorded, none hidden:**
  - the D-21 runtime bundle (`04-UAT.md`, plan 04-09): the caterpillar's volley cadence and needle poison split at real range, the raspberry's 4–8 tile window and its defence switch under combat, the moss's bind reliability (60 ticks of 束缚 every 60 frames against the engine's own webbed duration), all three spawn predicates inside Yggdrasil and their absence in an ordinary world, and the dedicated-server run;
  - **approved art for the three NPC sprites and the two projectile sprites** — blocker only, no placeholder art created (D-48/D-51); the D-49 migration is add `<Class>.png` beside the `.cs`, delete the `Texture` override, delete the blocker;
  - the Valley disguised-hazard visual system, which the two 伪装 blockers name (§7/§13);
  - the 森雨幽谷 / 刺苔庭园 region-level spawn predicates (Phases 5–6, D-52);
  - localization (D-20) and runtime verification (D-21) for the whole phase.
- **Plan 04-09** owns the reconciliation of the remaining in-scope rows and `REQUIREMENTS.md` stays untouched (BIO-03 advanced, not completed) until then.

---

*Phase: 04-remaining-ordinary-monsters*
*Completed: 2026-09-16*

## Self-Check: PASSED

- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/AssassinRaspberry.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs`
- FOUND commit: `f9fdf0d4a` (Task 1)
- FOUND commit: `e54850445` (Task 2)
- FOUND commit: `0012b19f3` (guard-form refactor)
- FOUND commit: `c6b843713` (Task 3)
- FOUND commit: `16c1ef950` (`plan_head_before`)
