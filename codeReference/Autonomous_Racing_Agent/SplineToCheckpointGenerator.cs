using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor; // 저장 및 Undo 기능을 위해 필수
#endif
// Unity 에디터에서 Generate Checkpoints 버튼을 눌러
// trakData의 StartPoints와 Checkpoints 배열 및 finishLine을 자동으로 채워주는 툴
[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(TrackData))]
public class SplineToCheckpointGenerator : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private TrackData trackData;

    [Header("설정")]
    [SerializeField] private int totalCheckpoints = 30;
    [SerializeField] private string rootObjectName = "Generated_Checkpoints";

    [Header("Checkpoint Settings (NEW)")]
    [Tooltip("자동 생성될 체크포인트 트리거의 크기")]
    public Vector3 checkpointColliderSize = new Vector3(15f, 5f, 1f);
    [Tooltip("자동 생성될 체크포인트의 태그")]
    [SerializeField] private string checkpointTag = "Checkpoint";
    [Tooltip("자동 생성될 Finish 라인의 태그")]
    [SerializeField] private string finishTag = "Finish";

    [Header("Start Grid Settings (NEW)")]
    [Tooltip("생성할 스타팅 그리드(출발선)의 개수")]
    [SerializeField] private int numberOfStartPositions = 5;
    [Tooltip("그리드 간의 가로 간격 (미터)")]
    [SerializeField] private float startPositionSpacing = 3f;
    [Tooltip("스타팅 그리드를 담을 부모 오브젝트의 이름")]
    [SerializeField] private string startGridRootName = "Generated_StartGrid";

    [ContextMenu("1. Generate Checkpoints from Spline")]
    public void GenerateCheckpoints()
    {
        if (spline == null) spline = GetComponent<SplineContainer>();
        if (trackData == null) trackData = GetComponent<TrackData>();
        if (spline == null || trackData == null)
        {
            Debug.LogError("SplineContainer 또는 TrackData가 없습니다!");
            return;
        }

        // 기존에 생성된 체크포인트 루트가 있다면 삭제
        Transform existingRoot = transform.Find(rootObjectName);
        if (existingRoot != null)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(existingRoot.gameObject);
#else
            DestroyImmediate(existingRoot.gameObject);
#endif
        }

        // 체크포인트들을 담을 새 부모 오브젝트 생성
        GameObject checkpointRoot = new GameObject(rootObjectName);
        checkpointRoot.transform.SetParent(this.transform);
        checkpointRoot.transform.localPosition = Vector3.zero;
        checkpointRoot.transform.localRotation = Quaternion.identity;

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(checkpointRoot, "Create Checkpoint Root");
#endif
        // 3. [중요] TrackData 변경 알림 (이게 없으면 리스트가 날아갑니다)
#if UNITY_EDITOR
        Undo.RecordObject(trackData, "Update TrackData Checkpoints");
#endif
        // trackDada 배열 초기화
        Transform[] checkPointsTemp = new Transform[totalCheckpoints];

        Debug.Log($"[{totalCheckpoints}]개의 체크포인트 생성을 시작합니다...");

        for (int i = 0; i < totalCheckpoints; i++)
        {
            // 스플라인을 따라 위치 계산 (첫 번째(i=0)는 1/N 지점, 마지막(i=N-1)은 N/N 지점)
            float progress = (float)(i + 1) / totalCheckpoints;

            // 스플라인에서 위치(pos), 방향(tangent), 위쪽(up) 정보 추출
            spline.Spline.Evaluate(progress, out float3 pos, out float3 tan, out float3 up);

            if (math.lengthsq(tan) == 0) tan = math.forward(); // 방향이 0일 경우 앞쪽으로 강제

            // 체크포인트 GameObject 생성
            GameObject checkpointObj = new GameObject($"Checkpoint_{i:00}");
            checkpointObj.transform.SetParent(checkpointRoot.transform);

            // 월드 좌표인 .position 대신, 부모 기준의 .localPosition을 사용합니다.
            checkpointObj.transform.localPosition = (Vector3)pos;
            // 월드 회전인 .rotation 대신, 부모 기준의 .localRotation을 사용합니다.
            checkpointObj.transform.localRotation = Quaternion.LookRotation((Vector3)tan, (Vector3)up);


#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(checkpointObj, "Create Checkpoint Node");
#endif

            // BoxCollider(Trigger)와 Tag 자동 추가
            BoxCollider collider = checkpointObj.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = checkpointColliderSize;
            checkpointObj.tag = checkpointTag;

            // 체크포인트
            CheckPoint cpScript = checkpointObj.AddComponent<CheckPoint>();
            cpScript.Init(i); // 단순히 자기 번호만 부여

            // [변경] TrackData에 등록
            checkPointsTemp[i] = checkpointObj.transform;
        }

        // trackData에 연결
        if (checkPointsTemp.Length > 0)
        {
            checkPointsTemp[checkPointsTemp.Length - 1].gameObject.tag = finishTag;
            trackData.SetCheckPoints(checkPointsTemp);
            trackData.SetFinishLine(checkPointsTemp[checkPointsTemp.Length - 1]);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(trackData);
#endif
        Debug.Log("체크포인트 생성 완료 및 TrackData에 등록됨.");
    }

    [ContextMenu("2. Generate Start Grid from Spline Start")]
    public void GenerateStartpoints()
    {
        if (spline == null) spline = GetComponent<SplineContainer>();
        if (trackData == null) trackData = GetComponent<TrackData>();
        if (spline == null || trackData == null) return;

        // 기존 그리드 루트 삭제
        Transform existingRoot = transform.Find(startGridRootName);
        if (existingRoot != null)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(existingRoot.gameObject);
#else
            DestroyImmediate(existingRoot.gameObject);
#endif
        }

        // 새 그리드 루트 생성
        GameObject startGridRoot = new GameObject(startGridRootName);
        startGridRoot.transform.SetParent(this.transform);
        startGridRoot.transform.localPosition = Vector3.zero;
        startGridRoot.transform.localRotation = Quaternion.identity;
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(startGridRoot, "Create StartGrid Root");
#endif
#if UNITY_EDITOR
        Undo.RecordObject(trackData, "Update TrackData StartPoints");
#endif

        // 임시 저장 배열
        Transform[] startPointsTemp = new Transform[numberOfStartPositions];

        // 스플라인의 0% 지점 계산
        spline.Spline.Evaluate(0f, out float3 pos, out float3 tan, out float3 up);
        if (math.lengthsq(tan) == 0) tan = math.forward();

        // 스플라인의 "오른쪽" 방향 벡터 계산
        float3 right = math.normalize(math.cross(up, tan));

        // --- [설정값] ---
        int columnsPerRow = 4; // 한 줄에 4대씩 배치 (4열)
        float rowSpacing = startPositionSpacing; // 앞뒤 간격
        float colSpacing = startPositionSpacing; // 좌우 간격 (필요시 * 1.5f 등 조절)

        Debug.Log($"[{numberOfStartPositions}]개의 스타팅 그리드를 {columnsPerRow}열로 생성합니다...");

        for (int i = 0; i < numberOfStartPositions; i++)
        {
            GameObject startObj = new GameObject($"StartPoint_{i:00}");
            startObj.transform.SetParent(startGridRoot.transform);

            // --- [4열 배치 로직] ---

            // 1. 행(Row)과 열(Col) 인덱스 계산
            // row: 0,0,0,0, 1,1,1,1 ... (4명마다 다음 줄로)
            int row = i / columnsPerRow;
            // col: 0,1,2,3, 0,1,2,3 ... (가로 위치)
            int col = i % columnsPerRow;

            // 2. 앞뒤 위치 (뒤로 줄을 서야 하므로 -tan 방향)
            Vector3 forwardPos = (Vector3)tan * (row * -rowSpacing);

            // 3. 좌우 위치 (트랙 중앙 정렬)
            // 4명일 때 중앙 기준 오프셋: -1.5, -0.5, +0.5, +1.5
            // 공식: (현재열 - (전체열-1)/2) * 간격
            float centerOffset = (col - (columnsPerRow - 1) / 2.0f);
            Vector3 sidePos = (Vector3)right * (centerOffset * colSpacing);

            // 최종 위치 합성
            Vector3 localPos = (Vector3)pos + forwardPos + sidePos;

            startObj.transform.localPosition = localPos;
            startObj.transform.localRotation = Quaternion.LookRotation((Vector3)tan, (Vector3)up);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(startObj, "Create StartPoint Node");
#endif
            startPointsTemp[i] = startObj.transform;
        }

        // trackData에 연결
        if (startPointsTemp.Length > 0)
        {
            trackData.SetStartPoints(startPointsTemp);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(trackData);
#endif
        Debug.Log("4열 스타팅 그리드 생성이 완료되었습니다!");
    }
}