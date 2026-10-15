---
name: code-review
description: Review code changes in the Everglow tModLoader mod for correctness, performance, and consistency with project conventions described in AGENTS.md. Use when reviewing PRs or code changes.
---

# Everglow Code Review

Review code changes against conventions established by Everglow maintainers and the tModLoader modding ecosystem. The authoritative project guide is `AGENTS.md` at the repository root — read it before reviewing.

**Reviewer mindset:** Be polite but skeptical. Your job is to speed the review process for maintainers, which includes finding problems the PR author may have missed *and* questioning the value of the PR in its entirety. Treat the PR description and linked issues as claims to verify, not facts to accept. Question the stated direction, probe edge cases, and flag concerns even when unsure.

This skill's rules stack on top of `AGENTS.md`; they never override it. If a rule here conflicts with `AGENTS.md` or a more specific module doc (e.g. `Sources/Everglow.Function/Mechanics/Quest/CONTRACTS.md`), the more specific file wins.

## When to Use This Skill

Use this skill when:
- Reviewing a PR or code change in the Everglow repository
- Checking code for correctness, performance, style, or consistency before merge
- Asked to review, critique, or provide feedback on code changes
- Validating that a change follows the conventions in `AGENTS.md`

## Review Process

### Step 0: Load Project Guide and Relevant Module Docs

**Before analyzing anything**, read:
1. `AGENTS.md` at the repository root — the project guide (architecture, build system, conventions, common pitfalls).
2. Any module doc whose path matches the diff:
   - `Sources/Modules/<Module>/<Module>Module.md` if it exists (e.g. `Sources/Modules/Food/FoodModule.md`)
   - `Sources/Everglow.Function/Mechanics/Quest/{README,CONTRACTS}.md` if Quest code is touched
   - `Sources/Everglow.Function/VFX/VFX.md` if VFX code is touched
   - `Sources/Everglow.Core/Utilities/Utils.md` if Core utils are touched
   - `Documents/源代码编译流程.md` if build pipeline / `Directory.Build.props` is touched
   - `.cursor/rules/unit-testing-terraria-main.mdc` if tests touching `Terraria.Main` are touched
3. Any `CONTRACTS.md` / `TODO.md` / `README.md` under `Sources/` whose path overlaps the diff.
4. The neighboring files (same directory as the changed file) — to learn the local naming/layout convention.

Treat all of these as the rule set for the review. If a required doc cannot be loaded, note it in the review and fall back to a careful first-principles review.

### Step 1: Gather Code Context (No PR Narrative Yet)

Collect code context **before** reading the PR description, linked issues, or existing review comments. You must form an independent judgment of what the code does before being exposed to the author's framing.

1. **Diff and file list**: Fetch the full diff and the list of changed files (`git diff --name-status <base> <head>`).
2. **Full source files**: For every changed file, read the **entire source file** (not just diff hunks). Surrounding code exposes invariants, hook lifecycles, `Ins` service registration, and module conventions. Diff-only review is the #1 cause of false positives in this repo.
3. **Consumers and callers**: If the change modifies a public/internal API, a `ModItem`/`ModNPC`/`ModProjectile`/`ModTile`/`ModSystem`, a hook handler, a VFX Visual, a packet, or a Quest, search for how it is consumed. Grep for `AddContent`, `Ins.VFXManager.Add`, `ModIns.PacketResolver`, `ModContent.Request<...>`, `Ins.HookManager`, template base-class usage (`Everglow.Commons.Templates.*`), and direct type references.
4. **Sibling types and related code**: If the change fixes a bug or adds a pattern in one module/type, check whether sibling modules/types have the same issue (e.g. all modules sharing a `EverglowModule` base, all weapons inheriting `Clubs`/`Whips`/`Yoyos`). tML auto-loading is reflection-driven: a missing `[Autoload]` or wrong `LocalizationCategory` silently drops content.
5. **Resource & localization peers**: When `*.hjson` localization files change, cross-check both `en-US/` and `zh-Hans/` copies exist and stay in sync (both are mandatory per AGENTS.md). When `*.png/.ogg/.fx/.atlas/.ttf` assets change, cross-check the resource type is on the Pack whitelist (`Sources/Directory.Build.props` or the module csproj).
6. **Git history**: `git log --oneline -20 -- <file>` to spot recent churn, prior reverts, or parallel attempts.
7. **Detect build-pipeline-affecting changes**: If the diff touches `Sources/Directory.Build.props`, any `*.csproj`, `Everglow.sln`, `.github/workflows/build-and-test.yml`, or `Sources/Directory.Build.targets`, treat it as a high-risk structural change and verify it stays in sync with the workflow's regeneration step (per AGENTS.md "CI 覆盖 Directory.Build.props").

### Step 2: Form an Independent Assessment

Based **only** on the code context gathered above (without the PR description or issue), answer:

1. **What does this change actually do?** Describe the behavioral change in your own words. What was the old behavior? What is the new behavior?
2. **Why might this change be needed?** Infer the motivation from the code itself. What bug, gap, or content addition does it appear to address?
3. **Is this the right approach?** Would a simpler alternative be more consistent with the codebase?
   - For new weapons/furniture: did the author inherit `Everglow.Commons.Templates.*` instead of writing from zero? (AGENTS.md mandates template inheritance.)
   - For cross-cutting concerns: was an `IModule` created unnecessarily? (Content-only additions don't need `IModule` — `AddContents()` auto-registers.)
   - For client-only logic (drawing, VFX, RenderTarget): is there a `Main.dedServ` / `!Main.dedServ` guard?
4. **What problems do you see?** Identify bugs, edge cases, missing validation, thread-safety issues, performance regressions, module-convention violations, resource-pack omissions, localization-key drift, `ModAsset` path-string hardcoding, and anything else that concerns you.

Write down your independent assessment before proceeding.

### Step 3: Incorporate PR Narrative and Reconcile

Now read the PR description, labels, linked issues (in full), author information, and existing review comments. Treat all of this as **claims to verify**, not facts to accept.

1. **PR metadata**: Fetch the PR description, labels, linked issues, and author. Read linked issues in full — they often contain the repro, root cause, and constraints the change must satisfy.
2. **Related issues**: Search for other open issues/PRs in the same module/area.
3. **Existing review comments**: Avoid duplicating feedback already on the PR.
4. **Reconcile** your assessment with the author's claims. Where your independent reading disagrees with the PR narrative, investigate further — do not simply defer to the author's framing. If the PR claims a bug fix or performance improvement, verify against the code and any provided evidence.
5. **Update your holistic assessment** if the additional context genuinely changes your evaluation. But do not soften findings just because the PR description sounds reasonable.

### Step 4: Detailed Analysis

1. **Focus on what matters.** Prioritize: bugs, performance regressions, server/client correctness (dedServ), module-system violations, build-system drift, localization-key breakage, `ModAsset` path-string hardcoding, missing `[Autoload]`/wrong `LocalizationCategory`, and tML API misuse. Do not comment on trivial style unless it violates `AGENTS.md` or `.editorconfig`.
2. **Consider collateral damage.** For every changed code path, brainstorm: what other scenarios, callers, NPCs, projectiles, modules, or save files flow through this? Could any break or behave differently after this change?
3. **Be specific and actionable.** Every comment should tell the author exactly what to change and why, citing the relevant convention from `AGENTS.md` or module doc.
4. **Flag severity clearly:**
   - ❌ **error** — Must fix before merge. Bugs, localization-key deletion, broken build pipeline, content that won't auto-load, save-incompatible `internal name` renames.
   - ⚠️ **warning** — Should fix. Performance, missing `Main.dedServ` guard, inconsistency with module conventions, missing template inheritance, missing companion `zh-Hans`/`en-US` localization.
   - 💡 **suggestion** — Consider changing. Style, readability, optional re-use of `ModAsset.*` instead of hardcoded paths.
5. **Don't pile on.** If the same issue appears across many files, flag it once on the primary file and list the others.
6. **Respect existing style.** When modifying existing files, the file's current style takes precedence (this repo mixes Chinese and English comments deliberately — don't "fix" that).
7. **Don't flag what CI catches.** Assume `dotnet build` and `dotnet test` will run separately; don't comment on issues they would catch.
8. **Avoid false positives.** Verify the concern applies given full context before flagging. Skip theoretical concerns with negligible real-world probability. If unsure, surface it as a low-confidence question, not a firm claim.
9. **Ensure code suggestions are valid.** Any code you suggest must be syntactically correct, complete, and follow C# / .NET 8 / FNA conventions used in the surrounding file.
10. **Format code suggestions correctly.** Suggested code must match surrounding indentation (this repo uses **tabs** for C# — do NOT switch to spaces in a suggestion) and follow Allman brace style.

### Step 5: Suggestion Syntax for Inline Comments

When an inline review comment proposes a concrete code replacement, use GitHub's suggestion fenced block so the author can apply it with one click:

````markdown
建议改为：

```suggestion
		int damage = Item.damage;
		// ...
```

理由:<引用 AGENTS.md 或模块文档的具体条目>
````

Rules for the suggestion block:
- The fence language **must** be exactly `suggestion` (lowercase, no other word). GitHub renders this as an "Apply suggestion" button.
- The code inside **must** use the same indentation as the surrounding code (tabs for `*.cs` in this repo; verify by reading the file).
- The block must contain **complete, valid** code that would compile if applied — not a sketch.
- Only use a suggestion block when you can write the exact replacement. If you can only describe the fix in prose (e.g. "refactor this method to …"), put it in the `### Detailed Findings` section without a fence.
- **Never emit a no-op suggestion.** If the suggested body is identical to the current line(s), do not use a `suggestion` fence (and usually do not comment at all). Classic failure mode: claiming "add a trailing newline" while suggesting the same line text unchanged — GitHub Apply then either does nothing useful or silently violates repo style.
- Do **not** use the suggestion fence for non-code content (config YAML, hjson, plain prose, `.gitattributes`). For non-C# files, describe the change in prose (or use a non-`suggestion` fence such as ```yaml … ``` / ```hjson … ``` only as an illustration). GitHub's "Apply suggestion" button only fires on the `suggestion` fence inside a review thread adjacent to a diff line, so use it sparingly and correctly.

---

## Incremental Re-review Rules

When the PR already has a prior `## Holistic Review`, apply these rules. They mirror the Holistic Review worker prompt's Step 1 ("Choose Initial vs Incremental Scope") so both the CI worker and a resident/session skill behave the same way.

### Detecting the mode

- **Workflow / agentic run**: If `$EVERGLOW_REVIEW_MODE`, `$EVERGLOW_REVIEW_HAS_CHANGES`, `$EVERGLOW_REVIEW_BASE_SHA`, `$EVERGLOW_PR_HEAD_SHA`, and `$EVERGLOW_PREV_REVIEWED_HEAD` are set, treat them as **authoritative**. Prefer `$EVERGLOW_REVIEW_SCOPE_DIR/{metadata.json,range-diff.txt,patch-diff.txt,incremental-files.txt}` over recomputing the scope yourself. Do **not** recompute scope from a direct previous-head-to-current-head tree diff — after a rebase that includes unrelated upstream changes, that diff is wrong. The pre-agent step already used `git merge-base` against the real base branch for both endpoints.
- **Resident / manual skill** (no env vars): Find the most recent PR review whose body starts with `## Holistic Review` and records a reviewed commit. If none → **initial**. If a prior review exists and the PR patch is unchanged (`git patch-id` of base..head matches the prior reviewed patch) → **no-op**. Otherwise → **incremental**, with the prior review's `commit_id` as the previous reviewed head.

### Initial review

Analyze the complete PR range `base..head` (the PR's actual merge-base-to-head range, not "current master vs head"). Follow the Review Process above in full.

### Incremental re-review

Use **two distinct scopes**:

1. **Cumulative refresh** — read the complete current PR range `base..head` **only** to refresh the holistic verdict (`**Motivation**` / `**Approach**` / `**Summary**`). Compare it with the prior review so the summary accurately reflects the current state after the PR has evolved. Build an **Assessment History** bullet that references the prior review as a Markdown permalink of the form `[review <review_id>](<server>/<repo>/pull/<n>#pullrequestreview-<review_id>)`. State its reviewed commit, its verdict, the current verdict, and whether the assessment is unchanged or changed. Only call an assessment **unchanged** when its verdict, motivation, approach, and risk assessment are **all** unchanged.

2. **Incremental findings** — restrict every new actionable finding to changes between the previous reviewed patch and the current patch. Prefer commit-level `git range-diff` (added / removed / modified PR patches); cross-reference a patch-level diff (captures merge-conflict resolutions range-diff misses) and the file list changed between previous-reviewed-head and current head. Inline findings must point to lines in the **current** base-to-head diff. Do **not** introduce a new finding about code that was already part of the PR at the previous reviewed head, even if an earlier review missed it.

Place the Assessment History bullet list **immediately after `**Summary**:` and before `---`**.

### No-op re-review

When the PR patch has not changed since the prior review (same `patch-id`):

- Do **not** inspect the source patch for new findings and do **not** exit silently.
- Submit a new `COMMENT` review stating that the PR patch has not changed.
- Include the required Assessment History.
- Contain **no** actionable findings.

This records every successful re-review without altering prior reviews.

### What not to do

- Do not modify, hide, supersede, or otherwise remove prior comments or reviews.
- Do not re-flag issues that remain only on unchanged lines from the previous reviewed head.
- Do not skip the cumulative Motivation / Approach / Summary refresh on incremental runs — the verdict must reflect the PR as it stands now.

---

## Review Output Format

When posting the review as a `submit_pull_request_review` safe output, use this exact top-level structure. This ensures consistency across reviews and makes the output easy to scan.

> 📝 **AI-generated content disclosure:** When posting review content to GitHub under a user's credentials — i.e. the account is **not** a dedicated "copilot" or "bot" account — you **MUST** include a concise `> [!NOTE]` disclosure at the bottom. Skip this only if the user explicitly asks to omit it.

### Structure

````markdown
## Holistic Review

**Motivation**: <1-2 sentences on whether the PR is justified and the problem is real>

**Approach**: <1-2 sentences on whether the fix/change takes the right approach>

**Summary**: <✅ LGTM / ⚠️ Needs Human Review / ⚠️ Needs Changes / ❌ Reject>. <2-3 sentence summary of the overall verdict and key points. If "Needs Human Review," explicitly state which findings you are uncertain about and what a human reviewer should focus on.>

<!-- Incremental / no-op only: Assessment History bullets go here, before --- -->
- [review <prev_id>](<server>/<repo>/pull/<n>#pullrequestreview-<prev_id>) — commit `<sha>`, verdict was <…>, now <…>; assessment <unchanged|changed>.

---

### Detailed Findings

#### ✅/⚠️/❌ <Category Name> — <Brief description>

<Explanation with specifics. Reference code, line numbers, interleavings, AGENTS.md sections, etc.>

(Repeat for each finding category. Group related findings under a single heading.)

> [!NOTE] This review was generated by this repository's Holistic Review agentic workflow.
````

### Guidelines

- Begin the review body with `## Holistic Review`, immediately followed by `**Motivation**:`, `**Approach**:`, and `**Summary**:` in that order. Do not add a `### Holistic Assessment` subheading, substitute a `Verdict` field, or rename those fields. For incremental / no-op re-reviews, insert the **Assessment History** bullet list immediately after `**Summary**:` and before `---` (see [Incremental Re-review Rules](#incremental-re-review-rules)).
- **Detailed Findings** uses emoji-prefixed category headers:
  - ✅ for things that are correct / look good (use to confirm important aspects were verified)
  - ⚠️ for warnings or impactful suggestions (should fix, or follow-up)
  - ❌ for errors (must fix before merge)
  - 💡 for minor suggestions or observations (nice-to-have)
- **Cross-cutting analysis** should be included when relevant: check whether sibling modules, callers, or both localization languages are affected by the same issue.
- **Test quality** should be assessed as its own finding when tests are part of the PR (everglow uses MSTest in `Sources/Everglow.UnitTests`; remember the `Program.SavePath = string.Empty` rule before touching `Main`).
- **Summary** gives a clear verdict: LGTM (no blocking issues — use only when confident), Needs Human Review (code may be correct but you have unresolved concerns), Needs Changes (with blocking issues listed), or Reject (this should be closed outright). **Never give a blanket LGTM when you are unsure.**
- Keep the review concise but thorough. Every claim should be backed by evidence from the code or `AGENTS.md`.

### Verdict Consistency Rules

The summary verdict **must** be consistent with the findings in the body. Follow these rules:

1. **The verdict must reflect your most severe finding.** If you have any ⚠️ findings, the verdict cannot be "LGTM." Use "Needs Human Review" or "Needs Changes" instead.
2. **When uncertain, always escalate to human review.** A false LGTM is far worse than an unnecessary escalation.
3. **Separate code correctness from approach completeness.** A change can be correct code that is an incomplete approach. Do not let "the code itself looks fine" collapse into LGTM.
4. **Classify each ⚠️ and ❌ finding as merge-blocking or advisory.** Before writing your summary, ask: "Would I be comfortable if this merged as-is?" If any answer is "no," the verdict must be "Needs Changes."
5. **Devil's advocate check before finalizing.** Re-read all your ⚠️ findings. If any represents an unresolved concern about approach, scope, or risk, the verdict must reflect that tension.

---

## Everglow-Specific Review Checklist

Apply these checks on every PR (in addition to the general process above). For each, the linked `AGENTS.md` section is the authoritative source.

### Build & Module System
- [ ] New modules added to `Sources/Directory.Build.props` `<Modules>`? (AGENTS.md "构建系统关键点") If yes, was `.github/workflows/build-and-test.yml` regenerated to match (CI overwrites this file)?
- [ ] New module `Everglow.<Name>.csproj` is an empty shell (no manual refs/using re-added)? (AGENTS.md "模块目录约定")
- [ ] Cross-module type reference: did the author add a `<ProjectReference>` in the consuming module's csproj? (Don't rely on transitive refs from the main project.)
- [ ] New resource type (not `.hjson/.txt/.png/.ogg/.wav/.mp3/.json/.atlas/.obj/.bmp/.mapio/.ttf`): is it added to the Pack whitelist?
- [ ] Resource path: does code use `ModAsset.*` source-generated constants instead of hardcoded string paths? `Commons.ModAsset.*` for shared-library resources.
- [ ] Duplicate resource filename (within the source-gen scope): source-gen `ModAsset` collides — filenames must be unique.

### tML Content Auto-Loading
- [ ] `ModItem`/`ModNPC`/`ModProjectile`/`ModBuff`/`ModBiome` overrides `LocalizationCategory` with a `LocalizationUtils.Categories.*` constant? (Required for the localization export tool to file the entry correctly.)
- [ ] No manual `AddContent` for content classes — `AddContents()` already scans all `Everglow.*` assemblies and registers them.
- [ ] New `IModule` is only added when the module actually needs hooks/preload/sky/event registration; plain content additions don't need it.
- [ ] `[Autoload]` semantics respected; `[ModuleHideType]` used where a type should not appear in `ModuleManager.Types`.
- [ ] `NPC.ai[]` / `Projectile.ai[]` indices wrapped in private enum + property, not scattered magic numbers.

### Server/Client Correctness
- [ ] Client-only logic (draw, VFX, RenderTarget, SpriteBatch, `Device`) guarded by `!Main.dedServ` or `Main.dedServ` blocks.
- [ ] No `SpriteBatch`/`GraphicsDevice`/`RenderTargetPool`/`VFXBatch` use in code that may run on the server (these services are only registered on the client).
- [ ] Network state changes set `netUpdate = true`.
- [ ] Custom packets go through `ModIns.PacketResolver` (`IPacket`/`IPacketHandler`), not raw `ModPacket`/`ReadPacket` patterns outside that framework.

### Localization
- [ ] Keys only added, never removed or renamed. (Renaming an `internal name` breaks `ItemID/NPCID.Search`, save compatibility, and existing hjson keys chain-break.)
- [ ] Both `en-US/` and `zh-Hans/` entries created for every new content key.
- [ ] No edits to `Localization/templates/` (templates must not be filled with real content).
- [ ] No key added to the legacy root `Localization/en-US_Mods.Everglow.hjson` (new content goes under `Sources/Everglow/Localization/`).
- [ ] When code reads text, uses `Language.GetTextValue("Mods.Everglow.Common.*")`; tile map names use `this.GetLocalization("MapEntry" + option)`.

### Conventions
- [ ] C# files use file-scoped namespace (`namespace Everglow.<Module>.<Area>.<Type>;`).
- [ ] Tab indentation (width 4), LF, trailing newline, UTF-8 without BOM, Allman braces, braces mandatory on control statements.
- [ ] No new `?` nullable annotations in `Sources/Everglow` or `Sources/Everglow.Function` (only `Everglow.UnitTests` enables `<Nullable>`).
- [ ] Don't mass-reformat historical files — style is mixed deliberately (AGENTS.md "最小改动").
- [ ] No binary/art assets committed (`.png/.obj/.ttf/.atlas/.xnb` under `Resources/`, `Libraries/`, module assets, `Tools/*.exe|dll`, `icon*.png` are read-only — do not modify).

### Tests
- [ ] `[TestInitialize]` runs `Program.SavePath = string.Empty;` before any `Terraria.Main` static member is touched.
- [ ] Tests don't `new Main()`, don't start graphics/content/real game loop.
- [ ] `Player.talkNPC` private setter assigned via reflection, never via `SetTalkNPC` (which trips `ShopHelper` NRE).
