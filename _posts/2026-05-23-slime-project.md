---
layout: single
title: "PROJECT REPORT // SLIME PROJECT"
excerpt: "상태머신 기반 게임 로직 및 확장 가능한 몬스터 AI 시스템"
categories: [project]
permalink: /project/slime-project/
tags: [Unity, AI, FSM, Design Pattern]
toc: true
toc_sticky: true
---

<style>
    /* --- Premium Report Layout Styles --- */
    
    .chapter-title {
        font-size: 2.2rem;
        color: #fff !important;
        background: linear-gradient(90deg, #007bff 0%, #58A6FF 100%);
        padding: 25px 35px;
        border-radius: 12px;
        margin-top: 60px !important;
        margin-bottom: 40px !important;
        box-shadow: 0 10px 30px rgba(0, 123, 255, 0.15);
        display: flex;
        align-items: center;
        border: none !important;
    }
    .chapter-title::before {
        content: "CHAPTER.";
        font-family: 'Fira Code', monospace;
        font-size: 0.9rem;
        letter-spacing: 2px;
        margin-right: 15px;
        opacity: 0.8;
    }

    .pf-visual-frame {
        width: 100%; padding: 35px; background: #fcfcfc;
        border: 1px solid #e1e4e8; border-radius: 16px;
        margin: 30px 0; text-align: center;
        box-shadow: inset 0 2px 10px rgba(0,0,0,0.02);
    }
    .pf-arch-diagram {
        display: flex; flex-direction: column; gap: 20px; margin: 25px 0;
    }
    .pf-arch-layer {
        padding: 20px 25px; border: 1px solid #e1e4e8; border-radius: 8px;
        background: #fff; position: relative; text-align: center;
        box-shadow: 0 4px 12px rgba(0,0,0,0.03);
    }
    .pf-arch-layer::before {
        content: ""; position: absolute; left: 0; top: 0; bottom: 0; width: 5px;
        background: #007bff; border-radius: 8px 0 0 8px;
    }
    .pf-arch-layer-title {
        color: #007bff; font-weight: 700; margin-bottom: 10px;
        font-family: 'Fira Code', monospace; font-size: 0.95rem; text-align: center;
    }
    .pf-arch-layer-items {
        display: flex; flex-wrap: wrap; gap: 10px; margin-top: 10px; justify-content: center;
    }
    .pf-arch-item {
        padding: 6px 14px; background: rgba(0, 123, 255, 0.08);
        border: 1px solid rgba(0, 123, 255, 0.3); border-radius: 6px;
        font-size: 0.85rem; color: #0056b3; text-align: center;
    }

    /* --- Code Details --- */
    details.pf-details {
        margin: 20px 0;
        border: 1px solid #e1e4e8;
        border-radius: 8px;
        background: #f8f9fa;
        overflow: hidden;
    }
    details.pf-details summary {
        padding: 15px 20px;
        font-weight: 700;
        color: #007bff;
        cursor: pointer;
        outline: none;
        background: #fff;
        display: flex;
        align-items: center;
    }

    .highlight-box {
        background: #f0f7ff;
        border-left: 5px solid #007bff;
        padding: 20px;
        margin: 20px 0;
        border-radius: 0 8px 8px 0;
    }

    /* --- Flow Arrows --- */
    .flow-arrow {
        color: #007bff; font-weight: bold; font-size: 1.5rem; margin: 10px 0;
    }

    /* --- Transaction Flow --- */
    .pf-transaction-flow {
        display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-family: 'Fira Code', monospace;
        gap: 12px; flex-wrap: wrap; margin: 20px 0;
    }
    .pf-flow-step { padding: 10px 18px; border: 1px solid #e1e4e8; border-radius: 6px; background: #fff; color: #333; text-align: center; box-shadow: 0 2px 5px rgba(0,0,0,0.05); line-height: 1.4; }
    .pf-flow-arrow { color: #007bff; font-weight: bold; font-size: 1.2rem; }
    
    /* --- Grid Systems --- */
    .pf-grid {
        display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; margin: 25px 0;
    }
    .pf-grid-item {
        background: #fff; border: 1px solid #e1e4e8; border-radius: 12px; padding: 25px; text-align: center;
        transition: 0.3s;
    }
    .pf-grid-item:hover { border-color: #007bff; transform: translateY(-5px); box-shadow: 0 10px 20px rgba(0,123,255,0.05); }
    .pf-grid-label { color: #007bff; font-weight: 700; margin-bottom: 12px; font-family: 'Fira Code', monospace; font-size: 1rem; display: block; }
    .pf-grid-desc { color: #555; font-size: 0.85rem; line-height: 1.6; }
</style>

Slime Project는 하드코어 액션 퍼즐 플랫포머 게임으로, **FiniteStateMachine(FSM) 기반 상태 관리 시스템**을 통해 모든 게임 엔티티의 행동을 체계적으로 관리합니다. 플레이어 슬라임의 움직임, 변신 능력, 그리고 변신 시 능력 변화를 구현했으며, 보스 몬스터와 일반 몬스터의 AI 로직을 담당하여 확장 가능한 게임 시스템을 구축했습니다.

---

## 1. 프로젝트 개요
{: .chapter-title }

본 프로젝트는 복잡한 게임 로직을 디자인 패턴을 활용해 구조화하는 데 집중했습니다. 모든 캐릭터와 몬스터는 통일된 인터페이스를 공유하며, 기능 중심의 컴포지션 설계를 통해 유지보수성을 극대화했습니다.

<div class="pf-visual-frame">
    <div class="pf-grid">
        <div class="pf-grid-item">
            <span class="pf-grid-label">상태머신 시스템</span>
            <p class="pf-grid-desc">FiniteStateMachine 기반<br>상태 전환 관리 및 라이프사이클</p>
        </div>
        <div class="pf-grid-item">
            <span class="pf-grid-label">플레이어 시스템</span>
            <p class="pf-grid-desc">슬라임 움직임/공격<br>변신 능력 동적 교체 구현</p>
        </div>
        <div class="pf-grid-item">
            <span class="pf-grid-label">보스 AI</span>
            <p class="pf-grid-desc">델리게이트 기반 스킬<br>HP 단계별 패턴 업그레이드</p>
        </div>
        <div class="pf-grid-item">
            <span class="pf-grid-label">일반 몬스터 AI</span>
            <p class="pf-grid-desc">상태머신 기반<br>유형별 행동 패턴 확장</p>
        </div>
    </div>
</div>

---

## 2. FiniteStateMachine 기반 상태 관리 시스템
{: .chapter-title }

모든 몬스터와 게임 엔티티의 행동을 **FiniteStateMachine** 패턴으로 관리하는 시스템을 구현했습니다. `State` 베이스 클래스를 통해 각 상태의 Enter/Exit/LogicUpdate/PhysicsUpdate 라이프사이클을 체계적으로 관리합니다.

### 2.1 상태머신 구조 및 전환 플로우

FiniteStateMachine은 현재 상태를 관리하고, 상태 전환 시 이전 상태의 Exit와 새 상태의 Enter를 자동으로 호출하여 물리 및 애니메이션 상태의 정합성을 보장합니다.

<div class="pf-visual-frame" style="overflow-x: auto;">
    <div style="display: flex; align-items: center; justify-content: flex-start; min-width: 800px; padding: 20px 0;">
        <!-- Init -->
        <div style="background: rgba(40, 167, 69, 0.05); border: 2px solid #28a745; border-radius: 8px; padding: 12px 20px; text-align: center; flex: 0 0 160px;">
            <div style="color: #28a745; font-weight: 700; font-size: 0.85rem; font-family: 'Fira Code';">State::Enter</div>
            <div style="color: #888; font-size: 0.7rem; margin-top: 4px;">상태 초기화</div>
        </div>
        
        <div style="color: #007bff; font-weight: bold; margin: 0 15px; font-size: 1.2rem;">→</div>
        
        <!-- Update Loop -->
        <div style="background: rgba(0, 123, 255, 0.05); border: 2px solid #007bff; border-radius: 8px; padding: 12px; text-align: center; flex: 0 0 240px; position: relative;">
            <div style="color: #007bff; font-weight: 700; font-size: 0.85rem; font-family: 'Fira Code'; margin-bottom: 8px;">Execution Loop</div>
            <div style="display: flex; gap: 8px;">
                <div style="background: #fff; border: 1px solid #e1e4e8; padding: 6px; border-radius: 4px; font-size: 0.7rem; flex: 1;">
                    <strong>LogicUpdate</strong>
                </div>
                <div style="background: #fff; border: 1px solid #e1e4e8; padding: 6px; border-radius: 4px; font-size: 0.7rem; flex: 1;">
                    <strong>PhysicsUpdate</strong>
                </div>
            </div>
            <!-- Loop Arrow -->
            <div style="position: absolute; top: -15px; left: 50%; transform: translateX(-50%); width: 40px; height: 10px; border: 2px solid #007bff; border-bottom: none; border-radius: 10px 10px 0 0;"></div>
        </div>
        
        <div style="color: #007bff; font-weight: bold; margin: 0 15px; font-size: 1.2rem;">→</div>
        
        <!-- Decision -->
        <div style="background: rgba(255, 123, 114, 0.05); border: 2px solid #ff7b72; border-radius: 50%; width: 120px; height: 120px; display: flex; align-items: center; justify-content: center; text-align: center; flex: 0 0 120px; font-size: 0.75rem; font-weight: 700; color: #ff7b72; font-family: 'Fira Code';">
            Trigger<br>Transition?
        </div>
        
        <div style="color: #007bff; font-weight: bold; margin: 0 15px; font-size: 1.2rem; position: relative;">
            <span style="position: absolute; top: -20px; left: 0; font-size: 0.7rem; color: #007bff;">[YES]</span>
            →
        </div>
        
        <!-- Exit & Transition -->
        <div style="display: flex; flex-direction: column; gap: 10px;">
            <div style="background: rgba(0, 123, 255, 0.05); border: 1px solid #007bff; border-radius: 6px; padding: 8px 15px; text-align: center; font-size: 0.75rem; color: #333; font-family: 'Fira Code';">
                State::Exit
            </div>
            <div style="background: #007bff; color: #fff; border-radius: 6px; padding: 8px 15px; text-align: center; font-size: 0.75rem; font-weight: 700; font-family: 'Fira Code';">
                NewState::Enter
            </div>
        </div>
    </div>
</div>

**상태머신 핵심 로직**
상태머신 초기화 및 안전한 상태 전환을 보장하는 핵심 로직입니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: FiniteStateMachine.cs</summary>

```csharp
// FiniteStateMachine.cs: 상태머신 핵심 로직
public class FiniteStateMachine
{
    public State currentState { get; private set; } 

    // 상태머신 초기화: 시작 상태 설정 및 Enter() 호출
    public void Init(State startingState) 
    {
        currentState = startingState;     // 현재 상태 설정
        currentState.Enter();             // 시작 상태 진입
    }

    // 상태 전환: 이전 상태 종료 후 새 상태 진입
    public void ChangeState(State newState) 
    {
        currentState.Exit();                // 이전 상태 종료
        currentState = newState;            // 새 상태로 전환
        currentState.Enter();               // 새 상태 진입
    }
}
```
</details>

### 2.2 State 베이스 클래스

모든 상태는 `State` 베이스 클래스를 상속받아 구현됩니다. 각 상태는 애니메이션 파라미터 제어와 상태 시작 시간 기록 등을 자동으로 처리합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: State.cs</summary>

```csharp
// State.cs: 모든 상태의 베이스 클래스
public class State
{
    protected FiniteStateMachine stateMachine;  // 상태머신 참조
    protected Entity entity;                    // 엔티티 참조
    protected float startTime;                  // 상태 시작 시간
    protected string animBoolName;              // 애니메이션 파라미터 이름

    public virtual void Enter()                     // 상태 진입 시 호출
    {
        startTime = Time.time;                      // 시작 시간 기록
        entity.anim.SetBool(animBoolName, true);    // 애니메이션 트리거
        DoChecks();                                 // 상태 전환 조건 확인
    }
    
    public virtual void Exit()                     // 상태 종료 시 호출
    {
        entity.anim.SetBool(animBoolName, false);   // 애니메이션 종료
    }
    
    public virtual void LogicUpdate()         // Update()에서 호출
    {
    }
    
    public virtual void PhysicsUpdate()      // FixedUpdate()에서 호출
    {
        DoChecks();                                 // 물리 업데이트마다 조건 확인
    }

    public virtual void DoChecks()          // 상태 전환 조건 확인
    {
        // 플레이어 감지, 벽/절벽 감지 등
    }
}
```
</details>

---

## 3. 플레이어 슬라임 및 변신 시스템
{: .chapter-title }

플레이어 슬라임의 움직임, 공격, 대시, 피격 처리를 구현했습니다. **컴포지션 패턴**을 활용하여 각 기능을 독립적인 클래스로 분리하고, `Player_Slime`에서 통합 관리합니다.

### 3.1 시스템 구조 (Composition Pattern)

`Player_Slime`은 모든 컴포넌트의 허브 역할을 수행하며, 각 기능 모듈은 자신의 도메인 로직에 집중합니다.

<div class="pf-visual-frame">
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">Player_Slime (Main Controller)</div>
            <div class="pf-arch-layer-items">
                <span class="pf-arch-item">Player_Entity Inherited</span>
                <span class="pf-arch-item">Component Hub</span>
            </div>
        </div>
        <div class="pf-flow-arrow" style="text-align: center;">↓ Dispatch</div>
        <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 15px;">
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">SlimeMovement</div>
                <div style="font-size: 0.8rem; color: #666;">이동, 점프, 입력 관리</div>
            </div>
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">SlimeAttack</div>
                <div style="font-size: 0.8rem; color: #666;">근접 공격, 데미지 트리거</div>
            </div>
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">SlimeDash</div>
                <div style="font-size: 0.8rem; color: #666;">대시 이동, 무적 프레임</div>
            </div>
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">SlimeDead</div>
                <div style="font-size: 0.8rem; color: #666;">사망 처리, 게임오버 연동</div>
            </div>
        </div>
    </div>
</div>

### 3.2 변신 능력 시스템

플레이어가 몬스터를 처치하면 해당 몬스터의 형태로 변신할 수 있습니다. 변신 시 각 형태의 고유 능력을 사용할 수 있으며, `Player_FormMovement`를 통해 특수 이동 로직이 활성화됩니다.

*   **슬라임 (기본):** 원거리 포물선 공격, 대시 무적 프레임 활용.
*   **토끼 (변신):** 더블 점프 및 벽 점프 능력 획득. `Rabbit_ability`와 연동하여 벽 점프 중 조작 제한 물리 로직 적용.

<details class="pf-details" markdown="1">
<summary>코드 보기: Player_Slime.cs - 초기화 로직</summary>

```csharp
// Player_Slime.cs: 플레이어 슬라임 메인 클래스
public class Player_Slime : Player_Entity
{
    public Player_SlimeMovement movement { get; private set; }
    public Player_Attack attack { get; private set; }
    public Player_Dash dash { get; private set; }

    protected override void Start()
    {
        base.Start();
        // 각 기능 컴포넌트 초기화 (Composition)
        movement = new Player_SlimeMovement(this, movementData, this);
        attack = new Player_Attack(this, attackData, meleePos);
        dash = new Player_Dash(this, dashData);
    }
}
```
</details>

---

## 4. 보스 및 일반 몬스터 AI 시스템
{: .chapter-title }

보스 몬스터의 스킬 시스템을 **델리게이트 기반**으로 구현하여 확장 가능한 AI를 설계했습니다. HP 단계에 따라 스킬이 업그레이드되며, 거리와 상황에 따라 지능적인 패턴을 생성합니다.

### 4.1 보스 AI 의사결정 및 스킬 시스템

보스는 매 업데이트마다 플레이어와의 거리를 산출하고, 델리게이트 배열에 등록된 스킬 풀(Pool)에서 최적의 패턴을 선택하여 실행합니다.

<div class="pf-visual-frame">
    <div class="pf-transaction-flow">
        <div class="pf-flow-step"><strong>1. Distance Calc</strong><br>플레이어 거리 및 방향 분석</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>2. Phase Check</strong><br>HP 50% 이하 스킬 업그레이드</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>3. Skill Select</strong><br>델리게이트 배열 랜덤 인덱싱</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>4. Action</strong><br>Invoke Skill Action</div>
    </div>
</div>

<details class="pf-details" markdown="1">
<summary>코드 보기: MainBossBear.cs - 델리게이트 기반 스킬 관리</summary>

```csharp
// MainBossBear.cs: 보스 스킬 시스템
public class MainBossBear : BossBearSetting
{
    private delegate void BossBear_SkillAction();
    private BossBear_SkillAction[] BossBear_Skills;

    public bool IsSkillUpgrade; // HP 단계 플래그

    public override void Start()
    {
        base.Start();
        // 델리게이트 배열에 메서드 직접 할당
        BossBear_Skills = new BossBear_SkillAction[4] { Rest, Smash, Dash, Rumbling };
    }

    private void RandomBossBearSkillState()
    {
        int RandomSkill_Index = Random.Range((int)BossBear_SkillEnum.Smash, (int)BossBear_SkillEnum.Skill_Max);
        // 선택된 스킬 메서드 대리 호출
        BossBear_Skills[RandomSkill_Index].Invoke();
    }
}
```
</details>

### 4.2 일반 몬스터 AI 확장 사례

일반 몬스터들은 공통의 `Enemy_State`를 상속받아 고유한 행동 패턴을 가집니다.

*   **Bear (돌진형):** `DashState`를 통한 강력한 물리 충격 기반 공격.
*   **Spider (매복형):** `JumpState`를 활용한 공중 도약 및 플레이어 타겟팅 낙하 공격.
*   **Ant (기본형):** 정형화된 순찰 및 근접 공격 루틴 수행.

---

## 5. 기술적 성과 요약
{: .chapter-title }

*   **FSM 아키텍처:** Enter/Exit 라이프사이클 관리를 통해 복잡한 상태 전이 시 발생할 수 있는 버그를 근본적으로 차단.
*   **컴포지션 기반 확장성:** 플레이어 기능을 모듈화하여 새로운 변신 형태나 능력을 추가할 때 기존 코드 수정을 최소화.
*   **델리게이트 기반 AI:** 보스의 스킬 패턴을 인덱스 기반으로 관리하여 패턴 추가 및 확률 조정의 유연성 확보.
*   **물리/애니메이션 동기화:** LogicUpdate와 PhysicsUpdate를 분리하여 프레임 드랍 환경에서도 일관된 충돌 판정 및 움직임 보장.

{: .notice--success}
**Conclusion:** Slime Project는 디자인 패턴을 실무에 어떻게 적용하여 대규모 게임 로직을 관리할 수 있는지에 대한 깊은 고민이 담긴 프로젝트입니다. FSM과 컴포지션을 통해 복잡한 액션 퍼즐의 메커니즘을 견고하게 구현해냈습니다.
