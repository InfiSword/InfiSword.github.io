#pragma once
#include "Collider.h"

class GameObject;

// 사각형 콜라이더
class BoxCollider : public Collider {
public:
    RECT m_boundingBox;   // 로컬 좌표 (상대 좌표)

    BoxCollider(GameObject* owner);
    virtual ~BoxCollider() = default;

    // 충돌 검사 메서드 구현
    virtual bool IntersectsCollider(const Collider* other) const override;
    
    virtual ColliderType GetColliderType() const override { return COLLIDER_BOX; }

    virtual bool ContainsPoint(float worldX, float worldY) const override;
    virtual void GetCenterWorld(float& outX, float& outY) const override;
    
    virtual Gdiplus::RectF GetWorldRect() const override;

    // 오브젝트(사각형) 콜라이더 설정
    void SetObjectCollider(int offsetX, int offsetY, int width, int height);

    // 월드 좌표 기준 bounding box 계산
    RECT GetWorldBoundingBox() const;
    
    // Gizmo 렌더링
    virtual void RenderGizmo() override;
};
