# PRIDEFALL

A single-player VR action-adventure in the spirit of *Hubris*, set on the
tidally locked planet Khepri, built natively for the **Virtuix Omni One**
(standalone VR headset + 360° omnidirectional treadmill).

You are Pathfinder Aris Voss, crash-landed in the eternal-dusk band of a world
where a hundred-year-old terraforming AI has decided humanity is a weed. Run,
climb, swim, and shoot your way to the heart of its Garden, with your real
legs doing the running.

## Repository layout

```
pridefall-vr/
  docs/
    game-design-document.md     Full GDD: pillars, setting, mechanics, chapters
    omni-one-integration.md     Omni One SDK wiring, calibration, store/build notes
  PridefallUnity/               Unity 2022.3 LTS project (URP, OpenXR, Android)
    Packages/manifest.json
    ProjectSettings/
    Assets/Scripts/
      Core/         Damage model, event bus, pooling, game/checkpoint manager
      Input/        ILocomotionProvider + Omni One adapter + editor simulator
      Player/       Locomotion, climbing, swimming, hands, health
      Weapons/      Sidearm, carbine, spike thrower, cells, fabricator
      Enemies/      Skimmers, Shardbacks, Wardens, Maw Eels, spawners
      Interaction/  Grabbables, climb holds, holsters
      Environment/  Water, geysers, dust storms, low-g zones, checkpoints
      UI/           Wrist HUD, comfort vignette
      Audio/        Audio director, adaptive music
```

## Locomotion: Omni One first, Quest 3 today

The design leans on the treadmill (see GDD pillar 1 and 2):

- All ground movement comes from real gait via `ILocomotionProvider`. There is
  no thumbstick locomotion in the treadmill mode.
- Body (ring) yaw and head yaw are independent; holsters, the wrist HUD, and
  combat encounters are all built around running one way while aiming another.
- Sprint-scaled jumps, flutter-kick swimming, and the Chapter 5 Colossus chase
  are tuned around the player's legs as the input device.

The Omni One SDK sits behind a paid developer license, so a **Quest 3
edition** exists to finish and validate the game first: left stick smooth
locomotion (head-relative), right stick 45 degree snap turn, A to jump,
stick-click to sprint. Everything above the locomotion provider, climbing,
swimming, combat, enemies, crafting, is identical on both platforms. See GDD
section 8 and `docs/quest3-port.md`.

`PlayerRig` picks the first active provider at boot: Omni One (on-device SDK)
-> Omni Connect (PCVR) -> Quest controllers -> `SimulatedLocomotionProvider`
(editor WASD). The Omni adapters stay in every build, dormant behind the
`OMNI_ONE_SDK` / `OMNI_CONNECT_SDK` defines, with zero impact on Quest
builds.

## Getting started

1. Open `PridefallUnity/` in Unity 2022.3 LTS.
2. Assemble the test scene per `Assets/Scripts/README-scene-setup.md`, plus
   the Quest wiring delta in `docs/quest3-port.md` (Quest provider on the
   rig, ground-motion vignette on).
3. Editor playtest: WASD to walk/turn the simulated ring, Shift to run,
   Space to jump. No hardware needed.
4. Quest 3 build: one-time OpenXR settings per `docs/quest3-port.md`, then
   `scripts/build-quest.sh` and `scripts/deploy-quest.sh` (VR Forge harness
   scripts) to a developer-mode headset.
5. Omni One build (later): import the SDK from the Virtuix developer portal
   and follow `docs/omni-one-integration.md` to enable the `OMNI_ONE_SDK`
   define; the rig then prefers the treadmill automatically.

## Status

- Vertical-slice systems code: complete and engine-ready (this repo).
- Quest 3 edition: wired. Controller locomotion provider, ground-motion
  comfort vignette, and the VR Forge build/deploy path (`docs/quest3-port.md`,
  comfort audit in `docs/quest3-comfort-audit.md`).
- Omni One: pending SDK license. Two methods to fill in
  `OmniOneLocomotionProvider` once access lands.
- Art, audio assets, levels: placeholder/blockout stage, not in repo.
