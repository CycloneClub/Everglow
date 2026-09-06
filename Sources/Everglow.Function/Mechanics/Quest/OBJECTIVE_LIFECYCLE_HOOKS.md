# WorldSide 目标生命周期钩子

本文是 WorldSide 目标副作用的设计说明，补充 `README.md` 与 `QUEST_SYSTEM_DESIGN.md`，不改写既有任务状态机。尚未实现。

## 要解决的问题

任务目标负责判定（对话、击杀、提交、到达）。作者还需要在**某个目标进入 / 维持 / 离开 Active** 时做世界副作用：生成或回收任务专用 NPC、启动或停止入侵、召唤或清掉特殊 Boss。

不希望：

- 为每种副作用新写目标子类，或再叠 `ILifecycle` / Actor 类型
- 在任务 `Update` / `OnUnlock` 里扫描 `FindCurrentObjectives()` 再自己判断该刷什么
- 为跨多个目标的存活窗口做闩锁或区间绑定

跨多个目标仍要在场的实体按常驻内容处理（城镇 NPC、世界旗标），不走本钩子。

## 方案

在 `WorldObjectiveBase` 上增加与 `WithDescription` / `WithTimeLimit` 同形的 fluent 委托，三拍对应窗口的进入、维持、离开：

```csharp
Objectives.Add(
    new WorldTalkObjective(guideType)
        .WithActivate((quest, objective) => { /* 刷 NPC / 开入侵 / 召唤 */ })
        .WithUpdate((quest, objective) => { /* 可选维持 */ })
        .WithDeactivate((quest, objective) => { /* 回收 / 停入侵 / 清 Boss */ }));
```

| 方法 | 何时调用 | 次数 |
| --- | --- | --- |
| `WithActivate` | 该目标进入 Active 且 `CanProgress` | 同一窗口一次 |
| `WithUpdate` | 该目标仍在 Active 且 `CanProgress`，跟随 `WorldQuestManager` 更新间隔 | 每拍零或多次 |
| `WithDeactivate` | 该目标离开 Active（完成、超时、任务失败/完成、重置） | 离开时一次 |

同一方法多次调用追加委托，不覆盖。需要区分完成与超时时，在 `WithDeactivate` 里读取 `objective.Completed`、`objective.IsTimedOut`、`quest.State`。框架不提供原因枚举，不提供补刷或跨存档一次性策略。

委托签名：`Action<WorldQuestBase, WorldObjectiveBase>`。返回 `this`。

不新增目标类型、不新增 Lifecycle 接口、不新增 Actor 基类。生成坐标、NPC 标记、入侵清理、net 守卫都写在委托里。

## 写法二：窗口当标记，复杂逻辑写在外面

三拍适合短副作用。入侵、Boss、跨系统配合可以把「窗口是否打开」当成标记，复杂过程写在任务字段、模块或 `GlobalNPC` 里。不要在 `WithUpdate` 里每拍把某个 bool 设为 `true`：关窗后 `WithUpdate` 不再跑，外部看不到下降沿，标记会粘住。

现成标记是 `WorldQuestBase.ActiveObjectives`（框架已维护）。外面检测即可，不必再扫 `FindCurrentObjectives()`：

```csharp
if (quest.State == WorldQuestState.Active
    && quest.ActiveObjectives.Contains(talk))
{
    // 维持入侵、补实体、和别的系统对话
}
```

若外面不方便拿目标引用，用三拍给任务自己的字段置位，检测字段：

```csharp
.WithActivate((_, _) => _invasionWindow = true)
.WithDeactivate((_, _) => _invasionWindow = false);
```

置位仍走进入/离开，不走每拍 `Update`。第一版不加按名查询的 `WithSignal`；需要时再补。

## 为什么委托必须由 Quest 分发

`WorldTalkObjective` / `WorldKillNPCObjective` 等重写 `Activate`、`Update`、`Deactivate` 时通常不调用 `base`。不能把钩子放进这些虚方法的基类实现里指望被执行。

`WorldQuestBase` 已经在下列路径调用 `objective.Activate` / `Deactivate`，并维护 `_activatedObjectives`：

- `UnlockCore` → `Objectives.Activate()` → `OnObjectiveActivated`
- 节点完成 → `OnObjectiveDeactivated` 再 `OnObjectiveActivated`
- 目标超时 → `OnObjectiveTimedOut` → 单个 `Deactivate`
- 任务完成 / 失败 → `Deactivate()`
- `ResetProgress` → 对当前已激活目标 `Deactivate`
- 目标重试 / 读档快照 → `RefreshActivatedObjectives`

`WithActivate` / `WithDeactivate` 必须挂在这些**同一批调用点旁边**，或抽成 `WorldQuestBase` 对 `Activate`/`Deactivate` 的唯一包装，避免有的路径调了虚方法、漏了钩子。

`WithUpdate` 不能依赖子类 `Update()` 调 `base`。应在任务更新里对 `_activatedObjectives` 中仍 `CanProgress` 的目标调用委托；节点 `Update` 之后或并列均可，但须保证：目标已完成本拍进度判定之后，若本拍即将完成或超时，本拍 `WithUpdate` 与 `WithDeactivate` 的先后与「完成优先于超时」一致（先判定完成，再推进计时）。`WithUpdate` 不保证离开窗口时还会再跑一拍。

## 明确不做

- 跨目标 `From` / `To`、多目标 `While` 列表、闩锁
- PlayerSide
- Presentation / UI 变化
- 框架级 NPC 标记、自动补刷、子世界可见性、客户端禁跑副作用（作者在委托里自行判断单机或主服）
- 把任务级 `OnUnlock` / `OnComplete` / `OnExpire` / `OnReset` 换成这套钩子

## 实现要点

1. `WorldObjectiveBase` 三条委托列表 + 三个 `With*`。不进入存档和网络快照。
2. `WorldQuestBase` 保证 Activate/Deactivate 包装成成对的「虚方法 + 钩子」，所有现有调用点只走包装。
3. 同一目标在 `_activatedObjectives` 中时不重复 `WithActivate`；从集合移除时调用 `WithDeactivate`。
4. 读档或快照后，当前仍 Active 的目标会再 `Activate`，因此 `WithActivate` 会再跑。作者若不要重复刷实体，自己在委托里判重。
5. 测试用计数委托，覆盖：激活一次、在场有 Update、完成/超时/失败/重置有 Deactivate、重试再 Activate、并行节点各目标独立、desired 未变不重复 Activate。不测 `NPC.NewNPC`。
6. 分发时逐目标、逐条委托捕获异常并记录，再继续同一批其余目标和其余任务。

## 已排除的替代

- 目标子类或 `WithLifecycle(IActor)`：类型会随内容增长。
- 只提供 `WithUpdate`：看不到完成、超时、失败，清不掉入侵和 Boss。
- 任务定义里扫描当前目标：作者重复实现结构机，容易漏重试和读档。

## 评审（2026-09-06）

作者侧三条可追加委托值得留。按上文「包住每一次 `Activate`/`Deactivate`、框架不守网络端、快照一律再 Activate」实现则不稳定。现有状态机里，容器事件和 `RefreshActivatedObjectives` 已经是两套路径。

必须先定的硬化：

1. 钩子只跟 `_activatedObjectives` 的成员变化走，不要包每一趟虚方法。虚方法继续全端重绑击杀/消耗监听。
2. 默认只在单机或主服调用这三条钩子。需要端特效的作者自己判端。
3. 并行/任选在本地 `CompleteNodeCore` 会整表拆装，快照只走 Refresh；同一推进在主机和下游的窗口序列不一致。钩子若跟集合走，要先统一这条对账，否则未完成兄弟会假离开再假进入。
4. `Reset` 之后的读档、进出子世界算新窗口；会话内 `NetReceive` 且 desired 不变不进窗口。
5. `WithUpdate` 放在 `UpdateNode` 之后，只打仍 `CanProgress` 的成员。
6. 逐目标包住委托异常，避免半激活和中断整个 Manager 循环。
7. 契约：用世界内查找，不用闭包记住 `whoAmI`。`WithDeactivate` 不能可靠区分「节点推进」和「仍为 Active 时拆窗」。

详细利弊与路径分析见本次会话中的评审。尚未决定是否把并行对账修进本项，还是只让钩子跟集合 diff、接受本地 CompleteNode 的假拆装。
