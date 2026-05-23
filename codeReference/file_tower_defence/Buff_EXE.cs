using UnityEngine;

public class Buff_CoolTime : MyBuff
{
    public override void Init(Transform _target)
    {
        buffTarget = _target; // 버프 타입 설정
        buffParticleType = Define.ParticleType.Particle_Obj_CoolTime; 
    }

    public override float GetBuffValue() => Util.RoundToTwoDecimalPlaces(addedSumValue / 100f);

    protected override void OnTick(BuffData buffData)
    {
        return; 
    }
}