#include "99_Default/pch.h"
#include "CircleCollider.h"
#include "../Transform/Transform.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/ColliderManager/ColliderManager.h"
#include "../../GameObject.h"


CircleCollider::CircleCollider(GameObject* owner, float centerX, float centerY, float radius)
	: Collider(owner), m_centerX(centerX), m_centerY(centerY), m_radius(radius)
{
}

bool CircleCollider::IntersectsCollider(const Collider* other) const
{
	return ColliderManager::GetInstance()->Intersects(const_cast<CircleCollider*>(this), const_cast<Collider*>(other));
}

bool CircleCollider::ContainsPoint(float worldX, float worldY) const
{
	float centerX, centerY, radius;
	GetWorldCircle(centerX, centerY, radius);
	float dx = worldX - centerX;
	float dy = worldY - centerY;
	return (dx * dx + dy * dy) <= (radius * radius);
}

void CircleCollider::GetCenterWorld(float& outX, float& outY) const
{
	float radius;
	GetWorldCircle(outX, outY, radius);
}

void CircleCollider::SetObjectCollider(float centerX, float centerY, float radius)
{
	m_centerX = centerX;
	m_centerY = centerY;
	m_radius = radius;
}

void CircleCollider::GetWorldCircle(float& centerX, float& centerY, float& radius) const
{
	GameObject* owner = GetOwner();
	Transform* transform = owner ? owner->GetComponent<Transform>() : nullptr;
	float ox = transform ? transform->GetX() : 0.0f;
	float oy = transform ? transform->GetY() : 0.0f;
	float sx = transform ? transform->GetScaleX() : 1.0f;
	float sy = transform ? transform->GetScaleY() : 1.0f;

	centerX = ox + m_centerX * sx;
	centerY = oy + m_centerY * sy;
	radius = m_radius * (sx + sy) * 0.5f; // 평균 스케일 적용
}

Gdiplus::RectF CircleCollider::GetWorldRect() const
{
	float cx, cy, r;
	GetWorldCircle(cx, cy, r);
	return Gdiplus::RectF(cx - r, cy - r, r * 2.0f, r * 2.0f);
}

void CircleCollider::RenderGizmo()
{
	if (!IsEnabled()) {
		return;
	}

	RenderManager* renderManager = RenderManager::GetInstance();
	CameraManager* cameraManager = CameraManager::GetInstance();

	float worldCenterX, worldCenterY, worldRadius;
	GetWorldCircle(worldCenterX, worldCenterY, worldRadius);

	Gdiplus::PointF screenCenter = cameraManager->WorldToScreen(worldCenterX, worldCenterY);

	Gdiplus::PointF screenRight = cameraManager->WorldToScreen(worldCenterX + worldRadius, worldCenterY);
	float screenRadius = abs(screenRight.X - screenCenter.X);

	Gdiplus::RectF gizmoRect(
		screenCenter.X - screenRadius,
		screenCenter.Y - screenRadius,
		screenRadius * 2.0f,
		screenRadius * 2.0f
	);

	Gdiplus::Color gizmoColor(255, 255, 0, 0); 
	Gdiplus::Color bgColor(30, 255, 0, 0); 

	renderManager->AddFillRectangleCommand(gizmoRect, bgColor, LAYER_DEBUG_OVERLAY, 9998.0f);

	renderManager->AddDrawRectCommand(gizmoRect, gizmoColor, 2.0f, LAYER_DEBUG_OVERLAY, 9999.0f);
}
