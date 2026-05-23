layout: single

title: "PROJECT REPORT // SLIME PROJECT"

excerpt: "State Machine 기반 게임 로직 및 AI 설계"

categories: \[Project]

tags: \[FSM, Unity, AI, Design Pattern]

toc: true

toc\_sticky: true



Slime Project는 하드코어 액션 퍼즐 플랫포머 게임으로, 게임 내 모든 엔티티의 행동과 AI를 체계적으로 관리하기 위해 디자인 패턴을 적극적으로 도입했습니다.



1\. FiniteStateMachine (FSM) 기반 상태 관리



모든 몬스터와 보스, 게임 엔티티의 상태를 제어하는 공통 상태머신(FSM) 아키텍처를 설계했습니다.



public class FiniteStateMachine

{

&#x20;   public State currentState { get; private set; } 



&#x20;   public void Init(State startingState) {

&#x20;       currentState = startingState;

&#x20;       currentState.Enter();

&#x20;   }



&#x20;   public void ChangeState(State newState) {

&#x20;       currentState.Exit();

&#x20;       currentState = newState;

&#x20;       currentState.Enter();

&#x20;   }

}





상태(State) 베이스 클래스 설계



상태머신에 들어가는 모든 객체는 State 클래스를 상속받습니다.



Enter() / Exit(): 애니메이션 파라미터 제어 및 변수 초기화



LogicUpdate() / PhysicsUpdate(): 프레임(Update) 및 물리(FixedUpdate) 연산



DoChecks(): 상태 전환의 조건(예: 절벽 감지, 플레이어 어그로 감지) 검사 로직 분리



2\. 플레이어 슬라임 (컴포지션 패턴)



방대해질 수 있는 플레이어 코드를 Player\_Slime 메인 클래스 하나에 몰아넣지 않고, 컴포지션(Composition) 패턴을 통해 기능별로 쪼개어 관리합니다.



Player\_SlimeMovement: 키보드 입력, 기본 이동, 점프 처리



Player\_Attack: 공격 타격 및 데미지 로직



Player\_Dash: 무적 프레임 및 순간 이동 계산



이를 통해 특정 적을 흡수했을 때 변신(예: 토끼 폼)하는 로직을 쉽게 구현했습니다. 토끼 폼 전용 이동 클래스(Player\_FormMovement)를 만들어 더블 점프, 벽 타기 등 능력을 오버라이딩하여 연동합니다.



3\. 보스 몬스터 AI (델리게이트 시스템)



보스의 복잡한 스킬 패턴 결정을 if-else문이나 switch문이 아닌 델리게이트 배열(Delegate Array)을 활용해 깔끔하게 관리합니다.



private delegate void BossBear\_SkillAction();

private BossBear\_SkillAction\[] BossBear\_Skills;



public override void Start()

{

&#x20;   // 스킬 목록을 배열에 할당

&#x20;   BossBear\_Skills = new BossBear\_SkillAction\[4] { Rest, Smash, Dash, Rumbling };

}



private void RandomBossBearSkillState()

{

&#x20;   int RandomSkill\_Index = Random.Range((int)SkillEnum.Smash, (int)SkillEnum.Skill\_Max);

&#x20;   

&#x20;   // 배열에서 선택된 스킬 메서드 직접 호출

&#x20;   BossBear\_Skills\[RandomSkill\_Index].Invoke();

}





💡 HP 기반 동적 난이도 조절

보스 체력이 50% 이하가 되면 IsSkillUpgrade = true 상태가 되며, 호출되는 모든 스킬이 강력한 업그레이드 버전으로 발동되도록 설계했습니다.

{: .notice--warning }



4\. 일반 몬스터 AI 확장



FSM을 바탕으로 일반 몬스터의 공통 기반(Move, Idle, Detect, Melee)을 만들어두고, 몬스터의 특성에 따라 파생 클래스에서 특수 상태만 추가하여 구현 효율을 높였습니다.



Bear (곰): DashState를 추가해 강력한 돌진 구현



Spider (거미): JumpState를 활용하여 원거리 공격 및 벽 타기 구현

