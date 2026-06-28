using UnityEngine;

public class Skill_BROWSER : Skill_AtkMain
{
    public override MotionKind AttackMotion => MotionKind.Shake;

    private GameObject[] attackPrefab = new GameObject[3];
    private float [] angles = {90, 225, 315};

    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
    }
    protected override void AttackTarget(IHealth target)
    {
        for (int i = 0; i < 3; i++)
        {
            attackPrefab[i] = Managers.Pool.GetBullet(Define.BulletType.Bullet_BROWSER);
            attackPrefab[i].GetComponent<Bullet_BROWSER>().Init(atk, duration, range, this.transform.position, angles[i], myFile);
            ApplyBulletDogi(attackPrefab[i], myFile);
        }
    }
}
