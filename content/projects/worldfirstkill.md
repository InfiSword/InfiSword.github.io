---
title: "PROJECT REPORT // WORLDFIRSTKILL"
excerpt: "Google 스프레드시트 기반 동적 CSV 파이프라인과 결정론적 Seed/Token RNG 완전 복원 시스템"
permalink: "/project/worldfirstkill/"
tags: [Data Architecture, Server/Client, C#, Unity, System Design, RNG Determinism]
---

<style>
    /* --- Premium Report Layout Styles --- */
    
    .chapter-title {
        font-size: 2.0rem;
        color: #fff !important;
        background: #1e293b;
        padding: 22px 30px;
        border-radius: 12px;
        margin-top: 50px !important;
        margin-bottom: 30px !important;
        box-shadow: 0 8px 24px rgba(15, 23, 42, 0.08);
        display: flex;
        align-items: center;
        border: 1px solid #334155 !important;
    }
    .chapter-title::before {
        content: "CHAPTER.";
        font-family: 'Fira Code', monospace;
        font-size: 0.85rem;
        letter-spacing: 2px;
        margin-right: 14px;
        color: #60a5fa;
        opacity: 0.9;
    }

    .pf-visual-frame {
        width: 100%; padding: 30px; background: #ffffff;
        border: 1px solid #e2e8f0; border-radius: 14px;
        margin: 25px 0; text-align: center;
        box-shadow: 0 4px 20px rgba(15, 23, 42, 0.04);
    }
    .pf-arch-diagram {
        display: flex; flex-direction: column; gap: 16px; margin: 20px 0;
    }
    .pf-arch-layer {
        padding: 18px 22px; border: 1px solid #e2e8f0; border-radius: 10px;
        background: #f8fafc; position: relative; text-align: left;
    }
    .pf-arch-layer::before {
        content: ""; position: absolute; left: 0; top: 0; bottom: 0; width: 4px;
        background: #2563eb; border-radius: 10px 0 0 10px;
    }
    .pf-arch-layer-title {
        color: #1e40af; font-weight: 700; margin-bottom: 8px;
        font-family: 'Fira Code', monospace; font-size: 0.9rem;
    }
    .pf-arch-layer-items {
        display: flex; flex-wrap: wrap; gap: 8px; margin-top: 8px;
    }
    .pf-arch-item {
        padding: 5px 12px; background: #ffffff;
        border: 1px solid #cbd5e1; border-radius: 6px;
        font-size: 0.82rem; color: #334155; font-family: 'Fira Code', monospace;
    }

    /* --- Code Details --- */
    details.pf-details {
        margin: 20px 0;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        background: #f8fafc;
        overflow: hidden;
    }
    details.pf-details summary {
        padding: 14px 20px;
        font-weight: 700;
        color: #2563eb;
        cursor: pointer;
        outline: none;
        background: #ffffff;
        display: flex;
        align-items: center;
        border-bottom: 1px solid transparent;
        transition: background 0.15s ease;
    }
    details.pf-details summary:hover {
        background: #f1f5f9;
    }
    details.pf-details[open] summary {
        border-bottom: 1px solid #e2e8f0;
    }

    /* --- Flowchart Elements --- */
    .pf-fc-compact {
        background: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        padding: 16px 20px;
        text-align: left;
        box-shadow: 0 2px 8px rgba(15, 23, 42, 0.03);
    }
    .pf-fc-card-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 6px;
    }
    .pf-fc-title {
        margin: 0;
        font-size: 0.98rem;
        font-weight: 700;
        color: #0f172a;
    }
    .pf-fc-badge {
        font-family: 'Fira Code', monospace;
        font-size: 0.75rem;
        padding: 3px 8px;
        border-radius: 4px;
        background: #eff6ff;
        color: #2563eb;
        border: 1px solid #bfdbfe;
        font-weight: 600;
    }
    .pf-fc-desc {
        margin: 0;
        font-size: 0.87rem;
        color: #475569;
        line-height: 1.55;
    }
    .pf-fc-arrow {
        display: flex;
        justify-content: center;
        align-items: center;
        color: #2563eb;
        margin: 10px 0;
    }
    .pf-fc-pill-start, .pf-fc-pill-end {
        display: inline-flex;
        align-items: center;
        gap: 10px;
        padding: 8px 18px;
        border-radius: 9999px;
        font-size: 0.88rem;
        font-weight: 600;
        border: 1px solid #cbd5e1;
        background: #f8fafc;
        color: #1e293b;
    }
    .pf-fc-mono-tag {
        font-family: 'Fira Code', monospace;
        font-size: 0.75rem;
        padding: 2px 6px;
        border-radius: 4px;
        background: #e2e8f0;
        color: #475569;
    }

    /* --- Token Block Diagram --- */
    .pf-token-block {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 14px;
        margin: 20px 0;
    }
    .pf-token-cell {
        background: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        padding: 16px;
        text-align: left;
        position: relative;
        box-shadow: 0 2px 6px rgba(15, 23, 42, 0.03);
    }
    .pf-token-cell.accent-blue {
        border-color: #93c5fd;
        background: #f8fbff;
    }
    .pf-token-cell.accent-green {
        border-color: #86efac;
        background: #f0fdf4;
    }
    .pf-token-cell-title {
        font-family: 'Fira Code', monospace;
        font-size: 0.82rem;
        font-weight: 700;
        color: #2563eb;
        margin-bottom: 6px;
    }
    .pf-token-cell.accent-green .pf-token-cell-title {
        color: #16a34a;
    }
    .pf-token-cell-spec {
        font-size: 1.15rem;
        font-weight: 800;
        color: #0f172a;
        margin-bottom: 4px;
        font-family: 'Fira Code', monospace;
    }
    .pf-token-cell-desc {
        font-size: 0.82rem;
        color: #64748b;
        margin: 0;
    }

    /* Seed Compare Grid */
    .pf-seed-compare-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
        gap: 16px;
        margin: 20px 0;
    }
    .pf-seed-card {
        background: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        padding: 18px;
        text-align: left;
        box-shadow: 0 2px 6px rgba(15, 23, 42, 0.03);
    }
    .pf-seed-card.seed-a {
        border-color: #93c5fd;
        background: #fcfdff;
    }
    .pf-seed-card.seed-b {
        border-color: #cbd5e1;
        background: #f8fafc;
    }
    .pf-seed-card-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 12px;
        border-bottom: 1px solid #f1f5f9;
        padding-bottom: 8px;
    }
    .pf-seed-title {
        font-family: 'Fira Code', monospace;
        font-size: 0.92rem;
        font-weight: 700;
        color: #0f172a;
        margin: 0;
    }
    .pf-seed-tag {
        font-family: 'Fira Code', monospace;
        font-size: 0.72rem;
        font-weight: 600;
        padding: 2px 8px;
        border-radius: 4px;
        background: #eff6ff;
        color: #2563eb;
        border: 1px solid #bfdbfe;
    }
    .pf-seed-tag.tag-attack {
        background: #fef2f2;
        color: #dc2626;
        border-color: #fecaca;
    }
    .pf-seed-box {
        background: #f8fafc;
        border: 1px solid #e2e8f0;
        border-radius: 6px;
        padding: 10px 14px;
        font-size: 0.84rem;
        line-height: 1.6;
        margin-bottom: 10px;
    }

    /* Comparison Table */
    .pf-table-wrapper {
        width: 100%;
        overflow-x: auto;
        margin: 25px 0;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
    }
    .pf-tech-table {
        width: 100%;
        border-collapse: collapse;
        font-size: 0.9rem;
        text-align: left;
    }
    .pf-tech-table th {
        background: #f8fafc;
        padding: 14px 18px;
        border-bottom: 2px solid #e2e8f0;
        font-weight: 700;
        color: #0f172a;
        font-family: 'Fira Code', monospace;
        font-size: 0.85rem;
    }
    .pf-tech-table td {
        padding: 14px 18px;
        border-bottom: 1px solid #e2e8f0;
        color: #334155;
    }
    .pf-tech-table tr:last-child td {
        border-bottom: none;
    }
    .pf-tech-table tr:hover td {
        background: #f8fafc;
    }
</style>

Google 스프레드시트를 마스터 데이터베이스로 활용한 **서버 연동 CSV 동적 파싱 및 증분 캐싱 파이프라인**과, 로그라이크 게임의 무작위 생성 결과를 100% 보존하는 **결정론적 Seed / Token 상태 복원 시스템**입니다. 기획 밸런스 데이터의 무중단 라이브 패치와 세이브/로드 시 RNG 스트림의 결합 교란을 차단한 핵심 아키텍처를 상세히 소개합니다.

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

---

## 1. 서버 CSV 동기화 및 증분 캐싱 아키텍처
{: .chapter-title }

월퍼킬은 기획 밸런스 수정 시 앱 바이너리를 재빌드하거나 스토어 심사를 거칠 필요 없이, **Google 스프레드시트를 마스터 서버(Single Source of Truth)로 활용하여 클라이언트와 실시간 동기화**합니다. 변경된 파일만 식별해 다운로드하는 **증분 캐싱(Incremental Caching)** 메커니즘을 통해 네트워크 트래픽과 로딩 시간을 최소화했습니다.

### 1.1 마스터 메타 시트 파이프라인 (Master Sheet Pipeline)

시스템의 진입점인 `ServerCSV_ConvertData`는 마스터 구글 시트의 0번 인덱스(`gid=0`) 메타 시트를 우선 다운로드하여 전체 데이터 카탈로그를 파악합니다.

- **메타 시트 4단 열 규격**:
  - `CSVDataType`: 대분류 카테고리 (`Define.CSVDataType` — `Quest`, `Region`, `Item`, `Game`, `Skill`, `Unit`)
  - `Attribute`: 소분류 속성 키 (예: `Weapon`, `Armor`, `Monster`, `Active`, `Passive`, `Dialog` 등 18종)
  - `Version`: 해당 테이블의 버전 식별자 (문자열 — 예: `"1"`, `"4"`, `"7"`)
  - `Link`: 해당 시트의 구글 드라이브 CSV Export URL

<div class="pf-visual-frame" style="text-align: left; padding: 24px; margin: 24px 0;">
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; flex-wrap: wrap; gap: 8px;">
        <div style="font-family: 'Fira Code', monospace; font-size: 0.95rem; font-weight: 700; color: #1e40af; display: flex; align-items: center; gap: 8px;">
            <span style="display: inline-block; width: 8px; height: 8px; border-radius: 50%; background: #2563eb;"></span>
            실제 연동 구글 스프레드시트 마스터 메타 시트 원본 (Live Master Meta Sheet)
        </div>
        <span class="pf-fc-badge">MasterSheetURL (gid=0)</span>
    </div>
    <p style="font-size: 0.9rem; color: #475569; margin: 0 0 18px 0; line-height: 1.6;">
        월퍼킬 클라이언트 코드(<code>ServerCSV_ConvertData.cs</code>)의 <code>MasterSheetURL</code>을 통해 다운로드하는 <strong>실제 구글 스프레드시트 마스터 메타 시트(<code>WFCK_GameConfiguration</code>)</strong> 화면입니다. 18종의 데이터 카탈로그 및 실시간 테이블 버전, 개별 CSV 다운로드 링크가 중앙에서 관리됩니다.
    </p>
    <figure style="margin: 0; border-radius: 8px; overflow: hidden; border: 1px solid #cbd5e1; box-shadow: 0 4px 16px rgba(15, 23, 42, 0.05);">
        <img src="/assets/images/World%20First%20Kill/wfk_master_meta_sheet.png" alt="월퍼킬 마스터 메타 시트 원본 (WFCK_GameConfiguration)" style="width: 100%; height: auto; display: block;" loading="lazy">
        <figcaption style="padding: 9px 14px; background: #f8fafc; border-top: 1px solid #e2e8f0; font-size: 0.8rem; color: #64748b; font-family: 'Fira Code', monospace;">
            ▲ ServerCSV_ConvertData가 최초 다운로드하는 gid=0 메타 시트 (19행: Unit_Monster 테이블 버전 및 개별 다운로드 Link 연동)
        </figcaption>
    </figure>
</div>

<details class="pf-details">
<summary>코드 보기: ServerCSV_ConvertData.cs 메타 시트 다운로드 및 2차원 카탈로그 구축</summary>

```csharp
// ServerCSV_ConvertData.cs: 메타 시트 다운로드 및 2차원 카탈로그 구축
public static async Task<bool> DownloadMetaSheetAsync()
{
    string csvText = await DownloadFileAsync(MasterSheetURL);
    if (string.IsNullOrEmpty(csvText)) return false;

    string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    csvURLDict.Clear();
    _serverVersions.Clear();

    for (int i = 1; i < lines.Length; i++)
    {
        string[] columns = lines[i].Split(',');
        if (columns.Length < 4) continue;

        string csvDataTypeStr = columns[0].Trim();
        string attribute = columns[1].Trim();
        string version = columns[2].Trim();
        string link = columns[3].Trim();

        if (!Enum.TryParse(csvDataTypeStr, out CSVDataType csvDataType)) continue;

        // 2차원 딕셔너리에 URL 및 최신 서버 버전 등록
        csvURLDict.GetOrAdd(csvDataType)[attribute] = link;
        _serverVersions.GetOrAdd(csvDataType)[attribute] = version;
    }
    return true;
}
```
</details>

### 1.2 조건부 증분 다운로드(Conditional Incremental Download) 플로우

모든 CSV를 매번 새로 내려받지 않고, 로컬 캐시 버전과 서버 버전을 대조하여 **변경된 파일만 네트워크로 전송받는 증분 파이프라인**입니다:

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <div class="pf-fc-pill-start">
      <span class="pf-fc-mono-tag">INIT</span>
      <span>DownloadAllCSVAsync() 시작 & LoadLocalVersions() 호출</span>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">1. 버전 대조 (Local vs Server Version Check)</h4>
        <span class="pf-fc-badge">VER_COMPARE</span>
      </div>
      <p class="pf-fc-desc">로컬에 파일이 존재하고 `localVersion == serverVersion` 인지 검사</p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- Branch Grid -->
    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 14px; width: 100%;">
      <div class="pf-fc-compact" style="border-color: #86efac; background: #f0fdf4;">
        <div class="pf-fc-card-header">
          <h4 class="pf-fc-title" style="color: #166534;">[버전 일치] 로컬 캐시 즉시 로드</h4>
          <span class="pf-fc-badge" style="background: #dcfce7; color: #15803d; border-color: #bbf7d0;">CACHE_HIT</span>
        </div>
        <p class="pf-fc-desc">`File.ReadAllText(localFilePath)` 호출. 네트워크 전송 0건, 즉시 로드 (로딩 0ms)</p>
      </div>
      <div class="pf-fc-compact" style="border-color: #93c5fd; background: #f8fbff;">
        <div class="pf-fc-card-header">
          <h4 class="pf-fc-title" style="color: #1e40af;">[버전 불일치/파일 없음] 서버 다운로드</h4>
          <span class="pf-fc-badge">DOWNLOAD</span>
        </div>
        <p class="pf-fc-desc">`UnityWebRequest`로 비동기 다운로드 후 `File.WriteAllText`로 로컬 갱신 및 버전 업데이트</p>
      </div>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <div class="pf-fc-pill-end" style="border-color: #3b82f6; background: #eff6ff; color: #1d4ed8;">
      <span class="pf-fc-mono-tag">COMPLETE</span>
      <span>SaveLocalVersions() 호출로 data_versions.json 저장 및 로드 완료</span>
    </div>

  </div>
</div>

<details class="pf-details">
<summary>코드 보기: ServerCSV_ConvertData 조건부 다운로드 및 로컬 캐싱 루틴</summary>

```csharp
// ServerCSV_ConvertData.cs: 조건부 다운로드 루틴
foreach (var kvp in csvURLDict)
{
    CSVDataType dataType = kvp.Key;
    foreach (var attrKvp in kvp.Value)
    {
        string attribute = attrKvp.Key;
        string url = attrKvp.Value;

        string serverVersion = _serverVersions[dataType][attribute];
        string localVersion = _localVersions.TryGetValue(dataType, out var lAttr) 
            && lAttr.TryGetValue(attribute, out var lVer) ? lVer : "0";

        string fileName = $"{dataType}_{attribute}.csv";
        string localFilePath = Path.Combine(LocalDataPath, fileName);
        bool needsDownload = true;

        // 로컬에 파일이 있고 버전이 동일하면 다운로드 생략 (캐시 적재)
        if (File.Exists(localFilePath) && localVersion == serverVersion)
        {
            try {
                fileContent = File.ReadAllText(localFilePath);
                needsDownload = false;
            } catch {
                needsDownload = true;
            }
        }

        if (needsDownload)
        {
            fileContent = await DownloadFileAsync(url);
            File.WriteAllText(localFilePath, fileContent);
            if (!_localVersions.ContainsKey(dataType)) 
                _localVersions[dataType] = new Dictionary<string, string>();
            _localVersions[dataType][attribute] = serverVersion;
        }

        currentCount++;
        progressCallback?.Invoke((float)currentCount / totalCount);
    }
}
SaveLocalVersions();
```
</details>

---

## 2. 실전 예시로 보는 리플렉션 CSV 파싱 전개 과정
{: .chapter-title }

월퍼킬의 파싱 엔진은 복잡한 내부 코드를 몰라도 **"구글 시트에 적힌 기획 데이터 한 줄이 C# 객체로 어떻게 변환되어 인게임 몬스터에게 도달하는가?"**라는 데이터 흐름으로 직관적으로 이해할 수 있습니다. 기획자가 구글 시트에서 몬스터 체력이나 공격력을 수정할 때마다 프로그래머가 C# 코드를 일일이 고치고 앱을 새로 빌드해야 한다면 라이브 서비스는 불가능합니다. 월퍼킬의 리플렉션 파서는 **"구글 시트에 적힌 텍스트 한 줄을 읽어 게임 속 몬스터에게 살아 숨 쉬는 실제 스탯으로 부여하는 자동 변환 파이프라인"**입니다. 18종의 데이터 테이블마다 수동 코드를 만들지 않고도 범용적으로 동작하는 실제 과정을 `Unit_Monster.csv`의 첫 번째 몬스터인 **'덤불 골렘'** 데이터를 예시로 한 단계씩 살펴봅니다.

<div class="pf-visual-frame" style="text-align: left; padding: 24px; margin: 24px 0;">
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; flex-wrap: wrap; gap: 8px;">
        <div style="font-family: 'Fira Code', monospace; font-size: 0.95rem; font-weight: 700; color: #1e40af; display: flex; align-items: center; gap: 8px;">
            <span style="display: inline-block; width: 8px; height: 8px; border-radius: 50%; background: #2563eb;"></span>
            실제 구글 스프레드시트 몬스터 기획 데이터 시트 원본 (WFCK_Unit)
        </div>
        <span class="pf-fc-badge">DATA TABLE SHEET</span>
    </div>
    <p style="font-size: 0.9rem; color: #475569; margin: 0 0 16px 0; line-height: 1.6;">
        마스터 메타 시트의 개별 링크를 통해 다운로드되는 <strong>실제 몬스터 기획 데이터 시트(<code>WFCK_Unit</code>)</strong> 원본 화면입니다. 1행(Index 0)의 <code>덤불 골렘</code>에 정의된 27개 기획 필드가 아래 4단계 리플렉션 파싱 과정을 거쳐 <strong>C# 데이터 객체(<code>Data_Monster</code>)</strong>로 변환되며, 게임 속 덤불 골렘에게 실제 스탯(체력·공격력·이동속도 등)을 부여하게 됩니다.
    </p>
    <figure style="margin: 0; border-radius: 8px; overflow: hidden; border: 1px solid #cbd5e1; box-shadow: 0 4px 16px rgba(15, 23, 42, 0.05);">
        <img src="/assets/images/World%20First%20Kill/wfk_unit_monster_sheet.png" alt="월퍼킬 몬스터 기획 시트 원본 (WFCK_Unit - Unit_Monster.csv)" style="width: 100%; height: auto; display: block;" loading="lazy">
        <figcaption style="padding: 9px 14px; background: #f8fafc; border-top: 1px solid #e2e8f0; font-size: 0.8rem; color: #64748b; font-family: 'Fira Code', monospace;">
            ▲ 실제 구글 시트 1행: Index 0 '덤불 골렘'의 27개 기획 스탯 컬럼 (C# 리플렉션 파서로 주입되는 원시 데이터)
        </figcaption>
    </figure>
</div>

### 2.1 실제 몬스터 데이터 파싱 4단계 전개 (Execution Walkthrough)

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- Raw CSV Input Stage -->
    <div class="pf-fc-pill-start">
      <span class="pf-fc-mono-tag">INPUT DATA</span>
      <span>Unit_Monster.csv 원시 데이터 (1행: 덤불 골렘)</span>
    </div>

    <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 14px 18px; margin: 10px 0; font-family: 'Fira Code', monospace; font-size: 0.82rem; text-align: left; overflow-x: auto; color: #0f172a; line-height: 1.6;">
      <div style="color: #64748b; margin-bottom: 4px;">// CSV 1행: Header (시트의 기획 스탯 열 이름)</div>
      <div style="color: #2563eb; font-weight: 600;">Index, Name, Description, Target_Team, Target_Enemy, WarningDis, DropItem, SkillNum1, SkillNum2, MoveSpeed, AttackSkill, AD_Damage, ...</div>
      <div style="color: #64748b; margin: 8px 0 4px 0;">// CSV 2행: Data Row (덤불 골렘 원시 데이터)</div>
      <div style="color: #0f172a;">0, 덤불 골렘, "갓 깨어난 자연의 정령으로, 주변의 풀과 덤불의 기운을 받아 태어났다.", 0, 3, 0, 1501001, -1, -1, 0.75, 0, 10, ...</div>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 01 -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">[틀 맞추기] C# 데이터 변수와 시트 열(Header) 1:1 자동 대응시키기(SetupFieldDict)</h4>
        <span class="pf-fc-badge">REFLECTION</span>
      </div>
      <p class="pf-fc-desc">
        리플렉션(Reflection) 기술을 통해 C# <code>Data_Monster</code> 클래스에 어떤 변수들이 선언되어 있는지 스스로 읽어옵니다. 그런 다음 시트 첫 행의 열 이름(<code>Index</code>, <code>Name</code>, <code>AD_Damage</code> 등)과 C# 변수명을 1:1로 짝지어 <strong>"어떤 열의 값을 어떤 C# 변수에 넣어야 하는지"</strong>를 안내하는 매핑 사전을 자동으로 구축합니다.
      </p>
      <div style="margin-top: 10px; display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 8px; font-family: 'Fira Code', monospace; font-size: 0.8rem;">
        <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px;">
          <span style="color: #2563eb;">[0] Index</span> ➔ <strong>m_monsterIndex</strong>
        </div>
        <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px;">
          <span style="color: #2563eb;">[1] Name</span> ➔ <strong>m_monsterName</strong>
        </div>
        <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px;">
          <span style="color: #2563eb;">[2] Description</span> ➔ <strong>m_description</strong>
        </div>
        <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px;">
          <span style="color: #2563eb;">[6] DropItem</span> ➔ <strong>m_dropItem</strong>
        </div>
        <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px;">
          <span style="color: #2563eb;">[9] MoveSpeed</span> ➔ <strong>m_addMoveSpeed (float)</strong>
        </div>
        <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px;">
          <span style="color: #2563eb;">[11] AD_Damage</span> ➔ <strong>m_ad_Damage</strong>
        </div>
      </div>

      <details class="pf-details" style="margin-top: 14px;">
        <summary>코드 보기: SetupFieldDict() 1:1 매핑 로직 및 Data_Monster 클래스 선언</summary>

```csharp
// CSVParser.cs: 리플렉션으로 FieldInfo[] 수집 및 SetupFieldDict 1:1 매핑 실행
Type myType = typeof(T); // typeof(Data_Monster)
FieldInfo[] myFieldInfo = myType.GetFields(
    BindingFlags.NonPublic | BindingFlags.Instance | 
    BindingFlags.Public | BindingFlags.Static
);

string[] attributeColumnArray = csvIndexData[0].Trim().Split(',');
SetupFieldDict(attributeColumnArray, myFieldInfo);

// SetupFieldDict() 구현: CSV 열 순서와 C# 필드명을 1:1 바인딩하는 매핑 사전 구축
protected virtual void SetupFieldDict(string[] attribute, FieldInfo[] fields)
{
    Data_Dict.Clear();
    DataIndex_Dict.Clear();

    for (int i = 0; i < attribute.Length; i++)
    {
        string header = attribute[i].Trim();
        Data_Dict[header] = fields[i].Name; // 예: Data_Dict["Name"] = "m_monsterName"
        DataIndex_Dict[i] = header;        // 예: DataIndex_Dict[1] = "Name"
    }
}
```

```csharp
// Data_Monster.cs: CSV 헤더 열 순서와 1:1로 매핑되는 C# DTO 필드 선언 구조
public class Data_Monster : BaseData
{
    public int m_monsterIndex;      // [0] Index
    public string m_monsterName;    // [1] Name
    public string m_description;    // [2] Description
    public int m_target_Team;       // [3] Target_Team
    public int m_target_Enemy;      // [4] Target_Enemy
    public int m_WarningDis;        // [5] WarningDis
    public int m_dropItem;          // [6] DropItem
    public int m_skillNum1;         // [7] SkillNum1
    public int m_skillNum2;         // [8] SkillNum2
    public float m_addMoveSpeed;    // [9] MoveSpeed (float)
    public int m_attackSkill;       // [10] AttackSkill
    public int m_ad_Damage;         // [11] AD_Damage
    // ... 총 27개 기획 스탯 필드
}
```
      </details>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 02 -->
    <div class="pf-fc-card pf-fc-compact">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title">STEP 2. [안전하게 쪼개기] 쉼표가 섞여도 안심하는 정규식 스마트 분할 (SmartSplit)</h4>
        <span class="pf-fc-badge">SMART_SPLIT</span>
      </div>
      <p class="pf-fc-desc">
        CSV 파일은 쉼표(<code>,</code>)로 열을 구분하지만, 덤불 골렘의 설명처럼 <em>"갓 깨어난 자연의 정령으로, 주변의 풀과..."</em>와 같이 글 안에 쉼표가 들어가 있으면 일반적인 자르기(<code>Split(',')</code>)로는 데이터 열 순서가 엉망으로 밀려버립니다. 이를 방지하기 위해 따옴표 속 쉼표는 문장으로 안전하게 보호하고, 데이터 구분용 쉼표만 정확히 골라내어 27개의 스탯 값을 오차 없이 안전하게 분리합니다.
      </p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 03 -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #93c5fd; background: #f8fbff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1e40af;">STEP 3. [스탯 담기] C# 몬스터 데이터 객체 생성 및 스탯 자동 주입 (Binding)</h4>
        <span class="pf-fc-badge">INJECTION</span>
      </div>
      <p class="pf-fc-desc">
        스탯을 담아둘 빈 <code>Data_Monster</code> 객체(C# 데이터 바구니)를 메모리에 새로 생성합니다. 그리고 시트에서 읽어온 문자열을 각 변수 타입에 맞춰(이름은 <code>string</code>, 공격력은 <code>int</code> 정수, 이동속도는 <code>float</code> 소수점) 자동 변환하여 덤불 골렘의 각 변수 칸에 차곡차곡 채워 넣습니다.
      </p>
      <div style="margin-top: 10px; background: #ffffff; border: 1px solid #cbd5e1; border-radius: 6px; padding: 10px 14px; font-family: 'Fira Code', monospace; font-size: 0.82rem; text-align: left; color: #0f172a; line-height: 1.6;">
        <div>monster.m_monsterIndex = <strong>0</strong> <span style="color: #64748b;">(정수 int 자동 변환)</span></div>
        <div>monster.m_monsterName = <strong>"덤불 골렘"</strong> <span style="color: #64748b;">(문자열 string)</span></div>
        <div>monster.m_description = <strong>"갓 깨어난 자연의 정령으로, 주변의 풀과 덤불의 기운을 받아 태어났다."</strong> <span style="color: #64748b;">(문자열 string)</span></div>
        <div>monster.m_dropItem = <strong>1501001</strong>, monster.m_target_Enemy = <strong>3</strong> <span style="color: #64748b;">(정수 int)</span></div>
        <div>monster.m_addMoveSpeed = <strong>0.75f</strong> <span style="color: #64748b;">(실수 float 자동 변환)</span></div>
        <div>monster.m_ad_Damage = <strong>10</strong>, monster.m_ground_Damage = <strong>10</strong> <span style="color: #64748b;">(정수 int)</span></div>
      </div>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"></line><polyline points="19 12 12 19 5 12"></polyline></svg>
    </div>

    <!-- STEP 04 -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #86efac; background: #f0fdf4;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #15803d;">STEP 4. [인게임 보관] DataManager 등록으로 소환 시 즉시 꺼내쓰기(ServerDataLoad)</h4>
        <span class="pf-fc-badge" style="background: #dcfce7; color: #166534; border-color: #bbf7d0;">CACHED O(1)</span>
      </div>
      <p class="pf-fc-desc">
        모든 스탯이 채워진 덤불 골렘 데이터 객체를 고유 식별 번호(<code>m_monsterIndex = 0</code>)와 함께 중앙 관리자인 <code>DataManager</code> 보관함에 등록합니다. 이제 필드에 덤불 골렘이 스폰되거나 전투가 일어날 때마다 <code>0</code>번 키값 하나로 0.0001초 만에(O(1)) 완벽한 스탯을 꺼내 몬스터에게 즉시 부여할 수 있게 됩니다.
      </p>
    </div>

  </div>
</div>

---

## 3. 결정론적 난수 생성 및 Seed / Token 상태 복원 시스템
{: .chapter-title }

로그라이크와 절차적 생성(Procedural Generation) 장르에서는 게임을 저장하고 불러왔을 때 **현재 월드뿐만 아니라 앞으로 생성될 모든 미래 확률 상태(RNG Stream)까지 세이브 시점과 100% 동일하게 복원**되어야 합니다. 월퍼킬은 대용량 JSON 직렬화의 한계를 극복하고, 마인크래프트의 월드 시드 메커니즘에서 착안하여 **도메인 격리 + 고정 폭 토큰 인코딩 + Skip 루프 복원** 아키텍처를 독자적으로 구축했습니다.

### 3.1 마인크래프트에서 착안한 발상 — 대용량 JSON 저장의 한계와 시드 압축

로그라이크 RPG는 던전 지형, 몬스터 스폰, 퀘스트 목록, 플레이어 클래스 및 스킬 조합 등 매 판 수많은 요소가 절차적으로 생성됩니다. 초기 설계 단계에서 해당 방대한 데이터를 직렬화/역직렬화로 저장하고 로드할 때 비효율적인 문제에 직면했으며, 이를 해결하기 위해 Seed 방식으로 맵을 생성하는 마인크래프트 게임에서 영감을 받아 시스템을 구현하고자 하였습니다.


<div class="pf-visual-frame" style="text-align: left; padding: 24px; margin: 24px 0;">
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; flex-wrap: wrap; gap: 8px;">
        <div style="font-family: 'Fira Code', monospace; font-size: 0.95rem; font-weight: 700; color: #1e40af; display: flex; align-items: center; gap: 8px;">
            <span style="display: inline-block; width: 8px; height: 8px; border-radius: 50%; background: #2563eb;"></span>
            마인크래프트의 결정론적 월드 생성 시드 메커니즘 (Minecraft World Seed)
        </div>
        <span class="pf-fc-badge">PROCEDURAL GENERATION</span>
    </div>
    <p style="font-size: 0.9rem; color: #475569; margin: 0 0 16px 0; line-height: 1.6;">
        마인크래프트는 수 기가바이트에 달하는 3D 복셀(Voxel) 블록 좌표를 일일이 파일에 저장하지 않습니다. 오직 <strong>단 하나의 시드(Seed) 숫자</strong>를 의사 난수 생성기(PRNG)에 전달하여 산맥, 동굴, 바이옴을 동일하게 재현합니다. 월퍼킬은 이 원리를 절차적 RPG에 적용하여, 방대한 게임 객체를 직렬화하는 대신 <strong>초기 시드 번호와 난수 소비 진행도만으로 월드와 스킬을 100% 동일하게 복원</strong>하는 초경량 상태 복원 엔진을 설계했습니다.
    </p>
    <figure style="margin: 0; border-radius: 8px; overflow: hidden; border: 1px solid #cbd5e1; box-shadow: 0 4px 16px rgba(15, 23, 42, 0.05);">
        <img src="/assets/images/World%20First%20Kill/extracted_world_first_kill_slide29_shape45_Seed_Example.png" alt="마인크래프트 월드 생성 시드(Seed for the World Generator) 입력 화면 예시" style="width: 100%; height: auto; display: block;" loading="lazy">
        <figcaption style="padding: 9px 14px; background: #f8fafc; border-top: 1px solid #e2e8f0; font-size: 0.8rem; color: #64748b; font-family: 'Fira Code', monospace;">
            ▲ 마인크래프트의 시드 입력 화면: 동일한 시드 값을 입력하면 언제나 동일한 월드 지형과 규칙이 결정론적으로 재현됨
        </figcaption>
    </figure>
</div>

#### 실시간 로그라이크 RPG에서의 핵심 딜레마와 해결
마인크래프트는 정적 지형을 사전에 일괄 생성하지만, 월퍼킬은 플레이어가 이동하고 대화하며 전투 스킬을 획득하는 과정에서 난수(RNG)가 실시간으로 계속 소비됩니다.
단순히 초기 시드값(`BaseSeed`)만 저장하면, 게임을 불러왔을 때 난수 발생기 포인터가 0으로 초기화되어 **세이브 이후 생성될 미래의 스킬, 보상, 퀘스트가 세이브 이전의 원래 세계선과 영구히 어긋나는 문제**가 발생합니다.

월퍼킬은 이를 해결하기 위해 **루트 시드(`BaseSeed`)**와 각 시스템별 **누적 난수 소비 횟수(`Usage Count`)**를 결합하여 압축하는 **고정 폭 토큰(Token) 시스템**을 구축했습니다.

---

### 3.2 도메인 격리 구조 및 SplitMix64 해싱 아키텍처

전역 `System.Random` 인스턴스를 하나만 공유하면 치명적인 문제가 발생합니다. 예를 들어 대화(Dialog)나 퀘스트에서 난수를 1회 소비하는 순간, 이후 생성되는 맵 타일이나 보상 아이템 드롭 결과가 완전히 뒤틀리게 됩니다(**결합 교란, Coupling Interference**). 이를 차단하기 위해 `GameSeed`는 목적별로 완벽히 독립된 서브 난수 스트림을 운영합니다.

<details class="pf-details">
<summary>코드 보기: GameSeed.cs 목적별 서브 스트림 도메인 열거형 (Domain)</summary>

```csharp
// GameSeed.cs: 목적별 서브 스트림 도메인 격리
public enum Domain : uint
{
    Map = 1, Region = 2, Field = 3, Quest = 4, Dialog = 5, Reward = 6, Class = 7,
    Class_Melee = 100, Class_Dexterity = 101, Class_Magic = 102, Class_Support = 103,
    Skill_Melee = 200, Skill_Dexterity = 201, Skill_Magic = 202, Skill_Support = 203,
}
```
</details>

#### 왜 단순 `+1` 가산이 아닌, PRNG 기반 SplitMix64를 채택했는가?

각 시스템(맵, 퀘스트, 스킬 등)마다 독립된 시드를 부여할 때, 가장 직관적인 방법은 기본 시드에 번호를 단순히 더하는 방식(`baseSeed + 1`, `baseSeed + domainId`)입니다. 하지만 절차적 생성 게임에서는 이 단순한 덧셈이 치명적인 문제를 초래할 수 있습니다.

컴퓨터의 난수 생성기(`System.Random`)는 서로 인접한 시드값(예: 100과 101)을 주입받으면 초기에 생성되는 난수 패턴 역시 서로 닮은 궤적을 그리는 성질(선형 상관관계)이 있습니다. 이로 인해 맵 생성 난수와 퀘스트 난수가 서로 맞물려, 특정 지형이 생성될 때마다 항상 동일한 퀘스트나 보상이 함께 등장하는 식의 **원치 않는 패턴 동기화 문제**가 있었습니다.

이러한 문제를 해결하기 위해서 난수 알고리즘인 **SplitMix64 비트 믹싱 기법**을 채택하였습니다.

<h4 style="font-size: 1.15rem; font-weight: 700; color: #0f172a; margin: 28px 0 12px 0;">SplitMix64 알고리즘의 원리와 도입 이점</h4>
SplitMix64는 자바(Java 8)나 고성능 난수 엔진에서 널리 검증된 64비트 비트 믹싱 알고리즘입니다. 숫자를 믹서기에 넣고 갈아버리듯, 비트들을 좌우로 밀고(Shift) 겹쳐가며(XOR) 거대한 소수(Prime Number)를 곱해 원래 숫자의 규칙성을 완전히 파괴합니다.

- **완벽한 독립성 (눈사태 효과, Avalanche Effect)**: 시스템 식별 번호가 `1`과 `2`처럼 바로 옆에 붙어 있는 숫자라도, 비트 믹싱을 거치면 결과 비트의 약 50%가 완전히 뒤바뀝니다. 따라서 맵 시드와 퀘스트 시드가 1 차이밖에 나지 않아도 서로 전혀 다른 무작위 세계를 만들어냅니다.
- **시스템 간 상호 간섭 원천 차단**: 맵 시스템에서 난수를 수백 번 소비하더라도 퀘스트나 전투 스킬의 생성 결과에는 단 1%의 영향도 미치지 않는 독립된 난수 스트림을 보장합니다.
- **메모리 낭비 없는 초고속 연산 (Zero-Allocation)**: 추가적인 객체 생성(GC 할당) 없이 순수 CPU 레지스터 안에서 비트 연산만으로 1~2나노초(10억 분의 1초) 만에 즉시 계산되어 게임 실행 속도에 전혀 영향을 주지 않습니다.

#### SplitMix64 기반 서브 시드 파생 파이프라인 (`DeriveSubSeed`)

루트 시드(`baseSeed`)와 시스템 고유 번호(`salt`)를 결합하여, 시스템 간 비트 상관관계를 완벽히 차단한 32비트 양수 시드를 만들어내는 4단계 파이프라인입니다:

<details class="pf-details">
<summary>코드 보기: GameSeed.cs SplitMix64 기반 서브 시드 유도 함수 (DeriveSubSeed)</summary>

```csharp
// GameSeed.cs: SplitMix64 기반 서브 시드 유도 함수
private static int DeriveSubSeed(int baseSeed, uint salt)
{
    unchecked
    {
        // STEP 1 & 2. 64비트 황금비 상수 가산 및 도메인 솔트 소수 승산
        ulong z = (ulong)((uint)baseSeed) + 0x9E3779B97F4A7C15UL + (ulong)((uint)salt * 0x85EBCA6B);

        // STEP 3. David Stafford Mix13 다단계 XOR-Shift & 64-bit 소수 승산
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        z = z ^ (z >> 31);

        // STEP 4. 부호 비트 소거를 통한 32-bit 양수 시드 보장
        return (int)(z & 0x7FFFFFFF);
    }
}
```
</details>

- **4단계 비트 믹싱 파이프라인의 쉬운 동작 원리**:
  1. **STEP 1. 황금비 상수 더하기 (규칙적인 패턴 깨뜨리기)**: 수학에서 숫자를 가장 고르게 흩뜨려 놓는 '황금비율(Fibonacci)' 기반 상수(`0x9E3779B9...`)를 더해, 시드 번호가 가진 일정한 간격이나 연속적인 패턴을 1차적으로 깨뜨립니다.
  2. **STEP 2. 도메인 고유 번호 흩뿌리기 (큰 소수 곱하기)**: 맵, 퀘스트, 스킬 등 각 시스템의 고유 식별 번호(`salt`)에 거대한 홀수 소수(`0x85EBCA6B`)를 곱합니다. 1, 2, 3처럼 작은 번호들이 순식간에 수십억 단위의 거대한 숫자로 뻥튀기되면서 비트가 전체 자릿수로 넓게 퍼져나갑니다.
  3. **STEP 3. 비트 반으로 쪼개서 뒤섞기 (XOR-Shift 믹싱)**: 숫자의 앞부분 비트와 뒷부분 비트를 좌우로 밀고 겹쳐가며(XOR), 거대한 64비트 소수들을 곱해 골고루 반죽하듯 섞어줍니다. 이 단계를 거치면 원래 숫자가 단 1비트만 달라도 결과는 완전히 다른 무작위 숫자로 뒤바뀝니다.
  4. **STEP 4. 안전한 양수 32비트 숫자로 완성 (부호 비트 정리)**: 유니티 C#의 기본 난수 생성기(`System.Random`)는 음수 시드가 들어오면 예기치 못한 오작동이 발생할 수 있습니다. 따라서 맨 앞자리 부호 비트를 0으로 고정(`& 0x7FFFFFFF`)하여 항상 안전하고 안정적인 양수 시드로 완성합니다.

#### 세이브 / 로드 2단계 파이프라인
`SaveLoadManager`와 `GameSeed` 간의 세이브 및 복원은 명확한 책임 분리로 수행됩니다:

<div class="pf-visual-frame pf-flowchart-frame">
  <div class="pf-flowchart">

    <!-- SAVE ROW -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #bfdbfe; background: #f8fbff;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #1e40af;">[SAVE] SaveSeedTokenToJson()</h4>
        <span class="pf-fc-badge">SERIALIZE</span>
      </div>
      <p class="pf-fc-desc">
        <code>GameSeed.GetUsageSnapshot()</code>으로 도메인별 소비량 스냅샷 추출 ➔ <code>BuildSeedToken()</code>으로 단일 토큰 압축 생성 ➔ <code>seed_state.json</code> 로컬 저장
      </p>
    </div>

    <div class="pf-fc-arrow">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
    </div>

    <!-- LOAD ROW -->
    <div class="pf-fc-card pf-fc-compact" style="border-color: #86efac; background: #f0fdf4;">
      <div class="pf-fc-card-header">
        <h4 class="pf-fc-title" style="color: #15803d;">[LOAD] TryLoadSeedToken() &amp; RestoreFromSnapshot()</h4>
        <span class="pf-fc-badge" style="background: #dcfce7; color: #166534; border-color: #bbf7d0;">DESERIALIZE</span>
      </div>
      <p class="pf-fc-desc">
        JSON에서 토큰 문자열 로드 ➔ <code>RestoreSeedToken()</code>으로 <code>baseSeed</code> 및 도메인별 누적 카운트 복원 ➔ Skip 루프로 RNG 상태를 100% 동일하게 복구
      </p>
    </div>

  </div>
</div>

---

### 3.3 DATA ENCODING MAP — 고정 폭 0-패딩 토큰 규격 및 JSON 구조

각 도메인별 누적 소비 횟수를 쉼표나 콜론 같은 구분 기호(Delimiter) 없이 파싱할 수 있도록 **고정 폭(Fixed-Width) 0-패딩 문자열 규격**으로 직렬화합니다:

<div class="pf-visual-frame">
  <div style="text-align: left; margin-bottom: 12px;">
    <span class="pf-fc-mono-tag" style="background: #2563eb; color: #ffffff; font-weight: 700;">DATA ENCODING MAP</span>
    <span style="font-size: 0.85rem; color: #64748b; margin-left: 8px;">실측 시드(1826533749) 기준 토큰 세그먼트 분할 맵</span>
  </div>

  <div class="pf-token-block">
    <div class="pf-token-cell accent-blue">
      <div class="pf-token-cell-title">[ BASE SEED ]</div>
      <div class="pf-token-cell-spec">00000000001826533749</div>
      <p class="pf-token-cell-desc"><strong>20자리 고정 폭</strong>: 맵과 규칙을 생성하는 64비트 고유 Seed 번호 (0-패딩)</p>
    </div>
    <div class="pf-token-cell accent-blue">
      <div class="pf-token-cell-title">[ COUNT ]</div>
      <div class="pf-token-cell-spec">014</div>
      <p class="pf-token-cell-desc"><strong>3자리 고정 폭</strong>: 저장된 총 도메인 개수 (Map, Quest, Skill 등 총 14개)</p>
    </div>
    <div class="pf-token-cell accent-green">
      <div class="pf-token-cell-title">[ DOMAIN ID ]</div>
      <div class="pf-token-cell-spec">001</div>
      <p class="pf-token-cell-desc"><strong>3자리 고정 폭</strong>: 대상 서브 시스템 고유 식별자 Enum 값 (Map = 1)</p>
    </div>
    <div class="pf-token-cell accent-green">
      <div class="pf-token-cell-title">[ USAGE PROGRESS ]</div>
      <div class="pf-token-cell-spec">000000000042</div>
      <p class="pf-token-cell-desc"><strong>12자리 고정 폭</strong>: 해당 시스템의 누적 난수 소비 횟수 (최대 1조 회 지원)</p>
    </div>
  </div>

  <div style="background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 12px 16px; margin-top: 14px; text-align: left;">
    <div style="font-family: 'Fira Code', monospace; font-size: 0.78rem; color: #64748b; margin-bottom: 6px;">// 실제 로컬에 저장되는 seed_state.json 구조</div>
    <div style="background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 10px 14px; font-family: 'Fira Code', monospace; font-size: 0.82rem; color: #0f172a; overflow-x: auto; line-height: 1.6;">
      <div>{</div>
      <div style="padding-left: 16px;">&quot;baseSeed&quot;: 1826533749,</div>
      <div style="padding-left: 16px;">&quot;seedToken&quot;: &quot;00000000001826533749014001000000000042...&quot;,</div>
      <div style="padding-left: 16px;">&quot;updatedAt&quot;: &quot;2026-05-19 21:20:54&quot;</div>
      <div>}</div>
    </div>
  </div>

  <p style="margin: 12px 0 0 0; font-size: 0.84rem; color: #64748b; text-align: left;">
    전체 토큰 구조: <code>[BaseSeed: 20자][Count: 3자] + { [DomainID: 3자][Usage: 12자] } &#42; N</code>
  </p>
</div>

- **고정 폭 토큰 인코딩의 핵심 장점**:
  1. **대용량 JSON 직렬화 오버헤드 원천 차단**: 수많은 게임 객체를 저장하지 않고 단 1줄의 고정 폭 문자열 토큰으로 압축하여 저장 용량을 수백 분의 1로 경감.
  2. **무할당 고속 파싱(Zero-Allocation O(1))**: 쉼표나 콜론 같은 구분 기호가 없어 문자열 분할 배열 생성 없이 `Substring(idx, width)` 오프셋 계산만으로 즉시 디코딩.
  3. **64비트 정수 보장**: 20자리 고정 폭으로 최대 64비트 시드값을 지원하여 대규모 절차적 생성에서도 시드 충돌 및 오버플로우 방지.

---

### 3.4 Skip 루프 복원 메커니즘과 실제 인게임 스킬셋 복원 실증

세이브 파일을 불러올 때, 토큰에서 추출된 각 도메인의 누적 소비 횟수(`count`)만큼 `rng.Next()`를 고속 더미 호출(Skip)하여 난수 엔진의 내부 포인터를 세이브 시점으로 100% 동일하게 강제 이동시킵니다:

<details class="pf-details">
<summary>코드 보기: GameSeed.cs 과거 난수 소비 지점으로 스트림 강제 롤백 (RestoreFromSnapshot)</summary>

```csharp
// GameSeed.cs: 과거 난수 소비 지점으로 스트림 강제 롤백
public static void RestoreFromSnapshot(int baseSeed, Dictionary<Domain, ulong> usageCounts)
{
    Init(baseSeed); // 서브 시드로 전체 도메인 재초기화

    foreach (var kv in usageCounts)
    {
        Domain d = kv.Key;
        ulong count = kv.Value;
        System.Random rng = new System.Random(DeriveSubSeed(baseSeed, (uint)d));

        // 저장 시점까지 소비되었던 횟수만큼 난수를 버림(Skip) 호출
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

#### 실제 인게임 실증: 시드값에 따른 스킬셋 절차적 생성 결과 비교
월퍼킬의 플레이어 스킬은 시드와 난수 스트림에 의해 결정론적으로 판정됩니다. 따라서 **개발자가 개별 스킬 객체를 세이브 데이터에 일일이 기록하지 않아도, 단 1줄의 `seedToken` 해석만으로 동일한 스킬셋이 100% 오차 없이 복원**됩니다:

<div class="pf-seed-compare-grid">

  <!-- SEED A -->
  <div class="pf-seed-card seed-a">
    <div class="pf-seed-card-header">
      <h4 class="pf-seed-title">SEED A (1459589277)</h4>
      <span class="pf-seed-tag">버프형 지원 스킬셋</span>
    </div>
    
    <div class="pf-seed-box">
      <div style="font-weight: 700; color: #1e40af; margin-bottom: 4px;">절차적 생성 스킬셋:</div>
      <div style="color: #334155; margin-bottom: 8px;">• 아군 대상 버프 스킬 조합 자동 구성</div>
      <div style="background: #ffffff; border: 1px solid #cbd5e1; border-radius: 4px; padding: 6px 10px; font-size: 0.8rem; color: #0f172a; margin-bottom: 10px;">
        <strong>[스킬 효과]</strong> "아군에게 스킬 사용 시 아군의 치명타 확률과 치명타 데미지를 증가시킵니다."
      </div>
      <figure style="margin: 0; border-radius: 6px; overflow: hidden; border: 1px solid #bfdbfe; box-shadow: 0 2px 8px rgba(37, 99, 235, 0.08);">
        <img src="/assets/images/World%20First%20Kill/스크린샷%202026-05-19%20212720.png" alt="SEED A 인게임 스킬 툴팁 (치명타 버프 스킬셋)" style="width: 100%; height: auto; display: block;" loading="lazy">
        <figcaption style="padding: 6px 10px; background: #eff6ff; border-top: 1px solid #bfdbfe; font-size: 0.74rem; color: #1e40af; font-family: 'Fira Code', monospace;">
          ▲ SEED A 인게임 실제 생성 스킬: 아군 치명타 버프 스킬셋
        </figcaption>
      </figure>
    </div>

    <div style="font-family: 'Fira Code', monospace; font-size: 0.78rem; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px; color: #475569;">
      <div>"baseSeed": 1459589277,</div>
      <div>"seedToken": "00000000001459589277014001...26"</div>
    </div>
  </div>

  <!-- SEED B -->
  <div class="pf-seed-card seed-b">
    <div class="pf-seed-card-header">
      <h4 class="pf-seed-title">SEED B (1459589276)</h4>
      <span class="pf-seed-tag tag-attack">물리 공격형 액티브 스킬셋</span>
    </div>
    
    <div class="pf-seed-box">
      <div style="font-weight: 700; color: #991b1b; margin-bottom: 4px;">절차적 생성 스킬셋:</div>
      <div style="color: #334155; margin-bottom: 8px;">• 단일 타겟 물리 타격 스킬 조합 자동 구성</div>
      <div style="background: #ffffff; border: 1px solid #cbd5e1; border-radius: 4px; padding: 6px 10px; font-size: 0.8rem; color: #0f172a; margin-bottom: 10px;">
        <strong>[강타]</strong> "하나의 적에게 120%의 AD 데미지를 입힌다."
      </div>
      <figure style="margin: 0; border-radius: 6px; overflow: hidden; border: 1px solid #fecaca; box-shadow: 0 2px 8px rgba(220, 38, 38, 0.08);">
        <img src="/assets/images/World%20First%20Kill/스크린샷%202026-05-19%20212644.png" alt="SEED B 인게임 스킬 툴팁 (120% AD 강타 스킬셋)" style="width: 100%; height: auto; display: block;" loading="lazy">
        <figcaption style="padding: 6px 10px; background: #fef2f2; border-top: 1px solid #fecaca; font-size: 0.74rem; color: #991b1b; font-family: 'Fira Code', monospace;">
          ▲ SEED B 인게임 실제 생성 스킬: 단일 대상 120% AD 강타 스킬셋
        </figcaption>
      </figure>
    </div>

    <div style="font-family: 'Fira Code', monospace; font-size: 0.78rem; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px 10px; color: #475569;">
      <div>"baseSeed": 1459589276,</div>
      <div>"seedToken": "00000000001459589276014001...27"</div>
    </div>
  </div>

</div>

<p style="font-size: 0.88rem; color: #334155; line-height: 1.6; margin-top: 10px;">
  <strong>실증 요약:</strong> 시드 번호 단 1의 차이(<code>...77</code> vs <code>...76</code>)만으로도 완전히 다른 직업 스킬셋(치명타 버프 vs 120% AD 물리 강타)이 수학적으로 유도되며, 저장 후 재로드 시에도 과거 누적 소비 지점(<code>...26</code> vs <code>...27</code>)으로 정확히 복원되어 인게임 런타임의 결정론적 무결성을 완벽히 증명했습니다.
</p>

<div class="pf-visual-frame" style="text-align: left; padding: 24px; margin: 24px 0;">
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; flex-wrap: wrap; gap: 8px;">
        <div style="font-family: 'Fira Code', monospace; font-size: 0.95rem; font-weight: 700; color: #1e40af; display: flex; align-items: center; gap: 8px;">
            <span style="display: inline-block; width: 8px; height: 8px; border-radius: 50%; background: #2563eb;"></span>
            포트폴리오 원본: Seed 비교 및 결정론적 데이터 복원 검증 슬라이드
        </div>
        <span class="pf-fc-badge">PORTFOLIO ARCHIVE</span>
    </div>
    <p style="font-size: 0.9rem; color: #475569; margin: 0 0 16px 0; line-height: 1.6;">
        기획 및 개발 포트폴리오 발표 자료(14페이지)에 수록된 <strong>시드값 차이에 따른 데이터 생성 및 무결성 비교 원본</strong>입니다. 시드값 단 1의 차이(<code>...77</code> vs <code>...76</code>)로 완전히 다른 직업 스킬(치명타 버프 vs 물리 강타)이 파생되는 과정과, 세이브 토큰 디코딩을 통한 Skip 루프 복원 무결성을 도식화하여 실증했습니다.
    </p>
    <figure style="margin: 0; border-radius: 8px; overflow: hidden; border: 1px solid #cbd5e1; box-shadow: 0 4px 16px rgba(15, 23, 42, 0.05);">
        <img src="/assets/images/World%20First%20Kill/wfk_slide14_seed_comparison.png" alt="포트폴리오 14페이지: Seed 비교 예시로 데이터 비교 슬라이드" style="width: 100%; height: auto; display: block;" loading="lazy">
        <figcaption style="padding: 9px 14px; background: #f8fafc; border-top: 1px solid #e2e8f0; font-size: 0.8rem; color: #64748b; font-family: 'Fira Code', monospace;">
            ▲ 포트폴리오 원본 슬라이드: Seed A(1459589277)와 Seed B(1459589276)의 절차적 스킬 생성 및 복원 데이터 실측 비교
        </figcaption>
    </figure>
</div>

<details class="pf-details">
<summary>코드 보기: SaveLoadManager의 시드 토큰 빌드 및 복원 소스 코드</summary>

```csharp
// SaveLoadManager.cs: 시드 토큰 빌드 및 복원 구현
private const int BASE_SEED_WIDTH = 10;
private const int COUNT_WIDTH = 3;
private const int DOMAIN_ID_WIDTH = 3;
private const int USAGE_WIDTH = 12;

public static string BuildSeedToken()
{
    int baseSeed = GameSeed.BaseSeed;
    Dictionary<GameSeed.Domain, ulong> usage = GameSeed.GetUsageSnapshot();

    string token = Pad(EncodeIntToUInt(baseSeed), BASE_SEED_WIDTH);

    List<GameSeed.Domain> domains = new List<GameSeed.Domain>((GameSeed.Domain[])Enum.GetValues(typeof(GameSeed.Domain)));
    domains.Sort((a, b) => (a).CompareTo(b));

    token += Pad((uint)domains.Count, COUNT_WIDTH);
    foreach (var d in domains)
    {
        ulong count = usage.GetValueOrDefault(d, 0UL);
        token += Pad((uint)d, DOMAIN_ID_WIDTH);
        token += Pad(count, USAGE_WIDTH);
    }
    return token;
}

public static bool RestoreSeedToken(string token)
{
    try
    {
        int idx = 0;
        if (token.Length < BASE_SEED_WIDTH + COUNT_WIDTH) return false;

        ulong baseSeedEncoded = ReadUInt(token, ref idx, BASE_SEED_WIDTH);
        int baseSeed = DecodeUlongToInt(baseSeedEncoded);

        uint count = (uint)ReadUInt(token, ref idx, COUNT_WIDTH);
        int expectedTotal = (BASE_SEED_WIDTH + COUNT_WIDTH) + (int)count * (USAGE_WIDTH + DOMAIN_ID_WIDTH);
        if (token.Length != expectedTotal) return false;

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

        GameSeed.RestoreFromSnapshot(baseSeed, usage);
        return true;
    }
    catch { return false; }
}
```
</details>

{: .notice--success}
**Conclusion:** `WorldFirstKill`은 구글 스프레드시트 기반의 무중단 데이터 배포 및 증분 캐싱 파이프라인으로 라이브 서비스 운영 효율을 극대화하였으며, 마인크래프트에서 영감을 얻은 도메인 격리 시드/토큰 시스템과 Skip 복원 루프로 방대한 절차적 생성 RPG의 세이브/로드 오버헤드를 극적으로 압축하고 결정론적 무결성을 성공적으로 구현했습니다.
