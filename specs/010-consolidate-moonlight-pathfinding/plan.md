# Implementation Plan: Consolidate MoonLight Pathfinding

**Branch**: `main` (unchanged) | **Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Remove the source from chapter 4 onward. Add section 3.4 explaining the customer/staff relationship to shared A* search through two short paragraphs and a two-row comparison table. Preserve the gallery, chapters 1/2 and existing pathfinding examples. Correct the nearby diagram's priority-queue wording to match the displayed List-based candidate selection.

## Technical Context
- Language/version: Markdown/HTML; existing Node >=18 builder.
- Dependencies/storage: Existing static site sources and generated HTML; no additions.
- Target platform: Desktop Chromium and Pixel 5 mobile.
- Testing: npm run build, existing npm test suite, one local browser review of the integrated summary and retained disclosures.
- Scope: One report source and its generated page, plus planning records.
- Constraints: No game-source edits, new styling, animation, commit, push or deployment. Preserve existing chapter 1/2/3 links and remove chapter 4/5 links.
- Reading target: New section under 500 characters; no detailed state lists or serving code.

## Constitution Check
Passed before research and after design. AGENTS.md governs because the constitution is a template. DESIGN.md is preserved through reuse of the existing report table and heading styles. This is a content consolidation, with no new interface design or runtime behavior.

## Project Structure
- content/projects/moonlight.md: source edit.
- project/moonlight/index.html: generated page.
- specs/010-consolidate-moonlight-pathfinding/: requirements, plan and validation.
- tests/slime-moonlight-gallery.spec.js and existing suite: unchanged regression checks.

## Implementation
1. Remove chapters 4/5 including their diagrams, state lists and Staff_AI code disclosure.
2. Add 3.4 covering customer seat/exit goals, staff order/tool/serving goals, shared A* routes and behavior continuation after movement.
3. Keep existing gallery and four earlier code disclosures byte-for-byte; correct only the pathfinding diagram's queue claim to match its code.
4. Build; run the existing desktop/mobile suite; inspect the new summary, contents links, code toggles and mobile overflow in a local browser.
5. Leave all changes uncommitted and report the local result.

## Complexity Tracking
No new runtime code, dependencies or permanent tests for this editorial change.
