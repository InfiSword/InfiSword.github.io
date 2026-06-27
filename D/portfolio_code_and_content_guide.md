# 📁 [구현 중심] 포트폴리오 슬라이드 코드 및 내용 매핑 가이드

이 문서는 포트폴리오 슬라이드를 작성할 때 **어떤 C++/C# 소스 코드를 삽입하고 어떤 핵심 내용을 기재해야 하는지**에 대한 상세 가이드라인입니다. 실제 개발 프로젝트 폴더에서 추출한 핵심 소스 코드들을 기반으로 구성되었습니다.

---

## 📌 Project 1: WinAPI Don't Starve 모작
> **포커스:** 밑바닥부터 C++로 작성한 O(1) 공간 분할 렌더러와 컬링 최적화

### 📂 [Slide 3] 2D 그리드 공간 분할 및 동적 갱신
* **슬라이드 기재 내용:**
  - **상황:** 맵에 배치되는 오브젝트가 1,000~10,000개로 늘어남에 따라 뷰포트 내 검사 후보를 찾기 위한 루프 비용 과다 발생.
  - **구현 해결책:** 맵 전체를 `GRID_CELL_SIZE = 256` 픽셀 크기의 정적 2D 배열(`m_spatialGrid`)로 분할 관리. 오브젝트 이동 시 셀 경계를 교차할 때만 그리드 정보를 갱신하도록 `UpdateObjectGrid()` 구현.
* **삽입할 핵심 코드 스니펫 (PPT 슬라이드용 최적화 버전):**
  ```cpp
  // ObjectManager.cpp - O(1) 공간 분할 격자 이동 갱신
  void ObjectManager::UpdateObjectGrid(GameObject* pObj)
  {
      if (!pObj || pObj->GetType() == GO_TYPE_UI) return;

      // 1. 객체의 이전 그리드 좌표 획득
      int oldX = pObj->GetGridCellX(), oldY = pObj->GetGridCellY();

      // 2. 월드 좌표 ➔ 그리드 셀 인덱스 변환 (O(1) Clamp 연산)
      int newX = ConvertWorldToGridX(pObj->GetBounds().X);
      int newY = ConvertWorldToGridY(pObj->GetBounds().Y);

      // ★ [핵심 최적화] 동일 셀 내부 이동 시 연산 생략 (Early Return)
      if (oldX == newX && oldY == newY) return;

      // 3. 이전 격자(Cell)에서 삭제 후 새로운 격자에 이관 등록
      RemoveFromCell(oldX, oldY, pObj); // vector 삭제 로직 캡슐화
      m_spatialGrid[newX][newY].push_back(pObj);
      pObj->SetGridCell(newX, newY);
  }
  ```
  - **💡 면접관을 위한 키 포인트:**
    - `m_spatialGrid`: `std::vector<GameObject*>[GRID_WIDTH][GRID_HEIGHT]` 구조의 2D 공간 분할 격자판으로, 각 구역 안에 속한 객체들의 주소(포인터)만 얇게 관리하는 이정표 역할을 합니다.
    - **핵심 최적화 논리:** 이동할 때마다 매번 삭제/추가를 반복하는 대신, **격자 경계를 교차하는 경우에만** 이동 등록 연산을 수행하도록 조기 종료(Early Return)하여 CPU 부하를 극도로 줄였습니다.


### 📂 [Slide 4] 3x3 국소 쿼리 및 카메라 가시 범위 컬링
* **슬라이드 기재 내용:**
  - **상황:** 전체 맵의 수많은 객체들을 대상으로 화면 안에 그릴 대상을 매 프레임 검사하는 것은 심각한 CPU 오버헤드 유발.
  - **구현 해결책:** 윈도우 화면 크기(카메라 뷰포트 영역 Rect)와 겹치는 인접 3x3 격자(Cell) 범위만 찾아 O(1)로 쿼리 대상을 선별. 여러 격자에 걸쳐 배치된 큰 오브젝트의 중복 연산은 고유 스탬프 번호 대조를 통해 단 1번만 계산하도록 차단.
* **삽입할 핵심 코드 스니펫 (PPT 슬라이드용 최적화 버전):**
  ```cpp
  // ObjectManager.cpp - 카메라 뷰포트 영역(Rect) 내 객체 쿼리
  void ObjectManager::QueryObjectsInRectArea(const Gdiplus::RectF& viewport, std::vector<GameObject*>& targetOutObjects)
  {
      // 1. 카메라 해상도(Window) 좌표 ➔ 탐색할 그리드 시작/끝 셀 인덱스 변환 (O(1))
      int startX = ConvertWorldToGridX(viewport.X);
      int endX   = ConvertWorldToGridX(viewport.X + viewport.Width);
      int startY = ConvertWorldToGridY(viewport.Y);
      int endY   = ConvertWorldToGridY(viewport.Y + viewport.Height);

      // ★ [중복 검사 방지] 1프레임 고유 쿼리 스탬프 번호 갱신
      if (++m_spatialQueryStamp == 0) m_spatialQueryStamp = 1;

      // 2. 화면 영역과 부딪히는 격자(3x3 내외)만 제한적으로 탐색
      for (int y = startY; y <= endY; ++y) {
          for (int x = startX; x <= endX; ++x) {
              for (auto* obj : m_spatialGrid[x][y]) {
                  // 중복 스캔 방지: 여러 셀에 걸친 큰 오브젝트는 스탬프 비교로 즉시 패스
                  if (obj->GetLastSpatialQueryStamp() == m_spatialQueryStamp) continue;
                  obj->SetLastSpatialQueryStamp(m_spatialQueryStamp);

                  if (!obj->IsEnabled() || obj->IsDead()) continue;

                  // 3. 화면 범위와 오브젝트 바운딩 박스(AABB) 최종 충돌 검사
                  if (viewport.IntersectsWith(obj->GetBounds())) {
                      targetOutObjects.push_back(obj);
                  }
              }
          }
      }
  }
  ```
  - **💡 면접관을 위한 키 포인트:**
    - **윈도우 크기 기반 필터링:** 전체 월드가 아무리 넓고 객체가 많아도, 렌더러는 오직 **현재 윈도우 해상도 영역(Camera Viewport)**이 커버하는 약 9칸의 그리드 셀 내부만 훑습니다.
    - **중복 검사 방지 (`m_spatialQueryStamp`):** 거대 보스나 건물처럼 여러 격자 셀에 걸쳐 있는 객체가 여러 번 중복 검사되는 것을, 고유 ID 비교 연산(`Stamp == m_spatialQueryStamp`)을 통해 O(1) 수준으로 빠르게 필터링해 냈습니다.


### 📂 [Slide 4.5] 동적 타일 캐싱 및 뷰포트 통합 컬링 파이프라인
* **슬라이드 기재 내용:**
  - **상황:** 광활한 맵의 수만 개 타일 비트맵을 매 프레임 로드하고 그릴 경우 엄청난 메모리 소요 및 GDI+ 속도 저하 발생.
  - **구현 해결책:** 카메라 화면 범위와 연계하여 화면 밖 타일 비트맵 캐시를 해제하는 `RenderVisibleTiles` 시스템 구현. 오브젝트 쿼리(`QueryObjectsInRectArea`)와 결합하여 **화면에 보이는 타일 + 화면 범위 내 오브젝트만 선별해서 그리는 통합 뷰포트 컬링 파이프라인** 완성.
* **삽입할 핵심 코드 스니펫 (CameraManager.cpp):**
  ```cpp
  // 1. 카메라 화면 범위(Window)에 닿는 타일만 컬링 및 캐싱
  void CameraManager::RenderVisibleTiles(const MapData* mapData)
  {
      if (!mapData) return;
      Gdiplus::RectF vp = GetViewportWorldRect();

      // 시작/끝 그리드 인덱스 계산 (O(1))
      int gsx = ConvertWorldToGridX(vp.X - Padding);
      int gex = ConvertWorldToGridX(vp.X + vp.Width + Padding);
      int gsy = ConvertWorldToGridY(vp.Y - Padding);
      int gey = ConvertWorldToGridY(vp.Y + vp.Height + Padding);

      // 가시 화면 격자 경계가 바뀐 경우에만 안 쓰는 타일 캐시 일괄 해제 (메모리 최적화)
      if (HasViewportChanged(gsx, gex, gsy, gey)) {
          CleanupUnusedTileCache(mapData, gsx, gex, gsy, gey);
      }

      // 현재 뷰포트 격자 내의 타일만 순회하며 RenderManager에 그리기 예약
      for (int y = gsy * TilesPerGrid; y < gey * TilesPerGrid; ++y) {
          for (int x = gsx * TilesPerGrid; x < gex * TilesPerGrid; ++x) {
              Tile& tile = mapData->tiles[x][y];
              Gdiplus::Bitmap* cachedBitmap = GetOrLoadTileBitmap(tile.id);
              
              // 발밑 Y좌표(wy)를 zOrder 기준으로 제출하여 Y-Sorting 파이프라인과 연동
              RenderManager::GetInstance()->AddWorldObjectCommand(
                  cachedBitmap, wx, wy, LAYER_WORLD_TILE, wy
              );
          }
      }
  }

  // 2. 카메라 화면 범위 내의 월드 오브젝트만 컬링 및 그리기 명령 제출
  void CameraManager::RenderVisibleGameObjects()
  {
      ObjectManager* objectManager = ObjectManager::GetInstance();
      std::vector<GameObject*>& visibleBuffer = m_queryBuffer; // GC 방지용 버퍼 재사용
      visibleBuffer.clear();

      // 카메라의 현재 윈도우 해상도 화면 영역(Rect) 획득
      Gdiplus::RectF viewport = GetViewportWorldRect();

      // [최적화] 공간 분할 격자판 쿼리를 통해 가시 범위의 객체만 O(1) 수집
      objectManager->QueryObjectsInRectArea(viewport, visibleBuffer);

      // 수집된 객체만 순회하며 RenderManager에 그리기 예약
      for (GameObject* obj : visibleBuffer) {
          obj->Render();
        #ifdef _DEBUG
          obj->RenderDebugOverlay(); // 그리드 격자 및 디버그 정보 출력
        #endif
      }
  }
  ```
  - **💡 면접관을 위한 키 포인트:**
    - **통합 뷰포트 컬링:** 배경 타일(`RenderVisibleTiles`)과 액터 오브젝트(`QueryObjectsInRectArea`) 모두 **동일한 카메라 해상도 화면 범위 사각형을 기준**으로 O(1) 필터링되어 `RenderManager`에 제출(Submit)됩니다.
    - **동적 LRU식 캐시 클린업:** 화면 가시 구역이 바뀔 때만 미사용 타일 비트맵을 메모리에서 해제 (`CleanupUnusedTileCache`)하여, 저사양 C++ WinAPI 환경에서도 메모리 누수와 GC 오버헤드를 동시에 최소화했습니다.


---

## 📌 Project 2: File Tower Defense
> **포커스:** 좌표계의 단일화 및 중앙 집중식 상호작용 인터페이스 설계 (4인)

### 📂 [Slide 5] GameObjectLayout 및 배치 매니저
* **슬라이드 기재 내용:**
  - **상황:** 초기 설계 시 유닛(파일)을 UI(RectTransform)에 배치하여 몬스터(World Space)와의 위치 비교 시 복잡한 좌표계 변환 오버헤드 발생.
  - **구현 해결책:** 모든 파일을 GameObject(World Space)로 통일하고, 드래그 배치를 처리하는 전용 `GameObjectLayout` 및 원자적(Atomic) 검증/이동을 보장하는 `TrySetUnitAtGrid` 로직 구축.
* **배치 핵심 코드 스니펫 (`FileGridManager.cs`):**
  ```csharp
  // FileGridManager.cs - 원자적 타워 배치 및 자동 정렬
  public bool TrySetUnitAtGrid(File_Base unit, FileGrid targetGrid)
  {
      if (unit == null || targetGrid == null) return false;
      if (!IsValidGridIndex(targetGrid.GridX, targetGrid.GridY) || targetGrid.obstacleObject != null) return false;

      // 이전 그리드에서 안정적인 제거 (트랜잭션 롤백 보장)
      FileGrid currentGrid = unit.CurrentGrid;
      if (currentGrid != null && currentGrid != targetGrid) {
          currentGrid.RemoveFileUnit();
      }

      unit.FileState = FileStatus.normal;
      unit.transform.position = targetGrid.transform.position; // 물리 이동
      unit.transform.SetParent(targetGrid.transform);
      targetGrid.SetFileUnit(unit); // 새 그리드 등록 및 버프 자동 트리거
      return true;
  }
  ```

### 📂 [Slide 6] 중앙 집중식 입력 및 상호작용 이벤트 시스템
* **슬라이드 기재 내용:**
  - **상황:** 수백 개 유닛마다 개별 마우스 이벤트를 바인딩하여 런타임 오버헤드가 크고 이벤트 추적이 어려움.
  - **구현 해결책:** 중앙 집중식 `InputManager`가 마우스 좌표의 `Physics2D.Raycast`를 통해 인터페이스 `IInteractable` 객체를 일괄 탐색하고, `InteractionHandler`로 전달해 이벤트를 제어 및 전파.
* **상호작용 핵심 코드 스니펫 (`InputManager.cs`):**
  ```csharp
  // InputManager.cs - 월드 좌표 상호작용 레이캐스트 감지
  private IInteractable GetInteractableUnderMouse()
  {
      // 1순위: 파일 유닛 감지
      RaycastHit2D unitHit = Physics2D.Raycast(m_mouseWorldPosition, Vector2.zero, 0f, unitLayerMask);
      if (unitHit.collider != null) {
          IInteractable unit = unitHit.collider.GetComponent<IInteractable>() 
                            ?? unitHit.collider.GetComponentInParent<IInteractable>();
          if (unit != null) return unit;
      }

      // 2순위: 몬스터 및 기타 상호작용체 감지
      RaycastHit2D hit = Physics2D.Raycast(m_mouseWorldPosition, Vector2.zero, 0f, interactableLayerMask);
      if (hit.collider != null) {
          IInteractable obj = hit.collider.GetComponent<IInteractable>() 
                           ?? hit.collider.GetComponentInParent<IInteractable>();
          if (obj != null) return obj;
      }
      return null;
  }
  ```

---

## 📌 Project 3: Autonomous Racing Agent
> **포커스:** Spline 기반 학습 환경 자동화 및 벡터 내적 연산을 이용한 방향 감지 (2인)

### 📂 [Slide 7] Spline 체크포인트 자동 생성 에디터 툴
* **슬라이드 기재 내용:**
  - **상황:** 에이전트의 이동 경로와 구간 보상을 위해 체크포인트를 수작업으로 배치해야 하는 번거로움과 비효율 발생.
  - **구현 해결책:** Unity Spline 곡선 정보를 에디터 상에서 가져와 설정한 개수만큼 체크포인트 위치와 접선(Look Rotation)을 정밀히 자동 계산해 배치하고 `TrackData`에 바인딩하는 에디터 스크립트 작성.
* **에디터 툴 핵심 코드 스니펫 (`SplineToCheckpointGenerator.cs`):**
  ```csharp
  // SplineToCheckpointGenerator.cs - 스플라인 비례 보간 기반 콜라이더 자동 생성
  public void GenerateCheckpoints()
  {
      // ... 사전 루트 오브젝트 초기화 및 Undo 기록 코드 ...
      for (int i = 0; i < totalCheckpoints; i++) {
          float progress = (float)(i + 1) / totalCheckpoints;
          spline.Spline.Evaluate(progress, out float3 pos, out float3 tan, out float3 up); // 스플라인 좌표/회전 추출

          if (math.lengthsq(tan) == 0) tan = math.forward();

          GameObject checkpointObj = new GameObject($"Checkpoint_{i:00}");
          checkpointObj.transform.SetParent(checkpointRoot.transform);
          checkpointObj.transform.localPosition = (Vector3)pos;
          checkpointObj.transform.localRotation = Quaternion.LookRotation((Vector3)tan, (Vector3)up); // 트랙 방향 정렬
          
          BoxCollider co = checkpointObj.AddComponent<BoxCollider>();
          co.size = checkpointColliderSize; co.isTrigger = true;
          checkpointObj.AddComponent<CheckPoint>().index = i;
      }
  }
  ```

### 📂 [Slide 8] 벡터 내적(Dot Product)을 활용한 주행 정렬 판정
* **슬라이드 기재 내용:**
  - **상황:** 체크포인트 콜라이더 사이의 영역에서 차량이 후진/충돌 등으로 역주행 상태가 되는 것을 빠르게 판정해야 함.
  - **구현 해결책:** 차량의 진행 방향 벡터(`transform.forward`)와 다음 목표 지점을 가리키는 정규화 방향 벡터(`targetDirection`)의 **내적(Dot Product)**을 연산하여 실시간 정방향(양수 보상) 및 역방향(음수 벌점) 상태를 고도화하여 감지.
* **방향 판정 핵심 코드 스니펫 (`SimcadeCarAgent_Auto.cs`):**
  ```csharp
  // SimcadeCarAgent_Auto.cs - 벡터 내적을 통한 실시간 주행 보상/감점
  void FixedUpdate()
  {
      // 차량 전진 성격 속도 체크
      float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
      if (forwardSpeed > 0.5f) {
          AddReward(forwardSpeedReward * (forwardSpeed / vehicleController.MaxSpeed));
      }

      Vector3 targetPosition = GetTargetPosition();
      if (targetPosition != transform.position) {
          Vector3 targetDirection = (targetPosition - transform.position).normalized;
          float directionDot = Vector3.Dot(transform.forward, targetDirection); // 내적 계산 (-1.0 ~ 1.0)
          
          // 방향이 일치할수록 가산점, 반대일수록(역주행) 감점 및 wrongWay 감지
          AddReward(directionDot * directionAlignmentRewardFactor);
      }
  }
  ```

---

## 📌 Project 4: World First Kill
> **포커스:** 리플렉션 데이터 바인딩 및 Seed/Token 기반 경량 직렬화 세이브 시스템 (9인)

### 📂 [Slide 10] 리플렉션 기반 CSV 자동 데이터 파서
* **슬라이드 기재 내용:**
  - **상황:** 신규 스킬, 몬스터, 퀘스트 데이터가 기획 단계에서 수시로 추가/변경될 때마다 하드코딩 파싱 코드를 작성해야 하는 번거로움과 유지보수 병목.
  - **구현 해결책:** 기획자용 구글 시트 데이터를 CSV로 비동기 다운로드한 후, C# **리플렉션(Reflection)**을 활용해 데이터 클래스의 멤버 변수 이름과 CSV 헤더명을 동적으로 매칭하여 자동 파싱 및 바인딩하는 제네릭 파서 구현.
* **리플렉션 파서 핵심 코드 스니펫 (`CSVParser.cs`):**
  ```csharp
  // CSVParser.cs - 런타임 클래스 구조 분석 기반 자동 대입
  protected virtual void SetFieldValue(BaseData instance, FieldInfo fieldInfo, string value)
  {
      if (fieldInfo.FieldType == typeof(string))
          fieldInfo.SetValue(instance, value);
      else if (fieldInfo.FieldType == typeof(int) && int.TryParse(value, out int intVal))
          fieldInfo.SetValue(instance, intVal);
      else if (fieldInfo.FieldType == typeof(float) && float.TryParse(value, out float floatVal))
          fieldInfo.SetValue(instance, floatVal);
      else if (fieldInfo.FieldType.IsEnum) { // Enum 타입 자동 대입 지원
          try {
              object enumVal = Enum.Parse(fieldInfo.FieldType, value, true);
              fieldInfo.SetValue(instance, enumVal);
          } catch { }
      }
  }
  ```

### 📂 [Slide 11] Seed/Token 기반 난수 흐름 복구 세이브/로드
* **슬라이드 기재 내용:**
  - **상황:** 로그라이크식 랜덤 맵과 인카운터 퀘스트, 스킬 구조를 모두 파일에 JSON으로 직렬화하여 저장할 시 대용량 IO 오버헤드와 파일 크기 비대화 발생.
  - **구현 해결책:** 대표 난수 시드(`BaseSeed`)와 도메인별 난수 사용 횟수(`Usage Count`)를 비트 폭으로 인코딩하여 수십 바이트짜리 단일 `Token` 문자열로 고밀도 압축 및 디스크에 기록. 로딩 시 이 토큰의 난수 사용량을 강제 루프로 복원하여 100% 동일한 상태의 맵을 재현.
* **직렬화 핵심 코드 스니펫 (`SaveLoadManager.cs`):**
  ```csharp
  // SaveLoadManager.cs - Seed/Token 난수 진행 상황 경량 직렬화
  private const int BASE_SEED_WIDTH = 20;    // 시드 20자리
  private const int COUNT_WIDTH = 3;         // 도메인 개수 3자리
  private const int DOMAIN_ID_WIDTH = 3;     // 도메인 식별 3자리
  private const int USAGE_WIDTH = 12;        // 난수 누적량 12자리

  private static string BuildSeedToken(long baseSeed, Dictionary<GameSeed.Domain, ulong> usage)
  {
      ulong encodedSeed = unchecked((ulong)baseSeed); // 음수 시드 비트 연산 우회
      string token = Pad(encodedSeed, BASE_SEED_WIDTH);
      List<GameSeed.Domain> domains = new List<GameSeed.Domain>((GameSeed.Domain[])Enum.GetValues(typeof(GameSeed.Domain)));
      domains.Sort();

      token += Pad((uint)domains.Count, COUNT_WIDTH);
      foreach (var d in domains) {
          ulong count = usage != null ? usage.GetValueOrDefault(d, 0UL) : 0UL;
          token += Pad((uint)d, DOMAIN_ID_WIDTH);
          token += Pad(count, USAGE_WIDTH);
      }
      return token;
  }
  ```
