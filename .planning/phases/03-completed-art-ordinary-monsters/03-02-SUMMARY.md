---
phase: 03-completed-art-ordinary-monsters
plan: 02
subsystem: gameplay-content
tags: [tmodloader, terraria, npc, local-ai, subworld, yggdrasil, kelp-curtain, loot-table, shell-state, poison, biology-matrix]

# Dependency graph
requires:
  - phase: 03-completed-art-ordinary-monsters
    plan: 01
    provides: the 31-row biology matrix + ASCII gate (D-24), the class shape to mirror (MossyThornTurtle) and the server-safe KelpCurtainBiome.IsKelpCurtainLayer(Player) predicate
  - phase: 01-item-inventory-completed-art-items
    provides: the GuppyShell accessory item type consumed by GuppyConch's 11% drop
provides:
  - Everglow.Yggdrasil.KelpCurtain.NPCs.GuppyConch (格普螺, full implementation with a hit-triggered shell state)
  - Everglow.Yggdrasil.KelpCurtain.NPCs.VerdantRods (叶飞棍, full implementation with water suffocation and 50% poison)
  - the D-39 "empty but commented loot table" precedent for a creature whose designed drops do not exist
affects: [03-03, 03-04, phase-4-remaining-ordinary-monsters, phase-7-bosses, phase-8-source-acceptance]

# Actuals (#2632) - pairs with the plan's estimate (58000 tokens, 3 tasks) to calibrate future estimates.
# chars/4 over the files actually changed, never a harness token count.
actuals:
  tokens: 26772
  tasks: 3
  commits: 3   # measured at SUMMARY write: git rev-list --count bfec96326..HEAD (3 task commits; the plan-metadata commit is the +1 verify-work allows)
plan_head_before: bfec96326be7511c19738f092279e0516e403246

tech-stack:
  added: []
  patterns:
    - "Hit-triggered NPC state machine (shell retract) kept server-authoritative in HitEffect while the damage scaling stays in ModifyIncomingHit"
    - "Design-documented drop that has no repository item rendered as a commented, deliberately empty loot table (D-39)"
    - "Environmental hazard (water suffocation) implemented as a small, non-fatal, server-authored NPC.life drain plus an escape bias instead of a new debuff"

key-files:
  created:
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs
    - Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs
  modified:
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json
    - .planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md
    - .planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md

key-decisions:
  - "The shell-state transition lives in HitEffect, not ModifyIncomingHit: tML documents ModifyIncomingHit as 'ONLY ... properties of the HitModifiers' with side effects belonging to the OnHit hooks, and HitEffect is the documented on-hit hook that also runs on the server (the plan's key_link named ModifyIncomingHit, but its action left the hook open and both the netmode guard and netUpdate are preserved)"
  - "Spawn gating reuses the plan-03-01 server-safe KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player); IsBiomeActive is never called from a spawn hook because its camera term is zero on a dedicated server"
  - "叶飞棍's 50% poison is applied unguarded on the local client (Player.AddBuff performs the hurt-path buff sync), while the water-suffocation life drain is netmode-guarded because it is authoritative NPC state"
  - "格普螺's absent 软体甲壳碎片 and 叶飞棍's absent 飞棍毛发/毒腺 produce no type reference at all: one row wires only GuppyShell, the other ships an explicitly empty ModifyNPCLoot"
  - "Both rows' internal_name was populated in the same edit that flipped code_complete, so the gate's class-resolution biconditional held at every point where the gate was asserted green"

patterns-established:
  - "A design drop that does not exist is not a stub: the loot table is written empty with the blocker in the matrix and the ledger (D-37/D-39)"
  - "An environmental hazard on a neutral creature is represented with a clamped NPC.life drain plus an escape bias rather than an invented debuff"

requirements-completed: []
requirements-advanced: [BIO-01, BIO-03, BIO-06, QUAL-03]

coverage:
  - id: D1
    description: "格普螺 (GuppyConch) end-to-end: subworld-isolated spawn, local crawl AI with the hit-triggered shell retract (defense 10 -> 20, 0.85 -> 0.70 incoming damage) and the 11% GuppyShell drop"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exits 0, packages Everglow.tmod)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariants 5/7/8 (class resolution, SpawnChance tokens, Main.dedServ, item-type resolution)"
        status: pass
    human_judgment: true
    rationale: "Spawn isolation, the retract feel and the drop are runtime properties; the D-21 client bundle in plan 03-04 records the run."
  - id: D2
    description: "叶飞棍 (VerdantRods) end-to-end: subworld-isolated spawn, neutral eight-frame hover/circle AI, water suffocation with escape, 50% poison, and an explicitly empty loot table"
    requirement: BIO-03
    verification:
      - kind: integration
        ref: "dotnet build /p:Configuration=Release /p:WarningLevel=0 (exits 0)"
        status: pass
      - kind: other
        ref: "check-biology.ps1 invariants 5/7/8 (class resolution, SpawnChance tokens, Main.dedServ, no item-type reference to an absent drop)"
        status: pass
    human_judgment: true
    rationale: "The hover feel, the poison rate and the suffocation are runtime properties; the D-21 client bundle in plan 03-04 records the run."
  - id: D3
    description: "The matrix pair (03-BIOLOGY.json row flip + 03-BIOLOGY.md mirror) reconciled with the precise drop blockers and the two classes' internal names, keeping the frozen counts, tranche ids, texture_complete set and design_art set"
    requirement: BIO-01
    verification:
      - kind: other
        ref: "check-biology.ps1 (invariants 2-6, 11) prints 'OK(0): phase3 tranche = 5 / 5 (rows=31)' and 'OK: implemented classes = 4 / 5'"
        status: pass
      - kind: other
        ref: "cell-by-cell JSON/MD parity check (all 31 rows: id, phase, name_en, internal_name, texture_complete, design_art, code_complete, status, blockers, deferred)"
        status: pass
    human_judgment: false
  - id: D4
    description: "03-DEVIATIONS.md extended (sections 4/5/7/8) with the shell timer, spawn weights, suffocation cadence, knockback conversions, the copper value/rarity reuse, the OnHitPlayer poison-locality note and the two rows' drop dispositions"
    verification: []
    human_judgment: true
    rationale: "It records design-facing assumptions and blockers that only a designer can confirm; no offline check can validate the intent."

duration: 157min
completed: 2026-09-15
status: complete
---

# Phase 3 Plan 02: 格普螺 + 叶飞棍 Summary

**A subworld-isolated shell-up neutral snail (`GuppyConch`) with its Phase 1 accessory drop, and a neutral eight-frame flying rod (`VerdantRods`) with water suffocation and a documented 50% poison**

## Performance

- **Duration:** ~157 min (wall clock)
- **Started:** 2026-09-15T13:23:21Z (inherited base `bfec96326`)
- **Completed:** 2026-09-15T16:00:00Z
- **Tasks:** 3
- **Files modified:** 5 (2 new classes, 3 planning artifacts)

## Accomplishments

- `GuppyConch.cs` implements 格普螺 end-to-end beside its tracked `GuppyConch.png` (so tML's default texture resolution finds the approved art with no `Texture` override and no asset move): `SpawnModBiomes` + registration with `NPCSpawnManager`, subworld-only `SpawnChance` gated on `SubworldSystem.IsActive<YggdrasilWorld>()` **and** the server-safe `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)`, a local crawl `AI()` (0.4f, reversing at walls and at un-walkable ledges), the design's hit-triggered shell retract (免疫 困惑, 防御 10 -> 20, 减伤 0.85 -> 0.70), and exactly one loot rule — `ItemDropRule.Common(ModContent.ItemType<GuppyShell>(), 9, 1, 1)` (11%).
- `VerdantRods.cs` implements 叶飞棍 end-to-end beside its 54x432 (eight-frame) `VerdantRods.png`: `Main.npcFrameCount = 8`, 免疫 中毒, a neutral `DarkGlimmeringRods`-style `noGravity` hover/circle AI with a sine wobble and velocity-derived rotation, an environmental water-suffocation drain (30-tick interval, clamped at 1 life, server-authored with `netUpdate`) plus an upward escape bias, the design's "偶尔会点水" dip toward a nearby liquid surface, the unguarded 50% `BuffID.Poisoned` for 600 ticks in `OnHitPlayer`, and a deliberately empty, fully commented `ModifyNPCLoot`.
- `03-BIOLOGY.json` / `03-BIOLOGY.md` now record both rows as `code_complete: true` with their `internal_name`s (`...NPCs.GuppyConch` frame_count 1, `...NPCs.VerdantRods` frame_count 8) and their precise drop blockers, while the frozen counts (31 rows, phase3 5, phase4 23, phase7 3, deferred 2, texture_complete 6, design_art 9), the five `phase3_tranche` ids and the two frozen sets are unchanged. The mirror was regenerated and verified cell-by-cell against the JSON.
- `03-DEVIATIONS.md` sections 4/5/7/8 were extended (never restructured) with this plan's conservative defaults, the `OnHitPlayer` poison-locality note, the two rows' drop disposition, the extended localization deferral and the D-21 runtime additions.

## Task Commits

Each task was committed atomically:

1. **Task 1: 格普螺 — shell-up neutral giant snail with its Phase 1 accessory drop** — `5ee052c46` (feat)
2. **Task 2: 叶飞棍 — neutral eight-frame flying rod with water suffocation and 50% poison** — `6fbc21e9f` (feat)
3. **Task 3: Reconcile both matrix rows, mirror and ledger; close the wave gate** — `86a5e7e2d` (docs)

**Plan metadata:** one further commit (`docs(03-02): complete ...`) carrying this SUMMARY, `STATE.md`, `ROADMAP.md`, `state.json` and `WINDOWS.md`; it is the fourth commit in the `plan_head_before..HEAD` range and its hash is recorded in the completion report.

## Files Created/Modified

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs` — `Everglow.Yggdrasil.KelpCurtain.NPCs.GuppyConch`
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs` — `Everglow.Yggdrasil.KelpCurtain.NPCs.VerdantRods`
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.json` — two rows flipped to `code_complete: true` with their `internal_name`, `frame_count` and drop blockers
- `.planning/phases/03-completed-art-ordinary-monsters/03-BIOLOGY.md` — the same two rows regenerated in the mirror
- `.planning/phases/03-completed-art-ordinary-monsters/03-DEVIATIONS.md` — sections 4/5/7/8 extended

## Decisions Made

- **The shell transition lives in `HitEffect`, not `ModifyIncomingHit`.** tML documents `ModifyIncomingHit` as using the hook "ONLY to modify properties of the HitModifiers. Any extra side effects should occur in OnHit hooks instead", and that hook "can be called on the local client or server, depending on who is dealing damage". `HitEffect` is the documented on-hit hook and is "Called on local, server, and remote clients", so it is the correct place for the server-authoritative state change (`Main.netMode != NetmodeID.MultiplayerClient` + `NPC.netUpdate = true`). `ModifyIncomingHit` keeps exactly the two `FinalDamage` scales the plan's acceptance criteria require (0.70 shelled / 0.85 crawling), so the observable behaviour is the plan's: the hit that triggers the retract is still resolved at 0.85 and every following hit lands at 0.70.
- **`NPCSpawnInfo.Player` is the member.** The plan's acceptance text writes `spawnInfo.player`; plan 03-01 already corrected this to the public **field** `spawnInfo.Player` (there is no lowercase `player` member) and this plan follows that correction.
- **Water is rejected, not required, in both spawn gates.** 格普螺 is a land crawler and 叶飞棍 is an open-air flier, so both return `0f` when `spawnInfo.Water` is set; the design's water interaction for 叶飞棍 lives in its AI (skim + suffocate + escape), not in a spawn requirement.
- **The absent drops are comments and matrix blockers, never type references.** 软体甲壳碎片 (格普螺) and 飞棍毛发 / 毒腺 (叶飞棍) have no repository `ModItem`; naming them in a code comment is what the plan's action asks for, and a `ModContent.ItemType<...>()` for any of them would be a compile error (D-37/D-39). No such reference exists in either class.
- **Requirements are advanced, not completed.** This plan advances BIO-01, BIO-03, BIO-06 and QUAL-03, but only four of the five tranche creatures exist, so `REQUIREMENTS.md` is again left untouched and the traceability rows stay Pending until plan 03-04 closes the phase (the plan-03-01 precedent).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2 - Engine contract] The shell-state transition moved from `ModifyIncomingHit` to `HitEffect`**
- **Found during:** Task 1 (authoring `GuppyConch.cs`)
- **Issue:** The plan's `key_links` chain `GuppyConch.ModifyIncomingHit -> shelled state`. Implementing the transition there would violate the engine contract the same engine documents for that hook ("This hook should be used ONLY to modify properties of the HitModifiers. Any extra side effects should occur in OnHit hooks instead.").
- **Fix:** The transition (`EnterShell()`: `NPC.defense = 20`, `NPC.velocity = Vector2.Zero`, `ShellTimer = 120`, `NPC.netUpdate = true`) is invoked from `HitEffect` under `Main.netMode != NetmodeID.MultiplayerClient`; `ModifyIncomingHit` keeps the 0.70/0.85 `FinalDamage` scaling. `HitEffect` runs on the server (and in single player), so the transition is exactly as server-authoritative as the plan's threat model (T-03-10) requires, and the first hit still resolves at 0.85 before the state flips.
- **Files modified:** `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs`
- **Verification:** `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0; the acceptance tokens (`ModifyIncomingHit` with both `0.70f` and `0.85f`) are present; the state change is `netUpdate`-ed under a multiplayer-client guard.
- **Committed in:** `5ee052c46` (Task 1 commit); also recorded in `WINDOWS.md`.

### Plan-text corrections (not code deviations)

**2. The plan's acceptance text writes `IsKelpCurtainLayer(spawnInfo.player)`.** `NPCSpawnInfo` exposes `Player` as a public field and there is no lowercase `player` member, so both classes use `KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player)` — the form plan 03-01 already corrected and the form the execution brief mandated.

**3. The plan's acceptance text says the class files contain "no reference to 软体甲壳碎片 / 飞棍毛发 / 毒腺", while the plan's action requires a comment naming those drops.** Both are satisfied by reading "reference" as a *type reference* (the plan's own prohibition defines the failure as "a type reference to them is a compile error"). Each class names the absent drops only in a comment that states they are not written, and neither contains any `ModContent.ItemType<...>()` for them; the Release build proves there is no such reference.

**Total deviations:** 1 auto-fixed (Rule 2, engine-contract hook placement) plus 2 plan-text corrections that change no behaviour.
**Impact on plan:** None on scope, intent or observable behaviour; the transition still happens on the authoritative side exactly once per retract, with the plan's damage scales intact.

## Issues Encountered

- The Windows broken-windows ledger has no delete verb, so an accidental probe entry (`id 16`, "Placeholder probe", created while checking the `windows append` signature) was marked `fixed` rather than removed; it is not a real defect and it does not affect the open count. The two genuine 03-02 entries were appended afterwards (`unrun-verify` for the D-21 client checks, `deviation` for the `HitEffect` placement).
- `git rev-list --count "$base..HEAD"` returned `0` inside one compound PowerShell command while the same expression returned `3` when run standalone; the standalone value was used for the `commits:` actual.

## User Setup Required

None - no external service configuration required.

## Known Stubs

None. Both classes are full implementations. The only deliberate omissions are design-sourced: 格普螺's `ModifyNPCLoot` omits the absent 软体甲壳碎片 drop with a comment, and 叶飞棍's `ModifyNPCLoot` is empty for the same reason; both are recorded as precise blockers in `03-BIOLOGY.json` and `03-DEVIATIONS.md` §5, which is the D-37/D-39 contract rather than an unfinished stub. No placeholder art, no HJSON key and no placeholder item type was introduced.

## Threat Flags

None. No new network endpoint, auth path, file-access pattern or trust-boundary schema was introduced. Both classes sit inside the existing `NPCSpawnManager`/`SpawnModBiomes` spawn boundary and the existing `KelpCurtainBiome` predicates; the only new state is per-NPC `ai[]`/`localAI[]` wrapped in named properties and synced with `NPC.netUpdate`.

## Verification

- `dotnet build /p:Configuration=Release /p:WarningLevel=0` exits 0 (0 errors, 0 warnings) and packages `Everglow.tmod`, after each of the three tasks.
- `scripts/check-biology.ps1` exits 0 after every task and prints `OK(0): phase3 tranche = 5 / 5 (rows=31)`; after Task 3 it prints `OK: implemented classes = 4 / 5` (`RiverSlug`, `MossyThornTurtle`, `GuppyConch`, `VerdantRods`; only 巨树人 remains `code_complete: false` with an empty `internal_name` for plan 03-03).
- Neither class contains `Main.LocalPlayer`, a `Texture` override or a handwritten asset path. 格普螺's `SpawnChance`/`ModifyIncomingHit`/`ModifyNPCLoot`/`SpecificDebuffImmunity` tokens and the exact `ItemDropRule.Common(ModContent.ItemType<GuppyShell>(), 9, 1, 1)` rule were checked; so were 叶飞棍's `Main.npcFrameCount[NPC.type] = 8`, `NPC.noGravity = true`, `BuffID.Poisoned, 600` **outside** any netmode guard, and its empty loot table.
- `03-BIOLOGY.json` parses with 31 rows, phase3 = 5, texture_complete = 6 and design_art = 9; both edited rows read `code_complete: true` with a resolving `internal_name`. A cell-by-cell JSON/MD parity script reported `PARITY OK` over all 31 rows. `03-BIOLOGY.json`, `03-BIOLOGY.md` and `03-DEVIATIONS.md` all remain UTF-8 without BOM and LF-only (the gate's invariant 13 is green).
- No `.png` or other binary asset was added, moved, renamed or modified (`git status` showed only the three planning files and the two new `.cs` files); no HJSON was edited and the Feishu document was not touched.
- The D-21 client checks for both rows are recorded as **not yet executed** in `03-DEVIATIONS.md` §8 and in `WINDOWS.md`, and are referred forward to plan 03-04's UAT bundle.

## Next Phase Readiness

- Wave 3 (03-03, 巨树人) precondition is satisfied: the matrix now has four `code_complete: true` tranche rows, the gate is green, and `GuppyConch.cs`/`VerdantRods.cs` provide two more local-`AI()` shapes to mirror.
- Plan 03-03 must reuse `KelpCurtainBiome.IsKelpCurtainLayer` (never `IsBiomeActive`) and must keep the class-name-equals-asset-basename rule so the gate's class-resolution invariant stays green; the last row it flips must also fill its `internal_name` in the same edit.
- The matrix's two newly flipped rows carry their drop blockers in the exact shape Phase 4 and Phase 8 will consume, and the `-RequireAll` gate will only be satisfiable once 03-03 lands.

## Self-Check: PASSED

- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/GuppyConch.cs` exists beside the tracked `GuppyConch.png`.
- `Sources/Modules/Yggdrasil/KelpCurtain/NPCs/VerdantRods.cs` exists beside the tracked `VerdantRods.png`.
- `03-BIOLOGY.json` parses with 31 rows, both rows `code_complete: true` and resolving `internal_name`s.
- `03-BIOLOGY.md` has 31 rows and matches the JSON cell-by-cell.
- `03-DEVIATIONS.md` retains its nine sections with sections 4/5/7/8 extended.
- `scripts/check-biology.ps1` exits 0 with `OK(0): phase3 tranche = 5 / 5 (rows=31)` and `OK: implemented classes = 4 / 5`.
- Commits `5ee052c46`, `6fbc21e9f` and `86a5e7e2d` exist on `Yggdrasil/newContent0-ai`.

---
*Phase: 03-completed-art-ordinary-monsters*
*Completed: 2026-09-15*
