# Feature Specification: Distill WorldFirstKill Report

**Feature Branch**: `main` (unchanged)
**Created**: 2026-10-20
**Status**: Reviewed
**Input**: Keep the simplified WorldFirstKill report; add collapsible token generation and parsing code in section 2.2 and reuse suitable example images from the prior article. Do not commit.

## User Scenarios & Testing

### User Story 1 - Scan the architecture and optionally inspect code (Priority: P1)

A portfolio reader quickly understands how planning data reaches gameplay and how random-generation progress is saved/restored. They inspect only the relevant implementation excerpt.

**Why this priority**: The existing report repeats detailed explanations, large diagrams and unsupported performance claims that obscure the core responsibilities.
**Independent Test**: Read two compact system sections with code collapsed; inspect a flow and its class responsibility map, then open the relevant code.

**Acceptance Scenarios**:

1. The report explains data synchronization/conversion and random-state save/restore through two short flows and explicit class responsibilities.
2. Six essential source-backed examples start collapsed and can be opened with pointer/touch or keyboard; section 2.2 presents token generation, parsing and Skip restoration in that order.
3. The original six-image gallery remains functional.
4. A local build preview opens visibly for the user without publishing or committing changes.
5. Relevant prior images illustrate the meta sheet, monster data and seed-dependent skill results next to the corresponding explanations.

### Edge Cases

- Preserve the distinction between random-state restoration and saving the entire game world.
- Do not retain unsupported timing, compression or universal determinism guarantees.
- Keep long class names and code from widening mobile pages.
- Preserve unrelated project content and existing gallery controls.

## Requirements

### Functional Requirements

- **FR-001**: Condense the report into data synchronization/conversion and random-state save/restore sections.
- **FR-002**: Provide one four-step flow per system and a compact map of responsible classes.
- **FR-003**: Preserve optional essential code with accurate excerpt labels and source names.
- **FR-004**: Preserve the six-image gallery and public route.
- **FR-005**: Open a visible local preview after successful build and verification.
- **FR-006**: Modify portfolio files only; no commit, push, deployment or game-source changes.
- **FR-007**: Add actual BuildSeedToken and RestoreUsageCount excerpts, accurately label the fixed-width helper dependencies and preserve the existing RestoreRNG example.
- **FR-008**: Reuse the existing master/monster sheet screenshots and two raw seed-specific skill screenshots with short accurate captions and links to their full-size originals. Do not treat differing skill screenshots as proof of save/restore correctness.

## Success Criteria

### Measurable Outcomes

- **SC-001**: Main report has two numbered chapters and four subsections; visible explanatory text is at most 1,600 characters excluding code and gallery.
- **SC-002**: Two four-step flows and five class responsibilities can be read without opening code.
- **SC-003**: All six source disclosures, contents links, six gallery images and four reused example images work on desktop/mobile, with no page overflow.
- **SC-004**: A visible browser preview displays the newly built local page.

## Assumptions

- This is refinement within the existing portfolio identity, not a rebrand.
- Existing colors, typefaces, site shell, gallery and optional-code convention are the visual authority.
- The local WorldFirstKill legacy restore sources provide implementation evidence. Read them without edits.
- Earlier files-only instructions remain active.
