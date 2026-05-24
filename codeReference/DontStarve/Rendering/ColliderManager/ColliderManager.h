#pragma once

class GameObject;
class Collider;

class ColliderManager : public CSingleTon<ColliderManager>
{
	friend class CSingleTon<ColliderManager>;
public:
	ColliderManager();
	~ColliderManager();

	void Init();
	void LateInit();
	void Update(float deltaTime);
	void LateUpdate();
	void Release();

	// 특정 콜라이더와 충돌 중인 객체들을 쿼리 (사용자 요청에 따른 능동형 충돌 검사)
	void QueryCollidingObjects(Collider* pSrc, std::vector<GameObject*>& outOwners);

	// 콜라이더 쌍 전용 검사: Box/ Circle/ 혼합 조합을 단일 진입점으로 처리
	bool Intersects(Collider* a, Collider* b);

private:
	std::vector<GameObject*> m_queryBuffer; // 매 프레임 재사용 버퍼(할당/해제 비용 절감)
};
