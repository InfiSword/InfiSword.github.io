#pragma once

#include "../02_GameObject/Component/Component.h"
#include "AnimationClip.h"

class GameObject;
class SpriteRenderer;

class Animator : public Component {
public:
    Animator(GameObject* owner, SpriteRenderer* renderTarget);
    virtual ~Animator();

    void RegisterAnimation(int state, Direction dir, 
                          const std::wstring& imagePath,
                          UINT frameWidth, UINT frameHeight,
                          UINT framesPerRow, UINT totalFrames,
                          float pivotX = 0.5f, float pivotY = 1.0f,
                          bool loop = true,
                          float frameDuration = 0.03f,
                          bool flipHorizontal = false);  
	// false(기본값), true: 강제 반전 
    
    AnimationClip* GetAnimationClip(int state, Direction dir);

    void SetState(int state, Direction direction, bool restart);

    virtual void Init() override;
    virtual void Update(float deltaTime) override;

	const SpriteRenderer* GetRenderTarget() const { return m_renderTarget; }
    const AnimationFrame& GetCurrentFrame() const;
    const SpriteSheet* GetSpriteSheet() const;
    bool IsAnimationDone() const;
    AnimationClip* GetClip() const { return m_currentClip; }
    float GetCurrentClipTotalDuration() const;
    int GetCurrentFrameIndex() const;

    void Play();
    void Pause();
    void Stop();

private:
    std::map<int, std::unique_ptr<AnimationClip>> m_animations;

	SpriteRenderer* m_renderTarget;  // 애니메이션 프레임 렌더링 대상.
    AnimationClip* m_currentClip;
    int m_currentState;
    int m_currentDirection;
    float m_elapsed;
    bool m_isPlaying;
    int m_lastTriggeredFrame;

    int GetAnimationKey(int state, int direction) const { return state * 1000 + direction; }
    void SelectAndPlayAnimation();
};
