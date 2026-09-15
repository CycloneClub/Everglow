---
status: partial
phase: 02-remaining-items-unfinished-art-materials
source: [02-VERIFICATION.md]
started: 2026-09-14T12:35:00Z
updated: 2026-09-14T13:00:00Z
---

## Current Test

[testing paused — 7 items outstanding (blocked on runtime environment)]

## Tests

### 1. Mod load / no missing-resource errors
expected: The mod loads cleanly with no missing-resource/disabled-mod entry; every Phase 2 item resolves against the shared fallback texture.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

### 2. 红月水藻 set equip + effects
expected: Both head pieces are accepted into the head slot (defense 8/11); the full set moves/swims faster than the greaves alone (+24% wet); a single hit of >= 10 damage restores roughly 15% of that hit at full HP.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

### 3. 巨石弹射装置 (BoulderCatapult)
expected: The boulder arcs under gravity, bursts on impact, 3-6 shards fly outward, no ammunition is consumed, tooltip damage reads 44.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

### 4. 肌腱巨弓 + 限制机 + 腥臭的诱饵
expected: Arrows are consumed and the mod arrow (TendonGreatbow_Arrow) fires; boss damage reads ~10% higher than the equivalent non-boss hit; 限制机 consumes 15 mana but produces no drone; 腥臭的诱饵 swings but is not consumed and summons nothing.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

### 5. 灵蛇玉卵 + 竹节步符
expected: 灵蛇玉卵 is Blue rarity / 10 gold and is not consumed while the Phase 7 encounter is absent; 竹节步符 equips into an accessory slot and grants no stats.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

### 6. All 12 shells + 灵蛇玉卵
expected: Each shell appears with a white-box icon, loads with no missing-resource error, and none grants a stat or effect.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

### 7. Consolidated end-of-phase UAT bundle
expected: In one client session the mod loads cleanly; the armor equips and its effects apply; the boulder bursts into 3-6 shards; the greatbow boss damage is ~10% higher; every shell appears with a white-box icon and no effect; no .png was ever required.
result: blocked
blocked_by: other
reason: "由于一些原因现在暂时无法进行实机测试，先跳过需要实机测试的内容，将其延后。"

## Summary

total: 7
passed: 0
issues: 0
pending: 0
skipped: 0
blocked: 7

## Gaps

<!-- None. Blocked tests are environment/prerequisite gates, not code issues; they are not added to Gaps. -->

## Deferred Follow-Ups

- test: 1
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
- test: 2
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
- test: 3
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
- test: 4
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
- test: 5
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
- test: 6
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
- test: 7
  idea: "实机测试（tModLoader 客户端）暂时无法进行；延后至可用时再执行本项 UAT。"
  deferred_at: 2026-09-14
