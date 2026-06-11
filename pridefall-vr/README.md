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

## Why the Omni One specifically

The whole design leans on the treadmill (see GDD pillar 1 and 2):

- All ground movement comes from real gait via `ILocomotionProvider`. There is
  no thumbstick locomotion in the core game.
- Body (ring) yaw and head yaw are independent; holsters, the wrist HUD, and
  combat encounters are all built around running one way while aiming another.
- Sprint-scaled jumps, flutter-kick swimming, and the Chapter 5 Colossus chase
  only work because the player's legs are the input device.

`SimulatedLocomotionProvider` (WASD) lets you develop everything in the editor
without hardware; `PlayerRig` auto-selects the Omni provider when the
treadmill is present.

## Getting started

1. Open `PridefallUnity/` in Unity 2022.3 LTS.
2. Import the Omni One SDK from the Virtuix developer portal and follow
   `docs/omni-one-integration.md` to enable the `OMNI_ONE_SDK` define.
3. Assemble the test scene per `Assets/Scripts/README-scene-setup.md`.
4. Editor playtest: WASD to walk/turn the simulated ring, Shift to run,
   Space to jump.
5. Device build: Android, IL2CPP, ARM64, OpenXR; sideload to the Omni One dev
   kit headset per the integration doc.

## Status

Vertical-slice systems code: complete and engine-ready (this repo).
Art, audio assets, levels: placeholder/blockout stage, not in repo.
