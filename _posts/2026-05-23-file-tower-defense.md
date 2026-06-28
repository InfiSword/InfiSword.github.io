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

**File Tower Defense** 프로젝트의 코어 시스템 설계를 담당하며, 유니티의 좌표계 특성을 분석한 아키텍처 재설계부터 대규모 객체의 입력 처리, 동적 버프 시스템, 그리고 그리드 기반 배치 로직까지 전반적인 시스템 개선 과정을 기록한 리포트입니다.

---

## 1. 시스템 리팩토링 및 개선
{: .chapter-title }

### 1.1 UI(RectTransform)에서 GameObject(Transform) 기반 전환

초기 설계에서는 윈도우 바탕화면의 아이콘 느낌을 살리기 위해 모든 파일 유닛을 UI 시스템(`RectTransform`)으로 구축했습니다. 그러나 프로젝트가 고도화됨에 따라 다음과 같은 한계에 직면했습니다.

*   **좌표계 종속성:** 캔버스의 앵커/피벗 설정 및 해상도에 따라 월드 좌표가 상대적으로 변하여 게임 월드 내 정밀한 위치 계산과 사거리 판정이 매우 까다로움.
*   **연산 오버헤드 (Canvas Rebuilding):** 유닛의 움직임, 호버 효과, 드래그 등의 UI 요소가 갱신될 때마다 Unity 내부에서 Canvas Rebuild가 강제로 유발되어 CPU 프레임 드랍 발생.
*   **물리 엔진의 부재:** 월드 좌표 기반의 바이러스(적)와 UI 유닛 간 물리 충돌 판정을 위해 매 프레임 `Camera.WorldToScreenPoint` 등의 물리-화면 좌표 변환 함수 호출 비용 발생.

<div class="pf-visual-frame">
  <div class="pf-comp-container">
    <div class="pf-comp-box old">
      <strong>RectTransform (UI)</strong><br>
      - Anchor/Pivot Dependent<br>
      - Frequent Canvas Rebuilds<br>
      - Heavy Screen-to-World conversions
    </div>
    <div class="pf-flow-arrow">>>></div>
    <div class="pf-comp-box new">
      <strong>Transform (World)</strong><br>
      - Absolute 2D Coordinates<br>
      - Zero Canvas Overhead<br>
      - Native Physics 2D & Colliders
    </div>
  </div>
</div>

이를 해결하기 위해 모든 유닛을 **GameObject(Transform) 기반**으로 전면 교체하여 월드 좌표계로 통일하였으며, Physics2D와의 직접적인 호환성을 확보하여 불필요한 연산을 제거하고 성능을 극적으로 최적화했습니다.

### 1.2 GameObjectGridLayout: 커스텀 레이아웃 엔진

화면 해상도에 맞춰 그리드의 간격과 셀 크기를 동적으로 계산하는 레이아웃 엔진을 구축했습니다. 유니티의 `GridLayoutGroup`은 UI 전용이므로, 일반 월드 공간 오브젝트를 위해 화면 크기에 맞게 자동으로 셀 크기를 스케일링하는 컴포넌트를 직접 개발했습니다.

<div class="pf-visual-frame">
  <strong>해상도 기반 자동 조절 공식:</strong><br>
  <code>cellSize = (screenSize - padding*2 - spacing*(n-1)) / n</code>
</div>

<details class="pf-details" markdown="1">
<summary>코드 보기: FitToScreen 로직</summary>

```csharp
// GameObjectGridLayout.cs: 해상도 대응 셀 크기 계산
public void FitToScreen() {
    Camera cam = Camera.main;
    float height = cam.orthographicSize * 2f; // 오르토그래픽 카메라 높이
    float width = height * cam.aspect;         // 카메라 종횡비 기반 너비

    // 가용 영역에서 패딩과 스페이싱을 제외한 실제 셀 크기 도출
    cellSize.x = (width - (padding.x * 2) - (spacing.x * (columns - 1))) / columns;
    cellSize.y = (height - (padding.y * 2) - (spacing.y * (rows - 1))) / rows;

    // 그리드의 좌상단 시작 월드 좌표 계산
    StartPos = new Vector2(-(width / 2) + padding.x + (cellSize.x / 2), 
                            (height / 2) - padding.y - (cellSize.y / 2));
}
```
</details>

---

## 2. 파일/바이러스/UI 입력 및 상호작용 시스템
{: .chapter-title }

수많은 유닛이 각자 `Update()`에서 마우스 충돌을 검사하면 연산량이 기하급수적으로 늘어납니다. 이를 방지하기 위해 **Mediator 패턴**을 활용하여 **InputManager**가 모든 마우스 입력을 통합 관리하고, **IInteractable** 인터페이스를 구현한 대상 객체에게만 이벤트를 전송하도록 설계했습니다.

### 2.1 중앙 집중식 입력 아키텍처

`InputManager`는 마우스의 상태 머신(`InputState`)을 기반으로 클릭, 드래그, 다중 선택(Ctrl+드래그)을 판단하여 `InteractionHandler`를 통해 이벤트를 전파합니다.

```mermaid
graph TD
    A[Mouse / Touch Input] --> B[InputManager]
    B -->|1. Raycast & UI Check| C{Is Pointer Over UI?}
    C -->|Yes| D[Ignore World Interaction / Handle UI]
    C -->|No| E[GetInteractableUnderMouse]
    E -->|2. Sort by Layer/Order| F[Identify Target IInteractable]
    B -->|3. Update InputState| G{InputState}
    G -->|Pressing| H[Check Drag Threshold]
    G -->|DraggingObject| I[OnDragHandler]
    G -->|DraggingBox| J[Multi-Select Drag Box]
    B -->|4. Dispatch Events| K[InteractionHandler]
    K -->|Trigger Interface Methods| L[IInteractable Objects]
    
    style B fill:#007bff,stroke:#0056b3,color:#fff
    style K fill:#28a745,stroke:#1e7e34,color:#fff
    style L fill:#17a2b8,stroke:#117a8b,color:#fff
```

#### 객체 중첩 판정 알고리즘
마우스 아래에 여러 유닛이나 바이러스, 혹은 UI 판정이 겹쳐있을 때, 다음과 같은 우선순위 기준을 사용하여 모호함을 해결합니다.
1.  **UI 레이어 우선:** `EventSystem.current.IsPointerOverGameObject()`를 통해 UI 상호작용 우선 처리.
2.  **물리 레이어 필터링:** `Physics2D.RaycastNonAlloc`을 활용하여 쓰레기 메모리(GC Alloc) 없이 감지된 콜라이더 배열을 가져옴.
3.  **정렬 레이어(Sorting Order) 정렬:** 감지된 객체들의 `SpriteRenderer` 내 `Sorting Layer ID` 및 `Order in Layer` 값을 비교하여 카메라와 가장 가까운(최상단에 렌더링된) 오브젝트를 타깃으로 최종 선택.

### 2.2 IInteractable 인터페이스 기반 확장성

클릭 및 드래그 상호작용이 필요한 모든 인게임 오브젝트(유닛 파일, 바이러스 등)는 [IInteractable](file:///D:/Workspace/codeReference/file_tower_defence/IInteractable.cs) 인터페이스를 상속받아 유연하게 확장할 수 있습니다.

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

[FileGrid](file:///D:/Workspace/codeReference/file_tower_defence/FileGrid.cs)는 단순한 위치 정보 홀더가 아니라, 유닛 배치 상태와 해당 셀에 작용 중인 오라(버프) 목록을 독자적으로 관리하는 지능형 컨테이너입니다.
*   **HashSet 기반 버프 소스 관리:** 현재 그리드 공간에 영향을 주는 버프 제공 유닛의 목록을 `HashSet<File_Base>`로 관리하여 버프의 중복 적용을 제거하고 $O(1)$의 빠른 조회 속도를 유지합니다.
*   **유닛 탈부착 시 자동 스탯 갱신:** 유닛이 배치되거나 이탈할 때 그리드에 축적된 버프 목록을 분석하여 대상 유닛의 스탯을 실시간으로 갱신해 줍니다.

### 3.2 원자적 배치 트랜잭션 (Transactional Pattern)

드래그 앤 드롭으로 유닛의 그리드 위치를 옮길 때 발생할 수 있는 데이터 불일치 및 예외 상황을 원자적으로 보장하기 위해 3단계 배치 트랜잭션 흐름을 설계했습니다.

```mermaid
sequenceDiagram
    autonumber
    actor Player
    participant IM as InputManager
    participant IH as InteractionHandler
    participant FGM as FileGridManager
    participant FG_Old as Old FileGrid
    participant FG_New as New FileGrid
    participant Unit as File Unit (File_Base)

    Player->>IM: Drag & Release Unit
    IM->>IH: End Drag Event
    IH->>FGM: GetGrid(Release Position)
    FGM-->>IH: Return Target Grid (New Grid)
    
    rect rgb(240, 248, 255)
        Note over IH, Unit: Transaction Phase
        IH->>FGM: Validate Placement (New Grid, No Obstacles)
        alt Validation Fails (Rollback)
            IH->>Unit: Rollback (Return to Old Grid)
        else Validation Success (Commit)
            IH->>FG_Old: RemoveFileUnit() (Clear occupant & remove old buffs)
            IH->>FG_New: SetFileUnit(Unit) (Assign occupant & apply new buffs)
            IH->>Unit: Commit Position (Smooth Lerp / Snap)
        end
    end
```

### 3.3 동적 양방향 좌표 변환 시스템

마우스의 월드 스페이스 좌표와 그리드의 인덱스(Row, Column) 좌표계를 상호 변환하기 위해 오프셋 기반의 연산 로직을 정밀하게 구현했습니다.

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

마우스를 드래그할 때 가장 가까운 그리드를 찾기 위해 전체 그리드($N \times M$개)를 전수 검사하는 것은 낭비입니다. 이를 극복하기 위해 **공간 분할(Spatial Partitioning)** 개념을 도입했습니다. 
마우스 월드 좌표를 기반으로 연산 $O(1)$ 만에 예상되는 타깃 그리드 인덱스를 수학적으로 산출하고, 해당 인덱스를 중심으로 **인접한 3x3 그리드 셀(총 9개)**만 가중치(거리 제곱) 계산을 수행하여 탐색 성능을 획기적으로 개선했습니다.

<div class="pf-visual-frame">
  <div class="pf-diagram-grid">
    <div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div>
    <div class="pf-grid-cell near"></div><div class="pf-grid-cell active"></div><div class="pf-grid-cell near"></div>
    <div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div>
  </div>
  <p style="font-size: 0.85rem; color: #666; margin-top: 10px;">Spatial Partitioning: Searching 9 cells instead of Entire Grids</p>
</div>

<details class="pf-details" markdown="1">
<summary>코드 보기: 공간 분할 탐색 알고리즘</summary>

<div class="details-desc">
월드 좌표를 인덱스로 즉시 변환한 뒤, 해당 인덱스를 중심으로 3x3 영역 내의 그리드만 제곱 거리(`sqrMagnitude`)로 비교하여 최적의 그리드를 탐색합니다.
</div>

```csharp
// FileGridManager.cs: 공간 분할 기반 탐색 최적화 로직
public FileGrid GetGrid(Vector2 worldPos) {
    // 1. 월드 좌표를 그리드 인덱스로 수학적 변환 (O(1))
    if (!WorldToGridIndex(worldPos, out int xCenter, out int yCenter)) return null;

    // 2. 인덱스가 유효하면 주변 3x3 영역만 빠르게 검사 (O(1))
    if (IsValidGridIndex(xCenter, yCenter)) {
        return FindClosestGridInRange(worldPos, xCenter, yCenter);
    }

    // 화면 영역 밖의 비정상 예외 상황에서만 전체 탐색 수행 (Safety Net)
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
                // 제곱 거리(sqrMagnitude) 사용으로 비용이 큰 제곱근(Sqrt) 연산 생략
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

그리드를 검색할 때 '비어 있는 곳', '장애물이 없는 곳', '이미 아군이 배치된 곳' 등 다양한 복합 조건을 비트 플래그 형태로 손쉽게 검색할 수 있도록 가변 플래그 검색 시스템을 설계했습니다.

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
        <span style="color: #555; display: inline-block; margin-top: 10px;">➔ <strong>"유닛이 없고 + 동시에 장애물도 없는"</strong> 가장 인접한 유효 그리드를 즉시 검색해 줍니다.</span>
    </div>
</div>
</div>

---

## 4. 다형성 기반 동적 버프 시스템
{: .chapter-title }

### 4.1 Observer 패턴 기반 버프 자동 전파

파일 유닛이 그리드 상에 배치되거나 이동할 때, 버프 영역을 동적으로 계산하고 전파하기 위해 **Observer 패턴** 구조를 활용했습니다.

```mermaid
graph TD
    A[Place Unit on Grid] --> B{Is Unit a Buff Source?}
    
    B -->|Yes| C[ApplyBuffActive]
    C -->|Register Aura Area| D[Skill_BuffMain.ApplyBuffArea]
    D -->|For each grid in range| E[Grid.AddBuffSource]
    E -->|If grid has occupant| F[ApplyBuffFromSource]
    F -->|Apply BuffStat| G[Target Unit.ApplyBuff]
    
    B -->|No| H[ApplyGridBuffs]
    H -->|For each source in Grid._activeBuffSources| I[ApplyBuffFromSource]
    I -->|Apply BuffStat| J[Placed Unit.ApplyBuff]

    style C fill:#007bff,stroke:#0056b3,color:#fff
    style H fill:#28a745,stroke:#1e7e34,color:#fff
    style G fill:#17a2b8,stroke:#117a8b,color:#fff
    style J fill:#17a2b8,stroke:#117a8b,color:#fff
```

*   **버프 전파 흐름:**
    1.  버프 특성을 가진 파일 유닛(예: `.mp3` 힐링 버프)이 그리드에 배치되면, 자신의 사거리(Aura Area) 내에 존재하는 모든 주변 `FileGrid` 셀들을 찾아 자신을 버프 소스로 등록(`AddBuffSource(this)`)합니다.
    2.  이후 새로운 유닛이 해당 버프 영향권 내부의 빈 그리드로 들어올 경우, [FileGrid](file:///D:/Workspace/codeReference/file_tower_defence/FileGrid.cs)가 즉각 감지하여 보관하고 있던 버프 데이터 목록(`_activeBuffSources`)을 진입한 유닛에게 자동으로 갱신/적용(`ApplyBuffFromSource`)합니다.
    3.  버프 유닛이 죽거나 이동하면, 영향권 내의 모든 그리드에서 버프 소스를 제거(`RemoveBuffSource`)하고 즉시 피적용 유닛의 스탯을 원복시킵니다.

### 4.2 다형성을 활용한 틱(Tick) 기반 버프 아키텍처

단순 스탯 증가 버프 외에도 주기적인 리젠(HP 회복), 지속 피해(도트 데미지) 등을 단일 코루틴 남발 없이 처리하기 위해 `MyBuff` 추상 클래스와 `BuffData` 구조를 결합한 중앙 업데이트 방식을 채택했습니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: 다형적 버프 실행 루프</summary>

```csharp
// BuffData_Base.cs: 다형적 버프 업데이트 루프
public virtual void Tick(float deltaTime) {
    // 딕셔너리 순회 중 요소 삭제 에러 방지를 위해 복사본 리스트로 순회
    foreach (int key in new List<int>(activeBuffs.Keys)) {
        // BuffData 내부에서 누적 시간을 체크하여 틱 주기 도달 시 true 반환
        if (activeBuffs[key].Tick(deltaTime)) {
            OnTick(activeBuffs[key]); // 다형성에 의해 상속받은 자식 클래스의 고유 로직 실행
        }
    }
}

// Buff_MP3.cs: 구체적인 힐링(HP 지속 회복) 버프 클래스 구현
public class Buff_MP3 : MyBuff {
    public override void Init(Transform _target) {
        buffTarget = _target;
        buffParticleType = Define.ParticleType.Buff_MP3;
    }

    protected override void OnTick(BuffData buffData) {
        // 대상 오브젝트의 IHealth 인터페이스를 찾아 체력 회복
        var health = buffTarget.GetComponent<IHealth>();
        if (health != null) {
            health.Heal(buffData.Amount);
        }
    }
}
```
</details>

---

## 5. 전체 시스템 기술적 특징 요약
{: .chapter-title }

*   **성능 최적화 (Optimization):** UI 구조에서 물리 공간 오브젝트 구조로 전면 교체하여 **Canvas Rebuilding 부하를 0으로** 격리시켰으며, 마우스 드래그 시 인접 3x3 영역만 거리를 탐색하는 **공간 분할 알고리즘**으로 불필요한 연산을 대폭 감축했습니다.
*   **지능형 상호작용 (Mediator Pattern):** 개별 유닛의 업데이트 루프에 의존하지 않는 중앙 집중식 `InputManager`를 구축하고 렌더링 우선순위 판정을 수식화하여, 깔끔하게 설계된 `IInteractable` 객체 이벤트 구조를 실현했습니다.
*   **데이터 무결성 (Transaction-Safe):** 배치 취소, 배치 위치 이동, 장애물 충돌 등으로 발생 가능한 데이터 오차를 예방하기 위해 사전 예외 검증과 배치 롤백이 보장되는 원자적 배치 단계를 구현했습니다.
*   **유연한 확장성 (Extensibility):** 비트 필터링 느낌의 플래그 기반 그리드 검색 시스템과 다형성 버프 구조를 구현하여, 향후 새로운 유닛 유형이나 기믹이 추가되더라도 기존 코드를 크게 수정하지 않고 확장할 수 있는 견고한 OOP 기반 아키텍처를 확보했습니다.

{: .notice--success}
**기술적 성과:** 윈도우 바탕화면의 파일 정렬 및 압축(Folder ZIP) 등의 조작 감성을 유니티 엔진 상에서 고성능으로 재해석하였으며, 대규모 유닛과 바이러스가 중첩되는 난전 상황 속에서도 **지속적인 60 FPS 이상을 안정적으로 방어**하는 고품질 인디 게임 코어 시스템을 완성했습니다.
