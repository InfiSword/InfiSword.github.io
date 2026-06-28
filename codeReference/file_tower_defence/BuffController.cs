using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

/// <summary>
/// 유닛의 버프를 관리해주는 컨트롤러.
/// 버프를 별도의 데이터 클래스가 아닌, 실제 버프 효과 컴포넌트(Buff_CPP 등)를 
/// 타겟에 직접 부착(AddComponent)하여 관리합니다.
/// </summary>
public class BuffController : MonoBehaviour
{
    private File_Base _owner;

    public void Init(File_Base owner)
    {
        _owner = owner;
    }

    public float GetBuffValue(Define.BuffType buffType)
    {
        float sum = 0;
        // Buff_Base 타입만 찾아 합산하므로 소스 스킬과 분리됨
        Buff_Base[] attachedBuffs = GetComponents<Buff_Base>();
        
        foreach (var buff in attachedBuffs)
        {
            if (buff != null && !buff.IsRemoving && buff.BuffType == buffType)
            {
                sum += buff.Amount;
            }
        }
        
        return Util.RoundToTwoDecimalPlaces(sum / 100f);
    }

    public bool ApplyBuff(BuffStat stat)
    {
        Buff_Base[] attachedBuffs = GetComponents<Buff_Base>();
        // 삭제 중인 버프는 무시하고 실제 유효한 버프만 찾음
        Buff_Base existing = Array.Find(attachedBuffs, b => b != null && !b.IsRemoving && b.SourceID == stat.instanceID && b.BuffType == stat.buffType);

        bool changed = false;
        if (existing != null)
        {
            if (!Util.Approximately(existing.Amount, stat.amount, 0.1f) ||
                !Util.Approximately(existing.WaitTickTime, stat.waitTickTime, 0.1f))
            {
                existing.Refresh(stat);
                changed = true;
            }
        }
        else
        {
            Type skillType = Util.GetSkillTypeForBuff(stat.buffType);
            if (skillType != null)
            {
                Buff_Base newBuff = gameObject.AddComponent(skillType) as Buff_Base;
                if (newBuff != null)
                {
                    newBuff.Init(stat);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            _owner?.FileSetStat();
        }

        return changed;
    }

    /// <summary>
    /// 모든 버프 삭제 로직이 통합된 유일한 엔트리 포인트입니다.
    /// </summary>
    public bool RemoveBuff(BuffStat stat)
    {
        Buff_Base[] attachedBuffs = GetComponents<Buff_Base>();
        Buff_Base target = Array.Find(attachedBuffs, b => !b.IsRemoving && b.SourceID == stat.instanceID && b.BuffType == stat.buffType);
        
        if (target != null)
        {
            target.SetRemoving(); 
            Destroy(target);
            _owner?.FileSetStat();
            return true;
        }
        return false;
    }

    public void RemoveAllBuffs()
    {
        Buff_Base[] attachedBuffs = GetComponents<Buff_Base>();
        foreach (var buff in attachedBuffs)
        {
            if (buff != null)
            {
                buff.SetRemoving();
                Destroy(buff);
            }
        }
    }
}