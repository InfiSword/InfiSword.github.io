# Research
- Decision: Neutral State lifecycle plus Monster_State and Player_State adapters.
- Rationale: Existing State depends on Monster_Entity, an animator parameter and a monster FSM; player modules require only Player_Entity. Sharing lifecycle signatures avoids null monster references and casts.
- Alternatives considered: Converting player input to an exclusive FSM changes movement/attack/dash concurrency and exceeds the requested base-class refactor. A generic hierarchy adds unnecessary types for this small shared contract.
- Preserve monster entry order: start time, animation true, DoChecks, debug log. Physics calls DoChecks exactly once. Retain original source encoding through byte-level substitutions.
- Validation environment: Unity 2022.3.26f1 is requested by the project but unavailable; available Unity 6000.x must not upgrade it. Use compiled executable lifecycle tests and clearly report that Unity Editor play testing remains outstanding.
- UI: UI UX Pro Max recommends keyboard access and visible focus. Existing DESIGN.md is authoritative; native details/summary and scoped focus styling preserve the report's visual language. No external research is needed for repository-specific refactoring.
