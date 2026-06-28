using UnityEngine;

public class Skill_DAW : Skill_AtkMain
{
    Bullet_DAW bullet_daw;
    public override void Init(BaseFileStat data)
    {
        base.Init(data);
    }
    protected override void AttackTarget(IHealth target)
    {
        if (bullet_daw != null && bullet_daw.gameObject.activeSelf)
            return;

        bullet_daw = Managers.Pool.GetBullet(Define.BulletType.Bullet_DAW).GetComponent<Bullet_DAW>();
        bullet_daw.Init(atk, GetComponent<File_Base>(), target);
        ApplyBulletDogi(bullet_daw.gameObject, myFile);
    }
}