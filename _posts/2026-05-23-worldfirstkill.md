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

새로운 게임 데이터(아이템, 몬스터 등)가 추가될 때마다 매핑 코드를 작성하는 수고를 덜기 위해, **리플렉션(Reflection)**을 활용한 자동 파싱 시스템을 구축했습니다.

### 1.1 핵심 작동 원리
1.  **타입 정보 추출:** 제네릭 메서드 `parseCSV<T>`에서 `typeof(T).GetFields()`를 통해 클래스 구조를 런타임에 분석.
2.  **헤더 매핑:** CSV의 컬럼명과 클래스의 필드명을 자동으로 매칭.
3.  **동적 할당:** `Activator.CreateInstance<T>()`로 객체를 생성하고, 필드 타입(int, float, string 등)에 맞춰 값을 자동 변환하여 할당.

```csharp
public List<T> parseCSV<T>(string csvData) where T : BaseData, new()
{
    Type myType = typeof(T);
    FieldInfo[] myFieldInfo = myType.GetFields();

    // CSV 행 순회 및 데이터 할당
    foreach (var line in csvLines) {
        T objectData = new T();
        // 리플렉션을 이용한 필드 값 세팅
        field.SetValue(objectData, Convert.ChangeType(value, field.FieldType));
    }
    return list;
}
```

---

# 2. Seed / Token 시스템 (RNG 완전 복원)

게임을 세이브하고 로드했을 때, 단순히 초기 시드값만 저장하면 이미 사용된 난수들이 다시 생성되어 게임 결과가 달라질 수 있습니다. 이를 방지하기 위해 **Seed/Token 시스템**을 설계했습니다.

### 2.1 토큰 구조 다이어그램
토큰은 **고정 폭(Fixed-Width) 인코딩**을 사용하여 파싱 안정성을 확보했습니다.

<div class="pf-visual-frame">
    <div style="font-weight: 700; margin-bottom: 20px; color: #007bff;">Seed Token 구조</div>
    <div class="pf-token-container">
        <div class="pf-token-row">
            <div class="pf-token-box">
                <div class="pf-token-box-title">BaseSeed</div>
                <div style="font-size: 0.85rem;">10자리 고정</div>
                <code style="color: #666;">예: 0000000123</code>
            </div>
            <div style="font-size: 1.5rem; color: #007bff;">+</div>
            <div class="pf-token-box">
                <div class="pf-token-box-title">Domain 개수</div>
                <div style="font-size: 0.85rem;">3자리 고정</div>
                <code style="color: #666;">예: 003</code>
            </div>
        </div>
        <div class="pf-domain-info">
            <div style="color: #28a745; font-weight: 700; margin-bottom: 10px;">Domain 정보 (반복)</div>
            <div class="pf-domain-grid">
                <div class="pf-domain-item">
                    <strong>Domain ID</strong> (3자리)
                </div>
                <div class="pf-domain-item">
                    <strong>Usage Count</strong> (12자리)
                </div>
            </div>
        </div>
    </div>
</div>

### 2.2 기술적 구현 (Build & Restore)
*   **Build:** 현재 초기 시드와 각 시스템(영역)별 난수 호출 횟수(Usage Count)를 가져와 하나의 문자열로 결합.
*   **Restore:** 토큰을 10자리, 3자리 단위로 잘라내어 시드와 사용량을 복구한 뒤, RNG 엔진의 상태를 강제로 동기화.

```csharp
public static string BuildSeedToken()
{
    int baseSeed = GameSeed.BaseSeed;  
    var usage = GameSeed.GetUsageSnapshot();  

    string token = Pad(baseSeed, 10); // 고정폭 패딩
    token += Pad(usage.Count, 3);
    // ... Domain 정보 추가
    return token;
}
```

---

# 3. 비동기 로딩 및 서버 동기화

서버의 최신 데이터와 로컬 데이터를 비교하여 필요한 파일만 업데이트하는 효율적인 로딩 시스템을 구축했습니다.

1.  **메타 시트 체크:** 서버의 버전 정보를 담은 메타 시트 우선 다운로드.
2.  **조건부 다운로드:** `로컬 버전 != 서버 버전`인 경우에만 `UnityWebRequest`를 통해 비동기 다운로드.
3.  **UI 블로킹 방지:** 무거운 파싱 작업은 `Task.Run()`을 통해 백그라운드 스레드에서 처리하고, `UnityMainThreadDispatcher`를 통해 UI를 갱신.

{: .notice--success}
**성과:** 수만 줄의 CSV 데이터를 파싱하는 동안에도 화면 멈춤(Freezing) 없이 매끄러운 로딩 게이지 애니메이션을 유지했습니다.
