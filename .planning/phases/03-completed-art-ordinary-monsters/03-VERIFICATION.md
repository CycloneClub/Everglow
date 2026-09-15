---
phase: 03-completed-art-ordinary-monsters
verified: 2026-09-15T09:50:43Z
status: human_needed
score: 21/25 must-haves verified
covered_files: [".planning/REQUIREMENTS.md", ".planning/phases/03-completed-art-ordinary-monsters/03-01-PLAN.md", ".planning/phases/03-completed-art-ordinary-monsters/03-01-SUMMARY.md", ".planning/phases/03-completed-art-ordinary-monsters/03-02-PLAN.md", ".planning/phases/03-completed-art-ordinary-monsters/03-02-SUMMARY.md", ".planning/phases/03-completed-art-ordinary-monsters/03-03-PLAN.md", ".planning/phases/03-completed-art-ordinary-monsters/03-03-SUMMARY.md", ".planning/phases/03-completed-art-ordinary-monsters/03-04-PLAN.md", ".planning/phases/03-completed-art-ordinary-monsters/03-04-SUMMARY.md", ".planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json", ".planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md", ".planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md", ".planning/phases/03-completed-art-ordinary-monsters/03-UAT.md", ".planning/phases/03-completed-art-ordinary-monsters/scripts/check-biology.ps1", "Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs", "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs", "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs", "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs", "Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs", "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Boulder.cs", "Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs"]
covered_digest: "v1:sha256:f845dd3013c77e9c289192246e22f381cbfb3f696dac51d0b3ea46ed502d8e08"
behavior_unverified: 4
behavior_unverified_items:
  - truth: "In a tModLoader client the mossy thorn turtle spawns in the Kelp Curtain, hides its legs while spinning, and never appears in a normal world."
    test: "Enter the Yggdrasil Subworld's Kelp Curtain in 森雨幽谷, find a 荆棘苔龟 and provoke its spin; then idle in an ordinary world at the same layer depth."
    expected: "The tortoise spawns only in the subworld, retracts/spins as a vanilla tortoise, and no 荆棘苔龟 appears in the normal world."
    why_human: "Spawn placement, the spin animation and main-world isolation are runtime properties; the offline gate asserts the isolation tokens statically only (D-21)."
  - truth: "In a tModLoader client the guppy conch hides in its shell after a hit and the leaf flying rod drifts around the player without ever chasing them."
    test: "Hit a 格普螺 and then walk into it; meet a 叶飞棍 and let it drift near the player."
    expected: "格普螺 retracts for ~2 s taking 0.70× damage and does not reset the window on a second hit; 叶飞棍 wobbles/circles without chasing and skims water."
    why_human: "AI feel, the shell window and the neutral drift are runtime gameplay properties (D-21)."
  - truth: "In a tModLoader client the giant tree man walks the Spiny Moss Court swamp, smashes a visible shockwave at close range, hurls a boulder at mid range, and is briefly vulnerable after the smash."
    test: "Fight 巨树人 through a full three-range cycle; confirm the 214x263 sprite can find a valid spawn area and that the shockwave stays ground-anchored on uneven terrain."
    expected: "5-tile 120-tick smash + visible ground shockwave; 300-tick defense-0 / 150% vulnerability; 5-15 tile boulder with >=240-tick gaps; beyond-15-tile faster chase."
    why_human: "Range switching, animation timing, the smash hit box and the spawn-area fit are runtime properties (D-21); the WR-04 terrain fix and the IN-06 hitbox risk cannot be observed offline."
  - truth: "In a tModLoader client every one of the five tranche creatures can be encountered in the Kelp Curtain and none of them appears in a normal world."
    test: "Run the consolidated end-of-phase bundle in 03-UAT.md (all 5 creatures, one full 巨树人 cycle, each wired drop, then idle in an ordinary world)."
    expected: "All five are met in their designed contexts, each wired drop is received, no tranche creature leaks into the normal world, and the tML log stays free of missing-resource/disabled-mod entries."
    why_human: "This is the D-21 client bundle; it is recorded as not-executed in 03-UAT.md and is the phase's outstanding human verification."
gaps: []
human_verification:
  - test: "Per-creature spawn and main-world isolation (03-UAT.md checks 1-2)"
    expected: "Each tranche creature appears in its designed Kelp Curtain context and none spawns in an ordinary world (BIO-06)."
    why_human: "Spawn placement and leakage are runtime properties; only static isolation tokens are gated offline."
  - test: "Behaviour — 荆棘苔龟 spin/retract + 1-10 reflect; 格普螺 shell retract; 叶飞棍 neutral hover + suffocation; 巨树人 three-range cycle (03-UAT.md check 3)"
    expected: "Each row behaves as its design row documents; the two open runtime questions (GiantDandelion spawn-area fit, shockwave on uneven ground) are settled."
    why_human: "AI feel, timing, the reflect and the vulnerability window are runtime gameplay properties (D-21)."
  - test: "Combat/damage and immunity cells (03-UAT.md check 4)"
    expected: "Contact damage per row, 巨树人 1.5× during the post-smash window, 荆棘苔龟 50→75 spin damage, and the documented immunities (格普螺 困惑, 叶飞棍 中毒)."
    why_human: "Damage exchange and status immunity are runtime combat properties."
  - test: "Drops (03-UAT.md check 5)"
    expected: "荆棘龟壳 5%, 格普螺外壳 11%, and the three guaranteed 巨树人 drops fire; absent materials never drop."
    why_human: "Drop acquisition is a runtime loot property."
  - test: "Dedicated server / multiplayer (03-UAT.md check 6, QUAL-03)"
    expected: "A dedicated-server launch runs the tranche with no client-only crash and no duplicated authoritative projectile spawns; no desync in a multiplayer client."
    why_human: "Server/multiplayer behaviour is runtime; only the static guards are gated offline."
  - test: "Localization fallback (03-UAT.md check 7, D-20)"
    expected: "The five creatures load with clean fallback display names and no missing-resource/disabled-mod log entry; no HJSON key was fabricated."
    why_human: "Fallback appearance is a runtime display property; the exporter was deliberately not run."
---

# Phase 3: Completed-Art Ordinary Monsters Verification Report

**Phase Goal:** Ordinary creatures with complete design textures are playable across the designed Kelp Curtain contexts, while their drops are supplied by the completed item work rather than deferred into creature implementation.
**Verified:** 2026-09-15T09:50:43Z
**Status:** human_needed
**Re-verification:** No — initial verification

## Goal Achievement

The completed-art (repository-art) tranche is the five ordinary creatures whose approved `.png` already exists under `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/` (D-41/D-42): 水蛞蝓 (`RiverSlug`, pre-existing), 荆棘苔龟 (`MossyThornTurtle`), 格普螺 (`GuppyConch`), 叶飞棍 (`VerdantRods`) and 巨树人 (`GiantDandelion`). All five exist as real, wired, non-stub `ModNPC` classes beside their tracked art; all four new classes compile into the packaged `Everglow.tmod`; the biology matrix is the machine source of truth with a green gate; and every wired drop resolves to an item implemented in Phases 1–2. Nothing in the phase introduces item scope. The only unverifiable part is the runtime behaviour the design-owner's rule D-21 explicitly defers to a live client.

### Observable Truths

| # | Truth | Status | Evidence |
| --- | --- | --- | --- |
| 1 | The tranche is the five repository-art creatures; the six design-art-only creatures are `texture_complete:false` / Phase 4 rows (D-41/D-42). | ✓ VERIFIED | `03-BIOLOGY.json` counts: phase3=5, texture_complete_true=6 (5 tranche + `bio-out-of-phase-kelp-snake`), design_art_true=9. The six design-art-only rows (水黾, 幽光蝾螈, 装甲虾, 帆鳍鳢, 覆藻章鱼, 大型覆藻章鱼) are all phase 4, `texture_complete:false`, `design_art:true`. `AcroporaSnake.png` is the Phase 7 boss row. |
| 2 | `03-BIOLOGY.json` is the 31-row machine source of truth with the full per-row schema and the class-resolution invariant. | ✓ VERIFIED | Parsed: 31 rows; each row carries `id, phase, region, name_zh, name_en, internal_name, repo_asset, feishu{...}, design_art, texture_complete, code_complete, status, frame_count, mapping_confidence, blockers, deferred, deferred_reason`. All five phase-3 rows are `code_complete:true` with an `internal_name` whose `.cs` exists; every phase-4 row has an empty `internal_name`. `check-biology.ps1 -RequireAll` exit 0. |
| 3 | The matrix derives only from the committed `evidence/biology.xml`; the design source is not mutated (D-25). | ✓ VERIFIED | JSON `source` = `{token: Jp5ndsvNBoCpljxq1eGc9S7vnfe, file: .planning/phases/01-item-inventory-completed-art-items/evidence/biology.xml, read_only: true}`; `git status` is clean and `git log` shows the XML unchanged since commit `d3e7a92f8`. |
| 4 | 荆棘苔龟 (`MossyThornTurtle`) end-to-end: subworld-only spawn, vanilla tortoise AI, spin 防御 10→20 / 伤害 50→75, 1–10 melee reflect, 荆棘龟壳 5%. | ✓ VERIFIED | `MossyThornTurtle.cs`: `CloneDefaults(NPCID.GiantTortoise)`, `defDamage=50`/`defDefense=10`, `PostAI` Spinning→75/20 and Retracting→50/20; `SpawnChance` gates on `SubworldSystem.IsActive<YggdrasilWorld>()` + `KelpCurtainBiome.IsKelpCurtainLayer`; `OnHitByItem` clamps `damageDone/10` to [1,10] under a `Main.myPlayer` guard; `ModifyNPCLoot` → `ItemDropRule.Common(ThornTurtleShell, 20, 1, 1)`. |
| 5 | `scripts/check-biology.ps1` is 100% ASCII PS 5.1, reads JSON/MD via `[IO.File]::ReadAllText`, resolves classes with `Get-ChildItem` (never `git ls-files`), exits 0/1/2. | ✓ VERIFIED | Byte scan: 15968 bytes, 0 bytes > 0x7F, no BOM. `ReadAllText` at lines 70/234/288; `Get-ChildItem` at 150/227; no `ls-files` token; `exit 2`/`exit 1`/`exit 0`. Ran green. |
| 6 | `03-DEVIATIONS.md` records the tranche rule, the repository-vs-design-art discrepancy, the mapping assumptions, the conservative defaults, the spawn-region gap and the D-20 deferral. | ✓ VERIFIED | Sections 1–7 exactly cover those items; §10 blocker register, §11 close-out, §12 decision disposition, §13 deferred registry are also present. |
| 7 | 格普螺 (`GuppyConch`) end-to-end: subworld-only spawn, slow land crawl, no attack, shell retract (防御 10→20, incoming ×0.70), 格普螺外壳 11%. | ✓ VERIFIED | `GuppyConch.cs`: `CrawlSpeed=0.4f`, `aiStyle=-1` local crawl, `EnterShell` sets `defense=20` under `netMode != MultiplayerClient` with `netUpdate`, `ModifyIncomingHit` ×0.70 shelled / ×0.85 normal, `ModifyNPCLoot` → `ItemDropRule.Common(GuppyShell, 9, 1, 1)`. |
| 8 | 叶飞棍 (`VerdantRods`) end-to-end: subworld-only spawn, eight-frame neutral hover/circle AI, water suffocation with escape, 50% 中毒 10 s. | ✓ VERIFIED | `VerdantRods.cs`: `Main.npcFrameCount=8` (54×432), `noGravity` hover mirroring `DarkGlimmeringRods`; suffocation `SuffocationTimer`/`SuffocationInterval=30` server-authored with `netUpdate`, floor at 1 life, upward escape bias; `OnHitPlayer` → `AddBuff(BuffID.Poisoned, 600)` on `NextBool(2)` outside any netmode guard. |
| 9 | Absent designed drops are omitted and recorded as precise blockers; the mod still builds and loads. | ✓ VERIFIED | `VerdantRods.ModifyNPCLoot` empty; `GuppyConch` only `GuppyShell`; JSON blockers name 软体甲壳碎片 / 飞棍毛发 / 毒腺; `dotnet build /p:Configuration=Release /p:WarningLevel=0` exit 0 with `Everglow.tmod` produced. |
| 10 | Neither new class contains `Main.LocalPlayer`; every dust/visual block is inside `if (!Main.dedServ)`. | ✓ VERIFIED | Grep: 0 matches for `Main.LocalPlayer` across all tranche classes/projectiles; every `Dust.NewDust` burst is inside `!Main.dedServ` (`HitEffect` 4×, projectile AI 3×). |
| 11 | `03-BIOLOGY.json`/`.md` are updated consistently for both rows (`code_complete`, `internal_name`, `frame_count`, blockers) without changing the frozen sets. | ✓ VERIFIED | Full 31-row cell-by-cell parity check (phase/region/name/internal/repo_asset/texture/design_art/code/status/blockers/deferred): 31 rows, 0 mismatches. Frozen counts unchanged. |
| 12 | 巨树人 (`GiantDandelion`) is a rare Spiny Moss Court enemy that spawns only inside the subworld, chases an aggroed player, and never leaks into the main world. | ✓ VERIFIED | `GiantDandelion.cs`: `SpawnChance` gates on `SubworldSystem.IsActive<YggdrasilWorld>()` + `KelpCurtainBiome.IsKelpCurtainLayer` + land-only, weight `0.25f`; `AggroRangeTiles=30f` gate makes Idle reachable and bounds chasing. |
| 13 | The three-range behaviour exists with the documented timings. | ✓ VERIFIED | 5-tile → `SmashWindUp` 120 → `Smash()` spawns shockwave → `SmashRecovery` 300 (`defense=0`, `ModifyIncomingHit ×1.5`); 5–15 tile → `BoulderWindUp` 120 / `BoulderThrow` 20 / `BoulderRecover` 120 with `MidRangeAttackCooldown=240` armed only from the mid-range throw; >15 tile → `ChaseSpeed=3.2f` vs `ApproachSpeed=1.5f`. |
| 14 | The three implemented drops are guaranteed rules; the absent 4~6 枯木碎块 is a precise blocker, not a type reference. | ✓ VERIFIED | `ModifyNPCLoot` → `ItemDropRule.Common(..., 1, 1, 1)` for `ArmOfGiantTree`, `HardenedWitherbarkHeart`, `BoulderCatapult` (all on disk, Phase 1/2); JSON blocker names 枯木碎块. |
| 15 | The two attack projectiles live under `Projectiles/Enemies/`, use the shared fallback texture, guard dust with `!Main.dedServ`, and reference no unimplemented item. | ✓ VERIFIED | `GiantDandelion_Shockwave.cs` (98 lines) and `GiantDandelion_Boulder.cs` (73 lines): `Texture => Commons.ModAsset.White_Mod`, `hostile=true`, dust inside `!Main.dedServ`, no `ModContent.ItemType<`. |
| 16 | No class contains `Main.LocalPlayer`; spawn/combat transitions are server-authoritative with `NPC.netUpdate` on change. | ✓ VERIFIED | No `Main.LocalPlayer` in any tranche file; `RunStateMachine` under `Main.netMode != NetmodeID.MultiplayerClient`; `EnterState` sets `NPC.netUpdate=true`; projectile spawns under the netmode guard from `NPC.GetSource_FromAI()`. |
| 17 | All five `phase:3` rows read `code_complete:true` with a resolving `internal_name`; `check-biology.ps1 -RequireAll` exits 0. | ✓ VERIFIED | Ran the gate: `OK(0): phase3 tranche = 5 / 5 (rows=31)`, `OK: implemented classes = 5 / 5`, `OK: UTF-8 BOM check passed (5 files).`, exit 0. |
| 18 | Every tranche row's blockers state its real disposition. | ✓ VERIFIED | Each of the five rows' `blockers` arrays record wired drops, absent drops, provisional effect parameters, the D-30 spawn-context gap (巨树人 only) and the D-20/D-21 marker; mirrored verbatim in `03-DEVIATIONS.md` §10. |
| 19 | `03-DEVIATIONS.md` is the single consolidated ledger with a coverage claim over all five rows and the deferred/unfinished-art/out-of-phase residue. | ✓ VERIFIED | Coverage claim names all five row ids; §11 close-out, §12 decision disposition, §13 deferred registry record the Phase-4 six, Phase-7 bosses, the White_Mod projectile art gap and localization. |
| 20 | The full offline gate chain is green in one run (Phase 3 gate, five Phase 1 gates, Phase 2 gate, Release build, Yggdrasil tests, BOM check). | ✓ VERIFIED | Individually re-run: `check-biology.ps1 -RequireAll` exit 0; `validate-inventory`, `check-inventory-reconciliation`, `check-carryover`, `check-tranche-A`, `check-tranche-B`, `check-armofgianttree-charge`, `check-phase2` all exit 0; `dotnet build /p:Configuration=Release /p:WarningLevel=0` exit 0 (0 warnings / 0 errors, `Everglow.tmod` packed); `dotnet test --no-build --filter "FullyQualifiedName~Yggdrasil"` 3 passed / 0 failed; AGENTS.md byte-level BOM check passed (1378 files). |
| 21 | `03-UAT.md` records the D-21 client bundle so the tranche is not presented as client-verified. | ✓ VERIFIED | `03-UAT.md` exists with 8 checks, every one `result: not-executed` / `blocked_by: runtime-environment`, and a header stating the bundle is recorded but not run. |
| 22 | [backstop] Client: mossy thorn turtle spawns in the Kelp Curtain, hides legs while spinning, never appears in a normal world. | ⚠️ PRESENT_BEHAVIOR_UNVERIFIED | Code is present and wired; no offline test exercises the spawn/animation/isolation transition. Routed to human verification (D-21). |
| 23 | [backstop] Client: guppy conch hides in its shell after a hit; leaf flying rod drifts around the player without chasing. | ⚠️ PRESENT_BEHAVIOR_UNVERIFIED | Code present and wired; no offline test exercises the state transition/AI feel. Routed to human verification (D-21). |
| 24 | [backstop] Client: giant tree man walks the swamp, smashes a visible shockwave, hurls a boulder, is briefly vulnerable after the smash. | ⚠️ PRESENT_BEHAVIOR_UNVERIFIED | Code present and wired (incl. the WR-04 terrain fix); no offline test exercises the range cycle or the 214×263 spawn-area fit (IN-06). Routed to human verification (D-21). |
| 25 | [backstop] Client: all five tranche creatures can be encountered in the Kelp Curtain and none appears in a normal world. | ⚠️ PRESENT_BEHAVIOR_UNVERIFIED | The D-21 end-of-phase bundle; recorded not-executed in `03-UAT.md`. Routed to human verification. |

**Score:** 21/25 truths verified (4 present, behavior-unverified — all four are the design-owner-deferred D-21 runtime backstop items).

### Advisory (Observations, no phase-goal impact)

| # | Finding | Category | Note |
| --- | --- | --- | --- |
| 1 | The `03-BIOLOGY.json` `tranche_rule` and `03-DEVIATIONS.md` §1 claim the repository art was enumerated with `Get-ChildItem -Recurse ... -Filter *.png` but list only the six top-level assets; the recursive listing also returns `NPCs/VampireMat/VampireMat.png` (+3 attack art). The phase-7 `bio-death-jade-lake-vampire-mat` row is `texture_complete:false` / `code_complete:false` yet carries a populated `internal_name`. | other | Out of phase-3 scope (VampireMat is a Phase 7 special encounter, not an ordinary creature). The gate enforces the `code_complete ⇒ internal_name exists` biconditional only for phase-3 rows, so this is a documentation/classification nuance for designer confirmation, not a gap. The `internal_name` still resolves to an on-disk class, so the matrix is not ahead of the code. |
| 2 | `03-UAT.md` check 3 describes the tortoise's spin switching "防御 10 / 伤害 50 walking, 防御 20 / 伤害 75 spinning"; the WR-01 fix additionally applies 防御 20 during the retract (缩壳) state. | other | Minor documentation drift written before the review fix; the current code matches the design row more closely. No action required for the goal. |

### Required Artifacts

| Artifact | Expected | Status | Details |
| --- | --- | --- | --- |
| `Sources/.../NPCs/MossyThornTurtle.cs` | 荆棘苔龟 full implementation | ✓ VERIFIED | 195 lines; spawn, AI, drop, reflect; beside tracked `MossyThornTurtle.png`. |
| `Sources/.../NPCs/GuppyConch.cs` | 格普螺 full implementation | ✓ VERIFIED | 280 lines; crawl/shell AI, drop; beside tracked `GuppyConch.png`. |
| `Sources/.../NPCs/VerdantRods.cs` | 叶飞棍 full implementation | ✓ VERIFIED | 296 lines; 8-frame hover, suffocation, poison, empty loot; beside tracked `VerdantRods.png`. |
| `Sources/.../NPCs/GiantDandelion.cs` | 巨树人 full implementation | ✓ VERIFIED | 688 lines; three-range state machine, vulnerability, 3 drops; beside tracked `GiantDandelion.png`. |
| `Sources/.../Projectiles/Enemies/GiantDandelion_Shockwave.cs` | Hostile ground wave | ✓ VERIFIED | 98 lines; `White_Mod`, multi-column floor anchor, dedServ-guarded dust. |
| `Sources/.../Projectiles/Enemies/GiantDandelion_Boulder.cs` | Hostile boulder | ✓ VERIFIED | 73 lines; `White_Mod`, gravity arc, dedServ-guarded dust. |
| `Sources/.../KelpCurtain/KelpCurtainBiome.cs` | Server-safe layer predicate | ✓ VERIFIED | `IsKelpCurtainLayer(Player)` uses synced player centre, never the client camera; documented band intent (WR-05). |
| `.planning/.../03-BIOLOGY.json` | 31-row machine matrix | ✓ VERIFIED | Parses; frozen counts and sets; 5 phase-3 rows code-complete. |
| `.planning/.../03-BIOLOGY.md` | Human mirror | ✓ VERIFIED | 31 rows; full cell parity with the JSON (0 mismatches). |
| `.planning/.../03-DEVIATIONS.md` | Consolidated ledger | ✓ VERIFIED | 13 sections incl. blocker register, close-out, deferred registry. |
| `.planning/.../03-UAT.md` | D-21 client bundle | ✓ VERIFIED | 8 not-executed entries. |
| `.planning/.../scripts/check-biology.ps1` | ASCII gate | ✓ VERIFIED | 100% ASCII, no BOM, runs green. |

### Key Link Verification

| From | To | Via | Status | Details |
| --- | --- | --- | --- | --- |
| Each NPC | `NPCSpawnManager.RegisterNPC` | `SetStaticDefaults` | ✓ WIRED | All four new classes register. |
| Each `SpawnChance` | `SubworldSystem.IsActive<YggdrasilWorld>()` + `KelpCurtainBiome.IsKelpCurtainLayer` | spawn gate | ✓ WIRED | Returns `0f` outside the subworld. |
| `MossyThornTurtle.ModifyNPCLoot` | `ThornTurtleShell` | `ItemDropRule.Common(...,20,1,1)` | ✓ WIRED | Item exists (added 2025-06-02). |
| `GuppyConch.ModifyNPCLoot` | `GuppyShell` | `ItemDropRule.Common(...,9,1,1)` | ✓ WIRED | Item exists (added 2025-06-02). |
| `GiantDandelion.ModifyNPCLoot` | `ArmOfGiantTree`, `HardenedWitherbarkHeart`, `BoulderCatapult` | guaranteed rules | ✓ WIRED | Items exist (Phase 1 `2d2623de0`, Phase 2 `e7692b528`, 2025 accessories). |
| `GiantDandelion` state machine | `GiantDandelion_Shockwave` / `_Boulder` | `Projectile.NewProjectile(NPC.GetSource_FromAI())` under netmode guard | ✓ WIRED | Present in `Smash()` and `ThrowBoulder()`. |
| `03-BIOLOGY.json` | `03-BIOLOGY.md` | gate invariant 11 + full cell parity | ✓ WIRED | 31 rows, 0 mismatches. |
| `03-BIOLOGY.json` | `scripts/check-biology.ps1` | coverage/classification/class-resolution/item/art/BOM invariants | ✓ WIRED | `-RequireAll` exit 0. |

### Data-Flow Trace (Level 4)

| Artifact | Data Variable | Source | Produces Real Data | Status |
| --- | --- | --- | --- | --- |
| `MossyThornTurtle` / `GuppyConch` / `GiantDandelion` `ModifyNPCLoot` | drop rules | `ModContent.ItemType<...>` of real Phase 1/2 items | Yes — every referenced type resolves to a `.cs` on disk; gate invariant asserts this | ✓ FLOWING |
| `GiantDandelion_Shockwave.AI` | `floorTileY` | `FindFloorTileY()` sampling the leading/centre/trailing world columns | Yes — real `Main.tile` reads, not a static value (WR-04) | ✓ FLOWING |
| `VerdantRods.OnHitPlayer` | poison buff | `BuffID.Poisoned` applied on the local client with `Player.AddBuff` hurt-path sync | Yes — real buff application | ✓ FLOWING |

### Behavioral Spot-Checks

| Behavior | Command | Result | Status |
| --- | --- | --- | --- |
| Phase 3 biology gate (all invariants, require-all) | `check-biology.ps1 -RequireAll` | exit 0; `5 / 5` tranche, `5 / 5` classes, BOM ok | ✓ PASS |
| Phase 1 regression gates | five `check-*.ps1` | all exit 0 | ✓ PASS |
| Phase 2 regression gate | `check-phase2.ps1` | exit 0; 21/21 implemented | ✓ PASS |
| Release build | `dotnet build /p:Configuration=Release /p:WarningLevel=0` | exit 0; 0 warnings / 0 errors; `Everglow.tmod` packed | ✓ PASS |
| Yggdrasil unit-test link | `dotnet test --no-build --filter "FullyQualifiedName~Yggdrasil"` | 3 passed / 0 failed | ✓ PASS |
| AGENTS.md byte-level BOM check | whole-branch script | passed (1378 files) | ✓ PASS |
| Runtime spawn/AI/drops/dedicated-server | — | no live client session (D-21) | ? SKIP → human verification |

### Probe Execution

No `probe-*.sh` probes exist for this phase (the phase's runnable check is the PowerShell gate `scripts/check-biology.ps1`, exercised above). **N/A.**

### Requirements Coverage

| Requirement | Source Plan | Description | Status | Evidence |
| --- | --- | --- | --- | --- |
| BIO-01 (completed-art tranche) | 03-01, 03-02, 03-04 | Death Jade Lake ordinary creatures | ✓ SATISFIED (tranche only) | 2 of 14 rows implemented (`RiverSlug` pre-existing + `VerdantRods`); requirement stays Pending (mapped to Phase 4). |
| BIO-02 (completed-art tranche) | 03-01, 03-03, 03-04 | Spiny Moss Court creatures | ✓ SATISFIED (tranche only) | 1 of 4 rows implemented (`GiantDandelion`); stays Pending (Phase 4). |
| BIO-03 (completed-art tranche) | 03-01, 03-02, 03-04 | Valley of Lush and Moist creatures | ✓ SATISFIED (tranche only) | 2 of 7 rows implemented (`MossyThornTurtle`, `GuppyConch`); stays Pending (Phase 4). |
| BIO-06 (structural half) | 03-01…03-04 | No main-world leakage | ✓ SATISFIED (structural half) | Subworld-isolated spawn gates + `EditSpawnPool` early return; runtime isolation is a human item. Stays Pending (Phase 8). |
| QUAL-03 (structural half) | 03-02, 03-03, 03-04 | Multiplayer/dedicated-server behaviour | ✓ SATISFIED (structural half) | `!Main.dedServ` guards, netmode-guarded authoritative spawns/transitions; runtime verification is a human item. Stays Pending (Phase 8). |
| QUAL-04 | 03-04 | Per-row comparison record | ✓ SATISFIED | `03-BIOLOGY.json` per-row comparison (feishu block ids, status, blockers, mapping_confidence) + `03-DEVIATIONS.md` ledger. Stays Pending (Phase 8). |

**Orphaned requirements:** None. No v1 requirement is mapped to Phase 3; the phase delivers a strict subset (tranche) of BIO-01/02/03 and the structural half of BIO-06/QUAL-03/QUAL-04.

### Anti-Patterns Found

| File | Line | Pattern | Severity | Impact |
| --- | --- | --- | --- | --- |
| `GiantDandelion.cs` | 584 | `return null;` in `GetTarget()` | ℹ️ Info | Legitimate "no valid player target" sentinel, not a stub. |
| `GiantDandelion_Shockwave.cs` / `_Boulder.cs` | 8, 15 | comment mentioning "placeholder art" | ℹ️ Info | Documents the deliberate `White_Mod` fallback in place of forbidden placeholder art; not a debt marker. |
| (all seven phase files) | — | `TBD`/`FIXME`/`XXX`/`TODO` debt markers | — | None found. |

### Human Verification Required

The phase carries four runtime backstop truths (the D-21 client bundle). See `behavior_unverified_items` and `human_verification` in the frontmatter; the full executable list is `03-UAT.md` (8 checks, all `not-executed`). In short:

1. **Spawn & isolation** — each of the five tranche creatures appears in its designed Kelp Curtain context; none appears in an ordinary world (BIO-06).
2. **Behaviour** — 荆棘苔龟 spin/retract + 1–10 reflect; 格普螺 shell retract; 叶飞棍 neutral hover + suffocation; 巨树人 three-range cycle, including the two open questions (214×263 spawn-area fit; shockwave on uneven terrain).
3. **Combat/damage & immunities** — per-row contact damage, 巨树人 1.5× vulnerability, 格普螺 困惑 / 叶飞棍 中毒 immunity.
4. **Drops** — 5% / 11% / guaranteed rules fire; absent materials never drop.
5. **Dedicated server / multiplayer (QUAL-03)** — no client-only crash, no duplicated authoritative spawns, no desync.
6. **Localization fallback (D-20)** — clean fallback names, no missing-resource/disabled-mod log entry, no fabricated HJSON key.

### Gaps Summary

No gaps. All 21 programmatically-verifiable must-have truths pass: the five repository-art creatures are real, wired, compiling implementations with correctly-gated subworld spawns and drops that resolve to Phase 1–2 items; the 31-row biology matrix is machine-consistent with its human mirror and its gate; the offline gate chain, Release build and Yggdrasil tests are all green; and the phase introduced no item, asset or localization scope. The four remaining truths are the design-owner's D-21 runtime backstop items, which are, by design, observable only in a live tModLoader client — recorded in `03-UAT.md` and routed to human verification rather than counted as gaps.

Two minor documentation observations (body "Advisory" table) touch only the out-of-phase `VampireMat` classification and a stale `03-UAT.md` sentence; neither affects the phase goal and neither is a blocking finding.

---

_Verified: 2026-09-15T09:50:43Z_
_Verifier: the agent (gsd-verifier)_
