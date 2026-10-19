const { test, expect } = require('@playwright/test');

test.beforeEach(async ({ page }) => {
  await page.goto('/project/slime-project/');
});

test('공통 State와 두 분기를 실제 코드 보기로 설명한다', async ({ page }) => {
  await expect(page.locator('.project-content')).toContainText('공통 기반 클래스를 공유하되 상태 인스턴스는 각 객체가 따로 관리합니다.');
  await expect(page.locator('.project-content')).not.toContainText('Enemy_State');

  const examples = [
    ['코드 보기: State.cs', 'public class State'],
    ['코드 보기: Player_State.cs', 'public class Player_State : State'],
    ['코드 보기: Monster_State.cs - 몬스터 공통 기반', 'public class Monster_State : State'],
    ['코드 보기: Bear_DashState.cs - 몬스터별 확장', 'public class Bear_DashState : DashState'],
  ];
  for (const [label, declaration] of examples) {
    const summary = page.locator('summary').filter({ hasText: label }).filter({ hasText: new RegExp(`^${label}$`) });
    const details = summary.locator('..');
    await expect(details).not.toHaveAttribute('open');
    await expect(details.locator('code')).toContainText(declaration);
  }

  const brokenLinks = await page.locator('.toc-list a').evaluateAll(links =>
    links.map(link => decodeURIComponent(link.hash.slice(1))).filter(id => !document.getElementById(id))
  );
  expect(brokenLinks).toEqual([]);
  await expect(page.locator('[id="42-일반-몬스터-ai-확장-사례"]')).toBeAttached();
});

test('공유 상태 코드가 포인터와 키보드로 펼쳐지고 접힌다', async ({ page }, testInfo) => {
  for (const label of ['코드 보기: State.cs', '코드 보기: Player_State.cs', '코드 보기: Monster_State.cs - 몬스터 공통 기반', '코드 보기: Bear_DashState.cs - 몬스터별 확장']) {
    const summary = page.getByText(label, { exact: true });
    const details = summary.locator('..');
    await summary.click();
    await expect(details).toHaveAttribute('open', '');
    await expect(details.locator('pre')).toBeVisible();

    await summary.focus();
    await page.keyboard.press('Enter');
    await expect(details).not.toHaveAttribute('open');
    await expect(details.locator('pre')).toBeHidden();
    await page.keyboard.press('Space');
    await expect(details).toHaveAttribute('open', '');
    const focus = await summary.evaluate(element => ({
      visible: element.matches(':focus-visible'),
      outline: getComputedStyle(element).outlineStyle,
      width: getComputedStyle(element).outlineWidth,
    }));
    expect(focus.visible).toBe(true);
    expect(focus.outline).toBe('solid');
    expect(focus.width).toBe('2px');
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
    if (label === '코드 보기: Player_State.cs' || label === '코드 보기: Monster_State.cs - 몬스터 공통 기반') {
      await summary.scrollIntoViewIfNeeded();
      await page.screenshot({ path: testInfo.outputPath(label.includes('Player') ? 'shared-player.png' : 'shared-monster.png') });
    }
    await page.keyboard.press('Enter');
    await expect(details).not.toHaveAttribute('open');
  }
});
