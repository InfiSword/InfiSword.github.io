# Implementation Plan: Shared State Base
**Date**: 2026-10-19 | **Spec**: [spec.md](spec.md)

## Summary
Extract a neutral State lifecycle. Move the existing monster State body into Monster_State : State, preserving constructor and callback order. Point the ten direct monster state roots at Monster_State. Player_State : State retains its typed owner and inherits the common no-op callbacks; existing controller and concrete player modules stay unchanged. Update the report with exact source examples and native disclosures.

## Technical Context
- Language: C#, Unity project 2022.3.26f1; Markdown/HTML, Node >=18 for the website.
- Dependencies: Existing packages only. Installed Unity editors are 6000.x; do not upgrade or open the original project with a mismatched editor.
- Testing: Offline .NET 9 executable harness linking the four real lifecycle sources, with explicit Unity/owner test doubles. Compile/execute shared dispatch and monster lifecycle semantics; statically inspect all ten roots. Website: build, existing Playwright regressions, desktop/mobile disclosure checks.
- Storage/platform: C:/MyProject/Slime-Project and static GitHub Pages source in the current repository.
- Scope: Fourteen game assets (State, Monster_State + metadata, Player_State, ten roots), a standalone lifecycle test harness outside Assets, one report and generated HTML.
- Constraints: No player FSM conversion, no asset GUID replacement, no gameplay tuning, no new runtime dependencies.

## Constitution Check
Passed before and after design. The constitution is an unfilled template; AGENTS.md is the active workflow. Specification and plan precede changes. Existing design and user edits are preserved. External project writes are limited to reviewed explicit paths with hashes guarding against concurrent changes.

## Project Structure
- Slime-Project/Assets/Scripts/NewMonster/State Machine/{State,Monster_State,FiniteStateMachine}.cs
- Slime-Project/Assets/Scripts/NewMonster/States/: ten direct state bases change parent only.
- Slime-Project/Assets/Scripts/Player/Player_Attribute/Player_State.cs
- Slime-Project/Tests/StateLifecycle/: dependency-free executable tests linking real sources.
- content/projects/slime-project.md -> project/slime-project/index.html
- specs/005-shared-state-base/: requirements, rationale, contracts and validation.

## Reading Surface Brief
Readers should understand one shared lifecycle and two specialized branches. Retain DESIGN.md typography/palette and existing code-disclosure controls. Chapter 2 shows the common base and player adapter; chapter 4.2 shows Monster_State and Bear_DashState. All blocks start closed. Native summary controls support pointer, Enter and Space with visible keyboard focus. No new visual system, icons or animation.

## Complexity Tracking
No architecture violations. The standalone harness uses test doubles and cannot certify Unity scene behavior; report that limit explicitly.
