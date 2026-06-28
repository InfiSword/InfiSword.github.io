using DG.Tweening;
using UnityEngine;

public class Skill_MP4 : Skill_AtkMain
{
    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
    }

    protected override void AttackTarget(IHealth targetHealth)
    {
        //총알 생성
        GameObject bullet = Managers.Pool.GetBullet(Define.BulletType.Bullet);
        bullet.GetComponent<Bullet>().Init(0, -1, true);

        //총알 방향, 움직임
        Vector2 dir = (targetHealth.targetObj.transform.position - this.transform.position).normalized;
        bullet.transform.position = this.transform.position;
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.transform.DOMove(targetHealth.targetObj.transform.position, 0.5f).OnComplete(CreateArea);

        //장판 생성
        void CreateArea()
        {
            Transform bullet_MP4 = Managers.Pool.GetBullet(Define.BulletType.Bullet_MP4).transform;
            Vector3 _pos = bullet.transform.position;
            _pos.z = 1;
            bullet_MP4.position = _pos;
            bullet.gameObject.SetActive(false);
            bullet_MP4.GetComponent<Bullet_MP4>().Init(atk, duration, waitTickTime, myFile);
            ApplyBulletDogi(bullet_MP4.gameObject, myFile);
        }
    }
}