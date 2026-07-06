# Game Design Document: [TITLE]

> Living document (see pipeline/phases.md). Any phase that changes scope
> appends here; nothing silently diverges. Fill every section before Phase 3.

## High Concept
One paragraph: the fantasy, the player, the session. "You are X doing Y in Z,
and it feels like W."

## Why VR
What this game does that a flat screen cannot. If the answer is "immersion",
be specific: physical aiming, body-scale spaces, two-handed manipulation,
spatial audio hunting. No credible answer = stop here.

## Core Verbs
3-5 verbs the player performs constantly (e.g. grab, throw, climb, deflect).
Every mechanic below must serve at least one verb.

## Comfort Rating + Options Matrix
Target rating: Comfortable / Moderate / Intense (Meta store scale).

| Option | Default | Alternatives |
|--------|---------|--------------|
| Turning | Snap 45 deg | Smooth (opt-in), 30/60 deg |
| Locomotion | Teleport + smooth | Either alone |
| Vignette | Light, auto-Strong when moving | Off / Strong |
| Play position | Standing | Seated (all reach verified) |

## Mechanics
One subsection per mechanic: rule, inputs, feedback (visual/haptic/audio),
failure state, which core verb it serves.

## Actors / Enemies
Per actor: role, behavior sketch, player counterplay, spawn/pool needs
(everything spawned goes through VRForge.Core.ObjectPool).

## Level List
| # | Name | Purpose | Key mechanics introduced | Target minutes |
|---|------|---------|--------------------------|----------------|
| 1 | | | | |

## Asset Manifest
Every model, SFX, music cue, and VO line the game needs. Source is one of:
`generate-meshy`, `generate-blender`, `generate-elevenlabs`, `primitive`,
`store`. Budget is triangles for models, seconds for audio. Status is one of
`todo / generated / imported / prefabbed / placeholder`.

| id | type | description | source | budget | status |
|----|------|-------------|--------|--------|--------|
| mdl-crossbow | model | Hero prop: worn wooden crossbow, painted style | generate-meshy | 8k tris | todo |
| mdl-crate | model | Kit-bash storage crate, hard-surface, 3 size variants | generate-blender | 1k tris | todo |
| mdl-target-dummy | model | Placeholder capsule + cube dummy until art pass | primitive | 200 tris | placeholder |
| sfx-bolt-impact | sfx | Crossbow bolt hitting wood, 3 round-robin variants | generate-elevenlabs | 2 s each | todo |
| mus-explore-loop | music | Calm exploration loop, seamless, low percussion | generate-elevenlabs | 90 s | todo |
| vo-intro-guide | vo | Guide NPC intro line: "The range is yours, recruit." | generate-elevenlabs | 6 s | todo |
| mdl-env-rocks | model | Background rock set from asset store pack | store | 15k tris total | todo |

## Perf Budget Reference
Budgets live in `templates/perf-budget.md`. Record here only the deltas this
game claims (e.g. "no realtime shadows at all, budget spent on particles").

## Scope Tier
`jam` (1 level, 1 mechanic) / `vertical-slice` (1 polished level, full loop) /
`full` (shippable). The asset manifest must be priced against this tier.

## Open Questions
Numbered list. Each entry: question, owner (agent), phase by which it must
be answered. `[NEED: data from X]` items land here too.
