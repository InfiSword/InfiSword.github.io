#include "99_Default/pch.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/DataManager/DataManager.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "../../../01_Manager/ObjectManager/ObjectManager.h"
#include "../../Component/Sprite/SpriteRenderer.h"
#include "../../Component/Transform/Transform.h"
#include "Rock.h"

Rock::Rock(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& baseDir, const std::wstring& imageName, ColliderType colliderType)
: Entity(id, x, y, pivotX, pivotY, dir, baseDir, imageName, colliderType, true, true)
, m_rockState(RockState::INTACT)
{
	m_type = GO_TYPE_NATURAL_ENVIRONMENT;
	if (id == GOID_NORMAL_ROCK) {
		m_maxHp = 120;
		m_hp = 120;
		SetDropItem(GOID_ITEM_NORMAL_ROCK, 2);
	}
	else if (id == GOID_GOLD_ROCK) {
		m_maxHp = 200;
		m_hp = 200;
		SetDropItem(GOID_ITEM_GOLD_ROCK, 1);
	}
	else {
		m_maxHp = 120;
		m_hp = 120;
	}
}

Rock::~Rock() {}

void Rock::Init()
{
	Entity::Init();
	const ResourcePathUtils::ObjectResourceDef* objData = DataManager::GetInstance()->GetObjectResourceInfo(GetID());
	if (!objData || objData->imageName.empty()) return;

	const std::wstring& baseDir = objData->baseDir;
	const std::wstring& imageName = objData->imageName;
	size_t pos = imageName.find_last_of(L'-');
	if (pos == std::wstring::npos) return;

	std::wstring prefix = imageName.substr(0, pos + 1);
	std::wstring fileName0 = prefix + L"0.png";
	std::wstring fileName1 = prefix + L"1.png";
	std::wstring fileName2 = prefix + L"2.png";

	ResourceManager* pRM = ResourceManager::GetInstance();
	std::wstring path0 = ResourcePathUtils::BuildResourcePath(baseDir, fileName0);
	std::wstring path1 = ResourcePathUtils::BuildResourcePath(baseDir, fileName1);
	std::wstring path2 = ResourcePathUtils::BuildResourcePath(baseDir, fileName2);

	m_spriteIntact = pRM->LoadSprite(path0, { objData->pivotX, objData->pivotY });
	m_spriteCracked = pRM->LoadSprite(path1, { objData->pivotX, objData->pivotY });
	m_spriteBroken = pRM->LoadSprite(path2, { objData->pivotX, objData->pivotY });

	if (m_spriteRenderer && m_spriteIntact)
		m_spriteRenderer->SetSprite(m_spriteIntact);
}

void Rock::Release()
{
	Entity::Release();
}

bool Rock::OnInteraction(GameObject* obj)
{
	return Entity::OnInteraction(obj);
}

void Rock::Damaged(int damage)
{
	if (m_rockState == RockState::DESTROYED || m_isDead) return;
	m_hp -= damage;

	if (m_hp <= 0) {
		m_rockState = RockState::DESTROYED;
		m_hp = 0;
		m_isDead = true;

		Die();
		return;
	}

	int threshold70 = m_maxHp * 70 / 100;
	int threshold30 = m_maxHp * 30 / 100;

	if (m_hp <= threshold30)
		m_rockState = RockState::BROKEN;
	else if (m_hp <= threshold70)
		m_rockState = RockState::CRACKED;
	else
		m_rockState = RockState::INTACT;

	if (m_spriteRenderer) {
		if (m_rockState == RockState::INTACT && m_spriteIntact)
			m_spriteRenderer->SetSprite(m_spriteIntact);
		else if (m_rockState == RockState::CRACKED && m_spriteCracked)
			m_spriteRenderer->SetSprite(m_spriteCracked);
		else if (m_rockState == RockState::BROKEN && m_spriteBroken)
			m_spriteRenderer->SetSprite(m_spriteBroken);
	}
}

void Rock::Die()
{
	float tx = m_transform ? m_transform->GetX() : 0.0f;
	float ty = m_transform ? m_transform->GetY() : 0.0f;

	ObjectManager* objMgr = ObjectManager::GetInstance();
	if (objMgr)
	{
		GameObjectID dropItemID = GetDropItemID();
		int count = GetDropItemCount();
		
		if (dropItemID != GOID_NONE && m_transform)
		{
			float tx = m_transform->GetX();
			float ty = m_transform->GetY();
			
			for (int i = 0; i < count; ++i)
			{
				float angle = (rand() / (float)RAND_MAX) * 6.28f;
				float spreadRadius = 20.0f + (rand() / (float)RAND_MAX) * 30.0f;
				float offsetX = cosf(angle) * spreadRadius;
				float offsetY = sinf(angle) * spreadRadius;
				objMgr->CreateObject(dropItemID, tx + offsetX, ty + offsetY);
			}
		}
		objMgr->RemoveGameObject(this);
	}
}
