const { test, expect } = require('@playwright/test');

test.describe('WorldFirstKill 스크린샷 갤러리 테스트 (상세 포스트 & 모달)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 3000 });
    }
  });

  test('상세 포스트 페이지에서 WFK 6종 스크린샷 갤러리가 올바르게 렌더링된다', async ({ page, isMobile }) => {
    await page.goto('/project/worldfirstkill/');

    const gallerySection = page.locator('#gallery');
    await expect(gallerySection).toBeVisible();

    const postGallery = page.locator('#pfWfkGalleryPost');
    await expect(postGallery).toBeVisible();

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toBeVisible();
    await expect(mainImg).toHaveAttribute('src', /wfk_ingame_01\.png/);

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('01 / 06');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('플레이어 팀 베이스캠프 및 메인 시스템 UI');

    const thumbs = postGallery.locator('.pf-ftd-thumb-card');
    await expect(thumbs).toHaveCount(6);

    // 유튜브 시연 링크 확인
    const ytLink = postGallery.locator('.pf-ftd-yt-link');
    await expect(ytLink).toHaveAttribute('href', 'https://www.youtube.com/watch?v=EWD2YidSgp0');

    await gallerySection.scrollIntoViewIfNeeded();
    await page.waitForTimeout(300);
    const filename = isMobile ? 'wfk-post-gallery-mobile.png' : 'wfk-post-gallery-desktop.png';
    await gallerySection.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });
  });

  test('상세 포스트 페이지에서 WFK 다음/이전 및 썸네일 클릭 전환이 정상 작동한다', async ({ page }) => {
    await page.goto('/project/worldfirstkill/');
    const postGallery = page.locator('#pfWfkGalleryPost');

    const nextBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const prevBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-prev');
    const counter = postGallery.locator('.pf-ftd-counter');
    const mainImg = postGallery.locator('.pf-ftd-main-img');

    // 다음 버튼 클릭 -> 2번째 스크린샷
    await nextBtn.click();
    await expect(counter).toHaveText('02 / 06');
    await expect(mainImg).toHaveAttribute('src', /wfk_ingame_02\.png/);

    // 6번째 썸네일(시드 UI, 인덱스 5) 직접 클릭
    const thumb6 = postGallery.locator('.pf-ftd-thumb-card[data-index="5"]');
    await thumb6.click();
    await expect(counter).toHaveText('06 / 06');
    await expect(mainImg).toHaveAttribute('src', /wfk_ingame_06\.png/);
    await expect(thumb6).toHaveClass(/is-active/);

    // 이전 버튼 클릭 -> 5번째 스크린샷
    await prevBtn.click();
    await expect(counter).toHaveText('05 / 06');
    await expect(mainImg).toHaveAttribute('src', /wfk_ingame_05\.png/);
  });

  test('홈 화면 WFK 요약 모달에서도 스크린샷 갤러리가 정상 동작한다', async ({ page, isMobile }) => {
    await page.locator('button[aria-controls="wfk"]').click();
    const modal = page.locator('#wfk');
    await expect(modal).toBeVisible();

    const modalGallery = modal.locator('#pfWfkGalleryModal');
    await expect(modalGallery).toBeVisible();

    const nextBtn = modalGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const counter = modalGallery.locator('.pf-ftd-counter');

    await nextBtn.click();
    await expect(counter).toHaveText('02 / 06');

    // 모달 내 "상세 포스트 보기 ↗" 링크 확인
    const detailLink = modalGallery.locator('a[href="/project/worldfirstkill/#gallery"]');
    await expect(detailLink).toBeVisible();

    await modal.evaluate(el => el.scrollTop = 0);
    await page.waitForTimeout(400);
    const filename = isMobile ? 'wfk-modal-mobile.png' : 'wfk-modal-desktop.png';
    await page.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });

    await page.keyboard.press('Escape');
    await expect(modal).toBeHidden();
  });
});
