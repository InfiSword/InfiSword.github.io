using System.Collections.Generic;
using UnityEngine;
using System;

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
    protected Define.BuffRangeShape buffShape;

    // protected int buffInnerRadius;
    // protected int buffDirection;

    // 현재 버프 파일이 버프를 적용 중인 그리드들의 목록
    private readonly HashSet<FileGrid> appliedAuraGrids = new HashSet<FileGrid>();

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
            Debug.LogError("버프 파일 오류");
            return;
        }

        this.apply_Amount = fileData.Atk;
        this.myBuffType = Util.BuffFileExtensions[myfileExtention];
        this.waitTickTime = fileData.WaitTickTime;
        this.duration = fileData.Duration;

        BuffFileStat buffFile = (BuffFileStat)fileData;
        this.buffShape = buffFile.ParsedBuffShape;
        this.buffRadiusX = buffFile.BuffRadiusX;
        this.buffRadiusY = buffFile.BuffRadiusY > 0 ? buffFile.BuffRadiusY : buffFile.BuffRadiusX;
        this.range = Mathf.Max(this.buffRadiusX, this.buffRadiusY);
    }

    protected override void OnDestroy()
    {
        // 씬 종료 때 버프 제거 시 오류 발생하여 추가
        if (Managers.isInstanceNull)
            return;

        ClearBuffAura();
    }

    /// <summary>
    /// 버프 영역을 적용합니다 (배치/이동 시 호출)
    /// </summary>
    public void ApplyBuffArea(bool apply)
    {   
        FileGrid myGrid = myFile.CurrentGrid ?? myFile.inFolder?.CurrentGrid;
        if (myGrid == null && myFile.inFolder == null)
        {
            RemoveArea();
            return;
        }

        // 기존 영역 제거
        RemoveArea();
        
        // 버프 적용
        if (apply)
        {
            // 새 영역 계산 및 적용
            foreach (FileGrid grid in SelectGridsByShape(myGrid))
            {
                appliedAuraGrids.Add(grid);
                grid.AddBuffSource(myFile); // 그리드에 파일이 있으면 자동으로 버프 적용
            }
        }

    }

    private IEnumerable<FileGrid> SelectGridsByShape(FileGrid centerGrid)
    {
        var gridMgr = Managers.GridMgr;
        if (gridMgr == null)
            yield break;

        switch (this.buffShape)
        {
            case Define.BuffRangeShape.Square:
            default:
                for (int x = centerGrid.GridX - this.buffRadiusX; x <= centerGrid.GridX + this.buffRadiusX; x++)
                    for (int y = centerGrid.GridY - this.buffRadiusY; y <= centerGrid.GridY + this.buffRadiusY; y++)
                    {
                        if (gridMgr.IsValidGridIndex(x, y))
                        {
                            FileGrid grid = gridMgr.gridArray[x, y];
                            if (grid != centerGrid)
                                yield return grid;
                        }
                    }
                break;
        }
    }

    private void ClearBuffAura()
    {
        List<FileGrid> gridsToProcess = new List<FileGrid>(appliedAuraGrids);
        foreach (var g in gridsToProcess)
        {
            g.RemoveBuffSource(myFile);
        }
        appliedAuraGrids?.Clear();
    }


    public void RemoveArea()
    {
        foreach (FileGrid g in new List<FileGrid>(appliedAuraGrids))
        {
            g.RemoveBuffSource(myFile);
        }
        appliedAuraGrids.Clear();
    }

}
