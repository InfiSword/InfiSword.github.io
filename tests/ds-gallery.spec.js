const { test, expect } = require('@playwright/test');

test.describe('WinAPI Don\'t Starve 스크린샷 갤러리 테스트 (상세 포스트 & 모달)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    const winScreen = page.locator('#win-lock-screen');
    if (await winScreen.isVisible()) {
      await winScreen.click();
      await expect(winScreen).toBeHidden({ timeout: 3000 });
    }
  });

  test('홈 카드 오른쪽 미디어가 타이틀 이미지를 표시하고 유튜브 링크를 가진다', async ({ page }) => {
    const dsCardMedia = page.locator('article[data-project="02"] .pf-card-media');
    await expect(dsCardMedia).toBeVisible();

    // 유튜브 링크 속성 확인
    await expect(dsCardMedia).toHaveAttribute('href', 'https://www.youtube.com/watch?v=TTdIN2U3gnQ');
    await expect(dsCardMedia).toHaveAttribute('target', '_blank');

    // 새로 적용된 타이틀 이미지 확인
    const img = dsCardMedia.locator('img');
    await expect(img).toBeVisible();
    await expect(img).toHaveAttribute('src', /dont_starve_title\.png/);

    // 유튜브 뱃지 확인
    const badge = dsCardMedia.locator('.pf-media-badge');
    await expect(badge).toContainText('YouTube');
  });

  test('상세 포스트 페이지에서 Don\'t Starve 8종 스크린샷 갤러리가 올바르게 렌더링된다', async ({ page }) => {
    await page.goto('/project/winapi-dont-starve/');

    const gallerySection = page.locator('#gallery');
    await expect(gallerySection).toBeVisible();

    const postGallery = page.locator('#pfDsGalleryPost');
    await expect(postGallery).toBeVisible();

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toBeVisible();
    await expect(mainImg).toHaveAttribute('src', /ds_ingame_01\.png/);

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('01 / 08');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('윌슨(Wilson) 캐릭터 선택 화면');

    const thumbs = postGallery.locator('.pf-ftd-thumb-card');
    await expect(thumbs).toHaveCount(8);
  });

  test('상세 포스트 페이지에서 다음(›)/이전(‹) 버튼으로 스크린샷을 넘겨볼 수 있다', async ({ page }) => {
    await page.goto('/project/winapi-dont-starve/');
    const postGallery = page.locator('#pfDsGalleryPost');

    const nextBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const prevBtn = postGallery.locator('.pf-ftd-nav.pf-ftd-prev');
    const counter = postGallery.locator('.pf-ftd-counter');
    const mainImg = postGallery.locator('.pf-ftd-main-img');

    // 다음 버튼 클릭 -> 2번째 스크린샷 (오픈월드 필드 생태계)
    await nextBtn.click();
    await expect(counter).toHaveText('02 / 08');
    await expect(mainImg).toHaveAttribute('src', /ds_ingame_02\.png/);

    // 이전 버튼 클릭 -> 1번째 스크린샷
    await prevBtn.click();
    await expect(counter).toHaveText('01 / 08');
    await expect(mainImg).toHaveAttribute('src', /ds_ingame_01\.png/);
  });

  test('상세 포스트 페이지에서 특정 썸네일(거미 여왕) 클릭 시 해당 스크린샷으로 즉시 전환된다', async ({ page }) => {
    await page.goto('/project/winapi-dont-starve/');
    const postGallery = page.locator('#pfDsGalleryPost');

    // 7번째 썸네일(거대 보스 거미 여왕, 인덱스 6) 클릭
    const thumb7 = postGallery.locator('.pf-ftd-thumb-card[data-index="6"]');
    await thumb7.click();

    const counter = postGallery.locator('.pf-ftd-counter');
    await expect(counter).toHaveText('07 / 08');

    const caption = postGallery.locator('.pf-ftd-caption');
    await expect(caption).toHaveText('거대 보스 거미 여왕(Spider Queen) 레이드 전투');

    const mainImg = postGallery.locator('.pf-ftd-main-img');
    await expect(mainImg).toHaveAttribute('src', /ds_ingame_07\.png/);
    await expect(thumb7).toHaveClass(/is-active/);
  });

  test('홈 화면 Don\'t Starve 요약 모달에서도 스크린샷 갤러리가 정상 동작한다', async ({ page }) => {
    await page.locator('button[aria-controls="dontStarve"]').click();
    const modal = page.locator('#dontStarve');
    await expect(modal).toBeVisible();

    const modalGallery = modal.locator('#pfDsGalleryModal');
    await expect(modalGallery).toBeVisible();

    const nextBtn = modalGallery.locator('.pf-ftd-nav.pf-ftd-next');
    const counter = modalGallery.locator('.pf-ftd-counter');

    await nextBtn.click();
    await expect(counter).toHaveText('02 / 08');

    // 모달 내 "상세 포스트 보기 ↗" 링크 확인
    const detailLink = modalGallery.locator('a[href="/project/winapi-dont-starve/#gallery"]');
    await expect(detailLink).toBeVisible();

    await page.keyboard.press('Escape');
    await expect(modal).toBeHidden();
  });
});
