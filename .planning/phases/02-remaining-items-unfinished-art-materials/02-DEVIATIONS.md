# Phase 2 Deviation Ledger — Remaining Items & Unfinished-Art Materials

Generated: 2026-09-14
Consolidated by plan 02-05 from the sections plans 02-01 to 02-04 appended.
Machine sources of truth: `01-INVENTORY.json` (per-entry status, blockers, `deviations[]`) and `02-CLASSIFICATION.json` (mode / blocker_kind / implemented).

This is the single consolidated ledger for the whole phase. It records every deviation, assumption and unresolved blocker met while implementing the remaining non-boss item scope (D-18 full implementations and identity-only shell classes). It is the audit trail for D-23 (implemented code wins unless the Feishu row is yellow with an explanation), D-11/D-22 (code-complete + art-incomplete marking), D-19 (system blockers), D-20 (localization deferred) and D-21 (runtime checks outstanding). No field below was changed by editing the Feishu design source, no localization key was created or renamed, and no `.png` or other binary/art asset was created or modified.

**Coverage claim.** This ledger covers all 21 implemented Phase 2 entries. Every implemented entry appears at least once in §2 Blockers with an `artwork` row, and a Phase 2 entry absent from this ledger is a defect.

## 1. Scope and Allocation

The Phase 1 inventory allocated **25 rows to `phase == 2`**. Phase 2 disposes of them as:

| Group | Count | Entries / rule |
| --- | --- | --- |
| Deferred by D-15 | 3 | `item-weapons.misc-a`, `item-weapons.misc-b`, `item-weapons.misc-c-名字要普通` (design names A/B/C, all `name_zh = 阵法修复材料`, no Feishu checkbox). These are the design team's own placeholders: they stay `deferred: true` with a non-empty `deferred_reason` and are **not** implemented in Phase 2. |
| Reallocated by D-16 | 1 | `biology_drop-weapons.misc-巨翼龙面具` (巨翼龙面具) is a Giant Winged Dragon reward reallocated from Phase 2 to **Phase 7 (ITEM-06)**. Its `phase` is set to `7` and `ITEM-06` is appended to `advances`; its Feishu fields, checkboxes, `artwork_complete`, `code_complete` and `status` are unchanged. |
| Implemented by Phase 2 | 21 | The remaining non-deferred `phase == 2` rows, classified in `02-CLASSIFICATION.json`. |

The 21 implemented entries split into **9 full implementations** (D-18) and **12 identity-only shell classes**.

**9 full implementations:** 肌腱巨弓 (`biology_drop-weapons.ranged-肌腱巨弓`), 限制机 (`biology_drop-weapons.summon-re01`), 腥臭的诱饵 (`biology_drop-weapons.summon-腥臭的诱饵`), 巨石弹射装置 (`item-weapons.misc-巨石弹射装置`), 红月水藻头饰 (`item-weapons.magic-红月水藻头饰`), 红月水藻面具 (`item-weapons.summon-红月水藻面具`), 红月水藻板甲 (`item-weapons.misc-红月水藻板甲`), 红月水藻护胫 (`item-weapons.misc-红月水藻护胫`), 灵蛇玉卵 (`item-weapons.summon-灵蛇玉卵`).

**12 identity-only shell classes:** 竹节步符 (`item-weapons.misc-竹节步符`), 竹制武器 (`item-weapons.misc-竹制武器`), 竹簪子 (`item-weapons.misc-竹簪子`), 桃枝护符 (`item-weapons.misc-桃枝护符`), 桃花纸鸢（风筝） (`item-weapons.misc-桃花纸鸢-风筝`), 熊猫宠物 (`item-weapons.misc-熊猫宠物`), 若干酒类 (`item-weapons.misc-若干酒类`), 弟子剑 (`item-weapons.melee-弟子剑`), 弟子时装 (`item-weapons.misc-弟子时装`), 技能竹简 (`item-weapons.misc-技能竹简`), 区域放置物品制作台 (`item-weapons.misc-区域放置物品制作台`), 荧光水螅召唤杖 (`item-weapons.summon-荧光水螅召唤杖`).

`blocker_kind` is `system` for the four items whose design implies an unimplemented system — 弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台 (D-19) — and `artwork` for the other 17.

All 21 implemented rows carry `code_complete: true`, `artwork_complete: false` and `status: "unchecked"` (D-11/D-22): code completion is a repository fact, while no Feishu row receives a colour until the artwork exists (PROJECT.md Design Status Synchronization rule 5).

## 2. Blockers

Every Phase 2 implemented entry carries at least one precise blocker. The table below is the consolidated register: one row per blocker, with the kind drawn from the vocabulary `artwork`, `system`, `effect`, `recipe`. All 21 entries appear with an `artwork` row (every class uses the shared fallback texture), plus the additional `system` / `effect` / `recipe` rows for the entries that carry them. Each **Exact blocker text** cell is byte-identical to the corresponding element of the entry's `blockers` array in `01-INVENTORY.json`.

| Entry id | Design name | Blocker kind | Exact blocker text | Why |
| --- | --- | --- | --- | --- |
| `biology_drop-weapons.ranged-肌腱巨弓` | 肌腱巨弓 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `biology_drop-weapons.ranged-肌腱巨弓` | 肌腱巨弓 | recipe | `recipe blocked (Phase 7 dependency absent): design 合成方式 lists 血云母 / 血肉聚合物 / 玉化龙骨, Phase 7 Giant Winged Dragon items that do not exist in the repository, so no AddRecipes body was written` | The design recipe names only absent Phase 7 Giant Winged Dragon items; referencing them would not compile. |
| `biology_drop-weapons.ranged-肌腱巨弓` | 肌腱巨弓 | effect | `effect blocked (missing design parameters): 按住左键蓄力拉出大弓 documents no charge time, damage curve or release behaviour` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `biology_drop-weapons.summon-re01` | 限制机 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `biology_drop-weapons.summon-re01` | 限制机 | recipe | `recipe blocked (Phase 7 dependency absent): design 合成方式 lists 血云母 / 血肉聚合物 / 熔炉钢 / 隐生之眼, Phase 7 Giant Winged Dragon items absent from the repository` | The design recipe names only absent Phase 7 Giant Winged Dragon items; referencing them would not compile. |
| `biology_drop-weapons.summon-re01` | 限制机 | effect | `effect blocked (absent repository type): the 限制无人机 summon projectile, its 聚能射线 beam, multi-target damage splitting and 浊燃 accumulation do not exist in the repository; Item.shoot is deliberately not set` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `biology_drop-weapons.summon-腥臭的诱饵` | 腥臭的诱饵 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `biology_drop-weapons.summon-腥臭的诱饵` | 腥臭的诱饵 | recipe | `recipe blocked (Phase 7 dependency absent): design 合成方式 lists 血云母 / 干枯心脏, Phase 7 Giant Winged Dragon items absent from the repository` | The design recipe names only absent Phase 7 Giant Winged Dragon items; referencing them would not compile. |
| `biology_drop-weapons.summon-腥臭的诱饵` | 腥臭的诱饵 | effect | `effect blocked (absent encounter): the 巨翼龙 encounter is Phase 7; the item consumes without spawning anything and references no NPC type` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `item-weapons.magic-红月水藻头饰` | 红月水藻头饰 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.magic-红月水藻头饰` | 红月水藻头饰 | effect | `effect blocked (outside this plan's file scope): set toxin clauses (红藻毒素Buff duration doubling, 免疫和红藻相关的所有伤害) and the greaves 免疫红藻减速 clause need the red-algae toxin/slow system or edits to accepted Phase-1 applier projectiles` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `item-weapons.melee-弟子剑` | 弟子剑 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.melee-弟子剑` | 弟子剑 | system | `system blocked (D-19): the item belongs to the 弟子 (disciple) progression system, which is not implemented in this phase; the class is an identity only and no disciple-system code exists` | The entry belongs to a system that is deliberately not built in Phase 2 (D-19). |
| `item-weapons.misc-区域放置物品制作台` | 区域放置物品制作台 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-区域放置物品制作台` | 区域放置物品制作台 | system | `system blocked (D-19): the item belongs to the regional-crafting system, which is not implemented in this phase, and the placement tile it will need does not exist either; the plain ModItem declares no ModTile, no Item.createTile and no DefaultToPlaceableTile call (design 描述 宝箱的副掉落，非核心物品)` | The entry belongs to a system that is deliberately not built in Phase 2 (D-19). |
| `item-weapons.misc-巨石弹射装置` | 巨石弹射装置 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-弟子时装` | 弟子时装 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-弟子时装` | 弟子时装 | system | `system blocked (D-19): the item belongs to the 弟子 (disciple) progression system, which is not implemented in this phase; the vanity declares no equip slot and the class is an identity only` | The entry belongs to a system that is deliberately not built in Phase 2 (D-19). |
| `item-weapons.misc-技能竹简` | 技能竹简 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-技能竹简` | 技能竹简 | system | `system blocked (D-19): the design 描述 提交给NPC后学习 places the item in the skill system, which is not implemented in this phase; the class is an identity only` | The entry belongs to a system that is deliberately not built in Phase 2 (D-19). |
| `item-weapons.misc-桃枝护符` | 桃枝护符 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-桃枝护符` | 桃枝护符 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-桃花纸鸢-风筝` | 桃花纸鸢（风筝） | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-桃花纸鸢-风筝` | 桃花纸鸢（风筝） | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells and the 描述 宝箱的副掉落，非核心物品, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-熊猫宠物` | 熊猫宠物 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-熊猫宠物` | 熊猫宠物 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-竹制武器` | 竹制武器 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-竹制武器` | 竹制武器 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-竹簪子` | 竹簪子 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-竹簪子` | 竹簪子 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-竹节步符` | 竹节步符 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-竹节步符` | 竹节步符 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-红月水藻护胫` | 红月水藻护胫 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-红月水藻护胫` | 红月水藻护胫 | effect | `effect blocked (outside this plan's file scope): set toxin clauses (红藻毒素Buff duration doubling, 免疫和红藻相关的所有伤害) and the greaves 免疫红藻减速 clause need the red-algae toxin/slow system or edits to accepted Phase-1 applier projectiles` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `item-weapons.misc-红月水藻板甲` | 红月水藻板甲 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-红月水藻板甲` | 红月水藻板甲 | effect | `effect blocked (outside this plan's file scope): set toxin clauses (红藻毒素Buff duration doubling, 免疫和红藻相关的所有伤害) and the greaves 免疫红藻减速 clause need the red-algae toxin/slow system or edits to accepted Phase-1 applier projectiles` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `item-weapons.misc-若干酒类` | 若干酒类 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.misc-若干酒类` | 若干酒类 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries no 伤害 / 价格 / 稀有度 / 效果 cells (its only populated cell is 描述 只喝了一两口就丢进去了), so the approved artwork and behaviour are undefined; the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.summon-灵蛇玉卵` | 灵蛇玉卵 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.summon-灵蛇玉卵` | 灵蛇玉卵 | effect | `effect blocked (absent encounter + location gate): the 苍翠灵蛇 encounter is Phase 7 scope, and the documented use condition 在森雨幽谷顶部使用 (use at the top of the Valley of Lush and Moist) requires a location gate that the encounter phase owns; the item consumes without summoning anything and references no NPC type` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `item-weapons.summon-红月水藻面具` | 红月水藻面具 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.summon-红月水藻面具` | 红月水藻面具 | effect | `effect blocked (outside this plan's file scope): set toxin clauses (红藻毒素Buff duration doubling, 免疫和红藻相关的所有伤害) and the greaves 免疫红藻减速 clause need the red-algae toxin/slow system or edits to accepted Phase-1 applier projectiles` | The effect depends on an absent encounter, system or accepted Phase 1 applier outside this phase's scope (D-21/D-23). |
| `item-weapons.summon-荧光水螅召唤杖` | 荧光水螅召唤杖 | artwork | `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |
| `item-weapons.summon-荧光水螅召唤杖` | 荧光水螅召唤杖 | artwork | `artwork blocked + identity-only shell (D-18): the design row carries no 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the 荧光水螅 summon projectile does not exist in the repository, so Item.shoot is not set and the class declares an item identity only` | Every implemented class reuses `Commons.ModAsset.White_Mod` because no approved art exists in the repository (D-13); the row stays unchecked (D-11/D-22). |

### 2.1 D-13 migration path

Every implemented class declares `public override string Texture => Commons.ModAsset.White_Mod;` (the shared fallback) so the mod loads with no approved art. When approved art arrives, the change is adding `<Class>.png` (and `<Class>_Head.png` / `_Body.png` / `_Legs.png` for the armor) beside the `.cs` and repointing the class's `Texture` / `Load()` registration from the shared fallback to the class's own asset path — a texture addition plus a blocker removal, not a class rework (D-14). No placeholder art was created and no `.png` or other binary asset was added or modified.

### 2.2 Outstanding set-clause effect blockers (E-1 / E-2)

The 红月水藻 four-piece set carries one `effect` blocker. Two of its design clauses cannot be completed inside this phase's file scope.

**Blocker E-1 — 红藻毒素Buff duration doubling (15 s -> 30 s).** The applied toxin duration is written by the accepted Phase 1 red-algae appliers, which are outside this phase's modify set. Implementing the doubling requires coordinated edits at these exact call sites:

- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/RedAlgaeMinionGyroscope_Proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/RedAlgaeMagicWhip_Proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/CrimsonMoonAlgaeSummonStaff_minion_spore.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/CrimsonMoonAlgaeSummonStaff_minion_Explosion.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Magic/RedAlgaeMagicStaff_Proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Magic/RedAlgaeMagicSpellBook_proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicWhip.cs`

**Blocker E-2 — 免疫红藻减速.** No player-facing red-algae **slow** effect exists in the repository, so there is nothing additional to immunize against beyond the one player red-algae effect that does exist: `RedAlgaeDebuff` (its `lifeRegen` drain). The implementable part of the immunity clause is applied — both head pieces set `player.buffImmune[ModContent.BuffType<RedAlgaeDebuff>()] = true` in `UpdateArmorSet`, which immunizes the set wearer against that debuff. The 减速 clause stays recorded until a red-algae slow effect is designed/implemented.

The hard-coded `900` accumulation reference has since been replaced by the shared `RedAlgae_FriendlyDebuff.Duration` constant (code-review fix WR-01, commit `8d06b7a34`); the `damage > 10` gate and the `DelBuff` call in `RedAlgae_FriendlyDebuff_glocalNPC` are unchanged.

### 2.3 Recipe blockers

No `AddRecipes` body is written for 肌腱巨弓, 限制机 or 腥臭的诱饵. Each design 合成方式 cell names only Phase 7 Giant Winged Dragon items that are absent from the repository, and an item-type reference to an absent type does not compile.

| Entry id | Design 合成方式 | Absent Phase 7 types | Reason no recipe |
| --- | --- | --- | --- |
| `biology_drop-weapons.ranged-肌腱巨弓` | 血云母 + 血肉聚合物 + 玉化龙骨 | 血云母, 血肉聚合物, 玉化龙骨 | Giant Winged Dragon reward items, Phase 7 |
| `biology_drop-weapons.summon-re01` | 血云母 + 血肉聚合物 + 熔炉钢 + 隐生之眼 | 血云母, 血肉聚合物, 熔炉钢, 隐生之眼 | Giant Winged Dragon reward items, Phase 7 |
| `biology_drop-weapons.summon-腥臭的诱饵` | 血云母 + 干枯心脏 | 血云母, 干枯心脏 | Giant Winged Dragon reward items, Phase 7 |

### 2.4 Implemented effects and shell shape (reference)

The design values the full implementations transcribe:

| Entry id | Class | 伤害 | 击退 | 暴击 | 使用时间 | 价格 | 稀有度 | Implemented effect |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `item-weapons.misc-巨石弹射装置` | `BoulderCatapult` (+ `BoulderCatapult_Proj` / `BoulderCatapult_SubProj`) | 44 | 15 | — | 77 | 2金 -> `Item.buyPrice(gold: 2)` | 橙色 -> `ItemRarityID.Orange` | Fires its own boulder (no `Item.useAmmo`, 不消耗子弹); the direct hit totals 150% (base hit plus a 50% owner-guarded second hit); on death it bursts into 3-6 `BoulderCatapult_SubProj` at 15% damage |
| `biology_drop-weapons.ranged-肌腱巨弓` | `TendonGreatbow` (+ `TendonGreatbow_Arrow`) | 58 | 强 -> 8f | 12% | 28 | 4g -> `Item.buyPrice(gold: 4)` | 粉 -> `ItemRarityID.Pink` | Consumes arrows and fires `TendonGreatbow_Arrow`; +10% final damage against boss targets |
| `biology_drop-weapons.summon-re01` | `RestrictionDeviceRE01` | 18 | 弱 -> 2f | / | 21 | 4g -> `Item.buyPrice(gold: 4)` | 粉 -> `ItemRarityID.Pink` | Identity and stats only: no `Item.shoot` and no recipe (blockers in §2) |
| `biology_drop-weapons.summon-腥臭的诱饵` | `ReekingBait` | — | — | — | — | 20S -> `Item.buyPrice(silver: 20)` | 蓝 -> `ItemRarityID.Blue` | Consumable summon identity only: no NPC spawn and no recipe (blockers in §2) |
| `item-weapons.summon-灵蛇玉卵` | `JadeSnakeEgg` | — | — | — | 12 | 10金 -> `Item.buyPrice(gold: 10)` | 蓝色 -> `ItemRarityID.Blue` | Consumable use item (`SummonItems` category, 12-frame swing, `UseSound = SoundID.Roar`); consumes one without summoning anything (blocker in §2) |

The 红月水藻 four-piece set implements: magic head (+18% damage, -12% mana cost), summon mask (+18% damage, +3 minions), breastplate (15% self-heal on hits >= 10), greaves (+12% speed, +24% more while wet); set detection accepts either head piece, adds `+50` max mana and `RedAlgaeDebuff` immunity, and scales the set-flagged toxin detonation by 2.5x. Per-piece/set effects run through `KelpCurtainPlayer.CrimsonMoonAlgaeBreastPlate`, `CrimsonMoonAlgaeGreaves` and `CrimsonMoonAlgaeSetBuff`; the recipe assumption is DD-01.

All **12 identity-only shells** (D-18) share the same conservative shape: `Item.width = Item.height = 20`, `Item.value = Item.buyPrice(silver: 50)`, `Item.rare = ItemRarityID.Blue`, and the D-13 `Texture` override with the standard two-line comment. Per-shell adjustments: 竹制武器 `Item.maxStack = 1`; 桃花纸鸢（风筝） `Item.maxStack = Item.CommonMaxStack`; 竹节步符 and 桃枝护符 `Item.accessory = true`; 竹簪子 and 弟子时装 `Item.vanity = true` with no equip slot; 若干酒类, 技能竹简 and 区域放置物品制作台 `Item.maxStack = Item.CommonMaxStack`. The extents, value and rarity mirror the accepted art-missing precedent `Items/Misc/ForestBreath.cs` because the design rows supply none; no shell declares `AddRecipes`, `Item.shoot`, `UpdateAccessory`, an equip registration, a `ModTile` or `Item.createTile`.

## 3. Design Deviations (D-23)

Per PROJECT.md Design Status Synchronization rule 6 (D-23), when implemented code conflicts with the design the implemented code wins unless the Feishu row is marked yellow with a corresponding explanation. No accepted Phase 1 class is re-aligned to design numbers. The full deviation register:

| # | Entry / scope | Deviation | Rationale |
| --- | --- | --- | --- |
| DD-01 | 红月水藻 four-piece set (头饰 / 面具 / 板甲 / 护胫) | **Recipe assumed.** The design table `D48ndhX87o9stux04tDc3aBonZd` supplies no recipe for this set (only 其他数值 / 价格 / 稀有度 / 效果 / 描述). Each piece mirrors the accepted red-algae family recipe: **15 × `JadeLakeRedAlgae_Item` + 1 × `CrimsonMoonSap` at a Work Bench (`TileID.WorkBenches`)**, the exact shape already implemented in `RedAlgaeMinionStaff.AddRecipes()`. | Dependency order (D-17): the set consumes materials that already exist; a recipe keeps the branch obtainable without inventing art or systems. Recorded here for designer confirmation. |
| DD-02 | 红月水藻 four-piece set | **Rarity mapping.** Design `浅橙` maps to `ItemRarityID.Orange`, matching the accepted red-algae family (`RedAlgaeMinionStaff`, `RedAlgaeMagicStaff`). | Phase 1 Pitfall-6 mapping; the design gives no engine rarity id. |
| DD-03 | Biology-table weapons (肌腱巨弓, 限制机, 腥臭的诱饵, etc.) | **Qualitative 击退 mapping record.** The design uses a qualitative 击退 cell (e.g. 强) rather than a numeric value; the concrete values chosen are recorded as DD-05. | The design gives no numeric knockback. |
| DD-04 | 红月水藻 four-piece set | **Parser `category` artefact.** The four armor rows carry parser categories `weapons.magic` / `weapons.summon` / `weapons.misc` because the design puts them in the item/weapons tables, while the content is armor (`LocalizationUtils.Categories.Armor`). The `id` and `category` fields are left unchanged because the id is compatibility-sensitive (Pitfall 5). | The design row is armor; the parser heuristic keyed off the table section. Recorded per D-23 (implemented code wins). |
| DD-05 | 肌腱巨弓, 限制机 | **Numeric 击退 mapping.** 强 -> `Item.knockBack = 8f`; 弱 -> `2f`, calibrated against the accepted family scale (`GreenThornBallLauncher` 5.5, `QuetzalsWish` 6, `RedAlgaeMinionStaff` 2). | The design gives no numeric knockback; pending designer confirmation. |
| DD-06 | 肌腱巨弓 | **Damage correction.** The committed `evidence/biology.xml` row gives 伤害 58 (击退 强, 暴击 12%, 使用时间 28, 价格 4g, 稀有度 粉); an earlier `02-RESEARCH.md` draft reported 28 by misreading the `28（慢）` use-time cell as damage. The revised RESEARCH and the implementation both use 58. | Evidence over research; recorded per D-23. |
| DD-07 | 竹簪子, 弟子时装 | **Vanity placement.** Both vanity shells live in `Items/Misc/` rather than a new `Items/Vanity/` folder: no `Items/Vanity/` directory exists anywhere in the repository and the only tracked vanity item, `Items/Misc/WitheredMask.cs`, lives beside the misc items. The pre-existing `02-CLASSIFICATION.json` paths `Items/Vanity/BambooHairpin.cs` and `Items/Vanity/DiscipleVanity.cs` were corrected. | The vanity folder was a planner assumption, not repository structure; the tracked `WitheredMask` precedent is authoritative. |
| DD-08 | 灵蛇玉卵 | **Use-item category artefact.** The inventory category is `weapons.summon` from the parser heuristic while the design row is a use-item egg; the content is implemented as a consumable use item and the artefact is recorded (D-23), leaving the entry id unchanged because it is compatibility-sensitive (Pitfall 5). | The parser heuristic keys off the design table section, not the content type. |
| DD-09 | 竹簪子, 桃枝护符 | **Category from the 描述 cell.** 竹簪子's design 描述 reads 时装 (vanity) and 桃枝护符's reads 饰品 (accessory), which select the `Vanity` and `Accessories` categories; both rows are otherwise empty. | The 描述 cell is the only category evidence in the row. |
| DD-10 | 桃花纸鸢（风筝） | **Non-core chest side drop.** The design 描述 reads 宝箱的副掉落，非核心物品, so no behaviour is defined beyond the item identity. | Recorded so the shell is not mistaken for an unfinished core item. |
| DD-11 | 技能竹简 | **Shell placement.** `Items/Misc/SkillBambooSlip.cs` rather than `02-PATTERNS.md`'s `Items/Weapons/`: the design 描述 提交给NPC后学习 (learned after submission to an NPC) and the empty combat columns make it a quest submission item; `Items/Misc/ForestBreath.cs` is the closer analog. The pre-existing `02-CLASSIFICATION.json` path was corrected. | The planner's folder guess was superseded by the design evidence and the repository analog. |
| DD-12 | 弟子时装 | **Shell placement.** `Items/Misc/DiscipleVanity.cs`, following DD-07. The pre-existing `02-CLASSIFICATION.json` path was corrected. | Same vanity-placement rule as DD-07. |
| DD-13 | 区域放置物品制作台 | **Shell placement.** `Items/Placeables/RegionalCraftingStation.cs`, chosen to keep its intended content type (`Placeables`) visible even though it stays a plain `ModItem` until the regional-crafting system and its tile exist. | Content-type visibility over folder symmetry. |

The head row's shared `rowspan="4"` 描述 cell is read as the **set bonus** (the set-bonus text spans all four armor rows): 红藻毒素Buff accumulates without damage and detonates on the next hit for damage scaled by the accumulated duration; the applied 红藻毒素Buff duration is doubled (15 s -> 30 s, blocker E-1); 引爆红藻毒素 damage +150%; 法力上限+50; 免疫和红藻相关的所有伤害 (blocker E-2).

## 4. System Blockers (D-19)

Four of the 21 implemented entries imply a system that Phase 2 must not build. Each is an identity-only shell whose blocker names the missing system exactly; **no disciple, skill or regional-crafting system was implemented in Phase 2 — there is no system class, no `ModTile` and no recipe for any of them anywhere in the phase's diff.**

| Entry id | Class | Missing system | Blocker text |
| --- | --- | --- | --- |
| `item-weapons.melee-弟子剑` | `DiscipleSword` | 弟子 (disciple) progression system | `system blocked (D-19): the item belongs to the 弟子 (disciple) progression system, which is not implemented in this phase; the class is an identity only and no disciple-system code exists` |
| `item-weapons.misc-弟子时装` | `DiscipleVanity` | 弟子 (disciple) progression system | `system blocked (D-19): the item belongs to the 弟子 (disciple) progression system, which is not implemented in this phase; the vanity declares no equip slot and the class is an identity only` |
| `item-weapons.misc-技能竹简` | `SkillBambooSlip` | skill system | `system blocked (D-19): the design 描述 提交给NPC后学习 places the item in the skill system, which is not implemented in this phase; the class is an identity only` |
| `item-weapons.misc-区域放置物品制作台` | `RegionalCraftingStation` | regional-crafting system **plus** its placement tile | `system blocked (D-19): the item belongs to the regional-crafting system, which is not implemented in this phase, and the placement tile it will need does not exist either; the plain ModItem declares no ModTile, no Item.createTile and no DefaultToPlaceableTile call (design 描述 宝箱的副掉落，非核心物品)` |

`RegionalCraftingStation` is deliberately a plain `ModItem`: it declares no `ModTile` subclass, sets no `Item.createTile` and calls no `Item.DefaultToPlaceableTile`, so no half-built tile is introduced while the regional-crafting system is absent (T-02-04).

`荧光水螅召唤杖` is **not** a system entry — it needs no system. It is recorded under the absent-summon-projectile/effect artwork blockers in §2: the 荧光水螅 summon projectile does not exist in the repository, so `Item.shoot` is deliberately not set and the class is an identity only.

## 5. Deferred Localization (D-20)

Localization remains **deferred by user directive** (D-20): the in-game `OutputLocalizationHjsonItem` exporter was not run, no HJSON file was created or hand-edited, and no localization key was fabricated for any Phase 2 entry. The deferral mirrors the Phase 1 ledger convention: `localization.blocked` stays `false` because this is an explicit user-directed deferral, not a post-exporter failure with a named missing culture, so the strict `check-localization-coverage.ps1` run stays honestly red while the advisory `-AllowMissing` form is the recorded gate.

One row per Phase 2 implemented entry:

| Entry id | Design name | Class file | `status` | Directive date | Note |
| --- | --- | --- | --- | --- | --- |
| `biology_drop-weapons.ranged-肌腱巨弓` | 肌腱巨弓 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.TendonGreatbow` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `biology_drop-weapons.summon-re01` | 限制机 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.RestrictionDeviceRE01` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `biology_drop-weapons.summon-腥臭的诱饵` | 腥臭的诱饵 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.ReekingBait` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.magic-红月水藻头饰` | 红月水藻头饰 | `Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeHeaddress` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.melee-弟子剑` | 弟子剑 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.DiscipleSword` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-区域放置物品制作台` | 区域放置物品制作台 | `Everglow.Yggdrasil.KelpCurtain.Items.Placeables.RegionalCraftingStation` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-巨石弹射装置` | 巨石弹射装置 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.BoulderCatapult` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-弟子时装` | 弟子时装 | `Everglow.Yggdrasil.KelpCurtain.Items.Misc.DiscipleVanity` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-技能竹简` | 技能竹简 | `Everglow.Yggdrasil.KelpCurtain.Items.Misc.SkillBambooSlip` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-桃枝护符` | 桃枝护符 | `Everglow.Yggdrasil.KelpCurtain.Items.Accessories.PeachBranchAmulet` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-桃花纸鸢-风筝` | 桃花纸鸢（风筝） | `Everglow.Yggdrasil.KelpCurtain.Items.Misc.PeachBlossomKite` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-熊猫宠物` | 熊猫宠物 | `Everglow.Yggdrasil.KelpCurtain.Items.Pets.PandaPet` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-竹制武器` | 竹制武器 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.BambooWeapon` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-竹簪子` | 竹簪子 | `Everglow.Yggdrasil.KelpCurtain.Items.Misc.BambooHairpin` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-竹节步符` | 竹节步符 | `Everglow.Yggdrasil.KelpCurtain.Items.Accessories.BambooStepTalisman` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-红月水藻护胫` | 红月水藻护胫 | `Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeGreaves` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-红月水藻板甲` | 红月水藻板甲 | `Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeBreastPlate` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.misc-若干酒类` | 若干酒类 | `Everglow.Yggdrasil.KelpCurtain.Items.Misc.AlcoholicDrinks` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.summon-灵蛇玉卵` | 灵蛇玉卵 | `Everglow.Yggdrasil.KelpCurtain.Items.Misc.JadeSnakeEgg` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.summon-红月水藻面具` | 红月水藻面具 | `Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae.CrimsonMoonAlgaeMask` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |
| `item-weapons.summon-荧光水螅召唤杖` | 荧光水螅召唤杖 | `Everglow.Yggdrasil.KelpCurtain.Items.Weapons.FluorescentHydraStaff` | deferred | 2026-09-12 | no exporter run, no key fabricated, no HJSON edit |

The same 21 records are mirrored in `01-INVENTORY.json` `deviations[]` with `field: "localization"`, `status: "deferred"` and `reason: "localization deferred by user directive (2026-09-12)"`.

## 6. Verification Evidence

Executed 2026-09-14 from the repository root by plan 02-05 Task 3. The complete Phase 2 gate chain and the AGENTS.md byte-level checks are captured here verbatim.

### 6.1 Gate chain

| # | Command | Exit | Summary line |
| --- | --- | --- | --- |
| 1 | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | 0 | `0 Warning(s)` / `0 Error(s)`; `Everglow -> …\Mods\Everglow.tmod` produced and enabled |
| 2 | `check-phase2.ps1` (no switch) | 0 | `OK(0): phase2 implemented = 21 / 21 (full=9, shell=12)` |
| 3 | `check-phase2.ps1 -RequireAll` | 0 | `OK(0): phase2 implemented = 21 / 21 (full=9, shell=12)` |
| 4 | `validate-inventory.ps1` | 0 | `OK(0): 103 entries (84 weapons) - green=50 yellow=8 unchecked=45` |
| 5 | `check-inventory-reconciliation.ps1` | 0 | `OK(0): 103 entries; matched=86; green=50 yellow=8 unchecked=45; labels=5 deferred=3 assumptions=7` |
| 6 | `check-carryover.ps1` | 0 | `OK(0): carry-over covered entries = 5 (of 5 selected)` |
| 7 | `check-tranche-A.ps1` | 0 | `OK(0): tranche-A covered entries = 43 (of 43 selected)` |
| 8 | `check-tranche-B.ps1` | 0 | `OK(0): tranche-B covered entries = 20 (of 20 selected)` |
| 9 | `check-localization-coverage.ps1 -AllowMissing` | 0 | `MISSING(18) of 63 selected (45 covered)` then `ALLOW-MISSING: baseline recorded; exits 0 by request.` |
| 10 | `check-localization-coverage.ps1` (strict, expected red under D-20) | 1 | `MISSING(18) of 63 selected (45 covered)` then `FAIL: Phase 1 completed-art entries lack a key in en-US and/or zh-Hans.` |

The chain's success line is `verify chain OK(0)`. The strict localization run (row 10) is expected-red evidence under D-20 and is recorded, not treated as a phase failure.

### 6.2 Advisory localization missing set (D-20)

The gate's selection predicate is `phase == 1` completed-art entries, so the Phase 2 entries are outside its selection; the missing set is the unchanged Phase 1 deferred set. `check-localization-coverage.ps1 -AllowMissing` selects **63** entries and reports **45 covered / 18 missing**:

`item-weapons.melee-evil-halbert-barnacle` (EvilHalbertBarnacle), `item-weapons.misc-arc-i` (ArcI), `item-weapons.misc-arm-of-giant-tree` (ArmOfGiantTree), `item-weapons.misc-crimson-moon-algae-magic-staff` (RedAlgaeMagicStaff), `item-weapons.misc-crimson-moon-algae-spell-book` (RedAlgaeMagicSpellBook), `item-weapons.misc-crimson-moon-algae-whip` (RedAlgaeMagicWhip), `item-weapons.misc-crimson-moon-sap` (CrimsonMoonSap), `item-weapons.misc-empty-water-staff` (EmptyWaterStaff), `item-weapons.misc-jade-lake-red-algae` (JadeLakeRedAlgae_Item), `item-weapons.misc-photophore` (Photophore), `item-weapons.misc-厄佛提根的净化粉末` (ElftigernPowder), `item-weapons.misc-枯萎面具` (WitheredMask), `item-weapons.misc-森林之息` (ForestBreath), `item-weapons.misc-青须手杖` (GreenSungloStaff), `item-weapons.ranged-魁札尔的愿望` (QuetzalsWish), `item-weapons.summon-activated-dog-staff` (ActivatedDogStaff), `item-weapons.summon-crimson-moon-algae-gyroscope` (RedAlgaeMinionGyroscope), `item-weapons.summon-crimson-moon-algae-summon-staff` (RedAlgaeMinionStaff).

No exporter was run, no key was fabricated and no HJSON file was edited for any of them.

### 6.3 AGENTS.md byte-level UTF-8 BOM check and no-art/no-binary guard

The plan named the Phase 2 baseline `926543d99` (the Phase 1 close-out commit, parent of the first Phase 2 planning commit `2fc6aa346`). That commit is an ancestor of `HEAD`, but because the branch contains a merge commit (`43478f8ef`) that joined a parallel developer line, the range `926543d99..HEAD` also includes the parallel commit `a1975d1bf` ("Fix bugs and improve visual effect of ForestRainVine."), which added 10 `.png` files. The guard therefore re-anchored to the true pre-Phase-2-execution boundary `43478f8ef`, the merge commit immediately preceding every Phase 2 execution commit — the plan's own instruction is to re-anchor when the named baseline does not isolate the phase's change set (the Phase 1 01-07 baseline-`8ed6f5862` precedent), and the re-anchor cannot hide a Phase 2 asset change because every Phase 2 `.cs`/planning change is still inside the range.

| Anchor | Changed files | KelpCurtain subset | BOM-prefixed | Changed art/binary | Verdict |
| --- | --- | --- | --- | --- | --- |
| `926543d99` (plan-named; includes the parallel `a1975d1bf` art) | 107 | 69 | 0 | 10 (all `.png` from `a1975d1bf`) | BOM OK; art guard false-positive on non-Phase-2 art |
| `43478f8ef` (re-anchored: merge commit immediately before the Phase 2 execution commits) | 41 | 26 | 0 | **0** | **BOM OK; no-art guard OK** |

Against `43478f8ef` the change set is exactly the Phase 2 work: the 21 implemented `.cs` classes, `BoulderCatapult_Proj`/`BoulderCatapult_SubProj`/`TendonGreatbow_Arrow`, `KelpCurtainPlayer.cs`, `RedAlgae_FriendlyDebuff_glocalNPC.cs`, the planning artifacts (`01-INVENTORY.json`/`.md`, `02-CLASSIFICATION.json`, `02-DEVIATIONS.md`, `02-VALIDATION.md`, `scripts/check-phase2.ps1`, the four `02-0x-SUMMARY.md`, `ROADMAP.md`, `REQUIREMENTS.md`, `STATE.md`, `state.json`, `WINDOWS.md`). No `.png`, `.obj`, `.ttf`, `.atlas`, `.xnb`, `.ogg`, `.mp3`, `.bmp`, `.mapio` or `.fx` path is added or modified anywhere in it, and no file begins with the UTF-8 BOM bytes `EF BB BF`.

### 6.4 Runtime-verification bundle (D-21)

Transcribed from every `<human-check>` block in plans 02-01 to 02-04 plus the plan 02-05 bundle. All entries are **pending a live tModLoader client session** (D-21).

| Plan | Item(s) | Test | Expected |
| --- | --- | --- | --- |
| 02-01 | 红月水藻头饰 | Launch a world, open the inventory, equip 红月水藻头饰 into the head slot. | The mod loads with no missing-resource/disabled-mod entry; the item shows a white-box icon and picks up defense 8; the head slot accepts it. |
| 02-01 | 红月水藻 four-piece set | Craft all four pieces at a Work Bench, equip head + body + legs, step into water. | Both head pieces are accepted; the full set swims/runs faster than the legs piece alone; a single hit of >= 10 damage restores roughly 15% of that hit. |
| 02-02 | 巨石弹射装置 | Fire `BoulderCatapult` at a flat wall from a moderate distance. | The boulder arcs under gravity, bursts on the wall, 3-6 shards fly outward, no ammunition is consumed, tooltip damage reads 44. |
| 02-02 | 肌腱巨弓, 限制机, 腥臭的诱饵 | Equip `TendonGreatbow` with any arrow and fire at a non-boss enemy then a boss; use 限制机 and 腥臭的诱饵. | Arrows are consumed and the mod arrow fires; boss damage reads ~10% higher than an equivalent non-boss target; 限制机 consumes 15 mana but produces no drone; 腥臭的诱饵 swings, consumes one, and summons nothing. |
| 02-03 | 灵蛇玉卵, 竹节步符 | Spawn and use/equip both. | 灵蛇玉卵 is consumable with a 10-gold value and Blue rarity and consumes one without summoning anything; 竹节步符 equips into an accessory slot and grants no stats. |
| 02-03 | six shells + 灵蛇玉卵 | Spawn all six shells plus 灵蛇玉卵 and confirm they appear in the inventory. | Each item is placeable, equips or uses per its declaration, and the tML log shows no missing-resource error. |
| 02-04 | the six 02-04 shells | Spawn all six shells and view them; check the tML log for load errors. | Each shell appears with a white-box icon, the mod loads with no missing-resource error, and none of the six grants a stat or effect. |
| 02-05 | whole bundle (end-of-phase UAT) | In one client session: load the mod, confirm no missing-resource error, craft and equip the 红月水藻 set, fire 巨石弹射装置 at a wall, compare `TendonGreatbow` damage against a boss and a non-boss, then spawn the remaining shells. | The mod loads cleanly; the armor equips and its effects apply; the boulder bursts into 3-6 shards; the greatbow's boss damage is about 10% higher; every shell appears with a white-box icon and no effect; no `.png` was ever required. |

### 6.5 Final counts

Confirmed against `01-INVENTORY.json` and `01-INVENTORY.md` (both mirror records updated by this plan; the Markdown matrix is exactly **103** rows):

- **103** total entries.
- **25** allocated `phase == 2` rows — **24** remaining in the inventory plus the **1** row (`巨翼龙面具`) reallocated to Phase 7 (ITEM-06).
- **3** deferred placeholders (D-15).
- **21** implemented entries.
- **21** entries with `artwork_complete: false`, `code_complete: true` and `status: "unchecked"` (D-11/D-22). No Phase 2 row carries a Feishu status colour.

## 7. Code-Review Fixes (2026-09-14)

`02-REVIEW.md` raised two blockers and two warnings against the phase-2 sources. All four were resolved by the code-fixer after review; the Info findings (IN-01..IN-05) remain advisory and were intentionally not actioned. The fixes are behaviour-preserving except where a defect is corrected, and no `.png`, other binary/art asset, or HJSON/localization file was touched.

| Finding | Class / file | Fix commit | Summary |
| --- | --- | --- | --- |
| CR-01 | `Items/Weapons/TendonGreatbow.cs` | `05e48b1ea` | Moved the arrow substitution into `ModifyShootStats(..., ref int type, ...)` and deleted the inert `Shoot` override, so `TendonGreatbow_Arrow` (and its +10% boss clause) actually fires. `ProjectileID.WoodenArrowFriendly` remains the no-ammo fallback. |
| CR-02 | `KelpCurtainPlayer.cs` | `737cefb7f` | Moved the breastplate heal from `OnHurt` (pre-damage) to `PostHurt` and switched to `Player.Heal`, so the 15% heal is no longer swallowed by the max-life clamp at full HP. The `info.Damage >= 10` gate is preserved. |
| WR-01 | `Buffs/RedAlgae_FriendlyDebuff.cs` + detonation + 7 appliers | `8d06b7a34` | Promoted the toxin window to `RedAlgae_FriendlyDebuff.Duration = 900`; the detonation and all applicator sites now share it. Values are unchanged (all 900 frames). |
| WR-02 | `Items/Misc/JadeSnakeEgg.cs`, `Items/Weapons/ReekingBait.cs` | `7647291e2` | Added `CanUseItem => false` so the no-op consumables no longer destroy a stack before the Phase-7 summon encounter exists; `Item.consumable` stays `true` and the gate is a one-line flip when the encounter lands. |

**Superseded descriptions.** The §2.4 rows for 腥臭的诱饵 and 灵蛇玉卵 still record the pre-fix "consumes without summoning anything" behaviour; after WR-02 both items block use entirely (`CanUseItem` returns `false`) and are **not** consumed while the encounter is absent, so the corresponding §6.4 UAT expectations ("swings, consumes one") are superseded. The inventory `blockers` arrays themselves are unchanged. The §2.2 note about the "hard-coded `900`" reference is likewise superseded by WR-01 (now the shared `Duration` constant), while the E-1 duration-doubling clause remains open.

**Verification.** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 (0 warnings, 0 errors) in the main checkout — the isolated worktree could not locate `tModLoader.targets`, so the gates ran in the main checkout after the fix commits were fast-forwarded onto the branch. `check-phase2.ps1 -RequireAll` reports `OK(0): phase2 implemented = 21 / 21 (full=9, shell=12)`.

---

This ledger covers all 21 implemented Phase 2 entries; a Phase 2 entry absent from it is a defect.

*Phase 2 · plan 02-01 opened this ledger; plan 02-02 (biology-design weapon drops) appended its section; plan 02-03 (snake egg use item and six art-pending shells) appended its section; plan 02-04 (the four system-dependent shells and the last two art-pending shells) appended its section; plan 02-05 (phase close-out) consolidated the whole ledger into this six-section form.*

