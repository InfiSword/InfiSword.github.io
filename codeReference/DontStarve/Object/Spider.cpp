#include "99_Default/pch.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../01_Manager/SoundManager/SoundManager.h"
#include "../../../03_Animation/Animator.h"
#include "../../../03_Animation/AnimationClip.h"
#include "../Player/Player.h"
#include "../../Component/Sprite/SpriteSheet.h"
#include "../../Component/Transform/Transform.h"
#include "../../Component/Collider/BoxCollider.h"
#include "../../Building/SpiderEgg.h"
#include "Spider.h"

Spider::Spider(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Monster(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType)
	, m_homeEgg(nullptr)
	, m_spawnRadius(200.0f)
	, m_bHasTaunted(false)
{
	m_type = GO_TYPE_MONSTER;
	m_walkSpeed = 60.0f;
	m_runSpeed = 150.0f;
	m_attackRange = 80.0f;
	m_attackCooldown = 1.2f;
	m_attackHitFrame = 45;
	m_damage = 15;
	m_attackBoxWidth = 90;
	m_attackBoxHeight = 60;
}

Spider::~Spider() {}

void Spider::Init()
{
	Monster::Init();
	SetupAggro(AggroType::ON_RANGE, 300.0f, 500.0f);
	SetupAttackBox(m_attackBoxWidth, m_attackBoxHeight,0, -50.f);

	ChangeState((int)SpiderState::IDLE);
	m_idleTimer = 0.0f;
	m_idleDuration = 2.0f + (rand() / (float)RAND_MAX) * 3.0f;
	m_bHasTaunted = false;

	if (this->m_transform) {
		m_targetX = this->m_transform->GetX();
		m_targetY = this->m_transform->GetY();
	}

	if (!m_animator) m_animator = AddComponent<Animator>(m_spriteRenderer);
	DataManager* pRM = DataManager::GetInstance();

	bool isWarrior = (m_id == GOID_MONSTER_WARRIOR_SPIDER);
	m_hp = isWarrior ? 200 : 80;
	m_maxHp = m_hp;
	m_walkSpeed = isWarrior ? 80.0f : 60.0f;
	m_runSpeed = isWarrior ? 180.0f : 150.0f;
	m_attackRange = isWarrior ? 100.0f : 80.0f;
	m_attackBoxWidth = isWarrior ? 82 : 70;
	m_attackBoxHeight = isWarrior ? 92 : 80;

	const ResourcePathUtils::ObjectResourceDef* objData = pRM->GetObjectResourceInfo(m_id);
	if (objData)
	{
		std::wstring base = objData->baseDir + L"\\";
		std::wstring prefix = isWarrior ? L"Warrior_spider_" : L"Spider_spider_";

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) m_animator->RegisterAnimation((int)SpiderState::IDLE, (Direction)dir, base + prefix + L"idle_01.png", 0, 0, 1, 1, objData->pivotX, objData->pivotY, true, 0.05f);

		m_animator->RegisterAnimation((int)SpiderState::WALK, DIR_DOWN, base + prefix + L"walk_loop_down.png", 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.02f);
		m_animator->RegisterAnimation((int)SpiderState::WALK, DIR_UP, base + prefix + L"walk_loop_up.png", 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.02f);
		std::wstring walkSidePath = base + prefix + L"walk_loop_side.png";
		m_animator->RegisterAnimation((int)SpiderState::WALK, DIR_LEFT, walkSidePath, 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.02f, false);
		m_animator->RegisterAnimation((int)SpiderState::WALK, DIR_RIGHT, walkSidePath, 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.02f);

		m_animator->RegisterAnimation((int)SpiderState::CHASE, DIR_DOWN, base + prefix + L"walk_loop_down.png", 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.015f); // 더 빠른 프레임
		m_animator->RegisterAnimation((int)SpiderState::CHASE, DIR_UP, base + prefix + L"walk_loop_up.png", 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.015f);
		m_animator->RegisterAnimation((int)SpiderState::CHASE, DIR_LEFT, walkSidePath, 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.015f, false);
		m_animator->RegisterAnimation((int)SpiderState::CHASE, DIR_RIGHT, walkSidePath, 0, 0, 7, 35, objData->pivotX, objData->pivotY, true, 0.015f);

		if (!isWarrior)
		{
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_DOWN, base + prefix + L"atk_down.png", 0, 0, 7, 71, objData->pivotX, objData->pivotY, false, 0.02f);
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_UP, base + prefix + L"atk_up.png", 0, 0, 7, 71, objData->pivotX, objData->pivotY, false, 0.02f);
			std::wstring attackSidePath = base + prefix + L"atk_side.png";
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_LEFT, attackSidePath, 0, 0, 7, 71, objData->pivotX, objData->pivotY, false, 0.02f, false);
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_RIGHT, attackSidePath, 0, 0, 7, 71, objData->pivotX, objData->pivotY, false, 0.02f);
		}
		else
		{
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_DOWN, base + prefix + L"atk_down.png", 0, 0, 7, 57, objData->pivotX, objData->pivotY, false, 0.02f);
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_UP, base + prefix + L"atk_up.png", 0, 0, 7, 57, objData->pivotX, objData->pivotY, false, 0.02f);
			std::wstring attackSidePath = base + prefix + L"atk_side.png";
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_LEFT, attackSidePath, 0, 0, 7, 57, objData->pivotX, objData->pivotY, false, 0.02f, false);
			m_animator->RegisterAnimation((int)SpiderState::ATTACK, DIR_RIGHT, attackSidePath, 0, 0, 7, 57, objData->pivotX, objData->pivotY, false, 0.02f);

		}

		if (!isWarrior)
		{
			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderState::ATTACK, (Direction)dir);

				clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
				clip->AddEventFrame(70, L"attack_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"attack_hit") this->OnAttackHit();
					else if (eventName == L"attack_end") this->OnAttackEnd();
					});
			}

		}
		else {
			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderState::ATTACK, (Direction)dir);

				clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
				clip->AddEventFrame(56, L"attack_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"attack_hit") this->OnAttackHit();
					else if (eventName == L"attack_end") this->OnAttackEnd();
					});
			}
		}

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderState::HIT, (Direction)dir, base + prefix + L"hit.png", 0, 0, 7, 34, objData->pivotX, objData->pivotY, false, 0.02f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderState::HIT, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(33, L"hit_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"hit_end") this->OnHitEnd();
					});
			}
		}

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderState::DEATH, (Direction)dir, base + prefix + L"death.png", 0, 0, 7, 56, objData->pivotX, objData->pivotY, false, 0.02f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderState::DEATH, (Direction)dir);

			clip->AddEventFrame(55, L"death_end");
			clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
				if (eventName == L"death_end") this->OnDeathEnd();
				});

		}

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderState::TAUNT, (Direction)dir, base + prefix + L"taunt.png", 0, 0, 7, 64, objData->pivotX, objData->pivotY, false, 0.01f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderState::TAUNT, (Direction)dir);

			clip->AddEventFrame(63, L"taunt_end");
			clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
				if (eventName == L"taunt_end")
				{
					m_bHasTaunted = true;
					ChangeState((int)SpiderState::CHASE);
				}
				});

		}

		ChangeState(m_state);
	}

	m_attackCollider = AddComponent<BoxCollider>();
	if (m_attackCollider) {
		UpdateAttackBoxByDirection(DIR_DOWN);
		m_attackCollider->SetColliderEnabled(false);

	}
}

void Spider::RenderDebugOverlay()
{
	Combatant::RenderDebugOverlay();
}

void Spider::SetHomeEgg(SpiderEgg* egg, float spawnRadius)
{
	m_homeEgg = egg;
	m_spawnRadius = spawnRadius;
	m_wanderRadius = spawnRadius;
}

void Spider::SetAggroTarget(GameObject* target)
{
	if (target && target->IsEnabled())
	{
		m_bCanChase = true;
		m_attackTarget = target;

		if (!m_bHasTaunted)
		{
			SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_scream.wav");
			ChangeState((int)SpiderState::TAUNT);
		}

		Transform* targetTr = target->GetComponent<Transform>();
		if (targetTr && m_transform) {
			float dx = targetTr->GetX() - m_transform->GetX();
			float dy = targetTr->GetY() - m_transform->GetY();
			Direction newDir = ResolveFacingDirection({ dx, dy });
			m_transform->SetDirection(newDir);
		}
	}
}

void Spider::ResetAggroSession()
{
	m_bHasTaunted = false;
}

void Spider::ResolveWanderCenter(float& outX, float& outY) const
{
	if (m_homeEgg && m_homeEgg->IsEnabled()) {
		Transform* eggTr = m_homeEgg->GetComponent<Transform>();
		if (eggTr) {
			outX = eggTr->GetX();
			outY = eggTr->GetY();
			return;
		}
	}

	Monster::ResolveWanderCenter(outX, outY);
}

void Spider::UpdateAI(float deltaTime)
{
	if (!IsEnabled() || !m_transform || !m_animator) return;

	// 거미집 소속에 따른 추격 가능 여부 체크
	bool canChaseNow = m_bCanChase || !m_homeEgg || !m_homeEgg->IsEnabled();
	// Monster의 m_bCanChase와 동기화
	SetCanChase(canChaseNow);

	// 기본 Monster::UpdateAI 호출
	Monster::UpdateAI(deltaTime);

	if (m_state == (int)SpiderState::CHASE || m_state == (int)SpiderState::WALK) ClampPositionToMapBounds();
}

void Spider::UpdateMovement(float deltaTime)
{
	// Monster의 기본 이동 로직 사용
	Monster::UpdateMovement(deltaTime);
}

int Spider::UpdateIdle(float deltaTime)
{
	int nextState = Monster::UpdateIdle(deltaTime);

	// CHASE로 전환될 때 도발(TAUNT) 체크
	if (nextState == (int)SpiderState::CHASE && !m_bHasTaunted)
	{
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_scream.wav");
		return (int)SpiderState::TAUNT;
	}

	return nextState;
}

int Spider::UpdateWalk(float deltaTime)
{
	int nextState = Monster::UpdateWalk(deltaTime);

	// CHASE로 전환될 때 도발(TAUNT) 체크
	if (nextState == (int)SpiderState::CHASE && !m_bHasTaunted)
	{
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_scream.wav");
		return (int)SpiderState::TAUNT;
	}

	return nextState;
}

int Spider::UpdateChase(float deltaTime)
{
	return Monster::UpdateChase(deltaTime);
}

void Spider::Damaged(int damage)
{
	Monster::Damaged(damage);
	if (!IsDead()) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_hurt.wav");
		m_bCanChase = true;
		ChangeState((int)SpiderState::HIT);
		m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
	}
}

void Spider::OnAttackHit() { 
	if (m_state == (int)SpiderState::ATTACK) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_attack.wav");
		ProcessAttackHit(m_damage);
	}
}

void Spider::OnAttackEnd()
{
	if (m_state != (int)SpiderState::ATTACK) return;
	if (m_attackCollider) m_attackCollider->SetColliderEnabled(false);

	if (m_attackTarget && m_attackTarget->IsEnabled() && m_distToPlayerSq <= (m_attackRange * m_attackRange * 1.1f)) {
		ChangeState((int)SpiderState::IDLE);
	}
	else
		ChangeState((int)SpiderState::CHASE);
}

void Spider::OnHitEnd()
{
	if (m_state != (int)SpiderState::HIT) return;
	ChangeState((int)SpiderState::IDLE);
}

void Spider::Die() {
	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_death.wav");
	SetDropItem(GOID_ITEM_MONSTER_MEAT, 1);
	ChangeState((int)SpiderState::DEATH);
}

bool Spider::OnInteraction(GameObject* obj) { return Entity::OnInteraction(obj); }