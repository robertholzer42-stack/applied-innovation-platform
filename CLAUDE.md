# PRIDEFALL - Project Instructions

PRIDEFALL is a VR action game built under the VR Forge harness conventions
(see the `vr-forge` repo for the full pipeline, roster, and phase gates).
The Unity 2022.3 project lives in `PridefallUnity/`, NOT at the repo root;
every script and tool accounts for that split.

**Meta Quest 3 is the primary build target** while Virtuix Omni One SDK
access is pending: Quest 3 builds are what we test, deploy, and gate on.
Omni One remains the design-intent platform (`docs/omni-one-integration.md`).

## Locomotion provider chain

`Pridefall.Input.ILocomotionProvider` abstracts ground movement. Providers
are tried in this order, first active wins:

1. `OmniOneLocomotionProvider` (treadmill, standalone headset build)
2. `OmniConnectLocomotionProvider` (treadmill via PC link)
3. `QuestControllerLocomotionProvider` (thumbstick, current default on Quest 3)
4. `SimulatedLocomotionProvider` (editor/CI)

The Omni files stay dormant behind the `OMNI_ONE_SDK` and `OMNI_CONNECT_SDK`
scripting defines. Do not add those defines or fill in the SDK stubs until
the SDKs actually land; without the defines the files must keep compiling.

## Quick reference

| I want to... | Do |
|---|---|
| Build the Quest APK | `scripts/build-quest.sh [Development|Release]` |
| Sideload + launch on headset | `scripts/deploy-quest.sh` |
| Run EditMode + PlayMode tests | `scripts/run-tests.sh` |
| See what is missing locally | `scripts/check-env.sh` |

App id defaults to `com.meridian.pridefall` (override with `VRFORGE_APP_ID`).
Build artifacts land in `PridefallUnity/Builds/` (apk, build.log, test logs).

## State markers and hooks

- `.claude/state/` at the REPO root is written only by `scripts/run-tests.sh`
  and `scripts/build-quest.sh`. Hooks in `.claude/hooks/` gate on it:
  `compile-failed` blocks `git push`; `dirty-since-last-green` nudges for a
  test run at end of turn.
- Never hand-delete `compile-failed` to get past a hook; fix the failure and
  let a green run clear it.
- Honest state: without Unity installed, `run-tests.sh` reports SKIPPED.
  A skip is never a green gate; say so when reporting status.

## C# style (match the existing codebase)

- One XML doc `<summary>` per class explaining intent, not mechanics.
- Namespaces follow folders: `Pridefall.Core`, `Pridefall.Player`, etc.
- No per-frame allocations; pooled instantiation through `ObjectPool` for
  projectiles, effects, and spawns.
- Cross-system communication goes through the `GameEvents` static bus;
  always unsubscribe in `OnDisable` (or test teardown).
- Serialized fields are `[SerializeField] private` with `_camelCase` names;
  public surface is properties with private setters.
- Single writer to the CharacterController; comfort settings live on
  `GameManager.Comfort` (`ComfortSettings`).
