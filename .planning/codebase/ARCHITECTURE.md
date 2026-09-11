<!-- refreshed: 2026-09-11 -->
# Architecture

**Analysis Date:** 2026-09-11

## System Overview

```text
┌─────────────────────────────────────────────────────────────┐
│                 tModLoader / Terraria runtime                │
│                 `Sources/Everglow/Everglow.cs`               │
└───────────────┬─────────────────────┬───────────────────────┘
                │                     │
                ▼                     ▼
┌────────────────────────┐  ┌────────────────────────────────┐
│ Composition root        │  │ tML shared systems              │
│ `Sources/Everglow/`    │  │ `Sources/Everglow.Function/`   │
│ DI, discovery, content  │  │ hooks, UI, VFX, mechanics, net │
└────────────┬───────────┘  └───────────────┬────────────────┘
             │                              │
             ▼                              ▼
┌────────────────────────┐  ┌────────────────────────────────┐
│ Active content modules  │  │ Terraria-independent foundation │
│ `Sources/Modules/`      │  │ `Sources/Everglow.Core/`       │
│ gameplay and assets     │  │ interfaces, DI, utilities       │
└────────────┬───────────┘  └───────────────┬────────────────┘
             │                              │
             └──────────────┬───────────────┘
                            ▼
                 ┌────────────────────────┐
                 │ Everglow.tmod package  │
                 │ resource lists/assets   │
                 │ `Sources/Directory.*`  │
                 └────────────────────────┘
```

Everglow is a tModLoader mod composed from one host project, two reusable code assemblies, and the active module assemblies listed in `Sources/Directory.Build.props`. The host project `Sources/Everglow/Everglow.csproj` references Core, Function, and dynamically expands the `Modules` property into module project references. The resulting build packages the assemblies and merged resource files into the Everglow mod output.

The runtime composition root is `Sources/Everglow/Everglow.cs`. It creates the shared service container, discovers `Everglow.*` assemblies, loads lifecycle modules, reflects content types, registers tML content, and installs packet handling. Ordinary gameplay is supplied by `ModItem`, `ModNPC`, `ModProjectile`, `ModSystem`, `ModPlayer`, tile, biome, UI, and subworld types distributed through `Sources/Everglow.Function/` and `Sources/Modules/`.

## Component Responsibilities

| Component | Responsibility | File |
|-----------|----------------|------|
| Mod host | Owns tML `Mod` lifecycle, service startup/shutdown, content registration, and packet entry | `Sources/Everglow/Everglow.cs` |
| Build composition | Defines target framework, active module symbols, resource packing, and module project expansion | `Sources/Directory.Build.props`, `Sources/Modules/Directory.Build.props`, `Sources/Everglow/Everglow.csproj` |
| Core foundation | Provides interfaces, service registration, module discovery primitives, pools, math, vertex, VFX, and other shared infrastructure without a direct tML project reference | `Sources/Everglow.Core/` |
| Function layer | Provides tML-facing shared mechanics, hooks, UI, VFX pipelines, physics, localization helpers, and netcode | `Sources/Everglow.Function/` |
| Module lifecycle | Loads optional module-wide hooks and effects through `IModule`/`EverglowModule` | `Sources/Everglow.Core/Modules/IModule.cs`, `Sources/Everglow.Function/Modules/EverglowModule.cs` |
| Content modules | Owns feature-specific items, NPCs, projectiles, tiles, systems, worlds, effects, and assets | `Sources/Modules/` |
| Quest domain | Owns player/world quest state, objective graphs, persistence, synchronization, actions, and presentation projection | `Sources/Everglow.Function/Mechanics/Quest/` |
| UI host | Owns the shared UI state, interface-layer insertion, UI update loop, and render-target composition | `Sources/Everglow.Function/UI/UISystem.cs`, `Sources/Everglow.Function/UI/EverglowUISystem.cs` |
| Tests | Verifies Core, Function, Quest, netcode, UI, physics, and selected module behavior with MSTest | `Sources/Everglow.UnitTests/` |

## Pattern Overview

**Overall:** Reflection-composed layered mod architecture with tModLoader lifecycle adapters and module-oriented content assemblies.

**Key Characteristics:**
- `Sources/Everglow/Everglow.cs` is the single composition root; it delegates type discovery to `Sources/Everglow.Core/Modules/ModuleManager.cs` rather than maintaining a manual content list.
- `Sources/Everglow.Core/Ins.cs` provides a singleton-style dependency container for shared services, while `Sources/Everglow.Function/ModIns.cs` carries the active tML mod, packet resolver, and lifecycle events.
- Module assemblies are isolated by project reference; cross-module dependencies are explicit in module `.csproj` files such as `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj`.
- tML `ModSystem`, `ModPlayer`, `GlobalNPC`, `GlobalItem`, and content callbacks are the game-facing adapters; domain and shared services hold reusable behavior behind those callbacks.
- Client graphics are layered through `CodeLayer`, `HookSystem`, `VFXManager`, and render-target pipelines, while dedicated-server paths register `FakeManager` or omit graphics services in `Sources/Everglow/Everglow.cs`.

## Layers

**Host and package layer:**
- Purpose: Enter and leave the tML mod lifecycle, assemble modules, and turn module resource lists into one mod package.
- Location: `Sources/Everglow/`, `Sources/Directory.Build.props`, `Sources/Directory.Build.targets`.
- Contains: `Everglow : Mod`, project references, resource MSBuild targets, mod metadata, and localization roots.
- Depends on: Core, Function, and the active modules from `Sources/Directory.Build.props`.
- Used by: tModLoader and the build pipeline.

**Core infrastructure layer:**
- Purpose: Supply tML-independent contracts and reusable low-level services.
- Location: `Sources/Everglow.Core/`.
- Contains: `Ins`, `IModule`, `ModuleManager`, `IHookManager`, `IVFXManager`, `IVisual`, object pools, coroutines, data structures, math, vertices, and VFX primitives.
- Depends on: .NET, `MathNet.Numerics`, dependency injection, and configured shared library references in `Sources/Everglow.Core/Everglow.Core.csproj`.
- Used by: Function, the host, modules, and tests. Keep tML implementations out of this layer.

**Function/shared tML layer:**
- Purpose: Adapt Core services to Terraria/tModLoader and provide features shared by modules.
- Location: `Sources/Everglow.Function/`.
- Contains: hook dispatch, UI, VFX registration and pipelines, physics, custom tiles, global gameplay mechanics, localization exporters, templates, and packet infrastructure.
- Depends on: Core and tML/FNA APIs.
- Used by: `Sources/Everglow/Everglow.cs`, every active module through `Sources/Modules/Directory.Build.props`, and unit tests.

**Module lifecycle layer:**
- Purpose: Run assembly-scoped load/unload behavior that ordinary tML content callbacks do not express.
- Location: `Sources/Everglow.Core/Modules/` and `Sources/Everglow.Function/Modules/`.
- Contains: `IModule`, `EverglowModule`, reflection discovery, and module-specific hooks such as `Sources/Modules/Myth/MythModule.cs` and `Sources/Modules/Yggdrasil/YggdrasilModule.cs`.
- Depends on: Core services and module/tML APIs.
- Used by: `Everglow.AddServices()` and `ModuleManager`.

**Content module layer:**
- Purpose: Implement independent gameplay/content slices and their matching assets.
- Location: `Sources/Modules/<ModuleName>/`.
- Contains: tML content classes, module systems, optional lifecycle modules, world generation, subworlds, UI, VFX, effects, music, localization-adjacent assets, and packed data.
- Depends on: Core and Function automatically; explicit project references for cross-module types.
- Used by: The host's reflected content registration and tML runtime.

**Domain/presentation layer:**
- Purpose: Keep complex shared gameplay models separate from tML callbacks and UI rendering.
- Location: `Sources/Everglow.Function/Mechanics/Quest/` and other `Mechanics/` subtrees.
- Contains: managers, state machines, objective structures, adapters, immutable presentation views, actions, packets, and UI elements.
- Depends on: tML only at integration boundaries; presentation adapters read domain state and `UISystem` consumes the resulting view/action model.
- Used by: `ModSystem`/`ModPlayer` adapters, packet handlers, and UI.

## Data Flow

### Primary Mod Load Path

1. tModLoader invokes `Everglow.Load()` in `Sources/Everglow/Everglow.cs`.
2. The host stores the active mod in `Sources/Everglow.Function/ModIns.cs`, calls `Ins.Begin()`, and registers logging, graphics, hook, thread, module, render-target, and VFX services through `Sources/Everglow.Core/Ins.cs`.
3. Building the provider makes `ModuleManager` scan loaded `Everglow.*` assemblies, instantiate eligible `IModule` implementations, and call `Load()` in `Sources/Everglow.Core/Modules/ModuleManager.cs`.
4. `AddContents()` reflects module types, registers eligible `ModConfig` instances, then calls tML `AddContent()` for reflected `ILoadable` types in `Sources/Everglow/Everglow.cs`.
5. `PacketResolver` scans the same type set, assigns packet IDs, binds `[HandlePacket]` handlers, and stores routing metadata in `Sources/Everglow.Function/Netcode/PacketResolver.cs`.
6. tML invokes later `ModSystem` lifecycle callbacks; `Everglow.PostSetupContent()` forwards the phase through `Sources/Everglow.Function/ModIns.cs`.

### Resource Build and Runtime Path

1. Each module inherits `Sources/Modules/Directory.Build.props`, which derives `ModuleName` and `PathPrefix` from the project name.
2. `Sources/Directory.Build.props` allowlists JSON, atlas, object, bitmap, mapio, font, and other configured resource inputs.
3. `Sources/Directory.Build.targets` writes per-prefix resource lists using `Everglow.Tasks.dll`.
4. `Sources/Everglow/Everglow.csproj` reads the Commons and module resource lists before building and adds them to the host's packed files.
5. Generated `ModAsset` members and runtime prefixes such as `Everglow/Yggdrasil/...` are consumed by content classes such as `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs`.

### Gameplay and Rendering Path

1. tML calls content callbacks and `ModSystem`/`ModPlayer` updates in the active assembly, for example `Sources/Everglow.Function/Mechanics/Quest/Hooks/QuestPlayer.cs` or `Sources/Modules/Yggdrasil/YggdrasilTown/YggdrasilTownCentralSystem.cs`.
2. Shared systems publish state changes through managers/events or register work with `HookManager` in `Sources/Everglow.Function/Hooks/HookManager.cs`.
3. `HookSystem` maps Terraria draw/update/world callbacks to `CodeLayer` dispatch in `Sources/Everglow.Function/Hooks/HookSystem.cs`; exceptions disable the failing handler after logging.
4. `VFXManager` discovers `IVisual` implementations, groups them by draw layer and pipeline, updates them at `PostUpdateEverything`, and renders them through pooled render targets in `Sources/Everglow.Function/VFX/VFXManager.cs`.
5. UI systems insert shared and feature-specific UI into Terraria interface layers through `Sources/Everglow.Function/UI/UISystem.cs`; Quest UI subscribes to presentation events through `Sources/Everglow.Function/Mechanics/Quest/Presentation/QuestPresentationSystem.cs`.

### Quest State, Persistence, and Presentation

1. `PlayerQuestSystem` constructs a player manager/actions pair, while `WorldQuestSystem` constructs a world manager/actions pair in `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/PlayerQuestSystem.cs` and `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestSystem.cs`.
2. Player state enters through `QuestPlayer.SaveData()`/`LoadData()` and is applied on `OnEnterWorld()` in `Sources/Everglow.Function/Mechanics/Quest/Hooks/QuestPlayer.cs`; world state enters through `ModSystem` save/load and `NetSend`/`NetReceive` in `WorldQuestSystem.cs`.
3. Managers update objectives from game hooks and tick callbacks, emit identity-based events, and route world deltas through `PacketResolver` as implemented by `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestManager.cs`.
4. `QuestPresentationSystem` creates one `QuestPresentationService` after setup. The service projects player/world domain objects through independent adapters into immutable `QuestPresentationEntry` views and actions in `Sources/Everglow.Function/Mechanics/Quest/Presentation/QuestPresentationService.cs`.
5. Quest UI reads those snapshots and refreshes on queued presentation events; it should not mutate managers directly. The separation contract is documented in `Sources/Everglow.Function/Mechanics/Quest/QUEST_SYSTEM_DESIGN.md`.

### Network Packet Path

1. A feature constructs an `IPacket` and sends it through `ModIns.PacketResolver`, typically from shared netcode or a module packet type in `Sources/Everglow.Function/Netcode/` or `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Packets/`.
2. tML calls `Everglow.HandlePacket()` in `Sources/Everglow/Everglow.cs`, which delegates to `PacketResolver.Resolve()`.
3. `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs` validates `WorldOnly`, `MainServer`, or `AllDownstream` routes, forwards through SubworldLibrary when required, deserializes the packet, and invokes registered handlers.
4. The handler mutates authoritative state or emits a sync packet; the Quest world path uses `QuestSyncPacket` in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Packets/QuestSyncPacket.cs`.

### Subworld and World-Generation Path

1. A content interaction calls SubworldLibrary through a world entry such as `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, `Sources/Modules/SubSpace/RoomWorld.cs`, or `Sources/Modules/Myth/TheTusk/WorldGeneration/TuskWorld.cs`.
2. The subworld defines dimensions, save behavior, lifecycle, and `GenPass` tasks; Yggdrasil's task enters `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs`.
3. Generation reads packed map/noise inputs, writes Terraria tiles/walls/liquids, and may compose prefabricated MapIO structures through shared tile helpers.
4. Runtime systems gate region-specific behavior using `SubworldSystem.IsActive<T>()` or the module's explicit context helper, as shown in `Sources/Modules/Yggdrasil/YggdrasilTown/YggdrasilTownCentralSystem.cs`.
5. Nested RoomWorld state is serialized to per-world mod data and restored by `Sources/Modules/SubSpace/RoomManager.cs`.

**State Management:**
- Runtime service state is held in the `Ins` provider and `ModIns` statics; both are cleared from `Sources/Everglow/Everglow.cs` during unload.
- Player state is attached to Terraria players through `ModPlayer` and serialized with `TagCompound`; world state is attached to `ModSystem` and serialized through world data/netcode callbacks.
- VFX and render targets are transient and cleared by `VFXManager` at save/quit or unload; subworld room maps use explicit files under Terraria's mod-data save path in `Sources/Modules/SubSpace/RoomManager.cs`.

## Key Abstractions

**Service container:**
- Purpose: Provide shared singleton infrastructure while keeping implementations replaceable by interface.
- Examples: `Sources/Everglow.Core/Ins.cs`, `Sources/Everglow/Everglow.cs`.
- Pattern: Register with `Ins.Add<TService, TImplementation>()`, build once with `Ins.End()`, resolve through typed properties, and clear on unload.

**Reflection module registry:**
- Purpose: Discover code assemblies, lifecycle modules, content types, VFX types, configs, and packet types without hardcoding every content class.
- Examples: `Sources/Everglow.Core/Modules/ModuleManager.cs`, `Sources/Everglow.Function/Netcode/PacketResolver.cs`.
- Pattern: Filter `Everglow.*` assemblies and `ModuleHideTypeAttribute`, instantiate public/non-abstract assignable types, and let tML own the content lifecycle after registration.

**Hook layer:**
- Purpose: Offer stable named update/draw/world lifecycle points over MonoMod and tML callbacks.
- Examples: `Sources/Everglow.Core/Interfaces/IHookManager.cs`, `Sources/Everglow.Function/Hooks/HookManager.cs`, `Sources/Everglow.Function/Hooks/HookSystem.cs`.
- Pattern: Register a `CodeLayer` delegate for shared dispatch or a direct MonoMod/IL hook for a specific method; remove/dispose through the manager.

**Visual pipeline:**
- Purpose: Register visual objects and render them in ordered pipelines with optional render-target passes.
- Examples: `Sources/Everglow.Core/Interfaces/IVisual.cs`, `Sources/Everglow.Function/VFX/Visual.cs`, `Sources/Everglow.Function/VFX/VFXManager.cs`.
- Pattern: Implement `IVisual`, choose a `CodeLayer`, annotate pipeline requirements, and let `VFXManager` handle update, sorting, pooling, and draw ordering.

**Quest presentation boundary:**
- Purpose: Expose player/world quest state as read-only views and validated actions without coupling UI to domain managers.
- Examples: `Sources/Everglow.Function/Mechanics/Quest/Presentation/QuestPresentationService.cs`, `Sources/Everglow.Function/Mechanics/Quest/Presentation/Adapters/WorldQuestViewAdapter.cs`.
- Pattern: Add domain behavior to the side-specific manager/action layer, add projection logic to the matching adapter, and consume snapshots from `QuestPresentationService`.

**Subworld context:**
- Purpose: Define isolated world dimensions, generation, transition, and region-specific behavior.
- Examples: `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, `Sources/Modules/SubSpace/RoomWorld.cs`, `Sources/Modules/SubSpace/RoomManager.cs`.
- Pattern: Put generation in `WorldGeneration/`, gate runtime code with SubworldLibrary state, and keep return/anchor state in the owning subworld module.

## Entry Points

**Mod load/unload entry:**
- Location: `Sources/Everglow/Everglow.cs`.
- Triggers: tModLoader calls `Load`, `PostSetupContent`, `Unload`, and `HandlePacket`.
- Responsibilities: Compose services, discover/register content, initialize packet routing, forward setup/unload events, and resolve incoming packets.

**tML shared systems:**
- Location: `Sources/Everglow.Function/Hooks/HookSystem.cs`, `Sources/Everglow.Function/UI/UISystem.cs`, `Sources/Everglow.Function/Mechanics/Quest/Presentation/QuestPresentationSystem.cs`.
- Triggers: tML `ModSystem` lifecycle and update/draw/world callbacks.
- Responsibilities: Bridge Terraria callbacks into shared hook layers, update/draw UI and VFX, and assemble quest presentation after content setup.

**Content module lifecycle:**
- Location: `Sources/Modules/Myth/MythModule.cs`, `Sources/Modules/Yggdrasil/YggdrasilModule.cs`, `Sources/Modules/Food/FoodSystem.cs`.
- Triggers: `ModuleManager` construction during host service resolution.
- Responsibilities: Install module-wide hooks, effects, filters, and events that are not ordinary reflected content.

**World/subworld entry:**
- Location: `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, `Sources/Modules/SubSpace/RoomWorld.cs`, `Sources/Modules/Myth/TheTusk/WorldGeneration/TuskWorld.cs`.
- Triggers: player/content interactions through SubworldLibrary.
- Responsibilities: Define dimensions, generation passes, enter/load/exit behavior, and context state.

**Build/resource entry:**
- Location: `Sources/Directory.Build.props`, `Sources/Directory.Build.targets`, `Sources/Everglow/Everglow.csproj`.
- Triggers: MSBuild restore/build.
- Responsibilities: Set shared compilation properties, generate module path prefixes, pack additional files, write resource lists, and merge module resources into the host project.

## Architectural Constraints

- **Threading:** Gameplay, tML callbacks, hook dispatch, UI, and graphics services assume Terraria's main/game thread; deferred graphics work is scheduled through `Sources/Everglow.Function/MainThreadContext.cs` and render-target allocation is guarded by the client path in `Sources/Everglow/Everglow.cs`.
- **Core boundary:** `Sources/Everglow.Core/Everglow.Core.csproj` removes direct tML/ReLogic references; keep Terraria/tML implementations in `Sources/Everglow.Function/` or `Sources/Modules/` and expose Core interfaces where a boundary is needed.
- **Global state:** `Sources/Everglow.Core/Ins.cs`, `Sources/Everglow.Function/ModIns.cs`, `PlayerQuestManager.Instance`, `WorldQuestManager.Instance`, and static subworld fields in `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`/`Sources/Modules/SubSpace/RoomWorld.cs` are process/runtime singletons; initialize and clear them through their owning lifecycle.
- **Assembly discovery:** `Sources/Everglow.Core/Modules/ModuleManager.cs` expects the active assemblies to use the `Everglow.*` naming convention and selects the last loaded host assembly on reload; renaming an assembly or namespace affects reflection discovery and serialized type names.
- **Module graph:** Module projects inherit Core and Function automatically from `Sources/Modules/Directory.Build.props`; every newly introduced cross-module type dependency must be explicit in the owning `.csproj`, with current edges in `Sources/Modules/Myth/Everglow.Myth.csproj`, `Sources/Modules/Ocean/Everglow.Ocean.csproj`, and `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj`.
- **Active modules:** `Sources/Directory.Build.props` is the source of truth for active module symbols and host project references; its matching CI block must stay synchronized with `.github/workflows/build-and-test.yml`.
- **Dedicated server:** Do not resolve graphics-only services or touch render targets, textures, or client-only effects on a dedicated server. The host registers graphics services only when `!Main.dedServ`, and the server receives `FakeManager` in `Sources/Everglow/Everglow.cs`.
- **Rendering hooks:** `Sources/Everglow.Function/Hooks/HookSystem.cs` and module IL hooks rely on tML callback order and local-variable/method shapes; preserve original calls where appropriate and fail visibly when an IL anchor cannot be found.
- **Circular imports:** No circular project-reference chain is present in the explicit module edges inspected; `Sources/Everglow.Core/Modules/DependencyGraph.cs` can detect type dependency cycles, but normal module ordering is primarily supplied by project references and reflection load order.
- **Resource paths:** Runtime paths are derived from `PathPrefix` in `Sources/Modules/Directory.Build.props`; use generated asset members and keep module-local assets beside the owning code instead of duplicating handwritten paths.

## Anti-Patterns

### Bypassing the module/content registry

**What happens:** A new ordinary tML content type is manually registered or a second ad hoc assembly scan is introduced instead of using `ModuleManager` and `Everglow.AddContents()` in `Sources/Everglow/Everglow.cs`.
**Why it's wrong:** It creates duplicate registration paths and can diverge from the filtering, hidden-type, config, and unload behavior in `Sources/Everglow.Core/Modules/ModuleManager.cs`.
**Do this instead:** Place the type in the correct `Sources/Modules/<ModuleName>/` or shared Function directory and let the existing reflection registration discover it; reserve `EverglowModule` for lifecycle hooks such as `Sources/Modules/Yggdrasil/YggdrasilModule.cs`.

### Mixing quest UI with quest domain mutation

**What happens:** UI code reaches into `PlayerQuestManager`/`WorldQuestManager` or mutates objectives rather than using presentation snapshots and validated actions.
**Why it's wrong:** It bypasses identity checks, hint masking, server authority, and the separate player/world contracts documented by `Sources/Everglow.Function/Mechanics/Quest/QUEST_SYSTEM_DESIGN.md`.
**Do this instead:** Query `QuestPresentationService` and execute its `QuestAction` values; keep state changes in `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/` or `WorldSide/`.

### Running graphics work on a server path

**What happens:** Module code accesses `Main.instance.GraphicsDevice`, render targets, textures, or shader assets without the client guard used in `Sources/Modules/Yggdrasil/YggdrasilModule.cs`.
**Why it's wrong:** Graphics services are not available on dedicated servers and the same code path is invoked during server-side content loading.
**Do this instead:** Guard registration and use with `!Main.dedServ`/the relevant net mode, and provide a server-safe implementation such as `Sources/Everglow.Function/VFX/FakeManager.cs`.

### Relying on silent generation failure

**What happens:** World-generation code catches broad exceptions without reporting them in `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs`.
**Why it's wrong:** A partially generated subworld can appear valid while hiding the cause from the caller and from the host's logging path.
**Do this instead:** Keep tile bounds checks and safe helpers, but log contextual failures through `Ins.Logger` and preserve explicit failure behavior at the generation-pass boundary.

## Error Handling

**Strategy:** Fail early for invalid composition, packet registration, config scope, and unsupported routing; isolate optional hook failures by logging and disabling the faulty handler; keep world-generation failures and IL patch failures visible to the owning runtime.

**Patterns:**
- `Sources/Everglow/Everglow.cs` throws when a `ModConfig` scope is incompatible with the loaded mod side, preventing an invalid configuration from entering runtime.
- `Sources/Everglow.Function/Netcode/PacketResolver.cs` throws for handlers bound to unknown packet types, while `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs` warns and rejects invalid routes or missing handlers.
- `Sources/Everglow.Function/Hooks/HookSystem.cs` catches handler exceptions, logs them through `Ins.Logger`, marks the handler disabled, and continues dispatching other handlers.
- `Sources/Modules/Yggdrasil/YggdrasilModule.cs` throws `OperationCanceledException` when an IL anchor is not found; `Sources/Modules/Myth/MythModule.cs` returns when its IL anchor cannot be located, so module hook changes require runtime verification.
- Domain persistence validates reflected quest types and logs malformed player data in `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/PlayerQuestManager.cs`.

## Cross-Cutting Concerns

**Logging:** Use the `ILog` registered by `Sources/Everglow/Everglow.cs` and resolved through `Sources/Everglow.Core/Ins.cs`; hook and persistence failures already follow this path in `Sources/Everglow.Function/Hooks/HookSystem.cs` and `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/PlayerQuestManager.cs`.

**Validation:** Use tML/config validation at registration, explicit `ArgumentException`/`InvalidOperationException` for invalid graph or packet composition, route and sender validation in `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs`, and domain identity/state checks in `Sources/Everglow.Function/Mechanics/Quest/Presentation/QuestPresentationService.cs`.

**Authentication:** Vanilla/tML content registration is controlled by the host; multiplayer gameplay authority is route- and server-based. World quest reward claims are validated against the connected network slot and player name in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestManager.cs`, then authorized to subworld servers through `QuestGiveRewardPacket`.

**Persistence:** Player data uses `ModPlayer` `TagCompound` callbacks in `Sources/Everglow.Function/Mechanics/Quest/Hooks/QuestPlayer.cs`; world data uses `ModSystem` callbacks in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestSystem.cs`; room maps use explicit mod-data files in `Sources/Modules/SubSpace/RoomManager.cs`.

**Client/server separation:** Shared code must distinguish dedicated-server execution from client rendering. The host, `Sources/Everglow.Function/UI/UISystem.cs`, `Sources/Everglow.Function/VFX/VFXManager.cs`, and module lifecycle classes establish the current guard pattern.

---

*Architecture analysis: 2026-09-11*
