using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이벤트 기반 상호작용을 관리하는 핸들러 클래스
/// </summary>
public class InteractionHandler
{
    // 선택 및 드래그 상태 관리 (SelectionData 기능 통합)
    public List<IInteractable> SelectedObjects { get; private set; } = new List<IInteractable>();
    public List<IInteractable> DragObjects { get; private set; } = new List<IInteractable>();
    private Dictionary<IInteractable, Vector3> OriginalPositions = new Dictionary<IInteractable, Vector3>();
    public int DragObjectsCount => DragObjects.Count;

    public InteractionHandler()
    {
    }

    #region Standard Interaction (File/Unit)
    /// <summary>
    /// 포인터 다운 처리
    /// </summary>
    public void OnClickEnterHandler(IInteractable obj, bool isMultiSelect)
    {
        if (obj == null) return;

        // 각 객체의 OnClickEnter 호출
        obj.OnClickEnter();

        // 선택 처리 (SelectionData 기능 통합)
        if (!obj.IsSelectable) return;

        if (!isMultiSelect)
        {
            if (!SelectedObjects.Contains(obj))
            {
                ClearSelections();
                AddSelection(obj);
                obj.OnSelectSingle();
            }
        }
        else
        {
            if (!SelectedObjects.Contains(obj)) AddSelection(obj);
            else RemoveSelection(obj);
        }


        // 각 객체의 OnClick 호출
        obj.OnClick();
    }

    /// <summary>
    /// 드래그 시작 처리
    /// </summary>
    public void OnBeginDragHandler(IInteractable obj)
    {
        if (obj == null || !obj.IsDraggable)
            return;

        // 선택 단계에서 이미 결정된 집합만 드래그 대상으로 인정
        if (SelectedObjects.Count == 0 || !SelectedObjects.Contains(obj))
            return;

        DragObjects.Clear();
        OriginalPositions.Clear();

        foreach (IInteractable o in SelectedObjects)
        {
            if (o != null && o.IsDraggable)
            {
                DragObjects.Add(o);
                OriginalPositions.TryAdd(o, o.transform.position);
            }
        }

        // 각 객체의 OnBeginDrag 호출 - 각자 알아서 처리
        foreach (IInteractable dragObj in DragObjects)
        {
            if (dragObj != null)
            {
                dragObj.OnBeginDrag(); // 시각/게임 로직 포함
            }
        }
    }

    /// <summary>
    /// 드래그 중 처리
    /// </summary>
    public void OnDragHandler(Vector2 mouseDelta)
    {
        // 각 객체의 OnDrag 호출 - 각자 위치 이동 처리
        foreach (IInteractable obj in DragObjects)
        {
            if (obj != null)
            {
                obj.OnDrag(mouseDelta);
            }
        }
    }

    /// <summary>
    /// 드래그 종료 처리
    /// </summary>
    public void OnEndDragHandler(IInteractable obj)
    {
        if (obj == null) return;

        IInteractable[] ended = DragObjects.ToArray();

        // 각 객체의 OnEndDrag에서 그리드 제거 및 시각 정리/게임 로직 처리
        foreach (IInteractable dragObj in ended)
        {
            if (dragObj != null)
            {
                dragObj.OnEndDrag(); // OnEndDrag에서 CurrentGrid?.RemoveFileUnit() 호출
            }
        }

        DragObjects.Clear();
        OriginalPositions.Clear();
        ClearSelections();
    }

    /// <summary>
    /// 더블클릭 처리
    /// </summary>
    public void OnDoubleClickHandler(IInteractable obj)
    {
        if (obj == null) return;

        // 각 객체의 OnDoubleClick 호출
        obj.OnDoubleClick();

        ClearSelections();
    }

    /// <summary>
    /// 우클릭 처리
    /// </summary>
    public void OnRightClickHandler(IInteractable obj)
    {
        if (obj == null || !obj.IsSelectable) return;

        AddSelection(obj);
        obj.OnSelectSingle();

        // 각 객체의 OnRightClick 호출
        obj.OnRightClick();
    }

    /// <summary>
    /// 포인터 진입 처리
    /// </summary>
    public void OnHoverEnterHandler(IInteractable obj)
    {
        if (obj == null) return;

        // 각 객체의 OnHoverEnter 호출
        obj.OnHoverEnter();
    }

    /// <summary>
    /// 포인터 지속됨 처리 (툴팁은 StageMain_UI에서 처리)
    /// </summary>
    public void UpdateTooltipHandler(IInteractable obj, float unscaledDeltaTime)
    {
        // 툴팁 처리는 StageMain_UI에서 담당
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        stageMain?.UICom?.UpdateTooltip(obj, unscaledDeltaTime);
    }

    /// <summary>
    /// 포인터 벗어남 처리
    /// </summary>
    public void OnHoverExitHandler(IInteractable obj)
    {
        if (obj == null) return;

        // 툴팁 숨김
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        stageMain?.UICom?.HideTooltip();

        // 각 객체의 OnHoverExit 호출
        obj.OnHoverExit();
    }

    /// <summary>
    /// 빈 공간 클릭 처리
    /// </summary>
    public void ClearSelectionsHandler()
    {
        ClearSelections();
    }

    /// <summary>
    /// 드래그 박스 선택 처리
    /// </summary>
    public void SelectObjectsHandler(IInteractable[] objectsInBox)
    {
        ClearSelections();
        if (objectsInBox == null) return;
        foreach (var o in objectsInBox)
        {
            if (o != null && o.IsSelectable) AddSelection(o);
        }
    }

    /// <summary>
    /// 현재 선택된 객체들 반환
    /// </summary>
    public IInteractable[] GetSelectedObjects()
    {
        return SelectedObjects.ToArray();
    }

    /// <summary>
    /// 현재 드래그 중인 객체들 반환
    /// </summary>
    public IInteractable[] GetDragObjects()
    {
        return DragObjects.ToArray();
    }

    /// <summary>
    /// 객체를 선택 목록에 추가
    /// </summary>
    public void AddSelection(IInteractable obj)
    {
        if (obj != null && obj.IsSelectable && !SelectedObjects.Contains(obj))
        {
            obj.OnSelected(true);
            SelectedObjects.Add(obj);
        }
    }

    /// <summary>
    /// 객체를 선택 목록에서 제거
    /// </summary>
    public void RemoveSelection(IInteractable unit)
    {
        if (unit != null)
        {
            unit.OnSelected(false);
            SelectedObjects.Remove(unit);
        }
    }

    /// <summary>
    /// 모든 선택 해제
    /// </summary>
    private void ClearSelections()
    {
        if (SelectedObjects == null || SelectedObjects.Count == 0)
            return;

        List<IInteractable> snapshot = new List<IInteractable>(SelectedObjects);
        foreach (var obj in snapshot)
        {
            if (obj is UnityEngine.Object unityObj && unityObj != null)
            {
                obj.OnSelected(false);
            }
        }
        SelectedObjects.Clear();
    }
    #endregion

    public void Clear()
    {
        ClearSelections();
        DragObjects.Clear();
        OriginalPositions.Clear();
    }
}
