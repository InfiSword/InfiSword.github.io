using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// 파일 상태(시각화, 정상, 잠김, 죽음)
/// </summary>
public enum FileStatus { VisualFile, normal, locked, Dead }
public abstract class File_Base : MonoBehaviour, IHealth, IInteractable, IInfectable    
{
    [Header("Unit Info")]
    protected FileStatus _fileState = FileStatus.normal;
    public FileStatus FileState
    {
        get => _fileState;
        set
        {
            if (_fileState != value)
            {
                _fileState = value;
                OnFileStateChanged(_fileState);
            }
        }
    }
    public BuffController buffController;

    [Header("File Stat")]
    public float useData;
    public float garbageData;
    protected float _maxHp;
    protected float _hp;
    public virtual float MAXHP
    {
        get { return _maxHp; }
        protected set
        {
            _maxHp = value;
            HP = Mathf.Min(HP, _maxHp); // 최대 HP가 줄어들면 현재 HP도 줄어들도록 맞춤
        }
    }

    public virtual float HP
    {
        get { return _hp; }
        protected set
        {
            if (FileState == FileStatus.Dead)
                return;

            _hp = Mathf.Clamp(value, 0, _maxHp);
            // 머티리얼이 아닌 CurHPImage의 FillAmount로 현재 체력 표시
            if (_hpImage != null)
                _hpImage.fillAmount = _maxHp > 0f ? Mathf.Clamp01(_hp / _maxHp) : 0f;
        }
    }

    [Header("Event UI")]
    protected File_VisualController VisualController; // TODO 파일 외형 변환에 대한 외부 접근 제공 함수 만들기
    public Canvas _unitCanvas => VisualController?._unitCanvas;
    public Image _backgroundImage => VisualController?._backgroundImage;
    public Image _image => VisualController?._image;
    public Image _hpImage => VisualController?._hpImage;
    public TMP_Text _text => VisualController?._text;
    public TMP_InputField _InputField => VisualController?._InputField;

    [Header("Connect Data")]
    public UI_PropertyWin propertyWin;

    protected event Action filePropertyWindowToggled;

    public Define.Extension myExtension;
    public Unit_Folder inFolder;
    public Skill_Main file_Skill;

    /// <summary>파일 모션 단일 진입점(SRP). File_Base.Init에서 생성된다.</summary>
    public FileMotionController Motion { get; private set; }

    // 현재 점유한 그리드 참조
    public FileGrid CurrentGrid;
    private FileGrid originalGridBeforeDrag; // 드래그 시작 시 원래 그리드 저장

    // IInteractable 인터페이스 구현
    protected bool isSelected = false;

    public bool IsInfected => FileState == FileStatus.locked && GetComponents<VirusInfection>().Length > 0;
    public virtual bool IsSelectable => FileState == FileStatus.normal;
    public virtual bool IsDraggable => FileState == FileStatus.normal;
    public GameObject targetObj => this.gameObject;
    public bool canAttacked => FileState == FileStatus.normal;
    public Sprite TooltipImg => _image.sprite;
    public bool IsTooltipEnabled => true;

    [Header("Infection Status")]
    private int currentCureStack = 0;
    private int maxCureStack = 10; // 10대 맞으면 치료

    // 해커 보스 감염 게이지(HackerFileInfection 용). cure-stack과 별개로 사용.
    public float currentInfectionGauge { get; protected set; } = 0f;
    public float maxInfectionGauge { get; protected set; } = 0f;
    public event System.Action<float, float> OnInfectionGaugeChanged;

    public void SetInfectionGauge(float max)
    {
        maxInfectionGauge = max;
        currentInfectionGauge = max;
        OnInfectionGaugeChanged?.Invoke(currentInfectionGauge, maxInfectionGauge);
    }

    /// <summary>감염 게이지 감소(0이면 InvokeFullStack로 치료 완료). 큐어 트리거에서 호출.</summary>
    public void ReduceInfectionGauge(float amount)
    {
        if (maxInfectionGauge <= 0f) return;
        currentInfectionGauge = Mathf.Max(0f, currentInfectionGauge - amount);
        OnInfectionGaugeChanged?.Invoke(currentInfectionGauge, maxInfectionGauge);
        if (currentInfectionGauge <= 0f) InvokeFullStack();
    }

    public string ToolTipDes => Util.ExtentionToDescription(myExtension);

    public virtual void Init()
    {
        VisualController = GetComponentInChildren<File_VisualController>();
        VisualController.InitVisuals(myExtension);

        Motion = GetComponent<FileMotionController>();
        if (Motion == null) Motion = gameObject.AddComponent<FileMotionController>();
        Motion.AttachOwner(this);

        buffController = GetComponent<BuffController>();
        buffController?.Init(this);
        NewPropertyWin();
    }

    // 파일 확장자(Extension)를 받는 Init 메서드
    public virtual void Init(Define.Extension extension)
    {
        myExtension = extension;
        Init();
    }

    #region Event

    // 파일 속성창을 생성/연결 처리
    protected virtual void NewPropertyWin()
    {
        switch (myExtension)
        {
            case Define.Extension.FOLDER:
                propertyWin = Managers.UI.MakeSubWin<UI_FolderPropertyWin>(Managers.UI.GetSceneUI<StageMain>().Windows);
                break;
            default:
                propertyWin = Managers.UI.MakeSubWin<UI_FileWin>(Managers.UI.GetSceneUI<StageMain>().Windows);
                break;
        }
        propertyWin.ConnectFile(this);
    }


    // 파일 상태 변경 시 처리
    protected virtual void OnFileStateChanged(FileStatus newState)
    {
        if (newState == FileStatus.locked || newState == FileStatus.Dead || newState == FileStatus.VisualFile)
        {
            if (file_Skill is Skill_BuffMain buffSkill)
                buffSkill.ApplyBuffArea(false);

            if (newState == FileStatus.locked)
            {
                CurrentGrid?.ZipRemoveNonHPBuffs();
            }
            else
            {
                CurrentGrid?.ZipRemoveGridBuffs();
                buffController.RemoveAllBuffs();
            }

            FileSetStat();
            propertyWin?.SetMyInfo();
        }
        else if (newState == FileStatus.normal)
        {
            if (file_Skill is Skill_BuffMain buffSkill)
                buffSkill.ApplyBuffArea(true);

            CurrentGrid?.ZipApplyGirdBuffs();
            FileSetStat();
        }
    }

    public virtual void AddFilePropertyWindowEvent(Action _filePropertyWindowToggleEvent)
    {
        filePropertyWindowToggled -= _filePropertyWindowToggleEvent;
        filePropertyWindowToggled += _filePropertyWindowToggleEvent;
    }

    public void NotifyPropertyWindowToggled()
    {
        filePropertyWindowToggled?.Invoke();
    }

    // 선택 상태(크기 변경/배경)
    public virtual void OnSelected(bool _isSelected)
    {
        if (VisualController == null || FileState == FileStatus.Dead)
            return;

        if (_isSelected)
        {
            VisualController?.SetSelected(true, 1.2f); // 스케일 값 전달
            isSelected = true;
            file_Skill?.ShowRange(true);
        }
        else
        {
            VisualController?.SetSelected(false, 1.0f);
            isSelected = false;
            file_Skill?.ShowRange(false);
        }
    }

    // 속성창 토글
    public void OpenPropertyWin()
    {
        if (propertyWin == null)
            return;

        propertyWin.gameObject.SetActive(!propertyWin.gameObject.activeSelf);

        filePropertyWindowToggled?.Invoke();

        TutorialEvents.FilePropertyClickInvoke(propertyWin);
    }
    
    #endregion

    #region IInteractable Implementation

    public void ApplyText(string text)
    {
        Debug.Log("ApplyText");
        if (_InputField?.text.Length > 0)
        {
            _text.text = text;
        }

        propertyWin?.ApplyText(text);
        _text?.gameObject.SetActive(true);
        _InputField?.gameObject.SetActive(false);
    }

    public void SetVisualOnOff(bool isTurnOn)   // 압축 시 폴더 자식으로 갈 경우에 사용
    {
        VisualController?.gameObject.SetActive(isTurnOn);
    }

    /// <summary>
    /// 마우스가 객체 위로 올라올 때 (ToolTip 타이밍 리셋용)
    /// </summary>
    public virtual void OnHoverEnter()
    {
        // 필요시 추가 로직 (예: 호버 효과 등)
    }

    /// <summary>
    /// 마우스가 객체에서 벗어날 때 (ToolTip 숨김)
    /// </summary>
    public virtual void OnHoverExit()
    {
        // ToolTip 숨기기
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        stageMain?.ToolTip?.ToolTipOff();
    }

    /// <summary>
    /// 마우스 버튼을 누를 때
    /// </summary>
    public virtual void OnClickEnter()
    {
    }

    /// <summary>
    /// 마우스 버튼을 뗄 때
    /// </summary>
    public virtual void OnClickExit()
    {
        // 필요시 추가 로직
    }

    public virtual void OnSelectSingle()
    {
    }

    public virtual void OnBeginDrag()
    {
        if (IsInfected) return;

        VisualController?.SetDragVisual(true);
        FileState = FileStatus.VisualFile;

        // 드래그 시작 시 원래 그리드 위치 저장
        originalGridBeforeDrag = CurrentGrid;

        // ToolTip 숨기기
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        stageMain?.ToolTip?.ToolTipOff();
    }

    public virtual void OnDrag(Vector2 mouseDelta)
    {
        // Unit_Base의 위치 이동 처리
        transform.position += (Vector3)mouseDelta;
    }

    public virtual void OnEndDrag()
    {
        // 시각 효과 복구
        VisualController?.SetDragVisual(false);

        // 그리드에서 파일 제거 (드래그 종료 시 그리드에서 먼저 제거)
        CurrentGrid?.RemoveFileUnit();

        if (FileState == FileStatus.VisualFile && inFolder != null)
        {
            originalGridBeforeDrag = null;
            return;
        }

        FileGridManager gridMgr = Managers.GridMgr;



        FileGrid targetGrid = null;
        Vector2 currentPos = transform.position;

        // 그리드 범위 내부면 해당 그리드 찾기
        if (gridMgr.IsWorldPositionInGridBounds(currentPos))
        {
            targetGrid = gridMgr.GetGrid(currentPos);
        }

        // 다른 그리드를 찾아야 하는 경우
        // 그리드가 없거나 장애물이 있음
        // 원래 그리드가 아니면서 다른 파일이 점유 중
        bool needNewGrid = targetGrid == null
            || targetGrid.obstacleObject != null
            || (targetGrid != originalGridBeforeDrag && targetGrid.GetFileUnit() != null);

        if (needNewGrid)
        {
            targetGrid = gridMgr.FindFlagGridWorld(currentPos, FileGridManager.SearchGridFlag.NotOccupied, FileGridManager.SearchGridFlag.NotObstacle);
        }

        bool success = gridMgr.TrySetUnitAtGrid(this, targetGrid);
        if (!success && originalGridBeforeDrag != null)
        {
            gridMgr.TrySetUnitAtGrid(this, originalGridBeforeDrag);
        }
        
        originalGridBeforeDrag = null;
    }

    /// <summary>
    /// 클릭 처리
    /// </summary>
    public virtual void OnClick()
    {
        // 컨텍스트 메뉴 닫기
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        stageMain?.CloseAllContextMenu();
    }

    /// <summary>
    /// 더블클릭 처리
    /// </summary>
    public virtual void OnDoubleClick()
    {
        // 더블클릭 시 파일 열기
        if (FileState != FileStatus.normal) return;

        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        if (stageMain == null) return;

        stageMain.CloseAllContextMenu();

        if (this is Unit_Folder folder)
        {
            folder.OpenWin();
        }
        else if (this is Unit_File file)
        {
            file.OpenPropertyWin();
        }
        else if (this is Unit_MyCom myCom)
        {
            myCom.OpenCreateWizard();
        }
    }

    /// <summary>
    /// 우클릭 처리 (컨텍스트 메뉴 표시)
    /// </summary>
    public virtual void OnRightClick()
    {
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        if (stageMain == null) return;

        stageMain.CloseAllContextMenu();

        // 컨텍스트 메뉴 표시
        stageMain.ShowFileContextMenu(this);
    }

    #endregion

    #region BUFF

    public abstract void FileSetStat();

    // 버프 적용
    public virtual void ApplyBuff(BuffStat _buffStat)
    {
        if (FileState != FileStatus.normal)
            return;

        buffController.ApplyBuff(_buffStat);
    }

    // 버프 제거
    public virtual void RemoveBuff(BuffStat _buffStat)
    {
        buffController.RemoveBuff(_buffStat);
    }


    #endregion

    #region Health

    // 방어 필터를 모아오는 가상 함수
    // Folder의 경우 자식 파일들을 가져오도록 재정의
    protected virtual IDamageFilter[] GetDamageFilters()
    {
        return GetComponents<IDamageFilter>();
    }

    // 데미지 처리
    public virtual bool TakeDmg(float dmg)
    {
        if (dmg < 0 || FileState == FileStatus.VisualFile || FileState == FileStatus.Dead)
            return false;

        // 최종 데미지 계산
        float finalDmg = dmg;
        
        // 내 몸에 붙어있는 모든 데미지 필터 컴포넌트를 가져오기
        IDamageFilter[] filters = GetDamageFilters();        
        foreach (var filter in filters)
        {
            // 필터를 거칠 때마다 데미지가 깎이거나 변합니다.
            finalDmg = filter.ModifyDamage(finalDmg);
        }

        if (finalDmg <= 0)
        {
            return true;
        }

        // 3. 체력 감소
        HP -= finalDmg;
        
        VisualController?.PlayHitEffect();
        UnitState.instance.FloatingText(((int)finalDmg).ToString(), this.transform.position, true);

        // 죽음 체크
        if (HP <= 0)
            StartCoroutine(UnitDie());

        // UI 업데이트: 속성창 버튼/HP 바
        propertyWin?.SetMyInfo();

        return true;
    }

    // 치유 처리
    public void Heal(float amount)
    {
        if (amount < 0 || FileState == FileStatus.Dead) return;
        HP += amount;
        // UI 업데이트: 속성창 버튼
        propertyWin?.SetMyInfo();
    }

    protected virtual IEnumerator UnitDie()
    {
        propertyWin?.gameObject.SetActive(false);
        VisualController?.PlayDieEffect();

        FileState = FileStatus.Dead;
        yield return new WaitForSeconds(0.5f);

        DeleteThisFile();
    }

    public virtual void DeleteThisFile()
    {
        // 설치 목록에서 제거
        Managers.GridMgr.ActiveFiles.Remove(this);

        // 그리드에서 제거
        FileGrid currentGrid = this.CurrentGrid;
        if (currentGrid != null)
            currentGrid.RemoveFileUnit();

        // 파일 기록 업데이트
        Managers.Data.RecordData[(int)Define.Record.DestroyFile] += 1;

        // 데이터 갱신
        Managers.nowPlayerData.capacityData.RemoveFile(useData, garbageData);
        
        Managers.Resource.Destroy(this.propertyWin?.gameObject);
        Managers.Resource.Destroy(this.gameObject);
    }
    #endregion


    #region IInfectable Implementation
    public void AddStack(int amount)
    {
        // 감염되지 않았다면 무시
        if (!IsInfected) return;

        currentCureStack += amount;

        // TODO: 스택 증가 시 시각적 효과 업데이트

        // 스택이 꽉 차면 즉시 효과 발동
        if (currentCureStack >= maxCureStack)
        {
            InvokeFullStack();
        }    
    }

    public void InvokeFullStack()
    {
        if (!IsInfected) return;

        // 1. 내 몸에 붙어있는 모든 감염 컴포넌트를 찾습니다. (중복 감염 대응)
        VirusInfection[] infections = GetComponents<VirusInfection>();

        // 2. 찾아낸 모든 바이러스 컴포넌트에게 강제 해제(ForceClear)를 명령합니다.
        foreach (var infection in infections)
        {
            infection.ForceClear(); 
        }

        // 3. 스택 초기화
        currentCureStack = 0;
        
        Debug.Log($"{gameObject.name}의 모든 감염이 치료되었습니다!");
        
        // TODO : 치료 완료 시 시각적 효과 업데이트
    }
    #endregion
}
