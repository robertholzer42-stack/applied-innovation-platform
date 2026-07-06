---
name: new-vr-game
description: Start a new VR game from a one-line concept. Runs pipeline Phases 1-3 (concept pitch, GDD, Unity scaffold + day-one gray-room deploy). Invoke when the user says /new-vr-game or asks to start/create a new VR game. Argument: the one-line game concept (required; ask for it if missing).
---

# /new-vr-game — Phases 1-3 of pipeline/phases.md

You orchestrate. The `game-director` agent does the creative work. Do not write gameplay code in this skill; Phase 2's gate says no code before the GDD gate.

## Phase 1 — Concept

1. Delegate to the `game-director` agent with this contract: "Concept: <arg>. Produce a pitch doc per Phase 1 of pipeline/phases.md: fantasy, core verb set, why-VR justification, target session length, comfort rating target (Comfortable/Moderate/Intense), scope tier (jam / vertical slice / full). Return the pitch as markdown, nothing else."
2. Present the pitch with **one** AskUserQuestion round (options: Approve / Revise with note / Abort). One revision loop back to game-director is allowed inside that single round's answer; do not ask again after that — proceed with the latest version and say so.

## Phase 2 — Design (GDD)

3. Delegate to `game-director` again: "Fill templates/gdd-template.md for the approved pitch. Every section. The asset manifest must list every model, texture, SFX, music cue the game needs, one row each, tagged exactly one of `generate` / `primitive` / `store`, priced against the scope tier. Fill the comfort section against templates/comfort-checklist.md and the perf section from templates/perf-budget.md."
4. Write the result to `docs/gdd.md`. Gate check before continuing: GDD exists, manifest rows all carry a tag, comfort section non-empty. If any fail, send the gap back to game-director once, then fix residuals yourself.

## Phase 3 — Scaffold and gray-room deploy

5. Copy the starter into the project root:
   ```
   cp -R templates/UnityStarter/. .
   ```
   Never overwrite an existing `Assets/` without asking.
6. Verify the environment: `./scripts/check-env.sh`
7. If check-env reports Unity 2022.3 present, run the day-one deploy gate:
   ```
   ./scripts/build-quest.sh && ./scripts/deploy-quest.sh
   ```
   Gate (per Phase 3): the gray starter scene runs on the headset. If no headset is connected, run the build only and mark the deploy step BLOCKED(no headset). If Unity is missing, mark Phase 3 BLOCKED(no Unity) and stop after scaffolding — do not fake a build.
8. On build failure: capture the exact error, hand it to the `build-engineer` agent for one diagnose-and-fix pass, re-run. Report honestly if still red.

## Finish — phase status board

Print exactly this board, filled in:

```
VR Forge — <game name>
Phase 1 Concept    [DONE]                pitch approved by user
Phase 2 Design     [DONE|GAPS]           docs/gdd.md, N manifest rows (G/P/S counts)
Phase 3 Scaffold   [DONE|BLOCKED(why)]   build: <ok/fail>, deploy: <ok/skipped>
Phase 4 Assets     [NEXT: /asset-pipeline]
Phase 5-7          [pending]
```
