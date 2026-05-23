using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// GameObject들을 GridLayout으로 배치하는 커스텀 컴포넌트입니다.
/// 일반 Transform을 사용하는 게임오브젝트용 그리드 레이아웃입니다.
/// </summary>
public class GameObjectGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int columns = 13; // X축 그리드 수
    [SerializeField] private int rows = 7;     // Y축 그리드 수

    [SerializeField] private Vector2 cellSize = Vector2.one; // 셀 크기
    [SerializeField] private Vector2 spacing = Vector2.zero; // 간격
    [SerializeField] private Vector2 padding = Vector2.zero; // 여백

    [Header("Layout Settings")]
    [SerializeField] private bool fitToScreen; // 화면에 맞게 자동 조정

    private List<GameObject> gridObjects = new List<GameObject>();

    // [추가] 화면 하단 UI를 위해 확보할 영역의 비율 (0.1 = 화면 높이의 10%)
    [SerializeField, Range(0f, 1f)] private float bottomOffsetRatio = 0.1f;

    // [추가] 그리드 전체의 실제 월드 크기
    private Vector2 _gridWorldSize;
    // [추가] 그리드의 좌측 하단 모서리의 월드 좌표
    private Vector2 _bottomLeftCorner;

    // Public Properties
    public int Columns => columns;
    public int Rows => rows;
    public Vector2 CellSize => cellSize;
    public Vector2 Spacing => spacing;
    public Vector2 Padding => padding;
    public bool GetFitToScreen => fitToScreen;

    /// <summary>
    /// 화면에 맞게 그리드의 크기를 자동 조정합니다.
    /// </summary>
    public void FitToScreen()
    {
        if (transform == null) return;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        // 카메라 기준 전체 화면 크기 계산
        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;

        // UI 오프셋을 월드 단위로 계산
        float yOffset = screenHeight * bottomOffsetRatio;

        // 오프셋을 반영하여 그리드가 사용 가능한 실제 높이 계산
        float availableGridHeight = screenHeight - yOffset;

        // 패딩을 제외한 사용 가능한 영역 계산
        Vector2 availableSize = new Vector2(screenWidth, availableGridHeight) - padding * 2;

        // 간격을 제외한 실제 셀 영역 계산
        Vector2 totalSpacing = new Vector2(
            (columns - 1) * spacing.x,
            (rows - 1) * spacing.y
        );

        Vector2 cellArea = availableSize - totalSpacing;

        // 최종 셀 크기 계산
        cellSize = new Vector2(
            cellArea.x / columns,
            cellArea.y / rows
        );

        // 그리드 전체의 실제 월드 크기 계산 (셀 + 간격)
        _gridWorldSize = new Vector2(
            columns * cellSize.x + (columns - 1) * spacing.x,
            rows * cellSize.y + (rows - 1) * spacing.y
        );

        // 그리드의 새 중심 Y좌표 계산
        Vector3 newPosition = transform.position;
        newPosition.y = yOffset / 2f;
        transform.position = newPosition; // 그리드 오브젝트의 위치(중심점)를 위로 이동

        // 그리드의 좌측 하단 모서리 좌표 계산
        _bottomLeftCorner = (Vector2)transform.position - _gridWorldSize / 2f;

        Debug.Log($"GridLayout 자동 조정 완료: {columns}x{rows}, 셀 크기: {cellSize}");
    }



    /// <summary>
    /// 그리드에 GameObject를 추가합니다.
    /// </summary>
    public void AddGridObject(GameObject obj, int x, int y)
    {
        if (obj == null) return;

        if (x < 0 || x >= columns || y < 0 || y >= rows)
        {
            Debug.LogWarning($"잘못된 그리드 위치: ({x}, {y}). 그리드 크기: {columns}x{rows}");
            return;
        }

        obj.transform.SetParent(transform);
        
        float xPos = _bottomLeftCorner.x + cellSize.x / 2f + x * (cellSize.x + spacing.x);
        float yPos = _bottomLeftCorner.y + cellSize.y / 2f + y * (cellSize.y + spacing.y);
        Vector2 worldPosition = new Vector2(xPos, yPos);
        obj.transform.position = worldPosition;

        // 크기 설정
        obj.transform.localScale = Vector3.one;

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = cellSize;
        }

        gridObjects.Add(obj);
    }

    /// <summary>
    /// 모든 그리드 오브젝트를 제거합니다.
    /// </summary>
    public void ClearGrid()
    {
        foreach (var obj in gridObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        gridObjects.Clear();
    }
}