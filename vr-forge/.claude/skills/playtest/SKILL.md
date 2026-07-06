---
name: playtest
description: Run the test suite and (when the Unity MCP is live) Play Mode tests plus a scripted editor smoke path, then set the .claude/state markers the hooks read. Invoke when the user says /playtest, asks to run tests, or after a gameplay system lands (pipeline Phase 5 cadence). Argument: focus area, e.g. "locomotion" or "combat" (optional; default is the full suite).
---

# /playtest — Phase 5 test cadence

Owner agent: `playtester`. Run the script yourself; delegate editor-driving and failure triage to `playtester`. The state markers in `.claude/state/` are the contract with `push-gate.sh` and `build-gate.sh` — maintaining them exactly is half this skill's job.

## 1. Run the suite

```
./scripts/run-tests.sh
```

The script records green/red in `.claude/state/` per the hook protocol. Capture its full output. If a focus area was given, still run the full suite (the hooks only trust a full run) but lead the summary with the focused results.

## 2. If the Unity MCP is live, go deeper

Check whether the `unity` MCP server responds. If yes, delegate to the `playtester` agent:

1. "Run the Play Mode test suite via the Unity MCP test runner; return per-test pass/fail and the console output of each failure."
2. "Drive a scripted smoke path in the editor: enter Play Mode, exercise the core loop (spawn player rig, perform each GDD core verb once, load each level in the GDD level list<, focusing on FOCUS if given>), watch the console for errors/exceptions, exit Play Mode. Report every console error with the action that triggered it."

If the Unity MCP is not connected, say so and rely on step 1 alone — do not pretend Play Mode ran.

## 3. Update the state markers — exactly as the hooks expect

- **All green** (script suite green, and Play Mode green if it ran):
  ```
  rm -f .claude/state/dirty-since-last-green .claude/state/compile-failed
  ```
- **Any red**: write the reason (first failing test or compile error, one line) into the compile marker so `push-gate.sh` can quote it:
  ```
  mkdir -p .claude/state && echo "<one-line reason>" > .claude/state/compile-failed
  ```
  Leave `dirty-since-last-green` alone on red — it clears only on green.
- Never edit the markers outside these two rules, and never clear `compile-failed` without a passing run behind it.

## 4. Summarize

```
Playtest — <focus or "full suite">
EditMode/script suite: <n passed / m failed>
Play Mode (MCP):       <n passed / m failed | not run: MCP offline>
Smoke path:            <clean | errors: n>
State:                 <green — markers cleared | red — compile-failed set: reason>
```

For every failure give: test/action name, expected vs actual, minimal repro steps (scene, object, input), and the exact console/log block. Offer the top fix candidate but do not auto-fix; fixing belongs to `gameplay-programmer`, then re-run `/playtest`.
