---
status: not-executed
phase: 03-completed-art-ordinary-monsters
source: [03-DEVIATIONS.md section 8, 03-BIOLOGY.json]
started: null
updated: 2026-09-15
---

# Phase 3 UAT — Completed-Art Ordinary Monsters (D-21 client bundle)

**This bundle is recorded, not run.** No live tModLoader client session is part of Phase 3 (D-21), so every entry below is **not yet executed** and the tranche is not presented as client-verified. The offline gates and the Release build prove that the five classes compile, resolve their tracked textures, stay inside the subworld spawn boundary and reference only implemented item types; they cannot observe spawn isolation, AI feel, drop acquisition, dedicated-server behaviour or localization fallback. Those are exactly the checks this bundle carries.

`result: not-executed` means the check has not been run — the same state `02-UAT.md` recorded as `blocked` on its runtime environment. `blocked_by: runtime-environment` is the reason in every entry.

## Current Test

[not started — no client session scheduled; all 8 checks outstanding (D-21)]

## Tests

### 1. Spawn — each tranche creature appears in its designed Kelp Curtain context

expected: In the Yggdrasil Subworld's Kelp Curtain layer, every one of the five tranche creatures can be encountered, each in its own designed context:

- 荆棘苔龟 (`MossyThornTurtle`) — on land in the 森雨幽谷 (Valley of Lush and Moist) context, not in water.
- 格普螺 (`GuppyConch`) — on land in the 森雨幽谷 context, crawling on the ground.
- 叶飞棍 (`VerdantRods`) — in open air (its row spans 亡碧湖 / 森雨幽谷), not restricted to the ground.
- 巨树人 (`GiantDandelion`) — on land in the 刺苔庭园 (Spiny Moss Court) area, at the deliberately rare weight; see check 3 for the open spawn-area question.
- 水蛞蝓 (`RiverSlug`) — on the 亡碧湖 (Death Jade Lake) surface, as before this phase.

result: not-executed
blocked_by: runtime-environment
reason: "No live tModLoader client session is part of Phase 3 (D-21); spawn placement is a runtime property no offline gate observes."

### 2. Isolation — none of the five appears in an ordinary world (BIO-06)

expected: After several minutes of idling in a normal (non-subworld) world at the same layer depth, none of the five creatures spawns. The per-creature `SpawnChance` requires `SubworldSystem.IsActive<YggdrasilWorld>()` **and** the server-safe `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`, and `NPCSpawnManager.EditSpawnPool` returns early outside the subworld, so the isolation is a two-layer guarantee that only a live run can confirm.

result: not-executed
blocked_by: runtime-environment
reason: "Main-world leakage is a runtime spawn property (BIO-06); the gate asserts the isolation tokens statically only."

### 3. Behaviour — each row behaves as its design row documents

expected: Read each creature's design section in `03-BIOLOGY.json` alongside this list:

- 荆棘苔龟 — walks, retracts and spins exactly as a vanilla tortoise does; the spin state switches the design's defence/damage (防御 10 / 伤害 50 walking, 防御 20 / 伤害 75 spinning), and a melee swing that lands during the spin is reflected (1–10) once on the attacking client.
- 格普螺 — crawls slowly, never turns to attack, deals contact damage, retracts into its shell for about two seconds after a hit (and the window does not reset on a further hit), taking noticeably less damage while retracted (0.85 normal → 0.70 shelled).
- 叶飞棍 — drifts in a wobbling hover and circles without chasing; suffers the water suffocation (a small, non-fatal life drain with an upward escape bias) and flies back out; occasionally skims a nearby liquid surface.
- 巨树人 — a full three-range state cycle: within 5 tiles a 120-tick arm raise ending in a ground smash (visible shockwave travelling away from its feet) followed by a 300-tick immobile recovery in which defence is 0 and incoming damage is amplified (150%); between 5 and 15 tiles a 120-tick backwards wind-up, a fast forward swing and an arcing boulder, with at least 240 ticks between mid-range attacks; beyond 15 tiles a visibly faster chase. **Two open questions to answer here:** (a) whether the sprite-derived `214x263` hit box is small enough for the creature to find a valid spawn area in the Kelp Curtain swamp — if it never spawns, revisit the extents, not the `0.25f` weight; (b) whether the floor-anchored shockwave keeps its ~2-tile-high hit box on uneven terrain.
- 水蛞蝓 — crawls and sinks as before this phase, is capturable, and provides its fishing power as bait.

result: not-executed
blocked_by: runtime-environment
reason: "AI feel, animation timing, the reflect and the vulnerability window are runtime gameplay properties (D-21)."

### 4. Combat — contact damage and the immunity cells behave as documented

expected: Contact damage matches each row's documented melee value (巨树人 70, with the 1.5× amplification during the post-smash window), 荆棘苔龟's damage follows its spin state (50 → 75), and every documented immunity behaves as the design states (格普螺 免疫 困惑; 叶飞棍 免疫 中毒; 水蛞蝓 as before). No creature attacks outside its documented state.

result: not-executed
blocked_by: runtime-environment
reason: "Damage exchange and status immunity are runtime combat properties (D-21)."

### 5. Drops — killing each creature produces the wired Phase 1 items at the documented chances

expected: Only the wired rules fire, at the documented chances and quantities:

- 荆棘苔龟 — 荆棘龟壳 (`ThornTurtleShell`) at 5%, quantity 1. The design names no other drop.
- 格普螺 — 格普螺外壳 (`GuppyShell`) at 11%, quantity 1. The design's 软体甲壳碎片 is absent from the repository and is deliberately not dropped (D-37/D-39).
- 叶飞棍 — no drop: its loot table is deliberately empty because both designed materials (飞棍毛发, 毒腺) do not exist in the repository.
- 巨树人 — 巨树之臂 (`ArmOfGiantTree`), 硬化枯木心脏 (`HardenedWitherbarkHeart`) and 巨石弹射装置 (`BoulderCatapult`), all guaranteed. The design's 4~6 枯木碎块 is absent and is deliberately not dropped.
- 水蛞蝓 — no drop: the design names none.

result: not-executed
blocked_by: runtime-environment
reason: "Drop acquisition is a runtime loot property; the gate proves only that every referenced item type exists on disk (D-21)."

### 6. Dedicated server and multiplayer (QUAL-03)

expected: A dedicated-server launch runs the tranche with no client-only crash and no graphics work on the server; the two 巨树人 attack projectiles emit no dust there (every dust call sits inside `!Main.dedServ`) and their authoritative spawns appear once rather than once per client (each is created inside a `Main.netMode != NetmodeID.MultiplayerClient` guard from `NPC.GetSource_FromAI()`); in a multiplayer client the tranche spawns and fights normally, the spawn band produced by the server-safe `KelpCurtainBiome.IsKelpCurtainLayer` matches the camera-driven `IsBiomeActive` band, and no state desync or duplicate NPC appears.

result: not-executed
blocked_by: runtime-environment
reason: "Dedicated-server safety and multiplayer behaviour are runtime properties; only the static guards are gated offline (D-21, QUAL-03)."

### 7. Localization fallback (D-20)

expected: The five creatures load with their display names falling back cleanly, the tML log shows no missing-resource or disabled-mod entry for any of them (their textures resolve from the tracked assets beside the classes), and no localization key has been fabricated. Localization is deferred by user directive (D-20) and is expected to read as a fallback name, not as an error.

result: not-executed
blocked_by: runtime-environment
reason: "The exporter was not run and no key was written (D-20); the fallback appearance is a runtime display property."

### 8. Consolidated end-of-phase bundle

expected: In one client session: load the mod, enter the Yggdrasil Subworld, descend into the Kelp Curtain, meet all five tranche creatures in their designed contexts, fight 巨树人 through a full state cycle, collect each wired drop, then idle in an ordinary world for several minutes and confirm no tranche creature appears there. The tML log stays free of missing-resource and disabled-mod entries throughout.

result: not-executed
blocked_by: runtime-environment
reason: "The end-of-phase client bundle is recorded here and not executed; this is the D-21 deferral the phase carries into Phase 8."

## Summary

total: 8
passed: 0
failed: 0
not_executed: 8
skipped: 0
blocked: 0

## Gaps

<!-- None. Every entry is a runtime-environment deferral (D-21), not a code issue, so nothing is added to Gaps. -->

## Deferred Follow-Ups

- test: 1
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 2
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 3
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 4
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 5
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 6
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 7
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
- test: 8
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-15
