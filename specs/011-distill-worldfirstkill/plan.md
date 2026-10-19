# Implementation Plan: Distill WorldFirstKill

**Branch**: `main` (unchanged) | **Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary
Preserve the concise two-chapter report and six-image gallery. Add actual SaveLoadManager.BuildSeedToken and RestoreUsageCount excerpts before the existing RestoreRNG example in section 2.2, for six source disclosures total. Restore the prior master and monster sheet screenshots in sections 1.1/1.2, plus the two seed-specific skill screenshots in section 2.2, with concise captions and full-size links. Keep page-scoped CSS in assets/css/site.css because the builder strips article style blocks.

## Technical Context
Markdown/HTML, Node >=18 static builder and existing Playwright. No new dependencies or runtime JavaScript. Desktop and Pixel 5 are the primary verified browser classes, with responsive checks at 375/768/1024/1440px.

## Constitution Check
Pass before research and after design. AGENTS.md governs; the constitution is a template. DESIGN.md and incumbent code define colors and type. Existing gallery and unrelated project reports are preserved. No game edits, commit, push or deployment.

## Reading Brief
Mode: Read. Audience: a reviewer scanning a game programmer's architecture work. Outcome: follow source data through cache and parsing to gameplay; follow seed/domain usage through a save token to RNG reconstruction. Use existing blue/white portfolio typography, restrained numbered flows and plain responsibility tables; code stays closed until requested. Avoid card grids, explanatory tangents, fabricated benchmarks and claims that seed state replaces all game-state storage.

## Structure
- content/projects/worldfirstkill.md -> project/worldfirstkill/index.html.
- specs/011-distill-worldfirstkill/: requirements, plan, evidence and validation.
- assets/css/site.css: page-scoped flow/table/focus styles.
- Existing tests: unchanged; add no permanent tests for editorial content.

## Implementation
1. Preserve front matter route and gallery; shorten introduction and excerpt.
2. Present two chapters with four subsections, two four-step flows and tables mapping three data classes and two RNG/save classes.
3. Read actual legacy restore sources. Show cache selection, header-field mapping, domain initialization/usage, and RestoreRNG. Label excerpts and omit only logs/comments where stated.
4. Remove old long diagrams, token bit-width comparison and unsupported timing/compression/absolute claims; reuse only the relevant original evidence images requested by the user.
5. Build, run existing tests and one batched desktop/mobile browser review. Run Impeccable detector and unslop scan once; fix any real scoped issues in one batch and confirm at most once.
6. Open the local preview visibly in the app and leave edits uncommitted.

## Complexity Tracking
No new runtime behavior or site-wide styles. Local CSS is limited to flow layout, responsibility-table wrapping and keyboard focus.

## Token code and relevant images

- Read the actual token methods and constants. Show their full core bodies with comments/blank lines trimmed; identify Pad and ReadUInt as fixed-width helpers.
- Preserve the generation order, signed seed bit conversion, domain sorting/counts, parsing guard/catch and RestoreRNG handoff.
- Use semantic figures with explicit image dimensions, lazy loading, alt text and original-image links. Pair the skill examples on desktop and stack them on mobile; scope figure styles to wfk classes.
- Keep screenshots illustrative: master catalog, source data and different seed-generated skills. Omit the old slide's absolute restoration claims.
- Build and run existing tests. Verify six code controls, three section 2.2 controls in order, all four example images, original links, mobile layout and the updated local preview. Leave files uncommitted.
