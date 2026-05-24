#pragma once
#include "../Entity.h"

enum class GrassState {
	IDLE = 0,    // 일반 상태
	PICKED,      // 뽑힌 상태
	REGROWING,   // 재성장 중
	COUNT
};

class Grass : public Entity
{
public:
    Grass(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& baseDir = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
    virtual ~Grass();

    virtual void Init() override;
    virtual void Update(float deltaTime) override;
    virtual void Release() override;

    virtual bool OnInteraction(GameObject* obj) override;
    virtual void Damaged(int damage) override;

    GrassState GetGrassState() const { return m_grassState; }

private:
    GrassState m_grassState;
    float m_regrowTimer;
};
