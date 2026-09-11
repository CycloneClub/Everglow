# Codebase Structure

**Analysis Date:** 2026-09-11

## Directory Layout

```text
Everglow/
├── Everglow.sln                         # active host, shared, module, and test projects
├── Sources/
│   ├── Everglow/                         # tML Mod host, metadata, localization
│   ├── Everglow.Core/                    # low-level shared contracts and utilities
│   ├── Everglow.Function/                # tML-facing shared systems and mechanics
│   ├── Everglow.UnitTests/               # MSTest project and tests
│   ├── Modules/                          # active and retired content assemblies
│   ├── Directory.Build.props             # shared build properties and active modules
│   └── Directory.Build.targets           # resource-list build targets
├── Resources/                            # repository-wide source resource inputs
├── Libraries/                            # required third-party/read-only assemblies
├── Documents/                            # build, IL, subworld, and design documentation
├── Tools/                                # build tasks, scripts, and utilities
├── Localization/                         # legacy/root localization area
└── .planning/codebase/                   # generated mapper reference documents
```

The solution and project boundaries are defined by `Everglow.sln` and the `.csproj` files under `Sources/`. The host project is `Sources/Everglow/Everglow.csproj`; shared code is split between `Sources/Everglow.Core/Everglow.Core.csproj` and `Sources/Everglow.Function/Everglow.Function.csproj`; active content modules are listed in `Sources/Directory.Build.props` and referenced from the host through `Sources/Everglow/Everglow.csproj`.

## Directory Purposes

**`Sources/Everglow/`:**
- Purpose: Host the single tModLoader `Mod` class and package metadata.
- Contains: `Sources/Everglow/Everglow.cs`, `Everglow.csproj`, `build.txt`, descriptions, icons, generated output, and localization.
- Key files: `Sources/Everglow/Everglow.cs`, `Sources/Everglow/Everglow.csproj`, `Sources/Everglow/Localization/en-US/`, `Sources/Everglow/Localization/zh-Hans/`.

**`Sources/Everglow.Core/`:**
- Purpose: Host the reusable lower layer that does not directly reference tModLoader/Terraria.
- Contains: `Ins`, module discovery, interfaces, object pools, coroutines, data structures, math/graphics helpers, vertices, and VFX primitives.
- Key files: `Sources/Everglow.Core/Ins.cs`, `Sources/Everglow.Core/Modules/ModuleManager.cs`, `Sources/Everglow.Core/Modules/IModule.cs`, `Sources/Everglow.Core/Interfaces/`, `Sources/Everglow.Core/Everglow.Core.csproj`.

**`Sources/Everglow.Function/`:**
- Purpose: Host tML-facing infrastructure shared by all content modules.
- Contains: `Hooks/`, `UI/`, `VFX/`, `Netcode/`, `Mechanics/`, `Physics/`, `CustomTiles/`, `Graphics/`, `Localization/`, `Templates/`, and shared integration files.
- Key files: `Sources/Everglow.Function/ModIns.cs`, `Sources/Everglow.Function/Hooks/HookSystem.cs`, `Sources/Everglow.Function/UI/UISystem.cs`, `Sources/Everglow.Function/VFX/VFXManager.cs`, `Sources/Everglow.Function/Netcode/PacketResolver.cs`.

**`Sources/Everglow.Function/Mechanics/`:**
- Purpose: Organize cross-module gameplay systems and their tML adapters.
- Contains: player/NPC/global mechanics, cooldowns, elemental debuffs, events, lighting, music helpers, and the Quest subsystem.
- Key files: `Sources/Everglow.Function/Mechanics/EverglowPlayer.cs`, `Sources/Everglow.Function/Mechanics/EverglowGlobalNPC.cs`, `Sources/Everglow.Function/Mechanics/Quest/`.

**`Sources/Everglow.Function/Mechanics/Quest/`:**
- Purpose: Keep the player-side and world-side quest domains, presentation projection, and UI together while preserving their separate persistence/sync models.
- Contains: `Core/`, `PlayerSide/`, `WorldSide/`, `Presentation/`, `Hooks/`, `UI/`, and subsystem design documents.
- Key files: `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/PlayerQuestSystem.cs`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestSystem.cs`, `Sources/Everglow.Function/Mechanics/Quest/Presentation/QuestPresentationService.cs`, `Sources/Everglow.Function/Mechanics/Quest/UI/QuestContainer.cs`.

**`Sources/Modules/`:**
- Purpose: Host independently named content assemblies that inherit the common module build properties.
- Contains: active modules `AssetReplace`, `CagedDomain`, `EternalResolve`, `Example`, `Food`, `MEAC`, `Minortopography`, `Myth`, `Ocean`, `Plant`, `PlantAndFarm`, `SpellAndSkull`, `SubSpace`, and `Yggdrasil`, plus retired directories `IIID`, `TwilightForest`, and `ZY`.
- Key files: `Sources/Modules/Directory.Build.props`, each active `Everglow.<ModuleName>.csproj`, and module-local content directories.

**`Sources/Modules/<ModuleName>/`:**
- Purpose: Group one content assembly by module boundary and resource prefix.
- Contains: ordinary tML content grouped by gameplay/content type, optional `<ModuleName>Module.cs`, effects, music/sounds, JSON/MapIO inputs, and module-specific systems.
- Key files: `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, `Sources/Modules/Yggdrasil/YggdrasilModule.cs`, `Sources/Modules/Myth/MythModule.cs`, `Sources/Modules/SubSpace/RoomManager.cs`.

**`Sources/Modules/Yggdrasil/`:**
- Purpose: Contain the largest subworld/content area and its shared world-generation and multiplayer state.
- Contains: `Common/`, `WorldGeneration/`, `YggdrasilTown/`, `KelpCurtain/`, `CorruptWormHive/`, `HurricaneMaze/`, `GreenCore/`, `CityOfMagicFlute/`, `Netcode/`, `Effects/`, `Music/`, and `IOcclusionProjectile.cs`.
- Key files: `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs`, `Sources/Modules/Yggdrasil/Common/YggdrasilPlayer.cs`, `Sources/Modules/Yggdrasil/Netcode/`.

**`Sources/Everglow.UnitTests/`:**
- Purpose: Test the shared layers and selected module behavior without starting the full game loop.
- Contains: `Core/`, `Function/`, `Modules/`, `UnitTest.cs`, and test project configuration.
- Key files: `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`, `Sources/Everglow.UnitTests/Function/QuestSystem/`, `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs`, `Sources/Everglow.UnitTests/Modules/Yggdrasil/`.

**`Documents/`:**
- Purpose: Store repository workflow and subsystem design documentation.
- Contains: build instructions, subworld guidance, IL documentation, and images.
- Key files: `Documents/源代码编译流程.md`, `Documents/子世界.md`, `Documents/ILDoc/`.

**`Tools/`:**
- Purpose: Provide build tasks and developer utilities used outside the runtime assemblies.
- Contains: `Everglow.Tasks`, `Everglow.Scripts`, asset utilities, executable tools, and generator scripts.
- Key files: `Tools/Everglow.Tasks/`, `Tools/Everglow.Scripts/`, `Tools/FurnitureGenerator.mjs`.

**`Resources/` and `Libraries/`:**
- Purpose: Hold repository-wide resource inputs and required third-party/read-only binaries.
- Contains: packed/runtime inputs under `Resources/` and external assemblies such as `Libraries/SubworldLibrary.dll`.
- Key files: `Libraries/SubworldLibrary.dll`, resource subtrees under `Resources/`.

## Key File Locations

**Entry Points:**
- `Sources/Everglow/Everglow.cs`: tModLoader `Mod` lifecycle, service composition, reflected content registration, and packet dispatch.
- `Sources/Everglow.Function/Hooks/HookSystem.cs`: shared update/draw/world callback bridge.
- `Sources/Everglow.Function/UI/UISystem.cs`: shared UI lifecycle and interface-layer insertion.
- `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`: Yggdrasil subworld definition and generation entry.
- `Sources/Modules/SubSpace/RoomWorld.cs`: nested room subworld definition.

**Configuration:**
- `Sources/Directory.Build.props`: .NET 8 target, active `Modules` list, compilation symbols, path/resource flags, and package-wide additional-file rules.
- `Sources/Modules/Directory.Build.props`: inherited module references, `ModuleName`, `PathPrefix`, global usings, and library references.
- `Sources/Everglow/Everglow.csproj`: host project references and resource-list merge target.
- `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`: MSTest dependencies and test runtime DLL copying.
- `.github/workflows/build-and-test.yml`: CI build/test configuration that mirrors active module build settings.

**Core Logic:**
- `Sources/Everglow.Core/Ins.cs`: shared dependency container.
- `Sources/Everglow.Core/Modules/ModuleManager.cs`: reflection-based assembly/type/module registry.
- `Sources/Everglow.Function/Netcode/PacketResolver.cs`: packet discovery, IDs, handlers, and route resolution.
- `Sources/Everglow.Function/VFX/VFXManager.cs`: visual registration, update, pipeline ordering, and render targets.
- `Sources/Everglow.Function/Mechanics/Quest/`: shared quest domain and presentation boundary.
- `Sources/Modules/<ModuleName>/`: feature-specific gameplay and content implementation.

**Testing:**
- `Sources/Everglow.UnitTests/Core/`: Core utility tests.
- `Sources/Everglow.UnitTests/Function/`: Function infrastructure, physics, UI, netcode, and Quest tests.
- `Sources/Everglow.UnitTests/Function/QuestSystem/`: Quest tests organized by `PlayerSide/`, `WorldSide/`, `Presentation/`, and `UI/` boundaries.
- `Sources/Everglow.UnitTests/Modules/Yggdrasil/`: selected Yggdrasil tests.
- `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Tests/` and `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/Tests/`: in-mod sample/debug quest content, not MSTest locations.

## Naming Conventions

**Files:**
- Project files use `Everglow.<ModuleName>.csproj`, for example `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj`.
- Shared infrastructure uses descriptive PascalCase names such as `Sources/Everglow.Function/Netcode/PacketResolver.cs` and `Sources/Everglow.Core/Modules/ModuleManager.cs`.
- Partial implementations use a dot suffix for a focused concern, such as `WorldQuestBase.Persistence.cs` and `WorldQuestBase.Netcode.cs` under `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Abstractions/`.
- Content files generally match their class/content internal name, such as `Sources/Modules/Food/Tiles/Stove.cs` and `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`.

**Directories:**
- Shared layers use PascalCase capability names: `Sources/Everglow.Function/Netcode/`, `Sources/Everglow.Function/Mechanics/`, and `Sources/Everglow.Core/DataStructures/`.
- Modules use the assembly/module name: `Sources/Modules/Yggdrasil/` corresponds to `Everglow.Yggdrasil` and the runtime resource prefix `Everglow/Yggdrasil`.
- Module content is normally grouped by feature area and tML type, for example `Sources/Modules/Yggdrasil/YggdrasilTown/Items/`, `Tiles/`, `NPCs/`, and `VFXs/`.
- Retired directories remain under `Sources/Modules/IIID/`, `Sources/Modules/TwilightForest/`, and `Sources/Modules/ZY/`; do not use them for new code.

**Namespaces and types:**
- Core and Function use `Everglow.Commons` as the root namespace, as configured in `Sources/Everglow.Core/Everglow.Core.csproj` and `Sources/Everglow.Function/Everglow.Function.csproj`.
- Content modules use `Everglow.<ModuleName>`, illustrated by `Sources/Modules/Yggdrasil/YggdrasilModule.cs` and `Sources/Modules/Myth/MythModule.cs`.
- New content should follow `Everglow.<Module>[.<Area>[.<Type>]]` and keep matching assets adjacent to the owning source under `Sources/Modules/<ModuleName>/`.

## Where to Add New Code

**New Feature:**
- Primary code: Add the feature to the owning active module under `Sources/Modules/<ModuleName>/<Area>/`, using neighboring content categories as the placement guide; put shared behavior that serves multiple modules under `Sources/Everglow.Function/`.
- Tests: Add pure/shared tests under `Sources/Everglow.UnitTests/Function/` or module tests under `Sources/Everglow.UnitTests/Modules/<ModuleName>/`; mirror the Quest boundary under `Sources/Everglow.UnitTests/Function/QuestSystem/` when changing Quest behavior.
- Cross-module dependency: Add an explicit `ProjectReference` to the owning module `.csproj`, following `Sources/Modules/Myth/Everglow.Myth.csproj` or `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj`; do not rely on incidental assembly visibility.

**New Component/Module:**
- Implementation: Add an active module directory and `Everglow.<ModuleName>.csproj` under `Sources/Modules/`, inherit `Sources/Modules/Directory.Build.props`, and add the module name to `Sources/Directory.Build.props` only as an approved structural change.
- Lifecycle hooks: Add an `EverglowModule` only when assembly-wide hooks/effects/events are required, following `Sources/Modules/Myth/MythModule.cs`; ordinary `ModItem`, `ModNPC`, `ModProjectile`, `ModTile`, and `ModSystem` types belong directly in the module content tree.
- Subworld: Put the subworld class beside its module entry or in `WorldGeneration/`, and put generation passes/helpers in `Sources/Modules/<ModuleName>/WorldGeneration/`, following `Sources/Modules/Yggdrasil/WorldGeneration/`.
- Shared service: Define a tML-independent interface in `Sources/Everglow.Core/Interfaces/`, then implement/register it in `Sources/Everglow.Function/` and compose it in `Sources/Everglow/Everglow.cs`.

**Utilities:**
- Shared low-level helper: `Sources/Everglow.Core/Utilities/`, `DataStructures/`, `Coroutines/`, `ObjectPool/`, or `Vertex/`, depending on the dependency direction and whether tML is required.
- Shared tML/gameplay helper: `Sources/Everglow.Function/Utilities/`, `Graphics/`, `TileHelper/`, `Physics/`, or the relevant `Mechanics/` subsystem.
- Module-only helper: Keep it in `Sources/Modules/<ModuleName>/Common/` or the owning feature area; do not promote region-specific implementation into a shared directory without a real consumer boundary.

**Quest code:**
- Domain type: Use `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/` or `WorldSide/` and preserve the separate manager/state/persistence/netcode models.
- Presentation: Use `Sources/Everglow.Function/Mechanics/Quest/Presentation/Views/`, `Adapters/`, `Icons/`, or the service/action files at the presentation root.
- UI: Use `Sources/Everglow.Function/Mechanics/Quest/UI/` and subscribe through `QuestPresentationSystem`; do not place MSTest files in the in-mod `Tests/` directories.

**Localization and assets:**
- New content localization: Maintain both `Sources/Everglow/Localization/en-US/` and `Sources/Everglow/Localization/zh-Hans/` using the repository exporter workflow.
- Module assets: Keep assets beside the relevant `.cs` implementation under `Sources/Modules/<ModuleName>/`; use generated `ModAsset` members and the module `PathPrefix` from `Sources/Modules/Directory.Build.props`.
- Nonstandard packed input: Update the owning `.csproj`/build rule only after checking `Documents/源代码编译流程.md`; resource-packing changes are structural.

## Special Directories

**`Sources/*/bin/` and `Sources/*/obj/`:**
- Purpose: MSBuild outputs and intermediates for host, shared, test, and module projects.
- Generated: Yes.
- Committed: No; treat as disposable build output and do not add source there.

**`Sources/Modules/IIID/`, `Sources/Modules/TwilightForest/`, `Sources/Modules/ZY/`, and `Sources/Everglow.Scripts/`:**
- Purpose: Retained deprecated/legacy source trees.
- Generated: No.
- Committed: Existing legacy source may be present, but these paths are not active module targets and are not places for new code.

**`Resources/`:**
- Purpose: Shared source resource inputs consumed by the mod/build pipeline.
- Generated: No.
- Committed: Yes, as repository-managed inputs; existing art/resource files are read-only for normal code changes.

**`Libraries/`:**
- Purpose: Required external assemblies, including `Libraries/SubworldLibrary.dll`.
- Generated: No.
- Committed: Yes and read-only; do not modify third-party binaries.

**`Tools/`:**
- Purpose: Build tasks, generated helper assemblies, asset utilities, and developer scripts.
- Generated: Mixed; `Tools/Everglow.Tasks/` and `Tools/Everglow.Scripts/` are source/tool projects, while executable and DLL outputs are built artifacts or shipped utilities.
- Committed: Existing tools are repository-managed; do not alter binaries as part of feature code.

**`Sources/Everglow/Localization/`:**
- Purpose: Active mod localization source for English and Simplified Chinese.
- Generated: Source-managed, with exporter-assisted key generation.
- Committed: Yes; localization keys are additive and the legacy root `Localization/` is not the target for new content keys.

**`.planning/codebase/`:**
- Purpose: GSD-generated architecture, structure, technology, integration, quality, and concerns references.
- Generated: Yes by mapping workflows.
- Committed: Planning artifacts are repository documents; update only the focused mapper documents when refreshing the map.

---

*Structure analysis: 2026-09-11*
