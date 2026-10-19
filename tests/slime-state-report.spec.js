const { test, expect } = require('@playwright/test');

test.beforeEach(async ({ page }) => {
  await page.goto('/project/slime-project/');
});

test('간결한 핵심 설명을 유지하며 관련 코드를 접어서 제공한다', async ({ page }) => {
  await expect(page.locator('h2.chapter-title')).toHaveText([
    '1. 프로젝트 개요',
    '2. FiniteStateMachine 기반 상태 관리 시스템',
    '3. 플레이어 슬라임 및 변신 시스템',
  ]);
  const content = page.locator('.project-content');
  const details = content.locator('details.pf-shared-state-code');
  await expect(details).toHaveCount(6);
  const declarations = [
    'public class FiniteStateMachine',
    'public class State',
    'public class Monster_State : State',
    'public override void Enter()',
    'movement = new Player_SlimeMovement',
    'public void OnKeyboard()',
  ];
  for (let index = 0; index < declarations.length; index++) {
    await expect(details.nth(index)).not.toHaveAttribute('open');
    await expect(details.nth(index).locator('pre')).toBeHidden();
    await expect(details.nth(index).locator('code')).toContainText(declarations[index]);
  }
  expect(await content.locator('pre').evaluateAll(blocks => blocks.every(block => block.closest('details.pf-shared-state-code')))).toBe(true);
  await expect(content).toContainText('몬스터의 행동 전환은 FSM이, 플레이어의 기능 조합은 플레이어 컨트롤러가 관리합니다.');
  await expect(content).toContainText('행동 규칙과 조정값을 분리');
  await expect(content).toContainText('더블 점프와 벽 점프');

  const coreText = await content.evaluate(element => {
    element = element.cloneNode(true);
    element.querySelectorAll('details').forEach(details => details.remove());
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
  for (const label of ['FSM 상태 전환', '공통 State와 플레이어 분기', '몬스터 상태 진입·종료', '공통 이동 로직', '플레이어 기능 조합', '토끼 형태의 벽 점프']) {
    const summary = page.getByText(`코드 보기: ${label}`, { exact: true });
    const details = summary.locator('..');
    await summary.click();
    await expect(details).toHaveAttribute('open', '');
    await expect(details.locator('pre')).toBeVisible();
    await summary.focus();
    await page.keyboard.press('Enter');
    await expect(details).not.toHaveAttribute('open');
    await page.keyboard.press('Space');
    await expect(details).toHaveAttribute('open', '');
    expect(await summary.evaluate(element => getComputedStyle(element).outlineWidth)).toBe('2px');
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
    if (label === '공통 State와 플레이어 분기' || label === '토끼 형태의 벽 점프') {
      await page.screenshot({ path: testInfo.outputPath(label === '토끼 형태의 벽 점프' ? 'rabbit-wall-jump-code-open.png' : 'key-state-code-open.png') });
    }
    await page.keyboard.press('Enter');
    await expect(details).not.toHaveAttribute('open');
  }
});
