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

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- START -->
    <div class="pf-fc-pill-start">
      <span class="pf-fc-mono-tag">TRIGGER</span>
      <span>State Transition Request (상태 전환 요청)</span>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 01: State::Enter -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">1. State::Enter (상태 진입 및 초기화)</h4>
        <span class="pf-fc-badge">INIT</span>
      </div>
      <p class="pf-fc-desc">상태 진입 시 애니메이션 트리거, 물리 속도 초기화, 타이머 리셋</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 02: Execution Loop -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">2. Execution Loop (프레임 업데이트)</h4>
        <span class="pf-fc-badge" style="background: #eff6ff; color: #2563eb; border-color: #bfdbfe;">LOOP</span>
      </div>
      <p class="pf-fc-desc"><strong>LogicUpdate():</strong> 입력·AI 로직 연산 &nbsp;|&nbsp; <strong>PhysicsUpdate():</strong> 물리 이동 및 충돌 판정</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 03: Decision -->
    <div class="pf-fc-decision pf-fc-compact">
      <div class="pf-fc-decision-header">
        <span class="pf-fc-decision-badge">판단</span>
        <h4 class="pf-fc-decision-title">Trigger Transition? (전환 조건 충족 여부)</h4>
      </div>
      <div class="pf-fc-branch-grid">
        <div class="pf-fc-branch-item pf-fc-item-compact">
          <span class="pf-fc-tag-no">No</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">현재 상태 유지 (Execution Loop 지속)</div>
        </div>
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #bbf7d0; background: #fdfffe;">
          <span class="pf-fc-tag-yes">Yes</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">ChangeState() 호출 &rarr; 상태 전환 착수</div>
        </div>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 04: Exit & Enter -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #bfdbfe; background: #f8fbff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1d4ed8;">3. State::Exit &amp; NewState::Enter (원자적 교체)</h4>
        <span class="pf-fc-badge">TRANSACTION</span>
      </div>
      <p class="pf-fc-desc">이전 상태 정리(`Exit`) 완료 후 즉시 다음 상태(`Enter`) 활성화로 물리·애니메이션 상태 결함 방지</p>
    </div>

    <!-- END -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-pill-end">
      <span class="pf-fc-mono-tag">STABLE</span>
      <span>예외 없는 안정적 FSM 상태 라이프사이클 완성</span>
    </div>

  </div>
</div>

**상태머신 핵심 로직**
상태머신 초기화 및 안전한 상태 전환을 보장하는 핵심 로직입니다.

<details class="pf-details">
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

<details class="pf-details">
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

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- Central Hub -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #3b82f6; background: #eff6ff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1d4ed8;">Player_Slime (중앙 제어 허브)</h4>
        <span class="pf-fc-badge" style="background: #dbeafe; color: #1e40af; border-color: #93c5fd;">CONTROLLER HUB</span>
      </div>
      <p class="pf-fc-desc">엔티티 생명주기 관리, 상태머신(FSM) 소유, 4대 도메인 컴포넌트 일괄 초기화 및 이벤트 디스패치</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- 4 Modules Grid -->
    <div class="pf-fc-branch-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); width: 100%;">
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">MOVEMENT</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">SlimeMovement</div>
        <div class="pf-fc-branch-desc">좌우 이동, 점프, 지면 감지 물리 로직</div>
      </div>
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">ATTACK</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">SlimeAttack</div>
        <div class="pf-fc-branch-desc">포물선 원거리 투사체, 근접 타격 판정</div>
      </div>
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">DASH</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">SlimeDash</div>
        <div class="pf-fc-branch-desc">순간 가속 대시 및 무적 프레임 제어</div>
      </div>
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">LIFECYCLE</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">SlimeDead</div>
        <div class="pf-fc-branch-desc">사망 연출, 콜라이더 비활성화, UI 연동</div>
      </div>
    </div>

  </div>
</div>

### 3.2 변신 능력 시스템

플레이어가 몬스터를 처치하면 해당 몬스터의 형태로 변신할 수 있습니다. 변신 시 각 형태의 고유 능력을 사용할 수 있으며, `Player_FormMovement`를 통해 특수 이동 로직이 활성화됩니다.

*   **슬라임 (기본):** 원거리 포물선 공격, 대시 무적 프레임 활용.
*   **토끼 (변신):** 더블 점프 및 벽 점프 능력 획득. `Rabbit_ability`와 연동하여 벽 점프 중 조작 제한 물리 로직 적용.

<details class="pf-details">
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

<details class="pf-details">
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
