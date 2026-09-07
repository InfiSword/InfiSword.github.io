const { chromium } = require('@playwright/test');
const path = require('path');
const fs = require('fs');

(async () => {
  const outputDir = path.resolve(__dirname, '../assets/images/World First Kill');

  const browser = await chromium.launch({
    headless: true,
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });

  const context = await browser.newContext({
    viewport: { width: 1400, height: 860 },
    locale: 'ko-KR',
  });

  const page = await context.newPage();

  console.log('1. Loading Master Meta Sheet...');
  const masterUrl = 'https://docs.google.com/spreadsheets/d/1lRHkPJ4bBWnLBiRpdvULuOsfFk-dIVAqr0B-rzpZxPU/edit#gid=0';
  await page.goto(masterUrl, { waitUntil: 'domcontentloaded', timeout: 30000 });
  await page.waitForTimeout(6000);

  // Click '확인' button or dismiss popup
  try {
    const confirmBtn = page.getByRole('button', { name: '확인' });
    if (await confirmBtn.isVisible()) {
      await confirmBtn.click();
      console.log('Clicked 확인 button!');
    }
  } catch (e) {
    console.log('확인 button error:', e);
  }

  await page.keyboard.press('Escape');
  await page.evaluate(() => {
    const butterbar = document.getElementById('docs-butterbar-container');
    if (butterbar) butterbar.style.display = 'none';
    const butterWraps = document.querySelectorAll('.docs-butterbar-wrap');
    butterWraps.forEach(el => el.style.display = 'none');
    const popups = document.querySelectorAll('.modal-dialog, .docs-material-dialog, [role="dialog"], .modal-dialog-bg');
    popups.forEach(el => el.style.display = 'none');
  });

  await page.waitForTimeout(1000);
  const masterPath = path.join(outputDir, 'wfk_master_meta_sheet.png');
  await page.screenshot({ path: masterPath });
  console.log('Saved clean master sheet screenshot to:', masterPath);

  await browser.close();
  console.log('Master sheet updated successfully!');
})();
