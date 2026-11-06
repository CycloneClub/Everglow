# 玩家具体加成效果

当前实现以具体效果状态驱动 StatBonusPlayer。需要协调属性依赖或执行顺序的内容在原有装备/Buff 钩子设置 bool 标记。每帧由 tML 调用 ResetEffects 清空状态，再由装备/Buff 收集本帧效果。

## 已提交的坠落速度接口

既有两个坠落速度目标及数值调用保留：

~~~csharp
player.AddStat(StatBonus.MaxFallSpeedCap, 2.5f);
player.AddStat(StatBonus.MaxFallSpeedForSpecialBehavior, 10000f);
~~~

普通上限取最小值，特殊动作接管值取最大值；特殊动作优先于缓降。普通加成直接使用原生字段：热带奶昔魔耗减 0.5，熔核减伤加 0.05，纸遮头通用伤害减 0.1、暴击减 10，物质环每档扣 3 点生命上限（最多 24 点）。

## 未提交效果改用直接状态

~~~csharp
player.GetModPlayer<StatBonusPlayer>().OrangeCrown = true;
player.statDefense += defenseBonus;
~~~

没有新增通用目标、回调注册、字段描述符、阶段枚举或通道归并器。原有未提交 StatModifier/Func 注册重载和 StatAdjustment 已移除。

胸章在物品钩子降低 10 个百分点近战暴击率，无攻速近战通过原生继承受影响；血泪晶冠在 PostUpdateMiscEffects 读取最终生命比例。岩石套在 UpdateArmorSet 直接增加原生防御，移动距离仍在原 PostUpdate 累计。

本帧反复设置同一 bool 标记不会重复收集效果。结算按 tML 的正常钩子顺序执行，下一帧重新收集；中枢不额外维护重复执行标记，不检查玩家 active/dead/outOfRange，也不在其他钩子或 Clone 中主动调用 ResetEffects。负恢复位于同一个 Player 的 UpdateBadLifeRegen partial；命中/受击钩子按每次事件运行。

## 按 tML 钩子分文件，方法内按依赖编排

文件直接对应 tML 钩子；依赖关系用方法内的分段标题和等号线表达。
计算直接写在钩子方法内，不再通过 ApplyLifeCapacity、ApplyDefense 等分组方法跳转。
同一效果产生多个目标加成时保持完整，例如残骸只计算一次缺血比例，再施加四项加成。

| 文件 | 方法内的处理顺序 |
| --- | --- |
| StatBonusPlayer.cs | ResetEffects 按所属钩子分组清理状态 |
| StatBonusPlayer.PostUpdateEquips.cs | 生命上限依赖 → 防御依赖 → 独立的坠落速度处理 |
| StatBonusPlayer.PostUpdateMiscEffects.cs | 生命上限依赖 → 魔力上限相关自然恢复加成 → 自然恢复抑制 → 直接回魔 |
| StatBonusPlayer.UpdateBadLifeRegen.cs | 清正恢复及计时 → 累计损失 → Inferno 限制 |
| StatBonusPlayer.PostHurt.cs | 原生分配受击无敌时间后，先加 30 帧，再翻倍 |

新增效果先选择正确的 tML 钩子，再放入方法内对应的依赖分段。
仅在存在真实依赖时规定顺序，例如胸章完成防御调整后，铃兰才读取防御计算伤害。
不建立按物品排列的全局执行列表、优先级、阶段枚举或自动排序。

原 BadLifeRegenPlayer 类型已移除；负恢复内容使用 GetModPlayer<StatBonusPlayer>()。普通事件效果继续使用物品原有 Player。


## 简单装备 Player 的归属

DevilHeartSet、CelesteStoneWaistPendant、WhiteCrown、ThermalConductor 由中枢处理属性依赖及恢复顺序。EmptyFowlbeast、FuneraryGoods、RecoveryBand、SpikedParma、HeatEmblem 恢复物品原有 Player，直接使用原生命中、击杀和受击钩子。MoltenCore 和 TropicalSmoothie 直接使用原生加成，无需中枢状态。物品/Buff 继续负责原生即时加成，只把需要统一时机或顺序的效果标记交给中枢；工具提示与中枢共享原有数值常量。

跨帧计数、冷却、动画、存档/同步和模块专属类型依赖仍由原模块负责。例如 RockArmorSetPlayer、CelticSealPlayer、ConcealSpellPlayer 保留独立状态；硬化枯木之心和蛾眼仍保留其模块内 Buff/武器判定。Function 不反向引用内容模块，也不为迁移引入通用回调注册。

瘟疫 Buff 直接记录负恢复贡献，旧 PlagueDebuffPlayer 中转已移除。食物中毒与芒果仅保留中枢的一份状态。重度饥饿使用原生 Player.starving 处理伤害，中枢不重复扣除生命恢复。
