# VR Comfort Checklist (Quest 3)

Audited in Phase 6 by the comfort reviewer. Every item must be checked at the
game's declared comfort rating, or the violation is listed with a waiver
reason in the audit output. Pass criterion follows each item.

## Locomotion
- [ ] Smooth locomotion AND teleport are both offered. Pass: either can finish the whole game.
- [ ] Snap turn is the default turning mode, ON at first boot. Pass: fresh install turns in 45 deg steps.
- [ ] Smooth turn exists only as an opt-in setting. Pass: it is off until the player enables it.
- [ ] Vignette engages during smooth locomotion (ComfortController default: Light, auto-Strong while moving). Pass: visible aperture close within 0.3 s of stick input.
- [ ] Vignette strength is player-adjustable including Off. Pass: setting persists across sessions.
- [ ] Smooth movement speed is 3 m/s or less by default. Pass: measured over 10 m in-game.
- [ ] No forced smooth locomotion in the critical path (moving platforms, rails) without vignette or teleport alternative. Pass: audit of every scripted movement.
- [ ] Acceleration is instant or vignetted; no easing curves on player velocity. Pass: velocity trace shows step change or vignette active.

## Camera
- [ ] The camera never translates or rotates without matching player input. Pass: zero scripted camera moves in the whole build.
- [ ] No cutscene camera grabs; cutscenes play in-world or fade-cut. Pass: audit of every cutscene trigger.
- [ ] Horizon lock option exists for any vehicle or platform motion. Pass: toggle present and functional.
- [ ] No head-bob, screen shake, or FOV animation (the comfort vignette is the only aperture change). Pass: grep for camera shake calls returns none.
- [ ] Recenter (view reset) is reachable from the pause menu at all times. Pass: works while moving and in menus.

## Interaction
- [ ] Two-handed actions are optional, never required to progress. Pass: single-hand completion of every mandatory interaction.
- [ ] Seated mode: every mandatory interactable is reachable seated, or offers a pull/summon. Pass: full playthrough from a chair.
- [ ] No mandatory sustained overhead reaches longer than 3 s. Pass: audit of every hold interaction.
- [ ] Holsters and belt sockets sit within a natural arm sweep (HolsterSocket placement on the rig, not world-fixed). Pass: reachable without leaning.
- [ ] World-space UI panels can be repositioned or re-summoned. Pass: panel follows recenter.

## Display
- [ ] 72 Hz sustained in the heaviest scene (see perf-budget.md). Pass: no dropped-frame spikes over a 60 s capture.
- [ ] No full-view flashes or flicker patterns above 3 Hz. Pass: audit of every VFX and damage feedback.
- [ ] All UI sits in the 0.75-10 m depth band. Pass: measured distance of every canvas.
- [ ] Body text is legible at its placed distance (about 1 deg of angular size or larger). Pass: readable without leaning in.
- [ ] Reticles and crosshairs render at target depth, not fixed screen depth. Pass: no double-vision on aim.

## Audio
- [ ] No sustained sounds above an 85 dB equivalent at default volume. Pass: loudness meter over the loudest combat minute.
- [ ] No instant full-volume jump-scare stingers without a comfort setting to soften them. Pass: audit of every stinger trigger.
- [ ] Spatialized audio matches visual positions (no hard-panned mono cheats). Pass: eyes-closed localization test on 5 sources.
