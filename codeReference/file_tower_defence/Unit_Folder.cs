using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
/// <summary>
/// 폴더 유닛: 파일을 담는 폴더의 생성/관리 처리 클래스
/// </summary>
public class Unit_Folder : Unit_File
{
    //private List<Unit_File> innerFiles = new List<Unit_File>();
    //public List<Unit_File> InnerFiles => innerFiles;
    public UI_FolderWin myFolderWin;
    public bool isZip = false;
    private Sprite[] folderImgs = new Sprite[3];


    // 파일 상태 변경 시 처리
    protected override void OnFileStateChanged(FileStatus newState)
    {
        base.OnFileStateChanged(newState);

        if (newState == FileStatus.locked || newState == FileStatus.Dead || newState == FileStatus.VisualFile)
        {
            if (isZip == true)
            {
                foreach (Unit_File inner in myFolderWin.GetInnerFiles)
                {
                    inner.RemoveAllBuffs();
                    inner.FileSetStat();
                }
            }
        }
        else if (newState == FileStatus.normal)
        {
            if (isZip == true)
            {
                foreach (Unit_File inner in myFolderWin.GetInnerFiles)
                {
                    inner.FileSetStat();
                }
            }
        }
    }

    public override void Init(Define.Extension extension)
    {
        base.Init(extension);
    }

    protected override IDamageFilter[] GetDamageFilters()
    {
        return GetComponentsInChildren<IDamageFilter>();
    }

    protected override void NewPropertyWin()
    {
        base.NewPropertyWin();
        Debug.Log(myExtension);

        if (!isZip)
        {
            // 폴더창 생성 - 폴더 UI 창을 생성 후 초기화
            myFolderWin = Managers.UI.MakeSubWin<UI_FolderWin>(Managers.UI.GetSceneUI<StageMain>().Windows);
            myFolderWin.ConnectFile(this);
            for (int i = 0; i < 3; i++)
            {
                folderImgs[i] = Resources.Load<Sprite>($"Sprite/File/Folder/folder{i}");
            }
        }

    }
    public override void ApplyBuff(BuffStat _buffStat)
    {
        if (FileState != FileStatus.normal)
            return;

        base.ApplyBuff(_buffStat);

        if (isZip)
        {
            foreach (Unit_File inFile in myFolderWin.GetInnerFiles)
            {
                inFile.ApplyBuff(_buffStat);
            }
        }
    }

    public override void RemoveBuff(BuffStat _buffStat)
    {
        base.RemoveBuff(_buffStat);

        if (isZip)
        {
            foreach (Unit_File inFile in myFolderWin.GetInnerFiles)
            {
                inFile.RemoveBuff(_buffStat);
            }
        }
    }
    public override void DeleteThisFile()
    {
        Managers.Resource.Destroy(this.myFolderWin.gameObject);

        base.DeleteThisFile();
    }

    // 압축(Zip)
    public bool Zip()
    {
        if (!isZip)
        {
            // 폴더 창에 들어있는 파일들 확인
            string innerFileExtensions = myFolderWin.GetInnerExtensions;
            Debug.Log(innerFileExtensions);

            if (innerFileExtensions.ToString() == "")  // 비어 있으면 리턴
                return false;

            // ------------압축 성공--------------
            TutorialEvents.FolderCompressionInvoke();
            isZip = true;
            myExtension = Define.Extension.ZIP;

            // ZIP 아이콘은 대표 떨림(Shake) + 쓰로틀. 내부 파일이 여기로 리다이렉트한다(스펙 권장안 A).
            Motion?.Configure(MotionKind.Shake, _image?.rectTransform, FileMotionController.ZipThrottle);

            // 파티클
            GameObject _particle = Managers.Pool.GetObjParticle(Define.ParticleType.Particle_Obj_Zip);
            _particle.transform.position = transform.position;
            _particle.SetActive(true);

            bool hasCombination = Managers.Data.CombineFileDict.ContainsKey(innerFileExtensions);
            bool isUnlocked = hasCombination ? Managers.Unlock.IsUnlocked(Managers.Data.CombineFileDict[innerFileExtensions].ToString()) : false;

            // 조합이 있을 경우 (상위 파일 생성)
            if (hasCombination && isUnlocked)
            {
                Debug.Log($"파일 조합 압축 실행: {innerFileExtensions} => {Managers.Data.CombineFileDict[innerFileExtensions]}");
                CreateUpperFile(Managers.Data.CombineFileDict[innerFileExtensions]);
            }
            // 조합이 없을 경우 (일반 Zip 파일 생성) 
            else
            {
                Unit_File[] unitFiles = myFolderWin.GetInnerFiles.ToArray();

                for (int i = 0; i < unitFiles.Length; i++)
                {
                    unitFiles[i].file_Skill.enabled = true;
                    unitFiles[i].FileState = FileStatus.normal;

                    Debug.Log($"파일의 HP 계산 확인: {unitFiles[i].myExtension}, 폴더 MAXHP: {this.MAXHP}, 파일 MAXHP: {unitFiles[i].MAXHP}");
                    this.MAXHP += unitFiles[i].MAXHP * 0.75f;
                    this.HP += unitFiles[i].HP * 0.75f;

                    // 위치/스케일 설정
                    unitFiles[i].transform.SetParent(this.transform);
                    unitFiles[i].transform.position = this.transform.position;

                    // 충돌 처리를 비활성화(내부 파일 충돌 방지)
                    unitFiles[i].GetComponent<BoxCollider2D>().enabled = false;
                    unitFiles[i].SetVisualOnOff(false);
                    myFolderWin.gameObject.SetActive(false);

                    unitFiles[i].gameObject.SetActive(true);

                    if (unitFiles[i].file_Skill is Skill_AtkMain _atkSkill)
                    {
                        _atkSkill.StartAttackCoroutine();
                    }
                }
                CurrentGrid.ZipApplyGirdBuffs();
            }

            // 폴더 상태를 Zip 상태로 변환

            // 1. 기본 압축 배율 계산 (기본 0.8에서 업그레이드 횟수와 추가 압축 값을 계산)
            // 두 번째 코드의 로직을 그대로 가져와 기본 배율(baseMult)을 설정합니다.
            float baseMult = 0.8f - UpgradeManager.Instance.CompressionValue;

            // 2. 도기(Dogi) 효과 반영 (곱연산)
            // Zipper 능력을 가진 도기의 수만큼 배율을 거듭제곱하여 곱합니다.
            float finalMult = baseMult * DogiProvider.GetZipperCompressMultiplier();
            finalMult = Mathf.Clamp(finalMult, 0.1f, 0.8f); // 배율이 너무 낮아지는 것을 방지하기 위해 최소값을 설정

            // 3. 최종 데이터 계산 및 적용
            // 소수점 2자리 반올림 후 정수로 변환하여 최종 데이터를 산출합니다.
            float newUseData = (int)Util.RoundToTwoDecimalPlaces(this.useData * finalMult);

            Debug.Log($"압축 배율 계산: baseMult={baseMult}, finalMult={finalMult}, newUseData={newUseData}");

            // 데이터를 갱신합니다.
            Managers.nowPlayerData.capacityData.ReviseAddData(Define.CapacityType.Use, this.useData, newUseData);
            // 트랜스폼 업데이트: 스프라이트/이름/UI 설정
            _image.sprite = folderImgs[2];
            _text.text = "Zip";
            gameObject.name = "Zip";

            // 기록 업데이트
            Managers.Data.RecordData[(int)Define.Record.CompressedCount] += 1;
            useData = newUseData;
            propertyWin?.SetMyInfo();
            return true;
        }

        return false;
    }

    protected override IEnumerator UnitDie()
    {
        myFolderWin.gameObject.SetActive(false);
        yield return base.UnitDie();
    }

    private void CreateUpperFile(Define.Extension _extension)
    {
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        FileGridManager gridMgr = Managers.GridMgr;

        FileGrid originGrid = this.CurrentGrid;
        Unit_File[] innerFiles = myFolderWin.transform.GetComponentsInChildren<Unit_File>(false);

        // 원본 폴더 제거, 관련된 폴더 창 제거        
        Managers.GridMgr.ActiveFiles.Remove(this);
        FileGrid currentGrid = this.CurrentGrid;
        if (currentGrid != null)
            currentGrid.RemoveFileUnit();
        Destroy(myFolderWin.gameObject);
        Destroy(propertyWin.gameObject);

        //if (stageMain.SelectedUnits != null)
        //    stageMain.SelectedUnits.Remove(this);

        Destroy(this.gameObject);

        // 새로운 파일 생성 후 초기화 (Util 팩토리 함수 사용)
        Unit_File upperFile = Util.CreateFileObject(originGrid.transform, _extension);
        if (upperFile == null)
        {
            Debug.LogError("압축 파일 생성 실패");
            return;
        }

        // 그리드에 파일 배치 시도
        if (gridMgr.TrySetUnitAtGrid(upperFile, originGrid))
        {
            Util.FloatingFileDesText(upperFile.myExtension, upperFile.transform.position);
            Managers.GridMgr.ActiveFiles.Add(upperFile);
            upperFile._InputField.onEndEdit.AddListener((text) => upperFile.ApplyText(text));
            upperFile._InputField.gameObject.SetActive(false);
        }

        if (innerFiles != null)
        {
            foreach (var f in innerFiles)
            {
                if (f != null)
                    Destroy(f.gameObject);
            }
        }

        // 7) 기록
        Managers.Data.RecordData[(int)Define.Record.CompressedCount] += 1;

        Debug.Log($"파일 조합 압축 완료: {_extension}");
    }

    public void FolderImg()
    {
        bool _isInFIle = myFolderWin.IsInFile();
        // 파일이 하나 이상 들어있을 때 폴더 이미지
        if (_isInFIle)
            _image.sprite = folderImgs[1];
        // 비어있을 때 기본 폴더 이미지
        else
            _image.sprite = folderImgs[0];
    }

    #region IInteractable Implementation

    public void OpenWin()
    {
        if (isZip)
        {
            propertyWin.SetMyInfo();
            propertyWin.gameObject.SetActive(!propertyWin.gameObject.activeSelf);
        }
        else
        {
            if (myFolderWin != null) { Debug.Log(myFolderWin.gameObject.name); myFolderWin.gameObject.SetActive(!myFolderWin.gameObject.activeSelf); }
        }
    }

    public override void OnSelected(bool _isSelected)
    {
        base.OnSelected(_isSelected);

        if (isZip && !isSelected)
        {
            foreach (var file in myFolderWin.GetInnerFiles)
            {
                file.drawRange?.Show(false);
                if (file.file_Skill is Skill_BuffMain buffSkill)
                    buffSkill.ShowRange(false);
            }
        }
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

        if (isZip)
        {
            foreach (var file in myFolderWin.GetInnerFiles)
            {
                if (file.file_Skill is Skill_BuffMain buffSkill)
                    buffSkill.ShowRange(true);
                else
                    file.drawRange?.Show(true);
            }
        }
    }

    /// <summary>
    /// 드래그된 파일들을 폴더에 드롭 시도
    /// </summary>
    public static bool TryDropToFolder(IInteractable[] dragObjects, UI_FolderWin folderWin)
    {

        bool isTutorialFolderDrop = Managers.myScene == Define.Scene.TutorialScene && Managers.TutorialMgr.tutorialState != TutorialManager.TutorialState.DragDrop;
        if (isTutorialFolderDrop || folderWin == null || folderWin.OwnerFolder == null || folderWin.OwnerFolder.isZip)
            return false;

        bool isSuccess = false;
        foreach (IInteractable obj in dragObjects)
        {
            if (obj is Unit_File unit)
            {
                bool success = folderWin.OnDropToFolderWindow(unit);
                if (success)
                {
                    FileGrid currentGrid = unit.CurrentGrid;
                    if (currentGrid != null)
                        currentGrid.RemoveFileUnit();

                    // 폴더에 들어가면 ActiveFiles에서 제거
                    Managers.GridMgr.ActiveFiles.Remove(unit);

                    isSuccess = success;
                }
            }
        }

        if (isSuccess)
        {
            TutorialEvents.FolderDropedInvoke();
        }

        return isSuccess;
    }

    #endregion
}
