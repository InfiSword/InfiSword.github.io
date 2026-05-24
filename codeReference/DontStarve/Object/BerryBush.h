#pragma once
#include "../Entity.h"

class ResourceManager;

class BerryBush : public Entity
{
public:
    BerryBush(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& resourcePath = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
    virtual ~BerryBush();

    virtual void Init() override;
    virtual void LateInit() override;
    virtual void Update(float deltaTime) override;
    virtual void LateUpdate() override;
    virtual void Release() override;

    virtual bool OnInteraction(GameObject* obj) override;

    virtual void Damaged(int damage) override;
};

