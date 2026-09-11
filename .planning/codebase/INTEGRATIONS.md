# External Integrations

**Analysis Date:** 2026-09-11

## APIs & External Services

**Terraria mod runtime:**
- tModLoader/Terraria - supplies the `Mod` lifecycle, content APIs, asset loading, configs, world persistence, UI, graphics, and native multiplayer transport used by `Sources/Everglow/Everglow.cs` and `Sources/Everglow.Function/`.
  - SDK/Client: local tModLoader assemblies and FNA/ReLogic libraries, referenced through `Sources/Modules/Directory.Build.props`, `Sources/Everglow.Core/Everglow.Core.csproj`, and `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`.
  - Auth: none in the mod; multiplayer identity and transport are supplied by Terraria/tModLoader.
- SubworldLibrary - provides subworld lifecycle, world transitions, copied world data, and main/subworld packet forwarding in `Sources/Modules/Yggdrasil/YggdrasilWorld.cs`, `Sources/Modules/SubSpace/RoomWorld.cs`, and `Sources/Everglow.Function/Netcode/PacketResolver.Send.cs`.
  - SDK/Client: `Libraries/SubworldLibrary.dll` and the runtime mod named by `Sources/Everglow/build.txt`.
  - Auth: none.
- ModLiquidLib - provides custom liquid types, liquid falls, and liquid IDs for Yggdrasil's Dark Sludge content in `Sources/Modules/Yggdrasil/YggdrasilTown/Liquids/` and `Sources/Modules/Yggdrasil/WorldGeneration/`.
  - SDK/Client: `Libraries/ModLiquidLib.dll` and the runtime mod reference in `Sources/Everglow/build.txt`.
  - Auth: none.

**Build and source hosting:**
- GitHub - hosts the repository, issues, pull requests, and GitHub Actions workflows referenced by `README.md`, `CONTRIBUTING.md`, and `.github/workflows/`.
  - SDK/Client: GitHub Actions, `actions/checkout`, `actions/setup-dotnet`, and the GitHub CLI/API in `.github/workflows/build-and-test.yml` and `.github/workflows/holistic-review.md`.
  - Auth: built-in `GITHUB_TOKEN`/`github.token` for workflow repository and pull-request operations in `.github/workflows/holistic-review.md`.
- DeepSeek-compatible Anthropic API - optional external LLM backend for the automated holistic pull-request review in `.github/workflows/holistic-review.md`.
  - SDK/Client: gh-aw/Copilot engine with `COPILOT_PROVIDER_TYPE=anthropic` and configurable provider base URL.
  - Auth: GitHub Actions repository secret `LLM_API_KEY`; the repository documents the default provider URL and model without embedding a credential in `.github/workflows/holistic-review.md`.

## Data Storage

**Databases:**
- None detected - the mod has no database driver, ORM, SQL schema, or database connection configuration under `Sources/`, `Libraries/`, or the project files under `Sources/`.

**File Storage:**
- Terraria/tModLoader save filesystem - world, player, and ModConfig persistence uses tModLoader `TagCompound`/world APIs in `Sources/Everglow.Function/Mechanics/Quest/WorldSide/WorldQuestSystem.cs`, `Sources/Modules/Yggdrasil/Common/YggdrasilPlayer.cs`, and `Sources/Everglow.Function/FeatureFlags/EverglowConfig.cs`.
- Mod data directory - developer and room-world data is written below `Main.SavePath/Mods/ModDatas/Everglow` and `Main.SavePath/Mods/ModDatas/Everglow_RoomWorlds` by `Sources/Everglow.Function/DeveloperContent/VFXs/TileToolBoxInterface.cs`, `Sources/Modules/SubSpace/RoomManager.cs`, and `Sources/Modules/SubSpace/CreateRoom.cs`.
- Bundled mod archive - maps, images, JSON, atlases, fonts, sounds, and Effects are read from the packaged mod through `Mod.GetFileStream`/`ModContent.GetFileBytes` in `Sources/Everglow.Function/Utilities/ImageReader.cs`, `Sources/Everglow.Function/TileHelper/MapIO.cs`, and `Sources/Modules/Yggdrasil/WorldGeneration/YggdrasilWorldGeneration.cs`.
- Build intermediates - per-module resource lists and caches use `bin/` and `obj/` paths configured by `Sources/Directory.Build.targets`; these are local build artifacts rather than application storage.

**Caching:**
- In-process tModLoader/ReLogic asset cache - assets are requested through `ModContent.Request` and generated `ModAsset` members in `Sources/Everglow.Function/` and `Sources/Modules/`.
- Build/resource cache only - MSBuild inputs and generated `.resource` lists are tracked by `Sources/Directory.Build.targets`; no external cache service is configured.

## Authentication & Identity

**Auth Provider:**
- Custom/game-provided identity - the mod does not implement accounts, OAuth, sessions, or a remote identity service. Player identity and server/client roles come from Terraria/tModLoader in `Sources/Everglow.Function/Netcode/NetUtils.cs` and `Sources/Everglow.Function/Netcode/PacketResolver.cs`.
- CI identity - GitHub Actions uses the workflow token for GitHub checkout/API operations, and the holistic review engine uses the repository secret `LLM_API_KEY` in `.github/workflows/holistic-review.md`.

## Monitoring & Observability

**Error Tracking:**
- None detected - no Sentry, Application Insights, hosted telemetry, or external error-reporting SDK appears in the project files or source directories `Sources/Everglow/`, `Sources/Everglow.Core/`, `Sources/Everglow.Function/`, or `Sources/Modules/`.

**Logs:**
- tModLoader/log4net logger - the mod registers Terraria's `Logger` into the Core service container in `Sources/Everglow/Everglow.cs` and consumes it through `Sources/Everglow.Core/Ins.cs`.
- Console and in-game messages - diagnostic output uses `Console.WriteLine`, `Ins.Logger`, and `Main.NewText` in `Sources/Everglow.Function/` and active modules.
- Pull-request review output - automated review findings are posted as GitHub pull-request comments/reviews by the safe outputs configured in `.github/workflows/holistic-review.md`.

## CI/CD & Deployment

**Hosting:**
- GitHub Actions on `windows-latest` - build/test automation is defined in `.github/workflows/build-and-test.yml`.
- Local tModLoader installation - successful MSBuild packaging deploys `Everglow.tmod` to the local tModLoader Mods directory through the ModBuilder pipeline described in `Documents/源代码编译流程.md`.

**CI Pipeline:**
- Checkout and full-history comparison - `.github/workflows/build-and-test.yml` checks out code, fetches `origin/master`, and rejects branches behind the target branch.
- .NET restore/build/test - `.github/workflows/build-and-test.yml` installs .NET 8, restores NuGet packages, builds Release, and runs `dotnet test`.
- tModLoader dependency bootstrap - CI downloads the latest tModLoader release from GitHub and creates a parent `tModLoader.targets` bridge in `.github/workflows/build-and-test.yml`.
- Graphics prerequisite bootstrap - CI downloads and installs the XNA Framework redistributable from Microsoft before building in `.github/workflows/build-and-test.yml`.
- Automated PR review - `.github/workflows/holistic-review.md` runs on pull-request open/synchronize/reopen/ready-for-review events, reads review history through GitHub, calls an allowlisted LLM provider, and submits a `COMMENT` review.

## Environment Configuration

**Required env vars:**
- `tMLPath`, `tMLServerPath`, and `tMLSteamPath` - MSBuild/launch properties required by the client/server launch profiles and unit-test DLL copy target in `Sources/Everglow/Properties/launchSettings.json` and `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`.
- `tMLLibraryPath` - MSBuild property used to exclude tModLoader-specific assemblies from the Terraria-independent Core project in `Sources/Everglow.Core/Everglow.Core.csproj`.
- `LLM_API_KEY` - GitHub Actions repository secret used only by the automated holistic review engine in `.github/workflows/holistic-review.md`.
- `LLM_BASE_URL` and `LLM_MODEL` - optional GitHub Actions repository variables that override the review provider URL/model in `.github/workflows/holistic-review.md`.

**Secrets location:**
- Runtime mod: no application secret store or secret-bearing configuration was detected under `Sources/` or the repository root.
- CI review: GitHub Actions repository secret/variables are referenced symbolically in `.github/workflows/holistic-review.md`; secret values are not stored in the repository.

## Webhooks & Callbacks

**Incoming:**
- GitHub `pull_request` events (`opened`, `synchronize`, `reopened`, and `ready_for_review`) trigger the automated review workflow in `.github/workflows/holistic-review.md`.
- Terraria/tModLoader mod callbacks - lifecycle, content, hook, and `Mod.HandlePacket` callbacks enter through `Sources/Everglow/Everglow.cs`; these are in-game callbacks, not HTTP webhooks.
- No application HTTP webhook endpoint was detected in `Sources/Everglow/`, `Sources/Everglow.Core/`, `Sources/Everglow.Function/`, or `Sources/Modules/`.

**Outgoing:**
- GitHub pull-request review comments and reviews - the holistic review workflow emits safe outputs to the PR in `.github/workflows/holistic-review.md`.
- Terraria multiplayer packets - custom packets are serialized/routed through the tModLoader transport in `Sources/Everglow.Function/Netcode/PacketResolver.Serialize.cs`, `Sources/Everglow.Function/Netcode/PacketResolver.Send.cs`, and `Sources/Everglow.Function/Netcode/PacketResolver.Resolve.cs`.
- No outbound application HTTP API call was detected in the gameplay/runtime source directories `Sources/Everglow/`, `Sources/Everglow.Core/`, `Sources/Everglow.Function/`, and `Sources/Modules/`.

---

*Integration audit: 2026-09-11*
