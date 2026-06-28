using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Skill_WormSaliva : Skill_BossMain
{
    [Header("Settings")]
    [SerializeField] private float shootWorldYOffset = 1.25f;
    
    private Animator animator;
    private Vector2 targetPos;
    private float bossAtk = 100f;

    public void Setup(Animator animator, float atk)
    {
        this.animator = animator;
        this.bossAtk = atk;
    }

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(ShootSalivaState());
    }

    private IEnumerator ShootSalivaState()
    {
        List<File_Base> targetUnit = Managers.GridMgr.ActiveFiles;
        if (targetUnit.Count <= 0)
        {
            FinishPattern();
            yield break;
        }

        int index = Random.Range(0, targetUnit.Count);
        if (targetUnit[index] == null)
        {
            FinishPattern();
            yield break;
        }
        targetPos = targetUnit[index].transform.position;

        animator.Play("Clip_Worm_ShootSaliva");

        float _duration = 0f;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Clip_Worm_ShootSaliva")
            {
                _duration = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(_duration);
        FinishPattern();
    }

    public void ExecuteShoot()
    {
        Managers.soundManager.PlaySfx(Define.SFX.SFX_WormBoss_ShootAtk_01, 1, false);

        Vector2 salivaPos = owner.transform.position;
        salivaPos.y += shootWorldYOffset;

        GameObject _bullet = Managers.Pool.GetBullet(Define.BulletType.Bullet_WormSaliva);
        _bullet.transform.position = salivaPos;

        _bullet.GetComponent<Bullet_WormSaliva>().Init(bossAtk, 10, salivaPos, targetPos, 5f);
    }
}
