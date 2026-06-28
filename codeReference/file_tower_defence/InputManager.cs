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
    private Vector2 _dragStartPosition;       // mousedown 시점의 "보정" 좌표 (박스 선택 판정용)
    private Vector2 _dragStartRaw;            // mousedown 시점의 "raw" 좌표 (박스 그리기용 - 박스는 왜곡 안 됨)
    private Vector2 _previousMousePosition;

    // 파일 드래그 떨림 방지: 렌즈 왜곡 보정이 마우스 미세 노이즈를 증폭하므로, 보정 좌표를 1€ 필터로 스무딩한다.
    private readonly OneEuroFilter2D _dragFilter = new OneEuroFilter2D(minCutoff: 1f, beta: 0.01f, dCutoff: 1f);
    private Vector2 _prevSmoothedMouse;

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

        m_mouseInput = LensDistortionCorrector.MousePosition; // 렌즈 왜곡 보정
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
            if (IsPointerOverUI())
            {
                if (_hoveredObject != null)
                {
                    m_interactionHandler.OnHoverExitHandler(_hoveredObject);
                    _hoveredObject = null;
                }

                return;
            }

            IInteractable objectUnderMouse = GetInteractableUnderMouse();
            if (objectUnderMouse != _hoveredObject)
            {
                if (_hoveredObject != null)
                    m_interactionHandler.OnHoverExitHandler(_hoveredObject);

                _hoveredObject = objectUnderMouse;

                if (_hoveredObject != null)
                    m_interactionHandler.OnHoverEnterHandler(_hoveredObject);
            }
            else
            {
                if (_hoveredObject != null)
                {
                    m_interactionHandler.UpdateTooltipHandler(_hoveredObject, Time.unscaledDeltaTime);
                }
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
        if (IsPointerOverUI())
            return;

        IInteractable prePressedObject = _pressedObject;
        _dragStartPosition = m_mouseInput;
        _dragStartRaw = Input.mousePosition;
        _pressedObject = _hoveredObject;

        //if (prePressedObject == null) return;

        currentState = InputState.Pressing;

        // UI가 아닌 게임 필드(파일/빈땅) 클릭에도 UI와 동일한 클릭음을 재생한다.
        // (HandleLeftMouseDown은 IsOverUI()면 이미 return하므로 여기는 필드 클릭 전용 → UI 클릭음과 중복되지 않음)
        Managers.soundManager?.PlaySfx(Define.SFX.SFX_click);

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
            stageMain?.HandleLeftClickEmpty();
            m_interactionHandler.ClearSelectionsHandler();
        }
    }

    private void HandleLeftMouseHold()
    {
        // 드래그 시작 조건 확인
        // 드래그/클릭 판정은 raw 좌표 기준으로 한다. 렌즈 왜곡 보정 좌표는 비선형 증폭 때문에
        // 짧은 클릭에도 임계값을 넘어 드래그로 오인되므로 사용하지 않는다.
        if (currentState == InputState.Pressing && Vector2.Distance((Vector2)Input.mousePosition, _dragStartRaw) > dragThreshold)
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
            // 보정 좌표를 1€ 필터로 스무딩해 떨림 제거 (느릴 땐 강하게, 빠를 땐 거의 지연 없이)
            float dt = Mathf.Max(Time.unscaledDeltaTime, 1e-5f);
            Vector2 smoothed = _dragFilter.Filter(m_mouseInput, dt);
            Vector2 mouseDelta = (Vector2)mainCamera.ScreenToWorldPoint(smoothed) - (Vector2)mainCamera.ScreenToWorldPoint(_prevSmoothedMouse);
            _prevSmoothedMouse = smoothed;
            m_interactionHandler.OnDragHandler(mouseDelta);
        }
        else if (currentState == InputState.DraggingBox)
        {
            if (dragBoxVisual == null) return;
            dragBoxParent = dragBoxVisual.parent as RectTransform;

            Camera uiCamera = (dragBoxCanvas != null && dragBoxCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? dragBoxCanvas.worldCamera
                : null;

            // 박스는 왜곡되지 않는 UI 오버레이이므로 raw 마우스 좌표로 그린다 (보정 시 떨림 + 위치 오프셋 발생)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragBoxParent, _dragStartRaw, uiCamera, out Vector2 startLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragBoxParent, Input.mousePosition, uiCamera, out Vector2 currentLocal);

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

        if (IsPointerOverUI())
            return;

        // 우클릭(파일/빈땅)에도 클릭음 재생 (UI는 위에서 return하므로 중복 없음)
        Managers.soundManager?.PlaySfx(Define.SFX.SFX_click);

        if (objectUnderMouse != null)
        {
            m_interactionHandler.OnRightClickHandler(objectUnderMouse);
        }
        else
        {
            stageMain?.HandleRightClickEmpty(m_mouseWorldPosition);
        }
    }

    private void StartDraggingObject()
    {
        currentState = InputState.DraggingObject;
        // 스무딩 필터를 현재 위치로 초기화해 드래그 시작 시 튐 방지
        _dragFilter.Reset(m_mouseInput);
        _prevSmoothedMouse = m_mouseInput;
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

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}

/// <summary>
/// 1€ Filter (Casiez et al.) 2D 버전. 포인터 신호의 고주파 떨림은 억제하면서
/// 빠르게 움직일 때는 지연을 최소화한다. (천천히 움직이거나 멈춰 있을 때만 강하게 스무딩)
/// </summary>
public class OneEuroFilter2D
{
    private readonly float _minCutoff;
    private readonly float _beta;
    private readonly float _dCutoff;

    private Vector2 _xPrev;
    private Vector2 _dxPrev;
    private bool _hasPrev;

    public OneEuroFilter2D(float minCutoff = 1f, float beta = 0.01f, float dCutoff = 1f)
    {
        _minCutoff = minCutoff;
        _beta = beta;
        _dCutoff = dCutoff;
    }

    public void Reset(Vector2 value)
    {
        _xPrev = value;
        _dxPrev = Vector2.zero;
        _hasPrev = true;
    }

    public Vector2 Filter(Vector2 x, float dt)
    {
        if (!_hasPrev || dt <= 0f)
        {
            Reset(x);
            return x;
        }

        // 미분(속도) 추정 후 저역통과
        Vector2 dx = (x - _xPrev) / dt;
        Vector2 dxHat = Vector2.Lerp(_dxPrev, dx, Alpha(_dCutoff, dt));

        // 속도가 클수록 cutoff를 높여 지연을 줄임
        float cutoff = _minCutoff + _beta * dxHat.magnitude;
        Vector2 xHat = Vector2.Lerp(_xPrev, x, Alpha(cutoff, dt));

        _xPrev = xHat;
        _dxPrev = dxHat;
        return xHat;
    }

    private static float Alpha(float cutoff, float dt)
    {
        float tau = 1f / (2f * Mathf.PI * cutoff);
        return 1f / (1f + tau / dt);
    }
}
