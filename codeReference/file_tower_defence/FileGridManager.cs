using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class FileGridManager : MonoBehaviour
{
    [SerializeField] private GameObject fileGridPrefab;
    [SerializeField] private GameObjectGridLayout gridLayout;

    public GameObjectGridLayout GridLayout => gridLayout;

    public FileGrid[,] gridArray;

    [Header("유닛 목록")]
    public List<File_Base> ActiveFiles = new List<File_Base>();

    // GameObjectGridLayout을 통한 그리드 크기 정보를 제공
    public int GridWidth => gridLayout.Columns;
    public int GridHeight => gridLayout.Rows;

    public Vector2 GridDistance => gridLayout.CellSize + gridLayout.Spacing;
    public Vector2 CellSize => gridLayout.CellSize;

    private void Awake()
    {
        // GameObjectGridLayout 컴포넌트가 없으면 추가
        if (gridLayout == null)
        {
            gridLayout = GetComponent<GameObjectGridLayout>();
            if (gridLayout == null)
            {
                gridLayout = gameObject.AddComponent<GameObjectGridLayout>();
            }
        }

        // 그리드 크기를 초기화 (화면에 맞게 조정)
        if (gridLayout.GetFitToScreen)
        {
            gridLayout.FitToScreen();
        }

        // 그리드 배열 초기화
        gridArray = new FileGrid[GridWidth, GridHeight];

        // 그리드 생성
        CreateGrids();

        Debug.Log($"그리드 생성 완료: {GridWidth}x{GridHeight}");
    }

    /// <summary>
    /// 그리드들을 생성합니다.
    /// </summary>
    private void CreateGrids()
    {
        // 기존 그리드 제거
        gridLayout.ClearGrid();

        // 그리드 생성
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                // 그리드 게임오브젝트 생성
                GameObject gridGo = Instantiate(fileGridPrefab);
                gridGo.name = $"Grid_{x},{y}";

                // FileGrid 컴포넌트 설정
                FileGrid grid = gridGo.GetComponent<FileGrid>();
                if (grid != null)
                {
                    grid.Init(x, y);
                }

                // 그리드 레이아웃에 추가
                gridLayout.AddGridObject(gridGo, x, y);

                // 배열에 저장
                gridArray[x, y] = grid;
            }
        }
    }

    public bool IsValidGridIndex(int x, int y)
    {
        return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;
    }

    /// <summary>그리드 정중앙 칸을 반환(보스 등장 위치 등에 사용).</summary>
    public FileGrid GetCenterGrid()
    {
        if (gridArray == null) return null;
        int centerX = GridWidth / 2;
        int centerY = GridHeight / 2;
        if (IsValidGridIndex(centerX, centerY))
            return gridArray[centerX, centerY];
        return null;
    }

    /// <summary>그리드 인덱스 → 월드 좌표 (GetGridWorldPosition 별칭).</summary>
    public Vector2 GridIndexToWorld(int x, int y) => GetGridWorldPosition(x, y);

    /// <summary>
    /// 그리드 인덱스에 해당하는 월드 좌표를 반환합니다.
    /// </summary>
    public Vector2 GetGridWorldPosition(int x, int y)
    {
        if (gridLayout == null) return Vector2.zero;

        // 그리드의 시작 위치 계산 (중앙 기준)
        Vector2 startPos = new Vector2(
            -gridLayout.CellSize.x * (GridWidth - 1) * 0.5f,
            -gridLayout.CellSize.y * (GridHeight - 1) * 0.5f
        );

        // 패딩 추가
        startPos += gridLayout.Padding;

        // 로컬 위치 계산
        Vector2 localPos = new Vector2(
            startPos.x + x * (gridLayout.CellSize.x + gridLayout.Spacing.x),
            startPos.y + y * (gridLayout.CellSize.y + gridLayout.Spacing.y)
        );

        // 로컬 좌표를 월드 좌표로 변환
        return transform.TransformPoint(localPos);
    }

    public bool TrySetUnitAtGrid(File_Base unit, FileGrid targetGrid)
    {
        if (unit == null || targetGrid == null)
        {
            Debug.LogWarning("unit 또는 targetGrid가 null입니다.");
            return false;
        }

        // 그리드가 유효한 범위 내에 있는지 확인
        if (!IsValidGridIndex(targetGrid.GridX, targetGrid.GridY))
        {
            Debug.LogWarning($"그리드 ({targetGrid.GridX}, {targetGrid.GridY})이 유효하지 않은 범위입니다.");
            return false;
        }

        // 대상 그리드에 장애물이 있는지 확인
        if (targetGrid.obstacleObject != null)
        {
            return false;
        }

        // 현재 그리드에서 유닛 제거 (자신이 아닌 경우에만)
        FileGrid currentGrid = unit.CurrentGrid;
        if (currentGrid != null && currentGrid != targetGrid)
        {
            currentGrid.RemoveFileUnit();
        }

        unit.FileState = FileStatus.normal;

        unit.transform.position = targetGrid.transform.position;
        unit.transform.SetParent(targetGrid.transform);
        targetGrid.SetFileUnit(unit); // 여기서 자동으로 그리드의 버프들이 적용됨

        return true;
    }

    // 월드 좌표를 그리드 인덱스로 변환
    private bool WorldToGridIndex(Vector2 worldPos, out int x, out int y)
    {
        x = 0;
        y = 0;

        if (gridLayout == null) return false;

        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        Vector2 startPos = new Vector2(
            -gridLayout.CellSize.x * (GridWidth - 1) * 0.5f,
            -gridLayout.CellSize.y * (GridHeight - 1) * 0.5f
        );

        startPos += gridLayout.Padding;

        float xFloat = (localPos.x - startPos.x) / (gridLayout.CellSize.x + gridLayout.Spacing.x);
        float yFloat = (localPos.y - startPos.y) / (gridLayout.CellSize.y + gridLayout.Spacing.y);

        x = Mathf.RoundToInt(xFloat);
        y = Mathf.RoundToInt(yFloat);

        return true;
    }

    public bool IsWorldPositionInGridBounds(Vector2 worldPos)
    {
        if (!WorldToGridIndex(worldPos, out int x, out int y))
            return false;

        return IsValidGridIndex(x, y);
    }

    /// <summary>
    /// 월드 좌표에서 그리드 중심과의 실제 거리를 기반으로 가장 가까운 그리드를 찾음
    /// </summary>
    public FileGrid GetGrid(Vector2 worldPos)
    {
        if (!WorldToGridIndex(worldPos, out int xCenter, out int yCenter))
            return null;

        if (IsValidGridIndex(xCenter, yCenter))
        {
            return FindClosestGridInRange(worldPos, xCenter, yCenter);
        }

        // 범위 밖이면 가장 가까운 그리드 찾기
        return FindClosestGridFromAll(worldPos);
    }

    private FileGrid FindClosestGridInRange(Vector2 worldPos, int centerX, int centerY)
    {
        float minDistSqr = float.MaxValue;
        FileGrid closestGrid = null;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = centerX + dx;
                int y = centerY + dy;

                if (IsValidGridIndex(x, y))
                {
                    FileGrid grid = gridArray[x, y];
                    SearchClosestBetter(grid, worldPos, ref minDistSqr, ref closestGrid);
                }
            }
        }

        return closestGrid;
    }

    private FileGrid FindClosestGridFromAll(Vector2 worldPos)
    {
        float minDistSqr = float.MaxValue;
        FileGrid closestGrid = null;

        foreach (FileGrid grid in gridArray)
        {
            SearchClosestBetter(grid, worldPos, ref minDistSqr, ref closestGrid);
        }

        return closestGrid;
    }

    private bool CheckGridMatchesFlags(FileGrid grid, SearchGridFlag[] flags)
    {
        if (grid == null || flags == null || flags.Length == 0)
            return false;

        bool isOccupied = grid.GetFileUnit() != null;
        bool hasObstacle = grid.obstacleObject != null;

        foreach (SearchGridFlag flag in flags)
        {
            switch (flag)
            {
                case SearchGridFlag.Occupied:
                    if (!isOccupied) return false;
                    break;
                case SearchGridFlag.NotOccupied:
                    if (isOccupied) return false;
                    break;
                case SearchGridFlag.Obstacle:
                    if (!hasObstacle) return false;
                    break;
                case SearchGridFlag.NotObstacle:
                    if (hasObstacle) return false;
                    break;
                case SearchGridFlag.None:
                    break;
            }
        }

        return true;
    }

    private void SearchClosestBetter(FileGrid candidate, Vector2 worldPos, ref float bestDistSqr, ref FileGrid bestGrid)
    {
        if (candidate == null) return;

        float distSqr = ((Vector2)candidate.transform.position - worldPos).sqrMagnitude;
        if (distSqr < bestDistSqr)
        {
            bestDistSqr = distSqr;
            bestGrid = candidate;
        }
    }

    public enum SearchGridFlag
    {
        Occupied,       // 점유
        NotOccupied,    // 비점유
        Obstacle,       // 장애물이 존재하는 그리드만
        NotObstacle,    // 장애물이 존재하지 않는 그리드만
        None,
    }

    /// <summary>
    /// 월드 좌표에서 가장 가까운 그리드를 찾는다.
    /// 여러 플래그 조건을 모두 만족하는 그리드를 탐색
    /// Occupied: 점유된 그리드만, NotOccupied: 비점유 그리드만
    /// Obstacle: 장애물이 있는 그리드만, NotObstacle: 장애물이 없는 그리드만
    /// None: 필터 없음 (다른 플래그와 함께 사용 시 무시됨)
    /// </summary>
    public FileGrid FindFlagGridWorld(Vector2 worldPos, params SearchGridFlag[] flags)
    {
        float bestDistSqr = float.MaxValue;
        FileGrid bestGrid = null;

        foreach (FileGrid grid in gridArray)
        {
            if (grid == null) continue;

            // 플래그 조건 체크
            if (!CheckGridMatchesFlags(grid, flags))
                continue;

            SearchClosestBetter(grid, worldPos, ref bestDistSqr, ref bestGrid);
        }

        return bestGrid;
    }

    /// <summary>
    /// 유닛을 그리드에서 제거합니다.
    /// </summary>
    public bool TryRemoveUnit(File_Base unit)
    {
        if (unit == null)
            return false;

        FileGrid grid = unit.CurrentGrid;
        if (grid == null)
            return false;

        // 그리드에서 유닛 해제
        grid.RemoveFileUnit();

        // 설치 목록에서 제거
        ActiveFiles.Remove(unit);
        return true;
    }

    /// <summary>
    /// 월드 좌표에 땅굴을 생성합니다.
    /// </summary>
    public void CreateWormHoleAtPosition(Vector2 worldPos)
    {
        FileGrid grid = GetGrid(worldPos);

        // 유효하지 않거나 이미 장애물이 있으면 생성하지 않음
        if (grid == null || grid.obstacleObject != null)
            return;

        // 해당 그리드에 유닛이 있으면 제거
        File_Base unit = grid.GetFileUnit();
        if (unit != null)
        {
            TryRemoveUnit(unit);
        }

        // 땅굴 프리팹 로드 및 생성
        GameObject holePrefab = Resources.Load<GameObject>("Prefabs/Effect/WormBlockLandObj");
        if (holePrefab == null)
        {
            Debug.LogError("프리팹을 찾을 수 없습니다.");
            return;
        }

        GameObject holeInstance = Instantiate(holePrefab, grid.transform);
        holeInstance.transform.localPosition = Vector3.zero;
        grid.SetObstacle(holeInstance);

        Debug.Log($"그리드 ({grid.GridX}, {grid.GridY})에 땅굴을 생성했습니다.");
    }

    /// <summary>
    /// 모든 그리드의 땅굴을 제거합니다.
    /// </summary>
    public void RemoveAllWormHoles()
    {
        foreach (var grid in gridArray)
        {
            if (grid != null && grid.obstacleObject != null)
            {
                grid.RemoveObstacle();
            }
        }
        Debug.Log("RemoveAllWormHoles: 모든 땅굴을 제거했습니다.");
    }

    /// <summary>
    /// 폴더 드롭 특수 처리: UI 폴더창 또는 그리드 상의 폴더로 드롭 시 폴더에 파일 추가
    /// 참고: 일반 그리드 드롭은 File_Base.OnEndDrag()에서 처리됨
    /// </summary>
    public void HandleDropAt(Vector2 screenPosition, IInteractable underMouse, InteractionHandler interactionHandler)
    {
        IInteractable[] dragObjects = interactionHandler?.GetDragObjects();
        if (dragObjects == null || dragObjects.Length == 0)
            return;

        // UI 레이캐스트로 드롭 대상 UI 찾기
        PointerEventData ped = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        for (int i = 0; i < results.Count; i++)
        {
            GameObject go = results[i].gameObject;
            if (go == null)
                continue;

            IUIDropTarget dropTarget = go.GetComponent<IUIDropTarget>();
            if (dropTarget != null)
            {
                if (dropTarget.HandleDrop(dragObjects, interactionHandler))
                {
                    return;
                }
            }
        }

        // 그리드 상의 폴더에 드롭
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        
        // 그리드 범위 내에 드롭했는지 확인
        if (IsWorldPositionInGridBounds(worldPos))
        {
            FileGrid grid = GetGrid(worldPos);
            if (grid != null && grid.GetFileUnit() is Unit_Folder gridFolder && !gridFolder.isZip)
            {
                bool isDraggingTargetFolder = false;
                foreach (IInteractable obj in dragObjects)
                {
                    if (obj.targetObj == gridFolder.gameObject)
                    {
                        isDraggingTargetFolder = true;
                        break;
                    }
                }

                // 드래그 중인 폴더가 없을 때만 폴더 드롭 처리
                if (!isDraggingTargetFolder)
                {
                    Unit_Folder.TryDropToFolder(dragObjects, gridFolder.myFolderWin);
                }
            }
        }
    }

    /// <summary>
    /// 현재 맵(필드) 위에 존재하는 정상 상태의 파일 개수를 카운트하여 반환합니다.
    /// </summary>
    public Dictionary<Define.Extension, int> GetAvailableFileCounts()
    {
        Dictionary<Define.Extension, int> fileCounts = new Dictionary<Define.Extension, int>();

        foreach (var unit in ActiveFiles)
        {
            // 폴더 안에 들어있지 않고, 정상 상태인 파일만 체크
            if (unit != null && unit.FileState == FileStatus.normal && !(unit is Unit_Folder))
            {
                if (!fileCounts.ContainsKey(unit.myExtension))
                {
                    fileCounts[unit.myExtension] = 0;
                }
                fileCounts[unit.myExtension]++;
            }
        }
        return fileCounts;
    }

    /// <summary>
    /// 현재 필드의 재료로 조합 가능한 (그리고 해금된) 상위 확장자 리스트를 반환합니다.
    /// </summary>
    public List<Define.Extension> GetCraftableExtensions()
    {
        List<Define.Extension> craftableList = new List<Define.Extension>();
        
        // 1. 현재 맵 위에 있는 정상 상태 파일들의 개수를 가져옵니다. (이전 답변의 함수)
        Dictionary<Define.Extension, int> availableFiles = GetAvailableFileCounts();

        // 2. CombineFileDict를 순회하며 조합 가능 여부를 체크합니다.
        foreach (var kvp in Managers.Data.CombineFileDict) // 접근 경로에 맞게 수정하세요 (예: Managers.Data.CombineFileDict)
        {
            string recipeKey = kvp.Key;            // 예: "TXT,TXT,TXT" 또는 "BROWSER,EXE"
            Define.Extension resultExt = kvp.Value; // 예: WWARD, CHOROME

            // ★ [해금 시스템 연동] 만약 아직 돈을 주고 해금하지 않은 상위 파일이라면 체크를 건너뜁니다.
            // if (!Managers.Unlock.IsUnlocked(resultExt)) continue;

            // 3. 콤마(,)를 기준으로 문자열을 분리하여 필요한 재료 배열 생성
            string[] requiredExtStrings = recipeKey.Split(',');

            // 4. 이 레시피에 필요한 각 확장자의 개수를 카운트 (임시 딕셔너리)
            Dictionary<string, int> requiredCounts = new Dictionary<string, int>();
            foreach (string extStr in requiredExtStrings)
            {
                if (!requiredCounts.ContainsKey(extStr))
                    requiredCounts[extStr] = 0;
                
                requiredCounts[extStr]++;
            }

            // 5. 필드에 있는 파일(availableFiles)이 요구량을 만족하는지 검사
            bool canCraft = true;
            foreach (var req in requiredCounts)
            {
                // 문자열("TXT")을 다시 Enum(Define.Extension.TXT)으로 변환하여 비교
                if (Enum.TryParse(req.Key, out Define.Extension parsedExt))
                {
                    // 필드에 해당 파일이 아예 없거나, 개수가 부족하면 제작 불가
                    if (!availableFiles.ContainsKey(parsedExt) || availableFiles[parsedExt] < req.Value)
                    {
                        canCraft = false;
                        break;
                    }
                }
                else
                {
                    // 혹시라도 Enum 파싱에 실패하면 안전하게 불가 처리
                    canCraft = false;
                    break;
                }
            }

            // 6. 모든 재료가 충분하다면 결과물 리스트에 추가
            if (canCraft)
            {
                // 중복 방지 처리
                if (!craftableList.Contains(resultExt))
                {
                    craftableList.Add(resultExt);
                }
            }
        }

        return craftableList;
    }

}