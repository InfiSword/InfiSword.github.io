#include "99_Default/pch.h"
#include "ObjectManager.h"
#include "../../99_Default/ClientOptimatzationOption.h"
#include "../ResourceManager/ResourceManager.h"
#include "../DataManager/DataManager.h"
#include "../CameraManager/CameraManager.h"
#include "../RenderManager/RenderManager.h"
#include "../../02_GameObject/GameObject.h"
#include "../../02_GameObject/Entity/Player/Player.h"
#include "../../02_GameObject/Item/Item.h"
#include "../../02_GameObject/Item/Tool/Tool.h"
#include "../../02_GameObject/Entity/Enviorment/Tree.h"
#include "../../02_GameObject/Entity/Enviorment/Rock.h"
#include "../../02_GameObject/Entity/Enviorment/Grass.h"
#include "../../02_GameObject/Entity/Enviorment/BerryBush.h"
#include "../../02_GameObject/Entity/Enviorment/Sapling.h"
#include "../../02_GameObject/Entity/Monster/Pig.h"
#include "../../02_GameObject/Entity/Monster/Spider.h"
#include "../../02_GameObject/Entity/Monster/Boss_SpiderQueen.h"
#include "../../02_GameObject/Entity/Monster/Hound.h"
#include "../../02_GameObject/Entity/Monster/Boss_RedHound.h"
#include "../../02_GameObject/Entity/Monster/Boss_IceHound.h"
#include "../../02_GameObject/Building/PigHouse.h"
#include "../../02_GameObject/Building/SpiderEgg.h"
#include "../../02_GameObject/UI/MenuUI.h"
#include "../../02_GameObject/UI/UIImage.h"
#include "../../02_GameObject/UI/UIButton.h"
#include "../../02_GameObject/UI/UIText.h"
#include "../../02_GameObject/UI/HPUI.h"
#include "../../02_GameObject/UI/GameOverUI.h"
#include "../../02_GameObject/UI/GameClearUI.h"
#include "../../02_GameObject/UI/IntroNoticeUI.h"
#include "../../02_GameObject/UI/UIElement.h"
#include "../../02_GameObject/Component/Transform/RectTransform.h"
#include "../../02_GameObject/Component/Collider/BoxCollider.h"
#include "../../02_GameObject/Component/Collider/CircleCollider.h"

ObjectManager::ObjectManager()
{
	m_cachedPlayer = nullptr;
}

ObjectManager::~ObjectManager()
{
	ClearAllObjects();
}

void ObjectManager::Init()
{
	ClearAllObjects();
}

void ObjectManager::LateInit()
{
	for (GameObject* obj : m_worldObjects) if (obj) obj->LateInit();
	for (GameObject* obj : m_uiObjects) if (obj) obj->LateInit();
	for (GameObject* obj : m_inactiveObjects) if (obj) obj->LateInit();
}

void ObjectManager::Update(float deltaTime)
{
	for (GameObject* obj : m_worldObjects) {
		if (obj && obj->IsEnabled())
		{
			obj->Update(deltaTime);
		}
	}
	for (GameObject* obj : m_uiObjects) {
		if (obj && obj->IsEnabled())
		{
			obj->Update(deltaTime);
		}
	}
	for (GameObject* obj : m_inactiveObjects) {
		if (obj) {
			obj->Update(deltaTime);
		}
	}

}

void ObjectManager::LateUpdate()
{
	for (GameObject* obj : m_worldObjects) if (obj && obj->IsEnabled()) obj->LateUpdate();
	for (GameObject* obj : m_uiObjects) if (obj && obj->IsEnabled()) obj->LateUpdate();

	ProcessPendingDeletions();
}

void ObjectManager::Render()
{
	// 월드 렌더의 최종 가시 컬링/제출은 CameraManager 단일 경로에서 처리한다.
	CameraManager* cameraManager = CameraManager::GetInstance();
	if (cameraManager) {
		cameraManager->RenderVisibleGameObjects();
	}

	// UI 렌더링
	for (GameObject* obj : m_uiObjects) {
		if (obj->IsEnabled()) {
			obj->Render();
		}
	}
}

void ObjectManager::Release()
{
	ClearAllObjects();
}

void ObjectManager::AddGameObject(GameObject* pObj)
{
	if (!pObj) return;

	if (std::find(m_worldObjects.begin(), m_worldObjects.end(), pObj) != m_worldObjects.end()) return;
	if (std::find(m_uiObjects.begin(), m_uiObjects.end(), pObj) != m_uiObjects.end()) return;
	if (std::find(m_inactiveObjects.begin(), m_inactiveObjects.end(), pObj) != m_inactiveObjects.end()) return;

	auto& targetList = (pObj->GetType() == GO_TYPE_UI) ? m_uiObjects : m_worldObjects;
	targetList.push_back(pObj);

	// 월드 객체인 경우 그리드에 추가
	if (pObj->GetType() != GO_TYPE_UI) {
		AddToGrid(pObj);
	}

	// 플레이어 캐싱
	if (pObj->GetType() == GO_TYPE_PLAYER) {
		m_cachedPlayer = static_cast<Player*>(pObj);
	}
}

void ObjectManager::RemoveGameObject(GameObject* pObj)
{
	if (!pObj) return;

	// 현재 매니저가 소유 중인 객체가 아니면 무시
	if (!IsManagedObject(pObj)) return;

	if (std::find(m_pendingDeletions.begin(), m_pendingDeletions.end(), pObj) != m_pendingDeletions.end()) return;
	if (pObj->IsDead()) return;

	pObj->SetDead(true);
	pObj->SetActive(false);

	// 그리드에서 즉시 제거
	if (pObj->GetType() != GO_TYPE_UI) {
		RemoveFromGrid(pObj);
	}

	m_pendingDeletions.push_back(pObj);
}

bool ObjectManager::IsScreenPointBlockedByUI(float screenX, float screenY) const
{
	// 활성화된 UIElement의 RectTransform 바운딩 박스 검사
	for (const GameObject* obj : m_uiObjects) {
		if (!obj->IsEnabled()) continue;

		const UIElement* element = static_cast<const UIElement*>(obj);
		RectTransform* rt = element->GetRectTransform();
		if (!rt) continue;
		Gdiplus::RectF bounds = rt->GetScreenBoundingBox();
		if (bounds.Width > 0.0f && bounds.Height > 0.0f && bounds.Contains(screenX, screenY))
			return true;
	}
	return false;
}

void ObjectManager::QueryObjectsInRectArea(const Gdiplus::RectF& rectArea, std::vector<GameObject*>& targetOutObjects)
{
	if (m_worldObjects.empty()) return;

#ifdef _DEBUG
	if (!g_bEnableSpatialPartitioning) {
		// [비최적화] 정밀 브루트포스 가시성 체크
		CameraManager* cameraManager = CameraManager::GetInstance();
		for (GameObject* obj : m_worldObjects) {
			if (!obj || !obj->IsEnabled() || obj->IsDead()) continue;
			
			if (cameraManager->IsObjectInViewport(obj)) {
				targetOutObjects.push_back(obj);
			}
		}
		return;
	}
#endif

	// 그리드 기반 쿼리 수행
	int startX = (int)floor(rectArea.X / GRID_CELL_SIZE);
	int startY = (int)floor(rectArea.Y / GRID_CELL_SIZE);
	int endX = (int)ceil((rectArea.X + rectArea.Width) / GRID_CELL_SIZE) - 1;
	int endY = (int)ceil((rectArea.Y + rectArea.Height) / GRID_CELL_SIZE) - 1;

	// 인덱스 범위 클램핑
	startX = (std::max<int>)(0, (std::min<int>)(GRID_WIDTH - 1, startX));
	startY = (std::max<int>)(0, (std::min<int>)(GRID_HEIGHT - 1, startY));
	endX = (std::max<int>)(0, (std::min<int>)(GRID_WIDTH - 1, endX));
	endY = (std::max<int>)(0, (std::min<int>)(GRID_HEIGHT - 1, endY));

	// 중복 방지용 스탬프 갱신
	if (++m_spatialQueryStamp == 0) m_spatialQueryStamp = 1;

	for (int y = startY; y <= endY; ++y) {
		for (int x = startX; x <= endX; ++x) {
			for (auto* obj : m_spatialGrid[x][y]) {
				// 이미 확인한 객체는 건너뜀
				if (obj->GetLastSpatialQueryStamp() == m_spatialQueryStamp) continue;
				obj->SetLastSpatialQueryStamp(m_spatialQueryStamp);

				if (!obj->IsEnabled() || obj->IsDead()) continue;

				// 최종 AABB 검사
				const Gdiplus::RectF bounds = obj->GetBounds();
				if (rectArea.X < bounds.X + bounds.Width && rectArea.X + rectArea.Width > bounds.X &&
					rectArea.Y < bounds.Y + bounds.Height && rectArea.Y + rectArea.Height > bounds.Y) {
					targetOutObjects.push_back(obj);
				}
			}
		}
	}
}

void ObjectManager::UpdateObjectGrid(GameObject* pObj)
{
	if (!pObj || pObj->GetType() == GO_TYPE_UI) return;

	// 기존 위치 저장
	int oldX = pObj->GetGridCellX();
	int oldY = pObj->GetGridCellY();

	// 새 위치 계산 (중심점 기준)
	Gdiplus::RectF bounds = pObj->GetBounds();
	int newX = (int)floor((bounds.X + bounds.Width * 0.5f) / GRID_CELL_SIZE);
	int newY = (int)floor((bounds.Y + bounds.Height * 0.5f) / GRID_CELL_SIZE);

	// 인덱스 범위 제한
	newX = (std::max<int>)(0, (std::min<int>)(GRID_WIDTH - 1, newX));
	newY = (std::max<int>)(0, (std::min<int>)(GRID_HEIGHT - 1, newY));

	// 셀이 변경된 경우만 갱신
	if (oldX == newX && oldY == newY) return;

	// 이전 셀에서 제거
	if (oldX >= 0 && oldX < GRID_WIDTH && oldY >= 0 && oldY < GRID_HEIGHT) {
		std::vector<GameObject*>& cell = m_spatialGrid[oldX][oldY];
		cell.erase(std::remove(cell.begin(), cell.end(), pObj), cell.end());
	}

	// 새 셀에 추가
	m_spatialGrid[newX][newY].push_back(pObj);
	pObj->SetGridCell(newX, newY);
}

void ObjectManager::AddToGrid(GameObject* pObj)
{
	if (!pObj || pObj->GetType() == GO_TYPE_UI) return;

	Gdiplus::RectF bounds = pObj->GetBounds();
	int x = (int)floor((bounds.X + bounds.Width * 0.5f) / GRID_CELL_SIZE);
	int y = (int)floor((bounds.Y + bounds.Height * 0.5f) / GRID_CELL_SIZE);

	x = (std::max<int>)(0, (std::min<int>)(GRID_WIDTH - 1, x));
	y = (std::max<int>)(0, (std::min<int>)(GRID_HEIGHT - 1, y));

	m_spatialGrid[x][y].push_back(pObj);
	pObj->SetGridCell(x, y);
}

void ObjectManager::RemoveFromGrid(GameObject* pObj)
{
	if (!pObj) return;
	int x = pObj->GetGridCellX();
	int y = pObj->GetGridCellY();

	if (x >= 0 && x < GRID_WIDTH && y >= 0 && y < GRID_HEIGHT) {
		std::vector<GameObject*>& cell = m_spatialGrid[x][y];
		cell.erase(std::remove(cell.begin(), cell.end(), pObj), cell.end());
		pObj->SetGridCell(-1, -1);
	}
}

Player* ObjectManager::GetPlayer() const
{
	return m_cachedPlayer;
}

GameObject* ObjectManager::FindGameObject(GameObjectID id)
{
	for (GameObject* obj : m_worldObjects) {
		if (obj && obj->GetID() == id) return obj;
	}
	for (GameObject* obj : m_uiObjects) {
		if (obj && obj->GetID() == id) return obj;
	}
	return nullptr;
}

void ObjectManager::ProcessPendingDeletions()
{
	if (m_pendingDeletions.empty()) return;

	// 월드 오브젝트 처리
	for (int i = (int)m_worldObjects.size() - 1; i >= 0; --i) {
		GameObject* obj = m_worldObjects[i];
		if (obj->IsDead()) {
			if (obj == m_cachedPlayer) m_cachedPlayer = nullptr;
			obj->Release();
			Utils::SafeDelete(obj);

			m_worldObjects[i] = m_worldObjects.back();
			m_worldObjects.pop_back();
		}
	}

	// UI 오브젝트 처리 
	for (int i = (int)m_uiObjects.size() - 1; i >= 0; --i) {
		GameObject* obj = m_uiObjects[i];
		if (obj->IsDead()) {
			obj->Release();
			Utils::SafeDelete(obj);

			m_uiObjects[i] = m_uiObjects.back();
			m_uiObjects.pop_back();
		}
	}

	m_pendingDeletions.clear();
}

void ObjectManager::ClearAllObjects()
{
	ProcessPendingDeletions();

	std::vector<GameObject*> allObjects;
	allObjects.reserve(m_worldObjects.size() + m_uiObjects.size() + m_inactiveObjects.size());
	allObjects.insert(allObjects.end(), m_worldObjects.begin(), m_worldObjects.end());
	allObjects.insert(allObjects.end(), m_uiObjects.begin(), m_uiObjects.end());
	allObjects.insert(allObjects.end(), m_inactiveObjects.begin(), m_inactiveObjects.end());

	m_cachedPlayer = nullptr;

	m_worldObjects.clear();
	m_worldObjects.shrink_to_fit();
	m_uiObjects.clear();
	m_uiObjects.shrink_to_fit();
	m_inactiveObjects.clear();
	m_inactiveObjects.shrink_to_fit();
	m_pendingDeletions.clear();
	m_pendingDeletions.shrink_to_fit();

	for (int y = 0; y < GRID_HEIGHT; ++y) {
		for (int x = 0; x < GRID_WIDTH; ++x) {
			m_spatialGrid[x][y].clear();
		}
	}
	m_spatialQueryStamp = 0;

	for (GameObject* obj : allObjects) {
		if (!obj) continue;
		obj->Release();
		Utils::SafeDelete(obj);
	}
}

bool ObjectManager::IsManagedObject(const GameObject* pObj) const
{
	if (!pObj) return false;
	if (std::find(m_worldObjects.begin(), m_worldObjects.end(), pObj) != m_worldObjects.end()) return true;
	if (std::find(m_uiObjects.begin(), m_uiObjects.end(), pObj) != m_uiObjects.end()) return true;
	if (std::find(m_inactiveObjects.begin(), m_inactiveObjects.end(), pObj) != m_inactiveObjects.end()) return true;
	return false;
}

GameObject* ObjectManager::CreateObject(GameObjectID id, float x, float y)
{
	switch (id)
	{
	case GOID_NORMAL_GRASS: return CreateObject<Grass>(id, x, y);
	case GOID_NORMAL_TREE_SHORT:
	case GOID_NORMAL_TREE_NORMAL:
	case GOID_NORMAL_TREE_TALL: return CreateObject<Tree>(id, x, y);
	case GOID_NORMAL_ROCK:
	case GOID_GOLD_ROCK: return CreateObject<Rock>(id, x, y);
	case GOID_NORMAL_SAPLING: return CreateObject<Sapling>(id, x, y);
	case GOID_BERRY_TREE: return CreateObject<BerryBush>(id, x, y);

	case GOID_MONSTER_PIG: return CreateObject<Pig>(id, x, y);
	case GOID_MONSTER_SPIDER:
	case GOID_MONSTER_WARRIOR_SPIDER: return CreateObject<Spider>(id, x, y);
	case GOID_MONSTER_QUEEN_SPIDER: return CreateObject<Boss_SpiderQueen>(id, x, y);
	case GOID_MONSTER_HOUNDDOG: return CreateObject<Hound>(id, x, y);
	case GOID_MONSTER_REDHOUNDDOG: return CreateObject<Boss_RedHound>(id, x, y);
	case GOID_MONSTER_ICEHOUNDDOG: return CreateObject<Boss_IceHound>(id, x, y);

	case GOID_BUILDING_PIGHOUSE: return CreateObject<PigHouse>(id, x, y);
	case GOID_BUILDING_SPIDER_SMALLEGG:
	case GOID_BUILDING_SPIDER_NORMALEGG:
	case GOID_BUILDING_SPIDER_TALLEGG:
	case GOID_BUILDING_SPIDER_SACEGG: return CreateObject<SpiderEgg>(id, x, y);

	case GOID_PLAYER_WILSON:
	case GOID_PLAYER_WILLOW:
	case GOID_PLAYER_WOLFGANG: return CreateObject<Player>(id, x, y);
	}

	if (id >= 301 && id <= 399) return CreateObject<Item>(id, x, y);
	if (id >= 401 && id <= 499) return CreateObject<Tool>(id, x, y);

	return CreateObject<GameObject>(id, x, y);
}

void ObjectManager::ApplyObjectData(GameObject* pObj, const ResourcePathUtils::ObjectResourceDef* data)
{
	if (!pObj || !data) return;

	if (data->hasCollider) {
		if (data->colliderType == COLLIDER_BOX) {
			BoxCollider* col = pObj->AddComponent<BoxCollider>();
			pObj->SetMainCollider(col);
			col->SetObjectCollider(data->colliderOffsetX, data->colliderOffsetY, data->colliderWidth, data->colliderHeight);
		}
		else if (data->colliderType == COLLIDER_CIRCLE) {
			CircleCollider* col = pObj->AddComponent<CircleCollider>();
			pObj->SetMainCollider(col);
			col->SetObjectCollider(data->colliderCenterX, data->colliderCenterY, data->colliderRadius);
		}
	}
}

UIButton* ObjectManager::CreateButton(GameObjectID id, float width, float height, const std::wstring& normalPath, const std::wstring& hoverPath, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY, float x, float y, std::function<void()> onClick)
{
	auto* resMgr = ResourceManager::GetInstance();
	auto normalSprite = resMgr->LoadSprite(normalPath);
	auto hoverSprite = resMgr->LoadSprite(hoverPath);

	UIButton* button = new UIButton(id, width, height, normalSprite, hoverSprite, anchorMinX, anchorMinY, anchorMaxX, anchorMaxY, x, y);
	if (button) {
		button->SetOnClickCallback(onClick);
		AddGameObject(button);
		button->Init();
	}
	return button;
}

UIImage* ObjectManager::CreateImage(GameObjectID id, float width, float height, RenderLayer layer, const std::wstring& path, float depth, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY, float x, float y)
{
	UIImage* image = new UIImage(id, width, height, layer, path, depth, anchorMinX, anchorMinY, anchorMaxX, anchorMaxY, x, y);
	if (image) {
		AddGameObject(image);
		image->Init();
	}
	return image;
}

UIText* ObjectManager::CreateText(GameObjectID id, float width, float height, const std::wstring& text, Gdiplus::Color color, float fontSize, Gdiplus::FontStyle fontStyle, float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY, float x, float y, float sortKey, Gdiplus::StringAlignment hAlign, Gdiplus::StringAlignment vAlign)
{
	RenderLayer layer = LAYER_UI_FOREGROUND;
	std::wstring fontName = L"Arial";

	UIText* uiText = new UIText(id, width, height, text, color, layer, sortKey, fontName, fontSize, fontStyle, hAlign, vAlign, anchorMinX, anchorMinY, anchorMaxX, anchorMaxY, x, y);
	if (uiText) {
		AddGameObject(uiText);
		uiText->Init();
	}
	return uiText;
}

MenuUI* ObjectManager::CreateMenuUI()
{
	MenuUI* ui = new MenuUI();
	if (ui) {
		AddGameObject(ui);
		ui->Init();
	}
	return ui;
}

HPUI* ObjectManager::CreateHPUI(Entity* pTarget, const std::wstring& name, float width, float height, Gdiplus::Color bgColor, Gdiplus::Color barColor, Gdiplus::Color nameColor, float anchorX, float anchorY, float pivotX, float pivotY, float x, float y, float bgSortKey, float barSortKey, bool usePortrait, bool useName)
{
	HPUI* ui = new HPUI(pTarget, name, width, height, bgColor, barColor, nameColor, anchorX, anchorY, pivotX, pivotY, x, y, bgSortKey, barSortKey, usePortrait, useName);
	if (ui) {
		AddGameObject(ui);
		ui->Init();
	}
	return ui;
}

GameOverUI* ObjectManager::CreateGameOverUI()
{
	GameOverUI* ui = new GameOverUI();
	if (ui) {
		AddGameObject(ui);
		ui->Init();
	}
	return ui;
}

GameClearUI* ObjectManager::CreateGameClearUI()
{
	GameClearUI* ui = new GameClearUI();
	if (ui) {
		AddGameObject(ui);
		ui->Init();
	}
	return ui;
}

IntroNoticeUI* ObjectManager::CreateIntroNoticeUI()
{
	IntroNoticeUI* ui = new IntroNoticeUI();
	if (ui) {
		AddGameObject(ui);
		ui->Init();
	}
	return ui;
}
