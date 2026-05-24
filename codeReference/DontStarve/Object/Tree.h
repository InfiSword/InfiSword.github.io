#pragma once
#include "../Entity.h"

enum class TreeState {
	IDLE = 0,    // 일반 상태 (서있음)
	CHOP,        // 벌목중인 상태
	FALL,        // 넘어지는 중인 상태
	FALLEN,      // 넘어진 후의 상태
	COUNT
};

class Tree : public Entity
{
public:
    Tree(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
		const std::wstring& baseDir = L"", const std::wstring& imageName = L"", 
		ColliderType colliderType = COLLIDER_BOX, bool isActive= true, bool isInteractive=true);
    virtual ~Tree();

    virtual void Init() override;
    virtual void Update(float deltaTime) override;
    virtual void Release() override;

	virtual bool OnInteraction(GameObject* obj) override;
    virtual void Damaged(int damage) override;
    virtual void Die() override;

    TreeState GetTreeState() const { return m_treeState; }

private:
    TreeState m_treeState;
	int m_hp;
	int maxHp;
	float m_baseX, m_baseY;
	float m_shakeDuration;
	float m_shakeAmount;
	float m_shakeSpeed;
	bool m_isShaking;
};
