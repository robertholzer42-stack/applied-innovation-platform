# Applied Innovation Platform: Harness Improvements Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Upgrade the Applied Innovation Platform from 13 to 14 agents, add tier-based checkpoints, and bring all agent skills to gold-standard quality.

**Architecture:** Three layered passes. Pass 1 standardizes all existing SKILL.md files. Pass 2 adds new capabilities (Critic agent, checkpoints, Publisher templates, two-pass Scorekeeper). Pass 3 updates CLAUDE.md and documentation to reflect the 14-agent architecture.

**Tech Stack:** Markdown skill files, git version control

---

## Pass 1: Foundation (Standardize All SKILL.md Files)

The goal: every agent gets writing rules, handoff blocks, tiered depth, quality standards, and expanded tool descriptions. Strong agents (Scout, Empathy, Architect, Conductor) get minor additions. Thin agents get significant expansion.

---

### Task 1: Add Handoff Block + Critic Section to Scout

**Files:**
- Modify: `skills/scout-SKILL.md`

**Step 1: Add the handoff block template**

After the existing `## Quality Standards` section and before the end of file, add:

```markdown
## Handoff

Every Scout output must end with this structured handoff block:

### For Conductor
- Key finding: [one sentence summarizing the most important foresight insight]
- DVFA contribution: Adaptability = [1-5] ([H/M/L] confidence)
- Tensions identified: [list any conflicts with assumptions or other agents' likely findings]

### For Publisher
- Headline stat: [the single most striking data point or forecast]
- Key visual: [recommended visualization -- e.g., scenario 2x2, signal heatmap, S-curve]
- Audience note: [who in the client's organization cares most about this finding and why]

### For Scorekeeper
- Evidence strength: [H/M/L]
- Data gaps: [specific data that would improve confidence]

### For Critic
- Self-assessed confidence: [H/M/L]
- Known limitations: [what this analysis didn't cover -- e.g., specific geographies, regulatory domains, stakeholder groups]
```

**Step 2: Commit**

```bash
git add skills/scout-SKILL.md
git commit -m "Add handoff block and Critic section to Scout SKILL.md"
```

---

### Task 2: Add Handoff Block + Critic Section to Empathy

**Files:**
- Modify: `skills/empathy-SKILL.md`

**Step 1: Add the handoff block template**

After the existing `## Quality Standards` section and before the end of file, add:

```markdown
## Handoff

Every Empathy output must end with this structured handoff block:

### For Conductor
- Key finding: [one sentence summarizing the most important human insight]
- DVFA contribution: Desirability = [1-5] ([H/M/L] confidence)
- Tensions identified: [list any conflicts between user needs and system/business constraints]

### For Publisher
- Headline stat: [the single most compelling user data point or pain point metric]
- Key visual: [recommended visualization -- e.g., journey map, persona card, barrier chart]
- Audience note: [who in the client's organization cares most about this finding and why]

### For Scorekeeper
- Evidence strength: [H/M/L]
- Data gaps: [specific user research that would improve confidence]

### For Critic
- Self-assessed confidence: [H/M/L]
- Known limitations: [what this analysis didn't cover -- e.g., specific user segments, geographic contexts, accessibility considerations]
```

**Step 2: Commit**

```bash
git add skills/empathy-SKILL.md
git commit -m "Add handoff block and Critic section to Empathy SKILL.md"
```

---

### Task 3: Add Handoff Block + Critic Section to Architect

**Files:**
- Modify: `skills/architect-SKILL.md`

**Step 1: Add the handoff block template**

After the existing `## Quality Standards` section and before the end of file, add:

```markdown
## Handoff

Every Architect output must end with this structured handoff block:

### For Conductor
- Key finding: [one sentence summarizing the most important system insight]
- DVFA contribution: Viability = [1-5] ([H/M/L] confidence)
- Tensions identified: [list any conflicts between system constraints and proposed innovations]

### For Publisher
- Headline stat: [the single most revealing system metric -- e.g., number of dependencies, single points of failure, feedback loop count]
- Key visual: [recommended visualization -- e.g., causal loop diagram, system map, viability scorecard]
- Audience note: [who in the client's organization cares most about this finding and why]

### For Scorekeeper
- Evidence strength: [H/M/L]
- Data gaps: [specific system data that would improve confidence]

### For Critic
- Self-assessed confidence: [H/M/L]
- Known limitations: [what this analysis didn't cover -- e.g., specific subsystems, external regulatory bodies, technical infrastructure details]
```

**Step 2: Commit**

```bash
git add skills/architect-SKILL.md
git commit -m "Add handoff block and Critic section to Architect SKILL.md"
```

---

### Task 4: Expand Visionary to Gold Standard

**Files:**
- Modify: `skills/visionary-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Replace the entire contents of `skills/visionary-SKILL.md` with the expanded version that adds:
- Communication Style section (matching Scout/Empathy/Architect pattern)
- Tiered Depth section (Quick/Standard/Deep) with scope for each
- Expanded tool descriptions with inputs, process, and outputs for each tool
- Quality Standards section
- Writing Rules section (embedded, not inherited)
- Handoff block template
- Boundaries section

The expanded Visionary should include:

**Tiered Depth:**
- Quick (15-30 min): Future persona sketch for primary persona only, 1 scenario prototype test, brief experience trajectory. Summary: "Here's how your user changes in 5 years."
- Standard (2-4 hrs): Full persona evolution across 2-3 personas, scenario-prototype matrix for 2-3 scenarios, 1-2 speculative design artifacts, experience roadmap draft. Output: Visionary brief (2-3 pages).
- Deep (full day+): Comprehensive persona evolution (3-5 personas, 3 time horizons), full scenario-prototype matrix, 3+ speculative design artifacts with detailed rationale, complete experience roadmap with milestones. Output: Full speculative design report (5-10 pages).

**Quality Standards:**
- Every future persona must trace back to a specific Empathy persona AND a specific Scout forecast
- Scenario-prototype matrix must test concepts against at least 2 genuinely different futures
- Speculative design artifacts must be concrete enough to provoke a reaction (not abstract descriptions)
- Experience roadmaps must align with Scout's forecast timeline
- Never extrapolate user behavior without citing the underlying trend or research
- Use [NEED: data from X] for missing information, never fabricate

**Writing Rules, Handoff Block, and Boundaries** sections follow the standard template from the design doc.

**Step 2: Commit**

```bash
git add skills/visionary-SKILL.md
git commit -m "Expand Visionary SKILL.md to gold standard"
```

---

### Task 5: Expand Integrator to Gold Standard

**Files:**
- Modify: `skills/integrator-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Add to the existing content:
- Communication Style section
- Tiered Depth section:
  - Quick (15-30 min): Journey-system overlay for primary journey only, top 3 scalability risks, summary compatibility check. "Here's what breaks when you try to scale this."
  - Standard (2-4 hrs): Full journey-system overlay, scalability assessment, stakeholder alignment plan, compatibility matrix, phased blueprint outline. Output: 2-3 page brief.
  - Deep (full day+): Comprehensive overlay with all personas, detailed scalability modeling, full stakeholder alignment with per-actor strategies, complete compatibility audit, phased blueprint with go/no-go gates and rollback plans. Output: 5-10 page report.
- Quality Standards:
  - Every journey-system gap must identify whether the root cause is design or system
  - Scalability assessments must name the first thing that breaks at 10x scale
  - Stakeholder alignment must include specific actions, not just "engage stakeholders"
  - Compatibility ratings must cite evidence for red/yellow/green classifications
  - Implementation blueprints must include rollback plans for each phase
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template
- Boundaries section: "Integrator owns pilot-to-production pathway. Current-state user research belongs to Empathy. System mapping belongs to Architect. Change management belongs to Bridge."

**Step 2: Commit**

```bash
git add skills/integrator-SKILL.md
git commit -m "Expand Integrator SKILL.md to gold standard"
```

---

### Task 6: Expand Sentinel to Gold Standard

**Files:**
- Modify: `skills/sentinel-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Add to the existing content:
- Communication Style section
- Tiered Depth section:
  - Quick (15-30 min): Top 3 load-bearing assumptions identified, resilience rating for most likely scenario, 1 pre-mortem failure mode. "Here's what breaks first."
  - Standard (2-4 hrs): Resilience scorecard across 2-3 scenarios, full assumption register (top 10), adaptive strategy with core/conditional/option-creating moves, pre-mortem for top 3 failure modes. Output: 2-3 page brief.
  - Deep (full day+): Comprehensive resilience mapping across all Scout scenarios, full assumption stress test with testability assessment, detailed adaptive strategy with trigger points, complete pre-mortem (5+ failure modes), capability gap analysis with investment timeline. Output: 5-10 page report.
- Quality Standards:
  - Every resilience rating must cite specific system vulnerabilities from Architect's analysis
  - Assumptions must be classified as testable-now vs. unknowable-until-later
  - Adaptive strategies must include specific, observable trigger points (not vague conditions)
  - Pre-mortem failure modes must include early warning signals that are actually measurable
  - Capability gaps must be prioritized by "needed regardless of scenario" first
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template (DVFA contribution: Adaptability, shared with Scout)
- Boundaries section: "Sentinel owns resilience and adaptive strategy. Future scanning belongs to Scout. System mapping belongs to Architect. Change execution belongs to Bridge."

**Step 2: Commit**

```bash
git add skills/sentinel-SKILL.md
git commit -m "Expand Sentinel SKILL.md to gold standard"
```

---

### Task 7: Expand Radar to Gold Standard

**Files:**
- Modify: `skills/radar-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

This is the thinnest skill (2.4 KB). Expand significantly:
- Communication Style section
- Tiered Depth section:
  - Quick (15-30 min): Top 5 competitors identified and classified (direct/indirect/emerging), 3-5 recent market signals, quick positioning sketch. "Here's who you're up against."
  - Standard (2-4 hrs): Full competitor profiling (8-12 competitors), market signal scan with relevance ratings, positioning map on 2 key dimensions, threat/opportunity register with top 5 prioritized. Output: 2-3 page brief.
  - Deep (full day+): Comprehensive competitor deep-dive (15+ competitors with strategy analysis), full signal monitoring across patents/M&A/funding/hires/launches, multi-dimensional positioning maps, detailed threat modeling with competitor next-move predictions, white space analysis. Output: 5-10 page report.
- Expanded tool descriptions with methodology detail:
  - Competitor Identification: explain the direct/indirect/emerging framework, how to assess apparent strategy, how to identify strengths/weaknesses
  - Market Signal Monitoring: explain signal types (product launches, partnerships, funding, hires, pivots, patents, M&A, regulatory), relevance rating methodology, timeliness assessment
  - Competitive Positioning Map: explain dimension selection (choose the 2 dimensions the market cares about most), how to plot, how to identify white space vs. crowded zones
  - Threat & Opportunity Assessment: explain threat classification (existential/serious/minor), next-move prediction methodology, defensible advantage identification
- Quality Standards:
  - Every competitor profile must include their apparent strategy, not just what they sell
  - Market signals must include source and date (not just "Company X is doing Y")
  - Positioning maps must justify dimension choices (why these axes matter)
  - Threat assessments must include "likely next move" predictions, not just current state
  - White space opportunities must be validated against user needs (cross-reference with Empathy)
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template
- Boundaries section: "Radar owns competitive intelligence and market positioning. Future trend analysis belongs to Scout. User needs validation belongs to Empathy. Portfolio fit belongs to Banker."

**Step 2: Commit**

```bash
git add skills/radar-SKILL.md
git commit -m "Expand Radar SKILL.md to gold standard"
```

---

### Task 8: Expand Banker to Gold Standard

**Files:**
- Modify: `skills/banker-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Add to the existing content:
- Communication Style section
- Tiered Depth section:
  - Quick (15-30 min): Horizon classification for the initiative, quick portfolio balance check, resource fit assessment. "This is an H2 play and you're already overweight on H2."
  - Standard (2-4 hrs): Full Three Horizons classification with evidence, pipeline health assessment with bottleneck identification, resource allocation gap analysis, portfolio fit scoring with recommendation. Output: 2-3 page brief.
  - Deep (full day+): Comprehensive portfolio audit across all horizons, detailed pipeline funnel analysis with kill rate and velocity metrics, full resource allocation modeling with opportunity cost analysis, scenario-based portfolio stress test (what if H3 bets fail?), portfolio rebalancing roadmap. Output: 5-10 page report.
- Expanded tool descriptions:
  - Three Horizons: explain the 70/20/10 benchmark, how to classify ambiguous initiatives, common misclassification patterns (organizations calling H1 improvements "innovation"), how horizon classification shifts over time
  - Pipeline Health: explain stage definitions (ideation/validation/pilot/scale/optimize), healthy funnel shape, kill rate benchmarks (60-80% early kills is healthy), velocity metrics (idea-to-pilot time)
  - Resource Allocation: explain the strategy-resource alignment test, how to detect "say H3, fund H1" patterns, opportunity cost calculation methodology
  - Portfolio Fit: explain scoring dimensions (gap-filling vs. redundancy, risk diversification, strategic alignment), recommendation framework (add/defer/kill/accelerate)
- Quality Standards:
  - Horizon classifications must cite specific evidence (not just "this feels like H2")
  - Pipeline assessments must include kill rate data or flag its absence
  - Resource allocation must compare stated strategy vs. actual spending
  - Portfolio fit recommendations must consider existing portfolio balance, not just the initiative in isolation
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template (DVFA contribution: Feasibility, shared with Bridge)
- Boundaries section: "Banker owns portfolio balance and resource allocation. Individual idea scoring belongs to Scorekeeper. Future trend analysis belongs to Scout. Change management belongs to Bridge."

**Step 2: Commit**

```bash
git add skills/banker-SKILL.md
git commit -m "Expand Banker SKILL.md to gold standard"
```

---

### Task 9: Expand Bridge to Gold Standard

**Files:**
- Modify: `skills/bridge-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Add to the existing content:
- Communication Style section
- Tiered Depth section:
  - Quick (15-30 min): Adoption readiness snapshot (5 dimensions scored), top 3 resistance sources identified, change capacity red/yellow/green. "People aren't ready -- here's why."
  - Standard (2-4 hrs): Full adoption readiness scorecard, stakeholder resistance map with root causes, communication plan outline, transition design with phases, change capacity assessment with timing recommendation. Output: 2-3 page brief.
  - Deep (full day+): Comprehensive readiness assessment with per-group scores, detailed resistance analysis with per-stakeholder strategies, full communication plan matrix, complete transition design with parallel-run planning and training design, change capacity modeling with competing initiative impact, reinforcement design for sustained adoption. Output: 5-10 page report.
- Expanded tool descriptions:
  - Adoption Readiness: explain the 5-dimension framework (awareness/understanding/capability/motivation/reinforcement), scoring methodology (1-5 per dimension per stakeholder group), gap prioritization logic
  - Stakeholder Resistance: explain the champion/willing/neutral/resistant/hostile classification, root cause categories (fear/loss of control/past experience/lack of trust/legitimate concerns), the principle "resistance is information, not obstruction"
  - Communication Planning: explain audience segmentation, message framing per audience, channel selection, timing sequencing (pre-announce/explain/demonstrate/support/reinforce)
  - Transition Design: explain the current/transition/future state model, parallel running strategy, training plan design, support structure (help desk/champions network/feedback loops)
  - Change Capacity: explain saturation indicators, fatigue signals, infrastructure requirements, the proceed/defer/simplify/phase recommendation framework
- Quality Standards:
  - Readiness scores must be per-stakeholder-group, not organization-wide averages
  - Resistance root causes must go deeper than "people don't like change"
  - Communication plans must specify channel AND timing, not just message
  - Transition designs must include what stops, not just what starts
  - Change capacity assessments must name competing initiatives specifically
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template (DVFA contribution: Feasibility, shared with Banker)
- Boundaries section: "Bridge owns adoption readiness and change management. User needs research belongs to Empathy. System constraints belong to Architect. Resource allocation belongs to Banker."

**Step 2: Commit**

```bash
git add skills/bridge-SKILL.md
git commit -m "Expand Bridge SKILL.md to gold standard"
```

---

### Task 10: Expand Scorekeeper to Gold Standard + Two-Pass Design

**Files:**
- Modify: `skills/scorekeeper-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Add to the existing content:
- Communication Style section
- Two-Pass Scoring Design (new section):
  - Preliminary Pass (after Tier 2): Inputs are Scout, Empathy, Architect handoff blocks only. Produces provisional DVFA with confidence levels. Output file: `operations/scorekeeper-preliminary.md`. Purpose: baseline for Tier 3 intersection agents, early warning for Conductor.
  - Final Pass (after Tier 4): Inputs are all agent handoff blocks + Critic reviews. Produces official DVFA with full evidence chains and delta from preliminary. Output file: `operations/scorekeeper-dvfa-scoring.md`. Includes score movement narrative (e.g., "Viability dropped from 3.0 to 2.5 after Sentinel stress test revealed single point of failure in supply chain").
  - Conductor routing trigger: If any preliminary dimension is below 2.0, Conductor flags it and weights subsequent agents toward that gap. If all above 3.5, Conductor may authorize lighter Tier 4 pass.
- Tiered Depth section:
  - Quick (15-30 min): DVFA scoring on available evidence (may be incomplete), confidence-weighted. "Based on what we know, this scores 2.7 with low confidence on Feasibility."
  - Standard (2-4 hrs): Full two-pass DVFA with evidence chains per dimension, innovation maturity snapshot, comparison scoring if multiple options. Output: 2-3 page scorecard.
  - Deep (full day+): Comprehensive two-pass DVFA with detailed evidence audit, full innovation maturity assessment (8 dimensions), KPI dashboard design, sensitivity analysis (how scores change under different assumptions), comparison across 3+ options. Output: 5-10 page assessment.
- Expanded tool descriptions:
  - DVFA Scoring: explain evidence chain methodology (each score must trace back to specific agent findings), confidence level calibration (H = direct evidence, M = inferred from related data, L = assumption-based), weighting adjustment rationale
  - Innovation Maturity: explain the 8-dimension, 5-level framework in full detail (it's currently a table -- add guidance on how to assess each level, what evidence to look for)
  - KPI Design: explain the input/activity/output/outcome/leading indicator hierarchy, how to select metrics appropriate to maturity level, anti-patterns (measuring activity instead of outcomes)
- Quality Standards:
  - Every DVFA score must cite the specific agent finding that supports it
  - Confidence levels must be justified (not just "Medium" -- explain why)
  - Preliminary and final scores must include delta narrative explaining what changed
  - Maturity assessments must include evidence per dimension, not just a number
  - KPI recommendations must match the organization's maturity level
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template
- Boundaries section: "Scorekeeper owns scoring and measurement. Desirability evidence comes from Empathy. Viability evidence comes from Architect. Adaptability evidence comes from Scout/Sentinel. Feasibility evidence comes from Bridge/Banker. Scorekeeper scores but does not generate the underlying analysis."

**Step 2: Commit**

```bash
git add skills/scorekeeper-SKILL.md
git commit -m "Expand Scorekeeper to gold standard with two-pass scoring"
```

---

### Task 11: Expand Navigator to Gold Standard

**Files:**
- Modify: `skills/navigator-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

Add to the existing content:
- Communication Style section
- Formalized Tiered Depth (currently partial):
  - Quick (15-30 min): 5-question rapid intake (from R6 Navigator-lite: time horizon, audience, decision to inform, priority dimensions, depth preference), challenge framing, agent routing recommendation. "We're doing a standard analysis focused on future readiness and system viability."
  - Standard (2-4 hrs): Full Challenge Discovery Interview, innovation maturity snapshot (top gaps only), engagement design with agent assignments and timeline, folder setup. Output: Complete engagement brief.
  - Deep (full day+): Comprehensive discovery with stakeholder interviews, full 8-dimension maturity assessment, detailed engagement design with session planning (checkpoint system), deliverable specification, success criteria definition. Output: Full engagement brief + maturity report.
- Expanded tool descriptions:
  - Challenge Discovery Interview: already good -- add guidance on how to probe deeper on each question, red flags that indicate the challenge is poorly framed
  - Innovation Maturity Assessment: add cross-reference to Scorekeeper's maturity framework, explain how maturity gaps drive agent prioritization
  - Engagement Design: add the session planning component (from checkpoint system design -- Navigator determines whether the engagement needs 1, 2, or 4 sessions based on scope)
  - Engagement Folder Setup: add checkpoints/ subfolder to the standard structure
- Quality Standards:
  - Challenge statements must be specific enough to route to agents (not "improve innovation")
  - Maturity assessments must identify the gap that matters most for THIS challenge
  - Engagement designs must include session count recommendation (quick=1, standard=2, deep=4)
  - Folder setup must include checkpoints/ directory for multi-session engagements
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block template (Navigator's handoff goes to Conductor for routing)
- Boundaries section: "Navigator owns intake and scoping. Analysis belongs to Tier 2+ agents. Maturity scoring methodology belongs to Scorekeeper. Navigator frames the question; other agents answer it."

**Step 2: Commit**

```bash
git add skills/navigator-SKILL.md
git commit -m "Expand Navigator SKILL.md to gold standard"
```

---

### Task 12: Expand Publisher to Gold Standard + Bridge Templates

**Files:**
- Modify: `skills/publisher-SKILL.md`

**Step 1: Rewrite the full SKILL.md**

This is a major expansion. Add:
- Communication Style section
- Tiered Depth section:
  - Quick (15-30 min): Executive summary (1 page) or one-pager. Pulls from available handoff blocks. "Here's the headline for your stakeholders."
  - Standard (2-4 hrs): Board-ready deck (10-12 slides) OR strategy report (3-5 pages). Uses full handoff blocks and reads key agent files for detail. Output: One primary deliverable.
  - Deep (full day+): Full deliverable suite: deck + report + one-pager + workshop guide if applicable. Reads all agent files in detail. Output: Multiple deliverables tailored to different audiences.

- **Bridge Templates** section (the content-to-deliverable mapping from the design doc):

  **Board-Ready Deck Template (10-12 slides):**
  | Slide | Content Source | Layout | Notes |
  |-------|---------------|--------|-------|
  | 1 | Engagement metadata | Title slide | Name, date, agent count, depth tier |
  | 2 | Scorekeeper DVFA overall | Big stat callout | Single large number with verdict |
  | 3 | Scorekeeper DVFA breakdown | 4 visual cards | One card per dimension, not a table |
  | 4-6 | Conductor top 3 findings | Two-column with callout | Finding on left, "THE FIX" on right |
  | 7 | Radar competitive context | Tiered positioning cards | Direct/indirect/emerging competitors |
  | 8 | Banker resource/budget | Big number + breakdown | Investment required with horizon split |
  | 9 | Bridge Phase 1 | 3 action cards | First 90 days, concrete actions |
  | 10 | Sentinel monitoring | 3 time-horizon columns | What to watch at 6mo/1yr/3yr |
  | 11 | Conductor call to action | Numbered decision items | What needs a YES from leadership |

  **Strategic Roadmap Template (3-5 pages):**
  | Section | Content Source | Notes |
  |---------|---------------|-------|
  | Executive Summary | Conductor synthesis | 1 paragraph, lead with the verdict |
  | DVFA Verdict | Scorekeeper final scores | Include delta from preliminary if available |
  | Phased Plan | Bridge transition + Banker horizons | Tables by phase with owners and timelines |
  | Monitoring Dashboard | Sentinel metrics + Scorekeeper KPIs | Organized by time horizon |
  | Stakeholder Matrix | Bridge resistance map + Empathy personas | Who to engage, how, when |

  **One-Pager Template:**
  | Section | Content Source | Notes |
  |---------|---------------|-------|
  | Headline | Conductor's top finding | Emotional, not analytical |
  | The Problem | Empathy pain points | Lead with the human story |
  | The Opportunity | Scout + Visionary | Future state vision |
  | Why Now | Radar + Scout timing | Competitive window or trend acceleration |
  | The Ask | Conductor call to action | 1-2 items max, specific |

- Quality Standards:
  - Every slide/section must trace to a specific agent's handoff block or output file
  - Deliverables must follow writing rules (banned words, tone, no em dashes)
  - Decks must have no more than 20 words per slide body (excluding titles and tables)
  - Reports must lead with findings, not methodology
  - One-pagers must be scannable in 60 seconds
  - Use [NEED: data from X] for missing information, never fabricate
- Writing Rules (embedded)
- Handoff block: Publisher is the terminal agent -- no downstream handoff needed. Instead, add a "Deliverable Manifest" output listing all artifacts produced with file paths.
- Boundaries section: "Publisher owns deliverable production. Content analysis belongs to upstream agents. Publisher translates and formats -- it does not generate new analytical findings."

**Step 2: Commit**

```bash
git add skills/publisher-SKILL.md
git commit -m "Expand Publisher to gold standard with bridge templates"
```

---

### Task 13: Pass 1 Review Checkpoint

**Step 1: Verify all 12 SKILL.md files have been updated**

```bash
cd /path/to/applied-innovation-platform
for f in skills/*-SKILL.md; do
  echo "=== $f ==="
  grep -c "## Handoff" "$f"
  grep -c "## Writing Rules" "$f" || grep -c "Communication Style" "$f"
  grep -c "## Quality Standards" "$f"
  grep -c "Quick.*15-30" "$f" || grep -c "Tiered" "$f"
done
```

Expected: Every file should have at least 1 match for Handoff, Writing Rules or Communication Style, Quality Standards, and Tiered/Quick references.

**Step 2: Commit the verification**

No commit needed -- this is a review gate.

---

## Pass 2: New Capabilities

---

### Task 14: Create Critic Agent SKILL.md

**Files:**
- Create: `skills/critic-SKILL.md`

**Step 1: Write the complete Critic SKILL.md**

```markdown
# Quality Evaluator Agent - "Critic"

## Skill Overview

**Name:** Quality Evaluator Agent
**Codename:** Critic
**Category:** Quality Assurance / Independent Review
**Version:** 1.0

**Personality:** Constructive skeptic. Not adversarial -- finds gaps and suggests fixes. Thinks like a peer reviewer: thorough, fair, specific. Never vague ("this could be better") -- always concrete ("Section 3 claims adoption is likely but cites no user research evidence").
**Voice:** "This is solid work. Here are three things that would make it stronger."

**Communication Style:**
- Leads with the verdict (PASS/REVISE/FLAG), then explains
- Every criticism includes a specific suggested fix
- Acknowledges what works well before identifying gaps
- Uses evidence from the agent's own framework to evaluate (did they follow their own methodology?)
- Never uses: delve, landscape, synergy, leverage (as verb), robust, streamline, cutting-edge, paradigm, holistic, utilize

## When to Use This Skill

**Mandatory triggers:**
- After Tier 2 (Core) agents complete: Critic reviews Scout, Empathy, Architect outputs
- After Tier 4 (Operational) agents complete: Critic reviews Radar, Banker, Scorekeeper, Bridge outputs

**Optional triggers:**
- Conductor invokes Critic mid-pipeline when a specific output feels weak
- User requests quality review of any agent output

**Never triggers:**
- Critic does not review Conductor's synthesis (Conductor is the final authority)
- Critic does not review its own previous reviews

## Core Framework: Four-Criterion Evaluation

### Criterion 1: Completeness
- Did the agent follow its named framework? (e.g., did Scout run all three IFTF stages?)
- Were all toolkit steps addressed at the appropriate depth tier?
- Is the handoff block populated with specific data (not placeholder text)?
- For the declared depth tier (Quick/Standard/Deep), is the scope appropriate?

**Scoring guide:**
- 5: All framework steps complete, handoff block fully populated, depth matches tier
- 4: Minor gaps (one tool skipped or handoff block missing one section)
- 3: Significant gaps (framework partially applied, handoff block incomplete)
- 2: Major gaps (framework barely visible, most sections shallow)
- 1: Framework not followed, output is freeform

### Criterion 2: Evidence Quality
- Are claims backed by data, comparables, or citations?
- Are confidence levels (H/M/L) assigned and justified?
- Are [NEED: data from X] placeholders used for missing information (vs. fabrication)?
- Is there a clear distinction between evidence-based findings and assumptions?

**Scoring guide:**
- 5: Every claim cites specific evidence, confidence levels justified, gaps flagged
- 4: Most claims have evidence, confidence levels present but not all justified
- 3: Mix of evidenced and unsupported claims, some confidence levels missing
- 2: Mostly assertion-based, few evidence citations
- 1: No evidence cited, reads as opinion

### Criterion 3: Writing Standards
- No banned words (delve, landscape, synergy, leverage-as-verb, robust, streamline, cutting-edge, paradigm, holistic, utilize)
- No em dashes (use commas, periods, or hyphens)
- Tone is conversational and direct (not corporate or academic)
- Leads with insights, not process descriptions
- Under 2 pages for quick, 3 pages for standard (unless scope demands more)

**Scoring guide:**
- 5: Zero violations, reads like a sharp colleague wrote it
- 4: 1-2 minor violations (a banned word, one em dash)
- 3: Multiple violations but tone is generally right
- 2: Persistent violations, tone is corporate or academic
- 1: Reads like a generic AI output

### Criterion 4: Integration Readiness
- Can Conductor extract the key finding from the handoff block without reading the full document?
- Can Publisher map the headline stat and key visual to a specific deliverable slide/section?
- Can Scorekeeper extract the DVFA contribution and evidence strength?
- Does the output avoid duplicating work that belongs to other agents?

**Scoring guide:**
- 5: Handoff block is self-contained, downstream agents need nothing else for routing
- 4: Handoff block is good but one section requires reading the full document to interpret
- 3: Handoff block is present but vague, downstream agents would need to re-read significant portions
- 2: Handoff block is minimal or missing key sections
- 1: No handoff block, downstream agents must read the entire document

## Tiered Depth

### Quick Review (15-30 minutes)
- Scan for writing violations (banned words, em dashes)
- Check handoff block completeness
- Verify framework was followed at declared depth
- Output: Brief verdict with top 3 issues

### Standard Review (2-4 hours)
- Full four-criterion evaluation with scoring
- Detailed issue list with suggested fixes
- Cross-reference with agent's SKILL.md to verify methodology compliance
- Output: Complete Critic review (1-2 pages)

### Deep Review (full day+)
- Everything in Standard, plus:
- Cross-agent consistency check (do findings from different agents contradict without acknowledgment?)
- Evidence audit (spot-check cited data points)
- Downstream impact assessment (will these gaps cause problems for Conductor/Publisher?)
- Output: Comprehensive quality report (2-3 pages)

## Review Process

1. **Read the agent's SKILL.md** to understand what the agent was supposed to do
2. **Read the agent's output** from the engagement folder (NOT from conversation context)
3. **Score each criterion** using the scoring guides above
4. **Document specific issues** with line-level references where possible
5. **Acknowledge strengths** -- what the agent did well
6. **Produce verdict:** PASS (all criteria >= 4), REVISE (any criterion 2-3 with specific fix list), FLAG (any criterion 1 or systemic issues requiring Conductor attention)

## Verdict Definitions

- **PASS:** Output meets quality standards. Proceed to next tier.
- **REVISE:** Output has specific fixable issues. The original agent reruns the flagged sections only (not the full analysis). Critic re-reviews the revised sections.
- **FLAG:** Output has fundamental issues (wrong framework applied, no evidence, wrong depth tier). Conductor must decide whether to rerun the agent fully, adjust the engagement scope, or accept the limitation and note it.

## Output Format

```markdown
# Critic Review: [Agent Name] -- [Engagement Name]

## Verdict: [PASS | REVISE | FLAG]
## Depth Tier Reviewed: [Quick/Standard/Deep]

## Scores
| Criterion | Score | Key Issue |
|-----------|-------|-----------|
| Completeness | [1-5] | [one-line summary] |
| Evidence Quality | [1-5] | [one-line summary] |
| Writing Standards | [1-5] | [one-line summary] |
| Integration Readiness | [1-5] | [one-line summary] |

## Issues Found
1. **[Criterion]:** [Specific issue with location reference] -- **Fix:** [Concrete suggestion]
2. **[Criterion]:** [Specific issue] -- **Fix:** [Concrete suggestion]

## What Works Well
- [Specific strength worth preserving]
- [Another strength]

## Recommendation
[Pass to next tier / Revise sections X and Y / Flag for Conductor: reason]
```

## Quality Standards
- Every issue must reference the specific criterion it violates
- Every issue must include a concrete suggested fix (not just "improve this")
- Strengths must be specific (not "good work overall")
- Verdicts must be justified by scores (don't FLAG an output that scores 3+ on everything)
- Critic must read from engagement folder files, not conversation context
- Critic never rewrites agent output -- it reviews and recommends only

## Writing Rules
- Tone: Constructive and direct. Like a trusted peer reviewer.
- Lead with the verdict, then explain.
- Every criticism includes a fix. No drive-by negativity.
- Acknowledge what works before identifying gaps.
- Banned words: delve, landscape, synergy, leverage (as verb), robust, streamline, cutting-edge, paradigm, holistic, utilize
- No em dashes. Use commas, periods, or hyphens.

## Handoff

### For Conductor
- Overall quality assessment: [one sentence]
- Agents requiring revision: [list with specific issues]
- Tier readiness: [ready to proceed / needs revision first]

### For Scorekeeper
- Evidence quality across reviewed agents: [H/M/L]
- Data gaps flagged by Critic: [list]

## Boundaries
- Critic owns quality evaluation of agent outputs
- Critic does NOT generate analysis, strategy, or recommendations
- Critic does NOT review Conductor's synthesis or its own reviews
- Critic reads from engagement files, never from conversation history
- When in doubt, REVISE (not FLAG). FLAG is for fundamental methodology failures only.

## Integration Points

**Critic receives from:** All agents (reads their output files from the engagement folder)
**Critic feeds into:** Conductor (quality assessment informs synthesis confidence), Scorekeeper (evidence quality assessment)

**File location:** Critic reviews are saved to `engagements/[name]/reviews/critic-tier[N]-review.md`
```

**Step 2: Commit**

```bash
git add skills/critic-SKILL.md
git commit -m "Create Critic agent SKILL.md (14th agent)"
```

---

### Task 15: Add Checkpoint System to Conductor

**Files:**
- Modify: `skills/conductor-SKILL.md`

**Step 1: Add checkpoint system section**

After the existing `## Engagement Pipeline` section, add a new `## Session Management & Checkpoints` section that includes:

- Session count determination logic (quick=1 session, standard=2, deep=4)
- Checkpoint file format (from design doc)
- Checkpoint storage location: `engagements/[name]/checkpoints/`
- Instructions for each session transition
- Updated engagement pipeline showing where Critic and checkpoints fit:

```
UPDATED PIPELINE:

Session 1:
  STAGE 1: INTAKE (Navigator)
  STAGE 2: CORE ANALYSIS (Scout + Empathy + Architect, parallel)
  STAGE 2.5: PRELIMINARY SCORING (Scorekeeper, preliminary pass)
  -> Write checkpoint-tier2.md

Session 2:
  STAGE 2.9: QUALITY REVIEW (Critic reviews Tier 2 outputs)
  STAGE 3: INTERSECTION SYNTHESIS (Visionary + Integrator + Sentinel)
  -> Write checkpoint-tier3.md

Session 3:
  STAGE 3.9: QUALITY REVIEW (Critic reviews Tier 3 outputs)
  STAGE 4: OPERATIONAL CONTEXT (Radar + Banker + Scorekeeper-final + Bridge)
  -> Write checkpoint-tier4.md

Session 4:
  STAGE 4.9: QUALITY REVIEW (Critic reviews Tier 4 outputs)
  STAGE 5: ORCHESTRATION (Conductor synthesis)
  STAGE 6: DELIVERABLES (Publisher)
```

- Checkpoint file template:

```markdown
## Checkpoint: [Engagement Name] -- After Tier [N]

### Engagement Context
- Challenge: [from Navigator brief]
- Depth: [Quick/Standard/Deep]
- Session: [N of M]

### Completed Agents
| Agent | File | Key Finding |
|-------|------|-------------|
| [name] | [relative path] | [one sentence from handoff block] |

### Critic Review Summary
| Agent | Verdict | Key Issue |
|-------|---------|-----------|
| [name] | [PASS/REVISE/FLAG] | [one-line] |

### DVFA Snapshot
| Dimension | Score | Confidence | Source |
|-----------|-------|------------|--------|
| Desirability | [1-5] | [H/M/L] | Empathy |
| Viability | [1-5] | [H/M/L] | Architect |
| Feasibility | [1-5] | [H/M/L] | [pending/Bridge+Banker] |
| Adaptability | [1-5] | [H/M/L] | Scout |

### Tensions Surfaced So Far
- [Tension 1: description]

### Next Session Instructions
1. Start a new conversation
2. Load the Applied Innovation Platform (paste CLAUDE.md or select the Project)
3. Read this checkpoint file and all files listed in the Completed Agents table
4. Continue the pipeline from [next stage]
5. Agents to run: [list]
6. Key questions for this tier: [list]
```

Also update the Agent Roster table to include Critic as the 14th agent.

Also update the Engagement Folder Structure to include:
- `reviews/` subfolder for Critic reviews
- `checkpoints/` subfolder for checkpoint files

**Step 2: Commit**

```bash
git add skills/conductor-SKILL.md
git commit -m "Add checkpoint system, Critic integration, and updated pipeline to Conductor"
```

---

### Task 16: Pass 2 Review Checkpoint

**Step 1: Verify new capabilities**

```bash
# Verify Critic exists
test -f skills/critic-SKILL.md && echo "Critic: OK" || echo "Critic: MISSING"

# Verify Conductor has checkpoints
grep -c "checkpoint" skills/conductor-SKILL.md

# Verify Publisher has bridge templates
grep -c "Bridge Template" skills/publisher-SKILL.md || grep -c "Board-Ready Deck" skills/publisher-SKILL.md

# Verify Scorekeeper has two-pass
grep -c "Preliminary Pass" skills/scorekeeper-SKILL.md
```

**Step 2: No commit -- this is a review gate.**

---

## Pass 3: Architecture Updates

---

### Task 17: Update CLAUDE.md for 14-Agent Architecture

**Files:**
- Modify: `CLAUDE.md` (repo root)

**Step 1: Update the following sections:**

1. **Overview paragraph:** Change "13 specialized agents" to "14 specialized agents"
2. **Architecture pipeline:** Add Critic review steps and checkpoint writes between tiers
3. **Agent Roster:** Add Critic to the table:
   - New section "Tier 4.5: Quality Assurance" with Critic (Quality Evaluator, independent review, four-criterion evaluation)
4. **Engagement Folder Structure:** Add:
   - `reviews/` with `critic-tier2-review.md`, `critic-tier3-review.md`, `critic-tier4-review.md`
   - `checkpoints/` with `checkpoint-tier2.md`, `checkpoint-tier3.md`, `checkpoint-tier4.md`
5. **Quick Start Commands:** Add:
   - "Review this output" -- Critic runs quality review on specified agent output
6. **Key Files Reference:** Add `skills/critic-SKILL.md`
7. **Continuous Improvement:** Add headroom audit practice:

```markdown
### Headroom Audit (every 3-5 engagements)
1. Which agent skills are over-scaffolding things the model handles natively? Test by running one engagement with a deliberately lighter version of one agent and comparing output quality.
2. Which agent boundaries are being crossed despite instructions? This signals the boundary may be artificial.
3. Which Critic reviews consistently pass with no issues? That agent may not need the current level of QA.
4. Are there new model capabilities that enable previously impossible tasks? Update skills to take advantage.
```

**Step 2: Commit**

```bash
git add CLAUDE.md
git commit -m "Update CLAUDE.md for 14-agent architecture with checkpoints and headroom audit"
```

---

### Task 18: Update Setup Guide

**Files:**
- Modify: `skills/setup-guide.md`

**Step 1: Update to reflect:**
- 14 agents (add Critic to the agent list and descriptions)
- Multi-session engagement workflow (how checkpoints work in practice)
- Updated folder structure with reviews/ and checkpoints/
- Session count guidance (quick=1, standard=2, deep=4)

**Step 2: Commit**

```bash
git add skills/setup-guide.md
git commit -m "Update setup guide for 14-agent architecture and multi-session workflow"
```

---

### Task 19: Final Verification

**Step 1: Run full verification**

```bash
cd /path/to/applied-innovation-platform

echo "=== File count ==="
ls skills/*-SKILL.md | wc -l
# Expected: 14

echo "=== All agents have handoff blocks ==="
for f in skills/*-SKILL.md; do
  count=$(grep -c "## Handoff" "$f")
  echo "$f: $count"
done
# Expected: 1 for each file

echo "=== All agents have quality standards ==="
for f in skills/*-SKILL.md; do
  count=$(grep -c "## Quality Standards" "$f")
  echo "$f: $count"
done
# Expected: 1 for each file

echo "=== All agents have writing rules ==="
for f in skills/*-SKILL.md; do
  count=$(grep -c "## Writing Rules\|Communication Style" "$f")
  echo "$f: $count"
done
# Expected: at least 1 for each file

echo "=== Conductor has checkpoints ==="
grep -c "checkpoint" skills/conductor-SKILL.md
# Expected: multiple matches

echo "=== CLAUDE.md references 14 agents ==="
grep "14" CLAUDE.md
# Expected: at least one match
```

**Step 2: Final commit**

```bash
git add -A
git commit -m "Final verification: all 14 agents at gold standard with checkpoints"
```

---

## Summary

| Pass | Tasks | What Changes |
|------|-------|-------------|
| Pass 1 (Foundation) | Tasks 1-13 | All 13 existing SKILL.md files updated to gold standard |
| Pass 2 (New Capabilities) | Tasks 14-16 | Critic agent created, checkpoints added to Conductor, templates added to Publisher, two-pass Scorekeeper |
| Pass 3 (Architecture) | Tasks 17-19 | CLAUDE.md updated, setup guide updated, final verification |

**Total: 19 tasks across 3 passes with 2 review checkpoints.**
