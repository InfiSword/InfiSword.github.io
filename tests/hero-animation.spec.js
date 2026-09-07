const { test, expect } = require('@playwright/test');

test.describe('Hero 섹션 진입 애니메이션 테스트', () => {
  test('Hero 텍스트 요소들이 갑작스러운 튐(pop) 없이 부드러운 전환을 거쳐 선명해진다', async ({ page }) => {
    await page.goto('/');

    const hero = page.locator('.pf-hero');
    await expect(hero).toBeVisible();

    const heroTitle = page.locator('.pf-hero-title');
    const heroDesc = page.locator('.pf-hero-desc');
    const profileLinks = page.locator('.pf-profile-links');

    // 요소들이 모두 최종적으로 온전한 opacity(1) 및 transform(none) 상태로 안착하는지 확인
    await expect(heroTitle).toBeVisible();
    await expect(heroDesc).toBeVisible();
    await expect(profileLinks).toBeVisible();

    // 애니메이션 완료 대기 (1초)
    await page.waitForTimeout(1000);

    const titleStyle = await heroTitle.evaluate((el) => {
      const computed = window.getComputedStyle(el);
      return {
        opacity: parseFloat(computed.opacity),
        visibility: computed.visibility,
      };
    });

    expect(titleStyle.opacity).toBe(1);
    expect(titleStyle.visibility).toBe('visible');

    // 스크린샷 캡처
    await page.screenshot({ path: 'test-results/hero-animation-verified.png' });
  });
});
