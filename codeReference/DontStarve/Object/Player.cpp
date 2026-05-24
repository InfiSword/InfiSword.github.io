#include "99_Default/pch.h"
#include "Player.h"
#include "../../../01_Manager/InputManager/InputManager.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/SoundManager/SoundManager.h"
#include "../../../02_GameObject/UI/Inventory.h"
#include "../../../02_GameObject/Component/Sprite/SpriteRenderer.h"
#include "../../../03_Animation/Animator.h"
#include "../../../03_Animation/AnimationClip.h"
#include "../../Item/Tool/Tool.h"
#include "../../Component/Transform/Transform.h"
#include "../../Component/Collider/Collider.h"
#include "../../Component/Collider/BoxCollider.h"
#include "../../../01_Manager/ColliderManager/ColliderManager.h"
#include "../../../01_Manager/RenderManager/RenderManager.h"
#include "../../../01_Manager/GameProgressManager/GameProgressManager.h"

Player::Player(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& resourcePath, const std::wstring& imageName, ColliderType colliderType)
	: Combatant(id, x, y, pivotX, pivotY, dir, resourcePath, imageName, true, false, colliderType),
	m_playerSpeed(330.f), m_stopThreshold(10),
	m_equippedSlotIndex(-1), m_equippedItemID(GOID_NONE), m_inventory(nullptr),
	m_pendingInteractionTarget(nullptr), m_activeInteractionTarget(nullptr),
	isMoveToGoal(false), m_bInputEnabled(true), m_speedModifier(1.0f), m_slowTimer(0.0f), m_walkSoundTimer(0.0f)
{
	m_hp = 100;
	m_maxHp = 100;
	ChangeState((int)PlayerState::IDLE);
	m_type = GO_TYPE_PLAYER;
}

Player::~Player() { Release(); }

void Player::Init()
{
	Combatant::Init();

	// Player 전용 공격 박스 설정 
	SetupAttackBox(120, 160, 0, -60);

	// Animator 생성 후 애니메이션 등록
	if (!m_animator) {
		m_animator = AddComponent<Animator>(m_spriteRenderer);
	}

	DataManager* pRM = DataManager::GetInstance();
	const ResourcePathUtils::ObjectResourceDef* objData = pRM->GetObjectResourceInfo(GetID());
	if (!objData) return;
	std::wstring base = objData->baseDir;

	// IDLE
	std::wstring idleDownPath = base + L"\\Idle\\Wilson_Idle_Down.png";
	m_animator->RegisterAnimation((int)PlayerState::IDLE, DIR_DOWN, idleDownPath,
		126, 189, 7, 64, objData->pivotX, objData->pivotY, true, 0.03f);

	std::wstring idleUpPath = base + L"\\Idle\\Wilson_Idle_Up.png";
	m_animator->RegisterAnimation((int)PlayerState::IDLE, DIR_UP, idleUpPath,
		128, 193, 7, 64, objData->pivotX, objData->pivotY, true, 0.03f);

	std::wstring idleSidePath = base + L"\\Idle\\Wilson_Idle_Side.png";
	m_animator->RegisterAnimation((int)PlayerState::IDLE, DIR_LEFT, idleSidePath,
		135, 194, 7, 64, objData->pivotX, objData->pivotY, true, 0.03f);
	m_animator->RegisterAnimation((int)PlayerState::IDLE, DIR_RIGHT, idleSidePath,
		135, 194, 7, 64, objData->pivotX, objData->pivotY, true, 0.03f);

	// WALK(RUN)
	std::wstring runDownPath = base + L"\\Run\\Wilson_Run_Down.png";
	m_animator->RegisterAnimation((int)PlayerState::WALK, DIR_DOWN, runDownPath,
		139, 226, 6, 33, objData->pivotX, objData->pivotY, true, 0.03f);

	std::wstring runUpPath = base + L"\\Run\\Wilson_Run_Up.png";
	m_animator->RegisterAnimation((int)PlayerState::WALK, DIR_UP, runUpPath,
		133, 231, 6, 33, objData->pivotX, objData->pivotY, true, 0.03f);

	std::wstring runSidePath = base + L"\\Run\\Wilson_Run_Side.png";
	m_animator->RegisterAnimation((int)PlayerState::WALK, DIR_LEFT, runSidePath,
		142, 226, 6, 33, objData->pivotX, objData->pivotY, true, 0.03f, true);
	m_animator->RegisterAnimation((int)PlayerState::WALK, DIR_RIGHT, runSidePath,
		141, 226, 6, 33, objData->pivotX, objData->pivotY, true, 0.03f);

	// PICKUP (마지막 프레임에 종료 이벤트)
	const UINT PICKUP_TOTAL_FRAMES = 20;
	const int PICKUP_LAST_FRAME = PICKUP_TOTAL_FRAMES - 1; // 19
	std::wstring pickupPath = base + L"\\Interact\\Interact_wilson_pickup_pst_down.png";
	for (int dir = DIR_UP; dir < DIR_COUNT; dir++) {
		m_animator->RegisterAnimation((int)PlayerState::PICKUP, (Direction)dir, pickupPath,
			127, 201, 6, PICKUP_TOTAL_FRAMES, objData->pivotX, objData->pivotY, false, 0.02f);
		// AnimationClip에 직접 이벤트 등록 및 콜백 설정
		AnimationClip* clip = m_animator->GetAnimationClip((int)PlayerState::PICKUP, (Direction)dir);
		if (clip) {
			clip->AddEventFrame(PICKUP_LAST_FRAME, L"pickup_end");
			clip->AddEventFrame(1, L"pickup_Trigger");
			// 이벤트 콜백 설정
			clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
				if (eventName == L"pickup_end") {
					this->OnPickupEnd();
				}
				else if (eventName == L"pickup_Trigger")
				{
					SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PlayerSound/pickup_sound.wav");
					if (m_activeInteractionTarget) {
						Transform* targetT = m_activeInteractionTarget->GetComponent<Transform>();
						if (targetT) {
							m_transform->SetPosition(targetT->GetX(), targetT->GetY() + 0.1f);
							this->SetSpatialDirty();
						}
					}
				}
				});
		}
	}

	// CHOP (이벤트 적용: chop_hit 시 나무에게 데미지, 마지막 프레임에 종료 이벤트)
	const UINT CHOP_TOTAL_FRAMES = 36;
	const int CHOP_HIT_FRAME = 4;
	const int CHOP_LAST_FRAME = CHOP_TOTAL_FRAMES - 1; // 35
	std::wstring chopPath = base + L"\\Axe\\axe_wilson_chop_loop_down.png";
	for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
		m_animator->RegisterAnimation((int)PlayerState::CHOP, (Direction)dir, chopPath,
			284, 248, 6, CHOP_TOTAL_FRAMES, CHOP_PIVOT_X, CHOP_PIVOT_Y, false, 0.01f);
		// AnimationClip에 직접 이벤트 등록 및 콜백 설정
		AnimationClip* clip = m_animator->GetAnimationClip((int)PlayerState::CHOP, (Direction)dir);
		if (clip) {
			clip->AddEventFrame(CHOP_HIT_FRAME, L"chop_hit");
			clip->AddEventFrame(CHOP_LAST_FRAME, L"chop_end");
			clip->AddEventFrame(1, L"chop_Trigger");
			// 이벤트 콜백 설정
			clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
				if (eventName == L"chop_hit") {
					SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PlayerSound/Chop_tree.wav");
					this->OnChopHit();
				}
				else if (eventName == L"chop_end") {
					this->OnChopEnd();
				}
				else
				{
					Transform* targetT = m_activeInteractionTarget->GetComponent<Transform>();
					if (targetT) {
						m_transform->SetPosition(targetT->GetX(), targetT->GetY() + 0.1f);
						this->SetSpatialDirty();
					}
				}
				});
		}
	}

	// MINE (곡괭이 채광, Down 방향 고정 — Chop과 동일 패턴)
	const UINT MINE_TOTAL_FRAMES = 39;
	const int MINE_HIT_FRAME = 4;
	const int MINE_LAST_FRAME = MINE_TOTAL_FRAMES - 1; // 50
	std::wstring pickaxePath = base + L"\\Pickaxe\\pickaxe_wilson_pickaxe_loop_down.png";
	for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
		m_animator->RegisterAnimation((int)PlayerState::MINE, (Direction)dir, pickaxePath,
			/*311*/0, /*360*/ 0, 6, MINE_TOTAL_FRAMES, MINE_PIVOT_X, MINE_PIVOT_Y, false, 0.01f);
		AnimationClip* clip = m_animator->GetAnimationClip((int)PlayerState::MINE, (Direction)dir);
		if (clip) {
			clip->AddEventFrame(MINE_HIT_FRAME, L"mine_hit");
			clip->AddEventFrame(MINE_LAST_FRAME, L"mine_end");
			clip->AddEventFrame(1, L"mine_Trigger");
			clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
				if (eventName == L"mine_hit") {
					SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PlayerSound/pickaxe_hitrock.wav");
					this->OnMineHit();
				}
				else if (eventName == L"mine_end") {
					this->OnMineEnd();
				}
				else
				{
					Transform* targetT = m_activeInteractionTarget->GetComponent<Transform>();
					if (targetT) {
						m_transform->SetPosition(targetT->GetX(), targetT->GetY() + 0.1f);
						this->SetSpatialDirty();
					}
				}
				});
		}
	}

	const UINT ATTACK_TOTAL_FRAMES = 36;
	const int ATTACK_HIT_FRAME = 16;
	const int ATTACK_LAST_FRAME = ATTACK_TOTAL_FRAMES - 1;
	std::wstring attackDownPath = base + L"\\Attack\\Wilson_Attack_down.png";
	std::wstring attackUpPath = base + L"\\Attack\\Wilson_Attack_up.png";
	std::wstring attackSidePath = base + L"\\Attack\\Wilson_Attack_side.png";
	m_animator->RegisterAnimation((int)PlayerState::ATTACK, DIR_DOWN, attackDownPath,
		209, 221, 4, ATTACK_TOTAL_FRAMES, objData->pivotX, objData->pivotY, false, 0.02f);
	m_animator->RegisterAnimation((int)PlayerState::ATTACK, DIR_UP, attackUpPath,
		199, 218, 4, ATTACK_TOTAL_FRAMES, objData->pivotX, objData->pivotY, false, 0.02f);
	m_animator->RegisterAnimation((int)PlayerState::ATTACK, DIR_LEFT, attackSidePath,
		207, 217, 4, ATTACK_TOTAL_FRAMES, objData->pivotX, objData->pivotY, false, 0.02f, false);
	m_animator->RegisterAnimation((int)PlayerState::ATTACK, DIR_RIGHT, attackSidePath,
		207, 217, 4, ATTACK_TOTAL_FRAMES, objData->pivotX, objData->pivotY, false, 0.02f);

	for (int dir = DIR_UP; dir <= DIR_RIGHT; dir++) {
		AnimationClip* clip = m_animator->GetAnimationClip((int)PlayerState::ATTACK, (Direction)dir);
		if (clip) {
			clip->AddEventFrame(ATTACK_HIT_FRAME, L"attack_hit");
			clip->AddEventFrame(ATTACK_LAST_FRAME, L"attack_end");
			clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
				if (eventName == L"attack_hit") this->OnAttackHit();
				else if (eventName == L"attack_end") this->OnAttackEnd();
				});
		}
	}

	// 공격 콜라이더는 Combatant::Init()에서 이미 생성됨

	if (!m_inventory) {
		m_inventory = new Inventory(this);
	}

	ChangeState(m_state);

	if (m_inventory)
		m_inventory->Init();

	// GameProgressManager에서 상태 복원
	if (GameProgressManager::GetInstance()->HasSavedPlayerState()) {
		RestoreState(GameProgressManager::GetInstance()->GetPlayerState());
	}
}

void Player::ToggleEquipItem(int slotIndex)
{
	if (slotIndex < 0 || slotIndex >= INVENTORY_SLOT_COUNT)
		return;

	const ItemSlot& targetSlot = m_inventory->GetSlot(slotIndex);
	if (targetSlot.IsEmpty())
		return;

	GameObjectID itemID = targetSlot.id;
	const ToolInfo* toolInfo = DataTable::GetToolInfo(itemID);
	if (!toolInfo)
		return;

	if (m_equippedSlotIndex == slotIndex) {
		// 해제
		m_equippedSlotIndex = -1;
		m_equippedItemID = GOID_NONE;
		// 기본 공격력/사거리로 복구 (필요시)
		m_damage = 10; 
		m_attackRange = 50.0f;
	}
	else {
		// 새로운 슬롯 장착
		m_equippedSlotIndex = slotIndex;
		m_equippedItemID = itemID;
		m_damage = static_cast<int>(toolInfo->damage);
		m_attackRange = toolInfo->attackRange;
	}
}

void Player::SetSlow(float duration, float modifier)
{
	m_slowTimer = (std::max)(m_slowTimer, duration);
	m_speedModifier = (std::min)(m_speedModifier, modifier);
}

void Player::Damaged(int damage)
{
	Entity::Damaged(damage);
}

void Player::Die()
{
	this->SetActive(false);
}

void Player::Heal(int amount)
{
	if (amount <= 0) return;
	m_hp = (std::min)(m_maxHp, m_hp + amount);
}

PlayerStateSnapshot Player::SaveState() const
{
	PlayerStateSnapshot snapshot;
	snapshot.hp = m_hp;
	snapshot.equippedSlotIndex = m_equippedSlotIndex;

	if (m_inventory) {
		snapshot.inventoryItems = m_inventory->GetAllItemsSnapshot();
	}

	return snapshot;
}

void Player::RestoreState(const PlayerStateSnapshot& snapshot)
{
	// HP 복원
	int currentHp = m_hp;
	if (snapshot.hp > currentHp) {
		Heal(snapshot.hp - currentHp);
	}
	else if (snapshot.hp < currentHp) {
		Damaged(currentHp - snapshot.hp);
	}

	// 인벤토리 복원
	if (m_inventory) {
		m_inventory->ClearAllItems();
		for (const auto& item : snapshot.inventoryItems) {
			m_inventory->AddItem(item.first, item.second);
		}
	}

	// 장착 슬롯 복원
	if (snapshot.equippedSlotIndex >= 0) {
		ToggleEquipItem(snapshot.equippedSlotIndex);
	}
}

void Player::SetTargetPosition(float worldX, float worldY) {

	m_targetWorldPos = Gdiplus::PointF(worldX, worldY);
	ChangeState((int)PlayerState::WALK);
	isMoveToGoal = true;

	float dx = worldX - m_transform->GetX();
	float dy = worldY - m_transform->GetY();

	Direction newDirection;
	if (std::abs(dx) > std::abs(dy)) {
		newDirection = (dx > 0) ? DIR_RIGHT : DIR_LEFT;
	}
	else {
		newDirection = (dy > 0) ? DIR_DOWN : DIR_UP;
	}

	if (this->m_transform->GetDirection() != newDirection)
	{
		m_transform->SetDirection(newDirection);
	}
}

void Player::Update(float deltaTime)
{
	Entity::Update(deltaTime);

	if (m_slowTimer > 0) {
		m_slowTimer -= deltaTime;
		if (m_slowTimer <= 0) {
			m_slowTimer = 0;
			m_speedModifier = 1.0f;
		}
	}

	if (m_inventory) {
		m_inventory->Update(deltaTime);
	}

	HandleMovement();

	// 이동 사운드 처리
	if (m_state == (int)PlayerState::WALK) {
		m_walkSoundTimer -= deltaTime;
		if (m_walkSoundTimer <= 0) {
			SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PlayerSound/walk_sound.wav");
			m_walkSoundTimer = 0.4f; // 0.4초 간격
		}
	}
	else {
		m_walkSoundTimer = 0.0f;
	}

	float moveSpeedThisFrame = m_playerSpeed * m_speedModifier * deltaTime;

	if (isMoveToGoal)
	{
		float dx = m_targetWorldPos.X - m_transform->GetX();
		float dy = m_targetWorldPos.Y - m_transform->GetY();
		float distance = std::sqrt(dx * dx + dy * dy);

		// 공격 대상으로 이동 중일 때: 사거리 내면 이동 중단 후 ATTACK
		if (m_attackTarget && m_attackTarget->IsEnabled()) {
			Transform* targetT = m_attackTarget->GetComponent<Transform>();
			if (targetT) {
				float ax = targetT->GetX() - m_transform->GetX();
				float ay = targetT->GetY() - m_transform->GetY();
				float distToTarget = std::sqrt(ax * ax + ay * ay);

				// 공격 사거리 내에 들어오면 즉시 중단 (콜라이더 외곽선 충돌이 아닌 사거리 기준)
				if (distToTarget <= m_attackRange) {
					isMoveToGoal = false;
					Direction faceDir = (std::abs(ax) > std::abs(ay)) ? (ax > 0 ? DIR_RIGHT : DIR_LEFT) : (ay > 0 ? DIR_DOWN : DIR_UP);
					m_transform->SetDirection(faceDir);
					ChangeState((int)PlayerState::ATTACK);
					return;
				}
			}
		}

		// 도착 여부 판별
		bool isArrived = false;
		if (m_pendingInteractionTarget && m_pendingInteractionTarget->IsEnabled()) {
			// 상호작용 시: 사용자 요청에 따라 외곽선에서 멈추지 않고 "정확히 오브젝트 위치" 근처까지 이동
			// 중심점 기준 m_stopThreshold(10) 또는 한 프레임 이동 거리 이내면 도착으로 판정
			isArrived = (distance <= m_stopThreshold) || (distance <= moveSpeedThisFrame);
		}
		else {
			// 단순 이동(땅 클릭)인 경우
			isArrived = (distance <= m_stopThreshold) || (moveSpeedThisFrame > 0.f && distance <= moveSpeedThisFrame);
		}

		if (isArrived) {
			// 땅 클릭 이동 시에만 최종 좌표를 클릭 지점으로 보정
			if (!m_pendingInteractionTarget) m_transform->SetPosition(m_targetWorldPos.X, m_targetWorldPos.Y);
			ClampPositionToMapBounds();
			isMoveToGoal = false;

			// 이동 완료 후 자신의 위치를 그리드 시스템에 동기화 (최적화용)
			//ObjectManager::GetInstance()->UpdateObjectGridCell(this);

			// 공격 대상(몬스터)으로 이동한 경우: 사거리 안이면 방향 맞추고 ATTACK, 밖이면 몬스터 현재 위치로 다시 이동
			if (m_attackTarget && m_attackTarget->IsEnabled()) {
				Transform* targetT = m_attackTarget->GetComponent<Transform>();
				if (targetT) {
					float ax = targetT->GetX() - m_transform->GetX();
					float ay = targetT->GetY() - m_transform->GetY();
					float distToTarget = std::sqrt(ax * ax + ay * ay);
					if (distToTarget <= m_attackRange) {

						Direction faceDir = (std::abs(ax) > std::abs(ay)) ? (ax > 0 ? DIR_RIGHT : DIR_LEFT) : (ay > 0 ? DIR_DOWN : DIR_UP);
						m_transform->SetDirection(faceDir);
						ChangeState((int)PlayerState::ATTACK);
						return;
					}
					// 몬스터가 움직여서 사거리 밖이면, 현재 몬스터 위치로 다시 이동
					SetTargetPosition(targetT->GetX(), targetT->GetY());
					return;
				}
			}

			// 도착 시 상호작용 처리
			if (m_pendingInteractionTarget && m_pendingInteractionTarget->IsEnabled()) {
				m_activeInteractionTarget = m_pendingInteractionTarget;
				m_pendingInteractionTarget = nullptr;

				// 방향 설정 
				Direction dir;
				if (std::abs(dx) > std::abs(dy))
					dir = (dx > 0) ? DIR_RIGHT : DIR_LEFT;
				else
					dir = (dy > 0) ? DIR_DOWN : DIR_UP;
				m_transform->SetDirection(dir);

				if (!OnInteraction(m_activeInteractionTarget)) {
					ChangeState((int)PlayerState::IDLE);
				}
			}
			else {
				// 대기 중인 상호작용이 없거나 유효하지 않으면 IDLE 상태로 전환
				m_pendingInteractionTarget = nullptr;
				ChangeState((int)PlayerState::IDLE);
			}
		}
		else {
			float moveDist = (std::min)(moveSpeedThisFrame, distance);
			m_transform->SetPosition(m_transform->GetX() + (dx / distance) * moveDist, m_transform->GetY() + (dy / distance) * moveDist);
			ClampPositionToMapBounds();  // 맵 경계 체크

			// 이동 중에는 기본적으로 WALK 애니메이션을 사용하지만,
			// 이미 PICKUP/CHOP 등 상호작용 애니메이션이 진행 중이면 덮어쓰지 않는다.
			if (m_state == (int)PlayerState::IDLE || m_state == (int)PlayerState::WALK)
			{
				ChangeState((int)PlayerState::WALK);
			}
		}
	}
}

void Player::TryStartInteraction(float worldX, float worldY)
{
	CameraManager* cameraManager = CameraManager::GetInstance();
	if (!cameraManager) return;

	// 마우스 위치 주변의 객체들만 쿼리 (통합된 QueryObjectsInArea 사용)
	std::vector<GameObject*> queryResults;
	float range = 100.0f;
	Gdiplus::RectF queryRect(worldX - range, worldY - range, range * 2, range * 2);
	cameraManager->QueryObjectsInteractive(queryRect, queryResults, true);

	GameObject* target = nullptr;
	float maxY = -1e9f;

	for (auto* obj : queryResults) {
		Collider* mainCol = obj->GetMainCollider();
		if (mainCol && mainCol->IsEnabled() && mainCol->ContainsPoint(worldX, worldY)) {
			float curY = obj->GetComponent<Transform>()->GetY();
			if (!target || curY > maxY) {
				target = obj;
				maxY = curY;
			}
		}
	}

	// CHOP/MINE/PICKUP 진행 중인 경우
	if (m_state == PlayerState::CHOP || m_state == PlayerState::MINE || m_state == PlayerState::PICKUP) {
		if (target != nullptr && target == m_activeInteractionTarget) {
			// 현재 상호작용 중인 대상과 동일 → 애니메이션 유지 (재시작 안 함)
			return;
		}
		// 다른 대상이나 빈 공간 클릭 → 현재 상호작용 중단
		m_activeInteractionTarget = nullptr;
		m_pendingInteractionTarget = nullptr;
		ChangeState((int)PlayerState::IDLE);
	}

	// 이동 중 대기 중인 상호작용 초기화
	m_pendingInteractionTarget = nullptr;
	m_attackTarget = nullptr;

	// 상호작용 가능 여부 확인
	bool canInteract = false;
	if (target && target->IsEnabled()) {
		GameObjectID objID = target->GetID();
		GameObjectType objType = target->GetType();
		switch (objType) {
		case GO_TYPE_NATURAL_ENVIRONMENT:
			if (objID == GOID_NORMAL_TREE_SHORT || objID == GOID_NORMAL_TREE_NORMAL || objID == GOID_NORMAL_TREE_TALL) {
				if (m_equippedItemID != GOID_NONE) {
					GameObjectID equippedID = m_equippedItemID;
					canInteract = (equippedID == GOID_TOOL_RED_AXE || equippedID == GOID_TOOL_SWAP_AXE);
				}
			}
			else if (objID == GOID_NORMAL_ROCK || objID == GOID_GOLD_ROCK) {
				if (m_equippedItemID != GOID_NONE) {
					GameObjectID equippedID = m_equippedItemID;
					canInteract = (equippedID == GOID_TOOL_GOLDEN_PICKAXE || equippedID == GOID_TOOL_PICKAXE);
				}
			}
			else {
				canInteract = true;
			}
			break;
		case GO_TYPE_ITEM:
			canInteract = true;
			break;
		case GO_TYPE_BUILDING:
			if (objID == GOID_BUILDING_PIGHOUSE)
			{
				if (m_equippedItemID != GOID_NONE)
				{
					GameObjectID equippedID = m_equippedItemID;
					canInteract = (equippedID == GOID_TOOL_HAMMER);
					m_attackTarget = target;
				}
			}
			else if (objID == GOID_BUILDING_SPIDER_NORMALEGG || objID == GOID_BUILDING_SPIDER_SMALLEGG || objID == GOID_BUILDING_SPIDER_TALLEGG)
			{
				const ToolInfo* toolInfo = DataTable::GetToolInfo(m_equippedItemID);
				canInteract = (toolInfo != nullptr && toolInfo->damage > 0);
				if (canInteract) m_attackTarget = target;
			}
			break;
		case GO_TYPE_MONSTER:
		{
			// 몬스터 클릭: 장착 도구가 공격 가능할 때만 추격 및 공격
			const ToolInfo* toolInfo = DataTable::GetToolInfo(m_equippedItemID);
			canInteract = (toolInfo != nullptr && toolInfo->damage > 0);
			if (canInteract)
				m_attackTarget = target;
			break;
		}
		default:
			canInteract = false;
			break;
		}
	}

	if (!target || !canInteract || !target->CanInteract()) {
		return;
	}

	Transform* targetTransform = target->GetComponent<Transform>();
	if (!targetTransform) return;

	SetTargetPosition(targetTransform->GetX(), targetTransform->GetY());
	m_pendingInteractionTarget = target;
}


void Player::FinalizePickup()
{
	if (!m_activeInteractionTarget || !m_activeInteractionTarget->IsEnabled()) {
		m_activeInteractionTarget = nullptr;
		return;
	}

	if (!m_inventory) {
		m_activeInteractionTarget = nullptr;
		return;
	}

	GameObjectID objID = m_activeInteractionTarget->GetID();
	GameObjectType objType = m_activeInteractionTarget->GetType();

	bool itemAdded = false;
	auto* objMgr = ObjectManager::GetInstance();

	if (objType == GO_TYPE_ITEM) {
		if (m_inventory->AddItem(objID, 1)) {
			itemAdded = true;
			objMgr->RemoveGameObject(m_activeInteractionTarget);
		}
	}
	else if (objType == GO_TYPE_NATURAL_ENVIRONMENT)
	{
		Entity* entity = dynamic_cast<Entity*>(m_activeInteractionTarget);
		GameObjectID itemID = entity ? entity->GetDropItemID() : GOID_NONE;
		int itemCount = entity ? entity->GetDropItemCount() : 0;
		if (itemID != GOID_NONE && itemCount > 0) {
			if (m_inventory->AddItem(itemID, itemCount)) {
				itemAdded = true;
				objMgr->RemoveGameObject(m_activeInteractionTarget);
			}
		}
	}

	// 상태 전환은 OnPickupEnd()에서 처리하므로 여기서는 UpdateAnimatorState() 호출하지 않음
	m_activeInteractionTarget = nullptr;
}

void Player::OnPickupEnd()
{
	if (m_state != (int)PlayerState::PICKUP) return;
	// PICKUP 종료 처리: 아이템 획득 및 상태 전환
	FinalizePickup();

	m_transform->SetDirection(DIR_DOWN);
	ChangeState((int)PlayerState::IDLE);
}

void Player::OnChopHit()
{
	if (m_state != PlayerState::CHOP || !m_activeInteractionTarget) return;

	m_activeInteractionTarget->Damaged(m_damage);
	Entity* entity = dynamic_cast<Entity*>(m_activeInteractionTarget);
	if (entity && entity->IsDead())
		m_activeInteractionTarget = nullptr;
}

void Player::OnChopEnd()
{
	if (m_state != (int)PlayerState::CHOP) return;

	m_transform->SetDirection(DIR_DOWN);
	m_activeInteractionTarget = nullptr;
	m_pendingInteractionTarget = nullptr;

	ChangeState((int)PlayerState::IDLE);
}

void Player::OnMineHit()
{
	if (m_state != PlayerState::MINE || !m_activeInteractionTarget) return;

	GameObjectID objID = m_activeInteractionTarget->GetID();
	if (objID != GOID_NORMAL_ROCK && objID != GOID_GOLD_ROCK)
		return;

	m_activeInteractionTarget->Damaged(m_damage);
	Entity* entity = dynamic_cast<Entity*>(m_activeInteractionTarget);
	if (entity && entity->IsDead()) m_activeInteractionTarget = nullptr;
}

void Player::OnMineEnd()
{
	if (m_state != (int)PlayerState::MINE) return;

	m_transform->SetDirection(DIR_DOWN);
	m_activeInteractionTarget = nullptr;
	m_pendingInteractionTarget = nullptr;

	ChangeState((int)PlayerState::IDLE);
}

void Player::OnAttackHit()
{
	if (m_state != PlayerState::ATTACK || !m_attackCollider || !m_transform) return;

	// Combatant의 공통 공격 처리 사용
	ProcessAttackHit(m_damage);
}

void Player::OnAttackEnd()
{
	if (m_state != (int)PlayerState::ATTACK) return;
	Combatant::OnAttackEnd();
	ChangeState((int)PlayerState::IDLE);
}

bool Player::OnInteraction(GameObject* obj)
{
	GameObjectID objID = obj->GetID();
	GameObjectType objType = obj->GetType();

	switch (objType)
	{
	case GO_TYPE_NATURAL_ENVIRONMENT:
		if (objID == GOID_NORMAL_TREE_SHORT || objID == GOID_NORMAL_TREE_NORMAL || objID == GOID_NORMAL_TREE_TALL)
		{
			m_transform->SetDirection(DIR_DOWN);
			ChangeState((int)PlayerState::CHOP);
			return true;
		}
		if (objID == GOID_NORMAL_ROCK || objID == GOID_GOLD_ROCK)
		{
			m_transform->SetDirection(DIR_DOWN);
			ChangeState((int)PlayerState::MINE);
			return true;
		}
		{
			// PICKUP 상태 설정 (애니메이션 이벤트에서 종료 처리)
			ChangeState((int)PlayerState::PICKUP);
			return true;
		}
	case GO_TYPE_ITEM:
		// PICKUP 상태 설정 (애니메이션 이벤트에서 종료 처리)
		ChangeState((int)PlayerState::PICKUP);
		return true;
	default:
		// 알 수 없는 타입의 경우 IDLE 상태로 전환
		ChangeState((int)PlayerState::IDLE);
		return false;
	}

	return false;
}

void Player::LateUpdate()
{
	GameObject::LateUpdate();
}

void Player::Render()
{
	Entity::Render();

	if (m_inventory) {
		m_inventory->Render(m_equippedSlotIndex);
	}
}

void Player::RenderDebugOverlay()
{
	// 부모 클래스(Combatant)의 공통 디버그 렌더링 호출
	Combatant::RenderDebugOverlay();
}

void Player::LateInit() {
}

void Player::Release() {
	// Player 전용 정리 작업
	if (m_inventory) {
		delete m_inventory;
		m_inventory = nullptr;
	}
	m_equippedItemID = GOID_NONE;
	m_pendingInteractionTarget = nullptr;
	m_activeInteractionTarget = nullptr;

	// 부모 클래스의 Release() 호출하여 컴포넌트 정리
	Combatant::Release();
}

// 좌/우클릭 입력 처리. 인벤토리 UI 영역 위 클릭은 월드 상호작용·이동에 사용하지 않음.
void Player::HandleMovement()
{
	if (GetHp() <= 0 || !m_bInputEnabled)
		return;

	InputManager* inputManager = InputManager::GetInstance();
	if (!inputManager)
		return;

	CameraManager* cameraManager = CameraManager::GetInstance();
	if (!cameraManager)
		return;

	// Space: 현재 방향으로 공격 (CHOP/MINE/PICKUP/ATTACK 중이 아닐 때만, 장착 도구가 공격 가능할 때만)
	if (inputManager->IsKeyPressed(VK_SPACE)) {
		const ToolInfo* toolInfo = DataTable::GetToolInfo(m_equippedItemID);
		if (toolInfo && toolInfo->damage > 0) {
			if (m_state != (int)PlayerState::CHOP && m_state != (int)PlayerState::MINE && m_state != (int)PlayerState::PICKUP && m_state != (int)PlayerState::ATTACK) {
				ChangeState((int)PlayerState::ATTACK);
			}
		}
	}

	if (inputManager->IsLButtonClicked()) {
		POINT mousePos = inputManager->GetMousePos();
		float sx = static_cast<float>(mousePos.x);
		float sy = static_cast<float>(mousePos.y);
		if (ObjectManager::GetInstance()->IsScreenPointBlockedByUI(sx, sy))
			return;

		Gdiplus::PointF worldPos = cameraManager->ScreenToWorld(sx, sy);
		TryStartInteraction(worldPos.X, worldPos.Y);
	}
	else if (inputManager->IsRButtonClicked())
	{
		// 우클릭 시 모든 상호작용 및 공격 대상 초기화
		m_pendingInteractionTarget = nullptr;
		m_attackTarget = nullptr;
		m_activeInteractionTarget = nullptr;

		POINT mousePos = inputManager->GetMousePos();
		float sx = static_cast<float>(mousePos.x);
		float sy = static_cast<float>(mousePos.y);

		// 인벤토리 우클릭 처리 - 처리되면 더 이상 진행하지 않음
		if (m_inventory && m_inventory->HandleRightClick(sx, sy, this)) {
			return;
		}

		// UI 영역 블로킹 체크 (인벤토리 외 다른 UI)
		if (ObjectManager::GetInstance()->IsScreenPointBlockedByUI(sx, sy)) {
			return;
		}

		if (m_state == (int)PlayerState::ATTACK) {
			OnAttackEnd();
		}

		Gdiplus::PointF worldPos = cameraManager->ScreenToWorld(sx, sy);
		SetTargetPosition(worldPos.X, worldPos.Y);
	}
}
