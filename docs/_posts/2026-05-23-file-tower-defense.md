layout: single

title: "PROJECT REPORT // FILE\_TOWER\_DEFENSE"

excerpt: "Systems Engineering: 아키텍처 재설계 및 공간 분할 최적화"

categories: \[Project]

tags: \[Unity, Optimization, Architecture, Algorithms]

toc: true

toc\_sticky: true



유니티의 좌표계 특성을 분석하여 아키텍처를 전면 재설계하고, 성능 최적화와 확장성 있는 시스템을 구축한 프로젝트입니다.



1\. UI(RectTransform)에서 GameObject(Transform) 기반으로의 전환



초기에는 윈도우 바탕화면 느낌을 위해 UI 시스템으로 구축했으나, 다음과 같은 설계적 한계를 마주했습니다.



좌표계 종속성: Anchor/Pivot에 따른 월드 좌표 상대적 변화



연산 비용 증가: WorldToScreenPoint 등 매 프레임 좌표 변환 오버헤드



계층 구조 복잡성: 서로 다른 부모 객체 간 위치 변환의 가독성 저하



이를 해결하기 위해 모든 유닛을 GameObject 기반 월드 좌표계로 통일하여 성능을 최적화하고 물리 시스템(Physics2D)과 직접 연동되게 구성했습니다.



2\. FileGrid: 데이터 중심의 개별 셀 매니저



윈도우 바탕화면 아이콘 정렬 시스템을 모방하여 유닛 배치 시스템을 구축했습니다.



public class FileGrid : MonoBehaviour

{

&#x20;   \[Header("State")]

&#x20;   private File\_Base fileUnit; // 현재 그리드를 점유 중인 파일 유닛

&#x20;   public int GridX { get; private set; }

&#x20;   public int GridY { get; private set; }



&#x20;   // HashSet을 활용한 버프 소스 관리 (O(1) 조회)

&#x20;   private readonly HashSet<File\_Base> activeBuffSources = new HashSet<File\_Base>(); 

}





💡 이벤트 기반 자동 버프 시스템 (Observer Pattern)

유닛 배치 시 이미 그리드에 활성화된 버프 소스를 즉각 적용받도록 설계되었으며, HashSet을 사용해 중복을 O(1)의 성능으로 방지합니다.

{: .notice--success }



3\. 공간 분할 및 트랜잭션 최적화



3.1 3x3 국소 영역 탐색 (Spatial Partitioning)



전체 그리드(91개)를 순회하는 대신, 중심 그리드 기준 3x3 영역만 검사하여 탐색 비용을 O(n)에서 O(1)로 단축했습니다. (약 10배 이상 성능 향상)



3.2 원자적 배치 로직 (Transactional Pattern)



유닛 배치 시 데이터 불일치를 방지하고 실패 시 이전 상태를 유지하는 트랜잭션을 구현했습니다.



public bool TrySetUnitAtGrid(File\_Base unit, FileGrid targetGrid)

{

&#x20;   // 1. 유효성 검사 (Pre-validation)

&#x20;   if (unit == null || targetGrid == null || targetGrid.obstacleObject != null) return false;



&#x20;   // 2. 기존 그리드에서 안전하게 제거 (Rollback 준비)

&#x20;   FileGrid currentGrid = unit.CurrentGrid;

&#x20;   if (currentGrid != null \&\& currentGrid != targetGrid) {

&#x20;       currentGrid.RemoveFileUnit(); 

&#x20;   }



&#x20;   // 3. Commit: 물리/논리 계층 구조 설정

&#x20;   unit.transform.position = targetGrid.transform.position;

&#x20;   unit.transform.SetParent(targetGrid.transform);

&#x20;   

&#x20;   // 4. Notify: 새 그리드 등록 및 버프 자동 적용

&#x20;   targetGrid.SetFileUnit(unit); 

&#x20;   return true;

}





4\. 중앙 집중식 입력 및 상호작용 시스템



수많은 유닛이 개별 이벤트를 수신하지 않고, InputManager가 입력을 통합 관리하여 IInteractable 객체에 전달합니다.



레이어 우선순위 처리: UI와 게임오브젝트 충돌 시 SortingLayer/Order 기반 정교한 판정 수행



Mediator 패턴: InteractionHandler를 통한 객체 간 직접 의존성 제거



상태 머신 기반 드래그: InputState에 따른 드래그, 더블클릭, 박스 다중 선택 제어

