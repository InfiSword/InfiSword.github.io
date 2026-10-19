---
title: "PROJECT REPORT // SLIME PROJECT"
excerpt: "상태머신 기반 게임 로직 및 확장 가능한 몬스터 AI 시스템"
permalink: "/project/slime-project/"
tags: [Unity, AI, FSM, Design Pattern]
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

Slime Project는 몬스터의 형태로 변신하며 전투와 지형을 공략하는 2D 액션 퍼즐 플랫포머입니다. **몬스터 AI의 상태 관리와 플레이어의 이동·전투·변신 기능**을 담당했습니다. 행동의 공통 규칙과 대상별 기능을 나누어, 새로운 몬스터와 능력을 확장할 수 있도록 구성했습니다.

<section id="gallery" class="pf-ftd-post-gallery-section">
    <div class="pf-ftd-section-header">
        <span class="pf-ftd-section-tag pf-mono">● IN-GAME SCREENSHOT GALLERY</span>
        <h3 class="pf-ftd-section-title">인게임 핵심 시스템 & 스크린샷 미리보기</h3>
        <p class="pf-ftd-section-desc">플레이어 슬라임의 조작과 몬스터 조우, 토끼 몬스터 타격 액션부터 거대 곰 보스 몬스터와의 페이즈별 전투까지 주요 인게임 화면을 넘겨가며 옆으로 스크롤하여 확인할 수 있습니다.</p>
    </div>

    <!-- INTERACTIVE SCREENSHOT GALLERY (HORIZONTAL SCROLL & SLIDER) -->
    <div class="pf-ftd-gallery" id="pfSlimeGalleryPost">
        <!-- Topbar Meta & Actions -->
        <div class="pf-ftd-gallery-topbar">
            <div class="pf-ftd-gallery-meta">
                <span class="pf-ftd-counter pf-mono">01 / 04</span>
                <span class="pf-ftd-caption">거대 곰 보스 몬스터와의 전투 및 패턴 공략</span>
            </div>
            <a href="https://www.youtube.com/watch?v=83lMfQY1Rwo" target="_blank" rel="noopener noreferrer" class="pf-ftd-yt-link" title="YouTube 시연 영상 새 창으로 열기">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z"/></svg>
                <span>YouTube 시연 영상 ↗</span>
            </a>
        </div>

        <!-- Main Viewing Stage -->
        <div class="pf-ftd-stage">
            <button type="button" class="pf-ftd-nav pf-ftd-prev" onclick="pfFtdGalleryGo(-1, this)" aria-label="이전 스크린샷">‹</button>
            <img class="pf-ftd-main-img" src="/assets/images/Slime/InGame/slime_ingame_01.png" alt="거대 곰 보스 몬스터와의 전투 및 패턴 공략" loading="eager">
            <button type="button" class="pf-ftd-nav pf-ftd-next" onclick="pfFtdGalleryGo(1, this)" aria-label="다음 스크린샷">›</button>
        </div>

        <!-- Horizontal Scrollable Thumbnail Strip -->
        <div class="pf-ftd-thumbs-wrapper">
            <div class="pf-ftd-thumbs-strip" role="region" aria-label="스크린샷 썸네일 가로 스크롤 목록">
                <button type="button" class="pf-ftd-thumb-card is-active" data-index="0" data-title="거대 곰 보스 몬스터와의 전투 및 패턴 공략" onclick="pfFtdGallerySelect(0, this)" title="1. 거대 곰 보스 몬스터와의 전투 및 패턴 공략">
                    <img src="/assets/images/Slime/InGame/slime_ingame_01.png" alt="1. 거대 곰 보스 몬스터와의 전투 및 패턴 공략" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">01</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="1" data-title="필드 탐색 및 기본 조작·몬스터 조우" onclick="pfFtdGallerySelect(1, this)" title="2. 필드 탐색 및 기본 조작·몬스터 조우">
                    <img src="/assets/images/Slime/InGame/slime_ingame_02.png" alt="2. 필드 탐색 및 기본 조작·몬스터 조우" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">02</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="2" data-title="토끼 몬스터 사냥 및 타격 액션 이펙트" onclick="pfFtdGallerySelect(2, this)" title="3. 토끼 몬스터 사냥 및 타격 액션 이펙트">
                    <img src="/assets/images/Slime/InGame/slime_ingame_03.png" alt="3. 토끼 몬스터 사냥 및 타격 액션 이펙트" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">03</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="3" data-title="플랫포머 기믹 지형 돌파 및 몬스터 전투" onclick="pfFtdGallerySelect(3, this)" title="4. 플랫포머 기믹 지형 돌파 및 몬스터 전투">
                    <img src="/assets/images/Slime/InGame/slime_ingame_04.png" alt="4. 플랫포머 기믹 지형 돌파 및 몬스터 전투" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">04</span>
                </button>
            </div>
        </div>
        <div class="pf-ftd-hint">
            <span>Tip: 좌우 화살표 버튼, 키보드 방향키(←, →), 또는 마우스 휠로 썸네일을 옆으로 스크롤하여 탐색할 수 있습니다.</span>
        </div>
    </div>
</section>

---

## 1. 프로젝트 개요
{: .chapter-title }

몬스터마다 다른 행동 패턴과 변신에 따라 달라지는 플레이어 능력을 관리하는 것이 핵심 과제였습니다. 몬스터는 **상태 전환**을 중심으로, 플레이어는 **기능 조합**을 중심으로 구성하고, 두 구조가 사용하는 상태 생명주기를 공통 기반으로 정리했습니다.

---

## 2. FiniteStateMachine 기반 상태 관리 시스템
{: .chapter-title }

### 2.1 행동 상태와 전환 관리

몬스터의 순찰·탐색·공격을 독립된 상태로 분리하고, FSM이 현재 상태와 전환을 관리하도록 했습니다. 상태를 바꿀 때는 **기존 상태 종료 → 새 상태 지정 → 새 상태 진입** 순서로 처리합니다. 각 상태에 행동과 전환 판단을 모아, 패턴을 수정할 때 변경 범위를 해당 상태에 집중했습니다.

<details class="pf-details pf-shared-state-code">
<summary>코드 보기: FSM 상태 전환</summary>

```csharp
// FiniteStateMachine.cs
public class FiniteStateMachine
{
    public State currentState { get; private set; }

    public void Init(State startingState) // 유한상태머신을 초기화한다는 의미
    {
        currentState = startingState;     // 현재 상태 설정
        currentState.Enter();             // 그 상태를 실행
    }

    public void ChangeState(State newState) // 상태 변경
    {
        currentState.Exit();                // 전의 상태를 종료하고
        currentState = newState;            // 현재 상태를 업데이트
        currentState.Enter();               // 현재 상태에 돌입
    }
}
```
</details>

### 2.2 State 베이스 클래스

몬스터와 플레이어가 공통 `State`를 상속하도록 구성했습니다. **상태의 진입·실행·종료 규칙을 공유**하고, 몬스터와 플레이어에 필요한 참조와 처리는 각 하위 클래스에서 확장합니다. 공유 범위는 상태의 기본 규칙이며, 실제 행동과 실행 흐름은 대상별로 구성했습니다.

| 구분 | 핵심 역할 |
| --- | --- |
| State | 상태의 공통 생명주기 정의 |
| Monster_State | 몬스터 참조와 감지·애니메이션 처리 공유 |
| Player_State | 플레이어 참조를 공유하는 기능별 상태의 기반 |

몬스터의 행동 전환은 FSM이, 플레이어의 기능 조합은 플레이어 컨트롤러가 관리합니다.

<details class="pf-details pf-shared-state-code">
<summary>코드 보기: 공통 State와 플레이어 분기</summary>

```csharp
// State.cs
public class State
{
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void DoChecks() { }
}

// Player_State.cs
public class Player_State : State
{
    protected Player_Entity entity;

    public Player_State(Player_Entity entity)
    {
        this.entity = entity;
    }
}
```
</details>

<details class="pf-details pf-shared-state-code">
<summary>코드 보기: 몬스터 상태 진입·종료</summary>

```csharp
// Monster_State.cs: 공통 참조와 진입·종료 처리 발췌
public class Monster_State : State
{
    protected FiniteStateMachine stateMachine;  // 상태머신
    protected Monster_Entity entity;                    // 객체 (몬스터의 부모 객체)

    protected float startTime;                  // 상태 시작 시간.

    protected string animBoolName;              // 애니메이션에 따른 파라미터 이름

    public Monster_State(FiniteStateMachine stateMachine, Monster_Entity entity, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.entity = entity;
        this.animBoolName = animBoolName;
    }

    public override void Enter()                     // 어떤 상태에 돌입했을 시,
    {
        startTime = Time.time;                      // 스타트 시간을 현재시간으로 설정
        entity.anim.SetBool(animBoolName, true);    // 파라미터세팅
        DoChecks();                                 // 체크해야될 요소 확인

        Debug.Log("Enter: " +  animBoolName);
    }
    public override void Exit()
    {
        entity.anim.SetBool(animBoolName, false);   // 애니메이션 종료
    }
}
```
</details>

### 2.3 행동 재사용과 확장

이동·공격 등 반복되는 행동을 공통 상태로 만들고, 이를 바탕으로 곰의 돌진과 거미의 도약 같은 개별 패턴을 확장했습니다. 속도와 행동 시간은 별도 데이터로 관리해 **행동 규칙과 조정값을 분리**했습니다. 공통 처리는 재사용하면서 몬스터별 패턴과 수치를 각각 조정할 수 있도록 했습니다.

<details class="pf-details pf-shared-state-code">
<summary>코드 보기: 공통 이동 로직</summary>

```csharp
// MoveState.cs: 데이터에 따른 이동 시작·시간 판단 발췌
public override void Enter()
{
    base.Enter();
    entity.SetVelocity(stateData.movementSpeed);        // move상태로 들어가면, 이동시작
    isMoveTimeOver = false;
    SetRandomMoveTime();
}

public override void LogicUpdate()
{
    base.LogicUpdate();
    if (Time.time >= startTime + moveTime)   // move가 끝났다면
    {
        isMoveTimeOver = true;              //  move가 끝났다는 bool을 실행
    }
}
```
</details>

---

## 3. 플레이어 슬라임 및 변신 시스템
{: .chapter-title }

### 3.1 기능 단위 조합

플레이어의 이동·공격·대시를 개별 기능 객체로 나누고, 컨트롤러가 입력과 현재 상황에 맞춰 호출하도록 구성했습니다. 각 기능이 자신의 처리를 담당하므로, 플레이어의 전체 로직을 한곳에 모으지 않고 동작 단위로 관리할 수 있습니다.

<details class="pf-details pf-shared-state-code">
<summary>코드 보기: 플레이어 기능 조합</summary>

```csharp
// Player_Slime.Start(): 기능 객체 초기화 부분 발췌
movement = new Player_SlimeMovement(this, movementData, this);
attack = new Player_Attack(this, attackData, meleePos);
dash = new Player_Dash(this, dashData);
dead = new Player_Dead(this, deadData);
```
</details>

### 3.2 변신 능력 시스템

몬스터를 처치한 뒤 해당 형태로 변신해 고유 능력을 활용합니다. 기본 슬라임은 원거리 공격과 대시를, 토끼 형태는 더블 점프와 벽 점프를 사용합니다. 형태별 능력 차이가 전투 방식과 이동 경로 선택으로 이어지도록 구성했습니다.
