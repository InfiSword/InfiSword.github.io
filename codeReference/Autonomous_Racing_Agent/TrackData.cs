using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 해당 트랙에 대한 정보
/// 차량 시작 위치들, 중간 지점들, 마지막 골인 지점 
/// </summary> <summary>
public class TrackData : MonoBehaviour
{
    [Header("Track Information")]
    [SerializeField] private Transform[] checkpoints; 
    [SerializeField] private Transform[] startPoints;
    [SerializeField] private Transform finishLine;
    [SerializeField] private int maxLab;

    // 편의 기능: 체크포인트 개수 반환
    public int CheckpointCount => checkpoints.Length;
    public int StartPointsCount => startPoints.Length;
    public int MaxLab 
    { 
        get 
        { 
            if(maxLab == 0) { Debug.LogError("MaxLab의 설정을 확인해주세요."); } 
            return maxLab; 
        } 
    }

    public void SetCheckPoints(Transform[] newCheckPoints) => checkpoints = newCheckPoints;
    public void SetStartPoints(Transform[] newStartPoints) => startPoints = newStartPoints;
    public void SetFinishLine(Transform newFinishLine) => finishLine = newFinishLine;   // TODO 
    public void SetMaxLab(int maxLab) => this.maxLab = maxLab;

    // 인덱스로 체크포인트 가져오기
    public Transform GetCheckpoint(int index)
    {
        if (checkpoints.Length == 0) 
        {
             Debug.LogError("checkpoints의 설정을 확인해주세요."); 
             return null; 
        }
       
        // 인덱스가 범위를 넘어가면 피니시 라인
        if (index >= checkpoints.Length){ return finishLine; }
        
        return checkpoints[index];
    }

    // 인덱스로 시작포인트 가져오기
    public Transform GetStartPoint(int index)
    {
        if (startPoints.Length == 0 || index >= startPoints.Length)
         {
            Debug.LogError("startPoints의 설정을 확인해주세요."); 
            return null; 
         }
        
        return startPoints[index];
    }

    // 마지막포인트 가져오기
    public Transform GetFinishLine()
    {
        if (finishLine == null)
        {
            Debug.LogError("finishLine의 설정을 확인해주세요."); 
            return null; 
        }

        return finishLine;
    }


    // Gizmo 그리기 (시각화 용도)
    private void OnDrawGizmos()
    {
        if (checkpoints == null) { return; }
        if (startPoints == null) { return; }
        
        Gizmos.color = Color.yellow;
        foreach (var cp in checkpoints)
        {
            if (cp != null) Gizmos.DrawWireSphere(cp.position, 1f);
        }

        Gizmos.color = Color.red;
        foreach (var sp in startPoints)
        {
            if (sp != null) Gizmos.DrawSphere(sp.position, 0.5f);
        }
    }

}