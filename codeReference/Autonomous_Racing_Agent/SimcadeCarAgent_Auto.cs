using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Ashsvp;
using Unity.Cinemachine;

public class SimcadeCarAgent_Auto : Agent
{
    [Header("Sim-Cade Vehicle Components")]
    public SimcadeVehicleController vehicleController;
    public GearSystem gearSystem;
    public InputManager_SVP inputManager;

    [Header("Spawn System (Multi-Track)")]
    [Tooltip("학습에 사용할 모든 트랙의 SpawnPointManager 리스트 (랜덤 위치 선택)")]
    private TrackData currentTrack; // 현재 주행 중인 트랙 데이터

    [Header("Action Smoothing")]
    public float maxSteeringChange = 0.5f;
    public float maxAccelerationChange = 0.7f;

    // 도로 및 벽 감지를 위한 레이캐스트 설정
    [Header("Sensing (NEW)")]
    [Tooltip("도로를 감지할 레이어 ('Road' 태그가 있는 오브젝트의 레이어)")]
    public LayerMask roadLayer;
    [Tooltip("도로 감지 레이의 최대 거리")]
    public float roadRayDistance = 50f;

    [Tooltip("벽/장애물을 감지할 레이어 ('wall' 태그가 있는 오브젝트의 레이어)")]
    public LayerMask wallLayer;

    [Header("Front Sensing Config")]
    [Tooltip("전방 벽/장애물 근접 페널티를 적용할 최대 거리")]
    // 브레이크(5)가 매우 약하므로, 10m가 아닌 15m부터 미리 감지해야 함
    public float wallProximityDistance = 15f;
    [Tooltip("전방 감지 레이 개수 (홀수 추천)")]
    public int frontRayCount = 9;

    [Header("Rear Sensing Config (NEW)")]
    [Tooltip("후방 벽/장애물 근접 페널티를 적용할 최대 거리")]
    public float rearWallProximityDistance = 5f;
    [Tooltip("후방 감지 레이 개수 (홀수 추천)")]
    public int rearRayCount = 3;

    [Tooltip("Gizmos 그리기에 사용")]
    public bool drawDebugGizmos = true;


    [Header("Reward Settings (Autonomous)")]
    [Tooltip("도로('Road' 태그) 위에서 전진 속도로 받는 보상 계수 (목표 지향 시 줄이는 것 권장)")]
    public float forwardSpeedReward = 0.01f; 

    // "목표 지향" 보상 시스템
    [Header("Goal-Oriented Rewards (NEW)")]
    [Tooltip("다음 체크포인트에 가까워질 때 받는 보상 계수")]
    public float distanceRewardFactor = 0.1f;
    [Tooltip("방향 정렬(Dot Product) 보상 계수")]
    public float directionAlignmentRewardFactor = 0.01f;
    [Tooltip("체크포인트 통과 시 받는 보상")]
    public float checkpointReward = 1.0f;
    [Tooltip("잘못된 체크포인트 통과 시 페널티")]
    public float wrongCheckpointPenalty = -0.5f;
    [Tooltip("Finish 라인 통과 시 받는 최종 보상")]
    public float finishReward = 10.0f;

    [Header("Checkpoint Timeout (NEW)")]
    [Tooltip("한 체크포인트를 통과하는 데 허용되는 최대 시간 (초)")]
    public float maxCheckpointTime = 30f;
    private float timeSinceLastCheckpoint = 0f;

    [Header("Reward Settings (Survival)")]
    public float timePenalty = -0.001f;

    [Header("Safety Reward Settings")]
    // 드리프트 차량(DriftFactor 0.2)이므로, 페널티를 대폭 완화
    public float slipPenalty = -0.005f;
    [Tooltip("드리프트 허용 임계값")]
    // 드리프트를 허용하기 위해 임계값을 0.7에서 0.9로 상향
    public float slipThreshold = 0.9f;

    [Header("Proximity Penalty Settings (NEW)")]
    [Tooltip("전방 벽/장애물에 가까워질수록 받는 페널티 계수")]
    // 감지 거리가 늘어난 만큼, 페널티도 소폭 상향하여 경고를 강화
    public float proximityPenalty = -0.015f;
    [Tooltip("후방 벽/장애물에 가까워질수록 받는 페널티 계수 (후진 시)")]
    public float rearProximityPenalty = -0.02f; // 후방 페널티를 더 강하게 설정


    // 역주행 페널티 
    [Header("Wrong Way Driving Penalty (NEW)")]
    [Tooltip("역주행(잘못된 방향)으로 주행 시 받는 페널티 계수")]
    // 차가 미끄러져 스핀하기 쉬우므로, 역주행 페널티를 강화
    public float wrongWayPenalty = -0.2f;
    [Tooltip("역주행으로 간주하는 최소 속도 (이 속도 이상으로 역주행 시 페널티)")]
    // 차가 느리므로(MaxSpeed 70), 역주행 감지 속도도 낮춤
    public float wrongWaySpeedThreshold = 1.5f;
    [Tooltip("역주행으로 간주하는 각도 임계값 (Dot Product). -0.76(140도) ~ -0.94(160도) 사이 값. -1에 가까울수록 엄격함.")]
    // -0.2f(약 101도) -> -0.866f(약 150도)로 변경하여 더 엄격하게
    public float wrongWayDotThreshold = -0.866f;


    [Header("Recovery Reward Settings")]
    public float wallCollisionPenalty = -0.5f; // 충돌 시 페널티
    public float stuckPenaltyPerSecond = -0.5f;
    public float reverseReward = 0.01f;

    private bool isStuckInWall = false;
    private float stuckStartTime = 0f;

    private Rigidbody rb;
    private CarProgress carProgress;
    private float lastSteeringInput;
    private float lastAccelerationInput;

    // 차량 뒤집힘 상태
    private bool isVehicleFlipped = false;
    private float flippedStartTime = 0f;

    // 정지 상태 체크 시스템
    private float stagnationStartTime = 0f;
    private bool isStagnating = false;
    private const float STAGNATION_THRESHOLD = 2f;

    // 뒤집힘
    private const float FLIP_THRESHOLD = 5f; // 5초 이상 뒤집혀있으면 에피소드 종료
    private const float FLIP_ANGLE_THRESHOLD = 160f;

    // 체크포인트 시스템 변수
    private float lastDistanceToTarget;

    // 레이캐스트 방향 (Gizmos에서도 사용하기 위해 멤버 변수로 선언)
    private Vector3[] roadRayDirections;
    private Vector3[] wallRayDirections;
    private Vector3[] rearWallRayDirections;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        carProgress = GetComponent<CarProgress>();
        vehicleController = GetComponent<SimcadeVehicleController>();
        gearSystem = GetComponent<GearSystem>();
        inputManager = GetComponent<InputManager_SVP>();

        currentTrack = GameManager.Race.curTrackData;

        if (inputManager != null)
        {
            inputManager.enabled = false;
        }

        lastSteeringInput = 0f;
        lastAccelerationInput = 0f;

        // [수정] 레이캐스트 방향 세분화 (DivideByZero 버그 수정)
        roadRayDirections = new Vector3[frontRayCount];
        wallRayDirections = new Vector3[frontRayCount];
        if (frontRayCount == 1)
        {
            roadRayDirections[0] = Vector3.forward;
            wallRayDirections[0] = Vector3.forward;
        }
        else
        {
            float angleStep = 180f / (frontRayCount - 1);
            for (int i = 0; i < frontRayCount; i++)
            {
                float angle = -90f + (angleStep * i);
                roadRayDirections[i] = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                wallRayDirections[i] = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            }
        }

        // --- [추가] 후방 레이캐스트 생성 (DivideByZero 버그 수정) ---
        rearWallRayDirections = new Vector3[rearRayCount];
        if (rearRayCount == 1)
        {
            rearWallRayDirections[0] = Vector3.back;
        }
        else
        {
            float angleStep = 60f / (rearRayCount - 1);
            for (int i = 0; i < rearRayCount; i++)
            {
                float angle = -30f + (angleStep * i);
                rearWallRayDirections[i] = Quaternion.Euler(0, angle, 0) * Vector3.back;
            }
        }

        // --- [추가] CarProgress 이벤트 구독 (중앙화된 감지 로직 활용) ---
        if (carProgress != null)
        {
            carProgress.OnCheckpointPassed += (index) => 
            {
                AddReward(checkpointReward);
                Academy.Instance.StatsRecorder.Add("Reward/CheckpointReward", checkpointReward);
                timeSinceLastCheckpoint = 0f;
                lastDistanceToTarget = Vector3.Distance(transform.position, GetTargetPosition());
            };
            carProgress.OnWrongCheckpoint += () => 
            {
                AddReward(wrongCheckpointPenalty);
                Academy.Instance.StatsRecorder.Add("Reward/WrongCheckpointPenalty", wrongCheckpointPenalty);
            };
            carProgress.OnFinishPassed += () => 
            {
                AddReward(finishReward);
                Academy.Instance.StatsRecorder.Add("Reward/FinishReward", finishReward);
                EndEpisode();
            };
            carProgress.OnFallZoneEntered += () => 
            {
                Debug.Log("Fell off map - ending episode with penalty.");
                AddReward(-5.0f);
                EndEpisode();
            };
        }
    }

    public override void OnEpisodeBegin()
    {
        // carProgress에서 현재 맵을 가져오고, 없으면 GameManager에서 가져옵니다.
        currentTrack = carProgress != null && carProgress.CurrentTrackData != null ? carProgress.CurrentTrackData : GameManager.Race.curTrackData;
        if (currentTrack != null)
        {
            Transform spawnTransform = currentTrack.GetStartPoint(Random.Range(0, currentTrack.StartPointsCount));  // 랜덤 스폰 위치 선택
            transform.position = spawnTransform.position;
            transform.rotation = spawnTransform.rotation;
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        lastSteeringInput = 0f;
        lastAccelerationInput = 0f;
        isVehicleFlipped = false;
        flippedStartTime = 0f;
        isStagnating = false;
        stagnationStartTime = 0f;
        if (gearSystem != null) { gearSystem.currentGear = 1; }
        isStuckInWall = false;
        stuckStartTime = 0f;

        // 체크포인트 시스템 리셋 
        timeSinceLastCheckpoint = 0f; 

        carProgress.Initialize();
        
        // 초기 거리 계산
        lastDistanceToTarget = Vector3.Distance(transform.position, GetTargetPosition());
    }

    // [변경 3] 목표 지점 계산 로직을 CarProgress 기반으로 변경
    private Vector3 GetTargetPosition()
    {
        if (currentTrack == null || carProgress == null) return transform.position;

        // 모든 체크포인트를 통과했으면 피니시 라인 반환
        if (carProgress.NextCheckpointIndex >= currentTrack.CheckpointCount)
        {
            return currentTrack.GetFinishLine().position;
        }

        // 다음 체크포인트 위치 반환
        Transform targetParam = currentTrack.GetCheckpoint(carProgress.NextCheckpointIndex);
        return targetParam != null ? targetParam.position : transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 기본 관측 데이터 (10)
        sensor.AddObservation(transform.InverseTransformDirection(rb.linearVelocity)); // 3
        sensor.AddObservation(transform.InverseTransformDirection(rb.angularVelocity)); // 3
        float normalizedSpeed = rb.linearVelocity.magnitude / vehicleController.MaxSpeed;
        sensor.AddObservation(normalizedSpeed); // 1
        sensor.AddObservation(vehicleController.vehicleIsGrounded ? 1 : 0); // 1
        sensor.AddObservation(lastSteeringInput); // 1
        sensor.AddObservation(lastAccelerationInput); // 1

        // 상태 관측 (1)
        sensor.AddObservation(isStuckInWall ? 1 : 0); // 1 (벽에 막혔는지?)

        // 레이캐스트 관측 (frontRayCount + frontRayCount + rearRayCount)
        AddRaycastObservations(sensor, roadRayDirections, roadLayer, roadRayDistance);
        AddRaycastObservations(sensor, wallRayDirections, wallLayer, wallProximityDistance);
        AddRaycastObservations(sensor, rearWallRayDirections, wallLayer, rearWallProximityDistance);

        // 목표 지향 관측 (4)
        Vector3 targetPosition = GetTargetPosition();
        Vector3 targetDirLocal = transform.InverseTransformPoint(targetPosition);

        sensor.AddObservation(targetDirLocal.normalized);                  // 3 (목표 방향)
        sensor.AddObservation(targetDirLocal.magnitude / roadRayDistance); // 1 (목표 거리, 정규화)

        // (총 관측: 10 + 1 + frontRayCount*2 + rearRayCount + 4)        
    }

    private void AddRaycastObservations(VectorSensor sensor, Vector3[] directions, LayerMask layer, float distance)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 worldRayDir = transform.TransformDirection(directions[i]);
            if (Physics.Raycast(transform.position, worldRayDir, out RaycastHit hit, distance, layer))
            {
                sensor.AddObservation(hit.distance / distance);
            }
            else
            {
                sensor.AddObservation(1f); 
            }
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // 액션 스무딩 제거: AI의 의도가 즉각적으로 반영되도록 수정
        float steeringInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float accelerationInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        lastSteeringInput = steeringInput;
        lastAccelerationInput = accelerationInput;

        vehicleController.ProvideInputs(accelerationInput, steeringInput, 0f);
        
        CalculateRewards();
        CheckEpisodeEndConditions();

        // 순위 시스템 정보 업데이트
        GetComponent<CarProgress>().UpdateProgress();
    }

    private void CalculateRewards()
    {
        if (vehicleController == null) return;
        
        // --- [NEW] 기본 시간 페널티 및 로깅 ---
        AddReward(timePenalty);
        Academy.Instance.StatsRecorder.Add("Reward/StepPenalty", timePenalty);

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (forwardSpeed > 0.5f && !isStuckInWall)
        {
            float speedReward = forwardSpeedReward * (forwardSpeed / vehicleController.MaxSpeed);
            AddReward(speedReward);
            Academy.Instance.StatsRecorder.Add("Reward/ForwardSpeedReward", speedReward);
        }

        Vector3 targetPosition = GetTargetPosition();
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition); 

        if (targetPosition != transform.position)
        {
            // --- [NEW] 네비게이션: 방향 정렬 보상 ---
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            float directionDot = Vector3.Dot(transform.forward, targetDirection);
            
            float alignmentReward = directionDot * directionAlignmentRewardFactor;
            AddReward(alignmentReward);
            Academy.Instance.StatsRecorder.Add("Reward/AlignmentReward", alignmentReward);

            // --- [NEW] 체크포인트 타임아웃 페널티 ---
            timeSinceLastCheckpoint += Time.fixedDeltaTime;
            if (timeSinceLastCheckpoint > maxCheckpointTime)
            {
                AddReward(-1.0f);
                Academy.Instance.StatsRecorder.Add("Reward/CheckpointTimeout", -1.0f);
                Debug.Log($"Checkpoint Timeout: {timeSinceLastCheckpoint:F1}s - Ending Episode");
                EndEpisode();
                return;
            }

            float distanceChange = lastDistanceToTarget - distanceToTarget;

            // 1. 통합 거리 기반 보상 (전진/후진 공통 적용)
            // 후진으로 가까워지는 것은 보상 효율을 낮추어(0.5배) 전진을 유도합니다.
            float weight = (forwardSpeed > 0.1f) ? 1.0f : 0.5f;
            float distanceReward = distanceChange * distanceRewardFactor * weight;
            AddReward(distanceReward);
            Academy.Instance.StatsRecorder.Add("Reward/DistanceReward", distanceReward);

            // 2. 추가 상태별 보상/패널티
            if (forwardSpeed < -0.1f) // 후진 중
            {
                if (isStuckInWall)
                {
                    ApplyProximityPenalty(wallRayDirections, wallLayer, wallProximityDistance, proximityPenalty);                    
                    Academy.Instance.StatsRecorder.Add("Reward/ReverseReward", reverseReward);
                }
                else
                {
                    AddReward(-0.005f); // 일반적인 후진은 미세 감점
                }
            }
            else if (Mathf.Abs(forwardSpeed) <= 0.1f) // 정지 중
            {
                CheckStagnation();
            }

            lastDistanceToTarget = distanceToTarget; 
        }

        // 이미 벽에 부딪혀 있는 상태(isStuckInWall)가 아닐 때만 근접 경고 패널티를 줍니다.
        if (!isStuckInWall)
        {
            ApplyProximityPenalty(wallRayDirections, wallLayer, wallProximityDistance, proximityPenalty);

            if (forwardSpeed < -0.1f) 
            {
                ApplyProximityPenalty(rearWallRayDirections, wallLayer, rearWallProximityDistance, rearProximityPenalty);
            }
        }

        if (isStuckInWall)
        {
            if (Time.time - stuckStartTime > 1f)
            {
                float sPenalty = stuckPenaltyPerSecond * Time.fixedDeltaTime;
                AddReward(sPenalty);
                Academy.Instance.StatsRecorder.Add("Reward/StuckPenalty", sPenalty);
            }
            if (Time.time - stuckStartTime > 10f)
            {
                AddReward(-5.0f);
                Academy.Instance.StatsRecorder.Add("Reward/StuckTimeout", -5.0f);
                Debug.Log($"Stuck in wall for {Time.time - stuckStartTime:F1}s - ending episode");
                EndEpisode();
            }
        }

        float avgSlipCoeff = 0f;
        for (int i = 0; i < vehicleController.slipCoeff.Length; i++)
        {
            avgSlipCoeff += vehicleController.slipCoeff[i];
        }
        avgSlipCoeff /= vehicleController.slipCoeff.Length;

        if (avgSlipCoeff > slipThreshold)
        {
            float sPenalty = slipPenalty * avgSlipCoeff;
            AddReward(sPenalty);
            Academy.Instance.StatsRecorder.Add("Reward/SlipPenalty", sPenalty);
        }

        CheckVehicleFlip();
    }

    // 근접 페널티를 위한 헬퍼 함수
    private void ApplyProximityPenalty(Vector3[] directions, LayerMask layer, float distance, float penaltyCoefficient)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 worldRayDir = transform.TransformDirection(directions[i]);
            if (Physics.Raycast(transform.position, worldRayDir, out RaycastHit hit, distance, layer))
            {
                // 거리가 가까울수록 더 큰 페널티 (0에 가까울수록 페널티 급증)
                float penalty = penaltyCoefficient * (1f - (hit.distance / distance));
                float scaledPenalty = penalty * Time.fixedDeltaTime;
                AddReward(scaledPenalty); // FixedUpdate 주기에 맞춰 스케일링
                Academy.Instance.StatsRecorder.Add("Reward/ProximityPenalty", scaledPenalty);
            }
        }
    }


    private void CheckStagnation()
    { 
        if (isStuckInWall) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float currentTime = Time.time;
        if (currentSpeed < 0.5f)
        {
            if (!isStagnating)
            {
                isStagnating = true;
                stagnationStartTime = currentTime;
            }
            else
            {
                float stagnationDuration = currentTime - stagnationStartTime;
                if (stagnationDuration > STAGNATION_THRESHOLD)
                {
                    AddReward(-0.02f * (stagnationDuration / STAGNATION_THRESHOLD));
                }
                if (stagnationDuration > 5f)
                {
                    AddReward(-5.0f);
                    Debug.Log($"Stagnation detected for {stagnationDuration:F1}s - ending episode");
                    EndEpisode();
                }
            }
        }
        else
        {
            isStagnating = false;
        }
    }

    private void CheckVehicleFlip()
    {
        float currentTime = Time.time;
        float angleFromUp = Vector3.Angle(transform.up, Vector3.up);
        if (angleFromUp > FLIP_ANGLE_THRESHOLD)
        {
            if (!isVehicleFlipped)
            {
                isVehicleFlipped = true;
                flippedStartTime = currentTime;
                Debug.Log($"Vehicle flipped detected! Angle: {angleFromUp:F1}도");
            }
            else
            {
                float flippedDuration = currentTime - flippedStartTime;
                if (flippedDuration > STAGNATION_THRESHOLD)
                {
                    AddReward(-0.05f); 
                }
                if (flippedDuration > FLIP_THRESHOLD)
                {
                    AddReward(-3.0f); 
                    Debug.Log($"Vehicle flipped for {flippedDuration:F1}s - ending episode");
                    EndEpisode();
                }
            }
        }
        else
        {
            if (isVehicleFlipped)
            {
                isVehicleFlipped = false;
                Debug.Log("Vehicle returned to normal orientation.");
            }
        }
    }

    private void CheckEpisodeEndConditions()
    {
        if (rb.linearVelocity.magnitude < 0.1f && !isStuckInWall)
        {
            AddReward(-0.001f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // (복구용 코드의 일부)
        if (((1 << collision.gameObject.layer) & wallLayer) != 0) // wallLayer와 충돌했는지 확인
        {
            AddReward(wallCollisionPenalty);
            Debug.Log("Wall collision - Penalty applied.");

            isStuckInWall = true;
            stuckStartTime = Time.time;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // (복구용 코드의 일부)
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            if (isStuckInWall)
            {
                isStuckInWall = false;
                Debug.Log("Escaped from wall.");
            }
        }
    }

    // 레이캐스트 Gizmos를 위한 헬퍼 함수
    private void DrawRaycastGizmos(Vector3[] directions, LayerMask layer, float distance, Color hitColor, Color missColor)
    {
        if (!Application.isPlaying) return; // 실행 중에만

        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 worldRayDir = transform.TransformDirection(directions[i]);
            if (Physics.Raycast(transform.position, worldRayDir, out RaycastHit hit, distance, layer))
            {
                Gizmos.color = hitColor;
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Gizmos.color = missColor;
                Gizmos.DrawRay(transform.position, worldRayDir * distance);
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos) return;

        if (rb != null)
        {
            Gizmos.color = Color.blue;
            Vector3 velocityDirection = rb.linearVelocity.normalized;
            if (velocityDirection.magnitude > 0.1f)
            {
                Gizmos.DrawRay(transform.position, velocityDirection * 3f);
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // --- [수정] Gizmos 헬퍼 함수 사용 ---
        DrawRaycastGizmos(roadRayDirections, roadLayer, roadRayDistance, Color.green, Color.gray);
        DrawRaycastGizmos(wallRayDirections, wallLayer, wallProximityDistance, Color.red, Color.yellow);
        DrawRaycastGizmos(rearWallRayDirections, wallLayer, rearWallProximityDistance, Color.magenta, Color.cyan);
      
        if (Application.isPlaying && currentTrack != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 targetPos = GetTargetPosition();
            Gizmos.DrawLine(transform.position, targetPos);
            Gizmos.DrawSphere(targetPos, 1.0f);
        }
    }
}