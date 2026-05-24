#include "99_Default/pch.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/SoundManager/SoundManager.h"
#include "../../../03_Animation/Animator.h"
#include "../../../03_Animation/AnimationClip.h"
#include "../Player/Player.h"
#include "../../Component/Sprite/SpriteSheet.h"
#include "../../Component/Transform/Transform.h"
#include "../../Component/Collider/BoxCollider.h"
#include "Hound.h"

Hound::Hound(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Monster(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType)
	, m_bHasHowled(false)
	, m_hasHowlStarted(false)
	, m_isCombatEnabled(true)
{
	m_hp = 90;
	m_maxHp = m_hp;
	m_type = GO_TYPE_MONSTER;
	m_walkSpeed = 80.0f;
	m_runSpeed = 200.0f;
	m_attackRange = 80.0f;
	m_attackCooldown = 1.0f;
	m_attackHitFrame = 4;
	m_damage = 20;
	m_attackBoxWidth = 80;
	m_attackBoxHeight = 55;
}

Hound::~Hound() {}

void Hound::Init()
{
	Monster::Init();
	SetupAggro(AggroType::ALWAYS, 0.0f, 0.0f);
	SetupAttackBox(m_attackBoxWidth, m_attackBoxHeight,0,-40.f);

	ChangeState((int)HoundState::IDLE);

	m_idleTimer = 0.0f;
	m_idleDuration = 2.0f + (rand() / (float)RAND_MAX) * 3.0f;
	m_bHasHowled = false;
	m_hasHowlStarted = false;

	if (this->m_transform) {
		m_targetX = this->m_transform->GetX();
		m_targetY = this->m_transform->GetY();
	}

	if (!m_animator) m_animator = AddComponent<Animator>(m_spriteRenderer);
	if (m_animator) {
		DataManager* pRM = DataManager::GetInstance();
		const ResourcePathUtils::ObjectResourceDef* objData = pRM->GetObjectResourceInfo(m_id);
		if (objData) {
			std::wstring base = objData->baseDir + L"\\";
			float px = objData->pivotX;
			float py = objData->pivotY;

			m_animator->RegisterAnimation((int)HoundState::IDLE, DIR_DOWN, base + L"Hound_hound_idle_down.png", 0, 0, 7, 20, px, py, true, 0.02f);
			m_animator->RegisterAnimation((int)HoundState::IDLE, DIR_UP, base + L"Hound_hound_idle_up.png", 0, 0, 7, 20, px, py, true, 0.02f);
			std::wstring idleSidePath = base + L"Hound_hound_idle_side.png";
			m_animator->RegisterAnimation((int)HoundState::IDLE, DIR_LEFT, idleSidePath, 0, 0, 7, 20, px, py, true, 0.02f, false);
			m_animator->RegisterAnimation((int)HoundState::IDLE, DIR_RIGHT, idleSidePath, 0, 0, 7, 20, px, py, true, 0.02f);

			m_animator->RegisterAnimation((int)HoundState::CHASE, DIR_DOWN, base + L"Hound_hound_run_loop_down.png", 0, 0, 7, 16, px, py, true, 0.02f);
			m_animator->RegisterAnimation((int)HoundState::CHASE, DIR_UP, base + L"Hound_hound_run_loop_up.png", 0, 0, 7, 16, px, py, true, 0.02f);
			std::wstring walkSidePath = base + L"Hound_hound_run_loop_side.png";
			m_animator->RegisterAnimation((int)HoundState::CHASE, DIR_LEFT, walkSidePath, 0, 0, 7, 16, px, py, true, 0.02f, false);
			m_animator->RegisterAnimation((int)HoundState::CHASE, DIR_RIGHT, walkSidePath, 0, 0, 7, 16, px, py, true, 0.02f);

			// WANDER 애니메이션도 CHASE와 동일한 리소스를 사용
			m_animator->RegisterAnimation((int)HoundState::WALK, DIR_DOWN, base + L"Hound_hound_run_loop_down.png", 0, 0, 7, 16, px, py, true, 0.03f);
			m_animator->RegisterAnimation((int)HoundState::WALK, DIR_UP, base + L"Hound_hound_run_loop_up.png", 0, 0, 7, 16, px, py, true, 0.03f);
			m_animator->RegisterAnimation((int)HoundState::WALK, DIR_LEFT, walkSidePath, 0, 0, 7, 16, px, py, true, 0.03f, false);
			m_animator->RegisterAnimation((int)HoundState::WALK, DIR_RIGHT, walkSidePath, 0, 0, 7, 16, px, py, true, 0.03f);

			m_animator->RegisterAnimation((int)HoundState::ATTACK_PRE, DIR_DOWN, base + L"Hound_hound_atk_pre_down.png", 0, 0, 7, 29, px, py, false, 0.03f);
			m_animator->RegisterAnimation((int)HoundState::ATTACK_PRE, DIR_UP, base + L"Hound_hound_atk_pre_up.png", 0, 0, 7, 29, px, py, false, 0.03f);
			std::wstring atkPreSidePath = base + L"Hound_hound_atk_pre_side.png";
			m_animator->RegisterAnimation((int)HoundState::ATTACK_PRE, DIR_LEFT, atkPreSidePath, 0, 0, 7, 29, px, py, false, 0.03f, false);
			m_animator->RegisterAnimation((int)HoundState::ATTACK_PRE, DIR_RIGHT, atkPreSidePath, 0, 0, 7, 29, px, py, false, 0.03f);

			m_animator->RegisterAnimation((int)HoundState::ATTACK, DIR_DOWN, base + L"Hound_hound_atk_down.png", 0, 0, 7, 18, px, py, false, 0.03f);
			m_animator->RegisterAnimation((int)HoundState::ATTACK, DIR_UP, base + L"Hound_hound_atk_up.png", 0, 0, 7, 18, px, py, false, 0.03f);
			std::wstring atkSidePath = base + L"Hound_hound_atk_side.png";
			m_animator->RegisterAnimation((int)HoundState::ATTACK, DIR_LEFT, atkSidePath, 0, 0, 7, 18, px, py, false, 0.03f, false);
			m_animator->RegisterAnimation((int)HoundState::ATTACK, DIR_RIGHT, atkSidePath, 0, 0, 7, 18, px, py, false, 0.03f);

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				AnimationClip* clip = m_animator->GetAnimationClip((int)HoundState::ATTACK, (Direction)dir);

				clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
				clip->AddEventFrame(17, L"attack_end"); // 마지막 프레임에 이벤트 등록
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"attack_hit") this->OnAttackHit();
					else if (eventName == L"attack_end") this->OnAttackEnd();
					});

			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)HoundState::HIT, (Direction)dir, base + L"Hound_hound_hit_side.png", 0, 0, 7, 27, px, py, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)HoundState::HIT, (Direction)dir);

				clip->AddEventFrame(26, L"hit_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"hit_end") this->OnHitEnd();
					});

			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)HoundState::DEATH, (Direction)dir, base + L"Hound_hound_death.png", 0, 0, 7, 52, px, py, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)HoundState::DEATH, (Direction)dir);

				clip->AddEventFrame(51, L"death_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"death_end") this->OnDeathEnd();
					});
			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++)
			{
				m_animator->RegisterAnimation((int)HoundState::HOWL, (Direction)dir, base + L"Hound_hound_howl.png", 0, 0, 7, 47, px, py, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)HoundState::HOWL, (Direction)dir);

				clip->AddEventFrame(0, L"howl_start");
				clip->AddEventFrame(46, L"howl_end"); // 47프레임이므로 마지막 인덱스는 46
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"howl_start")
					{
						SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_bark.wav");
						m_hasHowlStarted = true;
					}
					else if (eventName == L"howl_end")
					{
						m_bHasHowled = true;
						ChangeState((int)HoundState::CHASE);
					}
					});
			}
		}
		ChangeState(m_state);
	}

	m_attackCollider = AddComponent<BoxCollider>();
	if (m_attackCollider) {
		UpdateAttackBoxByDirection(DIR_DOWN);
		m_attackCollider->SetColliderEnabled(false);
	}
}

void Hound::RenderDebugOverlay()
{
	Combatant::RenderDebugOverlay();
}

void Hound::UpdateAI(float deltaTime)
{
	if (!IsEnabled() || !m_transform || !m_animator) return;

	if (!m_isCombatEnabled)
	{
		m_attackTarget = nullptr;
		if (!m_hasHowlStarted)
		{
			if (m_state != (int)HoundState::HOWL)
			{
				ChangeState((int)HoundState::HOWL);
			}
			return;
		}

		if (!m_bHasHowled)
		{
			return;
		}

		if (m_state != (int)HoundState::IDLE)
		{
			ChangeState((int)HoundState::IDLE);
		}
		return;
	}

	if (m_state == (int)HoundState::ATTACK_PRE)
	{
		if (m_animator->IsAnimationDone()) ChangeState((int)HoundState::ATTACK);
		return;
	}

	Monster::UpdateAI(deltaTime);

	if (m_state == (int)HoundState::CHASE || m_state == (int)HoundState::WALK) ClampPositionToMapBounds();
}

void Hound::UpdateMovement(float deltaTime)
{
	Monster::UpdateMovement(deltaTime);
}

int Hound::UpdateIdle(float deltaTime)
{
	int nextState = Monster::UpdateIdle(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)HoundState::IDLE : (int)HoundState::HOWL;
	}

	if (nextState == (int)HoundState::CHASE && !m_bHasHowled)
	{
		return (int)HoundState::HOWL;
	}

	if (nextState == (int)HoundState::ATTACK)
	{
		return (int)HoundState::ATTACK_PRE;
	}

	return nextState;
}

int Hound::UpdateWalk(float deltaTime)
{
	int nextState = Monster::UpdateWalk(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)HoundState::IDLE : (int)HoundState::HOWL;
	}

	if (nextState == (int)HoundState::CHASE && !m_bHasHowled)
	{
		return (int)HoundState::HOWL;
	}

	return nextState;
}

int Hound::UpdateChase(float deltaTime)
{
	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)HoundState::IDLE : (int)HoundState::HOWL;
	}

	int nextState = Monster::UpdateChase(deltaTime);

	if (nextState == (int)HoundState::ATTACK)
	{
		return (int)HoundState::ATTACK_PRE;
	}

	return nextState;
}

void Hound::Damaged(int damage)
{
	Monster::Damaged(damage);
	if (!IsDead()) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_hurt.wav");
		ChangeState((int)HoundState::HIT);
		m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
	}
}

void Hound::OnAttackHit()
{
	if (m_isCombatEnabled && m_state == (int)HoundState::ATTACK) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_attack.wav");
		ProcessAttackHit(m_damage);
	}
}

void Hound::OnAttackEnd()
{
	OutputDebugStringW(L"Hound::OnAttackEnd called\n");
	if (m_state != (int)HoundState::ATTACK) return;
	if (m_attackCollider) m_attackCollider->SetColliderEnabled(false);
	ChangeState((int)HoundState::CHASE);
}

void Hound::OnHitEnd()
{
	if (m_state != (int)HoundState::HIT) return;
	ChangeState((int)HoundState::IDLE);
}

void Hound::Die() {
	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_death.wav");

	SetDropItem(GOID_ITEM_MONSTER_MEAT, 1);
	ChangeState((int)HoundState::DEATH);
}

bool Hound::OnInteraction(GameObject* obj) { return Entity::OnInteraction(obj); }

void Hound::SetCombatEnabled(bool enabled)
{
	m_isCombatEnabled = enabled;
	if (!m_isCombatEnabled)
	{
		m_hasHowlStarted = false;
		m_bHasHowled = false;
		m_attackTarget = nullptr;
		m_attackCooldownTimer = 0.0f;
		if (m_attackCollider) m_attackCollider->SetColliderEnabled(false);
	}
	else
	{
		// ALWAYS 어그로 몬스터는 연출 중 타겟을 비웠다면 전투 재개 시 즉시 다시 플레이어를 잡아야 한다.
		if (m_aggroType == AggroType::ALWAYS)
		{
			m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
		}
	}
}
