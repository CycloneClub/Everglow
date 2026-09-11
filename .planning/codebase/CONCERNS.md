<!-- refreshed: 2026-09-11 -->
# Codebase Concerns

**Analysis Date:** 2026-09-11

Findings below are evidence-backed observations from the current source and build configuration. Entries explicitly marked **Not detected** mean the scan found no repository evidence; they are not proof that the issue cannot exist at runtime.

## Tech Debt

**Unimplemented runtime contracts:**
- Issue: Several public or discoverable implementations still throw `NotImplementedException`, return no-op values, or expose incomplete behavior. This includes quest reset, server-side VFX fallback, SDF collider methods, and Spine texture/rendering branches.
- Files: `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestManager.cs:262-265`, `Sources/Everglow.Function/VFX/FakeManager.cs:5-29`, `Sources/Everglow.Function/Physics/Colliders/CircleCollider2D.cs:56`, `Sources/Everglow.Function/Physics/Colliders/EdgeCollider2D.cs:62`, `Sources/Everglow.Function/Skeleton2D/Reader/XnaTextureLoader.cs:46-55`, `Sources/Everglow.Function/Skeleton2D/Renderer/SkeletonRenderer.cs:400-408`.
- Impact: A newly reachable content path can fail at runtime instead of degrading safely; the reflection-based registration in `Sources/Everglow/Everglow.cs:75-86` makes accidental reachability easier to introduce.
- Fix approach: Define supported/unsupported contracts explicitly, remove incomplete types from discovery with `ModuleHideTypeAttribute` where appropriate, and add focused tests for every supported fallback.

**Disabled or abandoned gameplay paths:**
- Issue: Active world-generation and quest paths contain permanent-looking early returns or commented-out behavior rather than feature flags or isolated prototypes.
- Files: `Sources/Modules/Minortopography/GreatTombLand/GreatTombLandGenPass.cs:57-65`, `Sources/Modules/Yggdrasil/WorldGeneration/KelpCurtainGeneration.cs:3609-3610`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Abstractions/WorldQuestBase.Behavior.cs:173-191`, `Sources/Everglow.Function/Templates/Weapons/Slingshots/SlingshotProjectile.cs:53-54`.
- Impact: Content appears registered and enabled while key generation, multiplayer retry, or weapon synchronization behavior is missing; unfinished code can be mistaken for a supported contract.
- Fix approach: Gate unfinished behavior with `EverglowConfig`/compile-time flags, or remove the dead path until it has a tested implementation.

**Build configuration has two sources of truth:**
- Issue: CI rewrites `Sources/Directory.Build.props` inline instead of building the checked-in file, duplicating the active module list and build settings.
- Files: `Sources/Directory.Build.props:3-32`, `.github/workflows/build-and-test.yml:47-84`.
- Impact: Local and CI builds can compile different module sets or resource rules; a future property change can silently be lost in CI.
- Fix approach: Make CI consume the repository property file and validate that the checked-in module list is the one used for restore/build.

## Known Bugs

**AABB collider state can disagree with its reported bounds:**
- Symptoms: `AABBCollider2D` stores geometry in `aabb` in its constructors, but `Contains`, `GetPolygon`, and `GetSDFWithGradient` use separate `Center`/`Size` properties. Conversely, the tile-SDF factory initializes `Center`/`Size` while leaving `aabb` at its default value.
- Files: `Sources/Everglow.Function/Physics/Colliders/AABBCollider2D.cs:13-39`, `Sources/Everglow.Function/Physics/Colliders/AABBCollider2D.cs:60-69`, `Sources/Everglow.Function/Utilities/SDFUtils.cs:83-115`.
- Trigger: Construct the collider through either constructor or the object-initializer pattern and then call a method that uses the other representation.
- Workaround: Use one representation consistently and add regression tests before using `AABBCollider2D` in collision or SDF code.

**Multiplayer world-quest retry is unavailable for the full quest:**
- Symptoms: `WorldQuestBase.Retry()` returns without action on a main server or client, and the presentation adapter only advertises `Retry` in single-player.
- Files: `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Abstractions/WorldQuestBase.Behavior.cs:170-207`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:23-36`.
- Trigger: Fail a retriable world quest in multiplayer and open its quest actions.
- Workaround: Only objective retry has a server request path through `ObjectiveRetryRequestPacket`; full quest retry requires single-player.

**Render-target resources leak across resolution changes:**
- Symptoms: `MEACManager.CreateRender` replaces `bloomTarget1` and `bloomTarget2` without disposing the old targets; only the latest pair is disposed during unload.
- Files: `Sources/Everglow.Function/MEAC/MEACManager.cs:21-28`, `Sources/Everglow.Function/MEAC/MEACManager.cs:32-40`.
- Trigger: Change resolution repeatedly while the MEAC module is loaded.
- Workaround: Restart the client/mod; the safe fix is to dispose or return the previous targets before allocating replacements.

**Room/map loading has weak malformed-data handling:**
- Symptoms: Map counts, dimensions, chest slots, and type-map entries are consumed without bounds or availability checks; `ReadChest` can index `-1` when no chest slot remains and unknown mod content is indexed directly from `typeMaping`.
- Files: `Sources/Everglow.Function/TileHelper/MapIO.cs:49-73`, `Sources/Everglow.Function/TileHelper/MapIO.cs:154-169`, `Sources/Everglow.Function/TileHelper/MapIO.cs:586-618`, `Sources/Everglow.Function/TileHelper/MapIO.cs:686-716`.
- Trigger: Load a corrupted room `.mapio` file or a file containing content from an unavailable/changed mod version.
- Workaround: Delete the affected room data under the Terraria mod-data directory; the loader should instead validate and reject the file without mutating world state.

## Security Considerations

**Client-controlled quest progress is accepted by the main server:**
- Risk: A multiplayer client can submit arbitrary delta values for objectives that are intentionally calculated on the client. The main server accepts `MainServer` packets from client slots, then `ReceiveDelta` mutates authoritative progress without validating the sender's actual action, magnitude, or sign.
- Files: `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs:123-139`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Packets/ObjectiveDeltaSyncPacket_SubProgress.cs:23-43`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Objectives/WorldExploreObjective.cs:54-57`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Objectives/WorldExploreObjective.cs:95-105`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Objectives/WorldConsumeItemObjective.cs:74-78`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Objectives/WorldConsumeItemObjective.cs:108-123`.
- Current mitigation: The packet checks quest state, objective range, `CanProgress`, and active-objective membership in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Packets/ObjectiveDeltaSyncPacket_SubProgress.cs:27-39`.
- Recommendations: Keep authoritative counters on the server where possible; otherwise bind deltas to the transport sender, cap them to server-observed activity, reject negative/non-finite values, and rate-limit requests.

**Malformed network frames can crash or allocate excessively:**
- Risk: Packet headers accept unvalidated packet IDs and payload lengths; `Resolve` reads `header.PayloadLength` bytes and dispatches packet-specific counts/indices without a frame-size ceiling or a top-level exception boundary.
- Files: `Sources/Everglow.Function/Netcode/PacketResolver.Serialize.cs:47-55`, `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs:27-85`, `Sources/Everglow.Function/Netcode/Packets/CooldownFullUpdatePacket.cs:21-35`, `Sources/Everglow.Function/Netcode/Packets/CooldownRemovalPacket.cs:19-27`, `Sources/Everglow.Function/Netcode/Packets/ElementalBuildUpPacket.cs:24-57`.
- Current mitigation: Route role checks reject several invalid sender/destination combinations in `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs:88-163`.
- Recommendations: Validate lengths against remaining bytes and a hard maximum, validate registry IDs and player/NPC indices, reject impossible counts, and catch/log malformed packets without taking down the network callback.

**Networked game state trusts unbounded client aim/input values:**
- Risk: The server stores and rebroadcasts a client-provided mouse-world vector without range or finite-value validation; several projectiles and interactions consume that value as gameplay input.
- Files: `Sources/Everglow.Function/Netcode/Packets/MousePositionSyncPacket.cs:19-43`, `Sources/Everglow.Function/Utilities/PlayerUtils.cs:73-89`, `Sources/Everglow.Function/MEAC/MeleeProj.cs:139-145`, `Sources/Modules/Yggdrasil/KelpCurtain/Projectiles/Ranged/ArcI_proj.cs:156-166`.
- Current mitigation: Packets are associated with the transport sender by `PacketResolver`, but the vector itself is not constrained.
- Recommendations: Clamp aim to an allowed range around the player, reject NaN/Infinity, and make damage/interaction decisions authoritative on the server.

**CI supply-chain inputs are not fully pinned:**
- Risk: The build downloads the latest tModLoader archive and XNA installer at run time, and the normal build workflow references `actions/checkout@v6` rather than a commit SHA. Restore also has no NuGet lock file.
- Files: `.github/workflows/build-and-test.yml:18-19`, `.github/workflows/build-and-test.yml:86-113`, `Sources/Directory.Build.props:23-31`, `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:10-17`.
- Current mitigation: The generated agentic workflow pins many action/container digests in `.github/workflows/holistic-review.lock.yml:35-53`; runtime mod code has no remote HTTP service.
- Recommendations: Pin tModLoader/XNA artifacts and action SHAs, use dependency lock/restore validation, and periodically review the checked-in `Libraries/ModLiquidLib.dll` and `Libraries/SubworldLibrary.dll` against documented versions.

**Not detected:**
- No application HTTP listener, database credential, embedded API key, or remote runtime authentication flow was found under `Sources/Everglow/`, `Sources/Everglow.Core/`, `Sources/Everglow.Function/`, or `Sources/Modules/`. Secret-bearing files were not opened.

## Performance Bottlenecks

**Elemental debuffs scale work with every NPC and every registered element:**
- Problem: Each NPC creates one `ElementalDebuffInstance` per registered debuff, then `AI`, `UpdateLifeRegen`, `ResetEffects`, and health-bar drawing iterate the full dictionary. Health-bar drawing also ends and restarts the global sprite batch for each NPC.
- Files: `Sources/Everglow.Function/Mechanics/ElementalDebuff/ElementalDebuffGlobalNPC.cs:8-38`, `Sources/Everglow.Function/Mechanics/ElementalDebuff/ElementalDebuffGlobalNPC.cs:40-90`, `Sources/Everglow.Function/Mechanics/ElementalDebuff/ElementalDebuffGlobalNPC.cs:93-136`.
- Cause: Dense per-entity state and repeated LINQ/dictionary enumeration rather than tracking only active elements.
- Improvement path: Store active debuffs separately, skip inactive instances in update/draw loops, and batch health-bar rendering outside the per-NPC callback.

**VFX and post-processing repeatedly scan global projectile collections:**
- Problem: MEAC and Yggdrasil effect hooks scan `Main.projectile` multiple times per capture and then perform several full-screen render-target passes.
- Files: `Sources/Everglow.Function/MEAC/MEACManager.cs:43-57`, `Sources/Everglow.Function/MEAC/MEACManager.cs:59-120`, `Sources/Everglow.Function/MEAC/MEACManager.cs:194-265`, `Sources/Modules/Yggdrasil/YggdrasilModule.cs:145-211`, `Sources/Modules/Yggdrasil/YggdrasilModule.cs:214-264`.
- Cause: Separate discovery and draw passes plus immediate shader/render-target changes for each effect family.
- Improvement path: Maintain active effect registries, combine capability checks into one traversal, and skip full-screen passes unless a non-empty render list is available.

**Cooldown UI performs expensive SpriteBatch state changes per icon:**
- Problem: Each cooldown icon can end and restart the sprite batch several times, apply shaders, draw a progress primitive, and format duration text during `PostDrawInterface`.
- Files: `Sources/Everglow.Function/Mechanics/Cooldown/CooldownSystem.cs:25-55`, `Sources/Everglow.Function/Mechanics/Cooldown/CooldownSystem.cs:94-143`.
- Cause: Per-icon rendering is not grouped by shader/state.
- Improvement path: Group icons by cut-shader requirement, batch progress geometry, and cache localized duration text until the displayed second changes.

## Fragile Areas

**Reflection-driven discovery and packet IDs depend on load shape:**
- Files: `Sources/Everglow.Core/Modules/ModuleManager.cs:11-49`, `Sources/Everglow.Function/Netcode/PacketResolver.cs:98-143`, `Sources/Everglow/Everglow.cs:54-86`.
- Why fragile: Assembly order, public parameterless constructors, type names beginning with `Everglow.`, and packet discovery all become runtime contracts; a missing constructor or changed assembly identity fails loading rather than producing a compile-time error.
- Safe modification: Add explicit diagnostics and discovery tests, preserve stable packet IDs or a protocol version, and avoid new global scan side effects in content constructors.
- Test coverage: No integration test covers full assembly discovery/content registration; only private route/serialization helpers are covered in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:12-177`.

**IL and hook integrations are version-sensitive:**
- Files: `Sources/Modules/Yggdrasil/YggdrasilModule.cs:88-117`, `Sources/Modules/Myth/MythModule.cs:66-115`, `Sources/Everglow.Function/Hooks/HookSystem.cs:37-78`.
- Why fragile: Yggdrasil matches local-variable indices and throws when anchors move; Myth silently returns when its anchor is absent; both depend on tModLoader/Terraria method shapes.
- Safe modification: Pin/test against the supported tML build, log patch status with the affected feature, and provide a runtime smoke test for map rendering and water effects.
- Test coverage: No automated client rendering or IL-hook test is detected in `Sources/Everglow.UnitTests/`.

**Global state and unload paths are difficult to reason about:**
- Files: `Sources/Everglow.Core/Ins.cs:15-67`, `Sources/Everglow.Function/ModIns.cs`, `Sources/Everglow.Function/CustomTiles/ColliderManager.cs:22-41`, `Sources/Modules/SubSpace/RoomWorld.cs:8-26`.
- Why fragile: Static service containers, event delegates, subworld fields, and hook registrations span module lifecycles; `ColliderManager.Unload` nulls its list but leaves `Enable` unchanged and relies on a commented-out manual unhook block.
- Safe modification: Make unload idempotent, clear all static flags/state, and centralize hook ownership/disposal.
- Test coverage: No reload/unload or repeated subworld transition test is detected.

## Scaling Limits

**Room-world persistence writes whole maps synchronously and grows without a retention policy:**
- Current capacity: `RoomWorld` is 300 x 300 tiles, but `SaveRoomDatas` constructs a `MapIO` over nearly the entire current world and writes one compressed file for every room anchor/depth transition.
- Files: `Sources/Modules/SubSpace/RoomWorld.cs:28-58`, `Sources/Modules/SubSpace/RoomManager.cs:137-152`, `Sources/Everglow.Function/TileHelper/MapIO.cs:121-146`.
- Limit: Deep or frequently visited rooms cause long transition stalls, large save directories, and repeated allocations/compression on the main thread.
- Scaling path: Serialize only the room bounds, write asynchronously or incrementally where the engine permits, cap/compact room history, and add save-size/transition-time telemetry.

**Yggdrasil generation is a very large single-threaded workload:**
- Current capacity: The subworld is fixed at 2000 x 21000 tiles and its generation calls large Kelp Curtain and town passes on the game thread.
- Files: `Sources/Modules/Yggdrasil/YggdrasilWorld.cs:22-33`, `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs:29-40`, `Sources/Modules/Yggdrasil/WorldGeneration/KelpCurtainGeneration.cs:313-395`, `Sources/Modules/Yggdrasil/WorldGeneration/KelpCurtainGeneration.cs:1517-1677`.
- Limit: Generation time and memory rise with world size; repeated nested loops and tile framing can cause long loading screens or watchdog-like behavior.
- Scaling path: Profile each generation pass, use bounded work/progress checkpoints, reduce repeated tile scans, and avoid retaining large temporary collections after each region.

**Per-NPC/per-projectile work is bounded by vanilla array sizes rather than active entities:**
- Current capacity: Several systems traverse all `Main.npc` or `Main.projectile` entries on frequent callbacks.
- Files: `Sources/Modules/Yggdrasil/YggdrasilTown/YggdrasilTownCentralSystem.cs:125-147`, `Sources/Modules/Yggdrasil/YggdrasilTown/YggdrasilTownCentralSystem.cs:311-395`, `Sources/Everglow.Function/Utilities/PlayerUtils.cs:203-208`, `Sources/Everglow.Function/MEAC/MEACManager.cs:43-57`.
- Limit: More active effects, NPCs, or multiplayer players increase frame cost even when most array slots are empty.
- Scaling path: Cache active targets, use lifecycle registration, and keep expensive scans out of per-player/per-projectile callbacks.

## Dependencies at Risk

**tModLoader version drift is uncontrolled:**
- Risk: CI downloads the latest tModLoader release while the project documents Terraria 1.4.4.9 and uses IL/On hooks tied to implementation details.
- Files: `.github/workflows/build-and-test.yml:96-101`, `README.md:35-39`, `Sources/Modules/Yggdrasil/YggdrasilModule.cs:88-117`, `Sources/Modules/Myth/MythModule.cs:66-115`.
- Impact: A new tML release can break compilation, IL anchors, shaders, or runtime behavior without a source change.
- Migration plan: Pin a known tML artifact/commit, record the supported API build, and run a deliberate compatibility update when changing it.

**Local binary dependencies lack reproducible version metadata:**
- Risk: `Libraries/ModLiquidLib.dll` and `Libraries/SubworldLibrary.dll` are consumed directly, while project files do not declare their versions or hashes.
- Files: `Libraries/ModLiquidLib.dll`, `Libraries/SubworldLibrary.dll`, `Sources/Modules/Directory.Build.props:14-20`, `Sources/Everglow/build.txt:1-4`.
- Impact: A changed binary can alter subworld routing, liquid behavior, or type compatibility without NuGet restore detecting the change.
- Migration plan: Track source release/version/hash metadata and add a compatibility check before packaging.

**Build package and framework versions are not locked as a repository-wide graph:**
- Risk: NuGet package versions are specified per project but no `packages.lock.json`, central package management, or lock validation is detected; `Microsoft.Extensions.DependencyInjection` 9.0.8 is used by a net8.0 project.
- Files: `Sources/Directory.Build.props:23-27`, `Sources/Everglow.Core/Everglow.Core.csproj:13-17`, `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:10-17`.
- Impact: Restore can change transitive dependencies between builds, and framework-major mismatches can surface only on a clean environment.
- Migration plan: Add lock files/central version policy and verify package compatibility with the net8.0 target.

## Missing Critical Features

**Persistent subworld progress is disabled:**
- Problem: Yggdrasil, RoomWorld, and MothWorld all set `ShouldSave => false`; Yggdrasil explicitly says this is temporary and should be changed for publication.
- Files: `Sources/Modules/Yggdrasil/YggdrasilWorld.cs:22-29`, `Sources/Modules/SubSpace/RoomWorld.cs:53-58`, `Sources/Modules/Myth/TheFirefly/WorldGeneration/MothWorld.cs:6-18`.
- Blocks: World/subworld changes and progression are lost across subworld reload/exit unless a separate custom persistence path handles them.

**Room navigation API is incomplete:**
- Problem: `GoToTargetLevelRoom` is an empty public method, while room entry/exit state is held in static fields.
- Files: `Sources/Modules/SubSpace/RoomManager.cs:94-100`, `Sources/Modules/SubSpace/RoomWorld.cs:16-26`.
- Blocks: Reliable restoration to an arbitrary saved room depth and safe recovery after interrupted transitions.

**Localization and presentation completion remain incomplete for shipped content:**
- Problem: Player-facing strings and bestiary text are hardcoded or use culture checks, while multiple active biome/content files retain TODO markers for BGM, backgrounds, icons, water styles, and translations.
- Files: `Sources/Modules/Myth/TheFirefly/Items/Armors/FireflywoodHelmet.cs:34-37`, `Sources/Modules/Myth/Acytaea/NPCs/Acytaea.cs:73-92`, `Sources/Modules/Yggdrasil/KelpCurtain/Biomes/DecayingWoodCourtStrongholdBiome.cs:7-32`.
- Blocks: Consistent localization across the repository's required `en-US` and `zh-Hans` targets and complete player-facing presentation.

## Test Coverage Gaps

**No malformed-packet or adversarial-input tests:**
- What's not tested: Negative/oversized payloads, invalid packet IDs, invalid registry IDs, invalid entity indices, forged quest deltas, and unknown quest/objective names.
- Files: `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:12-177`, `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs:27-85`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Packets/ObjectiveDeltaSyncPacket_SubProgress.cs:23-43`.
- Risk: A malformed or cheating client can crash a callback, allocate excessive memory, or mutate world progress.
- Priority: High.

**Graphics, resource, and lifecycle paths are not covered by automated tests:**
- What's not tested: Render-target disposal/resizing, shader pipeline ordering, dedicated-server graphics fallbacks, IL hook anchors, and mod reload/unload.
- Files: `Sources/Everglow.Function/MEAC/MEACManager.cs:32-40`, `Sources/Everglow.Function/VFX/VFXManager.cs:189-231`, `Sources/Everglow.Function/VFX/FakeManager.cs:5-29`, `Sources/Modules/Yggdrasil/YggdrasilModule.cs:25-117`.
- Risk: Client-only regressions and GPU/resource leaks require manual tML runtime verification and can evade the MSTest suite.
- Priority: High.

**World generation and MapIO have no focused tests:**
- What's not tested: Bounds, corrupted maps, missing content mappings, chest capacity, tile-entity restoration, generation determinism, and generation performance.
- Files: `Sources/Everglow.Function/TileHelper/MapIO.cs:49-73`, `Sources/Everglow.Function/TileHelper/MapIO.cs:586-716`, `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs:20-164`, `Sources/Everglow.UnitTests/Function/`.
- Risk: Save corruption, world-generation regressions, and long transition stalls are discovered only in live worlds.
- Priority: High.

**Module coverage is narrow:**
- What's not tested: Most active content modules, including Yggdrasil generation/rendering, Food UI, SubSpace room transitions, Myth IL/water hooks, and AssetReplace hooks.
- Files: `Sources/Everglow.UnitTests/Modules/Yggdrasil/Common/YggdrasilPlayerTests.cs`, `Sources/Everglow.UnitTests/Function/`, `Sources/Modules/Yggdrasil/`, `Sources/Modules/SubSpace/`, `Sources/Modules/Myth/`.
- Risk: The full mod can compile while content behavior, multiplayer synchronization, or client rendering fails in-game.
- Priority: Medium.

**Coverage enforcement is absent:**
- Requirements: No numeric coverage target or CI coverage publication/gate is configured; `coverlet.collector` is present but only available for local collection.
- Files: `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:14-17`, `.github/workflows/build-and-test.yml:115-119`.
- Risk: Regressions can reduce meaningful coverage without a visible quality signal.
- Priority: Medium.

**Not detected:**
- No end-to-end multiplayer, client-rendering, or UI automation framework is present in `Sources/Everglow.UnitTests/` or the test project configuration. The absence is documented here as a gap, not as evidence of a failing test.

---

*Concerns audit: 2026-09-11*
