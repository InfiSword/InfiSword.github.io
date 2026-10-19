# Feature Specification: Consolidate the Slime FSM Report
**Feature Branch**: Current working branch
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Merge monster AI extension examples into the State base-class section, remove chapter 4, and explain the state-machine implementation in more detail.

## User Scenarios & Testing
### User Story 1 - Understand the shared state hierarchy (Priority: P1)
Readers follow one continuous chapter from the common state base to specialized player and monster classes and concrete monster examples.
**Why this priority**: Remove the duplicated explanation and clarify the real architecture.
**Independent Test**: Read section 2.2; common and specialized code examples appear together exactly once.
**Acceptance Scenarios**:
1. Given section 2.2, when reading its examples, then the common base, player branch, monster branch and concrete extensions are adjacent.
2. Given the report, when navigating the chapters, then chapter 4 and its boss subsection are absent.

### User Story 2 - Trace the working FSM (Priority: P2)
Readers trace initialization, update/check timing, condition priority and animation-event completion using the actual project implementation.
**Why this priority**: Provide verifiable engineering detail and a practical extension path.
**Independent Test**: Follow the running bear example from its initial movement state to subsequent transitions and compare the explanations with the source.
**Acceptance Scenarios**:
1. Given the initialization example, then it identifies movement as the actual starting state.
2. Given movement conditions, then detection, jump and timeout priority match the source.
3. Given animation callbacks and data settings, then their relationship to state behavior is explicit.

### Edge Cases
- Do not claim that the player controller runs an exclusive-state FSM.
- Preserve the player chapter, gallery and existing State base anchor.
- Keep code initially collapsed and usable with keyboard and touch.
- Avoid claims about null safety, atomic transitions or fixed Update/FixedUpdate ratios that are unsupported by source.

## Requirements
### Functional Requirements
- **FR-001**: Remove chapter 4, the separate boss discussion and associated contents entries.
- **FR-002**: Integrate the former 4.2 content and its examples into section 2.2 without duplication.
- **FR-003**: Expand the FSM chapter with real initialization, execution/condition timing, transitions, state data and animation events.
- **FR-004**: Keep examples faithful to current game source and distinguish excerpts from full files.
- **FR-005**: Preserve the existing reading style, source disclosures, gallery and chapter 3.

## Success Criteria
### Measurable Outcomes
- **SC-001**: Three top-level chapters remain, with zero chapter-4 headings or links.
- **SC-002**: Each common/monster extension example occurs once and belongs to section 2.2.
- **SC-003**: All remaining contents links resolve; new disclosures operate on desktop/mobile and keyboard.
- **SC-004**: At least one complete real initialization-to-transition walkthrough is available in chapter 2.

## Assumptions
- This request reorganizes the report and does not change Unity gameplay source.
- Existing screenshot evidence is retained even where it includes a boss battle.
- Continue the established commit/push/deploy workflow after verification.
