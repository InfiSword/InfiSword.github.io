const { chromium } = require('@playwright/test');
const path = require('path');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage({ viewport: { width: 1280, height: 1100 } });

  await page.goto('http://localhost:4173/project/worldfirstkill/');

  // 1. Capture Section 1.1 Live Master Sheets
  const section1_1 = page.locator('.pf-visual-frame:has(span:has-text("LIVE GOOGLE SPREADSHEET"))');
  await section1_1.evaluate((el) => {
    const y = el.getBoundingClientRect().top + window.pageYOffset - 90;
    window.scrollTo({ top: y, behavior: 'instant' });
  });
  await page.waitForTimeout(1000);
  const out1 = path.resolve('C:/Users/seif4/.gemini/antigravity/brain/85ef00c0-1845-4994-b1fe-96164d48a59e/wfk_section1_sheets_rendered.png');
  await page.screenshot({ path: out1 });
  console.log('Saved Section 1.1 screenshot to:', out1);

  // 2. Capture Section 2 start
  const section2 = page.locator('h2:has-text("2. 실전 예시로 보는 리플렉션 CSV 파싱 전개 과정")');
  await section2.evaluate((el) => {
    const y = el.getBoundingClientRect().top + window.pageYOffset - 90;
    window.scrollTo({ top: y, behavior: 'instant' });
  });
  await page.waitForTimeout(1000);
  const out2 = path.resolve('C:/Users/seif4/.gemini/antigravity/brain/85ef00c0-1845-4994-b1fe-96164d48a59e/wfk_section2_clean_rendered.png');
  await page.screenshot({ path: out2 });
  console.log('Saved Section 2 screenshot to:', out2);

  await browser.close();
})();
