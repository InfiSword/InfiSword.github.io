# Research

- Decision: Treat “write the article again” as rewriting the Slime Project portfolio report, using the preceding concise-content and optional-code requirements.
- Rationale: It is the report being deployed and discussed. The intro currently overstates uniform FSM control; the rewrite should match the already-described distinction between monster transitions and player composition.
- Evidence: Existing State, Monster_State and Player_State excerpts; Player_Slime.cs calls behavior objects directly. Existing tests cover content structure, disclosure interactions, contents links and overflow.
- Alternatives: A new article or new design would widen the requested scope. No unresolved technical questions or new external research are needed.

## Transformation Example

- Rabbit_ability.cs contains OnKeyboard(), guarded by Space input, isWallSliding and !isWallJumping. It applies wall-jump velocity, resets the duration counter and flips the player if needed.
- Use that method as one concise source-backed excerpt in section 3.2. The displayed method matches the actual source after whitespace normalization. Other supporting methods and fields are intentionally outside this labeled excerpt.

## Consolidate composition into chapter 2

The latest request removes chapter 3 and transformation coverage, superseding the rabbit example. Keep the state/composition excerpts unchanged and move player composition into section 2.4. A compact three-column comparison explains behavior units, execution control and extension without suggesting that the player uses the monster single-state transition flow. No additional game-source or runtime changes are needed.

## Monster composition example

BearMonster.Start() creates idle, move, dash and dead states (alongside other states) with the same FSM and selects moveState via stateMachine.Init(moveState). Show these selected source statements immediately before the existing Player_Slime.Start() initialization excerpt. This makes the object construction comparable while retaining the difference in execution control. No game-source changes.
