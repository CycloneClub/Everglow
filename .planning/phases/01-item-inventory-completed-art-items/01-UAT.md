---
status: testing
phase: 01-item-inventory-completed-art-items
source: [01-VERIFICATION.md]
started: 2026-09-12T20:20:17Z
updated: 2026-09-12T20:20:17Z
---

## Current Test

number: 1
name: In-client usability of the 58 completed-art items
expected: |
  Each tested item is obtainable/craftable/equippable and behaves as the design snapshot states,
  or its recorded yellow blocker is confirmed acceptable.
awaiting: user response

## Tests

### 1. In-client usability of the 58 completed-art items
test: In a tModLoader client, obtain/craft/equip/use representative tranche-A and tranche-B items (e.g. DevilHeartHelmet's new Anvil recipe, ThornTurtleShell's `-10%` run speed, RuinMask rarity, a classified placeable) and compare them to the committed evidence rows.
expected: Each item is obtainable/craftable/equippable and behaves as the design states, or its recorded yellow blocker is confirmed acceptable.
why_human: The core criterion ("usable with the documented recipe, value, effect") is a runtime gameplay property offline scripts cannot prove.
result: [pending]

### 2. Both-culture localization display
test: Run the in-game `OutputLocalizationHjsonItem` exporter once, then confirm each completed-art item's localized name/tooltip renders in en-US and zh-Hans.
expected: Exporter reports zero unclassified; all 58 completed-art items show localized text; no pre-existing key is removed.
why_human: Plan-05 backstop truth, explicitly unverified (exporter not run per user directive). 13 entries are recorded as deferred.
result: [pending]

### 3. Effect/set-bonus blocker disposition review
test: Review the 8 recorded blockers (MossySpell, CyatheaArrow, GreenSungloStaff, EvilHalbertBarnacle, Photophore, WitherbarkHelmet, ShellMolluscsBreastPlate, RuinMask).
expected: Each is judged defensible (fix lives in projectiles/buffs/localization outside the modify set) or scheduled.
why_human: Plans 03/04 flag this as a scoping judgment (human_judgment: true).
result: [pending]

### 4. Matrix vs evidence human review
test: Compare `01-INVENTORY.md` (103 rows + label table + deferred section) against `evidence/*.xml`; check every yellow/unchecked reason and the unresolved Green Tundra label.
expected: No placeholder art recorded as complete; unresolved label remains unmerged/blocked.
why_human: Plan-02 task-3 human-check; source-comparison judgment.
result: [pending]

## Summary

total: 4
passed: 0
issues: 0
pending: 4
skipped: 0
blocked: 0

## Gaps
