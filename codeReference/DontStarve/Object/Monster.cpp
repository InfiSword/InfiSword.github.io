#include "99_Default/pch.h"
#include "Monster.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../02_GameObject/Component/Transform/Transform.h"
#include "../../../02_GameObject/Entity/Player/Player.h"
#include "../../../03_Animation/Animator.h"

Monster::Monster(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Combatant(id, x, y, pivotX, pivotY, dir, baseDir, imageName, true, true, colliderType),
	m_attackCooldownTimer(0.0f), m_walkSpeed(0.0f), m_runSpeed(0.0f),
	m_targetX(x), m_targetY(y), m_distToPlayerSq(1e10f), m_dirToPlayer(0.0f, 0.0f),
	m_aiTickTimer(0.0f),
	m_wanderRadius(200.0f), m_aggroRadius(300.0f), m_deaggroRadius(500.0f),
	m_idleTimer(0.0f), m_idleDuration(2.0f),
	m_aggroType(AggroType::ON_RANGE), m_hasBeenHit(false), m_bCanChase(true)
{
	// AI 연산 부하 분산을 위해 개체별로 0.1s ~ 0.2s 사이의 고유 틱 간격 부여
	m_aiTickInterval = 0.1f + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX / 0.1f));
}

Monster::~Monster()
{
}

void Monster::Init()
{
	Combatant::Init();
}

void Monster::SetupAggro(AggroType type, float aggroRadius, float deaggroRadius)
{
	m_aggroType = type;
	m_aggroRadius = aggroRadius;
	m_deaggroRadius = deaggroRadius;
	m_hasBeenHit = false;

	// ALWAYS 타입인 경우 즉시 플레이어를 타겟으로 설정
	if (m_aggroType == AggroType::ALWAYS)
	{
		m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
	}
}

void Monster::Damaged(int damage)
{
	Combatant::Damaged(damage);

	// ON_HIT_THEN_RANGE 타입: 피격 시 어그로 활성화
	if (m_aggroType == AggroType::ON_HIT_THEN_RANGE && !m_hasBeenHit)
	{
		m_hasBeenHit = true;
		m_attackTarget = ObjectManager::GetInstance()->GetPlayer();
	}
}

void Monster::Update(float deltaTime)
{
	Entity::Update(deltaTime);
	if (m_isDead) return;

	// 타겟 존재 여부에 따른 연산 분리
	if (m_attackTarget && m_attackTarget->IsEnabled()) {
		Transform* tTr = m_attackTarget->GetComponent<Transform>();
		float dx = tTr->GetX() - m_transform->GetX();
		float dy = tTr->GetY() - m_transform->GetY();
		m_distToPlayerSq = dx * dx + dy * dy;

		// 방향 벡터 정규화 (0 나누기 방지 1e-6f)
		float invDist = 1.0f / (sqrtf(m_distToPlayerSq) + 1e-6f);
		m_dirToPlayer.X = dx * invDist;
		m_dirToPlayer.Y = dy * invDist;

		// 어그로 해제 판정
		if (m_deaggroRadius > 0.0f && m_distToPlayerSq > (m_deaggroRadius * m_deaggroRadius)) {
			m_attackTarget = nullptr;
			ResetAggroSession();
		}
	}
	else {
		// 타겟이 없는 경우: 기본값 설정 및 신규 어그로 탐색 (ALWAYS는 이미 초기화 시점에 타겟을 잡았을 것이므로 제외)
		m_distToPlayerSq = 1e10f;
		m_dirToPlayer = { 0.0f, 0.0f };

		Player* p = ObjectManager::GetInstance()->GetPlayer();
		if (p && p->IsEnabled() && m_aggroType != AggroType::ALWAYS) {
			Transform* pTr = p->GetComponent<Transform>();
			float dSq = powf(pTr->GetX() - m_transform->GetX(), 2) + powf(pTr->GetY() - m_transform->GetY(), 2);

			// 어그로 획득 조건 판정 (범위 내 또는 피격 후 범위 내)
			bool canAggro = (m_aggroType == AggroType::ON_RANGE) || (m_aggroType == AggroType::ON_HIT_THEN_RANGE && m_hasBeenHit);
			if (canAggro && dSq <= m_aggroRadius * m_aggroRadius)
				m_attackTarget = p;
		}
	}

	UpdateAI(deltaTime);
	m_attackCooldownTimer = (std::max)(0.0f, m_attackCooldownTimer - deltaTime);
	UpdateMovement(deltaTime);
}

void Monster::UpdateAI(float deltaTime)
{
	if (m_isDead) return;

	int nextState = m_state;

	switch (m_state)
	{
	case (int)CombatantState::IDLE:
		nextState = UpdateIdle(deltaTime);
		break;
	case (int)CombatantState::WALK:
		nextState = UpdateWalk(deltaTime);
		break;
	case (int)CombatantState::CHASE:
		nextState = UpdateChase(deltaTime);
		break;
	case (int)CombatantState::ATTACK:
		nextState = UpdateAttack(deltaTime);
		break;
	case (int)CombatantState::HIT:
		nextState = UpdateHit(deltaTime);
		break;
	}

	ChangeState(nextState);
}

void Monster::UpdateMovement(float deltaTime)
{
	if (m_isDead) return;

	switch (m_state)
	{
	case (int)CombatantState::WALK:
		MoveTowardLocation(deltaTime, m_walkSpeed);
		break;
	case (int)CombatantState::CHASE:
		MoveTowardPlayer(deltaTime, m_runSpeed);
		break;
	}
}

int Monster::UpdateIdle(float deltaTime)
{
	if (m_attackTarget && m_attackTarget->IsEnabled())
	{
		LookAtPlayer();

		if (m_distToPlayerSq > (m_attackRange * m_attackRange))
		{
			if (m_bCanChase)
			{
				return (int)CombatantState::CHASE;
			}
		}
		else if (m_attackCooldownTimer <= 0.0f)
		{
			return (int)CombatantState::ATTACK;
		}
	}
	else
	{
		m_idleTimer += deltaTime;
		if (m_idleTimer >= m_idleDuration)
		{
			float centerX, centerY;
			ResolveWanderCenter(centerX, centerY);
			float angle = (rand() / (float)RAND_MAX) * 6.283185307f;
			float dist = (rand() / (float)RAND_MAX) * m_wanderRadius;
			m_targetX = centerX + cosf(angle) * dist;
			m_targetY = centerY + sinf(angle) * dist;

			// 맵 경계 내로 목표 지점 제한 (Entity::ClampPositionToMapBounds와 동일한 로직)
			float boundHalfWidth = 40.0f;
			float boundHalfHeight = 40.0f;
			float mapMaxX = static_cast<float>(MAP_WIDTH * TILE_SIZE) - boundHalfWidth;
			float mapMaxY = static_cast<float>(MAP_HEIGHT * TILE_SIZE) - boundHalfHeight;
			float mapMinX = boundHalfWidth;
			float mapMinY = boundHalfHeight;

			if (m_targetX < mapMinX) m_targetX = mapMinX;
			if (m_targetX > mapMaxX) m_targetX = mapMaxX;
			if (m_targetY < mapMinY) m_targetY = mapMinY;
			if (m_targetY > mapMaxY) m_targetY = mapMaxY;

			m_idleTimer = 0.0f;
			return (int)CombatantState::WALK;
		}
	}
	return (int)CombatantState::IDLE;
}

int Monster::UpdateWalk(float deltaTime)
{
	if (m_attackTarget && m_attackTarget->IsEnabled() && (m_bCanChase || m_aggroType == AggroType::ALWAYS))
	{
		return (int)CombatantState::CHASE;
	}

	if (!m_transform) return (int)CombatantState::IDLE;

	float wdx = m_targetX - m_transform->GetX();
	float wdy = m_targetY - m_transform->GetY();
	if (wdx * wdx + wdy * wdy < 4.0f)
	{
		m_idleTimer = 0.0f;
		return (int)CombatantState::IDLE;
	}

	return (int)CombatantState::WALK;
}

int Monster::UpdateChase(float deltaTime)
{
	if (!m_attackTarget || !m_attackTarget->IsEnabled() || !m_bCanChase)
	{
		m_idleTimer = 0.0f;
		return (int)CombatantState::IDLE;
	}

	if (m_distToPlayerSq <= (m_attackRange * m_attackRange))
	{
		if (m_attackCooldownTimer <= 0.0f)
		{
			return (int)CombatantState::ATTACK;
		}
		else
		{
			return (int)CombatantState::IDLE;
		}
	}

	return (int)CombatantState::CHASE;
}

int Monster::UpdateAttack(float deltaTime)
{
	return (int)CombatantState::ATTACK;
}

int Monster::UpdateHit(float deltaTime)
{
	return (int)CombatantState::HIT;
}

void Monster::OnDeathEnd()
{
	ObjectManager* objMgr = ObjectManager::GetInstance();
	if (objMgr)
	{
		GameObjectID dropItemID = GetDropItemID();
		int count = GetDropItemCount();

		if (dropItemID != GOID_NONE && m_transform)
		{
			float tx = m_transform->GetX();
			float ty = m_transform->GetY();

			for (int i = 0; i < count; ++i)
			{
				float angle = (rand() / (float)RAND_MAX) * 6.28f;
				float spreadRadius = 20.0f + (rand() / (float)RAND_MAX) * 30.0f;
				float offsetX = cosf(angle) * spreadRadius;
				float offsetY = sinf(angle) * spreadRadius;
				objMgr->CreateObject(dropItemID, tx + offsetX, ty + offsetY);
			}
		}
		objMgr->RemoveGameObject(this);
	}
}

void Monster::ResolveWanderCenter(float& outX, float& outY) const
{
	if (m_transform)
	{
		outX = m_transform->GetX();
		outY = m_transform->GetY();
		return;
	}

	outX = 0.0f;
	outY = 0.0f;
}

void Monster::MoveTowardPlayer(float deltaTime, float speed)
{
	if (!m_transform || !m_animator) return;

	// 실시간 방향 업데이트 (플레이어와의 상대적 위치 기준)
	Direction newDir = ResolveFacingDirection(m_dirToPlayer);

	m_transform->SetDirection(newDir);
	ChangeState(m_state);

	// 이동
	float moveDist = speed * deltaTime;
	m_transform->SetPosition(m_transform->GetX() + m_dirToPlayer.X * moveDist, m_transform->GetY() + m_dirToPlayer.Y * moveDist);

	// 맵 경계 체크
	ClampPositionToMapBounds();
}

void Monster::MoveTowardLocation(float deltaTime, float speed)
{
	if (!m_transform || !m_animator) return;

	float wdx = m_targetX - m_transform->GetX();
	float wdy = m_targetY - m_transform->GetY();
	float wdistSq = wdx * wdx + wdy * wdy;

	if (wdistSq < 0.0001f) {
		m_transform->SetPosition(m_targetX, m_targetY);
		return;
	}

	float wdist = sqrtf(wdistSq);

	// 실시간 방향 업데이트 (목표 지점 기준)
	Direction wDir = ResolveFacingDirection({ wdx, wdy });

	m_transform->SetDirection(wDir);
	ChangeState(m_state);

	// 목표 지점을 지나치지 않도록 스텝을 남은 거리로 클램프한다.
	const float moveStep = (std::min)(speed * deltaTime, wdist);
	if (moveStep <= 0.0f) return;

	if (moveStep >= wdist) {
		m_transform->SetPosition(m_targetX, m_targetY);
	}
	else {
		m_transform->SetPosition(m_transform->GetX() + (wdx / wdist) * moveStep, m_transform->GetY() + (wdy / wdist) * moveStep);
	}

	// 맵 경계 체크
	ClampPositionToMapBounds();
}

void Monster::LookAtPlayer()
{
	if (!m_transform || !m_attackTarget || !m_attackTarget->IsEnabled()) return;

	Direction newDir = ResolveFacingDirection(m_dirToPlayer);
	m_transform->SetDirection(newDir);
	UpdateAttackBoxByDirection(newDir);
}

Direction Monster::ResolveFacingDirection(const Gdiplus::PointF& dir)
{
	if (std::abs(dir.X) > std::abs(dir.Y)) {
		return (dir.X >= 0.0f) ? DIR_RIGHT : DIR_LEFT;
	}
	return (dir.Y >= 0.0f) ? DIR_DOWN : DIR_UP;
}