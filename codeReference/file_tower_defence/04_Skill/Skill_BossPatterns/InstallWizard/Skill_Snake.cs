using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_Snake : Skill_BossMain
{
    private enum SnakeState { Ready, Play, Destroyed }
    private SnakeState currentState = SnakeState.Ready;

    [Header("Components")]
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject bodyPrefab;

    [Header("Settings")]
    [SerializeField] private float moveTick = 0.5f;
    [SerializeField] private float phaseMaxDamage = 500f;
    [SerializeField] private float maxPatternDuration = 60f;

    private List<Vector2Int> segments = new List<Vector2Int>();
    private List<GameObject> segmentObjects = new List<GameObject>();
    private Vector2Int currentDirection = Vector2Int.right;
    private float phaseAccumulatedDamage = 0f;
    private float elapsedTime = 0f;

    private File_Base currentTargetFile;

    private Coroutine stateCoroutine;

    public override void Init(Unit_VirusBase owner)
    {
        base.Init(owner);
    }

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        phaseAccumulatedDamage = 0f;
        elapsedTime = 0f;
        
        // 초기 방향 설정
        currentDirection = Vector2Int.right;
        
        SetState(SnakeState.Ready);
    }

    private void SetState(SnakeState newState)
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }

        currentState = newState;
        switch (currentState)
        {
            case SnakeState.Ready:
                stateCoroutine = StartCoroutine(SequenceReady());
                break;
            case SnakeState.Play:
                stateCoroutine = StartCoroutine(SnakeMoveLoop());
                break;
            case SnakeState.Destroyed:
                FinishPattern();
                Cleanup();
                break;
        }
    }

    private IEnumerator SequenceReady()
    {
        // Initial setup: 1 Head + 2 Body
        segments.Clear();
        segments.Add(new Vector2Int(2, 2)); // Head
        segments.Add(new Vector2Int(1, 2)); // Body 1
        segments.Add(new Vector2Int(0, 2)); // Body 2
        
        CreateSegmentObjects();

        yield return new WaitForSeconds(1.0f);
        SetState(SnakeState.Play);
    }

    private void Update()
    {
        if (isRunning && currentState == SnakeState.Play)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxPatternDuration)
            {
                Debug.LogWarning("[Skill_Snake] Timeout reached. Forcing pattern end.");
                SetState(SnakeState.Destroyed);
            }
        }
    }

    private void CreateSegmentObjects()
    {
        foreach (var obj in segmentObjects)
        {
            if (obj != null) Managers.Resource.Destroy(obj);
        }
        segmentObjects.Clear();

        if (headPrefab == null || bodyPrefab == null)
        {
            Debug.LogError("[Skill_Snake] Head or Body Prefab is NOT assigned in the Inspector!");
            SetState(SnakeState.Destroyed);
            return;
        }

        for (int i = 0; i < segments.Count; i++)
        {
            GameObject prefab = (i == 0) ? headPrefab : bodyPrefab;
            // 프리팹이 Prefabs/Virus/ 폴더에 있으므로 경로 수정
            string resourcePath = $"Virus/{prefab.name}";
            GameObject go = Managers.Resource.Instantiate(resourcePath, transform);

            if (go == null)
            {
                Debug.LogError($"[Skill_Snake] Failed to load prefab : {resourcePath}");
                continue;
            }

            UpdateObjectPosition(go, segments[i]);
            segmentObjects.Add(go);

            if (i == 0)
            {
                go.tag = "Virus";
                InstallWizard_SnakeHead head = go.GetComponent<InstallWizard_SnakeHead>();
                if (head == null) head = go.AddComponent<InstallWizard_SnakeHead>();
                head.Init(this);
            }
        }
    }

    private void AddBodySegment(Vector2Int position)
    {
        if (bodyPrefab == null) return;

        segments.Add(position);
        string resourcePath = $"Virus/{bodyPrefab.name}";
        GameObject go = Managers.Resource.Instantiate(resourcePath, transform);
        if (go != null)
        {
            UpdateObjectPosition(go, position);
            segmentObjects.Add(go);
        }
    }

    private void UpdateObjectPosition(GameObject go, Vector2Int gridPos)
    {
        if (go == null) return; // NullReferenceException 방지

        FileGrid grid = Managers.GridMgr.gridArray[gridPos.x, gridPos.y];
        if (grid != null)
        {
            go.transform.position = grid.transform.position;
        }
    }

    private IEnumerator SnakeMoveLoop()
    {
        while (isRunning && currentState != SnakeState.Destroyed)
        {
            try
            {
                MoveTick();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Skill_Snake] MoveTick Error: {e.Message}\n{e.StackTrace}");
            }
            yield return new WaitForSeconds(moveTick);
        }
    }

    private void MoveTick()
    {
        UpdateTarget();

        DecideDirection();

        Vector2Int newHeadPos = segments[0] + currentDirection;

        // 그리드 경계를 벗어나려 하면 방향 전환 시도
        if (!Managers.GridMgr.IsValidGridIndex(newHeadPos.x, newHeadPos.y))
        {
            if (!TryTurn(ref newHeadPos))
            {
                // 완전히 갇힌 경우 (이론상 불가능하지만 방어적 처리)
                Wander();
                newHeadPos = segments[0] + currentDirection;
                if (!Managers.GridMgr.IsValidGridIndex(newHeadPos.x, newHeadPos.y))
                    return; 
            }
        }

        // 몸통 이동
        Vector2Int lastPos = segments[segments.Count - 1];
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i] = segments[i - 1];
            UpdateObjectPosition(segmentObjects[i], segments[i]);
        }

        // 머리 이동
        segments[0] = newHeadPos;
        UpdateObjectPosition(segmentObjects[0], segments[0]);
        
        // 상호작용 체크
        CheckFileInteraction(newHeadPos, lastPos);
    }

    private void UpdateTarget()
    {
        File_Base nearestFile = null;
        float minDistance = float.MaxValue;

        foreach (var file in Managers.GridMgr.ActiveFiles)
        {
            // 드래그 중이거나(VisualFile), 죽었거나(Dead), 그리드에 없는 경우 제외
            if (file == null || file.FileState == FileStatus.Dead || file.FileState == FileStatus.VisualFile || file.CurrentGrid == null)
                continue;

            float dist = Vector2Int.Distance(segments[0], new Vector2Int(file.CurrentGrid.GridX, file.CurrentGrid.GridY));
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestFile = file;
            }
        }

        if (nearestFile != currentTargetFile)
        {
            currentTargetFile = nearestFile;
        }
    }

    private void DecideDirection()
    {
        if (currentTargetFile == null || currentTargetFile.CurrentGrid == null)
        {
            // 타겟이 없으면 무작위 배회
            Wander();
            return;
        }

        Vector2Int targetPos = new Vector2Int(currentTargetFile.CurrentGrid.GridX, currentTargetFile.CurrentGrid.GridY);
        Vector2Int headPos = segments[0];

        Vector2Int nextStep = GetNextStepBFS(headPos, targetPos);
        if (nextStep != headPos)
        {
            Vector2Int newDir = nextStep - headPos;
            if (newDir != -currentDirection)
            {
                currentDirection = newDir;
            }
        }
    }

    private void Wander()
    {
        Vector2Int headPos = segments[0];
        Vector2Int nextPos = headPos + currentDirection;

        // 앞으로 갈 수 없거나, 일정 확률로 방향 전환
        if (!Managers.GridMgr.IsValidGridIndex(nextPos.x, nextPos.y) || Random.value < 0.2f)
        {
            Vector2Int[] potentialDirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            List<Vector2Int> validDirs = new List<Vector2Int>();

            foreach (var dir in potentialDirs)
            {
                if (dir == -currentDirection) continue; // 역주행 금지
                
                Vector2Int next = headPos + dir;
                if (Managers.GridMgr.IsValidGridIndex(next.x, next.y))
                {
                    validDirs.Add(dir);
                }
            }

            if (validDirs.Count > 0)
            {
                currentDirection = validDirs[Random.Range(0, validDirs.Count)];
            }
        }
    }

    private Vector2Int GetNextStepBFS(Vector2Int start, Vector2Int target)
    {
        int width = Managers.GridMgr.GridWidth;
        int height = Managers.GridMgr.GridHeight;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        parent[start] = start;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int curr = queue.Dequeue();
            if (curr == target)
            {
                Vector2Int step = curr;
                while (parent[step] != start)
                {
                    step = parent[step];
                }
                return step;
            }

            foreach (var d in dirs)
            {
                Vector2Int next = curr + d;
                if (Managers.GridMgr.IsValidGridIndex(next.x, next.y) && !parent.ContainsKey(next))
                {
                    if (curr == start && d == -currentDirection) continue;

                    parent[next] = curr;
                    queue.Enqueue(next);
                }
            }
        }

        return start;
    }

    private bool TryTurn(ref Vector2Int newHeadPos)
    {
        Vector2Int headPos = segments[0];
        Vector2Int[] potentialDirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in potentialDirs)
        {
            if (dir == currentDirection || dir == -currentDirection) continue;

            Vector2Int next = headPos + dir;
            if (Managers.GridMgr.IsValidGridIndex(next.x, next.y))
            {
                currentDirection = dir;
                newHeadPos = next;
                return true;
            }
        }
        return false;
    }

    private void CheckFileInteraction(Vector2Int headPos, Vector2Int lastTailPos)
    {
        FileGrid headGrid = Managers.GridMgr.gridArray[headPos.x, headPos.y];
        File_Base fileAtHead = GetFileAt(headPos);
        if (fileAtHead != null)
        {
            fileAtHead.DeleteThisFile();
            AddBodySegment(lastTailPos);
        }

        for (int i = 1; i < segments.Count; i++)
        {
            File_Base fileAtBody = GetFileAt(segments[i]);
            if (fileAtBody != null)
            {
                fileAtBody.TakeDmg(10f);
            }
        }
    }

    private File_Base GetFileAt(Vector2Int pos)
    {
        foreach (var file in Managers.GridMgr.ActiveFiles)
        {
            if (file != null && file.CurrentGrid != null &&
                file.FileState != FileStatus.VisualFile &&
                file.CurrentGrid.GridX == pos.x && file.CurrentGrid.GridY == pos.y)
            {
                return file;
            }
        }
        return null;
    }

    public void OnHeadHit(float damage)
    {
        if (currentState == SnakeState.Destroyed) return;

        float finalDamage = damage;
        phaseAccumulatedDamage += finalDamage;

        if (owner != null) owner.TakeDmg(finalDamage);

        if (phaseAccumulatedDamage >= phaseMaxDamage)
        {
            SetState(SnakeState.Destroyed);
        }
    }

    public override void StopPattern()
    {
        base.StopPattern();
        SetState(SnakeState.Destroyed);
    }

    private void Cleanup()
    {
        foreach (var obj in segmentObjects)
        {
            if (obj != null) Managers.Resource.Destroy(obj);
        }
        segmentObjects.Clear();
        segments.Clear();
    }

    private void OnDrawGizmos()
    {
        if (currentTargetFile != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTargetFile.transform.position, 1.0f);

            if (segments != null && segments.Count > 0)
            {
                FileGrid grid = Managers.GridMgr.gridArray[segments[0].x, segments[0].y];
                if (grid != null)
                {
                    Gizmos.DrawLine(grid.transform.position, currentTargetFile.transform.position);
                }
            }
        }
    }
}
