---
layout: single
title: "PROJECT REPORT // AUTONOMOUS_RACING_AGENT"
excerpt: "ML-Agents 자율 주행 경주차 에이전트 학습 시스템"
categories: [project]
permalink: /project/autonomous-racing-agent/
tags: [Machine Learning, ML-Agents, AI, Unity]
toc: true
toc_sticky: true
classes: wide
---

<style>
    /* Origin Styles Restoration */
    .pf-visual-frame {
        width: 100%; padding: 30px; background: rgba(0,0,0,0.03);
        border: 1px solid #e1e4e8; border-radius: 8px;
        margin: 25px 0; text-align: center;
    }
    .pf-arch-diagram {
        display: flex; flex-direction: column; gap: 15px; margin: 25px 0;
    }
    .pf-arch-layer {
        padding: 15px 20px; border: 1px solid #e1e4e8; border-radius: 6px;
        background: #f8f9fa; position: relative; text-align: center;
    }
    .pf-arch-layer::before {
        content: ""; position: absolute; left: 0; top: 0; bottom: 0; width: 4px;
        background: #007bff;
    }
    .pf-arch-layer-title {
        color: #007bff; font-weight: 700; margin-bottom: 8px;
        font-family: 'Fira Code', monospace; font-size: 0.9rem; text-align: center;
    }
    .pf-arch-layer-items {
        display: flex; flex-wrap: wrap; gap: 8px; margin-top: 8px; justify-content: center;
    }
    .pf-arch-item {
        padding: 5px 12px; background: rgba(0, 123, 255, 0.1);
        border: 1px solid #007bff; border-radius: 4px;
        font-size: 0.85rem; color: #333; text-align: center;
    }
</style>

Unity ML-Agents를 활용하여 정해진 트랙을 안정적으로 주행하는 자율 주행 에이전트를 학습시킨 프로젝트입니다.

---

# 1. 체크포인트 기반 네비게이션 시스템

초기 학습 단계에서는 에이전트들이 트랙을 역주행하거나 경로를 이탈하는 문제가 빈번했습니다. 이를 해결하기 위해 **순차적 체크포인트 검증 시스템**을 도입했습니다.

*   **동적 생성:** `SplineToCheckpointGenerator`를 통해 트랙의 스플라인 곡선을 따라 트리거를 자동 배치.
*   **순차 검증:** 현재 통과해야 할 인덱스와 일치할 때만 보상을 부여하고 다음 단계로 진행.

<div class="pf-visual-frame">
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">SplineToCheckpointGenerator</div>
            <div class="pf-arch-layer-items">
                <span class="pf-arch-item">스플라인 경로 분석</span>
                <span class="pf-arch-item">BoxCollider(Trigger) 자동 배치</span>
            </div>
        </div>
        <div style="text-align: center; color: #007bff; font-size: 1.5rem;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">SimcadeCarAgent_Auto</div>
            <div class="pf-arch-layer-items">
                <span class="pf-arch-item">체크포인트 통과 감지</span>
                <span class="pf-arch-item">순차적 인덱스 검증</span>
            </div>
        </div>
    </div>
</div>

```csharp
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player")) {
        CarProgress progress = other.GetComponentInParent<CarProgress>();
        
        if (progress.currentCheckPoint + 1 == checkPointIndex) {
            progress.currentCheckPoint = checkPointIndex;
            // 올바른 순서 통과 시 보상 부여
        }
    }
}
```

---

# 2. 다층적 보상 설계 (Reward Engineering)

에이전트의 효율적인 학습을 위해 보상 체계를 세 가지 계층으로 정교화했습니다.

### 2.1 목표 지향 보상 (Goal-Oriented)
*   **거리 기반 보상:** 다음 체크포인트와의 거리가 줄어들수록 매 프레임 보상.
*   **체크포인트 통과:** 올바른 순서로 통과 시 `+1.0`, Finish 라인 도달 시 `+10.0`.
*   **잘못된 경로:** 순서가 맞지 않는 체크포인트 통과 시 `-0.5` 페널티.

### 2.2 안전성 및 복구 보상 (Safety & Recovery)
*   **근접 페널티:** 레이캐스트가 벽을 감지할 때, 거리가 가까울수록 비선형적으로 페널티 급증.
*   **역주행 방지:** 차량의 진행 방향과 체크포인트 방향의 Dot Product를 계산하여 약 150도 이상 차이 날 경우 강력한 페널티.

```csharp
// 역주행 감지 로직 예시
float directionDot = Vector3.Dot(transform.forward, targetDirection);
if (directionDot < -0.866f && currentSpeed > 1.5f) {
    AddReward(wrongWayPenalty);
}
```

---

# 3. 레이캐스트 기반 환경 감지

에이전트는 주변 환경을 인식하기 위해 **총 36차원의 관측(Observation) 데이터**를 수집합니다.

*   **전방 감지:** 180도 범위 내 9개의 레이 (15m 거리).
*   **후방 감지:** 60도 범위 내 3개의 레이 (5m 거리, 후진 시 중요).
*   **데이터 정규화:** 모든 감지 거리를 0.0 ~ 1.0 범위로 변환하여 신경망 학습 안정성 확보.

<div class="pf-visual-frame">
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 10px;">
        <div style="padding: 10px; border: 1px solid #007bff; border-radius: 4px;">전속도/각속도 (7)</div>
        <div style="padding: 10px; border: 1px solid #007bff; border-radius: 4px;">레이캐스트 (21)</div>
        <div style="padding: 10px; border: 1px solid #007bff; border-radius: 4px;">목표 방향/거리 (4)</div>
        <div style="padding: 10px; border: 1px solid #007bff; border-radius: 4px;">입력 상태 (4)</div>
    </div>
</div>

---

# 4. 입력 스무딩 및 연속적 행동 결정

AI 특유의 급격한 지터링(Jittering)을 방지하고 실제 차량과 유사한 관성을 구현하기 위해 **Action Smoothing** 알고리즘을 적용했습니다.

```csharp
public override void OnActionReceived(ActionBuffers actions)
{
    float rawSteering = actions.ContinuousActions[0];
    
    // Action Smoothing: 직전 프레임 값과 비교하여 변화량 제한
    float steeringInput = Mathf.Clamp(rawSteering,
        lastSteeringInput - maxSteeringChange,
        lastSteeringInput + maxSteeringChange);
        
    lastSteeringInput = steeringInput;
    vehicleController.ProvideInputs(accelerationInput, steeringInput, 0f);
}
```

{: .notice--info}
**효과:** 이 보정 로직을 통해 에이전트는 더 이상 핸들을 급격하게 꺾지 않으며, 부드러운 주행 궤적을 그리며 코너를 통과하게 되었습니다.
