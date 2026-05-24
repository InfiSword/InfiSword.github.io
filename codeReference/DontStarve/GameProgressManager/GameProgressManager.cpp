#include "99_Default/pch.h"
#include "GameProgressManager.h"
#include <iomanip>
#include <ctime>
#include <fstream>

GameProgressManager::GameProgressManager()
	: m_saveFilePath(L"GameData/game_progress.txt")
{
}

GameProgressManager::~GameProgressManager()
{
	Release();
}

void GameProgressManager::Init()
{
	// 저장 파일 로드
	LoadFromFile(m_saveFilePath);
}

void GameProgressManager::Update(float deltaTime)
{
	m_totalGameTime += deltaTime;
}

void GameProgressManager::Release()
{
	// 캐릭터 해금 정보 저장
	SaveToFile(m_saveFilePath);
}

// ====================== 씬 클리어 관련 (런타임) =======================

bool GameProgressManager::IsSceneCleared(SceneType sceneType) const
{
	return m_gameProgress.IsSceneCleared(sceneType);
}

void GameProgressManager::ClearScene(SceneType sceneType)
{
	// 런타임 씬 클리어 처리
	m_gameProgress.ClearScene(sceneType);
	
	// 저장
	SaveToFile(m_saveFilePath);
	
	std::wstring msg = L"GameProgressManager: 씬 클리어 (런타임) - SceneType: " + std::to_wstring((int)sceneType) + L"\n";
	OutputDebugStringW(msg.c_str());
}

// ====================== 캐릭터 해금 관련 (영구 저장) =======================

bool GameProgressManager::IsCharacterUnlocked(GameObjectID characterID) const
{
	return m_gameProgress.IsCharacterUnlocked(characterID);
}

void GameProgressManager::UpdateCharacterUnlocks()
{
	m_gameProgress.UpdateCharacterUnlocks();
	
	// 저장
	SaveToFile(m_saveFilePath);
}

void GameProgressManager::SavePlayerState(const PlayerStateSnapshot& snapshot)
{
	m_playerSnapshot = snapshot;
	m_hasSavedPlayerState = true;
}

// ====================== 저장/로드 =======================

void GameProgressManager::SaveToFile(const std::wstring& filePath)
{
	// 저장 디렉터리가 없으면 생성
	size_t lastSlash = filePath.find_last_of(L"\\/");
	if (lastSlash != std::wstring::npos) {
		std::wstring dir = filePath.substr(0, lastSlash);
		if (!dir.empty()) {
			CreateDirectoryW(dir.c_str(), nullptr);
		}
	}

	std::wofstream file(filePath);
	if (!file.is_open())
	{
		return;
	}
	
	// 캐릭터 해금 정보 저장
	for (const auto& charInfo : m_gameProgress.characterUnlockInfos)
	{
		file << L"CHARACTER:" << (int)charInfo.characterID
			 << L",UNLOCKED:" << (charInfo.isUnlocked ? 1 : 0) << L"\n";
	}
	
	file.close();
	
	OutputDebugStringW(L"GameProgressManager: 캐릭터 해금 정보 저장 완료\n");
}

void GameProgressManager::LoadFromFile(const std::wstring& filePath)
{
	std::wifstream file(filePath);
	if (!file.is_open())
	{
		return;
	}
	
	std::wstring line;
	while (std::getline(file, line))
	{
		// 빈 줄이나 주석은 건너뛰기
		if (line.empty() || line[0] == L'#' || line[0] == L'[')
			continue;
		
		// 캐릭터 정보 파싱
		if (line.find(L"CHARACTER:") == 0)
		{
			ParseCharacterLine(line);
		}
	}
	
	file.close();
	
	OutputDebugStringW(L"GameProgressManager: 캐릭터 해금 정보 로드 완료\n");
}

void GameProgressManager::ParseCharacterLine(const std::wstring& line)
{
	// 형식: CHARACTER:1002,UNLOCKED:1
	size_t charPos = line.find(L"CHARACTER:") + 10;
	size_t unlockedPos = line.find(L",UNLOCKED:") + 10;
	
	try {
		int characterID = std::stoi(line.substr(charPos, line.find(L',', charPos) - charPos));
		int unlocked = std::stoi(line.substr(unlockedPos));
		
		// 해당 캐릭터 정보 업데이트
		for (auto& charInfo : m_gameProgress.characterUnlockInfos)
		{
			if ((int)charInfo.characterID == characterID)
			{
				// 저장된 값이 해금 상태인 경우에만 업데이트 (해금 기록 유지)
				if (unlocked == 1)
					charInfo.isUnlocked = true;
				break;
			}
		}
	} catch (...) {
		OutputDebugStringW(L"GameProgressManager: 캐릭터 라인 파싱 에러\n");
	}
}

void GameProgressManager::ResetRuntimeData()
{
	// 런타임 씬 클리어 정보 초기화 (Farming Area만 true)
	for (auto& sceneInfo : m_gameProgress.sceneClearInfos) {
		if (sceneInfo.sceneType == SCENE_GAME_FARMING_AREA) {
			sceneInfo.isCleared = true;
		} else {
			sceneInfo.isCleared = false;
		}
	}

	// 플레이어 상태 저장 초기화
	m_hasSavedPlayerState = false;
	m_playerSnapshot = PlayerStateSnapshot();

	m_totalGameTime = 0.0f;
}

void GameProgressManager::ResetAllProgress()
{
	// 모든 데이터 초기 상태로 리셋
	m_gameProgress = GameProgress();
	
	// 플레이어 상태 초기화
	m_hasSavedPlayerState = false;
	m_playerSnapshot = PlayerStateSnapshot();
	m_totalGameTime = 0.0f;

	// 초기화된 상태를 파일에 즉시 저장
	SaveToFile(m_saveFilePath);
}
