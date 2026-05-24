using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WFK_Challenge.WFK_Quest;
using static Define;

public class QuestContext
{
    public Quest_Difficulty difficultyTag;
    public int questLevel;
    public int danger;
    public int targetID;

    public Data_QuestType questData;
    public Data_QuestDialog dialogData;

    public string description;
    public int count;
    public float value;

    public QuestContext(Quest_Difficulty _difficultyTag, int _questLevel, int _danger, Data_QuestType _questData,
        Data_QuestDialog _dialogData, int _targetID, string _descriptionTemplate, int _count, float _value)
    {
        difficultyTag = _difficultyTag;
        questLevel = _questLevel;
        danger = _danger;
        questData = _questData;
        dialogData = _dialogData;
        targetID = _targetID;
        description = _descriptionTemplate;
        count = _count;
        value = _value;
    }
}

// 제네릭 QuestHandler 구현
public abstract class GenericQuestHandler
{
    public abstract Quest CreateQuest(QuestContext context);
}

// 제네릭 헌팅 퀘스트 핸들러
public class GenericHuntingQuestHandler : GenericQuestHandler
{
    public override Quest CreateQuest(QuestContext context)
    {
        // DataManager에서 몬스터 데이터 가져오기
        if (Managers.Data.MonsterData.TryGetValue(context.targetID, out Data_Monster targetMonsterData))
        {
            string monsterTypeStr = targetMonsterData.m_monsterName;
            string countStr = context.count.ToString();

            string des = context.description;
            if (!string.IsNullOrEmpty(des))
            {
                context.description = des
                    .Replace("N번", monsterTypeStr)
                    .Replace("Count", countStr);
            }

            if (context.dialogData != null && !string.IsNullOrEmpty(context.dialogData.m_dialogStartContext))
            {
                context.dialogData.m_dialogStartContext = context.dialogData.m_dialogStartContext
                    .Replace("N", monsterTypeStr)
                    .Replace("M", countStr);
            }

            // 컨텍스트 기반 생성자 사용
            Quest quest = new Quest(context);
            return quest;
        }
        else
        {
            Debug.LogError($"몬스터 ID {context.targetID}에 해당하는 몬스터 데이터를 찾을 수 없습니다.");
            return null;
        }
    }
}

public class QuestManager : MonoBehaviour
{
    // 메인 퀘스트 목록 (생성된 모든 메인 퀘스트)
    public List<Quest> MainQuestList { get; private set; }

    // 수락한 퀘스트 목록 
    public List<Quest> ActiveQuestList { get; private set; }

    // 완료된 퀘스트 목록 (UI 업데이트용)
    public List<int> CompletedQuestIndexList { get; private set; }

    public void Init()
    {
        MainQuestList = new List<Quest>();
        ActiveQuestList = new List<Quest>();
        CompletedQuestIndexList = new List<int>();

        Managers.Event.OnMonsterKilled -= HandleMonsterKilled;
        Managers.Event.OnMonsterKilled += HandleMonsterKilled;
    }

    /// <summary>
    /// 퀘스트 수락 처리 (키 기반 이벤트 버스에서 호출)
    /// </summary>
    private void QuestAccepted(int questIndex)
    {
        if (questIndex < 0 || questIndex >= MainQuestList.Count)
        {
            Debug.LogWarning($"유효하지 않은 퀘스트 인덱스입니다: {questIndex}");
            return;
        }

        Quest quest = MainQuestList[questIndex];
        
        if (ActiveQuestList.Contains(quest))
        {
            Debug.LogWarning($"퀘스트 '{quest.m_questName}'는 이미 수락된 상태입니다.");
            return;
        }

        ActiveQuestList.Add(quest);
        Debug.Log($"퀘스트 '{quest.m_questName}'를 수락했습니다.");
    }

    #region 퀘스트 진행도 및 완료 처리

    // 몬스터 처치 시 호출되는 핸들러
    // 관련 활성 퀘스트들의 진행도를 업데이트하고 완료 체크를 수행
    private void HandleMonsterKilled(int monsterId)
    {
        var key = (typeof(Unit_Monster), monsterId);
        List<int> completedQuestIndices = new List<int>();
        List<Quest> questsToComplete = new List<Quest>(); // 완료할 퀘스트를 임시 저장

        foreach (Quest quest in ActiveQuestList)
        {
            if (IsTargetQuest(quest, key))
            {
                // 진행도 업데이트
                UpdateQuestProgress(quest);

                if (quest.IsQuestCompleted())
                {
                    questsToComplete.Add(quest);
                }
            }
        }

        // 완료된 퀘스트들을 처리
        foreach (Quest quest in questsToComplete)
        {
            int questIndex = CompleteQuest(quest);
            if (questIndex >= 0)
            {
                completedQuestIndices.Add(questIndex);
            }
        }

        // 완료된 퀘스트가 있으면 List로 한 번에 이벤트 발행
        if (completedQuestIndices.Count > 0)
        {
            // 이벤트 발행 (UI 업데이트용)
            Managers.Event.QuestCompleted(completedQuestIndices);

            // UI 업데이트 후 완료된 퀘스트 인덱스를 목록에서 제거
            foreach (int questIndex in completedQuestIndices)
            {
                CompletedQuestIndexList.Remove(questIndex);
            }

            Debug.Log($"[QuestManager] {completedQuestIndices.Count} quest(s) completed: {string.Join(", ", completedQuestIndices)}");
        }
    }

    private bool IsTargetQuest(Quest quest, (Type, int) targetKey)
    {
        return quest.GetTargetType() == targetKey.Item1 && quest.GetTargetID() == targetKey.Item2;
    }

    // 퀘스트 진행도를 업데이트하고 진행도 변경 이벤트를 발행
    private void UpdateQuestProgress(Quest quest)
    {
        // 진행도 증가
        quest.IncrementProgress(1);

        // 퀘스트 인덱스 찾기
        int questIndex = MainQuestList.IndexOf(quest);

        // 진행도 변경 이벤트 발행 (UI 업데이트용) - 퀘스트 인덱스와 함께 전달
        int currentProgress = quest.GetCurrentProgress();
        Managers.Event.QuestProgressChanged(questIndex, currentProgress);

    }

    // 퀘스트 완료 처리를 수행
    private int CompleteQuest(Quest quest)
    {
        if (!ActiveQuestList.Contains(quest))
        {
            Debug.LogWarning($"Quest {quest.m_questName} is not in ActiveQuestList");
            return -1;
        }

        // 활성 퀘스트 목록에서 제거
        ActiveQuestList.Remove(quest);

        // 퀘스트 인덱스 찾기
        int questIndex = MainQuestList.IndexOf(quest);
        if (questIndex < 0)
        {
            Debug.LogError($"Quest {quest.m_questName} not found in MainQuestList");
            return -1;
        }

        // 완료된 퀘스트 목록에 추가
        if (!CompletedQuestIndexList.Contains(questIndex))
        {
            CompletedQuestIndexList.Add(questIndex);
        }

        // 보상 지급
        // GiveQuestReward(quest);

        Debug.Log($"Quest completed: {quest.m_questName} Index: {questIndex})");

        return questIndex;
    }
    #endregion

    // 퀘스트 생성 코드--------------------------------------------------------------------------------------------

    private const int MAX_REGION_LEVEL = 20;
    private const int MAX_QUEST_LEVEL = 25;

    // 메인 퀘스트 레벨별 위험도 세팅 테이블
    public static readonly Dictionary<(int startLevel, int endLevel), (int minDanager, int maxDanager)>
        dangerTable = new Dictionary<(int, int), (int, int)>
        {
            { (1, 2), (1, 1) },
            { (3, 4), (1, 2) },
            { (5, 6), (2, 2) },
            { (7, 8), (2, 3) },
            { (9, 10), (3, 4) },
            { (11, 12), (5, 5) },
            { (13, 14), (5, 6) },
            { (15, 16), (6, 7) },
            { (17, 18), (7, 8) },
            { (19, 20), (8, 9) },
        };

    // 키 값은 위험도, 벨류 값은 몬스터의 등급인 테이블
    public static readonly Dictionary<int, (int minRating, int maxRating)>
        monsterRatingTable = new Dictionary<int, (int, int)>
        {
            { (1), (1,1) },
            { (2), (1,2) },
            { (3), (2,2) },
            { (4), (2,3) },
            { (5), (3,3) },
            { (6), (3,4) },
            { (7), (4,4) },
            { (8), (4,5) },
            { (9), (5,5) },
        };

    // 메인 퀘스트 레벨에 따른 위험도 반환
    public int GetDanger(int level)
    {
        // MAX_REGION_LEVEL을 초과하는 레벨은 클램핑
        if (level > MAX_REGION_LEVEL)
        {
            level = MAX_REGION_LEVEL;
        }

        foreach (var entry in dangerTable)
        {
            var (start, end) = entry.Key;
            if (level >= start && level <= end)
            {
                var (min, max) = entry.Value;
                return GameSeed.NextInt(GameSeed.Domain.Quest, min, max + 1);
            }
        }
        return -1;
    }

    // 위험도에 따른 몬스터 등급 반환
    public (int minRating, int maxRating) GetMonsterRatingRange(int danger)
    {
        if (monsterRatingTable.TryGetValue(danger, out var ratingRange))
        {
            return ratingRange;
        }
        Debug.LogWarning($"위험도 {danger} 에 대한 등급 정보가 없습니다.");
        return (-1, -1);
    }


    #region 퀘스트 랜덤 생성 로직
    public Quest GenerateQuest(MonsterSet monsterSet, ref int questLevel, List<Data_QuestType> questTypeDataList,
    List<Data_QuestDialog> questDialogDataList)
    {
        var (questData, dialogData) = GetRandomQuestData(questTypeDataList, questDialogDataList);

        Quest_Type questType = (Quest_Type)Enum.Parse(typeof(Quest_Type), questData.m_questType);
        Quest_Difficulty difficultyTag = GameSeed.RandomEnum<Quest_Difficulty>(GameSeed.Domain.Quest, v => !EqualityComparer<Quest_Difficulty>.Default.Equals(v, Quest_Difficulty.None));

        var difficultyValues = new Dictionary<Quest_Difficulty, (int count, float value)>
        {
            { Define.Quest_Difficulty.Easy, (questData.m_easyCount, questData.m_easyValue) },
            { Define.Quest_Difficulty.Normal, (questData.m_normalCount, questData.m_normalValue) },
            { Define.Quest_Difficulty.Hard, (questData.m_hardCount, questData.m_hardValue) }
        };

        if (!difficultyValues.TryGetValue(difficultyTag, out var data))
        {
            Debug.LogError("유효하지 않은 퀘스트 난이도입니다!");
            return null;
        }

        // 대화 데이터 복제
        Data_QuestDialog clonedDialogData = dialogData != null ? new Data_QuestDialog
        {
            m_dialogIndex = dialogData.m_dialogIndex,
            m_dialogName = dialogData.m_dialogName,
            m_dialogQuestType = dialogData.m_dialogQuestType,
            m_dialogRewardContext = dialogData.m_dialogRewardContext,
            m_dialogStartContext = dialogData.m_dialogStartContext,
        } : null;

        if (MAX_REGION_LEVEL < questLevel)
            questLevel = MAX_REGION_LEVEL;

        int danger = GetDanger(questLevel);

        Quest newQuest = null;
        QuestContext questContext = null;

        switch (questType)
        {
            case Quest_Type.Hunting:
            case Quest_Type.Rare_Hunting:
                (int min, int max) = GetMonsterRatingRange(danger);
                int selectedGrade = GameSeed.NextInt(GameSeed.Domain.Quest, min, max + 1);
                int monsterID = monsterSet.GetMonsterID(selectedGrade);

                if (!Managers.Data.MonsterData.ContainsKey(monsterID))
                {
                    Debug.LogError("해당 몬스터 ID 데이터가 없음");
                    return null;
                }

                questContext = new QuestContext(
                    difficultyTag,
                    questLevel,
                    danger,
                    questData,
                    clonedDialogData,
                    monsterID,
                    questData.m_questDescription,
                    data.count,
                    data.value
                );
                break;
            default:
                Debug.LogError($"지원하지 않는 퀘스트 타입입니다: {questType}");
                return null;
        }

        if (questContext != null)
        {
            switch (questType)
            {
                case Quest_Type.Hunting:
                case Quest_Type.Rare_Hunting:
                    newQuest = new GenericHuntingQuestHandler().CreateQuest(questContext);
                    break;
            }
        }
        else
            Debug.LogError("Error! No QuestContext");

        if (newQuest != null)
        {
            questLevel += GameSeed.NextInt(GameSeed.Domain.Quest, 0, 3);
            int clampLevel = Mathf.Min(questLevel, MAX_QUEST_LEVEL);
            questLevel = clampLevel;

            MainQuestList.Add(newQuest);
            
            // 새로 생성된 퀘스트의 이벤트 구독 등록
            int questIndex = MainQuestList.Count - 1;
            Managers.Event.SubscribeQuestAccepted(questIndex, QuestAccepted);
        }

        return newQuest;
    }

    // 퀘스트 종류와, 다이어로그 랜덤 배정
    private (Data_QuestType, Data_QuestDialog) GetRandomQuestData(List<Data_QuestType> questTypeDataList,
 List<Data_QuestDialog> questDialogDataList)
    {
        // Hunting과 Rare_Hunting 퀘스트 타입만 필터링
        List<Data_QuestType> huntingQuestTypes = questTypeDataList
            .Where(q => q.m_questType == "Hunting" || q.m_questType == "Rare_Hunting")
            .ToList();

        int questTypeIndex = GameSeed.NextInt(GameSeed.Domain.Quest, 0, huntingQuestTypes.Count);
        Data_QuestType questData = huntingQuestTypes[questTypeIndex];
        Data_QuestDialog selectedDialog = GetRandomDialogData(questData.m_questType, questDialogDataList);

        Debug.Log($"선택된 퀘스트 타입: {questData.m_questType}");
        return (questData, selectedDialog);
    }

    private Data_QuestDialog GetRandomDialogData(string questType, List<Data_QuestDialog> questDialogList)
    {
        List<Data_QuestDialog> filteredDialogs = questDialogList.Where(dialog => dialog.m_dialogQuestType == questType).ToList();

        if (filteredDialogs.Count == 0)
        {
            Debug.LogWarning($"dialog 가 존재하지 않습니다.: {questType}");
            return null;
        }

        int randomIndex = GameSeed.NextInt(GameSeed.Domain.Dialog, 0, filteredDialogs.Count);
        return filteredDialogs[randomIndex];
    }
    #endregion
}
