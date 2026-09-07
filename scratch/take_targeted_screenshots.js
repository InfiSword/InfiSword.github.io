const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  await page.setViewportSize({ width: 1280, height: 1000 });
  await page.goto('http://127.0.0.1:4173/project/worldfirstkill/');

  // 1. Target the Minecraft frame specifically
  const mcFrame = page.locator('.pf-visual-frame:has-text("마인크래프트의 결정론적 월드 생성 시드")');
  await mcFrame.scrollIntoViewIfNeeded();
  await page.waitForTimeout(400);
  await mcFrame.screenshot({ path: 'scratch/minecraft_seed_frame.png' });

  // 2. Target the Seed Comparison Grid specifically
  const seedGrid = page.locator('.pf-seed-compare-grid');
  await seedGrid.scrollIntoViewIfNeeded();
  await page.waitForTimeout(400);
  await seedGrid.screenshot({ path: 'scratch/seed_compare_grid_full.png' });

  // 3. Mobile seed compare grid
  await page.setViewportSize({ width: 375, height: 812 });
  await seedGrid.scrollIntoViewIfNeeded();
  await page.waitForTimeout(400);
  await seedGrid.screenshot({ path: 'scratch/seed_compare_grid_mobile.png' });

  await browser.close();
  console.log('Targeted component screenshots captured');
})();
