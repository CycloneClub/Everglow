# Repository Guidelines

本文件是 Everglow 的根级 AI 编码指南，适用于整个仓库。进入子目录后，必须先查找更近的 `AGENTS.md`；局部规则优先。本仓库面向 Windows 上的 Terraria tModLoader，使用 C#、.NET 8、FNA，并以 GPL-3.0 发布。

## Start Here

开始任务前，先确认改动范围并阅读对应实现、测试和局部说明；不要凭项目概览猜测 API 或资源路径。

| 任务 | 先阅读 |
| --- | --- |
| 构建、打包或 CI | `Documents/源代码编译流程.md`、`Sources/Directory.Build.props`、`.github/workflows/build-and-test.yml` |
| Quest 任务系统 | `Sources/Everglow.Function/Mechanics/Quest/README.md`、`Sources/Everglow.Function/Mechanics/Quest/QUEST_SYSTEM_DESIGN.md` |
| 触及 `Terraria.Main` 的测试 | `.cursor/rules/unit-testing-terraria-main.mdc` 与临近测试 |
| Food 模块 | `Sources/Modules/Food/FoodModule.md` |
| 新增游戏内容 | `Sources/Modules/Example/` 中相同内容类型的范例 |
| 子世界或 IL 修改 | `Documents/子世界.md`、`Documents/ILDoc/` |
| Yggdrasil | `Sources/Modules/Yggdrasil/AGENTS.md`、其 `README.md` |

`Documents/`、子系统 README、`QUEST_SYSTEM_DESIGN.md` 与局部 `AGENTS.md` 是本文件的补充，不要将其内容复制回根指南。

## Repository Map

```text
Everglow.sln                         solution: active modules + unit tests
Sources/
  Everglow/                          Mod entry and all localization
  Everglow.Core/                     Terraria-independent base layer
  Everglow.Function/                 tML-facing shared functionality
  Everglow.UnitTests/                MSTest tests
  Modules/                           content assemblies (14 active modules)
Libraries/SubworldLibrary.dll        required external mod dependency; read-only
Tools/                               build tasks and utilities
Documents/                           design and workflow documentation
```

The project produces one `Everglow.tmod`: the main project assembles the Core, Function, and active content-module assemblies. The active module list is the `Modules` property in `Sources/Directory.Build.props`; the same value drives project references and conditional-compilation symbols.

`Modules/IIID`、`Modules/TwilightForest`、`Modules/ZY` 和 `Sources/Everglow.Scripts` 都是已废弃残留，不参与构建；不要在其中新增或恢复内容。

## Build and Test

Prerequisites: .NET SDK 8.0+, a local tModLoader installation, Windows, and a `tModLoader.targets` file in this repository or one of its five ancestors that points at tML's `tMLMod.targets`. Shader compilation also requires the XNA runtime components normally installed with tML.

Run commands from the repository root:

```powershell
dotnet restore
dotnet build
dotnet build /p:Configuration=Release /p:WarningLevel=0
dotnet test --verbosity normal /p:WarningLevel=0
dotnet test --filter "FullyQualifiedName~QuestSystem"
.\update.bat
```

`dotnet build` packages and deploys `Everglow.tmod` to the local tML Mods directory, then enables it. Do not use tML's in-game “Build Mod” action for this repository. Stale `bin/` and `obj/` outputs are a known cause of build failures; consult the build document before removing caches.

When modifying `Sources/Directory.Build.props`, explain the structural impact and obtain user confirmation first. Keep the matching `Directory.Build.props` block in `.github/workflows/build-and-test.yml` synchronized, because CI regenerates the file before restore.

## Architecture and Placement

```text
Everglow.Core  -> interfaces, infrastructure, utilities; no Terraria/tML reference
Everglow.Function -> tML implementations, templates, hooks, VFX, UI, netcode
Modules -> gameplay/content; reference Core and Function automatically
Everglow -> composition, content discovery, resource merge, .tmod output
```

- Both Core and Function use `Everglow.Commons` as their root namespace. Do not add a Terraria/tML reference to Core; define an interface there and implement/register it in Function through `Ins` instead.
- `Sources/Everglow/Everglow.cs` automatically scans the `Everglow.*` assemblies and registers content. Do not manually call `AddContent` for normal `ModItem`, `ModNPC`, `ModProjectile`, `ModTile`, or `ModSystem` classes.
- A content module needs an `EverglowModule` only for lifecycle work such as hooks, effect preload, skies, or events. Normal content classes require no module entry point.
- Module projects inherit Core, Function, `SubworldLibrary`, `PathPrefix`, and standard global usings from `Sources/Modules/Directory.Build.props`. Do not duplicate those references or usings in module `.csproj` files.
- Add an explicit `ProjectReference` in a module's `.csproj` for every newly introduced cross-module type dependency. Existing dependencies include `Myth → SpellAndSkull`, `Ocean → Myth`, and `Yggdrasil → CagedDomain, Food, SpellAndSkull, SubSpace`.

## Resources and Content

- Use the source-generated `ModAsset` members for assets in the current project. Use `Commons.ModAsset` for Function assets. Do not add handwritten asset paths unless a generated member cannot express the path.
- Runtime asset paths are `Everglow/<PathPrefix>/<relative path>`; a module's prefix is its module name and Function uses `Commons`.
- Resource packing is allowlisted. `.hjson`, `.txt`, and `.png` are automatic; other extensions need an appropriate packed `AdditionalFiles` entry. Read the build document before changing packing configuration.
- Never modify existing binary or art assets: `Resources/`, `Libraries/`, any module `.png/.obj/.ttf/.atlas/.xnb`, root `icon*.png`, or `Tools/*.exe|dll`. Do not create placeholder art; tell the user when a new asset is required.
- New content follows `Everglow.<Module>[.<Area>[.<Type>]]`. Keep matching assets beside their `.cs` file. Prefer the templates in `Everglow.Function/Templates` and the Example module over new bespoke base patterns.

## Code, Gameplay, and Networking

- Follow `.editorconfig`: tab indentation (width 4), LF and UTF-8 without BOM for text, Allman braces, file-scoped namespaces, and a trailing newline. Keep `.sln` and `.csproj` as CRLF.
- Match local naming, comment language, and existing style. Do not perform unrelated reformatting. Module projects already provide common global usings.
- For `NPC.ai[]` and `Projectile.ai[]`, define a private enum and descriptive wrapper properties instead of scattered numeric indexes.
- Guard client-only graphics, VFX, render targets, and texture work with `!Main.dedServ`. Graphics services are not registered on a dedicated server.
- Use `ModIns.PacketResolver` for custom packets. Synchronize gameplay state with `netUpdate` when needed and use `SendExtraAI`/`ReceiveExtraAI` for additional projectile or NPC state.
- Gate unfinished work with `CompileTimeFeatureFlags` or `EverglowConfig`; do not submit always-enabled experimental behavior.

## Localization

- Maintain both `Sources/Everglow/Localization/en-US/` and `zh-Hans/`; `templates/` is not a content target.
- Content classes must override `LocalizationCategory` with `LocalizationUtils.Categories.*`.
- Do not manually create classification HJSON keys. Use the in-game `OutputLocalizationHjsonItem` exporter to add missing keys. Localization keys are additive: never delete them.
- Do not rename a content internal name without searching and updating all references, localization keys, `ItemID/NPCID.Search` uses, and compatibility implications.
- Do not add new content to the legacy root `Localization/en-US_Mods.Everglow.hjson`.

## Tests and Verification

- Unit tests use MSTest 3.10.2 in `Sources/Everglow.UnitTests`; prefer pure logic tests. Function changes and tests require `dotnet test` after a successful build.
- Before a test first touches `Terraria.Main`, set `Program.SavePath = string.Empty;` in `[TestInitialize]`. Do not construct `Main`, start graphics/content loading, or run the game loop. Shared `Main` state means affected tests must not run in parallel.
- For `Player.talkNPC`, set the private property with reflection; do not call `SetTalkNPC`.
- Every code change requires `dotnet build`. Clearly state any tML runtime behavior that could not be verified locally.
- After text edits, run this byte-level UTF-8 BOM check against `origin/master`; do not substitute `git diff --check` or a text reader.

```powershell
$base = git merge-base HEAD origin/master
$files = @((git -c core.quotepath=false diff --name-only --diff-filter=ACMRTUXB $base --) + (git -c core.quotepath=false ls-files --others --exclude-standard)) | Sort-Object -Unique
$bom = @($files | Where-Object {
	if (-not (Test-Path -LiteralPath $_ -PathType Leaf)) { return $false }
	$b = [IO.File]::ReadAllBytes($_)
	$b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
})
if ($bom) { $bom | ForEach-Object { "UTF-8 BOM: $_" }; exit 1 }
"UTF-8 BOM check passed ($($files.Count) files)."
```

## Agent Boundaries

### Always

- Make the smallest focused patch, preserving unrelated user changes in a dirty worktree.
- Read the relevant implementation, nearby tests, and task-specific documentation before editing.
- Search the codebase for callers, implementations, and related patterns before editing.
- Verify changes in proportion to risk and report exact unverified behavior.

### Ask First

- Any structural change to `Sources/Directory.Build.props`, a project file, the solution, module activation, or resource-packing configuration.
- Adding a dependency, renaming/deleting files or public content names, creating new art assets, or changing build/CI behavior.
- Any Git state change: branch creation, staging, commit, push, rebase, merge, or reset.

### Never

- Modify binary/art assets, legacy/dead modules, generated output, or third-party libraries unless the user explicitly asks.
- Delete localization keys, broadly reformat files, introduce tML into Core, or use a manual `AddContent` registration for ordinary content.
- Use destructive Git commands or discard user changes without explicit instruction.
