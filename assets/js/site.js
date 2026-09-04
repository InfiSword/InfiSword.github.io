(function () {
  // GSAP Animation Suite
  if (typeof window.gsap !== "undefined") {
    const gsap = window.gsap;
    if (typeof window.ScrollTrigger !== "undefined") {
      gsap.registerPlugin(window.ScrollTrigger);
    }

    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    if (!prefersReducedMotion) {
      // Hero section stagger reveal
      const hero = document.querySelector(".pf-hero, .project-hero");
      if (hero) {
        gsap.from(hero.children, {
          opacity: 0,
          y: 20,
          duration: 0.8,
          stagger: 0.08,
          ease: "power3.out",
        });
      }

      // Scroll-triggered subtle elevation reveal for cards & sections
      const scrollTargets = document.querySelectorAll(
        ".pf-project-card, .pf-exp-col, .project-content > section, .project-content > h2"
      );
      if (scrollTargets.length && window.ScrollTrigger) {
        scrollTargets.forEach((target) => {
          gsap.from(target, {
            scrollTrigger: {
              trigger: target,
              start: "top 88%",
              once: true,
            },
            opacity: 0,
            y: 24,
            duration: 0.65,
            ease: "power2.out",
          });
        });
      }
    }
  }

  let modalTrigger = null;

  window.pfOpenModal = function (modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    modalTrigger = document.activeElement;
    modal.style.display = "flex";
    modal.setAttribute("aria-hidden", "false");
    document.body.style.overflow = "hidden";
    const closeButton = modal.querySelector(".pf-modal-close");
    if (closeButton) closeButton.focus();
  };

  window.pfCloseModal = function (modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    modal.style.display = "none";
    modal.setAttribute("aria-hidden", "true");
    document.body.style.overflow = "";
    if (modalTrigger && typeof modalTrigger.focus === "function") modalTrigger.focus();
    modalTrigger = null;
  };

  document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape") return;
    document.querySelectorAll(".pf-modal-overlay").forEach(function (modal) {
      modal.style.display = "none";
      modal.setAttribute("aria-hidden", "true");
    });
    document.body.style.overflow = "";
    if (modalTrigger && typeof modalTrigger.focus === "function") modalTrigger.focus();
    modalTrigger = null;
  });

  const tocLinks = Array.from(document.querySelectorAll(".toc-list a"));
  if (tocLinks.length) {
    const targets = tocLinks
      .map((link) => document.getElementById(decodeURIComponent(link.hash.slice(1))))
      .filter(Boolean);

    const observer = new IntersectionObserver(
      function (entries) {
        const visible = entries
          .filter((entry) => entry.isIntersecting)
          .sort((a, b) => a.boundingClientRect.top - b.boundingClientRect.top)[0];
        if (!visible) return;

        tocLinks.forEach((link) => link.classList.remove("is-active"));
        const active = tocLinks.find((link) => link.hash.slice(1) === visible.target.id);
        if (active) active.classList.add("is-active");
      },
      { rootMargin: "-90px 0px -70% 0px", threshold: 0.01 }
    );

    targets.forEach((target) => observer.observe(target));
  }

  if (document.querySelector(".mermaid")) {
    const script = document.createElement("script");
    script.src = "https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.min.js";
    script.onload = function () {
      window.mermaid.initialize({
        startOnLoad: true,
        theme: "neutral",
        securityLevel: "loose",
      });
    };
    document.head.appendChild(script);
  }

  // ==========================================
  // Windows 11 Loading Bar & Enter-to-Enter Suite
  // ==========================================
  const winScreen = document.getElementById("win-lock-screen");
  if (winScreen) {
    const lockBtn = document.getElementById("win-lock-btn");
    const progressBar = document.getElementById("win-progress-bar");
    const progressWrapper = document.getElementById("win-progress-wrapper");
    const progressStatus = document.getElementById("win-progress-status");
    const actionPrompt = document.getElementById("win-action-prompt");
    let isLoaded = false;
    let isTransitioning = false;

    function runLoadingSequence() {
      isLoaded = false;
      isTransitioning = false;
      if (progressBar) progressBar.style.width = "0%";
      if (progressWrapper) progressWrapper.style.display = "flex";
      if (progressStatus) progressStatus.textContent = "시스템 초기화 중...";
      if (actionPrompt) actionPrompt.style.display = "none";

      if (window.gsap && progressBar) {
        window.gsap.to(progressBar, {
          width: "100%",
          duration: 1.2,
          ease: "power1.inOut",
          onComplete: () => {
            onLoadingComplete();
          },
        });
      } else {
        setTimeout(onLoadingComplete, 1200);
      }
    }

    function onLoadingComplete() {
      isLoaded = true;
      if (progressWrapper) progressWrapper.style.display = "none";
      if (actionPrompt) {
        actionPrompt.style.display = "inline-flex";
        if (window.gsap) {
          window.gsap.fromTo(
            actionPrompt,
            { scale: 0.92, opacity: 0 },
            { scale: 1, opacity: 1, duration: 0.35, ease: "back.out(1.6)" }
          );
        }
      }
    }

    // Check if user already unlocked in this session
    const isUnlocked = sessionStorage.getItem("win_unlocked") === "true";
    if (isUnlocked) {
      winScreen.style.display = "none";
      document.body.classList.remove("is-locked");
    } else {
      document.body.classList.add("is-locked");
      runLoadingSequence();
    }

    window.winEnterPortfolio = function () {
      if (isTransitioning) return;
      isTransitioning = true;
      sessionStorage.setItem("win_unlocked", "true");

      if (window.gsap) {
        window.gsap.killTweensOf(progressBar);
        window.gsap.to(winScreen, {
          yPercent: -100,
          duration: 0.75,
          ease: "power3.inOut",
          onComplete: () => {
            winScreen.style.display = "none";
            document.body.classList.remove("is-locked");
            isTransitioning = false;
            const hero = document.querySelector(".pf-hero");
            if (hero) {
              window.gsap.from(hero.children, {
                opacity: 0,
                y: 24,
                duration: 0.7,
                stagger: 0.08,
                ease: "power3.out",
              });
            }
          },
        });
      } else {
        winScreen.style.display = "none";
        document.body.classList.remove("is-locked");
        isTransitioning = false;
      }
    };

    window.winReLock = function () {
      sessionStorage.removeItem("win_unlocked");
      document.body.classList.add("is-locked");
      winScreen.style.display = "flex";
      if (window.gsap) {
        window.gsap.fromTo(
          winScreen,
          { yPercent: -100, opacity: 1 },
          {
            yPercent: 0,
            duration: 0.65,
            ease: "power3.out",
            onComplete: () => {
              runLoadingSequence();
            },
          }
        );
      } else {
        runLoadingSequence();
      }
    };

    // Click anywhere on screen to enter
    winScreen.addEventListener("click", function () {
      window.winEnterPortfolio();
    });

    if (lockBtn) {
      lockBtn.addEventListener("click", window.winReLock);
    }

    // Keyboard navigation: Enter, Space, or any key after loaded
    document.addEventListener("keydown", function (e) {
      if (winScreen.style.display === "none") {
        if ((e.key === "l" || e.key === "L") && (e.altKey || e.ctrlKey)) {
          e.preventDefault();
          window.winReLock();
        }
        return;
      }

      if (e.key === "Enter" || e.key === " " || isLoaded) {
        e.preventDefault();
        window.winEnterPortfolio();
      }
    });
  }

  // ==========================================
  // File Tower Defense Interactive Screenshot Gallery Suite
  // ==========================================
  window.pfFtdGallerySelect = function (index, caller) {
    const galleries = getGalleries(caller);
    galleries.forEach((gallery) => {
      renderGalleryByIndex(gallery, index);
    });
  };

  window.pfFtdGalleryGo = function (delta, caller) {
    const galleries = getGalleries(caller);
    galleries.forEach((gallery) => {
      const thumbs = gallery.querySelectorAll(".pf-ftd-thumb-card");
      if (!thumbs.length) return;
      const currentActive = gallery.querySelector(".pf-ftd-thumb-card.is-active");
      let currentIndex = currentActive ? parseInt(currentActive.getAttribute("data-index") || "0", 10) : 0;
      let nextIndex = (currentIndex + delta + thumbs.length) % thumbs.length;
      renderGalleryByIndex(gallery, nextIndex);
    });
  };

  function getGalleries(caller) {
    if (caller) {
      const container = typeof caller === "string" ? document.getElementById(caller) : caller.closest(".pf-ftd-gallery");
      if (container) return [container];
    }
    const all = document.querySelectorAll(".pf-ftd-gallery");
    return Array.from(all);
  }

  function renderGalleryByIndex(gallery, index) {
    const thumbs = gallery.querySelectorAll(".pf-ftd-thumb-card");
    if (!thumbs.length || index < 0 || index >= thumbs.length) return;

    const targetThumb = thumbs[index];
    const thumbImg = targetThumb.querySelector("img");
    const newSrc = thumbImg ? thumbImg.getAttribute("src") : "";
    const title = targetThumb.getAttribute("data-title") || (thumbImg ? thumbImg.getAttribute("alt") : "");

    const mainImg = gallery.querySelector(".pf-ftd-main-img");
    const counter = gallery.querySelector(".pf-ftd-counter");
    const caption = gallery.querySelector(".pf-ftd-caption");

    if (mainImg && newSrc) {
      mainImg.style.opacity = "0.35";
      mainImg.src = newSrc;
      if (title) mainImg.alt = title;
      mainImg.onload = function () {
        mainImg.style.opacity = "1";
      };
      if (mainImg.complete) {
        mainImg.style.opacity = "1";
      }
    }

    if (counter) {
      const padIndex = String(index + 1).padStart(2, "0");
      const padTotal = String(thumbs.length).padStart(2, "0");
      counter.textContent = `${padIndex} / ${padTotal}`;
    }

    if (caption && title) {
      caption.textContent = title;
    }

    thumbs.forEach((thumb, idx) => {
      if (idx === index) {
        thumb.classList.add("is-active");
        thumb.scrollIntoView({ behavior: "smooth", block: "nearest", inline: "center" });
      } else {
        thumb.classList.remove("is-active");
      }
    });
  }

  function initFtdGalleries() {
    const galleries = document.querySelectorAll(".pf-ftd-gallery");
    galleries.forEach((gallery) => {
      const strip = gallery.querySelector(".pf-ftd-thumbs-strip");
      if (strip && !strip.dataset.wheelBound) {
        strip.dataset.wheelBound = "true";
        strip.addEventListener(
          "wheel",
          function (e) {
            if (e.deltaY !== 0) {
              e.preventDefault();
              strip.scrollLeft += e.deltaY;
            }
          },
          { passive: false }
        );
      }

      const stage = gallery.querySelector(".pf-ftd-stage");
      if (stage && !stage.dataset.touchBound) {
        stage.dataset.touchBound = "true";
        let touchStartX = 0;
        let touchEndX = 0;

        stage.addEventListener(
          "touchstart",
          function (e) {
            if (e.changedTouches && e.changedTouches[0]) {
              touchStartX = e.changedTouches[0].screenX;
            }
          },
          { passive: true }
        );

        stage.addEventListener(
          "touchend",
          function (e) {
            if (e.changedTouches && e.changedTouches[0]) {
              touchEndX = e.changedTouches[0].screenX;
              const diff = touchEndX - touchStartX;
              if (Math.abs(diff) > 40) {
                if (diff < 0) {
                  window.pfFtdGalleryGo(1, gallery);
                } else {
                  window.pfFtdGalleryGo(-1, gallery);
                }
              }
            }
          },
          { passive: true }
        );
      }
    });
  }

  // Keyboard navigation for active gallery in open modal or in viewport
  document.addEventListener("keydown", function (e) {
    if (e.key !== "ArrowLeft" && e.key !== "ArrowRight") return;

    // Check active open modal first
    const activeModal = document.querySelector(".pf-modal-overlay.is-active, .pf-modal-overlay[style*='display: flex']");
    if (activeModal) {
      const modalGallery = activeModal.querySelector(".pf-ftd-gallery");
      if (modalGallery) {
        window.pfFtdGalleryGo(e.key === "ArrowRight" ? 1 : -1, modalGallery);
        return;
      }
    }

    // Check visible post galleries in viewport
    const postGalleries = document.querySelectorAll(".pf-ftd-post-gallery-section .pf-ftd-gallery");
    for (let i = 0; i < postGalleries.length; i++) {
      const gallery = postGalleries[i];
      const rect = gallery.getBoundingClientRect();
      const inView = rect.top < window.innerHeight && rect.bottom > 0;
      if (inView) {
        window.pfFtdGalleryGo(e.key === "ArrowRight" ? 1 : -1, gallery);
        break;
      }
    }
  });

  function initSpatialSimulation() {
    const container = document.querySelector('.pf-diagram-frame');
    if (!container) return;

    const svg = container.querySelector('svg');
    const viewportDrag = document.getElementById('ds-sim-viewport-drag');
    const viewportRect = document.getElementById('ds-viewport-rect');
    const viewportBadgeBg = document.getElementById('ds-viewport-badge-bg');
    const viewportBadgeTxt = document.getElementById('ds-viewport-badge-txt');
    const activeGridBox = document.getElementById('ds-active-grid-box');
    const activeGridTag = document.getElementById('ds-active-grid-tag');
    const activeGridTagTxt = document.getElementById('ds-active-grid-tag-txt');
    const callouts = document.getElementById('ds-sim-callouts');

    const hudCam = document.getElementById('ds-sim-cam');
    const hudCells = document.getElementById('ds-sim-cells');
    const hudRendered = document.getElementById('ds-sim-rendered');
    const hudCulled = document.getElementById('ds-sim-culled');
    const hudRatio = document.getElementById('ds-sim-ratio');
    const btnReset = document.getElementById('ds-btn-reset');

    if (!svg || !viewportDrag || !viewportRect || !activeGridBox) return;

    // Grid column and row boundary coordinates (6 columns x 5 rows)
    const colBounds = [30.0, 176.67, 323.33, 470.0, 616.67, 763.33, 910.0];
    const rowBounds = [75.0, 170.0, 265.0, 360.0, 455.0, 550.0];

    // Viewport dimensions in SVG coordinates (420 x 230 px)
    const VP_WIDTH = 420;
    const VP_HEIGHT = 230;
    const MIN_X = 30;
    const MAX_X = 910 - VP_WIDTH; // 490
    const MIN_Y = 75;
    const MAX_Y = 550 - VP_HEIGHT; // 320

    const DEFAULT_X = 260;
    const DEFAULT_Y = 195;
    let curX = DEFAULT_X;
    let curY = DEFAULT_Y;

    // Cache dots with world coordinates and cell assignment
    const dots = Array.from(container.querySelectorAll('.ds-sim-dot')).map((el) => {
      const x = parseFloat(el.getAttribute('data-x'));
      const y = parseFloat(el.getAttribute('data-y'));
      const outer = el.querySelector('.ds-sim-outer');
      const inner = el.querySelector('.ds-sim-inner');

      let c = 0;
      for (let i = 0; i < 6; i++) {
        if (x >= colBounds[i] && (i === 5 ? x <= colBounds[i + 1] : x < colBounds[i + 1])) {
          c = i;
          break;
        }
      }
      let r = 0;
      for (let j = 0; j < 5; j++) {
        if (y >= rowBounds[j] && (j === 4 ? y <= rowBounds[j + 1] : y < rowBounds[j + 1])) {
          r = j;
          break;
        }
      }

      return { el, x, y, c, r, outer, inner };
    });

    const cellCoordTexts = Array.from(container.querySelectorAll('.ds-cell-coord'));

    function updateSimulation(x, y) {
      curX = Math.max(MIN_X, Math.min(MAX_X, x));
      curY = Math.max(MIN_Y, Math.min(MAX_Y, y));

      // 1. Move Viewport Rect & Badge
      viewportRect.setAttribute('x', curX);
      viewportRect.setAttribute('y', curY);
      if (viewportBadgeBg) {
        viewportBadgeBg.setAttribute('x', curX + 6);
        viewportBadgeBg.setAttribute('y', curY + 6);
      }
      if (viewportBadgeTxt) {
        viewportBadgeTxt.setAttribute('x', curX + 14);
        viewportBadgeTxt.setAttribute('y', curY + 22);
      }

      // 2. Broad Phase: Calculate overlapping columns & rows
      const xMin = curX;
      const xMax = curX + VP_WIDTH;
      const yMin = curY;
      const yMax = curY + VP_HEIGHT;

      let minCol = 5, maxCol = 0;
      for (let i = 0; i < 6; i++) {
        if (xMax > colBounds[i] + 0.5 && xMin < colBounds[i + 1] - 0.5) {
          if (i < minCol) minCol = i;
          if (i > maxCol) maxCol = i;
        }
      }
      if (minCol > maxCol) { minCol = 0; maxCol = 5; }

      let minRow = 4, maxRow = 0;
      for (let j = 0; j < 5; j++) {
        if (yMax > rowBounds[j] + 0.5 && yMin < rowBounds[j + 1] - 0.5) {
          if (j < minRow) minRow = j;
          if (j > maxRow) maxRow = j;
        }
      }
      if (minRow > maxRow) { minRow = 0; maxRow = 4; }

      // 3. Update Active Grid Highlight Box
      const activeLeft = colBounds[minCol];
      const activeTop = rowBounds[minRow];
      const activeRight = colBounds[maxCol + 1];
      const activeBottom = rowBounds[maxRow + 1];
      const activeW = activeRight - activeLeft;
      const activeH = activeBottom - activeTop;

      activeGridBox.setAttribute('x', activeLeft);
      activeGridBox.setAttribute('y', activeTop);
      activeGridBox.setAttribute('width', activeW);
      activeGridBox.setAttribute('height', activeH);

      // 4. Update Active Grid Tag
      const cellCount = (maxCol - minCol + 1) * (maxRow - minRow + 1);
      if (activeGridTag) {
        const tagX = Math.max(30, Math.min(activeLeft + 6, 650));
        const tagY = activeTop > 105 ? activeTop - 27 : activeBottom + 6;
        activeGridTag.setAttribute('transform', `translate(${tagX}, ${tagY})`);
      }
      if (activeGridTagTxt) {
        activeGridTagTxt.textContent = `활성 그리드 ${cellCount}개 셀 (Broad Phase 산출)`;
      }

      // 5. Update Grid Cell Coordinates
      cellCoordTexts.forEach((txt) => {
        const c = parseInt(txt.getAttribute('data-col'), 10);
        const r = parseInt(txt.getAttribute('data-row'), 10);
        const isActive = c >= minCol && c <= maxCol && r >= minRow && r <= maxRow;
        if (isActive) {
          txt.setAttribute('fill', '#60a5fa');
          txt.setAttribute('font-weight', '700');
        } else {
          txt.setAttribute('fill', 'rgba(148, 163, 184, 0.45)');
          txt.setAttribute('font-weight', '600');
        }
      });

      // 6. Narrow Phase: Evaluate 3-way object states
      let renderedCount = 0;
      let culledCount = 0;

      dots.forEach((d) => {
        const inActiveGrid = d.c >= minCol && d.c <= maxCol && d.r >= minRow && d.r <= maxRow;
        if (!inActiveGrid) {
          // [3] Inactive Grid (Broad Phase 원천 배제)
          d.outer.setAttribute('r', '5');
          d.outer.setAttribute('fill', '#1e293b');
          d.outer.setAttribute('stroke', '#475569');
          d.outer.setAttribute('stroke-width', '1.5');
          d.outer.removeAttribute('filter');
          d.outer.setAttribute('opacity', '0.55');
          if (d.inner) d.inner.setAttribute('opacity', '0');
          culledCount++;
        } else {
          // Broad phase passed. Narrow phase AABB test:
          const inViewport = d.x >= xMin && d.x <= xMax && d.y >= yMin && d.y <= yMax;
          if (inViewport) {
            // [1] Rendered (Narrow Phase 통과)
            d.outer.setAttribute('r', '9');
            d.outer.setAttribute('fill', '#22c55e');
            d.outer.setAttribute('stroke', '#15803d');
            d.outer.setAttribute('stroke-width', '2.5');
            d.outer.setAttribute('filter', 'url(#glow-green)');
            d.outer.setAttribute('opacity', '1');
            if (d.inner) d.inner.setAttribute('opacity', '1');
            renderedCount++;
          } else {
            // [2] Culled candidate (Narrow Phase AABB 탈락)
            d.outer.setAttribute('r', '7.5');
            d.outer.setAttribute('fill', '#f59e0b');
            d.outer.setAttribute('stroke', '#b45309');
            d.outer.setAttribute('stroke-width', '2');
            d.outer.setAttribute('filter', 'url(#glow-amber)');
            d.outer.setAttribute('opacity', '1');
            if (d.inner) d.inner.setAttribute('opacity', '0');
            culledCount++;
          }
        }
      });

      const total = renderedCount + culledCount;
      const ratio = total > 0 ? Math.round((culledCount / total) * 100) : 0;

      // 7. Update Callout Visibility (Fade out when exploring away from default)
      if (callouts) {
        const isDefault = Math.abs(curX - DEFAULT_X) < 4 && Math.abs(curY - DEFAULT_Y) < 4;
        callouts.style.transition = 'opacity 0.25s ease';
        callouts.style.opacity = isDefault ? '1' : '0';
        callouts.style.pointerEvents = 'none';
      }

      // 8. Update HUD metrics
      if (hudCam) {
        const engX = Math.round((curX - 30) * 2.8 + 600);
        const engY = Math.round((curY - 75) * 2.8 + 390);
        hudCam.textContent = `X: ${engX}, Y: ${engY}`;
      }
      if (hudCells) hudCells.textContent = `${cellCount} / 30`;
      if (hudRendered) hudRendered.textContent = `${renderedCount}`;
      if (hudCulled) hudCulled.textContent = `${culledCount}`;
      if (hudRatio) hudRatio.textContent = `${ratio}%`;
    }

    // Pointer Drag Interaction
    let isDragging = false;
    let dragStartX = 0;
    let dragStartY = 0;
    let initialVpX = 0;
    let initialVpY = 0;

    function getSvgPoint(e) {
      const pt = svg.createSVGPoint();
      pt.x = e.clientX;
      pt.y = e.clientY;
      return pt.matrixTransform(svg.getScreenCTM().inverse());
    }

    viewportDrag.addEventListener('pointerdown', function (e) {
      isDragging = true;
      viewportDrag.classList.add('is-dragging');
      viewportDrag.setPointerCapture(e.pointerId);

      const pt = getSvgPoint(e);
      dragStartX = pt.x;
      dragStartY = pt.y;
      initialVpX = curX;
      initialVpY = curY;
    });

    viewportDrag.addEventListener('pointermove', function (e) {
      if (!isDragging) return;
      const pt = getSvgPoint(e);
      const dx = pt.x - dragStartX;
      const dy = pt.y - dragStartY;
      updateSimulation(initialVpX + dx, initialVpY + dy);
    });

    function onPointerUp(e) {
      if (!isDragging) return;
      isDragging = false;
      viewportDrag.classList.remove('is-dragging');
      try {
        viewportDrag.releasePointerCapture(e.pointerId);
      } catch (_) {}
    }

    viewportDrag.addEventListener('pointerup', onPointerUp);
    viewportDrag.addEventListener('pointercancel', onPointerUp);

    // Reset Button
    if (btnReset) {
      btnReset.addEventListener('click', function () {
        updateSimulation(DEFAULT_X, DEFAULT_Y);
      });
    }

    // Initial simulation update
    updateSimulation(DEFAULT_X, DEFAULT_Y);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", function () {
      initFtdGalleries();
      initSpatialSimulation();
    });
  } else {
    initFtdGalleries();
    initSpatialSimulation();
  }
})();

