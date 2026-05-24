---
layout: single
title: "PROJECT REPORT // FILE_TOWER_DEFENSE"
excerpt: "Systems Engineering: UI-to-GameObject 리팩토링, 통합 입력 시스템 및 그리드 아키텍처"
categories: [project]
permalink: /project/file-tower-defense/
tags: [Unity, Optimization, Architecture, Algorithms]
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

File Tower Defense 프로젝트의 코어 시스템 설계를 담당하며, 유니티의 좌표계 특성을 분석한 아키텍처 재설계부터 대규모 객체의 입력 처리, 동적 버프 시스템, 그리고 그리드 기반 배치 로직까지 전반적인 시스템 개선 과정을 기록한 리포트입니다.

---

## 1. 시스템 리팩토링 및 개선
{: .chapter-title }

### 1.1 UI(RectTransform)에서 GameObject(Transform) 기반 전환

초기 설계에서는 윈도우 바탕화면의 아이콘 느낌을 살리기 위해 모든 파일 유닛을 UI 시스템(`RectTransform`)으로 구축했습니다. 그러나 프로젝트가 고도화됨에 따라 다음과 같은 한계에 직면했습니다.

*   **좌표계 종속성:** 캔버스의 앵커/피벗 설정에 따라 월드 좌표가 상대적으로 변하여 정밀한 위치 계산이 어려움.
*   **연산 오버헤드:** 월드 좌표 기반의 바이러스(적)와 UI 유닛 간 물리 충돌 판정을 위해 매 프레임 `Camera.WorldToScreenPoint` 등의 변환 함수 호출 비용 발생.
*   **계층 구조 복잡성:** 서로 다른 UI 부모 간의 위치 변환 로직이 가독성과 유지보수성을 저해함.

<div class="pf-visual-frame">
  <div class="pf-comp-container">
    <div class="pf-comp-box old">
      <strong>RectTransform (UI)</strong><br>
      - Anchor Dependent<br>
      - Pivot Relative<br>
      - WorldToScreen Required
    </div>
    <div class="pf-flow-arrow">>>></div>
    <div class="pf-comp-box new">
      <strong>Transform (World)</strong><br>
      - Absolute Position<br>
      - Physics 2D Optimized<br>
      - Unified Coordinate Space
    </div>
  </div>
</div>

이를 해결하기 위해 모든 유닛을 **GameObject(Transform) 기반**으로 전면 교체하여 월드 좌표계로 통일하였으며, Physics2D와의 직접적인 호환성을 확보하여 성능을 최적화했습니다.

### 1.2 GameObjectGridLayout: 커스텀 레이아웃 엔진

화면 해상도에 맞춰 그리드의 간격과 셀 크기를 동적으로 계산하는 레이아웃 엔진을 구축했습니다.

<div class="pf-visual-frame">
  <strong>계산 공식:</strong><br>
  <code>cellSize = (screenSize - padding*2 - spacing*(n-1)) / n</code>
</div>

<details class="pf-details" markdown="1">
<summary>코드 보기: FitToScreen 로직</summary>

```csharp
// GameObjectGridLayout.cs: 해상도 대응 셀 크기 계산
public void FitToScreen() {
    Camera cam = Camera.main;
    float height = cam.orthographicSize * 2f;
    float width = height * cam.aspect;

    cellSize.x = (width - (padding.x * 2) - (spacing.x * (columns - 1))) / columns;
    cellSize.y = (height - (padding.y * 2) - (spacing.y * (rows - 1))) / rows;

    StartPos = new Vector2(-(width / 2) + padding.x + (cellSize.x / 2), 
                            (height / 2) - padding.y - (cellSize.y / 2));
}
```
</details>

---

## 2. 파일/바이러스/UI 입력 및 상호작용 시스템
{: .chapter-title }

수많은 유닛이 개별 이벤트를 수신하지 않고, **InputManager**가 모든 마우스 입력을 통합 관리하여 **IInteractable** 인터페이스를 구현한 객체에 전달하는 **Mediator 패턴**을 적용했습니다.

### 2.1 중앙 집중식 입력 관리 (InputManager)

매 프레임 사용자 입력을 감지하고 `Physics2D.Raycast`를 통해 객체를 탐지합니다. 특히 UI와 게임 오브젝트가 겹쳤을 때 **SortingLayer와 SortingOrder**를 비교하여 정확한 우선순위를 판정합니다.

<div class="pf-visual-frame">
  <div class="pf-transaction-flow">
    <div class="pf-flow-step">HandleHover()<br>객체 탐지</div>
    <div class="pf-flow-arrow">→</div>
    <div class="pf-flow-step">HandleMouseInput()<br>클릭/드래그</div>
    <div class="pf-flow-arrow">→</div>
    <div class="pf-flow-step">InteractionHandler<br>이벤트 전달</div>
    <div class="pf-flow-arrow">→</div>
    <div class="pf-flow-step">IInteractable<br>객체 처리</div>
  </div>

  <div class="pf-arch-diagram">
    <div class="pf-arch-layer">
      <div class="pf-arch-layer-title">Input Layer (Mouse/Key)</div>
    </div>
    <div style="text-align: center; color: #007bff; font-size: 1.2rem; margin: 5px 0;">↓</div>
    <div class="pf-arch-layer">
      <div class="pf-arch-layer-title">InputManager (Central Hub)</div>
      <div class="pf-arch-layer-items">
        <span class="pf-arch-item">Sorting Priority Check</span>
        <span class="pf-arch-item">InputState Machine</span>
      </div>
    </div>
    <div style="text-align: center; color: #007bff; font-size: 1.2rem; margin: 5px 0;">↓ Dispatch</div>
    <div class="pf-arch-layer">
      <div class="pf-arch-layer-title">InteractionHandler (Mediator)</div>
      <div class="pf-arch-layer-items">
        <span class="pf-arch-item">Multi-Selection</span>
        <span class="pf-arch-item">Drag Grouping</span>
      </div>
    </div>
  </div>
</div>

### 2.2 IInteractable 인터페이스 기반 확장성

모든 상호작용 가능한 객체는 동일한 계약을 따르며, 자신의 고유 로직(클릭, 드래그, 호버)만을 구현합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: IInteractable 인터페이스</summary>

```csharp
public interface IInteractable {
    GameObject targetObj { get; }
    bool IsSelectable { get; }
    bool IsDraggable { get; }
    
    void OnHoverEnter();
    void OnHoverExit();
    void OnClick();
    void OnBeginDrag();
    void OnDrag(Vector2 mouseDelta);
    void OnEndDrag();
    void OnSelected(bool isSelected);
}
```
</details>

---

## 3. 그리드 기반 오브젝트 배치 시스템
{: .chapter-title }

### 3.1 FileGrid: 데이터 중심의 지능형 셀 매니저

단순한 위치 정보가 아닌, 점유 상태와 자신에게 적용된 버프 목록(`HashSet`)을 관리하는 지능형 컨테이너입니다. `HashSet`을 사용하여 버프 소스의 중복을 원천 차단하고 O(1)의 빠른 조회 성능을 확보했습니다.

<div class="pf-visual-frame">
  <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px; margin: 10px 0;">
    <div style="padding: 15px; border: 1px solid #007bff; border-radius: 8px; background: rgba(0, 123, 255, 0.02);">
      <div style="color: #007bff; font-weight: bold; margin-bottom: 8px; font-size: 0.85rem;">State</div>
      <div style="font-size: 0.75rem; color: #666; text-align: left;">
        • GridX, GridY<br>
        • fileUnit<br>
        • obstacleObject
      </div>
    </div>
    <div style="padding: 15px; border: 1px solid #28a745; border-radius: 8px; background: rgba(40, 167, 69, 0.02);">
      <div style="color: #28a745; font-weight: bold; margin-bottom: 8px; font-size: 0.85rem;">Buff System</div>
      <div style="font-size: 0.75rem; color: #666; text-align: left;">
        • HashSet&lt;File_Base&gt;<br>
        • O(1) Lookup<br>
        • Auto Sync
      </div>
    </div>
    <div style="padding: 15px; border: 1px solid #333; border-radius: 8px; background: rgba(0, 0, 0, 0.02);">
      <div style="color: #333; font-weight: bold; margin-bottom: 8px; font-size: 0.85rem;">Visuals</div>
      <div style="font-size: 0.75rem; color: #666; text-align: left;">
        • Color State<br>
        • Hover Effect<br>
        • Selection
      </div>
    </div>
  </div>
</div>

### 3.2 원자적 배치 트랜잭션 (Transactional Pattern)

데이터 불일치를 방지하고 실패 시 안전한 롤백을 위해 3단계 배치 로직을 설계했습니다.

1.  **Pre-validation:** 대상 그리드의 점유 여부 및 장애물 존재 여부 체크.
2.  **Rollback Prep:** 기존 그리드에서 유닛 정보 및 버프 소스 안전하게 제거.
3.  **Commit:** 새 그리드에 부모-자식 관계 설정 및 위치 동기화, 인접 버프 자동 적용.

### 3.3 동적 양방향 좌표 변환 시스템

월드 좌표와 그리드 인덱스 간의 완전한 호환성을 제공합니다.

<div class="pf-visual-frame">
  <div class="pf-coord-flow">
    <div class="pf-coord-box">
      <div class="pf-coord-box-title">World Position</div>
      <div class="pf-coord-box-formula">InverseTransformPoint(worldPos)</div>
    </div>
    <div style="display: flex; align-items: center; font-size: 1.5rem; color: #007bff;">⇄</div>
    <div class="pf-coord-box">
      <div class="pf-coord-box-title">Grid Index (X, Y)</div>
      <div class="pf-coord-box-formula">RoundToInt((local - start) / cellSize)</div>
    </div>
  </div>
</div>

### 3.4 공간 분할 기반 탐색 최적화 (O(1))

전체 그리드를 순회하는 대신, 마우스 위치의 인덱스를 기준으로 **인접 3x3 영역**만 검사하도록 설계하여 탐색 비용을 획기적으로 단축했습니다.

<div class="pf-visual-frame">
  <div class="pf-diagram-grid">
    <div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div>
    <div class="pf-grid-cell near"></div><div class="pf-grid-cell active"></div><div class="pf-grid-cell near"></div>
    <div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div>
  </div>
  <p style="font-size: 0.85rem; color: #666; margin-top: 10px;">Spatial Partitioning: Searching 9 cells instead of 91</p>
</div>

<details class="pf-details" markdown="1">
<summary>코드 보기: 공간 분할 탐색 알고리즘</summary>

<div class="details-desc">
월드 좌표를 인덱스로 즉시 변환한 뒤, 해당 인덱스를 중심으로 3x3 영역 내의 그리드만 제곱 거리(`sqrMagnitude`)로 비교하여 최적의 그리드를 탐색합니다.
</div>

```csharp
// FileGridManager.cs: 공간 분할 기반 탐색 최적화 로직
public FileGrid GetGrid(Vector2 worldPos) {
    // 1. 월드 좌표를 그리드 인덱스로 변환 (O(1))
    if (!WorldToGridIndex(worldPos, out int xCenter, out int yCenter)) return null;

    // 2. 인덱스가 유효하면 주변 3x3 영역만 탐색 (O(1))
    if (IsValidGridIndex(xCenter, yCenter)) {
        return FindClosestGridInRange(worldPos, xCenter, yCenter);
    }

    // 범위 밖일 경우에만 전체 탐색 (예외 처리)
    return FindGlobalClosestGrid(worldPos);
}

private FileGrid FindClosestGridInRange(Vector2 worldPos, int centerX, int centerY) {
    float minDistSqr = float.MaxValue;
    FileGrid closestGrid = null;

    // 중심 기준 3x3 루프
    for (int dx = -1; dx <= 1; dx++) {
        for (int dy = -1; dy <= 1; dy++) {
            int x = centerX + dx;
            int y = centerY + dy;

            if (IsValidGridIndex(x, y)) {
                FileGrid candidate = gridArray[x, y];
                // 제곱 거리 사용으로 sqrt 연산 제거 최적화
                float distSqr = ((Vector2)candidate.transform.position - worldPos).sqrMagnitude;
                if (distSqr < minDistSqr) {
                    minDistSqr = distSqr;
                    closestGrid = candidate;
                }
            }
        }
    }
    return closestGrid;
}
```

</details>

### 3.5 플래그 기반 확장 가능한 검색 시스템

그리드 검색 시 점유 상태, 장애물 존재 여부 등 다양한 조건을 플래그 조합으로 필터링할 수 있는 시스템입니다.

<div class="pf-visual-frame" markdown="1">
<div class="pf-table-wrapper" markdown="1">
<table class="pf-data-table" style="width: 100% !important; table-layout: fixed !important;">
    <thead>
        <tr>
            <th style="width: 20%; text-align: center;">플래그</th>
            <th style="width: 35%; text-align: center;">설명</th>
            <th style="width: 45%; text-align: center;">사용 예시 및 기대 결과</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="text-align: center;"><code>Occupied</code></td>
            <td style="text-align: left;">현재 파일 유닛이 배치되어 점유 중인 그리드 셀만을 탐색 대상으로 한정합니다.</td>
            <td style="text-align: left;">이미 설치된 특정 타워의 위치를 추적하거나, 인접한 유닛의 시너지를 계산할 때 활용됩니다.</td>
        </tr>
        <tr>
            <td style="text-align: center;"><code>NotOccupied</code></td>
            <td style="text-align: left;">파일 유닛이 배치되지 않은 비어있는 상태의 그리드 셀만을 필터링하여 검색합니다.</td>
            <td style="text-align: left;">플레이어가 새로운 파일 유닛을 드래그하여 설치 가능한 빈 공간을 유효성 검사할 때 필수적으로 사용됩니다.</td>
        </tr>
        <tr>
            <td style="text-align: center;"><code>Obstacle</code></td>
            <td style="text-align: left;">시스템 장애물(땅굴 등)이 생성되어 일반적인 유닛 배치가 불가능한 그리드만을 검색합니다.</td>
            <td style="text-align: left;">맵 파괴 이벤트나 바이러스의 특수 공격으로 인해 생성된 장애물 객체의 위치를 파악할 때 사용됩니다.</td>
        </tr>
        <tr>
            <td style="text-align: center;"><code>NotObstacle</code></td>
            <td style="text-align: left;">장애물이 존재하지 않아 물리적으로 객체 배치가 가능한 클린한 상태의 그리드만을 검색합니다.</td>
            <td style="text-align: left;">장애물을 피해 안전하게 유닛을 배치하거나, 투사체가 지나갈 수 있는 경로를 계산할 때 필터로 활용됩니다.</td>
        </tr>
        <tr>
            <td style="text-align: center;"><code>None</code></td>
            <td style="text-align: left;">별도의 필터 조건을 적용하지 않고 그리드 레이아웃 내의 모든 셀을 탐색 범위에 포함합니다.</td>
            <td style="text-align: left;">전체 그리드의 초기화, 일괄 색상 변경, 또는 모든 셀에 대한 거리 기반 전수 조사가 필요할 때 사용됩니다.</td>
        </tr>
    </tbody>
</table>
</div>

<div style="margin-top: 20px; padding: 20px; background: rgba(88, 166, 255, 0.05); border-radius: 8px; border: 1px solid #e1e4e8; text-align: left;">
    <div style="color: #007bff; font-weight: 700; margin-bottom: 12px; font-family: 'Fira Code', monospace; font-size: 1rem; border-bottom: 1px solid #eee; padding-bottom: 8px;">💡 복합 쿼리 조합 예시 (High-Density Logic)</div>
    <div style="color: #333; font-size: 0.95rem; font-family: 'Fira Code', monospace; line-height: 1.8;">
        <code style="background: #eef5ff; padding: 4px 8px; border-radius: 4px; color: #0056b3; font-weight: bold;">FindFlagGridWorld(pos, NotOccupied, NotObstacle)</code><br/>
        <span style="color: #555; display: inline-block; margin-top: 10px;">➔ <strong>"유닛이 없고 + 장애물도 없는"</strong> 가장 가까운 그리드를 반환하여 즉시 설치 가능한 환경을 보장합니다.</span>
    </div>
</div>
</div>

<details class="pf-details" markdown="1">
<summary>코드 보기: 플래그 기반 검색 로직</summary>

<div class="details-desc">
가변 인자(`params`)와 enum 플래그를 결합하여 복합적인 설치 조건을 단일 메서드로 필터링하는 확장 가능한 검색 아키텍처입니다.
</div>

```csharp
// FileGridManager.cs: 플래그 기반 확장 검색 시스템
public enum SearchGridFlag {
    Occupied,       // 점유됨
    NotOccupied,    // 비어있음
    Obstacle,       // 장애물 있음
    NotObstacle,    // 장애물 없음
    None,
}

public FileGrid FindFlagGridWorld(Vector2 worldPos, params SearchGridFlag[] flags) {
    float bestDistSqr = float.MaxValue;
    FileGrid bestGrid = null;

    foreach (FileGrid grid in gridArray) {
        if (grid == null) continue;

        // 플래그 조건 매칭 검사
        if (!CheckGridMatchesFlags(grid, flags)) continue;

        // 가장 가까운 거리 탐색 최적화
        SearchClosestBetter(grid, worldPos, ref bestDistSqr, ref bestGrid);
    }
    return bestGrid;
}

private bool CheckGridMatchesFlags(FileGrid grid, SearchGridFlag[] flags) {
    bool isOccupied = grid.GetFileUnit() != null;
    bool hasObstacle = grid.obstacleObject != null;

    foreach (SearchGridFlag flag in flags) {
        switch (flag) {
            case SearchGridFlag.Occupied: if (!isOccupied) return false; break;
            case SearchGridFlag.NotOccupied: if (isOccupied) return false; break;
            case SearchGridFlag.Obstacle: if (!hasObstacle) return false; break;
            case SearchGridFlag.NotObstacle: if (hasObstacle) return false; break;
        }
    }
    return true;
}
```

</details>

---

## 4. 다형성 기반 동적 버프 시스템
{: .chapter-title }

### 4.1 Observer 패턴 기반 자동 동기화

파일이 그리드에 배치되는 순간, 그리드가 이미 보유한 버프 소스(Aura)들을 유닛에게 즉각 적용합니다. 버프 소스와 그리드 간의 느슨한 결합으로 시스템 확장성을 확보했습니다.

<div class="pf-visual-frame">
  <div class="pf-arch-diagram">
    <div class="pf-arch-layer">
      <div class="pf-arch-layer-title">Buff Source (File_Base)</div>
      <div class="pf-arch-layer-items">
        <span class="pf-arch-item">AddBuffSource()</span>
        <span class="pf-arch-item">RemoveBuffSource()</span>
      </div>
    </div>
    <div style="text-align: center; color: #007bff; font-size: 1.2rem; margin: 5px 0;">↓ Notify</div>
    <div class="pf-arch-layer">
      <div class="pf-arch-layer-title">FileGrid (Observer)</div>
      <div class="pf-arch-layer-items">
        <span class="pf-arch-item">HashSet&lt;File_Base&gt;</span>
        <span class="pf-arch-item">Auto Apply Buff</span>
      </div>
    </div>
    <div style="text-align: center; color: #007bff; font-size: 1.2rem; margin: 5px 0;">↓ Apply</div>
    <div class="pf-arch-layer">
      <div class="pf-arch-layer-title">FileUnit (Target)</div>
      <div class="pf-arch-layer-items">
        <span class="pf-arch-item">ApplyBuffFromSource()</span>
        <span class="pf-arch-item">Real-time Sync</span>
      </div>
    </div>
  </div>
</div>

### 4.2 틱(Tick) 기반 버프 처리 설계

단순 능력치 증폭 외에 초당 체력 회복 등 주기적인 로직 처리를 위해 `MyBuff` 추상 클래스와 `OnTick` 메서드를 설계했습니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: 다형적 버프 실행 루프</summary>

```csharp
// BuffData_Base.cs: 다형적 버프 루프
public virtual void Tick(float deltaTime) {
    foreach (int key in new List<int>(activeBuffs.Keys)) {
        if (activeBuffs[key].Tick(deltaTime)) {
            OnTick(activeBuffs[key]); // 자식 클래스에서 재정의된 로직 호출
        }
    }
}

// Buff_MP3.cs: 구체적인 힐링 로직 구현
public class Buff_HP : MyBuff {
    protected override void OnTick(BuffData buffData) {
        buffTarget.GetComponent<IHealth>().Heal(buffData.Amount);
    }
}
```
</details>

---

## 5. 전체 시스템 기술적 특징 요약
{: .chapter-title }

*   **성능 최적화:** UI에서 GameObject로 전환하여 **Canvas Rebuilding 부하를 0으로** 제거하고 공간 분할 탐색 도입.
*   **지능형 상호작용:** SortingLayer 비교를 통한 정교한 우선순위 판정 및 Mediator 패턴 기반의 이벤트 통합.
*   **데이터 무결성:** 원자적 트랜잭션 배치를 통해 유닛 이동 시 데이터 불일치 가능성 차단.
*   **유연한 확장성:** 플래그 기반 검색 시스템과 다형성 버프 시스템으로 새로운 유닛/바이러스 추가에 용이한 구조.

{: .notice--success}
**기술적 성과:** 윈도우 바탕화면의 정렬 감성을 안정적인 게임 시스템으로 구현하였으며, 대규모 객체 환경에서도 60fps 이상의 안정적인 성능과 견고한 코어 아키텍처를 완성했습니다.
