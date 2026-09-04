const { test, expect } = require('@playwright/test');

const PROJECTS = [
  {
    slug: 'file-tower-defense',
    title: 'FILE_TOWER_DEFENSE',
    imagePattern: /file_tower_defense_title\.png/,
    screenshotName: 'hero-ftd.png',
  },
  {
    slug: 'winapi-dont-starve',
    title: 'WINAPI_DONT_STARVE',
    imagePattern: /dont_starve_title\.png/,
    screenshotName: 'hero-ds.png',
  },
  {
    slug: 'worldfirstkill',
    title: 'WORLDFIRSTKILL',
    imagePattern: /173659\.png/,
    screenshotName: 'hero-wfk.png',
  },
  {
    slug: 'autonomous-racing-agent',
    title: 'AUTONOMOUS_RACING_AGENT',
    imagePattern: /105116\.png/,
    screenshotName: 'hero-racing.png',
  },
  {
    slug: 'slime-project',
    title: 'SLIME PROJECT',
    imagePattern: /slide_40_img_31\.png/,
    screenshotName: 'hero-slime.png',
  },
  {
    slug: 'moonlight',
    title: '보름달 방앗간',
    imagePattern: /slide_42_img_30\.png/,
    screenshotName: 'hero-moonlight.png',
  },
];

test.describe('프로젝트 상세 페이지 상단 헤더 대표 이미지 및 그라데이션 테스트', () => {
  for (const proj of PROJECTS) {
    test(`[${proj.slug}] 헤더 패널에 대표 이미지가 올바르게 렌더링되고 그라데이션 오버레이가 적용된다`, async ({ page }) => {
      await page.goto(`/project/${proj.slug}/`);

      const hero = page.locator('.project-hero');
      await expect(hero).toBeVisible();

      // 헤더 텍스트 콘텐츠 확인
      const content = hero.locator('.project-hero__content');
      await expect(content).toBeVisible();
      const h1 = content.locator('h1');
      await expect(h1).toContainText(proj.title);

      // 대표 이미지 컨테이너 및 이미지 확인
      const art = hero.locator('.project-hero__art');
      await expect(art).toBeVisible();

      const img = art.locator('.project-hero__art-img');
      await expect(img).toBeVisible();
      await expect(img).toHaveAttribute('src', proj.imagePattern);

      // 그라데이션 오버레이 확인
      const gradient = art.locator('.project-hero__art-gradient');
      await expect(gradient).toBeVisible();

      // 스크린샷 캡처 (데스크톱)
      await hero.screenshot({
        path: `C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/${proj.screenshotName}`,
      });
    });
  }

  test('모바일 화면에서도 헤더 패널이 깨짐 없이 렌더링된다', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto('/project/file-tower-defense/');

    const hero = page.locator('.project-hero');
    await expect(hero).toBeVisible();

    const content = hero.locator('.project-hero__content');
    await expect(content).toBeVisible();

    await hero.screenshot({
      path: 'C:/Users/seif4/.gemini/antigravity/brain/a3bd1d93-4e13-4a43-8570-d834d36e9c08/hero-ftd-mobile.png',
    });
  });
});
