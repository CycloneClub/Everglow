---
status: not-executed
phase: 04-remaining-ordinary-monsters
source: [04-DEVIATIONS.md sections 11 and 14, 03-BIOLOGY.json]
started: null
updated: 2026-09-16
---

# Phase 4 UAT — Remaining Ordinary Monsters (D-21 client bundle)

**This bundle is recorded, not executed.** No live tModLoader client session is part of Phase 4 (D-21), so every entry below is **not executed** and the 21-row tranche is not presented as client-verified. The offline gates and the Release build prove that the 26 `ModNPC` classes and the eleven hostile projectiles compile, resolve their `Commons.ModAsset.White_Mod` fallback, stay inside the subworld spawn boundary, reference only implemented item types and contain no added binary asset; they cannot observe spawn placement, spawn isolation, AI feel, drop acquisition, dedicated-server behaviour or localization fallback. Those are exactly the checks this bundle carries.

`result: not-executed` means the check has not been run — the same state `02-UAT.md` recorded as `blocked` on its runtime environment and `03-UAT.md` recorded as `not-executed`. `blocked_by: runtime-environment` is the reason in every entry.

**Why this bundle matters more than a formality (T-04-55/T-04-58).** Every in-scope row keeps `status: "unchecked"` in `03-BIOLOGY.json` until Phase 8 synchronizes the design source, so this file is the *only* record of what remains unobserved. The two edge cases the offline chain cannot reach are named explicitly below: 大型荆棘苔龟 losing its target mid-flight, and a melee swing landing while its shell is closed.

## Current Test

[not started — no client session scheduled; all 24 checks outstanding (D-21)]

## The 21 rows at close-out

| # | Row id | Class (`internal_name`) | Region | Designed band |
| --- | --- | --- | --- | --- |
| 1 | `bio-death-jade-lake-fluorescent-hydra` | `FluorescentHydra` (D-45 shell) | Death Jade Lake | water |
| 2 | `bio-death-jade-lake-giant-tiger-shrimp` | `GiantTigerShrimp` (D-45 shell) | Death Jade Lake | water |
| 3 | `bio-death-jade-lake-water-strider` | `WaterStrider` | Death Jade Lake | water surface |
| 4 | `bio-death-jade-lake-toxic-toad` | `ToxicToad` | Death Jade Lake | dry land |
| 5 | `bio-death-jade-lake-glow-salamander` | `GlowSalamander` | Death Jade Lake | underwater |
| 6 | `bio-death-jade-lake-armored-shrimp` | `ArmoredShrimp` | Death Jade Lake | dry land / land drift |
| 7 | `bio-death-jade-lake-bomb-jellyfish` | `BombJellyfish` + `LargeBombJellyfish` | Death Jade Lake | underwater |
| 8 | `bio-death-jade-lake-sailfin-snakehead` | `SailfinSnakehead` | Death Jade Lake | underwater |
| 9 | `bio-death-jade-lake-radiolarian` | `Radiolarian` | Death Jade Lake | shallow water |
| 10 | `bio-death-jade-lake-algae-octopus` | `AlgaeOctopus` | Death Jade Lake | underwater |
| 11 | `bio-death-jade-lake-large-algae-octopus` | `LargeAlgaeOctopus` (+ `AlgaeOctopus_InkCloud`) | Death Jade Lake | water bottom |
| 12 | `bio-death-jade-lake-jade-anglerfish` | `JadeSpiritAnglerfish` | Death Jade Lake | water bottom |
| 13 | `bio-death-jade-lake-cannon-barnacle` | `CannonBarnacle` (D-45 shell) | Death Jade Lake | water |
| 14 | `bio-spiny-moss-court-withered-soldier` | `AnimatedWitherbarkSoldier` + `AnimatedWitherbarkSoldierRanged` + `AnimatedWitherbarkSoldierSpell` + `AnimatedWitherbarkHound` | Spiny Moss Court | dry land |
| 15 | `bio-spiny-moss-court-court-commander` | `CourtCommander` | Spiny Moss Court | dry land |
| 16 | `bio-spiny-moss-court-brodie-flydragon` | `BrodieFlydragon` + `SmallBrodieFlydragon` | Spiny Moss Court | open air |
| 17 | `bio-valley-of-lush-and-moist-red-needle-caterpillar` | `RedNeedleCaterpillar` | Valley of Lush and Moist | land band |
| 18 | `bio-valley-of-lush-and-moist-assassin-raspberry` | `AssassinRaspberry` | Valley of Lush and Moist | land band |
| 19 | `bio-valley-of-lush-and-moist-serpent-moss` | `SerpentMoss` | Valley of Lush and Moist | land band |
| 20 | `bio-valley-of-lush-and-moist-small-guppy-conch` | `SmallGuppyConch` | Valley of Lush and Moist | land band |
| 21 | `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle` | `LargeMossyThornTurtle` | Valley of Lush and Moist | land band |

## Tests

### 1. 荧光水螅 (`FluorescentHydra`) — D-45 identity shell

expected: **spawn** the creature appears in the Kelp Curtain layer's water band inside the Yggdrasil Subworld (its `SpawnChance` gates on `SubworldSystem.IsActive<YggdrasilWorld>()`, `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` and `spawnInfo.Water`) and never in an ordinary world. **behaviour** it is an identity shell: it loads, registers, faces the player and does nothing else (vanilla `aiStyle 0`, no `AI()` body, no attack hook, no loot override) — the design section is heading-only and the row carries the `behavior undefined in the design row (D-29/D-45 shell)` blocker. **combat** contact damage and defence read the D-54 defaults recorded in `04-DEVIATIONS.md` §10.1. **drops** none (the design names none, and no drop table is written). **load** the class resolves `Commons.ModAsset.White_Mod` with no missing-texture abort.

result: not-executed
blocked_by: runtime-environment
reason: "Spawn placement, the clean `White_Mod` fallback load and the shell's inertness are runtime loader/spawn properties; no offline gate observes them (D-21)."

### 2. 巨型虎虾 (`GiantTigerShrimp`) — D-45 identity shell

expected: same shape as check 1 in the same water band; its own D-54 defaults (hit box `56x28`, life 90, damage 25, defence 6, `value = 0`, `catchItem = 0`) apply, and it too carries the behaviour-undefined blocker rather than invented behaviour.

result: not-executed
blocked_by: runtime-environment
reason: "As check 1: runtime loader and spawn properties (D-21)."

### 3. 水黾 (`WaterStrider`)

expected: **spawn** only on the Kelp Curtain layer's water surface inside Yggdrasil (the shared `IsWaterSurface` predicate: `spawnInfo.Water` plus a dry tile directly above the spawn tile), never in an ordinary world and never in mid-water or on dry land. **behaviour** the design's dash cadence (60–200 frames between dashes, 45–150 frames of dash) reads as short repeated bursts rather than a continuous glide; the creature recovers from land by hopping back toward water and from below the surface by swimming up, and it applies 潮湿 (`BuffID.Wet`) rather than attacking. **combat** contact damage only, with the documented D-54 values. **drops** none. **tuning question to answer here:** whether the derived surface predicate selects the intended tiles on a real lake — if it proves too strict, `DeathJadeLakeBiome.LiquidSurfaceY` is the recorded secondary cross-check (`04-DEVIATIONS.md` §5, and the `effect:` element on this row).

result: not-executed
blocked_by: runtime-environment
reason: "The derived surface predicate's selectivity, the dash cadence and the land/water recovery are runtime properties (D-21, T-04-14)."

### 4. 剧毒蟾蜍 (`ToxicToad`)

expected: **spawn** on dry land in the Kelp Curtain layer inside Yggdrasil (the shared `IsDryLand` predicate), never in water and never in an ordinary world; **behaviour** it prefers a nearby 幽光蝾螈 (`GlowSalamander`) or 水蛞蝓 (`RiverSlug`) over the player, fires `ToxicToad_PoisonBubble`, and its death spawns `ToxicToad_PoisonCloud` (a 180-frame, 10-damage Venom cloud); **combat** contact splits 75% Poisoned / 25% Venom and the bubble carries the design's acidic-venom stand-in (`BuffID.Venom`, the recorded §10 substitution for the absent `BuffID.AcidVenom`); **drops** none — 毒腺 and 牛黄 are absent from the repository and deliberately not dropped (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "Prey preference, the bubble and death cloud, the 75/25 split and the surviving-cloud timing are runtime gameplay properties (D-21)."

### 5. 幽光蝾螈 (`GlowSalamander`)

expected: **spawn** underwater in the Kelp Curtain layer inside Yggdrasil, never in an ordinary world; **behaviour** the moisture cycle runs exactly 3600 / 600 / 30 frames (dry out, retreat to water, 30-frame `BuffID.Suffocation` pulses), the crawl-back-to-water search finds liquid within its bounded scan, and the creature retreats from a nearby 剧毒蟾蜍; the synced colour variant (灰蓝色 / 粉色 / 褐色) is drawn once on spawn and is inert while the art is missing (`Commons.ModAsset.White_Mod`); **combat** contact and the documented debuffs behave as recorded; **drops** none — 毒腺 and 牛黄 are absent and deliberately not dropped (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The 60 s / 10 s / 30-frame cycle, whether this build turns `BuffID.Suffocation` into visible pressure, and the avoidance reading are runtime properties (D-21)."

### 6. 装甲虾 (`ArmoredShrimp`)

expected: **spawn** on the Kelp Curtain layer's dry land / land drift inside Yggdrasil at the land weight, never in an ordinary world and never in water; **behaviour** `OnSpawn` rolls the design's 2–5 group and every member created through `NPC.NewNPC` carries the synced `ai1` follower marker, so the shoal **does not cascade** (no member rolls a second group even if `ai0..ai3` are applied in another order — the static `groupCreationDepth` guard) and the members drift together instead of scattering; the creature is passive (damage 0, no target acquisition); **combat** no attack; **drops** none — 软体甲壳碎片 is absent (D-58); **capture** no catch item (`NPC.catchItem = 0`, no catchable flag, no critter-count entry).

result: not-executed
blocked_by: runtime-environment
reason: "Group-spawn ordering, the no-cascade property and the drift are runtime properties (D-21, T-04-17)."

### 7. 爆弹水母 (`BombJellyfish` + `LargeBombJellyfish`)

expected: **spawn** underwater in their designed shallow / deep bands inside Yggdrasil — the 小/大 pair reads the local bounded liquid-column probe with opposite sides and a 6-tile threshold — never in an ordinary world; the 森雨幽谷 half of the small-only condition cannot fire yet (the D-52 region gap); **behaviour** both hover in place (horizontal velocity damped to zero, a two-leg rising/sinking bob) and drift nowhere, and each detonates exactly once on death through `BombJellyfish_Explosion`; **combat** the death blast is 30 (小) / 50 (大) with the radius derived from the value the dying creature passes, applied once and never duplicated per client; **drops** none; **capture** no catch item on either size (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "Band placement, the hover read and the single detonation per death are runtime properties (D-21)."

### 8. 帆鳍鳢 (`SailfinSnakehead`)

expected: **spawn** underwater in the Kelp Curtain layer inside Yggdrasil, never in an ordinary world; **behaviour** it cruises at a constant speed with no target (中立) until it is actually damaged, after which `HitEffect` opens the bounded 300-frame aggro window; the charge lasts the recorded bounded window, the 50-tile leash stops it being dragged out of its lake, the 40-frame charge cooldown recovers it, and a further hit refreshes the window; it retreats from a nearby 幽光蝾螈 and rams a nearby 水蛞蝓 (the unmodelled inter-creature contact damage stays the §7/§13 record); **combat** the charge and contact damage follow the design values; **drops** none — 亡碧膏 is absent (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The neutral-until-provoked reading and the charge window are runtime gameplay properties (D-21, T-04-22)."

### 9. 放射虫 (`Radiolarian`)

expected: **spawn** only in the Kelp Curtain's shallow water inside Yggdrasil (the bounded upward liquid-column probe) at the lowest aquatic weight, never in an ordinary world and never at the lake bottom; **behaviour** while submerged it fires `Radiolarian_WaterBolt` on its fixed cadence, and when its target comes inside four tiles it commits the designed dash on the 300-frame cooldown and then retreats and resumes fire; the dash trigger is the compile-time squared-pixel constant, so it must fire at the intended distance and must not read any camera value; **combat** the bolt carries the design's 35 ranged damage while the melee 45 stays on contact (re-asserted in `PostAI`); **drops** `RadialCarapace` at 6.7% (denominator 15), quantity 1, and no absent material drops.

result: not-executed
blocked_by: runtime-environment
reason: "Band selectivity, the fire cadence, the dash trigger distance and drop acquisition are runtime properties (D-21)."

### 10. 覆藻章鱼 (`AlgaeOctopus`)

expected: **spawn** underwater in the Kelp Curtain layer inside Yggdrasil, never in an ordinary world (the 森雨幽谷 sub-biome predicate does not exist yet — the row's `region:` blocker); **behaviour** it fades in only inside its 24-tile observation range (a linear `NPC.alpha` over the distance between the synced creature and target centres), dashes leaving ink puffs every 10 frames, and bursts one more on death; the disguise/ink **lingering rendering** is the recorded effect blocker while its damage is built; **combat** contact applies 缓慢 (`BuffID.Slow`) and the ink applies 黑暗 (`BuffID.Darkness`) for its documented duration, with max-alpha confirming it is genuinely invisible at range and not merely dark; **drops** none — 软体甲壳碎片 and 亡碧膏 are absent (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The stealth alpha curve, the ink trail and the dies-release case (`WINDOWS.md` entry 61) are runtime presentation properties (D-21)."

### 11. 大型覆藻章鱼 (`LargeAlgaeOctopus`)

expected: **spawn** only at the water bottom inside Yggdrasil (the shared `IsWaterBottom` predicate plus its floor probe), never in an ordinary world; **behaviour** the grab applies 束缚 (`BuffID.Webbed`) **and** 窒息 (`BuffID.Suffocation`) for 120 ticks and re-applies them every 60 frames — **timed only**, so a player is never left permanently held even if the creature dies mid-grab (verify the release case explicitly); the 180-frame wave emits four `AlgaeOctopus_InkCloud` instances (the first aimed at the prey, three random) and not twelve; the dash is fast and the low-life flee triggers only against a player, inking behind it; **combat** contact and ink damage match the design (60 for the large creature's ink); **drops** none — 软体甲壳碎片 (2~4), 亡碧膏 (3~6) and the 武器与饰品掉落待定 list are absent (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The grab's movement agency, the wave geometry, the flee threshold and the debuff release after death are runtime properties (D-21, T-04-27)."

### 12. 碧灵鮟鱇 (`JadeSpiritAnglerfish`)

expected: **spawn** only at the water bottom inside Yggdrasil at the rare bottom weight, never in an ordinary world; **behaviour** it is invisible beyond its reveal range (26 tiles, the recorded D-54 default), its lamp gives it away inside that range, and the first dash after a reveal carries the design's 60 damage before it settles back to the floor band; the inter-creature hostility clause is unmodelled (§7/§13); **combat** the reveal dash damage split (60 dashing / `defDamage` otherwise, re-asserted in `PostAI`) behaves as documented; **drops** `MeatLantern` at 25% (denominator 4) and `Photophore` at 12.5% (denominator 8), quantity 1 each.

result: not-executed
blocked_by: runtime-environment
reason: "The stealth lamp, the reveal-dash feel, the damage split and the two drop rates are runtime properties (D-21)."

### 13. 炮弹藤壶 (`CannonBarnacle`) — D-45 identity shell

expected: same shape as checks 1 and 2 in the water band, with its own D-54 defaults (hit box `28x24`, life 50, damage 15, defence 10, `value = 0`, `catchItem = 0`); it loads with the `White_Mod` fallback and carries the behaviour-undefined blocker rather than a fabricated cannon attack.

result: not-executed
blocked_by: runtime-environment
reason: "As checks 1 and 2: runtime loader and spawn properties (D-21)."

### 14. 枯木活化士兵 (`AnimatedWitherbarkSoldier` + `AnimatedWitherbarkSoldierRanged` + `AnimatedWitherbarkSoldierSpell` + `AnimatedWitherbarkHound`)

expected: **spawn** on dry land in the Spiny Moss Court inside Yggdrasil, never in an ordinary world, with four sibling classes present and **no** stat mutation outside `SetDefaults` (life 80 / 60 / 55 / 75); **behaviour** all four read 中立 until a non-lethal hit or a player inside the conservative provoke range, then return to 中立 past the leash range; the melee variant walks, jumps at walls/ledges and deals contact damage; the ranged variant performs the 120-frame boulder pickup and then the 8-tile / 200-frame spacing before its next `AnimatedWitherbarkSoldier_Boulder`; the spell variant casts its three-beam `AnimatedWitherbarkSoldier_SpellBeam` on a fixed interval and then relocates to a standable tile near the prey, never into terrain; the hound does its back-and-forth charge; **combat** contact and projectile damage follow the four stat rows, and the design's 意志高涨 morale bonus is **not** applied (the D-46 blocker — a soldier must not fight as though buffed); **drops** `ActivatedDogStaff` at 9% (denominator 11) on the **犬 variant only**, and nothing from the other three; 枯木碎块 and the morale-gated 干涸心脏 are absent (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The neutral-until-provoked reading, the four attack patterns, the 120/200-frame budgets and the single-variant drop are runtime properties (D-21, T-04-38)."

### 15. 王庭号令者 (`CourtCommander`)

expected: **spawn** on dry land in the Spiny Moss Court inside Yggdrasil, never in an ordinary world; **behaviour** the staff-raise tell plays, and the summon fires on `OnSpawn` and on the first staff use of each aggro entry **only once per aggro entry**, only while no soldier stands within its search radius, capped at the design's 1–3 randomly chosen variants, and created server-side so each client sees the same soldiers and none are duplicated; 10-tile spacing is respected; **combat** contact damage and defence follow the design row; the 意志高涨 buff and the summoned-soldier morale bonus are **not** applied (the two D-46 blockers); **drops** none — 枯木碎块 and 干涸心脏 are absent (D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The once-per-aggro summon, the no-duplicate property and the spacing are runtime properties (D-21, T-04-37)."

### 16. 布罗迪蝇蜓 (`BrodieFlydragon` + `SmallBrodieFlydragon`)

expected: **spawn** in the open air of the Spiny Moss Court / 森雨幽谷 inside Yggdrasil, never in an ordinary world; both sizes appear at their own natural weights (the standard at 1f, the small at 0.5f) and the 小 variant is **not** spawned by an egg system, because no egg NPC, projectile or class exists (the D-46 blocker); **behaviour** both sizes fly and melee with their own stat rows (40 / 20 life) and their 中毒 contact clause; **combat** damage and the poison application follow the two stat rows, and `NPC.value` reads 20 copper (standard) / 0 (small) per the design snapshot's own table; **drops** none.

result: not-executed
blocked_by: runtime-environment
reason: "The two sizes' relative spawn rates, the flight feel and the poison reading are runtime properties (D-21)."

### 17. 红针洋辣子 (`RedNeedleCaterpillar`)

expected: **spawn** on the Valley of Lush and Moist land band inside Yggdrasil, never in an ordinary world (its Valley region split is the recorded §5 gap and needs no row element); **behaviour** it crawls as a segmented worm and, from four tiles away, fires the design's 4–6 needle volley from its head every 180 frames; contact and needles apply the 25% / 20 s and 37.5% / 10 s 中毒 (`BuffID.Poisoned`) split; the class owns a `!Main.dedServ`-guarded `HitEffect` because the reused `Caterpillar` template's own dust is unguarded; **combat** the volley and contact damage follow the design values and no dust runs on the server; **drops** `CaterpillarJuice` guaranteed, quantity 1–2 (the OQ1 cross-namespace drop).

result: not-executed
blocked_by: runtime-environment
reason: "The volley cadence, the poison split and the template-derived dust guard are runtime properties (D-21, T-04-41/T-04-46)."

### 18. 阿萨辛覆盘子 (`AssassinRaspberry`)

expected: **spawn** on the Valley land band inside Yggdrasil, never in an ordinary world; **behaviour** it is a stationary ambusher: retracted beyond 8 tiles, extended inside 8, and silent **but still extended** below 4 tiles; the ground-disguise presentation is the recorded D-46 visual blocker, so the trigger, window and scatter must be judged from the attack itself rather than from a disguise sprite; the retract path restores the creature to its captured anchor position; **combat** the defence switches 20 (passive) → 4 (attacking) on entry to the attack state and back on retract, and no exit path can leave it stuck at 4; the ground spike (`AssassinRaspberry_Spike`) leaves at the creature's feet with the lift-then-gravity arc; **drops** none.

result: not-executed
blocked_by: runtime-environment
reason: "The 4–8 tile window, the defence switch and the anchor restore are runtime properties (D-21, T-04-44)."

### 19. 蛇行苔 (`SerpentMoss`)

expected: **spawn** on the Valley land band inside Yggdrasil, never in an ordinary world; **behaviour** it binds a player within the design's 2 tiles on the recorded 60-frame cadence, holding the bound player's index and its own timer in named `NPC.localAI[]` slots (**never on a `Player`**), and the 环境植物 disguise is the recorded D-46 visual blocker; **combat** the bind applies 束缚 (`BuffID.Webbed`) and the 33% roll (`PoisonChanceDenominator = 3`) behaves as a 1-in-3 chance on the documented hits, with every write under the netmode guard; **drops** none.

result: not-executed
blocked_by: runtime-environment
reason: "The bind cadence, the 1-in-3 roll and the absence of any player-side timer are runtime properties (D-21)."

### 20. 小格普螺 (`SmallGuppyConch`)

expected: **spawn** on the Valley land band inside Yggdrasil (the shared `IsDryLand` predicate plus the explicit `spawnInfo.Water` rejection), never in an ordinary world and never in water; **behaviour** it crawls slowly, reverses at a wall or ledge, alternates 爬行 / 静止 on its local timer and **never acquires a target** (verify it does not retaliate when attacked); **combat** contact damage only, with the design's 5% damage reduction visibly applied; **drops** none; **capture** no catch item (`NPC.catchItem = 0`, no catchable flag, no critter-count entry — the 可以被捕获 clause has no repository item, D-58).

result: not-executed
blocked_by: runtime-environment
reason: "The crawl/rest cadence, the no-retaliation property and the damage reduction are runtime properties (D-21)."

### 21. 大型荆棘苔龟 (`LargeMossyThornTurtle`) — the mini boss, four states

expected: **spawn** on the Valley land band inside Yggdrasil at the very rare mini-boss weight (0.1f), never in an ordinary world and never in the ordinary Thorn Mossy Tortoise's band; **behaviour** it crawls at 1 tile/s without phasing through blocks and never turns hostile above 900 life; damaged at or below it, the full four-state cycle runs with the design's exact budgets:

- **state 1 缩壳** — exits after 100 frames without being hit or 320 frames in the shell;
- **state 2 伸出头 + 震击地板** — a floor slam emitting `LargeMossyThornTurtle_Shockwave` (width 40 → 320), repeated once after 120 frames, then 120 more frames to state 3;
- **state 3 飞天下坠** — the shell flight ignores tile collision, lands with one more slam, then drops three evenly spaced `LargeMossyThornTurtle_Boulder` from 30 world tiles above (spread ±10 tiles, falling at 6), then goes to state 4;
- **state 4 爬行 240 帧** — crawls away from the player at 2 tiles/s for 240 frames, then back to state 1 facing away.

  **Two cases this bundle exists for (T-04-58).** (a) **Target lost mid-flight**: kill or teleport the attacker away during state 3 — the creature must still land, slam, drop its boulders and reach state 4, and defence must return to 10, never stuck at 999 (the server probes three columns and `FlightMaxFrames = 180` guarantees a landing, so no soft-lock is possible). (b) **Melee reflect while retracted**: swing a sword at the closed shell — the reflect must apply **exactly once**, clamped to 2–20, and must not apply while the shell is open or in the airborne state, and it must not double-apply in multiplayer.
  **combat** defence is 999 while retracted / airborne and 10 otherwise, with **no** third value reachable; the 减伤 20 (`FinalDamage *= 0.8f`) applies in every state; the rotated-contact escalation reads 85 during the airborne state (re-asserted in `PostAI` from the synced state); **drops** none in this phase — the design's 死亡掉落物暂定 list is TBD and absent (D-58), and `ThornTurtleShell` is Phase 3's creature, not this one.

result: not-executed
blocked_by: runtime-environment
reason: "The state machine's frame budgets, the landing guarantee, the reflect-once property and the defence return are runtime properties the offline gate cannot reach (D-21, T-04-53)."

### 22. Isolation — none of the 21 appears in an ordinary world (BIO-06)

expected: After several minutes of idling in a normal (non-subworld) world at the same layer depth, **none** of the 21 in-scope creatures spawns, and none of them spawns outside its designed band inside Yggdrasil either. Every `SpawnChance` requires `SubworldSystem.IsActive<YggdrasilWorld>()` **and** the server-safe `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`, and `NPCSpawnManager.EditSpawnPool` returns early outside the subworld, so the isolation is a two-layer guarantee that only a live run can confirm. The Phase 4 gate asserts the tokens statically, not the behaviour.

result: not-executed
blocked_by: runtime-environment
reason: "Main-world leakage is a runtime spawn property (BIO-06); the gate asserts the isolation tokens statically only."

### 23. Dedicated server and multiplayer (QUAL-03)

expected: A dedicated-server launch runs the whole tranche with no client-only crash and no graphics work on the server: the eleven hostile projectiles emit no dust or VFX there (every such call sits inside `!Main.dedServ`), every authoritative spawn appears **once** rather than once per client (each is created inside a `Main.netMode != NetmodeID.MultiplayerClient` guard with `NPC.netUpdate`), no `Main.LocalPlayer` dependency exists anywhere, and the mini boss's landing probe runs server-side. In a multiplayer client: the tranche spawns and fights normally, the server-safe spawn band matches the camera-driven `IsBiomeActive` band, the 大型覆藻章鱼 grab's two debuffs appear on the victim and expire, the 大型荆棘苔龟 reflect applies exactly once, no NPC or projectile is duplicated, and no state desync appears in the 装甲虾 shoal or the 王庭号令者 summon.

result: not-executed
blocked_by: runtime-environment
reason: "Dedicated-server safety and multiplayer behaviour are runtime properties; only the static guards are gated offline (D-21, QUAL-03)."

### 24. Localization fallback (D-20)

expected: All 26 classes load with their display names falling back cleanly, the tML log shows **no** missing-resource or disabled-mod entry for any of them (the art-missing classes resolve `Commons.ModAsset.White_Mod`), and no localization key has been fabricated. Localization is deferred by user directive (D-20) and is expected to read as a fallback name, not as an error. The canonical tail `localization deferred (D-20); runtime verification outstanding (D-21)` on every in-scope row is the machine-facing half of this same obligation.

result: not-executed
blocked_by: runtime-environment
reason: "The exporter was not run and no key was written (D-20); the fallback appearance is a runtime display property."

## Summary

total: 24
passed: 0
failed: 0
not_executed: 24
skipped: 0
blocked: 0

## Gaps

<!-- None. Every entry is a runtime-environment deferral (D-21), not a code issue, so nothing is added to Gaps. The 大型荆棘苔龟 state machine and reflect cases (check 21) and the item-scope drops are already recorded as blockers in 03-BIOLOGY.json and 04-DEVIATIONS.md section 14.1; they are not new gaps discovered here. -->

## Deferred Follow-Ups

- test: 1
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 2
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 3
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 4
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 5
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 6
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 7
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 8
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 9
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 10
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 11
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 12
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 13
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 14
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 15
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 16
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 17
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 18
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 19
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 20
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 21
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 22
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 23
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
- test: 24
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-16
