---
phase: 04-remaining-ordinary-monsters
fixed_at: 2026-09-16T11:18:17Z
review_path: .planning/phases/04-remaining-ordinary-monsters/04-REVIEW.md
iteration: 1
findings_in_scope: 5
fixed: 5
skipped: 0
status: all_fixed
---

# Phase 04: Code Review Fix Report

**Fixed at:** 2026-09-16T11:18:17Z
**Source review:** `.planning/phases/04-remaining-ordinary-monsters/04-REVIEW.md`
**Iteration:** 1

**Summary:**
- Findings in scope: 5 (2 Critical + 3 Warning)
- Fixed: 5
- Skipped: 0

## Fixed Issues

### CR-01: 剧毒蟾蜍's 75 % / 25 % debuff split was implemented inverted (33 % / 67 %)

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonBubble.cs`
**Commit:** `b7d134fe0`
**Status:** fixed
**Applied fix:** Both on-hit rolls now use `Main.rand.NextBool(3, 4)`, the documented "X out of Y" overload (true 3 of 4 = 75 %), instead of `Main.rand.NextBool(3)` (true 1 in 3 ≈ 33 %). The split is now 75 % `BuffID.Poisoned` (600 ticks) / 25 % `BuffID.Venom` (420 ticks), matching the design row and the `RedNeedleCaterpillar` pattern. The two doc comments that asserted the one-argument overload was "3 in 4" were corrected. The build proves the two-argument overload resolves.

### CR-02: Client-visible state in the never-synchronised `NPC.localAI[]`

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkHound.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs`
**Commit:** `dd2572f48`
**Status:** fixed: requires human verification
**Applied fix:** The decision state moved from `NPC.localAI[]` to synced `NPC.ai[]` slots, behind the existing named wrappers (no bare numeric index was introduced):

| Class | Wrapper | Was | Now |
| --- | --- | --- | --- |
| `AnimatedWitherbarkHound` | `ProvokedFlag` | `localAI[1]` | `ai[1]` |
| `AnimatedWitherbarkSoldierRanged` | `ProvokedFlag` | `localAI[1]` | `ai[1]` |
| `AnimatedWitherbarkSoldierSpell` | `ProvokedFlag` | `localAI[1]` | `ai[1]` |
| `SailfinSnakehead` | `AggroTimer` | `localAI[0]` | `ai[1]` |
| `SailfinSnakehead` | `ChargeCooldown` | `localAI[1]` | `ai[2]` |
| `ArmoredShrimp` | `Heading` | `localAI[1]` | `ai[2]` (`ai[1]` is the follower marker) |

`SailfinSnakehead.UpdateTimers` now decrements both counters on every side (so the synced `ai[]` copies stay in step) and keeps the `NPC.netUpdate` broadcast behind `Main.netMode != NetmodeID.MultiplayerClient`. Every authoritative write keeps its `NetmodeID.MultiplayerClient` guard and its `NPC.netUpdate = true`, so server authority is unchanged. The `AnimatedWitherbarkHound` / `AnimatedWitherbarkSoldierRanged` / `AnimatedWitherbarkSoldierSpell` `StateTimer`/`CastTimer` deliberately stay in `localAI[0]`: they are read only on the authoritative side (each client branch returns before touching them), so they are not client-visible decision state. The stale doc comments that claimed `NPC.localAI[]` is synchronised were corrected. Because the fix changes which branch a client takes at runtime, a compile cannot prove the client now matches the server; a D-21 multiplayer observation is still required.

### WR-01: `LargeMossyThornTurtle` state-3 flight flags set on the authoritative side only

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/LargeMossyThornTurtle.cs`
**Commit:** `0c79bd497`
**Status:** fixed: requires human verification
**Applied fix:** `ApplyStateMotion` (which runs on every side) now re-derives both flags from the synced `State` before its switch: `bool flying = State == LargeMossyThornTurtleState.AerialSlam; NPC.noTileCollide = flying; NPC.noGravity = flying;`. `EnterState` keeps its authoritative write and its net message, and the `ApplyStateMotion` doc comment records that the two flags are not part of the NPC sync and why the flight motion itself stays server-driven.

### WR-02: `AnimatedWitherbarkSoldierSpell` teleported a 46-px body into the floor tile it validated as solid

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs`
**Commit:** `86f2ff1f9`
**Status:** fixed
**Applied fix:** `TeleportNear` now bottom-aligns the body on the floor the clearance check validated — `NPC.Center = new Vector2(tileX * 16f + 8f, (tileY + 1) * 16f - NPC.height / 2f)` — so `NPC.Bottom.Y == (tileY + 1) * 16` (the top of the solid tile) instead of 15 px inside it. The 46-px body then occupies the three free tiles `IsStandableSpot` required. The check itself is unchanged; its doc comment now names the bottom-alignment contract.

### WR-03: `RedNeedleCaterpillar`'s volley origin came from per-instance, unsynced segment simulation

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs`
**Commit:** `043ce97c5`
**Status:** fixed: requires human verification
**Applied fix:** `UpdateVolley` now anchors the volley on the synced `NPC.Center` rather than `NPC.Center + Segments[0].SelfPosition`, and the `Segments.Count == 0` guard that existed only to protect that index read was removed. `FireVolley`'s `origin` parameter doc was updated. The `Caterpillar` template's segment simulation consumes `Main.rand` on every side and is never synchronised, so the segment-derived origin was per-side state; the synced centre is identical on both sides. The design's 在头部 phrasing is a compromise here: the worm's rendered head is a per-side offset with no synced slot, and the synced centre is the closest deterministic anchor (recorded in `04-DEVIATIONS.md` section 14.7).

## Info Findings (not in scope, left for a later pass)

The requested scope was Critical-and-Warning (`fix_scope: critical_warning`). These five Info findings were deliberately not applied; none was needed to make the build or the biology gate pass.

- **IN-01** — unused `using` directives in nine classes (`Terraria.GameContent.ItemDropRules;`) plus `Terraria.DataStructures` in `BombJellyfish` / `LargeBombJellyfish`.
- **IN-02** — `WaterStrider.DashAngle` is written but never read; its doc comment implies behaviour the class does not have.
- **IN-03** — `AlgaeOctopus.SpawnInkPuff` sets a redundant `NPC.netUpdate` every `InkTrailInterval` (the same one-shot pattern in `BombJellyfish`, `LargeBombJellyfish`, `ToxicToad` is equally unnecessary).
- **IN-04** — the three D-45 identity shells (`CannonBarnacle`, `FluorescentHydra`, `GiantTigerShrimp`) return the *water* weight without a water test, so they can spawn on dry land inside the layer. The class comments record the omission as deliberate.
- **IN-05** — `GlowSalamander`'s suffocation cadence uses `Main.GameUpdateCount % SuffocationInterval`, a global tick counter, instead of a per-NPC timer.

## Verification

- **Tier 1 (always):** every modified region was re-read and confirmed; the surrounding code is intact.
- **Tier 2 (preferred):** `dotnet build /p:Configuration=Release /p:WarningLevel=0` — exit 0, 0 warnings / 0 errors. C# has no per-file compiler check, so the full project build is the syntax/semantic gate; it compiled all five changed classes.
- **Tier 3:** not required.
- **Where the gates ran:** in the main checkout (`E:\Documents\My Games\Terraria\tModLoader\ModSources\Everglow`) on the committed tree, not in an isolated worktree. `workflow.use_worktrees` is not defined in `.planning/config.json`, and the POSIX git-worktree contract is not runnable under this Windows PowerShell 5.1 shell (the only `bash.exe` present is the WSL launcher, which cannot resolve `E:\...` paths), so the worktree path was not used; there was no concurrent writer on the working tree.

## Build and Gate Result

| Check | Result |
| --- | --- |
| `dotnet build /p:Configuration=Release /p:WarningLevel=0` | exit 0 — build succeeded, 0 warnings / 0 errors; `Everglow.tmod` packaged and the mod enabled |
| `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 -RequireAll` | exit 0 — `reconciled rows = 21 / 21`, `guarded classes = 37`, `UTF-8 BOM check passed (3 files)` |
| UTF-8 BOM / LF on every modified file | clean (byte-checked) |

---

_Fixed: 2026-09-16T11:18:17Z_
_Fixer: the agent (gsd-code-fixer)_
_Iteration: 1_
