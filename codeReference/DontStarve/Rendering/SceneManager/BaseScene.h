#pragma once

struct MapData;

// BaseScene: 모든 Scene의 부모가 되는 추상 클래스
class BaseScene
{
public:
	BaseScene() = default;
	virtual ~BaseScene() = default;

	// 기본 필수 함수들 - 모든 Scene에서 구현해야 함
	virtual void Init(const MapData* mapData) = 0;
	virtual void Update(float deltaTime) = 0;
	virtual void LateUpdate() = 0;
	virtual void Render() = 0;
	virtual void Release() = 0;  // 소멸자에서 호출 — 이 씬이 사용한 매니저/포인터 정리

	// 현재 SceneType 반환 (UI/씬 전환 시 필요, 이 Enum은 다른 곳에서 정의)
	virtual SceneType GetSceneType() const = 0;
};
