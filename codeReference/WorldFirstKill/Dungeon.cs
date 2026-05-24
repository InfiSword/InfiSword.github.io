using System;
using System.Collections.Generic;
using UnityEngine;
using WFK_Challenge.WFK_Map.WFK_Region;
using WFK_Challenge.WFK_Map.WFK_Region.WFK_Field;
using WFK_Challenge.WFK_Quest;
using static Define;

public class Dungeon : Field
{
    public int dungeonID { get; private set; }
    private int dungeonLevel;
    private DungeonMapWin dungeonUI;
    private MonsterType monsterType;

    public override void Init(int _level)
    {
        base.Init(_level);
        dungeonLevel = _level;
        // 던전 전용 초기화
    }

    public override void Enter()
    {
        //if (!isEnter)
        //{
        //    if (quest == null)
        //    {
        //        DungeonQuest _quest = baseQuest as DungeonQuest;
        //        quest = _quest;
        //        monsterType = _quest.m_monsterType;
        //        dungeonID = _quest.GetObjectiveIdentifier();
        //        danger = Managers.Quest.GetDanger(dungeonLevel);
        //        dungeonUI.GenStart(quest.difficulty);
        //    }

        //    List<ProcessMonsterData> allSpawnableMonsterList = Managers.Spawn.enemies;

        //    (int minRate, int maxRate) = Managers.Quest.GetMonsterRatingRange(danger);

        //    List<ProcessMonsterData> monsterSelectedList = new List<ProcessMonsterData>();

        //    Debug.Log($"min,max = {minRate}, {maxRate}");

        //    if (minRate < 5) // minRate가 최대값보다 작다면
        //    {
        //        monsterSelectedList.Add(allSpawnableMonsterList[minRate - 1 + ((int)monsterType)]);
        //        monsterSelectedList.Add(allSpawnableMonsterList[minRate + ((int)monsterType)]);
        //    }
        //    else
        //    {
        //        monsterSelectedList.Add(allSpawnableMonsterList[minRate - 1 + ((int)monsterType)]);
        //        monsterSelectedList.Add(allSpawnableMonsterList[maxRate + ((int)monsterType)]);
        //    }
        //    monster.Clear();

        //    monster = monsterSelectedList;

        //    dungeonUI.gameObject.SetActive(true);
        //    dungeonUI.StartEnterDungeon(this);
        //    isEnter = true;
        //}

       //  SpawnMonster();
    }

    public void UnLockNextNode()
    {
        dungeonUI.UnLockNextField();
    }

    public void MoveNextNode(DungeonNode nextDungeonNode)
    {
        dungeonUI.MoveToNode(nextDungeonNode);

        Enter();
    }

    public void ClearDungeon()
    {
        dungeonUI.gameObject.SetActive(false);
        isEnter = false;
        //quest = null;
        //monster.Clear();
    }

    //public override void SpawnMonster()
    //{
    //    List<(ProcessMonsterData, int)> SetMonster = new List<(ProcessMonsterData, int)>();

    //    DungeonNode node = dungeonUI.GetCurrentNode();
    //    monsterCount = node.monsterCount;

    //    if (node.nodeType == NodeType.NormalMonster)
    //    {
           
    //        if (monsterCount == 8)
    //        {
    //            SetMonster.Add((monster[0], 8));
    //        }
    //        else if (monsterCount == 6)
    //        {
    //            SetMonster.Add((monster[0], 4));
    //            SetMonster.Add((monster[1], 2));
    //        }
    //        else
    //        {
    //            SetMonster.Add((monster[1], 4));
    //        }
    //    }
    //    else if (node.nodeType == NodeType.EliteMonster)
    //    {
    //        if (monsterCount == 4)
    //        {
    //            // 엘리트 몬스터 1, 몬스터 3
    //            // 따로 몬스터 설정을 해줘야 함 
    //            // Ex) 몬스터 크기 설정, 몬스터 스텟 설정
    //            SetMonster.Add((monster[0], 1));
    //            SetMonster.Add((monster[1], 3));
    //        }
    //        else if (monsterCount == 2)
    //        {
    //            // 엘리트 몬스터 2
    //            SetMonster.Add((monster[0], 2));
    //        }

    //    }

    //    if (SetMonster.Count != 0)
    //    {
    //        foreach ((ProcessMonsterData monster, int monsterCount) spawnInfo in SetMonster)
    //        {
    //            Managers.Spawn.AddMob(spawnInfo.monster.monsterId, spawnInfo.monsterCount);
    //        }
    //        Managers.Spawn.FixedGenMonster();
    //        Managers.Spawn.DoStart(quest.m_questType, this);
    //    }
    //    else
    //    {
    //        // 보상방 로직
    //        Debug.Log("보상방");
    //        UnLockNextNode();
    //    }
    //}

}
