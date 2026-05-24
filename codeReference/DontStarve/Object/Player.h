#pragma once
#include "../Combatant.h"
#include "../../Item/Tool/Tool.h"
#include "../../../01_Manager/GameProgressManager/GameProgressManager.h"

class Inventory;

enum PlayerState {
	IDLE = (int)CombatantState::IDLE,
	WALK = (int)CombatantState::WALK,
	ATTACK = (int)CombatantState::ATTACK,
	HIT = (int)CombatantState::HIT,
	DEATH = (int)CombatantState::DEATH,

	PICKUP = (int)CombatantState::MAX_COMMON,
	CHOP,
	MINE,
	MOVING_TO_TARGET,
	COUNT,
};

enum Debuff
{
	DEBUFF_NONE = -1,
	SLOW,
	FIRE,
	DEBUFF_COUNT
};

class Player : public Combatant
{
public:
	Player(GameObjectID id, float x, float y, float pivotX, float pivotY, 
		Direction dir, const std::wstring& resourcePath = L"", 
		const std::wstring& imageName = L"", ColliderType colliderType = COLLIDER_BOX);
	virtual ~Player() override;

	virtual void Init() override;
	virtual void LateInit() override;
	virtual void Update(float deltaTime) override;
	virtual void LateUpdate() override;
	virtual void Render() override;
	virtual void Release() override;
	virtual void RenderDebugOverlay() override;

	void SetTargetPosition(float worldX, float worldY);
	void HandleMovement();
	virtual bool OnInteraction(GameObject* obj) override;

	void SetInputEnabled(bool enabled) { m_bInputEnabled = enabled; }
	bool IsInputEnabled() const { return m_bInputEnabled; }

	Inventory* GetInventory() { return m_inventory; }

	PlayerState GetPlayerState() const { return (PlayerState)m_state; }

	Tool* GetEquippedItem() const { return nullptr; } // 기존 호환성을 위해 nullptr 반환 (추후 제거 가능)
	GameObjectID GetEquippedItemID() const { return m_equippedItemID; }
	int GetEquippedSlotIndex() const { return m_equippedSlotIndex; };

	void ToggleEquipItem(int slotIndex);

	void SetSlow(float duration, float modifier);

	virtual void Damaged(int damage) override;
	virtual void Die() override;

	void Heal(int amount);

	// 상태 저장/복원 (씬 전환용)
	PlayerStateSnapshot SaveState() const;
	void RestoreState(const PlayerStateSnapshot& snapshot);

private:

	void TryStartInteraction(float worldX, float worldY);
	void FinalizePickup();

	// 애니메이션 이벤트 핸들러 함수들
	void OnPickupEnd();
	void OnChopHit();
	void OnChopEnd();
	void OnMineHit();
	void OnMineEnd();
	void OnAttackHit();
	virtual void OnAttackEnd() override;

	Inventory* m_inventory;
	GameObject* m_pendingInteractionTarget;  // 이동 후 상호작용할 대상
	GameObject* m_activeInteractionTarget;   // 현재 상호작용 중인 대상

	const float CHOP_PIVOT_X = 0.3f;
	const float CHOP_PIVOT_Y = 0.9f;
	const float MINE_PIVOT_X = 0.5f;
	const float MINE_PIVOT_Y = 0.9f;

	bool isMoveToGoal;
	float m_playerSpeed;
	float m_speedModifier;
	float m_slowTimer;
	float m_walkSoundTimer;

	Gdiplus::PointF m_targetWorldPos;
	float m_stopThreshold;

	int m_equippedSlotIndex;
	GameObjectID m_equippedItemID;
	bool m_bInputEnabled;
};
