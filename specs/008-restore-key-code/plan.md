# Implementation Plan: Restore Key Code
**Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Keep the concise prose and add five native details/summary sections: FSM transition class; State and Player_State; Monster_State owner/entry/exit excerpt; MoveState entry/logic excerpt; Player_Slime composition statements.

## Technical Context
Markdown/HTML, existing Node builder and Playwright. Read game sources without edits. Reuse pf-details and pf-shared-state-code styling, with no new dependencies or visuals.

## Constitution Check
Passed before/after planning. AGENTS.md governs; the constitution is a template. Preserve DESIGN.md, concise content, gallery and shared lifecycle truth.

## Project Structure
content/projects/slime-project.md -> project/slime-project/index.html; tests/slime-state-report.spec.js; specs/008-restore-key-code/.

## Reading Brief
The architecture remains readable while closed. Summaries identify evidence by purpose; excerpts are labeled. Existing pointer and keyboard behavior and source-block scrolling remain intact.

## Validation
Build, full desktop/mobile suite, source-parity and prose-preservation checks, one screenshot batch, then established commit/push/deploy and live verification.

## Complexity Tracking
No runtime architecture changes.
