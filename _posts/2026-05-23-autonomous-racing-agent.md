---
layout: single
title: "PROJECT REPORT // AUTONOMOUS_RACING_AGENT"
excerpt: "Unity ML-Agents를 활용한 자율 주행 경주차 에이전트 학습 시스템 구축 및 보상 설계 최적화"
categories: [project]
permalink: /project/autonomous-racing-agent/
tags: [Machine Learning, ML-Agents, AI, Unity, Reinforcement Learning]
toc: true
toc_sticky: true
classes: wide
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
    .details-desc {
        padding: 15px 20px;
        background: #fff;
        color: #666;
        font-size: 0.95rem;
        border-top: 1px solid #e1e4e8;
        line-height: 1.6;
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

---

## 1. 프로젝트 개요
{: .chapter-title }

본 프로젝트는 강화학습(Reinforcement Learning)을 통해 복잡한 트랙 환경에서 자율 주행이 가능한 경주차 에이전트를 개발하는 것을 목표로 합니다. **체크포인트 기반 네비게이션 시스템**과 **다층적 보상 설계**를 통해 에이전트가 트랙을 순차적으로 완주하도록 유도하며, **레이캐스트 기반 환경 감지**와 **입력 스무딩**을 통해 물리적으로 안정적인 주행 성능을 확보했습니다.

<div class="pf-visual-frame">
    <div style="background: #eee; height: 300px; display: flex; align-items: center; justify-content: center; border-radius: 8px; border: 2px dashed #ccc;">
        <span style="color: #666;">[ML-Agents 학습 시각화 영상/GIF 자리]</span>
    </div>
    <p style="text-align: center; color: #8b949e; font-size: 0.9rem; margin-top: 10px;">Unity ML-Agents를 통한 자율 주행 학습 과정</p>
</div>

---

## 2. 체크포인트 기반 네비게이션 시스템
{: .chapter-title }

강화학습 초기 단계에서 에이전트들이 최단 경로를 찾는 과정에서 트랙을 역주행하거나 경로를 이탈하는 문제가 빈번하게 발생했습니다. 이를 해결하기 위해 **순차적 체크포인트 시스템**을 구축하여 에이전트가 반드시 트랙의 순방향으로 전진하도록 강제했습니다.

<div class="highlight-box">
    <strong style="color: #007bff;">시행착오 및 문제 해결:</strong><br>
    초기 설계 시 에이전트가 목표 지점까지의 최단 경로를 찾는 과정에서 역주행하는 현상이 발생했습니다. 이를 방지하기 위해 목표 지점 사이에 다수의 체크포인트를 순차적으로 배치하고, 현재 가야 할 체크포인트를 통과해야만 다음 포인트에서 보상을 받을 수 있도록 논리를 강화했습니다.
</div>

### 2.1 시스템 아키텍처

트랙의 스플라인 곡선을 분석하여 체크포인트를 자동 생성하고, 에이전트의 진행 상태를 관리하는 계층적 구조를 설계했습니다.

<div class="pf-visual-frame">
<div class="pf-arch-diagram">
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">SplineToCheckpointGenerator</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">스플라인 경로 자동 분석</span>
<span class="pf-arch-item">Trigger Collider 일괄 배치</span>
<span class="pf-arch-item">태그 자동 할당</span>
</div>
</div>
<div class="flow-arrow">↓</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">SpawnPointManager / TrackData</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">체크포인트 인덱스 배열 관리</span>
<span class="pf-arch-item">다중 스폰 포인트 제어</span>
<span class="pf-arch-item">Finish 라인 및 랩(Lap) 정보 관리</span>
</div>
</div>
<div class="flow-arrow">↓</div>
<div class="pf-arch-layer">
<div class="pf-arch-layer-title">SimcadeCarAgent_Auto</div>
<div class="pf-arch-layer-items">
<span class="pf-arch-item">체크포인트 도달 감지 및 보상</span>
<span class="pf-arch-item">목표 위치 실시간 계산</span>
<span class="pf-arch-item">에피소드 종료 및 리셋 제어</span>
</div>
</div>
</div>
</div>

<details class="pf-details">
<summary>코드 보기: 순차적 체크포인트 검증 로직</summary>
<div class="details-desc">
차량이 통과한 체크포인트의 인덱스가 현재 기대되는 순서(NextCheckpointIndex)와 일치하는지 확인하여 논리적 주행 경로를 보장합니다.
</div>

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

<details class="pf-details">
<summary>코드 보기: 벽 근접 페널티 및 역주행 감지 로직</summary>

```csharp
// SimcadeCarAgent_Auto.cs
private void CalculateRewards() {
    // 1. 전방 벽 근접 페널티 (레이캐스트 기반)
    ApplyProximityPenalty(wallRayDirections, wallLayer, wallProximityDistance, proximityPenalty);

    // 2. 역주행 감지 (Dot Product 활용)
    Vector3 targetDirection = (targetPosition - transform.position).normalized;
    float directionDot = Vector3.Dot(transform.forward, targetDirection);
    
    // 목표 방향과 150도 이상 차이 날 경우 역주행으로 간주
    if (directionDot < -0.866f && rb.linearVelocity.magnitude > 1.5f) {
        AddReward(wrongWayPenalty);
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

### 4.2 입력 스무딩 및 연속 제어 (Action Smoothing)

강화학습 초기 단계의 급격한 조향 지터링(Jittering)을 방지하고 실제 차량과 유사한 관성을 부여하기 위해 **입력 보정 알고리즘**을 적용했습니다.

<div class="highlight-box">
    <strong style="color: #007bff;">입력 보정 원리:</strong><br>
    신경망이 현재 상황을 보고 판단한 '원시 조향값(rawSteering)'과 직전 프레임의 '최종 조향값(lastSteeringInput)'을 비교합니다. 프레임당 변화량(maxSteeringChange)을 제한하여 핸들이 급격하게 꺾이지 않도록 부드러운 궤적을 생성합니다.
</div>

<details class="pf-details">
<summary>코드 보기: Action Smoothing 알고리즘</summary>

```csharp
public override void OnActionReceived(ActionBuffers actions)
{
    // 1. 신경망의 원시 행동 값 수신 (-1.0 ~ 1.0)
    float rawSteering = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
    float rawAcceleration = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

    // 2. 입력 스무딩: 프레임당 최대 변화량 제한 (조향: 0.5, 가속: 0.7)
    float steeringInput = Mathf.Clamp(rawSteering,
        lastSteeringInput - maxSteeringChange,
        lastSteeringInput + maxSteeringChange);
    
    float accelerationInput = Mathf.Clamp(rawAcceleration,
        lastAccelerationInput - maxAccelerationChange,
        lastAccelerationInput + maxAccelerationChange);

    // 3. 상태 저장 및 차량 컨트롤러에 전달
    lastSteeringInput = steeringInput;
    lastAccelerationInput = accelerationInput;
    vehicleController.ProvideInputs(accelerationInput, steeringInput, 0f);
}
```
</details>

---

## 5. 학습 설정 및 일반화 성능 확보
{: .chapter-title }

특정 트랙에 과적합(Overfitting)되지 않고 다양한 환경에서 안정적으로 주행할 수 있도록 학습 설정을 최적화했습니다.

### 5.1 하이퍼파라미터 (PPO 기반)

| 파라미터 | 값 | 설명 |
| :--- | :--- | :--- |
| **Trainer Type** | PPO | Proximal Policy Optimization |
| **Batch Size** | 1024 | 대규모 에이전트 동시 업데이트 최적화 |
| **Learning Rate** | 0.0003 | 표준 안정 학습률 |
| **Hidden Units** | 256 | 신경망 복잡도 설정 (2개 층) |
| **Max Steps** | 10,000,000 | 충분한 수렴을 위한 대규모 학습 스텝 |
| **Curiosity Signal** | 0.02 | 새로운 경로 탐험(Exploration) 가중치 활성화 |

### 5.2 다중 트랙 랜덤 학습 시스템

에이전트의 일반화 성능을 극대화하기 위해 매 에피소드 시작 시 랜덤하게 트랙과 스폰 포인트를 교체하는 시스템을 도입했습니다.

```csharp
public override void OnEpisodeBegin()
{
    // 1. 등록된 모든 트랙 중 하나를 랜덤 선택
    currentTrack = allTracks[Random.Range(0, allTracks.Count)];
    
    // 2. 해당 트랙의 스폰 포인트 리스트 중 랜덤 위치 선정
    Transform spawnTransform = currentTrack.GetStartPoint(Random.Range(0, currentTrack.StartPointsCount));
    
    // 3. 물리 상태 초기화 및 배치
    transform.position = spawnTransform.position;
    transform.rotation = spawnTransform.rotation;
    rb.linearVelocity = Vector3.zero;
    carProgress.Initialize();
}
```

---

## 6. 기술적 성과 및 결론
{: .chapter-title }

*   **주행 안정성:** Action Smoothing을 통해 지터링 없는 부드러운 코너링과 직선 주행 구현.
*   **환경 적응력:** 다중 트랙 학습을 통해 학습되지 않은 새로운 트랙에서도 90% 이상의 완주율 달성.
*   **복구 지능:** 충돌 시 스스로 후진하여 경로로 복귀하거나, 복구 불가능 시 빠르게 재시도하는 효율적 학습 루프 구축.

{: .notice--success}
**Conclusion:** ML-Agents의 연속 행동 공간(Continuous Action Space)을 물리 기반 차량 컨트롤러와 결합하여, 단순한 경로 추적을 넘어 물리적 한계를 고려하며 최적의 궤적을 그리는 자율 주행 AI를 성공적으로 구축했습니다.
