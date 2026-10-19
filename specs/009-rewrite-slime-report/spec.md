# Feature Specification: Rewrite Slime Project Report

**Feature Branch**: `main` (existing working branch)
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Keep the two-chapter report and add collapsible monster composition code beside the player code in section 2.4. Edit files only, without committing or pushing.

## User Scenarios & Testing

### User Story 1 - Understand the design and inspect evidence (Priority: P1)

A portfolio reader understands the author's contribution, shared state rules, monster behavior extension and the difference between monster state transitions and player feature composition without reading source code.

**Why this priority**: The preceding requests call for broad, concise explanations with optional essential code.
**Independent Test**: Read the two-chapter article with every example closed, inspect the monster/player comparison in section 2.4, then open and close all six examples.

**Acceptance Scenarios**:

1. Given the rewritten report, when reading its two chapters, then the reader can distinguish shared lifecycle rules from each actor's execution flow in section 2.4.
2. Given a code example, when using pointer, touch or keyboard, then its contents can be opened and closed.
3. Given the report and contents list, then chapter 3, the transformation explanation and rabbit wall jump example are absent.
4. Given section 2.4, then monster state construction and player feature construction are available in adjacent, independently collapsible examples.

### Edge Cases

- Do not imply that the player uses the monster's single-active-state flow.
- Keep the removed third and fourth chapters absent and preserve remaining section links and screenshots.
- Long code examples must not widen a mobile page.

## Requirements

### Functional Requirements

- **FR-001**: Rewrite the introduction and explanatory prose around the author's contribution and design decisions.
- **FR-002**: Explain shared state lifecycle, monster transitions, behavior reuse and player composition accurately.
- **FR-003**: Keep chapters 1 and 2, the gallery and six initially closed state/composition examples. Chapter 3, transformation text and rabbit wall jump code remain absent.
- **FR-004**: Stop recovery monitoring as explicitly requested; further recovery is outside this rewrite.
- **FR-005**: Leave all edits uncommitted and do not push or deploy them.
- **FR-006**: Present feature composition in section 2.4, comparing monster and player responsibilities with separate adjacent monster and player code examples. The monster example must come from the actual project.

## Success Criteria

### Measurable Outcomes

- **SC-001**: Main chapter prose excluding code remains at most 1,600 characters.
- **SC-002**: All six examples start closed and open and close on desktop and mobile using pointer and keyboard; the two section 2.4 examples can remain open together.
- **SC-003**: All contents links resolve and neither device has page-level horizontal overflow.

## Assumptions

- The article is the Slime Project report discussed throughout this conversation.
- Keep previously requested essential code and concise scope; change prose, not game source or visual design.
- Cancel recovery and its recurring checks. Do not resume stalled deployment recovery while writing.
- This revision follows the user's explicit files-only instruction. The previous commit is not rewritten or reverted.
- The latest instruction supersedes the earlier request for a transformation example. Game sources are read only.
