using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 에이전트의 개인의 순위와 관련된 정보들을 관리
/// 다음 포인트까지의 거리, 현재 순위, 현재 바퀴 처리, 골인여부 관리 준
/// </summary> <summary>
public class CarProgress : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public CinemachineCamera CinemaCamera { get; private set; } 
    [SerializeField] private TrackData trackData; // 현재 트랙 정보
    public TrackData CurrentTrackData => trackData;

    private int maxLab => trackData != null ? trackData.MaxLab : 1;
    private int checkPointCount => trackData != null ? trackData.CheckpointCount : 0;

    public Vector3 GetNextTargetPosition()
    {
        if (trackData == null) return transform.position;

        if (NextCheckpointIndex >= trackData.CheckpointCount)
        {
            Transform finish = trackData.GetFinishLine();
            return finish != null ? finish.position : transform.position;
        }

        Transform target = trackData.GetCheckpoint(NextCheckpointIndex);
        return target != null ? target.position : transform.position;
    }

    [Header("State")]
    public int CurLap { get; private set; } = 1;
    public int NextCheckpointIndex { get; private set; } = 0;
    public int CurRank { get; private set; } = 0; // 현재 순위 저장용
    public int FinalRank { get; private set; } = 0; // 도착시 확정된 순위 (0이면 아직 완주 X)
    public bool IsFinished { get; private set; } = false;
    public int CarID { get; private set; }

    [Header("Use Ranking")]
    public float CurToNextPointDistance { get; private set; } 
    public float TotalScore { get; private set; } // RaceManager가 정렬할 때 사용

    // 이벤트 추가 (에이전트 보상 연동용)
    public System.Action<int> OnCheckpointPassed; // checkpointIndex
    public System.Action OnWrongCheckpoint;
    public System.Action OnFinishPassed;
    public System.Action OnFallZoneEntered;

    // 태그 설정 (중앙 관리)
    private const string CHECKPOINT_TAG = "CheckPoint";
    private const string FINISH_TAG = "Finish";
    private const string FALLZONE_TAG = "FallZone";

    // 랭킹 설정
    public void SetRank(int rank) => CurRank = rank;
    
    // 전체 중 얼마만큼 이동했는지 비율
    public float GetArriveRatio() => Mathf.Clamp01(TotalScore / CalculateTotalScore(maxLab, checkPointCount, 0)); 
    
    // 진행률 점수 계산공식
    public float CalculateTotalScore(int labCount, int checkPointCount, float distance) 
    {
        return labCount * 100000 + checkPointCount * 100 - distance;
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize(TrackData data, int carID)
    {
        this.CinemaCamera = GetComponentInChildren<CinemachineCamera>();
        this.trackData = data;
        NextCheckpointIndex = 0;
        CurLap = 1;
        IsFinished = false;
        CurRank = 0;
        CurToNextPointDistance = 0;
        TotalScore = 0;
        CarID = carID;
    }

    public void Initialize()
    {
        NextCheckpointIndex = 0;
        CurLap = 1;
        IsFinished = false;
        CurRank = 0;
        CurToNextPointDistance = 0;
        TotalScore = 0;
    }

    /// <summary>
    /// /// 이동 점수, 이동 거리 설정
    /// </summary>    
    public void UpdateProgress()
    {
        if (IsFinished) { return; }
        
        Transform target = trackData.GetCheckpoint(NextCheckpointIndex);

        CurToNextPointDistance = Vector3.Distance(transform.position, target.position);
        TotalScore = CalculateTotalScore(CurLap, NextCheckpointIndex, CurToNextPointDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsFinished) return;

        if (other.CompareTag(CHECKPOINT_TAG))
        {
            HandleCheckpointTrigger(other.transform);
        }
        else if (other.CompareTag(FINISH_TAG))
        {
            HandleFinishTrigger(other.transform);
        }
        else if (other.CompareTag(FALLZONE_TAG))
        {
            OnFallZoneEntered?.Invoke();
        }
    }

    private void HandleCheckpointTrigger(Transform hitCheckpoint)
    {
        if (trackData == null) return;
        
        Transform expected = trackData.GetCheckpoint(NextCheckpointIndex);
        if (expected != null && (hitCheckpoint == expected || hitCheckpoint.IsChildOf(expected)))
        {
            int passedIdx = NextCheckpointIndex;
            CheckpointPassed(NextCheckpointIndex);
            OnCheckpointPassed?.Invoke(passedIdx);
        }
        else
        {
            OnWrongCheckpoint?.Invoke();
        }
    }

    private void HandleFinishTrigger(Transform hitFinish)
    {
        if (trackData == null) return;

        // 모든 체크포인트를 통과한 상태에서 피니시 라인 통과 시
        Transform finishLine = trackData.GetFinishLine();
        if (finishLine != null && (hitFinish == finishLine || hitFinish.IsChildOf(finishLine)) && NextCheckpointIndex >= trackData.CheckpointCount)
        {
            CheckpointPassed(NextCheckpointIndex);
            OnFinishPassed?.Invoke();
        }
    }

    /// <summary>
    /// 체크포인트 충돌 처리 (내부 상태 갱신)
    /// </summary>
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
                    Debug.Log($"{gameObject.name} Race Finished!");
                }
            }
        }
    }
}