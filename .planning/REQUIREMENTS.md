# Requirements: Everglow Kelp Curtain

**Defined:** 2026-09-11
**Core Value:** Deliver a complete, publishable Kelp Curtain layer whose designed regions, gameplay loop, creatures, terrain, items, bosses, encounters, and rewards work together as a coherent Terraria experience.

## v1 Requirements

Requirements for the current second-layer milestone. Each maps to one roadmap phase.

### Terrain and World Generation

- [ ] **TERR-01**: The Yggdrasil Subworld generates the Kelp Curtain layer with the terrain-design regions, transitions, and intended vertical progression without corrupting the main world.
- [ ] **TERR-02**: The layer provides the planned Kelp Curtain, common Yggdrasil, lake, cave, and regional blocks, walls, plants, furniture, chests, and decorative terrain with the designed mining and interaction rules.
- [ ] **TERR-03**: Low-altitude caves, ascended caves, underwater cells, ruins, traps, chests, and traversal structures generate according to the terrain design and remain explorable and recoverable after world generation.
- [ ] **TERR-04**: Death Jade Lake provides the planned surface, underwater, Dragon Pond, Isle of Bloom, and underwater-maze geography, including boat travel, oxygen traversal, underwater treasury access, and environmental hazards.
- [ ] **TERR-05**: Isle of Bloom provides the planned settlement, restoration, progression hub, treasure, array-repair, and second-layer quest access functions.
- [ ] **TERR-06**: Spiny Moss Court, Town of Decaying Wood, Wilted Zones, and Valley of Lush and Moist provide their planned structures, safe areas, environmental hazards, purification spaces, and traversal features.
- [ ] **TERR-07**: World generation and region-specific systems use the established Yggdrasil context, asset-packing, and runtime boundaries, with acceptable generation time and no client-only work on dedicated servers.

### Creatures and Encounters

- [ ] **BIO-01**: Death Jade Lake implements the designed aquatic, amphibious, surface-water, passive, and predator creatures with their water/land behavior, spawning rules, hostility relationships, status effects, capture rules, and drops.
- [ ] **BIO-02**: Spiny Moss Court implements the designed Witherbark soldiers, command unit, giant tree, Brody dragonfly, and associated combat states, morale behavior, variants, and drops.
- [ ] **BIO-03**: Valley of Lush and Moist implements the designed common creatures, disguised hazards, neutral creatures, mini-boss encounter, and related drops and behavior.
- [ ] **BIO-04**: The Klein Snake / Kelp Curtain final boss implements the documented arena, phases, organs, healing projectiles, minions, attacks, transitions, defeat sequence, and reward progression.
- [ ] **BIO-05**: The Giant Winged Dragon special encounter implements the documented blood mica trigger, regional encounter behavior, phases, tail destruction flow, tail-kill phase, protection interaction, and rewards.
- [ ] **BIO-06**: Creature behavior, environmental interaction, status effects, spawn weights, and combat difficulty are consistent with the three source design documents and do not introduce unintended main-world behavior.

### Items, Materials, and Rewards

- [ ] **ITEM-01**: The planned natural-themed weapons, ammunition, materials, and utility items are implemented with the documented recipes, values, effects, and localization.
- [ ] **ITEM-02**: The Devil Heart Iron, Witherbark, Molluscs, and other planned armor/accessory sets are implemented with documented class effects, set effects, materials, and progression placement.
- [ ] **ITEM-03**: Underwater treasury, underwater maze, regional chest, fishing, and collection rewards are implemented with the documented loot sources, access conditions, and effects.
- [ ] **ITEM-04**: Quest-exclusive, NPC-trade, settlement-restoration, purification, and mission rewards are implemented with the documented unlock conditions and progression dependencies.
- [ ] **ITEM-05**: The Klein Snake series and its parasite/seed gameplay loop are implemented with the documented weapon behavior, debuff, drops, and class coverage.
- [ ] **ITEM-06**: Giant Winged Dragon rewards and craftable equipment are implemented with the documented drop conditions, special tail-kill rewards, recipes, and encounter progression.
- [ ] **ITEM-07**: Each implemented item is localized in both supported languages and uses the repository's generated asset and localization conventions.

### Progression and Gameplay Loops

- [ ] **GAME-01**: Players can enter, navigate, and leave the second layer through the intended challenge, boat, shrine, settlement, and teleport flows without soft-locking.
- [ ] **GAME-02**: The second layer provides the designed exploration-to-reward loop across its regions, including structures, hazards, chests, resources, combat, and progression gates.
- [ ] **GAME-03**: Wilted Zones can spread, spawn their intended enemies, be purified with the intended item, restore terrain, and increment non-repeatable purification progression.
- [ ] **GAME-04**: Settlement NPCs, missions, quest progression, boss access, reward purchases, and restoration stages expose the intended second-layer progression in the correct order.
- [ ] **GAME-05**: Persistent player state, world state, subworld transitions, and network state remain consistent when players use second-layer progression, permanent rewards, and multiplayer interactions.

### Quality and Design Traceability

- [ ] **QUAL-01**: The project builds successfully with the repository-required `dotnet build` workflow and does not introduce compiler, resource-packing, or shader errors.
- [ ] **QUAL-02**: The second layer is verified in a tModLoader client for world entry, generation, exploration, representative combat, progression, rendering, assets, localization, and reward acquisition.
- [ ] **QUAL-03**: Multiplayer behavior is verified for every changed networked, persistent, subworld, NPC, projectile, quest, and reward system; dedicated-server safety is checked where applicable.
- [ ] **QUAL-04**: Every design item is individually compared with the implementation after verification: matching completed items are checked and marked green in Feishu, while partial or conflicting items are marked yellow with a concise explanation.
- [ ] **QUAL-05**: Repository planning artifacts record unresolved blockers, known deviations, verification evidence, and newly discovered scope without silently changing the Feishu design source.

## v2 Requirements

Deferred to future releases or a later design decision. These are tracked but not part of the current roadmap unless explicitly promoted.

### Deferred Hardmode Content

- **V2-HARD-01**: Implement the design entries explicitly marked for later hardmode use, including the Withered Seed and Witherbark Guard content.

### Future Design Expansion

- **V2-FUT-01**: Add new Yggdrasil layers or content plans that are not yet defined by the current design documents.
- **V2-FUT-02**: Expand the second layer with later design additions that are created after this milestone is scoped.

## Out of Scope

| Feature | Reason |
|---------|--------|
| Unspecified future content | The user explicitly wants the right to add new plans later; undefined work must not be guessed into this roadmap. |
| Explicitly deferred hardmode entries | The source design marks these for a later difficulty and release scope. |
| Unrelated module rewrites | The milestone is content completion, not broad architecture replacement. |
| Placeholder art or replacement binary assets | Repository rules prohibit placeholders and protect existing art/design inputs. |

## Traceability

Updated during roadmap creation. Each v1 requirement must map to exactly one phase.

| Requirement | Phase | Status |
|-------------|-------|--------|
| TERR-01 through TERR-07 | Pending | Pending |
| BIO-01 through BIO-06 | Pending | Pending |
| ITEM-01 through ITEM-07 | Pending | Pending |
| GAME-01 through GAME-05 | Pending | Pending |
| QUAL-01 through QUAL-05 | Pending | Pending |

**Coverage:**
- v1 requirements: 30 total
- Mapped to phases: 0
- Unmapped: 30 ⚠️

---
*Requirements defined: 2026-09-11*
*Last updated: 2026-09-11 after initial project definition*
