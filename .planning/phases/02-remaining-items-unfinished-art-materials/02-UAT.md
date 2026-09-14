---
status: testing
phase: 02-remaining-items-unfinished-art-materials
source: [02-VERIFICATION.md]
started: 2026-09-14T12:35:00Z
updated: 2026-09-14T12:35:00Z
---

## Current Test

number: 1
name: Mod load / no missing-resource errors
expected: |
  The mod loads cleanly with no missing-resource/disabled-mod entry; every Phase 2 item resolves against the shared fallback texture.
awaiting: user response

## Tests

### 1. Mod load / no missing-resource errors
expected: The mod loads cleanly with no missing-resource/disabled-mod entry; every Phase 2 item resolves against the shared fallback texture.
result: [pending]

### 2. 红月水藻 set equip + effects
expected: Both head pieces are accepted into the head slot (defense 8/11); the full set moves/swims faster than the greaves alone (+24% wet); a single hit of >= 10 damage restores roughly 15% of that hit at full HP.
result: [pending]

### 3. 巨石弹射装置 (BoulderCatapult)
expected: The boulder arcs under gravity, bursts on impact, 3-6 shards fly outward, no ammunition is consumed, tooltip damage reads 44.
result: [pending]

### 4. 肌腱巨弓 + 限制机 + 腥臭的诱饵
expected: Arrows are consumed and the mod arrow (TendonGreatbow_Arrow) fires; boss damage reads ~10% higher than the equivalent non-boss hit; 限制机 consumes 15 mana but produces no drone; 腥臭的诱饵 swings but is not consumed and summons nothing.
result: [pending]

### 5. 灵蛇玉卵 + 竹节步符
expected: 灵蛇玉卵 is Blue rarity / 10 gold and is not consumed while the Phase 7 encounter is absent; 竹节步符 equips into an accessory slot and grants no stats.
result: [pending]

### 6. All 12 shells + 灵蛇玉卵
expected: Each shell appears with a white-box icon, loads with no missing-resource error, and none grants a stat or effect.
result: [pending]

### 7. Consolidated end-of-phase UAT bundle
expected: In one client session the mod loads cleanly; the armor equips and its effects apply; the boulder bursts into 3-6 shards; the greatbow boss damage is ~10% higher; every shell appears with a white-box icon and no effect; no .png was ever required.
result: [pending]

## Summary

total: 7
passed: 0
issues: 0
pending: 7
skipped: 0
blocked: 0

## Gaps
