# Compatibility Contracts
- Monster_State constructor retains (FiniteStateMachine, Monster_Entity, string); existing derived constructor calls are unchanged.
- Player_State constructor retains (Player_Entity); existing controller initialization is unchanged.
- State exposes public virtual void Enter/Exit/LogicUpdate/PhysicsUpdate/DoChecks.
- Monster entry/exit animation and sensing semantics remain identical; common State defaults have no engine or owner side effects.
- Existing .meta GUIDs and scene/prefab content are preserved.
- Report route and chapter 2/4.2 fragment links remain valid. New disclosures use existing pf-details with unique descriptive summaries, initially closed.
