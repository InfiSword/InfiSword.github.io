using UnityEngine;

public class Skill_PING : Skill_AtkMain
{
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
    }

    protected override void AttackTarget(IHealth target)
    {
        //총알 생성
        GameObject bulletPing = Managers.Pool.GetBullet(Define.BulletType.Bullet_PING);
        
        Vector2 dir = ((Vector2)target.targetObj.transform.position - (Vector2)this.transform.position).normalized;
        bulletPing.transform.position = this.transform.position;
        bulletPing.GetComponent<Bullet_PING>().Init(atk, dir, myFile);
        ApplyBulletDogi(bulletPing, myFile);
    }
}