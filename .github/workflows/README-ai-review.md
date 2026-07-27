# Everglow AI Review Bot

仿照 [dotnet/runtime](https://github.com/dotnet/runtime/tree/main/.github/workflows) 的 Holistic Review 机制,为 Everglow 仓库接入两条互补的 GitHub PR 审查流:

- **常规自动 review**:现有可配置 provider workflow 在 PR `opened` / `synchronize` / `reopened` / `ready_for_review` 时运行。
- **高精度按需 review**:有仓库 write 权限的维护者在 PR conversation comment 的第一个词输入 `/holistic-review`,调用 Kimi K3(`reasoning_effort=high`)。Actions UI 的 `workflow_dispatch` 可指定 PR 号执行强制 full review。

两条 workflow 都读取 `AGENTS.md` 与同一审查 skill,保留 initial / incremental / no-op、完整累计 verdict、Assessment History、最多 10 条 inline suggestions 和最终 `COMMENT` review。高精度流使用独立历史,不会把自动 review 当作增量基线。

两条流都使用 Copilot CLI BYOK,不要求 Copilot 订阅;模型调用费用由各自 API provider 计费。

## 架构

```
.github/workflows/holistic-review.md                 ← gh-aw 源(frontmatter + prompt + pre-agent-steps)
.github/workflows/holistic-review.lock.yml           ← `gh aw compile` 生成,需提交
.github/workflows/holistic-review-high-precision.md  ← 手动高精度 review 源(Kimi K3)
.github/workflows/holistic-review-high-precision.lock.yml
                                                    ← 手动高精度 review 的 gh-aw 编译产物
.github/workflows/agentics-maintenance.yml           ← `gh aw compile` 自动生成的维护工作流,需提交
.github/skills/code-review/SKILL.md                  ← 审查流程、增量复评规则、Everglow 约定、输出格式
AGENTS.md                                            ← 项目指南,SKILL 强制 Step 0 必读
```

> `.github/aw/`(gh-aw 编译缓存,已加入 `.gitignore`,不要提交)
>
> 旧版 cron orchestrator + PR 上的 "Workflow state" JSON 评论已移除;状态改由 PR 自身的 `## Holistic Review` 历史推导。

### 运行时数据流

```
pull_request 事件 ──→ 常规自动流 ─────┐
                                      ├─→ trusted pre-agent scope
PR comment /holistic-review ─→ Kimi 流 ┘    → LLM(读 SKILL + AGENTS)
workflow_dispatch ────────────→ 指定流      → safeoutputs
                                               ├─ inline comments(max 10)
                                               └─ one COMMENT review
```

高精度流会先解析并固定 PR head/base,拒绝 fork PR,再读取最近一次 `## High-Precision Holistic Review` 的元数据。发布 safe outputs 前会再次查询 PR HEAD;如果分析期间 HEAD 已变化,整次输出会被拒绝,避免发布过期行号和结论。

> 这是 GitHub 官方 [gh-aw](https://github.com/github/gh-aw) 的"agent 不持 token,只通过白名单校验过的 safe outputs 调 review API"模式。dotnet/runtime 也是这套。

### Trust overlay

Agent 看到的是 **PR head 的源码** + **default branch 的规则文件**(`.github/`、`AGENTS.md` 等)。PR 改动的 skill / prompt / AGENTS.md **不会**被本次审查采信,避免 PR 作者改规则绕过审查。

### 增量复评(Initial / Incremental / No-op)

每条流分别用 `git patch-id` 对比"自身上次 review 时的 PR patch"与"当前 PR patch":

| 模式 | 条件 | 行为 |
|---|---|---|
| **initial** | 尚无本流的匹配 review | 审完整 `merge-base..head` |
| **incremental** | 有先验,且 patch 已变 | 整体 verdict 刷新 + 新 finding 只盯增量;写 **Assessment History** |
| **no-op** | 有先验,且 `patch-id` 与 base tip 都相同 | 仍交一份 COMMENT,说明未变,无 actionable findings |

rebase、force-push、base branch 更新通过 merge-base、range-diff、patch-diff 和持久化 base/patch 元数据处理。若旧 commit 已不可达,高精度流会回退到完整当前 PR finding scope 并在 Assessment History 说明。规则正文在 `.github/skills/code-review/SKILL.md` 的 **Incremental Re-review Rules**(两条 CI worker 共用)。

## 启用步骤

### 1. 装 `gh aw` CLI 扩展

需要本机先有 `gh`(`winget install GitHub.cli` 或见 https://cli.github.com),然后:

```powershell
gh extension install github/gh-aw
gh extension upgrade gh-aw        # 已装可升级
gh auth login --scopes repo,workflow   # 顺便确保有 workflow scope
```

### 2. 配置 provider secret 与可选 Variables

```powershell
gh secret set LLM_API_KEY --repo <你的用户名>/Everglow
gh secret set HIGH_PRECISION_REVIEW_API_KEY --repo <你的用户名>/Everglow
```

`LLM_API_KEY` 供常规自动流使用,默认填写 DeepSeek API key;`HIGH_PRECISION_REVIEW_API_KEY` 供手动高精度流使用,默认填写 Kimi Open Platform API key。两个 secret 名都与 provider 解耦,后续替换 provider 时不必修改 workflow 的 secret 名。

常规自动流默认使用 DeepSeek Anthropic-compatible 接口与 `deepseek-v4-pro`。如需切换服务商或模型,设置仓库 Variables:

```powershell
gh variable set LLM_BASE_URL --body "https://api.moonshot.ai/anthropic" --repo <你的用户名>/Everglow
gh variable set LLM_MODEL --body "kimi-k3" --repo <你的用户名>/Everglow
```

高精度流默认使用 Kimi OpenAI-compatible 接口与 `kimi-k3`。它使用独立的两个 Repository Variables:

```powershell
gh variable set HIGH_PRECISION_REVIEW_BASE_URL --body "https://api.moonshot.ai/v1" --repo <你的用户名>/Everglow
gh variable set HIGH_PRECISION_REVIEW_MODEL --body "kimi-k3" --repo <你的用户名>/Everglow
```

两条流允许的服务商域名均为 `api.anthropic.com`、`api.deepseek.com`、`api.moonshot.ai`、`open.bigmodel.cn`、`api.minimaxi.com`、`api.minimax.io` 与 `openrouter.ai`。常规流的 Base URL 必须使用 Anthropic-compatible 路径;高精度流必须使用 OpenAI Chat Completions-compatible 路径。

> ⚠️ 本次代码不会创建或写入任何 GitHub Secret 或 Variable。合并后必须由仓库管理员在 GitHub 配置。两条流都是 Copilot CLI **BYOK**,不需要 OpenAI / Codex / Copilot 订阅;未设置 Variables 时,常规流回退到 `https://api.deepseek.com/anthropic` 与 `deepseek-v4-pro`,高精度流回退到 `https://api.moonshot.ai/v1` 与 `kimi-k3`;不必设置 `CODEX_API_KEY` / `OPENAI_API_KEY` / `COPILOT_GITHUB_TOKEN`。

### 3. 编译对应 lock 文件

`holistic-review.md` 是人写的源;GitHub Actions 实际跑的是编译后的 `.lock.yml`。在仓库根执行:

```powershell
gh aw compile holistic-review
gh aw compile holistic-review-high-precision
```

只改哪条源 workflow,就只编译哪条,避免无意刷新另一条已在运行的 lock。本仓库当前编译器版本是 **gh-aw v0.83.1**;新增高精度 lock 也必须由该版本生成。不要手改任何 `.lock.yml`。

### 4. 试跑

在同仓库的开放 PR conversation 中,由具有 `admin`、`maintainer` 或 `write` 权限的人发表:

```text
/holistic-review
```

命令必须是评论的第一个词。只响应 PR conversation comment;普通 issue comment、inline review comment、discussion、PR body、源码文本和未显式允许的 bot 都不会激活 agent。外部贡献者即使是 PR 作者也不能直接触发。slash command 会自动添加 eyes reaction 和状态评论。

需要强制 full review 时,从 Actions UI 运行 `Everglow High-Precision Holistic Review`,填写开放 PR 编号。该输入因 gh-aw v0.83.1 的 slash-command schema 限制不能标记为 `required: true`,但 resolver 会拒绝空值、非正整数、关闭的 PR 和 fork PR。

## 切换引擎

常规自动流使用 **Copilot BYOK → DeepSeek**(Anthropic 兼容端点)作为默认配置,也可通过 `LLM_BASE_URL` / `LLM_MODEL` 切换到白名单中的 Anthropic-compatible provider:

| 引擎 | provider | 文档明确支持 | 协议风险 | 配置差异 |
|---|---|---|---|---|
| **DeepSeek BYOK**(默认) | `copilot` + `COPILOT_PROVIDER_TYPE=anthropic` + `https://api.deepseek.com/anthropic` | ✅ | ✅ 走 Anthropic Messages,避开 Responses/reasoning_content 坑 | secret `LLM_API_KEY` only(BYOK 跳过 `COPILOT_GITHUB_TOKEN`) |
| **DeepSeek OpenAI 兼容** | `copilot` + `TYPE=openai` + `https://api.deepseek.com/v1` + `WIRE_API=completions` | ✅ | ⚠️ 多轮可能因 `reasoning_content` 回传 400 | 同上 |
| **其他白名单服务商** | `copilot` BYOK,设置 `LLM_BASE_URL` / `LLM_MODEL` | ✅ | ✅ 限 Anthropic-compatible 接口 | secret `LLM_API_KEY` 换成对应服务商 key |
| **OpenAI 官方** | `codex`,设 `OPENAI_API_KEY`,删 BYOK 字段 | ✅ | ✅ 原生 Responses | secret `OPENAI_API_KEY` |
| **Anthropic** | `claude`,换 model,`network.allowed` 增 `api.anthropic.com` | ✅ | ✅ | secret `ANTHROPIC_API_KEY` |
| **Gemini** | `gemini`,同上 | ✅ | ✅ | secret `GEMINI_API_KEY` |
| **Copilot 订阅**(GitHub 路由) | `copilot`,删全部 `COPILOT_PROVIDER_*` | ✅ | ✅ | secret `COPILOT_GITHUB_TOKEN` |

高精度流默认配置:

```yaml
model: kimi-k3?effort=high
network:
  allowed: [github, api.anthropic.com, api.deepseek.com, api.moonshot.ai, open.bigmodel.cn, api.minimaxi.com, api.minimax.io, openrouter.ai]
engine:
  id: copilot
  args: ["--effort", "high"]
  env:
    COPILOT_PROVIDER_TYPE: openai
    COPILOT_PROVIDER_BASE_URL: ${{ vars.HIGH_PRECISION_REVIEW_BASE_URL || 'https://api.moonshot.ai/v1' }}
    COPILOT_PROVIDER_WIRE_API: completions
    COPILOT_PROVIDER_API_KEY: ${{ secrets.HIGH_PRECISION_REVIEW_API_KEY }}
    COPILOT_MODEL: ${{ vars.HIGH_PRECISION_REVIEW_MODEL || 'kimi-k3' }}
```

切换白名单内的其他 OpenAI Chat Completions-compatible 高精度 provider 时,只需更新 `HIGH_PRECISION_REVIEW_BASE_URL`、`HIGH_PRECISION_REVIEW_MODEL` 与通用 secret `HIGH_PRECISION_REVIEW_API_KEY`,无需修改或重新编译 workflow。只有需要新增域名或切换协议时才修改源文件并运行:

```powershell
gh aw compile holistic-review-high-precision
```

`safeoutputs`、审查 prompt、权限和增量历史与 provider 无关,无需跟着改。不要在评论命令参数中动态选择模型:这会扩大攻击面、使费用和协议行为不可预测。

### Kimi K3 兼容性边界

Kimi 的 OpenAI-compatible endpoint 与模型名可静态接入;gh-aw v0.83.1 也能编译 `openai` + `completions` BYOK 配置。仍需在配置真实 secret 后做一次小 PR canary:K3 的思考型多轮 tool calling 要求客户端正确回传 provider 扩展的 reasoning 内容,而 Copilot CLI 1.0.73 的公开 BYOK 文档只保证 tool calling、streaming 与 Chat Completions 兼容,没有针对 Kimi K3 的端到端兼容承诺。canary 未通过前不要高频使用。

按 300K 输入、15K 输出、50% cache hit 的估算,典型约 ¥4.80/次;每个 PR 主动运行 1～3 次可把典型月成本控制在约 ¥104。实际费用以 Kimi 账单为准。

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
`holistic-review.md` → `concurrency` | `holistic-review-<pr>` + `cancel-in-progress: true` | 同 PR 新 push 取消旧 run
`holistic-review-high-precision.md` → `on.slash_command` | `/holistic-review`,仅 `pull_request_comment` | 按需触发
`holistic-review-high-precision.md` → `on.roles` | `admin, maintainer, write` | 精确权限 allowlist
`holistic-review-high-precision.md` → `concurrency` | `high-precision-holistic-review-<pr>` + `cancel-in-progress: false`,`queue: single` | 同 PR 不取消正在分析的高精度 run,只保留一个排队 run
两条源 → `timeout-minutes` | `30` | worker 单次最长跑多久
两条源 → `safe-outputs.create-pull-request-review-comment.max` | `10` | 每份 review 最多行内几条
两条源 → `safe-outputs.submit-pull-request-review.allowed-events` | `[COMMENT]` | 锁定提交事件,**禁止 approve/request_changes**
repo var `GH_AW_DEFAULT_MAX_DAILY_AI_CREDITS` | `5000` | 日额度护栏(gh-aw runtime)

## 注意事项 / 已知坑

1. **CI 会覆盖 `Sources/Directory.Build.props`**:Everglow 现有 `.github/workflows/build-and-test.yml` 在 restore 前会 echo 覆盖 `Directory.Build.props`(已在 AGENTS.md "CI" 段说明)。改那两个文件别忘了同步 workflow。
2. **Lock 文件是 gh-aw auto-generate 不要手编**:改任一源 `.md` 后只编译对应 workflow,commit 时**源与同名 lock 一起提**。
3. **本地化键只增不删**:review 命中的"删了 hjson 键 / 改了 internal name"这种是 ❌ 必拦项,SKILL.md 已显式列入 everglow-checklist。
4. **服务器/客户端**:VFX/RenderTarget/SpriteBatch 必须 `!Main.dedServ` 守卫——SKILL.md 已列入。
5. **forks**:现有自动流继续按原规则处理;高精度流的 deterministic resolver 明确要求 head/base repo ID 相同,来自 fork 的 PR 不审(避免烧 token)。
6. **Trust overlay**:PR 改 `.github/skills` / `AGENTS.md` / prompt 不会影响当次审查规则;要改规则必须先合进 default branch。
7. **增量 finding 不翻旧账**:已在上次 reviewed head 里的代码,即使先前漏报,本次也不再新增 finding;只刷新 cumulative verdict。
8. **PR 私有 LLM key 泄露**:key 是 repo secret,gh-aw 将真实 BYOK credential 隔离在 API proxy sidecar,agent 容器只见 dummy key;模型不持有 GitHub 写 token。`safeoutputs` 仍是唯一评论出口。
9. **HEAD 变化**:高精度流在 safe-output job 发布前重新查询 PR HEAD,不一致就整批拒绝,需要维护者在新 HEAD 上再次评论命令。
10. **忘记 compile**:GitHub Actions 会跑旧 lock。保持每个源与同名 lock 同步,不要为了更新一条流而无意刷新另一条。

## 调试

跑挂了:

```powershell
gh run watch --repo <你的用户名>/Everglow   # 实时跟随
gh run view --log --repo <你的用户名>/Everglow <run-id>  # 查日志
```

自动流 pre-agent 日志里应能看到 `Previous Holistic Review head: ...`;高精度流应看到 `Previous High-Precision Holistic Review head: ...`。两者都应输出 `metadata.json`(`mode` / `has_changes` / `current_patch_id`)。

worker 出 `model_not_supported_error` / `inference_access_error`:核对模型名、endpoint 与 secret,必要时按“切换引擎”一节替换 provider。

worker 出 `mcp_policy_error` / `http_400_response_error`:多半是 OpenAI 兼容服务不实现 Responses API → 切 Copilot BYOK 模式。

Kimi canary 在首次 tool call 后出 `reasoning_content`、tool result 或 assistant-message 相关 400:视为 Copilot CLI 1.0.73 与 K3 多轮协议不兼容,不要通过删思考内容或降低 review 流程来绕过;先升级并重新验证 gh-aw/Copilot CLI,或替换为已验证的 OpenAI-compatible provider。

`safeoutputs` 报 JSON 校验失败:检查 `body` 字符串里的 `\n` 与 `\"` 转义、Tab 字符(LF/CRLF 也会让它崩,JSON 字符串里必须用 `\n`)。

## 参考来源

- dotnet/runtime 的 holistic-review 系列文件:`.github/workflows/holistic-review.{md,lock.yml}`、`.github/skills/code-review/SKILL.md`
- gh-aw command triggers:`https://github.github.com/gh-aw/reference/command-triggers/`
- gh-aw triggers / roles / bot filtering:`https://github.github.com/gh-aw/reference/triggers/`
- gh-aw Copilot BYOK:`https://github.github.com/gh-aw/reference/engines/`
- Copilot CLI BYOK:`https://docs.github.com/en/copilot/how-tos/copilot-cli/customize-copilot/use-byok-models`
- Kimi API 文档:`https://platform.kimi.ai/docs/api/overview`
