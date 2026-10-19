# Research
- Decision: Consolidate class hierarchy and extensions in 2.2; explain runtime mechanics in 2.3 and data/animation in 2.4.
- Rationale: The old 4.2 repeated the base-class explanation, while the report omitted initialization and concrete transition priorities.
- Source findings: BearMonster.Start calls base.Start, creates eight state instances and initializes moveState. Monster_Entity routes Update/FixedUpdate to currentState. Monster_State checks on entry and physics updates; MoveState caches sensor booleans and updates its timeout; Bear_MoveState prioritizes detection, jump, timeout, then wall/ledge direction changes. AnimationToStateMachine forwards callbacks to the active Dash or AttackState. D_MoveState stores movement/time values.
- Alternatives: Keeping a separate AI chapter would retain duplication; rewriting gameplay to fit the report is unnecessary and outside scope.
- UX review: UI UX Pro Max heading-hierarchy guidance and the existing Impeccable/unslop-ui preservation rules support one reading sequence and initially closed source examples. No visual redesign is needed.
