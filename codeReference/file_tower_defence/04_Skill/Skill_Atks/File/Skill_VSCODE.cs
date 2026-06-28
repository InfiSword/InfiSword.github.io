using UnityEngine;

public class Skill_VSCODE : Skill_AtkMain
{
    private Sprite[] sprites;

    public override void Init(BaseFileStat fileData)
    {
        base.Init(fileData);
        // TXT를 임시로 사용
        sprites = Resources.LoadAll<Sprite>("Sprite/Skill/TXT");
    }

    protected override void AttackTarget(IHealth target)
    {
        //총알 생성
        GameObject bullet_VSCODE = Managers.Pool.GetBullet(Define.BulletType.Bullet_VSCODE);
        int index = UnityEngine.Random.Range(0, sprites.Length);
        bullet_VSCODE.GetComponent<Bullet_VSCODE>().Init(atk, 0, sprites[index], 0, transform.position, null, myFile);

        //총알 위치, 방향
        Vector2 dir = ((Vector2)target.targetObj.transform.position - (Vector2)transform.position).normalized;
        bullet_VSCODE.transform.position = transform.position;
        bullet_VSCODE.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        //총알 움직임
        ApplyBulletDogi(bullet_VSCODE, myFile);
        bullet_VSCODE.transform.gameObject.SetActive(true);
        bullet_VSCODE.transform.GetComponent<Bullet_VSCODE>().MoveTween(dir, 1.5f);
    }
}
