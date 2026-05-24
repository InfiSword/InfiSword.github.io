---
layout: single
title: "PROJECT REPORT // WORLDFIRSTKILL"
excerpt: "리플렉션 기반 동적 파싱 및 완전한 상태 복원 시스템"
categories: [project]
permalink: /project/worldfirstkill/
tags: [Data Structure, Server, C#, System Design]
toc: true
toc_sticky: true
classes: wide
---

<style>
    /* Origin Styles Restoration */
    .pf-visual-frame {
        width: 100%; padding: 30px; background: rgba(0,0,0,0.03);
        border: 1px solid #e1e4e8; border-radius: 8px;
        margin: 25px 0; text-align: center;
    }
    .pf-token-container {
        display: flex; flex-direction: column; gap: 20px; margin-top: 25px;
    }
    .pf-token-row {
        display: flex; align-items: center; gap: 15px; flex-wrap: wrap;
    }
    .pf-token-box {
        background: rgba(0, 123, 255, 0.05); padding: 15px 20px; border-radius: 8px; border: 2px solid #007bff; flex: 1; min-width: 200px;
    }
    .pf-token-box-title {
        color: #007bff; font-weight: 700; margin-bottom: 8px; font-family: 'Fira Code', monospace;
    }
    .pf-domain-info {
        margin: 20px 0; padding: 20px; background: rgba(0, 0, 0, 0.02); border-radius: 8px; border-left: 4px solid #28a745;
    }
    .pf-domain-grid {
        display: grid; grid-template-columns: 1fr 1fr; gap: 15px;
    }
    .pf-domain-item {
        background: rgba(40, 167, 69, 0.05); padding: 12px; border-radius: 6px; border: 1px solid #28a745;
    }
</style>

서버(구글 스프레드시트) 기반 게임 데이터 관리와, Seed/Token을 활용한 완벽한 상태 복원 시스템입니다.

---

# 1. 리플렉션 기반 CSV 파싱 시스템

서버(구글 스프레드시트)에서 다운로드한 CSV 파일의 헤더와 데이터 클래스의 필드를 자동으로 매핑하여 동적으로 파싱하는 시스템을 구현했습니다. **리플렉션(Reflection)**을 활용하여 타입 안전성을 보장하면서도 확장 가능한 파싱 로직을 설계했습니다. 새로운 게임 데이터(아이템, 몬스터 등)가 추가될 때마다 매핑 코드를 작성하는 수고를 덜 수 있습니다.

### 1.1 파싱 프로세스 플로우

CSV 파싱은 다음과 같은 4단계로 진행됩니다:

<div class="pf-visual-frame">
    <div style="font-weight: 700; margin-bottom: 20px; color: #007bff; font-family: 'Fira Code', monospace;">CSV Parsing Process</div>
    <div style="display: flex; flex-direction: column; gap: 15px; align-items: center;">
        <div style="background: rgba(35, 134, 54, 0.1); border: 2px solid #28a745; border-radius: 8px; padding: 15px 25px; text-align: center; width: 100%; max-width: 600px;">
            <div style="color: #28a745; font-weight: 700; margin-bottom: 8px;">1단계: 타입 정보 수집</div>
            <div style="color: #666; font-size: 0.85rem;">리플렉션으로 제네릭 타입 T의 모든 필드 정보 추출<br><span style="color: #8b949e;">GetFields()로 클래스 구조 분석</span></div>
        </div>
        <div style="color: #007bff; font-size: 1.5rem; font-weight: bold;">↓</div>
        <div style="background: rgba(88, 166, 255, 0.1); border: 2px solid #007bff; border-radius: 8px; padding: 15px 25px; text-align: center; width: 100%; max-width: 600px;">
            <div style="color: #007bff; font-weight: 700; margin-bottom: 8px;">2단계: 헤더 추출 및 매핑</div>
            <div style="color: #666; font-size: 0.85rem;">CSV 첫 줄(헤더)을 분리하고 클래스 필드명과 자동 매핑<br><span style="color: #8b949e;">예: "m_monsterIndex" ↔ "m_monsterIndex"</span></div>
        </div>
        <div style="color: #007bff; font-size: 1.5rem; font-weight: bold;">↓</div>
        <div style="background: rgba(255, 123, 114, 0.1); border: 2px solid #ff7b72; border-radius: 8px; padding: 15px 25px; text-align: center; width: 100%; max-width: 600px;">
            <div style="color: #ff7b72; font-weight: 700; margin-bottom: 8px;">3단계: 데이터 행 처리 및 객체 생성</div>
            <div style="color: #666; font-size: 0.85rem;">정규식(SmartSplit)으로 쉼표/따옴표 처리 후 인스턴스 생성<br><span style="color: #8b949e;">"이름,설명",100 → ["이름,설명", "100"]</span></div>
        </div>
        <div style="color: #007bff; font-size: 1.5rem; font-weight: bold;">↓</div>
        <div style="background: rgba(88, 166, 255, 0.1); border: 2px solid #007bff; border-radius: 8px; padding: 15px 25px; text-align: center; width: 100%; max-width: 600px;">
            <div style="color: #007bff; font-weight: 700; margin-bottom: 8px;">4단계: 타입 변환 및 필드 할당</div>
            <div style="color: #666; font-size: 0.85rem;">string → int/float/string 자동 변환 후 리스트 반환</div>
        </div>
    </div>
</div>

### 1.2 핵심 파싱 로직 및 SmartSplit

제네릭 메서드를 사용하여 타입에 독립적인 파싱을 수행합니다. 특히, CSV 내부의 쉼표(,)를 포함하는 문자열을 안전하게 처리하기 위해 정규식을 활용한 `SmartSplit` 함수를 구현했습니다.

```csharp
// CSVParser.cs: 정규식을 이용한 안전한 CSV 행 분할
string[] SmartSplit(string line)
{
    // 따옴표로 묶인 데이터 내부의 쉼표는 무시하고, 컬럼을 분리합니다.
    return Regex.Matches(line, @"(?:^|,)(?:""(?<val>[^""]*)""|(?<val>[^,""]*))")
                .Cast<Match>()
                .Select(m => m.Groups["val"].Value)
                .ToArray();
}

// 리플렉션 기반 CSV 파싱 핵심 메서드
public List<T> parseCSV<T>(string csvData) where T : BaseData, new()
{
    Type myType = typeof(T);
    // 1. 클래스의 모든 필드 정보 가져오기
    FieldInfo[] myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

    string[] csvIndexData = csvData.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    string[] attributeColumnArray = csvIndexData[0].Trim().Split(',');

    // 2. CSV 헤더와 클래스 필드 매핑 딕셔너리 구성
    SetupFieldDict(attributeColumnArray, myFieldInfo);
    
    List<T> csvDataList = new List<T>();

    // 3. 데이터 행 순회
    for (int rowIndex = 1; rowIndex < csvIndexData.Length; rowIndex++)
    {
        string[] column = SmartSplit(csvIndexData[rowIndex].Trim());
        if (column.Length == 0 || string.IsNullOrWhiteSpace(column[0])) continue;

        T objectData = Activator.CreateInstance<T>();

        // 4. 각 컬럼 데이터를 알맞은 타입으로 변환하여 할당
        for (int i = 0; i < column.Length; i++)
        {
            if (DataIndex_Dict.TryGetValue(i, out string attribute) &&
                Data_Dict.TryGetValue(attribute, out string fieldName))
            {
                FieldInfo fieldInfo = myType.GetField(fieldName);
                if (fieldInfo != null)
                    SetFieldValue(objectData, fieldInfo, column[i]); // 내부에서 ChangeType 수행
            }
        }
        csvDataList.Add(objectData);
    }
    return csvDataList;
}
```

### 1.3 파싱 예시 (몬스터 데이터)

서버에서 가져온 원시 텍스트가 C# 객체로 변환되는 과정입니다. 매핑을 위해 C# 클래스의 변수명이 CSV의 헤더명과 정확히 일치해야 합니다.

```csharp
// 서버에서 다운로드한 CSV 데이터 (첫 줄: 헤더)
// m_monsterIndex,m_monsterName,m_hp,m_attack,m_defense,m_dropItemID
// 1,슬라임,100,10,5,101

// C# 데이터 클래스 (BaseData 상속)
public class Data_Monster : BaseData
{
    public int m_monsterIndex;
    public string m_monsterName;
    public int m_hp;
    public int m_attack;
    public int m_defense;
    public int m_dropItemID;
}
```

---

# 2. Seed / Token 시스템 (RNG 완전 복원)

게임을 세이브하고 로드했을 때, 단순히 초기 시드값만 저장하면 이미 사용된 난수들이 다시 생성되어 게임 결과가 달라질 수 있습니다. 이를 방지하기 위해 **Seed/Token 시스템**을 설계했습니다.

### 2.1 토큰 구조 다이어그램
토큰은 **고정 폭(Fixed-Width) 인코딩**을 사용하여 파싱 안정성을 확보했습니다. 각 도메인(Map, Quest, Item 등)별로 난수 스트림을 분리하여 관리합니다.

<div class="pf-visual-frame">
    <div style="font-weight: 700; margin-bottom: 20px; color: #007bff; font-family: 'Fira Code', monospace;">Seed Token 구조</div>
    <div class="pf-token-container">
        <div class="pf-token-row" style="justify-content: center;">
            <div class="pf-token-box" style="flex: 0 1 auto;">
                <div class="pf-token-box-title">BaseSeed</div>
                <div style="font-size: 0.85rem;">20자리 고정</div>
                <code style="color: #8b949e; font-size: 0.8rem;">ulong 최대값 기준</code>
            </div>
            <div style="font-size: 1.5rem; color: #007bff;">+</div>
            <div class="pf-token-box" style="flex: 0 1 auto;">
                <div class="pf-token-box-title">Domain 개수</div>
                <div style="font-size: 0.85rem;">3자리 고정</div>
                <code style="color: #8b949e; font-size: 0.8rem;">예: 008</code>
            </div>
        </div>
        <div class="pf-domain-info" style="max-width: 600px; margin: 20px auto;">
            <div style="color: #28a745; font-weight: 700; margin-bottom: 10px; font-family: 'Fira Code', monospace;">Domain 정보 (반복)</div>
            <div class="pf-domain-grid">
                <div class="pf-domain-item">
                    <strong style="color: #28a745;">Domain ID</strong><br>
                    <span style="font-size: 0.85rem; color: #666;">(3자리)</span>
                </div>
                <div class="pf-domain-item">
                    <strong style="color: #28a745;">Usage Count</strong><br>
                    <span style="font-size: 0.85rem; color: #666;">(12자리)</span>
                </div>
            </div>
        </div>
    </div>
</div>

### 2.2 기술적 구현 (Build & Restore)

*   **Build:** 현재 초기 시드와 각 서브 도메인(영역)별 난수 호출 횟수(Usage Count)를 가져와 하나의 긴 문자열 토큰으로 결합합니다.
*   **Restore:** 토큰을 정해진 폭(20자리, 3자리, 12자리 등) 단위로 잘라내어 시드와 사용량을 복구한 뒤, RNG 엔진의 상태를 세이브 시점으로 정확히 강제 동기화합니다.

```csharp
// SaveLoadManager.cs: 시드와 사용량을 토큰 문자열로 결합
private static string BuildSeedToken(long baseSeed, Dictionary<GameSeed.Domain, ulong> usage)
{
    // 음수 시드 처리를 위해 ulong으로 안전하게 변환 후 20자리 패딩
    ulong encodedSeed = unchecked((ulong)baseSeed);
    string token = Pad(encodedSeed, 20);

    List<GameSeed.Domain> domains = new List<GameSeed.Domain>((GameSeed.Domain[])Enum.GetValues(typeof(GameSeed.Domain)));
    domains.Sort((a, b) => ((uint)a).CompareTo((uint)b));

    // 도메인 개수 3자리 패딩
    token += Pad((uint)domains.Count, 3);
    foreach (var d in domains)
    {
        ulong count = usage != null ? usage.GetValueOrDefault(d, 0UL) : 0UL;
        token += Pad((uint)d, 3);   // 도메인 ID 3자리
        token += Pad(count, 12);    // 사용량 12자리
    }
    return token;
}

// GameSeed.cs: 토큰에서 추출한 정보로 RNG 엔진 상태 강제 복원
public static void RestoreRNG(long baseSeed, Dictionary<Domain, ulong> usageCounts)
{
    foreach (var kv in usageCounts)
    {
        Domain d = kv.Key;
        ulong count = kv.Value;     
        
        // 서브 시드로 해당 도메인의 RNG 객체 재초기화
        System.Random rng = new System.Random(DeriveSubSeed(baseSeed, (uint)d));
        
        // 사용한 횟수만큼 숫자를 버림(Skip) 호출하여 RNG 상태를 세이브 시점과 동기화
        for (ulong i = 0; i < count; i++)
        {
            rng.Next();     
        }
        domainRng[d] = rng;
        domainUsageCounts[d] = count;
    }
}
```

---

# 3. 비동기 로딩 및 서버 동기화

수만 줄에 달하는 방대한 CSV 데이터를 게임 시작 시 동기적으로 파싱하면 메인 스레드가 차단되어 화면 멈춤(Freezing) 현상이 발생합니다. 이를 방지하고 서버 데이터를 효율적으로 관리하기 위해 `async/await` 기반의 **비동기 로딩 및 다운로드 시스템**을 구축했습니다.

### 3.1 로딩 프로세스 (LoadingManager)

게임 시작 시 데이터와 맵을 로드하는 과정은 가중치 기반 진행률(Progress) 시스템과 함께 비동기로 처리됩니다.

1.  **메타 시트 체크 (0~5%):** 서버의 버전 정보를 담은 메타 시트를 우선 다운로드하여 각 데이터 파일의 최신 버전을 확인합니다.
2.  **조건부 다운로드 (5~80%):** 메타 시트와 로컬의 `data_versions.json`을 비교하여, `로컬 버전 != 서버 버전`인 CSV 파일만 선택적으로 다운로드하여 네트워크 대역폭과 시간을 절약합니다.
3.  **데이터 파싱 및 적재 (80~100%):** 리플렉션 기반 파서로 CSV를 파싱하고 `DataManager`의 딕셔너리에 적재합니다.

```csharp
// LoadingManager.cs: 비동기 데이터 로딩 흐름
public async Task<bool> StartLoadingDataAsync(CancellationToken cancellationToken = default)
{
    loadingUI.gameObject.SetActive(true);
    UpdateProgress(0.01f, "서버 목록 확인 중...");

    // 1. 메타 시트 다운로드
    bool metaSuccess = await ServerCSV_ConvertData.DownloadMetaSheetAsync();
    
    // 2. 조건부 CSV 다운로드 (가중치 0.05 ~ 0.8)
    bool csvSuccess = await ServerCSV_ConvertData.DownloadAllCSVAsync((progress, fileName) =>
    {
        float weightedProgress = 0.05f + (progress * (0.8f - 0.05f));
        UpdateProgress(weightedProgress, $"데이터 수신 중: {fileName}");
    });

    // 3. 서버 데이터 로드 (파싱 및 딕셔너리 적재)
    UpdateProgress(0.8f, "데이터 로드 중...");
    await Managers.Data.ServerDataLoadAsync();

    UpdateProgress(1.0f, "데이터 로딩 완료!");
    return true;
}
```

### 3.2 UI 블로킹 방지 및 서버 통신

`UnityWebRequest`를 통해 백그라운드에서 데이터를 가져오며, `Task.Yield()`와 `async/await`를 활용하여 메인 스레드(렌더링 루프)를 차단하지 않습니다. 이를 통해 데이터를 다운로드하고 파싱하는 동안에도 로딩 게이지 애니메이션이 부드럽게 재생됩니다.

```csharp
// ServerCSV_ConvertData.cs: 비동기 웹 통신 유틸리티
private static async Task<string> DownloadFileAsync(string url)
{
    UnityWebRequest request = UnityWebRequest.Get(url);
    UnityWebRequestAsyncOperation operation = request.SendWebRequest();

    // 완료될 때까지 메인 스레드에 제어권 양보 (화면 멈춤 방지)
    while (!operation.isDone)
    {
        await Task.Yield(); 
    }

    if (request.result == UnityWebRequest.Result.Success)
        return request.downloadHandler.text;
    else
        return null;
}
```

{: .notice--success}
**성과:** `async/await` 패턴과 조건부 다운로드 시스템을 도입하여, 방대한 데이터를 로드하는 동안 화면 멈춤을 완벽히 해결하고 네트워크 사용량을 최소화했습니다.
