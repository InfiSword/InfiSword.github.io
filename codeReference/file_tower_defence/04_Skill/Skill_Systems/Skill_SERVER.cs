using Unity.Mathematics;
using UnityEngine;

public class Skill_SERVER : Skill_SystemMain
{
    private float preAmount;

    public override void SkillSetStat(BaseFileStat fileData)
    {
        base.SkillSetStat(fileData);
        Effect();
    }

    protected override void Effect()
    {
        if (Mathf.Abs(apply_Amount - preAmount) > float.Epsilon)
        {
            if (Managers.nowPlayerData.capacityData.ReviseAddData(Define.CapacityType.Max, preAmount, apply_Amount))
            {
                preAmount = apply_Amount;
            }
        }
    }

    protected override void OnDestroy()
    {
        if (myFile == null || myFile.FileState == FileStatus.Dead)
        {
            Managers.nowPlayerData.capacityData.AddData(Define.CapacityType.Max, -preAmount);
        }
    }
}
