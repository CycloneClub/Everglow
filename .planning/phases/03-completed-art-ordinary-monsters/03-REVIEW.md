---
phase: 03-completed-art-ordinary-monsters
reviewed: 2026-09-15T00:00:00Z
depth: standard
files_reviewed: 7
files_reviewed_list:
  - Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Boulder.cs
findings:
  critical: 0
  warning: 5
  info: 6
  total: 11
status: warnings_resolved
fix_iteration: 1
fixed_at: 2026-09-15
resolved_warnings: [WR-01, WR-02, WR-03, WR-04, WR-05]
remaining_findings: [IN-01, IN-02, IN-03, IN-04, IN-05, IN-06]
---

# Phase 3: Code Review Report

**Reviewed:** 2026-09-15
**Depth:** standard
**Files Reviewed:** 7
**Status:** issues_found

## Summary

The four `ModNPC` classes and two enemy projectiles were reviewed at standard depth, together with the new `KelpCurtainBiome.IsKelpCurtainLayer(Player)` helper. I traced the spawn gates, the authoritative/client split, every client-only API, the loot type references and the numeric mapping back to the phase plan and design rows.

**What checks out (verified, not assumed):**

- **Subworld isolation (BIO-06).** All four `SpawnChance` overrides (MossyThornTurtle.cs:169-177, GuppyConch.cs:249-269, VerdantRods.cs:272-285, GiantDandelion.cs:619-639) return `0f` unless `SubworldSystem.IsActive<YggdrasilWorld>()` **and** `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`. `NPCSpawnManager.EditSpawnPool` additionally returns early outside the subworld. No path leaks to the main world, and no `Main.LocalPlayer` is used anywhere in the tranche.
- **Dedicated-server safety.** All four death-dust bursts are inside `!Main.dedServ`; both projectiles guard every dust call (Shockwave.cs:55, Boulder.cs:53 & 63). `FindFrame`/`AI` contain no graphics API.
- **Server authority.** `GuppyConch.EnterShell`/shell-expiry, `VerdantRods` suffocation drain and all `GiantDandelion` transitions run under `Main.netMode != NetmodeID.MultiplayerClient` and each authoritative write sets `NPC.netUpdate`. `VerdantRods.OnHitPlayer` (VerdantRods.cs:235-241) is correctly **not** netmode-guarded (tML documents the hook as "Called on the local client only"), and `MossyThornTurtle.OnHitByItem` is correctly guarded by `player.whoAmI != Main.myPlayer` ("Called on the client doing the damage").
- **Loot wiring.** `ThornTurtleShell`, `GuppyShell`, `ArmOfGiantTree`, `HardenedWitherbarkHeart` and `BoulderCatapult` all exist on disk and their namespaces match the `using`s. `VerdantRods` correctly writes no rule instead of an absent type.
- **Compile safety / conventions.** `NPC.rarity` is a real field (`F:Terraria.NPC.rarity`); `SafeNormalize`, `SpawnModBiomes` collection expressions, `Categories.MagicProjectiles`, `NPCSpawnInfo.SpawnTileX/SpawnTileY/Water/Player` all resolve. All seven files are LF, UTF-8 without BOM, tab-indented, with a trailing newline. Sprite geometry matches the code (`MossyThornTurtle` 84x46, `GuppyConch` 114x58, `VerdantRods` 54x432 = 8 frames, `GiantDandelion` 214x263). No `.png` was added or modified and no HJSON has been hand-edited.

No correctness, security, data-loss or crash defect was found — in particular nothing here can leak a creature into the main world, and the authoritative/client split is sound. The findings below are design-fidelity and robustness defects.

## Warnings

### WR-01: Tortoise defence 20 is never applied while the shell is retracted (缩壳)

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs:96-108`
**Issue:** The class's own doc comment (line 93) states the design row as "防御 10（正常）/20（旋转/缩壳）", and the enum declares `Retracting = 1` (line 28). `PostAI` only tests `State == TortoiseState.Spinning`, so during `Retracting` the `else` branch restores `NPC.defense = NPC.defDefense` (10). The cloned `AI_039_Tortoise` re-asserts `defense = defDefense` in the retract state every tick, and `PostAI` runs after it, so the shelled tortoise takes *normal* damage for the whole retract window. The design's 缩壳 value (20) is therefore only half implemented. (The plan/UAT narrowed this to the spin state — see 03-UAT.md check 3 — so this may be a deliberate deviation; it needs a designer confirmation or the code fix.)
**Fix:**
```csharp
if (State is TortoiseState.Spinning or TortoiseState.Retracting)
{
    NPC.damage = (int)(NPC.defDamage * 1.5f); // 75 while spinning
    NPC.defense = NPC.defDefense * 2;         // 20 (旋转/缩壳)
}
else
{
    NPC.damage = NPC.defDamage; // 50
    NPC.defense = NPC.defDefense; // 10
}
```
(Adjust the damage line if 缩壳 should keep 50 rather than 75.)

### WR-02: `GiantDandelion` re-arms the mid-range cooldown from the post-smash throw

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs:402-434`
**Issue:** `ExitSmashRecovery` calls `ThrowBoulder`, and `ThrowBoulder` unconditionally writes `MidRangeAttackCooldown = MidRangeAttackGapTicks` (line 424). The design's 240-tick gap applies only to the two attacks of the **mid-range** state; the boulder flung at the end of the smash recovery is a different attack. As written, a close-range smash suppresses the mid-range boulder for up to 240 ticks afterwards, so the mid-range state can be observed approaching without ever attacking after a smash sequence.
**Fix:** Give `ThrowBoulder` a parameter and only arm the cooldown from the mid-range path, e.g.
```csharp
private void ThrowBoulder(Player target, bool armMidRangeCooldown)
{
    if (armMidRangeCooldown)
    {
        MidRangeAttackCooldown = MidRangeAttackGapTicks;
    }
    ...
}
```
called with `armMidRangeCooldown: true` from the `BoulderThrow` case and `false` from `ExitSmashRecovery`.

### WR-03: The no-aggro Idle state is unreachable while any player is alive

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs:233-242`, `305-332`, `559-569`
**Issue:** `RunStateMachine` calls `NPC.TargetClosest()` on every tick the creature is not in `SmashRecovery` (line 235). `TargetClosest` has no aggro radius — it selects the nearest live player at any distance — so `IsTargetValid()` is true whenever any player is alive, and `SelectLocomotionState` immediately leaves `Idle` for `Approach`/`Chase`/`SmashWindUp`. The design's state (0) "idle wander with no aggro" therefore never plays in normal gameplay, and the Giant Tree Man chases a player across the whole subworld. The giant hitbox plus the no-aggro design implies it is meant to laze in the swamp until approached.
**Fix:** Gate aggro on distance before leaving `Idle`, for example:
```csharp
const float AggroRangeTiles = 30f;
...
if (State == GiantDandelionState.Idle && (distance > AggroRangeTiles * 16f || !CanSeeTarget()))
{
    // stay Idle, keep wandering
}
```
or only call `TargetClosest()` once the creature is within an aggro radius.

### WR-04: The floor-anchored shockwave samples only one column for a 200 px-wide hitbox

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Enemies/GiantDandelion_Shockwave.cs:41-53`
**Issue:** `Projectile.width` is 200 but the floor probe uses `centerTileX = Projectile.Center.X / 16` only. On uneven ground the anchor is derived from whatever tile sits under the wave's centre, so: (a) when the centre is over a hole the wave sinks (`velocity.Y = 8f`) even if most of the hitbox is over solid floor, and (b) when the centre is over a higher terrace the whole 32 px-high hitbox is snapped up to that terrace while its leading half is over lower ground. This is exactly the open question recorded in `03-UAT.md` check 3(b). Because `Projectile.tileCollide = false`, nothing else corrects it.
**Fix:** Probe the leading edge (and the trailing edge) and anchor to the lowest floor found across the span, e.g.:
```csharp
int leadingX = (int)((Projectile.velocity.X >= 0f ? Projectile.Right.X : Projectile.Left.X) / 16f);
int centreX = (int)(Projectile.Center.X / 16f);
// take the highest floor tile under either column and anchor to that
```

### WR-05: `IsKelpCurtainLayer` does not reproduce the biome's vertical band

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainBiome.cs:35-48`, `64-77`
**Issue:** The new helper's doc comment says it "repeats the same Kelp Curtain vertical band", but `IsBiomeActive` compares `Main.screenPosition.Y` (the top of the camera) against `[0.72, 0.9] * maxTilesY * 16`, while `IsKelpCurtainLayer` compares `player.Center.Y` against the same numbers. The player is roughly half a screen below the camera top, so the server-side spawn band is shifted down by ~half a screen (tens of tiles) relative to the client-visible biome: creatures keep spawning for that distance after the biome's music/background have ended, and do not spawn near the top of the visible band. This is a functional inconsistency in the new predicate, not just wording.
**Fix:** Either subtract the nominal half-screen offset so the two tests cover the same world band,
```csharp
float bandTop = Main.maxTilesY * 0.72f * 16 + Main.screenHeight * 0.5f;
```
or change the doc comment to state explicitly that the spawn band is deliberately offset by the player-centre-vs-camera difference.

## Info

### IN-01: `GiantDandelion.EnterState` sets `netUpdate` even when the state does not change

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs:238-242`, `377`
**Issue:** `RunStateMachine` calls `EnterState(Idle)` and returns on **every** tick that `IsTargetValid()` is false (all players dead/inactive). `EnterState` always sets `NPC.netUpdate = true`, so the NPC requests a sync every tick for the whole window. The steady-state locomotion paths add `if (State != X)` guards, so this is the only repeated path, but it is needless churn.
**Fix:** `if (State != next) { NPC.netUpdate = true; }` (state assignment and timers can still run as-is).

### IN-02: Duplicated death-dust block across four classes

**File:** `MossyThornTurtle.cs:144-159`, `GuppyConch.cs:216-231`, `VerdantRods.cs:245-260`, `GiantDandelion.cs:592-607`
**Issue:** The same six-particle `LichenSlime`/`YggdrasilCyatheaLeafDust` burst (including the identical `Main.rand.NextBool()` scale branch) is copy-pasted four times. The only differences are the dust type and, in one case, the colour.
**Fix:** Extract a shared helper (e.g. `KelpCurtainDust.DeathBurst(NPC npc, int hitDirection, int dustType)`) in the KelpCurtain namespace and call it from all four `HitEffect`s.

### IN-03: Defence literals 10/20 are inline in `GuppyConch`

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs:86`, `127`, `180`
**Issue:** The class defines named constants for its crawl speed and shell duration but the two defence states are bare `10`/`20` in three places, so the design's 防御 value cannot be changed from one source of truth.
**Fix:** Add `private const int NormalDefense = 10;` / `private const int ShelledDefense = 20;` and use them in `SetDefaults`, the shell exit and `EnterShell`.

### IN-04: `MossyThornTurtle.ModifyIncomingHit` is an empty override

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/MossyThornTurtle.cs:116-118`
**Issue:** The override body is empty; the reasoning lives only in the XML comment. It is the only no-op hook in the tranche and adds a call that does nothing.
**Fix:** Remove the override and keep the "减伤 cell empty" note as a comment in `SetDefaults`, or keep it only if the gate script requires the token.

### IN-05: `VerdantRods.TargetPos` is a public mutable field

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs:45`
**Issue:** `public Vector2 TargetPos` is used only inside the class and is not synced. Public mutable state on a `ModNPC` invites external writes and is not part of the class contract (the `DarkGlimmeringRods` precedent it cites makes it a public field too, so this is a pre-existing style).
**Fix:** Make it `private`.

### IN-06: `GiantDandelion`'s 214x263 collision box risks never finding a spawn tile

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GiantDandelion.cs:171-172`, `619-639`
**Issue:** `NPC.width = 214; NPC.height = 263` gives a ~13x16-tile collision box, which tML must fit into a valid spawn position; the spawn gate additionally rejects water and liquid tiles. `03-UAT.md` check 3(a) already records the risk that the creature cannot find a valid spawn area. If the UAT run confirms it, the fix is the extents (not the 0.25f weight).
**Fix:** Keep the sprite as-is and give the collision box a smaller, humanoid-sized footprint (the sprite can still draw at full size), or add an area check to `SpawnChance` that verifies the hitbox fits at `spawnInfo.SpawnTileX/SpawnTileY`.

---

## Fix Resolution

**Resolved:** 2026-09-15, iteration 1. The five Warning findings were fixed against the committed design snapshot (`../01-item-inventory-completed-art-items/evidence/biology.xml`) and committed atomically. The six Info findings were intentionally left for a future pass (the requested scope was Warning-and-above). Full report: `03-REVIEW-FIX.md`.

| Finding | Status | Commit | Change and design row |
| --- | --- | --- | --- |
| WR-01 | resolved | `5183f8bcb` | `MossyThornTurtle.PostAI` now applies the design's 防御 20（旋转/缩壳） through the 缩壳 (`Retracting`) state as well as 旋转 (`Spinning`), while the 缩壳 state keeps the normal 伤害 50（旋转 keeps 75). The class doc comment states the three-state mapping. Design row: stats table `Cqvwdoc54o8vsWx3mIpcoKpinVg`, 伤害 `50（正常）/75（旋转）`, 防御 `10 / 20（旋转/缩壳）`. |
| WR-02 | resolved | `b2f74f5b2` | `GiantDandelion.ThrowBoulder` now takes an `armMidRangeCooldown` flag: the 240-tick gap is armed only from the mid-range `BoulderThrow` path, and the post-smash yank passes `false`, so a smash no longer suppresses the next mid-range attack. Design row: 这个状态下两段攻击间隔不低于240帧. |
| WR-03 | resolved | `0d4cc4ea8` | `GiantDandelion.RunStateMachine` gates the three locomotion states behind the new `AggroRangeTiles` (conservative 30-tile default, recorded in `03-DEVIATIONS.md` §4) and falls back to the no-aggro Idle wander when the player escapes it, so the design's state (0) is reachable. Design row: 游荡在刺苔庭院的沼泽中 / 玩家超过15格且有仇恨时. |
| WR-04 | resolved | `fe20e019d` | `GiantDandelion_Shockwave` now probes the leading edge, the centre and the trailing edge and anchors to the deepest floor found, so the 200 px hit box follows the ground across uneven terrain instead of a single centre column (the `200x32` size itself remains the D-34 default). |
| WR-05 | resolved | `49db5a865` | `KelpCurtainBiome.IsKelpCurtainLayer`'s doc now states the actual server-safe band intent — a player-centred band deliberately about half a screen below the camera-driven `IsBiomeActive` band — instead of claiming to reproduce it. The predicate keeps using `player.Center.Y` and never `Main.screenPosition`, so it stays server-safe (no band bound changed). |

_Reviewed: 2026-09-15_
_Reviewer: the agent (gsd-code-reviewer)_
_Depth: standard_
_Fixed: 2026-09-15 (iteration 1, all five Warnings resolved; six Info findings remain)_
