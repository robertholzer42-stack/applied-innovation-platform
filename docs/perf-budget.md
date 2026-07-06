# Quest 3 Performance Budget

Hard numbers the asset and level agents build against, and the comfort
reviewer audits against in Phase 6. Exceed a line item only by trading
another one down, recorded in the GDD's Perf Budget Reference section.

## Frame budget
| Metric | Budget | Notes |
|--------|--------|-------|
| Refresh rate | 72 Hz floor | 13.8 ms per frame, everything included. 90/120 Hz are upgrades, never the plan. |
| CPU frame time | < 11 ms | Leave headroom for runtime spikes (GC, streaming). |
| GPU frame time | < 12 ms | Stale frames > 0 in metrics means you are over. |

## Rendering
| Metric | Budget | Notes |
|--------|--------|-------|
| Draw calls | < 150 per eye | Static-batch environment; GPU instancing for repeated props. |
| Triangles in view | < 750k | Sum of everything the frustum sees, LODs applied. |
| Texture RAM | < 1.5 GB | ASTC compression (set by BuildQuest); 2048 max for hero props, 1024 default. |
| Realtime shadow lights | 1 max | One directional caster or none. Everything else baked. |
| Global illumination | Baked only | Lightmaps + light probes. No realtime GI on device. |
| MSAA | 4x | Set on the URP asset. Do not stack post-process AA on top. |

## Particles and transparency
Overdraw is the silent Quest killer: every transparent layer re-shades the
pixels behind it. Guidance: max 2-3 overlapping transparent layers anywhere
on screen, no full-view transparent quads, prefer fewer large particles over
many small ones, and cap particle systems near the camera (muzzle flashes,
impacts) to under 0.5 s lifetimes via ObjectPool despawn delays.

## Package
| Metric | Budget |
|--------|--------|
| APK size | < 1 GB |
| Startup to first frame | < 15 s |

## How to measure
- Frame timing on device: `adb logcat -s VrApi` while playing; read the
  `FPS=72/72 ... Tear=0,Early=0,Stale=0` lines. Stale > 0 means missed frames.
- Full HUD: install OVR Metrics Tool from the Meta Quest store apps and enable
  the persistent overlay (FPS, GPU util, memory).
- GPU frame capture: RenderDoc for Oculus (Meta's fork) attaches over USB and
  shows per-draw cost and overdraw; use it when draw calls or GPU time exceed
  budget and the cause is not obvious.
- Memory: `adb shell dumpsys meminfo <app id>` (defaults to com.vrforge.game).
- APK size: printed by `scripts/build-quest.sh` on every successful build.
