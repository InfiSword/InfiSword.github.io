#include "99_Default/pch.h"
#include "Animator.h"
#include "AnimationClip.h"
#include "../02_GameObject/Component/Sprite/SpriteSheet.h"
#include "../01_Manager/RenderManager/RenderManager.h"
#include "../01_Manager/ResourceManager/ResourceManager.h"
#include "../02_GameObject/GameObject.h"
#include "../02_GameObject/Component/Transform/Transform.h"
#include "../02_GameObject/Component/Sprite/SpriteRenderer.h"

Animator::Animator(GameObject* owner, SpriteRenderer* renderTarget)
	: Component(owner), m_currentClip(nullptr), m_currentState(-1), m_currentDirection(-1),
	m_elapsed(0.0f), m_isPlaying(false), m_lastTriggeredFrame(-1), m_renderTarget(renderTarget)
{ }

Animator::~Animator() {
	m_animations.clear();
	m_currentClip = nullptr;
}

void Animator::Init()
{ }

void Animator::RegisterAnimation(int state, Direction dir,
	const std::wstring& imagePath,
	UINT frameWidth, UINT frameHeight,
	UINT framesPerRow, UINT totalFrames,
	float pivotX, float pivotY,
	bool loop,
	float frameDuration,
	bool flipHorizontal)
{
	int key = GetAnimationKey(state, static_cast<int>(dir));
	bool shouldFlip = flipHorizontal ? true : (dir == DIR_LEFT);
	// ResourceManager를 통해 SpriteSheet 로드 (캐싱 지원)
	std::shared_ptr<SpriteSheet> sheet = ResourceManager::GetInstance()->LoadSpriteSheet(
		imagePath, frameWidth, frameHeight, framesPerRow, totalFrames, shouldFlip, { pivotX, pivotY });
	
	if (m_animations.find(key) != m_animations.end()) {
		return;
	}

	std::unique_ptr<AnimationClip> clip = std::make_unique<AnimationClip>
		(L"", sheet, loop, shouldFlip, frameDuration);
	if (!clip) return;

	m_animations[key] = std::move(clip);
}

AnimationClip* Animator::GetAnimationClip(int state, Direction dir)
{
	int key = GetAnimationKey(state, static_cast<int>(dir));
	auto it = m_animations.find(key);
	if (it != m_animations.end()) {
		return it->second.get();
	}
	return nullptr;
}

// SetState 구현
void Animator::SetState(int state, Direction direction, bool restart) {
	int newDirection = static_cast<int>(direction);

	if (m_currentState != state || m_currentDirection != newDirection || restart) {
		m_currentState = state;
		m_currentDirection = newDirection;
		SelectAndPlayAnimation();
	}
}

void Animator::SelectAndPlayAnimation() 
{
	int key = GetAnimationKey(m_currentState, m_currentDirection);
	auto it = m_animations.find(key);

	if (it != m_animations.end()) {
		AnimationClip* newClip = it->second.get();
		m_currentClip = newClip;

		m_elapsed = 0.0f;
		m_isPlaying = true;
		m_lastTriggeredFrame = -1;

		// 애니메이션 상태가 바뀔 때 SpriteRenderer에도 현재 클립의 대표 스프라이트와 반전 상태를 설정
		// 이를 통해 SpriteRenderer::GetPivotY() 등이 해당 애니메이션의 피벗을 올바르게 참조할 수 있음
		if (m_renderTarget && !newClip->GetFrames().empty()) {
			m_renderTarget->SetSprite(newClip->GetFrames()[0].sprite);
			m_renderTarget->SetPreFlipped(newClip->IsPreFlipped());
		}
	}
}

void Animator::Update(float deltaTime)
{
	if (m_isPlaying && m_currentClip)
	{
		// 경과 시간 누적
		m_elapsed += deltaTime;

		float totalDuration = m_currentClip->GetTotalDuration();
		// 루프가 아닌 애니메이션의 종료 체크
		if (!m_currentClip->IsLooping() && m_elapsed >= totalDuration) {
			m_elapsed = totalDuration;
			m_isPlaying = false;
		}

		// 현재 프레임 인덱스 계산
		int currentFrameIndex = GetCurrentFrameIndex();

		// 매 프레임 SpriteRenderer(m_renderTarget)에 현재 프레임의 스프라이트를 동기화
		if (m_renderTarget && currentFrameIndex != -1) {
			const auto& frames = m_currentClip->GetFrames();

			// 프레임 건너뛰기
			int displayIndex = (currentFrameIndex / 5) * 5;
			if (displayIndex >= (int)frames.size()) {
				displayIndex = (int)frames.size() - 1;
			}

			if (displayIndex < (int)frames.size()) {							
				m_renderTarget->SetSprite(frames[displayIndex].sprite);
			}
		}

		// 프레임 변경 시, 건너뛴 프레임 포함해 지나친 모든 프레임에 대해 이벤트 발생
		if (currentFrameIndex != -1 && currentFrameIndex != m_lastTriggeredFrame)
		{
			if (m_owner) m_owner->SetSpatialDirty();
			const std::map<int, std::wstring>& eventFrames = m_currentClip->GetEventFrames();
			const AnimationEventCallback& callback = m_currentClip->GetEventCallback();

			int startIdx = m_lastTriggeredFrame + 1;
			int endIdx = currentFrameIndex;

			// 현재 클립을 로컬에 저장 (콜백 중 m_currentClip이 바뀔 수 있음)
			AnimationClip* pCurrentClipBeforeCallback = m_currentClip;

			for (int fi = startIdx; fi <= endIdx && callback; ++fi ) {
				auto eventIt = eventFrames.find(fi);
				if (eventIt != eventFrames.end())
				{
					callback(fi, eventIt->second);

					// 콜백 내부에서 ChangeState 등으로 애니메이션이 바뀌었는지 확인
					if (pCurrentClipBeforeCallback != m_currentClip) {
						// 애니메이션이 바뀌었으므로 현재 루프 중단. 
						// m_lastTriggeredFrame은 이미 SelectAndPlayAnimation에서 -1로 리셋되었으므로 덮어씌우지 않음.
						return;
					}
				}
			}
			m_lastTriggeredFrame = currentFrameIndex;
		}
	}
}

const AnimationFrame& Animator::GetCurrentFrame() const {
	if (!m_currentClip || m_currentClip->GetFrames().empty()) {
		static AnimationFrame dummyFrame;
		return dummyFrame;
	}
	return m_currentClip->GetCurrentFrame(m_elapsed);
}

const SpriteSheet* Animator::GetSpriteSheet() const {
	if (!m_currentClip) {
		return nullptr;
	}
	return m_currentClip->GetSpriteSheet();
}

bool Animator::IsAnimationDone() const {
	if (!m_currentClip) return true;
	if (m_currentClip->IsLooping()) return false;
	return m_elapsed >= m_currentClip->GetTotalDuration();
}

float Animator::GetCurrentClipTotalDuration() const {
	if (m_currentClip) {
		return m_currentClip->GetTotalDuration();
	}
	return 0.0f;
}

int Animator::GetCurrentFrameIndex() const {
	if (!m_currentClip || m_currentClip->GetFrames().empty()) {
		return -1;
	}

	float t = m_currentClip->IsLooping() ? fmod(m_elapsed, m_currentClip->GetTotalDuration()) : min(m_elapsed, m_currentClip->GetTotalDuration());
	float acc = 0.0f;

	for (size_t i = 0; i < m_currentClip->GetFrames().size(); ++i) {
		acc += m_currentClip->GetFrames()[i].duration;
		if (t < acc) {
			return static_cast<int>(i);
		}
	}

	return static_cast<int>(m_currentClip->GetFrames().size() - 1);
}

void Animator::Play() { if (m_currentClip) { m_isPlaying = true; } }
void Animator::Pause() { m_isPlaying = false; }
void Animator::Stop() { m_isPlaying = false; m_elapsed = 0.0f; }
