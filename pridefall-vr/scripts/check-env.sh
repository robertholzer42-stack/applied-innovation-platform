#!/usr/bin/env bash
# SessionStart hook + manual env doctor. Prints one line per dependency.
# Never fails the session; it reports so agents plan around what exists.

set -u

check() { # name, command, hint
  if command -v "$2" >/dev/null 2>&1; then
    echo "[ok]      $1 ($(command -v "$2"))"
  else
    echo "[missing] $1 - $3"
  fi
}

echo "=== PRIDEFALL (VR Forge) environment ==="

# Unity: look in default Hub locations, then UNITY_PATH.
unity_bin="${UNITY_PATH:-}"
if [ -z "$unity_bin" ]; then
  for candidate in \
    /Applications/Unity/Hub/Editor/2022.3*/Unity.app/Contents/MacOS/Unity \
    "$HOME"/Unity/Hub/Editor/2022.3*/Editor/Unity \
    /opt/unity/editors/2022.3*/Editor/Unity; do
    [ -x "$candidate" ] && unity_bin="$candidate" && break
  done
fi
if [ -n "$unity_bin" ] && [ -x "$unity_bin" ]; then
  echo "[ok]      Unity 2022.3 ($unity_bin)"
else
  echo "[missing] Unity 2022.3 - install via Unity Hub with Android Build Support, or set UNITY_PATH"
fi

check "adb (Quest deploy)" adb "install android-platform-tools or Unity Android module"
check "uv (MCP servers)" uv "pip install uv"
check "git" git "install git"
check "blender (blender-mcp)" blender "optional: install Blender 4.x + the blender-mcp addon"

[ -n "${MESHY_API_KEY:-}" ]      && echo "[ok]      MESHY_API_KEY set"      || echo "[missing] MESHY_API_KEY - 3D asset generation disabled"
[ -n "${ELEVENLABS_API_KEY:-}" ] && echo "[ok]      ELEVENLABS_API_KEY set" || echo "[missing] ELEVENLABS_API_KEY - audio generation disabled"

if command -v adb >/dev/null 2>&1; then
  headsets=$(adb devices 2>/dev/null | awk 'NR>1 && $2=="device"' | wc -l | tr -d ' ')
  echo "[info]    headsets connected via adb: $headsets"
fi
echo "==========================="
exit 0
