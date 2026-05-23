---
layout: single
title: "PROJECT REPORT // FILE_TOWER_DEFENSE"
excerpt: "Systems Engineering: UI-to-GameObject 리팩토링, 통합 입력 시스템 및 그리드 아키텍처"
categories: [project]
permalink: /project/file-tower-defense/
tags: [Unity, Optimization, Architecture, Algorithms]
toc: true
toc_sticky: true
classes: wide
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

이를 해결하기 위해 모든 유닛을 **GameObject(Transform) 기반**으로 전면 교체하여 월드 좌표계로 통일하였으며, Physics2D와의 직접적인 호환성을 확보하여 성능을 최적화했습니다.

### 1.2 GameObjectGridLayout: 커스텀 레이아웃 엔진

화면 해상도에 맞춰 그리드의 간격과 셀 크기를 동적으로 계산하는 레이아웃 엔진을 구축했습니다.

<div class="pf-visual-frame" markdown="1">
**계산 공식:**  
`cellSize = (screenSize - padding*2 - spacing*(n-1)) / n`
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

<div class="pf-visual-frame" markdown="1">
<div class="pf-arch-diagram" markdown="1">
<div class="pf-arch-layer" markdown="1">
<div class="pf-arch-layer-title">Input Layer (Mouse/Key)</div>
</div>
<div style="text-align: center; color: #007bff; font-size: 1.2rem;">↓</div>
<div class="pf-arch-layer" markdown="1">
<div class="pf-arch-layer-title">InputManager (Central Hub)</div>
<div class="pf-arch-layer-items" markdown="1">
<span class="pf-arch-item">Sorting Priority Check</span>
<span class="pf-arch-item">InputState Machine</span>
</div>
</div>
<div style="text-align: center; color: #007bff; font-size: 1.2rem;">↓ Dispatch</div>
<div class="pf-arch-layer" markdown="1">
<div class="pf-arch-layer-title">InteractionHandler (Mediator)</div>
<div class="pf-arch-layer-items" markdown="1">
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

### 3.2 원자적 배치 트랜잭션 (Transactional Pattern)

데이터 불일치를 방지하고 실패 시 안전한 롤백을 위해 3단계 배치 로직을 설계했습니다.

1.  **Pre-validation:** 대상 그리드의 점유 여부 및 장애물 존재 여부 체크.
2.  **Rollback Prep:** 기존 그리드에서 유닛 정보 및 버프 소스 안전하게 제거.
3.  **Commit:** 새 그리드에 부모-자식 관계 설정 및 위치 동기화, 인접 버프 자동 적용.

### 3.3 동적 양방향 좌표 변환 시스템

월드 좌표와 그리드 인덱스 간의 완전한 호환성을 제공합니다.

<div class="pf-visual-frame" markdown="1">
<div class="pf-coord-flow" markdown="1">
<div class="pf-coord-box" markdown="1">
<div class="pf-coord-box-title">World Position</div>
<div class="pf-coord-box-formula">InverseTransformPoint(worldPos)</div>
</div>
<div style="display: flex; align-items: center; font-size: 1.5rem; color: #007bff;">⇄</div>
<div class="pf-coord-box" markdown="1">
<div class="pf-coord-box-title">Grid Index (X, Y)</div>
<div class="pf-coord-box-formula">RoundToInt((local - start) / cellSize)</div>
</div>
</div>
</div>

### 3.4 공간 분할 기반 탐색 최적화 (O(1))

전체 그리드를 순회하는 대신, 마우스 위치의 인덱스를 기준으로 **인접 3x3 영역**만 검사하도록 설계하여 탐색 비용을 획기적으로 단축했습니다.

<div class="pf-visual-frame" markdown="1">
<div class="pf-diagram-grid" markdown="1">
<div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div>
<div class="pf-grid-cell near"></div><div class="pf-grid-cell active"></div><div class="pf-grid-cell near"></div>
<div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div><div class="pf-grid-cell near"></div>
</div>
<p style="font-size: 0.85rem; color: #666; margin-top: 10px;">Spatial Partitioning: Searching 9 cells instead of 91</p>
</div>

---

## 4. 다형성 기반 동적 버프 시스템
{: .chapter-title }

### 4.1 Observer 패턴 기반 자동 동기화

파일이 그리드에 배치되는 순간, 그리드가 이미 보유한 버프 소스(Aura)들을 유닛에게 즉각 적용합니다. 버프 소스와 그리드 간의 느슨한 결합으로 시스템 확장성을 확보했습니다.

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
