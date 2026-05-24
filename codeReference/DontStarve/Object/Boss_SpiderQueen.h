#pragma once
#include "Spider.h"
#include <vector>

enum class SpiderQueenState
{
	IDLE = (int)CombatantState::IDLE,
	WALK = (int)CombatantState::WALK,
	CHASE = (int)CombatantState::CHASE,
	ATTACK = (int)CombatantState::ATTACK,
	HIT = (int)CombatantState::HIT,
	DEATH = (int)CombatantState::DEATH,

	BIRTH = (int)CombatantState::MAX_COMMON,
	TAUNT,
	COCOON,
	COCOON_HIT,
	COCOON_PRE,
	COMBO_ATTACK,
	POOP_PRE,
	POOP_LOOP,
	COUNT
};

class BoxCollider;

class Boss_SpiderQueen : public Spider
{
public:
	Boss_SpiderQueen(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
		const std::wstring& baseDir = L"", const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
	virtual ~Boss_SpiderQueen() override;

	virtual void Init() override;
	virtual void RenderDebugOverlay() override;
	virtual bool OnInteraction(GameObject* obj) override;
	virtual void Damaged(int damage) override;

	void SetCombatEnabled(bool enabled);
	bool HasTauntStarted() const { return m_hasTauntStarted; }
	bool HasTauntFinished() const { return m_bHasTaunted; }

	// 슈퍼아머 훅
	virtual bool IsInAttackState() const override { return m_state == (int)SpiderQueenState::ATTACK || m_state == (int)SpiderQueenState::COMBO_ATTACK || m_state == (int)SpiderQueenState::POOP_PRE || m_state == (int)SpiderQueenState::POOP_LOOP; }
	virtual int GetHitState() const override { return (int)SpiderQueenState::HIT; }
	virtual void TriggerAttackState() override { ChangeState((int)SpiderQueenState::ATTACK); }

protected:
	virtual void UpdateAI(float deltaTime) override;
	virtual void UpdateMovement(float deltaTime) override;

	virtual int UpdateIdle(float deltaTime) override;
	virtual int UpdateWalk(float deltaTime) override;
	virtual int UpdateChase(float deltaTime) override;

	virtual void OnAttackHit() override;
	virtual void OnAttackEnd() override;

	virtual void OnComboAttackHit();
	virtual void OnComboAttackEnd();

	virtual void OnPoopEgg();
	virtual void OnPoopEnd();

	virtual void OnHitEnd() override;

	// 애니메이션 이벤트 콜백
	void OnCocoonPreEnd();
	void OnBirthEnd();

private:
	void StartCocoonPhase();
	void EndCocoonPhase();
	void SummonSpider();
	void PreSpawnCocoonSpiders(int count);
	Spider* AcquirePooledSpider();

private:
	int m_bossPhase;
	float m_specialAttackCooldown;

	float m_comboAttackCooldown;
	int m_comboCount;

	float m_poopCooldown;
	int m_poopCount;

	float m_idleTimer;
	float m_idleDuration;

	// 회복 및 고치 로직 관련
	bool m_hasTriggeredCocoon;
	float m_cocoonTimer;
	float m_healTickTimer;

	float m_spawnOnHitCooldown;
	bool m_hasTauntStarted;
	bool m_isCombatEnabled;

	std::vector<Spider*> m_cocoonSpiderPool;
 	int m_cocoonPoolWarmCount;
};
