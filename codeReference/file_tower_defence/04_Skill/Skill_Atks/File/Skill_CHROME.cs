using UnityEngine;

public class Skill_CHROME : Skill_AtkMain
{
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
    }
    protected override void AttackTarget(IHealth target)
    {
        Debug.Log(Define.BulletType.Bullet_CHROME.ToString());
        GameObject chromeBullet = Managers.Pool.GetBullet(Define.BulletType.Bullet_CHROME);
        Debug.Log(chromeBullet.name);
        chromeBullet.GetComponent<Bullet_CHROME>().Init(atk, this.transform.position, myFile);
        ApplyBulletDogi(chromeBullet, myFile);
        chromeBullet.transform.SetParent(this.transform);
    }
}
