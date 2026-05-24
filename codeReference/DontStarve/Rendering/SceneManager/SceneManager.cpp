#include "99_Default/pch.h"
#include "SceneManager.h"
#include "TitleScene.h"
#include "CharacterSelectScene.h"
#include "LoadingScene.h"
#include "GameScene.h"
#include "ForestScene.h"
#include "BossHoundScene.h"
#include "BossSpiderQueenScene.h"
#include "../SoundManager/SoundManager.h"
#include "../GameProgressManager/GameProgressManager.h"
#include "../ObjectManager/ObjectManager.h"
#include "../ResourceManager/ResourceManager.h"
#include "../DataManager/DataManager.h"
#include "../../02_GameObject/Entity/Player/Player.h"

SceneManager::SceneManager()
	: m_currentScene(nullptr)
	, m_reservedSceneType(SCENE_NONE)
	, m_reservedCharacterID(GOID_NONE)
	, m_targetSceneType(SCENE_NONE)
	, m_targetCharacterID(GOID_NONE)
	, m_currentMapData(nullptr)
{
}

SceneManager::~SceneManager()
{
	Release();
}

void SceneManager::Init()
{
	// 모든 맵 데이터 로드 (게임 시작 시 한 번만 수행)
	LoadAllMapData();
	LoadTitleScene();
}

void SceneManager::Update(float deltaTime)
{
	// 프레임 시작 시 예약된 씬이 있다면 교체 
	ChangeSceneIfReserved();

	if (m_currentScene) {
		m_currentScene->Update(deltaTime);
	}
}

void SceneManager::LateUpdate()
{
	if (m_currentScene) {
		m_currentScene->LateUpdate();
	}
}

void SceneManager::Render()
{
	if (m_currentScene) {
		m_currentScene->Render();
	}
}

void SceneManager::Release()
{
	if (m_currentScene) {
		// 각 Scene 소멸자에서 Release를 수행하므로 여기서는 delete만 수행
		delete m_currentScene;
		m_currentScene = nullptr;
	}
	
	// m_mapDataStorage 정리
	for (auto& pair : m_mapDataStorage) {
		pair.second.gameObjects.clear();
		pair.second.gameObjects.shrink_to_fit();
		pair.second.mapName.clear();
		pair.second.mapFilePath.clear();
	}
	m_mapDataStorage.clear();
	
	m_currentMapData = nullptr;
}

void SceneManager::LoadTitleScene()
{
	m_reservedSceneType = SCENE_TITLE;
}

void SceneManager::LoadCharacterSelectScene()
{
	m_reservedSceneType = SCENE_CHARACTER_SELECT;
}

void SceneManager::LoadGameScene(SceneType sceneType, GameObjectID selectedCharacterID)
{
	// 현재 플레이어 상태 저장
	ObjectManager* objMgr = ObjectManager::GetInstance();
	Player* currentPlayer = objMgr->GetPlayer();
	if (currentPlayer) {
		GameProgressManager::GetInstance()->SavePlayerState(currentPlayer->SaveState());
	}

	// 로딩 씬을 먼저 예약하고, 실제 목적지를 저장해둠
	m_targetSceneType = sceneType;
	m_targetCharacterID = selectedCharacterID;

	m_reservedSceneType = SCENE_LOADING;
	m_reservedCharacterID = GOID_NONE; // 로딩 씬 자체에는 캐릭터 ID가 필요 없음
}

void SceneManager::FinishLoading()
{
	// 로딩이 끝나면 실제로 예약해둔 타겟 씬으로 전환 예약
	m_reservedSceneType = m_targetSceneType;
	m_reservedCharacterID = m_targetCharacterID;

	m_targetSceneType = SCENE_NONE;
	m_targetCharacterID = GOID_NONE;
}

void SceneManager::ChangeSceneIfReserved()
{
	if (m_reservedSceneType == SCENE_NONE) return;

	OutputDebugStringW(L"SceneManager: 예약된 씬으로 안전하게 교체 수행\n");

	// 기존 씬 정리 (이 시점에 ObjectManager::Release가 호출됨)
	if (m_currentScene) {
		delete m_currentScene;
		m_currentScene = nullptr;
	}

	// 타이틀로 돌아가는 경우 런타임 데이터 초기화
	if (m_reservedSceneType == SCENE_TITLE) {
		GameProgressManager::GetInstance()->ResetRuntimeData();
	}

	// 새 씬 인스턴스 생성
	switch (m_reservedSceneType)
	{
	case SCENE_TITLE:
		m_currentScene = new TitleScene();
		break;
	case SCENE_CHARACTER_SELECT:
		m_currentScene = new CharacterSelectScene();
		break;
	case SCENE_LOADING:
		m_currentScene = new LoadingScene();
		break;
	case SCENE_GAME_HOUND_FOREST:
		m_currentScene = new BossHoundScene();
		break;
	case SCENE_GAME_SPIDER_QUEEN_HOUSE:
		m_currentScene = new BossSpiderQueenScene();
		break;
	case SCENE_GAME_FARMING_AREA:
		m_currentScene = new ForestScene();
		break;
	default:
		m_currentScene = nullptr;
		break;
	}

	if (m_currentScene) {
		// BGM 재생
		SoundManager* soundMgr = SoundManager::GetInstance();
		if (m_reservedSceneType == SCENE_TITLE || m_reservedSceneType == SCENE_CHARACTER_SELECT)
		{
			soundMgr->PlayBGM(L"Resource/Sound/Title_Theme.wav");
		}
		else if (m_reservedSceneType == SCENE_GAME_HOUND_FOREST || m_reservedSceneType == SCENE_GAME_SPIDER_QUEEN_HOUSE)
		{
			soundMgr->PlayBGM(L"Resource/Sound/Boss_BGM.wav");
		}
		else if (m_reservedSceneType == SCENE_LOADING)
		{
			soundMgr->StopBGM();
		}
		else
		{			
			if (m_reservedSceneType == SCENE_GAME_FARMING_AREA)
				soundMgr->PlayBGM(L"Resource/Sound/Forest_Scene_BGM.wav");
			else
				soundMgr->StopBGM();
		}

		// 게임 씬인 경우 캐릭터 ID 설정
		if (m_reservedSceneType >= SCENE_GAME_FARMING_AREA && m_reservedSceneType <= SCENE_GAME_SPIDER_QUEEN_HOUSE) {
			GameScene* gameScene = static_cast<GameScene*>(m_currentScene);
			gameScene->SetSelectedCharacterID(m_reservedCharacterID);
		}

		// 맵 데이터 설정
		auto it = m_mapDataStorage.find(m_reservedSceneType);
		if (it != m_mapDataStorage.end()) {
			m_currentMapData = &it->second;
		} else {
			m_currentMapData = nullptr;
		}

		// 초기화 호출 (여기서 ObjectManager::Init 및 UI 생성이 수행됨)
		m_currentScene->Init(m_currentMapData);
	}

	// 예약 정보 초기화
	m_reservedSceneType = SCENE_NONE;
	m_reservedCharacterID = GOID_NONE;
}

SceneType SceneManager::GetCurrentSceneType() const
{
	if (!m_currentScene) return SCENE_NONE;
	return m_currentScene->GetSceneType();
}

void SceneManager::LoadAllMapData()
{
	// 게임 시작 시 모든 맵 데이터 로드
	for (const auto& entry : DataTable::SceneTypeTable) {
		MapData mapData;
		if (SceneManager::ParseMapFile(entry.path, mapData)) {
			m_mapDataStorage[entry.value] = std::move(mapData);
		}
	}
	OutputDebugStringW(L"SceneManager: 모든 맵 데이터 로드 완료\n");
}

bool SceneManager::ParseMapFile(const std::wstring& mapFileName, MapData& outMapData)
{
	auto getObjectResourceInfo = [](GameObjectID id) -> const ResourcePathUtils::ObjectResourceDef* {
		return DataManager::GetInstance()->GetObjectResourceInfo(id);
	};
	return ResourcePathUtils::ParseMapFileInto(mapFileName, outMapData, getObjectResourceInfo);
}
