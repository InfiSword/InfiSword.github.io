# 📁 [구현 중심] 포트폴리오 슬라이드 코드 및 내용 매핑 가이드

이 문서는 포트폴리오 슬라이드를 작성할 때 **어떤 C++/C# 소스 코드를 삽입하고 어떤 핵심 내용을 기재해야 하는지**에 대한 상세 가이드라인입니다. 실제 개발 프로젝트 폴더에서 추출한 핵심 소스 코드들을 기반으로 구성되었습니다.

---

## 📌 Project 1: WinAPI Don't Starve 모작
> **포커스:** 밑바닥부터 C++로 작성한 O(1) 공간 분할 렌더러와 컬링 최적화

### 📂 [Slide 3] 2D 그리드 공간 분할 및 동적 갱신
* **슬라이드 기재 내용:**
  - **상황:** 맵에 배치되는 오브젝트가 1,000~10,000개로 늘어남에 따라 뷰포트 내 검사 후보를 찾기 위한 루프 비용 과다 발생.
  - **구현 해결책:** 맵 전체를 `GRID_CELL_SIZE = 256` 픽셀 크기의 정적 2D 배열(`m_spatialGrid`)로 분할 관리. 오브젝트 이동 시 셀 경계를 교차할 때만 그리드 정보를 갱신하도록 `UpdateObjectGrid()` 구현.
* **삽입할 핵심 코드 스니펫 (`ObjectManager.cpp`):**
  ```cpp
  // ObjectManager.cpp - 객체 이동 시 그리드 셀 갱신
  void ObjectManager::UpdateObjectGrid(GameObject* pObj)
  {
      if (!pObj || pObj->GetType() == GO_TYPE_UI) return;
      int oldX = pObj->GetGridCellX(); int oldY = pObj->GetGridCellY();

      Gdiplus::RectF bounds = pObj->GetBounds();
      int newX = (std::max)(0, (std::min)(GRID_WIDTH - 1, (int)floor((bounds.X + bounds.Width * 0.5f) / GRID_CELL_SIZE)));
      int newY = (std::max)(0, (std::min)(GRID_HEIGHT - 1, (int)floor((bounds.Y + bounds.Height * 0.5f) / GRID_CELL_SIZE)));

      if (oldX == newX && oldY == newY) return; // 속한 셀이 동일하면 얼리 리턴(최적화)

      if (oldX >= 0 && oldX < GRID_WIDTH && oldY >= 0 && oldY < GRID_HEIGHT) {
          std::vector<GameObject*>& cell = m_spatialGrid[oldX][oldY];
          cell.erase(std::remove(cell.begin(), cell.end(), pObj), cell.end());
      }
      m_spatialGrid[newX][newY].push_back(pObj);
      pObj->SetGridCell(newX, newY);
  }
  ```

### 📂 [Slide 4] 3x3 국소 쿼리 및 카메라 가시 범위 컬링
* **슬라이드 기재 내용:**
  - **상황:** 카메라 뷰포트에 닿는 특정 영역의 오브젝트만 연산해 렌더링 부하 제거 필요.
  - **구현 해결책:** 뷰포트 Rect 영역 좌표를 인덱스로 변환하여 인접 3x3 범위의 그리드만 검사 ($O(1)$). 여러 셀에 걸친 큰 오브젝트 중복 연산은 고유 스탬프(`m_spatialQueryStamp`) 비교를 통해 $O(1)$로 차단.
* **삽입할 핵심 코드 스니펫 (`ObjectManager.cpp`):**
  ```cpp
  // ObjectManager.cpp - 뷰포트 기반 공간 분할 쿼리
  void ObjectManager::QueryObjectsInRectArea(const Gdiplus::RectF& rectArea, std::vector<GameObject*>& targetOutObjects)
  {
      int startX = (std::max)(0, (int)floor(rectArea.X / GRID_CELL_SIZE));
      int endX = (std::min)(GRID_WIDTH - 1, (int)ceil((rectArea.X + rectArea.Width) / GRID_CELL_SIZE) - 1);
      int startY = (std::max)(0, (int)floor(rectArea.Y / GRID_CELL_SIZE));
      int endY = (std::min)(GRID_HEIGHT - 1, (int)ceil((rectArea.Y + rectArea.Height) / GRID_CELL_SIZE) - 1);

      if (++m_spatialQueryStamp == 0) m_spatialQueryStamp = 1; // 오버플로우 방지 스탬프 갱신

      for (int y = startY; y <= endY; ++y) {
          for (int x = startX; x <= endX; ++x) {
              for (auto* obj : m_spatialGrid[x][y]) {
                  if (obj->GetLastSpatialQueryStamp() == m_spatialQueryStamp) continue; // 중복 쿼리 스킵
                  obj->SetLastSpatialQueryStamp(m_spatialQueryStamp);

                  if (!obj->IsEnabled() || obj->IsDead()) continue;
                  if (rectArea.IntersectsWith(obj->GetBounds())) { // 최종 AABB 정밀 검사
                      targetOutObjects.push_back(obj);
                  }
              }
          }
      }
  }
  ```

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
