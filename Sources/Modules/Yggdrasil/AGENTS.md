# Yggdrasil Module Guidelines

本文件适用于 `Sources/Modules/Yggdrasil/` 及其全部子目录，并补充根 `AGENTS.md`。Yggdrasil 是启用的内容模块，命名空间为 `Everglow.Yggdrasil`，资源前缀为 `Everglow/Yggdrasil`。

## Read Before Editing

- `README.md`：天穹树关卡顺序与当前开发重点；目前重点是 Kelp Curtain（苍苔帘幕）。
- `Everglow.Yggdrasil.csproj`：显式依赖 CagedDomain、Food、SpellAndSkull 和 SubSpace；新增跨模块类型引用必须在此加入对应 `ProjectReference`，并先获得用户确认。
- `YggdrasilWorld.cs`、`YggdrasilModule.cs`：子世界入口、世界尺寸、全局钩子和渲染流程。
- `Common/YggdrasilPlayer.cs` 与 `Netcode/`：玩家持久状态与同步模式。
- 修改世界生成、后处理或贴图资源前，先阅读对应区域的现有实现和根指南中的资源约束。

## Module Map

| 位置 | 责任 |
| --- | --- |
| `YggdrasilWorld.cs` | `Subworld` 定义、进入状态和世界尺寸/生成入口 |
| `YggdrasilModule.cs` | 客户端 Hook、地图着色、遮挡与后处理渲染 |
| `Common/` | 跨区域玩家、全局 NPC、内容和墙体逻辑 |
| `Netcode/` | 模块数据包与处理器 |
| `WorldGeneration/` | 天穹树世界、城镇与 Kelp Curtain 生成，以及 `.mapio`/噪声输入 |
| `YggdrasilTown/` | 城镇内容：生物群系、背景、家具、NPC、物品、投射物和 VFX |
| `KelpCurtain/` | 苍苔帘幕内容：群系、Tiles、NPC、物品、Buff、投射物和 VFX |
| `CorruptWormHive/`、`HurricaneMaze/`、`GreenCore/`、`CityOfMagicFlute/` | 其他已落地关卡区域 |
| `Effects/`、`Music/` | 模块 Effect 源文件与音乐资源 |

将新内容放入所属关卡/区域及对应内容类型目录；不要把区域专属实现塞入 `Common/`。关卡名、资源和内部类型名是内容及存档兼容性的一部分，重命名之前必须全局搜索并征求确认。

## Subworld and Generation

- 使用 `YggdrasilWorld.InYggdrasil` 或 `SubworldSystem.IsActive<YggdrasilWorld>()` 判断天穹树上下文；不要以地图尺寸、坐标或场景效果替代该判断。
- `YggdrasilWorld` 当前是 2000 × 21000、`ShouldSave => false` 的临时子世界。修改尺寸、保存策略、世界边界、进入/退出行为或 `Tasks` 属于结构性游戏行为，先获得用户确认。
- 世界生成改动放在 `WorldGeneration/`，并保持 `YggdrasilWorldGeneration.YggdrasilWorldGenPass` 的既有入口。涉及 `.mapio`、`.bmp` 噪声或 JSON 的改动先确认资源打包和运行时加载路径。
- 子世界专属 NPC 生成、背景、音乐、瓷砖和交互必须在正确的 `YggdrasilWorld`/`RoomWorld` 上下文中启用；新增逻辑不得意外影响主世界。

## Rendering and Effects

- `YggdrasilModule.Load()` 中的 `FilterManager`、`On_`、`IL_` Hook 和 `RenderTarget2D` 工作仅可在 `!Main.dedServ` 条件内注册或使用。服务端不能访问图形服务。
- `YggdrasilModule` 的 IL 注入依赖 tML 当前局部变量布局。改动匹配模式、注入点或升级 tML 后，必须保留明确的失败信息，并在客户端实际打开地图验证。
- 需要经过该模块遮挡/特效流程的投射物实现 `IOcclusionProjectile`，并实现 `DrawOcclusion(VFXBatch)` 和 `DrawEffect(VFXBatch)`；绘制必须只依赖客户端可用状态。
- 新 `.fx` 文件位于 `Effects/` 或所属区域的 `VFXs/`。全局构建属性会编译 Effect；修改 shader 后至少运行构建，并在客户端验证实际渲染。不要手改生成的 `.xnb`。
- 继续使用模块的 `ModAsset` 路径成员；不要把同一资源以不同硬编码路径重复请求。

## Multiplayer and Persistent State

- 自定义同步通过 `ModIns.PacketResolver`，使用 `IPacket`/`IPacketHandler` 和 `[HandlePacket]`。数据包读写顺序必须严格一致。
- 玩家永久增益状态归 `Common/YggdrasilPlayer.cs` 与 `Netcode/PermanentBoostPacket.cs` 管理。新增持久字段须同时审查 `SaveData`、`LoadData`、`SyncPlayer`、`CopyClientState`、`SendClientChanges` 和数据包的 `Send`/`Receive`/handler。
- NPC 或 Projectile 的权威状态变化应设置适当的 `netUpdate`，额外状态使用 `SendExtraAI`/`ReceiveExtraAI`。不要仅靠客户端视觉代码改变游戏状态。

## Resources and Verification

- `.png`、`.ogg`/`.mp3`、`.fx`、`.mapio` 和 `.bmp` 均是只读美术/设计输入；除非用户明确要求，不修改它们，也不创建占位资源。
- 此模块的 `.json` 通过项目文件中的 `Solaestas-ResourceFile` 包含；不要删除或扩展其范围而未确认结构性影响。
- 修改本模块 C# 或 Effect 后，从仓库根运行 `dotnet build`。涉及网络、子世界、世界生成或渲染的行为不能由单元测试充分覆盖；在最终说明中分别列出已验证与未验证的客户端、服务端和多人行为。
