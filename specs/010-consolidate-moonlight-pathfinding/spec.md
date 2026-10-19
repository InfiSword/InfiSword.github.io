# Feature Specification: Consolidate MoonLight Pathfinding Report

**Feature Branch**: `main` (existing branch)
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Remove MoonLight chapters 4 and 5 and briefly connect their relevant content to the pathfinding chapter.

## User Scenarios & Testing

### User Story 1 - Understand how NPC goals connect to pathfinding (Priority: P1)

A portfolio reader reads the spatial data and pathfinding explanation, then sees how customer and staff actions determine movement goals without reading two separate AI chapters.

**Why this priority**: Reduce repetition while preserving the relationship between NPC behavior, shared route calculation and resource occupancy.
**Independent Test**: Read the remaining three-chapter outline and the short customer/staff comparison under pathfinding.

**Acceptance Scenarios**:

1. Given the report and contents list, chapters 4 and 5 and their standalone diagrams/code are absent.
2. Given the pathfinding chapter, a brief customer/staff comparison explains destination selection, travel and the handoff back to behavior after movement.
3. Given the existing screenshots and code controls, they remain readable and usable on desktop and mobile.

### Edge Cases

- Distinguish route computation from selecting destinations, reserving resources and advancing behavior.
- Preserve existing chapter 1/2/3 links; remove obsolete chapter 4/5 contents links.
- Keep the summary short instead of reproducing detailed AI states or serving code.

## Requirements

### Functional Requirements

- **FR-001**: Remove chapters 4 and 5 including their subsections, diagrams, tables and serving code example.
- **FR-002**: Add a concise customer/staff relationship summary within chapter 3 covering seat/exit movement and cooking/serving movement.
- **FR-003**: Preserve the gallery, spatial data chapter and existing pathfinding code examples.
- **FR-004**: Avoid attributing destination choice or occupancy management to the route search itself.
- **FR-005**: Modify files only; no commit, push, deployment or game-source edits.

## Success Criteria

### Measurable Outcomes

- **SC-001**: Exactly three numbered chapters remain, with no broken contents links.
- **SC-002**: The integrated relationship summary consists of at most two short paragraphs and one two-row comparison table, under 500 characters in total.
- **SC-003**: Seven gallery images and four retained code disclosures remain usable on desktop and mobile without page-level horizontal overflow.

## Assumptions

- The referenced MoonLight route maps to content/projects/moonlight.md in this repository.
- Preserve existing presentation and optional code controls. No new design or dependency is needed.
- Earlier files-only instructions remain in effect; do not commit or push.
