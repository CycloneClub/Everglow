# Testing Guidelines

本文件适用于 `Sources/Everglow.Function/Mechanics/Quest/` 下的任务系统改动；相关自动化测试位于 `Sources/Everglow.UnitTests/Function/QuestSystem/`。同时遵守仓库根级 `AGENTS.md`。

## 测试决策

1. 新增测试前，必须先写明可能回归的可观察游戏规则或跨边界行为，以及失败时玩家、世界存档、网络端或 UI 会观察到什么。
2. 任务生命周期与状态转换、目标进度和计时、分支/并行/可选目标结构、存档恢复、世界侧与玩家侧同步、操作合法性、隐藏信息遮罩，以及领域模型到 Presentation View/Action 的映射发生变化时，应添加保护该行为的最小测试。
3. 如果无法指出受影响的游戏规则或集成行为，不要仅为提高覆盖率新增测试；分类确实不明确时，先说明怀疑存在的行为风险再决定。
4. 配置、声明、静态元数据、依赖注入或注册、资源路径、生成代码与生成产物、构建工具输出，以及纯视觉素材或布局调整，通常只需执行现有验证。不要为使这些内容“可测试”而抽取生产逻辑、添加快照、编写自定义检查器或新增 CI 门禁。
5. 不要测试 tModLoader/FNA 已提供的常规行为；只测试 Everglow 自定义的状态转换、校验、计算、持久化、同步、副作用和呈现规则。

## 测试放置与写法

- 使用现有 MSTest 项目，并在 `Sources/Everglow.UnitTests/Function/QuestSystem/` 下按生产边界组织测试：`PlayerSide/`、`WorldSide/`、`Presentation/` 与 `UI/`；继续镜像有意义的 `Abstractions/`、`Structure/`、`Objectives/`、`Adapters/` 与 `Views/` 子目录。`WorldSide/Tests/` 与 `PlayerSide/Tests/` 是模组内示例/调试任务内容，不是 MSTest 测试目录。
- 所有任务系统测试保持 `Everglow.UnitTests.Function.QuestSystem` 命名空间，不要让物理目录制造无意义的命名空间差异。测试文件应以主要被测类型命名；单个类型的测试过大时，在该类型的专属目录中按可观察行为拆为 `Type.BehaviorTests.cs`，并用 `partial` 复用局部 Stub 与辅助方法。不要仅为对称而拆分小文件或 UI 内部目录。
- 优先测试不依赖 Terraria 运行期的纯逻辑。复用邻近测试中的私有 `Stub`/`Test` 派生类、`IGameStateProvider` 测试替身和局部创建方法；不要因单个用例引入全局工厂、Mock 框架或新的测试抽象层。
- 用公开状态、返回值、事件、持久化数据或 Presentation View/Action 断言可观察结果。只有当公开边界无法表达既有契约时，才沿用邻近测试的反射方式；不要把实现细节锁死在测试中。
- 网络与外部边界必须使用本地替身或直接驱动数据包/管理器边界。自动化测试不得启动真实多人游戏、访问第三方网络、依赖凭据或可变外部数据。
- 测试首次触及 `Terraria.Main` 前，必须在 `[TestInitialize]` 中设置 `Terraria.Program.SavePath = string.Empty;`。不得构造 `Main`、启动图形/内容加载或运行游戏循环。修改共享静态状态的测试类必须标记 `[DoNotParallelize]`，保存原值并在 `[TestCleanup]` 中恢复。
- 涉及 `Player.talkNPC` 时，使用反射设置其私有属性，不要调用 `SetTalkNPC`。图形相关逻辑应尽量在 `Main.dedServ = true` 下验证非渲染行为；实际 GPU 绘制和素材外观应留给 tML 运行时验证。

## 验证与报告

- 每个任务系统代码改动都必须从仓库根目录运行 `dotnet build`。
- 有任务系统行为改动时，运行 `dotnet test --filter "FullyQualifiedName~QuestSystem"`。跨越 Quest 与其他 Function 子系统、共享基础设施或测试公共状态的改动，还应运行 `dotnet test --verbosity normal /p:WarningLevel=0`。
- 不需要新增测试但涉及可编译源码、资源或配置的非行为改动，至少运行 `dotnet build`，并按根级指南执行适用的文本、资源或构建配置检查。纯文档改动只需执行根级指南要求的文本检查。不要用新测试替代编译、资源打包或 tML 运行时验证。
- 最终报告必须分别列出新增/修改的测试、执行过的命令及结果，并明确说明未能在本地验证的多人同步、图形或 tML 运行时行为。

## Basis

- `README.md` 与 `QUEST_SYSTEM_DESIGN.md`：任务生命周期、目标结构、持久化、多人同步及 Presentation 分层契约。
- `Sources/Everglow.UnitTests/Everglow.UnitTests.csproj`：MSTest 3.10.2、测试项目引用与现有测试工具。
- `Sources/Everglow.UnitTests/Function/QuestSystem/`：测试位置、局部 Stub/测试派生类、状态清理、非并行和 `Terraria.Main` 初始化惯例。
- 根级 `AGENTS.md` 与 `.github/workflows/build-and-test.yml`：构建、筛选测试、完整测试及验证要求。
