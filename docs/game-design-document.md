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
