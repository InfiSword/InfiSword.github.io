using DG.Tweening;
using UnityEngine;

public class Skill_WWARD : Skill_AtkMain
{
    public Sprite[] sprites;
    float anim_Time = 2f;

    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
        sprites = Resources.LoadAll<Sprite>("Sprite/Skill/TXT");
    }

    protected override void AttackTarget(IHealth targetHealth)
    {
        GameObject bullet_TXT = Managers.Pool.GetBullet(Define.BulletType.Bullet_TXT);
        int index = UnityEngine.Random.Range(0, sprites.Length);
        bullet_TXT.GetComponent<Bullet_TXT>().Init(atk, 0, sprites[index], myFile);

        Vector2 dir = ((Vector2)targetHealth.targetObj.transform.position - (Vector2)transform.position).normalized;
        bullet_TXT.transform.position = transform.position;
        bullet_TXT.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        ApplyBulletDogi(bullet_TXT, myFile);

        bullet_TXT.transform.gameObject.SetActive(true);
        bullet_TXT.transform.GetComponent<Bullet_TXT>().MoveTween(dir, anim_Time);
    }

}
