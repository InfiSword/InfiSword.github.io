# Feature Specification: Restore Collapsible Key Code
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Keep the concise report and restore relevant essential code in collapsed sections.

## User Scenarios & Testing
### User Story 1 - Read the design, optionally inspect key code (Priority: P1)
Readers scan the concise architecture and open only the implementation evidence they need.
**Why this priority**: Support both quick portfolio reading and optional technical inspection.
**Independent Test**: Load the report with code hidden, then open each example with pointer and keyboard.
**Acceptance Scenarios**:
1. Given the report, then the concise explanatory prose is unchanged and code starts hidden.
2. When opening a code section, then a relevant source-backed example appears and can be closed again.

### Edge Cases
- Keep examples limited to FSM transitions, the shared state hierarchy, reusable movement and player composition.
- Label excerpts clearly and preserve existing source behavior in displayed code.
- Long code lines scroll within their own blocks without widening the page.

## Requirements
### Functional Requirements
- **FR-001**: Preserve the concise three-chapter explanation and gallery.
- **FR-002**: Add five named, initially collapsed source examples at their relevant sections.
- **FR-003**: Support pointer, Enter and Space with visible focus.
- **FR-004**: Use real project source without changing Unity files.

## Success Criteria
### Measurable Outcomes
- **SC-001**: Exactly five source disclosures start closed; all code is inside them.
- **SC-002**: Main prose excluding source disclosures remains under 1,600 characters.
- **SC-003**: All disclosures and contents links work on desktop/mobile without page overflow.

## Assumptions
Continue the established commit/push/deploy workflow after validation.
