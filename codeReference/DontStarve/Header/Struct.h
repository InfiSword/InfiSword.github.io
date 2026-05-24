#pragma once
#include "Define.h"
#include "Enum.h"

// ====================== 데이터 테이블 엔트리 구조체 =======================

// 아이템 정보 (이름, 설명)
struct ItemInfo {
	GameObjectID id;
	const wchar_t* name;
	const wchar_t* desc;
};

// 도구 정보 (이름, 설명, 공격력, 사거리)
struct ToolInfo {
	GameObjectID id;
	const wchar_t* name;
	const wchar_t* desc;
	int damage;
	float attackRange;
};

// TileType 테이블 엔트리 구조체
struct TileTypeEntry {
	TileType value;
	const wchar_t* name;
};

// TileID 테이블 엔트리 구조체
struct TileIDEntry {
	TileID value;
	const wchar_t* name;
};

// GameObjectID 테이블 엔트리 구조체
struct GameObjectIDEntry {
	GameObjectID value;
	const wchar_t* name;
};

struct SceneTypeByPath {
	SceneType value;
	const wchar_t* path;
};

// 타일 리소스 정적 테이블 엔트리 (컴파일 타임 상수)
struct TileDefEntry {
	TileType type;
	TileID id;
	const wchar_t* baseDir;
	const wchar_t* imageName;
};

// 오브젝트 리소스 정적 테이블 엔트리
struct ObjectDefEntry {
	GameObjectID id;
	const wchar_t* baseDir;
	const wchar_t* imageName;
};

#include "DataTable.h"

// ====================== 리소스 경로 관련 구조체 =======================

namespace ResourcePathUtils
{	
	// 타일 리소스 정의 구조체 (런타임 사용, std::wstring 사용)
	struct TileResourceDef {
		TileType type;
		TileID id;
		std::wstring baseDir;
		std::wstring imageName;

		TileResourceDef()
			: type(TILE_NONE), id(TILEID_NONE)
		{}

		TileResourceDef(TileType _type, TileID _id, const std::wstring& _baseDir, const std::wstring& _imageName)
			: type(_type), id(_id), baseDir(_baseDir), imageName(_imageName)
		{}
	};

	// 오브젝트 리소스 정의 구조체
	struct ObjectResourceDef {
		GameObjectID id;
		std::wstring baseDir;
		std::wstring imageName;
		float pivotX;
		float pivotY;
		
		float x = 0, y = 0;
		
		bool hasCollider = false;
		ColliderType colliderType = COLLIDER_BOX;
		int colliderOffsetX = 0;
		int colliderOffsetY = 0;
		int colliderWidth = 0;
		int colliderHeight = 0;
		float colliderCenterX = 0.0f;
		float colliderCenterY = 0.0f;
		float colliderRadius = 0.0f;

		ObjectResourceDef()
			: id(GOID_NONE), pivotX(0.5f), pivotY(1.0f), x(0), y(0)
		{}

		ObjectResourceDef(GameObjectID id_val, float x_val, float y_val,
			const std::wstring& baseDir_val, const std::wstring& imageName_val, float pivotX_val, float pivotY_val,
			bool hasCollider_val = false, ColliderType colliderType_val = COLLIDER_BOX,
			int colliderOffsetX_val = 0, int colliderOffsetY_val = 0,
			int colliderWidth_val = 0, int colliderHeight_val = 0,
			float colliderCenterX_val = 0.0f, float colliderCenterY_val = 0.0f, float colliderRadius_val = 0.0f)
			: id(id_val), baseDir(baseDir_val), imageName(imageName_val),
			pivotX(pivotX_val), pivotY(pivotY_val), x(x_val), y(y_val),
			hasCollider(hasCollider_val), colliderType(colliderType_val),
			colliderOffsetX(colliderOffsetX_val), colliderOffsetY(colliderOffsetY_val),
			colliderWidth(colliderWidth_val), colliderHeight(colliderHeight_val),
			colliderCenterX(colliderCenterX_val), colliderCenterY(colliderCenterY_val), colliderRadius(colliderRadius_val)
		{}
	};
}

// ====================== 팔레트 아이템 구조체 =======================

struct PaletteItem 
{
	UINT resourceId;
	ItemCategory category;
	RECT displayRect;
	Gdiplus::Bitmap* hBitmap;
	Gdiplus::RectF iconSourceRect;
};

// ====================== 렌더 명령 구조체 =======================

struct DrawCommand {
	DrawCommandType type;
	RenderLayer layer;
	float zOrder;
	Gdiplus::RectF destRect;

	struct SpriteData {
		Gdiplus::Bitmap* pBitmap;
		Gdiplus::RectF sourceRect;
		Gdiplus::Unit srcUnit;
		Direction direction;
		Gdiplus::Color tintColor;
		bool hasTint;
		bool preFlipped;
	};

	struct TextData {
		const std::wstring* textPtr;
		Gdiplus::Font* pFont;
		Gdiplus::Brush* pBrush;
		Gdiplus::StringFormat* pStringFormat;
	};

	struct PrimitiveData {
		Gdiplus::Color color;
		float thickness;
		bool isFilled;
	};

	union {
		SpriteData sprite;
		TextData text;
		PrimitiveData primitive;
	};

	DrawCommand() : type(DRAW_COMMAND_ENTITY), layer(LAYER_NONE), zOrder(0.0f), sprite{}
	{
		destRect = Gdiplus::RectF(0, 0, 0, 0);
	}
};

// ====================== 플레이어 스폰 데이터 구조체 =======================

struct PlayerSpawnData {
	float x, y;

	PlayerSpawnData() : x(-1), y(-1) {}
	PlayerSpawnData(float _x, float _y) : x(_x), y(_y) {}
};

// ====================== 맵 데이터 구조체 =======================

struct MapData {
	std::wstring mapName;
	std::wstring mapFilePath;

	int mapWidth;
	int mapHeight;
	PlayerSpawnData playerSpawn;

	ResourcePathUtils::TileResourceDef tiles[MAP_WIDTH][MAP_HEIGHT];
	std::vector<ResourcePathUtils::ObjectResourceDef> gameObjects;
	bool walkableAreas[MAP_WIDTH][MAP_HEIGHT];

	MapData()
		: mapName(L""), mapFilePath(L""), mapWidth(MAP_WIDTH), mapHeight(MAP_HEIGHT)
	{
		for (int y = 0; y < MAP_HEIGHT; ++y)
			for (int x = 0; x < MAP_WIDTH; ++x) {
				tiles[x][y] = ResourcePathUtils::TileResourceDef();
				walkableAreas[x][y] = true;
			}
	}

	MapData(const std::wstring& name, const std::wstring& filePath)
		: mapName(name), mapFilePath(filePath), mapWidth(MAP_WIDTH), mapHeight(MAP_HEIGHT)
	{
		for (int y = 0; y < MAP_HEIGHT; ++y)
			for (int x = 0; x < MAP_WIDTH; ++x) {
				tiles[x][y] = ResourcePathUtils::TileResourceDef();
				walkableAreas[x][y] = true;
			}
	}
};

// ====================== 애니메이션 프레임 구조체 =======================

class Sprite; // 전방 선언

struct AnimationFrame {
	std::shared_ptr<Sprite> sprite;
	float duration;

	AnimationFrame()
		: sprite(nullptr), duration(0.1f)
	{}

	AnimationFrame(std::shared_ptr<Sprite> s, float d)
		: sprite(s), duration(d)
	{}
};
