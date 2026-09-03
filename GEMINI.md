# InfiSword.github.io Development Rules & Workflow

이 프로젝트의 모든 개발, 기능 추가, UI 수정 및 리팩토링 작업 시 아래 워크플로우와 중요 원칙을 항상 최우선으로 준수해야 합니다.

---

## 1. 표준 작업 워크플로우 (Development Workflow)

1. **Spec Kit으로 요구사항과 구현 계획을 먼저 정리**
   - 기능 개발 전 `speckit-specify`와 `speckit-plan`을 통해 요구사항을 체계화하고 구현 계획을 확립합니다.
2. **UI 작업 시 UI UX Pro Max로 디자인 방향 검토**
   - UI/UX 관련 작업일 경우, `ui-ux-pro-max` 데이터베이스를 참고하여 프로젝트 톤앤매너에 맞는 최적의 레이아웃과 구성을 검토합니다.
3. **DESIGN.md 규칙 준수**
   - 루트의 `DESIGN.md`에 명시된 토큰(Colors, Typography, Elevation)과 핵심 규칙(*The 10% Luminous Rule*, *The Developer Monospace Stamp Rule*, *The Solid Authority Rule*, *The No-Emoji-As-Icon Rule* 등)을 엄격히 준수합니다.
4. **Impeccable로 UI 품질 검토**
   - UI 설계 시 `shape`, 마감 시 `critique` / `audit` / `polish`를 활용하여 프로덕션급 마감과 웹 접근성(a11y)을 보장합니다.
5. **unslop-ui로 AI스러운 generic 디자인 패턴 검사**
   - 진부한 보라색 그라데이션, 인위적인 글로우, 이모지 아이콘 남용, 정형화된 카드 레이아웃 등 AI Slop 패턴을 철저히 배제합니다.
6. **애니메이션이 필요하면 GSAP 사용**
   - 동적 인터랙션 및 스크롤 모션이 필요한 경우 프로젝트에 내장된 GSAP과 ScrollTrigger를 활용합니다 (`prefers-reduced-motion` 배려 필수).
7. **구현 완료 후 Playwright로 실제 브라우저 검증**
   - 작업 완료 후 `npm test` 및 Playwright를 실행하여 데스크톱/모바일 환경에서 회귀 에러와 화면 렌더링을 자동으로 검증합니다.

---

## 2. 필수 준수 원칙 (Important Constraints)

- **단계별 완결성**: 각 단계가 완전히 검토 및 확정되기 전에 다음 단계로 넘어가지 않습니다.
- **계획 선제시**: 코드 수정 전 반드시 구체적인 변경 계획과 작업 의도를 사용자에게 먼저 설명합니다.
- **기존 기능 보존**: 리팩토링이나 기능 추가 시 기존에 동작하던 레이아웃, 모달, 반응형 구조를 깨뜨리지 않습니다.
- **의존성 최소화**: 불필요하거나 무거운 외부 라이브러리/패키지(dependency)를 추가하지 않습니다.
- **실제 동작 검증**: 수정 후에는 반드시 사이트 빌드(`npm run build`) 및 테스트(`npm test`)를 거쳐 오류가 없음을 확인합니다.
