# Research

- Source: Read content/projects/moonlight.md completely. Confirmed the public route still has chapters 1 through 5 through a read-only HTTP request.
- Customer relationship: Choose and reserve an available chair; use A* to reach it, then release the seat and use A* for the exit after eating.
- Staff relationship: Orders identify the food and customer; reserve an appropriate cooking tool, move to it and to the customer for serving, then release the tool.
- Decision: Summarize those destination/occupancy responsibilities separately from shared A* route calculation in a compact section 3.4. Remove full behavior-state diagrams and serving code.
- Accuracy: Existing FindPath uses List<Grid> and scans for the lowest FCost, with hCost as tie-breaker. Its diagram should describe candidate-node selection rather than a priority queue.
- No unresolved questions or external implementation research are required. No new behavioral claims are introduced.

## Customer and staff A* request examples

Read the actual game project at C:/MyProject/MoonLight_Project/Assets/Scripts. CustomerAI.MoveToPosition calls Pathfinding.instance.FindPath, validates a nonempty result and hands it to customerMovement.MovingTo. Staff_AI.MoveToPosition uses the same search and hands the route plus isCook to NPCMovement. Both reset currentPath on failure. The new excerpts preserve those functions except debug logging and comments. Keep NPC goal selection and other service logic outside the excerpts.
