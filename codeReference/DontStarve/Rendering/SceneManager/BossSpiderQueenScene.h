#pragma once
#include "GameScene.h"

class UIText;
class UIImage;

class BossSpiderQueenScene : public GameScene
{
public:
    enum class BossPhase {
        Phase1_Minions,     // 일반 거미 처치 페이즈
        PhaseTransition,    // 페이즈 전환 대기
        Phase2_BossIntro,   // 보스 등장 카메라 연출
        Phase2_BossBattle,  // 보스 전투
        Cleared             // 클리어
    };

    BossSpiderQueenScene();
    virtual ~BossSpiderQueenScene();

    virtual void Init(const MapData* mapData) override;
    virtual void Update(float deltaTime) override;
    virtual void Render() override;
    virtual void Release() override;

    virtual SceneType GetSceneType() const override { return SCENE_GAME_SPIDER_QUEEN_HOUSE; }

    BossPhase GetCurrentPhase() const { return m_currentPhase; }
    bool IsIntroReturning() const;

private:
    void UpdatePhase1(float deltaTime);
    void UpdatePhaseTransition(float deltaTime);
    void UpdatePhase2Intro(float deltaTime);
    void UpdatePhase2Battle(float deltaTime);
    void UpdateCleared(float deltaTime);

    void StartBossIntro();

private:
    enum class IntroStep {
        MoveToBoss,
        WaitTaunt,
        WaitExtra,
        ReturnToPlayer
    };

    std::wstring m_bossName;
    class HPUI* m_bossHPUI;
    BossPhase m_currentPhase;
    float m_phaseTimer;
    
    // 카메라 연출 관련
    bool m_isIntroRunning;
    float m_introTimer;
    Gdiplus::PointF m_introTargetPos;
    Gdiplus::PointF m_introStartPos;
    IntroStep m_introStep;
    bool m_startedTaunt;

    GameObject* m_bossObject;
    std::vector<GameObject*> m_minionObjects;
    bool m_bossActivated;
    bool m_isClearUIShown;
    bool m_chaseStarted;

    // UI
    class GameClearUI* m_gameClearUI;
};
