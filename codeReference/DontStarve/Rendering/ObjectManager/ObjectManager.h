#pragma once
#include "../../../Header/SingleTon.h"
#include "../DataManager/DataManager.h"
#include "../../02_GameObject/GameObject.h"

class Player;
class Entity;
class UIImage;
class UIButton;
class UIText;
class MenuUI;
class HPUI;
class GameOverUI;
class GameClearUI;
class IntroNoticeUI;

class ObjectManager : public CSingleTon<ObjectManager>
{
	friend class CSingleTon<ObjectManager>;

public:
	ObjectManager();
	~ObjectManager();

	void Init();
	void LateInit();
	void Update(float deltaTime);
	void LateUpdate();
	void Render();
	void Release();

	void AddGameObject(GameObject* pObj);
	void RemoveGameObject(GameObject* pObj);
	void ClearAllObjects();
	bool IsScreenPointBlockedByUI(float screenX, float screenY) const;
	
	// 월드 오브젝트를 Rect 기준으로 찾는다.
	void QueryObjectsInRectArea(const Gdiplus::RectF& rect, std::vector<GameObject*>& outObjects);

	// ID로 오브젝트 찾기 
	GameObject* FindGameObject(GameObjectID id);
	template <typename T>
	T* FindGameObject(GameObjectID id) { 
		GameObject* obj = FindGameObject(id);
		if (!obj) return nullptr;
		return static_cast<T*>(obj); 
	}

	Player* GetPlayer() const;
	const std::vector<GameObject*>& GetWorldObjects() const { return m_worldObjects; }
	const std::vector<GameObject*>& GetUIObjects() const { return m_uiObjects; }

	// 게임오브젝트 생성 헬퍼
	GameObject* CreateObject(GameObjectID id, float x, float y);

	// 특정 타입 명시적 생성용 템플릿
	template <typename T>
	T* CreateObject(GameObjectID id, float x, float y)
	{
		const ResourcePathUtils::ObjectResourceDef* data = DataManager::GetInstance()->GetObjectResourceInfo(id);

		// 데이터가 없는 경우를 대비한 기본값 설정
		float px = 0.5f, py = 0.5f;
		std::wstring bd = L"", im = L"";
		ColliderType ct = COLLIDER_BOX;

		if (data) {
			px = data->pivotX; py = data->pivotY;
			bd = data->baseDir; im = data->imageName;
			ct = data->hasCollider ? data->colliderType : COLLIDER_BOX;
		}

		T* pObj = new T(id, x, y, px, py, DIR_DOWN, bd, im, ct);
		if (!pObj) return nullptr;

		if (data) ApplyObjectData(pObj, data);

		AddGameObject(pObj);
		pObj->Init();

		return pObj;
	}

	// 데이터 기반 초기화 헬퍼 (콜라이더 등 설정)
	void ApplyObjectData(GameObject* pObj, const ResourcePathUtils::ObjectResourceDef* data);

	// 공간 분할 관련
	void UpdateObjectGrid(GameObject* pObj);
	void AddToGrid(GameObject* pObj);
	void RemoveFromGrid(GameObject* pObj);

	// UI 생성 헬퍼
	UIButton* CreateButton(GameObjectID id, float width, float height, const std::wstring& normalPath, const std::wstring& hoverPath, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY, float x, float y, std::function<void()> onClick);
	UIImage*  CreateImage(GameObjectID id, float width, float height, RenderLayer layer, const std::wstring& path, float depth, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY, float x, float y);
	UIText*   CreateText(GameObjectID id, float width, float height, const std::wstring& text, Gdiplus::Color color, float fontSize, Gdiplus::FontStyle fontStyle, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY, float x, float y, float sortKey = 0, Gdiplus::StringAlignment hAlign = Gdiplus::StringAlignmentCenter, Gdiplus::StringAlignment vAlign = Gdiplus::StringAlignmentCenter);
	MenuUI* CreateMenuUI();
	HPUI*   CreateHPUI(Entity* pTarget, const std::wstring& name, float width, float height, Gdiplus::Color bgColor, Gdiplus::Color barColor, Gdiplus::Color nameColor, float anchorX, float anchorY, float pivotX, float pivotY, float x, float y, float bgSortKey, float barSortKey, bool usePortrait, bool useName);
	GameOverUI* CreateGameOverUI();
	GameClearUI* CreateGameClearUI();
	IntroNoticeUI* CreateIntroNoticeUI();

private:
	bool IsManagedObject(const GameObject* pObj) const;

	// 공간 분할 설정
	static constexpr int GRID_CELL_SIZE = 256;
	static constexpr int GRID_WIDTH = (MAP_WIDTH * TILE_SIZE / GRID_CELL_SIZE) + 2;
	static constexpr int GRID_HEIGHT = (MAP_HEIGHT * TILE_SIZE / GRID_CELL_SIZE) + 2;

	std::vector<GameObject*> m_spatialGrid[GRID_WIDTH][GRID_HEIGHT];
	uint32_t m_spatialQueryStamp = 1;

	std::vector<GameObject*> m_worldObjects;
	std::vector<GameObject*> m_uiObjects;
	std::vector<GameObject*> m_inactiveObjects;  // 인벤토리 등 시스템 내부 관리용 리스트
	std::vector<GameObject*> m_pendingDeletions; // 삭제 지연 큐
	
	Player* m_cachedPlayer; // 플레이어 캐시

	// 삭제 지연 처리
	void ProcessPendingDeletions();

};
