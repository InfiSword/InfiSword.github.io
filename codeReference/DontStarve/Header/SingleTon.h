#pragma once

template<typename T>
class CSingleTon
{
protected:
	explicit CSingleTon() = default;
	virtual ~CSingleTon() = default;
public:
	static T* GetInstance()
	{
		if (!m_pInstance)
		{
			m_pInstance = new T;
		}
		return m_pInstance;
	}
	static BOOL DestroyInstance()
	{
		if (m_pInstance)
		{
			delete m_pInstance;
			m_pInstance = nullptr;
		}
		return TRUE;
	}

private:
	static T* m_pInstance;
};

template<typename T>
T* CSingleTon<T>::m_pInstance = nullptr;
