# Phase 3 Deviation Ledger — Completed-Art Ordinary Monsters

Generated: 2026-09-15
Opened by plan 03-01; extended by plans 03-02 and 03-03 and consolidated by plan 03-04.
Machine source of truth: `03-BIOLOGY.json` (per-row `status`, `blockers`, `deferred`, `deferred_reason`, plus the `assumptions[]` and `deviations[]` arrays), mirrored by `03-BIOLOGY.md`. The consolidated blocker register in §10 is generated mechanically from that JSON, so its **Exact blocker text** cells cannot drift from the matrix, and gate invariant 11 re-asserts the row-id set and the per-row `status` on every run.

This is the deviation and blocker ledger for the Phase 3 repository-art ordinary-creature tranche (D-28 full implementations) and for the frozen Phase 3–4 biology matrix (D-24). It is the audit trail for D-25 (committed snapshot only), D-34 (conservative defaults), D-37/D-38/D-39 (a drop rule references only implemented items), D-41/D-42 (the tranche rule) and D-43 (the drop-availability correction). No field below was changed by editing the Feishu design source, no localization key was created or renamed, and no `.png` or other binary/art asset was created or modified.

**Coverage claim.** This ledger covers all five Phase 3 tranche rows — `bio-valley-of-lush-and-moist-mossy-thorn-turtle` (荆棘苔龟), `bio-valley-of-lush-and-moist-guppy-conch` (格普螺), `bio-death-jade-lake-verdant-rods` (叶飞棍), `bio-spiny-moss-court-giant-tree-man` (巨树人) and `bio-death-jade-lake-river-slug` (水蛞蝓) — and every blocker they carry. Each row appears in §1 (tranche and asset allocation), §5 (drop disposition), §7 (localization state), §8 (runtime checks) and §10 (its exact blocker rows). A tranche row absent from any of those sections is a defect.

**Navigation.** §1 tranche rule and allocation · §2 repository-art vs design-art discrepancy · §3 mapping assumptions · §4 conservative defaults · §5 drop availability · §6 spawn-region gap · §7 localization (D-20) · §8 runtime verification (D-21) · §9 attack projectiles · **§10 consolidated blocker register (mechanical)** · **§11 Phase 3 close-out** · **§12 decision disposition** · **§13 deferred registry**.

## 1. Tranche rule (D-41/D-42, supersedes 03-RESEARCH)

**D-41, verbatim:** "Texture-complete" — the Phase 3/4 split criterion — is **the repository already containing the creature's approved art** (a corresponding `.png` in the KelpCurtain creature asset tree), **not** a design-row checkbox or inline design `<img>`. Research proved `evidence/biology.xml` carries no per-creature texture checkbox (29 tables, 2 with name headers, both Giant Winged Dragon item tables; 22 `<checkbox>` total, 0 on creature rows) and that the only per-creature artwork marker is an inline `<img>` present for just 7 Death Jade Lake creatures.

**D-42, verbatim:** Creatures that have repository art but **no** design artwork marker (e.g. 荆棘苔龟, 格普螺, 叶飞棍) are **in Phase 3**. The repository-art-vs-design-art discrepancy is recorded in `03-DEVIATIONS.md` for later designer confirmation, not treated as out-of-scope.

`03-RESEARCH.md` §Summary Finding 1 and §Open Questions Q1 proposed a **different** rule — "texture-complete" = the creature's design section carries an inline `<img>` — which selected seven Death Jade Lake creatures and no 刺苔庭园 or 森雨幽谷 creature at all (A2 in the research Assumptions Log). CONTEXT D-41 supersedes it. The supersession reached the executor as a `CRITICAL_OVERRIDE` marker inside `03-01-PLAN.md`, not from the research document, so the research's recommendation is retained only as audit trail.

The repository creature art, enumerated from the working tree with `Get-ChildItem -Recurse 'Sources/Modules/Yggdrasil/KelpCurtain/NPCs' -File -Filter '*.png'`, is six git-tracked, approved assets:

| Repository asset | Size | Design row | Region | Phase 3? | Class |
| --- | --- | --- | --- | --- | --- |
| `NPCs/MossyThornTurtle.png` | 84x46 | 荆棘苔龟（Thorn Mossy Tortoise） | Valley of Lush and Moist | yes | `MossyThornTurtle` (plan 03-01) |
| `NPCs/GuppyConch.png` | 114x58 | 格普螺 | Valley of Lush and Moist | yes | `GuppyConch` (plan 03-02) |
| `NPCs/VerdantRods.png` | 54x432 | 叶飞棍 | Death Jade Lake (+ Valley) | yes | `VerdantRods` (plan 03-02) |
| `NPCs/GiantDandelion.png` | 214x263 | 巨树人 | Spiny Moss Court | yes | `GiantDandelion` (plan 03-03) |
| `NPCs/RiverSlug.png` + `RiverSlug.cs` | 36x104 | 水蛞蝓 | Death Jade Lake | yes — already implemented | `RiverSlug` |
| `NPCs/AcroporaSnake.png` | 132x132 | 苍带帘蛇/克莱因蛇 (boss) | out of phase | no — Phase 7 | — |

`RiverSlug.cs` is already-done evidence for 水蛭蝓: the matrix records `code_complete: true` for its row and no task re-implements it. The five `phase:3` rows are exactly `bio-valley-of-lush-and-moist-mossy-thorn-turtle`, `bio-death-jade-lake-verdant-rods`, `bio-spiny-moss-court-giant-tree-man`, `bio-death-jade-lake-river-slug` and `bio-valley-of-lush-and-moist-guppy-conch`. The one boss row that also has repository art is `bio-out-of-phase-kelp-snake` (`AcroporaSnake.png`, Phase 7).

## 2. Repository-art vs design-art discrepancy (D-42)

The design source's own artwork marker is an inline `<img>` inside the creature's section. It is recorded in the matrix as `design_art`, is derived mechanically from the ten enumerated `<img>` elements in nine sections, and is **independent** of `texture_complete`: the two flags disagree on this snapshot for several rows. `texture_complete` stays governed by D-41 (repository art).

| Creature | Repository asset | Design inline `<img>`? | Matrix disposition |
| --- | --- | --- | --- |
| 荆棘苔龟 | `MossyThornTurtle.png` | no | `texture_complete: true`, `design_art: false` — Phase 3 (D-42) |
| 格普螺 | `GuppyConch.png` | no | `texture_complete: true`, `design_art: false` — Phase 3 (D-42) |
| 叶飞棍 | `VerdantRods.png` | no | `texture_complete: true`, `design_art: false` — Phase 3 (D-42) |
| 巨树人 | `GiantDandelion.png` | no | `texture_complete: true`, `design_art: false` — Phase 3 (D-42) |
| 水蛞蝓 | `RiverSlug.png` | yes (36x27) | `texture_complete: true`, `design_art: true` — Phase 3 |
| 苍带帘蛇/克莱因蛇 (boss) | `AcroporaSnake.png` | yes | `texture_complete: true`, `design_art: true` — Phase 7 |

Six further creatures carry an inline design `<img>` but **no** repository texture, so D-41 places them in Phase 4 (`design_art: true`, `texture_complete: false`, `phase: 4`): 水黾, 幽光蝾螈（美西螈）, 装甲虾, 帆鳍鳢, 覆藻章鱼 and 大型覆藻章鱼. A later designer confirmation may move them back into Phase 3.

Two traps that would have corrupted this set are recorded so they are not re-introduced: (a) 吸血魔毯 (`bio-death-jade-lake-vampire-mat`) is a Phase 7 row carrying **no** inline design image and therefore `design_art: false`; (b) 水蛞蝓 (`bio-death-jade-lake-river-slug`) **is** one of the nine design-art rows — its 36x27 `<img>` is easy to miss because the row is already implemented, and an earlier six-name list wrongly omitted it.

## 3. Mapping assumptions

Every repository asset is mapped to a design row by name/description/geometry inference. None of these mappings is a design statement, so each is recorded for designer confirmation rather than treated as settled.

| Repository asset | Design row | Evidence | Status |
| --- | --- | --- | --- |
| `MossyThornTurtle.png` (84x46) | 荆棘苔龟（Thorn Mossy Tortoise） | The design heading itself gives "Thorn Mossy Tortoise"; the sprite is a single mossy, spiked tortoise. | high confidence, still recorded |
| `GuppyConch.png` (114x58) | 格普螺 | The design row describes a green-fleshed giant snail named after the warlock 格普; the asset basename is the only conch asset. | assumed |
| `VerdantRods.png` (54x432) | 叶飞棍 | 432 / 54 = 8, so the asset is an eight-frame rod strip; the design row describes a green leaf-like flying rod ("飞棍", "有叶片状外观"). | assumed |
| `GiantDandelion.png` (214x263) | 巨树人 | The sprite is a dead tree with a pale puff canopy on root legs; the design row describes a giant walking tree-man of 刺苔庭园. | assumed |
| `RiverSlug.png` + `RiverSlug.cs` (36x104) | 水蛞蝓 | `RiverSlugItem.bait = 30` implements the design's 提供渔力; `NPCID.Sets.CountsAsCritter` and `catchItem` implement 可以被捕捉. | high confidence |
| `AcroporaSnake.png` (132x132) | 苍带帘蛇/克莱因蛇 | The existing `KelpSnake`/`AcroporaSpear` asset family ties the `AcroporaSnake` basename to the boss. | assumed |

**Naming rule.** A new creature class takes its class name and internal name from the existing repository asset basename (`MossyThornTurtle.png` -> `MossyThornTurtle`), which keeps tML's default texture resolution working with **no** `Texture` override, **no** handwritten asset path and **no** asset move. A region subfolder was deliberately not used: tML resolves an `ModNPC`'s default texture from its own namespace path, so a class in `NPCs/<Region>/` would request a missing texture and abort mod loading while the approved art sits in `NPCs/`. Creature internal names are compatibility-sensitive once committed (D-28), so they are chosen once and never renamed.

**Class-name verification rule.** `03-BIOLOGY.json` populates a row's `internal_name` **only** once its class file exists on disk. `code_complete == true` therefore implies a non-empty `internal_name` whose `<short name>.cs` file resolves on disk, and `code_complete == false` implies an empty one. The three tranche rows that are still `code_complete: false` with an **empty** `internal_name` after plan 03-01 (`bio-death-jade-lake-verdant-rods`, `bio-valley-of-lush-and-moist-guppy-conch`, `bio-spiny-moss-court-giant-tree-man`) are the plan-frozen record of classes not yet created, not defects to be back-filled; plans 03-02 and 03-03 fill each one in the same task that creates its class. This is what makes the gate's class-resolution invariant jointly satisfiable with the matrix at every point where a plan asserts the gate green.

**English names.** The snapshot carries no English name column; only 荆棘苔龟 has one in its heading. Every other row's `name_en` is an executor-supplied label recorded here as an assumption.

**Plan 03-03 mapping note.** `GiantDandelion.png` (214x263) is mapped to 巨树人 with `mapping_confidence: "assumed"`, not high confidence: the sprite is a dead tree with root legs and a pale puff canopy, which matches the design row's 巨树人 (a dead giant tree man of 刺苔庭园) but is a description inference rather than a design statement. It is the only giant-tree creature asset in the Kelp Curtain creature tree, so no competing candidate exists. Recorded here for designer confirmation.

## 4. Conservative defaults (D-34/D-38)

Where the design is silent or gives a progression rather than one number, a conservative default is used and recorded here. The machine copy of this section is the JSON `assumptions[]` array (one entry per §1–§6 of this ledger), and the JSON `deviations[]` array carries the phase's recorded deviations.

**Plan 03-04 repair note (Rule 1).** Both arrays shipped from the plan-03-01 tracer commit as the six-element placeholder `["e", "e", "e", "e", "e", "e"]` — an automation defect that was present from commit `421f2a09a` onward, invisible to every gate because no invariant reads them. Plan 03-04 restored the machine copy: `assumptions[]` now holds one entry per §1–§6 and `deviations[]` holds the six deviations the phase actually recorded (the two Rule 1 API-name corrections, the plan-03-02 `HitEffect` placement, the D-23 `RiverSlug` acceptance, the `White_Mod` projectile art deferral, the absent-drop dispositions and the plan-03-03 seam staging). No other field of `03-BIOLOGY.json` changed.

| Default | Value | Rows | Reason |
| --- | --- | --- | --- |
| Spawn weight | `1.5f` (band 1f–3f) | 荆棘苔龟 (plans 03-02/03-03 use their own 0.5f–2f band) | The design supplies no weight; the band is calibrated against `NPCSpawnManager`'s 0.1f slime weights (A5). |
| Drop chance denominator | `20` = 5% | 荆棘苔龟 / 荆棘龟壳 | The design gives 5%; `ItemDropRule.Common` takes the denominator. |
| Slash-separated stats | first value | 巨树人 `500/900/1300`, `70/140/180` | The slashes are the difficulty progression; the normal-state value is the first. |
| Sprite-derived extents | `NPC.width` / `NPC.height` from the texture | all tranche rows | The design supplies no hitbox; the approved sprite is the only measurement available. |
| `frame_count` | `1` for 荆棘苔龟 (84x46 is not an integer `textureHeight / textureWidth` multiple); `4` for `RiverSlug` (`Main.npcFrameCount = 4`) | 荆棘苔龟, RiverSlug | Derived from the tracked asset, not invented. |
| 击退抗性 -> `NPC.knockBackResist` | `1 - percent / 100` | 荆棘苔龟 `0.3f` (70%) | The design's percentage is the resistance, so the reciprocal is the multiplier. |
| `NPC.value` copper transpose | 1:1, **no** conversion: 荆棘苔龟 `2银50铜` -> `250`; 格普螺 `4银` -> `400`; 叶飞棍 `2银` -> `200`; 巨树人 `2金50银` -> `25000` | tranche | `钱币（铜）` is the copper-coin column and tML documents `NPC.value` as "how many copper coins the NPC will drop", so the design value transposes 1:1. A 100x reading (`25000` / `40000` / `20000` / `250000`) is **explicitly rejected**: it would make an ordinary creature drop more than this repository's bosses, which sit at 32000–81000, while ordinary creatures sit at 200–400. |
| Rarity | `NPC.rarity = ItemRarityID.White` (= 0, the field's own default) | tranche | The design's trailing 稀有度 cell is **empty** for every row of this plan's table; 普通 is that row's 类型 cell, not a rarity value. 巨树人's headerless table instead leads with 稀有, whose `ItemRarityID.LightPurple` reading is recorded by plan 03-03. |
| `IsKelpCurtainLayer` degradation | X test returns `-1` when `StratumBoundCurve` is empty | all spawn-gated rows | `FindClosestStratumBoundPointX(float)` returns `-1` for an empty curve, so `player.Center.X >= -1 * 16` is always true: the test degrades to **permissive**, never to blocking, on a side that has not run `BuildBoundOf23Stratum`. |
| Vanilla tortoise state contract | `NPC.ai[0]` 0 walk / 1 retract / 3 spin / 4 recover | 荆棘苔龟 | Read back from the cloned `AI_039_Tortoise` behaviour installed by `CloneDefaults(NPCID.GiantTortoise)`; the spin branch sets `damage = defDamage * 2` and `defense = defDefense * 2`, which is why `PostAI` re-asserts the design's 75/20 after the vanilla AI. |
| Melee-reflect locality (D-35 note) | applied once on the client doing the damage | 荆棘苔龟 | D-35 makes subworld behaviour server-authoritative, but tML does **not** invoke `OnHitByItem` on the server at all, so the reflect cannot be applied server-side without a custom `ModIns.PacketResolver` round-trip that this phase does not take. Applying it once on the damaging client under a `player.whoAmI != Main.myPlayer` guard and letting `Player.Hurt` perform its normal hurt sync is the narrowest correct implementation; the not-taken packet alternative is recorded for a later phase. The hook is deliberately **not** wrapped in `Main.netMode != NetmodeID.MultiplayerClient`, which would make it dead code in multiplayer. |
| `aiStyle` choice | vanilla `CloneDefaults(NPCID.GiantTortoise)` | 荆棘苔龟 | No vanilla style provides a hit-triggered shell-retract state for 格普螺, so that row writes a local `AI()`; D-31 permits local `AI()` only where no vanilla style fits. |
| Shell retract duration (plan 03-02) | `120` ticks (~2 s), not reset by further hits | 格普螺 (`GuppyConch`) | The design states the retract ("被击中后会缩回壳中") but gives no duration; two seconds reads clearly and does not stall the fight. The window deliberately does **not** reset on a second hit, so the state stays a single server-authoritative transition. |
| Suffocation cadence (plan 03-02) | 30-tick interval, 2 life per interval, clamped at 1 life | 叶飞棍 (`VerdantRods`) | The design says the rod suffocates in water like a vanilla land creature but can fly out, so the drain is deliberately small and never fatal; the AI also biases its hover target upward while submerged. |
| Spawn weight (plan 03-02 rows) | `0.75f` (格普螺) and `1f` (叶飞棍), band 0.5f–2f | 格普螺, 叶飞棍 | The design supplies no weight; the band is calibrated against `NPCSpawnManager`'s 0.1f slime weights (A5) and plan 03-01's 1.5f. |
| 击退抗性 -> `NPC.knockBackResist` (plan 03-02 rows) | `1 - percent / 100`: 格普螺 `0.15f` (85%), 叶飞棍 `0.8f` (20%) | 格普螺, 叶飞棍 | The design's percentage is the resistance, so the reciprocal is the multiplier — the same rule as 荆棘苔龟's `0.3f` (70%). |
| `aiStyle` choice (格普螺, plan 03-02) | local `AI()`, state wrapped in the `GuppyConchState` enum over `NPC.ai[0]` | 格普螺 | No vanilla `aiStyle` provides a hit-triggered shell-retract state, so a local `AI()` is written; D-31 permits a local `AI()` where no vanilla style fits. 叶飞棍 (`VerdantRods`) likewise writes a local `AI()` for its neutral hover-and-circle behaviour. |
| Poison locality (D-35 note, plan 03-02) | 50% 中毒 for 600 ticks applied once on the client that owns the hit player, **not** wrapped in a netmode guard | 叶飞棍 | tML documents `ModNPC.OnHitPlayer` as "Called on the local client only", so a `Main.netMode != NetmodeID.MultiplayerClient` wrapper would make the designed 50% poison dead code in multiplayer; the hook runs once on the hit player's own client and `Player.AddBuff` performs its own hurt-path client -> server buff sync, exactly as `LargeBloodLanternGhost` and `LeafcutterAnt` do. The water-suffocation life drain in the same class **is** netmode-guarded, because that is authoritative NPC state rather than a player-side effect. |
| Copper `NPC.value` and `NPC.rarity` for the plan 03-02 rows | the tranche-wide rows above | 格普螺 (`4银` -> `400`), 叶飞棍 (`2银` -> `200`) | Both rows reuse the rules already recorded in this section: the copper value transposes 1:1 (the 100x readings `40000` / `20000` are explicitly rejected) and the empty 稀有度 cell yields the conservative `NPC.rarity = ItemRarityID.White` default. |

**Plan API-name corrections (Rule 1 — non-existent members).** Two member names written in `03-01-PLAN.md` do not exist in this tModLoader version and were corrected to the real members; both corrections are compile-verified:

| Plan wrote | Actual tML member | Evidence |
| --- | --- | --- |
| `spawnInfo.player` | `NPCSpawnInfo.Player` (a public **field**; there is no lowercase `player` member) | The tModLoader XML documentation lists only `F:Terraria.ModLoader.NPCSpawnInfo.Player`, and the in-repo precedent `RiverSlug.cs` reads `spawnInfo.Player`. The plan itself permitted `spawnInfo.Player` as "the property form". |
| `NPC.rare` | `NPC.rarity` (`F:Terraria.NPC.rarity`, the Lifeform Analyzer rarity; defaults to 0) | `Terraria.NPC` has no `rare` member. `NPC.rarity = ItemRarityID.White` is a no-op that records the design's empty 稀有度 cell. |

**Implemented by plan 03-03 (as built).** 巨树人 (`GiantDandelion`) is the only tranche creature whose design row specifies real timings, so most of its values are design-exact and only the unsupplied ones are defaults:

| Default | Value | Reason |
| --- | --- | --- |
| Smash wind-up / recovery | `120` / `300` ticks | Design-exact: the arm raise plus the smash takes 120 ticks, then a fixed 300-tick immobile recovery with the fist stuck in the floor. |
| Mid-range attack gap | `240` ticks minimum | Design-exact (至少240帧的间隔 between the state's two attacks); enforced by re-arming a cooldown whenever a boulder is thrown. |
| Forward arm swing | `20` ticks | The design gives no duration for the fast swing that brings up and releases the boulder (D-34). |
| Spawn weight | `0.25f` (band 0.1f–0.5f) | The design calls 巨树人 稀有 and supplies no weight; the value is deliberately lower than every other tranche creature (荆棘苔龟 `1.5f`, 叶飞棍 `1f`, 格普螺 `0.75f`). |
| Shockwave speed / lifetime / hit box | `6` px/tick, `40` ticks, `200x32` | The design gives no speed, lifetime or radius, only "large radius" and "roughly 2 tiles high" (D-34). |
| Boulder speed / gravity / pierce / spin | `9` px/tick launch, `0.35` px/tick², `1`, `0.2` rad/tick | The design gives no boulder speed, arc or pierce value (D-34). |
| Walk / approach / chase speeds | `0.5f` / `1.5f` / `3.2f` px/tick | The design only says the beyond-15-tile chase is faster than the 5-15 tile approach; the three values preserve that ordering. |
| Slash-separated stats | first value | 生命 `500/900/1300` -> `NPC.lifeMax = 500`; melee 伤害 `70/140/180` -> `NPC.damage = 70`. The slashes are the difficulty progression, so the normal-state value is the first. |
| Shockwave / boulder damage | `50` / `60` | The second and third rows of the 伤害 cell; passed to each projectile at spawn instead of being derived from `NPC.damage`. |
| `NPC.value` copper transpose | `2金50银` -> `25000`, 1:1 | `钱币（铜）` is the copper-coin column and `NPC.value` is documented in copper coins, so no conversion is applied; the 100x reading `250000` is **explicitly rejected** (it would exceed this repository's bosses, which sit at 32000–81000, while ordinary creatures sit at 200–400). |
| Rarity | `ItemRarityID.LightPurple` | This row's stats table is headerless with six cells and its leading cell reads `稀有`; the sibling tables' 稀有度 column is empty and absent here, so LightPurple is a conservative reading of that leading cell, not a design-exact value. |
| `aiStyle` choice | local `AI()`, state wrapped in the `GiantDandelionState` enum over `NPC.ai[0]` | No vanilla `aiStyle` provides a three-range charge/smash/recovery machine with a post-smash vulnerability window (D-31). |
| Sprite-derived extents | `NPC.width = 214`, `NPC.height = 263` | The approved sprite is the only measurement available. The full-sprite extents are large (about 13x16 tiles); §8 records the resulting spawn-area question as a runtime item. |

**Plan API-name corrections applied by plan 03-03 (Rule 1).** The two non-existent members corrected in plan 03-01 also appear in `03-03-PLAN.md`'s prose and were corrected the same way: `spawnInfo.player` -> `NPCSpawnInfo.Player` and `NPC.rare` -> `NPC.rarity`. Both corrections are compile-verified by the Release build.

**Plan 03 review-fix decisions (2026-09-15, `/gsd-code-review --fix`).** The Phase 3 code review (`03-REVIEW.md`) raised five Warning findings, all design-fidelity or robustness defects rather than correctness, security or crash defects. They were fixed against the committed design snapshot and committed atomically (`03-REVIEW-FIX.md`). The entries that introduce or re-state a value or band decision are recorded here:

| Default / decision | Value | Rows | Reason |
| --- | --- | --- | --- |
| 缩壳 defence | `NPC.defDefense * 2` = `20`, with the normal `50` contact damage | 荆棘苔龟 (`MossyThornTurtle`) | WR-01: the design's 防御 cell is `10 / 20（旋转/缩壳）` — design-exact, not a default — but `PostAI` only tested `Spinning`, so the cloned `AI_039_Tortoise` re-asserted `defense = defDefense` (10) through the whole retract window. `PostAI` now applies 20 during `Retracting`; only 旋转 raises 伤害 to 75, because the design's 伤害 cell brackets only 旋转. |
| Aggro radius | `30` tiles (`AggroRangeTiles`) | 巨树人 (`GiantDandelion`) | WR-03: the design's state (0) is a no-aggro wander and its chase clause is explicitly 有仇恨 (has aggro), but the row gives no radius. Without one, `TargetClosest()` made aggro true whenever any player was alive and Idle was unreachable. 30 tiles is wider than the 15-tile chase threshold, so the 5-tile smash, the 5-15 tile mid-range and the beyond-15-tile chase all stay reachable; the locomotion states fall back to Idle once the player escapes it. |
| Mid-range cooldown arming | armed only by the mid-range `BoulderThrow` path; the post-smash yank passes `armMidRangeCooldown: false` | 巨树人 (`GiantDandelion`) | WR-02: 至少240帧的间隔 is scoped by the design to 这个状态下 (the mid-range state's two attacks); the post-smash yank is a different attack, so arming the cooldown there suppressed the next mid-range attack for up to 240 ticks. |
| Shockwave floor anchoring | leading edge + centre + trailing edge sampled; the deepest floor wins | 巨树人's `GiantDandelion_Shockwave` | WR-04: the `200x32` hit box (already a D-34 default) was anchored from a single centre column, so on uneven ground an isolated hole dropped the wave and a higher terrace lifted it off the ground its leading half was over. |
| `IsKelpCurtainLayer` band anchor | synced **player centre** (`player.Center.Y`), deliberately about half a screen below the camera-driven `IsBiomeActive` band | all spawn-gated rows | WR-05: the helper's doc claimed to reproduce the `IsBiomeActive` band, but that band is measured from the client-only camera top. `Main.screenPosition` / `Main.screenHeight` are zero on a dedicated server, so the helper keeps the player-centre anchor as the only server-safe, side-identical band and the doc now states the offset instead of claiming parity. No band bound changed; §6 records the same clarification. |

## 5. Drop availability (D-43 nuance)

`03-RESEARCH.md` §Finding 2 concluded that Phase 3's drops are **all absent**. That conclusion used the superseded design-art tranche. Under the repository-art tranche it is **false**: three of the five tranche rows have at least one implemented item to wire, and five Phase 1–2 item types are consumed by this phase.

| Design drop | Repository item | Implementing phase | Tranche row(s) | Disposition |
| --- | --- | --- | --- | --- |
| 荆棘龟壳 | `ThornTurtleShell` | Phase 1 (`item-weapons.misc-thorn-turtle-shell`) | 荆棘苔龟 | **wired** — `ItemDropRule.Common(..., 20, 1, 1)` (5%) |
| 格普螺外壳 | `GuppyShell` | Phase 1 (`item-weapons.misc-guppy-shell`) | 格普螺 | **wired** — `ItemDropRule.Common(..., 9, 1, 1)` (11%) |
| 巨树之臂 | `ArmOfGiantTree` | Phase 1 (`item-weapons.misc-arm-of-giant-tree`) | 巨树人 | **wired** — guaranteed |
| 硬化枯木心脏 | `HardenedWitherbarkHeart` | Phase 1 (`item-weapons.misc-hardened-witherbark-heart`) | 巨树人 | **wired** — guaranteed |
| 巨石弹射装置 | `BoulderCatapult` | Phase 2 (`item-weapons.misc-巨石弹射装置`) | 巨树人 | **wired** — guaranteed (the item's own art remains incomplete) |
| 软体甲壳碎片 | — | — | 格普螺 | **absent** — blocker, no rule written (D-39) |
| 飞棍毛发 | — | — | 叶飞棍 | **absent** — blocker, no rule written |
| 毒腺 | — | — | 叶飞棍 | **absent** — blocker, no rule written |
| 枯木碎块 | — | — | 巨树人 | **absent** — blocker, no rule written |

Per-row disposition of the five tranche rows:

| Row | Drop table |
| --- | --- |
| 水蛞蝓 (`RiverSlug`) | The design names no drop for this row, so there is no table to write and no gap. |
| 荆棘苔龟 (`MossyThornTurtle`) | One rule (荆棘龟壳, 5%); the design names no other drop. |
| 格普螺 (`GuppyConch`) | One rule (格普螺外壳, 11%); 软体甲壳碎片 0–2 is absent and is a blocker — **partial**. |
| 叶飞棍 (`VerdantRods`) | Both designed drops (飞棍毛发 33%, 毒腺 11%) are absent, so the table is deliberately **empty** and the row carries the blocker. |
| 巨树人 (`GiantDandelion`) | Three guaranteed rules (巨树之臂, 硬化枯木心脏, 巨石弹射装置); 枯木碎块 4–6 is absent and is a blocker — **partial**. |

Four distinct absent materials block the five tranche rows — 软体甲壳碎片, 飞棍毛发, 毒腺 and 枯木碎块 — and the Phase 4 creatures add 亡碧膏, 牛黄 and 干涸心脏. No item scope was promoted into this phase (D-37/D-43); the mod still builds and loads.

**Implemented by plan 03-02 (as built).** 格普螺's single rule is written as `ItemDropRule.Common(ModContent.ItemType<GuppyShell>(), 9, 1, 1)` — denominator 9 = 11%, quantity 1 — and the design's other drop, 0~2 软体甲壳碎片, has no repository `ModItem`, so no rule exists for it and the blocker above stands. 叶飞棍's `ModifyNPCLoot` override is present but deliberately **empty**: 飞棍毛发 (33%) and 毒腺 (11%) have no repository `ModItem`, and a `ModContent.ItemType<...>()` for either would be a compile error that breaks the whole mod build (D-37/D-39). Neither class references an absent material as a type; the blockers live in `03-BIOLOGY.json` (overridden in the plan 03-02 task) and in this section.

**Implemented by plan 03-03 (as built).** 巨树人's `ModifyNPCLoot` writes exactly three rules, all guaranteed (chance denominator 1, quantity 1) and all naming already-implemented items: `ItemDropRule.Common(ModContent.ItemType<ArmOfGiantTree>(), 1, 1, 1)`, `ItemDropRule.Common(ModContent.ItemType<HardenedWitherbarkHeart>(), 1, 1, 1)` and `ItemDropRule.Common(ModContent.ItemType<BoulderCatapult>(), 1, 1, 1)` — the design's 一定掉落 for 巨树之臂, 硬化枯木心脏 and 巨石弹射装置. The design's 4~6 枯木碎块 has no repository `ModItem`, so no rule references it and no `ModContent.ItemType<...>()` for it exists anywhere in the class (that would be a compile error, D-37/D-39); the gap is the row's `drop blocked (absent item)` blocker. This makes 巨树人 the first tranche row that exercises a real multi-item drop table rather than only a blocked one.

## 6. Spawn-region gap

No biome predicate exists for 森雨幽谷 (Valley of Lush and Moist) or 刺苔庭园 (Spiny Moss Court), so per-region spawn refinement is impossible today. Spawn gating therefore uses the **layer-level** predicate plus local conditions, and the per-region refinement is a blocker naming the missing regional biome classes (Phase 5–6 terrain work).

The layer test is the new server-safe `KelpCurtainBiome.IsKelpCurtainLayer(Player)`, **not** `KelpCurtainBiome.IsBiomeActive`:

- `ModNPC.SpawnChance` runs "in single player or on the server only" (tModLoader XML documentation), and `IsBiomeActive` measures its vertical band from `Main.screenPosition` — the client-only camera, which is zero on a dedicated server, so the band test would always fail there and the whole tranche could never spawn.
- `IsKelpCurtainLayer` uses the same band bounds (`Main.maxTilesY * 0.72f * 16` … `Main.maxTilesY * 0.9f * 16`) and the same stratum-bound test (`player.Center.X >= FindClosestStratumBoundPointX(player.Center.Y / 16f) * 16`), but anchors the vertical band to the synced player centre (`player.Center.Y`) rather than to the camera top, so it evaluates identically in single player, on a client and on a dedicated server. The camera top sits roughly half a screen above the player, so this player-centred band is deliberately shifted down by about that offset relative to the camera-driven `IsBiomeActive` band: the two are **not** the same world rows, and the helper's doc states that offset rather than claiming parity (WR-05, review-fix 2026-09-15). No band bound changed.
- `FindClosestStratumBoundPointX(float)` returns `-1` when `StratumBoundCurve` is empty, which makes the X test permissive rather than blocking on a side that has not run `BuildBoundOf23Stratum`.
- `IsBiomeActive` itself was left byte-for-byte unchanged: it drives background and lighting scene transitions, which intentionally follow the camera.
- Every tranche `SpawnChance` also requires `SubworldSystem.IsActive<YggdrasilWorld>()`, because `NPCSpawnManager.EditSpawnPool` returns early outside the subworld and the per-creature gate is therefore the real isolation (BIO-06). The phase gate's invariant 7 fails any Phase 3 class that declares `SpawnChance` without both tokens.

**Plan 03-03 as-built note.** 巨树人 is the design's 刺苔庭园 (Spiny Moss Court) swamp creature. No Spiny Moss Court biome class, tile predicate or region X/Y range exists yet (the region work is Phase 5–6), so its `SpawnChance` uses the same layer-level server-safe gate as the other tranche rows — `SubworldSystem.IsActive<YggdrasilWorld>()` **and** `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` (never the client-camera `IsBiomeActive`, which would return `0f` on a dedicated server) — plus the land conditions already used by 格普螺 (reject `spawnInfo.Water` and reject a spawn tile whose `LiquidAmount > 0`), because 巨树人 is a land walker. The regional refinement remains a precise blocker on the row (`spawn context partial (D-30)`).

## 7. Localization (D-20)

Localization remains **deferred by user directive** (D-20). The in-game `OutputLocalizationHjsonItem` exporter was not run, no HJSON file was created or hand-edited, and no localization key was fabricated for any Phase 3 row.

| Row | Design name | Class file | Localization state |
| --- | --- | --- | --- |
| `bio-death-jade-lake-river-slug` | 水蛞蝓 | `Everglow.Yggdrasil.KelpCurtain.NPCs.RiverSlug` | deferred — pre-existing key untouched |
| `bio-valley-of-lush-and-moist-mossy-thorn-turtle` | 荆棘苔龟 | `Everglow.Yggdrasil.KelpCurtain.NPCs.MossyThornTurtle` | deferred — no key, exporter not run |
| `bio-valley-of-lush-and-moist-guppy-conch` | 格普螺 | `Everglow.Yggdrasil.KelpCurtain.NPCs.GuppyConch` | deferred — no key, exporter not run |
| `bio-death-jade-lake-verdant-rods` | 叶飞棍 | `Everglow.Yggdrasil.KelpCurtain.NPCs.VerdantRods` | deferred — no key, exporter not run |
| `bio-spiny-moss-court-giant-tree-man` | 巨树人 | `Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion` | deferred — no key, exporter not run |

Every class still overrides `LocalizationCategory` with `LocalizationUtils.Categories.NPCs` as the convention requires, but no key is written into it. Because no Feishu row carries a per-creature texture checkbox and no row's artwork **and** code are both verified in the source document, every row keeps `status: "unchecked"` and no completion colour (PROJECT.md Design Status Synchronization rules 4–5).

**Plan 03-02 deferral (extended, not lifted).** `GuppyConch` and `VerdantRods` are now code-complete, and their localization state is unchanged: the in-game `OutputLocalizationHjsonItem` exporter was not run for either row, no HJSON file was created or hand-edited, and no key was fabricated. The two classes override `LocalizationCategory` with `LocalizationUtils.Categories.NPCs` and carry the display names only in their XML doc comments, which are not localization keys.

**Plan 03-03 deferral (extended, not lifted).** `GiantDandelion` is now code-complete, and its localization state is unchanged: the in-game `OutputLocalizationHjsonItem` exporter was not run for this row, no HJSON file was created or hand-edited, and no key was fabricated. The class overrides `LocalizationCategory` with `LocalizationUtils.Categories.NPCs` and carries its display name only in its XML doc comment, which is not a localization key. Its two attack projectiles are not localized either.

## 8. Runtime verification (D-21)

Offline gates and `dotnet build /p:Configuration=Release /p:WarningLevel=0` cannot observe spawn isolation, AI feel, drop acquisition or dedicated-server behaviour. The following client checks are outstanding and are referred forward to plan 03-04's UAT bundle (`03-UAT.md`). Every entry is **not yet executed**; the tranche is therefore not presented as client-verified.

| Row | Client check | Expected |
| --- | --- | --- |
| 荆棘苔龟 | Enable the mod, enter Yggdrasil, descend into the Kelp Curtain; then idle in an ordinary world for several minutes. | The tortoise walks, retracts and spins as a vanilla tortoise does; defence/damage switch with the spin (10/50 walking, 20/75 spinning); a melee swing during the spin is reflected; it never appears in the ordinary world. |
| 格普螺 | Find one in the Kelp Curtain, hit it once, then walk into it. | It crawls slowly, never turns to attack, deals contact damage, retracts for about two seconds after a hit, and takes noticeably less damage while retracted. |
| 叶飞棍 | Meet one, let it touch the player repeatedly, then drive it into deep water. | It drifts in a wobbling path, circles without chasing, sometimes inflicts 中毒 on contact, loses health while submerged and flies back out. |
| 巨树人 | Meet one in the Spiny Moss Court area and fight it through a full state cycle. | A ground shockwave travels away from its feet; a boulder arcs toward the player; the post-smash window clears its defence. |
| 水蛞蝓 | Spawn and capture one, then use it as bait. | It crawls and sinks as before, is capturable, and provides fishing power. |

Also outstanding: a dedicated-server launch confirming the tranche spawns and fights there without client-only code crashing; a normal-world idle run confirming BIO-06 isolation for all five rows; and confirmation that the tML log shows no missing-resource or disabled-mod entry for the new classes (their textures resolve from the tracked assets beside them).

**Plan 03-02 rows (now implemented, checks still outstanding).** 格普螺 (`GuppyConch`) and 叶飞棍 (`VerdantRods`) are code-complete as of plan 03-02, so the two client checks in the table above are now executable for them; they remain **not yet executed**. New behaviours to observe in the D-21 run: the shell retract window (~2 s after a hit, with the normal-state 0.85 and shelled-state 0.70 减伤), the crawl reversal at walls and ledges, the leaf rod's neutral circling without chasing, its 50% 中毒 on contact, and its submerged life drain with the escape flight. A dedicated-server run should also confirm `KelpCurtainBiome.IsKelpCurtainLayer` produces the same band as the camera-driven `IsBiomeActive` for both creatures (the same requirement plan 03-01 recorded).

**Plan 03-03 rows (now implemented, checks still outstanding).** 巨树人 (`GiantDandelion`) and its two attack projectiles (`GiantDandelion_Shockwave`, `GiantDandelion_Boulder`) are code-complete as of plan 03-03, so the 巨树人 client check in the table above is now executable; it remains **not yet executed**. New behaviours to observe in the D-21 run: the 120-tick arm raise that ends in a visible ground wave, the 300-tick immobile recovery during which damage is clearly amplified (defence 0 and 150% incoming), the mid-range 120-tick backwards wind-up followed by the fast swing and the arcing boulder, the beyond-15-tile faster chase, and the ≥240-tick gap between mid-range attacks. Two open runtime questions are also recorded here: (a) whether the sprite-derived `214x263` hitbox is small enough for the creature to find a valid spawn area in the Kelp Curtain swamp — if it never spawns, the extents (not the `0.25f` weight) are the first thing to revisit; and (b) whether the floor-anchored shockwave keeps its 2-tile-high hit box on uneven terrain. A dedicated-server run should also confirm the two new projectiles emit no dust there (every dust call in both files and in the NPC sits inside `!Main.dedServ`), and that the authoritative spawns appear once rather than per client.

### 8.1 Close-out gate chain (plan 03-04, 2026-09-15)

Executed from the repository root **in one command** — `dotnet build` first, then each gate via `powershell -NoProfile -ExecutionPolicy Bypass -File`, then the MSTest link, then the AGENTS.md byte-level BOM block — chained with `if ($?)` so a failing link stops the chain. The whole chain exited **0** (`CHAIN_EXIT=0`).

| # | Command | Exit | Recorded success line |
| --- | --- | --- | --- |
| 1 | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | 0 | Build succeeded with `0 个警告` / `0 个错误` (0 warnings, 0 errors); `Everglow -> …\Mods\Everglow.tmod` produced and the mod enabled |
| 2 | `check-biology.ps1 -RequireAll` | 0 | `OK(0): phase3 tranche = 5 / 5 (rows=31)` · `OK: implemented classes = 5 / 5` · `OK: UTF-8 BOM check passed (5 files).` |
| 3 | `check-inventory-reconciliation.ps1` | 0 | `OK(0): 103 entries; matched=86; green=50 yellow=8 unchecked=45; labels=5 deferred=3 assumptions=7` |
| 4 | `check-carryover.ps1` | 0 | `OK(0): carry-over covered entries = 5 (of 5 selected)` |
| 5 | `check-tranche-A.ps1` | 0 | `OK(0): tranche-A covered entries = 43 (of 43 selected)` |
| 6 | `check-tranche-B.ps1` | 0 | `OK(0): tranche-B covered entries = 20 (of 20 selected)` |
| 7 | `check-armofgianttree-charge.ps1` | 0 | `OK(0): ArmOfGiantTree charge is per-player + per-stack slot-keyed, synced, and server-authoritative` |
| 8 | `check-phase2.ps1` | 0 | `OK(0): phase2 implemented = 21 / 21 (full=9, shell=12)` |
| 9 | `dotnet test --filter "FullyQualifiedName~Yggdrasil"` | 0 | `已通过 - 失败: 0, 通过: 3, 已跳过: 0, 总计: 3, 持续时间: 324 ms - Everglow.UnitTests.dll (net8.0)` (Passed — Failed 0, Passed 3, Skipped 0, Total 3) |
| 10 | AGENTS.md byte-level UTF-8 BOM block (§8.2) | 0 | `UTF-8 BOM check passed (1375 files).` |

Row 9 is the MSTest link `03-VALIDATION.md` declares part of the full suite command, so the plan and the validation strategy agree: the phase gate is the whole chain, not the build alone.

**Triage outcome: no earlier-phase failure occurred, so no triage was needed.** Phase 3 adds no item, tile, wall, buff or localization artifact, and every earlier gate stayed green: the five Phase 1 gates (rows 3–7) and the Phase 2 gate (row 8) ran unchanged, and no Phase 1 or Phase 2 artifact, gate script or baseline anchor was edited by this plan (T-03-24). Nothing was re-anchored, bypassed or silenced.

### 8.2 AGENTS.md byte-level UTF-8 BOM check (and the phase-scoped equivalent)

The block ran in the AGENTS.md shape: `$base = git merge-base HEAD origin/master`, then the file set from `git -c core.quotepath=false diff --name-only --diff-filter=ACMRTUXB $base --` plus `git -c core.quotepath=false ls-files --others --exclude-standard`, then a `[IO.File]::ReadAllBytes` first-three-bytes test against `0xEF 0xBB 0xBF`. Each of the three `git` calls checks `$LASTEXITCODE` explicitly (`git merge-base failed` / `git diff failed` / `git ls-files failed`) and no `git` call is followed by a shell pipeline, so a git failure aborts with a named message instead of being silently treated as an empty change set.

**Result: `UTF-8 BOM check passed (1375 files).`** — 1375 changed-or-untracked paths measured, 0 BOM-prefixed.

Anchor used: `origin/master`, resolved through `git merge-base HEAD origin/master`. Two facts about that anchor belong here:

- It is the **whole branch** measured against a distant pre-phase ancestor, so it is **advisory and non-isolating**. The binding, phase-scoped equivalent is invariant 13 inside `check-biology.ps1`, which the chain ran first (row 2) over Phase 3's own change set and which reported `OK: UTF-8 BOM check passed (5 files).` The AGENTS.md run is recorded for AGENTS.md compliance; it is not the gate that protects the phase's files.
- If the `origin/master`-anchored run ever reports a BOM in a path outside Phase 3's change set, that is a **stale-anchor finding** to record here with the offending path — nothing is edited to silence it, following the Phase 1 `01-07-SUMMARY.md` deviation 2 and the Phase 2 `02-05-SUMMARY.md` re-anchor precedent. This run reported no BOM at all.

## 9. Attack projectiles (plan 03-03)

巨树人 is the only tranche creature with designed attack projectiles, so this plan is the first to add hostile projectiles to the Kelp Curtain `Projectiles/Enemies/` tree. Two classes were written:

| Class | Role | SetDefaults | Visuals and dust |
| --- | --- | --- | --- |
| `GiantDandelion_Shockwave` | The state-1 ground smash: a `200x32` hit box (~2 tiles high) with a 40-tick lifetime, floor-anchored and ground-hugging, `tileCollide = false`, `penetrate = -1` | `Projectile.hostile = true`, `Projectile.friendly = false` | Shared fallback texture; every dust call inside `!Main.dedServ` |
| `GiantDandelion_Boulder` | The mid-range and post-recovery throw: a gravity arc that rotates with its velocity and despawns on the first tile struck (`tileCollide = true`, `penetrate = 1`) | `Projectile.hostile = true`, `Projectile.friendly = false` | Shared fallback texture; every dust call inside `!Main.dedServ` |

Both are spawned from `GiantDandelion` with `Projectile.NewProjectile(NPC.GetSource_FromAI(), ...)` inside a `Main.netMode != NetmodeID.MultiplayerClient` guard, so a single authoritative spawn is synced and never duplicated per client (T-03-18), and each carries the design's own damage row (50 shockwave / 60 boulder).

**Artwork blocker (raised here; not solvable in this phase).** No approved art exists in the repository for either projectile, and AGENTS.md forbids creating placeholder art, so both request only the existing shared `Commons.ModAsset.White_Mod` path — the same D-13 fallback policy the Phase 2 item `BoulderCatapult` and the accessory `RadialCarapace` already use. A real ground-wave sprite and a real boulder sprite are needed from the designer; until then the attacks are mechanically complete but visually untextured. This is the plan's contribution to the tranche's art-deferral family, and the row carries the `effect parameters provisional` blocker alongside this note.

**Why `BoulderCatapult_Proj` was not reused.** That class is a *friendly, player-owned* ranged projectile (`Projectile.friendly = true`, `owner`-relative direct-hit bonus, `Main.myPlayer == Projectile.owner` sub-projectile spawning) that assumes a player owner. Reusing it for a hostile attack would invert its ownership model and its 150% direct-hit bonus, and the plan's prohibitions forbid it outright. `GiantDandelion_Boulder` therefore imitates only its arc-and-rotate motion; the projectile was written fresh under `Projectiles/Enemies/`.

## 10. Blockers

The consolidated register: one row per blocker across the five Phase 3 tranche rows, generated mechanically from the `blockers` arrays of `03-BIOLOGY.json` by plan 03-04. Each **Exact blocker text** cell is byte-identical to the corresponding element of that row blocker array, so the ledger and the machine source of truth cannot disagree (T-03-25).

| Row id | Design name | Blocker kind | Exact blocker text | Why |
| --- | --- | --- | --- | --- |
| `bio-death-jade-lake-verdant-rods` | 叶飞棍 | drop | `drop blocked (absent items): design 33% 飞棍毛发 and 11% 毒腺 have no repository ModItem, so ModifyNPCLoot is deliberately empty (D-39)` | Phase 3 consumes Phases 1-2 item scope only: every designed drop is either wired to an implemented item or recorded as an absent-material blocker with no type reference written, because a reference to an absent type would not compile (D-36/D-37/D-39). |
| `bio-death-jade-lake-verdant-rods` | 叶飞棍 | localization | `localization deferred (D-20); runtime verification outstanding (D-21)` | Localization is deferred by user directive (D-20) and no live tModLoader client session is part of this phase (D-21); every row keeps status unchecked and no localization key was fabricated. |
| `bio-death-jade-lake-river-slug` | 水蛞蝓 | runtime | `pre-existing accepted implementation (D-23): class not modified by Phase 3; subworld isolation relies on SpawnModBiomes + KelpCurtainBiome and its HitEffect dust is not Main.dedServ-guarded (gate allowlist)` | D-23: RiverSlug is a pre-existing accepted implementation that Phase 3 does not modify; its HitEffect dust is not Main.dedServ-guarded, and the gate preExistingAllowlist exempts it from the Phase 3 structural guards. |
| `bio-death-jade-lake-river-slug` | 水蛞蝓 | localization | `localization deferred (D-20); runtime verification outstanding (D-21)` | Localization is deferred by user directive (D-20) and no live tModLoader client session is part of this phase (D-21); every row keeps status unchecked and no localization key was fabricated. |
| `bio-spiny-moss-court-giant-tree-man` | 巨树人 | drop | `drop blocked (absent item): design 4~6 枯木碎块 has no repository ModItem; the guaranteed 巨树之臂, 硬化枯木心脏 and 巨石弹射装置 rules are wired to Phase 1 items` | Phase 3 consumes Phases 1-2 item scope only: every designed drop is either wired to an implemented item or recorded as an absent-material blocker with no type reference written, because a reference to an absent type would not compile (D-36/D-37/D-39). |
| `bio-spiny-moss-court-giant-tree-man` | 巨树人 | effect | `effect parameters provisional: the design gives no shockwave radius, boulder speed or pierce values, so conservative values are used` | The design supplies no value for this parameter, so the tranche conservative default is used and recorded here rather than silently chosen (D-34). |
| `bio-spiny-moss-court-giant-tree-man` | 巨树人 | spawn | `spawn context partial (D-30): the design's 刺苔庭园 swamp has no biome or tile predicate yet; the gate uses the layer-level KelpCurtainBiome plus land conditions` | No regional biome or tile predicate exists yet for the design region, so spawn gating uses the server-safe layer-level predicate and the per-region refinement is Phase 5-6 terrain work (D-30). |
| `bio-spiny-moss-court-giant-tree-man` | 巨树人 | localization | `localization deferred (D-20); runtime verification outstanding (D-21)` | Localization is deferred by user directive (D-20) and no live tModLoader client session is part of this phase (D-21); every row keeps status unchecked and no localization key was fabricated. |
| `bio-valley-of-lush-and-moist-mossy-thorn-turtle` | 荆棘苔龟（Thorn Mossy Tortoise） | drop | `drop disposition: the design's 荆棘龟壳 is wired to the Phase 1 item ThornTurtleShell at 5% (ItemDropRule.Common(..., 20, 1, 1)); the design names no other drop` | Phase 3 consumes Phases 1-2 item scope only: every designed drop is either wired to an implemented item or recorded as an absent-material blocker with no type reference written, because a reference to an absent type would not compile (D-36/D-37/D-39). |
| `bio-valley-of-lush-and-moist-mossy-thorn-turtle` | 荆棘苔龟（Thorn Mossy Tortoise） | effect | `effect parameters provisional: the design supplies no spawn weight, hit box or knockback values, so the tranche's conservative defaults are used (D-34)` | The design supplies no value for this parameter, so the tranche conservative default is used and recorded here rather than silently chosen (D-34). |
| `bio-valley-of-lush-and-moist-mossy-thorn-turtle` | 荆棘苔龟（Thorn Mossy Tortoise） | localization | `localization deferred (D-20); runtime verification outstanding (D-21)` | Localization is deferred by user directive (D-20) and no live tModLoader client session is part of this phase (D-21); every row keeps status unchecked and no localization key was fabricated. |
| `bio-valley-of-lush-and-moist-guppy-conch` | 格普螺 | drop | `drop blocked (absent item): design 0~2 软体甲壳碎片 has no repository ModItem; the 11% 格普螺外壳 rule is wired to GuppyShell (Phase 1)` | Phase 3 consumes Phases 1-2 item scope only: every designed drop is either wired to an implemented item or recorded as an absent-material blocker with no type reference written, because a reference to an absent type would not compile (D-36/D-37/D-39). |
| `bio-valley-of-lush-and-moist-guppy-conch` | 格普螺 | localization | `localization deferred (D-20); runtime verification outstanding (D-21)` | Localization is deferred by user directive (D-20) and no live tModLoader client session is part of this phase (D-21); every row keeps status unchecked and no localization key was fabricated. |

Every tranche row carries the canonical `localization deferred (D-20); runtime verification outstanding (D-21)` element, so the D-20/D-21 disposition is uniform across the tranche and cannot be dropped from one row without failing the mechanical regeneration. The missing spawn predicate is carried per-row by 巨树人's `spawn context partial (D-30)` element and, for the whole tranche, in the prose of §6; the `drop` rows are the D-37/D-39 contract, and the single `runtime` row is the D-23 acceptance of the pre-existing 水蛞蝓 class.

## 11. Phase 3 close-out

Frozen counts, unchanged by this plan: **rows 31**, **phase3 5**, **phase4 23**, **phase7 3**, **deferred 2**, with `texture_complete_true` 6 and `design_art_true` 9. `03-BIOLOGY.json` is the machine source of truth (D-24) and `03-BIOLOGY.md` mirrors it row-for-row; gate invariant 11 re-asserts the row-id set and every row's `status` on each run, and `check-biology.ps1 -RequireAll` closes the tranche at `OK(0): phase3 tranche = 5 / 5 (rows=31)` / `OK: implemented classes = 5 / 5`.

The five implemented tranche rows — every one `code_complete: true` with a non-empty `internal_name` that resolves to a class file on disk:

| Row id | Design name | Class | Drops wired | Drops absent |
| --- | --- | --- | --- | --- |
| `bio-death-jade-lake-river-slug` | 水蛞蝓 | `RiverSlug` (pre-existing, D-23) | none (the design names none) | none |
| `bio-death-jade-lake-verdant-rods` | 叶飞棍 | `VerdantRods` | none | 飞棍毛发, 毒腺 |
| `bio-spiny-moss-court-giant-tree-man` | 巨树人 | `GiantDandelion` | `ArmOfGiantTree`, `HardenedWitherbarkHeart`, `BoulderCatapult` | 枯木碎块 |
| `bio-valley-of-lush-and-moist-mossy-thorn-turtle` | 荆棘苔龟 | `MossyThornTurtle` | `ThornTurtleShell` | none |
| `bio-valley-of-lush-and-moist-guppy-conch` | 格普螺 | `GuppyConch` | `GuppyShell` | 软体甲壳碎片 |

**Six design-art-only creatures stay in Phase 4.** They carry an inline design `<img>` but no approved repository texture, so D-41 places them in Phase 4 (`design_art: true`, `texture_complete: false`, `phase: 4`): 水黾, 幽光蝾螈（美西螈）, 装甲虾, 帆鳍鳢, 覆藻章鱼 and 大型覆藻章鱼. A later designer confirmation may move them back into Phase 3.

**One boss row also has repository art.** `bio-out-of-phase-kelp-snake` (苍带帘蛇/克莱因蛇, `NPCs/AcroporaSnake.png`) is `texture_complete: true` but `phase: 7`: it is the Phase 7 boss art, not tranche material.

**No item scope was promoted into this phase (D-37/D-43).** Phase 3 introduces no item, tile, wall, buff or localization artifact; it consumes Phases 1–2. The tranche's drop obligations are satisfied by wiring **five Phase 1–2 item types** — `ThornTurtleShell`, `GuppyShell`, `ArmOfGiantTree`, `HardenedWitherbarkHeart` and `BoulderCatapult` — and by recording **four** absent materials as blockers: 软体甲壳碎片, 飞棍毛发, 毒腺 and 枯木碎块. That is four distinct materials across the five tranche rows, not five, and no drop rule, type reference or placeholder item exists for any of them.

## 12. Decision disposition

| Decision | Applied? | Disposition |
| --- | --- | --- |
| D-28 (full implementation where the design row is complete) | yes — all five tranche rows | Every tranche row is a full `ModNPC` implementation (spawn, AI/movement, hostility, status effects, hit behaviour and drop wiring), not an identity shell. |
| D-29 (identity-only shell where behaviour is undefined) | **not triggered in this phase** | Every repository-art creature has a defined behaviour row, so no shell class was needed. The shell candidates are the Phase 4 rows whose design section defines no behaviour — 荧光水螅, 巨型虎虾, 吸血魔毯 and 炮弹藤壶 — each labelled `D-29 shell` in the matrix `blockers`. |
| D-30 (implement the independently completable part; name the missing system) | applied only to 巨树人 | Its design region 刺苔庭园 (Spiny Moss Court) has no biome or tile predicate yet, so the row carries `spawn context partial (D-30)`; the other tranche rows reuse the same server-safe layer-level gate, and the tranche-wide per-region refinement is recorded in §6. |
| D-31 (vanilla `aiStyle` where it fits; a local `AI()` only where necessary) | 荆棘苔龟 only | 荆棘苔龟 uses `CloneDefaults(NPCID.GiantTortoise)` with `defDamage`/`defDefense` pinned and the spin state wrapped over `NPC.ai[]`. 格普螺, 叶飞棍 and 巨树人 each write a local `AI()`, justified in §4: no vanilla style provides a hit-triggered shell retract, a neutral hover-and-circle, or a three-range charge/smash/recovery machine. |

## 13. Deferred registry

| Category | Item | Status | Owner |
| --- | --- | --- | --- |
| Unfinished art | The six design-art-only creatures (水黾, 幽光蝾螈（美西螈）, 装甲虾, 帆鳍鳢, 覆藻章鱼, 大型覆藻章鱼) | Deferred to Phase 4 (D-41) | Phase 4 |
| Unfinished art | Approved sprites for the two 巨树人 attack projectiles (`GiantDandelion_Shockwave`, `GiantDandelion_Boulder`), which use `Commons.ModAsset.White_Mod` today | Open — the designer must supply the art (§9) | Designer / Phase 8 |
| Boss / special encounter | 苍带帘蛇/克莱因蛇 (`bio-out-of-phase-kelp-snake`) and 巨翼龙 (`bio-out-of-phase-giant-winged-dragon`) | Deferred to Phase 7 | Phase 7 |
| Hardmode-deferred designs | 枯萎之种 and 枯木人卫士 (design moved to hardmode, 暂时不用，挪到困难模式) | Deferred — outside this milestone | V2-HARD-01 |
| Absent drop materials | 软体甲壳碎片, 飞棍毛发, 毒腺, 枯木碎块 (plus the Phase 4 materials 亡碧膏, 牛黄, 干涸心脏) | Open — item scope, not this phase (D-37/D-43) | Phase 1–2 follow-up / Phase 8 |
| Localization | All five tranche rows and the two attack projectiles | Deferred by user directive (D-20) — no exporter run, no key fabricated, no HJSON edited | Phase 8 |
| Live runtime verification | The D-21 client bundle in `03-UAT.md` (spawn, isolation, behaviour, combat, drops, dedicated server, localization fallback) | Recorded, not executed (D-21) | Client session / Phase 8 |
| Regional spawn refinement | 森雨幽谷 and 刺苔庭园 biome/tile predicates | Open — Phase 5–6 terrain work (D-30, §6) | Phases 5–6 |

**Phase 3 close-out statement (plan 03-04).** The phase closes with its tranche **code-complete**: all five rows implemented, the matrix and its mirror reconciled cell-for-cell, the consolidated ledger written, the full offline chain and the Release build green in one run (§8.1) and the D-21 client bundle recorded in `03-UAT.md`. **Every row of the table above stays open** — none was closed by this plan, and none was silently dropped. The two runtime-facing rows (live runtime verification, regional spawn refinement) and the two art rows (the six Phase 4 creatures, the two untextured attack projectiles) are the phase's explicit residue, and the localization and absent-material rows are carried by user directive (D-20) and by D-37/D-43 respectively. No `.png` or other binary/art asset was created, moved, renamed or modified by this phase; no HJSON file was created or hand-edited; no localization key was fabricated; and the Feishu design source was neither re-fetched nor mutated (D-25).
