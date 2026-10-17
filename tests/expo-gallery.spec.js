const { test, expect } = require('@playwright/test');

test('EXPO 활동에서 현장 사진 9장을 열람하고 기존 분류로 이동한다', async ({ page }) => {
  await page.goto('/');
  const activity = page.locator('#pf-expo-activity');
  await expect(activity).toContainText('팀원들과 함께 체험 부스를 운영');
  await expect(activity.locator('time')).toHaveText(['2026.09.30', '2026.10.02']);
  await expect(activity.getByRole('link', { name: 'KINTEX 행사 안내' }))
    .toHaveAttribute('href', 'https://www.kintex.com/web/ko/event/view.do?seq=26072901');

  await activity.getByRole('button', { name: 'EXPO 현장 사진 (9장)' }).click();
  const modal = page.locator('#pfAwardGalleryModal');
  const image = modal.locator('#pfAwardMainImg');
  await expect(modal).toBeVisible();
  expect(await modal.locator('[data-category="expo"]').evaluate(tab => {
    const bounds = tab.getBoundingClientRect();
    const bar = tab.parentElement.getBoundingClientRect();
    return bounds.left >= bar.left - 1 && bounds.right <= bar.right + 1 &&
      bounds.top >= bar.top - 1 && bounds.bottom <= bar.bottom + 1 && bounds.height >= 44;
  })).toBe(true);
  await expect(modal.locator('.pf-award-thumb-card')).toHaveCount(9);
  await expect(modal.locator('#pfAwardDate')).toHaveText('행사 기간: 2026.09.30 ~ 2026.10.02');
  await expect(modal.locator('#pfAwardViewRaw')).toHaveAttribute('href', /expo2026\/booth-demo.jpg$/);
  await expect.poll(() => image.evaluate(img => img.complete && img.naturalWidth > 0)).toBe(true);
  await expect(image).toHaveCSS('opacity', '1');
  await page.screenshot({ path: `test-results/expo-${test.info().project.name.replace(/\s/g, '-')}.png` });

  const photos = new Set();
  for (let index = 0; index < 9; index++) {
    await expect(modal.locator('#pfAwardCounter')).toHaveText(`${index + 1} / 9`);
    await expect.poll(() => image.evaluate(img => img.complete && img.naturalWidth > 0)).toBe(true);
    photos.add(await image.getAttribute('src'));
    await modal.getByRole('button', { name: '다음 사진', exact: true }).click();
  }
  expect(photos.size).toBe(9);
  await expect(modal.locator('#pfAwardCounter')).toHaveText('1 / 9');
  await page.keyboard.press('ArrowLeft');
  await expect(modal.locator('#pfAwardCounter')).toHaveText('9 / 9');
  await modal.locator('[data-category="certificates"]').click();
  await expect(modal.locator('#pfAwardCounter')).toHaveText('1 / 11');
  await modal.locator('[data-category="all"]').click();
  await expect(modal.locator('#pfAwardCounter')).toHaveText('1 / 60');
  await page.keyboard.press('Escape');
  await expect(modal).toBeHidden();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
});
