#pragma once
#include "../Entity.h"

enum class RockState {
	INTACT = 0,   // 온전한 상태 (level1)
	CRACKED,      // 금 간 상태 (level2)  
	BROKEN,       // 깨진 상태 (level3)
	DESTROYED,    // 완전히 파괴된 상태
	COUNT
};

class Rock : public Entity
{
public:
    Rock(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,const std::wstring& baseDir = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
    virtual ~Rock();

    virtual void Init() override;
    virtual void Release() override;

	virtual bool OnInteraction(GameObject* obj) override;

    virtual void Damaged(int damage) override;
    virtual void Die() override;

    RockState GetRockState() const { return m_rockState; }

private:
    RockState m_rockState;
	std::shared_ptr<Sprite> m_spriteIntact;
	std::shared_ptr<Sprite> m_spriteCracked;
	std::shared_ptr<Sprite> m_spriteBroken;
};
