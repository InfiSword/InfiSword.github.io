#pragma once
#include "Collider.h"

class GameObject;

// 원형 콜라이더
class CircleCollider : public Collider {
public:
    float m_centerX;      // 로컬 좌표 기준 중심 X
    float m_centerY;      // 로컬 좌표 기준 중심 Y
    float m_radius;       // 반지름

    CircleCollider(GameObject* owner, float centerX = 0.0f, float centerY = 0.0f, float radius = 10.0f);
    virtual ~CircleCollider() = default;

    // 충돌 검사 메서드 구현
    virtual bool IntersectsCollider(const Collider* other) const override;
        // 월드 좌표로 변환된 boundingBox 가져오기 (원을 감싸는 사각형)

    virtual ColliderType GetColliderType() const override { return COLLIDER_CIRCLE; }

    virtual bool ContainsPoint(float worldX, float worldY) const override;
    virtual void GetCenterWorld(float& outX, float& outY) const override;
    
    virtual Gdiplus::RectF GetWorldRect() const override;

    // 원형 오브젝트 콜라이더 설정
    void SetObjectCollider(float centerX, float centerY, float radius);
    
    // 월드 좌표에서 원의 중심점과 반지름 가져오기
    void GetWorldCircle(float& centerX, float& centerY, float& radius) const;
    
    virtual void RenderGizmo() override;
};
