---
title: "PROJECT REPORT // KRAFTON_JUNGLE_GAMETECH_PINBALL"
excerpt: "크래프톤 정글 GameTech 1기 입학 전형 기술 시험으로 구현한 DirectX 11 2D/3D 탄성 충돌 물리 시뮬레이터 및 심층 면접 복기 포스트모텀"
permalink: "/project/directx-pinball-game/"
tags: [DirectX 11, C++, HLSL, Physics Simulation, Dear ImGui, Retrospective]
hero_image: "assets/images/Pinball/pinball_title.png"
---

<style>
    .chapter-title {
        font-size: 2rem;
        color: #0f172a !important;
        background: #f8fafc;
        border: 1px solid #e2e8f0;
        border-left: 5px solid #2563eb !important;
        padding: 20px 28px;
        border-radius: 12px;
        margin-top: 50px !important;
        margin-bottom: 30px !important;
        box-shadow: 0 4px 20px rgba(15, 23, 42, 0.04);
        display: flex;
        align-items: center;
    }
    .chapter-title::before {
        content: "CHAPTER.";
        font-family: 'Fira Code', monospace;
        font-size: 0.85rem;
        letter-spacing: 2px;
        margin-right: 14px;
        color: #2563eb;
        font-weight: 700;
    }

    .pf-tech-callout {
        padding: 22px 26px;
        border-radius: 12px;
        background: #f8fbff;
        border: 1px solid #bfdbfe;
        border-left: 4px solid #2563eb;
        margin: 24px 0;
        box-shadow: 0 4px 16px rgba(37, 99, 235, 0.05);
    }
    .pf-tech-callout-title {
        font-family: 'Fira Code', monospace;
        font-size: 0.85rem;
        font-weight: 700;
        color: #1d4ed8;
        letter-spacing: 0.05em;
        margin-bottom: 8px;
        display: flex;
        align-items: center;
        gap: 8px;
    }
    .pf-tech-callout p {
        margin: 0;
        line-height: 1.7;
        color: #334155;
    }

    .pf-retrospective-panel {
        padding: 28px 32px;
        border-radius: 16px;
        background: #f0fdf4;
        border: 1px solid #bbf7d0;
        border-left: 6px solid #16a34a;
        margin: 35px 0;
        box-shadow: 0 10px 30px rgba(22, 163, 74, 0.08);
    }
    .pf-retrospective-panel.warning {
        background: #fffbeb;
        border-color: #fde68a;
        border-left-color: #d97706;
    }
    .pf-retrospective-panel.action {
        background: #eff6ff;
        border-color: #bfdbfe;
        border-left-color: #2563eb;
    }
    .pf-retro-badge {
        font-family: 'Fira Code', monospace;
        font-size: 0.76rem;
        font-weight: 700;
        padding: 4px 10px;
        border-radius: 6px;
        display: inline-block;
        margin-bottom: 12px;
        background: rgba(37, 99, 235, 0.1);
        color: #1d4ed8;
        border: 1px solid rgba(37, 99, 235, 0.25);
    }

    .pf-feature-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
        gap: 18px;
        margin: 24px 0;
    }
    .pf-feature-card {
        background: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 12px;
        padding: 22px;
        box-shadow: 0 4px 16px rgba(15, 23, 42, 0.04);
        transition: transform 0.2s ease, border-color 0.2s ease;
    }
    .pf-feature-card:hover {
        transform: translateY(-2px);
        border-color: #93c5fd;
    }
    .pf-feature-card-num {
        font-family: 'Fira Code', monospace;
        font-size: 0.8rem;
        color: #2563eb;
        font-weight: 700;
        margin-bottom: 6px;
    }
    .pf-feature-card-title {
        font-size: 1.05rem;
        font-weight: 700;
        color: #0f172a;
        margin-bottom: 10px;
    }
    .pf-feature-card-desc {
        font-size: 0.9rem;
        color: #475569;
        line-height: 1.62;
    }
</style>

<div class="pf-tech-callout">
    <div class="pf-tech-callout-title">
        <span>PROJECT IDENTITY // TIMED CODING TEST & RETROSPECTIVE</span>
    </div>
    <p>
        본 프로젝트는 <strong>크래프톤 정글 GameTech 1기 입학 전형</strong>의 기술 시험으로 치러진 실기 과제입니다. 
        상용 게임 엔진 프레임워크를 전혀 사용하지 않고 순수 <strong>C++와 DirectX 11, HLSL, Win32 API</strong>를 활용하여 정해진 시간 내에 요구사항인 <strong>2D/3D 탄성 충돌 물리 시뮬레이션, 침투 깊이 보정 및 벽면 반발, 2배 확장 동적 메모리 풀 관리</strong>를 밑바닥부터 완성했습니다.
        또한 기술 시험 합격 후 진행된 심층 면접 과정에서의 시선 처리와 발화 구조에 대한 솔직한 자기반성 및 극복 다짐을 기록한 성장 포스트모텀(Post-Mortem)을 포함합니다.
    </p>
</div>

<div class="pf-feature-grid">
    <div class="pf-feature-card">
        <div class="pf-feature-card-num">MODULE 01</div>
        <div class="pf-feature-card-title">DirectX 11 파이프라인 밑바닥 구축</div>
        <div class="pf-feature-card-desc">Device, SwapChain, RTV, Vertex/Pixel Shader 런타임 컴파일, Constant Buffer 변환 행렬 전송 파이프라인을 독자 구축했습니다.</div>
    </div>
    <div class="pf-feature-card">
        <div class="pf-feature-card-num">MODULE 02</div>
        <div class="pf-feature-card-title">2D/3D 완전 탄성 충돌 물리 엔진</div>
        <div class="pf-feature-card-desc">운동량 보존 법칙과 반발계수(e = 1.0) 기반 충격량(Impulse) 연산 및 질량비 침투 보정(Penetration Resolution)을 적용했습니다.</div>
    </div>
    <div class="pf-feature-card">
        <div class="pf-feature-card-num">MODULE 03</div>
        <div class="pf-feature-card-title">벽면 반발 및 경계 클램핑</div>
        <div class="pf-feature-card-desc">화면 경계([-1.0, 1.0]) 벽면 충돌 시 에너지 감쇠 반발계수(e = 0.9)와 반경 보정 클램핑으로 오브젝트 이탈을 완벽히 방지합니다.</div>
    </div>
    <div class="pf-feature-card">
        <div class="pf-feature-card-num">MODULE 04</div>
        <div class="pf-feature-card-title">동적 메모리 풀 용량 2배 확장</div>
        <div class="pf-feature-card-desc">원시 기하체 배열 용량 초과 시 2배 크기로 안전하게 재할당하고 포인터를 스왑하는 C 스타일 메모리 확장 풀을 구현했습니다.</div>
    </div>
</div>

<div class="pf-video-container" style="margin: 32px 0 40px 0; border-radius: 12px; overflow: hidden; border: 1px solid #dce6f3; box-shadow: 0 12px 36px rgba(37, 99, 235, 0.09); background: #020617;">
    <div style="padding: 10px 16px; background: #0f172a; color: #94a3b8; font-family: 'Fira Code', monospace; font-size: 0.78rem; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid rgba(255,255,255,0.08);">
        <span><span style="color: #2563eb;">●</span> IN-ENGINE REALTIME SIMULATION DEMO</span>
        <span style="color: #60a5fa;">DirectX 11 // Win32</span>
    </div>
    <video controls autoplay loop muted playsinline poster="/assets/images/Pinball/pinball_title.png" style="width: 100%; display: block; max-height: 540px; object-fit: contain; margin: 0 auto;" preload="metadata">
        <source src="/assets/videos/GameTest%202026-09-05%2014-45-17.mp4" type="video/mp4">
        Your browser does not support the video tag.
    </video>
    <div style="padding: 10px 18px; background: #f8fafc; border-top: 1px solid #e2e8f0; font-size: 0.82rem; color: #64748b; display: flex; justify-content: space-between; align-items: center;">
        <span>▲ DirectX 11 렌더링 파이프라인 및 강체 탄성 충돌 물리 시뮬레이션 시연</span>
        <span class="pf-mono" style="font-size: 0.76rem; color: #2563eb;">1080p 30FPS CAPTURE</span>
    </div>
</div>

---

## 1. 프로젝트 개요 및 기술 시험 요구사항
{: .chapter-title }

### 1.1 입학 시험 배경 및 목적
크래프톤 정글 GameTech는 현업 수준의 게임 클라이언트 및 코어 엔진 엔지니어를 양성하기 위한 몰입형 교육 과정입니다. 본 전형의 기술 시험은 지원자가 단순 라이브러리 사용자를 넘어 **컴퓨터 그래픽스 파이프라인의 하부 원리(DirectX 11 / HLSL)와 게임 수학 및 물리 법칙(충돌 감지, 운동량 보존, 벡터 해석)**을 직접 코드로 구현할 수 있는 하드웨어 친화적 엔지니어인지를 정밀 검증하는 타임어택 코딩 과제였습니다.

### 1.2 핵심 개발 요구사항

| 카테고리 | 상세 요구 사양 | 구현 대상 |
| :--- | :--- | :--- |
| **Graphics API** | DirectX 11 기반 윈도우 생성 및 스왑 체인, 버텍스 버퍼, 상수 버퍼 제어 | `URenderer.h`, `URenderer.cpp` |
| **Shader Language** | HLSL 기반 정점 셰이더(`VS`) 및 픽셀 셰이더(`PS`) 작성과 런타임 컴파일 | `Shader.hlsl` |
| **Object Architecture** | 다형성 기반 객체 지향 설계 (`UPrimitive` 추상 기반 클래스) | `main.cpp (UPrimitive, UBall)` |
| **Rigid Body Physics** | 원형/구체 간 탄성 충돌(Impulse Resolution) 및 질량비 침투 깊이 보정 | `UBall::HandlePrimitiveCollision` |
| **Boundary Physics** | 뷰포트 경계 벽면 충돌 감지 및 에너지 감쇠 반발($e = 0.9$) 적용 | `UBall::HandleWallCollision` |
| **Memory Management** | C 스타일 2배 확장(Capacity Doubling) 동적 메모리 풀 관리 | `EnsurePrimitiveCapacity` |

---

## 2. DirectX 11 렌더링 파이프라인 및 HLSL 셰이더
{: .chapter-title }

### 2.1 렌더러 아키텍처 (`URenderer`)
상용 엔진 없이 DirectX 11 API를 직접 제어하기 위해 렌더링 라이프사이클을 담당하는 `URenderer` 클래스를 설계했습니다:

```cpp
// URenderer 핵심 파이프라인 구조
void URenderer::Init(HWND hWnd)
{
    m_hWnd = hWnd;
    CreateDeviceAndSwapChain();
    CreateRenderTargetView();
    CreateInputLayout();
    CreateVertexShader();
    CreateConstantBuffer();
    CreateRasterizerState();
    CreatePixelShader();
}
```

1. **디바이스 및 스왑 체인 (`ID3D11Device`, `IDXGISwapChain`)**:  
   하드웨어 가속 플래그(`D3D_DRIVER_TYPE_HARDWARE`)와 더블 버퍼링 스왑 체인(`DXGI_SWAP_EFFECT_DISCARD`)을 구성했습니다.
2. **상수 버퍼(Constant Buffer)를 통한 트랜스폼 전달**:  
   각 구체의 월드 좌표 오프셋(Location)과 반지름 스케일(Scale)을 16바이트 정렬된 구조체로 묶어 매 프레임 GPU 레지스터 `b0`으로 업데이트합니다.
   
```cpp
void URenderer::UpdateConstant(FVector offset, FVector scale)
{
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    m_deviceContext->Map(m_constantBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
    FTransformData* data = (FTransformData*)mappedResource.pData;
    data->offset = offset;
    data->scale = scale;
    m_deviceContext->Unmap(m_constantBuffer, 0);
}
```

### 2.2 HLSL 셰이더 구조 (`Shader.hlsl`)
정점 셰이더에서 상수 버퍼의 오프셋과 스케일을 적용하여 투영 변환 없이 화면 정규화 좌표계(NDC [-1.0, 1.0]) 상에 구체 지오메트리를 직관적으로 배치합니다:

```hlsl
cbuffer TransformData : register(b0)
{
    float3 offset;
    float pad0;
    float3 scale;
    float pad1;
}

struct VS_INPUT
{
    float4 position : POSITION; 
    float4 color : COLOR;
};

struct PS_INPUT
{
    float4 position : SV_POSITION; 
    float4 color : COLOR;
};

PS_INPUT mainVS(VS_INPUT input)
{
    PS_INPUT output;
    output.position = float4(offset, 0.0f) + float4(input.position.xyz * scale, 1.0f);
    output.color = input.color;
    return output;
}

float4 mainPS(PS_INPUT input) : SV_TARGET
{
    return input.color;
}
```

---

## 3. 강체 탄성 충돌 물리 엔진 및 침투 해결 수식
{: .chapter-title }

### 3.1 완전 탄성 충돌 모델 (Momentum & Impulse Conservation)
두 구체 $A$와 $B$가 접촉했을 때 충돌 수직 축(Normal) 상에서의 상대 속도 $v_{rel}$와 충격량 $J$는 완전 탄성 충돌(반발계수 $e = 1.0$) 원리를 따릅니다:

$$v_{rel} = (\vec{v}_B - \vec{v}_A) \cdot \vec{n}$$

$$J = \frac{-(1 + e) \cdot v_{rel}}{\frac{1}{m_A} + \frac{1}{m_B}}$$

$$\vec{v}_A' = \vec{v}_A - \frac{J \cdot \vec{n}}{m_A}, \quad \vec{v}_B' = \vec{v}_B + \frac{J \cdot \vec{n}}{m_B}$$

```cpp
// main.cpp: UBall::HandlePrimitiveCollision 발췌
const float rvx = otherBall->Velocity.x - Velocity.x;
const float rvy = otherBall->Velocity.y - Velocity.y;
const float rvz = otherBall->Velocity.z - Velocity.z;

const float velocityAlongNormal = rvx * nx + rvy * ny + rvz * nz;
if (velocityAlongNormal > 0.0f) return; // 멀어지는 상태면 무시

const float restitution = 1.0f; // 완전 탄성 충돌
const float impulse = -(1.0f + restitution) * velocityAlongNormal / (1.0f / Mass + 1.0f / otherBall->Mass);

Velocity.x -= impulse * nx / Mass;
Velocity.y -= impulse * ny / Mass;
Velocity.z -= impulse * nz / Mass;

otherBall->Velocity.x += impulse * nx / otherBall->Mass;
otherBall->Velocity.y += impulse * ny / otherBall->Mass;
otherBall->Velocity.z += impulse * nz / otherBall->Mass;
```

### 3.2 질량비 기반 침투 깊이 위치 보정 (Penetration Depth Resolution)
이산 시간 시뮬레이션(Discrete Time-step)에서 공들이 고속으로 이동할 때 한 프레임 사이에 두 공이 서로의 반경 안으로 파고드는 겹침 현상이 발생합니다.  
이를 방지하기 위해 두 공의 질량비(Mass Ratio)를 산출하여 질량이 가벼운 공이 더 많은 거리를 밀려나도록 즉각적인 위치 보정을 가했습니다:

```cpp
const float overlap = Radius + otherBall->Radius - distance;
if (overlap > 0.0f)
{
    const float totalMass = Mass + otherBall->Mass;
    const float myRatio = otherBall->Mass / totalMass;
    const float otherRatio = Mass / totalMass;

    Location.x -= nx * overlap * myRatio;
    Location.y -= ny * overlap * myRatio;
    Location.z -= nz * overlap * myRatio;

    otherBall->Location.x += nx * overlap * otherRatio;
    otherBall->Location.y += ny * overlap * otherRatio;
    otherBall->Location.z += nz * overlap * otherRatio;
}
```

---

## 4. 벽면 충돌 반발 계수 및 동적 버퍼 용량 관리
{: .chapter-title }

### 4.1 경계면 충돌 및 에너지 감쇠 반발 (`HandleWallCollision`)
뷰포트 정규화 좌표계($[-1.0, 1.0]$) 내에서 구체가 화면 밖으로 이탈하지 않도록 구체의 반경(Radius)을 고려한 경계 클램핑 및 법선 속도 반전 반발계수($e = 0.9$)를 구현했습니다:

```cpp
void UBall::HandleWallCollision()
{
    const float wallRestitution = 0.9f;

    if (Location.x < LEFT + Radius)
    {
        Location.x = LEFT + Radius;
        if (Velocity.x < 0.0f)
        {
            Velocity.x = -Velocity.x * wallRestitution;
        }
    }
    else if (Location.x > RIGHT - Radius)
    {
        Location.x = RIGHT - Radius;
        if (Velocity.x > 0.0f)
        {
            Velocity.x = -Velocity.x * wallRestitution;
        }
    }

    if (Location.y < BOTTOM + Radius)
    {
        Location.y = BOTTOM + Radius;
        if (Velocity.y < 0.0f)
        {
            Velocity.y = -Velocity.y * wallRestitution;
        }
    }
    else if (Location.y > TOP - Radius)
    {
        Location.y = TOP - Radius;
        if (Velocity.y > 0.0f)
        {
            Velocity.y = -Velocity.y * wallRestitution;
        }
    }
}
```

### 4.2 C 스타일 동적 버퍼 2배 확장 아키텍처 (`EnsurePrimitiveCapacity`)
표준 템플릿 라이브러리(STL)의 오버헤드를 배제하고 메모리 재할당 비용을 최적화하기 위해, 벡터의 기하급수적 확장(Exponential Growth, 2배 용량 증가) 패턴을 직접 C 포인터 배열로 설계했습니다:

```cpp
void EnsurePrimitiveCapacity(UPrimitive*** primitiveList, int* capacity, int requiredCapacity)
{
    if (requiredCapacity <= *capacity)
    {
        return;
    }

    int newCapacity = *capacity;
    if (newCapacity < 1)
    {
        newCapacity = 1;
    }

    while (newCapacity < requiredCapacity)
    {
        newCapacity *= 2;
    }

    UPrimitive** newPrimitiveList = new UPrimitive*[newCapacity];
    for (int i = 0; i < newCapacity; ++i)
    {
        newPrimitiveList[i] = nullptr;
    }

    for (int i = 0; i < *capacity; ++i)
    {
        newPrimitiveList[i] = (*primitiveList)[i];
    }

    delete[](*primitiveList);
    *primitiveList = newPrimitiveList;
    *capacity = newCapacity;
}
```

---

## 5. 면접 복기 및 성장 회고 (Retrospective & Post-Mortem)
{: .chapter-title }

<div class="pf-retrospective-panel warning">
    <div class="pf-retro-badge">SELF REFLECTION // 아쉬웠던 점 및 솔직한 패인 분석</div>
    <h3 style="color: #92400e; margin-top: 0;">1. 면접 현장에서 스스로에게 남은 큰 아쉬움</h3>
    <p>
        기술 과제 테스트 자체는 DirectX 11 파이프라인과 2D/3D 탄성 충돌 물리 로직까지 요구 사양을 정해진 시간 내에 오차 없이 완결지었습니다.
        하지만 이어진 심층 기술 면접 과정에서 스스로에게 너무나도 큰 아쉬움이 남았습니다.<br><br>
        면접관과 마주했을 때 <strong>극도의 긴장감으로 인해 시선 처리가 자연스럽지 못하고 불안정</strong>했으며, 질문을 받았을 때 머릿속으로는 알고 있는 개념과 구조임에도 불구하고 이를 차분하게 두괄식으로 정리하여 말하지 못하고 <strong>말을 횡설수설했던 점</strong>이 불합격의 가장 결정적인 이유였다고 솔직하게 복기합니다.
    </p>
</div>

<div class="pf-retrospective-panel action">
    <div class="pf-retro-badge">ACTION PLAN // 뼈아픈 실패를 극복하기 위한 개선 훈련</div>
    <h3 style="color: #1e40af; margin-top: 0;">2. 교훈과 향후 면접 준비 및 발화 훈련 계획</h3>
    <p>
        코드로 구현할 수 있는 실력만큼이나, <strong>"자신이 작성한 코드와 아키텍처, 전공 지식을 타인에게 설득력 있고 명확하게 전달하는 소통 능력"</strong>이 엔지니어로서 얼마나 결정적인 역량인지 뼈저리게 체감했습니다.<br><br>
        이 아쉬운 탈락의 경험을 단순한 좌절이 아닌 가장 값진 성장 동력으로 삼기 위해, 앞으로의 면접 준비는 아래와 같이 구체적인 행동 원칙을 세워 훈련하고 있습니다:
    </p>
    <ul style="margin: 14px 0 0 0; padding-left: 20px; line-height: 1.8;">
        <li><strong>거울 보고 시선 처리 및 표정 훈련</strong>: 거울 및 모의 화상 카메라를 정면으로 응시하며 면접관과 아이컨택을 안정적으로 유지하고, 긴장으로 인해 시선이 흔들리지 않도록 반복 연습합니다.</li>
        <li><strong>두괄식 답변 구조화 (PREP 기법)</strong>: 결론(Point) → 이유(Reason) → 구현 사례(Example) → 요약(Point) 순서로 머릿속 지식을 횡설수설하지 않고 1분 이내로 핵심만 전달하는 스피치 구조화 훈련을 진행합니다.</li>
        <li><strong>CS 핵심 키워드 정리 노트</strong>: 그래픽스 파이프라인, 물리 적분법, 메모리 동적 할당 원리 등 핵심 지식을 구술 테스트 형태로 즉각 인출할 수 있도록 백지 복습법으로 체계화하고 있습니다.</li>
    </ul>
</div>

<div class="pf-tech-callout">
    <div class="pf-tech-callout-title">
        <span>ENGINEERING PHILOSOPHY // 실패를 직시하는 개발자</span>
    </div>
    <p>
        부족했던 점을 숨기지 않고 투명하게 인정하며, 실패의 원인을 정확히 메타인지(Metacognition)하고 행동으로 보완해 나가는 것 또한 훌륭한 게임 클라이언트 프로그래머가 갖추어야 할 중요한 소양이라고 믿습니다. 
        크래프톤 정글 GameTech 시험에서 겪은 이 값진 반성은 향후 어떤 기술 면접과 협업 현장에서도 더 단단하고 신뢰받는 엔지니어로 성장하는 든든한 밑거름이 될 것입니다.
    </p>
</div>
