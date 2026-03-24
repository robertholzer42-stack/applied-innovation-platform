# Applied Innovation Platform: Post-Engagement Feedback

**Engagement:** AI in Healthcare (2026-2031)
**Date:** 2026-03-19
**Pipeline Run:** Full 10-agent analysis + Publisher (3 deliverables)
**Agents Used:** Scout, Empathy, Architect, Visionary, Integrator, Sentinel, Radar, Banker, Scorekeeper, Bridge, Conductor, Publisher
**Agents Skipped:** Navigator (challenge was user-defined)
**Environment:** Claude Cowork (Opus), single conversation session

---

## What Worked Well

### 1. Challenge-to-Agent Routing Table
The routing matrix in CLAUDE.md is the fastest path from "user says a thing" to "agents start working." When the challenge was "AI in Healthcare," mapping to Scout + Empathy + Architect as primary with Radar and Scorekeeper supporting was immediate and unambiguous. This table should be preserved and expanded as new challenge types emerge.

### 2. DVFA as Shared Scoring Language
The Desirability-Viability-Feasibility-Adaptability framework gave every agent a common vocabulary. When Architect reported "viability is 2.5" and Empathy reported "desirability is 3.5," those scores composed naturally into Scorekeeper's final assessment without translation. This is the platform's strongest structural design choice. It turns 10 independent analyses into one coherent verdict.

### 3. Scout's Tiered Toolkit (Quick/Standard/Deep)
The explicit depth tiers prevented over-analysis and under-analysis. Knowing upfront that "standard" means 2-4 hours of depth set expectations for every downstream agent. This scoping mechanism should be replicated in other agents that don't currently have it.

### 4. Conductor's Tension Resolution Framework
This is the platform's real competitive advantage. Ten agents independently analyzing the same challenge naturally contradict each other (Scout's optimistic timelines vs. Sentinel's fragility warnings, Empathy's "people want this" vs. Bridge's "organizations can't absorb it"). The explicit mechanism to name and resolve tensions produced a more honest final output than any single-agent analysis would. The "portfolio is backwards" finding emerged specifically because Banker and Architect looked at the same data through different lenses and disagreed. Without the tension-surfacing step, that insight would have been buried.

### 5. Engagement Folder Structure
The predefined folder layout (intake/, analysis/, synthesis/, operations/, deliverables/) made output routing automatic. Each agent knew exactly where to write. This structure also made it easy for downstream agents to find upstream outputs.

---

## What Created Friction

### 1. Context Window Exhaustion (Critical)
Running 10 agents sequentially in a single conversation consumed the full context window. By the time Publisher started, the conversation had to be compacted, which meant Publisher lost direct access to nuanced content from earlier agents and was working from summaries instead of the actual analysis files. This is the single biggest operational problem with the platform today.

### 2. Inconsistent SKILL.md Depth Across Agents
Scout's SKILL.md is outstanding: IFTF framework, tiered toolkits, specific output formats, clear boundaries ("Action stage belongs to Conductor, not Scout"). Some other agents' SKILL.md files are thinner and leave more room for interpretation. Output quality varies directly with SKILL.md specificity. Agents with vague instructions produce vague outputs.

### 3. Banned Words Enforcement Is Reactive, Not Preventive
The banned words list (landscape, leverage, robust, streamline, synergy, etc.) caused repeated grep-and-fix cycles after every batch of agent outputs. Every sub-agent output had to be scanned and corrected post-generation. This is wasted compute and introduces risk that violations slip through. The enforcement happens after generation rather than being embedded in each agent's prompt.

### 4. Publisher Gap: Content-to-Design Translation
The 13 innovation agents have well-defined SKILL.md files, but Publisher's instructions don't bridge into the actual file-creation mechanics (PPTX design, DOCX formatting, HTML layout). There's a gap between "Conductor recommends a board-ready deck" and "how to translate 200 lines of strategy into 12 well-designed slides." The first PPTX attempt failed precisely because of this: the content was right but the design execution was poor. The deck had data overflowing slides, text running together, and no visual hierarchy.

### 5. Sub-Agent Context Inheritance
When agents run as sub-agents (parallel execution), they don't reliably inherit the full CLAUDE.md context. This means writing rules, banned words, and formatting standards may not apply unless explicitly passed in the sub-agent prompt. This caused inconsistencies that required post-hoc correction.

### 6. Scorekeeper Timing
Scorekeeper ran last in the operational tier, but DVFA scores would have been more useful as a mid-pipeline checkpoint. Intersection agents (Visionary, Integrator, Sentinel) were synthesizing without knowing the preliminary DVFA picture, and Conductor had to estimate scores before Scorekeeper confirmed them.

---

## Recommendations

### R1: Add a Context Budget and Multi-Session Pipeline (High Priority)

The Conductor's routing logic should estimate whether the full pipeline fits in a single conversation or needs to be split into phases. Suggested approach:

- **Single session:** Quick scan (2-3 agents) or targeted analysis (3-5 agents)
- **Two sessions:** Standard analysis (6-8 agents + Publisher)
- **Three sessions:** Deep dive (all 13 agents + Publisher with QA cycles)

For multi-session runs, the pipeline should have explicit "checkpoint" instructions:
- "After completing Tier 2 and Tier 3 agents, save all outputs to the engagement folder, write a checkpoint summary to `engagements/[name]/checkpoint-1.md`, and instruct the user to start a new conversation for Tier 4 + Publisher."

The engagement folder structure already supports this since agents write to files. The missing piece is documentation telling the Conductor when and how to break across sessions.

### R2: Embed Writing Rules in Every SKILL.md (High Priority)

Add a standardized "Writing Rules" section to every agent's SKILL.md file, not just the top-level CLAUDE.md. This section should include:

- The banned words list with suggested alternatives
- Tone guidance ("conversational and direct, not corporate")
- Evidence requirements ("every claim needs numbers, comparables, or citations")
- The "no em dashes" rule

This ensures that sub-agents generating in parallel all follow the same standards without relying on CLAUDE.md inheritance.

### R3: Create Publisher Bridge Templates (High Priority)

Add deliverable-specific translation guides to Publisher's SKILL.md or as companion files. For each deliverable type, provide a mapping from platform content to presentation structure:

**Board-Ready Deck (10-15 slides):**
- Slide 1: Title (engagement name, date, agent count)
- Slide 2: Verdict (big stat callout for overall DVFA score)
- Slide 3: DVFA scorecard (4 visual cards, not a table)
- Slides 4-6: Key findings (two-column with "THE FIX" callout)
- Slide 7: Competitive context (tiered cards)
- Slide 8: Budget overview (big number + bar chart)
- Slide 9: Phase 1 detail (3 cards, not a table)
- Slide 10: Monitoring dashboard (3 time-horizon columns)
- Slide 11: Call to action (numbered items)
- Slide 12: Close

**Strategic Roadmap (3-5 pages):** Executive summary, phased plan with tables, monitoring dashboard, stakeholder matrix.

**One-Pager:** Emotional value story, not ROI. Single-page HTML or PDF.

These templates prevent the "content is right but design is wrong" failure mode.

### R4: Add Structured Handoff Sections to Agent Output Formats (Medium Priority)

Each agent's output format should end with a structured handoff block that downstream agents can parse quickly:

```
## Handoff
### For Conductor
- Key finding: [one sentence]
- DVFA contribution: [dimension] = [score] ([confidence])
- Tensions identified: [list]

### For Publisher
- Headline stat: [the one number that matters]
- Key visual: [what chart/diagram would communicate this]
- Audience note: [who cares about this finding and why]

### For Scorekeeper
- Evidence strength: [H/M/L]
- Data gaps: [what's missing]
```

This eliminates the need for downstream agents to re-read entire documents to extract what they need, and significantly reduces context consumption.

### R5: Implement Two-Pass Scorekeeper (Medium Priority)

Split Scorekeeper into two passes:

- **Preliminary pass** (after Tier 2 Core agents): Score D, V, F, A based on Scout + Empathy + Architect evidence. This gives Intersection agents (Visionary, Integrator, Sentinel) a baseline to work from.
- **Final pass** (after Tier 4 Operational agents): Revise scores with full evidence from all agents. This becomes the official DVFA score.

The preliminary score also gives Conductor an early warning if a dimension is critically low, allowing pipeline adjustments (e.g., "Feasibility is 1.5, add extra weight to Bridge and Banker analysis").

### R6: Add Navigator-Lite Auto-Intake (Medium Priority)

Even when the challenge is user-defined, a 3-5 question scoping step would improve downstream quality. Suggested questions:

1. What's the time horizon?
2. Who's the primary audience for the final deliverables?
3. What decision does this analysis need to inform?
4. Are there specific dimensions you care about most (future trends, human experience, system dynamics, competitive positioning, portfolio fit, change readiness)?
5. Quick scan, standard analysis, or deep dive?

This could be a lightweight "Navigator-lite" that runs automatically before Conductor routes, or it could be built into Conductor's initial prompt.

### R7: Templatize Knowledge Base Capture (Low Priority)

The CLAUDE.md instruction "After every engagement, capture reusable patterns in knowledge-base/patterns/" needs a template. Create `knowledge-base/patterns/TEMPLATE.md`:

```
# Pattern: [Name]
**Source Engagement:** [name]
**Contributing Agents:** [list]
**Pattern Type:** [structural | behavioral | strategic | operational]

## Context
When does this pattern apply?

## The Pattern
What did we observe?

## Implication
What should you do when you see this?

## Evidence
What data supports this?
```

Without a template, this step will be skipped every time.

### R8: Add a "Devil's Advocate" Pass (Low Priority, High Value)

Consider adding an optional Conductor sub-step where two agents are deliberately pitted against each other on the engagement's most critical assumption. The Conductor would:

1. Identify the single assumption that, if wrong, invalidates the most recommendations
2. Assign one agent to argue FOR the assumption and one to argue AGAINST
3. Synthesize the debate into a confidence-adjusted finding

This would formalize the tension-surfacing that already happens informally and make it the platform's signature analytical move.

### R9: Standardize SKILL.md Quality Across All Agents (Medium Priority)

Audit all 13 SKILL.md files against Scout's as the gold standard. Each should include:

- Clear framework with named steps (not just general guidance)
- Tiered depth options (quick/standard/deep) with time estimates
- Specific output format with section headers
- Defined boundaries ("this agent does X, not Y")
- Example outputs or output fragments
- Handoff section template (see R4)
- Writing rules (see R2)

Agents whose SKILL.md files are currently thin: prioritize bringing them up to Scout's level of specificity.

### R10: Document Parallel Execution Patterns (Low Priority)

The pipeline diagram shows which agents can run in parallel, but there's no documentation on how to actually do this in practice. Add a section to the Conductor's SKILL.md or setup guide:

- Which agents have zero dependencies and can run simultaneously
- How to pass upstream outputs to parallel agents
- How to merge parallel outputs back into the pipeline
- What to do when parallel agents produce conflicting findings

The pattern we used: Scout first, then Visionary + Sentinel in parallel, then Empathy + Architect in parallel, then Integrator + Bridge + Radar + Banker in parallel, then Scorekeeper last. This should be documented as a reference execution plan.

---

## Priority Summary

| Priority | Recommendation | Impact |
|----------|---------------|--------|
| High | R1: Context budget + multi-session pipeline | Prevents the #1 failure mode (context exhaustion) |
| High | R2: Writing rules in every SKILL.md | Eliminates post-generation fix cycles |
| High | R3: Publisher bridge templates | Prevents "right content, wrong design" deliverables |
| Medium | R4: Structured handoff sections | Reduces context waste, speeds pipeline |
| Medium | R5: Two-pass Scorekeeper | Better mid-pipeline intelligence |
| Medium | R6: Navigator-lite auto-intake | Better scoping, fewer surprises at Publisher |
| Medium | R9: SKILL.md quality audit | Consistent output quality across all agents |
| Low | R7: Knowledge base template | Makes post-engagement capture actually happen |
| Low | R8: Devil's Advocate pass | Formalizes the platform's best analytical move |
| Low | R10: Parallel execution docs | Helps future runs execute faster |

---

## Raw Metrics from This Engagement

- **Total agents run:** 10 of 13 (+ Conductor + Publisher)
- **Outputs generated:** 11 analysis files + 3 deliverables
- **Context compactions:** 1 (occurred during Publisher phase)
- **Banned word violations caught:** 8+ across 6 agent outputs
- **PPTX iterations:** 3 (v1 failed QA, v2 had 2 critical issues, v3 passed)
- **DVFA score evolution:** 3.4 (initial estimate) -> 3.3 (after Empathy+Architect) -> 2.7 (final Scorekeeper)
- **Total engagement folder size:** 14 files across 5 subdirectories

---

*Prepared by: Publisher (post-engagement retrospective)*
*Engagement: AI in Healthcare | 2026-03-19*
