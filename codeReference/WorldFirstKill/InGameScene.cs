using UnityEngine;
using System.Threading.Tasks;
using WFK_Challenge.WFK_Map.WFK_Region;
using System.Collections.Generic;

public class InGameScene : BaseScene
{
    protected override async void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;

        // 1. 공통 데이터 로딩 (CSV 다운로드 및 파싱)
        bool dataSuccess = await Managers.Loading.StartLoadingDataAsync();
        if (!dataSuccess)
        {
            Debug.LogError("데이터 로딩 실패. 로비로 이동합니다.");
            Managers.Scene.LoadScene(Define.Scene.Lobby);
            return;
        }

        Managers.InitializeGameSystems();
        
        long baseSeed =0L;

        // 게임 시작 시 Seed 설정: 새 게임이면 이미 UI_Seed에서 Init 완료됨, 로드 게임이면 저장된 토큰으로 복원
        if (Managers.IsNewGame)
        {
            Debug.Log($"새 게임 시작: 입력된 시드 {GameSeed.BaseSeed} 기반으로 맵 생성 시작");
            await InitNewGame();
        }
        else
        {
            // 저장된 토큰 로드 및 RNG 복원
            if (!SaveLoadManager.TryLoadSeedToken(out string seedToken, out baseSeed))
            {
                Debug.LogError("로드 실패: 시드 토큰이 없습니다. 로비로 이동합니다.");
                Managers.Scene.LoadScene(Define.Scene.Lobby);
                return;
            }

            // 맵 재생성을 위해 우선 BaseSeed로 초기화 (사용량 0)
            GameSeed.Init(baseSeed);

            Debug.Log($"로드 게임 시작: 저장된 시드 {baseSeed} 기반으로 맵 복원 시작");
            await InitLoadGame();

            // 맵 복원이 완료 되었으니 RNG 상태를 세이브 시점으로 최종 복원 (연속성 보장)
            if (!string.IsNullOrEmpty(seedToken))
            {
                bool hasToken = SaveLoadManager.RestoreUsageCount(seedToken);
                if (!hasToken)
                {
                    Debug.LogWarning("Seed 토큰 복원 실패. BaseSeed로 시작한 상태로 진행합니다.");
                }
            }
        }

        SpawnPlayers();
        Managers.Game.SetDate();

        Managers.UI.ShowSceneUI<UI_InGame>();        
    }

    private async Task InitNewGame()
    {
        bool success = await Managers.Loading.StartMapInitAsync();
        if (success)
        {
            Managers.Game.RandomClassData();
            Debug.Log("맵 초기화 완료. 게임 시작 준비.");
            Managers.IsNewGame = false;
            SaveLoadManager.SaveSeedTokenToJson();
        }
        else
        {
            Debug.LogError("맵 초기화 실패 또는 취소. 로비 화면으로 이동.");
            Managers.Scene.LoadScene(Define.Scene.Lobby);
            return;
        }
    }

    private async Task InitLoadGame()
    {
        // Seed 또는 Seed 토큰으로 RNG가 설정된 직후이므로, 맵(방/지형/필드/퀘스트)은 반드시 재생성해야 함
        bool success = await Managers.Loading.StartMapInitAsync();
        if (!success)
        {
            Debug.LogError("로드 실패: 맵 초기화 실패. 로비로 이동합니다.");
            Managers.Scene.LoadScene(Define.Scene.Lobby);
            return;
        }

        Managers.Game.RandomClassData();
        Debug.Log("로드 성공: 맵 복원이 완료.");
    }

    private void SpawnPlayers()
    {
        Transform unitPos = GameObject.Find("Units").transform;
        Managers.Resource.Instantiate($"Unit/Player/Unit_Player{0}", unitPos).GetComponent<Unit_Player>().SetPlayer(0, Define.UIColor.Red);
        Managers.Resource.Instantiate($"Unit/Player/Unit_Player{1}", unitPos).GetComponent<Unit_Player>().SetPlayer(1, Define.UIColor.Green);
        Managers.Resource.Instantiate($"Unit/Player/Unit_Player{2}", unitPos).GetComponent<Unit_Player>().SetPlayer(2, Define.UIColor.Blue);
        Managers.Resource.Instantiate($"Unit/Player/Unit_Player{3}", unitPos).GetComponent<Unit_Player>().SetPlayer(3, Define.UIColor.Yellow);
    }

    public override void Clear()
    {
        // 씬 클리어시 호출
    }
}