const { chromium } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

async function main() {
  const browser = await chromium.launch();
  const page = await browser.newPage({
    viewport: { width: 1200, height: 675 },
    deviceScaleFactor: 2
  });

  const htmlContent = `
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; }
  body {
    width: 1200px;
    height: 675px;
    background: #090d16;
    color: #f8fafc;
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
    overflow: hidden;
    position: relative;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    padding: 36px 44px;
  }
  
  /* Blueprint Grid */
  .grid-bg {
    position: absolute;
    inset: 0;
    background-image: 
      linear-gradient(rgba(59, 130, 246, 0.08) 1px, transparent 1px),
      linear-gradient(90deg, rgba(59, 130, 246, 0.08) 1px, transparent 1px);
    background-size: 40px 40px;
    pointer-events: none;
  }

  .radial-glow {
    position: absolute;
    width: 700px;
    height: 700px;
    border-radius: 50%;
    background: radial-gradient(circle, rgba(37, 99, 235, 0.18) 0%, transparent 70%);
    top: 50%;
    left: 45%;
    transform: translate(-50%, -50%);
    pointer-events: none;
  }

  /* Header */
  .header {
    position: relative;
    z-index: 10;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
  }
  .kicker {
    font-family: "Fira Code", monospace;
    font-size: 13px;
    font-weight: 700;
    color: #60a5fa;
    letter-spacing: 0.12em;
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 8px;
  }
  .pulse {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: #3b82f6;
    box-shadow: 0 0 10px #3b82f6;
  }
  .title {
    font-size: 38px;
    font-weight: 800;
    letter-spacing: -0.03em;
    color: #ffffff;
    line-height: 1.15;
  }
  .subtitle {
    font-size: 16px;
    color: #94a3b8;
    margin-top: 6px;
    font-weight: 500;
  }
  
  .tech-badges {
    display: flex;
    gap: 8px;
  }
  .badge {
    font-family: "Fira Code", monospace;
    font-size: 12px;
    padding: 5px 12px;
    border-radius: 6px;
    background: rgba(30, 41, 59, 0.85);
    border: 1px solid rgba(148, 163, 184, 0.25);
    color: #cbd5e1;
    font-weight: 600;
  }
  .badge-primary {
    background: rgba(37, 99, 235, 0.2);
    border-color: rgba(96, 165, 250, 0.4);
    color: #93c5fd;
  }

  /* Simulation Canvas SVG */
  .sim-container {
    position: absolute;
    inset: 0;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  /* ImGui Window Overlay */
  .imgui-window {
    position: absolute;
    right: 44px;
    top: 130px;
    width: 290px;
    background: rgba(20, 24, 38, 0.92);
    border: 1px solid #3b82f6;
    border-radius: 6px;
    box-shadow: 0 12px 36px rgba(0, 0, 0, 0.6);
    font-family: "Segoe UI", Tahoma, sans-serif;
    font-size: 13px;
    color: #e2e8f0;
    z-index: 15;
    backdrop-filter: blur(12px);
  }
  .imgui-titlebar {
    background: #1e3a8a;
    padding: 6px 12px;
    font-weight: 700;
    font-size: 12px;
    color: #ffffff;
    border-top-left-radius: 5px;
    border-top-right-radius: 5px;
    display: flex;
    justify-content: space-between;
  }
  .imgui-body {
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
  .imgui-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 6px;
  }
  .imgui-btn {
    background: #1e293b;
    border: 1px solid #475569;
    color: #f1f5f9;
    padding: 3px 8px;
    border-radius: 4px;
    font-size: 12px;
    font-family: "Fira Code", monospace;
  }
  .imgui-input {
    background: #0f172a;
    border: 1px solid #334155;
    color: #38bdf8;
    padding: 3px 8px;
    border-radius: 4px;
    width: 60px;
    text-align: center;
    font-family: "Fira Code", monospace;
    font-size: 12px;
  }
  .imgui-action-btn {
    width: 100%;
    background: #2563eb;
    border: 1px solid #60a5fa;
    color: #ffffff;
    font-weight: 700;
    padding: 6px;
    border-radius: 4px;
    font-size: 12px;
    text-align: center;
    margin-top: 4px;
  }

  /* Bottom Metrics Bar */
  .footer-bar {
    position: relative;
    z-index: 10;
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    padding-top: 16px;
    border-top: 1px solid rgba(148, 163, 184, 0.15);
    font-family: "Fira Code", monospace;
    font-size: 12px;
    color: #64748b;
  }
  .metric-group {
    display: flex;
    gap: 24px;
  }
  .metric-item {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }
  .metric-label {
    font-size: 10px;
    color: #475569;
    letter-spacing: 0.08em;
  }
  .metric-val {
    color: #38bdf8;
    font-weight: 600;
  }
</style>
</head>
<body>

  <div class="grid-bg"></div>
  <div class="radial-glow"></div>

  <!-- Header -->
  <div class="header">
    <div>
      <div class="kicker">
        <span class="pulse"></span>
        <span>KRAFTON JUNGLE GAMETECH // ADMISSION TEST</span>
      </div>
      <h1 class="title">DirectX 11 Pinball &amp; Physics Simulation</h1>
      <p class="subtitle">2D/3D Elastic Collision Engine, Momentum Conservation &amp; Real-time ImGui Controls</p>
    </div>
    <div class="tech-badges">
      <span class="badge badge-primary">DirectX 11</span>
      <span class="badge">HLSL</span>
      <span class="badge">C++</span>
      <span class="badge">Dear ImGui</span>
    </div>
  </div>

  <!-- SVG Physics & Graphics Simulation Schematics -->
  <svg class="sim-container" width="1200" height="675" viewBox="0 0 1200 675" fill="none" xmlns="http://www.w3.org/2000/svg">
    <!-- Viewport Boundary Wireframe Box -->
    <rect x="200" y="110" width="580" height="420" rx="8" stroke="#1e3a8a" stroke-width="2" stroke-dasharray="6 4" fill="rgba(15, 23, 42, 0.4)"/>
    <text x="215" y="132" fill="#475569" font-family="Fira Code" font-size="11">VIEWPORT BOUNDS [-1.0, 1.0] (Wall Restitution e = 0.9)</text>

    <!-- Wall Collision Indicator Left -->
    <g transform="translate(200, 280)">
      <circle r="22" fill="url(#ballGold)" filter="drop-shadow(0 0 12px rgba(245, 158, 11, 0.5))"/>
      <line x1="0" y1="0" x2="45" y2="30" stroke="#f59e0b" stroke-width="2.5"/>
      <line x1="0" y1="-25" x2="0" y2="25" stroke="#ef4444" stroke-width="3"/>
      <text x="20" y="-12" fill="#fbbf24" font-family="Fira Code" font-size="10">Wall Bounce v' = -v * 0.9</text>
    </g>

    <!-- Ball 1 (Amber Sphere) -->
    <defs>
      <radialGradient id="ballGold" cx="30%" cy="30%" r="70%">
        <stop offset="0%" stop-color="#fef3c7"/>
        <stop offset="40%" stop-color="#f59e0b"/>
        <stop offset="80%" stop-color="#d97706"/>
        <stop offset="100%" stop-color="#78350f"/>
      </radialGradient>
    </defs>

    <!-- Normal Sphere 1 (Blue Metallic 3D Sphere) -->
    <g transform="translate(420, 240)">
      <defs>
        <radialGradient id="ballBlue" cx="30%" cy="30%" r="70%">
          <stop offset="0%" stop-color="#bae6fd"/>
          <stop offset="40%" stop-color="#38bdf8"/>
          <stop offset="80%" stop-color="#0284c7"/>
          <stop offset="100%" stop-color="#075985"/>
        </radialGradient>
      </defs>
      <circle r="34" fill="url(#ballBlue)" filter="drop-shadow(0 0 14px rgba(56, 189, 248, 0.6))"/>
      
      <!-- Velocity vector -->
      <line x1="34" y1="10" x2="90" y2="35" stroke="#38bdf8" stroke-width="3"/>
      <polygon points="90,35 78,31 82,23" fill="#38bdf8"/>
      <text x="35" y="-12" fill="#38bdf8" font-family="Fira Code" font-size="11">Velocity v1 (Mass m1)</text>
    </g>

    <!-- Normal Sphere 2 (Cyan 3D Sphere) -->
    <g transform="translate(560, 310)">
      <defs>
        <radialGradient id="ballCyan" cx="30%" cy="30%" r="70%">
          <stop offset="0%" stop-color="#a7f3d0"/>
          <stop offset="40%" stop-color="#34d399"/>
          <stop offset="80%" stop-color="#059669"/>
          <stop offset="100%" stop-color="#064e3b"/>
        </radialGradient>
      </defs>
      <circle r="26" fill="url(#ballCyan)" filter="drop-shadow(0 0 12px rgba(52, 211, 153, 0.5))"/>
      
      <!-- Velocity vector -->
      <line x1="-26" y1="-10" x2="-80" y2="-35" stroke="#34d399" stroke-width="3"/>
      <polygon points="-80,-35 -68,-31 -72,-23" fill="#34d399"/>
      <text x="-140" y="45" fill="#34d399" font-family="Fira Code" font-size="11">Velocity v2 (Mass m2)</text>
    </g>

    <!-- Ball 3 (Purple 3D Sphere) -->
    <g transform="translate(650, 200)">
      <defs>
        <radialGradient id="ballPurple" cx="30%" cy="30%" r="70%">
          <stop offset="0%" stop-color="#e9d5ff"/>
          <stop offset="40%" stop-color="#a855f7"/>
          <stop offset="80%" stop-color="#7e22ce"/>
          <stop offset="100%" stop-color="#4c1d95"/>
        </radialGradient>
      </defs>
      <circle r="28" fill="url(#ballPurple)" filter="drop-shadow(0 0 12px rgba(168, 85, 247, 0.5))"/>
      <line x1="-25" y1="20" x2="-65" y2="50" stroke="#a855f7" stroke-width="2.5"/>
      <polygon points="-65,50 -55,44 -58,38" fill="#a855f7"/>
      <text x="35" y="5" fill="#c084fc" font-family="Fira Code" font-size="10">v3</text>
    </g>

    <!-- Elastic Collision Point & Normal Marker -->
    <g transform="translate(485, 275)">
      <circle r="8" fill="none" stroke="#60a5fa" stroke-width="2" stroke-dasharray="2 2"/>
      <line x1="-35" y1="-30" x2="35" y2="30" stroke="#f1f5f9" stroke-width="1.5" stroke-dasharray="3 3"/>
      <text x="10" y="-14" fill="#93c5fd" font-family="Fira Code" font-size="11" font-weight="700">Impulse J = -Δv(1+e)/(1/m1+1/m2)</text>
      <text x="10" y="4" fill="#64748b" font-family="Fira Code" font-size="9">Penetration Depth Correction</text>
    </g>
  </svg>

  <!-- Dear ImGui Overlay Window -->
  <div class="imgui-window">
    <div class="imgui-titlebar">
      <span>Jungle Property Window</span>
      <span>✕</span>
    </div>
    <div class="imgui-body">
      <div class="imgui-row">
        <div class="imgui-input">12</div>
        <button class="imgui-btn">-</button>
        <button class="imgui-btn">+</button>
        <span>Number of Balls</span>
      </div>
      <div class="imgui-row" style="justify-content: flex-start; gap: 8px;">
        <input type="checkbox" checked style="accent-color: #2563eb;">
        <span>Gravity [9.8 m/s²]</span>
      </div>
    </div>
  </div>

  <!-- Footer Bar -->
  <div class="footer-bar">
    <div class="metric-group">
      <div class="metric-item">
        <span class="metric-label">GRAPHICS API</span>
        <span class="metric-val">DirectX 11 (D3D11 / HLSL)</span>
      </div>
      <div class="metric-item">
        <span class="metric-label">PHYSICS RESTITUTION</span>
        <span class="metric-val">e = 1.0 (Ball) / e = 0.9 (Wall)</span>
      </div>
      <div class="metric-item">
        <span class="metric-label">FRAME PACING</span>
        <span class="metric-val">QPC 30 FPS Fixed Loop</span>
      </div>
      <div class="metric-item">
        <span class="metric-label">GUI INTEGRATION</span>
        <span class="metric-val">Dear ImGui Win32/DX11</span>
      </div>
    </div>
    <div>
      <span style="color: #94a3b8;">MINHYUK LEE // TECHNICAL ARCHITECTURE</span>
    </div>
  </div>

</body>
</html>
  `;

  await page.setContent(htmlContent);
  const outPath = path.resolve(__dirname, '..', 'assets', 'images', 'Pinball', 'pinball_title.png');
  await page.screenshot({ path: outPath, type: 'png' });
  await browser.close();
  console.log("Successfully generated:", outPath);
}

main().catch(err => {
  console.error(err);
  process.exit(1);
});
