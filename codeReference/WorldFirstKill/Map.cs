using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WFK_Challenge.WFK_Map.WFK_Region;
using WFK_Challenge.WFK_Map.WFK_Region.WFK_Field;

namespace WFK_Challenge.WFK_Map
{
    public class Map : MonoBehaviour
    {
        private float m_randomRegionProbability;
        private int m_areaSetupCount;
        public float GetRandomRegionProbability => m_randomRegionProbability;
        public int GetAreaSetupCount => m_areaSetupCount;

        public int mapRanking { get; private set; }
        public int mapIndex { get; private set; }

        private List<int> m_regionIndexList;
        public IReadOnlyList<int> GetRegionIndexList => m_regionIndexList;

        public void Init(int _mapRanking, int _areaCount)
        {
            mapRanking = _mapRanking;
            m_areaSetupCount = _areaCount;
            m_randomRegionProbability = 0.7f;
            m_regionIndexList = new List<int>();

            Dictionary<RegionType, int> regionCountDict = new Dictionary<RegionType, int>();

            for (int i = (int)RegionType.First + 1; i < (int)RegionType.End; i++)
                regionCountDict.Add((RegionType)i, 0);

            for (int i = 0; i < m_areaSetupCount; i++)
            {
                int regionType = GameSeed.NextInt(GameSeed.Domain.Map, (int)RegionType.First + 1, (int)RegionType.End);
                regionCountDict[(RegionType)regionType]++;
            }

            List<int> regionTypeList = RandomRegion(regionCountDict);

            int calcRegionLevel = mapRanking;

            for (int i = 0; i < regionTypeList.Count; i++)
            {
                RegionType regionType = (RegionType)regionTypeList[i];
                string typeNamespace = $"WFK_Challenge.WFK_Map.WFK_{regionType}.{regionType}";
                string assemblyName = "Assembly-CSharp";

                Type type = Type.GetType($"{typeNamespace}, {assemblyName}");
                if (type == null)
                {
                    Debug.LogError($"클래스를 찾을 수 없습니다: {regionType}");
                    continue;
                }

                DataFactory.MakeRegion(this, type, calcRegionLevel, this.transform);                

                calcRegionLevel += GameSeed.NextInt(GameSeed.Domain.Map, 1, 3);

            }
        }

        public void AddRegionIndex(int regionIndex)
        {
            m_regionIndexList.Add(regionIndex);
        }

        /// <summary>
        /// 랜덤하게 지역을 선정하여 중복을 피하는 로직입니다.
        /// </summary>
        /// <param name="originalDict">원본 지역 타입 딕셔너리</param>
        /// <returns>랜덤한 지역 타입 리스트</returns>
        private List<int> RandomRegion(Dictionary<RegionType, int> originalDict)
        {
            List<RegionType> regionTypeKeyList = originalDict.Keys.ToList();
            List<int> result = new List<int>();

            // 가장 많이 나온 지역과, 그 지역의 카운트
            RegionType manyRegionType = RegionType.First + 1;
            int manyRegionCount = originalDict[manyRegionType];

            // 최소 1개는 있도록 보장 AND 가장 많은 지역 찾기 로직 탐색
            foreach (var type in regionTypeKeyList)
            {
                if (originalDict[type] == 0)
                    result.Add((int)type);

                if (originalDict[manyRegionType] < originalDict[type])
                {
                    manyRegionType = type;
                    manyRegionCount = originalDict[type];
                }
            }

            int surplus = manyRegionCount / 2;

            for (int i = 0; i < surplus; i++)
            {
                result.Add((int)manyRegionType);
            }

            while (m_areaSetupCount - result.Count != 0)
            {
                RegionType typeToAdd;

                // 확률에 따라 가장 많이 나온 지역(manyRegionType)을 추가할지, 다른 지역을 추가할지 결정합니다.
                // surplus 값이 클수록, m_randomRegionProbability 값이 클수록 다른 지역이 선택될 확률이 높아집니다.
                float rand01 = GameSeed.NextFloat01(GameSeed.Domain.Map);
                if (surplus > 0 && rand01 <= Mathf.Pow((0.7f / surplus), m_randomRegionProbability))
                {
                    // manyRegionCount 추가
                    typeToAdd = manyRegionType;
                }
                else
                {
                    // 다른 지역 중 하나 추가
                    // regionTypeKeyList에서 manyRegionType과 일치하지 않는 RegionType 리스트를 만들고
                    List<RegionType> otherTypes = regionTypeKeyList.Where(t => t != manyRegionType).ToList();
                    // manyRegionType을 제외한 RegionType 중 하나를 랜덤하게 선택하여 typeToAdd에 할당
                    typeToAdd = otherTypes[GameSeed.NextInt(GameSeed.Domain.Map, 0, otherTypes.Count)];
                }
                result.Add((int)typeToAdd);
                surplus--;
            }

            // 리스트를 랜덤화하여 반환
            return result;
        }
    }
}