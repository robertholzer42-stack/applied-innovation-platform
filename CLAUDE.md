# CLAUDE.md - Project Memory for Resilient Innovation Platform

## Project Overview

**Resilient Innovation Platform** is an AI-powered multi-dimensional innovation practice. 13 specialized agents orchestrated by a Conductor to deliver Future Thinking + Design Thinking + Systems Thinking analysis for enterprise clients. Built by Bob at Next Horizon Innovations.

**GitHub**: `robertholzer42-stack/resilient-innovation-platform` (private)
**Owner**: Bob Holzer (bob@nexthorizoninnovations.com)
**Based on**: Braden Kelley's Resilient Innovation framework (bradenkelley.com/2026/03/resilient-innovation/)

## Architecture

```
Engagement pipeline (sequential, Conductor-orchestrated):

  Navigator (intake, scoping, maturity assessment)
    -> Scout + Empathy + Architect (parallel core analysis)
      -> Visionary + Integrator + Sentinel (intersection synthesis)
        -> Radar + Banker + Scorekeeper + Bridge (operational context)
          -> Conductor (cross-agent synthesis, conflict resolution, unified strategy)
            -> Publisher (board-ready deliverables)
```

Each agent is a SKILL.md file in `skills/` with four components: personality profile, analytical framework, input/output schema, and trigger patterns. The Conductor routes challenges to the right agents based on the challenge type.

## Agent Roster (13 agents, 5 tiers)

### Tier 1: Client Interface
| Codename | Role | Key Capability |
|----------|------|---------------|
| **Navigator** | Client Intake | Challenge discovery, innovation maturity assessment, engagement scoping |
| **Publisher** | Deliverables | Board-ready decks, strategy reports, workshop guides, case studies |

### Tier 2: Core Thinking
| Codename | Role | Primary Framework |
|----------|------|------------------|
| **Scout** | Future Thinking | IFTF Prepare-Foresight-Insight cycle, tiered toolkit (quick/standard/deep) |
| **Empathy** | Design Thinking | Human-centered design: empathize, define, ideate, prototype |
| **Architect** | Systems Thinking | Draw Out Consequences (full DOC SKILL.md), causal loops, leverage points |

### Tier 3: Intersection
| Codename | Intersection | Key Capability |
|----------|-------------|---------------|
| **Visionary** | Future + Design | Future personas, speculative design, scenario-based prototyping |
| **Integrator** | Design + Systems | Scalable solutions, pilot-to-production, adoption pathway design |
| **Sentinel** | Future + Systems | Resilience mapping, assumption stress testing, adaptive strategy |

### Tier 4: Operational
| Codename | Role | Key Capability |
|----------|------|---------------|
| **Radar** | Competitive Intel | Market monitoring, patent scanning, positioning maps |
| **Banker** | Portfolio Mgmt | Three Horizons (H1/H2/H3), pipeline health, resource allocation |
| **Scorekeeper** | Metrics & Scoring | DVFA scoring, innovation maturity assessment, KPI design |
| **Bridge** | Change Mgmt | Adoption readiness, stakeholder resistance, transition design |

### Tier 5: Orchestration
| Codename | Role | Key Capability |
|----------|------|---------------|
| **Conductor** | Orchestrator | Cross-agent synthesis, conflict resolution, action planning |

## Critical Patterns and Conventions

### Agent Naming
- Always "Empathy" (not "Empath") for the Design Thinking agent
- Always use codenames in conversation: Scout, Empathy, Architect, etc.
- When operating as an agent, announce which agent you are

### IFTF Foresight Alignment (Scout)
Scout uses IFTF's Prepare-Foresight-Insight cycle. The **Action** stage belongs to the Conductor, not Scout.

Foresight tools are tiered by depth:
- **Quick scan** (15-30 min): Orient to the Future, 5-10 signals, 2-3 drivers, brief summary
- **Standard** (2-4 hrs): Full Prepare + signal scan + STEEP drivers + forecasts + cross-impacts
- **Deep dive** (full day+): Complete cycle + scenarios + DOC analysis + Ride Two Curves (if applicable)

Ride Two Curves is ONE tool in Scout's Insight stage, used only for S-curve/disruption analysis. It is NOT the default framework.

### DVFA Scoring Framework (Scorekeeper)
Every innovation opportunity is scored on four dimensions:
- **D** - Desirability (from Empathy): Do people want this?
- **V** - Viability (from Architect): Can this work in real-world systems?
- **F** - Feasibility (from operational reality): Can we build/deliver this?
- **A** - Adaptability (from Scout/Sentinel): Will this survive multiple futures?

Each scored 1-5 with confidence level (H/M/L). Overall Resilience Score = weighted average.

### Challenge-to-Agent Routing (Conductor)
| Challenge Type | Primary Agents | Supporting |
|---------------|----------------|------------|
| New product/service | Empathy, Scout, Visionary | Radar, Scorekeeper |
| Business model disruption | Scout, Architect, Integrator | Banker, Bridge |
| Process redesign | Architect, Empathy, Sentinel | Scorekeeper, Bridge |
| Strategic foresight | Scout, Sentinel, Architect | Radar, Banker |
| Innovation portfolio review | Banker, Scorekeeper, Scout | All as needed |
| Market entry | Radar, Scout, Empathy | Integrator, Scorekeeper |
| Technology adoption | Architect, Bridge, Empathy | Scout, Sentinel |
| Org transformation | Bridge, Architect, Empathy | Banker, Scorekeeper |

### Integration with Existing NHI Assets
| Asset | Integrated Into |
|-------|----------------|
| Draw Out Consequences SKILL.md (`Claude Code/Skils/`) | Architect agent's primary framework |
| Ride Two Curves app (`Claude Code/ride-two-curves-app/`) | Scout agent's S-curve tool (Insight stage only) |
| SynthData Sandbox pipeline architecture | Engineering pattern for sequential agent orchestration |
| HCSC Tech Radar methodology | Scout's horizon scanning approach |
| HCSC Innovation Pipeline management | Banker's portfolio management model |
| HCSC AI Governance frameworks | Architect's policy assessment lens |

## Workspace Structure

```
skills/              - 13 agent SKILL.md files + orchestrator + setup guide
docs/                - Executive overview, Venn diagram, architecture blueprint
engagements/         - Client engagement workspaces (one folder per client)
  [client-name]/
    intake/          - Navigator outputs: engagement brief, maturity assessment
    analysis/        - Core agent outputs: scout, empathy, architect
    synthesis/       - Intersection outputs: visionary, integrator, sentinel
    operations/      - Operational outputs: radar, banker, scorekeeper, bridge
    deliverables/    - Publisher outputs: decks, reports, one-pagers
knowledge-base/
  patterns/          - Reusable patterns captured from engagements
  case-studies/      - Documented engagement case studies
  frameworks/        - Reference frameworks and templates
```

## How I Work
- Plan before building. Use Plan Mode for anything non-trivial.
- If an approach goes sideways, STOP and re-plan.
- Start simple. Prefer minimal viable approaches.
- Ask before large changes. Confirm scope before modifying multiple files.
- IMPORTANT: Verify before claiming done. Never mark a task complete without proving it works.
- Always ask clarifying questions before generating substantial content.
- Flag missing information with `[NEED: data from X]` instead of guessing.

## Writing Rules
- Tone: Conversational and direct. Write like a smart colleague, not a corporate deck.
- Lead with the insight or finding, not the process.
- Every claim needs evidence: numbers, comparables, or citations.
- Keep documents under 2 pages unless I say otherwise.
- IMPORTANT: Banned words - delve, landscape, synergy, leverage (as verb), robust, streamline, cutting-edge, paradigm, holistic, utilize
- No em dashes. Use commas, periods, or hyphens.

## Sub-Agent Reviewer Roles
When I say "review as [role]", shift to that perspective. These work alongside the innovation agents:

| Role | Perspective |
|------|-------------|
| Engineer | Technical feasibility, architecture risks, build vs. buy, scalability |
| Designer | UX clarity, user journey, interaction coherence, accessibility |
| Executive | Strategic alignment, ROI, market positioning, board-readiness |
| Skeptic | Challenge assumptions, surface hidden risks, stress-test logic |
| Customer | Value proposition, pain point fit, willingness to pay, switching cost |
| Data Analyst | Metrics validity, statistical rigor, data gaps, measurement bias |

Additionally, "review as [agent codename]" triggers that specific innovation agent's perspective.

## Quick Start Commands

- "Run a full resilient innovation analysis on [topic]" - Complete 6-stage pipeline
- "Quick scan on [topic]" - Conductor picks 2-3 agents, quick tier
- "Scout, scan the horizon for [domain]" - Single agent, foresight
- "Empathy, map the user journey for [persona]" - Single agent, design
- "Architect, map the system for [initiative]" - Single agent, systems
- "Scorekeeper, score this opportunity" - DVFA scoring
- "Assess our innovation maturity" - Navigator maturity assessment
- "Generate deliverables for [engagement]" - Publisher produces artifacts

## Document Creation Pipeline
1. **Research** - Gather context, search workspace and web
2. **Draft** - Create using appropriate skill (read SKILL.md first!)
3. **Review** - Use sub-agent roles to stress-test from multiple perspectives
4. **Polish** - Apply formatting, add visuals
5. **Export** - Save to engagement deliverables folder

## Continuous Improvement
After every engagement:
1. Capture reusable patterns in `knowledge-base/patterns/`
2. Write the case study in `knowledge-base/case-studies/`
3. Note which agents provided the most value
4. Document what you'd do differently
5. Commit and push changes

## Key Files Reference

| File | Purpose |
|------|---------|
| `skills/conductor-SKILL.md` | Master orchestrator, routing, pipeline |
| `skills/scout-SKILL.md` | Future Thinking (IFTF-aligned) |
| `skills/empathy-SKILL.md` | Design Thinking (human-centered design) |
| `skills/architect-SKILL.md` | Systems Thinking (DOC framework) |
| `skills/visionary-SKILL.md` | Future + Design intersection |
| `skills/integrator-SKILL.md` | Design + Systems intersection |
| `skills/sentinel-SKILL.md` | Future + Systems intersection |
| `skills/radar-SKILL.md` | Competitive Intelligence |
| `skills/banker-SKILL.md` | Innovation Portfolio Management |
| `skills/scorekeeper-SKILL.md` | Metrics & DVFA Scoring |
| `skills/bridge-SKILL.md` | Change Management |
| `skills/navigator-SKILL.md` | Client Intake & Discovery |
| `skills/publisher-SKILL.md` | Deliverable Generation |
| `skills/setup-guide.md` | Complete setup and usage instructions |
| `docs/NHI_Resilient_Innovation_Executive_Overview.html` | Interview/pitch one-pager |
| `docs/NHI_Agent_Architecture_Venn_Diagram.html` | Interactive agent architecture visual |
| `docs/NHI_Resilient_Innovation_Architecture_Blueprint.docx` | Full architecture document |
