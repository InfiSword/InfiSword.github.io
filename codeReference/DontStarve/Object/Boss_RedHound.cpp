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
#include "Boss_RedHound.h"

Boss_RedHound::Boss_RedHound(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Hound(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType)
{
	m_hp = 300;
	m_maxHp = m_hp;
	m_bHasHowled = false;
	m_type = GO_TYPE_MONSTER;
	m_walkSpeed = 100.0f;
	m_runSpeed = 250.0f;
	m_attackRange = 100.0f;
	m_attackCooldown = 2.0f;
	m_attackHitFrame = 4;
	m_damage = 25;
	m_attackBoxWidth = 120;
	m_attackBoxHeight = 70;
	m_wanderRadius = 300.0f;
	m_aggroRadius = 400.0f;
	m_deaggroRadius = 600.0f;
	m_idleTimer = 0.0f;
	m_idleDuration = 2.0f;

	m_dashCooldown = 6.0f;
	m_dashCooldownTimer = 0.0f;
	m_dashDistance = 650.0f;
	m_dashSpeed = 1300.0f;
	m_dashDuration = m_dashDistance / m_dashSpeed;
	m_dashRemainingTime = 0.0f;
	m_dashHitProcessed = false;
}

Boss_RedHound::~Boss_RedHound() {}

void Boss_RedHound::Init()
{
	Monster::Init();
	m_bUseSuperArmor = true;
	SetupAggro(AggroType::ALWAYS, 0.0f, 0.0f);
	SetupAttackBox(m_attackBoxWidth, m_attackBoxHeight,0,-40.f);

	ChangeState((int)BossRedHoundState::IDLE);
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
			std::wstring prefix = L"RedHound_";
			std::wstring houndPrefix = prefix + L"hound_";

			m_animator->RegisterAnimation((int)BossRedHoundState::IDLE, DIR_DOWN, base + houndPrefix + L"idle_down.png", 0, 0, 7, 20, objData->pivotX, objData->pivotY, true, 0.02f);
			m_animator->RegisterAnimation((int)BossRedHoundState::IDLE, DIR_UP, base + houndPrefix + L"idle_up.png", 0, 0, 7, 20, objData->pivotX, objData->pivotY, true, 0.02f);
			std::wstring idleSidePath = base + houndPrefix + L"idle_side.png";
			m_animator->RegisterAnimation((int)BossRedHoundState::IDLE, DIR_LEFT, idleSidePath, 0, 0, 7, 20, objData->pivotX, objData->pivotY, true, 0.02f, false);
			m_animator->RegisterAnimation((int)BossRedHoundState::IDLE, DIR_RIGHT, idleSidePath, 0, 0, 7, 20, objData->pivotX, objData->pivotY, true, 0.02f);

			for (int state = (int)BossRedHoundState::WALK; state <= (int)BossRedHoundState::CHASE; ++state) {
				if (state != (int)BossRedHoundState::WALK && state != (int)BossRedHoundState::CHASE) continue;
				m_animator->RegisterAnimation(state, DIR_DOWN, base + houndPrefix + L"run_loop_down.png", 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f);
				m_animator->RegisterAnimation(state, DIR_UP, base + houndPrefix + L"run_loop_up.png", 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f);
				std::wstring walkSidePath = base + houndPrefix + L"run_loop_side.png";
				m_animator->RegisterAnimation(state, DIR_LEFT, walkSidePath, 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f, false);
				m_animator->RegisterAnimation(state, DIR_RIGHT, walkSidePath, 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f);
			}

			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK_PRE, DIR_DOWN, base + houndPrefix + L"atk_pre_down.png", 0, 0, 7, 29, objData->pivotX, objData->pivotY, false, 0.03f);
			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK_PRE, DIR_UP, base + houndPrefix + L"atk_pre_up.png", 0, 0, 7, 29, objData->pivotX, objData->pivotY, false, 0.03f);
			std::wstring atkPreSidePath = base + houndPrefix + L"atk_pre_side.png";
			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK_PRE, DIR_LEFT, atkPreSidePath, 0, 0, 7, 29, objData->pivotX, objData->pivotY, false, 0.03f, false);
			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK_PRE, DIR_RIGHT, atkPreSidePath, 0, 0, 7, 29, objData->pivotX, objData->pivotY, false, 0.03f);

			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK, DIR_DOWN, base + houndPrefix + L"atk_down.png", 0, 0, 7, 18, objData->pivotX, objData->pivotY, false, 0.03f);
			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK, DIR_UP, base + houndPrefix + L"atk_up.png", 0, 0, 7, 18, objData->pivotX, objData->pivotY, false, 0.03f);
			std::wstring atkSidePath = base + houndPrefix + L"atk_side.png";
			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK, DIR_LEFT, atkSidePath, 0, 0, 7, 18, objData->pivotX, objData->pivotY, false, 0.03f, false);
			m_animator->RegisterAnimation((int)BossRedHoundState::ATTACK, DIR_RIGHT, atkSidePath, 0, 0, 7, 18, objData->pivotX, objData->pivotY, false, 0.03f);

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossRedHoundState::ATTACK, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
					clip->AddEventFrame(17, L"attack_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"attack_hit") this->OnAttackHit();
						else if (eventName == L"attack_end") this->OnAttackEnd();
						});
				}
			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossRedHoundState::HIT, (Direction)dir, base + houndPrefix + L"hit_side.png", 0, 0, 7, 27, objData->pivotX, objData->pivotY, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossRedHoundState::HIT, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(26, L"hit_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"hit_end") this->OnHitEnd();
						});
				}
			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossRedHoundState::DEATH, (Direction)dir, base + houndPrefix + L"death.png", 0, 0, 7, 52, objData->pivotX, objData->pivotY, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossRedHoundState::DEATH, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(51, L"death_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"death_end") this->OnDeathEnd();
						});
				}
			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossRedHoundState::HOWL, (Direction)dir, base + houndPrefix + L"howl.png", 0, 0, 7, 47, objData->pivotX, objData->pivotY, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossRedHoundState::HOWL, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(0, L"howl_start");
					clip->AddEventFrame(46, L"howl_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"howl_start") {
							SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_bark.wav");
							m_hasHowlStarted = true;
						}
						else if (eventName == L"howl_end") {
							m_bHasHowled = true;
							m_bCanChase = false;
							if (m_bCanChase) ChangeState((int)BossRedHoundState::CHASE);
							else ChangeState((int)BossRedHoundState::IDLE);
						}
						});
				}
			}

			// DASH_PRE: Howl 애니메이션 사용
			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossRedHoundState::DASH_PRE, (Direction)dir, base + houndPrefix + L"howl.png", 0, 0, 7, 47, objData->pivotX, objData->pivotY, false, 0.02f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossRedHoundState::DASH_PRE, (Direction)dir);
				if (clip) {
					// 30 프레임 즈음에 플레이어 방향을 고정 (하울링 도중)
					clip->AddEventFrame(36, L"dash_lock");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"dash_lock") {
							m_dashDir = m_dirToPlayer;
							LookAtPlayer();
						}
						});
				}
			}

			// DASH: 돌진 애니메이션 (각 방향에 맞는 달리기 애니메이션 등록)
			m_animator->RegisterAnimation((int)BossRedHoundState::DASH, DIR_DOWN, base + houndPrefix + L"run_loop_down.png", 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f);
			m_animator->RegisterAnimation((int)BossRedHoundState::DASH, DIR_UP, base + houndPrefix + L"run_loop_up.png", 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f);
			std::wstring dashSidePath = base + houndPrefix + L"run_loop_side.png";
			m_animator->RegisterAnimation((int)BossRedHoundState::DASH, DIR_LEFT, dashSidePath, 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f, false);
			m_animator->RegisterAnimation((int)BossRedHoundState::DASH, DIR_RIGHT, dashSidePath, 0, 0, 7, 16, objData->pivotX, objData->pivotY, true, 0.02f);
		}
		ChangeState(m_state);
	}
}

void Boss_RedHound::RenderDebugOverlay()
{
	Combatant::RenderDebugOverlay();
}

bool Boss_RedHound::OnInteraction(GameObject* obj) { return Entity::OnInteraction(obj); }

void Boss_RedHound::UpdateAI(float deltaTime)
{
	if (!IsEnabled() || !m_transform || !m_animator) return;

	m_dashCooldownTimer = (std::max)(0.0f, m_dashCooldownTimer - deltaTime);

	if (!m_isCombatEnabled)
	{
		m_attackTarget = nullptr;
		m_dashRemainingTime = 0.0f;
		m_dashHitProcessed = false;
		if (m_attackCollider) m_attackCollider->SetColliderEnabled(false);
		if (!m_hasHowlStarted)
		{
			if (m_state != (int)BossRedHoundState::HOWL)
			{
				ChangeState((int)BossRedHoundState::HOWL);
			}
			return;
		}

		if (!m_bHasHowled)
		{
			return;
		}

		if (m_state != (int)BossRedHoundState::IDLE)
		{
			ChangeState((int)BossRedHoundState::IDLE);
		}
		return;
	}

	if (m_state == (int)BossRedHoundState::HOWL)
	{
		return;
	}

	if (m_state == (int)BossRedHoundState::DASH_PRE)
	{
		if (m_animator->IsAnimationDone()) {
			// 이미 dash_lock 이벤트에서 m_dashDir와 캐릭터 방향이 설정되었으므로 바로 전환
			m_dashRemainingTime = m_dashDuration;
			m_dashHitProcessed = false;
			SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Redhound_DashSound.wav");
			ChangeState((int)BossRedHoundState::DASH);
		}
		return;
	}

	if (m_state == (int)BossRedHoundState::DASH)
	{
		m_dashRemainingTime -= deltaTime;

		// 대쉬 중에는 공격 콜라이더 활성화
		if (m_attackCollider) {
			// 대쉬 방향에 맞춰 공격 박스 업데이트
			UpdateAttackBoxByDirection(m_transform->GetDirection());
			m_attackCollider->SetColliderEnabled(true);

			// 아직 데미지를 입히지 않았다면 충돌 체크 및 데미지 적용
			if (!m_dashHitProcessed) {
				if (ApplyAttackDamageToTarget((int)(m_damage * 1.5f))) {
					// 한번이라도 데미지를 입혔으면 해당 대쉬에서는 더 이상 처리 안함
					m_dashHitProcessed = true;
				}
			}
		}

		if (m_dashRemainingTime <= 0.0f) {
			if (m_attackCollider) m_attackCollider->SetColliderEnabled(false);
			m_dashCooldownTimer = m_dashCooldown;
			ChangeState((int)BossRedHoundState::CHASE);
		}
		return;
	}

	if (m_state == (int)BossRedHoundState::ATTACK_PRE)
	{
		if (m_animator->IsAnimationDone()) {
			LookAtPlayer();
			ChangeState((int)BossRedHoundState::ATTACK);
		}
		return;
	}

	Monster::UpdateAI(deltaTime);

	if (m_state == (int)BossRedHoundState::CHASE || m_state == (int)BossRedHoundState::WALK) ClampPositionToMapBounds();
}

void Boss_RedHound::UpdateMovement(float deltaTime)
{
	if (m_state == (int)BossRedHoundState::DASH) {
		float moveDist = m_dashSpeed * deltaTime;
		m_transform->SetPosition(m_transform->GetX() + m_dashDir.X * moveDist, m_transform->GetY() + m_dashDir.Y * moveDist);
		ClampPositionToMapBounds();
		return;
	}

	if (m_state == (int)BossRedHoundState::DASH_PRE) return;

	Monster::UpdateMovement(deltaTime);
}

int Boss_RedHound::UpdateIdle(float deltaTime)
{
	int nextState = Monster::UpdateIdle(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)BossRedHoundState::IDLE : (int)BossRedHoundState::HOWL;
	}

	if (nextState == (int)BossRedHoundState::CHASE && !m_bHasHowled)
	{
		return (int)BossRedHoundState::HOWL;
	}

	if (nextState == (int)BossRedHoundState::ATTACK)
	{
		LookAtPlayer();
		return (int)BossRedHoundState::ATTACK_PRE;
	}

	return nextState;
}

int Boss_RedHound::UpdateWalk(float deltaTime)
{
	int nextState = Monster::UpdateWalk(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)BossRedHoundState::IDLE : (int)BossRedHoundState::HOWL;
	}

	if (nextState == (int)BossRedHoundState::CHASE && !m_bHasHowled)
	{
		return (int)BossRedHoundState::HOWL;
	}

	return nextState;
}

int Boss_RedHound::UpdateChase(float deltaTime)
{
	if (!m_isCombatEnabled)
	{
		m_dashRemainingTime = 0.0f;
		m_dashHitProcessed = false;
		return m_bHasHowled ? (int)BossRedHoundState::IDLE : (int)BossRedHoundState::HOWL;
	}

	int nextState = Monster::UpdateChase(deltaTime);

	// 공격 거리 밖이고 쿨타임이 찼을 때만 대쉬 고려
	if (nextState == (int)BossRedHoundState::CHASE && m_dashCooldownTimer <= 0.0f) {
		return (int)BossRedHoundState::DASH_PRE;

	}
	if (nextState == (int)BossRedHoundState::ATTACK)
	{
		LookAtPlayer();
		return (int)BossRedHoundState::ATTACK_PRE;
	}

	return nextState;
}

void Boss_RedHound::Damaged(int damage)
{
	Monster::Damaged(damage);
	if (IsDead()) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_death.wav");
		return;
	}

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_hurt.wav");

	if (CheckSuperArmorHit()) return;

	ChangeState((int)BossRedHoundState::HIT);
}

void Boss_RedHound::OnAttackHit()
{
	if (!m_isCombatEnabled || m_state != (int)BossRedHoundState::ATTACK) return;

	LookAtPlayer();

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_attack.wav");
	ProcessAttackHit(m_damage);
}

void Boss_RedHound::OnAttackEnd()
{
	if (m_state != (int)BossRedHoundState::ATTACK) return;

	HandleAttackEndSuperArmor();

	ChangeState((int)BossRedHoundState::CHASE);
}

void Boss_RedHound::OnHitEnd()
{
	if (m_state != (int)BossRedHoundState::HIT) return;
	ChangeState((int)BossRedHoundState::IDLE);
}

void Boss_RedHound::Die() {
	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_death.wav");
	ChangeState((int)BossRedHoundState::DEATH);
}
