using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum CSVDataType
    {
        None,
        Quest,
        Region,
        Item,
        Game,
        Skill,
        Unit,
        End,
    }
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        Press,
    }

    public enum MouseEvent
    {
        Press,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    public enum SceneLayer
    {
        BackGround,
        Defalt,
        SceneUI,
        PopupUI,
        OverPopup
    }

    public enum FadeMove
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FADE
    }
    public enum UPDATE_COUNT
    {
        First,
        Second,
        Third,
    }
    // 각 배경음악
    public enum BGM
    {
        Count
    }
    // 각 효과음
    public enum SFX
    {
        Count
    }
    public enum StatCode
    {
        HP,
        AD,
        AP,
        DEF,
        MR,
        MoveSpeed,
        AttackSkill,
        AttackRange,
        AttackSpeed,
        AddHP,
        AddAD,
        AddAP,
        AddDefence,
        AddMR,
        AddMoveSpeed,
        AddCriticalChance,
        AD_Percent,
        AP_Percent,
        Defence_Percent,
        MR_Percent,
        AddAttackSpeed,
        AddAttackRange,
        ADDCriticalDamage,
        AD_Damage,
        AP_Damage,
        Fire_Damage,
        Water_Damage,
        Electro_Damage,
        Ground_Damage,
        Light_Damage,
        Dark_Damage,
        AD_Reduction,
        AP_Reduction,
        Fire_Reduction,
        Water_Reduction,
        Electro_Reduction,
        Ground_Reduction,
        Light_Reduction,
        Dark_Reduction,
        None
    }
    // 유닛 상태
    public enum UnitState
    {
        Idle,
        Move,
        Attack,
        Dead
    }
    public enum UIColor
    {
        Red, Green, Blue, Yellow, White, Gray, Black
    }
    // 공격 타입
    public enum AttackType
    {
        None,
        Fire,
        Water,
        Electro,
        Ground,
        Light,
        Dark
    }
    // 
    public enum EquipmentType
    {
        Sword,
        Bow,
        CrossBow,
        Staf,
        Wand,
        Mace,
        Cloth,
        Leather,
        Plate,
        Belt,
        Gloves,
        Ring,
        Pendant
    }
    public enum JobClass
    {
        Melee = 0,
        Dexterity,
        Magic,
        Support,

        Blademaster = 101,
        Viking,
        Guardian,
        Breaker,
        Knight,

        Hunter = 201,
        Sniper,
        Ranger,
        Engineer,
        Amazon,

        Archmage = 301,
        Arcanist,
        Celestial,
        Witch,
        Summoner,

        Priest = 401,
        Elementalist,
        Cleric,
        ShadowPriest,
        Paladin
    }
    // 전투 통계
    public enum BattleAnalysis
    {
        Damage,
        Defence,
        Support,
        Obstruct
    }
    // 대상 지정 타입
    public enum AggroType
    {
        Balanced,           // 밸런스 형
        Berserker,          // 광전사 형
        Healer_Hunter,      // 힐러 헌터 형
        Tank_Breaker,       // 탱커 브레이커
        Finisher,           // 마무리 형
        Disrupt_Focus,      // 방해 집중 형
        Revenge_Seeker,     // 복수심 형
        Close_Quarters,     // 근접 집착 형
        Stun_Killer,        // 상태 이상 처단 형
        Tactician,          // 전략가 형
        Adaptive,           // 강약 조절 형
        Area_Sentinel,      // 광역 경계 형
        PvP_Guardian,       // 아군 우선 형
        Survivor,           // 극단 생존 형
        Chaotic_Random      // 혼돈 형
    }
    public enum TriggerType
    {
        Permanent, // 상시 발동
        SkillEnemy, // 적에게 스킬 발동
        Attacked, // 공격 받았을 시 발동
        HPRatio, // HP 비율에 따라 발동
        AttackEnemy, // 적에게 공격시 발동
        SkillMyTeam // 아군에게 스킬 발동
    }

    public enum Region_Attribute
    {
        Resource,
    }

    // 아이템 속성값
    public enum Item_Attribute
    {
        Weapon = 0,
        Armor,
        Acessori,
        ConsumablePlayer,
        ConsumableUser,
        MaterialMonster,
        MaterialQuest,
        MaterialEngravement,
        EquipStat,
        End
    }
    public enum Game_Attribute
    {
        CC,
    }
    public enum Skill_Attribute
    {
        Active,
        Passive,
        Buff
    }
    public enum Unit_Attribute
    {
        Class,
        Monster
    }
    // 퀘스트 속성값
    public enum Quest_Attribute
    {
        Type = 0,
        Dialog,
        Reward,
        End
    }


    public enum Quest_Type
    {
        Hunting,
        Rare_Hunting,
        //Farming,
        //Rare_Farming,
        Dungeon,
        //Boss,
        //Event,
        None
    }
    // 퀘스트 난이도
    public enum Quest_Difficulty
    {
        None = -1,
        Easy = 1,
        Normal,
        Hard,     
    }

    public enum EnemySpawnType
    {
        /// <summary>
        /// 십자가 모양 (가운데 + 상하좌우)
        /// [ ][X][ ]
        /// [X][X][X]
        /// [ ][X][ ]
        /// </summary>
        Cross,

        /// <summary>
        /// 엑스 모양 (대각선)
        /// [X][ ][X]
        /// [ ][X][ ]
        /// [X][ ][X]
        /// </summary>
        XShape,

        /// <summary>
        /// 네모 모양 (전체 3x3)
        /// [X][X][X]
        /// [X][X][X]
        /// [X][X][X]
        /// </summary>
        Square,

        /// <summary>
        /// 모서리 4개만
        /// [X][ ][X]
        /// [ ][ ][ ]
        /// [X][ ][X]
        /// </summary>
        Corners,

        /// <summary>
        /// 가로 줄 (중앙 가로)
        /// [ ][ ][ ]
        /// [X][X][X]
        /// [ ][ ][ ]
        /// </summary>
        HorizontalLine,

        /// <summary>
        /// 세로 줄 (중앙 세로)
        /// [ ][X][ ]
        /// [ ][X][ ]
        /// [ ][X][ ]
        /// </summary>
        VerticalLine,
    }
    public enum EquipRarity
    {
        Normal = 25,
        Rare = 50,
        Epic = 100,
        Unique = 150,
        Legendary = 200
    }
    public enum MonsterType
    {
        Golem = 0,
        Wolf = 5,
        Slime = 10,
        Goblin = 15,
        Skeleton = 20,
    }

    public enum RegionContentType
    {
        Shop,
        Enhance,
        Craft,
        SlimeRace,
    }

    /// <summary>
    /// 필드 이벤트 타입
    /// </summary>
    public enum FieldEventType
    {
        None,
        Battle,         // 전투 이벤트
        Treasure,       // 보물 이벤트
        Trap,          // 함정 이벤트
        Shop,          // 상점 이벤트
        Rest,          // 휴식 이벤트
        Boss,          // 보스 이벤트
        End
    }
    
    /// <summary>
    /// 이벤트 등급
    /// </summary>
    public enum EventGrade
    {
        Common = 1,    // 일반
        Uncommon = 2,  // 희귀
        Rare = 3,      // 레어
        Epic = 4,      // 에픽
        Legendary = 5  // 전설
    }
    public static EquipRarity GetRandomEquipRarity()
    {
        // Enum의 값들을 배열로 가져옴
        EquipRarity[] rarityValues = (EquipRarity[])System.Enum.GetValues(typeof(EquipRarity));

        // UnityEngine.Random을 사용하여 랜덤 인덱스를 생성
        int randomIndex = UnityEngine.Random.Range(0, rarityValues.Length);

        // 랜덤 인덱스를 사용해 EquipRarity 값 반환
        return rarityValues[randomIndex];
    }
    public enum SlotTextType
    {
        None,
        Title,
        Description
    }

    public enum PassiveType
    {
        Permanent,
        SkillEmeny,
        KillEnemy,
        Attacked,
        HpRatio,
        AttackEemy,
        Turn,
        SkillMyTeam,
    };
    public enum TriggerCondition
    {
        None,
        Random,
        HPRatio,
        SkillType,
        isOn
    }
}
