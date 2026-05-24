using System;
using UnityEngine;
using static Define;
using WFK_Challenge.WFK_Map.WFK_Region;
using WFK_Challenge.WFK_Map.WFK_Region.WFK_Field;

namespace WFK_Challenge.WFK_Quest
{
    public class Quest
    {    
        public int m_id { get; protected set; }
        public string m_questName { get; protected set; }
        public string m_questDescription { get; protected set; }
        public int m_requiredCount { get; protected set; }
        public float m_value { get; protected set; }

        public int m_dialogIndex { get; protected set; }
        public string m_dialogQuestType { get; protected set; }
        public string m_dialogName { get; protected set; }
        public string m_dialogStartContext { get; protected set; }
        public string m_dialogRewardContext { get; protected set; }

        public int m_questLevel { get; protected set; }
        public int dangerLevel { get; protected set; }
        public int m_rewardGold { get; protected set; }
        public int m_rewardExp { get; protected set; }

        public Define.Quest_Difficulty difficulty { get; protected set; }
        public Define.Quest_Type questType { get; protected set; }

        protected int m_targetID;    // 목표 ID
        protected Type m_targetType; // 목표 타입
        protected int m_currentCount; // 수락 후 달성한 카운트

        // 진행도/완료 상태는 QuestManager에서 관리합니다.

        // Quest가 속한 Region과 Field의 인덱스를 설정합니다.
        public int OwnerRegionIndex { get; private set; }
        public int OwnerFieldIndex { get; private set; }

        public Quest(QuestContext ctx)
        {
            difficulty = ctx.difficultyTag;
            m_id = ctx.questData.m_id;

            if (Enum.TryParse(ctx.questData.m_questType, out Quest_Type type))
                questType = type;
            else
                questType = Quest_Type.None;

            m_questName = $"{ctx.questData.m_questType}_퀘스트";
            m_questDescription = ctx.description;
            m_requiredCount = ctx.count;
            m_value = ctx.value;

            if (ctx.dialogData != null)
            {
                m_dialogIndex = ctx.dialogData.m_dialogIndex;
                m_dialogQuestType = ctx.dialogData.m_dialogQuestType;
                m_dialogName = ctx.dialogData.m_dialogName;
                m_dialogStartContext = ctx.dialogData.m_dialogStartContext;
                m_dialogRewardContext = ctx.dialogData.m_dialogRewardContext;
            }

            float RewardRatio = (float)GameSeed.NextInt(GameSeed.Domain.Reward, 0, 11) / 10f;
            m_questLevel = ctx.questLevel;
            dangerLevel = ctx.danger;
  
            m_rewardGold = Mathf.RoundToInt(Managers.Data.ResourceData[m_questLevel].questGold * ((float)ctx.difficultyTag * RewardRatio));
            m_rewardExp = Mathf.RoundToInt(Managers.Data.ResourceData[m_questLevel].questExp * ((float)ctx.difficultyTag * (1f - RewardRatio)));

            m_targetID = ctx.targetID;
            m_targetType = typeof(Unit_Monster);
            m_currentCount = 0;
        }

        public void RewardAdjustment(int _setValue)
        {
            m_rewardGold /= _setValue;
            m_rewardExp /= _setValue;
        }        

        public virtual int GetCurrentProgress()
        {
            return m_currentCount;
        }

        public void IncrementProgress(int amount)
        {
            if (amount <= 0) return;
            m_currentCount += amount;
            if (m_currentCount < 0) m_currentCount = 0;
        }

        public bool IsQuestCompleted()
        {
            return m_currentCount >= m_requiredCount;
        }

        public int GetTargetID()
        {
            return m_targetID;
        }

        public Type GetTargetType()
        {
            return m_targetType;
        }

        /// <summary>
        /// Quest가 속한 Region과 Field의 인덱스를 설정합니다.
        /// </summary>
        public void SetOwnerRegionAndFieldIndex(int regionIndex, int fieldIndex)
        {
            OwnerRegionIndex = regionIndex;
            OwnerFieldIndex = fieldIndex;
        }

        /// <summary>
        /// Quest가 속한 Region을 인덱스로부터 가져옵니다.
        /// </summary>
        public Region GetOwnerRegion()
        {
            if (OwnerRegionIndex >= 0 && OwnerRegionIndex < Managers.Map.WorldRegionList.Count)
                return Managers.Map.WorldRegionList[OwnerRegionIndex];
            return null;
        }

        /// <summary>
        /// Quest가 속한 Field를 인덱스로부터 가져옵니다.
        /// </summary>
        public Field GetOwnerField()
        {
            if (OwnerFieldIndex >= 0 && OwnerFieldIndex < Managers.Map.WorldFieldList.Count)
                return Managers.Map.WorldFieldList[OwnerFieldIndex];
            return null;
        }
    }
}