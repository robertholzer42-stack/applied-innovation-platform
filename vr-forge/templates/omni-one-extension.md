# Retargeting VR Forge to the Virtuix Omni One

The Quest pipeline carries over almost wholesale; what changes is locomotion
input and the build target's headset runtime.

## Locomotion: swap thumbstick for gait
Adopt the provider pattern from the sibling project ../pridefall-vr:
- `../pridefall-vr/PridefallUnity/Assets/Scripts/Input/ILocomotionProvider.cs`
  defines the contract: BodyYawDegrees (ring direction, independent of HMD),
  StrideYawDegrees, GaitSpeed, ConsumeJump(), Calibrate().
- `OmniOneLocomotionProvider.cs` and `OmniConnectLocomotionProvider.cs` (same
  folder) are working treadmill implementations; `SimulatedLocomotionProvider.cs`
  drives the editor.
Replace the starter's ActionBasedContinuousMoveProvider with a provider-driven
move source, and holster sockets should follow BodyYawDegrees, not the camera.

## SDK defines
Gate treadmill code behind scripting defines so the project still compiles
without the hardware SDKs installed: `OMNI_ONE_SDK` (native Omni One SDK) and
`OMNI_CONNECT_SDK` (Omni Connect, PC-relay). Set them in Player Settings >
Scripting Define Symbols per build profile.

## Build target
The Omni One's bundled headset is Pico-based: replace the OpenXR Meta Quest
Support feature with the PICO Unity OpenXR integration, keep Android, IL2CPP,
ARM64, and Vulkan exactly as `BuildQuest` sets them, and sideload with the
same `adb install` flow (`scripts/deploy-quest.sh` works unchanged once the
app id matches).

## Comfort checklist deltas
Walking on the treadmill supplies real vestibular-ish gait feedback, so the
smooth-locomotion vignette items and the 3 m/s speed cap no longer apply to
walking and running. They still apply to any motion the legs do not produce:
climbing, swimming, vehicles, and scripted platforms keep their vignette and
camera rules. Snap turn becomes irrelevant (the player physically turns in
the ring); remove it from defaults but keep recenter.
