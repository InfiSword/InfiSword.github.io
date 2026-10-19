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

기획 데이터를 Google 스프레드시트에서 읽어 게임 객체로 연결합니다. **버전이 같은 CSV는 로컬 캐시를 사용**하고, 변경되거나 없는 파일을 내려받아 공통 파서로 변환합니다.

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

로컬 파일과 버전이 모두 일치하면 캐시를 읽습니다. 파일을 읽지 못하면 다운로드 대상으로 돌려, 캐시 판단과 파일 갱신을 연결했습니다.

<figure class="wfk-example">
<a href="/assets/images/World%20First%20Kill/wfk_master_meta_sheet.png" target="_blank" rel="noopener noreferrer" aria-label="데이터 종류와 버전, CSV 주소가 정리된 마스터 메타 시트 원본 이미지 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/wfk_master_meta_sheet.png" width="1400" height="860" alt="데이터 종류와 버전, CSV 주소가 정리된 마스터 메타 시트" loading="lazy">
</a>
<figcaption>마스터 메타 시트에서 데이터 종류·버전·다운로드 주소를 관리합니다. 이미지를 누르면 원본을 볼 수 있습니다.</figcaption>
</figure>

<details class="pf-details wfk-code">
<summary>코드 보기: CSV 캐시 사용 판단</summary>

```csharp
// ServerCSV_ConvertData.cs: 캐시 판단 부분 발췌 (로그 생략)
if (File.Exists(localFilePath) && localVersion == serverVersion)
{
    try
    {
        fileContent = File.ReadAllText(localFilePath);
        needsDownload = false;
    }
    catch (Exception e)
    {
        needsDownload = true;
    }
}
```
</details>

### 1.2 CSV를 C# 데이터로 변환

헤더 이름을 정규화해 대상 필드와 매핑하고, 각 값을 필드 타입에 맞게 변환합니다. 테이블마다 같은 파싱 흐름을 재사용하도록 구성했습니다.

<figure class="wfk-example">
<a href="/assets/images/World%20First%20Kill/wfk_unit_monster_sheet.png" target="_blank" rel="noopener noreferrer" aria-label="몬스터별 헤더와 능력치가 기록된 원본 데이터 시트 원본 이미지 새 창으로 열기">
<img src="/assets/images/World%20First%20Kill/wfk_unit_monster_sheet.png" width="1400" height="860" alt="몬스터별 헤더와 능력치가 기록된 원본 데이터 시트" loading="lazy">
</a>
<figcaption>몬스터 데이터 시트의 헤더와 행 데이터를 공통 파서로 읽어 C# 객체로 변환합니다.</figcaption>
</figure>

<details class="pf-details wfk-code">
<summary>코드 보기: CSV 헤더와 필드 매핑</summary>

```csharp
// CSVParser.cs: SetupFieldDict 발췌
protected virtual void SetupFieldDict(string[] attribute, FieldInfo[] fields)
{
    Data_Dict.Clear();
    DataIndex_Dict.Clear();
    for (int i = 0; i < attribute.Length; i++)
    {
        string header = attribute[i].Trim().Trim('\uFEFF');
        if (header.Length == 0)
            continue;
        string key = Normalize(header);
        foreach (FieldInfo field in fields)
        {
            if (NormalizeField(field) == key)
            {
                Data_Dict[header] = field.Name;
                DataIndex_Dict[i] = header;
                break;
            }
        }
    }
}
```
</details>

## 2. Seed·Token으로 난수 상태 저장·복원
{: .chapter-title }

맵·퀘스트·스킬의 난수 스트림을 도메인별로 나누고, **기본 시드와 도메인별 소비 횟수**를 저장합니다. 불러올 때 도메인 RNG를 다시 만들고 소비한 횟수만큼 진행시켜 난수 상태를 복원합니다.

<ol class="wfk-flow" aria-label="난수 상태 저장·복원 순서">
    <li><strong>시드 분리</strong><span>기본 시드와 도메인 ID로 개별 RNG 생성</span></li>
    <li><strong>사용량 기록</strong><span>도메인마다 난수 호출 횟수 누적</span></li>
    <li><strong>Token 저장</strong><span>시드와 소비 횟수를 토큰으로 인코딩</span></li>
    <li><strong>상태 복원</strong><span>토큰 해석 후 소비 횟수만큼 Skip</span></li>
</ol>

<table class="wfk-structure">
<thead><tr><th scope="col">클래스</th><th scope="col">책임</th></tr></thead>
<tbody>
<tr><th scope="row"><code>GameSeed</code></th><td>도메인 RNG, 난수 사용량 기록과 복원</td></tr>
<tr><th scope="row"><code>SaveLoadManager</code></th><td>토큰 인코딩·파일 저장·해석 후 복원 요청</td></tr>
</tbody>
</table>

### 2.1 도메인별 난수 분리

기본 시드와 도메인 ID를 혼합해 서브 시드를 만들고, 각각의 RNG를 운용합니다. 대화에서 난수를 더 사용해도 맵의 난수 소비 순서는 유지됩니다.

<details class="pf-details wfk-code">
<summary>코드 보기: 도메인 초기화와 난수 사용량 기록</summary>

```csharp
// GameSeed.cs: Init, NextInt 발췌
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

public static int NextInt(Domain domain, int minInclusive, int maxExclusive)
{
    _domainUsageCounts[domain] = _domainUsageCounts.GetValueOrDefault(domain, 0UL) + 1UL;
    return _domainRng[domain].Next(minInclusive, maxExclusive);
}
```
</details>

### 2.2 Token 저장과 Skip 복원

토큰에 기본 시드와 도메인별 소비 횟수를 저장합니다. 불러올 때 이를 해석해 RNG를 재생하고, 저장된 횟수만큼 Skip합니다. 같은 데이터·생성 로직을 전제로 난수 진행 상태를 복원합니다.

서로 다른 시드에서 생성된 스킬 구성 예시입니다.

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

<details class="pf-details wfk-code">
<summary>코드 보기: 토큰 생성</summary>

`Pad`로 각 숫자를 고정 폭으로 0 채움하고, 시드·도메인 수·도메인별 소비 횟수를 정해진 순서로 이어 붙입니다.

```csharp
// SaveLoadManager.cs: 토큰 규격 상수와 BuildSeedToken 발췌
const int BASE_SEED_WIDTH = 20;
const int COUNT_WIDTH = 3;
const int DOMAIN_ID_WIDTH = 3;
const int USAGE_WIDTH = 12;

static string BuildSeedToken(long baseSeed, Dictionary<GameSeed.Domain, ulong> usage)
{
    ulong encodedSeed = unchecked((ulong)baseSeed);
    string token = Pad(encodedSeed, BASE_SEED_WIDTH);
    List<GameSeed.Domain> domains = new List<GameSeed.Domain>((GameSeed.Domain[])Enum.GetValues(typeof(GameSeed.Domain)));
    domains.Sort((a, b) => ((uint)a).CompareTo((uint)b));
    token += Pad((uint)domains.Count, COUNT_WIDTH);
    foreach (GameSeed.Domain d in domains)
    {
        ulong count = usage != null ? usage.GetValueOrDefault(d, 0UL) : 0UL;
        token += Pad((uint)d, DOMAIN_ID_WIDTH);
        token += Pad(count, USAGE_WIDTH);
    }
    return token;
}
```
</details>

<details class="pf-details wfk-code">
<summary>코드 보기: 토큰 해석과 복원 요청</summary>

`ReadUInt`는 정해진 폭의 숫자를 읽고 읽기 위치를 전진시킵니다. 시드와 도메인별 소비 횟수를 해석한 뒤 `GameSeed.RestoreRNG`를 호출합니다.

```csharp
// SaveLoadManager.cs: RestoreUsageCount 발췌
public static bool RestoreUsageCount(string token)
{
    try
    {
        int idx = 0;
        if (token.Length < BASE_SEED_WIDTH + COUNT_WIDTH)
            return false;
        ulong baseSeedEncoded = ReadUInt(token, ref idx, BASE_SEED_WIDTH);
        long baseSeed = unchecked((long)baseSeedEncoded);
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
```
</details>

<details class="pf-details wfk-code">
<summary>코드 보기: 난수 소비 위치 복원</summary>

```csharp
// GameSeed.cs: RestoreRNG 발췌
public static void RestoreRNG(long baseSeed, Dictionary<Domain, ulong> usageCounts)
{
    foreach (KeyValuePair<Domain, ulong> kv in usageCounts)
    {
        Domain d = kv.Key;
        ulong count = kv.Value;
        System.Random rng = _domainRng[d] = new System.Random(DeriveSubSeed(baseSeed, (uint)d));
        for (ulong i = 0; i < count; i++)
        {
            rng.Next();
        }
        _domainRng[d] = rng;
        _domainUsageCounts[d] = count;
    }
}
```
</details>
