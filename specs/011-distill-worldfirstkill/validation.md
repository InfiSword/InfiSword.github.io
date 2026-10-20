# Validation

- Two chapters/four subsections, two four-step flows and five class responsibility rows remain. Main explanatory text excluding code/gallery/whitespace is 1,914 characters.
- Six native source disclosures start closed. Section 2.2 presents token generation, parsing/restoration request and RNG Skip restoration in that order.
- New BuildSeedToken/constants and RestoreUsageCount excerpts match the actual SaveLoadManager source after normalizing whitespace and omitting comments. The original four excerpts were verified in the previous pass. No game sources were edited.
- The original six-image gallery matches HEAD exactly. Four existing images are reused: master meta sheet, monster data and two different seed-specific skill results. Each loads, has descriptive alt text and explicit dimensions, links to its original with a successful HTTP 200 response, and uses safe new-tab attributes.
- npm run build passed. npm test: 74 passed (28.7s).
- Desktop Chromium and Pixel 5: all six controls passed click/tap, Enter, Space and visible-focus checks; code is initially hidden and shown when opened; summary targets are at least 44px high. Contents links are valid and page errors are absent.
- Desktop skill examples use two columns; mobile uses one. Flow steps use four columns on desktop and one on mobile. No horizontal page overflow at 375/768/1024/1440px or Pixel 5 width.
- One-off QA artifacts: test-results/wfk-token/results.json and desktop/mobile screenshots. Batched visual review checked section 2.2, the mobile skill pair, both sheet figures and expanded parsing code. Images retain proportions, captions remain readable and code scrolls internally.
- The test runner's temporary web server ended with the suite. Restarted the local preview server and successfully completed browser QA against it.
- Scoped Impeccable mechanical scan ran once, using limited regex fallback because optional HTML parser modules were unavailable. Its only warning is the existing DESIGN.md Plus Jakarta Sans choice. Unslop scan returned zero findings/score 0. Manual browser review supplements those limited scans.
- The updated local page is visibly open in the in-app browser at section 2.2 and marked as a deliverable: http://127.0.0.1:4173/project/worldfirstkill/#22-token-저장과-skip-복원.
- All changes remain uncommitted. No push or deployment performed.

## Visible token example follow-up

- Added a visible schema, a four-row storage/decoding table and a short parsing-to-Skip explanation before the skill images. Explicitly labels omitted domain rows and decorative separators.
- Compared against the actual GameSeed.Domain enum: 14 domains; Map=001 and Quest=004. The worked seed-42 token has 233 digits (20+3+14*(3+12)). A complete constructed token parses back to Map usage 3 and Quest usage 5, with the remaining 12 usages zero.
- Built successfully and all 74 existing tests passed (28.7s). One-off desktop/Pixel 5 checks confirmed the four displayed field examples, six closed code disclosures and three section 2.2 controls via click/tap, Enter and Space. No page errors or horizontal overflow at 375/768/1024/1440px and Pixel 5 width; numeric badges remain within table cells.
- Inspected desktop and mobile example screenshots. Artifact results: test-results/wfk-token-example/results.json, desktop-example.png and mobile-example.png. Main visible text stays within the 1,600-character limit (1,398 excluding code/gallery/whitespace).
- Scoped scans ran once for this change: the same intentional font warning/limited parser fallback; unslop returned no findings. Updated the visible local preview at section 2.2 and marked it to remain open. No commit, push or publication.

## Interviewer comprehension pass

- Added source-backed first-use explanations for reflection/FieldInfo and Seed/RNG/domain/Token/Skip. The visible CSV example connects Name to Data_Monster.m_monsterName via the verified NormalizeField alias rule. Explains why features use independent RNGs, why the seed alone does not save progress, and the conditions/linear cost of Skip restoration.
- Reduced all six initially closed excerpts to 13/15/9/11/15/10 nonblank lines. Compared each substantive source line to the actual game files; preserves real variable names and statements. Each excerpt explains helpers/inputs and omitted surrounding setup/validation. No game sources were edited.
- Original four sheet/skill figures and six-image gallery retained. Added the original prior Minecraft seed-input illustration in a separate closed reference. Five figures' image/original URLs returned HTTP 200, alt text is present, and no image assets changed. No fabricated benchmarks or full-world restoration claims were added.
- npm run build passed. Existing npm test suite passed 74 tests before and after the reference-focus fix; final suite 74 passed (25.1s). The final wording polish only removes revision-history wording from the hidden reference caption; rebuilt successfully afterward.
- Desktop Chromium and Pixel 5: six code controls and one reference are initially closed and all seven pass click/tap, Enter/Space and visible keyboard focus. Reference's default focus was initially only 1px; shared the existing 2px page-scoped focus rule, then confirmed the complete batch once.
- Body text excluding code/gallery/whitespace is 1,914 characters (within updated 2,300-character reading budget). Two chapters/four subsections, contents links, five image/original URLs and no page errors verified. No page or token-table overflow at 375/768/1024/1440px and Pixel 5 width.
- Inspected one desktop/mobile screenshot batch: CSV reading flow, annotated FieldInfo source, RNG reading flow and original reference. Source screenshots remain proportionate and captions explain what to inspect; expanded code scrolls internally. No further visual changes required. Artifacts: test-results/wfk-interview/results.json and device screenshots.
- Scoped Impeccable/unslop scans ran once: limited regex fallback with only the intentional existing DESIGN.md font warning; unslop returned no findings. No new dependencies, animation or site-wide design changes.
- Local preview refreshed and kept open for the user. No commit, push or publication.

## Reflection rationale follow-up

- Added three short bullets in section 1.2: why a shared parser uses reflection, the benefit of reusing mapping/type conversion, and runtime lookup/assignment cost plus conversion-time name/type checks. Source, figures, structure and CSS are unchanged by this follow-up.
- npm run build passed; npm test: 74 passed (24.2s). Desktop/Pixel 5 QA confirmed all three bullets visible, six disclosures initially closed, the CSV mapping disclosure still works by click/tap and Enter, and no page errors or horizontal overflow. Visible text excluding details/gallery/whitespace is 2,098 characters (under 2,300).
- Inspected desktop reading context and mobile bullet screenshots. No further changes needed. Artifacts: test-results/wfk-reflection/results.json and screenshots. Scoped scanners ran once with the same intentional font warning/limited parser fallback and no unslop findings.
- Refreshed the user's existing in-app preview, verified the three new bullets and marked the tab to stay open. No commit, push or deployment.

## Domain separation rationale follow-up

- Section 2.1 now has a compact definition/mechanism paragraph followed by three bullets: shared-RNG call interference, reproducibility/debugging benefit, and per-domain RNG/usage management plus within-domain call-order and stable-ID/derivation conditions. Removed the repeated causal explanation to keep the addition concise. No code, picture or CSS changes for this follow-up.
- npm run build passed; npm test: 74 passed (29.1s). Desktop/Pixel 5 checks confirmed all three bullets, six initially closed code controls, domain code operation by click/tap, Enter and Space, and no page errors or horizontal overflow. Main visible text is 2,201 characters excluding details/gallery/whitespace (within 2,300).
- Inspected desktop section context and mobile bullet screenshots; no changes needed. Artifacts: test-results/wfk-domain/results.json and screenshots. Scoped scanners ran once: intentional existing font warning with limited parser fallback; no unslop findings.
- Refreshed the existing in-app preview and opened section 2.1; marked it to stay open. No commit, push or publication.

## Semitransparent comment styling

- Added narrowly scoped WorldFirstKill comment CSS using rgba(212,212,212,0.7). Nested highlighted tokens inherit that color; whole-code opacity remains 1 and ordinary syntax colors are preserved. No article/source text or images changed.
- npm run build passed; npm test: 74 passed (26.8s). Desktop/Pixel 5 QA verified 14 comment spans and 7 nested tokens all use the same translucent gray, normal type keywords stay rgb(86,156,214), and composite comment contrast is 6.18:1 against the rgb(30,30,30) code background. Disclosure click/tap, Enter/Space, page errors and horizontal overflow checks passed.
- Inspected desktop/mobile expanded code screenshots in one batch. Scoped scanners ran once with only the existing intentional font warning/limited parser fallback and no unslop findings. Artifacts: test-results/wfk-comments/results.json and device screenshots.
- Refreshed the existing visible preview and left the CSV mapping code expanded so the user can see the new comment treatment. No commit, push or publication.

## Always-visible seed inspiration figure

- Removed the Seed design-origin disclosure wrapper and retained the original image/link/alt/dimensions as a normal figure at the same position. Its title is bold caption text and seed-design-origin is the stable anchor. Removed only the now-unused reference-summary focus selector; six code disclosures and their comment styling remain intact.
- npm run build passed; npm test: 74 passed (25.1s). Desktop/Pixel 5 confirmed the figure is visible without a details ancestor or user expansion, original image loads and original link returns HTTP 200, and all six code controls remain initially closed. Nearby code click/tap, Enter/Space, no page errors and no horizontal overflow passed.
- Visible text excluding code/gallery/whitespace is 2,283 characters (within 2,300). Inspected desktop/mobile figure captures in one batch; no further changes needed. Artifacts: test-results/wfk-visible-seed/results.json and device screenshots.
- Scoped scanners ran once with the same limited parser fallback/intentional font warning and zero unslop findings. Refreshed the existing visible preview to #seed-design-origin and marked it to stay open. No commit, push or publication.

## Corrected seed explanation and usage example

- Verified the user's explanation against actual GameSeed/SaveLoadManager and primary C# / SplitMix64 / System.Random sources. Corrected signed long BaseSeed versus ulong storage encoding, 20 decimal digits versus bit size, SplitMix64-like seed derivation versus actual System.Random generation, and Domain:uint / usage:ulong semantics. Keeps restoration scoped to RNG progress under matching data/logic/implementation.
- Added a clearly hypothetical Map sequence 12,47,8,31 after three consumed values. The two-row comparison shows seed-only replay restarting at 12 and seed+usage=3 replay skipping to 31. It is not represented as measured output from seed 42.
- All six C# excerpt blocks and all 12 image tags match their pre-edit contents. No game source, image, CSS or JS changes were needed. Existing visible seed-origin figure and translucent comments are preserved.
- npm run build passed; npm test: 74 passed (26.5s). Desktop/Pixel 5 checks confirmed corrected type/mix terms, two-row example values, six initially closed code controls and four RNG-related controls via click/tap, Enter and Space. No page errors or page overflow at 375/768/1024/1440px and Pixel 5 width. Main visible text is 2,852 characters (within the updated 3,000 budget).
- Inspected a batched set of desktop/mobile usage examples, storage-type explanation and mixing text. The new primary-source inline link inherited a low-contrast legacy link color, so removed that inline link and retained source citations in research/final reporting. Final build and one confirmation QA batch passed. Artifacts: test-results/wfk-seed-explanation/results.json and device screenshots.
- Scoped scanners ran once: limited parser fallback with intentional existing font warning and zero unslop findings. Refreshed the existing visible preview to #why-save-usage and marked it to stay open. No commit, push or publication.

## SplitMix64-like mixing rationale follow-up

- Section 2.1 keeps the domain separation bullets together and adds a separate seed-derivation group at splitmix-rationale. Three short bullets explain small-input-difference diffusion, stateless deterministic calculation with bit operations/multiplications, and collision/replay compatibility tradeoffs from 31-bit reduction and rule changes. Condensed the repeated mechanism explanation to keep the total main text at 2,980 characters (within 3,000).
- All six C# excerpts remained unchanged. No picture, CSS, JS or game source changes for this follow-up.
- npm run build passed; npm test: 74 passed (26.6s). Desktop/Pixel 5 QA confirmed the new three-bullet group, separate domain rationale, six initially closed code controls, nearby domain code click/tap and Enter/Space, no page errors and no horizontal overflow.
- Inspected desktop/mobile rationale screenshots in one batch; no further changes needed. Artifacts: test-results/wfk-mixing/results.json and device screenshots. Scoped scanners ran once with the existing intentional font warning/limited parser fallback and zero unslop findings.
- Updated the visible in-app preview to #splitmix-rationale and marked it to stay open. No commit, push or publication.

## Actual domain enum declaration

- Added one initially closed code disclosure immediately after section 2.1's Domain:uint explanation. It shows the full actual GameSeed.Domain declaration (18 nonblank lines), all 14 names/IDs and the original category comment. The other six excerpts remained unchanged. Seven code controls total.
- npm run build passed; npm test: 74 passed (26.4s). Desktop/Pixel 5 checks compared the rendered enum to the real source after whitespace normalization and verified all member names/IDs, seven initially closed controls, click/tap, Enter/Space, visible focus, inherited translucent comment color, no page errors and no horizontal page overflow. Main visible text remains 2,980 characters.
- Inspected desktop/mobile expanded enum captures. Existing code formatting and image assets are preserved. Artifacts: test-results/wfk-domain-enum/results.json and device screenshots. Scoped scanners ran once with the same intentional font warning/limited parser fallback and zero unslop findings.
- Updated the existing visible local preview to #domain-enum, expanded the new code and marked the tab to stay open. No game-source changes, commit, push or publication.

## Domain wording and management dictionaries

- Moved the requested Domain/Map/Quest sentence to section 2.1's first paragraph with bold inline identifiers and a separate following RNG/domain definition. Uses integer ID wording rather than the inaccurate signed-int C# type; the real enum is Domain:uint. Removed the redundant initial count sentence; the enum helper retains the total 14.
- In the same domain-enum disclosure, added the actual Dictionary<Domain,System.Random> _domainRng and Dictionary<Domain,ulong> _domainUsageCounts declarations directly below the complete enum, with shared-key/distinct-value explanation. Wrapped assignments only; declarations match the actual source after whitespace normalization. Original seven source blocks were unchanged. Seven controls now contain eight blocks.
- npm run build passed; npm test: 74 passed (29.2s). Desktop/Pixel 5 verified first-paragraph order and bold identifiers, separate definition, exact enum/dictionary source parity, two code blocks in the new control, seven initially closed controls, click/tap, Enter/Space, visible focus, no page errors and no horizontal page overflow. Main visible text is 2,974 characters.
- Inspected desktop combined enum/structures and mobile dictionary captures. Existing formatting and code scrolling are preserved; no further visual changes needed. Artifacts: test-results/wfk-domain-storage/results.json and device screenshots. Scoped scanners ran once with only the intentional existing font warning/limited parser fallback and zero unslop findings.
- Refreshed the visible local preview and left domain-enum expanded, including its two structures. No game-source changes, commit, push or publication.

## Requested wording correction

- Changed only the first Domain paragraph ending from 관리한다 to 관리합니다, preserving the requested bold code identifiers and paragraph placement. Built successfully; 74 tests passed (24.3s). Browser exact-text check and refreshed visible preview confirm the requested wording. Screenshot: test-results/wfk-domain-wording/sentence.png. No commit, push or publication.

## Always-visible domain structures

### Grouped container refinement

- Added a scoped bordered block around the existing heading, explanation and code using established background/border tokens, 12px radius and 24px/16px desktop/mobile padding. Copy and code are unchanged and remain visible independently of the enum.
- Build passed; full suite: 74 passed (25.5s). Desktop/Pixel 5 checks confirmed visible content, computed border/radius, seven closed controls/eight blocks, unchanged enum pointer/keyboard toggles, no page errors and no horizontal page overflow. Inspected both screenshots at test-results/wfk-grouped-structures/. Existing code scrolling remains usable on mobile.
- Scoped scanners ran once: intentional existing font warning/parser fallback only, zero unslop findings. Refreshed and retained the visible local preview. No commit, push or publication.

- Moved the management heading, explanation and dictionary code outside domain-enum into domain-structures. Enum stays initially collapsed with one block. All eight C# fragments are unchanged; no style or runtime changes.
- Build passed; 74 tests passed (26.7s). Desktop and Pixel 5 checks confirmed initially visible structures with no details ancestor, seven collapsed controls, eight blocks, click/tap and Enter/Space toggles that never hide the structures, no page errors and no page-level horizontal overflow. Visible prose is 3,068 characters, excluding code and gallery.
- Inspected both device screenshots; mobile code retains internal horizontal scrolling. Artifacts: test-results/wfk-visible-structures/results.json and desktop/mobile-structures.png. Scoped scans ran once: existing intentional font warning/parser fallback only, zero unslop findings.
- Refreshed the existing visible preview to #domain-structures and retained the tab. No commit, push or publication.

## Visible initialization and concise seed derivation

- Replaced the mixing rationale/pros/cons with a short accurate input-diffusion paragraph and the actual DeriveSubSeed method. The code derives the int seed; System.Random generates game values. Adjusted only its explanatory comment to say lower 31 bits, preserving executable source.
- Moved the unchanged Init/NextInt excerpt out of its disclosure immediately below the structures box. Shared translucent comment styling with the two visible code sections. Six collapsed controls and three visible blocks now contain nine C# blocks.
- Build passed; 74 tests passed (26.3s). Desktop/Pixel 5 checks confirmed source parity ignoring whitespace/comments, direct box-to-initialization ordering, visible code, all six controls via pointer/touch/Enter/Space, translucent comments, no page errors and no page overflow. Visible prose: 3,059 characters. Inspected all four mixing/initialization device screenshots in test-results/wfk-visible-mixing/.
- Scans ran once: existing intentional font warning/parser fallback, no unslop findings. Refreshed and retained the local preview at #splitmix-rationale. No game-source changes, commit, push or publication.

## Requested wording and initialization fold

- Changed +1·-1 to +1이나 -1. Restored domain-initialization as an initially closed native disclosure in the same position directly below the structures box; exact code and annotations are unchanged. Structures and seed derivation stay visible. Seven disclosures, nine total C# blocks.
- Build passed; 74 tests passed (26.0s). Desktop/Pixel 5 checks verified exact wording, initial hidden code, click/tap and Enter/Space opening/closing, preserved comment styling, visible other blocks, no errors and no horizontal overflow. Inspected both screenshots in test-results/wfk-init-fold/ and refreshed the visible preview to the collapsed initialization control. No commit, push or publication.

## Domain separation reason and benefit as prose

- Replaced only the three domain separation bullets with one Korean paragraph connecting shared-RNG interference, per-domain separation and easier map reproduction/debugging. Removed the caution bullet as requested; code and other explanations remain unchanged.
- Build passed; 74 tests passed (26.9s). Desktop/Pixel 5 checks confirmed the new paragraph and absent old caution, preserved seven disclosures/nine blocks, pointer/touch/Enter/Space behavior, no page errors and no page overflow. Inspected both test-results/wfk-domain-prose/ screenshots and refreshed the visible local preview. No commit, push or publication.

## Initialization below DeriveSubSeed

- Moved the unchanged domain-initialization disclosure immediately below splitmix-rationale. Removed the requested shared-RNG reason/benefit paragraph entirely. Nine source blocks and seven initially collapsed controls are preserved.
- Build passed; 74 tests passed (25.3s). Desktop/Pixel 5 checks confirmed direct seed-mixing-to-initialization ordering, absent paragraph, pointer/touch/Enter/Space operation, visible structures/mixing code, no page errors and no horizontal overflow. Inspected both screenshots in test-results/wfk-init-after-mixing/ and refreshed the visible preview. No commit, push, publication or game-source changes.

## Visible separate Init and NextInt methods

- Rewrote initialization/usage explanation around the requested flow while distinguishing DeriveSubSeed sub-seeds from System.Random output. Added domain/range usage and min-inclusive/max-exclusive explanation.
- Unfolded domain-initialization in its existing position after mixing. Replaced the mixed snippet with separately labeled actual Init and NextInt method declarations. Six closed controls and four visible code blocks total ten C# blocks; game source unchanged.
- Build passed; 74 tests passed (28.7s). Desktop/Pixel 5 checks verified both methods against GameSeed.cs after whitespace normalization, two initially visible blocks, order after mixing, all six remaining controls via pointer/touch/Enter/Space, no page errors and no page overflow. Inspected both device screenshots in test-results/wfk-split-init-next/ and refreshed/retained the visible preview. No commit, push or publication.

## Remove range-boundary sentence

Removed only the requested min-inclusive/max-exclusive sentence from the NextInt explanation. Code is unchanged. Build passed; 74 tests passed (25.5s). Desktop/mobile checks and screenshots confirm the shorter paragraph with no overflow, and the visible preview was refreshed. No commit or publication.

## Narrative token generation, parsing and restoration

- Reorganized section2.2 into three labeled, visible groups: token-generation, token-parsing and rng-restoration. Each explains its responsibility immediately before source. Moved the existing token example under generation and hypothetical usage comparison under restoration; consolidated repeated format/Skip descriptions and retained scope/cost once. Five source images and the six-image gallery are unchanged.
- Used complete actual BuildSeedToken and RestoreRNG methods and a parser core including the usage dictionary. Parser length validation and try/catch omissions are explicitly labeled. Three closed controls and seven visible blocks make ten C# blocks, each <=18 nonblank lines. No game-source/CSS/JS changes.
- Final build passed; 74 tests passed (26.9s). Desktop/Pixel5 QA confirmed ordered visible phases, all three source bodies against real source (parser normalized after removing explicitly omitted validation), three closed controls via pointer/touch/Enter/Space, translucent comments, five loaded original figures, six gallery thumbnails and no page errors. No horizontal page overflow on both device classes and375/768/1024px checks. Visible prose is3,083 characters.
- Initial QA corrected its gallery count selector to exclude the separate main image, then surfaced37 excess prose characters. Removed repeated sentences in one refinement batch and confirmed final results at test-results/wfk-token-flow/results.json. Inspected desktop/mobile phase screenshots. Capture-only styles hide the sticky header so labels are legible; actual page styling is unchanged.
- Scoped scans ran once: existing intentional font warning/parser fallback, zero unslop findings. Refreshed and retained the visible preview at #token-generation. Refreshed supporting contracts/model/validation guide to current counts. No commit, push or publication.

## Width constants and core-only token excerpts

- Added visible token-format before generation, showing the actual BASE_SEED_WIDTH20, COUNT_WIDTH3, DOMAIN_ID_WIDTH3 and USAGE_WIDTH12 declarations/comments. Explained decimal storage widths versus type sizes and the paired Pad/ReadUInt use once.
- Shortened generation to10 nonblank core lines (sorted-domain setup omitted), parsing to12 (length/ID checks and exceptions omitted), and restoration to13 (domain loop only). All omissions are labeled. Original examples and five figures/six gallery entries remain. Three disclosures plus eight visible blocks total eleven C# blocks.
- Build passed; 74 tests passed (25.5s). Desktop/Pixel5 QA compared constants and excerpts to real source with only documented omissions removed; verified visible ordered phases, controls via pointer/touch/Enter/Space, muted comments, five loaded figures/six gallery thumbnails, <=18-line blocks, no errors/overflow, including375/768/1024px checks. Visible prose3,068 characters. Inspected the batched format/mobile phase captures in test-results/wfk-token-widths/; capture-only sticky-header hiding leaves live styles unchanged.
- Scoped scans ran once: intentional existing font warning/parser fallback only, zero unslop findings. Refreshed and retained the visible preview at #token-format. No CSS/JS/game-source changes, commit, push or publication.

## Pad/ReadUInt helper examples and editorial cleanup

- Added token-helpers after the width constants, using the actual uint/ulong Pad overloads and ReadUInt body. Examples show Pad(42UL,5)->00042 and sequential00042014 reads42/14 with cursor0->5->8. Widths are introduced once; each helper's behavior is explained once.
- Removed editorial prose saying code was excerpted or checks/setup omitted, including equivalent CSV notices. Kept functional variable descriptions and changed the table coverage note to ordinary example wording. Behavioral comments (e.g. skipping a download) remain. No game-source, CSS or JS changes.
- Build passed; 74 tests passed (25.0s). Final desktop/Pixel5 QA confirmed actual helper and core source parity, both examples, absence of editorial notices, thirteen C# blocks/three controls, pointer/touch/Enter/Space, comment styling, five loaded figures/six gallery thumbnails, no errors and no page overflow, including375/768/1024px. Trimmed helper prose after the first reading-budget check; final visible prose3,082 characters. Artifacts: test-results/wfk-token-helpers/results.json and device screenshots.
- Inspected final desktop/mobile helper screenshots. Scoped scans ran once: existing intentional font warning/parser fallback only, zero unslop findings. Refreshed and retained visible preview at #token-helpers. No commit, push or publication.

## Fold Pad, ReadUInt, Init and NextInt code only

- Wrapped only the four existing source blocks in native initially closed controls with function-specific summaries/IDs. Visible headings, prose/examples, outer anchors and all thirteen C# blocks are unchanged. Seven controls and six always-visible blocks remain.
- Build passed; 74 tests passed (24.7s). Desktop/Pixel5 QA confirmed four code blocks initially hidden with descriptions visible, preserved source, all seven controls via pointer/touch/Enter/Space, helper examples, five loaded figures/six gallery thumbnails, no errors or horizontal overflow (including375/768/1024px). Visible prose remains3,082 characters. Source checks use textContent for hidden code rather than rendered innerText.
- Inspected desktop/mobile helper and initialization screenshots in test-results/wfk-fold-four-functions/. Refreshed and retained the existing preview; all four controls show collapsed. No CSS/JS/game-source edits, commit, push or publication.

## More spacing before section2.2

- Added a scoped adjacent-heading rule: domain-initialization + h3 uses margin-top72px. No source/copy or other section spacing changed.
- Build passed; 74 tests passed (26.0s). Desktop/Pixel5 checks measured72px from the NextInt control to the2.2 heading with code both closed/open, no errors and no page overflow. Inspected both expanded captures in test-results/wfk-section-gap/ and refreshed/retained the visible preview.
- Scans ran once: existing intentional font warning/parser fallback, zero unslop findings. No commit, push or publication.

## ReadUInt example line break

Inserted a line break before the00042014 example, preserving wording and code. Build passed; 74 tests passed (24.7s). Desktop/mobile confirmed the break and no overflow; screenshots are in test-results/wfk-helper-linebreak/. Refreshed the visible preview. No commit or publication.

## ReadUInt purpose and sequential-read example

- Replaced the terse cursor-first explanation with the function's purpose: extract one numeric field from a token and retain the next read position. Defined idx/width and explained ref updates the caller's position. A two-row shortened00042014 example separates00042->42 /014->14 from next positions5/8. Helper code remains unchanged and collapsed.
- Kept table labels short for mobile and raised the prose allowance to3,250 for this clarification; final visible prose3,226 characters. Build passed; full suite74 passed (28.9s). Final desktop/Pixel5 source/example/disclosure/image/overflow checks passed, including375/768/1024px checks. Inspected device tables at test-results/wfk-readuint-purpose/. Core source and other figures/layout remain intact.
- The test-owned preview server ended after the full suite; restarted the existing local serve command and completed final browser checks. Scans ran once: intentional existing font warning/parser fallback, zero unslop findings. No game edits, commit, push or publication.

## ReadUInt aligned with actual token widths

- Replaced the shortened5/3-digit example with the actual seed42/count14 prefix00000000000000000042014. The two-column table reads seed20 digits ->42, idx0->20; count3 digits ->14, idx20->23. It explicitly references the same generation example, then continues with repeated domain ID3/usage12 fields. Reused existing token-table styling to fit the20-digit value on mobile.
- Updated the generation note per request: | marks domain ID versus usage only; all14 domain records are ID-sorted and stored in one separator-free string ordered seed/domain count/[domain ID/usage]. Used domain count rather than an ambiguous domain field name. Source and disclosures are unchanged.
- Build passed; full suite74 passed (24.9s). Final desktop/Pixel5 QA confirmed actual widths, parsed values and positions0/20/23, alignment with generation, source parity, all seven controls via pointer/touch/Enter/Space, five figures/six gallery entries and no errors/overflow, including375/768/1024px. Final prose3,348 characters within the revised3,350 allowance. Corrected a QA assertion to compare the old short example exactly rather than reject a suffix of the new prefix.
- Inspected final desktop/mobile ReadUInt tables and generation note in test-results/wfk-real-token-read/. Scans ran once: intentional existing font warning/parser fallback only, zero unslop findings. Refreshed and retained preview. No game-source/CSS/JS changes, commit, push or publication.

## Total length below token constants

Added the visible20+3+14*(3+12)=233-digit formula directly below the four width constants. Source and behavior remain unchanged. Build passed; 74 tests passed (25.5s). Desktop/mobile formula and overflow checks passed; inspected screenshots at test-results/wfk-token-total/ and refreshed the visible preview. Prose allowance3,400 accommodates this requested formula. No commit, push or publication.

## Current-game total-length explanation

Expanded the total below width constants into the current-game breakdown:14 domains,20 seed digits+3 count digits+14 records of3 ID+12 usage digits=233; each domain15 digits, records210 total. Zero-usage domains are still included. Code is unchanged. Build passed; 74 tests passed (25.2s). Desktop/mobile checked the actual GameSeed enum has14 members, visible breakdown/formula and no overflow; inspected screenshots in test-results/wfk-current-game-length/ and refreshed the preview. Prose allowance3,500 accommodates the requested breakdown. No commit, push or publication.

## Helper roles integrated into generation, parsing and restoration

- Removed standalone token-helpers, both helper headings/definitions, the toy5-digit Pad example and both helper code disclosures. Added Pad's string/zero-fill/storage-width purpose to generation; added ReadUInt slice/parse/ref-cursor explanation to parsing. Moved the unchanged actual20/3 read table into parsing under the retained readuint-explanation anchor. Restoration now explicitly starts with the parsed seed/usage and explains Skip plus restored counters.
- Kept the233-digit current-game calculation, core phase code, all other source and original figures unchanged. Five closed controls and six visible blocks make eleven C# blocks; Init/NextInt remain folded. No CSS/JS/game-source changes.
- Build passed; 74 tests passed (30.0s). Desktop/Pixel5 QA confirmed standalone helper removal, phase-specific roles/order, actual-width table inside parsing, source parity, all five controls via pointer/touch/Enter/Space, five original figures/six gallery entries, no errors/overflow including375/768/1024px checks. Visible prose3,394 characters. Inspected desktop/mobile generation/parsing/restoration screenshots in test-results/wfk-integrated-helpers/.
- Scoped scans ran once: intentional existing font warning/parser fallback only, zero unslop findings. Refreshed and retained the visible preview at #token-generation. No commit, push or publication.

## Plain-language saved randomness explanation

- Replaced all visible Skip terminology with concrete draw/discard/resume wording. Renamed2.2 to 토큰 저장과 난수 이어 쓰기 and the third phase to 난수 이어 쓰기. Updated the flow, responsibility map, seed caption and hypothetical example label consistently.
- Simplified nearby encoding/consumption/input/cursor/cost wording. The explanation now starts with re-creating the generator from the loaded seed, re-drawing/discarding previously used values, restoring the counter and using the next value. Same-data/generation conditions, separate health/position saving and increased replay work remain explicit in ordinary language. All eleven C# blocks are unchanged.
- Build passed; 74 tests passed (26.1s). Desktop/Pixel5 QA confirmed no visible Skip, the new contents link reaches the correct heading, examples/source parity, five controls via pointer/touch/Enter/Space, five original figures/six gallery entries and no errors/overflow, including375/768/1024px. Visible prose3,367 characters. Inspected device continuation screenshots and desktop parsing in test-results/wfk-plain-rng/.
- Scoped scans ran once: intentional existing font warning/parser fallback only, zero unslop findings. Refreshed and retained the visible preview at #rng-restoration. No game-source/CSS/JS changes, commit, push or publication.

## Requested skill-image introduction wording

Replaced only the paragraph before the two skill images with the user's requested representative-skill-image wording. Images/code are unchanged. Build passed; 74 tests passed (32.0s). Desktop/mobile verified exact wording and both image entries, no overflow; inspected captures in test-results/wfk-skill-caption/ and refreshed the existing report preview without touching the user's image tab. No commit, push or publication.

### Boxed skill comparison and prior seed-input screenshot — 2026-10-20
- Preserved the requested introduction, both raw skill screenshots and captions; grouped them with the same token-based border/background and 24px/16px padding as domain-structures. Desktop two columns, Pixel 5 one column.
- Reused the actual-game seed-input screen InGame/wfk_ingame_06.png near the Minecraft reference, maximum width 480px, original link and accurate caption. No image bytes changed. Six inline figures and six gallery entries remain.
- Build passed. Existing suite: 74 passed (27.5s). Targeted desktop/Pixel 5 checks confirmed all six images load, full-size links and keyboard focus, five initially closed source controls with pointer/touch/Enter/Space, eleven code blocks, 3407 prose characters and no page errors or horizontal overflow (also 375/768/1024px).
- Reviewed desktop/mobile screenshots for both the comparison and seed screen. Evidence: test-results/wfk-comparison-box/results.json and matching PNG files. Targeted check initially expected an absolute image attribute; adjusted to the builder's valid relative asset paths and reran successfully.
- Impeccable scan used degraded regex fallback (optional parsers unavailable), with only the established Plus Jakarta Sans font warning. Unslop scan: no findings. No dependencies added.
- Local preview refreshed at #skill-seed-comparison. No commit, push, deployment or game-source edit.

### Fold three token-phase sources and rename restoration — 2026-10-20
- Renamed only the third phase to 3. 난수 상태 복구 — RestoreRNG. Added separate initially closed native details for BuildSeedToken, RestoreUsageCount and RestoreRNG; retained visible explanations, tables and existing section anchors.
- Snapshot comparison confirms all eleven C# blocks unchanged. Eight disclosures now start closed, with three other source blocks always visible. No CSS, dependencies, images, gallery or game changes.
- Build passed; existing suite: 74 passed (28.7s). Desktop/Pixel 5 checks passed all eight controls via click/tap, Enter and Space; phase tables remain outside details and visible. No page errors or horizontal overflow (including 375/768/1024px). Six inline figures and six gallery entries preserved.
- Batched desktop/mobile screenshot review completed. Evidence: test-results/wfk-token-folding/results.json, before-code.json and closed/open PNGs. No visual styling changed, so existing native disclosure pattern was retained without additional design changes.
- Local preview refreshed. No commit, push or publication.

### Explain token phases while source stays closed — 2026-10-20
- Rewrote visible generation/parsing/restoration prose around input, processing and output. Explained zero usage fallback and progress-only storage, reverse long/ulong conversion and ID-to-Domain usage reconstruction, per-domain generator recreation and restoring cumulative counters for subsequent saves. Existing tables/examples are preserved rather than repeated.
- All eleven code blocks match the prior snapshot. Eight controls remain initially collapsed and three source blocks remain visible; imagery and gallery unchanged. No CSS or game-source edit.
- Build passed. Existing suite: 74 passed (26.1s). Targeted desktop/Pixel 5 checks confirmed explanations outside disclosures, all eight controls via click/tap/Enter/Space, no page errors or overflow at device widths and 375/768/1024px. Visible prose 3678 characters within the updated requested-explanation budget of 3700.
- Reviewed desktop/mobile phase screenshots together. Evidence: test-results/wfk-token-narrative/results.json and phase PNGs. No additional visual styling introduced.
- Local preview refreshed; no commit, push or publication.

### Include Pad and ReadUInt inside calling code fences — 2026-10-20
- Appended the actual Pad(uint/ulong) definitions inside the BuildSeedToken code fence and actual ReadUInt inside RestoreUsageCount, separated by a short comment. Updated the two existing summaries to name both caller/helper. No separate helper section or extra control.
- Whitespace-normalized parity with actual SaveLoadManager.cs passed for all helper definitions. All pre-existing caller statements remain unchanged; eight initially closed controls, three visible fences and eleven total fences remain.
- Build passed. Existing suite: 74 passed (26.9s). Desktop/Pixel 5 targeted checks passed all controls via click/tap/Enter/Space, visible explanations/tables, helper colocation, no page errors and no horizontal overflow (also 375/768/1024px). Visible prose unchanged at 3678 characters.
- Reviewed desktop/mobile open-helper screenshots together. Evidence: test-results/wfk-token-inline-helpers/results.json and PNGs. No CSS, image, gallery, dependency or game-source changes.
- Preview refreshed; no commit, push or publication.

### Explain Pad and ReadUInt within code comments — 2026-10-20
- Added Korean comments within the existing helper code fences: Pad zero-fills to width with Pad(42UL,20) and overload responsibilities; ReadUInt reads a width-sized substring, updates the caller cursor through ref and parses a number. Real token example reads seed/count42/14 at cursor0->20->23.
- Helper and caller implementations match prior/actual sources when comments are ignored. No changes to outside prose, control count, CSS, image or gallery behavior.
- Build passed. Existing suite: 74 passed (25.1s). Desktop/Pixel 5 targeted checks passed helper examples, computed comment color rgba(212,212,212,0.7), all eight controls via click/tap/Enter/Space, no page errors or page overflow (also 375/768/1024px). Eleven code fences, three always visible; prose remains3678 characters.
- Batched desktop/mobile open-code screenshot review completed. Evidence: test-results/wfk-helper-comments/results.json and PNGs. Preview refreshed. No commit, push, publication or game-source changes.

### Helper explanation as prose inside disclosure — 2026-10-20
- Put Pad and ReadUInt descriptions and worked examples in ordinary paragraphs inside each existing disclosure body, immediately before its code block. Removed detailed comment examples; retained short helper labels and ReadUInt extraction/cursor/parse annotations.
- Existing helper/caller statements match actual/prior code ignoring comments. No external prose, CSS, image, gallery, dependency or game-source changes.
- Build passed; existing suite: 74 passed (24.9s). Desktop/Pixel 5 checks confirmed helper prose is outside pre, inside details and visible with opened source; all eight controls operate via click/tap/Enter/Space. Comment color remains rgba(212,212,212,0.7). No page errors or page overflow including 375/768/1024px.
- Reviewed desktop/mobile open code screenshots together. Evidence: test-results/wfk-helper-prose/results.json and PNGs. Preview refreshed; no commit, push or publication.

### Always-visible FieldInfo introduction — 2026-10-20
- Inserted reflection-fieldinfo after the 1.2 reflection introduction, with .NET/System.Reflection identity, Name/FieldType and GetValue/SetValue roles, GetField/GetFields-to-FieldInfo[] explanation and a compact always-visible usage example.
- Confirmed actual Data_Monster public string m_monsterName declaration in UnitData.cs and reflection API behavior against Microsoft Learn FieldInfo/FieldType/SetValue documentation. Linked official FieldInfo reference in the article. The example is labeled as usage, not the class implementation or actual parser excerpt.
- Preserved all previous caller code, source helpers, eight initially collapsed controls, six inline figures and six-image gallery. Twelve source fences, four always visible. No CSS/game/dependency change.
- Build passed. Existing suite: 74 passed (24.5s). Desktop/Pixel 5 checks confirmed FieldInfo prose/source always visible outside details, correct official reference, existing interactions and no page errors or overflow at device widths and375/768/1024px. Prose3921 within4000 characters.
- Reviewed desktop/mobile FieldInfo screenshots together. Evidence: test-results/wfk-fieldinfo/results.json and desktop/mobile-fieldinfo.png. Local preview refreshed; no commit, push or publication.

### Fold DeriveSubSeed only — 2026-10-20
- Wrapped the unchanged DeriveSubSeed fence in initially closed seed-derivation-code native details. Existing splitmix-rationale explanation stays visible; no CSS, copy, helper, image, gallery or game edits.
- Build passed; existing suite: 74 passed (24.4s). Targeted desktop/Pixel 5 checks passed source parity, new closed/open state and all nine disclosures via click/tap/Enter/Space. FieldInfo remains visible. Twelve fences, three always visible, prose3921, no page errors/overflow at device widths and375/768/1024px.
- Reviewed desktop/mobile closed and open screenshots. Evidence: test-results/wfk-fold-derivation/results.json and PNGs. Preview refreshed. No commit, push or publication.

### Concise FieldInfo documentation reference — 2026-10-20
- Replaced the expanded FieldInfo explanation and illustrative usage code with one short visible paragraph retaining the exact FieldInfo[] fields array definition and official Microsoft documentation link. Preserved reflection-fieldinfo anchor and surrounding CSV content.
- Build passed. Existing suite: 74 passed. Targeted desktop/Pixel 5 checks confirmed retained array text/link, absence of the detailed class heading/example, nine closed controls, eleven source fences/two visible, six inline images/gallery entries and no page errors/overflow at device widths and375/768/1024px.
- Reviewed both screenshots. Evidence: test-results/wfk-fieldinfo-reference/results.json and desktop/mobile.png. No CSS, dependencies, game-source change, commit, push or publication. Local preview refreshed.

### Box FieldInfo adoption rationale — 2026-10-20
- Wrapped the unchanged reason/benefit/caution bullets in an accessible visible section titled FieldInfo를 채택한 이유. Reused the established domain/comparison box styles via wfk-rationale, including24px desktop/16px mobile padding. No code, image, gallery or game-source changes.
- Build passed. Existing suite: 74 passed (27.0s). Targeted desktop/Pixel 5 checks confirmed exact three bullet texts, title, no disclosure, matching border/background/padding, nine initially closed code controls, eleven code fences, preserved reference and no page errors/overflow at device widths and375/768/1024px.
- Batched screenshot review identified the first captures during the existing section entrance effect; final confirmation waits for opacity1 before capture. Both final desktop/mobile screenshots reviewed. Evidence: test-results/wfk-fieldinfo-rationale/results.json and desktop/mobile.png.
- Impeccable regex fallback reported only established Plus Jakarta Sans warning; optional parser modules unavailable. Unslop scan: no findings. No dependencies added.
- Local preview refreshed. No commit, push or publication.

### Master metadata caption and access-code audit — 2026-10-20
- Replaced only the master-sheet caption with comma-separated data type, version and download URL wording plus per-row Link access. Removed the click-original instruction while preserving the image/full-size link.
- Current article1.1 contains cache judgment code and prose describing the flow, but does not show master loading, Link extraction/storage or HTTP download code. Actual ServerCSV_ConvertData.cs: DownloadMetaSheetAsync line127 downloads MasterSheetURL at131; Link is columns[3] at191 and stored in csvURLDict/attribute dictionaries at207-233. DownloadAllCSVAsync365 iterates URLs401-417 and downloads at485 as needed. DownloadFileAsync621 uses UnityWebRequest.Get625 and SendWebRequest629. These fetch CSV content rather than opening spreadsheet pages in a browser. No article code added.
- Build passed; existing suite: 74 passed. Desktop/Pixel 5 targeted checks passed new caption, removed instruction, preserved original-image link, unchanged11 code fences/nine controls, no page errors or page overflow at device widths and375/768/1024px.
- Reviewed both caption screenshots. Evidence: test-results/wfk-master-caption/results.json and desktop/mobile.png. Preview refreshed; no commit, push, publication or game-source changes.

### Add collapsible master-sheet and per-Link CSV flow — 2026-10-20
- Added master-csv-download after the meta-sheet figure and before existing cache code. One initially closed native disclosure contains three prose/code stages: MasterSheetURL CSV retrieval and Sheet/Attribute/Link registration, registered URL traversal with file-specific needsDownload, and shared async UnityWebRequest completion/success handling. Explained the cache flag via existing following control without duplicating its implementation.
- Confirmed actual master URL and essential read/register/download/request/save/version statements in ServerCSV_ConvertData.cs. Existing caller code matches the previous snapshot; game source unchanged. Ten disclosures, fourteen source fences, two always visible. Images, gallery and other report sections preserved.
- Build passed. Existing suite: 74 passed (31.6s). Targeted desktop/Pixel 5 checks passed default closed state, all ten controls via click/tap/Enter/Space, all three explanations inside the new disclosure, source references, prior-code preservation and no duplicated cache source. No page errors or page overflow including375/768/1024px.
- Batched desktop/mobile open/closed screenshot review completed. Evidence: test-results/wfk-master-download/results.json and PNGs. No CSS/dependency changes. Preview refreshed; no commit, push or publication.

### Nested csvURLDict declaration/example and shorter download narrative — 2026-10-20
- Added csv-url-structure within existing master-csv-download: real Dictionary<CSVDataType,Dictionary<string,string>> declaration, outer enum/inner Attribute/Link explanation, two-row Unit/Monster and Skill/Active example, and indexed lookup. Example Link values are explicitly descriptive address labels.
- Confirmed declaration against ServerCSV_ConvertData.cs and example names against Define.CSVDataType, Unit_Attribute.Monster and Skill_Attribute.Active. Shortened each of the three stage descriptions while preserving original core code and cache linkage.
- Build passed; existing suite: 74 passed (34.3s). Desktop/Pixel5 checks passed declaration parity, example mappings, structure inside the same disclosure, shortened prose, preserved previous source, all ten controls via click/tap/Enter/Space and no page errors or page overflow including375/768/1024px. Fifteen source fences, two always visible; no new control.
- Reviewed desktop/mobile structure screenshots together. Evidence: test-results/wfk-csv-url-structure/results.json and structure PNGs. No CSS, dependency, image, gallery or game changes. Preview refreshed; no commit, push or publication.

### Reduce download details to core data flow — 2026-10-20
- Removed the three method-by-method download sections and their source fences from master-csv-download. Replaced them with one short Link -> each CSV download -> C# object conversion/game registration paragraph. Retained csvURLDict declaration, nested explanation/table, lookup expression and separate cache example.
- Unrelated source matches prior snapshot. Ten native disclosures, twelve total fences, two always visible. No CSS, image, dependency or game-source change.
- Build passed; existing suite: 74 passed (29.4s). Desktop/Pixel5 targeted checks passed removed method sections, retained declaration/example and data-flow prose, all ten controls via click/tap/Enter/Space, no page errors or overflow including375/768/1024px.
- Reviewed desktop/mobile open-box screenshots. Evidence: test-results/wfk-download-flow-only/results.json and desktop/mobile.png. Preview refreshed; no commit, push or publication.

### Always-visible master-sheet structure and title — 2026-10-20
- Replaced only the master-csv-download native details/summary with an unboxed wfk-source div and visible title 마스터 시트와 CSV 주소 관리 구조. Preserved the stable anchor, existing flow prose, declaration, mapping table and lookup expression. No new numbered chapter/TOC entry or CSS change.
- Build passed; existing suite: 74 passed (24.6s). Desktop/Pixel5 checks confirmed no disclosure around this content, new title, visible source/table, preserved existing code/examples, all nine remaining controls via click/tap/Enter/Space and no page errors/overflow including375/768/1024px. Twelve code fences, three always visible.
- Visible prose is4058 characters because the existing description/table are now unfolded; requested visibility is reflected in a4200-character documentation budget. No added explanatory copy apart from the title.
- Reviewed desktop/mobile screenshots. Evidence: test-results/wfk-visible-master-structure/results.json and desktop/mobile.png. Preview refreshed; no commit, push, publication, dependency, image, gallery or game changes.

### Box the always-visible master-sheet structure — 2026-10-20
- Added existing wfk-rationale class to the wfk-source/master-csv-download wrapper. All inner title/prose/code/table/lookup content preserved; no disclosure or CSS change. Matches existing domain-structures border/background and24px desktop/16px mobile padding.
- Build passed; existing suite: 74 passed. Desktop/Pixel5 targeted checks confirmed always-visible content, style parity, child containment, preserved source/examples, remaining nine controls and no page errors/overflow including375/768/1024px. Twelve source fences, three visible, prose4058 unchanged.
- Reviewed desktop/mobile box screenshots together. Evidence: test-results/wfk-boxed-master-structure/results.json and desktop/mobile.png. Preview refreshed; no commit, push, publication, dependency, image, gallery or game change.

### Explicit outer enum and inner string-to-URL dictionary prose — 2026-10-20
- Replaced the short dictionary description with concrete Skill/Unit outer enum-key examples and explained that csvURLDict[CSVDataType.Unit] contains Dictionary<string,string>. Explained Monster as inner string key and its CSV URL as value, with matching Skill/Active pattern. Preserved declaration, table, lookup, box and all other source/content.
- Build passed; existing suite: 74 passed (30.8s). Desktop/Pixel5 checks passed explicit key/value prose, unchanged sources/examples and box style/containment, all nine remaining disclosures, no page errors or overflow including375/768/1024px. Visible prose4226 within the updated4300-character budget for this requested clarification.
- Reviewed desktop/mobile screenshots. Evidence: test-results/wfk-explicit-csv-dictionary/results.json and desktop/mobile.png. No CSS, dependency, image, gallery or game changes. Preview refreshed; no commit, push or publication.

### Remove CSV mapping table only — 2026-10-20
- Removed the requested two-row Unit/Monster and Skill/Active table and its caption. Preserved explanatory prose, declaration, lookup expression and always-visible box.
- Build passed; existing suite: 74 passed (22.8s). Desktop/Pixel5 targeted checks and screenshot review confirmed table/caption absent, source/lookup visible and no overflow. Evidence: test-results/wfk-remove-url-table/results.json and PNGs.
- No CSS, game, dependency, image or gallery change. Preview refreshed; no commit, push or publication.

### Concise CSV mapping wording and declaration without initializer — 2026-10-20
- Replaced outer/inner/2-stage dictionary labels with a short 데이터 종류별 항목 이름 -> CSV URL explanation using Unit/Monster and Skill/Active. Subheading now reads csvURLDict — 데이터 종류와 항목별 CSV 주소 관리.
- Removed only the new-expression initializer from the portfolio declaration. Retained nested type, lookup expression, surrounding box and other content; game implementation unchanged.
- Build passed; existing suite: 74 passed (25.6s). Desktop/Pixel5 targeted checks and screenshot review confirmed unwanted labels/initializer absent, concise relationship and declaration/lookup preserved, no overflow or page errors. Evidence: test-results/wfk-csv-wording/results.json and PNGs.
- Preview refreshed; no commit, push, publication, dependency, image or gallery change.

### Remove redundant CSV URL lookup sentence — 2026-10-20
- Removed only the trailing csvURLDict[CSVDataType.Unit]["Monster"] example sentence. Preserved preceding explanation, declaration and visible box; unrelated project pages unchanged.
- Build passed; existing suite: 74 passed (24.3s). Desktop/Pixel5 checks and screenshot review confirmed sentence absent, prior prose/declaration preserved, no overflow. Evidence: test-results/wfk-remove-lookup/results.json and PNGs.
- User's current WinAPI preview navigation was preserved. No commit, push, publication, CSS, game, dependency, image or gallery change.
