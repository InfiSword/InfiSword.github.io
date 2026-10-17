# Implementation Plan: 크래프톤 포트폴리오 삭제

**Date**: 2026-10-17 | **Spec**: [spec.md](spec.md)

## Summary & Technical Context
기존 HTML/JS 정적 사이트에서 요청된 콘텐츠만 제거한다. 신규 디자인과 의존성은 없다.
AGENTS.md 및 DESIGN.md의 기존 기능 보존·빌드·브라우저 검증 지침을 적용한다.

## Design Review
기존 UI와 타이포그래피 유지. UI UX Pro Max·Impeccable·unslop-ui의 일관성 및 빈 섹션 방지 관점에서 검토.
삭제 후 기존 Other Projects 목록으로 포트폴리오가 자연스럽게 끝나도록 한다.

## Implementation
1. 홈의 TECHNICAL CHALLENGE 섹션과 크래프톤 카드, 연결 모달 삭제.
2. 상세 포스트 원본과 생성 HTML 및 빌드 이미지 매핑 삭제.
3. 삭제한 콘텐츠에 종속된 테스트 제거.
4. npm run build, npm test, 데스크톱/모바일 하단 렌더링 확인.

## Scope
content/home.html, content/projects/directx-pinball-game.md,
project/directx-pinball-game/index.html, scripts/build-static-site.js,
tests/jungle-pinball.spec.js 및 빌드된 index.html.
