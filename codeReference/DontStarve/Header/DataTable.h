#pragma once
#include "Enum.h"
#include "Struct.h"

// ====================== 통합 데이터 테이블 =======================

struct DataTable
{
	static constexpr ItemInfo ItemTable[] = {
		{ GOID_ITEM_NORMAL_TREE_LOG,     L"통나무",           L"평범한 통나무입니다." },
		{ GOID_ITEM_NORMAL_TWIGS,        L"나뭇가지",         L"흔한 나뭇가지입니다." },
		{ GOID_ITEM_NORMAL_ROCK,         L"암석 조각",        L"작은 암석 조각입니다." },
		{ GOID_ITEM_CUT_NORMAL_GRASS,    L"풀",               L"제작에 사용되는 잘린 풀 묶음입니다." },
		{ GOID_ITEM_GOLD_ROCK,           L"금",               L"반짝이고 가치 있는 금입니다." },
		{ GOID_ITEM_ROPE,                L"밧줄",             L"제작에 유용하게 쓰입니다." },
		{ GOID_ITEM_CUT_NORMAL_STONE,    L"가공된 돌",        L"건축용 석재 블록입니다." },
		{ GOID_ITEM_MEAT,                L"고기",             L"신선한 고기입니다." },
		{ GOID_ITEM_BERRY,               L"베리",             L"달콤하고 영양가 있는 열매입니다." },
		{ GOID_ITEM_WOOD_2,              L"나무 판자",        L"제작을 위한 나무 판자입니다." },
		{ GOID_ITEM_SMALL_MEAT,          L"작은 고기",        L"작은 고기 조각입니다." },
		{ GOID_ITEM_MONSTER_MEAT,        L"괴물 고기",        L"괴물에게서 얻은 이상한 고기입니다." },
		{ GOID_ITEM_COOKED_MONSTER_MEAT, L"익힌 괴물 고기",   L"조리된 괴물 고기입니다." },
		{ GOID_ITEM_COOKED_SMALL_MEAT,   L"익힌 작은 고기",   L"조리된 작은 고기입니다." },
		{ GOID_ITEM_COOKED_MEAT,         L"익힌 고기",        L"맛있게 익은 고기입니다." }
	};
	static constexpr size_t ItemCount = sizeof(ItemTable) / sizeof(ItemInfo);

	static constexpr ToolInfo ToolTable[] = {
		{ GOID_TOOL_GOLDEN_PICKAXE, L"황금 곡괭이", L"황금 곡괭이입니다.", 35, 120.0f },
		{ GOID_TOOL_HAM_BAT,        L"햄 방망이",       L"고기로 만든 무기입니다.",       40, 100.0f },
		{ GOID_TOOL_PICKAXE,        L"곡괭이",     L"바위와 광석을 캡니다.",         15, 80.0f  },
		{ GOID_TOOL_SPEAR,          L"창",         L"전투를 위한 단순한 창입니다.",   30, 150.0f },
		{ GOID_TOOL_SWAP_SPEAR,     L"번개 창", L"번개의 힘이 깃든 창입니다.",     45, 160.0f },
		{ GOID_TOOL_TORCH,          L"횃불",       L"어둠 속에서 빛을 밝힙니다.",     10, 90.0f  },
		{ GOID_TOOL_RED_AXE,        L"빨간 도끼",   L"나무를 베는 데 사용합니다.",     35, 100.0f },
		{ GOID_TOOL_SWAP_AXE,       L"도끼",   L"기본 도끼입니다.",  200, 100.0f },
		{ GOID_TOOL_HALBERD,        L"할버드",     L"긴 사거리를 가진 무거운 폴암입니다.", 50, 140.0f },
		{ GOID_TOOL_HAMMER,         L"망치",       L"구조물을 해체할 때 사용합니다.", 15, 85.0f  }
	};
	static constexpr size_t ToolCount = sizeof(ToolTable) / sizeof(ToolInfo);

	// 타일 타입 테이블
	static constexpr TileTypeEntry TileTypeTable[] = {
		{ TILE_NONE,   L"TILE_NONE" },
		{ TILE_DIRT,   L"TILE_DIRT" },
		{ TILE_GRASS,  L"TILE_GRASS" },
		{ TILE_FOREST, L"TILE_FOREST" },
		{ TILE_COUNT,  L"TILE_COUNT" }
	};

	// 타일 ID 테이블
	static constexpr TileIDEntry TileIDTable[] = {
		{ TILEID_NONE,      L"TILEID_NONE" },
		{ TILEID_DIRT_00,   L"TILEID_DIRT_00" },
		{ TILEID_DIRT_01,   L"TILEID_DIRT_01" },
		{ TILEID_DIRT_02,   L"TILEID_DIRT_02" },
		{ TILEID_DIRT_03,   L"TILEID_DIRT_03" },
		{ TILEID_GRASS_00,  L"TILEID_GRASS_00" },
		{ TILEID_GRASS_01,  L"TILEID_GRASS_01" },
		{ TILEID_GRASS_02,  L"TILEID_GRASS_02" },
		{ TILEID_GRASS_03,  L"TILEID_GRASS_03" },
		{ TILEID_FOREST_00, L"TILEID_FOREST_00" },
		{ TILEID_FOREST_01, L"TILEID_FOREST_01" },
		{ TILEID_FOREST_02, L"TILEID_FOREST_02" },
		{ TILEID_FOREST_03, L"TILEID_FOREST_03" }
	};

	// 오브젝트 ID 테이블
	static constexpr GameObjectIDEntry GameObjectIDTable[] =
	{
		{ GOID_NONE,                     L"GOID_NONE" },
		{ GOID_NORMAL_GRASS,             L"GOID_NORMAL_GRASS" },
		{ GOID_NORMAL_TREE_SHORT,        L"GOID_NORMAL_TREE_SHORT" },
		{ GOID_NORMAL_TREE_NORMAL,       L"GOID_NORMAL_TREE_NORMAL" },
		{ GOID_NORMAL_TREE_TALL,         L"GOID_NORMAL_TREE_TALL" },
		{ GOID_NORMAL_ROCK,              L"GOID_NORMAL_ROCK" },
		{ GOID_GOLD_ROCK,                L"GOID_GOLD_ROCK" },
		{ GOID_NORMAL_SAPLING,           L"GOID_NORMAL_SAPLING" },
		{ GOID_BERRY_TREE,               L"GOID_BERRY_TREE" },
		{ GOID_MONSTER_PIG,              L"GOID_MONSTER_PIG" },
		{ GOID_MONSTER_SPIDER,           L"GOID_MONSTER_SPIDER" },
		{ GOID_MONSTER_WARRIOR_SPIDER,   L"GOID_MONSTER_WARRIOR_SPIDER" },
		{ GOID_MONSTER_QUEEN_SPIDER,     L"GOID_MONSTER_QUEEN_SPIDER" },
		{ GOID_MONSTER_HOUNDDOG,         L"GOID_MONSTER_HOUNDDOG" },
		{ GOID_MONSTER_REDHOUNDDOG,      L"GOID_MONSTER_REDHOUNDDOG" },
		{ GOID_MONSTER_ICEHOUNDDOG,      L"GOID_MONSTER_ICEHOUNDDOG" },
		{ GOID_BUILDING_PIGHOUSE,        L"GOID_BUILDING_PIGHOUSE" },
		{ GOID_BUILDING_SPIDER_SACEGG,   L"GOID_BUILDING_SPIDER_SACEGG" },
		{ GOID_BUILDING_SPIDER_SMALLEGG, L"GOID_BUILDING_SPIDER_SMALLEGG" },
		{ GOID_BUILDING_SPIDER_NORMALEGG,L"GOID_BUILDING_SPIDER_NORMALEGG" },
		{ GOID_BUILDING_SPIDER_TALLEGG,  L"GOID_BUILDING_SPIDER_TALLEGG" },
		{ GOID_ITEM_CUT_NORMAL_GRASS,    L"GOID_ITEM_CUT_NORMAL_GRASS" },
		{ GOID_ITEM_NORMAL_TREE_LOG,     L"GOID_ITEM_NORMAL_TREE_LOG" },
		{ GOID_ITEM_NORMAL_TWIGS,        L"GOID_ITEM_NORMAL_TWIGS" },
		{ GOID_ITEM_NORMAL_ROCK,         L"GOID_ITEM_NORMAL_ROCK" },
		{ GOID_ITEM_GOLD_ROCK,           L"GOID_ITEM_GOLD_ROCK" },
		{ GOID_ITEM_ROPE,                L"GOID_ITEM_ROPE" },
		{ GOID_ITEM_CUT_NORMAL_STONE,    L"GOID_ITEM_CUT_NORMAL_STONE" },
		{ GOID_ITEM_MEAT,                L"GOID_ITEM_MEAT" },
		{ GOID_ITEM_BERRY,               L"GOID_ITEM_BERRY" },
		{ GOID_ITEM_WOOD_2,             L"GOID_ITEM_WOOD_2" },
		{ GOID_ITEM_SMALL_MEAT,         L"GOID_ITEM_SMALL_MEAT" },
		{ GOID_ITEM_MONSTER_MEAT,       L"GOID_ITEM_MONSTER_MEAT" },
		{ GOID_ITEM_COOKED_MONSTER_MEAT, L"GOID_ITEM_COOKED_MONSTER_MEAT" },
		{ GOID_ITEM_COOKED_SMALL_MEAT,  L"GOID_ITEM_COOKED_SMALL_MEAT" },
		{ GOID_ITEM_COOKED_MEAT,        L"GOID_ITEM_COOKED_MEAT" },
		{ GOID_PLAYER_WILSON,            L"GOID_PLAYER_WILSON" },
		{ GOID_PLAYER_WILLOW,            L"GOID_PLAYER_WILLOW" },
		{ GOID_PLAYER_WOLFGANG,          L"GOID_PLAYER_WOLFGANG" },
	};

	static constexpr SceneTypeByPath SceneTypeTable[] =
	{
		{ SCENE_GAME_FARMING_AREA, L"GameData/00_map.dsm" },
		{ SCENE_GAME_HOUND_FOREST, L"GameData/01_BossHound.dsm" },
		{ SCENE_GAME_SPIDER_QUEEN_HOUSE, L"GameData/02_BossSpiderQueen.dsm"}
	};

	// 타일 리소스 정적 테이블
	static constexpr TileDefEntry TileResourceTable[] = {
		{ TILE_DIRT, TILEID_DIRT_00, L"Resource\\Tiles\\Dirt", L"dirt_01.png" },
		{ TILE_DIRT, TILEID_DIRT_01, L"Resource\\Tiles\\Dirt", L"dirt_02.png" },
		{ TILE_DIRT, TILEID_DIRT_02, L"Resource\\Tiles\\Dirt", L"dirt_03.png" },
		{ TILE_DIRT, TILEID_DIRT_03, L"Resource\\Tiles\\Dirt", L"dirt_04.png" },
		{ TILE_GRASS, TILEID_GRASS_00, L"Resource\\Tiles\\Grass", L"grass_01.png" },
		{ TILE_GRASS, TILEID_GRASS_01, L"Resource\\Tiles\\Grass", L"grass_02.png" },
		{ TILE_GRASS, TILEID_GRASS_02, L"Resource\\Tiles\\Grass", L"grass_03.png" },
		{ TILE_GRASS, TILEID_GRASS_03, L"Resource\\Tiles\\Grass", L"grass_04.png" },
		{ TILE_FOREST, TILEID_FOREST_00, L"Resource\\Tiles\\Forest", L"forest_01.png" },
		{ TILE_FOREST, TILEID_FOREST_01, L"Resource\\Tiles\\Forest", L"forest_02.png" },
		{ TILE_FOREST, TILEID_FOREST_02, L"Resource\\Tiles\\Forest", L"forest_03.png" },
		{ TILE_FOREST, TILEID_FOREST_03, L"Resource\\Tiles\\Forest", L"forest_04.png" }
	};
	static constexpr size_t TileResourceCount = sizeof(TileResourceTable) / sizeof(TileDefEntry);

	// 오브젝트 리소스 정적 테이블
	static constexpr ObjectDefEntry ObjectResourceTable[] = {
		{ GOID_PLAYER_WILSON, L"Resource\\Objects\\Player\\Wilson", L"Wilson_Image.png" },
		{ GOID_PLAYER_WILLOW, L"Resource\\Objects\\Player\\Willow", L"" },
		{ GOID_PLAYER_WOLFGANG, L"Resource\\Objects\\Player\\Wolfgang", L"" },
		{ GOID_NORMAL_TREE_SHORT, L"Resource\\Objects\\Tree1\\Short", L"evergreen_evergreen_short_idle_short_01.png" },
		{ GOID_NORMAL_TREE_NORMAL, L"Resource\\Objects\\Tree1\\Normal", L"evergreen_evergreen_short_idle_normal_01.png" },
		{ GOID_NORMAL_TREE_TALL, L"Resource\\Objects\\Tree1\\Tall", L"evergreen_evergreen_short_idle_tall_01.png" },
		{ GOID_NORMAL_ROCK, L"Resource\\Objects\\Rock\\Rock_Normal", L"rock01-0.png" },
		{ GOID_GOLD_ROCK, L"Resource\\Objects\\Rock\\Rock_Gold", L"rock02-0.png" },
		{ GOID_NORMAL_GRASS, L"Resource\\Objects\\Grass", L"grass.png" },
		{ GOID_NORMAL_SAPLING, L"Resource\\Objects\\Twign", L"sapling.png" },
		{ GOID_BERRY_TREE, L"Resource\\Objects\\Bush", L"BerryBush.png" },
		{ GOID_ITEM_CUT_NORMAL_GRASS, L"Resource\\Objects\\ingredient", L"cutgrass01-0.png" },
		{ GOID_ITEM_NORMAL_ROCK, L"Resource\\Objects\\ingredient", L"rocks01-0.png" },
		{ GOID_ITEM_NORMAL_TWIGS, L"Resource\\Objects\\ingredient", L"twigs01-0.png" },
		{ GOID_ITEM_NORMAL_TREE_LOG, L"Resource\\Objects\\ingredient", L"Tree1_log.png" },
		{ GOID_ITEM_GOLD_ROCK, L"Resource\\Objects\\ingredient", L"Gold_Item.png" },
		{ GOID_ITEM_ROPE, L"Resource\\Objects\\ingredient", L"rope01-0.png" },
		{ GOID_ITEM_CUT_NORMAL_STONE, L"Resource\\Objects\\ingredient", L"cutstone01-0.png" },
		{ GOID_ITEM_MEAT, L"Resource\\Objects\\food", L"meat-0.png" },
		{ GOID_ITEM_BERRY, L"Resource\\Objects\\food", L"Berry.png" },
		{ GOID_ITEM_WOOD_2, L"Resource\\Objects\\ingredient", L"Wood_2.png" },
		{ GOID_ITEM_SMALL_MEAT, L"Resource\\Objects\\food", L"meat_small01-0.png" },
		{ GOID_ITEM_MONSTER_MEAT, L"Resource\\Objects\\food", L"Monster_Meat.png" },
		{ GOID_ITEM_COOKED_MONSTER_MEAT, L"Resource\\Objects\\food", L"Cooked_Monster_Meat.png" },
		{ GOID_ITEM_COOKED_SMALL_MEAT, L"Resource\\Objects\\food", L"meat_small01-1.png" },
		{ GOID_ITEM_COOKED_MEAT, L"Resource\\Objects\\food", L"meat_01-1.png" },
		{ GOID_TOOL_GOLDEN_PICKAXE, L"Resource\\Objects\\Tools", L"Golden_Scythe_02.png" },
		{ GOID_TOOL_HAM_BAT, L"Resource\\Objects\\Tools", L"hamBat_01.png" },
		{ GOID_TOOL_PICKAXE, L"Resource\\Objects\\Tools", L"pickaxe-0.png" },
		{ GOID_TOOL_RED_AXE, L"Resource\\Objects\\Tools", L"Red_Axe_02.png" },
		{ GOID_TOOL_SPEAR, L"Resource\\Objects\\Tools", L"spear_03.png" },
		{ GOID_TOOL_SWAP_AXE, L"Resource\\Objects\\Tools", L"swap_axe-0.png" },
		{ GOID_TOOL_SWAP_SPEAR, L"Resource\\Objects\\Tools", L"swap_spear_wathgrithr_lightning-5.png" },
		{ GOID_TOOL_TORCH, L"Resource\\Objects\\Tools", L"torch.png" },
		{ GOID_TOOL_HALBERD, L"Resource\\Objects\\Tools", L"halberd.png" },
		{ GOID_TOOL_HAMMER, L"Resource\\Objects\\Tools", L"hammer.png" },
		{ GOID_MONSTER_SPIDER, L"Resource\\Objects\\Monster\\Spider\\Normal_Spider", L"Spider_spider_idle_01.png" },
		{ GOID_MONSTER_WARRIOR_SPIDER, L"Resource\\Objects\\Monster\\Spider\\Warrior_Spider", L"Warrior_spider_idle_01.png" },
		{ GOID_MONSTER_PIG, L"Resource\\Objects\\Monster\\Pig", L"pig_Image.png" },
		{ GOID_MONSTER_HOUNDDOG, L"Resource\\Objects\\Monster\\Hound\\Normal_Hound", L"Hound_hound_Image.png" },
		{ GOID_MONSTER_QUEEN_SPIDER, L"Resource\\Objects\\Monster\\Spider\\Queen", L"Queen_spider_queen_Image.png" },
		{ GOID_MONSTER_REDHOUNDDOG, L"Resource\\Objects\\Monster\\Hound\\Red_Hound", L"RedHound_hound_Image.png" },
		{ GOID_MONSTER_ICEHOUNDDOG, L"Resource\\Objects\\Monster\\Hound\\Ice_Hound", L"IceHound_hound_Image.png" },
		{ GOID_BUILDING_SPIDER_SACEGG, L"Resource\\Objects\\Building\\Egg", L"Egg_spider_cocoon_small_Image.png" },
		{ GOID_BUILDING_SPIDER_SMALLEGG, L"Resource\\Objects\\Building\\Egg", L"Egg_spider_cocoon_small_Image.png" },
		{ GOID_BUILDING_SPIDER_NORMALEGG, L"Resource\\Objects\\Building\\Egg", L"Egg_spider_cocoon_medium_Image.png" },
		{ GOID_BUILDING_SPIDER_TALLEGG, L"Resource\\Objects\\Building\\Egg", L"Egg_spider_cocoon_large_Image.png" },
		{ GOID_BUILDING_PIGHOUSE, L"Resource\\Objects\\Building\\House", L"pig_house.png" }
	};
	static constexpr size_t ObjectResourceCount = sizeof(ObjectResourceTable) / sizeof(ObjectDefEntry);

	// 유틸리티 함수
	static inline const ItemInfo* GetItemInfo(GameObjectID id) {
		for (size_t i = 0; i < ItemCount; ++i) {
			if (ItemTable[i].id == id) return &ItemTable[i];
		}
		return nullptr;
	}

	static inline const ToolInfo* GetToolInfo(GameObjectID id) {
		for (size_t i = 0; i < ToolCount; ++i) {
			if (ToolTable[i].id == id) return &ToolTable[i];
		}
		return nullptr;
	}
};
