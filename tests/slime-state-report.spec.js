const { test, expect } = require('@playwright/test');

test.beforeEach(async ({ page }) => {
  await page.goto('/project/slime-project/');
});

test('공통 State와 두 분기를 실제 코드 보기로 설명한다', async ({ page }) => {
  await expect(page.locator('.project-content')).toContainText('공통 기반 클래스를 공유하되 상태 인스턴스는 각 객체가 따로 관리합니다.');
  await expect(page.locator('.project-content')).not.toContainText('Enemy_State');
  await expect(page.locator('h2.chapter-title')).toHaveText([
    '1. 프로젝트 개요',
    '2. FiniteStateMachine 기반 상태 관리 시스템',
    '3. 플레이어 슬라임 및 변신 시스템',
  ]);
  await expect(page.locator('.project-content')).not.toContainText('MainBossBear');

  const examples = [
    ['코드 보기: State.cs', 'public class State'],
    ['코드 보기: Player_State.cs', 'public class Player_State : State'],
    ['코드 보기: Monster_State.cs - 몬스터 공통 기반', 'public class Monster_State : State'],
    ['코드 보기: Bear_DashState.cs - 몬스터별 확장', 'public class Bear_DashState : DashState'],
  ];
  for (const [label, declaration] of examples) {
    const summary = page.locator('summary').filter({ hasText: label }).filter({ hasText: new RegExp(`^${label}$`) });
    await expect(summary).toHaveCount(1);
    const details = summary.locator('..');
    await expect(details).not.toHaveAttribute('open');
    await expect(details.locator('code')).toContainText(declaration);
    const precedingSection = await summary.evaluate(element => [...document.querySelectorAll('h3')]
      .filter(heading => heading.compareDocumentPosition(element) & Node.DOCUMENT_POSITION_FOLLOWING)
      .at(-1)?.textContent);
    expect(precedingSection).toBe('2.2 State 베이스 클래스');
  }

  const brokenLinks = await page.locator('.toc-list a').evaluateAll(links =>
    links.map(link => decodeURIComponent(link.hash.slice(1))).filter(id => !document.getElementById(id))
  );
  expect(brokenLinks).toEqual([]);
  await expect(page.locator('[id="42-일반-몬스터-ai-확장-사례"]')).toHaveCount(0);
  await expect(page.locator('.toc-list a[href^="#4"]')).toHaveCount(0);
  await expect(page.getByRole('heading', { name: '2.3 초기화부터 상태 전환까지의 실행 흐름', exact: true })).toBeAttached();
  await expect(page.getByRole('heading', { name: '2.4 상태 데이터와 애니메이션 이벤트 연결', exact: true })).toBeAttached();
  const initialization = page.getByText('코드 보기: BearMonster.cs - 상태 생성과 최초 진입', { exact: true }).locator('..');
  await expect(initialization.locator('code')).toContainText('stateMachine.Init(moveState)');
  const priority = page.getByText('코드 보기: Bear_MoveState.cs - 상태 전환 우선순위', { exact: true }).locator('..');
  await expect(priority.locator('code')).toContainText('else if (isCheckJump)');
});

test('공유 상태 코드가 포인터와 키보드로 펼쳐지고 접힌다', async ({ page }, testInfo) => {
  for (const label of [
    '코드 보기: State.cs',
    '코드 보기: Player_State.cs',
    '코드 보기: Monster_State.cs - 몬스터 공통 기반',
    '코드 보기: Bear_DashState.cs - 몬스터별 확장',
    '코드 보기: BearMonster.cs - 상태 생성과 최초 진입',
    '코드 보기: MoveState.cs - 감지값 갱신',
    '코드 보기: Bear_MoveState.cs - 상태 전환 우선순위',
    '코드 보기: D_MoveState.cs - 이동 상태 설정',
    '코드 보기: AnimationToStateMachine.cs - 애니메이션 이벤트 전달',
  ]) {
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
  await page.getByRole('heading', { name: '2.3 초기화부터 상태 전환까지의 실행 흐름', exact: true }).scrollIntoViewIfNeeded();
  await page.screenshot({ path: testInfo.outputPath('fsm-walkthrough.png') });
});
