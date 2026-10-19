# Feature Specification: Rewrite Slime Project Report

**Feature Branch**: `main` (existing working branch)
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Cancel deployment recovery and write the article again.

## User Scenarios & Testing

### User Story 1 - Understand the design and inspect evidence (Priority: P1)

A portfolio reader understands the author's contribution, shared state rules, monster behavior extension and player ability composition without reading source code.

**Why this priority**: The preceding requests call for broad, concise explanations with optional essential code.
**Independent Test**: Read the article with every example closed, then open and close all five examples.

**Acceptance Scenarios**:

1. Given the rewritten report, when reading its three chapters, then the reader can distinguish shared lifecycle rules from each actor's execution flow.
2. Given a code example, when using pointer, touch or keyboard, then its contents can be opened and closed.

### Edge Cases

- Do not imply that the player uses the monster's single-active-state flow.
- Keep the removed fourth chapter absent and preserve section links and screenshots.
- Long code examples must not widen a mobile page.

## Requirements

### Functional Requirements

- **FR-001**: Rewrite the introduction and explanatory prose around the author's contribution and design decisions.
- **FR-002**: Explain shared state lifecycle, monster transitions, behavior reuse and player composition accurately.
- **FR-003**: Preserve five initially closed code examples, the gallery and the three-chapter structure.
- **FR-004**: Stop recovery monitoring as explicitly requested; further recovery is outside this rewrite.

## Success Criteria

### Measurable Outcomes

- **SC-001**: Main chapter prose excluding code remains at most 1,600 characters.
- **SC-002**: All five examples open and close on desktop and mobile using pointer and keyboard.
- **SC-003**: All contents links resolve and neither device has page-level horizontal overflow.

## Assumptions

- The article is the Slime Project report discussed throughout this conversation.
- Keep previously requested essential code and concise scope; change prose, not game source or visual design.
- Cancel recovery and its recurring checks. Do not resume stalled deployment recovery while writing.
