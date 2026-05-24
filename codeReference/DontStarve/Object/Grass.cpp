#include "99_Default/pch.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../Component/Sprite/SpriteRenderer.h"
#include "Grass.h"

Grass::Grass(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
	: Entity(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType, true, true)
	, m_grassState(GrassState::IDLE)
	, m_regrowTimer(0.0f)
{
	m_type = GO_TYPE_NATURAL_ENVIRONMENT;
	SetDropItem(GOID_ITEM_CUT_NORMAL_GRASS, 1);
}

Grass::~Grass() {}

void Grass::Init()
{
	Entity::Init();
}

void Grass::Update(float deltaTime)
{
	Entity::Update(deltaTime);

	if (m_grassState == GrassState::REGROWING)
	{
		m_regrowTimer += deltaTime;
		if (m_regrowTimer >= 10.0f) // 10초 후 재생성
		{
			m_grassState = GrassState::IDLE;
			if (m_spriteRenderer) m_spriteRenderer->SetActive(true);
		}
	}
}

void Grass::Release()
{
	Entity::Release();
}

bool Grass::OnInteraction(GameObject* obj)
{
	if (m_grassState != GrassState::IDLE) return false;

	// 상호작용 시 아이템 드롭 처리 등 가능
	m_grassState = GrassState::PICKED;
	if (m_spriteRenderer) m_spriteRenderer->SetActive(false);
	m_grassState = GrassState::REGROWING;
	m_regrowTimer = 0.0f;

	return Entity::OnInteraction(obj);;
}

void Grass::Damaged(int damage)
{
}
