---
phase: 04-remaining-ordinary-monsters
plan: 06
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, subworld, yggdrasil, kelp-curtain, spiny-moss-court, stat-variants, neutral-creature, summoner, flying-melee, missing-art, spawn-predicate, d-46-blocker]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    plan: 01
    provides: the green 13-invariant Phase 4 gate (`scripts/check-biology.ps1` with its `OK: guarded classes =` progress counter), `KelpCurtainSpawnConditions.IsDryLand` and the D-54 land weight band, the `NPCs/<Region>/` folder-and-namespace rule that makes the D-49 art migration mechanical, and the `JadeSpiritAnglerfish` art-missing class shape this plan mirrors
  - phase: 03-completed-art-ordinary-monsters
    provides: `MossyThornTurtle` (`ModifyNPCLoot` + `ItemDropRule.Common`, the pinned `defDamage`/`defDefense` clone pattern) and the `GuppyConch` / `VerdantRods` neutral-creature and empty-loot-table precedents
  - phase: 01-item-inventory-completed-art-items
    provides: the `ActivatedDogStaff` item type the hound's 9% drop wires (the only item type this plan references)
  - phase: 02-remaining-items-unfinished-art-materials
    provides: the hostile-projectile shape with the Commons.ModAsset.White_Mod fallback and the Main.dedServ dust guard
provides:
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs (枯木活化士兵 近战, the row's internal_name)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs (远程 sibling)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs (法术 sibling)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkHound.cs (犬 sibling, the only drop)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/CourtCommander.cs (王庭号令者)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs (布罗迪蝇蜓 标准, the row's internal_name)
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/SmallBrodieFlydragon.cs (小 sibling)
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs (the 远程 variant's attack)
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_SpellBeam.cs (the 法术 variant's attack)
  - the SpinyMossCourt region folder (the third and last Phase 4 region root the gate now counts)
affects: [04-09, phase-5-6-region-terrain, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (66000 tokens, 3 tasks) to calibrate future estimates.
# chars/4 over the realized diff (132470 bytes across the nine new source files), never a harness token count.
actuals:
  tokens: 33118
  tasks: 3
  commits: 4   # measured: git rev-list --count af8629559e..HEAD after the plan-metadata commit (3 task commits + 1 plan-metadata commit)
plan_head_before: af862955903e5f847d8f378a424b7470e686fdc2

tech-stack:
  added: []
  patterns:
    - "A four-way stat-variant family as four ModNPC classes sharing one `中立 until provoked` state machine and one morale-scalable member seam (OQ2)"
    - "A summoner whose summon is capped once per aggro entry by a named flag over NPC.localAI[2], created with NPC.NewNPC under Main.netMode != NetmodeID.MultiplayerClient (T-04-37)"
    - "A gravity-affected multi-ray spray: one cast constant, three evenly spread rays of a forward cone, all spawned from NPC.GetSource_FromAI inside the netmode guard"
    - "A clone-for-bases class: NPC.CloneDefaults(NPCID.DarkCaster) then NPC.aiStyle = -1, because the vanilla caster AI fires the engine's own projectile and cannot spray the design's three"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkHound.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/CourtCommander.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/SmallBrodieFlydragon.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_SpellBeam.cs
  modified:
    - .planning/WINDOWS.md
    - .planning/STATE.md
    - .planning/ROADMAP.md

key-decisions:
  - "枯木活化士兵 is four classes, not one with a stat switch: the design's four rows (生命 80/60/55/75, 伤害 22/35/24/20, 防御 4/0/0/2, 击退抗性 20/40/40/70 and the hound-only drop) cannot share one SetDefaults, and a runtime NPC.lifeMax mutation is exactly what OQ2/Pitfall 5 forbids (T-04-38)"
  - "All four soldier variants are functionally 中立: no player targeting until provoked by damage taken (HitEffect, netmode-guarded) or by a player inside the conservative ProvokeRange; the provoked state returns to 中立 past the leash range, both transitions written authoritatively with NPC.netUpdate"
  - "The melee variant owns a local fighter AI instead of cloning the vanilla skeleton AI: the vanilla fighter aiStyle always acquires a player target, so the design's explicit 中立 default would be inexpressible, and co-opting NPC.ai[0] while a cloned AI runs would break that AI's own state machine. The mirrored approach (walk, jump at a wall/ledge/above-prey, contact damage), the pinned defDamage/defDefense and the NPC.ai[0] state enum are all kept (recorded as a deviation)"
  - "The morale/command seam is a named public member seam (MoraleDefenseBonus / MoraleDamageScale / MoraleSpeedScale plus IsNeutral), read in PostAI and in the movement code, so a later phase raises defence/damage/speed without reworking any AI - the design's 意志高涨 buff itself is a D-46 blocker and applies nothing today"
  - "The one wired drop is the hound's ActivatedDogStaff at the exact 9% reciprocal (denominator 11); the other eight classes reference no item type at all, so 枯木碎块 and 干涸心脏 stay blockers with no type reference and the mod still builds (D-57/D-58, T-04-36)"
  - "CourtCommander's summon is limited to one per aggro entry by HasSummonedThisAggro (NPC.localAI[2]) and capped at the design's 1-3 members, created with NPC.NewNPC under Main.netMode != NetmodeID.MultiplayerClient with NPC.netUpdate (T-04-37); it fires on OnSpawn and on the first staff use of each aggro entry, and only while no soldier stands within SearchRadiusTiles"
  - "The spell variant clones NPCID.DarkCaster for its bases and then sets NPC.aiStyle = -1: the engine's caster AI fires its own projectile on its own cadence, so only the bases are inherited and the design's fixed-interval cast, three-beam spray and random teleport are owned locally (原版法师AI behaviour, locally implemented)"
  - "Both flydragon classes take their values from the design snapshot's own stats table V9zXdPuSdoQD7bxe6IVcdzqrncf (标准 40/25/5/50/中毒/20 copper, 小 20/15/2/50/中毒/0 copper), which differs from the plan's inline table on the two 钱币 cells; the plan's Task 3 action explicitly instructs reading the design table directly (D-25)"
  - "The 森雨幽谷 egg system is a D-46 blocker, not a half-build: no egg NPC, no egg-breaking spawn path, no egg class, and neither flydragon relies on one; the small variant is reachable through its own lower conservative natural weight, which also preserves 自然刷新只会刷新标准大小的蝇蜓"
  - "The 刺苔庭园 / 森雨幽谷 region split is a Phase 5-6 blocker: SpawnChance uses only the subworld token, KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player) and KelpCurtainSpawnConditions.IsDryLand, never IsBiomeActive and never a coordinate range (D-52/D-53, T-04-35)"
  - "Every conservative default this plan chose (hit boxes, provoke/aggro/leash ranges, speeds, spell cadence, teleport search, teleport clearance, commander staff-raise window, per-variant spawn weights) is recorded in the class comments and in this SUMMARY rather than in 04-DEVIATIONS.md section 10, because this plan's prohibitions forbid editing that ledger in wave 3 (the 04-05 precedent)"

patterns-established:
  - "Four sibling stat-variant classes sharing one behavioural vocabulary (State enum over NPC.ai[0], named NPC.localAI[] wrappers, Enter* transition helpers) but never one stat line"
  - "A clone-for-bases-only class: CloneDefaults for the collider/sound bases with an explicit NPC.aiStyle = -1 when the vanilla AI would fight the design"
  - "A summoner whose reinforcement call is idempotent per aggro entry through a single named flag slot instead of a scattered guard"
  - "A hostile multi-ray attack whose spread is derived from a single half-angle constant, so 'three beams' is a loop bound and not three copies of a spawn call"

requirements-completed: []
requirements-advanced: [BIO-02]

coverage:
  - id: D1
    description: "枯木活化士兵 as four loadable, layer-gated, art-missing ModNPC classes with the design's own four stat rows, the design's four attack patterns, the 中立 default and the morale-scalable member seam"
    requirement: BIO-02
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors, Everglow.tmod packaged)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 after Task 1: exit 0, OK: guarded classes = 15 (was 9, +6 = the four soldier classes + the two projectiles); token audit over the six files for White_Mod / LocalizationCategory / NPCSpawnManager.RegisterNPC / both spawn tokens / IsDryLand / catchItem = 0 / the netmode guard / netUpdate / Main.dedServ / 枯木碎块 / 干涸心脏 / D-46 / the three morale members, all present; the three non-hound classes reference no item type and the hound references only ActivatedDogStaff"
        status: pass
    human_judgment: true
    rationale: "Spawn isolation, the neutral-until-provoked feel, the 120-frame pickup then the 8-tile/200-frame spacing, the three-ray arc and the hound's back-and-forth charge are runtime properties; the plan's own human check is the D-21 bundle plan 04-09 records (04-DEVIATIONS.md section 11, WINDOWS.md entry 35)."
  - id: D2
    description: "王庭号令者 CourtCommander: the design's low spawn weight, the 漫无目的地游荡 / 高举权杖 / 保持10格 three-state machine, the once-per-aggro summon of 1-3 random soldier variants under the netmode guard, and the empty commented loot table"
    requirement: BIO-02
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 after Task 2: exit 0, OK: guarded classes = 16 (+1); token audit over CourtCommander.cs for enum CourtCommanderState, MinSoldiersNearby/SummonMin/SummonMax/RetreatTiles/SearchRadiusTiles/StaffRaiseFrames, NPC.NewNPC, ModContent.NPCType<, HasSummonedThisAggro, lifeMax 100 / damage 25 / defense 4 / value 250 / knockBackResist 0.5f, all present"
        status: pass
    human_judgment: true
    rationale: "The summon's once-per-aggro cap, the 10-tile spacing feel and the staff-raise tell need a live client; the D-21 bundle covers them (WINDOWS.md entry 35)."
  - id: D3
    description: "布罗迪蝇蜓 as two classes (标准 BrodieFlydragon and 小 SmallBrodieFlydragon) that hover, pursue on aggro and press in for contact damage, with the design's two stat rows, the egg-system D-46 blocker and the Phase 5-6 region blocker"
    requirement: BIO-02
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 after Task 3: exit 0, OK: guarded classes = 18 (= 9 + exactly 9 across the plan); token audit over both flydragon files for White_Mod / LocalizationCategory / NPCSpawnManager.RegisterNPC / both spawn tokens / catchItem = 0 / enum BrodieFlydragonState / the netmode guard / netUpdate / Main.dedServ / ModifyNPCLoot / the egg blocker / the Phase 5-6 region blocker, all present; lifeMax/damage 40/25 and 20/15 with value 20 and 0, and the standard class's weight (KelpCurtainSpawnConditions.LandWeight = 1f) above the small class's 0.5f"
        status: pass
    human_judgment: true
    rationale: "Natural-spawn-only reachability, the hover/pursue feel and the two sizes' relative rarity need a live client; the D-21 bundle covers them (WINDOWS.md entry 35). No egg class, egg projectile or buff class was created, which the acceptance criteria assert structurally."
  - id: D4
    description: "The nine-file structural contract: White_Mod on every file, the two spawn tokens on every class declaring SpawnChance, LocalizationCategory + NPCSpawnManager.RegisterNPC + Main.dedServ + NPC.netUpdate + ModifyNPCLoot on every creature class, no Main.LocalPlayer, no IsBiomeActive, and no new art or HJSON"
    requirement: BIO-02
    verification:
      - kind: other
        ref: "check-biology.ps1 invariants 7/8 over the guarded-class set (exit 0, guarded classes 9 -> 18 and no FAIL line); git diff --name-only af8629559..HEAD lists exactly the nine new .cs files (no .png/.obj/.xnb/.hjson/.ogg/.mp3/.fx/.bmp/.mapio/.ttf/.atlas); the AGENTS.md byte-level BOM check reports 'UTF-8 BOM check passed (1416 files).'"
        status: pass
      - kind: other
        ref: "git diff --stat a6f0a0df7..HEAD over 03-BIOLOGY.json and 03-BIOLOGY.md is empty (both byte-identical to their end-of-04-01 state, D-24/D-50), and the byte-identical Phase 3 gate still exits 0 under -RequireAll (OK(0): phase3 tranche = 5 / 5 (rows=31); OK: implemented classes = 5 / 5)"
        status: pass
    human_judgment: false

duration: 14min
completed: 2026-09-16
status: complete
---

# Phase 4 Plan 06: Spiny Moss Court — Soldiers, Commander & Flydragon Summary

**枯木活化士兵's four stat-variant classes with their boulder and three-ray spell, a summoning 王庭号令者 who keeps his distance, and 布罗迪蝇蜓 in both sizes — 18 of the phase's 37 guarded classes, with the morale/command and Valley egg systems left as precise D-46 blockers rather than half-built**

## Performance

- **Duration:** ~14 min (from the plan's first task to the plan-metadata commit)
- **Started:** 2026-09-16T07:58:43Z
- **Completed:** 2026-09-16T08:14:00Z
- **Tasks:** 3
- **Files modified:** 9 created (`+131 KB`), plus `.planning/WINDOWS.md`, `.planning/STATE.md` and `.planning/ROADMAP.md`

## Accomplishments

- **枯木活化士兵 is four classes, one per design stat row (Task 1, `92ccaf4ed`).** `AnimatedWitherbarkSoldier` is the row's own `internal_name` and carries 生命 80 / 伤害 22 / 防御 4 / 击退抗性 20 / 中毒 / 2银; `AnimatedWitherbarkSoldierRanged` 60/35/0/40/中毒+困惑/1银50铜; `AnimatedWitherbarkSoldierSpell` 55/24/0/40/中毒+困惑/1银50铜; `AnimatedWitherbarkHound` 75/20/2/70/中毒/1银20铜. Nothing mutates `NPC.lifeMax` outside `SetDefaults`, which is what makes OQ2/Pitfall 5 (T-04-38) structurally satisfied. All four are **functionally 中立**: no player targeting until a non-lethal hit (`HitEffect`, netmode-guarded) or a player inside the conservative `ProvokeRange` flips the state, and the provoked state falls back to 中立 past the leash range — every transition written under `Main.netMode != NetmodeID.MultiplayerClient` with `NPC.netUpdate`.
- **The four attack patterns are the design's own.** The melee variant mirrors the vanilla dungeon skeleton's approach (walk, jump at a wall/ledge/above-prey, contact damage) with a pinned `defDamage`/`defDefense` and a private `AnimatedWitherbarkSoldierState` over `NPC.ai[0]`. The ranged variant runs the design's exact cycle — `PickupFrames = 120` of immobility, then a throw of `AnimatedWitherbarkSoldier_Boulder`, then `RetreatFrames = 200` holding `PreferredRangeTiles = 8f` from the prey. The spell variant clones `NPCID.DarkCaster` for its bases and owns the mage loop: a 150-tick cast of **three** `AnimatedWitherbarkSoldier_SpellBeam` rays spread over a forward cone with gravity, then the vanilla mage's random teleport (up to 40 candidate tiles near the prey, never into terrain). The hound charges at 7.5 px/tick for 45 frames, recovers and turns, so 来回冲撞 is a real back-and-forth.
- **The morale/command seam is real, the system is not.** Every soldier exposes `MoraleDefenseBonus` / `MoraleDamageScale` / `MoraleSpeedScale` (neutral values 0 / 1 / 1) plus a named `IsNeutral`, read in `PostAI` and in the movement code, with a class comment stating that the Spiny Moss Court morale/command system is a D-46 blocker and that a later phase raises these members without reworking the AI. The design's 意志高涨 buff, its break-on-commander-death rule and the morale-gated 干涸心脏 drop apply nothing today.
- **Only the hound wires a drop.** `ItemDropRule.Common(ModContent.ItemType<ActivatedDogStaff>(), 11, 1, 1)` — the design's exact 9% (1/11). The other eight classes contain no `ModContent.ItemType<` token at all (audited mechanically), so 枯木碎块 (1~2 / hound 1) and 干涸心脏 (morale-gated 50%) remain D-58 blockers with no type reference and the mod still builds (T-04-36).
- **王庭号令者 (Task 2, `ea75f08b5`).** 生命 100 / 伤害 25（接触）/ 防御 4 / 击退抗性 50 / 2银50铜 at the design's low `CommanderWeight = 0.5f`, on a three-state `CourtCommanderState`: `Wandering` (aimless patrol), `Rallying` (the staff raised for `StaffRaiseFrames` as the design's visible tell, dust behind `!Main.dedServ`), `KeepingDistance` (steers away inside `RetreatTiles = 10f` while contact damage stays live, and gives the aggro up after `AggroGiveUpFrames` out of range). The summon fires on `OnSpawn` and on the first staff use of each aggro entry, only while fewer than `MinSoldiersNearby` soldiers stand within `SearchRadiusTiles`, capped at `SummonMin`–`SummonMax` (1–3) randomly chosen variants through `NPC.NewNPC` under the netmode guard, and made idempotent per aggro by the named `HasSummonedThisAggro` flag in `NPC.localAI[2]` (T-04-37).
- **布罗迪蝇蜓 in two sizes (Task 3, `782d9fdad`).** `BrodieFlydragon` (标准, the row's `internal_name`, 40/25/5/50/20 copper) and `SmallBrodieFlydragon` (小, 20/15/2/50/0 copper) share one behavioural vocabulary — a `BrodieFlydragonState` enum over `NPC.ai[0]`, named `HoverTimer`/`PursueTimer`/`AttackTimer` wrappers over `NPC.localAI[]`, and `Enter*` transition helpers — but keep their own stat lines and their own speeds. Both hover, pursue on aggro and press in for contact damage. Neither wires a drop (the design names none). The standard class carries the higher weight, so 自然刷新只会刷新标准大小的蝇蜓 is preserved, and the 森雨幽谷 egg route stays an unimplemented D-46 system: **no egg NPC, no egg projectile, no egg class and no reliance on one.**
- **The structural contract held across all nine files.** `Texture => Commons.ModAsset.White_Mod` on every file; `LocalizationCategory`, `NPCSpawnManager.RegisterNPC`, `Main.dedServ`, `NPC.netUpdate` and `ModifyNPCLoot` on every creature class; the two server-safe spawn tokens plus `KelpCurtainSpawnConditions.IsDryLand` on every `SpawnChance`; no `Main.LocalPlayer`, no `KelpCurtainBiome.IsBiomeActive` and no hard-coded region coordinate anywhere. The Phase 4 gate's `OK: guarded classes =` counter rose **9 → 15 → 16 → 18**, i.e. exactly the nine files this plan adds (T-04-35, T-04-39, T-04-40).

## Task Commits

Each task was committed atomically:

1. **Task 1: 枯木活化士兵 — four variant classes plus both soldier projectiles** — `92ccaf4ed` (feat)
2. **Task 2: 王庭号令者 `CourtCommander`** — `ea75f08b5` (feat)
3. **Task 3: 布罗迪蝇蜓 `BrodieFlydragon` + `SmallBrodieFlydragon`** — `782d9fdad` (feat)

**Plan metadata:** this SUMMARY, `STATE.md`, `ROADMAP.md` and `WINDOWS.md` are carried by the plan-metadata commit that follows it (the fourth commit in the `plan_head_before..HEAD` range).

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs` — 枯木活化士兵（近战）, the row's `internal_name`: neutral patrol → provoked fighter chase, the highest land weight (1.5f)
- `.../SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs` — 远程: the 120-frame boulder pickup, the throw, the 8-tile/200-frame spacing window
- `.../SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs` — 法术: the `NPCID.DarkCaster` clone, the fixed-interval three-ray cast, the random teleport
- `.../SpinyMossCourt/AnimatedWitherbarkHound.cs` — 犬: the charge-and-recover dash and the only wired drop (`ActivatedDogStaff`, 1/11)
- `.../SpinyMossCourt/CourtCommander.cs` — 王庭号令者: the wander / staff-raise / keep-10-tiles machine and the once-per-aggro summon
- `.../SpinyMossCourt/BrodieFlydragon.cs` — 布罗迪蝇蜓（标准）: hover / pursue / melee press at the standard weight
- `.../SpinyMossCourt/SmallBrodieFlydragon.cs` — 布罗迪蝇蜓（小）: the same three states at the small stat row and the lower weight
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs` — the 远程 variant's gravity-arc boulder
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_SpellBeam.cs` — one ray of the 法术 variant's three-ray spray
- `.planning/WINDOWS.md` — entries 35–39 appended (one `unrun-verify`, four `deviation`)

## Decisions Made

- **Four classes, never one with a mutated stat.** The design's four rows differ in life, damage, defence, knockback resistance and drops, and `SetDefaults` runs once per NPC type — so the family is four types (OQ2, T-04-38).
- **The melee variant owns its fighter AI rather than cloning the vanilla skeleton AI.** The vanilla fighter `aiStyle` always acquires a player target, which would make the design's explicit 中立 default (a `must_haves` truth) inexpressible, and a private state enum over `NPC.ai[0]` cannot coexist with a cloned AI that owns that slot. The mirrored approach, the pinned `defDamage`/`defDefense` and the `NPC.ai[0]` state enum are all kept; the divergence is recorded as a deviation (`WINDOWS.md` entry 36).
- **`中立` is a functional state, not a comment.** Provocation is damage taken plus a conservative proximity trigger; the provoked state is left past a leash range. Both transitions are authoritative with `NPC.netUpdate`, and the neutral value is exposed as a named member so a later morale system can read and flip it.
- **The spell variant clones for bases only and sets `NPC.aiStyle = -1`.** The engine's caster AI fires the vanilla caster's own projectile on its own cadence; keeping it would have meant two competing casts and the wrong beam. The design's documented 固定间隔 cast then 随机传送 is implemented locally instead, with the clone supplying only the caster's collider/sound bases.
- **The design snapshot wins on the flydragon 钱币 cells.** The plan's inline table marks both as empty; the design's own stats table `V9zXdPuSdoQD7bxe6IVcdzqrncf` gives 20 copper for 普通（标准） and 0 for 普通（小）, and Task 3's action instructs reading that table directly (D-25). `WINDOWS.md` entry 38 records it because `04-DEVIATIONS.md` section 10 cannot be edited by this plan.
- **The commander's summon is idempotent per aggro entry.** A single named flag over `NPC.localAI[2]` (`HasSummonedThisAggro`) is re-armed only when the aggro is entered, so 初次使用权杖时 can never summon twice for one aggro and the world cannot be flooded (T-04-37).
- **Two blockers, zero half-builds.** The morale/command system and the Valley egg system are recorded as precise D-46 blockers naming the affected files; the soldiers instead expose the members a later buff reads, and the small flydragon is reachable through its own natural weight so the design's creature exists without an egg path (D-46/D-47).
- **Conservative defaults live in the class comments and here, not in the ledger.** Every number this plan chose is named (see below), because wave-3 prohibitions forbid editing `04-DEVIATIONS.md` (the 04-05 precedent).

**Conservative defaults this plan chose (D-54/D-59; the ledger may not be edited by this plan).**

| Default | Value | Where |
| --- | --- | --- |
| Hit boxes (no sprite exists) | soldier variants `30x46` / hound `40x26` / commander `34x50` / flydragons `44x32` and `30x22` | all seven creature classes |
| Provocation proximity radius | `ProvokeRange = 10` tiles | the four soldier classes |
| Soldier patrol / chase / jump | `WanderSpeed` 0.8–1.1, `ChaseSpeed` 2.2 (melee), `JumpSpeed` 6.4, `JumpCooldownFrames` 45 | `AnimatedWitherbarkSoldier` |
| Soldier leash ranges | 60–70 tiles | all four soldier classes |
| Ranged pickup approach | `PickupRange = 16` tiles, `BoulderSpeed = 8.5`, spacing slack 1.5 tiles | `AnimatedWitherbarkSoldierRanged` |
| Spell cadence / spray / teleport | `CastInterval = 150`, `SpellSpreadRadians = 0.20`, `SpellBeamSpeed = 9`, teleport radius 20 x 8 tiles, `TeleportAttempts = 40`, `ClearanceTiles = 3` | `AnimatedWitherbarkSoldierSpell` |
| Hound dash | `ChargeSpeed = 7.5`, `ChargeFrames = 45`, `RecoverFrames = 60`, `RecoverSpeed = 1.4` | `AnimatedWitherbarkHound` |
| Commander | weight `0.5f`, `AggroRange = 30` tiles, `StaffRaiseFrames = 60`, retreat slack 2 tiles, `AggroGiveUpFrames = 180`, summon spread 6 tiles | `CourtCommander` |
| Flydragon flight | hover 2.0/2.2, pursue 3.6/3.8, lunge 5.5/5.8, melee range 2.6/2.2 tiles, hover cycle 150, lunge 30, give-up 300, bob 0.45/0.5 | both flydragon classes |
| Per-variant spawn weights | melee 1.5 (the design's highest), hound `LandWeight` 1f, ranged 1f, spell 0.75, standard flydragon `LandWeight` 1f, small flydragon 0.5 | the six `SpawnChance` bodies |
| Projectile lifetimes / gravity | boulder 300 ticks at 0.35; spell beam 180 ticks at 0.14 | both projectile classes |

## Deviations from Plan

### Auto-fixed Issues

None — no build break, no logic bug and no missing critical functionality was found during execution. `NPC.rare` (which the plan prose writes) was written as `NPC.rarity` from the start because the correction is already recorded in `04-DEVIATIONS.md` §10; this is its fifth occurrence in the project and needed no new fix.

### Documented implementation decisions (recorded, not silently taken)

**1. The melee variant owns a local fighter AI instead of cloning the vanilla skeleton AI**
- **Found during:** Task 1 (`AnimatedWitherbarkSoldier`)
- **Issue:** Task 1's action asks the melee class to mirror the vanilla dungeon skeleton *and* to keep a private state enum over `NPC.ai[0]` *and* to be neutral by default. The vanilla fighter `aiStyle` unconditionally acquires a player target, so neutrality would be inexpressible while it runs, and a state enum over `NPC.ai[0]` would collide with the cloned AI's own state machine.
- **Fix:** `NPC.aiStyle = -1` with an owned fighter AI (walk toward the prey, jump at a wall/ledge or when the prey is above, contact damage), the pinned `defDamage`/`defDefense` (22 / 4), the `AnimatedWitherbarkSoldierState` enum over `NPC.ai[0]` and the neutrality rule. The mirror is of the *approach*, not of the engine's targeting.
- **Verification:** Release build 0/0; the gate's guarded-class count rose by six for the task.
- **Recorded in:** `WINDOWS.md` entry 36.

**2. The flydragon 钱币 cells come from the design snapshot, not the plan's inline table**
- **Found during:** Task 3
- **Issue:** The plan's Design Values table lists both flydragon 钱币 cells as empty; the committed design snapshot's stats table `V9zXdPuSdoQD7bxe6IVcdzqrncf` gives `20` for 普通（标准） and `0` for 普通（小）.
- **Fix:** `NPC.value = 20` on `BrodieFlydragon` and `NPC.value = 0` on `SmallBrodieFlydragon`, following Task 3's explicit instruction to read the design table directly (D-25).
- **Verification:** Release build 0/0; both values audited in `04-06-SUMMARY.md`'s coverage row D3.
- **Recorded in:** `WINDOWS.md` entry 38.

**3. `NPC.GetSource_FromAI` is realised at the spawn site, not inside the projectile classes**
- **Found during:** Task 1
- **Issue:** Task 1's acceptance criteria ask for `NPC.GetSource_FromAI` in the two projectile files, but a `ModProjectile` exposes no `NPC` member, so the call cannot exist there.
- **Fix:** the real calls sit at the two spawn sites (`AnimatedWitherbarkSoldierRanged.ThrowBoulder`, `AnimatedWitherbarkSoldierSpell.UpdateCasting`), both inside `Main.netMode != NetmodeID.MultiplayerClient`, and each projectile's XML doc names that spawn site — the documented plan 04-02 precedent, not a comment-only fake.
- **Verification:** Release build 0/0; both projectile files contain the `NPC.GetSource_FromAI` token in the doc comment describing the real call.
- **Recorded in:** `WINDOWS.md` entry 39.

### Process deviations

**4. The `tdd="true"` task attribute is realised as the gate's monotone guarded-class counter plus the Release build**

- **Found during:** all three tasks
- **Issue:** All three tasks carry `tdd="true"`, but the repository has no unit-test infrastructure for `ModNPC` behaviour (spawn isolation, AI feel, netmode discipline) — the plan's own `<fails_when>` is defined in terms of `error CS`, the Phase 4 gate's exit code and the `OK: guarded classes =` delta.
- **Fix:** each task ran its full automated `<verify>` (Release build → Phase 4 gate) and was committed only when green, with the counter as the monotone progress instrument: **9 → 15** (Task 1, +6 = four soldier classes + two projectiles), **15 → 16** (Task 2, +1), **16 → 18** (Task 3, +2). No test was fabricated and no RED/GREEN pair is claimed. This mirrors plan 04-02, whose `tdd="true"` tasks were handled the same way.
- **Verification:** the three gate runs quoted in `## Accomplishments`, plus the final combined run below.
- **Recorded in:** this SUMMARY.

---

**Total deviations:** 3 documented implementation decisions + 1 documented process deviation, all recorded rather than fixed silently.
**Impact on plan:** No scope creep. The three implementation decisions are the ones the plan's own text leaves to the executor (neutrality vs a cloned vanilla AI, a plan-table-vs-snapshot value conflict, and an impossible token placement); none changes a design value, a spawn weight, a drop or a blocker.

## Issues Encountered

- **Console encoding noise (pre-existing).** `dotnet build` and `git diff` render CJK as mojibake in the PowerShell 5.1 session. It is a display artefact only: the gate's byte-level UTF-8 BOM check and the AGENTS.md BOM block both pass, and the AGENTS.md byte-level sweep reports `UTF-8 BOM check passed (1416 files).`
- **Time-zone skew in the elapsed-time calculation.** `[datetime]::Parse('…Z')` returns a local-time `DateTime`, so a naive subtraction against `Get-Date).ToUniversalTime()` reported a negative duration. The real elapsed window is the `Started`/`Completed` pair above (~14 min), taken from `Get-Date).ToUniversalTime()` at each end.

## Known Stubs

None. No hardcoded empty value, placeholder string or unwired data source was introduced. Two shapes could be mistaken for stubs and are not:

- **The morale members.** `MoraleDefenseBonus = 0`, `MoraleDamageScale = 1f`, `MoraleSpeedScale = 1f` are the *neutral* values of a documented public seam whose consumer is the unimplemented morale/command system (D-46). They are read (in `PostAI` and in the movement code), so they are wired, not dead.
- **The empty `ModifyNPCLoot` bodies.** They are the design's own empty tables: the soldier row and the commander row name absent materials (枯木碎块 / 干涸心脏, D-58) and the flydragon row names no drop at all, each explained in the method's XML doc — the `VerdantRods`/`GuppyConch` contract. The hound, the only variant with a repository item behind it, wires its rule.

## Threat Flags

None. Every file created by this plan is a hostile `ModNPC` or `ModProjectile` inside the existing Kelp Curtain layer, consuming only the three spawn predicates, the existing `WitherWoodDust` and the Phase 1 `ActivatedDogStaff` type. No new network endpoint, auth path, file-access pattern or trust-boundary schema change was introduced; the plan's own threat register (T-04-34 … T-04-40) covers the surface and every disposition is met.

## Defect Ledger

Five entries were appended to `.planning/WINDOWS.md` (they block `/gsd-ship` while open, by design; `open_count` is now 37):

- **#35 `unrun-verify`** — the plan's D-21 runtime human checks (spawn isolation for all seven creatures, the neutral-until-provoked reading, the four attack patterns, the summon cap, the clean `White_Mod` load) are not executed; plan 04-09 records them in `04-UAT.md`.
- **#36 `deviation`** — the melee variant's owned fighter AI instead of the cloned vanilla one.
- **#37 `deviation`** — the fifth `NPC.rare` → `NPC.rarity` correction.
- **#38 `deviation`** — the flydragon 钱币 values taken from the design snapshot rather than the plan's inline table.
- **#39 `deviation`** — the `NPC.GetSource_FromAI` acceptance token realised at the spawn site for the two projectiles.

## User Setup Required

None — no external service configuration required, no package was installed (`04-RESEARCH` §Package Legitimacy Audit: not applicable).

## Next Phase Readiness

- **The Spiny Moss Court is code-complete.** All three of its design rows now have loadable, layer-gated classes: the four 枯木活化士兵 variants (with their two projectiles), 王庭号令者 and both 布罗迪蝇蜓 sizes. The gate's `OK: guarded classes = 18` counts the `SpinyMossCourt` region folder for the first time, and wave 3 still has `04-03`, `04-04`, `04-07` and `04-08` outstanding before plan 04-09 closes the phase at 37.
- **Waves 3–4 have what they need:** the third and last region root exists, the D-49 art migration is the same three mechanical steps for all nine files, and the neutral-until-provoked + morale-seam shape is available to any later court creature.
- **Outstanding, all recorded, none hidden:**
  - the D-21 runtime bundle (`04-UAT.md`, plan 04-09): every spawn predicate inside Yggdrasil and its absence in an ordinary world, the soldiers' neutral-until-provoked reading, the 120-frame pickup / 8-tile spacing, the three-ray arc, the hound's back-and-forth charge, the commander's once-per-aggro summon and 10-tile spacing, and the clean `White_Mod` load of all nine classes (`WINDOWS.md` entry 35);
  - **approved art for the nine new sprites** — blocker only, no placeholder art created (D-48/D-51);
  - the unimplemented systems this plan touches: the Spiny Moss Court morale/command system (which would consume the soldiers' `Morale*` members and the 干涸心脏 drop) and the 森雨幽谷 Valley egg system (`04-DEVIATIONS.md` §7/§13);
  - the 森雨幽谷 / 刺苔庭园 region-level spawn predicates (Phases 5–6, D-52);
  - the absent drop materials 枯木碎块 / 干涸心脏 (D-58) and localization (D-20) for all seven creatures and two projectiles;
  - the conservative defaults table above, which is the first thing to revisit when the art arrives (each hit box in particular).
- **This plan flipped no matrix row.** `03-BIOLOGY.json`, `03-BIOLOGY.md` and `04-DEVIATIONS.md` are untouched; plan 04-09 mirrors the remaining twenty in-scope rows to `code_complete: true` and writes §14. `REQUIREMENTS.md` stays Pending until then.

---

*Phase: 04-remaining-ordinary-monsters*
*Completed: 2026-09-16*

## Self-Check: PASSED

- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkHound.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/CourtCommander.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/SmallBrodieFlydragon.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_SpellBeam.cs`
- FOUND commit: `92ccaf4ed` (Task 1)
- FOUND commit: `ea75f08b5` (Task 2)
- FOUND commit: `782d9fdad` (Task 3)
- FOUND commit: `af8629559` (`plan_head_before`)
