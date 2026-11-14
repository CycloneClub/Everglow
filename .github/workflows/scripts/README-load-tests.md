# tModLoader load checks

`build-and-test.yml` runs in order: repository checks, build and unit tests, content audit, then the server-load matrix. Both load stages use the same packaged `Everglow.tmod`, tML archive and Workshop dependencies produced by the build. If the audit fails, the server-load matrix is skipped.

- `Content names, textures and map templates` runs on Ubuntu with `Test-TModLoaderLoad.ps1 -AuditAssets`. It builds the small `../asset-audit/` helper into a fresh temporary save directory and loads it alongside Everglow. The helper is never added to the shared inputs or the published mod.
- After the audit succeeds, the existing server matrix checks loading on each supported environment in parallel.

The audit reads `Texture` from loaded items, NPCs, projectiles and `ModTexturedType` instances. It also checks every entry in Everglow's equipment registry, including `AutoloadEquip` and explicit `EquipLoader.AddEquipTexture` registrations. Thus an armor inventory icon does not substitute for its `_Head`, `_Body` or `_Legs` sprite.

Paths are checked case-sensitively against the **loaded mod packages**, including cross-mod references and `.png`, `.rawimg` and `.xnb` entries. Missing files are reported with the content/equipment owner and full resource path.

The audit also checks registered internal names and each packaged `.mapio` content index. Everglow names must use uppercase initials after underscores; tile, wall and tile-entity references must resolve to loaded content. Other mods retain their own naming conventions. Template payloads are not interpreted or rewritten.

A failed audit fails the job. One completion marker reports positive counts for internal names, textures, equipment and templates; a normal server load or partial audit cannot satisfy it.

Repository checks reject UTF-8 BOMs and invalid code/resource filenames while preserving external asset and publishing conventions. Roslyn checks type names using `.editorconfig`: PascalCase, uppercase underscore segments and numeric segments are accepted. Naming diagnostics appear as IDE warnings and are enforced as build errors in CI.

The helper accesses tML's internal equipment registry using reflection. If its shape changes, the check fails explicitly and must be updated for that tML version.

## Coverage limits

This is a packaged-file existence check, not a graphical client test. It does not decode images, verify sprite dimensions or frame layout, run shaders, or discover arbitrary `Request` calls in drawing/gameplay code. Beyond the primary `Texture` properties and equipment registry above, other framework-managed paths (NPC head/boss icons, water `BlockTexture`, mount layers) are not covered. Vanilla assets and registrations skipped on a dedicated server are not checked. Custom content-source redirects or dynamically generated textures would need corresponding audit support.

A hosted Ubuntu client probe in [PR #443](https://github.com/CycloneClub/Everglow/pull/443) confirmed that Xvfb/Mesa software OpenGL and SDL initialization work. The unmodified release client then failed Steam initialization before loading mods. Full client testing additionally needs a supported Terraria installation and client authentication; `-nosteam` does not disable release-client Steam initialization. The audit avoids those requirements without changing Everglow or tML.

## Local verification

Run all CI check tests with PowerShell 7 and the .NET 8 SDK. Each test script runs in its own process:

```powershell
./.github/workflows/scripts/Test-CiChecks.ps1
```

For an integration run, use `Test-TModLoaderLoad.ps1` with the same parameters as the workflow and `-AuditAssets`. `RunDirectory` must not exist. Logs, a mod hash and the temporary helper remain there for inspection; no installed mods or enabled-mod list are changed. Avoid a parent directory named `runtime*`, because tML's compiler filters library paths containing `/runtime`.
