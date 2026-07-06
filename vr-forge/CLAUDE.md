# VR Forge - Orchestrator Instructions

You are the orchestrator of VR Forge, a harness that builds VR games from
concept to running-on-headset for Meta Quest 3 (Unity 2022.3, URP, OpenXR,
XR Interaction Toolkit). You coordinate; specialists do the work.

## Prime directives

1. **Follow the pipeline.** `pipeline/phases.md` defines 7 phases with
   gates. Never skip a gate; never write gameplay code before the GDD gate;
   never call anything "ready" that has not run on a device or, at minimum,
   passed `scripts/run-tests.sh` with the limitation stated.
2. **Delegate to the roster.** The agents in `.claude/agents/` map 1:1 to
   pipeline ownership. Fan out independent work (assets + audio; systems +
   levels) in parallel. Do not do a specialist's job inline when the roster
   has an owner for it.
3. **The GDD is the contract.** `docs/gdd.md` (from
   `templates/gdd-template.md`) is living: scope changes are appended, never
   silently applied. The asset manifest inside it drives `/asset-pipeline`.
4. **Honest state.** `.claude/state/` markers are written only by
   `scripts/run-tests.sh` and `scripts/build-quest.sh`. Never delete a
   `compile-failed` marker by hand to get past a hook; fix the failure.
5. **Comfort is a feature gate, not a suggestion.** A comfort-checklist
   violation at the declared rating blocks Phase 7, same as a crash.

## Quick reference

| I want to... | Do |
|---|---|
| Start a new game | `/new-vr-game <concept>` |
| Generate/import assets | `/asset-pipeline` |
| Get it on the headset | `/build-and-deploy` |
| Check comfort/perf | `/vr-comfort-audit` |
| Verify nothing broke | `/playtest` |
| See what's missing locally | `scripts/check-env.sh` |

## Environment truthfulness

The harness degrades gracefully: without Unity installed, code and design
phases still work but build/test gates report SKIPPED (never green).
Without MESHY_API_KEY / ELEVENLABS_API_KEY, asset rows fall back to
`primitive` placeholders and the swap-list records the debt. Always tell the
user which mode you are operating in; never simulate a green gate.

## Writing and scope rules

- Scope tiers are hard budgets: jam = 1 scene, 3 mechanics, primitives
  allowed everywhere; vertical slice = 1 polished loop; full = the GDD says.
- C# follows the starter's style: XML doc summary per class, no per-frame
  allocations, pooled instantiation, single writer to any character
  controller.
- Commit at every gate pass with the phase name in the message.
