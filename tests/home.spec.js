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

  test('로딩바 완료 후 Enter 키를 눌러 포트폴리오로 정상 진입한다', async ({ page }) => {
    await page.goto('/');

    const winScreen = page.locator('#win-lock-screen');
    await expect(winScreen).toBeVisible();

    const userName = page.locator('.win-user-name');
    await expect(userName).toContainText('이민혁');

    // 상단 SYSTEM READY 배지가 제거되었는지 확인
    await expect(page.locator('.win-system-badge')).toHaveCount(0);
    await expect(page.locator('text=SYSTEM READY')).toHaveCount(0);
    await expect(page.locator('text=FILE_OS')).toHaveCount(0);

    // 1. 로딩바 요소 확인 (1.2초 로딩 후 완료 시 숨겨지므로 DOM 존재 확인)
    const progressBar = page.locator('#win-progress-bar');
    await expect(progressBar).toBeAttached();

    const prompt = page.locator('#win-action-prompt');
    await expect(prompt).toBeVisible({ timeout: 5000 });
    const projectName = test.info().project.name.replace(/\s+/g, '_');
    await page.screenshot({ path: `test-results/loading-screen-${projectName}.png` });

    // 3. Enter 키 입력으로 언락
    await page.keyboard.press('Enter');

    // 4. 화면 퇴장 및 본문 포트폴리오 노출 확인
    await expect(winScreen).toBeHidden({ timeout: 2500 });
    const hero = page.locator('.pf-hero-title');
    await expect(hero).toBeVisible();
  });

  test('화면 아무 곳이나 클릭 시 즉시 언락된다', async ({ page }) => {
    await page.goto('/');

    const winScreen = page.locator('#win-lock-screen');
    await expect(winScreen).toBeVisible();

    // 화면 아무 곳이나 클릭
    await winScreen.click();

    await expect(winScreen).toBeHidden({ timeout: 2000 });
    const hero = page.locator('.pf-hero-title');
    await expect(hero).toBeVisible();
  });
});
