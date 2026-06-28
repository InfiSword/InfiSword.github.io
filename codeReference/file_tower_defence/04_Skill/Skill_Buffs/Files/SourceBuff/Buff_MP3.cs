using UnityEngine;

public class Buff_MP3 : Buff_Base
{
    // 힐 소리 전역 스로틀: 여러 파일이 동시에/매 틱마다 힐해도 소리가 겹쳐 시끄럽지 않도록
    // 모든 Buff_MP3가 공유하는 쿨다운으로 일정 간격에 한 번만 재생한다.
    private static float _lastHealSfxTime = -999f;
    private const float HealSfxCooldown = 0.4f;      // 실시간 기준 최소 재생 간격
    private const float HealSfxVolume = 0.35f;       // 힐 소리 볼륨(피격·사망음과 동일하게 절대값)

    protected override void OnTick()
    {
        base.OnTick();

        if (owner != null)
        {
            owner.GetComponent<IHealth>()?.Heal(Amount);

            UnitState.instance.FloatingText($"+{Amount}", transform.position, false);

            // 전역 쿨다운(게임 속도와 무관한 실시간) 동안은 힐 소리를 다시 내지 않는다. 볼륨도 낮춰 재생.
            if (Time.unscaledTime - _lastHealSfxTime >= HealSfxCooldown)
            {
                _lastHealSfxTime = Time.unscaledTime;
                Managers.soundManager.PlaySfx(Define.SFX.SFX_Healing_01, volume: HealSfxVolume);
            }
        }
    }
}
