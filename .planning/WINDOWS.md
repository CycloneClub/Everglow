---
schema_version: 1
open_count: 41
waived_count: 0
fixed_count: 2
total_count: 43
last_updated: 2026-09-16T08:31:57.941Z
---

# Broken Windows Ledger

> Cross-phase defect register. With `workflow.windows_enforce` enabled, `/gsd-ship` blocks while `open_count > 0`.
> Waive with `gsd-tools windows waive <id> "<reason>"` (reason required).
> Mark fixed with `gsd-tools windows fixed <id>`.

| id | phase | kind | file | line | description | status | reason | recorded_at | resolved_at |
|----|-------|------|------|------|-------------|--------|--------|-------------|-------------|
| 1 | 01 | stub | .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json |  | Skeleton fields internal_name, repo_asset, dependencies, tranche, advances, deferred deferred_reason are empty on all entries; reconciled by plan 02 and tranche plans 03/04. | open |  | 2026-09-12T05:43:11.920Z |  |
| 2 | 01 | stub | .planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json |  | localization.{en_us,zh_hans} default false for all entries; resolved by plan 05 localization parity. | open |  | 2026-09-12T05:43:12.792Z |  |
| 3 | 01 | unrun-verify | .planning/phases/01-item-inventory-completed-art-items/scripts/check-tranche-A.ps1 |  | 01-03 Task 3 gate not authored: Task 2 tripped its >8 bound (18 class-less completed-art tranche-A entries) and the plan stopped. | open |  | 2026-09-12T06:29:39.672Z |  |
| 4 | 01 | deviation | .planning/phases/01-item-inventory-completed-art-items/01-03-SUMMARY.md |  | 01-03 Task 1 expanded from 2 declared files to 20 (D-12 numeric design parity); prose effects/set bonuses/recipes remain un-reconciled. | open |  | 2026-09-12T06:29:40.352Z |  |
| 5 | 01 | stub | Sources/Modules/Yggdrasil/KelpCurtain/Items/Pets/Photophore.cs |  | Photophore implements the Jade Fruit permanent-booster behaviour instead of the designed light pet; blocked, no pet asset exists | open |  | 2026-09-12T10:27:09.847Z |  |
| 6 | 01 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Witherbark/WitherbarkHelmet.cs |  | Witherbark set-bonus summoned minion attack pattern differs from the design (spikes 14 dmg/30f vs the wired dash+8-leaf burst in Projectiles/Summon); recorded as a 套装效果 blocker | open |  | 2026-09-12T10:27:22.325Z |  |
| 7 | 01 | stub | Sources/Everglow/Localization/en-US/Mods.Everglow.Items.*.hjson |  | 13 Phase 1 completed-art item display keys deferred by user directive (localization out of Phase 1): EvilHalbertBarnacle, ArcI, RedAlgaeMagicStaff, RedAlgaeMagicSpellBook, RedAlgaeMagicWhip, CrimsonMoonSap, EmptyWaterStaff, JadeLakeRedAlgae_Item, Photophore, GreenSungloStaff, ActivatedDogStaff, RedAlgaeMinionGyroscope, RedAlgaeMinionStaff | open |  | 2026-09-12T10:54:35.876Z |  |
| 8 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-02-PLAN.md |  | Plan 02-02 Task 1 human-check (D-21) not run offline: BoulderCatapult must be verified in a tModLoader client to arc under gravity, burst into 3-6 shards, consume no ammo and show damage 44. | open |  | 2026-09-14T10:29:32.758Z |  |
| 9 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-02-PLAN.md |  | Plan 02-02 Task 2 human-check (D-21) not run offline: TendonGreatbow boss-target +10% damage, RestrictionDeviceRE01 consuming 15 mana with no drone, and ReekingBait consuming without spawning require an in-client check. | open |  | 2026-09-14T10:29:33.546Z |  |
| 10 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-03-PLAN.md |  | Plan 02-03 Task 1 human-check (D-21) not run offline: JadeSnakeEgg must be verified in a tModLoader client as consumable (10-gold, Blue) and consuming one without summoning (recorded blocker); BambooStepTalisman must equip into an accessory slot and grant no stats. | open |  | 2026-09-14T10:43:46.626Z |  |
| 11 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-03-PLAN.md |  | Plan 02-03 Task 3 human-check (D-21) not run offline: all six shells plus JadeSnakeEgg must appear in the inventory with a white-box icon, equip/use per declaration, and the tML log must show no missing-resource error. | open |  | 2026-09-14T10:43:47.338Z |  |
| 12 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-04-PLAN.md |  | Runtime human-check (D-21): in a tModLoader client spawn all six 02-04 shells and confirm each appears with a white-box icon, the mod loads with no missing-resource error, and none grants a stat or effect. | open |  | 2026-09-14T10:55:25.450Z |  |
| 13 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-01-PLAN.md |  | Plan 02-01 runtime human-checks (D-21) not run offline: verify in a tModLoader client that CrimsonMoonAlgaeHeaddress equips into the head slot with a white-box icon and defense 8, and that the full four-piece set (crafted at a Work Bench) produces the faster wet swim/run and the ~15% self-heal on a >=10-damage hit. | open |  | 2026-09-14T11:21:21.999Z |  |
| 14 | 02 | unrun-verify | .planning/phases/02-remaining-items-unfinished-art-materials/02-05-PLAN.md |  | Plan 02-05 end-of-phase UAT bundle (D-21) not run offline: one tModLoader client session working the consolidated runtime-verification bundle from plans 02-01..02-04 (armor equip/set, BoulderCatapult arc/shards, TendonGreatbow boss +10%, shell loads with no missing-resource error). | open |  | 2026-09-14T11:21:22.699Z |  |
| 15 | 03 | unrun-verify | .planning/phases/03-completed-art-ordinary-monsters/03-01-PLAN.md |  | Plan 03-01 Task 1 runtime human-check (D-21) not run offline: in a tModLoader client the mossy thorn tortoise must spawn walking in the Kelp Curtain, retract and spin as a vanilla tortoise, show the design spin-state defence/damage switch (10/50 walking, 20/75 spinning), reflect a melee hit taken during the spin, and never appear in an ordinary world (BIO-06). | open |  | 2026-09-15T05:21:05.134Z |  |
| 16 | 03 | unrun-verify | .planning/phases/03-completed-art-ordinary-monsters/03-02-PLAN.md |  | Placeholder probe | fixed |  | 2026-09-15T07:58:20.485Z | 2026-09-15T07:59:10.116Z |
| 17 | 03 | unrun-verify | .planning/phases/03-completed-art-ordinary-monsters/03-02-PLAN.md |  | Plan 03-02 runtime human-checks (D-21) not run offline: in a tModLoader client GuppyConch must crawl slowly in the Kelp Curtain, never attack, deal contact damage, retract for ~2 s after a hit with reduced damage while shelled, drop GuppyShell at 11 percent and never appear in an ordinary world; VerdantRods must drift/circle without chasing, apply poison on ~50 percent of contacts, lose life while submerged and fly back out. | open |  | 2026-09-15T07:59:28.453Z |  |
| 18 | 03 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs |  | Plan 03-02 shell-state transition was placed in HitEffect instead of ModifyIncomingHit: tML documents ModifyIncomingHit as ONLY for HitModifiers properties with side effects belonging to OnHit hooks, and HitEffect is the documented on-hit hook that also runs on the server, so the server-authoritative transition (netmode guard + netUpdate) lives there while ModifyIncomingHit keeps the 0.85/0.70 damage scaling the plan's acceptance criteria require. | open |  | 2026-09-15T07:59:28.473Z |  |
| 19 | 3 | unrun-verify | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs |  | D-21 client checks for 巨树人 and its two attack projectiles (state cycle, smash wave, boulder arc, vulnerability window) are not yet executed; referred to plan 03-04's UAT bundle | open |  | 2026-09-15T08:30:32.937Z |  |
| 20 | 3 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs |  | Plan 03-03 applied the two Rule 1 API-name corrections the plan prose repeated: spawnInfo.player to NPCSpawnInfo.Player (the field) and NPC.rare to NPC.rarity (the engine's only NPC rarity field); both compile-verified by the Release build | open |  | 2026-09-15T08:31:27.481Z |  |
| 21 | 3 | stub | Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs |  | Approved art missing for both 巨树人 attack projectiles: GiantDandelion_Shockwave and GiantDandelion_Boulder request the shared Commons.ModAsset.White_Mod fallback because creating placeholder art is forbidden (D-13 policy); a ground-wave sprite and a boulder sprite are needed from the designer (03-DEVIATIONS.md section 9) | open |  | 2026-09-15T08:31:28.332Z |  |
| 22 | 3 | unrun-verify | .planning/phases/03-completed-art-ordinary-monsters/03-UAT.md |  | D-21 client UAT bundle recorded but not executed: all 8 checks (per-row spawn, main-world isolation, behaviour, combat, drops, dedicated-server/multiplayer, localization fallback, consolidated end-of-phase run) are marked not-executed because no live tModLoader client session is part of Phase 3 | open |  | 2026-09-15T09:06:42.913Z |  |
| 23 | 3 | deviation | .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json |  | Plan 03-04 repaired the corrupted assumptions[]/deviations[] machine copy of the matrix (six 'e' placeholders present since the plan-03-01 tracer commit 421f2a09a) with one entry per ledger section 1-6 plus the phase's six recorded deviations; no other field changed and check-biology.ps1 -RequireAll stays green | open |  | 2026-09-15T09:06:43.775Z |  |
| 24 | 4 | unrun-verify | .planning/phases/04-remaining-ordinary-monsters/04-UAT.md |  | Plan 04-01 Task 1 tracer human-check not executed: the D-21 client bundle (jade spirit anglerfish spawn isolation at the lake floor, its invisibility beyond the reveal range, the reveal dash and its 60/30 damage split, the two drops, a clean loader run with no missing-texture abort, and no main-world spawn in an ordinary world) needs a live tModLoader client; plan 04-09 records it in 04-UAT.md | open |  | 2026-09-16T06:06:59.285Z |  |
| 25 | 4 | deviation | .planning/phases/04-remaining-ordinary-monsters/04-01-PLAN.md |  | Plan 04-01 Task 1 is type=tracer and its verify carries a genuine human-check with no blocking-human gate; the literal checkpoints.md end-of-phase precedence chain (row 4) reads as STOP before the next task. The plan's own why_human states those client checks are the D-21 batch recorded in plan 04-09, and the identical tracer shape in plan 03-01 was executed the same way, so task 2 ran after the automated verify was re-run green and the human check is carried into 04-UAT.md instead of halting mid-flight. Documented as a process deviation in 04-01-SUMMARY.md and 04-DEVIATIONS.md section 11 | open |  | 2026-09-16T06:07:11.234Z |  |
| 26 | 4 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/JadeSpiritAnglerfish.cs |  | Plan 04-01 applied the Rule 1 API-name correction the plan prose repeats: NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field), recorded in 04-DEVIATIONS.md section 10; compile-verified by the Release build | open |  | 2026-09-16T06:07:12.736Z |  |
| 27 | 04 | stub | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs |  | probe | fixed |  | 2026-09-16T07:26:36.147Z | 2026-09-16T07:27:25.449Z |
| 28 | 04 | unrun-verify | .planning/phases/04-remaining-ordinary-monsters/04-UAT.md |  | Plan 04-02 runtime human-checks (D-21) not run offline: in a tModLoader client the WaterStrider must spawn only on the Kelp Curtain water surface inside Yggdrasil and never in an ordinary world, keep the design's 60-200 / 45-150 frame dash cadence, hop back to water from land and swim up from the lake bed; the ToxicToad must spawn on dry land, prefer GlowSalamander/RiverSlug over the player, fire poison bubbles and apply the 75/25 Poisoned/Venom split on contact plus a 180-frame 10-damage Venom death cloud; the GlowSalamander must spawn underwater, dash and melee, flee the toad, and run the 60s / 10s / 30-frame moisture and Suffocation cycle with the synced colour variant. Plan 04-09 records this in 04-UAT.md. | open |  | 2026-09-16T07:27:47.144Z |  |
| 29 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs |  | Plan 04-02 prose repeats the NPC.rare = ItemRarityID.White; line for all three creatures; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field), compile-verified by the Release build. The same Rule 1 correction plan 04-01 recorded in 04-DEVIATIONS.md section 10. | open |  | 2026-09-16T07:27:47.754Z |  |
| 30 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonCloud.cs |  | The plan's acceptance token NPC.GetSource_FromAI per projectile file is not expressible inside a ModProjectile (neither Projectile nor ModProjectile exposes an NPC member); the real spawn site is ToxicToad.cs, which calls Projectile.NewProjectile(NPC.GetSource_FromAI(), ...) for both the bubble and the death cloud. Recorded rather than faked with a comment-only token. | open |  | 2026-09-16T07:27:48.377Z |  |
| 31 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs |  | The design's cross-creature hostility is modelled as target preference only: the ToxicToad prefers a nearby GlowSalamander/RiverSlug over the player and the GlowSalamander retreats from a ToxicToad, but tML has no NPC-versus-NPC damage path (hostile projectiles damage players only), so neither actually damages the other. The residual limitation is already recorded in 04-DEVIATIONS.md sections 7 and 13. | open |  | 2026-09-16T07:27:48.982Z |  |
| 32 | 04 | stub | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs |  | The synced three-colour variant slot (NPC.localAI[1], drawn in OnSpawn) is inert until approved art arrives: with the shared Commons.ModAsset.White_Mod fallback there is no sprite to tint, so nothing on screen changes with the variant. Intentional per OQ2 - it exists so the D-49 migration maps the colour sheets onto frames without a class rework. | open |  | 2026-09-16T07:27:49.606Z |  |
| 33 | 04 | unrun-verify | .planning/phases/04-remaining-ordinary-monsters/04-UAT.md |  | Plan 04-05 runtime human-checks (D-21) not run offline: in a tModLoader client each of the three D-45 identity shells (FluorescentHydra, GiantTigerShrimp, CannonBarnacle) must spawn inside the Kelp Curtain layer of the Yggdrasil subworld only and never in an ordinary world, and each must load cleanly with the Commons.ModAsset.White_Mod fallback (no missing-texture abort). Plan 04-09 records this in 04-UAT.md. | open |  | 2026-09-16T07:44:12.761Z |  |
| 34 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs |  | Plan 04-05 prose and acceptance criteria repeat the NPC.rare = ItemRarityID.White; line for all three D-45 identity shells; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field), compile-verified by three Release builds. The fourth occurrence of the same Rule 1 correction (Phase 3, 04-01, 04-02, 04-05) already recorded in 04-DEVIATIONS.md section 10. | open |  | 2026-09-16T07:44:23.971Z |  |
| 35 | 04 | unrun-verify | .planning/phases/04-remaining-ordinary-monsters/04-UAT.md |  | Plan 04-06 runtime human-checks (D-21) not run offline: in a tModLoader client the four AnimatedWitherbarkSoldier variants, CourtCommander and both BrodieFlydragon sizes must spawn inside the Kelp Curtain layer of the Yggdrasil subworld only and never in an ordinary world, the soldiers' neutral-until-provoked reading and each attack pattern must match the design, CourtCommander's summon must stay capped at 1-3 once per aggro entry, and all nine classes must load cleanly with the Commons.ModAsset.White_Mod fallback. Plan 04-09 records this in 04-UAT.md. | open |  | 2026-09-16T08:13:00.787Z |  |
| 36 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs |  | Plan 04-06 Task 1 asks the melee variant to mirror the vanilla dungeon skeleton by cloning its AI and to keep a private state enum over NPC.ai[0]; the class instead owns a local fighter AI (walk/jump/contact) with NPC.aiStyle = -1. Reason: the vanilla fighter aiStyle always acquires a player target, which makes the design's explicit neutral-by-default reading (must_haves truth 3) inexpressible, and co-opting NPC.ai[0] while the cloned AI runs would break its own state machine. The mirrored approach, the pinned defDamage/defDefense and the NPC.ai[0] state enum are all kept; compile-verified by the Task 1 Release build. | open |  | 2026-09-16T08:13:13.812Z |  |
| 37 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs |  | Plan 04-06 prose and acceptance criteria repeat the NPC.rare = ItemRarityID.White; line for all four soldier variants, CourtCommander and both BrodieFlydragon classes; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field). The fifth occurrence of the same Rule 1 correction (Phase 3, 04-01, 04-02, 04-05, 04-06) already recorded in 04-DEVIATIONS.md section 10. | open |  | 2026-09-16T08:13:14.565Z |  |
| 38 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs |  | Plan 04-06's inline Design Values table lists the flydragon 钱币 cells as empty for both sizes, but the design snapshot's own stats table V9zXdPuSdoQD7bxe6IVcdzqrncf gives 20 copper for 普通（标准） and 0 for 普通（小）. Task 3's action instructs reading that table directly, so NPC.value = 20 and NPC.value = 0 were used (D-25: the committed snapshot is the value source). Recorded here because 04-DEVIATIONS.md section 10 may not be edited by this plan. | open |  | 2026-09-16T08:13:26.388Z |  |
| 39 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs |  | Plan 04-06's Task 1 acceptance criteria require NPC.GetSource_FromAI inside the two new projectile files, but a ModProjectile exposes no NPC member, so the call cannot exist there. The token is realised at the real spawn sites (AnimatedWitherbarkSoldierRanged.ThrowBoulder and AnimatedWitherbarkSoldierSpell.UpdateCasting) and named in each projectile's XML doc that describes that spawn site - the plan 04-02 precedent, recorded rather than faked with a comment-only token. | open |  | 2026-09-16T08:13:27.090Z |  |
| 40 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs |  | Plan 04-07 Task 1's acceptance criteria require NPC.GetSource_FromAI inside RedNeedleCaterpillar_Spike.cs, but a ModProjectile exposes no NPC member, so the call cannot exist there. The token is realised at the real spawn site (RedNeedleCaterpillar.UpdateVolley -> FireVolley) and named in the projectile's XML doc that describes that spawn site - the plan 04-02/04-06 precedent, recorded rather than faked with a comment-only token. | open |  | 2026-09-16T08:31:35.001Z |  |
| 41 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs |  | Plan 04-07 Task 2's acceptance criteria require NPC.GetSource_FromAI inside AssassinRaspberry_Spike.cs, but a ModProjectile exposes no NPC member, so the call cannot exist there. The token is realised at the real spawn site (AssassinRaspberry.FireSpikeScatter) and named in the projectile's XML doc that describes that spawn site - the plan 04-02/04-06 precedent, recorded rather than faked with a comment-only token. | open |  | 2026-09-16T08:31:46.018Z |  |
| 42 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs |  | Plan 04-07 Task 3's acceptance criteria list Main.rand.NextBool(3) as a required token in SerpentMoss.cs. The 33% roll is implemented, but through the named constant PoisonChanceDenominator (= 3) that the same task's action prescribes, with the literal form named in the adjacent comment; the behaviour is the same 1-in-3 roll, so the missing literal is a literal-grep false negative rather than an unimplemented behaviour. | open |  | 2026-09-16T08:31:46.653Z |  |
| 43 | 04 | deviation | Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs |  | Plan 04-07's behavior text repeats NPC.rare = ItemRarityID.White; for 红针洋辣子, 阿萨辛覆盘子 and 蛇行苔; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field, Lifeform Analyzer). The sixth occurrence of the same Rule 1 correction (Phase 3, 04-01, 04-02, 04-05, 04-06, 04-07) already recorded in 04-DEVIATIONS.md section 10, so no ledger edit was needed. | open |  | 2026-09-16T08:31:57.941Z |  |

````json
[
  {
    "id": 1,
    "kind": "stub",
    "phase": "01",
    "file": ".planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json",
    "line": null,
    "description": "Skeleton fields internal_name, repo_asset, dependencies, tranche, advances, deferred deferred_reason are empty on all entries; reconciled by plan 02 and tranche plans 03/04.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T05:43:11.920Z",
    "resolved_at": null
  },
  {
    "id": 2,
    "kind": "stub",
    "phase": "01",
    "file": ".planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json",
    "line": null,
    "description": "localization.{en_us,zh_hans} default false for all entries; resolved by plan 05 localization parity.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T05:43:12.792Z",
    "resolved_at": null
  },
  {
    "id": 3,
    "kind": "unrun-verify",
    "phase": "01",
    "file": ".planning/phases/01-item-inventory-completed-art-items/scripts/check-tranche-A.ps1",
    "line": null,
    "description": "01-03 Task 3 gate not authored: Task 2 tripped its >8 bound (18 class-less completed-art tranche-A entries) and the plan stopped.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T06:29:39.672Z",
    "resolved_at": null
  },
  {
    "id": 4,
    "kind": "deviation",
    "phase": "01",
    "file": ".planning/phases/01-item-inventory-completed-art-items/01-03-SUMMARY.md",
    "line": null,
    "description": "01-03 Task 1 expanded from 2 declared files to 20 (D-12 numeric design parity); prose effects/set bonuses/recipes remain un-reconciled.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T06:29:40.352Z",
    "resolved_at": null
  },
  {
    "id": 5,
    "kind": "stub",
    "phase": "01",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Items/Pets/Photophore.cs",
    "line": null,
    "description": "Photophore implements the Jade Fruit permanent-booster behaviour instead of the designed light pet; blocked, no pet asset exists",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T10:27:09.847Z",
    "resolved_at": null
  },
  {
    "id": 6,
    "kind": "deviation",
    "phase": "01",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/Witherbark/WitherbarkHelmet.cs",
    "line": null,
    "description": "Witherbark set-bonus summoned minion attack pattern differs from the design (spikes 14 dmg/30f vs the wired dash+8-leaf burst in Projectiles/Summon); recorded as a 套装效果 blocker",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T10:27:22.325Z",
    "resolved_at": null
  },
  {
    "id": 7,
    "kind": "stub",
    "phase": "01",
    "file": "Sources/Everglow/Localization/en-US/Mods.Everglow.Items.*.hjson",
    "line": null,
    "description": "13 Phase 1 completed-art item display keys deferred by user directive (localization out of Phase 1): EvilHalbertBarnacle, ArcI, RedAlgaeMagicStaff, RedAlgaeMagicSpellBook, RedAlgaeMagicWhip, CrimsonMoonSap, EmptyWaterStaff, JadeLakeRedAlgae_Item, Photophore, GreenSungloStaff, ActivatedDogStaff, RedAlgaeMinionGyroscope, RedAlgaeMinionStaff",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-12T10:54:35.876Z",
    "resolved_at": null
  },
  {
    "id": 8,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-02-PLAN.md",
    "line": null,
    "description": "Plan 02-02 Task 1 human-check (D-21) not run offline: BoulderCatapult must be verified in a tModLoader client to arc under gravity, burst into 3-6 shards, consume no ammo and show damage 44.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T10:29:32.758Z",
    "resolved_at": null
  },
  {
    "id": 9,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-02-PLAN.md",
    "line": null,
    "description": "Plan 02-02 Task 2 human-check (D-21) not run offline: TendonGreatbow boss-target +10% damage, RestrictionDeviceRE01 consuming 15 mana with no drone, and ReekingBait consuming without spawning require an in-client check.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T10:29:33.546Z",
    "resolved_at": null
  },
  {
    "id": 10,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-03-PLAN.md",
    "line": null,
    "description": "Plan 02-03 Task 1 human-check (D-21) not run offline: JadeSnakeEgg must be verified in a tModLoader client as consumable (10-gold, Blue) and consuming one without summoning (recorded blocker); BambooStepTalisman must equip into an accessory slot and grant no stats.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T10:43:46.626Z",
    "resolved_at": null
  },
  {
    "id": 11,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-03-PLAN.md",
    "line": null,
    "description": "Plan 02-03 Task 3 human-check (D-21) not run offline: all six shells plus JadeSnakeEgg must appear in the inventory with a white-box icon, equip/use per declaration, and the tML log must show no missing-resource error.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T10:43:47.338Z",
    "resolved_at": null
  },
  {
    "id": 12,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-04-PLAN.md",
    "line": null,
    "description": "Runtime human-check (D-21): in a tModLoader client spawn all six 02-04 shells and confirm each appears with a white-box icon, the mod loads with no missing-resource error, and none grants a stat or effect.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T10:55:25.450Z",
    "resolved_at": null
  },
  {
    "id": 13,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-01-PLAN.md",
    "line": null,
    "description": "Plan 02-01 runtime human-checks (D-21) not run offline: verify in a tModLoader client that CrimsonMoonAlgaeHeaddress equips into the head slot with a white-box icon and defense 8, and that the full four-piece set (crafted at a Work Bench) produces the faster wet swim/run and the ~15% self-heal on a >=10-damage hit.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T11:21:21.999Z",
    "resolved_at": null
  },
  {
    "id": 14,
    "kind": "unrun-verify",
    "phase": "02",
    "file": ".planning/phases/02-remaining-items-unfinished-art-materials/02-05-PLAN.md",
    "line": null,
    "description": "Plan 02-05 end-of-phase UAT bundle (D-21) not run offline: one tModLoader client session working the consolidated runtime-verification bundle from plans 02-01..02-04 (armor equip/set, BoulderCatapult arc/shards, TendonGreatbow boss +10%, shell loads with no missing-resource error).",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-14T11:21:22.699Z",
    "resolved_at": null
  },
  {
    "id": 15,
    "kind": "unrun-verify",
    "phase": "03",
    "file": ".planning/phases/03-completed-art-ordinary-monsters/03-01-PLAN.md",
    "line": null,
    "description": "Plan 03-01 Task 1 runtime human-check (D-21) not run offline: in a tModLoader client the mossy thorn tortoise must spawn walking in the Kelp Curtain, retract and spin as a vanilla tortoise, show the design spin-state defence/damage switch (10/50 walking, 20/75 spinning), reflect a melee hit taken during the spin, and never appear in an ordinary world (BIO-06).",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T05:21:05.134Z",
    "resolved_at": null
  },
  {
    "id": 16,
    "kind": "unrun-verify",
    "phase": "03",
    "file": ".planning/phases/03-completed-art-ordinary-monsters/03-02-PLAN.md",
    "line": null,
    "description": "Placeholder probe",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-09-15T07:58:20.485Z",
    "resolved_at": "2026-09-15T07:59:10.116Z"
  },
  {
    "id": 17,
    "kind": "unrun-verify",
    "phase": "03",
    "file": ".planning/phases/03-completed-art-ordinary-monsters/03-02-PLAN.md",
    "line": null,
    "description": "Plan 03-02 runtime human-checks (D-21) not run offline: in a tModLoader client GuppyConch must crawl slowly in the Kelp Curtain, never attack, deal contact damage, retract for ~2 s after a hit with reduced damage while shelled, drop GuppyShell at 11 percent and never appear in an ordinary world; VerdantRods must drift/circle without chasing, apply poison on ~50 percent of contacts, lose life while submerged and fly back out.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T07:59:28.453Z",
    "resolved_at": null
  },
  {
    "id": 18,
    "kind": "deviation",
    "phase": "03",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs",
    "line": null,
    "description": "Plan 03-02 shell-state transition was placed in HitEffect instead of ModifyIncomingHit: tML documents ModifyIncomingHit as ONLY for HitModifiers properties with side effects belonging to OnHit hooks, and HitEffect is the documented on-hit hook that also runs on the server, so the server-authoritative transition (netmode guard + netUpdate) lives there while ModifyIncomingHit keeps the 0.85/0.70 damage scaling the plan's acceptance criteria require.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T07:59:28.473Z",
    "resolved_at": null
  },
  {
    "id": 19,
    "kind": "unrun-verify",
    "phase": "3",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs",
    "line": null,
    "description": "D-21 client checks for 巨树人 and its two attack projectiles (state cycle, smash wave, boulder arc, vulnerability window) are not yet executed; referred to plan 03-04's UAT bundle",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T08:30:32.937Z",
    "resolved_at": null
  },
  {
    "id": 20,
    "kind": "deviation",
    "phase": "3",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs",
    "line": null,
    "description": "Plan 03-03 applied the two Rule 1 API-name corrections the plan prose repeated: spawnInfo.player to NPCSpawnInfo.Player (the field) and NPC.rare to NPC.rarity (the engine's only NPC rarity field); both compile-verified by the Release build",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T08:31:27.481Z",
    "resolved_at": null
  },
  {
    "id": 21,
    "kind": "stub",
    "phase": "3",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs",
    "line": null,
    "description": "Approved art missing for both 巨树人 attack projectiles: GiantDandelion_Shockwave and GiantDandelion_Boulder request the shared Commons.ModAsset.White_Mod fallback because creating placeholder art is forbidden (D-13 policy); a ground-wave sprite and a boulder sprite are needed from the designer (03-DEVIATIONS.md section 9)",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T08:31:28.332Z",
    "resolved_at": null
  },
  {
    "id": 22,
    "kind": "unrun-verify",
    "phase": "3",
    "file": ".planning/phases/03-completed-art-ordinary-monsters/03-UAT.md",
    "line": null,
    "description": "D-21 client UAT bundle recorded but not executed: all 8 checks (per-row spawn, main-world isolation, behaviour, combat, drops, dedicated-server/multiplayer, localization fallback, consolidated end-of-phase run) are marked not-executed because no live tModLoader client session is part of Phase 3",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T09:06:42.913Z",
    "resolved_at": null
  },
  {
    "id": 23,
    "kind": "deviation",
    "phase": "3",
    "file": ".planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json",
    "line": null,
    "description": "Plan 03-04 repaired the corrupted assumptions[]/deviations[] machine copy of the matrix (six 'e' placeholders present since the plan-03-01 tracer commit 421f2a09a) with one entry per ledger section 1-6 plus the phase's six recorded deviations; no other field changed and check-biology.ps1 -RequireAll stays green",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-15T09:06:43.775Z",
    "resolved_at": null
  },
  {
    "id": 24,
    "kind": "unrun-verify",
    "phase": "4",
    "file": ".planning/phases/04-remaining-ordinary-monsters/04-UAT.md",
    "line": null,
    "description": "Plan 04-01 Task 1 tracer human-check not executed: the D-21 client bundle (jade spirit anglerfish spawn isolation at the lake floor, its invisibility beyond the reveal range, the reveal dash and its 60/30 damage split, the two drops, a clean loader run with no missing-texture abort, and no main-world spawn in an ordinary world) needs a live tModLoader client; plan 04-09 records it in 04-UAT.md",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T06:06:59.285Z",
    "resolved_at": null
  },
  {
    "id": 25,
    "kind": "deviation",
    "phase": "4",
    "file": ".planning/phases/04-remaining-ordinary-monsters/04-01-PLAN.md",
    "line": null,
    "description": "Plan 04-01 Task 1 is type=tracer and its verify carries a genuine human-check with no blocking-human gate; the literal checkpoints.md end-of-phase precedence chain (row 4) reads as STOP before the next task. The plan's own why_human states those client checks are the D-21 batch recorded in plan 04-09, and the identical tracer shape in plan 03-01 was executed the same way, so task 2 ran after the automated verify was re-run green and the human check is carried into 04-UAT.md instead of halting mid-flight. Documented as a process deviation in 04-01-SUMMARY.md and 04-DEVIATIONS.md section 11",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T06:07:11.234Z",
    "resolved_at": null
  },
  {
    "id": 26,
    "kind": "deviation",
    "phase": "4",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/JadeSpiritAnglerfish.cs",
    "line": null,
    "description": "Plan 04-01 applied the Rule 1 API-name correction the plan prose repeats: NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field), recorded in 04-DEVIATIONS.md section 10; compile-verified by the Release build",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T06:07:12.736Z",
    "resolved_at": null
  },
  {
    "id": 27,
    "kind": "stub",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs",
    "line": null,
    "description": "probe",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-09-16T07:26:36.147Z",
    "resolved_at": "2026-09-16T07:27:25.449Z"
  },
  {
    "id": 28,
    "kind": "unrun-verify",
    "phase": "04",
    "file": ".planning/phases/04-remaining-ordinary-monsters/04-UAT.md",
    "line": null,
    "description": "Plan 04-02 runtime human-checks (D-21) not run offline: in a tModLoader client the WaterStrider must spawn only on the Kelp Curtain water surface inside Yggdrasil and never in an ordinary world, keep the design's 60-200 / 45-150 frame dash cadence, hop back to water from land and swim up from the lake bed; the ToxicToad must spawn on dry land, prefer GlowSalamander/RiverSlug over the player, fire poison bubbles and apply the 75/25 Poisoned/Venom split on contact plus a 180-frame 10-damage Venom death cloud; the GlowSalamander must spawn underwater, dash and melee, flee the toad, and run the 60s / 10s / 30-frame moisture and Suffocation cycle with the synced colour variant. Plan 04-09 records this in 04-UAT.md.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:27:47.144Z",
    "resolved_at": null
  },
  {
    "id": 29,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs",
    "line": null,
    "description": "Plan 04-02 prose repeats the NPC.rare = ItemRarityID.White; line for all three creatures; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field), compile-verified by the Release build. The same Rule 1 correction plan 04-01 recorded in 04-DEVIATIONS.md section 10.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:27:47.754Z",
    "resolved_at": null
  },
  {
    "id": 30,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonCloud.cs",
    "line": null,
    "description": "The plan's acceptance token NPC.GetSource_FromAI per projectile file is not expressible inside a ModProjectile (neither Projectile nor ModProjectile exposes an NPC member); the real spawn site is ToxicToad.cs, which calls Projectile.NewProjectile(NPC.GetSource_FromAI(), ...) for both the bubble and the death cloud. Recorded rather than faked with a comment-only token.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:27:48.377Z",
    "resolved_at": null
  },
  {
    "id": 31,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs",
    "line": null,
    "description": "The design's cross-creature hostility is modelled as target preference only: the ToxicToad prefers a nearby GlowSalamander/RiverSlug over the player and the GlowSalamander retreats from a ToxicToad, but tML has no NPC-versus-NPC damage path (hostile projectiles damage players only), so neither actually damages the other. The residual limitation is already recorded in 04-DEVIATIONS.md sections 7 and 13.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:27:48.982Z",
    "resolved_at": null
  },
  {
    "id": 32,
    "kind": "stub",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs",
    "line": null,
    "description": "The synced three-colour variant slot (NPC.localAI[1], drawn in OnSpawn) is inert until approved art arrives: with the shared Commons.ModAsset.White_Mod fallback there is no sprite to tint, so nothing on screen changes with the variant. Intentional per OQ2 - it exists so the D-49 migration maps the colour sheets onto frames without a class rework.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:27:49.606Z",
    "resolved_at": null
  },
  {
    "id": 33,
    "kind": "unrun-verify",
    "phase": "04",
    "file": ".planning/phases/04-remaining-ordinary-monsters/04-UAT.md",
    "line": null,
    "description": "Plan 04-05 runtime human-checks (D-21) not run offline: in a tModLoader client each of the three D-45 identity shells (FluorescentHydra, GiantTigerShrimp, CannonBarnacle) must spawn inside the Kelp Curtain layer of the Yggdrasil subworld only and never in an ordinary world, and each must load cleanly with the Commons.ModAsset.White_Mod fallback (no missing-texture abort). Plan 04-09 records this in 04-UAT.md.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:44:12.761Z",
    "resolved_at": null
  },
  {
    "id": 34,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs",
    "line": null,
    "description": "Plan 04-05 prose and acceptance criteria repeat the NPC.rare = ItemRarityID.White; line for all three D-45 identity shells; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field), compile-verified by three Release builds. The fourth occurrence of the same Rule 1 correction (Phase 3, 04-01, 04-02, 04-05) already recorded in 04-DEVIATIONS.md section 10.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T07:44:23.971Z",
    "resolved_at": null
  },
  {
    "id": 35,
    "kind": "unrun-verify",
    "phase": "04",
    "file": ".planning/phases/04-remaining-ordinary-monsters/04-UAT.md",
    "line": null,
    "description": "Plan 04-06 runtime human-checks (D-21) not run offline: in a tModLoader client the four AnimatedWitherbarkSoldier variants, CourtCommander and both BrodieFlydragon sizes must spawn inside the Kelp Curtain layer of the Yggdrasil subworld only and never in an ordinary world, the soldiers' neutral-until-provoked reading and each attack pattern must match the design, CourtCommander's summon must stay capped at 1-3 once per aggro entry, and all nine classes must load cleanly with the Commons.ModAsset.White_Mod fallback. Plan 04-09 records this in 04-UAT.md.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:13:00.787Z",
    "resolved_at": null
  },
  {
    "id": 36,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs",
    "line": null,
    "description": "Plan 04-06 Task 1 asks the melee variant to mirror the vanilla dungeon skeleton by cloning its AI and to keep a private state enum over NPC.ai[0]; the class instead owns a local fighter AI (walk/jump/contact) with NPC.aiStyle = -1. Reason: the vanilla fighter aiStyle always acquires a player target, which makes the design's explicit neutral-by-default reading (must_haves truth 3) inexpressible, and co-opting NPC.ai[0] while the cloned AI runs would break its own state machine. The mirrored approach, the pinned defDamage/defDefense and the NPC.ai[0] state enum are all kept; compile-verified by the Task 1 Release build.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:13:13.812Z",
    "resolved_at": null
  },
  {
    "id": 37,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs",
    "line": null,
    "description": "Plan 04-06 prose and acceptance criteria repeat the NPC.rare = ItemRarityID.White; line for all four soldier variants, CourtCommander and both BrodieFlydragon classes; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field). The fifth occurrence of the same Rule 1 correction (Phase 3, 04-01, 04-02, 04-05, 04-06) already recorded in 04-DEVIATIONS.md section 10.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:13:14.565Z",
    "resolved_at": null
  },
  {
    "id": 38,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs",
    "line": null,
    "description": "Plan 04-06's inline Design Values table lists the flydragon 钱币 cells as empty for both sizes, but the design snapshot's own stats table V9zXdPuSdoQD7bxe6IVcdzqrncf gives 20 copper for 普通（标准） and 0 for 普通（小）. Task 3's action instructs reading that table directly, so NPC.value = 20 and NPC.value = 0 were used (D-25: the committed snapshot is the value source). Recorded here because 04-DEVIATIONS.md section 10 may not be edited by this plan.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:13:26.388Z",
    "resolved_at": null
  },
  {
    "id": 39,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs",
    "line": null,
    "description": "Plan 04-06's Task 1 acceptance criteria require NPC.GetSource_FromAI inside the two new projectile files, but a ModProjectile exposes no NPC member, so the call cannot exist there. The token is realised at the real spawn sites (AnimatedWitherbarkSoldierRanged.ThrowBoulder and AnimatedWitherbarkSoldierSpell.UpdateCasting) and named in each projectile's XML doc that describes that spawn site - the plan 04-02 precedent, recorded rather than faked with a comment-only token.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:13:27.090Z",
    "resolved_at": null
  },
  {
    "id": 40,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs",
    "line": null,
    "description": "Plan 04-07 Task 1's acceptance criteria require NPC.GetSource_FromAI inside RedNeedleCaterpillar_Spike.cs, but a ModProjectile exposes no NPC member, so the call cannot exist there. The token is realised at the real spawn site (RedNeedleCaterpillar.UpdateVolley -> FireVolley) and named in the projectile's XML doc that describes that spawn site - the plan 04-02/04-06 precedent, recorded rather than faked with a comment-only token.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:31:35.001Z",
    "resolved_at": null
  },
  {
    "id": 41,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs",
    "line": null,
    "description": "Plan 04-07 Task 2's acceptance criteria require NPC.GetSource_FromAI inside AssassinRaspberry_Spike.cs, but a ModProjectile exposes no NPC member, so the call cannot exist there. The token is realised at the real spawn site (AssassinRaspberry.FireSpikeScatter) and named in the projectile's XML doc that describes that spawn site - the plan 04-02/04-06 precedent, recorded rather than faked with a comment-only token.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:31:46.018Z",
    "resolved_at": null
  },
  {
    "id": 42,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs",
    "line": null,
    "description": "Plan 04-07 Task 3's acceptance criteria list Main.rand.NextBool(3) as a required token in SerpentMoss.cs. The 33% roll is implemented, but through the named constant PoisonChanceDenominator (= 3) that the same task's action prescribes, with the literal form named in the adjacent comment; the behaviour is the same 1-in-3 roll, so the missing literal is a literal-grep false negative rather than an unimplemented behaviour.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:31:46.653Z",
    "resolved_at": null
  },
  {
    "id": 43,
    "kind": "deviation",
    "phase": "04",
    "file": "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs",
    "line": null,
    "description": "Plan 04-07's behavior text repeats NPC.rare = ItemRarityID.White; for 红针洋辣子, 阿萨辛覆盘子 and 蛇行苔; NPC.rare does not exist in this tML build and was written as NPC.rarity (the engine's only NPC rarity field, Lifeform Analyzer). The sixth occurrence of the same Rule 1 correction (Phase 3, 04-01, 04-02, 04-05, 04-06, 04-07) already recorded in 04-DEVIATIONS.md section 10, so no ledger edit was needed.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-09-16T08:31:57.941Z",
    "resolved_at": null
  }
]
````
