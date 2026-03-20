# Applied Innovation Platform - Setup & Getting Started Guide

## What This Is

A system of 13 AI agent skills orchestrated by a Conductor that together run multi-dimensional innovation analysis. Each agent has a distinct personality, framework, and output format. The Conductor orchestrates them into a repeatable engagement pipeline.

The whole thing runs inside Claude. No separate infrastructure needed. The skills are Markdown files that Claude reads and follows. The "agents" are Claude operating in different modes with different analytical frameworks.

## Quick Setup (10 minutes)

### Step 1: Clone or Download

```bash
git clone https://github.com/robertholzer42-stack/applied-innovation-platform.git
```

Or download the ZIP from GitHub and extract it.

### Step 2: Choose Your Claude Environment

This platform works in three Claude environments. Pick the one that fits your workflow.

**Option A: Claude Projects (Web/Desktop App)**
Best for interactive analysis, back-and-forth with individual agents, and iterative brainstorming.

1. Go to claude.ai and create a new Project
2. Name it "Applied Innovation Platform" (or whatever you prefer)
3. Upload all 13 SKILL.md files from the `skills/` folder as project knowledge
4. Copy the contents of `CLAUDE.md` into the project's custom instructions field
5. Start a conversation

**Option B: Claude Code (Terminal)**
Best for multi-step analysis, generating deliverables (docx, pptx), and automating the engagement pipeline.

1. Open your terminal in the repo directory
2. Claude Code will automatically read the `CLAUDE.md` file
3. Tell Claude to read the relevant skill files before running analysis:
```bash
claude "Read skills/conductor-SKILL.md and skills/scout-SKILL.md, then run a
standard-depth foresight analysis on [topic]. Time horizon: 5 years."
```

**Option C: Claude Cowork (Desktop)**
Best for document-heavy work, interactive sessions, and generating formatted deliverables.

1. Open Cowork and select the repo folder as your workspace
2. Claude will read the `CLAUDE.md` automatically
3. Start conversing with agents directly

### Step 3: Understand Where Outputs Go

All engagement work is saved to the `engagements/` folder at the root of this repo. Each engagement gets its own subfolder with a standard structure:

```
applied-innovation-platform/          <- repo root
  engagements/
    [challenge-name]/
      intake/                         - Navigator outputs
      analysis/                       - Scout, Empathy, Architect outputs
      synthesis/                      - Visionary, Integrator, Sentinel outputs
      operations/                     - Radar, Banker, Scorekeeper, Bridge outputs
      deliverables/                   - Publisher: decks, reports, one-pagers
```

**How this works in each environment:**

- **Claude Code / Cowork:** Claude creates the engagement folder and writes output files automatically when you run an analysis from the repo directory. Each agent writes to its designated subfolder using standard filenames (e.g., `scout-future-thinking.md`, `empathy-design-thinking.md`).

- **Claude Projects (Web/Desktop):** Claude cannot write to your local file system. It will produce each agent's output as a labeled message in conversation, prefixed with the target file path (e.g., `engagements/ai-strategy/analysis/scout-future-thinking.md`). You copy each output into the correct folder in your local clone.

You don't need to create the folder structure manually - Claude Code and Cowork will create it for you when starting a new engagement. For Claude Projects, create the folders locally and save outputs as you go.

## How to Use the Platform

### Option A: Full Engagement (Conductor-Led)

Start with: **"Run a full applied innovation analysis on [challenge]. Engagement name: [topic-context-version]."**

Name your engagement using the convention: `[topic]-[client-or-context]-[version]` (e.g., `ai-healthcare-acme-v1`, `ev-charging-strategy-v2`, `portfolio-review-q1-2026`).

The Conductor will:
1. Ask Navigator-style intake questions to scope the challenge
2. Build a shared research foundation (market data, key players, regulatory context)
3. Route to core agents (Scout, Empathy, Architect) running in parallel on the shared data
4. Run preliminary DVFA scoring to guide downstream agents
5. Run intersection agents with full upstream evidence (not summaries)
6. Run operational agents, then final DVFA scoring with delta tracking
7. Synthesize findings and surface conflicts (optional Devil's Advocate pass)
8. Produce board-ready deliverables using the platform's design system

### Option B: Single Agent Mode

Talk directly to one agent:

- "Scout, scan the horizon for trends in [domain] over the next 5 years"
- "Empathy, map the user journey for [persona] doing [task]"
- "Architect, map the system dependencies for [initiative]"
- "Sentinel, stress-test our strategy against these scenarios"
- "Banker, assess our innovation portfolio balance"
- "Scorekeeper, score this opportunity using DVFA"
- "Radar, what are our competitors doing in [space]?"
- "Bridge, assess adoption readiness for [change]"

### Option C: Quick Assessment

Start with: **"Quick scan on [topic]"**

The Conductor will select 2-3 agents, run their quick-tier toolkits (15-30 min), and give you a focused brief.

### Option D: Workshop Prep

Start with: **"Prepare a workshop on [topic] for [audience]"**

The Publisher will work with relevant agents to produce:
- Pre-work materials for participants
- Facilitation guide with exercises
- Participant handouts
- Scoring templates

## Running Your First Sample Engagement

Try this to see the full pipeline in action:

### Challenge: "AI Strategy for a Mid-Size Enterprise"

A good starter challenge because it touches all three thinking dimensions.

**Step 1: Navigator Intake**
```
Navigator, run intake for this challenge:
- Organization: Mid-size enterprise (~2,000 employees)
- Challenge: Want to deploy AI across operations but keep stalling after pilots
- Why now: Competitors are pulling ahead, leadership is asking questions
- What's been tried: A few chatbot pilots, one internal AI project that
  didn't make it to production
- Constraints: Budget pressure, limited AI talent, risk-averse culture
```

**Step 2: Research Foundation**
The Conductor builds a shared fact base (market data, key players, recent developments) so all agents work from the same information.

**Step 3: Core Agent Analysis** (run in parallel on the shared data)
- Scout: What's coming in enterprise AI over the next 3-5 years?
- Empathy: What do employees and customers actually need from AI?
- Architect: What are the system dependencies blocking AI at scale?

**Step 4: Preliminary DVFA Scoring**
Scorekeeper produces early scores to guide downstream agents. Flags dimensions with low confidence.

**Step 5: Intersection Synthesis** (receives full upstream evidence, not summaries)
- Visionary: What does the AI-enabled organization look like in 2030?
- Integrator: How do you get from pilot to production in this system?
- Sentinel: What happens if the competitive window closes?

**Step 6: Operational Context**
- Radar: What are competitors doing with AI?
- Banker: Where should AI investment be allocated across H1/H2/H3?
- Bridge: Is this organization ready for the change AI requires?
- Scorekeeper (final pass): Revised DVFA scores with delta tracking

**Step 7: Conductor Synthesis + Publisher Deliverables**
- Integrated strategy report with conflict resolution
- Board-ready deck using the platform's design system template
- Executive summary

Save everything in `engagements/ai-strategy-sample/` as your first reference engagement.

## Customization Guide

### Adding Industry Context

Each agent skill can be extended with industry-specific knowledge. For example, to specialize the Architect agent for healthcare:

1. Open `skills/architect-SKILL.md`
2. Add a section: "## Industry Context: Healthcare"
3. Include relevant regulations, system constraints, and common patterns
4. The agent will incorporate this context automatically

### Integrating Your Own Analytical Tools

The platform is designed to be extended. If you have existing frameworks:

- Add them as tools within the relevant agent's SKILL.md
- Follow the pattern: Tool name, description, inputs, process, output format
- Reference them in the Conductor's routing table if they change how challenges are assigned

### Adjusting DVFA Weights

By default, all four dimensions are weighted equally. To adjust:

1. Open `skills/scorekeeper-SKILL.md`
2. Modify the weighting section
3. Common adjustments: weight Adaptability higher for fast-changing industries, weight Feasibility higher for resource-constrained organizations

## Building the Knowledge Base

After every engagement:
1. Capture reusable patterns in `knowledge-base/patterns/`
2. Write the case study in `knowledge-base/case-studies/`
3. Note which agents provided the most value
4. Document what you'd do differently next time

## File Reference

| File | Purpose |
|------|---------|
| conductor-SKILL.md | Master orchestrator, engagement pipeline, routing |
| scout-SKILL.md | Future Thinking (IFTF Prepare-Foresight-Insight) |
| empathy-SKILL.md | Design Thinking (human-centered design process) |
| architect-SKILL.md | Systems Thinking (causal loops, consequence mapping) |
| visionary-SKILL.md | Future + Design intersection |
| integrator-SKILL.md | Design + Systems intersection |
| sentinel-SKILL.md | Future + Systems intersection |
| radar-SKILL.md | Competitive Intelligence |
| banker-SKILL.md | Innovation Portfolio Management |
| scorekeeper-SKILL.md | Metrics & DVFA Scoring |
| bridge-SKILL.md | Change Management |
| navigator-SKILL.md | Client Intake & Discovery |
| publisher-SKILL.md | Deliverable Generation |
| publisher-design-system.md | Visual identity spec: colors, typography, layout rules |
| setup-guide.md | This file |

**In the `docs/` folder:**

| File | Purpose |
|------|---------|
| publisher-deck-template.js | Reusable PptxGenJS template with design system and validation |
| setup-guide.md | Duplicate of skills/setup-guide.md for easy access |

## Tips for Getting the Best Results

**Use standard depth for most analyses.** Quick scans (2-3 agents) are good for initial exploration. Standard depth (6-8 agents, 2 sessions) hits the sweet spot for most real decisions. Deep dives (all 13, 3 sessions) are for high-stakes strategic choices.

**Let the Conductor build the research foundation.** For standard and deep engagements, the Conductor runs a research phase before launching Tier 2 agents. This prevents agents from doing duplicate web searches and ensures everyone works from the same facts.

**Watch the preliminary DVFA scores.** After Tier 2, the Scorekeeper produces early scores. If any dimension is below 2.0, the Conductor flags it for extra attention in the remaining stages. The final scores include a delta showing what changed and why.

**Use the "with Devil's Advocate" option for high-stakes decisions.** Add this to your engagement request and the Conductor will run an extra stress test after synthesis, identifying the single assumption that would invalidate the most recommendations.

**Connect Canva for polished decks.** If you're running in Cowork and have a Canva account, connecting the Canva MCP gives Publisher access to professional templates. Without Canva, Publisher uses a built-in PptxGenJS template (`docs/publisher-deck-template.js`) that produces clean, consistent decks.

**Name engagements consistently.** Use the format `[topic]-[client-or-context]-[version]`. Examples: `ai-healthcare-acme-v1`, `ev-charging-strategy-v2`, `portfolio-review-q1-2026`. This keeps the `engagements/` folder organized as you run more analyses.

## Troubleshooting

**"Claude doesn't seem to know about the agents"**
Make sure all 13 SKILL.md files are uploaded as project knowledge (in Claude Projects) or that you've told Claude to read them (in Claude Code/Cowork).

**"The analysis is too generic"**
Add industry-specific context to the relevant agent skills. The more specific the context, the better the output.

**"The agents keep using banned words"**
Reinforce the writing rules. Say: "Remember the writing rules from CLAUDE.md - no banned words."

**"I want deeper analysis but it's taking too long"**
Use the tiered toolkit. Start with Quick Scan to identify what matters, then go Standard or Deep only on the dimensions that need it.
