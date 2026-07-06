#!/usr/bin/env bash
# Build the Quest APK via Unity batchmode.
# Usage: scripts/build-quest.sh [Development|Release]   (default: Development)
# Layout: this script lives in <repo>/scripts/; the Unity project lives in
# <repo>/PridefallUnity/. State markers live at the REPO root (.claude/state/),
# where the hooks expect them.
# On success: PridefallUnity/Builds/quest.apk exists, state markers are cleared.
# On failure: prints the offending log lines, records .claude/state/compile-failed,
# and exits nonzero so hooks and agents can gate on it.

set -euo pipefail

CONFIG="${1:-Development}"
case "$CONFIG" in
  Development|Release) ;;
  *) echo "error: unknown config '$CONFIG' (expected Development or Release)" >&2; exit 1 ;;
esac

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
  echo "error: Unity 2022.3 not found. Install it via Unity Hub with Android Build Support, or set UNITY_PATH." >&2
  exit 1
fi

STATE_DIR="$REPO_DIR/.claude/state"
LOG="$UNITY_PROJECT/Builds/build.log"
APK="$UNITY_PROJECT/Builds/quest.apk"
mkdir -p "$UNITY_PROJECT/Builds" "$STATE_DIR"

# The BuildQuest editor script reads VRFORGE_APP_ID; default to the PRIDEFALL id.
export VRFORGE_APP_ID="${VRFORGE_APP_ID:-com.meridian.pridefall}"

echo "Building PridefallUnity/Builds/quest.apk ($CONFIG, appId=$VRFORGE_APP_ID) with $UNITY ..."
if ! "$UNITY" -batchmode -quit -projectPath "$UNITY_PROJECT" \
    -executeMethod "Pridefall.Editor.BuildQuest.$CONFIG" \
    -buildTarget Android \
    -logFile "$LOG"; then
  echo "error: Unity build failed ($CONFIG). Relevant log lines from PridefallUnity/Builds/build.log:" >&2
  grep -E 'error CS|Exception|Build Failed' "$LOG" | head -40 >&2 \
    || echo "(no matching lines; inspect $LOG directly)" >&2
  echo "quest $CONFIG build failed; see PridefallUnity/Builds/build.log" > "$STATE_DIR/compile-failed"
  exit 1
fi

if [ ! -f "$APK" ]; then
  echo "error: Unity exited 0 but $APK was not produced. Tail of the log:" >&2
  tail -20 "$LOG" >&2 || true
  echo "quest $CONFIG build produced no APK; see PridefallUnity/Builds/build.log" > "$STATE_DIR/compile-failed"
  exit 1
fi

rm -f "$STATE_DIR/compile-failed" "$STATE_DIR/dirty-since-last-green"
echo "Build OK: PridefallUnity/Builds/quest.apk ($(du -h "$APK" | cut -f1)) [$CONFIG]"
