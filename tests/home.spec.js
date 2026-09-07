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

  test('접속 즉시 잠금/로그인 화면 없이 포트폴리오 Hero 섹션이 바로 노출된다', async ({ page }) => {
    await page.goto('/');

    // 1. 잠금/로그인 화면 오버레이 및 잠금 버튼이 존재하지 않는지 확인
    const winScreen = page.locator('#win-lock-screen');
    await expect(winScreen).toHaveCount(0);

    const lockBtn = page.locator('#win-lock-btn');
    await expect(lockBtn).toHaveCount(0);

    // 2. 포트폴리오 Hero 타이틀 및 소개글이 즉시 노출되는지 확인
    const heroTitle = page.locator('.pf-hero-title');
    await expect(heroTitle).toBeVisible();
    await expect(heroTitle).toContainText('이민혁');

    const heroDesc = page.locator('.pf-hero-desc');
    await expect(heroDesc).toBeVisible();

    // 3. 외부 프로필 링크 노출 확인
    const profileLinks = page.locator('.pf-profile-links');
    await expect(profileLinks).toBeVisible();

    const githubLink = page.locator('.pf-profile-link--primary');
    await expect(githubLink).toHaveAttribute('href', 'https://github.com/InfiSword');

    const projectName = test.info().project.name.replace(/\s+/g, '_');
    await page.screenshot({ path: `test-results/home-direct-${projectName}.png` });
  });
});
