---
phase: 02
fixed_at: 2026-09-14T12:45:00Z
review_path: .planning/phases/02-remaining-items-unfinished-art-materials/02-REVIEW.md
iteration: 1
findings_in_scope: 4
fixed: 4
skipped: 0
status: all_fixed
---

# Phase 02: Code Review Fix Report

**Fixed at:** 2026-09-14
**Source review:** `.planning/phases/02-remaining-items-unfinished-art-materials/02-REVIEW.md`
**Iteration:** 1

**Summary:**
- Findings in scope: 4 (CR-01, CR-02, WR-01, WR-02)
- Fixed: 4
- Skipped: 0
- Info findings intentionally left unresolved: IN-01, IN-02, IN-03, IN-04, IN-05

## Fixed Issues

### CR-01: `TendonGreatbow` never fires `TendonGreatbow_Arrow`

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/TendonGreatbow.cs`
**Commit:** `05e48b1ea`
**Applied fix:** Removed the ineffective `Shoot` override (its `type` parameter is passed by value, so the mutation was discarded) and moved the substitution into `ModifyShootStats(Player, ref Vector2, ref Vector2, ref int type, ref int, ref float)`, which mutates the value the vanilla spawn call reads. The unused `using Terraria.DataStructures;` was removed. `TendonGreatbow_Arrow` — including its `+10%` boss-damage clause in `ModifyHitNPC` — is now reachable for any arrow ammo; `ProjectileID.WoodenArrowFriendly` remains the no-ammo fallback in `SetDefaults`.

### CR-02: Breastplate heal is swallowed at full HP

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/KelpCurtainPlayer.cs`
**Commit:** `737cefb7f`
**Applied fix:** Replaced `OnHurt` (documented to run *before* health is reduced) with `PostHurt` and replaced the manual `Player.statLife = Math.Min(...)` write with `Player.Heal((int)(info.Damage * 0.15f))`. `Player.Heal` applies the `statLifeMax2` clamp and handles the heal effect and life-state update, so the 15% heal now survives at full/near-full HP; the `info.Damage >= 10` gate is preserved and the ad-hoc `whoAmI == Main.myPlayer`/`HealEffect` guard was dropped. Logic change — live-client heal amount remains for the phase UAT bundle.

### WR-01: Detonation hardcodes `900 - buffTime` and duplicates the literal across seven call sites

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Buffs/RedAlgae_FriendlyDebuff.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Buffs/RedAlgae_FriendlyDebuff_glocalNPC.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Magic/RedAlgaeMagicStaff_Proj.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Magic/RedAlgaeMagicSpellBook_proj.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/RedAlgaeMagicWhip_Proj.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/CrimsonMoonAlgaeSummonStaff_minion_spore.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/CrimsonMoonAlgaeSummonStaff_minion_Explosion.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Summon/RedAlgaeMinionGyroscope_Proj.cs`
**Commit:** `8d06b7a34`
**Applied fix:** Added `public const int Duration = 900;` to `RedAlgae_FriendlyDebuff` as the single source of truth. The detonation now computes `RedAlgae_FriendlyDebuff.Duration - buffTime`, and all seven applicator sites (`AddBuff(type, 900)` → `AddBuff(type, RedAlgae_FriendlyDebuff.Duration)`) reference the constant. All values remain 900, so behaviour is unchanged; the numeric coupling between the detonation and the applicators is removed. This touches the accepted Phase-1 appliers named by blocker E-1, but only re-points the duration literal; E-1's duration-doubling clause remains open.

### WR-02: No-op consumables silently destroy the item

**Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/Items/Misc/JadeSnakeEgg.cs`, `Sources/Modules/Yggdrasil/KelpCurtain/Items/Weapons/ReekingBait.cs`
**Commit:** `7647291e2`
**Applied fix:** Added `public override bool CanUseItem(Player player) => false;` to both items (Allman body form) so they no longer consume a stack while the Phase-7 summon encounter is absent. `Item.consumable = true` is retained so the intended behaviour is a one-line gate flip when the encounter lands. Comments were updated to describe the non-destructive gate; no new gameplay system was invented and the recorded inventory/ledger blockers are unchanged.

## Intentionally Unresolved

The five Info findings (IN-01..IN-05) are advisory and were out of the requested fix scope: dedicated-server equip-slot `-1` divergence (IN-01), hardcoded detonation hit direction (IN-02), `AppliedDirectBonus` reset hygiene (IN-03), inert shell flags/categories (IN-04), and redundant client-local detonation hits/combat text (IN-05). Each is documented in `02-REVIEW.md` and remains for a future pass.

## Verification

- Build: `dotnet build /p:Configuration=Release /p:WarningLevel=0` → exit 0, `0 Warning(s)` / `0 Error(s)`, `Everglow.tmod` produced.
- Gate: `check-phase2.ps1 -RequireAll` → exit 0, `OK(0): phase2 implemented = 21 / 21 (full=9, shell=12)`.
- Verification ran in the **main checkout**, not the isolated worktree: the worktree (nested under `.claude/worktrees/`) could not locate `tModLoader.targets` because the ModBuilder searches a fixed parent depth, so the four fix commits were fast-forwarded onto the branch and the gates were run from the main checkout per the worktree protocol's documented fallback.
- Text-format constraints (tabs, LF, UTF-8 without BOM, trailing newline) were verified byte-level after the edits.

---

_Fixed: 2026-09-14_
_Fixer: the agent (gsd-code-fixer)_
_Iteration: 1_
