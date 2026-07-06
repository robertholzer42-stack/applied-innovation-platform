#!/usr/bin/env bash
# Build the Quest APK via Unity batchmode.
# Usage: scripts/build-quest.sh [Development|Release]   (default: Development)
# On success: Builds/quest.apk exists, .claude/state markers are cleared.
# On failure: prints the offending log lines, records .claude/state/compile-failed,
# and exits nonzero so hooks and agents can gate on it.

set -euo pipefail

CONFIG="${1:-Development}"
case "$CONFIG" in
  Development|Release) ;;
  *) echo "error: unknown config '$CONFIG' (expected Development or Release)" >&2; exit 1 ;;
esac

PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_DIR"

if [ ! -d Assets ] || [ ! -d ProjectSettings ]; then
  echo "error: no Unity project at $PROJECT_DIR (Assets/ or ProjectSettings/ missing)." >&2
  echo "hint: Phase 3 copies templates/UnityStarter/ over the project root first." >&2
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

STATE_DIR=".claude/state"
LOG="Builds/build.log"
APK="Builds/quest.apk"
mkdir -p Builds "$STATE_DIR"

echo "Building $APK ($CONFIG) with $UNITY ..."
if ! "$UNITY" -batchmode -quit -projectPath . \
    -executeMethod "VRForge.Editor.BuildQuest.$CONFIG" \
    -buildTarget Android \
    -logFile "$LOG"; then
  echo "error: Unity build failed ($CONFIG). Relevant log lines from $LOG:" >&2
  grep -E 'error CS|Exception|Build Failed' "$LOG" | head -40 >&2 \
    || echo "(no matching lines; inspect $LOG directly)" >&2
  echo "quest $CONFIG build failed; see $LOG" > "$STATE_DIR/compile-failed"
  exit 1
fi

if [ ! -f "$APK" ]; then
  echo "error: Unity exited 0 but $APK was not produced. Tail of $LOG:" >&2
  tail -20 "$LOG" >&2 || true
  echo "quest $CONFIG build produced no APK; see $LOG" > "$STATE_DIR/compile-failed"
  exit 1
fi

rm -f "$STATE_DIR/compile-failed" "$STATE_DIR/dirty-since-last-green"
echo "Build OK: $APK ($(du -h "$APK" | cut -f1)) [$CONFIG]"
