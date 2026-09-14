# Phase 2 Deviation Ledger — Remaining Items & Unfinished-Art Materials

Generated: 2026-09-14
Machine sources of truth: `01-INVENTORY.json` (per-entry status/blockers) and `02-CLASSIFICATION.json` (mode/blocker_kind/implemented).
Opened by plan 02-01 (the 红月水藻 armor-set tracer). This ledger accumulates for the whole phase; later plans append to it.

This ledger records every deviation, assumption and unresolved blocker met while implementing the remaining non-boss item scope (D-18 full implementations and identity shell classes). It is the audit trail for D-23 (implemented code wins unless the Feishu row is yellow with an explanation) and D-11/D-22 (code-complete + art-incomplete marking). No field below was changed by editing the Feishu design source, no localization key was created or renamed, and no `.png` or other binary/art asset was created or modified.

## 1. Scope and Allocation

The Phase 1 inventory carries **25 entries with `phase == 2`**. Phase 2 allocates them as:

| Group | Count | Entries / rule |
| --- | --- | --- |
| Deferred by D-15 | 3 | `item-weapons.misc-a`, `item-weapons.misc-b`, `item-weapons.misc-c-名字要普通` (design names A/B/C, all `name_zh = 阵法修复材料`, no Feishu checkbox). These are the design team's own placeholders: they stay `deferred: true` with a non-empty `deferred_reason` and are **not** implemented in Phase 2. |
| Reallocated by D-16 | 1 | `biology_drop-weapons.misc-巨翼龙面具` (巨翼龙面具) is a Giant Winged Dragon reward reallocated from Phase 2 to **Phase 7 (ITEM-06)**. Its `phase` is set to `7` and `ITEM-06` is appended to `advances`; its Feishu fields, checkboxes, `artwork_complete`, `code_complete` and `status` are unchanged. |
| Implemented by Phase 2 | 21 | The remaining non-deferred `phase == 2` entries, classified in `02-CLASSIFICATION.json`. |

The 21 implemented entries split into **9 full implementations** (D-18: 肌腱巨弓, 限制机/`biology_drop-weapons.summon-re01`, 腥臭的诱饵, 巨石弹射装置, 红月水藻头饰, 红月水藻面具, 红月水藻板甲, 红月水藻护胫, 灵蛇玉卵) and **12 identity-only shell classes** (竹节步符, 竹制武器, 竹簪子, 桃枝护符, 桃花纸鸢（风筝）, 熊猫宠物, 若干酒类, 弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台, 荧光水螅召唤杖).

`blocker_kind` is `system` for the four items whose design implies an unimplemented system — 弟子剑, 弟子时装, 技能竹简, 区域放置物品制作台 (D-19: a disciple/skill/regional-crafting system, not built in this phase) — and `artwork` for the other 17.

## 2. Design Deviations (D-23)

| # | Entry / scope | Deviation | Rationale |
| --- | --- | --- | --- |
| DD-01 | 红月水藻 four-piece set (头饰/面具/板甲/护胫) | **Recipe assumed.** The design table `D48ndhX87o9stux04tDc3aBonZd` supplies no recipe for this set (only 其他数值/价格/稀有度/效果/描述). Each piece mirrors the accepted red-algae family recipe: **15 × `JadeLakeRedAlgae_Item` + 1 × `CrimsonMoonSap` at a Work Bench** (`TileID.WorkBenches`), the exact shape already implemented in `RedAlgaeMinionStaff.AddRecipes()`. | Dependency order (D-17): the set consumes materials that already exist; a recipe keeps the branch obtainable without inventing art or systems. Recorded here for designer confirmation. |
| DD-02 | 红月水藻 four-piece set | **Rarity mapping.** Design `浅橙` maps to `ItemRarityID.Orange`, matching the accepted red-algae family (`RedAlgaeMinionStaff`, `RedAlgaeMagicStaff`). | Phase 1 Pitfall-6 mapping; the design gives no engine rarity id. |
| DD-03 | Biology-table weapons (肌腱巨弓, 限制机, 腥臭的诱饵, etc.) | **Qualitative 击退 mapping.** The design uses a qualitative 击退 cell (e.g. 强) rather than a numeric value; Phase 2 maps it to a conservative numeric `Item.knockBack` consistent with the nearest accepted ranged/summon analog for those entries when their plan implements them. | The design gives no numeric knockback; the mapped value is recorded at implementation time in the owning plan's summary. |
| DD-04 | 红月水藻 four-piece set | **Parser `category` artefact.** The four armor rows carry parser categories `weapons.magic` / `weapons.summon` / `weapons.misc` because the design puts them in the item/weapons tables, while the content is armor (`LocalizationUtils.Categories.Armor`). The `id` and `category` fields are left unchanged because the id is compatibility-sensitive (Pitfall 5). | The design row is armor; the parser heuristic keyed off the table section. Recorded per D-23 (implemented code wins). |

The head row's shared `rowspan="4"` 描述 cell is read as the **set bonus** (the set-bonus text spans all four armor rows): 红藻毒素Buff accumulates without damage and detonates on the next hit for damage scaled by the accumulated duration; the applied 红藻毒素Buff duration is doubled (15 s -> 30 s); 引爆红藻毒素 damage +150%; 法力上限+50; 免疫和红藻相关的所有伤害.

## 3. Blockers

Every Phase 2 entry carries at least one precise blocker (the gate requires `texture|artwork|system|effect|recipe`). The table below records the blocker kinds produced or outstanding at the tracer stage.

### 3.1 Missing-texture blockers (D-13)

| Entry id | Blocker kind | Exact text | Why |
| --- | --- | --- | --- |
| `item-weapons.magic-红月水藻头饰` | artwork | approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending | No approved art; the class reuses `Commons.ModAsset.White_Mod` and registers its equip slot against that fallback. |
| `item-weapons.summon-红月水藻面具` | artwork | approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending | Same as above. |
| `item-weapons.misc-红月水藻板甲` | artwork | approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending | Same as above. |
| `item-weapons.misc-红月水藻护胫` | artwork | approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending | Same as above. |

D-14 migration path: when approved art arrives, the change is adding `<Class>.png` (and `<Class>_Head.png` / `_Body.png` / `_Legs.png`) beside the `.cs` and repointing the `Load()` registration from the shared fallback to the class's own asset path — a texture addition plus a blocker removal, not a class rework.

### 3.2 Outstanding set-clause effect blockers

| Entry id | Blocker kind | Exact text | Why |
| --- | --- | --- | --- |
| `item-weapons.magic-红月水藻头饰` / `item-weapons.summon-红月水藻面具` / `item-weapons.misc-红月水藻板甲` / `item-weapons.misc-红月水藻护胫` | effect | effect blocked (outside this plan's file scope): set toxin clauses (红藻毒素Buff duration doubling, 免疫和红藻相关的所有伤害) and the greaves 免疫红藻减速 clause need the red-algae toxin/slow system or edits to accepted Phase-1 applier projectiles | Two set clauses cannot be completed inside this plan's file scope; see below. |

**Blocker E-1 — 红藻毒素Buff duration doubling (15 s -> 30 s).** The applied toxin duration is written by the accepted Phase 1 red-algae appliers, which are outside this plan's modify set. Implementing the doubling requires coordinated edits at these exact call sites:

- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/RedAlgaeMinionGyroscope_Proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/RedAlgaeMagicWhip_Proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/CrimsonMoonAlgaeSummonStaff_minion_spore.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/CrimsonMoonAlgaeSummonStaff_minion_Explosion.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Magic/RedAlgaeMagicStaff_Proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Magic/RedAlgaeMagicSpellBook_proj.cs`
- `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RedAlgaeMagicWhip.cs`

**Blocker E-2 — 免疫红藻减速.** No player-facing red-algae **slow** effect exists in the repository, so there is nothing additional to immunize against beyond the one player red-algae effect that does exist: `RedAlgaeDebuff` (its `lifeRegen` drain). The implementable part of the immunity clause is applied — both head pieces set `player.buffImmune[ModContent.BuffType<RedAlgaeDebuff>()] = true` in `UpdateArmorSet`, which immunizes the set wearer against that debuff. The 减速 clause stays recorded until a red-algae slow effect is designed/implemented.

**Implemented set clauses.** The following are active in code:

- 引爆红藻毒素 damage +150%: `RedAlgae_FriendlyDebuff_glocalNPC.ApplyRedAlgaeDetonation` multiplies the detonation damage by `2.5f` when the resolved attacking player has `KelpCurtainPlayer.CrimsonMoonAlgaeSetBuff` (the projectile path resolves the owner from `Main.player[projectile.owner]`, the item path uses the supplied `player`).
- 法力上限+50 and the `RedAlgaeDebuff` immunity: both head pieces' `UpdateArmorSet`.
- Per-piece effects: magic +18% / mana cost -12% (headdress); summon +18% / +3 max minions (mask); 15% heal of any hit >= 10 damage (`KelpCurtainPlayer.OnHurt`); +12% move speed and a further +24% while wet (greaves + `KelpCurtainPlayer.UpdateEquips`).

The hard-coded `900` accumulation reference, the `damage > 10` gate and the `DelBuff` call in `RedAlgae_FriendlyDebuff_glocalNPC` are unchanged.

## 4. Localization

Localization remains **deferred by user directive** (D-20): the in-game exporter was not run, no HJSON file was created or hand-edited, and localization is out of scope for every phase.

For the four 红月水藻 entries the inventory `localization` block is left exactly as Phase 1 recorded it:

| Entry id | `localization.en_us` | `localization.zh_hans` | `localization.blocked` |
| --- | --- | --- | --- |
| `item-weapons.magic-红月水藻头饰` | false | false | false |
| `item-weapons.summon-红月水藻面具` | false | false | false |
| `item-weapons.misc-红月水藻板甲` | false | false | false |
| `item-weapons.misc-红月水藻护胫` | false | false | false |

`blocked` stays `false` because this is an explicit user-directed deferral, not a post-exporter failure with a named missing culture — the Phase 1 deferred-localization ledger convention. The deferral makes the strict localization coverage gate stay honestly red while the advisory `-AllowMissing` baseline remains the recorded evidence.

## 5. Plan 02-02 — Biology-Design Weapon Drops

### 5.1 Design values implemented

| Entry id | Class | 伤害 | 击退 | 暴击 | 使用时间 | 价格 | 稀有度 | Implemented effect |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `item-weapons.misc-巨石弹射装置` | `BoulderCatapult` (+ `BoulderCatapult_Proj` / `BoulderCatapult_SubProj`) | 44 | 15 | — | 77 | 2金 -> `Item.buyPrice(gold: 2)` | 橙色 -> `ItemRarityID.Orange` | Fires its own boulder (no `Item.useAmmo`, 不消耗子弹); the direct hit totals 150% (base hit plus a 50% owner-guarded second hit); on death it bursts into 3-6 `BoulderCatapult_SubProj` at 15% damage |
| `biology_drop-weapons.ranged-肌腱巨弓` | `TendonGreatbow` (+ `TendonGreatbow_Arrow`) | 58 | 强 | 12% | 28 | 4g -> `Item.buyPrice(gold: 4)` | 粉 -> `ItemRarityID.Pink` | Consumes arrows and fires `TendonGreatbow_Arrow`; +10% final damage against boss targets |
| `biology_drop-weapons.summon-re01` | `RestrictionDeviceRE01` | 18 | 弱 | / | 21 | 4g -> `Item.buyPrice(gold: 4)` | 粉 -> `ItemRarityID.Pink` | Identity and stats only: no `Item.shoot` and no recipe (blockers in 5.2 / 5.3) |
| `biology_drop-weapons.summon-腥臭的诱饵` | `ReekingBait` | — | — | — | — | 20S -> `Item.buyPrice(silver: 20)` | 蓝 -> `ItemRarityID.Blue` | Consumable summon identity only: no NPC spawn and no recipe (blockers in 5.2 / 5.3) |

### 5.2 Recipe blockers

No `AddRecipes` body is written for `肌腱巨弓`, `限制机` or `腥臭的诱饵`. Each design 合成方式 cell names only Phase 7 Giant Winged Dragon items that are absent from the repository, and an item-type reference to an absent type does not compile (T-02-01).

| Entry id | Design 合成方式 | Absent Phase 7 types | Reason no recipe |
| --- | --- | --- | --- |
| `biology_drop-weapons.ranged-肌腱巨弓` | 血云母 + 血肉聚合物 + 玉化龙骨 | 血云母, 血肉聚合物, 玉化龙骨 | Giant Winged Dragon reward items, Phase 7 |
| `biology_drop-weapons.summon-re01` | 血云母 + 血肉聚合物 + 熔炉钢 + 隐生之眼 | 血云母, 血肉聚合物, 熔炉钢, 隐生之眼 | Giant Winged Dragon reward items, Phase 7 |
| `biology_drop-weapons.summon-腥臭的诱饵` | 血云母 + 干枯心脏 | 血云母, 干枯心脏 | Giant Winged Dragon reward items, Phase 7 |

### 5.3 Effect blockers

| Entry id | Unimplemented effect | Why |
| --- | --- | --- |
| `biology_drop-weapons.ranged-肌腱巨弓` | 按住左键蓄力拉出大弓 charge clause | The design documents no charge time, damage curve or release behaviour, so there is nothing precise to implement; the documented 对Boss单位额外造成10%伤害 clause IS implemented |
| `biology_drop-weapons.summon-re01` | 限制无人机 summon, 聚能射线, multi-target damage splitting, 浊燃 accumulation | The summon projectile and beam types do not exist in the repository; `Item.shoot` is deliberately not set |
| `biology_drop-weapons.summon-腥臭的诱饵` | 召唤巨翼龙 | The 巨翼龙 encounter is Phase 7; the item references no NPC type and spawns nothing |

No dust or sound call runs on a dedicated server: every client-only call in `BoulderCatapult_Proj.OnKill` and in `BoulderCatapult_SubProj` is wrapped in `if (!Main.dedServ)` (T-02-04). No projectile overrides `PreDraw` or assigns `Main.projFrames` (T-02-03).

### 5.4 Qualitative 击退 mapping (DD-05)

The biology design's 击退 cell is qualitative (强 / 弱); no documented numeric mapping exists. The planner-chosen values keep the branch on the accepted family scale (`GreenThornBallLauncher` 5.5, `QuetzalsWish` 6, `RedAlgaeMinionStaff` 2):

| Entry id | Design 击退 | `Item.knockBack` |
| --- | --- | --- |
| `biology_drop-weapons.ranged-肌腱巨弓` | 强 | 8f |
| `biology_drop-weapons.summon-re01` | 弱 | 2f |

Pending designer confirmation.

### 5.5 Design value corrections (DD-06)

An earlier draft of `02-RESEARCH.md` §Design Data reported `肌腱巨弓` 伤害 28 by misreading the `28（慢）` use-time cell as its damage. The revised RESEARCH (line 383) and the committed evidence table header and row both give 伤害 58. The implementation follows the committed `evidence/biology.xml` row: 伤害 58, 击退 强, 暴击 12%, 使用时间 28, 价格 4g, 稀有度 粉. Recorded per D-23 and T-02-08.

### 5.6 Localization (D-20)

The four entries' display keys remain deferred under D-20: the in-game exporter was not run and no HJSON file was created or hand-edited. Each row's `localization` block keeps `en_us: false` / `zh_hans: false` / `blocked: false` (the user-directed-deferral convention).

## 6. Plan 02-03 — Snake Egg Use Item and the Six Art-Pending Shells (Group A)

### 6.1 Design values implemented

| Entry id | Class | 价格 | 稀有度 | Implemented effect |
| --- | --- | --- | --- | --- |
| `item-weapons.summon-灵蛇玉卵` | `JadeSnakeEgg` | 10金 -> `Item.buyPrice(gold: 10)` | 蓝色 -> `ItemRarityID.Blue` | Consumable use item (`SummonItems` category, 12-frame swing, `UseSound = SoundID.Roar`); consumes one without summoning anything — the 苍翠灵蛇 encounter is Phase 7 scope and the 在森雨幽谷顶部使用 location condition is a gate that encounter phase owns (blocker 6.3) |

All six shells created by this plan (竹节步符, 竹制武器, 竹簪子, 桃枝护符, 桃花纸鸢（风筝）, 熊猫宠物) share the identity-only shape (D-18): `Item.width = Item.height = 20`, `Item.value = Item.buyPrice(silver: 50)`, `Item.rare = ItemRarityID.Blue`, and the D-13 `Texture => Commons.ModAsset.White_Mod` override with the standard two-line comment. 竹制武器 adds `Item.maxStack = 1`; 桃花纸鸢（风筝） adds `Item.maxStack = Item.CommonMaxStack`; 竹节步符 and 桃枝护符 add `Item.accessory = true`; 竹簪子 adds `Item.vanity = true`. The extents, value and rarity mirror the accepted art-missing precedent `Items/Misc/ForestBreath.cs` because the design rows supply none; no shell declares `AddRecipes`, `Item.shoot`, `UpdateAccessory` or an equip slot.

### 6.2 Identity-only shells (D-18)

The 12 shell entries and their `blocker_kind` in `02-CLASSIFICATION.json`:

| Entry id | Class | `blocker_kind` | Why identity-only | Owning plan |
| --- | --- | --- | --- | --- |
| `item-weapons.misc-竹节步符` | `BambooStepTalisman` | artwork | empty design row (no 伤害 / 价格 / 稀有度 / 效果); approved art absent | 02-03 |
| `item-weapons.misc-竹制武器` | `BambooWeapon` | artwork | same | 02-03 |
| `item-weapons.misc-竹簪子` | `BambooHairpin` | artwork | same (design 描述 reads 时装) | 02-03 |
| `item-weapons.misc-桃枝护符` | `PeachBranchAmulet` | artwork | same (design 描述 reads 饰品) | 02-03 |
| `item-weapons.misc-桃花纸鸢-风筝` | `PeachBlossomKite` | artwork | same (design 描述 reads 宝箱的副掉落，非核心物品) | 02-03 |
| `item-weapons.misc-熊猫宠物` | `PandaPet` | artwork | same | 02-03 |
| `item-weapons.misc-若干酒类` | `AlcoholicDrinks` | artwork | same | 02-04 |
| `item-weapons.summon-荧光水螅召唤杖` | `FluorescentHydraStaff` | artwork | same | 02-04 |
| `item-weapons.melee-弟子剑` | `DiscipleSword` | system | the design implies the unimplemented 弟子 system (D-19) | 02-04 |
| `item-weapons.misc-技能竹简` | `SkillBambooSlip` | system | the design implies the unimplemented skill system (D-19) | 02-04 |
| `item-weapons.misc-区域放置物品制作台` | `RegionalCraftingStation` | system | the design implies the unimplemented regional-crafting system (D-19) | 02-04 |
| `item-weapons.misc-弟子时装` | `DiscipleVanity` | system | the design implies the unimplemented 弟子 system (D-19) | 02-04 |

D-18 rule: a design row with no 伤害 / 价格 / 稀有度 / 效果 yields an identity-only class rather than invented numbers, because a shell is an identity, not a guessed stat block. A shell declares an item identity only — no `AddRecipes`, no `Item.shoot`, no `UpdateAccessory`, no equip slot. The eight art-pending shells carry `blocker_kind` `artwork`; the four system-dependent shells carry `system` (D-19).

### 6.3 Blockers

| Entry id | Blocker kind | Exact text | Why |
| --- | --- | --- | --- |
| `item-weapons.summon-灵蛇玉卵` | effect | `effect blocked (absent encounter + location gate): the 苍翠灵蛇 encounter is Phase 7 scope, and the documented use condition 在森雨幽谷顶部使用 (use at the top of the Valley of Lush and Moist) requires a location gate that the encounter phase owns; the item consumes without summoning anything and references no NPC type` | The encounter and its location are Phase 7; the item references no NPC type (T-02-04). |
| the six shell rows of this plan | artwork | `artwork blocked + identity-only shell (D-18): the design row carries empty 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the class declares an item identity only` | Empty design row plus absent approved artwork (T-02-01/T-02-02). |

Every entry additionally carries the D-13 texture blocker: `approved texture (贴图) missing from repository despite Feishu artwork checkbox; class implemented, art pending`.

### 6.4 Deviations

| # | Entry / scope | Deviation | Rationale |
| --- | --- | --- | --- |
| DD-07 | 竹簪子 (this plan) and 弟子时装 (02-04) | **Vanity placement.** Both vanity shells live in `Items/Misc/` rather than a new `Items/Vanity/` folder: no `Items/Vanity/` directory exists anywhere in the repository and the only tracked vanity item, `Items/Misc/WitheredMask.cs`, lives beside the misc items. The pre-existing `02-CLASSIFICATION.json` path `Items/Vanity/BambooHairpin.cs` was corrected to `Items/Misc/BambooHairpin.cs`; `弟子时装` is 02-04's row and is not touched here. | The vanity folder was a planner assumption, not repository structure; the tracked `WitheredMask` precedent is authoritative. |
| DD-08 | 灵蛇玉卵 | **Use-item category artefact.** The inventory category is `weapons.summon` from the parser heuristic while the design row is a use-item egg; the content is implemented as a consumable use item and the artefact is recorded (D-23), leaving the entry id unchanged because it is compatibility-sensitive (Pitfall 5). | The parser heuristic keys off the design table section, not the content type. |
| DD-09 | 竹簪子, 桃枝护符 | **Category from the 描述 cell.** 竹簪子's design 描述 reads 时装 (vanity) and 桃枝护符's reads 饰品 (accessory), which select the `Vanity` and `Accessories` categories; both rows are otherwise empty. | The 描述 cell is the only category evidence in the row. |
| DD-10 | 桃花纸鸢（风筝） | **Non-core chest side drop.** The design 描述 reads 宝箱的副掉落，非核心物品, so no behaviour is defined beyond the item identity. | Recorded so the shell is not mistaken for an unfinished core item. |

### 6.5 Localization (D-20)

The seven entries created by this plan keep their display keys deferred under D-20: the in-game exporter was not run and no HJSON file was created or hand-edited. Each row's `localization` block keeps `en_us: false` / `zh_hans: false` / `blocked: false` (the user-directed-deferral convention).

## 7. Plan 02-04 — The Four System-Dependent Shells and the Remaining Two Art-Pending Shells

### 7.1 System blockers (D-19)

Four of the six entries in this plan imply a system that Phase 2 must not build. Each is an identity-only shell whose blocker names the missing system exactly; **none of those systems (disciple, skill, regional-crafting) is implemented anywhere in this phase's diff — no system class, no `ModTile`, no recipe.**

| Entry id | Class | Missing system | Blocker text |
| --- | --- | --- | --- |
| `item-weapons.melee-弟子剑` | `DiscipleSword` | 弟子 (disciple) progression system | `system blocked (D-19): the item belongs to the 弟子 (disciple) progression system, which is not implemented in this phase; the class is an identity only and no disciple-system code exists` |
| `item-weapons.misc-弟子时装` | `DiscipleVanity` | 弟子 (disciple) progression system | `system blocked (D-19): the item belongs to the 弟子 (disciple) progression system, which is not implemented in this phase; the vanity declares no equip slot and the class is an identity only` |
| `item-weapons.misc-技能竹简` | `SkillBambooSlip` | skill system | `system blocked (D-19): the design 描述 提交给NPC后学习 places the item in the skill system, which is not implemented in this phase; the class is an identity only` |
| `item-weapons.misc-区域放置物品制作台` | `RegionalCraftingStation` | regional-crafting system **plus** its placement tile | `system blocked (D-19): the item belongs to the regional-crafting system, which is not implemented in this phase, and the placement tile it will need does not exist either; the plain ModItem declares no ModTile, no Item.createTile and no DefaultToPlaceableTile call (design 描述 宝箱的副掉落，非核心物品)` |

`RegionalCraftingStation` is deliberately a plain `ModItem`: it declares no `ModTile` subclass, sets no `Item.createTile` and calls no `Item.DefaultToPlaceableTile`, so no half-built tile is introduced while the regional-crafting system is absent (T-02-04).

`荧光水螅召唤杖` is **not** a system entry — it needs no system. It is recorded under the absent-summon-projectile/effect blockers instead (§7.3): the 荧光水螅 summon projectile does not exist in the repository, so `Item.shoot` is deliberately not set and the class is an identity only.

### 7.2 Shell placement

| # | Entry | Placement | Why |
| --- | --- | --- | --- |
| DD-11 | 技能竹简 | `Items/Misc/SkillBambooSlip.cs` | 02-PATTERNS.md proposed `Items/Weapons/`, but the design 描述 提交给NPC后学习 (learned after submission to an NPC) and the empty combat columns make it a quest submission item; `Items/Misc/ForestBreath.cs` is the closer analog. The pre-existing `02-CLASSIFICATION.json` path `Items/Weapons/SkillBambooSlip.cs` was corrected. |
| DD-12 | 弟子时装 | `Items/Misc/DiscipleVanity.cs` | Same vanity-placement rule as DD-07: no `Items/Vanity/` directory exists anywhere in the repository and `Items/Misc/WitheredMask.cs` is the tracked vanity precedent. The pre-existing `02-CLASSIFICATION.json` path `Items/Vanity/DiscipleVanity.cs` was corrected. |
| DD-13 | 区域放置物品制作台 | `Items/Placeables/RegionalCraftingStation.cs` | Chosen to keep its intended content type (`Placeables`) visible even though it stays a plain `ModItem` until the regional-crafting system and its tile exist. |

### 7.3 Effect/artwork blockers

| Entry id | Blocker kind | Exact text | Why |
| --- | --- | --- | --- |
| `item-weapons.misc-若干酒类` | artwork | `artwork blocked + identity-only shell (D-18): the design row carries no 伤害 / 价格 / 稀有度 / 效果 cells (its only populated cell is 描述 只喝了一两口就丢进去了), so the approved artwork and behaviour are undefined; the class declares an item identity only` | Empty design row (no consumable effect defined) plus absent approved artwork. |
| `item-weapons.summon-荧光水螅召唤杖` | artwork | `artwork blocked + identity-only shell (D-18): the design row carries no 伤害 / 价格 / 稀有度 / 效果 cells, so the approved artwork and behaviour are undefined; the 荧光水螅 summon projectile does not exist in the repository, so Item.shoot is not set and the class declares an item identity only` | Empty design row plus the absent summon projectile; no system is implicated. |

### 7.4 Localization (D-20)

The six entries of this plan extend the D-20 deferral: the in-game exporter was not run and no HJSON file was created or hand-edited. Each row's `localization` block keeps `en_us: false` / `zh_hans: false` / `blocked: false` (the user-directed-deferral convention).

---

*Phase 2 · plan 02-01 opened this ledger; plan 02-02 (biology-design weapon drops) appended §5; plan 02-03 (snake egg use item and six art-pending shells) appended §6; plan 02-04 (the four system-dependent shells and the last two art-pending shells) appended §7.*
