using System.Collections.Generic;
using UnityEngine;

public abstract class Skill_Main : MonoBehaviour
{
    public File_Base myFile;
    public Define.Extension myfileExtention; // 스킬 타입

    /// <summary>이 스킬이 발동 시 사용할 모션 종류(OCP). 기본 None.</summary>
    public virtual MotionKind AttackMotion => MotionKind.None;

    public abstract void Init(BaseFileStat fileData);

    public abstract void SkillSetStat(BaseFileStat fileData);

    /// <summary>
    /// 범위를 시각적으로 표시하거나 숨깁니다.
    /// </summary>
    public virtual void ShowRange(bool show) { }

    /// <summary>
    /// 오브젝트가 파괴될 때 호출되는 함수
    /// </summary>
    protected virtual void OnDestroy()
    {

    }
}