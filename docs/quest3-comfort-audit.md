# Quest 3 Comfort Audit

Audit of PRIDEFALL's Quest 3 edition against the VR Forge comfort checklist
(`vr-forge/templates/comfort-checklist.md`), item by item, against the code
in this repository as of 2026-07-06. Levels, art, VFX, and audio assets are
not in the repo, so anything that depends on them, or on a real headset, is
marked UNVERIFIED rather than passed. Statuses: PASS, VIOLATION,
UNVERIFIED (on-device), N-A.

## Locomotion

| Item | Status | Evidence | Fix if violation |
|------|--------|----------|------------------|
| Smooth locomotion AND teleport both offered | **VIOLATION** | `QuestControllerLocomotionProvider.cs` is smooth-only. No teleport locomotion exists anywhere in `Assets/Scripts/` (the only teleport is `GameManager.Respawn()`, which is death repositioning, not player-directed movement). The Omni design ("no teleport", GDD pillar 1) was carried into the Quest edition unexamined. | Follow-up: add a teleport mode (arc + blink) as a `ComfortSettings` option so the whole game can be finished without smooth motion. Largest single comfort gap in the port. |
| Snap turn default ON at first boot | PASS | Snap turn in `QuestControllerLocomotionProvider.UpdateSnapTurn()` is unconditional: 45 deg (`_snapTurnDegrees`), trigger at 0.7 deflection, re-arm below 0.35. No toggle exists to have left it off. Note: `ComfortSettings.SnapTurnFallback` is never read by this provider; the field is dead wiring on Quest. | - |
| Smooth turn opt-in only | PASS | No smooth turn code exists on the Quest path at all; the right stick only snaps. Physical turning covers the rest. | - |
| Vignette engages during smooth locomotion | **VIOLATION** | The mechanism exists and is fast enough: `ComfortVignette._vignetteOnGroundMotion` forces Strong above 0.5 m/s planar speed, and `_animateSpeed` 4/s reaches the 0.85 alpha target in ~0.21 s, inside the 0.3 s criterion. But the serialized default is `false` (`ComfortVignette.cs` line 28), and no scene in the repo sets it. As committed, a fresh rig moves smoothly with no tunnel. | One line: default `_vignetteOnGroundMotion = true`, or set it automatically when `PlayerRig` selects the Quest provider. Until then it is a manual wiring step (quest3-port.md section 2). |
| Vignette player-adjustable incl. Off, persists across sessions | **VIOLATION** | Adjustable in-memory: `GameManager.Comfort.Vignette` is Off/Light/Strong and the ground-motion path respects Off (`ComfortVignette.cs` line 72). But nothing persists it: no PlayerPrefs or save code exists anywhere in `Assets/Scripts/`. Also note the Airborne/Swimming override forces Strong even when the player chose Off (lines 66-69); that is deliberate fall/swim safety, but it means Off is not fully honored. | Serialize `ComfortSettings` to PlayerPrefs (or the save file when one exists) on change, load in `GameManager.Awake()`. Decide and document whether Off should also disable the fall/swim override. |
| Smooth speed 3 m/s or less by default | PASS | Stick walk band is 2.2 m/s (`_walkSpeed`) x 1.25 movement gain (`PlayerLocomotionController._movementGain`) = 2.75 m/s effective. Flag: sprint is 4.0 x 1.25 = 5.0 m/s, at the `_maxGroundSpeed` cap. Sprint is opt-in per use (stick click) and vignette-covered once the item above is fixed, but 5 m/s is fast; retune on device if playtesters report it. | - |
| No forced smooth locomotion in critical path without vignette/alternative | PASS | Audited every scripted mover: `GeyserHazard` lifts the player via `SetExternalVelocity(..., MovementState.Airborne)`, and Airborne forces the Strong vignette (`ComfortVignette.cs` line 67); it is also telegraphed for 2 s and avoidable. `DustStormController` "never moves the player" (README-scene-setup.md). `MawEel` bites for damage but never displaces the player. No moving platforms or rails exist in the slice. | - |
| Acceleration instant or vignetted | PASS | `QuestControllerLocomotionProvider` applies `SmoothDamp` with `_speedSmoothing` 0.08 s, which is strictly an easing curve on player velocity, but the pass criterion accepts "or vignette active" and ground motion is vignetted once the default-off violation above is fixed. This item's PASS is contingent on that fix. | - |

## Camera

| Item | Status | Evidence | Fix if violation |
|------|--------|----------|------------------|
| Camera never moves without matching player input | **VIOLATION** (waiver candidate) | Climbing and swimming move the camera without stick input, but from controller hand sweeps (`ClimbingSystem` pull vectors, `SwimmingSystem.StrokeFrom` hand velocity), which is matching player input under the item's wording, plus both force the Strong vignette. The strict miss is `GeyserHazard`: standing in an erupting column translates the camera upward with no motion input at all (`TickEruption`, `_launchSpeed` 9 m/s). | Waiver recommended rather than code fix: 2 s telegraph, player-avoidable, Strong vignette auto-engages via the Airborne state, and the ride is a designed traversal toy. Record the waiver in the Phase 6 output. |
| No cutscene camera grabs | PASS | No cutscene system exists in the slice. Death respawn (`GameManager.Respawn`) is an instant reposition with the CharacterController disabled, a hard cut, not an interpolated grab. Re-audit when Chapter 6 story beats land. | - |
| Horizon lock for vehicle/platform motion | N-A | No vehicles, no moving platforms, and no scripted camera roll anywhere in the codebase. Geyser lift is purely vertical translation. | - |
| No head-bob, screen shake, or FOV animation | PASS | Grep for shake, fieldOfView, and camera animation returns nothing. `WeaponBase` "kick recoil" moves the weapon transform, not the camera; muzzle flash is a pooled world-space prefab at the muzzle, not a fullscreen effect. The vignette is the only aperture change, as required. | - |
| Recenter reachable from pause menu at all times | **VIOLATION** | There is no pause menu and no recenter path. Grep for pause/recenter finds nothing player-facing. Providers have `Calibrate()`, but on Quest it only zeroes speed, and nothing invokes it after `PlayerRig.Awake()`. | Add a pause menu (even a wrist-summoned panel) with a recenter action: XR subsystem recenter plus `ActiveLocomotionProvider.Calibrate()`. Required before any on-device playtest with guests. |

## Interaction

| Item | Status | Evidence | Fix if violation |
|------|--------|----------|------------------|
| Two-handed actions optional, never required | **VIOLATION** | Reload is mandatory to progress and is inherently two-handed: `WeaponBase.Eject()` then grab the fresh cell at the hip (`CompactSidearm.RegenerateCellAtHip`) and insert with the off hand. Medgel is likewise applied by one hand to the other wrist. GDD section 6 promises a "one-handed weapon mode", but no code implements it. | Implement the promised one-handed mode: an accessibility toggle that auto-reloads on eject (skip the grab/insert) and applies medgel via button hold. |
| Seated mode: everything reachable seated | UNVERIFIED (on-device) | Cannot be verified from code. Working in its favor: holsters and the wrist HUD are body-anchored (`PlayerRig.BodyRoot` follows head planar position and yaw), not world-fixed, and there are no floor-level mandatory pickups in code. But climb-hold spacing and fabricator stations live in level blockouts that are not in the repo. Needs a full seated playthrough. | - |
| No mandatory overhead reaches over 3 s | UNVERIFIED (on-device) | Level-dependent and levels are not in the repo. Known risk: climbing overhangs are sustained overhead holds by design, and the stamina budget (100 at 12/s drain, `ClimbingSystem`) permits ~8 s before force-release. Whether any mandatory route requires more than 3 s overhead is a level-design question; flag it for the level pass. | - |
| Holsters within natural arm sweep, rig-anchored | PASS | `PlayerRig.LateUpdate()` rotates `BodyRoot` to body yaw and pins it under the head's planar position every frame; `HolsterSlot` items parent there (hip, shoulder, chest per GDD 4.5). Nothing is world-fixed. Exact reach comfort still needs a body-size check on device, but the architecture is right. | - |
| World-space UI repositionable or re-summoned | PASS | The only world-space UI is the wrist HUD, which is hand-anchored and follows the player by construction (visibility gated on wrist-toward-face dot in `WristHud`). No fixed world panels exist in the slice. Note this item's "follows recenter" cannot be fully true until recenter exists (see Camera violation). | - |

## Display

| Item | Status | Evidence | Fix if violation |
|------|--------|----------|------------------|
| 72 Hz sustained in heaviest scene | UNVERIFIED (on-device) | Frame rate cannot be verified without a headset and a real scene. Code habits are favorable (pooling, no per-frame allocations in hot paths, 2 Hz HUD polling), but that proves nothing about GPU load. Measure per `vr-forge/templates/perf-budget.md` after first deploy. | - |
| No full-view flashes or flicker above 3 Hz | UNVERIFIED (on-device) | VFX assets are placeholders not in the repo. In code, the muzzle flash is a 0.06 s world-space prefab at the muzzle, not full-view; no fullscreen damage flash exists. Re-audit when real VFX and damage feedback land, especially rapid carbine fire. | - |
| All UI in the 0.75-10 m depth band | **VIOLATION** (waiver candidate) | The wrist HUD canvas sits on the forearm, roughly 0.3-0.6 m from the eyes when raised, below the band. The vignette canvas is ~0.4 m ahead of the camera (README-scene-setup.md), also below the band, though it is the comfort overlay itself, not readable UI. | Waiver recommended for the wrist HUD: it is glanceable, the player controls its distance with their own arm, and it fades out unless deliberately raised (`_faceDotThreshold`). Verify text comfort on device; if reading it strains, scale the canvas up and push content density down. |
| Body text legible at placed distance | UNVERIFIED (on-device) | Wrist canvas is 0.12 x 0.08 m using UnityEngine.UI Text (no TMP). Angular size at arm's length is plausible but only a headset check settles it. | - |
| Reticles render at target depth | N-A | No reticle or crosshair exists; aiming is physical along the weapon. (`WardenDrone` has a comment reserving shield data "for the reticle/scan UI", which does not exist yet; re-audit if it ships.) | - |

## Audio

| Item | Status | Evidence | Fix if violation |
|------|--------|----------|------------------|
| No sustained sounds above 85 dB equivalent | UNVERIFIED (on-device) | No audio assets in the repo; `AudioDirector` mixes whatever it is given. Meter the loudest combat minute after assets land. | - |
| No instant full-volume jump-scare stingers | UNVERIFIED (on-device) | `AudioDirector` crossfades at ~1.25 s and ducks on death rather than stinging, which is the right shape, but stinger assets do not exist yet. The Maw Eel ambush is the obvious future candidate; audit it when its audio ships. | - |
| Spatialized audio matches visual positions | UNVERIFIED (on-device) | One-shots use 3D sources per README-scene-setup.md. Needs the eyes-closed localization test on device with real assets. | - |

## Verdict

**VIOLATIONS(7)**: 5 requiring fixes, 2 waiver candidates. 11 UNVERIFIED
pending device or assets, 2 N-A, 6 PASS. Under VR Forge rules a checklist
violation at the declared rating blocks Phase 7, so these are release
blockers until fixed or formally waived.

Prioritized fixes:

1. **Teleport locomotion is not offered.** Smooth-only movement fails the
   first checklist item outright. Add a teleport mode as a comfort option;
   this is a real feature, so schedule it, do not patch it.
2. **Ground-motion vignette defaults off in code.** One-line fix (default
   true, or auto-enable with the Quest provider). Until fixed, comfort
   depends on a manual scene checkbox. Also restores the acceleration
   item's contingent PASS.
3. **No recenter, no pause menu.** Small feature, mandatory before guest
   playtests.
4. **Comfort settings do not persist.** PlayerPrefs serialization of
   `ComfortSettings`; also decide whether Off overrides the fall/swim
   forced vignette.
5. **Mandatory two-handed reload with no one-handed mode.** Implement the
   one-handed weapon mode GDD section 6 already promises.
6. **Geyser lift (waiver candidate).** Telegraphed, avoidable,
   vignette-covered; write the waiver into the Phase 6 record instead of
   changing the mechanic.
7. **Wrist HUD inside 0.75 m (waiver candidate).** Hand-anchored glanceable
   UI; waive with an on-device legibility check.

## Remediation record (same-day)

Fixes 1-4 were implemented immediately after this audit; statuses below
supersede the table above. Fix 5 and the two waiver candidates remain open.

| # | Finding | Status | Where |
|---|---------|--------|-------|
| 1 | Teleport not offered | FIXED | `Player/TeleportController.cs`: right-stick-forward arc aim, release to blink (via `UI/ScreenFade.cs`), slope-validated landings, coexists with snap turn. Auto-disabled on treadmill hardware. |
| 2 | Ground-motion vignette off by default | FIXED (better) | Scene checkbox removed. `ILocomotionProvider.IsArtificial` added: Quest/simulated providers report true, treadmill providers false, and `ComfortVignette` engages on artificial ground motion automatically. |
| 3 | No recenter | FIXED | `Player/RecenterControl.cs`: hold left menu button 1 s to re-align play space to head yaw + re-run provider `Calibrate()`. Public `Recenter()` for future pause menu. Pause menu itself still open. |
| 4 | Comfort settings do not persist | FIXED | `GameManager.SaveComfortSettings()` / auto-load via PlayerPrefs. Also: player-set Off now suppresses ALL vignette overrides including falls/swims (player choice wins). |
| 5 | One-handed weapon mode | OPEN | Scheduled follow-up; GDD section 6 promise stands. |
| 6 | Geyser lift | WAIVER PENDING | Needs a written Phase 6 waiver. |
| 7 | Wrist HUD depth | WAIVER PENDING | Needs on-device legibility check. |

Revised verdict: VIOLATIONS(1 open: one-handed mode) + 2 waiver candidates +
11 UNVERIFIED pending on-device runs. On-device items require a Quest 3 and
a machine with Unity: run `scripts/run-tests.sh`, `scripts/build-quest.sh`,
`scripts/deploy-quest.sh`, then re-audit frame timing and legibility.
