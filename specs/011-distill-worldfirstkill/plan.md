# Implementation Plan: Distill WorldFirstKill

**Branch**: `main` (unchanged) | **Date**: 2026-10-20 | **Spec**: [spec.md](spec.md)

## Summary

Current refinement: delete the redundant trailing lookup sentence from csv-url-structure only. Build/test and verify that sentence is absent while prior prose/code/box are preserved on desktop/mobile. No commit.

Current refinement: rewrite the nested structure description into one short paragraph using 데이터 종류/세부 항목 이름/CSV URL, retain concrete Unit/Monster mapping, and show only the typed csvURLDict declaration ending with a semicolon. Remove initializer and outer/inner labels without changing game code. Build/test, verify copy and declaration, inspect desktop/mobile and refresh preview. No commit.

Current refinement: delete only the example table/caption within csv-url-structure. Build/test and verify absence of that table while prose, declaration, lookup and box remain visible on desktop/Pixel5 without overflow; refresh preview. No commit.

Current refinement: replace only the nested-dictionary explanation with two concrete short paragraphs connecting CSVDataType.Skill/Unit to inner string-to-URL dictionaries. Preserve existing code/table/lookup and box. Build/test, verify explicit key/value wording, source preservation and desktop/mobile layout, then refresh preview without committing.

Current refinement: add existing wfk-rationale grouping class to master-csv-download, preserving wfk-source comment styling and exact inner content. Reuse existing border/background/padding/responsive rules; no new CSS or disclosure. Build/test and inspect desktop/Pixel5 containment, visibility, style parity and overflow, then refresh preview without committing.

Current refinement: replace only the master-csv-download details/summary wrapper with a wfk-source div and visible bold title. Preserve body bytes, anchors and existing separate cache control. Reuse incumbent visible-source styling; no CSS change. Build/test, verify no disclosure surrounds structure/code and new title is visible on desktop/Pixel5, preserve other controls and no overflow, then refresh preview without committing.

Current refinement: replace the three download method sections with one concise master Link -> CSV -> C# game-data paragraph at the start of the existing disclosure. Preserve csvURLDict structure/example and all unrelated code. Build/test, verify removed method sections and retained declaration/mapping, ten controls and desktop/mobile layout, then refresh preview. No commit.

Current refinement: prepend a short csvURLDict structure explanation, actual declaration, two-row example table and example lookup comment to the existing download details. Use Unit/Monster and Skill/Active only after checking actual enums. Clearly label example addresses as descriptive labels. Shorten each stage's prose to one compact paragraph while leaving original core code unchanged. Build/test, verify declaration/source parity, nested mapping and desktop/mobile layout, then refresh preview. No commit.

Current refinement: add master-csv-download details containing three labeled explanations and code fences for metadata/link registration, per-URL conditional download and DownloadFileAsync. Include the actual master CSV URL, header skip, row/type validation and nested dictionary registration. Show request completion and success/failure return behavior. Connect per-file needsDownload to the following existing cache example; do not duplicate cache source. Build/test, compare core statements with actual source and preserve prior blocks, verify ten disclosures on desktop/Pixel5 and no overflow, refresh preview. No commit.

Current refinement: replace only the master-sheet figure caption, keeping its image/link unchanged. Read ServerCSV_ConvertData DownloadMetaSheetAsync, row Link extraction, DownloadAllCSVAsync and DownloadFileAsync to distinguish article coverage from implementation. Build/test, verify new caption and removed instruction on desktop/mobile, then refresh preview. No new article code or commit.

Current refinement: add an accessible fieldinfo-rationale section containing a title and the unchanged three bullets. Extend the incumbent grouped-box CSS selectors to wfk-rationale, including first/last margin resets and mobile padding. Build/test, check text preservation, border/padding parity, desktop/mobile containment and existing controls, then refresh preview. No commit.

Current refinement: shorten reflection-fieldinfo to the retained array explanation and official FieldInfo documentation link. Remove only this block's expanded prose and illustrative code; preserve 1.2 surrounding flow, anchor and existing CSV mapping disclosure. Build/test and verify concise visible text/link, removal of the example, existing nine controls and desktop/mobile layout. Refresh preview without committing.

Current refinement: add seed-derivation-code details inside splitmix-rationale around the unchanged DeriveSubSeed fence; explanation stays outside details. Reuse existing classes and styles. Build/test, verify code parity, initial closed state, click/tap/Enter/Space and desktop/mobile layout, then refresh the preview. No commit.

Current refinement: place a wfk-source reflection-fieldinfo block after the 1.2 introduction, with concise .NET metadata explanation and a visible GetField/Name/FieldType/SetValue/GetValue example based on Data_Monster.m_monsterName. Fold no part of this addition; preserve incumbent styles. Verify API semantics against Microsoft Learn and the actual field declaration, then build/test and inspect desktop/Pixel 5 visibility, comments, existing disclosures and overflow. Refresh the preview, no commit.

Current refinement: add a short helper explanation/example within each existing generation/parsing details body, before its pre block; remove the now-redundant detailed comment examples and retain brief functional annotations. Keep statements and source colocation unchanged. Build/test, verify explanation is inside details but outside pre and toggles with code, check desktop/mobile screenshots and refresh preview. No commit.

Current refinement: replace the short helper separator comments with useful in-code descriptions/examples, and annotate ReadUInt's three statements. Preserve helper implementations and caller statements, existing styling/prose, eight disclosures and eleven fences. Build/test, check non-comment code parity and actual example values, inspect desktop/mobile open code and refresh the preview. No commit.

Current refinement: copy Pad(uint/ulong) and ReadUInt(string,ref int,int) exactly from SaveLoadManager.cs into the existing generation/parsing code fences, with short separator comments. Update disclosure labels to include helper names. No new helper section, disclosure, CSS or visible explanation. Verify helper source parity, preserved caller statements, eight closed controls and desktop/mobile scrolling, then build/test and refresh the visible preview. No commit.

Current refinement: rewrite only the three phase descriptions so readers can follow seed/usage -> token -> seed/usage -> restored per-domain generators while all code is closed. Keep Pad/ReadUInt explanations next to their phases, explain missing usage/default zero and long/ulong reversal, and clarify that counter restoration also supports the next save. Preserve source text, examples, eight native controls and imagery. Build/test, verify visible prose and unchanged code on desktop/Pixel 5, then refresh the preview. Reading budget 3,700 characters; no commit.

Current refinement: change only the third token-phase label to 난수 상태 복구 and enclose its source and the generation/parsing sources in native pf-details/wfk-code controls. Preserve visible prose/examples and section wrappers/anchors. Validate unchanged source, eight closed controls with pointer/touch/Enter/Space, three visible blocks, desktop/mobile overflow, then build/test and refresh the preview without committing.

Current refinement: reuse the domain-structures border/background/padding rules for one skill-comparison group, preserving its introduction, original images and captions. Add InGame/wfk_ingame_06.png (912×861) after the Minecraft reference as the actual game's seed-input screen, constrained to 480px wide. Preserve source code and gallery. Build, run the existing suite, inspect desktop/Pixel 5 layout, image loading and original links, verify all five code disclosures and absence of horizontal overflow, then refresh the visible preview. No commit.

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

- Read the actual token methods and constants. Show their essential source statements with omissions explicitly labelled; identify Pad and ReadUInt as fixed-width helpers.
- Preserve the generation order, signed seed bit conversion, domain sorting/counts, parsing guard/catch and RestoreRNG handoff.
- Use semantic figures with explicit image dimensions, lazy loading, alt text and original-image links. Pair the skill examples on desktop and stack them on mobile; scope figure styles to wfk classes.
- Keep screenshots illustrative: master catalog, source data and different seed-generated skills. Omit the old slide's absolute restoration claims.
- Build and run existing tests. Verify six code controls, three section 2.2 controls in order, all four example images, original links, mobile layout and the updated local preview. Leave files uncommitted.

## Worked token example

Add a short visible schema and a two-column example table immediately after the section 2.2 introduction and before the skill pictures. Use the actual 14 Domain values: seed 42, Map(001)=3, Quest(004)=5, other 12 domains=0. Show the 20-digit seed, 3-digit count 014 and selected ID(3)/usage(12) records. Label plus signs as visual boundaries, state that omitted domain rows are still stored in ID order without separators, and explain reading 20/3 then 15 digits 14 times before per-domain Skip. Reuse existing typography/table conventions with a narrowly scoped table class for mobile readability. No new JavaScript or dependencies. Build, run the existing suite, verify field examples and desktop/Pixel 5 readability, preserve six controls/four figures and refresh the visible preview. Do not commit.

## Interviewer comprehension pass

Audience: a game programmer interviewer who understands C# but has no project context. Lead each system with the problem and design decision, define first-use terms inline, and add a real Name -> m_monsterName mapping example including why FieldInfo metadata is needed. Explain domain independence and seed-only versus progress restoration. Keep the worked token table.

Retain six closed code controls but replace full method bodies with annotated core excerpts of at most 18 nonblank lines. Preserve actual source statements and variable names; explicitly identify separate methods/omitted setup, sorting, empty-header checks and token validation so the excerpts do not imply complete executable methods. Use short nearby explanations of input, state changes and result. Mention Skip's dependence on data/generation rules and its cost proportional to saved usage.

Keep the original master/monster sheets and two skill pictures byte-for-byte; improve captions to connect columns and generated output to code. Reuse the previous article's Minecraft seed-input illustration unchanged inside one closed reference disclosure, labeled as design inspiration rather than an image of this game. Do not reintroduce dense old slide layouts, duplicate skill pictures or unsupported embedded claims. Preserve DESIGN.md and scoped existing styles; no new animations/dependencies.

Update source evidence, contracts and validation. Build, run all existing tests, and verify desktop/Pixel 5 reading structure, six shorter code controls plus one reference, images/original links, the token example, keyboard/pointer behavior and responsive widths in one batched round. Refresh and retain the visible local preview. No game edits, commit, push or deployment.

## Reflection rationale follow-up

Insert three concise bullets after the real CSV-to-field example and before the unchanged monster sheet. Explain why generic field lookup supports one common parser, how it reduces repeated mapping/conversion code, and the cost of runtime reflection plus the need to check name/type mismatches during conversion. Preserve code, images, chapter structure and CSS. Update the source-based evidence, build and run the existing tests; inspect the three bullets and preserved disclosure on desktop/Pixel 5. Refresh the visible preview. Do not commit or publish.

## Domain separation rationale follow-up

Replace the repeated causal explanation in section 2.1 with a short definition/mechanism paragraph and three visible bullets matching section 1.2: rationale, benefit and tradeoff. Use the actual GameSeed per-domain RNG/usage dictionaries and DeriveSubSeed input IDs as evidence. Explain cross-domain call isolation while explicitly bounding it by within-domain call order and stable IDs/derivation. Preserve the existing code, images, CSS and adjacent token example. Build, run the existing suite and inspect desktop/Pixel 5 readability and disclosure operation. Refresh the visible local section. Do not commit or publish.

## Semitransparent comments

Add page-scoped CSS for .wfk-code .hljs-comment using rgba(212,212,212,0.7), with nested syntax tokens inheriting that color so class-name highlighting inside comments does not stay saturated. Apply alpha to text color rather than whole-block opacity. Preserve existing code/markup and other syntax. Build and run the existing tests; verify computed comment/child colors, unchanged keyword styling, readable contrast and desktop/Pixel 5 disclosures in one review batch. Refresh the local preview with a code example open. No commit or publication.

## Always-visible seed inspiration figure

Replace only the Seed design-origin details/summary wrapper with a normal semantic figure at the same location. Preserve the original image, dimensions, alt text and original-image link; keep the title as bold text in its caption and give the figure a stable seed-design-origin ID. Remove the unused reference-summary focus selector while preserving code and image-link focus rules. Code excerpts remain six initially closed disclosures. Build, run the existing suite, and verify immediate image visibility, source loading, desktop/Pixel 5 layout and existing code behavior. Refresh the visible preview to the figure. No commit or publication.

## Corrected seed explanation and usage replay example

Verify actual GameSeed/SaveLoadManager declarations and compare the mix constants to the original SplitMix64 source. C# long/ulong ranges and System.Random seeding are checked against Microsoft primary documentation. Correct terminology while keeping the user's intended explanation: signed long BaseSeed is encoded as ulong only for token storage; fixed-width 20 means decimal storage width; Domain:uint identifies feature streams; usage:ulong records consumed calls. Explain that SplitMix64-like mixing derives a 31-bit positive int sub-seed, while System.Random produces the actual sequence. Do not promise unique sub-seeds or complete game-state restoration.

Add the corrected definitions to sections 2.1/2.2 and a compact hypothetical Map sequence example under the existing token table: after using 12,47,8, expected next is 31; seed-only reload starts at 12; seed plus usage 3 skips to 31. Mark numbers as explanatory rather than measured output of seed 42. Preserve current source excerpts and figures. Increase the reading budget to 3,000 characters to accommodate the explicitly requested explanation and example. Use an ordinary two-column table; no CSS, JS or dependency additions.

Build and run the existing suite, verify factual source statements and the worked example, then review desktop/Pixel 5 reading and disclosure/overflow in one batch. Refresh the visible preview to the new example. No game-source changes, commit, push or publication.

## SplitMix64-like mixing rationale

Keep the domain separation bullets together first. Move the existing seed-derivation explanation after them into a small unstyled splitmix-rationale group and shorten the repeated mechanism description. Add three brief bullets: selection reason (diffuse small input differences), benefit (stateless deterministic derivation with few bit operations/multiplications), tradeoff (31-bit output collisions and compatibility when mixing rules change). Match the real DeriveSubSeed pure function and original SplitMix64 mixing steps already researched; do not introduce cryptographic/statistical independence or benchmark claims. Preserve the six excerpts, images, token example and comment styling. Build, run the existing suite, verify the new group and code on desktop/Pixel 5, and refresh the visible preview. No commit or publication.

## Actual domain enum declaration

Extract the complete public enum Domain : uint declaration directly from GameSeed.cs, with all 14 actual names/IDs and its category comment. Insert it after the section 2.1 introduction as a native initially closed pf-details/wfk-code disclosure at domain-enum; label it as the full declaration from GameSeed.cs and explain 1-6 common functions, 100s classes and 200s skills. Preserve the other six excerpts byte-for-byte, existing figures, comment styling and all visible reading text. Seven code controls total, each <=18 nonblank lines. Build, run the existing suite and compare the rendered enum to the real source; check seven closed controls, pointer/touch, keyboard, styling and desktop/Pixel 5 overflow. Open the enum in the visible local preview. No game edits, commit or publication.

## Domain wording and management dictionaries

Make the requested Domain/Map/Quest sentence its own first paragraph under section 2.1, preserving the user's wording with integer ID rather than the inaccurate C# signed int type. Move the existing RNG/domain definitions into a separate following paragraph and remove the now-redundant count sentence (the enum's helper already states 14). Inside domain-enum, keep the complete enum unchanged and add the actual two static readonly dictionary declarations as a second source block directly below it. Explain Domain enum keys, System.Random values and ulong usage counts; wrap assignments for readability without altering tokens. Rename the summary to cover enum and structures. Seven disclosures, eight code blocks. No CSS/JS/game edits. Build, run the existing suite and verify source parity, paragraph order and desktop/Pixel 5 disclosure behavior/overflow; show the result in the local preview. No commit or publication.

## Always-visible domain structures

### Plain-language explanation of continuing saved randomness

Remove Skip from visible headings, flow and example labels. Describe restoring the same generator, drawing/discarding previously consumed values, restoring counts and using the next value. Simplify related wording such as encoding, RNG input and replay cost into concrete actions, while keeping method identifiers/source unchanged and preserving the conditional restoration scope. Rename2.2 to 토큰 저장과 난수 이어 쓰기 and adapt generated contents links. Preserve five controls/eleven code blocks,233-digit math and figures. Build/test and verify no visible Skip, unchanged source, correct example outputs and desktop/mobile layout; refresh preview. Do not commit.

### Integrate helpers into the three token phases

Remove token-helpers and the two standalone helper disclosures. Explain Pad's zero-fill/string conversion and shared storage widths within generation; retain the existing generation table/note. Explain ReadUInt's slice/parse/ref-cursor behavior in parsing and relocate the unchanged actual20/3 table there under readuint-explanation. After the table, explain reading fourteen ID/usage pairs into the usage dictionary. Restoration starts from those parsed numeric inputs and performs the existing domain RNG initialization/Skip/count restoration. Preserve all remaining source,233-digit calculation, figures and72px heading gap. Build/test, verify helper headings/code removed, phase-specific explanations/order, table placement/source parity and desktop/mobile overflow; refresh preview. No commit, push or game-source changes.

### Explain the current game's233-digit length

Expand the existing token-format total into a short statement of the current14-domain game and its field-by-field calculation:20+3+14*15=233. Explain zero-usage domains still occupy a record. Preserve width declarations and behavior. Build, run existing tests, verify the visible breakdown on desktop/mobile and refresh preview without committing.

### Total length below width constants

Add one visible formula below the token-format code: current14 domains produce20+3+14*(3+12)=233 digits. Keep source and all existing layout/behavior intact. Build and run existing tests; verify the formula on desktop/mobile and refresh preview. No commit or publication.

### Real-width ReadUInt example aligned with generation

Use the actual seed42/count14 token prefix00000000000000000042014, not the shortened5/3-digit illustration. Show a20-digit seed read returning42 at idx20, then a3-digit count read returning14 at idx23; subsequent3+12 records follow. Connect explicitly to the generation table without duplicating it. Update the generation note so | only marks domain ID versus usage, actual all14 records are ID-sorted in one string, and the order is seed/domain count/domain ID/usage. Keep the long prefix inside existing scrolling code styling if needed for mobile. Build/test and compare displayed widths/values/cursors to actual source; inspect desktop/Pixel5 and refresh preview. Do not commit or publish.

### ReadUInt purpose before cursor mechanics

Replace the short ReadUInt heading/arrow paragraph with a purpose-first explanation, input-parameter meanings and a two-row table showing reads of00042 and014 from the same shortened00042014 string. Show return values42/14 separately from cursor0->5->8, and explain ref means the caller keeps the updated position. Preserve the actual folded helper code and all other content. Permit3,250 visible prose characters to accommodate this clarification. Build/test, inspect desktop/Pixel5 including table widths and update the preview; no commit.

### More separation before section 2.2

Apply a scoped adjacent-heading margin to domain-initialization + h3, increasing its top margin to72px. Keep source/copy and other section spacing unchanged. Build, run existing tests and check the gap with NextInt closed/open on desktop/Pixel5, no horizontal overflow; refresh the preview without committing.

### Fold helper and initialization source only

Wrap only the four existing Pad/ReadUInt/Init/NextInt source fences in native pf-details/wfk-code controls, with function-specific summaries and stable IDs. Preserve visible headings, explanations, examples, all source bytes and other code visibility. No CSS/JS change. Build/test and verify initial closed state plus pointer/touch/Enter/Space on all seven controls, source parity, visible prose and desktop/mobile layout; refresh the preview. Do not commit or publish.

### Explain Pad/ReadUInt and remove editorial notices

Keep token-format focused on decimal widths. Add token-helpers before generation with Pad's actual uint/ulong overloads and ReadUInt's actual cursor/read/parse body. Explain Pad(42UL,5) -> 00042, and sequential reads of 00042014 (5 digits ->42, idx0->5; 3 digits ->14, idx5->8). Avoid repeating helper mechanics in each phase. Remove excerpt/omission prose from generation/parsing/restoration and equivalent CSV editorial notices, retaining the sorted-domain description and table's actual fourteen-domain coverage. Preserve algorithm code, images and the visible three-stage flow. Build/test, verify helper source parity and examples, inspect desktop/mobile and refresh the preview. No commit, push or game-source changes.

### Explain fixed widths and show core excerpts

Add the actual BASE_SEED_WIDTH=20, COUNT_WIDTH=3, DOMAIN_ID_WIDTH=3 and USAGE_WIDTH=12 declarations before token generation. Explain widths and Pad/ReadUInt once, linked to the existing fixed-width example. Replace full generation/restoration declarations with source-backed core excerpts, omit generation domain-list preparation and parser length/ID checks/try-catch with explicit labels, and retain the usage dictionary and restoration domain loop so variable responsibilities remain clear. Reduce prose to avoid repeating helper/format/Skip explanations. Preserve the three visible phases, figures and examples; no CSS/JS/game changes. Build/test and compare constants/excerpts to real source, inspect desktop/mobile and refresh preview. No commit or publication.

### Narrative token generation, parsing and restoration

Restructure section 2.2 into visible token-generation, token-parsing and rng-restoration groups, matching the preceding labeled method style. Explain each responsibility once immediately before its code. Place the existing fixed-width token example under generation and the hypothetical consumed-count example under restoration; retain scope/Skip cost once and unchanged skill pictures at the end. Use the full actual compact BuildSeedToken and RestoreRNG methods (<=18 nonblank lines), and a clearly labeled core RestoreUsageCount excerpt including the usage dictionary while omitting existing validation/try-catch. Keep helper definitions concise and accurately distinguish stored widths from types. No new CSS, dependencies or game changes. Build, test and inspect both desktop/Pixel 5 views together; verify method parity, ordering, visible code, three remaining disclosures, original figures, no errors/overflow and refreshed preview. Do not commit.

### Separate visible Init and NextInt methods

Replace domain-initialization details with a visible wfk-source group after seed derivation. Rewrite the requested explanation accurately: DeriveSubSeed mixes base seed/domain ID into a seed for System.Random; NextInt records consumption and returns an int. Split the previous mixed fragment into separately labeled actual Init(long seed) and NextInt(Domain domain, int minInclusive, int maxExclusive) declarations from GameSeed.cs. Preserve other source and figures. Build and verify source parity, ordering, two visible blocks, six remaining controls and desktop/mobile interaction/layout; no commit or game edit.

### Initialization after seed derivation

Move the entire unchanged domain-initialization disclosure immediately after splitmix-rationale, before section 2.2. Remove the requested shared-RNG reason/benefit paragraph. Preserve all nine C# blocks, seven collapsed controls, helper text and visible structures/mixing. Build, run existing tests and verify ordering, absent prose, disclosure interaction and desktop/mobile overflow; refresh preview. No commit or publication.

### Domain separation prose refinement

Replace the three domain separation bullets with one Korean paragraph explaining that a shared RNG lets added dialogue calls affect map generation, and separate RNGs keep map progress stable for easier reproduction/debugging. Remove that caution bullet only. Preserve surrounding code and source disclosures. Build, run existing tests and verify prose and layout on desktop/mobile; refresh the preview without committing.

### Wording and initialization disclosure refinement

Replace +1·-1 with +1이나 -1. Convert only domain-initialization back to a native pf-details/wfk-code control with its existing title as summary; preserve its position, annotation and exact code, and keep structures/mixing visible. Build, run existing tests and verify initial closed state, pointer/touch and Enter/Space operation plus desktop/mobile layout. No commit or game-source edit.

### Visible initialization and concise seed mixing

Move the unchanged Init/NextInt excerpt immediately after the structures box into domain-initialization with a visible title and helper explanation. Replace splitmix-rationale's long paragraph/list with a short accurate small-input-diffusion explanation, then the full actual DeriveSubSeed method. It derives the int seed; System.Random generates the game values. Use wfk-source for these visible code sections and share existing comment coloring. Six disclosures, nine total blocks. Verify source parity, ordering, visibility, syntax styling and desktop/mobile overflow, then build, run the existing suite and refresh the preview. No commit or game-source edits.

### Bordered grouping refinement

Add a scoped wfk-domain-structures class to the existing wrapper. Use the established background/border tokens, a 12px radius and 24px padding (16px on mobile) to enclose its heading, explanation and source together. Remove only the first/last child outer margins; keep internal code scrolling and existing syntax colors. Preserve all content and collapsed enum behavior. Build, run existing tests, inspect desktop/Pixel 5 screenshots and verify containment/overflow. No commit or publication.

Move the existing management-structure heading, explanation and dictionary code out of the domain-enum details into an unstyled domain-structures block immediately after it. Close the enum disclosure after the enum code, and restore its enum-only summary. Preserve every source block and image exactly; no CSS or JS changes. Seven closed code controls and one always-visible structure code block remain. Reading budget is 3,100 characters excluding all code (including the newly visible pre) and gallery, to accommodate the now-visible existing explanation. Build, run the existing tests and verify initial structure visibility, source preservation, enum-independent visibility, keyboard/touch behavior and desktop/Pixel 5 overflow. Open the structures in the visible preview. No game edits, commit or publication.
