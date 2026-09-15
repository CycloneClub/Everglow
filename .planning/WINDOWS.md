---
schema_version: 1
open_count: 20
waived_count: 0
fixed_count: 1
total_count: 21
last_updated: 2026-09-15T08:31:28.332Z
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
  }
]
````
