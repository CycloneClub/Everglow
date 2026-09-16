# Phase 4 Deviation Ledger — Remaining Ordinary Monsters

Generated: 2026-09-16
Opened by plan 04-01; §14 is reserved for the phase close-out that plan 04-09 writes.

**Design source (read-only, D-25):** the committed snapshot `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` (Feishu biology token `Jp5ndsvNBoCpljxq1eGc9S7vnfe`). The document is never re-fetched and never mutated; every stat, behaviour and drop value below comes from that snapshot.
**Machine source of truth (D-24):** `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json`, mirrored by `03-BIOLOGY.md` and read by the Phase 4 gate `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1`.

This is the single deviation and decision ledger for the 21 remaining ordinary Kelp Curtain creatures (D-44). It records the D-44…D-59 decisions as applied or not triggered, the OQ1–OQ5 resolutions the research deferred to the plans, the conservative defaults (D-54/D-59) and the blocker register. No field below was changed by editing the Feishu design source, no localization key was created or renamed, and no `.png`, `.obj`, `.xnb` or other binary/art asset was created, moved, renamed or modified (D-51, AGENTS.md).

**Coverage claim.** This ledger covers all 21 in-scope Phase 4 rows — the thirteen Death Jade Lake, three Spiny Moss Court and five Valley of Lush and Moist rows of `03-BIOLOGY.json`. Every row appears in §1 (scope), §4 (missing art), §5 (spawn routing), §6 (drop wiring) and §10 (defaults), and in §7 (effect blockers) and §13 (deferred registry) where it carries one. An in-scope row absent from this ledger is a defect.

**Navigation.** §1 scope and the 21-row set · §2 variant strategy · §3 folder and namespace · §4 missing-art handling · §5 spawn routing · §6 drop wiring (incl. §6.1 the OQ1 cross-namespace decision) · §7 projectile scope · §8 mini-boss depth call · §9 gate placement · §10 conservative defaults register · §11 runtime verification · §12 localization · §13 deferred registry · §14 phase close-out (reserved for plan 04-09).

## 1. Scope and the 21-row set (D-44)

`[phase] == 4` in `03-BIOLOGY.json` yields **23** rows. The two `out of phase` hardmode rows — `bio-out-of-phase-withered-seed` (枯萎之种) and `bio-out-of-phase-withered-tree-guardian` (枯木人卫士) — stay `deferred: true` with a non-empty `deferred_reason` and are **never** implemented (**V2-HARD-01**). 23 − 2 = **21** in-scope rows, frozen by the Phase 4 gate (`.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1`, invariant 3) against a literal 21-id list.

| Region | Rows | Row ids (class each plan creates) |
| --- | --- | --- |
| Death Jade Lake | 13 | `bio-death-jade-lake-fluorescent-hydra` (`FluorescentHydra`, D-45 shell) · `bio-death-jade-lake-giant-tiger-shrimp` (`GiantTigerShrimp`, D-45 shell) · `bio-death-jade-lake-water-strider` (`WaterStrider`) · `bio-death-jade-lake-toxic-toad` (`ToxicToad`) · `bio-death-jade-lake-glow-salamander` (`GlowSalamander`) · `bio-death-jade-lake-armored-shrimp` (`ArmoredShrimp`) · `bio-death-jade-lake-bomb-jellyfish` (`BombJellyfish`) · `bio-death-jade-lake-sailfin-snakehead` (`SailfinSnakehead`) · `bio-death-jade-lake-radiolarian` (`Radiolarian`) · `bio-death-jade-lake-algae-octopus` (`AlgaeOctopus`) · `bio-death-jade-lake-large-algae-octopus` (`LargeAlgaeOctopus`) · `bio-death-jade-lake-jade-anglerfish` (`JadeSpiritAnglerfish`, plan 04-01) · `bio-death-jade-lake-cannon-barnacle` (`CannonBarnacle`, D-45 shell) |
| Spiny Moss Court | 3 | `bio-spiny-moss-court-withered-soldier` (`AnimatedWitherbarkSoldier` + 3 stat-variant siblings) · `bio-spiny-moss-court-court-commander` (`CourtCommander`) · `bio-spiny-moss-court-brodie-flydragon` (`BrodieFlydragon` + 1 stat-variant sibling) |
| Valley of Lush and Moist | 5 | `bio-valley-of-lush-and-moist-red-needle-caterpillar` (`RedNeedleCaterpillar`) · `bio-valley-of-lush-and-moist-assassin-raspberry` (`AssassinRaspberry`) · `bio-valley-of-lush-and-moist-serpent-moss` (`SerpentMoss`) · `bio-valley-of-lush-and-moist-small-guppy-conch` (`SmallGuppyConch`) · `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle` (`LargeMossyThornTurtle`) |

**The entity is the ROW, not the class.** The phase freezes **21 rows** and creates **26** `ModNPC` classes, because three design rows carry stat variants as sibling classes (§2): 爆弹水母 `+1`, 布罗迪蝇蜓 `+1` and 枯木活化士兵 `+3`. With the eleven projectiles of §7 the phase adds `26 region-folder classes + 11 phase projectiles = 37` guarded class files, which is the number the gate's invariant 7 counts.

Eighteen of the 21 rows are full implementations; three are D-45 identity shells because their design section carries a heading and nothing else (荧光水螅, 巨型虎虾, 炮弹藤壶) — they load, register and carry a precise `behavior undefined in the design row (D-29/D-45 shell)` blocker rather than inventing behaviour.

## 2. Variant strategy (OQ2, D-47)

**OQ2 resolution: separate classes for stat variants; one class with a synced variant index for colour-only variants.** Recorded here because `03-BIOLOGY.json`'s schema names exactly one `internal_name` per row and `SetDefaults` is per-type (Pitfall 5).

| Design row | Row's `internal_name` | Sibling classes | Reason |
| --- | --- | --- | --- |
| 枯木活化士兵 | `AnimatedWitherbarkSoldier` (近战) | `AnimatedWitherbarkSoldierRanged` (远程), `AnimatedWitherbarkSoldierSpell` (法术), `AnimatedWitherbarkHound` (犬) | The design gives four stat rows (生命 80/60/55/75, 伤害 22/35/24/20, 防御 4/0/0/2, 击退抗性 20/40/40/70) and different drops (only the 犬 variant drops 活化之犬召唤杖). |
| 爆弹水母 | `BombJellyfish` (小型, 生命 10) | `LargeBombJellyfish` (大型, 生命 25) | The design's 小/大 pair has different life and different death-blast damage (30 / 50), so they cannot share one `SetDefaults`. |
| 布罗迪蝇蜓 | `BrodieFlydragon` (标准, 生命 40) | `SmallBrodieFlydragon` (小型, 生命 20) | The design's 小蝇蜓 is spawned by the Valley egg system, not by natural spawning; it is a distinct type with its own stats. |

**Colour-only variants are ONE class.** 幽光蝾螈's three colours (灰蓝色 / 粉色 / 褐色) are a visual property of one design row with one stat line, so `GlowSalamander` keeps a variant index over `NPC.localAI[]`, synchronised with `SendExtraAI`/`ReceiveExtraAI`. The index is **inert until art arrives** — with no sprite there is no observable colour — but it exists so the D-49 art migration can map frames to it without touching `SetDefaults`.

**The rule that makes this safe:** `SetDefaults` runs once per NPC *type*, so a stat variant must never be produced by mutating `NPC.lifeMax` (or any other stat) in `AI()` or `OnSpawn()`. Every stat variant is its own type with its own `SetDefaults`; only colour lives in per-instance state.

## 3. Folder and namespace decision (D-49, Pitfall 3)

Every Phase 4 class lives under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/<Region>/` with namespace `Everglow.Yggdrasil.KelpCurtain.NPCs.<Region>`, where `<Region>` is `DeathJadeLake`, `SpinyMossCourt` or `ValleyOfLushAndMoist` exactly as written in §1.

**Precedent in this repository:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VampireMat/VampireMat.cs` declares `namespace Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat`, carries `VampireMat.png` beside it and defines **no** `Texture` override — proof that tML's default (namespace-derived) texture resolution finds art inside a subfolder.

**Why this is load-bearing (Pitfall 3).** The module's `PathPrefix` is the module name (`Yggdrasil`), so `...NPCs.DeathJadeLake.JadeSpiritAnglerfish` must resolve to the runtime asset path `Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/JadeSpiritAnglerfish.png`, i.e. the file beside the `.cs`. Choosing the region folder now makes the D-49 migration exactly:

1. add `<Class>.png` beside the `.cs`;
2. delete `public override string Texture => Commons.ModAsset.White_Mod;`;
3. delete the row's `artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48)` blocker.

No namespace change, no asset move, no class rework.

Phase 3's naming rule (keep a `ModNPC` flat when its approved art sits directly in `NPCs/`) does **not** apply: its precondition is art that already exists in `NPCs/`, and no Phase 4 creature has repository art.

Opened by plan 04-01 with `NPCs/DeathJadeLake/JadeSpiritAnglerfish.cs`; `NPCs/SpinyMossCourt/` and `NPCs/ValleyOfLushAndMoist/` do not exist until waves 3–4, and the gate treats a missing region root as zero guarded files rather than an error.

## 4. Missing-art handling (D-48/D-49/D-50/D-51)

No Phase 4 creature has repository art, so **every** Phase 4 class overrides `public override string Texture => Commons.ModAsset.White_Mod;` (the Phase 2 precedent used by `RadialCarapace` and eighteen other art-missing classes; the backing asset `Sources/Everglow.Function/Textures/White.png` exists and the generated `Commons.ModAsset` member resolves). This is mandatory rather than cosmetic: a class that neither overrides `Texture` nor has a beside-`.png` requests a non-existent namespace-derived path and aborts mod loading.

Every one of the 21 in-scope rows therefore carries exactly one D-48 artwork blocker whose literal text is:

`artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48)`

The phase creates, moves, renames and modifies **no** `.png`, `.obj`, `.xnb` or other binary/art asset; the missing art is listed as a blocker only. The gate asserts both directions of the invariant:

- invariant 7: every guarded class contains `Commons.ModAsset.White_Mod` **or** has a resolving `<basename>.png` beside it;
- invariant 9: `git status --porcelain -uall` over the NPCs and enemy-projectile trees must report no added or modified `.png`/`.obj`/`.xnb` (D-51);
- invariant 5: every in-scope row carries at least one blocker whose text contains `artwork` (D-50).

The art debt this phase opens is 26 `ModNPC` sprites plus eleven projectile sprites, each migratable by the three-step move in §3.

## 5. Spawn routing (D-52/D-53/D-54/D-55)

**Layer gate.** Every `SpawnChance` starts with the two server-safe tokens and returns `0f` when either fails:

```csharp
if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
{
	return 0f;
}
```

`NPCSpawnManager.EditSpawnPool` returns early outside the subworld, so the pool is *not* filtered there and the per-creature `SpawnChance` is the only real isolation (BIO-06, Pitfall 1).

**`IsBiomeActive` is deliberately NOT used in `SpawnChance`.** It is camera-driven (`Main.screenPosition`), which is zero on a dedicated server and measures a band that is roughly half a screen away from the player-centre band. `KelpCurtainBiome.IsKelpCurtainLayer(Player)` is the server-safe counterpart Phase 3 added (D-52). The gate fails on `Main.LocalPlayer` in any guarded class.

**The three water conditions are three distinct predicates**, not one "in water" flag (Pitfall 6), and all three live in `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/KelpCurtainSpawnConditions.cs` (added by plan 04-01):

| Predicate | Design condition | Consumers |
| --- | --- | --- |
| `IsDryLand(spawnInfo)` | land (地表) — rejects `spawnInfo.Water` and a liquid spawn tile | 剧毒蟾蜍, 幽光蝾螈 (amphibious), 装甲虾 group land drift, 帆鳍鳢's land avoidance, all three Spiny Moss Court rows and all five Valley rows |
| `IsWaterSurface(spawnInfo)` | 水面上 — requires `spawnInfo.Water` and a dry tile directly above the spawn tile | 水黾 |
| `IsWaterBottom(spawnInfo)` | 水底 — requires `spawnInfo.Water` and a solid floor reached by the liquid column inside a bounded 24-tile probe | 碧灵鮟鱇, 放射虫, 大型覆藻章鱼 |

**Tuning note (水黾).** `NPCSpawnInfo` exposes no surface flag, so `IsWaterSurface` derives the surface from a dry tile above the spawn tile. If that proves too strict in the client run, `DeathJadeLakeBiome.LiquidSurfaceY` (the synced float `GetLiquidSurfaceY()` computes by scanning `TileUtils.SafeGetTile(..., LiquidAmount)`) is the secondary cross-check; it is recorded here rather than wired, because it is recomputed only on world-gen passes.

**No dependency on terrain Phases 5–6 have not built (D-53).** Spawn gating uses only the layer predicate, the subworld token, liquid/tile state and conservative weights. The region-level refinement gap is recorded as a Phase 5–6 blocker: 森雨幽谷 / 刺苔庭园 / 亡碧湖 sub-biome or tile predicates do not exist yet, so the matrix carries a `region: 森雨幽谷 sub-biome or tile predicate not implemented (Phase 5-6 terrain work; D-52)` element for 覆藻章鱼 and 大型覆藻章鱼, and the same gap applies to 布罗迪蝇蜓 (刺苔庭园 / 森雨幽谷) and 红针洋辣子 (Valley region split).

**Server-authoritative behaviour (D-55).** Every AI state transition is written under `Main.netMode != NetmodeID.MultiplayerClient` with `NPC.netUpdate = true`, and every `Lighting`, dust, gore or VFX call sits inside `if (!Main.dedServ)` (D-35). State that clients must agree on lives in the synced `NPC.ai[]` / `NPC.localAI[]` arrays and is read back through named wrapper properties — never a bare numeric `NPC.ai[]` index (AGENTS.md). Damage that depends on state is re-asserted in `PostAI()` from the synced state (the `MossyThornTurtle` precedent), because `NPC.damage` is not part of the NPC net message.

## 6. Drop wiring (D-56/D-57/D-58/D-59)

Drops use direct drop tables (`ModifyNPCLoot` + `ItemDropRule.Common(item, denominator, min, max)`) and reference **only items that already exist in the repository** (D-57). A rule naming an absent type fails the whole mod build (Pitfall 2/T-04-03), so absent design drops get **no type reference at all** plus a precise D-58 blocker.

### 6.1 The wireable drop rules

| Design row | Design drop | Wired rule | Denominator source |
| --- | --- | --- | --- |
| 碧灵鮟鱇 | 25% 1 肉食性提灯, 12.5% 1 灵灯 | `ItemDropRule.Common(ModContent.ItemType<MeatLantern>(), 4, 1, 1)` and `ItemDropRule.Common(ModContent.ItemType<Photophore>(), 8, 1, 1)` | 25% → 4, 12.5% → 8 |
| 放射虫 | 6.7% 放射状甲壳 (+ absent materials) | `ItemDropRule.Common(ModContent.ItemType<RadialCarapace>(), 15, 1, 1)` | 6.7% → 15 |
| 枯木活化士兵 (犬 variant only) | 9% (1/11) 活化之犬召唤杖 | `ItemDropRule.Common(ModContent.ItemType<ActivatedDogStaff>(), 11, 1, 1)` on `AnimatedWitherbarkHound` only | 9% → 11 |
| 红针洋辣子 | 掉落同树皮刺毛虫 | `ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 1, 2)`, mirroring `BarkSpicyCaterpillar.cs` | guaranteed → 1 |

**OQ1 cross-namespace decision (recorded explicitly).** 红针洋辣子's design text *"掉落同树皮刺毛虫"* is satisfied by wiring the **pre-existing, already-implemented** `CaterpillarJuice` from `Sources/Modules/Yggdrasil/YggdrasilTown/Items/Materials/CaterpillarJuice.cs`, exactly as the existing `Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/BarkSpicyCaterpillar.cs` drops it (`ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 1, 2)`). This is read as satisfying D-57's "no new item scope": nothing new is created, and an existing implemented item is referenced — but the referenced item sits **outside** the Kelp Curtain Phase 1–2 inventory, so the Phase 4 gate's item-type index deliberately covers the whole `Sources/Modules/Yggdrasil` tree rather than `KelpCurtain/Items` only (gate invariant 8). **Alternative considered and not taken:** an empty `ModifyNPCLoot` plus a D-58 blocker (the strict reading of D-57). It was not taken because the design explicitly names an existing drop, the reference is compile-safe, and an empty table would silently delete a reward the design specifies.

### 6.2 The absent / blocked drops

Each row below gets **no** `ModContent.ItemType<...>` reference anywhere in the phase; the design drop is recorded only as a D-58 blocker element.

| Design drop | Rows | Matrix blocker element |
| --- | --- | --- |
| 毒腺 | 剧毒蟾蜍 (33%), 幽光蝾螈 | `drop: 毒腺 ... not implemented (item scope; D-58)` |
| 牛黄 | 剧毒蟾蜍 (1%/2%), 幽光蝾螈 | `drop: ... 牛黄 ... not implemented (item scope; D-58)` |
| 软体甲壳碎片 | 装甲虾, 放射虫 (2~5), 覆藻章鱼, 大型覆藻章鱼 (2~4) | `drop: 软体甲壳碎片 ... not implemented (item scope; D-58)` |
| 亡碧膏 | 帆鳍鳢 (33%), 放射虫 (1~2), 覆藻章鱼, 大型覆藻章鱼 (3~6) | `drop: ... 亡碧膏 ... not implemented (item scope; D-58)` |
| 枯木碎块 | 枯木活化士兵 (1~2 / 犬 1), 王庭号令者 (2~4) | `drop: 枯木碎块 ... not implemented (item scope; D-58)` |
| 干涸心脏 | 枯木活化士兵 (morale-gated 50%), 王庭号令者 (50%) | `drop: ... 干涸心脏 (morale-gated 50%) not implemented (item scope; D-58)` |
| 大型荆棘苔龟 drop list | 大型荆棘苔龟 | `drop: design drop list is TBD (死亡掉落物暂定) not implemented (item scope; D-58)` |
| 武器与饰品掉落待定 | 大型覆藻章鱼 | covered by its own 软体甲壳碎片 / 亡碧膏 D-58 element |

Fifteen of the 21 rows have no designed drop at all; their `ModifyNPCLoot` follows the `VerdantRods` precedent — empty where a design drop is absent, or omitted entirely when the design names no drop, with the blocker living in the matrix rather than as dead code.

## 7. Projectile scope (OQ4)

**OQ4 resolution: build the projectile when it carries the creature's primary attack and is self-contained; record an effect blocker for pure-VFX or system behaviour.** The phase builds exactly **eleven (11)** projectiles, frozen literally in the gate's `$phaseProjectiles` list and in the ROADMAP §Phase 4 scope anchor:

| # | Projectile | Owner | Why it is built |
| --- | --- | --- | --- |
| 1 | `ToxicToad_PoisonBubble` | 剧毒蟾蜍 | the creature's only attack |
| 2 | `ToxicToad_PoisonCloud` | 剧毒蟾蜍 | its death cloud is its primary lingering threat |
| 3 | `Radiolarian_WaterBolt` | 放射虫 | its ranged attack |
| 4 | `RedNeedleCaterpillar_Spike` | 红针洋辣子 | its ranged attack |
| 5 | `AssassinRaspberry_Spike` | 阿萨辛覆盘子 | its only attack |
| 6 | `AnimatedWitherbarkSoldier_Boulder` | 枯木活化士兵 (远程) | the 远程 variant's attack |
| 7 | `AnimatedWitherbarkSoldier_SpellBeam` | 枯木活化士兵 (法术) | the 法术 variant's attack |
| 8 | `AlgaeOctopus_InkCloud` | 覆藻章鱼 and 大型覆藻章鱼 | shared ink cloud; the design's 4-cloud wave is four spawns of this one projectile, recorded as a simplification of a **pure-VFX multiplicity**, not of the damage, and 大型覆藻章鱼's every-180-frame wave uses the same class |
| 9 | `BombJellyfish_Explosion` | 爆弹水母 | its 30 (小) / 50 (大) death blast |
| 10 | `LargeMossyThornTurtle_Shockwave` | 大型荆棘苔龟 | the state-2 floor shockwave |
| 11 | `LargeMossyThornTurtle_Boulder` | 大型荆棘苔龟 | the state-3 off-screen boulder rain |

Items 2 and 9 are the two additions beyond the research's own OQ4 recommendation (the 剧毒蟾蜍 death cloud and the 爆弹水母 death blast); both carry a creature's primary threat, so they are built rather than blocked. Recorded here because the count 11 is frozen and a later plan may not add to it without reopening OQ4.

All eleven use `Commons.ModAsset.White_Mod` (D-48) and the `GiantDandelion_*` shape: spawned from `NPC.GetSource_FromAI()` inside `Main.netMode != NetmodeID.MultiplayerClient`, with every dust call inside `if (!Main.dedServ)` (D-35/D-55).

**Effect blockers recorded rather than built:**

| System / effect | Rows | Disposition |
| --- | --- | --- |
| Ink-cloud lingering rendering | 覆藻章鱼, 大型覆藻章鱼 | the damage is built (item 8); the design's lingering ink shader/rendering is a blocker |
| Spiny Moss Court morale / command buff system | 枯木活化士兵, 王庭号令者 | the design's 意志高涨 bonus (防御 +4, 攻击 +35%, 移速 +15%) and its break-on-commander-death rule need a court-wide system the phase does not build (D-46) |
| Valley egg system | 布罗迪蝇蜓 | the 森雨幽谷 egg-breaking spawn of the small variant needs the Valley egg system (D-46) |
| Disguised-hazard visual system | 阿萨辛覆盘子, 蛇行苔 | the trigger/attack mechanics are built; the 覆盘子 / 环境植物 disguise visuals need the Valley hazard system (D-46) |
| Water-surface tuning | 水黾 | the surface predicate is derived; see the tuning note in §5 |
| Cross-creature hostility | 碧灵鮟鱇 (and every predator row with a "attacks other aquatic creatures" clause) | tML has no NPC-versus-NPC aggro hook, so the creature attacks the nearest **player**; the design's inter-creature hostility is not modelled (see §13) |

## 8. Mini-boss depth call (OQ5)

**OQ5 resolution: 大型荆棘苔龟 (`LargeMossyThornTurtle`) gets the full four-state machine; no state-3 blocker is recorded.** The design row is complete enough (exact frame budgets and an explicit reflect formula pass D-47's "design row complete enough" test) and the wave budget allows it:

1. **State 1 缩壳** — the shell takes 20% of the incoming damage as a reflect clamped to 2–20; exits either after 100 frames without being hit or after 320 frames in the shell, then goes to state 2.
2. **State 2 伸出头 + 震击地板** — a floor slam that emits the shockwave (item 10); repeated once after 120 frames, then 120 more frames take it to state 3.
3. **State 3 飞天下坠** — the shell flight ignores tile collision, lands with one more floor slam, then drops three evenly spaced boulders (item 11) from off-screen above, then goes to state 4.
4. **State 4 爬行 240 帧** — head out, crawling away from the player at 2 tiles/s for 240 frames, then back to state 1 facing away.

Defence is 999 while retracted and 10 normally; the design's 减伤 20 becomes a single `FinalDamage` scale on the normal state only.

## 9. Gate placement (OQ3)

**OQ3 resolution: the Phase 4 gate is a NEW file — `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1` — reading the shared `03-BIOLOGY.json`.** `.planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1` is left **byte-identical** (verified with `git diff --stat` over that path during plan 04-01's Task 1), so the Phase 3 close-out chain still reproduces end to end and the Phase 3 gate still prints `OK(0): phase3 tranche = 5 / 5 (rows=31)` and `OK: implemented classes = 5 / 5` under `-RequireAll`.

The new gate mirrors the Phase 3 conventions — 100% ASCII (PowerShell 5.1 reads a BOM-less script as the system ANSI code page), `[IO.File]::ReadAllText` for the UTF-8 JSON/Markdown inputs, `Get-ChildItem -Recurse` (never `git ls-files`) for class resolution because a class created in the same task is still untracked, exit codes 0 OK / 1 invariant failure / 2 missing input, and a byte-level UTF-8 BOM guard over the phase's own change set (invariant 13) — and carries the 13 invariants of the plan: frozen counts, the frozen 21-id in-scope set, the two deferred rows, the per-row artwork blocker, matrix-wide class resolution plus the in-scope `code_complete`/`internal_name` biconditional, the guarded-class structural checks, item-type resolution over the whole `Sources/Modules/Yggdrasil` tree, the no-placeholder-art guard, `-RequireAll`, Markdown parity and the BOM guard.

**Alternative considered and not taken:** extending the Phase 3 script in place behind an opt-in switch. It was not taken because the Phase 3 close-out record freezes that script's phase-3 invariants (T-04-06), and an in-place extension would put the whole Phase 3 chain at risk to add Phase 4 assertions.

**Red/green proof.** The gate was authored before its class (Step A + B before Step D), so plan 04-01 ran it red — `FAIL(2)` naming the missing `JadeSpiritAnglerfish.cs` — and then green — `OK(0): phase4 in-scope set = 21 rows (rows=31)` / `OK: reconciled rows = 1 / 21` / `OK: guarded classes = 1` / `OK: UTF-8 BOM check passed (5 files).` The transition is real, not an artefact of a gate that was only ever run after everything landed.

## 10. Conservative defaults register (D-22/D-23/D-54/D-59)

Where the design supplies no value, the conservative default below is used and recorded here (D-54/D-59). Values the design does supply are not listed; they come from the snapshot (D-25).

| Default | Value | Rows | Reason |
| --- | --- | --- | --- |
| `NPC.rarity` (the plan prose's `NPC.rare`) | `ItemRarityID.White` (= 0, the field's own default) | all except 大型覆藻章鱼 | The design's trailing 稀有度 cell is empty and 普通 is the 类型 cell, so no design rarity exists. `NPC.rarity` is the engine's **only** NPC rarity field (Lifeform Analyzer); the plan's `NPC.rare` does not exist in this tML build and was corrected to `NPC.rarity` — the same Rule 1 correction Phase 3 recorded. |
| `NPC.rarity` for 稀有 | `ItemRarityID.LightPurple` (= 6) | 大型覆藻章鱼 | The design's 类型 cell reads 稀有; Phase 3's 巨树人 precedent maps 稀有 to `ItemRarityID.LightPurple`. |
| Land spawn weight | `KelpCurtainSpawnConditions.LandWeight = 1f` (band 0.75f–1.5f) | every land row | The design gives no weight; the band is calibrated against `NPCSpawnManager`'s 0.1f slime weights and Phase 3's 0.75f–1.5f band. |
| Water spawn weight | `KelpCurtainSpawnConditions.WaterWeight = 0.75f` (band 0.5f–1f) | every water row | as above; the design's 深水/浅水 split is a condition, not a weight. |
| Rare / water-bottom weight | `KelpCurtainSpawnConditions.RareWaterBottomWeight = 0.35f` (band 0.25f–0.5f) | 碧灵鮟鱇, 放射虫, 大型覆藻章鱼 | The design makes the lake-bottom predators the rarest aquatic creatures (权重是全部水生生物最低). |
| Mini-boss weight | `KelpCurtainSpawnConditions.MiniBossWeight = 0.1f` (band ≤ 0.1f) | 大型荆棘苔龟 | 非常稀有的MiniBoss. |
| Drop denominators | 25% → 4 · 12.5% → 8 · 6.7% → 15 · 9% → 11 · 50% → 2 · 33% → 3 · guaranteed → 1 | §6.1 rows | `ItemDropRule.Common` takes a denominator, and these are the exact reciprocals of the design's percentages. |
| 碧灵鮟鱇 hit box | `46x28` | 碧灵鮟鱇 (and the same exercise for every art-missing class) | No sprite exists to measure, so a conservative mid-sized aquatic box is used. It is the first value to revisit when the art arrives. |
| 碧灵鮟鱇 reveal range | `26` tiles | 碧灵鮟鱇 | The design only says 无法在远处被观察 and gives no number. |
| 碧灵鮟鱇 dash/duration/cooldown | `11f` dash speed, `45` frames, `300`-frame re-dash cooldown | 碧灵鮟鱇 | 破隐第一次冲刺速度非常快且伤害非常高 with no numbers; the cooldown keeps the dash attached to the first moment of a reveal. |
| 碧灵鮟鱇 floor settle band | `3` tiles, 24-tile floor probe | 碧灵鮟鱇 (shared shape for the other 水底 rows) | 尝试回到水底 with no tolerance given. |
| 酸性毒液 → `BuffID.Venom` | vanilla Venom (id 70, 10 s in the design's usage) | 剧毒蟾蜍 (attacks + death cloud) | **Recorded deviation, not a design match.** `BuffID.AcidVenom` does **not** exist in this tML build (reflection over `Terraria.ID.BuffID` in `tModLoader.dll`: `AcidVenom` MISSING), and creating a `ModBuff` would require an unapproved icon (D-48/D-51), so the closest vanilla debuff stands in. |
| 束缚 → `BuffID.Webbed` | vanilla Webbed (id 149) | 蛇行苔, 大型覆藻章鱼 | The design's 束缚 has no vanilla member of its own; Webbed is the engine's own immobilise debuff. |
| 潮湿 / 窒息 / 黑暗 / 缓慢 | `BuffID.Wet` (103), `BuffID.Suffocation` (68), `BuffID.Darkness` (22), `BuffID.Slow` (32) | 水黾, 幽光蝾螈, 覆藻章鱼 / 大型覆藻章鱼, 覆藻章鱼 | Verified present in **this** tML build by reflection over `Terraria.ID.BuffID` (all four resolved to the ids shown), so no substitution is needed. `BuffID.Poisoned` (20) and `BuffID.Confused` (31) were verified the same way. |

**Rule 1 API-name correction applied by plan 04-01.** The plan prose writes `NPC.rare = ItemRarityID.White;`. `NPC` exposes no `rare` member in this tML build — the engine's field is `NPC.rarity` — so the class writes `NPC.rarity = ItemRarityID.White;` and records the correction here rather than shipping a compile break. This is the same correction Phase 3 made for the same reason.

**Verified-by-reflection note.** Every `BuffID` member named above was resolved against `Terraria.ID.BuffID` in the installed `tModLoader.dll` (values in parentheses are the resolved buff ids); `AcidVenom` is the single name that does not resolve, which is what forces the mapping deviation in the 酸性毒液 row.

## 11. Runtime verification (D-21)

The client and dedicated-server checks this phase cannot run offline are **not** executed here and are not claimed as passing. Plan 04-09 turns this section into `.planning/phases/04-remaining-ordinary-monsters/04-UAT.md` as the D-21 bundle: per-row spawn isolation inside Yggdrasil and its absence in an ordinary world, the 水底 / 水面上 / land predicates on a real lake, the 碧灵鮟鱇 stealth-reveal-dash-return feel and its two drops, the mini-boss state cycle, the dedicated-server run (no graphics access, no `Main.LocalPlayer` dependency) and the clean-load check that the art-missing classes resolve their fallback texture without aborting mod loading.

The Phase 4 gate and `dotnet build /p:Configuration=Release /p:WarningLevel=0` are the phase's offline evidence; they cannot observe a missing texture at load time, a spawn leak in the main world, or any AI feel.

## 12. Localization (D-20)

Localization stays out of scope by user directive. No in-game `OutputLocalizationHjsonItem` exporter run, no HJSON key created, no HJSON key renamed, no key deleted and no hand-edit of any `.hjson` file. Every Phase 4 class declares `public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;` so the keys can be generated later, and every in-scope row's blocker array ends with the canonical element `localization deferred (D-20); runtime verification outstanding (D-21)`.

## 13. Deferred registry

| Category | Item | Where it is tracked |
| --- | --- | --- |
| Hardmode designs | 枯萎之种 (`bio-out-of-phase-withered-seed`), 枯木人卫士 (`bio-out-of-phase-withered-tree-guardian`) — `deferred: true` under V2-HARD-01, never implemented in this phase | `03-BIOLOGY.json` `deferred_reason`; gate invariant 4 |
| Boss / special encounters | Klein Snake (`bio-out-of-phase-kelp-snake`), Giant Winged Dragon, 吸血魔毯 (`bio-death-jade-lake-vampire-mat`) — Phase 7 | `03-BIOLOGY.json` `phase: 7` rows |
| Region-level spawn predicates | 森雨幽谷 / 刺苔庭园 / 亡碧湖 sub-biome or tile predicates — Phases 5–6 terrain work (D-52) | matrix blocker elements; §5 |
| Unimplemented systems | Spiny Moss Court morale/command buff; Valley egg system; Valley disguised-hazard visuals; capture items for 装甲虾 / 爆弹水母 / 小格普螺 (D-46/D-58) | matrix blocker elements; §7 |
| Cross-creature hostility | the design's "attacks other aquatic creatures" clauses (e.g. 碧灵鮟鱇's, 覆藻章鱼's) — tML has no NPC-versus-NPC aggro hook, so only the player is targeted | §7 effect blockers |
| Absent drop materials | 毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 枯木碎块, 干涸心脏, 大型荆棘苔龟's TBD list — item scope, no type reference written (D-58) | matrix blocker elements; §6.2 |
| Missing art | 26 `ModNPC` sprites + 11 projectile sprites — blocker only, never placeholder art (D-48/D-51) | matrix `artwork:` elements; §3/§4 |
| Localization | the whole phase (D-20) | §12 |
| Runtime verification | the whole phase (D-21) | §11, and `04-UAT.md` in plan 04-09 |

## 14. Phase 4 close-out (reserved)

Reserved for plan 04-09, which reconciles the remaining twenty in-scope rows to `code_complete: true`, records the full offline chain and the D-21 bundle and appends the mechanical blocker register. Nothing in §14 is written yet.
