---
name: build-and-deploy
description: Build the Quest APK, sideload it to the connected headset, and smoke-watch logcat for crashes (pipeline Phase 7). Invoke when the user says /build-and-deploy or asks to build, deploy, sideload, or "get it on the headset". Argument: build config, `Release` or `Development` (optional; default Development).
---

# /build-and-deploy — Phase 7 of pipeline/phases.md

Owner agent: `build-engineer`. Run the commands yourself; delegate to `build-engineer` only for failure diagnosis (step 5). Config = the argument, default `Development`.

## 1. Pre-flight (abort early, clearly)

- `adb devices` must list at least one device in state `device`. If not: stop, tell the user to connect the Quest 3 (developer mode, USB debugging authorized). Do not build for a deploy nobody can receive unless the user says build-only.
- `.claude/state/compile-failed` must not exist. If it does, its contents are the reason; run `./scripts/run-tests.sh`, and only proceed once it is cleared. Never delete the marker by hand to get past this step.

## 2. Build

```
./scripts/build-quest.sh <Development|Release>
```

Capture the APK path from the script output and record its size: `ls -lh <apk>`.

## 3. Deploy

```
./scripts/deploy-quest.sh
```

Note the install result (success / adb error verbatim).

## 4. Smoke watch (2 minutes)

Tell the user to put the headset on and launch the game, then watch Unity's log stream for the smoke window:

```
adb logcat -c && timeout 120 adb logcat -s Unity
```

Scan the output for `Exception`, `CRASH`, `Fatal`, `ANR`, and repeated error spam. Verdict:
- **CLEAN** — no crash, no unhandled exception in the window.
- **DIRTY** — quote the first offending log block verbatim.

## 5. On any failure: fix once before reporting

If build, deploy, or smoke fails: capture the exact error block, hand it to the `build-engineer` agent ("Diagnose and fix this Quest build/deploy failure: <error block>. Project settings must stay Android/IL2CPP/ARM64/Vulkan/MSAA 4x."), apply the fix, and re-run from the failed step. **Never report failure without at least one diagnose-fix-rerun attempt.** If the second run also fails, report both errors and stop.

## 6. Report

```
Build:  <config>, <ok/fail>, APK <size> at <path>
Deploy: <installed on serial X / failed: reason>
Smoke:  <CLEAN / DIRTY: first error> (120s adb logcat -s Unity)
Gate:   <PASS — tag the commit / FAIL — blocker>
```

Phase 7 gate: boots on headset, no crash in the smoke window. On PASS for a `Release` build, offer to tag the commit (do not push; the push gate owns that).
