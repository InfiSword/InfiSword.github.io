using System.Collections.Generic;
using UnityEngine;
using WFK_Challenge.WFK_Quest;
using static Define;

namespace WFK_Challenge.WFK_Map.WFK_Region.WFK_Field
{
    /// <summary>
    /// 게임 필드를 나타내는 클래스
    /// 퀘스트와 몬스터를 위한 전장 환경으로 사용되며,
    /// 퀘스트 정보는 QuestManager가 관리 담당
    /// </summary>
    public class Field : MonoBehaviour
    {
        public int fieldLevel { get; protected set; }           // 필드 레벨 (퀘스트 레벨과 동일)
        public FieldEventType eventType { get; protected set; } // 필드 이벤트 타입
        public EventGrade eventGrade { get; protected set; }    // 이벤트 등급
        public bool isUnlocked { get; protected set; }          // 필드 잠금 상태

        protected bool isEnter;
        public int backGroundIndex;

        public int m_questIndex { get; protected set; }

        public virtual void Init(int questIndex)
        {
            m_questIndex = questIndex;

            Quest quest = Managers.Quest.MainQuestList[m_questIndex];

            fieldLevel = quest.m_questLevel;

            backGroundIndex = GameSeed.NextInt(GameSeed.Domain.Field, 0, 9);
            isUnlocked = false; // 초기에는 잠겨있음

            // 이벤트 타입과 등급 랜덤 설정
            SetRandomEventType();
        }

        /// <summary>
        /// 필드의 이벤트 타입과 등급을 설정합니다.
        /// </summary>
        private void SetRandomEventType()
        {
            // 이벤트 타입 랜덤 선택 (None 제외)
            FieldEventType[] eventTypes = { FieldEventType.Battle, FieldEventType.Treasure,
                                          FieldEventType.Trap, FieldEventType.Shop,
                                          FieldEventType.Rest, FieldEventType.Boss };
            eventType = eventTypes[UnityEngine.Random.Range(0, eventTypes.Length)];

            // 등급 랜덤 설정 (Common이 가장 높은 확률로 나타나도록 가중치 설정)
            float random = GameSeed.NextFloat01(GameSeed.Domain.Field);
            if (random < 0.5f)
                eventGrade = EventGrade.Common;
            else if (random < 0.75f)
                eventGrade = EventGrade.Uncommon;
            else if (random < 0.9f)
                eventGrade = EventGrade.Rare;
            else if (random < 0.98f)
                eventGrade = EventGrade.Epic;
            else
                eventGrade = EventGrade.Legendary;
        }

        /// <summary>
        /// 필드를 해금합니다.
        /// </summary>
        public virtual void UnlockField()
        {
            isUnlocked = true;
            Debug.Log($"Field {this.name} unlocked!");
        }

        // 필드에 입장
        public virtual void Enter()
        {
            SpawnMonster(Managers.Quest.MainQuestList[m_questIndex]);
        }

        // 필드에서 몬스터를 생성
        public virtual void SpawnMonster(Quest baseQuest)
        {
            Managers.Spawn.RandomGenMonster();

            // 퀘스트에서 타겟 정보 가져와서 생성
            if (baseQuest != null)
            {
                (int minGrade, int maxGrade) = Managers.Quest.GetMonsterRatingRange(baseQuest.dangerLevel);
                MonsterSet monsterSet = Managers.Map.WorldRegionList[baseQuest.OwnerRegionIndex].monsterSet;

                List<int> monsterIDs = monsterSet.GetMonstersID(minGrade, maxGrade);

                foreach (int monsterID in monsterIDs)
                {
                    Managers.Spawn.AddMob(monsterID);
                }

                Managers.Spawn.DoStart(baseQuest.questType, this);
            }
        }
    }
}
