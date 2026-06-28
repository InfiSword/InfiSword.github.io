using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Skill_WormJump : Skill_BossMain
{
    private Animator animator;
    private List<SpriteRenderer> wormBodySpriteRender;
    private Queue<Vector3> trail = new Queue<Vector3>();
    
    private float bossAtk = 100f;
    private Vector2[] shootDir = new Vector2[]
    {
        new Vector2(-1, 1).normalized,
        new Vector2(1, 1).normalized,
        new Vector2(-1, -1).normalized,
        new Vector2(1, -1).normalized
    };

    public void Setup(Animator animator, List<SpriteRenderer> bodies, float atk)
    {
        this.animator = animator;
        this.wormBodySpriteRender = bodies;
        this.bossAtk = atk;
    }

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(WormJump());
    }

    private IEnumerator WormJump()
    {
        animator.Play("Transparent");

        Vector2 targetPos = Managers.GridMgr.GridIndexToWorld(Random.Range(0, Managers.GridMgr.GridLayout.Columns), Random.Range(0, Managers.GridMgr.GridLayout.Rows));
        Managers.soundManager.PlaySfx(Define.SFX.SFX_WormBoss_JumpAtk_01, 1, false);

        // 점프 시작 위치에 땅굴 생성
        Managers.GridMgr.CreateWormHoleAtPosition(owner.transform.position);

        trail.Clear();
        owner.GetComponent<SpriteRenderer>().sprite = null;

        Vector2 _startPos = owner.transform.position;
        float _jumpHeight = 8f;
        int _fixedGap = 4;
        float _maxSpeed = 3f;
        float _minDuration = 2f;
        Vector2 _direction = (Vector2)targetPos - _startPos;
        float _distance = _direction.magnitude;
        _direction.Normalize();
        float _duration = Mathf.Max(_distance / _maxSpeed, _minDuration);
        float _speed = _distance / _duration;
        float _traveled = 0f;

        for (int i = 0; i < wormBodySpriteRender.Count; i++)
        {
            if (targetPos.y > _startPos.y)
                wormBodySpriteRender[i].transform.SetAsLastSibling();
            else
                wormBodySpriteRender[i].transform.SetAsFirstSibling();
        }

        wormBodySpriteRender[0].gameObject.SetActive(true);
        ShootGroundStone();
        yield return null;

        while (_traveled < _distance)
        {
            float delta = Time.fixedDeltaTime;
            float moveStep = _speed * delta;
            _traveled += moveStep;
            if (_traveled > _distance)
                _traveled = _distance;

            Vector3 _flatPos = _startPos + _direction * _traveled;
            float t = _traveled / _distance;
            float _height = _jumpHeight * t * (1 - t);
            Vector3 _headPos = _flatPos + Vector3.up * _height;

            if (trail.Count > _fixedGap * wormBodySpriteRender.Count)
                trail.Dequeue();
            trail.Enqueue(_headPos);

            wormBodySpriteRender[0].transform.position = _headPos;
            owner.transform.position = _headPos;

            UpdateWormBodyPositions(_fixedGap);

            for (int i = 1; i < wormBodySpriteRender.Count; i++)
            {
                if (!wormBodySpriteRender[i].gameObject.activeSelf)
                {
                    if (trail.Count > _fixedGap * i + 1)
                    {
                        wormBodySpriteRender[i].gameObject.SetActive(true);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }

        owner.transform.position = wormBodySpriteRender[0].transform.position;
        wormBodySpriteRender[0].gameObject.SetActive(false);

        // 점프 착지 위치에 땅굴 생성
        Managers.GridMgr.CreateWormHoleAtPosition(targetPos);

        while (trail.Count > 0)
        {
            trail.Dequeue();
            UpdateWormBodyPositions(_fixedGap, true);

            for (int i = 1; i < wormBodySpriteRender.Count; i++)
            {
                if (wormBodySpriteRender[i].gameObject.activeSelf)
                {
                    int _threshold = _fixedGap * (wormBodySpriteRender.Count - i);
                    if (trail.Count <= _threshold)
                    {
                        wormBodySpriteRender[i].gameObject.SetActive(false);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        FinishPattern();
    }

    private void UpdateWormBodyPositions(int _fixedGap, bool IsHeadArrive = false)
    {
        if (trail.Count == 0) return;

        Vector3[] _trailArray = trail.ToArray();
        for (int i = 1; i < wormBodySpriteRender.Count; i++)
        {
            int _pieceIndex = _fixedGap * (wormBodySpriteRender.Count - i);
            _pieceIndex = Mathf.Clamp(_pieceIndex, 0, _trailArray.Length - 1);
            if (IsHeadArrive) _pieceIndex = Mathf.Min(_pieceIndex++, _trailArray.Length - 1);
            wormBodySpriteRender[i].transform.position = _trailArray[_pieceIndex];
        }
    }

    private void ShootGroundStone()
    {
        for (int i = 0; i < shootDir.Length; i++)
        {
            Vector2 dir = shootDir[i].normalized;
            GameObject _bullet = Managers.Pool.GetBullet(Define.BulletType.Bullet_WormGround);
            _bullet.transform.position = owner.transform.position;
            _bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            _bullet.GetOrAddComponent<Bullet_WormGround>().Init(bossAtk, 0, dir, 5f);
        }
    }
}
