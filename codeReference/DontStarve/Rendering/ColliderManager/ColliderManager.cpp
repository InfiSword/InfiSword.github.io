#include "99_Default/pch.h"
#include "ColliderManager.h"
#include "../ObjectManager/ObjectManager.h"
#include "../../01_Manager/CameraManager/CameraManager.h"
#include "../../02_GameObject/GameObject.h"
#include "../../02_GameObject/Component/Collider/Collider.h"
#include "../../02_GameObject/Component/Collider/BoxCollider.h"
#include "../../02_GameObject/Component/Collider/CircleCollider.h"

ColliderManager::ColliderManager()
{
}

ColliderManager::~ColliderManager()
{
	Release();
}

void ColliderManager::Init()
{
	m_queryBuffer.clear();
}

void ColliderManager::LateInit()
{
}

void ColliderManager::Update(float deltaTime)
{
}

void ColliderManager::LateUpdate()
{
	//  각 객체(Player, Monster 등)가 필요한 시점에 QueryCollidingObjects를 호출하여 
}

void ColliderManager::QueryCollidingObjects(Collider* pSrc, std::vector<GameObject*>& outOwners)
{
	outOwners.clear();
	if (!pSrc || !pSrc->IsEnabled()) return;

	GameObject* pSrcOwner = pSrc->GetOwner();
	if (!pSrcOwner || !pSrcOwner->IsEnabled()) return;

	ObjectManager* objMgr = ObjectManager::GetInstance();
	if (!objMgr) return;

	// 그리드 쿼리로 srcRect 주변 객체만 가져온다.
	Gdiplus::RectF srcRect = pSrc->GetWorldRect();

	m_queryBuffer.clear();
	objMgr->QueryObjectsInRectArea(srcRect, m_queryBuffer);

	for (GameObject* pDstOwner : m_queryBuffer)
	{
		if (!pDstOwner || pDstOwner == pSrcOwner || !pDstOwner->IsEnabled()) continue;

		Collider* pDst = pDstOwner->GetMainCollider();
		if (!pDst || !pDst->IsEnabled()) continue;

		if (Intersects(pSrc, pDst))
		{
			outOwners.push_back(pDstOwner);
		}
	}
}

void ColliderManager::Release()
{
	m_queryBuffer.clear();
	m_queryBuffer.shrink_to_fit();
}

bool ColliderManager::Intersects(Collider* a, Collider* b)
{
	if (!a || !b) return false;
	if (!a->IsEnabled() || !b->IsEnabled()) return false;

	// [최적화] AABB 사전 컷 (Broad-phase와 Narrow-phase 사이의 중간 필터)
	// 도형의 종류와 상관없이 각 콜라이더를 감싸는 사각형(AABB)이 겹치지 않으면 즉시 리턴한다.
	Gdiplus::RectF rectA = a->GetWorldRect();
	Gdiplus::RectF rectB = b->GetWorldRect();

	if (rectA.X + rectA.Width < rectB.X || rectA.X > rectB.X + rectB.Width ||
		rectA.Y + rectA.Height < rectB.Y || rectA.Y > rectB.Y + rectB.Height) {
		return false;
	}

	ColliderType typeA = a->GetColliderType();
	ColliderType typeB = b->GetColliderType();

	if (typeA == COLLIDER_BOX && typeB == COLLIDER_BOX) {
		// Box vs Box는 상단의 AABB 체크와 로직상 동일하므로, 여기까지 왔다면 겹친 것이다.
		return true;
	}
	else if (typeA == COLLIDER_CIRCLE && typeB == COLLIDER_CIRCLE) {
		CircleCollider* circleA = static_cast<CircleCollider*>(a);
		CircleCollider* circleB = static_cast<CircleCollider*>(b);
		float cxA, cyA, rA; circleA->GetWorldCircle(cxA, cyA, rA);
		float cxB, cyB, rB; circleB->GetWorldCircle(cxB, cyB, rB);
		float dx = cxA - cxB; float dy = cyA - cyB;
		float distSq = dx * dx + dy * dy;
		float rSum = rA + rB;
		return distSq <= (rSum * rSum);
	}
	else if ((typeA == COLLIDER_BOX && typeB == COLLIDER_CIRCLE) || (typeA == COLLIDER_CIRCLE && typeB == COLLIDER_BOX)) {
		BoxCollider* box = (typeA == COLLIDER_BOX) ? static_cast<BoxCollider*>(a) : static_cast<BoxCollider*>(b);
		CircleCollider* circle = (typeA == COLLIDER_CIRCLE) ? static_cast<CircleCollider*>(a) : static_cast<CircleCollider*>(b);

		RECT rect = box->GetWorldBoundingBox();
		float cx, cy, r; circle->GetWorldCircle(cx, cy, r);

		float closestX = (std::max)((float)rect.left, (std::min)(cx, (float)rect.right));
		float closestY = (std::max)((float)rect.top, (std::min)(cy, (float)rect.bottom));

		float dx = cx - closestX; float dy = cy - closestY;
		return (dx * dx + dy * dy) <= (r * r);
	}

	return false;
}
