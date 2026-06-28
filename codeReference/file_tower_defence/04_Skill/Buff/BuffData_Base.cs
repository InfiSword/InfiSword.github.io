using UnityEngine;
using System.Linq;
using Coffee.UIExtensions;

/// <summary>
/// 타겟 유닛에게 부착되어 실제로 버프 효과를 발휘하는 컴포넌트의 베이스 클래스입니다.
/// </summary>
public abstract class Buff_Base : MonoBehaviour
{
    public Define.BuffType BuffType { get; private set; }
    public int SourceID { get; private set; } = -1;
    public float Amount { get; private set; }
    public float Duration { get; private set; }
    public float WaitTickTime { get; private set; }
    public bool IsRemoving { get; private set; } = false;

    private float _durationTimer;
    private float _tickTimer;
    protected File_Base owner;

    private GameObject myParticle;

    public void Init(BuffStat stat)
    {
        owner = GetComponent<File_Base>();
        this.BuffType = stat.buffType;
        this.SourceID = stat.instanceID;
        this.Amount = stat.amount;
        this.Duration = stat.duration;
        this.WaitTickTime = stat.waitTickTime;

        this._durationTimer = stat.duration;
        this._tickTimer = 0f;

        OnAdded();
    }

    public void Refresh(BuffStat stat)
    {
        this.Amount = stat.amount;
        this.Duration = stat.duration;
        this.WaitTickTime = stat.waitTickTime;
        this._durationTimer = stat.duration;
    }

    protected virtual void Update()
    {
        if (IsRemoving) return;

        float deltaTime = Time.deltaTime;

        // 틱 처리
        if (WaitTickTime > 0)
        {
            _tickTimer += deltaTime;
            if (_tickTimer >= WaitTickTime)
            {
                _tickTimer -= WaitTickTime;
                OnTick();
            }
        }

        // 지속시간 처리
        if (Duration > 0)
        {
            _durationTimer -= deltaTime;
            if (_durationTimer <= 0)
            {
                Remove();
            }
        }
    }

    protected virtual void OnAdded()
    {
        ShowParticle();
    }

    protected virtual void OnTick() { }

    protected virtual void OnRemoved()
    {
        HideParticle();
    }

    public void Remove()
    {
        if (IsRemoving) return;
        IsRemoving = true;

        OnRemoved();

        // BuffController에 삭제 요청
        BuffStat myStat = new BuffStat(BuffType, SourceID, Amount, Duration, WaitTickTime);
        GetComponent<BuffController>()?.RemoveBuff(myStat);
    }

    public void SetRemoving()
    {
        if (IsRemoving) return;
        IsRemoving = true;
        OnRemoved();
    }

    private void OnDestroy()
    {
        if (!IsRemoving)
        {
            OnRemoved();
        }
    }

    #region Particle Management

    protected void ShowParticle()
    {
        // 이미 동일한 타입의 버프 파티클이 켜져 있다면 굳이 또 만들 필요 없음
        Buff_Base[] sameBuffs = GetComponents<Buff_Base>().Where(b => b.BuffType == this.BuffType && !b.IsRemoving).ToArray();
        
        // 이 버프가 해당 타입의 첫 번째 버프일 때만 파티클을 활성화
        if (sameBuffs.Length == 1 && sameBuffs[0] == this)
        {
            Define.ParticleType particleType = GetParticleType(BuffType);
            myParticle = Managers.Pool.GetObjParticle(particleType);

            if (myParticle != null)
            {
                Vector3 pos = transform.position;
                pos.y += 0.1f;
                myParticle.transform.position = pos;
                myParticle.SetActive(true);

                UIParticle uiParticle = myParticle.GetComponent<UIParticle>();
                if (uiParticle != null)
                {
                    uiParticle.RefreshParticles();
                    uiParticle.Play();
                }
            }
        }
    }

    protected void HideParticle()
    {
        // 동일 타입의 살아남은 버프 개수 확인
        int activeCount = GetComponents<Buff_Base>().Count(b => b.BuffType == this.BuffType && !b.IsRemoving && b != this);

        // 더 이상 이 타입의 버프가 없고, 파티클을 소유하고 있었다면 비활성화
        if (activeCount == 0 && myParticle != null)
        {
            myParticle.SetActive(false);
            myParticle = null;
        }
        else if (myParticle != null && activeCount > 0)
        {
             // 만약 내가 파티클을 들고 죽는데 아직 다른 동료 버프가 남아있다면 파티클 권한을 넘김
             var nextBuff = GetComponents<Buff_Base>().FirstOrDefault(b => b.BuffType == this.BuffType && !b.IsRemoving && b != this);
             if(nextBuff != null)
             {
                 nextBuff.myParticle = this.myParticle;
                 this.myParticle = null;
             }
        }
    }

    private Define.ParticleType GetParticleType(Define.BuffType buffType)
    {
        switch (buffType)
        {
            case Define.BuffType.ATK: return Define.ParticleType.Particle_Obj_ATK;
            case Define.BuffType.CoolTime: return Define.ParticleType.Particle_Obj_CoolTime;
            case Define.BuffType.HP: return Define.ParticleType.Particle_Obj_HP;
            default: return Define.ParticleType.Particle_Obj_ATK;
        }
    }

    #endregion
}
