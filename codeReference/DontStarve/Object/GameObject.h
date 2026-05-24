#pragma once

#include "Object.h"
#include "Component/Component.h"


class SpriteRenderer;
class Collider;

// 코루틴 핸들: deltaTime을 받아 실행 중이면 true, 완료되면 false를 반환하는 함수
using CoroutineHandle = std::function<bool(float)>;

enum GameObjectType
{
	GO_TYPE_NONE = 0,
	GO_TYPE_NATURAL_ENVIRONMENT,
	GO_TYPE_MONSTER,
	GO_TYPE_BUILDING,
	GO_TYPE_ITEM,
	GO_TYPE_PLAYER,
	GO_TYPE_UI,
};

class GameObject : public Object
{
protected:
	GameObjectID m_id;				// 오브젝트 아이디

	std::wstring m_name;					// 해당 게임 오브젝트 이름
	bool m_isInteractive;			// 상호작용 가능 여부
	GameObjectType m_type;					// 게임 오브젝트 타입
	// 컴포넌트 관리					
	std::vector<std::unique_ptr<Component>> m_components;

	// 캐싱된 바운딩 박스 
	Gdiplus::RectF m_cachedBounds = { 0, 0, 0, 0 };
	bool m_isBoundsDirty = true;
	bool m_isGridDirty = true; // 공간 분할 그리드 갱신용 플래그

	// 공간 분할용 그리드 좌표
	int m_gridCellX = -1;
	int m_gridCellY = -1;
	uint32_t m_lastSpatialQueryStamp = 0;

	bool m_isDead = false; // 삭제 여부 플래그

private:
	std::vector<CoroutineHandle> m_coroutines;

public:

	GameObject(GameObjectID id, float x = 0, float y = 0, float pivotX = 0.5f, float pivotY = 0.5f,
		Direction dir = DIR_DOWN, const std::wstring& resourcePath = L"", const std::wstring& imageName = L"",
		ColliderType colliderType = COLLIDER_BOX, bool isActive = true, bool isInteractive = true);

	virtual ~GameObject() override;

	virtual void Init();
	virtual void LateInit();
	virtual void Update(float deltaTime);
	virtual void LateUpdate();
	virtual void Render();
	virtual void Release();

	// 삭제 관련
	bool IsDead() const { return m_isDead; }
	void SetDead(bool dead) { m_isDead = dead; }

	// Entity 여부 반환
	virtual bool IsEntity() const { return false; }

	// 공간 분할용 접근자
	int GetGridCellX() const { return m_gridCellX; }
	int GetGridCellY() const { return m_gridCellY; }
	void SetGridCell(int x, int y) { m_gridCellX = x; m_gridCellY = y; }
	uint32_t GetLastSpatialQueryStamp() const { return m_lastSpatialQueryStamp; }
	void SetLastSpatialQueryStamp(uint32_t stamp) { m_lastSpatialQueryStamp = stamp; }

	virtual Collider* GetMainCollider() const { return nullptr; }
	virtual void SetMainCollider(Collider* col) {};

	// 코루틴 시스템
	void StartCoroutine(CoroutineHandle coroutine);
	void StopAllCoroutines();

	virtual void RenderDebugOverlay() {}
	virtual void MainColliderGizmo();

	// 상호작용 관련
	virtual bool OnInteraction(GameObject* obj);
	virtual void OnCollision(GameObject* other) {}
	virtual void Damaged(int damage) {}
	virtual bool CanInteract() const { return m_isInteractive; }
	virtual void SetInteractive(bool interactive) { m_isInteractive = interactive; }

	// 바운딩 박스 관련
	virtual Gdiplus::RectF GetBounds();
	void SetSpatialDirty() { m_isBoundsDirty = true; m_isGridDirty = true; }

	static bool g_bRenderDebugOverlay;

	template <typename T, typename... Args>
	T* AddComponent(Args&&... args) {
		auto newComponent = std::make_unique<T>(this, std::forward<Args>(args)...);
		T* componentPtr = newComponent.get();
		m_components.push_back(std::move(newComponent));
		return componentPtr;
	}

	template <typename T>
	T* GetComponent() const
	{
		for (const auto& component : m_components) {
			if (!component) continue;
			T* target = dynamic_cast<T*>(component.get());
			if (target) {
				return target;
			}
		}
		return nullptr;
	}

	// inline 함수
	inline GameObjectID GetID() const { return m_id; }
	inline GameObjectType GetType() const { return m_type; }
	inline const std::wstring& GetName() const { return m_name; }

private:
	void UpdateCoroutines(float deltaTime);
};
