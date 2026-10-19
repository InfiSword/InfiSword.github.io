# Implementation Plan: Consolidate Slime FSM
**Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Move the former 4.2 prose and Monster_State/Bear_DashState disclosures into 2.2. Delete the boss subsection and chapter-4 boundary. Expand chapter 2 with source-backed lifecycle and call-site walkthroughs, movement transition priority, ScriptableObject configuration and animation-event dispatch. Retain chapter 3 and update overview/contents accordingly.

## Technical Context
- Language/platform: Markdown/HTML, existing Node static builder, GitHub Pages.
- Dependencies: No additions. Native details/summary and existing styles remain authoritative.
- Sources: Read-only C:/MyProject/Slime-Project/Assets/Scripts/NewMonster/.
- Testing: npm run build, full npm test desktop/mobile suite, source-parity checks, heading/contents checks and one screenshot review batch.
- Scope: One source report, generated HTML, its existing report tests and this documentation.
- Constraints: No Unity source edits, no new visual language or motion, retain player composition and gallery behavior.

## Constitution Check
Passed before and after planning. The project constitution is a template; AGENTS.md supplies active requirements. Specification and plan precede edits. Existing DESIGN.md and incumbent source disclosures govern presentation.

## Project Structure
- content/projects/slime-project.md -> project/slime-project/index.html
- tests/slime-state-report.spec.js
- specs/006-consolidate-slime-fsm/: planning and validation

## Reading Surface Brief
The visitor should understand the hierarchy once, then follow its runtime behavior. Section 2.1 covers the FSM contract, 2.2 owns base classes and monster extensions, 2.3 traces initialization/updates/transition priority, and 2.4 connects data and animation callbacks. Keep long code behind existing named summaries. Use short sequential prose and a compact condition table. Preserve the old 2.2 heading/anchor, typography, colors and chapter 3.

## Complexity Tracking
No new runtime or UI architecture required.
