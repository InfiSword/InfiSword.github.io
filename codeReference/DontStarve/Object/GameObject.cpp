#include "99_Default/pch.h"
#include "GameObject.h"
#include "Component/Transform/Transform.h"
#include "Component/Sprite/SpriteRenderer.h"
#include "Component/Collider/Collider.h"
#include "../01_Manager/TimeManager/TimeManager.h"
#include "../03_Animation/Animator.h"
#include "../01_Manager/ObjectManager/ObjectManager.h"

bool GameObject::g_bRenderDebugOverlay = false;

GameObject::GameObject(GameObjectID id, float x, float y, float pivotX, float pivotY, Direction dir,
	const std::wstring& resourcePath, const std::wstring& imageName, ColliderType colliderType, bool isActive, bool isInteractive)
	: Object(), m_id(id), m_isInteractive(isInteractive), m_type(GameObjectType::GO_TYPE_NONE), m_isDead(false)
{
	SetActive(isActive);
}

GameObject::~GameObject()
{
	Release();
}

void GameObject::Init() {
	for (auto& component : m_components) {
		if (component) {
			component->Init();
		}
	}
	// 초기 위치에 따른 그리드 셀 설정
	if (m_type != GO_TYPE_UI) {
		ObjectManager::GetInstance()->UpdateObjectGrid(this);
	}
}

void GameObject::LateInit() {
	for (auto& component : m_components) {
		if (component) {
			component->LateInit();
		}
	}
}

void GameObject::Update(float deltaTime) {

	for (auto& component : m_components) {
		if (component && component->IsEnabled()) {
			component->Update(deltaTime);
		}
	}
	UpdateCoroutines(deltaTime);
}

void GameObject::LateUpdate() {

	// 위치/크기 변경이 있었다면 바운딩 박스 캐시 강제 갱신
	if (m_isBoundsDirty) {
		GetBounds();
	}

	// 공간 분할 그리드 셀 갱신은 별도의 플래그(m_isGridDirty)로 관리.
	// 쿼리 도중 GetBounds()가 호출되어 m_isBoundsDirty가 먼저 리셋되더라도 
	// LateUpdate에서 그리드 동기화가 누락되지 않도록 보장함.
	if (m_isGridDirty) {
		if (m_type != GO_TYPE_UI) {
			ObjectManager::GetInstance()->UpdateObjectGrid(this);
		}
		m_isGridDirty = false;
	}

	for (auto& component : m_components) {
		if (component && component->IsEnabled()) {
			component->LateUpdate();
		}
	}
}

void GameObject::Release()
{

	// 공간 분할 그리드에서 제거
	if (m_type != GO_TYPE_UI) {
		ObjectManager::GetInstance()->RemoveGameObject(this);
	}

	StopAllCoroutines();

	std::wstring().swap(m_name);

	// 컴포넌트 해제
	for (auto& component : m_components) {
		if (component) {
			component->Release();
		}
	}
	m_components.clear();
	m_components.shrink_to_fit();
}

bool GameObject::OnInteraction(GameObject* obj)
{
	if (!obj || !IsEnabled() || !CanInteract())
		return false;

	return true;
}

Gdiplus::RectF GameObject::GetBounds()
{
	return m_cachedBounds;
}

void GameObject::StartCoroutine(CoroutineHandle coroutine)
{
	if (!coroutine) return;
	m_coroutines.push_back(std::move(coroutine));
}

void GameObject::StopAllCoroutines()
{
	m_coroutines.clear();
	m_coroutines.shrink_to_fit();
}

void GameObject::UpdateCoroutines(float deltaTime)
{
	if (m_coroutines.empty()) return;

	size_t i = 0;
	while (i < m_coroutines.size()) {
		bool stillRunning = m_coroutines[i](deltaTime);
		if (!stillRunning) {
			if (i == m_coroutines.size() - 1) {
				m_coroutines.pop_back();
			}
			else {
				m_coroutines[i] = std::move(m_coroutines.back());
				m_coroutines.pop_back();
			}
		}
		else {
			++i;
		}
	}
}

void GameObject::Render()
{
}

void GameObject::MainColliderGizmo()
{
	Collider* pMainCol = GetMainCollider();
	if (pMainCol && pMainCol->IsEnabled()) {
		pMainCol->RenderGizmo();
	}
}
