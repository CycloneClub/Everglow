---
gsd_state_version: 1.0
milestone: v1.0
milestone_name: milestone
current_phase: 04
current_phase_name: Remaining Ordinary Monsters
current_plan: 1
status: executing
stopped_at: Completed 04-01-PLAN.md
last_updated: "2026-09-16T06:04:27.987Z"
last_activity: 2026-09-16
last_activity_desc: Phase 4 tracer 04-01 complete — the new 13-invariant Phase 4 gate (green, red-before-green proven), the 21-row reconciliation of the shared 03-BIOLOGY.json + mirror, KelpCurtainSpawnConditions, 碧灵鮟鱇 (JadeSpiritAnglerfish) and the 04-DEVIATIONS.md ledger
progress:
  total_phases: 4
  completed_phases: 3
  total_plans: 25
  completed_plans: 17
state_head: b18a8d692dd2111ff44f87d1d7a09f9d004be1da
---

# Project State

## Project Reference

See: `.planning/PROJECT.md` (updated 2026-09-12)

**Core value:** Deliver a complete, publishable Kelp Curtain layer whose designed regions, gameplay loop, creatures, terrain, items, bosses, encounters, and rewards work together as a coherent Terraria experience.
**Current focus:** Phase 04 — Remaining Ordinary Monsters

## Current Position

Current Plan: 1
Total Plans in Phase: 9
Phase: 04 (Remaining Ordinary Monsters) — EXECUTING (tracer 04-01 done; waves 2–4 outstanding)
Plans complete: 1 of 9 (04-01 done; 04-02 … 04-09 outstanding)
Status: Executing — the Phase 4 tracer landed 2026-09-16 with a green new gate, the 21-row matrix reconciliation, the shared spawn-predicate helper, 碧灵鮟鱇 implemented end-to-end and the phase ledger opened; runtime verification (D-21) is outstanding for every Phase 4 row
Last activity: 2026-09-16 — 04-01 complete (new 13-invariant Phase 4 gate over the shared `03-BIOLOGY.json` run red-before-green, the 21 in-scope rows reconciled in place with the D-48 artwork blocker and mirrored cell-for-cell, `KelpCurtainSpawnConditions` with the three server-safe water/land predicates, `JadeSpiritAnglerfish` with its stealth→reveal→dash→chase→return cycle and its two Phase 1 drops, and `04-DEVIATIONS.md` §1–§13 opened with the OQ1–OQ5 resolutions)

Progress: [██████████] 100% — Phase 3 of 4 complete; Phase 4 wave 1 of 4 landed

## Performance Metrics

**Velocity:**

- Total plans completed: 17
- Average duration: n/a
- Total execution time: 0 hours

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 1–8 | 0 | TBD | n/a |
| 1 | 7 | 7 | ~17min |
| 2 | 4 | 5 | 10min |
| 3 | 4 | 4 | ~96min |
| 4 | 1 | 9 | ~26min |

**Recent Trend:** No execution data yet.
**Per-Plan Metrics:**

| Plan | Duration | Tasks | Files |
|------|----------|-------|-------|
| Phase 01 P01 | 21min | 3 tasks | 9 files |
| Phase 01 P02 | ~7min | 3 tasks | 3 files |
| Phase 01 P03 | 18min | 3 tasks | 4 files |
| Phase 01 P04 | ~19min | 3 tasks | 21 files |
| Phase 01 P05 | ~14min | 3 tasks | 8 files |
| Phase 01 P06 | ~30min | 3 tasks | 12 files |
| Phase 01 P07 | ~7min | 3 tasks | 5 files |
| Phase 02 P01 | 17min | 3 tasks | 11 files |
| Phase 02 P02 | 10min | 3 tasks | 11 files |
| Phase 02 P03 | 7min | 3 tasks | 11 files |
| Phase 02 P04 | 8min | 3 tasks | 10 files |
| Phase 02 P05 | 10min | 3 tasks | 5 files |
| Phase 03 P01 | 184min | 2 tasks | 6 files |
| Phase 03 P02 | 157min | 3 tasks | 5 files |
| Phase 03 P03 | 11min | 3 tasks | 6 files |
| Phase 03 P04 | ~30min | 2 tasks | 5 files |
| Phase 04 P01 | ~26min | 2 tasks | 6 files |

## Accumulated Context

### Decisions

- [Phase 04]: The Phase 4 gate is a NEW script `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1` reading the SHARED `03-BIOLOGY.json`; the Phase 3 script is left byte-identical so the Phase 3 close-out chain still reproduces (`-RequireAll` stays green at `phase3 tranche = 5 / 5`, `implemented classes = 5 / 5`). It carries the plan's 13 invariants and ran red before its class existed (`FAIL(2)` naming `JadeSpiritAnglerfish.cs`) and green after (`OK(0): phase4 in-scope set = 21 rows (rows=31)`, `OK: reconciled rows = 1 / 21`, `OK: guarded classes = 1`).
- [Phase 04]: The in-scope set is frozen at the 21 non-deferred `phase: 4` rows (D-44); the two hardmode rows stay `deferred` with a non-empty `deferred_reason` under V2-HARD-01. `internal_name` is populated only once its class file exists on disk, so 04-01 reconciles exactly one row (`bio-death-jade-lake-jade-anglerfish`) and plan 04-09 flips the remaining twenty — that is what keeps the gate's class-resolution biconditional valid at every wave boundary.
- [Phase 04]: The matrix reconciliation was a formatting-preserving textual edit (never a JSON round-trip) plus a regenerated `03-BIOLOGY.md`, so a `git diff` audit shows only `blockers` (21 rows), `code_complete` (1) and `internal_name` (1) changed; the frozen `counts` / `phase3_tranche` / `texture_complete` / `design_art` sets and every `status` are untouched.
- [Phase 04]: Every Phase 4 creature class lives in `NPCs/<Region>/` with namespace `...NPCs.<Region>` (`DeathJadeLake`, `SpinyMossCourt`, `ValleyOfLushAndMoist`), because the region subfolder is what tML's namespace-derived texture resolution expects; the D-49 art migration is add `<Class>.png` beside the `.cs`, delete the `Texture` override, delete the blocker (Pitfall 3).
- [Phase 04]: `KelpCurtainSpawnConditions` is the shared server-safe seam — `IsDryLand`, `IsWaterSurface` (derived from a dry tile above, since `NPCSpawnInfo` has no surface flag) and `IsWaterBottom` (bounded 24-tile floor probe) read only `NPCSpawnInfo` and spawn-tile state, plus the D-54 weight bands (`LandWeight` 1f, `WaterWeight` 0.75f, `RareWaterBottomWeight` 0.35f, `MiniBossWeight` 0.1f). `SpawnChance` always gates on `SubworldSystem.IsActive<YggdrasilWorld>()` plus `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` and never on `IsBiomeActive`.
- [Phase 04]: `JadeSpiritAnglerfish` re-asserts its state-dependent contact damage in `PostAI` from the synced `NPC.ai[0]` (60 while dashing, `defDamage` otherwise) because `NPC.damage` is not part of the NPC net message; every transition is server-authoritative (`Main.netMode != NetmodeID.MultiplayerClient` + `NPC.netUpdate`) and every lamp/dust call sits behind `!Main.dedServ`.
- [Phase 04]: Stat variants are sibling classes (`AnimatedWitherbarkSoldierRanged` / `_Spell` / `_Hound`, `LargeBombJellyfish`, `SmallBrodieFlydragon`, OQ2/D-47); only 幽光蝾螈's colour uses one class with a synced `NPC.localAI[]` index. `SetDefaults` is per-type, so a stat variant is never produced by mutating `NPC.lifeMax` in `AI`/`OnSpawn` (Pitfall 5).
- [Phase 04]: The gate's guarded-class set is deliberately matrix-independent (the three region folders plus a name-scoped sweep of the enemy-projectile tree against the frozen eleven-projectile list), so waves 2–4 are guarded the moment their class lands and `OK: guarded classes =` is a monotone progress counter (37 when the phase closes). The item-type index spans the whole `Sources/Modules/Yggdrasil` tree because OQ1 wires the pre-existing `YggdrasilTown/Items/Materials/CaterpillarJuice.cs` from a KelpCurtain class.
- [Phase 04]: Rule 1 API-name correction: the plan's `NPC.rare = ItemRarityID.White;` does not exist in this tML build and was corrected to `NPC.rarity` (the engine's only NPC rarity field), recorded in `04-DEVIATIONS.md` §10 — the same correction Phase 3 made for the same reason.
- [Phase 04]: BIO-01/BIO-02/BIO-03 are advanced by 04-01 but complete in none of it (one of 21 rows); `REQUIREMENTS.md` is left untouched and its traceability rows stay Pending until plan 04-09 closes the phase.
- [Phase 04]: `04-DEVIATIONS.md` is the phase's single ledger (§1–§13, §14 reserved for plan 04-09): the 21-row scope, the OQ2 variant strategy, the D-49 folder rule, missing-art handling, spawn routing, drop wiring with the OQ1 cross-namespace decision in §6.1, the frozen eleven projectiles and every effect blocker, the OQ5 mini-boss call, the OQ3 gate placement, the conservative-defaults register (incl. 酸性毒液 → `BuffID.Venom`, because `BuffID.AcidVenom` was verified absent by reflection over `Terraria.ID.BuffID`) and the deferred registry.
- [Phase 04]: Tracer feedback gate: 04-01 is `type="tracer"` with a genuine `<human-check>` (no `blocking-human` gate), so the literal `end-of-phase` precedence chain would halt before task 2. The plan's own `<why_human>` states the client checks are the D-21 batch recorded in plan 04-09 (and plan 03-01, the identical tracer shape, was executed the same way), so the automated verify was re-run green before task 2 and the human check is carried into `04-UAT.md` rather than rubber-stamped. Recorded as a documented process deviation in `04-01-SUMMARY.md` and `04-DEVIATIONS.md` §11.

- **2026-09-15 — Phase 3 marked complete with runtime UAT deferred (user-authorized override).** Phase 3's four plans are code-complete (Release build 0/0; full gate chain green; `check-biology.ps1 -RequireAll` = `phase3 tranche 5 / 5`, `implemented classes 5 / 5`), but the D-21 in-client runtime verification in `03-UAT.md` was not executed because the tModLoader client environment was unavailable. The user authorized marking the phase complete and deferring the runtime UAT (same pattern as the Phase 1 ArmOfGiantTree multiplayer deferral). `03-VERIFICATION.md` remains `human_needed`, `03-UAT.md` stays unexecuted, and the deferral is tracked as an open `unrun-verify` item in `.planning/WINDOWS.md`. No verification result was fabricated.
- **2026-09-12 correction — item allocation is by design-artwork state only.** A completed-art entry belongs to the completed-art item tranche (Phase 1) whether or not the repository already has a class for it; class-less status must never defer an entry to a later phase. Plan 01-06 refined the routing: of the 18 artwork-complete class-less entries, the 5 non-boss item-table rows are Phase 1 carry-over and the 13 boss/special-encounter rows are Phase 7 (ITEM-05/ITEM-06); Phase 2 contains only unfinished-art entries (25).
- The three Feishu second-layer documents are authoritative; every item is compared through XML full fetch and receives green/exact, yellow/partial-conflicting, or unchecked/blocked status.
- Execution order is mandatory: items, ordinary monsters, terrain/generation, bosses and special encounters, then publishable integration.
- Within each content category, complete design artwork/textures precede incomplete artwork/textures; biology-design drops are item work and are not deferred to monster phases.
- Phase 1 owns source/inventory reconciliation, including Biology's three region labels and Terrain's additional Green Tundra and Town of Decaying Wood labels; Phase 8 owns final source-status synchronization.
- Explicit hardmode-deferred entries and undefined future designs remain outside this milestone.
- [Phase 01]: Evidence snapshots are committed as well-formed <fragment> XML; the offline parser is header-anchored and rowspan-aware, and validator exit 3 accepts an empty texture checkbox id only when a blocker is recorded.
- [Phase 01]: Tranche is assigned from the resolved repo class content family (or parser category when class-less); the terrain-sourced row takes an empty tranche.
- [Phase 01]: advances is type-based (ITEM-01/ITEM-02) with procurement overrides to ITEM-03/ITEM-04 and ITEM-07 on every entry.
- [Phase 01]: A shared-placeholder-texture class (RadialCarapace -> White_Mod) is recorded artwork-incomplete/yellow despite the Feishu checkbox; Green Tundra stays unresolved with a blocker.
- [Phase 01]: Design 价格 maps to Item.value (buyPrice/raw value); oversized sellPrice(...) usages are completed-art deviations corrected in 01-03.
- [Phase 01]: 01-03 tranche A halted at Task 2 by its >8 bound (18 class-less completed-art entries); PHASE SPLIT RECOMMENDED with a per-category breakdown.
- [Phase 01]: Replanned 01-03 completed the D-12 effect/recipe/set-bonus remainder: 7 recipes exact, 34 effects matched to wired projectiles/buffs, 4 effect blockers recorded.
- [Phase 01]: All 43 class-less entries were initially routed to Phase 2; plan 01-06 superseded that for the 18 completed-art ones (5 -> Phase 1 carry-over, 13 -> Phase 7). RadialCarapace and VineRepairWand stay phase 1 per D-11.
- [Phase 01]: ThornTurtleShell run-speed corrected to the design -10%; check-tranche-A.ps1 gates the completed-art tranche-A entries (43 after plan 01-06).
- [Phase 01]: Tranche-B design rows were header-anchored; the recipe embedded after '=' in the 效果 cell is a recipe (DevilHeart armor 18/20/32/24 bars at an Anvil).
- [Phase 01]: The four missing DevilHeart armor recipes and the Ruin/material rarity+value deviations were fixed (D-12); the Witherbark minion pattern, set-bonus display text, and Photophore misimplementation were recorded as blockers outside plan 04's file scope.
- [Phase 01]: Phase 1 localization is deferred by user directive (2026-09-12, 'record it; do not consider localization, just complete the code portion'); the in-game exporter was not run, no key was fabricated, and no HJSON was hand-edited. The missing-key entries are recorded as status=deferred deviations (plan 01-05: 13; plan 01-06 added 5 -> 18; advisory -AllowMissing baseline 45/63).
- [Phase 01]: 2026-09-12 allocation correction refined by plan 01-06: of the 18 completed-art class-less entries, 5 are Phase 1 carry-over and 13 are Phase 7 (9 Giant Winged Dragon -> ITEM-06, 4 Klein Snake -> ITEM-05); phase counts {1: 65, 2: 25, 7: 13}; P1A-03 superseded by P1A-12.
- [Phase 01]: The three carry-over entries with no repository texture (ForestBreath, WitheredMask, QuetzalsWish) reuse the shared Commons.ModAsset.White_Mod fallback (RadialCarapace precedent) so the mod loads, and carry named artwork blockers with repo_asset empty; no placeholder art was created.
- [Phase 01]: Localization deferral (2026-09-12 directive) extended to the five plan 01-06 entries via P1A-13; check-localization-coverage.ps1 -AllowMissing selects 63 (45 covered / 18 missing); no key fabricated and no HJSON edited.
- [Phase 01]: scripts/check-carryover.ps1 gates the 5 carry-over entries (class or texture|artwork blocker) and check-inventory-reconciliation.ps1 now accepts the D-06 recorded-artwork-blocker repo_asset exception.
- [Phase 01]: 01-07 moved ArmOfGiantTree charge off the shared per-type ModItem to KelpCurtainPlayer.ArmOfGiantTreeCharge (per-player) with an ArmOfGiantTreeChargedSlot discriminator keyed on player.selectedItem (per-stack); CR-01 closed.
- [Phase 01]: 01-07 made the ArmOfGiantTree full-charge shockwave server-authoritative via ArmOfGiantTreeChargePacket ReleaseSmash (client signals, handler clamps Charge + requires the sender to hold the item, ApplyShockwave sets npc.netUpdate); WR-01 closed.
- [Phase 01]: 01-07 gated the 0.75x..2x charge damage scaling on player.altFunctionUse != 2 so the right-click ordinary swing keeps base damage; WR-02 closed.
- [Phase 01]: 01-07 verification used the phase baseline 8ed6f5862 instead of the plan's stale origin/master anchor (origin/master predates the whole phase; 1312-file false-positive diff).
- [Phase 02]: Art-missing armor registers its equip slot explicitly via EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, type, this, nameof(Class)) in a Main.dedServ-guarded Load(); the autoload-equip attribute is forbidden while _Head/_Body/_Legs art is absent (02-RESEARCH Pitfall 1).
- [Phase 02]: Inventory rows mark code_complete=true once the class is implemented (D-11/D-22), superseding the plan-01-06 assumption that it mirrors the Feishu code checkbox; status stays unchecked because artwork is incomplete.
- [Phase 02]: The 红月水藻 set mirrors the accepted red-algae family recipe (15 x JadeLakeRedAlgae_Item + 1 x CrimsonMoonSap at Work Benches) because the design supplies none; recorded as a designer-confirmation deviation in 02-DEVIATIONS.md.
- [Phase 02]: The set bonus scales red-algae toxin detonation 2.5x in RedAlgae_FriendlyDebuff_glocalNPC when CrimsonMoonAlgaeSetBuff is set; the 15s->30s toxin-duration doubling is recorded as a blocker naming the seven accepted Phase-1 applier files.
- [Phase 02]: scripts/check-phase2.ps1 resolves implemented classes from the working tree via Get-ChildItem (not git ls-files) and builds the CJK inventory ids from [char]0x.... codepoints so the gate script stays 100% ASCII.
- [Phase 02]: The biology weapon plan implements only the two fully-specified effects (150% direct hit + 3-6 shards; +10% final damage to bosses) and records the TendonGreatbow charge curve, the RestrictionDeviceRE01 限制无人机/聚能射线/浊燃 system and the ReekingBait 巨翼龙 encounter as precise blockers.
- [Phase 02]: No AddRecipes body is written for TendonGreatbow/RestrictionDeviceRE01/ReekingBait because their design 合成方式 cells name only absent Phase 7 Giant Winged Dragon items; an item-type reference to an absent type is a compile error (T-02-01).
- [Phase 02]: The qualitative 击退 强/弱 maps to Item.knockBack 8f/2f on the accepted family scale (DD-05), and TendonGreatbow damage 58 comes from the committed evidence row (DD-06), superseding the RESEARCH draft 28 (a use-time 28（慢） misread).
- [Phase 02]: Both new projectile classes wrap every dust/sound call in if (!Main.dedServ), unlike the GreenThornLauncher analog (T-02-04); no PreDraw and no Main.projFrames (T-02-03).
- [Phase 02]: 灵蛇玉卵 is implemented as a consumable SummonItems use item (10 gold, Blue, 12-frame swing) that spawns nothing; the Phase 7 苍翠灵蛇 encounter and the 在森雨幽谷顶部使用 location gate are recorded as one precise effect blocker (T-02-04).
- [Phase 02]: The six plan-02-03 shells are identity-only classes (D-18) with ForestBreath's conservative defaults (20x20, 50 silver, Blue) and their family LocalizationCategory: Accessories (竹节步符/桃枝护符), MeleeWeapons (竹制武器), Vanity (竹簪子), Miscs (桃花纸鸢), Pets (熊猫宠物); no recipe, Item.shoot, UpdateAccessory or equip slot.
- [Phase 02]: Both vanity shells live in Items/Misc (no Items/Vanity directory exists; WitheredMask precedent); 02-CLASSIFICATION.json's 竹簪子 class_file was corrected from Items/Vanity to Items/Misc (DD-07).
- [Phase 02]: [Phase 02] The last six shells (AlcoholicDrinks, FluorescentHydraStaff, DiscipleSword, DiscipleVanity, SkillBambooSlip, RegionalCraftingStation) are identity-only classes (D-18) with ForestBreath's conservative defaults and their family LocalizationCategory: Miscs, SummonWeapons, MeleeWeapons, Vanity, Miscs, Placeables; no recipe, Item.shoot, UpdateAccessory, equip slot, ModTile or createTile.
- [Phase 02]: [Phase 02] The four system-dependent shells name the missing 弟子/skill/regional-crafting systems exactly and none of those systems is implemented (D-19); RegionalCraftingStation stays a plain ModItem because a tile without its system would be half-built; 02-CLASSIFICATION.json's SkillBambooSlip and DiscipleVanity class_file paths were corrected to Items/Misc (DD-11/DD-12).
- [Phase 02]: Re-anchored the Phase 2 BOM/no-art guard from the plan-named 926543d99 to the merge commit 43478f8ef because the branch merge pulled a parallel developer art commit (a1975d1bf, 10 .pngs) into 926543d99..HEAD; the re-anchored 41-file change set reports 0 BOM and 0 art/binary (Phase 1 01-07 precedent)
- [Phase 02]: Generated the consolidated 44-row blocker table mechanically from 01-INVENTORY.json so the ledger, JSON and Markdown blocker text is byte-identical; added 21 Phase 2 localization-deferral records to deviations[] and a phase2_closeout counts block
- [Phase 03]: The Phase 3/4 tranche is repository art (D-41/D-42), superseding 03-RESEARCH Finding 1 / Open Question 1 (inline design <img>); the five tranche rows are 荆棘苔龟, 格普螺, 叶飞棍, 巨树人 and the already-implemented 水蛞蝓, and the matrix carries 31 rows with counts rows 31 / phase3 5 / phase4 23 / phase7 3 / deferred 2 / texture_complete_true 6 / design_art_true 9.
- [Phase 03]: design_art is derived mechanically from the ten enumerated inline <img> elements in nine design sections and is independent of texture_complete; 水蛞蝓's 36x27 image is included and 吸血魔毯 (a Phase 7 row with no inline image) is not.
- [Phase 03]: internal_name is populated only once its class file exists on disk, so a code_complete:false tranche row carries an empty internal_name and plans 03-02/03-03 fill each in the same task that creates its class; that is what keeps the gate's class-resolution biconditional green at every wave boundary.
- [Phase 03]: scripts/check-biology.ps1 is the 100% ASCII phase gate (13 invariants) resolving classes from the working tree with Get-ChildItem; it ran red before MossyThornTurtle.cs existed and green after, and its invariant 13 is the phase-scoped UTF-8 BOM guard.
- [Phase 03]: KelpCurtainBiome.IsKelpCurtainLayer(Player) is the shared server-safe layer predicate (band + stratum test from player.Center); IsBiomeActive is left byte-for-byte unchanged because it drives camera-following scene transitions.
- [Phase 03]: The 荆棘苔龟 melee reflect runs once on the client doing the damage under a player.whoAmI != Main.myPlayer guard because tML never invokes OnHitByItem on the server; the ModIns.PacketResolver alternative is recorded as not taken.
- [Phase 03]: Two plan-named API members do not exist and were corrected (Rule 1): spawnInfo.player -> NPCSpawnInfo.Player, and NPC.rare -> NPC.rarity (a no-op recording the design's empty 稀有度 cell).
- [Phase 03]: BIO-01/BIO-02/BIO-03/BIO-06 are advanced by plan 03-01 but complete in none of it; REQUIREMENTS.md is left untouched and the traceability rows stay Pending until 03-04 closes the phase.
- [Phase 03]: Plan 03-02's shell-state transition lives in HitEffect (the documented on-hit hook, called on the server) under Main.netMode != NetmodeID.MultiplayerClient with NPC.netUpdate, while ModifyIncomingHit keeps the design's 0.85/0.70 FinalDamage scales; tML documents ModifyIncomingHit as modifiers-only with side effects belonging to OnHit hooks.
- [Phase 03]: 叶飞棍's 50% 中毒 is applied unguarded in OnHitPlayer (tML runs that hook on the local client and Player.AddBuff performs the buff sync), while its water-suffocation NPC.life drain is netmode-guarded and clamped at 1 life; a multiplayer-client guard on the poison would have made it dead code in multiplayer.
- [Phase 03]: Both plan-03-02 creatures reject spawnInfo.Water — 格普螺 is a land crawler and 叶飞棍 an open-air flier — so the design's water interaction lives in the leaf rod's AI (occasional skim, suffocation, escape bias) rather than in a spawn requirement; both reuse the server-safe KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player) and never IsBiomeActive.
- [Phase 03]: Absent designed drops stay absent: 格普螺 writes only the 11% GuppyShell rule (软体甲壳碎片 blocked) and 叶飞棍 ships an explicitly empty, commented ModifyNPCLoot (飞棍毛发 / 毒腺 blocked); neither class contains a type reference to an absent material (D-37/D-39).
- [Phase 03]: Plan 03-02 advances BIO-01/BIO-03/BIO-06/QUAL-03 but completes none of them; REQUIREMENTS.md is left untouched and the traceability rows stay Pending until 03-04 closes the phase.
- [Phase 03]: Plan 03-03 implements 巨树人 (`GiantDandelion`) as the tranche's only multi-range state machine: a private `GiantDandelionState` enum over a wrapped `NPC.ai[0]` with named timer wrappers over `NPC.localAI[0]`/`NPC.localAI[1]`, the design-exact 120-tick smash wind-up / 300-tick recovery / 240-tick mid-range gap, the 5-15 tile boulder branch and the beyond-15-tile faster chase, at a deliberately rare 0.25f spawn weight.
- [Phase 03]: The 巨树人 defense swing (防御 30 -> 0 -> 30) is owned by the `EnterState` helper rather than by hand-written exit branches, so every exit from `SmashRecovery` — including a target lost mid-recovery — restores the normal value under the server guard with `NPC.netUpdate`; `ModifyIncomingHit` keeps only the design's 1.5f FinalDamage scale.
- [Phase 03]: The two new attack projectiles (`GiantDandelion_Shockwave`, `GiantDandelion_Boulder`) use the shared `Commons.ModAsset.White_Mod` fallback because no approved art exists (creating art is forbidden), are spawned from `NPC.GetSource_FromAI()` inside `Main.netMode != NetmodeID.MultiplayerClient`, and guard every dust call with `!Main.dedServ`; `BoulderCatapult_Proj` was NOT reused because it is a friendly player-owned projectile (D-13 art policy, T-03-18/T-03-19/T-03-20).
- [Phase 03]: 巨树人's `ModifyNPCLoot` is the tranche's first real multi-item table (three guaranteed Phase 1-2 drops: `ArmOfGiantTree`, `HardenedWitherbarkHeart`, `BoulderCatapult`); the design's 4~6 枯木碎块 stays an absent-drop blocker with no type reference (D-37/D-39).
- [Phase 03]: 巨树人的 sprite-derived extents are `214x263` (about 13x16 tiles), which is larger than any hit box in this repository; it is flagged in `03-DEVIATIONS.md` section 8 as the first thing to revisit if the creature never finds a spawn area, rather than silently shrinking the design's creature.
- [Phase 03]: Plan 03-03 advances BIO-02/BIO-06/QUAL-03 but completes none of them; after it the wave-3 gate reports `implemented classes = 5 / 5`, and REQUIREMENTS.md is still left untouched until 03-04 closes the phase.
- [Phase 03]: Plan 03-04 closed the phase: all five tranche rows read `code_complete: true` with a resolving `internal_name` and final blocker text (each ending with the canonical `localization deferred (D-20); runtime verification outstanding (D-21)` element), the frozen counts / `phase3_tranche` / `texture_complete` / `design_art` sets are unchanged, `03-BIOLOGY.md` mirrors the JSON on all 31 rows × 13 cells, and `check-biology.ps1 -RequireAll` prints `OK(0): phase3 tranche = 5 / 5 (rows=31)` and `OK: implemented classes = 5 / 5`.
- [Phase 03]: The consolidated blocker register lives in `03-DEVIATIONS.md` **§10**, not §2 as 02-DEVIATIONS.md places it: renumbering §2–§9 would invalidate every section cross-reference carried by STATE.md, the three prior summaries and the plan texts, so the register is appended and a navigation line names the thirteen sections. It is generated mechanically from the JSON `blockers` arrays, so its **Exact blocker text** cells cannot drift from the matrix (T-03-25).
- [Phase 03]: The matrix's `assumptions[]`/`deviations[]` machine copy shipped from the 03-01 tracer commit `421f2a09a` as six `"e"` placeholders; plan 03-04 repaired it (Rule 1) with one entry per ledger §1–§6 plus the phase's six recorded deviations. No gate invariant reads those fields, so only a close-out could catch the corruption (`WINDOWS.md` entry 23).
- [Phase 03]: The close-out chain ran green in a single command and is recorded verbatim in `03-DEVIATIONS.md` §8.1/§8.2 — Release build (0 warnings, 0 errors, `Everglow.tmod` packaged), `check-biology.ps1 -RequireAll`, the five Phase 1 gates, `check-phase2.ps1`, `dotnet test --filter "FullyQualifiedName~Yggdrasil"` (3 passed, 0 failed) and the AGENTS.md byte-level BOM check (`UTF-8 BOM check passed (1375 files).`), `CHAIN_EXIT=0`; the `git merge-base HEAD origin/master` anchor is recorded as advisory and non-isolating with `check-biology.ps1` invariant 13 named as the binding phase-scoped gate.
- [Phase 03]: `REQUIREMENTS.md` keeps BIO-01/BIO-02/BIO-03 (mapped to Phase 4) and BIO-06/QUAL-03/QUAL-04 (mapped to Phase 8) **Pending**: Phase 3 implemented the repository-art tranche only — 5 of the 25 designed ordinary creatures (2 of 14 Death Jade Lake, 1 of 4 Spiny Moss Court, 2 of 7 Valley rows) — so the file records a Phase 3 tranche-advance note instead of a completion it cannot support.
- [Phase 03]: The D-21 client bundle is `.planning/phases/03-completed-art-ordinary-monsters/03-UAT.md` — 8 checks (per-row spawn, main-world isolation, behaviour, combat, drops, dedicated-server/multiplayer, localization fallback, consolidated end-of-phase run), every one marked not-executed with a header stating the bundle is recorded but not run, so the tranche is never presented as client-verified (T-03-23, `WINDOWS.md` entry 22).

### Pending Todos

None yet.

### Blockers/Concerns

- ⏳ **[Phase 4 — wave 1 landed 2026-09-16, waves 2–4 outstanding]** Plan 04-01 delivered the tracer slice: the green Phase 4 gate, the 21-row reconciliation, `KelpCurtainSpawnConditions`, 碧灵鮟鱇 (`JadeSpiritAnglerfish`) and the `04-DEVIATIONS.md` ledger. Twenty of the 21 in-scope rows are still `code_complete: false` (04-02 … 04-08 create their classes; 04-09 reconciles them and runs the D-21 bundle). Outstanding and recorded: the D-21 runtime bundle (`04-UAT.md` in plan 04-09) for every spawn predicate, the 碧灵鮟鱇 stealth/dash feel and its drops, the clean-load check for art-missing classes and a dedicated-server run; **approved art for 26 `ModNPC` sprites + 11 projectile sprites** (blocker only — no placeholder art was created, D-48/D-51); the 森雨幽谷 / 刺苔庭园 / 亡碧湖 region-level spawn predicates (Phases 5–6, D-52); the unimplemented systems (Spiny Moss Court morale/command, Valley egg system, disguised hazards, capture items, cross-creature hostility); localization (D-20); and the absent drop materials 毒腺 / 牛黄 / 软体甲壳碎片 / 亡碧膏 / 枯木碎块 / 干涸心脏 (D-58). See `04-DEVIATIONS.md` §5, §7, §11, §13.
- ⏸ **[Phase 3 — deferred, user-authorized 2026-09-15]** Runtime UAT for the five-creature tranche is unexecuted: `03-UAT.md` (8 checks: spawn isolation per BIO-06, behavior/combat/immunities, drops, dedicated-server/multiplayer per QUAL-03, localization fallback) and the 4 D-21 backstop items in `03-VERIFICATION.md` await a tModLoader client. The phase is marked complete on code grounds only; run `/gsd-verify-work 3` when the client is available. Also open: approved art for the two `GiantDandelion` projectiles, and 森雨幽谷/刺苔庭园 regional spawn predicates (Phases 5–6).
- ✅ **[Phase 1 carry-over — resolved 2026-09-13 by plan 01-06]** The 18 artwork-complete class-less entries were reallocated 5 Phase 1 / 13 Phase 7 (P1A-12); the five Phase 1 items are implemented and gated (`check-carryover.ps1` 5/5). No parser re-run (CR-01).
- Phase 1: Feishu source reconciliation must classify all five terrain labels and inventory every item/drop before implementation acceptance.
- Missing approved artwork must remain a visible blocker; no placeholder art may be introduced.
- Yggdrasil generation, rendering, subworld, persistence, and multiplayer behavior require live tModLoader verification beyond unit-test coverage.
- 01-03 (replanned) resolved the halt: the 18 class-less completed-art tranche-A entries and the 25 art-incomplete class-less entries are routed to Phase 2 (phase=2); the tranche gate `check-tranche-A.ps1` is authored (43 covered after plan 01-06). Four tranche-A weapons carry 效果 blockers (MossySpell, CyatheaArrow, GreenSungloStaff, EvilHalbertBarnacle) whose design behavior lives in projectiles outside the item-class modify set.
- RadialCarapace and VineRepairWand remain Phase 1 tracked blockers (code-complete, artwork-missing per D-11); their design deviations are queued to Phase 2 (D-12).
- Phase 1 localization is deferred by user directive: 18 completed-art items lack both-culture display keys (the plan 01-05 13 — EvilHalbertBarnacle, ArcI, RedAlgaeMagicStaff, RedAlgaeMagicSpellBook, RedAlgaeMagicWhip, CrimsonMoonSap, EmptyWaterStaff, JadeLakeRedAlgae_Item, Photophore, GreenSungloStaff, ActivatedDogStaff, RedAlgaeMinionGyroscope, RedAlgaeMinionStaff — plus the plan 01-06 carry-over 5: ArmOfGiantTree, ForestBreath, ElftigernPowder, WitheredMask, QuetzalsWish; 45/63 covered). Run the in-game OutputLocalizationHjsonItem exporter (or resolve in Phase 2/8). Recorded in 01-DEVIATIONS.md and 01-INVENTORY.json.

- [Phase 02] The four biology-design weapon rows (巨石弹射装置, 肌腱巨弓, 限制机, 腥臭的诱饵) are code-complete / art-incomplete. Outstanding blockers: approved textures (all four); the TendonGreatbow charge curve; the absent Phase 7 dependencies (限制无人机/聚能射线/浊燃 system, 巨翼龙 encounter, and the 血云母/血肉聚合物/熔炉钢/隐生之眼/干枯心脏/玉化龙骨 ingredients). Runtime verification (D-21) of both weapon chains is outstanding.
- [Phase 02] The plan-02-03 rows (灵蛇玉卵, 竹节步符, 竹制武器, 竹簪子, 桃枝护符, 桃花纸鸢（风筝）, 熊猫宠物) are code-complete / art-incomplete. Outstanding blockers: approved textures (all seven); the 苍翠灵蛇 Phase 7 encounter and its 在森雨幽谷顶部使用 location gate for 灵蛇玉卵; runtime verification (D-21) of the seven entries. Phase gate advanced to 15/21.
- [Phase 02] The plan-02-04 rows (若干酒类, 荧光水螅召唤杖, 弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台) are code-complete / art-incomplete. Outstanding blockers: approved textures (all six); the 弟子 (disciple) progression system for 弟子剑/弟子时装; the skill system for 技能竹简; the regional-crafting system plus its placement tile for 区域放置物品制作台; the absent 荧光水螅 summon projectile for 荧光水螅召唤杖. No system was implemented (D-19). Item coverage is closed at 21/21; runtime verification (D-21) of the six shells is outstanding (WINDOWS.md entry 12).
- [Phase 03] Plan 03-01 froze the repository-art tranche and implemented 荆棘苔龟 (`MossyThornTurtle`) end-to-end. Outstanding: runtime verification (D-21) for spawn isolation, the spin-state defence/damage switch, the melee reflect and the 5% 荆棘龟壳 drop; a dedicated-server run confirming `KelpCurtainBiome.IsKelpCurtainLayer` evaluates the same band as the camera-driven `IsBiomeActive`; designer confirmation of the five asset-to-creature mappings and of the six design-art-only creatures that D-41 sends to Phase 4; and a per-region spawn refinement once 刺苔庭园/森雨幽谷 biome predicates exist (03-DEVIATIONS.md sections 2, 3, 6, 8).
- ✅ **[Phase 03 — resolved 2026-09-15 by plan 03-03]** No tranche row is left `code_complete:false`: the last one (`bio-spiny-moss-court-giant-tree-man`) was flipped together with its class in Task 3, and `scripts/check-biology.ps1` now prints `OK: implemented classes = 5 / 5`. The plan-03-01/03-02 rule that `internal_name` is populated only once its class exists held at every wave boundary, so no row was ever mis-recorded.
- [Phase 03] Plan 03-02 implemented 格普螺 (`GuppyConch`) and 叶飞棍 (`VerdantRods`). Outstanding: runtime verification (D-21) for the shell retract feel and the 防御 10/20 + 0.85/0.70 减伤 switch, the crawl reversal at walls/ledges, the 11% 格普螺外壳 drop, the leaf rod's neutral circling without chasing, its 50% 中毒 and its submerged life drain with the escape flight (WINDOWS.md entries 17 and 18); the per-region spawn refinement for 叶飞棍 (亡碧湖 vs 森雨幽谷) once a regional predicate exists; and the same dedicated-server band check recorded for plan 03-01 (03-DEVIATIONS.md sections 4, 5, 6, 8).
- [Phase 03] Plan 03-03 implemented 巨树人 (`GiantDandelion`) and its two hostile projectiles (`GiantDandelion_Shockwave`, `GiantDandelion_Boulder`). Outstanding: runtime verification (D-21) of the full state cycle — the visible ground wave, the 300-tick amplified-damage window, the mid-range wind-up/swing/arc and the beyond-15-tile chase; **approved art for both projectiles** (they use `Commons.ModAsset.White_Mod` today; creating art is forbidden, so the designer must supply a ground-wave sprite and a boulder sprite — `03-DEVIATIONS.md` section 9); the 214x263 spawn-area question and the floor-anchored wave hit box on uneven terrain (`03-DEVIATIONS.md` section 8); the per-region spawn refinement for 刺苔庭园 once a regional predicate exists; and the dedicated-server check that the two projectiles emit no dust and spawn once (WINDOWS.md entries for plan 03-03).
- ✅ **[Phase 03 — tranche code-complete 2026-09-15 by plan 03-04]** All five repository-art rows are implemented (`implemented classes = 5 / 5`), the matrix and its mirror are reconciled cell-for-cell, and the phase's consolidated ledger, close-out counts and decision disposition are written. The phase's full offline chain is green in one run (Release build + `check-biology.ps1 -RequireAll` + five Phase 1 gates + `check-phase2.ps1` + the Yggdrasil MSTest link + the AGENTS.md BOM check) and recorded in `03-DEVIATIONS.md` §8.
- [Phase 03] **Outstanding after close-out (all recorded, none hidden):** the D-21 client bundle in `03-UAT.md` (8 not-executed checks: per-row spawn, main-world isolation, behaviour, combat, drops, dedicated-server/multiplayer, localization fallback and the consolidated run — `WINDOWS.md` entry 22); approved art for the two 巨树人 attack projectiles (`WINDOWS.md` entry 21); the 森雨幽谷 / 刺苔庭园 regional spawn predicates (Phase 5–6); localization deferred by user directive (D-20); and the four absent drop materials 软体甲壳碎片 / 飞棍毛发 / 毒腺 / 枯木碎块 (D-37/D-43). Phase 3 changed no Feishu status: every matrix row stays `unchecked` until Phase 8's source synchronization (D-25).

### Quick Tasks Completed

| # | Description | Date | Commit | Directory |
|---|-------------|------|--------|-----------|
| 260914-kl8 | Revert Phase 1 design-value parity edits to pre-Phase-1 code values; document code-wins-unless-Feishu-yellow rule | 2026-09-14 | e8103ff90 | [260914-kl8-revert-phase-1-design-value-parity-edits](./quick/260914-kl8-revert-phase-1-design-value-parity-edits/) |

## Deferred Items

| Category | Item | Status | Deferred At | Milestone |
|----------|------|--------|-------------|-----------|
| Hardmode | Explicitly hardmode-deferred source entries, including Withered Seed and Witherbark Guard | Deferred | 2026-09-11 | Kelp Curtain second layer |
| Future design | Undefined future Yggdrasil layers and later second-layer additions | Deferred | 2026-09-11 | Kelp Curtain second layer |

## Session Continuity

Last session: 2026-09-16T06:05:00.000Z
Stopped at: Completed 04-01-PLAN.md
Resume file: .planning/phases/04-remaining-ordinary-monsters/04-02-PLAN.md
