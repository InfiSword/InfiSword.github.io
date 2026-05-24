---
layout: single
title: "PROJECT REPORT // WINAPI_DONT_STARVE"
excerpt: "C++, WinAPI를 이용한 Don't Starve 모사 및 성능 최적화"
categories: [project]
permalink: /project/winapi-dont-starve/
tags: [C++, WinAPI, Optimization, Game Logic]
toc: true
toc_sticky: true
---

<style>
    /* --- Premium Report Layout Styles --- */
    
    .chapter-title {
        font-size: 2.2rem;
        color: #fff !important;
        background: linear-gradient(90deg, #007bff 0%, #58A6FF 100%);
        padding: 25px 35px;
        border-radius: 12px;
        margin-top: 60px !important;
        margin-bottom: 40px !important;
        box-shadow: 0 10px 30px rgba(0, 123, 255, 0.15);
        display: flex;
        align-items: center;
        border: none !important;
    }
    .chapter-title::before {
        content: "CHAPTER.";
        font-family: 'Fira Code', monospace;
        font-size: 0.9rem;
        letter-spacing: 2px;
        margin-right: 15px;
        opacity: 0.8;
    }

    .pf-visual-frame {
        width: 100%; padding: 35px; background: #fcfcfc;
        border: 1px solid #e1e4e8; border-radius: 16px;
        margin: 30px 0; text-align: center;
        box-shadow: inset 0 2px 10px rgba(0,0,0,0.02);
    }
    .pf-arch-diagram {
        display: flex; flex-direction: column; gap: 20px; margin: 25px 0;
    }
    .pf-arch-layer {
        padding: 20px 25px; border: 1px solid #e1e4e8; border-radius: 8px;
        background: #fff; position: relative; text-align: center;
        box-shadow: 0 4px 12px rgba(0,0,0,0.03);
    }
    .pf-arch-layer::before {
        content: ""; position: absolute; left: 0; top: 0; bottom: 0; width: 5px;
        background: #007bff; border-radius: 8px 0 0 8px;
    }
    .pf-arch-layer-title {
        color: #007bff; font-weight: 700; margin-bottom: 10px;
        font-family: 'Fira Code', monospace; font-size: 0.95rem; text-align: center;
    }
    .pf-arch-layer-items {
        display: flex; flex-wrap: wrap; gap: 10px; margin-top: 10px; justify-content: center;
    }
    .pf-arch-item {
        padding: 6px 14px; background: rgba(0, 123, 255, 0.08);
        border: 1px solid rgba(0, 123, 255, 0.3); border-radius: 6px;
        font-size: 0.85rem; color: #0056b3; text-align: center;
    }

    /* --- Code Details --- */
    details.pf-details {
        margin: 20px 0;
        border: 1px solid #e1e4e8;
        border-radius: 8px;
        background: #f8f9fa;
        overflow: hidden;
    }
    details.pf-details summary {
        padding: 15px 20px;
        font-weight: 700;
        color: #007bff;
        cursor: pointer;
        outline: none;
        background: #fff;
        display: flex;
        align-items: center;
    }

    .highlight-box {
        background: #f0f7ff;
        border-left: 5px solid #007bff;
        padding: 20px;
        margin: 20px 0;
        border-radius: 0 8px 8px 0;
    }

    /* --- Transaction Flow --- */
    .pf-transaction-flow {
        display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-family: 'Fira Code', monospace;
        gap: 12px; flex-wrap: wrap; margin: 20px 0;
    }
    .pf-flow-step { padding: 10px 18px; border: 1px solid #e1e4e8; border-radius: 6px; background: #fff; color: #333; text-align: center; box-shadow: 0 2px 5px rgba(0,0,0,0.05); line-height: 1.4; }
    .pf-flow-arrow { color: #007bff; font-weight: bold; font-size: 1.2rem; }

    /* --- Flow Arrows --- */
    .flow-arrow {
        color: #007bff; font-weight: bold; font-size: 1.5rem; margin: 10px 0;
    }
</style>

게임 엔진에 의존하지 않고 C++와 WinAPI만을 이용해 완전 밑바닥부터 게임을 구축하는 데 도전한 프로젝트입니다. 게임 엔진의 추상화된 기능 뒤에 숨겨진 전체적인 실행 파이프라인을 깊이 있게 이해하고 스스로 설계해보고자 시작하였으며, 기존 게임인 'Don't Starve'의 핵심 시스템을 참고하여 제작했습니다.

---

## 1. 주요 시스템 아키텍처
{: .chapter-title }

### 1.1 컴포넌트 기반 객체 설계 (Component-Based)

단일 상속의 복잡성을 피하고 객체 지향적인 유연성을 극대화하기 위해 **컴포넌트 기반 구조**를 채택했습니다. 모든 게임 객체(`GameObject`)는 기능 단위의 컴포넌트를 소유하며, 실행 시점에 필요한 기능을 동적으로 탈부착할 수 있습니다.

<div class="pf-visual-frame">
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">GameObject (Entity Container)</div>
            <div class="pf-arch-layer-items">
                <span class="pf-arch-item">ID / Type Management</span>
                <span class="pf-arch-item">std::vector&lt;Component*&gt;</span>
            </div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓ Has Components</div>
        <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 15px;">
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">Transform</div>
                <div style="font-size: 0.8rem; color: #666;">Position, Scale, Direction</div>
            </div>
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">SpriteRenderer</div>
                <div style="font-size: 0.8rem; color: #666;">GDI+ Sprite, Layer Info</div>
            </div>
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">Collider</div>
                <div style="font-size: 0.8rem; color: #666;">AABB Interaction Logic</div>
            </div>
            <div class="pf-arch-layer">
                <div class="pf-arch-layer-title">Animator</div>
                <div style="font-size: 0.8rem; color: #666;">Clip Control, Frame Events</div>
            </div>
        </div>
    </div>
</div>

이러한 구조를 통해 새로운 객체를 생성할 때 상속 계층에 얽매이지 않고 유연하게 기능을 조립할 수 있습니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: GameObject.h - 컴포넌트 추가 및 획득 핵심 로직</summary>

```cpp
// GameObject.h - 컴포넌트 추가 및 획득 핵심 로직
template <typename T, typename... Args>
T* AddComponent(Args&&... args) {
    auto newComponent = std::make_unique<T>(this, std::forward<Args>(args)...);
    T* componentPtr = newComponent.get();
    m_components.push_back(std::move(newComponent));
    return componentPtr;
}

template <typename T>
T* GetComponent() const {
    for (const auto& component : m_components) {
        if (!component) continue;
        T* target = dynamic_cast<T*>(component.get());
        if (target) return target;
    }
    return nullptr;
}
```
</details>

### 1.2 중앙 집중식 매니저 패턴 (Singleton Managers)

시스템의 전역적인 상태 관리와 자원 공유를 위해 **싱글톤 기반 매니저**들을 구축했습니다.

<div class="pf-visual-frame">
<div class="pf-arch-diagram">
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">ObjectManager</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">객체 생명주기 관리</span>
<span class="pf-arch-item">공간 분할 그리드 업데이트</span>
</div>
</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">RenderManager</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">커맨드 예약</span>
<span class="pf-arch-item">레이어별 Y-Sorting 렌더링</span>
</div>
</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">ResourceManager</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">비트맵/스프라이트 캐싱</span>
<span class="pf-arch-item">중복 로드 방지</span>
</div>
</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">CameraManager</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">가시 영역 컬링</span>
<span class="pf-arch-item">좌표계 변환 (World ↔ Screen)</span>
</div>
</div>
</div>
</div>

---

## 2. 렌더링 시스템 및 파이프라인
{: .chapter-title }

GDI+ 환경에서 대규모 오브젝트를 효율적으로 출력하기 위해 **커맨드 패턴 기반의 렌더링 파이프라인**을 구축했습니다. 매 프레임 모든 객체를 즉시 그리는 대신, 렌더링 명령을 레이어별로 수집하고 정렬한 뒤 일괄 실행(Flush)하는 방식을 취합니다.

### 2.1 렌더링 파이프라인 흐름

렌더링 프로세스는 크게 **제출(Submission) -> 정렬(Sorting) -> 실행(Execution)**의 3단계로 구성됩니다.

<div class="pf-visual-frame">
    <div class="pf-transaction-flow">
        <div class="pf-flow-step"><strong>1. Submission</strong><br>GameObject::Render()<br>DrawCommand 생성</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>2. Sorting</strong><br>RenderManager::Flush()<br>Layer & Y-Sorting</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>3. Execution</strong><br>ExecuteDrawCommand()<br>GDI+ DrawImage 호출</div>
    </div>
</div>

Top-down 뷰의 입체감을 위해 객체의 발밑(Pivot) 위치의 Y 좌표를 기준으로 정렬하여 출력함으로써 오브젝트 간의 앞뒤 관계를 정확히 표현합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: RenderManager.cpp - Y-Sorting 및 일괄 렌더링 처리</summary>

```cpp
// RenderManager.cpp - Y-Sorting 및 일괄 렌더링 처리
void RenderManager::Flush(Gdiplus::Graphics* pGraphics)
{
    if (!pGraphics) return;

    for (int i = LAYER_TILE_BACKGROUND; i < LAYER_COUNT; ++i) {
        if (m_layerCommands[i].empty()) continue;

        // zOrder(Y좌표)를 기준으로 오름차순 정렬하여 뒤에 있는 객체가 먼저 그려지도록 함
        if (m_layerCommands[i].size() > 1) {
            std::stable_sort(m_layerCommands[i].begin(), m_layerCommands[i].end(), [](const DrawCommand& a, const DrawCommand& b) {
                if (a.zOrder != b.zOrder) return a.zOrder < b.zOrder;
                return a.layer < b.layer;
            });
        }

        for (const auto& cmd : m_layerCommands[i]) {
            ExecuteDrawCommand(pGraphics, cmd);
        }
        m_layerCommands[i].clear();
    }
}
```
</details>

### 2.2 동적 타일 캐싱 및 가시 영역 렌더링

광활한 맵 전체를 그리는 부하를 막기 위해, 카메라의 뷰포트에 해당하는 타일만 선택적으로 렌더링합니다. 특히 타일 비트맵을 매번 로드하지 않고 **동적 캐싱 시스템**을 통해 메모리 효율을 극대화했습니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: CameraManager.cpp - 타일 맵 최적화 렌더링</summary>

```cpp
// CameraManager.cpp - 그리드 기반 타일 컬링 및 캐싱
void CameraManager::RenderVisibleTiles(const MapData* mapData) {
    if (!mapData) return;
    Gdiplus::RectF vp = GetViewportWorldRect();

    // 256px 그리드 단위로 가시 범위 인덱스 계산
    const int gridCellSize = 256;
    int sx = std::max(0, (int)floor(vp.X / gridCellSize)) * (gridCellSize / TILE_SIZE);
    int ex = std::min(MAP_WIDTH, (int)ceil((vp.X + vp.Width) / gridCellSize) * (gridCellSize / TILE_SIZE));
    // ... (Y 인덱스 생략)

    // 가시 영역 변경 시에만 미사용 캐시 정리
    if (sx != m_lastStartTileX || ex != m_lastEndTileX || ...) {
        CleanupUnusedTileCache(mapData, sx, ex, sy, ey);
    }

    for (int y = sy; y < ey; ++y) {
        for (int x = sx; x < ex; ++x) {
            auto& tileData = mapData->tiles[x][y];
            // 캐시 확인 및 RenderManager에 명령 제출
            Gdiplus::Bitmap* bm = GetOrLoadTileBitmap(tileData.id);
            RenderManager::GetInstance()->AddWorldObjectCommand(bm, ...);
        }
    }
}
```
</details>

---

## 3. 최적화 및 핵심 기술
{: .chapter-title }

### 3.1 공간 분할 기법: 그리드 기반 컬링 (Grid-based Culling)

월드에 수천 개의 오브젝트가 존재할 때의 업데이트 및 충돌 처리 성능 저하를 방지하기 위해 **공간 분할(Spatial Partitioning)** 기법을 적용했습니다.

<div class="pf-visual-frame">
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">1. Viewport Index Calculation</div>
            <div style="font-size: 0.8rem; color: #666;">카메라 영역을 기반으로 활성 그리드 범위(StartX~EndY) 산출</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">2. Query Stamp & Filtering</div>
            <div style="font-size: 0.8rem; color: #666;">중복 방지용 스탬프 대조 및 활성화/사망 상태 필터링</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">3. Precise AABB Check</div>
            <div style="font-size: 0.8rem; color: #666;">최종 뷰포트 사각형과의 교차 검사 후 업데이트 리스트 추가</div>
        </div>
    </div>
</div>

매 프레임 카메라의 현재 Viewport 영역에 해당하는 그리드 셀 내의 객체들만 추출하여 업데이트 및 렌더링을 수행합니다. 객체가 여러 셀에 겹쳐있을 경우 중복 처리를 막기 위해 **쿼리 스탬프(Query Stamp)** 기법을 활용했습니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: ObjectManager.cpp - 공간 분할 기반 객체 쿼리</summary>

```cpp
// ObjectManager.cpp - 공간 분할 기반 객체 쿼리
void ObjectManager::QueryObjectsInRectArea(const Gdiplus::RectF& rectArea, std::vector<GameObject*>& targetOutObjects)
{
    // ... 그리드 인덱스 계산 생략 ...
    
    // 중복 방지용 스탬프 갱신
    if (++m_spatialQueryStamp == 0) m_spatialQueryStamp = 1;

    for (int y = startY; y <= endY; ++y) {
        for (int x = startX; x <= endX; ++x) {
            for (auto* obj : m_spatialGrid[x][y]) {
                // 이미 확인한 객체는 건너뜀 (Stamp-based Optimization)
                if (obj->GetLastSpatialQueryStamp() == m_spatialQueryStamp) continue;
                obj->SetLastSpatialQueryStamp(m_spatialQueryStamp);

                if (!obj->IsEnabled() || obj->IsDead()) continue;

                // 최종 AABB 정밀 검사
                const Gdiplus::RectF bounds = obj->GetBounds();
                if (rectArea.IntersectsWith(bounds)) {
                    targetOutObjects.push_back(obj);
                }
            }
        }
    }
}
```
</details>

### 3.2 애니메이션 시스템 및 프레임 이벤트

`Animator`와 `AnimationClip`을 통한 고도로 제어 가능한 애니메이션 시스템을 구축했습니다. 프레임 이벤트를 통해 애니메이션 시점과 코드 로직을 동기화합니다.

<div class="pf-visual-frame">
    <div class="pf-transaction-flow">
        <div class="pf-flow-step"><strong>Update()</strong><br>Time Accumulation</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>Frame Switch</strong><br>Next Sprite Index</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>Event Trigger</strong><br>Execute Callback</div>
        <div class="pf-flow-arrow">→</div>
        <div class="pf-flow-step"><strong>Logic Sync</strong><br>Attack / Sound / Effect</div>
    </div>
</div>

애니메이션의 특정 프레임(예: 공격이 닿는 순간)에 이벤트를 바인딩하여 무기 데미지를 주거나 사운드를 재생할 수 있습니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: Animator.cpp - 프레임 이벤트 처리 로직</summary>

```cpp
// Animator.cpp - 프레임 이벤트 처리 로직
if (currentFrameIndex != -1 && currentFrameIndex != m_lastTriggeredFrame)
{
    const std::map<int, std::wstring>& eventFrames = m_currentClip->GetEventFrames();
    const AnimationEventCallback& callback = m_currentClip->GetEventCallback();

    for (int fi = m_lastTriggeredFrame + 1; fi <= currentFrameIndex && callback; ++fi ) {
        auto eventIt = eventFrames.find(fi);
        if (eventIt != eventFrames.end()) {
            callback(fi, eventIt->second); // 공격 판정, 사운드 등 실행
        }
    }
    m_lastTriggeredFrame = currentFrameIndex;
}
```
</details>

---

## 4. 데이터 로드 및 게임 진행 정보 저장
{: .chapter-title }

### 4.1 데이터 주도 설계 (Data-Driven)

게임의 밸런스 조정과 콘텐츠 확장을 용이하게 하기 위해 외부 데이터를 적극 활용합니다. 맵 파일뿐만 아니라, 오브젝트의 피벗(Pivot) 및 콜라이더 사이즈도 외부 텍스트 파일(`object_resource_overrides.txt`)을 파싱하여 동적으로 덮어씌웁니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: DataManager.cpp - 오브젝트 리소스 및 콜라이더 데이터 오버라이드</summary>

```cpp
// DataManager.cpp - 오브젝트 리소스 및 콜라이더 데이터 오버라이드
void DataManager::Init()
{
    ResourcePathUtils::ParseObjectResourceOverridesFile(L"GameData/object_resource_overrides.txt",
        [this](GameObjectID id, const ResourcePathUtils::ObjectResourceDef& overrideDef) {
            auto it = m_objectResources.find(id);
            if (it != m_objectResources.end()) {
                it->second.pivotX = overrideDef.pivotX;
                it->second.pivotY = overrideDef.pivotY;
                it->second.hasCollider = overrideDef.hasCollider;
                it->second.colliderWidth = overrideDef.colliderWidth;
                it->second.colliderHeight = overrideDef.colliderHeight;
                // ...
            }
        });
}
```
</details>

### 4.2 스냅샷 기반 저장 시스템

플레이어의 성장 상태와 게임 진행 정보를 영속적으로 관리합니다. 체력, 인벤토리 내 아이템 정보를 `PlayerStateSnapshot` 구조체로 캡슐화하여 씬 전환 및 세이브 시점에 저장합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: GameProgressManager.h & Player.cpp - 플레이어 상태 저장 및 스냅샷</summary>

```cpp
// Player.cpp - 상태 저장 (씬 전환용)
PlayerStateSnapshot Player::SaveState() const
{
    PlayerStateSnapshot snapshot;
    snapshot.hp = m_hp;
    snapshot.equippedSlotIndex = m_equippedSlotIndex;

    if (m_inventory) {
        snapshot.inventoryItems = m_inventory->GetAllItemsSnapshot();
    }

    return snapshot;
}
```
</details>

{: .notice--success}
**Conclusion:** `WINAPI_DONT_STARVE`는 저수준의 WinAPI 환경에서도 고성능 아키텍처와 공간 최적화 기법을 통해 대규모 오브젝트 시스템을 안정적으로 구현해냈습니다.
