# Implementation Plan: Remove Reward Engineering Chapter

**Branch**: Current working branch | **Date**: 2026-10-19 | **Spec**: [spec.md](spec.md)

## Summary
Remove the complete reward engineering chapter from the Markdown report, delete its unused local CSS, renumber chapter 4 to 3, and regenerate the static report and table of contents.

## Technical Context
**Language/Version**: Markdown, HTML, JavaScript, Node.js >=18.
**Primary Dependencies**: Existing static-site builder and Playwright; no additions.
**Storage**: Repository content and generated HTML.
**Testing**: npm run build, npm test, targeted desktop/mobile browser checks.
**Target Platform**: GitHub Pages desktop and mobile browsers.
**Project Type**: Static portfolio site.
**Performance Goals**: Preserve existing reading and gallery performance.
**Constraints**: Preserve DESIGN.md styling, other content and interactions.
**Scale/Scope**: One project report and its generated output.

## Constitution Check
Passed before and after planning. The constitution is an unfilled template; AGENTS.md provides the active workflow. Requirements and plan precede content edits. No dependency or visual-system changes.

## Project Structure
- content/projects/autonomous-racing-agent.md: source of truth.
- project/autonomous-racing-agent/index.html: generated report with automatic contents list.
- tests/racing-gallery.spec.js: existing regression coverage.
- specs/004-remove-reward-chapter/: specification and implementation/validation documents.

## Design Review
This is a bounded deletion in an existing reading surface. Preserve typography, colors, gallery and all remaining layout. Remove duplicate separators and keep heading hierarchy consecutive. UI design skills apply only as preservation checks; no replacement visual system is needed.

## Complexity Tracking
No violations or additional architecture.
