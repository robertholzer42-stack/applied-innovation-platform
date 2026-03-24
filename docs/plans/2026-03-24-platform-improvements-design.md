# Applied Innovation Platform: Harness Improvements Design

**Date:** 2026-03-24
**Author:** Bob Holzer + Claude
**Status:** Approved
**Source:** Anthropic "Harness Design for Long-Running Apps" article + post-engagement feedback (2026-03-19)

---

## Problem Statement

The platform's March 19 test run exposed three categories of issues:

1. **Context exhaustion** -- Running 10+ agents sequentially in one session causes compaction, degrading Publisher output quality
2. **Inconsistent agent quality** -- Core agents (Scout, Empathy, Architect) are gold-standard; intersection and operational agents are thin
3. **No independent quality review** -- Agents evaluate their own work, leading to self-evaluation bias (confirmed by Anthropic's research)

## Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Agent quality target | Full gold-standard, all 14 agents | Consistent output quality across the platform |
| QA mechanism | New 14th agent: Critic | Clean separation of concerns |
| Context management | Tier-based checkpoints with file handoffs | Proven by Anthropic research; engagement folders already support it |
| Implementation approach | 3 layered passes | Reviewable, each pass builds on the previous |

---

## Pass 1: Foundation (All SKILL.md Files)

### Gold-Standard Template

Every agent SKILL.md gets these mandatory sections:

1. **Header** -- Agent name, codename, role, tier, one-line personality
2. **Framework** -- Named multi-step methodology with clear stages
3. **Toolkit** -- Each tool: purpose, inputs, process, outputs
4. **Tiered Depth** -- Quick (15-30 min), Standard (2-4 hrs), Deep (full day+)
5. **Output Format** -- Structured Markdown template with section headers
6. **Quality Standards** -- 3-5 specific quality checks
7. **Writing Rules** -- Banned words, tone, evidence requirements, no em dashes
8. **Handoff Block** -- Structured data for Conductor, Publisher, Scorekeeper, Critic
9. **Boundaries** -- What this agent owns vs. what belongs elsewhere

### Handoff Block Standard

```
## Handoff
### For Conductor
- Key finding: [one sentence]
- DVFA contribution: [dimension] = [score] ([confidence])
- Tensions identified: [list]

### For Publisher
- Headline stat: [the one number that matters]
- Key visual: [recommended chart/diagram]
- Audience note: [who cares and why]

### For Scorekeeper
- Evidence strength: [H/M/L]
- Data gaps: [what's missing]

### For Critic
- Self-assessed confidence: [H/M/L]
- Known limitations: [what this analysis didn't cover]
```

### Writing Rules Block (embedded in every SKILL.md)

```
## Writing Rules
- Tone: Conversational and direct. Write like a smart colleague, not a corporate deck.
- Lead with the insight or finding, not the process.
- Every claim needs evidence: numbers, comparables, or citations.
- Use [NEED: data from X] for missing information. Never fabricate.
- Banned words: delve, landscape, synergy, leverage (as verb), robust, streamline, cutting-edge, paradigm, holistic, utilize
- No em dashes. Use commas, periods, or hyphens.
```

### Agents Requiring Significant Expansion

| Agent | Current Size | Current Rating | Key Gaps |
|-------|-------------|----------------|----------|
| Visionary | 2.8 KB | Thin | No tiered depth, sparse tool descriptions, no quality standards |
| Radar | 2.4 KB | Thin | Smallest skill, tools barely elaborated, no depth tiers |
| Banker | 2.9 KB | Adequate | Sparse tool descriptions, no tiered depth |
| Integrator | 3.6 KB | Adequate | No tiered depth, no quality standards |
| Sentinel | 3.9 KB | Adequate | No tiered depth, no quality standards |
| Bridge | 3.7 KB | Adequate | No tiered depth, no quality standards |
| Publisher | 4.0 KB | Adequate | No tiered depth, no quality standards, needs bridge templates |
| Navigator | 4.3 KB | Adequate | Partial tiers, sparse tool descriptions |
| Scorekeeper | 4.5 KB | Adequate | No tiered depth, needs two-pass design |

### Agents Requiring Minor Updates Only

| Agent | Current Size | Current Rating | Updates Needed |
|-------|-------------|----------------|----------------|
| Scout | 9.5 KB | Strong | Add handoff block + Critic section |
| Empathy | 8.1 KB | Strong | Add handoff block + Critic section |
| Architect | 8.8 KB | Strong | Add handoff block + Critic section |
| Conductor | 13 KB | Strong | Add checkpoint system + Critic integration (Pass 2) |

---

## Pass 2: New Capabilities

### Critic Agent (14th Agent)

- **Tier:** 4.5 (between Operational and Conductor)
- **Role:** Independent quality evaluator
- **Personality:** Constructive skeptic -- finds gaps and suggests fixes

**When Critic Runs:**
- Mandatory: After Tier 2 (Core) and after Tier 4 (Operational)
- Optional: Conductor can invoke mid-pipeline

**Evaluation Criteria:**
1. Completeness -- Did the agent follow its framework at the appropriate depth tier?
2. Evidence Quality -- Are claims backed by data, comparables, or citations?
3. Writing Standards -- Banned words, tone, em dashes, directness
4. Integration Readiness -- Can downstream agents use this output via handoff blocks?

**Output Format:**
```
## Critic Review: [Agent Name] -- [Engagement Name]
### Verdict: PASS | REVISE | FLAG

### Scores
- Completeness: [1-5]
- Evidence Quality: [1-5]
- Writing Standards: [1-5]
- Integration Readiness: [1-5]

### Issues Found
1. [Issue + criterion violated + suggested fix]

### What Works Well
- [Strength worth preserving]

### Recommendation
[Pass to next tier / Revise specific sections / Flag for Conductor]
```

**Key Rules:**
- Critic never rewrites agent output -- reviews and recommends only
- Critic reads from engagement folder files, not conversation context
- REVISE means rerun flagged sections only, not the full analysis

### Tier-Based Checkpoint System

**Checkpoint file format** (written by Conductor after each tier):

```
## Checkpoint: [Engagement Name] -- After Tier [N]
### Completed
- [Agent]: [one-line summary] -> [file path]

### Critic Review Summary
- [Agent]: PASS/REVISE -- [one-line verdict]

### DVFA Snapshot
- D: [score] | V: [score] | F: [score] | A: [score]

### Tensions Surfaced So Far
- [Tension description]

### Next Tier Instructions
- Agents to run: [list]
- Key questions to answer: [list]
- Files to read: [paths]

### Session Instructions
Start a new conversation. Load the platform (CLAUDE.md + skills/).
Read this checkpoint and the engagement files listed above.
Continue the pipeline from Tier [N+1].
```

**Session breakdown for a full (deep) run:**

| Session | Tiers | Agents | Output |
|---------|-------|--------|--------|
| 1 | 1 + 2 | Navigator, Scout, Empathy, Architect | checkpoint-tier2.md |
| 2 | Critic + 3 | Critic (Tier 2 review), Visionary, Integrator, Sentinel | checkpoint-tier3.md |
| 3 | Critic + 4 | Critic (Tier 3 review), Radar, Banker, Scorekeeper, Bridge | checkpoint-tier4.md |
| 4 | Critic + 5 + Deliverables | Critic (Tier 4 review), Conductor, Publisher | Final deliverables |

**Conductor determines session count at intake:**
- Quick scan (2-3 agents): 1 session
- Standard (6-8 agents): 2 sessions
- Deep (all 14 agents): 4 sessions

**Checkpoints go in:** `engagements/[name]/checkpoints/`

### Publisher Bridge Templates

Added to Publisher's SKILL.md. Map content sources to deliverable structure.

**Board-Ready Deck (10-12 slides):**

| Slide | Content Source | Layout |
|-------|---------------|--------|
| 1 | Engagement name, date, agent count | Title |
| 2 | Scorekeeper DVFA overall | Big stat callout |
| 3 | Scorekeeper DVFA breakdown | 4 visual cards |
| 4-6 | Conductor top 3 findings + agent fixes | Two-column with callout |
| 7 | Radar competitive context | Tiered cards |
| 8 | Banker resource/budget | Big number + breakdown |
| 9 | Bridge Phase 1 | 3 action cards |
| 10 | Sentinel monitoring | 3 time-horizon columns |
| 11 | Conductor call to action | Numbered items |

**Strategic Roadmap (3-5 pages):**

| Section | Content Source |
|---------|---------------|
| Executive Summary | Conductor synthesis |
| DVFA Verdict | Scorekeeper final scores |
| Phased Plan | Bridge + Banker |
| Monitoring | Sentinel + Scorekeeper |
| Stakeholders | Bridge + Empathy |

**One-Pager:**

| Section | Content Source |
|---------|---------------|
| Headline | Conductor top finding |
| Problem | Empathy pain points |
| Opportunity | Scout + Visionary |
| Why Now | Radar + Scout timing |
| The Ask | Conductor call to action |

### Two-Pass Scorekeeper

**Preliminary (after Tier 2):**
- Inputs: Scout, Empathy, Architect handoff blocks
- Output: Provisional DVFA with confidence levels
- File: `operations/scorekeeper-preliminary.md`
- Purpose: Baseline for Tier 3, early warning for Conductor

**Final (after Tier 4):**
- Inputs: All agent handoff blocks + Critic reviews
- Output: Official DVFA with full evidence chains + delta from preliminary
- File: `operations/scorekeeper-dvfa-scoring.md`

**Conductor routing adjustment:** If any preliminary DVFA dimension is below 2.0, Conductor flags it and weights subsequent agents toward that gap.

---

## Pass 3: Architecture Updates

### CLAUDE.md Updates
- Update agent roster from 13 to 14 (add Critic)
- Update pipeline diagram to include Critic review steps
- Update session breakdown table
- Add checkpoint folder to workspace structure
- Add headroom audit to Continuous Improvement section

### Headroom Audit Practice

Added to CLAUDE.md Continuous Improvement:

> After every 3-5 engagements, run a headroom check:
> 1. Which agent skills are over-scaffolding things the model handles natively?
> 2. Which agent boundaries are being crossed despite instructions?
> 3. Which Critic reviews consistently pass with no issues?
> 4. Test by running one engagement with a lighter version of one agent and compare output.

---

## Implementation Order

Pass 1 (Foundation): Update all 13 existing SKILL.md files to gold standard
Pass 2 (New Capabilities): Create Critic, add checkpoints to Conductor, add templates to Publisher, implement two-pass Scorekeeper
Pass 3 (Architecture): Update CLAUDE.md, routing tables, documentation
