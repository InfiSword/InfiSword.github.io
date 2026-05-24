#include "99_Default/pch.h"
#include "LoadingScene.h"
#include "SceneManager.h"
#include "../ObjectManager/ObjectManager.h"
#include "../../02_GameObject/UI/UIImage.h"
#include "../../02_GameObject/UI/UIText.h"

LoadingScene::LoadingScene()
	: m_loadingTime(0.0f)
	, m_bFinished(false)
{
}

LoadingScene::~LoadingScene()
{
	Release();
}

void LoadingScene::Init(const MapData* mapData)
{
	ObjectManager* objectManager = ObjectManager::GetInstance();
	objectManager->Init();

	// 배경 이미지 (Loading_BG.png) - 화면 전체 채우기
	objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		static_cast<float>(WINCX),
		static_cast<float>(WINCY),
		LAYER_UI_BACKGROUND,
		L"Resource/UI/Loading_BG.png",
		0.f,
		0.0f, 0.0f,  // anchorMin (TitleScene과 동일하게 0.0~1.0으로 설정)
		1.0f, 1.0f,  // anchorMax
		0.0f, 0.0f    // anchoredPosition
	);

	// 랜덤 스파이럴 이미지 (bg_spiral (1) ~ (8))
	int randomIndex = (rand() % 8) + 1;
	std::wstring spiralPath = L"Resource/UI/bg_spiral (" + std::to_wstring(randomIndex) + L").png";

	objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		static_cast<float>(WINCX),
		static_cast<float>(WINCY),
		LAYER_UI_FOREGROUND,
		spiralPath,
		0.1f,
		0.0f, 0.0f,  // anchorMin
		1.0f, 1.0f,  // anchorMax
		0.0f, 0.0f    // anchoredPosition
	);

	objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		400.0f,
		100.0f,
		L"로딩중...",
		Gdiplus::Color::Black,
		32.0f, Gdiplus::FontStyleBold,
		0.5f, 0.5f,
		0.5f, 0.5f,
		0.0f, 250.0f, 
		0.2f
	);
}

void LoadingScene::Update(float deltaTime)
{
	ObjectManager::GetInstance()->Update(deltaTime);

	m_loadingTime += deltaTime;

	if (!m_bFinished && m_loadingTime > 0.5f)
	{
		m_bFinished = true;
		SceneManager::GetInstance()->FinishLoading();
	}
}

void LoadingScene::LateUpdate()
{
	ObjectManager::GetInstance()->LateUpdate();
}

void LoadingScene::Render()
{
	ObjectManager::GetInstance()->Render();
}

void LoadingScene::Release()
{
	ObjectManager::GetInstance()->Release();
}
