using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager
{
    // 각 Attribute의 버전 정보를 담는 클래스
    [System.Serializable]
    public class AttributeVersionEntry
    {
        public string attributeName;
        public string version;
    }

    // 특정 CSVDataType에 속하는 Attribute들의 버전 목록을 담는 클래스
    [System.Serializable]
    public class CsvDataTypeVersionEntry
    {
        public Define.CSVDataType dataType;
        public List<AttributeVersionEntry> attributeVersionList = new List<AttributeVersionEntry>();
    }

    // 전체 버전 데이터들을 담는 클래스
    [System.Serializable]
    public class DataVersionContainer
    {
        public List<CsvDataTypeVersionEntry> dataEntryList = new List<CsvDataTypeVersionEntry>();

        public Dictionary<Define.CSVDataType, Dictionary<string, string>> ToDictionary()
        {
            var dict = new Dictionary<Define.CSVDataType, Dictionary<string, string>>();
            foreach (var entry in dataEntryList)
            {
                var innerDict = new Dictionary<string, string>();
                foreach (var attrVersion in entry.attributeVersionList)
                {
                    innerDict[attrVersion.attributeName] = attrVersion.version;
                }
                dict[entry.dataType] = innerDict;
            }
            return dict;
        }

        public static DataVersionContainer FromDictionary(Dictionary<Define.CSVDataType, Dictionary<string, string>> dict)
        {
            var container = new DataVersionContainer();
            foreach (var kvp in dict)
            {
                var containerEntry = new CsvDataTypeVersionEntry { dataType = kvp.Key };
                foreach (var innerKvp in kvp.Value)
                {
                    containerEntry.attributeVersionList.Add(new AttributeVersionEntry { attributeName = innerKvp.Key, version = innerKvp.Value });
                }
                container.dataEntryList.Add(containerEntry);
            }
            return container;
        }
    }

    // Seed/Token JSON 저장 구조 
    [Serializable]
    private class SeedState
    {
        public long baseSeed;
        public string seedToken;
        public string updatedAt;
    }

    private static string SeedStatePath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath + "/../seed_state_editor.json";
#else
            return Application.persistentDataPath + "/seed_state.json";
#endif
        }
    }

    // ===== Seed 기반 세이브/로드 규격 =====
    private const int BASE_SEED_WIDTH = 20;    // ulong 최대값 기준 20자리
    private const int COUNT_WIDTH = 3;         // 도메인 개수 3자리
    private const int DOMAIN_ID_WIDTH = 3;     // 도메인 ID 3자리
    private const int USAGE_WIDTH = 12;        // 사용량 12자리

    /// <summary>
    /// 현재 GameSeed 상태를 토큰 문자열로 빌드합니다. (음수 시드 지원)
    /// </summary>
    private static string BuildSeedToken(long baseSeed, Dictionary<GameSeed.Domain, ulong> usage)
    {
        // unchecked를 사용하여 음수 long 비트를 ulong으로 안전하게 변환
        ulong encodedSeed = unchecked((ulong)baseSeed);
        string token = Pad(encodedSeed, BASE_SEED_WIDTH);

        List<GameSeed.Domain> domains = new List<GameSeed.Domain>((GameSeed.Domain[])Enum.GetValues(typeof(GameSeed.Domain)));
        domains.Sort((a, b) => ((uint)a).CompareTo((uint)b));

        token += Pad((uint)domains.Count, COUNT_WIDTH);
        foreach (var d in domains)
        {
            ulong count = usage != null ? usage.GetValueOrDefault(d, 0UL) : 0UL;
            token += Pad((uint)d, DOMAIN_ID_WIDTH);
            token += Pad(count, USAGE_WIDTH);
        }

        return token;
    }

    /// <summary>
    /// 토큰 문자열을 해석하여 GameSeed의 상태를 복원합니다.
    /// </summary>
    public static bool RestoreUsageCount(string token)
    {
        try
        {
            int idx = 0;
            if (token.Length < BASE_SEED_WIDTH + COUNT_WIDTH) 
                return false;

            ulong baseSeedEncoded = ReadUInt(token, ref idx, BASE_SEED_WIDTH);
            long baseSeed = unchecked((long)baseSeedEncoded); // ulong 비트를 다시 long(음수 포함)으로 복원

            uint count = (uint)ReadUInt(token, ref idx, COUNT_WIDTH);
            Dictionary<GameSeed.Domain, ulong> usage = new Dictionary<GameSeed.Domain, ulong>();
            for (int i = 0; i < count; i++)
            {
                uint domIdU = (uint)ReadUInt(token, ref idx, DOMAIN_ID_WIDTH);
                ulong u = ReadUInt(token, ref idx, USAGE_WIDTH);
                if (Enum.IsDefined(typeof(GameSeed.Domain), domIdU))
                {
                    usage[(GameSeed.Domain)domIdU] = u;
                }
            }

            GameSeed.RestoreRNG(baseSeed, usage);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// JSON 파일에서 토큰을 읽고, BaseSeed를 함께 추출하여 반환합니다.
    /// </summary>
    public static bool TryLoadSeedToken(out string token, out long baseSeed)
    {
        token = string.Empty;
        baseSeed = 0;
        try
        {
            if (!File.Exists(SeedStatePath)) return false;

            string json = File.ReadAllText(SeedStatePath);
            SeedState state = JsonUtility.FromJson<SeedState>(json);
            if (state == null || string.IsNullOrEmpty(state.seedToken)) 
                return false;

            token = state.seedToken;

            int idx = 0;
            ulong baseSeedEncoded = ReadUInt(token, ref idx, BASE_SEED_WIDTH);
            baseSeed = unchecked((long)baseSeedEncoded);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"SeedToken 로드 실패: {e.Message}");
            return false;
        }
    }   

    /// <summary>
    /// 현재 메모리(GameSeed)에 있는 정보를 바탕으로 JSON 파일을 최신화하여 저장합니다.
    /// </summary>
    public static bool SaveSeedTokenToJson()
    {
        if (!GameSeed.IsInitialized)
        {
            Debug.LogWarning("GameSeed가 초기화되지 않아 저장할 수 없습니다.");
            return false;
        }

        SeedState state = new SeedState
        {
            baseSeed = GameSeed.BaseSeed,
            seedToken = BuildSeedToken(GameSeed.BaseSeed, GameSeed.GetUsageSnapshot()),
            updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // 로컬 시간 기준
        };

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(SeedStatePath, json);
        
        Debug.Log($"[Save] 시드 상태 최신화: Seed={GameSeed.BaseSeed}, Time={state.updatedAt}");
        return true;
    }

    private static string Pad(uint value, int width) => value.ToString().PadLeft(width, '0');
    private static string Pad(ulong value, int width) => value.ToString().PadLeft(width, '0');

    private static ulong ReadUInt(string s, ref int idx, int width)
    {
        string slice = s.Substring(idx, width);
        idx += width;
        return ulong.Parse(slice);
    }
}
