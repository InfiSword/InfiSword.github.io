# 🖥️ [Trouble Shooting] File Tower Defense 포트폴리오 슬라이드 가이드 (3장)

이 문서는 **파일 타워 디펜스(File Tower Defense)** 프로젝트의 핵심 문제 해결 과정(Trouble Shooting)을 포트폴리오 슬라이드(2~3장) 형태로 복사하여 사용할 수 있도록 최적화한 가이드입니다. 

---

## 📂 [Slide 1] UI(RectTransform)에서 월드 오브젝트(Transform)로의 전환 및 최적화
> **핵심 키워드:** Canvas Rebuilding 제거, 좌표계 단일화, 해상도 대응 커스텀 레이아웃

### 1. 상황 및 문제점 (Trouble)
*   **초기 설계:** 윈도우 바탕화면의 파일 정렬 및 드래그 감성을 살리기 위해 모든 파일 유닛을 UI 시스템(`RectTransform`) 기반으로 제작.
*   **성능 저하 (Canvas Rebuilding):** 유닛들이 드래그되거나 호버 애니메이션이 발생할 때마다 Unity 내부에서 Canvas 전체가 재구성(Rebuild)되어 CPU 병목 및 프레임 드랍(30 FPS 이하) 발생.
*   **좌표계 불일치:** 월드 공간에서 움직이는 적(바이러스)과 UI 공간의 유닛 간 사거리 및 충돌 판정을 위해 매 프레임 `Camera.WorldToScreenPoint` 등의 좌표 변환 연산을 호출하여 가비지 컬렉션(GC) 및 불필요한 연산 초과.

### 2. 해결 방안 및 구현 (Shooting)
*   **월드 오브젝트 전환:** 모든 파일 유닛을 GameObject와 일반 `Transform` 기반으로 전면 교체하여 월드 좌표계로 단일화. UI Canvas 영역에서 해방되어 Canvas Rebuild 부하를 **0%**로 제거.
*   **네이티브 2D 물리 적용:** 유닛과 바이러스에 2D Collider를 적용하여 월드 공간에서 직접 충돌 및 사거리를 감지하도록 설계.
*   **커스텀 레이아웃 엔진 개발:** UI용 `GridLayoutGroup`을 대체하기 위해, 월드 공간에서 해상도 및 카메라 크기를 기반으로 셀 스케일을 자동 계산하는 [GameObjectGridLayout.cs](file:///D:/Workspace/codeReference/file_tower_defence/GameObjectGridLayout.cs)를 직접 구현.

### 3. 핵심 코드 스니펫 (PPT용 최적화)
```csharp
// GameObjectGridLayout.cs - 카메라 오르토그래픽 해상도 기반 셀 크기 자동 계산
public void FitToScreen()
{
    Camera cam = Camera.main;
    float height = cam.orthographicSize * 2f; // 카메라 세로 월드 크기
    float width = height * cam.aspect;         // 가로 비율 계산

    // 가용 해상도 영역에서 패딩과 스페이싱을 제외한 순수 셀(Grid) 크기 계산
    cellSize.x = (width - (padding.x * 2) - (spacing.x * (columns - 1))) / columns;
    cellSize.y = (height - (padding.y * 2) - (spacing.y * (rows - 1))) / rows;

    // 배치 시작 위치를 카메라 좌상단 월드 좌표 기준으로 설정
    StartPos = new Vector2(-(width / 2) + padding.x + (cellSize.x / 2), 
                            (height / 2) - padding.y - (cellSize.y / 2));
}
```

### 4. 결과 및 피드백 (Result)
*   **성능 개선:** 대규모 유닛 배치 시 발생하던 Canvas Rebuild 병목을 완벽히 제거하여 **평균 프레임 60 FPS 이상으로 방어 성공**.
*   **생산성 증가:** 좌표계 변환을 위한 수동 보정 코드들이 사라져 물리 엔진(Physics 2D)의 기능을 100% 활용할 수 있게 됨.

---

## 📂 [Slide 2] Mediator 패턴 기반 통합 입력 및 다중 레이어 상호작용 해결
> **핵심 키워드:** InputManager 중심 제어, 정렬 순서(Sorting Order) 판정, UI 클릭 차단

### 1. 상황 및 문제점 (Trouble)
*   **입력 감지 분산:** 수십 개의 유닛이 개별적으로 `OnMouseDown`이나 `Update` 내에서 마우스를 감지하는 구조로 인해, 유닛이 많아질수록 마우스 체크 연산이 비례해서 증가 ($O(N)$).
*   **입력 꼬임 및 판정 모호성:** 월드 공간에 유닛들이 겹쳐 있거나, 유닛 뒤로 적(바이러스)이 지나갈 때, 혹은 화면 위의 UI 버튼을 클릭했음에도 뒤쪽의 월드 유닛이 동시에 클릭되는 현상 발생.

### 2. 해결 방안 및 구현 (Shooting)
*   **Mediator 패턴 도입:** 단 하나의 [InputManager.cs](file:///D:/Workspace/codeReference/file_tower_defence/InputManager.cs)만 매 프레임 입력을 감지하고, 감지된 이벤트를 [InteractionHandler.cs](file:///D:/Workspace/codeReference/file_tower_defence/InputManager.cs)를 거쳐 대상 오브젝트에 전파하는 구조로 설계를 단일화.
*   **정밀 정렬 판정(Sorting Priority):** `Physics2D.RaycastNonAlloc`을 사용하여 마우스 위치에 중첩된 모든 오브젝트를 무부하 탐색한 뒤, 각 오브젝트의 `Sorting Layer`와 `Order in Layer` 값을 비교하여 가장 화면 최상단에 렌더링된 객체 하나만 상호작용 대상으로 선정.
*   **UI 클릭 원천 차단:** 마우스 포인터가 UI 위에 위치하는 경우(`IsPointerOverUI`) 월드 공간 레이캐스트 연산을 조기 종료(Early Return)하여 UI와 월드 객체 간의 입력 간섭을 완벽 분리.

### 3. 핵심 코드 스니펫 (PPT용 최적화)
```csharp
// InputManager.cs - 중첩된 콜라이더 중 최상단 렌더링 객체 탐색 및 UI 차단
private IInteractable GetInteractableUnderMouse()
{
    // 1. UI 마우스 오버 시 월드 탐색 생략 (Early Return)
    if (IsPointerOverUI(Input.mousePosition)) return null;

    // 2. 가비지 컬렉터(GC) 자극 없는 Raycast 비할당 탐색
    int hitCount = Physics2D.RaycastNonAlloc(m_mouseWorldPosition, Vector2.zero, raycastHits, Mathf.Infinity, interactableLayerMask);
    if (hitCount == 0) return null;

    IInteractable bestTarget = null;
    int bestSortingOrder = int.MinValue;

    for (int i = 0; i < hitCount; i++)
    {
        var target = raycastHits[i].collider.GetComponent<IInteractable>();
        if (target == null) continue;

        // 3. SpriteRenderer의 Order in Layer를 대조하여 가장 위에 보이는 객체 선택
        var sRenderer = raycastHits[i].collider.GetComponent<SpriteRenderer>();
        int sortingOrder = sRenderer != null ? sRenderer.sortingOrder : int.MinValue;

        if (sortingOrder > bestSortingOrder)
        {
            bestSortingOrder = sortingOrder;
            bestTarget = target;
        }
    }
    return bestTarget;
}
```

### 4. 결과 및 피드백 (Result)
*   **입력 무결성 보장:** 클릭 간섭 현상이 사라져 플레이어 조작 피드백이 극도로 향상됨.
*   **구조적 결합도 감소:** 개별 유닛 클래스에서 마우스 클릭 감지용 보일러플레이트 코드가 제거되어 코드가 80% 이상 간결해짐.

---

## 📂 [Slide 3] O(1) 공간 분할 및 트랜잭션 기반 그리드 배치 시스템
> **핵심 키워드:** 인접 3x3 탐색 최적화, 배치 트랜잭션(Transaction) 및 롤백, 가변 플래그 검색

### 1. 상황 및 문제점 (Trouble)
*   **그리드 탐색 부하:** 유닛을 드래그하여 이동시킬 때마다 실시간으로 가장 가까운 안착 그리드 셀을 계산해야 함. 전체 그리드판($N \times M$개)을 전수 비교 계산($O(N)$)할 시 드래그 동작 중 프레임 드랍(Stuttering)이 일어남.
*   **배치 정합성 훼손:** 드래그 취소, 장애물 겹침, 유닛 간 겹침 상태에서 연산 순서가 꼬이면 이전 그리드에서 유닛 정보가 누락되거나 버프 수치가 꼬여버리는 데이터 왜곡 발생.

### 2. 해결 방안 및 구현 (Shooting)
*   **공간 분할(Spatial Partitioning) 3x3 검색:** 월드 좌표를 수학적으로 즉시 변환하여 1개의 대상 그리드 인덱스를 $O(1)$로 계산한 뒤, 전체 맵이 아닌 **인접 3x3 셀(총 9개)**만 루프를 돌며 거리 제곱을 비교하도록 설계하여 연산량을 상수로 고정.
*   **원자적 트랜잭션(Transactional) 배치:** 데이터 꼬임을 차단하기 위해 유닛 배치를 3단계 공정으로 모듈화.
    1.  `Validate` (대상 셀의 장애물 유무 및 중복 점유 검증)
    2.  `Rollback Prep` (검증 실패 시 이전 상태 및 버프 영향력 원복 보장)
    3.  `Commit` (이동 완료 처리, 이전 그리드 데이터 제거 및 신규 그리드 버프 전파)
*   **플래그 기반 일반화 검색:** `params SearchGridFlag[] flags` 배열을 지원하여 `NotOccupied`(유닛 없음), `NotObstacle`(장애물 없음) 등 복잡한 조건 필터링 쿼리를 단일 함수로 처리할 수 있게 설계.

### 3. 핵심 코드 스니펫 (PPT용 최적화)
```csharp
// FileGridManager.cs - 공간 분할 기반 O(1) 3x3 인접 그리드 검색
public FileGrid GetGrid(Vector2 worldPos)
{
    // 1. 월드 좌표 ➔ 그리드 좌표 인덱스로 다이렉트 수학적 변환 (O(1))
    if (!WorldToGridIndex(worldPos, out int xCenter, out int yCenter)) return null;

    if (IsValidGridIndex(xCenter, yCenter))
    {
        float minDistSqr = float.MaxValue;
        FileGrid closestGrid = null;

        // 2. 인덱스 기준 3x3(주변 9개 칸) 영역만 정밀 계산
        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                int x = xCenter + dx; int y = yCenter + dy;
                if (IsValidGridIndex(x, y)) {
                    FileGrid candidate = gridArray[x, y];
                    // 성능에 악영향을 주는 Sqrt(제곱근)를 배제하고 sqrMagnitude로 최단 거리 비교
                    float distSqr = ((Vector2)candidate.transform.position - worldPos).sqrMagnitude;
                    if (distSqr < minDistSqr) {
                        minDistSqr = distSqr; closestGrid = candidate;
                    }
                }
            }
        }
        return closestGrid;
    }
    return FindGlobalClosestGrid(worldPos); // 예외 복구용 전체 스캔
}
```

### 4. 결과 및 피드백 (Result)
*   **최적화 완료:** 수십 개의 유닛을 동시에 드래그하거나 대형 드래그 박스로 조작하는 런타임 환경에서도 **탐색 시간 0.1ms 이하**를 기록하여 극도의 버벅임 해소.
*   **데이터 안정성:** 트랜잭션 롤백 시스템 도입으로 어떠한 돌발적인 취소나 오작동 상황에서도 **스탯 버프 및 위치 정보의 정합성이 100% 유지**됨.
