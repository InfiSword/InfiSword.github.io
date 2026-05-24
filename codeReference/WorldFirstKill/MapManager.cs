using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WFK_Challenge.WFK_Map;
using WFK_Challenge.WFK_Map.WFK_Region;
using WFK_Challenge.WFK_Map.WFK_Region.WFK_Field;

public class MapManager : MonoBehaviour
{
    public int NowLocationMapRanking { get; private set; }

    private BGRender m_render;
    private GameObject m_mapParentObj;

    private List<Map> m_worldMapList;
    private List<Region> m_worldRegionList;
    private List<Field> m_worldFieldList;

    public IReadOnlyList<Map> WorldMapList => m_worldMapList;
    public IReadOnlyList<Region> WorldRegionList => m_worldRegionList;
    public IReadOnlyList<Field> WorldFieldList => m_worldFieldList;

    public Dictionary<int, bool> MapRankingUnlockDict { get; private set; }

    // 이어하기 할 때, 로딩을 나타내는 bool값
    public bool IsLoadedData { get; private set; }

    // 이벤트 발행 전 임시로 저장할 데이터
    public Region PendingRegion { get; private set; }
    public Field PendingField { get; private set; }

    public void Init()
    {
        m_worldMapList = new List<Map>();
        m_worldRegionList = new List<Region>();
        m_worldFieldList = new List<Field>();
        MapRankingUnlockDict = new Dictionary<int, bool>();

        IsLoadedData = false;

        if (m_mapParentObj == null)
            m_mapParentObj = new GameObject { name = "@Total_Map" };

        m_render = Managers.Resource.Instantiate("Object/BackGround", m_mapParentObj.transform).GetComponent<BGRender>();
        DontDestroyOnLoad(m_mapParentObj);

        Managers.Event.OnEnterMapEvent -= ChangeMap;
        Managers.Event.OnEnterRegionEvent -= ChangeRegion;
        Managers.Event.OnEnterFieldEvent -= ChangeField;

        Managers.Event.OnEnterMapEvent += ChangeMap;
        Managers.Event.OnEnterRegionEvent += ChangeRegion;
        Managers.Event.OnEnterFieldEvent += ChangeField;
    }

    public async Task<bool> NewGame_InitMap(Func<float, string, Task> onProgress)
    {
        if (m_mapParentObj == null)
        {
            return false;
        }

        Map map1 = DataFactory.MakeMap(1, 3, m_mapParentObj.transform);

        for (int i = 0; i < map1.GetAreaSetupCount; i++)
        {
            float progress = Mathf.Lerp(0.8f, 1.0f, (float)(i + 1) / map1.GetAreaSetupCount);

            if (onProgress != null)
                await onProgress(progress, $"지역 {i + 1}/{map1.GetAreaSetupCount} 생성 완료...");
        }
        MapRankingUnlockDict[1] = true;
        NowLocationMapRanking = 1;


        Map map10 = DataFactory.MakeMap(10, 3, m_mapParentObj.transform);

        for (int i = 0; i < map10.GetAreaSetupCount; i++)
        {
            float progress = Mathf.Lerp(0.8f, 1.0f, (float)(i + 1) / map10.GetAreaSetupCount);

            if (onProgress != null)
                await onProgress(progress, $"지역 {i + 1}/{map10.GetAreaSetupCount} 생성 완료...");
        }
        MapRankingUnlockDict[10] = false;

        Map map20 = DataFactory.MakeMap(20, 3, m_mapParentObj.transform);

        for (int i = 0; i < map20.GetAreaSetupCount; i++)
        {
            float progress = Mathf.Lerp(0.8f, 1.0f, (float)(i + 1) / map20.GetAreaSetupCount);

            if (onProgress != null)
                await onProgress(progress, $"지역 {i + 1}/{map20.GetAreaSetupCount} 생성 완료...");
        }
        MapRankingUnlockDict[20] = false;

        if (WorldRegionList != null && WorldRegionList.Count != 0)
        {
            foreach (Region region in WorldRegionList)
            {
                await region.GenerateRegion(onProgress);
            }
        }

        IsLoadedData = true;
        return true;
    }

    // 해당 업데이트를 실행할 때, 호출해 줄 함수
    public void UpdatingMap(Map map)
    {
        // 해금
        MapRankingUnlockDict[map.mapRanking] = true;
    }

    public List<Region> GetRegionListForMap(int mapRanking)
    {
        Map searchMap = m_worldMapList.FirstOrDefault(map => map.mapRanking == mapRanking);

        if (searchMap == null) return null;

        List<Region> regions = new List<Region>();
        foreach (int idx in searchMap.GetRegionIndexList)
        {
            if (idx >= 0 && idx < m_worldRegionList.Count && m_worldRegionList[idx] != null)
                regions.Add(m_worldRegionList[idx]);
        }
        return regions;
    }

    public void AddMap(Map map)
    {
        if (map != null && !m_worldMapList.Contains(map))
        {
            m_worldMapList.Add(map);
        }
    }

    public void AddRegion(Map map, Region region)
    {
        if (map != null && region != null && !m_worldRegionList.Contains(region))
        {
            m_worldRegionList.Add(region);
            map.AddRegionIndex(m_worldRegionList.IndexOf(region));
        }
    }
    public void AddField(Region region, Field field)
    {
        if (region != null && !m_worldFieldList.Contains(field))
        {
            m_worldFieldList.Add(field);
            region.AddFieldIndex(m_worldFieldList.IndexOf(field));
        }
    }

    private void ChangeMap(Map targetMap)
    {
        if (targetMap == null) return;

        foreach (Map map in m_worldMapList)
        {
            bool active = map.mapRanking == targetMap.mapRanking;
            map.gameObject.SetActive(active);
            if (active)
                NowLocationMapRanking = targetMap.mapRanking;
        }
    }

    private void ChangeRegion(Region region)
    {
        if (region == null) return;

        Managers.Spawn.Clear();
        Managers.Battle.isBattle = false;

        // UI 전환 및 Region UI 활성화
        UI_InGame uiInGame = Managers.UI.GetSceneUI<UI_InGame>();
        uiInGame.SwitchToRegionUI();
        uiInGame.MainTab.RegionTab.ActiveRegionUI(region);
    }

    private void ChangeField(Field field)
    {
        Managers.Spawn.Clear();
        Managers.Battle.isBattle = true;

        m_render.gameObject.SetActive(true);
        m_render.INDEX = (BGRender.BGList)field.backGroundIndex;
        Managers.UI.GetSceneUI<UI_InGame>().DoFade(Define.FadeMove.LEFT, Define.FadeMove.LEFT,
            () =>
            {
                m_render.SetBG(m_render.INDEX, true);
                Managers.Battle.DeleteEnemy();
            });

        field.Enter();
    }

    public int GetMapIndex(Map map)
    {
        return m_worldMapList.IndexOf(map);
    }
}
