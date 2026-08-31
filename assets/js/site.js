(function () {
  window.pfOpenModal = function (modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    modal.style.display = "flex";
    document.body.style.overflow = "hidden";
  };

  window.pfCloseModal = function (modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    modal.style.display = "none";
    document.body.style.overflow = "auto";
  };

  document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape") return;
    document.querySelectorAll(".pf-modal-overlay").forEach(function (modal) {
      modal.style.display = "none";
    });
    document.body.style.overflow = "auto";
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
})();
