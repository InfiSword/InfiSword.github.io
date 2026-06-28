using UnityEngine;

/// <summary>
/// 들어오는 데미지를 가공(감소, 무효화, 증폭 등)
/// </summary>
public interface IDamageFilter
{
    /// <summary>
    /// 최종 데미지를 계산하여 반환
    /// </summary>
    /// <param name="originalDamage">들어온 원래 데미지</param>
    /// <returns>필터를 거친 후의 데미지</returns>
    float ModifyDamage(float originalDamage);
}