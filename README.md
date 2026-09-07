# InfiSword.github.io

Minhyuk Lee (InfiSword)의 게임 클라이언트 & 그래픽스 엔지니어링 정적 포트폴리오 웹사이트입니다.  
GitHub Pages 기반으로 호스팅되며, AI 에이전트 워크플로우, 정밀한 디자인 시스템, 브라우저 자동화 검증 환경을 갖추고 있습니다.

---

## 📑 핵심 기술 및 아키텍처 목차 (Table of Contents)

1. [Spec Kit](#1-spec-kit)
2. [unslop-ui](#2-unslop-ui)
3. [UI UX Pro Max](#3-ui-ux-pro-max)
4. [Impeccable](#4-impeccable)
5. [Impeccable hook](#5-impeccable-hook)
6. [Playwright MCP](#6-playwright-mcp)
7. [DESIGN.md](#7-designmd)
8. [GSAP](#8-gsap)
9. [프로젝트 빌드 및 테스트 가이드](#9-프로젝트-빌드-및-테스트-가이드)

---

## 1. Spec Kit
- **위치**: [`.agents/skills/speckit-*`](.agents/skills/) (10종 도구 내장)
- **개념 및 역할**: 사양 및 엔지니어링 라이프사이클 관리 스위트입니다. 요구사항 정의부터 아키텍처 계획, 작업 분해, 구현까지 체계적인 엔지니어링 프로세스를 강제합니다.
- **주요 구성 스킬**:
  - `speckit-specify`: 자연어 요구사항을 표준 기능 사양서(`spec.md`)로 구체화
  - `speckit-plan`: 시스템 설계, 파일 변경 범위 및 단계별 접근법을 담은 구현 계획서(`plan.md`) 수립
  - `speckit-tasks`: 사양과 계획을 바탕으로 의존성과 순서가 명시된 실행 태스크 목록(`tasks.md`) 도출
  - `speckit-implement`: `tasks.md`의 작업을 순차적으로 실행하고 단위 검증 수행
  - `speckit-analyze` / `speckit-clarify` / `speckit-checklist` / `speckit-constitution` / `speckit-converge` / `speckit-taskstoissues`: 사양 교차 일관성 검증, 모호성 해소 질의, 프로젝트 원칙 수립, 미구현 작업 추적

---

## 2. unslop-ui
- **위치**: [`.agents/skills/unslop-ui`](.agents/skills/unslop-ui)
- **개념 및 역할**: AI가 생성하는 특유의 진부하고 획일화된 UI 디자인 패턴(AI Slop)을 감지하고 배제하는 디자인 필터링 스킬입니다.
- **적용 내용**:
  - 인위적인 보라색/네온 그라데이션 및 과도한 글로우 효과 제거
  - 이모지를 아이콘으로 사용하는 비전문적 패턴 배제
  - 무의미하고 정형화된 3단 카드 나열 대신, 엔지니어링 프로젝트의 특성을 드러내는 레이아웃 적용
  - 사람 손으로 다듬은 듯한 독창적이고 명확한 엔지니어링 포트폴리오 감성 보장

---

## 3. UI UX Pro Max
- **위치**: [`.agents/skills/ui-ux-pro-max`](.agents/skills/ui-ux-pro-max)
- **개념 및 역할**: 50개 디자인 스타일, 21개 색상 팔레트, 50개 폰트 페어링 지식베이스를 제공하는 UI/UX 인텔리전스 스킬입니다.
- **적용 내용**:
  - 본 포트폴리오의 디자인 테마인 **"Swiss Technical Light Architecture"**의 방향성 수립
  - 기하학적 현대 산세리프(`Plus Jakarta Sans`)와 개발자 모노스페이스(`Fira Code`)의 타이포그래피 페어링 확립
  - 고대비(WCAG AAA) 기반의 시각적 계층 구조 및 색상 토큰 정의

---

## 4. Impeccable
- **위치**: [`.agents/skills/impeccable`](.agents/skills/impeccable)
- **개념 및 역할**: 프론트엔드 인터페이스의 완성도를 프로덕션 수준으로 끌어올리는 정밀 UI 폴리싱 및 접근성(a11y) 검증 스킬입니다.
- **적용 내용**:
  - 시각적 위계(Visual Hierarchy) 및 정보 구조 최적화
  - 텍스트와 배경 간 명도 대비 16.5:1 (제목) / 7.5:1 이상 (본문) 준수
  - 마이크로 인터랙션, 반응형 여백, 컴포넌트 정렬의 픽셀 단위 마감 정밀화

---

## 5. Impeccable hook
- **위치**: [`.agents/skills/impeccable/scripts/hook-admin.mjs`](.agents/skills/impeccable/scripts/hook-admin.mjs) (`state: enabled`)
- **개념 및 역할**: UI 파일 수정 시 디자인 결함을 실시간으로 감지하여 에이전트에게 즉시 피드백을 전달하는 자동화 백그라운드 훅 시스템입니다.
- **동작 방식**:
  - `.html`, `.css`, `.js` 등의 UI 파일을 생성하거나 수정할 때마다 즉시 트리거
  - 깨진 이미지 링크, 요소 오버플로우, 명도 대비 미달, 그라데이션 텍스트, 인위적 글로우, 디자인 시스템 드리프트(토큰 이탈)를 기계적으로 감지하여 미연에 방지

---

## 6. Playwright MCP
- **위치**: [`.agents/plugins/playwright/mcp_config.json`](.agents/plugins/playwright/mcp_config.json)
- **개념 및 역할**: AI 에이전트가 실제 Chromium 브라우저를 직접 구동하여 웹 화면을 조작하고 검증할 수 있도록 지원하는 Model Context Protocol 서버입니다.
- **활용 내용**:
  - 헤드리스 및 GUI 브라우저를 통한 실제 렌더링 무결성 검증
  - 데스크톱(1280px+), 태블릿, 모바일(375px) 등 다양한 뷰포트에서의 반응형 레이아웃 점검
  - 프로젝트 카드 클릭, 모달 열기/닫기, 갤러리 슬라이더 전환 등 사용자 인터랙션 테스트 자동화
  - 화면 스크린샷 캡처를 통한 시각적 회귀 분석

---

## 7. DESIGN.md
- **위치**: [`DESIGN.md`](DESIGN.md)
- **개념 및 역할**: 본 프로젝트 고유의 디자인 언어인 **"Swiss Technical Light Architecture"**를 규정하는 디자인 시스템 헌장입니다.
- **4대 핵심 원칙**:
  1. **The 10% Luminous Rule**: 기본 캔버스는 순수/소프트 화이트 및 슬레이트 톤을 유지하며, 로열 블루(`#2563eb`) 액센트는 인터랙션 요소 및 상태 표시에만 10% 미만으로 절제하여 사용
  2. **The Developer Monospace Stamp Rule**: 날짜, 메트릭, 태그, 아키텍처 라벨 등 모든 기술적 메타데이터는 `Fira Code` 고정폭 폰트와 헤어라인 보더 뱃지를 적용
  3. **The Solid Authority Rule**: 가벼운 블러나 네온 효과를 지양하고, 1px 헤어라인 테두리(`border: 1px solid #e2e8f0`)와 정돈된 솔리드 엘레베이션으로 단단한 구조감 형성
  4. **The No-Emoji-As-Icon Rule**: 이모지를 UI 아이콘으로 사용하지 않고, 정밀한 인라인 SVG 또는 텍스트 라벨 사용

---

## 8. GSAP
- **위치**: [`package.json`](package.json) (`gsap@^3.15.0`)
- **개념 및 역할**: 웹사이트 전반의 마이크로 모션과 스크롤 인터랙션을 책임지는 고성능 하드웨어 가속 애니메이션 라이브러리입니다.
- **적용 내용**:
  - Hero 섹션 타이포그래피 등장 트랜지션 (`power3.out`)
  - ScrollTrigger 기반의 프로젝트 카드 및 기술 섹션 부드러운 리빌 연출 (`power2.out`)
  - `prefers-reduced-motion` 미디어 쿼리를 감지하여 모션 감소 설정 시 애니메이션을 즉시 비활성화하는 접근성 배려

---

## 9. 프로젝트 빌드 및 테스트 가이드

### 정적 사이트 빌드
```bash
npm run build
```
`content/home.html` 템플릿과 `content/projects/*.md` 마크다운 문서를 결합하여 정적 배포용 HTML 페이지를 생성합니다 (`scripts/build-static-site.js`).

### 로컬 웹서버 실행
```bash
npm run serve
```
로컬 정적 웹 서버를 띄워 브라우저에서 포트폴리오 사이트를 실시간 미리보기합니다.

### Playwright 브라우저 E2E 테스트 (9종 스위트)
```bash
# 전체 테스트 실행 (74개 테스트 전수 검증)
npm test

# 대화형 Playwright UI 모드로 실행
npm run test:ui

# 마지막 테스트 리포트 브라우저 확인
npm run test:report
```
