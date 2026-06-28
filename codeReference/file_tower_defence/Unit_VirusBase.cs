using UnityEngine;

public abstract class Unit_VirusBase : MonoBehaviour, IHealth, IInteractable
{
    public enum VirusState
    {
        Idle,
        Move,
        Attack,
        Dead,
    }

    public virtual bool canAttacked => true;    // 공격 받을 수 있는 상태를 나타내는 변수

    protected Define.VirusType myvirusType; // 바이러스 타입(보스 등 식별용)
    public Define.VirusType MyVirusType { get => myvirusType; set => myvirusType = value; }

    public abstract void Init(int _pathNum = -1, bool isElite = false);   // 길이 없을 때는 -1로 초기화
    public abstract bool TakeDmg(float atk);
    public abstract void DestroyVirus(bool isTimeEndDead = false);  // 사망 (true: 시간 종료로 사망, false: 일반 사망)
    public abstract void Heal(float amount);

    // IInteractable 인터페이스 구현
    public GameObject targetObj => this.gameObject;
    public virtual Sprite TooltipImg => null;
    public string ToolTipDes => "";
    
    public virtual bool IsSelectable => false;
    public virtual bool IsDraggable => false;
    public virtual bool IsTooltipEnabled => TooltipImg != null;


    /// <summary>
    /// 마우스가 객체 위로 올라올 때
    /// </summary>
    public virtual void OnHoverEnter()
    {
       // 필요시 추가 로직
    }

    /// <summary>
    /// 마우스가 객체에서 벗어날 때
    /// </summary>
    public virtual void OnHoverExit()
    {
      // 필요시 추가 로직
    }

    /// <summary>
    /// 마우스 버튼을 누를 때
    /// </summary>
    public virtual void OnClickEnter()
    {
       // 필요시 추가 로직
    }

    /// <summary>
    /// 마우스 버튼을 뗄 때
    /// </summary>
    public virtual void OnClickExit()
    {
       // 필요시 추가 로직
    }

    public virtual void OnBeginDrag()
    {
  
    }

    public virtual void OnDrag(Vector2 mouseDelta)
    {
 
    }

    public virtual void OnEndDrag()
    {

    }

    public virtual void OnClick()
    {
        // 필요시 추가 로직
    }

    public virtual void OnDoubleClick()
    {
        // 필요시 추가 로직
    }

    public virtual void OnRightClick()
    {
        // 필요시 추가 로직
    }

    public virtual void OnSelected(bool isSelected)
    {

    }

    public virtual void OnDragStarted()
    {

    }

    public virtual void OnDragEnded()
    {

    }

    public virtual void OnSelectSingle()
    {
        
    }

    public abstract void SetMoveSpeed(float timeScale = -1);

    public virtual void SetReverseMoveSequence(float duration)
    {
        return;
    }
}
