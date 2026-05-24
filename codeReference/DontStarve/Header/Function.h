#pragma once
#include <windows.h>
#include <gdiplus.h>
#include <vector>
#include <string>
#include "Enum.h"
#include "Struct.h"

using namespace Gdiplus;

// ====================== Enum 문자열 변환 유틸리티 함수 =======================

namespace EnumTables {
	// TileType -> 문자열
	inline const WCHAR* GetEnumName(TileType value) {
		for (const auto& entry : DataTable::TileTypeTable) {
			if (entry.value == value) return entry.name;
		}
		return L"UNKNOWN";
	}

	// 문자열 -> TileType
	inline TileType GetTileType(const WCHAR* name) {
		for (const auto& entry : DataTable::TileTypeTable) {
			if (std::wcscmp(entry.name, name) == 0) return entry.value;
		}
		return TILE_NONE;
	}

	// TileID -> 문자열
	inline const WCHAR* GetEnumName(TileID value) {
		for (const auto& entry : DataTable::TileIDTable) {
			if (entry.value == value) return entry.name;
		}
		return L"UNKNOWN";
	}

	// 문자열 -> TileID
	inline TileID GetTileID(const WCHAR* name) {
		for (const auto& entry : DataTable::TileIDTable) {
			if (std::wcscmp(entry.name, name) == 0) return entry.value;
		}
		return TILEID_NONE;
	}

	// GameObjectID -> 문자열
	inline const WCHAR* GetEnumName(GameObjectID value) {
		for (const auto& entry : DataTable::GameObjectIDTable) {
			if (entry.value == value) return entry.name;
		}
		return L"UNKNOWN";
	}

	// 문자열 -> GameObjectID
	inline GameObjectID GetGameObjectID(const WCHAR* name) {
		for (const auto& entry : DataTable::GameObjectIDTable) {
			if (std::wcscmp(entry.name, name) == 0) return entry.value;
		}
		return GOID_NONE;
	}
}

// ====================== 도구 스탯 유틸리티 함수 =======================

namespace ToolDataUtils
{
	// 도구 ID로 스탯 정보 조회 (DataTable 사용)
	inline const ToolInfo* GetToolStats(GameObjectID toolID) {
		return DataTable::GetToolInfo(toolID);
	}
}

// ====================== 아이템 디스플레이 이름 유틸리티 함수 =======================

namespace ItemDisplayUtils
{
	// 아이템 ID로 디스플레이 이름 조회 (DataTable 사용)
	inline const wchar_t* GetItemDisplayName(GameObjectID itemID) {
		if (auto* toolInfo = DataTable::GetToolInfo(itemID)) return toolInfo->name;
		if (auto* itemInfo = DataTable::GetItemInfo(itemID)) return itemInfo->name;
		return L"알 수 없는 아이템";
	}
}

// ====================== 리소스 경로 및 디스플레이 이름 유틸리티 =======================

namespace ResourceUtils
{
	// GameObjectID로 리소스 이미지 경로 가져오기 (ObjectResourceTable 기반)
	inline std::wstring GetResourceImagePath(GameObjectID itemID) {
		using namespace ResourcePathUtils;
		
		for (size_t i = 0; i < DataTable::ObjectResourceCount; ++i) {
			if (DataTable::ObjectResourceTable[i].id == itemID) {
				std::wstring path = DataTable::ObjectResourceTable[i].baseDir;
				path += L"/";
				path += DataTable::ObjectResourceTable[i].imageName;
				return path;
			}
		}
		return L"";
	}

	// GameObjectID로 디스플레이 이름 가져오기 (ItemDisplayTable 기반)
	inline std::wstring GetResourceDisplayName(GameObjectID itemID) {
		using namespace ItemDisplayUtils;
		return GetItemDisplayName(itemID);
	}
}

// 유틸리티 함수들
namespace Utils
{
	template<typename T>
	inline void SafeDelete(T& obj)
	{
		if (obj)
		{
			delete obj;
			obj = nullptr;
		}
	}

	// 거리 계산
	inline float CalculateDistance(float x1, float y1, float x2, float y2)
	{
		float dx = x2 - x1;
		float dy = y2 - y1;
		return sqrtf(dx * dx + dy * dy);
	}

	// 목표까지의 방향 계산
	inline Direction GetDirectionToTarget(float fromX, float fromY, float toX, float toY)
	{
		float dx = toX - fromX;
		float dy = toY - fromY;

		// 절댓값이 더 큰 방향을 우선적으로 선택
		if (abs(dx) > abs(dy))
		{
			return (dx > 0) ? DIR_RIGHT : DIR_LEFT;
		}
		else
		{
			return (dy > 0) ? DIR_DOWN : DIR_UP;
		}
	}

	inline float Random01()
	{
		return static_cast<float>(rand()) / static_cast<float>(RAND_MAX);
	}
}

namespace ResourcePathUtils
{
	// 맵 파일 파싱 함수
	// ResourceManager의 GetObjectResourceInfo 콜백을 통해 오브젝트 리소스 정보를 가져옴
	template<typename GetObjectResourceInfoFunc>
	inline bool ParseMapFileInto(const std::wstring& mapFileName, MapData& outMapData, GetObjectResourceInfoFunc getObjectResourceInfo)
	{
		outMapData.mapFilePath = mapFileName;

		// 맵 이름 추출
		size_t lastSlash = mapFileName.find_last_of(L"\\/");
		size_t lastDot = mapFileName.find_last_of(L".");
		if (lastSlash != std::wstring::npos) {
			outMapData.mapName = mapFileName.substr(lastSlash + 1, lastDot - lastSlash - 1);
		}
		else {
			outMapData.mapName = mapFileName.substr(0, lastDot);
		}

		// 파일 열기
		std::wifstream file(mapFileName);

		if (!file.is_open()) {
			return false;
		}

		// BOM 처리
		wchar_t bom[3] = { 0 };
		file.read(bom, 3);
		if (bom[0] != 0xFEFF && !(bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)) {
			file.seekg(0, std::ios::beg);
		}
		else if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF) {
			file.seekg(3, std::ios::beg);
		}

		std::wstring line;
		std::wstring pendingLine;  // for OBJECTS: next line read but not Collider (re-process next iteration)
		enum Section { NONE, METADATA, PLAYER, TILES, OBJECTS, WALKABLE } section = NONE;
		int currentTileRow = 0;
		int currentWalkRow = 0;

		while (true) {
			if (!pendingLine.empty()) {
				line = pendingLine;
				pendingLine.clear();
			}
			else if (!std::getline(file, line))
				break;
			if (line.empty() || line[0] == L'#') {
				if (line.find(L"# TILES") != std::wstring::npos || line == L"# TILES") {
					section = TILES;
					currentTileRow = 0;
				}
				else if (line.find(L"# OBJECTS") != std::wstring::npos || line == L"# OBJECTS") {
					section = OBJECTS;
				}
				else 				if (line.find(L"# WALKABLE_AREAS") != std::wstring::npos || line == L"# WALKABLE_AREAS") {
					section = WALKABLE;
					currentWalkRow = 0;
				}
				else if (line.find(L"# PLAYER_SPAWN") != std::wstring::npos || line == L"# PLAYER_SPAWN") {
					section = PLAYER;
				}
				continue;
			}

			// 메타데이터 파싱
			if (line.find(L"MAP_WIDTH=") != std::wstring::npos) {
				outMapData.mapWidth = std::stoi(line.substr(line.find(L"=") + 1));
			}
			else if (line.find(L"MAP_HEIGHT=") != std::wstring::npos) {
				outMapData.mapHeight = std::stoi(line.substr(line.find(L"=") + 1));
			}
			// 플레이어 스폰 파싱
			else if (section == PLAYER) {
				if (line.find(L"PLAYER_SPAWN_X=") != std::wstring::npos) {
					float x = std::stof(line.substr(line.find(L"=") + 1));
					if (x >= 0) {
						outMapData.playerSpawn.x = x;
					}
				}
				else if (line.find(L"PLAYER_SPAWN_Y=") != std::wstring::npos) {
					float y = std::stof(line.substr(line.find(L"=") + 1));
					if (y >= 0) {
						outMapData.playerSpawn.y = y;
					}
				}
			}
			// 타일 파싱
			else if (section == TILES && currentTileRow < outMapData.mapHeight) {
				std::wstringstream ss(line);
				std::wstring token;
				std::vector<std::wstring> tokens;
				while (std::getline(ss, token, L',')) {
					token.erase(0, token.find_first_not_of(L" \t"));
					token.erase(token.find_last_not_of(L" \t") + 1);
					tokens.push_back(token);
				}
				for (int i = 0; i < (int)tokens.size(); i += 2) {
					int tileX = i / 2;
					if (tileX < outMapData.mapWidth && currentTileRow < outMapData.mapHeight) {
						TileType tileType = EnumTables::GetTileType(tokens[i].c_str());
						TileID tileID = EnumTables::GetTileID(tokens[i + 1].c_str());
						
						// TileID로 경로 찾기
						std::wstring baseDir, imageName;
						for (size_t j = 0; j < DataTable::TileResourceCount; ++j) {
							if (DataTable::TileResourceTable[j].id == tileID) {
								baseDir = DataTable::TileResourceTable[j].baseDir;
								imageName = DataTable::TileResourceTable[j].imageName;
								break;
							}
						}
						
						outMapData.tiles[tileX][currentTileRow].type = tileType;
						outMapData.tiles[tileX][currentTileRow].id = tileID;
						outMapData.tiles[tileX][currentTileRow].baseDir = baseDir;
						outMapData.tiles[tileX][currentTileRow].imageName = imageName;
					}
				}
				currentTileRow++;
			}
			// 오브젝트 파싱 (형식: ID,x,y 3필드만. 리소스/피벗/콜라이더는 생성 시 ResourceManager에서 조회)
			else if (section == OBJECTS) {
				if (line.find(L"0,0,0,0,0") != std::wstring::npos) continue;
				
				std::wstringstream ss(line);
				std::wstring id, x, y;
				if (std::getline(ss, id, L',') &&
					std::getline(ss, x, L',') &&
					std::getline(ss, y, L',')) {
					
					id.erase(0, id.find_first_not_of(L" \t"));
					id.erase(id.find_last_not_of(L" \t") + 1);
					
					GameObjectID objID = EnumTables::GetGameObjectID(id.c_str());
					
					if (objID != GOID_NONE) {
						float objX = std::stof(x);
						float objY = std::stof(y);
						
						ResourcePathUtils::ObjectResourceDef objData;
						objData.id = objID;
						objData.x = objX;
						objData.y = objY;

						outMapData.gameObjects.push_back(objData);
					}
				}
			}
			// Walkable 영역 파싱
			else if (section == WALKABLE && currentWalkRow < outMapData.mapHeight) {
				std::wstringstream ss(line);
				std::wstring token;
				int currentCol = 0;
				while (std::getline(ss, token, L',') && currentCol < outMapData.mapWidth) {
					token.erase(0, token.find_first_not_of(L" \t"));
					token.erase(token.find_last_not_of(L" \t") + 1);
					outMapData.walkableAreas[currentCol][currentWalkRow] = (std::stoi(token) == 1);
					currentCol++;
				}
				currentWalkRow++;
			}
		}
		
		file.close();
		return true;
	}

	// 오브젝트 오버라이드 파일 파싱 (Client·Editor 공통)
	// 각 줄마다 apply(id, overrideDef) 호출. overrideDef에는 pivot·콜라이더 필드만 채워짐.
	template<typename ApplyOverrideFunc>
	inline bool ParseObjectResourceOverridesFile(const std::wstring& filePath, ApplyOverrideFunc apply)
	{
		std::wifstream ifs(filePath);
		if (!ifs.is_open()) return false;
		std::wstring line;
		while (std::getline(ifs, line)) {
			if (line.empty() || line[0] == L'#') continue;
			std::wstringstream iss(line);
			std::wstring idName;
			float pivotX = 0.5f, pivotY = 1.0f;
			int hasColliderInt = 0, colliderTypeInt = 0;
			int offsetX = 0, offsetY = 0, width = 0, height = 0;
			float centerX = 0, centerY = 0, radius = 0;
			if (!(iss >> idName >> pivotX >> pivotY >> hasColliderInt >> colliderTypeInt
				>> offsetX >> offsetY >> width >> height >> centerX >> centerY >> radius)) continue;
			GameObjectID id = EnumTables::GetGameObjectID(idName.c_str());
			if (id == GOID_NONE) continue;
			ResourcePathUtils::ObjectResourceDef overrideDef;
			overrideDef.id = id;
			overrideDef.pivotX = pivotX;
			overrideDef.pivotY = pivotY;
			overrideDef.hasCollider = (hasColliderInt != 0);
			overrideDef.colliderType = (colliderTypeInt >= 0 && colliderTypeInt < (int)COLLIDER_COUNT) ? (ColliderType)colliderTypeInt : COLLIDER_BOX;
			overrideDef.colliderOffsetX = offsetX;
			overrideDef.colliderOffsetY = offsetY;
			overrideDef.colliderWidth = width;
			overrideDef.colliderHeight = height;
			overrideDef.colliderCenterX = centerX;
			overrideDef.colliderCenterY = centerY;
			overrideDef.colliderRadius = radius;
			apply(id, overrideDef);
		}
		return true;
	}

	// baseDir와 imageName을 결합하여 전체 경로 생성
	inline std::wstring BuildResourcePath(const std::wstring& baseDir, const std::wstring& imageName) {
		if (baseDir.empty()) return imageName; // baseDir가 없으면 이름만 반환
		if (imageName.empty()) return L"";
		
		std::wstring fullPath = baseDir;
		if (fullPath.back() != L'\\' && fullPath.back() != L'/') {
			fullPath += L'\\';
		}
		fullPath += imageName;
		return fullPath;
	}
}
