const { test, expect } = require('@playwright/test');

test.describe('Autonomous Racing Agent 스크린샷 갤러리 테스트 (상세 포스트 & 모달)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 3000 });
    }
  });

  test('상세 포스트 페이지에서 Racing Agent 3종 스크린샷 갤러리가 올바르게 렌더링된다', async ({ page, isMobile }) => {
    await page.goto('/project/autonomous-racing-agent/');

    const gallerySection = page.locator('#gallery');
    await expect(gallerySection).toBeVisible();

    const postGallery = page.locator('#pfRacingGalleryPost');
    await expect(postGallery).toBeVisible();

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toBeVisible();
    await expect(mainImg).toHaveAttribute('src', /racing_ingame_01\.png/);

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('01 / 03');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('산악 서킷 고속 코너링 및 실시간 순위 리더보드 UI');

    const thumbs = postGallery.locator('.pf-ftd-thumb-card');
    await expect(thumbs).toHaveCount(3);

    // 유튜브 시연 링크 확인
    const ytLink = postGallery.locator('.pf-ftd-yt-link');
    await expect(ytLink).toHaveAttribute('href', 'https://www.youtube.com/watch?v=OGb1UzntLnI');

    await gallerySection.scrollIntoViewIfNeeded();
    await page.waitForTimeout(300);
    const filename = isMobile ? 'racing-post-gallery-mobile.png' : 'racing-post-gallery-desktop.png';
    await gallerySection.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });
  });

  test('상세 포스트 페이지에서 Racing Agent 다음/이전 및 썸네일 클릭 전환이 정상 작동한다', async ({ page }) => {
    await page.goto('/project/autonomous-racing-agent/');
    const postGallery = page.locator('#pfRacingGalleryPost');

    const nextBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const prevBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-prev');
    const counter = postGallery.locator('.pf-ftd-counter');
    const mainImg = postGallery.locator('.pf-ftd-main-img');

    // 다음 버튼 클릭 -> 2번째 스크린샷
    await nextBtn.click();
    await expect(counter).toHaveText('02 / 03');
    await expect(mainImg).toHaveAttribute('src', /racing_ingame_02\.png/);

    // 3번째 썸네일(센서 레이캐스트, 인덱스 2) 직접 클릭
    const thumb3 = postGallery.locator('.pf-ftd-thumb-card[data-index="2"]');
    await thumb3.click();
    await expect(counter).toHaveText('03 / 03');
    await expect(mainImg).toHaveAttribute('src', /racing_ingame_03\.png/);
    await expect(thumb3).toHaveClass(/is-active/);

    // 이전 버튼 클릭 -> 2번째 스크린샷
    await prevBtn.click();
    await expect(counter).toHaveText('02 / 03');
    await expect(mainImg).toHaveAttribute('src', /racing_ingame_02\.png/);
  });

  test('홈 화면 Racing Agent 요약 모달에서도 스크린샷 갤러리가 정상 동작한다', async ({ page, isMobile }) => {
    await page.locator('button[aria-controls="racing"]').click();
    const modal = page.locator('#racing');
    await expect(modal).toBeVisible();

    const modalGallery = modal.locator('#pfRacingGalleryModal');
    await expect(modalGallery).toBeVisible();

    const nextBtn = modalGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const counter = modalGallery.locator('.pf-ftd-counter');

    await nextBtn.click();
    await expect(counter).toHaveText('02 / 03');

    // 모달 내 "상세 포스트 보기 ↗" 링크 확인
    const detailLink = modalGallery.locator('a[href="/project/autonomous-racing-agent/#gallery"]');
    await expect(detailLink).toBeVisible();

    await modal.evaluate(el => el.scrollTop = 0);
    await page.waitForTimeout(400);
    const filename = isMobile ? 'racing-modal-mobile.png' : 'racing-modal-desktop.png';
    await page.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });

    await page.keyboard.press('Escape');
    await expect(modal).toBeHidden();
  });
});
