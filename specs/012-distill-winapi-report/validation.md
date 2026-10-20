# Validation — 2026-10-20

- 요구사항·계획 선행. 실제 C++와 기존 이미지를 확인해 본문 3개 장, 코드 details 5개, 설명 이미지 4개로 정리했다.
- 기존 8장 갤러리 HTML SHA-256 일치. 맵 파싱과 객체 생성 단계는 실제 소스에 맞춰 구분했다.
- npm run build 성공. npm test: **74 passed (27.0s)**.
- 데스크톱·Pixel 5에서 5개 코드 모두 기본 접힘. 클릭/탭, Enter, Space의 열기·닫기 통과.
- 375/768/1024/1440px 및 Pixel 5 393px에서 접힘/모두 펼침 모두 가로 넘침 없음.
- 이미지 4개 정상 로드, 원본 링크 HTTP 200. 갤러리 이전·다음·썸네일 정상. pageerror 없음.
- summary 최소 높이 55.5px/83px 및 키보드 focus outline 확인.
- Impeccable: 선택적 파서가 없어 regex fallback. 기존 DESIGN.md 지정 Plus Jakarta Sans 경고만 있으며 유지. 화면 별도 검토 완료.
- unslop-ui: findings 없음, score 0.
- 데스크톱 역할 표·처리 흐름·Pig 시트·코드, 모바일 본문·에디터 이미지를 화면 검토했다.
- 실제 인앱 브라우저에서도 첫 코드 열기·닫기 후 기본 접힘으로 유지했다.
- 증거: test-results/ds-distill/validation.json 및 desktop-overview.png, desktop-animation.png, desktop-code.png, mobile-editor.png.
- git diff --check 통과. 기존 WFK 등 사용자 수정 보존. main 유지. 게임 소스·의존성·배포 설정 변경 없음. 커밋·푸시·배포 없음.

## Follow-up: grid storage and viewport query
- 1.2에 256 크기 셀, 셀별 vector<GameObject*> 2차원 배열, 중심 좌표 등록·이동 갱신 설명을 추가했다.
- ds-grid-query에 실제 배열 선언과 셀 범위 계산·경계 제한·중복 스탬프·활성/삭제 검사·최종 AABB 조회 코드를 추가했다. 총 6개 접기.
- npm run build 성공, npm test **74 passed (24.3s)**.
- 데스크톱·Pixel 5에서 6개 모두 기본 접힘과 클릭/탭·Enter·Space 열기/닫기 통과. 375/768/1024/1440 및 393px에서 접힘/모두 펼침 모두 가로 넘침 없음.
- 코드 두 블록의 배열 선언 및 결과 추가문이 실제 표시됨을 검증했다. 이미지 4개·8장 갤러리 유지, pageerror 없음.
- 스타일 변경 없음. Impeccable의 기존 지정 폰트 경고·regex fallback 유지, unslop-ui 추가 발견 없음.
- 증거: test-results/ds-distill/validation.json, desktop-grid-explanation.png, desktop-grid-code.png, mobile-grid-code.png.
- 커밋·푸시·배포 및 게임 코드 변경 없음. git diff --check 통과.

## Follow-up: simplified query example
- QueryObjectsInRectArea 흐름을 설명용 C++ 11줄로 정리했다. 셀 순회·활성/삭제 검사·겹침·결과 추가 유지.
- GDI+ RectF::IntersectsWith를 로컬 Windows SDK 10.0.26100.0/um/gdiplustypes.h:495의 네 부등식과 대조해 기존 AABB와 같은 비교임을 확인했다. 원본 비교식을 간단히 표현한 예시임을 본문에 표시했다.
- 셀 범위 산식·클램프·중복 스탬프를 짧은 설명으로 옮겼다. 게임 소스·CSS 변경 없음.
- npm run build 성공. npm test: 74 passed (24.9s).
- 데스크톱·Pixel 5에서 6개 접기의 기본 상태, 포인터·Enter·Space 검사 통과. 접힘/모두 펼침 모두 375/768/1024/1440/393px 문서 가로 넘침 없음.
- 데스크톱·모바일 화면 검토 및 실제 인앱 브라우저에서 IntersectsWith 표시 확인. 기존 이미지·갤러리 정상.
- Impeccable 기존 폰트 경고와 parser fallback만 유지, unslop 추가 발견 없음. git diff --check 통과. 커밋·푸시·배포 없음.

## Follow-up: start/end cell calculation example
- 접기 내부 ds-cell-range에 시작 좌표 내림과 끝 좌표 올림 후 1 빼기를 짧게 설명했다.
- 설명용 카메라 영역 (300,100)~(768,512), 셀 크기 256에서 startX=1/endX=2/startY=0/endY=1, 총 4개 셀을 명시했다. 경계에서 다음 셀을 포함하지 않는 이유를 설명했다.
- 모바일 시각 검토 후 네 변수의 계산을 각 한 줄로 분리했다. 기존 조회 예시 11줄과 코드 블록 수 유지, CSS·게임 코드 변경 없음.
- npm run build 성공. npm test 74 passed (25.0s). 네 계산값·총 셀 수·11줄 코드 유지·6개 접기·포인터/키보드·이미지/갤러리·375~1440px 가로 넘침 검증 통과.
- 모바일 줄바꿈 보정 후 빌드와 계산 예시의 데스크톱/Pixel 5 표시 검증을 다시 통과했다. screenshots: desktop-cell-range.png, mobile-cell-range.png.
- Impeccable의 기존 폰트 경고와 parser fallback 유지, unslop 추가 발견 없음. 커밋·푸시·배포 없음.

## Follow-up: visible grid diagram and code-only disclosures
- 설명·계산 예시·그림을 details 밖으로 이동했다. ds-grid-storage와 ds-grid-query는 summary와 pre만 포함한다. 총 7개 접기, 조회 코드는 11줄 유지.
- SVG의 초록 셀은 (1,0),(2,0),(1,1),(2,1), 파란 점선은 카메라 (300,100)~(768,512)를 비례해 표시한다. 셀 인덱스·월드 좌표·범례·title/desc·캡션 제공.
- 첫 시각 검토에서 빌더가 줄 단위 title/desc를 문단으로 처리하는 문제를 확인해 SVG를 한 줄 HTML로 보존했다. SVG namespace, 내부 rect, 실제 크기 검증을 추가해 통과했다.
- 좌표 라벨과 셀 번호의 겹침을 제거했다. 데스크톱·모바일 표시 검증과 그림 검증을 최종 빌드 후 통과했다.
- npm run build 성공. npm test 74 passed (25.2s). 7개 접기 기본 상태·클릭/탭·Enter·Space 통과. 기존 4개 이미지/8장 갤러리 유지, pageerror 없음.
- 375/768/1024/1440 및 Pixel 5 393px에서 기본/펼침 모두 가로 넘침 없음. 설명과 그림은 모든 접기가 닫힌 상태에도 표시됨.
- evidence: test-results/ds-distill/grid-diagram-validation.json, validation.json, desktop-grid-diagram.png, mobile-grid-diagram.png.
- Impeccable 기존 폰트 경고·parser fallback만 유지, unslop 추가 발견 없음. 커밋·푸시·배포·게임 코드 변경 없음.

## Follow-up: candidate objects and final overlap filtering
- SVG에 객체 4개를 추가했다. 원·체크 2개는 카메라 내부, 사각형·X 2개는 검사 대상 셀 안의 카메라 외부에 표시했다. 결과별 범례·캡션·접근성 라벨·desc 제공.
- 객체별 월드 경계와 SVG getBBox의 좌표/크기가 정확히 대응하며, 실제 AABB 비교 결과가 render/exclude 표시와 일치함을 데스크톱과 Pixel 5에서 검증했다.
- 모든 객체 중심이 기존 검사 대상 셀 범위 X 1~2, Y 0~1에 포함됨을 확인했다. 기존 그림 좌표·7개 코드 접기·설명 공개·조회 11줄 유지.
- npm run build 성공, npm test **74 passed (29.7s)**. 7개 접기 기본 상태·클릭/탭·Enter·Space, 기존 이미지/갤러리, 375/768/1024/1440 및 393px 넘침 검사 통과. pageerror 없음.
- 데스크톱·모바일에서 객체 표시·범례·라벨 겹침 여부를 화면 검토했다. 실제 인앱 미리보기도 갱신했다.
- Impeccable 기존 폰트 경고·parser fallback 유지, unslop 추가 발견 없음. git diff --check 통과. 게임 소스 변경·커밋·푸시·배포 없음.
- 증거: test-results/ds-distill/grid-diagram-validation.json, desktop-grid-diagram.png, mobile-grid-diagram.png.

## Follow-up: restore the original interactive viewer
- HEAD 원문의 그리드 뷰어를 복원해 최근 정적 figure를 교체했다. 접근성 속성과 title/desc를 제외하면 원본 그리기/HUD 마크업이 정확히 일치함을 비교했다. 42개 객체·30개 셀·콜아웃·리셋·원래 SVG/스타일 유지.
- 4개 셀 계산 예시는 별도이며 뷰어는 6×5 예시라는 안내를 추가했다. 코드 전용 접기 7개와 설명 공개 유지. 폐기한 정적 그림 전용 CSS 제거.
- 기존 스크립트가 첫 번째 리셋 아이콘 SVG를 참조하던 문제를 viewportRect.ownerSVGElement로 수정했다. 방향키/Shift+방향키/Home 지원과 focus 표시를 추가했다.
- 초기 판정: 활성 셀 12/30, 렌더링 14, 활성 셀 내 제외 6, 비활성 셀 22. HUD 제외 28, 감소율 67% 일치.
- 실제 마우스와 CDP 터치 드래그로 위치·객체 색상·HUD가 변경됨을 확인했다. 모든 42개 객체를 각 위치의 셀/뷰포트 판정과 대조했다. 리셋/Home 초기 상태 복귀, 키보드 이동, 좌우 경계 제한 통과.
- npm run build 성공, npm test **74 passed (28.5s)**. 7개 코드 접기 기본 상태·클릭/탭·Enter·Space 및 기존 갤러리/이미지 유지, pageerror 없음.
- 데스크톱 375/768/1024/1440 및 Pixel 5 393px에서 문서 넘침 없음. 원래 모바일 뷰어의 내부 가로 스크롤 유지. 데스크톱/모바일 시각 검토 완료.
- Impeccable 기존 폰트 경고와 parser fallback만 유지, unslop 추가 발견 없음. git diff --check 통과. 커밋·푸시·배포·게임 코드 수정 없음.
- 증거: test-results/ds-distill/original-viewer.html, restored-viewer-validation.json, desktop-restored-viewer-full.png, mobile-restored-viewer-full.png.

## Follow-up: 12-cell calculation and concise Korean labels
- 별도의 4셀 예시를 제거하고 뷰어의 초기 상태와 일치하는 X 인덱스 1~4, Y 1~3, 4열×3행=12 계산을 연결했다. 표시 번호가 인덱스보다 1 큼을 명시했다.
- 초기 카메라 경계를 뷰어 격자 원점과 셀 간격으로 나눈 값 X 1.56818~4.43182/Y 1.26316~3.68421을 검산했다. 본문에는 근삿값 1.57~4.43/1.26~3.68로 표시하며 실제 월드/HUD 좌표와 혼합하지 않는다.
- 실제 활성 셀 텍스트 12개가 X1~4/Y1~3이며 그림 번호 X2~5/Y2~4와 대응함을 데스크톱과 Pixel 5에서 확인했다.
- 세 객체 설명을 각각 한글 제목+한 문장으로 줄였다. HUD/상단 제목/카메라 배지/콜아웃/동적 셀 태그도 한글로 정리하고 중복·영문 처리 단계·O(1) 표기를 제거했다. 도형·위치·동작 유지.
- npm run build 성공. npm test **74 passed (26.5s)**. 기존 마우스·실제 터치·방향키·리셋/Home·경계 제한·42개 객체 판정·HUD·넘침 검증 통과. 조회 코드는 11줄 유지.
- 데스크톱·모바일 화면 검토, 실제 인앱 미리보기 갱신 완료. Impeccable 기존 폰트 경고·parser fallback 유지, unslop 추가 발견 없음.
- 증거: test-results/ds-distill/twelve-cell-validation.json, restored-viewer-validation.json, desktop-twelve-cell-calculation.png, desktop-korean-viewer.png, mobile-korean-viewer.png.
- 커밋·푸시·배포·게임 소스 변경 없음.

## Follow-up: origins of 1.57 / 4.43 / 1.26 / 3.68
- 뷰어 내부 좌표 기준 격자 왼쪽 위 (30,75), 카메라 (260,195)~(680,425), 격자 크기880×475/6열×5행을 설명했다.
- 셀 크기880÷6≈146.67, 475÷5=95 및 (경계−원점)÷셀 크기를 네 인덱스의 변환식·반올림 규칙·결과로 연결했다. 기존 floor/ceil만 있던 네 줄은 대체해 중복을 줄였다.
- npm run build 성공, npm test **74 passed (23.5s)**. 데스크톱·Pixel 5에서 네 공식·실제 활성 셀12개·그림번호 대응·11줄 코드 유지·한글 설명을 검증했다.
- 375/768/1024/1440 및 393px 문서 가로 넘침 없음. 데스크톱·모바일 시각 검토 및 실제 인앱 미리보기 갱신 완료.
- Impeccable 기존 폰트 경고·parser fallback만 유지, unslop 추가 발견 없음. git diff --check 통과. 커밋·푸시·배포·게임 코드 변경 없음.
- 증거: test-results/ds-distill/twelve-cell-validation.json, desktop-twelve-cell-calculation.png, mobile-twelve-cell-calculation.png.

## Follow-up: concise scope and simulation instructions
- 상세한 내부 좌표·셀 크기·소수 네 값·floor/ceil 계산을 제거하고 초기4열×3행=12개 및 후보선별→화면겹침검사만 남겼다.
- 뷰어 밖에 마우스/터치 드래그, Tab+방향키, 리셋/Home 조작법과 셀강조·점선 카메라·초록/주황/회색 점·상단 수치/제외비율 의미를 추가했다. 뷰어와 하단 범례, 조회11줄·코드전용접기7개는 유지.
- npm run build 성공. npm test **74 passed (25.6s)**. 기존 실제마우스·CDP터치·방향키·리셋/Home·경계제한·42개객체판정/HUD 검사 통과.
- 데스크톱·Pixel5에서 상세수치 제거·조작법/표시의미·항상표시·초기12개·7개접기·11줄코드 유지 확인. 375/768/1024/1440 및393px 가로넘침 없음.
- 데스크톱·모바일 화면검토 및 인앱 미리보기 갱신 완료. Impeccable 기존폰트 경고/parser fallback, unslop 추가발견 없음. git diff --check 통과.
- 커밋·푸시·배포·게임코드·CSS·스크립트 수정 없음.
- 증거: test-results/ds-distill/concise-guide-validation.json, restored-viewer-validation.json, desktop-concise-viewer-guide.png, mobile-concise-viewer-guide.png.

## Follow-up: always-visible guide box and larger object legend
- 조작법과 표시 의미를 하나의 #ds-viewer-guide div로 묶었다. 기존 설명박스 CSS를 재사용하며 접기 없이 항상 표시한다. 데스크톱 패딩24px/모바일16px, 테두리1px/반경12px 확인.
- 초기 검사 범위 제목과 문단을 제거했다. 하단 세 범례 제목15.5→20px/본문14→18px로 확대했다. 글자 경계가 맞닿는 것을 확인해 본문 기준선을44→52로 내려 간격을 확보했다.
- npm run build 성공. npm test **74 passed (24.3s)**. 기존 마우스·실제터치·방향키·리셋/Home·경계제한·객체판정·HUD 검증 통과.
- 최종 글자 간격 조정 후 빌드와 데스크톱/Pixel5의 박스·삭제문구·세 범례 실제폰트·텍스트 경계 겹침 없음·오른쪽 끝 침범 없음·초기12개셀·7개접기 유지 검증 통과.
- 375/768/1024/1440 및393px 문서 가로넘침 없음. 데스크톱·모바일 시각 검토와 인앱 미리보기 갱신 완료.
- Impeccable 기존폰트경고/parser fallback 유지, unslop 추가발견 없음. git diff --check 통과. 커밋·푸시·배포·게임코드·스크립트 수정 없음.
- 증거: test-results/ds-distill/boxed-guide-validation.json, desktop-boxed-guide.png, desktop-larger-legend.png, mobile-boxed-guide.png.

## Follow-up: remove display-meaning list and enlarge viewer text
- #ds-viewer-guide의 ‘표시 의미’ 제목과 세 목록을 제거하고 항상 보이는 조작법만 남겼다.
- 뷰어 제목20px, 셀 좌표16px, 셀/카메라 태그15px, 하단 범례 제목24px/본문21px로 확대했다. 상단 수치·리셋도 키웠고 태그·콜아웃 배경과 기준선을 조정했다.
- npm run build 성공. npm test **74 passed (28.3s)**.
- 데스크톱 및 Pixel 5에서 설명 삭제·실제 폰트 크기·범례 간격·텍스트의 배경 경계 내 포함을 확인했다. 375/768/1024/1440 및393px 문서 가로 넘침 없음. 모바일 뷰어 내부 가로 스크롤 유지.
- 실제 마우스/터치 드래그·리셋·방향키/Home·42개 객체 색상과 HUD의 일치·코드7개 기본 접힘 및 클릭/Enter/Space 열고 닫기를 확인했다. 초기12셀/렌더링14/제외28 유지, 브라우저 오류 없음.
- 데스크톱/모바일 화면 검토 및 인앱 미리보기 갱신 완료. Impeccable 기존폰트경고/parser fallback 유지, unslop 추가발견 없음.
- 증거: test-results/ds-distill/boxed-guide-validation.json, viewer-final-validation.json, desktop-boxed-guide.png, desktop-larger-legend.png, mobile-larger-legend.png.
- 커밋·푸시·배포·게임 소스 변경 없음.

## Follow-up: grid storage code always visible
- ds-grid-storage의 details/summary를 일반 div와 제목으로 바꿨다. 셀 크기·배열 크기·vector 선언 내용과 앵커는 보존했다.
- npm run build 성공, npm test 74 passed (23.6s). 데스크톱/Pixel5에서 코드 상시 표시·접기 내부 아님·나머지 접기6개·문서 가로넘침 없음 확인. 미리보기 갱신 완료.
- 증거: test-results/grid-visible/desktop.png, mobile.png. 커밋·푸시 없음.

## Follow-up: .dsm section contents
- 3.2절 첫 문장에 맵 크기, 시작 위치, 타일, 객체, 이동 가능 영역을 구간별로 저장한다고 명시했다. 기존 저장 설명을 교체하여 중복을 피했다.
- npm run build 성공, npm test 74 passed (23.6s), git diff --check 통과. 인앱 미리보기의 변경 문장 확인 및 갱신 완료. 커밋·푸시 없음.
