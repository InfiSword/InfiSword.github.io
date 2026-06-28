using DG.Tweening;
using UnityEngine;

public class Skill_TXT : Skill_AtkMain
{
    private Sprite[] sprites;

    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
        sprites = Resources.LoadAll<Sprite>("Sprite/Skill/TXT");
    }

    protected override void AttackTarget(IHealth target)
    {
        //총알 생성
        GameObject bullet_TXT = Managers.Pool.GetBullet(Define.BulletType.Bullet_TXT);
        int index = UnityEngine.Random.Range(0, sprites.Length);
        bullet_TXT.GetComponent<Bullet_TXT>().Init(atk, 0, sprites[index], myFile);

        Vector2 dir = ((Vector2)target.targetObj.transform.position - (Vector2)transform.position).normalized;
        bullet_TXT.transform.position = transform.position;
        bullet_TXT.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        ApplyBulletDogi(bullet_TXT, myFile);

        bullet_TXT.transform.gameObject.SetActive(true);
        bullet_TXT.transform.GetComponent<Bullet_TXT>().MoveTween(dir, 1.5f);
    }
}
