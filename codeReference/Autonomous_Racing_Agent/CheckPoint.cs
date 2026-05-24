using UnityEngine;

/// <summary>
/// 체크포인트의 식별 인덱스를 저장하는 데이터 클래스입니다.
/// 충돌 로직은 CarProgress에서 중앙 제어하므로, 이 클래스는 정보 제공 역할만 수행합니다.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int index;

    public int Index => index;

    public void Init(int index)
    {
        this.index = index;
    }
}
