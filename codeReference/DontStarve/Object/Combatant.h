#pragma once
#include "Entity.h"

class BoxCollider;

enum class CombatantState {
    IDLE = 0,
	WALK,
    CHASE,
    ATTACK,
    ATTACK_PRE,
    HIT,
    DEATH,
    MAX_COMMON = 100
};

// 전투 가능한 엔티티의 기반 클래스 (Player & Monster)
class Combatant : public Entity
{
protected:
    // 전투 관련
    int m_damage;
    float m_attackRange;
    float m_attackCooldown;
    int m_attackHitFrame;
    int m_attackBoxWidth;
    int m_attackBoxHeight;
    BoxCollider* m_attackCollider;
    GameObject* m_attackTarget;
    bool m_bHitDuringAttack;
    bool m_bUseSuperArmor;
    
    // 공격 박스 (방향별)
    struct AttackBox {
        int offsetX, offsetY, width, height;
        
		AttackBox() : offsetX(0), offsetY(0), width(0), height(0) {}
        AttackBox(int ox, int oy, int w, int h) : offsetX(ox), offsetY(oy), width(w), height(h) {}
    };
    AttackBox m_attackBoxDown;
    AttackBox m_attackBoxUp;
    AttackBox m_attackBoxLeft;
    AttackBox m_attackBoxRight;

public:
    Combatant(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
              const std::wstring& baseDir = L"", const std::wstring& imageName = L"",
              bool isActive = true, bool isInteractive = false,
              ColliderType colliderType = COLLIDER_BOX);
    virtual ~Combatant() override;

    virtual void Init() override;
    virtual void Release() override;

    // 데미지 처리 (죽었을 때 공격 콜라이더 비활성화 로직 추가)
    virtual void Damaged(int damage) override;

    // 공격 관련 공통 메서드
    void SetupAttackBox(int width, int height, int offsetX = 0, int offsetY = 0);
    
	virtual void RenderDebugOverlay() override;

    // 공격 기즈모 표시 여부 제어
    static bool s_bShowAttackGizmo;

    virtual void ProcessAttackHit(int damage);
    virtual void OnAttackEnd();
	
    // 슈퍼아머 관련
    virtual bool IsInAttackState() const { 
        return m_state == (int)CombatantState::ATTACK || m_state == (int)CombatantState::ATTACK_PRE; 
    }
    virtual int GetHitState() const { return (int)CombatantState::HIT; }
    virtual void TriggerAttackState() { ChangeState((int)CombatantState::ATTACK); }

    bool CheckSuperArmorHit();
    void HandleAttackEndSuperArmor();

protected:
    void UpdateAttackBoxByDirection(Direction dir);
    virtual bool ApplyAttackDamageToTarget(int damage);
};
