using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static Define;

public static class ServerCSV_ConvertData
{
    // 시트 URL (구글 스프레드시트 CSV URL)
    private const string MasterSheetURL = "https://docs.google.com/spreadsheets/d/1lRHkPJ4bBWnLBiRpdvULuOsfFk-dIVAqr0B-rzpZxPU/export?format=csv&gid=0#gid=0";

    // 로컬 데이터 저장 경로
    private static string LocalDataPath
    {
        get
        {
#if UNITY_EDITOR

            string editorPath = Application.dataPath + "/../LocalGameData_Editor/";
            return editorPath;
#else
            // 배포판에서는 Application.persistentDataPath 사용
            return Application.persistentDataPath + "/LocalGameData/";
#endif
        }
    }

    private static string VersionContainerPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath + "/../data_versions_editor.json";
#else
            return Application.persistentDataPath + "/data_versions.json";
#endif
        }
    }

    // 시트에서 읽은 CSV 주소 정보 저장
    // Dictionary<CSVDataType, Dictionary<AttributeName(string), CSV URL(string)>>
    // 시트의 Attribute 컬럼 값을 키로 사용합니다.
    private static Dictionary<CSVDataType, Dictionary<string, string>> csvURLDict = new Dictionary<CSVDataType, Dictionary<string, string>>();

    // 시트에서 읽은 서버 버전 정보 저장
    // Dictionary<CSVDataType, Dictionary<AttributeName(string), Version(string)>>
    private static Dictionary<CSVDataType, Dictionary<string, string>> _serverVersions = new Dictionary<CSVDataType, Dictionary<string, string>>();

    // 로컬에 저장된 데이터의 버전 정보
    // Dictionary<CSVDataType, Dictionary<AttributeName(string), Version(string)>>
    private static Dictionary<CSVDataType, Dictionary<string, string>> _localVersions = new Dictionary<CSVDataType, Dictionary<string, string>>();

    private static CSVParser parser = new CSVParser(); // CSVParser 인스턴스

    /// <summary>
    /// 시트(WFCK_GameConfiguration) 다운로드
    /// 다운로드한 파싱 데이터를 사용해서, Dictionary를 완성시킴
    /// 이 Dictionary는 나중에 DataManager의 데이터를 세팅하는 것을 도와준다.
    /// </summary>
    public static async Task<bool> DownloadMetaSheetAsync()
    {
        string csvText = await DownloadFileAsync(MasterSheetURL);
        if (string.IsNullOrEmpty(csvText))
        {
            Debug.LogError("시트 다운로드 실패");
            return false;
        }

        string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogError("시트 데이터가 부족합니다.");
            return false;
        }

        csvURLDict.Clear();
        _serverVersions.Clear();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length < 4)
            {
                Debug.LogWarning($"시트 행 데이터 부족: {lines[i]}");
                continue;
            }

            string csvDataTypeStr = columns[0].Trim();
            string attribute = columns[1].Trim();
            string version = columns[2].Trim();
            string link = columns[3].Trim();

            if (!Enum.TryParse(csvDataTypeStr, out CSVDataType csvDataType))
            {
                Debug.LogWarning($"알 수 없는 CSVDataType: {csvDataTypeStr}");
                continue;
            }

            if (!csvURLDict.TryGetValue(csvDataType, out Dictionary<string, string> attrUrlDict))
            {
                attrUrlDict = new Dictionary<string, string>();
                csvURLDict[csvDataType] = attrUrlDict;
            }

            if (attrUrlDict.ContainsKey(attribute))
            {
                Debug.LogWarning($"시트에서 중복된 Attribute 발견 (URL): {csvDataTypeStr} - {attribute}. 마지막 URL로 덮어씁니다.");
                attrUrlDict[attribute] = link;
            }
            else
            {
                attrUrlDict.Add(attribute, link);
            }

            if (!_serverVersions.TryGetValue(csvDataType, out var attrVersionDict))
            {
                attrVersionDict = new Dictionary<string, string>();
                _serverVersions[csvDataType] = attrVersionDict;
            }

            if (attrVersionDict.ContainsKey(attribute))
            {
                Debug.LogWarning($"시트에서 중복된 Attribute 발견 (Version): {csvDataTypeStr} - {attribute}. 마지막 버전으로 덮어씁니다.");
                attrVersionDict[attribute] = version;
            }
            else
            {
                attrVersionDict.Add(attribute, version);
            }
        }

        Debug.Log("시트 파싱 완료");
        return true;
    }

    /// <summary>
    /// 로컬에 저장된 버전 데이터를 로드한다.
    /// </summary>
    private static void LoadLocalVersions()
    {
        _localVersions.Clear();
        if (File.Exists(VersionContainerPath))
        {
            string json = File.ReadAllText(VersionContainerPath);

            //  JSON 문자열을 C# 객체로 역직렬화
            SaveLoadManager.DataVersionContainer datas = JsonUtility.FromJson<SaveLoadManager.DataVersionContainer>(json);
            if (datas != null)
            {
                _localVersions = datas.ToDictionary();
                Debug.Log("로컬 버전 데이터 로드 완료.");
            }
            else
                Debug.LogError($"로컬 버전 데이터 로드 실패");
        }
        else
        {
            Debug.Log("로컬 버전 데이터 파일이 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// 현재 로컬 버전 정보를 파일로 저장한다.
    /// </summary>
    private static void SaveLocalVersions()
    {
        SaveLoadManager.DataVersionContainer datas = SaveLoadManager.DataVersionContainer.FromDictionary(_localVersions);
        string json = JsonUtility.ToJson(datas, true);
        File.WriteAllText(VersionContainerPath, json);
        Debug.Log("로컬 버전 데이터 저장 완료.");
    }


    /// <summary>
    /// 시트 기반으로 모든 CSV 데이터를 가져와서 Dictionary와, 로컬에 저장되어 있는
    /// 데이터를 기반으로, 저장,로드하여 데이터를 세팅해줌
    /// </summary>
    public static async Task<bool> DownloadAllCSVAsync(Action<float, string> progressCallback = null)
    {
        if (csvURLDict.Count == 0 || _serverVersions.Count == 0)
        {
            Debug.LogError("CSV URL 또는 서버 버전 정보가 없습니다. 메타 시트를 먼저 다운로드하세요.");
            return false;
        }

        LoadLocalVersions();

        int totalCount = csvURLDict.Sum(kv => kv.Value.Count);
        int currentCount = 0;

        if (!Directory.Exists(LocalDataPath))
        {
            Directory.CreateDirectory(LocalDataPath);
        }

        foreach (var kvp in csvURLDict)
        {
            CSVDataType dataType = kvp.Key;
            var attrUrlDict = kvp.Value;

            foreach (var attrKvp in attrUrlDict)
            {
                string attribute = attrKvp.Key;
                string url = attrKvp.Value;
                string fileName = $"{dataType}_{attribute}.csv";

                // 작업 시작 전 보고 (0%에서 즉시 변화를 주기 위함)
                progressCallback?.Invoke((float)currentCount / totalCount, fileName);

                string serverVersion = _serverVersions.TryGetValue(dataType, out var sAttrVerDict) 
                    && sAttrVerDict.TryGetValue(attribute, out var sVer) ? sVer : "0";

                string localVersion = _localVersions.TryGetValue(dataType, out var lAttrVerDict) 
                    && lAttrVerDict.TryGetValue(attribute, out var lVer) ? lVer : "0";

                string localFilePath = Path.Combine(LocalDataPath, fileName);
                string fileContent = null;
                bool needsDownload = true;

                if (File.Exists(localFilePath) && localVersion == serverVersion)
                {
                    try
                    {
                        fileContent = File.ReadAllText(localFilePath);
                        needsDownload = false;
                        Debug.Log($"로컬에서 로드: {fileName} (버전: {localVersion})");
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"로컬 파일 '{fileName}' 읽기 실패: {e.Message}. 서버에서 재다운로드합니다.");
                        needsDownload = true;
                    }
                }

                if (needsDownload)
                {
                    Debug.Log($"서버에서 다운로드: {fileName} (로컬:{localVersion} vs 서버:{serverVersion})");
                    fileContent = await DownloadFileAsync(url);
                    if (fileContent == null)
                    {
                        Debug.LogError($"CSV 다운로드 실패: {dataType} - {attribute}");
                        return false;
                    }

                    try
                    {
                        File.WriteAllText(localFilePath, fileContent);
                        if (!_localVersions.ContainsKey(dataType)) _localVersions[dataType] = new Dictionary<string, string>();
                        _localVersions[dataType][attribute] = serverVersion;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"로컬 파일 '{fileName}' 저장 실패: {e.Message}");
                    }
                }

                currentCount++;
                // 작업 완료 후 보고
                progressCallback?.Invoke((float)currentCount / totalCount, fileName);
            }
        }

        SaveLocalVersions();
        Debug.Log("모든 CSV 데이터 다운로드/로드 완료");
        return true;
    }

    /// <summary>
    /// CSV 데이터 파싱 요청
    /// attributeEnum은 해당 CSVDataType에 속하는 Attribute Enum 값 (예: Define.Quest_Attribute.Type)
    /// </summary>
    public static List<T> GetParseData<T>(Enum attributeEnum, bool isCutColumn = false, int start = 0, int end = 0) where T : BaseData, new()
    {
        Type dataType = typeof(T);

        // T 타입에 대응하는 CSVDataType 찾기
        if (!typeToCSVDataType_Dict.TryGetValue(dataType, out CSVDataType csvDataType))
        {
            Debug.LogWarning($"CSVDataType 매핑 실패: {dataType.Name}. typeToCSVDataType_Dict에 추가되었는지 확인하세요.");
            return null;
        }

        // Attribute Enum 값의 문자열 이름 가져오기
        string attributeName = attributeEnum.ToString();

        string fileName = $"{csvDataType}_{attributeName}.csv";
        string localFilePath = Path.Combine(LocalDataPath, fileName);

        string csvData = File.ReadAllText(localFilePath);

        // 찾은 CSV 데이터 문자열 파싱
        List<T> parsedList = parser.parseCSV<T>(csvData, isCutColumn, start, end);
        if (parsedList == null)
        {
            Debug.LogWarning($"{dataType.Name}: 파싱 실패");
            return null;
        }

        return parsedList.OfType<T>().ToList();
    }

    /// <summary>
    /// CSV 파일 다운로드 유틸리티
    /// </summary>
    private static async Task<string> DownloadFileAsync(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
            // Unity 메인 스레드에서 대기
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            return request.downloadHandler.text;
        }
        else
        {
            Debug.LogError($"CSV 다운로드 실패: {request.error} URL: {url}");
            return null;
        }

    }

    /// <summary>
    /// 타입 -> CSVDataType 매핑 (필요에 따라 수정/추가)
    /// Data_Monster는 Unit 타입으로 분류하는 등, 실제 데이터 구조에 맞게 매핑해야 합니다.
    /// </summary>
    private static readonly Dictionary<Type, CSVDataType> typeToCSVDataType_Dict = new Dictionary<Type, CSVDataType>
    {
        {typeof(Data_QuestType),            CSVDataType.Quest },
        {typeof(Data_QuestDialog),          CSVDataType.Quest },

        {typeof(Data_Resource),             CSVDataType.Region },

        {typeof(Data_Weapon),               CSVDataType.Item },
        {typeof(Data_Armor),                CSVDataType.Item },
        {typeof(Data_Acessori),             CSVDataType.Item },
        {typeof(Data_Consumable_Player),    CSVDataType.Item },
        {typeof(Data_Consumable_User),      CSVDataType.Item },
        {typeof(Data_Material_Monster),     CSVDataType.Item },
        {typeof(Data_Material_Quest),       CSVDataType.Item },
        {typeof(Data_Material_Engravement), CSVDataType.Item },
        {typeof(Data_EquipmentStat),        CSVDataType.Item },

        {typeof(Data_CC),                   CSVDataType.Game },

        {typeof(Data_ActiveSkill),          CSVDataType.Skill },
        {typeof(Data_PassiveSkill),         CSVDataType.Skill },
        {typeof(Data_Buff),                 CSVDataType.Skill },

        {typeof(Data_Class),                CSVDataType.Unit },
        {typeof(Data_Monster),              CSVDataType.Unit },
        // 필요에 따라 추가해야 함
    };
}


