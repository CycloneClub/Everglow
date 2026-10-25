# 子世界 NPC 自然生成

## 1. 系统介绍

系统通过 tModLoader 的自然生成钩子，为已注册的子世界统一配置候选 NPC、出现条件、相对权重和群体数量。普通刷怪节奏由公共层固定控制；随机机会判定、常规选点、容量统计及加权选择继续由 tML 执行。

[SubworldSpawnSystem](SubworldSpawnSystem.cs) 保存世界策略和区域表，[ManagedSpawnGlobalNPC](ManagedSpawnGlobalNPC.cs) 将其接入三个 hook：

- `EditSpawnRate`：按世界提供的禁刷判定直接设置生成间隔和容量，覆盖原版传入值。野外固定为 `spawnRate = 120`、`maxSpawns = 9`。
- `EditSpawnPool`：在已注册且活动的世界中清空当前候选池，再加入区域与条件匹配的规则。
- `SpawnNPC`：首只 NPC 自然生成后，由公共层按规则追加群体成员。

规则放在区域表中，NPC 自身不负责生成群体。该系统不统一控制 Boss 召唤、城镇入住、道具或 AI 的主动创建入口。未注册世界的候选池和频率保持原流程。

自定义事件只有显式设置 `OverridesNaturalSpawning = true` 时，普通系统才让出候选池、频率和扩群处理；目前灯笼月使用这一方式。默认的小事件不影响普通刷怪。

## 2. 现有规则表

目前只注册了天穹树世界，共 52 条规则，覆盖 45 种 NPC：

| 规则表 | 数量 | 内容 |
| --- | ---: | --- |
| [KelpCurtainSpawnTable](../../../Modules/Yggdrasil/KelpCurtain/NPCSpawn/KelpCurtainSpawnTable.cs) | 30 | 苍苔帘幕陆地、水中、水底、浅水和深水候选；包含装甲虾 2～5 只群体及水黾水面条件 |
| [YggdrasilTownSpawnTable](../../../Modules/Yggdrasil/YggdrasilTown/NPCSpawn/YggdrasilTownSpawnTable.cs) | 22 | 15 种 NPC，按森林白昼/夜晚、灯木特有、浅层/深层矿洞及球冻温床分组；缺失生物留注释占位 |

两张表由 [YggdrasilSpawnRegistration](../../../Modules/Yggdrasil/Common/NPCSpawn/YggdrasilSpawnRegistration.cs) 在 `PostSetupContent` 注册。建筑范围 `TownArea`、晶塔车站和连接轨道内禁止普通自然刷怪；镇外候选和群体成员也不能生成到该范围内；其余位置统一使用野外节奏。

苍苔帘幕的公共条件位于 [KelpCurtainSpawnConditions](../../../Modules/Yggdrasil/KelpCurtain/NPCSpawn/KelpCurtainSpawnConditions.cs)。Firefly/MothWorld、RoomWorld 和 TuskWorld 尚未接入。

一层的环境条件位于 [YggdrasilTownSpawnConditions](../../../Modules/Yggdrasil/YggdrasilTown/NPCSpawn/YggdrasilTownSpawnConditions.cs)：森林按候选地面的灯木草/暮光草区分，温床按球冻分泌块或墙识别，不读取屏幕位置或全局场景计数。矿洞沿用当前生成范围（X 为 40 至世界中线右侧 50 格，Y 为世界底部向上 820 至 50 格），向下前 300 格暂作浅层，余下为深层。森林地面与温床不混入矿洞名单。所有现有候选要求干燥且不在房屋墙内；位置仍受既有天穹镇地层注册范围约束。当前仅重写名单和相对权重，不额外提高温床整体生成频率。

## 3. 写法教程

### 添加规则

在所属区域表的 `AddRules(List<SubworldSpawnRule> rules)` 中追加条目。以现有装甲虾规则为例：

```csharp
using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;
using static Everglow.Yggdrasil.KelpCurtain.NPCSpawn.KelpCurtainSpawnConditions;

namespace Everglow.Yggdrasil.KelpCurtain.NPCSpawn;

public static class KelpCurtainSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ArmoredShrimp>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo)
				? 0.75f : 0f,
			MinGroupSize = 2,
			MaxGroupSize = 5,
		});
	}
}
```

这是现有表的写法示例，不要再次注册同一张表或重复追加同一条规则。普通单只 NPC 省略 `MinGroupSize`、`MaxGroupSize` 即可。

| 字段 | 含义 |
| --- | --- |
| `Type` | NPC 类型，通常使用 `ModContent.NPCType<T>()` |
| `SpawnChance` | 接收 `NPCSpawnInfo`，每次返回当前相对权重；`0` 不入池，省略时返回 `1`。返回值应为有限的非负数 |
| `MinGroupSize` / `MaxGroupSize` | 群体数量范围，默认均为 `1`；包含原生生成的首只 |

同一 NPC 有多条有效规则时，只采用最高权重，并使用该规则的群体数量、区域与落点条件；不会累加权重或再次随机选择配置。最高权重相同时保留最先注册的规则（先比较表的注册顺序，再比较表内条目顺序）。胜出规则若为单只，就不采用其他规则的群体数量。附加成员仅按胜出规则重新检查新位置，不回退到其他规则。群体受实际空间和创建结果限制，可能只生成部分成员。

### 根据环境和世界状态计算权重

时间、天气、进度、玩家状态和已有数量等动态判断都放在 `SpawnChance` 中。函数返回 `0` 就表示条件不满足，不再单独填写 `Condition`。注册时只保存函数，实际选怪时才读取当前状态。

例如下面展示同一种水生 NPC 在不同进度、天气下如何改变权重，数值仅为写法示例，未应用到现有规则：

```csharp
SpawnChance = spawnInfo =>
{
	if (!InKelpCurtain(spawnInfo) || !SpawnConditions.IsWater(spawnInfo))
	{
		return 0f;
	}

	float weight = Main.hardMode ? 1.5f : 0.75f;
	return Main.raining ? weight * 2f : weight;
},
```

简单条件用表达式 lambda，复杂规则可以提取成区域表中的普通方法并赋给 `SpawnChance`。规则仍按区域维护，无需把方法放回 NPC 类，也不需要另写策划文件或外部配置。函数应只读取状态，不创建实体或消耗随机数；群体新增位置会再次调用它进行判定。

`spawnInfo.Player` 是原生本次选点对应的玩家，不使用 `Main.LocalPlayer` 或摄像机位置代替它。`SpawnTileX/Y` 是格坐标，`SpawnTileY` 通常指向地面；身体处的液体从地面上一格读取。可复用的基础条件见 [SpawnConditions](SpawnConditions.cs)。

苍苔帘幕的水柱条件从地面向上测量：`InShallowWater` 为 1～6 格，`InDeepWater` 为超过 6 格，`NearWaterSurface` 为 1～16 格。水黾使用 `CanSpawnOnWaterSurface` 检查同列水面，再由自身 `SpawnNPC` 修正单只落点；不要在这个 hook 或 `OnSpawn` 中生成群体。

### 注册新表或新世界

在所属模块的 `ModSystem.PostSetupContent` 中先注册世界，再收集并注册表。下面是现有天穹树注册类中的调用方式：

```csharp
SubworldSpawnSystem.RegisterWorld(
	new SpawnWorldPolicy(WorldId, () => YggdrasilWorld.InYggdrasil, tilePosition => !InSafeArea(tilePosition * 16f)));

var rules = new List<SubworldSpawnRule>();
KelpCurtainSpawnTable.AddRules(rules);
SubworldSpawnSystem.RegisterRules(
	WorldId, nameof(KelpCurtainSpawnTable), rules, InKelpCurtain);
```

这里的 `WorldId`、`InSafeArea`、`InKelpCurtain` 均定义在 `YggdrasilSpawnRegistration` 中。给已注册世界增加表时只追加 `RegisterRules`，不要重复注册世界。

`RegisterRules` 的区域函数接收候选落点的**像素坐标**；`SpawnWorldPolicy.CanSpawn` 接收玩家中心的**格坐标**。省略时允许生成；`false` 禁刷，`true` 使用统一野外节奏，没有区域密度倍率。

接入其他子世界时，世界策略和区域表放在该内容模块，共享 Function 层不引用具体子世界类型。已迁入表的普通 NPC 不再另写 `SpawnChance()`；tML 的默认返回值为 `0`，由表统一加入候选池。
