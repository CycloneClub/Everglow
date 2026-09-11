<!-- refreshed: 2026-09-11 -->
# Testing Patterns

**Analysis Date:** 2026-09-11

## Test Framework

**Runner:**
- MSTest 3.10.2 is used through `MSTest.TestFramework` and `MSTest.TestAdapter` in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:10-14`.
- The Microsoft test host is `Microsoft.NET.Test.Sdk` 17.14.1 in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:10-13`.
- There is no test `.runsettings`, NUnit/xUnit configuration, or separate test configuration detected; the test project is configured directly in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`.

**Assertion Library:**
- Use MSTest `Assert`, `CollectionAssert`, and `TestContext` from the global import in `Sources/Everglow.UnitTests/Usings.cs:1`.
- Common assertions are `AreEqual`, `AreNotEqual`, `AreSame`, `IsTrue`, `IsFalse`, `IsNull`, `IsNotNull`, `IsEmpty`, `HasCount`, `IsInstanceOfType`, `CollectionAssert.AreEqual`, and `ThrowsExactly`, as demonstrated in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:38-71` and `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs:8-65`.
- Prefer exact exception assertions for validation contracts, as in `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Objectives/TalkNPCObjectiveTest.cs:8-22`; assert public state and returned values rather than private implementation details when possible.

**Run Commands:**
```bash
dotnet restore                                      # Restore dependencies from the repository root
dotnet build                                        # Build all projects and the tModLoader mod
dotnet test --verbosity normal /p:WarningLevel=0   # Run the full test suite, matching CI
dotnet test --filter "FullyQualifiedName~QuestSystem" # Run Quest System tests
dotnet test --collect:"XPlat Code Coverage"       # Collect coverlet data when coverage is needed
```
- The full build/test sequence and Quest filter are prescribed by `AGENTS.md:44-55` and `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:23-28`; CI runs `dotnet build /p:Configuration=Release /p:WarningLevel=0` and `dotnet test --verbosity normal /p:WarningLevel=0` in `.github/workflows/build-and-test.yml:115-119`.
- Tests run on Windows in CI (`windows-latest`) with .NET 8 in `.github/workflows/build-and-test.yml:12-16` and `:40-45`.

## Test File Organization

**Location:**
- Tests are separate from production files under `Sources/Everglow.UnitTests/`, organized by subsystem: `Core/`, `Function/`, and `Modules/Yggdrasil/`, as shown by `Sources/Everglow.UnitTests/Core/GraphicsUtilsTest.cs`, `Sources/Everglow.UnitTests/Function/Physics/ColliderTest.cs`, and `Sources/Everglow.UnitTests/Modules/Yggdrasil/Common/YggdrasilPlayerTests.cs`.
- Quest tests mirror meaningful production boundaries under `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/`, `WorldSide/`, `Presentation/`, and `UI/`, while all Quest test classes use the namespace `Everglow.UnitTests.Function.QuestSystem` per `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:13-18`.
- `Sources/Everglow.Function/Mechanics/Quest/WorldSide/Tests/` and `PlayerSide/Tests/` contain in-game example/debug content rather than MSTest files, as explicitly distinguished by `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:15-16`.

**Naming:**
- Name test files after the primary subject with a `Test` or `Tests` suffix, for example `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs` and `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs`.
- Name test methods after the observable behavior, commonly `Method_Condition_ExpectedResult`, such as `Update_ClampsAtLimitAndReportsOnlyFirstExpiration` in `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs:17` and `TryExecute_DispatchesAndRevalidatesAction` in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs:178`.
- When one type has many behavior slices, use partial test files with descriptive suffixes, such as `WorldQuestViewAdapter.ObjectiveTests.cs`, `WorldQuestViewAdapter.TimerTests.cs`, and `WorldQuestViewAdapter.SafetyTests.cs` under `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/Adapters/WorldQuestViewAdapter/`.

**Structure:**
```text
Sources/Everglow.UnitTests/
├── Core/                         # Terraria-independent utility tests
├── Function/                     # Function-layer physics, UI, netcode, and Quest tests
│   └── QuestSystem/
│       ├── PlayerSide/
│       ├── WorldSide/
│       ├── Presentation/
│       └── UI/
└── Modules/Yggdrasil/            # Module-specific behavior tests
```
- This layout is represented by `Sources/Everglow.UnitTests/Core/MathUtils/MathUtils.VectorsTest.cs`, `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/PlayerQuestActionTest.cs`, and `Sources/Everglow.UnitTests/Modules/Yggdrasil/Common/YggdrasilPlayerTests.cs`.

## Test Structure

**Suite Organization:**
```csharp
[TestClass]
[DoNotParallelize]
public class WorldObjectiveRetryNetworkTest
{
    [TestInitialize]
    public void Initialize() { /* reset shared Terraria state */ }

    [TestCleanup]
    public void Cleanup() { /* restore shared state */ }

    [TestMethod]
    public void SinglePlayerAction_AppliesImmediatelyAndPublishesOnce()
    {
        // arrange, act, assert
    }
}
```
- The concrete setup/cleanup and behavior test are in `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:12-15`, `:90-103`, and `:104-123`.

**Patterns:**
- Use one `[TestClass]` per subject and `[TestMethod]` per observable behavior, as in `Sources/Everglow.UnitTests/Function/Physics/ColliderTest.cs:8-68`.
- Use `[DataRow]` for compact input/state matrices instead of duplicating a test body, as in `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs:8-14`, `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Abstractions/PlayerQuestBasePersistenceTest.cs:16-22`, and `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/Adapters/PlayerQuestViewAdapter/PlayerQuestViewAdapterTest.cs:149-161`.
- Arrange local stubs and inputs, perform the operation, then assert externally visible state, events, serialized data, or presentation views. `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/PlayerQuestActionTest.cs:119-137` is a representative state/event test.
- Keep production-facing tests focused on Everglow behavior rather than tModLoader/FNA defaults, following the decision rule in `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:5-11`.

## Mocking

**Framework:**
- No mocking framework package is present in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:10-18`; tests use hand-written stubs, fake providers, direct in-memory streams, and local managers.

**Patterns:**
```csharp
private sealed class StubGameStateProvider : IGameStateProvider
{
    public double TimeForVisualEffects => 0;
    public bool GameMenu => false;
    public bool GameInactive => false;
    public bool GamePaused => false;
}
```
- The actual provider is in `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:79-88`; use the smallest private stub needed by the subject.
- For polymorphic domain objects, derive a private sealed test type and override only the relevant contract, as `StubQuest`, `StubObjective`, and `StubSource` do in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/Adapters/PlayerQuestViewAdapter/PlayerQuestViewAdapterTest.cs:16-102`.
- Use reflection only where a public boundary cannot express a pre-existing contract, such as private packet helpers in `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs:179-206` and shared tModLoader state in `Sources/Everglow.UnitTests/Modules/Yggdrasil/Common/YggdrasilPlayerTests.cs:14-35`.

**What to Mock:**
- Replace game-state providers, quest objects, packet transport boundaries, and external mutable state with local substitutes, as required by `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:17-20` and demonstrated in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs:81-90`.
- Exercise packet serialization directly with `MemoryStream`, `BinaryWriter`, and `BinaryReader` instead of starting multiplayer, as in `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/Abstractions/WorldObjectiveTimerTest.cs:71-93`.

**What NOT to Mock:**
- Do not mock or test normal tModLoader/FNA behavior; isolate Everglow state transitions and calculations instead, per `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:10-11`.
- Do not access real multiplayer, third-party networks, credentials, or variable external data in automation; the repository rule is `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:19-20`.

## Fixtures and Factories

**Test Data:**
```csharp
var quest = new StubQuest
{
    State = PlayerQuestState.Accepted,
    CompleteValue = true,
};
_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single();
```
- This inline, scenario-specific fixture pattern appears in `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/PlayerQuestActionTest.cs:101-110`; tests generally avoid shared global fixture files.
- Reusable construction helpers are private static methods within the owning test class, such as `CreateService` in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs:244-262` and `CreateManager`/`AddExpiredQuest` in `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:201-214`.
- Serialization fixtures use `TagCompound` or binary streams created in the test, as in `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Abstractions/PlayerQuestBasePersistenceTest.cs:21-44` and `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/Abstractions/WorldObjectiveTimerTest.cs:109-135`.

**Location:**
- No shared fixture/factory directory or test data repository is detected. Keep small helpers private to the test class unless multiple neighboring suites genuinely share the same substitute; this matches `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs:244-262` and the local-stub guidance in `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:16-18`.

## Coverage

**Requirements:**
- No numeric coverage target or coverage gate is enforced in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj` or `.github/workflows/build-and-test.yml:115-119`.
- `coverlet.collector` 6.0.4 is installed as a private test dependency in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:14-17`, but CI does not collect or publish a coverage report in `.github/workflows/build-and-test.yml`.

**View Coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage"   # Collect coverlet output for a local inspection
```
- This command is supported by the collector package in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj:14-17`; no checked-in report path or publishing step is configured in `.github/workflows/build-and-test.yml`.

## Test Types

**Unit Tests:**
- Most tests exercise pure or nearly pure Everglow logic: timers, geometry, UI string parsing, action validation, adapters, persistence, and packet wire layout. Examples are `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs`, `Sources/Everglow.UnitTests/Function/Physics/ColliderTest.cs`, and `Sources/Everglow.UnitTests/Function/Netcode/PacketResolverTest.cs`.
- Domain behavior is tested through local derived classes and public contracts, as in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/Adapters/PlayerQuestViewAdapter/PlayerQuestViewAdapterTest.cs:104-271`.

**Integration Tests:**
- Headless integration-style tests touch Terraria static state, tModLoader content objects, or module state while avoiding graphics/game-loop startup. Examples include `Sources/Everglow.UnitTests/Function/AssetUtilsTest.cs:6-34` and `Sources/Everglow.UnitTests/Modules/Yggdrasil/Common/YggdrasilPlayerTests.cs:20-35`.
- Persistence and serialization tests cross Everglow models with tModLoader `TagCompound` or binary protocol boundaries, as in `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Abstractions/PlayerQuestBasePersistenceTest.cs:21-79` and `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/Abstractions/WorldObjectiveTimerTest.cs:47-135`.

**E2E Tests:**
- No end-to-end or UI automation framework is detected. Rendering implementations such as `Sources/Modules/Yggdrasil/YggdrasilModule.cs:145-274` require client/tModLoader runtime verification rather than the MSTest suite, consistent with `Sources/Modules/Yggdrasil/AGENTS.md:36-42`.

## Common Patterns

**Async Testing:**
```text
Not detected in Sources/Everglow.UnitTests/**/*.cs.
```
- The current tests are synchronous; engine/network behavior is represented by direct method calls, state changes, and in-memory serialization, such as `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:104-199`.

**Error Testing:**
```csharp
[TestMethod]
public void Constructor_RejectsNonPositiveLimit()
{
    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new QuestTimer(0));
}
```
- Use exact typed exception assertions with invalid boundary inputs, as in `Sources/Everglow.UnitTests/Function/QuestSystem/Core/QuestTimerTest.cs:8-14` and `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Objectives/TalkNPCObjectiveTest.cs:8-22`.
- Test invalid network identities and malformed action arguments as rejected results with no mutation, as in `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs:228-242` and `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:140-170`.
- For headless tModLoader incompatibilities, narrowly catch the known `IOException`, document why, and continue asserting the relevant serialized values; examples are `Sources/Everglow.UnitTests/Function/QuestSystem/PlayerSide/Abstractions/PlayerQuestBasePersistenceTest.cs:26-38` and `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/Abstractions/WorldObjectiveTimerTest.cs:47-69`.

**Shared State and Cleanup:**
- Before a test first touches `Terraria.Main`, set `Program.SavePath = string.Empty` in `[TestInitialize]`, as in `Sources/Everglow.UnitTests/Function/AssetUtilsTest.cs:12-18` and `Sources/Everglow.UnitTests/Function/QuestSystem/WorldSide/WorldObjectiveRetryNetworkTest.cs:90-96`.
- Mark classes that mutate shared static game state with `[DoNotParallelize]`, save the original value, and restore it in `[TestCleanup]`; examples are `Sources/Everglow.UnitTests/Modules/Yggdrasil/Common/YggdrasilPlayerTests.cs:10-35` and `Sources/Everglow.UnitTests/Function/QuestSystem/Presentation/QuestPresentationServiceTest.cs:15-104`.
- Do not construct `Main`, start content loading, or run the game loop from tests; the constraint is stated in `AGENTS.md:101-104` and `Sources/Everglow.Function/Mechanics/Quest/AGENTS.md:19-21`.

---

*Testing analysis: 2026-09-11*
