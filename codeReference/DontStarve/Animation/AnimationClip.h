#pragma once

#include "../02_GameObject/Component/Sprite/SpriteSheet.h"

// 애니메이션 이벤트 콜백 타입 정의
typedef std::function<void(int, const std::wstring&)> AnimationEventCallback;

class AnimationClip {
public:
    // SpriteSheet를 받는 생성자 (preFlipped: 로드 시 이미 좌우 반전된 비트맵이면 true, 렌더 시 Transform 스킵용)
    // frameDuration: 모든 프레임에 적용되는 지속 시간(초), 기본 0.03
    AnimationClip(
        const std::wstring& name,
        std::shared_ptr<SpriteSheet> spriteSheet,
        bool loop = true,
        bool preFlipped = false,
        float frameDuration = 0.03f
    );

    ~AnimationClip();
    
    // 프레임 / 상태 조회 메서드
    bool IsLooping() const;
    float GetTotalDuration() const;
    const SpriteSheet* GetSpriteSheet() const;
    const std::vector<AnimationFrame>& GetFrames() const;
    const AnimationFrame& GetCurrentFrame(float elapsed) const;

    // 이벤트 관련 메서드
    void AddEventFrame(int frameIndex, const std::wstring& eventName);
    void SetEventCallback(AnimationEventCallback callback);
    const std::map<int, std::wstring>& GetEventFrames() const { return m_eventFrames; }
    const AnimationEventCallback& GetEventCallback() const { return m_eventCallback; }

    bool IsPreFlipped() const { return m_preFlipped; }

private:    
    std::shared_ptr<SpriteSheet> m_pSpriteSheet;
    std::vector<AnimationFrame> m_frames;
    bool m_isLooping;
    float m_totalDuration;
    bool m_preFlipped;

    // 이벤트 관련 멤버
    std::map<int, std::wstring> m_eventFrames;
    AnimationEventCallback m_eventCallback;
};
