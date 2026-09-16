---
phase: 04-remaining-ordinary-monsters
plan: 02
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, subworld, yggdrasil, kelp-curtain, death-jade-lake, spawn-predicate, water-surface, amphibious, moisture-budget, projectile, hostile-projectile, debuff-mapping, server-authoritative]

# Dependency graph
requires:
  - phase: 04-remaining-ordinary-monsters
    provides: the green 13-invariant Phase 4 gate (`scripts/check-biology.ps1`), `KelpCurtainSpawnConditions` with the three server-safe water/land predicates and the D-54 weight bands, the `DeathJadeLake` region-folder/namespace rule and the `JadeSpiritAnglerfish` art-missing class shape
  - phase: 03-completed-art-ordinary-monsters
    provides: the KelpCurtain NPC precedents (`GuppyConch`, `VerdantRods`, `RiverSlug`) and the hostile-projectile shape (`GiantDandelion_Shockwave`, `GiantDandelion_Boulder`)
provides:
  - Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.WaterStrider (水黾 implemented end-to-end)
  - Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.ToxicToad (剧毒蟾蜍 implemented end-to-end)
  - Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.GlowSalamander (幽光蝾螈 implemented end-to-end)
  - Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.ToxicToad_PoisonBubble (projectile 1 of the frozen eleven)
  - Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.ToxicToad_PoisonCloud (projectile 2 of the frozen eleven)
  - "the first real consumers of all three `KelpCurtainSpawnConditions` predicates: `IsWaterSurface` (水黾), `IsDryLand` (剧毒蟾蜍) and `spawnInfo.Water` (幽光蝾螈)"
affects: [04-03, 04-04, 04-05, 04-06, 04-07, 04-08, 04-09, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (60000 tokens, 2 tasks) to calibrate future estimates.
# chars/4 over the added lines of the plan's own commits (a6f0a0df7..HEAD), never a harness token count.
actuals:
  tokens: 34211
  tasks: 2
  commits: 3   # measured: git rev-list --count a6f0a0df7..HEAD (2 task commits + 1 plan-metadata commit)
plan_head_before: a6f0a0df7cef1b0c00cb6ab552a30e5db38c96e3

tech-stack:
  added: []
  patterns:
    - "One class per design water condition, each consuming the shared `KelpCurtainSpawnConditions` predicate instead of re-implementing the test (D-53, Pitfall 6)"
    - "The design's dash cadence stored in a named `NPC.localAI[]` wrapper and re-picked from the design's own two frame bands (60-200 / 45-150) on the authoritative side only"
    - "Amphibious moisture as a per-NPC budget over `NPC.localAI[]` with an explicit `SendExtraAI`/`ReceiveExtraAI` contract, never a `Player` field (D-55, T-04-14)"
    - "Cross-creature aggro preference modelled as synced prey/toad indices in `NPC.ai[]`, resolved through `ModContent.NPCType<...>` rather than a string or name lookup"
    - "Ground/wall recovery generalised across two creatures as a bounded horizontal liquid search plus a single-direction fallback"
  key_links:
    - "WaterStrider.SpawnChance -> KelpCurtainSpawnConditions.IsWaterSurface -> the design's 水面上 condition"
    - "ToxicToad.SpawnChance -> KelpCurtainSpawnConditions.IsDryLand -> the design's 在地表生成"
    - "GlowSalamander.SpawnChance -> spawnInfo.Water -> the design's 在水下生成"
    - "ToxicToad.OnHitPlayer / ToxicToad_PoisonBubble.OnHitPlayer / ToxicToad_PoisonCloud.OnHitPlayer -> Main.rand.NextBool(3) -> 600-tick BuffID.Poisoned : 420/300-tick BuffID.Venom"
    - "GlowSalamander moisture budget -> Main.GameUpdateCount % 30 -> NPC.AddBuff(BuffID.Suffocation, 60)"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonBubble.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonCloud.cs
  modified:
    - .planning/WINDOWS.md

key-decisions:
  - "The plan's `NPC.rare = ItemRarityID.White;` is written as `NPC.rarity` - the third repeat of the Rule 1 API-name correction (Phase 3 and plan 04-01 recorded the same), because `NPC` exposes no `rare` member in this tML build"
  - "`WaterStrider` derives its three-way water split (surface band / dry land / below the band) from synced tile reads and writes the state authoritatively, so a client branches exactly like the server without needing the state write (D-55)"
  - "`WaterStrider` switches `NPC.noGravity` from world state: surface tension while in liquid, gravity on land, which is what makes the 小跳 recovery read as a hop rather than a float"
  - "`ToxicToad` and `GlowSalamander` are co-authored in one task because the toad's preference list names the salamander type and the salamander's retreat rule names the toad type; splitting them would put a `CS0246` in the earlier task's build"
  - "The design's 酸性毒液 is applied as `BuffID.Venom` everywhere (contact, bubble, death cloud) through the single mapping recorded in `04-DEVIATIONS.md` §10; no new buff and no icon was created (D-51)"
  - "Both absent 剧毒蟾蜍/幽光蝾螈 drops (毒腺, 牛黄) ship as an explicitly empty, commented `ModifyNPCLoot` with no item-type reference, so the mod still builds and loads (D-58, T-04-11)"
  - "The 幽光蝾螈 colour variant lives in a synced `NPC.localAI[]` slot and is deliberately inert until approved art arrives (OQ2/D-49); it is drawn in `OnSpawn` on the authoritative side and sent through `SendExtraAI`/`ReceiveExtraAI`"
  - "The moisture budget drains and refills only on the authoritative side and forces an extra `NPC.netUpdate` every 30 drained frames, so every side derives the same Returning/Suffocating state from the synced budget"
  - "Cross-creature hostility is modelled as target preference only: `ToxicToad` prefers a nearby 幽光蝾螈/水蛞蝓 over the player and `GlowSalamander` retreats from a `ToxicToad`, but the engine has no NPC-versus-NPC damage path, so the residual limitation stays the one already recorded in `04-DEVIATIONS.md` §7/§13"

patterns-established:
  - "A surface-only creature consumes `IsWaterSurface` rather than inventing a `LiquidSurfaceY` comparison, keeping the single server-safe predicate the tracer created as the shared seam"
  - "A `White_Mod` art-missing pair of mutually-referencing creatures is co-authored in one task so the mutual `ModContent.NPCType<...>` references resolve inside a single Release build"
  - "Every KelpCurtain hostile projectile in this phase reuses an existing KelpCurtain dust inside `if (!Main.dedServ)` and overrides `Texture` to `Commons.ModAsset.White_Mod`, so the D-49 migration is art-only"
  - "Moisture / dash timers are per-NPC state in `NPC.localAI[]` with named wrappers; no timer is ever stored on a `Player`"

requirements-completed: []
requirements-advanced: [BIO-01, BIO-06]

coverage:
  - id: D1
    description: "水黾 (`WaterStrider`) end-to-end: surface-only spawn (subworld + layer + `IsWaterSurface`), the design's 60-200 / 45-150 frame dash cadence aiming at a near player, the 小跳回最近的水面 land recovery with the single-direction fallback, the 游回水面 lake-bed recovery, and the design's stats (生命 45 / 伤害 20 / 防御 10 / 击退抗性 40% / 钱币 80 / 免疫 中毒·潮湿) with an empty loot table"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors, Everglow.tmod packaged)"
        status: pass
      - kind: other
        ref: "powershell -NoProfile -ExecutionPolicy Bypass -File .planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 (exit 0; 'OK(0): phase4 in-scope set = 21 rows (rows=31)', 'OK: guarded classes = 2', 'OK: UTF-8 BOM check passed')"
        status: pass
      - kind: other
        ref: "token audit over WaterStrider.cs: the two spawn tokens, IsWaterSurface, the four dash-band constants, the recovery branches, and the absence of Main.LocalPlayer / ModContent.ItemType< / IsBiomeActive"
        status: pass
    human_judgment: true
    rationale: "Whether the derived surface test selects the intended tiles, whether the dash cadence reads as 一小段一小段, and whether the land hop actually lands the creature back in water can only be judged in a live tModLoader client; the client/dedicated-server checks are the D-21 bundle plan 04-09 records (04-DEVIATIONS.md section 5, section 11)."
  - id: D2
    description: "剧毒蟾蜍 (`ToxicToad`) plus both of its projectiles: dry-land spawn, the 蝾螈/水蛞蝓-over-player aggro preference, the poison-bubble fire cadence, swimming when submerged, the 75% 10 s 中毒 / 25% 7 s 酸性毒液 split on contact and from the bubble, the 180-frame / 10-damage / 300-tick-Venom death cloud, the design's stats (生命 80 / 伤害 25 / 防御 8 / 击退抗性 80% / 钱币 2银 / 免疫 中毒·酸性毒液), and the explicitly empty 毒腺/牛黄 loot table"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors); the mutual ModContent.NPCType<GlowSalamander>/<ToxicToad> references resolve in this one build"
        status: pass
      - kind: other
        ref: "check-biology.ps1 (exit 0; 'OK: guarded classes = 6', +4 against the previous task run as the plan's fails_when requires)"
        status: pass
      - kind: other
        ref: "token audit over ToxicToad.cs / ToxicToad_PoisonBubble.cs / ToxicToad_PoisonCloud.cs: IsDryLand, Player.HurtInfo, the unguarded OnHitPlayer body, Main.rand.NextBool(3), Player.GetSource-equivalent NPC.GetSource_FromAI at the spawn site, no ModContent.ItemType< and no Main.LocalPlayer"
        status: pass
    human_judgment: true
    rationale: "The prey-over-player preference, the bubble cadence, the 75/25 split feel and the death cloud's contact damage are runtime properties of a live client; the absent 毒腺/牛黄 materials and the engine's lack of an NPC-versus-NPC damage path are recorded blockers, not code outcomes."
  - id: D3
    description: "幽光蝾螈 (`GlowSalamander`) end-to-end: underwater spawn, the design's stats (生命 120 / 伤害 30 / 防御 4 / 击退抗性 20% / 钱币 2银 / 免疫 中毒), the synced three-colour variant slot drawn at spawn, the exact 3600-frame moisture budget with the 600-frame returning instinct, the 30-frame `BuffID.Suffocation` pulses at zero and the refill/clear on re-entry, the underwater dash-and-melee, `RiverSlug`/underwater prey selection, the toad retreat, the 10 s 中毒 on contact, and the empty 毒腺/牛黄 loot table"
    requirement: BIO-01
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (0 warnings, 0 errors)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 (exit 0; 'OK: guarded classes = 6')"
        status: pass
      - kind: other
        ref: "token audit over GlowSalamander.cs: spawnInfo.Water, the moisture constants 3600/600/30, BuffID.Suffocation, the two enums, SendExtraAI/ReceiveExtraAI, no ModContent.ItemType< / Main.LocalPlayer / IsBiomeActive, and no timer stored on a Player"
        status: pass
    human_judgment: true
    rationale: "The 60 s / 10 s / 30-frame moisture cycle, the crawl-back-to-water search and whether this tML build's NPC buff pipeline turns `BuffID.Suffocation` into visible pressure are runtime properties; the D-21 bundle plan 04-09 writes names them."
  - id: D4
    description: "The phase gate advanced by five new guarded classes (2 -> 6, one creature per task plus two projectiles) with the Phase 3 chain and the frozen matrix untouched"
    requirement: BIO-06
    verification:
      - kind: other
        ref: "check-biology.ps1 exit 0 with 'OK: guarded classes = 6' and 'OK: reconciled rows = 1 / 21' (waves 2-4 create classes; plan 04-09 reconciles)"
        status: pass
      - kind: other
        ref: "powershell -File .planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1 -RequireAll (exit 0; 'OK(0): phase3 tranche = 5 / 5 (rows=31)', 'OK: implemented classes = 5 / 5')"
        status: pass
      - kind: other
        ref: "AGENTS.md byte-level check: 'UTF-8 BOM check passed (1402 files).' and git status shows 03-BIOLOGY.json / 03-BIOLOGY.md unmodified"
        status: pass
    human_judgment: false

# Metrics
duration: 80min
completed: 2026-09-16
status: complete
---

# Phase 4 Plan 02: Death Jade Lake Surface & Amphibious Family Summary

**水黾 surface skater, 剧毒蟾蜍 amphibious ambusher with its poison bubble and death cloud, and 幽光蝾螈 moisture-budgeted axolotl — the first consumers of all three shared water conditions**

## Performance

- **Duration:** ~80 min (from the plan's start to the plan-metadata commit)
- **Started:** 2026-09-16T06:08:00Z
- **Completed:** 2026-09-16T07:28:00Z
- **Tasks:** 2
- **Files modified:** 6 (5 created, 1 updated)

## Accomplishments

- **水黾 (`WaterStrider`) — the phase's first `IsWaterSurface` consumer.** `SpawnChance` returns `0f` unless `SubworldSystem.IsActive<YggdrasilWorld>()`, `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` **and** `KelpCurtainSpawnConditions.IsWaterSurface(spawnInfo)` all hold, so the design's 只会在水面上刷新 is the shared predicate rather than a per-class test — and a lake-bed spawn is structurally impossible. Its three states (`Skating` / `Dashing` / `Returning`) ride a wrapped `NPC.ai[0]`, with the dash timer on `NPC.localAI[0]` and the chosen heading on `NPC.localAI[1]`. The cadence is the design's exactly: a random delay in **60–200** frames with no player nearby, tightening to **45–150** frames and aiming at the player inside 24 tiles. Both recovery clauses are implemented: on land it hops toward the nearest liquid inside a bounded 30-tile / 4-tile-deep search and, when there is none, keeps hopping in one direction (小跳回最近的水面 / 随机选择一个方向一直跳); below the surface band it swims up (游回水面). `NPC.noGravity` is derived from world state — surface tension in liquid, gravity on land — which is what makes the hop read as a hop. Stats are the design's own (生命 45 / 伤害 20 / 防御 10 / 击退抗性 40% → `0.6f` / 钱币 80 / `NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned]` + `[BuffID.Wet]`), the hit box takes the design's 体型接近史莱姆 proportion, and `ModifyNPCLoot` exists with no rule because the row names no drop.
- **剧毒蟾蜍 (`ToxicToad`) + `ToxicToad_PoisonBubble` + `ToxicToad_PoisonCloud` — the design's ranged amphibian and its whole kit.** Spawning is `KelpCurtainSpawnConditions.IsDryLand` (the design's 在地表生成) at the conservative `LandWeight`; the aggro-preference rule 对玩家仇恨低于这些生物 is a documented constant block plus a timed scan that resolves the two prey species through `ModContent.NPCType<GlowSalamander>()` / `<RiverSlug>()`, so a live 幽光蝾螈 or 水蛞蝓 inside 40 tiles outranks the player. The bubble is launched from `NPC.GetSource_FromAI()` on the authoritative side at the design's fire range; the design's 落入水中的话会游泳 is a `noGravity` swim. 不论何种方式都会有75%概率造成10秒中毒，剩下25%概率造成7秒酸性毒液 is one `Main.rand.NextBool(3)` split implemented identically on the creature's `OnHitPlayer` (deliberately **not** netMode-guarded, because tML runs that hook on the local client) and on the bubble's, with 酸性毒液 applied as the mapped `BuffID.Venom`. 死亡后爆炸…产生一小团剧毒云 is one `ToxicToad_PoisonCloud` spawned from the creature's `HitEffect` under `Main.netMode != NetmodeID.MultiplayerClient`: it is stationary, lives 180 frames (持续3秒), deals 10 contact damage and applies 300 ticks of `BuffID.Venom`, and its `ai[0]` carries the lifetime through a named `LifetimeCounter` wrapper. Its stats are the design's (生命 80 / 伤害 25 / 防御 8 / 击退抗性 80% → `0.2f` / 钱币 2银 / 免疫 中毒·酸性毒液) and its loot table is explicitly empty with the 毒腺 (33%) and 牛黄 (1%/2%) blockers named and **no** item-type reference (D-58).
- **幽光蝾螈 (`GlowSalamander`) — the phase's first moisture budget and its only synced colour variant.** `SpawnChance` consumes `spawnInfo.Water` (the design's 在水下生成), and the class runs the design's cycle literally: a **3600**-frame (60 s) budget that drains only while out of water, a **600**-frame (10 s) returning instinct that crawls back toward the nearest liquid with a single-direction fallback, and at zero a **60**-tick `BuffID.Suffocation` pulse every **30** frames until it re-enters water, which refills the budget and clears the pulse through engine-owned `NPC.AddBuff` / `NPC.DelBuff` calls (the authoritative pair the tML docs name for NPC debuffs). The whole budget is per-NPC state in `NPC.localAI[]` with a named `Moisture` wrapper — never a `Player` field — and it is re-sent every 30 drained frames so every side derives the same `Returning` / `Suffocating` state. The four-state machine (`Swimming` / `Dashing` / `Returning` / `Suffocating`) dashes at prey underwater on a cooldown, picks a 水蛞蝓 or any creature sitting in liquid as its target (会主动攻击所有发现的水下生物和蛞蝓) and retreats from a nearby 剧毒蟾蜍 (主动远离蟾蜍). The 三种颜色变种 slot is drawn once in `OnSpawn` on the authoritative side, synced through `SendExtraAI` / `ReceiveExtraAI`, and is **documented as inert until approved art arrives** with `Commons.ModAsset.White_Mod` — it exists so the D-49 migration maps colour sheets onto frames rather than reworking the class (OQ2). Stats are the design's (生命 120 / 伤害 30 / 防御 4 / 击退抗性 20% → `0.8f` / 钱币 2银 / 免疫 中毒), contact applies 10 s (600 ticks) of 中毒, and its loot table is empty with the same 毒腺/牛黄 blockers.
- **The phase gate advanced by five files with nothing else moving.** `check-biology.ps1` reports `OK: guarded classes = 2` after Task 1 and `= 6` after Task 2 (+4 exactly as Task 2's `fails_when` requires, i.e. +5 across the plan), `OK(0): phase4 in-scope set = 21 rows (rows=31)` and `OK: reconciled rows = 1 / 21` throughout. The Phase 3 chain still reproduces (`-RequireAll` → `OK(0): phase3 tranche = 5 / 5 (rows=31)`, `OK: implemented classes = 5 / 5`), and `03-BIOLOGY.json` / `03-BIOLOGY.md` are byte-identical to their end-of-04-01 state (`git status` clean for both, D-24/D-50/T-04-15).

## Task Commits

Each task was committed atomically:

1. **Task 1: 水黾 `WaterStrider` — surface-only skater with dash cadence, land-return hops and water-bottom recovery** — `23eb94c88` (feat)
2. **Task 2: 剧毒蟾蜍 `ToxicToad` and 幽光蝾螈 `GlowSalamander` plus both `ToxicToad` projectiles** — `5493aed98` (feat)

**Plan metadata:** this SUMMARY, `STATE.md`, `ROADMAP.md` and `.planning/WINDOWS.md` are carried by the plan-metadata commit that follows it (the third commit in the `a6f0a0df7..HEAD` range).

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.cs` — 水黾: surface-only `IsWaterSurface` spawn, the 60–200 / 45–150 frame dash cadence over named `localAI` wrappers, the land hop and lake-bed swim recoveries, design stats and an empty loot table
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs` — 剧毒蟾蜍: `IsDryLand` spawn, the 蝾螈/水蛞蝓-over-player preference, the bubble cadence, swimming, the 75%/25% 中毒/酸性毒液 contact split, the death cloud spawn and the empty 毒腺/牛黄 loot table
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs` — 幽光蝾螈: underwater spawn, the 3600/600/30 moisture-and-suffocation cycle over synced `localAI`, the synced colour-variant slot, the underwater dash-and-melee, prey selection and the toad retreat
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonBubble.cs` — the projectile 1 of the frozen eleven: a light hostile bubble carrying the creature's 75%/25% debuff split
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonCloud.cs` — the projectile 2 of the frozen eleven: the stationary 180-frame, 10-damage, 300-tick-`BuffID.Venom` death cloud with its `ai[0]` lifetime wrapper
- `.planning/WINDOWS.md` — five cross-phase ledger entries appended for this plan (see the Defect Ledger section)

## Decisions Made

- **`NPC.rare` → `NPC.rarity` (Rule 1, third occurrence).** The plan prose writes `NPC.rare = ItemRarityID.White;` for all three creatures; `NPC` exposes no `rare` member in this tML build, so every class writes `NPC.rarity` with the design's empty 稀有度 cell named in the comment. `04-DEVIATIONS.md` §10 already records the correction (plans 03-01 and 04-01 made it too), so nothing new was added to the ledger.
- **The water condition is derived and the state is written, not the other way round.** All three classes compute their state from synced world reads (tiles, `spawnInfo.Water`, the moisture budget) and only *write* `NPC.ai[0]` under `Main.netMode != NetmodeID.MultiplayerClient`. That way a client branches exactly like the server even on the tick before the write lands, which is what the design's "recover from wherever you ended up" clauses need to avoid flicker.
- **The co-authored pair stayed co-authored.** `ToxicToad.FindPreferredPrey` names `ModContent.NPCType<GlowSalamander>()` and `GlowSalamander.FindNearestToad` names `ModContent.NPCType<ToxicToad>()`. Both resolve in one Release build because they land in one task — splitting them would have put a `CS0246` in the earlier task's `dotnet build`.
- **酸性毒液 is `BuffID.Venom`, everywhere, from the one recorded mapping.** The creature's contact, the bubble and the death cloud all use it. No `ModBuff` and no icon was created (D-51), and the mapping stays the single deviation already in `04-DEVIATIONS.md` §10.
- **The two absent drop materials produce an empty table, not a type reference.** 毒腺 and 牛黄 have no class in the repository, so `ToxicToad.ModifyNPCLoot` and `GlowSalamander.ModifyNPCLoot` contain a commented no-op and **no** `ModContent.ItemType<...>` token — the gate's item-type index and the Release build are the two checks (D-58, T-04-11).
- **The 幽光蝾螈 variant is synced but deliberately inert.** It is drawn once on the authoritative side, stored in a named `localAI` wrapper, sent through `SendExtraAI`/`ReceiveExtraAI`, and documented as invisible until approved art arrives — the D-49 migration maps frames instead of reworking the class (OQ2).
- **The moisture budget is per-NPC, never per-player.** It lives in `NPC.localAI[0]` with a named wrapper, drains only on the authoritative side, and forces an extra `NPC.netUpdate` every 30 drained frames so all sides derive the same state (Pitfall 6, T-04-14).
- **Cross-creature hostility is target preference only.** The toad prefers 蝾螈/水蛞蝓 over the player and the salamander retreats from the toad, but tML has no NPC-versus-NPC damage path (hostile projectiles damage players only), so neither actually damages the other. That residual limitation is the one already recorded in `04-DEVIATIONS.md` §7/§13; no new mechanism was invented to work around it.
- **`Main.GameUpdateCount % 30` drives the suffocation pulse** rather than a ninth `ai`/`localAI` slot: it is the literal 每30帧 the design names, it is identical on every side, and it keeps the class's eight slots for the state, the variant, the prey/toad indices and the four timers.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] `NPC.rare` does not exist — corrected to `NPC.rarity`**
- **Found during:** Task 1 (`WaterStrider.SetDefaults`), repeated in Task 2 for the other two creatures
- **Issue:** The plan repeats `NPC.rare = ItemRarityID.White;` for all three rows. `NPC` exposes no `rare` member in this tML build (the engine's only NPC rarity field is `NPC.rarity`), so the prescribed line is a compile break.
- **Fix:** Wrote `NPC.rarity = ItemRarityID.White;` with the design's empty 稀有度 cell named in the comment. The correction is already recorded in `04-DEVIATIONS.md` §10 (plans 03-01 and 04-01 made the same one), so no ledger edit was needed.
- **Files modified:** `WaterStrider.cs`, `ToxicToad.cs`, `GlowSalamander.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 with 0 warnings and 0 errors.
- **Committed in:** `23eb94c88` (Task 1) and `5493aed98` (Task 2)

### Documented without a code change

**2. [Plan wording] The plan's per-projectile `NPC.GetSource_FromAI` acceptance token is not expressible inside a `ModProjectile`**
- **Found during:** Task 2 (acceptance audit of the two projectile files)
- **Issue:** Task 2's acceptance criteria require `NPC.GetSource_FromAI` **in each projectile file**, but neither `Projectile` nor `ModProjectile` exposes an `NPC` member to call it on, so the only ways to put the literal token there would be a comment-only mention or a fake receiver — both of which would be misleading rather than satisfying.
- **Fix:** No token was faked. The source is applied at the real spawn site, exactly where the plan's own `ToxicToad` `<action>` places it: `ToxicToad.cs` calls `Projectile.NewProjectile(NPC.GetSource_FromAI(), ...)` for both the bubble and the death cloud. `04-RESEARCH.md` §Verification and `04-DEVIATIONS.md` §7 read the requirement the same way ("spawned from `NPC.GetSource_FromAI()` inside `Main.netMode != NetmodeID.MultiplayerClient`"), so the behaviour is satisfied and only the token's location differs.
- **Files modified:** none (recorded here and as a `WINDOWS.md` entry)
- **Verification:** `grep NPC.GetSource_FromAI` finds the two spawn calls in `ToxicToad.cs`; the Release build proves both projectile types resolve.
- **Committed in:** `5493aed98` (Task 2)

**3. [Design residual, already blocked] Cross-creature hostility is target preference only**
- **Found during:** Task 2 (the toad's preference rule and the salamander's retreat rule)
- **Issue:** 会主动攻击蝾螈与水蛞蝓 / 主动远离蟾蜍 read as aggression between creatures, but tML offers no NPC-versus-NPC damage hook and hostile projectiles damage players only, so a toad can aim at a salamander without ever hurting it.
- **Fix:** Implemented the part the engine supports (the synced preference and the retreat) and left the damage path unmet. This is the residual already recorded in `04-DEVIATIONS.md` §7 and §13 under "Cross-creature hostility"; no new mechanism was invented.
- **Files modified:** none (recorded here and as a `WINDOWS.md` entry)
- **Verification:** `AI()` selects prey through `ModContent.NPCType<...>` indices and the bubble is a standard hostile projectile.
- **Committed in:** `5493aed98` (Task 2)

---

**Total deviations:** 1 auto-fixed (Rule 1) + 2 documented-without-code-change (one plan-wording mismatch, one already-blocked design residual)
**Impact on plan:** No scope creep. The Rule 1 fix was required for the build to pass; the other two are recorded so a reader sees the reasoning rather than an unexplained divergence. Nothing was moved out of scope.

## Issues Encountered

- **Console encoding noise.** `git diff` and PowerShell output render CJK as mojibake in this PS 5.1 session. It is a display artefact only: the bytes on disk are correct UTF-8 without BOM and CR-free LF, which the phase gate's own invariant 13 and the AGENTS.md byte-level check both confirm (`UTF-8 BOM check passed (1402 files).`).
- **A probe entry was accidentally appended to `.planning/WINDOWS.md`** while verifying the ledger verb's arguments. It was immediately marked `fixed` with an explanatory reason (`windows fixed 27`), so it cannot block `/gsd-ship`; the ledger now reads 30 open / 32 total and the five real 04-02 entries sit above it. Recorded here rather than silently rewritten, because the ledger is a history.

## Known Stubs

- **`GlowSalamander`'s three-colour variant slot is inert.** `VariantIndex` is drawn at spawn, synced and read by the class's glow, but with `Commons.ModAsset.White_Mod` there is no sprite to tint, so nothing on screen changes with the variant. This is **intentional and documented** (class comment, the matrix's artwork blocker, OQ2/D-48/D-49): the slot exists so the D-49 migration maps the 灰蓝色 / 粉色 / 褐色 sheets onto frames without a class rework. Tracked as a `WINDOWS.md` `stub` entry for the ship gate.
- **The empty `ModifyNPCLoot` bodies** of `WaterStrider` (no drop in the design row), `ToxicToad` and `GlowSalamander` (毒腺/牛黄 absent) are the design's own empty tables and the D-58 contract, not unwired data: they carry the precise blocker in the comment and in `04-DEVIATIONS.md` §6.

## Defect Ledger

Five entries were appended to `.planning/WINDOWS.md` for cross-phase visibility (they block `/gsd-ship` while open, by design):

- **#28 `unrun-verify`** — the D-21 runtime bundle for all three creatures and both projectiles (spawn isolation, the three water predicates, the dash cadence, the moisture/suffocation cycle, the 75/25 split and the death cloud) is not executed; plan 04-09 records it in `04-UAT.md`.
- **#29 `deviation`** — the `NPC.rare` → `NPC.rarity` Rule 1 correction.
- **#30 `deviation`** — the per-projectile `NPC.GetSource_FromAI` acceptance token is realised at the spawn site in `ToxicToad.cs` (a `ModProjectile` has no `NPC` member).
- **#31 `deviation`** — cross-creature hostility is target preference only; the engine has no NPC-versus-NPC damage path (already in `04-DEVIATIONS.md` §7/§13).
- **#32 `stub`** — the `GlowSalamander` colour variant is inert until approved art arrives (intentional, OQ2).

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- **Waves 3–4 have what they need:** the three `KelpCurtainSpawnConditions` predicates now each have a working consumer, so `IsWaterBottom` (blueprint for 放射虫 / 大型覆藻章鱼) and the plain `spawnInfo.Water` water family (装甲虾 / 爆弹水母 / 帆鳍鳢) inherit a proven shape. `OK: guarded classes = 6` is the progress counter; the phase closes at 37.
- **Outstanding, all recorded, none hidden:**
  - the D-21 runtime bundle (`04-UAT.md`, plan 04-09): every spawn predicate on a real lake, the 水黾 dash cadence and its two recoveries, the 剧毒蟾蜍 preference/bubble/death-cloud kit, the 幽光蝾螈 moisture cycle, the clean-load check for art-missing classes and a dedicated-server run;
  - **approved art** for these three sprites (`WaterStrider.png`, `ToxicToad.png`, `GlowSalamander.png`) and the two projectile sprites — blocker only, no placeholder art created (D-48/D-51);
  - the 亡碧湖 / 森雨幽谷 / 刺苔庭园 region-level spawn predicates (Phases 5–6, D-52);
  - localization (D-20), the absent drop materials 毒腺 / 牛黄 (D-58), and the cross-creature hostility residual (§7/§13);
  - the 水黾 water-surface tuning note (`04-DEVIATIONS.md` §5) if the dry-tile-above test proves too strict in the client run.
- **Plan 04-09 owns §14** of the ledger and the flip of the remaining twenty in-scope rows to `code_complete: true`. `REQUIREMENTS.md` stays Pending until then.

---

*Phase: 04-remaining-ordinary-monsters*
*Completed: 2026-09-16*

## Self-Check: PASSED

- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonBubble.cs`
- FOUND: `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonCloud.cs`
- FOUND commit: `23eb94c88` (Task 1)
- FOUND commit: `5493aed98` (Task 2)
- FOUND commit: `a6f0a0df7` (`plan_head_before`)
