using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// 게임 전역에서 사용하는 결정적 난수(Seed) 관리자.
/// - 같은 Seed와 같은 데이터 흐름이면 항상 동일한 결과를 생성
/// - 서브 도메인별로 독립된 RNG 스트림을 분리하여, 호출 순서 변화에 의한 교란을 최소화
/// </summary>
public static class GameSeed
{
    public enum Domain : uint
    {
        Map = 1,
        Region = 2,
        Field = 3,
        Quest = 4,
        Dialog = 5,
        Reward = 6,
                   // 직업 조합 및 스킬 셔플 세분화
        Class_Melee = 100,
        Class_Dexterity = 101,
        Class_Magic = 102,
        Class_Support = 103,
        Skill_Melee = 200,
        Skill_Dexterity = 201,
        Skill_Magic = 202,
        Skill_Support = 203,
    }

    private static readonly Dictionary<Domain, System.Random> domainRng = new Dictionary<Domain, System.Random>();
    private static readonly Dictionary<Domain, ulong> domainUsageCounts = new Dictionary<Domain, ulong>();
    public static long BaseSeed { get; private set; }
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// 외부에서 설정한 시드로 초기화. 동일 시드로 재초기화 시 스트림 리셋.
    /// </summary>
    public static void Init(long seed)
    {
        BaseSeed = seed;
        domainRng.Clear();
        domainUsageCounts.Clear();
        foreach (Domain d in Enum.GetValues(typeof(Domain)))
        {
            domainRng[d] = new System.Random(DeriveSubSeed(seed, (uint)d));
            domainUsageCounts[d] = 0UL;
        }
        IsInitialized = true;
    }

    /// <summary>
    /// int 범위 난수
    /// </summary>
    public static int NextInt(Domain domain, int minInclusive, int maxExclusive)
    {
        domainUsageCounts[domain] = domainUsageCounts.GetValueOrDefault(domain, 0UL) + 1UL;
        return domainRng[domain].Next(minInclusive, maxExclusive);
    }

    /// <summary>
    /// 0.0 이상 1.0 미만 float
    /// </summary>
    public static float NextFloat01(Domain domain)
    {
        domainUsageCounts[domain] = domainUsageCounts.GetValueOrDefault(domain, 0UL) + 1UL;
        return (float)domainRng[domain].NextDouble();
    }

    /// <summary>
    /// Enum에서 랜덤 선택 (기본값/End 같은 센티넬은 제외하도록 필터 지원)
    /// </summary>
    public static T RandomEnum<T>(Domain domain, Predicate<T> allow = null) where T : struct, Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        if (allow != null)
        {
            var filtered = new List<T>();
            foreach (T v in values)
            {
                if (allow(v)) filtered.Add(v);
            }
            values = filtered.ToArray();
        }

        if (values.Length == 0)
            throw new InvalidOperationException($"No available enum values for {typeof(T).Name}");

        int idx = NextInt(domain, 0, values.Length);
        return values[idx];
    }

    /// <summary>
    /// 리스트를 도메인 RNG로 셔플 (Fisher-Yates)
    /// </summary>
    public static void ShuffleInPlace<T>(Domain domain, IList<T> list)
    {
        Random rng = domainRng[domain];
        for (int i = list.Count - 1; i > 0; i--)
        {
            domainUsageCounts[domain] = domainUsageCounts.GetValueOrDefault(domain, 0UL) + 1UL;
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// 배열 셔플 도우미
    /// </summary>
    public static void ShuffleArrayInPlace(Domain domain, int[] array)
    {
        Random rng = domainRng[domain];
        for (int i = array.Length - 1; i > 0; i--)
        {
            domainUsageCounts[domain] = domainUsageCounts.GetValueOrDefault(domain, 0UL) + 1UL;
            int j = rng.Next(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    /// <summary>
    /// 도메인별 소비된 난수 호출 횟수 스냅샷을 반환
    /// </summary>
    public static Dictionary<Domain, ulong> GetUsageSnapshot()
    {
        Dictionary<Domain, ulong> copy = new Dictionary<Domain, ulong>();
        foreach (var kv in domainUsageCounts)
            copy[kv.Key] = kv.Value;
        return copy;
    }

    /// <summary>
    /// 사용량 스냅샷으로 RNG 상태를 복원 (재초기화 후 사용량만큼 버림 호출)
    /// </summary>
    public static void RestoreRNG(long baseSeed, Dictionary<Domain, ulong> usageCounts)
    {
        foreach (var kv in usageCounts)
        {
            Domain d = kv.Key;
            ulong count = kv.Value;     // 토큰 사용량
            System.Random rng = domainRng[d] = new System.Random(DeriveSubSeed(baseSeed, (uint)d));
            for (ulong i = 0; i < count; i++)
            {
                rng.Next();     // 사용한 횟수만큼 숫자를 버림 호출하여 RNG 상태를 맞춤
            }
            domainRng[d] = rng;
            domainUsageCounts[d] = count;   // 사용량 상태 업데이트
        }
    }

    /// <summary>
    /// 서브 시드 파생 (SplitMix64 유사 혼합, 32-bit 출력)
    /// </summary>
    private static int DeriveSubSeed(long baseSeed, uint salt)
    {
        unchecked
        {
            ulong z = (ulong)baseSeed + 0x9E3779B97F4A7C15UL + ((ulong)salt * 0x85EBCA6BUL);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            z = z ^ (z >> 31);
            return (int)(z & 0x7FFFFFFF); // 양의 정수로 고정
        }
    }
}


