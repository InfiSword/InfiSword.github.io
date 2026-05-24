#include "99_Default/pch.h"
#include "../../../01_Manager/CameraManager/CameraManager.h"
#include "../../../01_Manager/ResourceManager/ResourceManager.h"
#include "Sapling.h"

Sapling::Sapling(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir, const std::wstring& resourcePath, const std::wstring& imageName, ColliderType colliderType)
	: Entity(id, x, y, pivotX, pivotY, dir, resourcePath, imageName, colliderType, true, true)
{
	m_type = GO_TYPE_NATURAL_ENVIRONMENT;
	SetDropItem(GOID_ITEM_NORMAL_TWIGS, 1);
}

Sapling::~Sapling() {}

void Sapling::Init()
{
	Entity::Init();
	// 비트맵은 생성자에서 이미 로드됨
	// Transform은 이제 Scale만 관리 (기본값 1.0f)
	// 크기는 sprite의 실제 크기를 사용하므로 Transform에 설정할 필요 없음
}

void Sapling::LateInit()
{
}

void Sapling::Update(float deltaTime)
{
	// 부모 클래스의 Update() 호출하여 컴포넌트 업데이트
	Entity::Update(deltaTime);
}

void Sapling::LateUpdate()
{
}

void Sapling::Release()
{
	// Sapling 전용 정리 작업
	
	// 부모 클래스의 Release() 호출하여 컴포넌트 정리
	Entity::Release();
}

bool Sapling::OnInteraction(GameObject* obj)
{
	return Entity::OnInteraction(obj);
}

void Sapling::Damaged(int damage)
{
}
