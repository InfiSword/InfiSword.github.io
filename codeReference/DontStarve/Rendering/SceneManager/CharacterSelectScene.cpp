#include "99_Default/pch.h"
#include "CharacterSelectScene.h"
#include "SceneManager.h"
#include "../ObjectManager/ObjectManager.h"
#include "../InputManager/InputManager.h"
#include "../ResourceManager/ResourceManager.h"
#include "../DataManager/DataManager.h"
#include "../GameProgressManager/GameProgressManager.h"
#include "../../02_GameObject/UI/UIImage.h"
#include "../../02_GameObject/UI/UIButton.h"
#include "../../02_GameObject/UI/UIText.h"

CharacterSelectScene::CharacterSelectScene()
	: m_currentState(CharacterSelectionState::BROWSING), m_selectedCharacterIndex(-1),
	  m_isLockedCharacterSelected(false), m_isSelectButtonDisabled(true)
{
}

CharacterSelectScene::~CharacterSelectScene()
{
	Release();
}

void CharacterSelectScene::Init(const MapData* mapData)
{
	OutputDebugStringW(L"=== CharacterSelectScene::Init() 시작 ===\n");
	
	// 매니저 초기화 - Init() 시점에 수행하여 이전 씬의 객체들을 확실히 정리
	ObjectManager::GetInstance()->Init();
	InputManager::GetInstance()->Init();
	
	// 캐릭터 목록 초기화
	InitializeCharacters();
	
	// UI 생성
	ObjectManager* objectManager = ObjectManager::GetInstance();

	// 배경 이미지 생성 (전체 화면)
	objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		static_cast<float>(WINCX),
		static_cast<float>(WINCY),
		LAYER_UI_BACKGROUND,
		L"Resource/UI/BG.png",
		0.f,
		0.0f, 0.0f,  
		1.0f, 1.0f,  
		0.0f, 0.0f    
	);

	// 뒤로가기 버튼 생성 (좌측 중앙)
	objectManager->CreateButton(
		static_cast<GameObjectID>(GOID_UI_BUTTON),
		80.0f,
		100.0f,
		L"Resource/UI/Button.png",
		L"Resource/UI/Button.png",
		0.0f, 0.5f,  
		0.0f, 0.5f,  
		100.0f, 300.0f,
		[this]() { OnBackButtonClicked(); }
	);

	// 선택된 캐릭터 포트레이트 (우측 중앙) - 초기에는 숨김
	m_pPlayerPortrait = objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		350.0f,
		500.0f,
		LAYER_UI_FOREGROUND,
		L"Resource/UI/wilson.png",
		1.0f,
		1.0f, 0.5f,  
		1.0f, 0.5f,  
		-300.0f, -150.0f 
	);
	if (m_pPlayerPortrait) m_pPlayerPortrait->SetActive(false);

	// 캐릭터 정보창 (우측 중앙) - 초기에는 숨김
	m_pPlayerInfo = objectManager->CreateImage(
		static_cast<GameObjectID>(GOID_UI_IMAGE),
		550.0f,
		200.0f,
		LAYER_UI_FOREGROUND,
		L"Resource/UI/UI4.png",
		1.0f,
		1.0f, 0.5f,  
		1.0f, 0.5f,  
		-320.0f, 180.0f 
	);
	if (m_pPlayerInfo) m_pPlayerInfo->SetActive(false);  

	// 캐릭터 설명 텍스트 생성 - 정렬 매개변수 사용 (Near)
	m_pCharacterDescription = objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		500.0f - 40.0f,  
		200.0f - 40.0f,  
		L"",
		Gdiplus::Color::Black,
		16.0f, Gdiplus::FontStyleBold,
		1.0f, 0.5f, 
		1.0f, 0.5f,  
		-300.0f, 180.0f,
		6.0f, // sortKey
		Gdiplus::StringAlignmentNear // hAlign
	);
	if (m_pCharacterDescription) m_pCharacterDescription->SetActive(false);

	// 선택 버튼 (캐릭터 정보창 아래, 왼쪽) - 초기에는 숨김
	m_pSelectButton = objectManager->CreateButton(
		static_cast<GameObjectID>(GOID_UI_BUTTON),
		120.0f,
		50.0f,
		L"Resource/UI/Select_Bar.png",
		L"Resource/UI/Select_Bar.png",
		1.0f, 0.5f, 
		1.0f, 0.5f,  
		-390.0f, 320.0f,
		[this]() { this->OnSelectButtonClicked(); }
	);
	if (m_pSelectButton) m_pSelectButton->SetActive(false);

	// 선택 버튼 텍스트 생성
	m_pSelectText = objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		120.0f,
		50.0f,
		L"선택",
		Gdiplus::Color::Black,
		16.0f, Gdiplus::FontStyleBold,
		1.0f, 0.5f,  
		1.0f, 0.5f,  
		-400.0f, 320.0f,
		0.1f // sortKey
	);
	if (m_pSelectText) m_pSelectText->SetActive(false);

	// 취소 버튼 (캐릭터 정보창 아래, 오른쪽) - 초기에는 숨김
	m_pCancelButton = objectManager->CreateButton(
		static_cast<GameObjectID>(GOID_UI_BUTTON),
		120.0f,
		50.0f,
		L"Resource/UI/Select_Bar.png",
		L"Resource/UI/Select_Bar.png",
		1.0f, 0.5f,  
		1.0f, 0.5f,  
		-210.0f, 320.0f,
		[this]() { this->OnCancelButtonClicked(); }
	);
	if (m_pCancelButton) m_pCancelButton->SetActive(false);

	// 취소 버튼 텍스트 생성
	m_pCancelText = objectManager->CreateText(
		static_cast<GameObjectID>(GOID_UI_TEXT),
		120.0f,
		50.0f,
		L"취소",
		Gdiplus::Color::Black,
		16.0f, Gdiplus::FontStyleBold,
		1.0f, 0.5f,  
		1.0f, 0.5f, 
		-220.0f, 320.0f,
		0.1f // sortKey
	);
	if (m_pCancelText) m_pCancelText->SetActive(false);

	// 캐릭터 버튼들 생성
	CreateCharacterButtons();
}

void CharacterSelectScene::InitializeCharacters()
{
	m_characterList.clear();
	
	float startX = 150.0f;
	float spacing = 200.0f;
	float characterY = 300.0f;
	
	// Wilson
	m_characterList.emplace_back(
		L"Wilson",
		L"Resource/UI/wilson.png",
		L"Resource/UI/Willson_Character.png",
		L"기본 캐릭터입니다.\n기본 스텟이 안정적으로 분포되어 있는\n균형잡힌 캐릭터입니다.",
		startX,
		characterY,
		GOID_PLAYER_WILSON,
		true  
	);
	
	// Willow
	const ResourcePathUtils::ObjectResourceDef* willowData = DataManager::GetInstance()->GetObjectResourceInfo(GOID_PLAYER_WILLOW);

	m_characterList.emplace_back(
		L"Willow",
		L"Resource/UI/willow.png",
		L"Resource/UI/Willow_Character_1.png",
		L"체력이 적지만 재빠른 캐릭터입니다.\n\n해금 조건: 늑대 던전 클리어",
		startX + spacing,
		characterY,
		GOID_PLAYER_WILLOW,
		GameProgressManager::GetInstance()->IsCharacterUnlocked(GOID_PLAYER_WILLOW)
	);
	
	// Wolfgang
	const ResourcePathUtils::ObjectResourceDef* wolfgangData = DataManager::GetInstance()->GetObjectResourceInfo(GOID_PLAYER_WOLFGANG);

	m_characterList.emplace_back(
		L"Wolfgang",
		L"Resource/UI/wolfgang.png",
		L"Resource/UI/Wolfgang_Character.png",
		L"기본적으로 체력이 높은 캐릭터입니다.\n\n해금 조건: 거미 던전 클리어",
		startX + spacing * 2,
		characterY,
		GOID_PLAYER_WOLFGANG,
		GameProgressManager::GetInstance()->IsCharacterUnlocked(GOID_PLAYER_WOLFGANG)
	);
}

void CharacterSelectScene::Update(float deltaTime)
{
	ObjectManager::GetInstance()->Update(deltaTime);
}
 
void CharacterSelectScene::CreateCharacterButtons()
{
	float screenHeight = static_cast<float>(WINCY);
	ObjectManager* objectManager = ObjectManager::GetInstance();
	
	for (size_t i = 0; i < m_characterList.size(); ++i) {
		const CharacterInfo& charInfo = m_characterList[i];
		
		float buttonWidth = 150.0f;
		float buttonHeight = 150.0f;
		float anchorPosX = charInfo.buttonPosX;
		float anchorPosY = charInfo.buttonPosY - screenHeight / 2.0f;
		
		int characterIndex = static_cast<int>(i);
		UIButton* hudButton = objectManager->CreateButton(
			static_cast<GameObjectID>(GOID_UI_BUTTON),
			buttonWidth,
			buttonHeight,
			L"Resource/UI/quagmire_hud.png",
			L"Resource/UI/quagmire_hud.png",
			0.0f, 0.5f,
			0.0f, 0.5f,
			anchorPosX, anchorPosY,
			[this, characterIndex]() { OnCharacterButtonClicked(characterIndex); }
		);
		
		if (hudButton) {
			hudButton->SetNormalColor(Gdiplus::Color(255, 255, 255, 255));  
			hudButton->SetHoverColor(Gdiplus::Color(255, 250, 250, 200));  
		}
		
		std::wstring displayImagePath = charInfo.isUnlocked ? charInfo.characterImagePath : L"Resource/UI/locked_Character.png";
		
		objectManager->CreateImage(
			static_cast<GameObjectID>(GOID_UI_IMAGE),
			buttonWidth * 0.8f,
			buttonHeight * 0.8f,
			LAYER_UI_FOREGROUND,
			displayImagePath,
			4.0f,
			0.0f, 0.5f,
			0.0f, 0.5f,
			anchorPosX, anchorPosY + 15.0f
		);
	}
}
 
void CharacterSelectScene::LateUpdate()
{
	ObjectManager::GetInstance()->LateUpdate();
}

void CharacterSelectScene::Render()
{
	ObjectManager::GetInstance()->Render();
	InputManager::GetInstance()->Render();
}

void CharacterSelectScene::Release()
{
	ObjectManager::GetInstance()->Release();
	InputManager::GetInstance()->Release();
}

void CharacterSelectScene::UpdateCharacterDescription()
{
	if (m_selectedCharacterIndex >= 0 && m_selectedCharacterIndex < static_cast<int>(m_characterList.size())) {
		if (m_pCharacterDescription) {
			m_pCharacterDescription->SetText(m_characterList[m_selectedCharacterIndex].description);
			m_pCharacterDescription->SetActive(true);
		}
	}
	else {
		if (m_pCharacterDescription) m_pCharacterDescription->SetActive(false);
	}
}

void CharacterSelectScene::UpdateCharacterSelection()
{
	if (m_selectedCharacterIndex >= 0 && m_selectedCharacterIndex < static_cast<int>(m_characterList.size())) {
		const CharacterInfo& selectedChar = m_characterList[m_selectedCharacterIndex];
		if (m_pPlayerPortrait) {
			std::wstring portraitPath = selectedChar.isUnlocked ? selectedChar.portraitPath : L"Resource/UI/locked.png";
			m_pPlayerPortrait->LoadSprite(portraitPath);
		}
	}
}

std::wstring CharacterSelectScene::GetSelectedCharacterName() const
{
	if (m_selectedCharacterIndex >= 0 && m_selectedCharacterIndex < static_cast<int>(m_characterList.size())) {
		return m_characterList[m_selectedCharacterIndex].name;
	}
	return L"UnKnown";
}

GameObjectID CharacterSelectScene::GetSelectedCharacterID() const
{
	if (m_selectedCharacterIndex >= 0 && m_selectedCharacterIndex < static_cast<int>(m_characterList.size())) {
		return m_characterList[m_selectedCharacterIndex].characterID;
	}
	return GOID_NONE;
}

void CharacterSelectScene::OnCharacterButtonClicked(int characterIndex)
{
	if (characterIndex >= 0 && characterIndex < static_cast<int>(m_characterList.size())) {
		m_selectedCharacterIndex = characterIndex;
		m_isLockedCharacterSelected = !m_characterList[characterIndex].isUnlocked;
		
		if (m_pPlayerPortrait) m_pPlayerPortrait->SetActive(true);
		if (m_pPlayerInfo) m_pPlayerInfo->SetActive(true);
		if (m_pSelectButton) m_pSelectButton->SetActive(true);
		if (m_pSelectText) m_pSelectText->SetActive(true);
		if (m_pCancelButton) m_pCancelButton->SetActive(true);
		if (m_pCancelText) m_pCancelText->SetActive(true);
		
		UpdateCharacterSelection();
		UpdateCharacterDescription();
		m_currentState = CharacterSelectionState::CONFIRM_SELECT;
		UpdateSelectButtonState();
	}
}

void CharacterSelectScene::OnSelectButtonClicked()
{
	if (m_selectedCharacterIndex == -1 || m_isLockedCharacterSelected) return;
	
	GameObjectID selectedCharacterID = GetSelectedCharacterID();
	m_currentState = CharacterSelectionState::CLICK_GAME;
	SceneManager::GetInstance()->LoadGameScene(SCENE_GAME_FARMING_AREA, selectedCharacterID);
}

void CharacterSelectScene::OnCancelButtonClicked()
{
	m_selectedCharacterIndex = -1;
	m_isLockedCharacterSelected = false;
	m_currentState = CharacterSelectionState::BROWSING;
	
	m_pPlayerPortrait->SetActive(false);
	m_pPlayerInfo->SetActive(false);
	m_pSelectButton->SetActive(false);
	m_pSelectText->SetActive(false);
	m_pCancelButton->SetActive(false);
	m_pCancelText->SetActive(false);
	m_pCharacterDescription->SetActive(false);
	
	UpdateSelectButtonState();
}

void CharacterSelectScene::OnBackButtonClicked()
{
	SceneManager::GetInstance()->LoadTitleScene();
}

void CharacterSelectScene::UpdateSelectButtonState()
{
	m_isSelectButtonDisabled = m_isLockedCharacterSelected;
	m_pSelectButton->SetDisabled(m_isSelectButtonDisabled);
}

void CharacterSelectScene::UpdateCharacterUnlockStatus()
{
	GameProgressManager* progressManager = GameProgressManager::GetInstance();
	for (auto& charInfo : m_characterList) {
		charInfo.isUnlocked = progressManager->IsCharacterUnlocked(charInfo.characterID);
	}
}
