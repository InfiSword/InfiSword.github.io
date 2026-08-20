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
    /* --- Premium Report Layout Styles --- */
    
    .chapter-title {
        font-size: 2.2rem;
        color: #fff !important;
        background: linear-gradient(90deg, #007bff 0%, #58A6FF 100%);
        padding: 25px 35px;
        border-radius: 12px;
        margin-top: 60px !important;
        margin-bottom: 40px !important;
        box-shadow: 0 10px 30px rgba(0, 123, 255, 0.15);
        display: flex;
        align-items: center;
        border: none !important;
    }
    .chapter-title::before {
        content: "CHAPTER.";
        font-family: 'Fira Code', monospace;
        font-size: 0.9rem;
        letter-spacing: 2px;
        margin-right: 15px;
        opacity: 0.8;
    }

    .pf-visual-frame {
        width: 100%; padding: 35px; background: #fcfcfc;
        border: 1px solid #e1e4e8; border-radius: 16px;
        margin: 30px 0; text-align: center;
        box-shadow: inset 0 2px 10px rgba(0,0,0,0.02);
    }
    .pf-arch-diagram {
        display: flex; flex-direction: column; gap: 20px; margin: 25px 0;
    }
    .pf-arch-layer {
        padding: 20px 25px; border: 1px solid #e1e4e8; border-radius: 8px;
        background: #fff; position: relative; text-align: center;
        box-shadow: 0 4px 12px rgba(0,0,0,0.03);
    }
    .pf-arch-layer::before {
        content: ""; position: absolute; left: 0; top: 0; bottom: 0; width: 5px;
        background: #007bff; border-radius: 8px 0 0 8px;
    }
    .pf-arch-layer-title {
        color: #007bff; font-weight: 700; margin-bottom: 10px;
        font-family: 'Fira Code', monospace; font-size: 0.95rem; text-align: center;
    }
    .pf-arch-layer-items {
        display: flex; flex-wrap: wrap; gap: 10px; margin-top: 10px; justify-content: center;
    }
    .pf-arch-item {
        padding: 6px 14px; background: rgba(0, 123, 255, 0.08);
        border: 1px solid rgba(0, 123, 255, 0.3); border-radius: 6px;
        font-size: 0.85rem; color: #0056b3; text-align: center;
    }

    /* --- Token System Styling --- */
    .pf-token-container { display: flex; flex-direction: column; gap: 20px; margin-top: 25px; }
    .pf-token-row { display: flex; align-items: center; gap: 15px; flex-wrap: wrap; justify-content: center; }
    .pf-token-box { background: rgba(0, 123, 255, 0.05); padding: 15px 20px; border-radius: 8px; border: 2px solid #007bff; flex: 0 1 auto; min-width: 180px; text-align: center; }
    .pf-token-box-title { color: #007bff; font-weight: 700; margin-bottom: 8px; font-family: 'Fira Code', monospace; }
    .pf-domain-info { margin: 20px auto; padding: 20px; background: #fff; border-radius: 8px; border-left: 4px solid #28a745; border: 1px solid #eee; max-width: 600px; }
    .pf-domain-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; }
    .pf-domain-item { background: rgba(40, 167, 69, 0.05); padding: 12px; border-radius: 6px; border: 1px solid #28a745; text-align: center; }

    /* --- Code Details --- */
    details.pf-details {
        margin: 20px 0;
        border: 1px solid #e1e4e8;
        border-radius: 8px;
        background: #f8f9fa;
        overflow: hidden;
    }
    details.pf-details summary {
        padding: 15px 20px;
        font-weight: 700;
        color: #007bff;
        cursor: pointer;
        outline: none;
        background: #fff;
        display: flex;
        align-items: center;
    }

    .highlight-box {
        background: #f0f7ff;
        border-left: 5px solid #007bff;
        padding: 20px;
        margin: 20px 0;
        border-radius: 0 8px 8px 0;
    }

    /* --- Flow Arrows --- */
    .flow-arrow {
        color: #007bff; font-weight: bold; font-size: 1.5rem; margin: 10px 0;
    }
</style>

서버(구글 스프레드시트) 기반 게임 데이터 관리와, Seed/Token을 활용한 완벽한 상태 복원 시스템입니다. 리플렉션을 통한 동적 파싱과 비동기 로딩 시스템을 통해 유지보수성과 유저 경험을 동시에 확보했습니다.

---

## 1. 리플렉션 기반 CSV 파싱 및 버전 관리 시스템
{: .chapter-title }

서버에서 다운로드한 CSV 파일의 헤더와 데이터 클래스의 필드를 자동으로 매핑하여 동적으로 파싱하는 시스템을 구현했습니다. **리플렉션(Reflection)**을 활용하여 타입 안전성을 보장하면서도 확장 가능한 파싱 로직을 설계했습니다.

### 1.1 서버 버전 동기화 및 선택적 다운로드

게임 시작 시 `ServerCSV_ConvertData` 클래스가 메타 시트를 우선 다운로드하여 서버의 데이터 버전 정보를 확인합니다. 이후 로컬에 저장된 `data_versions.json` 파일과 대조하여 **변경된 파일만 선택적으로 다운로드**함으로써 네트워크 비용과 로딩 시간을 최소화했습니다.

### 1.2 CSV 파싱 프로세스 플로우

다운로드 된 데이터 파싱은 다음과 같은 4단계로 진행됩니다:

<div class="pf-visual-frame">
    <div style="font-weight: 700; margin-bottom: 20px; color: #007bff; font-family: 'Fira Code', monospace;">CSV Parsing Process</div>
    <div class="pf-arch-diagram">
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">1단계: 타입 정보 수집</div>
            <div style="font-size: 0.85rem; color: #666;">리플렉션으로 제네릭 타입 T의 모든 필드 정보(FieldInfo) 추출</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">2단계: 헤더 추출 및 매핑</div>
            <div style="font-size: 0.85rem; color: #666;">CSV 첫 줄(헤더)을 분리하고 클래스 필드명과 자동 매핑을 위한 딕셔너 구축</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">3단계: 데이터 행 처리 및 객체 생성</div>
            <div style="font-size: 0.85rem; color: #666;">정규식(SmartSplit)으로 쉼표/따옴표 처리 후 빈 인스턴스(Activator) 생성</div>
        </div>
        <div class="flow-arrow" style="text-align: center;">↓</div>
        <div class="pf-arch-layer">
            <div class="pf-arch-layer-title">4단계: 타입 변환 및 필드 할당</div>
            <div style="font-size: 0.85rem; color: #666;">Convert.ChangeType을 통해 string 데이터를 실제 필드 타입으로 변환 후 할당</div>
        </div>
    </div>
</div>

### 1.3 핵심 파싱 로직 및 SmartSplit

제네릭 메서드를 사용하여 타입에 독립적인 파싱을 수행합니다. 특히, CSV 내부의 쉼표(,)를 포함하는 문자열("이름, 설명")을 안전하게 처리하기 위해 정규식을 활용한 `SmartSplit` 함수를 구현했습니다.

**CSV 파싱 핵심 로직 및 SmartSplit**
정규식을 활용하여 따옴표 내부의 쉼표를 안전하게 처리하고, 리플렉션을 통해 동적으로 객체에 값을 할당합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: CSV 파싱 핵심 로직 및 SmartSplit</summary>

```csharp
// CSVParser.cs: 정규식을 이용한 안전한 CSV 행 분할
string[] SmartSplit(string line)
{
    return Regex.Matches(line, @"(?:^|,)(?:""(?<val>[^""]*)""|(?<val>[^,""]*))")
                .Cast<Match>()
                .Select(m => m.Groups["val"].Value)
                .ToArray();
}

// 리플렉션 기반 CSV 파싱 핵심 메서드
public List<T> parseCSV<T>(string csvData, bool isCutColumn, int start, int end) where T : BaseData, new()
{
    // 1. 제네릭 타입 T의 구조 분석
    Type myType = typeof(T);
    FieldInfo[] myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

    string[] csvIndexData = csvData.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    if (csvIndexData.Length == 0) return null;

    // 2. 헤더 매핑 딕셔너리 세팅
    string[] attributeColumnArray = csvIndexData[0].Trim().Split(',');
    SetupFieldDict(attributeColumnArray, myFieldInfo);

    List<T> csvDataList = new List<T>();
    // 3. 데이터 행 순회 및 파싱
    for (int rowIndex = 1; rowIndex < csvIndexData.Length; rowIndex++)
    {
        string[] column = SmartSplit(csvIndexData[rowIndex].Trim());
        if (column.Length == 0 || string.IsNullOrWhiteSpace(column[0])) continue;

        T objectData = Activator.CreateInstance<T>();
        
        // 4. 타입 변환 및 할당
        for (int i = 0; i < column.Length; i++)
        {
            if (DataIndex_Dict.TryGetValue(i, out string attribute) && Data_Dict.TryGetValue(attribute, out string fieldName))
            {
                FieldInfo fieldInfo = myType.GetField(fieldName);
                if (fieldInfo != null)
                {
                    SetFieldValue(objectData, fieldInfo, column[i]); // ChangeType 수행
                }
            }
        }
        csvDataList.Add(objectData);
    }
    return csvDataList;
}
```
</details>

---

## 2. 비동기 로딩 및 UI 블로킹 방지
{: .chapter-title }

방대한 CSV 데이터와 복잡한 맵 데이터를 메인 스레드에서 동기적으로 처리하면 화면 멈춤(Freezing) 현상이 발생합니다. 이를 방지하기 위해 `async/await` 기반의 **비동기 로딩 및 맵 초기화 시스템**을 구축했습니다.

### 2.1 가중치 기반 로딩 시스템 (LoadingManager)

데이터 다운로드, 파싱, 그리고 맵 생성의 진행 과정을 각 파트의 비중에 따라 가중치(Weight)를 두어 세밀하게 계산합니다. `Task.Yield()`를 활용하여 연산 도중 주기적으로 메인 렌더링 루프에 제어권을 양보함으로써 로딩 UI 애니메이션이 부드럽게 유지되도록 최적화했습니다.

**비동기 데이터 로딩 흐름**
서버 데이터 다운로드부터 파싱 적재까지 하나의 비동기 파이프라인으로 관리합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: 비동기 데이터 로딩 흐름</summary>

```csharp
// LoadingManager.cs: 비동기 데이터 로딩 흐름
public async Task<bool> StartLoadingDataAsync(CancellationToken cancellationToken = default)
{
    UpdateProgress(0.01f, "서버 목록 확인 중...");

    // 1. 메타 시트 비동기 다운로드
    bool metaSuccess = await ServerCSV_ConvertData.DownloadMetaSheetAsync();
    
    // 2. 조건부 CSV 다운로드 (진행률 가중치 적용 5% ~ 80%)
    bool csvSuccess = await ServerCSV_ConvertData.DownloadAllCSVAsync((progress, fileName) =>
    {
        UpdateProgress(0.05f + (progress * 0.75f), $"데이터 수신 중: {fileName}");
    });

    // 3. 서버 데이터 로드 (DataManager 비동기 파싱)
    UpdateProgress(0.80f, "데이터 로드 중...");
    await Managers.Data.ServerDataLoadAsync();

    UpdateProgress(1.0f, "데이터 로딩 완료!");
    return true;
}
```
</details>

---

## 3. 퀘스트 수락, 진행 및 동적 보상 시스템
{: .chapter-title }

구글 시트에서 파싱된 퀘스트 데이터 형식을 바탕으로, `QuestManager`가 상황에 맞는 퀘스트 객체를 팩토리 패턴(Handler)으로 생성합니다. 퀘스트 진행 상황은 이벤트 버스 시스템에 의해 완벽히 디커플링되어 처리됩니다.

### 3.1 이벤트 구동형 퀘스트 진행 (Event-Driven)

몬스터가 처치될 때마다 발생하는 `OnMonsterKilled` 이벤트를 `QuestManager`가 구독합니다. 활성화된 퀘스트(ActiveQuestList)의 타겟 정보와 일치할 경우 진행도를 올리며, 목표 수치에 도달하면 퀘스트 완료 이벤트를 다시 발행합니다.

### 3.2 시드(Seed) 기반 동적 보상

퀘스트 레벨과 `Data_Resource` 베이스 값을 기준으로, 퀘스트 수락 시점의 난이도와 `GameSeed` 가중치를 곱하여 게임할 때마다 조금씩 달라지는 동적 보상(Gold, Exp)을 산출합니다.

**퀘스트 이벤트 및 보상 계산 로직**
몬스터 처치 이벤트 구독과 시드 기반 보상 변동 로직입니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: 퀘스트 이벤트 및 보상 계산 로직</summary>

```csharp
// QuestManager.cs: 몬스터 처치 이벤트 핸들링 및 진행도 체크
private void HandleMonsterKilled(int monsterId)
{
    var key = (typeof(Unit_Monster), monsterId);
    List<int> completedQuestIndices = new List<int>();

    foreach (Quest quest in ActiveQuestList)
    {
        if (IsTargetQuest(quest, key))
        {
            // 진행도 증가 및 UI 업데이트 이벤트 발행
            UpdateQuestProgress(quest);

            if (quest.IsQuestCompleted())
                completedQuestIndices.Add(CompleteQuest(quest));
        }
    }

    if (completedQuestIndices.Count > 0)
    {
        Managers.Event.QuestCompleted(completedQuestIndices);
    }
}

// Quest.cs: 시드 기반 동적 보상 산출 (생성자 단계)
// GameSeed의 Reward 도메인을 사용하여 동일한 시드일 때 동일한 퀘스트 보상을 보장
float RewardRatio = (float)GameSeed.NextInt(GameSeed.Domain.Reward, 0, 11) / 10f;

m_rewardGold = Mathf.RoundToInt(Managers.Data.ResourceData[m_questLevel].questGold * ((float)ctx.difficultyTag * RewardRatio));
m_rewardExp = Mathf.RoundToInt(Managers.Data.ResourceData[m_questLevel].questExp * ((float)ctx.difficultyTag * (1f - RewardRatio)));
```
</details>

---

## 4. 계층적 맵 생성 및 이동 아키텍처
{: .chapter-title }

게임의 월드는 거대한 1개의 세계(Map) 안에 여러 지역(Region)이 있고, 지역 안에 수많은 전투 전장(Field)이 존재하는 **Map -> Region -> Field**의 계층적 구조를 가집니다. 

### 4.1 비동기 맵 초기화 및 구성
대규모 맵 객체를 한 번에 생성하면 부하가 크기 때문에, `MapManager`의 `NewGame_InitMap` 메서드가 `async/await` 흐름을 타고 Map과 Region 내부의 퀘스트/필드를 순차적으로 생성합니다. 생성에 관여하는 모든 무작위 확률은 `GameSeed.Domain.Map`, `GameSeed.Domain.Region` 등 분리된 난수 스트림에 의해 결정론적(Deterministic)으로 구성됩니다.

### 4.2 이벤트 기반 맵 이동 시스템
유저가 UI를 통해 특정 Field를 선택하면, `Managers.Event.OnEnterFieldEvent`가 발생합니다. `MapManager`는 이를 수신하여 현재 전투 환경(Battle Manager 상태)을 업데이트하고 `BGRender` 컴포넌트를 이용해 배경 이미지를 교체하며 페이드인 효과를 발생시킵니다.

**비동기 맵 계층 초기화 로직**
Map 객체 하위에 Region을 묶고, Region 내부에 퀘스트와 Field를 생성하여 인덱스로 연결합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: 비동기 맵 초기화 및 계층 구조 생성 로직</summary>

```csharp
// MapManager.cs: 비동기 맵 초기화 진입점
public async Task<bool> NewGame_InitMap(Func<float, string, Task> onProgress)
{
    // 1. 최상위 월드 맵 생성 (레벨/개수 지정)
    Map map1 = DataFactory.MakeMap(1, 3, m_mapParentObj.transform);
    Map map10 = DataFactory.MakeMap(10, 3, m_mapParentObj.transform);
    Map map20 = DataFactory.MakeMap(20, 3, m_mapParentObj.transform);

    // 2. 월드맵 하위에 할당된 모든 지역(Region)들에 대한 내부 필드/퀘스트 비동기 생성 
    if (WorldRegionList != null && WorldRegionList.Count != 0)
    {
        foreach (Region region in WorldRegionList)
        {
            // Region.GenerateRegion 안에서 Field 객체들이 생성되며 진행도 보고를 전달
            await region.GenerateRegion(onProgress);
        }
    }

    IsLoadedData = true;
    return true;
}

// Region.cs: 지역 내 필드 및 퀘스트 생성
public async Task GenerateRegion(Func<float, string, Task> onProgress)
{
    int questCount = GameSeed.NextInt(GameSeed.Domain.Region, 4, 7);
    MonsterType monsterType = GameSeed.RandomEnum<MonsterType>(GameSeed.Domain.Region);
    monsterSet = new MonsterSet(monsterType);
    
    // ... 생략 ...

    for (int i = 0; i < questCount; i++)
    {
        Quest mainQuest = Managers.Quest.GenerateQuest(monsterSet, ref currentQuestLevel, questDataList, questDialogList);
        int questIndex = Managers.Quest.MainQuestList.IndexOf(mainQuest);

        // Field 객체를 생성하고, 방금 생성된 메인 퀘스트의 인덱스를 넘겨 결합시킴
        Field field = DataFactory.MakeField(this, questIndex, transform);
        
        // ... (인덱스 저장 관리 생략)
        if (onProgress != null)
            await onProgress((float)(i + 1) / questCount, $"지역 생성 중...");
    }
}
```
</details>

---

## 5. Seed / Token 시스템 (RNG 완전 복원)
{: .chapter-title }

게임을 세이브하고 로드했을 때, 단순히 맵과 캐릭터의 상태만을 저장하는 것을 넘어 생성 규칙(시드) 자체를 완벽히 롤백해야 합니다. 초기 시드값만 저장할 경우 게임 도중 이미 소모된 난수 횟수로 인해 세이브 로드 시점에 다음 생성되는 아이템이나 맵이 달라지는 문제가 발생합니다. 이를 극복하기 위한 **Seed/Token 복원 시스템**입니다.

### 5.1 도메인 격리 구조 (Domain Isolation)

`System.Random` 객체를 하나만 사용하면, 퀘스트 생성 시 난수를 소모한 횟수에 따라 맵 생성 결과가 뒤틀리게 됩니다. 이를 방지하기 위해 `GameSeed`는 `Domain`(Map, Quest, Field, Reward 등)을 분리하여 각각 독립된 Random 객체 스트림을 보유합니다. 

### 5.2 고정 폭 인코딩 기반 토큰 다이어그램

모든 도메인의 난수 사용 횟수(Usage Count)를 하나의 암호화된 토큰(Token)으로 병합합니다. 파싱 시 데이터가 꼬이지 않도록 **고정 폭(Fixed-Width) 인코딩**을 사용했습니다.

<div class="pf-visual-frame">
    <div style="font-weight: 700; margin-bottom: 20px; color: #007bff; font-family: 'Fira Code', monospace;">Seed Token 구조 (Fixed-Width)</div>
    <div class="pf-token-container">
        <div class="pf-token-row">
            <div class="pf-token-box">
                <div class="pf-token-box-title">BaseSeed</div>
                <div style="font-size: 0.85rem;">20자리 고정 (ulong 패딩)</div>
            </div>
            <div class="flow-arrow" style="text-align: center;">+</div>
            <div class="pf-token-box">
                <div class="pf-token-box-title">Domain 개수</div>
                <div style="font-size: 0.85rem;">3자리 고정 (예: 008)</div>
            </div>
        </div>
        <div class="pf-domain-info">
            <div style="color: #28a745; font-weight: 700; margin-bottom: 10px; font-family: 'Fira Code', monospace;">Domain 정보 블록 (등록된 도메인 개수만큼 반복 병합)</div>
            <div class="pf-domain-grid">
                <div class="pf-domain-item">
                    <strong style="color: #28a745;">Domain ID</strong><br>
                    <span style="font-size: 0.85rem; color: #666;">(3자리 패딩)</span>
                </div>
                <div class="pf-domain-item">
                    <strong style="color: #28a745;">Usage Count</strong><br>
                    <span style="font-size: 0.85rem; color: #666;">(12자리 패딩)</span>
                </div>
            </div>
        </div>
    </div>
</div>

### 5.3 토큰 빌드 및 상태 복원 흐름

저장 시 위 규칙대로 텍스트 토큰을 빌드하여 저장하고, 로드할 때는 이 문자열을 길이 단위로 잘라낸 뒤 각 도메인의 Random 객체를 생성하고 사용 횟수만큼 `Next()`를 더미 호출(Skip) 시켜 게임의 미래 확률 상태를 정확히 세이브 시점과 동일하게 동기화합니다.

**토큰 인코딩 및 RNG 상태 강제 복구 로직**
고정 폭 문자열을 결합하여 저장하고, 로드시 추출하여 각 도메인의 스트림을 동기화합니다.

<details class="pf-details" markdown="1">
<summary>코드 보기: 토큰 인코딩 및 RNG 상태 복구 로직</summary>

```csharp
// SaveLoadManager.cs: 시드와 도메인 사용량을 하나의 고정 폭 토큰으로 결합
private static string BuildSeedToken(long baseSeed, Dictionary<GameSeed.Domain, ulong> usage)
{
    ulong encodedSeed = unchecked((ulong)baseSeed);
    string token = Pad(encodedSeed, 20); // 20자리 패딩

    List<GameSeed.Domain> domains = new List<GameSeed.Domain>((GameSeed.Domain[])Enum.GetValues(typeof(GameSeed.Domain)));
    domains.Sort((a, b) => ((uint)a).CompareTo((uint)b));

    token += Pad((uint)domains.Count, 3);
    foreach (var d in domains)
    {
        ulong count = usage != null ? usage.GetValueOrDefault(d, 0UL) : 0UL;
        token += Pad((uint)d, 3);    // 도메인 ID 3자리
        token += Pad(count, 12);     // 사용량 12자리
    }
    return token;
}

// GameSeed.cs: 토큰에서 추출된 정보로 RNG 엔진을 과거 상태로 복원
public static void RestoreRNG(long baseSeed, Dictionary<Domain, ulong> usageCounts)
{
    foreach (var kv in usageCounts)
    {
        Domain d = kv.Key;
        ulong count = kv.Value;     
        
        // BaseSeed를 해싱(Salt)하여 도메인별 고유 서브 시드로 RNG 재생성
        System.Random rng = new System.Random(DeriveSubSeed(baseSeed, (uint)d));
        
        // 토큰에 기록된 횟수만큼 난수를 버림(Skip) 호출하여 확률 스트림 위치를 강제 동기화
        for (ulong i = 0; i < count; i++)
        {
            rng.Next();     
        }
        
        domainRng[d] = rng;
        domainUsageCounts[d] = count;
    }
}
```
</details>

{: .notice--success}
**Conclusion:** `WorldFirstKill`은 비동기 파싱과 계층적 맵 생성으로 방대한 데이터 로딩 문제를 최적화했으며, Seed/Token 시스템을 통해 로그라이크 특유의 '완벽한 게임 플레이(RNG) 복원력'을 기술적으로 완성한 수준 높은 아키텍처를 자랑합니다.
