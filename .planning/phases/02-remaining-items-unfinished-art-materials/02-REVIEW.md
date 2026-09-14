---
phase: 02-remaining-items-unfinished-art-materials
reviewed: 2026-09-14T12:05:05Z
depth: standard
files_reviewed: 26
files_reviewed_list:
  - Sources/Modules/Yggdrasil/KelpCurtain/Buffs/RedAlgae_FriendlyDebuff_glocalNPC.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/BambooStepTalisman.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Accessories/PeachBranchAmulet.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeBreastPlate.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeGreaves.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeHeaddress.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeMask.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/AlcoholicDrinks.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/BambooHairpin.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/DiscipleVanity.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/JadeSnakeEgg.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/PeachBlossomKite.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/SkillBambooSlip.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Pets/PandaPet.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Placeables/RegionalCraftingStation.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BambooWeapon.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/BoulderCatapult.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/DiscipleSword.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/FluorescentHydraStaff.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ReekingBait.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/RestrictionDeviceRE01.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_Proj.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_SubProj.cs
  - Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/TendonGreatbow_Arrow.cs
findings:
  critical: 2
  warning: 2
  info: 5
  total: 9
status: fixed
fixes_applied: [CR-01, CR-02, WR-01, WR-02]
fixes_unresolved: [IN-01, IN-02, IN-03, IN-04, IN-05]
fixed_at: 2026-09-14
fix_commits:
  CR-01: 05e48b1ea
  CR-02: 737cefb7f
  WR-01: 8d06b7a34
  WR-02: 7647291e2
---

# Phase 02: Code Review Report

**Reviewed:** 2026-09-14T12:05:05Z
**Depth:** standard
**Files Reviewed:** 26
**Status:** fixed — CR-01, CR-02, WR-01 and WR-02 resolved (see the resolution note under each finding); IN-01..IN-05 are advisory and were intentionally left unresolved.

## Summary

Reviewed the 26 Phase-2 `.cs` files under `Sources/Modules/Yggdrasil/KelpCurtain/`: the 红月水藻 (CrimsonMoonAlgae) four-piece armor set, `BoulderCatapult`/`TendonGreatbow` and their projectiles, the red-algae detonation `GlobalNPC`, the `KelpCurtainPlayer` additions, and the identity-only "shell" items. Static constraint checks came back clean (see below), and the projectile item definitions match the DEVIATIONS ledger for damage/use-time/rarity/value. However, two functional defects were proven by tracing the tModLoader hook contracts:

1. **`TendonGreatbow` never fires its own arrow.** The `Shoot` override mutates the by-value `type` parameter and returns `true`; tML's `ItemLoader.Shoot` passes `type` by value and returns only a `bool`, so the assignment cannot reach vanilla's spawn call. `TendonGreatbow_Arrow` (and its +10% boss-damage clause) is dead code.
2. **The breastplate heal is nullified at full health.** `ModPlayer.OnHurt` runs *before* health is reduced, and the code clamps the pre-heal to `statLifeMax2`, so at full/near-full life the 15% heal is swallowed by the max-life clamp.

**Constraint compliance (all pass):**

- No new binary/art assets: the diff contains only `.cs` files; every missing-art item reuses the existing `Commons.ModAsset.White_Mod` fallback.
- No `[AutoloadEquip]` on any class that also overrides `Texture` with the fallback.
- All 26 files are file-scoped namespaces, tab-indented, LF-only, no UTF-8 BOM, trailing newline present (verified byte-level).
- No hand-edited HJSON; no `.hjson`/localization files are in the diff.
- `dotnet build` was reported green for the phase (build was not re-run in this read-only review).

---

## Critical Issues

### CR-01 [BLOCKER]: `TendonGreatbow.Shoot` mutates a by-value `type` and returns `true`, so `TendonGreatbow_Arrow` is never spawned

**Status:** ✅ Fixed in `05e48b1ea`. The type substitution now runs in `ModifyShootStats(Player, ref Vector2, ref Vector2, ref int type, ref int, ref float)` on the value the vanilla spawn call actually reads, so `TendonGreatbow_Arrow` (and its +10% boss clause) fires for any arrow ammo. The inert `Shoot` override and the then-unused `Terraria.DataStructures` using were removed; `ProjectileID.WoodenArrowFriendly` remains the no-ammo fallback in `SetDefaults`.

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs:38-45`

**Issue:** The override does:
```csharp
public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
{
    if (type == ProjectileID.WoodenArrowFriendly)
        type = ModContent.ProjectileType<TendonGreatbow_Arrow>();
    return true; // vanilla spawns with its OWN `type`; the local mutation is discarded
}
```
tModLoader's dispatcher is (verified against `tModLoader/1.4.4` `ItemLoader.cs`):
```csharp
public static bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source,
    Vector2 position, Vector2 velocity, int type, int damage, float knockback, bool defaultResult = true)
{
    ...
    return defaultResult && (item.ModItem?.Shoot(player, source, position, velocity, type, damage, knockback) ?? true);
}
```
`type` is an `int` (no `ref`), and the method's return value is a `bool`. Modifying `type` inside `ModItem.Shoot` therefore has **no effect** on the projectile vanilla spawns. The correct hook to change the projectile type is `ModifyShootStats(..., ref int type, ...)`, or the override must spawn the projectile itself and return `false`.

Consequences:
- `TendonGreatbow_Arrow` is never instantiated anywhere; class is dead code.
- The weapon's documented signature effect — `+10% final damage against boss targets` (`TendonGreatbow_Arrow.cs:35-42`) — can never trigger.
- The phase's own runtime UAT (`02-DEVIATIONS.md:248`, `02-VALIDATION.md:82`) states "arrows are consumed and **the mod arrow fires**"; that expectation is not met.

The repository already has the correct pattern in two places: `Items/Weapons/GreenThornBallLauncher.cs:33-37` (unconditional `type = ModContent.ProjectileType<...>()` in `ModifyShootStats`) and `YggdrasilTown/Items/Weapons/LightSeeker/BowOfEnlightment.cs:36-42` (the same `WoodenArrowFriendly` check, but in `ModifyShootStats`).

**Fix:**
```csharp
public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
{
    // Design/UAT: the greatbow fires its own tendon arrow with any arrow ammo.
    type = ModContent.ProjectileType<TendonGreatbow_Arrow>();
}
```
Remove the now-inert `Shoot` override. If special arrows are meant to keep their identity, keep the `if (type == ProjectileID.WoodenArrowFriendly)` guard — but the UAT text says the mod arrow fires "with any arrow", so the unconditional form is the one that matches the accepted behaviour. After the change, `ProjectileID.WoodenArrowFriendly` can remain as the no-ammo fallback in `SetDefaults`.

### CR-02 [BLOCKER]: Breastplate heal is applied before damage and clamped, so it does nothing at full HP

**Status:** ✅ Fixed in `737cefb7f`. The override moved from `OnHurt` (runs before health is reduced) to `PostHurt` (runs after), and uses `Player.Heal((int)(info.Damage * 0.15f))` instead of writing `Player.statLife` directly. `Player.Heal` applies the `statLifeMax2` clamp and handles the heal effect/state update, so the 15% heal now survives at full HP and the ad-hoc `whoAmI == Main.myPlayer`/`HealEffect` guard is gone. The `info.Damage >= 10` gate is preserved. Remaining unverified: live-client confirmation of the exact heal amount (runtime check is in the phase UAT bundle).

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs:103-114`

**Issue:** `ModPlayer.OnHurt` is documented by tML as: *"Called on the local client taking damage. Called right before health is reduced."* The code:
```csharp
public override void OnHurt(Player.HurtInfo info)
{
    if (CrimsonMoonAlgaeBreastPlate && info.Damage >= 10)
    {
        int heal = (int)(info.Damage * 0.15f);
        Player.statLife = Math.Min(Player.statLife + heal, Player.statLifeMax2); // clamps BEFORE damage is subtracted
        ...
    }
}
```
Because health is reduced *after* this hook, the pre-damage clamp absorbs the heal:
- full HP (100/100), hit for 20 → `min(100+3, 100) = 100`, then `100-20 = 80`. Intended `100-20+3 = 83`. **The 15% heal contributes 0.**
- 99/100, hit for 20 → `min(102,100)=100`, then 80 vs intended 82 (2 HP lost).

The heal only survives intact when `statLife + heal < statLifeMax2`, i.e. away from full HP. Since the item's sole design effect is "受到大于等于10的伤害时治疗该伤害的15%" (heal 15% of any hit of 10+), the item fails its primary advertised behaviour in the most common case. Directly assigning `statLife` also bypasses `Player.Heal`, which is the supported path (handles the max-life clamp, heal effect, and multiplayer state update).

**Fix:** Replace the `OnHurt` override with `PostHurt` (called after health is reduced) and use `Player.Heal`:
```csharp
public override void PostHurt(Player.HurtInfo info)
{
    if (CrimsonMoonAlgaeBreastPlate && info.Damage >= 10)
    {
        Player.Heal((int)(info.Damage * 0.15f));
    }
}
```
`Player.Heal` already applies the `statLifeMax2` clamp correctly and emits the heal effect, so the manual `whoAmI == Main.myPlayer` / `HealEffect` guard is no longer needed.

---

## Warnings

### WR-01 [WARNING]: Detonation damage hardcodes the buff's max duration (`900 - buffTime`) and duplicates a literal spread across seven call sites

**Status:** ✅ Fixed in `8d06b7a34`. `RedAlgae_FriendlyDebuff` now exposes `public const int Duration = 900;` as the single source of truth. The detonation uses `RedAlgae_FriendlyDebuff.Duration - buffTime`, and all seven applicator sites (`RedAlgaeMagicStaff_Proj`, `RedAlgaeMagicWhip_Proj`, `RedAlgaeMagicSpellBook_proj`, `CrimsonMoonAlgaeSummonStaff_minion_spore`, `CrimsonMoonAlgaeSummonStaff_minion_Explosion`, `RedAlgaeMinionGyroscope_Proj` ×2) apply the buff with the constant. Values are unchanged (all 900 frames), so behaviour is identical; the numeric coupling is removed. Note: this edits the accepted Phase-1 appliers referenced by blocker E-1, but only re-points the duration literal — the E-1 duration-doubling clause itself remains open.

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Buffs/RedAlgae_FriendlyDebuff_glocalNPC.cs:39`

**Issue:** `int damage = 900 - buffTime;` assumes `RedAlgae_FriendlyDebuff` is always applied with a 900-frame duration. That 900 is currently a bare literal repeated at every applicator: `RedAlgaeMagicStaff_Proj.cs:102`, `RedAlgaeMagicWhip_Proj.cs:130`, `RedAlgaeMagicSpellBook_proj.cs:175`, `CrimsonMoonAlgaeSummonStaff_minion_spore.cs:59`, `CrimsonMoonAlgaeSummonStaff_minion_Explosion.cs:89`, and `RedAlgaeMinionGyroscope_Proj.cs:86,165`. The detonation and the application are coupled only by a shared magic number; changing any one applicator (or refreshing the buff with a different duration) silently distorts the detonation damage — including making it negative (no detonation) if a longer duration is ever used.

**Fix:** Promote the duration to a single source of truth on the buff and reference it from both sides:
```csharp
public class RedAlgae_FriendlyDebuff : ModBuff
{
    public const int Duration = 900;
    ...
}
// detonation
int damage = RedAlgae_FriendlyDebuff.Duration - buffTime;
// applicators
target.AddBuff(type, RedAlgae_FriendlyDebuff.Duration);
```

### WR-02 [WARNING]: No-op consumables (`JadeSnakeEgg`, `ReekingBait`) silently destroy the item on use

**Status:** ✅ Fixed in `7647291e2`. Both items now override `CanUseItem(Player)` to return `false`, so they no longer consume a stack while the Phase-7 summon encounter does not exist. `Item.consumable = true` is retained so the intended behaviour is a one-line gate flip when the encounter lands; the recorded blockers in the inventory/ledger are unchanged. All changes are guarded so no new gameplay system was invented.

**Files:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/JadeSnakeEgg.cs:20-32`, `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ReekingBait.cs:11-26`

**Issue:** Both items set `Item.consumable = true` and provide a use animation/sound, but neither sets `Item.shoot` nor overrides `CanUseItem`/`UseItem`. The documented blocker is that the summon encounter is Phase 7, so "the item consumes without summoning anything" (`02-DEVIATIONS.md`). That is intentional, but the implemented behaviour is a **destructive no-op**: the player loses the stack and gets only a `SoundID.Roar` with no effect. Until the encounter exists this is an accidental-item-loss hazard (and an easy way to burn a rare drop), not a neutral placeholder.

**Fix:** Make the shells non-destructive until their effect exists, e.g. set `Item.consumable = false` (or gate use):
```csharp
public override bool CanUseItem(Player player)
{
    // Encounter not implemented (Phase 7 blocker) - do not consume the item.
    return false;
}
```
Then flip both when the summon is implemented. (This is a recommendation on an intentionally-blocked item, not a claim that the blocker decision was wrong.)

---

## Info

### IN-01 [INFO]: Dedicated-server equip slots resolve to `-1`, so `headSlot`/`bodySlot`/`legSlot` diverge between client and server

**Files:** `Items/Armors/CrimsonMoonAlgae/CrimsonMoonAlgaeHeaddress.cs:22-33,51`, `CrimsonMoonAlgaeMask.cs:21-32,48`, `CrimsonMoonAlgaeBreastPlate.cs:21-33,48`, `CrimsonMoonAlgaeGreaves.cs:21-33,48`

**Issue:** `Load()` returns early on `Main.dedServ`, so `EquipLoader.AddEquipTexture` never registers the equip textures on a dedicated server. The tML docs state `EquipLoader.GetEquipSlot(...)` "Returns -1 if no EquipTexture with the given name is found" (verified in the local `tModLoader.xml` at member `EquipLoader.GetEquipSlot`), so the server-side assignment is `Item.headSlot = -1` etc. This is not a crash and does not break equipping/defense/set bonuses (those use `Item` types and `defense`, not the slot fields), but any future server-side code that keys on `Item.headSlot/bodySlot/legSlot` or `Player.head/body/legs` would silently see different values than the client. Worth a comment and a note in the runtime-verification ledger.

### IN-02 [INFO]: Detonation hit direction is hardcoded

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Buffs/RedAlgae_FriendlyDebuff_glocalNPC.cs:54`

**Issue:** `HitDirection = 1` ignores the direction of the triggering hit (`hit.HitDirection`), so knockback/visual hit direction of the detonation never matches the attack that triggered it. Cosmetic, but trivially fixable by passing the incoming `hit.HitDirection` through `ApplyRedAlgaeDetonation`.

### IN-03 [INFO]: `AppliedDirectBonus` is not reset in `OnSpawn`

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/BoulderCatapult_Proj.cs:16,43`

**Issue:** The once-per-projectile guard is an auto-property with no explicit reset. Current tML versions create a fresh `ModProjectile` instance per `SetDefaults`, so this is safe today, but resetting it in `OnSpawn` is cheap insurance against any future instance reuse and makes the invariant explicit.

### IN-04 [INFO]: Several shells carry flags/categories that are inert because their slot/tile does not exist

**Files:** `Items/Misc/BambooHairpin.cs:24`, `Items/Misc/DiscipleVanity.cs:26`, `Items/Placeables/RegionalCraftingStation.cs:14-27`

**Issue:** `BambooHairpin`/`DiscipleVanity` set `Item.vanity = true` but declare no `headSlot`, so the flag is dead and the items cannot actually be worn as vanity; `RegionalCraftingStation` is filed under `Items/Placeables` with category `Placeables` but has no `createTile` and cannot be placed. These are documented blockers (D-13/D-18/D-19, missing art/system), so they are recorded as known gaps rather than regressions — but they should not ship in this state and the blocker ledger should stay prominent.

### IN-05 [INFO]: Detonation runs on every side that processes the hit, producing redundant client-local hits/combat text

**File:** `Sources/Modules/Yggdrasil/KelpCurtain/Buffs/RedAlgae_FriendlyDebuff_glocalNPC.cs:8-25,57-58`

**Issue:** `OnHitByProjectile`/`OnHitByItem` execute on the server and on the owner client. `StrikeNPCWithCustomCombatText` calls `npc.StrikeNPC` directly (not `ApplyDamageToNPC`, which is the net-synced path), so the client branch performs a local, unsynced hit and a local `npc.DelBuff`. The server branch remains authoritative, so this is not double damage; the visible risk is a combat-text value computed from a client-side `buffTime` that can differ by a tick from the server's, plus local buff-removal flicker before the server sync arrives. Gating the damage to the authoritative side (and syncing the combat text) would remove the redundancy.

---

_Reviewed: 2026-09-14T12:05:05Z_
_Reviewer: the agent (gsd-code-reviewer)_
_Depth: standard_
