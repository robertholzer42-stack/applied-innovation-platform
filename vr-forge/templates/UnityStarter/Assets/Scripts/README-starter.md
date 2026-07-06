# VRForge Starter: Scene Assembly and One-Time Setup

## Scene assembly (Assets/Scenes/Main.unity, add it to Build Settings)
1. GameObject > XR > XR Origin (VR): creates XR Origin > Camera Offset > Main Camera + two controllers. Exactly one XROrigin per scene (the smoke test enforces this).
2. On XR Origin add: LocomotionSystem, TeleportationProvider, ActionBasedContinuousMoveProvider, ActionBasedSnapTurnProvider (bind actions from the XRI Starter Assets presets).
3. Under Main Camera add the TunnelingVignette prefab from the XRI Starter Assets sample.
4. Add `ComfortController` (VRForge.Player) to XR Origin; assign the continuous move provider and the TunnelingVignetteController. No Starter Assets? Assign the fallback instead: a camera-space Canvas (~0.5 m in front of Main Camera) with a radial vignette sprite Image + CanvasGroup, and wire the CanvasGroup.
5. Add `PlayerHealth` (VRForge.Player) to XR Origin.
6. Create an empty "ObjectPool" GameObject with `ObjectPool` (VRForge.Core).
7. Ground: a 10x10 plane with a TeleportationArea. Grabbables: `XRGrabInteractable` + `GrabbableItem` (set its socket tag); holsters: `HolsterSocket` with the matching accepted tag, parented to the rig.

## One-time project settings (cannot be templated; click once per project)
- Edit > Project Settings > XR Plug-in Management: press Install, then on the Android tab check **OpenXR**.
- XR Plug-in Management > OpenXR > Android: add interaction profile **Oculus Touch Controller Profile**; enable the **Meta Quest Support** feature group.
- Package Manager > XR Interaction Toolkit > Samples: import **Starter Assets**; apply its input action presets (Edit > Project Settings > Preset Manager entries come with the sample).
- Rendering: Assets > Create > Rendering > URP Asset (with Universal Renderer); assign it in Project Settings > Graphics and in every Quality level; set MSAA 4x and disable HDR on the URP asset.
- Everything else (IL2CPP, ARM64, Vulkan, ASTC, Linear color, app id) is set by `VRForge.Editor.BuildQuest` at build time.

## Comfort defaults (see templates/comfort-checklist.md)
- Snap turn ON at 45 degrees; smooth turn is opt-in.
- Teleport AND smooth locomotion both wired; smooth speed <= 3 m/s.
- Vignette default Light, auto-escalates to Strong during smooth locomotion (ComfortController).
- Target 72 Hz; check templates/perf-budget.md before adding scene content.
