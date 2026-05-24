#pragma once
#include "BaseScene.h"

// 캐릭터 정보 구조체
struct CharacterInfo
{
	std::wstring name;              // 캐릭터 이름
	std::wstring portraitPath;      // 포트레이트 이미지 경로
	std::wstring characterImagePath; // 캐릭터 이미지 경로
	std::wstring description;       // 캐릭터 설명
	float buttonPosX;              // 버튼 X 위치
	float buttonPosY;              // 버튼 Y 위치
	GameObjectID characterID;       // 캐릭터 ID
	bool isUnlocked;               // 해금 여부
	
	CharacterInfo(const std::wstring& n, const std::wstring& pp, const std::wstring& cip, 
				  const std::wstring& desc, float x, float y, GameObjectID id, bool unlocked = false)
		: name(n), portraitPath(pp), characterImagePath(cip), description(desc), 
		  buttonPosX(x), buttonPosY(y), characterID(id), isUnlocked(unlocked) {}
};

enum class CharacterSelectionState
{
	BROWSING,       // 캐릭터 목록 보기 상태
	CHARACTER_INFO, // 캐릭터 정보 표시 상태
	CONFIRM_SELECT, // 선택 확인 상태
	CLICK_GAME,
};

class CharacterSelectScene : public BaseScene
{
private:
	// 캐릭터 목록
	std::vector<CharacterInfo> m_characterList;        // 모든 등록된 캐릭터 목록
	
	// 상태 관리 변수
	CharacterSelectionState m_currentState;
	int m_selectedCharacterIndex;       // 선택된 캐릭터 인덱스 (-1: 선택 안됨)
	
	// 선택 상태 관리 변수
	bool m_isLockedCharacterSelected;   // 잠금 캐릭터가 선택되었는지 여부
	bool m_isSelectButtonDisabled;      // 선택 버튼 비활성화 여부

	// UI Elements
	class UIImage* m_pPlayerPortrait = nullptr;
	class UIImage* m_pPlayerInfo = nullptr;
	class UIText* m_pCharacterDescription = nullptr;
	class UIButton* m_pSelectButton = nullptr;
	class UIButton* m_pCancelButton = nullptr;
	class UIText* m_pSelectText = nullptr;
	class UIText* m_pCancelText = nullptr;

public:
	CharacterSelectScene();
	virtual ~CharacterSelectScene();

	// BaseScene 가상함수 구현
	virtual void Init(const MapData* mapData) override;
	virtual void Update(float deltaTime) override;
	virtual void LateUpdate() override;
	virtual void Render() override;
	virtual void Release() override;
	virtual SceneType GetSceneType() const override { return SCENE_CHARACTER_SELECT; }
	
	// 캐릭터 선택 public 함수들
	std::wstring GetSelectedCharacterName() const;
	int GetSelectedCharacterIndex() const { return m_selectedCharacterIndex; }
	
	// 선택된 캐릭터 ID 반환
	GameObjectID GetSelectedCharacterID() const;

private:
	void InitializeCharacters();        // 캐릭터 목록 초기화
	void CreateCharacterButtons();      // 캐릭터 버튼 생성
	void UpdateCharacterSelection();    // 선택된 캐릭터 UI 업데이트
	void UpdateCharacterDescription();  // 캐릭터 설명 텍스트 업데이트
	void UpdateCharacterUnlockStatus(); // 캐릭터 해금 상태 업데이트
	void UpdateSelectButtonState();    // 선택 버튼 상태 업데이트
	
	// 버튼 콜백 함수들
	void OnCharacterButtonClicked(int characterIndex);
	void OnSelectButtonClicked();
	void OnCancelButtonClicked();
	void OnBackButtonClicked();        // 뒤로가기 버튼 클릭
};
