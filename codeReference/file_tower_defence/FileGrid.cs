using System.Collections.Generic;
using UnityEngine;

public class FileGrid : MonoBehaviour
{
    [Header("State")]
    private File_Base fileUnit;
    public int GridX { get; private set; }
    public int GridY { get; private set; }

    [Header("Visuals")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0f); // 기본 색상 (투명)

    private readonly HashSet<File_Base> _activeBuffSources = new HashSet<File_Base>();  // 현재 그리드에 적용시키고 있는 버프 파일들
    
    public GameObject obstacleObject {get; private set;}

    public void Init(int _x, int _y)
    {
        fileUnit = null;
        GridX = _x;
        GridY = _y;        
        obstacleObject = null;
    }
    public File_Base GetFileUnit() => fileUnit;

    public void SetFileUnit(File_Base unit, bool isReturn = false)
    {
        if (unit == null)
        {
            Debug.LogWarning("SetFileUnit: unit이 null입니다.");
            return;
        }

        if (!isReturn)
        {
            if (fileUnit != null && unit != fileUnit)
            {
                Debug.LogWarning($"SetFileUnit: 그리드 ({GridX}, {GridY})이 이미 점유되어 있습니다.");
                return;
            }
            fileUnit = unit;
            // 유닛의 현재 그리드 참조 업데이트
            unit.CurrentGrid = this;
            if (unit is Unit_Folder _folder && _folder.isZip)
            {
                foreach (var inFile in _folder.myFolderWin.GetInnerFiles)
                {
                    inFile.CurrentGrid = this;
                }
            }
        }


        // 파일이 그리드에 배치될 때, 이미 배치된 버프 소스들을 즉시 적용
        ApplyGridBuffs(true);
        ApplyBuffActive(true);
    }

    public void RemoveFileUnit()
    {
        if (fileUnit != null)
        {
            // 파일이 그리드에서 제거될 때, 해당 파일에 적용된 모든 버프를 제거
            ApplyGridBuffs(false);

            // 버프 파일이면 기존 영역 제거
            ApplyBuffActive(false);

            // 유닛의 현재 그리드 참조 제거
            fileUnit.CurrentGrid = null;
            if (fileUnit is Unit_Folder _folder && _folder.isZip)
            {
                foreach (var inFile in _folder.myFolderWin.GetInnerFiles)
                {
                    inFile.CurrentGrid = null;
                }
            }
            fileUnit = null;
        }
    }

    // 버프 소스 추가: 현재 그리드에 파일이 있으면 즉시 적용
    public void AddBuffSource(File_Base source)
    {
        if (source == null)
            return;

        if (!_activeBuffSources.Contains(source))
        {
            _activeBuffSources.Add(source);
        }

        if (fileUnit != null)
        {
            ApplyBuffFromSource(fileUnit, source, true);
        }
    }

    // 버프 소스 제거: 현재 그리드에 파일이 있으면 즉시 제거
    public void RemoveBuffSource(File_Base source)
    {
        if (source == null)
            return;

        if (_activeBuffSources.Contains(source))
        {
            _activeBuffSources.Remove(source);
        }

        if (fileUnit != null)
        {
            ApplyBuffFromSource(fileUnit, source, false);
        }
    }

    // 실제 버프 적용/제거 로직
    private void ApplyBuffFromSource(File_Base target, File_Base source, bool apply)
    {
        if (target == null || source == null || target == source)
            return;
        
        // 버프 파일 자신의 그리드에는 버프 적용 금지
        if (source.CurrentGrid == this)
            return;

        Skill_BuffMain buffSkill = source.file_Skill as Skill_BuffMain;
        if (buffSkill == null)
            return;

        BaseFileStat fileData = Managers.Data.FileDates[source.myExtension];
        float amount = fileData.Atk;
        float waitTickTime = fileData.WaitTickTime;

        // TODO 버프 객체 고민
        if (apply)
        {
            BuffStat _buffStat = new BuffStat(buffSkill.BuffType, source.GetInstanceID(), amount, -1f, waitTickTime);
            target.ApplyBuff(_buffStat);
        }
        else
        {
            BuffStat _buffStat = new BuffStat(buffSkill.BuffType, source.GetInstanceID(), amount, 0, waitTickTime);
            target.RemoveBuff(_buffStat);
        }
    }  

    // 장애물 관리 메서드들
    public void SetObstacle(GameObject obstacle)
    {
        if (obstacle == null)
        {
            Debug.LogWarning("SetObstacle: obstacle이 null입니다.");
            return;
        }
        obstacleObject = obstacle;
    }

    public void MoveObstacle(FileGrid fileGrid)
    {
        if (obstacleObject != null)
        {
            fileGrid.SetObstacle(obstacleObject);
            obstacleObject = null;
        }
    }

    public void RemoveObstacle()
    {
        if (obstacleObject != null)
        {
            Destroy(obstacleObject);
            obstacleObject = null;
        }
    }

    private void ApplyGridBuffs(bool apply)
    {
        foreach (File_Base source in _activeBuffSources)
        {
            ApplyBuffFromSource(fileUnit, source, apply);
        }
    }

    private void ApplyBuffActive(bool apply)
    {
        // 버프 파일이면 새 영역 적용
        if (fileUnit.file_Skill is Skill_BuffMain Buff)
        {
            Buff.ApplyBuffArea(apply);
        }

        // 압축 폴더일 시 안의 버프들 새 영역 적용  
        if (fileUnit is Unit_Folder folder && folder.isZip == true)
        {
            foreach (var inFile in folder.myFolderWin.GetInnerFiles)
            {
                if (inFile.file_Skill is Skill_BuffMain innerBuffSkill)
                {
                    innerBuffSkill.ApplyBuffArea(apply);
                }
            }
        }
    }

    // ZIP 되는 순간에 버프 적용하기 위한 접근용 함수
    public void ZipApplyGirdBuffs()
    {
        ApplyGridBuffs(true);
        ApplyBuffActive(true);
    }

    /// <summary>
    /// HP 버프를 제외한 모든 그리드 버프를 제거하고, 자신의 오라를 정지합니다. (감염 시 사용)
    /// </summary>
    public void ZipRemoveNonHPBuffs()
    {
        if (fileUnit == null) return;

        foreach (File_Base source in _activeBuffSources)
        {
            Skill_BuffMain buffSkill = source.file_Skill as Skill_BuffMain;
            if (buffSkill != null && buffSkill.BuffType != Define.BuffType.HP)
            {
                ApplyBuffFromSource(fileUnit, source, false);
            }
        }

        ApplyBuffActive(false);
    }

    public void ZipRemoveGridBuffs()
    {
        ApplyGridBuffs(false);
        ApplyBuffActive(false);
    }
}
