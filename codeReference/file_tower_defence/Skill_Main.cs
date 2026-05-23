using System.Collections.Generic;
using UnityEngine;

public abstract class Skill_Main : MonoBehaviour
{
    public File_Base myFile;
    public Define.Extension myfileExtention; // 스킬 타입

    public abstract void Init(BaseFileStat fileData);

    public abstract void SkillSetStat(BaseFileStat fileData);

    /// <summary>
    /// 오브젝트가 파괴될 때 호출되는 함수
    /// </summary>
    protected virtual void OnDestroy()
    {

    }
}