using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WFK_Challenge.WFK_Map.WFK_Region.WFK_Field;
using WFK_Challenge.WFK_Quest;
using static Define;

namespace WFK_Challenge.WFK_Map.WFK_Region
{
    public enum RegionType
    {
        First,
        City,
        Village,
        End,
    }

    /// <summary>
    /// 게임 지역을 나타내는 클래스입니다.
    /// 퀘스트 생성과 필드 관리를 담당하며, QuestManager와 협력하여 퀘스트를 관리합니다.
    /// </summary>
    public class Region : MonoBehaviour
    {
        public int regionLevel { get; private set; }
        public MonsterSet monsterSet { get; private set; }  // 몬스터 세트

        protected List<int> m_fieldIndexList;
        public IReadOnlyList<int> GetFieldIndexList => m_fieldIndexList;

        protected List<int> m_questIndexList;
        public IReadOnlyList<int> GetQuestIndexList => m_questIndexList;

        public bool IsOpen {  get; private set; }

        public virtual void Init(int _level)
        {
            regionLevel = _level;
            IsOpen = false;
            // 리스트 초기화
            m_fieldIndexList = new List<int>();
            m_questIndexList = new List<int>();
        }


        public async Task GenerateRegion(Func<float, string, Task> onProgress)
        {
            // 기존 데이터 초기화
            m_fieldIndexList.Clear();

            // 4~6개의 메인 퀘스트/필드 생성
            int questCount = GameSeed.NextInt(GameSeed.Domain.Region, 4, 7);
            int currentQuestLevel = regionLevel;

            // 몬스터 타입 랜덤 선택 (Seed 기반)
            MonsterType monsterType = GameSeed.RandomEnum<MonsterType>(GameSeed.Domain.Region);

            // 몬스터 세트 생성 
            monsterSet = new MonsterSet(monsterType);
       
            // 퀘스트 데이터 준비
            List<Data_QuestType> questDataList = Managers.Data.QuestTypeData.Values
                .OrderBy(q => q.m_id)
                .ToList();

            List<Data_QuestDialog> questDialogList = Managers.Data.QuestDialogData.Values
                .OrderBy(d => d.m_dialogIndex)
                .ToList();

            if (questDataList == null)
            {
                Debug.LogError("퀘스트 데이터가 없습니다!");
                return;
            }
          
            // 메인 퀘스트 생성          
            for (int i = 0; i < questCount; i++)
            {
                Quest mainQuest = Managers.Quest.GenerateQuest(monsterSet, ref currentQuestLevel, questDataList, questDialogList);

                int questIndex = Managers.Quest.MainQuestList.IndexOf(mainQuest);

                AddQuestIndex(questIndex);

                // 필드 생성 및 메인퀘스트와 연결
                Field field = DataFactory.MakeField(this, questIndex, transform);
                
                // Quest에 소유 Region과 Field 정보 설정 (인덱스 기반)
                List<Region> regionList = Managers.Map.WorldRegionList as List<Region>;
                List<Field> fieldList = Managers.Map.WorldFieldList as List<Field>;
                
                int regionIndex = regionList.IndexOf(this);
                int fieldIndex = fieldList.IndexOf(field);
                
                mainQuest.SetOwnerRegionAndFieldIndex(regionIndex, fieldIndex);

                // 진행률 업데이트
                float progress = (float)(i + 1) / questCount;
                if (onProgress != null)
                    await onProgress(progress, $"지역 필드 {i + 1}/{questCount} 생성 중...");

            }
        }
        
        public void EnterRegion()
        {
            IsOpen = true;
        }

        public void AddFieldIndex(int index)
        {
            m_fieldIndexList.Add(index);
        }

        public void AddQuestIndex(int index)
        {
            m_questIndexList.Add(index);
        }

    }
}