#pragma once
#include "BaseScene.h"

class LoadingScene : public BaseScene
{
public:
	LoadingScene();
	virtual ~LoadingScene();

	virtual void Init(const MapData* mapData) override;
	virtual void Update(float deltaTime) override;
	virtual void LateUpdate() override;
	virtual void Render() override;
	virtual void Release() override;
	virtual SceneType GetSceneType() const override { return SCENE_LOADING; }

private:
	float m_loadingTime;
	bool m_bFinished;
};
