using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 모든 공격 스킬의 기반 클래스입니다.
/// 공격에 필요한 스탯을 직접 소유하고, 주기적으로 적을 탐색하여 공격합니다.
/// </summary>
public abstract class Skill_AtkMain : Skill_Main
{
    public override MotionKind AttackMotion => MotionKind.Lunge;

    [Header("Attack Stats")]
    public float atk { get; set; }
    public float CoolTime;
    public float duration;
    public float waitTickTime;
    public float range;
    protected int maxTargetCount = 0; // 최대 타겟 수 (0 이하면 제한 없음)

    protected virtual string targetTag => "Virus";
    protected LayerMask targetLayer;
    private Coroutine _attackRoutine; // 공격 코루틴 중복 시작 방지용 핸들


    public override void Init(BaseFileStat fileData)
    {
        this.myFile = GetComponent<File_Base>();
        this.myfileExtention = fileData.myExtension;
        targetLayer = LayerMask.GetMask(targetTag);

        SkillSetStat(fileData);
        StartAttackCoroutine();
    }

    public override void SkillSetStat(BaseFileStat fileData)
    {
        if (myFile == null) return;

        // AtkFileStat으로 형변환 확인
        if (!(fileData is AtkFileStat))
        {
            Debug.LogError($"Skill_AtkMain.SkillSetStat: 잘못된 파일 타입입니다. AtkFileStat이 필요하지만 {fileData.GetType().Name}이 전달되었습니다.");
            return;
        }

        // Unit_File에서 이미 버프가 적용된 데이터를 전달받으므로 그대로 사용
        this.atk = fileData.Atk;
        this.CoolTime = fileData.CoolTime;
        
        // 공통 속성 사용
        this.duration = fileData.Duration;
        this.waitTickTime = fileData.WaitTickTime;
        this.range = fileData.Length;
        this.maxTargetCount = fileData.MaxTargetCount;
    }

    /// <summary>
    /// 공격 로직 구현 (일정 시간 마다 타겟 탐색 후 공격하는 중)
    /// </summary>
    /// <param name="target">타겟</param> <summary>
    protected abstract void AttackTarget(IHealth target);

    private void OnEnable()
    {
        if (myFile != null)
            StartAttackCoroutine();  
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _attackRoutine = null;
    }

    public void StartAttackCoroutine()
    {
        if (gameObject.activeInHierarchy)
        {
            // 이미 돌고 있는 공격 코루틴이 있으면 멈추고 다시 시작(중복 시작 → 총알 다발 발사 방지)
            if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            _attackRoutine = StartCoroutine(PeriodicAttackRoutine());
        }
    }
    private IEnumerator PeriodicAttackRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        while (true)
        {
            if ((myFile != null && myFile.FileState == FileStatus.normal)
                && (Managers.newStep != Define.StageState.ReadyState)
                && (myFile.inFolder == null || (myFile.inFolder != null && myFile.inFolder.FileState == FileStatus.normal)))
            {
                List<IHealth> targets = SearchTarget();

                // 사이클당 1회 모션 발동. 가장 가까운 유효 타깃 방향(Shake는 dir 무시).
                for (int t = 0; t < targets.Count; t++)
                {
                    if (!isTargetCorrect(targets[t])) continue;
                    Vector2 motionDir = ((Vector2)targets[t].targetObj.transform.position - (Vector2)transform.position).normalized;
                    myFile?.Motion?.PlayAttackMotion(motionDir);
                    break;
                }

                int tangTangCount = DogiProvider.GetDogiCount(Define.DogiAugment.TangTang);
                int shotsPerTarget = 1 + tangTangCount * DogiValues.TangTangExtraShotsPerDogi;
                const float tangTangDelay = 0.1f;

                for (int i = 0; i < targets.Count; i++)
                {
                    if (!isTargetCorrect(targets[i])) continue;

                    for (int s = 0; s < shotsPerTarget; s++)
                    {
                        if (s > 0)
                            yield return new WaitForSeconds(tangTangDelay);
                        AttackTarget(targets[i]);
                    }
                }
            }
            float waitTime = this.CoolTime > 0.1f ? this.CoolTime : 0.1f;
            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 타겟 검색 메서드 (공통 로직)
    /// </summary>
    protected virtual List<IHealth> SearchTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, Managers.GridMgr.GridDistance * range * 2, 0f, targetLayer);

        // 임시 리스트에 유효한 타겟들을 모두 추가
        List<IHealth> tempTargets = new List<IHealth>();
        foreach (Collider2D collider in colliders)
        {
            if (targetTag == "Virus")
            {
                // 태그가 일치하고, 공격 가능한 상태(canAttacked)인 유닛만 추가
                Unit_VirusBase virus = collider.GetComponent<Unit_VirusBase>();
                if (collider.CompareTag(targetTag) && virus.canAttacked)
                {
                    tempTargets.Add(virus);
                }
            }
            else
            {
                // 태그가 일치하고, 공격 가능한 상태(canAttacked)인 파일만 추가
                File_Base file = collider.GetComponent<File_Base>();
                if (collider.CompareTag(targetTag) && collider.gameObject != this.gameObject)
                {
                    tempTargets.Add(file);
                }
            }
        }

        if (tempTargets.Count > 0)
        {
            // 거리에 따라 오름차순으로 정렬
            tempTargets = tempTargets.OrderBy(t => Vector2.Distance(transform.position, t.targetObj.transform.position))
                                        .Take(maxTargetCount)
                                        .ToList();
        }

        return tempTargets;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, Managers.GridMgr.GridDistance.x * range);
    }

    /// <summary>
    /// 타겟이 유효한지 확인하는 메서드
    /// </summary>
    protected virtual bool isTargetCorrect(IHealth target) =>
        target != null &&
        target.targetObj != null &&
        target.targetObj.activeSelf &&
        target.canAttacked;

    /// <summary>
    /// 도기 버프를 탄환에 일괄 적용 (피해흡혈 소스, 엘리트 추가피해, 탄환 크기)
    /// </summary>
    protected void ApplyBulletDogi(GameObject bulletObj, File_Base sourceFile)
    {
        if (bulletObj == null) return;

        Bullet_File bf = bulletObj.GetOrAddComponent<Bullet_File>();
        bf.sourceFile = sourceFile;
        bf.eliteDamageMultiplier = 1f + DogiValues.EliteHunterDamageRatePerDogi * DogiProvider.GetDogiCount(Define.DogiAugment.EliteHunter);

        if (!bf.isBaseScaleStored)
        {
            bf.baseScale = bulletObj.transform.localScale;
            bf.isBaseScaleStored = true;
        }

        int bigShotCount = DogiProvider.GetDogiCount(Define.DogiAugment.BigShot);
        float multiplier = 1f + (DogiValues.BigShotSizeRatePerDogi * bigShotCount);
        bulletObj.transform.localScale = bf.baseScale * multiplier;
    }
}

