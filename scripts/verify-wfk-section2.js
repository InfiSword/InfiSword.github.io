const { chromium } = require('@playwright/test');
const path = require('path');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage({ viewport: { width: 1280, height: 1100 } });

  await page.goto('http://localhost:4173/project/worldfirstkill/');

  // Locate the visual frame inside section 2
  const frame = page.locator('h2:has-text("2. 실전 예시로 보는 리플렉션 CSV 파싱 전개 과정") + p + .pf-visual-frame');
  await frame.scrollIntoViewIfNeeded();
  await page.waitForTimeout(1000);

  const outputPath = path.resolve('C:/Users/seif4/.gemini/antigravity/brain/85ef00c0-1845-4994-b1fe-96164d48a59e/wfk_section2_images_rendered.png');
  await frame.screenshot({ path: outputPath });
  console.log('Saved frame screenshot to:', outputPath);

  await browser.close();
})();
