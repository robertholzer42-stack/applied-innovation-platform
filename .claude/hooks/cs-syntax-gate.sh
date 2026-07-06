#!/usr/bin/env bash
# PostToolUse hook (Write|Edit): cheap syntax gate for C# files.
# Reads the hook payload from stdin, and if the edited file is a .cs file,
# runs a fast brace/paren balance check plus csharpier when available.
# Exit 0 = pass, exit 2 = block with feedback to Claude.

set -u
payload="$(cat)"

file_path="$(printf '%s' "$payload" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -1)"
[ -z "$file_path" ] && exit 0
case "$file_path" in
  *.cs) ;;
  *) exit 0 ;;
esac
[ -f "$file_path" ] || exit 0

# Prefer a real formatter/parser when installed.
if command -v csharpier >/dev/null 2>&1; then
  if ! csharpier check "$file_path" >/dev/null 2>&1; then
    echo "csharpier reports a parse/format problem in $file_path. Fix the syntax." >&2
    exit 2
  fi
  exit 0
fi

# Fallback: brace balance. Catches the most common truncated-file failure.
opens=$(tr -cd '{' < "$file_path" | wc -c)
closes=$(tr -cd '}' < "$file_path" | wc -c)
if [ "$opens" -ne "$closes" ]; then
  echo "Unbalanced braces in $file_path ({: $opens, }: $closes). The file is likely truncated or malformed." >&2
  exit 2
fi
exit 0
