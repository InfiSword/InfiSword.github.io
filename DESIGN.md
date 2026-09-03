---
name: Minhyuk Lee Portfolio (InfiSword)
description: High-performance game client engineer portfolio specializing in systems architecture, rendering, and reinforcement learning
colors:
  primary: "#2563eb"
  primary-soft: "rgba(37, 99, 235, 0.08)"
  primary-hover: "#1d4ed8"
  background-base: "#f8fafc"
  surface-card: "#ffffff"
  surface-strong: "#ffffff"
  text-primary: "#0f172a"
  text-secondary: "#334155"
  text-muted: "#64748b"
  border-subtle: "#e2e8f0"
  accent-green: "#059669"
  accent-sky: "#0284c7"
typography:
  display:
    fontFamily: "Plus Jakarta Sans, Noto Sans KR, system-ui, sans-serif"
    fontSize: "clamp(2rem, 4vw, 3.2rem)"
    fontWeight: 800
    lineHeight: 1.2
    letterSpacing: "-0.03em"
  headline:
    fontFamily: "Plus Jakarta Sans, Noto Sans KR, system-ui, sans-serif"
    fontSize: "1.75rem"
    fontWeight: 700
    lineHeight: 1.3
    letterSpacing: "-0.02em"
  title:
    fontFamily: "Plus Jakarta Sans, Noto Sans KR, system-ui, sans-serif"
    fontSize: "1.25rem"
    fontWeight: 700
    lineHeight: 1.4
  body:
    fontFamily: "Plus Jakarta Sans, Noto Sans KR, system-ui, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.72
  label:
    fontFamily: "Fira Code, monospace"
    fontSize: "0.85rem"
    fontWeight: 500
    letterSpacing: "0.04em"
rounded:
  sm: "8px"
  md: "12px"
  lg: "18px"
  full: "9999px"
spacing:
  xs: "4px"
  sm: "8px"
  md: "16px"
  lg: "24px"
  xl: "48px"
  2xl: "72px"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "#ffffff"
    rounded: "{rounded.sm}"
    padding: "10px 18px"
  card-technical:
    backgroundColor: "{colors.surface-card}"
    textColor: "{colors.text-primary}"
    rounded: "{rounded.lg}"
    padding: "24px"
---

# Design System: Minhyuk Lee Portfolio (InfiSword)

## Overview

**Creative North Star: "Swiss Technical Light Architecture"**

A high-contrast, clean white technical design language built for an elite Game Client Programmer specializing in Systems Architecture, Graphics Pipelines, and AI Reinforcement Learning. The interface is crisp, modern, and exceptionally legible—bright white canvas, tactile paper cards with delicate hairline borders, structured developer typography, and authoritative royal blue accents.

The design embodies rigorous engineering precision: dense with technical substance yet effortless to read. Precision grid lines evoke blueprint schematics, while developer monospace badges anchor the visitor in real production-grade code.

**Key Characteristics:**
- **Clean White Canvas**: Crisp off-white background (`#f8fafc` / `#ffffff`) with subtle 48px schematic grid lines (`linear-gradient(rgba(148, 163, 184, 0.15) 1px, transparent 1px)`).
- **Tactile Paper Cards**: Crisp white surfaces (`#ffffff`) with 1px hairline borders (`#e2e8f0`), subtle elevation (`box-shadow: 0 4px 20px rgba(15, 23, 42, 0.05)`), and solid blue indicator accents (`#2563eb`).
- **Dual Type Stack**: Geometric modern sans (`Plus Jakarta Sans`) paired with developer-grade monospace (`Fira Code`) for metrics, dates, and architectural tags.
- **Cinematic Micro-Motion (GSAP)**: Hardware-accelerated entrance staggers (`power3.out`) and scroll-triggered card reveals (`power2.out`) that feel weighted, snappy, and responsive.

---

## Colors

The palette is tuned for maximum contrast (WCAG AAA) on light surfaces.

### Primary
- **Royal Blue** (`#2563eb`): The primary visual signal. Used for interactive links, active buttons, left card indicator bars, and primary callouts.
- **Blue Soft Tint** (`#eff6ff`): Subtle background tint for tech chips and metadata pills.

### Typography
- **Deep Obsidian Charcoal** (`#0f172a`): Used for hero title, card titles, and modal headlines. 명도 대비 16.5:1.
- **Dark Slate Body** (`#334155` / `#475569`): Used for project descriptions, bio, and secondary text. 명도 대비 7.5:1 이상.
- **Slate Muted** (`#64748b`): For timestamps, minor metadata labels, and footer marks.

### Borders & Dividers
- **Hairline Border** (`#e2e8f0`): Structural containment.
- **Active Border** (`#93c5fd`): Hover state illumination.
- **Frosted Midnight Glass** (`rgba(13, 18, 34, 0.88)`): Primary surface fill for cards, headers, and modal dialogs.
- **Solid Void Blue** (`#0d1222`): Opaque fallbacks and deep structural backgrounds.
- **Luminous Polar White** (`#eef4ff`): High-clarity primary text color for maximum legibility.
- **Slate Silver** (`#94a3b8`): Secondary text color for descriptions, metadata, and captions.
- **Hairline Hologram Border** (`rgba(148, 163, 184, 0.17)`): Single-pixel perimeter strokes delineating floating surfaces.

### Named Rules
**The 10% Luminous Rule.** Saturated electric blue (`#60a5fa`) is strictly reserved for key interactive focal points and status indicators, occupying no more than 10% of any viewport. Its scarcity preserves its high-value punch.

**The No Pure Black Rule.** Pure `#000000` is prohibited for broad background fills; `#050711` with procedural radial color accents must always be used to maintain depth and optical realism.

---

## Typography

**Display Font:** Plus Jakarta Sans (fallback: system-ui, -apple-system, sans-serif)  
**Body Font:** Plus Jakarta Sans & Noto Sans KR (for seamless bilingual English/Korean reading)  
**Label/Code Font:** Fira Code (monospace)  

**Character:** Technical authority meets modern editorial polish. Clean geometric grotesque shapes convey engineering discipline, while Fira Code provides unmistakable developer authenticity for architectural notations.

### Hierarchy
- **Display** (800 weight, `clamp(2rem, 4vw, 3.2rem)`, line-height `1.2`, letter-spacing `-0.03em`): Hero headlines. Features subtle white-to-blue gradient masking.
- **Headline** (700 weight, `1.75rem`, line-height `1.3`, letter-spacing `-0.02em`): Section headings (Projects, Architecture, Experience).
- **Title** (700 weight, `1.25rem`, line-height `1.4`): Project card titles and modal headers.
- **Body** (400/500 weight, `1rem`, line-height `1.72`): Paragraph descriptions, technical explanations. Max line width 75ch.
- **Label / Tag** (500 weight, `0.75rem - 0.85rem`, letter-spacing `0.04em`): Fira Code monospace tags, technical stack chips, dates, and metrics.

### Named Rules
**The Developer Monospace Stamp Rule.** All metadata, years, execution times, performance figures, and architecture labels must be rendered in `Fira Code`. Monospace serves as the visual signature of verified code and data.

---

## Layout

**Container Architecture:** Maximum content boundary of `1320px` (`--container: 1320px`), centered horizontally with responsive safety padding (`calc(100% - 48px)`).

**The Coordinate Grid Overlay:** The background renders a 48px × 48px subtle blueprint coordinate grid (`linear-gradient(rgba(148, 163, 184, 0.07) 1px, transparent 1px)`) masked with a vertical fade, establishing the feeling of a real-time graphics workspace.

**Responsive Grid Breakpoints:**
- **Desktop** (≥ 1024px): Multi-column project showcases, side-by-side architecture telemetry, split hero headers.
- **Tablet** (768px – 1023px): Two-column adaptive grid, sticky top navigation.
- **Mobile** (< 768px): Single-column stack, touch-optimized hit targets (min 44px), horizontal scrollable filter tabs.

---

## Elevation & Depth

Surfaces achieve depth not through heavy black drop shadows, but through **optical glass layering** and **colored ambient light diffusion**.

### Shadow Vocabulary
- **Card Float Shadow** (`box-shadow: 0 24px 70px rgba(0, 0, 0, 0.3)`): Ambient grounding for floating glass containers.
- **Modal Immersion Shadow** (`box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.8)`): High-contrast elevation for active dialog viewports.
- **Accent Glow Hover** (`box-shadow: 0 12px 28px rgba(37, 99, 235, 0.22)`): Luminous back-scatter emitted when hovering interactive buttons.

### Named Rules
**The Glass Horizon Rule.** Floating surfaces must combine `backdrop-filter: blur(20px)`, semi-transparent fill (`rgba(13, 18, 34, 0.88)`), and a 1px hairline border (`rgba(148, 163, 184, 0.17)`). One property without the other breaks the physical illusion.

---

## Shapes

- **Primary Containers & Modals**: Smooth `18px` – `28px` corner radiuses (`--radius: 18px`).
- **Media & Screenshots**: `12px` rounded radius with crisp border isolation.
- **Interactive Controls & Buttons**: `8px` – `10px` compact radiuses for tactical clickability.
- **Status Badges & Pills**: `9999px` full capsule radius.

---

## Components

### 1. Site Header
- **Structure**: Sticky navigation bar (`position: sticky; top: 0; z-index: 50;`).
- **Styling**: `rgba(5, 7, 17, 0.78)` with `backdrop-filter: blur(20px) saturate(140%)` and a 1px bottom border.
- **Navigation**: Minimalist text links with smooth accent hover color transitions.

### 2. Project Cards
- **Structure**: High-density glass panel displaying project thumbnail, title, technical kicker, excerpt, and tech badges.
- **Hover State**: `-2px` Y-axis lift, border highlight shift (`rgba(96, 165, 250, 0.5)`), and soft radial blue glow reflection.

### 3. Modals & Deep Dives
- **Structure**: Fixed backdrop overlay (`rgba(3, 5, 12, 0.85)`) with full-viewport blur, containing an animated modal card (`pf-modal-in`).
- **Accessibility**: Traps focus, closes on `Escape` key, restores trigger focus on close.

### 4. Motion Architecture (GSAP & ScrollTrigger)
- **Engine**: Powered by GreenSock Animation Platform (`gsap` + `ScrollTrigger`).
- **Hero Reveal**: Sequenced stagger (`0.08s`) across title, description, and action pills using `power3.out` easing.
- **Scroll Elevation**: Project cards and section milestones glide upward with subtle opacity reveal (`y: 24`, `duration: 0.65`, `ease: power2.out`) as they enter 88% of the viewport.
- **Reduced Motion Guard**: Automatically checks `(prefers-reduced-motion: reduce)` and bypasses transforms for users requesting zero motion.

---

## Do's and Don'ts

### Do:
- **Do** wrap all architecture notes, algorithms, metrics, and years in `Fira Code` tags.
- **Do** keep cards and dialogs on frosted glass surfaces with hairline borders.
- **Do** use GSAP for performance-critical, GPU-accelerated motion with `power2.out` or `power3.out` curves.
- **Do** ensure all interactive clickable elements have a minimum touch target size of 44px.
- **Do** verify and maintain responsive layout across desktop and mobile devices via Playwright tests.

### Don't:
- **Don't** use generic flat grey shadows or solid opaque `#000000` panels.
- **Don't** use decorative color gradients on heading or body text (The Solid Authority Rule).
- **Don't** use emojis as interface or feature icons (The No-Emoji-As-Icon Rule). Use developer typography (`Fira Code`), text labels, or clean SVG vectors instead.
- **Don't** use overly bouncy, cartoony easing (e.g. elastic/bounce) that conflicts with the serious engineering aesthetic.
- **Don't** clutter project cards with non-technical marketing fluff; highlight technical problems, rendering techniques, and performance optimizations.
- **Don't** allow horizontal scroll overflow on mobile viewports.
