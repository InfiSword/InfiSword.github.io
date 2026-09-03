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
})();
