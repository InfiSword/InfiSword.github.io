const { test, expect } = require('@playwright/test');

test.describe('크래프톤 정글 GameTech DirectX 11 핀볼 및 회고 테스트', () => {
  test('홈 화면에 04. TECHNICAL CHALLENGE & RETROSPECTIVE 섹션 및 프로젝트 카드가 렌더링된다', async ({ page }) => {
    await page.goto('/');

    // 락스크린 해제
    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 2000 });
    }

    // 섹션 헤딩 확인
    const sectionHeading = page.locator('#tech-retrospective');
    await expect(sectionHeading).toBeAttached();
    await expect(sectionHeading).toContainText('04. TECHNICAL CHALLENGE');

    // 프로젝트 카드 확인
    const card = page.locator('[data-project="07"]');
    await expect(card).toBeVisible();
    await expect(card).toContainText('크래프톤 정글 GameTech: DirectX 11 핀볼 & 물리 시뮬레이터');

    // 카드 기간 메타 2026년 확인
    await expect(card.locator('.pf-period')).toContainText('2026');

    // 모달 버튼 문구 확인 (핵심 아키텍처 요약)
    await expect(card.locator('button.pf-btn-modal')).toContainText('핵심 아키텍처 요약');

    // 카드 미디어 비디오 프리뷰 및 포스터 확인
    const cardVideo = card.locator('.pf-card-media video');
    await expect(cardVideo).toBeVisible();
    await expect(cardVideo).toHaveAttribute('poster', /pinball_title\.png/);
    await expect(cardVideo).toHaveAttribute('src', /GameTest.*\.mp4/);
    await expect(card.locator('.pf-media-badge')).toContainText('VIDEO DEMO');

    // 홈 화면 프로젝트 카드 내에서는 회고록 박스가 제거되었는지 확인 (표준 카드 레이아웃 유지)
    await expect(card.locator('.pf-retrospective-box')).toHaveCount(0);
  });

  test('홈 화면 요약 모달(junglePinball)이 열리고 회고 요약 없이 핵심 아키텍처 4종만 정상 표시된다', async ({ page }) => {
    await page.goto('/');

    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 2000 });
    }

    // 모달 열기 버튼 클릭
    const openBtn = page.locator('[data-project="07"] button.pf-btn-modal');
    await openBtn.click();

    const modal = page.locator('#junglePinball');
    await expect(modal).toBeVisible();
    await expect(modal.locator('.pf-modal-title')).toContainText('크래프톤 정글 GameTech: DirectX 11 핀볼 & 물리 시뮬레이터');
    await expect(modal.locator('.pf-modal-tag')).toContainText('ARCHITECTURE REPORT // KRAFTON_JUNGLE_GAMETECH');
    await expect(modal.locator('.pf-modal-meta')).toContainText('2026');

    // 모달 배너 내 비디오 확인
    const modalVideo = modal.locator('.pf-modal-banner video');
    await expect(modalVideo).toBeVisible();
    await expect(modalVideo).toHaveAttribute('poster', /pinball_title\.png/);
    await expect(modalVideo).toHaveAttribute('src', /GameTest.*\.mp4/);

    // 모달 내 회고록 블록 제거 확인 (순수 핵심 아키텍처 설명만 노출)
    await expect(modal.locator('.pf-retrospective-box')).toHaveCount(0);

    // 4대 아키텍처 아이템 확인 (BoomBall과 Dear ImGui가 없고 동적 메모리 풀이 존재하는지 확인)
    const items = modal.locator('.pf-modal-item');
    await expect(items).toHaveCount(4);
    await expect(modal).not.toContainText('UBoomBall');
    await expect(modal).not.toContainText('Dear ImGui');
    await expect(modal).toContainText('벽면 반발');
    await expect(modal).toContainText('동적 메모리 풀 관리');

    // 모달 닫기
    await modal.locator('.pf-modal-close').click();
    await expect(modal).toBeHidden();
  });

  test('상세 포스트 페이지(/project/directx-pinball-game/)가 올바르게 렌더링된다', async ({ page }) => {
    await page.goto('/project/directx-pinball-game/');

    // 타이틀 및 헤더 확인
    await expect(page).toHaveTitle(/KRAFTON_JUNGLE_GAMETECH_PINBALL/);

    const heading = page.locator('h1');
    await expect(heading).toContainText('PROJECT REPORT // KRAFTON_JUNGLE_GAMETECH_PINBALL');

    // 헤더 아트 이미지 확인
    const heroImg = page.locator('.project-hero__art-img');
    await expect(heroImg).toBeVisible();
    await expect(heroImg).toHaveAttribute('src', /pinball_title\.png/);

    // 인게임 비디오 컨테이너 및 비디오 요소 확인
    const videoContainer = page.locator('.pf-video-container');
    await expect(videoContainer).toBeVisible();
    await expect(videoContainer).toContainText('IN-ENGINE REALTIME SIMULATION DEMO');
    const demoVideo = videoContainer.locator('video');
    await expect(demoVideo).toBeVisible();
    await expect(demoVideo).toHaveAttribute('poster', /pinball_title\.png/);
    const videoSource = demoVideo.locator('source');
    await expect(videoSource).toHaveAttribute('src', /GameTest.*\.mp4/);

    // 챕터 타이틀 확인 (Dear ImGui 삭제 후 총 5개)
    const chapters = page.locator('.chapter-title');
    await expect(chapters).toHaveCount(5);

    // 목차(TOC) 패널 확인 (다른 프로젝트들과 동일하게 H2 목차 항목 5개 생성)
    const toc = page.locator('.toc-panel');
    await expect(toc).toBeVisible();
    const tocH2 = toc.locator('.toc-list__item--h2');
    await expect(tocH2).toHaveCount(5);
    await expect(toc).not.toContainText('Dear ImGui');

    // 5번 면접 복기 챕터 및 패널 확인
    const retroPanels = page.locator('.pf-retrospective-panel');
    await expect(retroPanels).toHaveCount(2);
    await expect(page.locator('text=거울 보고 시선 처리 및 표정 훈련')).toBeVisible();
  });
});
