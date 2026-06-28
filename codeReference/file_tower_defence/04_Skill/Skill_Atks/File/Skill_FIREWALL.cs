using UnityEngine;

public class Skill_FIREWALL : Skill_SystemMain, IDamageFilter
{
    [SerializeField] private float curTime = 0f;
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
        Managers.TimeUpdateEvent -= UpdateDeltaTime;
        Managers.TimeUpdateEvent += UpdateDeltaTime;
    }

    private void UpdateDeltaTime(float deltaTime)
    {
        if (myFile.FileState != FileStatus.normal)
            return;

        curTime += deltaTime;

        if (curTime >= coolTime)
        {
            curTime = 0;
            Effect();
        }
    }

    public float ModifyDamage(float originalDamage)
    {
        if (curTime > 0)
        {
            return originalDamage;
        }

        // TODO: 스킬 발동 파티클이나 사운드 연출 (예: 푸른색 방어막 이펙트)
        // Managers.soundManager.PlaySfx(Define.SFX.SFX_ShieldBlock);

        // 쿨타임 초기화
        curTime = coolTime;

        // 데미지를 50% (절반)으로 깎아서 반환

        return originalDamage * (apply_Amount / 100f);
    }

    protected override void Effect()
    {
        GameObject _particle = Managers.Pool.GetObjParticle(Define.ParticleType.Particle_Obj_FIREWALLShield);
        _particle.transform.SetParent(this.transform);
        _particle.transform.position = transform.position;
        _particle.SetActive(true);
        UnitState.instance.FloatingText("피해 감소!", this.transform.position, false);
    }

    protected override void OnDestroy()
    {
        Managers.TimeUpdateEvent -= UpdateDeltaTime;
    }
}
