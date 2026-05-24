#include "99_Default/pch.h"
#include "BoxCollider.h"
#include "../Transform/Transform.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/ColliderManager/ColliderManager.h"
#include "../../GameObject.h"


BoxCollider::BoxCollider(GameObject* owner)
	: Collider(owner)
{
	// 기본 boundingBox는 나중에 Entity::Init()에서 설정됨
	m_boundingBox = { 0, 0, 0, 0 };
}

bool BoxCollider::IntersectsCollider(const Collider* other) const
{
	return ColliderManager::GetInstance()->Intersects(const_cast<BoxCollider*>(this), const_cast<Collider*>(other));
}

bool BoxCollider::ContainsPoint(float worldX, float worldY) const
{
	const RECT& box = GetWorldBoundingBox();
	return worldX >= (float)box.left && worldX < (float)box.right
		&& worldY >= (float)box.top && worldY < (float)box.bottom;
}

void BoxCollider::GetCenterWorld(float& outX, float& outY) const
{
	const RECT& box = GetWorldBoundingBox();
	outX = ((float)box.left + (float)box.right) * 0.5f;
	outY = ((float)box.top + (float)box.bottom) * 0.5f;
}

void BoxCollider::SetObjectCollider(int offsetX, int offsetY, int width, int height)
{
	m_boundingBox = {
		offsetX,
		offsetY,
		offsetX + width,
		offsetY + height
	};
}

RECT BoxCollider::GetWorldBoundingBox() const
{
	GameObject* owner = GetOwner();
	Transform* transform = owner ? owner->GetComponent<Transform>() : nullptr;

	float ox = transform ? transform->GetX() : 0.0f;
	float oy = transform ? transform->GetY() : 0.0f;
	float sx = transform ? transform->GetScaleX() : 1.0f;
	float sy = transform ? transform->GetScaleY() : 1.0f;

	RECT worldBox;
	worldBox.left   = static_cast<LONG>(ox + static_cast<float>(m_boundingBox.left) * sx);
	worldBox.top    = static_cast<LONG>(oy + static_cast<float>(m_boundingBox.top) * sy);
	worldBox.right  = static_cast<LONG>(ox + static_cast<float>(m_boundingBox.right) * sx);
	worldBox.bottom = static_cast<LONG>(oy + static_cast<float>(m_boundingBox.bottom) * sy);

	return worldBox;
}

Gdiplus::RectF BoxCollider::GetWorldRect() const
{
	RECT rect = GetWorldBoundingBox();
	return Gdiplus::RectF(
		(float)rect.left,
		(float)rect.top,
		(float)(rect.right - rect.left),
		(float)(rect.bottom - rect.top)
	);
}

void BoxCollider::RenderGizmo()
{
	/*if (!IsEnabled()) {
		return;
	}*/

	RenderManager* renderManager = RenderManager::GetInstance();
	CameraManager* cameraManager = CameraManager::GetInstance();

	// 월드 좌표로 변환된 boundingBox 가져오기
	RECT worldBox = GetWorldBoundingBox();

	// 월드 좌표를 화면 좌표로 변환
	Gdiplus::PointF screenTopLeft = cameraManager->WorldToScreen((float)worldBox.left, (float)worldBox.top);
	Gdiplus::PointF screenBottomRight = cameraManager->WorldToScreen((float)worldBox.right, (float)worldBox.bottom);

	renderManager->AddDrawRectCommand(Gdiplus::RectF(screenTopLeft.X, screenTopLeft.Y, screenBottomRight.X - screenTopLeft.X, screenBottomRight.Y - screenTopLeft.Y), Gdiplus::Color(255, 0, 0), 2.0f, LAYER_DEBUG_OVERLAY, 10.0f);

}
