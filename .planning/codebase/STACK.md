# Technology Stack

**Analysis Date:** 2026-09-11

## Languages

**Primary:**
- C# - gameplay, tModLoader integration, shared infrastructure, UI, VFX, networking, and tests across `Sources/Everglow/`, `Sources/Everglow.Core/`, `Sources/Everglow.Function/`, `Sources/Modules/`, and `Sources/Everglow.UnitTests/`.
- C# - custom MSBuild tasks targeting `netstandard2.0` in `Tools/Everglow.Tasks/`.
- C# - standalone developer scripts targeting `net6.0` in `Tools/Everglow.Scripts/`.

**Secondary:**
- HLSL/XNA effect source (`.fx`) - shader and post-processing code under `Sources/Everglow.Function/VFX/` and active module VFX/effect directories; the build enables effect compilation in `Sources/Directory.Build.props`.
- HJSON - player-facing Terraria localization under `Sources/Everglow/Localization/en-US/` and `Sources/Everglow/Localization/zh-Hans/`.
- JSON and Spine/atlas asset data - bundled content under `Sources/Modules/Example/Skeleton/` and `Sources/Modules/Yggdrasil/`.
- JavaScript modules - the furniture generation utility in `Tools/FurnitureGenerator.mjs`.
- Binary content formats including PNG, BMP, MP3, OGG, TTF, OBJ, and MAPIO - game assets and world-generation inputs under `Sources/` and `Resources/`.

## Runtime

**Environment:**
- .NET 8 (`net8.0`) - all main mod, shared, module, and unit-test projects inherit or specify this target through `Sources/Directory.Build.props` and the project files under `Sources/`.
- .NET Standard 2.0 - MSBuild task assembly in `Tools/Everglow.Tasks/Everglow.Tasks.csproj`.
- .NET 6 (`net6.0`) - executable developer script assembly in `Tools/Everglow.Scripts/Everglow.Scripts.csproj`.
- Terraria 1.4.4.9 through tModLoader - runtime host and mod API documented in `README.md` and `Sources/Everglow/build.txt`.
- FNA/XNA graphics runtime - rendering, `SpriteBatch`, `GraphicsDevice`, and compiled Effects; the required runtime is supplied by the local tModLoader installation and the CI setup in `.github/workflows/build-and-test.yml`.

**Package Manager:**
- NuGet via SDK-style `PackageReference` entries - package restore is driven by the project files under `Sources/` and `Tools/`.
- Lockfile: missing; no `packages.lock.json`, `Directory.Packages.props`, or repository `nuget.config` was detected.
- Local assembly references are used alongside NuGet, including `Libraries/*.dll` through `Sources/Modules/Directory.Build.props` and `Sources/Everglow.Function/Everglow.Function.csproj`.

## Frameworks

**Core:**
- tModLoader/Terraria mod API - mod lifecycle, content registration, assets, configs, world data, UI, networking, and hooks in `Sources/Everglow/Everglow.cs` and `Sources/Everglow.Function/`.
- Solaestas.tModLoader.ModBuilder 1.5.11 - custom MSBuild packaging, resource processing, path generation, and combined `.tmod` creation via `Sources/Directory.Build.props`.
- Microsoft.Xna.Framework/FNA - 2D/3D drawing, render targets, and shader effects in `Sources/Everglow.Core/VFX/`, `Sources/Everglow.Function/VFX/`, and `Sources/Modules/Yggdrasil/YggdrasilModule.cs`.
- Microsoft.Extensions.DependencyInjection 9.0.8 - singleton service composition for the Core/Function boundary in `Sources/Everglow.Core/Ins.cs` and `Sources/Everglow/Everglow.cs`.

**Testing:**
- MSTest 3.10.2 with Microsoft.NET.Test.Sdk 17.14.1 - unit tests in `Sources/Everglow.UnitTests/` configured by `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`.
- coverlet.collector 6.0.4 - test coverage collection support declared in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`.

**Build/Dev:**
- MSBuild and `dotnet` CLI - restore, build, test, resource targets, and launch profiles in `Sources/Directory.Build.targets`, `Documents/源代码编译流程.md`, and `Sources/Everglow/Properties/launchSettings.json`.
- StyleCop.Analyzers.Unstable 1.2.0.556 - analyzer package with repository-specific diagnostic overrides in `Sources/Directory.Build.props` and `.editorconfig`.
- Custom MSBuild tasks - `WriteResource` and `ReadResource` are loaded from `Tools/Everglow.Tasks.dll` by `Sources/Directory.Build.targets`.

## Key Dependencies

**Critical:**
- `Solaestas.tModLoader.ModBuilder` 1.5.11 - the repository's custom build pipeline produces the combined Everglow mod and processes resources/effects; configured in `Sources/Directory.Build.props`.
- `SubworldLibrary.dll` - required tModLoader mod dependency for subworld entry, world switching, and cross-world packet routing in `Sources/Everglow/build.txt`, `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, and `Sources/Everglow.Function/Netcode/PacketResolver.Send.cs`.
- `ModLiquidLib.dll` - required tModLoader mod dependency for custom liquids in `Sources/Everglow/build.txt` and `Sources/Modules/Yggdrasil/YggdrasilTown/Liquids/DarkSludgeLiquid.cs`.
- tModLoader/Terraria assemblies - runtime host APIs used throughout `Sources/Everglow.Function/` and `Sources/Modules/`, with test copies sourced from `$(tMLSteamPath)` in `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`.
- `FontStashSharp.FNA.dll` - custom font rendering for Function UI and text drawers, referenced by `Sources/Everglow.Function/Everglow.Function.csproj` and used in `Sources/Everglow.Function/UI/FontManager.cs`.

**Infrastructure:**
- MathNet.Numerics 5.0.0 - vector, distribution, and numerical utilities in `Sources/Everglow.Core/Everglow.Core.csproj`, `Sources/Everglow.Core/Utilities/MathNetUtils.cs`, and `Sources/Everglow.Function/Mechanics/Quest/PlayerSide/PlayerQuestManager.cs`.
- SixLabors.ImageSharp 3.1.11 - decoding, pixel processing, and world-generation image inputs in `Sources/Everglow.Function/Everglow.Function.csproj` and `Sources/Everglow.Function/Utilities/ImageReader.cs`.
- SharpDX 4.2.0 - graphics-related support used by the Yggdrasil module and declared in `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj`.
- Newtonsoft.Json 13.0.3 - JSON handling for custom resource MSBuild tasks in `Tools/Everglow.Tasks/Everglow.Tasks.csproj` and `Tools/Everglow.Tasks/WriteResource.cs`.
- Microsoft.Build.Utilities.Core 17.9.5 - MSBuild task base APIs in `Tools/Everglow.Tasks/Everglow.Tasks.csproj`.
- PolySharp 1.14.1 - compatibility/source-generation support for the custom task assembly in `Tools/Everglow.Tasks/Everglow.Tasks.csproj`.
- MonoMod.RuntimeDetour, MonoMod.Cil, MonoMod.Utils, and ReLogic - tModLoader-supplied hook, IL, asset, and rendering support used in `Sources/Everglow.Function/Hooks/` and `Sources/Everglow.Function/VFX/`.

## Configuration

**Environment:**
- Main build properties are centralized in `Sources/Directory.Build.props`: `net8.0`, preview C# language version, unsafe code, active module list, path generation, effect compilation, and resource allowlists.
- Module projects inherit Core, Function, local library references, global usings, and module-specific path/compile constants from `Sources/Modules/Directory.Build.props`.
- tModLoader launch/build properties include `tMLPath`, `tMLServerPath`, `tMLSteamPath`, and `tMLLibraryPath`, referenced by `Sources/Everglow/Properties/launchSettings.json`, `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`, and `Sources/Everglow.Core/Everglow.Core.csproj`.
- Mod metadata and required runtime mod references are in `Sources/Everglow/build.txt`; runtime gameplay configuration is a server-side `ModConfig` in `Sources/Everglow.Function/FeatureFlags/EverglowConfig.cs`.
- No `.env`-style configuration file was detected; application configuration is supplied by tModLoader, MSBuild properties, mod metadata, and Terraria's save/config directories described in `Sources/Everglow.Function/`.

**Build:**
- `Sources/Directory.Build.targets` imports custom resource tasks from `Tools/Everglow.Tasks.dll`, writes per-project resource lists, and merges module/Common resources before the main project build.
- `Sources/Everglow/Everglow.csproj` references Core, Function, and every module named by `$(Modules)`, producing the final mod assembly/package.
- Resource packing is allowlisted in `Sources/Directory.Build.props` and specialized in module project files such as `Sources/Modules/Example/Everglow.Example.csproj` and `Sources/Modules/Yggdrasil/Everglow.Yggdrasil.csproj`.
- Formatting and analyzer behavior is defined in `.editorconfig`; solution/project files use CRLF while ordinary text uses LF and UTF-8 without BOM.

## Platform Requirements

**Development:**
- Windows - required by the project guidance in `AGENTS.md` and `CONTRIBUTING.md`.
- .NET SDK 8.0 or later - required for restore/build/test in `CONTRIBUTING.md` and `.github/workflows/build-and-test.yml`.
- Local tModLoader installation and a `tModLoader.targets` bridge to `tMLMod.targets` - required by `CONTRIBUTING.md` and `Documents/源代码编译流程.md`; no repository-local bridge file was detected in the scan.
- XNA Framework runtime components - required for Effect/shader compilation, installed in CI by `.github/workflows/build-and-test.yml`.

**Production:**
- Terraria 1.4.4.9 with tModLoader - the mod target documented in `README.md`.
- SubworldLibrary and ModLiquidLib enabled as tModLoader mod dependencies - declared in `Sources/Everglow/build.txt`.
- The packaged output is `Everglow.tmod`, assembled and deployed through the MSBuild pipeline described in `Documents/源代码编译流程.md` and `Sources/Everglow/Everglow.csproj`.

---

*Stack analysis: 2026-09-11*
