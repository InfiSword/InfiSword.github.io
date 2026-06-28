using UnityEngine;

public class Skill_WEB : Skill_AtkMain
{
    public override MotionKind AttackMotion => MotionKind.Shake;

    public override void Init(BaseFileStat data)
    {
        base.Init(data);
    }
    protected override void AttackTarget(IHealth target)
    {
        GameObject bullet_web = Managers.Pool.GetBullet(Define.BulletType.Bullet_WEB);

        Vector2 dir = ((Vector2)target.targetObj.transform.position - (Vector2)transform.position).normalized;
        bullet_web.transform.position = this.transform.position;
        bullet_web.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        bullet_web.GetComponent<Bullet_WEB>().Init(atk, 0, 0.5f, dir, myFile);
        ApplyBulletDogi(bullet_web, myFile);
    }
}
