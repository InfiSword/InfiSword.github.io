---
title: "PROJECT REPORT // WINAPI_DONT_STARVE"
excerpt: "C++, WinAPI를 이용한 Don't Starve 모사 및 성능 최적화"
permalink: "/project/winapi-dont-starve/"
tags: [C++, WinAPI, Optimization, Game Logic]
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

<section id="gallery" class="pf-ftd-post-gallery-section">
    <div class="pf-ftd-section-header">
        <span class="pf-ftd-section-tag pf-mono">● IN-GAME SCREENSHOT GALLERY</span>
        <h3 class="pf-ftd-section-title">인게임 핵심 시스템 & 스크린샷 미리보기</h3>
        <p class="pf-ftd-section-desc">캐릭터 선택부터 오픈월드 생태계, 크래프팅 및 인벤토리, 하운드/거미 전투, 거대 보스 거미 여왕 레이드까지 주요 인게임 화면을 넘겨가며 옆으로 스크롤하여 확인할 수 있습니다.</p>
    </div>

    <!-- INTERACTIVE SCREENSHOT GALLERY (HORIZONTAL SCROLL & SLIDER) -->
    <div class="pf-ftd-gallery" id="pfDsGalleryPost">
        <!-- Topbar Meta & Actions -->
        <div class="pf-ftd-gallery-topbar">
            <div class="pf-ftd-gallery-meta">
                <span class="pf-ftd-counter pf-mono">01 / 08</span>
                <span class="pf-ftd-caption">윌슨(Wilson) 캐릭터 선택 화면</span>
            </div>
            <a href="https://www.youtube.com/watch?v=TTdIN2U3gnQ" target="_blank" rel="noopener noreferrer" class="pf-ftd-yt-link" title="YouTube 시연 영상 새 창으로 열기">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z"/></svg>
                <span>YouTube 시연 영상 ↗</span>
            </a>
        </div>

        <!-- Main Viewing Stage -->
        <div class="pf-ftd-stage">
            <button type="button" class="pf-ftd-nav pf-ftd-prev" onclick="pfFtdGalleryGo(-1, this)" aria-label="이전 스크린샷">‹</button>
            <img class="pf-ftd-main-img" src="/assets/images/Dont%20Starve/InGame/ds_ingame_01.png" alt="윌슨(Wilson) 캐릭터 선택 화면" loading="eager">
            <button type="button" class="pf-ftd-nav pf-ftd-next" onclick="pfFtdGalleryGo(1, this)" aria-label="다음 스크린샷">›</button>
        </div>

        <!-- Horizontal Scrollable Thumbnail Strip -->
        <div class="pf-ftd-thumbs-wrapper">
            <div class="pf-ftd-thumbs-strip" role="region" aria-label="스크린샷 썸네일 가로 스크롤 목록">
                <button type="button" class="pf-ftd-thumb-card is-active" data-index="0" data-title="윌슨(Wilson) 캐릭터 선택 화면" onclick="pfFtdGallerySelect(0, this)" title="1. 윌슨(Wilson) 캐릭터 선택 화면">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_01.png" alt="1. 윌슨(Wilson) 캐릭터 선택 화면" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">01</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="1" data-title="오픈월드 필드 생태계 및 오브젝트 생성" onclick="pfFtdGallerySelect(1, this)" title="2. 오픈월드 필드 생태계 및 오브젝트 생성">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_02.png" alt="2. 오픈월드 필드 생태계 및 오브젝트 생성" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">02</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="2" data-title="크래프팅(제작) 메뉴 및 인벤토리 슬롯 시스템" onclick="pfFtdGallerySelect(2, this)" title="3. 크래프팅(제작) 메뉴 및 인벤토리 슬롯 시스템">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_03.png" alt="3. 크래프팅(제작) 메뉴 및 인벤토리 슬롯 시스템" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">03</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="3" data-title="하운드(Hound) 몬스터 조우 및 실시간 전투" onclick="pfFtdGallerySelect(3, this)" title="4. 하운드(Hound) 몬스터 조우 및 실시간 전투">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_04.png" alt="4. 하운드(Hound) 몬스터 조우 및 실시간 전투" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">04</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="4" data-title="얼음 하운드 & 레드 하운드 특수 기믹 전투" onclick="pfFtdGallerySelect(4, this)" title="5. 얼음 하운드 & 레드 하운드 특수 기믹 전투">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_05.png" alt="5. 얼음 하운드 & 레드 하운드 특수 기믹 전투" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">05</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="5" data-title="거미집 군락지 및 거미 사냥과 거미줄 파밍" onclick="pfFtdGallerySelect(5, this)" title="6. 거미집 군락지 및 거미 사냥과 거미줄 파밍">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_06.png" alt="6. 거미집 군락지 및 거미 사냥과 거미줄 파밍" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">06</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="6" data-title="거대 보스 거미 여왕(Spider Queen) 레이드 전투" onclick="pfFtdGallerySelect(6, this)" title="7. 거대 보스 거미 여왕(Spider Queen) 레이드 전투">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_07.png" alt="7. 거대 보스 거미 여왕(Spider Queen) 레이드 전투" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">07</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="7" data-title="보스전 게임 오버(Game Over) 및 시스템 UI" onclick="pfFtdGallerySelect(7, this)" title="8. 보스전 게임 오버(Game Over) 및 시스템 UI">
                    <img src="/assets/images/Dont%20Starve/InGame/ds_ingame_08.png" alt="8. 보스전 게임 오버(Game Over) 및 시스템 UI" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">08</span>
                </button>
            </div>
        </div>
        <div class="pf-ftd-hint">
            <span>Tip: 좌우 화살표 버튼, 키보드 방향키(←, →), 또는 마우스 휠로 썸네일을 옆으로 스크롤하여 탐색할 수 있습니다.</span>
        </div>
    </div>
</section>

---

## 1. 주요 시스템 아키텍처
{: .chapter-title }

### 1.1 컴포넌트 기반 객체 설계 (Component-Based)

단일 상속의 복잡성을 피하고 객체 지향적인 유연성을 극대화하기 위해 **컴포넌트 기반 구조**를 채택했습니다. 모든 게임 객체(`GameObject`)는 기능 단위의 컴포넌트를 소유하며, 실행 시점에 필요한 기능을 동적으로 탈부착할 수 있습니다.

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- Container Header -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #3b82f6; background: #eff6ff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1d4ed8;">GameObject (엔티티 컨테이너)</h4>
        <span class="pf-fc-badge" style="background: #dbeafe; color: #1e40af; border-color: #93c5fd;">ENTITY CONTAINER</span>
      </div>
      <p class="pf-fc-desc">고유 식별자(ID) 및 타입 관리, `std::vector&lt;std::unique_ptr&lt;Component&gt;&gt;` 소유 및 컴포넌트 생명주기 관리</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- 4 Components Grid -->
    <div class="pf-fc-branch-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); width: 100%;">
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">CORE</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">Transform</div>
        <div class="pf-fc-branch-desc">2D 위치(Position), 스케일, 바라보는 방향 연산</div>
      </div>
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">RENDER</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">SpriteRenderer</div>
        <div class="pf-fc-branch-desc">GDI+ 비트맵 렌더링 및 Y-Sorting 피벗 좌표 관리</div>
      </div>
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">PHYSICS</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">Collider</div>
        <div class="pf-fc-branch-desc">AABB 충돌 박스 및 상호작용 트리거 검사</div>
      </div>
      <div class="pf-fc-branch-item pf-fc-item-compact">
        <span class="pf-fc-badge">ANIM</span>
        <div class="pf-fc-branch-title" style="margin-top: 5px;">Animator</div>
        <div class="pf-fc-branch-desc">스프라이트시트 클립 재생 및 프레임 이벤트 콜백</div>
      </div>
    </div>

  </div>
</div>

이러한 구조를 통해 새로운 객체를 생성할 때 상속 계층에 얽매이지 않고 유연하게 기능을 조립할 수 있습니다.

<details class="pf-details">
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

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <div class="pf-fc-pill-start">
      <span class="pf-fc-mono-tag">TRIGGER</span>
      <span>Render Loop Trigger (매 프레임 렌더링 시작)</span>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">1. Submission (명령 제출)</h4>
        <span class="pf-fc-badge">COLLECT</span>
      </div>
      <p class="pf-fc-desc">`GameObject::Render()` 호출 &rarr; `DrawCommand` 생성 후 `RenderManager` 큐에 예약</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">2. Sorting (정렬 단계)</h4>
        <span class="pf-fc-badge" style="background: #eff6ff; color: #2563eb; border-color: #bfdbfe;">Y-SORTING</span>
      </div>
      <p class="pf-fc-desc">`RenderManager::Flush()` &rarr; 배경/오브젝트/UI 레이어 우선순위 분류 후 발밑(Pivot Y) 기준 오름차순 정렬</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact" style="border-color: #bbf7d0; background: #fdfffe;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #16a34a;">3. Execution (일괄 출력)</h4>
        <span class="pf-fc-badge" style="background: #ecfdf5; color: #059669; border-color: #a7f3d0;">GDI+ FLUSH</span>
      </div>
      <p class="pf-fc-desc">정렬된 명령 버퍼를 순회하며 백버퍼 DC에 `Graphics::DrawImage()` 최종 출력</p>
    </div>

  </div>
</div>

Top-down 뷰의 입체감을 위해 객체의 발밑(Pivot) 위치의 Y 좌표를 기준으로 정렬하여 출력함으로써 오브젝트 간의 앞뒤 관계를 정확히 표현합니다.

<details class="pf-details">
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

<details class="pf-details">
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

월드에 수천 개의 오브젝트가 존재할 때, 모든 객체에 대해 매 프레임 업데이트와 충돌 검사(Brute-force, `O(N)`)를 수행하면 심각한 성능 저하가 발생합니다. 이를 방지하기 위해 맵 전체를 256x256 픽셀 크기의 **정적 2D 그리드 배열(Spatial Partitioning)**로 분할했습니다.

<div class="pf-visual-frame pf-comp-frame">
  <div class="pf-comp-container">
    <!-- 기존 방식 (AS-IS) -->
    <div class="pf-comp-box old">
      <div class="pf-comp-header">
        <span class="pf-comp-badge old">기존 방식 (AS-IS)</span>
        <h4 class="pf-comp-title">전수 순회 검사 (Brute-Force)</h4>
      </div>
      <ul class="pf-comp-list">
        <li>
          <span class="pf-comp-icon old">✕</span>
          <span class="pf-comp-text">수천 개 전체 오브젝트 매 프레임 순회 O(N)</span>
        </li>
        <li>
          <span class="pf-comp-icon old">✕</span>
          <span class="pf-comp-text">화면 밖 비가시 객체 불필요 연산 부하</span>
        </li>
        <li>
          <span class="pf-comp-icon old">✕</span>
          <span class="pf-comp-text">오브젝트 밀집 시 극심한 CPU 프레임 드랍</span>
        </li>
      </ul>
    </div>

    <!-- 전환 브릿지 (OPTIMIZATION) -->
    <div class="pf-comp-bridge">
      <span class="pf-bridge-pill">OPTIMIZATION</span>
      <div class="pf-bridge-arrow">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="5" y1="12" x2="19" y2="12"></line>
          <polyline points="12 5 19 12 12 19"></polyline>
        </svg>
      </div>
    </div>

    <!-- 개선 방식 (TO-BE) -->
    <div class="pf-comp-box new">
      <div class="pf-comp-header">
        <span class="pf-comp-badge new">개선 방식 (TO-BE)</span>
        <h4 class="pf-comp-title">2D 그리드 공간 분할 (Spatial Grid)</h4>
      </div>
      <ul class="pf-comp-list">
        <li>
          <span class="pf-comp-icon new">✓</span>
          <span class="pf-comp-text">뷰포트 기반 O(1) 인덱싱 &amp; 가시 셀 선별</span>
        </li>
        <li>
          <span class="pf-comp-icon new">✓</span>
          <span class="pf-comp-text">쿼리 스탬프(Query Stamp) 중복 연산 0% (완전 배제)</span>
        </li>
        <li>
          <span class="pf-comp-icon new">✓</span>
          <span class="pf-comp-text">수천 개 객체 환경에서도 60 FPS 무결점 유지</span>
        </li>
      </ul>
    </div>
  </div>
</div>

<div class="pf-visual-frame">
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">1. Viewport Index Calculation</div>
            <div style="font-size: 0.8rem; color: #666;">카메라 영역을 기반으로 활성 그리드 범위(StartX~EndY) 즉시 산출 (O(1))</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">2. Query Stamp & Filtering</div>
            <div style="font-size: 0.8rem; color: #666;">다중 셀에 걸친 객체의 중복 처리를 막기 위한 고유 Stamp 대조</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">3. Precise AABB Check</div>
            <div style="font-size: 0.8rem; color: #666;">해당 셀 내의 활성 객체들만 뷰포트 사각형과 최종 교차 검사</div>
        </div>
    </div>
</div>

#### 3.1.1 쿼리 스탬프를 활용한 중복 검사 방지
크기가 큰 오브젝트는 여러 개의 그리드 셀에 걸쳐 존재할 수 있습니다. 뷰포트 영역 내의 셀들을 순회할 때 동일한 객체가 여러 번 쿼리되는 것을 막기 위해 **쿼리 스탬프(Query Stamp)** 기법을 도입했습니다. 쿼리가 발생할 때마다 전역 스탬프 값을 1씩 증가시키고, 이미 검사한 객체에 스탬프를 남겨 불필요한 중복 연산을 완전히 제거했습니다.

<details class="pf-details">
<summary>코드 보기: ObjectManager.cpp - O(1) 인덱싱 및 가시 객체 추출</summary>

```cpp
// ObjectManager.cpp - 뷰포트 기반 공간 분할 쿼리
void ObjectManager::QueryObjectsInRectArea(const Gdiplus::RectF& rectArea, std::vector<GameObject*>& targetOutObjects)
{
    // 월드 좌표를 셀 크기로 나누어 O(1)로 배열 인덱스 획득
    int startX = (int)floor(rectArea.X / GRID_CELL_SIZE);
    int startY = (int)floor(rectArea.Y / GRID_CELL_SIZE);
    int endX = (int)ceil((rectArea.X + rectArea.Width) / GRID_CELL_SIZE) - 1;
    int endY = (int)ceil((rectArea.Y + rectArea.Height) / GRID_CELL_SIZE) - 1;
    
    // ... 인덱스 클램핑 생략 ...

    // 매 쿼리마다 전역 스탬프 갱신
    if (++m_spatialQueryStamp == 0) m_spatialQueryStamp = 1;

    for (int y = startY; y <= endY; ++y) {
        for (int x = startX; x <= endX; ++x) {
            for (auto* obj : m_spatialGrid[x][y]) {
                // 이미 이번 프레임(쿼리)에서 처리된 객체는 O(1) 스킵
                if (obj->GetLastSpatialQueryStamp() == m_spatialQueryStamp) continue;
                obj->SetLastSpatialQueryStamp(m_spatialQueryStamp);

                if (!obj->IsEnabled() || obj->IsDead()) continue;

                // 해당 셀에 속한 객체만 AABB 교차 정밀 검사 수행
                if (rectArea.IntersectsWith(obj->GetBounds())) {
                    targetOutObjects.push_back(obj);
                }
            }
        }
    }
}
```
</details>

#### 3.1.2 동적 오브젝트의 그리드 이동 처리
플레이어나 몬스터처럼 실시간으로 좌표가 변하는 동적 객체들은 이동할 때마다 자신이 속한 그리드 셀을 갱신해야 합니다. `UpdateObjectGrid` 메서드는 객체의 위치가 셀의 경계를 넘어갔을 때만 이전 벡터 배열에서 객체를 `erase`하고 새 배열에 `push_back`하여 갱신 비용을 최소화합니다.

<details class="pf-details">
<summary>코드 보기: ObjectManager.cpp - 동적 그리드 갱신</summary>

```cpp
// ObjectManager.cpp - 객체 이동 시 그리드 셀 갱신
void ObjectManager::UpdateObjectGrid(GameObject* pObj)
{
    if (!pObj || pObj->GetType() == GO_TYPE_UI) return;

    int oldX = pObj->GetGridCellX();
    int oldY = pObj->GetGridCellY();

    Gdiplus::RectF bounds = pObj->GetBounds();
    int newX = (std::max)(0, (std::min)(GRID_WIDTH - 1, (int)floor((bounds.X + bounds.Width * 0.5f) / GRID_CELL_SIZE)));
    int newY = (std::max)(0, (std::min)(GRID_HEIGHT - 1, (int)floor((bounds.Y + bounds.Height * 0.5f) / GRID_CELL_SIZE)));

    // 속한 셀이 변경된 경우에만 이동 처리
    if (oldX == newX && oldY == newY) return;

    if (oldX >= 0 && oldX < GRID_WIDTH && oldY >= 0 && oldY < GRID_HEIGHT) {
        std::vector<GameObject*>& cell = m_spatialGrid[oldX][oldY];
        cell.erase(std::remove(cell.begin(), cell.end(), pObj), cell.end());
    }

    m_spatialGrid[newX][newY].push_back(pObj);
    pObj->SetGridCell(newX, newY);
}
```
</details>

#### 3.1.3 오브젝트 그리드 추가/제거 및 선언 구조 (Spatial Lifecycle Management)
오브젝트가 새로 생성되거나 소멸할 때, 해당 오브젝트를 그리드 시스템에 안전하게 등록하고 지워주는 과정이 필수적입니다. 또한, 월드의 크기와 타일 크기에 맞추어 그리드의 너비와 높이를 런타임 최적화를 위해 상수로 정의했습니다.

<details class="pf-details">
<summary>코드 보기: ObjectManager.h - 공간 분할 시스템 헤더 정의</summary>

```cpp
// ObjectManager.h - 공간 분할 시스템의 선언부
class ObjectManager {
public:
    void UpdateObjectGrid(GameObject* pObj);
    void AddToGrid(GameObject* pObj);
    void RemoveFromGrid(GameObject* pObj);

private:
    // 256px 정사각형 셀 기준의 정적 그리드 크기 정의
    static constexpr int GRID_CELL_SIZE = 256;
    static constexpr int GRID_WIDTH = (MAP_WIDTH * TILE_SIZE / GRID_CELL_SIZE) + 2;
    static constexpr int GRID_HEIGHT = (MAP_HEIGHT * TILE_SIZE / GRID_CELL_SIZE) + 2;

    // 2D 공간 분할 그리드 (각 셀마다 속한 오브젝트 포인터 벡터 관리)
    std::vector<GameObject*> m_spatialGrid[GRID_WIDTH][GRID_HEIGHT];
    uint32_t m_spatialQueryStamp = 1;
};
```
</details>

<details class="pf-details">
<summary>코드 보기: ObjectManager.cpp - 그리드 추가 및 삭제 구현</summary>

```cpp
// ObjectManager.cpp - 객체 생성 시 그리드 등록
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

// ObjectManager.cpp - 객체 소멸 시 그리드 제거
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
```
</details>

---

## 4. 애니메이션 시스템 및 이벤트 콜백
{: .chapter-title }

`Animator`와 `AnimationClip`을 분리하여 고도로 제어 가능한 프레임 기반 애니메이션 시스템을 구축했습니다. 특히, 애니메이션의 시각적 흐름과 게임의 논리적 흐름(데미지 판정, 사운드 재생 등)을 일치시키기 위해 **프레임 이벤트 콜백(Frame Event Callback)** 시스템을 도입했습니다.

### 4.1 클립 분리 및 프레임 진행

하나의 `SpriteSheet`에서 특정 프레임들을 잘라내어 `AnimationClip` 객체를 생성합니다. `Animator`는 현재 재생 중인 클립의 경과 시간(`m_elapsed`)을 바탕으로 현재 출력해야 할 프레임 인덱스를 계산합니다.

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">1. Update(deltaTime)</h4>
        <span class="pf-fc-badge">TIMER</span>
      </div>
      <p class="pf-fc-desc">경과 시간 누적 (`m_elapsed += deltaTime`) 및 클립 총 길이 대비 루프 판정</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">2. Frame Index 동기화</h4>
        <span class="pf-fc-badge">SYNC</span>
      </div>
      <p class="pf-fc-desc">FPS 기준 현재 출력 대상 프레임 인덱스(`currentFrameIndex`) 도출</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">3. Frame Skip 방지 검사</h4>
        <span class="pf-fc-badge" style="background: #fef2f2; color: #dc2626; border-color: #fecaca;">SAFETY</span>
      </div>
      <p class="pf-fc-desc">`m_lastTriggeredFrame`부터 현재 프레임까지 누락된 이벤트 루프 전수 검사</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact" style="border-color: #bfdbfe; background: #f8fbff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1d4ed8;">4. Logic Event Callback 실행</h4>
        <span class="pf-fc-badge">DISPATCH</span>
      </div>
      <p class="pf-fc-desc">타격 판정, 효과음, 파티클 이펙트 등 등록된 람다/함수 포인터 일괄 발동</p>
    </div>

  </div>
</div>

### 4.2 프레임 이벤트 검사 및 콜백 실행

프레임 델타 타임에 의해 여러 프레임을 한 번에 건너뛰더라도(Frame Skip), 이벤트가 누락되지 않도록 `m_lastTriggeredFrame`부터 `currentFrameIndex`까지 반복문을 돌며 등록된 모든 이벤트를 순차적으로 실행합니다.

<details class="pf-details">
<summary>코드 보기: Animator.cpp - 누락 없는 이벤트 처리 로직</summary>

```cpp
// Animator.cpp - 프레임 이벤트 처리 로직
if (currentFrameIndex != -1 && currentFrameIndex != m_lastTriggeredFrame)
{
    const std::map<int, std::wstring>& eventFrames = m_currentClip->GetEventFrames();
    const AnimationEventCallback& callback = m_currentClip->GetEventCallback();

    AnimationClip* pCurrentClipBeforeCallback = m_currentClip;

    // 건너뛴 프레임(startIdx)부터 현재 프레임(endIdx)까지 모두 검사하여 누락 방지
    for (int fi = m_lastTriggeredFrame + 1; fi <= currentFrameIndex && callback; ++fi ) {
        auto eventIt = eventFrames.find(fi);
        if (eventIt != eventFrames.end())
        {
            callback(fi, eventIt->second); // 등록된 콜백 실행 (공격 판정, 사운드 등)

            // 콜백 내부의 로직(예: 사망, 피격)으로 인해 애니메이션 상태가 변경되었다면 즉시 루프 중단
            if (pCurrentClipBeforeCallback != m_currentClip) {
                return;
            }
        }
    }
    m_lastTriggeredFrame = currentFrameIndex;
}
```
</details>

### 4.3 람다 캡처를 활용한 이벤트 바인딩

객체(Player 등) 초기화 시점에 `RegisterAnimation`을 통해 클립을 생성하고, 특정 프레임 번호에 이벤트 문자열(`eventName`)을 등록합니다. 이후 람다 함수를 통해 콜백을 바인딩하여 직관적으로 로직을 구성했습니다.

<details class="pf-details">
<summary>코드 보기: Player.cpp - 도끼질(Chop) 애니메이션 이벤트 바인딩 예시</summary>

```cpp
// Player.cpp - 도끼질 애니메이션 설정 및 이벤트 콜백 바인딩
const int CHOP_HIT_FRAME = 4; // 도끼가 나무에 맞는 프레임

// 클립에 이벤트 프레임 등록
AnimationClip* clip = m_animator->GetAnimationClip((int)PlayerState::CHOP, dir);
if (clip) {
    clip->AddEventFrame(CHOP_HIT_FRAME, L"chop_hit");
    clip->AddEventFrame(CHOP_LAST_FRAME, L"chop_end");

    // 이벤트 콜백 람다 바인딩
    clip->SetEventCallback([this](int frameIndex, const std::wstring& eventName) {
        if (eventName == L"chop_hit") {
            SoundManager::GetInstance()->PlaySFX(L"Resource/Sound/PlayerSound/Chop_tree.wav");
            this->OnChopHit(); // 타겟 나무에 데미지 적용
        }
        else if (eventName == L"chop_end") {
            this->OnChopEnd(); // 상태 전이
        }
    });
}
```
</details>

---

## 5. 데이터 로드 및 게임 진행 정보 저장
{: .chapter-title }

하드코딩을 배제하고 유지보수성을 높이기 위해 게임의 거의 모든 요소를 데이터 주도(Data-Driven) 방식으로 설계했습니다. 맵 배치부터 오브젝트의 고유 속성, 그리고 플레이어의 진행 상황까지 파일 기반으로 관리됩니다.

### 5.1 커스텀 맵 포맷(.dsm) 파싱 시스템

자체 제작한 맵 에디터에서 출력되는 `.dsm` (Don't Starve Map) 포맷을 파싱하여 게임 씬을 초기화합니다. 이 파일에는 맵의 메타데이터, 타일 정보, 이동 가능 구역(Walkable Area), 그리고 오브젝트의 배치 좌표가 포함됩니다.

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">1. File I/O &amp; BOM 제거</h4>
        <span class="pf-fc-badge">INPUT</span>
      </div>
      <p class="pf-fc-desc">`std::wifstream`으로 `.dsm` 파일 로드, UTF-8 BOM 식별 제거 및 라인 단위 스트림 분리</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">2. Section Parsing</h4>
        <span class="pf-fc-badge">PARSE</span>
      </div>
      <p class="pf-fc-desc">`# METADATA`, `# TILES`, `# OBJECTS` 헤더 기반 3대 영역 분할 및 토큰화</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">3. Object Mapping</h4>
        <span class="pf-fc-badge">BIND</span>
      </div>
      <p class="pf-fc-desc">문자열 ID를 열거형(Enum)으로 변환하고 `DataManager`로부터 스프라이트/콜라이더 리소스 매핑</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact" style="border-color: #bbf7d0; background: #fdfffe;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #16a34a;">4. MapData 씬 초기화 적용</h4>
        <span class="pf-fc-badge" style="background: #ecfdf5; color: #059669; border-color: #a7f3d0;">APPLY</span>
      </div>
      <p class="pf-fc-desc">`MapData` 엔티티 및 충돌 타일맵을 `SceneManager`에 전달하여 인게임 월드 스폰 완료</p>
    </div>

  </div>
</div>

<details class="pf-details">
<summary>코드 보기: Function.h - 맵 파일 파싱(ParseMapFileInto) 핵심 로직</summary>

```cpp
// Function.h - 맵 파일 파싱 로직 일부
template<typename GetObjectResourceInfoFunc>
inline bool ParseMapFileInto(const std::wstring& mapFileName, MapData& outMapData, GetObjectResourceInfoFunc getObjectResourceInfo)
{
    std::wifstream file(mapFileName);
    if (!file.is_open()) return false;
    
    // ... BOM 처리 생략 ...

    std::wstring line;
    enum Section { NONE, METADATA, PLAYER, TILES, OBJECTS, WALKABLE } section = NONE;

    while (std::getline(file, line)) {
        if (line.empty() || line[0] == L'#') {
            if (line.find(L"# TILES") != std::wstring::npos) section = TILES;
            else if (line.find(L"# OBJECTS") != std::wstring::npos) section = OBJECTS;
            // ... 섹션 분기 ...
            continue;
        }

        // 오브젝트 배치 정보 파싱
        if (section == OBJECTS) {
            std::wstringstream ss(line);
            std::wstring id, x, y;
            if (std::getline(ss, id, L',') && std::getline(ss, x, L',') && std::getline(ss, y, L',')) {
                
                GameObjectID objID = EnumTables::GetGameObjectID(id.c_str());
                if (objID != GOID_NONE) {
                    ResourcePathUtils::ObjectResourceDef objData;
                    objData.id = objID;
                    objData.x = std::stof(x);
                    objData.y = std::stof(y);
                    outMapData.gameObjects.push_back(objData);
                }
            }
        }
    }
    file.close();
    return true;
}
```
</details>

### 5.2 동적 속성 덮어쓰기 (Object Overrides)

맵 파싱 과정에서 생성될 오브젝트들의 크기, 콜라이더 정보, 피벗 좌표 등은 소스 코드를 수정하지 않고 텍스트 파일(`object_resource_overrides.txt`)을 수정하여 즉시 게임에 반영할 수 있습니다. `DataManager`가 게임 초기화 시점에 이 파일을 읽어 기본값을 덮어씌웁니다.

<details class="pf-details">
<summary>코드 보기: DataManager.cpp - 오브젝트 리소스 데이터 오버라이드</summary>

```cpp
// DataManager.cpp - 오브젝트 리소스 및 콜라이더 데이터 오버라이드
void DataManager::Init()
{
    // ... 정적 테이블 초기화 생략 ...

    ResourcePathUtils::ParseObjectResourceOverridesFile(L"GameData/object_resource_overrides.txt",
        [this](GameObjectID id, const ResourcePathUtils::ObjectResourceDef& overrideDef) {
            auto it = m_objectResources.find(id);
            if (it != m_objectResources.end()) {
                // 텍스트 파일에 정의된 값을 실제 게임 데이터 딕셔너리에 덮어씌움
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

### 5.3 스냅샷 기반 씬(Scene) 상태 보존

게임 플레이 도중 씬을 이동(포탈 이용 등)할 때, 플레이어의 현재 체력과 인벤토리 상태를 유지해야 합니다. `GameProgressManager`는 씬 전환 직전에 `PlayerStateSnapshot` 구조체를 이용해 플레이어의 상태를 캡처하고, 새로운 씬이 로드된 직후 `RestoreState` 함수를 통해 완벽하게 복원합니다.

<details class="pf-details">
<summary>코드 보기: Player.cpp - 상태 저장 및 복원</summary>

```cpp
// Player.cpp - 상태 캡처 (씬 전환용)
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

// Player.cpp - 상태 복원
void Player::RestoreState(const PlayerStateSnapshot& snapshot)
{
    m_hp = snapshot.hp;
    
    if (m_inventory) {
        m_inventory->ClearAllItems();
        for (const auto& item : snapshot.inventoryItems) {
            m_inventory->AddItem(item.first, item.second);
        }
    }

    if (snapshot.equippedSlotIndex >= 0) {
        ToggleEquipItem(snapshot.equippedSlotIndex);
    }
}
```
</details>

또한, 특정 보스를 클리어했을 때 신규 캐릭터가 해금되는 요소는 `game_progress.txt`에 문자열 포맷(`CHARACTER:1002,UNLOCKED:1`)으로 디스크에 영구 저장되어 다음 실행 시에도 로드됩니다.

### 5.4 영구적인 게임 진행도 저장 (Save/Load)

보스를 클리어하여 캐릭터가 해금되는 등의 영구적인 게임 진행 상황은 `GameProgressManager`를 통해 파일(`game_progress.txt`) 시스템에 기록됩니다. 파일 입출력을 통해 진행도를 파싱하고 직렬화하여 데이터를 안전하게 보존합니다.

<details class="pf-details">
<summary>코드 보기: GameProgressManager.cpp - 진행도 세이브 및 로드</summary>

```cpp
// GameProgressManager.cpp - 캐릭터 해금 정보 저장 및 로드
void GameProgressManager::SaveToFile(const std::wstring& filePath)
{
    // ... 디렉토리 생성 로직 생략 ...
    std::wofstream file(filePath);
    if (!file.is_open()) return;
    
    for (const auto& charInfo : m_gameProgress.characterUnlockInfos)
    {
        file << L"CHARACTER:" << (int)charInfo.characterID
             << L",UNLOCKED:" << (charInfo.isUnlocked ? 1 : 0) << L"\n";
    }
    file.close();
}

void GameProgressManager::LoadFromFile(const std::wstring& filePath)
{
    std::wifstream file(filePath);
    if (!file.is_open()) return;
    
    std::wstring line;
    while (std::getline(file, line))
    {
        if (line.empty() || line[0] == L'#' || line[0] == L'[') continue;
        
        if (line.find(L"CHARACTER:") == 0) {
            ParseCharacterLine(line); // 문자열을 파싱하여 해금 데이터 배열 업데이트
        }
    }
    file.close();
}

void GameProgressManager::ParseCharacterLine(const std::wstring& line)
{
    size_t charPos = line.find(L"CHARACTER:") + 10;
    size_t unlockedPos = line.find(L",UNLOCKED:") + 10;
    
    int characterID = std::stoi(line.substr(charPos, line.find(L',', charPos) - charPos));
    int unlocked = std::stoi(line.substr(unlockedPos));
    
    for (auto& charInfo : m_gameProgress.characterUnlockInfos) {
        if ((int)charInfo.characterID == characterID) {
            if (unlocked == 1) charInfo.isUnlocked = true;
            break;
        }
    }
}
```
</details>

{: .notice--success}
**Conclusion:** `WINAPI_DONT_STARVE`는 저수준의 WinAPI 환경에서도 고성능 아키텍처와 공간 최적화 기법을 통해 대규모 오브젝트 시스템을 안정적으로 구현해냈습니다.
