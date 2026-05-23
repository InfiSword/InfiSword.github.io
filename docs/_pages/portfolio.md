layout: single

title: "Minhyuk Lee | Systems \& Rendering"

permalink: /portfolio/

author\_profile: true

classes: wide



&#x20;   /\* --- Scoped Global Layouts \& UI Components --- \*/

&#x20;   #pf-custom-portfolio {

&#x20;       font-family: 'Inter', 'Noto Sans KR', sans-serif;

&#x20;       color: var(--pf-text-color);

&#x20;       line-height: 1.6;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-mono {

&#x20;       font-family: 'Fira Code', monospace;

&#x20;   }

&#x20;   #pf-custom-portfolio a {

&#x20;       color: var(--pf-accent-color);

&#x20;       text-decoration: none;

&#x20;   }

&#x20;   #pf-custom-portfolio a:hover {

&#x20;       text-decoration: underline;

&#x20;   }



&#x20;   /\* --- Custom Hero Tag --- \*/

&#x20;   #pf-custom-portfolio .pf-intro-text {

&#x20;       font-size: 1.1rem;

&#x20;       line-height: 1.8;

&#x20;       margin-bottom: 25px;

&#x20;       color: #c9d1d9;

&#x20;   }



&#x20;   /\* --- Tech Stack --- \*/

&#x20;   #pf-custom-portfolio .pf-skill-container {

&#x20;       display: flex;

&#x20;       flex-wrap: wrap;

&#x20;       gap: 15px;

&#x20;       margin-bottom: 50px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-skill-item {

&#x20;       background: var(--pf-card-bg);

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       padding: 15px 24px;

&#x20;       border-radius: 8px;

&#x20;       transition: 0.3s;

&#x20;       flex: 1 1 calc(33.333% - 15px);

&#x20;       min-width: 250px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-skill-item:hover {

&#x20;       border-color: var(--pf-accent-color);

&#x20;       transform: translateY(-3px);

&#x20;       box-shadow: 0 4px 15px rgba(88, 166, 255, 0.15);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-skill-label {

&#x20;       font-size: 0.8rem;

&#x20;       color: var(--pf-accent-color);

&#x20;       display: block;

&#x20;       margin-bottom: 5px;

&#x20;       text-transform: uppercase;

&#x20;       font-weight: 700;

&#x20;   }



&#x20;   /\* --- Project Cards --- \*/

&#x20;   #pf-custom-portfolio .pf-project-card {

&#x20;       background: var(--pf-card-bg);

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 12px;

&#x20;       overflow: hidden;

&#x20;       transition: 0.4s cubic-bezier(0.165, 0.84, 0.44, 1);

&#x20;       margin-bottom: 25px;

&#x20;       cursor: pointer;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-project-card:hover {

&#x20;       border-color: var(--pf-accent-color);

&#x20;       transform: scale(1.015);

&#x20;       box-shadow: 0 15px 30px rgba(0,0,0,0.5);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-project-info {

&#x20;       padding: 35px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-project-tag {

&#x20;       color: var(--pf-sub-accent);

&#x20;       font-size: 0.85rem;

&#x20;       font-weight: 700;

&#x20;       margin-bottom: 10px;

&#x20;       display: block;

&#x20;       text-transform: uppercase;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-project-name {

&#x20;       font-size: 1.8rem;

&#x20;       color: #f0f6fc;

&#x20;       margin-bottom: 15px;

&#x20;       margin-top: 0;

&#x20;       font-weight: 700;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-highlight {

&#x20;       color: #f0f6fc;

&#x20;       font-weight: 700;

&#x20;   }

&#x20;   

&#x20;   /\* --- Links \& Buttons inside Cards --- \*/

&#x20;   #pf-custom-portfolio .pf-card-actions {

&#x20;       display: flex;

&#x20;       gap: 15px;

&#x20;       margin-top: 20px;

&#x20;       flex-wrap: wrap;

&#x20;       align-items: center;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-btn-modal {

&#x20;       font-size: 0.85rem;

&#x20;       color: var(--pf-accent-color);

&#x20;       font-weight: bold;

&#x20;       cursor: pointer;

&#x20;       border: 1px solid rgba(88, 166, 255, 0.3);

&#x20;       padding: 6px 14px;

&#x20;       border-radius: 6px;

&#x20;       background: rgba(88, 166, 255, 0.05);

&#x20;       transition: 0.2s;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-btn-modal:hover {

&#x20;       background: rgba(88, 166, 255, 0.15);

&#x20;       border-color: var(--pf-accent-color);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-btn-post {

&#x20;       font-size: 0.85rem;

&#x20;       color: #f0f6fc;

&#x20;       font-weight: bold;

&#x20;       cursor: pointer;

&#x20;       border: 1px solid rgba(35, 134, 54, 0.5);

&#x20;       padding: 6px 14px;

&#x20;       border-radius: 6px;

&#x20;       background: rgba(35, 134, 54, 0.2);

&#x20;       transition: 0.2s;

&#x20;       text-decoration: none !important;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-btn-post:hover {

&#x20;       background: rgba(35, 134, 54, 0.4);

&#x20;       border-color: var(--pf-sub-accent);

&#x20;   }



&#x20;   /\* --- Modals --- \*/

&#x20;   .pf-modal-overlay {

&#x20;       display: none;

&#x20;       position: fixed;

&#x20;       top: 0;

&#x20;       left: 0;

&#x20;       width: 100vw;

&#x20;       height: 100vh;

&#x20;       background: rgba(0,0,0,0.92);

&#x20;       z-index: 99999;

&#x20;       justify-content: center;

&#x20;       align-items: center;

&#x20;       backdrop-filter: blur(10px);

&#x20;       margin: 0;

&#x20;       padding: 0;

&#x20;   }

&#x20;   .pf-modal-container {

&#x20;       background: var(--pf-bg-color);

&#x20;       width: 95%;

&#x20;       max-width: 1000px;

&#x20;       height: 90vh;

&#x20;       max-height: 90vh;

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 16px;

&#x20;       overflow-y: auto;

&#x20;       animation: pfModalIn 0.3s ease-out;

&#x20;       box-shadow: 0 10px 40px rgba(0,0,0,0.8);

&#x20;   }

&#x20;   @keyframes pfModalIn {

&#x20;       from { transform: translateY(20px); opacity: 0; }

&#x20;       to { transform: translateY(0); opacity: 1; }

&#x20;   }

&#x20;   .pf-modal-body {

&#x20;       padding: 40px;

&#x20;       color: var(--pf-text-color);

&#x20;   }

&#x20;   .pf-modal-header-nav {

&#x20;       display: flex;

&#x20;       justify-content: space-between;

&#x20;       align-items: center;

&#x20;       margin-bottom: 35px;

&#x20;       border-bottom: 1px solid var(--pf-border-color);

&#x20;       padding-bottom: 15px;

&#x20;       gap: 10px;

&#x20;       flex-wrap: wrap;

&#x20;   }

&#x20;   .pf-modal-body h1 {

&#x20;       color: #f0f6fc;

&#x20;       font-size: 2.2rem;

&#x20;       margin-top: 0;

&#x20;       margin-bottom: 25px;

&#x20;   }

&#x20;   .pf-modal-body h2 {

&#x20;       color: var(--pf-accent-color);

&#x20;       margin: 45px 0 20px;

&#x20;       font-size: 1.6rem;

&#x20;       border-left: 5px solid var(--pf-accent-color);

&#x20;       padding-left: 15px;

&#x20;   }

&#x20;   .pf-modal-body h3 {

&#x20;       color: #f0f6fc;

&#x20;       margin: 30px 0 15px;

&#x20;       font-size: 1.2rem;

&#x20;       font-weight: 600;

&#x20;       border-bottom: 1px solid var(--pf-border-color);

&#x20;       padding-bottom: 8px;

&#x20;   }

&#x20;   .pf-modal-body p {

&#x20;       margin-bottom: 15px;

&#x20;       color: #8b949e;

&#x20;       font-size: 1.02rem;

&#x20;       line-height: 1.7;

&#x20;   }

&#x20;   .pf-modal-body ul {

&#x20;       margin: 15px 0 25px 20px;

&#x20;       color: #8b949e;

&#x20;   }

&#x20;   .pf-modal-body li {

&#x20;       margin-bottom: 8px;

&#x20;   }



&#x20;   /\* --- Diagrams \& Visual Flows --- \*/

&#x20;   #pf-custom-portfolio .pf-visual-frame {

&#x20;       width: 100%;

&#x20;       padding: 25px;

&#x20;       background: rgba(0,0,0,0.4);

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 8px;

&#x20;       margin: 20px 0;

&#x20;       text-align: center;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-diagram-grid {

&#x20;       display: grid;

&#x20;       grid-template-columns: repeat(3, 40px);

&#x20;       gap: 5px;

&#x20;       justify-content: center;

&#x20;       margin: 10px 0;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-grid-cell {

&#x20;       width: 40px;

&#x20;       height: 40px;

&#x20;       border: 1px solid var(--pf-accent-color);

&#x20;       opacity: 0.2;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-grid-cell.active {

&#x20;       background: var(--pf-accent-color);

&#x20;       opacity: 1;

&#x20;       box-shadow: 0 0 15px var(--pf-accent-color);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-grid-cell.near {

&#x20;       border-color: var(--pf-sub-accent);

&#x20;       background: rgba(35, 134, 54, 0.2);

&#x20;       opacity: 1;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-transaction-flow {

&#x20;       display: flex;

&#x20;       align-items: center;

&#x20;       justify-content: center;

&#x20;       font-size: 0.75rem;

&#x20;       gap: 10px;

&#x20;       flex-wrap: wrap;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-flow-step {

&#x20;       padding: 8px 12px;

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 4px;

&#x20;       background: var(--pf-card-bg);

&#x20;       color: var(--pf-text-color);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-flow-arrow {

&#x20;       color: var(--pf-accent-color);

&#x20;       font-weight: bold;

&#x20;   }



&#x20;   /\* --- Tables --- \*/

&#x20;   #pf-custom-portfolio .pf-data-table {

&#x20;       width: 100%;

&#x20;       border-collapse: collapse;

&#x20;       margin: 20px 0;

&#x20;       font-size: 0.9rem;

&#x20;       background: var(--pf-card-bg);

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 8px;

&#x20;       overflow: hidden;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-data-table th {

&#x20;       background: rgba(88, 166, 255, 0.1);

&#x20;       color: var(--pf-accent-color);

&#x20;       padding: 12px 15px;

&#x20;       text-align: center;

&#x20;       font-weight: 700;

&#x20;       border-bottom: 2px solid var(--pf-border-color);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-data-table td {

&#x20;       padding: 10px 15px;

&#x20;       border-bottom: 1px solid var(--pf-border-color);

&#x20;       color: var(--pf-text-color);

&#x20;       text-align: center;

&#x20;   }



&#x20;   /\* --- Architecture \& Coord Flow --- \*/

&#x20;   #pf-custom-portfolio .pf-arch-diagram {

&#x20;       display: flex;

&#x20;       flex-direction: column;

&#x20;       gap: 12px;

&#x20;       margin: 20px 0;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-arch-layer {

&#x20;       padding: 15px;

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 6px;

&#x20;       background: var(--pf-card-bg);

&#x20;       position: relative;

&#x20;       text-align: center;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-arch-layer::before {

&#x20;       content: "";

&#x20;       position: absolute;

&#x20;       left: 0;

&#x20;       top: 0;

&#x20;       bottom: 0;

&#x20;       width: 4px;

&#x20;       background: var(--pf-accent-color);

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-arch-layer-title {

&#x20;       color: var(--pf-accent-color);

&#x20;       font-weight: 700;

&#x20;       margin-bottom: 8px;

&#x20;       font-size: 0.9rem;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-arch-layer-items {

&#x20;       display: flex;

&#x20;       flex-wrap: wrap;

&#x20;       gap: 8px;

&#x20;       justify-content: center;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-arch-item {

&#x20;       padding: 4px 10px;

&#x20;       background: rgba(88, 166, 255, 0.1);

&#x20;       border: 1px solid var(--pf-accent-color);

&#x20;       border-radius: 4px;

&#x20;       font-size: 0.8rem;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-flow {

&#x20;       display: flex;

&#x20;       gap: 15px;

&#x20;       margin: 20px 0;

&#x20;       overflow-x: auto;

&#x20;       padding-bottom: 10px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-box {

&#x20;       padding: 15px;

&#x20;       border: 2px solid var(--pf-accent-color);

&#x20;       border-radius: 8px;

&#x20;       background: rgba(88, 166, 255, 0.05);

&#x20;       text-align: center;

&#x20;       min-width: 180px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-box-title {

&#x20;       color: var(--pf-accent-color);

&#x20;       font-weight: 700;

&#x20;       margin-bottom: 8px;

&#x20;       border-bottom: 1px solid var(--pf-border-color);

&#x20;       padding-bottom: 5px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-box-desc {

&#x20;       font-size: 0.8rem;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-box-formula {

&#x20;       color: var(--pf-sub-accent);

&#x20;       font-size: 0.75rem;

&#x20;       margin-top: 5px;

&#x20;       padding: 4px;

&#x20;       background: rgba(35, 134, 54, 0.1);

&#x20;       border-radius: 4px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-arrow {

&#x20;       color: var(--pf-accent-color);

&#x20;       font-size: 1.5rem;

&#x20;       display: flex;

&#x20;       align-items: center;

&#x20;       justify-content: center;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-coord-bidirectional {

&#x20;       text-align: center;

&#x20;       color: var(--pf-sub-accent);

&#x20;       margin: 20px 0;

&#x20;       font-size: 0.9rem;

&#x20;       font-weight: 700;

&#x20;       padding: 10px;

&#x20;       background: rgba(35, 134, 54, 0.1);

&#x20;       border-radius: 6px;

&#x20;       border: 1px solid var(--pf-sub-accent);

&#x20;   }



&#x20;   /\* --- Code Blocks inside Modals --- \*/

&#x20;   #pf-custom-portfolio pre {

&#x20;       background: var(--pf-code-bg);

&#x20;       padding: 20px;

&#x20;       border-radius: 8px;

&#x20;       font-family: 'Fira Code', monospace;

&#x20;       font-size: 0.85rem;

&#x20;       color: #d1d5da;

&#x20;       overflow-x: auto;

&#x20;       margin: 20px 0;

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       line-height: 1.5;

&#x20;   }

&#x20;   .pf-c-kw { color: var(--pf-code-keyword); }

&#x20;   .pf-c-ty { color: var(--pf-code-type); }

&#x20;   .pf-c-fn { color: var(--pf-code-func); }

&#x20;   .pf-c-cm { color: var(--pf-code-comment); }

&#x20;   .pf-c-st { color: var(--pf-code-string); }



&#x20;   /\* --- Tab Control --- \*/

&#x20;   .pf-tab-container {

&#x20;       display: flex;

&#x20;       gap: 10px;

&#x20;       margin-bottom: 30px;

&#x20;       border-bottom: 2px solid var(--pf-border-color);

&#x20;   }

&#x20;   .pf-tab-button {

&#x20;       background: none;

&#x20;       border: none;

&#x20;       color: #8b949e;

&#x20;       padding: 12px 24px;

&#x20;       font-family: 'Fira Code', monospace;

&#x20;       font-size: 0.95rem;

&#x20;       cursor: pointer;

&#x20;       border-bottom: 3px solid transparent;

&#x20;       transition: all 0.3s;

&#x20;   }

&#x20;   .pf-tab-button:hover {

&#x20;       color: var(--pf-accent-color);

&#x20;   }

&#x20;   .pf-tab-button.active {

&#x20;       color: var(--pf-accent-color);

&#x20;       border-bottom-color: var(--pf-accent-color);

&#x20;   }

&#x20;   .pf-tab-content {

&#x20;       display: none;

&#x20;   }

&#x20;   .pf-tab-content.active {

&#x20;       display: block;

&#x20;   }



&#x20;   /\* --- Experience Timeline Panel --- \*/

&#x20;   #pf-custom-portfolio .pf-exp-panel {

&#x20;       padding: 25px;

&#x20;       border: 1px solid var(--pf-border-color);

&#x20;       border-radius: 8px;

&#x20;       background: rgba(0, 0, 0, 0.2);

&#x20;       margin-top: 30px;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-exp-title {

&#x20;       color: var(--pf-accent-color);

&#x20;       font-weight: 700;

&#x20;       margin-bottom: 15px;

&#x20;       font-size: 1.2rem;

&#x20;   }

&#x20;   #pf-custom-portfolio .pf-exp-section-title {

&#x20;       color: var(--pf-sub-accent);

&#x20;       font-weight: 700;

&#x20;       margin-top: 20px;

&#x20;       margin-bottom: 10px;

&#x20;       font-size: 1rem;

&#x20;   }

</style>



<div class="pf-intro-text">

&#x20;   안녕하세요. 저는 게임 개발자로서 꿈을 키우며 열심히 연마하고 있는 이민혁입니다. 

&#x20;   요즘엔 컴퓨터 그래픽스(Rendering), 그리고 머신러닝(ML-Agents)을 깊이 있게 연구하고 있으며, 역량 높은 소프트웨어 엔지니어가 되기 위해 Computer Science를 열정적으로 정독하고 있습니다.

</div>



<div class="pf-skill-container">

&#x20;   <div class="pf-skill-item">

&#x20;       <span class="pf-skill-label pf-mono">Languages</span>

&#x20;       <span class="pf-mono pf-highlight">C++, C#, HLSL, Python</span>

&#x20;   </div>

&#x20;   <div class="pf-skill-item">

&#x20;       <span class="pf-skill-label pf-mono">Engine \& Graphics</span>

&#x20;       <span class="pf-mono pf-highlight">Unity, Unreal, DirectX 11, Computer Graphics</span>

&#x20;   </div>

&#x20;   <div class="pf-skill-item">

&#x20;       <span class="pf-skill-label pf-mono">Specialized</span>

&#x20;       <span class="pf-mono pf-highlight">ML-Agents (Machine Learning)</span>

&#x20;   </div>

</div>



<h2 style="font-family: 'Fira Code', monospace; margin: 50px 0 25px 0;">01. Game Projects</h2>



<!-- PROJECT CARD: FILE TOWER DEFENSE -->

<article class="pf-project-card" onclick="pfOpenModal('fileTowerDefense')">

&#x20;   <div class="pf-project-info">

&#x20;       <span class="pf-project-tag pf-mono">Systems Engineering</span>

&#x20;       <h3 class="pf-project-name">File Tower Defense</h3>

&#x20;       <p style="color: #8b949e; margin-bottom: 15px;">

&#x20;           유니티의 좌표계 특성을 분석하여 <span class="pf-highlight">RectTransform(UI)에서 Transform(GameObject) 기반</span>으로 아키텍처를 전면 재설계. 

&#x20;           <span class="pf-highlight">공간 분할 최적화</span>와 <span class="pf-highlight">트랜잭션 기반 배치 시스템</span>을 구축하여 성능 최적화와 확장성 있는 그리드 관리 시스템을 완성. 

&#x20;           <span class="pf-highlight">중앙 집중식 마우스 입력 관리(InputManager)</span>와 <span class="pf-highlight">이벤트 기반 상호작용 시스템(InteractionHandler)</span>을 설계하여 프레임 최적화 달성.

&#x20;       </p>

&#x20;       <div class="pf-card-actions">

&#x20;           <span class="pf-mono pf-btn-modal">⚡ 빠른 기술요약 보기</span>

&#x20;           <a href="/project/file-tower-defense/" class="pf-mono pf-btn-post" onclick="event.stopPropagation();">🔗 개별 상세페이지로 가기 ↗</a>

&#x20;       </div>

&#x20;   </div>

</article>



<!-- PROJECT CARD: RACING AGENT -->

<article class="pf-project-card" onclick="pfOpenModal('racing')">

&#x20;   <div class="pf-project-info">

&#x20;       <span class="pf-project-tag pf-mono">ML-Agents \& Reinforcement Learning</span>

&#x20;       <h3 class="pf-project-name">Autonomous Racing Agent</h3>

&#x20;       <p style="color: #8b949e; margin-bottom: 15px;">

&#x20;           Unity ML-Agents를 활용한 <span class="pf-highlight">자율 주행 경주차 에이전트</span> 학습 시스템 구축. 

&#x20;           <span class="pf-highlight">체크포인트 기반 네비게이션 시스템</span>과 <span class="pf-highlight">목표 지향 보상 설계</span>를 통해 정해진 트랙을 안정적으로 주행하도록 학습. 

&#x20;           <span class="pf-highlight">레이캐스트 기반 환경 감지</span>(전방/후방 벽 및 도로 감지)와 <span class="pf-highlight">스플라인 기반 체크포인트 자동 생성 시스템</span>을 설계하여 높은 범용성 주행 구현.

&#x20;       </p>

&#x20;       <div class="pf-card-actions">

&#x20;           <span class="pf-mono pf-btn-modal">⚡ 빠른 기술요약 보기</span>

&#x20;           <a href="/project/autonomous-racing-agent/" class="pf-mono pf-btn-post" onclick="event.stopPropagation();">🔗 개별 상세페이지로 가기 ↗</a>

&#x20;       </div>

&#x20;   </div>

</article>



<!-- PROJECT CARD: WORLD FIRST KILL -->

<article class="pf-project-card" onclick="pfOpenModal('wfk')">

&#x20;   <div class="pf-project-info">

&#x20;       <span class="pf-project-tag pf-mono">Data Management \& Parsing</span>

&#x20;       <h3 class="pf-project-name">WorldFirstKill</h3>

&#x20;       <p style="color: #8b949e; margin-bottom: 15px;">

&#x20;           <span class="pf-highlight">리플렉션 기반 CSV 파싱 시스템</span>과 <span class="pf-highlight">서버 CSV 다운로드 및 버전 관리 시스템</span>을 설계하여 유기적인 실시간 시트 데이터 동기화 구현. 

&#x20;           <span class="pf-highlight">JSON 기반 저장/불러오기</span> 및 <span class="pf-highlight">Seed/Token 시스템</span>을 구현하여 난수(RNG) 생성 흐름 상태까지 완전히 안전하게 저장하고 복원하는 로드맵 구현.

&#x20;       </p>

&#x20;       <div class="pf-card-actions">

&#x20;           <span class="pf-mono pf-btn-modal">⚡ 빠른 기술요약 보기</span>

&#x20;           <a href="/project/worldfirstkill/" class="pf-mono pf-btn-post" onclick="event.stopPropagation();">🔗 개별 상세페이지로 가기 ↗</a>

&#x20;       </div>

&#x20;   </div>

</article>



<!-- PROJECT CARD: SLIME PROJECT -->

<article class="pf-project-card" onclick="pfOpenModal('slime')">

&#x20;   <div class="pf-project-info">

&#x20;       <span class="pf-project-tag pf-mono">State Machine \& Game Logic</span>

&#x20;       <h3 class="pf-project-name">Slime Project</h3>

&#x20;       <p style="color: #8b949e; margin-bottom: 15px;">

&#x20;           하드코어 액션 퍼즐 플랫포머 게임에서 <span class="pf-highlight">FiniteStateMachine 기반 상태 관리 시스템</span>을 설계하여 모든 엔티티의 행동 패턴을 통합 설계. 

&#x20;           <span class="pf-highlight">컴포지션 패턴 기반 플레이어 시스템</span>(움직임, 공격, 대시, 벽 점프 변신) 및 <span class="pf-highlight">보스 몬스터의 델리게이트 기반 스킬 패턴 AI</span>를 정형화하여 유연하고 확장성 높은 게임 루프 제작.

&#x20;       </p>

&#x20;       <div class="pf-card-actions">

&#x20;           <span class="pf-mono pf-btn-modal">⚡ 빠른 기술요약 보기</span>

&#x20;           <a href="/project/slime-project/" class="pf-mono pf-btn-post" onclick="event.stopPropagation();">🔗 개별 상세페이지로 가기 ↗</a>

&#x20;       </div>

&#x20;   </div>

</article>



<!-- EXPERIENCE \& AWARDS TIMELINE -->

<div class="pf-exp-panel">

&#x20;   <div class="pf-exp-title">02. Awards \& Experience</div>

&#x20;   

&#x20;   <div class="pf-exp-section-title">🏆 Awards (수상 경력)</div>

&#x20;   <div style="line-height: 1.8; padding-left: 10px; font-size: 0.95rem;">

&#x20;       <div>• <span class="pf-highlight">2025</span> 지스타 인디어워즈 <a href="https://www.youtube.com/watch?v=kxJznVTVCfk\&t=19s" target="\_blank">Best Experimental Game</a> (파일타워 디펜스)</div>

&#x20;       <div>• <span class="pf-highlight">2025</span> 제3회 <a href="https://www.tu.ac.kr/game/sub05\_01.do?mode=view\&articleNo=131034\&article.offset=0\&articleLimit=10" target="\_blank">TU Game Challenge</a> 우수상 (파일타워 디펜스)</div>

&#x20;       <div>• <span class="pf-highlight">2025</span> 창업아이디어 경진대회 우수상</div>

&#x20;       <div>• <span class="pf-highlight">2025</span> 부산지역인재 장학금 IT분야 수여 및 대동장학재단 장학금 수여</div>

&#x20;       <div>• <span class="pf-highlight">2021</span> TU-SEED 리빙랩 경진대회 입상</div>

&#x20;   </div>



&#x20;   <div class="pf-exp-section-title">🏃 Experience (경험 및 활동)</div>

&#x20;   <div style="line-height: 1.8; padding-left: 10px; font-size: 0.95rem;">

&#x20;       <div>• <span class="pf-highlight">2026</span> <a href="https://www.tu.ac.kr/game/sub05\_01.do?mode=view\&articleNo=144072\&article.offset=0\&articleLimit=10" target="\_blank">Mach Game Jam</a> 참여</div>

&#x20;       <div>• <span class="pf-highlight">2025</span> 서울 코믹 월드 인디 부스 / 지스타 인디 쇼케이스 부스 운영 (파일타워 디펜스)</div>

&#x20;       <div>• <span class="pf-highlight">2025</span> <a href="https://www.tu.ac.kr/gart/sub05\_05.do?mode=view\&articleNo=143632\&title=RISE+ALL+FESTA+%EC%9E%91%ED%92%88+%EC%B6%9C%ED%92%88+%EB%B0%8F+%EC%A0%84%EC%8B%9C" target="\_blank">RISE ALL FESTA</a> 작품 전시 및 굿즈 판매</div>

&#x20;       <div>• <span class="pf-highlight">2024\~2025</span> 지스타 학교 대표 부스 전시 참여</div>

&#x20;       <div>• <span class="pf-highlight">2022\~2023</span> 대한민국 육군 병장 만기전역</div>

&#x20;   </div>



&#x20;   <div class="pf-exp-section-title">📄 Certifications \& Media</div>

&#x20;   <div style="line-height: 1.8; padding-left: 10px; font-size: 0.95rem;">

&#x20;       <div>• <span class="pf-highlight">정보처리기능사</span> (Qnet, 2023.04.26) / <span class="pf-highlight">프롬프트 디자이너</span> (AbilityLAB, 2024.09.23)</div>

&#x20;       <div>• <span class="pf-highlight">게임메카 소개:</span> <a href="https://www.gamemeca.com/view.php?gid=1768224" target="\_blank">2025년 주목할 기대되는 인디작품 선정</a></div>

&#x20;       <div>• <span class="pf-highlight">스토브인디:</span> <a href="https://store.onstove.com/ko/games/103315" target="\_blank">파일타워 디펜스 데모판 입점</a></div>

&#x20;   </div>

</div>





&#x20;       <div class="pf-tab-container">

&#x20;           <button class="pf-tab-button active" onclick="pfSwitchTab('fileTowerDefense', 'refactoring')">리팩토링 및 개선</button>

&#x20;           <button class="pf-tab-button" onclick="pfSwitchTab('fileTowerDefense', 'existing')">입력 및 상호작용 시스템</button>

&#x20;       </div>

&#x20;       

&#x20;       <!-- Tab 1: Refactoring -->

&#x20;       <div id="pf-ftd-refactoring" class="pf-tab-content active">

&#x20;           <h1>시스템 리팩토링 및 개선</h1>

&#x20;           <h2>1. UI(RectTransform)에서 GameObject(Transform) 기반 전환</h2>

&#x20;           <p>바탕화면 아이콘 느낌 구현을 위해 초기엔 UI 시스템으로 개발하였으나, 다음과 같은 성능상 문제를 감지했습니다.</p>

&#x20;           <ul>

&#x20;               <li><span class="pf-highlight">좌표계 종속성:</span> RectTransform은 캔버스 앵커 설정에 따라 월드 좌표가 상대 변환됨</li>

&#x20;               <li><span class="pf-highlight">연산 비용 오버헤드:</span> 바이러스와 거리 계산 시 <code>WorldToScreenPoint</code> 연산 오버헤드 가속</li>

&#x20;           </ul>

&#x20;           <div class="pf-visual-frame">

&#x20;               <div style="display: flex; justify-content: space-around; align-items: center;">

&#x20;                   <div class="pf-mono" style="border: 1px solid #ff7b72; padding: 10px; font-size: 0.8rem;">

&#x20;                       RectTransform (UI)<br>WorldToScreen Required

&#x20;                   </div>

&#x20;                   <div class="pf-flow-arrow">>>></div>

&#x20;                   <div class="pf-mono" style="border: 1px solid var(--pf-sub-accent); padding: 10px; font-size: 0.8rem;">

&#x20;                       Transform (World)<br>Unified Physics2D Compatible

&#x20;                   </div>

&#x20;               </div>

&#x20;           </div>



&#x20;           <h2>2. FileGrid: 데이터 중심 버프 매니저</h2>

&#x20;           <p>HashSet을 이용해 셀 단위 점유 관리 및 버프 합산 시스템을 설계하여 무중단 중복 버프 제거를 수행합니다.</p>





&#x20;           <h2>3. 공간 분할 최적화 (O(n) -> O(1))</h2>

&#x20;           <p>주변 3x3 국소 영역만 탐색하게 차단하여 연산 루프 성능을 효율화했습니다.</p>

&#x20;           <div class="pf-visual-frame">

&#x20;               <div class="pf-diagram-grid">

&#x20;                   <div class="pf-grid-cell pf-near"></div><div class="pf-grid-cell pf-near"></div><div class="pf-grid-cell pf-near"></div>

&#x20;                   <div class="pf-grid-cell pf-near"></div><div class="pf-grid-cell pf-active"></div><div class="pf-grid-cell pf-near"></div>

&#x20;                   <div class="pf-grid-cell pf-near"></div><div class="pf-grid-cell pf-near"></div><div class="pf-grid-cell pf-near"></div>

&#x20;               </div>

&#x20;           </div>

&#x20;       </div>

&#x20;       

&#x20;       <!-- Tab 2: Interaction -->

&#x20;       <div id="pf-ftd-existing" class="pf-tab-content">

&#x20;           <h1>입력 및 상호작용 시스템</h1>

&#x20;           <h2>1. 중앙 집중식 입력 및 상호작용 시스템</h2>

&#x20;           <p>모든 이벤트를 단일 <code>InputManager</code>로 일원화하고, <code>IInteractable</code> 인터페이스 구현체에 이벤트를 전송하도록 연계했습니다.</p>

&#x20;           <div class="pf-visual-frame">

&#x20;               <div class="pf-transaction-flow pf-mono">

&#x20;                   <div class="pf-flow-step">Physics2D Raycast</div>

&#x20;                   <div class="pf-flow-arrow">→</div>

&#x20;                   <div class="pf-flow-step">InputManager Sorting</div>

&#x20;                   <div class="pf-flow-arrow">→</div>

&#x20;                   <div class="pf-flow-step">InteractionHandler Send</div>

&#x20;               </div>

&#x20;           </div>

&#x20;           <h2>2. 양방향 좌표 변환 시스템</h2>

&#x20;           <div class="pf-coord-flow">

&#x20;               <div class="pf-coord-box">

&#x20;                   <div class="pf-coord-box-title">World Pos</div>

&#x20;                   <div class="pf-coord-box-desc">마우스 월드 위치 수집</div>

&#x20;               </div>

&#x20;               <div class="pf-coord-arrow">→</div>

&#x20;               <div class="pf-coord-box">

&#x20;                   <div class="pf-coord-box-title">Local Pos</div>

&#x20;                   <div class="pf-coord-box-desc">InverseTransformPoint 적용</div>

&#x20;                   <div class="pf-coord-box-formula">localPos = Inverse(worldPos)</div>

&#x20;               </div>

&#x20;               <div class="pf-coord-arrow">→</div>

&#x20;               <div class="pf-coord-box">

&#x20;                   <div class="pf-coord-box-title">Grid Index</div>

&#x20;                   <div class="pf-coord-box-desc">셀 간격 연산 및 반올림</div>

&#x20;               </div>

&#x20;           </div>

&#x20;       </div>

&#x20;   </div>

</div>





&#x20;       <div class="pf-tab-container">

&#x20;           <button class="pf-tab-button active" onclick="pfSwitchTab('racing', 'nav')">네비게이션 및 보상설계</button>

&#x20;           <button class="pf-tab-button" onclick="pfSwitchTab('racing', 'sensor')">환경 감지 및 제어</button>

&#x20;       </div>



&#x20;       <!-- Tab 1: Nav \& Rewards -->

&#x20;       <div id="pf-racing-nav" class="pf-tab-content active">

&#x20;           <h1>ML-Agents 자율 주행 학습 시스템</h1>

&#x20;           <h2>1. 체크포인트 및 역주행 방지 설계</h2>

&#x20;           <p>단순 거리 주행 학습 시 발생하던 역주행 오류를 해결하고자 스플라인 기반의 체크포인트 자동 생성 시스템을 접목하고, 통과 순서를 검증하도록 하였습니다.</p>

&#x20;           <div class="pf-visual-frame">

&#x20;               <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 15px;">

&#x20;                   <div style="border: 1px solid var(--pf-sub-accent); padding: 15px; border-radius: 6px;">

&#x20;                       <h4 style="margin:0 0 10px 0; color: var(--pf-sub-accent);">목표 지향형 보상</h4>

&#x20;                       <span style="font-size:0.85rem; color:#8b949e;">• 체크포인트 통과: +1.0<br>• 피니시 도달: +10.0</span>

&#x20;                   </div>

&#x20;                   <div style="border: 1px solid #ff7b72; padding: 15px; border-radius: 6px;">

&#x20;                       <h4 style="margin:0 0 10px 0; color:#ff7b72;">안전성 페널티</h4>

&#x20;                       <span style="font-size:0.85rem; color:#8b949e;">• 벽 충돌: -1.0<br>• 역주행 속도 감지 시 페널티 적용</span>

&#x20;                   </div>

&#x20;               </div>

&#x20;           </div>

&#x20;       </div>



&#x20;       <!-- Tab 2: Sensor \& Actions -->

&#x20;       <div id="pf-racing-sensor" class="pf-tab-content">

&#x20;           <h1>환경 감지 및 제어</h1>

&#x20;           <h2>1. 36차원 관측 레이캐스트 센서</h2>

&#x20;           <p>전방 9방향(180도), 후방 3방향(60도)에 레이를 투사해 도로 경계 및 장애물 거리를 정규화하여 훈련 신경망에 공급합니다.</p>





&#x20;           <h2>2. 액션 스무딩 보정</h2>

&#x20;           <p>지터링 완화를 위해 이전 프레임의 가속/조향값을 활용한 입력 클램프(스무딩)를 탑재했습니다.</p>

&#x20;       </div>

&#x20;   </div>

</div>





&#x20;       <div class="pf-tab-container">

&#x20;           <button class="pf-tab-button active" onclick="pfSwitchTab('wfk', 'parsing')">CSV 동적 파싱</button>

&#x20;           <button class="pf-tab-button" onclick="pfSwitchTab('wfk', 'seed')">Seed Token 저장</button>

&#x20;       </div>



&#x20;       <!-- Tab 1: CSV -->

&#x20;       <div id="pf-wfk-parsing" class="pf-tab-content active">

&#x20;           <h1>동적 CSV 데이터 매니저</h1>

&#x20;           <h2>1. 리플렉션 기반 파서</h2>

&#x20;           <p>구조 변경 시마다 파서 코드를 수동 업데이트하지 않고, 제네릭과 리플렉션 기술로 필드 타입 자동 일치 변환을 적용했습니다.</p>





&#x20;       </div>



&#x20;       <!-- Tab 2: Seed -->

&#x20;       <div id="pf-wfk-seed" class="pf-tab-content">

&#x20;           <h1>Seed / Token 난수 복원</h1>

&#x20;           <h2>1. 난수 사용량 추적 시스템</h2>

&#x20;           <p>세이브/로드 시 난수 밸런스 이탈을 철저히 막기 위해, 시드 수치뿐 아니라 영역(Domain)별 실제 난수 방출 횟수를 추적하여 저장 및 빌드합니다.</p>

&#x20;           <div class="pf-visual-frame">

&#x20;               <span class="pf-mono" style="color: var(--pf-accent-color);">BaseSeed(10자) + Count(3자) + \[DomainID(3자) + Usage(12자)]</span>

&#x20;           </div>

&#x20;       </div>

&#x20;   </div>

</div>





&#x20;       <div class="pf-tab-container">

&#x20;           <button class="pf-tab-button active" onclick="pfSwitchTab('slime', 'fsm')">FSM 시스템</button>

&#x20;           <button class="pf-tab-button" onclick="pfSwitchTab('slime', 'boss')">보스 델리게이트 AI</button>

&#x20;       </div>



&#x20;       <!-- Tab 1: FSM -->

&#x20;       <div id="pf-slime-fsm" class="pf-tab-content active">

&#x20;           <h1>FSM (Finite State Machine) 아키텍처</h1>

&#x20;           <h2>1. 라이프사이클 캡슐화</h2>

&#x20;           <p>동작 무거움을 방지하기 위해 <code>Enter</code>, <code>Exit</code>, <code>LogicUpdate</code>, <code>PhysicsUpdate</code>로 상태 진출입 단계를 정교하게 분할 제어했습니다.</p>





&#x20;       </div>



&#x20;       <!-- Tab 2: Boss -->

&#x20;       <div id="pf-slime-boss" class="pf-tab-content">

&#x20;           <h1>델리게이트 기반 보스 AI</h1>

&#x20;           <h2>1. 함수 포인터형 기동</h2>

&#x20;           <p>지저분한 <code>if/switch</code> 분기 형태를 차단하고, 스킬 리스트를 함수 델리게이트 테이블로 배열화하여 무작위 가중치 제어로 대리 실행합니다.</p>

&#x20;           <p>보스 HP가 50% 미만으로 하강하면 자동으로 각 행동 계수에 업그레이드 배율 가중치를 상속 연동합니다.</p>

&#x20;       </div>

&#x20;   </div>

</div>



