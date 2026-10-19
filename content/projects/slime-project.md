---
title: "PROJECT REPORT // SLIME PROJECT"
excerpt: "상태머신 기반 게임 로직 및 확장 가능한 몬스터 AI 시스템"
permalink: "/project/slime-project/"
tags: [Unity, AI, FSM, Design Pattern]
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

    /* --- Flow Arrows --- */
    .flow-arrow {
        color: #007bff; font-weight: bold; font-size: 1.5rem; margin: 10px 0;
    }

    /* --- Grid Systems --- */
    .pf-grid {
        display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; margin: 25px 0;
    }
    .pf-grid-item {
        background: #fff; border: 1px solid #e1e4e8; border-radius: 12px; padding: 25px; text-align: center;
        transition: 0.3s;
    }
    .pf-grid-item:hover { border-color: #007bff; transform: translateY(-5px); box-shadow: 0 10px 20px rgba(0,123,255,0.05); }
    .pf-grid-label { color: #007bff; font-weight: 700; margin-bottom: 12px; font-family: 'Fira Code', monospace; font-size: 1rem; display: block; }
    .pf-grid-desc { color: #555; font-size: 0.85rem; line-height: 1.6; }
</style>

Slime Project는 하드코어 액션 퍼즐 플랫포머 게임으로, **FiniteStateMachine(FSM) 기반 상태 관리 시스템**을 통해 모든 게임 엔티티의 행동을 체계적으로 관리합니다. 플레이어 슬라임의 움직임, 변신 능력, 그리고 변신 시 능력 변화를 구현했으며, 보스 몬스터와 일반 몬스터의 AI 로직을 담당하여 확장 가능한 게임 시스템을 구축했습니다.

<section id="gallery" class="pf-ftd-post-gallery-section">
    <div class="pf-ftd-section-header">
        <span class="pf-ftd-section-tag pf-mono">● IN-GAME SCREENSHOT GALLERY</span>
        <h3 class="pf-ftd-section-title">인게임 핵심 시스템 & 스크린샷 미리보기</h3>
        <p class="pf-ftd-section-desc">플레이어 슬라임의 조작과 몬스터 조우, 토끼 몬스터 타격 액션부터 거대 곰 보스 몬스터와의 페이즈별 전투까지 주요 인게임 화면을 넘겨가며 옆으로 스크롤하여 확인할 수 있습니다.</p>
    </div>

    <!-- INTERACTIVE SCREENSHOT GALLERY (HORIZONTAL SCROLL & SLIDER) -->
    <div class="pf-ftd-gallery" id="pfSlimeGalleryPost">
        <!-- Topbar Meta & Actions -->
        <div class="pf-ftd-gallery-topbar">
            <div class="pf-ftd-gallery-meta">
                <span class="pf-ftd-counter pf-mono">01 / 04</span>
                <span class="pf-ftd-caption">거대 곰 보스 몬스터와의 전투 및 패턴 공략</span>
            </div>
            <a href="https://www.youtube.com/watch?v=83lMfQY1Rwo" target="_blank" rel="noopener noreferrer" class="pf-ftd-yt-link" title="YouTube 시연 영상 새 창으로 열기">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z"/></svg>
                <span>YouTube 시연 영상 ↗</span>
            </a>
        </div>

        <!-- Main Viewing Stage -->
        <div class="pf-ftd-stage">
            <button type="button" class="pf-ftd-nav pf-ftd-prev" onclick="pfFtdGalleryGo(-1, this)" aria-label="이전 스크린샷">‹</button>
            <img class="pf-ftd-main-img" src="/assets/images/Slime/InGame/slime_ingame_01.png" alt="거대 곰 보스 몬스터와의 전투 및 패턴 공략" loading="eager">
            <button type="button" class="pf-ftd-nav pf-ftd-next" onclick="pfFtdGalleryGo(1, this)" aria-label="다음 스크린샷">›</button>
        </div>

        <!-- Horizontal Scrollable Thumbnail Strip -->
        <div class="pf-ftd-thumbs-wrapper">
            <div class="pf-ftd-thumbs-strip" role="region" aria-label="스크린샷 썸네일 가로 스크롤 목록">
                <button type="button" class="pf-ftd-thumb-card is-active" data-index="0" data-title="거대 곰 보스 몬스터와의 전투 및 패턴 공략" onclick="pfFtdGallerySelect(0, this)" title="1. 거대 곰 보스 몬스터와의 전투 및 패턴 공략">
                    <img src="/assets/images/Slime/InGame/slime_ingame_01.png" alt="1. 거대 곰 보스 몬스터와의 전투 및 패턴 공략" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">01</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="1" data-title="필드 탐색 및 기본 조작·몬스터 조우" onclick="pfFtdGallerySelect(1, this)" title="2. 필드 탐색 및 기본 조작·몬스터 조우">
                    <img src="/assets/images/Slime/InGame/slime_ingame_02.png" alt="2. 필드 탐색 및 기본 조작·몬스터 조우" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">02</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="2" data-title="토끼 몬스터 사냥 및 타격 액션 이펙트" onclick="pfFtdGallerySelect(2, this)" title="3. 토끼 몬스터 사냥 및 타격 액션 이펙트">
                    <img src="/assets/images/Slime/InGame/slime_ingame_03.png" alt="3. 토끼 몬스터 사냥 및 타격 액션 이펙트" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">03</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="3" data-title="플랫포머 기믹 지형 돌파 및 몬스터 전투" onclick="pfFtdGallerySelect(3, this)" title="4. 플랫포머 기믹 지형 돌파 및 몬스터 전투">
                    <img src="/assets/images/Slime/InGame/slime_ingame_04.png" alt="4. 플랫포머 기믹 지형 돌파 및 몬스터 전투" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">04</span>
                </button>
            </div>
        </div>
        <div class="pf-ftd-hint">
            <span>Tip: 좌우 화살표 버튼, 키보드 방향키(←, →), 또는 마우스 휠로 썸네일을 옆으로 스크롤하여 탐색할 수 있습니다.</span>
        </div>
    </div>
</section>

---

## 1. 프로젝트 개요
{: .chapter-title }

몬스터의 형태로 변신하며 탐색과 전투를 이어가는 2D 액션 플랫포머입니다. 게임 로직을 **상태 관리와 기능 조합**으로 나누어, 다양한 행동과 능력을 확장할 수 있는 구조를 설계했습니다.

---

## 2. FiniteStateMachine 기반 상태 관리 시스템
{: .chapter-title }

### 2.1 행동 상태와 전환 관리

순찰·탐색·공격을 독립된 행동 상태로 나누었습니다. **FSM은 현재 행동과 전환 흐름을 관리**하고, 각 상태는 자신에게 필요한 판단과 처리를 담당합니다. 행동을 추가하거나 수정할 때 관련 상태를 중심으로 변경할 수 있도록 구성했습니다.

### 2.2 State 베이스 클래스

몬스터와 플레이어가 공통 `State`를 상속해 상태의 진입·실행·종료 규칙을 공유합니다. 대상별로 필요한 기능은 각 분기에서 확장합니다.

| 구분 | 핵심 역할 |
| --- | --- |
| State | 상태가 따르는 공통 생명주기 정의 |
| Monster_State | 몬스터의 감지·애니메이션 등 공통 처리 |
| Player_State | 플레이어의 이동·공격·대시 기능에 공통 기반 제공 |

몬스터의 행동 전환은 FSM이, 플레이어의 기능 조합은 플레이어 컨트롤러가 관리합니다.

### 2.3 행동 재사용과 확장

공통 이동·공격 행동을 바탕으로 곰의 돌진, 거미의 도약처럼 몬스터별 특징을 확장했습니다. 속도와 행동 시간은 별도 데이터로 관리해 **행동 규칙과 조정값을 분리**했습니다.

---

## 3. 플레이어 슬라임 및 변신 시스템
{: .chapter-title }

### 3.1 기능 단위 조합

이동·공격·대시를 독립된 기능으로 나누고 플레이어가 이를 조합하도록 구성했습니다. 기능 간 의존성을 줄여 새로운 동작이나 능력을 추가하기 쉽도록 설계했습니다.

### 3.2 변신 능력 시스템

몬스터를 처치하면 해당 형태의 능력을 활용할 수 있습니다. 기본 슬라임은 원거리 공격과 대시를, 토끼 형태는 더블 점프와 벽 점프를 사용해 전투와 이동 방식을 바꿉니다.
