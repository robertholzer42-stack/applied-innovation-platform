#!/usr/bin/env bash
# PreToolUse hook (Bash): gate `git push` on project health.
# The playtest/build agents record state in .claude/state/ ; a recorded
# compile failure blocks pushes until it is cleared by a passing run.
# Exit 0 = allow, exit 2 = block with reason.

set -u
payload="$(cat)"

# Only inspect git push commands; allow everything else instantly.
printf '%s' "$payload" | grep -q '"command"[[:space:]]*:[[:space:]]*"[^"]*git push' || exit 0

state_dir="${CLAUDE_PROJECT_DIR:-.}/.claude/state"
if [ -f "$state_dir/compile-failed" ]; then
  echo "Push blocked: the last Unity compile/test run failed ($(cat "$state_dir/compile-failed")). Run ./scripts/run-tests.sh and fix the errors, or delete .claude/state/compile-failed if this is stale." >&2
  exit 2
fi
exit 0
