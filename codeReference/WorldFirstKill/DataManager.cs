using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class DataManager
{
    #region 데이터 자료
    // 몬스터 데이터 가져오기
    public Dictionary<int, Data_Monster> MonsterData { get; private set; }
    public Dictionary<int, Data_QuestType> QuestTypeData { get; private set; }
    public Dictionary<int, Data_QuestDialog> QuestDialogData { get; private set; }

    //----------------------------------------------------------------------------------------------------------------//

    public Dictionary<int, Effects_Base> Effects { get; private set; }
    public Dictionary<int, Item> Items { get; private set; }
    public Dictionary<int, Buff> Buffs { get; private set; }
    public Dictionary<int, ActiveSkill> ActiveSkills { get; private set; }
    public Dictionary<int, PassiveSkill> PassiveSkills { get; private set; }

    public Dictionary<int, Data_TargetPoint> AggroTypes { get; private set; }

    public Dictionary<int, Data_Resource> ResourceData { get; private set; }

    public Dictionary<int, Data_EquipmentStat> EquipStatData { get; private set; }
    public Dictionary<int, Data_Weapon> WeaponData { get; private set; }
    public Dictionary<int, Data_Armor> ArmorData { get; private set; }
    public Dictionary<int, Data_Acessori> AcessoriData { get; private set; }
    public Dictionary<int, Data_Consumable_Player> ConsumablePlayerData { get; private set; }
    public Dictionary<int, Data_Consumable_User> ConsumableUserData { get; private set; }
    public Dictionary<int, Data_Material_Monster> MaterialMonsterData { get; private set; }
    public Dictionary<int, Data_Material_Quest> MaterialQuestData { get; private set; }
    public Dictionary<int, Data_Material_Engravement> MaterialEngravementData { get; private set; }
    public Dictionary<int, Data_CC> CCData { get; private set; }
    public Dictionary<int, Data_Class> ClassData { get; private set; }

    #endregion

    int DataNum = 1;
    string FilePath;

    public void Init()
    {
        FilePath = Application.persistentDataPath + "/save";

        Effects = SetEffectData();
        AggroTypes = ReadAggroTypeData();
    }

    public void SaveData()
    {
        PlayerData saveData = new PlayerData();

        if (Managers.nowPlayerData == null) return;

        saveData = Managers.nowPlayerData;

        string data = JsonUtility.ToJson(saveData);
        File.WriteAllText(FilePath + DataNum, data);
    }
    public void LoadData()
    {
        if (!File.Exists(FilePath + DataNum)) return;

        string data = File.ReadAllText(FilePath);

        PlayerData LoadData = JsonUtility.FromJson<PlayerData>(data);

        Managers.nowPlayerData = LoadData;

        Managers.Sound.BGMSoundVolume = Managers.nowPlayerData.BGMSound;
        Managers.Sound.SFXSoundVolume = Managers.nowPlayerData.SFXSound;
    }

    Dictionary<int, Effects_Base> SetEffectData()
    {
        Dictionary<int, Effects_Base> Dict = new Dictionary<int, Effects_Base>
        {
            { 0, new Effects_0() },
            { 1, new Effects_1() },
            { 2, new Effects_2() },
            { 3, new Effects_3() },
            { 4, new Effects_4() },
            { 5, new Effects_5() },
            { 6, new Effects_6() },
            { 7, new Effects_7() },
            { 8, new Effects_8() },
            { 9, new Effects_9() },
            { 10, new Effects_10() },
            { 11, new Effects_11() },
            { 12, new Effects_12() },
        };

        return Dict;
    }
    Dictionary<int, Buff> ReadBuffData(Dictionary<int, Data_Buff> buffs)
    {
        Dictionary<int, Buff> Dict = new Dictionary<int, Buff>();

        foreach (var Buffs in buffs)
        {
            Buff tempBuff = new Buff();
            Data_Buff data_Buff = Buffs.Value;

            tempBuff.BuffIndex = data_Buff.m_buffIndex;
            tempBuff.BuffName = data_Buff.m_buffName;
            tempBuff.Duration = data_Buff.m_duration;
            tempBuff.Transform = data_Buff.m_transfrom;
            tempBuff.Barrier = data_Buff.m_barrier;
            tempBuff.AddHP = data_Buff.m_addHP;
            tempBuff.AddAD = data_Buff.m_addAD;
            tempBuff.AddAP = data_Buff.m_addAP;
            tempBuff.AddDefence = data_Buff.m_addDefence;
            tempBuff.AddMR = data_Buff.m_addMR;
            tempBuff.AddMoveSpeed = data_Buff.m_addMoveSpeed;
            tempBuff.AddAttackSpeed = data_Buff.m_addAttackSpeed;
            tempBuff.AddAttackRange = data_Buff.m_addAttackRange;
            tempBuff.AD_Percent = data_Buff.m_ad_Percent;
            tempBuff.AP_Percent = data_Buff.m_ap_Percent;
            tempBuff.Defence_Percent = data_Buff.m_defence_Percent;
            tempBuff.MR_Percent = data_Buff.m_mr_Percent;
            tempBuff.AD_Reduction = data_Buff.m_ad_Reduction;
            tempBuff.AP_Reduction = data_Buff.m_ap_Reductione;
            tempBuff.AddCriticalChance = data_Buff.m_addCriticalChance;
            tempBuff.ADDCriticalDamage = data_Buff.m_addCriticalDamage;
            tempBuff.AD_Damage = data_Buff.m_ad_Damage;
            tempBuff.AP_Damage = data_Buff.m_ap_Damage;
            tempBuff.Fire_Damage = data_Buff.m_fire_Damage;
            tempBuff.Water_Damage = data_Buff.m_water_Damage;
            tempBuff.Electro_Damage = data_Buff.m_electro_Damage;
            tempBuff.Ground_Damage = data_Buff.m_ground_Damage;
            tempBuff.Light_Damage = data_Buff.m_light_Damage;
            tempBuff.Dark_Damage = data_Buff.m_dark_Damage;
            tempBuff.AD_Reduction = data_Buff.m_ad_Reduction;
            tempBuff.AP_Reduction = data_Buff.m_ap_Reductione;
            tempBuff.Fire_Reduction = data_Buff.m_fire_Reduction;
            tempBuff.Water_Reduction = data_Buff.m_water_Reduction;
            tempBuff.Electro_Reduction = data_Buff.m_electro_Reductione;
            tempBuff.Ground_Reduction = data_Buff.m_ground_Reductione;
            tempBuff.Light_Reduction = data_Buff.m_light_Reduction;
            tempBuff.Dark_Reduction = data_Buff.m_dark_Reduction;

            Dict.Add(Buffs.Key, tempBuff);
        }

        return Dict;
    }
    Dictionary<int, ActiveSkill> ReadActiveSkillData(Dictionary<int, Data_ActiveSkill> activeSkills)
    {
        Dictionary<int, ActiveSkill> Dict = new Dictionary<int, ActiveSkill>();

        foreach (var ActiveSkill in activeSkills)
        {
            ActiveSkill tempActice = new ActiveSkill();
            Data_ActiveSkill skills = ActiveSkill.Value;

            string[] effectList = skills.m_EffectList.Split("/");
            string[] effectRatio = skills.m_EffectRatio.Split("/");

            tempActice.SkillIndex = skills.m_skillIndex;
            tempActice.SkillName = skills.m_skillName;
            tempActice.Description = skills.m_Describtion;
            tempActice.EffectList = new int[effectList.Length];
            tempActice.EffectRatio = new int[effectRatio.Length];
            tempActice.isAD = skills.m_isAD == "TRUE" ? true : false;
            tempActice.MyTeam = skills.m_MyTeam == "TRUE" ? true : false;
            tempActice.MissileIndex = skills.m_missileIndex;
            tempActice.AEOIndex = skills.m_AEOIndex;
            tempActice.MotionIndex = skills.m_motionIndex;
            tempActice.SkillRange = skills.m_skillRange;
            switch (skills.m_skillType)
            {
                case "Fire": tempActice.SkillType = Define.AttackType.Fire; break;
                case "Water": tempActice.SkillType = Define.AttackType.Water; break;
                case "Electro": tempActice.SkillType = Define.AttackType.Electro; break;
                case "Ground": tempActice.SkillType = Define.AttackType.Ground; break;
                case "Light": tempActice.SkillType = Define.AttackType.Light; break;
                case "Dark": tempActice.SkillType = Define.AttackType.Dark; break;
                default: tempActice.SkillType = Define.AttackType.None; break;

            }
            tempActice.EffectRange = skills.m_effectRange;
            tempActice.TargetCount = skills.m_targetCount;
            tempActice.Delay = skills.m_delay;
            tempActice.CoolDown = skills.m_coolDown;
            tempActice.MyEffect = skills.m_myEffect;
            tempActice.TargetEffect = skills.m_targetEffect;

            for (int i = 0; i < effectList.Length; i++)
                tempActice.EffectList[i] = Convert.ToInt32(effectList[i]);
            for (int i = 0; i < effectRatio.Length; i++)
                tempActice.EffectRatio[i] = Convert.ToInt32(effectRatio[i]);

            tempActice.SkillIcon = Managers.Resource.Load<Sprite>($"Sprite/SkillIcon/Skill_{(int)(tempActice.SkillIndex * 0.1) * 10}");

            Dict.Add(ActiveSkill.Key, tempActice);
        }

        return Dict;
    }
    Dictionary<int, PassiveSkill> ReadPassiveSkillData(Dictionary<int, Data_PassiveSkill> passiveSkills)
    {
        Dictionary<int, PassiveSkill> Dict = new Dictionary<int, PassiveSkill>();

        foreach (var PassiveSkill in passiveSkills)
        {
            PassiveSkill tempPassive = new PassiveSkill();
            Data_PassiveSkill skills = PassiveSkill.Value;

            string[] effectList = skills.m_EffectList.Split("/");
            string[] effectRatio = skills.m_EffectRatio.Split("/");

            tempPassive.SkillIndex = skills.m_skillIndex;
            tempPassive.SkillName = skills.m_skillName;
            tempPassive.Description = skills.m_Describtion;
            tempPassive.Trigger = (Define.TriggerType)Enum.Parse(typeof(Define.TriggerType), skills.m_trigger);
            tempPassive.TriggerCondition = (Define.TriggerCondition)Enum.Parse(typeof(Define.TriggerCondition), skills.m_triggerCondition);
            tempPassive.TriggerValue = skills.m_triggerValue;
            tempPassive.EffectList = new int[effectList.Length];
            tempPassive.EffectRatio = new int[effectRatio.Length];
            tempPassive.isAD = skills.m_isAD == "TRUE"? true:false ;
            tempPassive.MyTeam = skills.m_MyTeam == "TRUE" ? true : false;
            tempPassive.MissileIndex = skills.m_missileIndex;
            tempPassive.AEOIndex = skills.m_AEOIndex;
            tempPassive.EffectRange = skills.m_effectRange;
            tempPassive.TargetCount = skills.m_targetCount;
            tempPassive.CoolDown = skills.m_coolDown;
            tempPassive.MyEffect = skills.m_myEffect;
            tempPassive.TargetEffect = skills.m_targetEffect;

            for (int i = 0; i < effectList.Length; i++)
                tempPassive.EffectList[i] = Convert.ToInt32(effectList[i]);
            for (int i = 0; i < effectRatio.Length; i++)
                tempPassive.EffectRatio[i] = Convert.ToInt32(effectRatio[i]);

            tempPassive.SkillIcon = Managers.Resource.Load<Sprite>($"Sprite/SkillIcon/Skill_{(int)(tempPassive.SkillIndex * 0.1) * 10}");

            Dict.Add(PassiveSkill.Key, tempPassive);
        }

        return Dict;
    }
    Dictionary<int, Data_TargetPoint> ReadAggroTypeData()
    {
        Dictionary<int, Data_TargetPoint> Dict = new Dictionary<int, Data_TargetPoint>();
        List<Dictionary<string, object>> PointData = CSVReader.Read("TargetPoint");

        // 데이터의 0번줄은 데이터 타입이므로 i=1 에서부터 시작
        for (int i = 0; i < PointData.Count; i++) // i=1부터 시작
        {
            int num = Convert.ToInt32(PointData[i]["Index"]); // 번호 설정

            // 객체 생성
            Dict.Add(num, new Data_TargetPoint(
                num,
                Convert.ToString(PointData[i]["Name"]),
                Convert.ToInt32(PointData[i]["Damage"]),
                Convert.ToInt32(PointData[i]["Defence"]),
                Convert.ToInt32(PointData[i]["Support"]),
                Convert.ToInt32(PointData[i]["Obstruct"]),
                Convert.ToInt32(PointData[i]["HPratio"]),
                Convert.ToInt32(PointData[i]["Targetted"]),
                Convert.ToInt32(PointData[i]["Stunted"]),
                Convert.ToInt32(PointData[i]["Distance"])
            ));
        }
        return Dict;
    }

    public async Task ServerDataLoadAsync()
    {
        MonsterData = new Dictionary<int, Data_Monster>();
        List<Data_Monster> TempMonsterList = ServerCSV_ConvertData.GetParseData<Data_Monster>(Define.Unit_Attribute.Monster);
        for (int i = 0; i < TempMonsterList.Count; i++)
            MonsterData.Add(TempMonsterList[i].m_monsterIndex, TempMonsterList[i]);

        QuestTypeData = new Dictionary<int, Data_QuestType>();
        List<Data_QuestType> TempQuestList = ServerCSV_ConvertData.GetParseData<Data_QuestType>(Define.Quest_Attribute.Type);
        for (int i = 0; i < TempQuestList.Count; i++)
            QuestTypeData.Add(TempQuestList[i].m_id, TempQuestList[i]);

        QuestDialogData = new Dictionary<int, Data_QuestDialog>();
        List<Data_QuestDialog> TempQuestDialogList = ServerCSV_ConvertData.GetParseData<Data_QuestDialog>(Define.Quest_Attribute.Dialog);
        for (int i = 0; i < TempQuestDialogList.Count; i++)
            QuestDialogData.Add(TempQuestDialogList[i].m_dialogIndex, TempQuestDialogList[i]);

        WeaponData = new Dictionary<int, Data_Weapon>();
        List<Data_Weapon> TempWeaponList = ServerCSV_ConvertData.GetParseData<Data_Weapon>(Define.Item_Attribute.Weapon);
        for (int i = 0; i < TempWeaponList.Count; i++)
            WeaponData.Add(TempWeaponList[i].m_index, TempWeaponList[i]);

        ArmorData = new Dictionary<int, Data_Armor>();
        List<Data_Armor> TempArmorList = ServerCSV_ConvertData.GetParseData<Data_Armor>(Define.Item_Attribute.Armor);
        for (int i = 0; i < TempArmorList.Count; i++)
            ArmorData.Add(TempArmorList[i].m_index, TempArmorList[i]);

        AcessoriData = new Dictionary<int, Data_Acessori>();
        List<Data_Acessori> TempAcessoriList = ServerCSV_ConvertData.GetParseData<Data_Acessori>(Define.Item_Attribute.Acessori);
        for (int i = 0; i < TempAcessoriList.Count; i++)
            AcessoriData.Add(TempAcessoriList[i].m_index, TempAcessoriList[i]);

        ConsumablePlayerData = new Dictionary<int, Data_Consumable_Player>();
        List<Data_Consumable_Player> TempConsumablePlayerList = ServerCSV_ConvertData.GetParseData<Data_Consumable_Player>(Define.Item_Attribute.ConsumablePlayer);
        for (int i = 0; i < TempConsumablePlayerList.Count; i++)
            ConsumablePlayerData.Add(TempConsumablePlayerList[i].m_index, TempConsumablePlayerList[i]);

        ConsumableUserData = new Dictionary<int, Data_Consumable_User>();
        List<Data_Consumable_User> TempConsumableUserList = ServerCSV_ConvertData.GetParseData<Data_Consumable_User>(Define.Item_Attribute.ConsumableUser);
        for (int i = 0; i < TempConsumableUserList.Count; i++)
            ConsumableUserData.Add(TempConsumableUserList[i].m_index, TempConsumableUserList[i]);

        MaterialMonsterData = new Dictionary<int, Data_Material_Monster>();
        List<Data_Material_Monster> TempMaterialMonsterList = ServerCSV_ConvertData.GetParseData<Data_Material_Monster>(Define.Item_Attribute.MaterialMonster);
        for (int i = 0; i < TempMaterialMonsterList.Count; i++)
            MaterialMonsterData.Add(TempMaterialMonsterList[i].m_index, TempMaterialMonsterList[i]);

        MaterialQuestData = new Dictionary<int, Data_Material_Quest>();
        List<Data_Material_Quest> TempMaterialQuestList = ServerCSV_ConvertData.GetParseData<Data_Material_Quest>(Define.Item_Attribute.MaterialQuest);
        for (int i = 0; i < TempMaterialQuestList.Count; i++)
            MaterialQuestData.Add(TempMaterialQuestList[i].m_index, TempMaterialQuestList[i]);

        MaterialEngravementData = new Dictionary<int, Data_Material_Engravement>();
        List<Data_Material_Engravement> TempMaterialEngravementList = ServerCSV_ConvertData.GetParseData<Data_Material_Engravement>(Define.Item_Attribute.MaterialEngravement);
        for (int i = 0; i < TempMaterialEngravementList.Count; i++)
            MaterialEngravementData.Add(TempMaterialEngravementList[i].m_index, TempMaterialEngravementList[i]);

        ResourceData = new Dictionary<int, Data_Resource>();
        List<Data_Resource> TempResourceData = ServerCSV_ConvertData.GetParseData<Data_Resource>(Define.Region_Attribute.Resource, true, 0, 5);
        for (int i = 0; i < TempResourceData.Count; i++)
            ResourceData.Add(TempResourceData[i].regionLevel, TempResourceData[i]);

        EquipStatData = new Dictionary<int, Data_EquipmentStat>();
        List<Data_EquipmentStat> TempStatList = ServerCSV_ConvertData.GetParseData<Data_EquipmentStat>(Define.Item_Attribute.EquipStat);
        for (int i = 0; i < TempStatList.Count; i++)
            EquipStatData.Add(TempStatList[i].m_no, TempStatList[i]);

        CCData = new Dictionary<int, Data_CC>();
        List<Data_CC> TempCCList = ServerCSV_ConvertData.GetParseData<Data_CC>(Define.Game_Attribute.CC);
        for (int i = 0; i < TempCCList.Count; i++)
            CCData.Add(TempCCList[i].m_index, TempCCList[i]);

        ClassData = new Dictionary<int, Data_Class>();
        List<Data_Class> TempClassList = ServerCSV_ConvertData.GetParseData<Data_Class>(Define.Unit_Attribute.Class);
        for (int i = 0; i < TempClassList.Count; i++)
            ClassData.Add(TempClassList[i].m_classIndex, TempClassList[i]);

        Dictionary<int, Data_ActiveSkill> ActiveSkillData = new Dictionary<int, Data_ActiveSkill>();
        List<Data_ActiveSkill> TempActiveSkillList = ServerCSV_ConvertData.GetParseData<Data_ActiveSkill>(Define.Skill_Attribute.Active);
        for (int i = 0; i < TempActiveSkillList.Count; i++)
            ActiveSkillData.Add(TempActiveSkillList[i].m_skillIndex, TempActiveSkillList[i]);

        Dictionary<int, Data_PassiveSkill> PassiveSkillData = new Dictionary<int, Data_PassiveSkill>();
        List<Data_PassiveSkill> TempPassiveList = ServerCSV_ConvertData.GetParseData<Data_PassiveSkill>(Define.Skill_Attribute.Passive);
        for (int i = 0; i < TempPassiveList.Count; i++)
            PassiveSkillData.Add(TempPassiveList[i].m_skillIndex, TempPassiveList[i]);

        Dictionary<int, Data_Buff> BuffData = new Dictionary<int, Data_Buff>();
        List<Data_Buff> TempBuffList = ServerCSV_ConvertData.GetParseData<Data_Buff>(Define.Skill_Attribute.Buff);
        for (int i = 0; i < TempBuffList.Count; i++)
            BuffData.Add(TempBuffList[i].m_buffIndex, TempBuffList[i]);

        ActiveSkills = ReadActiveSkillData(ActiveSkillData);
        PassiveSkills = ReadPassiveSkillData(PassiveSkillData);
        Buffs = ReadBuffData(BuffData);

        await Task.Yield(); // UI 업데이트를 위해 양보

        Debug.Log($"데이터 로드 완료: {WeaponData.Count}, {ArmorData.Count}, {AcessoriData.Count}, {ConsumablePlayerData.Count}, {ConsumableUserData.Count}, {MaterialMonsterData.Count}, {MaterialQuestData.Count}, {MaterialEngravementData.Count}");
    }
}
