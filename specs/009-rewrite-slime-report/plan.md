# Implementation Plan: Rewrite Slime Project Report

**Branch**: `main` (no branch change) | **Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Rewrite the introduction and all explanatory paragraphs of the existing Slime report with a more direct problem, design and practical effect narrative. Explain contribution, transition order, common lifecycle, data-driven monster reuse and player composition. Preserve headings, all five source excerpts, gallery and styles. This revision is files-only, with no commit or push.

## Technical Context
**Language/Version**: Markdown/HTML; Node >=18 static builder.
**Primary Dependencies**: Existing builder and Playwright; no additions.
**Storage**: Repository content and generated HTML.
**Testing**: npm run build; npm test on desktop Chromium and Pixel 5.
**Target Platform**: Existing static portfolio, desktop and mobile browsers.
**Project Type**: Static site editorial update.
**Performance Goals**: No runtime additions; main prose <=1,600 characters.
**Constraints**: Preserve factual scope, optional code, gallery, headings and styling; no game edits or deployment recovery.
**Scale/Scope**: One report source and its generated page.

## Constitution Check
Pass before research and after design. The constitution remains a template; AGENTS.md governs. Follow DESIGN.md by retaining the existing presentation. No new UI, dependency, animation or security change. Recovery automation was paused by explicit request.

## Project Structure
- content/projects/slime-project.md: revised prose.
- project/slime-project/index.html: generated page.
- tests/slime-state-report.spec.js: existing regression coverage, retained.
- specs/009-rewrite-slime-report/: requirements and plan.

## Implementation
1. Rewrite prose around concrete responsibilities and design decisions, retaining factual code relationships.
2. Keep the tested sentence that distinguishes monster FSM and player controller, along with existing anchors and evidence blocks.
3. Build and inspect the diff for excerpt/gallery preservation.
4. Run the existing complete browser suite and inspect desktop/mobile report screenshots.
5. Deliver the rewritten local artifact and verification results. Leave edits uncommitted; do not push, deploy or resume recovery.

## Complexity Tracking
No new runtime behavior or visual design. Existing browser assertions cover the unchanged interaction contract.
