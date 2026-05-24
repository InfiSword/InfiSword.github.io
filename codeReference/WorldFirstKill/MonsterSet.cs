using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterSet
{
    private const int MAX_MONSTER_GRADE = 5;
    public MonsterType monsterType { get; private set; }

    private GameObject[] monsters = new GameObject[MAX_MONSTER_GRADE];
    public MonsterSet(MonsterType _monsterType)
    {
        monsterType = _monsterType;

        for(int i = 0; i< monsters.Length; i++)
        {
            int key = i + (int)monsterType;
            if (Managers.Spawn.monsterPrefabs.ContainsKey(key))
            {
                monsters[i] = Managers.Spawn.monsterPrefabs[key];
            }
            else
            {
                Debug.LogWarning($"MonsterSet: 키 '{key}'가 monsterPrefabs 딕셔너리에 존재하지 않습니다. MonsterType: {monsterType}, Index: {i}");
                monsters[i] = null; 
            }
        }
    }

    public MonsterType GetMonsterType()
    {
        return monsterType;
    }

    public static MonsterType GetMonsterType(int monsterID)
    {
        int typeIndex = monsterID / MAX_MONSTER_GRADE;
        int calculatedTypeValue = typeIndex * MAX_MONSTER_GRADE;
        
        if (Enum.IsDefined(typeof(MonsterType), calculatedTypeValue))
        {
            return (MonsterType)calculatedTypeValue;
        }
        
        return MonsterType.Golem; // 기본값
    }

    public GameObject[] GetMonster()
    {
        return monsters;
    }

    public static int GetMonsterGrade(int monsterID)
    {
        int grade = (monsterID % MAX_MONSTER_GRADE) + 1; 
        return grade;
    }

    public static (MonsterType type, int grade) DecomposeMonsterID(int monsterID)
    {
        MonsterType type = GetMonsterType(monsterID);
        int grade = GetMonsterGrade(monsterID);
        return (type, grade);
    }

    // ID 추출
    public int ComposeMonsterID(int grade)
    {
        return (int)monsterType + (grade - 1);
    }

    public static int ComposeMonsterID(MonsterType type, int grade)
    {
        return (int)type + (grade - 1);
    }

    // ID 획득
    public int GetMonsterID(int monsterGrade)
    {
        return (int)monsterType + monsterGrade - 1; 
    }

    public static int GetMonsterID(MonsterType type, int monsterGrade)
    {
        return (int)type + monsterGrade - 1;
    }

    // 주어진 등급 범위에 해당하는 몬스터 ID 리스트 반환
    public List<int> GetMonstersID(int minGrade, int maxGrade)
    {
        List<int> monsterIDs = new List<int>();
        
        // min 등급 추가
        if (minGrade >= 1 && minGrade <= MAX_MONSTER_GRADE)
        {
            int minID = GetMonsterID(minGrade);
            if (Managers.Spawn.monsterPrefabs.ContainsKey(minID))
            {
                monsterIDs.Add(minID);
            }
        }
        
        // max 등급 추가 (min과 다를 때만)
        if (maxGrade >= 1 && maxGrade <= MAX_MONSTER_GRADE && maxGrade != minGrade)
        {
            int maxID = GetMonsterID(maxGrade);
            if (Managers.Spawn.monsterPrefabs.ContainsKey(maxID))
            {
                monsterIDs.Add(maxID);
            }
        }
        
        return monsterIDs;
    }
}
