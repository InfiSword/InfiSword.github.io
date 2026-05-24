#pragma once
#include "Monster.h"

enum class HoundState {
	IDLE = (int)CombatantState::IDLE,
	WALK = (int)CombatantState::WALK,
	CHASE = (int)CombatantState::CHASE,
	ATTACK = (int)CombatantState::ATTACK,
	ATTACK_PRE = (int)CombatantState::ATTACK_PRE,
	HIT = (int)CombatantState::HIT,
	DEATH = (int)CombatantState::DEATH,

	HOWL = (int)CombatantState::MAX_COMMON,
	COUNT
};

class Hound : public Monster
{
public:
    Hound(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
          const std::wstring& baseDir = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
    virtual ~Hound() override;

    virtual void Init() override;
	virtual void RenderDebugOverlay() override;
    virtual void Damaged(int damage) override;
    virtual bool OnInteraction(GameObject* obj) override;

	bool GetHowled() const { return m_bHasHowled; }
      bool HasHowlStarted() const { return m_hasHowlStarted; }
      bool HasHowlFinished() const { return m_bHasHowled; }
      void SetCombatEnabled(bool enabled);

protected:
    virtual void UpdateAI(float deltaTime) override;
    virtual void UpdateMovement(float deltaTime) override;

    virtual int UpdateIdle(float deltaTime) override;
    virtual int UpdateWalk(float deltaTime) override;
    virtual int UpdateChase(float deltaTime) override;

    virtual void OnAttackHit() override;
    virtual void OnAttackEnd() override;
    virtual void OnHitEnd() override;
    virtual void Die() override;

protected:
    bool m_bHasHowled;
  bool m_hasHowlStarted;
  bool m_isCombatEnabled;
};
