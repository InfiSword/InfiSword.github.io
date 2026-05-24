#include "99_Default/pch.h"
#include "Entity.h"
#include "../../01_Manager/CameraManager/CameraManager.h"
#include "../../01_Manager/RenderManager/RenderManager.h"
#include "../../03_Animation/Animator.h"
#include "../../02_GameObject/Component/Transform/Transform.h"
#include "../../02_GameObject/Component/Sprite/SpriteRenderer.h"
#include "../../02_GameObject/Component/Collider/BoxCollider.h"
#include "../../02_GameObject/Component/Collider/CircleCollider.h"
#include "../../01_Manager/ResourceManager/ResourceManager.h"

Entity::Entity(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType, bool isActive, bool isInteractive)
	:GameObject(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType, isActive, isInteractive),
	m_dropItemID(GOID_NONE),
	m_dropItemCount(0),
	m_isDead(false),
	m_hp(100),
	m_maxHp(100),
	m_state(0),
	m_animator(nullptr),
	m_entityCollider(nullptr),
	m_colliderType(colliderType)
{
	// Transform 컴포넌트 추가
	this->m_transform = AddComponent<Transform>();
	m_transform->SetPosition(x, y);
	m_transform->SetDirection(dir);

	// SpriteRenderer 컴포넌트 추가
	this->m_spriteRenderer = AddComponent<SpriteRenderer>();
	m_spriteRenderer->SetLayer(LAYER_WORLD_OBJECT);
	if (!imageName.empty())
	{
		ResourceManager* pRM = ResourceManager::GetInstance();
		std::wstring fullPath = ResourcePathUtils::BuildResourcePath(baseDir, imageName);
		if (!fullPath.empty()) {
			// 로드 시점에 전달받은 피벗 적용
			if (auto sprite = pRM->LoadSprite(fullPath, { pivotX, pivotY })) {
				m_spriteRenderer->SetSprite(sprite);
			}
		}
	}
}

Entity::~Entity()
{
}

void Entity::Init()
{
	GameObject::Init();

	// 콜라이더 타입에 따라 생성 및 캐싱
	if (m_colliderType == COLLIDER_BOX)
	{
		m_entityCollider = GetComponent<BoxCollider>();
	}
	else if (m_colliderType == COLLIDER_CIRCLE)
	{
		m_entityCollider = GetComponent<CircleCollider>();
	}
}

GameObjectID Entity::GetDropItemID() const
{
	return m_dropItemID;
}

int Entity::GetDropItemCount() const
{
	return m_dropItemCount;
}

void Entity::SetDropItem(GameObjectID itemID, int count)
{
	m_dropItemID = itemID;
	m_dropItemCount = count;
}

bool Entity::OnInteraction(GameObject* obj)
{
	if (m_isDead) return false;
	return GameObject::OnInteraction(obj);
}

Gdiplus::RectF Entity::GetBounds()
{
	if (!m_isBoundsDirty) return m_cachedBounds;

	if (!m_transform) {
		m_cachedBounds = { 0,0,0,0 };
		m_isBoundsDirty = false;
		return m_cachedBounds;
	}

	float w = 32, h = 32, px = 0.5f, py = 0.5f;
	if (m_animator != nullptr) {
		if (auto sprite = m_animator->GetCurrentFrame().sprite) {
			w = sprite->sourceRect.Width; h = sprite->sourceRect.Height;
			px = sprite->pivot.X; py = sprite->pivot.Y;
		}
	}
	else if (m_spriteRenderer != nullptr) {
		if (auto sprite = m_spriteRenderer->GetSpriteHandle()) {
			w = sprite->sourceRect.Width; h = sprite->sourceRect.Height;
			px = sprite->pivot.X; py = sprite->pivot.Y;
		}
	}
	w *= m_transform->GetScaleX(); h *= m_transform->GetScaleY();

	m_cachedBounds = { m_transform->GetX() - w * px, m_transform->GetY() - h * py, w, h };
	m_isBoundsDirty = false;
	return m_cachedBounds;
}

void Entity::ChangeState(int newState, bool restart)
{
	m_state = newState;
	if (m_animator && m_transform) {
		m_animator->SetState(m_state, m_transform->GetDirection(), restart);
	}
}

void Entity::Damaged(int damage)
{
	if (m_isDead || damage <= 0) return;

	m_hp -= damage;
	if (m_hp <= 0) {
		m_hp = 0;
		m_isDead = true;

		// 죽었을 때 콜라이더를 즉시 비활성화하여 상호작용(클릭 등) 방지
		if (m_entityCollider) m_entityCollider->SetColliderEnabled(false);

		Die();
	}
}

void Entity::Render()
{
	if (!IsEnabled() || !m_transform) return;

	if (m_spriteRenderer && m_spriteRenderer->IsEnabled()) 
	{
		m_spriteRenderer->Render();
	}
}

void Entity::Update(float deltaTime)
{
	// 부모 클래스의 Update() 호출하여 컴포넌트 업데이트
	GameObject::Update(deltaTime);
}

void Entity::Release()
{
	m_animator = nullptr;
	m_transform = nullptr;
	m_spriteRenderer = nullptr;
	m_entityCollider = nullptr;

	GameObject::Release();
}

void Entity::ClampPositionToMapBounds()
{
	if (!m_transform) return;

	float x = m_transform->GetX();
	float y = m_transform->GetY();

	const auto bounds = GetBounds();

	const float mapMinX = 0.0f;
	const float mapMinY = 0.0f;
	const float mapMaxX = static_cast<float>(MAP_WIDTH * TILE_SIZE);
	const float mapMaxY = static_cast<float>(MAP_HEIGHT * TILE_SIZE);

	float offsetX = 0.0f;
	float offsetY = 0.0f;

	if (bounds.X < mapMinX) {
		offsetX = mapMinX - bounds.X;
	}
	else if (bounds.X + bounds.Width > mapMaxX) {
		offsetX = mapMaxX - (bounds.X + bounds.Width);
	}

	if (bounds.Y < mapMinY) {
		offsetY = mapMinY - bounds.Y;
	}
	else if (bounds.Y + bounds.Height > mapMaxY) {
		offsetY = mapMaxY - (bounds.Y + bounds.Height);
	}

	if (offsetX != 0.0f || offsetY != 0.0f) {
		m_transform->SetPosition(x + offsetX, y + offsetY);
	}
}

