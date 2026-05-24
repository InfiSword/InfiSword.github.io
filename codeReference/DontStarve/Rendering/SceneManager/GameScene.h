#pragma once
#include "BaseScene.h"

class GameObject;
class Player;
class MenuUI;
class HPUI;
class GameOverUI;

class GameScene : public BaseScene
{
public:
	GameScene();
	virtual ~GameScene();

	// BaseScene 가상 함수 구현
	virtual void Init(const MapData* mapData) override;
	virtual void Update(float deltaTime) override;
	virtual void LateUpdate() override;
	virtual void Render() override;
	virtual void Release() override;

	virtual SceneType GetSceneType() const override { return SCENE_GAME_FARMING_AREA; }

	// 선택된 캐릭터 ID 설정
	void SetSelectedCharacterID(GameObjectID characterID) { m_selectedCharacterID = characterID; }
	GameObjectID GetSelectedCharacterID() const { return m_selectedCharacterID; }

protected:
	void CreateGameObjectsFromMapData();
	void SpawnPlayer();
	
protected:
	const MapData* m_mapData;
	GameObjectID m_selectedCharacterID;

private:
	MenuUI* m_craftingUI;
	HPUI* m_playerHPUI;
	GameOverUI* m_gameOverUI;
};
