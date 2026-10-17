# Feature Specification: 크래프톤 포트폴리오 삭제

**Created**: 2026-10-17
**Status**: Reviewed
**Input**: 하단 크래프톤 포트폴리오 내용 삭제.

## User Scenarios & Testing
### User Story 1 (P1)
방문자는 크래프톤 정글 GameTech 기술 시험 포트폴리오가 제외된 프로젝트 목록을 본다.
**Acceptance**: 홈 하단 TECHNICAL CHALLENGE 항목, 해당 카드, 요약 모달 및 상세 포스트가 삭제된다.

### Edge Cases
- 삭제한 항목으로 연결되는 링크와 빈 섹션을 남기지 않는다.
- 기존 EXPO 활동 및 다른 프로젝트를 보존한다.

## Requirements
- FR-001: 크래프톤 정글 항목을 홈페이지와 상세 포스트에서 제거한다.
- FR-002: 정적 빌드에서 삭제한 상세 페이지가 재생성되지 않는다.
- FR-003: 해당 항목 표시를 전제한 테스트를 정리한다.

## Success Criteria
- 해당 콘텐츠와 링크가 홈페이지에 0개 존재한다.
- 빌드 성공 및 나머지 데스크톱/모바일 테스트 통과.

## Assumptions
하단 단독 섹션과 연결된 상세 포스트가 삭제 대상이다. 이미지·동영상 원본 자산은 보관한다.
