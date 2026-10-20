---
title: "PROJECT REPORT // WORLDFIRSTKILL"
excerpt: "CSV 데이터 갱신·변환과 Seed·Token 기반 난수 상태 저장·복원"
permalink: "/project/worldfirstkill/"
tags: [Data Architecture, Server/Client, C#, Unity, System Design, RNG Determinism]
---

WorldFirstKill은 절차적 생성 요소를 사용하는 RPG입니다. **CSV 기반 기획 데이터 갱신·변환**과 **Seed·Token 기반 난수 상태 저장·복원**을 구현했습니다.

<section id="gallery" class="pf-ftd-post-gallery-section">
    <div class="pf-ftd-section-header">
        <span class="pf-ftd-section-tag pf-mono">● IN-GAME SCREENSHOT GALLERY</span>
        <h3 class="pf-ftd-section-title">인게임 핵심 시스템 & 스크린샷 미리보기</h3>
        <p class="pf-ftd-section-desc">플레이어 팀 베이스캠프와 메인 시스템 UI부터 NPC 의뢰 및 사냥 퀘스트 수락, 월드맵 지역 탐색, 숲 필드 몬스터 실시간 전투, 인벤토리/상점 거래, 그리고 결정적 절차 생성을 위한 랜덤 시드 UI까지 주요 인게임 화면을 넘겨가며 확인할 수 있습니다.</p>
    </div>

    <!-- INTERACTIVE SCREENSHOT GALLERY (HORIZONTAL SCROLL & SLIDER) -->
    <div class="pf-ftd-gallery" id="pfWfkGalleryPost">
        <!-- Topbar Meta & Actions -->
        <div class="pf-ftd-gallery-topbar">
            <div class="pf-ftd-gallery-meta">
                <span class="pf-ftd-counter pf-mono">01 / 06</span>
                <span class="pf-ftd-caption">플레이어 팀 베이스캠프 및 메인 시스템 UI</span>
            </div>
            <a href="https://www.youtube.com/watch?v=EWD2YidSgp0" target="_blank" rel="noopener noreferrer" class="pf-ftd-yt-link" title="YouTube 시연 영상 새 창으로 열기">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="currentColor"><path d="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z"/></svg>
                <span>YouTube 시연 영상 ↗</span>
            </a>
        </div>

        <!-- Main Viewing Stage -->
        <div class="pf-ftd-stage">
            <button type="button" class="pf-ftd-nav pf-ftd-prev" onclick="pfFtdGalleryGo(-1, this)" aria-label="이전 스크린샷">‹</button>
            <img class="pf-ftd-main-img" src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_01.png" alt="플레이어 팀 베이스캠프 및 메인 시스템 UI" loading="eager">
            <button type="button" class="pf-ftd-nav pf-ftd-next" onclick="pfFtdGalleryGo(1, this)" aria-label="다음 스크린샷">›</button>
        </div>

        <!-- Horizontal Scrollable Thumbnail Strip -->
        <div class="pf-ftd-thumbs-wrapper">
            <div class="pf-ftd-thumbs-strip" role="region" aria-label="스크린샷 썸네일 가로 스크롤 목록">
                <button type="button" class="pf-ftd-thumb-card is-active" data-index="0" data-title="플레이어 팀 베이스캠프 및 메인 시스템 UI" onclick="pfFtdGallerySelect(0, this)" title="1. 플레이어 팀 베이스캠프 및 메인 시스템 UI">
                    <img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_01.png" alt="1. 플레이어 팀 베이스캠프 및 메인 시스템 UI" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">01</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="1" data-title="NPC 의뢰 및 사냥 퀘스트 수락 팝업" onclick="pfFtdGallerySelect(1, this)" title="2. NPC 의뢰 및 사냥 퀘스트 수락 팝업">
                    <img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_02.png" alt="2. NPC 의뢰 및 사냥 퀘스트 수락 팝업" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">02</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="2" data-title="월드맵 지역 탐색 및 퀘스트 필드 이동 UI" onclick="pfFtdGallerySelect(2, this)" title="3. 월드맵 지역 탐색 및 퀘스트 필드 이동 UI">
                    <img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_03.png" alt="3. 월드맵 지역 탐색 및 퀘스트 필드 이동 UI" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">03</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="3" data-title="필드 몬스터 조우 및 파티 실시간 전투 액션" onclick="pfFtdGallerySelect(3, this)" title="4. 필드 몬스터 조우 및 파티 실시간 전투 액션">
                    <img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_04.png" alt="4. 필드 몬스터 조우 및 파티 실시간 전투 액션" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">04</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="4" data-title="인벤토리 및 무기/장비 상점 거래 UI" onclick="pfFtdGallerySelect(4, this)" title="5. 인벤토리 및 무기/장비 상점 거래 UI">
                    <img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_05.png" alt="5. 인벤토리 및 무기/장비 상점 거래 UI" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">05</span>
                </button>
                <button type="button" class="pf-ftd-thumb-card" data-index="5" data-title="절차적 맵 생성을 위한 랜덤 시드(Random Seed) 설정 UI" onclick="pfFtdGallerySelect(5, this)" title="6. 절차적 맵 생성을 위한 랜덤 시드(Random Seed) 설정 UI">
                    <img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_06.png" alt="6. 절차적 맵 생성을 위한 랜덤 시드(Random Seed) 설정 UI" loading="lazy">
                    <span class="pf-ftd-thumb-num pf-mono">06</span>
                </button>
            </div>
        </div>
        <div class="pf-ftd-hint">
            <span>Tip: 좌우 화살표 버튼, 키보드 방향키(←, →), 또는 마우스 휠로 썸네일을 옆으로 스크롤하여 탐색할 수 있습니다.</span>
        </div>
    </div>
</section>

## 1. CSV 데이터 파이프라인
{: .chapter-title }

기획 데이터는 여러 Google 스프레드시트로 나뉘어 있습니다. **파일별 갱신 여부 판단과 C# 객체 변환을 공통화**해, 각 테이블에서 같은 다운로드·변환 코드를 반복하지 않도록 구성했습니다. CSV는 시트의 행과 열을 텍스트로 내보낸 형식입니다.

<ol class="wfk-flow" aria-label="CSV 데이터 처리 순서">
    <li><strong>메타 시트 확인</strong><span>데이터 종류·버전·다운로드 주소 조회</span></li>
    <li><strong>버전 비교</strong><span>일치하면 캐시, 다르면 다운로드</span></li>
    <li><strong>CSV 변환</strong><span>열 이름을 C# 필드에 연결하고 타입 변환</span></li>
    <li><strong>인게임 조회</strong><span>변환된 데이터를 ID로 등록·조회</span></li>
</ol>

<table class="wfk-structure">
<thead><tr><th scope="col">클래스</th><th scope="col">책임</th></tr></thead>
<tbody>
<tr><th scope="row"><code>ServerCSV_ConvertData</code></th><td>메타 시트, 버전 비교, 다운로드와 캐시 관리</td></tr>
<tr><th scope="row"><code>CSVParser</code></th><td>CSV 헤더·필드 매핑과 값의 타입 변환</td></tr>
<tr><th scope="row"><code>DataManager</code></th><td>변환된 데이터를 게임에서 조회할 수 있도록 등록</td></tr>
</tbody>
</table>

### 1.1 필요한 파일만 갱신

메타 시트는 각 데이터 파일의 종류·버전·주소를 모은 목록입니다. 이 목록의 버전과 PC에 저장된 CSV를 비교해 **같은 버전이면 로컬 캐시를 읽고, 다르거나 파일이 없으면 다운로드**합니다. 캐시를 읽지 못한 경우도 다운로드 대상으로 돌립니다.

<figure class="wfk-example">
<a href="/assets/images/World%20First%20Kill/wfk_master_meta_sheet.png" target="_blank" rel="noopener noreferrer" aria-label="데이터 종류와 버전, CSV 주소가 정리된 마스터 메타 시트 원본 이미지 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/wfk_master_meta_sheet.png" width="1400" height="860" alt="데이터 종류와 버전, CSV 주소가 정리된 마스터 메타 시트" loading="lazy">
</a>
<figcaption>마스터 메타 시트에서 데이터 종류, 버전, 다운로드 주소를 관리합니다. 각 행의 Link를 통해 각각의 데이터 시트에 접근할 수 있습니다.</figcaption>
</figure>

<div class="wfk-source wfk-rationale" id="master-csv-download">

**마스터 시트와 CSV 주소 관리 구조**

마스터 시트의 **`Link` 주소 목록을 따라 각 CSV를 다운로드**합니다. 받은 CSV는 C# 데이터 객체로 변환해, 몬스터·스킬 등 게임에서 사용하는 데이터로 등록합니다.

<div id="csv-url-structure">

**`csvURLDict` — 데이터 종류와 항목별 CSV 주소 관리**

`csvURLDict`는 **데이터 종류별로 세부 항목 이름과 CSV URL을 연결**합니다. `CSVDataType` Enum의 `Unit`, `Skill`로 종류를 구분하고, 항목 이름인 `"Monster"`, `"Active"`에 해당하는 URL을 저장합니다. 예를 들어 **`Unit`의 `"Monster"` 항목에는 몬스터 CSV 주소**가 연결됩니다.

```csharp
private static Dictionary<CSVDataType, Dictionary<string, string>> csvURLDict;
```

</div>

</div>

<details class="pf-details wfk-code">
<summary>코드 보기: CSV 캐시 사용 판단</summary>

`needsDownload`는 처음에 `true`입니다. 파일 존재와 버전 일치가 확인돼도 읽기가 성공해야 `false`로 바뀝니다.

```csharp
// ServerCSV_ConvertData.cs: 캐시 판단·읽기 부분
if (File.Exists(localFilePath) && localVersion == serverVersion)
{
    try
    {
        fileContent = File.ReadAllText(localFilePath);
        needsDownload = false; // 읽기 성공: 다운로드 생략
    }
    catch (Exception e)
    {
        needsDownload = true; // 읽기 실패: 다운로드 경로로 복귀
    }
}
```
</details>

### 1.2 CSV를 C# 데이터로 변환

**리플렉션**으로 실행 중인 C# 클래스의 필드 목록을 읽어 CSV 열과 연결합니다. 열 이름과 필드 이름의 표기를 맞춘 뒤, 값을 `int`·`float`·`string`으로 변환해 객체에 넣습니다.

<div class="wfk-source" id="reflection-fieldinfo">

**`FieldInfo[] fields`는 필드 정보 객체를 모은 배열**입니다. `FieldInfo`의 자세한 설명은 [Microsoft 공식 문서: FieldInfo](https://learn.microsoft.com/dotnet/api/system.reflection.fieldinfo)를 참고해 주세요.

</div>

예를 들어 아래 시트의 `Name` 열은 별칭 규칙을 거쳐 `Data_Monster.m_monsterName`에 연결됩니다. 몬스터 이름 한 칸이 C# 객체의 문자열 필드로 들어가는 흐름입니다.

<section class="wfk-rationale" id="fieldinfo-rationale" aria-labelledby="fieldinfo-rationale-title">
<p id="fieldinfo-rationale-title"><strong><code>FieldInfo</code>를 채택한 이유</strong></p>

- **사용 이유:** 몬스터·스킬처럼 서로 다른 데이터 클래스를 하나의 CSV 파서로 처리하기 위해, 실행 중 클래스의 필드를 찾는 방식을 선택했습니다.
- **장점:** 테이블마다 값을 옮기는 코드를 반복하지 않고, 공통 이름 매핑과 타입 변환을 재사용할 수 있습니다.
- **주의점:** 직접 대입보다 필드 조회·값 설정 비용이 추가되고, 열 이름·타입 불일치를 컴파일 단계에서 알기 어렵습니다. 필드나 별칭을 추가할 때 매핑·변환 결과를 확인해야 합니다.

</section>

<figure class="wfk-example">
<a href="/assets/images/World%20First%20Kill/wfk_unit_monster_sheet.png" target="_blank" rel="noopener noreferrer" aria-label="몬스터별 헤더와 능력치가 기록된 원본 데이터 시트 원본 이미지 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/wfk_unit_monster_sheet.png" width="1400" height="860" alt="몬스터별 헤더와 능력치가 기록된 원본 데이터 시트" loading="lazy">
</a>
<figcaption>맨 위의 Index·Name 같은 열 이름이 매핑의 출발점입니다. 열 순서에만 의존하지 않고, 이름을 기준으로 C# 필드를 찾습니다.</figcaption>
</figure>

<details class="pf-details wfk-code">
<summary>코드 보기: CSV 헤더와 필드 매핑</summary>

`header`는 현재 CSV 열 이름, `fields`는 `GetFields`로 얻은 C# 필드 정보입니다. `NormalizeField`가 `m_` 접두사와 `monstername → name` 같은 별칭을 정리합니다.

`SetFieldValue`는 `FieldType`을 확인해 숫자 문자열을 변환하고, `SetValue`로 객체에 저장합니다.

```csharp
// CSVParser.SetupFieldDict: 현재 열과 연결할 필드 찾기
string key = Normalize(header);
foreach (FieldInfo field in fields)
{
    if (NormalizeField(field) == key)
    {
        Data_Dict[header] = field.Name; // 열 이름 → 필드 이름
        DataIndex_Dict[i] = header;    // 열 위치 → 열 이름
        break;
    }
}
// CSVParser.parseCSV: 연결한 필드에 현재 행의 값 저장
FieldInfo fieldInfo = myType.GetField(fieldName);
// 실제 코드는 fieldInfo가 null인지 확인한 뒤 호출
SetFieldValue(objectData, fieldInfo, columnData);
```
</details>

## 2. Seed·Token으로 난수 상태 저장·복원
{: .chapter-title }

**Seed는 난수 생성의 시작값**입니다. 같은 생성 방법과 시드에서 같은 순서로 난수를 요청하면 같은 순서의 숫자를 얻습니다. 시드만 저장하면 처음부터 다시 뽑으므로, **몇 번 사용했는지 함께 기록**해 저장한 뒤 필요한 값부터 이어 쓰도록 설계했습니다.

<ol class="wfk-flow" aria-label="난수 상태 저장·복원 순서">
    <li><strong>시드 분리</strong><span>기능마다 따로 난수 생성기 준비</span></li>
    <li><strong>사용량 기록</strong><span>기능마다 난수를 뽑은 횟수 기록</span></li>
    <li><strong>토큰 저장</strong><span>시드와 사용 횟수를 문자열로 저장</span></li>
    <li><strong>난수 이어 쓰기</strong><span>사용한 만큼 다시 뽑아 버리고 이어 쓰기</span></li>
</ol>

<table class="wfk-structure">
<thead><tr><th scope="col">클래스</th><th scope="col">책임</th></tr></thead>
<tbody>
<tr><th scope="row"><code>GameSeed</code></th><td>기능별 난수 생성·사용 횟수 기록·불러온 뒤 이어 쓰기</td></tr>
<tr><th scope="row"><code>SaveLoadManager</code></th><td>토큰 생성·파일 저장·읽기와 난수 이어 쓰기 요청</td></tr>
</tbody>
</table>

<figure class="wfk-example" id="seed-design-origin">
<a href="/assets/images/World%20First%20Kill/extracted_world_first_kill_slide29_shape45_Seed_Example.png" target="_blank" rel="noopener noreferrer" aria-label="참고한 마인크래프트 시드 입력 화면 원본 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/extracted_world_first_kill_slide29_shape45_Seed_Example.png" width="631" height="351" alt="마인크래프트의 세계 생성 시드 입력 화면: 같은 시작값으로 생성 규칙을 재사용하는 발상의 참고자료" loading="lazy">
</a>
<figcaption><strong>Seed 설계의 출발점.</strong> 마인크래프트의 시드 입력 화면에서 시작값으로 생성 결과를 재현하는 발상을 참고했습니다. 이 프로젝트는 각 기능에서 난수를 몇 번 사용했는지도 기록합니다.</figcaption>
</figure>

<figure class="wfk-example wfk-seed-screen" id="game-seed-screen">
<a href="/assets/images/World%20First%20Kill/InGame/wfk_ingame_06.png" target="_blank" rel="noopener noreferrer" aria-label="World First Kill 시드 입력 화면 원본 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/InGame/wfk_ingame_06.png" width="912" height="861" alt="World First Kill의 게임 시작 화면: 시드 입력란과 Random·Start 버튼" loading="lazy">
</a>
<figcaption><strong>실제 게임의 시드 입력 화면.</strong> 시작 전에 시드를 확인하거나 Random 버튼으로 새 시드를 생성합니다.</figcaption>
</figure>

### 2.1 도메인별 난수 분리

**`Domain : uint`** Enum의 ID로 구별하며, **`Map = 1`**·**`Quest = 4`**처럼 기능마다 관련 도메인 ID(정수값)와 해당 ID의 사용량을 따로 관리합니다.

**RNG는 난수 생성기**, **도메인은 맵·대화·스킬처럼 난수를 사용하는 기능 구분**입니다.

<details class="pf-details wfk-code" id="domain-enum">
<summary>코드 보기: 도메인 Enum 정의</summary>

`GameSeed.cs`의 `Domain` 전체 선언입니다. 공통 기능은 1~6, 직업은 100번대, 스킬은 200번대로 구분하며, 총 14개입니다.

```csharp
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
```

</details>

<div class="wfk-domain-structures" id="domain-structures">

**도메인별 관리 자료구조**

두 사전 모두 `Domain` Enum을 키로 사용합니다. `_domainRng`는 ID별 난수 생성기, `_domainUsageCounts`는 ID별 사용량(`ulong`)을 보관합니다.

```csharp
static readonly Dictionary<Domain, System.Random> _domainRng =
    new Dictionary<Domain, System.Random>();

static readonly Dictionary<Domain, ulong> _domainUsageCounts =
    new Dictionary<Domain, ulong>();
```

</div>

<div class="wfk-mixing-rationale wfk-source" id="splitmix-rationale">

**시드 파생:** 기본 시드가 +1이나 -1만큼 달라도 그 차이가 여러 비트에 퍼지도록, SplitMix64에서 영감을 받은 믹싱으로 기본 시드와 도메인 ID를 섞습니다. 아래 코드로 `int` 서브 시드를 만들고, 이를 받은 `System.Random`이 실제 게임 난수를 생성합니다.

<details class="pf-details wfk-code" id="seed-derivation-code">
<summary>코드 보기: DeriveSubSeed</summary>

```csharp
static int DeriveSubSeed(long baseSeed, uint salt)
{
    unchecked
    {
        ulong z = (ulong)baseSeed + 0x9E3779B97F4A7C15UL + ((ulong)salt * 0x85EBCA6BUL);
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        z = z ^ (z >> 31);
        return (int)(z & 0x7FFFFFFF); // 하위 31비트를 int 서브 시드로 사용
    }
}
```

</details>

</div>

<div class="wfk-source" id="domain-initialization">

**도메인 초기화와 난수 사용량 기록**

아래 코드는 초기화 함수 `Init`과 난수 요청 함수 `NextInt`입니다. `Init`은 `DeriveSubSeed`를 호출해 기본 시드와 도메인 ID를 SplitMix64 유사 믹싱으로 섞고, 그 결과인 서브 시드로 도메인별 `System.Random`을 초기화합니다. 실제 난수는 이 생성기에서 얻습니다.

`int` 자료형 난수가 필요하면 `NextInt`에 도메인과 범위를 전달합니다. 호출할 때마다 해당 도메인의 사용량을 기록하고, 지정한 범위의 난수를 반환합니다.

**1. Init — 도메인별 난수 생성기와 사용량 초기화**

<details class="pf-details wfk-code" id="domain-init-code">
<summary>코드 보기: Init</summary>

```csharp
public static void Init(long seed)
{
    BaseSeed = seed;
    _domainRng.Clear();
    _domainUsageCounts.Clear();
    foreach (Domain d in Enum.GetValues(typeof(Domain)))
    {
        _domainRng[d] = new System.Random(DeriveSubSeed(seed, (uint)d));
        _domainUsageCounts[d] = 0UL;
    }
    IsInitialized = true;
}
```

</details>

**2. NextInt — 해당 도메인의 사용량 기록과 난수 반환**

<details class="pf-details wfk-code" id="domain-nextint-code">
<summary>코드 보기: NextInt</summary>

```csharp
public static int NextInt(Domain domain, int minInclusive, int maxExclusive)
{
    _domainUsageCounts[domain] = _domainUsageCounts.GetValueOrDefault(domain, 0UL) + 1UL;
    return _domainRng[domain].Next(minInclusive, maxExclusive);
}
```

</details>

</div>

### 2.2 토큰 저장과 난수 이어 쓰기

<div class="wfk-source" id="token-format">

**토큰 저장 규격**

아래 상수는 자료형의 크기가 아니라 **토큰 안의 십진수 저장 자릿수**입니다. 저장·해석에 같은 폭을 사용합니다.

```csharp
const int BASE_SEED_WIDTH = 20;    // ulong 최대값 기준 20자리
const int COUNT_WIDTH = 3;         // 도메인 개수 3자리
const int DOMAIN_ID_WIDTH = 3;     // 도메인 ID 3자리
const int USAGE_WIDTH = 12;        // 사용량 12자리
```

**현재 게임의 토큰 문자열 길이: 총 233자리(도메인 14개 기준)**

시드 20자리와 도메인 수 3자리에, 도메인마다 ID 3자리·사용량 12자리를 붙입니다. 도메인 하나는 15자리이고, 14개는 210자리이므로 **`20 + 3 + 14 × (3 + 12) = 233자리`**입니다. 사용하지 않은 도메인도 사용량을 0으로 채워 포함합니다.

</div>

<div class="wfk-source" id="token-generation">

**1. 토큰 생성 — `BuildSeedToken`**

`BuildSeedToken`은 **기본 시드와 도메인별 사용 횟수**를 받아, 저장용 숫자 문자열인 **Token**을 만듭니다. `long` 시드는 `unchecked((ulong)baseSeed)`로 비트 패턴을 보존해 저장합니다.

`Pad`는 숫자를 문자열로 바꾸고, 정해진 자릿수보다 짧으면 앞을 0으로 채웁니다. 덕분에 구분자가 없어도 저장할 때와 같은 자릿수로 잘라 읽을 수 있습니다.

<div class="wfk-token-example">
<p><strong>저장 순서:</strong> 시드 20자리 → 도메인 수 3자리 → [ID 3자리 + 사용량 12자리] 반복</p>
<table class="wfk-token-table">
<caption>예시: 시드 42 · Map 3회 · Quest 5회 (나머지 12개 도메인은 0회)</caption>
<thead><tr><th scope="col">저장된 구간</th><th scope="col">해석</th></tr></thead>
<tbody>
<tr><td><code>00000000000000000042</code></td><td>시드 42</td></tr>
<tr><td><code>014</code></td><td>도메인 14개</td></tr>
<tr><td><code>001 | 000000000003</code></td><td>Map: 3회</td></tr>
<tr><td><code>004 | 000000000005</code></td><td>Quest: 5회</td></tr>
</tbody>
</table>
<p><code>|</code>는 난수 ID와 해당 ID의 난수 사용량을 구분하기 위한 표시입니다. 실제 토큰은 인게임 도메인 ID순으로 14개의 데이터를 다루며, <strong>시드 → 도메인 수 → [도메인 ID → 난수 사용량]</strong> 순서로 구분자 없는 하나의 문자열로 저장합니다.</p>
</div>

도메인 ID순으로 정렬한 `domains`를 돌며 사용량 사전에서 횟수를 읽고, 기록이 없으면 0을 넣습니다. 생성된 난수 목록 대신 **시작값과 진행 횟수**를 저장하는 방식입니다.

<details class="pf-details wfk-code" id="token-generation-code">
<summary>코드 보기: BuildSeedToken · Pad</summary>

**`Pad` — 숫자를 정해진 자릿수의 문자열로 저장**

`Pad`는 숫자를 문자열로 바꾸고, `width`보다 짧으면 앞을 0으로 채웁니다. 예를 들어 `Pad(42UL, 20)`은 `"00000000000000000042"`를 만듭니다. `uint` 버전은 도메인 수·ID를, `ulong` 버전은 시드·사용 횟수를 처리합니다.

```csharp
ulong encodedSeed = unchecked((ulong)baseSeed);
string token = Pad(encodedSeed, BASE_SEED_WIDTH);
token += Pad((uint)domains.Count, COUNT_WIDTH);
foreach (GameSeed.Domain d in domains)
{
    ulong count = usage != null ? usage.GetValueOrDefault(d, 0UL) : 0UL;
    token += Pad((uint)d, DOMAIN_ID_WIDTH);
    token += Pad(count, USAGE_WIDTH);
}
return token;

// 저장할 숫자의 자릿수 맞추기
static string Pad(uint value, int width) => value.ToString().PadLeft(width, '0');
static string Pad(ulong value, int width) => value.ToString().PadLeft(width, '0');
```

</details>

</div>

<div class="wfk-source" id="token-parsing">

**2. 토큰 해석 — `RestoreUsageCount`**

토큰 문자열을 **기본 시드와 [도메인 → 사용 횟수] 사전**으로 되돌립니다. 먼저 시드와 도메인 수를 읽고, 시드는 저장할 때의 변환을 반대로 적용해 원래의 `long` 값으로 복구합니다.

<div id="readuint-explanation">

`ReadUInt`는 현재 위치 `idx`에서 정해진 글자 수 `width`만큼 잘라 숫자로 바꿉니다. 읽은 만큼 `idx`를 옮기고, `ref idx`로 다음에 읽을 위치도 함께 바꿔 이어 읽습니다.

위 저장 예시인 **시드 42·도메인 14개**를 읽으면 다음과 같습니다.

<table class="wfk-token-table" id="readuint-example" aria-label="시드 20자리와 도메인 수 3자리를 읽은 결과와 다음 위치">
<thead><tr><th scope="col">읽는 구간</th><th scope="col">반환값·다음 위치</th></tr></thead>
<tbody>
<tr><td>시드 20자리<br><code>00000000000000000042</code></td><td>반환값 <code>42</code><br><code>idx:0→20</code></td></tr>
<tr><td>도메인 수 3자리<br><code>014</code></td><td>반환값 <code>14</code><br><code>idx:20→23</code></td></tr>
</tbody>
</table>

이후 `idx = 23`부터 ID 3자리·사용량 12자리를 14번 읽습니다. 숫자 ID를 `Domain` Enum으로 바꾸고, 해당 사용 횟수를 사전에 연결합니다. 이렇게 읽은 시드와 사전을 `RestoreRNG`에 전달하면 난수 상태 복구가 시작됩니다.

</div>

<details class="pf-details wfk-code" id="token-parsing-code">
<summary>코드 보기: RestoreUsageCount · ReadUInt</summary>

**`ReadUInt` — 토큰의 한 구간을 숫자로 읽고 다음 구간으로 이동**

토큰 `s`에서 현재 위치 `idx`부터 `width`글자를 잘라 숫자로 반환합니다. `ref idx`로 읽은 길이만큼 호출한 쪽의 위치도 옮기므로, 다음 호출은 바로 다음 구간을 읽습니다.

앞의 저장 예시에서 시드 20자리를 읽으면 **42를 반환하고 `idx`는 0 → 20**, 이어 도메인 수 3자리를 읽으면 **14를 반환하고 `idx`는 20 → 23**이 됩니다.

```csharp
int idx = 0;
ulong baseSeedEncoded = ReadUInt(token, ref idx, BASE_SEED_WIDTH);
long baseSeed = unchecked((long)baseSeedEncoded);
uint count = (uint)ReadUInt(token, ref idx, COUNT_WIDTH);
Dictionary<GameSeed.Domain, ulong> usage = new Dictionary<GameSeed.Domain, ulong>();
for (int i = 0; i < count; i++)
{
    uint domIdU = (uint)ReadUInt(token, ref idx, DOMAIN_ID_WIDTH);
    ulong u = ReadUInt(token, ref idx, USAGE_WIDTH);
    usage[(GameSeed.Domain)domIdU] = u;
}
GameSeed.RestoreRNG(baseSeed, usage);

// 한 구간을 읽고 다음 위치로 이동
static ulong ReadUInt(string s, ref int idx, int width)
{
    string slice = s.Substring(idx, width); // 읽을 구간 추출
    idx += width; // 호출한 쪽의 위치도 갱신
    return ulong.Parse(slice); // 문자열 → 숫자
}
```

</details>

</div>

<div class="wfk-source" id="rng-restoration">

**3. 난수 상태 복구 — `RestoreRNG`**

`RestoreRNG`는 해석한 시드와 사용량 사전을 받아, **도메인별 난수 생성기의 진행 위치**를 되찾습니다. 각 도메인 ID와 기본 시드를 `DeriveSubSeed`로 섞어 같은 생성기를 다시 만들면, 난수는 처음부터 나옵니다.

따라서 **저장된 사용 횟수만큼 난수를 다시 뽑아 버린 뒤**, 다음 요청부터 저장 당시의 다음 값을 반환하도록 합니다. 사용량 카운터도 같은 횟수로 맞춰, 이후 저장할 때 누적 사용량이 이어지게 합니다.

<div class="wfk-usage-example" id="why-save-usage">
<p><strong>사용량을 저장해야 하는 이유:</strong> 설명용으로 Map의 난수 순서가 <code>12 → 47 → 8 → 31</code>이라고 가정합니다. 앞의 세 값을 사용한 뒤 저장했다면, 이어서 필요한 값은 <code>31</code>입니다.</p>
<table>
<thead><tr><th scope="col">저장한 정보</th><th scope="col">불러온 뒤 다음 값</th></tr></thead>
<tbody>
<tr><td>시드만 저장</td><td><code>12</code> — 처음부터 다시 시작</td></tr>
<tr><td>시드 + Map 사용량 <code>3</code></td><td><code>31</code> — 이미 쓴 앞의 세 값을 건너뛴 뒤 사용</td></tr>
</tbody>
</table>
</div>

<details class="pf-details wfk-code" id="rng-restoration-code">
<summary>코드 보기: RestoreRNG</summary>

```csharp
foreach (KeyValuePair<Domain, ulong> kv in usageCounts)
{
    Domain d = kv.Key;
    ulong count = kv.Value;     // 토큰 사용량
    System.Random rng = _domainRng[d] = new System.Random(DeriveSubSeed(baseSeed, (uint)d));
    for (ulong i = 0; i < count; i++)
    {
        rng.Next();     // 사용한 횟수만큼 숫자를 버림 호출하여 RNG 상태를 맞춤
    }
    _domainRng[d] = rng;
    _domainUsageCounts[d] = count;   // 사용량 상태 업데이트
}
```

</details>

게임 데이터와 난수 생성 방법이 같아야 같은 결과를 이어 쓸 수 있습니다. 체력·위치는 따로 저장해야 하며, 사용 횟수가 많을수록 다시 뽑아 버리는 작업도 늘어납니다.

</div>

<section class="wfk-comparison" id="skill-seed-comparison" aria-labelledby="skill-seed-comparison-title">
<p id="skill-seed-comparison-title"><strong>시드에 따른 스킬 구성 비교</strong></p>

아래 두 그림은 서로 다른 시드에서 생성된 대표적인 스킬 구성 이미지입니다. 아래와 같이 변경됨을 볼 수 있습니다.

<div class="wfk-example-pair">
<figure class="wfk-example">
<a href="/assets/images/World%20First%20Kill/%EC%8A%A4%ED%81%AC%EB%A6%B0%EC%83%B7%202026-05-19%20212720.png" target="_blank" rel="noopener noreferrer" aria-label="Seed 1459589277에서 생성된 치명타 버프 스킬 구성 원본 이미지 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/%EC%8A%A4%ED%81%AC%EB%A6%B0%EC%83%B7%202026-05-19%20212720.png" width="652" height="482" alt="Seed 1459589277에서 생성된 치명타 버프 스킬 구성" loading="lazy">
</a>
<figcaption>Seed 1459589277: 아군의 치명타 확률·데미지를 높이는 스킬 예시.</figcaption>
</figure>

<figure class="wfk-example">
<a href="/assets/images/World%20First%20Kill/%EC%8A%A4%ED%81%AC%EB%A6%B0%EC%83%B7%202026-05-19%20212644.png" target="_blank" rel="noopener noreferrer" aria-label="Seed 1459589276에서 생성된 단일 대상 공격 스킬 구성 원본 이미지 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/%EC%8A%A4%ED%81%AC%EB%A6%B0%EC%83%B7%202026-05-19%20212644.png" width="645" height="470" alt="Seed 1459589276에서 생성된 단일 대상 공격 스킬 구성" loading="lazy">
</a>
<figcaption>Seed 1459589276: 한 명의 적에게 물리 피해를 주는 스킬 예시.</figcaption>
</figure>

</div>
</section>
