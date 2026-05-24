#pragma once

// GameObject와 Component의 공통 기본 클래스
// 활성화 상태를 통합 관리
class Object
{
protected:
	bool m_enabled;		// 활성화 상태

public:
	Object();
	virtual ~Object();

	// 활성화 상태 제어
	void SetActive(bool enabled) { m_enabled = enabled; }
	bool IsEnabled() const { return m_enabled; }
};

