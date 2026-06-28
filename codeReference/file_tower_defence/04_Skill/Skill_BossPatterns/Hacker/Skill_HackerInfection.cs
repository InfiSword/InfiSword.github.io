using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Skill_HackerInfection : Skill_BossMain
{
    private LineRenderer linePrefab;
    
    private Animator animator;
    private List<HackerFileInfection> activeInfections = new List<HackerFileInfection>();
    
    private bool canTriggerInfection = false;

    // 단 하나라도 파일이 감염되어 파괴되었는지, 해제되었는지 확인하는 플래그
    private bool isAnyExpired = false;

    public void Setup(Animator animator, LineRenderer linePrefab)
    {
        this.animator = animator;
        this.linePrefab = linePrefab;
    }

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(Pattern_HackerInfection());
    }

    private IEnumerator Pattern_HackerInfection()
    {
        canTriggerInfection = false;
        isAnyExpired = false;
        Debug.Log("[HackerSkill] Pattern 1 시작: 바이러스 감염");

        animator.Play("VirusInfection_FingerSnap");
        yield return new WaitUntil(() => canTriggerInfection);

        List<File_Base> candidates = Managers.GridMgr.ActiveFiles
            .Where(f => f != null && f.FileState == FileStatus.normal && !f.IsInfected)
            .ToList();

        int targetCount = Mathf.Min(candidates.Count, Random.Range(5, 7));
        if (targetCount == 0)
        {
            FinishPattern();
            yield break;
        }

        candidates = candidates.OrderBy(x => System.Guid.NewGuid()).Take(targetCount).ToList();
        foreach (var file in candidates)
        {
            HackerFileInfection infection = file.gameObject.AddComponent<HackerFileInfection>();
            infection.SetOwner((VirusBoss_Hacker)owner, linePrefab);
            activeInfections.Add(infection);
        }

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("VirusInfection_FingerSnap") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        animator.Play("Laugh");

        float elapsed = 0f;
        while (elapsed < 30f && activeInfections.Count > 0)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        bool clearedNormally = (activeInfections.Count == 0 && elapsed < 30f && !isAnyExpired);
        bool isTimeOut = (elapsed >= 30f && activeInfections.Count > 0);
        
        ClearLinesAndInfections(isTimeOut);
        
        FinishPattern();
    }

    public void OnTriggerInfection() => canTriggerInfection = true;

    public void NotifyInfectionCleared(HackerFileInfection infection)
    {
        RemoveInfection(infection, false);
    }

    public void NotifyInfectionExpired(HackerFileInfection infection)
    {
        RemoveInfection(infection, true);
    }

    private void RemoveInfection(HackerFileInfection infection, bool isExpired)
    {
        if (isExpired)
            isAnyExpired = true;

        if (infection != null)
        {
            activeInfections.Remove(infection);
        }
    }

    private void ClearLinesAndInfections(bool isTimeOut = false)
    {
        var remaining = activeInfections.ToList();
        foreach (var infection in remaining)
        {
            if (infection != null)
            {
                if (isTimeOut)
                {
                    // 시간 초과 시 파일 파괴
                    if (infection.TargetFile != null)
                    {
                        infection.TargetFile.DeleteThisFile();
                    }
                }
                else
                {
                    // 그 외 강제 종료 시 일반 정화 치료
                    infection.ForceClear();
                }
            }
        }
        activeInfections.Clear();
    }

    public override void StopPattern()
    {
        base.StopPattern();
        ClearLinesAndInfections();
    }
}
