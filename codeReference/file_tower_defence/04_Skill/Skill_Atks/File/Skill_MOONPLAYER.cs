using System.Collections;
using UnityEngine;

public class Skill_MOONPLAYER : Skill_AtkMain
{
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
    }

    protected override void AttackTarget(IHealth _targetHealth)
    {
        GameObject _moonBulluet = Managers.Pool.GetBullet(Define.BulletType.Bullet_MOONPLAYER);
        _moonBulluet.transform.position = _targetHealth.targetObj.transform.position;
        _moonBulluet.GetComponent<Bullet_MOONPLAYER>().Init(atk, duration, waitTickTime, myFile);
        ApplyBulletDogi(_moonBulluet, myFile);
    }
}