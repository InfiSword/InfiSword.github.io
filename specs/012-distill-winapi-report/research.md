# Research

- 기존 글은 1,600줄 이상이며 아키텍처/렌더링/최적화/애니메이션/에디터 설명이 분산되어 있다. 세 장으로 통합하고 8장 갤러리 HTML은 그대로 유지한다.
- 실제 소스: C:/MyProject/DontStarve_WinAPI/DontStarve_WinApi/DontStarve_Client (읽기 전용).
- GameObject.h는 unique_ptr<Component>를 소유하고 AddComponent로 기능을 조합한다.
- CameraManager.cpp는 QueryObjectsInRectArea로 후보를 조회한다. ObjectManager.cpp는 그리드 범위→AABB 검사를 수행한다. RenderManager.cpp는 레이어별 stable_sort 후 DrawCommand를 실행한다.
- Player.cpp는 CHOP의 프레임 4와 35에 이벤트를 등록한다. Animator.cpp는 지나친 프레임의 이벤트를 검사하고 콜백 중 클립이 바뀌면 중단한다.
- DataManager.cpp는 기본 리소스 등록 후 ID별 피벗/콜라이더 설정을 덮어쓴다. Header/Function.h의 현재 맵 파서는 ID·좌표를 보관하며 GameScene.cpp에서 CreateObject를 호출한다. 과거 글의 파싱 시 리소스 결합 설명은 현재 구현과 맞지 않아 수정한다.
- UI UX Pro Max 접근성 권장과 DESIGN.md의 기존 색·폰트를 따른다. Impeccable distill/polish/craft-floor 및 unslop-ui 적용. 기존 글의 큰 장식 도식 대신 짧은 처리 순서·실제 이미지를 사용한다.
- 기존 Optimization PNG의 수치·O(1) 표기는 측정 결과로 오해될 수 있어 채택하지 않는다. 기존 Pig idle 시트와 실제 에디터 화면을 사용한다.
