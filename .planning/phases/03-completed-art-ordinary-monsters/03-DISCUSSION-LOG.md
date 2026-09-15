# Phase 3: Completed-Art Ordinary Monsters - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md — this log preserves the alternatives considered.

**Date:** 2026-09-15
**Phase:** 3-completed-art-ordinary-monsters
**Areas discussed:** 来源比对与矩阵, 行为实现深度, 刷怪与场景接入, 掉落接线

---

## 来源比对与矩阵 (Source comparison & matrix)

### 矩阵载体

| Option | Description | Selected |
|--------|-------------|----------|
| 新建 03-BIOLOGY.json + .md | 沿用 Phase 1 约定，作为 Phase 3–4 事实源 | ✓ |
| 仅 Markdown 矩阵 | 只写 MD，不建 JSON | |
| 扩展 01-INVENTORY.json | 把生物实体追加进 item 清单 | |

**User's choice:** 新建 03-BIOLOGY.json + .md
**Notes:** 按推荐项；沿用 01-INVENTORY 的每行字段形状。

### 数据源

| Option | Description | Selected |
|--------|-------------|----------|
| 已提交的 biology.xml 快照 | 与 Phase 1/2 一致，不重新抓取 Feishu | ✓ |
| 重新抓取 Feishu | lark-cli XML full fetch 取最新勾选 | |
| 混合（快照为主） | 存疑时重抓单行 | |

**User's choice:** 已提交的 biology.xml 快照

### 完成判定

| Option | Description | Selected |
|--------|-------------|----------|
| 贴图勾选框为唯一判据 | 与 Phase 1 artwork_complete 判定一致 | ✓ |
| 勾选框 + 仓库存在 .png | 更严 | |
| 勾选 OR 仓库存在 | 更宽松 | |

**User's choice:** 贴图勾选框为唯一判据

### 离线门

| Option | Description | Selected |
|--------|-------------|----------|
| 新建 check-biology.ps1 | 覆盖/分类/blocker/无占位图守卫，ASCII PS5.1 | ✓ |
| 仅人工核对 | 不写脚本 | |
| 复用 Phase 1 脚本 | 面向 item，不适用 | |

**User's choice:** 新建 check-biology.ps1

---

## 行为实现深度 (Behavior implementation depth)

### 实现深度

| Option | Description | Selected |
|--------|-------------|----------|
| 完整实现 | 设计行完整的生物做完整行为 | ✓ |
| 仅身份壳 | 只做可加载壳 | |
| 逐项判定 | 按复杂度逐个决定 | |

**User's choice:** 完整实现

### 无行为定义

| Option | Description | Selected |
|--------|-------------|----------|
| 身份壳 + blocker | 沿用 D-18，不编造行为 | ✓ |
| 延后到后续阶段 | 整只延后 | |
| 自行推测行为 | 根据名字/区域推测 | |

**User's choice:** 身份壳 + blocker

### 依赖系统

| Option | Description | Selected |
|--------|-------------|----------|
| 能做的做 + blocker | 实现可行部分，未实现系统记精确 blocker | ✓ |
| 整只延后 | 系统实现前不做 | |
| 顺带实现系统 | 本阶段也实现捕获/状态系统 | |

**User's choice:** 能做的做 + blocker

### AI 方式

| Option | Description | Selected |
|--------|-------------|----------|
| aiStyle 为主 + 局部自定义 | 复用 RiverSlug/VampireMat 先例，不新建基类 | ✓ |
| 全部自定义 AI | 每个生物自写 AI | |
| 新建生物基类 | 统一基类/接口 | |

**User's choice:** aiStyle 为主 + 局部自定义

---

## 刷怪与场景接入 (Spawn & scene integration)

### 空间边界

| Option | Description | Selected |
|--------|-------------|----------|
| 仅子世界内刷新 | 通过子世界判定隔离，绝不污染主世界 | ✓ |
| 允许主世界稀有刷新 | 可能与 BIO-06 冲突 | |
| 仅手动生成 | 不做自然刷新 | |

**User's choice:** 仅子世界内刷新

### 接入点

| Option | Description | Selected |
|--------|-------------|----------|
| 复用现有刷新先例 | 用已有 Yggdrasil Subworld/刷怪 ModSystem | ✓ |
| 新建专用刷新系统 | 为 Kelp Curtain 另起一套 | |
| vanilla SpawnOnPlayer | 雕像式生成 | |

**User's choice:** 复用现有刷新先例

### 条件权重

| Option | Description | Selected |
|--------|-------------|----------|
| 按设计表 + 保守默认 | 设计中缺权重时用保守默认并记录假设 | ✓ |
| 自行设定 | 不参考设计表 | |
| 全部延后 | 到 Phase 4/6 统一调 | |

**User's choice:** 按设计表 + 保守默认

### 服务器安全

| Option | Description | Selected |
|--------|-------------|----------|
| 服务器权威 + 图形守卫 | 沿用 Phase 1/2 约定，`!Main.dedServ` 守卫客户端工作 | ✓ |
| 沿用默认行为 | 由框架默认决定 | |
| 延后服务器检查 | 只保证客户端 | |

**User's choice:** 服务器权威 + 图形守卫

---

## 掉落接线 (Drop wiring)

### 掉落方式

| Option | Description | Selected |
|--------|-------------|----------|
| 直接掉落表 | `NPCLoot()`/`ItemDropRule`，含概率与数量 | ✓ |
| 掉落袋 | 集中用一个袋 | |
| 混合 | 普通直出，稀有用袋 | |

**User's choice:** 直接掉落表

### 引用范围

| Option | Description | Selected |
|--------|-------------|----------|
| 仅引用已实现物品 + blocker | 只引用 Phase 1–2 物品，不新增 item 范围 | ✓ |
| 顺带实现缺失物品 | 本阶段补齐物品 | |
| vanilla 临时代替 | 用 vanilla 物品代替 | |

**User's choice:** 仅引用已实现物品 + blocker

### 概率数量

| Option | Description | Selected |
|--------|-------------|----------|
| 设计表 + 保守默认 | 照设计表，缺则保守默认并记录 | ✓ |
| 自行设定 | 完全自定 | |
| 延后平衡 | 到后续阶段统一 | |

**User's choice:** 设计表 + 保守默认

### 缺失掉落

| Option | Description | Selected |
|--------|-------------|----------|
| 留空 + 精确 blocker | 不引用未实现类型，保证构建/加载 | ✓ |
| 整只生物延后 | 掉落存在前不做 | |
| 先引用后补 | 会导致编译失败 | |

**User's choice:** 留空 + 精确 blocker

---

## the agent's Discretion

- NPC folder layout / class naming under `NPCs/`, and which existing NPC to mirror.
- Exact shell-class contents and blocker wording.
- Which vanilla `aiStyle` fits each creature and where a small custom `AI()` is warranted.
- Whether a creature's projectile/buff/VFX class is added now or recorded as an effect blocker.
- Exact conservative default spawn weights / drop chances where the design is silent.

## Deferred Ideas

- Localization (all phases) — D-20; record in the deferred ledger only.
- Unfinished-art creatures — Phase 4.
- Boss / special-encounter creatures — Phase 7.
- Unimplemented systems (capture rules, status effects, variants, morale) — precise blockers only.
- Runtime verification — needs a live tModLoader client (D-21).
