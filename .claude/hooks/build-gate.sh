#!/usr/bin/env bash
# Stop hook: soft gate at end of turn.
# Never hard-blocks routine conversation; it only blocks stopping when the
# harness knows gameplay code changed after the last successful build/test
# run, so a "done" claim is backed by a green run.

set -u
state_dir="${CLAUDE_PROJECT_DIR:-.}/.claude/state"
marker="$state_dir/dirty-since-last-green"

[ -f "$marker" ] || exit 0

# One nudge per dirty period: convert the marker to 'nudged' so we do not loop.
if [ "$(cat "$marker" 2>/dev/null)" = "nudged" ]; then
  exit 0
fi
echo "nudged" > "$marker"
echo "Gameplay code changed since the last green run. Run ./scripts/run-tests.sh (records pass/fail in .claude/state/) before finishing, or state explicitly that the change is untested." >&2
exit 2
