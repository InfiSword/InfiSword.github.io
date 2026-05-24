#include "99_Default/pch.h"
#include <chrono>
#include "CameraManager.h"
#include "../../99_Default/ClientOptimatzationOption.h"
#include "../../02_GameObject/Component/Transform/Transform.h"
#include "../../02_GameObject/Component/Sprite/SpriteRenderer.h"
#include "../../02_GameObject/Component/Collider/Collider.h"
#include "../../03_Animation/Animator.h"
#include "../../02_GameObject/GameObject.h"
#include "../ObjectManager/ObjectManager.h"
#include "../RenderManager/RenderManager.h"
#include "../ColliderManager/ColliderManager.h"

CameraManager::CameraManager()
{
}

void CameraManager::Init() {
	m_cameraPos = { 0,0 };
	m_queryBuffer.clear();
	// 가시성 쿼리 버퍼는 프레임마다 재사용하므로 초기 reserve로 재할당을 줄인다.
	m_queryBuffer.reserve(2048);
	ClearTileCache();
	m_hasWalkableBounds = false;
	m_lastViewportRect = { 0, 0, 0, 0 };
	m_lastStartTileX = -1;
#ifdef _DEBUG
	m_cullVisibleGameObjectsSampleCount = 0;
	m_avgCullVisibleGameObjectsMs = 0.0f;
	m_avgRenderVisibleGameObjectsMs = 0.0f;
	m_renderVisibleGameObjectsSampleCount = 0;
	m_avgRenderVisibleTilesMs = 0.0f;
	m_renderVisibleTilesSampleCount = 0;
#endif
}

void CameraManager::Update(float deltaTime) {
	UNREFERENCED_PARAMETER(deltaTime);

	if (m_followMode && m_target)
		FollowTarget();
}

void CameraManager::Release() {
	m_queryBuffer.clear();
	m_queryBuffer.shrink_to_fit();
	ClearTileCache();
	m_lastViewportRect = { 0, 0, 0, 0 };
#ifdef _DEBUG
	m_cullVisibleGameObjectsSampleCount = 0;
	m_avgCullVisibleGameObjectsMs = 0.0f;
	m_avgRenderVisibleGameObjectsMs = 0.0f;
	m_renderVisibleGameObjectsSampleCount = 0;
	m_avgRenderVisibleTilesMs = 0.0f;
	m_renderVisibleTilesSampleCount = 0;
#endif
}

Gdiplus::RectF CameraManager::GetViewportWorldRect() const {
	const float halfW = static_cast<float>(WINCX) * 0.5f;
	const float halfH = static_cast<float>(WINCY) * 0.5f;
	return { m_cameraPos.X - halfW, m_cameraPos.Y - halfH, static_cast<float>(WINCX), static_cast<float>(WINCY) };
}

void CameraManager::FollowTarget()
{
	if (Transform* t = m_target->GetComponent<Transform>()) {
		float targetX = t->GetX();
		float targetY = t->GetY();

		m_cameraPos.X = targetX;
		m_cameraPos.Y = targetY;

		if (m_hasWalkableBounds) {
			m_cameraPos.X = (std::max)(m_walkableMinX + WINCX * 0.5f, (std::min)(m_walkableMaxX - WINCX * 0.5f, m_cameraPos.X));
			m_cameraPos.Y = (std::max)(m_walkableMinY + WINCY * 0.5f, (std::min)(m_walkableMaxY - WINCY * 0.5f, m_cameraPos.Y));
		}
	}
}

// 그리드 기반 입력 상호작용 처리
void CameraManager::QueryObjectsInteractive(const Gdiplus::RectF& area, std::vector<GameObject*>& outObjects, bool onlyInteraction)
{
	outObjects.clear();

	ObjectManager* objectManager = ObjectManager::GetInstance();
	if (!objectManager) return;

	std::vector<GameObject*> localQueryBuffer;
#ifdef _DEBUG
	std::vector<GameObject*>& queryBuffer = g_bEnableBufferReuse ? m_queryBuffer : localQueryBuffer;
#else
	std::vector<GameObject*>& queryBuffer = m_queryBuffer;
#endif

	queryBuffer.clear();
	objectManager->QueryObjectsInRectArea(area, queryBuffer);

	for (auto* obj : queryBuffer) {
		if (!obj || !obj->IsEnabled() || obj->IsDead()) continue;

		if (onlyInteraction && !obj->CanInteract()) continue;

		outObjects.push_back(obj);
	}
}

void CameraManager::RenderVisibleTiles(const MapData* mapData) {
#ifdef _DEBUG
	const auto profileStart = std::chrono::high_resolution_clock::now();
#endif

	if (!mapData) return;
	Gdiplus::RectF vp = GetViewportWorldRect();
	const float tileCullPadding = 8.f;

	// 공간 분할(Grid/Chunk) 기반 가시 범위 계산
	const int gridCellSize = 256;
	const int tilesPerGrid = gridCellSize / TILE_SIZE;

	int gsx = std::max(0, (int)floor((vp.X - tileCullPadding) / gridCellSize));
	int gex = (int)ceil((vp.X + vp.Width + tileCullPadding) / gridCellSize);
	int gsy = std::max(0, (int)floor((vp.Y - tileCullPadding) / gridCellSize));
	int gey = (int)ceil((vp.Y + vp.Height + tileCullPadding) / gridCellSize);

	// 그리드 단위로 캐시 관리 및 렌더링을 수행하기 위해 타일 인덱스 계산
	int sx = gsx * tilesPerGrid;
	int ex = gex * tilesPerGrid;
	int sy = gsy * tilesPerGrid;
	int ey = gey * tilesPerGrid;

	// 맵 범위 클램핑
	ex = std::min(MAP_WIDTH, ex);
	ey = std::min(MAP_HEIGHT, ey);

#ifdef _DEBUG
	if (!g_bEnableTileCaching)
	{
		// 비최적화 모드: 매 프레임 캐시 제거(사실상 캐시 미사용)
		ClearTileCache();
		m_lastStartTileX = -1;
		m_lastEndTileX = -1;
		m_lastStartTileY = -1;
		m_lastEndTileY = -1;
	}
	else
#endif
		// 그리드 영역이 변경된 경우에만 캐시 정리 수행
		if (sx != m_lastStartTileX || ex != m_lastEndTileX || sy != m_lastStartTileY || ey != m_lastEndTileY)
		{
			CleanupUnusedTileCache(mapData, sx, ex, sy, ey);
			m_lastStartTileX = sx; m_lastEndTileX = ex; m_lastStartTileY = sy; m_lastEndTileY = ey;
		}

	const float tileSizeF = static_cast<float>(TILE_SIZE);
	RenderManager* rm = RenderManager::GetInstance();
	for (int y = sy; y < ey; ++y)
	{
		for (int x = sx; x < ex; ++x)
		{
			auto& tileData = mapData->tiles[x][y];
			if (tileData.id == TILEID_NONE)
				continue;

			auto it = m_tileCache.find(tileData.id);
			if (it == m_tileCache.end()) {
				TileCacheData cd;
				LoadTileBitmap(tileData, cd);
				if (!cd.bitmap)
					continue;
				m_tileCache[tileData.id] = cd;
				it = m_tileCache.find(tileData.id);
			}
			Gdiplus::Bitmap* bm = it->second.bitmap;
			const float bitmapW = static_cast<float>(bm->GetWidth());
			const float bitmapH = static_cast<float>(bm->GetHeight());
			const float wx = static_cast<float>(x) * tileSizeF + tileSizeF * 0.5f;
			const float wy = static_cast<float>(y) * tileSizeF + tileSizeF * 0.5f;

			rm->AddWorldObjectCommand(bm, { 0.0f, 0.0f, bitmapW, bitmapH }, wx, wy, tileSizeF / bitmapW, tileSizeF / bitmapH, 0.5f, 0.5f, LAYER_WORLD_TILE, wy);
		}
	}

#ifdef _DEBUG
	const auto profileEnd = std::chrono::high_resolution_clock::now();
	const auto elapsedUs = std::chrono::duration_cast<std::chrono::microseconds>(profileEnd - profileStart).count();
	const float elapsedMs = static_cast<float>(elapsedUs) / 1000.0f;
	constexpr float kEmaAlpha = 0.10f;
	if (m_renderVisibleTilesSampleCount == 0) {
		m_avgRenderVisibleTilesMs = elapsedMs;
	}
	else {
		m_avgRenderVisibleTilesMs += kEmaAlpha * (elapsedMs - m_avgRenderVisibleTilesMs);
	}
	++m_renderVisibleTilesSampleCount;
#endif
}

void CameraManager::RenderVisibleGameObjects() {
#ifdef _DEBUG
	const auto cullStart = std::chrono::high_resolution_clock::now();
#endif

	ObjectManager* objectManager = ObjectManager::GetInstance();
	if (!objectManager) {
		return;
	}

#ifdef _DEBUG
	if (!g_bEnableOptimizationMode)
	{
		// 비최적화 비교 모드: 카메라 컬링 없이 월드 객체를 모두 렌더한다.
		const auto& worldObjects = objectManager->GetWorldObjects();
		int renderedWorldCount = 0;

		const float cullElapsedMs = 0.0f;

		const auto renderStart = std::chrono::high_resolution_clock::now();
		for (GameObject* obj : worldObjects) {
			if (!obj || !obj->IsEnabled() || obj->IsDead()) {
				continue;
			}
			obj->Render();
			obj->RenderDebugOverlay();
			++renderedWorldCount;

			if (obj->IsEntity()) {
				RenderManager::GetInstance()->AddRenderedEntity();
			}
		}

		const auto renderEnd = std::chrono::high_resolution_clock::now();
		const float renderElapsedMs = static_cast<float>(std::chrono::duration_cast<std::chrono::microseconds>(renderEnd - renderStart).count()) / 1000.0f;
		constexpr float kEmaAlpha = 0.10f;

		if (m_cullVisibleGameObjectsSampleCount == 0) {
			m_avgCullVisibleGameObjectsMs = cullElapsedMs;
		}
		else {
			m_avgCullVisibleGameObjectsMs += kEmaAlpha * (cullElapsedMs - m_avgCullVisibleGameObjectsMs);
		}
		++m_cullVisibleGameObjectsSampleCount;

		if (m_renderVisibleGameObjectsSampleCount == 0) {
			m_avgRenderVisibleGameObjectsMs = renderElapsedMs;
		}
		else {
			m_avgRenderVisibleGameObjectsMs += kEmaAlpha * (renderElapsedMs - m_avgRenderVisibleGameObjectsMs);
		}
		++m_renderVisibleGameObjectsSampleCount;
	}
	else
#endif
	{
		const vector<GameObject*>& worldObjects = objectManager->GetWorldObjects();
		std::vector<GameObject*> localVisibleBuffer;
#ifdef _DEBUG
		std::vector<GameObject*>& visibleBuffer = g_bEnableBufferReuse ? m_queryBuffer : localVisibleBuffer;
#else
		std::vector<GameObject*>& visibleBuffer = m_queryBuffer;
#endif

		visibleBuffer.clear();

		// [최적화] 전체 순회 대신 그리드 쿼리 사용
		objectManager->QueryObjectsInRectArea(GetViewportWorldRect(), visibleBuffer);

#ifdef _DEBUG
		const auto cullEnd = std::chrono::high_resolution_clock::now();
		const float cullElapsedMs = static_cast<float>(std::chrono::duration_cast<std::chrono::microseconds>(cullEnd - cullStart).count()) / 1000.0f;
#endif

#ifdef _DEBUG
		const auto renderStart = std::chrono::high_resolution_clock::now();
#endif
		for (GameObject* obj : visibleBuffer) {
			obj->Render();
			obj->RenderDebugOverlay();
#ifdef _DEBUG
			if (obj->IsEntity()) {
				RenderManager::GetInstance()->AddRenderedEntity();
			}
#endif
		}

#ifdef _DEBUG
		const auto renderEnd = std::chrono::high_resolution_clock::now();
		const float renderElapsedMs = static_cast<float>(std::chrono::duration_cast<std::chrono::microseconds>(renderEnd - renderStart).count()) / 1000.0f;
		constexpr float kEmaAlpha = 0.10f;

		if (m_cullVisibleGameObjectsSampleCount == 0) {
			m_avgCullVisibleGameObjectsMs = cullElapsedMs;
		}
		else {
			m_avgCullVisibleGameObjectsMs += kEmaAlpha * (cullElapsedMs - m_avgCullVisibleGameObjectsMs);
		}
		++m_cullVisibleGameObjectsSampleCount;

		if (m_renderVisibleGameObjectsSampleCount == 0) {
			m_avgRenderVisibleGameObjectsMs = renderElapsedMs;
		}
		else {
			m_avgRenderVisibleGameObjectsMs += kEmaAlpha * (renderElapsedMs - m_avgRenderVisibleGameObjectsMs);
		}
		++m_renderVisibleGameObjectsSampleCount;
#endif
	}
}

void CameraManager::CleanupUnusedTileCache(const MapData* md, int sx, int ex, int sy, int ey) {
	std::unordered_set<UINT> visible;

	// 현재 뷰포트 내에 존재하는 타일 ID 수집
	for (int y = sy; y < ey; ++y) for (int x = sx; x < ex; ++x)
		if (x >= 0 && x < md->mapWidth && y >= 0 &&
			y < md->mapHeight && md->tiles[x][y].id != TILEID_NONE)
			visible.insert(md->tiles[x][y].id);

	// 캐시에 존재하지만 현재 뷰포트에 없는 타일은 메모리에서 제거
	for (auto it = m_tileCache.begin(); it != m_tileCache.end();) {
		if (visible.find(it->first) == visible.end())
		{ 
			delete it->second.bitmap; 
			it = m_tileCache.erase(it);
		}
		else
			++it;
	}
}

void CameraManager::ClearTileCache() {
	if (!m_tileCache.empty())
	{
		for (auto& p : m_tileCache) {
			if (p.second.bitmap) {
				delete p.second.bitmap;
			}
		}
		m_tileCache.clear();
	}
}

void CameraManager::SetWalkableBoundsFromMapData(const MapData* md) {
	if (!md) return;
	const float tileSizeF = static_cast<float>(TILE_SIZE);
	float minX = 1e9f, minY = 1e9f, maxX = -1e9f, maxY = -1e9f; bool found = false;
	for (int y = 0; y < md->mapHeight; ++y) for (int x = 0; x < md->mapWidth; ++x) {
		if (!md->walkableAreas[x][y]) continue;
		float wx = static_cast<float>(x) * tileSizeF + tileSizeF * 0.5f;
		float wy = static_cast<float>(y) * tileSizeF + tileSizeF * 0.5f;
		minX = (std::min)(minX, wx); minY = (std::min)(minY, wy);
		maxX = (std::max)(maxX, wx); maxY = (std::max)(maxY, wy);
		found = true;
	}
	if (found) SetWalkableBounds(minX, minY, maxX, maxY);
}

void CameraManager::SetWalkableBounds(float minX, float minY, float maxX, float maxY) {
	m_hasWalkableBounds = true; m_walkableMinX = minX; m_walkableMinY = minY; m_walkableMaxX = maxX; m_walkableMaxY = maxY;
}

Gdiplus::PointF CameraManager::WorldToScreen(float worldX, float worldY) const {
	Gdiplus::RectF vp = GetViewportWorldRect();
	return { worldX - vp.X, worldY - vp.Y };
}

Gdiplus::PointF CameraManager::ScreenToWorld(float screenX, float screenY) const {
	Gdiplus::RectF vp = GetViewportWorldRect();
	return { screenX + vp.X, screenY + vp.Y };
}


void CameraManager::LoadTileBitmap(const ResourcePathUtils::TileResourceDef& td, TileCacheData& cd)
{
	std::wstring path = ResourcePathUtils::BuildResourcePath(td.baseDir, td.imageName);
	if (path.empty()) return;

	cd.bitmap = new Gdiplus::Bitmap(path.c_str());
	if (cd.bitmap->GetLastStatus() != Gdiplus::Ok)
	{
		delete cd.bitmap;
		cd.bitmap = nullptr;
	}
}


// 뷰포트 내에 모든 오브젝트 탐색(그리드 기반 탐색 X)
bool CameraManager::IsObjectInViewport(GameObject* obj) const {

	if (!obj || !obj->IsEnabled()) return false;

	Gdiplus::RectF viewportRect = GetViewportWorldRect();
	const float M = -16.f;

	Gdiplus::RectF bounds = obj->GetBounds();
	const bool overlapsObjectBounds =
		bounds.X < viewportRect.X + viewportRect.Width + M && bounds.X + bounds.Width > viewportRect.X - M &&
		bounds.Y < viewportRect.Y + viewportRect.Height + M && bounds.Y + bounds.Height > viewportRect.Y - M;
	if (!overlapsObjectBounds) {
		return false;
	}

	// 2차 정밀 컷: 스프라이트가 있는 경우 실제 렌더 bounds 기준으로 한 번 더 판정.
	Transform* transform = obj->GetComponent<Transform>();
	if (!transform) {
		return true;
	}

	std::shared_ptr<Sprite> sprite;
	if (SpriteRenderer* spriteRenderer = obj->GetComponent<SpriteRenderer>()) {
		sprite = spriteRenderer->GetSpriteHandle();
	}
	if (!sprite) {
		if (Animator* animator = obj->GetComponent<Animator>()) {
			sprite = animator->GetCurrentFrame().sprite;
		}
	}

	if (!sprite || !sprite->bitmap) {
		return true;
	}

	const float width = fabsf(sprite->sourceRect.Width * transform->GetScaleX());
	const float height = fabsf(sprite->sourceRect.Height * transform->GetScaleY());
	if (width <= 0.0f || height <= 0.0f) {
		return false;
	}

	const float worldX = transform->GetX();
	const float worldY = transform->GetY();
	Gdiplus::RectF renderBounds(
		worldX - width * sprite->pivot.X,
		worldY - height * sprite->pivot.Y,
		width,
		height);

	return renderBounds.X < viewportRect.X + viewportRect.Width + M && renderBounds.X + renderBounds.Width > viewportRect.X - M &&
		renderBounds.Y < viewportRect.Y + viewportRect.Height + M && renderBounds.Y + renderBounds.Height > viewportRect.Y - M;
}
