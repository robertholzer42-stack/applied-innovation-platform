# NHI Resilient Innovation Platform - Setup & Getting Started Guide

## What This Is

This is a system of 11 AI agent skills + 1 orchestrator that together run multi-dimensional innovation analysis. Each agent has a distinct personality, framework, and output format. The Conductor orchestrates them into a repeatable engagement pipeline.

The whole thing runs inside Claude Code and Claude Cowork. No separate infrastructure needed. The skills are text files that Claude reads and follows. The "agents" are Claude operating in different modes with different frameworks.

## Quick Setup (15 minutes)

### Step 1: Copy the Skills

Copy the entire `skills/` folder into your NHI workspace. The recommended location:

```
Next Horizon Innovations/
  Resilient Innovation/
    skills/
      conductor-SKILL.md
      scout-SKILL.md
      empathy-SKILL.md
      architect-SKILL.md
      visionary-SKILL.md
      integrator-SKILL.md
      sentinel-SKILL.md
      radar-SKILL.md
      banker-SKILL.md
      scorekeeper-SKILL.md
      bridge-SKILL.md
      navigator-SKILL.md
      publisher-SKILL.md
    engagements/
      (created per client)
    knowledge-base/
      patterns/
      case-studies/
      frameworks/
    docs/
      setup-guide.md (this file)
```

### Step 2: Create the Claude Project

In Claude (web or app), create a new Project called "Resilient Innovation Platform."

**Project Instructions (paste into the project's custom instructions):**

```
You are operating as the NHI Resilient Innovation Platform, an AI-powered innovation practice built on Braden Kelley's Resilient Innovation framework. The platform uses 11 specialized agents, each with a distinct personality and analytical framework, orchestrated by the Conductor.

When the user asks for innovation analysis, follow the engagement pipeline defined in conductor-SKILL.md. Route challenges to the appropriate agents based on the routing table. Each agent has its own SKILL.md file with personality, framework, and output format.

Key operating rules:
- Always identify which agent(s) you're operating as
- Use the agent's voice and personality when responding
- Follow the agent's specific framework and output format  
- Flag conflicts between agents openly
- Use [NEED: data from X] for missing information, never fabricate
- Follow Bob's writing rules: conversational, direct, no banned words, no em dashes

Available agents: Scout, Empathy, Architect, Visionary, Integrator, Sentinel, Radar, Banker, Scorekeeper, Bridge, Navigator, Publisher, Conductor

The user's existing foresight tools (Draw Out Consequences, Ride Two Curves) are integrated. The Architect agent uses DOC as its primary analytical tool. The Scout agent uses R2C only when S-curve disruption analysis is specifically needed.
```

### Step 3: Add Skills as Project Knowledge

Upload all 13 SKILL.md files to the project as knowledge files. Claude will reference them automatically when you invoke agents.

### Step 4: Set Up Your First Engagement Folder

For each client or internal project, create:

```
engagements/[name]/
  intake/
  analysis/
  synthesis/
  operations/
  deliverables/
```

## How to Use the Platform

### Option A: Full Engagement (Conductor-Led)

Start with: "Run a full resilient innovation analysis on [challenge]"

The Conductor will:
1. Ask Navigator-style intake questions to scope the challenge
2. Route to the appropriate core agents (Scout, Empathy, Architect)
3. Run intersection agents where relevant
4. Synthesize findings and surface conflicts
5. Produce integrated strategy with DVFA scoring

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

Start with: "Quick scan on [topic]"

The Conductor will select 2-3 agents, run their quick-tier toolkits (15-30 min), and give you a focused brief.

### Option D: Workshop Prep

Start with: "Prepare a workshop on [topic] for [audience]"

The Publisher will work with relevant agents to produce:
- Pre-work materials for participants
- Facilitation guide with exercises
- Participant handouts
- Scoring templates

## Using in Claude Code (Engineering Mode)

Claude Code is best for:
- Building and testing new skills
- Running multi-step analysis with sub-agents
- Generating complex deliverables (docx, pptx)
- Automating the engagement pipeline

**Typical Claude Code workflow:**

```bash
# Start a new engagement
claude "Read the conductor-SKILL.md and navigator-SKILL.md, then run 
intake for a new client: [client name], [challenge description]"

# Run individual agents
claude "Read scout-SKILL.md and run a standard-depth foresight analysis 
on [topic]. Time horizon: 5 years."

# Generate deliverables
claude "Read publisher-SKILL.md and the analysis files in 
engagements/[client]/analysis/, then create an executive summary deck."
```

## Using in Claude Cowork (Desktop/Consulting Mode)

Cowork is best for:
- Interactive client analysis sessions
- Iterative brainstorming with specific agents
- Document review and refinement
- Quick questions to individual agents

**Typical Cowork workflow:**

1. Open the Resilient Innovation project
2. Start a conversation: "I'm working on [client challenge]. Let's start with Scout."
3. Have a back-and-forth conversation with Scout's personality and framework
4. When done, ask Scout to produce its output in the standard format
5. Switch to another agent: "Now let's hear from Empathy on this same challenge"
6. After all agents have weighed in: "Conductor, synthesize these perspectives"

## Running Your First Sample Engagement

To build your case study portfolio, run this exercise:

### Challenge: "AI Adoption in Healthcare Payer Organizations"

This one plays to your strengths. You lived this at HCSC for 21 years.

**Step 1: Navigator Intake**
```
Navigator, run intake for this challenge:
- Client: Mid-size health plan (~5M members)
- Challenge: They want to deploy AI across claims, member services, 
  and care management but keep stalling after pilots
- Why now: Competitors are pulling ahead, board is asking questions
- What's been tried: A few chatbot pilots, one claims AI project that 
  died in production
- Constraints: HIPAA, state regulatory variation, risk-averse culture, 
  limited AI talent
```

**Step 2: Core Agent Analysis**
- Scout: What's coming in healthcare AI over the next 3-5 years?
- Empathy: What do claims processors, nurses, and members actually need?
- Architect: What are the system dependencies blocking AI at scale?

**Step 3: Intersection Synthesis**
- Visionary: What does the AI-enabled payer experience look like in 2030?
- Integrator: How do you get from pilot to production in this system?
- Sentinel: What happens if regulation tightens? If a competitor leaps ahead?

**Step 4: Operational Context**
- Radar: What are competing payers doing with AI?
- Banker: Where should AI investment be allocated across H1/H2/H3?
- Scorekeeper: Score the top 3 AI use cases using DVFA
- Bridge: Is this organization ready for the change AI requires?

**Step 5: Conductor Synthesis + Publisher Deliverables**
- Integrated strategy report
- Executive summary deck
- Implementation roadmap

Save everything in `engagements/healthcare-ai-payer/` as your first case study.

## Tips for Client Engagements

### The Sales Pitch
"I run your strategic challenges through a multi-dimensional innovation engine that combines three disciplines most organizations only use one at a time: Future Thinking, Design Thinking, and Systems Thinking. Each perspective is powered by a specialized AI agent with its own analytical framework. The result is innovation strategy that's not just creative, but resilient across multiple possible futures."

### The Live Demo
In a sales meeting, offer to run a quick scan:
1. Ask: "What's one trend or challenge that keeps you up at night?"
2. Run Scout (quick tier) live in the meeting
3. Show them 5-10 signals and 2-3 forecasts in 15 minutes
4. Close with: "That's one of eleven agents. Imagine what the full analysis looks like."

### Pricing Guidance
- Quick Sprint (1-2 weeks, 2-3 agents): $15K-$25K
- Standard Engagement (4-6 weeks, full Tier 2 + intersections): $30K-$50K
- Deep Engagement (8-12 weeks, all agents): $50K-$100K
- Monthly Advisory Retainer: $5K-$10K
- Workshop (single day): $5K-$15K

### Building the Knowledge Base
After every engagement:
1. Capture reusable patterns in `knowledge-base/patterns/`
2. Write the case study in `knowledge-base/case-studies/`
3. Note which agents provided the most value
4. Document what you'd do differently next time

## For Job Interviews

When interviewing for innovation leadership roles:

### What to Show
- The executive one-pager (shows strategic thinking + technical execution)
- A completed sample engagement (shows the framework in action)
- The architecture diagram (shows systems-level design capability)
- The DVFA scoring framework (shows you can measure innovation, not just talk about it)

### What to Say
"I built an AI-powered innovation practice that operationalizes the intersection of Future Thinking, Design Thinking, and Systems Thinking. I designed the agent architecture, wrote the analytical frameworks, built the engagement pipeline, and ran client engagements through it. Here's a sample output."

"Most organizations innovate through one lens. I built a system that forces multi-dimensional thinking every time, so innovations are desirable, viable, AND resilient across multiple futures."

### Questions to Be Ready For
- "How is this different from just using ChatGPT?" -- The agents have specific frameworks, personalities, and output formats. ChatGPT gives generic answers. This system applies 50+ years of IFTF foresight methodology, formal design thinking process, and systems dynamics analysis in a structured, repeatable way.
- "Does it replace human judgment?" -- No. It augments it. I bring 21 years of enterprise innovation experience to interpret and apply what the agents produce. The combination is the value.
- "Show me a real output" -- Have the healthcare payer case study ready.

## File Reference

| File | Purpose |
|------|---------|
| conductor-SKILL.md | Master orchestrator, engagement pipeline, routing |
| scout-SKILL.md | Future Thinking (IFTF Prepare-Foresight-Insight) |
| empathy-SKILL.md | Design Thinking (human-centered design process) |
| architect-SKILL.md | Systems Thinking (DOC framework, causal loops) |
| visionary-SKILL.md | Future + Design intersection |
| integrator-SKILL.md | Design + Systems intersection |
| sentinel-SKILL.md | Future + Systems intersection |
| radar-SKILL.md | Competitive Intelligence |
| banker-SKILL.md | Innovation Portfolio Management |
| scorekeeper-SKILL.md | Metrics & DVFA Scoring |
| bridge-SKILL.md | Change Management |
| navigator-SKILL.md | Client Intake & Discovery |
| publisher-SKILL.md | Deliverable Generation |
| setup-guide.md | This file |

## Existing NHI Assets That Integrate

| Asset | Where It Plugs In |
|-------|-------------------|
| Draw Out Consequences SKILL.md | Architect agent's primary tool |
| Ride Two Curves app | Scout agent's S-curve tool (when applicable) |
| SynthData Sandbox patterns | Engineering blueprint for pipeline orchestration |
| HCSC Tech Radar experience | Scout's horizon scanning methodology |
| HCSC Innovation Pipeline | Banker's portfolio management model |
| HCSC AI Governance | Architect's policy assessment lens |
| docx/pptx/pdf skills | Publisher's deliverable production |
