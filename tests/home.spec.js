const { test, expect } = require('@playwright/test');

test.describe('포트폴리오 홈 페이지 테스트', () => {
  test('홈 페이지가 정상 로드되고 올바른 타이틀을 표시한다', async ({ page }) => {
    await page.goto('/');

    // 타이틀 확인
    await expect(page).toHaveTitle(/Minhyuk Lee \| Game Developer/);

    // 헤더 및 네비게이션 확인
    const header = page.locator('header.site-header');
    await expect(header).toBeVisible();

    const homeNav = page.locator('nav.site-nav a[href="/"]');
    await expect(homeNav).toBeVisible();

    const projectsNav = page.locator('nav.site-nav a[href="/#projects"]');
    await expect(projectsNav).toBeVisible();
  });

  test('프로젝트 섹션이 정상적으로 렌더링된다', async ({ page }) => {
    await page.goto('/');

    const projectsSection = page.locator('#projects');
    await expect(projectsSection).toBeAttached();
  });
});
