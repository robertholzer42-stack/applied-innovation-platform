# VR Forge Pipeline: Concept to Headset in 7 Phases

Every game built with the harness moves through these phases. Each phase
names an owner (agent), its MCP/tooling, and a **gate**: the objective
condition that must hold before the next phase starts. The director agent
enforces gates; hooks enforce the mechanical ones.

## Phase 1 - Concept
- **Owner:** `game-director`
- **Input:** the user's one-line idea (from `/new-vr-game`).
- **Work:** pitch doc: fantasy, core verb set, why-VR justification (what the
  game does that flat screens cannot), target session length, comfort rating
  target, scope tier (jam / vertical slice / full).
- **Gate:** user approves the pitch (one AskUserQuestion round, max).

## Phase 2 - Design
- **Owner:** `game-director`, with `vr-comfort-reviewer` consulting.
- **Work:** fill `templates/gdd-template.md`: mechanics, enemies/actors,
  progression, level list, asset manifest (every model, texture, SFX, music
  cue the game needs, each tagged generate/primitive/store), perf budget from
  `templates/perf-budget.md`.
- **Gate:** GDD exists, asset manifest is priced against scope tier, comfort
  section filled. No code before the GDD gate.

## Phase 3 - Scaffold
- **Owner:** `gameplay-programmer` + `build-engineer`.
- **Work:** copy `templates/UnityStarter/` into the project; configure
  OpenXR + XR Interaction Toolkit for Quest (Android, IL2CPP, ARM64, Vulkan,
  MSAA 4x); verify the empty project builds and deploys
  (`scripts/build-quest.sh && scripts/deploy-quest.sh`).
- **Gate:** the starter scene runs on the actual headset at 72 Hz. Deploying
  a gray room on day one is the whole point: every later phase inherits a
  known-good build path.

## Phase 4 - Assets
- **Owner:** `asset-artist` (models/textures) + `audio-designer` (SFX/music/VO).
- **Tooling:** Meshy MCP for organic/hero props, Blender MCP for hard-surface
  and kit-bash geometry, ElevenLabs MCP for audio, Unity MCP for import +
  prefab wiring. Driven by `/asset-pipeline` against the GDD manifest.
- **Work:** generate, import, LOD/decimate to budget, prefab with colliders
  and audio sources. Placeholder-primitive anything the generators do badly;
  never block the pipeline on asset quality.
- **Gate:** every manifest entry resolved (asset or explicit placeholder),
  total triangle/texture budget within `perf-budget.md`.

## Phase 5 - Systems and Levels
- **Owner:** `gameplay-programmer` (systems) + `level-designer` (spaces),
  in parallel; `playtester` runs after each merged system.
- **Work:** implement mechanics against the starter's core (rig, interaction,
  locomotion, pooling, events); block out levels in the live editor via Unity
  MCP; wire encounters. Play Mode tests accompany each system
  (`scripts/run-tests.sh` records green/red for the hooks).
- **Gate:** all GDD mechanics playable start-to-finish in editor; test suite
  green (`.claude/state/` clean).

## Phase 6 - Comfort and Performance QA
- **Owner:** `vr-comfort-reviewer` via `/vr-comfort-audit`.
- **Work:** audit against `templates/comfort-checklist.md` (locomotion
  options, vection, snap turn, vignette, no unrequested camera motion) and
  the perf budget (frame timing on-device via `adb logcat` + OVR metrics).
- **Gate:** zero checklist violations at the declared comfort rating; 72 Hz
  sustained on device in the heaviest scene.

## Phase 7 - Build, Deploy, Play
- **Owner:** `build-engineer` via `/build-and-deploy`.
- **Work:** version bump, release APK (`scripts/build-quest.sh Release`),
  install to headset, smoke-run the first two minutes on-device, capture
  `adb logcat` for crashes.
- **Gate:** the game boots on the headset, no crash in smoke window - it is
  ready to play. Tag the commit.

## Cross-phase rules
- The GDD is living: any phase that discovers a scope change appends to it,
  never silently diverges.
- Hooks are the floor, agents are the ceiling: hooks catch broken syntax,
  failed compiles, and un-tested pushes mechanically; agents are responsible
  for everything hooks cannot see.
- One phase can be re-entered any time; gates are re-checked on re-entry.
