---
name: vr-comfort-audit
description: Audit the project against the VR comfort checklist and perf budget (pipeline Phase 6). Invoke when the user says /vr-comfort-audit or asks about comfort, motion sickness, vection, or frame-rate compliance. Takes no arguments.
---

# /vr-comfort-audit — Phase 6 of pipeline/phases.md

Delegate the audit to the `vr-comfort-reviewer` agent; you compile its findings into the verdict. Ground rule: **never soften a violation.** No "minor", no "probably fine", no averaging a violation away. A violation is reported at full strength or proven absent.

## 1. Delegate the checklist walk

Launch `vr-comfort-reviewer` with this contract: "Walk `templates/comfort-checklist.md` item by item. For every item, inspect the actual project — scene files (`Assets/**/*.unity`), locomotion and rig settings (XR Interaction Toolkit provider components, snap turn / vignette configuration), and any camera-moving C# in `Assets/`. Judge each item PASS / VIOLATION / NOT-VERIFIABLE with the concrete evidence (file, scene object, or setting) you checked. Evidence from files only — never assume a default is in place without seeing it. The declared comfort rating is in the comfort section of `docs/gdd.md`; audit at that rating."

## 2. Report every violation in fixed format

One line per violation, exactly:

```
[<checklist item>] [<severity: BLOCKER|MAJOR|MINOR>] [<file or scene:object>] [<concrete fix>]
```

Severity is about player harm (BLOCKER = likely sickness at the declared rating), not effort to fix. NOT-VERIFIABLE items are listed separately with what would be needed to verify — they are not passes.

## 3. Performance audit

1. Check `ProjectSettings/` against `templates/perf-budget.md`: target frame rate / Vulkan / IL2CPP / ARM64 / MSAA 4x, quality settings, texture compression. Read the files directly (`ProjectSettings/ProjectSettings.asset`, `QualitySettings.asset`); report mismatches in the same violation format.
2. If `adb devices` shows a connected headset and the game is installed, sample live frame timing while the user plays the heaviest scene:
   ```
   adb logcat -c && timeout 60 adb logcat -s Unity VrApi
   ```
   Extract FPS / frame-time lines. The Phase 6 gate demands 72 Hz sustained; any sustained dip below 72 is a MAJOR violation with the scene named.
3. No headset connected: state "on-device frame timing NOT SAMPLED (no headset)" — do not extrapolate from editor stats.

## 4. Verdict

```
Comfort audit — <game>, declared rating: <rating>
Checklist: <n> items — <p> pass, <v> violations, <u> not verifiable
Perf:      <settings ok/mismatched>, on-device: <72Hz held / dipped to X / not sampled>
Verdict:   PASS | VIOLATIONS (<v>)
```

PASS requires zero violations *and* zero unverifiable items on BLOCKER-class checks. Anything else is VIOLATIONS with the count. The Phase 6 gate stays closed until PASS.
