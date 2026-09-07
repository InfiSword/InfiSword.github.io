const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  await page.setViewportSize({ width: 1280, height: 950 });
  await page.goto('http://127.0.0.1:4173/project/worldfirstkill/');

  const pipeHeader = page.locator('h4:has-text("SplitMix64 기반 서브 시드 파생 파이프라인")');
  await pipeHeader.scrollIntoViewIfNeeded();
  await page.waitForTimeout(400);
  await page.screenshot({ path: 'scratch/sec3_2_pipeline.png' });

  await browser.close();
  console.log('Captured sec3_2_pipeline.png');
})();
