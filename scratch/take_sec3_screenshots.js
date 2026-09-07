const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  
  // Desktop
  {
    const page = await browser.newPage();
    await page.setViewportSize({ width: 1280, height: 900 });
    await page.goto('http://127.0.0.1:4173/project/worldfirstkill/');

    const sec3_1 = page.locator('h3:has-text("3.1 마인크래프트에서 착안한 발상")');
    await sec3_1.scrollIntoViewIfNeeded();
    await page.waitForTimeout(500);
    await page.screenshot({ path: 'scratch/sec3_1_desktop.png' });

    const sec3_4 = page.locator('h4:has-text("실제 인게임 실증: 시드값에 따른 스킬셋 절차적 생성 결과 비교")');
    await sec3_4.scrollIntoViewIfNeeded();
    await page.waitForTimeout(500);
    await page.screenshot({ path: 'scratch/sec3_4_desktop_grid.png' });

    const archive = page.locator('.pf-visual-frame:has-text("포트폴리오 원본: Seed 비교")');
    await archive.scrollIntoViewIfNeeded();
    await page.waitForTimeout(500);
    await page.screenshot({ path: 'scratch/sec3_4_desktop_archive.png' });
    await page.close();
  }

  // Mobile
  {
    const page = await browser.newPage();
    await page.setViewportSize({ width: 375, height: 812 });
    await page.goto('http://127.0.0.1:4173/project/worldfirstkill/');

    const sec3_1 = page.locator('h3:has-text("3.1 마인크래프트에서 착안한 발상")');
    await sec3_1.scrollIntoViewIfNeeded();
    await page.waitForTimeout(500);
    await page.screenshot({ path: 'scratch/sec3_1_mobile.png' });

    const sec3_4 = page.locator('h4:has-text("실제 인게임 실증: 시드값에 따른 스킬셋 절차적 생성 결과 비교")');
    await sec3_4.scrollIntoViewIfNeeded();
    await page.waitForTimeout(500);
    await page.screenshot({ path: 'scratch/sec3_4_mobile_grid.png' });
    await page.close();
  }

  await browser.close();
  console.log('All verification screenshots captured successfully');
})();
