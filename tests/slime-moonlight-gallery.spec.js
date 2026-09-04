const { test, expect } = require('@playwright/test');

test.describe('Slime Project & MoonLight 스크린샷 갤러리 테스트 (상세 포스트 & 모달)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 3000 });
    }
  });

  // ==========================================
  // 1. SLIME PROJECT TESTS
  // ==========================================
  test('상세 포스트 페이지에서 Slime Project 4종 스크린샷 갤러리가 올바르게 렌더링된다', async ({ page, isMobile }) => {
    await page.goto('/project/slime-project/');

    const gallerySection = page.locator('#gallery');
    await expect(gallerySection).toBeVisible();

    const postGallery = page.locator('#pfSlimeGalleryPost');
    await expect(postGallery).toBeVisible();

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toBeVisible();
    await expect(mainImg).toHaveAttribute('src', /slime_ingame_01\.png/);

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('01 / 04');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('거대 곰 보스 몬스터와의 전투 및 패턴 공략');

    const thumbs = postGallery.locator('.pf-ftd-thumb-card');
    await expect(thumbs).toHaveCount(4);

    await gallerySection.scrollIntoViewIfNeeded();
    await page.waitForTimeout(300);
    const filename = isMobile ? 'slime-post-gallery-mobile.png' : 'slime-post-gallery-desktop.png';
    await gallerySection.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });
  });

  test('상세 포스트 페이지에서 Slime 다음/이전 및 썸네일 클릭 전환이 정상 작동한다', async ({ page }) => {
    await page.goto('/project/slime-project/');
    const postGallery = page.locator('#pfSlimeGalleryPost');

    const nextBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const prevBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-prev');
    const counter = postGallery.locator('.pf-ftd-counter');
    const mainImg = postGallery.locator('.pf-ftd-main-img');

    // 다음 버튼 클릭 -> 2번째 스크린샷
    await nextBtn.click();
    await expect(counter).toHaveText('02 / 04');
    await expect(mainImg).toHaveAttribute('src', /slime_ingame_02\.png/);

    // 4번째 썸네일(인덱스 3) 직접 클릭
    const thumb4 = postGallery.locator('.pf-ftd-thumb-card[data-index="3"]');
    await thumb4.click();
    await expect(counter).toHaveText('04 / 04');
    await expect(mainImg).toHaveAttribute('src', /slime_ingame_04\.png/);
    await expect(thumb4).toHaveClass(/is-active/);

    // 이전 버튼 클릭 -> 3번째 스크린샷
    await prevBtn.click();
    await expect(counter).toHaveText('03 / 04');
    await expect(mainImg).toHaveAttribute('src', /slime_ingame_03\.png/);
  });

  test('홈 화면 Slime Project 요약 모달에서도 스크린샷 갤러리가 정상 동작한다', async ({ page, isMobile }) => {
    await page.locator('button[aria-controls="slime"]').click();
    const modal = page.locator('#slime');
    await expect(modal).toBeVisible();

    const modalGallery = modal.locator('#pfSlimeGalleryModal');
    await expect(modalGallery).toBeVisible();

    const nextBtn = modalGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const counter = modalGallery.locator('.pf-ftd-counter');

    await nextBtn.click();
    await expect(counter).toHaveText('02 / 04');

    // 모달 내 "상세 포스트 보기 ↗" 링크 확인
    const detailLink = modalGallery.locator('a[href="/project/slime-project/#gallery"]');
    await expect(detailLink).toBeVisible();

    await modal.evaluate(el => el.scrollTop = 0);
    await page.waitForTimeout(400);
    const filename = isMobile ? 'slime-modal-mobile.png' : 'slime-modal-desktop.png';
    await page.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });

    await page.keyboard.press('Escape');
    await expect(modal).toBeHidden();
  });

  // ==========================================
  // 2. MOONLIGHT (보름달 방앗간) TESTS
  // ==========================================
  test('상세 포스트 페이지에서 MoonLight 7종 스크린샷 갤러리가 올바르게 렌더링된다', async ({ page, isMobile }) => {
    await page.goto('/project/moonlight/');

    const gallerySection = page.locator('#gallery');
    await expect(gallerySection).toBeVisible();

    const postGallery = page.locator('#pfMlGalleryPost');
    await expect(postGallery).toBeVisible();

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toBeVisible();
    await expect(mainImg).toHaveAttribute('src', /ml_ingame_01\.png/);

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('01 / 07');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('매장 전경 및 손님·직원 NPC 운영 루프');

    const thumbs = postGallery.locator('.pf-ftd-thumb-card');
    await expect(thumbs).toHaveCount(7);

    await gallerySection.scrollIntoViewIfNeeded();
    await page.waitForTimeout(300);
    const filename = isMobile ? 'moonlight-post-gallery-mobile.png' : 'moonlight-post-gallery-desktop.png';
    await gallerySection.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });
  });

  test('상세 포스트 페이지에서 MoonLight 다음/이전 및 썸네일 클릭 전환이 정상 작동한다', async ({ page }) => {
    await page.goto('/project/moonlight/');
    const postGallery = page.locator('#pfMlGalleryPost');

    const nextBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const prevBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-prev');
    const counter = postGallery.locator('.pf-ftd-counter');
    const mainImg = postGallery.locator('.pf-ftd-main-img');

    // 다음 버튼 클릭 -> 2번째 스크린샷
    await nextBtn.click();
    await expect(counter).toHaveText('02 / 07');
    await expect(mainImg).toHaveAttribute('src', /ml_ingame_02\.png/);

    // 7번째 썸네일(인벤토리, 인덱스 6) 직접 클릭
    const thumb7 = postGallery.locator('.pf-ftd-thumb-card[data-index="6"]');
    await thumb7.click();
    await expect(counter).toHaveText('07 / 07');
    await expect(mainImg).toHaveAttribute('src', /ml_ingame_07\.png/);
    await expect(thumb7).toHaveClass(/is-active/);

    // 이전 버튼 클릭 -> 6번째 스크린샷
    await prevBtn.click();
    await expect(counter).toHaveText('06 / 07');
    await expect(mainImg).toHaveAttribute('src', /ml_ingame_06\.png/);
  });

  test('홈 화면 MoonLight 요약 모달에서도 스크린샷 갤러리가 정상 동작한다', async ({ page, isMobile }) => {
    await page.locator('button[aria-controls="moonlight"]').click();
    const modal = page.locator('#moonlight');
    await expect(modal).toBeVisible();

    const modalGallery = modal.locator('#pfMlGalleryModal');
    await expect(modalGallery).toBeVisible();

    const nextBtn = modalGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const counter = modalGallery.locator('.pf-ftd-counter');

    await nextBtn.click();
    await expect(counter).toHaveText('02 / 07');

    // 모달 내 "상세 포스트 보기 ↗" 링크 확인
    const detailLink = modalGallery.locator('a[href="/project/moonlight/#gallery"]');
    await expect(detailLink).toBeVisible();

    await modal.evaluate(el => el.scrollTop = 0);
    await page.waitForTimeout(400);
    const filename = isMobile ? 'moonlight-modal-mobile.png' : 'moonlight-modal-desktop.png';
    await page.screenshot({ path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${filename}` });

    await page.keyboard.press('Escape');
    await expect(modal).toBeHidden();
  });
});
