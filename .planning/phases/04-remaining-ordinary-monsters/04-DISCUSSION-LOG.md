# Phase 4: Remaining Ordinary Monsters - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-09-15
**Phase:** 4-remaining-ordinary-monsters
**Areas discussed:** 范围与壳策略, 缺图与美术处理, 区域刷怪与地形依赖, 掉落与缺失材料

---

## 范围与壳策略

| Question | Selected | Alternatives considered |
|----------|----------|-------------------------|
| 实现范围 | **21 只（23−2 hardmode）** | 含 hardmode 共 23 只; 仅做已定义行为者 |
| 无行为定义 | **身份壳 + blocker** | 自行推测行为; 整只延后 |
| 依赖未实现系统 | **能做就做 + blocker** | 顺带实现系统; 整只延后 |
| 复杂生物（BIO-02） | **逐例判定（安全优先）** | 全部完整实现; 复杂组延后 |

**Notes:** 沿用 D-29/D-30/D-18 的壳与 blocker 策略；每例判定记录在 `04-DEVIATIONS.md`。

---

## 缺图与美术处理

| Question | Selected | Alternatives considered |
|----------|----------|-------------------------|
| 缺图回退 | **White_Mod 回退 + 精确 blocker** | 等美术再做; 借用现有贴图 |
| 美术迁移 | **旁加 `.png` + 移除 blocker** | 到时重构类 |
| 缺图防护 | **门脚本断言无缺失引用** | 不加断言 |
| 美术边界 | **不碰任何美术资源** | 允许占位图 |

**Notes:** 沿用 D-13/D-14；本阶段不创建/修改任何 `.png`/`.obj`/`.xnb`。

---

## 区域刷怪与地形依赖

| Question | Selected | Alternatives considered |
|----------|----------|-------------------------|
| 刷怪门 | **层级谓词 `IsKelpCurtainLayer` + 区域 blocker** | 现在就建区域谓词; 不做自然刷怪 |
| 地形依赖 | **不硬依赖未实装地形** | 直接引用地形瓦片; 延后到地形完成 |
| 条件权重 | **设计表 + 保守默认** | 自行设定 |
| 服务器安全 | **服务器权威 + 图形守卫** | 不处理 |

**Notes:** 区域级细分（森雨幽谷/刺苔庭园）记 blocker 留待 Phase 5–6；谓词必须服务器安全。

---

## 掉落与缺失材料

| Question | Selected | Alternatives considered |
|----------|----------|-------------------------|
| 掉落方式 | **直接掉落表（`NPCLoot`/`ItemDropRule`）** | 掉落袋 |
| 引用范围 | **仅引用已实现物品** | 本阶段补实现缺失物品 |
| 缺失材料 | **留空 + 精确 blocker** | vanilla 临时代替 |
| 概率数量 | **设计表 + 保守默认** | 自行设定 |

**Notes:** 沿用 D-36/D-37/D-38/D-39/D-43；缺失材料（毒腺/牛黄/软体甲壳碎片/亡碧膏/飞棍毛发/枯木碎块）为 item 范围，不在本阶段实现。

---

## the agent's Discretion

- NPC 文件夹布局/类命名，及镜像的现有 NPC。
- 身份壳类的最小内容与 blocker 措辞。
- 具体生物用哪个 vanilla `aiStyle`、何处需要小范围自定义 `AI()`。
- 生物的弹幕/buff/VFX 类是本阶段新增还是记为精确 effect blocker。
- 设计未给出时的保守默认刷怪权重/掉落概率。

## Deferred Ideas

- Hardmode：枯萎之种、枯木人卫士（V2-HARD-01）
- 本地化（全程 D-20）
- 区域级刷怪谓词（Phase 5–6 地形）
- Boss/特殊遭遇（Klein Snake、Giant Winged Dragon、吸血魔毯/VampireMat）→ Phase 7
- 未实现系统（土气/号令、伪装危害、捕捉物）→ 精确 blocker
- 缺失掉落材料 → 记录 blocker
- 实机运行时验证（D-21）→ 与 Phase 2/3 的 UAT 合并延后
