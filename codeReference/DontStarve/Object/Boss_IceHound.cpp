#include "99_Default/pch.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/SoundManager/SoundManager.h"
#include "../../../03_Animation/Animator.h"
#include "../../../03_Animation/AnimationClip.h"
#include "../Player/Player.h"
#include "../../Component/Sprite/SpriteSheet.h"
#include "../../Component/Transform/Transform.h"
#include "../../Component/Collider/BoxCollider.h"
#include "../../Skill/IceProjectile.h"
#include "Boss_IceHound.h"


Boss_IceHound::Boss_IceHound(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
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
	m_damage = 30;
	m_attackBoxWidth = 100;
	m_attackBoxHeight = 60;
	m_wanderRadius = 300.0f;
	m_aggroRadius = 400.0f;
	m_deaggroRadius = 600.0f;
	m_idleTimer = 0.0f;
	m_idleDuration = 2.0f;
	m_projectileCooldown = 3.0f;
	m_projectileCooldownTimer = 0.0f;
	m_projectileSpeed = 430.0f;
	m_projectileRange = 520.0f;
	m_projectileAttackRange = 250.0f;
	m_retreatSpeed = 220.0f;
	m_retreatBeforeShotDuration = 0.25f;
	m_retreatBeforeShotTimer = 0.0f;
	m_retreatThenShootPending = false;
	m_projectileDamage = 20;
}

Boss_IceHound::~Boss_IceHound() {}

void Boss_IceHound::Init()
{
	Monster::Init();

	SetupAggro(AggroType::ALWAYS, 0.0f, 0.0f);
	SetupAttackBox(m_attackBoxWidth, m_attackBoxHeight,0,-40.f);

	ChangeState((int)BossIceHoundState::IDLE);
	m_idleTimer = 0.0f;
	m_idleDuration = 2.0f + (rand() / (float)RAND_MAX) * 3.0f;

	m_bUseSuperArmor = true;
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
			std::wstring prefix = L"IceHound_";
			std::wstring houndPrefix = prefix + L"hound_";
			float px = objData->pivotX;
			float py = objData->pivotY;

			m_animator->RegisterAnimation((int)BossIceHoundState::IDLE, DIR_DOWN, base + houndPrefix + L"idle_down.png", 0, 0, 7, 20, px, py, true, 0.03f);
			m_animator->RegisterAnimation((int)BossIceHoundState::IDLE, DIR_UP, base + houndPrefix + L"idle_up.png", 0, 0, 7, 20, px, py, true, 0.03f);
			std::wstring idleSidePath = base + houndPrefix + L"idle_side.png";
			m_animator->RegisterAnimation((int)BossIceHoundState::IDLE, DIR_LEFT, idleSidePath, 0, 0, 7, 20, px, py, true, 0.03f, false);
			m_animator->RegisterAnimation((int)BossIceHoundState::IDLE, DIR_RIGHT, idleSidePath, 0, 0, 7, 20, px, py, true, 0.03f);

			for (int state = (int)BossIceHoundState::WALK; state <= (int)BossIceHoundState::CHASE; ++state) {
				if (state != (int)BossIceHoundState::WALK && state != (int)BossIceHoundState::CHASE) continue;
				m_animator->RegisterAnimation(state, DIR_DOWN, base + houndPrefix + L"run_loop_down.png", 0, 0, 7, 16, px, py, true, 0.03f);
				m_animator->RegisterAnimation(state, DIR_UP, base + houndPrefix + L"run_loop_up.png", 0, 0, 7, 16, px, py, true, 0.03f);
				std::wstring walkSidePath = base + houndPrefix + L"run_loop_side.png";
				m_animator->RegisterAnimation(state, DIR_LEFT, walkSidePath, 0, 0, 7, 16, px, py, true, 0.03f, false);
				m_animator->RegisterAnimation(state, DIR_RIGHT, walkSidePath, 0, 0, 7, 16, px, py, true, 0.03f);
			}

			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK_PRE, DIR_DOWN, base + houndPrefix + L"atk_pre_down.png", 0, 0, 7, 29, px, py, false, 0.02f);
			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK_PRE, DIR_UP, base + houndPrefix + L"atk_pre_up.png", 0, 0, 7, 29, px, py, false, 0.02f);
			std::wstring atkPreSidePath = base + houndPrefix + L"atk_pre_side.png";
			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK_PRE, DIR_LEFT, atkPreSidePath, 0, 0, 7, 29, px, py, false, 0.02f, false);
			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK_PRE, DIR_RIGHT, atkPreSidePath, 0, 0, 7, 29, px, py, false, 0.02f);

			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK, DIR_DOWN, base + houndPrefix + L"atk_down.png", 0, 0, 7, 18, px, py, false, 0.03f);
			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK, DIR_UP, base + houndPrefix + L"atk_up.png", 0, 0, 7, 18, px, py, false, 0.03f);
			std::wstring atkSidePath = base + houndPrefix + L"atk_side.png";
			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK, DIR_LEFT, atkSidePath, 0, 0, 7, 18, px, py, false, 0.03f, false);
			m_animator->RegisterAnimation((int)BossIceHoundState::ATTACK, DIR_RIGHT, atkSidePath, 0, 0, 7, 18, px, py, false, 0.03f);

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossIceHoundState::ATTACK, (Direction)dir);

				clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
				clip->AddEventFrame(17, L"attack_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"attack_hit") this->OnAttackHit();
					else if (eventName == L"attack_end") this->OnAttackEnd();
					});

			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossIceHoundState::HIT, (Direction)dir, base + houndPrefix + L"hit_side.png", 0, 0, 7, 27, px, py, false, 0.02f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossIceHoundState::HIT, (Direction)dir);

				clip->AddEventFrame(26, L"hit_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"hit_end") this->OnHitEnd();
					});

			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossIceHoundState::DEATH, (Direction)dir, base + houndPrefix + L"death.png", 0, 0, 7, 52, px, py, false, 0.02f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossIceHoundState::DEATH, (Direction)dir);
				clip->AddEventFrame(51, L"death_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"death_end") this->OnDeathEnd();
					});
			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)BossIceHoundState::HOWL, (Direction)dir, base + houndPrefix + L"howl.png", 0, 0, 7, 47, px, py, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)BossIceHoundState::HOWL, (Direction)dir);
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
						ChangeState((int)BossIceHoundState::IDLE);
					}
					});
			}
		}

		ChangeState(m_state);
	}

	// 발사체 5개 미리 생성하여 풀링
	for (int i = 0; i < 5; ++i) {
		IceProjectile* projectile = new IceProjectile();
		projectile->Init();
		projectile->SetActive(false); // 초기에는 비활성화 상태
		ObjectManager::GetInstance()->AddGameObject(projectile);
		m_projectiles.push_back(projectile);
	}
}

void Boss_IceHound::RenderDebugOverlay()
{
	Combatant::RenderDebugOverlay();
}

bool Boss_IceHound::OnInteraction(GameObject* obj) { return Entity::OnInteraction(obj); }

void Boss_IceHound::UpdateAI(float deltaTime)
{
	if (!IsEnabled() || !m_transform || !m_animator) return;
	m_projectileCooldownTimer = (std::max)(0.0f, m_projectileCooldownTimer - deltaTime);
	if (m_retreatThenShootPending)
		m_retreatBeforeShotTimer = (std::max)(0.0f, m_retreatBeforeShotTimer - deltaTime);

	if (!m_isCombatEnabled)
	{
		m_attackTarget = nullptr;
		m_retreatThenShootPending = false;
		m_retreatBeforeShotTimer = 0.0f;
		if (!m_hasHowlStarted)
		{
			if (m_state != (int)BossIceHoundState::HOWL)
			{
				ChangeState((int)BossIceHoundState::HOWL);
			}
			return;
		}

		if (!m_bHasHowled)
		{
			return;
		}

		if (m_state != (int)BossIceHoundState::IDLE)
		{
			ChangeState((int)BossIceHoundState::IDLE);
		}
		return;
	}

	if (m_state == (int)BossIceHoundState::HOWL)
	{
		// HOWL은 애니메이션 이벤트(howl_end)에서 상태 전이가 이루어짐
		return;
	}
	if (m_state == (int)BossIceHoundState::ATTACK_PRE)
	{
		if (m_animator->IsAnimationDone()) {
			LookAtPlayer();
			ChangeState((int)BossIceHoundState::ATTACK);
		}
		return;
	}

	Monster::UpdateAI(deltaTime);

	if (m_state == (int)BossIceHoundState::CHASE || m_state == (int)BossIceHoundState::WALK) ClampPositionToMapBounds();
}

void Boss_IceHound::UpdateMovement(float deltaTime)
{
	// 투사체 준비 완료 + 사거리 밖이면 잠깐 후진하여 간격을 벌린 뒤 발사 준비
	if (m_state == (int)BossIceHoundState::CHASE && m_retreatThenShootPending)
	{
		MoveAwayFromPlayer(deltaTime, m_retreatSpeed);
		return;
	}

	Monster::UpdateMovement(deltaTime);
}

int Boss_IceHound::UpdateIdle(float deltaTime)
{
	int nextState = Monster::UpdateIdle(deltaTime);
	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)BossIceHoundState::IDLE : (int)BossIceHoundState::HOWL;
	}

	if (nextState == (int)BossIceHoundState::CHASE && !m_bHasHowled)
	{
		return (int)BossIceHoundState::HOWL;
	}
	if (nextState == (int)BossIceHoundState::ATTACK)
	{
		LookAtPlayer();
		return (int)BossIceHoundState::ATTACK_PRE; // 근접 공격 유지
	}
	if (CanStartProjectileAttack())
	{
		LookAtPlayer();
		return (int)BossIceHoundState::ATTACK_PRE; // 원거리 공격
	}

	return nextState;
}

int Boss_IceHound::UpdateWalk(float deltaTime)
{
	int nextState = Monster::UpdateWalk(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasHowled ? (int)BossIceHoundState::IDLE : (int)BossIceHoundState::HOWL;
	}

	if (nextState == (int)BossIceHoundState::CHASE && !m_bHasHowled)
	{
		return (int)BossIceHoundState::HOWL;
	}
	if (CanStartProjectileAttack())
	{
		LookAtPlayer();
		return (int)BossIceHoundState::ATTACK_PRE;
	}

	return nextState;
}

int Boss_IceHound::UpdateChase(float deltaTime)
{
	if (!m_isCombatEnabled)
	{
		m_retreatThenShootPending = false;
		m_retreatBeforeShotTimer = 0.0f;
		return m_bHasHowled ? (int)BossIceHoundState::IDLE : (int)BossIceHoundState::HOWL;
	}

	if (m_retreatThenShootPending)
	{
		// 플레이어가 너무 가까워지면 즉시 근접/투사체 공격으로 전환
		if (m_distToPlayerSq <= (m_attackRange * m_attackRange))
		{
			m_retreatThenShootPending = false;
			m_retreatBeforeShotTimer = 0.0f;
			return (int)BossIceHoundState::ATTACK_PRE;
		}

		if (m_retreatBeforeShotTimer <= 0.0f)
		{
			m_retreatThenShootPending = false;
			return (int)BossIceHoundState::ATTACK_PRE;
		}

		return (int)BossIceHoundState::CHASE;
	}

	int nextState = Monster::UpdateChase(deltaTime);
	if (nextState == (int)BossIceHoundState::ATTACK)
	{
		LookAtPlayer();
		return (int)BossIceHoundState::ATTACK_PRE; // 근접 공격 유지
	}
	if (CanStartProjectileAttack())
	{
		LookAtPlayer();
		return (int)BossIceHoundState::ATTACK_PRE;
	}

	// 투사체 쿨다운이 끝났는데 플레이어가 발사 사거리보다 멀면, 후진 후 발사 로직 시작
	if (m_projectileCooldownTimer <= 0.0f && m_distToPlayerSq > (m_projectileAttackRange * m_projectileAttackRange))
	{
		m_retreatThenShootPending = true;
		m_retreatBeforeShotTimer = m_retreatBeforeShotDuration;
		return (int)BossIceHoundState::CHASE;
	}

	return nextState;
}

void Boss_IceHound::Damaged(int damage)
{
	Monster::Damaged(damage);
	if (IsDead()) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_death.wav");
		return;
	}

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_hurt.wav");

	if (CheckSuperArmorHit()) return;

	ChangeState((int)BossIceHoundState::HIT);
}

void Boss_IceHound::OnAttackHit()
{
	if (!m_isCombatEnabled) return;
	if (m_state != (int)BossIceHoundState::ATTACK) return;

	LookAtPlayer();

	// 발사체 쿨타임이 완료되었다면 무조건 발사
	if (m_projectileCooldownTimer <= 0.0f) {
		FireIceProjectile();
		m_projectileCooldownTimer = m_projectileCooldown;
		m_retreatThenShootPending = false;
		m_retreatBeforeShotTimer = 0.0f;
	}

	if (m_distToPlayerSq <= (m_attackRange * m_attackRange)) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Hound_attack.wav");
		ProcessAttackHit(m_damage); // 근접 공격 처리
	}
}

void Boss_IceHound::OnAttackEnd()
{
	if (m_state != (int)BossIceHoundState::ATTACK) return;

	HandleAttackEndSuperArmor();

	ChangeState((int)BossIceHoundState::CHASE);
}

void Boss_IceHound::OnHitEnd()
{
	if (m_state != (int)BossIceHoundState::HIT) return;
	ChangeState((int)BossIceHoundState::IDLE);
}

void Boss_IceHound::Die() { ChangeState((int)BossIceHoundState::DEATH); }

void Boss_IceHound::FireIceProjectile()
{
	if (!m_attackTarget || !m_attackTarget->IsEnabled()) return;

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/HoundSound/Icehound_SkillSound.wav");

	float dx, dy;
	Transform* targetTr = m_attackTarget->GetComponent<Transform>();
	if (targetTr) {
		dx = targetTr->GetX() - m_transform->GetX();
		dy = targetTr->GetY() - m_transform->GetY();
	}


	IceProjectile* projectile = nullptr;
	for (IceProjectile* p : m_projectiles) {
		if (p && !p->IsEnabled()) {
			projectile = p;
			break;
		}
	}

	if (!projectile) {
		projectile = new IceProjectile();
		projectile->Init();
		ObjectManager::GetInstance()->AddGameObject(projectile);
		m_projectiles.push_back(projectile);
	}

	const float spawnOffset = 30.0f;
	float x = m_transform->GetX();
	float y = m_transform->GetY() - spawnOffset;

	projectile->Fire(x, y, dx, dy, m_projectileDamage, m_projectileSpeed, m_projectileRange);
}

bool Boss_IceHound::IsPlayerInProjectileRange() const
{
	if (!m_attackTarget || !m_attackTarget->IsEnabled()) return false;
	const float rangeSq = m_projectileAttackRange * m_projectileAttackRange;
	return m_distToPlayerSq <= rangeSq;
}

bool Boss_IceHound::CanStartProjectileAttack() const
{
	if (m_state == (int)BossIceHoundState::HIT || m_state == (int)BossIceHoundState::DEATH) return false;
	if (m_projectileCooldownTimer > 0.0f) return false;
	return IsPlayerInProjectileRange();
}

void Boss_IceHound::MoveAwayFromPlayer(float deltaTime, float speed)
{
	if (!m_transform) return;

	// 플레이어 방향 반대로 이동
	float moveX = -m_dirToPlayer.X;
	float moveY = -m_dirToPlayer.Y;
	float moveDist = speed * deltaTime;
	m_transform->SetPosition(m_transform->GetX() + moveX * moveDist, m_transform->GetY() + moveY * moveDist);
	ClampPositionToMapBounds();

	// 바라보는 방향은 플레이어 쪽으로 유지(공격 애니메이션 연결 자연스럽게)
	LookAtPlayer();
}