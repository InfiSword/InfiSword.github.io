using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 바이러스의 특수 기술(방해 공작)을 실행하고 관리하는 컴포넌트.
/// </summary>
public class Skill_VirusBase : MonoBehaviour
{
    private static readonly System.Type[] InfectionTypes = new System.Type[]
    {
        typeof(WormInfection),
        typeof(RansomwareInfection),
        // 새로운 감염 타입은 여기에 추가
    };

    private Vector2 offset = new Vector2(50f, -50f);
    private WaitForSeconds waitTime = new WaitForSeconds(0.05f);

    /// <summary>
    /// 랜덤한 바이러스 기술을 실행합니다.
    /// </summary>
    public void ExecuteRandomSkill()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: LoadPopUP(); break;
            case 1: StartCoroutine(CoruManyPopUP()); break;
            case 2: InfectionFile(); break;
        }
    }

    /// <summary>
    /// 튜토리얼용 바이러스 팝업을 생성합니다.
    /// </summary>
    public void TutorialVirusPopup()
    {
        ManyPopUP(true);
        LoadPopUP(true);
    }

    /// <summary>
    /// 다운로드 팝업을 생성합니다.
    /// </summary>
    public void LoadPopUP(bool isTutorial = false)
    {
        UI_LoadPopup popup = Managers.Pool.GetWin<UI_LoadPopup>(Define.PoolingWinType.UI_LoadPopup);
        if (popup == null) return;

        popup.GetComponent<RectTransform>().anchoredPosition = GetRandomScreenPos();
        popup.gameObject.SetActive(true);
        if (isTutorial)
        {
            popup.TutorialMarkUI(true);
        }
    }

    /// <summary>
    /// 많은 팝업을 생성합니다.
    /// </summary>
    public void ManyPopUP(bool isTutorial = false)
    {
        StartCoroutine(CoruManyPopUP(isTutorial));
    }

    public IEnumerator CoruManyPopUP(bool isTutorial = false)
    {
        Vector2 createPos = GetRandomScreenPos();
        int _rand = Random.Range(0, 2);
        StageMain stageMain = Managers.UI.GetSceneUI<StageMain>();
        Canvas canvas = stageMain.GetComponentInParent<Canvas>();

        for (int i = 0; i < 1; i++)
        {
            UI_ManyPopup _popup = Managers.Pool.GetWin<UI_ManyPopup>(Define.PoolingWinType.UI_ManyPopup);
            if (_popup == null) yield break;

            RectTransform _poupRect = _popup.GetComponent<RectTransform>();
            _poupRect.anchoredPosition = createPos;

            if (_rand == 0)
                createPos += offset;
            else
                createPos = GetRandomScreenPos();

            if (Util.IsOverScreen(canvas, createPos, _poupRect, stageMain.UICamera))
            {
                createPos = GetRandomScreenPos();
            }

            _popup.gameObject.SetActive(true);
            _popup.RandomAnimation();
            if (isTutorial)
            {
                _popup.TutorialMarkUI(true);
            }

            yield return waitTime;
        }
    }

    /// <summary>
    /// 무작위 파일을 감염시킵니다.
    /// </summary>
    public void InfectionFile()
    {
        if (InfectionTypes.Length == 0)
        {
            Debug.LogWarning("[Skill_VirusBase] 등록된 감염 타입이 없습니다!");
            return;
        }

        if (Managers.GridMgr.ActiveFiles.Count <= 0)
            return;

        // 감염 가능한 대상 필터링 (이미 감염되었거나 죽은 파일 제외)
        List<File_Base> targetCandidates = Managers.GridMgr.ActiveFiles
            .Where(file => file != null && 
                           file.FileState == FileStatus.normal && 
                           !file.IsInfected)
            .ToList();

        if (targetCandidates.Count == 0)
            return;

        // 랜덤 대상 및 랜덤 감염 타입 선택
        File_Base target = targetCandidates[Random.Range(0, targetCandidates.Count)];
        System.Type selectedInfectionType = InfectionTypes[Random.Range(0, InfectionTypes.Length)];

        // 감염 컴포넌트 추가
        target.gameObject.AddComponent(selectedInfectionType);
        Debug.Log($"[Skill_VirusBase] {target.name}에 {selectedInfectionType.Name} 감염 적용!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit_MyCom myCom = collision.GetComponent<Unit_MyCom>();
        if (myCom == null) return;

        // 내 컴퓨터에 도달했을 때 감염 시도
        InfectionFile();
    }

    private Vector2 GetRandomScreenPos()
    {
        float x = Random.Range(0, 1420);
        float y = Random.Range(0, -730);
        return new Vector2(x, y);
    }

    // 전역 호출(치트/튜토리얼)을 위한 정적 헬퍼
    public static void GlobalExecuteRandomSkill() => Managers.Pool.Root.GetOrAddComponent<Skill_VirusBase>().ExecuteRandomSkill();
    public static void GlobalTutorialVirusPopup() => Managers.Pool.Root.GetOrAddComponent<Skill_VirusBase>().TutorialVirusPopup();
    public static void GlobalLoadPopup() => Managers.Pool.Root.GetOrAddComponent<Skill_VirusBase>().LoadPopUP();
    public static void GlobalManyPopUP() => Managers.Pool.Root.GetOrAddComponent<Skill_VirusBase>().ManyPopUP();
    public static void GlobalInfectionFile() => Managers.Pool.Root.GetOrAddComponent<Skill_VirusBase>().InfectionFile();
}
