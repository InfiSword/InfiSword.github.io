using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 게임 데이터 및 맵 초기화 등 로딩 과정을 관리하는 매니저 클래스입니다.
/// </summary>
public class LoadingManager
{
    private UI_Loading loadingUI;

    private bool isInitialLoading = false;
    private bool isMapLoading = false;

    // 초기 로딩 가중치 설정 (데이터 로드: 80%, 서버 데이터 적용: 20%)
    private const float INITIAL_TOTAL_WEIGHT = 1.0f;
    private const float DATA_LOAD_WEIGHT = 0.8f;
    private const float SERVER_DATA_WEIGHT = 0.2f;

    // 맵 초기화 가중치 설정
    private const float MAP_INIT_WEIGHT = 1.0f;

    /// <summary>
    /// 로딩 매니저를 초기화합니다.
    /// </summary>
    public void Init()
    {
        if (loadingUI == null)
        {
            loadingUI = Managers.UI.ShowPopupUI<UI_Loading>();
            loadingUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 로딩 상태를 초기화하고 UI를 숨깁니다.
    /// </summary>
    public void Clear()
    {
        if (loadingUI != null)
        {
            loadingUI.gameObject.SetActive(false);
        }
        isInitialLoading = false;
        isMapLoading = false;
    }

    /// <summary>
    /// 로딩 UI에 진행 상황과 메시지를 업데이트합니다.
    /// </summary>
    private void UpdateProgress(float progress, string message)
    {
        if (loadingUI != null)
        {
            loadingUI.SetProgress(progress, message);
        }
    }

    /// <summary>
    /// 데이터 로드 완료 여부를 반환합니다.
    /// </summary>
    public bool IsDataLoaded { get; private set; } = false;

    /// <summary>
    /// 초기 데이터 로딩 프로세스를 시작합니다 (메타 시트, CSV 다운로드, 데이터 로드).
    /// </summary>
    public async Task<bool> StartLoadingDataAsync(CancellationToken cancellationToken = default)
    {
        if (IsDataLoaded)
            return true;

        if (isInitialLoading)
            return false;

        isInitialLoading = true;
        float currentProgress = 0f;

        if (loadingUI == null)
            loadingUI = Managers.UI.ShowPopupUI<UI_Loading>();

        // 켜질 때 최상단으로 오도록 순서 갱신 (레이어는 유지)
        Managers.UI.SetCanvas(loadingUI.gameObject, Define.SceneLayer.PopupUI);
        loadingUI.gameObject.SetActive(true);

        // 0% -> 5% 구간: 메타 데이터 확인 (0% 멈춤 현상 방지)
        UpdateProgress(0.01f, "서버 목록 확인 중...");

        // 1. 메타 시트 다운로드
        bool metaSuccess = await ServerCSV_ConvertData.DownloadMetaSheetAsync();
        if (!metaSuccess)
        {
            Debug.LogError("메타 시트 다운로드 실패");
            isInitialLoading = false;
            loadingUI.gameObject.SetActive(false);
            return false;
        }

        currentProgress = 0.05f;
        UpdateProgress(currentProgress, "데이터 목록 수신 완료");

        // 2. 모든 CSV 파일 다운로드 및 로컬 저장 (5% -> 80% 구간)
        bool csvSuccess = await ServerCSV_ConvertData.DownloadAllCSVAsync((progress, fileName) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            // progress(0~1)를 5%~80% 구간으로 변환
            float startRange = 0.05f;
            float endRange = DATA_LOAD_WEIGHT;
            float weightedProgress = startRange + (progress * (endRange - startRange));

            UpdateProgress(weightedProgress, $"데이터 수신 중: {fileName}");
        });

        if (!csvSuccess)
        {
            Debug.LogError("CSV 데이터 다운로드 실패");
            isInitialLoading = false;
            loadingUI.gameObject.SetActive(false);
            return false;
        }

        // 3. 서버 데이터 로드 (80% -> 100% 구간)
        cancellationToken.ThrowIfCancellationRequested();

        UpdateProgress(DATA_LOAD_WEIGHT, "데이터 로드 중...");

        await Managers.Data.ServerDataLoadAsync();

        currentProgress = INITIAL_TOTAL_WEIGHT;
        UpdateProgress(currentProgress, "데이터 로딩 완료!");

        loadingUI.gameObject.SetActive(false);

        IsDataLoaded = true;
        isInitialLoading = false;
        return true;
    }

    /// <summary>
    /// 맵 데이터 및 시스템 초기화 로딩을 시작합니다.
    /// </summary>
    public async Task<bool> StartMapInitAsync(CancellationToken cancellationToken = default)
    {
        if (isMapLoading)
            return false;

        isMapLoading = true;
        float currentProgress = 0f;

        if (loadingUI == null)
            loadingUI = Managers.UI.ShowPopupUI<UI_Loading>();

        loadingUI.gameObject.SetActive(true);
        UpdateProgress(0f, "맵 초기화 중...");

        // 맵 초기화 진행 (MapManager 호출)
        bool mapInitSuccess = await Managers.Map.NewGame_InitMap(async (progress, message) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            float totalProgress = progress * MAP_INIT_WEIGHT;
            UpdateProgress(totalProgress, message);
            await Task.Yield();
        });

        if (!mapInitSuccess)
        {
            Debug.LogError("맵 초기화 실패");
            isMapLoading = false;
            loadingUI.gameObject.SetActive(false);
            return false;
        }

        currentProgress = INITIAL_TOTAL_WEIGHT;
        UpdateProgress(currentProgress, "맵 초기화 완료!");

        isMapLoading = false;
        loadingUI.gameObject.SetActive(false);

        return true;
    }
}
