#pragma once
#include "GameScene.h"

class BossHoundScene : public GameScene
{
public:
    enum class BossPhase {
        Phase1_Hounds,      // 일반 하운드 처치 페이즈
        PhaseTransition,    // 페이즈 전환 대기
        Phase2_BossIntro,   // 보스 등장 카메라 연출
        Phase2_BossBattle,  // 보스 전투
        Cleared             // 클리어
    };

    BossHoundScene();
    virtual ~BossHoundScene();

    virtual void Init(const MapData* mapData) override;
    virtual void Update(float deltaTime) override;
    virtual void Render() override;
    virtual void Release() override;

    virtual SceneType GetSceneType() const override { return SCENE_GAME_HOUND_FOREST; }

    // 보스 AI가 참조할 상태 정보
    BossPhase GetCurrentPhase() const { return m_currentPhase; }
    bool IsIntroReturning() const;

private:
    void UpdatePhase1(float deltaTime);
    void UpdatePhaseTransition(float deltaTime);
    void UpdatePhase2Intro(float deltaTime);
    void UpdatePhase2Battle(float deltaTime);
    void UpdateCleared(float deltaTime);

    void StartBossIntro();
    void SpawnBoss();

private:
    enum class IntroStep {
        MoveToBoss,
        WaitHowl,
        WaitExtra,
        ReturnToPlayer
    };

    BossPhase m_currentPhase;
    float m_phaseTimer;
    
    // 카메라 연출 관련
    bool m_isIntroRunning;
    float m_introTimer;
    int m_introTargetBossIndex;
    IntroStep m_introStep;
    bool m_startedHowl;
    Gdiplus::PointF m_introTargetPos;
    Gdiplus::PointF m_introStartPos;

    std::vector<GameObject*> m_bossObjects;
    bool m_bossesActivated;
    bool m_isClearUIShown;

    // UI
    class GameClearUI* m_gameClearUI;
    class HPUI* m_iceBossHPUI;
    class HPUI* m_redBossHPUI;
};
