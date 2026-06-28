using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 
public class GameManager
{
    public void Init()
    {
        TimeCount = 0;
        DayCount = 1;
        UpdateCount = UPDATE_COUNT.First;
    }
    #region 시간 관리
    public int TimeCount { get; private set; } = 0;  // 0~5 : 총 6개의 시간 카운트, 밤이면 다음날로 넘어감.
    public int DayCount { get; private set; } = 1;   // 1~ : 1부터 시작 증가하는 날짜, 7일 단위로 업데이트가 발생한다.
    public UPDATE_COUNT UpdateCount { get; private set; } = UPDATE_COUNT.First;   // 1~3
    public Action RenewTimeCount; // TimeCount가 업데이트될 때 작동하는 이벤트 (밤으로 넘어가면 작동안함)
    public Action RenewDayCount; // DayCount가 업데이트될 때 작동하는 이벤트 (주간 업데이트가 실행되면 작동안함)
    // 현재 시간을 초기화
    public void SetDate(int timeCount = 0, int dayCount = 1, UPDATE_COUNT updateCount = UPDATE_COUNT.First, bool isFirst = false)
    {
        TimeCount = timeCount;
        DayCount = dayCount;
        UpdateCount = updateCount;

        // 최초 처리시 경우 업데이트 실행
        if (isFirst) InGame_Update();
    }
    // 원하는 만큼 시간을 소모
    public void UsingTime(int timeCount = 1)
    {
        TimeCount += timeCount;

        if (TimeCount >= 6)
        {
            TimeCount -= 6;
            EndOfTheDay();
        }
        else
            if (RenewTimeCount != null) RenewTimeCount.Invoke();
    }
    // 하루 끝 처리
    void EndOfTheDay()
    {
        DayCount++;

        // 7일이 지나면 경우
        if (DayCount % 7 == 0)
            switch (UpdateCount)
            {
                case UPDATE_COUNT.First:
                    UpdateCount = UPDATE_COUNT.Second;
                    InGame_Update();
                    break;
                case UPDATE_COUNT.Second:
                    UpdateCount = UPDATE_COUNT.Third;
                    InGame_Update();
                    break;
            }
        else
            if (RenewDayCount != null) RenewDayCount.Invoke();
    }
    // 주간 업데이트 실행
    void InGame_Update()
    {
        switch (UpdateCount)
        {
            case UPDATE_COUNT.First:
                
                break;
            case UPDATE_COUNT.Second:

                break;
            case UPDATE_COUNT.Third:

                break;
        }
    }
    #endregion
    #region 캐릭터 관리
    public Dictionary<int, MyClass> InGame_Class { get; private set; } = new Dictionary<int, MyClass>();
    // 해당 ShuffleArrayInPlace를 통해서, 특정 Seed값으로 특정 캐릭터 클래스 스킬을 결정을 한다.
    public void RandomClassData()
    {
        InGame_Class.Clear();
        int[] RandomNums = { 0, 1, 2, 3, 4 };
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Class_Melee, RandomNums);
        Managers.nowPlayerData.MeleeClass = new int[] { 101 + RandomNums[0], 101 + RandomNums[1], 101 + RandomNums[2] };
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Class_Dexterity, RandomNums);
        Managers.nowPlayerData.DexterityClass = new int[] { 201 + RandomNums[0], 201 + RandomNums[1], 201 + RandomNums[2] };
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Class_Magic, RandomNums);
        Managers.nowPlayerData.MagicClass = new int[] { 301 + RandomNums[0], 301 + RandomNums[1], 301 + RandomNums[2] };
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Class_Support, RandomNums);
        Managers.nowPlayerData.SupportClass = new int[] { 401 + RandomNums[0], 401 + RandomNums[1], 401 + RandomNums[2] };

        #region 근접 설정
        Managers.nowPlayerData.StartSkill[0, 0] = 100010;
        Managers.nowPlayerData.StartSkill[0, 1] = 100020;
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Melee, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.MeleeSkill[0, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[0]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Melee, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.MeleeSkill[0, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[0]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[0, 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[0]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Melee, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.MeleeSkill[1, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[1]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Melee, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.MeleeSkill[1, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[1]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[0, 3] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[1]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Melee, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.MeleeSkill[2, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[2]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Melee, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.MeleeSkill[2, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[2]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[0, 4] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MeleeClass[2]], RandomNums[4] + 5);
        #endregion

        #region 민첩 설정
        Managers.nowPlayerData.StartSkill[1, 0] = 100110;
        Managers.nowPlayerData.StartSkill[1, 1] = 100120;
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Dexterity, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.DexteritySkill[0, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[0]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Dexterity, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.DexteritySkill[0, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[0]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[1, 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[0]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Dexterity, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.DexteritySkill[1, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[1]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Dexterity, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.DexteritySkill[1, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[1]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[1, 3] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[1]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Dexterity, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.DexteritySkill[2, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[2]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Dexterity, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.DexteritySkill[2, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[2]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[1, 4] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.DexterityClass[2]], RandomNums[4] + 5);
        #endregion

        #region 마법 설정
        Managers.nowPlayerData.StartSkill[2, 0] = 100210;
        Managers.nowPlayerData.StartSkill[2, 1] = 100220;
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Magic, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.MagicSkill[0, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[0]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Magic, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.MagicSkill[0, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[0]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[2, 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[0]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Magic, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.MagicSkill[1, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[1]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Magic, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.MagicSkill[1, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[1]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[2, 3] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[1]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Magic, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.MagicSkill[2, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[2]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Magic, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.MagicSkill[2, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[2]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[2, 4] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.MagicClass[2]], RandomNums[4] + 5);
        #endregion

        #region 지원 설정
        Managers.nowPlayerData.StartSkill[3, 0] = 100310;
        Managers.nowPlayerData.StartSkill[3, 1] = 100320;
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Support, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.SupportSkill[0, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[0]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Support, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.SupportSkill[0, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[0]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[3, 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[0]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Support, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.SupportSkill[1, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[1]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Support, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.SupportSkill[1, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[1]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[3, 3] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[1]], RandomNums[4] + 5);

        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Support, RandomNums);
        for (int i = 0; i < 2; i++)
            Managers.nowPlayerData.SupportSkill[2, i] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[2]], RandomNums[i]);
        GameSeed.ShuffleArrayInPlace(GameSeed.Domain.Skill_Support, RandomNums);
        for (int i = 0; i < 3; i++)
            Managers.nowPlayerData.SupportSkill[2, i + 2] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[2]], RandomNums[i] + 5);
        Managers.nowPlayerData.StartSkill[3, 4] = LoadSkillData(Managers.Data.ClassData[Managers.nowPlayerData.SupportClass[2]], RandomNums[4] + 5);
        #endregion

        InGame_Class.Add(0, SetMyClass(0, new int[] { Managers.nowPlayerData.StartSkill[0, 0], Managers.nowPlayerData.StartSkill[0, 1], Managers.nowPlayerData.StartSkill[0, 2], Managers.nowPlayerData.StartSkill[0, 3], Managers.nowPlayerData.StartSkill[0, 4] }));
        InGame_Class.Add(1, SetMyClass(1, new int[] { Managers.nowPlayerData.StartSkill[1, 0], Managers.nowPlayerData.StartSkill[1, 1], Managers.nowPlayerData.StartSkill[1, 2], Managers.nowPlayerData.StartSkill[1, 3], Managers.nowPlayerData.StartSkill[1, 4] }));
        InGame_Class.Add(2, SetMyClass(2, new int[] { Managers.nowPlayerData.StartSkill[2, 0], Managers.nowPlayerData.StartSkill[2, 1], Managers.nowPlayerData.StartSkill[2, 2], Managers.nowPlayerData.StartSkill[2, 3], Managers.nowPlayerData.StartSkill[2, 4] }));
        InGame_Class.Add(3, SetMyClass(3, new int[] { Managers.nowPlayerData.StartSkill[3, 0], Managers.nowPlayerData.StartSkill[3, 1], Managers.nowPlayerData.StartSkill[3, 2], Managers.nowPlayerData.StartSkill[3, 3], Managers.nowPlayerData.StartSkill[3, 4] }));

        for (int i = 0; i < 3; i++)
        {
            InGame_Class.Add(Managers.nowPlayerData.MeleeClass[i], SetMyClass(Managers.nowPlayerData.MeleeClass[i], new int[] { Managers.nowPlayerData.MeleeSkill[i, 0], Managers.nowPlayerData.MeleeSkill[i, 1], Managers.nowPlayerData.MeleeSkill[i, 2], Managers.nowPlayerData.MeleeSkill[i, 3], Managers.nowPlayerData.MeleeSkill[i, 4] }));
            InGame_Class.Add(Managers.nowPlayerData.DexterityClass[i], SetMyClass(Managers.nowPlayerData.DexterityClass[i], new int[] { Managers.nowPlayerData.DexteritySkill[i, 0], Managers.nowPlayerData.DexteritySkill[i, 1], Managers.nowPlayerData.DexteritySkill[i, 2], Managers.nowPlayerData.DexteritySkill[i, 3], Managers.nowPlayerData.DexteritySkill[i, 4] }));
            InGame_Class.Add(Managers.nowPlayerData.MagicClass[i], SetMyClass(Managers.nowPlayerData.MagicClass[i], new int[] { Managers.nowPlayerData.MagicSkill[i, 0], Managers.nowPlayerData.MagicSkill[i, 1], Managers.nowPlayerData.MagicSkill[i, 2], Managers.nowPlayerData.MagicSkill[i, 3], Managers.nowPlayerData.MagicSkill[i, 4] }));
            InGame_Class.Add(Managers.nowPlayerData.SupportClass[i], SetMyClass(Managers.nowPlayerData.SupportClass[i], new int[] { Managers.nowPlayerData.SupportSkill[i, 0], Managers.nowPlayerData.SupportSkill[i, 1], Managers.nowPlayerData.SupportSkill[i, 2], Managers.nowPlayerData.SupportSkill[i, 3], Managers.nowPlayerData.SupportSkill[i, 4] }));
        }

        Debug.Log("게임에 존재 스킬 : " + InGame_Class.Count);
    }
    MyClass SetMyClass(int classIndex, int[] Skills)
    {
        MyClass tempClass = new MyClass();
        tempClass.ClassIndex = classIndex;
        tempClass.Name = Managers.Data.ClassData[classIndex].m_className;
        tempClass.Description = Managers.Data.ClassData[classIndex].m_description;
        tempClass.Stats = new int[]
        {
            Managers.Data.ClassData[classIndex].m_plusHP,
            Managers.Data.ClassData[classIndex].m_plusAD,
            Managers.Data.ClassData[classIndex].m_plusAP,
            Managers.Data.ClassData[classIndex].m_plusDEF,
            Managers.Data.ClassData[classIndex].m_plusMR
        };
        tempClass.Skills = Skills;
        tempClass.Sprite_Helmet = Managers.Resource.Load<Sprite>($"Sprite/ClassWear/{tempClass.ClassIndex}_{(Define.JobClass)tempClass.ClassIndex}/Helmet");
        tempClass.Sprites_Armor = Managers.Resource.LoadAll<Sprite>($"Sprite/ClassWear/{tempClass.ClassIndex}_{(Define.JobClass)tempClass.ClassIndex}/Armor");
        tempClass.Sprite_Back = Managers.Resource.Load<Sprite>($"Sprite/ClassWear/{tempClass.ClassIndex}_{(Define.JobClass)tempClass.ClassIndex}/Back");
        tempClass.Sprite_Shield = Managers.Resource.Load<Sprite>($"Sprite/ClassWear/{tempClass.ClassIndex}_{(Define.JobClass)tempClass.ClassIndex}/Shield");

        return tempClass;
    }
    int LoadSkillData(Data_Class data, int num)
    {
        switch (num)
        {
            case 0:
                return data.m_actionSkill01;
            case 1:
                return data.m_actionSkill02;
            case 2:
                return data.m_actionSkill03;
            case 3:
                return data.m_actionSkill04;
            case 4:
                return data.m_actionSkill05;
            case 5:
                return data.m_passiveSkill01;
            case 6:
                return data.m_passiveSkill02;
            case 7:
                return data.m_passiveSkill03;
            case 8:
                return data.m_passiveSkill04;
            case 9:
                return data.m_passiveSkill05;
        }
        return 0;
    }
    #endregion

    public void LevelUp()
    {
        Managers.nowPlayerData.PlayerLevel++;
        Managers.nowPlayerData.PlayerEXP -= 100;
        Managers.nowPlayerData.SkillPoint += 4;
        for (int i = 0; i < 4; i++)
        {
            Managers.Battle._playerList[i].LevelUp();
        }
    }
}
