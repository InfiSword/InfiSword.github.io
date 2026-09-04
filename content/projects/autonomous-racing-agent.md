---
title: "PROJECT REPORT // AUTONOMOUS_RACING_AGENT"
excerpt: "Unity ML-Agents를 활용한 자율 주행 경주차 에이전트 학습 시스템 구축 및 보상 설계 최적화"
permalink: "/project/autonomous-racing-agent/"
tags: [Machine Learning, ML-Agents, AI, Unity, Reinforcement Learning]
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

    /* --- Reward Engineering --- */
    .reward-container { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 15px; margin: 20px 0; }
    .reward-box { padding: 18px; border-radius: 8px; text-align: center; }
    .reward-box.positive { border: 2px solid #28a745; background: rgba(40, 167, 69, 0.02); }
    .reward-box.negative { border: 2px solid #ff7b72; background: rgba(255, 123, 114, 0.02); }
    .reward-box.info { border: 2px solid #007bff; background: rgba(0, 123, 255, 0.02); }
    .reward-title { font-weight: 700; margin-bottom: 12px; font-family: 'Fira Code'; font-size: 0.95rem; border-bottom: 1px solid #eee; padding-bottom: 8px; }

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

    /* --- Training Table --- */
    .pf-data-table {
        width: 100%; border-collapse: collapse; margin: 20px 0; font-size: 0.9rem;
        background: #fff; border: 1px solid #e1e4e8; border-radius: 8px; overflow: hidden;
    }
    .pf-data-table th { background: rgba(0, 123, 255, 0.08); color: #007bff; padding: 12px; text-align: left; }
    .pf-data-table td { padding: 10px 12px; border-bottom: 1px solid #e1e4e8; }
    
    /* --- Flow Arrows --- */
    .flow-arrow {
        color: #007bff; font-weight: bold; font-size: 1.5rem; margin: 10px 0;
    }
    
    /* --- Responsive Grid --- */
    .pf-grid {
        display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin: 20px 0;
    }

    .highlight-box {
        background: #f0f7ff;
        border-left: 5px solid #007bff;
        padding: 20px;
        margin: 20px 0;
        border-radius: 0 8px 8px 0;
    }
</style>

Unity ML-Agents를 활용하여 정해진 트랙을 안정적으로 주행하는 자율 주행 경주차 에이전트를 학습시킨 프로젝트입니다. 시뮬레이션 환경 구축부터 보상 체계 설계, 신경망 관측 데이터 최적화까지의 전 과정을 다룹니다.

<section id="gallery" class="pf-ftd-post-gallery-section">
    <div class="pf-ftd-section-header">
        <span class="pf-ftd-section-tag pf-mono">● IN-GAME SCREENSHOT GALLERY</span>
        <h3 class="pf-ftd-section-title">인게임 핵심 시스템 & 스크린샷 미리보기</h3>
        <p class="pf-ftd-section-desc">산악 서킷 고속 코너링과 실시간 순위 리더보드 UI, 급경사 헤어핀 커브 물리 주행 제어, 그리고 ML-Agents 다방향 센서 레이캐스트와 웨이포인트 경로 디버그 시각화까지 주요 인게임 화면을 넘겨가며 확인할 수 있습니다.</p>
    </div>

    <!-- INTERACTIVE SCREENSHOT GALLERY (HORIZONTAL SCROLL & SLIDER) -->
    <div class="pf-ftd-gallery" id="pfRacingGalleryPost">
        <!-- Topbar Meta & Actions -->
        <div class="pf-ftd-gallery-topbar">
            <div class="pf-ftd-gallery-meta">
                <span class="pf-ftd-counter pf-mono">01 / 03</span>
                <span class="pf-ftd-caption">산악 서킷 고속 코너링 및 실시간 순위 리더보드 UI</span>
            </div>
            <a href="https://www.youtube.com/watch?v=OGb1UzntLnI" target="_blank" rel="noopener noreferrer" class="pf-ftd-yt-link" title="YouTube 시연 영상 새 창으로 열기">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z"/></svg>
                <span>YouTube 시연 영상 ↗</span>
            </a>
        </div>

        <!-- Main Viewing Stage -->
        <div class="pf-ftd-stage">
            <button type="button" class="pf-ftd-nav pf-ftd-prev" onclick="pfFtdGalleryGo(-1, this)" aria-label="이전 스크린샷">‹</button>
            <img class="pf-ftd-main-img" src="/assets/images/Car%20Simulation/InGame/racing_ingame_01.png" alt="산악 서킷 고속 코너링 및 실시간 순위 리더보드 UI" loading="eager">
            <button type="button" class="pf-ftd-nav pf-ftd-next" onclick="pfFtdGalleryGo(1, this)" aria-label="다음 스크린샷">›</button>
        </div>

        <!-- Horizontal Scrollable Thumbnail Strip -->
        <div class="pf-ftd-thumbs-wrapper">
            <div class="pf-ftd-thumbs-strip" role="region" aria-label="스크린샷 썸네일 가로 스크롤 목록">
                <button type="button" class="pf-ftd-thumb-card is-active" data-index="0" data-title="산악 서킷 고속 코너링 및 실시간 순위 리더보드 UI" onclick="pfFtdGallerySelect(0, this)" title="1. 산악 서킷 고속 코너링 및 실시간 순위 리더보드 UI">
                    <img src="/assets/images/Car%20Simulation/InGame/racing_ingame_01.png" alt="1. 산악 서킷 고속 코너링 및 실시간 순위 리더보드 UI" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">01</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="1" data-title="급경사 헤어핀 커브 구간 및 차량 물리 주행 제어" onclick="pfFtdGallerySelect(1, this)" title="2. 급경사 헤어핀 커브 구간 및 차량 물리 주행 제어">
                    <img src="/assets/images/Car%20Simulation/InGame/racing_ingame_02.png" alt="2. 급경사 헤어핀 커브 구간 및 차량 물리 주행 제어" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">02</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="2" data-title="ML-Agents 다방향 센서 레이캐스트 및 웨이포인트 경로 디버그 시각화" onclick="pfFtdGallerySelect(2, this)" title="3. ML-Agents 다방향 센서 레이캐스트 및 웨이포인트 경로 디버그 시각화">
                    <img src="/assets/images/Car%20Simulation/InGame/racing_ingame_03.png" alt="3. ML-Agents 다방향 센서 레이캐스트 및 웨이포인트 경로 디버그 시각화" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">03</span>
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

본 프로젝트는 강화학습(Reinforcement Learning)을 통해 복잡한 트랙 환경에서 자율 주행이 가능한 경주차 에이전트를 개발하는 것을 목표로 합니다. **체크포인트 기반 네비게이션 시스템**과 **다층적 보상 설계**를 통해 에이전트가 트랙을 순차적으로 완주하도록 유도하며, **레이캐스트 기반 환경 감지**와 **입력 스무딩**을 통해 물리적으로 안정적인 주행 성능을 확보했습니다.

<div class="pf-visual-frame">
    <img src="/assets/Gifs/autonomous-racing.gif" alt="Unity ML-Agents Training Visualization" style="width: 100%; border-radius: 8px;">
    <p style="text-align: center; color: #8b949e; font-size: 0.9rem; margin-top: 10px;">Unity ML-Agents를 통한 자율 주행 학습 과정</p>
</div>

<div class="pf-visual-frame">
    <img src="/assets/images/Car%20Simulation/mlagent_4.png" alt="ML-Agents Track Environment" style="width: 100%; border-radius: 8px;">
    <p style="text-align: center; color: #8b949e; font-size: 0.9rem; margin-top: 10px;">구불구불한 도로와 바위 지형으로 구성된 학습 트랙 환경</p>
</div>

---

## 2. 체크포인트 기반 네비게이션 시스템
{: .chapter-title }

강화학습 초기 단계에서 에이전트들이 최단 경로를 찾는 과정에서 트랙을 역주행하거나 경로를 이탈하는 문제가 빈번하게 발생했습니다. 이를 해결하기 위해 **순차적 체크포인트 시스템**을 구축하여 에이전트가 반드시 트랙의 순방향으로 전진하도록 강제했습니다.

<div class="highlight-box">
    <strong style="color: #007bff;">시행착오 및 문제 해결:</strong><br>
    초기 설계 시 에이전트가 목표 지점까지의 최단 경로를 찾는 과정에서 역주행하는 현상이 발생했습니다. 이를 방지하기 위해 목표 지점 사이에 다수의 체크포인트를 순차적으로 배치하고, 현재 가야 할 체크포인트를 통과해야만 다음 포인트에서 보상을 받을 수 있도록 논리를 강화했습니다. 추가로, 트랙 생성의 효율성을 위해 스플라인(Spline)을 따라 체크포인트를 자동 생성하는 툴을 개발하여 개발 생산성을 크게 높였습니다.
</div>

### 2.1 시스템 아키텍처

트랙의 스플라인 곡선을 분석하여 체크포인트를 자동 생성하고, 에이전트의 진행 상태를 관리하는 계층적 구조를 설계했습니다. `SplineToCheckpointGenerator`를 통해 수작업 없이 일정한 간격으로 체크포인트와 스타팅 그리드를 구성할 수 있습니다.

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- 0. START -->
    <div class="pf-fc-pill-start">
      <span class="pf-fc-mono-tag">INPUT</span>
      <span>Track Spline Curve (입력 곡선)</span>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 01: SplineToCheckpointGenerator -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">SplineToCheckpointGenerator (오프라인 베이킹)</h4>
        <span class="pf-fc-badge">EDITOR TOOL</span>
      </div>
      <p class="pf-fc-desc">트랙 스플라인의 접선(Tangent)·곡률을 샘플링하여 등간격 Trigger Collider 일괄 생성 및 태그 자동 부여</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 02: SpawnPointManager / TrackData -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">SpawnPointManager / TrackData (런타임 트랙 관리)</h4>
        <span class="pf-fc-badge" style="background: #ecfdf5; color: #059669; border-color: #a7f3d0;">TRACK MANAGER</span>
      </div>
      <p class="pf-fc-desc">체크포인트 인덱스 순서 배열 바인딩, 동적 스폰 그리드 위치 지정, 결승선(Finish) 통과 및 랩(Lap) 타임 기록</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 03: SimcadeCarAgent_Auto -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #bfdbfe; background: #f8fbff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1d4ed8;">SimcadeCarAgent_Auto (RL 에이전트 연동)</h4>
        <span class="pf-fc-badge">RL AGENT</span>
      </div>
      <p class="pf-fc-desc">순차적 체크포인트 통과 감지 및 단계별 보상 지급, 역주행 방지 유도, 에피소드 종료 및 차량 리셋 제어</p>
    </div>

    <!-- Arrow -->
    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- 4. END -->
    <div class="pf-fc-pill-end">
      <span class="pf-fc-mono-tag">OUTPUT</span>
      <span>실시간 보상 최적화 &amp; 완벽한 주행 경로 학습</span>
    </div>

  </div>
</div>

**스플라인 기반 체크포인트 자동 생성 로직**
Unity Spline을 기반으로 정해진 개수만큼의 체크포인트를 트랙의 곡률(탄젠트)에 맞게 회전시켜 자동 생성하고, TrackData에 연동하는 핵심 로직입니다.

<details class="pf-details">
<summary>코드 보기: 체크포인트 자동 생성 로직</summary>

```csharp
// SplineToCheckpointGenerator.cs: 체크포인트 자동 생성 로직
public void GenerateCheckpoints()
{
    // ... 초기화 및 검증 생략 ...
    for (int i = 0; i < totalCheckpoints; i++)
    {
        // 스플라인을 따라 위치 계산
        float progress = (float)(i + 1) / totalCheckpoints;
        spline.Spline.Evaluate(progress, out float3 pos, out float3 tan, out float3 up);
        if (math.lengthsq(tan) == 0) tan = math.forward(); 

        // 체크포인트 GameObject 생성 및 배치
        GameObject checkpointObj = new GameObject($"Checkpoint_{i:00}");
        checkpointObj.transform.SetParent(checkpointRoot.transform);
        checkpointObj.transform.localPosition = (Vector3)pos;
        checkpointObj.transform.localRotation = Quaternion.LookRotation((Vector3)tan, (Vector3)up);

        // BoxCollider(Trigger)와 Tag 자동 추가
        BoxCollider collider = checkpointObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = checkpointColliderSize;
        checkpointObj.tag = checkpointTag;

        CheckPoint cpScript = checkpointObj.AddComponent<CheckPoint>();
        cpScript.Init(i); 
        checkPointsTemp[i] = checkpointObj.transform;
    }
    
    // TrackData에 최종 등록
    trackData.SetCheckPoints(checkPointsTemp);
}
```
</details>

**순차적 체크포인트 검증 로직**
차량이 통과한 체크포인트의 인덱스가 현재 기대되는 순서(NextCheckpointIndex)와 일치하는지 확인하여 논리적 주행 경로를 보장합니다.

<details class="pf-details">
<summary>코드 보기: 순차적 체크포인트 검증 로직</summary>

```csharp
// CarProgress.cs: 체크포인트 충돌 처리 (내부 상태 갱신)
private void CheckpointPassed(int checkpointIndex)
{
    if (IsFinished || trackData == null) return;

    // 내가 가야 할 체크포인트가 맞는지 확인
    if (checkpointIndex == NextCheckpointIndex)
    {
        // 다음 체크포인트로 인덱스 증가
        NextCheckpointIndex++;

        // 마지막 체크포인트를 넘었으면? (랩 종료 또는 완주)
        if (NextCheckpointIndex >= trackData.CheckpointCount)
        {
            NextCheckpointIndex = 0; // 다시 0번부터
            CurLap++;

            if (CurLap > maxLab)
            {
                IsFinished = true;
                FinalRank = CurRank;
                GameManager.Race.AddFinishedCount();
            }
        }
    }
}
```
</details>

---

## 3. 정교한 보상 설계 (Reward Engineering)
{: .chapter-title }

에이전트가 단순히 전진하는 것을 넘어, 사고 없이 효율적으로 주행하도록 **다층적 보상 시스템**을 설계했습니다.

### 3.1 목표 지향 보상 (Goal-Oriented)

에이전트가 트랙의 흐름에 따라 체크포인트를 순차적으로 정복하도록 유도합니다.

<div class="reward-container">
    <div class="reward-box positive">
        <div class="reward-title">Positive Rewards</div>
        <div style="text-align: left; font-size: 0.85rem; color: #555;">
            • 체크포인트 통과: +1.0 (정밀 도달 확인)<br>
            • Finish 라인 통과: +10.0 (최종 목표 달성)<br>
            • 거리 감소 보상: 매 프레임 목표에 근접 시 비례 보상<br>
            • 방향 정렬 보상: 차량 전방과 목표 방향 일치 시 가점
        </div>
    </div>
    <div class="reward-box negative">
        <div class="reward-title">Negative Penalties</div>
        <div style="text-align: left; font-size: 0.85rem; color: #555;">
            • 잘못된 체크포인트: -0.5 (역주행/우회 방지)<br>
            • 시간 페널티: 매 프레임 -0.001 (빠른 완주 유도)<br>
            • 체크포인트 타임아웃: 30초 내 미도달 시 에피소드 종료
        </div>
    </div>
</div>

### 3.2 안전성 및 복구 보상 (Safety & Recovery)

차량이 벽에 충돌하거나 멈추는 등의 비효율적 상황을 최소화하도록 설계된 시스템입니다.

<div class="reward-container">
    <div class="reward-box negative">
        <div class="reward-title">Safety Penalties</div>
        <div style="text-align: left; font-size: 0.85rem; color: #555;">
            • 벽 충돌: -1.0 (즉각적 물리 충돌 발생 시)<br>
            • 벽 근접 페널티: 거리에 따라 비선형적 감점<br>
            • 역주행 페널티: 목표와 150도 이상 차이 시 -0.2 (per frame)<br>
            • 미끄러짐(Slip): 드리프트 임계값 초과 시 비례 감점
        </div>
    </div>
    <div class="reward-box info">
        <div class="reward-title">Recovery & Exit</div>
        <div style="text-align: left; font-size: 0.85rem; color: #555;">
            • 벽 탈출 후진: +0.01 (복구를 위한 전략적 행동)<br>
            • 정지 감지: 2초 이상 속도 0.5 미만 시 감점<br>
            • 뒤집힘 감지: 160도 이상 기울어짐 5초 지속 시 종료<br>
            • 맵 이탈: FallZone 도달 시 -5.0 및 에피소드 종료
        </div>
    </div>
</div>

**통합 보상 계산 및 페널티 부여 로직**
목표 지점을 향한 거리 보상, 방향 정렬(Dot Product), 그리고 전방/후방 레이캐스트 센서를 활용한 벽 근접 페널티를 종합적으로 계산하는 핵심 함수입니다.

<details class="pf-details">
<summary>코드 보기: 벽 근접 페널티 및 역주행 감지 로직</summary>

```csharp
// SimcadeCarAgent_Auto.cs: 보상 계산 로직
private void CalculateRewards() {
    // 1. 기본 시간 페널티 (빠른 완주 유도)
    AddReward(timePenalty);

    Vector3 targetPosition = GetTargetPosition();
    float distanceToTarget = Vector3.Distance(transform.position, targetPosition); 

    if (targetPosition != transform.position) {
        // 2. 방향 정렬 보상 (목표 방향과 현재 차량의 전방 방향 일치도)
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        float directionDot = Vector3.Dot(transform.forward, targetDirection);
        AddReward(directionDot * directionAlignmentRewardFactor);

        // 3. 거리 기반 보상 (가까워질수록 보상)
        float distanceChange = lastDistanceToTarget - distanceToTarget;
        float weight = (Vector3.Dot(rb.linearVelocity, transform.forward) > 0.1f) ? 1.0f : 0.5f;
        AddReward(distanceChange * distanceRewardFactor * weight);
        
        lastDistanceToTarget = distanceToTarget;
    }

    // 4. 벽 근접 페널티 (레이캐스트 활용)
    if (!isStuckInWall) {
        ApplyProximityPenalty(wallRayDirections, wallLayer, wallProximityDistance, proximityPenalty);
    }
}

// 레이캐스트 기반 근접 페널티 계산
private void ApplyProximityPenalty(Vector3[] directions, LayerMask layer, float distance, float penaltyCoefficient) {
    for (int i = 0; i < directions.Length; i++) {
        Vector3 worldRayDir = transform.TransformDirection(directions[i]);
        if (Physics.Raycast(transform.position, worldRayDir, out RaycastHit hit, distance, layer)) {
            // 거리가 가까울수록 더 큰 페널티를 부여하여 충돌 회피 유도
            float penalty = penaltyCoefficient * (1f - (hit.distance / distance));
            AddReward(penalty * Time.fixedDeltaTime);
        }
    }
}
```
</details>

---

## 4. 환경 감지 및 행동 결정 아키텍처
{: .chapter-title }

에이전트는 주변 환경을 입체적으로 인식하기 위해 **총 36차원의 관측(Observation) 데이터**를 수집하며, 이를 기반으로 물리적인 제어를 수행합니다.

### 4.1 레이캐스트 기반 환경 센싱

에이전트가 도로의 폭, 곡률, 장애물과의 거리를 정확히 파악하도록 다방향 레이캐스트 시스템을 구축했습니다.

<div class="pf-visual-frame">
    <a href="https://www.youtube.com/watch?v=OGb1UzntLnI" target="_blank" rel="noopener noreferrer" aria-label="Autonomous Racing Agent 유튜브 영상 보기"><img src="/assets/images/Car%20Simulation/스크린샷%202026-04-27%20105116.png" alt="Raycast Sensor Visualization" style="width: 100%; border-radius: 8px;"></a>
    <p style="text-align: center; color: #8b949e; font-size: 0.9rem; margin-top: 10px;">차량의 레이캐스트 센서 시스템 시각화 (빨간색: 감지된 대상, 초록색: 일반 센서 범위, 체크포인트 및 경로 스플라인 표시)</p>
</div>

<div class="pf-visual-frame">
<div class="pf-grid">
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">전방 센서 (9개)</div>
<div style="font-size: 0.85rem; color: #666;">
180도 범위 (-90~90)<br>감지 거리 15m (안전 제동 거리)<br>도로 경계 및 벽 식별
</div>
</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">후방 센서 (3개)</div>
<div style="font-size: 0.85rem; color: #666;">
60도 범위 (-30~30)<br>감지 거리 5m<br>후진 시 후방 충돌 방지
</div>
</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">관측 데이터 구성</div>
<div style="font-size: 0.85rem; color: #666;">
선속도/각속도 (7차원)<br>레이캐스트 정규화 거리 (21차원)<br>목표 방향 및 거리 (4차원)
</div>
</div>
</div>
</div>

### 4.2 인지-행동 지연 없는 직접 제어 (Direct Action Control)

초기 학습 단계에서는 급격한 조향 지터링(Jittering)을 방지하기 위해 프레임 간 조향값의 변화량을 제한하는 **입력 보정(Action Smoothing)** 알고리즘을 테스트했습니다. 그러나 차량의 반응성을 떨어뜨려 코너링에서 에이전트의 의도가 지연되는 현상이 확인되었습니다.

<div class="pf-visual-frame">
    <img src="/assets/images/Car%20Simulation/mlagent_3.png" alt="Direct Control Visualization" style="width: 100%; border-radius: 8px;">
    <p style="text-align: center; color: #8b949e; font-size: 0.9rem; margin-top: 10px;">신경망 출력을 물리 차량 컨트롤러에 직접 매핑하여 반응성을 극대화</p>
</div>

<div class="highlight-box">
    <strong style="color: #007bff;">입력 보정 제거 및 반응성 향상:</strong><br>
    최종 모델에서는 신경망이 현재 상황을 보고 판단한 원시 출력값(Continuous Actions)을 즉각적으로 물리 엔진(Vehicle Controller)에 전달하도록 수정했습니다. 지터링 문제는 강화학습의 하이퍼파라미터 조정 및 보상 체계(Slip Penalty 등) 강화를 통해 궤적 자체를 부드럽게 그리도록 유도하는 방식으로 근본적인 해결을 이끌어냈습니다.
</div>

**즉각적 행동 매핑 로직**
입력 스무딩을 제거하여 AI의 조향 및 가속 의도가 지연 없이 차량에 반영되도록 최적화한 컨트롤 메서드입니다.

<details class="pf-details">
<summary>코드 보기: 즉각적 행동 매핑 로직</summary>

```csharp
public override void OnActionReceived(ActionBuffers actions)
{
    // 액션 스무딩 제거: AI의 의도가 즉각적으로 반영되도록 수정
    float steeringInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
    float accelerationInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

    lastSteeringInput = steeringInput;
    lastAccelerationInput = accelerationInput;

    // 보정 없이 차량 컨트롤러에 원시 입력값 직접 전달
    vehicleController.ProvideInputs(accelerationInput, steeringInput, 0f);
    
    CalculateRewards();
    CheckEpisodeEndConditions();
    GetComponent<CarProgress>().UpdateProgress();
}
```
</details>

