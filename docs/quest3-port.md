# Quest 3 Port Guide

Engineer-facing notes for building and running PRIDEFALL on a Meta Quest 3
using the VR Forge harness (`../../vr-forge/`). Design rationale lives in
GDD section 8; this is the wiring.

## 1. Provider chain: why the Omni code stays

`PlayerRig.SelectProvider()` walks a priority list at boot and picks the
first provider whose `IsActive` is true:

1. `OmniOneLocomotionProvider` - on-device treadmill. `IsActive` is false
   unless the `OMNI_ONE_SDK` scripting define is set, so without the SDK it
   compiles to inert no-ops.
2. `OmniConnectLocomotionProvider` - PCVR treadmill via Omni Connect. Gated
   behind `OMNI_CONNECT_SDK` plus Windows-only platform defines.
3. `QuestControllerLocomotionProvider` - active whenever a tracked left XR
   controller with a thumbstick exists. This is what a Quest 3 gets.
4. `SimulatedLocomotionProvider` - always active; wins in the editor on
   desktops without XR controllers (WASD, Shift, Space).

The Omni providers cost nothing in a Quest build: both defines are unset,
so their SDK touchpoints are stripped by the preprocessor, `IsActive`
returns false, and their `Update` bodies early-out. Do not delete them;
they are the whole point of the abstraction.

## 2. Scene wiring delta (from README-scene-setup.md baseline)

On the existing `PlayerRig` object:

1. Add the `QuestControllerLocomotionProvider` component next to the Omni
   and Simulated providers.
2. On it, assign `_head` (the Head camera transform) and `_playSpace` (the
   PlaySpace transform). Snap turn rotates `_playSpace` around `_head`.
3. On `PlayerRig`, assign the new component to the `_questProvider` slot.
4. `ComfortVignette` needs no per-scene checkbox: it asks the rig's active
   provider for `IsArtificial` and engages the tunnel during smooth ground
   motion automatically (thumbstick yes, treadmill no). If the player sets
   the vignette to Off, that choice is respected everywhere.
5. Optional but recommended comfort components on the rig:
   `TeleportController` (right stick forward = arc aim, release = blink;
   assign the right hand, an arc LineRenderer, and a reticle) and
   `RecenterControl` (hold left menu button 1 s to recenter and recalibrate).
   Teleport needs a `ScreenFade` (fullscreen black Image under the Head
   canvas) for blink transitions.
6. Make sure the scene is added and enabled in Build Settings; the build
   script fails on zero enabled scenes.

## 3. One-time Unity project settings (manual clicks)

`Packages/manifest.json` already has `com.unity.xr.management` 4.5.0 and
`com.unity.xr.openxr` 1.10.0. In the editor, once:

- **Project Settings > XR Plug-in Management > Android tab:** enable
  **OpenXR**.
- **Project Settings > XR Plug-in Management > OpenXR (Android):** enable
  the **Meta Quest Support** feature, and add the **Oculus Touch
  Controller Profile** under Interaction Profiles.
- **Player Settings (Android):** IL2CPP, ARM64 only, Vulkan. You can click
  these, but the build script sets them programmatically anyway (below);
  the XR loader and OpenXR feature/profile choices are the parts Unity only
  exposes as editor state, which is why they are manual.

## 4. Build and sideload (VR Forge scripts, already adapted in-repo)

The harness scripts are ported into this repo at `scripts/` (repo root, not
inside `PridefallUnity/`), and `PridefallUnity/Assets/Scripts/Editor/BuildScript.cs`
provides the `Pridefall.Editor.BuildQuest.Development|Release` entry points:
IL2CPP, ARM64 only, Vulkan, ASTC, min SDK 29, linear color space, enabled
scenes, output to `PridefallUnity/Builds/quest.apk`.

```bash
scripts/build-quest.sh Development    # or Release; set UNITY_PATH if Hub globs miss
scripts/deploy-quest.sh               # adb install -r + launch
scripts/run-tests.sh                  # EditMode + PlayMode, writes .claude/state markers
```

App id defaults to `com.meridian.pridefall`; override with `VRFORGE_APP_ID`
(both build and deploy read it).

Headset prep: enable developer mode from the Meta Horizon phone app
(requires a developer account), plug in USB, accept the debugging prompt in
the headset. `deploy-quest.sh` refuses to run unless `adb devices` shows
exactly one authorized device.

## 5. Re-enabling the Omni One later

When the Omni One developer license is purchased:

1. Import the OmniSDK `.unitypackage` from the Virtuix developer portal.
2. Add the `OMNI_ONE_SDK` scripting define for the Android build target.
3. Fill the two marked methods in `OmniOneLocomotionProvider.cs`,
   `ReadGaitVector()` and `ReadBodyYaw()`, from the SDK's Movement module
   (see `docs/omni-one-integration.md` for the expected data shapes).

Nothing else changes. `PlayerRig` starts preferring the treadmill on Omni
hardware, and the Quest provider stays in the build as dead weight measured
in bytes. Comfort adapts by itself: the Omni providers report
`IsArtificial = false`, so the ground-motion vignette and teleport aiming
disable automatically on treadmill hardware.
