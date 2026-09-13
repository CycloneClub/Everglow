# Roadmap: Everglow Kelp Curtain

## Overview

Complete the Kelp Curtain second layer in explicit content-priority order: reconcile the authoritative source inventory and finish item work first, then ordinary monsters, then terrain and generation, then the Klein Snake and Giant Winged Dragon encounters, and finally publishable integration. Within items, monsters, and terrain, entries with complete design artwork/textures are implemented before entries with incomplete artwork/textures. Biology-design drops are item work and are completed in the item phases; monster phases implement behavior and consume those already-classified drops. Undefined future content and explicitly hardmode-deferred entries remain out of scope.

## Design-source acceptance path

Each phase plan maintains an item-level comparison matrix for the biology, item, and terrain documents named in `.planning/PROJECT.md`. Before implementation or acceptance, fetch the relevant Feishu document with **XML and full detail** so block IDs, checkbox state, and formatting are available; verify each item individually against the source. The acceptance status is:

- **Status colour applies only when both the artwork and code checkboxes are complete.** When either checkbox is incomplete the row is **unchecked — no colour**, whatever the repository asset state, and the conflict is recorded as a blocker.
- **Green and checked:** both checkboxes complete, exact design match, acceptance checks passing.
- **Yellow:** both checkboxes complete but a content/description conflict, materially different behavior, or known exception exists, with a concise explanation retained.
- **Unchecked (no colour):** artwork and/or code incomplete, or implementation blocked/not done, with the dependency/blocker recorded in planning artifacts.

Phase 1 owns the source/inventory reconciliation record for the label discrepancy: Biology names Death Jade Lake, Spiny Moss Court, and Valley of Lush and Moist, while Terrain also names Green Tundra and Town of Decaying Wood. The record must classify each label as a region, nested area, structure, transition, or alias before affected work is accepted. This reconciliation is an inventory prerequisite, not a reason to make region geography the first implementation priority. If a label cannot be resolved, its affected item stays unchecked (no colour)/blocked rather than being silently folded into another region. Phase 8 performs the final per-item Feishu status synchronization and audit.

## Phases

- [ ] **Phase 1: Item Inventory & Completed-Art Items** - Reconcile source labels and inventory every item and biology-design drop, then implement entries with complete design textures. (reopened 2026-09-12 — 18-entry completed-art class-less carry-over; original tranche complete 2026-09-12)
- [ ] **Phase 2: Remaining Items & Unfinished-Art Materials** - Complete item entries without finished design textures, including unfinished-art drops/materials, using Phase 1 dependencies.
- [ ] **Phase 3: Completed-Art Ordinary Monsters** - Implement ordinary creature behavior for entries whose design textures are complete; their drops are already item work.
- [ ] **Phase 4: Remaining Ordinary Monsters** - Implement ordinary creatures without complete design textures and their behavior after the completed-art tranche.
- [ ] **Phase 5: Completed-Art Terrain & Structures** - Implement terrain and structures whose design textures are complete.
- [ ] **Phase 6: Remaining Terrain, Generation & Integration** - Complete unfinished-art terrain and integrate generation, traversal, hazards, and progression systems.
- [ ] **Phase 7: Bosses, Special Encounters & Rewards** - Implement Klein Snake, Giant Winged Dragon, and their reward chains last among content.
- [ ] **Phase 8: Publishable Integration & Source Acceptance** - Verify build, client/server/multiplayer behavior, persistence, localization, and per-item Feishu status.

## Phase Details

### Phase 1: Item Inventory & Completed-Art Items

**Goal:** The complete item inventory is reconciled against the authoritative designs, and every item or biology-design drop with finished design artwork is usable without introducing placeholder assets.
**Mode:** mvp
**Depends on:** Nothing (first phase)
**Requirements:** QUAL-05
**Scope anchor:** Primary item requirements are assigned to Phase 2; this phase delivers their completed-art tranche and the source/dependency record needed by Phase 2.
**Success Criteria** (what must be TRUE):

  1. The inventory identifies every in-scope item, material, and biology-design drop, records whether its design artwork/texture is complete, and records dependencies, missing assets, hardmode-deferred entries, and undefined future entries.
  2. The biology/terrain source-label discrepancy is classified in an auditable record before geography-specific implementation is accepted.
  3. Players can obtain, craft, equip, or use every unblocked item and completed-art biology drop in this tranche with the documented recipe, value, effect, and localization behavior; no placeholder art is introduced.
  4. Every completed-art item accepted in this phase has an XML-full-fetch comparison record; both-checkbox-complete exact matches are eligible for green/check status, both-checkbox-complete conflicts or known exceptions are yellow with reasons, and entries with an incomplete checkbox or a blocker remain unchecked (no colour) with reasons.

**Verification needs:** XML `--detail full` fetches and per-item matrix; source-label taxonomy; item recipe/effect/drop checks; asset/localization convention review; `dotnet build`; blocker and dependency audit.
**Plans:** 7 plans (6 executed; 1 gap-closure plan 01-07)
Plans:

- [x] 01-06-PLAN.md
- [ ] 01-07-PLAN.md — Gap closure: ArmOfGiantTree per-player charge + server-authoritative shockwave (SC3 / CR-01, WR-01, WR-02)

**Wave 1**

- [x] 01-01-PLAN.md — Inventory pipeline tracer: committed XML evidence + one-category inventory + validator

**Wave 2** *(blocked on Wave 1 completion)*

- [x] 01-02-PLAN.md — Full inventory reconciliation, five-label taxonomy, deferred/assumption record

**Wave 3** *(blocked on Wave 2 completion)*

- [x] 01-03-PLAN.md — Completed-art tranche A parity (weapons, ammo, materials, accessories) + class-less Phase 2 routing + coverage gate

**Wave 4** *(blocked on Wave 3 completion)*

- [x] 01-04-PLAN.md — Completed-art tranche B parity (armor, placeables, pets, boosters) + placeable categories + coverage gate

**Wave 5** *(blocked on Wave 4 completion)*

- [x] 01-05-PLAN.md — Localization parity, deviation ledger, final Phase 1 gate

**Cross-cutting constraints:**

- `dotnet build /p:Configuration=Release /p:WarningLevel=0` passes.
- No `.png` or other binary asset is added or modified.
- **Entry allocation is by design-artwork state only.** Every entry whose Feishu texture/artwork checkbox is complete belongs to the completed-art item tranche (Phase 1); every remaining entry belongs to Phase 2. Whether the repository already contains an implementation class for an entry does **not** affect its allocation — a completed-art entry with no existing class is still Phase 1 work, and class-less entries are never deferred to a later phase on that basis.
- **2026-09-12 correction (carry-over):** the earlier rule ("class-less → route to Phase 2") mis-allocated 18 entries that are artwork-complete but had no repository class. Allocation is two-step: (1) design section → phase (non-boss item tables stay in the item phases; the Giant Winged Dragon and Klein Snake sections are Phase 7 via ITEM-06/ITEM-05); (2) within the item phases, Feishu artwork state → Phase 1 (complete) or Phase 2 (incomplete). Applying that rule, **5 of the 18 are Phase 1 carry-over** (implemented in plan 01-06) and **13 are Phase 7 encounter rewards**; resulting counts are `{1: 65, 2: 25, 7: 13}`. See `.planning/phases/01-item-inventory-completed-art-items/01-INVENTORY.json` (`phase1_actions[]` P1A-12) and `01-DEVIATIONS.md` §Allocation Correction.

### Phase 2: Remaining Items & Unfinished-Art Materials

**Goal:** The remaining non-boss item scope is implemented in dependency order, including entries and biology-design drops/materials whose design artwork or textures were not complete during Phase 1.
**Mode:** mvp
**Depends on:** Phase 1 inventory, source reconciliation, and dependency ledger
**Allocation note:** Phase 2 contains only entries whose Feishu design artwork/texture checkbox is incomplete. Existing repository classes are irrelevant to allocation; of the former 18 completed-art class-less entries, the 5 non-boss item-table rows are Phase 1 carry-over and the 13 boss/special-encounter rows are Phase 7, so none are Phase 2 scope.
**Requirements:** ITEM-01, ITEM-02, ITEM-03, ITEM-04
**Success Criteria** (what must be TRUE):

  1. Players can obtain, craft, equip, or use every unblocked remaining natural weapon, ammunition, material, utility item, armor/accessory, exploration reward, collection reward, quest reward, trade reward, restoration reward, and associated biology-design drop in the documented progression.
  2. Unfinished-art drops and materials are counted in item implementation and are not deferred to the monster phases; entries blocked by unavailable approved art remain explicitly unchecked rather than receiving placeholder assets.
  3. Recipes, values, effects, set effects, access conditions, and progression dependencies discovered in Phase 1 work together without bypassing the intended item progression.
  4. Each accepted remaining item has an individual XML-full-fetch comparison and an evidence record that supports green for both-checkbox-complete exact completion, yellow for both-checkbox-complete conflicts or known exceptions, and unchecked (no colour) for work with an incomplete checkbox or a blocker.

**Verification needs:** Phase 1 dependency ledger; per-item source matrix; craft/loot/quest/trade/restoration checks; both localization targets for available items; `dotnet build`; blocker review.
**Plans:** TBD

### Phase 3: Completed-Art Ordinary Monsters

**Goal:** Ordinary creatures with complete design textures are playable across the designed Kelp Curtain contexts, while their drops are supplied by the completed item work rather than deferred into creature implementation.
**Mode:** mvp
**Depends on:** Phase 2
**Requirements:** No additional v1 IDs; completed-art tranche of BIO-01, BIO-02, and BIO-03 is the primary dependency for Phase 4.
**Success Criteria** (what must be TRUE):

  1. Every unblocked ordinary creature whose design texture is complete spawns in its intended context and exhibits the documented water/land behavior, hostility, status effects, capture rules, combat states, variants, or neutral behavior.
  2. Players can encounter these creatures and receive the already-implemented documented drops; no biology drop is first introduced or left unfinished in this monster phase.
  3. Completed-art creature entries are individually compared to XML-full-fetch source records, with exact completion, partial/conflict, and blocked states visible for the later remaining-creature tranche.

**Verification needs:** Completed-art biology matrix; client spawn/combat/capture checks; drop-source checks against Phases 1–2; dedicated-server context checks; `dotnet build`.
**Plans:** TBD

### Phase 4: Remaining Ordinary Monsters

**Goal:** Ordinary creatures without complete design textures are implemented after the completed-art tranche, with all ordinary-creature behavior integrated against the item drops already classified and implemented in Phases 1–2.
**Mode:** mvp
**Depends on:** Phase 3
**Requirements:** BIO-01, BIO-02, BIO-03
**Success Criteria** (what must be TRUE):

  1. The remaining aquatic, amphibious, surface-water, passive, predator, Witherbark, command, giant-tree, Brody dragonfly, valley, disguised-hazard, neutral, and mini-boss entries that are unblocked by approved assets spawn and behave according to their designs.
  2. Ordinary-creature behavior, spawn weights, hostility, environmental interaction, variants, and status effects do not leak into the main world or bypass the documented item/progression dependencies.
  3. Associated drops are obtainable from the item implementations in Phases 1–2, including unfinished-art drops/materials; monster completion does not become a substitute for item completion.
  4. Every ordinary-creature entry has an individual source comparison and an explicit exact/partial-conflicting/blocked result; hardmode-deferred and undefined future entries remain out of scope.

**Verification needs:** Remaining biology matrix; client spawn/behavior/combat checks; drop and progression integration checks; dedicated-server safety; `dotnet build`; unresolved-source audit.
**Plans:** TBD

### Phase 5: Completed-Art Terrain & Structures

**Goal:** Terrain and structures with complete design textures are implemented as usable, traversable building blocks for the later integrated Kelp Curtain generation.
**Mode:** mvp
**Depends on:** Phase 4
**Requirements:** No additional v1 IDs; completed-art tranche of TERR-01 through TERR-07 is the primary dependency for Phase 6.
**Success Criteria** (what must be TRUE):

  1. Every unblocked completed-art block, wall, plant, furniture piece, chest, decorative element, and structure can be placed or generated with its documented mining and interaction rules.
  2. Completed-art traversal structures, hazards, ruins, caves, underwater cells, and safe-area elements are reachable in isolation without requiring unfinished-art replacements.
  3. Terrain entries are individually checked against XML-full-fetch source records, and missing/conflicting art or source ambiguity remains visible for Phase 6.

**Verification needs:** Completed-art terrain matrix; tile interaction and structure reachability checks; resource-packing review; dedicated-server generation-context check; `dotnet build`.
**Plans:** TBD

### Phase 6: Remaining Terrain, Generation & Integration

**Goal:** The remaining terrain and structures are completed and the full Kelp Curtain layer generates, connects, and supports its designed traversal, hazards, restoration, and exploration-to-reward progression.
**Mode:** mvp
**Depends on:** Phase 5 and the completed item/ordinary-monster tranches
**Requirements:** TERR-01, TERR-02, TERR-03, TERR-04, TERR-05, TERR-06, TERR-07, GAME-01, GAME-02, GAME-03
**Success Criteria** (what must be TRUE):

  1. Players can enter and leave Yggdrasil with the Kelp Curtain generated in the intended vertical progression and without corrupting the main world; Death Jade Lake, Isle of Bloom, Spiny Moss Court, Town of Decaying Wood, Wilted Zones, Valley of Lush and Moist, Green Tundra, and nested/transition labels follow the Phase 1 reconciliation.
  2. Remaining blocks, walls, plants, furniture, chests, caves, ruins, traps, traversal structures, underwater cells, boat/oxygen routes, treasury/maze access, hazards, settlement functions, and restoration spaces work with the documented rules and remain explorable/recoverable.
  3. The exploration-to-reward loop connects resources, structures, combat, chests, fishing, purification, and progression gates without soft-locking or allowing unintended order bypasses.
  4. Wilted Zones spread, spawn intended enemies, accept the intended purification item, restore terrain, and increment non-repeatable progression exactly once per eligible restoration.
  5. Generation uses Yggdrasil context and resource boundaries, has an acceptable measured generation budget, and performs no client-only work on a dedicated server.

**Verification needs:** Remaining terrain XML matrix; clean client generation and return-flow checks; region/traversal/hazard/restoration/progression playthrough; generation timing; dedicated-server load; `dotnet build`.
**Plans:** TBD

### Phase 7: Bosses, Special Encounters & Rewards

**Goal:** After all item, ordinary-monster, and terrain work is available, players can complete the Klein Snake and Giant Winged Dragon encounters and receive their documented reward chains.
**Mode:** mvp
**Depends on:** Phase 6
**Requirements:** BIO-04, BIO-05, ITEM-05, ITEM-06, GAME-04
**Carry-over note (2026-09-12):** the 13 artwork-complete class-less boss/special-encounter rows are Phase 7 item scope: 9 Giant Winged Dragon rows (tail-kill drops, relic/medal, craftable equipment) advance ITEM-06 and 4 Klein Snake series rows advance ITEM-05. They were mis-routed to Phase 2 and are recorded in `01-DEVIATIONS.md` §Allocation Correction / `01-INVENTORY.json` P1A-12.
**Success Criteria** (what must be TRUE):

  1. The Klein Snake arena, trigger, phases, organs, healing projectiles, minions, attacks, transitions, defeat sequence, parasite/seed loop, debuff, and class coverage behave as documented.
  2. The blood mica trigger starts the correct Giant Winged Dragon encounter; protection, phases, tail destruction, tail-kill transition, failure, retry, and completion rules cannot be skipped or falsely satisfied.
  3. Klein Snake and Giant Winged Dragon drops, tail-kill rewards, recipes, craftable equipment, boss-access progression, missions, purchases, and restoration-stage dependencies are granted only under documented conditions.
  4. A player can reach and complete both encounters after the preceding item, ordinary-monster, and terrain prerequisites without receiving rewards from an interrupted or invalid encounter.

**Verification needs:** Repeated client boss/encounter runs; phase, projectile, minion, organ, protection, tail, failure, and retry checks; reward/crafting/source matrix checks; multiplayer cases carried into Phase 8; `dotnet build`.
**Plans:** TBD

### Phase 8: Publishable Integration & Source Acceptance

**Goal:** The Kelp Curtain is a publishable, localized, persistent, network-safe layer whose implementation and every design item are reconciled against the authoritative Feishu documents.
**Mode:** mvp
**Depends on:** Phase 7
**Requirements:** BIO-06, ITEM-07, GAME-05, QUAL-01, QUAL-02, QUAL-03, QUAL-04
**Success Criteria** (what must be TRUE):

  1. Creature behavior, environmental interaction, status effects, spawn weights, and combat difficulty match the three designs or have explicit documented exceptions, with no unintended main-world behavior.
  2. Every implemented item has complete `en-US` and `zh-Hans` localization and follows generated asset/localization conventions; missing approved art is not hidden by placeholders.
  3. Player state, world state, subworld transitions, permanent rewards, NPC/projectile state, quest state, and network state remain consistent across save/load, exit/re-entry, dedicated-server, and multiplayer scenarios.
  4. The repository `dotnet build` workflow passes without compiler, resource-packing, or shader errors, and client verification covers entry, generation, exploration, rendering, combat, progression, localization, and reward acquisition.
  5. Every design item is individually synchronized from XML-full-fetch evidence: both-checkbox-complete exact completion is checked/green, both-checkbox-complete conflicts or known exceptions are yellow with a concise explanation, and work with an incomplete checkbox or otherwise blocked remains unchecked (no colour) with its blocker recorded.

**Verification needs:** Clean build and relevant tests; full client regression; dedicated-server and multiplayer verification for changed networked/persistent/subworld/NPC/projectile/quest/reward paths; final per-item Feishu status writes; planning evidence and blocker audit.
**Plans:** TBD

## Progress

**Execution Order:**
Phases execute in numeric order: 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8. This order is intentional: items, then ordinary monsters, then terrain, then bosses, then final integration.

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Item Inventory & Completed-Art Items | 6/7 | In Progress|  |
| 2. Remaining Items & Unfinished-Art Materials | 0/TBD | Not started | - |
| 3. Completed-Art Ordinary Monsters | 0/TBD | Not started | - |
| 4. Remaining Ordinary Monsters | 0/TBD | Not started | - |
| 5. Completed-Art Terrain & Structures | 0/TBD | Not started | - |
| 6. Remaining Terrain, Generation & Integration | 0/TBD | Not started | - |
| 7. Bosses, Special Encounters & Rewards | 0/TBD | Not started | - |
| 8. Publishable Integration & Source Acceptance | 0/TBD | Not started | - |
