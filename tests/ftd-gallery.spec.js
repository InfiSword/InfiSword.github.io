const { test, expect } = require('@playwright/test');

test.describe('File Tower Defense 스크린샷 갤러리 테스트 (카드 & 상세 포스트)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 3000 });
    }
  });

  test('홈 카드 오른쪽 미디어가 타이틀 이미지를 표시하고 유튜브 링크를 가진다', async ({ page }) => {
    const ftdCardMedia = page.locator('article[data-project="01"] .pf-card-media');
    await expect(ftdCardMedia).toBeVisible();

    // 유튜브 링크 속성 확인
    await expect(ftdCardMedia).toHaveAttribute('href', 'https://www.youtube.com/watch?v=LNJKQiQ8ahs');
    await expect(ftdCardMedia).toHaveAttribute('target', '_blank');

    // 새로 적용된 타이틀 이미지 확인
    const img = ftdCardMedia.locator('img');
    await expect(img).toBeVisible();
    await expect(img).toHaveAttribute('src', /file_tower_defense_title\.png/);

    // 유튜브 뱃지 확인
    const badge = ftdCardMedia.locator('.pf-media-badge');
    await expect(badge).toContainText('YouTube');
  });

  test('홈 카드에서 상세 포스트로 넘어가면 인게임 스크린샷 갤러리가 표시된다', async ({ page }) => {
    const postLink = page.locator('article[data-project="01"] a.pf-btn-post');
    await expect(postLink).toBeVisible();
    await postLink.click();

    // 상세 포스트 페이지 URL 확인
    await expect(page).toHaveURL(/\/project\/file-tower-defense\//);

    // 인게임 스크린샷 갤러리 섹션 확인
    const gallerySection = page.locator('#gallery');
    await expect(gallerySection).toBeVisible();

    const postGallery = page.locator('#pfFtdGalleryPost');
    await expect(postGallery).toBeVisible();

    // 첫 번째 인게임 스크린샷 표시 확인
    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toBeVisible();
    await expect(mainImg).toHaveAttribute('src', /ftd_screenshot_01\.png/);

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('01 / 05');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('파일 유닛 배치 및 웨이브 방어');
  });

  test('상세 포스트 페이지에서 다음(›)/이전(‹) 버튼으로 스크린샷을 넘겨볼 수 있다', async ({ page }) => {
    await page.goto('/project/file-tower-defense/');
    const postGallery = page.locator('#pfFtdGalleryPost');

    const nextBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const prevBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-prev');
    const counter = postGallery.locator('.pf-ftd-counter');
    const mainImg = postGallery.locator('.pf-ftd-main-img');

    // 다음 버튼 클릭 -> 2번째 스크린샷
    await nextBtn.click();
    await expect(counter).toHaveText('02 / 05');
    await expect(mainImg).toHaveAttribute('src', /ftd_screenshot_02\.png/);

    // 이전 버튼 클릭 -> 1번째 스크린샷
    await prevBtn.click();
    await expect(counter).toHaveText('01 / 05');
    await expect(mainImg).toHaveAttribute('src', /ftd_screenshot_01\.png/);
  });

  test('상세 포스트 페이지에서 특정 썸네일 클릭 시 해당 스크린샷으로 즉시 전환된다', async ({ page }) => {
    await page.goto('/project/file-tower-defense/');
    const postGallery = page.locator('#pfFtdGalleryPost');

    // 3번째 썸네일(인게임 언락 상점) 클릭
    const thumb3 = postGallery.locator('.pf-ftd-thumb-card[data-index="2"]');
    await thumb3.click();

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('03 / 05');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('인게임 언락 상점(UnLock Shop) 시스템');

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toHaveAttribute('src', /ftd_screenshot_03\.png/);
    await expect(thumb3).toHaveClass(/is-active/);
  });

  test('홈 화면 요약 모달에서도 스크린샷 갤러리가 정상 동작한다', async ({ page }) => {
    await page.locator('button[aria-controls="fileTowerDefense"]').click();
    const modal = page.locator('#fileTowerDefense');
    await expect(modal).toBeVisible();

    const modalGallery = modal.locator('#pfFtdGallery');
    await expect(modalGallery).toBeVisible();

    const nextBtn = modalGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const counter = modalGallery.locator('.pf-ftd-counter');

    await nextBtn.click();
    await expect(counter).toHaveText('02 / 05');

    await page.keyboard.press('Escape');
    await expect(modal).toBeHidden();
  });
});
