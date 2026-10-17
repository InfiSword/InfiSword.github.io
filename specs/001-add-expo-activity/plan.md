# Implementation Plan: 산학연협력 EXPO 활동 추가

**Date**: 2026-10-14 | **Spec**: [spec.md](spec.md)

## Summary
기존 활동 목록과 갤러리를 확장하여 EXPO 전시 및 팀 부스 운영 경험과 사진 9장을 추가한다.

## Technical Context
- HTML/CSS/JavaScript, Node 정적 사이트 빌드. 신규 의존성 없음.
- 사진은 assets/awards/expo2026/에 원본 보존 복사. 최대 1920px 표시용 이미지와 320px 썸네일을 별도 생성, 기존 lazy loading 사용.
- 대상: GitHub Pages의 데스크톱 및 모바일 브라우저.
- 검증: npm run build, npm test(Playwright), EXPO 열람 화면 확인.

## Constitution Check
헌법은 미작성 템플릿이므로 AGENTS.md 및 DESIGN.md의 실효 지침 적용.
계획 선제시, 기존 동작 보존, 의존성 최소화, 빌드 및 브라우저 검증: 전후 검토 통과.

## Design Review
- 기존 밝은 바탕, 파란 인터랙션, Plus Jakarta Sans/Noto Sans KR 본문 및 Fira Code 날짜 재사용.
- UI UX Pro Max 사진 중심 열람·분류 탐색 지침 적용. 기존 갤러리에 EXPO 분류 추가.
- Impeccable shape 검토: 방문자는 전시와 협업 경험을 확인하고 현장 사진 버튼으로 증거를 열람. 구성이 정해진 기존 UI의 좁은 확장.
- unslop-ui 검토: 기존 디자인을 유지하며 새 장식이나 일반적인 카드 템플릿을 도입하지 않는다.

## Implementation
1. 행사 개최 정보와 사용자 경험을 근거에 맞게 작성한다.
2. 사진 9장을 고유한 이름으로 복사한다.
3. 활동 목록 최상단에 EXPO 항목, 두 안내 링크, 현장 사진 버튼을 추가한다.
4. EXPO 데이터와 분류 버튼을 추가하고 전체 및 분류 수량을 실제 데이터에서 계산한다.
5. 빌드, 전체 테스트, 데스크톱·모바일 화면 검토를 수행한다.

## Project Structure
- content/home.html: 목록 및 갤러리 원본.
- index.html: 생성 결과.
- assets/awards/expo2026/: 첨부 사진 9장.
- specs/001-add-expo-activity/: 명세 및 검증 자료.
