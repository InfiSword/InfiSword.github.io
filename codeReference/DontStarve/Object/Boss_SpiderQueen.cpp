#include "99_Default/pch.h"
#include "Boss_SpiderQueen.h"
#include "Spider.h"
#include "../Player/Player.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../01_Manager/SoundManager/SoundManager.h"
#include "../../Building/SpiderEgg.h"
#include "../../../03_Animation/Animator.h"
#include "../../../03_Animation/AnimationClip.h"
#include "../../Component/Transform/Transform.h"
#include "../../Component/Collider/BoxCollider.h"

Boss_SpiderQueen::Boss_SpiderQueen(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Spider(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType)
	, m_bossPhase(1)
	, m_specialAttackCooldown(0.0f)
	, m_comboAttackCooldown(5.0f)
	, m_comboCount(0)
	, m_poopCooldown(15.0f)
	, m_poopCount(0)
	, m_idleTimer(0.0f)
	, m_idleDuration(2.0f)
	, m_hasTriggeredCocoon(false)
	, m_cocoonTimer(0.0f)
	, m_healTickTimer(0.0f)
	, m_spawnOnHitCooldown(0.0f)
	, m_hasTauntStarted(false)
	, m_isCombatEnabled(true)
	, m_cocoonPoolWarmCount(16)
{
	m_hp = 1000;
	m_maxHp = m_hp;
	m_type = GO_TYPE_MONSTER;
	m_walkSpeed = 50.0f;
	m_runSpeed = 120.0f;
	m_attackRange = 130.0f;
	m_attackCooldown = 1.5f;
	m_attackHitFrame = 40;
	m_damage = 25;
	m_attackBoxWidth = 110;
	m_attackBoxHeight = 80;
}

Boss_SpiderQueen::~Boss_SpiderQueen() {}

void Boss_SpiderQueen::Init()
{
	Monster::Init();
	m_bUseSuperArmor = true;
	SetupAggro(AggroType::ALWAYS, 0.0f, 0.0f);
	SetupAttackBox(m_attackBoxWidth, m_attackBoxHeight,0,-40.f);

	ChangeState((int)SpiderQueenState::IDLE);
	m_bossPhase = 1;
	m_specialAttackCooldown = 0.0f;
	m_hasTauntStarted = false;
	m_bHasTaunted = false;

	if (!m_animator) m_animator = AddComponent<Animator>(m_spriteRenderer);

	DataManager* pRM = DataManager::GetInstance();
	const ResourcePathUtils::ObjectResourceDef* objData = pRM->GetObjectResourceInfo(GOID_MONSTER_QUEEN_SPIDER);
	if (objData) {
		std::wstring base = objData->baseDir + L"\\";
		float px = objData->pivotX;
		float py = objData->pivotY;

		std::wstring idlePath = base + L"Queen_spider_queen_idle_side.png";
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) m_animator->RegisterAnimation((int)SpiderQueenState::IDLE, (Direction)dir, idlePath, 0, 0, 4, 50, px, py, true, 0.02f);

		std::wstring walkPath = base + L"Walk_spider_queen_walk_loop_side.png";
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) m_animator->RegisterAnimation((int)SpiderQueenState::CHASE, (Direction)dir, walkPath, 0, 0, 7, 65, px, py, true, 0.02f);

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) m_animator->RegisterAnimation((int)SpiderQueenState::WALK, (Direction)dir, walkPath, 0, 0, 7, 65, px, py, true, 0.03f);

		std::wstring attackPath = base + L"Queen_spider_queen_atk_side.png";
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::ATTACK, (Direction)dir, attackPath, 0, 0, 7, 53, px, py, false, 0.02f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::ATTACK, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(m_attackHitFrame, L"attack_hit");
				clip->AddEventFrame(52, L"attack_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"attack_hit") this->OnAttackHit();
					else if (eventName == L"attack_end") this->OnAttackEnd();
					});
			}
		}

		// 3콤보 공격 애니메이션 (기존 Attack 리소스 재사용)
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::COMBO_ATTACK, (Direction)dir, attackPath, 0, 0, 7, 53, px, py, false, 0.015f); // 콤보는 약간 더 빠르게
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::COMBO_ATTACK, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(m_attackHitFrame, L"combo_hit");
				clip->AddEventFrame(52, L"combo_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"combo_hit") this->OnComboAttackHit();
					else if (eventName == L"combo_end") this->OnComboAttackEnd();
					});
			}
		}

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::HIT, (Direction)dir, base + L"Queen_spider_queen_hit_side.png", 0, 0, 7, 29, px, py, false, 0.02f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::HIT, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(28, L"hit_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"hit_end") this->OnHitEnd();
					});
			}
		}

		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::DEATH, (Direction)dir, base + L"Queen_spider_queen_death.png", 0, 0, 7, 45, px, py, false, 0.03f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::DEATH, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(44, L"death_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"death_end") this->OnDeathEnd();
					});
			}
		}

		// ENTER (BIRTH) 애니메이션
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::BIRTH, (Direction)dir, base + L"Queen_spider_queen_enter.png", 0, 0, 7, 58, px, py, false, 0.02f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::BIRTH, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(57, L"birth_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"birth_end") this->OnBirthEnd();
					});
			}
		}

		// COCOON_PRE 애니메이션
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::COCOON_PRE, (Direction)dir, base + L"Cocoon_spider_queen_cocoon.png", 0, 0, 7, 56, px, py, false, 0.02f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::COCOON_PRE, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(44, L"cocoon_pre_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"cocoon_pre_end") this->OnCocoonPreEnd();
					});
			}
		}

		// COCOON 상태: SmallEgg 리소스 사용
		const ResourcePathUtils::ObjectResourceDef* eggData = pRM->GetObjectResourceInfo(GOID_BUILDING_SPIDER_SMALLEGG);
		if (eggData) {
			std::wstring eggBase = eggData->baseDir + L"\\";
			for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
				// Idle
				m_animator->RegisterAnimation((int)SpiderQueenState::COCOON, (Direction)dir, eggBase + L"Egg_spider_cocoon_small_Idle.png", 0, 0, 1, 1, px, py, true, 0.02f);
				// Hit
				m_animator->RegisterAnimation((int)SpiderQueenState::COCOON_HIT, (Direction)dir, eggBase + L"Hit\\Egg_spider_cocoon_cocoon_small_hit.png", 0, 0, 7, 33, px, py, false, 0.02f);
			}
		}

		// TAUNT 애니메이션
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::TAUNT, (Direction)dir, base + L"Queen_spider_queen_taunt.png", 0, 0, 7, 50, px, py, false, 0.03f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::TAUNT, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(0, L"taunt_start");
				clip->AddEventFrame(49, L"taunt_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"taunt_start")
					{
						SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/SpiderQueen_Scream.wav");
						m_hasTauntStarted = true;
					}
					else if (eventName == L"taunt_end")
					{
						m_bHasTaunted = true;
						m_bCanChase = false;
						ChangeState((int)SpiderQueenState::IDLE);
					}
					});
			}
		}

		// POOP_PRE 애니메이션
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::POOP_PRE, (Direction)dir, base + L"Queen_spider_queen_poop_pre.png", 0, 0, 7, 45, px, py, false, 0.03f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::POOP_PRE, (Direction)dir);
			if (clip) {
				clip->AddEventFrame(44, L"poop_pre_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"poop_pre_end") this->ChangeState((int)SpiderQueenState::POOP_LOOP);
					});
			}
		}

		// POOP_LOOP 애니메이션
		for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
			m_animator->RegisterAnimation((int)SpiderQueenState::POOP_LOOP, (Direction)dir, base + L"Queen_spider_queen_poop_loop.png", 0, 0, 7, 43, px, py, false, 0.03f);
			AnimationClip* clip = m_animator->GetAnimationClip((int)SpiderQueenState::POOP_LOOP, (Direction)dir);
			if (clip) {	
				clip->AddEventFrame(36, L"poop_egg");
				clip->AddEventFrame(42, L"poop_end");
				clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
					if (eventName == L"poop_egg") this->OnPoopEgg();
					else if (eventName == L"poop_end") this->OnPoopEnd();
					});
			}
		}
	}

	if (m_attackCollider) {
		UpdateAttackBoxByDirection(DIR_DOWN);
		m_attackCollider->SetColliderEnabled(false);
	}

	m_cocoonSpiderPool.reserve(static_cast<size_t>(m_cocoonPoolWarmCount) * 2);
	if (m_cocoonSpiderPool.empty()) {
		PreSpawnCocoonSpiders(m_cocoonPoolWarmCount);
	}
}

void Boss_SpiderQueen::RenderDebugOverlay()
{
	Combatant::RenderDebugOverlay();
}

void Boss_SpiderQueen::UpdateAI(float deltaTime)
{
	if (!IsEnabled() || !m_transform || !m_animator) return;

	// 인트로 연출 중에는 도발(TAUNT)만 허용하고 전투 전환을 막는다.
	if (!m_isCombatEnabled)
	{
		m_attackTarget = nullptr;
		if (!m_hasTauntStarted)
		{
			if (m_state != (int)SpiderQueenState::TAUNT)
			{
				ChangeState((int)SpiderQueenState::TAUNT);
			}
			return;
		}

		if (!m_bHasTaunted)
		{
			return;
		}

		if (m_state != (int)SpiderQueenState::IDLE)
		{
			ChangeState((int)SpiderQueenState::IDLE);
		}
		return;
	}

	switch ((SpiderQueenState)m_state)
	{
	case SpiderQueenState::TAUNT:
		return;

	case SpiderQueenState::COCOON_PRE:
	case SpiderQueenState::BIRTH:
		// 애니메이션 이벤트로 상태 전이하므로 별도 로직 없음
		break;

	case SpiderQueenState::COCOON:
	case SpiderQueenState::COCOON_HIT:
		m_cocoonTimer += deltaTime;
		m_healTickTimer += deltaTime;
		if (m_spawnOnHitCooldown > 0.0f) m_spawnOnHitCooldown -= deltaTime;

		// 1초마다 5% 회복
		if (m_healTickTimer >= 1.0f)
		{
			m_hp += static_cast<int>(m_maxHp * 0.05f);
			if (m_hp > m_maxHp) m_hp = m_maxHp;
			m_healTickTimer = 0.0f;
		}

		// 피격 애니메이션 종료 후 복귀
		if (m_state == (int)SpiderQueenState::COCOON_HIT && m_animator->IsAnimationDone())
		{
			ChangeState((int)SpiderQueenState::COCOON);
		}

		// 10초 후 종료
		if (m_cocoonTimer >= 10.0f)
		{
			EndCocoonPhase();
		}
		break;

	case SpiderQueenState::COMBO_ATTACK:
		// 콤보 공격 중에는 별도 AI 로직 없음 (애니메이션 이벤트로 처리)
		break;

	case SpiderQueenState::POOP_PRE:
	case SpiderQueenState::POOP_LOOP:
		// 알 낳기 중에는 별도 AI 로직 없음
		break;

	default:
		if (m_comboAttackCooldown > 0.0f) m_comboAttackCooldown -= deltaTime;
		if (m_poopCooldown > 0.0f) m_poopCooldown -= deltaTime;
		Monster::UpdateAI(deltaTime);
		break;
	}
}

void Boss_SpiderQueen::UpdateMovement(float deltaTime)
{
	if (!IsEnabled()) return;

	switch ((SpiderQueenState)m_state)
	{
	case SpiderQueenState::COCOON:
	case SpiderQueenState::BIRTH:
	case SpiderQueenState::COCOON_PRE:
	case SpiderQueenState::TAUNT:
	case SpiderQueenState::COMBO_ATTACK:
	case SpiderQueenState::POOP_PRE:
	case SpiderQueenState::POOP_LOOP:
		break;
	default:
		Monster::UpdateMovement(deltaTime);
		break;
	}
}

int Boss_SpiderQueen::UpdateIdle(float deltaTime)
{
	int nextState = Monster::UpdateIdle(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasTaunted ? (int)SpiderQueenState::IDLE : (int)SpiderQueenState::TAUNT;
	}

	if ((nextState == (int)SpiderQueenState::CHASE || nextState == (int)SpiderQueenState::ATTACK) && !m_bHasTaunted)
	{
		return (int)SpiderQueenState::TAUNT;
	}

	return nextState;
}

int Boss_SpiderQueen::UpdateWalk(float deltaTime)
{
	int nextState = Monster::UpdateWalk(deltaTime);

	if (!m_isCombatEnabled)
	{
		return m_bHasTaunted ? (int)SpiderQueenState::IDLE : (int)SpiderQueenState::TAUNT;
	}

	if (nextState == (int)SpiderQueenState::CHASE && !m_bHasTaunted)
	{
		return (int)SpiderQueenState::TAUNT;
	}

	return nextState;
}

int Boss_SpiderQueen::UpdateChase(float deltaTime)
{
	if (!m_isCombatEnabled)
	{
		return m_bHasTaunted ? (int)SpiderQueenState::IDLE : (int)SpiderQueenState::TAUNT;
	}

	const float comboRangeSq = m_attackRange * m_attackRange * 2.25f;

	if (m_poopCooldown <= 0.0f)
	{
		m_poopCooldown = 25.0f; // 사용 후 20초 쿨타임
		m_poopCount = 0;
		return (int)SpiderQueenState::POOP_PRE;
	}

	if (m_comboAttackCooldown <= 0.0f && m_distToPlayerSq <= comboRangeSq) // 약간 더 먼 거리에서도 발동 가능
	{
		m_comboCount = 0;
		m_comboAttackCooldown = 10.0f; // 사용 후 10초 쿨타임
		return (int)SpiderQueenState::COMBO_ATTACK;
	}

	return Monster::UpdateChase(deltaTime);
}

void Boss_SpiderQueen::OnAttackHit()
{
	if (!m_isCombatEnabled || m_state != (int)SpiderQueenState::ATTACK) return;

	LookAtPlayer();

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/SpiderQueen_attack.wav");
	ProcessAttackHit(m_damage);
}

void Boss_SpiderQueen::OnAttackEnd()
{
	if (m_state != (int)SpiderQueenState::ATTACK) return;

	HandleAttackEndSuperArmor();

	ChangeState((int)SpiderQueenState::CHASE);
}

void Boss_SpiderQueen::OnComboAttackHit()
{
	if (m_state != (int)SpiderQueenState::COMBO_ATTACK) return;

	LookAtPlayer();

	// 플레이어 방향으로 전진 (약 40픽셀)
	if (m_transform)
	{
		float moveDist = 40.0f;
		float nx = m_transform->GetX() + m_dirToPlayer.X * moveDist;
		float ny = m_transform->GetY() + m_dirToPlayer.Y * moveDist;
		m_transform->SetPosition(nx, ny);
		ClampPositionToMapBounds();
	}

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/SpiderQueen_attack.wav");
	ProcessAttackHit(m_damage);
}

void Boss_SpiderQueen::OnComboAttackEnd()
{
	if (m_state != (int)SpiderQueenState::COMBO_ATTACK) return;

	m_comboCount++;
	if (m_comboCount < 3)
	{
		// 애니메이션 재시작 (콤보 연결)
		ChangeState((int)SpiderQueenState::COMBO_ATTACK, true);
	}
	else
	{
		m_comboCount = 0;
		HandleAttackEndSuperArmor();
		ChangeState((int)SpiderQueenState::CHASE);
	}
}

void Boss_SpiderQueen::OnPoopEgg()
{
	if (m_state != (int)SpiderQueenState::POOP_LOOP) return;
	m_poopCount++;

	ObjectManager* objectManager = ObjectManager::GetInstance();
	if (!objectManager || !m_transform) return;

	static const GameObjectID eggIDs[] = {
		GOID_BUILDING_SPIDER_SMALLEGG,
		GOID_BUILDING_SPIDER_NORMALEGG,
		GOID_BUILDING_SPIDER_TALLEGG
	};

	// 무작위 3개 알 생성 (이벤트 콜백이 3번 호출됨)
	const float angle = Utils::Random01() * 6.283185f;
	const float dist = 100.0f + Utils::Random01() * 100.0f;
	float ex = m_transform->GetX() + cosf(angle) * dist;
	float ey = m_transform->GetY() + sinf(angle) * dist;

	// 알 ID 랜덤 선택 (Small, Normal, Tall 중 하나)
	const GameObjectID selectedID = eggIDs[rand() % _countof(eggIDs)];

	SpiderEgg* eggObj = objectManager->CreateObject<SpiderEgg>(selectedID, ex, ey);
	if (eggObj)
	{
		SpiderEgg* egg = dynamic_cast<SpiderEgg*>(eggObj);
		if (egg)
		{
			// 보스가 생성한 알은 주기적으로 거미를 스폰하도록 설정
			egg->SetPeriodicSpawn(true, 5.0f);
		}
	}
}

void Boss_SpiderQueen::OnPoopEnd()
{
	if (m_state != (int)SpiderQueenState::POOP_LOOP) return;
	HandleAttackEndSuperArmor();
	ChangeState((int)SpiderQueenState::CHASE);
}

void Boss_SpiderQueen::OnHitEnd()
{
	if (m_state != (int)SpiderQueenState::HIT) return;
	ChangeState((int)SpiderQueenState::IDLE);
}

void Boss_SpiderQueen::OnCocoonPreEnd()
{
	ChangeState((int)SpiderQueenState::COCOON);
}

void Boss_SpiderQueen::OnBirthEnd()
{
	ChangeState((int)SpiderQueenState::IDLE);
}

bool Boss_SpiderQueen::OnInteraction(GameObject* obj) { return Entity::OnInteraction(obj); }

void Boss_SpiderQueen::SetCombatEnabled(bool enabled)
{
	m_isCombatEnabled = enabled;
	if (!m_isCombatEnabled)
	{
		m_hasTauntStarted = false;
		m_bHasTaunted = false;
		m_attackTarget = nullptr;
		m_attackCooldownTimer = 0.0f;
	}
	else
	{
		// ALWAYS 어그로는 인트로 중 비운 타겟을 전투 재개 시 즉시 복구해야 추격이 시작된다.
		if (m_aggroType == AggroType::ALWAYS)
		{
			m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
		}
	}
}

void Boss_SpiderQueen::Damaged(int damage)
{
	// 진입/탈출 연출 중 무적 처리
	if (m_state == (int)SpiderQueenState::COCOON_PRE || m_state == (int)SpiderQueenState::BIRTH)
		return;

	if (m_state == (int)SpiderQueenState::COCOON || m_state == (int)SpiderQueenState::COCOON_HIT)
	{
		// 고치 상태에서 피격 시 연출 및 시간 단축
		if (m_state != (int)SpiderQueenState::COCOON_HIT)
		{
			ChangeState((int)SpiderQueenState::COCOON_HIT);
		}

		// 피격당 고치 지속 시간 0.5초 단축 
		m_cocoonTimer += 0.5f;

		//// 고치 상태 피격 시 방어 소환 
		//if (m_spawnOnHitCooldown <= 0.0f)
		//{
		//	SummonSpider();
		//	m_spawnOnHitCooldown = 1.0f;
		//}
		return;
	}

	Monster::Damaged(damage);
	if (IsDead())
	{
		SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/spiderQueen_Death.wav");
		m_hp = 0; ChangeState((int)SpiderQueenState::DEATH); m_isDead = true;
		OutputDebugStringW(L"Boss_SpiderQueen: 보스가 처치되었습니다\n");
		return;
	}

	// HP 50% 이하일 때 고치 페이즈 발동 (1회)
	if (!m_hasTriggeredCocoon && m_hp <= m_maxHp * 0.5f)
	{
		StartCocoonPhase();
		return;
	}

	if (m_hp <= m_maxHp * 0.5f && m_bossPhase == 1)
	{
		m_bossPhase = 2; OutputDebugStringW(L"Boss_SpiderQueen: 보스 페이즈가 2단계로 전환!\n");
	}

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/Spider_hurt.wav");

	if (CheckSuperArmorHit()) return;

	ChangeState((int)SpiderQueenState::HIT);
}

void Boss_SpiderQueen::StartCocoonPhase()
{
	m_hasTriggeredCocoon = true;
	m_cocoonTimer = 0.0f;
	m_healTickTimer = 0.0f;

	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/SpiderQueen_Scream.wav");

	ChangeState((int)SpiderQueenState::COCOON_PRE);

	// 고치 진입 시 거미 소환
	SummonSpider();

	OutputDebugStringW(L"Boss_SpiderQueen: 고치 상태 돌입! 거미를 소환하고 회복을 시작합니다.\n");
}

void Boss_SpiderQueen::EndCocoonPhase()
{
	SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/SpiderSound/SpiderQueen_Birth.wav");

	ChangeState((int)SpiderQueenState::BIRTH);

	OutputDebugStringW(L"Boss_SpiderQueen: 고치에서 나옵니다!\n");
}

void Boss_SpiderQueen::SummonSpider()
{
	ObjectManager* objMgr = ObjectManager::GetInstance();
	if (!objMgr || !m_transform) return;

	int spawnCount = 3;
	PreSpawnCocoonSpiders(spawnCount);

	float spawnRadius = 150.0f;
	const float baseX = m_transform->GetX();
	const float baseY = m_transform->GetY();
	Player* player = objMgr->GetPlayer();

	for (int i = 0; i < spawnCount; ++i)
	{
		Spider* spider = AcquirePooledSpider();
		if (!spider) continue;

		const float angle = Utils::Random01() * 6.283185f;
		const float dist = Utils::Random01() * spawnRadius;
		float sx = baseX + cosf(angle) * dist;
		float sy = baseY + sinf(angle) * dist;

		Transform* spiderTr = spider->GetComponent<Transform>();
		if (spiderTr) {
			spiderTr->SetPosition(sx, sy);
			//objMgr->UpdateObjectGridCell(spider);
		}

		spider->SetActive(true);
		spider->ClampPositionToMapBounds();

		if (player) spider->SetAggroTarget(player);
	}
	OutputDebugStringW((L"Boss_SpiderQueen: 거미 " + std::to_wstring(spawnCount) + L"마리 소환 완료\n").c_str());
}

void Boss_SpiderQueen::PreSpawnCocoonSpiders(int count)
{
	if (count <= 0) return;

	ObjectManager* objMgr = ObjectManager::GetInstance();
	if (!objMgr) return;

	if (m_cocoonSpiderPool.size() >= static_cast<size_t>(count)) {
		return;
	}

	if (m_cocoonSpiderPool.capacity() < static_cast<size_t>(count)) {
		m_cocoonSpiderPool.reserve(static_cast<size_t>(count) * 2);
	}

	while (m_cocoonSpiderPool.size() < static_cast<size_t>(count))
	{
		GameObjectID spiderID = (rand() % 2 == 0) ? GOID_MONSTER_SPIDER : GOID_MONSTER_WARRIOR_SPIDER;
		Spider* spiderObj = objMgr->CreateObject<Spider>(spiderID, 0.0f, 0.0f);
		if (!spiderObj) continue;

		Spider* spider = dynamic_cast<Spider*>(spiderObj);
		if (!spider) {
			objMgr->RemoveGameObject(spiderObj);
			continue;
		}

		spider->SetActive(false);
		m_cocoonSpiderPool.push_back(spider);
	}
}

Spider* Boss_SpiderQueen::AcquirePooledSpider()
{
	if (m_cocoonSpiderPool.empty()) {
		PreSpawnCocoonSpiders((std::max)(1, m_cocoonPoolWarmCount / 2));
	}

	if (m_cocoonSpiderPool.empty()) return nullptr;

	Spider* spider = m_cocoonSpiderPool.back();
	m_cocoonSpiderPool.pop_back();
	return spider;
}
