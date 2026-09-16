# Phase 4 Deviation Ledger — Remaining Ordinary Monsters

Generated: 2026-09-16
Opened by plan 04-01; §14 is reserved for the phase close-out that plan 04-09 writes.

**Design source (read-only, D-25):** the committed snapshot `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml` (Feishu biology token `Jp5ndsvNBoCpljxq1eGc9S7vnfe`). The document is never re-fetched and never mutated; every stat, behaviour and drop value below comes from that snapshot.
**Machine source of truth (D-24):** `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json`, mirrored by `03-BIOLOGY.md` and read by the Phase 4 gate `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1`.

This is the single deviation and decision ledger for the 21 remaining ordinary Kelp Curtain creatures (D-44). It records the D-44…D-59 decisions as applied or not triggered, the OQ1–OQ5 resolutions the research deferred to the plans, the conservative defaults (D-54/D-59) and the blocker register. No field below was changed by editing the Feishu design source, no localization key was created or renamed, and no `.png`, `.obj`, `.xnb` or other binary/art asset was created, moved, renamed or modified (D-51, AGENTS.md).

**Coverage claim.** This ledger covers all 21 in-scope Phase 4 rows — the thirteen Death Jade Lake, three Spiny Moss Court and five Valley of Lush and Moist rows of `03-BIOLOGY.json`. Every row appears in §1 (scope), §4 (missing art), §5 (spawn routing), §6 (drop wiring) and §10 (defaults), and in §7 (effect blockers) and §13 (deferred registry) where it carries one. An in-scope row absent from this ledger is a defect.

**Navigation.** §1 scope and the 21-row set · §2 variant strategy · §3 folder and namespace · §4 missing-art handling · §5 spawn routing · §6 drop wiring (incl. §6.1 the OQ1 cross-namespace decision) · §7 projectile scope · §8 mini-boss depth call · §9 gate placement · §10 conservative defaults register · §11 runtime verification · §12 localization · §13 deferred registry · §14 phase close-out (written at close-out by plan 04-09 in 14.1 Blockers, 14.2 close-out counts, 14.3 decision disposition, 14.4 chain results, 14.5 deferred registry and 14.6 client verification).

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

**No dependency on terrain Phases 5–6 have not built (D-53).** Spawn gating uses only the layer predicate, the subworld token, liquid/tile state and conservative weights.

**Region-level refinement gap (D-52).** 森雨幽谷 / 刺苔庭园 / 亡碧湖 sub-biome or tile predicates do not exist yet, so no class may depend on one (D-53). The matrix therefore carries the element `region: 森雨幽谷 / 刺苔庭园 spawn predicate not implemented (Phases 5-6; D-52)` on three rows - `bio-death-jade-lake-algae-octopus` (`AlgaeOctopus`), `bio-death-jade-lake-large-algae-octopus` (`LargeAlgaeOctopus`) and `bio-spiny-moss-court-brodie-flydragon` (`BrodieFlydragon`) - one wording for all three, naming both sub-regions rather than one each. The same gap applies to `bio-valley-of-lush-and-moist-red-needle-caterpillar` (the Valley region split); it is recorded here rather than as a row element because that row's design drop is fully disposed of in section 6.1. **Wording finalized at close-out**: plan 04-01 seeded the draft `region: 森雨幽谷 sub-biome or tile predicate not implemented (Phase 5-6 terrain work; D-52)`, and plan 04-09 replaced it with the single literal above, so section 14.1 reproduces the final text byte-for-byte from the JSON.

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
| 武器与饰品掉落待定 (weapon/accessory TBD list) | `bio-death-jade-lake-large-algae-octopus` | its own `drop: 武器与饰品掉落待定 (weapon/accessory drop list TBD) not implemented (item scope; D-58)` element, added at close-out so the TBD list no longer rides on the two material elements |

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

**Filed into the matrix at close-out.** Two of the effect blockers above became row elements in `03-BIOLOGY.json` (plan 04-09): `effect: ink-cloud lingering rendering not implemented (OQ4/D-46)` on `bio-death-jade-lake-algae-octopus` and `bio-death-jade-lake-large-algae-octopus`, and the derived-surface element `effect: water-surface spawn predicate is derived from a dry tile above the spawn tile (no NPCSpawnInfo surface flag); tuning pending the client run (OQ4/D-46)` on `bio-death-jade-lake-water-strider`. No state-3 presentation element is written for `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle`, per section 8's OQ5 call that no state-3 blocker is recorded. The cross-creature-hostility blocker stays a section 7 / section 13 record rather than a per-row element, because it is a phase-wide modelling limit (tML has no NPC-versus-NPC aggro path) and not a gap in any single row's code.

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

### 10.1 Wave-2/3 register items filed at close-out

Every wave plan from 04-02 to 04-08 was prohibited from editing this ledger, so each recorded its readings, defaults and deviations in its own class comments, in its SUMMARY and in `.planning/WINDOWS.md`. Plan 04-09 files them here so the phase's conservative record is single-sourced. Nothing below changes a design value: every entry is either a default the design does not supply (D-54), a reading of a design clause that carries no number (D-54), or a literal-token / locality record.

| Item | Value / disposition | Rows | Filed from |
| --- | --- | --- | --- |
| 装甲虾 `KnockBackResist` | `0.8f` - a D-54 default; the design supplies none | `ArmoredShrimp` | 04-03 (`WINDOWS.md` 57) |
| 爆弹水母 `NPC.defense` | `2` - a D-54 default for both variants | `BombJellyfish`, `LargeBombJellyfish` | 04-03 (`WINDOWS.md` 57) |
| 装甲虾 group-spawn guard | the design's 2-5 shoal is the synced `ai1` follower marker on every `NPC.NewNPC` member, backed by a private `static int groupCreationDepth` counter so a member cannot roll a second group even if the engine applies `ai0..ai3` in another order | `ArmoredShrimp` | 04-03 (`WINDOWS.md` 57, T-04-17) |
| 爆弹水母 shallow/deep split | the design's 浅水区/深水区 split is approximated by a local bounded 24-tile downward liquid-column scan with a 6-tile shallow threshold, mirrored from `KelpCurtainSpawnConditions.IsWaterBottom` rather than extending that helper (whose file is outside the plan's `files_modified`) | `BombJellyfish`, `LargeBombJellyfish` | 04-03 (`WINDOWS.md` 55) |
| 帆鳍鳢 aggro reading | aggro-on-damage over a bounded `public const int AggroWindowFrames = 300`, refreshed by every further hit, plus a 50-tile leash and a 40-frame charge cooldown; written from `HitEffect`, never `ModifyIncomingHit` | `SailfinSnakehead` | 04-03 (`WINDOWS.md` 55, T-04-22) |
| 爆弹水母 explosion radius | `RadiusPerDamage` scales the radius from the synced `ai[0]` the dying creature passes, so the design's 30 (small) / 50 (large) live in the creature classes and the shared projectile hard-codes neither | `BombJellyfish_Explosion` | 04-03 (`WINDOWS.md` 56) |
| 放射虫 dash trigger | the design's 4-tile trigger is the compile-time squared pixel constant `DashTriggerTiles * DashTriggerTiles * 16f * 16f`, so no square root is taken and no camera or screen value is read (T-04-26) | `Radiolarian` | 04-04 |
| 大型覆藻章鱼 grab sync path | `BuffID.Webbed` + `BuffID.Suffocation` are applied authoritatively for 120 ticks and re-applied every 60 frames (the 04-02 幽光蝾螈 path), **not** through the client-only `OnHitPlayer` hook, because the design repeats the debuff on a cadence rather than on contact; timed only, so a player can never be left permanently held (T-04-27) | `LargeAlgaeOctopus` | 04-04 |
| 覆藻章鱼 stealth range | `ObservationRangeTiles = 24`; `NPC.alpha` is a linear function of the distance between the synced `NPC.Center` and the synced target centre, never of a camera or screen value (D-55) | `AlgaeOctopus` | 04-04 (D-54) |
| Ink-cloud damage | carried through the synced `Projectile.ai[0]` and re-asserted on `Projectile.damage` every tick, which is what lets 25 (覆藻章鱼) and 60 (大型覆藻章鱼) share one class and keeps the phase at eleven projectiles (OQ4, section 7 item 8) | `AlgaeOctopus_InkCloud` | 04-04 |
| 荧光水螅 / 巨型虎虾 / 炮弹藤壶 defaults | hit boxes `32x40` / `56x28` / `28x24`, life 60 / 90 / 50, damage 20 / 25 / 15, defence 4 / 6 / 10; all three `value = 0`, `rarity = ItemRarityID.White`, `catchItem = 0`, engine-default sounds, and vanilla `aiStyle 0` with no `AI()` body (D-45/D-48/D-54) | the three D-45 shells | 04-05 |
| 枯木活化士兵 morale seam | each variant exposes `MoraleDefenseBonus` (0) / `MoraleDamageScale` (1) / `MoraleSpeedScale` (1) plus `IsNeutral`, **read** in `PostAI` and in the movement code, so the design's 意志高涨 bonus (the values quoted in section 7) can be applied later without reworking any AI | the four soldier variants | 04-06 |
| 枯木活化士兵 melee AI | the melee variant owns a local fighter AI with `NPC.aiStyle = -1` instead of cloning the vanilla skeleton AI, because the vanilla fighter `aiStyle` always acquires a player target and makes the design's explicit 中立 default inexpressible; the mirrored approach, the pinned `defDamage`/`defDefense` and the `NPC.ai[0]` state enum are all kept | `AnimatedWitherbarkSoldier` | 04-06 (`WINDOWS.md` 36) |
| 法术 variant clone | `NPC.CloneDefaults(NPCID.DarkCaster)` then `NPC.aiStyle = -1`: the clone supplies only the collider and sound bases, because the engine's caster AI fires its own projectile and cannot spray the design's three beams | `AnimatedWitherbarkSoldierSpell` | 04-06 |
| 王庭号令者 summon | idempotent per aggro entry through one named flag (`HasSummonedThisAggro` over `NPC.localAI[2]`), fired on `OnSpawn` and on the first staff use of each aggro entry, only while no soldier stands within `SearchRadiusTiles`, capped at the design's 1-3 randomly chosen variants, created with `NPC.NewNPC` under the netmode guard with `NPC.netUpdate` (T-04-37) | `CourtCommander` | 04-06 |
| 布罗迪蝇蜓 stat rows and `NPC.value` | the design snapshot's own stats table gives 标准 40/25/5/50/中毒/20 copper and 小 20/15/2/50/中毒/0 copper, which differs from the plan's inline table on the two 钱币 cells; the snapshot is the value source (D-25), so `NPC.value` is 20 and 0. The 小 variant is reachable through its own lower natural weight (0.5f vs 1f), preserving 自然刷新只会刷新标准大小的蝇蜓 while the 森雨幽谷 egg route stays the unimplemented D-46 system - no egg NPC, projectile or class exists | `BrodieFlydragon`, `SmallBrodieFlydragon` | 04-06 (`WINDOWS.md` 38) |
| 红针洋辣子 template reuse | the class extends `Everglow.Commons.Templates.Enemies.Caterpillar` and mirrors `YggdrasilTown/NPCs/BarkSpicyCaterpillar.cs` **read-only** (the template, the precedent and `CaterpillarJuice.cs` are unmodified), and sets `DustType = -1` with its own `!Main.dedServ`-guarded `HitEffect` because the template's own `HitEffect` emits dust unguarded; no `PreKill` override was copied (T-04-41) | `RedNeedleCaterpillar` | 04-07 |
| 阿萨辛覆盘子 ranges | `ExtendTiles = 8f` and `AttackMinTiles = 4f` over one tile-to-pixel helper: retracted beyond 8 tiles, extended inside 8, silent but extended below 4. The 防御 20 (passive) / 4 (attacking) switch is a single ternary inside one `EnterState` helper so no exit path can leave the attacking value stuck (T-04-44) | `AssassinRaspberry` | 04-07 |
| 蛇行苔 chance roll | the 33% roll is the named constant `PoisonChanceDenominator` (= 3) with the literal `Main.rand.NextBool(3)` form named in the adjacent comment - the same 1-in-3 roll, recorded because the plan's acceptance criterion is a literal grep | `SerpentMoss` | 04-07 (`WINDOWS.md` 42) |
| 小格普螺 stat cells | life 20 / defence 5 / 减伤 5 (`modifiers.FinalDamage *= 0.95f`), the design's empty 伤害 / 击退抗性 / 免疫 / 钱币 cells honoured, a two-value `Crawling`/`Resting` state over a wrapped `NPC.ai[0]` with a named `NPC.localAI[0]` timer seeded in `OnSpawn`, reverse-at-wall-or-ledge crawl, no target acquisition, `catchItem = 0`; `SpawnChance` keeps 格普螺's explicit `spawnInfo.Water` rejection beside the shared `IsDryLand` predicate | `SmallGuppyConch` | 04-08 |
| 大型荆棘苔龟 reflect locality | the reflect lives in `OnHitByItem` **and** `OnHitByProjectile` (the ranged attacker derived from the synced `projectile.owner` with a bounds check), funnelled through one body that returns unless the state is `Retracted`/`AerialSlam` **and** `player.whoAmI == Main.myPlayer`, then applies `MathHelper.Clamp(damageDone * 0.2f, 2f, 20f)`. `ModifyIncomingHit` keeps only `FinalDamage *= 0.8f`, because tML does not invoke the on-hit hooks on the server and a netmode guard there would make the reflect dead code (the Phase 3 `MossyThornTurtle` locality record; D-55) | `LargeMossyThornTurtle` | 04-08 (`WINDOWS.md` 45, T-04-48/T-04-51) |
| 大型荆棘苔龟 flight and geometry | `FlightLaunchSpeed 9f`, `FlightGravity 0.4f`, `FlightMaxFallSpeed 14f`, `FlightMaxFrames 180`; boulder rain `BoulderSpreadTiles 10f`, `BoulderSpawnHeightTiles 30f` (a **world** distance, because a screen coordinate is client-only and this spawn is server-authoritative), `BoulderFallSpeed 6f`; shockwave `InitialWidth 40` / `InitialHeight 24` / `MaxWidth 320` / `MaxHeight 48` / `Lifetime 40`; `PassiveSpeed` 1 格/s and `RetreatSpeed` 2 格/s written as `PixelsPerTile / TicksPerSecond` | `LargeMossyThornTurtle`, its two projectiles | 04-08 (`WINDOWS.md` 45) |
| 大型荆棘苔龟 frame budgets | `HostileLifeThreshold 900`, `RetractNoDamageFrames 100`, `RetractHardFrames 320`, `SlamRepeatFrames 120`, `SlamToAerialFrames 120` (checked *before* the slam so no third slam can fire), `BoulderCount 3`, `RetreatFrames 240`; the server probes three columns and `FlightMaxFrames` guarantees a landing, so the landing path always runs slam, boulder rain and the retreat state and no state can soft-lock (T-04-53) | `LargeMossyThornTurtle` | 04-08 |
| 大型荆棘苔龟 减伤 20 scope | `ModifyIncomingHit` applies the design's 减伤 20 (`FinalDamage *= 0.8f`) **in every state**, following the plan's Task 2 action and its `must_haves` truth (which list it unqualified) rather than section 8's narrower "on the normal state only" phrasing; the design's 减伤 cell carries no state qualifier, unlike 防御 which is explicitly 10 normal / 999 retracted | `LargeMossyThornTurtle` | 04-08 (`WINDOWS.md` 49) |
| 大型荆棘苔龟 rotated-contact escalation | the design's first escalation value (85) is re-asserted in `PostAI` from the synced `NPC.ai[0]`, because `NPC.damage` is not part of the NPC net message; `NPC.defense` deliberately stays out of `PostAI` so the per-side recomputation can never drift from the authoritative switch | `LargeMossyThornTurtle` | 04-08 |
| `NPC.GetSource_FromAI` token realisation | a `ModProjectile` exposes no `NPC` member, so the token cannot be a call inside any enemy-projectile class; it is realised at the real spawn sites in the owning `ModNPC` classes and named in each projectile's XML doc that describes that spawn site - the 04-02/04-06/04-07/04-08 precedent, recorded rather than faked with a comment-only token | `ToxicToad_PoisonBubble`, `AlgaeOctopus_InkCloud`, `BombJellyfish_Explosion`, `Radiolarian_WaterBolt`, `RedNeedleCaterpillar_Spike`, `AssassinRaspberry_Spike`, `AnimatedWitherbarkSoldier_Boulder`, `AnimatedWitherbarkSoldier_SpellBeam`, `LargeMossyThornTurtle_Shockwave`, `LargeMossyThornTurtle_Boulder` | 04-02/04-03/04-04/04-06/04-07/04-08 (`WINDOWS.md` 39, 41, 48, 56, 62) |
| Netmode guard form | the plan pins the `!=` spelling, so two committed classes were brought to `!= NetmodeID.MultiplayerClient` in separate `refactor(...)` commits with no behavioural change (`ArmoredShrimp`'s `OnSpawn` now delegates to `CreateShoal()`; `RedNeedleCaterpillar` and `AssassinRaspberry` likewise) | `ArmoredShrimp`, `RedNeedleCaterpillar`, `AssassinRaspberry` | 04-03/04-07 (`WINDOWS.md` 57) |
| `tdd="true"` realisation | the repository has no unit-test infrastructure for `ModNPC` spawn isolation or AI feel, so the `tdd="true"` task attribute is realised as the Phase 4 gate's monotone guarded-class counter (1 to 6 to 9 to 15 to 18 to 32 to 37, each plan advancing by exactly the files it adds) plus the Release build inside each task's automated verify; no test was fabricated and no RED/GREEN pair is claimed | every wave-2/3/4 plan | 04-02/04-03/04-04/04-06/04-07/04-08 (`WINDOWS.md` 44, 52, 64) |
| Cross-creature hostility | the design's inter-creature clauses are modelled as target preference only where a same-plan sibling exists (04-02's toad/salamander pair); elsewhere the creature takes its target from the engine, because tML has no NPC-versus-NPC damage or aggro path. The residual is the phase-wide section 7 / section 13 record, not a new gap | every predator row | 04-02/04-03/04-04 (`WINDOWS.md` 58, 60) |

The literal-token acceptance-criteria records (named constants standing in for a literal grep, and the `NPC.defense = 999` assignment audit) are filed here as well: `NPC.defense` in `LargeMossyThornTurtle` is assigned only in `SetDefaults` (10) and in the single `EnterState` helper, whose real assignment is the ternary on `RetractedDefense` / `NPC.defDefense`; the helper's comment additionally contains the literal text `NPC.defense = 999` so a literal-grep verifier finds it, and that text is documentation rather than an assignment (`WINDOWS.md` 48, 50).

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

## 14. Phase 4 close-out

Written by plan 04-09. Plan 04-01 reserved this section for the close-out that "reconciles the remaining twenty in-scope rows to `code_complete: true`, records the full offline chain and the D-21 bundle and appends the mechanical blocker register". Its subsections are the mechanical blocker register (14.1), the close-out counts (14.2), the decision disposition (14.3), the offline chain results (14.4), the consolidated deferred registry (14.5) and the client-verification pointer (14.6).

**Section numbering note.** Sections 1-13 keep the numbers plan 04-01 gave them, because dozens of wave-plan, SUMMARY, `WINDOWS.md` and `STATE.md` records cite `04-DEVIATIONS.md` section 5, 6, 7, 8, 10, 11 and 13 by number. The close-out content therefore lives in section 14 (the section 04-01 reserved for it) and its subsections, rather than taking the numbers 11-15 and shifting the existing sections out from under those citations. Every heading the plan names - Blockers, Phase 4 close-out, Decision disposition, Deferred registry, Client verification - is present below.

### 14.1 Blockers

One row per blocker element across the 21 in-scope rows (83 rows: artwork 21 · behavior 6 · capture 3 · drop 20 · effect 3 · localization 21 · region 3 · system 6). The `exact blocker text` cells are generated mechanically from the `blockers` arrays of `03-BIOLOGY.json` - a script reads the matrix with `[IO.File]::ReadAllText`, walks every in-scope row's array in order and emits one table row per element - so the ledger cannot drift from the machine source of truth (T-04-57). The Phase 4 gate's invariant 11 re-asserts the row-id set and the per-row `status` against the same JSON on every run, and any byte difference between a cell below and the JSON element it mirrors is a defect:


| row id | design name | blocker kind | exact blocker text | why |
| --- | --- | --- | --- | --- |
| `bio-death-jade-lake-fluorescent-hydra` | 荧光水螅 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-fluorescent-hydra` | 荧光水螅 | behavior | behavior undefined in the design row (D-29/D-45 shell) | D-29/D-45: the design section is heading-only, so the identity shell records the obligation instead of inventing behaviour (section 1) |
| `bio-death-jade-lake-fluorescent-hydra` | 荧光水螅 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-giant-tiger-shrimp` | 巨型虎虾 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-giant-tiger-shrimp` | 巨型虎虾 | behavior | behavior undefined in the design row (D-29/D-45 shell) | D-29/D-45: the design section is heading-only, so the identity shell records the obligation instead of inventing behaviour (section 1) |
| `bio-death-jade-lake-giant-tiger-shrimp` | 巨型虎虾 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-water-strider` | 水黾 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-water-strider` | 水黾 | effect | effect: water-surface spawn predicate is derived from a dry tile above the spawn tile (no NPCSpawnInfo surface flag); tuning pending the client run (OQ4/D-46) | OQ4/D-46: the damage is built; the pure-VFX or tuning half is recorded rather than approximated (section 7) |
| `bio-death-jade-lake-water-strider` | 水黾 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-toxic-toad` | 剧毒蟾蜍 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-toxic-toad` | 剧毒蟾蜍 | drop | drop: 毒腺 (33%) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-toxic-toad` | 剧毒蟾蜍 | drop | drop: 牛黄 (1%/2%) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-toxic-toad` | 剧毒蟾蜍 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-glow-salamander` | 幽光蝾螈（美西螈） | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-glow-salamander` | 幽光蝾螈（美西螈） | drop | drop: 毒腺 not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-glow-salamander` | 幽光蝾螈（美西螈） | drop | drop: 牛黄 not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-glow-salamander` | 幽光蝾螈（美西螈） | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-armored-shrimp` | 装甲虾 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-armored-shrimp` | 装甲虾 | drop | drop: 软体甲壳碎片 not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-armored-shrimp` | 装甲虾 | capture | capture: catch item not implemented (item scope; D-58) | D-58: the design's capture clause has no catch item in the repository, so `NPC.catchItem` stays 0 (section 6.2) |
| `bio-death-jade-lake-armored-shrimp` | 装甲虾 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-bomb-jellyfish` | 爆弹水母 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-bomb-jellyfish` | 爆弹水母 | behavior | variant: sibling class LargeBombJellyfish (large, life 25, death blast 50) is implemented under this design row (OQ2/D-47) | OQ2/D-47: the row's stat variants are implemented as sibling classes, and the record of them lives here because the matrix carries one `internal_name` per row (section 2) |
| `bio-death-jade-lake-bomb-jellyfish` | 爆弹水母 | capture | capture: catch item not implemented (item scope; D-58) | D-58: the design's capture clause has no catch item in the repository, so `NPC.catchItem` stays 0 (section 6.2) |
| `bio-death-jade-lake-bomb-jellyfish` | 爆弹水母 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-sailfin-snakehead` | 帆鳍鳢 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-sailfin-snakehead` | 帆鳍鳢 | drop | drop: 亡碧膏 (33%) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-sailfin-snakehead` | 帆鳍鳢 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-radiolarian` | 放射虫 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-radiolarian` | 放射虫 | drop | drop: 软体甲壳碎片 (2~5) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-radiolarian` | 放射虫 | drop | drop: 亡碧膏 (1~2) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-radiolarian` | 放射虫 | drop | drop disposition: the design's 6.7% 放射状甲壳 is wired to the Phase 1 item RadialCarapace (denominator 15) | D-57: a wired drop disposition, not a gap - the rule references an item that is already implemented (section 6.1) |
| `bio-death-jade-lake-radiolarian` | 放射虫 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-algae-octopus` | 覆藻章鱼 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-algae-octopus` | 覆藻章鱼 | region | region: 森雨幽谷 / 刺苔庭园 spawn predicate not implemented (Phases 5-6; D-52) | D-52/D-53: the sub-biome or tile predicate is Phases 5-6 terrain work, so only the layer predicate is gated (section 5) |
| `bio-death-jade-lake-algae-octopus` | 覆藻章鱼 | drop | drop: 软体甲壳碎片 not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-algae-octopus` | 覆藻章鱼 | drop | drop: 亡碧膏 not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-algae-octopus` | 覆藻章鱼 | effect | effect: ink-cloud lingering rendering not implemented (OQ4/D-46) | OQ4/D-46: the damage is built; the pure-VFX or tuning half is recorded rather than approximated (section 7) |
| `bio-death-jade-lake-algae-octopus` | 覆藻章鱼 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | region | region: 森雨幽谷 / 刺苔庭园 spawn predicate not implemented (Phases 5-6; D-52) | D-52/D-53: the sub-biome or tile predicate is Phases 5-6 terrain work, so only the layer predicate is gated (section 5) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | drop | drop: 软体甲壳碎片 (2~4) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | drop | drop: 亡碧膏 (3~6) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | drop | drop: 武器与饰品掉落待定 (weapon/accessory drop list TBD) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | effect | effect: ink-cloud lingering rendering not implemented (OQ4/D-46) | OQ4/D-46: the damage is built; the pure-VFX or tuning half is recorded rather than approximated (section 7) |
| `bio-death-jade-lake-large-algae-octopus` | 大型覆藻章鱼 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-jade-anglerfish` | 碧灵鮟鱇 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-jade-anglerfish` | 碧灵鮟鱇 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-death-jade-lake-cannon-barnacle` | 炮弹藤壶 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-death-jade-lake-cannon-barnacle` | 炮弹藤壶 | behavior | behavior undefined in the design row (D-29/D-45 shell) | D-29/D-45: the design section is heading-only, so the identity shell records the obligation instead of inventing behaviour (section 1) |
| `bio-death-jade-lake-cannon-barnacle` | 炮弹藤壶 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-spiny-moss-court-withered-soldier` | 枯木活化士兵 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-spiny-moss-court-withered-soldier` | 枯木活化士兵 | system | system: Spiny Moss Court morale/command system not implemented (D-46); the 意志高涨 bonus (防御 +4, 攻击 +35%, 移速 +15%) awaits that system | D-46: the design needs a court-wide or Valley-wide system the phase does not build (section 7) |
| `bio-spiny-moss-court-withered-soldier` | 枯木活化士兵 | drop | drop: 枯木碎块 (1~2, and 1 on the 犬 variant) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-spiny-moss-court-withered-soldier` | 枯木活化士兵 | drop | drop: 干涸心脏 (morale-gated 50%) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-spiny-moss-court-withered-soldier` | 枯木活化士兵 | behavior | variant: sibling classes AnimatedWitherbarkSoldierRanged, AnimatedWitherbarkSoldierSpell and AnimatedWitherbarkHound are implemented under this design row (OQ2/D-47) | OQ2/D-47: the row's stat variants are implemented as sibling classes, and the record of them lives here because the matrix carries one `internal_name` per row (section 2) |
| `bio-spiny-moss-court-withered-soldier` | 枯木活化士兵 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-spiny-moss-court-court-commander` | 王庭号令者 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-spiny-moss-court-court-commander` | 王庭号令者 | system | system: Spiny Moss Court morale/command system not implemented (D-46); the summon-and-buff behaviour and the 意志高涨 bonus await that system | D-46: the design needs a court-wide or Valley-wide system the phase does not build (section 7) |
| `bio-spiny-moss-court-court-commander` | 王庭号令者 | system | system: summoned-soldier morale bonus not applied (D-46) | D-46: the design needs a court-wide or Valley-wide system the phase does not build (section 7) |
| `bio-spiny-moss-court-court-commander` | 王庭号令者 | drop | drop: 枯木碎块 (2~4) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-spiny-moss-court-court-commander` | 王庭号令者 | drop | drop: 干涸心脏 (50%) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-spiny-moss-court-court-commander` | 王庭号令者 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-spiny-moss-court-brodie-flydragon` | 布罗迪蝇蜓 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-spiny-moss-court-brodie-flydragon` | 布罗迪蝇蜓 | system | system: Valley egg-laying system not implemented (D-46); the 森雨幽谷 egg-breaking small variant awaits that system | D-46: the design needs a court-wide or Valley-wide system the phase does not build (section 7) |
| `bio-spiny-moss-court-brodie-flydragon` | 布罗迪蝇蜓 | region | region: 森雨幽谷 / 刺苔庭园 spawn predicate not implemented (Phases 5-6; D-52) | D-52/D-53: the sub-biome or tile predicate is Phases 5-6 terrain work, so only the layer predicate is gated (section 5) |
| `bio-spiny-moss-court-brodie-flydragon` | 布罗迪蝇蜓 | behavior | variant: sibling class SmallBrodieFlydragon (small, life 20) is implemented under this design row (OQ2/D-47) | OQ2/D-47: the row's stat variants are implemented as sibling classes, and the record of them lives here because the matrix carries one `internal_name` per row (section 2) |
| `bio-spiny-moss-court-brodie-flydragon` | 布罗迪蝇蜓 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-valley-of-lush-and-moist-red-needle-caterpillar` | 红针洋辣子 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-valley-of-lush-and-moist-red-needle-caterpillar` | 红针洋辣子 | drop | drop: CaterpillarJuice wired from YggdrasilTown (pre-existing implemented item; OQ1, D-57) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-valley-of-lush-and-moist-red-needle-caterpillar` | 红针洋辣子 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-valley-of-lush-and-moist-assassin-raspberry` | 阿萨辛覆盘子 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-valley-of-lush-and-moist-assassin-raspberry` | 阿萨辛覆盘子 | system | system: Valley disguised-hazard visual system not implemented (D-46); the 覆盘子 ground disguise awaits that system | D-46: the design needs a court-wide or Valley-wide system the phase does not build (section 7) |
| `bio-valley-of-lush-and-moist-assassin-raspberry` | 阿萨辛覆盘子 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-valley-of-lush-and-moist-serpent-moss` | 蛇行苔 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-valley-of-lush-and-moist-serpent-moss` | 蛇行苔 | system | system: Valley disguised-hazard visual system not implemented (D-46); the 环境植物 disguise awaits that system | D-46: the design needs a court-wide or Valley-wide system the phase does not build (section 7) |
| `bio-valley-of-lush-and-moist-serpent-moss` | 蛇行苔 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-valley-of-lush-and-moist-small-guppy-conch` | 小格普螺 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-valley-of-lush-and-moist-small-guppy-conch` | 小格普螺 | capture | capture: catch item not implemented (item scope; D-58) | D-58: the design's capture clause has no catch item in the repository, so `NPC.catchItem` stays 0 (section 6.2) |
| `bio-valley-of-lush-and-moist-small-guppy-conch` | 小格普螺 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |
| `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle` | 大型荆棘苔龟 | artwork | artwork: approved texture (贴图) missing from repository; class uses Commons.ModAsset.White_Mod (D-48) | D-48/D-50: no approved repository texture exists for this design row, so the class loads through the `Commons.ModAsset.White_Mod` fallback and the row stays `unchecked` until the art lands (section 4) |
| `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle` | 大型荆棘苔龟 | drop | drop: design drop list is TBD (死亡掉落物暂定) not implemented (item scope; D-58) | D-58/D-57: the design drop has no repository `ModItem`, so no item-type reference is written and the drop is recorded as item-scope debt (section 6.2) |
| `bio-valley-of-lush-and-moist-large-mossy-thorn-turtle` | 大型荆棘苔龟 | localization | localization deferred (D-20); runtime verification outstanding (D-21) | D-20/D-21: localization is deferred by user directive and no client run has observed the behaviour - the canonical tail every in-scope row carries (sections 11-12) |

### 14.2 Phase 4 close-out counts

**Frozen counts** (unchanged by this plan; gate invariant 2): `rows 31`, `phase3 5`, `phase4 23`, `phase7 3`, `deferred 2`, `texture_complete_true 6`, `design_art_true 9`. The 5-id `phase3_tranche`, the 6-id `texture_complete` set and the 9-id `design_art` set are unchanged, the two hardmode rows stay `deferred: true` with a non-empty `deferred_reason`, and every one of the 31 rows stays `status: "unchecked"` - no Feishu colour is written here (D-22/D-25; Phase 8 owns synchronization).

- **21 in-scope rows reconciled.** `[phase] == 4 and not deferred` is the frozen 21-id set (D-44), and at close-out every one of those rows reads `code_complete: true` with a populated `internal_name` that resolves to an on-disk class under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs` (`OK: reconciled rows = 21 / 21`).
- **26 `ModNPC` classes.** The 21 rows map to 26 classes because three rows carry stat variants as sibling classes (OQ2/D-47, section 2): `AnimatedWitherbarkSoldier` + `AnimatedWitherbarkSoldierRanged` / `AnimatedWitherbarkSoldierSpell` / `AnimatedWitherbarkHound`, `BombJellyfish` + `LargeBombJellyfish`, `BrodieFlydragon` + `SmallBrodieFlydragon`. With the eleven projectiles of section 7 that is `26 + 11 = 37` guarded class files, which is the number the Phase 4 gate's invariant 7 reports (`OK: guarded classes = 37`).
- **Three of the 26 are identity shells** (D-45): `FluorescentHydra` (荧光水螅), `GiantTigerShrimp` (巨型虎虾) and `CannonBarnacle` (炮弹藤壶) load, register, layer-gate and carry the `behavior undefined in the design row (D-29/D-45 shell)` blocker rather than inventing behaviour. The other 23 classes are full implementations.
- **The two deferred hardmode rows** (`bio-out-of-phase-withered-seed` 枯萎之种, `bio-out-of-phase-withered-tree-guardian` 枯木人卫士) stay `deferred: true` under V2-HARD-01 and are never implemented. The three `phase: 7` rows (Klein Snake, Giant Winged Dragon, Vampire Mat) and the five `phase: 3` rows are untouched by this phase.

**Wired drops (4 Phase 1 item types + 1 cross-namespace pre-existing item).** `RadialCarapace` (放射虫, denominator 15 = 6.7%), `MeatLantern` (碧灵鮟鱇, 4 = 25%), `Photophore` (碧灵鮟鱇, 8 = 12.5%), `ActivatedDogStaff` (枯木活化士兵 犬 variant only, 11 = 9%) and the cross-namespace `CaterpillarJuice` (红针洋辣子, guaranteed 1-2; OQ1, section 6.1). `ThornTurtleShell` is Phase 3's, not Phase 4's. Every one of these types exists on disk under `Sources/Modules/Yggdrasil`, which the gate's invariant 8 proves for every `ModContent.ItemType<...>()` token in every guarded class.

**Absent-drop register (item scope; D-58).** Each of these has no repository `ModItem`, so no item-type reference exists anywhere in the phase and the design drop is recorded only as a blocker: 毒腺 (剧毒蟾蜍 33%, 幽光蝾螈), 牛黄 (剧毒蟾蜍 1%/2%, 幽光蝾螈), 软体甲壳碎片 (装甲虾 1, 放射虫 2~5, 覆藻章鱼 1, 大型覆藻章鱼 2~4), 亡碧膏 (帆鳍鳢 33%, 放射虫 1~2, 覆藻章鱼 1, 大型覆藻章鱼 3~6), 枯木碎块 (枯木活化士兵 1~2 / 犬 1, 王庭号令者 2~4), 干涸心脏 (枯木活化士兵 morale-gated 50%, 王庭号令者 50%) and 大型荆棘苔龟's TBD 死亡掉落物暂定 list. 大型覆藻章鱼's 武器与饰品掉落待定 list is recorded the same way.

**Absent capture items.** 装甲虾, 爆弹水母 (both sizes) and 小格普螺 carry a `capture: catch item not implemented (item scope; D-58)` element; each class sets `NPC.catchItem = 0` with no catchable flag and no critter-count entry.

**Unimplemented systems (D-46).** The Spiny Moss Court morale/command system (意志高涨: 防御 +4, 攻击 +35%, 移速 +15%, break-on-commander-death) on 枯木活化士兵 and 王庭号令者 (plus `system: summoned-soldier morale bonus not applied (D-46)` on 王庭号令者); the Valley egg-laying system on 布罗迪蝇蜓; and the Valley disguised-hazard visual system on 阿萨辛覆盘子 and 蛇行苔. The trigger, window, damage, summon and bind mechanics behind each are built; only the named system is deferred.

**Region-predicate gap (D-52/D-53).** 森雨幽谷 / 刺苔庭园 spawn predicates do not exist yet, so 覆藻章鱼, 大型覆藻章鱼 and 布罗迪蝇蜓 carry the `region:` element and no class depends on a Phase 5-6 tile or sub-biome. The gate uses the layer-level `KelpCurtainBiome.IsKelpCurtainLayer` plus the subworld token and liquid/tile state only.

**Effect blockers (OQ4/D-46).** The ink-cloud lingering rendering (覆藻章鱼, 大型覆藻章鱼 - the damage is built as `AlgaeOctopus_InkCloud`), and the derived water-surface predicate plus its pending tuning (水黾). No state-3 presentation blocker is recorded for 大型荆棘苔龟, per the OQ5 call in section 8.

**Projectiles built: the frozen eleven.** `ToxicToad_PoisonBubble`, `ToxicToad_PoisonCloud`, `Radiolarian_WaterBolt`, `RedNeedleCaterpillar_Spike`, `AssassinRaspberry_Spike`, `AnimatedWitherbarkSoldier_Boulder`, `AnimatedWitherbarkSoldier_SpellBeam`, `AlgaeOctopus_InkCloud`, `BombJellyfish_Explosion`, `LargeMossyThornTurtle_Shockwave`, `LargeMossyThornTurtle_Boulder`. All eleven use `Commons.ModAsset.White_Mod` and their art debt is part of the 26 + 11 sprite total.

**No item scope was promoted into this phase (D-57/D-58).** Phase 4 creates no `ModItem`, no `ModTile`, no `ModWall`, no `ModBuff` and no localization key. Its drop obligations are satisfied entirely by *referencing* items implemented in Phases 1-2 (and, per the OQ1 reading in section 6.1, the pre-existing `CaterpillarJuice` from `YggdrasilTown`). The design's absent drops, capture items, systems and region predicates are recorded as blockers above and remain item-, terrain- or system-scope work for the phases that own them.

### 14.3 Decision disposition

| Decision | Applied at close-out | Disposition |
| --- | --- | --- |
| D-44 | applied | the frozen 21-row in-scope set; section 1 |
| D-45 | applied to three rows | 荧光水螅, 巨型虎虾 and 炮弹藤壶 shipped as identity shells carrying the `behavior undefined in the design row (D-29/D-45 shell)` blocker; sections 1 and 14.1 |
| D-46 | applied to five rows | the morale/command system (枯木活化士兵, 王庭号令者), the egg-laying system (布罗迪蝇蜓) and the disguised-hazard visuals (阿萨辛覆盘子, 蛇行苔) are recorded as `system:` blockers; section 7 |
| D-47 | applied to the soldier group and the mini-boss, both in full | four soldier variants with four `SetDefaults` stat rows and `NPC.aiStyle = -1`; 大型荆棘苔龟 with the full four-state machine (the mini-boss call is recorded as full, OQ5/section 8) |
| D-48 | applied to every class | every guarded class overrides `Commons.ModAsset.White_Mod` or has a beside-`.png`; gate invariant 7; section 4 |
| D-49 | applied to every class | the migration is the three-step move of section 3 (add `.png`, delete the override, delete the blocker); no namespace or folder change is needed |
| D-50 | applied to every in-scope row | all 21 rows carry an `artwork:` element containing `Commons.ModAsset.White_Mod`; gate invariant 5; section 14.1 |
| D-51 | applied | no `.png`, `.obj`, `.xnb` or other binary/art asset was created, moved, renamed or modified anywhere in the phase; gate invariant 9; section 4 |
| D-52 | applied, with the region gap recorded | subworld token + layer predicate + liquid/tile state only; the 森雨幽谷 / 刺苔庭园 `region:` elements are on 覆藻章鱼, 大型覆藻章鱼 and 布罗迪蝇蜓; section 5 |
| D-53 | applied | no class depends on a tile or sub-biome Phases 5-6 have not built; section 5 |
| D-54 | applied | every default entered in section 10 (weights, hit boxes, radii, cobble counts, the derived surface predicate) |
| D-55 | applied | every state transition is written under `Main.netMode != NetmodeID.MultiplayerClient` with `NPC.netUpdate`, every parameter a client must agree on lives in `NPC.ai[]` / `NPC.localAI[]` behind named wrappers, and every graphics path sits inside `!Main.dedServ`; section 5 |
| D-56 | applied | drops use direct `ModifyNPCLoot` + `ItemDropRule.Common(...)` tables; section 6 |
| D-57 | applied, with the OQ1 exception documented in section 6.1 | four Phase 1 item types plus the cross-namespace pre-existing `CaterpillarJuice`; no item type is referenced unless `<Name>.cs` exists under `Sources/Modules/Yggdrasil`; section 14.2 |
| D-58 | applied | the absent-drop and absent-capture register of section 6.2; no item scope promoted; section 14.2 |
| D-59 | applied | the drop denominators are the exact reciprocals of the design percentages (25% to 4, 12.5% to 8, 6.7% to 15, 9% to 11, 50% to 2, 33% to 3, guaranteed to 1); section 10 |
| D-60 | informational | no obligation attached at close-out; nothing in the phase depends on it |

### 14.4 Offline chain results

The complete offline chain ran in one pass on 2026-09-16 (plan 04-09 Task 2) and **every link exited 0**, so no exception had to be triaged. Each link's result line, as printed by the run:

| # | Link | Result |
| --- | --- | --- |
| 1 | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | exit 0 - build succeeded, **0 warnings / 0 errors**, 31.56 s; `Everglow.tmod` packaged and the mod enabled (Everglow, HEROsMod, CheatSheet, DragonLens) |
| 2 | Phase 4 gate `scripts/check-biology.ps1 -RequireAll` | exit 0 - `OK(0): phase4 in-scope set = 21 rows (rows=31)`, `OK: reconciled rows = 21 / 21`, `OK: guarded classes = 37`, `OK: UTF-8 BOM check passed (2 files).` |
| 3 | Phase 3 gate `../03-completed-art-ordinary-monsters/scripts/check-biology.ps1 -RequireAll` | exit 0 - `OK(0): phase3 tranche = 5 / 5 (rows=31)`, `OK: implemented classes = 5 / 5` (the Phase 3 chain still reproduces; that script is byte-identical, OQ3) |
| 4 | Phase 1 `check-inventory-reconciliation.ps1` | exit 0 - `OK(0): 103 entries; matched=86; green=50 yellow=8 unchecked=45; labels=5 deferred=3 assumptions=7` |
| 5 | Phase 1 `check-carryover.ps1` | exit 0 - `OK(0): carry-over covered entries = 5 (of 5 selected)` |
| 6 | Phase 1 `check-tranche-A.ps1` | exit 0 - `OK(0): tranche-A covered entries = 43 (of 43 selected)` |
| 7 | Phase 1 `check-tranche-B.ps1` | exit 0 - `OK(0): tranche-B covered entries = 20 (of 20 selected)` |
| 8 | Phase 1 `check-armofgianttree-charge.ps1` | exit 0 - `OK(0): ArmOfGiantTree charge is per-player + per-stack slot-keyed, synced, and server-authoritative` |
| 9 | Phase 2 `check-phase2.ps1` | exit 0 - `OK(0): phase2 implemented = 21 / 21 (full=9, shell=12)` |
| 10 | `dotnet test --filter "FullyQualifiedName~Yggdrasil"` | exit 0 - **3 passed, 0 failed, 0 skipped, 3 total**, 184 ms (`Everglow.UnitTests.dll (net8.0)`) |
| 11 | AGENTS.md byte-level UTF-8 BOM check | exit 0 - anchor `git merge-base HEAD origin/master` = `f222f9316508c71658a331a1c309776fd862fa2d`; `UTF-8 BOM check passed (1440 files).` |

**Triage outcome: none required.** Phase 4 adds no item, tile, wall, buff or localization artifact, so every earlier gate was expected to stay green and did. No earlier-phase gate or artifact was edited to make a gate pass (T-04-56), and no failure was bypassed. The chain ran as a single command whose per-link exit statuses are the phase's gate (T-04-60).

**The AGENTS.md BOM block, stated in its two parts (the `03-DEVIATIONS.md` section 8.2 precedent).** (a) It is **advisory and non-isolating**: it resolves `$base = git merge-base HEAD origin/master`, and on this long-lived branch that ancestor predates the whole phase, so the block measures 1440 changed and untracked files across the entire branch rather than this phase's change set. It reported no BOM-prefixed file, and the binding, phase-scoped equivalent is **invariant 13 inside the Phase 4 gate**, which ran first (link 2) over the phase's own change set and passed. (b) The block's three `git` stages each checked `$LASTEXITCODE` explicitly (`git merge-base`, `git -c core.quotepath=false diff --name-only`, `git -c core.quotepath=false ls-files --others --exclude-standard`) and each aborts with a named message on failure; the deduplicated file set and the BOM list are assembled with plain `foreach` loops rather than a shell pipeline after a `git` call, so a git failure aborts instead of passing as an empty change set (T-04-60). Should the check ever report a BOM in a path outside Phase 4's change set, that is a stale-anchor finding to record here with the offending path; nothing is edited to silence it, and no such path was found.

**What the chain does not cover.** The runtime half of the phase is untouched by all of this: the D-21 client bundle in `04-UAT.md` records 24 checks, every one not executed (section 14.6).

### 14.5 Consolidated deferred registry

Section 13 is the phase's deferred registry as plan 04-01 opened it; this close-out repeats it with the status each entry holds once the tranche is code-complete, so the phase's residue is readable in one place (QUAL-05).

| Category | Item | Status at close-out | Tracked in |
| --- | --- | --- | --- |
| Hardmode designs | 枯萎之种, 枯木人卫士 | `deferred: true`, never implemented (V2-HARD-01) | `03-BIOLOGY.json` `deferred_reason`; gate invariant 4 |
| Boss / special encounters | Klein Snake, Giant Winged Dragon, 吸血魔毯 (`VampireMat`) | untouched by this phase; Phase 7 | the three `phase: 7` rows |
| Region-level spawn predicates | 森雨幽谷 / 刺苔庭园 / 亡碧湖 sub-biome or tile predicates | open; Phases 5-6 terrain work (D-52) | `region:` elements (section 14.1); section 5 |
| Unimplemented systems | Spiny Moss Court morale/command buff; Valley egg system; Valley disguised-hazard visuals | open; recorded as `system:` blockers (D-46) | section 14.1; section 7 |
| Capture items | 装甲虾, 爆弹水母, 小格普螺 | open; item scope (D-58), `catchItem = 0` | `capture:` elements; section 6.2 |
| Absent drop materials | 毒腺, 牛黄, 软体甲壳碎片, 亡碧膏, 枯木碎块, 干涸心脏, 大型荆棘苔龟's TBD list, 大型覆藻章鱼's 武器与饰品 TBD list | open; item scope (D-58), no type reference written | `drop:` elements; section 6.2 |
| Cross-creature hostility | the design's "attacks other aquatic creatures" clauses | unmodelled; tML has no NPC-versus-NPC aggro path, so only the player is targeted | section 7; section 13 |
| Missing art | 26 `ModNPC` sprites + 11 projectile sprites | blocker only; never placeholder art (D-48/D-51), migratable by the three-step move of section 3 | `artwork:` elements; sections 3 and 4 |
| Localization | the whole phase | deferred by user directive (D-20); no exporter run, no key fabricated, no HJSON edited | section 12 |
| Runtime verification | the whole phase | outstanding; the D-21 bundle is recorded in `04-UAT.md` and not executed | section 11; `04-UAT.md` |

### 14.6 Client verification

The out-of-reach client and dedicated-server checks of the 21-row tranche are recorded, not executed, in `.planning/phases/04-remaining-ordinary-monsters/04-UAT.md` (the D-21 bundle). Section 11 describes what that bundle must observe; no entry in it is presented as verified.
