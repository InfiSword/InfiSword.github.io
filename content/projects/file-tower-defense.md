---
title: "PROJECT REPORT // FILE_TOWER_DEFENSE"
excerpt: "Systems Engineering: UI-to-GameObject 리팩토링, 통합 입력 시스템 및 그리드 아키텍처"
permalink: "/project/file-tower-defense/"
tags: [Unity, Optimization, Architecture, Algorithms]
mermaid: true
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
    .pf-diagram-grid {
        display: grid; grid-template-columns: repeat(3, 45px); gap: 8px; justify-content: center; margin: 15px 0;
    }
    .pf-grid-cell { width: 45px; height: 45px; border: 1px solid #58A6FF; opacity: 0.2; border-radius: 4px; }
    .pf-grid-cell.active { background: #58A6FF; opacity: 1; box-shadow: 0 0 15px #58A6FF; }
    .pf-grid-cell.near { border-color: #28a745; background: rgba(40, 167, 69, 0.15); opacity: 1; }
    
    .pf-logic-container {
        display: flex; flex-direction: column; gap: 10px; margin: 20px 0; text-align: left;
    }
    .pf-logic-row {
        display: grid; grid-template-columns: 120px 1fr; gap: 15px; align-items: center;
        padding: 15px; border-radius: 8px; background: #fff; border: 1px solid #eee;
    }
    .pf-logic-label {
        font-weight: 700; color: #007bff; font-family: 'Fira Code', monospace; text-align: center;
        background: rgba(0, 123, 255, 0.05); padding: 5px; border-radius: 4px;
    }

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
    .details-desc {
        padding: 15px 20px;
        background: #fff;
        color: #666;
        font-size: 0.95rem;
        border-top: 1px solid #e1e4e8;
        line-height: 1.6;
    }

    /* --- Transaction Flow --- */
    .pf-transaction-flow {
        display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-family: 'Fira Code', monospace;
        gap: 12px; flex-wrap: wrap; margin: 20px 0;
    }
    .pf-flow-step { padding: 10px 18px; border: 1px solid #e1e4e8; border-radius: 6px; background: #fff; color: #333; text-align: center; box-shadow: 0 2px 5px rgba(0,0,0,0.05); line-height: 1.4; }
    .pf-flow-arrow { color: #007bff; font-weight: bold; font-size: 1.2rem; }

    /* --- Tables --- */
    .pf-table-wrapper { display: block !important; width: 100% !important; overflow-x: auto; margin: 20px 0; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
    .pf-data-table {
        width: 100% !important; border-collapse: collapse; font-size: 0.9rem;
        background: #fff; border: 1px solid #e1e4e8; table-layout: fixed;
    }
    .pf-data-table th {
        background: rgba(0, 123, 255, 0.08); color: #007bff;
        padding: 15px; text-align: center; font-weight: 700; border-bottom: 2px solid #e1e4e8;
        font-family: 'Fira Code', monospace;
    }
    .pf-data-table td {
        padding: 12px 15px; border-bottom: 1px solid #e1e4e8; color: #555;
        text-align: center; line-height: 1.5;
    }
    .pf-data-table tr:last-child td { border-bottom: none; }
    .pf-data-table tr:hover { background: rgba(0, 123, 255, 0.02); }

    /* Coordinate Flow */
    .pf-coord-flow {
        display: flex; gap: 15px; margin: 25px 0;
        align-items: stretch; overflow-x: auto; padding-bottom: 10px;
    }
    .pf-coord-box {
        padding: 18px; border: 2px solid #007bff; border-radius: 8px;
        background: rgba(0, 123, 255, 0.02); text-align: center; display: flex; flex-direction: column;
        justify-content: center; min-height: 110px; min-width: 190px; flex: 0 0 auto;
    }
    .pf-coord-box-title {
        color: #007bff; font-weight: 700; margin-bottom: 10px;
        font-family: 'Fira Code', monospace; font-size: 0.9rem; border-bottom: 1px solid #eee;
        padding-bottom: 8px;
    }
    .pf-coord-box-formula {
        color: #28a745; font-size: 0.75rem; font-family: 'Fira Code', monospace;
        margin-top: 8px; padding: 6px; background: rgba(40, 167, 69, 0.05); border-radius: 4px;
    }
    
    /* Comparison Boxes */
    .pf-comp-container { display: flex; justify-content: space-around; align-items: center; flex-wrap: wrap; gap: 20px; }
    .pf-comp-box { font-family: 'Fira Code', monospace; font-size: 0.8rem; border-radius: 8px; padding: 15px; min-width: 200px; text-align: left; line-height: 1.6; }
    .pf-comp-box.old { border: 1px solid #ff7b72; background: rgba(255, 123, 114, 0.05); color: #ff7b72; }
    .pf-comp-box.new { border: 1px solid #28a745; background: rgba(40, 167, 69, 0.05); color: #28a745; }
</style>

**File Tower Defense** 프로젝트의 코어 시스템 설계를 담당하며, 유니티의 좌표계 특성을 분석한 아키텍처 재설계부터 대규모 객체의 입력 처리, 동적 버프 시스템, 그리고 그리드 기반 배치 로직까지 전반적인 시스템 개선 과정을 기록한 리포트입니다.

---

## 1. 시스템 리팩토링 및 개선
{: .chapter-title }

### 1.1 UI(RectTransform)에서 GameObject(Transform) 기반 전환

초기 설계에서는 윈도우 바탕화면의 아이콘 느낌을 살리기 위해 모든 파일 유닛을 UI 시스템(`RectTransform`)으로 구축했습니다. 그러나 프로젝트가 고도화됨에 따라 다음과 같은 한계에 직면했습니다.

*   **좌표계 종속성:** 캔버스의 앵커/피벗 설정 및 해상도에 따라 월드 좌표가 상대적으로 변하여 게임 월드 내 정밀한 위치 계산과 사거리 판정이 매우 까다로움.
*   **연산 오버헤드 (Canvas Rebuilding):** 유닛의 움직임, 호버 효과, 드래그 등의 UI 요소가 갱신될 때마다 Unity 내부에서 Canvas Rebuild가 강제로 유발되어 CPU 프레임 드랍 발생.
*   **물리 엔진의 부재:** 월드 좌표 기반의 바이러스(적)와 UI 유닛 간 물리 충돌 판정을 위해 매 프레임 `Camera.WorldToScreenPoint` 등의 물리-화면 좌표 변환 함수 호출 비용 발생.

<div class="pf-visual-frame pf-comp-frame">
  <div class="pf-comp-container">
    <!-- 기존 방식 (AS-IS) -->
    <div class="pf-comp-box old">
      <div class="pf-comp-header">
        <span class="pf-comp-badge old">기존 방식 (AS-IS)</span>
        <h4 class="pf-comp-title">RectTransform (UI)</h4>
      </div>
      <ul class="pf-comp-list">
        <li>
          <span class="pf-comp-icon old">✕</span>
          <span class="pf-comp-text">앵커·피벗 기반 좌표 종속성</span>
        </li>
        <li>
          <span class="pf-comp-icon old">✕</span>
          <span class="pf-comp-text">빈번한 Canvas Rebuild 연산 부하</span>
        </li>
        <li>
          <span class="pf-comp-icon old">✕</span>
          <span class="pf-comp-text">과도한 화면-월드 좌표 변환 비용</span>
        </li>
      </ul>
    </div>

    <!-- 전환 브릿지 (REFACTORING) -->
    <div class="pf-comp-bridge">
      <span class="pf-bridge-pill">REFACTORING</span>
      <div class="pf-bridge-arrow">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="5" y1="12" x2="19" y2="12"></line>
          <polyline points="12 5 19 12 12 19"></polyline>
        </svg>
      </div>
    </div>

    <!-- 개선 방식 (TO-BE) -->
    <div class="pf-comp-box new">
      <div class="pf-comp-header">
        <span class="pf-comp-badge new">개선 방식 (TO-BE)</span>
        <h4 class="pf-comp-title">Transform (World)</h4>
      </div>
      <ul class="pf-comp-list">
        <li>
          <span class="pf-comp-icon new">✓</span>
          <span class="pf-comp-text">독립적인 절대 2D 직교 좌표계</span>
        </li>
        <li>
          <span class="pf-comp-icon new">✓</span>
          <span class="pf-comp-text">Canvas Rebuild 오버헤드 0% (완전 배제)</span>
        </li>
        <li>
          <span class="pf-comp-icon new">✓</span>
          <span class="pf-comp-text">Physics 2D 물리 엔진 및 콜라이더 직결</span>
        </li>
      </ul>
    </div>
  </div>
</div>
        



이를 해결하기 위해 모든 유닛을 **GameObject(Transform) 기반**으로 전면 교체하여 월드 좌표계로 통일하였으며, Physics2D와의 직접적인 호환성을 확보하여 불필요한 연산을 제거하고 성능을 극적으로 최적화했습니다.

### 1.2 GameObjectGridLayout: 커스텀 레이아웃 엔진

화면 해상도에 맞춰 그리드의 간격과 셀 크기를 동적으로 계산하는 레이아웃 엔진을 구축했습니다. 유니티의 기본 `GridLayoutGroup`은 UI(Canvas) 전용 컴포넌트이므로, 일반 월드 공간 GameObject를 위해 카메라 크기에 맞게 자동으로 셀 크기를 스케일링하고 하단 작업표시줄 영역을 확보하는 커스텀 컴포넌트 `GameObjectGridLayout`을 직접 개발했습니다.

<div class="pf-visual-frame">
  <div class="pf-grid" style="grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 20px; align-items: start;">
    <!-- 컴포넌트 인스펙터 설정 -->
    <div style="text-align: center;">
      <img src="/assets/images/File%20Tower%20Defences/grid_layout_inspector.png" alt="GameObjectGridLayout Inspector Component" style="width: 100%; border-radius: 10px; border: 1px solid rgba(148, 163, 184, 0.25); box-shadow: 0 4px 16px rgba(0,0,0,0.06);">
      <div style="color: #64748b; font-size: 0.85rem; margin-top: 10px; font-weight: 500; font-family: 'Fira Code', monospace; line-height: 1.5;">
        <strong>[컴포넌트]</strong> GameObjectGridLayout 인스펙터 설정<br>
        <span style="font-size: 0.8rem; color: #94a3b8;">(Columns: 14, Rows: 8, Fit To Screen: On, Bottom Offset Ratio: 0.1)</span>
      </div>
    </div>

    <!-- 실제 인게임 월드 그리드 시각화 -->
    <div style="text-align: center;">
      <img src="/assets/images/File%20Tower%20Defences/grid_layout_scene.png" alt="In-Game World Grid Visualization in Scene View" style="width: 100%; border-radius: 10px; border: 1px solid rgba(148, 163, 184, 0.25); box-shadow: 0 4px 16px rgba(0,0,0,0.06);">
      <div style="color: #64748b; font-size: 0.85rem; margin-top: 10px; font-weight: 500; font-family: 'Fira Code', monospace; line-height: 1.5;">
        <strong>[실제 그리드]</strong> 초록색 격자선 = 인게임 실제 월드 그리드<br>
        <span style="font-size: 0.8rem; color: #2563eb; font-weight: 600;">(하단 UI 여백 10% 제외 후 가용 영역 14×8 셀 자동 피팅)</span>
      </div>
    </div>
  </div>
</div>

*   **컴포넌트 중심의 유연한 제어:** 에디터 인스펙터에서 행과 열(`Columns: 14`, `Rows: 8`), 패딩 및 스페이싱을 설정하면 오르토그래픽 카메라의 가로·세로 월드 크기를 실시간 계산하여 최적의 셀 크기(`Cell Size`)를 자동으로 도출합니다.
*   **실제 초록색 그리드(World Grid) 영역:** 씬 뷰 화면에 초록색 라인으로 표시되는 격자가 **실제 인게임 파일 유닛과 바이러스가 배치·이동하는 물리적 그리드**입니다. `Bottom Offset Ratio(0.1)` 설정을 통해 하단 작업표시줄 UI 영역(약 10%)을 비워두고, 상단 가용 영역에만 정확히 14×8 격자가 피팅되도록 중심점을 자동 보정합니다.

---

## 2. 파일/바이러스/UI 입력 및 상호작용 시스템
{: .chapter-title }

수많은 유닛이 각자 `Update()`에서 마우스 충돌을 검사하는 것은 비효율적이고 관리하기 어렵다고 판단했습니다. 이를 개선하기 위해 **Mediator 패턴**을 도입하여 **InputManager**가 모든 마우스 입력을 중앙 집중식으로 수집·판정하고, 상호작용 중재자인 **InteractionHandler**가 유닛의 선택 및 드래그 상태를 통합 관리하며, **IInteractable** 인터페이스를 구현한 대상 객체에게만 이벤트를 안전하게 전파하도록 설계했습니다.

### 2.1 중앙 집중식 입력 아키텍처

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- 0. START -->
    <div class="pf-fc-pill-start">
      <span>🖱️</span>
      <span>Mouse &amp; Touch Input</span>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 01: InputManager -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">InputManager (입력 수신 &amp; 좌표 보정)</h4>
        <span class="pf-fc-badge">CONTROLLER</span>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 02: 조건 판단 1 (UI 판정 & Raycast) -->
    <div class="pf-fc-decision pf-fc-compact">
      <div class="pf-fc-decision-header">
        <span class="pf-fc-decision-badge">판단 01</span>
        <h4 class="pf-fc-decision-title">Is Pointer Over UI? (UI 포인터 검사)</h4>
      </div>
      <div class="pf-fc-branch-grid">
        <!-- YES 분기 -->
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #fecaca; background: #fffdfd;">
          <span class="pf-fc-tag-yes">Yes</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">UI 우선 처리 (월드 입력 차단)</div>
        </div>
        <!-- NO 분기 -->
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #bbf7d0; background: #fdfffe;">
          <span class="pf-fc-tag-no">No</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">Raycast &amp; Sorting Order 정렬 ➔ Target 식별</div>
        </div>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 03: 조건 판단 2 (InputState FSM) -->
    <div class="pf-fc-decision pf-fc-compact">
      <div class="pf-fc-decision-header">
        <span class="pf-fc-decision-badge">판단 02</span>
        <h4 class="pf-fc-decision-title">InputState 머신 판정</h4>
      </div>
      <div class="pf-fc-branch-grid">
        <!-- 1. 클릭 -->
        <div class="pf-fc-branch-item pf-fc-item-compact">
          <span class="pf-fc-tag-state">Pressing</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">클릭 / 호버 (Threshold 검사)</div>
        </div>
        <!-- 2. 드래그 -->
        <div class="pf-fc-branch-item pf-fc-item-compact">
          <span class="pf-fc-tag-state">DraggingObject</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">단일 유닛 드래그 (OnDrag)</div>
        </div>
        <!-- 3. 다중 선택 -->
        <div class="pf-fc-branch-item pf-fc-item-compact">
          <span class="pf-fc-tag-state">DraggingBox</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">다중 선택 박스 (Ctrl+드래그)</div>
        </div>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 04: InteractionHandler -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">InteractionHandler (이벤트 일괄 디스패치)</h4>
        <span class="pf-fc-badge" style="background: #ecfdf5; color: #059669; border-color: #a7f3d0;">MEDIATOR</span>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 05: IInteractable Objects -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #bfdbfe; background: #f8fbff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1d4ed8;">IInteractable Objects (OnClick / OnDrag / OnSelected 실행)</h4>
        <span class="pf-fc-badge">EXECUTION</span>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- 6. END / RESULT -->
    <div class="pf-fc-pill-end">
      <span>⚡</span>
      <span>개별 객체 Update() 0% &amp; 60+ FPS 방어</span>
    </div>

  </div>
</div>

<div class="pf-visual-frame">
    <img src="/assets/Gifs/file-tower-defense-interaction.gif" alt="File Tower Defense Input Interaction Demo" style="width: 100%; border-radius: 8px;">
    <p style="color: #666; font-size: 0.85rem; margin-top: 10px; font-family: 'Fira Code', monospace;">[DEMO] InputManager 기반 마우스 호버 및 클릭 감지</p>
</div>

#### 객체 중첩 판정 알고리즘
마우스 아래에 여러 유닛이나 바이러스, 혹은 UI 판정이 겹쳐있을 때, 다음과 같은 우선순위 기준을 사용하여 모호함을 해결합니다.
1.  **UI 레이어 우선:** `EventSystem.current.IsPointerOverGameObject()`를 통해 UI 상호작용 우선 처리.
2.  **물리 레이어 필터링:** `Physics2D.RaycastNonAlloc`을 활용하여 쓰레기 메모리(GC Alloc) 없이 감지된 콜라이더 배열을 가져옴.
3.  **정렬 레이어(Sorting Order) 정렬:** 감지된 객체들의 `SpriteRenderer` 내 `Sorting Layer ID` 및 `Order in Layer` 값을 비교하여 카메라와 가장 가까운(최상단에 렌더링된) 오브젝트를 타깃으로 최종 선택.

### 2.2 IInteractable 인터페이스 기반 확장성

클릭 및 드래그 상호작용이 필요한 모든 인게임 오브젝트(유닛 파일, 바이러스 등)는 **IInteractable** 인터페이스를 상속받아 유연하게 확장할 수 있습니다.

<div class="pf-visual-frame">
    <img src="/assets/Gifs/file-tower-defense-drag.gif" alt="File Tower Defense Mouse Drag Event Demo" style="width: 100%; border-radius: 8px;">
    <p style="color: #666; font-size: 0.85rem; margin-top: 10px; font-family: 'Fira Code', monospace;">[DEMO] IInteractable 상속 객체의 실시간 마우스 드래그 & 배치 흐름</p>
</div>

<details class="pf-details">
<summary>코드 보기: IInteractable 인터페이스</summary>

```csharp
// IInteractable.cs: 인게임 오브젝트(파일/바이러스) 통합 입력 인터페이스
public interface IInteractable
{
    GameObject targetObj { get; }
    Transform transform { get; }
    Sprite TooltipImg { get; }  // 툴팁은 클래스로 만들까 고민
    string ToolTipDes { get; }  
    
    // 선택 상태 관리
    bool IsSelectable { get; }
    bool IsDraggable { get; }
    bool IsTooltipEnabled { get; }
    
    // 이벤트 메서드
    void OnHoverEnter();     // 마우스가 객체 위로 올라올 때 (ToolTip 타이밍 리셋용)
    void OnHoverExit();      // 마우스가 객체에서 벗어날 때 (ToolTip 숨김)
    void OnClickEnter();     // 마우스 버튼을 누를 때
    void OnClickExit();      // 마우스 버튼을 뗄 때
    void OnBeginDrag();
    void OnDrag(Vector2 mouseDelta); 
    void OnEndDrag();
    void OnClick();          // 클릭 처리
    void OnDoubleClick();    // 더블클릭 처리
    void OnRightClick();     // 우클릭 처리
    void OnSelectSingle();

    // 선택 상태 표시
    void OnSelected(bool isSelected);
}
```
</details>

---

## 3. 그리드 기반 오브젝트 배치 시스템
{: .chapter-title }

### 3.1 FileGrid: 데이터 중심의 지능형 셀 매니저

**FileGrid**는 단순한 위치 정보 홀더가 아니라, 유닛 배치 상태와 해당 셀에 작용 중인 오라(버프) 목록을 독자적으로 관리하는 지능형 컨테이너입니다.
*   **HashSet 기반 버프 소스 관리:** 현재 그리드 공간에 영향을 주는 버프 제공 유닛의 목록을 `HashSet<File_Base>`로 관리하여 버프의 중복 적용을 제거하고 $O(1)$의 빠른 조회 속도를 유지합니다.
*   **유닛 탈부착 시 자동 스탯 갱신:** 유닛이 배치되거나 이탈할 때 그리드에 축적된 버프 목록을 분석하여 대상 유닛의 스탯을 실시간으로 갱신해 줍니다.

### 3.2 원자적 배치 트랜잭션 (Transactional Pattern)

드래그 앤 드롭으로 유닛의 그리드 위치를 옮길 때 발생할 수 있는 데이터 불일치 및 예외 상황을 방지하기 위해, 데이터베이스의 트랜잭션 개념을 차용한 **원자적 배치(Atomic Placement) 파이프라인**을 구축했습니다. **탐색/검증(Validation) ➔ 확정(Commit) ➔ 복구(Rollback)** 흐름을 단일 프로세스로 통합하여 상태 결함을 원천 차단합니다.

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- 0. START -->
    <div class="pf-fc-pill-start">
      <span>📦</span>
      <span>Drag &amp; Drop Release (유닛 드롭)</span>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 01: 탐색 및 검증 -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">1. Search &amp; Validation (목표 셀 탐색 및 검증)</h4>
        <span class="pf-fc-badge">VALIDATE</span>
      </div>
      <p class="pf-fc-desc">드롭 좌표 기반 목표 그리드 식별 (인접 3x3 탐색) ➔ 유효 영역 및 장애물/기존 유닛 점유 여부 판정</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 02: 조건 분기 (Commit vs Rollback) -->
    <div class="pf-fc-decision pf-fc-compact">
      <div class="pf-fc-decision-header">
        <span class="pf-fc-decision-badge">분기 판정</span>
        <h4 class="pf-fc-decision-title">배치 가능 여부 판정 (Can Place Unit?)</h4>
      </div>
      <div class="pf-fc-branch-grid">
        <!-- COMMIT 분기 -->
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #bbf7d0; background: #fdfffe;">
          <span class="pf-fc-tag-no">Commit (검증 성공)</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">2. 트랜잭션 확정</div>
          <p class="pf-fc-branch-desc">• 이전 그리드 점유 해제 (OldGrid.Remove)<br>• 신규 그리드 점유 등록 (NewGrid.Set)<br>• 스탯 및 오라 버프 실시간 재계산<br>• 목표 셀 위치로 유닛 좌표 확정</p>
        </div>
        <!-- ROLLBACK 분기 -->
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #fecaca; background: #fffdfd;">
          <span class="pf-fc-tag-yes">Rollback (검증 실패)</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">3. 트랜잭션 복구</div>
          <p class="pf-fc-branch-desc">• 유효하지 않은 셀 또는 장애물 충돌<br>• 배치 트랜잭션 즉시 취소<br>• RestoreOriginalPosition() 호출<br>• 드래그 전 원래 그리드 위치로 안전 복원</p>
        </div>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- 4. END -->
    <div class="pf-fc-pill-end">
      <span>🔒</span>
      <span>원자성(Atomicity) 확보 &amp; 데이터 무결성 100% 유지</span>
    </div>

  </div>
</div>

### 3.3 공간 분할 기반 국소 탐색 시스템

마우스 드래그 좌표로부터 목표 그리드 인덱스를 수학적으로 산출하고, 해당 인덱스를 중심으로 **인접한 3×3 그리드 셀(총 9개)**만을 국소 탐색하여 최적의 그리드를 신속하게 판정합니다.

<div class="pf-visual-frame" style="padding: 28px 20px; background: #f8fbff; border: 1px solid #dce8f6; border-radius: 16px;">
  <!-- LOCAL 3x3 SEARCH MATRIX -->
  <div style="display: flex; flex-direction: column; align-items: center; justify-content: center;">
    <span style="font-family: 'Fira Code', monospace; font-size: 0.8rem; font-weight: 800; color: #2563eb; letter-spacing: 0.5px; margin-bottom: 16px;">LOCAL 3×3 SEARCH MATRIX</span>

    <div style="display: grid; grid-template-columns: repeat(3, 76px); grid-template-rows: repeat(3, 76px); gap: 8px;">
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(-1, +1)
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(0, +1)
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(+1, +1)
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(-1, 0)
      </div>
      <div style="background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%); border: 2px solid #1e40af; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; color: #ffffff; box-shadow: 0 4px 14px rgba(37,99,235,0.35);">
        <span style="font-size: 1.1rem; margin-bottom: 2px;">🎯</span>
        <span style="font-family: 'Fira Code', monospace; font-size: 0.8rem; font-weight: 800;">Target</span>
        <span style="font-size: 0.62rem; opacity: 0.9; font-family: 'Fira Code', monospace;">(0, 0)</span>
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(+1, 0)
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(-1, -1)
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(0, -1)
      </div>
      <div style="background: #f0f7ff; border: 1.5px solid #bfdbfe; border-radius: 10px; display: flex; flex-direction: column; align-items: center; justify-content: center; font-family: 'Fira Code', monospace; font-size: 0.8rem; color: #1e40af; font-weight: 700;">
        <span style="font-size: 0.62rem; color: #64748b; font-weight: 500;">dx, dy</span>(+1, -1)
      </div>
    </div>

    <div style="display: flex; gap: 14px; margin-top: 16px; font-size: 0.76rem; color: #64748b; font-weight: 600;">
      <span><strong style="color: #2563eb;">■</strong> Target (Center)</span>
      <span><strong style="color: #93c5fd;">■</strong> dx, dy 오프셋 탐색 (9개 셀)</span>
    </div>
  </div>
</div>

<details class="pf-details">
<summary>코드 보기: 3×3 국소 탐색 루프 (FindClosestGridInRange)</summary>

<div class="details-desc">
타깃 인덱스(<code>centerX, centerY</code>)를 기준으로 <code>dx</code>, <code>dy</code>를 각각 -1부터 1까지 순회하며 인접 9개 셀에 대해서만 유효성 검사 및 최단 거리 비교를 수행합니다.
</div>

```csharp
// FileGridManager.cs: 3x3 국소 탐색 루프
private FileGrid FindClosestGridInRange(Vector2 worldPos, int centerX, int centerY)
{
    float minDistSqr = float.MaxValue;
    FileGrid closestGrid = null;

    for (int dx = -1; dx <= 1; dx++)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            int x = centerX + dx;
            int y = centerY + dy;

            if (IsValidGridIndex(x, y))
            {
                FileGrid grid = gridArray[x, y];
                SearchClosestBetter(grid, worldPos, ref minDistSqr, ref closestGrid);
            }
        }
    }
    return closestGrid;
}
```

</details>

### 3.4 플래그 기반 확장 가능한 검색 시스템

그리드를 검색할 때 '비어 있는 곳', '장애물이 없는 곳', '이미 아군이 배치된 곳' 등 다양한 복합 조건을 가변 인자(`params`) 형태로 손쉽게 검색할 수 있도록 **SearchGridFlag 가변 필터링 시스템**을 설계했습니다.

<div class="pf-visual-frame" style="padding: 24px 20px; background: #f8fbff; border: 1px solid #dce8f6; border-radius: 16px; margin-bottom: 20px;">

  <!-- 플래그 상태 카드 그리드 -->
  <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 12px;">

    <!-- Card 1: Occupied -->
    <div style="background: #ffffff; border: 1.5px solid #dce5f0; border-radius: 12px; padding: 14px 16px; box-shadow: 0 4px 12px rgba(37,99,235,0.03);">
      <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 6px;">
        <span style="font-family: 'Fira Code', monospace; font-size: 0.82rem; font-weight: 800; color: #2563eb; background: #eff6ff; padding: 2px 7px; border-radius: 5px; border: 1px solid #bfdbfe;">Occupied</span>
        <span style="font-size: 0.72rem; color: #64748b; font-weight: 700;">점유 셀</span>
      </div>
      <p style="font-size: 0.8rem; color: #475569; margin: 0; line-height: 1.45;">파일 유닛이 배치되어 있는 셀(<code>GetFileUnit() != null</code>)만을 탐색 대상으로 한정</p>
    </div>

    <!-- Card 2: NotOccupied -->
    <div style="background: #ffffff; border: 1.5px solid #dce5f0; border-radius: 12px; padding: 14px 16px; box-shadow: 0 4px 12px rgba(37,99,235,0.03);">
      <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 6px;">
        <span style="font-family: 'Fira Code', monospace; font-size: 0.82rem; font-weight: 800; color: #059669; background: #ecfdf5; padding: 2px 7px; border-radius: 5px; border: 1px solid #a7f3d0;">NotOccupied</span>
        <span style="font-size: 0.72rem; color: #64748b; font-weight: 700;">빈 셀</span>
      </div>
      <p style="font-size: 0.8rem; color: #475569; margin: 0; line-height: 1.45;">유닛이 배치되지 않은 빈 셀만을 필터링 (신규 유닛 설치 가능 공간 검증)</p>
    </div>

    <!-- Card 3: Obstacle -->
    <div style="background: #ffffff; border: 1.5px solid #dce5f0; border-radius: 12px; padding: 14px 16px; box-shadow: 0 4px 12px rgba(37,99,235,0.03);">
      <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 6px;">
        <span style="font-family: 'Fira Code', monospace; font-size: 0.82rem; font-weight: 800; color: #dc2626; background: #fef2f2; padding: 2px 7px; border-radius: 5px; border: 1px solid #fecaca;">Obstacle</span>
        <span style="font-size: 0.72rem; color: #64748b; font-weight: 700;">장애물</span>
      </div>
      <p style="font-size: 0.8rem; color: #475569; margin: 0; line-height: 1.45;">시스템 장애물(<code>obstacleObject != null</code>)이 위치한 셀을 식별</p>
    </div>

    <!-- Card 4: NotObstacle -->
    <div style="background: #ffffff; border: 1.5px solid #dce5f0; border-radius: 12px; padding: 14px 16px; box-shadow: 0 4px 12px rgba(37,99,235,0.03);">
      <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 6px;">
        <span style="font-family: 'Fira Code', monospace; font-size: 0.82rem; font-weight: 800; color: #0284c7; background: #f0f9ff; padding: 2px 7px; border-radius: 5px; border: 1px solid #bae6fd;">NotObstacle</span>
        <span style="font-size: 0.72rem; color: #64748b; font-weight: 700;">통과 가능</span>
      </div>
      <p style="font-size: 0.8rem; color: #475569; margin: 0; line-height: 1.45;">장애물이 없어 객체 배치 및 투사체 궤적 형성이 가능한 클린 셀</p>
    </div>

    <!-- Card 5: None -->
    <div style="background: #ffffff; border: 1.5px solid #dce5f0; border-radius: 12px; padding: 14px 16px; box-shadow: 0 4px 12px rgba(37,99,235,0.03);">
      <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 6px;">
        <span style="font-family: 'Fira Code', monospace; font-size: 0.82rem; font-weight: 800; color: #64748b; background: #f1f5f9; padding: 2px 7px; border-radius: 5px; border: 1px solid #cbd5e1;">None</span>
        <span style="font-size: 0.72rem; color: #64748b; font-weight: 700;">전체 필터</span>
      </div>
      <p style="font-size: 0.8rem; color: #475569; margin: 0; line-height: 1.45;">조건 필터링 없이 레이아웃 내 모든 셀을 탐색 범위로 처리</p>
    </div>

  </div>

</div>

```csharp
public enum SearchGridFlag
{
    Occupied,       // 점유
    NotOccupied,    // 비점유
    Obstacle,       // 장애물이 존재하는 그리드만
    NotObstacle,    // 장애물이 존재하지 않는 그리드만
    None,
}
```

<!-- 가변 인자(params) 기반 복합 쿼리 호출 예시 -->
<div style="background: #ffffff; border: 1.5px solid #dce5f0; border-radius: 14px; padding: 18px 22px; box-shadow: 0 6px 18px rgba(37,99,235,0.04); margin-top: 20px; margin-bottom: 20px;">
  <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 12px;">
    <span style="font-size: 1rem;">💡</span>
    <span style="font-family: 'Fira Code', monospace; font-size: 0.9rem; font-weight: 800; color: #1e293b;">가변 인자(params) 기반 복합 쿼리 호출 예시</span>
  </div>
  <div style="background: #0f172a; padding: 14px 18px; border-radius: 10px; font-family: 'Fira Code', monospace; font-size: 0.84rem; color: #f8fafc; overflow-x: auto; margin-bottom: 12px; border: 1px solid #1e293b;">
    <span style="color: #38bdf8;">FileGrid</span> target = FindFlagGridWorld(mousePos, <span style="color: #4ade80;">SearchGridFlag.NotOccupied</span>, <span style="color: #4ade80;">SearchGridFlag.NotObstacle</span>);
  </div>
  <div style="display: flex; flex-wrap: wrap; gap: 8px; align-items: center; font-size: 0.82rem; color: #475569;">
    <span style="background: #eff6ff; color: #2563eb; font-weight: 700; padding: 3px 8px; border-radius: 5px; border: 1px solid #bfdbfe;">params 다중 플래그</span>
    <span>➔</span>
    <span><strong>&quot;유닛이 없고 + 동시에 장애물도 없는&quot;</strong> 유효 그리드만 순회 루프 내에서 인라인 판정하여 최단 거리 그리드를 반환</span>
  </div>
</div>

<details class="pf-details">
<summary>코드 보기: FindFlagGridWorld</summary>

```csharp
public FileGrid FindFlagGridWorld(Vector2 worldPos, params SearchGridFlag[] flags)
{
    if (gridArray == null || flags == null || flags.Length == 0)
        return null;

    float bestDistSqr = float.MaxValue;
    FileGrid bestGrid = null;

    foreach (FileGrid grid in gridArray)
    {
        if (grid == null) continue;

        // 플래그 조건 체크
        bool isOccupied = grid.GetFileUnit() != null;
        bool hasObstacle = grid.obstacleObject != null;
        bool isMatch = true;

        foreach (SearchGridFlag flag in flags)
        {
            switch (flag)
            {
                case SearchGridFlag.Occupied:
                    if (!isOccupied) isMatch = false;
                    break;
                case SearchGridFlag.NotOccupied:
                    if (isOccupied) isMatch = false;
                    break;
                case SearchGridFlag.Obstacle:
                    if (!hasObstacle) isMatch = false;
                    break;
                case SearchGridFlag.NotObstacle:
                    if (hasObstacle) isMatch = false;
                    break;
                case SearchGridFlag.None:
                    break;
            }

            if (!isMatch) break;
        }

        if (!isMatch) continue;

        SearchClosestBetter(grid, worldPos, ref bestDistSqr, ref bestGrid);
    }

    return bestGrid;
}
```

</details>

---

## 4. 다형성 기반 동적 버프 시스템
{: .chapter-title }

### 4.1 Observer 패턴 기반 버프 자동 전파

파일 유닛이 그리드 상에 배치되거나 이동할 때, 버프 영역을 동적으로 계산하고 전파하기 위해 **Observer 패턴** 구조를 활용했습니다.

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- 0. START -->
    <div class="pf-fc-pill-start">
      <span>📦</span>
      <span>유닛 그리드 배치 (Unit Placed on Grid)</span>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 01: 조건 분기 (버프 제공 유닛 vs 일반 유닛) -->
    <div class="pf-fc-decision pf-fc-compact">
      <div class="pf-fc-decision-header">
        <span class="pf-fc-decision-badge">유닛 역할 판정</span>
        <h4 class="pf-fc-decision-title">배치된 유닛이 버프 제공체인가? (Is Buff Source?)</h4>
      </div>
      <div class="pf-fc-branch-grid">
        <!-- YES: 버프 제공 유닛 (오라 전파) -->
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #bfdbfe; background: #f8fbff;">
          <span class="pf-fc-tag-no" style="background: #2563eb; color: #fff;">YES (버프 제공 유닛)</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">Aura Provider (오라 영역 등록)</div>
          <p class="pf-fc-branch-desc">
            • 사거리 내 모든 인접 <code>FileGrid</code> 탐색<br>
            • 그리드에 버프 소스 등록 (<code>Grid.AddBuffSource</code>)<br>
            • 이미 배치된 유닛에 즉각 버프 전파 (<code>ApplyBuff</code>)
          </p>
        </div>
        <!-- NO: 일반 수혜 유닛 (버프 수혜) -->
        <div class="pf-fc-branch-item pf-fc-item-compact" style="border-color: #bbf7d0; background: #fdfffe;">
          <span class="pf-fc-tag-yes" style="background: #16a34a; color: #fff;">NO (일반 수혜 유닛)</span>
          <div class="pf-fc-branch-title" style="margin-top: 5px;">Buff Consumer (그리드 버프 수혜)</div>
          <p class="pf-fc-branch-desc">
            • 진입한 <code>FileGrid</code>의 활성 버프 풀 조회<br>
            • 축적된 <code>_activeBuffSources</code> 목록 확인<br>
            • 진입한 유닛에게 모든 버프 일괄 적용 (<code>ApplyBuff</code>)
          </p>
        </div>
      </div>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 02: 동기화 완료 및 라이프사이클 보장 -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #cbd5e1; background: #ffffff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #0f172a;">✨ Observer 기반 결합도 분리 (Decoupled Sync)</h4>
        <span class="pf-fc-badge" style="background: #e2e8f0; color: #334155;">LIFECYCLE</span>
      </div>
      <p class="pf-fc-desc" style="color: #475569; margin: 0; font-size: 0.82rem; line-height: 1.5;">
        유닛 간 직접 참조 대신 <strong>FileGrid가 중계자(Subject)</strong> 역할을 수행하여, 유닛의 배치·진입·이탈·사망 시 버프 스탯이 누락 없이 자동 갱신 및 원복됩니다.
      </p>
    </div>

  </div>
</div>

*   **버프 전파 및 라이프사이클 핵심:**
    1.  **오라 영역 등록 (Provider):** 버프 특성을 가진 유닛이 배치되면 사거리 내 `FileGrid` 셀들에 자신을 소스로 등록하고, 기배치된 유닛에 즉시 버프를 전파합니다.
    2.  **진입 시 자동 적용 (Consumer):** 일반 유닛이 버프 영역의 그리드로 진입하면, `FileGrid`에 누적되어 있던 버프 데이터(`_activeBuffSources`)를 즉각 전달받아 적용합니다.
    3.  **이탈 및 사망 시 자동 원복 (Cleanup):** 버프 유닛이 이동하거나 사망하면 해당 그리드에서 소스가 제거(`RemoveBuffSource`)되며, 영향받던 유닛의 스탯이 즉시 원복됩니다.

<details class="pf-details">
<summary>코드 보기: FileGrid 옵저버 기반 버프 전파 및 수혜 파이프라인</summary>

```csharp
// FileGrid.cs: Observer 패턴 기반 핵심 버프 전파 및 수혜 로직
public class FileGrid : MonoBehaviour
{
    private File_Base fileUnit;

    // 해당 그리드에 영향을 미치는 버프 제공자 목록 (중복 제거 O(1))
    private readonly HashSet<File_Base> _activeBuffSources = new HashSet<File_Base>();

    #region Core Buff Logic
    // 버프 유닛(오라) 배치 시 그리드에 소스 등록 및 기존 유닛 버프 전파
    public void AddBuffSource(File_Base source)
    {
        // ...
    }

    // 버프 유닛 이탈/사망 시 소스 제거 및 버프 원복
    public void RemoveBuffSource(File_Base source)
    {
        // ...
    }

    // 단일 버프 소스로부터 대상 유닛에 버프 스탯 부여 또는 해제
    private void ApplyBuffFromSource(File_Base target, File_Base source, bool apply)
    {
        // ...
    }

    // 신규 유닛 진입/이탈 시 그리드에 축적된 모든 버프 일괄 적용/원복
    private void ApplyGridBuffs(bool apply)
    {
        // ...
    }
    #endregion
}
```

</details>

### 4.2 컴포넌트 기반 버프 기본 형태 (Buff_Base)

다양한 종류의 버프(스탯 강화, 슬로우, 힐링 도트 등)를 객체지향적으로 확장할 수 있도록, 유니티의 컴포넌트(`MonoBehaviour`) 기반 추상 클래스 **Buff_Base**를 설계했습니다. 유닛에 버프가 부착될 때 필요한 기본 데이터 모델 바인딩과 중복 파티클 방지 메커니즘을 캡슐화했습니다.

<details class="pf-details">
<summary>코드 보기: 버프 기본 형태 및 컴포넌트 초기화 (Buff_Base.cs)</summary>

```csharp
// Buff_Base.cs: 컴포넌트 기반 버프 추상 클래스
public abstract class Buff_Base : MonoBehaviour
{
    // ==========================================
    // [버프 데이터 프로퍼티]
    // ==========================================
    public Define.BuffType BuffType { get; private set; }  // 버프 고유 분류 (공격력 증가, 슬로우, 도트 힐 등)
    public float Amount { get; private set; }             // 버프 효과 수치 (스탯 증감량 또는 틱당 데미지/회복량)
    public float Duration { get; private set; }           // 버프 총 유지 시간 (초 단위, 0 이하면 오라형 영구 지속)
    public float WaitTickTime { get; private set; }       // 틱 발동 주기 간격 (초 단위, 주기적 OnTick 호출 간격)
    public bool IsRemoving { get; private set; } = false; // 버프 삭제/해제 절차 진행 여부 (중복 해제 방지 플래그)

    // ==========================================
    // [내부 타이머 및 인스턴스 필드]
    // ==========================================
    protected float _durationTimer;  // 남은 지속 시간을 측정하는 카운트다운 타이머
    protected float _tickTimer;      // 다음 틱 도달까지 누적되는 델타타임 타이머
    protected File_Base owner;       // 버프가 부착되어 적용되는 대상 파일 유닛
    private GameObject myParticle;   // 유닛 상단에 출력되는 활성화된 VFX 파티클

    // ==========================================
    // [멤버 함수 (간략화)]
    // ==========================================

    // 컴포넌트 부착 시 스탯 바인딩 및 파티클 트리거
    public void Init(BuffStat stat)
    {
        owner = GetComponent<File_Base>();
        BuffType = stat.buffType;
        Amount = stat.amount;
        Duration = _durationTimer = stat.duration;
        WaitTickTime = stat.waitTickTime;
        _tickTimer = 0f;

        OnAdded();
    }

    // 버프 부착 완료 시 호출 (구체 클래스에서 필요 시 오버라이드)
    protected virtual void OnAdded() => ShowParticle();

    // 동일 버프 중복 검사 후 단일 파티클만 오브젝트 풀에서 활성화
    protected void ShowParticle()
    {
        bool isOnlyOne = GetComponents<Buff_Base>().Count(b => b.BuffType == BuffType && !b.IsRemoving) == 1;
        if (isOnlyOne)
        {
            myParticle = Managers.Pool.GetObjParticle(GetParticleType(BuffType));
            if (myParticle != null) myParticle.SetActive(true);
        }
    }

    // 버프 해제 시 구체 클래스별 스탯 원복 및 풀 반환 처리
    public abstract void Remove();
}
```

</details>

### 4.3 다형성을 활용한 틱(Tick) 기반 업데이트 엔진

단순 스탯 증가 버프 외에도 주기적인 리젠(HP 회복), 지속 피해(도트 데미지) 등을 개별 코루틴 남발 없이 고성능으로 처리하기 위해, 중앙 `Update()` 루프에서 델타타임을 누적하고 다형적 가상 함수 `OnTick()`을 호출하는 방식을 채택했습니다.

<details class="pf-details">
<summary>코드 보기: 다형적 틱(Tick) 주기 연산 및 Duration 타이머 루프</summary>

```csharp
// Buff_Base.cs: 중앙 집중식 틱(Tick) 연산 및 지속시간 관리 루프
protected virtual void Update()
{
    if (IsRemoving) return;

    float deltaTime = Time.deltaTime;

    // 1. 틱(Tick) 주기 연산 (단일 코루틴 남발 방지)
    if (WaitTickTime > 0)
    {
        _tickTimer += deltaTime;
        if (_tickTimer >= WaitTickTime)
        {
            _tickTimer -= WaitTickTime;
            OnTick(); // 상속받은 구체 클래스의 고유 로직(힐링/도트 피해 등) 다형적 실행
        }
    }

    // 2. 버프 지속 시간(Duration) 관리
    if (Duration > 0)
    {
        _durationTimer -= deltaTime;
        if (_durationTimer <= 0)
        {
            Remove();
        }
    }
}

protected virtual void OnTick() { }
```

</details>
