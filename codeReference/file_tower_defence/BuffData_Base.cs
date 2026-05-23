using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;

public class BuffData
{
    // 버프 데이터: 수치/틱 간격과 실행 코루틴 및 소유자 보관
    private float _amount;                // 버프 수치(예: 공격력 증가 비율, 쿨타임 감소 등)
    private float _waitTickTime;          // 버프 틱 간격
    private float _tickTimer;             // 누적 틱 시간

    public float Amount { get { return _amount; } }
    public float WaitTickTime { get { return _waitTickTime; } }

    public BuffData(float _amount, float _waitTickTime)
    {
        this._amount = Util.RoundToTwoDecimalPlaces(_amount);
        this._waitTickTime = Util.RoundToTwoDecimalPlaces(_waitTickTime);
        this._tickTimer = 0f; // 타이머 초기화
    }

    public void SetData(float _amount, float _waitTickTime)
    {
        this._amount = Util.RoundToTwoDecimalPlaces(_amount);
        this._waitTickTime = Util.RoundToTwoDecimalPlaces(_waitTickTime);
        this._tickTimer = 0f; 
    }

    /// <summary>
    /// 틱 시간을 누적하고 틱이 발동될 시간인지 확인
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    /// <returns>틱이 발동되면 true</returns>
    public bool Tick(float deltaTime)
    {
        // _waitTickTime이 0 이하면 틱 버프가 아님
        if (_waitTickTime <= float.Epsilon) 
            return false;

        _tickTimer += deltaTime;
        if (_tickTimer >= _waitTickTime)
        {
            _tickTimer -= _waitTickTime;
            return true;
        }
        
        return false;
    }
}

public abstract class MyBuff
{
    protected float addedSumValue;       // 버프 누적 합계 값(예: 10, 15 => 25)
    protected Dictionary<int, BuffData> activeBuffs = new Dictionary<int, BuffData>(); // 적용 중인 버프 사전(instanceID 기반)
    public Dictionary<int, BuffData> ActiveBuffs => activeBuffs;
    protected Transform buffParticle;
    protected Define.ParticleType buffParticleType;
    protected Transform buffTarget;

    /// <summary>
    /// 버프 초기화
    /// </summary>
    public abstract void Init(Transform _target);

    /// <summary>
    /// 지속 버프 로직
    /// </summary>
    public virtual bool ApplyBuff(BuffStat _buffStat)
    {
        if (activeBuffs.ContainsKey(_buffStat.instanceID))
        {
            SetParticleOn(buffParticleType);

            if (Util.Approximately(activeBuffs[_buffStat.instanceID].Amount, _buffStat.amount, 0.5f) ||
                Util.Approximately(activeBuffs[_buffStat.instanceID].WaitTickTime, _buffStat.waitTickTime))
            {
                return false; // 동일한 값이면 업데이트하지 않음
            }
            else
            {
                // 이미 적용된 버프가 있다면 기존 버프를 제거하고 새로운 버프 적용
                addedSumValue -= activeBuffs[_buffStat.instanceID].Amount;
                activeBuffs[_buffStat.instanceID].SetData(_buffStat.amount, _buffStat.waitTickTime);
                addedSumValue += _buffStat.amount;

                return true;
            }
        }

        // 새로운 버프 데이터 추가
        activeBuffs[_buffStat.instanceID] = new BuffData(_buffStat.amount, _buffStat.waitTickTime);
        addedSumValue += _buffStat.amount;
        
        if (_buffStat.waitTickTime <= float.Epsilon)
            SetParticleOn(buffParticleType);

        return true;
    }

    /// <summary>
    /// 틱 버프 로직을 처리 (BuffManager가 Update마다 호출)
    /// </summary>
    public virtual void Tick(float deltaTime)
    {
        // 틱 버프가 아닌 경우 자식 클래스에서 override 헤서 return 처리
        List<int> keys = new List<int>(activeBuffs.Keys);

        foreach (int key in keys)
        {
            if (!activeBuffs.TryGetValue(key, out BuffData data))
                continue;

            // BuffData의 틱 타이머를 업데이트하고, 틱이 발동될 시간인지 확인
            if (data.Tick(deltaTime))
            {
                // 파티클
                SetParticleOn(buffParticleType, 1f);            
                // 동작
                OnTick(data);
            }
        }
    }
    
    protected abstract void OnTick(BuffData buffData);

    /// <summary>
    /// 버프 제거
    /// </summary>
    /// <returns>제거되면 true</returns>
    public virtual bool RemoveBuff(BuffStat _buffStat)
    {
        if (activeBuffs.ContainsKey(_buffStat.instanceID))
        {
            // 해당 키를 제거하여 해당 버프 제거
            addedSumValue -= activeBuffs[_buffStat.instanceID].Amount;
            activeBuffs.Remove(_buffStat.instanceID);
            if (activeBuffs.Count == 0)
            {
                addedSumValue = 0; // 모든 버프가 제거되면 합계 초기화
                SetParticleOff(); // 버프 파티클 비활성화
            }
            return true;
        }

        return false;
    }

    public virtual void RemoveAllBuffs()
    {
        activeBuffs.Clear();
        addedSumValue = 0; // 모든 버프 제거 후 누적값 초기화

        if (buffParticle != null)
            SetParticleOff(); // 파티클 비활성화
    }

    /// <summary>
    /// 버프 비율 반환
    /// </summary>
    /// <returns>버프 비율</returns>
    public abstract float GetBuffValue();

    // 버프 파티클 생성 및 활성화
    protected void NewParticle(Define.ParticleType _ParticleBuff)
    {
        if (buffParticle == null)
            buffParticle = Managers.Pool.GetObjParticle(_ParticleBuff).transform; // 파티클 프리팹 로드
    }

    // 파티클 켜기
    protected void SetParticleOn(Define.ParticleType _ParticleBuff, float _duration = 0f)
    {
        NewParticle(_ParticleBuff);
        if (buffTarget == null)
            return;         
        SetParticlePos();
        var go = buffParticle != null ? buffParticle.gameObject : null;
        if (go != null)
            go.SetActive(true);
        RefreshParticles();

        // 일정 시간 후 파티클 종료 할 경우
        if (_duration > 0)
        {
            ParticleAutoDisable autoDisable = buffParticle.GetComponent<ParticleAutoDisable>();
            if (autoDisable == null)
                autoDisable = buffParticle.gameObject.AddComponent<ParticleAutoDisable>();
            
            autoDisable.StartTimer(_duration);
        }
    }

    // 파티클 끄기
    protected void SetParticleOff()
    {
        if (buffParticle == null)
        {
            buffParticle = null;
            return;
        }
        var go = buffParticle.gameObject;
        if (go != null)
            go.SetActive(false);
    }

    // 파티클 위치 설정
    public void SetParticlePos()
    {
        if (buffParticle == null || buffTarget == null)
            return;

        // 위치 설정
        Vector3 _pos = buffTarget.position;
        _pos.y += 0.1f;
        buffParticle.position = _pos;
    }

    // 파티클 새로고침
    protected void RefreshParticles()
    {
        if (buffParticle == null)
            return;
        UIParticle particle = buffParticle.GetComponent<UIParticle>();
        if (particle == null)
            return;
        particle.RefreshParticles();
        particle.Play();
    }
}
