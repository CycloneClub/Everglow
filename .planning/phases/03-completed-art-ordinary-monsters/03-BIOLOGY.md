# Phase 3 Biology Matrix - Completed-Art Ordinary Monsters

Generated mirror of `03-BIOLOGY.json` (the machine source of truth, D-24). Do not hand-edit:
regenerate from the JSON so the two artifacts cannot drift. The phase gate
`scripts/check-biology.ps1` invariant 11 re-asserts the row-id set and the per-row `status`
against the JSON on every run.

- Source snapshot: `.planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml`
  (Feishu token `Jp5ndsvNBoCpljxq1eGc9S7vnfe`, read-only, D-25).
- Tranche rule (D-41/D-42): D-41 (verbatim): "Texture-complete" - the Phase 3/4 split criterion - is the repository already containing the creature approved art (a corresponding .png in the KelpCurtain creature asset tree), not a design-row checkbox or inline design <img>. Research proved evidence/biology.xml carries no per-creature texture checkbox and that the only per-creature artwork marker is an inline <img>. D-42 (verbatim): Creatures that have repository art but no design artwork marker (e.g. 荆棘苔龟, 格普螺, 叶飞棍) are in Phase 3. The repository-art-vs-design-art discrepancy is recorded in 03-DEVIATIONS.md for later designer confirmation, not treated as out-of-scope. Enumerated with Get-ChildItem -Recurse Sources/Modules/Yggdrasil/KelpCurtain/NPCs -File -Filter *.png: MossyThornTurtle.png, GuppyConch.png, VerdantRods.png, GiantDandelion.png, RiverSlug.png and AcroporaSnake.png (the last is the boss art consumed by Phase 7). This CRITICAL_OVERRIDE supersedes 03-RESEARCH.md Finding 1 / Open Question 1, which proposed an inline design-<img> proxy.
- Naming rule: A new creature class takes its class name and internal name from the existing repository art basename (MossyThornTurtle.png -> MossyThornTurtle), keeping the tML default texture resolution working with no Texture override, no handwritten asset path and no asset move. Never place a ModNPC class in a subfolder of NPCs/ for a creature whose approved art sits directly in NPCs/. No asset is created, moved or renamed.
- Frozen counts: rows 31, phase3 5, phase4 23, phase7 3, deferred 2, texture_complete_true 6, design_art_true 9.
- Phase 3 tranche: `bio-death-jade-lake-river-slug`, `bio-death-jade-lake-verdant-rods`, `bio-spiny-moss-court-giant-tree-man`, `bio-valley-of-lush-and-moist-mossy-thorn-turtle`, `bio-valley-of-lush-and-moist-guppy-conch`.

| id | phase | region | 名称 (name_zh) | name_en | internal_name | repo_asset | texture_complete | design_art | code_complete | status | blockers | deferred |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| bio-death-jade-lake-fluorescent-hydra | 4 | Death Jade Lake | 荧光水螅 | Fluorescent Hydra | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41); behavior undefined in the design row (D-29 shell) | false |
| bio-death-jade-lake-giant-tiger-shrimp | 4 | Death Jade Lake | 巨型虎虾 | Giant Tiger Shrimp | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41); behavior undefined in the design row (D-29 shell) | false |
| bio-death-jade-lake-water-strider | 4 | Death Jade Lake | 水黾 | Water Strider | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-verdant-rods | 3 | Death Jade Lake | 叶飞棍 | Verdant Rods | `Everglow.Yggdrasil.KelpCurtain.NPCs.VerdantRods` | `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.png` | true | false | true | unchecked | drop blocked (absent items): design 33% 飞棍毛发 and 11% 毒腺 have no repository ModItem, so ModifyNPCLoot is deliberately empty (D-39); localization deferred (D-20); runtime verification outstanding (D-21) | false |
| bio-death-jade-lake-river-slug | 3 | Death Jade Lake | 水蛞蝓 | River Slug | `Everglow.Yggdrasil.KelpCurtain.NPCs.RiverSlug` | `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/RiverSlug.png` | true | true | true | unchecked | pre-existing accepted implementation (D-23): class not modified by Phase 3; subworld isolation relies on SpawnModBiomes + KelpCurtainBiome and its HitEffect dust is not Main.dedServ-guarded (gate allowlist); localization deferred (D-20); runtime verification outstanding (D-21) | false |
| bio-death-jade-lake-toxic-toad | 4 | Death Jade Lake | 剧毒蟾蜍 | Toxic Toad | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-glow-salamander | 4 | Death Jade Lake | 幽光蝾螈（美西螈） | Glow Salamander (Axolotl) | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-armored-shrimp | 4 | Death Jade Lake | 装甲虾 | Armored Shrimp | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-bomb-jellyfish | 4 | Death Jade Lake | 爆弹水母 | Bomb Jellyfish | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-sailfin-snakehead | 4 | Death Jade Lake | 帆鳍鳢 | Sailfin Snakehead | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-radiolarian | 4 | Death Jade Lake | 放射虫 | Radiolarian | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-algae-octopus | 4 | Death Jade Lake | 覆藻章鱼 | Algae-Covered Octopus | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-large-algae-octopus | 4 | Death Jade Lake | 大型覆藻章鱼 | Large Algae-Covered Octopus | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-jade-anglerfish | 4 | Death Jade Lake | 碧灵鮟鱇 | Jade Spirit Anglerfish | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-death-jade-lake-vampire-mat | 7 | Death Jade Lake | 吸血魔毯 | Vampire Mat | `Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat.VampireMat` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 7 (D-41); behavior undefined in the design row (D-29 shell) | false |
| bio-death-jade-lake-cannon-barnacle | 4 | Death Jade Lake | 炮弹藤壶 | Cannon Barnacle | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41); behavior undefined in the design row (D-29 shell) | false |
| bio-spiny-moss-court-withered-soldier | 4 | Spiny Moss Court | 枯木活化士兵 | Animated Witherbark Soldier | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-spiny-moss-court-court-commander | 4 | Spiny Moss Court | 王庭号令者 | Court Commander | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-spiny-moss-court-giant-tree-man | 3 | Spiny Moss Court | 巨树人 | Giant Tree Man | `Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion` | `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.png` | true | false | true | unchecked | drop blocked (absent item): design 4~6 枯木碎块 has no repository ModItem; the guaranteed 巨树之臂, 硬化枯木心脏 and 巨石弹射装置 rules are wired to Phase 1 items; effect parameters provisional: the design gives no shockwave radius, boulder speed or pierce values, so conservative values are used; spawn context partial (D-30): the design's 刺苔庭园 swamp has no biome or tile predicate yet; the gate uses the layer-level KelpCurtainBiome plus land conditions; localization deferred (D-20); runtime verification outstanding (D-21) | false |
| bio-spiny-moss-court-brodie-flydragon | 4 | Spiny Moss Court | 布罗迪蝇蜓 | Brodie Flydragon | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-valley-of-lush-and-moist-mossy-thorn-turtle | 3 | Valley of Lush and Moist | 荆棘苔龟（Thorn Mossy Tortoise） | Thorn Mossy Tortoise | `Everglow.Yggdrasil.KelpCurtain.NPCs.MossyThornTurtle` | `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.png` | true | false | true | unchecked | drop disposition: the design's 荆棘龟壳 is wired to the Phase 1 item ThornTurtleShell at 5% (ItemDropRule.Common(..., 20, 1, 1)); the design names no other drop; effect parameters provisional: the design supplies no spawn weight, hit box or knockback values, so the tranche's conservative defaults are used (D-34); localization deferred (D-20); runtime verification outstanding (D-21) | false |
| bio-valley-of-lush-and-moist-red-needle-caterpillar | 4 | Valley of Lush and Moist | 红针洋辣子 | Red-Needle Stinging Caterpillar | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-valley-of-lush-and-moist-assassin-raspberry | 4 | Valley of Lush and Moist | 阿萨辛覆盘子 | Assassin Raspberry | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-valley-of-lush-and-moist-serpent-moss | 4 | Valley of Lush and Moist | 蛇行苔 | Serpent Moss | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-valley-of-lush-and-moist-guppy-conch | 3 | Valley of Lush and Moist | 格普螺 | Guppy Conch | `Everglow.Yggdrasil.KelpCurtain.NPCs.GuppyConch` | `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.png` | true | false | true | unchecked | drop blocked (absent item): design 0~2 软体甲壳碎片 has no repository ModItem; the 11% 格普螺外壳 rule is wired to GuppyShell (Phase 1); localization deferred (D-20); runtime verification outstanding (D-21) | false |
| bio-valley-of-lush-and-moist-small-guppy-conch | 4 | Valley of Lush and Moist | 小格普螺 | Small Guppy Conch | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-valley-of-lush-and-moist-large-mossy-thorn-turtle | 4 | Valley of Lush and Moist | 大型荆棘苔龟 | Large Mossy Thorn Turtle | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | false |
| bio-out-of-phase-kelp-snake | 7 | out of phase | 苍带帘蛇/克莱因蛇 | Kelp Snake / Klein Snake | `` | `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/AcroporaSnake.png` | true | true | false | unchecked | boss/special encounter: out of Phase 3 scope; Phase 7 (D-41/D-42) | false |
| bio-out-of-phase-withered-seed | 4 | out of phase | 枯萎之种 | Withered Seed | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | true |
| bio-out-of-phase-withered-tree-guardian | 4 | out of phase | 枯木人卫士 | Witherbark Guardian | `` | `` | false | false | false | unchecked | artwork: no approved repository texture; Phase 4 (D-41) | true |
| bio-out-of-phase-giant-winged-dragon | 7 | out of phase | 巨翼龙 | Giant Winged Dragon | `` | `` | false | true | false | unchecked | artwork: no approved repository texture; Phase 7 (D-41) | false |

## Legend

- `phase` - 3 = the repository-art tranche implemented by this phase; 4 = remaining ordinary
  creatures (no approved repository texture); 7 = boss/special encounters and the
  already-implemented Vampire Mat, out of this phase.
- `texture_complete` - the repository already contains the creature approved art (a `.png`
  under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/`), which is the D-41/D-42 Phase 3/4 split
  criterion; it is NOT derived from a design-row checkbox or an inline design `<img>`.
- `design_art` - the design source own artwork marker: an inline `<img>` inside that creature
  design section. It is independent of `texture_complete` (D-41).
- `code_complete` - an implemented `ModNPC` class exists on disk for the row. When it is
  `true` the `internal_name` is populated and resolves to a `<short name>.cs` file; when it is
  `false` the `internal_name` is deliberately empty, so the matrix never names a class that
  does not exist.
- `status` - `unchecked` for every row: the snapshot carries no per-creature texture checkbox
  and no Feishu row receives a completion colour until both the artwork and the code are
  verified (PROJECT.md Design Status Synchronization rules 4-5).

