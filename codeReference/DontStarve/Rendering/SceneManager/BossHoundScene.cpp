#include "99_Default/pch.h"
#include "BossHoundScene.h"
#include "../ObjectManager/ObjectManager.h"
#include "../CameraManager/CameraManager.h"
#include "../../02_GameObject/Entity/Player/Player.h"
#include "../../02_GameObject/Entity/Entity.h"
#include "../../02_GameObject/Entity/Monster/Monster.h"
#include "../../02_GameObject/Entity/Monster/Hound.h"
#include "../../02_GameObject/Component/Transform/Transform.h"
#include "../GameProgressManager/GameProgressManager.h"
#include "../SceneManager/SceneManager.h"
#include "../../02_GameObject/UI/UIText.h"
#include "../../02_GameObject/UI/GameClearUI.h"
#include "../../02_GameObject/UI/HPUI.h"

BossHoundScene::BossHoundScene()
	: GameScene()
	, m_currentPhase(BossPhase::Phase1_Hounds)
	, m_phaseTimer(0.0f)
	, m_isIntroRunning(false)
	, m_introTimer(0.0f)
	, m_introTargetBossIndex(0)
	, m_introStep(IntroStep::MoveToBoss)
	, m_startedHowl(false)
	, m_bossesActivated(false)
	, m_isClearUIShown(false)
	, m_gameClearUI(nullptr)
	, m_iceBossHPUI(nullptr)
	, m_redBossHPUI(nullptr)
{
}

BossHoundScene::~BossHoundScene()
{
}

void BossHoundScene::Init(const MapData* mapData)
{
	// 부모 클래스의 Init(mapData) 호출하여 맵 데이터 기반 오브젝트들 생성
	GameScene::Init(mapData);

	// 부모 클래스 초기화 후 상태값 재설정 (Init() 호출 시 덮어써지는 것 방지)
	m_currentPhase = BossPhase::Phase1_Hounds;
	m_phaseTimer = 0.0f;
	m_bossesActivated = false;
	m_isIntroRunning = false;
	m_isClearUIShown = false;
	m_introTargetBossIndex = 0;
	m_introStep = IntroStep::MoveToBoss;
	m_startedHowl = false;
	m_bossObjects.clear();
	m_gameClearUI = nullptr;
	m_iceBossHPUI = nullptr;
	m_redBossHPUI = nullptr;

	ObjectManager* objMgr = ObjectManager::GetInstance();
	// 클리어 UI 생성
	m_gameClearUI = objMgr->CreateGameClearUI();
	if (m_gameClearUI) m_gameClearUI->SetType(GameClearUIType::NoButtons);

	// 맵 데이터에서 생성된 보스들을 찾아 비활성화 (전투 준비 단계)
	// 연출 순서 보장을 위해 순서대로 찾음 (Ice -> Red)
	const auto& objects = objMgr->GetWorldObjects();

	GameObject* iceBoss = nullptr;
	GameObject* redBoss = nullptr;

	for (auto* obj : objects)
	{
		if (!obj) continue;
		if (obj->GetID() == GOID_MONSTER_ICEHOUNDDOG) iceBoss = obj;
		else if (obj->GetID() == GOID_MONSTER_REDHOUNDDOG) redBoss = obj;
	}

	if (iceBoss) { iceBoss->SetActive(false); m_bossObjects.push_back(iceBoss); }
	if (redBoss) { redBoss->SetActive(false); m_bossObjects.push_back(redBoss); }

	// 보스 HP UI 미리 생성 (Order: Ice -> Red)
	if (iceBoss)
	{
		m_iceBossHPUI = objMgr->CreateHPUI(dynamic_cast<Entity*>(iceBoss), L"얼음 하운드", 600.0f, 25.0f,
			Gdiplus::Color(200, 40, 0, 0), Gdiplus::Color(255, 200, 0, 0), Gdiplus::Color(255, 100, 150, 255),
			0.5f, 0.0f, 0.5f, 0.0f, 0.0f, 60.0f,
			1000.0f, 1002.0f, false, false);
		if (m_iceBossHPUI) m_iceBossHPUI->SetActive(false);
	}

	if (redBoss)
	{
		m_redBossHPUI = objMgr->CreateHPUI(dynamic_cast<Entity*>(redBoss), L"레드 하운드", 600.0f, 25.0f,
			Gdiplus::Color(200, 40, 0, 0), Gdiplus::Color(255, 200, 0, 0), Gdiplus::Color(255, 255, 100, 100),
			0.5f, 0.0f, 0.5f, 0.0f, 0.0f, 150.0f,
			1000.0f, 1002.0f, false, false);
		if (m_redBossHPUI) m_redBossHPUI->SetActive(false);
	}


	OutputDebugStringW((L"BossHoundScene: Initialized with MapData. Bosses hidden: " + std::to_wstring(m_bossObjects.size()) + L"\n").c_str());
}

void BossHoundScene::Update(float deltaTime)
{
	// 페이즈에 따라 게임 로직 업데이트 순서 조정 가능
	GameScene::Update(deltaTime);

	switch (m_currentPhase)
	{
	case BossPhase::Phase1_Hounds:
		UpdatePhase1(deltaTime);
		break;
	case BossPhase::PhaseTransition:
		UpdatePhaseTransition(deltaTime);
		break;
	case BossPhase::Phase2_BossIntro:
		UpdatePhase2Intro(deltaTime);
		break;
	case BossPhase::Phase2_BossBattle:
		UpdatePhase2Battle(deltaTime);
		break;
	case BossPhase::Cleared:
		UpdateCleared(deltaTime);
		break;
	}
}

void BossHoundScene::Render()
{
	GameScene::Render();
}

void BossHoundScene::UpdatePhase1(float deltaTime)
{
	// 필드에 있는 모든 일반 하운드(minion)가 죽었는지 체크
	ObjectManager* objMgr = ObjectManager::GetInstance();
	const auto& objects = objMgr->GetWorldObjects();

	bool houndsAlive = false;
	for (auto* obj : objects)
	{
		// 보스가 아닌 일반 하운드만 체크
		if (obj && obj->GetID() == GOID_MONSTER_HOUNDDOG && obj->IsEnabled())
		{
			houndsAlive = true;
			break;
		}
	}

	if (!houndsAlive)
	{
		m_currentPhase = BossPhase::PhaseTransition;
		m_phaseTimer = 0.0f;
		OutputDebugStringW(L"BossHoundScene: Phase 1 Cleared. Transitioning to Phase 2...\n");
	}
}

void BossHoundScene::UpdatePhaseTransition(float deltaTime)
{
	m_phaseTimer += deltaTime;

	// 2초 정도 대기 후 보스 등장 연출 시작
	if (m_phaseTimer >= 2.0f)
	{
		StartBossIntro();
	}
}

void BossHoundScene::StartBossIntro()
{
	m_currentPhase = BossPhase::Phase2_BossIntro;
	m_introTimer = 0.0f;
	m_isIntroRunning = true;
	m_introTargetBossIndex = 0;
	m_introStep = IntroStep::MoveToBoss;
	m_startedHowl = false;

	CameraManager* camMgr = CameraManager::GetInstance();
	camMgr->SetFollowMode(false); // 수동 카메라 제어
	m_introStartPos = camMgr->GetCameraPos();

	// 플레이어 입력 비활성화
	Player* player = ObjectManager::GetInstance()->GetPlayer();
	if (player) player->SetInputEnabled(false);

	if (!m_bossObjects.empty())
	{
		for (auto* bossObj : m_bossObjects)
		{
			Hound* pBossHound = dynamic_cast<Hound*>(bossObj);
			if (pBossHound) pBossHound->SetCombatEnabled(false);

			Monster* pMonster = dynamic_cast<Monster*>(bossObj);
			if (pMonster) pMonster->SetCanChase(false);
		}

		Transform* tr = m_bossObjects[0]->GetComponent<Transform>();
		if (tr) m_introTargetPos = { tr->GetX(), tr->GetY() };
	}
	else
	{
		m_introTargetPos = m_introStartPos;
	}

	OutputDebugStringW(L"BossHoundScene: Boss Intro Started. Player input disabled.\n");
}

void BossHoundScene::UpdatePhase2Intro(float deltaTime)
{
	CameraManager* camMgr = CameraManager::GetInstance();
	Player* player = ObjectManager::GetInstance()->GetPlayer();
	if (!player) return;

	int bossCount = (int)m_bossObjects.size();

	switch (m_introStep)
	{
	case IntroStep::MoveToBoss:
	{
		if (m_introTargetBossIndex >= bossCount)
		{
			m_introStep = IntroStep::ReturnToPlayer;
			m_introTimer = 0.0f;
			break;
		}

		m_introTimer += deltaTime;
		float moveDuration = 1.0f;
		
		GameObject* boss = m_bossObjects[m_introTargetBossIndex];
		Transform* bossTr = boss->GetComponent<Transform>();
		Gdiplus::PointF bossPos = { bossTr->GetX(), bossTr->GetY() };

		// 이전 보스 위치 또는 시작 위치에서 현재 보스 위치로 이동
		Gdiplus::PointF startPos = m_introStartPos;
		if (m_introTargetBossIndex > 0)
		{
			Transform* prevTr = m_bossObjects[m_introTargetBossIndex - 1]->GetComponent<Transform>();
			if (prevTr) startPos = { prevTr->GetX(), prevTr->GetY() };
		}

		float t = (std::min)(m_introTimer / moveDuration, 1.0f);
		float smoothT = t * t * (3 - 2 * t);
		float curX = startPos.X + (bossPos.X - startPos.X) * smoothT;
		float curY = startPos.Y + (bossPos.Y - startPos.Y) * smoothT;
		camMgr->SetCameraPos(curX, curY);

		if (t >= 1.0f)
		{
			boss->SetActive(true);
			m_introStep = IntroStep::WaitHowl;
			m_introTimer = 0.0f;
			m_startedHowl = false;
		}
		break;
	}
	case IntroStep::WaitHowl:
	{
		GameObject* boss = m_bossObjects[m_introTargetBossIndex];
		Hound* bossEnt = dynamic_cast<Hound*>(boss);
		if (!bossEnt) { m_introStep = IntroStep::WaitExtra; break; }

		if (bossEnt->HasHowlStarted() && !m_startedHowl) m_startedHowl = true;
		
		else if (m_startedHowl && bossEnt->HasHowlFinished())
		{
			m_introStep = IntroStep::WaitExtra;
			m_introTimer = 0.0f;
		}
		m_introTimer += deltaTime;
		break;
	}
	case IntroStep::WaitExtra:
	{
		m_introTimer += deltaTime;
		if (m_introTimer >= 0.5f)
		{
			m_introTargetBossIndex++;
			if (m_introTargetBossIndex < bossCount)
			{
				m_introStep = IntroStep::MoveToBoss;
			}
			else
			{
				m_introStep = IntroStep::ReturnToPlayer;
			}
			m_introTimer = 0.0f;
		}
		break;
	}
	case IntroStep::ReturnToPlayer:
	{
		m_introTimer += deltaTime;
		float returnDuration = 1.5f;

		Transform* playerTr = player->GetComponent<Transform>();
		Gdiplus::PointF playerPos = { playerTr->GetX(), playerTr->GetY() };

		Gdiplus::PointF lastPos = m_introStartPos;
		if (!m_bossObjects.empty())
		{
			Transform* lastTr = m_bossObjects.back()->GetComponent<Transform>();
			if (lastTr) lastPos = { lastTr->GetX(), lastTr->GetY() };
		}

		float t = (std::min)(m_introTimer / returnDuration, 1.0f);
		float smoothT = t * t * (3 - 2 * t);
		float curX = lastPos.X + (playerPos.X - lastPos.X) * smoothT;
		float curY = lastPos.Y + (playerPos.Y - lastPos.Y) * smoothT;
		camMgr->SetCameraPos(curX, curY);

		if (t >= 1.0f)
		{
			// 모든 연출 종료
			m_bossesActivated = true;
			camMgr->SetFollowMode(true);
			m_currentPhase = BossPhase::Phase2_BossBattle;

			// 보스들에게 추격 허용
			for (auto* boss : m_bossObjects)
			{
				Hound* pBossHound = dynamic_cast<Hound*>(boss);
				if (pBossHound) pBossHound->SetCombatEnabled(true);

				Monster* pMonster = dynamic_cast<Monster*>(boss);
				if (pMonster) pMonster->SetCanChase(true);
			}

			// 보스 HP UI 활성화
			if (m_iceBossHPUI) m_iceBossHPUI->SetActive(true);
			if (m_redBossHPUI) m_redBossHPUI->SetActive(true);

			// 플레이어 입력 재활성화
			player->SetInputEnabled(true);
		}
		break;
	}
	}
}

bool BossHoundScene::IsIntroReturning() const
{
	return (m_currentPhase == BossPhase::Phase2_BossIntro && m_introStep == IntroStep::ReturnToPlayer);
}

void BossHoundScene::SpawnBoss()
{
	if (m_bossesActivated) return;

	for (auto* boss : m_bossObjects)
	{
		if (boss) boss->SetActive(true);
	}
	m_bossesActivated = true;
	OutputDebugStringW(L"BossHoundScene: Bosses Spawned and Activated!\n");
}

void BossHoundScene::UpdatePhase2Battle(float deltaTime)
{
	// 기존 보스 생존 체크 로직 유지
	ObjectManager* objMgr = ObjectManager::GetInstance();
	const auto& objects = objMgr->GetWorldObjects();
	bool bossesAlive = false;

	bool iceAlive = false;
	bool redAlive = false;

	for (auto* obj : objects)
	{
		if (obj && obj->IsEnabled())
		{
			if (obj->GetID() == GOID_MONSTER_ICEHOUNDDOG) iceAlive = true;
			else if (obj->GetID() == GOID_MONSTER_REDHOUNDDOG) redAlive = true;
		}
	}

	if (iceAlive || redAlive) bossesAlive = true;

	// 죽은 보스의 HP UI 숨기기
	if (!iceAlive && m_iceBossHPUI) m_iceBossHPUI->SetActive(false);
	if (!redAlive && m_redBossHPUI) m_redBossHPUI->SetActive(false);

	if (!bossesAlive)
	{
		m_currentPhase = BossPhase::Cleared;
		m_phaseTimer = 0.0f;
		m_isClearUIShown = false;

		if (m_iceBossHPUI) m_iceBossHPUI->SetActive(false);
		if (m_redBossHPUI) m_redBossHPUI->SetActive(false);

		OutputDebugStringW(L"BossHoundScene: All Bosses Defeated! Scene Cleared.\n");

		GameProgressManager::GetInstance()->ClearScene(SCENE_GAME_HOUND_FOREST);
	}
}

void BossHoundScene::UpdateCleared(float deltaTime)
{
	m_phaseTimer += deltaTime;

	// 1초 뒤에 클리어 UI 표시
	if (m_phaseTimer >= 1.0f && !m_isClearUIShown)
	{
		if (m_gameClearUI) m_gameClearUI->Show();

		m_isClearUIShown = true;
		OutputDebugStringW(L"BossHoundScene: Clear UI displayed.\n");
	}

	// 클리어 화면 뜬지 3초(총 4초) 뒤에 씬 이동
	if (m_phaseTimer >= 4.0f)
	{
		OutputDebugStringW(L"BossHoundScene: Transitioning back to Farming Area...\n");
		SceneManager::GetInstance()->LoadGameScene(SCENE_GAME_FARMING_AREA, GetSelectedCharacterID());
	}
}

void BossHoundScene::Release()
{
	m_gameClearUI = nullptr;
	m_iceBossHPUI = nullptr;
	m_redBossHPUI = nullptr;
	m_bossObjects.clear();

	GameScene::Release();
}
