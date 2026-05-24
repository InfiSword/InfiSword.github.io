#pragma once
#include "BaseScene.h"

class TitleScene : public BaseScene
{
public:
	TitleScene();
	virtual ~TitleScene();

	// BaseScene 가상함수 구현
	virtual void Init(const MapData* mapData) override;
	virtual void Update(float deltaTime) override;
	virtual void LateUpdate() override;
	virtual void Render() override;
	virtual void Release() override;
	virtual SceneType GetSceneType() const override { return SCENE_TITLE; }

private:
	void OnStartButtonClicked();
	void OnExitButtonClicked();
	void OnResetButtonClicked();

	class UIText* m_resetMessageText = nullptr;
	bool m_bIsReleased = false;
};
