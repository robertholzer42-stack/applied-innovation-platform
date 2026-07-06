#!/usr/bin/env bash
# Run the Unity Test Framework suites in batchmode: EditMode, then PlayMode.
# Usage: scripts/run-tests.sh
# Layout: this script lives in <repo>/scripts/; the Unity project lives in
# <repo>/PridefallUnity/. State markers live at the REPO root (.claude/state/).
# State protocol (consumed by .claude/hooks/):
#   any failure  -> writes .claude/state/compile-failed with a one-line reason, exits 1
#   all green    -> removes compile-failed and dirty-since-last-green, exits 0
#   Unity absent -> prints a skip notice and exits 0 (graceful degradation)

set -euo pipefail

REPO_DIR="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_PROJECT="$REPO_DIR/PridefallUnity"
cd "$REPO_DIR"

if [ ! -d "$UNITY_PROJECT/Assets" ] || [ ! -d "$UNITY_PROJECT/ProjectSettings" ]; then
  echo "error: no Unity project at $UNITY_PROJECT (Assets/ or ProjectSettings/ missing)." >&2
  exit 1
fi

# Find Unity: UNITY_PATH first, then the same Hub globs as scripts/check-env.sh.
find_unity() {
  if [ -n "${UNITY_PATH:-}" ] && [ -x "${UNITY_PATH}" ]; then
    echo "$UNITY_PATH"
    return 0
  fi
  for candidate in \
    /Applications/Unity/Hub/Editor/2022.3*/Unity.app/Contents/MacOS/Unity \
    "$HOME"/Unity/Hub/Editor/2022.3*/Editor/Unity \
    /opt/unity/editors/2022.3*/Editor/Unity; do
    if [ -x "$candidate" ]; then
      echo "$candidate"
      return 0
    fi
  done
  return 1
}

if ! UNITY="$(find_unity)"; then
  echo "skip: Unity 2022.3 is not installed on this machine; tests not run."
  echo "      Install via Unity Hub (with Android Build Support) or set UNITY_PATH to enable them."
  exit 0
fi

STATE_DIR="$REPO_DIR/.claude/state"
mkdir -p "$UNITY_PROJECT/Builds" "$STATE_DIR"

FAIL_REASON=""

for PLATFORM in EditMode PlayMode; do
  RESULTS="$UNITY_PROJECT/Builds/test-results-${PLATFORM}.xml"
  LOG="$UNITY_PROJECT/Builds/test-${PLATFORM}.log"
  rm -f "$RESULTS"

  echo "Running $PLATFORM tests ..."
  # Note: -runTests must not be combined with -quit; Unity exits on its own.
  set +e
  "$UNITY" -batchmode -projectPath "$UNITY_PROJECT" \
    -runTests -testPlatform "$PLATFORM" \
    -testResults "$RESULTS" \
    -logFile "$LOG"
  UNITY_EXIT=$?
  set -e

  if [ ! -f "$RESULTS" ]; then
    echo "error: $PLATFORM run produced no results file (Unity exit $UNITY_EXIT); likely a compile failure. Log lines:" >&2
    grep -E 'error CS|Exception' "$LOG" | head -20 >&2 \
      || echo "(no matching lines; inspect $LOG directly)" >&2
    FAIL_REASON="$PLATFORM tests did not run (Unity exit $UNITY_EXIT); see PridefallUnity/Builds/test-${PLATFORM}.log"
    break
  fi

  FAILED_COUNT="$(grep -c 'result="Failed"' "$RESULTS" || true)"
  if [ "$FAILED_COUNT" -gt 0 ]; then
    echo "error: $PLATFORM tests failed. Failing cases:" >&2
    grep -o '<test-case[^>]*result="Failed"' "$RESULTS" \
      | grep -o 'fullname="[^"]*"' | head -10 >&2 || true
    FAIL_REASON="$PLATFORM tests failed; see PridefallUnity/Builds/test-results-${PLATFORM}.xml"
    break
  fi

  echo "$PLATFORM: green"
done

if [ -n "$FAIL_REASON" ]; then
  echo "$FAIL_REASON" > "$STATE_DIR/compile-failed"
  exit 1
fi

rm -f "$STATE_DIR/compile-failed" "$STATE_DIR/dirty-since-last-green"
echo "All tests green (EditMode + PlayMode)."
