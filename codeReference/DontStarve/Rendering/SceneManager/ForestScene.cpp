#include "99_Default/pch.h"
#include "ForestScene.h"
#include "../ObjectManager/ObjectManager.h"

bool ForestScene::m_bIntroPlayed = false;

ForestScene::ForestScene()
    : GameScene()
{
}

ForestScene::~ForestScene()
{
}

void ForestScene::Init(const MapData* mapData)
{
    GameScene::Init(mapData);
    
	if (!m_bIntroPlayed)
		// ForestScene 전용 초기화 로직
	{
		introUI = ObjectManager::GetInstance()->CreateIntroNoticeUI();
		m_bIntroPlayed = true;
	}

    OutputDebugStringW(L"ForestScene: 초기화 완료\n");
}
