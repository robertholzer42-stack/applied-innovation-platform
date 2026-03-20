/**
 * Applied Innovation Platform - Board-Ready Deck Template
 *
 * Reusable PptxGenJS template that implements the platform's design system.
 * Publisher should READ this file and adapt it rather than building from scratch.
 *
 * Design system:
 *   Primary:   #1B3A5C (navy)     - headers, titles, dark backgrounds
 *   Accent:    #0D8A8A (teal)     - highlights, callouts, data points
 *   Coral:     #D85A30            - warnings, tensions, risk items
 *   Purple:    #534AB7            - intersection insights
 *   Green:     #1D9E75            - operational items, positive indicators
 *   Light BG:  #F0F6FA            - content area backgrounds
 *   White:     #FFFFFF            - cards, text backgrounds
 *
 * Typography:
 *   Titles:    Georgia, 28-36pt, bold
 *   Subtitles: Calibri, 18-20pt
 *   Body:      Calibri, 12-14pt
 *   Callout #: Georgia, 48-60pt, bold (for big stat numbers)
 *
 * Layout rules:
 *   - LAYOUT_16x9 (10" x 5.625")
 *   - 0.5" margins on all sides
 *   - Content area: 9" wide x 4.625" tall
 *   - Cards: minimum 2.2" wide, 0.3" internal padding
 *   - Accent bar: 0.12" wide left-edge bar in tier color
 *   - Title maximum: 40 characters at 28pt for single-line rendering
 *   - Max 6 bullets per slide, each under 12 words
 *
 * IMPORTANT PptxGenJS rules (from pptxgenjs.md):
 *   - NEVER use "#" prefix on hex colors
 *   - NEVER encode opacity in hex strings (use opacity property)
 *   - NEVER reuse option objects across calls (PptxGenJS mutates them)
 *   - Use bullet: true, not unicode "•"
 *   - Use breakLine: true between text array items
 *   - Use RECTANGLE not ROUNDED_RECTANGLE when pairing with accent bars
 */

const pptxgen = require("pptxgenjs");

// ============================================================
// DESIGN TOKENS - Change these to rebrand the entire deck
// ============================================================
const COLORS = {
  navy:       "1B3A5C",
  teal:       "0D8A8A",
  coral:      "D85A30",
  purple:     "534AB7",
  green:      "1D9E75",
  lightBg:    "F0F6FA",
  white:      "FFFFFF",
  darkText:   "1B3A5C",
  bodyText:   "333333",
  mutedText:  "6B7280",
  cardBorder: "E5E7EB",
};

const FONTS = {
  title:    "Georgia",
  body:     "Calibri",
};

const LAYOUT = {
  marginX:    0.5,
  marginY:    0.5,
  contentW:   9.0,
  contentH:   4.625,
  slideW:     10.0,
  slideH:     5.625,
};

// ============================================================
// HELPER: Fresh shadow object (never reuse across calls)
// ============================================================
function cardShadow() {
  return { type: "outer", color: "000000", blur: 4, offset: 1, angle: 135, opacity: 0.08 };
}

// ============================================================
// HELPER: Add a card with left accent bar
// ============================================================
function addCard(slide, { x, y, w, h, accentColor, title, body, titleSize = 14, bodySize = 11 }) {
  // Card background
  slide.addShape("rect", {
    x, y, w, h,
    fill: { color: COLORS.white },
    shadow: cardShadow(),
  });
  // Accent bar
  slide.addShape("rect", {
    x, y, w: 0.12, h,
    fill: { color: accentColor || COLORS.teal },
  });
  // Title
  if (title) {
    slide.addText(title, {
      x: x + 0.3, y: y + 0.15, w: w - 0.5, h: 0.35,
      fontFace: FONTS.body, fontSize: titleSize, bold: true,
      color: COLORS.darkText, margin: 0,
    });
  }
  // Body
  if (body) {
    slide.addText(body, {
      x: x + 0.3, y: y + 0.5, w: w - 0.5, h: h - 0.65,
      fontFace: FONTS.body, fontSize: bodySize,
      color: COLORS.bodyText, margin: 0, valign: "top",
    });
  }
}

// ============================================================
// HELPER: Big stat callout
// ============================================================
function addBigStat(slide, { x, y, w, number, label, color }) {
  slide.addText(String(number), {
    x, y, w, h: 0.8,
    fontFace: FONTS.title, fontSize: 48, bold: true,
    color: color || COLORS.teal, align: "center", margin: 0,
  });
  slide.addText(label, {
    x, y: y + 0.75, w, h: 0.35,
    fontFace: FONTS.body, fontSize: 12,
    color: COLORS.mutedText, align: "center", margin: 0,
  });
}

// ============================================================
// HELPER: Section header bar (thin teal line + section title)
// ============================================================
function addSectionLabel(slide, { x, y, w, text }) {
  slide.addShape("rect", {
    x, y, w, h: 0.03,
    fill: { color: COLORS.teal },
  });
  slide.addText(text.toUpperCase(), {
    x, y: y + 0.08, w, h: 0.25,
    fontFace: FONTS.body, fontSize: 9, bold: true,
    color: COLORS.mutedText, charSpacing: 3, margin: 0,
  });
}

// ============================================================
// SLIDE 1: Title Slide
// ============================================================
function addTitleSlide(pres, { title, subtitle, date }) {
  const slide = pres.addSlide();
  slide.background = { color: COLORS.navy };

  slide.addText(title, {
    x: 0.8, y: 1.5, w: 8.4, h: 1.2,
    fontFace: FONTS.title, fontSize: 36, bold: true,
    color: COLORS.white, margin: 0,
  });
  slide.addText(subtitle || "Applied Innovation Analysis", {
    x: 0.8, y: 2.7, w: 8.4, h: 0.5,
    fontFace: FONTS.body, fontSize: 18,
    color: COLORS.teal, margin: 0,
  });
  slide.addText(date || new Date().toISOString().split("T")[0], {
    x: 0.8, y: 3.5, w: 8.4, h: 0.35,
    fontFace: FONTS.body, fontSize: 12,
    color: COLORS.mutedText, margin: 0,
  });
  // Bottom accent line
  slide.addShape("rect", {
    x: 0.8, y: 4.5, w: 2.0, h: 0.04,
    fill: { color: COLORS.teal },
  });
  return slide;
}

// ============================================================
// SLIDE 2: The Verdict
// ============================================================
function addVerdictSlide(pres, { verdict, overallScore }) {
  const slide = pres.addSlide();
  slide.background = { color: COLORS.white };

  addSectionLabel(slide, { x: 0.5, y: 0.4, w: 9.0, text: "The Verdict" });

  slide.addText(verdict, {
    x: 0.5, y: 0.9, w: 6.5, h: 1.5,
    fontFace: FONTS.title, fontSize: 22, bold: true,
    color: COLORS.darkText, margin: 0, valign: "top",
  });

  addBigStat(slide, {
    x: 7.2, y: 1.0, w: 2.3,
    number: overallScore, label: "Overall DVFA Score",
    color: COLORS.teal,
  });

  return slide;
}

// ============================================================
// SLIDE 3: DVFA Scorecard (4 cards)
// ============================================================
function addDVFASlide(pres, { scores }) {
  // scores = [{ dimension, score, confidence, evidence }, ...]
  const slide = pres.addSlide();
  slide.background = { color: COLORS.lightBg };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: "DVFA Scorecard" });

  const dimensionColors = {
    "Desirability": COLORS.teal,
    "Viability":    COLORS.navy,
    "Feasibility":  COLORS.green,
    "Adaptability": COLORS.purple,
  };

  const cardW = 2.05;
  const cardH = 2.8;
  const startX = 0.5;
  const gap = 0.3;
  const startY = 0.9;

  scores.forEach((s, i) => {
    const x = startX + i * (cardW + gap);
    const accent = dimensionColors[s.dimension] || COLORS.teal;

    // Card background
    slide.addShape("rect", {
      x, y: startY, w: cardW, h: cardH,
      fill: { color: COLORS.white },
      shadow: cardShadow(),
    });
    // Accent bar
    slide.addShape("rect", {
      x, y: startY, w: 0.12, h: cardH,
      fill: { color: accent },
    });
    // Dimension name
    slide.addText(s.dimension, {
      x: x + 0.25, y: startY + 0.15, w: cardW - 0.4, h: 0.3,
      fontFace: FONTS.body, fontSize: 11, bold: true,
      color: COLORS.darkText, margin: 0,
    });
    // Score
    slide.addText(String(s.score), {
      x: x + 0.25, y: startY + 0.5, w: cardW - 0.4, h: 0.7,
      fontFace: FONTS.title, fontSize: 36, bold: true,
      color: accent, margin: 0,
    });
    // Confidence
    slide.addText(`Confidence: ${s.confidence}`, {
      x: x + 0.25, y: startY + 1.2, w: cardW - 0.4, h: 0.25,
      fontFace: FONTS.body, fontSize: 9,
      color: COLORS.mutedText, margin: 0,
    });
    // Evidence
    slide.addText(s.evidence || "", {
      x: x + 0.25, y: startY + 1.5, w: cardW - 0.4, h: cardH - 1.7,
      fontFace: FONTS.body, fontSize: 9,
      color: COLORS.bodyText, margin: 0, valign: "top",
    });
  });

  return slide;
}

// ============================================================
// SLIDE 4-6: Key Finding (two-column: finding + fix)
// ============================================================
function addKeyFindingSlide(pres, { number, finding, fix, agent, accentColor }) {
  const slide = pres.addSlide();
  slide.background = { color: COLORS.white };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: `Key Finding #${number}` });

  // Source agent tag
  slide.addText(`Source: ${agent}`, {
    x: 0.5, y: 0.65, w: 3.0, h: 0.25,
    fontFace: FONTS.body, fontSize: 9, italic: true,
    color: COLORS.mutedText, margin: 0,
  });

  // Finding card (left)
  addCard(slide, {
    x: 0.5, y: 1.1, w: 4.2, h: 3.0,
    accentColor: accentColor || COLORS.navy,
    title: "THE FINDING",
    body: finding,
  });

  // Fix card (right)
  addCard(slide, {
    x: 5.0, y: 1.1, w: 4.5, h: 3.0,
    accentColor: COLORS.green,
    title: "THE FIX",
    body: fix,
  });

  return slide;
}

// ============================================================
// SLIDE 7: Competitive Context
// ============================================================
function addCompetitiveSlide(pres, { competitors }) {
  // competitors = [{ name, position, threat }, ...]
  const slide = pres.addSlide();
  slide.background = { color: COLORS.lightBg };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: "Competitive Context" });

  const cardW = 2.8;
  const startX = 0.5;
  const gap = 0.3;

  competitors.slice(0, 3).forEach((c, i) => {
    addCard(slide, {
      x: startX + i * (cardW + gap), y: 0.9, w: cardW, h: 3.2,
      accentColor: COLORS.coral,
      title: c.name,
      body: `Position: ${c.position}\n\nThreat Level: ${c.threat}`,
    });
  });

  return slide;
}

// ============================================================
// SLIDE 8: Portfolio / Budget (big number + bar)
// ============================================================
function addPortfolioSlide(pres, { h1Pct, h2Pct, h3Pct, totalBudget, recommendation }) {
  const slide = pres.addSlide();
  slide.background = { color: COLORS.white };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: "Portfolio Allocation" });

  if (totalBudget) {
    addBigStat(slide, {
      x: 0.5, y: 0.8, w: 3.0,
      number: totalBudget, label: "Recommended Investment",
      color: COLORS.navy,
    });
  }

  // Horizontal stacked bar
  const barY = 2.5;
  const barH = 0.6;
  const barW = 9.0;
  const h1W = barW * (h1Pct / 100);
  const h2W = barW * (h2Pct / 100);
  const h3W = barW * (h3Pct / 100);

  slide.addShape("rect", { x: 0.5, y: barY, w: h1W, h: barH, fill: { color: COLORS.navy } });
  slide.addShape("rect", { x: 0.5 + h1W, y: barY, w: h2W, h: barH, fill: { color: COLORS.teal } });
  slide.addShape("rect", { x: 0.5 + h1W + h2W, y: barY, w: h3W, h: barH, fill: { color: COLORS.purple } });

  // Labels
  slide.addText(`H1: ${h1Pct}%`, { x: 0.5, y: barY + barH + 0.1, w: 3, h: 0.3, fontFace: FONTS.body, fontSize: 10, color: COLORS.navy, margin: 0 });
  slide.addText(`H2: ${h2Pct}%`, { x: 3.5, y: barY + barH + 0.1, w: 3, h: 0.3, fontFace: FONTS.body, fontSize: 10, color: COLORS.teal, margin: 0 });
  slide.addText(`H3: ${h3Pct}%`, { x: 6.5, y: barY + barH + 0.1, w: 3, h: 0.3, fontFace: FONTS.body, fontSize: 10, color: COLORS.purple, margin: 0 });

  if (recommendation) {
    slide.addText(recommendation, {
      x: 0.5, y: 3.8, w: 9.0, h: 0.8,
      fontFace: FONTS.body, fontSize: 12, color: COLORS.bodyText, margin: 0, valign: "top",
    });
  }

  return slide;
}

// ============================================================
// SLIDE 9: Phase 1 Actions (3 cards)
// ============================================================
function addActionsSlide(pres, { actions }) {
  // actions = [{ action, owner, timeline }, ...]
  const slide = pres.addSlide();
  slide.background = { color: COLORS.lightBg };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: "Immediate Actions" });

  const cardW = 2.8;
  const gap = 0.3;

  actions.slice(0, 3).forEach((a, i) => {
    addCard(slide, {
      x: 0.5 + i * (cardW + gap), y: 0.9, w: cardW, h: 3.2,
      accentColor: COLORS.green,
      title: a.action,
      body: `Owner: ${a.owner}\nTimeline: ${a.timeline}`,
    });
  });

  return slide;
}

// ============================================================
// SLIDE 10: Monitoring (3 time horizon columns)
// ============================================================
function addMonitoringSlide(pres, { day30, day90, month12 }) {
  const slide = pres.addSlide();
  slide.background = { color: COLORS.white };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: "What We're Watching" });

  const cols = [
    { label: "30 Days", items: day30, color: COLORS.green },
    { label: "90 Days", items: day90, color: COLORS.teal },
    { label: "12 Months", items: month12, color: COLORS.purple },
  ];

  cols.forEach((col, i) => {
    const x = 0.5 + i * 3.1;
    slide.addText(col.label, {
      x, y: 0.9, w: 2.8, h: 0.35,
      fontFace: FONTS.body, fontSize: 14, bold: true,
      color: col.color, margin: 0,
    });
    const bullets = col.items.map((item, j) => ({
      text: item,
      options: { bullet: true, breakLine: j < col.items.length - 1, fontSize: 11, color: COLORS.bodyText },
    }));
    slide.addText(bullets, {
      x, y: 1.35, w: 2.8, h: 3.0,
      fontFace: FONTS.body, margin: 0, valign: "top",
    });
  });

  return slide;
}

// ============================================================
// SLIDE 11: The Ask
// ============================================================
function addAskSlide(pres, { items }) {
  // items = [{ text, deadline }, ...]
  const slide = pres.addSlide();
  slide.background = { color: COLORS.white };

  addSectionLabel(slide, { x: 0.5, y: 0.3, w: 9.0, text: "The Ask" });

  const bullets = items.map((item, i) => ({
    text: `${item.text}${item.deadline ? " - " + item.deadline : ""}`,
    options: { bullet: { type: "number" }, breakLine: i < items.length - 1, fontSize: 14, bold: true, color: COLORS.darkText },
  }));

  slide.addText(bullets, {
    x: 0.5, y: 0.9, w: 9.0, h: 3.5,
    fontFace: FONTS.body, margin: 0, valign: "top",
  });

  return slide;
}

// ============================================================
// SLIDE 12: Close
// ============================================================
function addCloseSlide(pres, { keyStat, statLabel, contactInfo }) {
  const slide = pres.addSlide();
  slide.background = { color: COLORS.navy };

  addBigStat(slide, {
    x: 2.5, y: 1.2, w: 5.0,
    number: keyStat, label: statLabel,
    color: COLORS.teal,
  });

  slide.addText("Applied Innovation Platform", {
    x: 2.5, y: 3.0, w: 5.0, h: 0.4,
    fontFace: FONTS.title, fontSize: 18,
    color: COLORS.white, align: "center", margin: 0,
  });

  if (contactInfo) {
    slide.addText(contactInfo, {
      x: 2.5, y: 3.6, w: 5.0, h: 0.35,
      fontFace: FONTS.body, fontSize: 12,
      color: COLORS.mutedText, align: "center", margin: 0,
    });
  }

  return slide;
}

// ============================================================
// EXPORT: Build a complete deck from engagement data
// ============================================================
async function buildDeck(data, outputPath) {
  const pres = new pptxgen();
  pres.layout = "LAYOUT_16x9";
  pres.author = "Applied Innovation Platform";
  pres.title = data.title || "Applied Innovation Analysis";

  addTitleSlide(pres, {
    title: data.title,
    subtitle: data.subtitle,
    date: data.date,
  });

  addVerdictSlide(pres, {
    verdict: data.verdict,
    overallScore: data.overallScore,
  });

  addDVFASlide(pres, { scores: data.dvfaScores });

  // Key findings (up to 3)
  (data.keyFindings || []).slice(0, 3).forEach((f, i) => {
    addKeyFindingSlide(pres, { number: i + 1, ...f });
  });

  if (data.competitors) {
    addCompetitiveSlide(pres, { competitors: data.competitors });
  }

  if (data.portfolio) {
    addPortfolioSlide(pres, data.portfolio);
  }

  if (data.actions) {
    addActionsSlide(pres, { actions: data.actions });
  }

  if (data.monitoring) {
    addMonitoringSlide(pres, data.monitoring);
  }

  if (data.asks) {
    addAskSlide(pres, { items: data.asks });
  }

  addCloseSlide(pres, {
    keyStat: data.overallScore,
    statLabel: "Overall DVFA Score",
    contactInfo: data.contactInfo,
  });

  await pres.writeFile({ fileName: outputPath });
  return outputPath;
}

// ============================================================
// USAGE EXAMPLE (for Publisher reference)
// ============================================================
/*
const data = {
  title: "AI in Healthcare",
  subtitle: "Applied Innovation Analysis - Standard Depth",
  date: "2026-03-19",
  verdict: "Healthcare AI is real but fragile. Liability, not regulation, is the binding constraint.",
  overallScore: "3.1",
  dvfaScores: [
    { dimension: "Desirability", score: "3.8", confidence: "High", evidence: "Clinician burnout creates strong pull" },
    { dimension: "Viability", score: "2.4", confidence: "Medium", evidence: "Liability framework undefined" },
    { dimension: "Feasibility", score: "3.0", confidence: "Medium", evidence: "Capital available, talent scarce" },
    { dimension: "Adaptability", score: "3.2", confidence: "High", evidence: "Strong across 3 of 4 scenarios" },
  ],
  keyFindings: [
    { finding: "Liability, not regulation...", fix: "Governance-first strategy...", agent: "Architect + Sentinel", accentColor: "1B3A5C" },
    { finding: "AI Validation-as-a-Service...", fix: "Build validation platform...", agent: "Visionary", accentColor: "534AB7" },
    { finding: "Portfolio is backwards...", fix: "Rebalance to 40/40/20...", agent: "Banker + Bridge", accentColor: "1D9E75" },
  ],
  competitors: [
    { name: "Epic Systems", position: "Incumbent EHR", threat: "High - owns the workflow" },
    { name: "Google Health", position: "AI-first entrant", threat: "Medium - regulatory challenges" },
    { name: "Startups", position: "Point solutions", threat: "Low individually, high collectively" },
  ],
  portfolio: { h1Pct: 40, h2Pct: 40, h3Pct: 20, totalBudget: "$2.5M", recommendation: "Rebalance from current 70/20/10" },
  actions: [
    { action: "Establish AI governance board", owner: "CTO + Legal", timeline: "30 days" },
    { action: "Launch clinician co-design program", owner: "Product", timeline: "60 days" },
    { action: "Begin validation framework pilot", owner: "Engineering", timeline: "90 days" },
  ],
  monitoring: {
    day30: ["Governance board formed", "Legal review of liability"],
    day90: ["Co-design cohort recruited", "Validation pilot live"],
    month12: ["First FDA submission", "Clinician adoption > 30%"],
  },
  asks: [
    { text: "Approve governance board charter", deadline: "Week 2" },
    { text: "Allocate $500K for validation pilot", deadline: "Week 4" },
    { text: "Assign clinical liaison", deadline: "Week 1" },
  ],
  contactInfo: "your-email@example.com",
};

buildDeck(data, "Applied_Innovation_Deck.pptx");
*/

// ============================================================
// VALIDATION: Pre-flight checks before writing the file
// ============================================================

// Font height reference (approximate, in inches)
const FONT_HEIGHTS = {
  48: 0.70, 40: 0.60, 36: 0.55, 28: 0.45, 24: 0.40,
  18: 0.30, 14: 0.25, 12: 0.22, 11: 0.20, 10: 0.18, 9: 0.16,
};

function estimateFontHeight(fontSize) {
  return FONT_HEIGHTS[fontSize] || (fontSize * 0.015);
}

/**
 * Validate that text fits inside its container.
 * Call after building each slide to catch overflow before writing.
 *
 * @param {string} label - Slide identifier for warning messages
 * @param {object} container - { y, h } of the parent shape
 * @param {object} text - { y, fontSize } of the text element
 * @param {number} safetyMargin - minimum gap from container bottom (default 0.15")
 */
function validateBoundingBox(label, container, text, safetyMargin = 0.15) {
  const containerBottom = container.y + container.h;
  const textBottom = text.y + estimateFontHeight(text.fontSize);
  const gap = containerBottom - textBottom;
  if (gap < safetyMargin) {
    console.warn(`[QA] ${label}: Text (${text.fontSize}pt at y=${text.y}) overflows container (bottom=${containerBottom.toFixed(2)}). Gap=${gap.toFixed(2)}", need >=${safetyMargin}"`);
    return false;
  }
  return true;
}

/**
 * Validate bullet count and word length per slide.
 *
 * @param {Array<string>} bullets - Array of bullet text strings
 * @param {string} slideLabel - Slide identifier for warning messages
 * @param {number} maxBullets - Maximum bullets allowed (default 6)
 * @param {number} maxWords - Maximum words per bullet (default 12)
 */
function validateBullets(bullets, slideLabel, maxBullets = 6, maxWords = 12) {
  const warnings = [];
  if (bullets.length > maxBullets) {
    warnings.push(`${slideLabel}: ${bullets.length} bullets (max ${maxBullets})`);
  }
  bullets.forEach((b, i) => {
    const words = b.trim().split(/\s+/).length;
    if (words > maxWords) {
      warnings.push(`${slideLabel}, bullet ${i + 1}: ${words} words (max ${maxWords})`);
    }
  });
  if (warnings.length > 0) {
    warnings.forEach(w => console.warn(`[QA] ${w}`));
    return false;
  }
  return true;
}

/**
 * Validate title length won't wrap at the given font size.
 *
 * @param {string} title - Title text
 * @param {number} fontSize - Font size in pt
 * @param {string} slideLabel - Slide identifier for warning messages
 */
function validateTitleLength(title, fontSize, slideLabel) {
  const maxChars = fontSize >= 36 ? 30 : fontSize >= 28 ? 40 : 60;
  if (title.length > maxChars) {
    console.warn(`[QA] ${slideLabel}: Title "${title.slice(0, 30)}..." is ${title.length} chars (max ${maxChars} at ${fontSize}pt)`);
    return false;
  }
  return true;
}

// ============================================================
// STRUCTURAL COLOR RULES (use these, not agent colors, for
// recurring design elements)
// ============================================================
const STRUCTURAL_COLORS = {
  theFix:         COLORS.coral,     // "The Fix" / recommendation callouts
  sectionHeader:  COLORS.navy,      // Section headers and labels
  confidenceHigh: COLORS.green,     // High confidence
  confidenceMed:  "D4A030",         // Medium confidence (amber)
  confidenceLow:  "C0392B",         // Low confidence (red)
  footer:         COLORS.navy,      // Footer bars
  agentAttrib:    COLORS.mutedText, // Agent attribution (small, muted)
};

module.exports = {
  // Slide builders
  buildDeck, addTitleSlide, addVerdictSlide, addDVFASlide, addKeyFindingSlide,
  addCompetitiveSlide, addPortfolioSlide, addActionsSlide, addMonitoringSlide,
  addAskSlide, addCloseSlide,
  // Helpers
  addCard, addBigStat, addSectionLabel, cardShadow,
  // Validation
  validateBoundingBox, validateBullets, validateTitleLength, estimateFontHeight,
  // Design tokens
  COLORS, FONTS, LAYOUT, STRUCTURAL_COLORS,
};
