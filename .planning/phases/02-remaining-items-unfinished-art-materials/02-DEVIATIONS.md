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

---

*Phase 2 · plan 02-01 opened this ledger; later plans append their own rows.*
