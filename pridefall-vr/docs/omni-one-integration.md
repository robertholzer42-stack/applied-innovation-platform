# Omni One Integration Guide

How PRIDEFALL talks to the Virtuix Omni One, what is confirmed about the
platform, and exactly what to do once you have SDK access. Facts below are
sourced from Virtuix's public docs/portal pages, the public Omni Connect SDK
source, and the legacy Omni SDK source; items marked [PORTAL-GATED] require a
Virtuix developer account to pin down.

## 1. Platform facts

- **Hardware:** Omni One = 360° low-friction treadmill (~4 ft disc, special
  overshoes + foot sensor pods, support-arm vest) + standalone headset.
- **Headset:** customized **Pico 4 Enterprise**: Snapdragon XR2, 8 GB RAM,
  2160x2160 per eye @ 90 Hz, pancake lenses, Android-based OS. Build target
  is therefore an **Android APK, IL2CPP, ARM64**, with the Pico/OpenXR Unity
  stack underneath Virtuix's SDK.
- **Tracked independently:** head, hands, and feet/body direction
  ("three-axis separation"). The treadmill supports walking, running,
  strafing, backpedaling, crouching, kneeling, and 360° turning.
- **Distribution:** games ship via the Omni One store; submissions go through
  the developer portal (developers.virtuix.com) and Virtuix dev relations.
  An **Omni One Developer License** bundle includes a hardware dev kit and
  Unity/Unreal SDK access. [PORTAL-GATED: cert checklist, store cut]

## 2. The two SDK paths

### A. On-device: Omni One Unity SDK (the shipping path)
- Distributed via the developer portal as an **OmniSDK `.unitypackage`**
  (not UPM). Docs: docs.virtuix.com ("Start Developing for Omni One").
- Root namespace **`Omni`**. Platform services (`Omni.Platform.CoreService`,
  user identity, multiplayer/`GameInvite`) initialize via an **Omni One
  Platform Manager prefab** placed in the first scene.
- The **Movement module** is separate and explicitly "does not need
  initialization": it retrieves movement data from the treadmill. Across
  every Virtuix SDK generation that data is a **2D gait vector + body/ring
  yaw decoupled from the HMD**. [PORTAL-GATED: exact member names; the
  doxygen group is `group___Movement` in the portal docs]
- URP is supported and has a dedicated setup page; so does controller/input
  mapping (Pico-style A/B + X/Y controllers).

Wiring in this project: `Assets/Scripts/Input/OmniOneLocomotionProvider.cs`.
After importing the SDK:
1. Add scripting define `OMNI_ONE_SDK` (Android target).
2. Fill the two marked methods, `ReadGaitVector()` and `ReadBodyYaw()`, from
   the Movement group of the portal doxygen. Everything else is done.

### B. PCVR: Omni Connect SDK (dev iteration + Omni One Core support)
Fully public and confirmed (UPM package `com.virtuix.omniconnectsdk`,
v0.1.2): the Omni Connect Windows app writes treadmill data to a
memory-mapped file (`"OmniOneSharedMemory"`, struct
`{ float movementX; float movementY; float armYaw; }`), and the SDK exposes:

```csharp
Virtuix.OmniConnectSdk.OmniConnectManager.Instance
static Vector2 OmniConnectManager.GetMovementVector()  // gait, body frame
static float   OmniConnectManager.GetArmYaw()          // body yaw, degrees
```

Wiring: `Assets/Scripts/Input/OmniConnectLocomotionProvider.cs`, enabled by
the `OMNI_CONNECT_SDK` define on Windows. This is the cheapest way to test
real treadmill feel before on-device SDK access lands.

### Legacy reference (sanity check for the data model)
The original Omni Unity SDK (public mirrors on GitHub) exposed
`OmniMovementComponent` with `currentOmniYaw` (ring angle, degrees),
`OmniMotionData.GamePad_X/Y` (normalized gait vector), a
`couplingPercentage` blending camera-vs-ring steering, and
`GetForwardMovement()/GetStrafeMovement()` consumed by a
`CharacterController.Move()`. PRIDEFALL's `ILocomotionProvider` is the same
model with the coupling fixed at full ring authority (pillar 2 of the GDD).

## 3. How the game consumes the treadmill

```
OmniOneLocomotionProvider      (on-device, OMNI_ONE_SDK)
OmniConnectLocomotionProvider  (PCVR, OMNI_CONNECT_SDK, Windows)
SimulatedLocomotionProvider    (editor WASD fallback)
        |  ILocomotionProvider: BodyYawDegrees, StrideYawDegrees,
        |                       GaitSpeed (m/s), ConsumeJump(), Calibrate()
        v
PlayerLocomotionController     single writer to CharacterController.Move
PlayerRig.BodyRoot             rotated to body yaw; holsters + wrist HUD
ClimbingSystem / SwimmingSystem  take over via SetExternalVelocity()
```

`PlayerRig` picks the first active provider in that priority order at boot
and calls `Calibrate()` (the diegetic "suit fitting" in Chapter 1 re-runs
it). Calibration zeroes the ring yaw against the play space forward axis.

Design rules this enforces:
- The HMD never steers the hips. Holsters, reload ports, and the wrist HUD
  hang off `BodyRoot` (ring yaw), so they stay put while the player looks
  around, the core Omni One affordance.
- Gait is speed-authoritative: the game multiplies by a comfort-tunable
  `MovementGain` (default 1.25x) but never moves when legs don't.
- Assisted jump impulse scales with live gait speed: sprint on the disc,
  press jump, long jump. Low-g zones divide by sqrt(GravityScale).

## 4. Build and submission checklist (standalone)

1. Unity 2022.3 LTS, Android build target, IL2CPP, ARM64 only.
2. XR Plug-in Management + OpenXR (swap in the Pico XR plugin if the portal
   setup guide specifies it [PORTAL-GATED]).
3. URP with mobile-tier settings: 72-90 Hz, MSAA 4x, single realtime shadow
   light, baked GI, fixed foveated rendering if exposed by the Pico stack.
4. Import OmniSDK .unitypackage, add Omni One Platform Manager prefab to the
   boot scene (needed for store entitlements/identity, not for movement).
5. Define `OMNI_ONE_SDK`, fill the two Movement touchpoints, on-device test:
   verify ring decoupling by running forward while aiming 90° off-axis.
6. Submit through developers.virtuix.com; expect a collaboration pass with
   Virtuix dev relations (they "adapt titles for movement" with developers).

## 5. Comfort notes specific to the treadmill

- Real gait kills most vection sickness; the remaining risk windows are
  climbing, swimming, geyser launches, and falls, all of which auto-force
  the strong vignette (`ComfortVignette`).
- Never snap-turn the camera: players physically rotate. The accessibility
  thumbstick mode is the only exception and is clearly labeled.
- Checkpoint every 4 minutes (auto) plus on rest-ledge holds: treadmill
  sessions are physically bounded, respect the player's legs.
