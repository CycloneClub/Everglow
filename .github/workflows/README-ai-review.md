# Everglow AI Review Bot

仿照 [dotnet/runtime](https://github.com/dotnet/runtime/tree/main/.github/workflows) 的 Holistic Review 机制,为 Everglow 仓库接入 GitHub PR 自动审查。

**事件驱动**:PR `opened` / `synchronize` / `reopened` / `ready_for_review` 时直接跑 worker;也可 `workflow_dispatch` 指定 PR 号强制复评。Agent 读 `AGENTS.md` 与审查 skill,经增量复评规则分析后,用 GitHub Review API 提交一份 `## Holistic Review` + 行内建议(带 `suggestion` 一键应用块)。

**完全开源、零成本、不绑 Copilot 订阅**——默认 Copilot BYOK + DeepSeek,也可换 Kimi/OpenAI/Anthropic 等。

## 架构

```
.github/workflows/holistic-review.md                 ← gh-aw 源(frontmatter + prompt + pre-agent-steps)
.github/workflows/holistic-review.lock.yml           ← `gh aw compile` 生成,需提交
.github/workflows/agentics-maintenance.yml           ← `gh aw compile` 自动生成的维护工作流,需提交
.github/skills/code-review/SKILL.md                  ← 审查流程、增量复评规则、Everglow 约定、输出格式
AGENTS.md                                            ← 项目指南,SKILL 强制 Step 0 必读
```

> `.github/aw/`(gh-aw 编译缓存,已加入 `.gitignore`,不要提交)
>
> 旧版 cron orchestrator + PR 上的 "Workflow state" JSON 评论已移除;状态改由 PR 自身的 `## Holistic Review` 历史推导。

### 运行时数据流

```
pull_request / workflow_dispatch
        │
        ▼
 pre-agent-steps(可信,非 LLM)
   ① 查 PR reviews → 最近一次 ## Holistic Review 的 commit_id
   ② checkout PR head,清掉 agent-config 路径,从 default branch 覆盖回来(trust overlay)
   ③ 用 merge-base 算 scope → $EVERGLOW_REVIEW_SCOPE_DIR
      (initial / incremental / no-op + range-diff / patch-diff)
        │
        ▼
 LLM agent(读 SKILL.md + AGENTS.md + scope)
        │
        ▼
 safeoutputs CLI(白名单校验)
   ├─ create_pull_request_review_comment  → POST .../pulls/{n}/comments  (行内,最多 10 条,锁 side=RIGHT)
   └─ submit_pull_request_review          → POST .../pulls/{n}/reviews   (整体,锁 event=COMMENT)
```

> 这是 GitHub 官方 [gh-aw](https://github.com/github/gh-aw) 的"agent 不持 token,只通过白名单校验过的 safe outputs 调 review API"模式。dotnet/runtime 也是这套。

### Trust overlay

Agent 看到的是 **PR head 的源码** + **default branch 的规则文件**(`.github/`、`AGENTS.md` 等)。PR 改动的 skill / prompt / AGENTS.md **不会**被本次审查采信,避免 PR 作者改规则绕过审查。

### 增量复评(Initial / Incremental / No-op)

Pre-agent 用 `git patch-id` 对比"上次 Holistic Review 时的 PR patch"与"当前 PR patch":

| 模式 | 条件 | 行为 |
|---|---|---|
| **initial** | 尚无匹配的 Holistic Review | 审完整 `merge-base..head` |
| **incremental** | 有先验,且 patch 已变 | 整体 verdict 刷新 + 新 finding 只盯增量;写 **Assessment History** |
| **no-op** | 有先验,且 `patch-id` 相同 | 仍交一份 COMMENT,说明未变,无 actionable findings |

规则正文在 `.github/skills/code-review/SKILL.md` 的 **Incremental Re-review Rules**(常驻 skill 与 CI worker 共用)。Worker prompt 的 Step 1 与之对齐。

## 启用步骤(4 步)

### 1. 装 `gh aw` CLI 扩展

需要本机先有 `gh`(`winget install GitHub.cli` 或见 https://cli.github.com),然后:

```powershell
gh extension install github/gh-aw
gh extension upgrade gh-aw        # 已装可升级
gh auth login --scopes repo,workflow   # 顺便确保有 workflow scope
```

### 2. 配置模型 API Key 与可选 Variables

```powershell
gh secret set LLM_API_KEY --repo <你的用户名>/Everglow
```

粘贴所选服务商的 API key。默认使用 DeepSeek Anthropic-compatible 接口与 `deepseek-v4-pro`。如需切换服务商或模型,设置仓库 Variables:

```powershell
gh variable set LLM_BASE_URL --body "https://api.moonshot.ai/anthropic" --repo <你的用户名>/Everglow
gh variable set LLM_MODEL --body "kimi-k3" --repo <你的用户名>/Everglow
```

允许的服务商域名为 `api.anthropic.com`、`api.deepseek.com`、`api.moonshot.ai`、`open.bigmodel.cn`、`api.minimaxi.com`、`api.minimax.io` 与 `openrouter.ai`;Base URL 必须使用对应服务商的 Anthropic-compatible 路径。

> ⚠️ **不需要 OpenAI / Codex / Copilot 订阅。** 默认引擎是 Copilot CLI 的 **BYOK** 模式。仓库里只要有 `LLM_API_KEY` 即可;`LLM_BASE_URL` 与 `LLM_MODEL` 未设置时分别回退到 `https://api.deepseek.com/anthropic` 与 `deepseek-v4-pro`;不必设 `CODEX_API_KEY` / `OPENAI_API_KEY` / `COPILOT_GITHUB_TOKEN`。

### 3. 生成 `holistic-review.lock.yml`(gh-aw 编译)

`holistic-review.md` 是人写的源;GitHub Actions 实际跑的是编译后的 `.lock.yml`。在仓库根执行:

```powershell
gh aw compile
```

第一次运行会创建 `.github/aw/`(编译缓存,已加入 `.gitignore`)和两个 workflow 文件:`.github/workflows/holistic-review.lock.yml` 与 `.github/workflows/agentics-maintenance.yml`。**源 + lock + maintenance + skill + 本 README 都要提交**。

### 4. 提交并手动触发一次试跑

```powershell
git add .github/workflows/holistic-review.md `
        .github/workflows/holistic-review.lock.yml `
        .github/workflows/agentics-maintenance.yml `
        .github/skills/code-review/SKILL.md `
        .github/workflows/README-ai-review.md `
        .gitignore
git commit -m "ci(ai-review): add Holistic Review workflow"
git push
```

手动跑一次验证(挑一个开 PR 的编号,比如 #42):

```powershell
gh workflow run "Everglow Holistic Review" `
    --repo <你的用户名>/Everglow `
    -f pr_number=42
```

也可以直接往开着的 PR push 一次(触发 `synchronize`)。跑成功后 PR 上会多一条 `## Holistic Review` review。

## 切换引擎

默认已是 **Copilot BYOK → DeepSeek**(Anthropic 兼容端点)。**不要用 `engine.id: codex` 接 DeepSeek**:Codex CLI 走 OpenAI Responses API(`/v1/responses`),DeepSeek 没有该接口,会表现为打 `api.openai.com` 的 401,或打 DeepSeek `/v1/responses` 的 404。

`holistic-review.md` 的 `engine:` 块可改成别的:

| 引擎 | provider | 文档明确支持 | 协议风险 | 配置差异 |
|---|---|---|---|---|
| **DeepSeek BYOK**(默认) | `copilot` + `COPILOT_PROVIDER_TYPE=anthropic` + `https://api.deepseek.com/anthropic` | ✅ | ✅ 走 Anthropic Messages,避开 Responses/reasoning_content 坑 | secret `LLM_API_KEY` only(BYOK 跳过 `COPILOT_GITHUB_TOKEN`) |
| **DeepSeek OpenAI 兼容** | `copilot` + `TYPE=openai` + `https://api.deepseek.com/v1` + `WIRE_API=completions` | ✅ | ⚠️ 多轮可能因 `reasoning_content` 回传 400 | 同上 |
| **其他白名单服务商** | `copilot` BYOK,设置 `LLM_BASE_URL` / `LLM_MODEL` | ✅ | ✅ 限 Anthropic-compatible 接口 | secret `LLM_API_KEY` 换成对应服务商 key |
| **OpenAI 官方** | `codex`,设 `OPENAI_API_KEY`,删 BYOK 字段 | ✅ | ✅ 原生 Responses | secret `OPENAI_API_KEY` |
| **Anthropic** | `claude`,换 model,`network.allowed` 增 `api.anthropic.com` | ✅ | ✅ | secret `ANTHROPIC_API_KEY` |
| **Gemini** | `gemini`,同上 | ✅ | ✅ | secret `GEMINI_API_KEY` |
| **Copilot 订阅**(GitHub 路由) | `copilot`,删全部 `COPILOT_PROVIDER_*` | ✅ | ✅ | secret `COPILOT_GITHUB_TOKEN` |

⚠️ **为何默认不用 codex+DeepSeek**:`Execute Codex CLI` 一步即使用 `OPENAI_BASE_URL=https://api.deepseek.com/v1`,仍会调 `/v1/responses`(或绕过代理打 `api.openai.com`/`chatgpt.com`)。DeepSeek 官方只保证 Chat Completions 与 Anthropic Messages。

**`safeoutputs` 的 review API 与引擎无关** —— 换引擎只改 `holistic-review.md` 的 `engine:` 块和 secret,然后重跑 `gh aw compile`。

## 输出长什么样

最终在 PR review 里看到的内容(节选;增量复评会多 Assessment History):

````markdown
## Holistic Review

**Motivation**: PR 加了 Myth 模块的新剑,符合模块化内容新增方向,但使用了手写贴图路径,违反 AGENTS.md "ModAsset 路径" 约定。

**Approach**: 继承 `StabbingSwords` 模板方向正确,但贴图加载顺序应改为通过源生成常量。

**Summary**: ⚠️ Needs Changes. 1 个 ❌ build 系统 / 2 个 ⚠️ 约定问题,需修复才能合并。

- [review 1234567890](https://github.com/<owner>/Everglow/pull/42#pullrequestreview-1234567890) — commit `abc1234`, verdict was Needs Changes, now Needs Changes; assessment unchanged.

---

### Detailed Findings

#### ❌ Build & Module System — 未在 `<Modules>` 注册新模块
新增 `Everglow.Myth` 子模块却没改 `Sources/Directory.Build.props`...

#### ⚠️ ModAsset 路径硬编码 — Sources/Modules/Myth/.../SwordItem.cs:42
应使用 `ModAsset.SwordItem_Mod` 源生成常量(见 AGENTS.md "tModLoader 开发约定")。

> [!NOTE] This review was generated by this repository's [Holistic Review]... agentic workflow.
````

行内评价会带 **一键应用** 的 `suggestion` 块:

````markdown
应使用 ModAsset 常量代替硬编码路径(AGENTS.md "tModLoader 开发约定")。

```suggestion
		Texture2D t = ModAsset.SwordItem_Mod.Value;
```
````

**注意**:Everglow C# 用 **Tab 缩进**,所以 `suggestion` 块内必须用 Tab(不是空格),否则合并后会污染缩进——这一点 SKILL.md 的 Step 5 已经强制要求。

## 关键参数可调

文件 | 默认 | 用途
---|---|---
`holistic-review.md` → `on.pull_request.types` | `opened, synchronize, reopened, ready_for_review` | 触发事件
同上 → `concurrency` | `holistic-review-<pr>` + `cancel-in-progress: true` | 同 PR 新 push 取消旧 run
同上 → `timeout-minutes` | `30` | worker 单次最长跑多久
同上 → `safe-outputs.create-pull-request-review-comment.max` | `10` | 每份 review 最多行内几条
同上 → `safe-outputs.submit-pull-request-review.allowed-events` | `[COMMENT]` | 锁定提交事件,**禁止 approve/request_changes**
repo var `GH_AW_DEFAULT_MAX_DAILY_AI_CREDITS` | `5000` | 日额度护栏(gh-aw runtime)

## 注意事项 / 已知坑

1. **CI 会覆盖 `Sources/Directory.Build.props`**:Everglow 现有 `.github/workflows/build-and-test.yml` 在 restore 前会 echo 覆盖 `Directory.Build.props`(已在 AGENTS.md "CI" 段说明)。改那两个文件别忘了同步 workflow。
2. **Lock 文件是 gh-aw auto-generate 不要手编**:改 `holistic-review.md` 后必跑 `gh aw compile`,commit 时**源与 lock 一起提**。
3. **本地化键只增不删**:review 命中的"删了 hjson 键 / 改了 internal name"这种是 ❌ 必拦项,SKILL.md 已显式列入 everglow-checklist。
4. **服务器/客户端**:VFX/RenderTarget/SpriteBatch 必须 `!Main.dedServ` 守卫——SKILL.md 已列入。
5. **forks**:编译后的 lock 在 activation 阶段要求 `pull_request.head.repo.id == github.repository_id`,来自 fork 的 PR 默认不审(避免烧 token)。
6. **Trust overlay**:PR 改 `.github/skills` / `AGENTS.md` / prompt 不会影响当次审查规则;要改规则必须先合进 default branch。
7. **增量 finding 不翻旧账**:已在上次 reviewed head 里的代码,即使先前漏报,本次也不再新增 finding;只刷新 cumulative verdict。
8. **PR 私有 LLM key 泄露**:key 是 repo secret,agent 只通过 `engine.env` 拿到值,不会嵌入 prompt 文本;`safeoutputs` 的 `body` 也只暴露 review 内容,不泄 key。仍然建议 PR 含敏感关键词时人工 review。
9. **改了 `holistic-review.md` 但忘 compile**:GitHub Actions 会跑旧 lock 直到下次 commit 新 lock。SKILL 没改但 prompt 变了就是这种情况——保持 lock 与源同步即可。

## 调试

跑挂了:

```powershell
gh run watch --repo <你的用户名>/Everglow   # 实时跟随
gh run view --log --repo <你的用户名>/Everglow <run-id>  # 查日志
```

Pre-agent 日志里应能看到 `Previous Holistic Review head: ...` 与 `metadata.json`(`mode` / `has_changes` / `current_patch_id`)。

worker 出 `model_not_supported_error` / `inference_access_error`:换引擎,见上表。

worker 出 `mcp_policy_error` / `http_400_response_error`:多半是 OpenAI 兼容服务不实现 Responses API → 切 Copilot BYOK 模式。

`safeoutputs` 报 JSON 校验失败:检查 `body` 字符串里的 `\n` 与 `\"` 转义、Tab 字符(LF/CRLF 也会让它崩,JSON 字符串里必须用 `\n`)。

## 参考来源

- dotnet/runtime 的 holistic-review 系列文件:`.github/workflows/holistic-review.{md,lock.yml}`、`.github/skills/code-review/SKILL.md`
- gh-aw 文档:`https://github.com/github/gh-aw`
- gh-aw 自定义端点:`docs/src/content/docs/reference/engines.md`(`OPENAI_BASE_URL` / `engine.api-target`)
- 安全沙箱:`https://github.com/github/gh-aw/blob/main/docs/src/content/docs/introduction/architecture.mdx`
