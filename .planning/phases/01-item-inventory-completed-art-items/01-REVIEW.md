---
phase: 01-item-inventory-completed-art-items
reviewed: 2026-09-13T05:40:55Z
depth: standard
base_ref: 8ed6f5862bdf12b2662cb527127da80e69a2064c
files_reviewed: 5
files_reviewed_list:
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/ElftigernPowder.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/WitheredMask.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs
findings:
  critical: 1
  warning: 2
  info: 5
  total: 8
status: issues_found
---

# Phase 01: Code Review Report

**Reviewed:** 2026-09-13T05:40:55Z
**Depth:** standard
**Base ref:** `8ed6f5862` (prior phase review commit)
**Files Reviewed:** 5
**Status:** issues_found

## Summary

Wave-scoped review of the five Phase 1 carry-over item classes added by plan 01-06
(`ArmOfGiantTree`, `ElftigernPowder`, `ForestBreath`, `WitheredMask`, `QuetzalsWish`).
The four "identity-only" items (`ElftigernPowder`, `ForestBreath`, `WitheredMask`,
`QuetzalsWish`) are small, conventional `ModItem` defs; the shared
`Commons.ModAsset.White_Mod` fallback matches the existing `RadialCarapace` precedent,
and the `LocalizationUtils.Categories.*` values all resolve to real constants
(`Weapons.Melee`, `Materials`, `Miscs`, `Vanity`). No binary/art asset was added or
modified, and no BOM/CRLF violations appear in the new files.

`ArmOfGiantTree` is the only class with behaviour and carries all findings below.

Note: plans 01-01..01-05 were covered by the prior deep review (59 files) now in git
history; this report is the incremental scope since that review
(`8ed6f5862..HEAD`).

## Critical Issues

### CR-01: `ChargeTimer` is mutable per-type state on a shared `ModItem` singleton

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs:16`
**Issue:** tModLoader creates one `ModItem` instance per item type and shares it across
every stack/copy and every player (`ModItem` has no `CloneNewInstances` in 1.4; the repo
only overrides it on `ModProjectile`/`ModPlayer`). `public int ChargeTimer;` therefore
holds global, cross-player state. In multiplayer both players' charge accumulates into
the same field, and two `ArmOfGiantTree` copies in one inventory share it (holding one
and charging, then holding the other, skips the reset because both have the same `Type`).
The result is a free/instant full-charge shockwave — incorrect behaviour, not just a
robustness concern.
**Fix:** Move the charge to per-player state:
```csharp
// ArmOfGiantTree.cs
public override void HoldItem(Player player)
{
    var mp = player.GetModPlayer<KelpCurtainPlayer>();
    // ... read/write mp.ArmOfGiantTreeCharge instead of this.ChargeTimer
}

public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
{
    float charge = player.GetModPlayer<KelpCurtainPlayer>().ArmOfGiantTreeCharge / (float)MaxChargeFrames;
    damage *= MathHelper.Lerp(0.75f, 2f, charge);
}
```
(Add a synced `int ArmOfGiantTreeCharge` to `KelpCurtainPlayer`; reset it in `ResetEffects`.)

## Warnings

### WR-01: Shockwave damage is applied client-side only, with no multiplayer sync

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs:75-93`
**Issue:** The shockwave loop is gated on `Main.myPlayer == player.whoAmI`, i.e. it runs
only on the owner's client, and calls `npc.SimpleStrikeNPC(...)` directly. `SimpleStrikeNPC`
does not perform server-authoritative damage or `netUpdate` on its own, so in multiplayer
the area damage is client-local (desync / ghost damage) and can be rejected or double-applied
relative to the normal melee hit. This path could not be verified at runtime here.
**Fix:** Gate on server authority and let the normal netcode carry it, e.g. run the loop when
`Main.netMode != NetmodeID.MultiplayerClient` (or send a `ModPacket` from owner → server) and
keep client-side only the sound/visual.

### WR-02: Right-click "ordinary swing" still receives the 0.75x charge penalty

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs:45-53,67-71`
**Issue:** `HoldItem` resets `ChargeTimer = 0` for the right-click ordinary swing (and the
comment calls it "ordinary"), but `ModifyWeaponDamage` unconditionally applies
`MathHelper.Lerp(0.75f, 2f, charge)`, so with `charge == 0` the ordinary swing deals only
75% of base damage. The stated design reserves the 75%..200% scaling for the *charged*
left-click smash, so the alt swing either should not be penalised or the doc/design must
say it is.
**Fix:** Only apply the charge modifier when the current use is the charged left-click,
e.g. branch on `player.altFunctionUse != 2`, or document the 75% floor as intentional.

## Info

### IN-01: `ChargeTimer` should not be public

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs:16`
**Issue:** No external consumer; a mutable public field invites accidental cross-class use.
**Fix:** Make it `private` (or remove entirely per CR-01).

### IN-02: Shockwave uses raw `Item.damage`, not the charged weapon damage

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/UnderwaterTreasury/ArmOfGiantTree.cs:78`
**Issue:** `int shockDamage = Math.Max(1, Item.damage)` is the unmodified base (55), while
the comment calls it "100% shockwave" next to a 200% charged smash. If "100%" means 100% of
the *charged* hit (200% base), this under-delivers; if it means 100% of base, it is correct.
**Fix:** Clarify intent in the comment/design; if scaled, multiply by the resolved charge.

### IN-03: `ForestBreath` lacks the quest-item flag

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/ForestBreath.cs:15-22`
**Issue:** The class doc calls it a 任务道具 (quest item), but `Item.questItem` is never set,
so Phase 6 consumers that check it may not treat it as one.
**Fix:** Set `Item.questItem = true;` if the Phase 6 consumer expects it; otherwise leave as-is.

### IN-04: `ElftigernPowder` uses dead use-configuration

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Materials/ElftigernPowder.cs:19-28`
**Issue:** `consumable`, `useStyle`, `useTime`, `useAnimation` are configured but
`CanUseItem` always returns `false`, so the item can never be used/consumed (documented
Phase 6 blocker). The config is unused surface.
**Fix:** Keep (harmless) but ensure the Phase 6 purge hook flips `CanUseItem`; or drop the
unused fields until then.

### IN-05: Deferred equip/projectile wiring for `WitheredMask` and `QuetzalsWish`

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/WitheredMask.cs:16-23` and `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/QuetzalsWish.cs:17-32`
**Issue:** `WitheredMask` sets `Item.vanity = true` without a `headSlot`/equip texture, and
`QuetzalsWish` sets melee stats with no `Item.shoot`/projectile/buff. Both are recorded as
named artwork/effect blockers, so this is expected, but neither is loadable beyond a plain
item.
**Fix:** Track these as Phase 7 follow-ups so the blockers are not silently dropped.

---

_Reviewed: 2026-09-13T05:40:55Z_
_Reviewer: gsd-code-reviewer (orchestrator-inline; subagent runtime unavailable)_
_Depth: standard_
