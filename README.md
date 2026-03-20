# Applied Innovation Platform

An open-source, AI-powered innovation analysis system that applies Future Thinking, Design Thinking, and Systems Thinking together through 13 specialized agents. Inspired by my 8+ years leading Innovation teams, IFTF's frameworks and Braden Kelley's Resilient Innovation framework and designed to run inside Claude (Anthropic).

## The Problem

Most organizations have a federated or disconnected model to drive innovation, don't connect innovation back to strategic goals and often don't think about the various innovation skills needed to be successful. They run design thinking workshops but ignore the systems their ideas have to survive in. They do signal monitoring and scenario planning but never ask whether anyone wants what they're building. One-dimensional innovation fails.

## The Solution

This platform orchestrates 13 AI agents, each with a distinct personality, analytical framework, and output format, into a repeatable engagement pipeline. Each agent is a Markdown skill file that Claude reads and follows. No infrastructure, no API keys, no code to deploy. Just structured prompts that turn Claude into a multi-dimensional innovation practice.

**What it covers:**
- Product and service innovation
- Business model disruption analysis
- Process and operational redesign
- Strategic foresight and scenario planning
- Innovation portfolio management
- Change readiness assessment

## Prerequisites

- A Claude Pro, Team, or Enterprise subscription (claude.ai)
- OR Claude Code / Claude Cowork (desktop)
- Basic familiarity with Claude Projects

## Quick Start (10 minutes)

**1. Clone this repo**
```bash
git clone https://github.com/robertholzer42-stack/applied-innovation-platform.git
```

**2. Create a Claude Project**

In Claude (web or desktop app), create a new Project. Upload all 13 files from the `skills/` folder as project knowledge.

**3. Add the project instructions**

Copy the contents of `CLAUDE.md` into your project's custom instructions field.

**4. Run your first analysis**

```
Run a full applied innovation analysis on [your challenge here]
```

That's it. The Conductor agent will route your challenge to the right agents and produce an integrated strategy.

See the [Setup Guide](skills/setup-guide.md) for detailed instructions, alternative deployment options (Claude Code, Cowork), and a walkthrough of your first engagement.

## Agent Architecture

### Tier 1: Client Interface
| Codename | Role |
|----------|------|
| **Navigator** | Challenge discovery, innovation maturity assessment, engagement scoping |
| **Publisher** | Board-ready decks, strategy reports, workshop guides |

### Tier 2: Core Thinking
| Codename | Framework |
|----------|-----------|
| **Scout** | Future Thinking - IFTF-aligned Prepare-Foresight-Insight cycle |
| **Empathy** | Design Thinking - human-centered design process |
| **Architect** | Systems Thinking - causal loops, leverage points, consequence mapping |

### Tier 3: Intersection
| Codename | Intersection |
|----------|-------------|
| **Visionary** | Future + Design - future personas, speculative design, scenario prototyping |
| **Integrator** | Design + Systems - scalable solutions, pilot-to-production pathways |
| **Sentinel** | Future + Systems - resilience mapping, stress testing, adaptive strategy |

### Tier 4: Operational
| Codename | Role |
|----------|------|
| **Radar** | Competitive intelligence, market monitoring, positioning analysis |
| **Banker** | Innovation portfolio management, Horizon 1/2/3 balancing |
| **Scorekeeper** | DVFA scoring (Desirability, Viability, Feasibility, Adaptability) |
| **Bridge** | Change management, adoption readiness, transition design |

### Tier 5: Orchestration
| Codename | Role |
|----------|------|
| **Conductor** | Cross-agent synthesis, conflict resolution, unified strategy |

## How It Works

The Conductor manages an 8-stage pipeline:

1. **Intake** (Navigator) - Scope the challenge, assess innovation maturity
2. **Research Foundation** (Conductor) - Shared fact base so all agents work from the same data
3. **Core Analysis** (Scout + Empathy + Architect) - Three independent analytical lenses, run in parallel
4. **Preliminary Scoring** (Scorekeeper) - Early DVFA scores to guide downstream agents
5. **Intersection Synthesis** (Visionary + Integrator + Sentinel) - Where the lenses overlap
6. **Operational Context** (Radar + Banker + Bridge, then Scorekeeper final) - Real-world grounding + final DVFA with delta tracking
7. **Orchestration** (Conductor) - Cross-agent synthesis, conflict resolution
8. **Deliverables** (Publisher) - Board-ready artifacts with reusable design system

You can run the full pipeline, use individual agents directly, or ask for a quick 2-3 agent assessment.

## DVFA Scoring Framework

Every innovation opportunity is scored across four dimensions:

- **D - Desirability** (from Design Thinking): Do people want this?
- **V - Viability** (from Systems Thinking): Can this work in real-world systems?
- **F - Feasibility** (from operational reality): Can we build and deliver this?
- **A - Adaptability** (from Future Thinking): Will this survive across multiple futures?

Each dimension scored 1-5 with confidence levels. The result: an Innovation Score that goes beyond the typical desirability-viability-feasibility triangle.

## Repository Structure

```
skills/              - 13 agent SKILL.md files + design system + setup guide
docs/                - Example deliverables, architecture diagram, PPTX template
engagements/         - Your engagement workspaces (one folder per challenge)
knowledge-base/      - Reusable patterns, case studies, frameworks
  patterns/
  case-studies/
  frameworks/
CLAUDE.md            - Project instructions (paste into your Claude Project)
```

### Key Reference Files

| File | Purpose |
|------|---------|
| `docs/publisher-deck-template.js` | Reusable PptxGenJS template with design system, slide builders, and validation functions |
| `skills/publisher-design-system.md` | Visual identity spec: colors, typography, layout rules, QA checklist |
| `knowledge-base/patterns/TEMPLATE.md` | Template for capturing reusable patterns from engagements |
| `knowledge-base/case-studies/TEMPLATE.md` | Template for documenting completed engagements |

## Customization

This platform is designed to be forked and adapted. Common customizations:

- **Add industry-specific context** to agent skills (e.g., add healthcare regulations to Architect)
- **Adjust DVFA weights** for your organization's priorities
- **Create new agents** following the SKILL.md pattern
- **Integrate your own tools** - the Architect agent supports pluggable analytical frameworks
- **Modify writing rules** in CLAUDE.md to match your organization's voice

## Methodology Foundations

- **Future Thinking:** Based on the Institute for the Future's Prepare-Foresight-Insight cycle, with 50+ years of proven foresight methodology
- **Design Thinking:** Human-centered design with empathy research, personas, journey mapping, and prototyping briefs
- **Systems Thinking:** Causal loop analysis, leverage point identification, and consequence mapping

The innovation at the core of this platform is that these three disciplines are applied together, systematically, every time. The intersection agents (Visionary, Integrator, Sentinel) exist specifically to find insights that no single lens would reveal.

## Contributing

Contributions welcome. Some ideas:

- New agent skills for additional analytical dimensions
- Industry-specific templates and context
- Example engagements that others can learn from
- Improvements to existing agent frameworks
- Documentation and tutorials

Please open an issue to discuss significant changes before submitting a PR.

## Credits

Created by Bob Holzer. Inspired by IFTF and Braden Kelley's Resilient Innovation framework, which articulates how Future Thinking, Design Thinking, and Systems Thinking intersect to produce innovations that are desirable, viable, and resilient.

## License

Apache License 2.0 - See [LICENSE](LICENSE) for details.
