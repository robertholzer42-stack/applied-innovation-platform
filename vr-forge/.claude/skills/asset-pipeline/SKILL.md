---
name: asset-pipeline
description: Run pipeline Phase 4 — generate, import, and prefab every asset in the manifest via Meshy/Blender/ElevenLabs + Unity MCP. Invoke when the user says /asset-pipeline or asks to generate/produce the game's assets. Argument: path to an asset manifest file (optional; defaults to the asset manifest table in docs/gdd.md).
---

# /asset-pipeline — Phase 4 of pipeline/phases.md

Prime directive from the phase contract: never block the pipeline on asset quality. A placeholder primitive is a success state, not a failure.

## 1. Parse the manifest

- Manifest = the file given as the argument, else the asset manifest table in `docs/gdd.md`. If neither exists, stop: tell the user to run `/new-vr-game` first.
- Each row: asset name, type (model / texture / SFX / music / VO), tag (`generate` / `primitive` / `store`), and any description columns.
- `store` rows: do not generate; list them for the user to acquire and mark BLOCKED(store).
- `primitive` rows: skip generation, go straight to step 4's placeholder path (that is their intended form).

## 2. Fan out generation in parallel batches

Batch the `generate` rows (4-6 per batch) and launch subagents for one batch concurrently, in a single message:

- **Models/textures -> `asset-artist` agent.** Contract per asset: "Generate <name>: <description>. Route: Meshy MCP for organic/hero props, Blender MCP for hard-surface and kit-bash geometry. Save the exported file under `Assets/Generated/<name>/` and report the file path, triangle count, and texture size."
- **SFX/music/VO -> `audio-designer` agent.** Contract: "Generate <name> via the ElevenLabs MCP: <description>. Save under `Assets/Audio/<name>` and report path, duration, format."

Wait for a batch to finish before launching the next; do not interleave batches.

## 3. Import and prefab (per generated asset, via Unity MCP)

1. Import the file into the Unity project through the Unity MCP.
2. Decimate/LOD until the asset fits the per-asset numbers in `templates/perf-budget.md` (triangle and texture caps). Record before/after counts.
3. Create a prefab: mesh + collider (mesh collider only if the budget file allows; otherwise box/capsule approximation) + AudioSource for assets with attached audio.
4. If the Unity MCP is not connected, stop the import stage, keep the generated files on disk, and mark those rows BLOCKED(unity-mcp) — generation still counts as done.

## 4. Failure -> placeholder, always logged

Any generation or import failure after one retry: create a placeholder primitive prefab (cube/capsule/cylinder at the asset's rough dimensions, flat material) and append a row to `docs/asset-swaplist.md`:
`| <asset> | <what failed> | <placeholder used> | <suggested fix/route> |`
Create the file with that header if missing.

## 5. Coverage report (the Phase 4 gate)

Finish with a manifest coverage table, one row per manifest entry:

```
| Asset | Tag | Status (resolved / placeholder / blocked) | Tris/size | Notes |
```

Then totals: X resolved, Y placeholder, Z blocked, and total triangle/texture spend vs `templates/perf-budget.md`. The gate passes only when zero rows are unaccounted for; blocked rows must name their blocker.
