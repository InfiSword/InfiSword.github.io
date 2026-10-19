const { test, expect } = require('@playwright/test');

test.beforeEach(async ({ page }) => {
  await page.goto('/project/slime-project/');
});

test('세부 코드 없이 핵심 설계와 역할을 간결하게 설명한다', async ({ page }) => {
  await expect(page.locator('h2.chapter-title')).toHaveText([
    '1. 프로젝트 개요',
    '2. FiniteStateMachine 기반 상태 관리 시스템',
    '3. 플레이어 슬라임 및 변신 시스템',
  ]);
  const content = page.locator('.project-content');
  await expect(content.locator('pre, details')).toHaveCount(0);
  await expect(content).toContainText('몬스터의 행동 전환은 FSM이, 플레이어의 기능 조합은 플레이어 컨트롤러가 관리합니다.');
  await expect(content).toContainText('행동 규칙과 조정값을 분리');
  await expect(content).toContainText('더블 점프와 벽 점프');

  const coreText = await content.evaluate(element => {
    const range = document.createRange();
    range.setStartBefore(element.querySelector('h2.chapter-title'));
    range.setEndAfter(element.lastElementChild);
    return range.toString();
  });
  expect(coreText.length).toBeLessThanOrEqual(1600);
  expect(coreText).not.toMatch(/LogicUpdate|PhysicsUpdate|DoChecks|TriggerDashing|MainBossBear|Enemy_State/);
});

test('간결한 목차와 상태 역할 표를 데스크톱과 모바일에서 읽을 수 있다', async ({ page }, testInfo) => {
  const links = await page.locator('.toc-list a').evaluateAll(items => items.map(link => link.hash));
  const brokenLinks = await page.locator('.toc-list a').evaluateAll(items => items.filter(link =>
    !document.getElementById(decodeURIComponent(link.hash.slice(1)))));
  expect(brokenLinks).toEqual([]);
  expect(links.some(hash => hash.startsWith('#4'))).toBe(false);

  const fragment = '#22-state-베이스-클래스';
  const link = page.locator('.toc-list a').filter({ hasText: '2.2 State 베이스 클래스' });
  if (await link.isVisible()) {
    await link.click();
  } else {
    await page.goto(`/project/slime-project/${fragment}`);
  }
  await expect(page.locator('[id="22-state-베이스-클래스"]')).toBeInViewport();
  const table = page.locator('.project-content table');
  await expect(table.locator('tbody tr')).toHaveCount(3);
  await expect(table).toContainText('Monster_State');
  await expect(table).toContainText('Player_State');
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
  await page.screenshot({ path: testInfo.outputPath('concise-state-overview.png') });
});
