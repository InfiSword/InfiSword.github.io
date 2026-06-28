using UnityEngine;
using System;

/// <summary>
/// 보스 패턴(패턴형 스킬)의 기반이 되는 클래스입니다.
/// </summary>
public abstract class Skill_BossMain : MonoBehaviour
{
    public event Action OnPatternComplete;
    protected bool isRunning = false;
    public bool IsRunning => isRunning;

    protected Unit_VirusBase owner;

    public virtual void Init(Unit_VirusBase owner)
    {
        this.owner = owner;
    }

    public abstract void StartPattern();
    
    public virtual void StopPattern()
    {
        isRunning = false;
    }

    protected void FinishPattern()
    {
        isRunning = false;
        OnPatternComplete?.Invoke();
    }
}
