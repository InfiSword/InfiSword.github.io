#pragma once

namespace ResourcePathUtils { struct TileResourceDef; }

struct TileCacheData {
	Gdiplus::Bitmap* bitmap;
	TileCacheData() : bitmap(nullptr) {}
};

class GameObject;
class Collider;

class CameraManager : public CSingleTon<CameraManager>
{
	friend class CSingleTon<CameraManager>;
public:
	CameraManager();
	~CameraManager() { Release(); }

	void Init();
	void Update(float deltaTime);
	void Release();

	Gdiplus::RectF GetViewportWorldRect() const;
	Gdiplus::PointF GetCameraPos() const { return m_cameraPos; }
	void SetCameraPos(float x, float y) { m_cameraPos = { x, y }; }

	void SetTarget(GameObject* target) { m_target = target; }
	GameObject* GetTarget() { return m_target; }
	void FollowTarget();
	void SetFollowMode(bool enabled) { m_followMode = enabled; }
	
	// 통합된 영역 쿼리 함수
	void QueryObjectsInteractive(const Gdiplus::RectF& area, std::vector<GameObject*>& outObjects, bool onlyInteraction = true);

	void RenderVisibleTiles(const MapData* mapData);
	void RenderVisibleGameObjects();
	bool IsObjectInViewport(GameObject* pObj) const;
	void ClearTileCache();

	void SetWalkableBoundsFromMapData(const MapData* mapData);
	void SetWalkableBounds(float minX, float minY, float maxX, float maxY);

	Gdiplus::PointF WorldToScreen(float worldX, float worldY) const;
	Gdiplus::PointF ScreenToWorld(float screenX, float screenY) const;

	float GetAvgRenderVisibleGameObjectsMs() const {
#ifdef _DEBUG
		return m_avgRenderVisibleGameObjectsMs;
#else
		return 0.0f;
#endif
	}

	float GetAvgRenderVisibleTilesMs() const {
#ifdef _DEBUG
		return m_avgRenderVisibleTilesMs;
#else
		return 0.0f;
#endif
	}

	float GetAvgCullVisibleGameObjectsMs() const {
#ifdef _DEBUG
		return m_avgCullVisibleGameObjectsMs;
#else
		return 0.0f;
#endif
	}

private:
	GameObject* m_target = nullptr;
	std::unordered_map<UINT, TileCacheData> m_tileCache;
	std::vector<GameObject*> m_queryBuffer; // 공간 분할 쿼리용

	Gdiplus::RectF m_lastViewportRect = { 0, 0, 0, 0 };
	Gdiplus::PointF m_cameraPos = { 0, 0 };
	float m_walkableMinX = 0, m_walkableMinY = 0, m_walkableMaxX = 0, m_walkableMaxY = 0;
	int m_lastStartTileX = -1, m_lastStartTileY = -1, m_lastEndTileX = -1, m_lastEndTileY = -1;

#ifdef _DEBUG
	unsigned long long m_cullVisibleGameObjectsSampleCount = 0;
	unsigned long long m_renderVisibleGameObjectsSampleCount = 0;
	unsigned long long m_renderVisibleTilesSampleCount = 0;
	float m_avgCullVisibleGameObjectsMs = 0.0f;
	float m_avgRenderVisibleGameObjectsMs = 0.0f;
	float m_avgRenderVisibleTilesMs = 0.0f;
#endif
	bool m_followMode = true;
	bool m_hasWalkableBounds = false;

	void LoadTileBitmap(const ResourcePathUtils::TileResourceDef& tileData, TileCacheData& cacheData);
	void CleanupUnusedTileCache(const MapData* mapData, int startX, int endX, int startY, int endY);
};
