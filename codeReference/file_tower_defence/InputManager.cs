using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 게임 전체의 입력을 중앙에서 처리하는 입력 매니저
/// </summary>
public class InputManager
{
    private StageMain stageMain;

    private Camera mainCamera;
    private RectTransform dragBoxVisual;
    private RectTransform dragBoxParent;
    private Canvas dragBoxCanvas;

    private LayerMask unitLayerMask;
    private LayerMask virusLayerMask;
    private LayerMask interactableLayerMask; // 유닛과 바이러스를 모두 포함하는 레이어 마스크

    private float dragThreshold;

    // 입력 처리 상태 구분
    private enum InputState { None, Pressing, DraggingObject, DraggingBox }
    private InputState currentState = InputState.None;

    private IInteractable _hoveredObject;
    private IInteractable _pressedObject;
    private Vector2 _dragStartPosition;
    private Vector2 _previousMousePosition;

    // 이벤트 기반 상호작용 핸들러
    private InteractionHandler m_interactionHandler;

    private Vector2 m_mouseInput;
    private Vector3 m_mouseWorldPosition;

    private float _lastClickTime;
    private const float _doubleClickThreshold = 0.3f;
    bool isMultiSelect;

    public void Init(RectTransform dragBox, float threshold)
    {
        stageMain = Managers.UI.GetSceneUI<StageMain>();
        m_interactionHandler = stageMain?.InteractionHandler;

        mainCamera = Camera.main;
        dragBoxVisual = dragBox;
        unitLayerMask = LayerMask.GetMask("Files");
        virusLayerMask = LayerMask.GetMask("Virus");

        // 상호작용 가능한 모든 레이어 마스크
        interactableLayerMask = unitLayerMask;
        interactableLayerMask.value = unitLayerMask.value | virusLayerMask.value;

        dragThreshold = threshold;
        isMultiSelect = false;
        if (dragBoxVisual == null)
        {
            // 드래그 박스를 자동으로 생성하고 설정
            // 기본 부모는 UI(Canvas) 계층으로 설정
            Transform parentForDragBox = Managers.UI.GetSceneUI<StageMain>()?.transform;
            if (parentForDragBox == null)
            {
                var anyCanvas = Managers.UI.GetSceneUI<StageMain>()?.GetComponent<Canvas>();
                parentForDragBox = anyCanvas != null ? anyCanvas.transform : Managers.Pool.Root.transform;
            }
            GameObject box = new GameObject("@DragSelectionBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            box.transform.SetParent(parentForDragBox, false);
            var img = box.GetComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 1f, 0.2f);
            img.raycastTarget = false;
            dragBoxVisual = box.GetComponent<RectTransform>();
            dragBoxVisual.anchorMin = Vector2.zero;
            dragBoxVisual.anchorMax = Vector2.zero;
            dragBoxVisual.pivot = Vector2.zero;
            dragBoxVisual.anchoredPosition = Vector2.zero;
            dragBoxVisual.sizeDelta = Vector2.zero;

            Canvas canvas = dragBoxVisual.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Covered";
            canvas.sortingOrder = 0;
        }

        if (dragBoxVisual != null)
        {
            dragBoxCanvas = dragBoxVisual.GetComponent<Canvas>();
            if (dragBoxCanvas == null)
                dragBoxCanvas = dragBoxVisual.GetComponentInParent<Canvas>();

            dragBoxParent = dragBoxVisual.parent as RectTransform ?? dragBoxVisual;
            dragBoxVisual.gameObject.SetActive(false);
        }

        Managers.RealTimeUpdateEvent -= UnscaledUpdate;
        Managers.RealTimeUpdateEvent += UnscaledUpdate;
    }

    private void UnscaledUpdate(float unscaledTime)
    {
        if (Managers.newStep == Define.StageState.ReadyState)
            return;

        m_mouseInput = Input.mousePosition;
        // 마우스 월드 좌표 갱신 (메뉴/드롭 등 월드 기준 로직에서 사용)
        m_mouseWorldPosition = mainCamera.ScreenToWorldPoint(m_mouseInput);

        HandleHover();
        HandleMouseInput();
        HandleKeyInput();
        _previousMousePosition = m_mouseInput;
    }

    public void Clear()
    {
        Managers.RealTimeUpdateEvent -= UnscaledUpdate;
        _hoveredObject = null;
        _pressedObject = null;
        currentState = InputState.None;
        m_interactionHandler?.Clear();
    }

    // HandleHover, HandleMouseInput 등 모든 입력 처리 로직입니다.
    #region Input Handling Logic
    private void HandleHover()
    {
        if (currentState == InputState.None || currentState == InputState.Pressing)
        {
            if (IsPointerOverUI(Input.mousePosition))
            {
                if (_hoveredObject == null) return;

                m_interactionHandler.OnHoverExitHandler(_hoveredObject);
                _hoveredObject = null;

                return;
            }

            IInteractable objectUnderMouse = GetInteractableUnderMouse();
            if (objectUnderMouse != _hoveredObject)
            {
                if (_hoveredObject == null) return;
                m_interactionHandler.OnHoverExitHandler(_hoveredObject);

                _hoveredObject = objectUnderMouse;

                if (_hoveredObject == null) return;
                m_interactionHandler.OnHoverEnterHandler(_hoveredObject);
            }
            else
            {
                if (_hoveredObject == null) return;
                m_interactionHandler.UpdateTooltipHandler(_hoveredObject, Time.unscaledDeltaTime);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) HandleLeftMouseDown();
        else if (Input.GetMouseButton(0)) HandleLeftMouseHold();
        else if (Input.GetMouseButtonUp(0)) HandleLeftMouseUp();
        else if (Input.GetMouseButtonDown(1)) HandleRightMouseDown();
    }

    private void HandleKeyInput()
    {
        isMultiSelect = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            IInteractable[] selectObj = m_interactionHandler.GetSelectedObjects();
            for (int i = 0; i < selectObj.Length; i++)
            {
                if (selectObj[i] is File_Base file)
                {
                    if (file.FileState != FileStatus.normal) { return; }

                    file.DeleteThisFile();
                }
            }
        }
    }

    private void HandleLeftMouseDown()
    {
        // UI 위에 있으면 드래그 관련 로직을 모두 무시
        if (IsPointerOverUI(Input.mousePosition))
            return;

        IInteractable prePressedObject = _pressedObject;
        _dragStartPosition = m_mouseInput;
        _pressedObject = _hoveredObject;

        //if (prePressedObject == null) return;

        currentState = InputState.Pressing;

        if (_pressedObject != null)
        {
            // 더블클릭 처리
            if (Time.unscaledTime - _lastClickTime < _doubleClickThreshold
                && _pressedObject == prePressedObject)
            {
                m_interactionHandler.OnDoubleClickHandler(_pressedObject);
            }
            else
            {
                _lastClickTime = Time.unscaledTime;
                m_interactionHandler.OnClickEnterHandler(_pressedObject, isMultiSelect);
            }
        }
        else
        {
            stageMain?.UICom?.HandleLeftClickEmpty();
            m_interactionHandler.ClearSelectionsHandler();
        }
    }

    private void HandleLeftMouseHold()
    {
        // 드래그 시작 조건 확인
        if (currentState == InputState.Pressing && Vector2.Distance(m_mouseInput, _dragStartPosition) > dragThreshold)
        {
            if (_pressedObject != null)
            {
                StartDraggingObject();
            }
            else
            {
                StartDraggingBox();
            }
        }

        // 드래그 중 처리
        if (currentState == InputState.DraggingObject)
        {
            Vector2 mouseDelta = (Vector2)mainCamera.ScreenToWorldPoint(m_mouseInput) - (Vector2)mainCamera.ScreenToWorldPoint(_previousMousePosition);
            m_interactionHandler.OnDragHandler(mouseDelta);
        }
        else if (currentState == InputState.DraggingBox)
        {
            if (dragBoxVisual == null) return;
            dragBoxParent = dragBoxVisual.parent as RectTransform;

            Camera uiCamera = (dragBoxCanvas != null && dragBoxCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? dragBoxCanvas.worldCamera
                : null;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragBoxParent, _dragStartPosition, uiCamera, out Vector2 startLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragBoxParent, m_mouseInput, uiCamera, out Vector2 currentLocal);

            Vector2 lowerLeft = new Vector2(Mathf.Min(startLocal.x, currentLocal.x), Mathf.Min(startLocal.y, currentLocal.y));
            Vector2 upperRight = new Vector2(Mathf.Max(startLocal.x, currentLocal.x), Mathf.Max(startLocal.y, currentLocal.y));

            Vector2 parentOffset = Vector2.zero;
            if (dragBoxParent != null)
                parentOffset = new Vector2(dragBoxParent.rect.width * dragBoxParent.pivot.x, dragBoxParent.rect.height * dragBoxParent.pivot.y);

            // 드래그 좌표 기준으로 고정된 마우스의 박스 그리기
            dragBoxVisual.anchoredPosition = lowerLeft + parentOffset;
            dragBoxVisual.sizeDelta = upperRight - lowerLeft;
        }
    }

    private void HandleLeftMouseUp()
    {
        switch (currentState)
        {
            case InputState.DraggingObject:
                HandleDragObjectEnd();
                break;
            case InputState.DraggingBox:
                HandleDragBoxEnd();
                break;
            case InputState.None:
                ClearSelections();
                break;
        }

        _pressedObject?.OnClickExit();
        currentState = InputState.None;
    }

    private void HandleRightMouseDown()
    {
        IInteractable objectUnderMouse = GetInteractableUnderMouse();

        if (IsPointerOverUI(Input.mousePosition))
            return;

        if (objectUnderMouse != null)
        {
            m_interactionHandler.OnRightClickHandler(objectUnderMouse);
        }
        else
        {
            stageMain?.UICom?.HandleRightClickEmpty(m_mouseWorldPosition);
        }
    }

    private void StartDraggingObject()
    {
        currentState = InputState.DraggingObject;
        m_interactionHandler.OnBeginDragHandler(_pressedObject);
    }

    private void StartDraggingBox()
    {
        currentState = InputState.DraggingBox;
        if (dragBoxVisual != null)
        {
            dragBoxVisual.gameObject.SetActive(true);
        }
    }

    private void HandleDragObjectEnd()
    {
        IInteractable underMouseObj = GetInteractableUnderMouse();
        Managers.GridMgr.HandleDropAt(m_mouseInput, underMouseObj, m_interactionHandler);

        m_interactionHandler.OnEndDragHandler(_pressedObject);

        m_interactionHandler.Clear();
    }

    private void HandleDragBoxEnd()
    {
        // 위에 있는 UI 요소 박스 드래그로 오브젝트들을 선택(유닛 UI 요소 제외)
        // if (!IsOverUI())
        // {
        // }
        SelectObjectsInDragBox();

        dragBoxVisual.gameObject.SetActive(false);
    }

    private IInteractable GetInteractableUnderMouse()
    {
        // ================================================================
        // [1순위] Unit (파일) 레이어 먼저 감지
        // ================================================================
        RaycastHit2D unitHit = Physics2D.Raycast(m_mouseWorldPosition, Vector2.zero, 0f, unitLayerMask);

        if (unitHit.collider != null)
        {
            IInteractable unit = unitHit.collider.GetComponent<IInteractable>()
                              ?? unitHit.collider.GetComponentInParent<IInteractable>();

            if (unit != null)
            {
                return unit;
            }
        }

        // ================================================================
        // [2순위] 파일이 없을 때만 나머지 상호작용(바이러스 등) 감지
        // ================================================================
        RaycastHit2D hit = Physics2D.Raycast(m_mouseWorldPosition, Vector2.zero, 0f, interactableLayerMask);

        if (hit.collider != null)
        {
            IInteractable unit = hit.collider.GetComponent<IInteractable>()
                              ?? hit.collider.GetComponentInParent<IInteractable>();

            if (unit != null)
            {
                return unit;
            }
        }
        return null;
    }

    private void SelectObjectsInDragBox()
    {
        Vector2 startWorldPos = mainCamera.ScreenToWorldPoint(_dragStartPosition);
        Vector2 currentWorldPos = mainCamera.ScreenToWorldPoint(m_mouseInput);
        Vector2 min = new Vector2(Mathf.Min(startWorldPos.x, currentWorldPos.x), Mathf.Min(startWorldPos.y, currentWorldPos.y));
        Vector2 max = new Vector2(Mathf.Max(startWorldPos.x, currentWorldPos.x), Mathf.Max(startWorldPos.y, currentWorldPos.y));
        Collider2D[] collidersInBox = Physics2D.OverlapAreaAll(min, max, interactableLayerMask);

        List<IInteractable> objectsInBox = new List<IInteractable>();
        foreach (var collider in collidersInBox)
        {
            if (collider.TryGetComponent<File_Base>(out var unit))
            {
                // MyCom 유닛은 드래그 박스 선택에서 제외
                if (!(unit is Unit_MyCom))
                {
                    objectsInBox.Add(unit);
                }
            }
            else if (collider.TryGetComponent<Unit_VirusBase>(out var virus))
            {
                objectsInBox.Add(virus);
            }
        }

        m_interactionHandler.SelectObjectsHandler(objectsInBox.ToArray());
    }

    private void ClearSelections()
    {
        m_interactionHandler.ClearSelectionsHandler();
    }
    #endregion

    #region UI Check Logic

    public T GetUIComponentAt<T>(Vector2 screenPos) where T : Component
    {
        if (EventSystem.current == null)
            return null;

        PointerEventData ped = new PointerEventData(EventSystem.current) { position = screenPos };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        foreach (var result in results)
        {
            if (result.gameObject != null)
            {
                T component = result.gameObject.GetComponent<T>()
                                ?? result.gameObject.GetComponentInParent<T>();

                if (component != null)
                    return component;
            }
        }
        return null;
    }

    /// <summary>
    /// 현재 마우스 위치 또는 지정된 위치가 UI에 가려져 있는지 확인
    /// 게임 오브젝트와 UI의 Sorting Layer/Order를 비교하여 실제 가려짐 여부를 판단
    /// </summary>
    public bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        PointerEventData ped = new PointerEventData(EventSystem.current) { position = screenPos };
        List<RaycastResult> uiResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, uiResults);

        if (uiResults.Count == 0)
            return false;

        Canvas uiCanvas = uiResults[0].gameObject.GetComponentInParent<Canvas>();
        if (uiCanvas == null)
            return true;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        RaycastHit2D gameObjectHit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, interactableLayerMask);

        IInteractable interactable = gameObjectHit.collider?.GetComponent<IInteractable>()
                                  ?? gameObjectHit.collider?.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable.IsSelectable)
        {
            if (interactable.targetObj.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                int spriteLayerValue = SortingLayer.GetLayerValueFromName(spriteRenderer.sortingLayerName);
                int canvasLayerValue = SortingLayer.GetLayerValueFromName(uiCanvas.sortingLayerName);

                if (canvasLayerValue > spriteLayerValue)
                    return true;
                if (canvasLayerValue == spriteLayerValue && uiCanvas.sortingOrder > spriteRenderer.sortingOrder)
                    return true;
                return false;
            }

            Canvas gameObjectCanvas = interactable.targetObj.GetComponentInChildren<Canvas>();
            if (gameObjectCanvas == null)
                return false;

            int objCanvasLayerValue = SortingLayer.GetLayerValueFromName(gameObjectCanvas.sortingLayerName);
            int uiCanvasLayerValue = SortingLayer.GetLayerValueFromName(uiCanvas.sortingLayerName);

            if (uiCanvasLayerValue > objCanvasLayerValue)
                return true;
            if (uiCanvasLayerValue == objCanvasLayerValue && uiCanvas.sortingOrder > gameObjectCanvas.sortingOrder)
                return true;

            return false;
        }

        return true; // 레이캐스트된 게임 오브젝트가 없는데 UI는 있으면 UI 위로 간주
    }

    #endregion
}
