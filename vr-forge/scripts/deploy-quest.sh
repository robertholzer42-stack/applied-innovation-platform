#!/usr/bin/env bash
# Install Builds/quest.apk on the single connected Quest and launch it.
# Usage: scripts/deploy-quest.sh
# Requires: adb on PATH, exactly one device in 'adb devices', a prior build.

set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_DIR"

APK="Builds/quest.apk"
APP_ID="${VRFORGE_APP_ID:-com.vrforge.game}"

if ! command -v adb >/dev/null 2>&1; then
  echo "error: adb not found on PATH. Install android-platform-tools or Unity's Android module." >&2
  exit 1
fi

if [ ! -f "$APK" ]; then
  echo "error: $APK not found. Run scripts/build-quest.sh first." >&2
  exit 1
fi

# Exactly one authorized device; anything else is ambiguous or not ready.
DEVICES="$(adb devices | awk 'NR>1 && $2=="device" {print $1}')"
COUNT="$(printf '%s' "$DEVICES" | grep -c . || true)"
if [ "$COUNT" -ne 1 ]; then
  echo "error: expected exactly 1 ready adb device, found $COUNT. Full device list:" >&2
  adb devices >&2
  echo "hint: 'unauthorized' means accept the USB debugging prompt inside the headset." >&2
  exit 1
fi
SERIAL="$DEVICES"

echo "Installing $APK on $SERIAL ..."
adb -s "$SERIAL" install -r "$APK"

echo "Launching $APP_ID ..."
adb -s "$SERIAL" shell monkey -p "$APP_ID" 1 >/dev/null

echo "deployed + launched ($APP_ID on $SERIAL)"
