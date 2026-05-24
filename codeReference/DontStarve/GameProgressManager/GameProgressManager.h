#pragma once
#include "../../../Header/SingleTon.h"

// 전방 선언
enum SceneType;
enum GameObjectID : UINT;

// 플레이어 상태 스냅샷 (씬 전환 시 저장/복원용)
struct PlayerStateSnapshot
{
	int hp;
	int equippedSlotIndex;
	std::vector<std::pair<GameObjectID, UINT>> inventoryItems;

	PlayerStateSnapshot() : hp(100), equippedSlotIndex(-1) {}
};

// ====================== 게임 진행도 관련 구조체 =======================

// 씬(맵) 클리어 정보 구조체 (런타임용)
struct SceneClearInfo
{
	SceneType sceneType;
	bool isCleared;

	SceneClearInfo(SceneType type, bool cleared = false)
		: sceneType(type), isCleared(cleared) {}
};

// 캐릭터 해금 정보 구조체 (저장용)
struct CharacterUnlockInfo
{
	GameObjectID characterID;
	bool isUnlocked;
	SceneType requiredScene;  // 해금에 필요한 씬

	CharacterUnlockInfo(GameObjectID id, bool unlocked = false, SceneType scene = SCENE_NONE)
		: characterID(id), isUnlocked(unlocked), requiredScene(scene) {}
};

// 게임 전체 진행도 데이터 구조체
struct GameProgress
{
	std::vector<SceneClearInfo> sceneClearInfos;
	std::vector<CharacterUnlockInfo> characterUnlockInfos;

	GameProgress()
	{
		// 런타임 클리어 정보 초기화 (Farming Area만 기본 true)
		sceneClearInfos.emplace_back(SCENE_GAME_FARMING_AREA, true);
		sceneClearInfos.emplace_back(SCENE_GAME_HOUND_FOREST, false);
		sceneClearInfos.emplace_back(SCENE_GAME_SPIDER_QUEEN_HOUSE, false);

		// 기본 캐릭터 해금 정보
		characterUnlockInfos.emplace_back(GOID_PLAYER_WILSON, true, SCENE_NONE);
		characterUnlockInfos.emplace_back(GOID_PLAYER_WILLOW, false, SCENE_GAME_HOUND_FOREST);
		characterUnlockInfos.emplace_back(GOID_PLAYER_WOLFGANG, false, SCENE_GAME_SPIDER_QUEEN_HOUSE);
	}

	// 씬 클리어 여부 확인 (런타임)
	bool IsSceneCleared(SceneType sceneType) const
	{
		for (const auto& sceneInfo : sceneClearInfos)
			if (sceneInfo.sceneType == sceneType)
				return sceneInfo.isCleared;
		return false;
	}

	// 캐릭터 해금 여부 확인 (저장 데이터)
	bool IsCharacterUnlocked(GameObjectID characterID) const
	{
		for (const auto& charInfo : characterUnlockInfos)
			if (charInfo.characterID == characterID)
				return charInfo.isUnlocked;
		return false;
	}

	// 씬 클리어 처리 (런타임 상태 업데이트 및 캐릭터 해금 체크)
	void ClearScene(SceneType sceneType)
	{
		for (auto& sceneInfo : sceneClearInfos)
		{
			if (sceneInfo.sceneType == sceneType)
			{
				sceneInfo.isCleared = true;
				UpdateCharacterUnlocks(); // 캐릭터 자동 해금 체크
				break;
			}
		}
	}

	// 캐릭터 해금 업데이트 (한 번 해금되면 유지)
	void UpdateCharacterUnlocks()
	{
		for (auto& charInfo : characterUnlockInfos)
		{
			if (charInfo.requiredScene != SCENE_NONE && IsSceneCleared(charInfo.requiredScene))
				charInfo.isUnlocked = true;
		}
	}
};

// ====================== GameProgressManager 클래스 =======================

class GameProgressManager : public CSingleTon<GameProgressManager>
{
	friend class CSingleTon<GameProgressManager>;
private:
	GameProgressManager();
	~GameProgressManager();

public:
	// 생명주기 메서드
	void Init();
	void Update(float deltaTime);
	void LateUpdate() {}
	void Render() {}
	void Release();

	// 게임 시간 관련
	float GetTotalGameTime() const { return m_totalGameTime; }
	void SetTotalGameTime(float time) { m_totalGameTime = time; }

	// 씬 클리어 관련 (런타임)
	bool IsSceneCleared(SceneType sceneType) const;
	void ClearScene(SceneType sceneType);

	// 캐릭터 해금 관련 (영구 저장)
	bool IsCharacterUnlocked(GameObjectID characterID) const;
	void UpdateCharacterUnlocks();

	// 저장/로드 (텍스트 파일 형식, 캐릭터 해금 정보만 저장)
	void SaveToFile(const std::wstring& filePath);
	void LoadFromFile(const std::wstring& filePath);

	// 플레이어 상태 저장/복원 관련 (씬 전환용)
	void SavePlayerState(const PlayerStateSnapshot& snapshot);
	const PlayerStateSnapshot& GetPlayerState() const { return m_playerSnapshot; }
	bool HasSavedPlayerState() const { return m_hasSavedPlayerState; }
	void ClearSavedPlayerState() { m_hasSavedPlayerState = false; }

	// 런타임 게임 데이터 초기화 (타이틀로 복귀 시)
	void ResetRuntimeData();

	// 모든 게임 진행 상황 초기화 (저장된 캐릭터 해금 포함)
	void ResetAllProgress();

private:
	GameProgress m_gameProgress;  // 게임 진행도 데이터

	// 플레이어 상태 저장
	PlayerStateSnapshot m_playerSnapshot;
	bool m_hasSavedPlayerState = false;

	std::wstring m_saveFilePath;  // 저장 파일 경로

	float m_totalGameTime = 0.0f; // 전체 게임 시간

	// 내부 헬퍼 함수
	void ParseCharacterLine(const std::wstring& line);
};
