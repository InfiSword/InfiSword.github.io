using DG.Tweening;
using UnityEngine;

public class Skill_PNG : Skill_AtkMain
{
    public Sprite[] sprites;

    private void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprite/Skill/PNG");
    }

    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
    }

    protected override void AttackTarget(IHealth targetHealth)
    {
        GameObject bullet = Managers.Pool.GetBullet(Define.BulletType.Bullet_PNGShoot);
        int index = UnityEngine.Random.Range(0, sprites.Length);
        bullet.GetComponent<Bullet>().Init(0, -1, true, sprites[index]);

        Vector2 dir = ((Vector2)targetHealth.targetObj.transform.position - (Vector2)this.transform.position).normalized;
        bullet.transform.position = this.transform.position;
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        
        bullet.transform.DOMove(targetHealth.targetObj.transform.position, 0.5f).OnComplete(CreatePNG);

        void CreatePNG()
        {
            bullet.gameObject.SetActive(false);
            
            GameObject bullet_PNG = Managers.Pool.GetBullet(Define.BulletType.Bullet_PNG);
            
            bullet_PNG.transform.position = bullet.transform.position;
            bullet_PNG.GetComponent<Bullet_PNG>().Init(atk, sprites[index], index, myFile);
            ApplyBulletDogi(bullet_PNG, myFile);
        }
    }
}