# Implementation Plan: Distill Slime Report
**Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Rewrite the text after the gallery as three concise chapters. Retain the State anchor and a compact responsibility table; remove all code disclosures, procedural walkthroughs and oversized diagrams. Keep existing gallery markup and design tokens unchanged.

## Technical Context
- Markdown/HTML, existing Node static builder, GitHub Pages.
- No new dependencies or Unity changes.
- Tests: build, existing desktop/mobile suite with obsolete report assertions replaced, contents navigation, no overflow and one screenshot review batch.
- Scope: report source, generated HTML, its existing tests and planning documents.

## Constitution Check
Passed before and after planning. The constitution remains a template; AGENTS.md governs the workflow. Source findings from prior work establish factual accuracy. DESIGN.md and the incumbent reading surface remain authoritative.

## Project Structure
content/projects/slime-project.md -> project/slime-project/index.html; tests/slime-state-report.spec.js; specs/007-distill-slime-report/.

## Reading Brief
Communicate why the architecture exists and who owns each responsibility. Explain shared State, monster FSM, reusable behavior/data and player composition once. Use short paragraphs, one three-row responsibility table and a short transformation example. Remove source listings and method names. Preserve the gallery and typography.

## Complexity Tracking
No additional architecture or runtime changes.
