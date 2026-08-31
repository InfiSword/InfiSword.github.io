const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "..");
const contentDir = path.join(root, "content");
const projectSourceDir = path.join(contentDir, "projects");
const legacyPostsDir = path.join(root, "_posts");

const site = {
  title: "Minhyuk Lee | Game Developer",
  author: "이민혁 (Minhyuk Lee)",
  description:
    "게임 클라이언트 프로그래머 이민혁의 시스템 아키텍처, 렌더링, 강화학습 중심 포트폴리오입니다.",
  url: "https://infisword.github.io",
};

function ensureDir(dir) {
  fs.mkdirSync(dir, { recursive: true });
}

function readText(filePath) {
  return fs.readFileSync(filePath, "utf8");
}

function writeText(filePath, value) {
  ensureDir(path.dirname(filePath));
  fs.writeFileSync(filePath, value, "utf8");
}

function stripFrontMatter(source) {
  if (!source.startsWith("---")) {
    return { data: {}, body: source };
  }

  const end = source.indexOf("\n---", 3);
  if (end === -1) {
    return { data: {}, body: source };
  }

  const frontMatter = source.slice(3, end).trim();
  const body = source.slice(end + 4).replace(/^\r?\n/, "");
  return { data: parseFrontMatter(frontMatter), body };
}

function parseFrontMatter(frontMatter) {
  const data = {};

  for (const rawLine of frontMatter.split(/\r?\n/)) {
    const line = rawLine.trim();
    if (!line || line.startsWith("#")) continue;

    const match = line.match(/^([A-Za-z0-9_-]+):\s*(.*)$/);
    if (!match) continue;

    const key = match[1];
    let value = match[2].trim();
    if (value === "") {
      data[key] = "";
      continue;
    }

    if (
      (value.startsWith('"') && value.endsWith('"')) ||
      (value.startsWith("'") && value.endsWith("'"))
    ) {
      value = value.slice(1, -1);
    } else if (value.startsWith("[") && value.endsWith("]")) {
      value = value
        .slice(1, -1)
        .split(",")
        .map((item) => item.trim())
        .filter(Boolean);
    } else if (value === "true" || value === "false") {
      value = value === "true";
    }

    data[key] = value;
  }

  return data;
}

function migrateSources() {
  ensureDir(contentDir);
  ensureDir(projectSourceDir);

  const homeSource = path.join(contentDir, "home.html");
  if (!fs.existsSync(homeSource)) {
    const currentIndex = path.join(root, "index.html");
    if (fs.existsSync(currentIndex)) {
      const { body } = stripFrontMatter(readText(currentIndex));
      writeText(homeSource, normalizeLegacyLiquid(body));
    }
  }

  if (fs.existsSync(legacyPostsDir)) {
    for (const entry of fs.readdirSync(legacyPostsDir)) {
      if (!entry.endsWith(".md")) continue;
      const source = path.join(legacyPostsDir, entry);
      const target = path.join(projectSourceDir, entry.replace(/^\d{4}-\d{2}-\d{2}-/, ""));
      if (!fs.existsSync(target)) {
        writeText(target, normalizeProjectSource(readText(source)));
      }
    }
  }

  sanitizeProjectSources();
}

function serializeFrontMatter(data) {
  const keys = ["title", "excerpt", "permalink", "tags", "mermaid"];
  const lines = ["---"];

  for (const key of keys) {
    if (data[key] === undefined || data[key] === "") continue;
    if (Array.isArray(data[key])) {
      lines.push(`${key}: [${data[key].join(", ")}]`);
    } else if (typeof data[key] === "boolean") {
      lines.push(`${key}: ${data[key]}`);
    } else {
      lines.push(`${key}: ${JSON.stringify(data[key])}`);
    }
  }

  lines.push("---", "");
  return lines.join("\n");
}

function normalizeProjectSource(source) {
  const parsed = stripFrontMatter(normalizeLegacyLiquid(source));
  return serializeFrontMatter(parsed.data) + parsed.body;
}

function sanitizeProjectSources() {
  for (const entry of fs.readdirSync(projectSourceDir)) {
    if (!entry.endsWith(".md")) continue;
    const filePath = path.join(projectSourceDir, entry);
    writeText(filePath, normalizeProjectSource(readText(filePath)));
  }
}

function normalizeLegacyLiquid(source) {
  return source
    .replace(/\{\{\s*'([^']+)'\s*\|\s*relative_url\s*\}\}/g, "$1")
    .replace(/\{\{\s*"([^"]+)"\s*\|\s*relative_url\s*\}\}/g, "$1")
    .replace(/file:\/\/\/D:\/Workspace\/codeReference\//g, "/codeReference/")
    .replace(/\smarkdown="1"/g, "");
}

function escapeHtml(value) {
  return String(value)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function slugify(value, usedIds) {
  const base =
    value
      .replace(/<[^>]*>/g, "")
      .trim()
      .toLowerCase()
      .replace(/[^\p{L}\p{N}\s_-]+/gu, "")
      .replace(/\s+/g, "-")
      .replace(/-+/g, "-")
      .replace(/^-|-$/g, "") || "section";

  let id = base;
  let index = 2;
  while (usedIds.has(id)) {
    id = `${base}-${index}`;
    index += 1;
  }
  usedIds.add(id);
  return id;
}

function processInline(value) {
  const codeTokens = [];
  let output = value.replace(/`([^`]+)`/g, (_, code) => {
    const token = `@@CODE${codeTokens.length}@@`;
    codeTokens.push(`<code>${escapeHtml(code)}</code>`);
    return token;
  });

  output = output
    .replace(/!\[([^\]]*)\]\(([^)]+)\)/g, '<img src="$2" alt="$1">')
    .replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>')
    .replace(/\*\*([^*]+)\*\*/g, "<strong>$1</strong>")
    .replace(/\*([^*]+)\*/g, "<em>$1</em>");

  codeTokens.forEach((html, index) => {
    output = output.replace(`@@CODE${index}@@`, html);
  });

  return output;
}

function isHtmlLine(line) {
  return /^<\/?(style|script|div|span|p|table|thead|tbody|tr|th|td|details|summary|img|br|hr|strong|em|code|section|article|figure|figcaption|ul|ol|li)\b/i.test(
    line.trim()
  );
}

function collectParagraph(lines, start) {
  const values = [];
  let index = start;

  while (index < lines.length) {
    const line = lines[index];
    const trimmed = line.trim();
    if (
      !trimmed ||
      /^#{1,6}\s+/.test(trimmed) ||
      /^```/.test(trimmed) ||
      /^\{:\s*\.[A-Za-z0-9_-]+\s*\}$/.test(trimmed) ||
      /^[-*]\s+/.test(trimmed) ||
      /^\d+\.\s+/.test(trimmed) ||
      isTableStart(lines, index) ||
      isHtmlLine(trimmed)
    ) {
      break;
    }
    values.push(trimmed);
    index += 1;
  }

  return { value: values.join(" "), nextIndex: index };
}

function isTableStart(lines, index) {
  return (
    lines[index] &&
    lines[index].trim().startsWith("|") &&
    lines[index + 1] &&
    /^\|\s*:?-{3,}:?\s*(\|\s*:?-{3,}:?\s*)+\|?$/.test(lines[index + 1].trim())
  );
}

function renderTable(lines, start) {
  const rows = [];
  let index = start;

  while (index < lines.length && lines[index].trim().startsWith("|")) {
    rows.push(
      lines[index]
        .trim()
        .replace(/^\|/, "")
        .replace(/\|$/, "")
        .split("|")
        .map((cell) => processInline(cell.trim()))
    );
    index += 1;
  }

  const [head, , ...body] = rows;
  const columnCount = head.length;
  const html = [
    '<div class="pf-table-wrapper">',
    '<table class="pf-data-table">',
    "<thead><tr>",
    ...head.map((cell) => `<th>${cell}</th>`),
    "</tr></thead>",
    "<tbody>",
    ...body.map(
      (row) =>
        `<tr>${Array.from({ length: columnCount }, (_, i) => `<td>${row[i] || ""}</td>`).join(
          ""
        )}</tr>`
    ),
    "</tbody>",
    "</table>",
    "</div>",
  ].join("");

  return { html, nextIndex: index };
}

function renderList(lines, start, ordered, blockClass) {
  const marker = ordered ? /^\d+\.\s+/ : /^[-*]\s+/;
  const tag = ordered ? "ol" : "ul";
  const items = [];
  let index = start;

  while (index < lines.length && marker.test(lines[index].trim())) {
    items.push(lines[index].trim().replace(marker, ""));
    index += 1;
  }

  const className = blockClass ? ` class="${blockClass}"` : "";
  return {
    html: `<${tag}${className}>${items.map((item) => `<li>${processInline(item)}</li>`).join("")}</${tag}>`,
    nextIndex: index,
  };
}

function renderMarkdown(source) {
  const lines = normalizeLegacyLiquid(source).split(/\r?\n/);
  const usedIds = new Set();
  const headings = [];
  const html = [];
  let index = 0;
  let pendingBlockClass = "";

  while (index < lines.length) {
    const line = lines[index];
    const trimmed = line.trim();

    if (!trimmed) {
      index += 1;
      continue;
    }

    const attrMatch = trimmed.match(/^\{:\s*\.([A-Za-z0-9_-]+)\s*\}$/);
    if (attrMatch) {
      pendingBlockClass = attrMatch[1];
      index += 1;
      continue;
    }

    if (/^```/.test(trimmed)) {
      const language = trimmed.replace(/^```/, "").trim();
      const body = [];
      index += 1;
      while (index < lines.length && !/^```/.test(lines[index].trim())) {
        body.push(lines[index]);
        index += 1;
      }
      index += 1;

      if (language === "mermaid") {
        html.push(`<pre class="mermaid">${escapeHtml(body.join("\n"))}</pre>`);
      } else {
        html.push(
          `<pre><code${language ? ` class="language-${escapeHtml(language)}"` : ""}>${escapeHtml(
            body.join("\n")
          )}</code></pre>`
        );
      }
      continue;
    }

    if (/^<style\b/i.test(trimmed)) {
      const block = [line];
      index += 1;
      while (index < lines.length && !/<\/style>/i.test(lines[index])) {
        block.push(lines[index]);
        index += 1;
      }
      if (index < lines.length) {
        block.push(lines[index]);
        index += 1;
      }
      html.push(block.join("\n"));
      continue;
    }

    const headingMatch = trimmed.match(/^(#{1,6})\s+(.+)$/);
    if (headingMatch) {
      let className = pendingBlockClass;
      if (lines[index + 1] && /^\{:\s*\.[A-Za-z0-9_-]+\s*\}$/.test(lines[index + 1].trim())) {
        className = lines[index + 1].trim().match(/^\{:\s*\.([A-Za-z0-9_-]+)\s*\}$/)[1];
        index += 1;
      }

      const level = headingMatch[1].length;
      const text = processInline(headingMatch[2]);
      const id = slugify(headingMatch[2], usedIds);
      const classAttr = className ? ` class="${className}"` : "";
      html.push(`<h${level} id="${id}"${classAttr}>${text}</h${level}>`);
      if (level >= 2 && level <= 4) {
        headings.push({ level, text: headingMatch[2].replace(/<[^>]*>/g, ""), id });
      }
      pendingBlockClass = "";
      index += 1;
      continue;
    }

    if (isTableStart(lines, index)) {
      const table = renderTable(lines, index);
      html.push(table.html);
      index = table.nextIndex;
      continue;
    }

    if (/^[-*]\s+/.test(trimmed)) {
      const list = renderList(lines, index, false, pendingBlockClass);
      html.push(list.html);
      pendingBlockClass = "";
      index = list.nextIndex;
      continue;
    }

    if (/^\d+\.\s+/.test(trimmed)) {
      const list = renderList(lines, index, true, pendingBlockClass);
      html.push(list.html);
      pendingBlockClass = "";
      index = list.nextIndex;
      continue;
    }

    if (/^---+$/.test(trimmed)) {
      html.push("<hr>");
      index += 1;
      continue;
    }

    if (isHtmlLine(trimmed)) {
      html.push(normalizeLegacyLiquid(line));
      index += 1;
      continue;
    }

    const paragraph = collectParagraph(lines, index);
    const classAttr = pendingBlockClass ? ` class="${pendingBlockClass}"` : "";
    html.push(`<p${classAttr}>${processInline(paragraph.value)}</p>`);
    pendingBlockClass = "";
    index = paragraph.nextIndex;
  }

  return { html: html.join("\n"), headings };
}

function renderToc(headings) {
  if (!headings.length) return "";
  return [
    '<aside class="toc-panel" aria-label="목차">',
    '<div class="toc-panel__inner">',
    '<h2 class="toc-panel__title">목차</h2>',
    '<ol class="toc-list">',
    ...headings.map(
      (heading) =>
        `<li class="toc-list__item toc-list__item--h${heading.level}"><a href="#${heading.id}">${escapeHtml(
          heading.text
        )}</a></li>`
    ),
    "</ol>",
    "</div>",
    "</aside>",
  ].join("\n");
}

function renderShell({ title, description, body, pageClass = "" }) {
  return `<!doctype html>
<html lang="ko">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>${escapeHtml(title)}</title>
  <meta name="description" content="${escapeHtml(description || site.description)}">
  <meta property="og:title" content="${escapeHtml(title)}">
  <meta property="og:description" content="${escapeHtml(description || site.description)}">
  <meta property="og:type" content="website">
  <meta property="og:url" content="${site.url}">
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Fira+Code:wght@400;500;600;700&family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=Noto+Sans+KR:wght@300;400;500;600;700;900&display=swap" rel="stylesheet">
  <link rel="stylesheet" href="/assets/css/site.css">
</head>
<body class="${pageClass}">
  <a class="skip-link" href="#main">본문으로 이동</a>
  <header class="site-header">
    <div class="site-header__inner">
      <a class="site-title" href="/">${site.title}</a>
      <nav class="site-nav" aria-label="주요 메뉴">
        <a href="/">Home</a>
        <a href="/#projects">Projects</a>
        <a href="mailto:seif4688@gmail.com">Contact</a>
      </nav>
    </div>
  </header>
  <main id="main" class="site-main">
${body}
  </main>
  <footer class="site-footer">
    <div class="site-footer__inner">
      <p><span class="site-footer__mark">ML</span> 게임의 구조와 플레이 감각을 함께 설계합니다.</p>
      <a href="mailto:seif4688@gmail.com">seif4688@gmail.com ↗</a>
    </div>
  </footer>
  <script src="/assets/js/site.js"></script>
</body>
</html>
`;
}

function renderHome() {
  const homeSource = path.join(contentDir, "home.html");
  const body = `    <div class="site-container site-container--home">
${readText(homeSource)}
    </div>`;
  writeText(
    path.join(root, "index.html"),
    renderShell({
      title: site.title,
      description: site.description,
      body,
      pageClass: "page-home",
    })
  );
}

function outputPathForPost(data, sourceFile) {
  const permalink = data.permalink || `/project/${path.basename(sourceFile, ".md")}/`;
  return path.join(root, permalink.replace(/^\/+/, ""), "index.html");
}

function renderProject(sourceFile) {
  const parsed = stripFrontMatter(readText(sourceFile));
  const articleSource = parsed.body.replace(/<style\b[^>]*>[\s\S]*?<\/style>\s*/gi, "");
  const article = renderMarkdown(articleSource);
  const tags = Array.isArray(parsed.data.tags) ? parsed.data.tags : [];
  const title = parsed.data.title || path.basename(sourceFile, ".md");
  const description = parsed.data.excerpt || site.description;
  const body = `    <article class="site-container project-page">
      <header class="project-hero">
        <a class="back-link" href="/#projects">← 프로젝트 목록으로</a>
        <p class="project-kicker">PROJECT REPORT</p>
        <h1>${escapeHtml(title)}</h1>
        <p>${escapeHtml(description)}</p>
        ${
          tags.length
            ? `<div class="project-tags">${tags
                .map((tag) => `<span>${escapeHtml(tag)}</span>`)
                .join("")}</div>`
            : ""
        }
      </header>
      <div class="project-layout">
        <section class="project-content">
${article.html}
        </section>
${renderToc(article.headings)}
      </div>
    </article>`;

  writeText(
    outputPathForPost(parsed.data, sourceFile),
    renderShell({
      title: `${title} | ${site.author}`,
      description,
      body,
      pageClass: "page-project",
    })
  );
}

function renderProjects() {
  for (const entry of fs.readdirSync(projectSourceDir).sort()) {
    if (!entry.endsWith(".md")) continue;
    renderProject(path.join(projectSourceDir, entry));
  }
}

migrateSources();
renderHome();
renderProjects();
