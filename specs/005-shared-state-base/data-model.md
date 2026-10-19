# State Model
- State: shared Enter, Exit, LogicUpdate, PhysicsUpdate, DoChecks callbacks, with neutral defaults.
- Monster_State : State: owns Monster_Entity, FiniteStateMachine, animation name and start time. Existing ten behavior roots derive from this branch.
- Player_State : State: owns Player_Entity. Player_Movement, Player_Attack, Player_Dash and Player_Dead retain this parent and controller composition.
- FiniteStateMachine: unchanged; holds State and calls Exit before Enter. Used by monster controllers; player composition does not gain an exclusive active-state slot.
