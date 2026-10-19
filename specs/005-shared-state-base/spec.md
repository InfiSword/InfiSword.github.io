# Feature Specification: Shared Player and Monster State Base

**Feature Branch**: Current working branch
**Created**: 2026-10-19
**Status**: Reviewed
**Input**: Make player and monster states share State in Slime-Project and update the portfolio to match the real implementation.

## User Scenarios & Testing

### User Story 1 - Share the state lifecycle (Priority: P1)
As the developer, I can treat player and monster states through one common State lifecycle without changing established character behavior.
**Why this priority**: The implementation must support the architecture described in the report.
**Independent Test**: Exercise both branches through the shared lifecycle; verify monster animation/check order and player composition remain intact.
**Acceptance Scenarios**:
1. Given player and monster state instances, when used through State, then lifecycle calls dispatch to their respective implementations.
2. Given a monster transition, when leaving and entering states, then existing animation and sensor-check behavior is preserved.
3. Given player movement, attack, dash and death modules, when the controller invokes them, then they retain their current independent operation.

### User Story 2 - Read an accurate portfolio (Priority: P2)
Readers see the implemented shared hierarchy and understand how monster transitions differ from player feature composition.
**Why this priority**: Avoid implying that both controllers already use the same exclusive-state transition loop.
**Independent Test**: Compare published code examples with source and open/close them on desktop/mobile and keyboard.
**Acceptance Scenarios**:
1. Given chapter 2, when reading the architecture, then shared lifecycle and specialized branches are explicit.
2. Given chapter 4.2, when expanding code, then the actual monster base and representative derived class are visible.

### Edge Cases
- Shared player lifecycle must not require a monster, animator parameter or state machine.
- Monster entry checks must run once after animation activation, with the existing start-time semantics.
- Preserve original script metadata, scene references, unrelated project changes and the racing report edits from earlier work.
- Existing Korean source files may use legacy encoding; preserve untouched bytes.

## Requirements

### Functional Requirements
- **FR-001**: Provide one common State base inherited by player and monster state branches.
- **FR-002**: Keep monster-specific entity references, animation parameters, entry time and sensor calls in a specialized monster base.
- **FR-003**: Keep player-specific entity references in Player_State; preserve controller call sites.
- **FR-004**: Preserve existing monster FSM behavior and all leaf-state behavior.
- **FR-005**: Update portfolio chapters 2, 3 and 4.2 with accurate terminology, hierarchy and source-backed collapsible examples.
- **FR-006**: Preserve the site's existing design, navigation, gallery and responsive behavior.

## Success Criteria

### Measurable Outcomes
- **SC-001**: Both state branches are assignable to the common State type.
- **SC-002**: Lifecycle checks pass for initialization, transition order, monster animation/sensing and player no-op defaults.
- **SC-003**: All changed report examples match the implemented class bodies.
- **SC-004**: Code disclosures open and close with pointer and keyboard on desktop and mobile.

## Assumptions
- Sharing a base class does not require rewriting player composition into an exclusive-state FSM.
- Existing leaf state behavior, gameplay balance and game engine version stay unchanged.
- Changes are local; deployment and committing are outside this request.
