# Contributing to Everglow

Thank you for helping improve Everglow. The project is a GPL-3.0 Terraria tModLoader mod built with C# and .NET 8. Contributions may include code, tests, documentation, localization, bug reports, and design discussion.

## Before You Start

- Search [existing issues](https://github.com/CycloneClub/Everglow/issues) before opening a new one.
- Open an issue or discuss the change with maintainers before investing in a large feature, a new module, a public content rename, a build-system change, or new art/audio assets.
- Keep one pull request focused on one problem. Avoid drive-by formatting changes or unrelated refactors.
- Read [AGENTS.md](AGENTS.md) for the repository map, architectural boundaries, resource rules, and task-specific documentation links. It is written for coding agents but documents project constraints that apply to every contribution.

## Development Setup

Everglow is developed on Windows. Before building, install:

1. [.NET SDK 8.0 or later](https://dotnet.microsoft.com/download/dotnet/8.0).
2. A local [tModLoader](https://store.steampowered.com/app/1281930/tModLoader) installation.
3. The XNA runtime components used by tModLoader for Effect compilation.

Clone the repository under tModLoader's `ModSources` directory. A `tModLoader.targets` file must exist in this repository or within one of its five parent directories and point to tML's `tMLMod.targets`. In the normal Windows layout, `ModSources/tModLoader.targets` supplies this link.

Run commands from the repository root:

```powershell
dotnet restore
dotnet build
dotnet test --verbosity normal /p:WarningLevel=0
```

The CI-equivalent build is:

```powershell
dotnet build /p:Configuration=Release /p:WarningLevel=0
```

Do not use tModLoader's in-game **Build Mod** action for this repository. The MSBuild pipeline compiles the module assemblies, processes resources and Effects, and writes the combined `Everglow.tmod` to the local tModLoader Mods directory.

## Making Changes

- Place gameplay content in the appropriate active module below `Sources/Modules/`; do not add code to the retired `IIID`, `TwilightForest`, or `ZY` module directories.
- Preserve the layer boundary: `Everglow.Core` must not reference Terraria or tModLoader. Add tML-facing implementations in `Everglow.Function` instead.
- Follow `.editorconfig`: tab indentation, Allman braces, file-scoped namespaces, LF text files, UTF-8 without BOM, and a trailing newline. Keep `.sln` and `.csproj` files as CRLF.
- Prefer the existing templates in `Everglow.Function/Templates` and the examples in `Sources/Modules/Example/` when adding common tModLoader content types.
- Use generated `ModAsset` members for assets. Do not edit existing binary or art assets, including module textures, audio, atlases, fonts, compiled assets, `Resources/`, `Libraries/`, or tool binaries, unless a maintainer specifically requests it.
- Maintain both `Sources/Everglow/Localization/en-US/` and `zh-Hans/` for player-facing content. Do not delete localization keys.
- Treat changes to `Sources/Directory.Build.props`, project files, the solution, resource-packing rules, active modules, and CI as architectural changes; discuss them first.

## Tests and Verification

Run `dotnet build` for every code change. Run the full test command above when changing `Everglow.Function`, unit tests, or shared logic. For a focused test run:

```powershell
dotnet test --filter "FullyQualifiedName~QuestSystem"
```

Tests use MSTest and live in `Sources/Everglow.UnitTests`. Terraria static state makes graphics-dependent tests unsuitable; favor pure logic tests. Before a test first accesses `Terraria.Main`, set `Program.SavePath = string.Empty;` in `[TestInitialize]`. See `.cursor/rules/unit-testing-terraria-main.mdc` before writing those tests.

Before submitting text-file changes, ensure they contain no UTF-8 BOM. `AGENTS.md` contains the repository's required byte-level verification command.

## Pull Requests

- Branch from an up-to-date `master`; CI rejects branches that are behind `origin/master`.
- Use a concise English imperative commit message. Conventional Commit prefixes are welcome, for example `fix(Myth): correct projectile sync`.
- In the pull request, explain the problem, the chosen solution, and the verification you ran. Call out any client-only, server-only, multiplayer, or tML runtime behavior that you could not test locally.
- Do not commit generated `bin/`, `obj/`, `TestResults/`, unrelated binary files, credentials, or secrets.
- Be ready to respond to review feedback with small follow-up commits rather than rebasing away the discussion.

## Getting Help and Discussing Ideas

Use GitHub Issues for reproducible bugs and well-scoped feature proposals. Join the [Everglow Discord server](https://discord.gg/pdXvp89Dbp) for community discussion, early design feedback, or general tModLoader help. Please follow the [Code of Conduct](CODE_OF_CONDUCT.md) in all project spaces.
