using UnityEngine;
using System.Collections;

public class Buff_HP : MyBuff
{
    public override void Init(Transform _target)
    {
        buffTarget = _target; // 버프 타입 설정
        buffParticleType = Define.ParticleType.Particle_Obj_HP;
    }

    // 힐은 합산되지 않음
    public override float GetBuffValue() => addedSumValue / 100f;

    protected override void OnTick(BuffData buffData)
    {
        buffTarget.GetComponent<IHealth>().Heal(buffData.Amount);
    }
}