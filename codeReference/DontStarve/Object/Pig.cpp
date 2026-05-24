#include "99_Default/pch.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/SoundManager/SoundManager.h"
#include "../../../03_Animation/Animator.h"
#include "../../../03_Animation/AnimationClip.h"
#include "../Player/Player.h"
#include "../../Component/Sprite/SpriteSheet.h"
#include "../../Component/Transform/Transform.h"
#include "../../Component/Collider/BoxCollider.h"
#include "Pig.h"

Pig::Pig(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Monster(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType)
{
	m_hp = 100;
	m_maxHp = m_hp;
	m_type = GO_TYPE_MONSTER;
	m_walkSpeed = 80.0f;
	m_runSpeed = 200.0f;
	m_attackRange = 70.0f;
	m_attackCooldown = 3.f;
	m_attackHitFrame = 28;
	m_damage = 15;
	m_attackBoxWidth = 90;
	m_attackBoxHeight = 100;
}

Pig::~Pig() {}

void Pig::Init()
{
	Monster::Init();
	SetupAggro(AggroType::ON_HIT_THEN_RANGE, 250.0f, 600.0f);
	SetupAttackBox(m_attackBoxWidth, m_attackBoxHeight,0, -100);

	ChangeState((int)PigState::IDLE);
	m_idleTimer = 0.0f;
	m_idleDuration = 2.0f + (rand() / (float)RAND_MAX) * 3.0f;

	if (this->m_transform) {
		m_targetX = this->m_transform->GetX();
		m_targetY = this->m_transform->GetY();
	}

	if (!m_animator) m_animator = AddComponent<Animator>(m_spriteRenderer);
	if (m_animator)
	{
		DataManager* pRM = DataManager::GetInstance();
		const ResourcePathUtils::ObjectResourceDef* objData = pRM->GetObjectResourceInfo(GOID_MONSTER_PIG);
		if (objData) {
			std::wstring base = objData->baseDir + L"\\";
			std::wstring baseAction = base + L"Action\\";

			m_animator->RegisterAnimation((int)PigState::IDLE, DIR_DOWN, baseAction + L"pig_pigman_idle_loop_down.png", 0, 0, 4, 33, objData->pivotX, objData->pivotY, true, 0.03f);
			m_animator->RegisterAnimation((int)PigState::IDLE, DIR_UP, baseAction + L"pig_pigman_idle_loop_up.png", 0, 0, 4, 33, objData->pivotX, objData->pivotY, true, 0.03f);
			std::wstring idleSidePath = baseAction + L"pig_pigman_idle_loop_side.png";
			m_animator->RegisterAnimation((int)PigState::IDLE, DIR_LEFT, idleSidePath, 0, 0, 4, 33, objData->pivotX, objData->pivotY, true, 0.025f, false);
			m_animator->RegisterAnimation((int)PigState::IDLE, DIR_RIGHT, idleSidePath, 0, 0, 4, 33, objData->pivotX, objData->pivotY, true, 0.025f);

			m_animator->RegisterAnimation((int)PigState::WALK, DIR_DOWN, baseAction + L"pig_pigman_walk_loop_down.png", 0, 0, 4, 41, objData->pivotX, objData->pivotY, true, 0.03f);
			m_animator->RegisterAnimation((int)PigState::WALK, DIR_UP, baseAction + L"pig_pigman_walk_loop_up.png", 0, 0, 4, 41, objData->pivotX, objData->pivotY, true, 0.03f);
			std::wstring walkSidePath = baseAction + L"pig_pigman_walk_loop_side.png";
			m_animator->RegisterAnimation((int)PigState::WALK, DIR_LEFT, walkSidePath, 0, 0, 4, 41, objData->pivotX, objData->pivotY, true, 0.03f, false);
			m_animator->RegisterAnimation((int)PigState::WALK, DIR_RIGHT, walkSidePath, 0, 0, 4, 41, objData->pivotX, objData->pivotY, true, 0.03f);

			m_animator->RegisterAnimation((int)PigState::CHASE, DIR_DOWN, baseAction + L"pig_pigman_run_loop_down.png", 225, 198, 4, 33, objData->pivotX, objData->pivotY, true, 0.03f);
			m_animator->RegisterAnimation((int)PigState::CHASE, DIR_UP, baseAction + L"pig_pigman_run_loop_up.png", 225, 195, 4, 33, objData->pivotX, objData->pivotY, true, 0.03f);
			std::wstring runSidePath = baseAction + L"pig_pigman_run_loop_side.png";
			m_animator->RegisterAnimation((int)PigState::CHASE, DIR_LEFT, runSidePath, 222, 200, 4, 33, objData->pivotX, objData->pivotY, true, 0.03f, false);
			m_animator->RegisterAnimation((int)PigState::CHASE, DIR_RIGHT, runSidePath, 222, 200, 4, 33, objData->pivotX, objData->pivotY, true, 0.03f);

			std::wstring baseAttack = base + L"Attack\\";
			m_animator->RegisterAnimation((int)PigState::ATTACK, DIR_DOWN, baseAttack + L"down_pigman_atk_down.png", 0, 0, 4, 66, objData->pivotX, objData->pivotY, false, 0.03f);
			m_animator->RegisterAnimation((int)PigState::ATTACK, DIR_UP, baseAttack + L"up_pigman_atk_up.png", 0, 0, 4, 66, objData->pivotX, objData->pivotY, false, 0.03f);
			std::wstring atkSidePath = baseAttack + L"side_pigman_atk_side.png";
			m_animator->RegisterAnimation((int)PigState::ATTACK, DIR_LEFT, atkSidePath, 0, 0, 4, 66, objData->pivotX, objData->pivotY, false, 0.03f, false);
			m_animator->RegisterAnimation((int)PigState::ATTACK, DIR_RIGHT, atkSidePath, 0, 0, 4, 66, objData->pivotX, objData->pivotY, false, 0.03f);
			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				AnimationClip* clip = m_animator->GetAnimationClip((int)PigState::ATTACK, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
					clip->AddEventFrame(65, L"attack_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"attack_hit") this->OnAttackHit();
						else if (eventName == L"attack_end") this->OnAttackEnd();
						});
				}
			}

			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)PigState::HIT, (Direction)dir, base + L"Hit\\Hit_pigman_hit.png", 232, 245, 4, 29, objData->pivotX, objData->pivotY, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)PigState::HIT, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(28, L"hit_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"hit_end") this->OnHitEnd();
						});
				}
			}
			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				m_animator->RegisterAnimation((int)PigState::DEATH, (Direction)dir, base + L"Death\\Death_pigman_death.png", 227, 243, 4, 64, objData->pivotX, objData->pivotY, false, 0.03f);
				AnimationClip* clip = m_animator->GetAnimationClip((int)PigState::DEATH, (Direction)dir);
				if (clip) {
					clip->AddEventFrame(63, L"death_end");
					clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
						if (eventName == L"death_end") this->OnDeathEnd();
						});
				}
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

void Pig::RenderDebugOverlay()
{
	Combatant::RenderDebugOverlay();
}

void Pig::UpdateAI(float deltaTime)
{
	Monster::UpdateAI(deltaTime);
}

void Pig::UpdateMovement(float deltaTime)
{
	Monster::UpdateMovement(deltaTime);
}

int Pig::UpdateIdle(float deltaTime)
{
	return Monster::UpdateIdle(deltaTime);
}

int Pig::UpdateWalk(float deltaTime)
{
	return Monster::UpdateWalk(deltaTime);
}

int Pig::UpdateChase(float deltaTime)
{
	return Monster::UpdateChase(deltaTime);
}

void Pig::Damaged(int damage)
{
	Monster::Damaged(damage);
	if (!IsDead())
	{
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PigSound/Pig_hurt.wav");

		if (m_state == (int)PigState::HIT)
		{
			m_animator->SetState((int)PigState::HIT, m_transform->GetDirection(), true);
		}
		else {
			ChangeState((int)PigState::HIT);
		}
		m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
	}
}

void Pig::OnAttackHit() { 
	if (m_state == (int)PigState::ATTACK) {
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PigSound/Pig_Attack.wav");
		ProcessAttackHit(m_damage);
	}
}

void Pig::OnAttackEnd()
{
	if (m_state != (int)PigState::ATTACK) return;
	if (m_attackCollider) m_attackCollider->SetColliderEnabled(false);

	// 공격 종료 후에도 사거리 내에 있다면 IDLE로 전환하여 쿨타임을 기다림
	if (m_attackTarget && m_attackTarget->IsEnabled() && m_distToPlayerSq <= (m_attackRange * m_attackRange * 1.1f)) {
		ChangeState((int)PigState::IDLE);
	}
	else {
		ChangeState((int)PigState::CHASE);
	}
}

void Pig::OnHitEnd()
{
	if (m_state != (int)PigState::HIT) return;
	ChangeState((int)PigState::CHASE);
}

void Pig::Die() {
	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PigSound/Pig_death.wav");

	// 0: MEAT, 1: SMALL_MEAT
	int r = rand() % 2;
	SetDropItem(r == 0 ? GOID_ITEM_MEAT : GOID_ITEM_SMALL_MEAT, 1);
	ChangeState((int)PigState::DEATH);
}

bool Pig::OnInteraction(GameObject* obj) { return Entity::OnInteraction(obj); }
