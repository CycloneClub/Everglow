<!-- refreshed: 2026-09-11 -->
# Coding Conventions

**Analysis Date:** 2026-09-11

## Naming Patterns

**Files:**
- Name production files after their primary type, such as `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs` and `Sources/Everglow.Function/Netcode/PacketResolver.cs`.
- Use suffixes that identify the role or lifecycle boundary, such as `*System.cs`, `*Manager.cs`, `*Module.cs`, `*Packet.cs`, and `*Test.cs`; split large partial implementations with descriptive suffixes such as `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs` and `Sources/Everglow.Function/Netcode/PacketResolver.Send.cs`.
- Keep tests in the separate `Sources/Everglow.UnitTests/` tree. Quest tests use the production-boundary directories under `Sources/Everglow.UnitTests/Function/QuestSystem/`, while the namespace remains `Everglow.UnitTests.Function.QuestSystem` as specified by `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md`.

**Functions:**
- Use PascalCase for methods and overrides, including tModLoader callbacks such as `Sources/Modules/Example/Test/HandholdItem.cs:13` (`SetDefaults`) and `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:23` (`Update`).
- Prefer behavior-oriented names for tests, generally in `Action_ExpectedResult` form, such as `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs:17` and `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/PlayerQuestActionTest.cs:102`.
- Use expression-bodied members for simple accessors or one-line operations, as in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:19` and `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:18`.

**Variables:**
- Use camelCase for locals and parameters, for example `timeLimit`, `elapsedFrames`, and `objectiveId` in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs`.
- Prefer descriptive locals over abbreviations at public or protocol boundaries, as shown by `destination`, `logicalSource`, and `transportSender` in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:179`.
- Private-field naming is mixed in existing code: newer code uses `_manager` and `_originalNetMode` in `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/PlayerQuestActionTest.cs:12` and `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:16`, while older infrastructure uses `m_typeToIdMapping` in `Sources/Everglow.Core/Modules/DependencyGraph.cs:6` and lower-camel fields in `Sources/Everglow.Function/Netcode/PacketResolver.cs:64`. For new code, use the underscore-prefixed style used by adjacent modern code and do not rename unrelated legacy fields.

**Types:**
- Use PascalCase for classes, structs, enums, delegates, properties, events, and methods; the analyzer rules in `/.editorconfig:214-236` make these naming violations warnings.
- Prefix interfaces with `I`, such as `IModule` in `Sources/Everglow.Core/Modules/IModule.cs` and `IGameStateProvider` in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Abstractions/IGameStateProvider.cs`; `/.editorconfig:210-212` enforces the prefix.
- Keep namespaces file-scoped and aligned with the directory/module boundary, for example `Everglow.Commons.Mechanics.Quest.Core` in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:1` and `Everglow.Example.Test` in `Sources/Modules/Example/Test/HandholdItem.cs:4`.
- Use nested `private sealed` test types for local production substitutes, as in `StubQuest` and `StubObjective` in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/Adapters/PlayerQuestViewAdapter/PlayerQuestViewAdapterTest.cs:16` and `:55`.

## Code Style

**Formatting:**
- Use the repository root `/.editorconfig:5-20`: tab indentation with width 4, LF line endings, UTF-8 without BOM, and a final newline. Keep `.sln` and `.csproj` files as CRLF under `/.editorconfig:31-34`.
- Use Allman braces and braces on all control-flow blocks; `/.editorconfig:161-172` requires new lines before open braces and enables braces for embedded statements. `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:7-10` is a representative implementation.
- Use file-scoped namespaces, configured at `/.editorconfig:129-133`; do not introduce block-scoped namespaces in new C# files.
- Prefer modern C# collection expressions and target-typed construction where the surrounding code uses them, such as `[]` in `Sources/Everglow.Function/Netcode/PacketResolver.cs:75-78` and `new()` in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs`.
- Follow the operator, comma, parenthesis, and wrapping rules in `/.editorconfig:178-204`; preserve existing local formatting instead of applying unrelated repository-wide reformatting, as required by `AGENTS.md:84-85`.

**Linting:**
- StyleCop analyzers are supplied by `Sources/Directory.Build.props:23-27`, but documentation and ordering diagnostics are explicitly disabled in `/.editorconfig:292-384`; do not assume XML documentation or member-order warnings are enforced.
- Treat accessibility modifiers as required for non-interface members (`/.editorconfig:61-63`) and use explicit modifiers in production code such as `Sources/Everglow.Core/Modules/ModuleManager.cs:5-11`.
- Keep `using` directives outside namespaces and sort `System` directives first; these preferences are declared in `/.editorconfig:35-43` and are visible in `Sources/Everglow.Function/Hooks/HookSystem.cs:1-7`.
- The build uses nullable reference types explicitly for tests in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:3-8`, while shared production properties enable implicit usings and preview C# in `Sources/Directory.Build.props:3-7`.

## Import Organization

**Order:**
1. `System` and framework namespaces first when present, as in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:1-2`.
2. Project namespaces next, grouped by `Everglow.*`, as in `Sources/Everglow/Everglow.cs:2-9` and `Sources/Everglow.Function/Hooks/HookSystem.cs:1-3`.
3. External/game namespaces after project imports, such as `Terraria.*`, `Microsoft.Xna.Framework.*`, `MonoMod.*`, and `SubworldLibrary`, as in `Sources/Modules/Yggdrasil/YggdrasilModule.cs:1-6`.
- Do not add blank import groups solely for stylistic separation: `dotnet_separate_import_directive_groups = false` is set in `/.editorconfig:40-42`.

**Path Aliases:**
- No C# `global using` alias scheme is detected. Test-wide MSTest imports come from `Sources/Everglow.UnitTests/Usings.cs:1`, and module-wide game/project imports come from `Sources/Modules/Directory.Build.props:14-20`.
- Use the inherited `Everglow.Commons` root namespace and module project usings rather than duplicating the same references in module project files, per `Sources/Modules/Directory.Build.props:8-21` and `AGENTS.md:68-72`.

## Error Handling

**Patterns:**
- Validate constructor and public-boundary arguments immediately with a specific exception and parameter/value context, as in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:7-10` and `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:13-16`.
- Use `ArgumentNullException` for null dependencies, `ArgumentOutOfRangeException` for invalid numeric input, `InvalidDataException` for malformed quest structures or persisted data, and `InvalidOperationException` for invalid lifecycle/state transitions; examples are in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Structure/Nodes/WorldLeafNode.cs:13-17`, `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Abstractions/WorldQuestBase.Behavior.cs:347-353`, and `Sources/Everglow.Function/Mechanics/Quest/Presentation/Adapters/WorldQuestViewAdapter.cs:84-88`.
- Use switch-expression throw arms to reject unknown enum/type states rather than silently choosing a default, as in `Sources/Everglow.Function/Mechanics/Quest/Presentation/Adapters/PlayerQuestViewAdapter.cs:80-86` and `:171-176`.
- Return `false`, `null`, or an empty collection when the API defines an expected absence or rejected action, as in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:38-77` and `Sources/Everglow.Function/Mechanics/Quest/Presentation/Adapters/PlayerQuestActionAdapter.cs:11-17`; document or preserve that sentinel contract.
- Reserve broad `Exception` throws for load/configuration or legacy utility paths only; current examples include `Sources/Everglow/Everglow.cs:60-67` and `Sources/Everglow.Function/TileHelper/WorldGenMisc.cs:37-42`. Prefer a more specific exception for new reusable logic.
- Use `Debug.Assert` for internal invariants that indicate programmer misuse, as in `Sources/Everglow.Core/Ins.cs:46-51` and `Sources/Everglow.Core/VFX/VFXBatch.cs:46-65`; do not use assertions as user-facing validation.
- Hook dispatch catches handler failures, logs the exception, and disables the failing handler so the remaining hook pipeline continues; follow `Sources/Everglow.Function/Hooks/HookSystem.cs:19-34` and `:164-181` for this boundary-specific recovery pattern.

## Logging

**Framework:** `log4net` is exposed through the service locator as `Ins.Logger` in `Sources/Everglow.Core/Ins.cs:23-26`; it is registered from the mod logger in `Sources/Everglow/Everglow.cs:28-32`.

**Patterns:**
- Use `Ins.Logger.Info`, `Warn`, `Debug`, and `Error` for runtime diagnostics, with contextual identifiers and state, as in `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs:37-73`, `Sources/Everglow.Function/Netcode/PacketResolver.cs:130-142`, and `Sources/Everglow.Function/Mechanics/Cooldown/CooldownInstance.cs:70-82`.
- Log hook exceptions with the handler identity and exception details before disabling the handler, as in `Sources/Everglow.Function/Hooks/HookSystem.cs:28-33`.
- Use `Main.NewText` or `Main.instance.MouseText` for deliberate in-game player/developer feedback, not for server diagnostics; examples are in `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/PlayerQuestManager.cs:173-225` and `Sources/Modules/Example/Test/SoundIDPlayItem.cs:27-45`.
- `Console.WriteLine` remains in developer tools, localization exporters, and some older quest/network paths, such as `Sources/Everglow.Function/Localization/ExportHjson.cs:84-152` and `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestManager.cs:275-286`; use `Ins.Logger` for new mod runtime diagnostics unless console output is specifically a tool/UI requirement.
- `Debug.Assert` and `Debug.Fail` are used for development-time invariant failures in graphics and hook code, for example `Sources/Everglow.Core/VFX/VFXBatch.cs:438-545` and `Sources/Everglow.Function/Hooks/HookSystem.cs:28-31`.

## Comments

**When to Comment:**
- Comment non-obvious engine or protocol constraints, such as the headless `TagCompound` workaround in `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Abstractions/PlayerQuestBasePersistenceTest.cs:26-33` and the packet routing contract in `Sources/Everglow.Function/Netcode/PacketResolver.cs:5-57`.
- Keep comments close to the code they explain; gameplay and rendering code contains both English and Chinese comments, for example `Sources/Everglow.Core/Modules/DependencyGraph.cs:5-10` and `Sources/Modules/Yggdrasil/YggdrasilModule.cs:126-143`.
- Use TODO/debug comments sparingly and avoid leaving commented-out runtime behavior in new code; existing examples include `Sources/Everglow.Core/Modules/DependencyGraph.cs:108` and `Sources/Modules/Yggdrasil/YggdrasilModule.cs:126`.

**JSDoc/TSDoc:**
- Use XML documentation for public reusable APIs, protocol enums, and exception contracts, as in `Sources/Everglow.Function/Netcode/PacketResolver.cs:5-8`, `:11-57`, and `Sources/Everglow.Core/Utilities/GraphicsUtils.cs:13-18`.
- XML documentation is not mandatory at analyzer level because `SA1600` and the documentation-rule category are disabled in `/.editorconfig:292-304`; document behavior where callers need a contract rather than adding boilerplate to every content override.

## Function Design

**Size:**
- Keep pure domain methods compact and explicit, with validation followed by state transition, as in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:23-41`.
- Large lifecycle/rendering methods exist where the engine callback requires orchestration, such as `Sources/Modules/Yggdrasil/YggdrasilModule.cs:145-212`; split new behavior into private helpers when it introduces an independent boundary, as `DrawOcclusion`, `DrawEffect`, and `GetOrig` do at `:214-274`.

**Parameters:**
- Pass required collaborators through constructors and reject null dependencies, as in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:11-16`.
- Use named arguments for semantically sensitive protocol/game-state calls, as in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:101-105` and `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:93-96`.
- Use small private helpers to hide reflection or protocol plumbing from assertions/callers, as in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:179-206`.

**Return Values:**
- Prefer typed results and immutable/read-only views at service boundaries, such as `IReadOnlyList<QuestActionType>` in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestActions.cs:18-36`.
- Use `out` results when a missing result is normal and should not throw, as in `Sources/Everglow.Function/Utilities/GraphicsUtils.cs:139-143`; use exceptions when the input violates a documented invariant, as in `Sources/Everglow.Function/Mechanics/Quest/Core/QuestTimer.cs:23-27`.
- Preserve stream alignment and explicit serialization order in binary methods; tests in `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/Abstractions/WorldObjectiveTimerTest.cs:109-135` show the expected contract.

## Module Design

**Exports:**
- Core abstractions and services live under `Sources/Everglow.Core/` and avoid Terraria/tModLoader dependencies; tModLoader-facing implementations live under `Sources/Everglow.Function/` and content under `Sources/Modules/`, as summarized in `AGENTS.md:61-72`.
- Register shared services through `Ins.Add` during mod load and retrieve them through `Ins`, following `Sources/Everglow/Everglow.cs:28-51` and `Sources/Everglow.Core/Ins.cs:37-63`.
- Let normal `ModItem`, `ModNPC`, `ModProjectile`, `ModTile`, and `ModSystem` classes be discovered by the module scan rather than manually registering each content type; the composition boundary is `Sources/Everglow/Everglow.cs:54-86` and the repository rule is `AGENTS.md:68-70`.

**Barrel Files:**
- No barrel-file convention is detected. Files expose their types directly through namespaces; shared test imports use only `Sources/Everglow.UnitTests/Usings.cs:1`, and module imports are supplied by `Sources/Modules/Directory.Build.props:14-20`.
- Use partial classes only for cohesive behavioral splits, such as `Sources/Everglow.Function/Netcode/PacketResolver.cs`, `PacketResolver.Resolve.cs`, and `PacketResolver.Send.cs`, or the behavior-split quest production files under `Sources/Everglow.Function/Mechanics/Quest/`.

---

*Convention analysis: 2026-09-11*
