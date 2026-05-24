#pragma once
#include "../Combatant.h"

class Monster : public Combatant
{
public:
	enum class AggroType
	{
		ON_RANGE,
		ALWAYS,
		ON_HIT_THEN_RANGE
	};

protected:
	float m_attackCooldownTimer;
	float m_walkSpeed;
	float m_runSpeed;
	float m_targetX;
	float m_targetY;
	float m_distToPlayerSq;

	float m_aiTickTimer;
	float m_aiTickInterval;
	float m_wanderRadius;
	float m_aggroRadius;
	float m_deaggroRadius;
	float m_idleTimer;
	float m_idleDuration;

	Gdiplus::PointF m_dirToPlayer;
	AggroType m_aggroType;

	bool m_hasBeenHit;
	bool m_bCanChase;

public:
	Monster(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
		const std::wstring& baseDir = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
	virtual ~Monster() override;

	virtual void Init() override;
	virtual void Update(float deltaTime) override;
	virtual void Damaged(int damage) override;

	void SetupAggro(AggroType type, float aggroRadius = 300.0f, float deaggroRadius = 500.0f);

	void SetCanChase(bool canChase) { m_bCanChase = canChase; }
	bool CanChase() const { return m_bCanChase; }

protected:
	virtual void UpdateAI(float deltaTime);
	virtual void UpdateMovement(float deltaTime);

	// New Virtual State Update Functions
	virtual int UpdateIdle(float deltaTime);
	virtual int UpdateWalk(float deltaTime);
	virtual int UpdateChase(float deltaTime);
	virtual int UpdateAttack(float deltaTime);
	virtual int UpdateHit(float deltaTime);

	virtual void OnAttackHit() {}
	virtual void OnAttackEnd() {}
	virtual void OnHitEnd() {}
	virtual void OnDeathEnd();
	virtual void ResetAggroSession() {}
	virtual void ResolveWanderCenter(float& outX, float& outY) const;

	void MoveTowardPlayer(float deltaTime, float speed);
	void MoveTowardLocation(float deltaTime, float speed);

	void LookAtPlayer();
	Direction ResolveFacingDirection(const Gdiplus::PointF& dir);
};
