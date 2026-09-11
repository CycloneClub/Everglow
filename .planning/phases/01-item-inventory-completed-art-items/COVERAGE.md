# API Coverage — Phase 1: Item Inventory & Completed-Art Items

No external API integration: the three Feishu design docs are read through the developer-installed lark-cli CLI only; no external API/SDK/service is linked into or called from the tModLoader mod runtime.

> **Detector over-fire note.** The deterministic detector (`gsd-core/bin/lib/api-coverage.cjs`) fired on two prose rows in `01-RESEARCH.md`: the "`.NET SDK`" build-tooling table entry, and the "Feishu status write-back | Direct API calls" anti-pattern row describing Phase 8. Neither names a service this phase integrates. Phase 1 calls no external HTTP API and links no third-party SDK; the lark-cli fetch is a one-time, read-only developer-tool invocation that produces committed XML evidence, not a runtime integration. This declaration is the reasoned human overrule (per `gsd-core/references/api-coverage.md` §"Declaring no external API integration").
