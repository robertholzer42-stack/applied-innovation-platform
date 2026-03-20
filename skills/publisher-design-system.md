# Publisher Design System Reference

Quick-reference for the Applied Innovation Platform's visual identity. All values are codified in `docs/publisher-deck-template.js` as constants. This file is the human-readable spec.

## Color Palette

### Primary Colors
| Name | Hex | Usage |
|------|-----|-------|
| Navy | #1B3A5C | Headers, titles, dark backgrounds, footer bars |
| Teal | #0D8A8A | Highlights, callouts, data point emphasis, accent lines |

### Secondary Accents
| Name | Hex | Usage |
|------|-----|-------|
| Coral | #D85A30 | Warnings, tensions, risk items, "The Fix" callouts |
| Purple | #534AB7 | Intersection insights (Visionary, Integrator, Sentinel) |
| Green | #1D9E75 | Operational items, positive indicators, actions |

### Backgrounds and Text
| Name | Hex | Usage |
|------|-----|-------|
| Light BG | #F0F6FA | Content area backgrounds (alternating slides) |
| White | #FFFFFF | Cards, text containers |
| Dark text | #1B3A5C | Primary text on light backgrounds |
| Body text | #333333 | Body copy |
| Muted text | #6B7280 | Labels, dates, attribution, secondary info |
| Card border | #E5E7EB | Card edges (if visible) |

### Structural vs. Agent Colors

Recurring design elements use FIXED colors regardless of source agent:

| Element | Color | Rule |
|---------|-------|------|
| "The Fix" / recommendation boxes | Coral (#D85A30) | Always coral, never agent-specific |
| Section headers / labels | Navy (#1B3A5C) | Always navy |
| Confidence: High | Green (#1D9E75) | |
| Confidence: Medium | Amber (#D4A030) | |
| Confidence: Low | Red (#C0392B) | |
| Footer bars | Navy (#1B3A5C) | Always navy |
| Agent attribution | Muted (#6B7280) | Small text, footer only |

Agent-specific colors (teal for Scout, purple for Visionary, etc.) are ONLY used in data visualizations and DVFA dimension cards where the agent identity is the content.

## Typography

| Element | Font | Size | Weight |
|---------|------|------|--------|
| Slide titles (dark bg) | Georgia | 32-36pt | Bold |
| Slide titles (light bg) | Georgia | 24-28pt | Bold |
| Subtitles | Calibri | 18-20pt | Regular |
| Body text | Calibri | 12-14pt | Regular |
| Card titles | Calibri | 14pt | Bold |
| Card body | Calibri | 11pt | Regular |
| Big stat numbers | Georgia | 48-60pt | Bold |
| Stat labels | Calibri | 12pt | Regular |
| Section label text | Calibri | 9pt | Bold, UPPERCASE, charSpacing: 3 |
| Muted labels | Calibri | 9-10pt | Regular |

### Title Character Limits (single-line guarantee)
| Font Size | Max Characters |
|-----------|---------------|
| 36pt | 30 characters |
| 28pt | 40 characters |
| 24pt | 50 characters |

## Layout (16:9)

| Dimension | Value |
|-----------|-------|
| Slide size | 10" x 5.625" |
| Margins | 0.5" all sides |
| Content area | 9" wide x 4.625" tall |
| Card minimum width | 2.2" |
| Card internal padding | 0.3" |
| Card gap (in grid) | 0.3" |
| Accent bar width | 0.12" |
| DVFA mini-indicator spacing | 2.2" minimum between items |

## Bounding Box Safety Rules

PptxGenJS does not clip, warn, or error on text overflow. These rules prevent visual bugs.

### Font Height Reference
| Font Size (pt) | Approximate Height (inches) |
|---------------|-----------------------------|
| 48 | 0.70" |
| 40 | 0.60" |
| 36 | 0.55" |
| 28 | 0.45" |
| 24 | 0.40" |
| 18 | 0.30" |
| 14 | 0.25" |
| 12 | 0.22" |
| 11 | 0.20" |
| 9 | 0.16" |

### Placement Rules
- Text must sit at least 0.15" inside any container's bottom edge
- Never place text >24pt in the bottom 20% of a container
- Calculate: if shape bottom = y + h, then text bottom (y + font_height) must be <= shape bottom - 0.15"

## Data Visualization Rules

### Bar Charts
- Partial-fill bars: always use dark text (navy or black) that reads against both bar AND background
- Never use white text on bars that don't span full width
- Place percentage labels above or below the bar if contrast is uncertain
- Stacked bars: labels outside the segments

### DVFA Scorecards
- Each dimension gets its own card with colored accent bar
- Desirability: teal accent
- Viability: navy accent
- Feasibility: green accent
- Adaptability: purple accent

## QA Checklist

Before delivering any .pptx, verify every slide against:

- [ ] Title renders on a single line
- [ ] No text overflows its container
- [ ] Text is readable against every background it touches
- [ ] No dead zones (>1" empty space)
- [ ] Structural elements use consistent colors (not agent colors)
- [ ] Max 6 bullets per slide, each under 12 words
- [ ] Every number/claim traces to a specific agent
- [ ] Slide uses at least 75% of vertical space
