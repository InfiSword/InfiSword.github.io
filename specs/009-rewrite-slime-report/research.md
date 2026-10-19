# Research

- Decision: Treat “write the article again” as rewriting the Slime Project portfolio report, using the preceding concise-content and optional-code requirements.
- Rationale: It is the report being deployed and discussed. The intro currently overstates uniform FSM control; the rewrite should match the already-described distinction between monster transitions and player composition.
- Evidence: Existing State, Monster_State and Player_State excerpts; Player_Slime.cs calls behavior objects directly. Existing tests cover content structure, disclosure interactions, contents links and overflow.
- Alternatives: A new article or new design would widen the requested scope. No unresolved technical questions or new external research are needed.

## Transformation Example

- Rabbit_ability.cs contains OnKeyboard(), guarded by Space input, isWallSliding and !isWallJumping. It applies wall-jump velocity, resets the duration counter and flips the player if needed.
- Use that method as one concise source-backed excerpt in section 3.2. The displayed method matches the actual source after whitespace normalization. Other supporting methods and fields are intentionally outside this labeled excerpt.
