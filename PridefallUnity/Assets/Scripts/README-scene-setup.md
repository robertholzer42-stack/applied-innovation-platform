# PRIDEFALL — Scene Assembly Guide

How to wire a playable scene from the scripts in this folder. Unity 2022.3 LTS, URP, OpenXR, target device: Virtuix Omni One (standalone Android XR headset).

## 1. Systems object

One root GameObject named `Systems`:

| Component | Notes |
|-----------|-------|
| `GameManager` (Pridefall.Core) | Persists across scenes (DontDestroyOnLoad). Set Comfort defaults here. Auto-checkpoint interval defaults to 240 s. |
| `ObjectPool` (Pridefall.Core) | Scene-level. Pooled instances parent under this object. |
| `AudioDirector` (Pridefall.Audio) | Assign three music loop AudioSources (calm/combat/boss) as children, each with its clip, loop on, play-on-awake off. Leave the one-shot template empty for default 3D sources, or assign one to control rolloff. |

## 2. PlayerRig hierarchy

Exactly as documented in `Player/PlayerRig.cs`:

```
PlayerRig                  (CharacterController, PlayerLocomotionController,
                            PlayerHealth, PlayerRig, SwimmingSystem, ClimbingSystem)
  PlaySpace                (XR origin; TrackedPoseDriver moves children)
    Head                   (Camera, AudioListener)
      VignetteCanvas       (Canvas: Screen Space - Camera or World Space ~0.4 m ahead,
                            ComfortVignette + fullscreen Image with inverted radial sprite)
    LeftHand               (HandController)
      WristAnchor          (empty; +Y out of the top of the wrist)
        WristHudCanvas     (World Space Canvas ~0.12 x 0.08 m, CanvasGroup, WristHud)
    RightHand              (HandController)
  BodyRoot                 (empty; rotated to Omni ring yaw, holsters parent here)
```

Component wiring:
- `PlayerRig`: assign PlaySpace, Head, LeftHand, RightHand, BodyRoot, plus `OmniOneLocomotionProvider` and `SimulatedLocomotionProvider` (both on the PlayerRig object; the rig auto-selects hardware, falls back to simulator in editor).
- `WristHud`: assign the rig, WristAnchor, the CanvasGroup, 3 health pip Images (Image Type: **Filled**), the air group + air fill Image (Filled), a toast `Text`, and a resource `Text`. Plain UnityEngine.UI Text is used throughout, no TMP dependency in code.
- `ComfortVignette`: assign the locomotion controller and the overlay Image. The sprite must be opaque at the rim, transparent in the center.

### CharacterController capsule

| Setting | Value |
|---------|-------|
| Height | 1.8 (overwritten by calibration if the provider reports player height) |
| Radius | 0.3 |
| Center | (0, 0.9, 0) |
| Step Offset | 0.3 |
| Slope Limit | 50 |
| Skin Width | 0.04 |

`PlayerLocomotionController` is the **single writer** to `CharacterController.Move`. Geysers, swimming, and climbing all route through `SetExternalVelocity` / `ReleaseExternalControl`. Never add another script that calls `cc.Move`.

## 3. Layers

| Layer | Used for | Notes |
|-------|----------|-------|
| `Player` | PlayerRig capsule | Geyser/kill/checkpoint triggers filter on this. |
| `PlayerHands` | Hand colliders | Excluded from Player-vs-trigger logic; LowGravityZone already ignores non-CharacterController colliders. |
| `Grabbable` | Grabbable props, climb holds | Hand overlap queries only. |
| `Enemy` | Skimmer/Shardback/Warden colliders | Projectile and geyser damage masks. |
| `Water` | WaterVolume triggers | Exclude from projectile masks. |

Physics matrix: disable `PlayerHands` vs `Player`, and `Water` vs everything except `Player` and `Enemy`.

## 4. Environment pieces

- **GeyserHazard**: empty at the vent base, +Y up. Child telegraph and eruption VFX objects assigned (it toggles them). Column height/radius define the damage + lift capsule.
- **DustStormController**: one per chapter scene. Forward axis = wind direction. It publishes `CurrentSightMultiplier` and `CurrentWind` statics; it never moves the player.
- **LowGravityZone / BubbleVent / CheckpointZone / FallKillVolume**: each needs a trigger collider sized to the volume (Awake forces `isTrigger`). CheckpointZone wants a separate respawn anchor transform on flat ground facing play direction.
- **WaterVolume**: BoxCollider trigger; the top face is the swim surface.

## 5. NavMesh

Use `com.unity.ai.navigation` components (not the legacy static-flag bake): add a `NavMeshSurface` to the level root, agent radius 0.4 / height 1.6 for Shardbacks and Wardens, and bake per scene. Skimmers fly and need no NavMesh.

## 6. Android / OpenXR build checklist (standalone headset)

1. **File > Build Settings**: platform **Android**, Texture Compression **ASTC**.
2. **Player Settings**:
   - Scripting Backend **IL2CPP**, Target Architectures **ARM64 only**.
   - Graphics APIs: **Vulkan** first, **GLES3** as fallback (remove GLES3 once Vulkan is validated on device; keeping both inflates the binary).
   - Minimum API Level 29+, .NET Standard 2.1, Multithreaded Rendering on.
3. **XR Plug-in Management** (Android tab): enable **OpenXR**; add the device's OpenXR interaction/feature profile (Omni One SDK supplies the treadmill input; controllers via the standard Android XR profile).
4. **URP asset**: **MSAA 4x** (cheap on tiled GPUs and load-bearing for VR edge quality), HDR off, one realtime shadow light max, baked lighting everywhere else.
5. **Fixed Foveated Rendering**: enable via the vendor OpenXR extension at level Medium; the comfort vignette hides peripheral resolution loss anyway.
6. Target **72 Hz** minimum; keep combat spaces under 150k tris per view (see GDD section 5).
