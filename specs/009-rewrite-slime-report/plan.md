# Implementation Plan: Rewrite Slime Project Report

**Branch**: `main` (no branch change) | **Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Keep the project overview and state management chapter, with chapter 3 and transformation coverage absent. Preserve the section 2.4 comparison and existing player initialization excerpt. Add an adjacent monster disclosure showing selected BearMonster.Start() state constructors and stateMachine.Init(moveState) from the actual game source. Retain the gallery, styling and five existing excerpts for six total disclosures. Files only, without commit or push.

## Technical Context
**Language/Version**: Markdown/HTML; Node >=18 static builder.
**Primary Dependencies**: Existing builder and Playwright; no additions.
**Storage**: Repository content and generated HTML.
**Testing**: npm run build; npm test on desktop Chromium and Pixel 5.
**Target Platform**: Existing static portfolio, desktop and mobile browsers.
**Project Type**: Static site editorial update.
**Performance Goals**: No runtime additions; main prose <=1,600 characters.
**Constraints**: Preserve factual scope, optional retained code, gallery, chapter 1/2 anchors and styling; remove chapter 3 anchors; no game edits or deployment recovery.
**Scale/Scope**: One report source and its generated page.

## Constitution Check
Pass before research and after design. The constitution remains a template; AGENTS.md governs. Follow DESIGN.md by retaining the existing presentation. No new UI, dependency, animation or security change. Recovery automation was paused by explicit request.

## Project Structure
- content/projects/slime-project.md: revised prose.
- project/slime-project/index.html: generated page.
- tests/slime-state-report.spec.js: existing regression coverage, retained.
- specs/009-rewrite-slime-report/: requirements and plan.

## Implementation
1. Update the intro/overview to focus on monster and player behavior management.
2. Replace chapter 3 with section 2.4 and a compact comparison table; move the existing player composition excerpt there. Remove the transformation text and rabbit code.
3. Build and inspect the diff for remaining excerpt/gallery preservation and chapter removal.
4. Run the existing complete browser suite and inspect desktop/mobile report screenshots.
5. Deliver the rewritten local artifact and verification results. Leave edits uncommitted; do not push, deploy or resume recovery.

## Chapter 2 Comparison

- Compare behavior units, execution control and extension for monsters and players in section 2.4.
- Keep the lifecycle role table in section 2.2; target each table explicitly in existing tests.
- Add a source-backed BearMonster.Start() excerpt before the player excerpt: idle, move, dash and dead state creation, plus the initial move state. Clearly label it as selected initialization code.
- Update the existing checks for two chapters, six disclosures, chapter 3 removal, the comparison anchor and mobile overflow. Verify the two section 2.4 controls can stay open together. Build and run the complete desktop/mobile suite.

## Complexity Tracking
No new runtime behavior or visual design. Existing browser assertions cover the unchanged interaction contract.
