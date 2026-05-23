---
layout: single
title: "PROJECT REPORT // SLIME PROJECT"
excerpt: "State Machine 기반 게임 로직 및 AI 설계"
categories: [project]
permalink: /project/slime-project/
tags: [FSM, Unity, AI, Design Pattern]
toc: true
toc_sticky: true
classes: wide
---

<style>
    /* Origin Styles Restoration */
    .pf-visual-frame {
        width: 100%; padding: 30px; background: rgba(0,0,0,0.03);
        border: 1px solid #e1e4e8; border-radius: 8px;
        margin: 25px 0; text-align: center;
    }
    .pf-arch-diagram {
        display: flex; flex-direction: column; gap: 15px; margin: 25px 0;
    }
    .pf-arch-layer {
        padding: 15px 20px; border: 1px solid #e1e4e8; border-radius: 6px;
        background: #f8f9fa; position: relative; text-align: center;
    }
    .pf-arch-layer::before {
        content: ""; position: absolute; left: 0; top: 0; bottom: 0; width: 4px;
        background: #007bff;
    }
    .pf-arch-layer-title {
        color: #007bff; font-weight: 700; margin-bottom: 8px;
        font-family: 'Fira Code', monospace; font-size: 0.9rem; text-align: center;
    }
    .pf-arch-layer-items {
        display: flex; flex-wrap: wrap; gap: 8px; margin-top: 8px; justify-content: center;
    }
    .pf-arch-item {
        padding: 5px 12px; background: rgba(0, 123, 255, 0.1);
        border: 1px solid #007bff; border-radius: 4px;
        font-size: 0.85rem; color: #333; text-align: center;
    }
</style>

하드코어 액션 퍼즐 플랫포머 게임으로, 디자인 패턴을 적극 활용하여 확장성 있는 게임 시스템을 구축한 프로젝트입니다.

---

# 1. FiniteStateMachine (FSM) 기반 상태 관리

모든 몬스터와 플레이어의 행동을 체계적으로 제어하기 위해 공통 상태머신 아키텍처를 설계했습니다.

### 1.1 아키텍처 구조
*   **Init:** 시작 상태 진입 및 `Enter()` 호출.
*   **ChangeState:** 현재 상태의 `Exit()` 호출 후, 새로운 상태의 `Enter()` 호출로 안전한 전이 보장.
*   **Lifecycle:** `LogicUpdate()` (프레임 연산), `PhysicsUpdate()` (물리 연산) 분리로 연산 효율화.

```csharp
public void ChangeState(State newState) {
    currentState.Exit();
    currentState = newState;
    currentState.Enter();
}
```

---

# 2. 플레이어 슬라임 (컴포지션 패턴)

기능의 결합도를 낮추고 유지보수성을 높이기 위해 **컴포지션(Composition) 패턴**을 도입했습니다.

### 2.1 기능별 컴포넌트 분리
*   **Movement:** 키보드 입력 및 기본 물리 이동 관리.
*   **Attack:** 공격 판정 및 데미지 로직 처리.
*   **Dash:** 무적 프레임 및 위치 보간 계산.
*   **Form (변신):** 특정 적 처치 시 해당 능력을 오버라이딩하여 적용 (예: 토끼 폼의 벽 점프).

<div class="pf-visual-frame">
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">Player_Slime (Main)</div>
            <div class="pf-arch-layer-items">
                <span class="pf-arch-item">SlimeMovement</span>
                <span class="pf-arch-item">SlimeAttack</span>
                <span class="pf-arch-item">SlimeDash</span>
            </div>
        </div>
        <div style="text-align: center; color: #007bff; font-size: 1.5rem;">↓ Transform</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">Rabbit Form (Specialized)</div>
            <div class="pf-arch-layer-items">
                <span class="pf-arch-item">Double Jump</span>
                <span class="pf-arch-item">Wall Sliding</span>
            </div>
        </div>
    </div>
</div>

---

# 3. 보스 몬스터 AI (델리게이트 시스템)

보스의 복잡한 스킬 패턴을 `if-else` 또는 `switch`문 없이 관리하기 위해 **델리게이트 배열(Delegate Array)**을 활용했습니다.

### 3.1 기술적 특징
*   **랜덤 패턴:** 델리게이트 배열에서 인덱스를 랜덤으로 추출하여 스킬 실행.
*   **HP 기반 업그레이드:** 보스 체력이 50% 이하로 감소 시 `IsSkillUpgrade` 플래그를 활성화하여 모든 스킬의 강화 버전 발동.

```csharp
private delegate void Boss_Skill();
private Boss_Skill[] Boss_Skills;

// 스킬 실행 예시
Boss_Skills[Random.Range(0, Skill_Max)].Invoke();
```

---

# 4. 일반 몬스터 AI 확장

공통 기반 상태(Move, Idle, Detect)를 상속받아 몬스터별 특수 행동만 추가하는 방식으로 구현 효율을 극대화했습니다.

*   **Bear:** `DashState`를 추가하여 강력한 돌진 구현.
*   **Spider:** `JumpState`와 원거리 투사체 로직 결합.

{: .notice--warning}
**디자인 철학:** 새로운 몬스터가 추가되어도 공통 FSM 구조 덕분에 코드 중복 없이 10분 내외로 기초 AI를 구성할 수 있는 구조를 완성했습니다.
