#pragma once
#include "../Entity.h"

class ResourceManager;

class Sapling : public Entity
{
public:
    Sapling(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& resourcePath = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
    virtual ~Sapling();

    virtual void Init() override;
    virtual void LateInit() override;
    virtual void Update(float deltaTime) override;
    virtual void LateUpdate() override;
    virtual void Release() override;

    virtual bool OnInteraction(GameObject* obj) override;
    virtual void Damaged(int damage) override;

private:
};
