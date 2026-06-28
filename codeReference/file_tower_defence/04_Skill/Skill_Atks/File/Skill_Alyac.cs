using UnityEngine;

public class Skill_ALYAC : Skill_AtkMain
{
    protected override string targetTag => "Files";
    protected override void AttackTarget(IHealth target)
    {
        // TODO: 알약 투사체(Projectile) 생성 및 발사 연출
        GameObject bullet_ALYAC = Managers.Pool.GetBullet(Define.BulletType.Bullet_ALYAC);
        Bullet_ALYAC bulletScript = bullet_ALYAC.GetComponent<Bullet_ALYAC>();

        bulletScript.Init(atk, 0, myFile);
     
        Vector2 dir = ((Vector2)target.targetObj.transform.position - (Vector2)transform.position).normalized;
        bullet_ALYAC.transform.position = transform.position;
        bullet_ALYAC.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        ApplyBulletDogi(bullet_ALYAC, myFile);

        bullet_ALYAC.transform.gameObject.SetActive(true);
        bulletScript.MoveTween(dir, 1.5f);
    }
}
