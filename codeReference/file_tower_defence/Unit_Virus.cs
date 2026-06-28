using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sequence = DG.Tweening.Sequence;
using NUnit.Framework;

/// <summary>
/// 속도 변경 효과(수정자) 관리 클래스
/// </summary>
public class SpeedModifierHandle
{
    /// <summary>
    /// 속도 변경 비율. e.g., +0.5 (가속), -0.3 (둔화)
    /// </summary>
    public readonly float PercentChange; 
    public SpeedModifierHandle(float percentChange) { PercentChange = percentChange; }
}

/// <summary>
/// 바이러스 유닛 메인 로직
/// </summary>
public class Unit_Virus : Unit_VirusBase, IHealth
{   
    // --- 유닛 기본 스탯 ---
    protected float moveSpeed; // 이동 속도
    protected float maxhp;     // 최대 체력
    protected float hp;        // 현재 체력
    protected float atk;       // 공격력
    protected float CoolTime = 0.5f; // 공격 쿨타임
    public float atkRange = 1.0f; // 공격 사거리(그리드 단위) — 보스 미니언 근접공격용
    protected bool isirochi = false; // 희귀 개체 여부
    protected bool isElite = false;  // 엘리트 여부 

    /// <summary>
    /// 현재 적용 중인 모든 속도 버프 목록(가속/둔화)
    /// </summary>
    private List<SpeedModifierHandle> m_speedModifiers = new List<SpeedModifierHandle>();
    private bool isRewind = false;  // 역재생

    // --- DOTween 시퀀스 핸들러 ---
    private Sequence idleSequence; // 대기
    private Sequence moveSequence; // 이동
    private Sequence atkSequence;  // 공격

    // --- 상태 관리 ---
    private VirusState _state = VirusState.Idle; // 유닛 현재 상태
    public override bool canAttacked => _state != VirusState.Dead; // 사망 아닐 시 피격 가능
    
    // 바이러스 감염 테스트용 변수
    // private static bool isFirstVirusSpawned = false;

    /// <summary>
    /// 유닛 현재 상태 (변경 시 시퀀스 제어)
    /// </summary>
    public VirusState CurState
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                PauseElse(_state); // 상태 변경 시 시퀀스 재생/일시정지
            }
        }
    }

    private GameObject gob; // 현재 공격 대상
    private Color originalColor; // 원래 색상 저장용

    private void Awake()
    {
        originalColor = this.GetComponent<SpriteRenderer>().color; // 원래 색상 저장
    }

    /// <summary>
    /// 유닛 생성 시 초기화
    /// </summary>
    public override void Init(int pathNum, bool isElite = false)
    {
        gob = null; // 공격 대상 초기화

        // 쉐이더 값 초기화
        this.GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 0f);
        this.GetComponent<SpriteRenderer>().material.SetFloat("_ShakeUvSpeed", 3f * PreferenceData.EffectIntensity);

        // 데이터 매니저에서 스탯 로드
        VirusStat virusStat = Managers.Data.VirusDates[myvirusType];
        
        // 난이도(주차)에 따른 스탯 보정
        float difficultyBonus = (1 + (Managers.nowPlayerData.curWeekCount * 0.25f));

        // 기획 변경
        // 바이러스의 체력은 현재 체력 = 기본 체력 * (1 + (웨이브 수 * 0.05))
        maxhp = virusStat.Max_HP * difficultyBonus * (1 + (Managers.nowPlayerData.curRoundData.RoundNum * 0.05f));
        maxhp = isElite ? maxhp * 4 : maxhp;
        hp = maxhp;
        atk = isElite ? virusStat.Atk * difficultyBonus * 1.2f : virusStat.Atk * difficultyBonus;
        CoolTime = virusStat.CoolTime;
        moveSpeed = isElite ? virusStat.Speed * 0.5f : virusStat.Speed;
        transform.localScale = isElite ? Vector3.one * 1.8f : Vector3.one;
        this.isElite = isElite;

        if (isElite)
        {
            // 엘리트 바이러스는 빨간색으로 표시
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f, originalColor.a);
            Debug.Log("엘리트 바이러스 등장! 체력과 공격력이 증가하고 이동 속도가 감소합니다.");
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = originalColor;
        }

        // 이동 경로 설정
        GetVirusPath(pathNum);
        isRewind = false;

        // 시퀀스 초기화
        IdleSequence();
        idleSequence.Pause(); 
        
        this.gameObject.GetOrAddComponent<VirusInfectionAbility>();
    }

    /// <summary>
    /// 웨이포인트에서 이동 경로 가져오기
    /// </summary>
    private void GetVirusPath(int pathNum)
    {
        FileGridManager gridMgr = Managers.GridMgr;
        
        List<Vector2Int> waypoints = Managers.nowPlayerData.curRoundData.curWaypointsArray[pathNum];
        Vector2 [] positions = new Vector2[waypoints.Count];
        int i = 0;
        foreach (var point in waypoints)
        {
            positions[i++] = gridMgr.gridArray[point.x, point.y].transform.position;
        }

        moveSequence?.Kill(); // 기존 이동 시퀀스 제거

        this.transform.position = positions[0]; // 시작 위치로 이동
        MovementSequence(positions); // 새 이동 시퀀스 생성
    }

    /// <summary>
    /// 시퀀스 안전하게 재생
    /// </summary>
    private void PlaySequence(Sequence sequence)
    {
        if (sequence != null && sequence.IsActive() && !sequence.IsPlaying())
        {
            sequence.Play();
        }
    }

    /// <summary>
    /// 시퀀스 안전하게 일시정지
    /// </summary>
    private void PauseSequence(Sequence sequence)
    {
        if (sequence != null && sequence.IsActive() && sequence.IsPlaying())
        {
            sequence.Pause();
        }
    }

    // --- (속도 제어 로직) ---

    /// <summary>
    /// 속도 수정자 'duration'초 뒤 제거 코루틴
    /// </summary>
    private IEnumerator TimedSpeedChangeCoroutine(SpeedModifierHandle mod, float duration)
    {
        m_speedModifiers.Add(mod);
        UpdateMoveSequenceTimeScale(); // 속도 즉시 갱신

        yield return new WaitForSeconds(duration);

        m_speedModifiers.Remove(mod); // 수정자 제거
        UpdateMoveSequenceTimeScale(); // 속도 다시 갱신
    }

    /// <summary>
    /// (가장 강한 둔화만 적용) timeScale 갱신
    /// </summary>
    private void UpdateMoveSequenceTimeScale()
    {
        if (moveSequence == null || !moveSequence.IsActive())
            return;

        float finalPercentChange = 0f; // 기본값 (효과 없음)

        if (m_speedModifiers.Count > 0)
        {
            // 리스트에서 가장 "낮은" 값(가장 강한 둔화) 탐색
            finalPercentChange = m_speedModifiers.Min(m => m.PercentChange);
        }

        // 최종 timeScale 계산 (1.0f가 기본)
        float finalTimeScale = 1f + finalPercentChange;

        // 최소 속도 0.1로 보정
        moveSequence.timeScale = Mathf.Max(0.1f, finalTimeScale);
    }

    /// <summary>
    /// [외부 호출용] 둔화(양수) 또는 가속(음수) 2초간 적용
    /// </summary>
    /// <param name="velocityRatio">속도 비율 (0.3 = 30% 둔화, -0.5 = 50% 가속)</param>
    public override void SetMoveSpeed(float velocityRatio = -1)
    {
        if (!isActiveAndEnabled)
            return;

        if (velocityRatio <= 0 && velocityRatio != -1) // 음수 값은 '가속'
        {
            // e.g., -0.5 입력 -> +0.5로 변환
            SpeedModifierHandle mod = new SpeedModifierHandle(-velocityRatio);
            StartCoroutine(TimedSpeedChangeCoroutine(mod, 2.0f)); // 2초 고정
        }
        else if (velocityRatio > 0) // 양수 값은 '둔화'
        {
            // e.g., 0.3 입력 -> -0.3으로 변환
            SpeedModifierHandle mod = new SpeedModifierHandle(-velocityRatio);
            StartCoroutine(TimedSpeedChangeCoroutine(mod, 2.0f)); // 2초 고정
        }
    }
    public override void SetReverseMoveSequence(float duration)
    {
        if (!isActiveAndEnabled)
            return;

        if (!isRewind)
            StartCoroutine(ReverseMoveCoroutine(duration));
    }
    
    IEnumerator ReverseMoveCoroutine(float duration)
    {
        isRewind = true;
        moveSequence.PlayBackwards();
        
        yield return new WaitForSeconds(duration);
        
        isRewind = false;
        moveSequence.PlayForward();
    }

    // --- (상태 머신 및 시퀀스 관리) ---

    /// <summary>
    /// 상태 변경 시 시퀀스 제어 (상태 머신)
    /// </summary>
    private void PauseElse(VirusState newStatus)
    {
        switch (newStatus)
        {
            case VirusState.Idle:
                PlaySequence(idleSequence);
                PauseSequence(moveSequence);
                PauseSequence(atkSequence);
                break;

            case VirusState.Move:
                PauseSequence(idleSequence);
                PlaySequence(moveSequence);
                PauseSequence(atkSequence);
                break;

            case VirusState.Attack:
                PauseSequence(idleSequence);
                PauseSequence(moveSequence);
                PlaySequence(atkSequence);
                break;

            case VirusState.Dead:
                PauseSequence(idleSequence);
                PauseSequence(moveSequence);
                PauseSequence(atkSequence);
                break;
        }
    }

    /// <summary>
    /// 대기 시퀀스 (완료 시 이동 시작)
    /// </summary>
    private void IdleSequence()
    {
        this._state = VirusState.Idle;
        idleSequence = DOTween.Sequence().SetLink(gameObject);
        idleSequence.Play();
        idleSequence.OnComplete(() => moveSequence.Play());
    }

    /// <summary>
    /// 이동 시퀀스 설정 (웨이포인트 따라 이동)
    /// </summary>
    /// <param name="targetPositions">웨이포인트 배열</param>
    private void MovementSequence(Vector2[] targetPositions)
    {
        transform.position = targetPositions[0];

        // 기존 시퀀스 확실히 파괴
        if (moveSequence != null)
        {
            moveSequence.Kill();
            moveSequence = null;
        }

        // Vector2 배열을 DOPath가 요구하는 Vector3 배열로 변환
        Vector3[] path3D = new Vector3[targetPositions.Length];
        for (int i = 0; i < targetPositions.Length; i++)
        {
            path3D[i] = targetPositions[i];
        }

        // 전체 경로 총 거리 계산
        float distanceSum = 0;
        for (int i = 0; i < targetPositions.Length - 1; i++)
        {
            distanceSum += Vector2.Distance(targetPositions[i], targetPositions[i + 1]);
        }
        
        float totalMoveTime = distanceSum / moveSpeed; // 총 이동 시간

        moveSequence = DOTween.Sequence().SetLink(gameObject);

        // ★ DOPath를 사용하면 for문 없이 1개의 트윈으로 전체 웨이포인트를 따라갑니다.
        // PathType.Linear: 점과 점 사이를 직선으로 이동
        // PathMode.TopDown2D: 2D 게임에 맞는 경로 설정
        moveSequence.Append(transform.DOPath(path3D, totalMoveTime, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear));

        moveSequence.Play();
    }

    /// <summary>
    /// 공격 시퀀스 설정
    /// </summary>
    private void AtkMotionSequence(GameObject targetObject)
    {
        if (CurState != VirusState.Attack)
        {
            CurState = VirusState.Attack;
        }

        atkSequence?.Kill(); // 기존 공격 시퀀스 중지

        atkSequence = DOTween.Sequence().SetLink(gameObject);

        atkSequence.AppendCallback(() => IsEnd()); // 스테이지 종료 체크
        atkSequence.Append(transform.DOMove((Vector2)targetObject.transform.position, 0.2f).SetEase(Ease.OutCubic)); // 1. 타겟 접근
        atkSequence.AppendCallback(() => targetObject.GetComponent<File_Base>().TakeDmg(Mathf.Abs(atk))); // 2. 데미지 적용
        atkSequence.Append(transform.DOMove(transform.position, 0.1f).SetEase(Ease.Linear)); // 3. 살짝 복귀
        atkSequence.AppendInterval(CoolTime); // 4. 공격 쿨타임
        atkSequence.OnComplete(() => {
            if (targetObject != null && targetObject.activeSelf && CurState == VirusState.Attack) 
            {
                AtkMotionSequence(targetObject); // 타겟이 있으면 다시 공격 시퀀스 시작
            }
        });        
        atkSequence.Play();
    }

    // --- (트리거 및 피격/사망 처리) ---

    /// <summary>
    /// 공격 범위(트리거) 내 대상 확인
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 공격 불가능 대상 무시
        if (collision == null
            || !collision.CompareTag("Files")
            || collision.GetComponent<Unit_MyCom>()
            || gob == collision.gameObject)
            return;

        if (collision.GetComponent<File_Base>().FileState == FileStatus.normal
            && myvirusType != Define.VirusType.Virus_Trojan)
        {
            gob = collision.gameObject; // 대상 설정
            CurState = VirusState.Attack; // 공격 상태 전환
            AtkMotionSequence(collision.gameObject); // 공격 시작
        }
    }

    /// <summary>
    /// 공격 대상이 범위 이탈
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Files") && gob == collision.gameObject)
        {
            gob = null; // 대상 초기화
            CurState = VirusState.Move; // 이동 상태 복귀
        }
    }

    /// <summary>
    /// 10% 확률로 돈 10원 획득
    /// </summary>
    private void GetRandomMoney()
    {
        if (Random.Range(0, 100) < 10) // 10% 확률
        {
            int userMoney = PlayerPrefs.GetInt(Define.SaveData.UserMoney.ToString(), 0) + 10 + UpgradeManager.Instance.MoneyGainvalue;
            PlayerPrefs.SetInt(Define.SaveData.UserMoney.ToString(), userMoney);
            PlayerPrefs.Save();

            Debug.Log("돈 저장 현재 돈: " + userMoney);
        }
    }

    /// <summary>
    /// 데미지 처리 (IHealth)
    /// </summary>
    public override bool TakeDmg(float atk)
    {
        return ApplyDamage(atk, null, 1f);
    }

    /// <summary>
    /// 파일이 입힌 데미지 처리. 
    /// </summary>
    public bool TakeDmgFromFile(float atk, File_Base damageSource, float eliteBonusMultiplier = 1f)
    {
        return ApplyDamage(atk, damageSource, eliteBonusMultiplier);
    }

    /// <summary>
    /// 데미지 처리 전부 이 함수에서 수행 (엘리트 보정, 피격 연출, 뱀파이어 회복, 사망 처리).
    /// damageSource가 null이면 뱀파이어 회복 없음.
    /// </summary>
    private bool ApplyDamage(float atk, File_Base damageSource, float eliteBonusMultiplier)
    {
        float finalAtk = atk;
        if (isElite && eliteBonusMultiplier > 1f)
            finalAtk *= eliteBonusMultiplier;

        if (hp - finalAtk > maxhp) // 오버힐 방지
            hp = maxhp;
        else
        {
            // --- 피격 처리 ---
            UnitState.instance.FloatingText(((int)finalAtk).ToString(), this.transform.position, true);

            Material _material = this.transform.GetComponent<SpriteRenderer>().material;
            UnitState.instance.Animate(_material, "HITEFFECT_ON", "_HitEffectBlend", 0f, 0.7f * PreferenceData.EffectIntensity, 0.3f);

            hp -= finalAtk;

            if (hp > 0f)
                HitSound();

            Managers.Data.RecordData[(int)Define.Record.TotalDamage] += finalAtk;

            // 뱀파이어 도기: 데미지 소스(파일)가 있을 때만, 입힌 피해의 N% * 도기 개수만큼 회복
            if (damageSource != null)
            {
                int vampireCount = DogiProvider.GetDogiCount(Define.DogiAugment.Vampire);
                if (vampireCount > 0)
                {
                    float healAmount = finalAtk * (DogiValues.VampireHealRatePerDogi * vampireCount);
                    damageSource.Heal(healAmount);
                }
            }
        }

        // --- 사망 처리 ---
        if (hp <= 0)
        {
            CurState = VirusState.Dead;
            KillSequence();
            DieSound();

            if (gameObject.activeSelf)
                DestroyVirus();
        }
        return true;
    }

    /// <summary>
    /// 유닛 파괴 코루틴 시작
    /// </summary>
    public override void DestroyVirus(bool isTimeEndDead = false)
    {
        if (!isActiveAndEnabled)
            return;

        StartCoroutine(DestroyVirusCoru(isTimeEndDead));
    }


    /// <summary>
    /// 사망 연출 및 자원 정리 코루틴
    /// </summary>
    private IEnumerator DestroyVirusCoru(bool isTimeEndDead = false)
    {
        if (isTimeEndDead) // 시간 초과 즉시 사망
        {
            transform.gameObject.SetActive(false);
            yield break;
        }

        yield return new WaitForSeconds(0.5f); // 사망 연출 대기

        Managers.Data.RecordData[(int)Define.Record.VirusKill] += 1; // 킬 수 기록

        KillSequence(); // 시퀀스 정리
        GetRandomMoney(); // 돈 획득 시도
        Managers.Pool.virusSpawner.DecreaseAliveVirusCount(); // 살아있는 바이러스 수 감소

        if (Managers.nowPlayerData.curRoundData.IsStageStart)
            Managers.nowPlayerData.curRoundData.AddKillCount(); // 라운드 킬 카운트 증가

        // 라운드 종료 조건 확인
        if (Managers.nowPlayerData.curRoundData.KillCount >= Managers.nowPlayerData.curRoundData.TotalSpawnCount)
        {
            // Managers.UI.GetSceneUI<StageMain>().RoundCom.RoundNext();
        }

        if (isirochi) // 이로치 처치 시 특수 이벤트
        {
            Managers.Pool.Root.GetOrAddComponent<Skill_VirusBase>().ExecuteRandomSkill();
        }

        transform.gameObject.SetActive(false); // 오브젝트 비활성화 (풀링 반환)
    }

    // --- (사운드 및 유틸리티) ---

    private void HitSound()
    {
        // 다중 공격 시 여러 개 겹치므로 개별 볼륨을 낮춰 합산이 과해지지 않게 한다.
        Managers.soundManager.PlaySfx(Define.SFX.SFX_Enemy_Taking_Damage_01, Random.Range(0.9f, 1.1f), false, 0.3f);
    }

    private void DieSound()
    {
        if (CurState != VirusState.Dead)
            return;
        // 다중 처치 시 여러 개 겹치므로 피격음과 같은 수준으로 볼륨을 낮춘다.
        Managers.soundManager.PlaySfx(Define.SFX.SFX_Death_Enemy_01, Random.Range(0.9f, 1.1f), false, 0.3f);
    }

    /// <summary>
    /// 대상 파일을 향해 근접 공격(접근→피해→복귀→쿨타임 후 반복). 보스 미니언(Survivor) 등에서 사용.
    /// 재탐색(CheckForAttackAtCurrentPosition)은 Move 상태 복귀로 단순화.
    /// </summary>
    public virtual void PerformMeleeAttack(GameObject targetObject)
    {
        if (targetObject == null || !targetObject.activeSelf)
        {
            CurState = VirusState.Move;
            return;
        }

        Vector2 startPos = transform.position;
        float maxDist = Managers.GridMgr.GridDistance.x * atkRange;
        if (Vector2.Distance(transform.position, targetObject.transform.position) > maxDist + 0.1f)
        {
            CurState = VirusState.Move;
            return;
        }

        if (CurState != VirusState.Attack)
            CurState = VirusState.Attack;

        atkSequence?.Kill();
        atkSequence = DOTween.Sequence().SetLink(gameObject);
        atkSequence.AppendCallback(() => { if (Managers.newStep == Define.StageState.EndState) KillSequence(); });
        atkSequence.Append(transform.DOMove((Vector2)targetObject.transform.position, 0.2f).SetEase(Ease.OutCubic));
        atkSequence.AppendCallback(() =>
        {
            if (targetObject != null && targetObject.activeSelf &&
                Vector2.Distance(startPos, targetObject.transform.position) <= maxDist + 0.1f)
            {
                targetObject.GetComponent<File_Base>()?.TakeDmg(Mathf.Abs(atk));
            }
        });
        atkSequence.Append(transform.DOMove(startPos, 0.1f).SetEase(Ease.Linear));
        atkSequence.AppendInterval(CoolTime);
        atkSequence.OnComplete(() =>
        {
            if (targetObject != null && targetObject.activeSelf && CurState == VirusState.Attack)
                PerformMeleeAttack(targetObject);
            else
                CurState = VirusState.Move;
        });
        atkSequence.Play();
    }

    /// <summary>
    /// 모든 DOTween 시퀀스 안전하게 Kill
    /// </summary>
    private void KillSequence()
    {
        if (idleSequence != null) { idleSequence.Kill(); idleSequence = null; }
        if (moveSequence != null) { moveSequence.Kill(); moveSequence = null; }
        if (atkSequence != null) { atkSequence.Kill(); atkSequence = null; }
    }

/// <summary>
    /// 오브젝트 비활성화 (풀링 반환) 시 완벽한 초기화
    /// </summary>
    private void OnDisable()
    {
        // 1. 코루틴 모두 정지
        StopAllCoroutines();

        // 2. 현재 오브젝트에 연결된 모든 DOTween 강제 종료 및 파괴
        transform.DOKill(); 

        KillSequence();

        // 4. 상태 초기화
        m_speedModifiers.Clear(); 
        CurState = VirusState.Idle;
        gob = null;
    }
    /// <summary>
    /// 시퀀스 중간에 스테이지 종료 확인
    /// </summary>
    private void IsEnd()
    {
        if (Managers.newStep == Define.StageState.EndState)
            KillSequence();
    }

    /// <summary>
    /// (미구현) 체력 회복
    /// </summary>
    public override void Heal(float amount)
    {
    }
}
