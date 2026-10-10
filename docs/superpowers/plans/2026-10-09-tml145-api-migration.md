# Terraria 1.4.5 API Migration Implementation Plan

> **For agentic workers:** Use executing-plans for sequential migration and dispatching-parallel-agents for independent manual fixes. Git operations require explicit user authorization.

**Goal:** Adapt active Everglow source to the locally built tML 1.4.5 API, preserving gameplay intent and reporting remaining external/runtime limits.
**Architecture:** Run official tModPorter per active project, inspect its edits and guidance, then fix unsupported API changes against the pinned tML source. Keep all builds/deployment in the isolated D: environment.
**Tech Stack:** C#, .NET 10, FNA, tML 1.4.5 a9fd7502, MSTest.
**Spec:** User request to fix migration using tML's MigrationGuide_1.4.5.md; repository AGENTS.md.

## Global Constraints

- Preserve existing framework edits and unrelated user changes; stage, commit, or push only with user authorization.
- Never modify inactive modules, binary/art assets or external dependencies.
- Preserve content names, localization keys, quest save/network semantics and client/server guards.
- Build only against independent 1.4.5 output; deploy only to UserData/Mods.

## Review Focus

- Projectile draw ownership/layers must preserve rendering intent, including held projectiles.
- WorldItem migration must retain Item payload and pickup/reward semantics.
- NPC interactions must preserve shop, quest and dialogue actions.
- Changed hooks/IL must retain dedicated-server guards and failures must remain visible.
- Third-party dependencies may still target 1.4.4; do not hide that limitation.

## Tasks

- [x] 1. Run official tModPorter for Function and active module projects; inspect changed files and comments, normalize only edited C# files to UTF-8 without BOM/LF.
- [x] 2. Repair remaining Function API changes (projectile hooks, WorldItem/pickup, NPC interactions, collision/render hooks). Use existing tests for unchanged rules; add focused tests only where migration alters an observable custom boundary.
- [x] 3. Build, then repair active module API failures using the same official guide and source. Do not disable content to make compilation pass.
- [x] 4. Run dotnet build, dotnet test after successful build, byte-level BOM check, and review diff. Report graphics, multiplayer and dependency compatibility not verified in-game.

## Execution Notes

- Initial failure: 35 Function compiler errors against 1.4.5. Environment references and isolated output were verified in prior turn.
- Compiler failures provide the regression baseline for API signature migrations; no tests duplicating tML APIs.

## Verification Results

- User approved the two-line Spine namespace qualification. Skin.cs now explicitly uses Spine.Collections.OrderedDictionary; no other third-party library source was changed.
- `dotnet build --no-restore /p:WarningLevel=0 --verbosity quiet` succeeded with zero errors. It produced `D:/CodeDocs/TML/1.4.5/UserData/Mods/Everglow.tmod` and enabled the mod in that isolated directory.
- The first test run passed 346/357; all 11 failures were missing TerrariaHooks or MonoMod.Utils runtime assemblies. The existing test project manually copies only some tML DLLs, while tML defaults OutputTmlReferences to false.
- Using tML's existing reference-copy switch passed all 357 tests (zero failed/skipped): `dotnet test Sources/Everglow.UnitTests/Everglow.UnitTests.csproj --no-restore /p:OutputTmlReferences=true /p:BuildProjectReferences=false /p:WarningLevel=0`. This command is run after the complete solution build. No project/build configuration was changed to persist this switch.
- Passed regressions include pickup, special-shop item protection, armor combinations, legacy/versioned map sections, variable chest capacity, tall-world map caches/IL anchors, and continuous difficulty exemptions.
- Original 1.4.4 tML DLL, deployed Everglow.tmod, and enabled.json hashes remain unchanged. No binary/art asset or inactive module was modified. The linked worktree remains on 1.4.5-migration. Pure API adaptations were split into seven commits at the user's request; the user subsequently authorized committing and pushing the remaining migration work.
- Everglow's existing Terraria/TerrariaServer launch profiles resolve to the independently built tML runtime via the local tModLoader.targets. Client graphics, live mod loading, world transitions, armor lifecycle, and multiplayer behavior have not been validated in game; build/test success does not establish full runtime compatibility.

## In-memory Import Check (2026-10-10)

A standalone .NET 10 process used the pinned tML runtime's real TmodFile reader and AssemblyManager.ModLoadContext. It did not execute Everglow.Load, start the game, or modify enabled-mod lists.

- Package SHA-256: C4DB70887296C9B138438C533C92E48A2D90256F400D79D2103B17B1974473B9.
- Official hash validation passed; all 6,814 entries could be read/decompressed (293,587,450 bytes total).
- 29 assemblies loaded into memory before type discovery failed.
- Type discovery failed at Everglow.Commons.Mechanics.Quest.WorldSide.WorldQuestSystem because SubworldLibrary was unavailable. The isolated Mods directory lacks both declared mod dependencies: SubworldLibrary and ModLiquidLib.
- This establishes package readability and assembly loading, not successful complete mod import. Neither dependency's 1.4.5 runtime compatibility has been established by this check.
- The user chose to stop runtime migration here pending compatible dependencies. After installing verified 1.4.5-compatible dependency packages, repeat import/type discovery, then test the Everglow client/server launch profiles and actual game behavior.

The temporary import probe and machine-specific tModLoader.targets remain outside this repository. The local tML installation, save data, build outputs and .tmod package are not repository source artifacts.

## CI Follow-up

The workflow itself still targets the existing branches, installs .NET 8, and downloads the latest released tML. The mirrored Directory.Build.props now matches the migration's .NET 10 target, but CI has not been adapted or validated for this branch. Before enabling migration CI, configure .NET 10, the intended 1.4.5 tML build, isolated deployment and test runtime references.
