# 任务系统总设计

## 总体目标

Everglow 的任务系统保留两套领域模型：PlayerSide 提供跟随玩家存档的个人、可重复体验；WorldSide 提供跟随世界存档的静态流程和多人同步。两侧共享展示语义，但不合并 QuestBase、Manager、状态机、Objective、节点、生命周期或存档格式。

统一数据流为：

```text
PlayerSide Manager ── Player adapter ──┐
                                       ├── QuestPresentationService ── QuestPresentationEntry
WorldSide Manager  ── World adapter  ──┘
PlayerSide/WorldSide Actions <──────────── QuestPresentationService.TryExecute
```

## 分层职责

- Quest 层保存真实状态并执行解锁、推进、完成、失败、重试、持久化、同步和发奖规则，是唯一事实来源。
- Presentation 层用两侧独立 adapter 生成统一、只读的 `QuestView` 与 `QuestAction` 快照；`QuestPresentationService` 提供查询和执行入口，`QuestPresentationSystem` 管理 Service 的运行期生命周期。View 不包含 Manager、delegate、领域行为或可变领域集合。

## 身份与状态

`QuestIdentity` 由 `Side`、`DefinitionId` 和 `InstanceId` 组成。

- Player：`DefinitionId = Name`；对象构造时生成 N 格式 GUID 作为 `InstanceId`。Available、Accepted、Cancel 回池以及当前对象的 Retry/Reset 不更换 ID；重新创建对象会产生新 ID。Manager 仍按 `Name` 判重，不支持同定义多实例并存。
- World：固定单实例，`DefinitionId = InstanceId = Name`；运行期 `WhoAmI` 不进入展示身份。

统一展示状态为 Locked、Available、Active、Completed、Failed。Player 的 Available/Accepted/Completed/Failed 分别映射到同名语义（Accepted 映射 Active）；World 的 Locked/Active/Completed/Failed 直接映射。任务到期与其他失败原因统一进入 Failed；展示状态不推导重试、领奖或按钮能力。

## 文本、Hint 与可见性

DisplayName、Description、Hint、Objective 描述和非物品奖励描述均为 `string`，默认空字符串，并可继续携带现有 StringDrawer 标记。包含非空白内容的 Hint 由 adapter 在快照层替换详情：Description 为空、ObjectiveNodes 与 Rewards 为空、Progress 和 ElapsedTime 为 0、TimeLimit 为 `null`；纯空白 Hint 不触发遮蔽。统一遮蔽文本使用 `QuestHintText.Masked`（`"???"`）。

Hint 与 `Visible` 相互独立；adapter 原样导出 Player `IsVisible` 或 World `Visible`，不新增 `IsListed`，也不把不可见自动转换为 Masked Hint。

## 进度、时间、来源、图标与奖励

- Quest 与 Objective 进度在导出时约束到 `[0, 1]`，NaN 归零；Completed Objective 强制为 1，Skipped Objective 强制为 0。
- 时间统一使用游戏帧。Quest 的 ElapsedTime 来自两侧 `Time`；`TimeLimit <= 0` 归一化为 `null`；RemainingTime 不小于 0。Objective 可通过 `WithTimeLimit` 持有独立 `QuestTimer`，只有当前激活目标推进计时，完成判定优先于同一时间片的到期判定。超时目标保持未完成并停止推进，任务重置或重试时计时归零。UI 负责换算秒或分钟。
- QuestType 原样导出，仅作为无行为的展示和筛选标签，不推导状态、操作能力或 UI 布局。
- Source 继续使用 `QuestSourceBase`，空值归一化为 `Default`；仅 Player 有 SubSource。来源图标与任务图标分离，UI 可按 Source/SubSource 单独创建来源图标。
- Icons 永不为 `null`。Player adapter 对现有图标结果生成数组快照并过滤 `QuestSourceIcon`，保留普通任务及汇总后的 Objective 图标；World 当前导出空数组。轮播状态和默认占位图属于 UI，`DrawerItem` 不进入 View。
- Rewards 是 `RewardView` 快照。物品奖励保留任务创建的 `Item` 引用，由 UI 只读展示 Terraria 名称、数量和 tooltip；`Item == null` 时使用 Description 表示非物品奖励。领奖与发放仍属于领域层。

## Presentation Objective 树

`QuestView.ObjectiveNodes` 按领域定义顺序保存互斥的只读节点：Leaf、Parallel、AnyOf（映射 Optional）和 Branch。Branch 包含有序 `ObjectiveBranchView`；未选择、已选择、已排除分别为 Candidate、Selected、Skipped。已完成的已选分支仍为 Selected，不另设分支完成态。

`ObjectiveView` 只含实例内 ID、Description、Progress、可空 Timer 和 Pending/Active/Completed/TimedOut/Skipped 状态。状态优先级固定为：位于已排除分支、Objective 已完成、Objective 已超时、Quest 为 Active 且属于 `FindCurrentObjectives()`、其他。两侧 adapter 都将领域 `QuestTimer` 导出为独立 `TimerView` 快照。Player adapter 通过 `GetObjectivesText(List<string>)` 收集多行描述；World adapter 导出目标定义提供的描述。

两侧具体节点仅向同程序集 adapter 提供最小 `internal` 只读出口：Leaf 的 Objective，Parallel/Optional 的 Objective 序列，Branch 的嵌套分支序列与可空 SelectedBranchIndex。出口使用只读包装，不暴露内部 List、游标、存档键或行为，也不让领域层依赖 Presentation。

## Adapter 与领域边界

adapter 只读取状态、归一化空值与范围并创建数组快照；不得调用 Update、Complete、Reset、Retry、`TryRecordRewardClaim`、`GrantRewardItems` 或 Manager，不得改变任务、Objective、奖励、存档和网络状态，也不得访问 UI。未知领域状态或节点类型应显式失败，避免静默生成错误展示。

Player `InstanceId` 随任务写入玩家存档；加载时只有合法 N 格式 GUID 才覆盖构造 ID，缺失或非法旧数据保留新 ID。World 的任务生命周期与多人领奖均由主世界服务器授权；World Objective 的计时进度随目标存档和全量/单任务网络快照传输。客户端与子服在本地到期时先刷新待发送差量，此后停止上传；主服仅在任务为 Active、目标仍是当前激活实例且尚未超时时接收差量，并只广播已接受的进度。全任务/单任务同步、进度上传、奖励请求与子世界授权发奖由 WorldSide 的 NetSend/NetReceive/Packet 维护。

## Presentation 查询与操作

`QuestPresentationEntry` 在 `QuestView` 外并列携带 `IReadOnlyList<QuestAction>`。Service 每次查询都重新读取当前 Manager 中的领域任务，并连续投影 View 与 Actions；不保存 entry 缓存，也不暴露 Manager 集合。旧 entry 及其集合层级都是数组快照，Manager 后续变化不会反向修改旧结果。

`QuestPresentationService` 同时支持 Player 与 World：

- `GetAll()` 投影并返回两侧全部 entry。
- `TryGet` 使用完整 `QuestIdentity` 做 Ordinal 匹配。Player 必须同时匹配定义名与当前实例 GUID；World 必须满足 `DefinitionId == InstanceId == Name`。
- `TryExecute` 根据 `QuestSide` 调用与对应 Manager 成对的既有 Actions；Actions 重新校验完整 Identity、Hint 和当前可用操作，并以 `bool` 表示执行结果。
- 未知 Side、任务缺失或 Player 实例过期时返回 `false`。adapter 的投影异常直接向调用方暴露。

Player 的 `Submit` 只在 Accepted、已完成且未被 Hint 遮蔽时提供；执行时调用任务既有完成入口，并以是否进入 Completed 作为结果。World 不提供 Submit；Failed 状态下的 `Retry` 仍只在单机导出。Completed 状态下，当前玩家名不在 Ordinal 领取名单时导出 `ClaimReward`。单机执行后直接记录并发奖；多人执行只发送任务名并等待主服快照，玩家身份不进入 `QuestAction`。

WorldSide 领奖只保存 `RewardClaimedPlayers`，不保存全局 bool。主服根据经 `PacketResolver` 校正的真实槽位读取 `Netplay.Clients[whoAmI].Name`：主世界活动玩家本地发奖，子世界玩家通过 `AllDownstream` 授权包交由持有匹配活动槽位的子服发奖。授权包要求服务器来源 `sourceWhoAmI == -1`；普通客户端的来源会被强制改为真实槽位，不能伪造。主服随后广播完整任务快照，`NetReceive` 以服务器名单覆盖本地名单。

## 运行期组合

`QuestPresentationSystem` 在 `PostSetupContent` 从 `PlayerQuestSystem` 和 `WorldQuestSystem` 取得各自成对的 Manager/Actions，并创建 `QuestPresentationService`；World entry 的 Adapter 不接收身份参数，`WorldQuestActions` 仅在投影或执行本地 Action 时读取 `Main.LocalPlayer.name`，不会在构造或专用服务器初始化阶段读取本地玩家。系统不使用 Manager 静态 locator，不注册到 `Ins`，也不解析图形或 UI 服务。`Unload` 清空 Service 引用。

`WorldQuestSystem` 在 `Load` 中为自身 Manager 创建 `WorldQuestActions`，并在 `Unload` 中与 Manager 一并清空引用。
