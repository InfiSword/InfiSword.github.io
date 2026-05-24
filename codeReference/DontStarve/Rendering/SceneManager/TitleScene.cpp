#include "99_Default/pch.h"
#include "TitleScene.h"
#include "SceneManager.h"
#include "../InputManager/InputManager.h"
#include "../ResourceManager/ResourceManager.h"
#include "../ObjectManager/ObjectManager.h"
#include "../GameProgressManager/GameProgressManager.h"
#include "../../02_GameObject/UI/UIImage.h"
#include "../../02_GameObject/UI/UIButton.h"
#include "../../02_GameObject/UI/UIText.h"

TitleScene::TitleScene()
	: m_resetMessageText(nullptr)
{
}

TitleScene::~TitleScene()
{
	OutputDebugStringW(L"TitleScene: 소멸자 호출\n");
	Release();
}

void TitleScene::Init(const MapData* mapData)
{
	// TitleScene에 필요한 매니저들 초기화
	ObjectManager::GetInstance()->Init();
	InputManager::GetInstance()->Init();
	
	// UI 생성
	ObjectManager* objectManager = ObjectManager::GetInstance();
	ResourceManager* resourceManager = ResourceManager::GetInstance();

	// 배경 이미지 생성 (전체 화면)
	objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		static_cast<float>(WINCX),
		static_cast<float>(WINCY),
		LAYER_UI_BACKGROUND,
		L"Resource/UI/motd_fallbacks_box6.png",
		0.f,
		0.0f, 0.0f,  // anchorMin
		1.0f, 1.0f,  // anchorMax
		0.0f, 0.0f    // anchoredPosition
	);

	// 로고 이미지 생성 (화면 상단 중앙)
	objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		400.0f,
		200.0f,
		LAYER_UI_FOREGROUND,
		L"Resource/UI/logo.png",
		0.f,
		0.5f, 0.0f,  // anchorMin (상단 중앙)
		0.5f, 0.0f,  // anchorMax (상단 중앙)
		0.0f, 200.0f // anchoredPosition (상단에서 아래로 200px)
	);

	// 게임시작 버튼 생성 (화면 중앙 기준 아래로 100px)
	UIButton* startButton = objectManager->CreateButton(
		static_cast<GameObjectID>(GOID_UI_BUTTON),
		200.0f,
		60.0f,
		L"Resource/UI/frontscreen.png",
		L"Resource/UI/HighLight_frontscreen.png",
		0.5f, 0.5f,  // anchorMin (중앙)
		0.5f, 0.5f,  // anchorMax (중앙)
		0.0f, 100.0f, // anchoredPosition (중앙에서 아래로 100px)
		[this]() { OnStartButtonClicked(); }
	);

	if (startButton) {
		// Hover 시 밝게 빛나는 효과 설정
		startButton->SetNormalColor(Gdiplus::Color(255, 255, 255, 255));  // 완전 흰색 (밝게)
		startButton->SetHoverColor(Gdiplus::Color(255, 220, 220, 220)); // 기본 (약간 어둡게)
	}

	// 게임시작 버튼 텍스트 생성 (버튼과 동일한 anchor)
	objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		200.0f,
		60.0f,
		L"게임시작",
		Gdiplus::Color::Black,
		16.0f, Gdiplus::FontStyleRegular,
		0.5f, 0.5f,  // anchorMin (중앙)
		0.5f, 0.5f,  // anchorMax (중앙)
		0.0f, 100.0f, // anchoredPosition (중앙에서 아래로 100px)
		0.1f // sortKey
	);

	// 종료 버튼 생성 (화면 중앙 기준 아래로 200px)
	UIButton* exitButton = objectManager->CreateButton(
		static_cast<GameObjectID>(GOID_UI_BUTTON),
		200.0f,
		60.0f,
		L"Resource/UI/frontscreen.png",
		L"Resource/UI/HighLight_frontscreen.png",
		0.5f, 0.5f,  // anchorMin (중앙)
		0.5f, 0.5f,  // anchorMax (중앙)
		0.0f, 200.0f, // anchoredPosition (중앙에서 아래로 200px)
		[this]() { OnExitButtonClicked(); }
	);

	if (exitButton) {
		// Hover 시 밝게 빛나는 효과 설정
		exitButton->SetNormalColor(Gdiplus::Color(255, 255, 255, 255));  // 완전 흰색 (밝게)
		exitButton->SetHoverColor(Gdiplus::Color(255, 220, 220, 220)); // 기본 (약간 어둡게)
	}

	// 종료 버튼 텍스트 생성 (버튼과 동일한 anchor)
	objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		200.0f,
		60.0f,
		L"종료",
		Gdiplus::Color::Black,
		16.0f, Gdiplus::FontStyleRegular,
		0.5f, 0.5f,  // anchorMin (중앙)
		0.5f, 0.5f,  // anchorMax (중앙)
		0.0f, 200.0f, // anchoredPosition (중앙에서 아래로 200px)
		0.1f // sortKey
	);

	// --- 진행상황 초기화 버튼 추가 (오른쪽 위) ---
	objectManager->CreateButton(
		static_cast<GameObjectID>(GOID_UI_BUTTON),
		180.0f,
		40.0f,
		L"Resource/UI/frontscreen.png",
		L"Resource/UI/HighLight_frontscreen.png",
		1.0f, 0.0f,  // anchorMin (우측 상단)
		1.0f, 0.0f,  // anchorMax (우측 상단)
		-100.0f, 50.0f, // anchoredPosition (우측에서 100px 좌측, 상단에서 50px 아래)
		[this]() { OnResetButtonClicked(); }
	);

	objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		180.0f,
		40.0f,
		L"진행상황 초기화",
		Gdiplus::Color::DarkRed,
		12.0f, Gdiplus::FontStyleRegular,
		1.0f, 0.0f,
		1.0f, 0.0f,
		-100.0f, 50.0f,
		0.1f
	);

	// 리셋 완료 메시지 텍스트 (초기에는 비활성)
	m_resetMessageText = objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		400.0f,
		60.0f,
		L"초기화 되었습니다!",
		Gdiplus::Color::LimeGreen,
		24.0f, Gdiplus::FontStyleBold,
		0.5f, 0.5f,  // anchorMin (중앙)
		0.5f, 0.5f,  // anchorMax (중앙)
		0.0f, 0.0f,   // anchoredPosition (중앙)
		0.0f // sortKey (가장 앞에 렌더링)
	);
	if (m_resetMessageText) m_resetMessageText->SetActive(false);
}

void TitleScene::Update(float deltaTime)
{
	// TitleScene에서 ObjectManager 업데이트
	// InputManager는 메인 루프에서 가장 먼저 업데이트됨 (반응 속도 개선)
	ObjectManager::GetInstance()->Update(deltaTime);
}

void TitleScene::LateUpdate()
{
	ObjectManager::GetInstance()->LateUpdate();
	// InputManager::LateUpdate는 메인 루프에서 처리됨
}

void TitleScene::Render()
{
	// 매니저들 렌더링
	ObjectManager::GetInstance()->Render();
	InputManager::GetInstance()->Render();
}

void TitleScene::Release()
{
	if (m_bIsReleased) {
		OutputDebugStringW(L"TitleScene: 이미 Release됨 (중복 호출 방지)\n");
		return;
	}
	m_bIsReleased = true;
	OutputDebugStringW(L"TitleScene: Release() 실행\n");

	ObjectManager::GetInstance()->Release();
	InputManager::GetInstance()->Release();
	m_resetMessageText = nullptr;
}

void TitleScene::OnStartButtonClicked()
{
	SceneManager::GetInstance()->LoadCharacterSelectScene();
}

void TitleScene::OnExitButtonClicked()
{
	PostQuitMessage(0);
}

void TitleScene::OnResetButtonClicked()
{
	GameProgressManager::GetInstance()->ResetAllProgress();
	if (m_resetMessageText) {
		m_resetMessageText->SetActive(true);
		
		// 2초 뒤에 메시지 숨기기 (코루틴 활용)
		m_resetMessageText->StopAllCoroutines();
		float timer = 0.0f;
		m_resetMessageText->StartCoroutine([this, timer](float dt) mutable -> bool {
			timer += dt;
			if (timer >= 2.0f) {
				m_resetMessageText->SetActive(false);
				return false;
			}
			return true;
		});
	}
}
