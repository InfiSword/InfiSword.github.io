#include "99_Default/pch.h"
#include "Combatant.h"
#include "../../01_Manager/CameraManager/CameraManager.h"
#include "../../01_Manager/ColliderManager/ColliderManager.h"
#include "../Component/Transform/Transform.h"
#include "../Component/Collider/BoxCollider.h"

bool Combatant::s_bShowAttackGizmo = false;

Combatant::Combatant(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName,
	bool isActive, bool isInteractive, ColliderType colliderType)
	: Entity(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType, isActive, isInteractive)
	, m_damage(0)
	, m_attackRange(0.0f)
	, m_attackCooldown(0.0f)
	, m_attackHitFrame(0)
	, m_attackBoxWidth(0)
	, m_attackBoxHeight(0)
	, m_attackCollider(nullptr)
	, m_attackTarget(nullptr)
	, m_bHitDuringAttack(false)
	, m_bUseSuperArmor(false)
{
}

Combatant::~Combatant()
{
	Release();
}

void Combatant::Init()
{
	Entity::Init();

	// 공격 콜라이더 생성
	m_attackCollider = AddComponent<BoxCollider>();
	if (m_attackCollider) {
		m_attackCollider->SetObjectCollider(0, 0, 80, 80);
		m_attackCollider->SetColliderEnabled(false);
	}
}

void Combatant::Release()
{
	m_attackCollider = nullptr;
	m_attackTarget = nullptr;
	Entity::Release();
}

void Combatant::Damaged(int damage)
{
	Entity::Damaged(damage);

	// 죽었을 때 공격 콜라이더도 즉시 비활성화
	if (m_isDead && m_attackCollider) {
		m_attackCollider->SetColliderEnabled(false);
	}
}

bool Combatant::CheckSuperArmorHit()
{
	if (!m_bUseSuperArmor) return false;

	if (IsInAttackState())
	{
		m_bHitDuringAttack = true;
		return true; // 슈퍼아머 동작 중이므로 히트 애니메이션 스킵
	}
	return false;
}

void Combatant::HandleAttackEndSuperArmor()
{
	if (m_bHitDuringAttack)
	{
		m_bHitDuringAttack = false;
	}
}

void Combatant::SetupAttackBox(int width, int height, int offsetX, int offsetY)
{
	// 기본 공격 박스 크기로 각 방향별 오프셋 자동 계산

	// DOWN: 캐릭터 앞쪽 (Y축 양의 방향)
	m_attackBoxDown.offsetX = -width / 2 + offsetX;
	m_attackBoxDown.offsetY = 0 + offsetY;
	m_attackBoxDown.width = width;
	m_attackBoxDown.height = height;

	// UP: 캐릭터 뒤쪽 (Y축 음의 방향)
	m_attackBoxUp.offsetX = -width / 2 + offsetX;
	m_attackBoxUp.offsetY = -height + offsetY;
	m_attackBoxUp.width = width;
	m_attackBoxUp.height = height;

	// LEFT: 캐릭터 왼쪽 (X축 음의 방향)
	m_attackBoxLeft.offsetX = -width + offsetX;
	m_attackBoxLeft.offsetY = -height / 2 + offsetY;
	m_attackBoxLeft.width = width;
	m_attackBoxLeft.height = height;

	// RIGHT: 캐릭터 오른쪽 (X축 양의 방향)
	m_attackBoxRight.offsetX = 0 + offsetX;
	m_attackBoxRight.offsetY = -height / 2 + offsetY;
	m_attackBoxRight.width = width;
	m_attackBoxRight.height = height;
}

void Combatant::RenderDebugOverlay()
{
	// 전역 디버그 플래그가 켜져 있을 때만 동작
	if (!s_bShowAttackGizmo) return;

	// 공격 중이고 공격 콜라이더가 있을 때 표시
	if (IsInAttackState() && m_attackCollider)
	{
		UpdateAttackBoxByDirection(m_transform->GetDirection());
		m_attackCollider->RenderGizmo();
	}
}

void Combatant::UpdateAttackBoxByDirection(Direction dir)
{
	if (!m_attackCollider) return;

	const AttackBox* box = &m_attackBoxDown;

	switch (dir)
	{
	case DIR_NONE:
	case DIR_COUNT:
		return;
	case DIR_DOWN:
		box = &m_attackBoxDown;
		break;
	case DIR_UP:
		box = &m_attackBoxUp;
		break;
	case DIR_LEFT:
		box = &m_attackBoxLeft;
		break;
	case DIR_RIGHT:
		box = &m_attackBoxRight;
		break;
	}

	if (box->width <= 0 || box->height <= 0) return;
	m_attackCollider->SetObjectCollider(box->offsetX, box->offsetY, box->width, box->height);
}

void Combatant::ProcessAttackHit(int damage)
{
	if (!m_attackCollider || !m_transform) return;

	// 현재 방향에 맞게 공격 박스 업데이트
	UpdateAttackBoxByDirection(m_transform->GetDirection());

	// 콜라이더 활성화
	m_attackCollider->SetColliderEnabled(true);

	// 데미지 적용
	ApplyAttackDamageToTarget(damage);

	// 콜라이더 비활성화
	m_attackCollider->SetColliderEnabled(false);
}

bool Combatant::ApplyAttackDamageToTarget(int damage)
{
	if (!m_attackCollider) return false;

	auto canApplyDamageTo = [this](GameObject* target) {
		if (!target || !target->IsEnabled() || target == this) return false;
		if (this->GetType() == GO_TYPE_MONSTER && target->GetType() != GO_TYPE_PLAYER) {
			return false;
		}
		return true;
	};

	// 클릭/AI로 지정된 타겟이 있으면 최우선으로 판정한다.
	if (canApplyDamageTo(m_attackTarget)) {
		Collider* targetCollider = m_attackTarget->GetMainCollider();
		if (targetCollider && targetCollider->IsEnabled() && 
			ColliderManager::GetInstance()->Intersects(m_attackCollider, targetCollider)) {
			m_attackTarget->Damaged(damage);
			return true;
		}
	}

	std::vector<GameObject*> hits;
	ColliderManager::GetInstance()->QueryCollidingObjects(m_attackCollider, hits);

	// 필터링: 공격 가능한 대상만 남김
	hits.erase(std::remove_if(hits.begin(), hits.end(), [&](GameObject* obj) {
		return !canApplyDamageTo(obj);
	}), hits.end());

	if (hits.empty()) return false;

	// 나(공격자)와의 거리를 기준으로 정렬 (가장 가까운 적부터 처리)
	Transform* myTr = m_transform;
	if (!myTr) return false;

	std::sort(hits.begin(), hits.end(), [myTr](GameObject* a, GameObject* b) {
		Transform* ta = a->GetComponent<Transform>();
		Transform* tb = b->GetComponent<Transform>();
		if (!ta) return false;
		if (!tb) return true;

		float distSqA = (ta->GetX() - myTr->GetX()) * (ta->GetX() - myTr->GetX()) +
			(ta->GetY() - myTr->GetY()) * (ta->GetY() - myTr->GetY());
		float distSqB = (tb->GetX() - myTr->GetX()) * (tb->GetX() - myTr->GetX()) +
			(tb->GetY() - myTr->GetY()) * (tb->GetY() - myTr->GetY());

		return distSqA < distSqB;
		});

	for (GameObject* obj : hits) {
		if (!canApplyDamageTo(obj)) continue;
		obj->Damaged(damage);
		return true;
	}

	return false;
}

void Combatant::OnAttackEnd()
{
	if (m_attackCollider)
		m_attackCollider->SetColliderEnabled(false);
	m_attackTarget = nullptr;
}
