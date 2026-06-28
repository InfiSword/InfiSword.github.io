using UnityEngine; 

public interface IInteractable
{
    GameObject targetObj { get; }
    Transform transform { get; }
    Sprite TooltipImg { get; }  // 툴팁은 클래스로 만들까 고민
    string ToolTipDes { get; }  
    
    // 선택 상태 관리
    bool IsSelectable { get; }
    bool IsDraggable { get; }
    bool IsTooltipEnabled { get; }
    
    // 이벤트 메서드
    void OnHoverEnter();     // 마우스가 객체 위로 올라올 때 (ToolTip 타이밍 리셋용)
    void OnHoverExit();      // 마우스가 객체에서 벗어날 때 (ToolTip 숨김)
    void OnClickEnter();     // 마우스 버튼을 누를 때
    void OnClickExit();      // 마우스 버튼을 뗄 때
    void OnBeginDrag();
    void OnDrag(Vector2 mouseDelta); 
    void OnEndDrag();
    void OnClick();          // 클릭 처리
    void OnDoubleClick();    // 더블클릭 처리
    void OnRightClick();     // 우클릭 처리
    void OnSelectSingle();

    // 선택 상태 표시
    void OnSelected(bool isSelected);
}

public interface IUIDropTarget
{
    bool HandleDrop(IInteractable[] dragObjects, InteractionHandler interactionHandler);
}
