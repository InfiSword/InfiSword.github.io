# Implementation Plan: 다이어그램 흰 글자

**Date**: 2026-10-19 | **Spec**: [spec.md](spec.md)

## Technical Context
HTML/CSS 정적 포트폴리오. 공통 assets/css/site.css와 프로젝트 Markdown 소스를 수정한다. 새 의존성 없음.

## Constitution Check & Design Review
AGENTS.md/ DESIGN.md를 따른다. 밝은 사이트 안의 도표 패널만 기존 어두운 패널 색(#0d1222 계열)으로 통일하고 사용자가 요청한 흰색 텍스트를 사용한다.
Spec Kit으로 범위와 계획 확정. UI UX Pro Max 대비 권고(4.5:1) 참고, Impeccable/unslop-ui로 한정된 기존 UI 개선 검토.
새 레이아웃이나 모션 없음. 코드 구문 강조 및 SVG 선/도형을 보존한다.

## Implementation
1. 일반 도표 텍스트는 흰색으로 유지하고, 기존 단계·역할·분기 및 SVG 강조색을 되살린다.
2. 공통 도표 카드·분기·배지 배경을 어둡게 정리.
3. 도표 내부 인라인 밝은 배경에 의미 있는 공통 클래스를 부여해 동일한 대비 보장.
4. 빌드, 기존 Playwright 전체 테스트 및 6개 프로젝트의 텍스트/대비 검사.
5. 데스크톱/모바일 밝은/어두운 테마의 대표 도표 화면을 일괄 확인.

## Follow-up: 강조색 보존
사용자 피드백에 따라 전체 흰색 강제를 수정한다. 기존 CSS/원문 인라인 색을 비교하여 입력·역할(파랑), 성공·정상 분기(초록), 차단·오류(빨강), 상태(보라), 조건 판정(노랑)을 보존한다. 인라인 강조는 pf-diagram-accent-* 클래스로 명시하고 일반 회색 설명은 흰색으로 유지한다.

## Scope
assets/css/site.css, content/projects/*.md, 빌드된 project/*/index.html.
