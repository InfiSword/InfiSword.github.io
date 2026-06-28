using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit_File : File_Base
{
    public DrawFileSkillLength drawRange { get; private set; }  // 파일 사거리 표시

    public override void Init(Define.Extension extension)
    {
        base.Init(extension);
        if (extension == Define.Extension.DOGI)
        {
            // Unit_Dogi.Init은 File_Base.Init 실행을 위해 의도적으로 base.Init(DOGI)를 호출한다(정상 경로).
            // 따라서 도기 본인이 아니라 '다른' 호출자가 DOGI로 Unit_File.Init을 직접 호출한 경우에만 경고한다.
            if (!(this is Unit_Dogi))
                Debug.LogWarning("도기 파일은 Unit_Dogi.Init(DogiAugment)를 사용하세요.");
            return;
        }
        myExtension = extension;

        BaseFileStat fileData = Managers.Data.FileDates[myExtension];
        transform.name = fileData.Name;
        _text.text = fileData.Name;
        _image.sprite = fileData.sprite;


        // 파일 타입에 따른 애니메이션 설정
        VisualController.SetFileAnimation(extension);

        useData = fileData.UseData;
        garbageData = fileData.GarbageData;

        // 스킬 컴포넌트 생성 및 설정
        file_Skill = Util.CreateSkillComponent(gameObject, myExtension);

        // 기본 스탯 설정(FileType 기반)
        this.MAXHP = fileData.Max_HP;
        this.HP = MAXHP;

        // 스킬 설정 및 초기화: DataManager(FileType) 값을 매개변수로 전달하여 설정(1회)
        if (file_Skill != null)
        {
            file_Skill.myFile = this;
            file_Skill.Init(fileData);

            // 스킬이 선언한 모션 종류로 컨트롤러 구성(확장자 switch 없음).
            Motion?.Configure(file_Skill.AttackMotion, _image?.rectTransform, FileMotionController.DefaultThrottle);

            DrawFileSkillLength skillRangeView = gameObject.GetComponentInChildren<DrawFileSkillLength>(true);
            if (skillRangeView != null)
            {
                skillRangeView.DrawLength(fileData.Length);
                skillRangeView.Show(false);

                drawRange = skillRangeView;
            }
        }

        propertyWin.SetMyInfo();
    }

    public override void FileSetStat()
    {
        // 버프 반영을 위해 현재 BaseFileType + 버프 합산으로 재설정
        var cur = Managers.Data.FileDates[myExtension];

        // 버프가 적용된 새로운 파일 데이터 생성
        BaseFileStat buffedFileData = CreateBuffedFileData(cur);

        // 스킬 스탯 설정
        if (file_Skill != null)
        {
            file_Skill.SkillSetStat(buffedFileData);
        }

        propertyWin?.SetMyInfo();
    }

    /// <summary>
    /// 버프가 적용된 파일 데이터를 생성합니다.
    /// </summary>
    private BaseFileStat CreateBuffedFileData(BaseFileStat originalData)
    {
        // 원본 데이터를 복사하여 버프 적용
        BaseFileStat buffedData = Util.CreateFileType(originalData.myExtension);

        // 공통 속성 복사
        buffedData.myExtension = originalData.myExtension;
        buffedData.sprite = originalData.sprite;
        buffedData.Num = originalData.Num;
        buffedData.Name = originalData.Name;
        buffedData.Power = originalData.Power;

        // 버프 적용된 스탯 설정
        buffedData.Max_HP = originalData.Max_HP;
        buffedData.Atk = originalData.Atk * (1 + buffController.GetBuffValue(Define.BuffType.ATK));
        buffedData.CoolTime = originalData.CoolTime * Mathf.Max(0.1f, 1 - buffController.GetBuffValue(Define.BuffType.CoolTime));
        buffedData.WaitTickTime = originalData.WaitTickTime;
        buffedData.MaxTargetCount = originalData.MaxTargetCount;

        // 도기 효과 적용
        buffedData.Atk = DogiProvider.ApplyAtkDogi(buffedData.Atk);
        buffedData.CoolTime = DogiProvider.ApplyCoolTimeDogi(buffedData.CoolTime);
        buffedData.Max_HP = DogiProvider.ApplyMaxHpDogi(buffedData.Max_HP);

        // 공통 속성 복사
        buffedData.Duration = originalData.Duration;
        buffedData.Length = originalData.Length;
        // 타입별 특수 속성 복사
        if (originalData is BuffFileStat originalBuff && buffedData is BuffFileStat buffedBuff)
        {
            buffedBuff.BuffRadiusX = originalBuff.BuffRadiusX;
            buffedBuff.BuffRadiusY = originalBuff.BuffRadiusY;
            buffedBuff.BuffMask = originalBuff.BuffMask;
            buffedBuff.ActiveOffsets = originalBuff.ActiveOffsets; // 캐싱된 오프셋 복사
            buffedBuff.Atk = DogiProvider.ApplyAtkDogi(originalBuff.Atk);
        }


        return buffedData;
    }

    public virtual void RemoveAllBuffs()
    {
        buffController?.RemoveAllBuffs();
    }

    public override void OnHoverEnter()
    {
        base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
    }

    public override void OnClickEnter()
    {
        base.OnClickEnter();
    }

    public override void OnClickExit()
    {
        base.OnClickExit();
    }

    public override void OnBeginDrag()
    {
        base.OnBeginDrag();

        // 다중 드래그 시 사거리 표시 끄기
        if (Managers.UI.GetSceneUI<StageMain>()?.InteractionHandler.DragObjectsCount > 1)
        {
            drawRange?.Show(false);
            if (file_Skill is Skill_BuffMain buffSkill)
                buffSkill.ShowRange(false);
        }
    }

    public override void OnDrag(Vector2 mouseDelta)
    {
        base.OnDrag(mouseDelta);
    }

    public override void OnEndDrag()
    {
        base.OnEndDrag();
    }

    public override void OnClick()
    {
        base.OnClick();
    }

    public override void OnDoubleClick()
    {
        base.OnDoubleClick();
    }

    public override void OnRightClick()
    {
        base.OnRightClick();
    }

    public override void OnSelectSingle()
    {
        base.OnSelectSingle();
        
        if (file_Skill is Skill_BuffMain buffSkill)
        {
            buffSkill.ShowRange(true);
        }
        else
        {
            drawRange?.Show(true);
        }
    }

    public override void OnSelected(bool _isSelected)
    {
        base.OnSelected(_isSelected);

        if (!_isSelected)
        {
            drawRange?.Show(false);
        }
        else
        {
            // 버프 파일의 경우, 다중 선택 시에는 범위 표시를 하지 않음 (OnSelectSingle에서만 켬)
            if (file_Skill is Skill_BuffMain buffSkill)
                buffSkill.ShowRange(false);
        }
    }
}
