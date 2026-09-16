---
phase: 04-remaining-ordinary-monsters
reviewed: 2026-09-16T11:03:36Z
depth: standard
files_reviewed: 38
files_reviewed_list:
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/KelpCurtainSpawnConditions.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/AlgaeOctopus.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/BombJellyfish.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/CannonBarnacle.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GiantTigerShrimp.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/JadeSpiritAnglerfish.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeAlgaeOctopus.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeBombJellyfish.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkHound.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/BrodieFlydragon.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/CourtCommander.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/SmallBrodieFlydragon.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/AssassinRaspberry.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/LargeMossyThornTurtle.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SmallGuppyConch.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AlgaeOctopus_InkCloud.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_Boulder.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AnimatedWitherbarkSoldier_SpellBeam.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/AssassinRaspberry_Spike.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/LargeMossyThornTurtle_Boulder.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/LargeMossyThornTurtle_Shockwave.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/Radiolarian_WaterBolt.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/RedNeedleCaterpillar_Spike.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonBubble.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/ToxicToad_PoisonCloud.cs
findings:
  critical: 2
  warning: 3
  info: 5
  total: 10
status: criticals_and_warnings_resolved
fix_iteration: 1
fixed_at: 2026-09-16
resolved_criticals: [CR-01, CR-02]
resolved_warnings: [WR-01, WR-02, WR-03]
remaining_findings: [IN-01, IN-02, IN-03, IN-04, IN-05]
---

# Phase 04: Code Review Report

**Reviewed:** 2026-09-16T11:03:36Z
**Depth:** standard
**Files Reviewed:** 38
**Status:** issues_found

## Summary

The phase is unusually well documented and the design-fidelity work is mostly sound: stat rows, money/knockback/debuff-immunity transposes, `NPC.rarity` (not `NPC.rare`), enum-wrapped `NPC.ai[]`/`localAI[]` access, `Projectile.NewProjectile(NPC.GetSource_FromAI(), …)` ownership, `!Main.dedServ` graphics guards, and the BIO-06 `SubworldSystem.IsActive<YggdrasilWorld>() && KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` double gate are correct on all 26 NPC classes and all 11 projectiles (no file in scope reads `Main.LocalPlayer` or `Main.screenPosition`). Loot wiring resolves (`MeatLantern`, `Photophore`, `RadialCarapace`, `ActivatedDogStaff`, `CaterpillarJuice` all exist; every absent design item is omitted rather than referenced), all 26 classes call `NPCSpawnManager.RegisterNPC`, and the 38 files are UTF-8 without BOM and LF-only. `LargeMossyThornTurtle` meets the requested shape: `NPC.defense` is assigned only in `SetDefaults` and the single `EnterState` helper, and `MathHelper.Clamp(damageDone * 0.2f, 2f, 20f)` is the reflect; the three D-45 shells use the engine's `aiStyle = 0` (documented as "will face the player automatically") and invent no AI.

Two defects must be fixed before this ships. One is a straight inverted probability in the toad family (the design's 75 %/25 % split is implemented as 33 %/67 %). The other is a cross-cutting netcode error: five classes keep **client-visible decision state in `NPC.localAI[]`, which tModLoader does not synchronise**, so in multiplayer the client runs a different AI branch from the server for the whole life of the creature. Three warnings follow (mini-boss flight flags, a teleport placement that embeds the caster in the floor, and a volley origin taken from unsynced state), plus five info items.

## Narrative Findings (AI reviewer)

### Critical Issues

#### CR-01: 剧毒蟾蜍's 75 % / 25 % debuff split is implemented inverted (33 % / 67 %)

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.cs:283` (and `Projectiles/Enemies/ToxicToad_PoisonBubble.cs:84`)
**Issue:** The design row reads 不论何种方式都会有 **75 %** 概率造成 10 秒中毒，剩下 **25 %** 概率造成 7 秒酸性毒液. The code rolls `Main.rand.NextBool(3)` and treats `true` as the 75 % branch, but the one-argument overload is documented as "Returns true **1 out of X** times" (`Terraria.Utils.NextBool(UnifiedRandom, int)` in the installed tModLoader XML docs), i.e. `NextBool(3)` is 1-in-3 ≈ 33 %, not "3 in 4". The comment on `ToxicToad.cs:56-58` ("The design's 75% branch is `Main.rand.NextBool(3)` (3 in 4)") and `ToxicToad_PoisonBubble.cs:23-26` record the same mistaken reading. The effect is that the toad applies the mild 中毒 only 33 % of the time and the much stronger 酸性毒液 67 % of the time — a materially harder encounter than designed, on both the contact hit and the projectile hit. The phase's other two split rolls are correct (`RedNeedleCaterpillar.cs:242` uses `NextBool(4)` for 25 % and `Main.rand.Next(8) < 3` for 37.5 %), which shows the correct pattern was already in hand.
**Fix:** Use the documented "X out of Y" overload for the 75 % branch in both files:
```csharp
if (Main.rand.NextBool(3, 4)) // 75% -> 10 s (600 ticks) of 中毒
{
	target.AddBuff(BuffID.Poisoned, PoisonTicks);
}
else // the remaining 25% -> 7 s (420 ticks) of 酸性毒液
{
	target.AddBuff(BuffID.Venom, VenomTicks);
}
```
(equivalently `Main.rand.Next(4) != 0`), and correct the two doc comments that state `NextBool(3)` is 3-in-4.

#### CR-02: Client-visible state stored in `NPC.localAI[]`, which is never synchronised — clients run a different AI branch than the server

**Files:**
- `NPCs/SpinyMossCourt/AnimatedWitherbarkHound.cs:99-103` (`ProvokedFlag` = `localAI[1]`), read by `IsNeutral` (`:121`) in `AI()` (`:200`) on every side
- `NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierRanged.cs:113-117` (`ProvokedFlag` = `localAI[1]`), read by `IsNeutral` (`:135`) in `AI()` (`:211`)
- `NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs:111-115` (`ProvokedFlag` = `localAI[1]`), read by `IsNeutral` (`:133`) in `AI()` (`:213`)
- `NPCs/DeathJadeLake/SailfinSnakehead.cs:103-107` (`AggroTimer` = `localAI[0]`), read by `HasAggro` (`:117`) in `AI()` (`:209`)
- `NPCs/DeathJadeLake/ArmoredShrimp.cs:107-111` (`Heading` = `localAI[1]`), read in `AI()` (`:272`) on every side

**Issue:** tModLoader documents `NPC.ai` as "occasionally synced from the server to clients … synced automatically", and `NPC.localAI` as "Acts like `ai`, **but does not sync to the server**" (installed `tModLoader.xml`, `F:Terraria.NPC.localAI`; the Basic Netcode wiki repeats that only `ai[]` + `SendExtraAI`/`ReceiveExtraAI` reach the clients). Every write to these wrappers is behind an authoritative guard (`HitEffect`/`Update*` return early on `NetmodeID.MultiplayerClient`) and **no `SendExtraAI`/`ReceiveExtraAI` override exists in any of the five files**, so a multiplayer client's copy of the flag keeps its `SetDefaults` value forever. The client then evaluates a *different* branch of the same AI it is expected to reproduce deterministically:

- the hound / ranged soldier / spell soldier stay `IsNeutral == true` on every client, so a creature the server has provoked into charging or casting instead runs `UpdatePatrolling()` and wanders (files above);
- `SailfinSnakehead` never sees `HasAggro` on a client, so 获得仇恨时…撞击 never plays client-side, and its `NPC.netUpdate = true` in `HitEffect` (`:271`) / `UpdateTimers` (`:324`) does not carry the window across;
- `ArmoredShrimp` drives `NPC.velocity.X` from a heading that only the server ever updates, so the client's shrimp drift in one direction while the server's shoal turns (`:272`, `:326`).

The classes' own doc comments assert the opposite ("pushes the new values to the clients through the synced `NPC.ai[]` / `NPC.localAI[]` arrays", `ArmoredShrimp.cs:252`, `:311`), so the intent was to sync; the melee sibling does it correctly by keeping its state in `NPC.ai[0]` (`AnimatedWitherbarkSoldier.cs:83-88`).
**Fix:** Move the decision state into `NPC.ai[]` (the slots freed by `ai[0]`/`ai[1]` where applicable), which is synchronised whenever the NPC is:
```csharp
/// <summary>Named wrapper over <c>NPC.ai[1]</c>: 1 once provoked, 0 while 中立 holds.</summary>
private int ProvokedFlag
{
	get => (int)NPC.ai[1];
	set => NPC.ai[1] = value;
}
```
`SailfinSnakehead.AggroTimer`/`ChargeCooldown` should likewise use `ai[1]`/`ai[2]` and `ArmoredShrimp.Heading` `ai[2]`; alternatively keep the wrapper on `localAI` and add `SendExtraAI`/`ReceiveExtraAI` (the `GlowSalamander` pattern at `GlowSalamander.cs:270-282`) plus an `NPC.netUpdate` at each write.

### Warnings

#### WR-01: `LargeMossyThornTurtle` state-3 flight flags are set on the authoritative side only

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/LargeMossyThornTurtle.cs:392-393`, `:444-446`
**Issue:** `EnterState` returns immediately on `NetmodeID.MultiplayerClient` (`:378`) but is the only place that sets `NPC.noTileCollide` / `NPC.noGravity` for the flight, and `ApplyStateMotion` deliberately does nothing for `AerialSlam` ("the client reads the synced velocity/position", `:444-446`). Neither flag is part of the NPC sync message, so a multiplayer client keeps `noTileCollide = false` and `noGravity = false` (from `SetDefaults`, `:276-277`) while the server's copy ignores tile collision and gravity for up to `FlightMaxFrames` (180) ticks: the client's mini-boss is gravity-accelerated and stopped by terrain mid-dive, then snaps on the next state transition. Every other Phase 4 class sets its `noGravity`/collision state from synced inputs on every side (e.g. `Radiolarian.cs:252`, `ToxicToad.cs:178`, `WaterStrider.cs:294`), so this is a deviation rather than a house convention.
**Fix:** Apply the flags from the synced `State` in `ApplyStateMotion`, which already runs on all sides:
```csharp
case LargeMossyThornTurtleState.AerialSlam:
	NPC.noTileCollide = true;   // 无视物块碰撞 (state 3), re-asserted every side
	NPC.noGravity = true;
	break;
```
and restore both in the other branches (or keep `EnterState` for the authoritative write and mirror it here).

#### WR-02: `AnimatedWitherbarkSoldierSpell` teleports a 46-px NPC into the floor tile it validated as solid

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldierSpell.cs:419` (`IsStandableSpot` at `:433-454`)
**Issue:** `IsStandableSpot` accepts a spot when `Main.tile[tileX, tileY + 1].HasTile` (floor) and the three tiles `tileY`, `tileY-1`, `tileY-2` are free (`ClearanceTiles = 3`, matching the 46 px / ~3-tile body at `:164`). The placement then puts the **centre** of that body on the candidate tile's centre: `NPC.Center = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f)`, so `NPC.Bottom.Y == tileY * 16 + 31` — 15 px inside the solid tile the check required. The result is a caster half-buried in the courtyard floor after every 随机传送, which is exactly the failure the clearance test was written to avoid.
**Fix:** Anchor the feet to the validated floor instead of the tile centre:
```csharp
NPC.Center = new Vector2(tileX * 16f + 8f, (tileY + 1) * 16f - NPC.height / 2f);
```

#### WR-03: `RedNeedleCaterpillar`'s volley origin comes from per-instance, unsynced segment simulation

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/RedNeedleCaterpillar.cs:196-197` (used at `:228`)
**Issue:** `UpdateVolley` fires from `NPC.Center + Segments[0].SelfPosition`. `Segments` is a plain `List<Segment>` on the `Caterpillar` template (`Templates/Enemies/Caterpillar.cs:34`) with no `SendExtraAI`/`ReceiveExtraAI` anywhere in the template, and the template's coroutines consume `Main.rand` on every side (`Caterpillar.cs:478`, `:647-649`, `:736-741`) — the case this phase's own D-55 doctrine says must be server-gated and synced. The segment offsets are therefore per-side state, so in multiplayer the server spawns 4~6 needles at a head position the client may not be showing, leaving the needles visually detached from (or emerging behind) the rendered head.
**Fix:** Spawn from a synced anchor instead of the local segment list, e.g. `Vector2 head = NPC.Center + new Vector2(NPC.direction * (SegmentHitBoxSize * (SegmentCount / 2f)), 0f);`, or sync `Segments[0].SelfPosition` (and the coroutine's random values) through `SendExtraAI`/`ReceiveExtraAI` before using it as a spawn origin.

### Info

#### IN-01: Unused `using` directives

**Files:** `using Terraria.GameContent.ItemDropRules;` is unused in `AlgaeOctopus.cs:5`, `ArmoredShrimp.cs:5`, `BombJellyfish.cs:6`, `LargeBombJellyfish.cs:6`, `SailfinSnakehead.cs:4`, `ToxicToad.cs:5`, `WaterStrider.cs:4`, `SerpentMoss.cs:5`, `SmallGuppyConch.cs:5` — those classes only name the `NPCLoot` parameter type, which lives in `Terraria.ModLoader`, not in the drop-rules namespace (the sibling `CourtCommander.cs:559` declares the same empty `ModifyNPCLoot(NPCLoot)` with no such import). `BombJellyfish.cs:5` and `LargeBombJellyfish.cs:5` additionally import `Terraria.DataStructures` with no `IEntitySource` use (neither class overrides `OnSpawn`).
**Fix:** Delete the unused directives (files that do call `ItemDropRule.Common`, e.g. `JadeSpiritAnglerfish.cs`, `Radiolarian.cs`, `AnimatedWitherbarkHound.cs`, `RedNeedleCaterpillar.cs`, must keep theirs).

#### IN-02: `WaterStrider.DashAngle` is written but never read

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.cs:98-102` (written at `:333`)
**Issue:** The `localAI[1]` wrapper is assigned the chosen dash rotation on the authoritative side and never read anywhere in the class; its doc comment claims the heading "survives the round trip", but clients actually follow the synced `NPC.velocity`. Dead state plus a comment that implies behaviour the class does not have.
**Fix:** Delete the property and the assignment at `:333` (the `NPC.netUpdate = true` on the following line is the part that actually keeps clients in step), or read it in `UpdateDashing` so the comment is true.

#### IN-03: Redundant `NPC.netUpdate` when only a projectile was spawned

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/AlgaeOctopus.cs:474` (`SpawnInkPuff`), called every `InkTrailInterval` = 10 ticks during a dash (`:426`) and once from `HitEffect` (`:280`)
**Issue:** Nothing about the NPC changes when an ink puff is created — projectile spawns are networked by the projectile system — so this forces a full NPC sync (position/velocity/`ai[]`) once every 10 ticks for the duration of each dash. The neighbouring state helpers already set `netUpdate` only when they write NPC state.
**Fix:** Remove the `NPC.netUpdate = true;` from `SpawnInkPuff` (keep it in `EnterDash` / `EnterState` / the dash-end branch). The same one-shot calls in `BombJellyfish.cs:221`, `LargeBombJellyfish.cs:227` and `ToxicToad.cs:309` are harmless but equally unnecessary.

#### IN-04: The three D-45 identity shells can spawn on dry land

**Files:** `CannonBarnacle.cs:89-97`, `FluorescentHydra.cs:87-95`, `GiantTigerShrimp.cs:88-96`
**Issue:** Each shell gates on the subworld + layer and then returns `KelpCurtainSpawnConditions.WaterWeight` without any water test, so 炮弹藤壶 (design description: 亡碧湖…水底固着的有壳生物) can appear on dry ground inside the layer. The class comments record the choice as deliberate ("invents no positional condition"), so this is a note rather than a defect — but the returned weight is the *water* band, which implies water was intended.
**Fix:** If the review of D-45 wants the shells to stay loadable without inventing behaviour, add the cheap `if (!spawnInfo.Water) { return 0f; }` used by every other water creature in the phase; otherwise leave as is and keep the deviation recorded.

#### IN-05: `GlowSalamander` suffocation cadence rides a global tick counter

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.cs:492`
**Issue:** 潮湿值清零后每30帧获得1秒窒息 is implemented as `Main.GameUpdateCount % SuffocationInterval != 0`, so every suffocating salamander pulses on the same global ticks rather than on its own 30-frame cadence after the budget empties, and the first pulse can be delayed by up to 29 ticks. The class otherwise keeps per-NPC timers in `localAI` (Pitfall 6 doctrine).
**Fix:** Add a per-NPC pulse timer next to `SuffocationInterval`, e.g. reuse `RetargetTimer`-style `localAI` slot: increment it instead of reading `Main.GameUpdateCount` and fire when it reaches `SuffocationInterval`, resetting to 0.

---

## Fix Resolution

**Resolved:** 2026-09-16, iteration 1. All two Critical and all three Warning findings were fixed against the committed Phase 4 source tree and committed atomically (`fix(04): ...`), one commit per finding. The five Info findings were intentionally left unresolved — the requested scope was Critical-and-Warning, which is `/gsd-code-review --fix`'s default `fix_scope: critical_warning`. Full report: `04-REVIEW-FIX.md`.

| Finding | Status | Commit | Change | Design / doctrine row |
| --- | --- | --- | --- | --- |
| CR-01 | resolved | `b7d134fe0` | `ToxicToad.OnHitPlayer` and `ToxicToad_PoisonBubble.OnHitPlayer` now roll the 75 % branch with `Main.rand.NextBool(3, 4)` (the documented "X out of Y" overload) instead of `Main.rand.NextBool(3)` (1 in 3 ≈ 33 %), and both doc comments no longer claim the one-argument overload is "3 in 4". | 不论何种方式都会有75%概率造成10秒中毒，剩下25%概率造成7秒酸性毒液; 酸性毒液 → `BuffID.Venom` (`04-DEVIATIONS.md` section 10) |
| CR-02 | resolved | `dd2572f48` | The five client-visible decision flags moved out of the never-synchronised `NPC.localAI[]` into synced `NPC.ai[]` slots behind their existing named wrappers: `AnimatedWitherbarkHound` / `AnimatedWitherbarkSoldierRanged` / `AnimatedWitherbarkSoldierSpell` `ProvokedFlag` → `ai[1]`; `SailfinSnakehead` `AggroTimer` → `ai[1]` and `ChargeCooldown` → `ai[2]`; `ArmoredShrimp` `Heading` → `ai[2]`. `SailfinSnakehead.UpdateTimers` now runs the two counters on every side so the synced copies stay in step, while the `netUpdate` broadcast stays authoritative. Every existing `NetmodeID.MultiplayerClient` guard and `NPC.netUpdate` write is kept. | AGENTS.md "Synchronize gameplay state with `netUpdate` … use `SendExtraAI`/`ReceiveExtraAI`"; `AnimatedWitherbarkSoldier`'s `ai[0]` precedent |
| WR-01 | resolved | `0c79bd497` | `LargeMossyThornTurtle.ApplyStateMotion` re-derives `NPC.noTileCollide` / `NPC.noGravity` from the synced `State` on every side (`bool flying = State == AerialSlam`), so a multiplayer client no longer keeps gravity and tile collision through the state-3 dive. `EnterState` keeps its authoritative write. | 3、缩壳，飞天下坠，但是无视物块碰撞 (D-55) |
| WR-02 | resolved | `86f2ff1f9` | `AnimatedWitherbarkSoldierSpell.TeleportNear` bottom-aligns the 46-px body on the validated floor: `NPC.Center = new Vector2(tileX * 16f + 8f, (tileY + 1) * 16f - NPC.height / 2f)` instead of anchoring on the candidate tile's centre, which buried the feet 15 px into the solid tile the clearance check required. `IsStandableSpot` is unchanged. | 然后随机传送（原版法师AI） |
| WR-03 | resolved | `043ce97c5` | `RedNeedleCaterpillar.UpdateVolley` anchors the volley on the synced `NPC.Center` instead of `NPC.Center + Segments[0].SelfPosition`, and the now-unneeded `Segments.Count == 0` guard is removed; `FireVolley`'s parameter doc is updated. The `Caterpillar` template's segment simulation is per-side (it consumes `Main.rand` on every side and is never synchronised), so a segment-derived origin did not match the head the client renders. | 在头部发射4~6尖刺 (D-55) |

**Verification (3-tier).** Tier 1 (re-read each modified region) and Tier 2 (the Phase's `dotnet build /p:Configuration=Release /p:WarningLevel=0`, which compiles all five changed classes) both passed for every fix. CR-02, WR-01 and WR-03 change runtime branch/sync logic rather than only syntax, so they are flagged `fixed: requires human verification` in `04-REVIEW-FIX.md`; the compile cannot prove the multiplayer branch now matches the server.

**Build and gate evidence (run in the main checkout, on the committed tree).** `dotnet build /p:Configuration=Release /p:WarningLevel=0` → exit 0, **0 warnings / 0 errors**, `Everglow.tmod` packaged and the mod enabled. `.planning/phases/04-remaining-ordinary-monsters/scripts/check-biology.ps1 -RequireAll` → exit 0, `OK: reconciled rows = 21 / 21`, `OK: guarded classes = 37`, `OK: UTF-8 BOM check passed (3 files).` No Info finding had to be touched to make either pass.

**Left unresolved (Info, outside the requested scope).** IN-01 (unused `using Terraria.GameContent.ItemDropRules;` in nine classes, plus `Terraria.DataStructures` in the two 爆弹水母 classes), IN-02 (`WaterStrider.DashAngle` written but never read), IN-03 (redundant `NPC.netUpdate` when only a projectile was spawned), IN-04 (the three D-45 identity shells can spawn on dry land) and IN-05 (`GlowSalamander` suffocation cadence rides `Main.GameUpdateCount`). Each is listed in `04-REVIEW-FIX.md` so a later pass can pick it up.

---

_Reviewed: 2026-09-16T11:03:36Z_
_Reviewer: the agent (gsd-code-reviewer)_
_Depth: standard_
_Fixed: 2026-09-16 (iteration 1, both Criticals and all three Warnings resolved; five Info findings remain)_
