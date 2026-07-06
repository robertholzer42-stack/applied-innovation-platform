# VR Forge

A complete Claude Code harness for building VR games end to end: concept,
design, assets, code, audio, build, and on-headset deployment. Drop this
folder into a new repo (or copy it over an empty Unity project folder), open
it in Claude Code on a machine with Unity installed, and say
`/new-vr-game <your idea>`.

**Primary target: Meta Quest 3** (standalone Android, OpenXR). Chosen because
it is the most reproducible loop: no PC tether, a documented Unity + OpenXR +
XR Interaction Toolkit stack, and one-command sideloading over `adb`. An
extension path for the Virtuix Omni One (treadmill locomotion) is documented
in `templates/omni-one-extension.md`.

## What is in the harness

```
vr-forge/
  .mcp.json               MCP servers: Unity editor control, Blender, Meshy,
                          ElevenLabs (asset + audio generation)
  .claude/
    settings.json         Hooks wiring + permission allowlist
    hooks/                Lifecycle scripts: env check, C# gate, push gate,
                          build gate
    agents/               8 specialist subagents (director, gameplay,
                          level, assets, audio, comfort QA, build, playtest)
    skills/               Pipeline skills: /new-vr-game, /asset-pipeline,
                          /build-and-deploy, /vr-comfort-audit, /playtest
  pipeline/phases.md      The 7-phase concept-to-headset pipeline
  templates/              Unity starter (VR core C#), GDD template,
                          comfort checklist, perf budget, Omni One extension
  scripts/                check-env.sh, build-quest.sh, run-tests.sh,
                          deploy-quest.sh
```

## Prerequisites (local machine)

1. **Unity 2022.3 LTS** with Android Build Support (SDK/NDK/OpenJDK modules).
2. **adb** on PATH (comes with Android Build Support; `brew install
   android-platform-tools` also works) and a Quest 3 in developer mode.
3. **uv** (`pip install uv`) for the Python-based MCP servers.
4. API keys for the generation services you want:
   - `MESHY_API_KEY` (text -> 3D models) - meshy.ai
   - `ELEVENLABS_API_KEY` (SFX, music, voice) - elevenlabs.io
5. Unity MCP bridge package in the Unity project (see below).

Run `scripts/check-env.sh` (also runs automatically at session start) to see
what is missing; nothing else in the harness assumes a tool that this script
does not check for.

## MCP server setup

`.mcp.json` declares four servers. Two work out of the box once keys are set;
two need a one-time local install:

| Server | Purpose | Setup |
|--------|---------|-------|
| `blender` | Scene/prop modeling via Blender | Install the addon from github.com/ahujasid/blender-mcp, then the `uvx blender-mcp` entry works |
| `elevenlabs` | SFX, music, dialogue | Set `ELEVENLABS_API_KEY` |
| `unity` | Live editor control: scenes, components, console, play mode tests | Install the Unity MCP bridge package (github.com/CoplayDev/unity-mcp) into the project via the Package Manager, then update the command path in `.mcp.json` per that repo's README |
| `meshy` | Text -> textured 3D assets | Set `MESHY_API_KEY`; update the command per github.com/meshy-dev/meshy-mcp-server if the package name changed |

The Unity and Meshy entries ship with placeholder commands and a `_comment`
key: exact launch commands change between releases, so confirm them against
each repo's README on install day rather than trusting this file.

## The loop

1. `/new-vr-game <concept>` - Director agent runs Phase 1-2 (concept + GDD),
   scaffolds the Unity project from `templates/UnityStarter/`.
2. `/asset-pipeline <manifest>` - Asset + audio agents generate models via
   Meshy/Blender and audio via ElevenLabs, import and prefab them via the
   Unity MCP.
3. Gameplay/level agents implement systems and block out levels in the live
   editor; the playtest agent runs Play Mode tests after each system lands.
4. `/vr-comfort-audit` - comfort QA agent checks locomotion, vection,
   framerate budget against `templates/comfort-checklist.md`.
5. `/build-and-deploy` - build agent produces the APK
   (`scripts/build-quest.sh`) and installs it on the connected headset
   (`scripts/deploy-quest.sh`). Put the headset on and play.

Hooks enforce the floor the whole way: the session-start hook reports the
environment, the C# hook syntax-checks edited scripts, the push gate blocks
pushes when the project has compile errors recorded, and the stop gate
reminds about unbuilt changes. See `pipeline/phases.md` for the full
phase-by-phase contract of who does what and what "done" means at each gate.

## Honest limits

- Asset generation quality is prototype-grade: Meshy output ships a game jam,
  not a AAA title. Budget artist passes for production.
- The Unity MCP drives a *live editor*; it cannot run headless in CI. CI
  builds use `scripts/build-quest.sh` (Unity batchmode) instead.
- Hooks are POSIX shell; on Windows use WSL or Git Bash.
