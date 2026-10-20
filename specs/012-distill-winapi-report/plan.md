# Implementation Plan: WinAPI Don't Starve 핵심 흐름 정리

**Branch**: `main` | **Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
기존 5개 장의 장문·중복 도식을 세 장으로 통합한다. 객체 구성과 가시 객체 렌더링, 애니메이션 이벤트, 에디터 데이터 로드를 설명하고 실제 C++의 핵심 5개를 기본 접힘으로 제공한다.

## Technical Context
- Language: Markdown/HTML, CSS, C++ 설명 코드. 기존 Node 정적 빌더 사용.
- Dependencies: 기존 marked/highlight.js 및 Playwright만 사용, 추가 없음.
- Storage: content/projects/winapi-dont-starve.md → project/winapi-dont-starve/index.html.
- Testing: npm run build, npm test(74개), Chromium/Pixel 5 일괄 검증.
- Target: 데스크톱 및 모바일 웹.
- Performance: 불필요한 새 스크립트·애니메이션 없음, 기존 이미지 lazy loading.
- Constraints: 커밋·푸시·배포 금지, 게임 소스 읽기 전용, 다른 페이지 수정 보존.
- Scope: 보고서 한 페이지와 공유 CSS의 DS 전용 selector 추가.

## Constitution Check
헌법 문서는 미작성 템플릿이며 추가 규칙이 없다. AGENTS.md와 DESIGN.md 적용. 요구사항·계획 선행, 기존 갤러리 보존, 종속성 추가 없음. 설계 전·후 모두 통과.

## Project Structure
- content/projects/winapi-dont-starve.md: 원본 글과 갤러리
- assets/css/site.css: 기존 패턴에 ds 클래스 연결
- project/winapi-dont-starve/index.html: 빌드 산출물
- specs/012-distill-winapi-report/: spec, research, data-model, contracts, quickstart, validation
- test-results/ds-distill/: 임시 브라우저 검증 기록

## Implementation sequence
1. 원본 갤러리를 그대로 보관한다. 본문은 세 장과 간단한 클래스 역할 표·순서 목록으로 작성한다.
2. AddComponent, 가시 객체 조회/정렬, 프레임 이벤트, 오브젝트 설정, 맵 객체 생성의 5개 details를 구성한다.
3. 기존 Pig 스프라이트 및 오브젝트/맵 에디터/인게임 결과 이미지를 재사용한다. 캡션·원본 링크·크기를 제공한다.
4. 기존 WFK 흐름·코드·이미지 CSS에 ds 클래스만 추가한다. Pig 시트는 비율을 유지하고 높이를 제한한다.
5. 빌드·전체 테스트·포인터/키보드/모바일·이미지·가로 넘침을 한 번의 일괄 검사로 확인한다. 실제 화면을 검토하고 필요한 결함만 수정한다.

## Complexity Tracking
추가 추상화, 새 의존성, 사용자 승인 대기 없음. 해결되지 않은 조사 항목 없음.

## Follow-up plan: grid query
1. ObjectManager.h/.cpp의 배열 선언, 중심 셀 등록, QueryObjectsInRectArea를 읽기 전용으로 확인했다.
2. 1.2 도입을 자료구조와 조회 흐름 두 문단으로 수정하고 ds-grid-query details를 추가한다. 선언/셀 범위와 최종 겹침 검사를 구분한다. 기존 CSS 사용.
3. 빌드·전체 회귀 테스트 후 데스크톱/Pixel 5에서 6개 접기의 클릭/탭·Enter·Space, 가로 넘침, 새 코드의 표시와 화면을 검증한다.

## Follow-up plan: simplify query code
1. 셀 좌표 산식·경계 제한·스탬프를 코드 밖 짧은 설명으로 옮긴다.
2. 원본의 직접 AABB 비교를 Windows SDK RectF::IntersectsWith와 대조한 뒤 동일 흐름을 짧은 예시로 표시한다.
3. 기존 접기·자료구조는 유지하고 빌드·전체 테스트 및 데스크톱/모바일 코드 표시를 검증한다. 커밋 없음.

## Follow-up plan: cell range example
1. 기존 그리드 코드 앞에 ds-cell-range 설명을 추가한다. 시작=floor(좌표/256), 끝=ceil(좌표/256)-1을 두 축에 적용해 보여준다.
2. 경계에 닿는 다음 셀을 포함하지 않기 위한 -1을 한 문장으로 설명하고 코드 블록·CSS는 유지한다.
3. 빌드·전체 회귀 테스트 및 데스크톱/Pixel 5의 내용·접기·넘침·화면 검증을 수행한다.

## Follow-up plan: grid diagram and code-only disclosures
1. 기존 그리드 접기를 자료구조와 조회 코드의 두 접기로 나누고 모든 관련 설명은 밖으로 이동한다. 중복 설명은 줄인다.
2. 4열×3행 그리드에서 (1,0),(2,0),(1,1),(2,1)을 초록 셀로, 카메라는 파란 점선으로 표시한다. 월드 좌표와 예시 수치가 정확하게 대응하도록 SVG를 작성한다.
3. 색 외에도 채움·점선·텍스트로 구별한다. title/desc와 HTML 범례/캡션을 제공하고 모바일에서 비율을 유지한다. 기존 코드 11줄·원본 이미지·갤러리 유지.
4. 빌드·전체 회귀 및 데스크톱/모바일에서 항상 보임·code-only details·7개 접기 조작·그림·가로 넘침을 검증한다.

## Follow-up plan: objects inside candidate cells
1. 카메라 안의 객체 2개, 위쪽 셀 여백의 카메라 밖 객체 2개를 월드 좌표로 배치한다. 모든 객체는 검사 셀 (1,0),(2,0),(1,1),(2,1) 안에 속한다.
2. 기존 SVG에 결과별 심볼과 범례를 추가하고 겹침 검사로 제외되는 이유를 캡션에 쓴다. SVG 한 줄 형식 유지.
3. 객체 경계의 실제 AABB 판정과 그림의 배치·결과 라벨을 자동 대조하고 빌드·회귀 테스트·모바일/데스크톱 화면 검증을 수행한다.

## Follow-up plan: restore original interactive viewer
1. HEAD 원문의 pf-visual-frame pf-diagram-frame 블록을 추출해 정적 figure를 교체한다. 원래 스타일·42개 객체·콜아웃·HUD·리셋을 재사용한다. ds-grid-diagram 앵커 유지.
2. 기존 initSpatialSimulation이 리셋 아이콘 대신 뷰포트의 ownerSVGElement를 사용하도록 바로잡아 드래그 좌표를 정상 변환한다. 뷰포트에 키보드 이동과 Home 초기화를 추가하고 원래 포인터 동작을 보존한다.
3. 폐기한 정적 그림 전용 CSS를 제거한다. 모바일 가로 스크롤은 기존 뷰어 안에서만 작동하도록 보존한다. 새 의존성 없음.
4. 원문 블록과 복원 내용을 비교하고 마우스·터치·키보드 이동, HUD/셀/객체 색상, 리셋, 경계 제한, 회귀 및 모바일 넘침을 검증한다.

## Follow-up plan: align calculation and simplify Korean copy
1. 뷰어 원점 (30,75), 셀 간격 (880/6,95), 초기 카메라 (260,195)~(680,425)로 정규화 경계를 검산한다.
2. 실제 월드 256 좌표와 혼합하지 않고 뷰어 셀 단위 설명·네 인덱스·총12개를 제시한다. 반복된 독립 예시 문장을 제거한다.
3. SVG 하단 세 설명과 작은 콜아웃·HUD·동적 셀 태그를 간단한 한글로 통일한다. 기존 도형·위치·상호작용 보존.
4. 빌드·전체 회귀·기존 마우스/터치/키보드 뷰어 검증·계산과 12개 활성 셀·한글 문구·화면·넘침을 검증한다.

## Follow-up plan: explain normalized boundaries
1. 뷰어 실제 좌표와 셀 가로880/6≈146.67, 세로475/5=95를 일반 문장으로 소개한다. HUD 수치와 혼동하지 않도록 내부 그림 좌표임을 명시한다.
2. 기존 floor/ceil 단독 예시를 네 경계의 변환식→반올림 규칙→인덱스로 교체해 중복을 줄인다. 구조·뷰어·코드·CSS는 유지한다.
3. 빌드·회귀·기존12개셀 검증과 파생 수치·모바일 줄바꿈을 한 번에 확인한다.

## Follow-up plan: concise simulation guide
1. ds-cell-range 앞부분의 상세 수치와 네 공식 목록을 짧은 초기검사범위 설명으로 대체한다.
2. 뷰어 조작법 한 문단과 표시 의미 세 항목을 추가한다. 원래 그림과 하단 범례·스크립트·11줄 코드는 그대로 둔다.
3. 빌드·회귀와 데스크톱/모바일에서 내용/조작법/초기12개셀/조회코드/실제마우스·터치·키보드·리셋/넘침을 확인한다.

## Follow-up plan: boxed guide and larger legend
1. 초기검사범위 제목/문단을 삭제하고 기존 조작법·표시의미를 ds-viewer-guide div로 함께 감싼다. details 사용 없음.
2. 기존 문서 설명박스 CSS selector에 ds-viewer-guide만 추가해 같은 border/background/spacing을 재사용한다.
3. 하단3개 SVG 범례 제목15.5→20, 본문14→18로 확대한다. 위치·뷰어 이벤트 유지.
4. 빌드·전체회귀·마우스/터치/키보드 뷰어·박스상시표시·제거문구·범례글자·텍스트겹침/넘침을 데스크톱과 모바일에서 검증한다.

## Follow-up plan: larger viewer typography
1. 표시 의미 블록만 제거한다. 상시 안내박스와 조작법 유지.
2. HUD 라벨/값을 .9rem/1rem으로, 셀번호16px, 내부 태그14~15px, 콜아웃14/12.5px, 하단범례24/21px로 확대한다. 태그/배지 상자와 본문 기준선은 글자크기에 맞춘다.
3. 기존 객체/뷰포트 판정은 유지한다. 동적 태그와 배지의 위치 오프셋만 동기화한다.
4. 빌드/회귀·뷰어 실제마우스/터치/키보드·숫자 및 확대 글자 치수·라벨상자 포함·모바일 넘침을 검증하고 화면을 검토한다.
# 후속 계획: 그리드 자료구조 상시 표시
- 해당 details/summary를 일반 div와 제목으로 교체하고 앵커·코드를 보존한다. 빌드 및 기존 테스트 후 데스크톱/모바일에서 표시 상태를 확인한다.

## 후속 계획: 맵 저장 항목 명시
- 3.2절 첫 문장을 저장 항목 다섯 개를 포함하도록 교체해 중복을 피한다. 빌드와 기존 테스트, 브라우저 표시를 확인한다.
