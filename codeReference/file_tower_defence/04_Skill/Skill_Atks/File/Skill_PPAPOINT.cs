using DG.Tweening;
using UnityEngine;

public class Skill_PPAPOINT : Skill_AtkMain
{
    float anim_Time = 1.5f;
    private Sprite [] sprites;
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
        sprites = Resources.LoadAll<Sprite>("Sprite/Skill/PPAPOINT");
    }

    protected override void AttackTarget(IHealth targetHealth)
    {
        //총알 생성
        GameObject bullet_PPAPOINT = Managers.Pool.GetBullet(Define.BulletType.Bullet_PPAPOINT);
        int index = UnityEngine.Random.Range(0, sprites.Length);

        //총알 위치, 방향
        Vector2 dir = ((Vector2)targetHealth.targetObj.transform.position - (Vector2)transform.position).normalized;
        bullet_PPAPOINT.transform.position = transform.position;
        bullet_PPAPOINT.transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);

        //총알 움직임
        bullet_PPAPOINT.transform.gameObject.SetActive(true);
        bullet_PPAPOINT.GetComponent<Bullet_PPAPOINT>().Init(atk, dir, anim_Time, sprites[index], myFile);
        ApplyBulletDogi(bullet_PPAPOINT, myFile);
    }
}
