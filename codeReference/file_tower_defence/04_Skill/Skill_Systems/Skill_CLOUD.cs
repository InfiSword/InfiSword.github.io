using UnityEngine;

public class Skill_CLOUD : Skill_SystemMain
{
    [SerializeField] private float curTime;
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
        Managers.TimeUpdateEvent -= UpdateDeltaTime;
        Managers.TimeUpdateEvent += UpdateDeltaTime;
    }

    protected override void Effect()
    {


        // 파티클
        GameObject _particle = Managers.Pool.GetObjParticle(Define.ParticleType.Particle_Obj_Cloud);
        _particle.transform.position = transform.position;
        _particle.SetActive(true);

        // 텍스트 UI
        UnitState.instance.FloatingText("용량 증가!", this.transform.position, false);

        Managers.nowPlayerData.capacityData.AddData(Define.CapacityType.Max, apply_Amount);
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

    protected override void OnDestroy()
    {
        Managers.TimeUpdateEvent -= UpdateDeltaTime;
    }
}
