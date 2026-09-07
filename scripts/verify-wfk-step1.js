const { chromium } = require('@playwright/test');
const path = require('path');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage({ viewport: { width: 1280, height: 1300 } });

  await page.goto('http://localhost:4173/project/worldfirstkill/');

  const step1 = page.locator('.pf-fc-card:has(h4:has-text("STEP 1. DTO 필드 분석"))');
  
  // Scroll element into view with header offset
  await step1.evaluate((el) => {
    const y = el.getBoundingClientRect().top + window.pageYOffset - 90;
    window.scrollTo({ top: y, behavior: 'instant' });
  });
  await page.waitForTimeout(1000);

  const outputPath = path.resolve('C:/Users/seif4/.gemini/antigravity/brain/85ef00c0-1845-4994-b1fe-96164d48a59e/wfk_step1_code_rendered.png');
  await page.screenshot({ path: outputPath });
  console.log('Saved clean viewport screenshot to:', outputPath);

  await browser.close();
})();
