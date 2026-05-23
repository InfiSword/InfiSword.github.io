layout: single

title: "PROJECT REPORT // AUTONOMOUS\_RACING\_AGENT"

excerpt: "ML-Agents 자율 주행 경주차 에이전트 학습 시스템"

categories: \[Project]

tags: \[Machine Learning, ML-Agents, AI, Unity]

toc: true

toc\_sticky: true



Unity ML-Agents를 활용하여 정해진 트랙을 안정적으로 주행하는 자율 주행 에이전트를 학습시켰습니다.



1\. 체크포인트 기반 네비게이션 시스템



초기 학습에서 발생한 역주행 현상을 해결하기 위해, 체크포인트를 순차적으로 통과해야만 하는 시스템을 설계했습니다.

SplineToCheckpointGenerator를 활용해 트랙 곡선을 따라 트리거(Trigger)를 자동 생성하도록 하였습니다.



2\. 다층적 보상 설계 (Reward Engineering)



에이전트가 트랙을 올바르고 안전하게 주행하도록 세 가지 계층으로 보상을 설계했습니다.



목표 지향 보상 (Goal-Oriented)



거리 기반 보상: 다음 체크포인트에 가까워질수록 보상



체크포인트/피니시 라인: 올바른 순서 통과 시 +1.0, 피니시 달성 시 +10.0



우회 방지: 잘못된 체크포인트 통과 시 -0.5 페널티



안전성 및 복구 보상 (Safety \& Recovery)



\[Header("Proximity Penalty Settings")]

public float proximityPenalty = -0.015f;      // 전방 벽 근접 페널티 계수

public float wrongWayPenalty = -0.2f;         // 역주행 페널티 계수





근접 페널티: 레이캐스트가 벽을 감지할 때, 거리가 가까울수록 비선형적으로 급증하는 페널티 부여



역주행 방지: 목표 방향과 차량 방향의 Dot Product가 약 150도 이상 차이나고 가속 중일 때 페널티



3\. 레이캐스트 기반 환경 감지



에이전트 주변 환경을 인식하기 위해 총 36차원의 관측(Observation) 데이터를 수집합니다.

전방 9개(180도), 후방 3개(60도)의 레이를 방사하여 도로와 벽의 거리를 0.0\~1.0으로 정규화하여 신경망에 전달합니다.



public override void CollectObservations(VectorSensor sensor)

{

&#x20;   // 10차원: 속도, 지면접촉, 이전 조향/가속

&#x20;   // 1차원: 벽 막힘 상태

&#x20;   // 21차원: 다방향 레이캐스트 센서 데이터 (전방 도로/벽, 후방 벽)

&#x20;   // 4차원: 목표 체크포인트 방향 및 거리

&#x20;   // 총 36차원 관측 데이터 수집

}





4\. 입력 스무딩 및 연속적 행동 결정



AI 모델은 불연속적 이동이 아닌, -1.0 \~ 1.0 사이의 연속적인 조향(Steering)과 가속(Acceleration) 실수 값을 출력합니다.

이때 발생하는 AI 특유의 급격한 지터링(Jittering)을 막기 위해 입력 보정 알고리즘을 추가했습니다.



💡 Action Smoothing

직전 프레임의 최종 제어값과 현재 신경망 출력값의 차이를 계산하여, 프레임당 최대 변화량(maxSteeringChange) 이내로 값을 보정(Clamp)함으로써 실제 차량과 유사한 관성을 구현했습니다.

{: .notice--info }



5\. 학습 설정 및 다중 트랙 지원



알고리즘: PPO (Proximal Policy Optimization) 활용, 1천만 Step 학습



Curiosity Signal 활성화: 에이전트의 다양한 행동 패턴과 탐험 유도



다중 트랙: 에피소드 시작 시 다양한 트랙과 랜덤 스폰 위치를 지정해 특정 맵에 대한 과적합(Overfitting) 방지

