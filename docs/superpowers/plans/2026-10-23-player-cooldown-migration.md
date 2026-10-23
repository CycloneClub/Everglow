# Player Cooldown Migration Implementation Plan

> Execution: preserve the existing Everglow-acquisition working tree; no staging, commits, branch changes or spreadsheet edits.

Goal: migrate player-scoped handwritten cooldown state into the existing Cooldown registry while preserving original gameplay update phases and lifecycle.

Architecture: existing per-player TypeID storage remains the only countdown. Add an opt-out from automatic PostUpdateMiscEffects ticking and a shared explicit TickCooldown entry point; legacy gameplay hooks invoke the same instance transition. Existing automatic cooldown behavior stays unchanged. Renderers honor existing IsVisible for hidden internal delays.

## Constraints and decisions
- Values, start gates, update order, pause conditions, effect/animation timers, recipes and drops remain unchanged.
- Migrated timers have explicit AutomaticTick=false, CanTickDown=true, PersistentCooldown=true, CooldownNoSave=true; old gameplay hook controls pause/death updates.
- Scope conflict: item instance timers cannot be converted to fixed player TypeID without merging copies. Keep SkullCollection, Vortex E/Q, TerraViewerHowitzer, StabbingSwordItem, GlobalWeaponReplace Starlight, RottenGoldBayonet, FlappedHangGlider; document needed instance support or user sharing decision.
- BloodGoldBayonet and BlossomThorn remain projectile-instance timestamps.
- No packet routing or existing cooldown save format fixes. Hidden rendering support is required for migrated short internal cooldowns.
- Keep audit artifacts external. Plan copied into repository as required by planning workflow; no Git operation.

## Tasks
- [x] Inspect root/local AGENTS, framework, complete active module searches and classify raw cooldown/timer/timestamp matches.
- [x] Snapshot pre-existing 4890 text files and Git state externally.
- [x] RED tests: original hook timing, shared state, pause, death/save, no automatic double tick, hidden renderer.
- [x] Function: CooldownBase.AutomaticTick; CooldownInstance.Tick; Player.GetCooldown/TickCooldown; reuse Tick in automatic loop; skip hidden UI slots.
- [x] Function consumers: StabbingSwordStaminaPlayer recovery45 (initial delay, full-stamina pause, attack refresh), HangingTile_Player switch30 and rope references.
- [x] SpellAndSkull: SkullHand HandCooling18 from original PreUpdate.
- [x] Yggdrasil/Food: Anemo configured ProtectionCooldown ResetEffects and inventory display; Arena60 conditional PostUpdateEquips; Elderberry50 PreUpdateMovement including same-trigger-frame decrement.
- [x] Build Release, focused then full MSTest; diff/BOM check; compare unchanged files and Git state; independent review.

## Review focus
- On the expiry frame, does the original check-before/after-decrement ordering still hold?
- A second item/player must not consume another independent scope's cooldown.
- Removing equipment/losing buffs/exiting arena must retain exact former progression or pause.
- Death does not delete former ordinary fields; nonserialized state does not become saved progress.
- Rendering must not advance time or touch hidden icon textures; no second countdown survives migration.

## Evidence / ledger
- Initial RED compile exposed tests referencing not-yet-added types; adapting tests to compile against current code before measuring missing behavior.


## Completion and verification
- Migrated: AnemoShell configured ProtectionCooldown; SkullHand18; ElderberryDash50; ArenaShield60; StabbingSwordRecovery45; HangingTileSwitch30. Duration/animation fields remain in their original owners.
- Default automatic cooldowns still run in PostUpdateMiscEffects. Migrated cooldowns use the same CooldownInstance transition from their original gameplay hooks; no independent remaining-time field survives.
- Required display support: IsVisible is now honored before layout and graphics access. Anemo's original inventory progress display reads registered state.
- Release build passed (only five existing ImageSharp vulnerability warnings). Focused tests20/20, full suite854/854; nineteen new tests. No client/gameplay loop was started.
- Tests cover explicit phase, repeated start, expiry, CanTickDown pause, retained death state, omitted save entries, distinct players, full-stamina pause, exhaustion recovery, rope failed attachment, and hidden display access. Arena's actual floor activation and client visuals remain runtime checks, not unit-test claims.
- Test-fixture failure resolved by registering only tested cooldown types rather than scanning unrelated module types requiring ModLiquidLib. No runtime dependency or project file change.
- Independent production review found no blocking regression. The reported test registration problem was fixed; final focused/full suites pass.
- Snapshot comparison: only the19 intended C# files changed relative to this turn; unrelated prior source/localization edits remain identical. Branch, HEAD and staged state remain unchanged.
- Byte-level BOM scan against origin/1.4.4 passed263 files; diff whitespace check passed.

## Retained implementations / unresolved scope choices
| Candidate | Retained reason |
|---|---|
| SkullCollection | Each accessory copy owns240 ticks; only equipped copy advances. Player TypeID would merge copies and change transfer/pause behavior. |
| VortexVanquisher E and Q | Each weapon instance owns its own counters, currently advanced by inventory drawing. Moving these into the existing fixed player slots would merge copies. Draw-driven decrement is explicitly still unresolved; needs instance-scoped support in this system or an approved scope change, not an unannounced shared timer. |
| TerraViewerHowitzer | Instance-specific initial180-tick loading and right-click cooldown; off-inventory pause and projectile's current-held-item display would change. |
| StabbingSwordItem | Per-item initial/repeated heavy-stab delay, modified by subclasses; inventory-only progression. |
| GlobalWeaponReplace Starlight | InstancePerEntity item state; shared player ID would merge weapons. |
| RottenGoldBayonet | Projectile retains the source item reference;60-tick mark interval belongs to that exact item. |
| FlappedHangGlider |45-tick waiting is bound to each item's durability; held/grounded progression and copy switching remain. |
| BloodGoldBayonet / BlossomThorn | Projectile-instance timestamps; retained20/15 intervals and different success/attempt start points. |

No new source was restored for dead fields. NPC/minion/held-projectile AI, hit immunity, animations, charges, buff duration/windows, UI repeat, world/TileEntity timing, healing budgets and unconnected historical fields remain outside the migration. Full module audit evidence is retained in the external cooldown-migration output directory.

Network packet shapes and routing were not changed. Live client/server synchronization, visual behavior, actual death hook ordering and save/exit/reload were not exercised in tML; unit tests prove the direct hook/state/save-filter behavior only.
