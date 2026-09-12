---
schema_version: 1
open_count: 7
waived_count: 0
fixed_count: 0
total_count: 7
last_updated: 2026-09-12T10:54:35.876Z
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
  }
]
````
