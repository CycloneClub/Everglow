---
status: partial
phase: 01-item-inventory-completed-art-items
source: [01-VERIFICATION.md]
started: 2026-09-12T20:20:17Z
updated: 2026-09-13T09:10:00Z
---

## Current Test

[testing complete]

## Tests

### 1. In-client usability of the 58 completed-art items
test: In a tModLoader client, obtain/craft/equip/use representative tranche-A and tranche-B items (e.g. DevilHeartHelmet's new Anvil recipe, ThornTurtleShell's `-10%` run speed, RuinMask rarity, a classified placeable) and compare them to the committed evidence rows.
expected: Each item is obtainable/craftable/equippable and behaves as the design states, or its recorded yellow blocker is confirmed acceptable.
why_human: The core criterion ("usable with the documented recipe, value, effect") is a runtime gameplay property offline scripts cannot prove.
result: pass

### 2. Both-culture localization display
test: Run the in-game `OutputLocalizationHjsonItem` exporter once, then confirm each completed-art item's localized name/tooltip renders in en-US and zh-Hans.
expected: Exporter reports zero unclassified; all 58 completed-art items show localized text; no pre-existing key is removed.
why_human: Plan-05 backstop truth, explicitly unverified (exporter not run per user directive). 18 entries are recorded as deferred.
result: pass

### 3. Effect/set-bonus blocker disposition review
test: Review the 8 recorded blockers (MossySpell, CyatheaArrow, GreenSungloStaff, EvilHalbertBarnacle, Photophore, WitherbarkHelmet, ShellMolluscsBreastPlate, RuinMask) plus the 3 carry-over artwork blockers and the ElftigernPowder/QuetzalsWish effect blockers.
expected: Each is judged defensible (fix lives in projectiles/buffs/localization outside the modify set) or scheduled.
why_human: Plans 03/04 and 01-06/07 flag this as a scoping judgment (human_judgment: true).
result: pass
note: "User directive (2026-09-12): where code anywhere conflicts with the design doc, and the Feishu original row is marked green (both artwork and code checkboxes complete), follow the code. Applied to the 8 both-complete effect/set-bonus blockers — accepted as-is; not gaps."

### 4. Matrix vs evidence human review
test: Compare `01-INVENTORY.md` (103 rows + label table + deferred section) against `evidence/*.xml`; check every yellow/unchecked reason and the unresolved Green Tundra label.
expected: No placeholder art recorded as complete; unresolved label remains unmerged/blocked.
why_human: Plan-02 task-3 human-check; source-comparison judgment.
result: pass

### 5. ArmOfGiantTree per-stack/per-player charge and server-authoritative shockwave
test: In a tModLoader client (singleplayer and a 2-client multiplayer session), exercise `ArmOfGiantTree`: hold one stack and charge, switch to a second same-type stack (must start uncharged), have two players charge simultaneously (no shared charge), then release at full charge and confirm the area shockwave is applied server-side and propagates to clients via `npc.netUpdate`. Optionally obtain/use the five Phase 1 carry-over items and confirm their recorded dispositions.
expected: Charge is isolated per stack and per player; the full-charge smash delivers 200% charged hit + 100% shockwave with the retract lock; shockwave damage is authoritative and replicates.
why_human: Three plan-01-07 behavior-unverified truths plus the plan-01-06 backstop truth — no offline script can prove multiplayer runtime behavior.
result: blocked
blocked_by: other
reason: "User (2026-09-13): live client testing not convenient right now — temporarily skip this verification, do it later. Needs a running tModLoader client (singleplayer + 2-client multiplayer)."

### 6. Advisory code-review disposition (WR-01/WR-02)
test: Review the two new advisory findings in `01-REVIEW.md`: WR-01 (the `ReleaseSmash` handler does not require `Charge >= MaxChargeFrames`) and WR-02 (`ArmOfGiantTreeChargedSlot` is not synced, so the replicated charge has no durable authoritative consumer).
expected: Either accept them as known client-authoritative limitations (charge accumulation is inherently client-driven) with anti-cheat hardening scheduled to Phase 8 (QUAL-03), or create a follow-up plan.
why_human: Scoping/risk judgment; the proposed one-line Charge gate is itself bypassable because the charge value is client-supplied, so a durable fix needs server-side charge tracking (out of plan 01-07 scope).
result: pass
note: "User (2026-09-13): accepted as a known client-authoritative limitation — normal multiplayer is unaffected (the shockwave is computed from player.HeldItem, not the replicated charge); anti-cheat hardening is out of scope for a sandbox mod and may be scheduled to Phase 8 (QUAL-03)."

## Summary

total: 6
passed: 5
issues: 0
pending: 0
skipped: 0
blocked: 1

## Gaps
