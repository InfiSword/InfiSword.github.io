using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 버프를 주변 영역에 제공하는 '소스' 역할을 수행하는 베이스 클래스입니다.
/// </summary>
public abstract class Skill_BuffMain : Skill_Main
{
    protected Define.BuffType myBuffType;
    public Define.BuffType BuffType => myBuffType;

    public float waitTickTime { get; private set; }
    public float duration { get; private set; }
    public float range { get; private set; }

    public int BuffRadiusX => buffRadiusX;
    public int BuffRadiusY => buffRadiusY;

    public float apply_Amount;
    protected int buffRadiusX;
    protected int buffRadiusY;
    protected string buffMask;

    private IReadOnlyList<Vector2Int> activeOffsets;
    private readonly HashSet<FileGrid> appliedAuraGrids = new HashSet<FileGrid>();
    private bool isShowingRange = false;

    public override void Init(BaseFileStat fileData)
    {
        this.myFile = GetComponent<File_Base>();
        this.myfileExtention = fileData.myExtension;
        SkillSetStat(fileData);
    }

    public override void SkillSetStat(BaseFileStat fileData)
    {
        if (!(fileData is BuffFileStat) && Util.BuffFileExtensions.ContainsKey(myfileExtention))
        {
            Debug.LogError($"버프 파일 오류: {myfileExtention} is not BuffFileStat");
            return;
        }

        float oldAmount = this.apply_Amount;
        float oldWaitTick = this.waitTickTime;

        this.apply_Amount = fileData.Atk;
        this.myBuffType = Util.BuffFileExtensions[myfileExtention];
        this.waitTickTime = fileData.WaitTickTime;
        this.duration = fileData.Duration;

        BuffFileStat buffFile = (BuffFileStat)fileData;
        this.buffRadiusX = buffFile.BuffRadiusX;
        this.buffRadiusY = buffFile.BuffRadiusY > 0 ? buffFile.BuffRadiusY : buffFile.BuffRadiusX;
        this.range = Mathf.Max(this.buffRadiusX, this.buffRadiusY);
        this.buffMask = buffFile.BuffMask;

        this.activeOffsets = buffFile.ActiveOffsets;

        Debug.Log($"[Skill_BuffMain] SkillSetStat: {myfileExtention}, Type: {myBuffType}, Amount: {apply_Amount}, Offsets: {activeOffsets?.Count ?? 0}");

        // 스탯이 변했다면 주변 유닛들에게 정보 갱신
        if (myFile != null && myFile.FileState == FileStatus.normal)
        {
            if (!Util.Approximately(oldAmount, apply_Amount, 0.01f) || !Util.Approximately(oldWaitTick, waitTickTime, 0.01f))
            {
                ApplyBuffArea(true);
            }
        }
    }

    protected override void OnDestroy()
    {
        ClearBuffAura();
    }

    // ==========================================
    // 오라(Aura) 관리 로직
    // ==========================================
    public void ApplyBuffArea(bool apply)
    {
        // 새로운 목표 격자 리스트 계산
        HashSet<FileGrid> targetGrids = new HashSet<FileGrid>();
        FileGrid centerGrid = myFile.CurrentGrid ?? myFile.inFolder?.CurrentGrid;

        // 버프를 적용해야 하고 그리드가 존재하는 경우에만 목표 계산
        if (apply && centerGrid != null && activeOffsets != null)
        {
            FileGridManager gridMgr = Managers.GridMgr;

            foreach (Vector2Int offset in activeOffsets)
            {
                int targetX = centerGrid.GridX + offset.x;
                int targetY = centerGrid.GridY + offset.y;

                if (gridMgr.IsValidGridIndex(targetX, targetY))
                {
                    FileGrid grid = gridMgr.gridArray[targetX, targetY];
                    if (grid != null && grid != centerGrid)
                    {
                        targetGrids.Add(grid);
                    }
                }
            }
        }
    
        // 제거: 목표 리스트에 없는데 현재 적용 중인 격자들 버프 해제
        List<FileGrid> toRemove = new List<FileGrid>();
        foreach (FileGrid g in appliedAuraGrids)
        {
            if (!targetGrids.Contains(g))
                toRemove.Add(g);
        }
        foreach (FileGrid g in toRemove)
        {
            if (g != null)
            {
                g.RemoveBuffSource(myFile);
                g.RemoveBuffHighlightSource(myFile);
            }
            appliedAuraGrids.Remove(g);
        }

        // 추가/갱신: 목표 리스트에 있는 격자들에게 버프 부여 및 갱신
        foreach (FileGrid g in targetGrids)
        {
            if (!appliedAuraGrids.Contains(g))
            {
                g.AddBuffSource(myFile);
                appliedAuraGrids.Add(g);

                // 범위 표시 중이라면 즉시 하이라이트
                if (isShowingRange)
                    g.AddBuffHighlightSource(myFile);
            }
            else
            {
                // 이미 적용 중인 격자라도 스탯 변화를 알리기 위해 다시 호출
                g.AddBuffSource(myFile);
            }
        }
    }

    private void ClearBuffAura()
    {
        ApplyBuffArea(false);
    }

    public void RemoveArea()
    {
        ApplyBuffArea(false);
    }

    // ==========================================
    // 범위 시각화 로직
    // ==========================================
    public override void ShowRange(bool show)
    {
        isShowingRange = show;

        // 현재 실제로 버프를 주고 있는 모든 격자의 하이라이트 상태를 show 값에 맞춰 갱신
        foreach (FileGrid g in appliedAuraGrids)
        {
            if (g == null)
                continue;

            if (show)
                g.AddBuffHighlightSource(myFile);
            else
                g.RemoveBuffHighlightSource(myFile);
        }
    }
}
