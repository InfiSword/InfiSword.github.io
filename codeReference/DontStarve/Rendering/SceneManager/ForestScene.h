#pragma once
#include "GameScene.h"

class IntroNoticeUI;

class ForestScene : public GameScene
{
public:
    ForestScene();
    virtual ~ForestScene();

    // 초기 숲 지역(Farming Area) 전용 로직을 위한 가상 함수 재정의
    virtual void Init(const MapData* mapData) override;
    virtual SceneType GetSceneType() const override { return SCENE_GAME_FARMING_AREA; }

private:
	IntroNoticeUI* introUI;
	static bool m_bIntroPlayed;
};
