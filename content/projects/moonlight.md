---
title: "PROJECT REPORT // 보름달 방앗간"
excerpt: "A* 길찾기와 커스텀 타일 데이터를 기반으로 직원·손님의 타이쿤 서비스 루프를 구현한 Unity 프로젝트"
permalink: "/project/moonlight/"
tags: [Unity, C#, A* Pathfinding, Tilemap, NPC AI]
---

보름달 방앗간은 손님을 맞이하고 음식을 조리·서빙하며 매장을 운영하는 **2D 타이쿤 게임**입니다. 저는 Unity Tilemap 위에서 동작하는 **A* 길찾기**, **커스텀 타일 데이터**, **직원·손님 AI**를 구현해 매장의 공간 정보가 실제 서비스 흐름으로 이어지도록 설계했습니다.

---

## 1. 프로젝트 개요
{: .chapter-title }

이 프로젝트의 핵심 과제는 “NPC를 목적지까지 이동시키는 것”에 그치지 않고, 매장 안의 좌석과 조리 도구가 계속 사용되고 반환되는 타이쿤의 운영 흐름을 안정적으로 연결하는 것이었습니다.

<div class="pf-visual-frame">
    <div class="pf-grid">
        <div class="pf-grid-item">
            <span class="pf-grid-label">Grid & Tile</span>
            <p class="pf-grid-desc">Tilemap 셀을 이동 노드로 변환하고<br>가구별 점유 데이터를 좌표로 관리</p>
        </div>
        <div class="pf-grid-item">
            <span class="pf-grid-label">A* Pathfinding</span>
            <p class="pf-grid-desc">4방향 탐색과 Manhattan 휴리스틱으로<br>직원·손님의 이동 경로 계산</p>
        </div>
        <div class="pf-grid-item">
            <span class="pf-grid-label">Customer AI</span>
            <p class="pf-grid-desc">입장·자리 탐색·주문·식사·퇴장으로<br>이어지는 손님 상태 흐름</p>
        </div>
        <div class="pf-grid-item">
            <span class="pf-grid-label">Staff AI</span>
            <p class="pf-grid-desc">주문 큐와 조리 도구 예약을 바탕으로<br>조리·서빙 업무 수행</p>
        </div>
    </div>
</div>

| 구분 | 내용 |
| --- | --- |
| 장르 | 2D 타이쿤 / 매장 운영 |
| 개발 환경 | Unity, C# |
| 담당 시스템 | A* 길찾기, GridManager, 커스텀 테이블·의자 타일, 직원·손님 AI |
| 핵심 목표 | 공간 점유 상태와 NPC 행동을 하나의 서비스 사이클로 연결 |

---

## 2. 커스텀 타일 기반 공간 데이터 시스템
{: .chapter-title }

### 2.1 Tilemap을 탐색 가능한 Grid로 변환

`GridManager`는 Tilemap의 각 셀을 순회하며 셀 좌표, 월드 좌표, 이동 가능 여부를 가진 `Grid` 노드를 생성합니다. NPC는 월드 좌표를 직접 비교하지 않고 `WorldToCell`과 `CellToWorld`를 통해 동일한 그리드 좌표계를 사용합니다.

<details class="pf-details">
<summary>코드 보기: Tilemap을 Grid 노드로 변환</summary>

```csharp
for (int x = 0; x < gridSize.x; x++)
{
    var row = new List<Grid>();
    for (int y = 0; y < gridSize.y; y++)
    {
        Vector3Int cell = new Vector3Int(x, y, 0);
        Vector3 worldPosition = tilemap.CellToWorld(cell);
        bool isWalkable = tilemap.HasTile(cell);

        row.Add(new Grid(isWalkable, worldPosition, x, y));
    }
    grid.Add(row);
}
```

</details>

이 구조를 통해 클릭 지점, 좌석 위치, 조리 도구 위치처럼 서로 다른 출처의 좌표도 A*가 사용하는 노드로 일관되게 변환할 수 있습니다.

### 2.2 커스텀 테이블·의자 타일 데이터

테이블과 의자는 단순한 이미지 타일이 아니라 게임 상태를 가진 커스텀 타일입니다. `CustomTableTile`과 `CustomChairTile`에 사용 가능 여부를 저장하고, `GridManager`가 이를 셀 좌표 기반 Dictionary로 관리합니다.

<details class="pf-details">
<summary>코드 보기: 커스텀 타일 데이터와 좌표별 Dictionary</summary>

```csharp
public Dictionary<Vector3Int, CustomTableTile> tablesDictionary;
public Dictionary<Vector3Int, CustomChairTile> chairesDictionary;

public class TableTileData
{
    public bool isUseAble;
    public string tileName;
    public int tableUseAbleIndex = 4;
}
```

</details>

초기화 시 타일별 데이터를 복제해 등록했기 때문에, 같은 타일 에셋을 사용하는 좌석이라도 각 셀의 점유 상태를 독립적으로 다룰 수 있습니다.

### 2.3 좌석과 조리 도구의 점유 관리

손님이 좌석을 선택하면 테이블의 잔여 수용 수와 의자의 `isUseAble` 값을 갱신하고, 식사를 마치고 퇴장할 때 다시 반환합니다. 직원이 조리 도구를 선택하는 과정도 동일하게 사용 가능 상태를 선점하고, 서빙 단계에서 반납합니다.

<div class="pf-visual-frame">
    <div class="pf-transaction-flow">
        <div class="pf-flow-step"><strong>탐색</strong><br>좌표별 자원 조회</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>선점</strong><br>isUseAble = false</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>사용</strong><br>착석 또는 조리</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>반납</strong><br>isUseAble = true</div>
    </div>
</div>

이 점유 플래그는 여러 NPC가 같은 의자나 조리 도구를 동시에 목표로 선택하는 상황을 줄이고, 타이쿤 게임의 한정된 설비를 명시적인 자원으로 다루게 합니다.

---

## 3. A* 길찾기 알고리즘
{: .chapter-title }

### 3.1 노드 비용과 탐색 기준

각 `Grid` 노드는 시작점부터의 실제 비용 `gCost`, 목적지까지의 휴리스틱 비용 `hCost`, 그리고 부모 노드를 보관합니다. 최종 우선순위는 두 비용의 합인 `FCost`로 결정합니다.

<details class="pf-details">
<summary>코드 보기: A* 노드의 F 비용 계산</summary>

```csharp
public int FCost => gCost + hCost;
```

</details>

매장 이동은 상·하·좌·우 4방향으로 제한했고, 이에 맞춰 Manhattan Distance를 휴리스틱으로 사용했습니다.

<details class="pf-details">
<summary>코드 보기: Manhattan Distance 휴리스틱</summary>

```csharp
int GetDistance(Grid nodeA, Grid nodeB)
{
    int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
    int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
    return 10 * (distanceX + distanceY);
}
```

</details>

### 3.2 Open Set과 Closed Set 탐색

탐색할 후보는 Open Set, 검사가 끝난 노드는 Closed Set으로 분리합니다. 현재 후보 중 `FCost`가 가장 낮은 노드를 선택하고, 비용이 같다면 목적지에 더 가까운 `hCost`가 낮은 노드를 우선합니다.

<details class="pf-details">
<summary>코드 보기: Open Set의 최우선 노드 선택</summary>

```csharp
if (candidate.FCost < current.FCost ||
    candidate.FCost == current.FCost && candidate.hCost < current.hCost)
{
    current = candidate;
}
```

</details>

이후 이동 불가능하거나 이미 방문한 이웃은 제외하고, 더 짧은 경로가 발견되면 비용과 부모 노드를 갱신합니다.

### 3.3 경로 역추적과 NPC 이동

목적지에 도달하면 각 노드가 가진 `parent`를 시작점까지 역추적한 뒤 리스트를 뒤집어 이동 경로를 생성합니다. `NPCMovement`와 `CustomerMovement`는 이 리스트를 순서대로 소비하며 셀의 월드 위치로 이동합니다.

<div class="pf-visual-frame">
    <div class="pf-transaction-flow">
        <div class="pf-flow-step">World Position</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step">WorldToCell</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step">A* Node Search</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step">Parent Retrace</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step">NPC Movement</div>
    </div>
</div>

길찾기와 실제 이동을 분리했기 때문에, 직원과 손님은 서로 다른 행동 상태를 가지면서도 같은 경로 계산 계층을 재사용합니다.

---

## 4. 손님 AI: 입장부터 퇴장까지의 서비스 루프
{: .chapter-title }

### 4.1 상태 기반 행동 분리

손님의 전체 행동은 다음 상태로 구성했습니다.

```text
Enter → FindingTable → MovingToChair → Sitting
      → EatFood → OutStore → None
```

| 상태 | 주요 행동 |
| --- | --- |
| Enter | 매장 입구까지 이동하고 입장 완료 확인 |
| FindingTable | 사용 가능한 테이블과 인접 의자 탐색 |
| MovingToChair | A*로 의자까지 이동 |
| Sitting | 주문 UI를 활성화하고 메뉴 선택 |
| EatFood | 음식 수령 후 식사 애니메이션 재생 |
| OutStore | 결제 처리, 좌석 반환, 출구 이동 |

`ICustomerLogic` 코루틴이 현재 상태에 맞는 행동을 실행하며, 이동이나 식사처럼 시간이 필요한 동작은 완료 조건을 기다린 뒤 다음 상태로 전환합니다.

### 4.2 테이블과 인접 의자 선택

손님은 사용 가능한 테이블을 먼저 찾고, 해당 테이블의 좌우 인접 셀에서 비어 있는 의자를 확인합니다. 좌석이 확정되는 시점에 테이블 수용 수와 의자 점유 플래그를 즉시 갱신합니다.

<div class="highlight-box">
    <strong>설계 포인트:</strong> “테이블 찾기”와 “의자 확정”을 분리해, 가구의 시각적 배치와 실제 이용 가능 상태가 손님 AI의 의사결정에 직접 반영되도록 구성했습니다.
</div>

### 4.3 식사·결제·자원 반환

직원에게 음식을 전달받으면 손님은 `EatFood` 상태로 전환하고, 식사 애니메이션이 끝나면 `OutStore`로 이동합니다. 이 단계에서 매출을 반영하고 테이블과 의자를 다시 사용 가능한 상태로 돌린 뒤 출구까지 A* 경로를 요청합니다.

이렇게 손님의 한 사이클이 끝날 때 공간 자원도 함께 복구되므로, 다음 손님이 같은 좌석을 정상적으로 사용할 수 있습니다.

---

## 5. 직원 AI: 주문 큐와 조리·서빙 파이프라인
{: .chapter-title }

### 5.1 직원 상태 머신

직원은 네 가지 핵심 상태로 동작합니다.

```text
Roaming → MovingToCook → Cooking → ServingDish
   ↑                                      │
   └──────── 주문 처리 완료 ───────────────┘
```

- `Roaming`: 가구가 없는 셀 중 하나를 선택해 매장을 배회합니다.
- `MovingToCook`: 주문 음식에 맞는 조리 도구를 예약하고 이동합니다.
- `Cooking`: 도구별 작업 스프라이트와 애니메이션을 재생합니다.
- `ServingDish`: 손님 위치까지 이동해 음식을 전달합니다.

### 5.2 음식·손님을 연결하는 주문 큐

직원은 `Queue<FoodItem>`과 `Queue<CustomerAI>`를 함께 사용합니다. 새로운 주문이 들어오면 음식과 손님을 같은 순서로 enqueue하고, 서빙 완료 시 각각 dequeue해 해당 손님에게 조리된 음식과 가격 정보를 전달합니다.

<details class="pf-details">
<summary>코드 보기: 음식·손님 주문 큐 처리</summary>

```csharp
public Queue<FoodItem> order_foodType = new Queue<FoodItem>();
public Queue<CustomerAI> order_Customer = new Queue<CustomerAI>();

CustomerAI customer = order_Customer.Dequeue();
FoodItem food = order_foodType.Dequeue();
customer.TakeFoodToStaff(food.foodPrice);
```

</details>

주문이 더 남아 있다면 다음 손님과 음식으로 다시 `MovingToCook` 상태에 진입하고, 큐가 비면 `Roaming`으로 복귀합니다.

### 5.3 메뉴에 맞는 조리 도구 예약

음식 종류에 따라 절구(`Mortar`) 또는 장독대(`Jangdokdae`)를 선택합니다. 사용 가능한 도구를 발견하면 먼저 점유 처리한 후 A* 경로를 계산하고, 조리와 서빙이 끝났을 때 다시 반환합니다.

이 구조는 직원의 상태 전이, 주문 순서, 설비 점유가 서로 어긋나지 않도록 하나의 작업 파이프라인으로 묶어 줍니다.

---

## 6. 시스템 통합과 기술적 성과
{: .chapter-title }

### 6.1 하나의 공간 모델을 공유하는 NPC

직원과 손님은 행동 목적은 다르지만 다음 기반 시스템을 공유합니다.

<div class="pf-arch-diagram">
    <div class="pf-arch-layer">
        <div class="pf-arch-layer-title">Behavior Layer</div>
        <div class="pf-arch-layer-items">
            <span class="pf-arch-item">Customer State Machine</span>
            <span class="pf-arch-item">Staff State Machine</span>
            <span class="pf-arch-item">Order Queue</span>
        </div>
    </div>
    <div class="flow-arrow">↓</div>
    <div class="pf-arch-layer">
        <div class="pf-arch-layer-title">Navigation Layer</div>
        <div class="pf-arch-layer-items">
            <span class="pf-arch-item">A* Search</span>
            <span class="pf-arch-item">Path Retrace</span>
            <span class="pf-arch-item">NPC Movement</span>
        </div>
    </div>
    <div class="flow-arrow">↓</div>
    <div class="pf-arch-layer">
        <div class="pf-arch-layer-title">Spatial Data Layer</div>
        <div class="pf-arch-layer-items">
            <span class="pf-arch-item">Unity Tilemap</span>
            <span class="pf-arch-item">Grid Nodes</span>
            <span class="pf-arch-item">Furniture Occupancy</span>
        </div>
    </div>
</div>

### 6.2 포트폴리오 핵심 요약

- **커스텀 타일 데이터:** 테이블·의자의 사용 가능 상태와 수용 정보를 셀 좌표별 데이터로 관리했습니다.
- **A* 직접 구현:** Open/Closed Set, G/H/F 비용, Manhattan 휴리스틱, 부모 노드 역추적으로 4방향 경로를 계산했습니다.
- **공통 이동 계층:** 직원과 손님이 동일한 길찾기 결과를 각자의 이동 컴포넌트에서 재사용하도록 분리했습니다.
- **상태 머신 기반 NPC:** 긴 행동 흐름을 명시적인 상태와 완료 조건으로 나눠 읽기 쉽고 확장 가능한 구조로 구성했습니다.
- **자원 예약과 주문 처리:** 좌석·조리 도구 점유 플래그와 FIFO 주문 큐를 연결해 타이쿤의 서비스 순환을 구현했습니다.

<div class="highlight-box">
    <strong>Conclusion:</strong> 보름달 방앗간은 A* 알고리즘을 독립 기능으로 구현하는 데서 끝나지 않고, 커스텀 타일의 점유 정보와 직원·손님의 상태 전이를 결합해 실제 타이쿤 플레이를 움직이는 시스템으로 확장한 프로젝트입니다.
</div>
