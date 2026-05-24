#include "99_Default/pch.h"
#include "AnimationClip.h"
#include "../02_GameObject/Component/Sprite/SpriteSheet.h"

// ============== AnimationClip 구현 ==============

AnimationClip::AnimationClip(
    const std::wstring& name,
    std::shared_ptr<SpriteSheet> spriteSheet,
    bool loop,
    bool preFlipped,
    float frameDuration) 
    : m_pSpriteSheet(std::move(spriteSheet)),
      m_isLooping(loop), 
      m_totalDuration(0.0f),
      m_preFlipped(preFlipped) {
    
    if (m_pSpriteSheet) {
        const auto& sprites = m_pSpriteSheet->GetSprites();
        for (const auto& sprite : sprites) {
            // Note: pivot is already set in Sprite during SpriteSheet::Slice
            m_frames.emplace_back(sprite, frameDuration);
            m_totalDuration += frameDuration;
        }
    }
}

AnimationClip::~AnimationClip() {}

bool AnimationClip::IsLooping() const {
    return m_isLooping;
}

float AnimationClip::GetTotalDuration() const {
    return m_totalDuration;
}

const SpriteSheet* AnimationClip::GetSpriteSheet() const {
    return m_pSpriteSheet.get();
}

const std::vector<AnimationFrame>& AnimationClip::GetFrames() const {
    return m_frames;
}

const AnimationFrame& AnimationClip::GetCurrentFrame(float elapsed) const {
    if (m_frames.empty()) {
        static AnimationFrame dummyFrame;
        return dummyFrame;
    }

    float t;
    if (m_totalDuration <= 0.0f) {
        t = 0.0f;  
    } else {
        t = m_isLooping ? fmod(elapsed, m_totalDuration) : min(elapsed, m_totalDuration);
    }
    
    float acc = 0.0f;
    for (const auto& frame : m_frames) {
        acc += frame.duration;
        if (t < acc) return frame;
    }
    return m_frames.back(); 
}

void AnimationClip::AddEventFrame(int frameIndex, const std::wstring& eventName) {
    if (frameIndex >= 0 && frameIndex < m_frames.size()) {
        m_eventFrames[frameIndex] = eventName;
    }
}

void AnimationClip::SetEventCallback(AnimationEventCallback callback) {
    m_eventCallback = callback;
}
