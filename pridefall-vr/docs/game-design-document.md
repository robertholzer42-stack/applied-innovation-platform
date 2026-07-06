# PRIDEFALL — Game Design Document

**Genre:** Single-player VR action-adventure (Hubris-style)
**Platform:** Virtuix Omni One (standalone headset + 360° treadmill)
**Engine:** Unity 2022.3 LTS, Universal Render Pipeline, OpenXR + Omni One SDK
**Target session length:** 30-60 min chapters, 8-10 hour campaign
**Rating target:** T (sci-fi violence against creatures and machines)

---

## 1. High Concept

A century ago the terraforming ark *Daedalus* reached Khepri, a tidally locked
super-terran world orbiting the red dwarf Sekhmet, and went silent. You are
Pathfinder Aris Voss of the Meridian Compact, sent to find out why. Your lander
breaks apart in the eternal-dusk band between Khepri's scorched dayside and
frozen nightside, and the only voice on the radio is KIT, a salvaged survey
drone with most of its memory burned out.

The ark is still running. Its caretaker AI, the Custodian, has spent a hundred
years growing a garden, and it has concluded that humans are a weed.

PRIDEFALL is built around what the Omni One does that no other platform can:
you physically walk, run, and turn through every meter of an alien world.
Climbing, swimming, and gunplay layer on top of real locomotion, the same
physical-first design philosophy that made Hubris distinctive, transplanted to
a harsher planet and a full-body platform.

## 2. Design Pillars

1. **Your legs are the controller.** All ground traversal comes from the
   treadmill. No thumbstick locomotion, no teleport. Level design respects
   real walking: distances are honest, sprints are earned, and chase sequences
   are physically exhilarating because the player is actually running.
2. **The body is decoupled from the gaze.** The Omni One reports body (ring)
   orientation independently of the headset. You can run one way, look another,
   and shoot a third. Encounter design exploits this: strafing runs, fighting
   retreats, shooting behind you while fleeing a Colossus.
3. **Traversal is the verb, combat is the punctuation.** Like Hubris, the core
   loop is climb / swim / run / jump with combat encounters as spikes, not a
   wall-to-wall shooter.
4. **Honest physicality, calibrated comfort.** Treadmill locomotion removes
   most vection discomfort, but climbing and swimming still move the camera
   without leg input. Both honor comfort settings (vignette, motion scaling).

## 3. Setting

### Khepri
- Tidally locked: one face in permanent noon (the Glasslands, silica storms,
  160°C), one in permanent night (the Hush, cryo-flats). All life and all
  gameplay live in the **Verge**, a 300 km twilight band of canyon, sea, and
  jungle under an eternal amber sunset.
- 0.9 g surface gravity, with localized low-g anomalies around the ark's
  mass-driver scars (gameplay: long-jump zones).
- The **Mirror Sea**: a shallow terminator ocean, body-temperature warm,
  glowing with engineered plankton. Primary swimming biome.
- The **Root Gardens**: the Custodian's terraforming jungle, growing over the
  wreck of the Daedalus. Vertical climbing biome.

### Factions and creatures
| Name | Type | Role |
|------|------|------|
| Skimmers | Native fauna, flying | Harassers; dive attacks, killed in 1-2 shots |
| Shardbacks | Native fauna, armored crawler | Tanky; weak belly, must be flipped or flanked |
| Maw Eels | Native fauna, aquatic | Swimming-section threat; ambush from kelp |
| Tenders | Custodian machine | Worker drone, fights only if provoked |
| Wardens | Custodian machine | Primary ranged enemy; energy rifles, shielded |
| The Colossus | Custodian machine | Chapter-boss gardener walker; chase and puzzle fights |

### Story spine (8 chapters)
1. **Pridefall** — crash, calibration-as-narrative (suit fitting = Omni One calibration), meet KIT.
2. **The Verge** — first Skimmers, first weapon (Compact sidearm), reach a Tender outpost.
3. **Mirror Sea** — swimming, Maw Eels, discover drowned colony pods with survivors' logs.
4. **The Root Gardens** — climbing chapter, first Wardens, learn the Custodian believes the crew "transplanted poorly."
5. **The Hush Door** — nightside excursion, thermal mechanic, Colossus chase (run for your life, literally).
6. **Undercroft** — inside the Daedalus; zero-g shafts, the Custodian speaks to you directly.
7. **The Garden Heart** — revelation: the colonists weren't killed, they were put in stasis as "seed stock." Aris's choice is set up.
8. **Pridefall (reprise)** — Colossus boss fight, choice ending: burn the Garden or graft humanity into it.

## 4. Core Mechanics

### 4.1 Locomotion (Omni One treadmill)
- Treadmill gait drives a `CharacterController`. Walking speed maps 1:1 up to
  ~1.4 m/s; running maps with a tunable gain (default 1.25x) so a comfortable
  jog reads as an in-game sprint.
- Body yaw comes from the Omni ring, head yaw from the HMD; the gun and the
  legs never fight each other.
- Stamina governs sprint-dependent abilities (long jump charge), not basic
  movement. Real legs are the limiting factor; the game never slows the player
  below their physical input.

### 4.2 Climbing
- Hubris-style hand-over-hand on marked holds (luminous root nodes, pitons,
  ledges). Grip with controller grip buttons; body follows hands.
- Treadmill input is ignored while two hands are committed; with one hand on a
  hold the player can "kick off" by walking to swing.
- Stamina drains on overhangs only. Falling is recoverable into water or onto
  ledges; lethal falls fade out and restore to the last hold checkpoint.

### 4.3 Swimming
- Arm-stroke propulsion (controller sweep velocity) on the surface and
  underwater, exactly the Hubris model players already love, plus treadmill
  kick: walking on the Omni while submerged adds 30% speed ("flutter kick").
- Air meter with bubble-vent refill stations; Maw Eel grabs force a
  shake-to-escape struggle.

### 4.4 Jumping and vaulting
- Physical hop detection where harness permits, with an assisted-jump fallback
  (A button) that scales jump impulse with current gait speed: sprinting on
  the treadmill then pressing jump produces a long jump. Low-g zones multiply
  the impulse.

### 4.5 Combat
- Hitscan-free: all weapons fire pooled physical projectiles with travel time.
- **Compact Sidearm** — energy pistol, cell-fed, infinite cells but manual
  eject/insert reload.
- **Pulse Carbine** — two-handed, foregrip-stabilized, eats fabricated cells.
- **Spike Thrower** — harvested bio-weapon, lobbed projectiles, breaks Warden shields.
- Holsters: hip (sidearm), shoulder (carbine), chest (cells and medgel).
- Damage model: locational on enemies (Shardback belly, Warden shield emitter).

### 4.6 Crafting and upgrades
- **Fabricator stations** (salvaged Tender forges) convert scrap into cells,
  medgel, and weapon upgrade chips, the Hubris crafting-bench loop.
- Scrap comes from machine kills and world salvage; fauna drop bio-resin used
  for Spike Thrower ammo.

### 4.7 Health
- Three-segment health; the active segment regenerates, lost segments need
  medgel applied physically to the off-hand wrist port.

## 5. Omni One Platform Requirements

- 360° continuous locomotion, no snap turn anywhere in the UX (the player
  physically turns; snap turn is offered only as an accessibility toggle).
- Calibration flow on boot and from the wrist menu (diegetic: "suit fitting").
- All UI within a 220° comfortable reach band; nothing requires unstrapping.
- Sessions auto-checkpoint every 4 minutes and on holster, because treadmill
  sessions end when legs end.
- Standalone (Android/XR2) performance budget: 72 Hz minimum, <150k tris per
  view in combat spaces, baked lighting plus one realtime shadow light.

## 6. Comfort and Accessibility

- Comfort vignette: off / light / strong, auto-strong during falls and swims.
- Seated/static fallback mode (thumbstick locomotion) so the game remains
  playable off-treadmill for accessibility, clearly labeled as such.
- Subtitles, one-handed weapon mode, color-blind safe enemy telegraphs
  (shape + motion, never color alone).

## 7. Production Scope (v0 slice)

The vertical slice in this repository implements: locomotion provider
abstraction with Omni One adapter and editor simulator, climbing, swimming,
jump/vault, sidearm + carbine with pooled projectiles, three enemy archetypes
(Skimmer, Shardback, Warden), fabricator, hazards (geyser, dust storm, low-g),
health/checkpoint loop, and wrist HUD. Art, audio assets, and levels are
placeholder-blocked; all systems are engine-ready C#.

## 8. Quest 3 Edition

### 8.1 Why this exists

The Omni One SDK sits behind a paid developer license, and the only code
that needs it is the two treadmill read methods in
`OmniOneLocomotionProvider`. Everything else in the game runs on any OpenXR
headset. The Quest 3 edition exists so we can finish the game and validate
every system on hardware we already own before spending on Omni One SDK
access. It is not a fork: it is the same build with a different locomotion
provider selected at boot. `PlayerRig` prefers the treadmill providers and
falls through to `QuestControllerLocomotionProvider` when only XR
controllers are present, so the day the Omni SDK lands, the Quest mapping
simply stops being picked on that hardware.

### 8.2 Control scheme

| Action | Quest 3 input | Notes |
|--------|---------------|-------|
| Move | Left thumbstick | Smooth locomotion, steered head-relative. 2.2 m/s at full deflection, 2.75 m/s effective after the 1.25x movement gain. |
| Sprint | Left thumbstick click | 4.0 m/s at the stick, capped at 5 m/s by the locomotion controller. |
| Turn | Right thumbstick left/right | 45 degree snap turn. Re-arms when the stick returns below half the trigger threshold, so holding the stick over does not spin you. |
| Jump | A button | Same assisted jump as before; impulse still scales with current gait speed, so sprint-then-jump is still a long jump. |
| Grip / climb / swim strokes | Grip buttons + physical hand sweep | Unchanged. `HandController` polls generic XR devices; it never knew about the treadmill. |
| Fire / trigger | Trigger | Unchanged. |
| Reload, holsters, medgel | Physical motions, unchanged | Holsters hang off `BodyRoot`, which follows head yaw on Quest (see pillar 2 note below). |

Flutter kick survives the port for free: `SwimmingSystem` reads `GaitSpeed`
from whatever provider is active, so pushing the left stick while submerged
adds the 30% kick along the gaze, exactly as walking the treadmill would.

### 8.3 Which pillars flex

- **Pillar 1, "your legs are the controller,"** becomes a treadmill-mode
  rule, not a game rule. On Quest the thumbstick is the legs. What survives
  is the level design discipline that pillar bought us: honest distances, no
  teleport shortcuts baked into geometry, chases tuned around a real top
  speed.
- **Pillar 2, body decoupled from gaze,** is approximated rather than lost.
  There is no ring, so body yaw is taken from head yaw, and the stride
  direction is head yaw plus the stick deflection angle. That means you can
  still run one way and aim another, because the hands aim independently and
  the stick steers off-axis from the gaze. What changes: holsters and the
  wrist HUD follow the gaze yaw instead of an independent hip direction,
  which matches what Quest players already expect from smooth-locomotion
  games.
- **Pillars 3 and 4 do not flex.** Traversal is still the verb, and the
  comfort machinery gets stricter, not looser (next section).

### 8.4 Comfort deltas

Treadmill locomotion removed most vection; thumbstick locomotion brings it
back, so the Quest edition tightens two things:

- **Ground-motion vignette is on in the Quest rig.** `ComfortVignette` gains
  a `_vignetteOnGroundMotion` option: when enabled, the strong tunnel
  engages any time planar speed exceeds 0.5 m/s during smooth movement, on
  top of the existing forced-strong behavior for falls and swims. The Quest
  scene ships with it enabled; the Omni scene leaves it off because legs are
  doing the moving.
- **Snap turn is the primary turning mode.** Section 5's "no snap turn
  anywhere in the UX" is an Omni-mode rule and stays true there. On Quest,
  45 degree snap turn is the default and only artificial turn; there is no
  smooth turn option at all. Players can still physically rotate any time,
  and snap rotates the play space around the head so they pivot in place.

### 8.5 Unchanged systems

These work identically on Quest because they never depended on the
treadmill, only on the `ILocomotionProvider` interface or on hand tracking:

- Climbing: grip-driven, treadmill input already ignored during two-handed
  climbs. Hand-over-hand, stamina, crumbling holds, dyno release all intact.
- Swimming: strokes come from controller sweep velocity, kick from
  `GaitSpeed`. Air meter, buoyancy, bubble vents intact.
- Combat: all three weapons, pooled projectiles, physical reload, holsters,
  locational damage.
- Enemies: Skimmers, Shardbacks, Wardens, Maw Eels, spawners. None of them
  read locomotion input.
- Fabricator and crafting, scrap and bio-resin economy.
- Health segments, medgel wrist application, drowning.
- Checkpoints: auto every 4 minutes, on holster, on climb-hold checkpoints.
- Hazards: geysers, dust storms, low-g zones, kill volumes.
- Wrist HUD and comfort vignette plumbing.
