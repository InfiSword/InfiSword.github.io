#include "99_Default/pch.h"
#include "GameScene.h"
#include "../ObjectManager/ObjectManager.h"
#include "../CameraManager/CameraManager.h"
#include "../ColliderManager/ColliderManager.h"
#include "../../02_GameObject/Entity/Player/Player.h"
#include "../../02_GameObject/Component/Transform/Transform.h"
#include "../../02_GameObject/UI/MenuUI.h"
#include "../../02_GameObject/UI/HPUI.h"
#include "../../02_GameObject/UI/GameOverUI.h"

GameScene::GameScene()
	: m_mapData(nullptr)
	, m_selectedCharacterID(GOID_NONE)
	, m_craftingUI(nullptr)
	, m_playerHPUI(nullptr)
	, m_gameOverUI(nullptr)
{
}

GameScene::~GameScene()
{
	Release();
}

void GameScene::Init(const MapData* mapData)
{
	// 1. 씬 상태 초기화 (카메라 리셋 및 이동 제한 영역 설정)
	ObjectManager* objectManager = ObjectManager::GetInstance();
	objectManager->Init();
	CameraManager::GetInstance()->Init();
	m_mapData = mapData;
	if (m_mapData) {
		CameraManager::GetInstance()->SetWalkableBoundsFromMapData(m_mapData);
	}

	// 2. 플레이어 및 월드 오브젝트 생성
	SpawnPlayer();
	CreateGameObjectsFromMapData();

	// 3. UI 생성 및 초기화
	SceneType currentSceneType = GetSceneType();
	bool isBossScene = (currentSceneType == SCENE_GAME_HOUND_FOREST ||
		currentSceneType == SCENE_GAME_SPIDER_QUEEN_HOUSE);

	if (!isBossScene) {
		if (!m_craftingUI) {
			m_craftingUI = objectManager->CreateMenuUI();
		}
	}

	if (!m_playerHPUI) {
		Player* player = objectManager->GetPlayer();
		m_playerHPUI = objectManager->CreateHPUI(player, L"", 220.f, 28.0f,
			Gdiplus::Color(255, 60, 0, 0), Gdiplus::Color(255, 255, 0, 0), Gdiplus::Color(255, 255, 255, 255),
			1.0f, 0.0f, 1.0f, 0.0f, -120.f, 34.0f,
			10.1f, 10.2f, true, true);
	}

	if (!m_gameOverUI) {
		m_gameOverUI = objectManager->CreateGameOverUI();
	}
}

void GameScene::Update(float deltaTime)
{
	ObjectManager::GetInstance()->Update(deltaTime);
	CameraManager::GetInstance()->Update(deltaTime);

	Player* player = ObjectManager::GetInstance()->GetPlayer();
	if (player && player->IsDead() && m_gameOverUI && !m_gameOverUI->IsEnabled())
	{
		m_gameOverUI->Show();
	}
}

void GameScene::LateUpdate()
{
	ObjectManager::GetInstance()->LateUpdate();
	ColliderManager::GetInstance()->LateUpdate();
}

void GameScene::Render()
{
	// 1. 타일 렌더링
	CameraManager* cameraManager = CameraManager::GetInstance();
	if (cameraManager && m_mapData && m_mapData->mapWidth > 0 && m_mapData->mapHeight > 0) {
		cameraManager->RenderVisibleTiles(m_mapData);
	}

	// 2. 월드 오브젝트 및 UI 렌더링
	ObjectManager* objectManager = ObjectManager::GetInstance();
	if (objectManager) {
		objectManager->Render();
	}
}

void GameScene::Release()
{
	// UI 포인터 무효화 (실제 메모리는 ObjectManager가 관리)
	m_playerHPUI = nullptr;
	m_gameOverUI = nullptr;
	m_craftingUI = nullptr;

	ObjectManager::GetInstance()->Release();
	ColliderManager::GetInstance()->Release();
	CameraManager::GetInstance()->Release();

	m_mapData = nullptr;
}

void GameScene::CreateGameObjectsFromMapData()
{
	ObjectManager* objectManager = ObjectManager::GetInstance();
	if (!objectManager || !m_mapData) return;

	for (const ResourcePathUtils::ObjectResourceDef& objData : m_mapData->gameObjects)
	{
		GameObjectID id = objData.id;
		if (id >= 3000) {
			if (id == GOID_UI_MENU && !m_craftingUI)
			{
				m_craftingUI = objectManager->CreateMenuUI();
				continue;
			}
		}

		objectManager->CreateObject(id, objData.x, objData.y);
	}
}

void GameScene::SpawnPlayer()
{
	ObjectManager* objectManager = ObjectManager::GetInstance();
	if (!objectManager || !m_mapData) return;

	objectManager->CreateObject(m_selectedCharacterID, m_mapData->playerSpawn.x, m_mapData->playerSpawn.y);
	Player* cachedPlayer = objectManager->GetPlayer();

	if (cachedPlayer)
	{
		CameraManager* cameraManager = CameraManager::GetInstance();
		if (cameraManager) {
			cameraManager->SetTarget(cachedPlayer);
			cameraManager->SetFollowMode(true);
		}
	}
}
