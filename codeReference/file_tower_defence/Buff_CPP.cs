using UnityEngine;

public class Buff_ATK : MyBuff
{
    public override void Init(Transform _target)
    {
        buffTarget = _target;
        buffParticleType = Define.ParticleType.Particle_Obj_ATK; 
    }

    public override float GetBuffValue() => Util.RoundToTwoDecimalPlaces(addedSumValue / 100f);

    protected override void OnTick(BuffData buffData)
    {
        return;
    }
}