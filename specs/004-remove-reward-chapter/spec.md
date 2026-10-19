# Feature Specification: Remove Reward Engineering Chapter

**Feature Branch**: Current working branch
**Created**: 2026-10-19
**Status**: Reviewed
**Input**: Remove chapter 3, 정교한 보상 설계 (Reward Engineering), from the autonomous racing project.

## User Scenarios & Testing

### User Story 1 - Read the revised project report (Priority: P1)
Visitors read the report without the removed reward engineering chapter.
**Why this priority**: This is the requested content correction.
**Independent Test**: Open the project report and inspect its chapters and contents list.
**Acceptance Scenarios**:
1. Given the report, when visitors read after chapter 2, then the environment sensing chapter follows directly.
2. Given the contents list, when visitors navigate chapters, then no removed reward chapter or subsection appears.

### Edge Cases
- Remove all nested explanations, reward panels, and code examples within the removed chapter.
- Renumber the following chapter and its subsections consistently.
- Preserve reward mentions elsewhere because the request targets one chapter.

## Requirements

### Functional Requirements
- **FR-001**: Remove the entire reward engineering chapter and its contents-list entries.
- **FR-002**: Number the remaining chapters and subsections consecutively.
- **FR-003**: Preserve all other report content, gallery behavior, and responsive presentation.

## Success Criteria

### Measurable Outcomes
- **SC-001**: Zero removed chapter headings, panels, or examples remain visible in the report.
- **SC-002**: All remaining chapter links resolve to their corresponding headings.
- **SC-003**: The gallery remains usable on desktop and mobile.

## Assumptions
- This is a content removal, with no new visual design or dependencies.
- Publishing is outside the requested local edit.
